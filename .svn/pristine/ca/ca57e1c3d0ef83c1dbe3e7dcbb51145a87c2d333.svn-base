using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Krista.FM.Server.Dashboards.Components.Components
{
    public partial class RestImageBox : System.Web.UI.UserControl
    {
        private double value = 0;

        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }

        private double maxValue = 1;

        public double MaxValue
        {
            get { return maxValue; }
            set { this.maxValue = value == 0 ? 1 : value; }
        }

        private string restId = "1";

        public string RestId
        {
            get { return restId; }
            set { this.restId = value; }
        }

        private string tooltip = String.Empty;

        public string Tooltip
        {
            get { return tooltip; }
            set { this.tooltip = value; }
        }

        public String Name
        {
            get { return lbName.Text; }
            set { lbName.Text = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            double height = value / maxValue * 193;
            chart.Style.Add("height", String.Format("{0:N0}px", height));
            lbValue.Text = String.Format("<span class='DigitsValueXLarge'>{0:N2}</span><br/> млрд.руб.", value);
            chart.Style.Add("background", String.Format("transparent url(../../images/rests/{0}.png)", restId));
            chart.Style.Add("background-position", String.Format("0px {0:N0}px", height - 193));
            chart.Style.Add("margin-bottom", "-2px");
            chart.Attributes.Add("title", tooltip);
            DivBottom.Style.Add("background", String.Format("transparent url(../../images/rests/{0}bottom.png) no-repeat", restId));
            DivBottom.Style.Add("background-position", "20px 0px");
            DivBottom.Style.Add("margin-top", "-2px");
            DivTop.Style.Add("background", String.Format("transparent url(../../images/rests/1top.png) no-repeat", restId));
            DivTop.Style.Add("background-position", "20px 0px");
            //DivBottom.Style.Add("margin-top", "-2px");
        }
    }
}