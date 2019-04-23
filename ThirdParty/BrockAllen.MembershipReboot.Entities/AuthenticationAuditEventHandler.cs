using System;
using System.Web;

namespace BrockAllen.MembershipReboot.Entities
{
    public class AuthenticationAuditEventHandler :
      IEventHandler<SuccessfulLoginEvent<CustomUserAccount>>,
      IEventHandler<FailedLoginEvent<CustomUserAccount>>
    {
        public void Handle(SuccessfulLoginEvent<CustomUserAccount> evt)
        {
            using (var db = new CustomDatabase())
            {
                var audit = new AuthenticationAudit
                {
                    Date = DateTime.UtcNow,
                    Activity = "Autenticação com sucesso!",
                    Detail = null,
                    ClientIP = HttpContext.Current.Request.UserHostAddress,
                };
                db.Audits.Add(audit);
                db.SaveChanges();
            }
        }

        public void Handle(FailedLoginEvent<CustomUserAccount> evt)
        {
            using (var db = new CustomDatabase())
            {
                var audit = new AuthenticationAudit
                {
                    Date = DateTime.UtcNow,
                    Activity = "Falha de Login",
                    Detail = evt.GetType().Name + ", Número de Falhas em Login: " + evt.Account.FailedLoginCount,
                    ClientIP = HttpContext.Current.Request.UserHostAddress,
                };
                db.Audits.Add(audit);
                db.SaveChanges();
            }
        }
    }
}
