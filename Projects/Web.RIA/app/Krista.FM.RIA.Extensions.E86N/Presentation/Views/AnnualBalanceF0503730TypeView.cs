using System.Globalization;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class AnnualBalanceF0503730TypeView : AnnualBalanceBaseView
    {
        public override void InitDoc(ViewPage page)
        {
            base.InitDoc(page);
            Details.Add(GetDetail(F0503130F0503730Details.NonfinancialAssets));
            Details.Add(GetDetail(F0503130F0503730Details.FinancialAssets));
            Details.Add(GetDetail(F0503130F0503730Details.Liabilities));
            Details.Add(GetDetail(F0503130F0503730Details.FinancialResult));
            Details.Add(GetDetail(F0503130F0503730Details.Information));
        }

        private GridPanel GetDetail(F0503130F0503730Details detail)
        {
            Store store = GetStore(detail.ToString(), new F0503730Fields(), false);
            store.SetBaseParams("section", ((int)detail).ToString(CultureInfo.InvariantCulture), ParameterMode.Raw);
            store.SetWriteBaseParams("section", ((int)detail).ToString(CultureInfo.InvariantCulture), ParameterMode.Raw);
            store.Sort(F0503730Fields.LineCode.ToString(), SortDirection.ASC);

            Page.Controls.Add(store);

            GridPanel gp = UiBuilders.CreateGridPanel(detail.ToString(), store);
            gp.Title = AnnualBalanceHelpers.F0503130F0503730DetailsNameMapping(detail);
            
            gp.AddRefreshButton();
            if (Auth.IsAdmin() || (detail == F0503130F0503730Details.Information))
            {
                gp.AddNewRecordNoEditButton();
                gp.AddRemoveRecordButton();
            }
            
            gp.AddSaveButton();

            if (detail == F0503130F0503730Details.Information)
            {
                gp.ColumnModel.AddColumn(F0503730Fields.NumberOffBalance.ToString(), AnnualBalanceHelpers.F0503730NameMapping(F0503730Fields.NumberOffBalance, detail), DataAttributeTypes.dtString)
                     .SetWidth(20).SetMaxLengthEdior(3);

                gp.ColumnModel.AddColumn(F0503730Fields.Name.ToString(), AnnualBalanceHelpers.F0503730NameMapping(F0503730Fields.Name, detail), DataAttributeTypes.dtString)
                             .SetEditableString().SetWidth(300).SetMaxLengthEdior(300);

                AddLineCodeColumn(gp, F0503730Fields.LineCode.ToString(), AnnualBalanceHelpers.F0503730NameMapping(F0503730Fields.LineCode, detail));
            }
            else
            {
                gp.Listeners.BeforeEdit.Fn(Scope, "BeforeEdit");

                AddSummBtn(gp, (int)detail);

                gp.ColumnModel.AddColumn(F0503730Fields.Name.ToString(), AnnualBalanceHelpers.F0503730NameMapping(F0503730Fields.Name, detail), DataAttributeTypes.dtString)
                             .SetWidth(300).SetMaxLengthEdior(300);

                AddLineCodeColumn(gp, F0503730Fields.LineCode.ToString(), AnnualBalanceHelpers.F0503730NameMapping(F0503730Fields.LineCode, detail), false, false);
            } 
           
            AddDecimalColumn(gp, F0503730Fields.TargetFundsBegin, detail);
            AddDecimalColumn(gp, F0503730Fields.TargetFundsEnd, detail);
            AddDecimalColumn(gp, F0503730Fields.StateTaskFundStartYear, detail);
            AddDecimalColumn(gp, F0503730Fields.StateTaskFundEndYear, detail);
            AddDecimalColumn(gp, F0503730Fields.RevenueFundsStartYear, detail);
            AddDecimalColumn(gp, F0503730Fields.RevenueFundsEndYear, detail);
            AddDecimalColumn(gp, F0503730Fields.TotalStartYear, detail);
            AddDecimalColumn(gp, F0503730Fields.TotalEndYear, detail);
            AddDecimalColumn(gp, F0503730Fields.ServicesBegin, detail);
            AddDecimalColumn(gp, F0503730Fields.ServicesEnd, detail);
            AddDecimalColumn(gp, F0503730Fields.TemporaryFundsBegin, detail);
            AddDecimalColumn(gp, F0503730Fields.TemporaryFundsEnd, detail);
            
            var gridFilters = new GridFilters
                {
                    Local = true
                };

            if (detail == F0503130F0503730Details.Information)
            {
                gridFilters.Filters.Add(new StringFilter { DataIndex = F0503730Fields.NumberOffBalance.ToString() });
            }

            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503730Fields.Name.ToString() });
            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503730Fields.LineCode.ToString() });

            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503730Fields.TargetFundsBegin.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503730Fields.TargetFundsEnd.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503730Fields.ServicesBegin.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503730Fields.ServicesEnd.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503730Fields.TemporaryFundsBegin.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503730Fields.TemporaryFundsEnd.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503730Fields.StateTaskFundStartYear.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503730Fields.StateTaskFundEndYear.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503730Fields.RevenueFundsStartYear.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503730Fields.RevenueFundsEndYear.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503730Fields.TotalStartYear.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503730Fields.TotalEndYear.ToString() });

            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private void AddDecimalColumn(GridPanel gp, F0503730Fields field, F0503130F0503730Details detail)
        {
            gp.ColumnModel.AddColumn(
                field.ToString(),
                AnnualBalanceHelpers.F0503730NameMapping(field, detail),
                DataAttributeTypes.dtDouble).SetWidth(100)
                .Editor.Add(new NumberField
                {
                    AllowDecimals = true,
                    DecimalPrecision = 2,
                    DecimalSeparator = ",",
                    MaxLength = 20
                });
        }
    }
}