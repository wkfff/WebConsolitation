namespace Krista.FM.Client.Common.Forms
{
	partial class FormException
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormException));
            this.tbErrorText = new System.Windows.Forms.TextBox();
            this.tbFriendlyMessage = new System.Windows.Forms.TextBox();
            this.btnContinue = new Infragistics.Win.Misc.UltraButton();
            this.btnClose = new Infragistics.Win.Misc.UltraButton();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.btnDetails = new Infragistics.Win.Misc.UltraButton();
            this.btnReport = new Infragistics.Win.Misc.UltraButton();
            this.pbErrIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbErrIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // tbErrorText
            // 
            this.tbErrorText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbErrorText.Location = new System.Drawing.Point(14, 107);
            this.tbErrorText.Multiline = true;
            this.tbErrorText.Name = "tbErrorText";
            this.tbErrorText.ReadOnly = true;
            this.tbErrorText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbErrorText.Size = new System.Drawing.Size(604, 338);
            this.tbErrorText.TabIndex = 0;
            this.tbErrorText.Visible = false;
            // 
            // tbFriendlyMessage
            // 
            this.tbFriendlyMessage.BackColor = System.Drawing.SystemColors.Control;
            this.tbFriendlyMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbFriendlyMessage.Location = new System.Drawing.Point(68, 12);
            this.tbFriendlyMessage.Multiline = true;
            this.tbFriendlyMessage.Name = "tbFriendlyMessage";
            this.tbFriendlyMessage.ReadOnly = true;
            this.tbFriendlyMessage.Size = new System.Drawing.Size(550, 48);
            this.tbFriendlyMessage.TabIndex = 8;
            // 
            // btnContinue
            // 
            appearance1.Image = global::Krista.FM.Client.Common.Properties.Resources.ArrowContinue;
            this.btnContinue.Appearance = appearance1;
            this.btnContinue.ImageTransparentColor = System.Drawing.Color.Lime;
            this.btnContinue.Location = new System.Drawing.Point(251, 71);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(138, 23);
            this.btnContinue.TabIndex = 1;
            this.btnContinue.Text = "Продолжить работу";
            this.btnContinue.Click += new System.EventHandler(this.btnClick);
            // 
            // btnClose
            // 
            appearance2.Image = global::Krista.FM.Client.Common.Properties.Resources.Exit;
            this.btnClose.Appearance = appearance2;
            this.btnClose.ImageTransparentColor = System.Drawing.Color.Lime;
            this.btnClose.Location = new System.Drawing.Point(473, 71);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(143, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Закрыть приложение";
            this.btnClose.Click += new System.EventHandler(this.btnClick);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "txt";
            this.saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";
            this.saveFileDialog.RestoreDirectory = true;
            // 
            // btnDetails
            // 
            this.btnDetails.ImageTransparentColor = System.Drawing.Color.Lime;
            this.btnDetails.Location = new System.Drawing.Point(12, 71);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new System.Drawing.Size(125, 23);
            this.btnDetails.TabIndex = 0;
            this.btnDetails.Text = "Детали ошибки >>";
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
            // 
            // btnReport
            // 
            this.btnReport.Location = new System.Drawing.Point(392, 71);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(78, 23);
            this.btnReport.TabIndex = 9;
            this.btnReport.Text = "Отчет";
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // pbErrIcon
            // 
            this.pbErrIcon.Image = global::Krista.FM.Client.Common.Properties.Resources.Attention;
            this.pbErrIcon.Location = new System.Drawing.Point(12, 12);
            this.pbErrIcon.Name = "pbErrIcon";
            this.pbErrIcon.Size = new System.Drawing.Size(48, 48);
            this.pbErrIcon.TabIndex = 6;
            this.pbErrIcon.TabStop = false;
            // 
            // FormException
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 104);
            this.Controls.Add(this.btnReport);
            this.Controls.Add(this.btnDetails);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.tbFriendlyMessage);
            this.Controls.Add(this.pbErrIcon);
            this.Controls.Add(this.tbErrorText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 131);
            this.Name = "FormException";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Ошибка пользователя";
            ((System.ComponentModel.ISupportInitialize)(this.pbErrIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.TextBox tbErrorText;
        private System.Windows.Forms.PictureBox pbErrIcon;
        private System.Windows.Forms.TextBox tbFriendlyMessage;
        private Infragistics.Win.Misc.UltraButton btnContinue;
        private Infragistics.Win.Misc.UltraButton btnClose;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.Misc.UltraButton btnDetails;
        private Infragistics.Win.Misc.UltraButton btnReport;
	}
}