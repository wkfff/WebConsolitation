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

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class Food_0002_0001 : CustomReportPage
    {
        private DateTime lastDate;
        private DateTime currentDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (String.IsNullOrEmpty(UserParams.Region.Value) ||
                String.IsNullOrEmpty(UserParams.StateArea.Value))
            {
                UserParams.Region.Value = "ƒальневосточный федеральный округ";
                UserParams.StateArea.Value = " амчатский край";
            }

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Food_0002_0001_incomes_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            lastDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
            currentDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[1][1].ToString(), 3);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[1][1].ToString();
            UserParams.PeriodLastDate.Value = dtDate.Rows[0][1].ToString();

            InitializeDescription();
        }

        #region “екст
        private DataTable dt;

        private void InitializeDescription()
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("Food_0002_0001_Text");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            string grownGoods = String.Empty;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (!(dt.Rows[i][2].ToString().Contains("-")))
                {
                    grownGoods += String.Format(" {0}&nbsp;<nobr>(на&nbsp;<span class='DigitsValue'>{1:N2}%</span>)</nobr>,<br/>", dt.Rows[i][0].ToString().ToLower(), dt.Rows[i][2]);
                }
            }

            string fallingGoods = String.Empty;
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                if (dt.Rows[i][2].ToString().Contains("-"))
                {
                    fallingGoods += String.Format(" {0}&nbsp;<nobr>(на&nbsp;<span class='DigitsValue'>{1:N2}%</span>)</nobr>,<br/>", dt.Rows[i][0].ToString().ToLower(), dt.Rows[i][2]);
                }
            }

            //lbDescription.Text = String.Format("–озничные цены на продукты питани€ на&nbsp;<span class='DigitsValue'>{1:dd.MM.yyyy}</span>&nbsp;(изменение за период с&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;по&nbsp;<span class='DigitsValue'>{1:dd.MM.yyyy}</span>&nbsp;)<br/><table><tr><td>наибольшее увеличение цен</td><td><img src='../../../images/arrowRedUpBB.png'></td><td style='padding-left: 15px'>{2}</td></tr><tr><td>наибольшее снижение цен</td><td><img src='../../../images/arrowGreenDownBB.png'></td><td style='padding-left: 15px'>{3}</td></tr></table>", lastDate, currentDate, grownGoods.TrimEnd(','), fallingGoods.TrimEnd(','));
            lbDescription.Text = String.Format("–озничные цены на продукты питани€ на&nbsp;<span class='DigitsValue'>{1:dd.MM.yyyy}</span>&nbsp;(изменение за период с&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;по&nbsp;<span class='DigitsValue'>{1:dd.MM.yyyy}</span>)<br/><table><tr><td><span class='DigitsValue'>Ќаибольшее увеличение цен</span>&nbsp;<img src='../../../images/arrowRedUpBB.png'></td></tr><tr><td style='padding-left: 15px'>{2}</td></tr><tr><td><span class='DigitsValue'>Ќаибольшее снижение цен</span>&nbsp;<img src='../../../images/arrowGreenDownBB.png'></td></tr><tr><td style='padding-left: 15px'>{3}</td></tr></table>", lastDate, currentDate, grownGoods.Remove(grownGoods.Length - 6, 6), fallingGoods.TrimEnd(',').Remove(fallingGoods.Length - 6, 6));
        }

        #endregion
       
    }
}
