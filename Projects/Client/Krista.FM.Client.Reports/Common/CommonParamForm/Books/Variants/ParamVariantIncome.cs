using Krista.FM.Client.Reports.Database.ClsData;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Variants
{
    class ParamVariantIncome : ParamInfo
    {
        public ParamVariantIncome()
        {
            Description = "Вариант.Проект доходов";
            BookInfo = new ParamBookInfo
                           {
                               EntityKey = d_Variant_PlanIncomes.InternalKey
                           };
        }
    }
}
