using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Drawing.Imaging;
using System.IO;
using Infragistics.Documents.Reports.Report.Text;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0007
{
    public partial class DefaultAssessment : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2000;
        private int endYear = 2011;
        private string deficitName = string.Empty;

        private GridHeaderLayout headerLayout;

        /// <summary>
        /// Выбраны ли все федеральные округа
        /// </summary>
        public bool AllFO
        {
            get { return ComboFO.SelectedIndex == 0; }
        }

        /// <summary>
        /// Выбран ли расчет "and"
        /// </summary>
        public bool AndCalculation
        {
            get { return CalculationButtonList.SelectedIndex == 1; }
        }

        #region Параметры запроса

        // Выбранный вид расчета
        private CustomParam deficitOperation;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.7 - 225);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.7);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            #region Инициализация параметров запроса

            if (deficitOperation == null)
            {
                deficitOperation = UserParams.CustomParam("deficit_operation");
            }

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.ColumnChart.NullHandling = NullHandling.DontPlot;
            UltraChart.Data.EmptyStyle.LegendDisplayType = LegendEmptyDisplayType.PE;
            UltraChart.Data.EmptyStyle.ShowInLegend = true;

            UltraChart.Axis.X.Extent = 60;
            UltraChart.Axis.Y.Extent = 45;
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 20);
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart.Axis.Y.Labels.Visible = true;

            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.SpanPercentage = 6;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value) / 2;
            UltraChart.Legend.Margins.Bottom = 5;

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:N3> млн.руб.";

            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            UltraChart.TitleTop.Visible = false;
            UltraChart.TitleBottom.Visible = false;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Text = "Млн.руб.";
            UltraChart.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.VerticalAlign = StringAlignment.Near;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Margins.Right = 1;
            UltraChart.TitleLeft.Margins.Left = 1;

            UltraChart.BorderColor = Color.Transparent;
            UltraChart.Data.SwapRowsAndColumns = true;
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0011_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            endYear = Convert.ToInt32(dtDate.Rows[0][0]);

            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboFO.Title = "ФО";
                ComboFO.Width = 300;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
                ComboFO.SetСheckedState("Все федеральные округа", true);

                UserParams.Filter.Value = ComboFO.SelectedValue;
                UserParams.Subject.Value = string.Empty;
            }

            Page.Title = string.Format("Оценка дефицита бюджетов субъектов {0}", ComboFO.SelectedIndex == 0
                                                                                     ? "РФ"
                                                                                     : RegionsNamingHelper.ShortName(
                                                                                         ComboFO.SelectedValue));
            Label1.Text =
                string.Format(
                    "Оценка соблюдения статьи 92.1 Бюджетного кодекса РФ по предельному объему дефицита бюджетов субъектов {0}",
                    ComboFO.SelectedIndex == 0
                        ? "РФ"
                        : RegionsNamingHelper.ShortName(ComboFO.SelectedValue));

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());

            string calculationKind = AndCalculation
                                         ? "с учетом поступлений от продажи акции И снижения остатков средств бюджета"
                                         : "с учетом поступлений от продажи акции ИЛИ снижения остатков средств бюджета";

            if (MeasureButtonList.SelectedIndex == 0)
            {
                deficitName = "Утвержденный дефицит";

                UserParams.SelectedMap.Value = "Назначено";
                UserParams.SelectItem.Value = "Назначено";

                if (monthNum == 12)
                {
                    monthNum = 1;
                }
                else
                {
                    monthNum++;
                }

                if (ComboYear.SelectedValue == endYear.ToString())
                {
                    Label2.Text = string.Format("План на {0} год по состоянию на 1 {1} ({2})", yearNum,
                                                CRHelper.RusMonthGenitive(monthNum), calculationKind);
                }
                else
                {
                    Label2.Text = string.Format("План на {0} год ({1})", yearNum, calculationKind);
                }
            }
            else
            {
                deficitName = "Фактический дефицит";

                UserParams.SelectItem.Value = "Исполнено";

                if (ComboYear.SelectedValue == endYear.ToString())
                {
                    UserParams.SelectedMap.Value = "Назначено";
                    Label2.Text =
                        string.Format(
                            "Факт за {0} {1} {2} года, оценка предельного дефицита по плану на {2} год ({3})", monthNum,
                            CRHelper.RusManyMonthGenitive(monthNum), yearNum, calculationKind);
                }
                else
                {
                    UserParams.SelectedMap.Value = "Исполнено";
                    Label2.Text = string.Format("Факт за {0} {1} {2} года {3}", 12,
                                                CRHelper.RusManyMonthGenitive(monthNum), yearNum, calculationKind);
                }
            }

            int year = Convert.ToInt32(ComboYear.SelectedValue);

            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodEndYear.Value = (year - 2).ToString();
            UserParams.PeriodFirstYear.Value = (year - 3).ToString();
            UserParams.PeriodYear.Value = year.ToString();

            if (ComboYear.SelectedValue == endYear.ToString())
            {
                string month = dtDate.Rows[0][3].ToString();
                string halfYear = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(month)));
                string quarter = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum((CRHelper.MonthNum(month))));
                UserParams.PeriodDayFK.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year, halfYear, quarter, month);
                UserParams.VariantMesOtch.Value = "Максимально допустимый дефицит";
            }
            else
            {
                UserParams.VariantMesOtch.Value = "Допустимый дефицит";
                UserParams.PeriodDayFK.Value = string.Format("[{0}].[Полугодие 2].[Квартал 4].[Декабрь]", year);
            }

            if (AllFO)
            {
                UserParams.Filter.Value = " ";
                UserParams.FKRFilter.Value = string.Format(" and ([КД].[Сопоставимый].[Текущий дефицит] <> 0)");
            }
            else
            {
                UserParams.Filter.Value = string.Format(" and ([Территории].[Сопоставимый].CurrentMember.Parent is [Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[{0}])",
                        ComboFO.SelectedValue);
                UserParams.FKRFilter.Value = " ";
            }

            deficitOperation.Value = AndCalculation ? "and" : "or";

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            string patternValue = UserParams.Subject.Value;
            int defaultRowIndex = 1;
            if (patternValue == string.Empty)
            {
                patternValue = UserParams.StateArea.Value;
                defaultRowIndex = 0;
            }

            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
            ActiveGridRow(row);

            UltraChart.DataBind();
        }

        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            UserParams.Subject.Value = row.Cells[0].Text;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0011_assessment_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value && row[i].ToString() != string.Empty &&
                        (i == 3 || i == 4 || i == 5 || i == 7 || i == 8 ||
                         i == 9 || i == 12))
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }
            }

            UltraWebGrid.DataSource = dtGrid;
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (ComboFO.SelectedIndex != 0)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count < 13)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(190);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            SetColumnParams(e.Layout, 0, 1, "", 40, false);
            SetColumnParams(e.Layout, 0, 2, "N0", 47, false);
            SetColumnParams(e.Layout, 0, 3, "N3", 90, false);
            SetColumnParams(e.Layout, 0, 4, "N3", 96, false);
            SetColumnParams(e.Layout, 0, 5, "N3", 96, false);
            SetColumnParams(e.Layout, 0, 6, "P2", 80, false);
            SetColumnParams(e.Layout, 0, 7, "N3", 85, false);
            SetColumnParams(e.Layout, 0, 8, "N3", 70, false);
            SetColumnParams(e.Layout, 0, 9, "N3", 108, false);
            SetColumnParams(e.Layout, 0, 10, "P2", 103, false);
            SetColumnParams(e.Layout, 0, 11, "", 80, true);
            SetColumnParams(e.Layout, 0, 12, "N3", 103, false);
            SetColumnParams(e.Layout, 0, 13, "", 69, true);

            headerLayout.AddCell("Субъект", 2).AddCell("1");
            headerLayout.AddCell("ФО", "Федеральный округ, которому принадлежит субъект", 2).AddCell("2");
            headerLayout.AddCell("Группа", "Группа по доле межбюджетных трансфертов\n(за исключением субвенций) в собственных\nдоходах консолидированного бюджета", 2).AddCell("3");
            headerLayout.AddCell(deficitName, String.Format("{0} бюджета субъекта{1}", deficitName, MeasureButtonList.SelectedIndex == 0 ? " на год" : String.Empty), 2).AddCell("4");
            headerLayout.AddCell("Предельный дефицит (с учетом бюджетных кредитов)", "Предельная сумма дефицита с учетом разницы\nмежду полученными и погашенными бюджетными\nкредитами от других бюджетов (№58-ФЗ\nот 9.04.2009 г.)", 2).AddCell("5 = 6 + 12");
            headerLayout.AddCell("Прежний предельный дефицит (по доле в доходах, установленной БК РФ)", "Статья 92.1 БК РФ: доля дефицита бюджета\nв доходах (без безвозмездных поступлений)\nне должна превышать 15% для субъектов\n2-4 групп и 10% для субъектов 1 группы", 2).AddCell("при 8>0, 9>0 6=7*10+8+9, иначе 6=7*10");

            GridHeaderCell cell = headerLayout.AddCell("Оценка предельного дефицита (по доле в доходах, установленной БК РФ)", "Оценка производится в соответствии с методикой,\nутвержденной Приказом МФ РФ №676\nот 29 декабря 2008 г.");
            cell.AddCell("Нормативное значение", "Для субъектов групп 2-4 – 15%,\nдля субъектов 1 группы – 10%").AddCell("7");
            cell.AddCell("Поступления от продажи акций", "Объем поступлений от продажи акций и\nиных форм участия в капитале, находящихся\nв собственности субъекта РФ").AddCell("8");
            cell.AddCell("Снижение остатков средств бюджета", "Величина снижения остатков средств\nна счетах по учету средств бюджета\nсубъекта РФ").AddCell("9");
            cell.AddCell("Доходы бюджета без безвозмездных поступлений", "Объем доходов бюджета субъекта за исключением\nбезвозмездных поступлений").AddCell("10");
            cell.AddCell("Доля дефицита в доходах без безвозмездных поступлений", "Отношение дефицита бюджета к доходам\n(без безвозмездных поступлений)").AddCell("при 8>0, 9>0 11=(4-8-9)/10, иначе 11=4/10");

            cell = headerLayout.AddCell("Оценка предельного дефицита (№58-ФЗ)", "Оценка производится согласно изменениям\nстатьи 92.1 БК РФ, внесенным Федеральным законом\nот 9.04.2009 г. №58-ФЗ");
            cell.AddCell("Разница между полученными и погашенными бюджетными кредитами", "С 2009 до 2013 года дефицит может превысить\nграницы, установленные п.2 статьи 92.1 БК РФ,\nв пределах утвержденной разницы между\nполученными и погашенными бюджетными\nкредитами от других бюджетов").AddCell("12");

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Top = 5;
                e.Row.Cells[i].Style.Padding.Bottom = 5;

                int indicateIndex = -1;
                int errorIndex = -1;
                string errorImg = string.Empty;
                string errorHint = string.Empty;
                if (i == 4)
                {
                    indicateIndex = i;
                    errorIndex = 11;
                    errorImg = (ComboYear.SelectedValue == endYear.ToString() ? "~/images/ballRedBB.png" : "~/images/ballRedBBdim.png");
                    errorHint = "Нарушение БК РФ";
                }
                else if (i == 5)
                {
                    indicateIndex = i;
                    errorIndex = 13;
                    errorImg = (ComboYear.SelectedValue == (endYear - 1).ToString() ? "~/images/ballRedBB.png" : "~/images/ballRedBBdim.png");
                    errorHint = "Превышение разрешенной доли дефицита в доходах бюджета";
                }

                if (indicateIndex != -1 && e.Row.Cells[errorIndex].Value != null && e.Row.Cells[errorIndex].Value.ToString() != string.Empty)
                {
                    if (e.Row.Cells[errorIndex].Value.ToString() == "Нарушение")
                    {
                        e.Row.Cells[indicateIndex].Style.BackgroundImage = errorImg;
                        e.Row.Cells[indicateIndex].Title = errorHint;
                    }
                    else
                    {
                        if (i == 4 && ComboYear.SelectedValue == endYear.ToString() ||
                            i == 5 && ComboYear.SelectedValue == (endYear - 1).ToString())
                        {
                            e.Row.Cells[indicateIndex].Style.BackgroundImage = "~/images/ballGreenBB.png";
                        }
                        else
                        {
                            e.Row.Cells[indicateIndex].Style.BackgroundImage = "~/images/ballGreenBBdim.png";
                        }
                        e.Row.Cells[indicateIndex].Title = "Нарушений нет";
                    }
                    e.Row.Cells[indicateIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }

            foreach (UltraGridCell cell in e.Row.Cells)
            {
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        #endregion

        #region Обработчики диаграммы


        /// <summary>
        /// Инвертирует следование строк в таблице
        /// </summary>
        /// <param name="dt">входная таблица</param>
        /// <returns>выходная таблица</returns>
        private static DataTable ReverseRowsDataTable(DataTable dt)
        {
            DataTable resDt = new DataTable();

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                DataColumn column = new DataColumn(dt.Columns[i].Caption, dt.Columns[i].DataType);
                resDt.Columns.Add(column);
            }

            for (int i = dt.Rows.Count; i > 0; i--)
            {
                DataRow row = resDt.NewRow();
                row.ItemArray = dt.Rows[i - 1].ItemArray;
                resDt.Rows.Add(row);
            }

            return resDt;
        }

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            if (dtGrid == null || dtGrid.Rows.Count == 0)
            {
                UltraWebGrid.Height = Unit.Empty;
                return;
            }

            string query = DataProvider.GetQueryText("FK_0001_0011_assessment_chart");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            RegionsNamingHelper.ReplaceRegionNames(dtChart, 0);

            foreach (DataRow row in dtChart.Rows)
            {
                for (int i = 1; i <= 3; i++)
                {
                    if (row[i] != DBNull.Value && row[i].ToString() != string.Empty)
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }
            }

            if (dtChart.Columns.Count < 4)
            {
                return;
            }

            UltraChart.Series.Clear();

            if (dtChart.Rows.Count > 20)
            {
                UltraChart.TitleLeft.Visible = false;
                UltraChart.TitleTop.Visible = true;
                UltraChart.TitleTop.Text = "Млн.руб.";
                UltraChart.TitleTop.HorizontalAlign = StringAlignment.Center;

                UltraChart.TitleBottom.Visible = true;
                UltraChart.TitleBottom.Text = "Млн.руб.";
                UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;

                UltraChart.ChartType = ChartType.BarChart;
                UltraChart.Height = 2000;
                UltraChart.Axis.X.Labels.Visible = true;
                UltraChart.Axis.X2.Visible = true;
                UltraChart.Axis.X2.Extent = 40;
                UltraChart.Axis.X.Labels.Font = new Font("Verdana", 14);
                UltraChart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                UltraChart.Axis.X2.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                UltraChart.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 14);
                UltraChart.Axis.Y.Labels.Visible = false;
                UltraChart.Axis.Y.Labels.SeriesLabels.Visible = true;
                UltraChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
                UltraChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Far;
                UltraChart.Axis.Y.Extent = 60;

                UltraChart.Axis.Y.StripLines.PE.Fill = Color.Gainsboro;
                UltraChart.Axis.Y.StripLines.PE.FillOpacity = 150;
                UltraChart.Axis.Y.StripLines.PE.Stroke = Color.DarkGray;
                UltraChart.Axis.Y.StripLines.Interval = 2;
                UltraChart.Axis.Y.StripLines.Visible = true;

                UltraChart.Legend.Visible = true;
                UltraChart.Legend.SpanPercentage = 17;
                UltraChart.Legend.Location = LegendLocation.Right;
                UltraChart.Legend.Margins.Bottom = 5 * Convert.ToInt32(UltraChart.Height.Value) / 6;
                UltraChart.Legend.Margins.Right = 5;

                dtChart = ReverseRowsDataTable(dtChart);
                NumericSeries series1 = CRHelper.GetNumericSeries(3, dtChart);
                series1.Label = "Прежний предельный дефицит";
                UltraChart.Series.Add(series1);

                NumericSeries series2 = CRHelper.GetNumericSeries(2, dtChart);
                series2.Label = "Предельный дефицит";
                UltraChart.Series.Add(series2);

                NumericSeries series3 = CRHelper.GetNumericSeries(1, dtChart);
                series3.Label = deficitName;
                UltraChart.Series.Add(series3);

            }
            else
            {
                NumericSeries series1 = CRHelper.GetNumericSeries(1, dtChart);
                series1.Label = deficitName;
                UltraChart.Series.Add(series1);

                NumericSeries series2 = CRHelper.GetNumericSeries(2, dtChart);
                series2.Label = "Предельный дефицит";
                UltraChart.Series.Add(series2);

                NumericSeries series3 = CRHelper.GetNumericSeries(3, dtChart);
                series3.Label = "Прежний предельный дефицит";
                UltraChart.Series.Add(series3);
            }
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.Series != null)
                        {
                            box.Series.Label = RegionsNamingHelper.FullName(box.Series.Label);
                        }

                        switch (box.DataPoint.Label)
                        {
                            case "Предельный дефицит":
                                {
                                    box.PE.ElementType = PaintElementType.CustomBrush;
                                    box.PE.Fill = Color.Red;
                                    //box.PE.FillStopColor = Color.Transparent;
                                    //box.PE.Hatch = FillHatchStyle.Wave;
                                    box.PE.FillOpacity = 100;
                                    //box.lineStyle.DrawStyle = LineDrawStyle.Dash;
                                    break;
                                }
                            case "Прежний предельный дефицит":
                                {
                                    box.PE.ElementType = PaintElementType.CustomBrush;
                                    box.PE.Fill = Color.Blue;
                                    // box.PE.FillStopColor = Color.Transparent;
                                    //box.PE.Hatch = FillHatchStyle.Weave;
                                    box.PE.FillOpacity = 100;
                                    //box.lineStyle.DrawStyle = LineDrawStyle.Dash;
                                    break;
                                }
                            default:
                                {
                                    if (box.DataPoint.Label == deficitName)
                                    {
                                        int rowIndex = box.Row;
                                        if (rowIndex < dtChart.Rows.Count && dtChart.Rows[rowIndex][4] != DBNull.Value)
                                        {
                                            if (ComboYear.SelectedValue == endYear.ToString() || ComboYear.SelectedValue == (endYear - 1).ToString())
                                            {
                                                if (dtChart.Rows[rowIndex][4].ToString() == "Превышение")
                                                {
                                                    box.DataPoint.Label = string.Format("{0} (нарушено)", deficitName);
                                                    box.PE.Fill = Color.Red;
                                                    box.PE.FillStopColor = Color.Maroon;
                                                }
                                                else
                                                {
                                                    box.DataPoint.Label = string.Format("{0} (нет нарушения)", deficitName);
                                                    box.PE.Fill = Color.Green;
                                                    box.PE.FillStopColor = Color.ForestGreen;
                                                }
                                            }
                                            else
                                            {
                                                box.DataPoint.Label = string.Format("{0}", deficitName);
                                                box.PE.Fill = Color.LightSkyBlue;
                                                box.PE.FillStopColor = Color.Blue;
                                            }
                                        }
                                    }
                                    break;
                                }
                        }
                    }
                    else if (box.Path == "Legend" && i != 0)
                    {
                        Primitive lastPrimitive = e.SceneGraph[i - 1];
                        if (lastPrimitive is Text)
                        {
                            Text text = (Text)lastPrimitive;
                            switch (text.GetTextString())
                            {
                                case "Предельный дефицит":
                                    {
                                        box.PE.ElementType = PaintElementType.CustomBrush;
                                        box.PE.Fill = Color.Red;
                                        //box.PE.FillStopColor = Color.Transparent;
                                        //box.PE.Hatch = FillHatchStyle.Wave;
                                        box.PE.FillOpacity = 100;
                                        break;
                                    }
                                case "Прежний предельный дефицит":
                                    {
                                        box.PE.ElementType = PaintElementType.CustomBrush;
                                        box.PE.Fill = Color.Blue;
                                        //box.PE.FillStopColor = Color.Transparent;
                                        //box.PE.Hatch = FillHatchStyle.Weave;
                                        box.PE.FillOpacity = 100;
                                        break;
                                    }
                                default:
                                    {
                                        if (text.GetTextString() == deficitName)
                                        {
                                            if (ComboYear.SelectedValue == endYear.ToString() || ComboYear.SelectedValue == (endYear - 1).ToString())
                                            {
                                                box.PE.ElementType = PaintElementType.CustomBrush;
                                                LinearGradientBrush brush =
                                                    new LinearGradientBrush(box.rect, Color.Red, Color.Green, 45, false);
                                                box.PE.CustomBrush = brush;
                                            }
                                            else
                                            {
                                                box.PE.ElementType = PaintElementType.CustomBrush;
                                                LinearGradientBrush brush =
                                                    new LinearGradientBrush(box.rect, Color.LightSkyBlue, Color.Blue, 45, false);
                                                box.PE.CustomBrush = brush;
                                            }
                                            break;
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Экспорт

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            if (ComboFO.SelectedValue == "Все федеральные округа")
            {
                section2.PageSize = new PageSize(section2.PageSize.Width, section2.PageSize.Height + 250);
                section2.PageMargins.Top = 35;
                section2.PageMargins.Left = 30;


                IText title = section2.AddText();
                Font font = new Font("Verdana", 14);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.Style.Font.Bold = true;
                title.AddContent(Label1.Text);

                title = section2.AddText();
                font = new Font("Verdana", 12);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent(Label2.Text);

                MemoryStream imageStream = new MemoryStream();
                UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 1));
                UltraChart.SaveTo(imageStream, ImageFormat.Png);
                Infragistics.Documents.Reports.Graphics.Image image = (new Bitmap(imageStream)).ScaleImageIg(0.8);
                section2.AddImage(image);
                ReportPDFExporter1.HeaderCellHeight = 50;
                ReportPDFExporter1.Export(headerLayout, Label2.Text, section1);
            }
            else
            {
                ReportPDFExporter1.PageTitle = Label1.Text;
                ReportPDFExporter1.HeaderCellHeight = 50;
                ReportPDFExporter1.Export(headerLayout, Label2.Text, section1);
                IText title = section2.AddText();
                Font font = new Font("Verdana", 14);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.Style.Font.Bold = true;
                title.AddContent(Label1.Text);

                title = section2.AddText();
                font = new Font("Verdana", 12);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent(Label2.Text);
                UltraChart.Legend.Margins.Right = 5;
                UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
                ReportPDFExporter1.Export(UltraChart, section2);
            }


        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");

            ReportExcelExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart, sheet2, 3);
        }

        #endregion
    }
}
