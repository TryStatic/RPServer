using System;
using System.Threading.Tasks;
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

        public DbConnection()
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
        }

        public async Task<bool> OpenAsync()
        {
            try
            {
                await Connection.OpenAsync();
                return true;
            }
            catch (MySqlException ex)
            {
                Logger.MySqlError(ex.Message, ex.Code);
                return false;
            }
        }

        public static async Task<bool> TestConnection()
        {
            using (var dbConn = new DbConnection())
            {
                var connected = false;
                var tries = 0;
                do
                {
                    tries++;
                    Logger.MySqlInfo($"{DbStrings.InfoTryDBConnect}... ({tries})");
                    connected = await dbConn.OpenAsync();
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
