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
               .RuleFor(t => t.IDConsumer, f => "ae74421f-50a1-4b45-a14b-9f4b2b3ee2a5")
               .RuleFor(t => t.Date, f => f.Date.Soon())
               .RuleFor(t => t.Price, f => f.Random.Decimal(0, 1000))
               .RuleFor(t => t.Time, f => f.Random.Int(0, 60))
               .RuleFor(t => t.Bonus, f => f.Random.Bool());

            var idSchedule = _scheduleFacade.Insert(fakeService, out string errorMessage).IDSchedule;

            Assert.IsTrue(idSchedule > 0);
        }

        [TestMethod]
        public void Schedule_GetGrid()
        {
            var schedules = _scheduleFacade.GetGrid(3, 0, 0, string.Empty, null, null, true, out string errorMessage);
            Assert.IsTrue(schedules.Any());
        }

        [TestMethod]
        public void Schedule_GetAvailableHours()
        {
            var availables = _scheduleFacade.GetAvailableHours(1, 3, 8, DateTime.Now, true, out string errorMessage);
            Assert.IsTrue(availables.Any());
        }

        [TestMethod]
        public void Schedule_GetScheduleById()
        {
            var schedule = _scheduleFacade.GetScheduleById(5, out string errorMessage);
            Assert.IsTrue(!schedule.IDSchedule.Equals(0));
        }

        [TestMethod]
        public void Schedule_Reschedule()
        {
            var schedules = new List<TSchedules>()
            {
                new TSchedules()
                {
                    IDSchedule = 2
                }
            };

            _scheduleFacade.Reschedule(schedules, "2019-05-10", out string errorMessage);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void Schedule_DeleteByList()
        {
            var schedules = new List<TSchedules>()
            {
                new TSchedules()
                {
                    IDSchedule = 8
                }
            };

            _scheduleFacade.Delete(schedules, out string errorMessage);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void Schedule_DeleteById()
        {
            _scheduleFacade.Delete(2, out string errorMessage);
            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void Schedule_CheckAvailability()
        {
            var schedule = _scheduleFacade.GetScheduleById(2, out string errorMessage);
            schedule.Date = DateTime.Parse($"{DateTime.Parse("2019-05-18").ToString("yyyy-MM-dd")} {schedule.Date.ToString("HH:mm")}");
            var schedules = new List<TSchedules>
            {
                schedule
            };

            var availabilityCheck = _scheduleFacade.CheckAvailability(schedules, out errorMessage);
            
            Assert.IsTrue(string.IsNullOrEmpty(availabilityCheck));
        }
    }
}
