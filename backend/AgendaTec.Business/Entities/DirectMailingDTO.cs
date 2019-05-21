using System;

namespace AgendaTec.Business.Entities
{
    public class DirectMailingDTO
    {
        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public DateTime Start { get; set; }
        public DateTime Last { get; set; }
        public int Interval { get; set; }
        public bool Active { get; set; }
    }
}
