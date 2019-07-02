using System;
using System.Threading;
using Shared;
using GTANetworkAPI;
using RPServer.Database;
using RPServer.Models;
using RPServer.Util;

namespace RPServer._init
{
    internal class Globals : Script
    {
        public const string SERVER_NAME = "AlphaRP";
        public const uint VERSION_MAJOR = 0;
        public const uint VERSION_MINOR = 1;
        public const uint VERSION_PATCH = 0;
        public const string PRE_RELEASE = "";

        public static readonly Vector3 DefaultSpawnPos = new Vector3(-782.1527709960938f, 19.77294921875f, 41.93227767944336f);
        private static Timer _expiredEmailTokensTimer;

        [ServerEvent(Event.ResourceStart)]
        public async void OnResourceStart()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\n\n---------------------------- STARTING {SERVER_NAME} ({VERSION_MAJOR}.{VERSION_MINOR}.{VERSION_PATCH}{PRE_RELEASE}) ----------------------------");
            Console.ResetColor();
            Console.WriteLine();

            // Server Settings
            NAPI.Server.SetAutoSpawnOnConnect(false);
            NAPI.Server.SetAutoRespawnAfterDeath(false);
            NAPI.Server.SetDefaultSpawnLocation(DefaultSpawnPos);
            NAPI.Server.SetGlobalServerChat(false);

            // Sever World Settings
            NAPI.World.SetTime(0, 0, 0);
            NAPI.World.ResetIplList();

            // Initialize the Logger 
            Logger.GetInstance();

            // Geyt Database Settings (meta.xml)
            DbConnection.MySqlHost = NAPI.Resource.GetSetting<string>(this, "DB_HOST");
            DbConnection.MySqlPort = NAPI.Resource.GetSetting<uint>(this, "DB_PORT");
            DbConnection.MySqlDatabase = NAPI.Resource.GetSetting<string>(this, "DB_DATABASE");
            DbConnection.MySqlUsername = NAPI.Resource.GetSetting<string>(this, "DB_USERNAME");
            DbConnection.MySqlPassword = NAPI.Resource.GetSetting<string>(this, "DB_PASSWORD");
            // Test MySql Connection
            await DbConnection.TestConnection();

            EmailSender.SmtpHost = NAPI.Resource.GetSetting<string>(this, "SMTP_HOST");
            EmailSender.SmtpPort = NAPI.Resource.GetSetting<int>(this, "SMTP_PORT");
            EmailSender.SmtpUsername = NAPI.Resource.GetSetting<string>(this, "SMTP_USERNAME");
            EmailSender.SmtpPassword = NAPI.Resource.GetSetting<string>(this, "SMTP_PASSWORD");
            // Remove expired tokens from the Database
            await EmailToken.RemoveExpiredCodesAsync();
            // Have expired tokens get removed once per hour
            _expiredEmailTokensTimer = new Timer(OnRemoveExpiredEmailTokens, null, 1000 * 60 * 60, Timeout.Infinite);
        }

        [RemoteEvent(ClientToServer.SubmitPlayerCommand)]
        public void ClientEvent_OnPlayerCommand(Client client, string cmd)
        { // This CANNOT block commands
            var username = "UNREGISTERED";
            if (client.IsLoggedIn())
            {
                var accData = client.GetAccountData();
                username = accData.Username;
            }
            Logger.GetInstance().CommandLog(username, cmd);
        }

        private async void OnRemoveExpiredEmailTokens(object state)
        {
            await EmailToken.RemoveExpiredCodesAsync();
            _expiredEmailTokensTimer.Change(1000 * 60 * 60, Timeout.Infinite);

        }
    }
}