using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = System.Drawing.Font;
using System.Collections;

namespace Krista.FM.Server.Dashboards.reports.STAT_0001_0005
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private DataTable dtChart;
        private DataTable dtPolarChart2009;
        private DataTable dtPolarChart2010;
        private DataTable dtPolarChart2011;
        private DataTable dtKoeff;

        #endregion

        #region Параметры запроса

        // Выбранная территория
        private CustomParam selectedSubject;
        // Текущий год
        private CustomParam currentYear;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.30 - 15);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.82 - 120);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);

            CommentGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth * 0.7 - 15);
            CommentGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 120);
            CommentGrid.DisplayLayout.NoDataMessage = "Нет данных";
            CommentGrid.DataBound += new EventHandler(CommentGrid_DataBound);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 110);

            PolarUltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            PolarUltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 110);

            #region Инициализация параметров запроса

            if (selectedSubject == null)
            {
                selectedSubject = UserParams.CustomParam("selected_subject");
            }
            if (currentYear == null)
            {
                currentYear = UserParams.CustomParam("current_year");
            }

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.SplineChart;
            UltraChart.Border.Thickness = 0;

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N0> чел.";

            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart.Axis.X.Extent = 70;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart.Axis.Y.Extent = 45;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Text = "Численность безработных";
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent - 5;
            UltraChart.TitleLeft.Font = new Font("Verdana", 10);

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.3;
            UltraChart.SplineChart.LineAppearances.Add(lineAppearance);

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value) / 2;
            UltraChart.Legend.SpanPercentage = 14;
            UltraChart.Legend.Font = new Font("Verdana", 10);

            UltraChart.Data.ZeroAligned = true;
            UltraChart.SplineChart.NullHandling = NullHandling.DontPlot;

            UltraChart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            #region Настройка Radar диаграммы

            PolarUltraChart.ChartType = ChartType.RadarChart;
            PolarUltraChart.BorderWidth = 0;

            PolarUltraChart.Data.SwapRowsAndColumns = true;

            PolarUltraChart.Data.RowLabelsColumn = 0;
            PolarUltraChart.Data.UseRowLabelsColumn = true;

            PolarUltraChart.RadarChart.ColorFill = true;
            PolarUltraChart.RadarChart.LineThickness = 3;
            PolarUltraChart.RadarChart.LineEndCapStyle = LineCapStyle.Round;
            PolarUltraChart.RadarChart.NullHandling = NullHandling.Zero;

            PolarUltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N0> чел.";

            PolarUltraChart.Axis.Y.LineThickness = 1;
            PolarUltraChart.Axis.Y.LineDrawStyle = LineDrawStyle.Solid;
            PolarUltraChart.Axis.Y.LineColor = Color.DarkGray;
            PolarUltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            PolarUltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;

            PolarUltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;

            PolarUltraChart.Legend.Visible = true;
            PolarUltraChart.Legend.Location = LegendLocation.Top;
            PolarUltraChart.Legend.SpanPercentage = 10;
            PolarUltraChart.Legend.Margins.Right = ((int)PolarUltraChart.Width.Value / 4) * 3;

            PolarUltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
            PolarUltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.75 - 100);

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.3;
            UltraChart.LineChart.LineAppearances.Add(lineAppearance);

            PolarUltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);


            #endregion

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                ComboRegion.Width = 400;
                ComboRegion.MultiSelect = false;
                ComboRegion.ParentSelect = true;
                ComboRegion.FillDictionaryValues((CustomMultiComboDataHelper.FillFOSubjectList(DataDictionariesHelper.FOSubjectList, false)));
                ComboRegion.Title = "Территория";
                ComboRegion.SetСheckedState("Курганская область", true);
            }

            Page.Title = string.Format("Тенденция и прогноз численности безработных ({0})", ComboRegion.SelectedValue);
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = "Анализ тенденции и прогнозирование общей численности зарегистрированных безработных граждан";
            chartElementCaption.Text = string.Format(@"Диаграмма сезонного изменения общей численности зарегистрированных безработных граждан по неделям года в {0}", RegionsNamingHelper.ShortName(ComboRegion.SelectedValue));

            selectedSubject.Value = string.Format("[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[Уральский федеральный округ].[{0}]",
                        ComboRegion.SelectedValue);
            
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
            UltraChart.DataBind();
            PolarUltraChart.DataBind();

            CommentGrid.Bands.Clear();
            CommentGrid.DataBind();
        }

        private int GetKoeffNumber(string subjectName)
        {
            if (dtKoeff == null)
            {
                dtKoeff = GetSubjectKoeffTable();
            }

            if (dtKoeff.Rows.Count != 0)
            {
                for (int i = 0; i < dtKoeff.Rows.Count; i++)
                {
                    DataRow row = dtKoeff.Rows[i];
                    if (row[0] != DBNull.Value && row[0].ToString() == subjectName)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private static DataTable GetSubjectKoeffTable()
        {
            DataTable dt = new DataTable();

            DataColumn column = new DataColumn("Субъект", typeof(string));
            dt.Columns.Add(column);

            for (int i = 0; i < 16; i++)
            {
                string columnName;
                switch(i)
                {
                    case 0:
                        {
                            columnName = "Y-пересечение";
                            break;
                        }
                    case 13:
                        {
                            columnName = "Множественный R";
                            break;
                        }
                    case 14:
                        {
                            columnName = "R-квадрат";
                            break;
                        }
                    case 15:
                        {
                            columnName = "Стандартная ошибка";
                            break;
                        }
                    default:
                        {
                            columnName = "Переменная X" + i;
                            break;
                        }
                }
                column = new DataColumn(columnName, typeof(string));
                dt.Columns.Add(column);
            }

            DataRow row = dt.NewRow();
            object[] array0 = { "Уральский федеральный округ", 4.834817, 0.011686, 0.056984, 0.087004, 0.093705, 0.084681, 0.049598, 0.013778, -0.00585,
                -0.02109, -0.04614, -0.05875, -0.04432,0 ,0 ,0};
            row.ItemArray = array0;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array1 = { "Челябинская область", 4.275802, 0.013926, 0.040292, 0.077282, 0.073687, 0.053920, 0.0079087, -0.010274,
                -0.025252, -0.045110, -0.077741, -0.097024, -0.067357, 0.825015, 0.680650, 0.143380};
            row.ItemArray = array1;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array2 =  { "Курганская область", 4.030716, 0.004315, 0.050866, 0.080746, 0.082402, 0.049926, 0.008242, -0.023601, -0.031912, -0.044864,
                -0.074787, -0.079989, -0.050917, 0.725983, 0.527051, 0.085934};
            row.ItemArray = array2;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array3 =  { "Свердловская область", 4.301695, 0.016367, 0.073546, 0.089745, 0.080245, 0.066612, 0.038165, 0.027910, 0.016237, 0.000114, -0.027503,
                -0.043127, -0.037974, 0.872560, 0.761360, 0.133229};
            row.ItemArray = array3;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array4 =  { "Тюменская область", 4.027350, -0.007477, 0.013083, 0.032511, 0.039706, 0.049971, 0.042602, 0.002576, -0.010114, -0.018256, -0.047603,
                -0.043513, -0.034087, 0.950109, 0.902708, 0.037437};
            row.ItemArray = array4;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array5 =  { "Ханты-Мансийский автономный округ", 4.021664, 0.004053, 0.055707, 0.080262, 0.077477, 0.069644, 0.033193, 0.003313, -0.024575,
                -0.040061, -0.047314, -0.049641, -0.038991, 0.597971, 0.357569, 0.110876};
            row.ItemArray = array5;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array6 =  { "Ямало-Ненецкий автономный округ", 3.676308, 0.001662, 0.045314, 0.075064, 0.069432, 0.048719, 0.009784,
                -0.069183, -0.130112, -0.124571, -0.106291, -0.079426, -0.051240, 0.747083, 0.558133, 0.082268};
            row.ItemArray = array6;
            dt.Rows.Add(row);

            return dt;
        }

        #region Обработчики грида

        private static bool IsYear(string source)
        {
            decimal year;
            if (decimal.TryParse(source, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out year))
            {
                return year > 1900;
            }
            else
            {
                return false;
            }
        }

        private static bool IsMonth(string source)
        {
            return !(CRHelper.MonthNum(source) == 1 && source.ToLower() != "январь");
        }

        private static bool IsDay(string source)
        {
            decimal day;
            if (decimal.TryParse(source, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out day))
            {
                return day > 0 && day < 32;
            }
            else
            {
                return false;
            }
        }

        private static string GetLastMonth(DataTable dt, int monthColumnIndex)
        {
            string month = string.Empty;
            foreach (DataRow row in dt.Rows)
            {
                if (row[monthColumnIndex] != DBNull.Value && row[monthColumnIndex].ToString() != string.Empty)
                {
                    if (IsMonth(row[monthColumnIndex].ToString()))
                    {
                        month = row[monthColumnIndex].ToString();
                    }
                }
            }
            return month;
        }

        private static int GetLastYear(DataTable dt, int yearColumnIndex)
        {
            int year = -1;
            foreach (DataRow row in dt.Rows)
            {
                if (row[yearColumnIndex] != DBNull.Value && row[yearColumnIndex].ToString() != string.Empty)
                {
                    if (IsYear(row[yearColumnIndex].ToString()))
                    {
                        year = Convert.ToInt32(row[yearColumnIndex].ToString());
                    }
                }
            }

            return year;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0005_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Год, месяц", dtGrid);

            int koeffNumber = GetKoeffNumber(ComboRegion.SelectedValue);
            DataRow koeffRow = dtKoeff.Rows[koeffNumber];

            if (dtGrid.Rows.Count > 0 && koeffNumber != -1)
            {
                dtChart = new DataTable();
                DataColumn column = new DataColumn("Период", typeof(string));
                dtChart.Columns.Add(column);
                column = new DataColumn("Фактическая численность безработных", typeof(double));
                dtChart.Columns.Add(column);
                column = new DataColumn("Прогноз", typeof(double));
                dtChart.Columns.Add(column);

                // добавляем последний прогнозный месяц
                int lastYear = GetLastYear(dtGrid, 0);
                string lastMonth = GetLastMonth(dtGrid, 0);
                if (lastMonth != string.Empty)
                {
                    DataRow monthRow = dtGrid.NewRow();
                    if (lastMonth.ToLower() == "декабрь")
                    {
                        DataRow yearRow = dtGrid.NewRow();
                        yearRow[0] = lastYear + 1;
                        dtGrid.Rows.Add(yearRow);
                        monthRow[0] = "Январь";
                    }
                    else
                    {
                        monthRow[0] = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(CRHelper.MonthNum(lastMonth) + 1));
                    }
                    dtGrid.Rows.Add(monthRow);
                }

                try
                {
                    int i = 0;
                    // глобальный счетчик месяцев
                    int monthCount = 0;
                    while (i < dtGrid.Rows.Count)
                    {
                        DataRow row = dtGrid.Rows[i];
                        if (IsYear(row[0].ToString()))
                        {
                            string year = row[0].ToString();

                            // локальный счетчик месяцев
                            int j = 0;
                            while (j < 12)
                            {
                                DataRow monthRow = dtGrid.Rows[i + j];
                                if (IsMonth(monthRow[0].ToString()))
                                {
                                    string month = monthRow[0].ToString();

                                    double b0 = Convert.ToDouble(koeffRow[1]);
                                    double xi = Convert.ToDouble(koeffRow[2])*(monthCount + j);
                                    double koeff = (j == 11) ? 0 : Convert.ToDouble(koeffRow[j + 3]);
                                    double logForecast = b0 + xi + koeff;
                                    double forecastValue = Math.Pow(10, logForecast);

                                    monthRow[2] = forecastValue;

                                    // заполняем таблицу для диаграммы
                                    DataRow chartRow = dtChart.NewRow();
                                    chartRow[0] = (j == 0) ? string.Format("{0}-{1}", year, month) : month;

                                    if (monthRow[1] != DBNull.Value && monthRow[1].ToString() != string.Empty)
                                    {
                                        chartRow[1] = Convert.ToDouble(monthRow[1].ToString());
                                    }
                                   
                                    chartRow[2] = forecastValue;
                                    dtChart.Rows.Add(chartRow);
                                    j++;
                                }
                                else
                                {
                                    if (i < dtGrid.Rows.Count)
                                    {
                                        i++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            monthCount += j;
                            i += j - 1;
                        }
                        i++;
                    }
                }
                catch(Exception ex)
                {

                }

                ((UltraWebGrid)sender).DataSource = dtGrid;
                //UltraChart.DataSource = dtChart;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
//            UltraWebGrid1.Width = Unit.Empty;
//            UltraWebGrid1.Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.RowAlternateStyleDefault.BackColor = Color.White;
            e.Layout.CellClickActionDefault = CellClickAction.NotSet;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                int widthColumn = 95;

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(80);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Фактическая численность безработных", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Прогноз", "");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            bool isYear = false;
            bool isMonth = false;
            if (e.Row.Cells[0] != null && e.Row.Cells[0].Value.ToString() != string.Empty)
            {
                isYear = IsYear(e.Row.Cells[0].Value.ToString());
                isMonth = IsMonth(e.Row.Cells[0].Value.ToString());
            }

            if (!isYear && !isMonth)
            {
                e.Row.Style.Font.Italic = true;
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell c = e.Row.Cells[i];
                if (c.Value != null && c.Value.ToString() != string.Empty)
                {
                    if (isYear)
                    {
                        c.Style.Font.Bold = true;
                        c.ColSpan = e.Row.Band.Columns.Count;
                        if (c.Row.PrevRow != null)
                        {
                            c.Row.PrevRow.Style.BorderDetails.WidthBottom = 2;
                        }
                    }
                    else if (!isMonth)
                    {
                        c.Style.Padding.Left = 20;
                    }

                    decimal value;
                    if (decimal.TryParse(c.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            c.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        #endregion

        #region Поясняющий грид

        private string GetKoeffValueStr(int koeffNum)
        {
            int koeffNumber = GetKoeffNumber(ComboRegion.SelectedValue);
            DataRow koeffRow = dtKoeff.Rows[koeffNumber];

            double value = 0;
            if (koeffRow[koeffNum] != DBNull.Value && koeffRow[koeffNum].ToString() != string.Empty)
            {
                value = Convert.ToDouble(koeffRow[koeffNum]);
                return value < 0 ? string.Format("({0:N4})", value)  : string.Format("{0:N4}", value);
            }

            return string.Empty;
        }

        private DataTable GetCommentGridTable()
        {
            DataTable dt = new DataTable();
            
            DataColumn column = new DataColumn("Коэффициент регрессионной модели", typeof(string));
            dt.Columns.Add(column);
            column = new DataColumn("Значение bi=logBi", typeof(double));
            dt.Columns.Add(column);
            column = new DataColumn("Значение Bi=10^bi", typeof(double));
            dt.Columns.Add(column);
            column = new DataColumn("Интерпретация значения коэффициента", typeof(string));
            dt.Columns.Add(column);

            int koeffNumber = GetKoeffNumber(ComboRegion.SelectedValue);
            DataRow koeffRow = dtKoeff.Rows[koeffNumber];

            CommentText.Text = string.Format(@"Уравнение регрессионной модели экспоненциального роста с учетом сезонного компонента:<br/>
<i>&nbsp;&nbsp;&nbsp;Log Y<sub>i</sub> = b0 + b1*X<sub>i</sub> + b2*M1 + b3*M2 + b4*M3 + b5*M4 + b6*M5 + b7*M6 + b8*M7 + b9*M8 + b10*M9 + b11*M10 + b12*M11</i><br/>" +
"<i>&nbsp;&nbsp;&nbsp;Log Y<sub>i</sub> = <b>{0}</b> + <b>{1}</b>*Xi + <b>{2}</b>*M1 + <b>{3}</b>*M2 + <b>{4}</b>*M3 + <b>{5}</b>*M4 + <b>{6}</b>*M5 + <b>{7}</b>*M6 + <b>{8}</b>*M7 + <b>{9}</b>*M8 + <b>{10}</b>*M9 + <b>{11}</b>*M10 + <b>{12}</b>*M11</i>",
GetKoeffValueStr(1), GetKoeffValueStr(2), GetKoeffValueStr(3), GetKoeffValueStr(4), GetKoeffValueStr(5), GetKoeffValueStr(6), GetKoeffValueStr(7), GetKoeffValueStr(8), GetKoeffValueStr(9), GetKoeffValueStr(10), GetKoeffValueStr(11),
GetKoeffValueStr(12), GetKoeffValueStr(13));

            CommentText2.Text = string.Format(@"Характеристика регрессионной модели:&nbsp;&nbsp;&nbsp;
<table>
<tr><td style='padding-left:20px;'>{0}</td><td align='right' style='width:120px;'><b>{3:N6}</b></td></tr>
<tr><td style='padding-left:20px;'>{1}</td><td align='right' style='width:120px;'><b>{4:N6}</b></td></tr>
<tr><td style='padding-left:20px;'>{2}</td><td align='right' style='width:120px;'><b>{5:N6}</b></td></tr>
</table>", dtKoeff.Columns[14].ColumnName, dtKoeff.Columns[15].ColumnName, dtKoeff.Columns[16].ColumnName,
            Convert.ToDouble(koeffRow[14]), Convert.ToDouble(koeffRow[15]), Convert.ToDouble(koeffRow[16]));

            CommentText3.Text = string.Format(@"Значение R-квадрат = <b>{0:N6}</b>, таким образом, <b>{0:P2}</b> вариации численности 
безработных может быть определена с помощью временного периода, остальная часть (<b>{1:P2}</b>) изменчивости численности безработных 
объясняется другими факторами, не учтенными в регрессионной модели.", Convert.ToDouble(koeffRow[15]), 1 - Convert.ToDouble(koeffRow[15]));

            for (int i = 0; i <= 12; i++)
            {
                DataRow row = dt.NewRow();

                double bi = 0;
                if (koeffRow[i + 1] != DBNull.Value && koeffRow[i + 1].ToString() != string.Empty)
                {
                    bi = Convert.ToDouble(koeffRow[i + 1]);
                }

                string rowName = string.Empty;
                string commentText = string.Empty;
                switch (i)
                {
                    case 0:
                        {
                            rowName = string.Format("b{0}: сдвиг", i);
                            double value = Math.Pow(10, bi);
                            commentText = string.Format(@"Значение некорректированного тренда месячного числа безработных в первом временном периоде 
(январе 2007 года) составила <b>{0:N0}</b> чел.", value);
                            break;
                        }
                    case 1:
                        {
                            rowName = string.Format("b{0}: наклон", i);
                            double value = Math.Pow(10, bi) - 1;
                            commentText = string.Format("Ежемесячный темп прироста числа безработных <b>{0:P2}</b>", value);
                            break;
                        }
                    default:
                        {
                            rowName = string.Format("b{0}: {1}", i, CRHelper.RusMonth(i - 1).ToLower());
                            double value = Math.Pow(10, bi) - 1;
                            commentText = string.Format(@"Сезонный коэффициент для <b>{2}</b> по отношению к декабрю. Численность безработных в <b>{3}</b> на 
<b>{0:p2}</b>&nbsp;<b>{1}</b> численности безработных в декабре того же года.", Math.Abs(value), value < 0 ? "меньше" : "больше", 
                                                                         CRHelper.RusMonthAblative(i - 1),
                                                                         CRHelper.RusMonthPrepositional(i - 1));
                            break;
                        }
                }

                row[0] = rowName;
                row[1] = bi;
                row[2] = Math.Pow(10, bi);
                row[3] = commentText;

                dt.Rows.Add(row);
            }

            return dt;
        }

        protected void CommentGrid_DataBind(object sender, EventArgs e)
        {
            CommentGrid.DataSource = GetCommentGridTable();
        }

        protected void CommentGrid_DataBound(object sender, EventArgs e)
        {
            CommentGrid.Width = Unit.Empty;
            CommentGrid.Height = Unit.Empty;
        }

        protected void CommentGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.RowAlternateStyleDefault.BackColor = Color.White;
            e.Layout.AllowUpdateDefault = AllowUpdate.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 1)
            {
                int widthColumn = 100;

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N6");
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(100);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(480);
            e.Layout.Bands[0].Columns[3].CellStyle.Wrap = true;

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "Коэффициент регрессионной модели", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Значение bi=logBi", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Значение Bi=10*bi", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Интерпретация значения коэффициента", "");
        }

        protected void CommentGrid_InitializeRow(object sender, RowEventArgs e)
        {

        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            if (dtChart != null)
            {
                UltraChart.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart.Series.Add(series);
                }
            }
        } 

        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            string currentYear = string.Empty;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline) primitive;
                    foreach (DataPoint point in polyline.points)
                    {
                        if (point.DataPoint != null)
                        {
                            string serieName = (point.Row == 0) ? "Фактическая численность безработных" : "Прогноз численности";
                            string monthStr = point.DataPoint.Label;
                            string[] strs = monthStr.Split('-');
                            if (strs.Length > 1)
                            {
                                currentYear = strs[0];
                                monthStr = strs[1];
                            }

                            if (currentYear != string.Empty)
                            {
                                monthStr = string.Format("{0} {1} года", monthStr.ToLower(), currentYear);
                            }

                            point.DataPoint.Label = string.Format("{0}\n{1}", serieName, monthStr);
                        }
                    }
                }
//
//                if (primitive is Polyline && primitive.Path != null && primitive.Path.ToString().Contains("Legend"))
//                {
//                    Polyline line = (Polyline)primitive;
//                    Box box = new Box(new Rectangle(line.points[0].point.X, line.points[0].point.Y - 5, 10, 10));
//                    box.PE = line.PE;
//                }
            }
        }

        #endregion

        #region Обработчики полярной диаграммы

        protected void PolarUltraChart_DataBinding(object sender, EventArgs e)
        {
            currentYear.Value = "2009";
            string query = DataProvider.GetQueryText("STAT_0001_0005_polarChart");
            dtPolarChart2009 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtPolarChart2009);

            currentYear.Value = "2010";
            query = DataProvider.GetQueryText("STAT_0001_0005_polarChart");
            dtPolarChart2010 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtPolarChart2010);
            
            currentYear.Value = "2011";
            query = DataProvider.GetQueryText("STAT_0001_0005_polarChart");
            dtPolarChart2011 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtPolarChart2011);

            currentYear.Value = "2011";
            query = DataProvider.GetQueryText("STAT_0001_0005_polarChart");
            dtPolarChart2011 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtPolarChart2011);

            if (dtPolarChart2009.Rows.Count > 0 && dtPolarChart2010.Rows.Count > 0)
            {
                DataTable newDt = new DataTable();
                DataColumn column = new DataColumn("Номер", typeof(int));
                newDt.Columns.Add(column);
                column = new DataColumn("2009 год", typeof(double));
                newDt.Columns.Add(column);
                column = new DataColumn("2010 год", typeof(double));
                newDt.Columns.Add(column);
                column = new DataColumn("2011 год", typeof(double));
                newDt.Columns.Add(column);

//                DataRow newRow = newDt.NewRow();
//                newRow[0] = 1;
//                newRow[1] = 0;
//                newRow[2] = 0;
//                newDt.Rows.Add(newRow);

                for (int i = 0; i < 51; i++)
                {
                    DataRow newRow = newDt.NewRow();
                    newRow[0] = i + 2;
                    if (i < dtPolarChart2009.Rows.Count && dtPolarChart2009.Rows[i][1] != DBNull.Value && dtPolarChart2009.Rows[i][1].ToString() != string.Empty)
                    {
                        newRow[1] = Convert.ToDouble(dtPolarChart2009.Rows[i][1]);
                    }
                    else
                    {
                        newRow[1] = 0;
                    }

                    if (i < dtPolarChart2010.Rows.Count && dtPolarChart2010.Rows[i][1] != DBNull.Value && dtPolarChart2010.Rows[i][1].ToString() != string.Empty)
                    {
                        newRow[2] = Convert.ToDouble(dtPolarChart2010.Rows[i][1]);
                    }
                    else
                    {
                        newRow[2] = 0;
                    }
                    if (i < dtPolarChart2011.Rows.Count && dtPolarChart2011.Rows[i][1] != DBNull.Value && dtPolarChart2011.Rows[i][1].ToString() != string.Empty)
                    {
                        newRow[3] = Convert.ToDouble(dtPolarChart2011.Rows[i][1]);
                    }
                    else
                    {
                        newRow[3] = 0;
                    }

                    if (i < dtPolarChart2011.Rows.Count && dtPolarChart2011.Rows[i][1] != DBNull.Value && dtPolarChart2011.Rows[i][1].ToString() != string.Empty)
                    {
                        newRow[3] = Convert.ToDouble(dtPolarChart2011.Rows[i][1]);
                    }
                    else
                    {
                        newRow[3] = 0;
                    }

                    newDt.Rows.Add(newRow);
                }


//                UltraChart.Series.Clear();
//                for (int i = 1; i < dtChart.Columns.Count; i++)
//                {
//                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
//                    series.Label = dtChart.Columns[i].ColumnName;
//                    UltraChart.Series.Add(series);
//                }

                PolarUltraChart.DataSource = newDt;
            }
        }

        void PolarUltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
//            string currentYear = string.Empty;
//
//            for (int i = 0; i < e.SceneGraph.Count; i++)
//            {
//                Primitive primitive = e.SceneGraph[i];
//
//                if (primitive is Polyline)
//                {
//                    Polyline polyline = (Polyline)primitive;
//                    foreach (DataPoint point in polyline.points)
//                    {
//                        if (point.DataPoint != null)
//                        {
//                            string serieName = (point.Row == 0) ? "Фактическая численность безработных" : "Прогноз численности";
//                            string monthStr = point.DataPoint.Label;
//                            string[] strs = monthStr.Split('-');
//                            if (strs.Length > 1)
//                            {
//                                currentYear = strs[0];
//                                monthStr = strs[1];
//                            }
//
//                            if (currentYear != string.Empty)
//                            {
//                                monthStr = string.Format("{0} {1} года", monthStr.ToLower(), currentYear);
//                            }
//
//                            point.DataPoint.Label = string.Format("{0}\n{1}", serieName, monthStr);
//                        }
//                    }
//                }
//
//                //                if (primitive is Line && primitive.Path != null && primitive.Path.ToString().Contains("Legend"))
//                //                {
//                //                    Line line = (Line)primitive;
//                //                }
//            }
        }

        #endregion

        #region Экспорт в Excel

        private static void SetExportGridParams(UltraWebGrid grid)
        {
            foreach (UltraGridRow row in grid.Rows)
            {
                if (row.Cells[3].Value != null)
                {
                    row.Cells[3].Value = CommentTextExportsReplaces(row.Cells[3].Value.ToString());
                }
            }
        }

        private static string CommentTextExportsReplaces(string source)
        {
            string commentText = source;

            commentText = commentText.Replace("<\n>", "");
            commentText = commentText.Replace("<\r>", "");
            commentText = commentText.Replace(@"
", " ");
            commentText = commentText.Replace("&nbsp;", " ");
            commentText = commentText.Replace("<br/>", "\n");
            commentText = commentText.Replace("<b>", "");
            commentText = commentText.Replace("</b>", "");
            commentText = commentText.Replace("<i>", "");
            commentText = commentText.Replace("</i>", "");
            commentText = commentText.Replace("<sub>", "");
            commentText = commentText.Replace("</sub>", "");
            commentText = commentText.Replace("<table>", "\n");
            commentText = commentText.Replace("</table>", "");
            commentText = commentText.Replace("<tr>", "");
            commentText = commentText.Replace("</tr>", "\n");
            commentText = commentText.Replace("<td style='padding-left:20px;'>", "   ");
            commentText = commentText.Replace("<td align='right' style='width:120px;'>", "  ");
            commentText = commentText.Replace("</td>", "");

            return commentText;
        }

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.Workbook.Worksheets["Тенденция и прогноз"].Rows[0].Cells[0].Value = PageTitle.Text;
            e.Workbook.Worksheets["Тенденция и прогноз"].Rows[1].Cells[0].Value = PageSubTitle.Text;
            e.Workbook.Worksheets["Уравнение регрессионной модели"].Rows[0].Cells[0].Value =
                CommentTextExportsReplaces(CommentText.Text) + "\n" + CommentTextExportsReplaces(CommentText2.Text) +
                CommentTextExportsReplaces(CommentText3.Text);
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            if (e.CurrentWorksheet.Name == "Тенденция и прогноз")
            {
                e.CurrentWorksheet.Columns[0].Width = 100*37;
                e.CurrentWorksheet.Columns[1].Width = 120*37;
                e.CurrentWorksheet.Columns[2].Width = 100*37;

                e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0";
                e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0";
            }
            else if (e.CurrentWorksheet.Name == "Уравнение регрессионной модели")
            {
                e.CurrentWorksheet.Columns[0].Width = 100 * 37;
                e.CurrentWorksheet.Columns[1].Width = 100 * 37;
                e.CurrentWorksheet.Columns[2].Width = 100 * 37;
                e.CurrentWorksheet.Columns[3].Width = 400 * 37;

                e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,######0.000000";
                e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,######0.000000";
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Тенденция и прогноз");
            Worksheet sheet2 = workbook.Worksheets.Add("Уравнение регрессионной модели");

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            SetExportGridParams(CommentGrid);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1, sheet1);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(CommentGrid, sheet2);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            //e.HeaderText = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF

        private bool firstGridExport = false;

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            Report report = new Report();
            ReportSection section1 = new ReportSection(report);

            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1, section1);
            SetExportGridParams(CommentGrid);
            UltraGridExporter1.PdfExporter.Export(CommentGrid, section1);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            if (!firstGridExport)
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = 100;
                }
            }
            else
            {
                e.Section.AddPageBreak();

                IText textTitle = e.Section.AddText();
                Font textGont = new Font("Verdana", 12);
                textTitle.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(textGont);
                textTitle.AddContent("\n" + CommentTextExportsReplaces(CommentText.Text));
                e.Layout.Bands[0].Columns[3].Width = 400;
            }

            if (firstGridExport)
            {
                return;
            }

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            UltraChart.Legend.Margins.Right = 0;
            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);

           ((ReportSection) e.Section).cellIndex = 1;
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            if (firstGridExport)
            {
                ((ReportSection)e.Section).cellIndex = 2;

                IText textTitle = e.Section.AddText();
                Font textGont = new Font("Verdana", 12);
                textTitle.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(textGont);
                textTitle.AddContent("\n" + CommentTextExportsReplaces(CommentText2.Text) + CommentTextExportsReplaces(CommentText3.Text));

                ((ReportSection)e.Section).cellIndex = 3;

                e.Section.AddPageBreak();
                IText title = e.Section.AddText();
                Font font = new Font("Verdana", 12);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent("\n" + chartElementCaption.Text);

                UltraChart.Width = Unit.Pixel((int) (CustomReportConst.minScreenWidth*0.82));
                Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(PolarUltraChart);
                e.Section.AddImage(img);
            }

            firstGridExport = true;
        }

        #endregion
    }

    public class ReportSection : ISection
    {

       #region ISection Members

        private readonly ISection section;
        private ITableCell chart1Cell;
        private ITableCell chart2Cell;
        private ITableCell grid1Cell;
        private ITableCell grid2Cell;
        public int cellIndex = 0;

        public ReportSection(Report report)
        {
            section = report.AddSection();
            ITable table = section.AddTable();
            ITableRow chart1Row = table.AddRow();
            chart1Cell = chart1Row.AddCell();
            ITableRow grid1Row = table.AddRow();
            grid1Cell = grid1Row.AddCell();
            ITableRow grid2Row = table.AddRow();
            grid2Cell = grid2Row.AddCell();
            ITableRow chart2Row = table.AddRow();
            chart2Cell = chart2Row.AddCell();
        }

        public Infragistics.Documents.Reports.Report.Band.IBand AddBand()
        {
            throw new Exception("The method or operation is not implemented.");
            
        }

        public Infragistics.Documents.Reports.Report.ICanvas AddCanvas()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IChain AddChain()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.ICondition AddCondition(Infragistics.Documents.Reports.Report.IContainer container, bool fit)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IContainer AddContainer(string name)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IDecoration AddDecoration()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.Flow.IFlow AddFlow()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ISectionFooter AddFooter()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IGap AddGap()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.Grid.IGrid AddGrid()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IGroup AddGroup()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ISectionHeader AddHeader()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IImage AddImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            switch (cellIndex)
            {
                case 0:
                    {
                        return chart1Cell.AddImage(image);
                    }
                case 1:
                    {
                        return grid1Cell.AddImage(image);
                    }
                case 2:
                    {
                        return grid2Cell.AddImage(image);
                    }
                case 3:
                    {
                        return chart2Cell.AddImage(image);
                    }
            }

            return grid1Cell.AddImage(image);
        }

        public Infragistics.Documents.Reports.Report.Index.IIndex AddIndex()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.List.IList AddList()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IMetafile AddMetafile(Infragistics.Documents.Reports.Graphics.Metafile metafile)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ISectionPage AddPage(float width, float height)
        {
            return this.section.AddPage(width, height);
        }

        public ISectionPage AddPage(Infragistics.Documents.Reports.Report.PageSize size)
        {
            return this.section.AddPage(size);
        }

        public ISectionPage AddPage()
        {
            return this.section.AddPage();
        }

        public void AddPageBreak()
        {
            switch (cellIndex)
            {
                case 0:
                    {
                        chart1Cell.AddPageBreak();
                        break;
                    }
                case 1:
                    {
                        grid1Cell.AddPageBreak();
                        break;
                    }
                case 2:
                    {
                        grid2Cell.AddPageBreak();
                        break;
                    }
                case 3:
                    {
                       chart2Cell.AddPageBreak();
                       break;
                    }
                default:
                    {
                        grid1Cell.AddPageBreak();
                        break;
                    }
            }
        }

        public Infragistics.Documents.Reports.Report.IQuickImage AddQuickImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.QuickList.IQuickList AddQuickList()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.QuickTable.IQuickTable AddQuickTable()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.QuickText.IQuickText AddQuickText(string text)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IRotator AddRotator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IRule AddRule()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.Segment.ISegment AddSegment()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.ISite AddSite()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IStationery AddStationery()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.IStretcher AddStretcher()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.TOC.ITOC AddTOC()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public Infragistics.Documents.Reports.Report.Table.ITable AddTable()
        {
            switch (cellIndex)
            {
                case 0:
                    {
                        return chart1Cell.AddTable();
                    }
                case 1:
                    {
                        return grid1Cell.AddTable();
                    }
                case 2:
                    {
                        return grid2Cell.AddTable();
                    }
                case 3:
                    {
                        return chart2Cell.AddTable();
                    }
            }

            return grid1Cell.AddTable();
        }

        public IText AddText()
        {
            switch (cellIndex)
            {
                case 0:
                    {
                        return chart1Cell.AddText();
                    }
                case 1:
                    {
                        return grid1Cell.AddText();
                    }
                case 2:
                    {
                        return grid2Cell.AddText();
                    }
                case 3:
                    {
                        return chart2Cell.AddText();
                    }
            }

            return grid1Cell.AddText();
        }

        public Infragistics.Documents.Reports.Report.Tree.ITree AddTree()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Flip
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public SectionLineNumbering LineNumbering
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Infragistics.Documents.Reports.Report.ContentAlignment PageAlignment
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Infragistics.Documents.Reports.Report.Background PageBackground
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Infragistics.Documents.Reports.Report.Borders PageBorders
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Infragistics.Documents.Reports.Report.Margins PageMargins
        {
            get
            {
                return this.section.PageMargins;
            }
            set
            {
                this.section.PageMargins = value;
            }
        }

        public Infragistics.Documents.Reports.Report.Section.PageNumbering PageNumbering
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Infragistics.Documents.Reports.Report.PageOrientation PageOrientation
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Infragistics.Documents.Reports.Report.Paddings PagePaddings
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Infragistics.Documents.Reports.Report.PageSize PageSize
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                this.section.PageSize = value;
            }
        }

        public Infragistics.Documents.Reports.Report.Report Parent
        {
            get { return section.Parent; }
        }

        public IEnumerable Content
        {
            get { throw new NotImplementedException(); }
        }
				
        #endregion
    }
}
