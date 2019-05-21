using NLog;
using System.Security;

namespace AgendaTec.Service
{
    public class ServiceConfiguration
    {
        public int SendMailInterval { get; set; }
        public string SendMailHost { get; set; }
        public string SendMailUserName { get; set; }
        public SecureString SendMailPassword { get; set; }
        public int SendMailPort { get; set; }
        public int LogDays { get; set; }
        public LoggerControl LoggerControl { get; set; }
    }

    public class LoggerControl
    {
        public Logger ServiceInfo { get; set; }
        public Logger ServiceError { get; set; }
        public Logger SendMailInfo { get; set; }
        public Logger SendMailError { get; set; }        
        public Logger CleanUpInfo { get; set; }
        public Logger CleanUpError { get; set; }
    }
}
