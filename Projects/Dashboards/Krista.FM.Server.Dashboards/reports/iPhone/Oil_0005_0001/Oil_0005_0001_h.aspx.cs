using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class Oil_0005_0001_h : CustomReportPage
    {

        private CustomParam lastDateParam;
        private CustomParam prevDateParam;
        private CustomParam yearDateParam;
        private CustomParam rfYearDateParam;
        private CustomParam yearParam;

        private DateTime lastDate;
        private DateTime prevDate;
        private DateTime yearDate;
        private DateTime rfYearDate;
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            #region Даты

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "День", dtDate);

            if (dtDate.Rows.Count == 0)
            {
                return;
            }

            lastDateParam = UserParams.CustomParam("last_date");
            prevDateParam = UserParams.CustomParam("prev_date");
            yearParam = UserParams.CustomParam("year");
            yearDateParam = UserParams.CustomParam("year_date");
            rfYearDateParam = UserParams.CustomParam("rf_year_date");
            
            lastDateParam.Value = dtDate.Rows[0]["Дата"].ToString(); ;
            lastDate = CRHelper.DateByPeriodMemberUName(lastDateParam.Value, 3);

            if (dtDate.Rows.Count == 1)
            {
                prevDate = lastDate.AddDays(-7);
                prevDateParam.Value = CRHelper.PeriodMemberUName("[Период__День].[Период__День].[Данные всех периодов]", prevDate, 5);
            }
            else
            {
                prevDateParam.Value = dtDate.Rows[1]["Дата"].ToString();
                prevDate = CRHelper.DateByPeriodMemberUName(prevDateParam.Value, 3);
            }

            yearParam.Value = CRHelper.PeriodMemberUName("[Период__День].[Период__День].[Данные всех периодов]", new DateTime(lastDate.Year, 1, 1), 1);
            dtDate = new DataTable();
            query = DataProvider.GetQueryText("Oil_0005_0001_h_year");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "День", dtDate);
            if (dtDate.Rows.Count == 1)
            {
                yearDateParam.Value = dtDate.Rows[0]["Дата"].ToString();
                yearDate = CRHelper.DateByPeriodMemberUName(yearDateParam.Value, 3);
            }
            else
            {
                yearDate = new DateTime(lastDate.Year, 1, 1);
                yearDateParam.Value = CRHelper.PeriodMemberUName("[Период__День].[Период__День].[Данные всех периодов]", yearDate, 5);
            }

            yearParam.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", new DateTime(lastDate.Year, 1, 1), 1);
            dtDate = new DataTable();
            query = DataProvider.GetQueryText("Oil_0005_0001_h_year_rf");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "День", dtDate);
            if (dtDate.Rows.Count == 1)
            {
                rfYearDateParam.Value = dtDate.Rows[0]["Дата"].ToString();
                rfYearDate = CRHelper.DateByPeriodMemberUName(rfYearDateParam.Value, 3);
            }
            else
            {
                rfYearDate = new DateTime(lastDate.Year, 1, 1);
                rfYearDateParam.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", yearDate, 5);
            }

            #endregion

            Label.Text = String.Format("Розничные цены на нефтепродукты на&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span><br/>", lastDate);

            Label.Text += "<table Class=HtmlTable width=470px>";

            Label.Text += FillData(lastDate, prevDate, prevDate, "за неделю");
            Label.Text += FillData(lastDate, yearDate, rfYearDate, "с начала года");

            Label.Text += "</table>";

        }

        protected DataTable MakeHmaoTable(DataTable dtLastDate, DataTable dtPrevDate)
        {
            DataTable dtGrid = new DataTable();

            dtGrid.Columns.Add("Вид ГСМ", typeof(string));
            dtGrid.Columns.Add("Средняя по субъекту на последнюю дату", typeof(double));
            dtGrid.Columns.Add("Средняя по субъекту на предыдущую дату", typeof(double));
            dtGrid.Columns.Add("Прирост", typeof(double));
            dtGrid.Columns.Add("Темп прироста", typeof(double));

            string[] oil = { "Бензин марки АИ-80", "Бензин марки АИ-92", "Бензин марки АИ-95", "Дизельное топливо" };

            foreach (string fuel in oil)
            {
                DataRow row = dtGrid.NewRow();

                row["Вид ГСМ"] = fuel;
                row["Средняя по субъекту на последнюю дату"] = MathHelper.GeoMean(dtLastDate.Columns[fuel], DBNull.Value);
                row["Средняя по субъекту на предыдущую дату"] = MathHelper.GeoMean(dtPrevDate.Columns[fuel], DBNull.Value);
                row["Прирост"] = MathHelper.Minus(row["Средняя по субъекту на последнюю дату"], row["Средняя по субъекту на предыдущую дату"]);
                row["Темп прироста"] = MathHelper.Grown(row["Средняя по субъекту на последнюю дату"], row["Средняя по субъекту на предыдущую дату"]);

                dtGrid.Rows.Add(row);
            }

            return dtGrid;
        }

        protected string FillData(DateTime currentDate, DateTime compareDate, DateTime rfCompareDate, string period)
        {
            lastDateParam.Value = CRHelper.PeriodMemberUName("[Период__День].[Период__День].[Данные всех периодов]", currentDate, 5);
            prevDateParam.Value = CRHelper.PeriodMemberUName("[Период__День].[Период__День].[Данные всех периодов]", compareDate, 5);

            string result = String.Format("<tr><td colspan=7 style=\"padding-left: 0px; border-color: black;\">Изменение за период с&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;по&nbsp;<span class='DigitsValue'>{1:dd.MM.yyyy}</span></td></tr>",
                compareDate, currentDate);

            /*DataTable dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_v_grid_hmao");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Вид ГСМ", dtGrid);*/

            #region Цена по ХМАО

            CustomParam dateParam = UserParams.CustomParam("date");
            dateParam.Value = lastDateParam.Value;
            DataTable dtLastDate = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Организация", dtLastDate);

            dateParam.Value = prevDateParam.Value;
            DataTable dtPrevDate = new DataTable();
            query = DataProvider.GetQueryText("Oil_0005_0001_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Организация", dtPrevDate);

            DataTable dtGrid = MakeHmaoTable(dtLastDate, dtPrevDate);

            #endregion

            lastDateParam.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", currentDate, 5);
            prevDateParam.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", rfCompareDate, 5);

            DataTable dtGridRf = new DataTable();
            query = DataProvider.GetQueryText("Oil_0005_0001_v_grid_rf");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Вид ГСМ", dtGridRf);

            if (dtGrid.Rows.Count == 0)
                return result;

            //result += "<table Class=HtmlTable width=480px>";

            result += "<tr><th Class=HtmlTableHeader rowspan=\"2\" style=\"width: 140px; border-color: black;\">Вид ГСМ</th>";
            result += String.Format("<th Class=HtmlTableHeader colspan=2 style=\"width: 110px; border-color: black;\">Цены на {0:dd.MM.yyyy}</th>", currentDate);
            result += String.Format("<th Class=HtmlTableHeader colspan=2 style=\"width: 110px; border-color: black;\">Цены на {0:dd.MM.yyyy}</th>", compareDate);
            result += String.Format("<th Class=HtmlTableHeader colspan=2 style=\"width: 110px; border-color: black;\">Динамика {0}</th></tr>", period);
            result += "<tr><th Class=HtmlTableHeader style=\"width: 55px; border-color: black;\">ХМАО</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 55px; border-color: black;\">РФ</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 55px; border-color: black;\">ХМАО</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 55px; border-color: black;\">РФ</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 55px; border-color: black;\">ХМАО</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 55px; border-color: black;\">РФ</th></tr>";

            for (int i = 0; i < dtGrid.Rows.Count && i < dtGridRf.Rows.Count; ++i)
            {
                DataRow rowHMAO = dtGrid.Rows[i];
                DataRow rowRF = dtGridRf.Rows[i];
                string hmao = String.Format("{0:N2}<br/>{1:P2}", rowHMAO["Прирост"], rowHMAO["Темп прироста"]);
                string rf = String.Format("{0:N2}<br/>{1:P2}", rowRF["Прирост"], rowRF["Темп прироста"]);

                string imageHMAO = String.Empty, imageRF = String.Empty;

                if (MathHelper.IsDouble(rowHMAO["Прирост"]))
                    if (MathHelper.AboveZero(rowHMAO["Прирост"]))
                    {
                        imageHMAO = " background-image: url(../../../images/arrowRedUpBB.png); background-repeat: no-repeat; background-position: 5% 5%;";
                    }
                    else if (MathHelper.SubZero(rowHMAO["Прирост"]))
                    {
                        imageHMAO = " background-image: url(../../../images/arrowGreenDownBB.png); background-repeat: no-repeat; background-position: 5% 5%;";
                    }
                if (MathHelper.IsDouble(rowRF["Прирост"]))
                    if (MathHelper.AboveZero(rowRF["Прирост"]))
                    {
                        imageRF = " background-image: url(../../../images/arrowRedUpBB.png); background-repeat: no-repeat; background-position: 5% 5%;";
                    }
                    else if (MathHelper.SubZero(rowRF["Прирост"]))
                    {
                        imageRF = " background-image: url(../../../images/arrowGreenDownBB.png); background-repeat: no-repeat; background-position: 5% 5%;";
                    }

                result += "<tr>";

                result += String.Format("<td>{0}</td>", rowHMAO["Вид ГСМ"].ToString().Replace(" АИ-", "<br/>АИ-"));
                result += String.Format("<td style=\"text-align: right;\">{0:N2}</td>", rowHMAO["Средняя по субъекту на последнюю дату"]);
                result += String.Format("<td style=\"text-align: right;\">{0:N2}</td>", rowRF["Цена на последнюю дату"]);
                result += String.Format("<td style=\"text-align: right;\">{0:N2}</td>", rowHMAO["Средняя по субъекту на предыдущую дату"]);
                result += String.Format("<td style=\"text-align: right;\">{0:N2}</td>", rowRF["Цена на предыдущую дату"]);
                result += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", hmao, imageHMAO);
                result += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", rf, imageRF);

                result += "</tr>";
            }

            //result += "</table>";

            return result;

        }

    }
}
