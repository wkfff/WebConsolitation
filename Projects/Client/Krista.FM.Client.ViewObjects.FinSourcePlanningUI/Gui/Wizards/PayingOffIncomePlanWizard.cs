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
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Capital;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public partial class PayingOffIncomePlanWizard : Form
    {
        private DataSet dsCustomPeriod;

        private PayPeriodicity payPeriodicity;

        private CapitalServer server;

        private Credit credit;

        public PayingOffIncomePlanWizard(CapitalServer server, UltraGridRow activeRow, PayPeriodicity payPeriodicity)
        {
            InitializeComponent();

            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            credit = new Credit(content.GetActiveDataRow());

            this.payPeriodicity = payPeriodicity;
            this.server = server;

            dsCustomPeriod = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(Int32));
            dt.Columns.Add("StartDate", typeof(DateTime));
            dt.Columns.Add("EndDate", typeof(DateTime));
            dsCustomPeriod.Tables.Add(dt);
            // метод с единовременным погашением долга

            AddEnumItemsToCombo(typeof(Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server.PayPeriodicity));

            ugeCustomPeriods.ugData.DisplayLayout.GroupByBox.Hidden = true;
            ugeCustomPeriods.BorderStyle = BorderStyle.Fixed3D;
            ugeCustomPeriods.OnSaveChanges += new Krista.FM.Client.Components.SaveChanges(ugeCustomPeriods_OnSaveChanges);
            ugeCustomPeriods.OnCancelChanges += new Krista.FM.Client.Components.DataWorking(ugeCustomPeriods_OnCancelChanges);
            ugeCustomPeriods.OnClearCurrentTable += new Krista.FM.Client.Components.DataWorking(ugeCustomPeriods_OnClearCurrentTable);
            ugeCustomPeriods.OnGetGridColumnsState += new Krista.FM.Client.Components.GetGridColumnsState(ugeCustomPeriods_OnGetGridColumnsState);
            ugeCustomPeriods.OnAfterRowInsert += new AfterRowInsert(ugeCustomPeriods_OnAfterRowInsert);
            ugeCustomPeriods.DataSource = dsCustomPeriod;

            InitializeData(activeRow);

            ccEndDate.ValueChanged += new System.EventHandler(ParametersPage_ValueChanged);
            ccStartDate.ValueChanged += new System.EventHandler(ParametersPage_ValueChanged);
            ucePeriods.ValueChanged += new EventHandler(ParametersPage_ValueChanged);
            uneDayCount.ValueChanged += new EventHandler(ParametersPage_ValueChanged);

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

        private void InitializeData(UltraGridRow row)
        {
            DateTime startDate = (DateTime)row.Cells["StartDate"].Value;
            
			DateTime endDate = (DateTime)row.Cells["EndDate"].Value;

            ccStartDate.Value = startDate;
            ccEndDate.Value = endDate;

            int dayCount = (endDate - startDate).Days; 
            double yearLength = Math.Round(dayCount / (double)365, 2, MidpointRounding.AwayFromZero);
            if (payPeriodicity == PayPeriodicity.Single)
            {
                nePeriodsCount.Value = 1;
                ucePeriods.SelectedIndex = GetIndex(PayPeriodicity.Single);
            }
            else
            {
                if (yearLength <= 1)
                {
                    // Краткосрочный
                    ucePeriods.SelectedIndex = GetIndex(PayPeriodicity.Month);
                }
                else if (yearLength > 1 && yearLength <= 5)
                {
                    // Среднесрочный
                    ucePeriods.SelectedIndex = GetIndex(PayPeriodicity.Quarter);
                }
                else if (yearLength > 5 && yearLength <= 30)
                {
                    // Долгосрочный
                    ucePeriods.SelectedIndex = GetIndex(PayPeriodicity.HalfYear);
                }

                nePeriodsCount.Value = Utils.GetPeriodCount(startDate, endDate,
                    (PayPeriodicity)Enum.Parse(typeof(PayPeriodicity),
                    (ucePeriods.SelectedIndex).ToString()), dataSet.Tables[0]);
            }
        }

        private void ParametersPage_ValueChanged(object sender, EventArgs e)
        {
            DateTime startDate = (DateTime)ccStartDate.Value;
            DateTime endDate = (DateTime)ccEndDate.Value;
            PayPeriodicity payPeriod = (PayPeriodicity)Enum.Parse(typeof(PayPeriodicity),
                ucePeriods.SelectedItem.DataValue.ToString());

            uneDayCount.Enabled = payPeriod == PayPeriodicity.Day;
            if (payPeriod == PayPeriodicity.Single)
            {
                ccStartDate.Value = endDate.AddMonths(-1);
                nePeriodsCount.Value = 1;
            }
            else nePeriodsCount.Value = payPeriod == PayPeriodicity.Day ?
                Utils.GetPeriodCount(startDate, endDate, payPeriod, dataSet.Tables[0], Convert.ToInt32(uneDayCount.Value)) :
                Utils.GetPeriodCount(startDate, endDate, payPeriod, 31, dataSet.Tables[0], true);
        }

        private void wizard_Next(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            if (e.CurrentPage == wizardParametersPage)
            {
                PayPeriodicity pp = (PayPeriodicity)ucePeriods.SelectedItem.DataValue;

				if (pp != PayPeriodicity.Other)
                {
                    wizardFinalPage.Description2 = CalculateAcquittanceMainPlan();

                    e.Step = 2;
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
                    e.Step = 1;
                }
                else
                    wizardFinalPage.Description2 = CalculateAcquittanceMainPlan();
            }
        }

        private void wizard_Back(object sender, Krista.FM.Client.Common.Wizards.WizardForm.EventNextArgs e)
        {
            if (e.CurrentPage.FinishPage)
            {
                Server.PayPeriodicity pp = (Server.PayPeriodicity)ucePeriods.SelectedItem.DataValue;
				if (pp != Server.PayPeriodicity.Other)
                {
                    e.Step = 2;
                }
            }
        }

        private string CalculateAcquittanceMainPlan()
        {
            try
            {
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
                FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Формирование плана выплаты дохода";

				FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;

                return "План выплаты дохода успешно заполнен.";
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
    }
}