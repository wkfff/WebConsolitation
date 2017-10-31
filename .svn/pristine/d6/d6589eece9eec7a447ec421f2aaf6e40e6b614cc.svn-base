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
    public class AnnualBalanceF0503137TypeView : AnnualBalanceBaseView
    {
        public override void InitDoc(ViewPage page)
        {
            base.InitDoc(page);
            Details.Add(GetDetail(F0503137Details.Incomes));
            Details.Add(GetDetail(F0503137Details.Expenses));
            Details.Add(GetDetail(F0503137Details.SourcesOfFinancing));
        }

        private GridPanel GetDetail(F0503137Details detail)
        {
            Store store = GetStore(detail.ToString(), new F0503137Fields(), false, "F0503137");
            store.SetBaseParams("section", ((int)detail).ToString(CultureInfo.InvariantCulture), ParameterMode.Raw);
            store.SetWriteBaseParams("section", ((int)detail).ToString(CultureInfo.InvariantCulture), ParameterMode.Raw);
            Page.Controls.Add(store);

            GridPanel gp = UiBuilders.CreateGridPanel(detail.ToString(), store);
            gp.Title = AnnualBalanceHelpers.F0503137DetailsNameMapping(detail);

            gp.AddRefreshButton();
            gp.AddNewRecordNoEditButton();
            gp.AddRemoveRecordButton();
            gp.AddSaveButton();

            gp.ColumnModel.AddColumn(F0503137Fields.Name.ToString(), AnnualBalanceHelpers.F0503137NameMapping(F0503137Fields.Name, detail), DataAttributeTypes.dtString)
                         .SetEditableString().SetWidth(300).SetMaxLengthEdior(300);

            AddLineCodeColumn(gp, F0503137Fields.LineCode.ToString(), AnnualBalanceHelpers.F0503137NameMapping(F0503137Fields.LineCode, detail));

            gp.ColumnModel.AddColumn(F0503137Fields.BudgClassifCode.ToString(), AnnualBalanceHelpers.F0503137NameMapping(F0503137Fields.BudgClassifCode, detail), DataAttributeTypes.dtString)
                .SetNullable().SetWidth(100).Editor.Add(new TextArea
                {
                    AllowBlank = true,
                    MaxLength = 20,
                    MaskRe = @"[0-9]",
                    Regex = @"\d{20}",
                    RegexText = @"Значение «{0}» должно быть равно 20 символам".FormatWith(AnnualBalanceHelpers.F0503137NameMapping(F0503137Fields.BudgClassifCode, detail))
                });

            AddDecimalColumn(gp, F0503137Fields.ApproveEstimateAssign, detail);

            AddDecimalColumn(gp, F0503137Fields.ExecFinancAuthorities, detail);
            AddDecimalColumn(gp, F0503137Fields.ExecBankAccounts, detail);
            AddDecimalColumn(gp, F0503137Fields.ExecNonCashOperation, detail);
            AddDecimalColumn(gp, F0503137Fields.ExecTotal, detail);
            AddDecimalColumn(gp, F0503137Fields.UnexecAssignments, detail);

            var gridFilters = new GridFilters
            {
                Local = true
            };

            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503137Fields.Name.ToString() });
            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503137Fields.LineCode.ToString() });
            gridFilters.Filters.Add(new StringFilter { DataIndex = F0503137Fields.BudgClassifCode.ToString() });

            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503137Fields.ApproveEstimateAssign.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503127Fields.ExecFinAuthorities.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503127Fields.ExecBankAccounts.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503127Fields.ExecNonCashOperation.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503127Fields.ExecTotal.ToString() });
            gridFilters.Filters.Add(new NumericFilter { DataIndex = F0503127Fields.UnexecAssignments.ToString() });

            gp.Plugins.Add(gridFilters);

            gp.AddColumnsWrapStylesToPage(Page);

            return gp;
        }

        private void AddDecimalColumn(GridPanel gp, F0503137Fields field, F0503137Details detail)
        {
            gp.ColumnModel.AddColumn(
                field.ToString(),
                AnnualBalanceHelpers.F0503137NameMapping(field, detail),
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