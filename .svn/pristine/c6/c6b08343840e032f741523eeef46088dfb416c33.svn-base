using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Core.Primitives;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class EO_0002_0003 : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private string multiplierCaption = "млн.руб.";

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;

        #endregion


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            #region Настройка гриа


            GRBSGridBrick.Height = Unit.Empty;
            GRBSGridBrick.Width = Unit.Empty;
            GRBSGridBrick.InitializeLayout += new InitializeLayoutEventHandler(GRBSGrid_InitializeLayout);
            GRBSGridBrick.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            UltraChart11.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart11_FillSceneGraph);

            UltraChart21.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart21_FillSceneGraph);

            UltraChart31.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart31_FillSceneGraph);
            UltraChart32.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart32_FillSceneGraph);

            UltraChart41.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart41_FillSceneGraph);
            UltraChart42.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart42_FillSceneGraph);
            UltraChart43.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart43_FillSceneGraph);
            UltraChart44.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart44_FillSceneGraph);

            UltraChart51.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart51_FillSceneGraph);
            UltraChart52.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart52_FillSceneGraph);

            UltraChart61.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart61_FillSceneGraph);

            UltraChart71.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart71_FillSceneGraph);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            currentDate = CubeInfo.GetLastDate(DataProvidersFactory.SecondaryMASDataProvider, "FK_0004_0006_lastDate");

            selectedPeriod.Value = CRHelper.PeriodMemberUName("[Период].[Период]", currentDate, 4);
            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            IPadElementHeader10.Text = String.Format(IPadElementHeader10.Text, currentDate.AddMonths(1));

            GridDataBind();
            InitializeChart();

            DataTable dtPercent = new DataTable();
            string query = DataProvider.GetQueryText("EO_0002_0003_percent");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtPercent);

            Label2.Text = String.Format(@"<table>
<tr><td align=center>{1}</td></tr><tr><td align=center><span style='font-family: Arial; font-size: 18px; color: white'>{0:P2}</span></td><tr>
<td align=center>Процент исполнения финансирования за&nbsp;<span class='DigitsValue'>{2:yyyy}г.</span>&nbsp;по состоянию на&nbsp;<span class='DigitsValue'>{2:dd.MM.yyyy}</span></td></tr></table>", dtPercent.Rows[0][1], GetGaugeUrl(dtPercent.Rows[0][1]), currentDate);

            UserParams.Filter.Value = "[Исполнение расходов__Задачи].[Исполнение расходов__Задачи].[Все задачи].[Проведение лесоустроительных работ]";
            DataTable dtAction = new DataTable();
            query = DataProvider.GetQueryText("EO_0002_0003_Action");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtAction);

            ConfigureActionChart(UltraChart11, dtAction, 1, "1.1");

            UserParams.Filter.Value = "[Исполнение расходов__Задачи].[Исполнение расходов__Задачи].[Все задачи].[Создание условий реализации инвестиционных проектов]";
            dtAction = new DataTable();
            query = DataProvider.GetQueryText("EO_0002_0003_Action");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtAction);

            ConfigureActionChart(UltraChart21, dtAction, 1, "2.1");

            UserParams.Filter.Value = "[Исполнение расходов__Задачи].[Исполнение расходов__Задачи].[Все задачи].[Повышение уровня использования земель лесного фонда]";
            dtAction = new DataTable();
            query = DataProvider.GetQueryText("EO_0002_0003_Action");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtAction);

            ConfigureActionChart(UltraChart31, dtAction, 1, "3.1");
            ConfigureActionChart(UltraChart32, dtAction, 2, "3.2");

            UserParams.Filter.Value = @"[Исполнение расходов__Задачи].[Исполнение расходов__Задачи].[Все задачи].[Повышение уровня использования низкосортной древесины и древесных отходов]";
            dtAction = new DataTable();
            query = DataProvider.GetQueryText("EO_0002_0003_Action");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtAction);

            ConfigureActionChart(UltraChart41, dtAction, 1, "4.1");
            ConfigureActionChart(UltraChart42, dtAction, 2, "4.2");
            ConfigureActionChart(UltraChart43, dtAction, 3, "4.3");
            ConfigureActionChart(UltraChart44, dtAction, 4, "4.4");

            UserParams.Filter.Value = @"[Исполнение расходов__Задачи].[Исполнение расходов__Задачи].[Все задачи].[Модернизация действующих производств]";
            dtAction = new DataTable();
            query = DataProvider.GetQueryText("EO_0002_0003_Action");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtAction);

            ConfigureActionChart(UltraChart51, dtAction, 1, "5.1");
            ConfigureActionChart(UltraChart52, dtAction, 2, "5.2");

            UserParams.Filter.Value = @"[Исполнение расходов__Задачи].[Исполнение расходов__Задачи].[Все задачи].[Развитие сети лесовозных дорог круглогодового действия для обеспечения доступности лесных ресурсов]";
            dtAction = new DataTable();
            query = DataProvider.GetQueryText("EO_0002_0003_Action");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtAction);

            ConfigureActionChart(UltraChart61, dtAction, 1, "6.1");

            UserParams.Filter.Value = @"[Исполнение расходов__Задачи].[Исполнение расходов__Задачи].[Все задачи].[Обеспечение привлекательности лесной отрасли для инвесторов, представление достижений и задач отрасли]";
            dtAction = new DataTable();
            query = DataProvider.GetQueryText("EO_0002_0003_Action");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtAction);

            ConfigureActionChart(UltraChart71, dtAction, 1, "7.1");
        }

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("EO_0002_0003_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);
            if (gridDt.Columns.Count > 0)
            {
                //  gridDt.Columns.RemoveAt(0);
            }

            GRBSGridBrick.DataSource = gridDt;
            GRBSGridBrick.DataBind();
        }

        void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Style.Font.Bold = true;
                e.Row.Style.BorderDetails.WidthBottom = 3;
            }
        }

        protected string GetGaugeUrl(object oValue)
        {
            if (oValue == DBNull.Value)
                return String.Empty;
            double value = Convert.ToDouble(oValue) * 100;
            if (value > 100)
                value = 100;
            string path = "Prog_0002_0001_gauge_" + value.ToString("N0") + ".png";
            string returnPath = String.Format("<img style=\"margin-left: 10px\" src=\"../../../TemporaryImages/{0}\"/>", path);
            string serverPath = String.Format("~/TemporaryImages/{0}", path);

            if (File.Exists(Server.MapPath(serverPath)))
            {
                return returnPath;
            }
            Infragistics.UltraGauge.Resources.LinearGaugeScale scale = ((Infragistics.UltraGauge.Resources.LinearGauge)Gauge.Gauges[0]).Scales[0];
            scale.Markers[0].Value = value;
            Infragistics.UltraGauge.Resources.MultiStopLinearGradientBrushElement BrushElement = (Infragistics.UltraGauge.Resources.MultiStopLinearGradientBrushElement)(scale.Markers[0].BrushElement);
            BrushElement.ColorStops.Clear();
            if (value > 70)
            {

                BrushElement.ColorStops.Add(Color.FromArgb(223, 255, 192), 0);
                BrushElement.ColorStops.Add(Color.FromArgb(128, 255, 128), 0.41F);
                BrushElement.ColorStops.Add(Color.FromArgb(0, 192, 0), 0.428F);
                BrushElement.ColorStops.Add(Color.Green, 1);
            }
            else if (value < 30)
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

            Size size = new Size(200, 40);
            Gauge.SaveTo(Server.MapPath(serverPath), Infragistics.UltraGauge.Resources.GaugeImageType.Png, size);
            return returnPath;
        }

        protected void GRBSGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            int columnCount = e.Layout.Bands[0].Columns.Count;

            for (int i = 1; i < columnCount; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N1");
                e.Layout.Bands[0].Columns[i].Width = 70;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[0].Width = 58;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Bold = true;
            e.Layout.Bands[0].Columns[1].CellStyle.Font.Bold = true;
            e.Layout.Bands[0].Columns[2].CellStyle.Font.Bold = true;
            //e.Layout.Bands[0].Columns[3].CellStyle.Font.Size = 14;
            //e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(175);

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[4].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[6].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[8].CellStyle.BorderDetails.WidthRight = 3;
            e.Layout.Bands[0].Columns[10].CellStyle.BorderDetails.WidthRight = 3;

            GridHeaderLayout headerLayout = new GridHeaderLayout(e.Layout.Grid);

            GridHeaderCell cell = headerLayout.AddCell("Финансирование программы", 2);
            cell.AddCell("");
            cell.AddCell("План");
            cell.AddCell("Факт");

            cell = headerLayout.AddCell("в т.ч. по источникам финансирования:");
            GridHeaderCell childCell = cell.AddCell("Федеральный бюджет");
            childCell.AddCell("План");
            childCell.AddCell("Факт");

            childCell = cell.AddCell("Бюджет автономного округа");
            childCell.AddCell("План");
            childCell.AddCell("Факт");

            childCell = cell.AddCell("Бюджеты муниципальных образований");
            childCell.AddCell("План");
            childCell.AddCell("Факт");

            childCell = cell.AddCell("Другие источники");
            childCell.AddCell("План");
            childCell.AddCell("Факт");

            headerLayout.ApplyHeaderInfo();
        }

        #region Диаграмма

        private void InitializeChart()
        {
            DataTable dt = new DataTable();

            string query = DataProvider.GetQueryText("EO_0002_0003_Chart");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt);

            UltraChart1.Series.Clear();
            for (int j = 1; j < dt.Columns.Count; j++)
            {
                UltraChart1.Series.Add(CRHelper.GetNumericSeries(j, dt));
            }

            //UltraChart.DataSource = dt;            

            UltraChart1.Width = 750;
            UltraChart1.Height = 245;
            // UltraChart.Style.Add("margin-right", "")

            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "млн.руб.";
            UltraChart1.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.FontColor = Color.FromArgb(209, 209, 209);
            UltraChart1.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart1.Axis.X.Extent = 40;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.Y.Extent = 40;

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetColor(i);

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }

            UltraChart1.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><SERIES_LABEL>г.<br/><ITEM_LABEL>\n<b><DATA_VALUE:N3></b>&nbsp;млн.руб.</span>";
            UltraChart1.Data.SwapRowsAndColumns = true;

            UltraChart1.Legend.Visible = false;

            UltraChart1.DataBind();
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {                
                case 1:
                    {
                        return Color.DarkTurquoise;
                    }                
                case 2:
                    {
                        return Color.RoyalBlue;
                    }
                default:
                    {
                        return Color.RoyalBlue;
                    }
            }
        }

        #endregion

        #region Мероприятия
        private void ConfigureActionChart(UltraChart chart, DataTable dataSource, int dataIndex, string num)
        {
            chart.Width = 100;
            chart.Height = 100;
            chart.ChartType = ChartType.PieChart;
            chart.Border.Thickness = 0;
            chart.Axis.Y.Extent = 5;
            chart.Axis.X.Extent = 5;
            chart.Tooltips.FormatString = "<SERIES_LABEL>";
            chart.Legend.Visible = false;
            chart.Axis.X.Labels.SeriesLabels.Visible = false;
            chart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            chart.PieChart.Labels.Visible = false;
            chart.PieChart.Labels.LeaderLineThickness = 0;
            //chart.PieChart.RadiusFactor = 110;
            // chart.Style.Add("padding", "0px");
            //  chart.Style.Add("margin", "-3px");

            double percent = 0;
            if (dataSource.Rows[3][dataIndex] != DBNull.Value)
            {
                double.TryParse(dataSource.Rows[3][dataIndex].ToString(), out percent);
                percent = percent * 100;
            }

            double plan = 0;
            if (dataSource.Rows[1][dataIndex] != DBNull.Value)
            {
                double.TryParse(dataSource.Rows[1][dataIndex].ToString(), out plan);
            }

            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            chart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                Color color = Color.Gray;
                Color colorEnd = Color.Gray;
                PaintElement pe = new PaintElement();
                if (plan != 0)
                {
                    if (percent < 30)
                    {
                        color = Color.Red;
                        colorEnd = Color.Red;
                    }
                    else if (percent >= 30 && percent < 70)
                    {
                        color = Color.Yellow;
                        colorEnd = Color.Yellow;
                    }
                    else if (percent >= 70)
                    {
                        color = Color.Green;
                        colorEnd = Color.Green;
                    }
                }

                pe.Fill = color;
                pe.FillStopColor = colorEnd;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.Horizontal;
                pe.FillOpacity = 150;
                chart.ColorModel.Skin.PEs.Add(pe);
            }
            //chart.Style.Add("margin-top", " -10px");

            DataTable actionDataTable = new DataTable();
            actionDataTable.Columns.Add("name", typeof(string));
            actionDataTable.Columns.Add("value", typeof(double));

            object[] fictiveValue = { GetActionHint(dataSource, dataIndex, num), 100 };

            actionDataTable.Rows.Add(fictiveValue);
            chart.DataSource = actionDataTable;
            chart.DataBind();
        }

        private string GetActionHint(DataTable dataSource, int dataIndex, string num)
        {
            string hint = @"<span style='font-family: Arial; font-size: 14pt'>Мероприятие&nbsp;<b>{0}</b>:&nbsp;{1}
Исполнитель:&nbsp;{2}
Выделено&nbsp;финансирования&nbsp;в&nbsp;{6:yyyy}&nbsp;году: <b>{3:N2}</b>&nbsp;млн.руб.
Фактически&nbsp;финансировано&nbsp;в&nbsp;{6:yyyy}&nbsp;году: <b>{4:N2}</b>&nbsp;млн.руб.
Процент&nbsp;исполнения:&nbsp;<b>{5:P2}</b></span>";

            double plan = 0;
            if (dataSource.Rows[1][dataIndex] != DBNull.Value)
            {
                double.TryParse(dataSource.Rows[1][dataIndex].ToString(), out plan);
            }
            double fact = 0;
            if (dataSource.Rows[1][dataIndex] != DBNull.Value)
            {
                double.TryParse(dataSource.Rows[2][dataIndex].ToString(), out fact);
            }
            double percent = 0;
            if (dataSource.Rows[1][dataIndex] != DBNull.Value)
            {
                double.TryParse(dataSource.Rows[3][dataIndex].ToString(), out percent);
            }

            return GetWarpedHint(String.Format(hint, num,
                dataSource.Columns[dataIndex].ColumnName,
                dataSource.Rows[0][dataIndex],
                plan,
                fact,
                percent,
                currentDate));
        }

        void UltraChart11_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "M 1.1");
        }

        void UltraChart21_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "M 2.1");
        }

        void UltraChart22_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "M 2.2");
        }

        void UltraChart31_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "M 3.1");
        }

        void UltraChart32_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "M 3.2");
        }

        void UltraChart41_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "M 4.1");
        }

        void UltraChart42_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "M 4.2");
        }

        void UltraChart43_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "M 4.3");
        }

        void UltraChart44_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "M 4.4");
        }

        void UltraChart51_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "M 5.1");
        }

        void UltraChart52_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "M 5.2");
        }

        void UltraChart61_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "M 6.1");
        }

        void UltraChart71_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, "M 7.1");
        }

        private static void AddText(FillSceneGraphEventArgs e, string textValue)
        {
            Label lb = new Label();
            Text text = new Text(new Point(17, 50), textValue);
            LabelStyle style = new LabelStyle();

            style.Font = new Font("Arial", 16, FontStyle.Bold);
            //style.Font.Bold = true;
            style.FontColor = Color.White;
            text.SetLabelStyle(style);
            e.SceneGraph.Add(text);
        }

        private string GetWarpedHint(string hint)
        {
            string name = hint.Replace("\"", "'");
            if (name.Length > 40)
            {
                int k = 0;

                for (int j = 0; j < name.Length; j++)
                {
                    k++;
                    if (k > 40 && name[j] == ' ')
                    {
                        name = name.Insert(j, "<br/>");
                        k = 0;
                    }
                }
            }
            return name;
        }

        #endregion
    }
}
