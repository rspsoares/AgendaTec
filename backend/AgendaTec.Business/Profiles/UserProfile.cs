using AgendaTec.Business.Entities;
using AgendaTec.Business.Helpers;
using AgendaTec.Infrastructure.DatabaseModel;
using AutoMapper;
using System;

namespace AgendaTec.Business.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AspNetUsers, UserAccountDTO>()
               .ForMember(d => d.Id, s => s.MapFrom(m => m.Id))
               .ForMember(d => d.UserName, s => s.MapFrom(m => m.UserName))
               .ForMember(d => d.FirstName, s => s.MapFrom(m => m.FirstName))
               .ForMember(d => d.LastName, s => s.MapFrom(m => m.LastName))
               .ForMember(d => d.FullName, s => s.MapFrom(m => $"{m.FirstName} {m.LastName}"))
               .ForMember(d => d.CPF, s => s.MapFrom(m => m.CPF))
               .ForMember(d => d.Birthday, s => s.MapFrom(m => m.BirthDate.HasValue ? m.BirthDate.Value.ToString("yyyy-MM-dd") : string.Empty))
               .ForMember(d => d.Email, s => s.MapFrom(m => m.Email))
               .ForMember(d => d.Phone, s => s.MapFrom(m => m.PhoneNumber))
               .ForMember(d => d.IsEnabled, s => s.MapFrom(m => m.IsEnabled))
               .ForMember(d => d.IdRole, s => s.MapFrom(m => m.IdRole))
               .ForMember(d => d.RoleDescription, s => s.MapFrom(m => m.AspNetRoles.Name))
               .ForMember(d => d.DirectMail, s => s.MapFrom(m => m.DirectMail))
               .ForMember(d => d.UserCustomers, s => s.MapFrom(m => m.TCGCustomersAspNetUsers));

            CreateMap<TCGCustomersAspNetUsers, UserCustomerDTO>()
              .ForMember(d => d.Id, s => s.MapFrom(m => m.IDCustomersAspNetUsers))
              .ForMember(d => d.IDAspNetUsers, s => s.MapFrom(m => m.AspNetUsers.Id))
              .ForMember(d => d.IDCustomer, s => s.MapFrom(m => m.TCGCustomers.IDCustomer));

            CreateMap<UserAccountDTO, AspNetUsers>()
               .ForMember(d => d.Id, s => s.MapFrom(m => m.Id))
               .ForMember(d => d.FirstName, s => s.MapFrom(m => m.FirstName))
               .ForMember(d => d.LastName, s => s.MapFrom(m => m.LastName))
               .ForMember(d => d.CPF, s => s.MapFrom(m => m.CPF.CleanMask()))
               .ForMember(d => d.BirthDate, s => s.MapFrom(m => DateTime.Parse(m.Birthday)))
               .ForMember(d => d.IdRole, s => s.MapFrom(m => m.IdRole))               
               .ForMember(d => d.Email, s => s.MapFrom(m => m.Email))
               .ForMember(d => d.PhoneNumber, s => s.MapFrom(m => m.Phone.CleanMask()))
               .ForMember(d => d.IsEnabled, s => s.MapFrom(m => m.IsEnabled))
               .ForMember(d => d.DirectMail, s => s.MapFrom(m => m.DirectMail));

            CreateMap<AspNetRoles, UserAccountDTO>()
               .ForMember(d => d.IdRole, s => s.MapFrom(m => m.Id))
               .ForMember(d => d.RoleDescription, s => s.MapFrom(m => m.Name));
        }
    }
}
