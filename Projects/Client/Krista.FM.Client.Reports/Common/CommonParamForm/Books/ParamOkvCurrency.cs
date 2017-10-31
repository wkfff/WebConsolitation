using Krista.FM.Client.Reports.Database.ClsData;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books
{
    class ParamOkvCurrency : ParamInfo
    {
        public ParamOkvCurrency()
        {
            Description = "Валюта";
            BookInfo = new ParamBookInfo
                           {
                               EntityKey = d_OKV_Currency.internalKey
                           };
        }
    }
}
