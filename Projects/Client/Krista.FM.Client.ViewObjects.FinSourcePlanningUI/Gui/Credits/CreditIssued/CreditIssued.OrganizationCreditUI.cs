using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.CreditIssued;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Credits.CreditIssued
{
    public class OrganizationCredit : CreditIssuedUI
    {
        public OrganizationCredit(IFinSourceBaseService service, string key)
            : base(service, key)
        {
        }

        public override void InitializeData()
        {
            base.InitializeData();
            StateButtonTool tool = (StateButtonTool)((BaseClsView)ViewCtrl).ugeCls.utmMain.Tools["RefRegions" + UltraGridEx.LOOKUP_COLUMN_POSTFIX];
            tool.Checked = false;
        }

        public override void SetTaskId(ref UltraGridRow row)
        {
            row.Cells["RefSTypeCredit"].Value = 4;
            base.SetTaskId(ref row);
        }

        protected override void AddFilter()
        {
            dataQuery += " and RefSTypeCredit = 4";
        }
    }
}
