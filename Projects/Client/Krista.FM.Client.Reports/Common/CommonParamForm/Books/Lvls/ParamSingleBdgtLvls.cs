using System.Collections.Generic;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Lvls
{
    class ParamSingleBdgtLvls : ParamInfo
    {
        public ParamSingleBdgtLvls()
        {
            Description = "Уровень бюджета";
            MultiSelect = false;
            Values = new List<object> { "Конс. бюджет субъекта", "Бюджет субъекта", "Местный бюджет" };
        }
    }
}
