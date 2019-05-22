using System.ComponentModel;

namespace AgendaTec.Business.Entities
{
    public enum EnMailType
    {
        [Description("Mala Direta - Geral")]
        All = 0,

        [Description("Mala Direta - E-Mail")]
        Email = 1,

        [Description("Mala Direta - WhatsApp")]
        WhatsApp = 2
    }
}
