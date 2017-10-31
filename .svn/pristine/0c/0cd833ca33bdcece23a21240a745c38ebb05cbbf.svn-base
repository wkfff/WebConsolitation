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
using System.Web;
using System.IO;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FST_0001_0001 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.Region.Value = RegionsNamingHelper.FullName(UserParams.Region.Value.Replace("УФО", "УрФО"));

            UltraChartFST_0001_0001_Chart1.QueryName = "FST_0001_0001_ChartElectro";
            UltraChartFST_0001_0001_Chart2.QueryName = "FST_0001_0001_Chartheat";
            UltraChartFST_0001_0001_Chart3.QueryName = "FST_0001_0001_ChartWater";

            UltraChartFST_0001_0001_Chart1.TaxName = "тариф для населения";

            string filename = HttpContext.Current.Server.MapPath("~/TemporaryImages/FST_0001_0001/") + "TouchElementBounds.xml";
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/TemporaryImages/FST_0001_0001/"));
            File.WriteAllText(filename, String.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?><touchElements><element id=\"fst_0001_0012_FO={0}\" bounds=\"x=0;y=0;width=768;height=310\" openMode=\"loaded\"/><element id=\"fst_0001_0022_FO={0}\" bounds=\"x=0;y=310;width=768;height=310\" openMode=\"loaded\"/><element id=\"fst_0001_0032_FO={0}\" bounds=\"x=0;y=620;width=768;height=310\" openMode=\"loaded\"/></touchElements>", CustomParams.GetFoIdByName(UserParams.Region.Value)));

            IPadElementHeader1.MultitouchReport = String.Format("fst_0001_0012_FO={0}", CustomParams.GetFoIdByName(UserParams.Region.Value));
            IPadElementHeader2.MultitouchReport = String.Format("fst_0001_0022_FO={0}", CustomParams.GetFoIdByName(UserParams.Region.Value));
            IPadElementHeader3.MultitouchReport = String.Format("fst_0001_0032_FO={0}", CustomParams.GetFoIdByName(UserParams.Region.Value));
            
            UltraChartFST_0001_0001_Chart1.RfMiddleLevel = GetMiddleLevel("FST_0001_0002_all_electro");
            UltraChartFST_0001_0001_Chart2.RfMiddleLevel = GetMiddleLevel("FST_0001_0002_all_heat");
            UltraChartFST_0001_0001_Chart3.RfMiddleLevel = GetMiddleLevel("FST_0001_0002_all_water");
           
            UltraChartFST_0001_0001_Chart2.fileName = "~/FstRegionsHeat.xml";
            UltraChartFST_0001_0001_Chart3.fileName = "~/FstRegionsWater.xml";
        }

        private static double GetMiddleLevel(string queryName)
        {
            int notZeroCount = 0;
            double foAvg = 0;

            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                double taxValue;
                if (dtChart.Rows[i][4] != DBNull.Value &&
                    Double.TryParse(dtChart.Rows[i][4].ToString(), out taxValue) &&
                    taxValue != -1)
                {
                    notZeroCount++;
                    foAvg += taxValue;
                }
            }

            return notZeroCount == 0 ? 0 : foAvg / notZeroCount;
        }
    }
}
