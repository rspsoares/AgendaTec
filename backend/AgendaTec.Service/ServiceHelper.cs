using AgendaTec.Business.Helpers;
using FluentValidation;
using NLog;
using System;
using System.Configuration;
using System.Linq;

namespace AgendaTec.Service
{
    public static class ServiceHelper
    {
        public static ServiceConfiguration LoadConfigurations(out string errorMessage)
        {
            var serviceConfiguration = new ServiceConfiguration()
            {
                SendMailInterval = int.Parse(ConfigurationManager.AppSettings["SendMailInterval"] ?? "60"),
                SendMailHost = ConfigurationManager.AppSettings["SendMailHost"],
                SendMailUserName = ConfigurationManager.AppSettings["SendMailUserName"],
                SendMailPassword = SecurityHelper.Decrypt(Convert.FromBase64String(ConfigurationManager.AppSettings["SendMailPassword"])),
                SendMailPort = int.Parse(ConfigurationManager.AppSettings["SendMailPort"] ?? "587"),
                LogDays = int.Parse(ConfigurationManager.AppSettings["LogDays"] ?? "30"),
                LoggerControl = new LoggerControl()
                {
                    ServiceInfo = LogManager.GetLogger("ServiceInfoLogger"),
                    ServiceError = LogManager.GetLogger("ServiceErrorLogger"),
                    SendMailInfo = LogManager.GetLogger("SendMailInfoLogger"),
                    SendMailError = LogManager.GetLogger("SendMailErrorLogger"),
                    CleanUpInfo = LogManager.GetLogger("CleanUpInfoLogger"),
                    CleanUpError = LogManager.GetLogger("CleanUpErrorLogger")
                }
            };

            var results = new ServiceConfigurationValidator().Validate(serviceConfiguration);

            errorMessage = string.Join(Environment.NewLine, results.Errors.Select(x => x.ErrorMessage).ToArray());

            return serviceConfiguration;
        }

        private class ServiceConfigurationValidator : AbstractValidator<ServiceConfiguration>
        {
            public ServiceConfigurationValidator()
            {
                RuleFor(config => config.SendMailInterval).NotNull().NotEqual(0).WithMessage("Intervalo de Verificação não definido.");

                RuleFor(config => config.SendMailHost).NotNull().NotEmpty().WithMessage("Send Mail Host não definido.");
                RuleFor(config => config.SendMailUserName).NotNull().NotEmpty().WithMessage("Send Mail User Name não definido.");
                RuleFor(config => config.SendMailPassword).NotNull().NotEmpty().WithMessage("Send Mail Password não definido.");
                RuleFor(config => config.SendMailPort).NotNull().NotEqual(0).WithMessage("Send Mail Port não definido.");

                RuleFor(config => config.LogDays).NotNull().NotEqual(0).WithMessage("Log Days não definido.");

                RuleFor(config => config.LoggerControl.ServiceInfo).NotNull().WithMessage("Log de Service Info não definido.");
                RuleFor(config => config.LoggerControl.ServiceError).NotNull().WithMessage("Log de Service Error não definido.");
                RuleFor(config => config.LoggerControl.SendMailInfo).NotNull().WithMessage("Log de Send Mail Info não definido.");
                RuleFor(config => config.LoggerControl.SendMailError).NotNull().WithMessage("Log de Send Mail Error não definido.");
                RuleFor(config => config.LoggerControl.CleanUpInfo).NotNull().WithMessage("Log de Limpeza de logs antigos Info não definido.");
                RuleFor(config => config.LoggerControl.CleanUpError).NotNull().WithMessage("Log de Limpeza de logs antigos Error não definido.");
            }
        }
    }
}
