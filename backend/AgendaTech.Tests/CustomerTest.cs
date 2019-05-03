using System.Linq;
using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bogus.Extensions.Brazil;
using System.Text.RegularExpressions;
using System;

namespace AgendaTech.Tests
{
    [TestClass]
    public class CustomerTest
    {
        private readonly ICustomerFacade _customerRepository;

        public CustomerTest()
        {
            _customerRepository = new CustomerFacade();
        }

        //GetCustomerById
        //Update

        [TestMethod]
        public void Customer_Insert()
        {
            var fakeService = new Faker<TCGCustomers>()
                .RuleFor(t => t.CompanyName, f => f.Company.CompanyName())
                .RuleFor(t => t.CNPJ, f => Regex.Replace(f.Company.Cnpj(), @"[^\d]", ""))
                .RuleFor(t => t.Address, f => f.Address.StreetAddress())
                .RuleFor(t => t.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(t => t.HireDate, f => f.Date.Recent())
                .RuleFor(t => t.Active, f => f.Random.Bool())
                .RuleFor(t => t.Note, f => f.Lorem.Sentence(5));

            var idCustomer = _customerRepository.Insert(fakeService, out string errorMessage).IDCustomer;

            Assert.IsTrue(idCustomer > 0);
        }

        [TestMethod]
        public void Customer_GetGrid()
        {
            var customers = _customerRepository.GetGrid(string.Empty, out string errorMessage);
            Assert.IsTrue(customers.Any());
        }

        [TestMethod]
        public void Customer_GetCompanyNameCombo()
        {
            var customers = _customerRepository.GetCompanyNameCombo(1, out string errorMessage);
            Assert.IsTrue(customers.Any());
        }

        [TestMethod]
        public void Customer_GetCustomerById()
        {
            var customer = _customerRepository.GetCustomerById(1, out string errorMessage);
            Assert.IsTrue(!customer.IDCustomer.Equals(0));
        }

        [TestMethod]
        public void Customer_Update()
        {
            var customer = _customerRepository.GetCustomerById(1, out string errorMessage);
            customer.HireDate = DateTime.Now;
            _customerRepository.Update(customer, out errorMessage);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }
    }
}
