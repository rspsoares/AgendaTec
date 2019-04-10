using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using AgendaTech.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgendaTech.Business.Bindings
{
    public class CustomerFacade : ICustomerFacade
    {
        private readonly ICommonRepository<TCGCustomers> _commonRepository;

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
                    customers = _commonRepository.Filter(x => x.SocialName.Contains(customerName));
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;               
            }

            return customers
                .Select(x => new TCGCustomers()
                {
                    IDCustomer = x.IDCustomer,
                    SocialName = x.SocialName,
                    HireDate = x.HireDate,
                    Active = x.Active
                })
                .OrderBy(x => x.SocialName)
                .ToList();
        }

        public List<TCGCustomers> GetSocialNameCombo(out string errorMessage)
        {
            var customers = new List<TCGCustomers>();

            errorMessage = string.Empty;

            try
            {
                customers = _commonRepository
                    .GetAll()
                    .Select(x => new TCGCustomers()
                    {
                        IDCustomer = x.IDCustomer,
                        SocialName = x.SocialName
                    })
                    .OrderBy(x => x.SocialName)
                    .ToList();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return customers;
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
                    SocialName = result.SocialName,
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
            }
        }
    }
}
