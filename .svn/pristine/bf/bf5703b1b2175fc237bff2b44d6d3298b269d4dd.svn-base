using System;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books
{
    class ParamKVSRBridge : ParamInfo
    {
        public ParamKVSRBridge()
        {
            Description = "Администратор";
            BookInfo = new ParamBookInfo
                           {
                               WriteFullText = true,
                               MultiSelect = true,
                               FullScreen = true,
                               DefaultSort = b_KVSR_Bridge.Code,
                               ItemTemplate = String.Format("{0} {1}", b_KVSR_Bridge.Code, b_KVSR_Bridge.Name),
                               EntityKey = b_KVSR_Bridge.InternalKey
                           };
        }
    }
}
