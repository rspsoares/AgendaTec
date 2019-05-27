using System;

namespace AgendaTec.Business.Entities
{
    public class ProfessionalDTO
    {
        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public string IdUser { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Phone { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
    }
}
