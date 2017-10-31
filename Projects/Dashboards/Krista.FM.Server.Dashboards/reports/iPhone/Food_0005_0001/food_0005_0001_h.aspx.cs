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
    public partial class Food_0005_0001_h : CustomReportPage
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
            string query = DataProvider.GetQueryText("Food_0005_0001_date");
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
            query = DataProvider.GetQueryText("Food_0005_0001_h_year");
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
            query = DataProvider.GetQueryText("Food_0005_0001_h_year_rf");
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

            Label.Text = "<table Class=HtmlTable width=580px>";

            //Label.Text += FillData(lastDate, prevDate, prevDate, "за неделю");
            //Label.Text += FillData(lastDate, yearDate, rfYearDate, "с начала года");

            Label.Text += FillData();

            Label.Text += "</table>";

        }

        protected string FillData()
        {
            string result = String.Format("<tr><td colspan=7 style=\"padding-left: 0px; border-color: black;\">Розничные цены на продукты питания на&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span></td></tr>",
                lastDate);

            DataTable dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("Food_0005_0001_h_grid_hmao");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            if (dtGrid.Rows.Count == 0)
                return result;

            lastDateParam.Value = lastDateParam.Value.Replace("[Период__День]", "[Период__Период]");
            prevDateParam.Value = prevDateParam.Value.Replace("[Период__День]", "[Период__Период]");
            yearDateParam.Value = yearDateParam.Value.Replace("[Период__День]", "[Период__Период]");

            DataTable dtGridRf = new DataTable();
            query = DataProvider.GetQueryText("Food_0005_0001_h_grid_rf");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGridRf);

            Dictionary<string, string> rfFoods = new Dictionary<string, string>(dtGridRf.Rows.Count);
            foreach (DataRow rfGridRow in dtGridRf.Rows)
            {
                rfFoods.Add(rfGridRow["Наименование"].ToString(), String.Format("{0:N2};{1:N2};{2:P2};{3:N2};{4:P2}",
                    rfGridRow["Цена на последнюю дату"], rfGridRow["Прирост"], rfGridRow["Темп прироста"], rfGridRow["Прирост к началу года"], rfGridRow["Темп прироста к началу года"]));
            }

            result += "<tr><th Class=HtmlTableHeader rowspan=\"2\" style=\"width: 200px; border-color: black;\">Продукт</th>";
            result += String.Format("<th Class=HtmlTableHeader colspan=2 style=\"width: 120px; border-color: black;\">Цены на {0:dd.MM.yyyy}</th>", lastDate);
            result += String.Format("<th Class=HtmlTableHeader colspan=2 style=\"width: 130px; border-color: black;\">Динамика за неделю</th>");
            result += String.Format("<th Class=HtmlTableHeader colspan=2 style=\"width: 130px; border-color: black;\">Динамика с начала года</th></tr>");
            result += "<tr><th Class=HtmlTableHeader style=\"width: 60px; border-color: black;\">ХМАО</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 60px; border-color: black;\">РФ</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 65px; border-color: black;\">ХМАО</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 65px; border-color: black;\">РФ</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 65px; border-color: black;\">ХМАО</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 65px; border-color: black;\">РФ</th></tr>";

            string imgHmaoPrevDate = String.Empty, imgHmaoYearDate = String.Empty, imgRfPrevDate = String.Empty, imgRfYearDate = String.Empty;

            foreach (DataRow row in dtGrid.Rows)
            {
                string food = ConvertFoodName(row["Наименование"].ToString());
                string unit = row["Единица измерения"].ToString().Replace("Килограмм", "кг.").Replace("Литр", "л.");

                string rfCost = "-";
                string rfPrevDate = "-<br/>-";
                string rfYearDate = "-<br/>-";
                string rfInfo = String.Empty;
                if (rfFoods.TryGetValue(food, out rfInfo))
                {
                    string[] rfInfoSplit = rfInfo.Split(';');
                    rfCost = rfInfoSplit[0];
                    if (MathHelper.IsDouble(rfInfoSplit[1]))
                    {
                        rfPrevDate = String.Format("{0}<br/>{1}", rfInfoSplit[1], rfInfoSplit[2]);
                        imgRfPrevDate = GetImage(rfInfoSplit[1]);
                    }
                    if (MathHelper.IsDouble(rfInfoSplit[3]))
                    {
                        rfYearDate = String.Format("{0}<br/>{1}", rfInfoSplit[3], rfInfoSplit[4]);
                        imgRfYearDate = GetImage(rfInfoSplit[3]);
                    }
                }

                imgHmaoPrevDate = GetImage(row["Прирост"].ToString());
                imgHmaoYearDate = GetImage(row["Прирост к началу года"].ToString());

                result += "<tr>";

                result += String.Format("<td>{0}, руб./{1}</td>", food, unit);
                result += String.Format("<td style=\"text-align: right;\">{0:N2}</td>", row["Цена на последнюю дату"]);
                result += String.Format("<td style=\"text-align: right;\">{0}</td>", rfCost);
                result += String.Format("<td style=\"text-align: right;{2}\">{0:N2}<br/>{1:P2}</td>", row["Прирост"], row["Темп прироста"], imgHmaoPrevDate);
                result += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", rfPrevDate, imgRfPrevDate);
                result += String.Format("<td style=\"text-align: right;{2}\">{0:N2}<br/>{1:P2}</td>", row["Прирост к началу года"], row["Темп прироста к началу года"], imgHmaoYearDate);
                result += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", rfYearDate, imgRfYearDate);

                result += "</tr>";
            }

            return result;
        }

        protected string GetImage(string value)
        {
            if (!MathHelper.IsDouble(value))
                return String.Empty;
            else if (MathHelper.AboveZero(value))
                return " background-image: url(../../../images/arrowRedUpBB.png); background-repeat: no-repeat; background-position: 5% center;";
            else if (MathHelper.SubZero(value))
                return " background-image: url(../../../images/arrowGreenDownBB.png); background-repeat: no-repeat; background-position: 5% center;";
            else
                return String.Empty;
        }

        /// <summary>
        /// Конвертация классификатора в сопоставимый
        /// </summary>
        /// <param name="food">Наименование продукта в классификаторе</param>
        /// <returns>Наименование того же продукта в сопоставимом классификаторе</returns>
        private string ConvertFoodName(string food)
        {
            return food.
                Replace("Рыба мороженная неразделенная", "Рыба замороженная неразделанная").
                Replace("(2,5-3,2 % жирн.)", "2,5-3,2% жирности").
                Replace("Колбаса вареная, 1 сорт", "Колбаса вареная 1 сорта").
                Replace("Крупа гречневая - ядрица", "Крупа гречневая-ядрица").
                Replace("Куры (кроме куринных окорочков)", "Куры (кроме куриных окорочков)").
                Replace("Макаронные изделия", "Макаронные изделия из пшеничной муки высшего сорта").
                Replace("Мука пшеничная (в/с, 1 сорт)", "Мука пшеничная").
                Replace("Сыры твердые", "Сыры сычужные твердые и мягкие").
                Replace("Водка крепостью 40% об.спирта  и выше обыкновенного качества", "Водка крепостью 40% об.спирта и выше обыкновенного качества").
                Replace("Хлеб и булочные изделия из муки пшеничной 1 и 2 сортов", "Хлеб и булочные изделия из пшеничной муки 1 и 2 сортов");
        }
    }
}
