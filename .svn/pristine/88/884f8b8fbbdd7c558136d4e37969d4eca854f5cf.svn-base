using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.iPadBricks.iPadBricks;

namespace Krista.FM.Server.Dashboards.iPhone.FK_0004_0001_Chart
{
    public partial class FK_0004_0001_Chart : UserControl
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
            chart.Style.Add("background", String.Format("transparent url(../../../images/rests/{0}.png)", restId));
            chart.Style.Add("background-position", String.Format("0px {0:N0}px", height - 193));
            DivBottom.Style.Add("background", String.Format("transparent url(../../../images/rests/{0}bottom.png) no-repeat", restId));
            DivBottom.Style.Add("background-position", "20px 0px");
            DivBottom.Style.Add("margin-top", "-2px");
            DivTop.Style.Add("background", String.Format("transparent url(../../../images/rests/1top.png) no-repeat", restId));
            DivTop.Style.Add("background-position", "20px 0px");
            //DivBottom.Style.Add("margin-top", "-2px");
        }
    }
}