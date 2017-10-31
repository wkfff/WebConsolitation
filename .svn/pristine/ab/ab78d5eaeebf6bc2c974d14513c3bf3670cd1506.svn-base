using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Band;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.TOC;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Section;
//using Infragistics.WebUI.UltraWebGrid.DocumentExport;
//using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using Infragistics.Documents.Reports.Report;
//using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = System.Drawing.Font;
using IList = Infragistics.Documents.Reports.Report.List.IList;
using LinearGradientBrush = System.Drawing.Drawing2D.LinearGradientBrush;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0015
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtGrid1 = new DataTable();
        private DataTable dtGrid2 = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private DateTime currentDate;
        private GridHeaderLayout headerLayout1;
        private GridHeaderLayout headerLayout;


        public bool RFSelected
        {
            get { return ComboFO.SelectedIndex == 0; }
        }

        public bool PlanSelected
        {
            get { return MeasureButtonList.SelectedIndex == 0; }
        }

        #region Параметры запроса

        // Выбранная мера
        private CustomParam selectedMeasure;
        // Выбранный федеральный округ
        private CustomParam selectedFO;
        // Выбранный уровень бюджета
        private CustomParam selectedBudgetLevel;
        private CustomParam PercentPerfomance;
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.3 - 225);
            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid1_DataBound);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid2.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.7 - 225);
            UltraWebGrid2.DataBound += new EventHandler(UltraWebGrid2_DataBound);
            UltraWebGrid2.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.7);

            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            #region Инициализация параметров запроса
           
            if (selectedMeasure == null)
            {
                selectedMeasure = UserParams.CustomParam("selected_measure");
            }
            if (selectedFO == null)
            {
                selectedFO = UserParams.CustomParam("selected_fo");
            }
            if (selectedBudgetLevel == null)
            {
                selectedBudgetLevel = UserParams.CustomParam("selected_budget_level");
            }
            if (PercentPerfomance == null)
            {
                PercentPerfomance = UserParams.CustomParam("percent_performance");
            }
            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Data.ZeroAligned = true;

            UltraChart.Axis.X.Extent = 60;
            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
            UltraChart.Axis.Y.Extent = 40;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart.Axis.X.Labels.WrapText = false;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            UltraChart.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart.Width.Value) / 4;
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 8;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Text = "Млн.руб.";

            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>";
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion


        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0001_0015_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "date", dtDate);
                currentDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
                endYear = currentDate.Year;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                String month = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(currentDate.Month));
                ComboMonth.SetСheckedState(month, true);

                ComboFO.Title = "ФО";
                ComboFO.Width = 300;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));

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

                ComboBudgetLevel.Width = 250;
                ComboBudgetLevel.Title = "Уровень бюджета";
                ComboBudgetLevel.ParentSelect = true;
                ComboBudgetLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillSKIFLevels());
                ComboBudgetLevel.SetСheckedState("Собственный бюджет субъекта", true);
            }

            int year = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(year, ComboMonth.SelectedIndex + 1, 1);

            string region = RFSelected ? "РФ" : RegionsNamingHelper.ShortName(ComboFO.SelectedValue);
            PageTitle.Text = string.Format("Мониторинг сбалансированности бюджетов субъектов {0} ({1})", region, ComboBudgetLevel.SelectedValue);
            Page.Title = PageTitle.Text;
            DateTime nextMonth = currentDate.AddMonths(1);
            PageSubTitle.Text = !PlanSelected
                ? string.Format("Данные по состоянию на 1 {0} {1} года", CRHelper.RusMonthGenitive(nextMonth.Month), nextMonth.Year)
                : string.Format("План на {1} год по состоянию на 1 {0} {1} года", CRHelper.RusMonthGenitive(nextMonth.Month), nextMonth.Year);
            chartElementCaption.Text = string.Format("Распределение бюджетов субъектов РФ по объему профицита(+)/дефицита(-) ({0})", region);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));

            selectedMeasure.Value = PlanSelected ? "Назначено" : "Исполнено";
            selectedBudgetLevel.Value = ComboBudgetLevel.SelectedValue;
            if (RFSelected)
            {
                selectedFO.Value = " ";
            }
            else
            {
                selectedFO.Value = string.Format(".[{0}]", ComboFO.SelectedValue);
            }
            // Из за проблем скрытия столбцов в выгрузке делаем такую штуку
            if (PlanSelected)
            {
                PercentPerfomance.Value = ",[Measures].[Процент исполнения]";
            }
            else
            {
                PercentPerfomance.Value = string.Empty;
            }
            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();

            headerLayout = new GridHeaderLayout(UltraWebGrid2);
            UltraWebGrid2.Bands.Clear();
            UltraWebGrid2.DataBind();

            UltraChart.DataBind();
        }

        #region Обработчики грида 1

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0015_grid1");
            dtGrid1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid1);

            if (dtGrid1.Rows.Count > 0)
            {
                UltraWebGrid1.DataSource = dtGrid1;
            }
        }

        void UltraWebGrid1_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.Width = Unit.Empty;
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = 300;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            SetColumnParams(e.Layout, 0, 1, "", 150, false);
            SetColumnParams(e.Layout, 0, 2, "", 150, false);
            SetColumnParams(e.Layout, 0, 3, "", 60, true);
            SetColumnParams(e.Layout, 0, 4, "", 60, true);

            headerLayout1.AddCell("Показатель", "");
            headerLayout1.AddCell("План", "");
            headerLayout1.AddCell("Факт", "");
            headerLayout1.ApplyHeaderInfo();

        }

        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                string rowName = e.Row.Cells[0].Value.ToString();
                bool greenRow = rowName.ToLower().Contains("профицит");
                bool redRow = rowName.ToLower().Contains("дефицит");
                bool hintColumn = (i == 1 || i == 2);
                bool hintRow = (e.Row.Index == 1 || e.Row.Index == 2 || e.Row.Index == 3);
                bool rubValueRow = (e.Row.Index == 4 || e.Row.Index == 5);

                if (i == 0 && greenRow)
                {
                    e.Row.Style.ForeColor = Color.Green;
                }
                if (i == 0 && redRow)
                {
                    e.Row.Style.ForeColor = Color.Red;
                }

                if (i == 0 && hintRow)
                {
                    e.Row.Cells[i].Style.Padding.Left = 15;
                }
                if (hintRow && hintColumn && e.Row.Cells[i + 2].Value != null)
                {
                    e.Row.Cells[i].Title = e.Row.Cells[i + 2].Value.ToString();
                }

                double value;
                if (i != 0 && rubValueRow && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty &&
                    double.TryParse(e.Row.Cells[i].Value.ToString(), out value))
                {
                    e.Row.Cells[i].Value = string.Format("{0:N3}", value);
                }
            }
        }

        #endregion

        #region Обработчики грида 2

        protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0015_grid2");
            dtGrid2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid2);

            if (dtGrid2.Rows.Count > 0)
            {
                UltraWebGrid2.DataSource = dtGrid2;
            }
        }

        void UltraWebGrid2_DataBound(object sender, EventArgs e)
        {
            if (!RFSelected)
            {
                UltraWebGrid2.Height = Unit.Empty;
            }
        }

        protected void UltraWebGrid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                if (PlanSelected)
                {
                    if ((i - 2) % 3 == 0)
                    {
                        formatString = "N2";
                    }

                    else if (i != e.Layout.Bands[0].Columns.Count - 2)
                    {
                        formatString = "P2";
                    }
                }
                else
                    if (i != e.Layout.Bands[0].Columns.Count - 2)
                    {
                        formatString = "N2";
                    }

                int widthColumn = PlanSelected ? 90 : 128;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(190);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(40);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            // скрываем ранги
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }
            if (PlanSelected)
            {
                headerLayout.AddCell("Субъект");
                headerLayout.AddCell("ФО");
                GridHeaderCell cell = headerLayout.AddCell("Доходы итого");
                cell.AddCell("Назначено, млн.руб.", "Сумма плановых годовых назначений бюджета субъекта РФ");
                cell.AddCell("Темп роста", "Темп роста к аналогичному периоду прошлого года ");
                cell.AddCell("Процент исполнения", "Процент исполнения годовых назначений");
                cell = headerLayout.AddCell("Налоговые и неналоговые доходы");
                cell.AddCell("Назначено, млн.руб.", "Сумма плановых годовых назначений бюджета субъекта РФ");
                cell.AddCell("Темп роста", "Темп роста к аналогичному периоду прошлого года ");
                cell.AddCell("Процент исполнения", "Процент исполнения годовых назначений");
                cell = headerLayout.AddCell("Расходы итого");
                cell.AddCell("Назначено, млн.руб.", "Сумма плановых годовых назначений бюджета субъекта РФ");
                cell.AddCell("Темп роста", "Темп роста к аналогичному периоду прошлого года ");
                cell.AddCell("Процент исполнения", "Процент исполнения годовых назначений");
                cell = headerLayout.AddCell("Объем дефицита(-) / профицита(+)");
                cell.AddCell("Назначено, млн.руб.", "Плановый объем профицита(+)/дефицита(-)");
                headerLayout.ApplyHeaderInfo();
                #region было так, но из за косякового вывода пришлось делать через headerLayout, могут быть не уточнены тонкости задачи
                /*
                for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 3)
                {
                    bool deficitColumn = (i == e.Layout.Bands[0].Columns.Count - 3);
                    bool rankColumn = (i + 1 == e.Layout.Bands[0].Columns.Count - 2);
                    int spanColumnCount = deficitColumn ? 1 : PlanSelected ? 3 : 2;

                    ColumnHeader ch = new ColumnHeader(true);
                    string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                    ch.Caption = captions[0];


                    string factHint = string.Format("Сумма {0} бюджета субъекта РФ", PlanSelected ? "плановых годовых назначений" : "фактического исполнения");
                    string rateHint = rankColumn
                        ? string.Format("Ранг (место) субъекта РФ по объему профицита(+)/дефицита(-) в {0}", RFSelected ? "РФ" : "ФО")
                        : "Темп роста к аналогичному периоду прошлого года ";
                    string deficitHint = PlanSelected ? "Плановый объем профицита(+)/дефицита(-)" : "Фактический объем профицита(+)/дефицита(-)";

                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, PlanSelected ? "Назначено, млн.руб." : "Исполнено, млн.руб.", deficitColumn ? deficitHint : factHint);
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, rankColumn ? "Ранг" : "Темп роста", rateHint);
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "Процент исполнения", "Процент исполнения годовых назначений");


                    ch.RowLayoutColumnInfo.OriginY = 0;
                    ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                    multiHeaderPos += 3;
                    ch.RowLayoutColumnInfo.SpanX = spanColumnCount;
                    e.Layout.Bands[0].HeaderLayout.Add(ch);
                }*/
                #endregion
            }
            else
            {
                headerLayout.AddCell("Субъект");
                headerLayout.AddCell("ФО");
                GridHeaderCell cell = headerLayout.AddCell("Доходы итого");
                cell.AddCell("Исполнено, млн.руб.", "Сумма фактического исполнения бюджета субъекта РФ");
                cell.AddCell("Темп роста", "Темп роста к аналогичному периоду прошлого года ");
                cell = headerLayout.AddCell("Налоговые и неналоговые доходы");
                cell.AddCell("Исполнено, млн.руб.", "Сумма фактического исполнения бюджета субъекта РФ");
                cell.AddCell("Темп роста", "Темп роста к аналогичному периоду прошлого года ");
                cell = headerLayout.AddCell("Расходы итого");
                cell.AddCell("Исполнено, млн.руб.", "Сумма фактического исполнения бюджета субъекта РФ");
                cell.AddCell("Темп роста", "Темп роста к аналогичному периоду прошлого года ");
                cell = headerLayout.AddCell("Объем дефицита(-) / профицита(+)");
                cell.AddCell("Исполнено, млн.руб.", "Фактический объем профицита(+)/дефицита(-)");
                headerLayout.ApplyHeaderInfo();
                #region было так, но из за косякового вывода пришлось делать через headerLayout, могут быть не уточнены тонкости задачи
                /*  for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
                  {
                      bool deficitColumn = (i == e.Layout.Bands[0].Columns.Count - 2);
                      bool rankColumn = (i + 1 == e.Layout.Bands[0].Columns.Count - 1);
                      int spanColumnCount = deficitColumn ? 1 : PlanSelected ? 3 : 2;

                      ColumnHeader ch = new ColumnHeader(true);
                      string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                      ch.Caption = captions[0];

                      string factHint = string.Format("Сумма {0} бюджета субъекта РФ", PlanSelected ? "плановых годовых назначений" : "фактического исполнения");
                      string rateHint = rankColumn
                          ? string.Format("Ранг (место) субъекта РФ по объему профицита(+)/дефицита(-) в {0}", RFSelected ? "РФ" : "ФО")
                          : "Темп роста к аналогичному периоду прошлого года ";
                      string deficitHint = PlanSelected ? "Плановый объем профицита(+)/дефицита(-)" : "Фактический объем профицита(+)/дефицита(-)";

                      CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, PlanSelected ? "Назначено, млн.руб." : "Исполнено, млн.руб.", deficitColumn ? deficitHint : factHint);
                      CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, rankColumn ? "Ранг" : "Темп роста", rateHint);

                      ch.RowLayoutColumnInfo.OriginY = 0;
                      ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                      multiHeaderPos += 2;
                      ch.RowLayoutColumnInfo.SpanX = spanColumnCount;
                      e.Layout.Bands[0].HeaderLayout.Add(ch);

                  }*/
                #endregion

            }




        }

        protected void UltraWebGrid2_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rank = (i == e.Row.Cells.Count - 2);
                bool rate = true;
                if (PlanSelected)
                {
                    rate = !rank && ((i - 2) % 3 == 1);
                }
                else
                {
                    rate = !rank && ((i - 2) % 2 == 1);
                }
                if (rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty && e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("Наиболее профицитный бюджет субъекта по {0}", !RFSelected ? "ФО" : "РФ");
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = string.Format("Наиболее дефицитный бюджет субъекта по {0}", !RFSelected ? "ФО" : "РФ");
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
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Прирост к прошлому году";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
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
            string query = DataProvider.GetQueryText("FK_0001_0015_chart");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            DataTable dtCopyChart = dtChart.Copy();
            if (dtCopyChart.Columns.Count > 2)
            {
                dtCopyChart.Columns[1].ColumnName = dtCopyChart.Columns[1].ColumnName.Split(';')[0];
                dtCopyChart.Columns.RemoveAt(2);
            }

            UltraChart.DataSource = dtCopyChart;
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

                    text.SetTextString(RegionsNamingHelper.ShortName(text.GetTextString()));
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        string otherMeasureText = string.Empty;
                        double otherValue = 0;
                        if (dtChart != null && dtChart.Rows[box.Row][2] != DBNull.Value &&
                            dtChart.Rows[box.Row][2].ToString() != string.Empty)
                        {
                            otherValue = Convert.ToDouble(dtChart.Rows[box.Row][2]);
                            if (!PlanSelected)
                            {
                                otherMeasureText = otherValue > 0 ? "Плановый профицит (годовой)" : "Плановый дефицит (годовой)";
                            }
                            else
                            {
                                otherMeasureText = otherValue > 0 ? "Фактический профицит" : "Фактический дефицит";
                            }
                        }

                        double value = Convert.ToDouble(box.Value);
                        if (value > 0)
                        {
                            box.DataPoint.Label = string.Format("{0} {1:N2} млн.руб.\n ({2} {3:N2} млн.руб.)",
                                !PlanSelected ? "Фактический профицит" : "Плановый профицит (годовой)",
                                value, otherMeasureText, otherValue);
                            box.PE.ElementType = PaintElementType.Gradient;
                            box.PE.FillGradientStyle = GradientStyle.Horizontal;
                            box.PE.Fill = Color.Green;
                            box.PE.FillStopColor = Color.ForestGreen;
                        }
                        else
                        {
                            box.DataPoint.Label = string.Format("{0} {1:N2} млн.руб.\n ({2} {3:N2} млн.руб.)",
                                !PlanSelected ? "Фактический дефицит" : "Плановый дефицит (годовой)",
                                value, otherMeasureText, otherValue);
                            box.PE.ElementType = PaintElementType.Gradient;
                            box.PE.FillGradientStyle = GradientStyle.Horizontal;
                            box.PE.Fill = Color.Red;
                            box.PE.FillStopColor = Color.Maroon;
                        }
                    }
                    else if (box.Path != null && box.Path.ToLower().Contains("legend") && box.rect.Width < 20)
                    {
                        box.PE.ElementType = PaintElementType.CustomBrush;
                        LinearGradientBrush brush = new LinearGradientBrush(box.rect, Color.Red, Color.Green, 45, false);
                        box.PE.CustomBrush = brush;
                    }
                }
            }
        }

        #endregion

        #region Export excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");
            sheet2.Rows[0].Cells[0].Value = PageTitle.Text;
            sheet2.Rows[1].Cells[0].Value = PageSubTitle.Text;
            sheet3.Rows[0].Cells[0].Value = PageTitle.Text;
            sheet3.Rows[1].Cells[0].Value = PageSubTitle.Text;
            sheet3.Rows[2].Cells[0].Value = chartElementCaption.Text;

            ReportExcelExporter1.HeaderCellHeight = 70;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout1, sheet1, 3);
            ReportExcelExporter1.Export(headerLayout, sheet2, 3);
            ReportExcelExporter1.Export(UltraChart, sheet3, 4);

        }
        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout1, section1);
            IText text = section1.AddText();
            text.AddContent(" ");
            ReportPDFExporter1.Export(headerLayout,PageSubTitle.Text, section1);

            ISection section2 = report.AddSection();
            IText title = section2.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            title = section2.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chartElementCaption.Text);

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 5;
            ReportPDFExporter1.Export(UltraChart, section2);
        }


    }
        #endregion


    public class ReportSection : ISection
    {
        private readonly bool withFlowColumns;
        private readonly ISection section;
        private IFlow flow;
        private ITableCell titleCell;

        public ReportSection(Report report, bool withFlowColumns)
        {
            this.withFlowColumns = withFlowColumns;
            section = report.AddSection();
            ITable table = section.AddTable();
            ITableRow row = table.AddRow();
            titleCell = row.AddCell();
        }

        public void AddFlowColumnBreak()
        {
            if (flow != null)
                flow.AddColumnBreak();
        }

        public IBand AddBand()
        {
            return section.AddBand();
        }

        #region ISection members
        public ISectionHeader AddHeader()
        {
            throw new NotImplementedException();
        }

        public ISectionFooter AddFooter()
        {
            throw new NotImplementedException();
        }

        public IStationery AddStationery()
        {
            throw new NotImplementedException();
        }

        public IDecoration AddDecoration()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(PageSize size)
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(float width, float height)
        {
            throw new NotImplementedException();
        }

        public ISegment AddSegment()
        {
            throw new NotImplementedException();
        }

        public IQuickText AddQuickText(string text)
        {
            throw new NotImplementedException();
        }

        public IQuickImage AddQuickImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            throw new NotImplementedException();
        }

        public IQuickList AddQuickList()
        {
            throw new NotImplementedException();
        }

        public IQuickTable AddQuickTable()
        {
            throw new NotImplementedException();
        }

        public IText AddText()
        {
            return this.section.AddText();
        }

        public IImage AddImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            return this.section.AddImage(image);
        }

        public IMetafile AddMetafile(Metafile metafile)
        {
            throw new NotImplementedException();
        }

        public IRule AddRule()
        {
            throw new NotImplementedException();
        }

        public IGap AddGap()
        {
            throw new NotImplementedException();
        }

        public IGroup AddGroup()
        {
            throw new NotImplementedException();
        }

        public IChain AddChain()
        {
            throw new NotImplementedException();
        }

        public ITable AddTable()
        {
            if (flow != null)
                return flow.AddTable();
            return this.section.AddTable();
        }

        public IGrid AddGrid()
        {
            throw new NotImplementedException();
        }

        public IFlow AddFlow()
        {
            throw new NotImplementedException();
        }

        public IList AddList()
        {
            throw new NotImplementedException();
        }

        public ITree AddTree()
        {
            throw new NotImplementedException();
        }

        public ISite AddSite()
        {
            throw new NotImplementedException();
        }

        public ICanvas AddCanvas()
        {
            throw new NotImplementedException();
        }

        public IRotator AddRotator()
        {
            throw new NotImplementedException();
        }

        public IContainer AddContainer(string name)
        {
            throw new NotImplementedException();
        }

        public ICondition AddCondition(IContainer container, bool fit)
        {
            throw new NotImplementedException();
        }

        public IStretcher AddStretcher()
        {
            throw new NotImplementedException();
        }

        public void AddPageBreak()
        {
            throw new NotImplementedException();
        }

        public ITOC AddTOC()
        {
            throw new NotImplementedException();
        }

        public IIndex AddIndex()
        {
            throw new NotImplementedException();
        }

        public bool Flip
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public PageSize PageSize
        {
            get { throw new NotImplementedException(); }
            set { this.section.PageSize = new PageSize(960, 1350); }
        }

        public PageOrientation PageOrientation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.ContentAlignment PageAlignment
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Borders PageBorders
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Margins PageMargins
        {
            get { return this.section.PageMargins; }
            set { throw new NotImplementedException(); }
        }

        public Paddings PagePaddings
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Background PageBackground
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Section.PageNumbering PageNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public SectionLineNumbering LineNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Report Parent
        {
            get { return this.section.Parent; }
        }

        public IEnumerable Content
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
