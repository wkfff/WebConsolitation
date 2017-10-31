using Krista.FM.Client.Reports.Database.ClsData;

namespace Krista.FM.Client.Reports.Common.CommonParamForm.Books
{
    public class ParamRegionAnalisys : ParamInfo
    {
        public ParamRegionAnalisys()
        {
            Description = "Районы.Анализ";
            BookInfo = new ParamBookInfo
                           {
                               SourceCode = "6",
                               SourceName = "ФО",
                               HasHierarchy = true,
                               HasSource = true,
                               FullScreen = true,
                               EntityKey = d_Regions_Analysis.internalKey
                           };
        }
    }
}
