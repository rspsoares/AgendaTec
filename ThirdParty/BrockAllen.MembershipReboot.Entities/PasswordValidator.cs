using System.ComponentModel.DataAnnotations;

namespace BrockAllen.MembershipReboot.Entities
{
    public class PasswordValidator : IValidator<CustomUserAccount>
    {
        public ValidationResult Validate(UserAccountService<CustomUserAccount> service, CustomUserAccount account, string value)
        {
            if (value.Contains(account.Username))
            {
                return new ValidationResult("Por favor não utilize seu usuário em sua senha.");
            }

            if (value.Contains(account.Email))
            {
                return new ValidationResult("Por favor não utilize seu e-mail em sua senha");
            }

            return null;
        }
    }
}
