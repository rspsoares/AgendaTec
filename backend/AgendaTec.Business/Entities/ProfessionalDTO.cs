using System;
using System.Collections.Generic;

namespace AgendaTec.Business.Entities
{
    public class ProfessionalDTO
    {
        public ProfessionalDTO()
        {
            Services = new List<ProfessionalServiceDTO>();
        }

        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public string IdUser { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Phone { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public List<ProfessionalServiceDTO> Services { get; set; }
    }

    public class ProfessionalServiceDTO
    {
        public Guid Id { get; set; }        
        public int IdService { get; set; }
        public string Description { get; set; }
        public int Time { get; set; }
        public decimal Price { get; set; }
    }
}
