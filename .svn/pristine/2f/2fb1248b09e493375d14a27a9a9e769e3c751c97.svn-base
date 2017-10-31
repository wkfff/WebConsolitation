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
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Server.Dashboards.reports.FK_0004_0004
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable grbsGridDt = new DataTable();
        private DataTable restChartDt = new DataTable();
        private DataTable dynamicChartDt = new DataTable();
        private DataTable dynamicChartDtNormalized = new DataTable();
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

        private bool IsMlnRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string multiplierCaption;

        // выбранный множитель рублей
        private CustomParam rubMultiplier;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            CrossLink1.Text = "Характеристика&nbsp;сектора&nbsp;сектора&nbsp;гос.&nbsp;управления";
            CrossLink1.NavigateUrl = "~/reports/FK_0004_0002/Default.aspx";

            CrossLink2.Text = "Доходы&nbsp;сектора&nbsp;гос.&nbsp;управления";
            CrossLink2.NavigateUrl = "~/reports/FK_0004_0003/Default.aspx";

            #region Настройка грида

            GRBSGridBrick.AutoSizeStyle = GridAutoSizeStyle.None;
            GRBSGridBrick.Height = Unit.Empty;
            GRBSGridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            GRBSGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(GRBSGrid_InitializeLayout);
            GRBSGridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            #region Настройка диаграммы динамики

            multiplierCaption = IsMlnRubSelected ? "млн.руб." : "млрд.руб.";


            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
            selectedGridIndicator = UserParams.CustomParam("selected_grid_indicator");

            #endregion
            rubMultiplier = UserParams.CustomParam("rub_multiplier");

            rubMultiplier.Value = IsMlnRubSelected ? "1000000" : "1000000000";

            ChartControl.Width = CRHelper.GetChartWidth(Convert.ToInt32(CustomReportConst.minScreenWidth - 45));
            ChartControl.Height = CRHelper.GetChartWidth(Convert.ToInt32(CustomReportConst.minScreenHeight * 0.8));
            ChartControl.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(ChartControl_FillSceneGraph);
            ChartControl.Border.Color = Color.White;

            ChartControl.ChartType = ChartType.StackColumnChart;

            ChartControl.Legend.Visible = true;
            ChartControl.Legend.Location = LegendLocation.Top;
            ChartControl.Legend.SpanPercentage = 17;
            ChartControl.Legend.Font = new Font("Verdana", 10);

            ChartControl.Axis.Y.Extent = 100;
            ChartControl.Data.SwapRowsAndColumns = false;
            ChartControl.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            ChartControl.Axis.Y.Labels.Visible = true;            
            ChartControl.Axis.Y.Labels.Font = new Font("Verdana", 8);
            ChartControl.Axis.Y.Labels.Orientation = TextOrientation.Horizontal;
            if (normalize.Checked)
            {
                ChartControl.Axis.Y.TickmarkStyle = AxisTickStyle.Percentage;
                ChartControl.Axis.Y.TickmarkPercentage = 10;
            }
            ChartControl.Axis.Y.Labels.ItemFormatString = normalize.Checked ? "<DATA_VALUE:N0>%" : "<DATA_VALUE:N0>";

            ChartControl.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;
            WrapTextAxisLabelLayoutBehavior wrapBehavior = new WrapTextAxisLabelLayoutBehavior();
            wrapBehavior.EnableRollback = false;
            ChartControl.Axis.X.Labels.SeriesLabels.Layout.BehaviorCollection.Add(wrapBehavior);

            ChartControl.TitleLeft.Visible = !normalize.Checked;
            ChartControl.TitleLeft.HorizontalAlign = StringAlignment.Center;
            ChartControl.TitleLeft.VerticalAlign = StringAlignment.Center;
            ChartControl.TitleLeft.Text = multiplierCaption;
            ChartControl.TitleLeft.Extent = 20;
            ChartControl.TitleLeft.Font = new Font("Verdana", 10);
            ChartControl.Tooltips.FormatString = String.Format("<SERIES_LABEL><BR/><ITEM_LABEL>");

            ChartControl.ColorModel.ModelStyle = ColorModels.CustomSkin;
            ChartControl.ColorModel.Skin.ApplyRowWise = false;
            ChartControl.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 20; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetColor(i);

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                ChartControl.ColorModel.Skin.PEs.Add(pe);
            }

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.Olive;
                    }
                case 2:
                    {
                        return Color.SeaGreen;
                    }
                case 3:
                    {
                        return Color.PaleTurquoise;
                    }
                case 4:
                    {
                        return Color.DarkTurquoise;
                    }
                case 5:
                    {
                        return Color.LawnGreen;
                    }
                case 6:
                    {
                        return Color.Moccasin;
                    }
                case 7:
                    {
                        return Color.RoyalBlue;
                    }
                case 8:
                    {
                        return Color.LightSalmon;
                    }
                case 9:
                    {
                        return Color.Aquamarine;
                    }
                case 11:
                    {
                        return Color.SlateBlue;
                    }
                case 12:
                    {
                        return Color.Crimson;
                    }
                case 13:
                    {
                        return Color.Thistle;
                    }
                case 14:
                    {
                        return Color.RoyalBlue;
                    }
                case 15:
                    {
                        return Color.SandyBrown;
                    }
                case 16:
                    {
                        return Color.Honeydew;
                    }
                default:
                    {
                        return Color.PowderBlue;
                    }
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(ChartControl);
                chartWebAsyncPanel.AddLinkedRequestTrigger(normalize.ClientID);
                CustomCalendar1.WebCalendar.SelectedDate = CubeInfo.GetLastDate(DataProvidersFactory.SecondaryMASDataProvider, "FK_0004_0004_lastDate");
            }

            currentDate = CustomCalendar1.WebCalendar.SelectedDate;

            Page.Title = String.Format("Расходы сектора государственного управления ");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("по состоянию на <b>{0:dd.MM.yyyy} г., {1}</b>", currentDate.AddDays(1), multiplierCaption);

            DynamicChartCaption.Text = String.Format("Структура расходов сектора государственного управления на {0:dd.MM.yyyy} г., {1}</b>", currentDate.AddDays(1), multiplierCaption);

            selectedPeriod.Value = CRHelper.PeriodMemberUName("[Период].[Период]", currentDate, 5);
            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            GridDataBind();
            DynamicChartDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FK_0004_0004_grid");
            grbsGridDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", grbsGridDt);

            if (grbsGridDt.Rows.Count > 0)
            {
                if (grbsGridDt.Columns.Count > 0)
                {
                    grbsGridDt.Columns.RemoveAt(0);
                }

                FontRowLevelRule levelRule = new FontRowLevelRule(grbsGridDt.Columns.Count - 1);
                levelRule.AddFontLevel("1", GRBSGridBrick.BoldFont10pt);
                GRBSGridBrick.AddIndicatorRule(levelRule);

                GRBSGridBrick.DataTable = grbsGridDt;
            }
        }

        void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[e.Row.Cells.Count - 1].Value != null &&
                e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString() == "3")
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
        }

        protected void GRBSGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(230);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 1; i < columnCount - 1; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
            }

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(145);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(145);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(145);
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(145);
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(145);
            e.Layout.Bands[0].Columns[6].Width = CRHelper.GetColumnWidth(145);
            

            GridHeaderLayout headerLayout = GRBSGridBrick.GridHeaderLayout;

            headerLayout.AddCell("Показатели");

            headerLayout.AddCell("Консолидированный бюджет РФ и государственных внебюджетных фондов", "Консолидированный бюджет РФ и государственных внебюджетных фондов");
            GridHeaderCell cell = headerLayout.AddCell("Федеральный уровень бюджетной системы", "Федеральный уровень бюджетной системы");
            cell.AddCell("Федеральный бюджет", "Федеральный бюджет");
            cell.AddCell("Бюджеты государственных внебюджетных фондов", "Бюджеты государственных внебюджетных фондов");
            cell = headerLayout.AddCell("Региональный уровень бюджетной системы", "Региональный уровень бюджетной системы");
            cell.AddCell("Бюджеты субъектов РФ", "Бюджеты субъектов РФ");
            cell.AddCell("Бюджеты территориальных государственных внебюджетных фондов (бюджеты территориальных фондов ОМС)", "Бюджеты территориальных государственных внебюджетных фондов (бюджеты территориальных фондов ОМС)");
            headerLayout.AddCell("Местные бюджеты", "Местные бюджеты");

            headerLayout.ApplyHeaderInfo();
        }

        #endregion

        #region Обработчики диаграммы динамики

        private void DynamicChartDataBind()
        {
            dynamicChartDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("FK_0004_0004_dynamicChart"), "Наименование показателей", dynamicChartDt);
            dynamicChartDtNormalized = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("FK_0004_0004_dynamicChart_normalized"), "Наименование показателей", dynamicChartDtNormalized);

            dynamicChartDt.Columns.RemoveAt(0);
            dynamicChartDtNormalized.Columns.RemoveAt(0);

            ChartControl.Series.Clear();

            if (normalize.Checked)
            {
                ChartControl.StackChart.StackStyle = StackStyle.Complete;
                for (int i = 1; i < dynamicChartDtNormalized.Columns.Count; i++)
                {
                    ChartControl.Series.Add(CRHelper.GetNumericSeries(i, dynamicChartDtNormalized));
                }
            }
            else
            {
                ChartControl.StackChart.StackStyle = StackStyle.Normal;
                for (int i = 1; i < dynamicChartDt.Columns.Count; i++)
                {
                    ChartControl.Series.Add(CRHelper.GetNumericSeries(i, dynamicChartDt));
                }
            }

            ChartControl.DataBind();
        }

        protected void ChartControl_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    if (text.Row == -1)
                    {
                        text.bounds.Height = 40;

                        text.bounds = new Rectangle(text.bounds.Left, text.bounds.Y, text.bounds.Width, text.bounds.Height);

                        text.labelStyle.HorizontalAlign = StringAlignment.Center;

                        text.labelStyle.VerticalAlign = StringAlignment.Near;
                        text.labelStyle.FontSizeBestFit = false;
                        text.labelStyle.Font = new Font("Verdana", 8);
                        text.labelStyle.WrapText = true;
                    }
                }
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Border.Title.Left"))
                {
                    Text text = (Text)primitive;
                    text.bounds.Width = 20;
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {

                        if (box.DataPoint.Label.Contains("Дефицит"))
                        {

                            double value = Convert.ToDouble(box.Value);
                            if (value > 0)
                            {
                                box.DataPoint.Label = box.DataPoint.Label.Replace("Дефицит/Профицит", "Профицит");
                                box.PE.ElementType = PaintElementType.Gradient;
                                box.PE.FillGradientStyle = GradientStyle.Horizontal;
                                box.PE.Fill = Color.Green;
                                box.PE.FillStopColor = Color.ForestGreen;
                            }
                            else
                            {
                                box.DataPoint.Label = box.DataPoint.Label.Replace("Дефицит/Профицит", "Дефицит");
                                box.PE.ElementType = PaintElementType.Gradient;
                                box.PE.FillGradientStyle = GradientStyle.Horizontal;
                                box.PE.Fill = Color.Red;
                                box.PE.FillStopColor = Color.Maroon;
                            }
                        }
                        box.DataPoint.Label = normalize.Checked ?
                            String.Format("{0}<br/><b>{1:P2}</b><br><b>{2:N2}</b> {3}", box.DataPoint.Label, dynamicChartDtNormalized.Rows[box.Column][box.Row + 1], dynamicChartDt.Rows[box.Column][box.Row + 1], multiplierCaption) :
                            String.Format("{0}<br/><b>{1:N2}</b> {2}", box.DataPoint.Label, dynamicChartDt.Rows[box.Column][box.Row + 1], multiplierCaption);
                    }
                }
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
            ChartControl.Width = Convert.ToInt32(ChartControl.Width.Value * 0.8);
            ReportExcelExporter1.Export(ChartControl, DynamicChartCaption.Text, sheet2, 3);
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
            ChartControl.Width = Convert.ToInt32(ChartControl.Width.Value * 0.8);
            ReportPDFExporter1.Export(ChartControl, DynamicChartCaption.Text, section2);
        }

        #endregion
    }
}