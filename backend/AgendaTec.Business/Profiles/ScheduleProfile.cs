using AgendaTec.Business.Entities;
using AgendaTec.Infrastructure.DatabaseModel;
using AutoMapper;

namespace AgendaTec.Business.Profiles
{
    public class ScheduleProfile: Profile
    {
        public ScheduleProfile()
        {
            CreateMap<TSchedules, ScheduleDTO>()
               .ForMember(d => d.Id, s => s.MapFrom(m => m.IDSchedule))
               .ForMember(d => d.IdCustomer, s => s.MapFrom(m => m.IDCustomer))
               .ForMember(d => d.IdProfessional, s => s.MapFrom(m => m.IDProfessional))
               .ForMember(d => d.ProfessionalName, s => s.MapFrom(m => m.TCGProfessionals.Name))
               .ForMember(d => d.IdService, s => s.MapFrom(m => m.IDService))
               .ForMember(d => d.ServiceName, s => s.MapFrom(m => m.TCGServices.Description))
               .ForMember(d => d.IdConsumer, s => s.MapFrom(m => m.IDConsumer))
               .ForMember(d => d.ConsumerName, s => s.MapFrom(m => m.AspNetUsers.FirstName + m.AspNetUsers.LastName))
               .ForMember(d => d.Date, s => s.MapFrom(m => m.Date.ToString("yyyy-MM-dd")))
               .ForMember(d => d.Hour, s => s.MapFrom(m => m.Date.ToString("HH:mm")))
               .ForMember(d => d.Time, s => s.MapFrom(m => m.Time))
               .ForMember(d => d.Finish, s => s.MapFrom(m => m.Date.AddMinutes(m.Time).ToString("HH:mm")))
               .ForMember(d => d.Price, s => s.MapFrom(m => m.Price))
               .ForMember(d => d.Bonus, s => s.MapFrom(m => m.Bonus));

            CreateMap<ScheduleDTO, TSchedules>()
                .ForMember(d => d.IDSchedule, s => s.MapFrom(m => m.Id))
                .ForMember(d => d.IDCustomer, s => s.MapFrom(m => m.IdCustomer))
                .ForMember(d => d.IDProfessional, s => s.MapFrom(m => m.IdProfessional))
                .ForMember(d => d.IDService, s => s.MapFrom(m => m.IdService))
                .ForMember(d => d.IDConsumer, s => s.MapFrom(m => m.IdConsumer))
                .ForMember(d => d.Date, s => s.MapFrom(m => m.Date))
                .ForMember(d => d.Price, s => s.MapFrom(m => m.Price))
                .ForMember(d => d.Time, s => s.MapFrom(m => m.Time))
                .ForMember(d => d.Bonus, s => s.MapFrom(m => m.Bonus));

            CreateMap<TSchedules, AppointmentDTO>()
                .ForMember(d => d.Title, s => s.MapFrom(m => m.TCGServices.Description + " - " + m.AspNetUsers.FirstName + " " + m.AspNetUsers.LastName))
                .ForMember(d => d.Start, s => s.MapFrom(m => m.Date.ToString("HH:mm")))
                .ForMember(d => d.End, s => s.MapFrom(m => m.Date.AddMinutes(m.Time).ToString("HH:mm")));
        }
    }
}
