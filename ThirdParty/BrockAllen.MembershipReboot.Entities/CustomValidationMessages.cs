namespace BrockAllen.MembershipReboot.Entities
{
    public class CustomValidationMessages : ICommandHandler<GetValidationMessage>
    {
        public void Handle(GetValidationMessage cmd)
        {
            if (cmd.ID == MembershipRebootConstants.ValidationMessages.UsernameRequired)
            {
                cmd.Message = "usuário requerido!";
            }
        }
    }
}
