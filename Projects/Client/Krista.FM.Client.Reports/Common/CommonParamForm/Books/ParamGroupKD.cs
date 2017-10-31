using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books
{
    class ParamGroupKD : ParamInfo
    {
        public ParamGroupKD()
        {
            Description = "Группа КД";
            BookInfo = new ParamBookInfo
                           {
                               DeepSelect = true,
                               HasHierarchy = true,
                               MultiSelect = true,
                               FullScreen = true,
                               EntityKey = b_D_GroupKD.InternalKey,
                               DefaultSort = b_D_GroupKD.CodeStr
                           };
        }
    }
}
