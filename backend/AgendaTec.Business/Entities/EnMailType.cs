using System.ComponentModel;

namespace AgendaTec.Business.Entities
{
    public enum EnMailType
    {
        [Description("Geral")]
        All = 0,

        [Description("E-Mail")]
        Email = 1,

        [Description("WhatsApp")]
        WhatsApp = 2
    }
}
