using Krista.FM.Client.Reports.Database.ClsData;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.Variants
{
    class ParamVariantBorrow : ParamInfo
    {
        public ParamVariantBorrow()
        {
            Description = "Вариант.ИФ";
            BookInfo = new ParamBookInfo
                           {
                               EntityKey = d_Variant_Borrow.internalKey
                           };
        }
    }
}
