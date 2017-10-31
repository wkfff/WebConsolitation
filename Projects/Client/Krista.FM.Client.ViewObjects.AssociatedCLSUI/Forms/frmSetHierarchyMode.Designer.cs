namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI
{
    partial class frmSetHierarchyMode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSetHierarchyMode));
            this.rbAllRecords = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.pbQuestion = new System.Windows.Forms.PictureBox();
            this.cbDivideCode = new System.Windows.Forms.CheckBox();
            this.cbSetHierarchy = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbQuestion)).BeginInit();
            this.SuspendLayout();
            // 
            // rbAllRecords
            // 
            this.rbAllRecords.AutoSize = true;
            this.rbAllRecords.Enabled = false;
            this.rbAllRecords.Location = new System.Drawing.Point(89, 89);
            this.rbAllRecords.Name = "rbAllRecords";
            this.rbAllRecords.Size = new System.Drawing.Size(205, 17);
            this.rbAllRecords.TabIndex = 0;
            this.rbAllRecords.Text = "Для всех записей классификатора";
            this.rbAllRecords.UseVisualStyleBackColor = true;
            this.rbAllRecords.CheckedChanged += new System.EventHandler(this.rbAllRecords_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Enabled = false;
            this.radioButton2.Location = new System.Drawing.Point(89, 66);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(283, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Только для записей, где не установлена иерархия";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(205, 124);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "ОК";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(286, 124);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Отмена";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // pbQuestion
            // 
            this.pbQuestion.Image = ((System.Drawing.Image)(resources.GetObject("pbQuestion.Image")));
            this.pbQuestion.Location = new System.Drawing.Point(12, 12);
            this.pbQuestion.Name = "pbQuestion";
            this.pbQuestion.Size = new System.Drawing.Size(48, 48);
            this.pbQuestion.TabIndex = 11;
            this.pbQuestion.TabStop = false;
            // 
            // cbDivideCode
            // 
            this.cbDivideCode.AutoSize = true;
            this.cbDivideCode.Checked = true;
            this.cbDivideCode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDivideCode.Location = new System.Drawing.Point(72, 12);
            this.cbDivideCode.Name = "cbDivideCode";
            this.cbDivideCode.Size = new System.Drawing.Size(181, 17);
            this.cbDivideCode.TabIndex = 15;
            this.cbDivideCode.Text = "Выполнить расщепление кода";
            this.cbDivideCode.UseVisualStyleBackColor = true;
            this.cbDivideCode.CheckedChanged += new System.EventHandler(this.cbDivideCode_CheckedChanged);
            // 
            // cbSetHierarchy
            // 
            this.cbSetHierarchy.AutoSize = true;
            this.cbSetHierarchy.Location = new System.Drawing.Point(72, 43);
            this.cbSetHierarchy.Name = "cbSetHierarchy";
            this.cbSetHierarchy.Size = new System.Drawing.Size(186, 17);
            this.cbSetHierarchy.TabIndex = 16;
            this.cbSetHierarchy.Text = "Выполнить установку иерархии";
            this.cbSetHierarchy.UseVisualStyleBackColor = true;
            this.cbSetHierarchy.CheckedChanged += new System.EventHandler(this.cbSetHierarchy_CheckedChanged);
            // 
            // frmSetHierarchyMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 159);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.rbAllRecords);
            this.Controls.Add(this.cbSetHierarchy);
            this.Controls.Add(this.cbDivideCode);
            this.Controls.Add(this.pbQuestion);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSetHierarchyMode";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Расщепление кода и установка иерархии";
            ((System.ComponentModel.ISupportInitialize)(this.pbQuestion)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbAllRecords;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox pbQuestion;
        internal System.Windows.Forms.CheckBox cbDivideCode;
        internal System.Windows.Forms.CheckBox cbSetHierarchy;
    }
}