using System;
using System.Data;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using Krista.FM.Client.Common;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Balance;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public partial class CalculatePlanResultWizard : Form
    {
        private const string variantIncomeKey = "1525f07f-8a60-47af-9b80-7200e74956bc";
        private const string variantOutcomeKey = "e8cb8e78-f486-46c1-800f-284eb791d95a";

        public CalculatePlanResultWizard()
        {
            InitializeComponent();

            InfragisticComponentsCustomize.CustomizeUltraGridParams(ultraGridEx.ugData);

            uteVariantIncome.EditorButtonClick += uteVariantIncome_EditorButtonClick;
            uteVariantOutcome.EditorButtonClick += uteVariantOutcome_EditorButtonClick;
            ultraGridEx.OnGetGridColumnsState += ultraGridEx_OnGetGridColumnsState;
            ultraGridEx.AllowDeleteRows = false;
            ultraGridEx.AllowAddNewRecords = false;
            ultraGridEx.ColumnsToolbarVisible = false;
            ultraGridEx.ugData.DisplayLayout.GroupByBox.Hidden = true;
            ultraGridEx.ugData.AfterCellUpdate += ugData_AfterCellUpdate;
            ultraGridEx.utmMain.Visible = false;

            wizard.Cancel += wizard_Cancel;
            wizard.Next += wizard_Next;
            wizard.Back += wizard_Back;
            wizard.Finish += wizard_Finish;
            wizard.WizardClosed += wizard_WizardClosed;
        }

        void ugData_AfterCellUpdate(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            BalanceServer.CalculateBalance(ref dtBalance);
        }

        Components.GridColumnsStates ultraGridEx_OnGetGridColumnsState(object sender)
        {
            Components.GridColumnsStates states = new Components.GridColumnsStates();

            Components.GridColumnState state = new Components.GridColumnState("SourceID");
            state.IsHiden = true;
            states.Add(state);

            state = new Components.GridColumnState("CurrentVariant");
            state.IsHiden = true;
            states.Add(state);

            state = new Components.GridColumnState("IncomesVariant");
            state.IsHiden = true;
            states.Add(state);

            state = new Components.GridColumnState("OutcomesVariant");
            state.IsHiden = true;
            states.Add(state);

            state = new Components.GridColumnState("Incomes");
            state.ColumnCaption = "Доходы";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Components.GridColumnState("Outcomes");
            state.ColumnCaption = "Расходы";
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Components.GridColumnState("Accretion");
            state.IsHiden = true;
            states.Add(state);

            state = new Components.GridColumnState("Recession");
            state.IsHiden = true;
            states.Add(state);

            state = new Components.GridColumnState("RemainsAccretion");
            state.ColumnCaption = "Увеличение остатков средств бюджета";
            state.IsReadOnly = true;
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Components.GridColumnState("RemainsRecession");
            state.ColumnCaption = "Уменьшение остатков средств бюджета";
            state.IsReadOnly = true;
            state.Mask = "-nnnnnnnnnnnnnn.nn";
            states.Add(state);

            state = new Components.GridColumnState("RemainsChange");
            state.ColumnCaption = "Изменение остатков средств на счетах";
            state.IsReadOnly = true;
            state.Mask = "-nnnnnnnnnnnnnn.nn";
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

        private int sourceID;
        private DataTable dtBalance = new DataTable();

        void wizard_Next(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            int currentPageIndex = e.CurrentPage.Index;

            switch (currentPageIndex)
            {
                case 0:
                    wizard.WizardButtons = wizard.WizardButtons &= ~Krista.FM.Client.Common.Wizards.WizardForm.TWizardsButtons.Next;
                    break;
                case 1:
                    wizard.WizardButtons = Krista.FM.Client.Common.Wizards.WizardForm.TWizardsButtons.All;
                    // Расчет параметров
                    sourceID = FinSourcePlanningNavigation.Instance.CurrentSourceID;
                    if (sourceID > -1)
                    {
                        dtBalance = BalanceServer.GetBalanceTable(sourceID, FinSourcePlanningNavigation.BaseYear,
                            FinSourcePlanningNavigation.Instance.CurrentVariantID,
                            Convert.ToInt32(uteVariantIncome.Tag),
                            Convert.ToInt32(uteVariantOutcome.Tag));
                        ultraGridEx.DataSource = dtBalance;
                        ultraGridEx.ugData.DisplayLayout.Bands[0].CardView = true;
                        ultraGridEx.ugData.DisplayLayout.Bands[0].CardSettings.Width = 200;
                    }
                    break;
                case 2:
                    wizard.WizardButtons = Krista.FM.Client.Common.Wizards.WizardForm.TWizardsButtons.All;
                    // Запись результатов в базу
                    if (sourceID > -1)
                    {
                        BalanceServer.SaveData(dtBalance.Rows[0], FinSourcePlanningNavigation.BaseYear);
                        wizardFinalPage.Description2 = "Расчет остатков бюджетных средств успешно добавлен в таблицу";
                        wizard.WizardButtons = Common.Wizards.WizardForm.TWizardsButtons.Next;
                    }
                    else
                    {
                        wizardFinalPage.Description2 = string.Format("Источник данных по {0} году не найден",
                            FinSourcePlanningNavigation.BaseYear);
                    }
                    break;
            }
        }

        private void uteVariantOutcome_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            string variantCaption = uteVariantOutcome.Text;
            int variant = ChooseVariant(variantOutcomeKey, ref variantCaption);
            uteVariantOutcome.Text = variantCaption;
            uteVariantOutcome.Tag = variant != -1 ? (object)variant : null;
            if (uteVariantIncome.Tag != null && uteVariantOutcome.Tag != null)
                wizard.WizardButtons = wizard.WizardButtons |= Krista.FM.Client.Common.Wizards.WizardForm.TWizardsButtons.Next;
        }

        void uteVariantIncome_EditorButtonClick(object sender, EditorButtonEventArgs e)
        {
            string variantCaption = uteVariantIncome.Text;
            int variant = ChooseVariant(variantIncomeKey, ref variantCaption);
            uteVariantIncome.Text = variantCaption;
            uteVariantIncome.Tag = variant != -1 ? (object)variant : null;
            if (uteVariantIncome.Tag != null && uteVariantOutcome.Tag != null)
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
    }
}