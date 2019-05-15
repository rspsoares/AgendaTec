using AgendaTec.Business.Entities;
using AgendaTec.Infrastructure.DatabaseModel;
using AutoMapper;

namespace AgendaTec.Business.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<TCGCustomers, CustomerDTO>()
                .ForMember(d => d.Id, s => s.MapFrom(m => m.IDCustomer))
                .ForMember(d => d.Key, s => s.MapFrom(m => m.CustomerKey))
                .ForMember(d => d.Name, s => s.MapFrom(m => m.CompanyName))
                .ForMember(d => d.CNPJ, s => s.MapFrom(m => m.CNPJ))
                .ForMember(d => d.Address, s => s.MapFrom(m => m.Address))
                .ForMember(d => d.Phone, s => s.MapFrom(m => m.Phone))
                .ForMember(d => d.Hire, s => s.MapFrom(m => m.HireDate))
                .ForMember(d => d.Active, s => s.MapFrom(m => m.Active))
                .ForMember(d => d.Start, s => s.MapFrom(m => m.StartTime))
                .ForMember(d => d.End, s => s.MapFrom(m => m.EndTime))
                .ForMember(d => d.CPFRequired, s => s.MapFrom(m => m.CPFRequired))
                .ForMember(d => d.Note, s => s.MapFrom(m => m.Note));

            CreateMap<CustomerDTO, TCGCustomers>()
                .ForMember(d => d.IDCustomer, s => s.MapFrom(m => m.Id))
                .ForMember(d => d.CustomerKey, s => s.MapFrom(m => m.Key))
                .ForMember(d => d.CompanyName, s => s.MapFrom(m => m.Name))
                .ForMember(d => d.CNPJ, s => s.MapFrom(m => m.CNPJ))
                .ForMember(d => d.Address, s => s.MapFrom(m => m.Address))
                .ForMember(d => d.Phone, s => s.MapFrom(m => m.Phone))
                .ForMember(d => d.HireDate, s => s.MapFrom(m => m.Hire))
                .ForMember(d => d.Active, s => s.MapFrom(m => m.Active))
                .ForMember(d => d.StartTime, s => s.MapFrom(m => m.Start))
                .ForMember(d => d.EndTime, s => s.MapFrom(m => m.End))
                .ForMember(d => d.CPFRequired, s => s.MapFrom(m => m.CPFRequired))
                .ForMember(d => d.Note, s => s.MapFrom(m => m.Note));
        }
    }
}
