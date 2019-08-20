using NLog;

namespace AgendaTec.Business.Entities
{
    public class ServiceConfiguration
    {
        public ServiceConfiguration()
        {
            MailConfigurationService = new MailConfiguration();            
            LoggerControl = new LoggerConfiguration();
        }

        public MailConfiguration MailConfigurationService { get; set; }        
        public LoggerConfiguration LoggerControl { get; set; }
        public int LogDays { get; set; }
    }

    public class LoggerConfiguration
    {
        public Logger ServiceInfo { get; set; }
        public Logger ServiceError { get; set; }
        public Logger MailServiceInfo { get; set; }
        public Logger MailServiceError { get; set; }          
    }
}
