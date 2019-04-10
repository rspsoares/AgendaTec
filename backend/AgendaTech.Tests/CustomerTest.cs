using System;
using System.Linq;
using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var cliente = new TCGCustomers()
            {
                SocialName = "Razao",
                CNPJ = "55555555555",
                Address = "Rua Blas",
                Phone = "Telefone",
                HireDate= DateTime.Now,
                Active = true,
                Note = "Observacao"
            };

            var idCustomer = _customerRepository.Insert(cliente, out string errorMessage).IDCustomer;

            Assert.IsTrue(idCustomer > 0);
        }

        [TestMethod]
        public void Customer_GetAll()
        {
            var customers = _customerRepository.GetGrid(string.Empty, out string errorMessage);
            Assert.IsTrue(customers.Any());
        }

        [TestMethod]
        public void Customer_Update()
        {
            var customer = _customerRepository.GetGrid(string.Empty, out string errorMessage).First();
            customer.SocialName = "Updated";
            _customerRepository.Update(customer, out errorMessage);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }
    }
}
