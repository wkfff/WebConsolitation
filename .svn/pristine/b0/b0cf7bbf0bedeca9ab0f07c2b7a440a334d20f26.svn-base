using System.Data;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Client.Reports.Common.Commands
{
    static class TemplatesStore
    {
        public static IScheme scheme;
        private static DataTable dtTemplatesData = new DataTable();
        public static DataTable dtTemplates
        {
            get 
            {
                if (dtTemplatesData.Rows.Count == 0)
                {
                    dtTemplatesData = scheme.TemplatesService.Repository.GetTemplatesInfo(TemplateTypes.System);
                }
                return dtTemplatesData;
            }
        }
    }
}
