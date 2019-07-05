using System;
using System.Data.Common;
using RPServer.Util;
using System.Threading.Tasks;
using RPServer.Strings;
using MySql.Data.MySqlClient;

namespace RPServer.Database
{
    internal static class DbConnectionProvider
    {
        public static string ProviderName { get; set; }
        public static string ConnectionString { get; set; }

        public static DbConnection CreateDbConnection()
        {
            DbConnection connection = null;

            if (ConnectionString != null)
            {
                try
                {
                    DbProviderFactory factory = GenerateDbProviderFactory();

                    connection = factory.CreateConnection();
                    connection.ConnectionString = ConnectionString;
                }
                catch(DbException e)
                {
                    if (connection != null)
                    {
                        connection = null;
                    }
                    HandleDbException(e);
                }
            }

            return connection;
        }

        public static DbConnectionStringBuilder CreateDbConnectionStringBuilder()
        {
            DbProviderFactory factory = GenerateDbProviderFactory();
            return factory.CreateConnectionStringBuilder();
        }

        public static async Task<bool> TestConnection()
        {
            using (var dbConn = CreateDbConnection())
            {
                var connected = false;
                var tries = 0;
                do
                {
                    tries++;
                    Logger.GetInstance().SqlInfo($"{DbStrings.InfoTryDBConnect}... ({tries})");
                    try
                    {
                        await dbConn.OpenAsync();
                        connected = true;
                    }
                    catch (DbException)
                    {
                        connected = false;
                    }
                } while (!connected && tries < 10);

                if (tries >= 10)
                {
                    Logger.GetInstance().SqlInfo(DbStrings.InfoFailedDBConnect);
                    return false;
                }
                Logger.GetInstance().SqlInfo(DbStrings.InfoSuccessDBConnect);
            }
            return true;
        }

        private static DbProviderFactory GenerateDbProviderFactory()
        {
            if (ProviderName == "MySql.Data.MySqlClient")
            {
                return MySqlClientFactory.Instance;
            }

            return null;
        }

        public static void HandleDbException(DbException exception)
        {
            if(exception is MySqlException)
            {
                MySqlException mySqlException = (MySqlException)exception;
                Logger.GetInstance().MySqlError(mySqlException.Message, mySqlException.Code);
            }
            else
            {
                Logger.GetInstance().SqlError(exception.ToString());
            }
        }
    }
}
