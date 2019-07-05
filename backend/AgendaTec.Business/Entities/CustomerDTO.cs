using System;
using System.Collections.Generic;

namespace AgendaTec.Business.Entities
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string CNPJ { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime Hire { get; set; }        
        public bool Active { get; set; }
        public bool Root { get; set; }
        public bool CPFRequired { get; set; }
        public string Note { get; set; }
        public bool ShowPrice { get; set; }
        public List<CustomerTimeRangeDTO> TimeRanges { get; set; }
    }

    public class CustomerTimeRangeDTO
    {
        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
