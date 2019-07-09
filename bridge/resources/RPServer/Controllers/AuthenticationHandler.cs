using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using GTANetworkAPI;
using RPServer.Game;
using RPServer.Models;
using RPServer.Strings;
using RPServer.Util;
using Shared;
using static RPServer.Util.DataValidator;
using Task = System.Threading.Tasks.Task;

namespace RPServer.Controllers
{
    public delegate void OnPlayerSuccessfulLoginDelegate(object source, EventArgs e);

    internal class AuthenticationHandler : Script
    {
        public static event OnPlayerSuccessfulLoginDelegate PlayerSuccessfulLogin;
        
        [Command(CmdStrings.CMD_ToggleTwoFactorEmail)]
        public void CMD_ToggleTwoFactorEmail(Client client)
        {
            if (!client.IsLoggedIn()) return;
            var acc = client.GetAccountData();
            acc.HasEnabledTwoStepByEmail = !acc.HasEnabledTwoStepByEmail;
            acc.HasPassedTwoStepByEmail = acc.HasEnabledTwoStepByEmail;
            client.SendChatMessage($"2FA by Email has been {acc.HasEnabledTwoStepByEmail}");
        }

        [Command(CmdStrings.CMD_ToggleTwoFactorGA)]
        public void CMD_ToggleTwoFactorGA(Client client)
        {
            if (!client.IsLoggedIn()) return;

            var acc = client.GetAccountData();
            if (!acc.Is2FAbyGAEnabled())
            {
                var key = GoogleAuthenticator.GenerateTwoFactorGASharedKey();
                var link = GoogleAuthenticator.GetGQCodeImageLink(acc.Username, key, 150, 150);

                client.TriggerEvent(ServerToClient.ShowQRCode, link);
                if (acc.TempTwoFactorGASharedKey == null) acc.TempTwoFactorGASharedKey = new byte[50];
                acc.TempTwoFactorGASharedKey = key;
                acc.HasPassedTwoStepByGA = true;
            }
            else
            {
                if (!client.CanRunTask()) return;
                acc.TwoFactorGASharedKey = null;
                acc.HasPassedTwoStepByGA = false;

                TaskManager.Run(client, async () => await acc.UpdateAsync());
            }

        }

        [Command(CmdStrings.CMD_Logout)]
        public void Cmd_Logout(Client player)
        {
            if (!player.CanRunTask()) return;
            if (!player.IsLoggedIn())
            {
                player.SendChatMessage("You are not logged in.");
                return;
            }

            var acc = player.GetAccountData();

            TaskManager.Run(player, async () => await acc.UpdateAsync());

            player.Logout();
            player.SendChatMessage("Bye!");
            SetLoginState(player, true);
        }


        [RemoteEvent(ClientToServer.SubmitRegisterAccount)]
        public void ClientEvent_OnSubmitRegisterAccount(Client client, string username, string emailAddress, string password)
        {
            if(!client.CanRunTask()) return;
            if (client.IsLoggedIn(true))
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
            TaskManager.Run(client, async () => await OnRegisterNewAccountAsync(client, username, emailAddress, password));
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

            await Account.CreateAsync(username, password, client.SocialClubName);
            var newAcc = await Account.FetchAsync(username);
            await EmailToken.CreateAsync(newAcc, emailAddress);
            await EmailToken.SendEmail(newAcc);

            client.TriggerEvent(ServerToClient.RegistrationSuccess, AccountStrings.SuccessRegistration);
        }

        [RemoteEvent(ClientToServer.SubmitLoginAccount)]
        public void ClientEvent_OnSubmitLoginAccount(Client client, string username, string password)
        {
            if (!client.CanRunTask()) return;
            if (client.IsLoggedIn(true))
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
            TaskManager.Run(client, async () => await OnLoginAccountAsync(client, username, password));
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
                client.TriggerEvent(ServerToClient.Show2FAbyGoogleAuth);
                return;
            }

            SetLoginState(client, false);
        }

        [RemoteEvent(ClientToServer.SubmitEmailToken)]
        public void ClientEvent_OnSubmitEmailToken(Client client, string token)
        {
            if(!client.CanRunTask()) return;
            if (!client.IsLoggedIn(true))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            if (!ValidateString(ValidationStrings.EmailVerificationCode, token))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorInvalidVerificationCode);
                return;
            }
            TaskManager.Run(client, async () => await OnVerifyTwoStepByEmailAsync(client, token));
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

            if (!client.IsLoggedIn(true))
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
            if (!client.CanRunTask()) return;

            if (!client.IsLoggedIn(true))
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

            TaskManager.Run(client, async () => await OnVerifyEmailAsync(client, providedToken));
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
            await accData.UpdateAsync();
            client.SendChatMessage(AccountStrings.SuccessEmailVerification);

            SetLoginState(client, false);
        }

        [RemoteEvent(ClientToServer.SubmitNewVerificationEmail)]
        public void ClientEvent_OnSubmitNewVerificationEmail(Client client, string newEmail)
        {
            if(!client.CanRunTask()) return;
            if (!client.IsLoggedIn(true))
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
            TaskManager.Run(client, async () => await OnChangeVerificationEmailAsync(client, newEmail));
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
        public void ClientEvent_OnSubmitResendEmail(Client client)
        {
            if(!client.CanRunTask()) return;
            if (!client.IsLoggedIn(true))
            {
                client.TriggerEvent(ServerToClient.DisplayError, AccountStrings.ErrorPlayerNotLoggedIn);
                return;
            }

            TaskManager.Run(client, async () => await OnResendEmailAsync(client));
        }
        public static async Task OnResendEmailAsync(Client client)
        {
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
            if (player.IsLoggedIn(true)) player.Logout();
            player.TriggerEvent(ServerToClient.ShowLoginPage);
        }

        [RemoteEvent(ClientToServer.SubmitEnableGoogleAuthCode)]
        public void ClientEvent_OnSubmitEnableGoogleAuthCode(Client player, string code)
        {
            if(!player.CanRunTask()) return;
            if (!player.IsLoggedIn(true)) return;
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

            TaskManager.Run(player, async () => await acc.UpdateAsync());
            player.TriggerEvent(ServerToClient.ShowQRCodeEnabled);
        }


        private static async Task LoginAccount(Account fetchedAcc, Client client)
        {
            fetchedAcc.LastHWID = client.Serial;
            fetchedAcc.LastIP = client.Address;
            fetchedAcc.LastLoginDate = DateTime.Now;
            fetchedAcc.LastSocialClubName = client.SocialClubName;
            client.Login(fetchedAcc);
            await fetchedAcc.UpdateAsync();
        }
        public static void SetLoginState(Client client, bool state)
        {
            if (state)
            {
                client.Transparency = 0;
                client.Dimension = (uint)client.Value + 1500;
            }
            else
            { // This part gets triggered only once per successful login
                client.Transparency = 255;
                NAPI.Player.SpawnPlayer(client, Initialization.DefaultSpawnPos);
                client.SendChatMessage(AccountStrings.SuccessLogin);
                client.SendChatMessage("SUM COMMANDS: /cmds");
            }
            client.SetSharedData(SharedDataKey.AccountLoggedIn, state);
            client.TriggerEvent(ServerToClient.SetLoginScreen, state);

            // Keep this at the end of the Method
            if(!state) PlayerSuccessfulLogin?.Invoke(client, EventArgs.Empty);
        }
        private static bool IsAccountLoggedIn(Account account)
        {
            foreach (var p in NAPI.Pools.GetAllPlayers())
            {
                if (!p.IsLoggedIn(true)) continue;
                if (p.GetAccountData() != account) continue;
                return true;
            }
            return false;
        }
        private static void HandleTaskCompletion(Task t)
        {
            if (t.IsFaulted && t.Exception != null) throw t.Exception;
        }
    }

    public static class GoogleAuthenticator
    {
        private const int IntervalLength = 30;
        private const int PinLength = 6;
        private static readonly int PinModulo = (int)Math.Pow(10, PinLength);

        /// <summary>
        ///   Number of intervals that have elapsed.
        /// </summary>
        private static long CurrentInterval
        {
            get
            {
                var elapsedSeconds = (long)Math.Floor((DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds);

                return elapsedSeconds / IntervalLength;
            }
        }

        /// <summary>
        ///   Generates a QR code bitmap for provisioning.
        /// </summary>
        public static string GetGQCodeImageLink(string username, byte[] key, int width, int height)
        {
            var keyString = Encoder.Base32Encode(key);
            var provisionUrl = Encoder.UrlEncode($"otpauth://totp/{username}?secret={keyString}&issuer={Initialization.SERVER_NAME}");

            var chartUrl = $"https://chart.apis.google.com/chart?cht=qr&chs={width}x{height}&chl={provisionUrl}";
            return chartUrl;
            /*using (var client = new WebClient())
            {
                return client.DownloadData(chartUrl);
            }*/
        }

        public static byte[] GenerateTwoFactorGASharedKey()
        {
            return RandomGenerator.GetInstance().GenerateRandomBytes(50);
        }

        /// <summary>
        ///   Generates a pin for the given key.
        /// </summary>
        public static string GeneratePin(byte[] key)
        {
            return GeneratePin(key, CurrentInterval);
        }

        /// <summary>
        ///   Generates a pin by hashing a key and counter.
        /// </summary>
        static string GeneratePin(byte[] key, long counter)
        {
            const int sizeOfInt32 = 4;

            var counterBytes = BitConverter.GetBytes(counter);

            if (BitConverter.IsLittleEndian)
            {
                //spec requires bytes in big-endian order
                Array.Reverse(counterBytes);
            }

            var hash = new HMACSHA1(key).ComputeHash(counterBytes);
            var offset = hash[hash.Length - 1] & 0xF;

            var selectedBytes = new byte[sizeOfInt32];
            Buffer.BlockCopy(hash, offset, selectedBytes, 0, sizeOfInt32);

            if (BitConverter.IsLittleEndian)
            {
                //spec interprets bytes in big-endian order
                Array.Reverse(selectedBytes);
            }

            var selectedInteger = BitConverter.ToInt32(selectedBytes, 0);

            //remove the most significant bit for interoperability per spec
            var truncatedHash = selectedInteger & 0x7FFFFFFF;

            //generate number of digits for given pin length
            var pin = truncatedHash % PinModulo;

            return pin.ToString(CultureInfo.InvariantCulture).PadLeft(PinLength, '0');
        }
        #region Nested type: Encoder

        private static class Encoder
        {
            /// <summary>
            ///   Url Encoding (with upper-case hexadecimal per OATH specification)
            /// </summary>
            public static string UrlEncode(string value)
            {
                const string urlEncodeAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

                var builder = new StringBuilder();

                foreach (var symbol in value)
                {
                    if (urlEncodeAlphabet.IndexOf(symbol) != -1)
                    {
                        builder.Append(symbol);
                    }
                    else
                    {
                        builder.Append('%');
                        builder.Append(((int)symbol).ToString("X2"));
                    }
                }
                return builder.ToString();
            }

            /// <summary>
            ///   Base-32 Encoding
            /// </summary>
            public static string Base32Encode(byte[] data)
            {
                const int inByteSize = 8;
                const int outByteSize = 5;
                const string base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

                int i = 0, index = 0;
                var builder = new StringBuilder((data.Length + 7) * inByteSize / outByteSize);

                while (i < data.Length)
                {
                    int currentByte = data[i];
                    int digit;

                    //Is the current digit going to span a byte boundary?
                    if (index > (inByteSize - outByteSize))
                    {
                        int nextByte;

                        if ((i + 1) < data.Length)
                        {
                            nextByte = data[i + 1];
                        }
                        else
                        {
                            nextByte = 0;
                        }

                        digit = currentByte & (0xFF >> index);
                        index = (index + outByteSize) % inByteSize;
                        digit <<= index;
                        digit |= nextByte >> (inByteSize - index);
                        i++;
                    }
                    else
                    {
                        digit = (currentByte >> (inByteSize - (index + outByteSize))) & 0x1F;
                        index = (index + outByteSize) % inByteSize;

                        if (index == 0)
                        {
                            i++;
                        }
                    }

                    builder.Append(base32Alphabet[digit]);
                }

                return builder.ToString();
            }
        }

        #endregion
    }

}
