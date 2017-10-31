using System;
using System.Data;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Text = Infragistics.UltraChart.Core.Primitives.Text;
using System.Drawing;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FST_0001_0002_Horizontal : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            FST_0001_0002_Horizontal_Text1.QueryNameGrown = "FST_0001_0002_Horizontal_TopCount_Electro";
            FST_0001_0002_Horizontal_Text2.QueryNameGrown = "FST_0001_0002_Horizontal_TopCount_heat";
            FST_0001_0002_Horizontal_Text3.QueryNameGrown = "FST_0001_0002_Horizontal_TopCount_water";

            FST_0001_0002_Horizontal_Text2.fileName = "~/FstRegionsHeat.xml";
            FST_0001_0002_Horizontal_Text3.fileName = "~/FstRegionsWater.xml";

            FST_0001_0002_Horizontal_Text1.QueryNameRF = "FST_0001_0002_Horizontal_all_electro";
            FST_0001_0002_Horizontal_Text2.QueryNameRF = "FST_0001_0002_Horizontal_all_heat";
            FST_0001_0002_Horizontal_Text3.QueryNameRF = "FST_0001_0002_Horizontal_all_water";

            IPadElementHeader1.MultitouchReport = "FST_0001_0011_Horizontal";
            IPadElementHeader2.MultitouchReport = "FST_0001_0021_Horizontal";
            IPadElementHeader3.MultitouchReport = "FST_0001_0031_Horizontal";
        }
    }
}
