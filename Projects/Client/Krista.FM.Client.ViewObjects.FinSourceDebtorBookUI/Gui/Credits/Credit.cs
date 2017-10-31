using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui.Credits
{
    internal class Credit : DebtBookCls
    {
        protected string filter;

        internal Credit(IEntity entity)
            : base(entity)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
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
                String.Format("(SourceID = {0})", DebtBookNavigation.Instance.CurrentSourceID),
                String.Format(" and RefVariant = {0}", DebtBookNavigation.Instance.CurrentVariantID));

            AddFilter();
            filterStr = dataQuery;

            return ActiveDataObj.GetDataUpdater(dataQuery, null, null);
        }

        protected override void AttachDataSources(IEntity obj)
        {
            FixDataSource();
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
