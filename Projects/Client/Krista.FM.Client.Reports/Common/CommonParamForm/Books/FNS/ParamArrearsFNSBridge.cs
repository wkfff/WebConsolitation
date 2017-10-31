using System;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS
{
    class ParamArrearsFNSBridge : ParamInfo
    {
        public ParamArrearsFNSBridge()
        {
            Description = "Задолженность.ФНС Сопоставимый";
            BookInfo = new ParamBookInfo
                           {
                               WriteFullText = false,
                               MultiSelect = true,
                               FullScreen = true,
                               HasHierarchy = true,
                               DeepSelect = true,
                               EntityKey = b_Arrears_FNSBridge.InternalKey,
                               DefaultSort = b_Arrears_FNSBridge.Code,
                               ItemTemplate =
                                   String.Format("{0} {1}", b_Arrears_FNSBridge.Code, b_Arrears_FNSBridge.Name)
                           };
        }
    }
}
