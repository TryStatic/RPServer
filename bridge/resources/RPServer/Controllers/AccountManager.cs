using System;
using GTANetworkAPI;
using RPServer.Models;
using RPServer.Strings;
using RPServer.Util;
using EventNames;
using static RPServer.Models.Savable;
using static RPServer.Util.DataValidator;
using Task = System.Threading.Tasks.Task;

namespace RPServer.Controllers
{
    internal class AccountManager : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Client client)
        {
            client.SendChatMessage(AccountStrings.InfoWelcome);
            SetLoginState(client, true);
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(Client player, DisconnectionType type, string reason)
        {
            if (!player.IsLoggedIn()) return;

            var acc = player.GetAccountData();
            Task.Run(async() => await acc.SaveAsync());
            player.Logout();

        }


        [Command("toggletwofactoremail")]
        public void CMD_ToggleTwoFactorEmail(Client client)
        {
            if (!client.IsLoggedIn()) return;
            var acc = client.GetAccountData();
            acc.HasEnabledTwoStepByEmail = !acc.HasEnabledTwoStepByEmail;
            client.SendChatMessage($"2FA by Email has been {acc.HasEnabledTwoStepByEmail}");
        }

        [Command("toggletwofactorga")]
        public void CMD_ToggleTwoFactorGA(Client client)
        {
            if (!client.IsLoggedIn()) return;

            var acc = client.GetAccountData();
            if (!acc.Is2FAbyGAEnabled())
            {
                var key = GoogleAuthenticator.GenerateTwoFactorGASharedKey();
                var link = GoogleAuthenticator.GetGQCodeImageLink(acc.Username, key, 150, 150);

                client.TriggerEvent(ServerToClient.ShowQRCode, link);
                Console.WriteLine(link.Length);
                if (acc.TempTwoFactorGASharedKey == null) acc.TempTwoFactorGASharedKey = new byte[50];
                acc.TempTwoFactorGASharedKey = key;
            }
            else
            {
                acc.TwoFactorGASharedKey = null;
                Task.Run(() => acc.SaveSingleAsync(Column.TwoFactorGASharedKey));
            }

        }

        [Command("logout")]
        public void Cmd_Logout(Client player)
        {
            if (!player.IsLoggedIn())
            {
                player.SendChatMessage("You are not logged in.");
                return;
            }
            var acc = player.GetAccountData();
            Task.Run(async () => await acc.SaveAsync());

            player.Logout();
            player.SendChatMessage("Bye!");
            SetLoginState(player, true);
        }


        [RemoteEvent(ClientToServer.SubmitRegisterAccount)]
        public void ClientEvent_OnSubmitRegisterAccount(Client client, string username, string emailAddress, string password)
        {
            if (client.IsLoggedIn())
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorPlayerAlreadyLoggedIn);
                return;
            }
            if (!ValidateString(ValidationStrings.Username, username))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorUsernameInvalid);
                return;
            }
            if (!ValidateString(ValidationStrings.Password, password))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorPasswordInvalid);
                return;
            }
            if (!ValidateString(ValidationStrings.EmailAddress, emailAddress))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorEmailInvalid);
                return;
            }

            Task.Run(async () => await OnRegisterNewAccountAsync(client, username, emailAddress, password));
        }
        public static async Task OnRegisterNewAccountAsync(Client client, string username, string emailAddress, string password)
        {
            if (await Account.ExistsAsync(username))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorUsernameTaken);
                return;
            }
            if (await Account.IsEmailTakenAsync(emailAddress))
            { // Another account with the that email address
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorEmailTaken);
                return;
            }
            if (await EmailToken.IsEmailTakenAsync(emailAddress))
            { // Another account in the list of email tokens with that address
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorEmailTokenAddressTaken);
                return;
            }

            var newAcc = await Account.CreateAsync(username, password, client.SocialClubName);
            await EmailToken.CreateAsync(newAcc, emailAddress);
            await EmailToken.SendEmail(newAcc);

            client.TriggerEvent(ServerToClient.RegistrationSuccess, AccountStrings.SuccessRegistration);
        }

        [RemoteEvent(ClientToServer.SubmitLoginAccount)]
        public void ClientEvent_OnSubmitLoginAccount(Client client, string username, string password)
        {
            if (client.IsLoggedIn())
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorPlayerAlreadyLoggedIn);
                return;
            }
            if (!ValidateString(ValidationStrings.Username, username))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorUsernameInvalid);
                return;
            }
            if (!ValidateString(ValidationStrings.Password, password))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorPasswordInvalid);
                return;
            }

            Task.Run(async () => await OnLoginAccountAsync(client, username, password));
        }
        public static async Task OnLoginAccountAsync(Client client, string username, string password)
        {
            if (!await Account.ExistsAsync(username))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorUsernameNotExist);
                return;
            }
            if (!await Account.AuthenticateAsync(username, password))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorInvalidCredentials);
                return;
            }

            var fetchedAcc = await Account.FetchAsync(username);

            if (IsAccountLoggedIn(fetchedAcc))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorAccountAlreadyLoggedIn);
                return;
            }

            await LoginAccount(fetchedAcc, client);

            if (!fetchedAcc.HasVerifiedEmail())
            {
                client.TriggerEvent(ServerToClient.ShowInitialEmailVerification);
                return;
            }
            if (fetchedAcc.Is2FAbyEmailEnabled())
            {
                fetchedAcc.HasPassedTwoStepByEmail = false;
                await EmailToken.CreateAsync(fetchedAcc, fetchedAcc.EmailAddress);
                await EmailToken.SendEmail(fetchedAcc);
                client.TriggerEvent(ServerToClient.Show2FAbyEmailAddress);
                return;
            }
            if (fetchedAcc.Is2FAbyGAEnabled())
            {
                fetchedAcc.HasPassedTwoStepByGA = false;
                return;
            }

            SetLoginState(client, false);
        }

        [RemoteEvent(ClientToServer.SubmitEmailToken)]
        public void ClientEvent_OnSubmitEmailToken(Client client, string token)
        {
            if (!client.IsLoggedIn())
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.EmailVerificationCode, token))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorInvalidVerificationCode);
                return;
            }

            Task.Run(async () => await OnVerifyTwoStepByEmailAsync(client, token));
        }
        public static async Task OnVerifyTwoStepByEmailAsync(Client client, string providedEmailToken)
        {
            var accData = client.GetAccountData();

            if (!accData.HasVerifiedEmail()) throw new Exception("Tried to verify Two-Step by Email when user has no email set"); // Dummy check

            if (!accData.Is2FAbyEmailEnabled())
            {
                client.TriggerEvent(ServerToClient.DisplayError, "2FA by EMAIL is not enabled for this account.");
                return;
            }

            if (!await EmailToken.ValidateAsync(accData, providedEmailToken))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorInvalidVerificationCode);
                return;
            }

            accData.HasPassedTwoStepByEmail = true;

            if (accData.Is2FAbyGAEnabled() && !accData.HasPassedTwoStepByGA)
            {
                client.TriggerEvent(ServerToClient.Show2FAbyGoogleAuth, "Need to Verify 2FA by GA");
                return;
            }
            SetLoginState(client, false);
        }

        [RemoteEvent(ClientToServer.SubmitGoogleAuthCode)]
        public void ClientEvent_OnSubmitGoogleAuthCode(Client client, string token)
        {
            var accountData = client.GetAccountData();

            if (!client.IsLoggedIn())
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }
            if (!ValidateString(ValidationStrings.GoogleAuthenticatorCode, token))
            {
                client.TriggerEvent(ServerToClient.DisplayError, "Must be 6 characters and only digits");
                return;
            }
            if (!accountData.HasVerifiedEmail())
                throw new Exception("Tried to verify Two-Step by GA when user has no email set"); // Dummy check

            if (!accountData.Is2FAbyGAEnabled())
            {
                client.TriggerEvent(ServerToClient.DisplayError, "Two Step auth by GA is not enabled for this account.");
                return;
            }
            if (GoogleAuthenticator.GeneratePin(accountData.TwoFactorGASharedKey) != token)
            {
                client.TriggerEvent(ServerToClient.DisplayError, "Wrong 2FA code, try again");
                return;
            }

            accountData.HasPassedTwoStepByGA = true;

            if (accountData.Is2FAbyEmailEnabled() && !accountData.HasPassedTwoStepByEmail)
            {
                client.TriggerEvent(ServerToClient.Show2FAbyEmailAddress);
                return;
            }

            SetLoginState(client, false);
        }


        [RemoteEvent(ClientToServer.SubmitFirstEmailToken)]
        public void ClientEvent_OnSubmitFirstEmailToken(Client client, string providedToken)
        {
            if (!client.IsLoggedIn())
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }
            if (!ValidateString(ValidationStrings.EmailVerificationCode, providedToken))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorInvalidVerificationCode);
                return;
            }
            if (client.GetAccountData().HasVerifiedEmail())
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorEmailAlreadyVerified);
                return;
            }

            Task.Run(async () => await OnVerifyEmailAsync(client, providedToken));
        }
        public static async Task OnVerifyEmailAsync(Client client, string providedToken)
        {
            var accData = client.GetAccountData();
            var accToken = await EmailToken.FetchAsync(accData);
            var accEmail = accToken.EmailAddress;

            if (!await EmailToken.ValidateAsync(accData, providedToken))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorInvalidVerificationCode);
                return;
            }

            accData.EmailAddress = accEmail;
            await accData.SaveSingleAsync(Column.EmailAddress);
            client.SendChatMessage(AccountStrings.SuccessEmailVerification);

            SetLoginState(client, false);
        }

        [RemoteEvent(ClientToServer.SubmitNewVerificationEmail)]
        public void ClientEvent_OnSubmitNewVerificationEmail(Client client, string newEmail)
        {
            if (!client.IsLoggedIn())
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.EmailAddress, newEmail))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorEmailInvalid);
                return;
            }

            if (client.GetAccountData().HasVerifiedEmail())
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorEmailAlreadyVerified);
                return;
            }

            Task.Run(async () => await OnChangeVerificationEmailAsync(client, newEmail));
        }
        public static async Task OnChangeVerificationEmailAsync(Client client, string newEmail)
        {
            if (await Account.IsEmailTakenAsync(newEmail))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorEmailTaken);
                return;
            }
            if (await EmailToken.IsEmailTakenAsync(newEmail))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorEmailTokenAddressTaken);
                return;
            }

            var accData = client.GetAccountData();
            var accTok = await EmailToken.FetchAsync(accData);

            if (accTok != null)
            {
                if (accTok.EmailAddress == newEmail)
                {
                    client.TriggerEvent(ServerToClient.ShowChangeEmailAddress);
                    client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorChangeVerificationEmailDuplicate);
                    return;
                }
                await EmailToken.ChangeEmailAsync(client.GetAccountData(), newEmail);
                await EmailToken.SendEmail(client.GetAccountData());
            }
            else
            { // Handles the case where there's no token entry in the database
                await EmailToken.CreateAsync(accData, newEmail);
                await EmailToken.SendEmail(accData);
            }
            client.TriggerEvent(ServerToClient.ShowInitialEmailVerification);
            client.SendChatMessage(AccountStrings.SuccessChangeVerificationEmailAddress);
        }

        [RemoteEvent(ClientToServer.SubmitResendEmail)]
        public void ClientEvent_OnSubmitResendEmail(Client player)
        {
            Task.Run(async () => await OnResendEmailAsync(player));
        }
        public static async Task OnResendEmailAsync(Client client)
        {
            if (!client.IsLoggedIn())
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            if (!await EmailToken.ExistsAsync(client.GetAccountData()))
            {
                client.TriggerEvent(ServerToClient.DisplayError, "Error No Token for that Account");
                return;
            }

            await EmailToken.SendEmail(client.GetAccountData());

            client.SendChatMessage(AccountStrings.SuccessResendVerificationEmail);

        }

        [RemoteEvent(ClientToServer.SubmitBackToLogin)]
        public void ClientEvent_OnSubmitBackToLogin(Client player)
        {
            if (player.IsLoggedIn())
            {
                var accData = player.GetAccountData();
                Task.Run(async () => await accData.SaveAsync());
                player.Logout();
            }
            player.TriggerEvent(ServerToClient.ShowLoginPage);

        }

        [RemoteEvent(ClientToServer.SubmitEnableGoogleAuthCode)]
        public void ClientEvent_OnSubmitEnableGoogleAuthCode(Client player, string code)
        {
            if (!player.IsLoggedIn()) return;
            var acc = player.GetAccountData();
            if (acc.TempTwoFactorGASharedKey == null) return;

            if (!ValidateString(ValidationStrings.GoogleAuthenticatorCode, code))
            {
                player.TriggerEvent(ServerToClient.DisplayError, "Must be 6 characters and only digits");
                return;
            }

            if (GoogleAuthenticator.GeneratePin(acc.TempTwoFactorGASharedKey) != code)
            {
                player.TriggerEvent(ServerToClient.DisplayError, "Wrong code try again");
                return;
            }

            acc.TwoFactorGASharedKey = acc.TempTwoFactorGASharedKey;
            Task.Run(async () => await acc.SaveSingleAsync(Column.TwoFactorGASharedKey));
            player.TriggerEvent(ServerToClient.ShowQRCodeEnabled);
        }


        private static async Task LoginAccount(Account fetchedAcc, Client client)
        {
            fetchedAcc.LastHWID = client.Serial;
            fetchedAcc.LastIP = client.Address;
            fetchedAcc.LastLoginDate = DateTime.Now;
            fetchedAcc.LastSocialClubName = client.SocialClubName;
            client.Login(fetchedAcc);
            await fetchedAcc.SaveAsync();
            client.SendChatMessage(AccountStrings.SuccessLogin);
        }
        private static bool IsAccountLoggedIn(Account account)
        {
            foreach (var p in NAPI.Pools.GetAllPlayers())
            {
                if (!p.IsLoggedIn()) continue;
                if (p.GetAccountData() != account) continue;
                return true;
            }
            return false;
        }
        private static void SetLoginState(Client client, bool state)
        {
            if (state)
            {
                client.Transparency = 0;
                client.Dimension = (uint)client.Value + 1500;
            }
            else
            {
                client.Transparency = 255;
                client.Dimension = 0;
            }
            client.TriggerEvent(ServerToClient.SetLoginScreen, state);
        }

    }
}
