using AgendaTech.Business.Contracts;
using AgendaTech.Business.Entities;
using AgendaTech.Infrastructure.Contracts;
using AgendaTech.Infrastructure.DatabaseModel;
using AgendaTech.Infrastructure.Repositories;
using Itenso.TimePeriod;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;

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

        public List<ScheduleDTO> GetGrid(int idCustomer, int idProfessional, int idService, Guid idConsumer, DateTime? dateFrom, DateTime? dateTo, bool? bonus, out string errorMessage)
        {
            var schedules = new List<TSchedules>();
            IUserFacade userRepository = new UserFacade();
            var userAccountDTO = new UserAccountDTO();

            errorMessage = string.Empty;

            try
            {                
                schedules = _commonRepository.GetAll();
                               
                schedules = schedules.Where(x => x.IDCustomer.Equals(idCustomer)).ToList();

                if (idProfessional > 0)
                    schedules = schedules.Where(x => x.IDProfessional.Equals(idProfessional)).ToList();

                if (idService > 0)
                    schedules = schedules.Where(x => x.IDService.Equals(idService)).ToList();

                if (!idConsumer.Equals(Guid.Empty))
                {
                    schedules = schedules.Where(x => x.IDConsumer.Equals(idConsumer)).ToList();
                    userAccountDTO = userRepository.GetUserByUq(idConsumer, out errorMessage);
                }

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
                    IDProfessional = x.IDProfessional,
                    ProfessionalName = x.TCGProfessionals.Name,
                    ServiceName = x.TCGServices.Description,
                    ConsumerName = $"{userAccountDTO.FirstName} {userAccountDTO.LastName}",
                    Date = x.Date.ToString("dd/MM/yyyy"),
                    Hour = x.Date.ToString("HH:mm"),
                    Price = x.Price,
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
                    Price = result.Price,
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
                _commonRepository.Update(e.IDSchedule, e);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }
        }

        public void Delete(List<TSchedules> schedules, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                schedules.ForEach(schedule => { _commonRepository.Delete(schedule.IDSchedule); });
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

        public string CheckAvailability(List<TSchedules> schedules, out string errorMessage, string newDate = null)
        {
            var currentSchedules = new List<TSchedules>();
            var availabilityCheck = new StringBuilder();
            
            errorMessage = string.Empty;

            try
            {
                schedules.ForEach(schedule =>
                {
                    if(!string.IsNullOrEmpty(newDate))
                        schedule.Date = DateTime.Parse($"{DateTime.Parse(newDate).ToString("yyyy-MM-dd")} {schedule.Date.ToString("HH:mm")}");

                    var timeRangeNewSchedule = new TimeRange(
                        schedule.Date,
                        schedule.Date.AddMinutes(schedule.Time));

                    if (schedule.IDSchedule.Equals(0))
                    {
                        currentSchedules = _commonRepository
                            .Filter(x => x.IDProfessional.Equals(schedule.IDProfessional))
                            .Where(x => x.Date.Date.Equals(schedule.Date.Date))
                            .ToList();
                    }                        
                    else
                    {
                        currentSchedules = _commonRepository
                            .Filter(x => !x.IDSchedule.Equals(schedule.IDSchedule) && x.IDProfessional.Equals(schedule.IDProfessional))
                            .Where(x => x.Date.Date.Equals(schedule.Date.Date))
                            .ToList();
                    }

                    currentSchedules.ForEach(current =>
                    {
                        var timeRangeCurrent = new TimeRange(
                            current.Date,
                            current.Date.AddMinutes(current.Time));

                        if (timeRangeCurrent.OverlapsWith(timeRangeNewSchedule))
                        {
                            availabilityCheck.AppendLine(
                                $"Já existe um horário marcado para o funcionário {currentSchedules.First().TCGProfessionals.Name} " +
                                $"no dia {schedule.Date.ToString("dd/MM/yyyy")} " +
                                $"das {timeRangeCurrent.Start.ToString("HH:mm")} " +
                                $"às {timeRangeCurrent.End.ToString("HH:mm")}.");
                        }                        
                    });
                });
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }            

            return availabilityCheck.ToString();
        }        
      
        public void Reschedule(List<TSchedules> schedules, string newDate, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                schedules.ForEach(schedule => 
                {
                    var appointment = _commonRepository.GetById(schedule.IDSchedule);
                    var dateProposal = DateTime.Parse($"{DateTime.Parse(newDate).ToString("yyyy-MM-dd")} {appointment.Date.ToString("HH:mm")}");

                    appointment.Date = dateProposal;
                    _commonRepository.Update(appointment.IDSchedule, appointment);
                });
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }
        }
    }
}
