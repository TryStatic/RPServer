using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using RPServer.Database;

namespace RPServer.Models
{
    internal abstract class Model<T> where T: Model<T>
    {
        [Key]
        public int ID { get; set; }

        #region CRUD
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
        public static async Task UpdateAllByKeyAsync<TC>(Expression<Func<TC>> expression, object searchKey, HashSet<T> dataref)
        {
            var dbRecords = (await ReadByKeyAsync(expression, searchKey)).ToHashSet();
            var data = dataref.ToHashSet();

            foreach (var dbRec in dbRecords)
            {
                if (data.Contains(dbRec))
                {
                    await UpdateAsync(dbRec);
                    data.Remove(dbRec);
                }
                else
                {
                    await DeleteAsync(dbRec);
                }
            }

            foreach (var i in data)
            {
                await CreateAsync(i);
            }

            dataref = (await ReadByKeyAsync(expression, searchKey)).ToHashSet();
        }

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
        #endregion

        public override bool Equals(object obj) => !ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) || obj.GetType() == this.GetType() && Equals((Model<T>) obj));
        protected bool Equals(Model<T> other) => ID == other.ID;
        public override int GetHashCode() => ID;
        public static bool operator ==(Model<T> left, Model<T> right) => Equals(left, right);
        public static bool operator !=(Model<T> left, Model<T> right) => !Equals(left, right);
    }
}