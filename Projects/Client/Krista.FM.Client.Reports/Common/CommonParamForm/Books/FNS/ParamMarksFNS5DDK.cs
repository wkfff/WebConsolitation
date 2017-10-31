using System;
using Krista.FM.Client.Reports.Database.ClsData.FNS;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS
{
    class ParamMarksFNS5DDK : ParamInfo
    {
        public ParamMarksFNS5DDK()
        {
            Description = "Показатели.ФНС_5 ДДК";
            BookInfo = new ParamBookInfo
            {
                HasSource = true,
                SourceName = "ФНС",
                SourceCode = "25",
                SourceYear = Convert.ToString(DateTime.Now.Year),
                MultiSelect = true,
                FullScreen = true,
                HasHierarchy = true,
                DeepSelect = true,
                EntityKey = d_Marks_FNS5DDK.InternalKey,
                DefaultSort = d_Marks_FNS5DDK.Code,
                ItemTemplate = String.Format("{0} {1}", d_Marks_FNS5DDK.Code, d_Marks_FNS5DDK.Name),
                WriteFullText = true
            };
        }
    }
}
