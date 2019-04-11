using System.Linq;
using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bogus.Extensions.Brazil;
using System.Text.RegularExpressions;

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
        public void Customer_GetAll()
        {
            var customers = _customerRepository.GetGrid(string.Empty, out string errorMessage);
            Assert.IsTrue(customers.Any());
        }     
    }
}
