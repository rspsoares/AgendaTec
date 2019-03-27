using System;
using System.Linq;
using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgendaTech.Tests
{
    [TestClass]
    public class ClientTest
    {
        private readonly ICustomerFacade _customerRepository;

        public ClientTest()
        {
            _customerRepository = new CustomerFacade();
        }

        [TestMethod]
        public void Client_Insert()
        {
            var cliente = new TCGCustomers()
            {
                RazaoSocial = "Razao",
                CNPJ_CPF = "55555555555",
                Endereco = "Rua Blas",
                Telefone = "Telefone",
                DataContratacao = DateTime.Now,
                Ativo = true,
                Observacao = "Observacao"
            };

            var idCustomer = _customerRepository.Insert(cliente, out string errorMessage).IDCustomer;

            Assert.IsTrue(idCustomer > 0);
        }

        [TestMethod]
        public void Client_GetAll()
        {
            var customers = _customerRepository.GetGrid(string.Empty, out string errorMessage);
            Assert.IsTrue(customers.Any());
        }

        [TestMethod]
        public void Client_Update()
        {
            var customer = _customerRepository.GetGrid(string.Empty, out string errorMessage).First();
            customer.RazaoSocial = "Updated";
            _customerRepository.Update(customer, out errorMessage);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }
    }
}
