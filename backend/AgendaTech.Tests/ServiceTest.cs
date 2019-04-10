using System.Linq;
using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgendaTech.Tests
{
    [TestClass]
    public class ServiceTest
    {
        private readonly IServiceFacade _serviceRepository;

        public ServiceTest()
        {
            _serviceRepository = new ServiceFacade();
        }

        [TestMethod]
        public void Service_Insert()
        {
            var service = new TCGServices()
            {
                IDCustomer = 1,
                Description = "Descrição",
                Value = decimal.Parse("123,34"),
                Time = 30
            };

            var idService = _serviceRepository.Insert(service, out string errorMessage).IDService;

            Assert.IsTrue(idService > 0);
        }

        [TestMethod]
        public void Service_GetAll()
        {
            var services = _serviceRepository.GetGrid(0, string.Empty, out string errorMessage);
            Assert.IsTrue(services.Any());
        }

        [TestMethod]
        public void Service_Update()
        {
            var customer = _serviceRepository.GetGrid(0, string.Empty, out string errorMessage).First();
            customer.Description = "Updated";
            _serviceRepository.Update(customer, out errorMessage);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }       
    }
}
