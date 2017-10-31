using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebNavigator;
using Krista.FM.Server.Dashboards.Components;
using System.Collections.Generic;
using Infragistics.UltraChart.Resources.Appearance;
using System.Drawing;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

namespace Krista.FM.Server.Dashboards.reports.FSGS_0001_0004
{
    public partial class Default : CustomReportPage
    {

        #region Поля

        private DataTable dtGrid;
        private DataTable dtChart;

        private static MemberAttributesDigest periodDigest;
        private static MemberAttributesDigest paramDigest;
        private static MemberAttributesDigest foodDigest;
        private static MemberAttributesDigest foDigest;

        private static int columnWidth = 200;

        #endregion

        #region Параметры запроса

        private CustomParam selectedPeriod;
        private CustomParam sepPeriod;
        private CustomParam selectedFood;
        private CustomParam totalParam;
        private CustomParam foodParam;
        private CustomParam foParam;

        #endregion

        private static int Resolution
        {
            get { return CRHelper.GetScreenWidth; }
        }

        private static int Height
        {
            get { return CRHelper.GetScreenHeight; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            // Установка размеров
            if (Resolution < 900)
            {
                UltraWebGrid.Width = Unit.Parse("770px");
                UltraChart.Width = Unit.Parse("730px");
            }
            else if (Resolution < 1200)
            {
                UltraWebGrid.Width = Unit.Parse("940px");
                UltraChart.Width = Unit.Parse("900px");
            }
            else
            {
                UltraWebGrid.Width = Unit.Parse("1250px");
                UltraChart.Width = Unit.Parse("1210px");
            }

            UltraWebGrid.RedNegativeColoring = false;
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.Grid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.Grid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            
            #endregion

            #region Настройка диаграммы

            //UltraChart.Width = UltraWebGrid.Width;
            UltraChart.Height = CRHelper.GetChartHeight(400);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.DataBinding += new EventHandler(UltraChart_DataBinding);

            #endregion

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
            sepPeriod = UserParams.CustomParam("sep_period");
            selectedFood = UserParams.CustomParam("selected_food");
            foodParam = UserParams.CustomParam("food");
            totalParam = UserParams.CustomParam("total");
            foParam = UserParams.CustomParam("fo");

            #endregion

            #region Экспорт

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            #endregion
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            if (!Page.IsPostBack)
            {
                ComboPeriod.Title = "Период";
                ComboPeriod.ParentSelect = false;
                ComboPeriod.MultiSelect = false;
                ComboPeriod.Width = 220;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.SecondaryMASDataProvider, "FSGS_0001_0004_date");
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                ComboPeriod.SelectLastNode();

                ComboParam.Title = "Период для сравнения";
                ComboParam.MultiSelect = false;
                ComboParam.Width = 350;
                paramDigest = new MemberAttributesDigest(DataProvidersFactory.SecondaryMASDataProvider, "FSGS_0001_0004_param");
                ComboParam.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(paramDigest.UniqueNames, paramDigest.MemberLevels));
                ComboParam.SetСheckedState("к предыдущему периоду", true);

                ComboFood.Title = "Товар";
                ComboFood.MultiSelect = false;
                ComboFood.Width = 350;
                foodDigest = new MemberAttributesDigest(DataProvidersFactory.SecondaryMASDataProvider, "FSGS_0001_0004_food");
                ComboFood.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(foodDigest.UniqueNames, foodDigest.MemberLevels));
                ComboFood.SetСheckedState(GetSelectedFood(), true);

                ComboFO.Title = "Территория";
                ComboFO.MultiSelect = false;
                ComboFO.Width = 500;
                foDigest = new MemberAttributesDigest(DataProvidersFactory.SecondaryMASDataProvider, "FSGS_0001_0004_fo");
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(foDigest.UniqueNames, foDigest.MemberLevels));
                ComboFO.SetСheckedState("Уральский федеральный округ", true);
            }

            selectedPeriod.Value = periodDigest.GetMemberUniqueName(ComboPeriod.SelectedValue);
            sepPeriod.Value = selectedPeriod.Value.Replace("[Период__Период].[Период__Период].", "[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].");
            selectedFood.Value = foodDigest.GetMemberUniqueName(ComboFood.SelectedValue);
            foParam.Value = foDigest.GetMemberUniqueName(ComboFO.SelectedValue);
            totalParam.Value = paramDigest.GetMemberUniqueName(ComboParam.SelectedValue).Split(';')[0];
            foodParam.Value = paramDigest.GetMemberUniqueName(ComboParam.SelectedValue).Split(';')[1];

            Page.Title = String.Format("Темпы роста цен на продовольственные товары в разрезе территорий РФ");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format(
                "Ежемесячная информация о темпах роста цен <b>{0}</b> на социально значимые продовольственные товары в субъектах РФ и Российской Федерации, {1}, по состоянию на {2}",
                ComboParam.SelectedValue, ComboFO.SelectedValue, ComboPeriod.SelectedValue.ToLowerFirstSymbol());

            GridHeader.Text = String.Format("Темпы роста цен на товар «{0}»", ComboFood.SelectedValue);
            GridDataBind();

            ChartHeader.Text = String.Format(
                "Распределение территорий, входящих в {0} по темпу роста цен {1} на товар «{2}», по состоянию на {3}",
                ComboFO.SelectedValue, ComboParam.SelectedValue, ComboFood.SelectedValue, ComboPeriod.SelectedValue.ToLowerFirstSymbol());
            UltraChart.DataBind();
            SetupChart();
        }

        protected string GetSelectedFood()
        {
            selectedPeriod.Value = periodDigest.GetMemberUniqueName(ComboPeriod.SelectedValue);
            return DataProvider.GetDataTableForChart("FSGS_0001_0004_selected_food", DataProvidersFactory.SecondaryMASDataProvider).Rows[0]["Товар"].ToString();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FSGS_0001_0004_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Территории", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataTable = dtGrid;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            if (band.Columns.Count == 0)
            {
                return;
            }

            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowDeleteDefault = AllowDelete.No;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            band.Columns[0].CellStyle.Wrap = true;
            band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            band.Columns[1].CellStyle.Wrap = true;
            band.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            band.Columns[0].Width = Unit.Parse("300px");
            for (int i = 1; i < band.Columns.Count; ++i)
            {
                band.Columns[i].Width = Unit.Parse(String.Format("{0}px", columnWidth));
                CRHelper.FormatNumberColumn(band.Columns[i], "P2");
            }

            GridHeaderLayout headerLayout = UltraWebGrid.GridHeaderLayout;

            headerLayout.AddCell("Территории");
            headerLayout.AddCell("Всего");
            headerLayout.AddCell("Продовольственные товары");
            headerLayout.AddCell(ComboFood.SelectedValue);

            headerLayout.ApplyHeaderInfo();

        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index < 2)
            {
                e.Row.Style.Font.Bold = true;
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            dtChart = dtGrid.Copy();

            dtChart.Columns["Всего"].ColumnName = dtChart.Columns["Всего"].Caption = "Все товары и услуги";
            dtChart.Columns["Продукт"].ColumnName = dtChart.Columns["Продукт"].Caption = ComboFood.SelectedValue;
            foreach (DataRow row in dtChart.Rows)
            {
                row["Территории"] = RegionsNamingHelper.ShortName(row["Территории"].ToString()).Replace("Российская  Федерация", "РФ");
            }

        }

        private void SetupChart()
        {
            UltraChart.ChartType = ChartType.Composite;
            UltraChart.BorderWidth = 0;

            UltraChart.Tooltips.FormatString = String.Format("<ITEM_LABEL>\n{0}\n<DATA_VALUE:P2>", ComboPeriod.SelectedValue);

            UltraChart.Legend.MoreIndicatorText = " ";
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            Color color1 = Color.Violet;

            PaintElement pe = CRHelper.GetFillPaintElement(color1, 150);
            pe.StrokeWidth = 0;

            UltraChart.ColorModel.Skin.PEs.Add(pe);
            UltraChart.ColorModel.Skin.ApplyRowWise = false;

            ChartArea area = new ChartArea();
            area.Border.Thickness = 0;
            UltraChart.CompositeChart.ChartAreas.Add(area);

            AxisItem axisX = new AxisItem();
            axisX.OrientationType = AxisNumber.X_Axis;
            axisX.SetLabelAxisType = SetLabelAxisType.GroupBySeries;
            axisX.DataType = AxisDataType.String;
            axisX.LineThickness = 1;
            axisX.Extent = 60;
            axisX.Labels.Visible = true;
            axisX.Labels.ItemFormatString = "<ITEM_LABEL>";
            axisX.Labels.HorizontalAlign = StringAlignment.Center;
            axisX.Labels.SeriesLabels.Visible = false;
            axisX.Labels.WrapText = true;
            axisX.Key = "X";

            AxisItem axisX2 = new AxisItem();
            axisX2.OrientationType = AxisNumber.X_Axis;
            axisX2.SetLabelAxisType = SetLabelAxisType.ContinuousData;
            axisX2.DataType = AxisDataType.String;
            axisX2.Visible = false;
            axisX2.Labels.Visible = false;
            axisX2.Key = "X2";

            AxisItem axisY = new AxisItem();
            axisY.OrientationType = AxisNumber.Y_Axis;
            axisY.DataType = AxisDataType.Numeric;
            axisY.LineThickness = 1;
            axisY.Extent = 60;
            axisY.Labels.ItemFormat = AxisItemLabelFormat.DataValue;
            axisY.Labels.HorizontalAlign = StringAlignment.Far;
            axisY.Labels.ItemFormatString = "<DATA_VALUE:P2>";
            axisY.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            axisY.Labels.Layout.Padding = 5;
            axisY.TickmarkStyle = AxisTickStyle.Smart;

            AxisItem axisY2 = new AxisItem();
            axisY2.OrientationType = AxisNumber.Y2_Axis;
            axisY2.DataType = AxisDataType.Numeric;
            axisY2.LineThickness = 0;
            axisY2.Extent = 10;
            axisY2.TickmarkStyle = AxisTickStyle.Smart;
            axisY2.Visible = true;
            axisY2.Labels.Visible = false;

            AxisItem hiddenAxisX2 = new AxisItem();
            hiddenAxisX2.OrientationType = AxisNumber.X2_Axis;
            hiddenAxisX2.Extent = 20;
            hiddenAxisX2.Labels.Visible = false;
            hiddenAxisX2.LineThickness = 0;
            hiddenAxisX2.Margin.Near.Value = 10;
            hiddenAxisX2.Margin.Far.Value = 10;
            hiddenAxisX2.Visible = true;

            area.Axes.Add(axisX);
            area.Axes.Add(axisX2);
            area.Axes.Add(axisY);
            area.Axes.Add(axisY2);
            area.Axes.Add(hiddenAxisX2);

            ChartLayerAppearance layer1 = new ChartLayerAppearance();
            ChartLayerAppearance layer2 = new ChartLayerAppearance();
            ChartLayerAppearance layer3 = new ChartLayerAppearance();

            layer1.ChartType = ChartType.ColumnChart;
            ((ColumnChartAppearance)layer1.ChartTypeAppearance).ColumnSpacing = 1;
            ((ColumnChartAppearance)layer1.ChartTypeAppearance).ChartText.Add(GetAllChartText());

            layer2.ChartType = ChartType.LineChart;
            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Square;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance.Thickness = 0;
            lineAppearance.IconAppearance.PE.ElementType = PaintElementType.SolidFill;
            lineAppearance.IconAppearance.PE.Fill = Color.Yellow;
            ((LineChartAppearance)layer2.ChartTypeAppearance).LineAppearances.Add(lineAppearance);
            
            layer3.ChartType = ChartType.LineChart;
            LineAppearance emptylineAppearance = new LineAppearance();
            emptylineAppearance.IconAppearance.Icon = SymbolIcon.Square;
            emptylineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            emptylineAppearance.IconAppearance.PE.ElementType = PaintElementType.SolidFill;
            emptylineAppearance.IconAppearance.PE.Fill = Color.Red;
            emptylineAppearance.Thickness = 0;
            ((LineChartAppearance)layer3.ChartTypeAppearance).LineAppearances.Add(emptylineAppearance);

            NumericSeries series = CRHelper.GetNumericSeries(3, dtChart);
            UltraChart.CompositeChart.Series.Add(series);
            layer1.Series.Add(series);

            series = CRHelper.GetNumericSeries(2, dtChart);
            UltraChart.CompositeChart.Series.Add(series);
            layer2.Series.Add(series);

            series = CRHelper.GetNumericSeries(1, dtChart);
            UltraChart.CompositeChart.Series.Add(series);
            layer3.Series.Add(series);

            layer1.ChartArea = area;
            layer1.AxisX = axisX;
            layer1.AxisY = axisY;
            layer1.LegendItem = LegendItemType.Series;

            layer2.ChartArea = area;
            layer2.AxisX = axisX2;
            layer2.AxisY = axisY;
            layer2.LegendItem = LegendItemType.Series;

            layer3.ChartArea = area;
            layer3.AxisX = axisX2;
            layer3.AxisY = axisY;
            layer3.LegendItem = LegendItemType.Series;

            UltraChart.CompositeChart.ChartLayers.Add(layer1);
            UltraChart.CompositeChart.ChartLayers.Add(layer2);
            UltraChart.CompositeChart.ChartLayers.Add(layer3);

            CompositeLegend compositeLegend = new CompositeLegend();
            compositeLegend.ChartLayers.Add(layer3);
            compositeLegend.ChartLayers.Add(layer2);
            compositeLegend.ChartLayers.Add(layer1);
            compositeLegend.PE.ElementType = PaintElementType.SolidFill;
            compositeLegend.PE.Fill = Color.FloralWhite;
            compositeLegend.BoundsMeasureType = MeasureType.Percentage;
            compositeLegend.Bounds = new System.Drawing.Rectangle(2, 92, 98, 7);
            compositeLegend.LabelStyle.Font = new Font("Verdana", 10);
            UltraChart.CompositeChart.Legends.Add(compositeLegend);
        }

        private ChartTextAppearance GetAllChartText()
        {
            ChartTextAppearance chartText = new ChartTextAppearance();
            chartText.Column = -2;
            chartText.Row = -2;
            chartText.VerticalAlign = StringAlignment.Far;
            chartText.HorizontalAlign = StringAlignment.Center;
            chartText.ItemFormatString = "<DATA_VALUE_ITEM:P2>";
            chartText.ChartTextFont = new Font("Verdana", 8);
            chartText.Visible = true;

            return chartText;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)UltraChart.CompositeChart.ChartLayers[0].ChartLayer.Grid["X"];

            if (xAxis == null)
                return;

            double xMin = xAxis.MapMinimum;
            double axisStep = (xAxis.Map(1) - xAxis.Map(0));
            bool firstLegendIconChanged = false;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Box && primitive.Series != null)
                {
                    Box box = primitive as Box;
                    PaintElement pe = box.PE.Clone();

                    pe.StrokeWidth = 1;

                    box.PE = pe;

                    if (box.DataPoint != null)
                    {
                        box.DataPoint.Label = ComboFood.SelectedValue;
                    }

                }

                if (primitive is Polyline)
                {
                    Polyline poly = (Polyline)primitive;

                    if (poly.Series != null && (poly.Series.Label == "Все товары и услуги" || poly.Series.Label == "Продовольственные товары"))
                    {

                        double offsetX = axisStep / 2;
                        for (int j = 0; j < poly.points.Length; j++)
                        {
                            DataPoint point = poly.points[j];

                            point.point = new Point((int)xMin + (int)offsetX, point.point.Y);

                            Text chartText = new Text();
                            chartText.labelStyle.Font = new Font("Verdana", 8);
                            chartText.labelStyle.FontColor = point.PolylineParent.PE.Fill;
                            chartText.labelStyle.HorizontalAlign = StringAlignment.Center;
                            chartText.bounds = new System.Drawing.Rectangle(point.point.X - 20, point.point.Y - 25, 50, 20);
                            //chartText.SetTextString(Convert.ToDouble(point.Value).ToString("N1"));
                            e.SceneGraph.Add(chartText);

                            offsetX += 2 * axisStep;

                            point.DataPoint.Label = String.Format("{0}", poly.Series.Label);
                        }
                    }
                    else if (poly.Path != null && poly.Path.ToLower().Contains("legend"))
                    {
                        poly.Visible = false;

                        Box icon = (!firstLegendIconChanged)
                                           ? GetSquareIcon(poly, Color.Red, 15)
                                           : GetSquareIcon(poly, Color.Yellow, 15);

                        e.SceneGraph.Add(icon);

                        firstLegendIconChanged = true;
                    }
                }
            }
        }

        private static Box GetSquareIcon(Polyline polyline, Color color, int radius)
        {
            Point center = new Point(polyline.points[0].point.X + (polyline.points[2].point.X - polyline.points[0].point.X) / 2, polyline.points[0].point.Y);
            Point topLeft = new Point(center.X - radius / 2, center.Y - radius / 2);
            Box box = new Box(topLeft, radius, radius);
            box.PE.ElementType = PaintElementType.SolidFill;
            box.PE.Fill = color;

            return box;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            Worksheet sheet3 = workbook.Worksheets.Add("Так надо");

            SetExportGridParams(UltraWebGrid);

            ReportExcelExporter1.HeaderCellHeight = 25;
            ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Left;
            ReportExcelExporter1.TitleStartRow = 0;

            ReportExcelExporter1.Export(UltraWebGrid.GridHeaderLayout, GridHeader.Text, sheet1, 4);

            sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            sheet1.Rows[1].Height = 550;

            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.7);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
            ReportExcelExporter1.Export(UltraChart, ChartHeader.Text, sheet2, 1);
            sheet2.MergedCellsRegions.Clear();
            sheet2.MergedCellsRegions.Add(0, 0, 0, 17);
            sheet2.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            sheet2.Rows[0].Height = 550;
            sheet2.Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;

            ReportExcelExporter1.Export(UltraWebGrid.GridHeaderLayout, sheet3, 0);

            workbook.Worksheets.Remove(sheet3);
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
        }

        private static void SetExportGridParams(UltraGridBrick grid)
        {
            string exportFontName = "Verdana";
            int fontSize = 10;
            double coeff = 1.1;
            foreach (UltraGridColumn column in grid.Grid.Columns)
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
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text.Replace("<b>", "").Replace("</b>", "");

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 40;
            ReportPDFExporter1.Export(UltraWebGrid.GridHeaderLayout, GridHeader.Text, section1);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.8);
            ReportPDFExporter1.Export(UltraChart, ChartHeader.Text, section2);
        }

        #endregion
    }
}