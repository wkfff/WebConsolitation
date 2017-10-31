using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui.Capital
{
    internal class Capital : DebtBookCls
    {
        protected string filter;

        internal Capital(IEntity entity)
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

        public override void SetPermissionsToClassifier(UltraGridEx gridEx)
        {
            ((BaseClsView)ViewCtrl).ugeCls.ServerFilterEnabled = false;
            ((BaseClsView)ViewCtrl).ugeCls.AllowAddNewRecords = true;
            ((BaseClsView)ViewCtrl).ugeCls.AllowDeleteRows = true;
            ((BaseClsView)ViewCtrl).ugeCls.AllowEditRows = true;
            ((BaseClsView) ViewCtrl).ugeCls.StateRowEnable = true;
            ((BaseClsView) ViewCtrl).ugeCls.IsReadOnly = !DebtBookNavigation.Instance.AllowEditData;
        }
    }
}
