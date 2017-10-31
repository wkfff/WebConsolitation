using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Dundas.Maps.WebControl;

using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Text;

/**
 *  Анализ розничных цен на социально значимые продовольственные товары по состоянию на ЧЧ.ММ.ГГГГ
 */
namespace Krista.FM.Server.Dashboards.reports.ORG_0003_0018
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate;
        private DataTable dtFood;
        private DataTable dtGrid;
        private DataTable dtChart;
        private DataTable medianDT;
        private GridHeaderLayout headerLayout;

        #endregion

        // имя папки с картами региона
        private string mapFolderName;

        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 900; }
        }

        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }

        private static string getBrowser
        {
            get { return HttpContext.Current.Request.Browser.Browser; }
        }

        #region Параметры запроса

        // выбранный товар
        private CustomParam IndexInf;


        // выбранный товар
        private CustomParam selectedFood;
        // выбранная дата
        private CustomParam selectedDate;
        // предыдущий месяц дата
        private CustomParam previousDate;
        // начало года
        private CustomParam yearDate;
        // те же, но в текстовом формате (для вывода на экран, чтобы не конвертировать)
        private static string selectedDateText;
        private static string previousDateText;
        private static string yearDateText;

        #endregion

        // --------------------------------------------------------------------

        // заголовок страницы
        private const string PageTitleCaption = "Средние розничные цены в регионе (по состоянию на конец месяца)";
        private const string PageSubTitleCaption = "Eжемесячный мониторинг средних розничных цен на социально значимые товары в разрезе муниципальных образований, Ханты-Мансийский автономный округ – Югра, по состоянию на {0}.";
        // заголовок для UltraChart
        private const string ChartTitleCaption = "Распределение территорий по средней розничной цене на товар \"{0}\", рублей за {1}, по состоянию на {2}";
        private const string MapTitleCaption = "Средняя розничная цена на товар \"{0}\", рублей за {1}, по состоянию на {2}";

        // Единицы измерения
        private static Dictionary<string, string> dictUnits;

        // --------------------------------------------------------------------

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            mapFolderName = "ХМАОDeer";

            ComboDate.Title = "Выберите период";
            ComboDate.Width = IsSmallResolution ? 250 : 300;
            ComboDate.ParentSelect = true;
            ComboFood.Title = "Товар";
            ComboFood.Width = IsSmallResolution ? 400 : 500;

            #region Настройка грида

            UltraWebGrid.Width = IsSmallResolution ? 750 : CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #endregion

            #region Настройка диаграммы

            UltraChart.Width = IsSmallResolution ? 740 : CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 17);
            UltraChart.Height = Unit.Empty;

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;

            UltraChart.ColumnChart.SeriesSpacing = 1;
            UltraChart.ColumnChart.ColumnSpacing = 1;

            UltraChart.Axis.X.Extent = 150;
            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart.Axis.Y.Extent = 50;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;

            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\nРозничная цена: <b><DATA_VALUE:N2></b>, рубль";
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);



            #endregion

            #region Параметры

            selectedDate = UserParams.CustomParam("selected_date");
            previousDate = UserParams.CustomParam("previous_date");
            yearDate = UserParams.CustomParam("year_date");
            selectedFood = UserParams.CustomParam("selected_food");
            IndexInf = UserParams.CustomParam("IndexInf");

            #endregion

            #region Карта

            //mapFolderName = RegionSettingsHelper.Instance.GetPropertyValue("MapFolderName");
            //mapZoomValue = Convert.ToDouble(RegionSettingsHelper.Instance.GetPropertyValue("MapZoomValue"));

            DundasMap.Width = IsSmallResolution ? 750 : CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 190);

            #endregion

            #region Экспорт
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;
            #endregion
        }

        // --------------------------------------------------------------------

        Node GetRoot(Node n)
        {
            if (n.Parent != null)
            {
                return GetRoot(n.Parent);
            }
            return n;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            {
                FillComboDate("ORG_0003_0018_list_of_dates");
                FillComboFood("ORG_0003_0018_list_of_food");
            }

            #region Анализ параметров
            GetDates(ComboDate, selectedDate, previousDate, yearDate);

            selectedDateText = MDXDateToShortDateString(selectedDate.Value);

            previousDateText = MDXDateToShortDateString(previousDate.Value);

            yearDateText = MDXDateToShortDateString(yearDate.Value);

            if (ComboFood.SelectedNode.Level == 0)
            {
                selectedFood.Value = Unamefood[ComboFood.SelectedNode.FirstNode.Text];
                    //StringToMDXFood(ComboFood.SelectedValue, ComboFood.SelectedNode.FirstNode.Text);
            }
            if (ComboFood.SelectedNode.Level == 1)
            {
                selectedFood.Value = Unamefood[ComboFood.SelectedValue];
                //selectedFood.Value = StringToMDXFood(ComboFood.SelectedNode.Parent.Text, ComboFood.SelectedValue);
            }

            if ((GetRoot(ComboFood.SelectedNode).Text.Contains("Моющие")) || (GetRoot(ComboFood.SelectedNode).Text.Contains("Прочие")))
            {
                IndexInf.Value = "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на непродовольственные товары за период с начала года]";
            }
            else
            {
                IndexInf.Value = "[Организации__Товары и услуги].[Организации__Товары и услуги].[Все товары и услуги].[Показатели].[Индекс потребительских цен на продовольственные товары за период с начала года]";
            }

            #endregion

            PageTitle.Text = PageTitleCaption;
            //"Ежемесячный мониторинг средних розничных цен на социально значимые продовольственные товары в разрезе муниципальных образований, Ханты-Мансийский автономный округ – Югра, по состоянию на март 2011 года.";
            //"Анализ средних розничных цен на отдельные виды продовольственных товаров в разрезе муниципальных образований по состоянию на конец месяца";

            Page.Title = PageTitle.Text;

            PageSubTitle.Text = String.Format(PageSubTitleCaption, ComboDate.SelectedValue.ToLower());

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            calculateRank();

            UltraChart.DataBind();

            #region Карта

            DundasMap.Shapes.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Cost");
            DundasMap.ShapeFields["Cost"].Type = typeof(double);
            DundasMap.ShapeFields["Cost"].UniqueIdentifier = false;

            SetMapSettings();
            AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);
            AddMapLayer(DundasMap, mapFolderName, "Граница", CRHelper.MapShapeType.SublingAreas);
            FillMapData();

            #endregion

            GenerateChartSpeedDeviation();
            SpeedDeviationChart.Tooltips.FormatString = "<SERIES_LABEL><br>Темп прироста к <b>" + 
                string.Format(" январю {0}а", ComboDate.SelectedNode.Parent.Text) + "</b><br><b><DATA_VALUE:N2></b> %";

            string unit;
            dictUnits.TryGetValue(ComboFood.SelectedValue, out unit);
            GridLabel.Text = string.Format("Средние розничные цены на товар «{0}», рублей за {1}",
                ComboFood.SelectedValue,
                unit.ToLower());

            labelChart0.Text = string.Format("Распределение территорий по темпу прироста средней розничной цены на {0} с начала года, процент", ComboDate.SelectedValue.ToLower());
            if ( UltraWebGrid.Rows.Count > 0)
                if (UltraWebGrid.Rows[0].Cells[0].Text.Contains("округ"))
                {
                    UltraWebGrid.Rows[0].Style.Font.Bold = true;
                }
            foreach (UltraGridColumn col in UltraWebGrid.Columns)
            {
                col.CellStyle.Wrap = true;
            }

            //yearDateText);
        }

        private void GenerateChartSpeedDeviation()
        {
            ConfSpeedDeviationChart();
            DataBindDeviationChart();
        }

        class CoolTable : DataTable
        {
            public DataRow AddRow()
            {
                DataRow Row = this.NewRow();
                this.Rows.Add(Row);
                return Row;
            }
        }
        double? LineVal = null;
        private void DataBindDeviationChart()
        {
            int IndexColSpeedDeviation = UltraWebGrid.Columns.Count - 1;

            CoolTable tableChartSP = new CoolTable();
            tableChartSP.Columns.Add("MO");
            tableChartSP.Columns.Add("V", typeof(decimal));

            DataTable LineValue = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("ORG_0003_0018_chart_line"), "нечто унылое", LineValue);
            try
            {
                LineVal = double.Parse(LineValue.Rows[0][1].ToString()) - 100;
            }
            catch { }

            decimal Max = decimal.MinValue;
            decimal Min = decimal.MaxValue;

            foreach (UltraGridRow Row in UltraWebGrid.Rows)
            {
                decimal Value;
                try
                {
                    string strVal = Row.Cells[IndexColSpeedDeviation].Text;
                    Value = decimal.Parse(strVal.Remove(strVal.Length - 1));
                    DataRow NewRow = tableChartSP.AddRow();
                    NewRow["MO"] = ConverToShortName(Row.Cells[0].Text);
                    NewRow["V"] = Value;

                    Max = Max < Value ? Value : Max;
                    Min = Min > Value ? Value : Min;
                }
                catch { }
            }

            if (LineVal != null)
            {
                Max = Max < (decimal)LineVal ? (decimal)LineVal : Max;
                Min = Min > (decimal)LineVal ? (decimal)LineVal : Min;
            }

            foreach (DataRow Row in tableChartSP.Rows)
            {
                if (Row["MO"].ToString().Contains("Югра"))
                {
                    Row["MO"] = Row["MO"].ToString().Replace("Ханты-Мансийский автономный округ - Югра", "ХМАО");
                    break;
                }
            }

            SpeedDeviationChart.Axis.Y.RangeType = AxisRangeType.Custom;
            SpeedDeviationChart.Axis.Y.RangeMax = (double)Max * 1.1 + 1;
            SpeedDeviationChart.Axis.Y.RangeMin = (double)Min - 3;


            SpeedDeviationChart.Data.SwapRowsAndColumns = true;
            SpeedDeviationChart.DataSource = tableChartSP;
            SpeedDeviationChart.DataBind();
        }

        decimal SecondHmaoChartVal = -1;

        string ConverToShortName(string NameRegion)
        {
            return NameRegion.Replace("муниципальный район", "м-р").Replace("Город", "г. ");
        }

        private void ConfSpeedDeviationChart()
        {
            #region Настройка диаграммы 1
            UltraChart UltraChart1 = SpeedDeviationChart;

            if (!IsSmallResolution)
            {
                UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 17);
            }
            else
            {
                UltraChart1.Width = CRHelper.GetChartWidth(750);
            }
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);

            UltraChart1.ChartType = ChartType.LineChart;

            UltraChart1.Border.Thickness = 0;

            UltraChart1.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
            UltraChart1.Axis.X.Margin.Near.Value = 10;
            UltraChart1.Axis.X.Margin.Far.MarginType = LocationType.Pixels;
            UltraChart1.Axis.X.Margin.Far.Value = 10;
            UltraChart1.Axis.Y.Margin.Near.MarginType = LocationType.Pixels;
            UltraChart1.Axis.Y.Margin.Near.Value = 10;
            UltraChart1.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            UltraChart1.Axis.Y.Margin.Far.Value = 10;

            UltraChart1.Axis.X.Extent = 120;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.Y.Extent = 50;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:### ### ##0.##>";


            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 10;
            UltraChart1.Legend.Font = new Font("Microsoft Sans Serif", 9);
            UltraChart1.Legend.Visible = false;

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.PEs.Clear();

            PaintElement pe;
            pe = new PaintElement(Color.Yellow);
            pe.StrokeWidth = 0;
            UltraChart1.ColorModel.Skin.PEs.Add(pe);
            pe = new PaintElement(Color.Yellow);
            pe.StrokeWidth = 0;
            UltraChart1.ColorModel.Skin.PEs.Add(pe);
            pe = new PaintElement(Color.Yellow);
            pe.StrokeWidth = 0;
            UltraChart1.ColorModel.Skin.PEs.Add(pe);

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Triangle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Large;
            pe = new PaintElement(Color.Yellow);
            lineAppearance.IconAppearance.PE = pe;
            lineAppearance.Thickness = 0;
            UltraChart1.LineChart.LineAppearances.Add(lineAppearance);

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Triangle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Large;
            pe = new PaintElement(Color.Yellow);
            lineAppearance.IconAppearance.PE = pe;
            lineAppearance.Thickness = 0;
            UltraChart1.LineChart.LineAppearances.Add(lineAppearance);

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Triangle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Large;
            pe = new PaintElement(Color.Yellow);
            lineAppearance.IconAppearance.PE = pe;
            lineAppearance.Thickness = 0;
            UltraChart1.LineChart.LineAppearances.Add(lineAppearance);



            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion
        }

        protected void GenHorizontalLineAndLabel(int startX, int StartY, int EndX, int EndY, Color ColorLine, string Label, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e, bool TextUP)
        {
            Infragistics.UltraChart.Core.Primitives.Line Line = new Infragistics.UltraChart.Core.Primitives.Line(new Point(startX, StartY), new Point(EndX, EndY));

            Line.PE.Stroke = ColorLine;
            Line.PE.StrokeWidth = 2;
            Line.lineStyle.DrawStyle = LineDrawStyle.Dot;

            Text textLabel = new Text();
            textLabel.labelStyle.Orientation = TextOrientation.Horizontal;
            textLabel.PE.Fill = System.Drawing.Color.Black;
            textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 10, FontStyle.Italic);

            textLabel.labelStyle.FontColor = Color.LightGray;

            textLabel.labelStyle.HorizontalAlign = StringAlignment.Near;
            textLabel.labelStyle.VerticalAlign = StringAlignment.Near;

            if (TextUP)
            {
                textLabel.bounds = new System.Drawing.Rectangle(startX + 50, StartY - 15, 800, 15);
            }
            else
            {
                textLabel.bounds = new System.Drawing.Rectangle(startX + 50, StartY + 1, 800, 15);
            }

            textLabel.labelStyle.FontColor = Color.LightGray;

            textLabel.SetTextString(Label);
            e.SceneGraph.Add(Line);

            e.SceneGraph.Add(textLabel);


        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (!String.IsNullOrEmpty(primitive.Path) && primitive.Path == "Legend" && primitive is Polyline)
                {
                    Polyline line = primitive as Polyline;
                    Infragistics.UltraChart.Core.Primitives.Symbol symbol = new Infragistics.UltraChart.Core.Primitives.Symbol();
                    symbol.PE = line.PE;
                    symbol.icon = SymbolIcon.Triangle;
                    symbol.iconSize = SymbolIconSize.Medium;
                    symbol.point = line.points[1].point;
                    symbol.fillColor = Color.Yellow;
                    e.SceneGraph[i] = symbol;
                }
                if (primitive is PointSet)
                {
                }
            }

            Text Caption = new Text();
            Caption.SetTextString("Процент");
            Caption.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            Caption.labelStyle.FontColor = Color.Gray;
            Caption.bounds.X = -30;
            Caption.bounds.Y = 120;
            Caption.bounds.Width = 100;
            Caption.bounds.Height = 100;

            e.SceneGraph.Add(Caption);


            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];


            try
            {
                GenHorizontalLineAndLabel(
                        (int)xAxis.Map(xAxis.Minimum),
                        (int)yAxis.Map(LineVal),
                        (int)xAxis.Map(xAxis.Maximum),
                        (int)yAxis.Map(LineVal),
                        Color.Gray,
                        "Уровень инфляции: " + string.Format("{0:N2}%", LineVal),
                        e,
                        true);
            }
            catch { }
            try
            {
                if (SecondHmaoChartVal != -1)
                    GenHorizontalLineAndLabel(
                            (int)xAxis.Map(xAxis.Minimum),
                            (int)yAxis.Map(SecondHmaoChartVal),
                            (int)xAxis.Map(xAxis.Maximum),
                            (int)yAxis.Map(SecondHmaoChartVal),
                            Color.Blue,
                            "Ханты-Мансийский автономный округ - Югра: " + string.Format("{0:N2}%", SecondHmaoChartVal),
                            e,
                            true);
            }
            catch { }

            
        }

        protected void GetDates(CustomMultiCombo combo, CustomParam selectedDate, CustomParam previousDate, CustomParam yearDate)
        {
            Node node = new Node();

            node = combo.SelectedNode;

            selectedDate.Value = KeyAndUNameComboDate[node.Text];

            previousDate.Value = KeyAndUNamePrevComboDate[node.Text];

            yearDate.Value = KeyAndUNameYearComboDate[node.Text];
        }

        // --------------------------------------------------------------------

        #region Обработчики карты

        public void SetMapSettings()
        {
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = 100;
            //DundasMap.Viewport.Zoom = (float)mapZoomValue;

            // добавляем легенду
            Legend legend = new Legend("CostLegend");
            legend.Visible = true;
            legend.BackColor = Color.White;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            string unit;
            dictUnits.TryGetValue(ComboFood.SelectedValue, out unit);
            legend.Title = String.Format("Средняя розничная\nцена на товар,\nрублей за {0}", unit.ToLower());
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CostRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Cost";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Green;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CostLegend";
            DundasMap.ShapeRules.Add(rule);
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../../../maps/Субъекты/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "Name", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        public static Shape FindMapShape(MapControl map, string patternValue)
        {
            foreach (Shape shape in map.Shapes)
            {
                if (shape.Name.ToLower() == patternValue.ToLower())
                {
                    return shape;
                }
            }
            return null;
        }

        public void FillMapData()
        {
            string valueSeparator = IsMozilla ? ". " : "\n";
            string shapeHint = "{0}" + valueSeparator + "Розничная цена: {1:N2}, рубль" + valueSeparator + "Ранг: {2}";
            string unit;
            dictUnits.TryGetValue(ComboFood.SelectedValue, out unit);
            labelMap.Text = String.Format(MapTitleCaption, ComboFood.SelectedValue, unit.ToLower(), ComboDate.SelectedValue.ToLower());
            if (dtGrid == null || DundasMap == null)
            {
                return;
            }
            foreach (UltraGridRow row in UltraWebGrid.Rows)
            {
                try
                {
                    // заполняем карту данными
                    string subject = row.Cells[0].Value.ToString().Replace("Город ", "г. ").Replace(" муниципальный район", " р-н");
                    double value = Convert.ToDouble(row.Cells[1].Value.ToString());
                    Shape shape = FindMapShape(DundasMap, subject.Replace("*", ""));
                    shape.Visible = true;
                    string shapeName = shape.Name;
                    shape["Name"] = subject;
                    shape["Cost"] = Convert.ToDouble(row.Cells[3].Value.ToString());
                    shape.ToolTip = String.Format(shapeHint, subject, row.Cells[3].Value, row.Cells[4].Value);
                    shape.TextVisibility = TextVisibility.Shown;
                    shape.Text = String.Format("{0}\n{1:N2}", shapeName, value);
                }
                catch { }
            }
        }

        #endregion

        // --------------------------------------------------------------------

        #region Обработчики грида

        class SortDataRow : System.Collections.Generic.IComparer<DataRow>
        {
            #region Члены IComparer<RegionValue>

            public int Compare(DataRow x, DataRow y)
            {
                return -Compare_(x, y);
            }

            public int Compare_(DataRow x, DataRow y)
            {
                string Xname = x[0].ToString();
                string Yname = y[0].ToString();

                if (Xname == Yname)
                {
                    return 0;
                }

                if (Xname.Contains("округ"))
                {
                    return 1;
                }
                
                if (Yname.Contains("округ"))
                {
                    return -1;
                }

                if (Xname.Contains("Город Ханты-Мансийск"))
                {
                    return 1;
                }

                if (Yname.Contains("Город Ханты-Мансийск"))
                {
                    return -1;
                }
                if ((Xname[0] == 'Г') && (Yname[0] != 'Г'))
                {
                    return 1;
                }

                if ((Xname[0] != 'Г') && (Yname[0] == 'Г'))
                {
                    return -1;
                }


                return Yname.CompareTo(Xname);
            }

            #endregion
        }

        DataTable SortTable(DataTable Table)
        {
            DataTable TableSort = new DataTable();

            foreach (DataColumn col in Table.Columns)
            {
                TableSort.Columns.Add(col.ColumnName, col.DataType);
            }

            List<DataRow> LR = new System.Collections.Generic.List<DataRow>();

            foreach (DataRow row in Table.Rows)
            {
                LR.Add(row);
            }

            LR.Sort(new SortDataRow());



            foreach (DataRow Row in LR)
            {
                TableSort.Rows.Add(Row.ItemArray);
            }
            return TableSort;
        }


        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("ORG_0003_0018_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                dtGrid.Columns.Add("Ранг", typeof(int));
                dtGrid.Columns.Add();
                dtGrid.Columns.Add();
                dtGrid.Columns.Add();
                dtGrid.Columns.Add();
                UltraWebGrid.DataSource = SortTable(dtGrid);
            }
            else
            {
                UltraWebGrid.Height = UltraChart.Height;
                UltraWebGrid.DataSource = null;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            e.Layout.NullTextDefault = "-";
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(230);
            int columnWidth = 120;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; ++i)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
            }
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "P2");
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(columnWidth - 60);

            // Заголовки
            GridHeaderCell header;

            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.VerticalAlign = VerticalAlign.Middle;
            headerLayout.AddCell("Наименование МО");
            header = headerLayout.AddCell("Средняя розничная цена, рубль");
            header.AddCell(ComboDate.SelectedValue.ToLower().Replace(" 20", "<br>   20"));
            //selectedDateText);             
            header.AddCell(KeyDisplayAndCaption[ComboDate.SelectedValue].FirstString.ToLower().Replace(" 20", "<br>20"));
            //previousDateText);
            //header.AddCell(yearDateText);
            header.AddCell(KeyDisplayAndCaption[ComboDate.SelectedValue].SecondString.ToLower().Replace(" 20", "<br>   20"));

            headerLayout.AddCell("Ранг");

            header = headerLayout.AddCell("Динамика к предыдущему отчетному периоду");

            header.AddCell("Абсолютное отклонение, рубль");
            header.AddCell("Темп прироста, %");

            header = headerLayout.AddCell("Динамика за период с начала года");

            header.AddCell("Абсолютное отклонение, рубль");
            header.AddCell("Темп прироста, %");

            headerLayout.ApplyHeaderInfo();
        }

        private void SetStarChar(UltraGridCell Cell)
        {
            string NameRegion = Cell.Text;

            string[] StarRegions = new string[13] { "Ханты-Мансийский автономный округ", "Советск", "Сургутск", "Когал", "Ланге", "Мегион", "Нефтеюганск", "Нижневартовский-", "Нягань", "Сургут", "Пыть", "Югорск", "Город Нижневар" };
            foreach (string R in StarRegions)
            {
                if (NameRegion.Contains(R))
                {
                    return;
                }
            }
            Cell.Text += "*";
        }

        void SetImageFromCell(UltraGridCell Cell, string ImageName)
        {
            string ImagePath = "~/images/" + ImageName;
            //;
            Cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
            Cell.Style.BackgroundImage = ImagePath;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string name = e.Row.Cells[0].GetText();
            name = name.Replace("ДАННЫЕ", String.Empty).Replace("(", String.Empty).Replace(")", String.Empty).Trim();
            e.Row.Cells[0].Value = name;
            double value;
            if (!Double.TryParse(e.Row.Cells[1].ToString(), out value))
            {
                e.Row.Delete();
            }
            double prevValue, yearValue;
            if (Double.TryParse(e.Row.Cells[3].ToString(), out prevValue))
            {
                e.Row.Cells[7].Value = value - prevValue;
                e.Row.Cells[8].Value = String.Format("{0:P2}", (value - prevValue) / prevValue);
                if (Math.Abs(value - prevValue) > 0.01)
                {
                    if (value < prevValue)
                    {
                        SetImageFromCell(e.Row.Cells[8], "ArrowGreenDownBB.png");
                    }
                    else
                    {
                        SetImageFromCell(e.Row.Cells[8], "ArrowRedUpBB.png");
                    }
                }
            }

            if (Double.TryParse(e.Row.Cells[2].ToString(), out yearValue))
            {
                e.Row.Cells[5].Value = value - yearValue;
                e.Row.Cells[6].Value = String.Format("{0:P2}", (value - yearValue) / yearValue);
                if (Math.Abs(value - yearValue) > 0.01)
                {
                    if (value < yearValue)
                    {
                        SetImageFromCell(e.Row.Cells[6], "ArrowGreenDownBB.png");
                    }
                    else if (value > yearValue)
                    {
                        SetImageFromCell(e.Row.Cells[6], "ArrowRedUpBB.png");

                    }
                }
            }
            else
            {
                //e.Row.Cells[1].Value = "-";
                //e.Row.Cells[5].Value = "-";
                //e.Row.Cells[6].Value = "-";
            }
            // Хинты


            e.Row.Cells[5].Title = String.Format("Изменение в руб. к {0}",
                KeyDisplayAndCaptionSclon[ComboDate.SelectedValue].FirstString.ToLower());
            //yearDateText);
            e.Row.Cells[6].Title = String.Format("Изменение в % к {0}",
                KeyDisplayAndCaptionSclon[ComboDate.SelectedValue].FirstString.ToLower());
            //yearDateText);
            e.Row.Cells[7].Title = String.Format("Изменение в руб. к январю {0}а",
                ComboDate.SelectedNode.Parent.Text);
                //KeyDisplayAndCaptionSclon[ComboDate.SelectedValue].SecondString.ToLower()); 
            //KeyDisplayAndCaptionSclon);
            e.Row.Cells[8].Title = //"ляля";
                String.Format("Изменение в % к январю {0}а",
                ComboDate.SelectedNode.Parent.Text);
            //previousDateText);
             
            SetStarChar(e.Row.Cells[0]);

        }


        class RankingField
        {
            class SortKeyVal : System.Collections.Generic.IComparer<KeyVal>
            {
                #region Члены IComparer<KeyVal>

                public int Compare(KeyVal x, KeyVal y)
                {
                    if (x.Val > y.Val)
                    {
                        return 1;
                    }
                    if (x.Val < y.Val)
                    {
                        return -1;
                    }
                    return 0;
                }

                #endregion
            }

            struct KeyVal
            {
                public string Key;
                public decimal Val;
            }

            List<KeyVal> Fields = new List<KeyVal>();

            public int Count
            {
                get { return Fields.Count; }
            }

            public void AddItem(string Key, decimal Val)
            {
                KeyVal NewFild = new KeyVal();
                NewFild.Key = Key;
                NewFild.Val = Val;
                Fields.Add(NewFild);
            }

            void ClearDoubleVal()
            {
                List<KeyVal> RemoveList = new List<KeyVal>();
                for (int i = 0; i < Fields.Count - 1; i++)
                {
                    for (int j = i + 1; j < Fields.Count; j++)
                    {
                        if (Fields[i].Key == Fields[j].Key)
                        {
                            //RemoveList.Add(Fields[j]);
                            Fields.Remove(Fields[j]);
                        }
                    }
                }

                foreach (KeyVal kv in RemoveList)
                {
                    Fields.Remove(kv);
                }
            }

            public object GetRang(string Key)
            {
                ClearDoubleVal();
                Fields.Sort(new SortKeyVal());

                int i = 0;
                foreach (KeyVal kv in Fields)
                {
                    i++;

                    if (kv.Key.Split(';')[0] == Key.Split(';')[0])
                    {
                        return i;
                    }
                }
                return DBNull.Value;
            }

        }

        protected void calculateRank()
        {
            //double[] rank = new double[UltraWebGrid.Rows.Count];
            //for (int i = 0; i < UltraWebGrid.Rows.Count; ++i)
            //{
            //    rank[i] = Convert.ToDouble(UltraWebGrid.Rows[i].Cells[1].Value);
            //}
            //Array.Sort(rank);
            //int rank_value;
            //for (int i = 0; i < rank.Length; ++i)
            //{
            //    for (int j = 0; j < UltraWebGrid.Rows.Count; ++j)
            //    {
            //        if (rank[i] == Convert.ToDouble(UltraWebGrid.Rows[j].Cells[1].Value))
            //        {
            //            if (!Int32.TryParse(UltraWebGrid.Rows[j].Cells[4].ToString(), out rank_value))
            //            {
            //                UltraWebGrid.Rows[j].Cells[4].Value = i + 1;
            //            }
            //           
            //        }
            //    }
            //}

            RankingField Ranger = new RankingField();
            foreach (UltraGridRow Row in UltraWebGrid.Rows)
            {
                if (!Row.Cells[0].ToString().Contains("округ"))
                {
                    Ranger.AddItem(Row.Cells[1].ToString(), (decimal)Row.Cells[1].Value);
                }
            }

            foreach (UltraGridRow Row in UltraWebGrid.Rows)
            {
                if (!Row.Cells[0].ToString().Contains("округ"))
                try
                {
                    int Rank = (int)Ranger.GetRang(Row.Cells[1].ToString());
                    Row.Cells[4].Value = Rank;

                    if (Rank == 1)
                    {
                        Row.Cells[4].Style.BackgroundImage = "~/images/starYellowbb.png";
                        Row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        Row.Cells[4].Title = "Самый низкий уровень цены";
                    }
                    if (Rank == Ranger.Count)
                    {
                        Row.Cells[4].Style.BackgroundImage = "~/images/starGraybb.png";
                        Row.Cells[4].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        Row.Cells[4].Title = "Самый высокий уровень цены";
                    }
                }
                catch { }

            }



        }

        #endregion

        // --------------------------------------------------------------------

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string unit;
            dictUnits.TryGetValue(ComboFood.SelectedValue, out unit);
            labelChart.Text = String.Format(ChartTitleCaption, ComboFood.SelectedValue, unit.ToLower(), ComboDate.SelectedValue.ToLower());
            //yearDateText);
            string query = DataProvider.GetQueryText("ORG_0003_0018_chart");
            dtChart = new DataTable();
            medianDT = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Розничная цена", dtChart);
            double minValue = Double.PositiveInfinity; ;
            double maxValue = Double.NegativeInfinity;
            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    row[0] = row[0].ToString().Replace("ДАННЫЕ", String.Empty).Replace("(", String.Empty).Replace(")", String.Empty).Trim();
                    row[0] = row[0].ToString().Replace(" муниципальный район", " р-н");
                    row[0] = row[0].ToString().Replace("Город ", "Г. ");
                }
            }
            if (dtChart.Rows.Count > 1)
            {
                double avgValue = 0;
                for (int i = 0; i < dtChart.Rows.Count; ++i)
                {
                    double value = Convert.ToDouble(dtChart.Rows[i][1]);
                    avgValue += value;
                    minValue = value < minValue ? value : minValue;
                    maxValue = value > maxValue ? value : maxValue;
                }
                avgValue /= dtChart.Rows.Count;
                // рассчитываем медиану
                int medianIndex = MedianIndex(dtChart.Rows.Count);
                medianDT = dtChart.Clone();
                double medianValue = MedianValue(dtChart, 1);
                for (int i = 0; i < dtChart.Rows.Count - 1; i++)
                {

                    medianDT.ImportRow(dtChart.Rows[i]);

                    double value;
                    Double.TryParse(dtChart.Rows[i][1].ToString(), out value);
                    double nextValue;
                    Double.TryParse(dtChart.Rows[i + 1][1].ToString(), out nextValue);
                    if (((value <= avgValue) && (nextValue > avgValue)) && (i == medianIndex))
                    {
                        if (medianValue > avgValue)
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Среднее";
                            row[1] = avgValue;
                            medianDT.Rows.Add(row);
                            row = medianDT.NewRow();
                            row[0] = "Медиана";
                            row[1] = MedianValue(dtChart, 1);
                            medianDT.Rows.Add(row);
                        }
                        else
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Медиана";
                            row[1] = MedianValue(dtChart, 1);
                            medianDT.Rows.Add(row);
                            row = medianDT.NewRow();
                            row[0] = "Среднее";
                            row[1] = avgValue;
                            medianDT.Rows.Add(row);
                        }
                    }
                    else
                    {
                        if ((value <= avgValue) && (nextValue > avgValue))
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Среднее";
                            row[1] = avgValue;
                            medianDT.Rows.Add(row);
                        }

                        if (i == medianIndex)
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "Медиана";
                            row[1] = MedianValue(dtChart, 1);
                            medianDT.Rows.Add(row);
                        }
                    }
                }
                medianDT.ImportRow(dtChart.Rows[dtChart.Rows.Count - 1]);

                if (!Double.IsPositiveInfinity(minValue) && !Double.IsNegativeInfinity(maxValue))
                {
                    UltraChart.Axis.Y.RangeType = AxisRangeType.Custom;
                    UltraChart.Axis.Y.RangeMax = maxValue * 1.1;
                    UltraChart.Axis.Y.RangeMin = minValue / 1.1;
                }

                if (medianDT !=null)
                {
                    DataRow HMAORow = null;

                    foreach (DataRow row in medianDT.Rows)
                    {
                        if (row[0].ToString().Contains("округ"))
                        {
                            HMAORow = row;
                            break;
                        }
                    }
                    if (HMAORow != null)
                    {
                        if (HMAORow[1] != DBNull.Value)
                        {
                            HMAoVal = (decimal)HMAORow[1];
                        }
                        medianDT.Rows.Remove(HMAORow);
                    }
                }
                 

            }


            List<DataRow> DelRows = new List<DataRow>();

            foreach (DataRow row in medianDT.Rows)
            {
                if (row[0].ToString().Contains("Медиа")||row[0].ToString().Contains("Средне"))
                {
                    DelRows.Add(row);
                }
            }
            foreach(DataRow Row in DelRows)
            {
                medianDT.Rows.Remove(Row);
            }

            UltraChart.DataSource = (medianDT == null) ? null : medianDT.DefaultView;
        }

        decimal HMAoVal = -1;

        protected void GenHorizontalLineAndLabel(decimal val, Color ColorLine, string Label, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e, bool TextUP)
        {
            if (val< 0)
            {
                return;
            }

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];


            int CorVal = (int)yAxis.Map(val);
            int CorX = (int)xAxis.Map(xAxis.Minimum);

            Infragistics.UltraChart.Core.Primitives.Line Line = new Infragistics.UltraChart.Core.Primitives.Line(
                new Point(CorX, CorVal),
                new Point((int)xAxis.Map(xAxis.Maximum), CorVal)); 

            Line.PE.Stroke = ColorLine;
            Line.PE.StrokeWidth = 2;
            Line.lineStyle.DrawStyle = LineDrawStyle.Dot;


            Text textLabel = new Text();
            textLabel.labelStyle.Orientation = TextOrientation.Horizontal;
            textLabel.PE.Fill = System.Drawing.Color.Black;
            //textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 8);
            textLabel.labelStyle.Font = new System.Drawing.Font("Arial", 10, FontStyle.Italic);

            textLabel.labelStyle.FontColor = Color.LightGray;

            textLabel.labelStyle.HorizontalAlign = StringAlignment.Near;
            textLabel.labelStyle.VerticalAlign = StringAlignment.Near;

            if (TextUP)
            {
                textLabel.bounds = new System.Drawing.Rectangle(CorX + 50, CorVal - 15, 500, 15);
            }
            else
            {
                textLabel.bounds = new System.Drawing.Rectangle(CorX + 50, CorVal + 1, 500, 15);
            }


            textLabel.labelStyle.FontColor = Color.LightGray;

            textLabel.SetTextString(Label);

            e.SceneGraph.Add(Line);
            e.SceneGraph.Add(textLabel);


        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.Series != null && (box.Series.Label == "Среднее" || box.Series.Label == "Медиана"))
                        {
                            box.PE.Fill = Color.Orange;
                            box.PE.FillStopColor = Color.OrangeRed;
                        }
                    }
                }
            }
            string unit;
            dictUnits.TryGetValue(ComboFood.SelectedValue, out unit);
            Text Caption = new Text();
            Caption.SetTextString("Рублей за " + unit.ToLower());
            Caption.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
            Caption.labelStyle.FontColor = Color.Gray;
            Caption.bounds.X = -30;
            Caption.bounds.Y = 40;
            Caption.bounds.Width = 100;
            Caption.bounds.Height = 100;

            e.SceneGraph.Add(Caption);
            GenHorizontalLineAndLabel(HMAoVal, 
                Color.Blue, 
                string.Format(
                    "Ханты-Мансийский автономный округ - Югра {0:N2}, рублей за {1}",
                    HMAoVal, 
                    dictUnits[ComboFood.SelectedValue].ToLower()),
                e,
                true);  
            

        }

        #endregion

        // --------------------------------------------------------------------

        #region Заполнение словарей и выпадающих списков параметров


        Dictionary<string, string> Unamefood = new Dictionary<string, string>();

        protected void FillComboFood(string queryName)
        {
            // Загрузка списка продуктов
            dtFood = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtFood);
            // Закачку придется делать через словарь
            Dictionary<string, int> dictFood = new Dictionary<string, int>();
            dictUnits = new Dictionary<string, string>();
            
            for (int row = 0; row < dtFood.Rows.Count; ++row)
            {
                string food_group = dtFood.Rows[row][1].ToString();
                string food = dtFood.Rows[row][2].ToString().Replace("Рыба мороженная неразделенная","Рыба мороженная неразделанная");
                string unit = dtFood.Rows[row][4].ToString();
                AddPairToDictionary(dictFood, food_group, 0);
                AddPairToDictionary(dictFood, food, 1);
                dictUnits.Add(food, unit.Replace("илограмм", "г."));

                Unamefood.Add(food, dtFood.Rows[row]["[Measures].[Uname]"].ToString());

                

            }
            if (!Page.IsPostBack)
            {
                ComboFood.FillDictionaryValues(dictFood);
            }
        }

        Dictionary<string, string> KeyAndUNameComboDate = new Dictionary<string, string>();
        Dictionary<string, string> KeyAndUNameYearComboDate = new Dictionary<string, string>();
        Dictionary<string, string> KeyAndUNamePrevComboDate = new Dictionary<string, string>();
        Dictionary<string, DoubleString> KeyDisplayAndCaption = new Dictionary<string, DoubleString>();
        Dictionary<string, DoubleString> KeyDisplayAndCaptionSclon = new Dictionary<string, DoubleString>();

        class DoubleString
        {
            public string FirstString;
            public string SecondString;

            public DoubleString(string F, string S)
            {
                this.FirstString = F;
                this.SecondString = S;
            }
        }

        protected void FillComboDate(string queryName)
        {
            // Загрузка списка актуальных дат
            string PrevDate = string.Empty;

            string Prev_Key_sclon = "";
            string Key_sclon = "";

            string Key = "";
            string year_ = "";
            string year__ = "";
            string prev_ = "";


            dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            // Закачку придется делать через словарь

            //-----------------------------------------
            Dictionary<string, int> dictDate = new Dictionary<string, int>();
            for (int row = 0; row < dtDate.Rows.Count; ++row)
            {
                string year = dtDate.Rows[row][0].ToString();
                string month = dtDate.Rows[row][3].ToString();
                string day = dtDate.Rows[row][4].ToString();

                Key = month + " " + year + " года";
                Key_sclon = CRHelper.RusMonthDat(CRHelper.MonthNum(month)) + " " + year + " года";

                if (AddPairToDictionary(dictDate, year + " год", 0))
                {
                    year_ = dtDate.Rows[row]["[Measures].[uniqueName]"].ToString();
                    year__ = Key;
                }

                AddPairToDictionary(dictDate, month + " " + year + " года", 1);

                //AddPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);

                KeyDisplayAndCaption.Add(Key, new DoubleString(prev_, year__));

                KeyDisplayAndCaptionSclon.Add(Key, new DoubleString(Prev_Key_sclon, Key_sclon));




                KeyAndUNameComboDate.Add(
                     Key,
                     dtDate.Rows[row]["[Measures].[uniqueName]"].ToString());

                KeyAndUNamePrevComboDate.Add(
                        Key,
                        PrevDate);

                KeyAndUNameYearComboDate.Add(
                    Key,
                    year_);

                PrevDate = dtDate.Rows[row]["[Measures].[uniqueName]"].ToString();
                prev_ = Key;
                Prev_Key_sclon = Key_sclon;
            }

            if (!Page.IsPostBack)
            {
                ComboDate.FillDictionaryValues(dictDate);

                ComboDate.SelectLastNode();
            }
        }

        protected bool AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Функции-полезняшки преобразования и все такое

        public string StringToMDXDate(string str)
        {
            string template = "[Период].[День].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
            string[] dateElements = str.Split(' ');
            int year = Convert.ToInt32(dateElements[2]);
            string month = CRHelper.RusMonth(CRHelper.MonthNum(dateElements[1]));
            int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
            int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
            int day = Convert.ToInt32(dateElements[0]);
            return String.Format(template, year, halfYear, quarter, month, day);
        }

        public string StringToMDXFood(string foodGroup, string food)
        {
            string template = "[Организации].[Товары и услуги].[Все товары и услуги].[Продовольственные товары].[{0}].[{1}]";
            return String.Format(template, foodGroup, food);
        }

        public string MDXDateToShortDateString(string MDXDateString)
        {
            string[] separator = { "].[" };
            string[] dateElements = MDXDateString.Split(separator, StringSplitOptions.None);
            string template = "{0}.{1}.{2} г.";
            string day = dateElements[7].Replace("]", String.Empty);
            day = day.Length == 1 ? "0" + day : day;
            string month = CRHelper.MonthNum(dateElements[6]).ToString();
            month = month.Length == 1 ? "0" + month : month;
            string year = dateElements[3];
            return String.Format(template, day, month, year);
            return MDXDateString;
        }

        public bool isPreviousMonth(string firstMonth, string secondMonth)
        {
            int MonthNumDelta = CRHelper.MonthNum(firstMonth) - CRHelper.MonthNum(secondMonth);
            return ((MonthNumDelta == 1) || (MonthNumDelta == 11));
        }

        public string getMonthFromString(string date)
        {
            string[] dateElements = date.Split(' ');
            return dateElements[1];
        }

        public string getYearFromString(string date)
        {
            string[] dateElements = date.Split(' ');
            return dateElements[2];
        }

        public string getYearDate(string date)
        {
            string[] dateElements = date.Split(' ');
            return String.Format("31 января {0} года", dateElements[2]);
        }

        public string replaceMonth(string date)
        {
            string[] dateElements = date.Split(' ');
            int monthIndex = CRHelper.MonthNum(dateElements[1]);
            int year = Convert.ToInt32(dateElements[2]);
            string newMonth = String.Empty;
            if (monthIndex == 1)
            {
                newMonth = "декабря";
                --year;
            }
            else
            {
                newMonth = CRHelper.RusMonthGenitive(monthIndex - 1);
            }
            return String.Format("{0} {1} {2} года", CRHelper.MonthLastDay(CRHelper.MonthNum(newMonth)), newMonth, year);
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

        private static double MedianValue(DataTable dt, int medianValueColumn)
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

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section4 = report.AddSection();
            ISection section3 = report.AddSection();

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout, GridLabel.Text, section1);
            ReportPDFExporter1.Export(UltraChart, labelChart.Text, section2);

            ReportPDFExporter1.Export(SpeedDeviationChart, labelChart0.Text, section4);

            section3.PagePaddings = section2.PagePaddings;
            section3.PageMargins = section2.PageMargins;
            section3.PageBorders = section2.PageBorders;
            section3.PageSize = new PageSize(section2.PageSize.Height, section2.PageSize.Width);
            section3.PageOrientation = PageOrientation.Landscape;
            IText title = section3.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(labelMap.Text);

            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            DundasMap.Height = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.5));
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            section3.AddImage(img);
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
            Worksheet sheet4 = workbook.Worksheets.Add("Диаграмма 2");
            Worksheet sheet3 = workbook.Worksheets.Add("Карта");

            SetExportGridParams(headerLayout.Grid);

            ReportExcelExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Left;
            ReportExcelExporter1.TitleStartRow = 0;

            ReportExcelExporter1.Export(headerLayout, sheet1, 4);
            sheet1.Rows[0].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            sheet1.Rows[0].Height = 600;
            sheet1.Columns[0].Width = sheet1.Columns[0].Width + 1600;
            sheet1.Rows[1].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            sheet1.Rows[1].Height = 550;

            sheet1.Rows[3].Cells[0].Value = GridLabel.Text;



            ReportExcelExporter1.WorksheetTitle = String.Empty;
            ReportExcelExporter1.WorksheetSubTitle = String.Empty;

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.7);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);
            ReportExcelExporter1.Export(UltraChart, labelChart.Text, sheet2, 1);
            sheet2.MergedCellsRegions.Clear();

            SpeedDeviationChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.7);
            SpeedDeviationChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);
            ReportExcelExporter1.Export(SpeedDeviationChart, labelChart0.Text, sheet4, 1);
            sheet4.MergedCellsRegions.Clear();
            sheet4.MergedCellsRegions.Add(0, 0, 0, 20);

            sheet3.Rows[0].Cells[0].Value = labelMap.Text;
            sheet3.Rows[0].Cells[0].CellFormat.Font.Name = "Verdana";
            sheet3.Rows[0].Cells[0].CellFormat.Font.Height = 220;
            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.7);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5);
            ReportExcelExporter.MapExcelExport(sheet3.Rows[1].Cells[0], DundasMap);
            sheet1.MergedCellsRegions.Add(3, 0, 3, 10);

            sheet1.Rows[3].Cells[0].CellFormat.Font.Name = sheet1.Rows[1].Cells[0].CellFormat.Font.Name;
            sheet1.Rows[3].Cells[0].CellFormat.Font.Height = sheet1.Rows[1].Cells[0].CellFormat.Font.Height;

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

        private void SetExportGridParams(UltraWebGrid grid)
        {
            string exportFontName = "Verdana";
            int fontSize = 10;
            double coeff = 1;


            //grid.Columns.Add("Безымяный столбик");
            //foreach (UltraGridRow Row in grid.Rows)
            //{
            //    if (Row.Index % 3 == 0)
            //    {
            //        Row.Cells.FromKey("Безымяный столбик").Value = "Значние";
            //        Row.NextRow.Cells.FromKey("Безымяный столбик").Value = "Темп прироста";
            //        Row.NextRow.NextRow.Cells.FromKey("Безымяный столбик").Value = "Абсолютное отклонение";
            //    }
            //}
            //headerLayout = new GridHeaderLayout(UltraWebGrid);

            //GridHeaderCell _ = headerLayout.AddCell("Наименование МО");
            //_.AddCell("");
            //_.AddCell("");
            //GridHeaderCell header = headerLayout.AddCell("Средняя розничная цена, рубль");
            //header.AddCell(yearDateText);
            //header.AddCell(previousDateText);
            //header.AddCell(selectedDateText);
            //headerLayout.AddCell("Ранг");

            //header = headerLayout.AddCell("Динамика к предыдущему отчетному периоду");

            //header.AddCell("Абсолютное отклонение, рубль");
            //header.AddCell("Темп прироста, %");

            //header = headerLayout.AddCell("Динамика за период с начала года");

            //header.AddCell("Абсолютное отклонение, рубль");
            //header.AddCell("Темп прироста, %");

            //headerLayout.ApplyHeaderInfo();

            //grid.Columns.FromKey("Безымяный столбик").Move(1);
            //grid.Columns.FromKey("Безымяный столбик").Width = 180;


            foreach (UltraGridColumn column in grid.Columns)
            {
                column.Width = Convert.ToInt32(column.Width.Value * coeff);
                column.CellStyle.Font.Name = exportFontName;
                column.Header.Style.Font.Name = exportFontName;
                column.CellStyle.Font.Size = fontSize;
                column.Header.Style.Font.Size = fontSize;
            }
        }

        #endregion
    }
}
