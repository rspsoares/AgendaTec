
using AgendaTec.Business.Entities;
using System;
using System.Collections.Generic;

namespace AgendaTec.Business.Contracts
{
    public interface IReportFacade
    {
        List<ScheduleReportDTO> GetScheduleReport(int idCustomer, DateTime initialDate, DateTime finalDate, out string errorMessage);
    }
}
