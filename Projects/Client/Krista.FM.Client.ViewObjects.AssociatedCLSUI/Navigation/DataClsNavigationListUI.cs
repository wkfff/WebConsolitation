using System;
using System.Drawing;
using Krista.FM.Client.SMO;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Workplace.Gui;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Navigation
{
	public class DataClsNavigationListUI : EntityNavigationListUI
	{
        public DataClsNavigationListUI()
            : base(typeof(DataClsNavigationListUI).FullName)
        {
            Caption = "Классификаторы данных";
        }

		public override Icon Icon
		{
			get { return Icon.FromHandle(Properties.Resources.DataCls.GetHicon()); }
		}

		protected override BaseViewObj CreateBaseClsUI(string key)
		{
		    IEntity dataCls = WorkplaceSingleton.Workplace.ActiveScheme.RootPackage.FindEntityByName(key);
            var variantCls = dataCls as SmoVariantDataClassifier;
            if (variantCls != null)
            {
                return new VariantClsUI(dataCls);
            }

            return new DataCls.DataClsUI(dataCls);
		}

		protected override string GetDataFilter()
		{
			return String.Format("ClassType={0}", (int)ClassTypes.clsDataClassifier);
		}

        protected override void Grid_OnInitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            if (e.Row.Cells.Exists("GoToObject"))
                e.Row.Cells["GoToObject"].ToolTipText = "Перейти на классификатор данных";
        }
	}
}
