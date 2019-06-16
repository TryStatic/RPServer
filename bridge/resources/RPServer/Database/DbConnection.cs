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
                HandleMySqlException(ex);
                return false;
            }
        }

        public void Dispose()
        {
            Connection.Close();
        }

        private static void HandleMySqlException(MySqlException ex)
        {
            switch (ex.Code)
            {
                case (uint)MySqlErrorCode.None: // 0
                    Logger.MySqlError(DbStrings.ErrorNone, ex.Code);
                    break;
                case (uint)MySqlErrorCode.AccessDenied: // 1045
                    Logger.MySqlError(DbStrings.ErrorAccessDenied, ex.Code);
                    break;
                default: // Unspecified
                    Logger.MySqlError(DbStrings.ErrorUnspecified, ex.Code);
                    break;
            }
        }
    }
}
