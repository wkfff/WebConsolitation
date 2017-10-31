using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Font=System.Drawing.Font;
using Orientation = Infragistics.Documents.Excel.Orientation;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0025
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2006;
        private int endYear = 2011;
        private int currentYear;
        private int currentMonth;
        
        private GridHeaderLayout headerLayout;
        private static MemberAttributesDigest grbsDigest;

        #endregion

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private OutcomesType SelectedOutcomesType
        {
            get
            {
                if (RzPrCheckBox.Checked && KOSGUCheckBox.Checked)
                {
                    return OutcomesType.RzPr_KOSGU;
                }
                else if (RzPrCheckBox.Checked)
                {
                    return OutcomesType.RzPr;
                }
                else
                {
                    return OutcomesType.KOSGU;
                }
            }
        }

        #region Параметры запроса

        // множество видов расходов
        private CustomParam outcomesSet;
        // выбранная мера
        private CustomParam selectedChartMeasure;
        // выбранный бюджет
        private CustomParam selectedBudget;
        // множество для рядов диаграммы
        private CustomParam chartRowSet;
        // выбранный бюджет для диаграммы
        private CustomParam chartBudget;
        // выбранный множитель рублей
        private CustomParam rubMultiplier;
        // фильтр по измерению
        private CustomParam dimensionSlicer;
        // уровень районов
        private CustomParam regionsLevel;

        #endregion

        private bool IsFactSelected
        {
            get { return MeasureFact.Checked; }
        }

        private bool IsGRBS
        {
            get { return grbsDigest.GetMemberType(ComboBudget.SelectedValue) == "ГРБС"; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.EnableViewState = false;

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.85);
            UltraChart.FillSceneGraph +=new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.StackColumnChart;
            UltraChart.BorderWidth = 0;
            
            UltraChart.Axis.X.Extent = 180;
            UltraChart.Axis.Y.Extent = 70;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Text = RubMiltiplierButtonList.SelectedValue;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent + Convert.ToInt32(UltraChart.Height.Value * UltraChart.Legend.SpanPercentage / 100); 
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 7;
            UltraChart.Legend.Font = new Font("Verdana", 9);

            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;

            UltraChart.Data.ZeroAligned = true;
            
            #endregion

            #region Инициализация параметров запроса

            outcomesSet = UserParams.CustomParam("outcomes_set");
            selectedChartMeasure = UserParams.CustomParam("selected_chart_measure");
            selectedBudget = UserParams.CustomParam("selected_budget");
            chartRowSet = UserParams.CustomParam("chart_row_set");
            chartBudget = UserParams.CustomParam("chart_budget");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            dimensionSlicer = UserParams.CustomParam("dimension_slicer");
            regionsLevel = UserParams.CustomParam("regions_level");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            ReportPDFExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            if (!Page.IsPostBack)
            {
                MeasureFact.Attributes.Add("onclick", string.Format("uncheck('{0}', false)", MeasurePlan.ClientID));
                MeasurePlan.Attributes.Add("onclick", string.Format("uncheck('{0}', false)", MeasureFact.ClientID));

                RzPrCheckBox.Attributes.Add("onclick", string.Format("uncheck('{0}', true)", KOSGUCheckBox.ClientID));
                KOSGUCheckBox.Attributes.Add("onclick", string.Format("uncheck('{0}', true)", RzPrCheckBox.ClientID));

                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(MeasureFact.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(MeasurePlan.ClientID);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0025_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month, true);

                ComboBudget.Title = "Бюджет";
                ComboBudget.Width = 500;
                ComboBudget.MultiSelect = false;
                ComboBudget.ParentSelect = true;
                grbsDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "OmskGRBSList", CRHelper.BasePath);
                ComboBudget.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(grbsDigest.UniqueNames, grbsDigest.MemberLevels));
                ComboBudget.SetСheckedState("Консолидированный бюджет субъекта", true);
            }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue);
            currentMonth = ComboMonth.SelectedIndex + 1;

            PageTitle.Text = "Оценка исполнения, структуры и динамики расходов";
            Page.Title = PageTitle.Text;
            PageSubTitle.Text = String.Format("{3} за {0} {1} {2} года", currentMonth, CRHelper.RusManyMonthGenitive(currentMonth), currentYear, ComboBudget.SelectedValue);
            chartElementCaption.Text = "Структура расходов бюджета в разрезе основных статей КОСГУ";

            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";

            if (IsGRBS && SelectedOutcomesType == OutcomesType.RzPr)
            {
                dimensionSlicer.Value = "[КОСГУ].[Сопоставимый].[Фильтр по КОСГУ],";
            }
            else if (SelectedOutcomesType == OutcomesType.KOSGU)
            {
                dimensionSlicer.Value = "[РзПр].[Сопоставимый].[Фильтр по РзПр],";
            }
            else
            {
                dimensionSlicer.Value = String.Empty;
            }

            UserParams.PeriodYear.Value = currentYear.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentMonth));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentMonth));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentMonth);
            selectedBudget.Value = grbsDigest.GetMemberUniqueName(ComboBudget.SelectedValue);

            chartRowSet.Value = IsGRBS ? "Список ГРБС" : "Список РзПр";
            chartBudget.Value = IsGRBS ? " " : "," + selectedBudget.Value;

            switch (SelectedOutcomesType)
            {
                case OutcomesType.RzPr:
                    {
                        outcomesSet.Value = "Список РзПр";
                        break;
                    }
                case OutcomesType.KOSGU:
                    {
                        outcomesSet.Value = "Список КОСГУ";
                        break;
                    }
                case OutcomesType.RzPr_KOSGU:
                    {
                        outcomesSet.Value = "Список РзПр-КОСГУ";
                        break;
                    }
            }

            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                headerLayout = new GridHeaderLayout(UltraWebGrid);
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();
            }

            UltraChart.Tooltips.FormatString = String.Format("{1}: <SERIES_LABEL>\nКОСГУ: <ITEM_LABEL>\n{0}: <DATA_VALUE:N2> тыс.руб.",
                 IsFactSelected ? "Исполнено" : "Сумма годовых назначений",
                 IsGRBS ? "ГРБС" : "РзПр");
            selectedChartMeasure.Value = IsFactSelected ? "Факт, тыс.руб." : "Годовые назначения, тыс.руб.";
            UltraChart.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0025_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                if (dtGrid.Columns.Count > 1)
                {
                    dtGrid.Columns.RemoveAt(0);
                }

                UltraWebGrid.DataSource = dtGrid;
            }
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
                int columnIndex = (i - 1) % 3;
                int groupIndex = (i - 1) / 3;

                string formatString = "N0";
                int widthColumn = 150;

                switch (columnIndex)
                {
                    case 0:
                        {
                            widthColumn = 85;
                            formatString = "N1";
                            break;
                        }
                    case 1:
                        {
                            widthColumn = groupIndex > 1 ? 75 : 85;
                            formatString = groupIndex > 1 ? "N2" : "N1";
                            break;
                        }
                    case 2:
                        {
                            widthColumn = groupIndex > 1 ? 60 : 70;
                            formatString = "N2";
                            break;
                        }
                }
                
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(180);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            headerLayout.AddCell("Наименование показателей");

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 3)
            {
                int groupIndex = (i - 1) / 3;
                string headerCaption = String.Empty;
                switch (groupIndex)
                {
                    case 0:
                        {
                            headerCaption = String.Format("{0} год", currentYear - 1);
                            break;
                        }
                    case 1:
                        {
                            headerCaption = String.Format("{0} год", currentYear);
                            break;
                        }
                    case 2:
                        {
                            headerCaption = String.Format("{0} к {1} годовые назначения", currentYear, currentYear - 1);
                            break;
                        }
                    case 3:
                        {
                            headerCaption = String.Format("{0} к {1} факт", currentYear, currentYear - 1);
                            break;
                        }
                }

                string rubItemCaption = RubMiltiplierButtonList.SelectedValue;
                GridHeaderCell header = headerLayout.AddCell(headerCaption);

                if (groupIndex < 2)
                {
                    header.AddCell(
                        String.Format("Годовые назначения, {0}", rubItemCaption),
                         groupIndex == 0 
                            ? "Годовые назначения в прошлом году"
                            : "Годовые назначения на текущий год");
                    header.AddCell(
                        String.Format("Факт, {0}", rubItemCaption),
                        String.Format("Исполнено за {0} {1} {2} года", currentMonth, CRHelper.RusManyMonthGenitive(currentMonth),
                            groupIndex == 0
                                ? "прошлого"
                                : "текущего"));
                    header.AddCell(
                        "% исполн.",
                        String.Format("% исполнения годовых назначений за {0} {1} {2} года", currentMonth, CRHelper.RusManyMonthGenitive(currentMonth),
                            groupIndex == 0
                                ? "прошлого"
                                : "текущего"));
                }
                else
                {
                    header.AddCell(
                        String.Format("Отклонение (+,-), {0}", rubItemCaption),
                           String.Format("Отклонение {0} текущего года от прошлого",
                                groupIndex == 2
                                    ? "назначения"
                                    : "исполнения"));
                    header.AddCell(
                        "Темп роста, %",
                        String.Format("Темп роста {0} к аналогичному периоду прошлого года", 
                            groupIndex == 2
                                ? "годовых назначений"
                                : "факта "));
                    header.AddCell(
                        "Рост/ сниж., %",
                        String.Format("Рост (снижение) {0} расходов к прошлому году",
                            groupIndex == 2
                                ? "годовых назначений"
                                : "факта "));
                }
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string levelName = String.Empty;
            int levelColumnIndex = e.Row.Cells.Count - 1;
            if (e.Row.Cells[levelColumnIndex].Value != null)
            {
                levelName = e.Row.Cells[levelColumnIndex].Value.ToString();
            }

            e.Row.Style.Padding.Right = 5;

            for (int i = 0; i < e.Row.Cells.Count - 1; i++)
            {
                bool executePercent = (i == 6);
                bool growRate = (i == 8 || i == 11);
                
                if (executePercent && e.Row.Cells[i].Value != null)
                {
                    double percent = (ComboMonth.SelectedIndex + 1) * 100.0 / 12;

                    if (Convert.ToDouble(e.Row.Cells[i].Value) < percent)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                        e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:N2}%)", percent);
                    }
                    else
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                        e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:N2}%)", percent);
                    }
                    e.Row.Cells[i].Style.Padding.Right = 2;
                }

                if (growRate && e.Row.Cells[i].Value != null)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = (i == 8) ? "Рост плановых назначений расходов" : "Рост расходов";

                    }
                    else if (Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = (i == 8) ? "Снижение плановых назначений расходов" : "Снижение расходов";
                    }
                }
                e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";

                switch(levelName)
                {
                    case "Раздел":
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                            break;
                        }
                    case "Статья":
                        {
                            e.Row.Cells[i].Style.Font.Bold = (SelectedOutcomesType == OutcomesType.KOSGU);
                            e.Row.Cells[i].Style.Font.Italic = (SelectedOutcomesType == OutcomesType.RzPr_KOSGU);
                            break;
                        }
                    case "Подстатья":
                        {
                            if (SelectedOutcomesType == OutcomesType.RzPr_KOSGU)
                            {
                                e.Row.Cells[i].Style.Padding.Left = 10;
                            }
                            break;
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
            string query = DataProvider.GetQueryText("FO_0002_0025_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Rows.Count > 2)
            {
                if (grbsDigest.GetMemberType(ComboBudget.SelectedValue) == "ГРБС")
                {
                    foreach (DataRow row in dtChart.Rows)
                    {
                        if (row[0] != DBNull.Value)
                        {
                            row[0] = grbsDigest.GetShortName(row[0].ToString());
                        }
                    }
                }

                UltraChart.Series.Clear();
                UltraChart.Data.SwapRowsAndColumns = true;
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart.Series.Add(series);
                }
            }
        }

        protected void UltraChart_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((UltraChart.Legend.Location == LegendLocation.Top) || (UltraChart.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart.Width.Value - 20;
                }
                else
                {
                    widthLegendLabel = (UltraChart.Legend.SpanPercentage * (int)UltraChart.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart.Legend.Margins.Left + UltraChart.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            double axisStep = (xAxis.Map(1) - xAxis.Map(0));

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    
                    if (IsGRBS)
                    {
                        text.bounds.Width = 30;
                        text.bounds.X -= (int)axisStep / 6;
                        text.labelStyle.VerticalAlign = StringAlignment.Center;
                        text.labelStyle.Font = new Font("Verdana", 8);
                    }
                    else
                    {
                        text.bounds.Width = 50;
                        text.bounds.X -= (int)axisStep / 6;
                        text.labelStyle.VerticalAlign = StringAlignment.Center;
                        text.labelStyle.Font = new Font("Verdana", 8);
                    }
                    
                    text.labelStyle.FontSizeBestFit = false;
                    
                    text.labelStyle.WrapText = true;
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 40;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");

            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            ReportExcelExporter1.RowsAutoFitEnable = true;

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart, chartElementCaption.Text, sheet2, 3);
        }

        #endregion

        #region Экспорт в Pdf

        void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(chartElementCaption.Text);

            UltraChart.Legend.Margins.Right = 5;
            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section = report.AddSection();
            section.PageSize = new PageSize(2000, 1200);

            ReportPDFExporter1.HeaderCellHeight = 60;
            ReportPDFExporter1.Export(headerLayout, section);
        }

        #endregion
    }

    public enum OutcomesType
    {
        RzPr,
        KOSGU,
        RzPr_KOSGU
    }
}