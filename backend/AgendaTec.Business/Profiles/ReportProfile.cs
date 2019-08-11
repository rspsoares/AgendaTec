using AgendaTec.Business.Entities;
using AgendaTec.Infrastructure.DatabaseModel;
using AutoMapper;

namespace AgendaTec.Business.Profiles
{
    public class ReportProfile: Profile
    {
        public ReportProfile()
        {
            CreateMap<TSchedules, ScheduleReportDTO>()
               .ForMember(d => d.Date, s => s.MapFrom(m => m.Date.ToString("yyyy/MM/dd")))
               .ForMember(d => d.ConsumerName, s => s.MapFrom(m => $"{m.AspNetUsers.FirstName} {m.AspNetUsers.LastName}"))
               .ForMember(d => d.ServiceDescription, s => s.MapFrom(m => m.TCGServices.Description))
               .ForMember(d => d.Price, s => s.MapFrom(m => m.Price))
               .ForMember(d => d.Attended, s => s.MapFrom(m => m.Attended ? "Sim" : "Não"));
        }
    }
}
