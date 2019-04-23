using System;

namespace BrockAllen.MembershipReboot.Entities
{
    public class PasswordChanged :
        IEventHandler<AccountCreatedEvent<CustomUserAccount>>,
        IEventHandler<PasswordChangedEvent<CustomUserAccount>>
    {
        public void Handle(AccountCreatedEvent<CustomUserAccount> evt)
        {
            if (evt.InitialPassword != null)
            {
                AddPasswordHistoryEntry(evt.Account.ID, evt.InitialPassword);
            }
        }

        public void Handle(PasswordChangedEvent<CustomUserAccount> evt)
        {
            AddPasswordHistoryEntry(evt.Account.ID, evt.NewPassword);
        }

        private static void AddPasswordHistoryEntry(Guid accountID, string password)
        {
            using (var db = new CustomDatabase())
            {
                var pw = new PasswordHistory
                {
                    UserID = accountID,
                    DateChanged = DateTime.UtcNow,
                    PasswordHash = new DefaultCrypto().HashPassword(password, 1000)
                };
                db.PasswordHistory.Add(pw);
                db.SaveChanges();
            }
        }
    }
}
