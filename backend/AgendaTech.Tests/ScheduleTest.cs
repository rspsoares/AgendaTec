using AgendaTech.Business.Bindings;
using AgendaTech.Business.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgendaTech.Tests
{
    [TestClass]
    public class ScheduleTest
    {
        private readonly IScheduleFacade _scheduleFacade;

        public ScheduleTest()
        {
            _scheduleFacade = new ScheduleFacade();
        }

        [TestMethod]
        public void Schedule_Insert()
        {
            var fakeService = new Faker<TSchedules>()
               .RuleFor(t => t.IDCustomer, f => 1)
               .RuleFor(t => t.IDProfessional, f => 1)
               .RuleFor(t => t.IDService, f => 1)
               .RuleFor(t => t.IDConsumer, f => "AF3165EC-BF80-4C29-B3CB-60443BA047A4")
               .RuleFor(t => t.Date, f => f.Date.Soon())
               .RuleFor(t => t.Price, f => f.Random.Decimal(0, 1000))
               .RuleFor(t => t.Time, f => f.Random.Int(0, 60))
               .RuleFor(t => t.Bonus, f => f.Random.Bool());

            var idSchedule = _scheduleFacade.Insert(fakeService, out string errorMessage).IDSchedule;

            Assert.IsTrue(idSchedule > 0);
        }

        [TestMethod]
        public void Schedule_GetAll()
        {
            var schedules = _scheduleFacade.GetGrid(1, 0, 0, string.Empty, null, null, false, out string errorMessage);
            Assert.IsTrue(schedules.Any());
        }

        [TestMethod]
        public void Schedule_Delete()
        {
            var schedules = new List<TSchedules>()
            {
                new TSchedules()
                {
                    IDSchedule = 1
                }
            };

            _scheduleFacade.Delete(schedules, out string errorMessage);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void Schedule_CheckAvailability()
        {
            var schedule = _scheduleFacade.GetScheduleById(14, out string errorMessage);
            schedule.Date = DateTime.Parse($"{DateTime.Parse("2019-05-18").ToString("yyyy-MM-dd")} {schedule.Date.ToString("HH:mm")}");
            var schedules = new List<TSchedules>
            {
                schedule
            };

            var availabilityCheck = _scheduleFacade.CheckAvailability(schedules, out errorMessage);
            
            Assert.IsTrue(!string.IsNullOrEmpty(availabilityCheck));
        }
    }
}
