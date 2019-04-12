using AgendaTech.Business.Contracts;
using AgendaTech.Business.Entities;
using AgendaTech.Infrastructure.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using AgendaTech.Infrastructure.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AgendaTech.Business.Bindings
{
    public class ScheduleFacade : IScheduleFacade
    {
        private readonly ICommonRepository<TSchedules> _commonRepository;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public ScheduleFacade()
        {
            _commonRepository = new CommonRepository<TSchedules>();
        }

        public List<ScheduleDTO> GetGrid(int idCustomer, int idProfessional, int idService, int idConsumer, DateTime? dateFrom, DateTime? dateTo, bool? bonus, out string errorMessage)
        {
            var schedules = new List<TSchedules>();

            errorMessage = string.Empty;

            try
            {
                schedules = _commonRepository.GetAll();
                               
                schedules = schedules.Where(x => x.IDCustomer.Equals(idCustomer)).ToList();

                if (idProfessional > 0)
                    schedules = schedules.Where(x => x.IDProfessional.Equals(idProfessional)).ToList();

                if (idService > 0)
                    schedules = schedules.Where(x => x.IDService.Equals(idService)).ToList();

                if (idConsumer > 0)
                    schedules = schedules.Where(x => x.IDConsumer.Equals(idConsumer)).ToList();

                if(dateFrom.HasValue)
                    schedules = schedules.Where(x => x.Date >= dateFrom.Value).ToList();

                if (dateTo.HasValue)
                    schedules = schedules.Where(x => x.Date <= dateTo.Value).ToList();

                if(bonus.HasValue)
                    schedules = schedules.Where(x => x.Bonus.Equals(bonus)).ToList();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return schedules
                .Select(x => new ScheduleDTO()
                {
                    IDSchedule = x.IDSchedule,
                    ProfessionalName = x.TCGProfessionals.Name,
                    ServiceName = x.TCGServices.Description,
                    ConsumerName = $"{x.UserAccounts.FirstName} {x.UserAccounts.LastName}",
                    Date = x.Date,
                    Value = x.Value,
                    Time = x.Time,
                    Bonus = x.Bonus
                })
                .OrderBy(x => x.Date)
                .ThenBy(x => x.ProfessionalName)
                .ToList();
        }

        public TSchedules GetScheduleById(int idSchedule, out string errorMessage)
        {
            var schedule = new TSchedules();

            errorMessage = string.Empty;

            try
            {
                var result = _commonRepository.GetById(idSchedule);

                schedule = new TSchedules()
                {
                    IDSchedule = result.IDSchedule,
                    IDCustomer = result.IDCustomer,
                    IDProfessional = result.IDProfessional,
                    IDService = result.IDService,
                    IDConsumer = result.IDConsumer,
                    Date = result.Date,
                    Value = result.Value,
                    Time = result.Time,
                    Bonus = result.Bonus
                };
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return schedule;
        }

        public TSchedules Insert(TSchedules e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                e = _commonRepository.Insert(e);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return e;
        }

        public void Update(TSchedules e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                _commonRepository.Update(e);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }
        }

        public void Delete(int idSchedule, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                _commonRepository.Delete(idSchedule);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }
        }
    }
}
