using System;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;

using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class MFRF_0001_0005 : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dt = new DataTable();
            DataTable dtDate = new DataTable();

            string query = DataProvider.GetQueryText("MFRF_0001_0005_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodLastYear.Value = "2010";
            UserParams.PeriodYear.Value = "2011";

            query = DataProvider.GetQueryText("MFRF_0001_0005");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt);

            Label1.Text = string.Format("Межбюджетные трансферты в бюджет {0} из Федеральных фондов", UserParams.ShortStateArea.Value);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    dt.Columns[j].ColumnName = dt.Columns[j].ColumnName.TrimEnd('_');
                    dt.Columns[j].ColumnName = dt.Columns[j].ColumnName.Replace("br","\n");
                    if (dt.Rows[i][j] != DBNull.Value)
                    {
                        dt.Rows[i][j] = Convert.ToDouble(dt.Rows[i][j]) / 1000;
                    }
                }
            }

            UltraChart.DataSource = dt;
            UltraChart.DataBind();

            UltraChart.Width = 310;
            UltraChart.Height = 340;
            ((GradientEffect)UltraChart.Effects.Effects[0]).Coloring = GradientColoringStyle.Lighten;

            UltraChart.ChartType = ChartType.StackColumnChart;
            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Text = "                      Млн. руб.";
            UltraChart.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.FontColor = Color.FromArgb(209, 209, 209);
            UltraChart.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart.Axis.X.Extent = 50;
            UltraChart.Axis.Y.Extent = 30;
            UltraChart.Tooltips.FormatString = "<SERIES_LABEL> <ITEM_LABEL>\n<DATA_VALUE:N3> млн.руб.";
            UltraChart.Data.SwapRowsAndColumns = true;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 20;
        }

//        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
//        {
//            foreach (Primitive primitive in e.SceneGraph)
//            {
//                Text text = primitive as Text;
//
//                if (text == null)
//                {
//                    continue;
//                }
//
//                if (text.GetTextString().Contains("апрель 2009"))
//                {
//                    text.labelStyle.Font = new Font("Verdana", 7, FontStyle.Bold);
//                    text.labelStyle.FontColor = Color.White;
//                    break;
//                }
//            }
//        }
    }
}
