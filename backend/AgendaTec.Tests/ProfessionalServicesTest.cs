using AgendaTec.Business.Bindings;
using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace AgendaTec.Tests
{
    [TestClass]
    public class ProfessionalServicesTest
    {
        private readonly IProfessionalServiceFacade _professionalServiceFacade;

        public ProfessionalServicesTest()
        {
            _professionalServiceFacade = new ProfessionalServiceFacade();
        }

        [TestMethod]
        public void ProfessionalService_GetAvailablesProfessionalServices()
        {
            var services = _professionalServiceFacade.GetAvailablesProfessionalServices(1, 0, out string errorMessage);
            Assert.IsTrue(services.Any());
        }

        [TestMethod]
        public void ProfessionalService_Save()
        {
            var services = new List<ProfessionalServiceDTO>()
            {
                new ProfessionalServiceDTO()
                {                    
                    IdService = 5
                }
            };

            _professionalServiceFacade.SaveProfessionalService(3, services, out string errorMessage);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void ProfessionalService_GetServicesComboClient()
        {
            var services = _professionalServiceFacade.GetServicesComboClient(1, true, out string errorMessage);
            Assert.IsTrue(services.Any());
        }

        [TestMethod]
        public void ProfessionalService_GetProfessionalNameComboClient()
        {
            ProfilesHelper.Initialize();
            var professionals = _professionalServiceFacade.GetProfessionalNameComboClient(5, 5, true, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(professionals.Any());
        }
    }
}
