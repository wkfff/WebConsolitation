using System;
using System.Configuration;
using System.Data;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards
{
    public partial class EmbeddedReports : CustomReportPage
    {
        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
            if (ConfigurationManager.AppSettings["SiteName"] != null)
            {
                Title = ConfigurationManager.AppSettings["SiteName"];
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (Request.Params["hotReport"] != null)
            {
                GenerateReportPreview(Request.Params["hotReport"]);
            }
        }

        private void GenerateReportPreview(string reportID)
        {
            DataSet hotReports = HotReportsHelper.LoadHotReportsStore();
            if (hotReports == null ||
                hotReports.Tables["Reports"] == null)
            {
                return;
            }
            this.Form.Controls.Add(HotReportsHelper.GetEmbeddedPreview(hotReports, reportID));
        }
    }
}
