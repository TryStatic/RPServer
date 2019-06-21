using System.Net.Mail;
using System.Threading.Tasks;

namespace RPServer.Util
{
    public class EmailSender
    {
        public static string SmtpHost { set; get; }
        public static int SmtpPort { set; get; }
        public static string SmtpUsername { set; get; }
        public static string SmtpPassword { set; get; }

        public static async Task SendMailMessageAsync(string toEmail, string subject, string body)
        {
            using (var mMsg = new MailMessage())
            {
                mMsg.From = new MailAddress(SmtpUsername);
                mMsg.To.Add(new MailAddress(toEmail));
                mMsg.Subject = subject;
                mMsg.Body = body;
                mMsg.IsBodyHtml = false;
                mMsg.Priority = MailPriority.Normal;

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Host = SmtpHost;
                    smtpClient.Port = SmtpPort;
                    smtpClient.Credentials = new System.Net.NetworkCredential(SmtpUsername, SmtpPassword);
                    smtpClient.EnableSsl = true;

                    //send the mail message
                    await smtpClient.SendMailAsync(mMsg);
                }
            }

        }
    }
}