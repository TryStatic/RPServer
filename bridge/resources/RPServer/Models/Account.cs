using System;
using System.Data;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using RPServer.Database;
using RPServer.Util;

namespace RPServer.Models
{
    internal class Account : SaveableData
    {
        public static readonly string DataKey = "ACCOUNT_DATA";

        #region SQL_SAVEABLE_DATA
        public int SqlId { get; }
        [SqlColumnName("username")]
        public string Username { get; set; }
        [SqlColumnName("emailaddress")]
        public string EmailAddress { get; set; }
        [SqlColumnName("hash")]
        public byte[] Hash { get; set; }
        [SqlColumnName("forumname")]
        public string ForumName { get; set; }
        [SqlColumnName("nickname")]
        public string NickName { get; set; }
        [SqlColumnName("regsocialclubname")]
        public string RegSocialClubName { get; set; }
        [SqlColumnName("lastsocialclubname")]
        public string LastSocialClubName { get; set; }
        [SqlColumnName("LastIP")]
        public string LastIP { get; set; }
        [SqlColumnName("LastHWID")]
        public string LastHWID { get; set; }
        [SqlColumnName("creationdate")]
        public DateTime CreationDate { get; set; }
        [SqlColumnName("lastlogindate")]
        public DateTime LastLoginDate { get; set; }
        [SqlColumnName("enabled2FAbyemail")]
        public bool HasEnabledTwoStepByEmail { get; set; }
        [SqlColumnName("twofactorsharedkey")]
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

        #region CRUD
        public static async Task CreateAsync(string username, string password, string regSocialClubName)
        {
            if (await ExistsAsync(username))
                return;

            var hash = new PasswordHash(password).ToArray();

            const string query = "INSERT INTO accounts(username, hash, regsocialclubname, creationdate) VALUES (@username, @hash, @regsocialclubname, @creationdate)";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@hash", hash);
                    cmd.Parameters.AddWithValue("@regsocialclubname", regSocialClubName);
                    cmd.Parameters.AddWithValue("@creationdate", DateTime.Now);
                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
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

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@username", username);
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
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }

                return null;
            }
        }
        public static async Task<bool> ExistsAsync(string username)
        {
            const string query = "SELECT accountID FROM accounts WHERE username = @username";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@username", username);
                    await dbConn.OpenAsync();
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        return await r.ReadAsync() && r.HasRows;
                    }
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
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

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@sqlId", SqlId);

                    cmd.Parameters.AddWithValue("@username", Username);
                    cmd.Parameters.AddWithValue("@emailaddress", EmailAddress);
                    cmd.Parameters.AddWithValue("@hash", Hash);
                    cmd.Parameters.AddWithValue("@forumname", ForumName);
                    cmd.Parameters.AddWithValue("@nickname", NickName);
                    cmd.Parameters.AddWithValue("@LastIP", LastIP);
                    cmd.Parameters.AddWithValue("@LastHWID", LastHWID);
                    cmd.Parameters.AddWithValue("@regsocialclubname", RegSocialClubName);
                    cmd.Parameters.AddWithValue("@lastsocialclubname", LastSocialClubName);
                    cmd.Parameters.AddWithValue("@creationdate", CreationDate);
                    cmd.Parameters.AddWithValue("@lastlogindate", LastLoginDate);
                    cmd.Parameters.AddWithValue("@enabled2FAbyemail", HasEnabledTwoStepByEmail);
                    cmd.Parameters.AddWithValue("@twofactorsharedkey", TwoFactorGASharedKey);

                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
            }
        }
        public async Task SaveSingleAsync<T>(Expression<Func<T>> expression)
        {
            var column = GetColumnName(expression, out var value);
            if(string.IsNullOrEmpty(column) || string.IsNullOrWhiteSpace(column)) throw new Exception("Invalid Column Name");

            var query = $"UPDATE accounts SET {column} = @value WHERE accountID = @sqlId";


            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@sqlId", SqlId);
                    cmd.Parameters.AddWithValue("@value", value);

                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
            }
        }
        public static async Task<bool> DeleteAsync(string username)
        {
            // TODO: Delete pending email tokens related to that account
            // TODO: Delete characters related to that account
            throw new NotImplementedException();
        }
        #endregion

        public static async Task<bool> AuthenticateAsync(string username, string password)
        {
            const string query = "SELECT username, hash FROM accounts WHERE username = @username LIMIT 1";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@username", username);

                    await dbConn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
                            return false;

                        var fetchedPass = reader["hash"] as byte[];
                        return new PasswordHash(fetchedPass).Verify(password);
                    }
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
                return false;
            }
        }
        public static async Task<bool> IsEmailTakenAsync(string emailAddress)
        {
            const string query = "SELECT accountID FROM accounts WHERE emailaddress = @emailaddress";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@emailaddress", emailAddress);

                    await dbConn.OpenAsync();
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        return await r.ReadAsync() && r.HasRows;
                    }
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
            }
            throw new Exception("There was an error in [Account.IsEmailTakenAsync]");
        }
        public bool HasVerifiedEmail()
        {
            if (string.IsNullOrEmpty(EmailAddress) || string.IsNullOrWhiteSpace(EmailAddress))
                return false;
            return true;

        }
        public bool Is2FAbyEmailEnabled() => HasEnabledTwoStepByEmail;
        public bool Is2FAbyGAEnabled() => TwoFactorGASharedKey != null;

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

    internal sealed class PasswordHash
    {
        private const int SaltSize = 20;
        private const int HashSize = 30;
        private const int HashIter = 10000;

        private readonly byte[] _salt;
        private readonly byte[] _hash;

        public byte[] Salt => (byte[])_salt.Clone();
        public byte[] Hash => (byte[])_hash.Clone();

        public PasswordHash(string password)
        {
            new RNGCryptoServiceProvider().GetBytes(_salt = new byte[SaltSize]);
            _hash = new Rfc2898DeriveBytes(password, _salt, HashIter).GetBytes(HashSize);
        }


        public PasswordHash(byte[] hashBytes)
        {
            Array.Copy(hashBytes, 0, _salt = new byte[SaltSize], 0, SaltSize);
            Array.Copy(hashBytes, SaltSize, _hash = new byte[HashSize], 0, HashSize);
        }

        public PasswordHash(byte[] salt, byte[] hash)
        {
            Array.Copy(salt, 0, _salt = new byte[SaltSize], 0, SaltSize);
            Array.Copy(hash, 0, _hash = new byte[HashSize], 0, HashSize);
        }

        public byte[] ToArray()
        {
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(_salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(_hash, 0, hashBytes, SaltSize, HashSize);
            return hashBytes;
        }

        public bool Verify(string password)
        {
            byte[] test = new Rfc2898DeriveBytes(password, _salt, HashIter).GetBytes(HashSize);
            for (int i = 0; i < HashSize; i++)
                if (test[i] != _hash[i])
                    return false;
            return true;
        }
    }

}