using AgendaTec.Business.Entities;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AgendaTec.Business.Helpers
{
    public static class DirectMailHelper
    {
        public static void SendMail(DirectMailDTO directMail, List<UserAccountDTO> recipients)
        {
            MailMessage mm = new MailMessage();
            SmtpClient smtp = new SmtpClient();

            mm.From = new MailAddress("From", "DisplayName", Encoding.UTF8);
            mm.To.Add(new MailAddress("To", "DisplayName"));

            mm.Subject = "Subject";
            mm.Body = "Body";

            mm.IsBodyHtml = true;
            smtp.Host = "smtp.gmail.com";

            mm.Priority = MailPriority.High;
            mm.SubjectEncoding = Encoding.GetEncoding("ISO-8859-1");
            mm.BodyEncoding = Encoding.GetEncoding("ISO-8859-1");
            
            //if (ccAdd != "")
            //{
            //    mm.CC.Add(ccAdd);
            //}

            smtp.EnableSsl = true;
            var networkCredential = new NetworkCredential
            {
                UserName = "xyz@gmail.com",//gmail user name
                Password = "Password"// password
            };

            smtp.UseDefaultCredentials = true;
            smtp.Credentials = networkCredential;
            smtp.Port = 587;
            smtp.Send(mm);

        }


    }
}
