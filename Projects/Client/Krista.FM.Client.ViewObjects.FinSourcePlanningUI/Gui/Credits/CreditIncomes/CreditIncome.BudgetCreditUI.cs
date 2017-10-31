using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Services;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits.CreditIncomes
{
    public class BudgetCredit : CreditIncomeUI
    {
        public BudgetCredit(IFinSourceBaseService service, string key)
			: base(service, key)
		{
		}

        public override void SetTaskId(ref UltraGridRow row)
        {
            row.Cells["RefSTypeCredit"].Value = 1;
            base.SetTaskId(ref row);
        }

        protected override void AddFilter()
        {
            dataQuery += " and RefSTypeCredit = 1";
        }
    }
}
