using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AgendaTec.Business.Bindings;
using AgendaTec.Business.Contracts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AgendaTec.Portal.Models
{    
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CPF { get; set; }
        public DateTime? BirthDate { get; set; }
        public string IDRole { get; set; }    
        public bool IsEnabled { get; set; }
        public bool RootUser { get; set; }
        public bool DirectMail { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            IUserFacade userFacade = new UserFacade();

            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
         
            userIdentity.AddClaim(new Claim("FirstName", FirstName));
            userIdentity.AddClaim(new Claim("FullName", $"{FirstName} {LastName}"));
            userIdentity.AddClaim(new Claim("IDRole", IDRole));      
            userIdentity.AddClaim(new Claim("RootUser", RootUser ? "1" : "0"));

            var user = userFacade.GetUserById(userIdentity.GetUserId(), out string errorMessage);
            userIdentity.AddClaim(new Claim("IDCustomer", user.UserCustomers.First().IDCustomer.ToString()));
            
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