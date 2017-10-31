using Krista.FM.Client.Reports.Database.ClsBridge.EGRUL;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.EGRUL
{
    class ParamKOPF : ParamInfo
    {
        public ParamKOPF()
        {
            Description = "КОПФ";
            BookInfo = new ParamBookInfo
                           {
                               EntityKey = b_KOPF_Bridge.InternalKey,
                               MultiSelect = true,
                               HasHierarchy = true,
                               FullScreen = true,
                               DefaultSort = b_KOPF_Bridge.Code
                           };
        }
    }
}
