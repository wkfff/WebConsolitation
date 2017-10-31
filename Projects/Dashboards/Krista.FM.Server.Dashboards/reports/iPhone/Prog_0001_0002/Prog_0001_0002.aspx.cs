using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.UI.WebControls;

using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGrid;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class Prog_0001_0002 : CustomReportPage
    {

        CustomParam progDataSourceParam;

        DataTable dtData;
        DataTable dtChart2;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (String.IsNullOrEmpty(UserParams.Prog.Value))
            {
                CustomParams.MakeProgParams("18", "id");
            }

            ProgTitle.Text = UserParams.Prog.Value;

            #region Инициализация параметров запроса

            progDataSourceParam = UserParams.CustomParam("prog_data_source");

            #endregion

            #region Получение дат
            DataTable dtMonth = new DataTable();
            string query = DataProvider.GetQueryText("Prog_0001_0002_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Даты", dtMonth);
            UserParams.PeriodMonth.Value = dtMonth.Rows[0][1].ToString();
            DateTime date = CRHelper.DateByPeriodMemberUName(UserParams.PeriodMonth.Value, 3);
            progDataSourceParam.Value = String.Format("ЭО\\0005 Программы - вариант: {0:yyyy}, ДЦП, месяц: {0:MM}, территория: Ненецкий автономный округ",
                CRHelper.DateByPeriodMemberUName(UserParams.PeriodMonth.Value, 3));

            #endregion

            dtData = new DataTable();
            query = DataProvider.GetQueryText("Prog_0001_0002_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные о программе", dtData);

            BindTextPart1(dtData.Rows[0]);
            BindTextPart2(dtData.Rows[0]);
            BindTextPart3(dtData.Rows[0]);
            BindTextPart4(dtData.Rows[0]);

            #region Настройка диаграммы 1

            UltraChart1.ChartType = ChartType.BarChart;
            UltraChart1.Width = Unit.Parse("260px");
            UltraChart1.Data.ZeroAligned = true;
            UltraChart1.Legend.Visible = false;
            UltraChart1.Axis.X.LineColor = Color.FromArgb(192, 192, 192);
            UltraChart1.Axis.Y.LineColor = Color.FromArgb(192, 192, 192);
            UltraChart1.Axis.X.Extent = 0;
            UltraChart1.Axis.Y.Extent = 50;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Visible = false;
            UltraChart1.Axis.Y.Labels.Visible = true;
            UltraChart1.Axis.Y.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 10);
            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL> год\n<DATA_VALUE:N0> тыс.руб.";

            UltraChart1.Data.SwapRowsAndColumns = true;

            //UltraChart1.Style.Add("margin-right", "-5px");
            UltraChart1.Style.Add("margin-top", "-18px");

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            PaintElement pe = new PaintElement();
            Color color = Color.FromArgb(192, 178, 224);
            Color stopColor = Color.FromArgb(44, 20, 91);
            pe.Fill = color;
            pe.FillStopColor = stopColor;
            pe.ElementType = PaintElementType.Gradient;
            pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
            pe.FillOpacity = 150;
            UltraChart1.ColorModel.Skin.PEs.Add(pe);

            UltraChart1.ColorModel.Skin.ApplyRowWise = false;

            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);

            UltraChart1.DataBind();

            #endregion

            dtChart2 = new DataTable();
            query = DataProvider.GetQueryText("Prog_0001_0002_chart2");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart2);

            #region Настройка диаграммы 2

            UltraChart2.Width = 240;
            UltraChart2.Height = 100;

            UltraChart2.ChartType = ChartType.PieChart;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.Axis.Y.Extent = 0;
            UltraChart2.Axis.X.Extent = 0;
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N0> человек (<PERCENT_VALUE:N2>%)";
            UltraChart2.Legend.Visible = false;
            UltraChart2.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart2.Data.ZeroAligned = true;
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.PieChart.Labels.FormatString = "<ITEM_LABEL>\n<PERCENT_VALUE:N2>%";
            UltraChart2.PieChart.Labels.FontColor = Color.FromArgb(192, 192, 192);
            UltraChart2.PieChart.Labels.LeaderLineColor = Color.FromArgb(192, 192, 192);
            UltraChart2.PieChart.Labels.LeaderDrawStyle = LineDrawStyle.Solid;
            UltraChart2.PieChart.Labels.LeaderEndStyle = LineCapStyle.Square;
            UltraChart2.PieChart.Labels.Font = new Font("Verdana", 10);
            UltraChart2.PieChart.OthersCategoryPercent = 0;
            UltraChart2.PieChart.RadiusFactor = 110;

            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart2.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                pe = new PaintElement();
                color = Color.White;
                stopColor = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Green;
                            stopColor = Color.ForestGreen;
                            break;
                        }
                    case 2:
                        {
                            color = Color.Red;
                            stopColor = Color.Gold;
                            break;
                        }
                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart2.ColorModel.Skin.PEs.Add(pe);
            }

            UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);
            UltraChart2.DataBind();

            #endregion

            tbGauges.Width = "758px";
            BindTextWithGauges(date);

            GridHeader.Text = String.Format("Исполнение программы по состоянию на {0:01.MM.yyyy} г.", date.AddMonths(1));
            BindTables(date);

        }

        #region Грид

        private void BindTables(DateTime date)
        {
            string query = DataProvider.GetQueryText("Prog_0001_0002_grid");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Программа", dtGrid);

            LabelGrid.Text = "<table Class=\"HtmlTableCompact\" width=\"760px\" style=\"border: black 0px solid;\">";

            LabelGrid.Text += GetTable(dtGrid, 2, 3, "Общий объем финансирования", date, 4, false);
            LabelGrid.Text += GetTable(dtGrid, 3, 3, "Финансирование за счет бюджета субъекта", date, 4, true);
            LabelGrid.Text += GetTable(dtGrid, 4, 3, "Финансирование за счет бюджета МО", date, 4, false);

            LabelGrid.Text += "</table>";

        }

        private string GetTable(DataTable dt, int startColumn, int step, string tableTitle, DateTime date, int testColumn, bool mainTable)
        {
            if (!MathHelper.IsDouble(dt.Rows[0][startColumn]) || Convert.ToDouble(dt.Rows[0][startColumn]) == 0)
                return String.Empty;

            if (!mainTable && (!MathHelper.IsDouble(dt.Rows[0][testColumn]) || Convert.ToDouble(dt.Rows[0][testColumn]) == 0))
                return String.Empty;

            string result = String.Format("<tr><td colspan=4 style=\"border: black 0px solid; font-weight: bold;\">{0}</td></tr>", tableTitle);

            result += "<tr Class=\"HtmlTableHeader\">";
            result += "<td width=400px style=\"border-color: #3c3c3c;\">&nbsp;</td>";
            result += String.Format("<td width=120px style=\"border-color: #3c3c3c;\">План финансирования на {0:yyyy} год, тыс.руб.</td>", date);
            result += String.Format("<td width=120px style=\"border-color: #3c3c3c;\">Выделенное финасирование с начала {0:yyyy} года, тыс.руб.</td>", date);
            result += String.Format("<td width=120px style=\"border-color: #3c3c3c;\">Освоено средств с начала {0:yyyy} года, тыс.руб.</td>", date);
            result += "</tr>";

            foreach (DataRow row in dt.Rows)
            {
                int level = 2;
                string textStyle = String.Empty;
                switch (row[1].ToString())
                {
                    case "Программа":
                        {
                            row[0] = "ВСЕГО ПО ПРОГРАММЕ";
                            textStyle = " font-weight: bold;";
                            level = 2;
                            break;
                        }
                    case "Исполнитель":
                        {
                            level = 22;
                            textStyle = " text-decoration: underline;";
                            break;
                        }
                    case "Вид расхода":
                        {
                            level = 42;
                            textStyle = " font-style: italic;";
                            break;
                        }
                    case "Детализация":
                        {
                            level = 62;
                            break;
                        }
                }
                result += "<tr style=\"text-align: right;\">";
                result += String.Format("<td style=\"padding-left: {1}px; text-align: left;{2}\">{0}</td>", row[0], level, textStyle);
                for (int i = startColumn; i < dt.Columns.Count; i += step)
                {
                    result += String.Format("<td>{0:N1}</td>", row[i]);
                }
                result += "</tr>";
            }

            return result;
        }

        #endregion

        #region Диаграмма 1

        private void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string dataString = dtData.Rows[0]["Объем и источники финансирования"].ToString();
            if (dataString.IndexOf(", в том числе:") == -1)
            {
                TableFin.Visible = false;
                LabelFin.Visible = true;
                LabelFin.Text = dataString;
            }
            else
            {
                LabelFin.Visible = false;
                TableFin.Visible = true;
                LabelTotalFin.Text = dataString.Substring(0, dataString.IndexOf(", в том числе:")).Replace("бщий объем финансирования: ", "бщий объем финансирования:<br/>");
                UltraChart1.Visible = true;
                string[] data = dataString.Substring(dataString.IndexOf(", в том числе:") + 14)
                    .Replace(" ", String.Empty).Replace("г.", String.Empty).Replace("года", String.Empty).Replace("тыс.руб.", String.Empty).Split(';');
                DataTable dtChart = new DataTable();
                dtChart.Columns.Add("Год", typeof(string));
                dtChart.Columns.Add("Финансирование", typeof(double));
                LabelSplit.Text = String.Empty;
                foreach (string s in data)
                {
                    DataRow row = dtChart.NewRow();
                    char[] charsToTrim = { '\n', '\r', ' ' };
                    row[0] = s.Split('-')[0].Trim(charsToTrim);
                    row[1] = Convert.ToDouble(s.Split('-')[1].Trim(charsToTrim));
                    dtChart.Rows.InsertAt(row, 0);
                    LabelSplit.Text += String.Format("{0} г. - {1:N0} тыс.руб.<br/>", row[0], row[1]);
                }
                UltraChart1.Height = new Unit(dtChart.Rows.Count * 50);
                UltraChart1.DataSource = dtChart;
            }
        }

        #endregion

        #region Диаграмма 2

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            dtChart2.Rows[0][0] = "Субъект";
            dtChart2.Rows[1][0] = "МО";

            UltraChart2.DataSource = dtChart2;
        }

        #endregion

        #region Текст

        private void BindTextPart1(DataRow row)
        {
            Text1.Text = String.Format("<span class='DigitsValue'>Основание для разработки:</span>&nbsp;{0}<br/>", row["Основание для разработки программы"]);
            Text1.Text += String.Format("<span class='DigitsValue'>Срок реализации:</span>&nbsp;{0}<br/>", row["Сроки реализации"]);
            Text1.Text += String.Format("<span class='DigitsValue'>Ожидаемый результат:</span>&nbsp;{0}<br/>", row["Ожидаемый результат реализации"]);
            Text1.Text += String.Format("<span class='DigitsValue'>Контроль за исполнением:</span>&nbsp;{0}", row["Система реализации контроля за исполнением"]);
        }

        private void BindTextPart2(DataRow row)
        {
            Text2.Text = row["Цель программы"].ToString().Replace("; - ", ";<br/>- ");
        }

        private void BindTextPart3(DataRow row)
        {
            Text3.Text = row["Задачи программы"].ToString().Replace("; - ", ";<br/>- ");
        }

        private void BindTextPart4(DataRow row)
        {
            Text4.Text = row["Перечень основных мероприятий"].ToString().Replace("; - ", ";<br/>- ");
        }

        #endregion

        #region Гейджи

        private void BindTextWithGauges(DateTime date)
        {
            DataTable dtGauges = new DataTable();
            string query = DataProvider.GetQueryText("Prog_0001_0002_gauges");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Данные для гейджей", dtGauges);
            DataRow row = dtGauges.Rows[0];

            tbGauges.Rows[0].Cells[1].InnerText = String.Format("План на {0:yyyy} г.", date);
            tbGauges.Rows[1].Cells[0].InnerText = String.Format("Исполнено на {0:01.MM.yyyy} г.", date.AddMonths(1));
            tbGauges.Rows[2].Cells[0].InnerText = String.Format("Освоено на {0:01.MM.yyyy} г.", date.AddMonths(1));

            tbGauges.Rows[0].Cells[2].InnerText = String.Format("{0:N0} тыс.руб.", row["План"]);
            tbGauges.Rows[1].Cells[1].InnerText = String.Format("{0:N0} тыс.руб.", row["Исполнено"]);
            tbGauges.Rows[2].Cells[1].InnerText = String.Format("{0:N0} тыс.руб.", row["Освоено"]);

            tbGauges.Rows[1].Cells[2].InnerHtml = String.Format("{0}", GetGaugeUrl(row["Исполнено (процент)"]));
            tbGauges.Rows[2].Cells[2].InnerHtml = String.Format("{0}", GetGaugeUrl(row["Освоено (процент)"]));

            tbGauges.Rows[1].Cells[3].InnerHtml = String.Format("{0:P2}<br/>от плана", row["Исполнено (процент)"]);
            tbGauges.Rows[2].Cells[3].InnerHtml = String.Format("{0:P2}<br/>от выд. средств", row["Освоено (процент)"]);
        }

        protected string GetGaugeUrl(object oValue)
        {
            if (oValue == DBNull.Value)
                return String.Empty;
            double value = Convert.ToDouble(oValue) * 100;
            if (value > 100)
                value = 100;
            string path = "Prog_0001_0001_gauge_" + value.ToString("N0") + ".png";
            string returnPath = String.Format("<img style=\"FLOAT: left;\" src=\"../../../TemporaryImages/{0}\"/>", path);
            string serverPath = String.Format("~/TemporaryImages/{0}", path);

            if (File.Exists(Server.MapPath(serverPath)))
                return returnPath;
            LinearGaugeScale scale = ((LinearGauge)Gauge.Gauges[0]).Scales[0];
            scale.Markers[0].Value = value;
            MultiStopLinearGradientBrushElement BrushElement = (MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);
            BrushElement.ColorStops.Clear();
            if (value > 90)
            {

                BrushElement.ColorStops.Add(Color.FromArgb(223, 255, 192), 0);
                BrushElement.ColorStops.Add(Color.FromArgb(128, 255, 128), 0.41F);
                BrushElement.ColorStops.Add(Color.FromArgb(0, 192, 0), 0.428F);
                BrushElement.ColorStops.Add(Color.Green, 1);
            }
            else
            {
                if (value < 50)
                {

                    BrushElement.ColorStops.Add(Color.FromArgb(253, 119, 119), 0);
                    BrushElement.ColorStops.Add(Color.FromArgb(239, 87, 87), 0.41F);
                    BrushElement.ColorStops.Add(Color.FromArgb(224, 0, 0), 0.428F);
                    BrushElement.ColorStops.Add(Color.FromArgb(199, 0, 0), 1);
                }
                else
                {
                    BrushElement.ColorStops.Add(Color.FromArgb(255, 255, 128), 0);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.41F);
                    BrushElement.ColorStops.Add(Color.Yellow, 0.428F);
                    BrushElement.ColorStops.Add(Color.FromArgb(255, 128, 0), 1);
                }
            }

            Size size = new Size(100, 40);
            Gauge.SaveTo(Server.MapPath(serverPath), GaugeImageType.Png, size);
            return returnPath;
        }
        
        #endregion

    }
}
