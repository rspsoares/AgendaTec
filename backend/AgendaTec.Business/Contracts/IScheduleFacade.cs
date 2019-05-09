using AgendaTec.Business.Entities;
using AgendaTec.Infrastructure.DatabaseModel;
using System;
using System.Collections.Generic;

namespace AgendaTec.Business.Contracts
{
    public interface IScheduleFacade
    {
        List<ScheduleDTO> GetGrid(int idCustomer, int idProfessional, int idService, string idConsumer, DateTime? dateFrom, DateTime? dateTo, bool? bonus, out string errorMessage);
        TSchedules GetScheduleById(int idSchedule, out string errorMessage);
        TSchedules Insert(TSchedules e, out string errorMessage);
        void Update(TSchedules e, out string errorMessage);
        void Delete(int idSchedule, out string errorMessage);
        void Delete(List<TSchedules> schedules, out string errorMessage);
        string CheckAvailability(List<TSchedules> schedules, out string errorMessage, string newDate = null);
        void Reschedule(List<TSchedules> schedules, string newDate, out string errorMessage);
        List<string> GetAvailableHours(int idCustomer, int idProfessional, int idService, DateTime date, string idConsumer, bool authenticated, out string errorMessage);
    }
}
