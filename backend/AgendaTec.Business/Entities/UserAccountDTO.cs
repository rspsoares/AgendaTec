namespace AgendaTec.Business.Entities
{
    public class UserAccountDTO
    {
        public string Id { get; set; }        
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsEnabled { get; set; }
        public string IdRole { get; set; }       
        public string RoleDescription { get; set; }
        public int IDCustomer { get; set; }
        public string CustomerName { get; set; }
        public string InitialPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
