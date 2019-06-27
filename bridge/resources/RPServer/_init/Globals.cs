using System.Threading;
using EventNames;
using GTANetworkAPI;
using RPServer.Database;
using RPServer.Models;
using RPServer.Util;

namespace RPServer._init
{
    internal class Globals : Script
    {
        public static RandomGenerator Random = new RandomGenerator();

        private static Timer _expiredEmailTokensTimer;

        [ServerEvent(Event.ResourceStart)]
        public async void OnResourceStart()
        {
            // Server Settings
            //NAPI.Server.SetAutoSpawnOnConnect(false);
            //NAPI.Server.SetAutoRespawnAfterDeath(false);
            //NAPI.Server.SetDefaultSpawnLocation();
            NAPI.Server.SetGlobalServerChat(false);

            // Sever World Settings
            NAPI.World.SetTime(0, 0, 0);
            NAPI.World.ResetIplList();
            
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
        {
            var username = "UNREGISTERED";
            if (client.IsLoggedIn())
            {
                var accData = client.GetAccountData();
                username = accData.Username;
            }
            Logger.CommandLog(username, cmd);
        }

        private async void OnRemoveExpiredEmailTokens(object state)
        {
            await EmailToken.RemoveExpiredCodesAsync();
            _expiredEmailTokensTimer.Change(1000 * 60 * 60, Timeout.Infinite);

        }
    }
}