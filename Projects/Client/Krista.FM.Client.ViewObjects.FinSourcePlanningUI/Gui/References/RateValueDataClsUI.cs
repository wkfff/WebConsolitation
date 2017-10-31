using System;
using System.Collections.Generic;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Commands;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.Client.Workplace.Services;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    public class RateValueDataClsUI : ReferenceUI
    {
        private int refOKV = -1;

        private Dictionary<int, int> currencyCodes;

        public RateValueDataClsUI(IEntity entity, int refOKV)
            : base(entity)
        {
            this.refOKV = refOKV;
        }

        public override void Initialize()
        {
            base.Initialize();

            currencyCodes = new Dictionary<int, int>();

            vo.ugeCls.ugData.AfterCellUpdate += ugData_AfterCellUpdate;

            #region Инициализация панели инструментов
            UltraToolbar utbFinSourcePlanning = new UltraToolbar("FinSourcePlanning");
            utbFinSourcePlanning.DockedColumn = 0;
            utbFinSourcePlanning.DockedRow = 0;
            utbFinSourcePlanning.Text = "FinSourcePlanning";
            utbFinSourcePlanning.Visible = true;
            ((BaseClsView)ViewCtrl).ugeCls.utmMain.Toolbars.AddRange(new UltraToolbar[] { utbFinSourcePlanning });

            CommandService.AttachToolbarTool(new RequestRateExchangeCBRCommand(), utbFinSourcePlanning);

            #endregion Инициализация панели инструментов
        }

        void ugData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (string.Compare(e.Cell.Column.Key, "RefOKV", true) == 0)
            {
                object refOkv = e.Cell.Row.Cells["RefOKV"].Value;
                if (refOkv != null && refOkv != DBNull.Value)
                    e.Cell.Row.Cells["CurrencyCode"].Value = Utils.GetCurrencyCode(refOkv);
            }
        }

        protected override void AddFilter()
        {
            if (refOKV != -1)
                dataQuery += string.Format(" and refOKV = {0}", refOKV);
        }

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            dataQuery = GetDataSourcesFilter();
            AddFilter();
            filterStr = dataQuery;

            return ActiveDataObj.GetDataUpdater(dataQuery, null, null);
        }

       public override void AfterImportDataActions()
        {
            Workplace.ActiveScheme.FinSourcePlanningFace.SetCurrencyRatesReferences();
        }

        protected override void ugeCls_OnClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            base.ugeCls_OnClickCellButton(sender, e);

            if (string.Compare(UltraGridEx.GetSourceColumnName(e.Cell.Column.Key), "RefOKV", true) == 0)
            {
                object refOkv = e.Cell.Row.Cells["RefOKV"].Value;
                if (refOkv != null && refOkv != DBNull.Value)
                    e.Cell.Row.Cells["CurrencyCode"].Value = Utils.GetCurrencyCode(refOkv);
            }
        }
    }
}
