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

namespace Krista.FM.Server.Dashboards.Components.Components
{
    public partial class CustomLogin : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Browser.Browser == "Firefox")
            {
                loginDiv.Style["top"] = "185px";
                loginDiv.Style["left"] = "101px";
            }
        }

        public Login Login
        {
            get
            {
                return this.aspLogin;
            }
        }
    }
}