using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Core.Primitives;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class MFRF_0001_0005_wm2 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e); 

            DataTable dt = new DataTable();
            DataTable dtDate = new DataTable();

            string query = DataProvider.GetQueryText("MFRF_0001_0005_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodLastYear.Value = "2009";
            UserParams.PeriodYear.Value = "2010";

            query = DataProvider.GetQueryText("MFRF_0001_0005");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt);

            LabelTitle.Font.Size = 9;
            LabelTitle.Text = string.Format("ћежбюдж. трансферты в бюджет {0} из ‘едеральных фондов", UserParams.ShortStateArea.Value);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    dt.Columns[j].ColumnName = dt.Columns[j].ColumnName.TrimEnd('_');
                    dt.Columns[j].ColumnName = dt.Columns[j].ColumnName.Replace("br","\n");
                    if (j == 3) dt.Columns[j].ColumnName = dt.Columns[j].ColumnName.Replace(" ", "\n");
                    if (dt.Rows[i][j] != DBNull.Value)
                    {
                        dt.Rows[i][j] = Convert.ToDouble(dt.Rows[i][j]) / 1000;
                    }
                }
            }

            UltraChart.DataSource = dt;
            UltraChart.DataBind();

            UltraChart.Width = 455;
            UltraChart.Height = 450;
            ((GradientEffect)UltraChart.Effects.Effects[0]).Coloring = GradientColoringStyle.Lighten;

            UltraChart.ChartType = ChartType.StackColumnChart;
            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Text = "                      ћлн. руб.";
            UltraChart.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.FontColor = Color.FromArgb(209, 209, 209);
            UltraChart.Axis.X.Extent = 70;
            UltraChart.Axis.Y.Extent = 70;
            UltraChart.Tooltips.FormatString = "<SERIES_LABEL> <ITEM_LABEL>\n<DATA_VALUE:N3> млн.руб.";
            UltraChart.Data.SwapRowsAndColumns = true;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 12);
            UltraChart.TitleLeft.Font = new Font("Verdana", 12);
            UltraChart.Legend.Font = new Font("Verdana", 12);

            UltraChart.Legend.Visible = true; 
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 20;

            UltraChart.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            
            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.FontSizeBestFit = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 12);
        }

        protected void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    text.bounds.Height = 80;
                    text.labelStyle.Font = new Font("Verdana", 12);
                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                }
            }
        }
    }
}
