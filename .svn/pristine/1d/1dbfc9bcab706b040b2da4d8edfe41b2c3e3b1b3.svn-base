using System;
using Krista.FM.Client.Reports.Database.ClsData.FNS;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS
{
    class ParamMarksFNS5YSN : ParamInfo
    {
        public ParamMarksFNS5YSN()
        {
            Description = "Показатели.ФНС 5 УСН";
            BookInfo = new ParamBookInfo
            {
                HasSource = true,
                SourceName = "ФНС",
                SourceCode = "13",
                SourceYear = Convert.ToString(DateTime.Now.Year),
                MultiSelect = true,
                FullScreen = true,
                HasHierarchy = true,
                DeepSelect = true,
                EntityKey = d_Marks_FNS5YSN.InternalKey,
                DefaultSort = d_Marks_FNS5YSN.Code,
                ItemTemplate = String.Format("{0} {1}", d_Marks_FNS5YSN.Code, d_Marks_FNS5YSN.Name),
                WriteFullText = true
            };
        }
    }
}
