using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AgendaTech.Tests
{
    [TestClass]
    public class ScheduleTest
    {
        private readonly IScheduleFacade _scheduleRepository;

        public ScheduleTest()
        {
            _scheduleRepository = new ScheduleFacade();
        }

        [TestMethod]
        public void Schedule_Insert()
        {
            var fakeService = new Faker<TSchedules>()
               .RuleFor(t => t.IDCustomer, f => 1)
               .RuleFor(t => t.IDProfessional, f => 1)
               .RuleFor(t => t.IDService, f => 1)
               .RuleFor(t => t.IDConsumer, f => 1)
               .RuleFor(t => t.Date, f => f.Date.Soon())
               .RuleFor(t => t.Value, f => f.Random.Decimal(0, 1000))
               .RuleFor(t => t.Time, f => f.Random.Int(0, 60))
               .RuleFor(t => t.Bonus, f => f.Random.Bool());

            var idSchedule = _scheduleRepository.Insert(fakeService, out string errorMessage).IDSchedule;

            Assert.IsTrue(idSchedule > 0);
        }

        [TestMethod]
        public void Schedule_GetAll()
        {
            var schedules = _scheduleRepository.GetGrid(1, 0, 0, 0, null, null, false, out string errorMessage);
            Assert.IsTrue(schedules.Any());
        }

        [TestMethod]
        public void Schedule_Delete()
        {
            _scheduleRepository.Delete(1, out string errorMessage);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }
    }
}
