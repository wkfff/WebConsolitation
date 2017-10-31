using System.Globalization;
using System.Web.Mvc;

using Ext.Net;

using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Extensions;
using Krista.FM.RIA.Extensions.E86N.Models.AnnualBalance;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controllers.RestControllers;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class AnnualBalanceF0503721TypeView : AnnualBalanceBaseView
    {
        private readonly AnnualBalanceF0503721ViewModel annualBalanceF0503721ViewModel = new AnnualBalanceF0503721ViewModel();

        public override void InitDoc(ViewPage page)
        {
            base.InitDoc(page);
            Details.Add(GetDetail(F0503721Details.Incomes));
            Details.Add(GetDetail(F0503721Details.Expenses));
            Details.Add(GetDetail(F0503721Details.NonFinancialAssets));
            Details.Add(GetDetail(F0503721Details.FinancialAssetsLiabilities));
        }

        private GridPanel GetDetail(F0503721Details detail)
        {
             Store store = StoreExtensions.StoreUrlCreateDefault(
                 detail + "Store",
                 false,
                 UiBuilders.GetUrl<AnnualBalanceViewController>("F0503721Read"),
                 UiBuilders.GetUrl<DocCommonController>("SaveAction"),
                 UiBuilders.GetUrl<DocCommonController>("SaveAction"),
                 UiBuilders.GetUrl<DocCommonController>("DeleteAction"));

            store.SetBaseParams("docId", DocId.ToString(), ParameterMode.Raw);
            store.SetWriteBaseParams("docId", DocId.ToString(), ParameterMode.Raw);

            store.SetBaseParams("modelType", annualBalanceF0503721ViewModel.GetType().AssemblyQualifiedName, ParameterMode.Value);
            store.SetWriteBaseParams("modelType", annualBalanceF0503721ViewModel.GetType().AssemblyQualifiedName, ParameterMode.Value);

            store.SetBaseParams("section", ((int)detail).ToString(CultureInfo.InvariantCulture), ParameterMode.Raw);

            store.AddField(UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.ID));

            store.AddField(UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.RefParent), DocId.ToString());
            store.AddField(UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.Section), ((int)detail).ToString());
             
            store.AddField(UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.Name));
            store.AddField(UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.LineCode));
            store.AddField(UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.AnalyticCode));
            store.AddField(UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.TargetFunds));
            store.AddField(UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.Services));
            store.AddField(UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.TemporaryFunds));
            store.AddField(UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.StateTaskFunds));
            store.AddField(UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.RevenueFunds));
            store.AddField(UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.Total));
            
            Page.Controls.Add(store);
            
            GridPanel gp = UiBuilders.CreateGridPanel(detail.ToString(), store);
            gp.Title = AnnualBalanceHelpers.F0503721DetailsNameMapping(detail);

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            AddSummBtn(gp, (int)detail);

            gp.ColumnModel.AddColumn(() => annualBalanceF0503721ViewModel.Name);

            AddLineCodeColumn(gp, UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.LineCode), UiBuilders.DescriptionOf(() => annualBalanceF0503721ViewModel.LineCode));
          
            gp.ColumnModel.AddColumn(() => annualBalanceF0503721ViewModel.AnalyticCode);
            gp.ColumnModel.AddColumn(() => annualBalanceF0503721ViewModel.TargetFunds);
            gp.ColumnModel.AddColumn(() => annualBalanceF0503721ViewModel.StateTaskFunds);
            gp.ColumnModel.AddColumn(() => annualBalanceF0503721ViewModel.RevenueFunds);
            gp.ColumnModel.AddColumn(() => annualBalanceF0503721ViewModel.Total);
            gp.ColumnModel.AddColumn(() => annualBalanceF0503721ViewModel.Services);
            gp.ColumnModel.AddColumn(() => annualBalanceF0503721ViewModel.TemporaryFunds);
            
            var gridFilters = new GridFilters
                {
                    Local = true
                };

            gridFilters.Filters.Add(new StringFilter { DataIndex = UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.Name) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.LineCode) });
            gridFilters.Filters.Add(new StringFilter { DataIndex = UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.AnalyticCode) });

            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.TargetFunds) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.Services) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.TemporaryFunds) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.StateTaskFunds) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.RevenueFunds) });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = UiBuilders.NameOf(() => annualBalanceF0503721ViewModel.Total) });
            
            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }
    }
}