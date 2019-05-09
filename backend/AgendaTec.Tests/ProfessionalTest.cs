using AgendaTec.Business.Bindings;
using AgendaTec.Business.Contracts;
using AgendaTec.Infrastructure.DatabaseModel;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AgendaTec.Tests
{
    [TestClass]
    public class ProfessionalTest
    {
        private readonly IProfessionalFacade _professionalRepository;

        public ProfessionalTest()
        {
            _professionalRepository = new ProfessionalFacade();
        }        

        [TestMethod]
        public void Professional_Insert()
        {
            var fakeProfessional = new Faker<TCGProfessionals>()
                .RuleFor(t => t.IDCustomer, f => 1)
                .RuleFor(t => t.IDUser, f => "8f6df88c-eb27-4b5b-a1eb-1b487eb49f52")
                .RuleFor(t => t.Name, f => f.Name.FullName())
                .RuleFor(t => t.Birthday, f => f.Date.Past(20))
                .RuleFor(t => t.Phone, f => f.Phone.PhoneNumber());
                
            var idProfessional = _professionalRepository.Insert(fakeProfessional, out string errorMessage).IDProfessional;

            Assert.IsTrue(idProfessional > 0);
        }

        [TestMethod]
        public void Professional_GetGrid()
        {
            var professionals = _professionalRepository.GetGrid(0, string.Empty, out string errorMessage);
            Assert.IsTrue(professionals.Any());
        }

        [TestMethod]
        public void Professional_GetProfessionalNameCombo()
        {
            var professionals = _professionalRepository.GetProfessionalNameCombo(1, Guid.Empty, out string errorMessage);
            Assert.IsTrue(professionals.Any());
        }

        [TestMethod]
        public void Professional_GetProfessionalById()
        {
            var professional = _professionalRepository.GetProfessionalById(1, out string errorMessage);
            Assert.IsTrue(!professional.IDProfessional.Equals(0)); 
        }

        [TestMethod]
        public void Professional_CheckUserInUse()
        {
            var inUse = _professionalRepository.CheckUserInUse(1, "8f6df88c-eb27-4b5b-a1eb-1b487eb49f52", out string errorMessage);
            Assert.IsTrue(inUse);
        }

        [TestMethod]
        public void Professional_Update()
        {
            var professional = _professionalRepository.GetGrid(0, string.Empty, out string errorMessage).First();
            professional.Name = new Bogus.DataSets.Name().FullName();
            _professionalRepository.Update(professional, out errorMessage);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void Professional_GetProfessionalNameComboClient()
        {
            var professional = _professionalRepository.GetProfessionalNameComboClient(1, true, out string errorMessage);            
            Assert.IsTrue(professional.Any());
        }
    }
}
