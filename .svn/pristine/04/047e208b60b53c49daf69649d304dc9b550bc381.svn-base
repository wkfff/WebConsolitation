using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Guarantee;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.Guarantees;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public partial class GuaranteeMainPlanWizard : Form
    {
        private DataSet dsCustomPeriod;

        private PayPeriodicity payPeriodicity;

        private GuaranteeServer guaranteeServer;

        private PrincipalContract principalContract;

        public GuaranteeMainPlanWizard(DataRow activeMasterRow)
        {
            InitializeComponent();
            DataRow principalContractRow = GuaranteeServer.GetPrincipalContract(activeMasterRow);
            principalContract = new PrincipalContract(principalContractRow, new Guarantee(activeMasterRow));
            guaranteeServer = GuaranteeServer.GetGuaranteeServer(principalContract.RefOkv, FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme);
            
            int repayDebtMetod = Convert.ToInt32(principalContractRow["RefSRepayDebt"]);
            payPeriodicity = repayDebtMetod == 0 ? PayPeriodicity.Single : PayPeriodicity.Other;

            dsCustomPeriod = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(Int32));
            dt.Columns.Add("StartDate", typeof(DateTime));
            dt.Columns.Add("EndDate", typeof(DateTime));
            dsCustomPeriod.Tables.Add(dt);
            // метод с единовременным погашением долга
            AddEnumItemsToCombo(new PayPeriodicity[6]
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

            InitializeData(principalContract);

            ccEndDate.ValueChanged += new System.EventHandler(this.ParametersPage_ValueChanged);
            ccStartDate.ValueChanged += new System.EventHandler(this.ParametersPage_ValueChanged);
            ucePeriods.ValueChanged += new EventHandler(this.ParametersPage_ValueChanged);

            ultraComboEditor1.SelectedIndex = 31;

            if (payPeriodicity == PayPeriodicity.Single)
            {
                //DateTime endDate = (DateTime)ccEndDate.Value;
                //ccStartDate.Value = endDate.AddMonths(-1);
                nePeriodsCount.Value = 1;
                ultraComboEditor1.SelectedIndex = -1;
                //ucePeriods.Enabled = false;
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

        private void InitializeData(PrincipalContract principalContract)
        {
            DateTime startDate = principalContract.StartDate;

            DateTime endDate = principalContract.EndDate;

            ccStartDate.Value = startDate;
            ccEndDate.Value = endDate;
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
                        payPeriodicity, GetPayDay(), dataSet.Tables[0], true);
                    break;
            }
        }

        private void ParametersPage_ValueChanged(object sender, EventArgs e)
        {
            DateTime startDate = (DateTime)ccStartDate.Value;
            DateTime endDate = (DateTime)ccEndDate.Value;
            PayPeriodicity payPeriod = (PayPeriodicity)Enum.Parse(typeof(PayPeriodicity),
                ucePeriods.SelectedItem.DataValue.ToString());
            ultraComboEditor1.Enabled = payPeriod != PayPeriodicity.Single;
            switch (payPeriod)
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
                    ucePeriods.SelectedIndex = (int)payPeriod - 1;
                    nePeriodsCount.Value = Utils.GetPeriodCount(startDate, endDate,
                        payPeriod, GetPayDay(), dataSet.Tables[0], true);
                    break;
            }
        }

        private void wizard_Next(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            if (e.CurrentPage == wizardParametersPage)
            {
                Server.PayPeriodicity pp = (Server.PayPeriodicity)ucePeriods.SelectedItem.DataValue;

				if (pp != Server.PayPeriodicity.Other)
                {
                    wizardFinalPage.Description2 = CalculateAcquittanceMainPlan();
                    e.Step = 2;
                    wizard.WizardButtons = Common.Wizards.WizardForm.TWizardsButtons.Next;
                }
                else
                {
                    e.Step = 1;
                }
            }
            if (e.CurrentPage == wizardCustomPeriodsPage)
            {
                if ((PayPeriodicity)ucePeriods.SelectedItem.DataValue == PayPeriodicity.Other &&
                    dsCustomPeriod.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("Добавте хотя бы один период", "План погашения основного долга", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Step = 0;
                }
                else
                {
                    wizardFinalPage.Description2 = CalculateAcquittanceMainPlan();
                    wizard.WizardButtons = Common.Wizards.WizardForm.TWizardsButtons.Next;
                }
            }
        }

        private void wizard_Back(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            if (e.CurrentPage.FinishPage)
            {
                PayPeriodicity pp = (PayPeriodicity)ultraComboEditor1.SelectedItem.DataValue;
                e.Step = pp == PayPeriodicity.Other ? 1 : 2;
            }
        }

        private string CalculateAcquittanceMainPlan()
        {
            try
            {
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Формирование плана погашения";

				FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;

                int payDay = GetPayDay();
                guaranteeServer.FillMainPlanTable(principalContract, FinSourcePlanningNavigation.BaseYear, (DateTime)ccStartDate.Value,
                    (DateTime) ccEndDate.Value, (PayPeriodicity) ucePeriods.SelectedItem.DataValue,
                    payDay, dsCustomPeriod.Tables[0]);
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

        private void wizard_Cancel(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventWizardCancelArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void wizard_Finish(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
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
    }
}