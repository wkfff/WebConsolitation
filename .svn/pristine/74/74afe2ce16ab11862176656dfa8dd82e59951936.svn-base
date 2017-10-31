using System;
using Krista.FM.Client.Reports.Database.ClsData.FNS;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS
{
    class ParamMarksFNS5NDFL : ParamInfo
    {
        public ParamMarksFNS5NDFL()
        {
            Description = "Показатели.ФНС 5 НДФЛ";
            BookInfo = new ParamBookInfo
            {
                HasSource = true,
                SourceName = "ФНС",
                SourceCode = "10",
                SourceYear = Convert.ToString(DateTime.Now.Year),
                MultiSelect = true,
                FullScreen = true,
                HasHierarchy = true,
                DeepSelect = true,
                EntityKey = d_Marks_FNS5NDFL.InternalKey,
                DefaultSort = d_Marks_FNS5NDFL.Code,
                ItemTemplate = String.Format("{0} {1}", d_Marks_FNS5NDFL.Code, d_Marks_FNS5NDFL.Name),
                WriteFullText = true
            };
        }
    }
}
