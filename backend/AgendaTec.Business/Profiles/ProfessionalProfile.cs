using AgendaTec.Business.Entities;
using AgendaTec.Infrastructure.DatabaseModel;
using AutoMapper;

namespace AgendaTec.Business.Profiles
{
    public class ProfessionalProfile: Profile
    {
        public ProfessionalProfile()
        {
            CreateMap<TCGProfessionals, ProfessionalDTO>()
               .ForMember(d => d.Id, s => s.MapFrom(m => m.IDProfessional))
               .ForMember(d => d.IdCustomer, s => s.MapFrom(m => m.IDCustomer))
               .ForMember(d => d.IdUser, s => s.MapFrom(m => m.IDUser))
               .ForMember(d => d.Name, s => s.MapFrom(m => m.Name))
               .ForMember(d => d.Birthday, s => s.MapFrom(m => m.Birthday))
               .ForMember(d => d.Phone, s => s.MapFrom(m => m.Phone));               

            CreateMap<ProfessionalDTO, TCGProfessionals>()
               .ForMember(d => d.IDProfessional, s => s.MapFrom(m => m.Id))
               .ForMember(d => d.IDCustomer, s => s.MapFrom(m => m.IdCustomer))
               .ForMember(d => d.IDUser, s => s.MapFrom(m => m.IdUser))
               .ForMember(d => d.Name, s => s.MapFrom(m => m.Name))
               .ForMember(d => d.Birthday, s => s.MapFrom(m => m.Birthday))
               .ForMember(d => d.Phone, s => s.MapFrom(m => m.Phone));
        }
    }
}
