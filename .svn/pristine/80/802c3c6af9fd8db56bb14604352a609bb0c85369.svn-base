using System.Globalization;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class AnnualBalanceF0503130TypeView : AnnualBalanceBaseView
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
            Store store = GetStore(detail.ToString(), new F0503130Fields(), false, "F0503130");
            store.SetBaseParams("section", ((int)detail).ToString(CultureInfo.InvariantCulture), ParameterMode.Raw);
            store.SetWriteBaseParams("section", ((int)detail).ToString(CultureInfo.InvariantCulture), ParameterMode.Raw);
            store.Sort(F0503130Fields.LineCode.ToString(), SortDirection.ASC);
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
                gp.ColumnModel.AddColumn(F0503130Fields.NumberOffBalance.ToString(), AnnualBalanceHelpers.F0503130NameMapping(F0503130Fields.NumberOffBalance, detail), DataAttributeTypes.dtString)
                    .SetWidth(20).SetMaxLengthEdior(3);

                gp.ColumnModel.AddColumn(F0503130Fields.Name.ToString(), AnnualBalanceHelpers.F0503130NameMapping(F0503130Fields.Name, detail), DataAttributeTypes.dtString)
                         .SetEditableString().SetWidth(300).SetMaxLengthEdior(300);

                AddLineCodeColumn(gp, F0503130Fields.LineCode.ToString(), AnnualBalanceHelpers.F0503130NameMapping(F0503130Fields.LineCode, detail));
            }
            else
            {
                gp.Listeners.BeforeEdit.Fn(Scope, "BeforeEdit");

                AddSummBtn(gp, (int)detail);

                gp.ColumnModel.AddColumn(F0503130Fields.Name.ToString(), AnnualBalanceHelpers.F0503130NameMapping(F0503130Fields.Name, detail), DataAttributeTypes.dtString)
                         .SetWidth(300).SetMaxLengthEdior(300);

                AddLineCodeColumn(gp, F0503130Fields.LineCode.ToString(), AnnualBalanceHelpers.F0503130NameMapping(F0503130Fields.LineCode, detail), false, false);
            }
            
            AddDecimalColumn(gp, F0503130Fields.BudgetActivityBegin, detail);
            AddDecimalColumn(gp, F0503130Fields.BudgetActivityEnd, detail);
            AddDecimalColumn(gp, F0503130Fields.IncomeActivityBegin, detail);
            AddDecimalColumn(gp, F0503130Fields.IncomeActivityEnd, detail);
            AddDecimalColumn(gp, F0503130Fields.AvailableMeansBegin, detail);
            AddDecimalColumn(gp, F0503130Fields.AvailableMeansEnd, detail);
            AddDecimalColumn(gp, F0503130Fields.TotalBegin, detail);
            AddDecimalColumn(gp, F0503130Fields.TotalEnd, detail);

            var gridFilters = new GridFilters
            {
                Local = true
            };

            if (detail == F0503130F0503730Details.Information)
            {
                gridFilters.Filters.Add(new StringFilter { DataIndex = F0503130Fields.NumberOffBalance.ToString() });
            }

            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503130Fields.Name.ToString() });
            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503130Fields.LineCode.ToString() });

            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503130Fields.BudgetActivityBegin.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503130Fields.BudgetActivityEnd.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503130Fields.IncomeActivityBegin.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503130Fields.IncomeActivityEnd.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503130Fields.AvailableMeansBegin.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503130Fields.AvailableMeansEnd.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503130Fields.TotalBegin.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503130Fields.TotalEnd.ToString() });

            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private void AddDecimalColumn(GridPanel gp, F0503130Fields field, F0503130F0503730Details detail)
        {
            gp.ColumnModel.AddColumn(
                field.ToString(),
                AnnualBalanceHelpers.F0503130NameMapping(field, detail),
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