using System.Linq;
using AgendaTec.Business.Bindings;
using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgendaTec.Tests
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
            ProfilesHelper.Initialize();

            var fakeService = new Faker<ServiceDTO>()
                .RuleFor(t => t.IdCustomer, f => 1)
                .RuleFor(t => t.Description, f => f.Commerce.ProductName().ToString())
                .RuleFor(t => t.Price, f => f.Random.Decimal(0, 1000))
                .RuleFor(t => t.Time, f => f.Random.Int(0, 60));          

            var idService = _serviceRepository.Insert(fakeService, out string errorMessage).Id;

            ProfilesHelper.Reset();

            Assert.IsTrue(idService > 0);
        }

        [TestMethod]
        public void Service_GetGrid()
        {
            ProfilesHelper.Initialize();
            var services = _serviceRepository.GetGrid(0, string.Empty, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(services.Any());
        }

        [TestMethod]
        public void Service_GetServiceNameCombo()
        {
            ProfilesHelper.Initialize();
            var services = _serviceRepository.GetServiceNameCombo(1, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(services.Any());
        }

        [TestMethod]
        public void Service_GetServiceNameComboClient()
        {
            ProfilesHelper.Initialize();
            var services = _serviceRepository.GetServiceNameComboClient(1, true, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(services.Any());
        }

        [TestMethod]
        public void Service_GetServiceById()
        {
            ProfilesHelper.Initialize();
            var service = _serviceRepository.GetServiceById(5, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(!service.Id.Equals(0));
        }

        [TestMethod]
        public void Service_Update()
        {
            ProfilesHelper.Initialize();
            var customer = _serviceRepository.GetGrid(0, string.Empty, out string errorMessage).First();
            customer.Description = new Bogus.DataSets.Commerce().ProductName();
            _serviceRepository.Update(customer, out errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }       
    }
}
