using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            var professional = new TCGProfessionals()
            {
                IDCustomer = 1,
                Name = "Name",
                Birthday = DateTime.Now,
                Phone = "1111",
                Email = "mmmmm"
            };

            var idProfessional = _professionalRepository.Insert(professional, out string errorMessage).IDProfessional;

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
            professional.Name = "Updated";
            _professionalRepository.Update(professional, out errorMessage);

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

    }
}
