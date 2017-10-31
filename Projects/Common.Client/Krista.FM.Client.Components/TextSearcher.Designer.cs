namespace Krista.FM.Client.Components
{
    partial class TextSearcher
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ultraButton1 = new Infragistics.Win.Misc.UltraButton();
            this.ultraButton2 = new Infragistics.Win.Misc.UltraButton();
            this.chkMatchCase = new System.Windows.Forms.CheckBox();
            this.lblSearchDirection = new System.Windows.Forms.Label();
            this.lblLookIn = new System.Windows.Forms.Label();
            this.lblFindWhat = new System.Windows.Forms.Label();
            this.cboFindWhat = new System.Windows.Forms.ComboBox();
            this.clbSearchColumns = new System.Windows.Forms.CheckedListBox();
            this.cboSearchDirection = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // ultraButton1
            // 
            this.ultraButton1.Location = new System.Drawing.Point(263, 172);
            this.ultraButton1.Name = "ultraButton1";
            this.ultraButton1.Size = new System.Drawing.Size(75, 23);
            this.ultraButton1.TabIndex = 41;
            this.ultraButton1.Text = "Отмена";
            this.ultraButton1.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // ultraButton2
            // 
            this.ultraButton2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ultraButton2.Location = new System.Drawing.Point(182, 172);
            this.ultraButton2.Name = "ultraButton2";
            this.ultraButton2.Size = new System.Drawing.Size(75, 23);
            this.ultraButton2.TabIndex = 40;
            this.ultraButton2.Text = "Далее";
            this.ultraButton2.UseHotTracking = Infragistics.Win.DefaultableBoolean.True;
            this.ultraButton2.Click += new System.EventHandler(this.cmdFindNext_Click);
            // 
            // chkMatchCase
            // 
            this.chkMatchCase.Location = new System.Drawing.Point(16, 171);
            this.chkMatchCase.Name = "chkMatchCase";
            this.chkMatchCase.Size = new System.Drawing.Size(129, 24);
            this.chkMatchCase.TabIndex = 39;
            this.chkMatchCase.Text = "С учетом регистра";
            // 
            // lblSearchDirection
            // 
            this.lblSearchDirection.AutoSize = true;
            this.lblSearchDirection.Location = new System.Drawing.Point(13, 138);
            this.lblSearchDirection.Name = "lblSearchDirection";
            this.lblSearchDirection.Size = new System.Drawing.Size(78, 13);
            this.lblSearchDirection.TabIndex = 38;
            this.lblSearchDirection.Text = "Направление:";
            // 
            // lblLookIn
            // 
            this.lblLookIn.Location = new System.Drawing.Point(13, 74);
            this.lblLookIn.Name = "lblLookIn";
            this.lblLookIn.Size = new System.Drawing.Size(66, 32);
            this.lblLookIn.TabIndex = 37;
            this.lblLookIn.Text = "Искать в строке:";
            // 
            // lblFindWhat
            // 
            this.lblFindWhat.AutoSize = true;
            this.lblFindWhat.Location = new System.Drawing.Point(13, 32);
            this.lblFindWhat.Name = "lblFindWhat";
            this.lblFindWhat.Size = new System.Drawing.Size(47, 13);
            this.lblFindWhat.TabIndex = 35;
            this.lblFindWhat.Text = "Искать:";
            // 
            // cboFindWhat
            // 
            this.cboFindWhat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboFindWhat.Location = new System.Drawing.Point(97, 24);
            this.cboFindWhat.Name = "cboFindWhat";
            this.cboFindWhat.Size = new System.Drawing.Size(238, 21);
            this.cboFindWhat.TabIndex = 36;
            this.cboFindWhat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cboFindWhat_KeyPress);
            // 
            // clbSearchColumns
            // 
            this.clbSearchColumns.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbSearchColumns.CheckOnClick = true;
            this.clbSearchColumns.FormattingEnabled = true;
            this.clbSearchColumns.Location = new System.Drawing.Point(97, 60);
            this.clbSearchColumns.Name = "clbSearchColumns";
            this.clbSearchColumns.Size = new System.Drawing.Size(238, 60);
            this.clbSearchColumns.TabIndex = 37;
            this.clbSearchColumns.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbSearchColumns_ItemCheck);
            // 
            // cboSearchDirection
            // 
            this.cboSearchDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSearchDirection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboSearchDirection.Items.AddRange(new object[] {
            "Вниз",
            "Вверх",
            "В любом направлении"});
            this.cboSearchDirection.Location = new System.Drawing.Point(97, 130);
            this.cboSearchDirection.Name = "cboSearchDirection";
            this.cboSearchDirection.Size = new System.Drawing.Size(238, 21);
            this.cboSearchDirection.TabIndex = 38;
            // 
            // TextSearcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ultraButton1);
            this.Controls.Add(this.ultraButton2);
            this.Controls.Add(this.chkMatchCase);
            this.Controls.Add(this.lblSearchDirection);
            this.Controls.Add(this.lblLookIn);
            this.Controls.Add(this.lblFindWhat);
            this.Controls.Add(this.cboFindWhat);
            this.Controls.Add(this.clbSearchColumns);
            this.Controls.Add(this.cboSearchDirection);
            this.Name = "TextSearcher";
            this.Size = new System.Drawing.Size(348, 206);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextSearcher_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.Misc.UltraButton ultraButton1;
        private Infragistics.Win.Misc.UltraButton ultraButton2;
        internal System.Windows.Forms.CheckBox chkMatchCase;
        internal System.Windows.Forms.Label lblSearchDirection;
        internal System.Windows.Forms.Label lblLookIn;
        internal System.Windows.Forms.Label lblFindWhat;
        internal System.Windows.Forms.ComboBox cboFindWhat;
        private System.Windows.Forms.CheckedListBox clbSearchColumns;
        internal System.Windows.Forms.ComboBox cboSearchDirection;
    }
}
