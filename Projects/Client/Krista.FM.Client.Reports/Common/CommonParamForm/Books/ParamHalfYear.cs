using System.Collections.Generic;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books
{
    class ParamHalfYear : ParamInfo
    {
        public ParamHalfYear()
        {
            MultiSelect = true;
            Description = "Полугодие";
            NotUncheckBeforeSelect = true;
            Values = new List<object> { "I полугодие", "За год" };
        }
    }
}
