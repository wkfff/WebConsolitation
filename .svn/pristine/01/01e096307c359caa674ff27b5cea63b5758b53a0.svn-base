using System;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books
{
    class ParamKDBridge : ParamInfo
    {
        public ParamKDBridge()
        {
            Description = "КД.Сопоставимый";
            BookInfo = new ParamBookInfo
                           {
                               WriteFullText = false,
                               MultiSelect = true,
                               HasHierarchy = true,
                               DeepSelect = true,
                               FullScreen = true,
                               DefaultSort = b_KD_Bridge.CodeStr,
                               ItemTemplate = String.Format("{0} {1}", b_KD_Bridge.CodeStr, b_KD_Bridge.Name),
                               EntityKey = b_KD_Bridge.InternalKey
                           };
        }
    }
}
