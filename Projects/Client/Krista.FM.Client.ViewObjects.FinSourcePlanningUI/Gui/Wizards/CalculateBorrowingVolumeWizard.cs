using System;
using System.Drawing;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.VolumeHoldings;
using Krista.FM.ServerLibrary;

using System.Data;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public partial class CalculateBorrowingVolumeWizard : Form
    {
        private const string variantIncomeKey = "1525f07f-8a60-47af-9b80-7200e74956bc";
        private const string variantOutcomeKey = "e8cb8e78-f486-46c1-800f-284eb791d95a";
        private const string variantIFKey = "e91d4e9b-7288-4489-b533-9af04bd52042";

        private BorrowingVolumeServer borrowingVolumeServer;
        private int sourceID;
        private DataTable dtBorrowingVolumeData;

        public CalculateBorrowingVolumeWizard()
        {
            InitializeComponent();

            ultraGridEx.AllowAddNewRecords = false;
            ultraGridEx.AllowClearTable = false;
            ultraGridEx.AllowDeleteRows = false;
            ultraGridEx.AllowEditRows = true;
            ultraGridEx.AllowImportFromXML = false;
            ultraGridEx.ColumnsToolbarVisible = false;
            ultraGridEx.ExportImportToolbarVisible = false;
            ultraGridEx.ToolClick += ultraGridEx_ToolClick;
           
            ImageList il = new ImageList();
            il.TransparentColor = Color.Magenta;
            il.Images.Add(Resources.ru.excelDocument);

            ButtonTool tool = new ButtonTool("btnReport");
            tool.SharedProps.AppearancesSmall.Appearance.Image = il.Images[0];
            ultraGridEx.utmMain.Tools.Add(tool);
            ultraGridEx.utmMain.Toolbars[0].Tools.AddTool("btnReport");

            ultraGridEx.OnGetGridColumnsState += ultraGridEx_OnGetGridColumnsState;
            ultraGridEx.ugData.AfterCellUpdate += ugData_AfterCellUpdate;
            ultraGridEx.ugData.DisplayLayout.GroupByBox.Hidden = true;
            ultraGridEx.OnInitializeRow += ultraGridEx_OnInitializeRow;
            

            uteVariantIncome.EditorButtonClick += uteVariantIncome_EditorButtonClick;
            uteVariantOutcome.EditorButtonClick += uteVariantOutcome_EditorButtonClick;
            uteVariantIF.EditorButtonClick += uteVariantIF_EditorButtonClick;

            wizard.Cancel += wizard_Cancel;
            wizard.Next += wizard_Next;
            wizard.Back += wizard_Back;
            wizard.Finish += wizard_Finish;
            wizard.WizardClosed += wizard_WizardClosed;

            borrowingVolumeServer = new BorrowingVolumeServer(FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme);
        }

        void ultraGridEx_OnInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            /*DataRow row = dtBorrowingVolumeData.Rows[0];
            decimal deficit = Convert.ToDecimal(row["AllIncomes"]) - Convert.ToDecimal(row["Outcomes"]);
            if (deficit > 0)
            {
                e.Row.Cells["Deficit"].Appearance.BackColor = Color.Crimson;
            }
            else
                e.Row.Cells["Deficit"].Appearance.ResetBackColor();*/
        }

        void ugData_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            Calculate();
        }

        void ultraGridEx_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (e.Tool.Key == "btnReport")
                CreateReport();
        }

        Components.GridColumnsStates ultraGridEx_OnGetGridColumnsState(object sender)
        {
            Components.GridColumnsStates states = new Components.GridColumnsStates();

            Components.GridColumnState state = new Krista.FM.Client.Components.GridColumnState("SourceID");
            state.IsHiden = true;
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("AllIncomes");
            state.ColumnCaption = "Доходы";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("GratuitousIncomes");
            state.ColumnCaption = "     Безвозмездные поступления";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("SubventionIncomes");
            state.ColumnCaption = "     Субвенции";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("Outcomes");
            state.ColumnCaption = "Расходы";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("Deficit");
            state.ColumnCaption = "Дефицит бюджета";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            state.IsReadOnly = true;
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("Stock");
            state.ColumnCaption = "Поступления по акциям";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("RestAccount");
            state.ColumnCaption = "Остатки на счету";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("Credit");
            state.ColumnCaption = "Кредиты от кредитных организаций";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("OrganizationCreditsReceipt");
            state.ColumnCaption = "     Получение кредитов от кредитных организаций";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("OrganizationCreditsPaying");
            state.ColumnCaption = "     Погашение кредитов от кредитных организаций";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("BudgetCredit");
            state.ColumnCaption = "Бюджетные кредиты";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("BudgetCreditsReceipt");
            state.ColumnCaption = "     Получение бюджетных кредитов";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("BudgetCreditsPaying");
            state.ColumnCaption = "     Погашение бюджетных кредитов";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("Borrowing");
            state.ColumnCaption = "Бюджетные кредиты и ссуды";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("budgetCreditsGrantsReceipt");
            state.ColumnCaption = "     Предоставление бюджетных кредитов";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("budgetCreditsGrantsPaying");
            state.ColumnCaption = "     Возврат бюджетных кредитов, предоставленных местным бюджетам";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("budgetCreditsGrantsUr");
            state.ColumnCaption = "     Возврат бюджетных кредитов, предоставленных юридическим лицам";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("BorrowingVolume");
            state.ColumnCaption = "Необходимый объем заимствований";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("Variant");
            state.ColumnCaption = "Вариант заимствования";
            state.IsHiden = true;
            state.IsReadOnly = true;
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("IncomesVariant");
            state.ColumnCaption = "Вариант доходов";
            state.IsHiden = true;
            state.IsReadOnly = true;
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("OutcomesVariant");
            state.ColumnCaption = "Вариант расходов";
            state.IsHiden = true;
            state.IsReadOnly = true;
            states.Add(state);

            state = new Krista.FM.Client.Components.GridColumnState("IFVariant");
            state.ColumnCaption = "Вариант ИФ";
            state.IsHiden = true;
            state.IsReadOnly = true;
            states.Add(state);

            return states;
        }

        void wizard_WizardClosed(object sender, EventArgs e)
        {
            Close();
        }

        void wizard_Finish(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            Close();
        }

        void wizard_Cancel(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventWizardCancelArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        void wizard_Back(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            wizard.WizardButtons = wizard.WizardButtons &= Krista.FM.Client.Common.Wizards.WizardForm.TWizardsButtons.Next;
        }

        void wizard_Next(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            /*
            int currentPageIndex = e.CurrentPage.Index;

            switch (currentPageIndex)
            {
                case 0:
                    wizard.WizardButtons = wizard.WizardButtons &= ~Krista.FM.Client.Common.Wizards.WizardForm.TWizardsButtons.Next;
                    break;
                case 1:
                    wizard.WizardButtons = Krista.FM.Client.Common.Wizards.WizardForm.TWizardsButtons.All;
                    // Расчет параметров
                    sourceID = BalanceServer.GetDataSourceID(FinSourcePlanningNavigation.BaseYear);
                    if (sourceID > -1)
                    {
                        dtBorrowingVolumeData = borrowingVolumeServer.CalculateBorrowingVolume(FinSourcePlanningNavigation.Instance.CurrentVariantID,
                            Convert.ToInt32(uteVariantIncome.Tag), Convert.ToInt32(uteVariantOutcome.Tag), Convert.ToInt32(uteVariantIF.Tag), FinSourcePlanningNavigation.BaseYear);

                        dtBorrowingVolumeData.Rows[0]["SourceID"] = sourceID;
                        ultraGridEx.DataSource = dtBorrowingVolumeData;
                        ultraGridEx.ugData.DisplayLayout.Bands[0].CardView = true;
                        ultraGridEx.ugData.DisplayLayout.Bands[0].CardSettings.Width = 150;
                        Calculate();
                    }
                    break;
                case 2:
                    wizard.WizardButtons = Krista.FM.Client.Common.Wizards.WizardForm.TWizardsButtons.All;
                    // Запись результатов в базу
                    if (sourceID > -1)
                    {
                        if (Convert.ToDecimal(dtBorrowingVolumeData.Rows[0]["BorrowingVolume"]) < 0)
                        {
                            wizardFinalPage.Description2 = "Доходы и поступления по источникам финансирования дефицита покрывают текущие расходы и расходы на обслуживание государственного долга. Заимствования не требуются";
                        }
                        else
                        {
                            borrowingVolumeServer.SaveBorrowingVolume(dtBorrowingVolumeData.Rows[0]);
                            wizardFinalPage.Description2 =
                                "Расчет определения необходимого заимствования успешно добавлен в таблицу";
                        }
                    }
                    else
                    {
                        wizardFinalPage.Description2 = string.Format("Источник данных по {0} году не найден",
                            FinSourcePlanningNavigation.BaseYear);
                    }
                    break;
            }*/
        }

        private void uteVariantOutcome_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            string variantCaption = uteVariantOutcome.Text;
            int variant = ChooseVariant(variantOutcomeKey, ref variantCaption);
            uteVariantOutcome.Text = variantCaption;
            uteVariantOutcome.Tag = variant != -1 ? (object)variant : null;
            if (uteVariantIncome.Tag != null && uteVariantOutcome.Tag != null && uteVariantIF.Tag != null)
                wizard.WizardButtons = wizard.WizardButtons |= Krista.FM.Client.Common.Wizards.WizardForm.TWizardsButtons.Next;
        }

        void uteVariantIncome_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            string variantCaption = uteVariantIncome.Text;
            int variant = ChooseVariant(variantIncomeKey, ref variantCaption);
            uteVariantIncome.Text = variantCaption;
            uteVariantIncome.Tag = variant != -1 ? (object)variant : null;
            if (uteVariantIncome.Tag != null && uteVariantOutcome.Tag != null && uteVariantIF.Tag != null)
                wizard.WizardButtons = wizard.WizardButtons |= Krista.FM.Client.Common.Wizards.WizardForm.TWizardsButtons.Next;
        }

        void uteVariantIF_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            string variantCaption = uteVariantIF.Text;
            int variant = ChooseVariant(variantIFKey, ref variantCaption);
            uteVariantIF.Text = variantCaption;
            uteVariantIF.Tag = variant != -1 ? (object)variant : null;
            if (uteVariantIncome.Tag != null && uteVariantOutcome.Tag != null && uteVariantIF.Tag != null)
                wizard.WizardButtons = wizard.WizardButtons |= Krista.FM.Client.Common.Wizards.WizardForm.TWizardsButtons.Next;
        }

        private static int ChooseVariant(string clsKey, ref string variantCaption)
        {
            frmModalTemplate tmpClsForm = new frmModalTemplate();

            IClassifier cls = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.Classifiers[clsKey];
            BaseClsUI clsUI = new DataClsUI(cls);
            clsUI.Workplace = FinSourcePlanningNavigation.Instance.Workplace;
            clsUI.Initialize();
            clsUI.RefreshAttachedData();
            tmpClsForm.SuspendLayout();
            try
            {
                tmpClsForm.AttachCls(clsUI);
                ComponentCustomizer.CustomizeInfragisticsControls(tmpClsForm);
            }
            finally
            {
                tmpClsForm.ResumeLayout();
            }
            if (tmpClsForm.ShowDialog() == DialogResult.OK)
            {
                variantCaption = cls.OlapName;
                variantCaption += string.Format(".{0}", clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["NAME"].Value);
                return Convert.ToInt32(clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["ID"].Value);
            }
            return -1;
        }

        private void Calculate()
        {
            DataRow row = dtBorrowingVolumeData.Rows[0];
            row["Deficit"] = Convert.ToDecimal(row["AllIncomes"]) - Convert.ToDecimal(row["Outcomes"]);
            row["Credit"] = Convert.ToDecimal(row["OrganizationCreditsReceipt"]) - Convert.ToDecimal(row["OrganizationCreditsPaying"]);
            row["BudgetCredit"] = Convert.ToDecimal(row["BudgetCreditsReceipt"]) - Convert.ToDecimal(row["BudgetCreditsPaying"]);
            row["Borrowing"] = Convert.ToDecimal(row["BudgetCreditsGrantsPaying"]) + Convert.ToDecimal(row["BudgetCreditsGrantsUr"]) -
                Convert.ToDecimal(row["BudgetCreditsGrantsReceipt"]);
            row["BorrowingVolume"] = 0 - (Convert.ToDecimal(row["Deficit"]) + Convert.ToDecimal(row["Credit"]) +
                Convert.ToDecimal(row["BudgetCredit"]) + Convert.ToDecimal(row["Borrowing"]));
        }

        private void CreateReport()
        {
            string fileName = "Отчет по объему заимствований";
            if (ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, true, ref fileName))
            {
                Infragistics.Excel.Workbook wb = new Infragistics.Excel.Workbook();
                Infragistics.Excel.Worksheet ws = wb.Worksheets.Add("Report");

                Infragistics.Excel.WorksheetColumn column = ws.Columns[1];
                column.Width = 15000;
                column.CellFormat.WrapText = Infragistics.Excel.ExcelDefaultableBoolean.True;
                column = ws.Columns[2];
                column.Width = 5000;

                Infragistics.Excel.WorksheetCell cell = ws.GetCell("B3");
                cell.Value = string.Format("Объем необходимых заимствований в {0} году",
                                           FinSourcePlanningNavigation.BaseYear);
                cell = ws.GetCell("B5");
                cell.Value = string.Format("Вариант доходов: {0}", uteVariantIncome.Text);
                cell = ws.GetCell("B6");
                cell.Value = string.Format("Вариант расходов: {0}", uteVariantOutcome.Text);
                cell = ws.GetCell("B7");
                cell.Value = string.Format("Вариант ИФ: {0}", uteVariantIF.Text);

                for (int i = 1; i <= dtBorrowingVolumeData.Columns.Count - 5; i++)
                {
                    cell = ws.GetCell(string.Format("B{0}", 11 + i));
                    cell.Value = ultraGridEx.ugData.DisplayLayout.Bands[0].Columns[i].Header.Caption;
                    cell = ws.GetCell(string.Format("C{0}", 11 + i));
                    cell.Value = dtBorrowingVolumeData.Rows[0][i].ToString();
                }
                wb.Save(fileName);

            }
        }
    }
}