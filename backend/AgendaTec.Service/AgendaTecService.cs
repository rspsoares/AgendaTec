using AgendaTec.Business.Bindings;
using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
using NLog;
using System;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace AgendaTec.Service
{
    public partial class AgendaTecService : ServiceBase
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
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
            _configuration = ServiceHelper.LoadConfigurations(out string errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                throw new Exception(errorMessage);

            _logger.Info($"({MethodBase.GetCurrentMethod().Name}) Starting Service...");

            sendMailTimer = new Timer(new TimerCallback(SendMailCallback), null, (int)TimeSpan.FromSeconds(1).TotalMilliseconds, (int)TimeSpan.FromSeconds(_configuration.SendMailInterval).TotalMilliseconds);
            cleanUpTimer = new Timer(new TimerCallback(CleanUpCallback), null, (int)TimeSpan.FromSeconds(10).TotalMilliseconds, (int)TimeSpan.FromHours(1).TotalMilliseconds);
        }

        protected override void OnStop()
        {
            _logger.Info($"({MethodBase.GetCurrentMethod().Name}) Disposing Service...");

            sendMailTimer.Dispose();
            cleanUpTimer.Dispose();
        }

        private void SendMailCallback(object state)
        {
            if (sendMailLock)
                return;

            sendMailLock = true;

            _logger.Info($"[{MethodBase.GetCurrentMethod().Name}] Starting to send direct mailing...");

            try
            {
                var mailPendings = _directMailFacade.GetPendingDirectMail(out string errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    _logger.Fatal($"[{MethodBase.GetCurrentMethod().Name}] {errorMessage}");
                    sendMailLock = false;
                    return;
                }

                mailPendings.ForEach(mail =>
                {
                    try
                    {
                        _logger.Info($"[{MethodBase.GetCurrentMethod().Name}] Sending Direct Mail: {mail.Description}...");

                        var recipients = _userFacade.GetUserRecipients(mail.IdCustomer, out errorMessage);
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            _logger.Fatal($"[{MethodBase.GetCurrentMethod().Name}] {errorMessage}");
                            return;
                        }

                        switch ((EnMailType)mail.MailType)
                        {
                            case EnMailType.All:
                                DirectMailHelper.SendMail(_configuration, mail, recipients);
                                //TODO: WhatsApp
                                break;
                            case EnMailType.Email:
                                DirectMailHelper.SendMail(_configuration, mail, recipients);
                                break;
                            case EnMailType.WhatsApp:
                                //TODO: WhatsApp
                                break;
                        }

                        mail.Last = DateTime.Now;
                        mail.Resend = false;

                        _directMailFacade.Update(mail, out errorMessage);
                        if (!string.IsNullOrEmpty(errorMessage))                        
                            _logger.Error($"[{MethodBase.GetCurrentMethod().Name}] Error on updating direct mail send: {errorMessage}");

                        _logger.Info($"[{MethodBase.GetCurrentMethod().Name}] Sending Direct Mail: {mail.Description} completed.");
                    }
                    catch (Exception ex)
                    {
                        _logger.Fatal($"[{MethodBase.GetCurrentMethod().Name}] ID: {mail.Id}. Error: {ex.Message} - {ex.InnerException}");
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.Fatal($"[{MethodBase.GetCurrentMethod().Name}] {ex.Message} - {ex.InnerException}");
            }
            finally
            {
                sendMailLock = false;
                _logger.Info($"[{MethodBase.GetCurrentMethod().Name}] Process complete.");
            }
        }

        private void CleanUpCallback(object state)
        {           
            if (cleanUpLock)
                return;

            cleanUpLock = true;

            _logger.Info($"[{MethodBase.GetCurrentMethod().Name}] Starting cleanup...");

            try
            {
                ServiceHelper.DeleteOldLocalLogs(_configuration.LogDays);
            }
            catch (Exception ex)
            {
                _logger.Fatal($"[{MethodBase.GetCurrentMethod().Name}] {ex.Message} - {ex.InnerException}");
            }
            finally
            {
                cleanUpLock = false;
                _logger.Info($"[{MethodBase.GetCurrentMethod().Name}] Process complete.");
            }
        }


        private void WhatsApp()
        {

           
        }
        public void Debug()
        {
            _configuration = ServiceHelper.LoadConfigurations(out string configErrorMessage);

            //CleanUpCallback(null);
            DirectMailHelper.WhatsApp();

            //SendMailCallback(null);

            
        }
    }
}
