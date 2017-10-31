using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.ConsBudgetForecastUI.GUI.TaxpayersSum
{
    public class MunicipalsUI : DataClsUI
    {
        private int SourceId
        {
            get; set;
        }

        internal MunicipalsUI(IEntity entity, int sourceId)
            : base(entity)
        {
            SourceId = sourceId;
        }

        public override bool HasDataSources()
        {
            return false;
        }

        public override void UpdateToolbar()
        {
            base.UpdateToolbar();
            vo.utbToolbarManager.Visible = false;
            vo.ugeCls.utmMain.Visible = false;
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            AdditionalFilter = string.Format(" and (RefTerr = 5 or RefTerr = 6 or RefTerr = 7) and SourceID = {0}", SourceId);
            base.LoadData(sender, e);
            vo.ugeCls.DataSource = dsObjData;
            vo.ugeCls.IsReadOnly = true;
            vo.ugeCls.ugData.DisplayLayout.AddNewBox.Hidden = true;
            vo.ugeCls.ugData.DisplayLayout.GroupByBox.Hidden = true;
            vo.utbToolbarManager.Visible = false;
            vo.ugeCls.utmMain.Visible = false;
            vo.ugeCls.ugData.DisplayLayout.GroupByBox.Hidden = true;
            vo.ugeCls.ugFilter.DisplayLayout.GroupByBox.Hidden = true;
            vo.ugeCls.ugData.DisplayLayout.Bands[0].Columns[UltraGridEx.StateColumnName].Hidden = true;
        }

        public override HierarchyInfo GetHierarchyInfoFromClsObject(IEntity clsObject, UltraGridEx gridEx)
        {
            HierarchyInfo info = base.GetHierarchyInfoFromClsObject(clsObject, gridEx);
            info.ParentClmnName = string.Empty;
            info.ParentRefClmnName = string.Empty;
            info.LevelsCount = 1;
            info.loadMode = LoadMode.OnFlatScroll;
            return info;
        }
    }
}
