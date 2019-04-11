using AgendaTech.Business.Contracts;
using AgendaTech.Business.Helpers;
using AgendaTech.Infrastructure.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using AgendaTech.Infrastructure.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AgendaTech.Business.Bindings
{
    public class ServiceFacade : IServiceFacade
    {
        private readonly ICommonRepository<TCGServices> _commonRepository;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public ServiceFacade()
        {
            _commonRepository = new CommonRepository<TCGServices>();
        }

        public List<TCGServices> GetGrid(int idCustomer, string serviceName, out string errorMessage)
        {
            var services = new List<TCGServices>();
            
            errorMessage = string.Empty;

            try
            {
                services = _commonRepository.GetAll();

                if (idCustomer > 0)
                    services = services.Where(x => x.IDCustomer.Equals(idCustomer)).ToList();

                if (!string.IsNullOrEmpty(serviceName))
                    services = services.Where(x => x.Description.Contains(serviceName)).ToList();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return services
                .Select(x => new TCGServices()
                {
                   IDService = x.IDService,
                   IDCustomer = x.IDCustomer,
                   Description = x.Description,
                   Value = x.Value,
                   Time = x.Time
                })         
                .OrderBy(x => x.IDCustomer)
                .ThenBy(x => x.Description)
                .ToList();
        }

        public TCGServices GetServiceById(int idService, out string errorMessage)
        {
            var service = new TCGServices();

            errorMessage = string.Empty;

            try
            {
                var result = _commonRepository.GetById(idService);

                service = new TCGServices()
                {
                    IDService = result.IDService,
                    IDCustomer = result.IDCustomer,
                    Description = result.Description,
                    Value = result.Value,
                    Time = result.Time
                };
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return service;
        }

        public TCGServices Insert(TCGServices e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                e = _commonRepository.Insert(e);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return e;
        }

        public void Update(TCGServices e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                _commonRepository.Update(e);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }
        }
    }
}
