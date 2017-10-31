namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms
{
    partial class BudgetTransfertParamsForm
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
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton1 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton2 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.EditorButton editorButton3 = new Infragistics.Win.UltraWinEditors.EditorButton();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            this.uteOutcomeVariant = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.uteIncomeVariant = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.btnOK = new Infragistics.Win.Misc.UltraButton();
            this.btnCancel = new Infragistics.Win.Misc.UltraButton();
            this.ultraTextEditor1 = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            ((System.ComponentModel.ISupportInitialize)(this.uteOutcomeVariant)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteIncomeVariant)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor1)).BeginInit();
            this.SuspendLayout();
            // 
            // uteOutcomeVariant
            // 
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            this.uteOutcomeVariant.Appearance = appearance5;
            this.uteOutcomeVariant.BackColor = System.Drawing.SystemColors.Window;
            appearance6.FontData.BoldAsString = "True";
            editorButton1.Appearance = appearance6;
            editorButton1.Text = "...";
            this.uteOutcomeVariant.ButtonsRight.Add(editorButton1);
            this.uteOutcomeVariant.Location = new System.Drawing.Point(159, 56);
            this.uteOutcomeVariant.Name = "uteOutcomeVariant";
            this.uteOutcomeVariant.ReadOnly = true;
            this.uteOutcomeVariant.Size = new System.Drawing.Size(362, 21);
            this.uteOutcomeVariant.TabIndex = 4;
            this.uteOutcomeVariant.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.uteVariantOutcome_EditorButtonClick);
            // 
            // uteIncomeVariant
            // 
            appearance3.BackColor = System.Drawing.SystemColors.Window;
            this.uteIncomeVariant.Appearance = appearance3;
            this.uteIncomeVariant.BackColor = System.Drawing.SystemColors.Window;
            appearance4.FontData.BoldAsString = "True";
            editorButton2.Appearance = appearance4;
            editorButton2.Text = "...";
            this.uteIncomeVariant.ButtonsRight.Add(editorButton2);
            this.uteIncomeVariant.Location = new System.Drawing.Point(159, 29);
            this.uteIncomeVariant.Name = "uteIncomeVariant";
            this.uteIncomeVariant.ReadOnly = true;
            this.uteIncomeVariant.Size = new System.Drawing.Size(362, 21);
            this.uteIncomeVariant.TabIndex = 3;
            this.uteIncomeVariant.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(this.uteVariantIncome_EditorButtonClick);
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Location = new System.Drawing.Point(12, 56);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(131, 21);
            this.ultraLabel2.TabIndex = 19;
            this.ultraLabel2.Text = "Вариант расходов:";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.Location = new System.Drawing.Point(12, 29);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(131, 21);
            this.ultraLabel1.TabIndex = 18;
            this.ultraLabel1.Text = "Вариант доходов:";
            // 
            // btnOK
            // 
            this.btnOK.AcceptsFocus = false;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(365, 128);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 21;
            this.btnOK.Text = "ОК";
            // 
            // btnCancel
            // 
            this.btnCancel.AcceptsFocus = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(446, 128);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 22;
            this.btnCancel.Text = "Отмена";
            // 
            // ultraTextEditor1
            // 
            this.ultraTextEditor1.AcceptsReturn = true;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            this.ultraTextEditor1.Appearance = appearance1;
            this.ultraTextEditor1.BackColor = System.Drawing.SystemColors.Window;
            appearance2.FontData.BoldAsString = "True";
            editorButton3.Appearance = appearance2;
            editorButton3.Text = "...";
            this.ultraTextEditor1.ButtonsRight.Add(editorButton3);
            this.ultraTextEditor1.Enabled = false;
            this.ultraTextEditor1.Location = new System.Drawing.Point(159, 84);
            this.ultraTextEditor1.Name = "ultraTextEditor1";
            this.ultraTextEditor1.ReadOnly = true;
            this.ultraTextEditor1.Size = new System.Drawing.Size(362, 21);
            this.ultraTextEditor1.TabIndex = 5;
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.Location = new System.Drawing.Point(12, 84);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(131, 21);
            this.ultraLabel4.TabIndex = 20;
            this.ultraLabel4.Text = "Источник данных:";
            // 
            // BudgetTransfertParamsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 165);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.ultraLabel4);
            this.Controls.Add(this.ultraLabel2);
            this.Controls.Add(this.ultraLabel1);
            this.Controls.Add(this.ultraTextEditor1);
            this.Controls.Add(this.uteOutcomeVariant);
            this.Controls.Add(this.uteIncomeVariant);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "BudgetTransfertParamsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Параметры переноса данных";
            ((System.ComponentModel.ISupportInitialize)(this.uteOutcomeVariant)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteIncomeVariant)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTextEditor1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal Infragistics.Win.UltraWinEditors.UltraTextEditor uteOutcomeVariant;
        internal Infragistics.Win.UltraWinEditors.UltraTextEditor uteIncomeVariant;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private Infragistics.Win.Misc.UltraButton btnOK;
        private Infragistics.Win.Misc.UltraButton btnCancel;
        internal Infragistics.Win.UltraWinEditors.UltraTextEditor ultraTextEditor1;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
    }
}