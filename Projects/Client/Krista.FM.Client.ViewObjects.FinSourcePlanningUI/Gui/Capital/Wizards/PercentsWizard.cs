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
    public partial class PercentsWizard : Form
    {
        private Stock stock;

        private int periodsCount;

        public PercentsWizard(DataRow stockRow)
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
            ugeCustomPeriods.AllowClearTable = false;
            ugeCustomPeriods.AllowDeleteRows = false;
            ugeCustomPeriods.DataSource = dataSet;

            neCouponDaysCount.ValueChanged += new EventHandler(neCouponDaysCount_ValueChanged);
            periodsCount = 0;

            InitializeData(stockRow);
        }

        void neCouponDaysCount_ValueChanged(object sender, EventArgs e)
        {
            if (ccStartDate.Value != null && ccEndDate.Value != null && neCouponDaysCount.Value != null)
            {
                if (Convert.ToInt32(neCouponDaysCount.Value) == 0)
                    periodsCount = 0;
                else
                {
                    periodsCount = Convert.ToInt32(
                        Math.Round(
                        Convert.ToDecimal((Convert.ToDateTime(ccEndDate.Value) - Convert.ToDateTime(ccStartDate.Value)).Days /
                        Convert.ToInt32(neCouponDaysCount.Value)), 0, MidpointRounding.AwayFromZero));
                }
                nePeriodsCount.Value = periodsCount;
            }
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
            state.ColumnName = "StartDate";
            state.ColumnCaption = "Дата начала купонного периода";
            state.ColumnWidth = 120;
            states.Add("StartDate", state);

            state = new GridColumnState();
            state.ColumnName = "EndDate";
            state.ColumnCaption = "Дата окончания купонного периода";
            state.ColumnWidth = 120;
            states.Add("EndDate", state);

            state = new GridColumnState();
            state.ColumnName = "DaysCount";
            state.ColumnCaption = "Длительность купонного периода в днях";
            state.Mask = "9999";
            state.ColumnWidth = 120;
            states.Add("DaysCount", state);

            state = new GridColumnState();
            state.ColumnName = "PercentRate";
            state.ColumnCaption = "Купонная ставка, процентов годовых";
            state.Mask = "nnn.nn";
            state.ColumnWidth = 120;
            states.Add("PercentRate", state);

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
            ccEndDate.Value = row["EndDate"];
        }

        private void wizard_Next(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            if (e.CurrentPage == wizardParametersPage)
            {
                if (periodsCount == 0)
                {
                    MessageBox.Show("Количество периодов не указано или равно нулю", "Журнал ставок процентов",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Step = 0;
                    return;
                }
                FillPeriods();
                e.Step = 1;
            }
            if (e.CurrentPage == wizardCustomPeriodsPage)
            {
                if (dataSet.Tables[0].Rows.Count != periodsCount)
                {
                    MessageBox.Show("Количество периодов не соответствует расчитанному количеству", "Журнал ставок процентов",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Step = 0;
                    return;
                }
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    for (int i = 1; i <= dataSet.Tables[0].Columns.Count - 1; i++)
                    {
                        if (row.IsNull(i))
                        {
                            MessageBox.Show(string.Format("Не все значения поля '{0}' указаны", dataSet.Tables[0].Columns[i].Caption),
                                "План погашения номинальной стоимости", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            e.Step = 0;
                            return;
                        }
                    }
                }
                wizardFinalPage.Description2 = CalculatePercenst();
                e.Step = 1;
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
        }

        private void wizard_Back(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            dataSet.Tables[0].Clear();
            if (e.CurrentPage.FinishPage)
                e.Step = 2;
            else
                e.Step = 1;
        }

        private string CalculatePercenst()
        {
            try
            {
                CapitalServer server = new CapitalServer();
                server.FillPercents(stock, dataSet.Tables[0],
                                    FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme);
                return "Расчет журнала процентов завершен";
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

        private void FillPeriods()
        {
            DateTime startDate = Convert.ToDateTime(ccStartDate.Value);
            DateTime endDate = startDate.AddDays(Convert.ToInt32(neCouponDaysCount.Value));

            for (int i = 0; i <= periodsCount -1; i++)
            {
                DataRow newRow = dataSet.Tables[0].NewRow();
                newRow["ID"] = dataSet.Tables[0].Rows.Count + 1;
                newRow["StartDate"] = startDate;
                newRow["EndDate"] = endDate;
                newRow["DaysCount"] = neCouponDaysCount.Value;
                dataSet.Tables[0].Rows.Add(newRow);
                startDate = endDate;
                endDate = startDate.AddDays(Convert.ToInt32(neCouponDaysCount.Value));
            }
            dataSet.Tables[0].Rows[dataSet.Tables[0].Rows.Count - 1]["EndDate"] = ccEndDate.Value;
            dataSet.Tables[0].Rows[dataSet.Tables[0].Rows.Count - 1]["DaysCount"] =
                (Convert.ToDateTime(dataSet.Tables[0].Rows[dataSet.Tables[0].Rows.Count - 1]["EndDate"]) -
                 Convert.ToDateTime(dataSet.Tables[0].Rows[dataSet.Tables[0].Rows.Count - 1]["StartDate"])).Days;
        }
    }
}