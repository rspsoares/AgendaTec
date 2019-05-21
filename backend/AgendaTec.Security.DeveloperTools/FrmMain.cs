using AgendaTec.Business.Helpers;
using System;
using System.Windows.Forms;

namespace AgendaTec.Security.DeveloperTools
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btPasswordEncrypt_Click(object sender, EventArgs e)
        {
            var byteEncrypted = SecurityHelper.Encrypt(tbEncPlainPassword.Text.ToSecureString());
            tbEncPasswordEncrypted.Text = Convert.ToBase64String(byteEncrypted);
        }

        private void btPasswordDecrypt_Click(object sender, EventArgs e)
        {
            tbDecPlainPassword.Text = SecurityHelper.Decrypt(Convert.FromBase64String(tbDecPasswordEncrypted.Text)).ToUnsecureString();
        }
    }
}
