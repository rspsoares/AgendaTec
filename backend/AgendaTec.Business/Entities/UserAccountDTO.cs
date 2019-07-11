using System.Collections.Generic;

namespace AgendaTec.Business.Entities
{
    public class UserAccountDTO
    {
        public string Id { get; set; }        
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string CPF { get; set; }
        public string Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsEnabled { get; set; }
        public string IdRole { get; set; }       
        public string RoleDescription { get; set; }        
        public string InitialPassword { get; set; }
        public string NewPassword { get; set; }
        public bool RootUser { get; set; }
        public bool DirectMail { get; set; }
        public List<UserCustomerDTO> UserCustomers { get; set; }
    }

    public class UserCustomerDTO
    {
        public int Id { get; set; }
        public string IDAspNetUsers { get; set; }
        public int IDCustomer { get; set; }
    }
}
