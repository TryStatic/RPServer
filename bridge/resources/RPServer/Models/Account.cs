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

        #region SQL_SAVEABLE_DATA
        public int SqlId { get; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public byte[] Hash { get; set; }
        public string ForumName { get; set; }
        public string NickName { get; set; }
        public string RegSocialClubName { get; set; }
        public string LastSocialClubName { get; set; }
        public string LastIP { get; set; }
        public string LastHWID { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public bool HasEnabledTwoStepByEmail { get; set; }
        public byte[] TwoFactorGASharedKey { get; set; }
        #endregion

        #region Per_Client_Session_Variables
        public bool HasPassedTwoStepByGA = false;
        public bool HasPassedTwoStepByEmail = false;
        public byte[] TempTwoFactorGASharedKey = null;
        #endregion

        private Account(int sqlId)
        {
            SqlId = sqlId;
        }

        #region DATABASE
        public static async Task CreateAsync(string username, string password, string regSocialClubName)
        {
            if (await ExistsAsync(username))
                return;

            var hash = new PasswordHash(password).ToArray();

            const string query = "INSERT INTO accounts(username, hash, regsocialclubname, creationdate) VALUES (@username, @hash, @regsocialclubname, @creationdate)";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var cmd = dbConn.CreateCommandWithText(query);
                    cmd.AddParameterWithValue("@username", username);
                    cmd.AddParameterWithValue("@hash", hash);
                    cmd.AddParameterWithValue("@regsocialclubname", regSocialClubName);
                    cmd.AddParameterWithValue("@creationdate", DateTime.Now);
                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
        }
        public static async Task<Account> FetchAsync(string username)
        {
            const string query = "SELECT accountID, username, emailaddress, hash, forumname, nickname, LastIP, " +
                                 "LastHWID, regsocialclubname, lastsocialclubname, creationdate, lastlogindate, " +
                                 "enabled2FAbyemail, twofactorsharedkey" +
                                 " FROM accounts " +
                                 "WHERE username = @username LIMIT 1";

            if (!await ExistsAsync(username))
                return null;

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var cmd = dbConn.CreateCommandWithText(query);
                    cmd.AddParameterWithValue("@username", username);
                    await dbConn.OpenAsync();
                    using (var r = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess))
                    {
                        if (!await r.ReadAsync())
                            return null;

                        var sqlId = r.GetInt32Extended("accountID");
                        if(sqlId < 0) throw new Exception("Error fetching AccountID (SqlID) from the database");
                        var fetchedAcc = new Account(sqlId)
                        {
                            Username = r.GetStringExtended("username"),
                            EmailAddress = r.GetStringExtended("emailaddress"),
                            Hash = r["hash"] as byte[],
                            ForumName = r.GetStringExtended("forumname"),
                            NickName = r.GetStringExtended("nickname"),
                            LastIP = r.GetStringExtended("LastIP"),
                            LastHWID = r.GetStringExtended("LastHWID"),
                            RegSocialClubName = r.GetStringExtended("regsocialclubname"),
                            LastSocialClubName = r.GetStringExtended("lastsocialclubname"),
                            CreationDate = r.GetDateTimeExtended("creationdate"),
                            LastLoginDate = r.GetDateTimeExtended("lastlogindate"),
                            HasEnabledTwoStepByEmail = r.GetBooleanExtended("enabled2FAbyemail"),
                            TwoFactorGASharedKey = r["twofactorsharedkey"] as byte[]
                        };
                        return fetchedAcc;

                    }
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }

                return null;
            }
        }
        public static async Task<bool> ExistsAsync(string username)
        {
            const string query = "SELECT accountID FROM accounts WHERE username = @username";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var cmd = dbConn.CreateCommandWithText(query);
                    cmd.AddParameterWithValue("@username", username);
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
            throw new Exception("There was an error in [Account.ExistsAsync]");
        }
        public async Task SaveAsync()
        {
            const string query = "UPDATE accounts " +
                                 "SET username = @username, emailaddress = @emailaddress, hash = @hash," +
                                 "forumname = @forumname, nickname = @nickname, LastIP = @LastIP, LastHWID = @LastHWID," +
                                 "regsocialclubname = @regsocialclubname, lastsocialclubname = @lastsocialclubname," +
                                 "creationdate = @creationdate, lastlogindate = @lastlogindate, enabled2FAbyemail = @enabled2FAbyemail, twofactorsharedkey = @twofactorsharedkey " +
                                 "WHERE accountID = @sqlId";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var cmd = dbConn.CreateCommandWithText(query);
                    cmd.AddParameterWithValue("@sqlId", SqlId);

                    cmd.AddParameterWithValue("@username", Username);
                    cmd.AddParameterWithValue("@emailaddress", EmailAddress);
                    cmd.AddParameterWithValue("@hash", Hash);
                    cmd.AddParameterWithValue("@forumname", ForumName);
                    cmd.AddParameterWithValue("@nickname", NickName);
                    cmd.AddParameterWithValue("@LastIP", LastIP);
                    cmd.AddParameterWithValue("@LastHWID", LastHWID);
                    cmd.AddParameterWithValue("@regsocialclubname", RegSocialClubName);
                    cmd.AddParameterWithValue("@lastsocialclubname", LastSocialClubName);
                    cmd.AddParameterWithValue("@creationdate", CreationDate);
                    cmd.AddParameterWithValue("@lastlogindate", LastLoginDate);
                    cmd.AddParameterWithValue("@enabled2FAbyemail", HasEnabledTwoStepByEmail);
                    cmd.AddParameterWithValue("@twofactorsharedkey", TwoFactorGASharedKey);

                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
        }
        public static async Task DeleteAsync(string username)
        {
            const string query = "DELETE FROM accounts WHERE username = @username";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var cmd = dbConn.CreateCommandWithText(query);
                    cmd.AddParameterWithValue("@username", username);

                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
        }
        public static async Task<int> GetSqlIdAsync(string username)
        {
            const string query = "SELECT accountID FROM accounts WHERE username = @username LIMIT 1";

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
                            return -1;

                        var sqlId = reader.GetInt32Extended("accountID");
                        return sqlId;
                    }
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
                return -1;
            }
        }

        public async Task<List<Character>> GetCharactersAsync() => await Character.FetchAllAsync(this);
        public async Task<int> GetCharacterCountAsync() => await Character.FetchCount(this);

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
            if (string.IsNullOrWhiteSpace(EmailAddress))
                return false;
            return true;

        }
        public bool Is2FAbyEmailEnabled() => HasEnabledTwoStepByEmail;
        public bool Is2FAbyGAEnabled() => TwoFactorGASharedKey != null;
        public bool IsTwoFactorAuthenticated()
        {
            if (!Is2FAbyEmailEnabled()) return !Is2FAbyGAEnabled() || HasPassedTwoStepByGA;
            if (!HasPassedTwoStepByEmail) return false;
            return !Is2FAbyGAEnabled() || HasPassedTwoStepByGA;
        }

        protected bool Equals(Account other)
        {
            return SqlId == other.SqlId;
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
            return SqlId;
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