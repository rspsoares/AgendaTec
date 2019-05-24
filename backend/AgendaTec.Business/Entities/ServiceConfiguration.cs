using System.Security;

namespace AgendaTec.Business.Entities
{
    public class ServiceConfiguration
    {
        public int SendMailInterval { get; set; }
        public string SendMailHost { get; set; }
        public string SendMailLogin { get; set; }
        public SecureString SendMailPassword { get; set; }
        public int SendMailPort { get; set; }
        public int LogDays { get; set; }
    }
}
