using AgendaTec.Business.Entities;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace AgendaTec.Business.Helpers
{
    public class SendMailHelper
    {
        public void SendMail(MailMessage mailMessage)
        {
            var mailConfiguration = new MailConfiguration()
            {
                SendMailInterval = int.Parse(ConfigurationManager.AppSettings["SendMailInterval"] ?? "60"),
                SendMailHost = ConfigurationManager.AppSettings["SendMailHost"],
                SendMailLogin = ConfigurationManager.AppSettings["SendMailLogin"],
                SendMailPassword = SecurityHelper.Decrypt(Convert.FromBase64String(ConfigurationManager.AppSettings["SendMailPassword"])),
                SendMailPort = int.Parse(ConfigurationManager.AppSettings["SendMailPort"] ?? "587")
            };
            var smtp = new SmtpClient
            {
                Host = mailConfiguration.SendMailHost,
                EnableSsl = true,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential()
                {
                    UserName = mailConfiguration.SendMailLogin,
                    Password = mailConfiguration.SendMailPassword.ToUnsecureString()
                },
                Port = mailConfiguration.SendMailPort
            };

            smtp.Send(mailMessage);
        }
    }
}
