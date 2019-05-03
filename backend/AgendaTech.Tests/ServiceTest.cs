using System.Linq;
using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using Bogus;
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
            var fakeService = new Faker<TCGServices>()
                .RuleFor(t => t.IDCustomer, f => 1)
                .RuleFor(t => t.Description, f => f.Commerce.ProductName().ToString())
                .RuleFor(t => t.Price, f => f.Random.Decimal(0, 1000))
                .RuleFor(t => t.Time, f => f.Random.Int(0, 60));          

            var idService = _serviceRepository.Insert(fakeService, out string errorMessage).IDService;

            Assert.IsTrue(idService > 0);
        }

        [TestMethod]
        public void Service_GetGrid()
        {
            var services = _serviceRepository.GetGrid(0, string.Empty, out string errorMessage);
            Assert.IsTrue(services.Any());
        }

        [TestMethod]
        public void Service_GetServiceNameCombo()
        {
            var services = _serviceRepository.GetServiceNameCombo(1, out string errorMessage);
            Assert.IsTrue(services.Any());
        }

        [TestMethod]
        public void Service_GetServiceById()
        {
            var service = _serviceRepository.GetServiceById(1, out string errorMessage);
            Assert.IsTrue(!service.IDService.Equals(0));
        }

        [TestMethod]
        public void Service_Update()
        {
            var customer = _serviceRepository.GetGrid(0, string.Empty, out string errorMessage).First();
            customer.Description = new Bogus.DataSets.Commerce().ProductName();
            _serviceRepository.Update(customer, out errorMessage);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }       
    }
}
