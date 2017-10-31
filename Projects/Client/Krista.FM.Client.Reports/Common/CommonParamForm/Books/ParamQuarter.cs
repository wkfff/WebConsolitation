using System.Collections.Generic;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books
{
    class ParamQuarter : ParamInfo
    {
        public ParamQuarter()
        {
            MultiSelect = true;
            Description = "Квартал";
            NotUncheckBeforeSelect = true;
            Values = new List<object> { "I квартал", "II квартал", "III квартал", "IV квартал" };
        }
    }
}
