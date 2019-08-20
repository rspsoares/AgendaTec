using AgendaTec.Business.Bindings;
using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
using NLog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace AgendaTec.Service
{
    public partial class AgendaTecService : ServiceBase
    {        
        private ServiceConfiguration _configuration = new ServiceConfiguration();
        private readonly IDirectMailFacade _directMailFacade = new DirectMailFacade();
        private readonly IUserFacade _userFacade = new UserFacade();

        public AgendaTecService()
        {
            InitializeComponent();

            ProfilesHelper.Initialize();
        }

        protected override void OnStart(string[] args)
        {
            var loggerInfo = _configuration.LoggerControl.ServiceInfo;
            
            _configuration = ServiceHelper.LoadConfigurations(out string errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                throw new Exception(errorMessage);

            loggerInfo.Info($"({MethodBase.GetCurrentMethod().Name}) Starting Service...");

            sendMailTimer = new Timer(new TimerCallback(SendMailCallback), null, (int)TimeSpan.FromSeconds(1).TotalMilliseconds, (int)TimeSpan.FromSeconds(_configuration.MailConfigurationService.SendMailInterval).TotalMilliseconds);
            cleanUpTimer = new Timer(new TimerCallback(CleanUpCallback), null, (int)TimeSpan.FromSeconds(10).TotalMilliseconds, (int)TimeSpan.FromHours(1).TotalMilliseconds);
        }       

        protected override void OnStop()
        {
            var loggerInfo = _configuration.LoggerControl.ServiceInfo;

            loggerInfo.Info($"({MethodBase.GetCurrentMethod().Name}) Disposing Service...");

            sendMailTimer.Dispose();           
            cleanUpTimer.Dispose();
        }

        private void SendMailCallback(object state)
        {
            var loggerInfo = _configuration.LoggerControl.MailServiceInfo;
            var loggerError = _configuration.LoggerControl.MailServiceError;

            var directMailHelper = new DirectMailHelper();

            if (sendMailLock)
                return;

            sendMailLock = true;

            loggerInfo.Info($"[{MethodBase.GetCurrentMethod().Name}] Starting to send direct mailing...");

            try
            {
                var mailPendings = _directMailFacade.GetPendingDirectMail(out string errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    loggerError.Fatal($"[{MethodBase.GetCurrentMethod().Name}] {errorMessage}");
                    sendMailLock = false;
                    return;
                }

                mailPendings.ForEach(mail =>
                {
                    try
                    {
                        loggerInfo.Info($"[{MethodBase.GetCurrentMethod().Name}] Sending Direct Mail: {mail.Description}...");

                        var recipients = _userFacade.GetUserRecipients(mail.IdCustomer, out errorMessage);
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            loggerError.Fatal($"[{MethodBase.GetCurrentMethod().Name}] {errorMessage}");
                            return;
                        }

                        switch ((EnMailType)mail.MailType)
                        {
                            case EnMailType.All:
                                directMailHelper.SendMail(mail, recipients);                                
                                //TODO: WhatsApp
                                break;
                            case EnMailType.Email:
                                directMailHelper.SendMail(mail, recipients);
                                break;
                            case EnMailType.WhatsApp:
                                //TODO: WhatsApp
                                break;
                        }

                        mail.Last = DateTime.Now;
                        mail.Resend = false;

                        _directMailFacade.Update(mail, out errorMessage);
                        if (!string.IsNullOrEmpty(errorMessage))                        
                            loggerError.Error($"[{MethodBase.GetCurrentMethod().Name}] Error on updating direct mail send: {errorMessage}");

                        loggerInfo.Info($"[{MethodBase.GetCurrentMethod().Name}] Sending Direct Mail: {mail.Description} completed.");
                    }
                    catch (Exception ex)
                    {
                        loggerError.Fatal($"[{MethodBase.GetCurrentMethod().Name}] ID: {mail.Id}. Error: {ex.Message} - {ex.InnerException}");
                    }
                });
            }
            catch (Exception ex)
            {
                loggerError.Fatal($"[{MethodBase.GetCurrentMethod().Name}] {ex.Message} - {ex.InnerException}");
            }
            finally
            {
                sendMailLock = false;
                loggerInfo.Info($"[{MethodBase.GetCurrentMethod().Name}] Process complete.");
            }
        }

        private void CleanUpCallback(object state)
        {
            var loggerInfo = _configuration.LoggerControl.ServiceInfo;
            var loggerError = _configuration.LoggerControl.ServiceError;

            if (cleanUpLock)
                return;

            cleanUpLock = true;

            loggerInfo.Info($"[{MethodBase.GetCurrentMethod().Name}] Starting cleanup...");

            try
            {
                ServiceHelper.DeleteOldLocalLogs(_configuration.LogDays);
            }
            catch (Exception ex)
            {
                loggerError.Fatal($"[{MethodBase.GetCurrentMethod().Name}] {ex.Message} - {ex.InnerException}");
            }
            finally
            {
                cleanUpLock = false;
                loggerInfo.Info($"[{MethodBase.GetCurrentMethod().Name}] Process complete.");
            }
        }

        private void WhatsApp()
        {

           
        }
        public void Debug()
        {
            _configuration = ServiceHelper.LoadConfigurations(out string configErrorMessage);

           // CleanUpCallback(null);
           // DirectMailHelper.SendWhatsApp();

            SendMailCallback(null);          
        }
    }
}
