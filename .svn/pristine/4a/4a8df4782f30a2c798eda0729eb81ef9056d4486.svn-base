using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.ServerLibrary;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Domain.Services.FinSourceDebtorBook;

namespace Krista.FM.Client.ViewObjects.FinSourceDebtorBookUI.Gui
{
    class RegionsAccordanceUI : BaseClsUI
    {
        public RegionsAccordanceUI(IEntity entity)
            : base(entity)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            vo.ugeCls.ServerFilterEnabled = false;
            FillData();
            DebtBookNavigation.Instance.VariantChanged += new EventHandler(Instance_VariantChanged);
        }

        void Instance_VariantChanged(object sender, EventArgs e)
        {
            ((BaseClsView)ViewCtrl).ugeCls.BurnRefreshDataButton(true);
        }

        private void FillData()
        {
            // TODO: Либо создавать экземпляры на клиенте (для разработки) либо получать их от вервера (реальная эксплуатация).
            DebtBookNavigation.Instance.Services.RegionsAccordanceService.FillData(DebtBookNavigation.Instance.VariantYear);
        }

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            dataQuery = String.Format("SourceID = {0}", DebtBookNavigation.Instance.CurrentSourceID);

            AddFilter();
            filterStr = dataQuery;

            return ActiveDataObj.GetDataUpdater(dataQuery, null, null);
        }

        public override bool HasDataSources()
        {
            return false;
        }
    }
}
