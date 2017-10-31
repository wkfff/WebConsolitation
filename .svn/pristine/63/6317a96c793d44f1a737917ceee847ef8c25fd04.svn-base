using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class MFRF_0001_0005_wm1_V : CustomReportPage
    {
        public const int fontSize = 8;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("MFRF_0001_0005_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodYear.Value = "2010";
            Label1.Text = "данные на 2010 год";
            Label2.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            query = DataProvider.GetQueryText("MFRF_0001_0005_v");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt);
            LabelTitle.Text = string.Format("ћежбюджетные трансферты {0} из ‘едеральных фондов на {1} год", dt.Rows[0][2], UserParams.PeriodYear.Value);

            foreach (DataRow row in dt.Rows)
            {
                for (int i = 3; i <= 6; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000;
                    }
                }
            }

            UltraChart.DataSource = dt;
            UltraChart.DataBind();

            //UltraChart.Width = 217;
            //UltraChart.Height = 500;

            UltraChart.Width = 215;
            UltraChart.Height = 500;
            ((GradientEffect)UltraChart.Effects.Effects[0]).Coloring = GradientColoringStyle.Lighten;

            UltraChart.ChartType = ChartType.StackBarChart;
            UltraChart.StackChart.StackStyle = StackStyle.Normal;
            UltraChart.Axis.X.Visible = true;
            UltraChart.Axis.X2.Visible = true;
            UltraChart.Axis.Y.Extent = 50;
            UltraChart.Axis.Y.Labels.Visible = false;
            UltraChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;
            UltraChart.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", fontSize);
            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X2.Extent = 40;
            UltraChart.Axis.X2.Labels.FontColor = Color.FromArgb(209, 209, 209);
            UltraChart.Axis.X2.Labels.ItemFormatString = "<DATA_VALUE:#0>";
            UltraChart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:#0>";

            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.Font = new Font("Verdana", fontSize);
            UltraChart.TitleBottom.FontColor = Color.FromArgb(209, 209, 209);
            UltraChart.TitleBottom.Text = "              ћлн. руб.";
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;
            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE_ITEM:N3> млн.руб.";

            UltraChart.Data.RowLabelsColumn = 1;
            UltraChart.Data.UseRowLabelsColumn = true;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 14;
            UltraChart.Legend.Font = new Font("Verdana", fontSize);

            UltraChart.Axis.X.Labels.Font = new Font("Verdana", fontSize);
            UltraChart.Axis.X2.Labels.Font = new Font("Verdana", fontSize);

            UltraChart.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        }

        protected void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            LabelStyle style = new LabelStyle();
            style.Font = new Font("Verdana", fontSize);
            style.FontColor = Color.FromArgb(209, 209, 209);
            
            Text text = new Text();
            text.bounds = new Rectangle(Convert.ToInt32(UltraChart.Width.Value / 2), 62, 70, 20);
            text.SetTextString("ћлн.руб.");
            text.SetLabelStyle(style);
            e.SceneGraph.Add(text);
        }
    }
}
