using System;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS
{
    class ParamMarksFNS5YSNBridge : ParamInfo
    {
        public ParamMarksFNS5YSNBridge()
        {
            Description = "Показатели.ФНС 5 УСН Сопоставимый";
            BookInfo = new ParamBookInfo
            {
                MultiSelect = true,
                FullScreen = true,
                HasHierarchy = true,
                DeepSelect = true,
                EntityKey = b_Marks_FNS5YSNBridge.InternalKey,
                DefaultSort = b_Marks_FNS5YSNBridge.Code,
                ItemTemplate = String.Format("{0} {1}", b_Marks_FNS5YSNBridge.Code, b_Marks_FNS5YSNBridge.Name),
                WriteFullText = true
            };
        }
    }
}
