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
    public partial class Oil_0005_0001_v : CustomReportPage
    {

        private CustomParam lastDateParam;
        private CustomParam prevDateParam;
        private CustomParam dateParam;
        private CustomParam grown80Param;
        private CustomParam grown92Param;
        private CustomParam grown95Param;
        private CustomParam grownDTParam;

        private DateTime lastDate;
        private DateTime prevDate;

        private DataTable dtGrid;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "День", dtDate);

            if (dtDate.Rows.Count < 2)
            {
                LabelInfo.Text = String.Format("Данные по розничным ценам на нефтепродукты отсутствуют");
                return;
            }

            dateParam = UserParams.CustomParam("date");
            grown80Param = UserParams.CustomParam("grown_80");
            grown92Param = UserParams.CustomParam("grown_92");
            grown95Param = UserParams.CustomParam("grown_95");
            grownDTParam = UserParams.CustomParam("grown_DT");

            lastDateParam = UserParams.CustomParam("last_date");
            lastDateParam.Value = dtDate.Rows[0]["Дата"].ToString(); ;
            lastDate = CRHelper.DateByPeriodMemberUName(lastDateParam.Value, 3);
            LabelInfo.Text = String.Format("Розничные цены на нефтепродукты на&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;", lastDate);

            prevDateParam = UserParams.CustomParam("prev_date");
            prevDateParam.Value = dtDate.Rows[1]["Дата"].ToString();
            prevDate = CRHelper.DateByPeriodMemberUName(prevDateParam.Value, 3);
            LabelInfo.Text += String.Format("(изменение за период с&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>)", prevDate);

            FillInfoLabel();
            FillTextLabel();
            FillTable();

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

        protected void FillInfoLabel()
        {
            CustomParam dateParam = UserParams.CustomParam("date");
            dateParam.Value = lastDateParam.Value;
            DataTable dtLastDate = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Организация", dtLastDate);

            dateParam.Value = prevDateParam.Value;
            DataTable dtPrevDate = new DataTable();
            query = DataProvider.GetQueryText("Oil_0005_0001_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Организация", dtPrevDate);

            dtGrid = MakeHmaoTable(dtLastDate, dtPrevDate);

            string fuelUp = String.Empty, fuelDown = String.Empty, fuelConst = String.Empty;
            foreach (DataColumn column in dtLastDate.Columns)
            {
                if (column.ColumnName == "Организация")
                    continue;
                string fuel = column.ColumnName;
                object price = MathHelper.GeoMean(column, DBNull.Value);
                object prevPrice = MathHelper.GeoMean(dtPrevDate.Columns[fuel], DBNull.Value);
                object value = MathHelper.Grown(price, prevPrice, 0, CalcMode.CalcIfTwo);
                switch (fuel)
                {
                    case "Бензин марки АИ-80":
                        {
                            grown80Param.Value = value.ToString();
                            break;
                        }
                    case "Бензин марки АИ-92":
                        {
                            grown92Param.Value = value.ToString();
                            break;
                        }
                    case "Бензин марки АИ-95":
                        {
                            grown95Param.Value = value.ToString();
                            break;
                        }
                    case "Дизельное топливо":
                        {
                            grownDTParam.Value = value.ToString();
                            break;
                        }
                }
                if (MathHelper.AboveZero(value))
                {
                    fuelUp += String.Format("<br/>{0} на&nbsp;<span class='DigitsValue'>{1:P2}</span><br/>(цена - {2:N2} руб.)", fuel, value, price);
                }
                else if (MathHelper.SubZero(value))
                {
                    fuelDown += String.Format("<br/>{0} на&nbsp;<span class='DigitsValue'>{1:P2}</span><br/>(цена - {2:N2} руб.)", fuel, MathHelper.Abs(value), price);
                }
                else
                {
                    fuelConst += String.Format("<br/>{0}", fuel);
                }
            }
            
            if (fuelUp == fuelDown)
            {
                LabelInfo.Text += "<br/>Цены в Югре на бензин всех марок и дизельное топливо остались без изменений.";
                return;
            }

            if (!String.IsNullOrEmpty(fuelUp))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>Отмечено увеличение цен{0}на:</span>{1}", "&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;", fuelUp);
            }

            if (!String.IsNullOrEmpty(fuelDown))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>Отмечено снижение цен{0}на:</span>{1}", "&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;", fuelDown);
            }

            if (!String.IsNullOrEmpty(fuelConst))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>Цены стабильны на:</span>{0}", fuelConst);
            }
        }

        protected void FillTextLabel()
        {
            DataTable dtText = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_v_text");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "ГСМ;МО", dtText);
            if (dtText.Rows.Count == 0)
                return;
            string[] names = new string[dtText.Rows.Count];
            string[] values = new string[dtText.Rows.Count];
            for (int i = 0; i < dtText.Rows.Count; ++i)
            {
                DataRow row = dtText.Rows[i];
                names[i] = row["ГСМ;МО"].ToString().Split(';')[1];
                values[i] = row["ГСМ;МО"].ToString().Split(';')[0] + "/" + row["Средняя по МО на последнюю дату"].ToString() + "/" + row["Темп роста по МО"].ToString();
            }
            Array.Sort(names, values);
            LabelText.Text = "В отчетном периоде в МО ХМАО-Югры темп роста, превышающий среднеокружные показатели отмечен в:";
            string prevMO = String.Empty;
            for (int i = 0; i < names.Length; ++i)
            {
                string mo = names[i].Replace(" муниципальный район", " МР");
                string fuel = values[i].Split('/')[0].Replace("Бензин марки ", String.Empty).Replace("Дизельное топливо", "ДТ");
                double cost = Convert.ToDouble(values[i].Split('/')[1]);
                double grown = Convert.ToDouble(values[i].Split('/')[2]);
                if (mo != prevMO)
                {
                    prevMO = mo;
                    LabelText.Text += String.Format("<br/><i>{0}:</i>", mo);
                }
                LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0} (цена - {1:N2} руб./л.) - &nbsp;<span class='DigitsValue'>{2:P2}</span>", fuel, cost, grown);
            }

        }

        protected void FillTable()
        {
            /*DataTable dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_v_grid_hmao");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Вид ГСМ", dtGrid);*/

            lastDateParam.Value = lastDateParam.Value.Replace("[Период__День]", "[Период__Период]");
            prevDateParam.Value = prevDateParam.Value.Replace("[Период__День]", "[Период__Период]");

            DataTable dtGridRf = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_v_grid_rf");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Вид ГСМ", dtGridRf);

            if (dtGrid.Rows.Count == 0)
                return;

            LabelGrid.Text = "<table Class=HtmlTable width=310px>";
            LabelGrid.Text += "<tr><th Class=HtmlTableHeader style=\"width: 110px; border-color: black;\">Вид ГСМ</th>";
            LabelGrid.Text += "<th Class=HtmlTableHeader style=\"width: 100px; border-color: black;\">ХМАО</th>";
            LabelGrid.Text += "<th Class=HtmlTableHeader style=\"width: 100px; border-color: black;\">РФ</th></tr>";

            for (int i = 0; i < dtGrid.Rows.Count && i < dtGridRf.Rows.Count; ++i)
            {
                DataRow rowHMAO = dtGrid.Rows[i];
                DataRow rowRF = dtGridRf.Rows[i];
                string fuel = rowHMAO["Вид ГСМ"].ToString();
                string hmao = String.Format("{0:N2}<br/>{1:N2}<br/>{2:P2}",
                    rowHMAO["Средняя по субъекту на последнюю дату"], rowHMAO["Прирост"], rowHMAO["Темп прироста"]);
                string rf = String.Format("{0:N2}<br/>{1:N2}<br/>{2:P2}",
                    rowRF["Цена на последнюю дату"], rowRF["Прирост"], rowRF["Темп прироста"]);

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
                if (MathHelper.IsDouble(rowRF["Прирост"]))
                    if (MathHelper.AboveZero(rowRF["Прирост"]))
                    {
                        imageRF = " background-image: url(../../../images/arrowRedUpBB.png); background-repeat: no-repeat; background-position: 5% center;";
                    }
                    else if (MathHelper.SubZero(rowRF["Прирост"]))
                    {
                        imageRF = " background-image: url(../../../images/arrowGreenDownBB.png); background-repeat: no-repeat; background-position: 5% center;";
                    }

                LabelGrid.Text += "<tr>";

                LabelGrid.Text += String.Format("<td>{0}</td>", fuel);
                LabelGrid.Text += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", hmao, imageHMAO);
                LabelGrid.Text += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", rf, imageRF);

                LabelGrid.Text += "</tr>";
            }

            LabelGrid.Text += "</table>";
        }

    }
}
