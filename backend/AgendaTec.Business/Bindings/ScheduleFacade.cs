using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Infrastructure.Contracts;
using AgendaTec.Infrastructure.DatabaseModel;
using AgendaTec.Infrastructure.Repositories;
using AutoMapper;
using Itenso.TimePeriod;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AgendaTec.Business.Bindings
{
    public class ScheduleFacade : IScheduleFacade
    {
        private readonly ICommonRepository<TSchedules> _commonRepository;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public ScheduleFacade()
        {
            _commonRepository = new CommonRepository<TSchedules>();
        }

        public List<ScheduleDTO> GetGrid(int idCustomer, int idProfessional, int idService, string idConsumer, DateTime? dateFrom, DateTime? dateTo, bool? bonus, out string errorMessage)
        {
            var result = new List<ScheduleDTO>();
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

                if (!idConsumer.Equals(string.Empty))                
                    schedules = schedules.Where(x => x.IDConsumer.Equals(idConsumer)).ToList();                    
                
                if (dateFrom.HasValue)
                    schedules = schedules.Where(x => x.Date.Date >= dateFrom.Value.Date).ToList();

                if (dateTo.HasValue)
                    schedules = schedules.Where(x => x.Date.Date <= dateTo.Value.Date).ToList();

                if(bonus.HasValue)
                    schedules = schedules.Where(x => x.Bonus.Equals(bonus)).ToList();

                result = Mapper.Map<List<TSchedules>, List<ScheduleDTO>>(schedules);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return result
                .OrderBy(x => x.ProfessionalName)
                .ThenBy(x => x.Date)
                .ThenBy(x => x.Hour)
                .ToList();
        }

        public List<string> GetAvailableHours(int idCustomer, int idProfessional, int idService, DateTime date, string idConsumer, bool authenticated, out string errorMessage)
        {
            var hours = new List<string>();
            ICustomerFacade customerRepository = new CustomerFacade();
            IServiceFacade serviceRepository = new ServiceFacade();

            errorMessage = string.Empty;

            try
            {
                if(authenticated)
                {
                    if (idService > 0 && idProfessional > 0)
                    {
                        var customer = customerRepository.GetCustomerById(idCustomer, out errorMessage);
                        var service = serviceRepository.GetServiceById(idService, out errorMessage);

                        var possibleTimes = GetPossibleTimes(customer, service, date);                        

                        var appointments = _commonRepository
                            .GetAll()
                            .Where
                                (x => x.IDCustomer.Equals(idCustomer) && x.Date.ToString("yyyy-MM-dd").Equals(date.ToString("yyyy-MM-dd")))
                            .ToList();

                        var professionalAppointments = appointments.Where(x => x.IDProfessional.Equals(idProfessional)).ToList();
                        professionalAppointments.ForEach(appointment =>
                        {
                            var timeRangeAppointment = new TimeRange(
                                appointment.Date,
                                appointment.Date.AddMinutes(appointment.Time));

                            possibleTimes.RemoveAll(x => x >= timeRangeAppointment.Start && x < timeRangeAppointment.End);
                        });
                        
                        var consumerAppointments = appointments.Where(x => x.IDConsumer.ToUpper().Equals(idConsumer.ToUpper())).ToList();
                        consumerAppointments.ForEach(appointment =>
                        {
                            var timeRangeAppointment = new TimeRange(
                                appointment.Date,
                                appointment.Date.AddMinutes(appointment.Time));

                            possibleTimes.RemoveAll(x => x >= timeRangeAppointment.Start && x < timeRangeAppointment.End);
                        });

                        possibleTimes.ForEach(time => { hours.Add(time.ToString("HH:mm")); });
                    }
                    else
                        errorMessage = "Para visualizar as opções de horários, favor selecionar o profissional e o serviço.";
                }
                else
                    errorMessage = "Para visualizar as opções de horários, favor efetuar o Login.";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return hours;
        }

        private List<DateTime> GetPossibleTimes(CustomerDTO customer, ServiceDTO service, DateTime date)
        {
            var possibleHours = new List<DateTime>();            
            var hourLimit = new DateTime(date.Year, date.Month, date.Day, int.Parse(customer.End.ToString("HH")), int.Parse(customer.End.ToString("mm")), 0);
            
            var timeBlockStart = new TimeBlock(
                new DateTime(date.Year, date.Month, date.Day, int.Parse(customer.Start.ToString("HH")), int.Parse(customer.Start.ToString("mm")), 0),
                new TimeSpan(0, service.Time, 0));

            possibleHours.Add(timeBlockStart.Start);
            possibleHours.Add(timeBlockStart.End);
            var nextPeriod = (ITimeBlock)timeBlockStart;

            while (true)
            {
                nextPeriod = nextPeriod.GetNextPeriod(new TimeSpan(0, service.Time, 0));

                if (nextPeriod.End > hourLimit)
                    break;

                possibleHours.Add(nextPeriod.Start);
                possibleHours.Add(nextPeriod.End);                
            }

            return possibleHours;
        }

        public ScheduleDTO GetScheduleById(int idSchedule, out string errorMessage)
        {
            var result = new ScheduleDTO();
            var schedule = new TSchedules();
            
            errorMessage = string.Empty;

            try
            {
                schedule = _commonRepository.GetById(idSchedule);
                result = Mapper.Map<TSchedules, ScheduleDTO>(schedule);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return result;
        }

        public ScheduleDTO Insert(ScheduleDTO e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var result = Mapper.Map<ScheduleDTO, TSchedules>(e);
                result = _commonRepository.Insert(result);
                e.Id = result.IDSchedule;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return e;
        }

        public void Update(ScheduleDTO e, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                var result = Mapper.Map<ScheduleDTO, TSchedules>(e);
                _commonRepository.Update(result.IDSchedule, result);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }
        }

        public void Delete(List<ScheduleDTO> schedules, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                schedules.ForEach(schedule => { _commonRepository.Delete(schedule.Id); });
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

        public string CheckAvailability(List<ScheduleDTO> schedules, out string errorMessage, string newDate = null)
        {
            var currentSchedules = new List<TSchedules>();
            var availabilityCheck = new StringBuilder();
            
            errorMessage = string.Empty;

            try
            {
                schedules.ForEach(schedule =>
                {
                    if(!string.IsNullOrEmpty(newDate))
                        schedule.Date = DateTime.Parse($"{DateTime.Parse(newDate).ToString("yyyy-MM-dd")} {DateTime.Parse(schedule.Date).ToString("HH:mm")}").ToString("yyyy-MM-dd HH:mm");

                    var timeRangeNewSchedule = new TimeRange(
                        DateTime.Parse(schedule.Date),
                        DateTime.Parse(schedule.Date).AddMinutes(schedule.Time));

                    if (schedule.Id.Equals(0))
                    {
                        currentSchedules = _commonRepository
                            .Filter(x => x.IDProfessional.Equals(schedule.IdProfessional))
                            .Where(x => x.Date.Date.Equals(DateTime.Parse(schedule.Date).Date))
                            .ToList();
                    }                        
                    else
                    {
                        currentSchedules = _commonRepository
                            .Filter(x => !x.IDSchedule.Equals(schedule.Id) && x.IDProfessional.Equals(schedule.IdProfessional))
                            .Where(x => x.Date.Date.Equals(DateTime.Parse(schedule.Date).Date))
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
                                $"no dia {DateTime.Parse(schedule.Date).ToString("dd/MM/yyyy")} " +
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
      
        public void Reschedule(List<ScheduleDTO> schedules, string newDate, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                schedules.ForEach(schedule => 
                {
                    var appointment = _commonRepository.GetById(schedule.Id);
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

        public List<AppointmentDTO> GetTodaysAppointments(int idProfessional, out string errorMessage)
        {
            var result = new List<AppointmentDTO>();

            errorMessage = string.Empty;

            try
            {
                var appointments = _commonRepository
                    .Filter(x => x.IDProfessional.Equals(idProfessional))
                    .Where(x => x.Date.ToString("yyyy-MM-dd").Equals(DateTime.Now.ToString("yyyy-MM-dd")))
                    .ToList();

                result = Mapper.Map<List<TSchedules>, List<AppointmentDTO>>(appointments);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {ex.Message} - {ex.InnerException}");
            }

            return result;
        }
    }
}
