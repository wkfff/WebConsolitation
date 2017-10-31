using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color=System.Drawing.Color;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;

namespace Krista.FM.Server.Dashboards.reports.STAT_0001_0009
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private DataTable dtChart;
        private DataTable dtChartAverage;
        private DataTable dtChart12;
        private DataTable dtDate;

        #endregion

        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 900; }
        }

        private static int MinScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private static int MinScreenHeight
        {
            get { return CustomReportConst.minScreenHeight; }
        }
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            //UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart1.Width = IsSmallResolution ? CRHelper.GetChartWidth(MinScreenWidth - 15) : CRHelper.GetChartWidth(MinScreenWidth - 61);
            UltraChart1.Height = CRHelper.GetChartHeight(MinScreenHeight / 1.5);
            
            #region Настройка диаграммы

            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.Y.Extent = 45;
            UltraChart1.Axis.X.Extent = 40;
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\nЗаработная плата <DATA_VALUE:N2> руб.";
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 50;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 12);
            UltraChart1.Axis.X.Labels.FontColor = Color.Black;
            UltraChart1.Data.ZeroAligned = true;
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "руб.";
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Far;
            //UltraChart1.TitleLeft.Visible = true;

             #endregion

            UltraGridExporter1.Visible = true;
            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <DocumentExportEventArgs>(PdfExporter_BeginExport);
        }

        private int offsetX = 0;
        private int startOffsetX = 0;
        private Dictionary<string, int> widths = new Dictionary<string, int>();
        private Dictionary<string, int> kindNumbers = new Dictionary<string, int>();
        private int widthAll = 0;
        private double workersAll = 0;

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box) primitive;
                    if (box.DataPoint != null)
                    {
                        widthAll += box.rect.Width - 2;
                    }
                }
            }
             
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box) primitive;
                    if (box.DataPoint != null)
                    {
                        if (offsetX == 0)
                        {
                            offsetX = box.rect.X;
                            startOffsetX = box.rect.X;
                        }

                        double workersCount = GetWorkersCount(box.DataPoint.Label);
                        if (workersCount > 0)
                        {
                            double width = (workersCount / workersAll) * widthAll;
                            widths.Add(box.DataPoint.Label, (int)width);
                            box.rect.X = offsetX + 2;
                            box.rect.Width = widths[box.DataPoint.Label];
                            offsetX += box.rect.Width + 2;
                            box.DataPoint.Label +=
                                String.Format("<br />Численность занятого населения {0:N0} чел.", workersCount);
                        }
                    }
                }
            } 
            offsetX = 0;
            int labelCount = 1;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text)
                {
                    Text text = (Text) primitive;
                    if (text.Path.Contains("Border.Title.Grid.X"))
                    {
                        if (offsetX == 0)
                        {
                            offsetX = startOffsetX;
                        }
                        if (widths.ContainsKey(text.GetTextString()))
                        {
                            text.bounds.X = offsetX - 8 + widths[text.GetTextString()]/2;
                            offsetX += widths[text.GetTextString()] + 2;
                            kindNumbers.Add(text.GetTextString(), labelCount);
                            text.SetTextString(labelCount.ToString());
                            text.bounds.Width = 20;
                            labelCount++;
                        }
                    }
                }
            }
            IAdvanceAxis xAxis = (IAdvanceAxis) e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis) e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int xMin = (int) xAxis.MapMinimum;
            int xMax = (int) xAxis.MapMaximum;
            
            double urfoAverage;
            if (double.TryParse(dtChartAverage.Rows[0][0].ToString(), out urfoAverage))
            {
                int fmY = (int) yAxis.Map(urfoAverage);
                Line line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dot;
                line.PE.Stroke = Color.DarkGray;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point(xMin, fmY);
                line.p2 = new Point(xMax, fmY);
                e.SceneGraph.Add(line);
                               
                Text text = new Text();
                text.labelStyle.Font = new System.Drawing.Font("Verdana", 10);
                text.PE.Fill = Color.Black;
                text.bounds = new Rectangle(xMin + 10, fmY - 20, 780, 15);
                text.SetTextString(
                    String.Format("Средняя з/п {0}: {1:N2} руб.",
                                  RegionsNamingHelper.ShortName(ComboRegion.SelectedValue), urfoAverage));
                e.SceneGraph.Add(text);
            }
        }


        void UltraChart1_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                widthLegendLabel = (int)UltraChart1.Width.Value + 20;

                widthLegendLabel -= UltraChart1.Legend.Margins.Left + UltraChart1.Legend.Margins.Right;
                text.bounds.Width = widthLegendLabel;
                if (kindNumbers.ContainsKey(text.GetTextString()))
                {
                    int kindNumber = kindNumbers[text.GetTextString()];
                    if (IsSmallResolution && text.GetTextString().Contains("Оптовая и розничная торговля"))
                    {
                        text.SetTextString(
                            "Оптовая и розничная торговля; ремонт автотранспортных средств, бытовых изделий");
                    }
                    text.SetTextString(String.Format(" {0}. {1} ({2:N2} руб.)", kindNumber, text.GetTextString(), dtChart.Rows[0][kindNumber - 1]));
                }
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0009_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodYear.Value = dtDate.Rows[0][0].ToString();
            
            if (!Page.IsPostBack)
            {
                FillComboRegions();
                ComboRegion.Title = "Территория";
                ComboRegion.Width = 400;
                ComboRegion.SetСheckedState("Уральский федеральный округ", true);
                ComboRegion.ParentSelect = true;
                
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.ShowSelectedValue = false;
                ComboYear.ParentSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(2005, 2009));
                ComboYear.SelectLastNode();
                ComboYear.PanelHeaderTitle = "Год";
            }
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            Page.Title = "Распределение занятых в экономике и среднемесячная заработная плата";
            PageTitle.Text = "Распределение занятых в экономике и среднемесячная номинальная начисленная заработная плата и выплаты социального характера в расчете на одного работника";
            PageSubTitle.Text = string.Format("{0}, за {1} год.",ComboRegion.SelectedValue, UserParams.PeriodYear.Value);
                       
            UserParams.SubjectFO.Value = ComboRegion.SelectedValue == "Уральский федеральный округ"
                                             ? String.Empty
                                             : String.Format(".[{0}]", ComboRegion.SelectedValue);

            UserParams.Filter.Value = ComboRegion.SelectedValue == "Уральский федеральный округ"
                                             ? ".DataMember"
                                             : String.Format(".[{0}]", ComboRegion.SelectedValue);
            
            chart1ElementCaption.Text = "Распределение занятых в экономике и среднемесячная начисленная заработная плата работников организаций (с учетом социальных выплат)";
            
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
            UltraChart1.DataBind();
        }

        private void FillComboRegions()
        {
            DataTable dtRegions = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0009_regions");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dtRegions);

            Dictionary<string, int> regions = new Dictionary<string, int>();
            //regions.Add(dtRegions.Rows[0][1].ToString(), 0);
            foreach (DataRow row in dtRegions.Rows)
            {
                regions.Add(row[0].ToString(), 0);
            }
            regions.Add("Уральский федеральный округ", 0);
            ComboRegion.FillDictionaryValues(regions);
        }
        
        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0009_grid"));
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Вид деятельности ОКВЭД", dtGrid);
            ((UltraWebGrid)sender).DataSource = dtGrid;
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid1.Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
           
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(280, 1280);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(180, 1280);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(180, 1280);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0009_chart1"));
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart);

            query = DataProvider.GetQueryText(String.Format("STAT_0001_0009_chart1_2"));
            dtChart12 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart12);

            for (int i = 0; i < dtChart.Columns.Count; i++)
            {
                workersAll += GetWorkersCount(dtChart.Columns[i].ColumnName);
            }

            query = DataProvider.GetQueryText(String.Format("STAT_0001_0009_chart1_average"));
            dtChartAverage = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChartAverage);

            UltraChart1.DataSource = dtChart;
        }

        private double GetWorkersCount(string kind)
        {
            for (int i = 0; i < dtChart12.Columns.Count; i++)
            {
                if (dtChart12.Columns[i].Caption == kind)
                    return Convert.ToDouble(dtChart12.Rows[0][i]);
            }
            return 0;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid1.Columns.Count;
            int rowsCount = UltraWebGrid1.Rows.Count;

            for (int i = 0; i < columnCount; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 100 * 37;
            }

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00";
            
            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount + 4; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1);
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            while (col.Hidden)
            {
                offset++;
                col = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            }
            e.HeaderText = col.Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF


        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            UltraChart1.Legend.Margins.Right = 0;
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);

            e.Section.AddPageBreak();
        }

        #endregion

        #region Непустые дни мониторинга рынка труда

        /// <summary>
        /// Непустые дни мониторинга рынка труда
        /// </summary>
        private static Dictionary<string, string> newWorkplaceNonEmptyDays;

        /// <summary>
        /// Возвращает словарь непустых дней мониторинга рынка труда
        /// </summary>
        public static Dictionary<string, string> NewWorkplaceNonEmptyDays
        {
            get
            {
                // если словарь пустой
                if (newWorkplaceNonEmptyDays == null || newWorkplaceNonEmptyDays.Count == 0)
                {
                    // заполняем его
                    FillNewWorkplaceNonEmptyDays();
                }
                return newWorkplaceNonEmptyDays;
            }
        }

        private static void FillNewWorkplaceNonEmptyDays()
        {
            newWorkplaceNonEmptyDays = new Dictionary<string, string>();

            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText("NewWorkplaceNonEmptyDays");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            string year = string.Empty;
            string month = string.Empty;

            foreach (DataRow row in dt.Rows)
            {
                string curYear = row[0].ToString();
                if (curYear != year)
                {
                    newWorkplaceNonEmptyDays.Add(GetDictionaryUniqueKey(newWorkplaceNonEmptyDays, curYear), "Year");
                    year = curYear;
                    month = string.Empty;
                }

                string curMonth = row[3].ToString();
                if (curMonth != month)
                {
                    newWorkplaceNonEmptyDays.Add(GetDictionaryUniqueKey(newWorkplaceNonEmptyDays, curMonth), "Month");
                    month = curMonth;
                }

                string day = GetDictionaryUniqueKey(newWorkplaceNonEmptyDays, row[4].ToString());
                newWorkplaceNonEmptyDays.Add(day, "Day");
            }
        }

        #endregion 

        private static string GetDictionaryUniqueKey(Dictionary<string, string> dictionary, string key)
        {
            string newKey = key;
            while (dictionary.ContainsKey(newKey))
            {
                newKey += " ";
            }
            return newKey;
        }
    }
}
