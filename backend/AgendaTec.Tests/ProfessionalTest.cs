using AgendaTec.Business.Bindings;
using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
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
            ProfilesHelper.Initialize();

            var fakeProfessional = new Faker<ProfessionalDTO>()
                .RuleFor(t => t.IdCustomer, f => 1)
                .RuleFor(t => t.IdUser, f => "039a1db4-d562-4014-8f68-17dff6a388e1")
                .RuleFor(t => t.Name, f => f.Name.FullName())
                .RuleFor(t => t.Birthday, f => f.Date.Past(20))
                .RuleFor(t => t.Phone, f => f.Phone.PhoneNumber());
                
            var idProfessional = _professionalRepository.Insert(fakeProfessional, out string errorMessage).Id;

            ProfilesHelper.Reset();

            Assert.IsTrue(idProfessional > 0);
        }

        [TestMethod]
        public void Professional_GetGrid()
        {
            ProfilesHelper.Initialize();
            var professionals = _professionalRepository.GetGrid(0, string.Empty, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(professionals.Any());
        }

        [TestMethod]
        public void Professional_GetProfessionalNameCombo()
        {
            ProfilesHelper.Initialize();
            var professionals = _professionalRepository.GetProfessionalNameCombo(1, Guid.Empty, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(professionals.Any());
        }

        [TestMethod]
        public void Professional_GetProfessionalById()
        {
            ProfilesHelper.Initialize();
            var professional = _professionalRepository.GetProfessionalById(3, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(!professional.Id.Equals(0)); 
        }

        [TestMethod]
        public void Professional_CheckUserInUse()
        {
            ProfilesHelper.Initialize();
            var inUse = _professionalRepository.CheckUserInUse(3, "039a1db4-d562-4014-8f68-17dff6a388e1", out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(inUse);            
        }

        [TestMethod]
        public void Professional_Update()
        {
            ProfilesHelper.Initialize();

            var professional = _professionalRepository.GetGrid(0, string.Empty, out string errorMessage).First();
            professional.Name = new Bogus.DataSets.Name().FullName();
            _professionalRepository.Update(professional, out errorMessage);

            ProfilesHelper.Reset();

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }       
    }
}
