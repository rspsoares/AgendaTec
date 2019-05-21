namespace AgendaTec.Security.DeveloperTools
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btPasswordEncrypt = new System.Windows.Forms.Button();
            this.tbEncPasswordEncrypted = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbEncPlainPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btPasswordDecrypt = new System.Windows.Forms.Button();
            this.tbDecPlainPassword = new System.Windows.Forms.TextBox();
            this.tbDecPasswordEncrypted = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btPasswordEncrypt);
            this.groupBox1.Controls.Add(this.tbEncPasswordEncrypted);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbEncPlainPassword);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(476, 92);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Encrypt Password";
            // 
            // btPasswordEncrypt
            // 
            this.btPasswordEncrypt.Location = new System.Drawing.Point(395, 63);
            this.btPasswordEncrypt.Name = "btPasswordEncrypt";
            this.btPasswordEncrypt.Size = new System.Drawing.Size(75, 23);
            this.btPasswordEncrypt.TabIndex = 4;
            this.btPasswordEncrypt.Text = "Encrypt";
            this.btPasswordEncrypt.UseVisualStyleBackColor = true;
            this.btPasswordEncrypt.Click += new System.EventHandler(this.btPasswordEncrypt_Click);
            // 
            // tbEncPasswordEncrypted
            // 
            this.tbEncPasswordEncrypted.BackColor = System.Drawing.SystemColors.Control;
            this.tbEncPasswordEncrypted.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbEncPasswordEncrypted.Location = new System.Drawing.Point(235, 32);
            this.tbEncPasswordEncrypted.Name = "tbEncPasswordEncrypted";
            this.tbEncPasswordEncrypted.ReadOnly = true;
            this.tbEncPasswordEncrypted.Size = new System.Drawing.Size(235, 20);
            this.tbEncPasswordEncrypted.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(232, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Encrypted";
            // 
            // tbEncPlainPassword
            // 
            this.tbEncPlainPassword.Location = new System.Drawing.Point(6, 32);
            this.tbEncPlainPassword.Name = "tbEncPlainPassword";
            this.tbEncPlainPassword.Size = new System.Drawing.Size(223, 20);
            this.tbEncPlainPassword.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Plain";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btPasswordDecrypt);
            this.groupBox2.Controls.Add(this.tbDecPlainPassword);
            this.groupBox2.Controls.Add(this.tbDecPasswordEncrypted);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(3, 101);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(476, 91);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Decrypt Password";
            // 
            // btPasswordDecrypt
            // 
            this.btPasswordDecrypt.Location = new System.Drawing.Point(397, 58);
            this.btPasswordDecrypt.Name = "btPasswordDecrypt";
            this.btPasswordDecrypt.Size = new System.Drawing.Size(73, 23);
            this.btPasswordDecrypt.TabIndex = 4;
            this.btPasswordDecrypt.Text = "Decrypt";
            this.btPasswordDecrypt.UseVisualStyleBackColor = true;
            this.btPasswordDecrypt.Click += new System.EventHandler(this.btPasswordDecrypt_Click);
            // 
            // tbDecPlainPassword
            // 
            this.tbDecPlainPassword.Location = new System.Drawing.Point(235, 32);
            this.tbDecPlainPassword.Name = "tbDecPlainPassword";
            this.tbDecPlainPassword.ReadOnly = true;
            this.tbDecPlainPassword.Size = new System.Drawing.Size(235, 20);
            this.tbDecPlainPassword.TabIndex = 3;
            // 
            // tbDecPasswordEncrypted
            // 
            this.tbDecPasswordEncrypted.Location = new System.Drawing.Point(6, 32);
            this.tbDecPasswordEncrypted.Name = "tbDecPasswordEncrypted";
            this.tbDecPasswordEncrypted.Size = new System.Drawing.Size(223, 20);
            this.tbDecPasswordEncrypted.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(232, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Plain";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Encrypted";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 196);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Password Encrypt / Decrypt";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btPasswordEncrypt;
        private System.Windows.Forms.TextBox tbEncPasswordEncrypted;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbEncPlainPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btPasswordDecrypt;
        private System.Windows.Forms.TextBox tbDecPlainPassword;
        private System.Windows.Forms.TextBox tbDecPasswordEncrypted;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}

