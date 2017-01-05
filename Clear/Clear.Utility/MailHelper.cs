using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Clear.Utility
{
    public class MailHelper
    {
        private static readonly string SmtpHost = ConfigurationManager.AppSettings["SmtpHost"];
        private static readonly string SysMailAddress = ConfigurationManager.AppSettings["SysMailAddress"];
        private static readonly string[] Bcc = ConfigurationManager.AppSettings["Bcc"].Split(',');
        public static void SendMail(string[] to, string[] cc, string subject, string body)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(SysMailAddress, "TestProject", Encoding.UTF8);
            foreach (var item in to)
            {
                mailMessage.To.Add(item);
            }
            foreach (var item in cc)
            {
                mailMessage.CC.Add(item);
            }
            foreach (var item in Bcc)
            {
                mailMessage.Bcc.Add(item);
            }

            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;
            mailMessage.BodyEncoding = Encoding.UTF8;
            SmtpClient smtpClient = new SmtpClient(SmtpHost);
            smtpClient.Timeout = 200000;
            smtpClient.Credentials = new NetworkCredential();
            //smtpClient.SendAsync(mailMessage,"DisposalSystem");
            smtpClient.Send(mailMessage);
        }

    }
}
