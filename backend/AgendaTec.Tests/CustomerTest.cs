using System.Linq;
using AgendaTec.Business.Bindings;
using AgendaTec.Business.Contracts;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bogus.Extensions.Brazil;
using System.Text.RegularExpressions;
using System;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;

namespace AgendaTec.Tests
{
    [TestClass]
    public class CustomerTest
    {
        private readonly ICustomerFacade _customerRepository;

        public CustomerTest()
        {
            _customerRepository = new CustomerFacade();            
        }        

        [TestMethod]
        public void Customer_Insert()
        {
            ProfilesHelper.Initialize();

            var fakeCustomer = new Faker<CustomerDTO>()
                .RuleFor(t => t.Key, f => Guid.NewGuid().ToString())
                .RuleFor(t => t.Name, f => f.Company.CompanyName())
                .RuleFor(t => t.CNPJ, f => Regex.Replace(f.Company.Cnpj(), @"[^\d]", ""))
                .RuleFor(t => t.Address, f => f.Address.StreetAddress())
                .RuleFor(t => t.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(t => t.Hire, f => f.Date.Recent())
                .RuleFor(t => t.Start, f => f.Date.Recent())
                .RuleFor(t => t.End, f => f.Date.Recent())
                .RuleFor(t => t.Active, f => f.Random.Bool())
                .RuleFor(t => t.Note, f => f.Lorem.Sentence(5));

            var idCustomer = _customerRepository.Insert(fakeCustomer, out string errorMessage).Id;

            ProfilesHelper.Reset();

            Assert.IsTrue(idCustomer > 0);            
        }

        [TestMethod]
        public void Customer_GetGrid()
        {
            ProfilesHelper.Initialize();
            var customers = _customerRepository.GetGrid(string.Empty, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(customers.Any());
        }

        [TestMethod]
        public void Customer_GetCompanyNameCombo()
        {
            ProfilesHelper.Initialize();
            var customers = _customerRepository.GetCompanyNameCombo(1, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(customers.Any());
        }

        [TestMethod]
        public void Customer_GetCustomerById()
        {
            ProfilesHelper.Initialize();
            var customer = _customerRepository.GetCustomerById(1, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(!customer.Id.Equals(0));
        }

        [TestMethod]
        public void Customer_Update()
        {
            ProfilesHelper.Initialize();
            var customer = _customerRepository.GetCustomerById(1, out string errorMessage);
            customer.Hire = DateTime.Now;
            _customerRepository.Update(customer, out errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }
    }
}
