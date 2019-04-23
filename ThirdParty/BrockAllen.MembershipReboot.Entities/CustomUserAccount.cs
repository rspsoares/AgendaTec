using BrockAllen.MembershipReboot.Relational;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrockAllen.MembershipReboot.Entities
{
    public class CustomUserAccount : RelationalUserAccount
    {
        public virtual int Source { get; set; }
        public virtual int Inscription { get; set; }
        public virtual string StartingPass { get; set; }

        [NotMapped]
        public string OtherFirstName
        {
            get
            {
                return this.GetClaimValue(ClaimTypes.GivenName);
            }
        }
    }
}
