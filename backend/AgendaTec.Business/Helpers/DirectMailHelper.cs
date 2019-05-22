using System.Net.Mail;

namespace AgendaTec.Business.Helpers
{
    public static class DirectMailHelper
    {
        public static void SendMail()
        {
            MailMessage mm = new MailMessage();
            SmtpClient smtp = new SmtpClient();

            mm.From = new MailAddress("From", "DisplayName", System.Text.Encoding.UTF8);
            mm.To.Add(new MailAddress("To"));
            mm.Subject = "Subject";
            mm.Body = "Body";

            mm.IsBodyHtml = true;
            smtp.Host = "smtp.gmail.com";
            //if (ccAdd != "")
            //{
            //    mm.CC.Add(ccAdd);
            //}
            smtp.EnableSsl = true;
            System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            NetworkCred.UserName = "xyz@gmail.com";//gmail user name
            NetworkCred.Password = "Password";// password
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587; //Gmail port for e-mail 465 or 587
            smtp.Send(mm);

        }

    }
}
