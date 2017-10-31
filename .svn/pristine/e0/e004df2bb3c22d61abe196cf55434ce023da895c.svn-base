using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color=System.Drawing.Color;
using Font=System.Drawing.Font;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0017
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtCommentText = new DataTable();
        private int firstYear = 2008;
        private int endYear = 2011;
        private string selectedPeriodStr;

        #endregion

        #region Параметры запроса

        // множество выбранных лет
        private CustomParam yearSet;
        // выбранный период
        private CustomParam selectedPeriod;
        // выбранный год
        private CustomParam selectedYear;
        // выбирать факт на конец года
        private CustomParam endYearFactSelected;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.6 - 250);
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.365 - 110);
            UltraChart1.InvalidDataReceived +=new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.30 - 110);
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            UltraChart3.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart3.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.30 - 110);
            UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart3.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            UltraChart4.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart4.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.38 - 110);
            UltraChart4.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart4.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            
            #region Инициализация параметров запроса

            if (yearSet == null)
            {
                yearSet = UserParams.CustomParam("year_set");
            }
            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }
            if (selectedYear == null)
            {
                selectedYear = UserParams.CustomParam("selected_year");
            }
            if (endYearFactSelected == null)
            {
                endYearFactSelected = UserParams.CustomParam("end_year_fact_selected");
            }

            #endregion

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Visible = false;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.FKRAllLevel.Value = RegionSettingsHelper.Instance.FKRAllLevel;
            UserParams.FKRSectionLevel.Value = RegionSettingsHelper.Instance.FKRSectionLevel;
            UserParams.FKRSubSectionLevel.Value = RegionSettingsHelper.Instance.FKRSubSectionLevel;
            UserParams.EKRDimension.Value = RegionSettingsHelper.Instance.EKRDimension;

            if (!Page.IsPostBack)
            {
                commentTextWebAsyncPanel.AddRefreshTarget(DeficitLimitText);
                commentTextWebAsyncPanel.AddRefreshTarget(DebtLimitText);
                commentTextWebAsyncPanel.AddRefreshTarget(DebtOutcomesLimitText);
                commentTextWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0017_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Width = 150;
                ComboYear.Title = "Годы";
                ComboYear.MultiSelect = true;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
                ComboYear.SetСheckedState((endYear - 1).ToString(), true);
                ComboYear.SetСheckedState((endYear - 2).ToString(), true);

                PeriodLabel.Text = string.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}]", endYear);
            }

            Page.Title = string.Format("Исполнение областного бюджета Новосибирской области (долг, дефицит субъекта)");
            PageTitle.Text = Page.Title;

            if (!commentTextWebAsyncPanel.IsAsyncPostBack)
            {
                Collection<string> selectedValues = ComboYear.SelectedValues;
                if (selectedValues.Count > 0)
                {

                    PageSubTitle.Text = string.Format("за {0} {1}, руб.",
                        CRHelper.GetDigitIntervals(ComboYear.SelectedValuesString, ','),
                        ComboYear.SelectedValues.Count == 1 ? "год" : "годы");

                    string yearSetStr = string.Empty;
                    for (int i = 0; i < selectedValues.Count; i++)
                    {
                        string year = selectedValues[i];
                        yearSetStr += string.Format("[Период].[Период].[Данные всех периодов].[{0}],", year);
                    }

                    yearSetStr = yearSetStr.TrimEnd(',');
                    yearSet.Value = yearSetStr;
                }
                else
                {
                    PageSubTitle.Text = string.Empty;
                    yearSet.Value = " ";
                }
                
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();

                selectedYear.Value = "2008";
                endYearFactSelected.Value = "true";
                ChartSetup(UltraChart1);
                UltraChart1.DataBind();
                
                selectedYear.Value = "2009";
                endYearFactSelected.Value = "true";
                ChartSetup(UltraChart2);
                UltraChart2.DataBind();

                selectedYear.Value = "2010";
                endYearFactSelected.Value = "true";
                ChartSetup(UltraChart3);
                UltraChart3.DataBind();

                selectedYear.Value = "2010";
                endYearFactSelected.Value = "false";
                ChartSetup(UltraChart4);
                UltraChart4.DataBind();

                SetAxisSettings(UltraChart1);
                SetAxisSettings(UltraChart2);
                SetAxisSettings(UltraChart3);
                SetAxisSettings(UltraChart4);

                string patternValue = PeriodLabel.Text;
                int defaultRowIndex = UltraWebGrid.Rows.Count - 1;
                if (patternValue == string.Empty)
                {
                    defaultRowIndex = UltraWebGrid.Rows.Count - 1;
                }

                if (UltraWebGrid.Columns.Count > 0 && UltraWebGrid.Rows.Count > 0)
                {
                    // ищем строку
                    UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, UltraWebGrid.Columns.Count - 1, defaultRowIndex);
                    // выделяем строку
                    ActiveGridRow(row);
                }
            }

            CommentTextDataBind();
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

            string periodName = row.Cells[0].Text;
            PeriodLabel.Text = row.Cells[row.Cells.Count - 1].Text;

            DateTime selectedDate;
//            decimal value;
//            if (decimal.TryParse(periodName, out value))
//            {
//                PeriodLabel.Text = string.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие 2].[Квартал 4].[Декабрь]", periodName);
//            }

            selectedDate = CRHelper.PeriodDayFoDate(PeriodLabel.Text);
            selectedPeriod.Value = PeriodLabel.Text;

            selectedDate = selectedDate.AddMonths(1);
            commentTextCaption.Text = string.Format("На 1&nbsp;{0}&nbsp;{1}&nbsp;года", CRHelper.RusMonthGenitive(selectedDate.Month), selectedDate.Year);
            CommentTextDataBind();
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0017_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);

            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N2";
                bool limitEvennessColumn = (i == 1 || i == 3 || i == 7);
                int widthColumn = limitEvennessColumn ? 105 : 105;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;

                if (i >= e.Layout.Bands[0].Columns.Count - 4)
                {
                    e.Layout.Bands[0].Columns[i].Hidden = true;
                }
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(70);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 4; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 4; i = i + 2)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0];

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Годовые назначения", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Факт", "");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count - 4; i++)
            {
                e.Row.Cells[i].Style.Padding.Right = 2;
                string periodName = e.Row.Cells[0].Value.ToString();

                bool decemberRow = periodName.ToLower().Contains("декабрь");
                bool periodColumn = (i == 0);
                bool limitEvennessColumn = (decemberRow && (i == 2 || i == 4 || i == 8) ||
                                            !decemberRow && (i == 1 || i == 3 || i == 7));

                if (periodColumn && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    string period = e.Row.Cells[i].Value.ToString();
                    decimal value;
                    if (decimal.TryParse(period, out value))
                    {
                        e.Row.Cells[i].Style.Font.Bold = true;
                        e.Row.Cells[i].ColSpan = e.Row.Cells.Count - 4;

                        for (int j = 1; j < e.Row.Cells.Count - 1; j ++)
                        {
                            e.Row.Cells[j].Value = string.Empty;
                        }
                    }
                    else
                    {
                        e.Row.Cells[i].Style.Padding.Left = 15;
                    }
                }

                if (limitEvennessColumn && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    int limitColumnIndex = 0;
                    switch(i)
                    {
                        case 1:
                        case 2:
                             {
                                 limitColumnIndex = e.Row.Cells.Count - 4;
                                 break;
                             }
                        case 3:
                        case 4:
                             {
                                 limitColumnIndex = e.Row.Cells.Count - 3;
                                 break;
                             }
                        case 7:
                        case 8:
                             {
                                 limitColumnIndex = e.Row.Cells.Count - 2;
                                 break;
                             }
                    }

                    if (e.Row.Cells[limitColumnIndex].Value != null && e.Row.Cells[limitColumnIndex].Value.ToString() == "нарушение")
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                        e.Row.Cells[i].Title = string.Format("Нарушение");
                    }
                    else
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                        e.Row.Cells[i].Title = string.Format("Нет нарушения");
                    }
                                        
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                UltraGridCell cell = e.Row.Cells[i];
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
        
        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Height = Unit.Empty;
        }

        void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        #endregion

        #region Обработчики диаграммы

        private void ChartSetup(UltraChart chart)
        {
            if (chart == UltraChart1)
            {
                chart.Legend.Visible = true;
                chart.Legend.Location = LegendLocation.Top;
                chart.Legend.SpanPercentage = 28;
                chart.Legend.Margins.Right = Convert.ToInt32(chart.Width.Value) / 4;
                chart.Legend.Font = new Font("Verdana", 9);

                chart.TitleLeft.Margins.Top = Convert.ToInt32(chart.Height.Value * chart.Legend.SpanPercentage) / 100;
            }
            else
            {
                chart.Legend.Visible = false;
                
            }

            chart.ChartType = ChartType.StackBarChart;
            chart.Border.Thickness = 0;
            chart.ColumnChart.SeriesSpacing = 2;

            chart.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE_ITEM:N3> млн.руб.";

            chart.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            chart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            chart.Axis.Y.Labels.SeriesLabels.VerticalAlign = StringAlignment.Center;
            chart.Axis.Y.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            chart.Axis.Y.Labels.SeriesLabels.WrapText = true;

            if (chart == UltraChart4)
            {
                chart.Axis.X.Visible = true;
                chart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                chart.Axis.X.Labels.Font = new Font("Verdana", 8);
                chart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
                chart.Axis.X.Extent = 45;

                chart.TitleBottom.Text = "Млн. руб.";
                chart.TitleBottom.HorizontalAlign = StringAlignment.Center;
                chart.TitleBottom.Extent = 30;
                chart.TitleBottom.Margins.Left = chart.Axis.Y.Extent;
                chart.TitleBottom.Font = new Font("Verdana", 8);
                chart.TitleBottom.Visible = true;

                chart.TitleLeft.Margins.Bottom = chart.Axis.X.Extent;

                chart.Axis.Y.Extent = 105;
            }
            else
            {
                chart.Axis.Y.Extent = 110;
                chart.Axis.X.Visible = false;
            }

            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            chart.TitleLeft.Extent = 30;
            chart.TitleLeft.Visible = true;
            chart.TitleLeft.Font = new Font("Verdana", 9);
            
            //chart.Data.SwapRowsAndColumns = true;

            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            Color color1 = Color.LightBlue;
            Color color2 = Color.RoyalBlue;
            Color color3 = Color.Gold;
            Color color4 = Color.Red;
            Color color5 = Color.DarkViolet;
            Color color6 = Color.Goldenrod;
            Color color7 = Color.Silver;

            chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color3, 150));
            chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color4, 150));
            chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color5, 150));
            chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color6, 150));
            chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color7, 150));
            chart.ColorModel.Skin.ApplyRowWise = false;

            chart.Effects.Effects.Clear();
            GradientEffect effect = new GradientEffect();
            effect.Style = GradientStyle.ForwardDiagonal;
            effect.Coloring = GradientColoringStyle.Darken;
            effect.Enabled = true;
            chart.Effects.Enabled = true;
            chart.Effects.Effects.Add(effect);  
        }

        private void SetAxisSettings(UltraChart chart)
        {
            if (axisXMax != double.MinValue && axisXMin != double.MaxValue)
            {
                chart.Axis.X.RangeType = AxisRangeType.Custom;
                chart.Axis.X.RangeMin = axisXMin - axisXMin / 10;
                chart.Axis.X.RangeMax = axisXMax + axisXMax / 10;
            }
            else
            {
                chart.Axis.X.RangeType = AxisRangeType.Automatic;
            }
        }

        private double axisXMin = double.MaxValue;
        private double axisXMax = double.MinValue;

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0017_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                if (dtChart.Rows[0].ItemArray[dtChart.Columns.Count - 1] != DBNull.Value)
                {
                    string measure = endYearFactSelected.Value == "true" ? dtChart.Rows[0].ItemArray[dtChart.Columns.Count - 1].ToString() : "факт";
                    ((UltraChart)sender).TitleLeft.Text = string.Format("{0} ({1})", selectedYear.Value, measure);
                }
                dtChart.Columns.Remove("План/факт ");

                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    double minSum = 0;
                    double maxSum = 0;

                    foreach (DataRow row in dtChart.Rows)
                    {
                        if (row[i] != DBNull.Value && row[i].ToString() != string.Empty)
                        {
                            double value = Convert.ToDouble(row[i]);
                            if (value < 0)
                            {
                                minSum += value;
                            }
                            else
                            {
                                maxSum += value;
                            }
                        }
                    }
                    
                    if (minSum < axisXMin)
                    {
                        axisXMin = minSum;
                    }
                    if (maxSum > axisXMax)
                    {
                        axisXMax = maxSum;
                    }
                }

                ((UltraChart)sender).Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    string columnName = dtChart.Columns[i].ColumnName;

                    columnName = columnName.Replace("Дефицит ", "Дефицит(-)/ профицит(+)");
                    columnName = columnName.Replace("Ист.фин. ", "Источники финансирования");

                    series.Label = columnName;
                    ((UltraChart)sender).Series.Add(series);
                }

                //((UltraChart) sender).DataSource = dtChart;
            }
        }

        private void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.Y"))
                {
                    Text text = (Text)primitive;
                    text.bounds.Height = 30;
                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                    text.labelStyle.FontSizeBestFit = false;
                    text.labelStyle.Font = new Font("Verdana", 8);
                    text.labelStyle.WrapText = true;
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;

                    if (box.DataPoint != null && box.Value != null)
                    {
                        double value = Convert.ToDouble(box.Value);
                        if (box.DataPoint.Label != null && box.DataPoint.Label.ToLower().Contains("дефицит"))
                        {
                            if (value > 0)
                            {
                                box.DataPoint.Label = "Профицит";
                                box.PE.ElementType = PaintElementType.Gradient;
                                box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                                box.PE.Fill = Color.LimeGreen;
                                box.PE.FillStopColor = Color.ForestGreen;
                            }
                            else
                            {
                                box.DataPoint.Label = "Дефицит";
                                box.PE.ElementType = PaintElementType.Gradient;
                                box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;
                                box.PE.Fill = Color.Red;
                                box.PE.FillStopColor = Color.Maroon;
                            }
                        }
                        else if (box.DataPoint.Label != null && box.DataPoint.Label.ToLower().Contains("изменение"))
                        {
                            if (value < 0)
                            {
                                box.DataPoint.Label = "Увеличение остатков средств";
                            }
                            else
                            {
                                box.DataPoint.Label = "Снижение остатков средств";
                            }
                        }
                    }
                    else if (i != 0 && box.Path != null && box.Path.ToLower().Contains("legend") && box.rect.Width < 20)
                    {
                        if (box.Path == "Legend")
                        {
                            Primitive lastPrimitive = e.SceneGraph[i - 1];
                            if (lastPrimitive is Text)
                            {
                                Text text = (Text)lastPrimitive;
                                if (text.GetTextString().ToLower().Contains("дефицит"))
                                {
                                    box.PE.ElementType = PaintElementType.CustomBrush;
                                    System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(box.rect, Color.Red, Color.Green, 45, false);
                                    box.PE.CustomBrush = brush;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Комментарии к гриду

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        private void CommentTextDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0017_commentText");
            dtCommentText = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtCommentText);

            DeficitLimitText.Text = string.Empty;
            DebtLimitText.Text = string.Empty;
            DebtOutcomesLimitText.Text = string.Empty;

            if (dtCommentText.Rows.Count > 0)
            {
                double deficitLimit = GetDoubleDTValue(dtCommentText, "Предельный дефицит");
                double deficitLimitPercent = GetDoubleDTValue(dtCommentText, "Предельный дефицит_доля полож");
                string deficitLimitImg = deficitLimitPercent > 0.15
                                             ? "<img src=\"../../images/ballRedBB.png\" width=\"15px\" height=\"15px\">"
                                             : "<img src=\"../../images/ballGreenBB.png\" width=\"15px\" height=\"15px\">";
                double debtLimit = GetDoubleDTValue(dtCommentText, "Предельный долг");
                double debtLimitPercent = GetDoubleDTValue(dtCommentText, "Предельный долг_доля");
                string debtLimitImg = debtLimitPercent > 1
                             ? "<img src=\"../../images/ballRedBB.png\" width=\"15px\" height=\"15px\">"
                             : "<img src=\"../../images/ballGreenBB.png\" width=\"15px\" height=\"15px\">";
                double debtOutcomesLimit = GetDoubleDTValue(dtCommentText, "Предельные расходы на долг");
                double debtOutcomesLimitPercent = GetDoubleDTValue(dtCommentText, "Предельные расходы на долг_доля");
                string debtOutcomesLimitImg = debtOutcomesLimitPercent > 0.15
                             ? "<img src=\"../../images/ballRedBB.png\" width=\"15px\" height=\"15px\">"
                             : "<img src=\"../../images/ballGreenBB.png\" width=\"15px\" height=\"15px\">";

                string deficitFormula = string.Format(@"
<i>Р=(A-B-C)/(D-E)</i> при <i>B</i><0 и <i>C</i><0, иначе <i>Р=A/(D-E)</i>,
где&nbsp;<i>A</i> – размер дефицита бюджета субъекта РФ без учета величины, определяемой как разница между полученными и погашенными бюджетными кредитами, предоставленными бюджету субъекта РФ другими бюджетами бюджетной системы РФ;
&nbsp;<i>B</i> – объем поступлений от продажи акций и иных форм участия в капитале, находящихся в собственности субъекта РФ;
&nbsp;<i>С</i> – величина снижения остатков средств на счетах по учету средств бюджета субъекта РФ;
&nbsp;<i>D</i> – объем доходов бюджета субъекта РФ;
&nbsp;<i>Е</i> – объем безвозмездных поступлений.");

                string debtFormula = string.Format(@"
<i>Р = А/(В-С)</i>,
где&nbsp;<i>А</i> - объем государственного долга субъекта РФ,
&nbsp;<i>В</i> - общий объем доходов бюджета субъекта РФ,
&nbsp;<i>С</i> - объем безвозмездных поступлений.
");

                string debtOutcomesFormula = string.Format(@"
<i>Р = А/(В-С)</i>,
где <i>А</i> - объем расходов бюджета субъекта РФ на обслуживание государственного долга субъекта РФ,
&nbsp;<i>В</i> - объем расходов бюджета субъекта РФ,
&nbsp;<i>С</i> - объем расходов, которые осуществлялись за счет субвенций, предоставляемых из бюджетов бюджетной системы.
");

                DeficitLimitText.Text = string.Format(
                    "<span style='padding-left:15px;'><b>Показатель:</b> Отношение дефицита бюджета субъекта РФ к общему годовому объему доходов бюджета субъекта РФ без учета объема безвозмездных поступлений (Бюджетный кодекс РФ ст. 92.1, Федеральный закон от 09.04.2009 №58-ФЗ).</span><br/>" +
                    "<span style='padding-left:15px;'><b>Нормативное значение:</b>&nbsp;{1:P2}&nbsp;({2:N2} руб.)</span><br/>" +
                    "<span style='padding-left:15px;'><b>Расчетное значение:</b>&nbsp;{3:P2}&nbsp;{4}</span><br/>" +
                    "<span style='padding-left:15px;'><b>Формула:</b>&nbsp;{0}</span><br/>",
                        deficitFormula, 0.15, deficitLimit, deficitLimitPercent, deficitLimitImg);

                DebtLimitText.Text = string.Format(
                    "<span style='padding-left:15px;'><b>Показатель:</b> Отношение объема государственного долга субъекта РФ к общему годовому объему доходов бюджета субъекта РФ без учета объема безвозмездных поступлений (Бюджетный кодекс РФ ст. 107, Федеральный закон от 09.04.2009 №58-ФЗ).</span><br/>" +
                    "<span style='padding-left:15px;'><b>Нормативное значение:</b>&nbsp;{1:P2}&nbsp;({2:N2} руб.)</span><br/>" +
                    "<span style='padding-left:15px;'><b>Расчетное значение:</b>&nbsp;{3:P2}&nbsp;{4}</span><br/>" +
                    "<span style='padding-left:15px;'><b>Формула:</b>&nbsp;{0}</span><br/>",
                        debtFormula, 1, debtLimit, debtLimitPercent, debtLimitImg);

                DebtOutcomesLimitText.Text = string.Format(
                    "<span style='padding-left:15px;'><b>Показатель:</b> Отношение доли расходов на обслуживание государственного долга субъекта РФ к объему расходов бюджета субъекта РФ, за исключением объема расходов, которые осуществляются за счет субвенций, предоставляемых из бюджетов бюджетной системы РФ (Бюджетный кодекс ст. 112).</span><br/>" +
                    "<span style='padding-left:15px;'><b>Нормативное значение:</b>&nbsp;{1:P2}&nbsp;({2:N2} руб.)</span><br/>" +
                    "<span style='padding-left:15px;'><b>Расчетное значение:</b>&nbsp;{3:P2}&nbsp;{4}</span><br/>" +
                    "<span style='padding-left:15px;'><b>Формула:</b>&nbsp;{0}</span><br/>",
                        debtOutcomesFormula, 0.15, debtOutcomesLimit, debtOutcomesLimitPercent, debtOutcomesLimitImg);
            }
        }

        #endregion

        #region Экспорт в Excel

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
            commentText = commentText.Replace("<img src=\"../../images/ballRedBB.png\" width=\"15px\" height=\"15px\">", "");
            commentText = commentText.Replace("<img src=\"../../images/ballGreenBB.png\" width=\"15px\" height=\"15px\">", "");

            return commentText;
        }

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 32 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[4].Height = 20 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            e.CurrentWorksheet.Columns[0].Width = 100 * 37;

            for (int i = 1; i < UltraWebGrid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0.00";
                int widthColumn = i < UltraWebGrid.Bands[0].Columns.Count - 2 ? 110 : 120;

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }

            //e.CurrentWorksheet.Rows[rowsCount + 6].Cells[0].Value = CommentTextExportsReplaces(DeficitLimitText.Text);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        #endregion
    }
}
