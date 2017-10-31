using System;
using Krista.FM.Client.Reports.Database.ClsFx;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS
{
    class ParamTypesIncomes : ParamInfo
    {
        public ParamTypesIncomes()
        {
            Description = "Виды.Доходы";
            NotUncheckBeforeSelect = true;
            BookInfo = new ParamBookInfo
            {
                MultiSelect = true,
                FullScreen = true,
                EntityKey = fx_Types_Incomes.InternalKey,
                DefaultSort = fx_Types_Incomes.Code,
                ItemTemplate = String.Format("{0}", fx_Types_Incomes.Name),
                ItemFilter = String.Format("{0} = {1}", fx_Types_Incomes.ParentID, fx_Types_Incomes.All),
                WriteFullText = true
            };
        }
    }
}
