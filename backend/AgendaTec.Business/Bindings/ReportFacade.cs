using AgendaTec.Business.Contracts;
using AgendaTec.Business.Entities;
using AgendaTec.Infrastructure.Contracts;
using AgendaTec.Infrastructure.DatabaseModel;
using AgendaTec.Infrastructure.Repositories;
using AutoMapper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AgendaTec.Business.Bindings
{
    public class ReportFacade : IReportFacade
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public List<ScheduleReportDTO> GetScheduleReport(int idCustomer, DateTime initialDate, DateTime finalDate, out string errorMessage)
        {
            ICommonRepository<TSchedules> _commonRepository = new CommonRepository<TSchedules>();
            var report = new List<ScheduleReportDTO>();
            var schedules = new List<TSchedules>();

            errorMessage = string.Empty;

            try
            {
                schedules = _commonRepository
                    .Filter(x => x.IDCustomer.Equals(idCustomer) && x.Date >= initialDate && x.Date <= finalDate)
                    .ToList();

                report = Mapper.Map<List<TSchedules>, List<ScheduleReportDTO>>(schedules);
            }
            catch (Exception ex)
            {
                errorMessage = $"{ex.Message} - {ex.InnerException}";
                _logger.Error($"({MethodBase.GetCurrentMethod().Name}) {errorMessage}");
            }

            return report;            
        }
    }
}
