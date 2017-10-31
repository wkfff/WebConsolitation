using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class OIL_0004_0001_Text : UserControl
    {
        public int ChartWidth = 750;
        public int ChartHeight = 235;

        private double foAvg;
        private double rfAvg;

        private string queryName = "Oil_0004_0002_chart";

        public string QueryName
        {
            get
            {
                return queryName;
            }
            set
            {
                queryName = value;
            }
        }

        private string queryNameTaxes = "Oil_0004_0002_taxes";

        public string QueryNameTaxes
        {
            get
            {
                return queryNameTaxes;
            }
            set
            {
                queryNameTaxes = value;
            }
        }

        private Color appearanceColor = Color.Red;

        public Color AppearanceColor
        {
            get
            {
                return appearanceColor;
            }
            set
            {
                appearanceColor = value;
            }
        }

        private double rfMiddleLevel = 0;

        public double RfMiddleLevel
        {
            get
            {
                return rfMiddleLevel;
            }
            set
            {
                rfMiddleLevel = value;
            }
        }

        private double foMiddleLevel = 0;

        public double FoMiddleLevel
        {
            get
            {
                return foMiddleLevel;
            }
            set
            {
                foMiddleLevel = value;
            }
        }

        private DateTime reportDate;
        public DateTime ReportDate
        {
            get
            {
                return reportDate;
            }
            set
            {
                reportDate = value;
            }
        }

        private DateTime lastDate;
        public DateTime LastDate
        {
            get
            {
                return lastDate;
            }
            set
            {
                lastDate = value;
            }
        }

        private DateTime firstDate;
        public DateTime FirstDate
        {
            get
            {
                return firstDate;
            }
            set
            {
                firstDate = value;
            }
        }

        private string taxName = "Бензин марки АИ-80";

        public string TaxName
        {
            get
            {
                return taxName;
            }
            set
            {
                taxName = value;
            }
        }

        private string oilId = "1";

        public string OilId
        {
            get
            {
                return oilId;
            }
            set
            {
                oilId = value;
            }
        }

        DataTable dtChart;
        private string currentFO = Core.CustomParam.CustomParamFactory("region").Value;

        protected void Page_Load(object sender, EventArgs e)
        {
            CustomParams.MakeOilParams(oilId, "id");
            IncomesHeader.MultitouchReport = String.Format("Oil_0004_0011_OIL={0}", oilId);

            Core.CustomParam.CustomParamFactory("region").Value = RegionsNamingHelper.FullName(Core.CustomParam.CustomParamFactory("region").Value.Replace("УФО", "УрФО"));
            currentFO = Core.CustomParam.CustomParamFactory("region").Value;
            IncomesHeader.Text = taxName;

            DataTable dtText = new DataTable();
            string query = DataProvider.GetQueryText(queryNameTaxes);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtText);

            lowestTax.Text = String.Format("<img src=\"../../../images/min.png\">&nbsp;Наименьшая цена, руб.:<br/>&nbsp;{0}&nbsp;<span class='DigitsValue'>{1:N2}</span><br/>&nbsp;{2}&nbsp;<span class='DigitsValue'>{3:N2}</span><br/>&nbsp;{4}&nbsp;<span class='DigitsValue'>{5:N2}</span><br/>&nbsp;{6}&nbsp;<span class='DigitsValue'>{7:N2}</span><br/>&nbsp;{8}&nbsp;<span class='DigitsValue'>{9:N2}</span><br/>&nbsp;{10}&nbsp;<span class='DigitsValue'>{11:N2}</span><br/>&nbsp;{12}&nbsp;<span class='DigitsValue'>{13:N2}</span>",
                    dtText.Rows[7][0], dtText.Rows[7][1], dtText.Rows[8][0], dtText.Rows[8][1], dtText.Rows[9][0], dtText.Rows[9][1], dtText.Rows[10][0], dtText.Rows[10][1], dtText.Rows[11][0], dtText.Rows[11][1], dtText.Rows[12][0], dtText.Rows[12][1],dtText.Rows[13][0], dtText.Rows[13][1]);
            higestTax.Text = String.Format("<img src=\"../../../images/max.png\">&nbsp;Наибольшая цена, руб.:<br/>&nbsp;{0}&nbsp;<span class='DigitsValue'>{1:N2}</span><br/>&nbsp;{2}&nbsp;<span class='DigitsValue'>{3:N2}</span><br/>&nbsp;{4}&nbsp;<span class='DigitsValue'>{5:N2}</span><br/>&nbsp;{6}&nbsp;<span class='DigitsValue'>{7:N2}</span><br/>&nbsp;{8}&nbsp;<span class='DigitsValue'>{9:N2}</span><br/>&nbsp;{10}&nbsp;<span class='DigitsValue'>{11:N2}</span><br/>&nbsp;{12}&nbsp;<span class='DigitsValue'>{13:N2}</span>",
                    dtText.Rows[0][0].ToString(), dtText.Rows[0][1], dtText.Rows[1][0].ToString(), dtText.Rows[1][1], dtText.Rows[2][0].ToString(), dtText.Rows[2][1], dtText.Rows[3][0].ToString(), dtText.Rows[3][1], dtText.Rows[4][0].ToString(), dtText.Rows[4][1], dtText.Rows[5][0].ToString(), dtText.Rows[5][1], dtText.Rows[6][0].ToString(), dtText.Rows[6][1]);

            DataTable dtLabel = new DataTable();
            query = DataProvider.GetQueryText("Oil_0004_0002_Label");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtLabel);

            foAvg = Convert.ToDouble(dtLabel.Rows[0][6]);

            double value = Convert.ToDouble(dtLabel.Rows[0][7].ToString());
            string grownDescription = value < 0 ?
                String.Format("уменьшилась&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\">&nbsp;на&nbsp;<span class='DigitsValue'>{0:N2}</span>&nbsp;руб. темп прироста&nbsp;<span class='DigitsValue'>{1:P1}</span>", Convert.ToDouble(dtLabel.Rows[0][7].ToString().Replace("-", "")), dtLabel.Rows[0][8]) :
                value > 0 ?
                String.Format("увеличилась&nbsp;<img src=\"../../../images/arrowRedUpBB.png\">&nbsp;на&nbsp;<span class='DigitsValue'>{0:N2}</span>&nbsp;руб. темп прироста&nbsp;<span class='DigitsValue'>+{1:P1}</span>", dtLabel.Rows[0][7], dtLabel.Rows[0][8]) :
                "не изменилась";
            string plus="";
            if (dtLabel.Rows[0][10]!=DBNull.Value)
            if (Convert.ToDouble(dtLabel.Rows[0][10])>0)
            { plus = String.Format("+{0:P1}", dtLabel.Rows[0][10]); }
            else
            { plus = String.Format("{0:P1}",dtLabel.Rows[0][10]);}
            Label1.Text = String.Format(
@"На&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy} г.</span>&nbsp;в&nbsp;<span class='DigitsValue'>{1}</span>&nbsp;средняя розничная цена на {2} составила&nbsp;<span class='DigitsValueXLarge'>{3:N2}</span>&nbsp;руб.<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;с&nbsp;<span class='DigitsValue'>{4:dd.MM.yyyy} г.</span>&nbsp;цена {5}<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;с&nbsp;<span class='DigitsValue'>{8:dd.MM.yyyy} г.</span>&nbsp;цена увеличилась&nbsp;<img src='../../../images/arrowRedUpBB.png'>&nbsp;на&nbsp;<span class='DigitsValue'>{6:N2}</span>&nbsp;руб. темп прироста&nbsp;<span class='DigitsValue'>{7}</span>",
                reportDate, "РФ", CRHelper.ToLowerFirstSymbol(taxName), dtLabel.Rows[0][6], lastDate, grownDescription, dtLabel.Rows[0][9], plus,firstDate);
        }


    }
}