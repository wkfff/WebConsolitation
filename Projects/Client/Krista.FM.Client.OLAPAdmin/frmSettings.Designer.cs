namespace Krista.FM.Client.OLAPAdmin
{
	partial class frmSettings
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
			this.pnlSettings = new System.Windows.Forms.Panel();
			this.pnlButtons = new System.Windows.Forms.Panel();
			this.propGridSettings = new System.Windows.Forms.PropertyGrid();
			this.pnlSettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlSettings
			// 
			this.pnlSettings.Controls.Add(this.propGridSettings);
			this.pnlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlSettings.Location = new System.Drawing.Point(0, 0);
			this.pnlSettings.Name = "pnlSettings";
			this.pnlSettings.Size = new System.Drawing.Size(632, 451);
			this.pnlSettings.TabIndex = 0;
			// 
			// pnlButtons
			// 
			this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlButtons.Location = new System.Drawing.Point(0, 398);
			this.pnlButtons.Name = "pnlButtons";
			this.pnlButtons.Size = new System.Drawing.Size(632, 53);
			this.pnlButtons.TabIndex = 1;
			// 
			// propGridSettings
			// 
			this.propGridSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propGridSettings.Location = new System.Drawing.Point(0, 0);
			this.propGridSettings.Name = "propGridSettings";
			this.propGridSettings.Size = new System.Drawing.Size(632, 451);
			this.propGridSettings.TabIndex = 0;
			// 
			// frmSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(632, 451);
			this.Controls.Add(this.pnlButtons);
			this.Controls.Add(this.pnlSettings);
			this.Name = "frmSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Настройки";
			this.pnlSettings.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel pnlSettings;
		private System.Windows.Forms.Panel pnlButtons;
		public System.Windows.Forms.PropertyGrid propGridSettings;
	}
}