using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using RPServer.Game;
using RPServer.Models;

namespace RPServer.Email
{
    internal class EmailSender
    {
        public static string SmtpHost { set; get; }
        public static int SmtpPort { set; get; }
        public static string SmtpUsername { set; get; }
        public static string SmtpPassword { set; get; }

        internal static async Task SendEmailVerificationCode(EmailToken token)
        {
            var message =
                $"<h2>Hello {token.Account.Username}!</h2><p>Here you have your security code to authenticate the ownership of your account:</p>" +
                $"<h3>{token.Token}</h3>" +
                "<p>As a security measure, the code will only be valid for 24 hours.<br /><br /></p>" +
                "<p>&nbsp;</p>" +
                "<p><strong>If this wasn&rsquo;t you:&nbsp;</strong>Change your password, and consider changing your email password as well to ensure your account security.</p><p>&nbsp;</p>" +
                $"<p>Thanks, {Globals.SERVER_NAME}</p>";

            await SendMailMessageAsync(token.EmailAddress, $"{Globals.SERVER_NAME} - Email Verification", message);
        }

        private static async Task SendMailMessageAsync(string toEmail, string subject, string body)
        {
            using (var mMsg = new MailMessage())
            {
                mMsg.From = new MailAddress(SmtpUsername);
                mMsg.To.Add(new MailAddress(toEmail));
                mMsg.Subject = subject;
                mMsg.Body = body;
                mMsg.IsBodyHtml = true;
                mMsg.Priority = MailPriority.Normal;

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Host = SmtpHost;
                    smtpClient.Port = SmtpPort;
                    smtpClient.Credentials = new NetworkCredential(SmtpUsername, SmtpPassword);
                    smtpClient.EnableSsl = true;

                    //send the mail message
                    await smtpClient.SendMailAsync(mMsg);
                }
            }
        }
    }
}