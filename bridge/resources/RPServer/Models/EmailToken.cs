using System;
using MySql.Data.MySqlClient;
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


        public static bool Create(Account account, string emailAddress)
        {
            const string query = "INSERT INTO emailtokens(accountID, token, expirydate, emailaddress) VALUES (@accountid, @token, @expirydate, @emailaddress)";

            if (Exists(account))
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
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
                return false;
            }
        }

        public static bool Exists(Account account)
        {
            const string query = "SELECT accountID FROM emailtokens WHERE accountID = @accountid";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@accountid", account.SqlId);

                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read()) return false;
                        return r.HasRows;
                    }
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
            }
            throw new Exception("There was an error in [EmailToken.Exists]");
        }

        private static EmailToken Fetch(Account account)
        {
            const string query = "SELECT accountID, token, expirydate, emailaddress FROM emailtokens WHERE accountID = @accountid";

            if (!Exists(account))
                return null;

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@accountid", account.SqlId);

                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read()) return null;
                        return new EmailToken(account, r.GetSafeString("token"), r.GetSafeDateTime("expirydate"), r.GetSafeString("emailaddress"));
                    }
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
            }
            throw new Exception("Error in [EmailToken.Fetch]");
        }

        public static bool Validate(Account account, string token)
        {
            if (!Exists(account))
                return false;

            var fetchedToken = Fetch(account);


            if (fetchedToken.ExpiryDate < DateTime.Now)
            { // Expired Token
                Remove(account);
                return false;
            }
            if (fetchedToken.Token != token) 
                return false;

            account.EmailAddress = fetchedToken.EmailAddress;
            Remove(account);
            return true;
        }

        private void Save()
        {
            throw new NotImplementedException();
        }

        private static void Remove(Account account)
        {
            if (!Exists(account)) return;

            const string query = "DELETE FROM emailtokens WHERE accountID = @accountid";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@accountid", account.SqlId);
                    cmd.ExecuteNonQuery();
                    return;
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
            }
            throw new Exception("Error in [EmailTokens.Remove]");
        }


        public static void ChangeEmail(Account account, string newEmailAddress)
        {
            var fetchedToken = Fetch(account);

            fetchedToken.EmailAddress = newEmailAddress;
            fetchedToken.Token = GenerateNewToken();
            fetchedToken.ExpiryDate = DateTime.Now.AddDays(1);
            fetchedToken.Save();
        }

        public static void SendEmail(Account account)
        {
            throw new NotImplementedException();
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