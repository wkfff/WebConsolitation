using System;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.ServerLibrary;
using System.Data;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui
{
    internal class RegionClsUI : DataClsUI
    {
        internal RegionClsUI(IEntity entity)
            : base(entity)
        {
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            base.LoadData(sender, e);

            foreach (DataRow row in dsObjData.Tables[0].Rows)
                row["ParentID"] = DBNull.Value;
        }

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            // запрашиваем данные по району, к которому принадлежит пользователь
            string selectDataSourceQuery = string.Format("select id from DataSources where SupplierCode = 'ФО' and DataCode = 6 and DataName = 'Анализ данных' and Year = {0}", DebtBookNavigation.Instance.VariantYear);
            switch (DebtBookNavigation.Instance.UserRegionType)
            {
                case UserRegionType.Region:
                case UserRegionType.Town:
                    dataQuery = string.Format(" (ParentID IN (select id from {1} where ParentID = {0})) and SourceID = ({2})",
                        DebtBookNavigation.Instance.CurrentRegion, ActiveDataObj.FullDBName, selectDataSourceQuery);
                    break;
                case UserRegionType.Settlement:
                    dataQuery = string.Format(" ID = {0}", DebtBookNavigation.Instance.CurrentRegion);
                    break;
                case UserRegionType.Subject:
                    dataQuery = string.Format(" (RefTerr = 4 or RefTerr = 7) and SourceID = ({0})",
                        selectDataSourceQuery);
                    break;
            }
            

            filterStr = dataQuery;

            return ActiveDataObj.GetDataUpdater(dataQuery, null, null);
        }

        public override void UpdateToolbar()
        {
            vo.utbToolbarManager.Toolbars["utbFilters"].Visible = false;
            vo.utbToolbarManager.Toolbars["SelectTask"].Visible = false;
            vo.ugeCls.utmMain.Tools["menuSave"].SharedProps.Enabled = true;
        }
    }
}
