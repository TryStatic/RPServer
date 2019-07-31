using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using RPServer.Models.Util;

namespace RPServer.Models
{
    [Table("accounts")]
    internal class AccountModel : Model<AccountModel>
    {
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
        public int LastSpawnedCharId { get; set; } = -1;
        public ushort AdminLevel { set; get; }


        public bool HasPassedTwoStepByGA = false;
        public bool HasPassedTwoStepByEmail = false;
        public byte[] TempTwoFactorGASharedKey = null;

        public AccountModel() { }

        public AccountModel(string username, byte[] hash, string regSocialClubName)
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
            var newAcc = new AccountModel(username, hash, regSocialClubName);
            await newAcc.CreateAsync();
        }
        public static async Task<AccountModel> FetchAsync(string username)
        {
            var result = await ReadByKeyAsync(() => AccountModel.Mock.Username, username);
            return result.FirstOrDefault();
        }
        public static async Task<bool> ExistsAsync(string username)
        {
            var result = await ReadByKeyAsync(() => AccountModel.Mock.Username, username);
            return result.FirstOrDefault() != null;
        }
        public static async Task<bool> AuthenticateAsync(string username, string password)
        {
            var acc = await FetchAsync(username);
            return acc != null && new PasswordHash(acc.Hash).Verify(password);
        }
        public static async Task<bool> IsEmailTakenAsync(string emailAddress)
        {
            var accList = await ReadByKeyAsync(() => AccountModel.Mock.EmailAddress, emailAddress);
            return accList.FirstOrDefault() != null;
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
    }
}