using System;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books
{
    class ParamRegionBridge : ParamInfo
    {
        public ParamRegionBridge()
        {
            Description = "Районы.Сопоставимый";
            BookInfo = new ParamBookInfo
                           {
                               FullScreen = true,
                               EntityKey = b_Regions_Bridge.InternalKey,
                               HasHierarchy = true,
                               MultiSelect = true,
                               DeepSelect = true,
                               WriteFullText = false,
                               ItemTemplate = String.Format("{0} {1}", b_Regions_Bridge.CodeLine, b_Regions_Bridge.Name),
                           };
            BookInfo.SkipLevels.Add(1);
            BookInfo.SkipLevels.Add(3);
        }
    }
}
