using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using RPServer.Database;
using RPServer.Models.Helpers;
using RPServer.Util;

namespace RPServer.Models
{
    [Table("accounts")]
    internal class Account : Model<Account>
    {
        public static readonly string DataKey = "ACCOUNT_DATA";

        [Key]
        public int AccountID { get; set; }
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


        public bool HasPassedTwoStepByGA = false;
        public bool HasPassedTwoStepByEmail = false;
        public byte[] TempTwoFactorGASharedKey = null;

        public Account() { }

        public Account(string username, byte[] hash, string regSocialClubName)
        {
            Username = username;
            Hash = hash;
            RegSocialClubName = regSocialClubName;
            CreationDate = DateTime.Now;
        }

        #region DATABASE
        public static async Task CreateAsync(string username, string password, string regSocialClubName)
        {
            var hash = new PasswordHash(password).ToArray();
            var newAcc = new Account(username, hash, regSocialClubName);
            await newAcc.CreateAsync();
        }
        public static async Task<Account> FetchAsync(string username)
        {
            var result = await ReadByKeyAsync(() => new Account().Username, username);
            var accountModels = result.ToList();
            return accountModels.Any() ? accountModels.First() : null;
        }

        public static async Task<bool> ExistsAsync(string username)
        {
            var result = await ReadByKeyAsync(() => new Account().Username, username);
            var accountModels = result.ToList();
            return accountModels.Any();
        }
        public async Task SaveAsync()
        {
            await UpdateAsync();
        }


        public static async Task<bool> AuthenticateAsync(string username, string password)
        { // TODO: Move to model
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
        { // TODO: Move to model
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
            return AccountID == other.AccountID;
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
            return AccountID;
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