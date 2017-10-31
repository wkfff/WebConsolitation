using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Drawing;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FNS_0001_0005
{
    public partial class Default_avgIncChart : CustomReportPage
    {
        protected void Page_Preload(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            int currentWidth = (int)Session["width_size"] - 60;
            UltraChart.Width = (int)(currentWidth);

            int currentHeight = (int)Session["height_size"] - 270;
            UltraChart.Height = (int)(currentHeight);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                comboYear.SelectedIndex = 10;
                comboMonth.SelectedIndex = 4;                
            }
            string pValue = string.Format("[Период].[Месяц].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}]",
                comboYear.SelectedRow.Cells[0].Text,
                CRHelper.HalfYearNumByMonthNum(comboMonth.SelectedIndex + 1),
                CRHelper.QuarterNumByMonthNum(comboMonth.SelectedIndex + 1),
                comboMonth.SelectedRow.Cells[0].Text);
            
            UserParams.PeriodMonth.Value = pValue;
            UserParams.PeriodYear.Value = string.Format("[Период].[Год].[Данные всех периодов].[{0}]",
                comboYear.SelectedRow.Cells[0].Text);
            UltraChart.DataBind();            
        }

        DataTable dtChart = new DataTable();
        
        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("chartAvgInc");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Район", dtChart);
            UltraChart.DataSource = dtChart;
        }

        protected void chart_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
        {
            e.LabelStyle.FontColor = Color.Black;
            e.LabelStyle.Font = new Font("Verdana", 10);
            e.Text = "Нет данных";
            e.LabelStyle.VerticalAlign = StringAlignment.Near;
        }
    }
}
