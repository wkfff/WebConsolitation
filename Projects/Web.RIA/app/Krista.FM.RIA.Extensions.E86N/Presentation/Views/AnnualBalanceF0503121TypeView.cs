using System.Globalization;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.RIA.Extensions.E86N.Utils;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class AnnualBalanceF0503121TypeView : AnnualBalanceBaseView
    {
        public override void InitDoc(ViewPage page)
        {
            base.InitDoc(page);
            Details.Add(GetDetail(F0503121Details.Incomes));
            Details.Add(GetDetail(F0503121Details.Expenses));
            Details.Add(GetDetail(F0503121Details.OperatingResult));
            Details.Add(GetDetail(F0503121Details.OperationNonfinancialAssets));
            Details.Add(GetDetail(F0503121Details.OperationFinancialAssets));
        }

        private GridPanel GetDetail(F0503121Details detail)
        {
            Store store = GetStore(detail.ToString(), new F0503121Fields(), false, "F0503121");
            store.SetBaseParams("section", ((int)detail).ToString(CultureInfo.InvariantCulture), ParameterMode.Raw);
            store.SetWriteBaseParams("section", ((int)detail).ToString(CultureInfo.InvariantCulture), ParameterMode.Raw);
            Page.Controls.Add(store);
            
            GridPanel gp = UiBuilders.CreateGridPanel(detail.ToString(), store);
            gp.Title = AnnualBalanceHelpers.F0503121DetailsNameMapping(detail);

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            AddSummBtn(gp, (int)detail);

            gp.ColumnModel.AddColumn(F0503121Fields.Name.ToString(), AnnualBalanceHelpers.F0503121NameMapping(F0503121Fields.Name, detail), DataAttributeTypes.dtString)
                         .SetEditableString().SetWidth(300).SetMaxLengthEdior(300);

            AddLineCodeColumn(gp, F0503121Fields.LineCode.ToString(), AnnualBalanceHelpers.F0503121NameMapping(F0503121Fields.LineCode, detail));

            gp.ColumnModel.AddColumn(
                detail.ToString() + F0503121Fields.RefKosgyName.ToString(),
                AnnualBalanceHelpers.F0503121NameMapping(F0503121Fields.RefKosgyName, detail),
                DataAttributeTypes.dtString)
                .SetNullable().SetWidth(200).SetHbLookup(D_KOSGY_KOSGY.Key, string.Empty, Scope + ".SelectRowGrid");

            AddDecimalColumn(gp, F0503121Fields.BudgetActivity, detail);
            AddDecimalColumn(gp, F0503121Fields.IncomeActivity, detail);
            AddDecimalColumn(gp, F0503121Fields.AvailableMeans, detail);
            AddDecimalColumn(gp, F0503121Fields.Total, detail);
            
            var gridFilters = new GridFilters
                {
                    Local = true
                };

            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503121Fields.Name.ToString() });
            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503121Fields.LineCode.ToString() });
            gridFilters.Filters.Add(new StringFilter { DataIndex = detail.ToString() + F0503121Fields.RefKosgyName.ToString() });

            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503121Fields.BudgetActivity.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503121Fields.IncomeActivity.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503121Fields.AvailableMeans.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503121Fields.Total.ToString() });
            
            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private void AddDecimalColumn(GridPanel gp, F0503121Fields field, F0503121Details detail)
        {
            gp.ColumnModel.AddColumn(
                field.ToString(),
                AnnualBalanceHelpers.F0503121NameMapping(field, detail),
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