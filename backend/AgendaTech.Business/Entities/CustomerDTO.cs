using System;

namespace AgendaTech.Business.Entities
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Hire { get; set; }
        public bool Active { get; set; }
    }
}
