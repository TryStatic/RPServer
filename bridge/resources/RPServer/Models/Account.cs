using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using RPServer.Database;
using RPServer.Models.Helpers;
using RPServer.Util;

namespace RPServer.Models
{
    internal class Account
    {
        public static readonly string DataKey = "ACCOUNT_DATA";

        public DbAcc DbData;

        public bool HasPassedTwoStepByGA = false;
        public bool HasPassedTwoStepByEmail = false;
        public byte[] TempTwoFactorGASharedKey = null;

        private Account(DbAcc dbData)
        {
            DbData = dbData;
        }

        #region DATABASE
        public static async Task CreateAsync(string username, string password, string regSocialClubName)
        {
            if (await ExistsAsync(username))
                return;

            var hash = new PasswordHash(password).ToArray();
            var newAcc = new DbAcc(username, hash, regSocialClubName);
            await newAcc.CreateAsync();
        }

        public static async Task<Account> FetchAsync(string username)
        {
            var sqlID = await DbAcc.GetSqlIdAsync(username);
            if (sqlID < 0) return null;

            var dbData = await DbAcc.ReadAsync(sqlID);
            var acc = new Account(dbData);
            return acc;

        }


        public static async Task<bool> ExistsAsync(string username)
        {
            var sqlID = await DbAcc.GetSqlIdAsync(username);
            return sqlID >= 0;
        }

        public async Task SaveAsync()
        {
            await DbData.UpdateAsync();
        }

        public static async Task<bool> AuthenticateAsync(string username, string password)
        {
            const string query = "SELECT username, hash FROM accounts WHERE username = @username LIMIT 1";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var cmd = dbConn.CreateCommandWithText(query);
                    cmd.AddParameterWithValue("@username", username);

                    await dbConn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
                            return false;

                        var fetchedPass = reader["hash"] as byte[];
                        return new PasswordHash(fetchedPass).Verify(password);
                    }
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
                return false;
            }
        }
        public static async Task<bool> IsEmailTakenAsync(string emailAddress)
        {
            const string query = "SELECT accountID FROM accounts WHERE emailaddress = @emailaddress";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var cmd = dbConn.CreateCommandWithText(query);
                    cmd.AddParameterWithValue("@emailaddress", emailAddress);

                    await dbConn.OpenAsync();
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        return await r.ReadAsync() && r.HasRows;
                    }
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
            throw new Exception("There was an error in [Account.IsEmailTakenAsync]");
        }
        #endregion

        public bool HasVerifiedEmail()
        {
            if (string.IsNullOrWhiteSpace(DbData.EmailAddress))
                return false;
            return true;

        }
        public bool Is2FAbyEmailEnabled() => DbData.HasEnabledTwoStepByEmail;
        public bool Is2FAbyGAEnabled() => DbData.TwoFactorGASharedKey != null;
        public bool IsTwoFactorAuthenticated()
        {
            if (!Is2FAbyEmailEnabled()) return !Is2FAbyGAEnabled() || HasPassedTwoStepByGA;
            if (!HasPassedTwoStepByEmail) return false;
            return !Is2FAbyGAEnabled() || HasPassedTwoStepByGA;
        }

        protected bool Equals(Account other)
        {
            return DbData.AccountID == other.DbData.AccountID;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Account)obj);
        }
        public override int GetHashCode()
        {
            return DbData.AccountID;
        }
        public static bool operator ==(Account left, Account right)
        {
            return Equals(left, right);
        }
        public static bool operator !=(Account left, Account right)
        {
            return !Equals(left, right);
        }
    }
}