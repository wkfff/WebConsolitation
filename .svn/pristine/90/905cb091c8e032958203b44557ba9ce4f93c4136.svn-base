using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class Food_0001_0001 : CustomReportPage
    {
        private DateTime lastDate;
        private DateTime currentDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (String.IsNullOrEmpty(UserParams.Region.Value) ||
                String.IsNullOrEmpty(UserParams.StateArea.Value))
            {
                UserParams.Region.Value = "Дальневосточный федеральный округ";
                UserParams.StateArea.Value = "Камчатский край";
            }

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Food_0001_0001_incomes_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            lastDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
            currentDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[1][1].ToString(), 3);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[1][1].ToString();
            UserParams.PeriodLastDate.Value = dtDate.Rows[0][1].ToString();

            InitializeDescription();
            InitializeChart();
        }

        #region Текст
        private DataTable dt;

        private void InitializeDescription()
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("Food_0001_0001_Text");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            string grownGoods = String.Empty;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (!(dt.Rows[i][2].ToString().Contains("-")))
                {
                    grownGoods += String.Format(" {0}&nbsp;<nobr>(на&nbsp;<span class='DigitsValue'>{1:N2}%</span>)</nobr>,", dt.Rows[i][0].ToString().ToLower(), dt.Rows[i][2]);
                }
            }

            string fallingGoods = String.Empty;
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i][2].ToString().Contains("-"))
                {
                    fallingGoods += String.Format(" {0}&nbsp;<nobr>(на&nbsp;<span class='DigitsValue'>{1:N2}%</span>)</nobr>,", dt.Rows[i][0].ToString().ToLower(), dt.Rows[i][2]);
                }
            }

            lbDescription.Text = String.Format("<div style='padding-left: 29px; clear: both'>Розничные цены на продукты питания на&nbsp;<span class='DigitsValue'>{1:dd.MM.yyyy}</span>&nbsp;(изменение за период с&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>)</div><table style='margin-top: 12px'><tr><td align='right' style='color: white'><span class='DigitsValue'>Наибольшее увеличение&nbsp;цен</span></td><td style='padding-left: 15px'><img src='../../../images/arrowRedUpBB.png'></td><td style='padding-left: 15px'>{2}</td></tr><tr><td align='right' style='color: white'><span  class='DigitsValue' style='padding-top: 5px'>Наибольшее снижение&nbsp;цен</span></td><td style='padding-left: 15px; padding-top: 5px'><img src='../../../images/arrowGreenDownBB.png'></td><td style='padding-left: 15px; padding-top: 5px'>{3}</td></tr></table>", lastDate, currentDate, grownGoods.TrimEnd(','), fallingGoods.TrimEnd(','));
        }

        #endregion

     
        #region Фонды

        private DataTable dtChart;

        private void InitializeChart()
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("Food_0001_0001_charts");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            IPadElementHeader1.Text = dtChart.Rows[0][0].ToString();
            IPadElementHeader2.Text = dtChart.Rows[1][0].ToString();
            IPadElementHeader3.Text = dtChart.Rows[2][0].ToString();
            IPadElementHeader4.Text = dtChart.Rows[3][0].ToString();
            IPadElementHeader5.Text = dtChart.Rows[4][0].ToString();
            IPadElementHeader6.Text = dtChart.Rows[5][0].ToString();

            UltraChartFood_0001_0001_Chart1.Source = dtChart.Rows[0];
            UltraChartFood_0001_0001_Chart2.Source = dtChart.Rows[1];
            UltraChartFood_0001_0001_Chart3.Source = dtChart.Rows[2];
            UltraChartFood_0001_0001_Chart4.Source = dtChart.Rows[3];
            UltraChartFood_0001_0001_Chart5.Source = dtChart.Rows[4];
            UltraChartFood_0001_0001_Chart6.Source = dtChart.Rows[5];
        }

        #endregion
    }
}
