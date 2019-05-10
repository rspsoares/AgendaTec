using AgendaTec.Business.Entities;
using AgendaTec.Infrastructure.DatabaseModel;
using AutoMapper;

namespace AgendaTec.Business.Profiles
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<TCGServices, ServiceDTO>()
                .ForMember(d => d.Id, s => s.MapFrom(m => m.IDService))
                .ForMember(d => d.IdCustomer, s => s.MapFrom(m => m.IDCustomer))
                .ForMember(d => d.Description, s => s.MapFrom(m => m.Description))
                .ForMember(d => d.Price, s => s.MapFrom(m => m.Price))
                .ForMember(d => d.Time, s => s.MapFrom(m => m.Time));

            CreateMap<ServiceDTO, TCGServices>()
                .ForMember(d => d.IDService, s => s.MapFrom(m => m.Id))
                .ForMember(d => d.IDCustomer, s => s.MapFrom(m => m.IdCustomer))
                .ForMember(d => d.Description, s => s.MapFrom(m => m.Description))
                .ForMember(d => d.Price, s => s.MapFrom(m => m.Price))
                .ForMember(d => d.Time, s => s.MapFrom(m => m.Time));
        }
    }
}
