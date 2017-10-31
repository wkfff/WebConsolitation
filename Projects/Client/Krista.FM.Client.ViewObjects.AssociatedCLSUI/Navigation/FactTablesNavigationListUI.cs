using System.Drawing;

using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Workplace.Gui;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Navigation
{
	public class FactTablesNavigationListUI : EntityNavigationListUI
	{
        public FactTablesNavigationListUI()
            : base(typeof(FactTablesNavigationListUI).FullName)
        {
            Caption = "Таблицы фактов";
        }

		public override Icon Icon
		{
			get { return Icon.FromHandle(Properties.Resources.Facttable.GetHicon()); }
		}

		protected override BaseViewObj CreateBaseClsUI(string key)
		{
			return new FactTables.FactTablesUI(WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindEntityByName(key));
		}

		protected override System.Data.DataTable GetNavigationDataTable()
		{
			return WorkplaceSingleton.Workplace.ActiveScheme.FactTables.GetDataTable();
		}

        protected override void Grid_OnInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if (e.Row.Cells.Exists("GoToObject"))
                e.Row.Cells["GoToObject"].ToolTipText = "Перейти на таблицу фактов";
        }
	}
}
