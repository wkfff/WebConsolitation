using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CreditIssued;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;
using Krista.FM.Client.Reports;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits.CreditIssued
{
    public class BudgetCredit : CreditIssuedUI
    {
        public BudgetCredit(IFinSourceBaseService service, string key)
            : base(service, key)
        {

        }

        public override void InitializeData()
        {
            base.InitializeData();
            StateButtonTool tool = (StateButtonTool)((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools["RefOrganizations" + UltraGridEx.LOOKUP_COLUMN_POSTFIX];
            tool.Checked = false;

            UltraToolbar toolbar = ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars["FinSourcePlanning"];

            ReportMenuParams reportMenuParams = new ReportMenuParams();
            reportMenuParams.tb = toolbar;
            reportMenuParams.tbManager = ((BaseClsView)ViewCtrl).ugeCls.utmMain;
            IFReportMenu reportMenu = new IFReportMenu(reportMenuParams);
            reportMenu.scheme = Workplace.ActiveScheme;
            reportMenu.window = Workplace.WindowHandle;
            reportMenu.operationObj = Workplace.OperationObj;
            reportMenu.IFCreditIssuedReportMenuList();
        }

        public override void SetTaskId(ref UltraGridRow row)
        {
            row.Cells["RefSTypeCredit"].Value = 3;
            base.SetTaskId(ref row);
        }

        protected override void AddFilter()
        {
            dataQuery += " and RefSTypeCredit <> 4";
        }
    }
}
