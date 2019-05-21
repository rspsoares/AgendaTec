using System.Threading;

namespace AgendaTec.Service
{
    partial class AgendaTecService
    {
        Timer sendMailTimer;
        bool sendMailLock = false;

        Timer cleanUpTimer;
        bool cleanUpLock = false;

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.ServiceName = "AgendaTecService";
        }

        #endregion
    }
}
