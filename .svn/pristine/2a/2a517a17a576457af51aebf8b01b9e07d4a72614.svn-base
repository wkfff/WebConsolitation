using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using System.Web;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections.Generic;
namespace Krista.FM.Server.Dashboards.reports.FO_0042_0003_Gub
{
    public partial class _default : CustomReportPage
    {
        #region Поля 

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart;
        private int firstYear = 2008;
        private int endYear = 2011;

        private int currentYear;
        private double avgEvaluation;

        #endregion

        #region Параметры запроса

        // выбранный индикатор
        private CustomParam selectedIndicator;
        private CustomParam selectedQuater;
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            Grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            Grid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.45);
            Grid.DisplayLayout.NoDataMessage = "Нет данных";
            Grid.ActiveRowChange += new ActiveRowChangeEventHandler(Grid_ActiveRowChange);


            Chart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            Chart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.50);
            Chart.FillSceneGraph += new FillSceneGraphEventHandler(Chart_FillSceneGraph);
            Chart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #region Настройка диаграммы

            Chart.ChartType = ChartType.ColumnChart;
            Chart.ColumnChart.ColumnSpacing = 0;
            Chart.ColumnChart.SeriesSpacing = 0; 
            Chart.Data.ZeroAligned = true;
            Chart.Border.Thickness = 0;

            Chart.Axis.X.Extent = 30;
            Chart.Axis.X.Labels.Visible = false;
            Chart.Axis.X.Labels.SeriesLabels.Visible = true;
            Chart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            Chart.Axis.Y.Extent = 20;
            Chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:0.##>";
            Chart.Axis.Y.Labels.Font = new Font("Verdana", 8);


            Chart.Axis.X2.Visible = true;
            Chart.Axis.X2.Labels.Visible = false;
            Chart.Axis.X2.Labels.SeriesLabels.Visible = false;
            Chart.Axis.X2.LineColor = Color.Transparent;
            Chart.Axis.X2.Extent = 10;

            Chart.Legend.Margins.Right = 3 * Convert.ToInt32(Chart.Width.Value) / 4;
            Chart.Legend.Font = new Font("Verdana", 8);
            Chart.Legend.Visible = true;
            Chart.Legend.Location = LegendLocation.Top;
            Chart.Legend.SpanPercentage = 9;
            Chart.Legend.FormatString = "Среднее";

            Chart.TitleLeft.Visible = true;
            Chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            Chart.TitleLeft.Margins.Bottom = Chart.Axis.X.Extent;
            Chart.TitleLeft.Text = "Количество ГРБС";
            Chart.TitleLeft.Font = new Font("Verdana", 8);

            Chart.TitleBottom.Visible = true;
            Chart.TitleBottom.HorizontalAlign = StringAlignment.Center;
            Chart.TitleBottom.Margins.Left = Chart.Axis.Y.Extent;
            Chart.TitleBottom.Text = "Оценки";
            Chart.TitleBottom.Font = new Font("Verdana", 8);

            //CRHelper.FillCustomColorModel(UltraChart, 10, true);

            Chart.Legend.MoreIndicatorText = " ";
            Chart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            Color color1 = Color.LimeGreen;
            Color color2 = Color.DodgerBlue;
            Color color3 = Color.Gold;
            Color color4 = Color.Red;
            Color color5 = Color.Orange;

            Chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            Chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            Chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color3, 150));
            Chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color4, 150));
            Chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color5, 150));
            Chart.ColorModel.Skin.ApplyRowWise = true;

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N0>";
            appearance.ChartTextFont = new Font("Verdana", 9, FontStyle.Bold);
            appearance.Visible = true;
            Chart.ColumnChart.ChartText.Add(appearance);

           // Chart.Tooltips.FormatString = "Оценка в интервале <SERIES_LABEL> наблюдается у <DATA_VALUE:0.##> ГРБС";
            Chart.Tooltips.FormatString = "<ITEM_LABEL>";

            #endregion

            #region Инициализация параметров запроса

            if (selectedIndicator == null)
            {
                selectedIndicator = UserParams.CustomParam("selected_indicator");
                
            }
            selectedQuater = UserParams.CustomParam("selected_quater");
            #endregion

            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;оценки&nbsp;качества";
            CrossLink1.NavigateUrl = "~/reports/FO_0042_0001_Gub/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Рейтинг&nbsp;ГРБС";
            CrossLink2.NavigateUrl = "~/reports/FO_0042_0002_Gub/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(chartCaption);
                chartWebAsyncPanel.AddRefreshTarget(Chart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(Grid);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0042_0003_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                ComboQuater.Title = "Оценка качества";
                ComboQuater.FillDictionaryValues(QuaterLoad());
                ComboQuater.SelectLastNode();
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboQuater.Width = 300;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(YearsLoad());
                //ComboYear.SetСheckedState(endYear.ToString(), true);
                ComboYear.SelectLastNode();
                
                hiddenIndicatorLabel.Text = "[Показатели].[Оценка качества ФМ].[Данные всех источников].[ФО\0042 Оценка качества ФМ - 2009 квартал 0].[Качество ведения реестра расходных обязательств Омской области]";
            }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue);
            if (ComboQuater.SelectedIndex == 1)
            {
                selectedQuater.Value = "Данные года";
                Page.Title = String.Format("Анализ оценки качества финансового менеджмента, осуществляемого главными распорядителями средств областного бюджета в разрезе показателей по состоянию на {0}","1.01." + (int.Parse(ComboYear.SelectedValue) + 1).ToString() + " года");

            }
            else
            {
                selectedQuater.Value = "Остатки на начало года";
                Page.Title = String.Format("Анализ оценки качества финансового менеджмента, осуществляемого главными распорядителями средств областного бюджета в разрезе показателей по состоянию на {0}", "1.07." + ComboYear.SelectedValue + " года");
            }
            
            PageTitle.Text = Page.Title;

            UserParams.PeriodYear.Value = currentYear.ToString();

            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                Grid.Bands.Clear();
                Grid.DataBind();

                string patternValue = selectedIndicator.Value;
                int defaultRowIndex = 0;
                if (patternValue == string.Empty)
                {
                    defaultRowIndex = 0;
                }

                if (Grid.Columns.Count > 0 && Grid.Rows.Count > 0)
                {
                    // ищем строку
                    UltraGridRow row = CRHelper.FindGridRow(Grid, patternValue, Grid.Columns.Count - 1, defaultRowIndex);
                    // выделяем строку
                    ActiveGridRow(row);
                }
            }

            //UltraChart.DataBind();
        }

        Dictionary<string, int> YearsLoad()
        {
            DataTable dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("FO_0042_0003_date"), "Дата", dt);
            Dictionary<string, int> d = new Dictionary<string, int>();
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                d.Add(dt.Rows[i][0].ToString(), 0);
            }
            return d;
        }

        Dictionary<string, int> QuaterLoad()
        {
            Dictionary<string, int> d = new Dictionary<string, int>();
            d.Add("по состоянию на 1.07", 0);
            d.Add("по итогам года", 0);
            return d;
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

            string indicatorCode = row.Cells[1].Text;
            string indicatorName = row.Cells[2].Text;

            avgEvaluation = Double.MinValue;
            Chart.Legend.Visible = false;
            if (row.Cells[3].Value != null && row.Cells[3].Value.ToString() != String.Empty)
            {
                avgEvaluation = Convert.ToDouble(row.Cells[3].Value);
                Chart.Legend.Visible = true;
                Chart.Legend.FormatString = String.Format("Среднее значение оценки: {0:N2}", avgEvaluation);
            }

            hiddenIndicatorLabel.Text = row.Cells[row.Cells.Count - 1].Text;
            selectedIndicator.Value = hiddenIndicatorLabel.Text;

            chartCaption.Text = String.Format("Показатель «{0}» ({1})", indicatorName, indicatorCode);

            Chart.DataBind();
        }

        void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        protected void Grid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0003_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателя", dtGrid);
            if (dtGrid.Rows.Count > 0 && dtGrid.Columns.Count > 2 && dtGrid.Columns.Contains("Средняя оценка показателя исходя из его применимости к ГРБС"))
            {
                dtGrid.Columns.RemoveAt(0);

                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 4; i <= 6; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            row[i] = GetShortGRBSNames(row[i].ToString());
                        }
                    }
                }

                Grid.DataSource = dtGrid;
            }
            else
            {
                Grid.DataSource = null;
            }
        }

        private static string GetShortGRBSNames(string fullNames)
        {
            string shortNames = String.Empty;

            string[] names = fullNames.Split(';');
            foreach (string s in names)
            {
                if (s != String.Empty)
                {
                    shortNames += String.Format("{0}, ", GetShortGRBSName(s));
                }
            }

            return shortNames.TrimEnd(' ').TrimEnd(',');
        }

        public static Dictionary<string, string> ShortGRBSNames
        {
            get
            {
                // если словарь пустой
                if (shortGRBSNames == null || shortGRBSNames.Count == 0)
                {
                    // заполняем его
                    FillShortGRBSNames();
                }
                return shortGRBSNames;
            }
        }

        public static string GetShortGRBSName(string fullName)
        {
            if (ShortGRBSNames.ContainsKey(fullName))
            {
                return ShortGRBSNames[fullName];
            }
            return fullName;
        }

        private static Dictionary<string, string> shortGRBSNames = new Dictionary<string, string>();
        private static Dictionary<string, string> shortGRBSCodes = new Dictionary<string, string>();

        private static void FillShortGRBSNames()
        {
         //   shortGRBSNames = new Dictionary<string, string>();
         //   shortGRBSCodes = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("shortGRBSNames");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Краткие наименования ГРБС", dt);

            foreach (DataRow row in dt.Rows)
            {
                shortGRBSNames.Add(row[0].ToString(), row[1].ToString());
                shortGRBSCodes.Add(row[1].ToString(), row[2].ToString());
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            UltraGridColumn numberColumn = e.Layout.Bands[0].Columns[0];
            numberColumn.Header.Caption = "№ п/п";
            numberColumn.Width = CRHelper.GetColumnWidth(30);
            numberColumn.CellStyle.Padding.Right = 5;
            numberColumn.CellStyle.BackColor = numberColumn.Header.Style.BackColor;
            numberColumn.CellStyle.Font.Bold = true;
            numberColumn.SortingAlgorithm = SortingAlgorithm.NotSet;
            numberColumn.CellStyle.HorizontalAlign = HorizontalAlign.Right;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "";
                int widthColumn = 250;
                bool textWrap = false;
                HorizontalAlign textAlignment = HorizontalAlign.Left;

                switch (i)
                {
                    case 1:
                        {
                            widthColumn = 50;
                            textAlignment = HorizontalAlign.Left;
                            break;
                        }
                    case 2:
                        {
                            widthColumn = 230;
                            textWrap = true;
                            break;
                        }
                    case 3:
                        {
                            widthColumn = 100;
                            formatString = "N2";
                            textAlignment = HorizontalAlign.Right;
                            break;
                        }
                    case 4:
                    case 5:
                    case 6:
                        {
                            widthColumn = 233;
                            textWrap = true;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = textAlignment;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = textWrap;
            }

            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Обозн.", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Наименование показателя", "");
            //CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, e.Layout.Bands[0].Columns[2].Header.Caption, "Ед.изм.: балл");
           // CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, e.Layout.Bands[0].Columns[3].Header.Caption, "");
          //  CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, e.Layout.Bands[0].Columns[4].Header.Caption, "");
        //    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, e.Layout.Bands[0].Columns[5].Header.Caption, "");

            e.Layout.Bands[0].Columns[7].Hidden = true;
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Value = Convert.ToInt32(e.Row.Index + 1).ToString("N0");

            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != String.Empty)
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

        protected void Chart_DataBinding(object sender, EventArgs e)
        {
            string query = currentYear == 2008
                ? DataProvider.GetQueryText("FO_0042_0003_chart_fourth")
                : DataProvider.GetQueryText("FO_0042_0003_chart_tenth");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            bool flag = true;
            for (int i = 0; i < dtChart.Rows.Count;i++ )
            {
                if (dtChart.Rows[i][2] != DBNull.Value)
                {
                    flag = false;
                }
            }
            if (flag)
            {
                Chart.DataSource = null;
                dtChart = null;
            }
            else
            {
                foreach (DataRow row in dtChart.Rows)
                {
                    
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("_", "]");
                    }

                    if (row[2] != DBNull.Value)
                    {
                        row[2] = GetShortGRBSNames(row[2].ToString());
                    }
                }

                DataTable dtChartCopy = dtChart.Copy();
                if (dtChartCopy.Columns.Count > 2)
                {
                    dtChartCopy.Columns.RemoveAt(2);
                }

                Chart.DataSource = dtChartCopy;
            }
        }


        protected void Chart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            if (dtChart!=null)
            {
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                    {
                        Text axisText = (Text)primitive; 
                        axisText.bounds.Width = 50;
                        axisText.labelStyle.HorizontalAlign = StringAlignment.Near;
                        axisText.labelStyle.FontSizeBestFit = false;
                        axisText.labelStyle.Font = new Font("Verdana", 8);
                        axisText.labelStyle.WrapText = false;
                    //    axisText.bounds.X -= 20;
                    }
                    if (primitive is Box)
                    {
                        Box box = (Box)primitive;
                        if (box.DataPoint != null && box.Value != null)
                        {
                            int rowIndex = box.Row; 
                            int columnIndex = 2;
                            
                            string indicatorList = "Значение оценки - " + dtChart.Rows[box.Row][0].ToString() + " наблюдается у " + dtChart.Rows[box.Row][1].ToString() + " ГРБС\n" ;
                            string list = String.Format("({0})", dtChart.Rows[box.Row][2].ToString().Replace("\"","\'"));
                            list = BreakCollocator(list, ',', 3);
                            indicatorList += list.Replace(",", ", ");
                            
                            box.DataPoint.Label = indicatorList;
                        }
                        else if (box.Path != null && box.Path.ToLower().Contains("legend") && box.rect.Width < 20)
                        {
                            box.PE.Fill = Color.MediumPurple;
                            box.PE.FillStopColor = Color.MediumOrchid;
                            box.rect = new Rectangle(box.rect.X, box.rect.Y + box.rect.Height / 3, box.rect.Width, box.rect.Height / 3);
                        }
                    }
                }

                if (avgEvaluation != Double.MinValue)
                {
                    IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
                    IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

                    if (xAxis == null || yAxis == null)
                        return;

                    double yMin = yAxis.MapMinimum;
                    double yMax = yAxis.MapMaximum;

                    double axisStep = (xAxis.Map(1) - xAxis.Map(0));
                    int scale = currentYear == 2010 ? 1 : 10;
                    double colIndex = (avgEvaluation+0.5) * scale;
                    double lineX = xAxis.Map(colIndex) + avgEvaluation / axisStep;

                    Line line = new Line();
                    line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                    line.p1 = new Point((int)lineX, (int)yMax);
                    line.PE.Stroke = Color.MediumOrchid;
                    line.PE.FillOpacity = 50;
                    line.PE.StrokeWidth = 5;
                    line.p2 = new Point((int)lineX, (int)yMin);
                    e.SceneGraph.Add(line);
                    //e.SceneGraph.Insert(10, line);
                }
            }
        }

        private static string BreakCollocator(string source, char breakChar, int charIndex)
        {
            string breakedStr = String.Empty;

            int charCount = 0;
            foreach (char ch in source)
            {
                breakedStr += ch;
                if (ch == breakChar)
                {
                    charCount++;
                    if (charCount == charIndex)
                    {
                        breakedStr += "\n";
                        charCount = 0;
                    }
                }
            }

            return breakedStr;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = Grid.Columns.Count - 1;
            int rowsCount = Grid.Rows.Count;

            e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "0";
            e.CurrentWorksheet.Columns[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            e.CurrentWorksheet.Columns[0].Width = 90 * 37;

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[beginExportIndex - 1].Height = 20 * 37;
                e.CurrentWorksheet.Rows[beginExportIndex - 1].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[beginExportIndex - 1].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[beginExportIndex - 1].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                string formatString = "";
                int widthColumn = 250;
                ExcelDefaultableBoolean textWrap = ExcelDefaultableBoolean.False;
                HorizontalCellAlignment textAlignment = HorizontalCellAlignment.Left;

                switch (i)
                {
                    case 1:
                        {
                            widthColumn = 50;
                            formatString = "0";
                            textAlignment = HorizontalCellAlignment.Right;
                            break;
                        }
                    case 2:
                        {
                            widthColumn = 240;
                            textWrap = ExcelDefaultableBoolean.True;
                            break;
                        }
                    case 3:
                        {
                            widthColumn = 100;
                            formatString = "#,###0.00;[Red]-#,###0.00";
                            textAlignment = HorizontalCellAlignment.Right;
                            break;
                        }
                    case 4:
                    case 5:
                    case 6:
                        {
                            widthColumn = 400;
                            textWrap = ExcelDefaultableBoolean.True;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = textAlignment;
                e.CurrentWorksheet.Columns[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Columns[i].CellFormat.WrapText = textWrap;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }

            // расставляем стили у начальных колонок
            for (int i = beginExportIndex; i < rowsCount + beginExportIndex; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 50 * 37;
            }

            int chartRowIndex = beginExportIndex + rowsCount + 2;
            e.CurrentWorksheet.Rows[chartRowIndex - 1].Cells[0].Value = chartCaption.Text;

            Chart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Chart.Legend.Margins.Right = 2 * Convert.ToInt32(Chart.Width.Value) / 3;

            UltraGridExporter.ChartExcelExport(e.CurrentWorksheet.Rows[chartRowIndex].Cells[0], Chart);
        }

        private int beginExportIndex = 4;

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = beginExportIndex;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(Grid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = Grid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {

            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(Grid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

           

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chartCaption.Text);

            Chart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Chart.Legend.Margins.Right = 2 * Convert.ToInt32(Chart.Width.Value) / 3;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(Chart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
