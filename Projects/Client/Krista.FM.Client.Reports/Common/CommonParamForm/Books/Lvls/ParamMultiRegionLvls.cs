using System.Collections.Generic;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Lvls
{
    class ParamMultiRegionLvls : ParamInfo
    {
        public ParamMultiRegionLvls()
        {
            MultiSelect = true;
            Description = "Уровень бюджета";
            Values = new List<object> { "Зачисления в бюджеты МР", "Зачисления в бюджеты поселений" };
        }
    }
}
