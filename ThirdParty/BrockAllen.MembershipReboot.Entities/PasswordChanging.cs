using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace BrockAllen.MembershipReboot.Entities
{
    public class PasswordChanging : IEventHandler<PasswordChangedEvent<CustomUserAccount>>
    {
        public void Handle(PasswordChangedEvent<CustomUserAccount> evt)
        {
            using (var db = new CustomDatabase())
            {
                var oldEntires =
                    db.PasswordHistory.Where(x => x.UserID == evt.Account.ID).OrderByDescending(x => x.DateChanged).ToArray();
                for (var i = 0; i < 3 && oldEntires.Length > i; i++)
                {
                    var oldHash = oldEntires[i].PasswordHash;
                    if (new DefaultCrypto().VerifyHashedPassword(oldHash, evt.NewPassword))
                    {
                        throw new ValidationException("A nova senha não pode ser a igual a nenhum das 3 últimas utilizadas.");
                    }
                }
            }
        }
    }
}
