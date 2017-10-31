using System;
using Krista.FM.Client.Reports.Database.ClsFx;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books
{
    class ParamDocTypeSKIF : ParamInfo
    {
        public ParamDocTypeSKIF()
        {
            Description = "Тип документа.СКИФ";
            BookInfo = new ParamBookInfo
            {
                MultiSelect = false,
                EntityKey = fx_DocType_SKIF.InternalKey,
                ItemFilter = String.Format("{0} > 1", fx_DocType_SKIF.ID)
            };
        }
    }
}
