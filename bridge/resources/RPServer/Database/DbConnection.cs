using System;
using MySql.Data.MySqlClient;
using RPServer.Util;

namespace RPServer.Database
{
    internal class DbConnection : IDisposable
    {
        public readonly MySqlConnection Connection;

        public DbConnection()
        {
            // Build Connection String
            var mysqlCbs = new MySqlConnectionStringBuilder
            {
                Server = DbSettings.DB_HOST,
                Database = DbSettings.DB_DATABASE,
                Port = uint.Parse(DbSettings.DB_PORT),
                UserID = DbSettings.DB_USERNAME,
                Password = DbSettings.DB_PASSWORD
            };
            // Create MySQL Connection Instance
            Connection = new MySqlConnection(mysqlCbs.ConnectionString);

        }

        private bool OpenConnection()
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
            using (var dbConn = new DbConnection())
            {
                var connected = false;
                var tries = 0;
                do
                {
                    tries++;
                    Logger.MySqlInfo($"{DbStrings.InfoTryDBConnect}... ({tries})");
                    connected = dbConn.OpenConnection();
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
