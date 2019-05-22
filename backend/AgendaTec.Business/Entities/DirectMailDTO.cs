using System;

namespace AgendaTec.Business.Entities
{
    public class DirectMailDTO
    {
        public int Id { get; set; }
        public int IdCustomer { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }        
        public DateTime? Last { get; set; }
        public string Interval { get; set; }
        public int IntervalType { get; set; }
        public bool Resend { get; set; }
    }
}
