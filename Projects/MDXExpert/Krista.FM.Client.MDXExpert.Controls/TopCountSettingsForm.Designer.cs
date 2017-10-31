namespace Krista.FM.Client.MDXExpert.Controls
{
    partial class TopCountSettingsForm
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
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.pContainer = new Infragistics.Win.Misc.UltraPanel();
            this.neTopCount = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.btCalcel = new Infragistics.Win.Misc.UltraButton();
            this.ceIsTopCountCalculate = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.cpTopCountColor = new Infragistics.Win.UltraWinEditors.UltraColorPicker();
            this.btOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.pContainer.ClientArea.SuspendLayout();
            this.pContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.neTopCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceIsTopCountCalculate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpTopCountColor)).BeginInit();
            this.SuspendLayout();
            // 
            // pContainer
            // 
            // 
            // pContainer.ClientArea
            // 
            this.pContainer.ClientArea.Controls.Add(this.neTopCount);
            this.pContainer.ClientArea.Controls.Add(this.ultraLabel1);
            this.pContainer.ClientArea.Controls.Add(this.btCalcel);
            this.pContainer.ClientArea.Controls.Add(this.ceIsTopCountCalculate);
            this.pContainer.ClientArea.Controls.Add(this.cpTopCountColor);
            this.pContainer.ClientArea.Controls.Add(this.btOK);
            this.pContainer.ClientArea.Controls.Add(this.ultraLabel2);
            this.pContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pContainer.Location = new System.Drawing.Point(0, 0);
            this.pContainer.Name = "pContainer";
            this.pContainer.Size = new System.Drawing.Size(260, 151);
            this.pContainer.TabIndex = 0;
            // 
            // neTopCount
            // 
            this.neTopCount.Location = new System.Drawing.Point(174, 41);
            this.neTopCount.MaskDisplayMode = Infragistics.Win.UltraWinMaskedEdit.MaskMode.IncludeLiterals;
            this.neTopCount.MaskInput = "nnnnnnnnn";
            this.neTopCount.MaxValue = 10000;
            this.neTopCount.MinValue = 0;
            this.neTopCount.Name = "neTopCount";
            this.neTopCount.PromptChar = ' ';
            this.neTopCount.Size = new System.Drawing.Size(54, 21);
            this.neTopCount.SpinButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Always;
            this.neTopCount.SpinIncrement = 1;
            this.neTopCount.TabIndex = 20;
            this.neTopCount.Value = 5;
            // 
            // ultraLabel1
            // 
            appearance1.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel1.Appearance = appearance1;
            this.ultraLabel1.AutoSize = true;
            this.ultraLabel1.Location = new System.Drawing.Point(42, 45);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(126, 14);
            this.ultraLabel1.TabIndex = 19;
            this.ultraLabel1.Text = "Количество элементов";
            // 
            // btCalcel
            // 
            this.btCalcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCalcel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCalcel.Location = new System.Drawing.Point(168, 116);
            this.btCalcel.Name = "btCalcel";
            this.btCalcel.Size = new System.Drawing.Size(75, 23);
            this.btCalcel.TabIndex = 18;
            this.btCalcel.Text = "Отмена";
            // 
            // ceIsTopCountCalculate
            // 
            appearance4.BackColor = System.Drawing.Color.Transparent;
            this.ceIsTopCountCalculate.Appearance = appearance4;
            this.ceIsTopCountCalculate.BackColor = System.Drawing.Color.Transparent;
            this.ceIsTopCountCalculate.BackColorInternal = System.Drawing.Color.Transparent;
            this.ceIsTopCountCalculate.Location = new System.Drawing.Point(24, 12);
            this.ceIsTopCountCalculate.Name = "ceIsTopCountCalculate";
            this.ceIsTopCountCalculate.Size = new System.Drawing.Size(204, 20);
            this.ceIsTopCountCalculate.TabIndex = 17;
            this.ceIsTopCountCalculate.Text = "Выделять первые ячейки";
            // 
            // cpTopCountColor
            // 
            this.cpTopCountColor.Color = System.Drawing.Color.Red;
            this.cpTopCountColor.Location = new System.Drawing.Point(174, 70);
            this.cpTopCountColor.Name = "cpTopCountColor";
            this.cpTopCountColor.Size = new System.Drawing.Size(42, 21);
            this.cpTopCountColor.TabIndex = 5;
            this.cpTopCountColor.Text = "Red";
            // 
            // btOK
            // 
            this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(86, 116);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 4;
            this.btOK.Text = "OK";
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // ultraLabel2
            // 
            appearance2.BackColor = System.Drawing.Color.Transparent;
            this.ultraLabel2.Appearance = appearance2;
            this.ultraLabel2.AutoSize = true;
            this.ultraLabel2.Location = new System.Drawing.Point(42, 77);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(109, 14);
            this.ultraLabel2.TabIndex = 3;
            this.ultraLabel2.Text = "Цвет заливки ячеек";
            // 
            // TopCountSettingsForm
            // 
            this.AcceptButton = this.btOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCalcel;
            this.ClientSize = new System.Drawing.Size(260, 151);
            this.ControlBox = false;
            this.Controls.Add(this.pContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TopCountSettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "k-первых элементов";
            this.pContainer.ClientArea.ResumeLayout(false);
            this.pContainer.ClientArea.PerformLayout();
            this.pContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.neTopCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceIsTopCountCalculate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cpTopCountColor)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.Misc.UltraPanel pContainer;
        private Infragistics.Win.Misc.UltraButton btOK;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraColorPicker cpTopCountColor;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor ceIsTopCountCalculate;
        private Infragistics.Win.Misc.UltraButton btCalcel;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor neTopCount;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
    }
}