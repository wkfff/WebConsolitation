using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0032_0001_H : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            DataTable dtDate = new DataTable();

            string query = DataProvider.GetQueryText("FO_0032_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            Label.Text = string.Format("данные на {0} {1} {2} года", dtDate.Rows[0][4], CRHelper.RusMonthGenitive(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][0]);
            LabelDate.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            DataTable dt = new DataTable();

            UserParams.DayCount.Value = "30";
            query = DataProvider.GetQueryText("FO_0032_0001_chart");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "SeriesName", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[1] != DBNull.Value)
                {
                    row[1] = Convert.ToDouble(row[1]) / 1000000;
                }
            }

            UltraChart.DataSource = dt;
            UltraChart.DataBind();
        }

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            UltraChart.ChartType = ChartType.AreaChart;
            UltraChart.Width = 667;
            UltraChart.Height = 250;

            UltraChart.Axis.X.Extent = 20;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.Y.Extent = 30;
            UltraChart.Tooltips.FormatString = "<DATA_VALUE:N3> млн.руб.";
            UltraChart.Legend.Visible = false;
            UltraChart.Data.SwapRowsAndColumns = true;

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Diamond;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Large;
            UltraChart.AreaChart.LineAppearances.Add(lineAppearance);
        }

        protected void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            Infragistics.UltraChart.Core.Primitives.Text text = new Infragistics.UltraChart.Core.Primitives.Text();
            text.PE.Fill = Color.FromArgb(209, 209, 209);
            text.bounds = new Rectangle(5, 90, 10, 45);
            text.SetTextString("млн. руб");
            text.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            e.SceneGraph.Add(text);
        }
    }
}
