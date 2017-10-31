using Krista.FM.Client.Reports.Database.ClsFx;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books
{
    class ParamActivityStatus :ParamInfo
    {
        public ParamActivityStatus()
        {
            Description = "Вид деятельности";
            BookInfo = new ParamBookInfo
                           {
                               WriteFullText = false,
                               MultiSelect = true,
                               DefaultSort = fx_ActivityStatus_IndexProfit.Code,
                               ItemTemplate = fx_ActivityStatus_IndexProfit.Name,
                               EntityKey = fx_ActivityStatus_IndexProfit.InternalKey
                           };
        }
    }
}
