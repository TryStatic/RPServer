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
    }
}