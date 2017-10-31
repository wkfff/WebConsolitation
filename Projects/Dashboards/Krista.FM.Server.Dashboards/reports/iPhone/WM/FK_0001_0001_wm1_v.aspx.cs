using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FK_0001_0001_wm1_v : CustomReportPage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string monthStr = CRHelper.RusManyMonthGenitive(monthNum);

            Label3.Text = string.Format("—труктура расходов {0} за {1}&nbsp;{2}&nbsp;{3}&nbsp;года", UserParams.ShortRegion.Value, monthNum, monthStr, yearNum);
            CRHelper.SetNextMonth(ref yearNum, ref monthNum);

            Label1.Text = string.Format("данные на 1 {0} {1} года", CRHelper.RusMonthGenitive(monthNum), yearNum);
            Label2.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();

            query = DataProvider.GetQueryText("FK_0001_0001_v"); 
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "–айон", dt);

            foreach (DataColumn column in dt.Columns)  
            {
                column.ColumnName = column.ColumnName.TrimEnd('_');
                column.ColumnName = column.ColumnName.Replace("безопасность", "безопасн.");
                column.ColumnName = column.ColumnName.Replace("правоохранит", "правоохр");
                column.ColumnName = column.ColumnName.Replace("де€т.", string.Empty);
            }

            UltraChart.DataSource = dt;
            UltraChart.DataBind();

            //UltraChart.Width = 220;
            //UltraChart.Height = 580;

            UltraChart.Width = 215;  
            UltraChart.Height = 580;
            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            ((GradientEffect)UltraChart.Effects.Effects[0]).Coloring = GradientColoringStyle.Lighten;

            UltraChart.ChartType = ChartType.StackBarChart;
            UltraChart.StackChart.StackStyle = StackStyle.Complete;
            UltraChart.Axis.Y.Extent = 80; 
            UltraChart.Axis.Y.Labels.Visible = false;
            UltraChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Near;

            UltraChart.Axis.X.Visible = false; 
              
            UltraChart.Data.RowLabelsColumn = 1;
            UltraChart.Data.UseRowLabelsColumn = true;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 46;

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE_ITEM:P2>";


            UltraChart.Legend.Font = new Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit; 
            UltraChart.Axis.Y.Labels.SeriesLabels.FontSizeBestFit = false;
        }
    }
}

