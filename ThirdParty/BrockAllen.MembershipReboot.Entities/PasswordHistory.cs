using System;
using System.ComponentModel.DataAnnotations;

namespace BrockAllen.MembershipReboot.Entities
{
    public class PasswordHistory
    {
        public int ID { get; set; }
        public Guid UserID { get; set; }
        public DateTime DateChanged { get; set; }
        [Required]
        public string PasswordHash { get; set; }
    }
}
