using AgendaTec.Business.Entities;
using System;
using System.Collections.Generic;

namespace AgendaTec.Business.Contracts
{
    public interface IScheduleFacade
    {
        List<ScheduleDTO> GetGrid(int idCustomer, int idProfessional, int idService, string idConsumer, DateTime? dateFrom, DateTime? dateTo, bool? bonus, out string errorMessage);
        ScheduleDTO GetScheduleById(int idSchedule, out string errorMessage);
        ScheduleDTO Insert(ScheduleDTO e, out string errorMessage);
        void Update(ScheduleDTO e, out string errorMessage);
        void Delete(int idSchedule, out string errorMessage);
        void Delete(List<ScheduleDTO> schedules, out string errorMessage);
        string CheckAvailability(List<ScheduleDTO> schedules, out string errorMessage, string newDate = null);
        void Reschedule(List<ScheduleDTO> schedules, string newDate, out string errorMessage);
        List<string> GetAvailableHours(int idCustomer, int idProfessional, int idService, DateTime date, string idConsumer, bool authenticated, out string errorMessage);
    }
}
