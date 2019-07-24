using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using RPServer.Database;
using RPServer.Util;

namespace RPServer.Models
{
    internal class EmailToken
    {
        public static int Length = GetTokenLength();

        public Account Account { get; set; }
        public string Token { get; set; }
        public string EmailAddress { get; set; }
        public DateTime ExpiryDate { get; set; }

        private EmailToken(Account account, string emailAddress)
        {
            Account = account;
            Token = GenerateNewToken();
            EmailAddress = emailAddress;
            ExpiryDate = DateTime.Now.AddDays(1);
        }

        private EmailToken(Account account, string token, DateTime expiryDate, string emailAddress)
        {
            Account = account;
            Token = token;
            EmailAddress = emailAddress;
            ExpiryDate = expiryDate;
        }

        #region DATABASE
        public static async Task<bool> CreateAsync(Account account, string emailAddress)
        {
            const string query = "INSERT INTO emailtokens(accountID, token, expirydate, emailaddress) VALUES (@accountid, @token, @expirydate, @emailaddress)";

            if (await ExistsAsync(account)) await RemoveAsync(account);

            var emailToken = new EmailToken(account, emailAddress);

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    await dbConn.ExecuteAsync(query, new
                    {
                        accountid = emailToken.Account.ID,
                        token = emailToken.Token,
                        emailaddress = emailToken.EmailAddress,
                        expirydate = emailToken.ExpiryDate

                    });
                    return true;
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
                return false;
            }
        }
        public static async Task<bool> ExistsAsync(Account account)
        {
            const string query = "SELECT accountID FROM emailtokens WHERE accountID = @accountid";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var result = await dbConn.QueryAsync<int?>(query, new { accountid = account.ID });
                    return result.Any();
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
            throw new Exception("There was an error in [EmailToken.ExistsAsync]");
        }
        public static async Task<bool> IsEmailTakenAsync(string emailAddress)
        {
            const string query = "SELECT emailaddress FROM emailtokens WHERE emailaddress = @emailaddress";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {

                try
                {
                    var result = await dbConn.QueryAsync(query, new { emailaddress = emailAddress });
                    return result.Any();
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
            throw new Exception("There was an error in [EmailToken.IsEmailTakenAsync]");
        }
        public static async Task<EmailToken> FetchAsync(Account account)
        {
            const string query = "SELECT accountID, token, expirydate, emailaddress FROM emailtokens WHERE accountID = @accountid";

            if (!await ExistsAsync(account))
                return null;

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    var result = await dbConn.QueryAsync(query, new { accountid = account.ID } );
                    var unwrapped = result.SingleOrDefault();

                    if (unwrapped == null) return null;
                    return new EmailToken(account, unwrapped.token, unwrapped.expirydate, unwrapped.emailaddress);
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
            throw new Exception("Error in [EmailToken.FetchAsync]");
        }
        public static async Task<bool> ValidateAsync(Account account, string token)
        {
            if (!await ExistsAsync(account))
                return false;

            var fetchedToken = await FetchAsync(account);

            if (fetchedToken.ExpiryDate < DateTime.Now)
            { // Expired Token
                await RemoveAsync(account);
                return false;
            }
            if (fetchedToken.Token != token) 
                return false;

            await RemoveAsync(account);
            return true;
        }
        public async Task SaveAsync()
        {
            const string query = "UPDATE emailtokens " +
                                 "SET token = @token, emailAddress = @emailaddress, expirydate = @expirydate " +
                                 "WHERE accountID = @sqlId";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    await dbConn.ExecuteAsync(query, new
                    {
                        sqlId = Account.ID,
                        token = Token,
                        emailaddress = EmailAddress,
                        expirydate = ExpiryDate
                    });
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
        }
        public static async Task RemoveExpiredCodesAsync()
        {
            const string query = "DELETE FROM emailtokens WHERE expirydate < @current";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    await dbConn.ExecuteAsync(query, new { current = DateTime.Now });
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
        }
        private static async Task RemoveAsync(Account account)
        {
            if (!await ExistsAsync(account)) return;

            const string query = "DELETE FROM emailtokens WHERE accountID = @accountid";

            using (var dbConn = DbConnectionProvider.CreateDbConnection())
            {
                try
                {
                    await dbConn.ExecuteAsync(query, new {accountid = account.ID });
                }
                catch (DbException ex)
                {
                    DbConnectionProvider.HandleDbException(ex);
                }
            }
        }
        public static async Task ChangeEmailAsync(Account account, string newEmailAddress)
        {
            var fetchedToken = await FetchAsync(account);

            fetchedToken.EmailAddress = newEmailAddress;
            fetchedToken.Token = GenerateNewToken();
            fetchedToken.ExpiryDate = DateTime.Now.AddDays(1);
            await fetchedToken.SaveAsync();
        }
        public static async Task SendEmail(Account account)
        {
            var tok = await FetchAsync(account);
            await EmailSender.SendMailMessageAsync(tok.EmailAddress, "RPServer - Email Verifciaton", $"Your verification token is {tok.Token} and it's valid until {tok.ExpiryDate}.");

        }
        #endregion

        #region TokenCodeGeneration
        private static string GenerateNewToken()
        {
            return RandomGenerator.GetInstance().Next(100000, 1000000).ToString();
        }
        private static int GetTokenLength()
        {
            return 6;
        }
        #endregion
    }
}