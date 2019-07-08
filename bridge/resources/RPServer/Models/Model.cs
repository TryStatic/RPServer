using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
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
        public static async Task<IEnumerable<T>> ReadByKeyAsync<TC>(Expression<Func<TC>> expression, object searchKey)
        {
            var tAttribute = (TableAttribute)typeof(T).GetCustomAttributes(typeof(TableAttribute), true)[0];
            var tableName = tAttribute.Name;
            if (tableName == null) return null;


            var expressionBody = expression.Body as MemberExpression;
            var expressionMember = expressionBody?.Member;
            var columnName = expressionMember?.Name;
            if (columnName == null) return null;

            var query = $"SELECT * FROM {tableName} WHERE {columnName} = @value;";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    return await dbConn.QueryAsync<T>(query, new { value = searchKey });
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
            return null;
        }
        public static async Task<bool> ExistsAsync(int sqlID)
        {
            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    return (await dbConn.GetAsync<T>(sqlID)) != null;
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return false;
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