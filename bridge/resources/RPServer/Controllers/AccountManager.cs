using System;
using System.Threading.Tasks;
using GTANetworkAPI;
using RPServer.Controllers.Util;
using RPServer.Game;
using RPServer.InternalAPI.Extensions;
using RPServer.Models;
using RPServer.Resource;
using Shared.Data;
using Shared.Events.ServerToClient;
using static RPServer.Controllers.Util.DataValidator;

namespace RPServer.Controllers
{
    public delegate void OnPlayerLoginDelegate(object source, EventArgs e);

    internal class AccountManager : Script
    {
        public AccountManager()
        {
            PlayerLogin += OnPlayerLogin;
        }

        public static event OnPlayerLoginDelegate PlayerLogin;

        [Command(CmdStrings.CMD_ToggleTwoFactorEmail)]
        public void CMD_ToggleTwoFactorEmail(Client client)
        {
            if (!client.IsLoggedIn()) return;
            var acc = client.GetAccount();
            acc.HasEnabledTwoStepByEmail = !acc.HasEnabledTwoStepByEmail;
            acc.HasPassedTwoStepByEmail = acc.HasEnabledTwoStepByEmail;
            client.SendChatMessage($"2FA by Email has been {acc.HasEnabledTwoStepByEmail}");
        }

        [Command(CmdStrings.CMD_ToggleTwoFactorGA)]
        public async void CMD_ToggleTwoFactorGA(Client client)
        {
            if (!client.IsLoggedIn()) return;

            var acc = client.GetAccount();
            if (!acc.Is2FAbyGAEnabled())
            {
                var key = GoogleAuthenticator.GenerateTwoFactorGASharedKey();
                var link = GoogleAuthenticator.GetGQCodeImageLink(acc.Username, key, 150, 150);

                client.TriggerEvent(Authentication.ShowQRCode, link);
                if (acc.TempTwoFactorGASharedKey == null) acc.TempTwoFactorGASharedKey = new byte[50];
                acc.TempTwoFactorGASharedKey = key;
                acc.HasPassedTwoStepByGA = true;
            }
            else
            {
                acc.TwoFactorGASharedKey = null;
                acc.HasPassedTwoStepByGA = false;

                await acc.UpdateAsync();
            }
        }

        [Command(CmdStrings.CMD_Logout)]
        public async void CMD_Logout(Client player)
        {
            if (!player.IsLoggedIn())
            {
                player.SendChatMessage("You are not logged in.");
                return;
            }

            await LogoutAccount(player);

            player.SendChatMessage("Bye!");
        }


        [RemoteEvent(Shared.Events.ClientToServer.Authentication.SubmitRegisterAccount)]
        public async void ClientEvent_OnSubmitRegisterAccount(Client client, string username, string emailAddress,
            string password)
        {
            if (client.IsLoggedIn(true))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorPlayerAlreadyLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.Username, username))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorUsernameInvalid);
                return;
            }

            if (!ValidateString(ValidationStrings.Password, password))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorPasswordInvalid);
                return;
            }

            if (!ValidateString(ValidationStrings.EmailAddress, emailAddress))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorEmailInvalid);
                return;
            }

            if (await AccountModel.ExistsAsync(username))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorUsernameTaken);
                return;
            }

            if (await AccountModel.IsEmailTakenAsync(emailAddress))
            {
                // Another account with the that email address
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorEmailTaken);
                return;
            }

            if (await EmailToken.IsEmailTakenAsync(emailAddress))
            {
                // Another account in the list of email tokens with that address
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorEmailTokenAddressTaken);
                return;
            }

            await AccountModel.CreateAsync(username, password, client.SocialClubName);
            var newAcc = await AccountModel.FetchAsync(username);
            await EmailToken.CreateAsync(newAcc, emailAddress);
            await EmailToken.SendEmail(newAcc);

            client.TriggerEvent(Authentication.RegistrationSuccess, AccountStrings.SuccessRegistration);
        }

        [RemoteEvent(Shared.Events.ClientToServer.Authentication.SubmitLoginAccount)]
        public async void ClientEvent_OnSubmitLoginAccount(Client client, string username, string password)
        {
            if (client.IsLoggedIn(true))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorPlayerAlreadyLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.Username, username))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorUsernameInvalid);
                return;
            }

            if (!ValidateString(ValidationStrings.Password, password))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorPasswordInvalid);
                return;
            }

            if (!await AccountModel.ExistsAsync(username))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorUsernameNotExist);
                return;
            }

            if (!await AccountModel.AuthenticateAsync(username, password))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorInvalidCredentials);
                return;
            }

            var fetchedAcc = await AccountModel.FetchAsync(username);

            if (IsAccountAlreadyLoggedIn(fetchedAcc))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorAccountAlreadyLoggedIn);
                return;
            }

            await LoginAccount(client, fetchedAcc);

            if (!fetchedAcc.HasVerifiedEmail())
            {
                client.TriggerEvent(Authentication.ShowInitialEmailVerification);
                return;
            }

            if (fetchedAcc.Is2FAbyEmailEnabled())
            {
                fetchedAcc.HasPassedTwoStepByEmail = false;
                await EmailToken.CreateAsync(fetchedAcc, fetchedAcc.EmailAddress);
                await EmailToken.SendEmail(fetchedAcc);
                client.TriggerEvent(Authentication.Show2FAbyEmailAddress);
                return;
            }

            if (fetchedAcc.Is2FAbyGAEnabled())
            {
                fetchedAcc.HasPassedTwoStepByGA = false;
                client.TriggerEvent(Authentication.Show2FAbyGoogleAuth);
                return;
            }

            SetLoginState(client, false);
        }

        [RemoteEvent(Shared.Events.ClientToServer.Authentication.SubmitEmailToken)]
        public async void ClientEvent_OnSubmitEmailToken(Client client, string token)
        {
            if (!client.IsLoggedIn(true))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.EmailVerificationCode, token))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorInvalidVerificationCode);
                return;
            }

            var accData = client.GetAccount();

            if (!accData.HasVerifiedEmail()) return;

            if (!accData.Is2FAbyEmailEnabled())
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.TwoFactorByEmailIsNotEnabled);
                return;
            }

            if (!await EmailToken.ValidateAsync(accData, token))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorInvalidVerificationCode);
                return;
            }

            accData.HasPassedTwoStepByEmail = true;

            if (accData.Is2FAbyGAEnabled() && !accData.HasPassedTwoStepByGA)
            {
                client.TriggerEvent(Authentication.Show2FAbyGoogleAuth, AccountStrings.VerifyTwoFactorByGA);
                return;
            }

            SetLoginState(client, false);
        }

        [RemoteEvent(Shared.Events.ClientToServer.Authentication.SubmitGoogleAuthCode)]
        public void ClientEvent_OnSubmitGoogleAuthCode(Client client, string token)
        {
            var accountData = client.GetAccount();

            if (!client.IsLoggedIn(true))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.GoogleAuthenticatorCode, token))
            {
                client.TriggerEvent(Authentication.DisplayError, "Must be 6 characters and only digits");
                return;
            }

            if (!accountData.HasVerifiedEmail())
                throw new Exception("Tried to verify Two-Step by GA when user has no email set"); // Dummy check

            if (!accountData.Is2FAbyGAEnabled())
            {
                client.TriggerEvent(Authentication.DisplayError,
                    "Two Step auth by GA is not enabled for this account.");
                return;
            }

            if (GoogleAuthenticator.GeneratePin(accountData.TwoFactorGASharedKey) != token)
            {
                client.TriggerEvent(Authentication.DisplayError, "Wrong 2FA code, try again");
                return;
            }

            accountData.HasPassedTwoStepByGA = true;

            if (accountData.Is2FAbyEmailEnabled() && !accountData.HasPassedTwoStepByEmail)
            {
                client.TriggerEvent(Authentication.Show2FAbyEmailAddress);
                return;
            }

            SetLoginState(client, false);
        }

        [RemoteEvent(Shared.Events.ClientToServer.Authentication.SubmitFirstEmailToken)]
        public async void ClientEvent_OnSubmitFirstEmailToken(Client client, string providedToken)
        {
            if (!client.IsLoggedIn(true))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.EmailVerificationCode, providedToken))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorInvalidVerificationCode);
                return;
            }

            if (client.GetAccount().HasVerifiedEmail())
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorEmailAlreadyVerified);
                return;
            }

            var accData = client.GetAccount();
            var accToken = await EmailToken.FetchAsync(accData);
            var accEmail = accToken.EmailAddress;

            if (!await EmailToken.ValidateAsync(accData, providedToken))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorInvalidVerificationCode);
                return;
            }

            accData.EmailAddress = accEmail;
            await accData.UpdateAsync();
            client.SendChatMessage(AccountStrings.SuccessEmailVerification);

            SetLoginState(client, false);
        }

        [RemoteEvent(Shared.Events.ClientToServer.Authentication.SubmitNewVerificationEmail)]
        public async void ClientEvent_OnSubmitNewVerificationEmail(Client client, string newEmail)
        {
            if (!client.IsLoggedIn(true))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.EmailAddress, newEmail))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorEmailInvalid);
                return;
            }

            if (client.GetAccount().HasVerifiedEmail())
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorEmailAlreadyVerified);
                return;
            }

            if (await AccountModel.IsEmailTakenAsync(newEmail))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorEmailTaken);
                return;
            }

            if (await EmailToken.IsEmailTakenAsync(newEmail))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorEmailTokenAddressTaken);
                return;
            }

            var accData = client.GetAccount();
            var accTok = await EmailToken.FetchAsync(accData);

            if (accTok != null)
            {
                if (accTok.EmailAddress == newEmail)
                {
                    client.TriggerEvent(Authentication.ShowChangeEmailAddress);
                    client.TriggerEvent(Authentication.DisplayError,
                        AccountStrings.ErrorChangeVerificationEmailDuplicate);
                    return;
                }

                await EmailToken.ChangeEmailAsync(client.GetAccount(), newEmail);
                await EmailToken.SendEmail(client.GetAccount());
            }
            else
            {
                // Handles the case where there's no token entry in the database
                await EmailToken.CreateAsync(accData, newEmail);
                await EmailToken.SendEmail(accData);
            }

            client.TriggerEvent(Authentication.ShowInitialEmailVerification);
            client.SendChatMessage(AccountStrings.SuccessChangeVerificationEmailAddress);
        }

        [RemoteEvent(Shared.Events.ClientToServer.Authentication.SubmitResendEmail)]
        public async void ClientEvent_OnSubmitResendEmail(Client client)
        {
            if (!client.IsLoggedIn(true))
            {
                client.TriggerEvent(Authentication.DisplayError, AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            if (!await EmailToken.ExistsAsync(client.GetAccount()))
            {
                client.TriggerEvent(Authentication.DisplayError, "Error No Token for that Account");
                return;
            }

            await EmailToken.SendEmail(client.GetAccount());
            client.SendChatMessage(AccountStrings.SuccessResendVerificationEmail);
        }

        [RemoteEvent(Shared.Events.ClientToServer.Authentication.SubmitBackToLogin)]
        public void ClientEvent_OnSubmitBackToLogin(Client player)
        {
            if (player.IsLoggedIn(true)) player.Logout();
            player.TriggerEvent(Authentication.ShowLoginPage);
        }

        [RemoteEvent(Shared.Events.ClientToServer.Authentication.SubmitEnableGoogleAuthCode)]
        public async void ClientEvent_OnSubmitEnableGoogleAuthCode(Client player, string code)
        {
            if (!player.IsLoggedIn(true)) return;
            var acc = player.GetAccount();
            if (acc.TempTwoFactorGASharedKey == null) return;

            if (!ValidateString(ValidationStrings.GoogleAuthenticatorCode, code))
            {
                player.TriggerEvent(Authentication.DisplayError, "Must be 6 characters and only digits");
                return;
            }

            if (GoogleAuthenticator.GeneratePin(acc.TempTwoFactorGASharedKey) != code)
            {
                player.TriggerEvent(Authentication.DisplayError, "Wrong code try again");
                return;
            }

            acc.TwoFactorGASharedKey = acc.TempTwoFactorGASharedKey;

            await acc.UpdateAsync();
            player.TriggerEvent(Authentication.ShowQRCodeEnabled);
        }

        private static async Task LoginAccount(Client client, AccountModel fetchedAcc)
        {
            fetchedAcc.LastHWID = client.Serial;
            fetchedAcc.LastIP = client.Address;
            fetchedAcc.LastLoginDate = DateTime.Now;
            fetchedAcc.LastSocialClubName = client.SocialClubName;
            client.Login(fetchedAcc);
            await fetchedAcc.UpdateAsync();
        }

        public static async Task LogoutAccount(Client client)
        {
            var acc = client.GetAccount();
            if (acc != null)
            {
                await acc.UpdateAsync();
                var ch = client.GetActiveChar();
                if (ch != null) CharacterHandler.DespawnCharacter(client);
                client.Logout();
                SetLoginState(client, true);
            }
        }

        public static void SetLoginState(Client client, bool state)
        {
            client.TriggerEvent(Authentication.SetLoginScreen, state);
            if (!state)
                // This part gets triggered only once per successful login
                PlayerLogin?.Invoke(client, EventArgs.Empty);
        }

        private void OnPlayerLogin(object source, EventArgs e)
        {
            var client = source as Client;
            if (client == null) return;

            NAPI.Player.SpawnPlayer(client, Initialization.DefaultSpawnPos);
            client.SendChatMessage(AccountStrings.SuccessLogin);
            if (client.GetAccount().IsAdmin())
                ChatHandler.SendClientMessage(client,
                    $"{Colors.COLOR_RED}<!> {Colors.COLOR_WHITE}You have logged in as a level {client.GetAccount().AdminLevel} admin.");
            client.SendChatMessage(
                "All commands: /allcmds <-- WARNING: this uses reflection to get all cmds which is really slow and hangs the server. Takes ~10secs to execute.");
        }

        private static bool IsAccountAlreadyLoggedIn(AccountModel account)
        {
            foreach (var p in NAPI.Pools.GetAllPlayers())
            {
                if (!p.IsLoggedIn(true)) continue;
                if (p.GetAccount() != account) continue;
                return true;
            }

            return false;
        }
    }
}