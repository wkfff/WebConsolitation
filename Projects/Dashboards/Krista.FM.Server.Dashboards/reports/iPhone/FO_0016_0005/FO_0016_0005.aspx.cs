using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Core.Primitives;
using System.Web.UI.HtmlControls;
using Infragistics.UltraChart.Shared.Events;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0016_0005: CustomReportPage
    {
        private DataTable gridDt = new DataTable();
        private DataTable gridDate = new DataTable();
        private DataTable gridMax = new DataTable();
        private DataTable gridMin = new DataTable();
        
        private int maxValue = 0;
        private int maxCount = 0;
        private int minValue = 0;
        private int minCount = 0;

        private CustomParam selectedMoType;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            selectedMoType = UserParams.CustomParam("selected_motype");

            string query = DataProvider.GetQueryText("FO_0016_0005_date");
            gridDate = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, gridDate);
            UserParams.PeriodYear.Value = gridDate.Rows[0][0].ToString();
            UserParams.PeriodHalfYear.Value = gridDate.Rows[0][1].ToString();
            UserParams.PeriodQuater.Value = gridDate.Rows[0][2].ToString();

            #region Настройка надписей max, min
            string moType = UserParams.MoType.Value;
            selectedMoType.Value = moType;         
            string query1 = DataProvider.GetQueryText("FO_0016_0005_labelMax");
            gridMax = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query1, "Дата", gridMax);

            if (gridMax.Rows.Count > 0)
            {
                maxValue = Convert.ToInt32(gridMax.Rows[0][3]);
                maxCount = Convert.ToInt32(gridMax.Rows[0][4]);
            }

            string query2 = DataProvider.GetQueryText("FO_0016_0005_labelMin");
            gridMin = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query2, "Дата", gridMin);

            if (gridMin.Rows.Count > 0)
            {
                minValue = Convert.ToInt32(gridMin.Rows[0][3]);
                minCount = Convert.ToInt32(gridMin.Rows[0][4]);
            }
            MaxFontBold.Text = string.Format("&nbsp;({0})&nbsp;", maxValue);
            MinFontBold.Text = string.Format("&nbsp;({0})&nbsp;", minValue);

            if (moType.Contains("Городские округа"))
            {
                MaxFontBold2.Text = string.Format("&nbsp;{0}&nbsp;ГО:", maxCount);
                MinFontBold2.Text = string.Format("&nbsp;{0}&nbsp;ГО:", minCount);
            }
            else if (moType.Contains("Муниципальные районы"))
            {
                MaxFontBold2.Text = string.Format("&nbsp;{0}&nbsp;МР:", maxCount);
                MinFontBold2.Text = string.Format("&nbsp;{0}&nbsp;МР:", minCount);
            }
            else {
                MaxFontBold2.Text = string.Format("&nbsp;{0}&nbsp;СП (ГП):", maxCount);
                MinFontBold2.Text = string.Format("&nbsp;{0}&nbsp;СП (ГП):", minCount);
            }

            if (maxCount > 0)
            {
                string max = "";
                for (int i = 0; i < gridMax.Rows.Count; i++)
                {
                    gridMax.Rows[i][1] = gridMax.Rows[i][1].ToString().Replace(" ", "&nbsp;");
                    max += " " + gridMax.Rows[i][1];
                    if (gridMax.Rows[i][2] != DBNull.Value)
                    {
                        gridMax.Rows[i][2] = gridMax.Rows[i][2].ToString().Replace(" ", "&nbsp;");
                        max += string.Format(" ({0})", gridMax.Rows[i][2]);
                    }
                    max += ",";
                }
                max = max.Remove(max.Length - 1);
                MaxFont.Text = max;
            }

            if (minCount > 0)
            {
                string min = "";
                for (int i = 0; i < gridMin.Rows.Count; i++)
                {
                    gridMin.Rows[i][1] = gridMin.Rows[i][1].ToString().Replace(" ", "&nbsp;");
                    min += " " + gridMin.Rows[i][1].ToString();
                    if (gridMin.Rows[i][2] != DBNull.Value)
                    {
                        gridMin.Rows[i][2] = gridMin.Rows[i][2].ToString().Replace(" ", "&nbsp;");
                        min += string.Format(" ({0})", gridMin.Rows[i][2]);
                    }
                    min += ",";
                }
                min = min.Remove(min.Length - 1);
                MinFont.Text = min;
            }

            #endregion
        }

    
    }
}