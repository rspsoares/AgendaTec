using System;
using System.Collections.Generic;
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
    public class StressTest
    {
        private readonly IProfessionalFacade _professionalRepository;
        private readonly IScheduleFacade _scheduleFacade;
        private readonly IUserFacade _userRepository;
        private readonly IServiceFacade _serviceRepository;

        public StressTest()
        {
            _scheduleFacade = new ScheduleFacade();
            _professionalRepository = new ProfessionalFacade();
            _userRepository = new UserFacade();
            _serviceRepository = new ServiceFacade();
        }

        [TestMethod]
        public void Stress_GenerateSchedules()
        {
            var errorMessage = string.Empty;
            var qtdProfessionals = 3;

            ProfilesHelper.Initialize();

            var consumer = GetConsumer();
            var service = GetService();
            var allDays = GenarateAllDayTimes(service);
            var professionals = GetProfessionals(qtdProfessionals);
            
            professionals.ForEach(professional =>
            {
                allDays.ForEach(day =>
                {
                    var fakeSchedule = new Faker<ScheduleDTO>()
                        .RuleFor(t => t.IdCustomer, f => 1)
                        .RuleFor(t => t.IdProfessional, f => professional.Id)
                        .RuleFor(t => t.IdService, f => service.Id)
                        .RuleFor(t => t.IdConsumer, f => consumer.Id)
                        .RuleFor(t => t.Date, f => day.ToString("yyyy-MM-dd HH:mm"))
                        .RuleFor(t => t.Price, f => f.Random.Decimal(0, 1000))
                        .RuleFor(t => t.Time, f => service.Time)
                        .RuleFor(t => t.Bonus, f => f.Random.Bool());

                    _scheduleFacade.Insert(fakeSchedule, out errorMessage);
                });
            });            
            
            ProfilesHelper.Reset();

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
        }

        private List<ProfessionalDTO> GetProfessionals(int qtdProfessionals)
        {
            return _professionalRepository
              .GetGrid(1, string.Empty, out string errorMessage)
              .Take(qtdProfessionals)
              .ToList();
        }

        private UserAccountDTO GetConsumer()
        {
            return _userRepository
                .GetConsumerNamesCombo(1, out string errorMessage)
                .FirstOrDefault();
        }

        private ServiceDTO GetService()
        {
            var fakeService = new Faker<ServiceDTO>()
               .RuleFor(t => t.Id, f => 0)
               .RuleFor(t => t.IdCustomer, f => 1)
               .RuleFor(t => t.Description, f => f.Commerce.ProductName().ToString())
               .RuleFor(t => t.Price, f => f.Random.Decimal(0, 1000))
               .RuleFor(t => t.Time, f => 30);

            var service = (ServiceDTO)fakeService;
            service.Id = _serviceRepository.Insert(fakeService, out string errorMessage).Id;

            return service;
        }
        
        private List<DateTime> GenarateAllDayTimes(ServiceDTO service)
        {
            var allDays = new List<DateTime>();
            var customer = new CustomerDTO()
            {
                Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 8, 0, 0),
                End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 0, 0),
            };

            var days = GetDays(DateTime.Now.Year);
            days.ForEach(day => {
                allDays.AddRange(_scheduleFacade.GetPossibleTimes(customer, service, DateTime.Parse(day)));
            });

            return allDays;
        }

        private List<string> GetDays(int year)
        {
            var days = new List<string>();
            var start = new DateTime(year, 1, 1);

            while (start.Year.Equals(year))
            {
                days.Add(start.ToString("yyyy-MM-dd"));
                start = start.AddDays(1);
            }

            return days;
        }
    }
}
