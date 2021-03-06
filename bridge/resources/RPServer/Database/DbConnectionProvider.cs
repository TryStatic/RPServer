﻿using System.Data.Common;
using RPServer.Util;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace RPServer.Database
{
    internal static class DbConnectionProvider
    {
        public static string ProviderName { get; set; }
        public static string ConnectionString { get; set; }

        public static DbConnection CreateDbConnection()
        {
            DbConnection connection;

            if (ConnectionString == null)
            {
                Logger.GetInstance().SqlError("Tried CreateDbConnection() with null ConnectionString.");
                return null;
            }

            try
            {
                var factory = GenerateDbProviderFactory();
                connection = factory.CreateConnection();
                if (connection == null)
                {
                    Logger.GetInstance().SqlError("DbProviderFactory was unable to retrieve a connection.");
                    return null;
                }
                connection.ConnectionString = ConnectionString;
            }
            catch(DbException e)
            {
                connection = null;
                HandleDbException(e);
            }

            return connection;
        }

        public static DbConnectionStringBuilder CreateDbConnectionStringBuilder()
        {
            var factory = GenerateDbProviderFactory();
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
                    Logger.GetInstance().SqlInfo($"Trying to establish a connection with the MySQL Database... ({tries})");
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
                    Logger.GetInstance().SqlInfo("Failed to establish a connection with the MySQL Database");
                    return false;
                }
                Logger.GetInstance().SqlInfo("Successfully established a connection with the MySQL Database");
            }
            return true;
        }

        private static DbProviderFactory GenerateDbProviderFactory()
        {
            if (ProviderName == DbProvider.MySql)
            {
                return MySqlClientFactory.Instance;
            }

            return null;
        }

        public static void HandleDbException(DbException exception)
        {
            if(exception is MySqlException mySqlException)
            {
                Logger.GetInstance().MySqlError(mySqlException.Message, mySqlException.Code);
            }
            else
            {
                Logger.GetInstance().SqlError(exception.ToString());
            }
        }

        private struct DbProvider
        {
            private DbProvider(string value) => ProviderString = value;
            private string ProviderString { get; }
            public override string ToString() => ProviderString;
            public static implicit operator string(DbProvider provider) => provider.ToString();

            // Providers list
            public static DbProvider MySql => new DbProvider("MySql.Data.MySqlClient");

        }
    }
}
