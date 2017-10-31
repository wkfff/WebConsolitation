using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FK_0004_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable grbsGridDt = new DataTable();
        private DataTable restChartDt = new DataTable();
        private DataTable dynamicChartDt = new DataTable();
        private DataTable dtChartLimit = new DataTable();

        private DateTime currentDate;

        private double chartLimit;

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;
        // выбранный показатель таблицы
        private CustomParam selectedGridIndicator;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GRBSGridBrick.AutoSizeStyle = GridAutoSizeStyle.None;
            GRBSGridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.7 - 235);
            GRBSGridBrick.Width =  Convert.ToInt32(CustomReportConst.minScreenWidth * 0.7 - 15);
            GRBSGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(GRBSGrid_InitializeLayout);
            GRBSGridBrick.Grid.ActiveRowChange += new ActiveRowChangeEventHandler(GRBSGridBrick_ActiveRowChange);
            GRBSGridBrick.Grid.DataBound += new EventHandler(GRBSGridBrick_DataBound);

            #endregion

            #region Настройка диаграммы динамики

            DynamicChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.57 - 100);
            DynamicChartBrick.Chart.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(Chart_FillSceneGraph);

            DynamicChartBrick.YAxisLabelFormat = "N2";
            DynamicChartBrick.DataFormatString = "N2";
            DynamicChartBrick.DataItemCaption = "Млрд.руб.";
            DynamicChartBrick.Legend.Visible = false;
            DynamicChartBrick.Legend.Location = LegendLocation.Bottom;
            DynamicChartBrick.Legend.SpanPercentage = 10;
            DynamicChartBrick.ColorModel = ChartColorModel.DefaultFixedColors;
            DynamicChartBrick.XAxisExtent = 80;
            DynamicChartBrick.YAxisExtent = 60;
            DynamicChartBrick.ZeroAligned = true;
            DynamicChartBrick.SwapRowAndColumns = true;
            DynamicChartBrick.IconSize = SymbolIconSize.Medium;

            #endregion

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
            selectedGridIndicator = UserParams.CustomParam("selected_grid_indicator");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(DynamicChartBrick.Chart.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(DynamicChartCaption.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(GRBSGridBrick.Grid.ClientID);

                CustomCalendar1.WebCalendar.SelectedDate = CubeInfo.GetLastDate(DataProvidersFactory.SecondaryMASDataProvider, "FK_0004_0001_lastDate");

                selectedGridIndicator.Value = "[Показатели].[Остатки средств].[Все показатели].[1000_Остатки средств федерального бюджета на ЕКС]";
                hiddenIndicatorLabel.Text = "Сеть государственных и муниципальных учреждений и органов власти";
            }
            
            currentDate = CustomCalendar1.WebCalendar.SelectedDate;

            Page.Title = String.Format("Анализ остатков средств бюджета на счетах Казначейства России, Резервного фонда и Фонда национального благосостояния");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("по состоянию на {0:dd.MM.yyyy} г.", currentDate);

            selectedPeriod.Value = CRHelper.PeriodMemberUName("[Период].[Период]", currentDate, 5);
            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            GridDataBind();
            ChartLimitDataBind();
            RestChartDataBind();
        }

        #region Обработчики грида
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FK_0004_0001_grid");
            grbsGridDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", grbsGridDt);

            if (grbsGridDt.Rows.Count > 0)
            {
                if (grbsGridDt.Columns.Count > 0)
                {
                    grbsGridDt.Columns.RemoveAt(0);
                }

                FontRowLevelRule levelRule = new FontRowLevelRule(grbsGridDt.Columns.Count - 2);
                levelRule.AddFontLevel("1", GRBSGridBrick.BoldFont8pt);
                GRBSGridBrick.AddIndicatorRule(levelRule);

                GrowRateRule growRateRule = new GrowRateRule(3, "Прирост к началу года", "Снижение к началу года");
                GRBSGridBrick.AddIndicatorRule(growRateRule);

                GRBSGridBrick.DataTable = grbsGridDt;
            }
        }

        protected void GRBSGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(430);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;

            for (int i = 1; i < columnCount - 2; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GRBSGridBrick.GridHeaderLayout;

            headerLayout.AddCell("Показатели");

            headerLayout.AddCell(String.Format("На 01.01.{0}, млрд.руб.", currentDate.Year));
            headerLayout.AddCell(String.Format("На {0:dd.MM.yyyy}, млрд.руб.", currentDate));
            headerLayout.AddCell("Изменение с начала года, млрд.руб.");

            headerLayout.ApplyHeaderInfo();
        }

        private void GRBSGridBrick_DataBound(object sender, EventArgs e)
        {
            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                UltraGridRow row = CRHelper.FindGridRow(GRBSGridBrick.Grid, selectedGridIndicator.Value, GRBSGridBrick.Grid.Columns.Count - 1, 0);
                ActivateGridRow(row);
            }
        }

        private void GRBSGridBrick_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActivateGridRow(e.Row);
        }

        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
            {
                return;
            }

            string indicatorUniqueName = row.Cells[row.Cells.Count - 1].Text;
            string indicatorName = row.Cells[0].Text;

            hiddenIndicatorLabel.Text = indicatorUniqueName;
            selectedGridIndicator.Value = hiddenIndicatorLabel.Text;

            DynamicChartBrick.TooltipFormatString = String.Format("{0}\nфакт на <b><SERIES_LABEL></b> г.\n<b><DATA_VALUE_ITEM:N2></b> млрд.руб.", indicatorName);

            DynamicChartCaption.Text = String.Format("Динамика остатков ({0})", indicatorName);
            DynamicChartDataBind();
        }

        #endregion

        #region Обработчики диаграммы остатков

        private Dictionary<string, double> restDictionary;
        private Dictionary<string, string> restHintDictionary;

        private void RestChartDataBind()
        {
            string query = DataProvider.GetQueryText("FK_0004_0001_restChart");
            restChartDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", restChartDt);

            if (restChartDt.Rows.Count > 0)
            {
                if (restChartDt.Columns.Count > 0)
                {
                    restChartDt.Columns.RemoveAt(0);
                }

                restDictionary = new Dictionary<string, double>();
                restHintDictionary = new Dictionary<string, string>();

                double maxValue = Double.MinValue;
                for (int i = 0; i < restChartDt.Rows.Count; i++)
                {
                    DataRow row = restChartDt.Rows[i];

                    string name = String.Empty;
                    if (row[0] != DBNull.Value && row[0].ToString() != String.Empty)
                    {
                        name = row[0].ToString();
                    }

                    double value = Double.MinValue;
                    if (row[1] != DBNull.Value && row[1].ToString() != String.Empty)
                    {
                        value = Convert.ToDouble(row[1].ToString());
                    }

                    string hint = String.Empty;
                    if (row[2] != DBNull.Value && row[2].ToString() != String.Empty)
                    {
                        hint = row[2].ToString();
                    }
                    
                    if (name != String.Empty && value != Double.MinValue)
                    {
                        restDictionary.Add(name, value);
                        restHintDictionary.Add(name, hint);
                        maxValue = Math.Max(maxValue, value);
                    }
                }

                int restId = 1;
                foreach (string name in restDictionary.Keys)
                {
                    AddRestBox(restId.ToString(), name, restDictionary[name], maxValue, restHintDictionary[name]);
                    restId++;
                }
            }
        }

        private void AddRestBox(string id, string name, double value, double maxValue, string tooltip)
        {
            RestImageBox restBox = (RestImageBox)Page.LoadControl("../../Components/RestImageBox.ascx");
            restBox.Value = value;
            restBox.Name = name;
            restBox.RestId = id;
            restBox.MaxValue = maxValue;
            restBox.Tooltip = tooltip;

            HtmlTableCell restCell = new HtmlTableCell();
            restCell.VAlign = "bottom";
            restCell.Height = "100%";
            restCell.Controls.Add(restBox);
            RestsTr.Controls.Add(restCell);
        }

        #endregion

        #region Обработчики диаграммы динамики

        private void DynamicChartDataBind()
        {
            string query = DataProvider.GetQueryText("FK_0004_0001_dynamicChart");
            dynamicChartDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dynamicChartDt);
            if (dynamicChartDt.Rows.Count > 0)
            {
                ChartLimitDataBind();

                double minValue = 0;
                double maxValue = 0;

                foreach (DataRow row in dynamicChartDt.Rows)
                {
                    if (row["Выбранный показатель"] != DBNull.Value && row["Выбранный показатель"].ToString() != String.Empty)
                    {
                        double value = Convert.ToDouble(row["Выбранный показатель"]);
                        CRHelper.SaveToErrorLog(value.ToString());
                        minValue = Math.Min(value, minValue);
                        maxValue = Math.Max(value, maxValue);
                    }
                }


                maxValue = Math.Max(chartLimit, maxValue);
                minValue = Math.Min(chartLimit, minValue);

                if (maxValue == 0 && minValue == 0)
                {
                    DynamicChartBrick.Chart.Axis.Y.RangeType = AxisRangeType.Automatic; 
                }
                else
                {
                    DynamicChartBrick.Chart.Axis.Y.RangeType = AxisRangeType.Custom;
                    DynamicChartBrick.Chart.Axis.Y.RangeMax = maxValue;
                    DynamicChartBrick.Chart.Axis.Y.RangeMin = minValue;
                }
                
                DynamicChartBrick.DataTable = dynamicChartDt;
                DynamicChartBrick.DataBind();
            }
        }

        private void ChartLimitDataBind()
        {
            string query = DataProvider.GetQueryText("FK_0004_0001_dynamicChartLimit");
            dtChartLimit = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChartLimit);

            chartLimit = double.MinValue;
            if (dtChartLimit.Rows.Count > 0)
            {
                if (dtChartLimit.Rows[0][1] != DBNull.Value && dtChartLimit.Rows[0][1].ToString() != String.Empty)
                {
                    chartLimit = Convert.ToDouble(dtChartLimit.Rows[0][1]);
                }
            }
        }

        private void Chart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            if (chartLimit != Double.MinValue)
            {
                IAdvanceAxis xAxis = (IAdvanceAxis) e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis) e.Grid["Y"];

                if (xAxis == null || yAxis == null)
                    return;

//                DynamicChartBrick.Chart.Axis.Y.RangeType = AxisRangeType.Custom;
//                DynamicChartBrick.Chart.Axis.Y.RangeMax = Math.Max(chartLimit, (double)yAxis.Maximum) * 1.01;
//                DynamicChartBrick.Chart.Axis.Y.RangeMin = Math.Min(chartLimit, (double)yAxis.Minimum) * 1.01;
//
//                yAxis.Maximum = Math.Max(chartLimit, (double)yAxis.Maximum) * 1.01;
//                yAxis.Minimum = Math.Min(chartLimit, (double)yAxis.Minimum) * 1.01;

                int textWidth = 350;
                int textHeight = 16;
                int lineStart = (int) xAxis.MapMinimum;
                int lineLength = (int)xAxis.MapMaximum;

                Line line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                line.PE.Stroke = Color.Red;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point(lineStart, (int) yAxis.Map(chartLimit));
                line.p2 = new Point(lineStart + lineLength - 100, (int) yAxis.Map(chartLimit));
                e.SceneGraph.Add(line);

                Text text = new Text();
                text.PE.Fill = Color.Black;
                text.labelStyle.Font = new Font("Verdana", 10);
                text.bounds = new Rectangle(lineLength - textWidth, ((int)yAxis.Map(chartLimit)) - textHeight, textWidth, textHeight);
                text.SetTextString(string.Format("Факт на начало года: {0:N2} млрд.руб.", chartLimit));
                e.SceneGraph.Add(text);
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook(); 

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(GRBSGridBrick.GridHeaderLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            DynamicChartBrick.Chart.Width = Convert.ToInt32(DynamicChartBrick.Chart.Width.Value * 0.8);
            ReportExcelExporter1.Export(DynamicChartBrick.Chart, DynamicChartCaption.Text, sheet2, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GRBSGridBrick.GridHeaderLayout, section1);

            ISection section2 = report.AddSection();
            DynamicChartBrick.Chart.Width = Convert.ToInt32(DynamicChartBrick.Chart.Width.Value * 0.8);
            ReportPDFExporter1.Export(DynamicChartBrick.Chart, DynamicChartCaption.Text, section2);
        }

        #endregion
    }
}