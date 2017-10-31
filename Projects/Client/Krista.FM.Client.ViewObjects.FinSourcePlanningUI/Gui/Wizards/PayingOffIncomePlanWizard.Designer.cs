namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    partial class PayingOffIncomePlanWizard
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
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance("LabelsApp", 72651641);
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance("LabelsApp", 72651641);
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance("LabelsApp", 72793219);
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance("LabelsApp", 72651641);
            this.wizard = new Krista.FM.Client.Common.Wizards.WizardForm();
            this.wizardWelcomePage1 = new Krista.FM.Client.Common.Wizards.WizardWelcomePage();
            this.wizardParametersPage = new Krista.FM.Client.Common.Wizards.WizardPageBase();
            this.nePeriodsCount = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.ccEndDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.ccStartDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.ucePeriods = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel9 = new Infragistics.Win.Misc.UltraLabel();
            this.uneDayCount = new Infragistics.Win.UltraWinEditors.UltraNumericEditor();
            this.wizardCustomPeriodsPage = new Krista.FM.Client.Common.Wizards.WizardPageBase();
            this.ugeCustomPeriods = new Krista.FM.Client.Components.UltraGridEx();
            this.wizardFinalPage = new Krista.FM.Client.Common.Wizards.WizardFinalPage();
            this.dataSet = new System.Data.DataSet();
            this.CustomPeriods = new System.Data.DataTable();
            this.dataColumnID = new System.Data.DataColumn();
            this.dataColumnDateStart = new System.Data.DataColumn();
            this.dataColumnDateEnd = new System.Data.DataColumn();
            this.wizardParametersPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nePeriodsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ccEndDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ccStartDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucePeriods)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uneDayCount)).BeginInit();
            this.wizardCustomPeriodsPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CustomPeriods)).BeginInit();
            this.SuspendLayout();
            // 
            // wizard
            // 
            this.wizard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizard.Location = new System.Drawing.Point(0, 0);
            this.wizard.Name = "wizard";
            this.wizard.PageIndex = 1;
            this.wizard.Pages.AddRange(new Krista.FM.Client.Common.Wizards.WizardPageBase[] {
            this.wizardWelcomePage1,
            this.wizardParametersPage,
            this.wizardCustomPeriodsPage,
            this.wizardFinalPage});
            this.wizard.Size = new System.Drawing.Size(678, 426);
            this.wizard.TabIndex = 0;
            this.wizard.Cancel += new Krista.FM.Client.Common.Wizards.WizardForm.WizardCancelEventHandler(this.wizard_Cancel);
            this.wizard.WizardClosed += new System.EventHandler(this.wizard_WizardClosed);
            this.wizard.Back += new Krista.FM.Client.Common.Wizards.WizardForm.WizardNextEventHandler(this.wizard_Back);
            this.wizard.Finish += new Krista.FM.Client.Common.Wizards.WizardForm.WizardNextEventHandler(this.wizard_Finish);
            this.wizard.Next += new Krista.FM.Client.Common.Wizards.WizardForm.WizardNextEventHandler(this.wizard_Next);
            // 
            // wizardWelcomePage1
            // 
            this.wizardWelcomePage1.BackColor = System.Drawing.Color.White;
            this.wizardWelcomePage1.Description = "Добро пожаловать в мастер формирования плана выплаты дохода";
            this.wizardWelcomePage1.Description2 = "";
            this.wizardWelcomePage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardWelcomePage1.Index = 0;
            this.wizardWelcomePage1.Location = new System.Drawing.Point(0, 0);
            this.wizardWelcomePage1.Name = "wizardWelcomePage1";
            this.wizardWelcomePage1.Size = new System.Drawing.Size(678, 379);
            this.wizardWelcomePage1.TabIndex = 0;
            this.wizardWelcomePage1.Title = "План выплаты дохода";
            this.wizardWelcomePage1.WelcomePage = true;
            this.wizardWelcomePage1.WizardPageParent = this.wizard;
            // 
            // wizardParametersPage
            // 
            this.wizardParametersPage.Controls.Add(this.nePeriodsCount);
            this.wizardParametersPage.Controls.Add(this.ultraLabel7);
            this.wizardParametersPage.Controls.Add(this.ultraLabel1);
            this.wizardParametersPage.Controls.Add(this.ultraLabel2);
            this.wizardParametersPage.Controls.Add(this.ultraLabel6);
            this.wizardParametersPage.Controls.Add(this.ccEndDate);
            this.wizardParametersPage.Controls.Add(this.ccStartDate);
            this.wizardParametersPage.Controls.Add(this.ultraLabel4);
            this.wizardParametersPage.Controls.Add(this.ultraLabel5);
            this.wizardParametersPage.Controls.Add(this.ultraLabel3);
            this.wizardParametersPage.Controls.Add(this.ucePeriods);
            this.wizardParametersPage.Controls.Add(this.ultraLabel8);
            this.wizardParametersPage.Controls.Add(this.ultraLabel9);
            this.wizardParametersPage.Controls.Add(this.uneDayCount);
            this.wizardParametersPage.Description = "Дата начала и окончания выплаты дохода по умолчанию проставляется из таблицы теку" +
                "щего кредитного договора. Для изменения дат и периода, пожалуйста, выберите знач" +
                "ения вручную.";
            this.wizardParametersPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardParametersPage.Index = 1;
            this.wizardParametersPage.Location = new System.Drawing.Point(0, 0);
            this.wizardParametersPage.Name = "wizardParametersPage";
            this.wizardParametersPage.Size = new System.Drawing.Size(662, 315);
            this.wizardParametersPage.TabIndex = 0;
            this.wizardParametersPage.Title = "Параметры";
            this.wizardParametersPage.WizardPageParent = this.wizard;
            // 
            // nePeriodsCount
            // 
            this.nePeriodsCount.Location = new System.Drawing.Point(318, 222);
            this.nePeriodsCount.Name = "nePeriodsCount";
            this.nePeriodsCount.ReadOnly = true;
            this.nePeriodsCount.Size = new System.Drawing.Size(75, 21);
            this.nePeriodsCount.TabIndex = 15;
            // 
            // ultraLabel7
            // 
            this.ultraLabel7.Location = new System.Drawing.Point(39, 226);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(273, 14);
            this.ultraLabel7.TabIndex = 14;
            this.ultraLabel7.Text = "Количество периодов погашения основного долга:";
            // 
            // ultraLabel1
            // 
            appearance1.TextHAlignAsString = "Left";
            appearance1.TextVAlignAsString = "Middle";
            this.ultraLabel1.Appearance = appearance1;
            appearance4.TextHAlignAsString = "Left";
            appearance4.TextVAlignAsString = "Middle";
            this.ultraLabel1.Appearances.Add(appearance4);
            this.ultraLabel1.Location = new System.Drawing.Point(56, 146);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(89, 21);
            this.ultraLabel1.TabIndex = 13;
            this.ultraLabel1.Text = "Период:";
            // 
            // ultraLabel2
            // 
            this.ultraLabel2.Location = new System.Drawing.Point(39, 125);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(562, 14);
            this.ultraLabel2.TabIndex = 8;
            this.ultraLabel2.Text = "Периодичность графика выплаты дохода";
            // 
            // ultraLabel6
            // 
            this.ultraLabel6.Location = new System.Drawing.Point(39, 64);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(562, 28);
            this.ultraLabel6.TabIndex = 4;
            this.ultraLabel6.Text = "Дата окончания - дата, до которой необходимо погасить часть основной суммы долга " +
                "в соответствии с графиком погашения";
            // 
            // ccEndDate
            // 
            this.ccEndDate.DateTime = new System.DateTime(((long)(0)));
            this.ccEndDate.Location = new System.Drawing.Point(151, 98);
            this.ccEndDate.Name = "ccEndDate";
            this.ccEndDate.Size = new System.Drawing.Size(132, 21);
            this.ccEndDate.TabIndex = 3;
            this.ccEndDate.Value = new System.DateTime(((long)(0)));
            // 
            // ccStartDate
            // 
            this.ccStartDate.DateTime = new System.DateTime(((long)(0)));
            this.ccStartDate.Location = new System.Drawing.Point(151, 37);
            this.ccStartDate.Name = "ccStartDate";
            this.ccStartDate.Size = new System.Drawing.Size(132, 21);
            this.ccStartDate.TabIndex = 2;
            this.ccStartDate.Value = new System.DateTime(((long)(0)));
            // 
            // ultraLabel4
            // 
            appearance7.TextHAlignAsString = "Left";
            appearance7.TextVAlignAsString = "Middle";
            this.ultraLabel4.Appearance = appearance7;
            appearance8.TextHAlignAsString = "Left";
            appearance8.TextVAlignAsString = "Middle";
            this.ultraLabel4.Appearances.Add(appearance8);
            this.ultraLabel4.Location = new System.Drawing.Point(56, 98);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(89, 21);
            this.ultraLabel4.TabIndex = 1;
            this.ultraLabel4.Text = "Дата окончания:";
            // 
            // ultraLabel5
            // 
            appearance6.TextVAlignAsString = "Middle";
            this.ultraLabel5.Appearance = appearance6;
            appearance3.TextVAlignAsString = "Middle";
            this.ultraLabel5.Appearances.Add(appearance3);
            this.ultraLabel5.Location = new System.Drawing.Point(56, 37);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(89, 21);
            this.ultraLabel5.TabIndex = 1;
            this.ultraLabel5.Text = "Дата начала:";
            // 
            // ultraLabel3
            // 
            this.ultraLabel3.Location = new System.Drawing.Point(39, 3);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(562, 28);
            this.ultraLabel3.TabIndex = 1;
            this.ultraLabel3.Text = "Дата начала - дата, начиная с которой необходимо погасить часть основной суммы до" +
                "лга в соответствии с графиком погашения";
            // 
            // ucePeriods
            // 
            this.ucePeriods.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.ucePeriods.Location = new System.Drawing.Point(151, 145);
            this.ucePeriods.Name = "ucePeriods";
            this.ucePeriods.Size = new System.Drawing.Size(132, 21);
            this.ucePeriods.TabIndex = 1;
            // 
            // ultraLabel8
            // 
            appearance5.TextHAlignAsString = "Left";
            appearance5.TextVAlignAsString = "Middle";
            this.ultraLabel8.Appearance = appearance5;
            appearance2.TextHAlignAsString = "Left";
            appearance2.TextVAlignAsString = "Middle";
            this.ultraLabel8.Appearances.Add(appearance2);
            this.ultraLabel8.Location = new System.Drawing.Point(56, 195);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(89, 21);
            this.ultraLabel8.TabIndex = 5;
            this.ultraLabel8.Text = "Количество дней";
            // 
            // ultraLabel9
            // 
            this.ultraLabel9.Location = new System.Drawing.Point(39, 172);
            this.ultraLabel9.Name = "ultraLabel9";
            this.ultraLabel9.Size = new System.Drawing.Size(306, 14);
            this.ultraLabel9.TabIndex = 15;
            this.ultraLabel9.Text = "Количество дней в периоде";
            // 
            // uneDayCount
            // 
            this.uneDayCount.Enabled = false;
            this.uneDayCount.Location = new System.Drawing.Point(151, 195);
            this.uneDayCount.Name = "uneDayCount";
            this.uneDayCount.PromptChar = ' ';
            this.uneDayCount.Size = new System.Drawing.Size(132, 21);
            this.uneDayCount.TabIndex = 1;
            this.uneDayCount.Value = 30;
            // 
            // wizardCustomPeriodsPage
            // 
            this.wizardCustomPeriodsPage.Controls.Add(this.ugeCustomPeriods);
            this.wizardCustomPeriodsPage.Description = "Введите вручную значения начала и конца периода в поле «Начало периода» и «Конец " +
                "периода» соответственно";
            this.wizardCustomPeriodsPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardCustomPeriodsPage.Index = 2;
            this.wizardCustomPeriodsPage.Location = new System.Drawing.Point(0, 0);
            this.wizardCustomPeriodsPage.Name = "wizardCustomPeriodsPage";
            this.wizardCustomPeriodsPage.Size = new System.Drawing.Size(662, 315);
            this.wizardCustomPeriodsPage.TabIndex = 0;
            this.wizardCustomPeriodsPage.Title = "Другая периодичность";
            this.wizardCustomPeriodsPage.WizardPageParent = this.wizard;
            // 
            // ugeCustomPeriods
            // 
            this.ugeCustomPeriods.AllowAddNewRecords = true;
            this.ugeCustomPeriods.AllowClearTable = true;
            this.ugeCustomPeriods.AllowImportFromXML = false;
            this.ugeCustomPeriods.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ugeCustomPeriods.Caption = "";
            this.ugeCustomPeriods.ColumnsToolbarVisible = false;
            this.ugeCustomPeriods.ExportImportToolbarVisible = false;
            this.ugeCustomPeriods.InDebugMode = false;
            this.ugeCustomPeriods.LoadMenuVisible = false;
            this.ugeCustomPeriods.Location = new System.Drawing.Point(24, 3);
            this.ugeCustomPeriods.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ugeCustomPeriods.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ugeCustomPeriods.Name = "ugeCustomPeriods";
            this.ugeCustomPeriods.SaveLoadFileName = "";
            this.ugeCustomPeriods.SaveMenuVisible = false;
            this.ugeCustomPeriods.ServerFilterEnabled = false;
            this.ugeCustomPeriods.SingleBandLevelName = "Добавить период...";
            this.ugeCustomPeriods.Size = new System.Drawing.Size(609, 309);
            this.ugeCustomPeriods.sortColumnName = "ID";
            this.ugeCustomPeriods.StateRowEnable = false;
            this.ugeCustomPeriods.TabIndex = 0;
            // 
            // wizardFinalPage
            // 
            this.wizardFinalPage.BackColor = System.Drawing.Color.White;
            this.wizardFinalPage.Description = "";
            this.wizardFinalPage.Description2 = "";
            this.wizardFinalPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardFinalPage.FinishPage = true;
            this.wizardFinalPage.Index = 3;
            this.wizardFinalPage.Location = new System.Drawing.Point(0, 0);
            this.wizardFinalPage.Name = "wizardFinalPage";
            this.wizardFinalPage.Size = new System.Drawing.Size(678, 379);
            this.wizardFinalPage.TabIndex = 0;
            this.wizardFinalPage.Title = "";
            this.wizardFinalPage.WelcomePage = true;
            this.wizardFinalPage.WizardPageParent = this.wizard;
            // 
            // dataSet
            // 
            this.dataSet.DataSetName = "DataSet";
            this.dataSet.Tables.AddRange(new System.Data.DataTable[] {
            this.CustomPeriods});
            // 
            // CustomPeriods
            // 
            this.CustomPeriods.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumnID,
            this.dataColumnDateStart,
            this.dataColumnDateEnd});
            this.CustomPeriods.TableName = "CustomPeriods";
            // 
            // dataColumnID
            // 
            this.dataColumnID.AllowDBNull = false;
            this.dataColumnID.AutoIncrement = true;
            this.dataColumnID.AutoIncrementSeed = ((long)(1));
            this.dataColumnID.Caption = "№ периода";
            this.dataColumnID.ColumnName = "ID";
            this.dataColumnID.DataType = typeof(int);
            // 
            // dataColumnDateStart
            // 
            this.dataColumnDateStart.AllowDBNull = false;
            this.dataColumnDateStart.Caption = "Начало периода";
            this.dataColumnDateStart.ColumnName = "DateStart";
            this.dataColumnDateStart.DataType = typeof(System.DateTime);
            // 
            // dataColumnDateEnd
            // 
            this.dataColumnDateEnd.AllowDBNull = false;
            this.dataColumnDateEnd.Caption = "Конец периода";
            this.dataColumnDateEnd.ColumnName = "DateEnd";
            this.dataColumnDateEnd.DataType = typeof(System.DateTime);
            // 
            // PayingOffIncomePlanWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 426);
            this.Controls.Add(this.wizard);
            this.Name = "PayingOffIncomePlanWizard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "План погашения основного долга";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AcquittanceMainPlanWizard_FormClosed);
            this.Load += new System.EventHandler(this.AcquittanceMainPlanWizard_Load);
            this.wizardParametersPage.ResumeLayout(false);
            this.wizardParametersPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nePeriodsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ccEndDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ccStartDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucePeriods)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uneDayCount)).EndInit();
            this.wizardCustomPeriodsPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CustomPeriods)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Krista.FM.Client.Common.Wizards.WizardForm wizard;
        private Krista.FM.Client.Common.Wizards.WizardWelcomePage wizardWelcomePage1;
        private Krista.FM.Client.Common.Wizards.WizardPageBase wizardParametersPage;
        private Krista.FM.Client.Common.Wizards.WizardPageBase wizardCustomPeriodsPage;
        private Krista.FM.Client.Common.Wizards.WizardFinalPage wizardFinalPage;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor ccStartDate;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        private Infragistics.Win.UltraWinEditors.UltraDateTimeEditor ccEndDate;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor nePeriodsCount;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        private System.Data.DataSet dataSet;
        private System.Data.DataTable CustomPeriods;
        private System.Data.DataColumn dataColumnID;
        private System.Data.DataColumn dataColumnDateStart;
        private System.Data.DataColumn dataColumnDateEnd;
        private Krista.FM.Client.Components.UltraGridEx ugeCustomPeriods;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor ucePeriods;
        private Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private Infragistics.Win.Misc.UltraLabel ultraLabel9;
        private Infragistics.Win.UltraWinEditors.UltraNumericEditor uneDayCount;
    }
}