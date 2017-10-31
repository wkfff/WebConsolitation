using Krista.FM.Server.Dashboards.Core;
using System;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Data;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class ICC_0001_0001_v : CustomReportPage
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

            query = DataProvider.GetQueryText("ICC_0001_0001_text");
            DataTable dtText = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Текст", dtText);

            LabelText.Text = String.Format("Стоимость минимального набора продуктов питания в среднем по ХМАО-Югре в конце {0} {1:yyyy} г. составила" +
                "&nbsp;<span class='DigitsValue'>{2:N2} руб.</span>&nbsp;в расчете на месяц",
                CRHelper.RusMonthGenitive(lastDate.Month), lastDate, dtText.Rows[0]["Значение на последнюю дату"]);

            if (MathHelper.IsDouble(dtText.Rows[0]["Прирост"]))
            {
                if (MathHelper.AboveZero(dtText.Rows[0]["Прирост"]))
                {
                    LabelText.Text += String.Format("<br/>По сравнению с последним месяцем предыдущего года стоимость увеличилась" +
                        "&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;на&nbsp;<span class='DigitsValue'>{0:N2} руб.</span>&nbsp;",
                        Convert.ToDouble(dtText.Rows[0]["Прирост"]));
                }
                else if (MathHelper.SubZero(dtText.Rows[0]["Прирост"]))
                {
                    LabelText.Text += String.Format("<br/>По сравнению с последним месяцем предыдущего года стоимость снизилась" +
                        "&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;на&nbsp;<span class='DigitsValue'>{0:N2} руб.</span>&nbsp;",
                        -Convert.ToDouble(dtText.Rows[0]["Прирост"]));
                }
                else
                {
                    LabelText.Text += String.Format("<br/>По сравнению с последним месяцем предыдущего года стоимость не изменилась");
                }
            }

            LabelText.Text += "<br/>Индекс потребительских цен<br/>";
            LabelText.Text += String.Format("<span class='DigitsValue'>({0} {1:yyyy} г. к {3} {2:yyyy} г.)</span>", CRHelper.RusMonth(lastDate.Month), lastDate, prevDate, CRHelper.RusMonthDat(prevDate.Month));
            query = DataProvider.GetQueryText("ICC_0001_0001_h_grid");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Индекс", dtGrid);
            
            LabelGrid.Text = "<table Class=HtmlTable width=310px>";
            LabelGrid.Text += "<tr>";
            LabelGrid.Text += String.Format("<th Class=HtmlTableHeader style=\"width: 240px; border-color: black;\">Индекс</th>");
            LabelGrid.Text += String.Format("<th Class=HtmlTableHeader style=\"width: 70px; border-color: black;\">ХМАО</th>");
            LabelGrid.Text += "</tr>";

            foreach (DataRow row in dtGrid.Rows)
            {
                LabelGrid.Text += "<tr>";
                for (int i = 0; i < 2; ++i)
                {
                    object cell = row[i];
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
                        LabelGrid.Text += String.Format("<td style=\"text-align: left;\">{0}</td>", cell.ToString().Replace("коммунальнальные", "коммунальные"));
                }
                LabelGrid.Text += "</tr>";
            }

            LabelGrid.Text += "</table>";
        }
    }
}
