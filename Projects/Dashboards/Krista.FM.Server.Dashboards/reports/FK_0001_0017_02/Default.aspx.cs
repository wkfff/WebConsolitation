using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font=System.Drawing.Font;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0017_02
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2006;
        private int endYear = 2011;
        private DateTime currentDate;
        private string shortSelectedRegion;

        public bool PlanSelected
        {
            get { return MeasureButtonList.SelectedIndex == 0; }
        }

        public bool RFSelected
        {
            get { return ComboFO.SelectedIndex == 0; }
        }

        #region Параметры запроса

        // Выбранная мера
        private CustomParam selectedMeasure;
        // Выбранный уровень бюджета
        private CustomParam selectedBudgetLevel;
        // Выбранный код ФКР
        private CustomParam selectedFKR;
        // Выбранный федеральный округ
        private CustomParam selectedFO;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 225);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
            UltraChart.FillSceneGraph +=new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #region Инициализация параметров запроса

            if (selectedMeasure == null)
            {
                selectedMeasure = UserParams.CustomParam("selected_measure");
            }
            if (selectedBudgetLevel == null)
            {
                selectedBudgetLevel = UserParams.CustomParam("selected_budget_level");
            }
            if (selectedFKR == null)
            {
                selectedFKR = UserParams.CustomParam("selected_fkr");
            }
            if (selectedFO == null)
            {
                selectedFO = UserParams.CustomParam("selected_fo");
            }

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.StackColumnChart;
            UltraChart.BorderWidth = 0;

            UltraChart.Tooltips.FormatString = string.Format("<SERIES_LABEL>\n<ITEM_LABEL>: <DATA_VALUE:P2>");

            UltraChart.Axis.X.Extent = 60;
            UltraChart.Axis.Y.Extent = 50;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.ColumnChart.SeriesSpacing = 0;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 11;
            //UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value / 4);
            UltraChart.Legend.Font = new Font("Verdana", 8);

            UltraChart.ColorModel.ModelStyle = ColorModels.CustomLinear;

            UltraChart.Effects.Effects.Clear();
            GradientEffect effect = new GradientEffect();
            effect.Coloring = GradientColoringStyle.Darken;
            effect.Enabled = true;
            UltraChart.Effects.Enabled = true;
            UltraChart.Effects.Effects.Add(effect);  

            UltraChart.Data.ZeroAligned = true;
            UltraChart.Data.SwapRowsAndColumns = true;
                        
            #endregion

            CrossLink.Visible = true;
            CrossLink.Text = "Распределение&nbsp;субъектов&nbsp;РФ&nbsp;по&nbsp;доле&nbsp;расходов&nbsp;на&nbsp;увеличение&nbsp;основных&nbsp;средств";
            CrossLink.NavigateUrl = "~/reports/FK_0001_0017_01/Default.aspx";

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0017_02_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

                int yearNum = Convert.ToInt32(dtDate.Rows[0][0].ToString());
                int monthNum = Convert.ToInt32(CRHelper.MonthNum(dtDate.Rows[0][3].ToString()));

                currentDate = new DateTime(yearNum, monthNum, 1);
                endYear = currentDate.Year;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 115;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                String month = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(currentDate.Month));
                ComboMonth.SetСheckedState(month, true);

                ComboFO.Title = "ФО";
                ComboFO.Width = 250;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
                ComboFO.SetСheckedState("Все федеральные округа", true);

                if (!string.IsNullOrEmpty(UserParams.Region.Value))
                {
                    ComboFO.SetСheckedState(UserParams.Region.Value, true);
                }
                else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    string foName = RegionSettings.Instance.Id.ToLower() == "urfo"
                                         ? "Уральский федеральный округ"
                                         : RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name);
                    ComboFO.SetСheckedState(foName, true);
                }

                ComboFKR.Width = 280;
                ComboFKR.Title = "РзПр";
                ComboFKR.MultiSelect = false;
                ComboFKR.ParentSelect = true;
                ComboFKR.FillDictionaryValues(CustomMultiComboDataHelper.FillFKRNames(DataDictionariesHelper.OutcomesFKRTypes, DataDictionariesHelper.OutcomesFKRLevels));
                ComboFKR.RemoveTreeNodeByName("Итого внутренних оборотов");
                ComboFKR.RemoveTreeNodeByName("Расходы бюджета - ВСЕГО");
                ComboFKR.SetСheckedState("Расходы бюджета - ИТОГО", true);

                ComboBudgetLevel.Width = 250;
                ComboBudgetLevel.Title = "Уровень бюджета";
                ComboBudgetLevel.ParentSelect = true;
                ComboBudgetLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillSKIFLevels());
                ComboBudgetLevel.SetСheckedState("Собственный бюджет субъекта", true);
            }

            int year = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(year, ComboMonth.SelectedIndex + 1, 1);

            shortSelectedRegion = RFSelected ? "РФ" : RegionsNamingHelper.ShortName(ComboFO.SelectedValue);

            PageTitle.Text = string.Format("Распределение субъектов {0} по доле первоочередных расходов", shortSelectedRegion);
            Page.Title = PageTitle.Text;
            PageSubTitle.Text = string.Format("{4}, {5} ({2} {0} {3} {1} года)",
                currentDate.Month, currentDate.Year, PlanSelected ? "план на" : "факт за", CRHelper.RusManyMonthGenitive(currentDate.Month),
                ComboFKR.SelectedValue, ComboBudgetLevel.SelectedValue);
            chartElementCaption.Text = string.Format("Распределение субъектов {0} по доле расходов на первоочередные расходы", shortSelectedRegion);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));

            selectedMeasure.Value = PlanSelected ? "Назначено" : "Исполнено";
            selectedBudgetLevel.Value = ComboBudgetLevel.SelectedValue;
            if (ComboFKR.SelectedValue.Contains("Расходы бюджета - ИТОГО"))
            {
                selectedFKR.Value = "[РзПр].[Сопоставимый].[Расходы бюджета - ИТОГО ]";
            }
            else
            {
                selectedFKR.Value = DataDictionariesHelper.OutcomesFKRTypes[ComboFKR.SelectedValue];
            }

            if (RFSelected)
            {
                selectedFO.Value = " ";
            }
            else
            {
                selectedFO.Value = string.Format(".[{0}]", ComboFO.SelectedValue);
            }

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            UltraChart.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0017_02_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект РФ", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {

        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N2";
                int widthColumn = 100;

                switch(i)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        {
                            formatString = "N2";
                            break;
                        }
                    case 5:
                        {
                            widthColumn = 110;
                            formatString = "N2";
                            break;
                        }
                    case 6:
                    case 8:
                    case 9:
                        {
                           formatString = "P2";
                           break;
                        }
                    case 7:
                        {
                            formatString = "N0";
                            break;
                        }
                }
                
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(190);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Расходы на оплату труда и начисления на выплаты по оплате труда, тыс.руб.", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Расходы на социальное обеспечение, тыс.руб.", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Расходы на коммунальные услуги, тыс.руб.", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Расходы на обслуживание гос. (муниц.) долга, тыс.руб.", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Расходы всего, тыс.руб.", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "Доля первоочередных расходов в расходах бюджета", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, string.Format("Ранг в {0} по доле первоочередных расходов", shortSelectedRegion), "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, string.Format("Оценка доли первоочередных расходов в расходах бюджета по {0}", shortSelectedRegion), "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9, "Темп роста первоочередных расходов в расходах бюджета", "");
            
            e.Layout.Bands[0].Columns[10].Hidden = true;//8
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rank = (i == 7);
                bool rate = (i == 9);

                if (rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[e.Row.Cells.Count - 1].Value != null &&
                         e.Row.Cells[i].Value.ToString() != string.Empty && e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("Cамая маленькая доля расходов по {0}", shortSelectedRegion); 
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[e.Row.Cells.Count - 1].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = string.Format("Cамая большая доля расходов по {0}", shortSelectedRegion); 
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            e.Row.Cells[i].Title = "Прирост к прошлому году";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            e.Row.Cells[i].Title = "Снижение к прошлому году";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
                {
                    if (!RegionsNamingHelper.IsSubject(e.Row.Cells[0].Value.ToString()))
                    {
                        e.Row.Cells[i].Style.Font.Bold = true;
                    }
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
        
        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0017_02_chart");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);


            if (dtChart.Rows.Count > 1)
            {
                // получаем среднее значение (последний элемент в таблице)
                double avgValue = 0;
                if (dtChart.Rows.Count != 0 && dtChart.Rows[dtChart.Rows.Count - 1][0] != DBNull.Value)
                {
                    DataRow row = dtChart.Rows[dtChart.Rows.Count - 1];
                    if (row[0].ToString().Contains("Среднее"))
                    {
                        avgValue = Convert.ToDouble(row[1]);
                        // удаляем строку со средним из таблицы
                        dtChart.Rows.Remove(row);
                    }
                }

                // рассчитываем медиану
                int medianIndex = MedianIndex(dtChart.Rows.Count);
                DataTable medianDT = dtChart.Clone();
                for (int i = 0; i < dtChart.Rows.Count; i++)
                {

                    medianDT.ImportRow(dtChart.Rows[i]);

                    double value;
                    Double.TryParse(dtChart.Rows[i][1].ToString(), out value);
                    if (value >= avgValue && i != dtChart.Rows.Count - 1)
                    {
                        Double.TryParse(dtChart.Rows[i + 1][1].ToString(), out value);
                        if (value < avgValue)
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Среднее";
                            row[2] = avgValue;
                            medianDT.Rows.Add(row);
                        }
                    }

                    if (i == medianIndex)
                    {
                        DataRow row = medianDT.NewRow();
                        row[0] = "Медиана";
                        row[2] = MedianValue(dtChart, "Доля первоочередных расходов в расходах бюджета");
                        medianDT.Rows.Add(row);
                    }
                }

                for (int i = 2; i < medianDT.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, medianDT);
                    series.Label = medianDT.Columns[i].ColumnName;
                    UltraChart.Series.Add(series);
                }
            }
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    text.bounds.Width = 30;
                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                    text.labelStyle.FontSizeBestFit = false;
                    text.labelStyle.Font = new Font("Verdana", 7);
                    text.labelStyle.WrapText = true;

                    if (text.GetTextString() == "Среднее" || text.GetTextString() == "Медиана")
                    {
                        LabelStyle boldStyle = text.GetLabelStyle();
                        boldStyle.Font = new Font("Verdana", 8, FontStyle.Bold);
                        boldStyle.FontColor = Color.Black;
                        text.SetLabelStyle(boldStyle);
                    }
                    else
                    {
                        text.SetTextString(RegionsNamingHelper.ShortName(text.GetTextString()));
                    }
                }

                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.Series != null && (box.Series.Label == "Среднее" || box.Series.Label == "Медиана"))
                        {
                            box.PE.Fill = Color.Orange;
                            box.PE.FillStopColor = Color.OrangeRed;
                            box.DataPoint.Label = "Доля первоочередных расходов в расходах бюджета";
                        }
                    }
                }
            }
        }

        #endregion

        #region Расчет медианы

        private static bool Even(int input)
        {
            if (input % 2 == 0)
            {
                return true;
            }
            return false;
        }

        private static int MedianIndex(int length)
        {
            if (length == 0)
            {
                return 0;
            }

            if (Even(length))
            {
                return length / 2 - 1;
            }
            else
            {
                return (length + 1) / 2 - 1;
            }
        }

        private static double MedianValue(DataTable dt, string medianValueColumn)
        {
            if (dt.Rows.Count == 0)
            {
                return 0;
            }

            if (Even(dt.Rows.Count))
            {
                double value1;
                double value2;
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count)][medianValueColumn].ToString(),
                        out value1);
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count) + 1][medianValueColumn].ToString(),
                        out value2);
                return (value1 + value2) / 2;
            }
            else
            {
                double value;
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count)][medianValueColumn].ToString(),
                        out value);
                return value;
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;

            if (e.CurrentWorksheet.Name == "Диаграмма")
            {
                UltraGridExporter.ChartExcelExport(e.CurrentWorksheet.Rows[3].Cells[0], UltraChart);
            }
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;

            e.CurrentWorksheet.Columns[0].Width = 200*37;

            // расставляем стили у ячеек хидера
            for (int i = 0; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 25*37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }
            
            int width = 170;

            for (int i = 1; i < columnCount; i++)
            {
                string formatString = "#,##0.00;[Red]-#,##0.00";

                switch (i)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        {
                            formatString = "#,##0.00;[Red]-#,##0.00";
                            break;
                        }
                    case 6:
                    case 8:
                    case 9:
                        {
                            formatString = UltraGridExporter.ExelPercentFormat;
                            break;
                        }
                    case 7:
                        {
                            formatString = "#0";
                            break;
                        }
                }
                e.CurrentWorksheet.Columns[i].Width = width * 37;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");

            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
            UltraGridExporter1.ExcelExporter.Export(emptyExportGrid, sheet2);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            UltraGridExporter1.GridElementCaption = PageSubTitle.Text;
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private bool titleAdded = false;

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            if (!titleAdded)
            {
                IText title = e.Section.AddText();
                Font font = new Font("Verdana", 16);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.Style.Font.Bold = true;
                title.AddContent(PageTitle.Text);

//                title = e.Section.AddText();
//                font = new Font("Verdana", 14);
//                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
//                title.AddContent(PageSubTitle.Text);
            }
            
            titleAdded = true;
        }

        private void PdfExporter_EndExport(object sender,  Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chartElementCaption.Text);

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.9));
            UltraChart.Legend.Margins.Right = 5;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
