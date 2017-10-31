using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class UFK_0017_0001_H : CustomReportPage
    {
        private const string header = "Динамика остатка собственных средств бюджета";

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string query = DataProvider.GetQueryText("UFK_0017_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            Label.Text = string.Format("данные на {0}, {1} {2}", CRHelper.RusMonth(CRHelper.MonthNum(dtDate.Rows[0][3].ToString())), dtDate.Rows[0][4], dtDate.Rows[0][0]);
            LabelDate.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            query = DataProvider.GetQueryText("UFK_0017_0001_chart");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "SeriesName", dt);

            UltraChart.DataSource = dt;
            UltraChart.DataBind();
        }

        private DataTable dt = new DataTable();
        private DataTable dtDate = new DataTable();

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            UltraChart.ChartType = ChartType.AreaChart;
            UltraChart.Width = 667;
            UltraChart.Height = 250;

            UltraChart.Axis.X.Extent = 20;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.Y.Extent = 40;
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
