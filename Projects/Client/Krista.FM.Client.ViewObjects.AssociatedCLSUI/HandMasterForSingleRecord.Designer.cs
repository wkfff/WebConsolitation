using Infragistics.Win.Misc;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls
{
	partial class HandMasterForSingleRecord
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HandMasterForSingleRecord));
            this.panel6 = new System.Windows.Forms.Panel();
            this.lCaption = new Infragistics.Win.Misc.UltraLabel();
            this.label4 = new Infragistics.Win.Misc.UltraLabel();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbAddToTraslTable = new System.Windows.Forms.CheckBox();
            this.cbApplyToAllRecords = new System.Windows.Forms.CheckBox();
            this.lbAssociationRules = new System.Windows.Forms.ListBox();
            this.label19 = new Infragistics.Win.Misc.UltraLabel();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.SystemColors.Window;
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.lCaption);
            this.panel6.Controls.Add(this.label4);
            this.panel6.Controls.Add(this.pictureBox5);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(558, 94);
            this.panel6.TabIndex = 15;
            this.panel6.Paint += new System.Windows.Forms.PaintEventHandler(this.panel6_Paint);
            // 
            // lCaption
            // 
            this.lCaption.Location = new System.Drawing.Point(39, 29);
            this.lCaption.Name = "lCaption";
            this.lCaption.Size = new System.Drawing.Size(393, 59);
            this.lCaption.TabIndex = 3;
            this.lCaption.Text = "Выберите параметры для ручного сопоставления";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(13, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(222, 14);
            this.label4.TabIndex = 2;
            this.label4.Text = "Параметры для ручного сопоставления";
            // 
            // pictureBox5
            // 
            this.pictureBox5.Dock = System.Windows.Forms.DockStyle.Right;
            this.pictureBox5.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox5.Image")));
            this.pictureBox5.Location = new System.Drawing.Point(438, 0);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(118, 92);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox5.TabIndex = 1;
            this.pictureBox5.TabStop = false;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.panel1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 381);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(558, 55);
            this.panel4.TabIndex = 14;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnApply);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(77, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(479, 53);
            this.panel1.TabIndex = 0;
            // 
            // btnApply
            // 
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Location = new System.Drawing.Point(304, 19);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(83, 23);
            this.btnApply.TabIndex = 7;
            this.btnApply.Text = "Сопоставить";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(393, 19);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbAddToTraslTable
            // 
            this.cbAddToTraslTable.AutoSize = true;
            this.cbAddToTraslTable.Checked = true;
            this.cbAddToTraslTable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAddToTraslTable.Location = new System.Drawing.Point(41, 145);
            this.cbAddToTraslTable.Name = "cbAddToTraslTable";
            this.cbAddToTraslTable.Size = new System.Drawing.Size(209, 17);
            this.cbAddToTraslTable.TabIndex = 16;
            this.cbAddToTraslTable.Text = "Добавить в таблицу перекодировки";
            // 
            // cbApplyToAllRecords
            // 
            this.cbApplyToAllRecords.AutoSize = true;
            this.cbApplyToAllRecords.Checked = true;
            this.cbApplyToAllRecords.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbApplyToAllRecords.Location = new System.Drawing.Point(41, 203);
            this.cbApplyToAllRecords.Name = "cbApplyToAllRecords";
            this.cbApplyToAllRecords.Size = new System.Drawing.Size(295, 17);
            this.cbApplyToAllRecords.TabIndex = 17;
            this.cbApplyToAllRecords.Text = "Применить к подобным записям во всех источниках";
            // 
            // lbAssociationRules
            // 
            this.lbAssociationRules.FormattingEnabled = true;
            this.lbAssociationRules.Location = new System.Drawing.Point(41, 268);
            this.lbAssociationRules.Name = "lbAssociationRules";
            this.lbAssociationRules.Size = new System.Drawing.Size(273, 56);
            this.lbAssociationRules.TabIndex = 18;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(49, 252);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(132, 14);
            this.label19.TabIndex = 19;
            this.label19.Text = "Правила сопоставления";
            // 
            // HandMasterForSingleRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 436);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.lbAssociationRules);
            this.Controls.Add(this.cbApplyToAllRecords);
            this.Controls.Add(this.cbAddToTraslTable);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel4);
            this.Name = "HandMasterForSingleRecord";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Мастер ручного сопоставления";
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panel6;
        private UltraLabel lCaption;
        private UltraLabel label4;
		private System.Windows.Forms.PictureBox pictureBox5;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.CheckBox cbAddToTraslTable;
		private System.Windows.Forms.CheckBox cbApplyToAllRecords;
        private UltraLabel label19;
		internal System.Windows.Forms.ListBox lbAssociationRules;
	}
}