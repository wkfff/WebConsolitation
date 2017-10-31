using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public partial class CalculateCurrencyDiffWizard : Form
    {
        private UltraGridRow masterRow;

        private FinSourcePlanningServer finSourcePlanningServer;
        private string errors = string.Empty;

        public CalculateCurrencyDiffWizard(FinSourcePlanningServer finSourcePlanningServer, UltraGridRow activeRow)
        {
            InitializeComponent();

            masterRow = activeRow;
            this.finSourcePlanningServer = finSourcePlanningServer;
            ccStartDate.ValueChanged += ccStartDate_ValueChanged;
            ccEndDate.ValueChanged += ccEndDate_ValueChanged;
            wizard.Cancel += wizard_Cancel;
            wizard.Finish += wizard_Finish;
            wizard.Next += wizard_Next;
            wizard.Back += wizard_Back;
            wizard.WizardClosed += wizard_WizardClosed;
            SetData();
        }

        private void wizard_Next(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            if (e.CurrentPage == wizardParametersPage)
            {
                wizardFinalPage.Description2 = CalculateCurrencyDiff();
            }
        }

        private void wizard_Back(object sender, Common.Wizards.WizardForm.EventNextArgs e)
        {
            
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

        void ccStartDate_ValueChanged(object sender, EventArgs e)
        {
            if (ccStartDate.Value != null)
            {
                int refOKV = Convert.ToInt32(masterRow.Cells["refOKV"].Value);
                decimal currencyExch = Utils.GetLastCurrencyExchange(Convert.ToDateTime(ccStartDate.Value), refOKV);
                if (currencyExch == -1)
                {
                    currencyExch = 0;
                    string.Concat(errors, string.Format("{0} В справочнике курсов валют не было найдено курса за {1}", Environment.NewLine, ccEndDate.Value));
                }
                tbStartExchangeRate.Text = currencyExch.ToString();
            }
        }

        void ccEndDate_ValueChanged(object sender, EventArgs e)
        {
            if (ccEndDate.Value != null)
            {
                int refOKV = Convert.ToInt32(masterRow.Cells["refOKV"].Value);
                decimal currencyExch = Utils.GetLastCurrencyExchange(Convert.ToDateTime(ccEndDate.Value), refOKV);
                if (currencyExch == -1)
                {
                    currencyExch = 0;
                    string.Concat(errors, string.Format("{0} В справочнике курсов валют не было найдено курса за {1}", Environment.NewLine, ccEndDate.Value));
                }
                tbEndExchangeRate.Text = currencyExch.ToString();
            }
        }

        private void SetData()
        {
            ccStartDate.Value = Convert.ToDateTime(masterRow.Cells["StartDate"].Value);
            ccEndDate.Value = Convert.ToDateTime(masterRow.Cells["EndDate"].Value);
            tbStartExchangeRate.Text = masterRow.Cells["ExchangeRate"].Value.ToString();
            Text = string.Format("{0} ({1})", Text, Utils.GetCurrencyName(Convert.ToInt32(masterRow.Cells["RefOKV"].Value)));
        }

        private string CalculateCurrencyDiff()
        {
            DataTable dtFactDebt = finSourcePlanningServer.GetDebtFact(Convert.ToInt32(masterRow.Cells["ID"].Value));
            //DateTime startDate = Convert.ToDateTime(ccStartDate.Value);
            DateTime endDate = Convert.ToDateTime(ccEndDate.Value);

            DataRow[] startRows = dtFactDebt.Select(string.Format("FactDate <= '{0}'", endDate));
            DataRow[] endRows = dtFactDebt.Select(string.Format("FactDate <= '{0}'", endDate));

            decimal startSum = 0;
            foreach (DataRow row in startRows)
            {
                startSum += !row.IsNull("CurrencySum") && !row.IsNull("ExchangeRate") ?
                    Convert.ToDecimal(row["CurrencySum"]) * Convert.ToDecimal(row["ExchangeRate"]) :
                    0;
            }

            decimal endSum = Convert.ToDecimal(masterRow.Cells["CurrencySum"].Value);
            foreach (DataRow row in endRows)
            {
                endSum -= !row.IsNull("CurrencySum") ? Convert.ToDecimal(row["CurrencySum"]) : 0;
            }

            decimal diff = Convert.ToDecimal(masterRow.Cells["CurrencySum"].Value)*
                           Convert.ToDecimal(tbStartExchangeRate.Text) - startSum -
                           (endSum * Convert.ToDecimal(tbEndExchangeRate.Text));
            // полученное значение записываем в базу
            FinSourcePlanningUI content = (FinSourcePlanningUI)WorkplaceSingleton.Workplace.ActiveContent;
            string exchangeDiff = string.Format("{0} - {1}", tbEndExchangeRate.Text, tbStartExchangeRate.Text);
            finSourcePlanningServer.FillCurrencyDiff(diff, exchangeDiff,
                FinSourcePlanningNavigation.BaseYear, Convert.ToDateTime(ccStartDate.Value),
                Convert.ToDateTime(ccEndDate.Value), finSourcePlanningServer.GetCredit(content.GetActiveDataRow()));
            return "Курсовая разница успешно расчитана";
            /*
            StringBuilder sb = new StringBuilder();
            if (Convert.ToDecimal(tbEndExchangeRate.Text) == 0)
            {
                sb.AppendLine("");
            }
            if (Convert.ToDecimal(tbEndExchangeRate.Text) == 0)
            {
                sb.AppendLine("");
            }
            return sb.ToString();*/
        }
    }
}