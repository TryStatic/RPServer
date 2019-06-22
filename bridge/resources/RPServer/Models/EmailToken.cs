using System;
using MySql.Data.MySqlClient;
using RPServer.Database;
using RPServer.Util;
using System.Threading.Tasks;

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


        public static async Task<bool> CreateAsync(Account account, string emailAddress)
        {
            const string query = "INSERT INTO emailtokens(accountID, token, expirydate, emailaddress) VALUES (@accountid, @token, @expirydate, @emailaddress)";

            if (await ExistsAsync(account))
                return false;

            var emailToken = new EmailToken(account, emailAddress);

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@accountid", emailToken.Account.SqlId);
                    cmd.Parameters.AddWithValue("@token", emailToken.Token);
                    cmd.Parameters.AddWithValue("@emailaddress", emailToken.EmailAddress);
                    cmd.Parameters.AddWithValue("@expirydate", emailToken.ExpiryDate);
                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
                return false;
            }
        }

        public static async Task<bool> ExistsAsync(Account account)
        {
            const string query = "SELECT accountID FROM emailtokens WHERE accountID = @accountid";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@accountid", account.SqlId);

                    await dbConn.OpenAsync();
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        if (!await r.ReadAsync()) return false;
                        return r.HasRows;
                    }
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
            }
            throw new Exception("There was an error in [EmailToken.ExistsAsync]");
        }

        public static async Task<bool> IsEmailTakenAsync(string emailAddress)
        {
            const string query = "SELECT emailaddress FROM emailtokens WHERE emailaddress = @emailaddress";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@emailaddress", emailAddress);

                    await dbConn.OpenAsync();
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        if (!await r.ReadAsync()) return false;
                        return r.HasRows;
                    }
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
            }
            throw new Exception("There was an error in [EmailToken.IsEmailTakenAsync]");
        }

        public static async Task<EmailToken> FetchAsync(Account account)
        {
            const string query = "SELECT accountID, token, expirydate, emailaddress FROM emailtokens WHERE accountID = @accountid";

            if (!await ExistsAsync(account))
                return null;

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@accountid", account.SqlId);

                    await dbConn.OpenAsync();
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        if (!await r.ReadAsync()) return null;
                        return new EmailToken(account, r.GetStringExtended("token"), r.GetDateTimeExtended("expirydate"), r.GetStringExtended("emailaddress"));
                    }
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
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

            account.EmailAddress = fetchedToken.EmailAddress;
            await RemoveAsync(account);
            return true;
        }

        public async Task SaveAsync()
        {
            const string query = "UPDATE emailtokens " +
                                 "SET token = @token, emailAddress = @emailaddress, expirydate = @expirydate " +
                                 "WHERE accountID = @sqlId";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@sqlId", Account.SqlId);

                    cmd.Parameters.AddWithValue("@token", Token);
                    cmd.Parameters.AddWithValue("@emailaddress", EmailAddress);
                    cmd.Parameters.AddWithValue("@expirydate", ExpiryDate);

                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
            }
        }

        private static async Task RemoveAsync(Account account)
        {
            if (!await ExistsAsync(account)) return;

            const string query = "DELETE FROM emailtokens WHERE accountID = @accountid";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@accountid", account.SqlId);

                    await dbConn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return;
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
            }
            throw new Exception("Error in [EmailTokens.RemoveAsync]");
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

        #region TokenCodeGeneration
        private static string GenerateNewToken()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        private static int GetTokenLength()
        {
            // NewGuid() is always 36 chars hence after Replace(..) the rest is specific too
            return Guid.NewGuid().ToString().Replace("-", "").Length;
        }
        #endregion
    }
}