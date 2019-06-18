using System;
using MySql.Data.MySqlClient;
using RPServer.Database;
using RPServer.Util;


namespace RPServer.Models
{
    internal class Account
    {
        public int? SqlId { get; private set; }
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

        private Account()
        {

        }

        public static Account Create(string username, string password, string regSocialClubName)
        {
            if (Exists(username))
                return null;

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
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
            }
            return Fetch(username);
        }
        public static Account Fetch(string username)
        {
            const string query = "SELECT accountID, username, emailaddress, hash, forumname, nickname, LastIP, " +
                                 "LastHWID, regsocialclubname, lastsocialclubname, creationdate, lastlogindate " +
                                 "FROM accounts " +
                                 "WHERE username = @username LIMIT 1";

            if (!Exists(username))
                return null;

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@username", username);

                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                            return null;

                        var fetchedAcc = new Account
                        {
                            SqlId = r.GetInt32("accountID"),
                            Username = r.GetSafeString("username"),
                            EmailAddress = r.GetSafeString("emailaddress"),
                            Hash = r["hash"] as byte[],
                            ForumName = r.GetSafeString("forumname"),
                            NickName = r.GetSafeString("nickname"),
                            LastIP = r.GetSafeString("LastIP"),
                            LastHWID = r.GetSafeString("LastHWID"),
                            RegSocialClubName = r.GetSafeString("regsocialclubname"),
                            LastSocialClubName = r.GetSafeString("lastsocialclubname"),
                            CreationDate = r.GetSafeDateTime("creationdate"),
                            LastLoginDate = r.GetSafeDateTime("lastlogindate")
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
        public static bool Exists(string username)
        {
            const string query = "SELECT accountID FROM accounts WHERE username = @username";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@username", username);

                    using (var r = cmd.ExecuteReader())
                    {
                        return r.Read() && r.HasRows;
                    }
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
            }
            throw new Exception("There was an error in [Account.Exists]");
        }
        public void Save()
        {
            const string query = "UPDATE accounts " +
                                 "SET accountID = @accountID, username = @username, emailaddress = @emailaddress, hash = @hash," +
                                 "forumname = @forumname, nickname = @nickname, LastIP = @LastIP, LastHWID = @LastHWID," +
                                 "regsocialclubname = @regsocialclubname, lastsocialclubname = @lastsocialclubname," +
                                 "creationdate = @creationdate, lastlogindate = @lastlogindate " +
                                 "WHERE accountID = @sqlId";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@sqlId", SqlId);

                    cmd.Parameters.AddWithValue("@accountID", SqlId);
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
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
            }
        }

        public static bool Authenticate(string username, string password)
        {
            const string query = "SELECT username, hash FROM accounts WHERE username = @username LIMIT 1";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@username", username);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
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
        public static int? GetSqlId(string username)
        {
            const string query = "SELECT accountID FROM accounts WHERE username = @username";

            using (var dbConn = new DbConnection())
            {
                try
                {
                    var cmd = new MySqlCommand(query, dbConn.Connection);
                    cmd.Parameters.AddWithValue("@username", username);

                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                            return null;

                        return r.GetInt32("accountID");
                    }
                }
                catch (MySqlException ex)
                {
                    Logger.MySqlError(ex.Message, ex.Code);
                }
                return null;
            }
        }
    }
}