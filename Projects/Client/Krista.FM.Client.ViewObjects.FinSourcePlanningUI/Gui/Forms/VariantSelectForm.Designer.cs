namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms
{
    partial class VariantSelectForm
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
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton2 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton3 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            this.uteIFVariant = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.uteOutcomeVariant = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.uteIncomeVariant = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbBudgetDataType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.uteIFVariant)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteOutcomeVariant)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteIncomeVariant)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uteIFVariant
            // 
            this.uteIFVariant.AcceptsReturn = true;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            this.uteIFVariant.Appearance = appearance1;
            this.uteIFVariant.BackColor = System.Drawing.SystemColors.Window;
            appearance2.FontData.BoldAsString = "True";
            editorButton1.Appearance = appearance2;
            editorButton1.Text = "...";
            this.uteIFVariant.ButtonsRight.Add(editorButton1);
            this.uteIFVariant.Location = new System.Drawing.Point(149, 75);
            this.uteIFVariant.Name = "uteIFVariant";
            this.uteIFVariant.ReadOnly = true;
            this.uteIFVariant.Size = new System.Drawing.Size(362, 21);
            this.uteIFVariant.TabIndex = 2;
            this.uteIFVariant.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.uteVariantIF_EditorButtonClick);
            // 
            // uteOutcomeVariant
            // 
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            this.uteOutcomeVariant.Appearance = appearance5;
            this.uteOutcomeVariant.BackColor = System.Drawing.SystemColors.Window;
            appearance6.FontData.BoldAsString = "True";
            editorButton2.Appearance = appearance6;
            editorButton2.Text = "...";
            this.uteOutcomeVariant.ButtonsRight.Add(editorButton2);
            this.uteOutcomeVariant.Location = new System.Drawing.Point(149, 48);
            this.uteOutcomeVariant.Name = "uteOutcomeVariant";
            this.uteOutcomeVariant.ReadOnly = true;
            this.uteOutcomeVariant.Size = new System.Drawing.Size(362, 21);
            this.uteOutcomeVariant.TabIndex = 1;
            this.uteOutcomeVariant.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.uteVariantOutcome_EditorButtonClick);
            // 
            // uteIncomeVariant
            // 
            appearance3.BackColor = System.Drawing.SystemColors.Window;
            this.uteIncomeVariant.Appearance = appearance3;
            this.uteIncomeVariant.BackColor = System.Drawing.SystemColors.Window;
            appearance4.FontData.BoldAsString = "True";
            editorButton3.Appearance = appearance4;
            editorButton3.Text = "...";
            this.uteIncomeVariant.ButtonsRight.Add(editorButton3);
            this.uteIncomeVariant.Location = new System.Drawing.Point(149, 21);
            this.uteIncomeVariant.Name = "uteIncomeVariant";
            this.uteIncomeVariant.ReadOnly = true;
            this.uteIncomeVariant.Size = new System.Drawing.Size(362, 21);
            this.uteIncomeVariant.TabIndex = 0;
            this.uteIncomeVariant.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.uteVariantIncome_EditorButtonClick);
            // 
            // btnCancel
            // 
            this.btnCancel.AcceptsFocus = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(436, 136);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Отмена";
            // 
            // btnOK
            // 
            this.btnOK.AcceptsFocus = false;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(355, 136);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "ОК";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Location = new System.Drawing.Point(12, 21);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(131, 21);
            this.ultraLabel1.TabIndex = 15;
            this.ultraLabel1.Text = "Вариант доходов:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Location = new System.Drawing.Point(12, 48);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(131, 21);
            this.ultraLabel2.TabIndex = 16;
            this.ultraLabel2.Text = "Вариант расходов:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.Location = new System.Drawing.Point(12, 75);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(131, 21);
            this.ultraLabel3.TabIndex = 17;
            this.ultraLabel3.Text = "Вариант ИФ:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbBudgetDataType);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(12, 112);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(198, 55);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Данные АС Бюджет";
            // 
            // cbBudgetDataType
            // 
            this.cbBudgetDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBudgetDataType.FormattingEnabled = true;
            this.cbBudgetDataType.Items.AddRange(new object[] {
            "Бюджетная роспись",
            "Поквартальный кассовый план",
            "Помесячный кассовый план"});
            this.cbBudgetDataType.Location = new System.Drawing.Point(6, 24);
            this.cbBudgetDataType.Name = "cbBudgetDataType";
            this.cbBudgetDataType.Size = new System.Drawing.Size(186, 21);
            this.cbBudgetDataType.TabIndex = 0;
            // 
            // VariantSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 171);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ultraLabel3);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.uteIFVariant);
            this.Controls.Add(this.uteOutcomeVariant);
            this.Controls.Add(this.uteIncomeVariant);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VariantSelectForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выбор вариантов";
            ((System.ComponentModel.ISupportInitialize)(this.uteIFVariant)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteOutcomeVariant)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteIncomeVariant)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal Infragistics.Win.UltraWinEditors.UltraTextEditor uteIFVariant;
        internal Infragistics.Win.UltraWinEditors.UltraTextEditor uteOutcomeVariant;
        internal Infragistics.Win.UltraWinEditors.UltraTextEditor uteIncomeVariant;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbBudgetDataType;
    }
}