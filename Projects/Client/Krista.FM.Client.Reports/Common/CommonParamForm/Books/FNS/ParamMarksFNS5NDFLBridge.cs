using System;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS
{
    class ParamMarksFNS5NDFLBridge : ParamInfo
    {
        public ParamMarksFNS5NDFLBridge()
        {
            Description = "Показатели.ФНС 5 НДФЛ Сопоставимый";
            BookInfo = new ParamBookInfo
            {
                MultiSelect = true,
                FullScreen = true,
                HasHierarchy = true,
                DeepSelect = true,
                EntityKey = b_Marks_FNS5NDFLBridge.InternalKey,
                DefaultSort = b_Marks_FNS5NDFLBridge.Code,
                ItemTemplate = String.Format("{0} {1}", b_Marks_FNS5NDFLBridge.Code, b_Marks_FNS5NDFLBridge.Name),
                WriteFullText = true
            };
        }
    }
}
