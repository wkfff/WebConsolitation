using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common.Wizards;

namespace Krista.FM.Utils.DTSGenerator.Wizards
{
    partial class PartialWizardForm
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
            this.partialWizard = new Krista.FM.Client.Common.Wizards.WizardForm();
            this.wizardPageValidate = new Krista.FM.Client.Common.Wizards.WizardPageBase();
            this.ultraGridExValidate = new Krista.FM.Client.Components.UltraGridEx();
            this.wizardPageTables = new Krista.FM.Client.Common.Wizards.WizardPageBase();
            this.ultraGridExTables = new Krista.FM.Client.Components.UltraGridEx();
            this.wizardPageBase1 = new Krista.FM.Client.Common.Wizards.WizardWelcomePage();
            this.wizardPageDataSource = new Krista.FM.Client.Common.Wizards.WizardPageBase();
            this.ultraGridExDataSorce = new Krista.FM.Client.Components.UltraGridEx();
            this.wizardFinalPage = new WizardFinalPage();
            this.wizardPageValidate.SuspendLayout();
            this.wizardPageTables.SuspendLayout();
            this.SuspendLayout();
            // 
            // partialWizard
            // 
            this.partialWizard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.partialWizard.Location = new System.Drawing.Point(0, 0);
            this.partialWizard.Name = "WizardForm";
            this.partialWizard.PageIndex = 0;
            this.partialWizard.Pages.AddRange(new Krista.FM.Client.Common.Wizards.WizardPageBase[] {
            this.wizardPageDataSource,
            this.wizardPageTables,
            this.wizardPageValidate,
            this.wizardPageBase1,
            this.wizardFinalPage});
            this.partialWizard.Size = new System.Drawing.Size(684, 457);
            this.partialWizard.TabIndex = 0;
            // 
            // wizardPageValidate
            // 
            this.wizardPageValidate.Controls.Add(this.ultraGridExValidate);
            this.wizardPageValidate.Description = "Проверка на идентичность метаданных в переносимом блоке";
            this.wizardPageValidate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPageValidate.Index = 2;
            this.wizardPageValidate.Location = new System.Drawing.Point(0, 0);
            this.wizardPageValidate.Name = "WizardPageValidate";
            this.wizardPageValidate.Size = new System.Drawing.Size(480, 261);
            this.wizardPageValidate.TabIndex = 0;
            this.wizardPageValidate.Title = "Валидация метаданных в базах";
            this.wizardPageValidate.WizardPageParent = this.partialWizard;
            // 
            // ultraGridExValidate
            // 
            this.ultraGridExValidate.AllowAddNewRecords = true;
            this.ultraGridExValidate.AllowClearTable = true;
            this.ultraGridExValidate.AllowDeleteRows = false;
            this.ultraGridExValidate.Caption = "";
            this.ultraGridExValidate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGridExValidate.InDebugMode = false;
            this.ultraGridExValidate.IsReadOnly = true;
            this.ultraGridExValidate.LoadMenuVisible = false;
            this.ultraGridExValidate.Location = new System.Drawing.Point(0, 0);
            this.ultraGridExValidate.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ultraGridExValidate.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ultraGridExValidate.Name = "ultraGridExValidate";
            this.ultraGridExValidate.SaveLoadFileName = "";
            this.ultraGridExValidate.SaveMenuVisible = false;
            this.ultraGridExValidate.ServerFilterEnabled = false;
            this.ultraGridExValidate.SingleBandLevelName = "Добавить запись...";
            this.ultraGridExValidate.Size = new System.Drawing.Size(480, 261);
            this.ultraGridExValidate.sortColumnName = "";
            this.ultraGridExValidate.StateRowEnable = false;
            this.ultraGridExValidate.TabIndex = 0;
            this.ultraGridExValidate.SetHierarchyFilter(Krista.FM.Client.Components.UltraGridEx.FilterConditionAction.Set);
            this.ultraGridExValidate.ugData.DisplayLayout.Override.AllowColSizing = AllowColSizing.Free;
            this.ultraGridExValidate.ugData.DisplayLayout.Override.ColumnSizingArea = ColumnSizingArea.EntireColumn;
            // 
            // wizardPageTables
            // 
            this.wizardPageTables.Controls.Add(this.ultraGridExTables);
            this.wizardPageTables.Description = null;
            this.wizardPageTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPageTables.Index = 1;
            this.wizardPageTables.Location = new System.Drawing.Point(0, 0);
            this.wizardPageTables.Name = "WizardPageTables";
            this.wizardPageTables.Size = new System.Drawing.Size(480, 261);
            this.wizardPageTables.TabIndex = 0;
            this.wizardPageTables.Title = "Список таблиц для переноса";
            this.wizardPageTables.WizardPageParent = this.partialWizard;
            // 
            // ultraGridExTables
            // 
            this.ultraGridExTables.AllowAddNewRecords = true;
            this.ultraGridExTables.AllowClearTable = true;
            this.ultraGridExTables.Caption = "";
            this.ultraGridExTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGridExTables.InDebugMode = false;
            this.ultraGridExTables.IsReadOnly = true;
            this.ultraGridExTables.LoadMenuVisible = false;
            this.ultraGridExTables.Location = new System.Drawing.Point(0, 0);
            this.ultraGridExTables.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ultraGridExTables.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ultraGridExTables.Name = "ultraGridExTables";
            this.ultraGridExTables.SaveLoadFileName = "";
            this.ultraGridExTables.SaveMenuVisible = false;
            this.ultraGridExTables.ServerFilterEnabled = false;
            this.ultraGridExTables.SingleBandLevelName = "Добавить запись...";
            this.ultraGridExTables.Size = new System.Drawing.Size(480, 261);
            this.ultraGridExTables.sortColumnName = "";
            this.ultraGridExTables.StateRowEnable = false;
            this.ultraGridExTables.TabIndex = 0;
            // 
            // wizardPageBase1
            // 
            this.wizardPageBase1.BackColor = System.Drawing.Color.White;
            this.wizardPageBase1.Description = null;
            this.wizardPageBase1.Description2 = "some more description...";
            this.wizardPageBase1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPageBase1.Index = 0;
            this.wizardPageBase1.Location = new System.Drawing.Point(0, 0);
            this.wizardPageBase1.Name = "WizardPage";
            this.wizardPageBase1.Size = new System.Drawing.Size(684, 410);
            this.wizardPageBase1.TabIndex = 0;
            this.wizardPageBase1.Title = null;
            this.wizardPageBase1.WelcomePage = true;
            this.wizardPageBase1.WizardPageParent = this.partialWizard;
            // 
            // wizardPageDataSource
            // 
            this.wizardPageDataSource.Description = null;
            this.wizardPageDataSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardPageDataSource.Index = 3;
            this.wizardPageDataSource.Location = new System.Drawing.Point(0, 0);
            this.wizardPageDataSource.Name = "WizardPageDataSource";
            this.wizardPageDataSource.Size = new System.Drawing.Size(668, 346);
            this.wizardPageDataSource.TabIndex = 0;
            this.wizardPageDataSource.Title = "Выбор источников для переноса";
            this.wizardPageDataSource.WizardPageParent = this.partialWizard;
            this.wizardPageDataSource.Controls.Add(ultraGridExDataSorce);
            // 
            // ultraGridEx1
            // 
            this.ultraGridExDataSorce.AllowAddNewRecords = false;
            this.ultraGridExDataSorce.AllowClearTable = false;
            this.ultraGridExDataSorce.ExportImportToolbarVisible = false;
            this.ultraGridExDataSorce.Caption = "";
            this.ultraGridExDataSorce.InDebugMode = false;
            this.ultraGridExDataSorce.LoadMenuVisible = false;
            this.ultraGridExDataSorce.Location = new System.Drawing.Point(12, 56);
            this.ultraGridExDataSorce.MaxCalendarDate = new System.DateTime(((long)(0)));
            this.ultraGridExDataSorce.MinCalendarDate = new System.DateTime(((long)(0)));
            this.ultraGridExDataSorce.Name = "ultraGridEx1";
            this.ultraGridExDataSorce.SaveLoadFileName = "";
            this.ultraGridExDataSorce.SaveMenuVisible = false;
            this.ultraGridExDataSorce.ServerFilterEnabled = false;
            this.ultraGridExDataSorce.SingleBandLevelName = "Добавить запись...";
            this.ultraGridExDataSorce.Size = new System.Drawing.Size(777, 480);
            this.ultraGridExDataSorce.sortColumnName = "";
            this.ultraGridExDataSorce.StateRowEnable = false;
            this.ultraGridExDataSorce.TabIndex = 1;
            this.ultraGridExDataSorce.Dock = DockStyle.Fill;

            //
            // wizardFinalPage
            //
            this.wizardFinalPage.BackColor = System.Drawing.Color.White;
            this.wizardFinalPage.Description = null;
            this.wizardFinalPage.Description2 = null;
            this.wizardFinalPage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wizardFinalPage.Index = 4;
            this.wizardFinalPage.Location = new System.Drawing.Point(0, 0);
            this.wizardFinalPage.Name = "FinalPage";
            this.wizardFinalPage.Size = new System.Drawing.Size(684, 410);
            this.wizardFinalPage.TabIndex = 0;
            this.wizardFinalPage.WelcomePage = false;
            this.wizardFinalPage.FinishPage = true;
            this.wizardFinalPage.WizardPageParent = this.partialWizard;

            // 
            // PartialWizardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 457);
            this.Controls.Add(this.partialWizard);
            this.MinimumSize = new System.Drawing.Size(692, 484);
            this.Name = "PartialWizardForm";
            this.Text = "Мастер частичного переноса данных";
            this.wizardPageValidate.ResumeLayout(false);
            this.wizardPageTables.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Krista.FM.Client.Common.Wizards.WizardForm partialWizard;
        private Krista.FM.Client.Common.Wizards.WizardWelcomePage wizardPageBase1;
        private Krista.FM.Client.Common.Wizards.WizardPageBase wizardPageTables;
        private Krista.FM.Client.Common.Wizards.WizardPageBase wizardPageValidate;
        private Krista.FM.Client.Common.Wizards.WizardPageBase wizardPageDataSource;
        private Krista.FM.Client.Common.Wizards.WizardFinalPage wizardFinalPage;
        private Krista.FM.Client.Components.UltraGridEx ultraGridExTables;
        private Krista.FM.Client.Components.UltraGridEx ultraGridExValidate;
        private Krista.FM.Client.Components.UltraGridEx ultraGridExDataSorce;
    }
}