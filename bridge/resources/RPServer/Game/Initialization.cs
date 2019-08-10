using System;
using System.Globalization;
using System.Threading;
using GTANetworkAPI;
using RPServer.Controllers;
using RPServer.Controllers.Util;
using RPServer.Database;
using RPServer.Email;
using RPServer.Models;
using RPServer.Models.Inventory;
using RPServer.Util;

namespace RPServer.Game
{
    internal class Initialization : Script
    {
        public static readonly Vector3 DefaultSpawnPos = new Vector3(-782.1527709960938f, 19.77294921875f, 41.93227767944336f);
        private static Timer _expiredEmailTokensTimer;

        [ServerEvent(Event.ResourceStart)]
        public async void OnResourceStart()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\n\n---------------------------- STARTING {Globals.SERVER_NAME} ({Globals.VERSION}) ----------------------------");
            Console.ResetColor();
            Console.WriteLine();

            // Server Settings
            NAPI.Server.SetAutoSpawnOnConnect(false);
            NAPI.Server.SetAutoRespawnAfterDeath(false);
            NAPI.Server.SetDefaultSpawnLocation(DefaultSpawnPos);
            NAPI.Server.SetGlobalServerChat(false);
            NAPI.Server.SetCommandErrorMessage($"<span style=\'color: #ff6666;\'>Error: </span><span style=\'color: #e3e3e3;\'>We couldn't find that command. Use /helpme if you need further assistance.</span>");
            // Initialize the Logger 
            Logger.GetInstance();

            // Get Database Settings (meta.xml)
            DbConnectionProvider.ProviderName = NAPI.Resource.GetSetting<string>(this, "DB_PROVIDER");
            var dbConnectionStringBuilder = DbConnectionProvider.CreateDbConnectionStringBuilder();
            dbConnectionStringBuilder.Add("Server", NAPI.Resource.GetSetting<string>(this, "DB_HOST"));
            dbConnectionStringBuilder.Add("Port", NAPI.Resource.GetSetting<uint>(this, "DB_PORT"));
            dbConnectionStringBuilder.Add("Database", NAPI.Resource.GetSetting<string>(this, "DB_DATABASE"));
            dbConnectionStringBuilder.Add("UserID", NAPI.Resource.GetSetting<string>(this, "DB_USERNAME"));
            dbConnectionStringBuilder.Add("Password", NAPI.Resource.GetSetting<string>(this, "DB_PASSWORD"));
            dbConnectionStringBuilder.Add("ConvertZeroDateTime", true);
            DbConnectionProvider.ConnectionString = dbConnectionStringBuilder.ConnectionString;

            // Test SQL Connection
            await DbConnectionProvider.TestConnection();

            // Get SMTP Settings (meta.xml)
            EmailSender.SmtpHost = NAPI.Resource.GetSetting<string>(this, "SMTP_HOST");
            EmailSender.SmtpPort = NAPI.Resource.GetSetting<int>(this, "SMTP_PORT");
            EmailSender.SmtpUsername = NAPI.Resource.GetSetting<string>(this, "SMTP_USERNAME");
            EmailSender.SmtpPassword = NAPI.Resource.GetSetting<string>(this, "SMTP_PASSWORD");
            // Have expired tokens get removed once per hour
            _expiredEmailTokensTimer = new Timer(OnRemoveExpiredEmailTokens, null, 1, 1000 * 60 * 60);

            // Read Sever World Settings from Database
            var worldData = await WorldModel.GetWorldData();
            WorldHandler.CurrentTime = worldData.ServerTime;
            NAPI.World.ResetIplList();

            // Initialize ValidVehicleIDs
            DataValidator.InitializeValidVehicleModelIDs();

            // Populate Item Template List
            ItemTemplate.LoadAllItems();
        }

        public static void OnServerShutdown()
        {
            _expiredEmailTokensTimer.Dispose();
        }

        private async void OnRemoveExpiredEmailTokens(object state)
        {
            await EmailToken.RemoveExpiredCodesAsync();
            Logger.GetInstance().ServerInfo("Removing expired email verification tokens from the database.");
        }
    }
}