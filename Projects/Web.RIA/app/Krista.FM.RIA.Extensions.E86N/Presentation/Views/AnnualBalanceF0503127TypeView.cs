using System.Globalization;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Presentation.Controls;
using Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Views
{
    public class AnnualBalanceF0503127TypeView : AnnualBalanceBaseView
    {
        public override void InitDoc(ViewPage page)
        {
            base.InitDoc(page);
            Details.Add(GetDetail(F0503127Details.BudgetIncomes));
            Details.Add(GetDetail(F0503127Details.BudgetExpenses));
            Details.Add(GetDetail(F0503127Details.SourcesOfFinancing));
        }

        private GridPanel GetDetail(F0503127Details detail)
        {
            Store store = GetStore(detail.ToString(), new F0503127Fields(), false, "F0503127");
            store.SetBaseParams("section", ((int)detail).ToString(CultureInfo.InvariantCulture), ParameterMode.Raw);
            store.SetWriteBaseParams("section", ((int)detail).ToString(CultureInfo.InvariantCulture), ParameterMode.Raw);
            Page.Controls.Add(store);
            
            GridPanel gp = UiBuilders.CreateGridPanel(detail.ToString(), store);
            gp.Title = AnnualBalanceHelpers.F0503127DetailsNameMapping(detail);

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn(F0503127Fields.Name.ToString(), AnnualBalanceHelpers.F0503127NameMapping(F0503127Fields.Name, detail), DataAttributeTypes.dtString)
                         .SetEditableString().SetWidth(300).SetMaxLengthEdior(300);

            AddLineCodeColumn(gp, F0503127Fields.LineCode.ToString(), AnnualBalanceHelpers.F0503127NameMapping(F0503127Fields.LineCode, detail));
           
            gp.ColumnModel.AddColumn(F0503127Fields.BudgClassifCode.ToString(), AnnualBalanceHelpers.F0503127NameMapping(F0503127Fields.BudgClassifCode, detail), DataAttributeTypes.dtString)
                .SetNullable().SetWidth(100).Editor.Add(new TextArea
                {
                    AllowBlank = true,
                    MaxLength = 20,
                    Regex = @"\S{20}",
                    RegexText = @"Значение «{0}» должно быть равно 20 символам и не содержать пробелов".FormatWith(AnnualBalanceHelpers.F0503127NameMapping(F0503127Fields.BudgClassifCode, detail))
                });

            AddDecimalColumn(gp, F0503127Fields.ApproveBudgAssign, detail);

            if (detail == F0503127Details.BudgetExpenses)
            {
                AddDecimalColumn(gp, F0503127Fields.BudgObligatLimits, detail);
            }

            AddDecimalColumn(gp, F0503127Fields.ExecFinAuthorities, detail);
            AddDecimalColumn(gp, F0503127Fields.ExecBankAccounts, detail);
            AddDecimalColumn(gp, F0503127Fields.ExecNonCashOperation, detail);
            AddDecimalColumn(gp, F0503127Fields.ExecTotal, detail);
            AddDecimalColumn(gp, F0503127Fields.UnexecAssignments, detail);

            if (detail == F0503127Details.BudgetExpenses)
            {
                AddDecimalColumn(gp, F0503127Fields.UnexecBudgObligatLimit, detail);
            }

            var gridFilters = new GridFilters
                {
                    Local = true
                };

            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503127Fields.Name.ToString() });
            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503127Fields.LineCode.ToString() });
            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503127Fields.BudgClassifCode.ToString() });

            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503127Fields.ApproveBudgAssign.ToString() });

            if (detail == F0503127Details.BudgetExpenses)
            {
                gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503127Fields.BudgObligatLimits.ToString() });
            }

            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503127Fields.ExecFinAuthorities.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503127Fields.ExecBankAccounts.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503127Fields.ExecNonCashOperation.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503127Fields.ExecTotal.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503127Fields.UnexecAssignments.ToString() });

            if (detail == F0503127Details.BudgetExpenses)
            {
                gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503127Fields.UnexecBudgObligatLimit.ToString() });
            }

            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private void AddDecimalColumn(GridPanel gp, F0503127Fields field, F0503127Details detail)
        {
            gp.ColumnModel.AddColumn(
                field.ToString(),
                AnnualBalanceHelpers.F0503127NameMapping(field, detail),
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