namespace Krista.FM.Client.OLAPAdmin
{
	partial class frmGenerateNew
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
			this.propGridDWSettings = new System.Windows.Forms.PropertyGrid();
			this.pnlButtons = new System.Windows.Forms.Panel();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.pnlButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// propGridDWSettings
			// 
			this.propGridDWSettings.Dock = System.Windows.Forms.DockStyle.Top;
			this.propGridDWSettings.Location = new System.Drawing.Point(0, 0);
			this.propGridDWSettings.Name = "propGridDWSettings";
			this.propGridDWSettings.Size = new System.Drawing.Size(632, 404);
			this.propGridDWSettings.TabIndex = 0;
			// 
			// pnlButtons
			// 
			this.pnlButtons.Controls.Add(this.btnCancel);
			this.pnlButtons.Controls.Add(this.btnOK);
			this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlButtons.Location = new System.Drawing.Point(0, 404);
			this.pnlButtons.Name = "pnlButtons";
			this.pnlButtons.Size = new System.Drawing.Size(632, 47);
			this.pnlButtons.TabIndex = 1;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(522, 3);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(107, 41);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Отмена";
			this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(409, 3);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(107, 41);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "ОК";
			this.btnOK.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// frmGenerateNew
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(632, 451);
			this.Controls.Add(this.pnlButtons);
			this.Controls.Add(this.propGridDWSettings);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmGenerateNew";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Установите парметры генерация новых объектов многомерной базы";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGenerateNew_FormClosing);
			this.pnlButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.PropertyGrid propGridDWSettings;
		private System.Windows.Forms.Panel pnlButtons;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;


	}
}