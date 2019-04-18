using AgendaTech.Business.Entities;
using AgendaTech.Infrastructure.DatabaseModel;
using System;
using System.Collections.Generic;

namespace AgendaTech.Business.Contracts
{
    public interface IScheduleFacade
    {
        List<ScheduleDTO> GetGrid(int idCustomer, int idProfessional, int idService, Guid idConsumer, DateTime? dateFrom, DateTime? dateTo, bool? bonus, out string errorMessage);
        TSchedules GetScheduleById(int idSchedule, out string errorMessage);
        TSchedules Insert(TSchedules e, out string errorMessage);
        void Update(TSchedules e, out string errorMessage);
        void Delete(List<TSchedules> schedules, out string errorMessage);
        bool CheckAvailability(List<TSchedules> schedules, string newDate, out string errorMessage);
        void Reschedule(List<TSchedules> schedules, string newDate, out string errorMessage);
    }
}
