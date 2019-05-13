using AgendaTec.Business.Bindings;
using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
using Bogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgendaTec.Tests
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
            ProfilesHelper.Initialize();

            var fakeService = new Faker<ScheduleDTO>()
               .RuleFor(t => t.IdCustomer, f => 1)
               .RuleFor(t => t.IdProfessional, f => 4)
               .RuleFor(t => t.IdService, f => 8)
               .RuleFor(t => t.IdConsumer, f => "039a1db4-d562-4014-8f68-17dff6a388e1")
               .RuleFor(t => t.Date, f => f.Date.Soon().ToString("yyyy-MM-dd HH:mm"))
               .RuleFor(t => t.Price, f => f.Random.Decimal(0, 1000))
               .RuleFor(t => t.Time, f => f.Random.Int(0, 60))
               .RuleFor(t => t.Bonus, f => f.Random.Bool());

            var idSchedule = _scheduleFacade.Insert(fakeService, out string errorMessage).Id;

            ProfilesHelper.Reset();

            Assert.IsTrue(idSchedule > 0);
        }

        [TestMethod]
        public void Schedule_GetGrid()
        {
            ProfilesHelper.Initialize();
            var schedules = _scheduleFacade.GetGrid(1, 0, 0, string.Empty, null, null, true, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(schedules.Any());
        }

        [TestMethod]
        public void Schedule_GetAvailableHours()
        {
            ProfilesHelper.Initialize();
            var availables = _scheduleFacade.GetAvailableHours(1, 4, 10, DateTime.Parse("2019-05-08"), "258adb1e-781f-4691-bc8f-5d6ce7d3dbaf", true, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(availables.Any());
        }

        [TestMethod]
        public void Schedule_GetScheduleById()
        {
            ProfilesHelper.Initialize();
            var schedule = _scheduleFacade.GetScheduleById(16, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(!schedule.Id.Equals(0));
        }

        [TestMethod]
        public void Schedule_Reschedule()
        {
            var schedules = new List<ScheduleDTO>()
            {
                new ScheduleDTO()
                {
                    Id = 20
                }
            };

            ProfilesHelper.Initialize();
            _scheduleFacade.Reschedule(schedules, "2019-05-10", out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void Schedule_DeleteByList()
        {
            var schedules = new List<ScheduleDTO>()
            {
                new ScheduleDTO()
                {
                    Id = 106
                }
            };

            ProfilesHelper.Initialize();
            _scheduleFacade.Delete(schedules, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void Schedule_DeleteById()
        {
            ProfilesHelper.Initialize();
            _scheduleFacade.Delete(107, out string errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        [TestMethod]
        public void Schedule_CheckAvailability()
        {
            ProfilesHelper.Initialize();

            var schedule = _scheduleFacade.GetScheduleById(20, out string errorMessage);
            schedule.Date = DateTime.Parse($"{DateTime.Parse("2019-05-18").ToString("yyyy-MM-dd")} {DateTime.Parse(schedule.Date).ToString("HH:mm")}").ToString("yyyy-MM-dd HH:mm");
            var schedules = new List<ScheduleDTO>
            {
                schedule
            };
            
            var availabilityCheck = _scheduleFacade.CheckAvailability(schedules, out errorMessage);
            ProfilesHelper.Reset();

            Assert.IsTrue(string.IsNullOrEmpty(availabilityCheck));
        }

        [TestMethod]
        public void Schedule_GetTodaysAppointments()
        {
            ProfilesHelper.Initialize();
            var schedules = _scheduleFacade.GetTodaysAppointments(5, out string errorMessage);           
            ProfilesHelper.Reset();

            Assert.IsTrue(schedules.Any());
        }
    }
}
