using System;

namespace AgendaTech.Business.Entities
{
    public class UserAccountDTO
    {
        public int IDUser { get; set; }
        public Guid UkUser { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public int IDUserGroup { get; set; }       
        public string GroupDescription { get; set; }
        public int IDCustomer { get; set; }
        public string CustomerName { get; set; }
        public string InitialPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
