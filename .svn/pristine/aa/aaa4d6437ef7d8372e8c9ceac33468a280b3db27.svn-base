using System;
using Krista.FM.Client.Reports.Database.ClsFx;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Lvls
{
    class ParamFxBudgetLevels : ParamInfo
    {
        public ParamFxBudgetLevels()
        {
            Description = "Уровни бюджета";
            NotUncheckBeforeSelect = true;
            BookInfo = new ParamBookInfo
            {
                WriteFullText = true,
                MultiSelect = true,
                DefaultSort = fx_FX_BudgetLevels.ID,
                ItemTemplate = fx_FX_BudgetLevels.Name,
                EntityKey = fx_FX_BudgetLevels.InternalKey,
                ItemFilter = String.Format("{0} > 0", fx_FX_BudgetLevels.ID)
            };
        }
    }
}
