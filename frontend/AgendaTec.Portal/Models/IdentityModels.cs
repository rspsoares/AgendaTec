using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AgendaTec.Portal.Models
{    
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CPF { get; set; }
        public string IDRole { get; set; }
        public int IDCustomer { get; set; }
        public bool IsEnabled { get; set; }
        public bool RootUser { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        { 
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
         
            userIdentity.AddClaim(new Claim("FirstName", FirstName));
            userIdentity.AddClaim(new Claim("FullName", $"{FirstName} {LastName}"));
            userIdentity.AddClaim(new Claim("IDRole", IDRole));
            userIdentity.AddClaim(new Claim("IDCustomer", IDCustomer.ToString()));
            userIdentity.AddClaim(new Claim("RootUser", RootUser ? "1" : "0"));

            return userIdentity;
        }        
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}