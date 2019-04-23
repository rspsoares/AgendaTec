using System;

namespace BrockAllen.MembershipReboot.Entities
{
    public class AuthenticationAudit
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Activity { get; set; }
        public string Detail { get; set; }
        public string ClientIP { get; set; }
    }
}
