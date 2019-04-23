using System.Collections.Generic;

namespace BrockAllen.MembershipReboot.Entities
{
    public class CustomEmailMessageFormatter : EmailMessageFormatter<CustomUserAccount>
    {
        public CustomEmailMessageFormatter(ApplicationInformation info)
            : base(info)
        {
        }

        protected override Tokenizer GetTokenizer(UserAccountEvent<CustomUserAccount> evt)
        {
            return new Tokenizer(evt.Account.StartingPass);
        }

        protected override string LoadBodyTemplate(UserAccountEvent<CustomUserAccount> evt)
        {
            if (evt is AccountCreatedEvent<CustomUserAccount> && !string.IsNullOrEmpty(evt.Account.StartingPass))
            {
                return LoadTemplate("AccountCreatedEvent_Custom_Body");
            }
            else
            {
                return LoadTemplate(CleanGenericName(evt.GetType()) + "_Body");
            }
        }

        protected override string GetBody(UserAccountEvent<CustomUserAccount> evt, IDictionary<string, string> values)
        {
            //if (evt is EmailVerifiedEvent<CustomUserAccount>)
            //{
            //    return "sua conta foi verificado pelo " + this.ApplicationInformation.ApplicationName + ". você pode agora utilizar sem problemas.";
            //}

            //if (evt is AccountClosedEvent<CustomUserAccount>)
            //{
            //    return FormatValue(evt, "sua conta foi emcerrada pelo {applicationName}. ", values);
            //}

            return base.GetBody(evt, values);
        }
    }
}
