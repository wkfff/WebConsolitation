using System;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS
{
    class ParamMarksFNS5DDKBridge : ParamInfo
    {
        public ParamMarksFNS5DDKBridge()
        {
            Description = "Показатели.ФНС_5 ДДК_Сопоставимый";
            BookInfo = new ParamBookInfo
            {
                MultiSelect = true,
                FullScreen = true,
                HasHierarchy = true,
                DeepSelect = true,
                EntityKey = b_Marks_FNS5DDKBridge.InternalKey,
                DefaultSort = b_Marks_FNS5DDKBridge.Code,
                ItemTemplate = String.Format("{0} {1}", b_Marks_FNS5DDKBridge.Code, b_Marks_FNS5DDKBridge.Name),
                WriteFullText = true
            };
        }
    }
}
