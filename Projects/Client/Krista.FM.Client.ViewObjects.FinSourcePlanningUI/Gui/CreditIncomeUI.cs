using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary.FinSourcePlanning;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public class CreditIncomeUI : FinSourcePlanningUI
    {
        public CreditIncomeUI(FinSourcePlanningServiceTypes serviceType, IFinSourceBaseService service)
            : base(serviceType, service)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            UltraToolbar utbCreditIncome = new UltraToolbar("utbCreditIncome");
            utbCreditIncome.DockedColumn = 0;
            utbCreditIncome.DockedRow = 0;
            utbCreditIncome.Text = "utbCreditIncome";
            utbCreditIncome.Visible = true;

            ButtonTool btnCalcAcquittanceMainPlan = new ButtonTool("btnCalcAcquittanceMainPlan");
            btnCalcAcquittanceMainPlan.SharedProps.Caption = "Рассчитать план погашения основного долга";
            utbCreditIncome.NonInheritedTools.AddRange(new ToolBase[] { btnCalcAcquittanceMainPlan });

            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars.AddRange(new UltraToolbar[] { utbCreditIncome });

            btnCalcAcquittanceMainPlan.ToolClick += new ToolClickEventHandler(btnCalcAcquittanceMainPlan_ToolClick);
        }

        /// <summary>
        /// Вычистение плана погашения основного долга.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalcAcquittanceMainPlan_ToolClick(object sender, ToolClickEventArgs e)
        {
            //CalcAcquittanceMainPlan.Calculate();
        }
    }
}
