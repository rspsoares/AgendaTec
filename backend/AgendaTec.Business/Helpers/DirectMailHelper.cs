using AgendaTec.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace AgendaTec.Business.Helpers
{
    public class DirectMailHelper
    {
        public void SendMail(DirectMailDTO directMail, List<UserAccountDTO> users)
        {
            var mailHelper = new SendMailHelper();
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

            mailHelper.SendMail(mailMessage);
        }

        //public static void SendWhatsApp()
        //{
        //    const string accountSid = "ACa92047e3a6ae5077af119e032ef65536";
        //    const string authToken = "173a0316a3c545503475206b821c480e";

        //    TwilioClient.Init(accountSid, authToken);

        //    try
        //    {
        //        var message = MessageResource.Create(
        //            from: new PhoneNumber("whatsapp:+14155238886"),
        //            body: "Promoção imperdível dia dos Namorados: nononononononononono",
        //            to: new PhoneNumber("whatsapp:+5511998056533")
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
