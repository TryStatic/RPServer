using System;
using GTANetworkAPI;
using RPServer.Models;
using RPServer.Util;
using static RPServer.Util.DataValidator;

namespace RPServer.Controllers
{
    internal class AccountManager : Script
    {
        public static void RegisterNewAccount(Client client, string username, string password, string emailAddress)
        {
            if (!ValidateString(ValidationStrings.Username, username))
            {
                client.SendChatMessage("Username too short, 4 chars min"); // TODO: Implement proper chat handler
                return;
            }

            if (!ValidateString(ValidationStrings.Password, password))
            {
                client.SendChatMessage("Password too short, 4 chars min");
                return;
            }

            if (!ValidateString(ValidationStrings.EmailAddress, emailAddress))
            {
                client.SendChatMessage("That's not an email address");
                return;
            }
            // Create the Account
            var newAcc = Account.Create(username, password, client.SocialClubName);

            // Create the Token
            EmailToken.Create(newAcc, emailAddress);

            // TODO: Send the email (???)

            // Auto-login after registration
            client.Login(newAcc);
            client.SendChatMessage("You have automatically logged in. You now need to verify your email address");

        }


        public static void LoginAccount(Client client, string username, string password)
        {
            if (!ValidateString(ValidationStrings.Username, username))
            {
                client.SendChatMessage("Username too short, 4 chars min"); // TODO: Implement proper chat handler
                return;
            }

            if (!ValidateString(ValidationStrings.Password, password))
            {
                client.SendChatMessage("Password too short, 4 chars min");
                return;
            }

            if (!Account.Authenticate(username, password))
            {
                client.SendChatMessage("Wrong pass, try again");
                return;
            }

            // Authentication succeeded
            var fetchedAcc = Account.Fetch(username);
            client.Login(fetchedAcc);

            if (EmailToken.Exists(fetchedAcc))
            {
                client.SendChatMessage("You haven't verified your account yet.");
                return;
            }

            // Email verification already happened
            // TODO: Toggle logging in screen off

        }

        public static void VerifyEmail(Client client, string providedToken)
        {
            if (!client.IsLoggedIn())
            {
                client.SendChatMessage("You must be logged in to verify your email address.");
                return;
            }

            if (!ValidateString(ValidationStrings.EmailVerificationCode, providedToken))
            {
                client.SendChatMessage("Invlid Code Length");
                return;
            }

            if (!EmailToken.Validate(client.GetAccountData(), providedToken))
            {
                client.SendChatMessage("The code you provided is wrong, try again.");
                return;
            }

            // Success, when EmailToken.Validate(..) return true the entry from EmailTokens is already removed.
            client.SendChatMessage("Email Verificaton passed. Go play...");

            // TODO: Toggle logging in screen off

        }




    }
}
