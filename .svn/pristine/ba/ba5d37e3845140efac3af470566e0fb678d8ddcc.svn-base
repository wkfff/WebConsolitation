using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards
{
    public partial class PageProcessor : System.Web.UI.Page
    {
        protected string PageToLoad;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "iМониторинг";
            string redirectPath = Request.Params["requestedUrl"] != null
                                  ? Request.Params["requestedUrl"].ToString()
                                  : CustomReportConst.indexPageUrl;
            
            progressDiv.InnerHtml = String.Format("Загружается отчет:<br/>{0}", Session["currentReportName"]);
            Session["Process"] = false;
            PageToLoad = redirectPath;
        } 
    }
}
