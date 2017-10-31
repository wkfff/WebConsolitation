using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
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
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;

namespace Krista.FM.Server.Dashboards.reports.STAT_0001_0011
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtAllocationChart;
        private DataTable dtDynamicChart;
        private DataTable dtAVG = new DataTable();
        private int firstYear = 2008;
        private int endYear = 2009;
        private DateTime date;
        private Dictionary<string, string> FODictionary;

        #endregion

        public bool FactSelected
        {
            get { return MeasureButtonList.SelectedIndex == 0; }
        }

        #region Параметры запроса

        // Выбранная федеральный округ
        private CustomParam selectedFO;
        // Выбранная субъект
        private CustomParam selectedSubject;
        // Выбранная мера
        private CustomParam selectedMeasure;
        // Набор колонок для диаграммы
        private CustomParam chartColumnSet;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 25);
            UltraWebGrid.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6 - 50);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);

            AllocationChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            AllocationChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6 - 50);

            DynamicChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            DynamicChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.6 - 50);

            #region Настройка диаграммы 1

            AllocationChart.ChartType = ChartType.ScatterChart;
            AllocationChart.Border.Thickness = 0;
            AllocationChart.Axis.X.Extent = 50;
            AllocationChart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            AllocationChart.Axis.Y.Extent = 40;
            AllocationChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            AllocationChart.TitleLeft.Visible = true;
            AllocationChart.TitleLeft.Text = "Налоговая нагрузка, тыс.руб.\nна единицу экономически активного населения";
            AllocationChart.TitleLeft.Font = new Font("Verdana", 10);
            AllocationChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            AllocationChart.TitleLeft.Extent = 50;

            AllocationChart.TitleBottom.Visible = true;
            AllocationChart.TitleBottom.Text = "Численность экономически активного населения, чел.";
            AllocationChart.TitleBottom.Font = new Font("Verdana", 10);
            AllocationChart.TitleBottom.HorizontalAlign = StringAlignment.Center;

            AllocationChart.Legend.Visible = false;

            AllocationChart.ScatterChart.Icon = SymbolIcon.Square;
            AllocationChart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            AllocationChart.Tooltips.FormatString = "<SERIES_LABEL>\nНалоговая нагрузка: <DATA_VALUE_Y:N3> тыс.руб. на единицу экономически активного населения\nЧисленность экономически активного населения: <DATA_VALUE_X:N0> чел.";
            AllocationChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            AllocationChart.FillSceneGraph += new FillSceneGraphEventHandler(AllocationChart_FillSceneGraph);

            AllocationChart.Style.Add("padding-top", "10px");

            #endregion

            #region Настройка диаграммы 2

            DynamicChart.ChartType = ChartType.ColumnChart;
            DynamicChart.Border.Thickness = 0;

            DynamicChart.Axis.X.Labels.SeriesLabels.Visible = true;
            DynamicChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            DynamicChart.Axis.X.Labels.Visible = false;
            DynamicChart.Axis.Y.Extent = 40;
            DynamicChart.Axis.Y.Extent = 40;
            DynamicChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            DynamicChart.TitleLeft.Visible = true;
            DynamicChart.TitleLeft.Text = "Тыс.руб.";
            DynamicChart.TitleLeft.Font = new Font("Verdana", 10);
            DynamicChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            DynamicChart.TitleLeft.Margins.Bottom = DynamicChart.Axis.Y.Extent + 10;
            DynamicChart.TitleLeft.Extent = 50;

            DynamicChart.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N3> тыс.руб. на единицу экономически активного населения";

            DynamicChart.Legend.Visible = true;
            DynamicChart.Legend.SpanPercentage = 10;
            DynamicChart.Legend.Location = LegendLocation.Top;

            DynamicChart.Data.SwapRowsAndColumns = false;
            DynamicChart.ColorModel.ModelStyle = ColorModels.CustomLinear;

            DynamicChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            #region Инициализация параметров запроса

            if (selectedFO == null)
            {
                selectedFO = UserParams.CustomParam("selected_fo");
            }
            if (selectedSubject == null)
            {
                selectedSubject = UserParams.CustomParam("selected_subject");
            }
            if (selectedMeasure == null)
            {
                selectedMeasure = UserParams.CustomParam("selected_measure");
            }
            if (chartColumnSet == null)
            {
                chartColumnSet = UserParams.CustomParam("chart_column_set");
            }

            #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            //UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler <DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler <EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                ChartWebAsyncPanel.AddRefreshTarget(DynamicChart.ClientID);
                ChartWebAsyncPanel.AddRefreshTarget(subjectLabel.ClientID);
                ChartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid.ClientID);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("STAT_0001_0011_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query,"date", dtDate);
                date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
                endYear = date.Year;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                String month = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(date.Month));
                ComboMonth.SetСheckedState(month, true);

                FillComboRegions(true);
                ComboRegiones.Title = "Территория";
                ComboRegiones.Width = 400;
                ComboRegiones.SetСheckedState("Уральский федеральный округ", true);

                subjectLabel.Text = "Уральский федеральный округ";
                selectedSubject.Value = "].[Уральский федеральный округ]";
            }
            
            int year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

            if (FODictionary == null || FODictionary.Count == 0)
            {
                FillComboRegions(false);
            }
            selectedFO.Value = FODictionary[ComboRegiones.SelectedValue];
            selectedMeasure.Value = FactSelected ? "Исполнено" : "Назначено";

            Label1.Text = string.Format("Налоговая нагрузка на единицу экономически активного населения ({0})", ComboRegiones.SelectedValue);
            Label2.Text = String.Format("Анализ налоговой нагрузки (по основным видам налогов, уплачиваемых физическими лицами) в субъектах Российской Федерации на единицу экономически активного населения за {0} {1} года",
                    ComboMonth.SelectedValue.ToLower(), ComboYear.SelectedValue);

            if (!ChartWebAsyncPanel.IsAsyncPostBack)
            {
                UltraWebGrid.DataBind();
                AllocationChart.DataBind();

                string patternValue = subjectLabel.Text;
                int defaultRowIndex = 0;
                if (patternValue == string.Empty)
                {
                    patternValue = UserParams.StateArea.Value;
                    defaultRowIndex = 0;
                }

                UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
                ActiveGridRow(row);
            }

            dynamicChartCaption.Text = string.Format("Динамика {0} налоговой нагрузки на единицу экономически активного населения", FactSelected ? "фактической" : "плановой");
            if (!Page.IsPostBack)
            {
                DynamicChart.DataBind();
            }
        }

        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            string subject = row.Cells[0].Text;

            if (RegionsNamingHelper.IsRF(subject))
            {
                selectedSubject.Value = "]";
                chartColumnSet.Value = "[Measures].[Налоговая нагрузка на ед.акт.населения]";                
            }
            else if (RegionsNamingHelper.IsFO(subject))
            {
                selectedSubject.Value = string.Format("].[{0}]", subject);
                chartColumnSet.Value = "[Measures].[Налоговая нагрузка на ед.акт.населения], [Measures].[Средняя налоговая нагрузка по РФ]";
            }
            else
            {
                selectedSubject.Value = string.Format("].[{0}].[{1}]", RegionsNamingHelper.GetFoBySubject(subject), subject);
                chartColumnSet.Value = "[Measures].[Налоговая нагрузка на ед.акт.населения], [Measures].[Средняя налоговая нагрузка по ФО],[Measures].[Средняя налоговая нагрузка по РФ]"; 
            }
            subjectLabel.Text = subject;
            DynamicChart.DataBind();
        }

        void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            string subject = e.Row.Cells[0].Text;
            if (RegionsNamingHelper.IsRF(subject))
            {
                selectedSubject.Value = "]";
                chartColumnSet.Value = "[Measures].[Налоговая нагрузка на ед.акт.населения]";
            }
            else if (RegionsNamingHelper.IsFO(subject))
            {
                selectedSubject.Value = string.Format("].[{0}]", subject);
                chartColumnSet.Value = "[Measures].[Налоговая нагрузка на ед.акт.населения], [Measures].[Средняя налоговая нагрузка по РФ]";
            }
            else
            {
                selectedSubject.Value = string.Format("].[{0}].[{1}]", RegionsNamingHelper.GetFoBySubject(subject), subject);
                chartColumnSet.Value = "[Measures].[Налоговая нагрузка на ед.акт.населения], [Measures].[Средняя налоговая нагрузка по ФО],[Measures].[Средняя налоговая нагрузка по РФ]";
            }
            subjectLabel.Text = subject;
            DynamicChart.DataBind();
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0011_grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территория", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            SetColumnParams(e.Layout, 0, 1, "N3", 105, false);
            SetColumnParams(e.Layout, 0, 2, "N0", 105, false);
            SetColumnParams(e.Layout, 0, 3, "N0", 105, true);
            SetColumnParams(e.Layout, 0, 4, "N0", 105, false);
            SetColumnParams(e.Layout, 0, 5, "N3", 105, false);
            SetColumnParams(e.Layout, 0, 6, "N3", 110, false);
            SetColumnParams(e.Layout, 0, 7, "N3", 110, false);
            SetColumnParams(e.Layout, 0, 8, "N3", 110, false);
            SetColumnParams(e.Layout, 0, 9, "N3", 110, false);

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i < 6)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = "в том числе по видам налогов";

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Налоговая нагрузка на ед.акт.населения, тыс.руб./чел.", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Ранг налоговой нагрузки на ед. экономически активного населения", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Численность экономически активного населения, чел.", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Налоговая нагрузка ВСЕГО, тыс.руб.", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "Налог на доходы физических лиц, тыс.руб.", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, "Единый социальный налог (страховые взносы с 2010г), тыс.руб.", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, "Налог на имущество физических лиц, тыс.руб.", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9, "Транспортный налог с физических лиц, тыс.руб.", "");

            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 6;
            ch.RowLayoutColumnInfo.SpanX = 4;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (ComboRegiones.SelectedIndex != 0)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }
        
        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rank = (i == 2);

                if (rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = "Самая низкая налоговая нагрузка на ед. экономически активного населения";
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = "Самая высокая налоговая нагрузка на ед. экономически активного населения";
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
        }

        #endregion

        #region Обработчики диаграммы

        private double maxFO;

        protected void AllocationChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0011_avg_allocation");
            dtAVG = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Среднее", dtAVG);

            query = DataProvider.GetQueryText("STAT_0001_0011_chart_allocation");
            dtAllocationChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территория", dtAllocationChart);
            AllocationChart.DataSource = dtAllocationChart;

            if (ComboRegiones.SelectedIndex != 0 && dtAllocationChart != null && dtAVG != null)
            {
                double avgRFValue = 0;
                if (dtAVG.Rows[0][0] != DBNull.Value &&
                    dtAVG.Rows[0][0].ToString() != string.Empty)
                {
                    avgRFValue = Convert.ToDouble(dtAVG.Rows[0][0]);
                }

                maxFO = 0;
                foreach (DataRow row in dtAllocationChart.Rows)
                {
                    if (row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                    {
                        double value = Convert.ToDouble(row[2]);
                        if (value > maxFO)
                        {
                            maxFO = value;
                        }
                    }
                }

                if (avgRFValue > maxFO)
                {
                    AllocationChart.Axis.Y.RangeType = AxisRangeType.Custom;
                    AllocationChart.Axis.Y.RangeMax = avgRFValue;

                    //Response.Write(maxFO.ToString());
                }
                else
                {
                    AllocationChart.Axis.Y.RangeType = AxisRangeType.Automatic;
                }
            }
        }

        protected void DynamicChart_DataBinding(object sender, EventArgs e)
        {
            dtDynamicChart = new DataTable();

            string query = DataProvider.GetQueryText("STAT_0001_0011_chart_dynamic");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территория", dtDynamicChart);

            if (dtDynamicChart.Rows.Count > 0)
            {
                string currentYear = string.Empty;

                foreach (DataColumn column in dtDynamicChart.Columns)
                {
                    string subjectName = RegionsNamingHelper.GetFoBySubject(subjectLabel.Text);
                    if (subjectName == string.Empty)
                    {
                        subjectName = subjectLabel.Text;
                    }
                    column.ColumnName = column.ColumnName.Replace("ФО", RegionsNamingHelper.ShortName(subjectName));
                    column.ColumnName = column.ColumnName.Replace("Налоговая нагрузка на ед.акт.населения", "Налоговая нагрузка");
                }

                foreach (DataRow row in dtDynamicChart.Rows)
                {
                    string year = row[1].ToString();
                    if (currentYear != year)
                    {
                        currentYear = year;
                        row[0] = string.Format("{0} - {1}", currentYear, row[0]);
                    }
                }

                dtDynamicChart.Columns.RemoveAt(1);
                DynamicChart.DataSource = dtDynamicChart;
            }
        }

        void AllocationChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int textWidht = 200;
            int textHeight = 12;
            int lineStart = (int)xAxis.MapMinimum;
            int lineLength = (int)xAxis.MapMaximum;

            if (dtAVG != null && dtGrid.Rows.Count > 0)
            {
                double avgRFValue = 0;
                if (dtAVG.Rows[0][0] != DBNull.Value &&
                    dtAVG.Rows[0][0].ToString() != string.Empty)
                {
                    avgRFValue = Convert.ToDouble(dtAVG.Rows[0][0]);
                }

                Line line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                line.PE.Stroke = Color.DarkGray;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point(lineStart, (int)yAxis.Map(avgRFValue));
                line.p2 = new Point(lineStart + lineLength, (int)yAxis.Map(avgRFValue));
                e.SceneGraph.Add(line);

                Text text = new Text();
                text.PE.Fill = Color.Black;
                text.bounds = new Rectangle(lineLength - textWidht, ((int)yAxis.Map(avgRFValue)) - textHeight, textWidht, textHeight);
                string regionStr = "РФ";
                text.SetTextString(string.Format("Среднее по {1}: {0:N3} тыс.руб./чел.", avgRFValue, regionStr));
                e.SceneGraph.Add(text);

                if (ComboRegiones.SelectedIndex != 0)
                {
                    double avgFOValue = 0;
                    if (dtAVG.Rows[0][1] != DBNull.Value &&
                        dtAVG.Rows[0][1].ToString() != string.Empty)
                    {
                        avgFOValue = Convert.ToDouble(dtAVG.Rows[0][1]);
                    }

                    line = new Line();
                    line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                    line.PE.Stroke = Color.DarkGray;
                    line.PE.StrokeWidth = 2;
                    line.p1 = new Point(lineStart, (int)yAxis.Map(avgFOValue));
                    line.p2 = new Point(lineStart + lineLength, (int)yAxis.Map(avgFOValue));
                    e.SceneGraph.Add(line);

                    text = new Text();
                    text.PE.Fill = Color.Black;
                    text.bounds = new Rectangle(lineLength - textWidht, ((int)yAxis.Map(avgFOValue)) - textHeight, textWidht, textHeight);
                    regionStr = RegionsNamingHelper.ShortName(ComboRegiones.SelectedValue);
                    text.SetTextString(string.Format("Среднее по {1}: {0:N3} тыс.руб./чел.", avgFOValue, regionStr));
                    e.SceneGraph.Add(text);
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            e.CurrentWorksheet.Columns[0].Width = 250*37;
            e.CurrentWorksheet.Columns[1].Width = 110*37;
            e.CurrentWorksheet.Columns[2].Width = 110*37;
            e.CurrentWorksheet.Columns[3].Width = 110*37;
            e.CurrentWorksheet.Columns[4].Width = 110*37;
            e.CurrentWorksheet.Columns[5].Width = 110*37;
            e.CurrentWorksheet.Columns[6].Width = 110*37;
            e.CurrentWorksheet.Columns[7].Width = 110*37;
            e.CurrentWorksheet.Columns[8].Width = 110*37;
            e.CurrentWorksheet.Columns[9].Width = 110*37;

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0.000";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#0";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0";
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0.000";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "#,##0.000";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "#,##0.000";
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "#,##0.000";
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "#,##0.000";
            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "#,##0.000";

            // расставляем стили у ячеек хидера
            for (int i = 0; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 17 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Лист 1");

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            Report report = new Report();
            ReportSection section1 = new ReportSection(report);

            IText title = section1.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = section1.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);

            AllocationChart.Width = 1100;
            section1.AddImage(UltraGridExporter.GetImageFromChart(AllocationChart));
            section1.AddPageBreak();
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid, section1);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(90);
            }
        }

        private void PdfExporter_EndExport(object sender, EndExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(string.Format("{0} ({1})", dynamicChartCaption.Text, subjectLabel.Text));

            DynamicChart.Width = 1100;
            DynamicChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(DynamicChart);
            e.Section.AddImage(img);
        }
        
        #endregion

        private void FillComboRegions(bool fillCombo)
        {
            DataTable dtRegions = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0011_regions");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dtRegions);

            FODictionary = new Dictionary<string, string>();
            Dictionary<string, int> regions = new Dictionary<string, int>();
            foreach (DataRow row in dtRegions.Rows)
            {
                regions.Add(row[0].ToString(), 0);
                FODictionary.Add(row[0].ToString(), row[1].ToString());
            }
            if (fillCombo)
            {
                ComboRegiones.FillDictionaryValues(regions);
            }
        }
    }

    public class ReportSection : ISection
    {

        #region ISection Members

        private readonly ISection section;

        public ReportSection(Report report)
        {
            section = report.AddSection();
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
            return this.section.AddImage(image);
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
            section.AddPageBreak();
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
            return section.AddTable();
        }

        public IText AddText()
        {
            return section.AddText();
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
                this.section.PageSize = new PageSize(880, value.Height);
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
