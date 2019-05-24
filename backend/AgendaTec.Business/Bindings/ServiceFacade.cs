using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Infrastructure.Contracts;
using AgendaTec.Infrastructure.DatabaseModel;
using AgendaTec.Infrastructure.Repositories;
using AutoMapper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AgendaTec.Business.Bindings
{
    public class ServiceFacade : IServiceFacade
    {
        private readonly ICommonRepository<TCGServices> _commonRepository;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public ServiceFacade()
        {
            _commonRepository = new CommonRepository<TCGServices>();
        }

        public List<ServiceDTO> GetGrid(int idCustomer, string serviceName, out string errorMessage)
        {
            var result = new List<ServiceDTO>();
            var services = new List<TCGServices>();
            
            errorMessage = string.Empty;

            try
            {
                services = _commonRepository.GetAll();

                if (idCustomer > 0)
                    services = services.Where(x => x.IDCustomer.Equals(idCustomer)).ToList();

                if (!string.IsNullOrEmpty(serviceName))
                    services = services.Where(x => x.Description.ToUpper().Contains(serviceName.ToUpper())).ToList();

                result = Mapper.Map<List<TCGServices>, List<ServiceDTO>>(services);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result
                .OrderBy(x => x.IdCustomer)
                .ThenBy(x => x.Description)
                .ToList();
        }

        public List<ServiceDTO> GetServiceNameCombo(int idCustomer, out string errorMessage)
        {
            var result = new List<ServiceDTO>();
            var services = new List<TCGServices>();

            errorMessage = string.Empty;

            try
            {
                services = _commonRepository.Filter(x => x.IDCustomer.Equals(idCustomer));
                result = Mapper.Map<List<TCGServices>, List<ServiceDTO>>(services);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result
                .OrderBy(x => x.Description)
                .ToList();
        }

        public List<ServiceDTO> GetServiceNameComboClient(int idCustomer, bool authenticated, out string errorMessage)
        {
            var result = new List<ServiceDTO>();
            var services = new List<TCGServices>();

            errorMessage = string.Empty;

            try
            {
                if (authenticated)
                    services = _commonRepository.Filter(x => x.IDCustomer.Equals(idCustomer));                
                else
                {
                    services.Add(new TCGServices()
                    {
                        IDService = 0,
                        Description = "Para visualizar as opções de serviços, favor efetuar o Login."
                    });
                }

                result = Mapper.Map<List<TCGServices>, List<ServiceDTO>>(services);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result
                .OrderBy(x => x.Description)
                .ToList();
        }

        public ServiceDTO GetServiceById(int idService, out string errorMessage)
        {
            var result = new ServiceDTO();
            var service = new TCGServices();

            errorMessage = string.Empty;

            try
            {
                service = _commonRepository.GetById(idService);
                result = Mapper.Map<TCGServices, ServiceDTO>(service);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result;
        }

        public ServiceDTO Insert(ServiceDTO e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var result = Mapper.Map<ServiceDTO, TCGServices>(e);
                result = _commonRepository.Insert(result);
                e.Id = result.IDService;
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return e;
        }

        public void Update(ServiceDTO e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var result = Mapper.Map<ServiceDTO, TCGServices>(e);
                _commonRepository.Update(result.IDService, result);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }
        }
    }
}
