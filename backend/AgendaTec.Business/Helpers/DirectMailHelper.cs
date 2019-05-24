using AgendaTec.Business.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AgendaTec.Business.Helpers
{
    public static class DirectMailHelper
    {
        public static void SendMail(ServiceConfiguration configuration, DirectMailDTO directMail, List<UserAccountDTO> users)
        {
            var encoding = Encoding.GetEncoding("ISO-8859-1");
            var adminSender = users.Where(x => ((EnUserType)int.Parse(x.IdRole)).Equals(EnUserType.Administrator)).First();

            var adminCCs = users
                .Where(x => ((EnUserType)int.Parse(x.IdRole)).Equals(EnUserType.Administrator))                
                .ToList();

            var recipients = users
                .Where(x => ((EnUserType)int.Parse(x.IdRole)).Equals(EnUserType.Consumer))
                .Select(x => x.Email)
                .ToList();

            var mailMessage = new MailMessage()
            {
                From = new MailAddress(adminSender.Email, adminSender.FullName, Encoding.UTF8),
                Subject = directMail.Description,
                Body = directMail.Content,
                IsBodyHtml = true,
                Priority = MailPriority.High,
                SubjectEncoding = encoding,
                BodyEncoding = encoding
            };

            adminCCs.ForEach(adminsCc =>
            {
                mailMessage.To.Add(new MailAddress(adminsCc.Email, adminsCc.FullName));
            });

            recipients.ForEach(recipient =>
            {
                mailMessage.Bcc.Add(recipient);
            });

            var networkCredential = new NetworkCredential
            {
                UserName = configuration.SendMailLogin,
                Password = configuration.SendMailPassword.ToUnsecureString()
            };

            var smtp = new SmtpClient
            {
                Host = configuration.SendMailHost,
                EnableSsl = true,
                UseDefaultCredentials = true,
                Credentials = networkCredential,
                Port = configuration.SendMailPort
            };

            smtp.Send(mailMessage);
        }
    }
}
