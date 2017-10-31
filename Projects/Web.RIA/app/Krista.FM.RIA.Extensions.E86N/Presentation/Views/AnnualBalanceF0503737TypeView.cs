using System.Globalization;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls.SearchCombobox;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class AnnualBalanceF0503737TypeView : AnnualBalanceBaseView
    {
        public override void InitDoc(ViewPage page)
        {
            IsVisibleField = true;
            base.InitDoc(page);
            Details.Add(GetDetail(F0503737Details.Incomes));
            Details.Add(GetDetail(F0503737Details.Expenses));
            Details.Add(GetDetail(F0503737Details.SourcesOfFinancing));
            Details.Add(GetDetail(F0503737Details.ReturnExpense));
        }

        private GridPanel GetDetail(F0503737Details detail)
        {
            Store store = GetStore(detail.ToString(), new F0503737Fields(), false, "F0503737");
            store.SetBaseParams("section", ((int)detail).ToString(CultureInfo.InvariantCulture), ParameterMode.Raw);
            store.SetWriteBaseParams("section", ((int)detail).ToString(CultureInfo.InvariantCulture), ParameterMode.Raw);
            Page.Controls.Add(store);
            
            GridPanel gp = UiBuilders.CreateGridPanel(detail.ToString(), store);
            gp.Title = AnnualBalanceHelpers.F0503737DetailsNameMapping(detail);

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn(F0503737Fields.Name.ToString(), AnnualBalanceHelpers.F0503737NameMapping(F0503737Fields.Name, detail), DataAttributeTypes.dtString)
                         .SetEditableString().SetWidth(300).SetMaxLengthEdior(300);

            AddLineCodeColumn(gp, F0503737Fields.LineCode.ToString(), AnnualBalanceHelpers.F0503737NameMapping(F0503737Fields.LineCode, detail));
            
            gp.ColumnModel.AddColumn(F0503737Fields.AnalyticCode.ToString(), AnnualBalanceHelpers.F0503737NameMapping(F0503737Fields.AnalyticCode, detail), DataAttributeTypes.dtString)
                .SetNullable().SetEditableString().SetWidth(50).SetMaxLengthEdior(3);

            gp.ColumnModel.AddColumn(
                F0503737Fields.RefTypeFinSupportName.ToString(),
                AnnualBalanceHelpers.F0503737NameMapping(F0503737Fields.RefTypeFinSupportName, detail),
                DataAttributeTypes.dtString)
                       .SetWidth(200)
                .AddLookupEditorForColumn(
                    F0503737Fields.RefTypeFinSupport.ToString(),
                    F0503737Fields.RefTypeFinSupportName.ToString(),
                    "/Entity/DataWithCustomSearch?objectKey={0}{1}".FormatWith(FX_FX_typeFinSupport.Key, "&start=0&limit=-1"),
                    true,
                    Page);

            AddDecimalColumn(gp, F0503737Fields.ApprovePlanAssign, detail);
            AddDecimalColumn(gp, F0503737Fields.ExecPersonAuthorities, detail);
            AddDecimalColumn(gp, F0503737Fields.ExecBankAccounts, detail);
            AddDecimalColumn(gp, F0503737Fields.ExecNonCashOperation, detail);
            AddDecimalColumn(gp, F0503737Fields.ExecCashAgency, detail);
            AddDecimalColumn(gp, F0503737Fields.ExecTotal, detail);
            AddDecimalColumn(gp, F0503737Fields.UnexecPlanAssign, detail);
           
            var gridFilters = new GridFilters
                {
                    Local = true
                };

            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503737Fields.Name.ToString() });
            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503737Fields.LineCode.ToString() });
            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503737Fields.AnalyticCode.ToString() });
            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503737Fields.RefTypeFinSupportName.ToString() });

            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503737Fields.ApprovePlanAssign.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503737Fields.ExecPersonAuthorities.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503737Fields.ExecBankAccounts.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503737Fields.ExecNonCashOperation.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503737Fields.ExecCashAgency.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503737Fields.ExecTotal.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503737Fields.UnexecPlanAssign.ToString() });
            
            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private void AddDecimalColumn(GridPanel gp, F0503737Fields field, F0503737Details detail)
        {
            gp.ColumnModel.AddColumn(
                field.ToString(),
                AnnualBalanceHelpers.F0503737NameMapping(field, detail),
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