using System;
using System.Web.UI;

namespace Krista.FM.Server.Dashboards.Components
{
    public partial class HeaderPR : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            main_ref.HRef = Convert.ToString(Session["MainIndexRef"]);                   
        }
    }
}