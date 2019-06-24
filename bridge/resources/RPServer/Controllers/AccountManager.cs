using System;
using System.Threading.Tasks;
using GTANetworkAPI;
using RPServer.Models;
using RPServer.Strings;
using RPServer.Util;
using static RPServer.Util.DataValidator;

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

        #region CMDS
        [Command("cmds", "USAGE", SensitiveInfo = true)]
        public void Cmd_cmds(Client player)
        {
            player.SendChatMessage("-----------------------------------");
            player.SendChatMessage("/register /login /logout");
            player.SendChatMessage("/verifyemail /changeverificationmail /resendemail /forgotpass");
            player.SendChatMessage("/verifytwostepemail /verifytwostepga");
            player.SendChatMessage("-----------------------------------");

        }

        [Command("register", "Usage: /register [username] [password] [emailaddress]", SensitiveInfo = true)]
        public void Cmd_Register(Client player, string username, string password, string emailAddress)
        {
            Task.Run(async () => await OnRegisterNewAccountAsync(player, username, password, emailAddress));
        }


        [Command("login", "Usage: /login [username] [password]", SensitiveInfo = true)]
        public void Cmd_Login(Client player, string username, string password)
        {
            Task.Run(async () => await OnLoginAccountAsync(player, username, password));
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

        [Command("verifyemail", "Usage: /verify(e)mail [code]", SensitiveInfo = true, Alias = "verifymail")]
        public void Cmd_VerifyEmail(Client player, string code)
        {
            Task.Run(async () => await OnVerifyEmailAsync(player, code));
        }

        [Command("changeverificationmail", "Usage: /changeVerificationMail [new email]", SensitiveInfo = true)]
        public void Cmd_ChangeVerEmail(Client player, string newEmail)
        {
            Task.Run(async () => await OnChangeVerificationEmailAsync(player, newEmail));
        }

        [Command("resendemail", "Usage: /resendemail", SensitiveInfo = true)]
        public void Cmd_ResendEmail(Client player)
        {
            Task.Run(async () => await OnResendEmailAsync(player));
        }

        [Command("verifytwostepemail", "Usage: ", SensitiveInfo = true)]
        public void cmd_verifytwostepemail(Client player, string token)
        {
            Task.Run(async () => await OnVerifyTwoStepByEmail(player, token));
        }

        [Command("verifytwostepga", "Usage: ", SensitiveInfo = true)]
        public void cmd_verifytwostepga(Client player, string token)
        {
            OnVerifyTwoStepByGA(player, token);
        }
        #endregion

        #region LoginRegister
        public static async Task OnRegisterNewAccountAsync(Client client, string username, string password, string emailAddress)
        {
            if (client.IsLoggedIn())
            {
                client.SendChatMessage(AccountStrings.ErrorPlayerAlreadyLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.Username, username))
            {
                client.SendChatMessage(AccountStrings.ErrorUsernameInvalid);
                return;
            }

            if (!ValidateString(ValidationStrings.Password, password))
            {
                client.SendChatMessage(AccountStrings.ErrorPasswordInvalid);
                return;
            }

            if (!ValidateString(ValidationStrings.EmailAddress, emailAddress))
            {
                client.SendChatMessage(AccountStrings.ErrorEmailInvalid);
                return;
            }

            if (await Account.ExistsAsync(username))
            {
                client.SendChatMessage(AccountStrings.ErrorUsernameTaken);
                return;
            }

            if (await Account.IsEmailTakenAsync(emailAddress))
            {
                client.SendChatMessage(AccountStrings.ErrorEmailTaken);
                return;
            }

            if (await EmailToken.IsEmailTakenAsync(emailAddress))
            {
                client.SendChatMessage(AccountStrings.ErrorEmailTokenAddressTaken);
                return;
            }

            var newAcc = await Account.CreateAsync(username, password, client.SocialClubName);
            await EmailToken.CreateAsync(newAcc, emailAddress);
            await EmailToken.SendEmail(newAcc);

            client.SendChatMessage(AccountStrings.SuccessRegistration);
        }

        public static async Task OnLoginAccountAsync(Client client, string username, string password)
        {
            if (client.IsLoggedIn())
            {
                client.SendChatMessage(AccountStrings.ErrorPlayerAlreadyLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.Username, username))
            {
                client.SendChatMessage(AccountStrings.ErrorUsernameInvalid);
                return;
            }

            if (!ValidateString(ValidationStrings.Password, password))
            {
                client.SendChatMessage(AccountStrings.ErrorPasswordInvalid);
                return;
            }

            if (!await Account.ExistsAsync(username))
            {
                client.SendChatMessage(AccountStrings.ErrorUsernameNotExist);
                return;
            }

            if (!await Account.AuthenticateAsync(username, password))
            {
                client.SendChatMessage(AccountStrings.ErrorInvalidCredentials);
                return;
            }

            var fetchedAcc = await Account.FetchAsync(username);

            if (IsAccountLoggedIn(fetchedAcc))
            {
                client.SendChatMessage(AccountStrings.ErrorAccountAlreadyLoggedIn);
                return;
            }

            LoginAccount(fetchedAcc, client);

            if (!fetchedAcc.HasVerifiedEmail())
            {
                client.SendChatMessage(AccountStrings.ErrorUnverifiedEmail);
                return;
            }

            if (fetchedAcc.Is2FAbyEmailEnabled())
            {
                client.SetData("HasPassedTwoStepByEmail", false);
                await EmailToken.CreateAsync(fetchedAcc, fetchedAcc.EmailAddress);
                await EmailToken.SendEmail(fetchedAcc);

                client.SendChatMessage("Verify 2FA by Email to continue");
            }

            if (fetchedAcc.Is2FAbyGAEnabled())
            {
                client.SetData("HasPassedTwoStepByGA", false);
                client.SendChatMessage("Verify 2FA by GA to continue");
            }
        }
        #endregion


        #region TwoStepVerification
        public static async Task OnVerifyTwoStepByEmail(Client client, string providedToken)
        {
            var accountData = client.GetAccountData();

            if (!client.IsLoggedIn())
            {
                client.SendChatMessage(AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.EmailVerificationCode, providedToken))
            {
                client.SendChatMessage(AccountStrings.ErrorInvalidVerificationCode);
                return;
            }

            if (!accountData.HasVerifiedEmail()) throw new Exception("Tried to verify Two-Step by Email when user has no email set"); // Dummy check

            if (!accountData.Is2FAbyEmailEnabled())
            {
                client.SendChatMessage("2FA by EMAIL is not enabled for this account.");
                return;
            }

            if (!await EmailToken.ValidateAsync(accountData, providedToken))
            {
                client.SendChatMessage(AccountStrings.ErrorInvalidVerificationCode);
                return;
            }

            client.SendChatMessage("Verifed 2FA by EMAIL");
            client.SetData("HasPassedTwoStepByEmail", true);

            if (!client.GetData("HasPassedTwoStepByGA") && accountData.Is2FAbyGAEnabled())
            {
                client.SendChatMessage("Verify 2FA by Google Auth to continue...");
                return;
            }
            client.SendChatMessage("Login finalized. Go play.");
            SetLoginState(client, false);
        }

        public static void OnVerifyTwoStepByGA(Client client, string providedGAKey)
        {
            var accountData = client.GetAccountData();

            if (!client.IsLoggedIn())
            {
                client.SendChatMessage(AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.GoogleAuthenticatorCode, providedGAKey))
            {
                client.SendChatMessage("Must be 6 characters and only digits");
                return;
            }

            if (!accountData.HasVerifiedEmail()) throw new Exception("Tried to verify Two-Step by GA when user has no email set"); // Dummy check

            if (!accountData.Is2FAbyGAEnabled())
            {
                client.SendChatMessage("Two Step auth by GA is not enabled for this account.");
                return;
            }

            if (GoogleAuthenticator.GeneratePin(accountData.TwoFactorGASharedKey) != providedGAKey)
            {
                client.SendChatMessage("Wrong 2FA code, try again");
                return;
            }

            client.SendChatMessage("Verified Two-Step by GA");
            client.SetData("HasPassedTwoStepByGA", true);
            if (!client.GetData("HasPassedTwoStepByEmail") && accountData.Is2FAbyEmailEnabled())
            {
                client.SendChatMessage("Now need to verify by EMAIL");
                return;
            }
            client.SendChatMessage("Finally Logged in now");
            SetLoginState(client, false);
        }
        #endregion

        #region InitialEmailVerificaiton
        public static async Task OnVerifyEmailAsync(Client client, string providedToken)
        {
            if (!client.IsLoggedIn())
            {
                client.SendChatMessage(AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.EmailVerificationCode, providedToken))
            {
                client.SendChatMessage(AccountStrings.ErrorInvalidVerificationCode);
                return;
            }

            if (client.GetAccountData().HasVerifiedEmail())
            {
                client.SendChatMessage(AccountStrings.ErrorEmailAlreadyVerified);
                return;
            }

            var tok = await EmailToken.FetchAsync(client.GetAccountData());
            var storedEmail = tok.EmailAddress;

            if (!await EmailToken.ValidateAsync(client.GetAccountData(), providedToken))
            {
                client.SendChatMessage(AccountStrings.ErrorInvalidVerificationCode);
                return;
            }

            client.GetAccountData().EmailAddress = storedEmail;
            client.SendChatMessage(AccountStrings.SuccessEmailVerification);
            SetLoginState(client, false);
        }

        public static async Task OnChangeVerificationEmailAsync(Client client, string newEmailAddress)
        {
            if (!client.IsLoggedIn())
            {
                client.SendChatMessage(AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.EmailAddress, newEmailAddress))
            {
                client.SendChatMessage(AccountStrings.ErrorEmailInvalid);
                return;
            }

            if (client.GetAccountData().HasVerifiedEmail())
            {
                client.SendChatMessage(AccountStrings.ErrorEmailAlreadyVerified);
                return;
            }

            var tok = await EmailToken.FetchAsync(client.GetAccountData());
            if (tok.EmailAddress == newEmailAddress)
            {
                client.SendChatMessage(AccountStrings.ErrorChangeVerificationEmailDuplicate);
                return;
            }
            await EmailToken.ChangeEmailAsync(client.GetAccountData(), newEmailAddress);
            await EmailToken.SendEmail(client.GetAccountData());
            client.SendChatMessage(AccountStrings.SuccessChangeVerificationEmailAddress);
        }
        #endregion

        public static async Task OnResendEmailAsync(Client client)
        {
            if (!client.IsLoggedIn())
            {
                client.SendChatMessage(AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            if (!await EmailToken.ExistsAsync(client.GetAccountData()))
            {
                client.SendChatMessage("Error No Token for that Account");
                return;
            }

            await EmailToken.SendEmail(client.GetAccountData());

            client.SendChatMessage(AccountStrings.SuccessResendVerificationEmail);

        }
        private static bool IsAccountLoggedIn(Account fetchedAcc)
        {
            foreach (var p in NAPI.Pools.GetAllPlayers())
            {
                if (!p.IsLoggedIn()) continue;
                if (p.GetAccountData() != fetchedAcc) continue;
                return true;
            }
            return false;
        }
        private static async void LoginAccount(Account fetchedAcc, Client client)
        {
            fetchedAcc.LastHWID = client.Serial;
            fetchedAcc.LastIP = client.Address;
            fetchedAcc.LastLoginDate = DateTime.Now;
            fetchedAcc.LastSocialClubName = client.SocialClubName;
            client.Login(fetchedAcc);
            await fetchedAcc.SaveAsync();
            client.SendChatMessage(AccountStrings.SuccessLogin);
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
            NAPI.ClientEvent.TriggerClientEvent(client, "SetLoginScreen", state);
        }
    }
}
