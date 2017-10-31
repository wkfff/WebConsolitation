using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public partial class AcquittanceMainPlanWizard : Form
    {
        private DataSet dsCustomPeriod;

        private PayPeriodicity payPeriodicity;

        private Credit credit;

        private FinSourcePlanningServer finSourcePlanningServer;

        public AcquittanceMainPlanWizard(FinSourcePlanningServer finSourcePlanningServer,
            UltraGridRow activeRow, PayPeriodicity payPeriodicity, bool hasAttractionFacts)
        {
            this.finSourcePlanningServer = finSourcePlanningServer;

            InitializeComponent();

            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            credit = finSourcePlanningServer.GetCredit(content.GetActiveDataRow());

            this.payPeriodicity = payPeriodicity;

            dsCustomPeriod = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(Int32));
            dt.Columns.Add("StartDate", typeof(DateTime));
            dt.Columns.Add("EndDate", typeof(DateTime));
            dsCustomPeriod.Tables.Add(dt);
            // метод с единовременным погашением долга
            AddEnumItemsToCombo( new PayPeriodicity[6]
            {
                PayPeriodicity.Month, PayPeriodicity.Quarter, PayPeriodicity.HalfYear,
                PayPeriodicity.Year, PayPeriodicity.Single, PayPeriodicity.Other
            });

            ugeCustomPeriods.ugData.DisplayLayout.GroupByBox.Hidden = true;
            ugeCustomPeriods.BorderStyle = BorderStyle.Fixed3D;
            ugeCustomPeriods.OnSaveChanges += new Krista.FM.Client.Components.SaveChanges(ugeCustomPeriods_OnSaveChanges);
            ugeCustomPeriods.OnCancelChanges += new Krista.FM.Client.Components.DataWorking(ugeCustomPeriods_OnCancelChanges);
            ugeCustomPeriods.OnClearCurrentTable += new Krista.FM.Client.Components.DataWorking(ugeCustomPeriods_OnClearCurrentTable);
            ugeCustomPeriods.OnGetGridColumnsState += new Krista.FM.Client.Components.GetGridColumnsState(ugeCustomPeriods_OnGetGridColumnsState);
            ugeCustomPeriods.OnAfterRowInsert += new AfterRowInsert(ugeCustomPeriods_OnAfterRowInsert);
            ugeCustomPeriods.DataSource = dsCustomPeriod;

            if (hasAttractionFacts)
            {
                uceCurrentRemain.SelectedIndex = 0;
            }
            else
            {
                uceCurrentRemain.SelectedIndex = 1;
                uceCurrentRemain.Enabled = false;
            }

            InitializeData();

            ccEndDate.ValueChanged += new System.EventHandler(this.ParametersPage_ValueChanged);
            ccStartDate.ValueChanged += new System.EventHandler(this.ParametersPage_ValueChanged);
            ucePeriods.ValueChanged += new EventHandler(this.ParametersPage_ValueChanged);

            ultraComboEditor1.SelectedIndex = 31;

            if (payPeriodicity == PayPeriodicity.Single)
            {
                nePeriodsCount.Value = 1;
                ultraComboEditor1.SelectedIndex = -1;
            }

            ParametersPage_ValueChanged(null, null);
        }

        /// <summary>
        /// создание списка отображения по всем значениям перечисления
        /// </summary>
        /// <param name="enumType"></param>
        private void AddEnumItemsToCombo(Type enumType)
        {
            ucePeriods.Items.Clear();

            foreach (FieldInfo fi in enumType.GetFields())
            {
                if (fi.IsLiteral)
                {
                    DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(
                        fi, typeof(DescriptionAttribute));

                    Infragistics.Win.ValueListItem valueListItem = new Infragistics.Win.ValueListItem();
                    valueListItem.DataValue = fi.GetRawConstantValue();
                    valueListItem.DisplayText = da != null ? da.Description : fi.Name;
                    ucePeriods.Items.Add(valueListItem);
                }
            }
            ucePeriods.SelectedIndex = 0;
        }

        /// <summary>
        /// создание списка отображения из списка значений перечисления
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumItems"></param>
        private void AddEnumItemsToCombo<T>(IEnumerable<T> enumItems)
        {
            Type enumType = typeof(T);
            // если тип не перечисление, выходим, ничего не делаем
            if (!enumType.IsEnum)
                return;

            foreach (T enumItem in enumItems)
            {
                FieldInfo fldInfo = enumType.GetField(enumItem.ToString());
                DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(
                    fldInfo, typeof(DescriptionAttribute));
                string displayText = da != null ? da.Description : fldInfo.Name;
                ucePeriods.Items.Add(fldInfo.GetRawConstantValue(), displayText);
            }
            ucePeriods.SelectedIndex = 0;
        }


        void ugeCustomPeriods_OnAfterRowInsert(object sender, UltraGridRow row)
        {
            row.Cells["ID"].Value = dsCustomPeriod.Tables[0].Rows.Count + 1;
        }

        GridColumnsStates ugeCustomPeriods_OnGetGridColumnsState(object sender)
        {
            GridColumnsStates states = new GridColumnsStates();

            GridColumnState state = new GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "Номер периода";
            state.ColumnWidth = 100;
            state.IsReadOnly = true;
            states.Add("ID", state);

            state = new GridColumnState();
            state.ColumnName = "StartDate";
            state.ColumnCaption = "Начало периода";
            state.ColumnWidth = 120;
            states.Add("StartDate", state);

            state = new GridColumnState();
            state.ColumnName = "EndDate";
            state.ColumnCaption = "Конец периода";
            state.ColumnWidth = 120;
            states.Add("EndDate", state);

            return states;
        }

        void ugeCustomPeriods_OnClearCurrentTable(object sender)
        {
            dsCustomPeriod.Clear();
            dsCustomPeriod.AcceptChanges();
        }

        void ugeCustomPeriods_OnCancelChanges(object sender)
        {
            dsCustomPeriod.Tables[0].RejectChanges();
        }

        bool ugeCustomPeriods_OnSaveChanges(object sender)
        {
            dsCustomPeriod.Tables[0].AcceptChanges();
            return true;
        }

        private void InitializeData()
        {
            DateTime startDate = uceCurrentRemain.SelectedIndex == 0 ? DateTime.Today : credit.StartDate;
            DateTime endDate = credit.EndDate;

            ccStartDate.Value = startDate;
            if (endDate < DateTime.Today && uceCurrentRemain.SelectedIndex == 0) endDate = DateTime.Today;
            if (endDate.Year > 1)
                ccEndDate.Value = endDate;
            else
                ccEndDate.Value = null;
            switch (payPeriodicity)
            {
                case PayPeriodicity.Single:
                    nePeriodsCount.Value = 1;
                    ucePeriods.SelectedIndex = GetIndex(PayPeriodicity.Single);
                    break;
                case PayPeriodicity.Other:
                    nePeriodsCount.Value = null;
                    ucePeriods.SelectedIndex = GetIndex(PayPeriodicity.Other);
                    break;
                default:
                    ucePeriods.SelectedIndex = (int)payPeriodicity - 1;
                    nePeriodsCount.Value = Utils.GetPeriodCount(startDate, endDate,
                        payPeriodicity, GetPayDay(), dataSet.Tables[0], credit.ChargeFirstDay);
                    break;
            }
        }

        private void ParametersPage_ValueChanged(object sender, EventArgs e)
        {
            if (ccStartDate.Value == null || ccEndDate.Value == null)
            {
                return;
            }
            DateTime startDate = (DateTime)ccStartDate.Value;
            DateTime endDate = (DateTime)ccEndDate.Value;
            PayPeriodicity payPeriod = (PayPeriodicity)Enum.Parse(typeof(PayPeriodicity),
                ucePeriods.SelectedItem.DataValue.ToString());
            ultraComboEditor1.Enabled = payPeriod != PayPeriodicity.Single;
            switch (payPeriod)
            {
                case PayPeriodicity.Single:
                    nePeriodsCount.Value = 1;
                    break;
                case PayPeriodicity.Other:
                    nePeriodsCount.Value = null;
                    break;
                default :
                    nePeriodsCount.Value = Utils.GetPeriodCount(startDate, endDate,
                        payPeriod, GetPayDay(), dataSet.Tables[0], credit.ChargeFirstDay);
                    break;
            }
        }

        private void wizard_Next(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            if (e.CurrentPage == wizardWelcomePage1 &&
                !(credit.CreditsType == CreditsTypes.BudgetIncoming || credit.CreditsType == CreditsTypes.OrganizationIncoming))
            {
                e.Step = 2;
            }

            if (e.CurrentPage == wizardParametersPage)
            {
                PayPeriodicity pp = (PayPeriodicity)ucePeriods.SelectedItem.DataValue;
				if (pp != PayPeriodicity.Other)
                {
                    wizardFinalPage.Description2 = CalculateAcquittanceMainPlan();
                    e.Step = 2;
                    wizard.WizardButtons = Common.Wizards.WizardForm.TWizardsButtons.Next;
                }
                else
                    e.Step = 1;
            }
            if (e.CurrentPage == wizardCustomPeriodsPage)
            {
                if ((PayPeriodicity)ucePeriods.SelectedItem.DataValue == PayPeriodicity.Other)
                {
                    if (dsCustomPeriod.Tables[0].Rows.Count == 0)
                    {
                        MessageBox.Show("Добавте хотя бы один период", "План погашения основного долга",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        e.Step = 0;
                        return;
                    }
                    foreach (DataRow row in dsCustomPeriod.Tables[0].Rows)
                    {
                        if (row.IsNull("StartDate") || row.IsNull("EndDate"))
                        {
                            MessageBox.Show("В периодах не все данные заполнены", "План погашения основного долга",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            e.Step = 0;
                            return;
                        }
                    }
                }
                wizardFinalPage.Description2 = CalculateAcquittanceMainPlan();
                wizard.WizardButtons = Common.Wizards.WizardForm.TWizardsButtons.Next;
            }

            if (e.CurrentPage == wizardPageBase1)
            {
                if (uceVersionName.Enabled && string.IsNullOrEmpty(uceVersionName.Text))
                {
                    MessageBox.Show("Необходимо указать наименование версии расчета плана погашения основного долга", "План обслуживания долга", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    e.Step = 0;
                    return;
                }

                if (dteFormDate.Value == null || dteFormDate.Value == DBNull.Value)
                {
                    MessageBox.Show("Необходимо указать дату формирования плана погашения основного долга", "План обслуживания долга", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    e.Step = 0;
                    return;
                }
            }
        }

        private void wizard_Back(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            if (e.CurrentPage.FinishPage)
            {
                PayPeriodicity pp = (PayPeriodicity)ucePeriods.SelectedItem.DataValue;
                e.Step = pp == PayPeriodicity.Other ? 1 : 2;
            }
            if (e.CurrentPage == wizardParametersPage &&
                !(credit.CreditsType == CreditsTypes.BudgetIncoming || credit.CreditsType == CreditsTypes.OrganizationIncoming))
            {
                e.Step = 2;
            }
        }

        private string CalculateAcquittanceMainPlan()
        {
            try
            {
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Формирование плана погашения";
                finSourcePlanningServer.CalculateAcquittanceMainPlan(
                    credit, GetCalculationParams());
                return "План погашения основного долга успешно заполнен.";
            }
            // TODO: Сделать обработку ошибок
            catch (FinSourcePlanningException ex)
            {
                return ex.Message;
            }
            finally
            {
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
            }
        }

        private MainDebtPaiPlanParams GetCalculationParams()
        {
            var calcParams = new MainDebtPaiPlanParams();
            calcParams.BaseYear = FinSourcePlanningNavigation.BaseYear;
            calcParams.StartDate = Convert.ToDateTime(ccStartDate.Value);
            calcParams.EndDate = Convert.ToDateTime(ccEndDate.Value);
            calcParams.PayPeriodicity = (PayPeriodicity) ucePeriods.SelectedItem.DataValue;
            calcParams.PayDay = GetPayDay();
            calcParams.HasAttractionFacts = uceCurrentRemain.SelectedIndex == 0;
            calcParams.Periods = dsCustomPeriod.Tables[0];
            calcParams.CalculationName = uceVersionName.Text;
            calcParams.FormDate = Convert.ToDateTime(dteFormDate.Value);
            return calcParams;
        }

        private void wizard_Cancel(object sender, Common.Wizards.WizardForm.EventWizardCancelArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void wizard_Finish(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            Close();
        }

        private void wizard_WizardClosed(object sender, EventArgs e)
        {
            Close();
        }

        private void AcquittanceMainPlanWizard_Load(object sender, EventArgs e)
        {
            //DefaultFormState.Load(this);
        }

        private void AcquittanceMainPlanWizard_FormClosed(object sender, FormClosedEventArgs e)
        {
            //DefaultFormState.Save(this);
        }

        private int GetIndex(PayPeriodicity period)
        {
            foreach (Infragistics.Win.ValueListItem item in ucePeriods.Items)
            {
                PayPeriodicity itemPeriod = (PayPeriodicity)Enum.Parse(typeof(PayPeriodicity), item.DataValue.ToString());
                if (itemPeriod == period)
                {
                    ucePeriods.SelectedItem = item;
                    return ucePeriods.SelectedIndex;
                }
            }
            return -1;
        }

        private int GetPayDay()
        {
            return ultraComboEditor1.SelectedItem != null ? Convert.ToInt32(ultraComboEditor1.SelectedItem.DataValue) : 31;
        }

        private void uceCurrentRemain_ValueChanged(object sender, EventArgs e)
        {
            InitializeData();
        }

    }
}