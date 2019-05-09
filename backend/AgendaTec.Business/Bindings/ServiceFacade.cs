﻿using AgendaTec.Business.Contracts;
using AgendaTec.Infrastructure.Contracts;
using AgendaTec.Infrastructure.DatabaseModel;
using AgendaTec.Infrastructure.Repositories;
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
                   Price = x.Price,
                   Time = x.Time
                })         
                .OrderBy(x => x.IDCustomer)
                .ThenBy(x => x.Description)
                .ToList();
        }


        public List<TCGServices> GetServiceNameCombo(int idCustomer, out string errorMessage)
        {
            var services = new List<TCGServices>();

            errorMessage = string.Empty;

            try
            {
                services = _commonRepository.Filter(x => x.IDCustomer.Equals(idCustomer));
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
                    Description = x.Description                    
                })
                .OrderBy(x => x.Description)                
                .ToList();
        }

        public List<TCGServices> GetServiceNameComboClient(int idCustomer, bool authenticated, out string errorMessage)
        {
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
                    Description = x.Description,
                    Time = x.Time,
                    Price = x.Price
                })
                .OrderBy(x => x.Description)
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
                    Price = result.Price,
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
                _commonRepository.Update(e.IDService, e);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }
        }
    }
}