using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
using FluentValidation;
using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace AgendaTec.Service
{
    public static class ServiceHelper
    {
        public static ServiceConfiguration LoadConfigurations(out string errorMessage)
        {
            var serviceConfiguration = new ServiceConfiguration()
            {
                MailConfigurationService = new MailConfiguration()
                {
                    SendMailInterval = int.Parse(ConfigurationManager.AppSettings["SendMailInterval"] ?? "60"),
                    SendMailHost = ConfigurationManager.AppSettings["SendMailHost"],
                    SendMailLogin = ConfigurationManager.AppSettings["SendMailLogin"],
                    SendMailPassword = SecurityHelper.Decrypt(Convert.FromBase64String(ConfigurationManager.AppSettings["SendMailPassword"])),
                    SendMailPort = int.Parse(ConfigurationManager.AppSettings["SendMailPort"] ?? "587"),
                },               
                LoggerControl = new LoggerConfiguration()
                {
                    ServiceInfo = LogManager.GetLogger("ServiceInfoLogger"),
                    ServiceError = LogManager.GetLogger("ServiceErrorLogger"),
                    MailServiceInfo = LogManager.GetLogger("EmailInfoLogger"),
                    MailServiceError = LogManager.GetLogger("EmailErrorLogger")                   
                },                
                LogDays = int.Parse(ConfigurationManager.AppSettings["LogDays"] ?? "30")                
            };

            //var results = new ServiceConfigurationValidator().Validate(serviceConfiguration);

            //errorMessage = string.Join(Environment.NewLine, results.Errors.Select(x => x.ErrorMessage).ToArray());
            errorMessage = string.Empty;

            return serviceConfiguration;
        }

        //private class ServiceConfigurationValidator : AbstractValidator<ServiceConfiguration>
        //{
        //    public ServiceConfigurationValidator()
        //    {
        //        RuleFor(config => config.SendMailInterval).NotNull().NotEqual(0).WithMessage("Intervalo de Verificação não definido.");

        //        RuleFor(config => config.SendMailHost).NotNull().NotEmpty().WithMessage("Send Mail Host não definido.");
        //        RuleFor(config => config.SendMailLogin).NotNull().NotEmpty().WithMessage("Send Mail Login não definido.");
        //        RuleFor(config => config.SendMailPassword).NotNull().NotEmpty().WithMessage("Send Mail Password não definido.");
        //        RuleFor(config => config.SendMailPort).NotNull().NotEqual(0).WithMessage("Send Mail Port não definido.");

        //        RuleFor(config => config.LogDays).NotNull().NotEqual(0).WithMessage("Log Days não definido.");             
        //    }
        //}

        public static void DeleteOldLocalLogs(int daysLimit)
        {
            GetLogFolders().ForEach(logFolder =>
            {
                if (Directory.Exists(logFolder))
                {
                    var oldLogs = Directory
                        .GetFiles(logFolder)
                        .Where(x => new FileInfo(x).LastWriteTime.Date < DateTime.Today.AddDays(-daysLimit).Date)
                        .ToList();

                    oldLogs.ForEach(x => { DeleteFile(x); });
                }
            });
        }

        private static List<string> GetLogFolders()
        {
            var logFolders = new List<string>();
            var logEventInfo = new LogEventInfo
            {
                TimeStamp = DateTime.Now
            };

            LogManager.Configuration.LoggingRules.ToList().ForEach(logRule =>
            {
                logRule.Targets.ToList().ForEach(targetLog =>
                {
                    var logTarget = (FileTarget)targetLog;
                    logFolders.Add(Path.GetDirectoryName(logTarget.FileName.Render(logEventInfo)));
                });
            });

            return logFolders;
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
