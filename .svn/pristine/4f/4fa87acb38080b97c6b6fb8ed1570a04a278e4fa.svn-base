using System;
using Krista.FM.Client.Reports.Database.ClsData.MOFO;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.MOFO
{
    class ParamVariantPropertyTax : ParamInfo
    {
        public ParamVariantPropertyTax()
        {
            Description = "Вариант";
            BookInfo = new ParamBookInfo
                           {
                               MultiSelect = false,
                               EntityKey = d_Variant_PropertyTax.InternalKey,
                               ItemFilter = String.Format("{0} <> -1", d_Variant_PropertyTax.ID)
                           };
        }
    }
}
