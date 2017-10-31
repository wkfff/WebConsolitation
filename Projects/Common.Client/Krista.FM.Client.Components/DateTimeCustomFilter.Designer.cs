using Infragistics.Win.Misc;

namespace Krista.FM.Client.Components
{
    partial class DateTimeCustomFilter
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
            this.udteEndDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.udteStartDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new UltraLabel();
            this.label2 = new UltraLabel();
            this.label4 = new UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.udteEndDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udteStartDate)).BeginInit();
            this.SuspendLayout();
            // 
            // udteEndDate
            // 
            this.udteEndDate.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.VisualStudio2005;
            this.udteEndDate.Location = new System.Drawing.Point(234, 23);
            this.udteEndDate.MaskInput = "{date} {time}";
            this.udteEndDate.Name = "udteEndDate";
            this.udteEndDate.Size = new System.Drawing.Size(120, 21);
            this.udteEndDate.TabIndex = 0;
            this.udteEndDate.ValueChanged += new System.EventHandler(this.udteEndDate_ValueChanged);
            // 
            // udteStartDate
            // 
            this.udteStartDate.DisplayStyle = Infragistics.Win.EmbeddableElementDisplayStyle.VisualStudio2005;
            this.udteStartDate.Location = new System.Drawing.Point(83, 23);
            this.udteStartDate.MaskInput = "{date} {time}";
            this.udteStartDate.Name = "udteStartDate";
            this.udteStartDate.Size = new System.Drawing.Size(120, 21);
            this.udteStartDate.TabIndex = 1;
            this.udteStartDate.ValueChanged += new System.EventHandler(this.udteStartDate_ValueChanged);
            // 
            // btnApply
            // 
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Location = new System.Drawing.Point(198, 84);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 9;
            this.btnApply.Text = "OK";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(279, 84);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Отмена";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.Location = new System.Drawing.Point(209, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "по";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Период";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(64, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "с";
            // 
            // DateTimeCustomFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 119);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.udteStartDate);
            this.Controls.Add(this.udteEndDate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DateTimeCustomFilter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DateTimeCustomFilter";
            ((System.ComponentModel.ISupportInitialize)(this.udteEndDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udteStartDate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor udteEndDate;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor udteStartDate;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private UltraLabel label1;
        private UltraLabel label2;
        private UltraLabel label4;
    }
}