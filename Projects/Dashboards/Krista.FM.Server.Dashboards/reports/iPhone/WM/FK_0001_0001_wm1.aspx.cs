using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FK_0001_0001_wm1 : CustomReportPage
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
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dt);

            if (dt.Rows.Count > 2)
            {
                dt.Rows[0][0] = UserParams.ShortStateArea.Value;
                dt.Rows[1][0] = UserParams.ShortRegion.Value;
                dt.Rows[2][0] = "РФ";
            }

            foreach (DataColumn column in dt.Columns)
            {
                column.ColumnName = column.ColumnName.TrimEnd('_');
                column.ColumnName = column.ColumnName.Replace("правоохранит.", "правоохр.");
            }

            UltraChart.DataSource = dt;
            UltraChart.DataBind();

            //UltraChart.Width = 220; 
            //UltraChart.Height = 250;

            UltraChart.Width = 231;
            UltraChart.Height = 261;

            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            ((GradientEffect)UltraChart.Effects.Effects[0]).Coloring = GradientColoringStyle.Lighten;

            UltraChart.ChartType = ChartType.StackBarChart;
            UltraChart.StackChart.StackStyle = StackStyle.Complete;
            UltraChart.Axis.Y.Extent = 40;
            UltraChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;

            UltraChart.Axis.X.Visible = false;
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 77;

            UltraChart.Legend.Font = new Font("Verdana", (float)6.7);
            UltraChart.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", (float)6.8);

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE_ITEM:P2>";

            UltraChart.BackColor = Color.Black;
            UltraChart.Axis.Y.Labels.SeriesLabels.FontColor = Color.White;
            UltraChart.Legend.FontColor = Color.White;
            UltraChart.Legend.BorderColor = Color.Gray;
            UltraChart.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            UltraChart.Legend.BorderColor = Color.Black;
            UltraChart.Legend.BackgroundColor = Color.DimGray;
            UltraChart.Legend.FontColor = Color.FromArgb(209, 209, 209);
            UltraChart.Legend.AlphaLevel = 50;
        }
    }
}
