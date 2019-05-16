using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AgendaTec.Client.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CPF { get; set; }
        public string IDRole { get; set; }
        public int IDCustomer { get; set; }
        public bool IsEnabled { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            userIdentity.AddClaim(new Claim("IDUser", Id));
            userIdentity.AddClaim(new Claim("FirstName", FirstName));
            userIdentity.AddClaim(new Claim("FullName", $"{FirstName} {LastName}"));
            userIdentity.AddClaim(new Claim("IDRole", IDRole));
            userIdentity.AddClaim(new Claim("IDCustomer", IDCustomer.ToString()));

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