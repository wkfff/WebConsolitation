using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FK_0001_0001 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            DataTable dtDate = new DataTable();
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();

            query = DataProvider.GetQueryText("FK_0001_0001");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Ðàéîí", dt);

            if (dt.Rows.Count > 2)
            {
                dt.Rows[0][0] = UserParams.ShortStateArea.Value;
                dt.Rows[1][0] = UserParams.ShortRegion.Value;
                dt.Rows[2][0] = "ÐÔ";
            }

            foreach(DataColumn column in dt.Columns)
            {
                column.ColumnName = column.ColumnName.TrimEnd('_');
            }

            UltraChart.DataSource = dt;
            UltraChart.DataBind();

            UltraChart.Width = 310;
            UltraChart.Height = 340;
            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            ((GradientEffect) UltraChart.Effects.Effects[0]).Coloring = GradientColoringStyle.Lighten;

            UltraChart.ChartType = ChartType.StackBarChart;
            UltraChart.StackChart.StackStyle = StackStyle.Complete;
            UltraChart.Axis.Y.Extent = 63;
            UltraChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;

            UltraChart.Axis.X.Visible = false;
//            UltraChart.Axis.X2.Extent = 0;
//            UltraChart.Axis.X2.Visible = true;
//            UltraChart.Axis.X2.Labels.ItemFormatString = "<DATA_VALUE:#0.0>";
//            UltraChart.Axis.X2.Labels.FontColor = Color.FromArgb(209, 209, 209);

//            UltraChart.Data.RowLabelsColumn = 1;
//            UltraChart.Data.UseRowLabelsColumn = true;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 71;

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE_ITEM:P2>";
        }
    }
}
