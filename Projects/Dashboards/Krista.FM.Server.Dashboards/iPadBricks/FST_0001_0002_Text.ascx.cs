using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.IO;
using System.Drawing.Imaging;
using System.Web;
using System.Xml;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class FST_0001_0002_Text : UserControl
    {
        public string fileName = String.Empty;

        private string queryNameGrown = String.Empty;

        public string QueryNameGrown
        {
            get
            {
                return queryNameGrown;
            }
            set
            {
                queryNameGrown = value;
            }
        }

        private string queryNameRF = String.Empty;

        public string QueryNameRF
        {
            get
            {
                return queryNameRF;
            }
            set
            {
                queryNameRF = value;
            }
        }

        private string taxName = "среднеотпускной тариф";

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

        DataTable dtText;
        private int currentYear = 2011;
        private string currentFO = "Российской федерации";

        protected void Page_Load(object sender, EventArgs e)
        {
            dtText = new DataTable();
            String query = DataProvider.GetQueryText(queryNameGrown);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtText);

            DataTable dtChart = new DataTable();
            query = DataProvider.GetQueryText(queryNameRF);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            lowestTax.Text = String.Format("<img src=\"../../../images/min.png\">&nbsp;Наименьший тариф, {6}:<br/>&nbsp;{0}&nbsp;<span class='DigitsValue'>{1:N2}</span><br/>&nbsp;{2}&nbsp;<span class='DigitsValue'>{3:N2}</span><br/>&nbsp;{4}&nbsp;<span class='DigitsValue'>{5:N2}</span>",
                    GetName(dtText.Rows[6][0].ToString()), dtText.Rows[6][1], GetName(dtText.Rows[7][0].ToString()), dtText.Rows[7][1], GetName(dtText.Rows[8][0].ToString()), dtText.Rows[8][1], dtChart.Rows[0][7]);
            higestTax.Text = String.Format("<img src=\"../../../images/max.png\">&nbsp;Наибольший тариф, {6}:<br/>&nbsp;{0}&nbsp;<span class='DigitsValue'>{1:N2}</span><br/>&nbsp;{2}&nbsp;<span class='DigitsValue'>{3:N2}</span><br/>&nbsp;{4}&nbsp;<span class='DigitsValue'>{5:N2}</span>",
                    GetName(dtText.Rows[0][0].ToString()), dtText.Rows[0][1], GetName(dtText.Rows[1][0].ToString()), dtText.Rows[1][1], GetName(dtText.Rows[2][0].ToString()), dtText.Rows[2][1], dtChart.Rows[0][7]);

            lowestTaxGrown.Text = String.Format("<img src=\"../../../images/min.png\">&nbsp;Наименьший рост тарифа:<br/>&nbsp;{0}&nbsp;<span class='DigitsValue'>{1:P2}</span><br/>&nbsp;{2}&nbsp;<span class='DigitsValue'>{3:P2}</span><br/>&nbsp;{4}&nbsp;<span class='DigitsValue'>{5:P2}</span>",
                    GetName(dtText.Rows[9][0].ToString()), dtText.Rows[9][2], GetName(dtText.Rows[10][0].ToString()), dtText.Rows[10][2], GetName(dtText.Rows[11][0].ToString()), dtText.Rows[11][2], dtChart.Rows[0][7]);
            higestTaxGrown.Text = String.Format("<img src=\"../../../images/max.png\">&nbsp;Наибольший рост тарифа:<br/>&nbsp;{0}&nbsp;<span class='DigitsValue'>{1:P2}</span><br/>&nbsp;{2}&nbsp;<span class='DigitsValue'>{3:P2}</span><br/>&nbsp;{4}&nbsp;<span class='DigitsValue'>{5:P2}</span>",
                    GetName(dtText.Rows[3][0].ToString()), dtText.Rows[3][2], GetName(dtText.Rows[4][0].ToString()), dtText.Rows[4][2], GetName(dtText.Rows[5][0].ToString()), dtText.Rows[5][2], dtChart.Rows[0][7]);

            int minColIndex = 0;
            int maxColIndex = 0;
            double minValue = Double.MaxValue;
            double maxValue = Double.MinValue;

            int notZeroCount = 0;
            double foAvg = 0;

            int notZeroGrownCount = 0;
            double foGrownAvg = 0;

            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                double taxValue;
                if (dtChart.Rows[i][8] != DBNull.Value &&
                    Double.TryParse(dtChart.Rows[i][8].ToString(), out taxValue) &&
                    taxValue != 0)
                {
                    notZeroCount++;
                    foAvg += taxValue;
                }
                double value;
                if (dtChart.Rows[i][3] != DBNull.Value &&
                    Double.TryParse(dtChart.Rows[i][3].ToString(), out value) && value != -1)
                {
                    if (value < minValue)
                    {
                        minValue = value;
                        minColIndex = i;
                    }
                    if (value > maxValue)
                    {
                        maxValue = value;
                        maxColIndex = i;
                    }
                }
                double grownValue;
                if (dtChart.Rows[i][4] != DBNull.Value &&
                    Double.TryParse(dtChart.Rows[i][4].ToString(), out grownValue) &&
                    grownValue != -1)
                {
                    notZeroGrownCount++;
                    foGrownAvg += grownValue;
                }
            }

            foAvg = notZeroCount == 0 ? 0 : foAvg / notZeroCount;
            foGrownAvg = notZeroGrownCount == 0 ? 0 : foGrownAvg / notZeroGrownCount;

            Label1.Text = String.Format("В&nbsp;<b><span class='DigitsValue'>{0}</span></b>&nbsp;году в&nbsp;<b><span class='DigitsValue'>{1}</span></b>&nbsp;{9} составил&nbsp;<b><span class='DigitsValueXLarge'>{2:N2}</span></b>&nbsp;{8}",
                currentYear, RegionsNamingHelper.ShortName(currentFO), foAvg, currentYear - 1, foGrownAvg, 0, 0, 0, dtChart.Rows[0][7], taxName);

            Label2.Text = String.Format("По сравнению с&nbsp;<b><span class='DigitsValue'>{3}</span></b>&nbsp;годом прирост составил&nbsp;<b><span class='DigitsValueXLarge'>{4:P2}</span></b>",
                currentYear, RegionsNamingHelper.ShortName(currentFO), foAvg, currentYear - 1, foGrownAvg, 0, 0, 0, dtChart.Rows[0][7]);
        }

        private string GetName(string name)
        {            
            return name;
        }
    }
}