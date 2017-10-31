namespace Krista.FM.Client.ViewObjects.ForecastUI.Gui.Form2P
{
	partial class frmSaveForm2p
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnSave = new System.Windows.Forms.Button();
			this.panel2 = new System.Windows.Forms.Panel();
			this.btnSelect2 = new System.Windows.Forms.Button();
			this.btnSelect1 = new System.Windows.Forms.Button();
			this.tbVar2 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tbVar1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnCancel);
			this.panel1.Controls.Add(this.btnSave);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 136);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(410, 44);
			this.panel1.TabIndex = 0;
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(207, 12);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 20);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Отмена";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(288, 12);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(111, 21);
			this.btnSave.TabIndex = 0;
			this.btnSave.Text = "Сохранить";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.btnSelect2);
			this.panel2.Controls.Add(this.btnSelect1);
			this.panel2.Controls.Add(this.tbVar2);
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this.tbVar1);
			this.panel2.Controls.Add(this.label1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(410, 136);
			this.panel2.TabIndex = 1;
			// 
			// btnSelect2
			// 
			this.btnSelect2.Location = new System.Drawing.Point(338, 91);
			this.btnSelect2.Name = "btnSelect2";
			this.btnSelect2.Size = new System.Drawing.Size(60, 23);
			this.btnSelect2.TabIndex = 2;
			this.btnSelect2.Text = "Выбрать";
			this.btnSelect2.UseVisualStyleBackColor = true;
			this.btnSelect2.Click += new System.EventHandler(this.btnSelect2_Click);
			// 
			// btnSelect1
			// 
			this.btnSelect1.Location = new System.Drawing.Point(338, 34);
			this.btnSelect1.Name = "btnSelect1";
			this.btnSelect1.Size = new System.Drawing.Size(60, 23);
			this.btnSelect1.TabIndex = 2;
			this.btnSelect1.Text = "Выбрать";
			this.btnSelect1.UseVisualStyleBackColor = true;
			this.btnSelect1.Click += new System.EventHandler(this.btnSelect1_Click);
			// 
			// tbVar2
			// 
			this.tbVar2.Location = new System.Drawing.Point(15, 94);
			this.tbVar2.Name = "tbVar2";
			this.tbVar2.Size = new System.Drawing.Size(317, 20);
			this.tbVar2.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 78);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(175, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Форма 2П для второго варианта";
			// 
			// tbVar1
			// 
			this.tbVar1.Location = new System.Drawing.Point(15, 37);
			this.tbVar1.Name = "tbVar1";
			this.tbVar1.Size = new System.Drawing.Size(317, 20);
			this.tbVar1.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(176, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Форма 2П для первого варианта";
			// 
			// frmSaveForm2p
			// 
			this.AcceptButton = this.btnSave;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(410, 180);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MinimizeBox = false;
			this.Name = "frmSaveForm2p";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Сохранение Формы 2П в формате МЭР";
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button btnSelect2;
		private System.Windows.Forms.Button btnSelect1;
		private System.Windows.Forms.TextBox tbVar2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbVar1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnCancel;
	}
}