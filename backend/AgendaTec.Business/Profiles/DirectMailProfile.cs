using AgendaTec.Business.Entities;
using AgendaTec.Infrastructure.DatabaseModel;
using AutoMapper;

namespace AgendaTec.Business.Profiles
{
    public class DirectMailProfile : Profile
    {
        public DirectMailProfile()
        {
            CreateMap<TDirectMail, DirectMailDTO>()
                .ForMember(d => d.Id, s => s.MapFrom(m => m.IDMail))
                .ForMember(d => d.IdCustomer, s => s.MapFrom(m => m.IDCustomer))
                .ForMember(d => d.Description, s => s.MapFrom(m => m.Description))
                .ForMember(d => d.Content, s => s.MapFrom(m => m.BodyContent))
                .ForMember(d => d.Last, s => s.MapFrom(m => m.LastProcessed))
                .ForMember(d => d.IntervalType, s => s.MapFrom(m => m.IntervalType));

            CreateMap<DirectMailDTO, TDirectMail>()
               .ForMember(d => d.IDMail, s => s.MapFrom(m => m.Id))
               .ForMember(d => d.IDCustomer, s => s.MapFrom(m => m.IdCustomer))
               .ForMember(d => d.Description, s => s.MapFrom(m => m.Description))
               .ForMember(d => d.BodyContent, s => s.MapFrom(m => m.Content))
               .ForMember(d => d.LastProcessed, s => s.MapFrom(m => m.Last))
               .ForMember(d => d.IntervalType, s => s.MapFrom(m => m.IntervalType));
        }
    }
}
