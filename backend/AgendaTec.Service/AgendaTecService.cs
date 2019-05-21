using AgendaTec.Business.Helpers;
using System;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace AgendaTec.Service
{
    public partial class AgendaTecService : ServiceBase
    {
        private ServiceConfiguration _configuration = new ServiceConfiguration();

        public AgendaTecService()
        {
            InitializeComponent();

            ProfilesHelper.Initialize();
        }

        protected override void OnStart(string[] args)
        {
            _configuration = ServiceHelper.LoadConfigurations(out string errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                throw new Exception(errorMessage);

            sendMailTimer = new Timer(new TimerCallback(SendMailCallback), null, (int)TimeSpan.FromSeconds(1).TotalMilliseconds, (int)TimeSpan.FromSeconds(_configuration.SendMailInterval).TotalMilliseconds);
            cleanUpTimer = new Timer(new TimerCallback(CleanUpCallback), null, (int)TimeSpan.FromSeconds(10).TotalMilliseconds, (int)TimeSpan.FromHours(1).TotalMilliseconds);
        }

        protected override void OnStop()
        {
            sendMailTimer.Dispose();
            cleanUpTimer.Dispose();
        }

        private void SendMailCallback(object state)
        {
            var loggerInfo = _configuration.LoggerControl.SendMailInfo;
            var loggerError = _configuration.LoggerControl.SendMailError;

            if (sendMailLock)
                return;

            sendMailLock = true;

            loggerInfo.Info($"[{MethodBase.GetCurrentMethod().Name}] Starting to send direct mailing...");

            try
            {
                

            }
            catch (Exception ex)
            {
                loggerError.Fatal($"[{MethodBase.GetCurrentMethod().Name}] {ex.Message} - {ex.InnerException}");
            }
            finally
            {
                sendMailLock = false;
                loggerInfo.Info($"[{MethodBase.GetCurrentMethod().Name}] Proccess complete.");
            }
        }

        private void CleanUpCallback(object state)
        {
            var loggerInfo = _configuration.LoggerControl.CleanUpInfo;
            var loggerError = _configuration.LoggerControl.CleanUpError;

            if (cleanUpLock)
                return;

            cleanUpLock = true;

            loggerInfo.Info($"[{MethodBase.GetCurrentMethod().Name}] Starting cleanup...");

            try
            {
                //CoreExtensions.DeleteOldLocalLogs(_configuration.LogDays);
            }
            catch (Exception ex)
            {
                loggerError.Fatal($"[{MethodBase.GetCurrentMethod().Name}] {ex.Message} - {ex.InnerException}");
            }
            finally
            {
                cleanUpLock = false;
                loggerInfo.Info($"[{MethodBase.GetCurrentMethod().Name}] Proccess complete.");
            }
        }

        public void Debug()
        {
            _configuration = ServiceHelper.LoadConfigurations(out string configErrorMessage);

            CleanUpCallback(null);
            SendMailCallback(null);
        }
    }
}
