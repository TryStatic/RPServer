using GTANetworkAPI;
using RPServer.Database;
using RPServer.Util;

namespace RPServer._init
{
    internal class Init : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            // Server Settings
            //NAPI.Server.SetAutoSpawnOnConnect(false);
            //NAPI.Server.SetAutoRespawnAfterDeath(false);

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
            DbConnection.TestConnection();

            EmailSender.SmtpHost = NAPI.Resource.GetSetting<string>(this, "SMTP_HOST");
            EmailSender.SmtpPort = NAPI.Resource.GetSetting<int>(this, "SMTP_PORT");
            EmailSender.SmtpUsername = NAPI.Resource.GetSetting<string>(this, "SMTP_USERNAME");
            EmailSender.SmtpPassword = NAPI.Resource.GetSetting<string>(this, "SMTP_PASSWORD");

        }
    }
}