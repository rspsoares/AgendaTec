using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AgendaTech.Tests
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
                .RuleFor(t => t.Name, f => f.Name.FullName())
                .RuleFor(t => t.Birthday, f => f.Date.Past(20))
                .RuleFor(t => t.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(t => t.Email, f => f.Internet.ExampleEmail());
                
            var idProfessional = _professionalRepository.Insert(fakeProfessional, out string errorMessage).IDProfessional;

            Assert.IsTrue(idProfessional > 0);
        }

        [TestMethod]
        public void Professional_GetAll()
        {
            var professional = _professionalRepository.GetGrid(0, string.Empty, out string errorMessage);
            Assert.IsTrue(professional.Any());
        }

        [TestMethod]
        public void Professional_Update()
        {
            var professional = _professionalRepository.GetGrid(0, string.Empty, out string errorMessage).First();
            professional.Name = new Bogus.DataSets.Name().FullName();
            _professionalRepository.Update(professional, out errorMessage);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }
    }
}
