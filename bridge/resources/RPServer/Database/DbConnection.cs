using System;
using MySql.Data.MySqlClient;
using RPServer.Strings;
using RPServer.Util;

namespace RPServer.Database
{
    internal class DbConnection : IDisposable
    {
        public readonly MySqlConnection Connection;

        public static string MySqlHost { get; set; }
        public static uint MySqlPort { get; set; }
        public static string MySqlDatabase { get; set; }
        public static string MySqlUsername { get; set; }
        public static string MySqlPassword { get; set; }

        public DbConnection(bool autoConnect = true)
        {
            // Build Connection String
            var mysqlCbs = new MySqlConnectionStringBuilder
            {
                Server = MySqlHost,
                Port = MySqlPort,
                Database = MySqlDatabase,
                UserID = MySqlUsername,
                Password = MySqlPassword,
                ConvertZeroDateTime = true
            };

            // Create MySQL Connection Instance
            Connection = new MySqlConnection(mysqlCbs.ConnectionString);

            if (autoConnect) Open();

        }

        public bool Open()
        {
            try
            {
                Connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                Logger.MySqlError(ex.Message, ex.Code);
                return false;
            }
        }

        public static bool TestConnection()
        {
            using (var dbConn = new DbConnection(false))
            {
                var connected = false;
                var tries = 0;
                do
                {
                    tries++;
                    Logger.MySqlInfo($"{DbStrings.InfoTryDBConnect}... ({tries})");
                    connected = dbConn.Open();
                } while (!connected && tries < 10);

                if (tries >= 10)
                {
                    Logger.MySqlInfo(DbStrings.InfoFailedDBConnect);
                    return false;
                }
                Logger.MySqlInfo(DbStrings.InfoSuccessDBConnect);
            }
            return true;
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}
