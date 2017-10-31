using System;
using System.Data;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0003_0007_Horizontal : CustomReportPage
    {
        protected string listNum;
        protected string listTitle;
        protected string listSubject;

        protected override void Page_Load(object sender, EventArgs e)
        {
            listNum = String.Format("state{0}", HttpContext.Current.Session["CurrentSubjectID"]);
            listTitle = "iМониторинг";
            listSubject = UserParams.StateArea.Value;
        }
    }
}
