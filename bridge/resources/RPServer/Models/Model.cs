using System.Data.Common;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using RPServer.Database;

namespace RPServer.Models
{
    internal abstract class Model<T> where T: Model<T>
    {
        public static async Task<int> CreateAsync(T newEntry)
        {
            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    return await dbConn.InsertAsync(newEntry);
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return -1;
            }
        }
        public async Task<int> CreateAsync() => await CreateAsync(this as T);

        public static async Task<T> ReadAsync(int sqlID)
        {
            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    return await dbConn.GetAsync<T>(sqlID);
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return null;
            }
        }

        public static async Task<bool> UpdateAsync(T entry)
        {
            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    return await dbConn.UpdateAsync(entry);
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return false;
            }
        }
        public async Task<bool> UpdateAsync() => await UpdateAsync(this as T);

        public static async Task<bool> DeleteAsync(T entry)
        {
            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    return await dbConn.DeleteAsync(entry);
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return false;
            }
        }
        public async Task<bool> DeleteAsync() => await DeleteAsync(this as T);
    }
}