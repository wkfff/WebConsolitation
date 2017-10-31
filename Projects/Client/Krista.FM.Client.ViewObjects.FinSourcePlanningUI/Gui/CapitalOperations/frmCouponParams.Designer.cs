namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CapitalPlanningOperations
{
    partial class frmCouponParams
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
            this.ne1 = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ne2 = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ne1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ne2)).BeginInit();
            this.SuspendLayout();
            // 
            // ne1
            // 
            this.ne1.Location = new System.Drawing.Point(196, 39);
            this.ne1.MaskInput = "nnn.nn";
            this.ne1.Name = "ne1";
            this.ne1.NumericType = Infragistics.Win.UltraWinEditors.NumericType.Decimal;
            this.ne1.Size = new System.Drawing.Size(84, 21);
            this.ne1.TabIndex = 5;
            // 
            // ne2
            // 
            this.ne2.Location = new System.Drawing.Point(196, 12);
            this.ne2.Name = "ne2";
            this.ne2.PromptChar = ' ';
            this.ne2.Size = new System.Drawing.Size(84, 21);
            this.ne2.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(168, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Количество купонных периодов";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Ставка купонного дохода";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(205, 75);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(124, 75);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // frmCouponParams
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 107);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ne2);
            this.Controls.Add(this.ne1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCouponParams";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Купоны";
            ((System.ComponentModel.ISupportInitialize)(this.ne1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ne2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraNumericEditor ne1;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor ne2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
    }
}