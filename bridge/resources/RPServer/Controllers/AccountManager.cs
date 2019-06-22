using System;
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
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(Client player, DisconnectionType type, string reason)
        {
            if (!player.IsLoggedIn()) return;

            var acc = player.GetAccountData();
            acc.Save();
            player.Logout();
        }

        public static void RegisterNewAccount(Client client, string username, string password, string emailAddress)
        {
            if (client.IsLoggedIn())
            {
                client.SendChatMessage(AccountStrings.ErrorAlreadyLoggedIn);
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

            if (Account.Exists(username))
            {
                client.SendChatMessage(AccountStrings.ErrorUsernameTaken);
                return;
            }

            if (Account.IsEmailTaken(emailAddress))
            {
                client.SendChatMessage(AccountStrings.ErrorEmailTaken);
                return;
            }

            var newAcc = Account.Create(username, password, client.SocialClubName);
            EmailToken.Create(newAcc, emailAddress);
            EmailToken.SendEmail(newAcc);

            client.SendChatMessage(AccountStrings.SuccessRegistration);
        }

        public static void LoginAccount(Client client, string username, string password)
        {
            if (client.IsLoggedIn())
            {
                client.SendChatMessage(AccountStrings.ErrorAlreadyLoggedIn);
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

            if (!Account.Exists(username))
            {
                client.SendChatMessage(AccountStrings.ErrorUsernameNotExist);
                return;
            }

            if (!Account.Authenticate(username, password))
            {
                client.SendChatMessage(AccountStrings.ErrorInvalidCredentials);
                return;
            }

            var fetchedAcc = Account.Fetch(username);
            fetchedAcc.LastHWID = client.Serial;
            fetchedAcc.LastIP = client.Address;
            fetchedAcc.LastLoginDate = DateTime.Now;
            fetchedAcc.LastSocialClubName = client.SocialClubName;
            client.Login(fetchedAcc);
            fetchedAcc.Save();

            if (EmailToken.Exists(fetchedAcc))
            {
                client.SendChatMessage(AccountStrings.ErrorUnverifiedEmail);
                return;
            }

            client.SendChatMessage(AccountStrings.SuccessLogin);
            // TODO: Toggle logging in screen off

        }

        public static void VerifyEmail(Client client, string providedToken)
        {
            if (!client.IsLoggedIn())
            {
                client.SendChatMessage(AccountStrings.ErrorNotLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.EmailVerificationCode, providedToken))
            {
                client.SendChatMessage(AccountStrings.ErrorInvalidVerificationCode);
                return;
            }

            if (!EmailToken.Validate(client.GetAccountData(), providedToken))
            {
                client.SendChatMessage(AccountStrings.ErrorInvalidVerificationCode);
                return;
            }

            // Success, when EmailToken.Validate(..) return true the entry from EmailTokens is already removed.
            client.SendChatMessage(AccountStrings.SuccessEmailVerification);

            // TODO: Toggle logging in screen off
        }

        public static void ChangeVerificationEmail(Client client, string newEmailAddress)
        {
            if (!client.IsLoggedIn())
            {
                client.SendChatMessage(AccountStrings.ErrorNotLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.EmailAddress, newEmailAddress))
            {
                client.SendChatMessage(AccountStrings.ErrorEmailInvalid);
                return;
            }

            if (EmailToken.Fetch(client.GetAccountData()).EmailAddress == newEmailAddress)
            {
                client.SendChatMessage(AccountStrings.ErrorChangeVerificationEmailDuplicate);
                return;
            }

            EmailToken.ChangeEmail(client.GetAccountData(), newEmailAddress);
            EmailToken.SendEmail(client.GetAccountData());
            client.SendChatMessage(AccountStrings.SuccessChangeVerificationEmailAddress);
        }

        public static void ResendEmail(Client client)
        {
            if (!client.IsLoggedIn())
            {
                client.SendChatMessage(AccountStrings.ErrorNotLoggedIn);
                return;
            }

            if (!EmailToken.Exists(client.GetAccountData()))
            {
                client.SendChatMessage(AccountStrings.ErrorEmailAlreadyVerified);
                return;
            }

            EmailToken.SendEmail(client.GetAccountData());
            client.SendChatMessage(AccountStrings.SuccessResendVerificationEmail);

        }
    }
}
