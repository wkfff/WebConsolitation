using Infragistics.Win.Misc;

namespace Krista.FM.Client.Workplace
{
    partial class FormChangePassword
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
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChangePassword));
            this.edPassword = new System.Windows.Forms.TextBox();
            this.edLogin = new System.Windows.Forms.TextBox();
            this.laPassword = new Infragistics.Win.Misc.UltraLabel();
            this.laLogin = new Infragistics.Win.Misc.UltraLabel();
            this.edNewPassword2 = new System.Windows.Forms.TextBox();
            this.laConfirmPassword = new Infragistics.Win.Misc.UltraLabel();
            this.edNewPassword1 = new System.Windows.Forms.TextBox();
            this.laNewPassword = new Infragistics.Win.Misc.UltraLabel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // edPassword
            // 
            this.edPassword.Location = new System.Drawing.Point(135, 38);
            this.edPassword.Name = "edPassword";
            this.edPassword.Size = new System.Drawing.Size(200, 20);
            this.edPassword.TabIndex = 2;
            // 
            // edLogin
            // 
            this.edLogin.Enabled = false;
            this.edLogin.Location = new System.Drawing.Point(135, 12);
            this.edLogin.Name = "edLogin";
            this.edLogin.Size = new System.Drawing.Size(200, 20);
            this.edLogin.TabIndex = 1;
            // 
            // laPassword
            // 
            appearance2.TextHAlignAsString = "Right";
            this.laPassword.Appearance = appearance2;
            this.laPassword.Location = new System.Drawing.Point(49, 40);
            this.laPassword.Name = "laPassword";
            this.laPassword.Size = new System.Drawing.Size(80, 20);
            this.laPassword.TabIndex = 11;
            this.laPassword.Text = "Пароль";
            // 
            // laLogin
            // 
            appearance3.TextHAlignAsString = "Right";
            this.laLogin.Appearance = appearance3;
            this.laLogin.Location = new System.Drawing.Point(81, 14);
            this.laLogin.Name = "laLogin";
            this.laLogin.Size = new System.Drawing.Size(48, 20);
            this.laLogin.TabIndex = 10;
            this.laLogin.Text = "Логин";
            // 
            // edNewPassword2
            // 
            this.edNewPassword2.Location = new System.Drawing.Point(135, 90);
            this.edNewPassword2.Name = "edNewPassword2";
            this.edNewPassword2.PasswordChar = '*';
            this.edNewPassword2.Size = new System.Drawing.Size(200, 20);
            this.edNewPassword2.TabIndex = 4;
            // 
            // laConfirmPassword
            // 
            appearance4.TextHAlignAsString = "Right";
            this.laConfirmPassword.Appearance = appearance4;
            this.laConfirmPassword.Location = new System.Drawing.Point(18, 92);
            this.laConfirmPassword.Name = "laConfirmPassword";
            this.laConfirmPassword.Size = new System.Drawing.Size(111, 20);
            this.laConfirmPassword.TabIndex = 14;
            this.laConfirmPassword.Text = "Подтверждение";
            // 
            // edNewPassword1
            // 
            this.edNewPassword1.Location = new System.Drawing.Point(135, 64);
            this.edNewPassword1.Name = "edNewPassword1";
            this.edNewPassword1.PasswordChar = '*';
            this.edNewPassword1.Size = new System.Drawing.Size(200, 20);
            this.edNewPassword1.TabIndex = 3;
            // 
            // laNewPassword
            // 
            appearance1.TextHAlignAsString = "Right";
            this.laNewPassword.Appearance = appearance1;
            this.laNewPassword.Location = new System.Drawing.Point(18, 66);
            this.laNewPassword.Name = "laNewPassword";
            this.laNewPassword.Size = new System.Drawing.Size(111, 20);
            this.laNewPassword.TabIndex = 16;
            this.laNewPassword.Text = "Новый пароль";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(179, 119);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "Установить";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(260, 119);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Отмена";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(11, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 20;
            this.pictureBox1.TabStop = false;
            // 
            // FormChangePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 153);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.edNewPassword1);
            this.Controls.Add(this.laNewPassword);
            this.Controls.Add(this.edNewPassword2);
            this.Controls.Add(this.laConfirmPassword);
            this.Controls.Add(this.edPassword);
            this.Controls.Add(this.edLogin);
            this.Controls.Add(this.laPassword);
            this.Controls.Add(this.laLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormChangePassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Изменение пароля";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormChangePassword_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox edPassword;
        private System.Windows.Forms.TextBox edLogin;
        private UltraLabel laPassword;
        private UltraLabel laLogin;
        private System.Windows.Forms.TextBox edNewPassword2;
        private UltraLabel laConfirmPassword;
        private System.Windows.Forms.TextBox edNewPassword1;
        private UltraLabel laNewPassword;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}