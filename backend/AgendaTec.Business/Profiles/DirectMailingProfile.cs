using AgendaTec.Business.Entities;
using AgendaTec.Infrastructure.DatabaseModel;
using AutoMapper;

namespace AgendaTec.Business.Profiles
{
    public class DirectMailingProfile : Profile
    {
        public DirectMailingProfile()
        {
            CreateMap<TDirectMailing, DirectMailingDTO>()
                .ForMember(d => d.Id, s => s.MapFrom(m => m.IDMail))
                .ForMember(d => d.IdCustomer, s => s.MapFrom(m => m.IDCustomer))
                .ForMember(d => d.Description, s => s.MapFrom(m => m.Description))
                .ForMember(d => d.Content, s => s.MapFrom(m => m.BodyContent))
                .ForMember(d => d.Start, s => s.MapFrom(m => m.StartDate))
                .ForMember(d => d.Last, s => s.MapFrom(m => m.LastProcessed))
                .ForMember(d => d.Interval, s => s.MapFrom(m => m.IntervalType))
                .ForMember(d => d.Active, s => s.MapFrom(m => m.Active));

            CreateMap<DirectMailingDTO, TDirectMailing>()
               .ForMember(d => d.IDMail, s => s.MapFrom(m => m.Id))
               .ForMember(d => d.IDCustomer, s => s.MapFrom(m => m.IdCustomer))
               .ForMember(d => d.Description, s => s.MapFrom(m => m.Description))
               .ForMember(d => d.BodyContent, s => s.MapFrom(m => m.Content))
               .ForMember(d => d.StartDate, s => s.MapFrom(m => m.Start))
               .ForMember(d => d.LastProcessed, s => s.MapFrom(m => m.Last))
               .ForMember(d => d.IntervalType, s => s.MapFrom(m => m.Interval))
               .ForMember(d => d.Active, s => s.MapFrom(m => m.Active));
        }
    }
}
