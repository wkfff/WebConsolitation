using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards
{
    public partial class HotReports : CustomReportPage
    {
        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
            Session["Embedded"] = true;
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
                GenerateReportsControls(Request.Params["hotReport"]);
            }
            else
            {
                GenerateReportsControls();
            }
        }

        protected override void Page_LoadComplete(object sender, EventArgs e)
        {
            base.Page_LoadComplete(sender, e);

           // ((ReportsMasterPage)Page.Master).SiteHeader.Visible = true;
        } 

        private void GenerateReportsControls()
        {
            DataSet hotReports = HotReportsHelper.LoadHotReportsStore();
            if (hotReports == null ||
                hotReports.Tables["Reports"] == null)
            {
                return;
            }
            PlaceHolder1.Controls.Add(HotReportsHelper.GetAllReports(hotReports));
        }

        private void GenerateReportsControls(string reportID)
        {
            DataSet hotReports = HotReportsHelper.LoadHotReportsStore();
            if (hotReports == null ||
                hotReports.Tables["Reports"] == null)
            {
                return;
            }
            this.Form.Controls.Add(HotReportsHelper.GetHotReport(hotReports, reportID));
        }
    }
}
