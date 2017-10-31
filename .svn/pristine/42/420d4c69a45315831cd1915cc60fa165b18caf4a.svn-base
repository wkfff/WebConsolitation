using System;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Components;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui.Guarantee
{
    internal class Guarantee : DebtBookCls
    {
        protected string filter;

        internal Guarantee(IEntity entity)
            : base(entity)
        {
        }

        public override bool HasDataSources()
        {
            return true;
        }

        public override void SetTaskId(ref UltraGridRow row)
        {
            row.Cells["RefVariant"].Value = DebtBookNavigation.Instance.CurrentVariantID;
            row.Cells["SourceID"].Value = DebtBookNavigation.Instance.CurrentSourceID;
            if (DebtBookNavigation.Instance.CurrentRegion != -1)
                row.Cells["RefRegion"].Value = DebtBookNavigation.Instance.CurrentRegion;
            row.Cells["TASKID"].Value = -1;
        }

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            dataQuery = String.Concat(
                GetDataSourcesFilter(),
                String.Format(" and RefVariant = {0}", DebtBookNavigation.Instance.CurrentVariantID));

            AddFilter();
            filterStr = dataQuery;
            return ActiveDataObj.GetDataUpdater(dataQuery, null, null);
        }

        protected override void AttachDataSources(IEntity obj)
        {
            FixDataSource();
        }

        public override void UpdateToolbar()
        {
            vo.utbToolbarManager.Toolbars["utbFilters"].Visible = false;
            vo.utbToolbarManager.Toolbars["SelectTask"].Visible = false;
            vo.ugeCls.utmMain.Tools["menuSave"].SharedProps.Enabled = true;
        }

        public override void SetPermissionsToClassifier(UltraGridEx gridEx)
        {
            ((BaseClsView)ViewCtrl).ugeCls.ServerFilterEnabled = false;
            ((BaseClsView)ViewCtrl).ugeCls.AllowAddNewRecords = allowAddRecord;
            ((BaseClsView)ViewCtrl).ugeCls.AllowDeleteRows = true;
            ((BaseClsView)ViewCtrl).ugeCls.AllowEditRows = true;
            ((BaseClsView)ViewCtrl).ugeCls.StateRowEnable = true;
            ((BaseClsView)ViewCtrl).ugeCls.IsReadOnly = !DebtBookNavigation.Instance.AllowEditData;
        }

        public override void CheckClassifierPermissions()
        {
            base.CheckClassifierPermissions();
            allowEditRecords = um.CheckPermissionForSystemObject(currentClassifierName, (int)AllFactTablesOperations.EditRecord, false);
            if (!allowEditRecords)
                allowEditRecords = um.CheckPermissionForSystemObject(currentClassifierName, (int)FactTableOperations.EditRecord, false);
        }
    }
}
