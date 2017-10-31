using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.WebUI.Misc;
using Infragistics.WebUI.WebSchedule;

namespace Krista.FM.Server.Dashboards.Components
{
    public partial class CustomCalendar : System.Web.UI.UserControl
    {
        public WebCalendar WebCalendar
        {
            get
            {
                return webCalendar;
            }
        }

        public WebPanel WebPanel
        {
            get
            {
                return webPanel;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["PrintVersion"] != null && (bool)Session["PrintVersion"])
            {
                this.Visible = false;
            }

            webPanel.Expanded = false;            

            webPanel.ClientSideEvents.ExpandedStateChanging = string.Format("{0}_ExpandedStateChanging", this.ID);
            webPanel.PanelStyle.CustomRules =
                string.Format("position: absolute; z-index: 5; overflow: hidden; margin-top: -1px; clip: rect(0px, 204px, 200px, 0px); *padding-top: 22px;");
            webPanel.PanelStyle.BackColor = Color.Transparent;
            webCalendar.BorderStyle = BorderStyle.Solid;
            webCalendar.BorderWidth = 1;
            webCalendar.BorderColor = Color.FromArgb(171, 193, 222);
            webPanel.Text = "Выбрать дату";
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            StringBuilder scriptString = new StringBuilder();
            scriptString.Append(@"
                <script id='Infragistics' type='text/javascript'>
                <!--");

            string methodName = string.Format("{0}_ExpandedStateChanging", this.ID);
            scriptString.AppendFormat(
                    @"
                    function {0}(oWebPanel, oEvent)
                    {{
                        oWebPanel._contentPanelElement.style.width = '204px';
                        oWebPanel._contentPanelElement.style.height = '200px';
                    }}
                    ",
                    methodName);
            scriptString.Append(@"
                -->
            </script>");
            writer.Write(scriptString.ToString());
        }
    }
}