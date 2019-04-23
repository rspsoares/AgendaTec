using System.Security.Claims;

namespace BrockAllen.MembershipReboot.Entities
{
    public class CustomClaimsMapper : ICommandHandler<MapClaimsFromAccount<CustomUserAccount>>
    {
        public void Handle(MapClaimsFromAccount<CustomUserAccount> cmd)
        {
            cmd.MappedClaims = new System.Security.Claims.Claim[]
            {
                new Claim(ClaimTypes.Actor, cmd.Account.FirstName + " " + cmd.Account.LastName),
                new Claim(ClaimTypes.GivenName, cmd.Account.FirstName),
                new Claim(ClaimTypes.Surname, cmd.Account.LastName),
                new Claim(ClaimTypes.GroupSid, cmd.Account.Inscription.ToString()),
                new Claim(ClaimTypes.PrimaryGroupSid, cmd.Account.Source.ToString()),
            };
        }
    }
}
