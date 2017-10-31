using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class ICC_0001_0001_h : CustomReportPage
    {

        private CustomParam lastDateParam;
        private CustomParam prevDateParam;
        private CustomParam yearParam;

        private DateTime lastDate;
        private DateTime prevDate;
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            lastDateParam = UserParams.CustomParam("last_date");
            prevDateParam = UserParams.CustomParam("prev_date");
            yearParam = UserParams.CustomParam("year");

            string query = DataProvider.GetQueryText("ICC_0001_0001_last_date");
            DataTable dtDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Месяц", dtDate);

            if (dtDate.Rows.Count == 0)
                return;

            lastDateParam.Value = dtDate.Rows[0]["Последний месяц с данными"].ToString();
            lastDate = CRHelper.DateByPeriodMemberUName(lastDateParam.Value, 3);

            yearParam.Value = (lastDate.Year - 1).ToString();

            query = DataProvider.GetQueryText("ICC_0001_0001_prev_date");
            dtDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Месяц", dtDate);

            prevDateParam.Value = dtDate.Rows[0]["Последний месяц предыдущего года"].ToString();
            prevDate = CRHelper.DateByPeriodMemberUName(prevDateParam.Value, 3);

            LabelText.Text = "Индекс потребительских цен<br/>";
            LabelText.Text += String.Format("<span class='DigitsValue'>({0} {1:yyyy} г. к {3} {2:yyyy} г.)</span>", CRHelper.RusMonth(lastDate.Month), lastDate, prevDate, CRHelper.RusMonthDat(prevDate.Month));
            query = DataProvider.GetQueryText("ICC_0001_0001_h_grid");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Индекс", dtGrid);

            LabelGrid.Text = "<table Class=HtmlTable width=470px>";
            LabelGrid.Text += "<tr>";
            foreach (DataColumn c in dtGrid.Columns)
            {
                string title = c.Caption;
                if (title.Contains("Федерация"))
                    title = "РФ";
                else if (title.Contains("Уральский"))
                    title = "УРФО";
                else if (title.Contains("Ханты"))
                    title = "ХМАО";
                LabelGrid.Text += String.Format("<th Class=HtmlTableHeader style=\"width: {1}px; border-color: black;\">{0}</th>",
                    title, title == "Индекс" ? 290 : 60);
            }
            LabelGrid.Text += "</tr>";

            foreach (DataRow row in dtGrid.Rows)
            {
                LabelGrid.Text += "<tr>";
                foreach (object cell in row.ItemArray)
                {
                    double value;
                    if (Double.TryParse(cell.ToString(), out value))
                    {
                        string img = String.Empty;
                        if (value > 100)
                            img = " background-image: url(../../../images/arrowRedUpBB.png); background-repeat: no-repeat; background-position: 5% center;";
                        else if (value < 100)
                            img = " background-image: url(../../../images/arrowGreenDownBB.png); background-repeat: no-repeat; background-position: 5% center;";
                        LabelGrid.Text += String.Format("<td style=\"text-align: right;{1}\">{0:N2}</td>", value, img);
                    }
                    else
                        LabelGrid.Text += String.Format("<td style=\"text-align: left;\">{0}</td>", cell);
                }
                LabelGrid.Text += "</tr>";
            }

            LabelGrid.Text += "</table>";
        }

    }
}
