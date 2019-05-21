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
    public class DirectMailingTest
    {
        private readonly IDirectMailingFacade _directMailingRepository;

        public DirectMailingTest()
        {
            _directMailingRepository = new DirectMailingFacade();
        }

        [TestMethod]
        public void DirectMailing_Insert()
        {
            ProfilesHelper.Initialize();

            var fakeMail = new Faker<DirectMailingDTO>()
                .RuleFor(t => t.IdCustomer, f => 1)
                .RuleFor(t => t.Description, f => f.Name.JobArea())
                .RuleFor(t => t.Content, f => f.Lorem.Sentence(20))
                .RuleFor(t => t.Start, f => f.Date.Past(20))
                .RuleFor(t => t.Last, f => f.Date.Past(20))
                .RuleFor(t => t.Interval, f => 1)
                .RuleFor(t => t.Active, f => f.Random.Bool());

            var idDirectMail = _directMailingRepository.Insert(fakeMail, out string errorMessage).Id;

            ProfilesHelper.Reset();

            Assert.IsTrue(idDirectMail > 0);
        }

        [TestMethod]
        public void DirectMailing_GetGrid()
        {
            ProfilesHelper.Initialize();
            var mailings = _directMailingRepository.GetGrid(1, string.Empty, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(mailings.Any());
        }

        [TestMethod]
        public void DirectMailing_GetDirectMailingById()
        {
            ProfilesHelper.Initialize();
            var mailing = _directMailingRepository.GetDirectMailingById(1, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(!mailing.Id.Equals(0));
        }

        [TestMethod]
        public void DirectMailing_Update()
        {
            ProfilesHelper.Initialize();

            var mailing = _directMailingRepository.GetGrid(1, string.Empty, out string errorMessage).First();
            mailing.Description = new Bogus.DataSets.Name().JobArea();
            _directMailingRepository.Update(mailing, out errorMessage);

            ProfilesHelper.Reset();

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void DirectMailing_Delete()
        {
            _directMailingRepository.Delete(1, out string errorMessage);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }
    }
}
