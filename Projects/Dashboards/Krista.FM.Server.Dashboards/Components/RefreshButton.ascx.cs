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

namespace Krista.FM.Server.Dashboards.Components
{
    public partial class RefreshButton : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((Session["PrintVersion"] != null && (bool)Session["PrintVersion"]) ||
                Session["ShowParams"] != null && !(bool)Session["ShowParams"])
            {
                this.Visible = false;
            }
        }
    }
}