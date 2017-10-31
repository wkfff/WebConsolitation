using System;
using System.Drawing;

using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Workplace.Gui;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Navigation
{
	public class FixedClsNavigationListUI : EntityNavigationListUI
	{
        public FixedClsNavigationListUI()
            : base(typeof(FixedClsNavigationListUI).FullName)
        {
            Caption = "Фиксированные классификаторы";
        }

		public override Icon Icon
		{
			get { return Icon.FromHandle(Properties.Resources.Fixedcls1.GetHicon()); }
		}

		protected override BaseViewObj CreateBaseClsUI(string key)
		{
			return new FixedCls.FixedClsUI(WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindEntityByName(key));
		}

		protected override string GetDataFilter()
		{
			return String.Format("ClassType={0}", (int)ClassTypes.clsFixedClassifier);
		}

        protected override void Grid_OnInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if (e.Row.Cells.Exists("GoToObject"))
                e.Row.Cells["GoToObject"].ToolTipText = "Перейти на фиксированный классификатор";
        }
	}
}
