using NLog;

namespace AgendaTec.Business.Entities
{
    public class ServiceConfiguration
    {
        public ServiceConfiguration()
        {
            MailConfigurationService = new MailConfiguration();
            ImportUserConfigurationService = new ImportUserConfiguration();
            LoggerControl = new LoggerConfiguration();
        }

        public MailConfiguration MailConfigurationService { get; set; }
        public ImportUserConfiguration ImportUserConfigurationService { get; set; }
        public LoggerConfiguration LoggerControl { get; set; }
        public int LogDays { get; set; }
    }

    public class ImportUserConfiguration
    {
        public int Interval { get; set; }
        public string OriginFolder { get; set; }
        public string DestinationFolder { get; set; }
    }

    public class LoggerConfiguration
    {
        public Logger ServiceInfo { get; set; }
        public Logger ServiceError { get; set; }
        public Logger MailServiceInfo { get; set; }
        public Logger MailServiceError { get; set; }
        public Logger ImportUserInfo { get; set; }
        public Logger ImportUserError { get; set; }       
    }
}
