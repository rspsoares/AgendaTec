using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using AgendaTech.Infrastructure.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AgendaTech.Business.Bindings
{
    public class CustomerFacade : ICustomerFacade
    {
        private readonly ICommonRepository<TCGCustomers> _commonRepository;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public CustomerFacade()
        {
            _commonRepository = new CommonRepository<TCGCustomers>();
        }

        public List<TCGCustomers> GetGrid(string customerName, out string errorMessage)
        {
            var customers = new List<TCGCustomers>();

            errorMessage = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(customerName))
                    customers = _commonRepository.GetAll();
                else
                    customers = _commonRepository.Filter(x => x.CompanyName.Contains(customerName));
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return customers
                .Select(x => new TCGCustomers()
                {
                    IDCustomer = x.IDCustomer,
                    CompanyName = x.CompanyName,
                    CNPJ = x.CNPJ,
                    Address = x.Address,
                    Phone = x.Phone,
                    HireDate = x.HireDate,
                    Active = x.Active,
                    Note = x.Note
                })
                .OrderBy(x => x.CompanyName)
                .ToList();
        }

        public List<TCGCustomers> GetCompanyNameCombo(int idCustomer, out string errorMessage)
        {
            var customers = new List<TCGCustomers>();

            errorMessage = string.Empty;

            try
            {
                customers = _commonRepository.GetAll();

                if (idCustomer > 0)
                    customers = customers.Where(x => x.IDCustomer.Equals(idCustomer)).ToList();                   
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return customers
                .Select(x => new TCGCustomers()
                {
                    IDCustomer = x.IDCustomer,
                    CompanyName = x.CompanyName
                })
                .OrderBy(x => x.CompanyName)
                .ToList();
        }

        public TCGCustomers GetCustomerById(int idCustomer, out string errorMessage)
        {
            var customer = new TCGCustomers();

            errorMessage = string.Empty;

            try
            {
                var result = _commonRepository.GetById(idCustomer);

                customer = new TCGCustomers()
                {                    
                    IDCustomer = result.IDCustomer,
                    CompanyName = result.CompanyName,
                    CNPJ = result.CNPJ,
                    Address = result.Address,
                    Phone = result.Phone,
                    HireDate = result.HireDate,
                    Active = result.Active,
                    Note = result.Note                    
                };
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return customer;
        }

        public TCGCustomers Insert(TCGCustomers e, out string errorMessage)
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

        public void Update(TCGCustomers e, out string errorMessage)
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
