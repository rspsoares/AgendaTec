namespace BrockAllen.MembershipReboot.Entities
{
    public class NotifyAccountOwnerWhenTooManyFailedLoginAttempts
      : IEventHandler<TooManyRecentPasswordFailuresEvent<CustomUserAccount>>
    {
        public void Handle(TooManyRecentPasswordFailuresEvent<CustomUserAccount> evt)
        {
            var smtp = new SmtpMessageDelivery();
            var msg = new Message
            {
                To = evt.Account.Email,
                Subject = "Sua conta no AgendaTech",
                Body = "Aparentemente você (ou alguém) tentou entrar em sua conta e errou a senha muitas vezes, sua conta está neste momento bloqueada. "
            };
            smtp.Send(msg);
        }
    }
}
