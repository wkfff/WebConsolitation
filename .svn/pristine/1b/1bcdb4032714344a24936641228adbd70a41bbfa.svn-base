using System;
using Krista.FM.Client.Reports.Database.ClsFx;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books
{
    class ParamMeansTypeSKIF : ParamInfo
    {
        public ParamMeansTypeSKIF()
        {
            Description = "Тип средств.СКИФ";
            BookInfo = new ParamBookInfo
                           {
                               MultiSelect = false,
                               EntityKey = fx_MeansType_SKIF.InternalKey,
                               ItemFilter = String.Format("{0} > 0", fx_MeansType_SKIF.ID)
                           };
        }
    }
}
