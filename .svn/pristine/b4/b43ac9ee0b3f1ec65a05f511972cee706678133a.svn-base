using System;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books
{
    class ParamOrgPayerBridge : ParamInfo
    {
        public class ParamBookOrgPayerInfo : ParamBookInfo
        {
            public override bool CheckSearchMask(object value)
            {
                var str = Convert.ToString(value);
                return str.Aggregate(true, (current, t) => current && (t == ' ' || t == ';' || Char.IsDigit(t)));
            }
        }

        public ParamOrgPayerBridge()
        {
            Description = "Плательщики.Сопоставимый";
            BookInfo = new ParamBookOrgPayerInfo
                           {
                               WriteFullText = false,
                               EntityKey = b_Org_PayersBridge.InternalKey,
                               MultiSelect = true,
                               FullScreen = true,
                               DefaultSort = b_Org_PayersBridge.Name,
                               ItemTemplate = String.Format("{0} {1}", b_Org_PayersBridge.INN, b_Org_PayersBridge.Name),
                               ItemFilter = String.Format("{0} is not null", b_Org_PayersBridge.ParentID),
                               EditorSearchField = b_Org_PayersBridge.INN
                           };
        }
    }
}
