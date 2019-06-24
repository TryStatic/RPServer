using System;

namespace RPServer.Models
{
    internal static class Savable
    {
        public enum Column
        {
            Username,
            EmailAddress,
            Hash,
            ForumName,
            NickName,
            RegSocialClubName,
            LastSocialClubName,
            LastIP,
            LastHWID,
            CreationDate,
            LastLoginDate,
            HasEnabledTwoStepByEmail,
            TwoFactorGASharedKey
        }

        public static void GetColumnAndValue(Account acc, Column c, out string column, out object value)
        {
            switch (c)
            {
                case Column.Username:
                    column = "username"; value = acc.Username; break;
                case Column.EmailAddress:
                    column = "emailaddress"; value = acc.EmailAddress; break;
                case Column.Hash:
                    column = "hash"; value = acc.Hash; break;
                case Column.ForumName:
                    column = "forumname"; value = acc.ForumName; break;
                case Column.NickName:
                    column = "nickname"; value = acc.NickName; break;
                case Column.RegSocialClubName:
                    column = "regsocialclubname"; value = acc.RegSocialClubName; break;
                case Column.LastSocialClubName:
                    column = "lastsocialclubname"; value = acc.LastSocialClubName; break;
                case Column.LastIP:
                    column = "LastIP"; value = acc.LastIP; break;
                case Column.LastHWID:
                    column = "LastHWID"; value = acc.LastHWID; break;
                case Column.CreationDate:
                    column = "creationdate"; value = acc.CreationDate; break;
                case Column.LastLoginDate:
                    column = "lastlogindate"; value = acc.LastLoginDate; break;
                case Column.HasEnabledTwoStepByEmail:
                    column = "enabled2FAbyemail"; value = acc.HasEnabledTwoStepByEmail; break;
                case Column.TwoFactorGASharedKey:
                    column = "twofactorsharedkey"; value = acc.TwoFactorGASharedKey; break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(c), c, null);
            }
        }
    }
}