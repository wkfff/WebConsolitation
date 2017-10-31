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
    public partial class Food_0005_0001_v : CustomReportPage
    {

        private CustomParam lastDateParam;
        private CustomParam prevDateParam;
        private CustomParam food;

        private DateTime lastDate;
        private DateTime prevDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Food_0005_0001_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "День", dtDate);

            if (dtDate.Rows.Count < 2)
            {
                LabelInfo.Text = String.Format("Данные по розничным ценам на продукты питания отсутствуют");
                return;
            }

            food = UserParams.CustomParam("food");

            lastDateParam = UserParams.CustomParam("last_date");
            lastDateParam.Value = dtDate.Rows[0]["Дата"].ToString(); ;
            lastDate = CRHelper.DateByPeriodMemberUName(lastDateParam.Value, 3);
            LabelInfo.Text = String.Format("Розничные цены на продукты питания на&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;", lastDate);

            prevDateParam = UserParams.CustomParam("prev_date");
            prevDateParam.Value = dtDate.Rows[1]["Дата"].ToString();
            prevDate = CRHelper.DateByPeriodMemberUName(prevDateParam.Value, 3);
            LabelInfo.Text += String.Format("(изменение за период с&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>)", prevDate);

            FillInfoLabel();
            FillTextLabel();
            FillTable();

        }

        protected void FillInfoLabel()
        {
            DataTable dtData = new DataTable();
            string query = DataProvider.GetQueryText("Food_0005_0001_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование", dtData);

            string foodUp = String.Empty, foodDown = String.Empty, foodConst = String.Empty;
            foreach (DataRow row in dtData.Rows)
            {
                if (!MathHelper.IsDouble(row["Темп прироста"]))
                    continue;
                string food = row["Наименование"].ToString();
                double value = Convert.ToDouble(row["Темп прироста"]);
                if (value > 0)
                {
                    foodUp += String.Format("<br/>&nbsp;&nbsp;&nbsp;{0} (на&nbsp;<span class='DigitsValue'>{1:P2}</span>)", food, value);
                }
                else if (value < 0)
                {
                    foodDown += String.Format("<br/>&nbsp;&nbsp;&nbsp;{0} (на&nbsp;<span class='DigitsValue'>{1:P2}</span>)", food, -value);
                }
                else
                {
                    foodConst += String.Format("<br/>&nbsp;&nbsp;&nbsp;{0}", food);
                }
            }
            if (foodUp == foodDown)
            {
                LabelInfo.Text += "<br/>Цены в Югре на продукты питания остались без изменений.";
                return;
            }

            if (!String.IsNullOrEmpty(foodUp))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>Отмечено увеличение цен{0}на:</span>{1}", "&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;", foodUp);
            }

            if (!String.IsNullOrEmpty(foodDown))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>Отмечено снижение цен{0}на:</span>{1}", "&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;", foodDown);
            }

            if (!String.IsNullOrEmpty(foodConst))
            {
                LabelInfo.Text += String.Format("<br/>На остальные продукты питания цены стабильны или изменились незначительно.");
            }

        }

        protected void FillTextLabel()
        {
            double[] values = new double[0];
            string[] names = new string[0];
            DataTable dtHMAO = new DataTable();
            string query = DataProvider.GetQueryText("Food_0005_0001_v_text_hmao");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Продукт", dtHMAO);
            foreach (DataRow row in dtHMAO.Rows)
            {
                double growthHMAO = Convert.ToDouble(row["Темп роста"]);
                string shortFood = ConvertFoodName(row["Продукт"].ToString());
                string unit = row["Единица измерения"].ToString().Replace("Килограмм", "кг.").Replace("Литр", "л.");
                food.Value = row["Уникальное имя"].ToString();
                query = DataProvider.GetQueryText("Food_0005_0001_v_one_food_stat_data");
                DataTable dtFood = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "МО", dtFood);
                DataRow beloyarRow = null;
                foreach (DataRow moRow in dtFood.Rows)
                {
                    string mo = moRow["МО"].ToString();
                    if (mo == "гп Белоярский")
                    {
                        beloyarRow = moRow;
                        continue;
                    }
                    object growthMO = moRow["Темп роста"];
                    object lastCost = moRow["Цена на последнюю дату"];
                    if (MathHelper.IsDouble(growthMO) && Convert.ToDouble(growthMO) > growthHMAO)
                    {
                        Array.Resize(ref values, values.Length + 1);
                        Array.Resize(ref names, names.Length + 1);
                        values[values.Length - 1] = Convert.ToDouble(growthMO);
                        names[names.Length - 1] = String.Format("{0}-/-{1}-/-{2}-/-{3}", mo, shortFood, unit, lastCost);
                    }
                }

                query = DataProvider.GetQueryText("Food_0005_0001_v_one_food_data");
                dtFood = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "МО", dtFood);
                foreach (DataRow moRow in dtFood.Rows)
                {
                    string mo = moRow["МО"].ToString();
                    object lastCostMO, prevCostMO;
                    if ((mo == "Белоярский муниципальный район") && (beloyarRow != null))
                    {
                        lastCostMO = MathHelper.GeoMean(moRow, 1, 2, DBNull.Value, beloyarRow["Цена на последнюю дату"]);
                        prevCostMO = MathHelper.GeoMean(moRow, 2, 2, DBNull.Value, beloyarRow["Цена на предыдущую дату"]);
                    }
                    else
                    {
                        lastCostMO = MathHelper.GeoMean(moRow, 1, 2, DBNull.Value);
                        prevCostMO = MathHelper.GeoMean(moRow, 2, 2, DBNull.Value);
                    }
                    object growthMO = MathHelper.Div(lastCostMO, prevCostMO);
                    if (MathHelper.IsDouble(growthMO) && Convert.ToDouble(growthMO) > growthHMAO)
                    {
                        Array.Resize(ref values, values.Length + 1);
                        Array.Resize(ref names, names.Length + 1);
                        values[values.Length - 1] = Convert.ToDouble(growthMO);
                        names[names.Length - 1] = String.Format("{0}-/-{1}-/-{2}-/-{3}", mo, shortFood, unit, lastCostMO);
                    }
                }
            }

            Array.Sort(values, names, new ReverseComparer());

            double[] shortValues;
            string[] shortNames;

            int index = values.Length >= 5 ? 5 : values.Length;

            shortValues = new double[index];
            shortNames = new string[index];
            Array.Copy(names, shortNames, index);
            Array.Copy(values, shortValues, index);

            Array.Sort(names, values);

            LabelText.Text = "В отчетном периоде темп роста, превышающий среднеокружные показатели отмечен в МО ХМАО-Югры:";
            string prevMO = String.Empty;
            for (int i = 0; i < shortNames.Length; ++i)
            {
                string[] separator = { "-/-" };
                string mo = shortNames[i].Split(separator, StringSplitOptions.None)[0].Replace(" муниципальный район", " МР");
                string food = shortNames[i].Split(separator, StringSplitOptions.None)[1];
                string unit = shortNames[i].Split(separator, StringSplitOptions.None)[2];
                double cost = Convert.ToDouble(shortNames[i].Split(separator, StringSplitOptions.None)[3]);
                double grown = Convert.ToDouble(shortValues[i]);
                if (mo != prevMO)
                {
                    prevMO = mo;
                    LabelText.Text += String.Format("<br/><i>{0}:</i>", mo);
                }
                LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0} (цена - {1:N2} руб./{3}) - &nbsp;<span class='DigitsValue'>{2:P2}</span>", food, cost, grown, unit);
            }

        }

        protected void FillTable()
        {
            DataTable dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("Food_0005_0001_v_grid_hmao");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            if (dtGrid.Rows.Count == 0)
                return;

            lastDateParam.Value = lastDateParam.Value.Replace("[Период__День]", "[Период__Период]");
            prevDateParam.Value = prevDateParam.Value.Replace("[Период__День]", "[Период__Период]");

            DataTable dtGridRf = new DataTable();
            query = DataProvider.GetQueryText("Food_0005_0001_v_grid_rf");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGridRf);

            Dictionary<string, string> rfFoods = new Dictionary<string, string>(dtGridRf.Rows.Count);
            foreach (DataRow rfGridRow in dtGridRf.Rows)
            {
                rfFoods.Add(rfGridRow["Наименование"].ToString(), String.Format("{0:N2};{1:N2};{2:P2}", rfGridRow["Цена на последнюю дату"], rfGridRow["Прирост"], rfGridRow["Темп прироста"]));
            }

            LabelGrid.Text = "<table Class=HtmlTable width=310px>";
            LabelGrid.Text += "<tr><th Class=HtmlTableHeader style=\"width: 110px; border-color: black;\">Продукт</th>";
            LabelGrid.Text += "<th Class=HtmlTableHeader style=\"width: 100px; border-color: black;\">ХМАО</th>";
            LabelGrid.Text += "<th Class=HtmlTableHeader style=\"width: 100px; border-color: black;\">РФ</th></tr>";

            for (int i = 0; i < dtGrid.Rows.Count; ++i)
            {
                DataRow rowHMAO = dtGrid.Rows[i];
                string food = ConvertFoodName(rowHMAO["Наименование"].ToString());
                string unit = rowHMAO["Единица измерения"].ToString().Replace("Килограмм", "кг.").Replace("Литр", "л.");
                string hmao = String.Format("{0:N2}<br/>{1:N2}<br/>{2:P2}",
                    rowHMAO["Цена на последнюю дату"], rowHMAO["Прирост"], rowHMAO["Темп прироста"]);
                string rfInfo = String.Empty;
                string rf = "-<br/>-<br/>-";
                if (rfFoods.TryGetValue(food, out rfInfo))
                {
                    rf = String.Format("{0}<br/>{1}<br/>{2}", rfInfo.Split(';')[0], rfInfo.Split(';')[1], rfInfo.Split(';')[2]);
                }

                string imageHMAO = String.Empty, imageRF = String.Empty;

                if (MathHelper.IsDouble(rowHMAO["Прирост"]))
                    if (MathHelper.AboveZero(rowHMAO["Прирост"]))
                    {
                        imageHMAO = " background-image: url(../../../images/arrowRedUpBB.png); background-repeat: no-repeat; background-position: 5% center;";
                    }
                    else if (MathHelper.SubZero(rowHMAO["Прирост"]))
                    {
                        imageHMAO = " background-image: url(../../../images/arrowGreenDownBB.png); background-repeat: no-repeat; background-position: 5% center;";
                    }
                if (!String.IsNullOrEmpty(rfInfo) && MathHelper.IsDouble(rfInfo.Split(';')[1]))
                    if (MathHelper.AboveZero(rfInfo.Split(';')[1]))
                    {
                        imageRF = " background-image: url(../../../images/arrowRedUpBB.png); background-repeat: no-repeat; background-position: 5% center;";
                    }
                    else if (MathHelper.SubZero(rfInfo.Split(';')[1]))
                    {
                        imageRF = " background-image: url(../../../images/arrowGreenDownBB.png); background-repeat: no-repeat; background-position: 5% center;";
                    }

                LabelGrid.Text += "<tr>";

                LabelGrid.Text += String.Format("<td>{0}, руб./{1}</td>", food, unit);
                LabelGrid.Text += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", hmao, imageHMAO);
                LabelGrid.Text += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", rf, imageRF);

                LabelGrid.Text += "</tr>";
            }

            LabelGrid.Text += "</table>";
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
