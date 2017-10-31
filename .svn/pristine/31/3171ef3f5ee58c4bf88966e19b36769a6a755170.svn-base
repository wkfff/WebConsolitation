using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class ICC_0001_0001 : CustomReportPage
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

            LabelTitle.Text += "Индекс потребительских цен<br/>";
            LabelTitle.Text += String.Format("<span class='DigitsValue'>({0} {1:yyyy} г. к {3} {2:yyyy} г.)</span>", CRHelper.RusMonth(lastDate.Month), lastDate, prevDate, CRHelper.RusMonthDat(prevDate.Month));

            #region Закомментированный кусок

            /*
            query = DataProvider.GetQueryText("ICC_0001_0001_grid");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Индекс", dtGrid);
            
            if (dtGrid.Rows.Count == 0)
                return;

            DataRow row = dtGrid.Rows[0];

            object hmaoValue = row["Значение на последнюю дату; Ханты-Мансийский автономный округ"];
            object hmaoGrowth = row["Прирост; Ханты-Мансийский автономный округ"];
            object urfoValue = row["Значение на последнюю дату; Уральский федеральный округ"];
            object urfoGrowth = row["Прирост; Уральский федеральный округ"];
            object rfValue = row["Значение на последнюю дату; Российская  Федерация"];
            object rfGrowth = row["Прирост; Российская  Федерация"];

            string img = String.Empty;

            if (MathHelper.IsDouble(hmaoValue))
            {
                if (!MathHelper.IsDouble(hmaoGrowth) || Convert.ToDouble(hmaoGrowth) == 0)
                {
                    LabelText.Text = String.Format("Индекс потребительских цен в ХМАО составляет&nbsp;<span class='DigitsValue'>{0:N2}%</span>", hmaoValue);
                }
                else if (Convert.ToDouble(hmaoGrowth) < 0)
                {
                    LabelText.Text = String.Format("Индекс потребительских цен в ХМАО снизился&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;до&nbsp;<span class='DigitsValue'>{0:N2}%</span>", hmaoValue);
                }
                else
                {
                    LabelText.Text = String.Format("Индекс потребительских цен в ХМАО увеличился&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;до&nbsp;<span class='DigitsValue'>{0:N2}%</span>", hmaoValue);
                }

                if (MathHelper.IsDouble(urfoValue))
                {
                    if (Convert.ToDouble(hmaoValue) - Convert.ToDouble(urfoValue) < 0)
                    {
                        LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;выше&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;чем в УРФО&nbsp;<span class='DigitsValue'>{0:N2}%</span>", urfoValue);
                    }
                    else if (Convert.ToDouble(hmaoValue) - Convert.ToDouble(urfoValue) > 0)
                    {
                        LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ниже&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;чем в УРФО&nbsp;<span class='DigitsValue'>{0:N2}%</span>", urfoValue);
                    }
                    else
                    {
                        LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;соответствует УРФО&nbsp;<span class='DigitsValue'>{0:N2}%</span>", urfoValue);
                    }
                }

                if (MathHelper.IsDouble(rfValue))
                {
                    if (Convert.ToDouble(hmaoValue) - Convert.ToDouble(rfValue) < 0)
                    {
                        LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;выше&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;чем в РФ&nbsp;<span class='DigitsValue'>{0:N2}%</span>", rfValue);
                    }
                    else if (Convert.ToDouble(hmaoValue) - Convert.ToDouble(rfValue) > 0)
                    {
                        LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ниже&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;чем в РФ&nbsp;<span class='DigitsValue'>{0:N2}%</span>", rfValue);
                    }
                    else
                    {
                        LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;соответствует РФ&nbsp;<span class='DigitsValue'>{0:N2}%</span>", rfValue);
                    }
                }
            }

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
            */

            #endregion

            query = DataProvider.GetQueryText("ICC_0001_0001_v_grid");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Индекс", dtGrid);

            //LabelGrid.Text = "Индекс потребительских цен<br/>" + String.Format("({0:MMMM yyyy} г. к {2} {1:yyyy} г.)", lastDate, prevDate, CRHelper.RusMonthDat(prevDate.Month)).ToLower();

            LabelGrid.Text = "<table Class=HtmlTable width=310px>";
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
                    title, title == "Индекс" ? 100 : 70);
            }
            LabelGrid.Text += "</tr>";

            string[] names = {"потреби-тельских цен", "на продов. товары", "на непродов. товары", "платные услуги населе-нию"};

            for (int rowIndex = 0; rowIndex < dtGrid.Rows.Count; ++rowIndex)
            {
                DataRow row = dtGrid.Rows[rowIndex];
                LabelGrid.Text += "<tr>";
                LabelGrid.Text += String.Format("<td style=\"text-align: left; padding-left: {1}px;\">{0}</td>", names[rowIndex], rowIndex == 0 ? 5 : 15);
                for (int i = 1; i < row.ItemArray.Length; ++i)
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
                        LabelGrid.Text += String.Format("<td style=\"text-align: left;\">{0}</td>", cell);
                }
                LabelGrid.Text += "</tr>";
            }

            LabelGrid.Text += "</table>";
        }
    }
}
