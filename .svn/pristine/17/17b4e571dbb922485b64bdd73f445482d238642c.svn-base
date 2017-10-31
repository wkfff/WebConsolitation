using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Capital;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public partial class NominalValueRepaymentPlanWizard : Form
    {
        private Stock stock;

        public NominalValueRepaymentPlanWizard( DataRow stockRow)
        {
            InitializeComponent();

            stock = new Stock(stockRow);

            ugeCustomPeriods.ugData.DisplayLayout.GroupByBox.Hidden = true;
            ugeCustomPeriods.BorderStyle = BorderStyle.Fixed3D;
            ugeCustomPeriods.OnSaveChanges += new Krista.FM.Client.Components.SaveChanges(ugeCustomPeriods_OnSaveChanges);
            ugeCustomPeriods.OnCancelChanges += new Krista.FM.Client.Components.DataWorking(ugeCustomPeriods_OnCancelChanges);
            ugeCustomPeriods.OnClearCurrentTable += new Krista.FM.Client.Components.DataWorking(ugeCustomPeriods_OnClearCurrentTable);
            ugeCustomPeriods.OnGetGridColumnsState += new Krista.FM.Client.Components.GetGridColumnsState(ugeCustomPeriods_OnGetGridColumnsState);
            ugeCustomPeriods.OnAfterRowInsert += new AfterRowInsert(ugeCustomPeriods_OnAfterRowInsert);
            ugeCustomPeriods.AllowAddNewRecords = false;
            ugeCustomPeriods.AllowDeleteRows = false;
            ugeCustomPeriods.AllowClearTable = false;
            ugeCustomPeriods.DataSource = dataSet;

            ucePeriods.ValueChanged += new EventHandler(ucePeriods_ValueChanged);

            InitializeData(stockRow);
        }

        void ucePeriods_ValueChanged(object sender, EventArgs e)
        {
            nePeriodsCount.Visible = ucePeriods.SelectedIndex == 1;
            ultraLabel7.Visible = ucePeriods.SelectedIndex == 1;
            ultraLabel8.Visible = ucePeriods.SelectedIndex == 1;
            ucbPeriodsParams.Visible = ucePeriods.SelectedIndex == 1;
        }

        void ugeCustomPeriods_OnAfterRowInsert(object sender, UltraGridRow row)
        {
            row.Cells["ID"].Value = dataSet.Tables[0].Rows.Count + 1;
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
            state.ColumnName = "PayDate";
            state.ColumnCaption = "Дата выплаты";
            state.ColumnWidth = 120;
            states.Add("PayDate", state);

            state = new GridColumnState();
            state.ColumnName = "DaysCount";
            state.ColumnCaption = "Количество дней";
            state.ColumnWidth = 120;
            state.Mask = "9999";
            states.Add("DaysCount", state);

            state = new GridColumnState();
            state.ColumnName = "Percent";
            state.ColumnCaption = "Процент";
            state.Mask = "nnn.nn";
            state.ColumnWidth = 120;
            states.Add("Percent", state);

            return states;
        }

        void ugeCustomPeriods_OnClearCurrentTable(object sender)
        {
            dataSet.Clear();
            dataSet.AcceptChanges();
        }

        void ugeCustomPeriods_OnCancelChanges(object sender)
        {
            dataSet.Tables[0].RejectChanges();
        }

        bool ugeCustomPeriods_OnSaveChanges(object sender)
        {
            dataSet.Tables[0].AcceptChanges();
            return true;
        }

        private void InitializeData(DataRow row)
        {
            ccStartDate.Value = row["StartDate"];
            ccEndDate.Value = row["DateDischarge"];
            ucePeriods.SelectedIndex = 0;
            ucbPeriodsParams.SelectedIndex = 0;
        }

        private void wizard_Next(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            if (e.CurrentPage == wizardParametersPage)
            {
                if (ucePeriods.SelectedIndex == 0)
                {
                    wizardFinalPage.Description2 = CalculatePlan();
                    e.Step = 2;
                    wizard.WizardButtons = Common.Wizards.WizardForm.TWizardsButtons.Next;
                }
                else
                {
                    if (nePeriodsCount.Value == null || nePeriodsCount.Value == DBNull.Value || Convert.ToInt32(nePeriodsCount.Value) == 0)
                    {
                        MessageBox.Show("Количество амортизационных выплат должно быть больше нуля", "План погашения номинальной стоимости",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        e.Step = 0;
                        return;
                    }
                    SetPeriodsData(Convert.ToInt32(nePeriodsCount.Value));
                    e.Step = 1;
                }
            }
            if (e.CurrentPage == wizardCustomPeriodsPage)
            {
                if (dataSet.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("Количество амортизационных выплат должно быть больше нуля", "План погашения номинальной стоимости",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Step = 0;
                    return;
                }

                decimal percents = 0;
                if (ugeCustomPeriods.ugData.ActiveRow != null) 
                    ugeCustomPeriods.ugData.ActiveRow.Update();

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    if (ucbPeriodsParams.SelectedIndex == 0 && row.IsNull("PayDate"))
                    {
                        MessageBox.Show("Не все даты выплаты указаны", "План погашения номинальной стоимости",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        e.Step = 0;
                        return;
                    }
                    if (ucbPeriodsParams.SelectedIndex == 1 && row.IsNull("DaysCount"))
                    {
                        MessageBox.Show("Не во всех выплатах указано количество дней", "План погашения номинальной стоимости",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        e.Step = 0;
                        return;
                    }

                    if (row.IsNull("Percent"))
                    {
                        MessageBox.Show("Не во всех выплатах указаны проценты", "План погашения номинальной стоимости",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        e.Step = 0;
                        return;
                    }

                    percents += Convert.ToDecimal(row["Percent"]);
                }

                if (percents != 100)
                {
                    MessageBox.Show("Сумма процентов должна равняться 100", "План погашения номинальной стоимости",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Step = 0;
                    return;
                }

                wizardFinalPage.Description2 = CalculatePlan();
                wizard.WizardButtons = Common.Wizards.WizardForm.TWizardsButtons.Next;
            }
        }

        private void SetPeriodsData(int periodsCount)
        {
            dataSet.Tables[0].BeginLoadData();
            for (int i = 0; i <= periodsCount - 1; i++)
            {
                DataRow row = dataSet.Tables[0].NewRow();
                row["ID"] = dataSet.Tables[0].Rows.Count + 1;
                dataSet.Tables[0].Rows.Add(row);
            }
            ugeCustomPeriods.DataSource = dataSet;

            ugeCustomPeriods.ugData.DisplayLayout.Bands[0].Columns[1].Hidden = ucbPeriodsParams.SelectedIndex == 1;
            ugeCustomPeriods.ugData.DisplayLayout.Bands[0].Columns[2].Hidden = ucbPeriodsParams.SelectedIndex == 0;
        }

        private void wizard_Back(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            dataSet.Tables[0].Clear();
            if (e.CurrentPage.FinishPage)
            {
                PayPeriodicity pp = (PayPeriodicity)ucePeriods.SelectedItem.DataValue;
                e.Step = pp == PayPeriodicity.Other ? 1 : 2;
            }
        }

        private string CalculatePlan()
        {
            try
            {
                CapitalServer.FillNominalValueRepaymentPlan(stock, Convert.ToDateTime(ccStartDate.Value), dataSet.Tables[0],
                                                        FinSourcePlanningNavigation.Instance.CurrentSourceID,
                                                        FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme);
                return "Расчет плана погашения номинальной стоимости успешно завершен";
            }
            catch (Exception e)
            {
                return e.Message;
            }
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
    }
}