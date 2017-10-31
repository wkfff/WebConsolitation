using System;
using Krista.FM.Client.Reports.Database.ClsData.FNS;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books.FNS
{
    class ParamMarksFNS5MN : ParamInfo
    {
        public ParamMarksFNS5MN()
        {
            Description = "Показатели.ФНС 5 МН Сопоставимый";
            BookInfo = new ParamBookInfo
                           {
                               HasSource = true,
                               SourceName = "ФНС",
                               SourceCode = "22",
                               SourceYear = Convert.ToString(DateTime.Now.Year),
                               MultiSelect = true,
                               FullScreen = true,
                               HasHierarchy = true,
                               DeepSelect = true,
                               EntityKey = d_Marks_FNS5MN.InternalKey,
                               DefaultSort = d_Marks_FNS5MN.Code,
                               ItemTemplate = String.Format("{0} {1}", d_Marks_FNS5MN.Code, d_Marks_FNS5MN.Name),
                               WriteFullText = true
                           };
        }

        public void SetSourceYear(string year)
        {
            if (BookInfo.SourceYear != year)
            {
                BookInfo.SourceYear = year;
                BookInfo.ClearItemsList();
                BookInfo.CreateDataList();
            }
        }
    }
}
