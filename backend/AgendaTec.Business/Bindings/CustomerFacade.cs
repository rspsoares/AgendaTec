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
using Itenso.TimePeriod;
using AgendaTec.Business.Helpers;

namespace AgendaTec.Business.Bindings
{
    public class CustomerFacade : ICustomerFacade
    {
        private readonly ICommonRepository<TCGCustomers> _commonRepository;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public CustomerFacade()
        {
            _commonRepository = new CommonRepository<TCGCustomers>();
        }

        public List<CustomerDTO> GetGrid(string customerName, out string errorMessage)
        {
            var result = new List<CustomerDTO>();
            var customers = new List<TCGCustomers>();

            errorMessage = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(customerName))
                    customers = _commonRepository.GetAll();
                else
                    customers = _commonRepository.Filter(x => x.CompanyName.Contains(customerName));

                result = Mapper.Map<List<TCGCustomers>, List<CustomerDTO>>(customers);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result.OrderBy(x => x.Name).ToList();
        }

        public List<CustomerDTO> GetCompanyNameCombo(int idCustomer, out string errorMessage)
        {
            var customers = new List<TCGCustomers>();
            var result = new List<CustomerDTO>();
            errorMessage = string.Empty;

            try
            {
                customers = _commonRepository.GetAll();

                if (idCustomer > 0)
                    customers = customers.Where(x => x.IDCustomer.Equals(idCustomer)).ToList();

                result = Mapper.Map<List<TCGCustomers>, List<CustomerDTO>>(customers);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return result.OrderBy(x => x.Name).ToList();
        }

        public CustomerDTO GetCustomerById(int idCustomer, out string errorMessage)
        {
            var customer = new CustomerDTO();

            errorMessage = string.Empty;

            try
            {
                var result = _commonRepository.GetById(idCustomer);
                customer = Mapper.Map<TCGCustomers, CustomerDTO>(result);              
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return customer;
        }

        public bool CheckCPFRequired(int idCustomer, string cpf)
        {
            var customer = GetCustomerById(idCustomer, out string errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
                return false;

            if (string.IsNullOrEmpty(cpf))
                return !customer.CPFRequired;
            else
                return cpf.IsCPF();
        }

        public CustomerDTO GetCustomerByKey(string customerKey, out string errorMessage)
        {
            var customer = new CustomerDTO();

            errorMessage = string.Empty;

            try
            {
                var result = _commonRepository
                    .Filter(x => x.CustomerKey.ToString().ToUpper().Equals(customerKey.ToUpper()))
                    .FirstOrDefault();

                customer = Mapper.Map<TCGCustomers, CustomerDTO>(result);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return customer;
        }

        public CustomerDTO Insert(CustomerDTO e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var result = Mapper.Map<CustomerDTO, TCGCustomers>(e);
                result = _commonRepository.Insert(result);
                e.Id = result.IDCustomer;
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return e;
        }

        public void Update(CustomerDTO e, out string errorMessage)
        {
            IUserFacade userFacade = new UserFacade();
            ICommonRepository<TCGCustomerTimeRanges> _timeRangeRepository = new CommonRepository<TCGCustomerTimeRanges>();

            try
            {
                var result = Mapper.Map<CustomerDTO, TCGCustomers>(e);

                _commonRepository.Update(result.IDCustomer, result);
                userFacade.UpdateAdminUsersByCustomer(result.IDCustomer, out errorMessage);

                _timeRangeRepository
                    .Filter(x => x.IDCustomer.Equals(result.IDCustomer))
                    .ForEach(timeRange =>
                    {
                        _timeRangeRepository.Delete(timeRange.IDCustomerTimeRanges);
                    });

                result
                    .TCGCustomerTimeRanges
                    .ToList()
                    .ForEach(timeRange =>
                    {
                        _timeRangeRepository.Insert(timeRange);
                    });
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }
        }      

        public bool CheckValidTimeRanges(List<CustomerTimeRangeDTO> timeRanges, out string errorMessage)
        {
            var timePeriods = new TimePeriodCollection();

            errorMessage = string.Empty;

            try
            {                
                timeRanges.ForEach(timeRange => { timePeriods.Add(new TimeRange(timeRange.Start, timeRange.End, false)); });
                return !timePeriods.HasOverlaps();                
            }
            catch (Exception ex)
            {
                errorMessage ="Ocorreu um erro ao validar os intervalios de atendimento";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");

                return false;
            }          
        }
    }
}
