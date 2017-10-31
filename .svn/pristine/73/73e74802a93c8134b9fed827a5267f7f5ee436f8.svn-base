using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;
using System.Drawing;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.reports.STAT_0002_0008
{

    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable chartDt = new DataTable();
        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private static MemberAttributesDigest periodDigest;
        private int firstYear = 2009;
        private CustomParam currentPeriod;
        private CustomParam lastPeriod;
        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // множество для среза данных
        private CustomParam sliceSet;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.AutoHeightRowLimit = 15;
            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.85 - 235);
            GridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 15);
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            #region Настройка диаграммы динамики

            ChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            ChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.8);

            ChartBrick.YAxisLabelFormat = "N2";
            ChartBrick.DataFormatString = "N2";
            ChartBrick.DataItemCaption = "млн. руб.";
            ChartBrick.Legend.Visible = true;
            ChartBrick.Legend.Location = LegendLocation.Bottom;
            ChartBrick.Legend.SpanPercentage = 10;
            ChartBrick.ColorModel = ChartColorModel.DefaultFixedColors;
            ChartBrick.XAxisExtent = 265;
            ChartBrick.YAxisExtent = 90;
            ChartBrick.ZeroAligned = true;
            ChartBrick.SeriesLabelWrap = true;
            ChartBrick.TooltipFormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N2> млн.руб.";
            ChartBrick.SwapRowAndColumns = true;
            #endregion

            #region Инициализация параметров запроса

            currentPeriod = UserParams.CustomParam("current_period");
            lastPeriod = UserParams.CustomParam("last_period");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
        
        DateTime lastDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                ComboYear.PanelHeaderTitle = "Выберите период";
                ComboYear.Title = "Выберите период";
                ComboYear.Width = 290;
                ComboYear.ParentSelect = false;
                ComboYear.ShowSelectedValue = true;
                ComboYear.MultiSelect = false;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0002_0008_Date");
            }

            if (!Page.IsPostBack)
            {
                FillComboDate(ComboYear, "STAT_0002_0008_list_of_dates", 0);
            }
            ChartCaption.Text = String.Format("Распределение предприятий банковского сектора по общему объему выданных кредитов, млн.рублей");
            string periodUniqueName = string.Empty;
            switch (ComboYear.SelectedNode.Level)
            {
                case 0:
                    {
                        periodUniqueName = StringToMDXDate(ComboYear.GetLastChild(ComboYear.SelectedNode).FirstNode.Text);
                        break;
                    }
                case 1:
                    {
                        periodUniqueName = StringToMDXDate(ComboYear.SelectedNode.FirstNode.Text);
                        break;
                    }
                case 2:
                    {
                        periodUniqueName = StringToMDXDate(ComboYear.SelectedNode.Text);
                        break;
                    }
            }
            //string periodUniqueName = periodDigest.GetMemberUniqueName(ComboYear.SelectedValue);
            currentPeriod.Value = periodUniqueName;
            currentDate = CRHelper.PeriodDayFoDate(periodUniqueName);
            lastDate = currentDate.AddMonths(-1);
            string lastPeriodUniqueName = periodDigest.GetMemberUniqueName(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(lastDate.Month)));
            if (lastPeriodUniqueName != string.Empty)
                lastPeriod.Value = string.Format(", {0}.[{1}]", lastPeriodUniqueName, currentDate.Day);
            DateTime nextDayDate = currentDate;
            Page.Title = "Анализ объемов выданных кредитов в разрезе банков ХМАО - Югры";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Ежемесячный мониторинг предприятий банковского сектора по основным показателям, характеризующим объемы выданных кредитов, Ханты-Мансийский автономный округ – Югра, по состоянию на {0:dd.MM.yyyy} года", nextDayDate);
            headerLayout = new GridHeaderLayout(GridBrick.Grid);
            ChartBrick.TooltipFormatString = string.Format("<ITEM_LABEL>\nпо состоянию на {0:dd.MM.yyyy} года\n<DATA_VALUE:N2> млн.руб.", nextDayDate);
            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);
            GridDataBind();
            ChartDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("STAT_0002_0008_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование", gridDt);

            if (gridDt.Rows.Count > 0)
            {

                if (gridDt.Columns.Count > 1)
                {
                    gridDt.Columns.RemoveAt(0);
                }

                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].MergeCells = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(80);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;
            }

            headerLayout.AddCell("Наименование");

            // Заголовки
            GridHeaderCell header;
            GridHeaderCell underheader;
            headerLayout.AddCell("Объём, млн. рублей, ВСЕГО");
            headerLayout.AddCell("Доля отказов по кредитным продуктам, от общего числа заявок на кредиты, %");
            header = headerLayout.AddCell("Потребительские кредиты");
            header.AddCell("Объём выданных кредитов, млн. руб.");
            header.AddCell("Количество выданных кредитов, единиц");
            underheader = header.AddCell("Условия кредитования");
            underheader.AddCell("% ставка");
            underheader.AddCell("сроки (месяцев)");

            header = headerLayout.AddCell("Ипотечные кредиты");

            header.AddCell("Объём выданных кредитов, млн. руб.");
            header.AddCell("Количество выданных кредитов, единиц");
            underheader = header.AddCell("Условия кредитования");
            underheader.AddCell("% ставка");
            underheader.AddCell("сроки (месяцев)");
            headerLayout.ApplyHeaderInfo();

        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellCount = e.Row.Cells.Count;

            int type = 0;
            if (e.Row.Cells[cellCount - 1].Value != null)
            {
                type = Convert.ToInt32(e.Row.Cells[cellCount - 1].Value.ToString());
            }

            for (int i = 1; i < cellCount - 1; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                cell.Style.Padding.Right = 3;

                switch (type)
                {
                    case 0:
                        {
                            if (cell.Value != null)
                            {
                                if (ComboYear.SelectedValue.Contains("евр"))
                                {
                                    cell.Value = string.Empty;
                                    cell.Style.BorderDetails.WidthTop = 0;
                                    cell.Style.BorderDetails.WidthBottom = 0;
                                    break;
                                }
                                cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N2");
                                cell.Title = string.Format("Абсолютное отклонение к {0:dd.MM.yyyy}г.", lastDate);
                            }
                            cell.Style.BorderDetails.WidthTop = 0;
                            cell.Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                    case 1:
                        {
                            if (cell.Value != null)
                            {
                                CRHelper.SaveToErrorLog("Индекс:" + ComboYear.SelectedIndex.ToString() + "значение:" + ComboYear.SelectedValue.ToString());
                                if (ComboYear.SelectedValue.Contains("евр"))
                                {
                                    cell.Value = string.Empty;
                                    break;
                                }
                                if (gridDt.Columns[i].Caption.Contains("Доля отказов"))
                                {
                                    double growRate = Convert.ToDouble(cell.Value.ToString());
                                    cell.Value = growRate.ToString("P2");

                                    if (growRate > 0)
                                    {
                                        cell.Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                                        cell.Title = string.Format("Темп прироста к {0:dd.MM.yyyy}г.", lastDate);
                                    }
                                    else if (growRate < 0)
                                    {
                                        cell.Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                                        cell.Title = string.Format("Темп прироста к {0:dd.MM.yyyy}г.", lastDate);
                                    }
                                }
                                else
                                {
                                    double growRate = Convert.ToDouble(cell.Value.ToString());
                                    cell.Value = growRate.ToString("P2");

                                    if (growRate > 0)
                                    {
                                        cell.Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                                        cell.Title = string.Format("Темп прироста к {0:dd.MM.yyyy}г.", lastDate);
                                    }
                                    else if (growRate < 0)
                                    {
                                        cell.Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                        cell.Title = string.Format("Темп прироста к {0:dd.MM.yyyy}г.", lastDate);
                                    }
                                }
                                cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";

                            }
                            cell.Style.BorderDetails.WidthTop = 0;
                            break;
                        }
                    case 2:
                        {
                            if (cell.Value != null)
                            {
                                cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N2");
                            }
                            cell.Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                }
            }
        }

        #endregion

        #region Обработчики диаграммы

        private void ChartDataBind()
        {
            string queryText = DataProvider.GetQueryText("STAT_0002_0008_chart");
            chartDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(queryText, "Показатель", chartDt);
            if (chartDt.Rows.Count > 0)
            {
                ChartBrick.Chart.Series.Clear();
                foreach (DataRow row in chartDt.Rows)
                {
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        if (i == 0 && row[i] != DBNull.Value)
                        {
                            row[i] = row[i].ToString().Replace("\"", "'");
                        }
                    }
                }
                for (int i = 1; i < chartDt.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, chartDt);
                    series.Label = chartDt.Columns[i].ColumnName;
                    ChartBrick.Chart.Series.Add(series);
                }
            }
        }

        #endregion

        private void ExportGridSetup()
        {
            for (int i = 0; i < GridBrick.Grid.Rows.Count; i++)
            {
                UltraGridCell cell = GridBrick.Grid.Rows[i].Cells[0];

                int groupIndex = i % 3;

                switch (groupIndex)
                {
                    case 0:
                        {
                            cell.Value = String.Empty;
                            cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
                            break;
                        }
                    case 1:
                        {
                            cell.Style.BorderDetails.StyleTop = BorderStyle.None;
                            cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
                            break;
                        }
                    case 2:
                        {
                            cell.Value = String.Empty;
                            cell.Style.BorderDetails.StyleTop = BorderStyle.None;
                            break;
                        }
                }
            }
        }

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;
            SetExportGridParams(headerLayout.Grid);

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            ChartBrick.Chart.Width = Convert.ToInt32(ChartBrick.Chart.Width.Value * 0.7);
            ReportExcelExporter1.Export(ChartBrick.Chart, ChartCaption.Text, sheet2, 3);
        }

        private void SetExportGridParams(UltraWebGrid grid)
        {
            string exportFontName = "Verdana";
            int fontSize = 10;
            double coeff = 1.0;

            grid.Columns.Add("Безымяный столбик");
            foreach (UltraGridRow Row in grid.Rows)
            {
                if (Row.Index % 3 == 0)
                {
                    Row.Cells.FromKey("Безымяный столбик").Value = "Значение";
                    Row.NextRow.Cells.FromKey("Безымяный столбик").Value = "Абсолютное отклонение";
                    Row.NextRow.NextRow.Cells.FromKey("Безымяный столбик").Value = "Темп прироста";
                }
            }

            headerLayout = new GridHeaderLayout(GridBrick.Grid);
            headerLayout.AddCell("Наименование");

            // Заголовки
            GridHeaderCell header;
            GridHeaderCell underheader;
            headerLayout.AddCell(" "); 
            headerLayout.AddCell("Объём, млн. рублей, ВСЕГО");
            headerLayout.AddCell("Доля отказов по кредитным продуктам, от общего числа заявок на кредиты, %");
            header = headerLayout.AddCell("Потребительские кредиты");

            header.AddCell("Объём выданных кредитов, млн. руб.");
            header.AddCell("Количество выданных кредитов, единиц");
            underheader = header.AddCell("Условия кредитования");
            underheader.AddCell("% ставка");
            underheader.AddCell("сроки (месяцев)");

            header = headerLayout.AddCell("Ипотечные кредиты");

            header.AddCell("Объём выданных кредитов, млн. руб.");
            header.AddCell("Количество выданных кредитов, единиц");
            underheader = header.AddCell("Условия кредитования");
            underheader.AddCell("% ставка");
            underheader.AddCell("сроки (месяцев)");

            headerLayout.ApplyHeaderInfo();

            grid.Columns.FromKey("Безымяный столбик").Move(1);
            grid.Columns.FromKey("Безымяный столбик").Width = 180;

            foreach (UltraGridColumn column in grid.Columns)
            {
                column.Width = Convert.ToInt32(column.Width.Value * coeff);
                column.CellStyle.Font.Name = exportFontName;
                column.Header.Style.Font.Name = exportFontName;
                column.CellStyle.Font.Size = fontSize;
                column.Header.Style.Font.Size = fontSize;
            }
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ExportGridSetup();
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            SetExportGridParams(headerLayout.Grid);

            ReportPDFExporter1.HeaderCellHeight = 60;
            ReportPDFExporter1.Export(headerLayout, "", section1);

            ISection section2 = report.AddSection();
            ChartBrick.Chart.Width = Convert.ToInt32(ChartBrick.Chart.Width.Value * 0.8);
            ReportPDFExporter1.Export(ChartBrick.Chart, ChartCaption.Text, section2);
        }

        #endregion
        protected void FillComboDate(CustomMultiCombo combo, string queryName, int offset)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            if (dtDate.Rows.Count == 0)
            {
                throw new Exception("Данные для построения отчета отсутствуют в кубе");
            }
            Dictionary<string, int> dictDate = new Dictionary<string, int>();
            for (int row = 0; row < dtDate.Rows.Count - offset; ++row)
            {
                string year = dtDate.Rows[row][0].ToString();
                string month = dtDate.Rows[row][3].ToString();
                string day = dtDate.Rows[row][4].ToString();
                AddPairToDictionary(dictDate, year + " год", 0);
                AddPairToDictionary(dictDate, month + " " + year + " года", 1);
                AddPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
            }
            combo.FillDictionaryValues(dictDate);
            combo.SelectLastNode();
        }

        protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
        }

        public string StringToMDXDate(string str)
        {
            string template = "[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
            string[] dateElements = str.Split(' ');
            int year = Convert.ToInt32(dateElements[2]);
            string month = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(CRHelper.MonthNum(dateElements[1])));
            int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
            int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
            int day = Convert.ToInt32(dateElements[0]);
            return String.Format(template, year, halfYear, quarter, month, day);
        }
    }
}