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

        [Command("register", "Usage: /register [username] [password] [emailaddress]", SensitiveInfo = true)]
        public void Cmd_Register(Client player, string username, string password, string emailAddress)
        {
            Task.Run(async () => await RegisterNewAccountAsync(player, username, password, emailAddress));
        }


        [Command("login", "Usage: /login [username] [password]", SensitiveInfo = true)]
        public void Cmd_Login(Client player, string username, string password)
        {
            Task.Run(async () => await LoginAccountAsync(player, username, password));
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
            Task.Run(async () => await VerifyEmailAsync(player, code));
        }

        [Command("changeverificationmail", "Usage: /changeVerificationMail [new email]", SensitiveInfo = true)]
        public void Cmd_ChangeVerEmail(Client player, string newEmail)
        {
            Task.Run(async () => await ChangeVerificationEmailAsync(player, newEmail));
        }

        [Command("resendemail", "Usage: /resendemail", SensitiveInfo = true)]
        public void Cmd_ResendEmail(Client player)
        {
            Task.Run(async () => await ResendEmailAsync(player));
        }

        public static async Task RegisterNewAccountAsync(Client client, string username, string password, string emailAddress)
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

        public static async Task LoginAccountAsync(Client client, string username, string password)
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

            fetchedAcc.LastHWID = client.Serial;
            fetchedAcc.LastIP = client.Address;
            fetchedAcc.LastLoginDate = DateTime.Now;
            fetchedAcc.LastSocialClubName = client.SocialClubName;
            client.Login(fetchedAcc);
            await fetchedAcc.SaveAsync();

            if (await EmailToken.ExistsAsync(fetchedAcc))
            {
                client.SendChatMessage(AccountStrings.ErrorUnverifiedEmail);
                return;
            }

            client.SendChatMessage(AccountStrings.SuccessLogin);
            SetLoginState(client, false);
        }

        public static async Task VerifyEmailAsync(Client client, string providedToken)
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

            if (!await EmailToken.ValidateAsync(client.GetAccountData(), providedToken))
            {
                client.SendChatMessage(AccountStrings.ErrorInvalidVerificationCode);
                return;
            }

            // Success, when EmailToken.ValidateAsync(..) return true the entry from EmailTokens is already removed.
            client.SendChatMessage(AccountStrings.SuccessEmailVerification);
            SetLoginState(client, false);
        }

        public static async Task ChangeVerificationEmailAsync(Client client, string newEmailAddress)
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

        public static async Task ResendEmailAsync(Client client)
        {
            if (!client.IsLoggedIn())
            {
                client.SendChatMessage(AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            if (!await EmailToken.ExistsAsync(client.GetAccountData()))
            {
                client.SendChatMessage(AccountStrings.ErrorEmailAlreadyVerified);
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
