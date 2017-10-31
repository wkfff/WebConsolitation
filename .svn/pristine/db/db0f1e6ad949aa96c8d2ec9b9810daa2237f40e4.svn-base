using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;

namespace Krista.FM.Server.Dashboards.reports.RG_0001_0001
{
    public partial class Default : CustomReportPage
    {
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
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }

        #region Поля

        private DataTable dtIndicartorComments = new DataTable();
        private DataTable dtGrid = new DataTable();
        private string formatString = "N0";
        private string measure = String.Empty;

        #endregion

        #region Параметры запроса

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса



            #endregion
            
            UltraWebGrid.Width = IsSmallResolution ? CRHelper.GetGridWidth(MinScreenWidth + 20) : CRHelper.GetGridWidth(MinScreenWidth - 30);
            UltraWebGrid.Height = IsSmallResolution ? CRHelper.GetGridHeight(MinScreenHeight - 300) : CRHelper.GetGridHeight(MinScreenHeight - 195);

            DundasMap1.Width = CRHelper.GetChartWidth(MinScreenWidth - 25);
            DundasMap1.Height = CRHelper.GetChartHeight(MinScreenHeight * 0.78);

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
                       

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.33 - 55);
            UltraChart2.Height = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.23 - 55);

            #region Настройка диаграммы

            SetupRadarChart(UltraChart, true);
            UltraChart.DataBinding += new EventHandler(UltraChart_DataBinding);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            SetupRadarChart(UltraChart1, false);
            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

            SetupRadarChart(UltraChart3, false);
            UltraChart3.DataBinding += new EventHandler(UltraChart3_DataBinding);
            UltraChart3.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart3_FillSceneGraph);

            SetupRadarChart(UltraChart4, false);
            UltraChart4.DataBinding += new EventHandler(UltraChart4_DataBinding);
            UltraChart4.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart4_FillSceneGraph);

            SetupLegendChart();

            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 10);
            #endregion
           
            LoadIndicators();
        }

        private void SetupLegendChart()
        {
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
            UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);
            UltraChart2.Legend.Visible = true;
            AddLineAppearencesUltraChart(UltraChart2);

            UltraChart2.Legend.Location = LegendLocation.Bottom;
            UltraChart2.Legend.SpanPercentage = 100;
            UltraChart2.Legend.Font = new Font("Verdana", 10);
            UltraChart2.Border.Thickness = 0;
            
            UltraChart2.Tooltips.Display = TooltipDisplay.Never;

            UltraChart2.Tooltips.Font.Name = "Courier New";
            UltraChart2.Tooltips.Font.Bold = true;

            UltraChart2.Axis.X.Labels.FontColor = Color.Transparent;
            UltraChart2.Axis.X.Labels.SeriesLabels.FontColor = Color.Transparent;
            UltraChart2.Axis.X.StripLines.PE.Fill = Color.Transparent;
            UltraChart2.Axis.Y.StripLines.PE.Fill = Color.Transparent;
            UltraChart2.Axis.X.LineColor = Color.Transparent;
            UltraChart2.Axis.Y.LineColor = Color.Transparent;
            UltraChart2.Axis.Y.MajorGridLines.Visible = false;
            UltraChart2.Axis.Y.MinorGridLines.Visible = false;

            UltraChart2.Axis.X.MajorGridLines.Visible = false;
            UltraChart2.Axis.X.MinorGridLines.Visible = false;
        }

        private void SetupRadarChart(UltraChart chart, bool main)
        {
            int minusValue = main ? -50 : 50;
            chart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.33 - minusValue);
            chart.Height = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.33 - minusValue);

            chart.ChartType = ChartType.RadarChart;
            chart.Data.SwapRowsAndColumns = true;
            chart.Data.RowLabelsColumn = 0;
            chart.Data.UseRowLabelsColumn = false;

            chart.RadarChart.ColorFill = true;
            chart.RadarChart.LineThickness = 3;
            chart.RadarChart.LineEndCapStyle = LineCapStyle.Round;
            chart.RadarChart.NullHandling = NullHandling.Zero;

            chart.Tooltips.FormatString = "";
            chart.Tooltips.Display = TooltipDisplay.Never;

            chart.Axis.Y.LineThickness = 1;
            chart.Axis.Y.LineDrawStyle = LineDrawStyle.Solid;
            chart.Axis.Y.LineColor = Color.Transparent;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            chart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            chart.Legend.Visible = false;
            AddLineAppearencesUltraChart(chart);
        }

        private void LoadIndicators()
        {
            string filePath = HttpContext.Current.Server.MapPath("~/reports/rg_0001_0001/Default.Settings.xml");

            DataSet ds = new DataSet();
            ds.ReadXml(filePath, XmlReadMode.Auto);

            dtIndicartorComments = ds.Tables["Indicator"];
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            WebAsyncPanel.AddRefreshTarget(UltraWebGrid);
            WebAsyncPanel.AddLinkedRequestTrigger(detail.ClientID);

            if (!Page.IsPostBack)
            {
                detail.Checked = false;

                FillComboIndicators();
                ComboIndicator.Title = "Показатель";
                ComboIndicator.Width = IsSmallResolution ? 500 : 500;
                ComboIndicator.SetСheckedState("Комплексная (интегральная) оценка эффективности ОИВ", true);
                ComboIndicator.ParentSelect = true;
                UserParams.Filter.Value = "Объем валового регионального продукта на одного жителя";
            }

            UserParams.Filter.Value = ComboIndicator.SelectedValue;

            SetupMap();

            Page.Title = "Оценка эффективности деятельности ОИВ субъектов РФ, входящих в Уральский федеральный округ";
            PageTitle.Text = "Оценка эффективности деятельности ОИВ субъектов РФ, входящих в Уральский федеральный округ";
            PageSubTitle.Text = "Оценка эффективности деятельности органов исполнительной власти субъектов РФ, входящих в Уральский федеральный округ, согласно оперативному краткому перечню индикаторов эффективности.";

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            UltraChart.DataBind();
            UltraChart1.DataBind();
            UltraChart2.DataBind();
            UltraChart3.DataBind();
            UltraChart4.DataBind();

            // заполняем карту формами
            string regionStr = "УрФО";
            DundasMap1.Shapes.Clear();
            DundasMap1.LoadFromShapeFile(Server.MapPath(string.Format("../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);
            // заполняем карту данными
            FillMapData(DundasMap1);
        }
                

        #region Настройка карты

        private void SetupMap()
        {
            DundasMap1.Meridians.Visible = false;
            DundasMap1.Parallels.Visible = false;
            DundasMap1.ZoomPanel.Visible = !IsSmallResolution;
            DundasMap1.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap1.NavigationPanel.Visible = !IsSmallResolution;
            DundasMap1.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap1.Viewport.EnablePanning = true;

            // добавляем поля для раскраски
            DundasMap1.ShapeFields.Clear();
            DundasMap1.ShapeFields.Add("Name");
            DundasMap1.ShapeFields["Name"].Type = typeof(string);
            DundasMap1.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap1.ShapeFields.Add("IndicatorValue");
            DundasMap1.ShapeFields["IndicatorValue"].Type = typeof(double);
            DundasMap1.ShapeFields["IndicatorValue"].UniqueIdentifier = false;

            // добавляем легенду
            DundasMap1.Legends.Clear();
            Legend legend1 = new Legend("CompleteLegend");
            legend1.Visible = true;
            legend1.Dock = PanelDockStyle.Left;
            legend1.BackColor = Color.White;
            legend1.BackSecondaryColor = Color.Gainsboro;
            legend1.BackGradientType = GradientType.DiagonalLeft;
            legend1.BackHatchStyle = MapHatchStyle.None;
            legend1.BorderColor = Color.Gray;
            legend1.BorderWidth = 1;
            legend1.BorderStyle = MapDashStyle.Solid;
            legend1.BackShadowOffset = 4;
            legend1.TextColor = Color.Black;
            legend1.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend1.AutoFitText = true;
            //legend1.Size.Width = 500f;
            legend1.AutoFitMinFontSize = 7;

            string query = DataProvider.GetQueryText("RG_0001_0002_measures");

            DataTable dtMeasures = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtMeasures);

            bool direct = true;
            string ruleLegengText = "#FROMVALUE{N1} - #TOVALUE{N1}";
            string element = String.Empty;

            if (dtMeasures.Rows.Count > 0)
            {
                measure = dtMeasures.Rows[0][0].ToString().ToLower();
                direct = dtMeasures.Rows[0][2].ToString() == "0";

                switch (measure)
                {
                    case "процент":
                        {
                            measure = "%";
                            formatString = "N2";
                            ruleLegengText = "#FROMVALUE{N0}% - #TOVALUE{N0}%";
                            break;
                        }
                    case "рубль":
                        {
                            measure = "руб.";
                            if (ComboIndicator.SelectedValue != "Оборот розничной торговли")
                            {
                                formatString = "N2";
                            }
                            break;
                        }
                    case "место":
                        {
                            measure = "мест";
                            formatString = "N0";
                            break;
                        }
                    case "год":
                        {
                            measure = "лет";
                            formatString = "N2";
                            break;
                        }
                    case "на 1000 человек":
                        {
                            formatString = "N2";
                            break;
                        }
                    case "на 10000 человек":
                        {
                            formatString = "N2";
                            break;
                        }
                    case "раз (отношение)":
                        {
                            formatString = "N3";
                            ruleLegengText = "#FROMVALUE{N2} - #TOVALUE{N2}";
                            break;
                        }
                    case "раз":
                        {
                            formatString = "N3";
                            ruleLegengText = "#FROMVALUE{N2} - #TOVALUE{N2}";
                            break;
                        }
                    case "доля":
                        {
                            formatString = "N3";
                            ruleLegengText = "#FROMVALUE{N2} - #TOVALUE{N2}";
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }


                string prefix = "за ";
                string filter = String.Format("ID='{0}'", dtMeasures.Rows[0][3]);
                DataRow[] rows = dtIndicartorComments.Select(filter);
                if (rows.Length == 1)
                {
                    if (rows[0]["period"] != DBNull.Value &&
                        rows[0]["period"].ToString() != String.Empty)
                    {
                        prefix = rows[0]["period"].ToString();
                    }
                    if (rows[0]["precision"] != DBNull.Value &&
                        rows[0]["precision"].ToString() != String.Empty)
                    {
                        formatString = rows[0]["precision"].ToString();
                    }
                }

                element = GetElement(dtMeasures.Rows[0][1].ToString(), prefix);
            }

            // добавляем правила раскраски
            DundasMap1.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "IndicatorValueRule";
            rule.Category = String.Empty;
            rule.ShapeField = "IndicatorValue";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = direct ? Color.Red : Color.Green;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = direct ? Color.Green : Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";

            rule.LegendText = ruleLegengText;
            DundasMap1.ShapeRules.Add(rule);

            legend1.Title = String.Format("{0}", ComboIndicator.SelectedValue);
            DundasMap1.Legends.Add(legend1);

            string measureInfo = measure != String.Empty ? String.Format(", {0}", measure) : String.Empty;
            mapElementCaption.Text = String.Format("{0} {1}{2}", UserParams.Filter.Value, element, measureInfo);
            //mapElementCaption.Text = String.Format("{0} по субъектам УрФО", UserParams.Filter.Value);
        }

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <returns>найденная форма</returns>
        public static Shape FindMapShape(MapControl map, string patternValue)
        {
            string subject = patternValue.Replace("область", "обл.");
            subject = subject.Replace("автономный округ", "АО");

            ArrayList shapeList = map.Shapes.Find(subject, true, false);
            if (shapeList.Count > 0)
            {
                return (Shape)shapeList[0];
            }
            return null;
        }

        public void FillMapData(MapControl map)
        {
            if (map == null)
            {
                return;
            }

            switch (ComboIndicator.SelectedValue)
            {
                case "Комплексная (интегральная) оценка эффективности ОИВ":
                    {
                        foreach (KeyValuePair<string, double> pair in complexValues)
                        {
                            FillMapFromDictionary(map, pair, rankComplexValues[pair.Key], "Комплексная (интегральная) оценка эффективности ОИВ");
                        }
                        break;
                    }
                case "Индикаторы, характеризующие социальное направление":
                    {
                        foreach (KeyValuePair<string, double> pair in markValues1)
                        {
                            FillMapFromDictionary(map, pair, rankValues1[pair.Key], "Индикаторы, характеризующие социальное направление");
                        }
                        break;
                    }
                case "Индикаторы, характеризующие экономическое направление":
                    {
                        foreach (KeyValuePair<string, double> pair in markValues2)
                        {
                            FillMapFromDictionary(map, pair, rankValues2[pair.Key], "Индикаторы, характеризующие экономическое направление");
                        }
                        break;
                    }
                case "Индикаторы, характеризующие финансовое направление":
                    {
                        foreach (KeyValuePair<string, double> pair in markValues3)
                        {
                            FillMapFromDictionary(map, pair, rankValues3[pair.Key], "Индикаторы, характеризующие финансовое направление");
                        }
                        break;
                    }
                default:
                    {
                        string query = DataProvider.GetQueryText("RG_0001_0001_map");

                        DataTable dtMap1 = new DataTable();
                        DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtMap1);

                        if (dtMap1.Rows.Count == 0)
                        {
                            return;
                        }

                        map.Symbols.Clear();

                        foreach (DataRow row in dtMap1.Rows)
                        {
                            // заполняем карту данными
                            if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                            {
                                string regionName = row[0].ToString();

                                if (RegionsNamingHelper.IsSubject(regionName))
                                {
                                    Shape shape = FindMapShape(map, regionName);
                                    if (shape != null)
                                    {
                                        if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                                        {
                                            double indicatorValue = Convert.ToDouble(row[1]);

                                            shape["Name"] = regionName;
                                            shape["IndicatorValue"] = indicatorValue;
                                            shape.ToolTip = string.Format("#NAME \nранг по УрФО {0} \n{1} {2} \n{3}", row[2], ComboIndicator.SelectedValue, indicatorValue.ToString(formatString), measure);
                                            shape.TextAlignment = ContentAlignment.MiddleCenter;
                                            shape.Offset.X = -15;
                                            if (!IsSmallResolution)
                                            {
                                                shape.Offset.Y = -30;
                                            }

                                            shape.Text =
                                                string.Format("{0}\n{1} {2}", IsSmallResolution ? RegionsNamingHelper.ShortName(regionName) : shape.Name, indicatorValue.ToString(formatString),
                                                              row[3].ToString().Replace(" (отношение)", String.Empty));
                                        }

                                        shape.BorderWidth = 2;
                                        shape.TextColor = Color.Black;
                                        shape.Font = new Font("Verdana", 10);
                                        shape.TextVisibility = TextVisibility.Shown;
                                    }
                                }
                            }
                        }
                        break;
                    }
            }

        }

        private void FillMapFromDictionary(MapControl map, KeyValuePair<string, double> pair, double rank, string name)
        {
            string regionName = pair.Key;

            if (RegionsNamingHelper.IsSubject(regionName))
            {
                Shape shape = FindMapShape(map, regionName);
                if (shape != null)
                {

                    double indicatorValue = pair.Value;

                    shape["Name"] = regionName;
                    shape["IndicatorValue"] = indicatorValue;
                    shape.ToolTip = string.Format("#NAME \nранг по УрФО {0} \n{1} {2}", rank, name, indicatorValue.ToString("N3"));
                    shape.TextAlignment = ContentAlignment.MiddleCenter;
                    shape.Offset.X = -15;
                    if (!IsSmallResolution)
                    {
                        shape.Offset.Y = -30;
                    }

                    shape.Text =
                        string.Format("{0}\n{1} {2}", IsSmallResolution ? RegionsNamingHelper.ShortName(regionName) : shape.Name, indicatorValue.ToString("N3"),
                                      measure.Replace(" (отношение)", String.Empty));


                    shape.BorderWidth = 2;
                    shape.TextColor = Color.Black;
                    shape.Font = new Font("Verdana", 10);
                    shape.TextVisibility = TextVisibility.Shown;
                }
            }
        }
        #endregion

        #region Обработчики грида

        private Dictionary<string, double> markValues1 = InitMarkDictionary();
        private Dictionary<string, double> markValues2 = InitMarkDictionary();
        private Dictionary<string, double> markValues3 = InitMarkDictionary();

        private Dictionary<string, double> complexValues = InitMarkDictionary();

        private static Dictionary<string, double> InitMarkDictionary()
        {
            Dictionary<string, double> markValues = new Dictionary<string, double>();

            markValues.Add("Курганская область", 0);
            markValues.Add("Свердловская область", 0);
            markValues.Add("Тюменская область", 0);
            markValues.Add("Ханты-Мансийский автономный округ", 0);
            markValues.Add("Челябинская область", 0);
            markValues.Add("Ямало-Ненецкий автономный округ", 0);

            return markValues;
        }

        private static Dictionary<string, int> InitComboDictionary()
        {
            Dictionary<string, int> markValues = new Dictionary<string, int>();

            markValues.Add("Комплексная (интегральная) оценка эффективности ОИВ", 0);
            markValues.Add("Индикаторы, характеризующие социальное направление", 0);
            markValues.Add("Индикаторы, характеризующие экономическое направление", 0);
            markValues.Add("Индикаторы, характеризующие финансовое направление", 0);

            return markValues;
        }

        private Dictionary<string, double> rankValues1;
        private Dictionary<string, double> rankValues2;
        private Dictionary<string, double> rankValues3;
        private Dictionary<string, double> rankComplexValues;

        private static Dictionary<string, double> FillRanksDictionary(Dictionary<string, double> dictionary)
        {
            Dictionary<string, double> rankValues = InitMarkDictionary();

            double maxDouble = Double.MinValue;
            string maxRegion = String.Empty;
            foreach (KeyValuePair<string, double> value in dictionary)
            {
                if (value.Value > maxDouble && rankValues[value.Key] == 0)
                {
                    maxDouble = value.Value;
                    maxRegion = value.Key;
                }
            }
            rankValues[maxRegion] = 1;

            maxDouble = Double.MinValue;
            maxRegion = String.Empty;
            foreach (KeyValuePair<string, double> value in dictionary)
            {
                if (value.Value > maxDouble && rankValues[value.Key] == 0)
                {
                    maxDouble = value.Value;
                    maxRegion = value.Key;
                }
            }
            rankValues[maxRegion] = 2;
            maxDouble = Double.MinValue;
            maxRegion = String.Empty;
            foreach (KeyValuePair<string, double> value in dictionary)
            {
                if (value.Value > maxDouble && rankValues[value.Key] == 0)
                {
                    maxDouble = value.Value;
                    maxRegion = value.Key;
                }
            }
            rankValues[maxRegion] = 3;
            maxDouble = Double.MinValue;
            maxRegion = String.Empty;
            foreach (KeyValuePair<string, double> value in dictionary)
            {
                if (value.Value > maxDouble && rankValues[value.Key] == 0)
                {
                    maxDouble = value.Value;
                    maxRegion = value.Key;
                }
            }
            rankValues[maxRegion] = 4;
            maxDouble = Double.MinValue;
            maxRegion = String.Empty;
            foreach (KeyValuePair<string, double> value in dictionary)
            {
                if (value.Value > maxDouble && rankValues[value.Key] == 0)
                {
                    maxDouble = value.Value;
                    maxRegion = value.Key;
                }
            }
            rankValues[maxRegion] = 5;
            maxDouble = Double.MinValue;
            maxRegion = String.Empty;
            foreach (KeyValuePair<string, double> value in dictionary)
            {
                if (value.Value > maxDouble && rankValues[value.Key] == 0)
                {
                    maxDouble = value.Value;
                    maxRegion = value.Key;
                }
            }
            rankValues[maxRegion] = 6;


            return rankValues;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(String.Format("RG_0001_0001_Grid"));
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            for (int rowCount = 0; rowCount < dtGrid.Rows.Count; rowCount++)
            {
                if (dtGrid.Rows[rowCount][27] != DBNull.Value &&
                    dtGrid.Rows[rowCount][26] != DBNull.Value)
                {
                    double weigh = Convert.ToDouble(dtGrid.Rows[rowCount][27]);
                    string direction = dtGrid.Rows[rowCount][26].ToString();

                    for (int columnsCount = 3; columnsCount < 20; columnsCount += 3)
                    {
                        string region = dtGrid.Columns[columnsCount].ColumnName.Split(';')[0];
                        if (dtGrid.Rows[rowCount][columnsCount] != DBNull.Value)
                        {
                            double value = Convert.ToDouble(dtGrid.Rows[rowCount][columnsCount]);
                            value = value * weigh;
                            switch (direction)
                            {
                                case "1":
                                    {
                                        markValues1[region] = markValues1[region] + value;
                                        break;
                                    }
                                case "2":
                                    {
                                        markValues2[region] = markValues2[region] + value;
                                        break;
                                    }
                                case "3":
                                    {
                                        markValues3[region] = markValues3[region] + value;
                                        break;
                                    }
                            }
                        }

                    }
                }
            }
            dtGrid.Columns[26].ColumnName = "Направление";
            dtGrid.AcceptChanges();

            complexValues["Курганская область"] = markValues1["Курганская область"] * 3 + markValues2["Курганская область"] * 3 + markValues3["Курганская область"] * 4;
            complexValues["Свердловская область"] = markValues1["Свердловская область"] * 3 + markValues2["Свердловская область"] * 3 + markValues3["Свердловская область"] * 4;
            complexValues["Тюменская область"] = markValues1["Тюменская область"] * 3 + markValues2["Тюменская область"] * 3 + markValues3["Тюменская область"] * 4;
            complexValues["Ханты-Мансийский автономный округ"] = markValues1["Ханты-Мансийский автономный округ"] * 3 + markValues2["Ханты-Мансийский автономный округ"] * 3 + markValues3["Ханты-Мансийский автономный округ"] * 4;
            complexValues["Челябинская область"] = markValues1["Челябинская область"] * 3 + markValues2["Челябинская область"] * 3 + markValues3["Челябинская область"] * 4;
            complexValues["Ямало-Ненецкий автономный округ"] = markValues1["Ямало-Ненецкий автономный округ"] * 3 + markValues2["Ямало-Ненецкий автономный округ"] * 3 + markValues3["Ямало-Ненецкий автономный округ"] * 4;

            DataTable source = dtGrid.Clone();

            DataRow newRow = source.NewRow();
            newRow[0] = "Комплексная (интегральная) оценка эффективности ОИВ";

            rankComplexValues = FillRanksDictionary(complexValues);

            newRow[3] = complexValues["Курганская область"];
            newRow[6] = complexValues["Свердловская область"];
            newRow[9] = complexValues["Тюменская область"];
            newRow[12] = complexValues["Ханты-Мансийский автономный округ"];
            newRow[15] = complexValues["Челябинская область"];
            newRow[18] = complexValues["Ямало-Ненецкий автономный округ"];

            newRow[2] = rankComplexValues["Курганская область"];
            newRow[5] = rankComplexValues["Свердловская область"];
            newRow[8] = rankComplexValues["Тюменская область"];
            newRow[11] = rankComplexValues["Ханты-Мансийский автономный округ"];
            newRow[14] = rankComplexValues["Челябинская область"];
            newRow[17] = rankComplexValues["Ямало-Ненецкий автономный округ"];
            
            source.Rows.Add(newRow);

            rankValues1 = FillRanksDictionary(markValues1);

            newRow = source.NewRow();
            newRow[0] = "Индикаторы, характеризующие социальное направление";

            newRow[3] = markValues1["Курганская область"];
            newRow[6] = markValues1["Свердловская область"];
            newRow[9] = markValues1["Тюменская область"];
            newRow[12] = markValues1["Ханты-Мансийский автономный округ"];
            newRow[15] = markValues1["Челябинская область"];
            newRow[18] = markValues1["Ямало-Ненецкий автономный округ"];

            newRow[2] = rankValues1["Курганская область"];
            newRow[5] = rankValues1["Свердловская область"];
            newRow[8] = rankValues1["Тюменская область"];
            newRow[11] = rankValues1["Ханты-Мансийский автономный округ"];
            newRow[14] = rankValues1["Челябинская область"];
            newRow[17] = rankValues1["Ямало-Ненецкий автономный округ"];

            newRow["Нулл; Код"] = 1;

            source.Rows.Add(newRow);

            DataRow[] rows = dtGrid.Select("Направление = '1'");
            foreach (DataRow row in rows)
            {
                source.ImportRow(row);
            }

            rankValues2 = FillRanksDictionary(markValues2);

            newRow = source.NewRow();
            newRow[0] = "Индикаторы, характеризующие экономическое направление";

            newRow[3] = markValues2["Курганская область"];
            newRow[6] = markValues2["Свердловская область"];
            newRow[9] = markValues2["Тюменская область"];
            newRow[12] = markValues2["Ханты-Мансийский автономный округ"];
            newRow[15] = markValues2["Челябинская область"];
            newRow[18] = markValues2["Ямало-Ненецкий автономный округ"];

            newRow[2] = rankValues2["Курганская область"];
            newRow[5] = rankValues2["Свердловская область"];
            newRow[8] = rankValues2["Тюменская область"];
            newRow[11] = rankValues2["Ханты-Мансийский автономный округ"];
            newRow[14] = rankValues2["Челябинская область"];
            newRow[17] = rankValues2["Ямало-Ненецкий автономный округ"];

            newRow["Нулл; Код"] = 2;

            source.Rows.Add(newRow);

            rows = dtGrid.Select("Направление = '2'");
            foreach (DataRow row in rows)
            {
                source.ImportRow(row);
            }

            rankValues3 = FillRanksDictionary(markValues3);

            newRow = source.NewRow();
            newRow[0] = "Индикаторы, характеризующие финансовое направление";

            newRow[3] = markValues3["Курганская область"];
            newRow[6] = markValues3["Свердловская область"];
            newRow[9] = markValues3["Тюменская область"];
            newRow[12] = markValues3["Ханты-Мансийский автономный округ"];
            newRow[15] = markValues3["Челябинская область"];
            newRow[18] = markValues3["Ямало-Ненецкий автономный округ"];

            newRow[2] = rankValues3["Курганская область"];
            newRow[5] = rankValues3["Свердловская область"];
            newRow[8] = rankValues3["Тюменская область"];
            newRow[11] = rankValues3["Ханты-Мансийский автономный округ"];
            newRow[14] = rankValues3["Челябинская область"];
            newRow[17] = rankValues3["Ямало-Ненецкий автономный округ"];

            newRow["Нулл; Код"] = 3;

            source.Rows.Add(newRow);

            rows = dtGrid.Select("Направление = '3'");
            foreach (DataRow row in rows)
            {
                source.ImportRow(row);
            }


            ((UltraWebGrid)sender).DataSource = source;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraGridColumn col = e.Layout.Grid.Columns[24];
            e.Layout.Grid.Columns.RemoveAt(24);
            e.Layout.Grid.Columns.Insert(0, col);

            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.FilterOptionsDefault.AllowRowFiltering = RowFiltering.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(40, 1280);
            e.Layout.Bands[0].Columns[0].Header.Caption = "Код";
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[0], "N0");
            e.Layout.Bands[0].Columns[1].Width = IsSmallResolution ? CRHelper.GetColumnWidth(160, 1280) : CRHelper.GetColumnWidth(195, 1280);
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 3].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 4].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 5].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 6].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 7].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 8].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 9].Hidden = true;
            
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1 || i == e.Layout.Bands[0].Columns.Count)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 2;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count - 9; i = i + 3)
            {
                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = IsSmallResolution ? RegionsNamingHelper.ShortName(e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0].Trim(' ')) : e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[1], "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, e.Layout.Bands[0].Columns[i + 1].Header.Caption.Split(';')[1], "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, e.Layout.Bands[0].Columns[i + 2].Header.Caption.Split(';')[1], "");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                ch.RowLayoutColumnInfo.SpanY = 1;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                e.Layout.Bands[0].HeaderLayout.Add(ch);

                // CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "N0");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "N3");

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(65, 1280);
                e.Layout.Bands[0].Columns[i + 1].Hidden = true;
                e.Layout.Bands[0].Columns[i + 2].Width = CRHelper.GetColumnWidth(80, 1280);
            }
            //}
        }

        private int shownRows = 0;

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int someValue = 0;
            if (e.Row.Cells[0].Value != null &&
                e.Row.Cells[0].Value.ToString() == "307")
            {
                someValue++;
            }
            int otherValue = someValue;

            if (e.Row.Cells[1].Value == null ||
                e.Row.Cells[1].Value.ToString() == string.Empty ||
                e.Row.Cells[1].Value.ToString().Contains("Индикаторы, характеризующие ") ||
                 e.Row.Cells[1].Value.ToString().Contains("Комплексная (интегральная) оценка эффективности ОИВ"))
            {
                SetupAssessValues(e);
                return;
            }
            if (e.Row.Cells[e.Row.Cells.Count - 1].Value == null ||
                e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString() == string.Empty ||
                e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString().Split('.').Length < 3)
            {
                e.Row.Hidden = true;
                return;
            }
            else if (!detail.Checked)
            {
                e.Row.Hidden = true;
                return;
            }

            string measureLocal = e.Row.Cells[e.Row.Cells.Count - 5].Value.ToString().ToLower();
            string formatStringLocal = "N2";
            switch (measureLocal)
            {
                case "процент":
                    {
                        measureLocal = "%";
                        formatStringLocal = "N2";
                        break;
                    }
                case "рубль":
                    {
                        formatStringLocal = "N2";
                        measureLocal = "руб.";
                        break;
                    }
                case "место":
                    {
                        measureLocal = "мест";
                        formatStringLocal = "N0";
                        break;
                    }
                case "год":
                    {
                        measureLocal = "лет";
                        break;
                    }
                case "на 1000 человек":
                    {
                        formatStringLocal = "N2";
                        break;
                    }
                case "раз (отношение)":
                    {
                        formatStringLocal = "N3";
                        break;
                    }
                case "раз":
                    {
                        formatStringLocal = "N3";
                        break;
                    }
                case "доля":
                    {
                        formatStringLocal = "N3";
                        break;
                    }
                default:
                    {
                        measureLocal = e.Row.Cells[e.Row.Cells.Count - 5].Value.ToString().ToLower();
                        break;
                    }
            }

            string element;
            string prefix = "за ";
            string filter = String.Format("ID='{0}'", e.Row.Cells[0].Value);
            DataRow[] rows = dtIndicartorComments.Select(filter);
            if (rows.Length == 1)
            {
                if (rows[0]["period"] != DBNull.Value &&
                    rows[0]["period"].ToString() != String.Empty)
                {
                    prefix = rows[0]["period"].ToString();
                }
                if (rows[0]["precision"] != DBNull.Value &&
                    rows[0]["precision"].ToString() != String.Empty)
                {
                    formatStringLocal = rows[0]["precision"].ToString();
                }
            }
            element = GetElement(e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString(), prefix);

            string indicatorNumber = string.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                indicatorNumber = e.Row.Cells[0].Value.ToString();
            }

            e.Row.Cells[1].Value = String.Format("{0} {1}, {2}", e.Row.Cells[1].Value, element, measureLocal);
            for (int i = 2; i < e.Row.Cells.Count - 7; i += 3)
            {
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(e.Row.Cells[i].Value.ToString(), out value))
                    {
                        if (value < 0 ||
                            (measureLocal == "%" && e.Row.Cells[0].Value.ToString().ToLower().Contains("темп") && value < 100))
                        {
                            e.Row.Cells[i].Style.ForeColor = Color.Red;
                        }
                    }

                    if ((Request.Form.AllKeys.Length == 0) ||
                        (Request.Form["__EVENTTARGET"] != null && !Request.Form["__EVENTTARGET"].Contains("excel")))
                    {
                        e.Row.Cells[i].Value = value.ToString(formatStringLocal);
                        if (e.Row.Cells[i + 2] != null && e.Row.Cells[i + 2].Value != null)
                        {
                            decimal integralValue;
                            if (decimal.TryParse(e.Row.Cells[i + 2].Value.ToString(), out integralValue))
                            {
                                e.Row.Cells[i + 2].Value = String.Format("{0}<br/>ранг {1}", integralValue.ToString("N3"), e.Row.Cells[i + 1].Value);
                            } 
                        }
                        else
                        {
                            e.Row.Cells[i + 2].Value = String.Format("ранг {0}", e.Row.Cells[i + 1].Value);
                        }
                    }

                    if (e.Row.Cells[i + 1].Value != null && e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToInt32(e.Row.Cells[i + 1].Value) == 1)
                        {
                            e.Row.Cells[i + 2].Style.BackgroundImage = "~/images/starYellowBB.png";
                        }
                        else if (e.Row.Cells[e.Row.Cells.Count - 6].Value != null && e.Row.Cells[e.Row.Cells.Count - 6].Value.ToString() != string.Empty
                            && Convert.ToInt32(e.Row.Cells[e.Row.Cells.Count - 6].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                        {
                            e.Row.Cells[i + 2].Style.BackgroundImage = "~/images/starGrayBB.png";
                        }
                    }
                    e.Row.Cells[i + 2].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    e.Row.Cells[i + 1].Style.HorizontalAlign = HorizontalAlign.Right;
                }

                e.Row.Cells[i].Style.BorderDetails.WidthLeft = 2;
                e.Row.Cells[i + 2].Style.BorderDetails.WidthRight = 2;
            }
            e.Row.Cells[1].Style.BorderDetails.WidthRight = 2;
            shownRows++;

            if (rows.Length == 1 &&
                rows[0]["link"] != DBNull.Value &&
                rows[0]["link"].ToString() != String.Empty)
            {
                e.Row.Cells[1].Value = string.Format("{0} <a href='{1}'>>>></a>", e.Row.Cells[1].Value, rows[0]["link"]).Replace(", неизвестные данные", " ");
            }

            if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString().Length == 3)
            {
                e.Row.Cells[0].Value = String.Format("{0}.{1}{2}", e.Row.Cells[0].Value.ToString()[0], e.Row.Cells[0].Value.ToString()[1].ToString() == "0" ? String.Empty : e.Row.Cells[0].Value.ToString()[1].ToString(), e.Row.Cells[0].Value.ToString()[2]);
            }
        }

        private void SetupAssessValues(RowEventArgs e)
        {
            for (int i = 2; i < e.Row.Cells.Count - 7; i += 3)
            {
                if (e.Row.Cells[i + 1].Value != null && e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                {
                    if (Convert.ToInt32(e.Row.Cells[i + 1].Value) == 1)
                    {
                        e.Row.Cells[i + 2].Style.BackgroundImage = "~/images/starYellowBB.png";
                    }
                    else if (6 == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                    {
                        e.Row.Cells[i + 2].Style.BackgroundImage = "~/images/starGrayBB.png";
                    }
                }
                e.Row.Cells[i + 2].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                e.Row.Cells[i + 1].Style.HorizontalAlign = HorizontalAlign.Right;

                if ((Request.Form.AllKeys.Length == 0) ||
                    (Request.Form["__EVENTTARGET"] != null && !Request.Form["__EVENTTARGET"].Contains("excel")))
                {
                    if (e.Row.Cells[i + 2] != null && e.Row.Cells[i + 2].Value != null)
                    {
                        decimal integralValue;
                        if (decimal.TryParse(e.Row.Cells[i + 2].Value.ToString(), out integralValue))
                        {
                            e.Row.Cells[i + 2].Value = String.Format("{0}<br/>ранг {1}", integralValue.ToString("N3"), e.Row.Cells[i + 1].Value);
                        }
                    }
                }

                e.Row.Cells[i].Style.BorderDetails.WidthLeft = 2;
                e.Row.Cells[i + 2].Style.BorderDetails.WidthRight = 2;
                e.Row.Cells[i + 2].Style.Font.Bold = true;
            }

            e.Row.Cells[0].Style.Font.Bold = true;
            e.Row.Cells[1].Style.Font.Bold = true;

            e.Row.Cells[1].Style.BorderDetails.WidthRight = 2;
        }

        private static string GetElement(string period, string prefix)
        {
            string element;
            string[] periodDimension = period.ToString().Split('.');
            int value;
            if (periodDimension[periodDimension.Length - 1].Length < 5 &&
                Int32.TryParse(periodDimension[periodDimension.Length - 1].Trim('[').Trim(']'), out value))
            {
                element = String.Format("{1}{0:dd.MM.yyyy}", new DateTime(Convert.ToInt32(periodDimension[3].Trim('[').Trim(']')), CRHelper.MonthNum(periodDimension[6].Trim('[').Trim(']')), value), prefix);
            }
            else if (period.Contains("квартала"))
            {
                if (prefix == "за 1-" && periodDimension[5].Contains("1"))
                {
                    prefix = "за ";
                }
                element = String.Format("{2}{0} квартал {1} года", periodDimension[5].Trim('[').Trim(']').Replace("Квартал", String.Empty), periodDimension[3].Trim('[').Trim(']'), prefix);
            }
            else if (period.Contains("Квартал"))
            {
                if (prefix == "за январь-" && periodDimension[6].Contains("Январь"))
                {
                    prefix = "за ";
                }
                element = String.Format("{2}{0} {1} года", periodDimension[6].Trim('[').Trim(']').ToLower(), periodDimension[3].Trim('[').Trim(']'), prefix);
            }
            else
            {
                element = String.Format("{1}{0} год", periodDimension[3].Trim('[').Trim(']'), prefix);
            }
            return element;
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (shownRows < 18)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        #endregion

        #region Обработчики диаграммы

        private void AddLineAppearencesUltraChart(UltraChart chart)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = true;
            chart.ColorModel.Skin.PEs.Clear();

            for (int i = 1; i <= 7; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                pe.Fill = color;
                pe.Stroke = color;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.StrokeWidth = 2;
                chart.ColorModel.Skin.PEs.Add(pe);
                chart.Border.Thickness = 0;
            }
        }

        private static Color GetColor(int i)
        {
            Color color = Color.White;
            switch (i)
            {
                case 1:
                    {
                        // Курганская
                        color = Color.Green;
                        break;
                    }
                case 2:
                    {
                        // Свердловска
                        color = Color.Gold;
                        break;
                    }
                case 3:
                    {
                        // Тюменская
                        color = Color.Black;
                        break;
                    }
                case 4:
                    {
                        // ХМАО
                        color = Color.LightSlateGray;
                        break;
                    }
                case 5:
                    {
                        // Челябинская
                        color = Color.Red;
                        break;
                    }
                case 6:
                    {
                        // ЯНАО
                        color = Color.Blue;
                        break;
                    }
                case 7:
                    {
                        color = Color.DarkViolet;
                        break;
                    }
            }
            return color;
        }
               
        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            DataTable dtChart2 = CreateDataTable();

            int j = 1;
            for (int i = 0; i < dtChart2.Rows.Count; i += 2)
            {
                dtChart2.Rows[i][j] = complexValues[RegionsNamingHelper.FullName(dtChart2.Rows[i][0].ToString())];
                dtChart2.Rows[i + 1][j] = complexValues[RegionsNamingHelper.FullName(dtChart2.Rows[i + 1][0].ToString())];
                j++;
            }

            dtChart2.AcceptChanges();
            UltraChart.DataSource = dtChart2;
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            DataTable dtChart2 = CreateDataTable();

            int j = 1;
            for (int i = 0; i < dtChart2.Rows.Count; i += 2 )
            {
                dtChart2.Rows[i][j] = markValues1[RegionsNamingHelper.FullName(dtChart2.Rows[i][0].ToString())];
                dtChart2.Rows[i + 1][j] = markValues1[RegionsNamingHelper.FullName(dtChart2.Rows[i + 1][0].ToString())];
                j++;
            }

            dtChart2.AcceptChanges();
            UltraChart1.DataSource = dtChart2;
        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            DataTable dtChart2 = CreateDataTable();

            int j = 1;
            for (int i = 0; i < dtChart2.Rows.Count; i += 2)
            {
                dtChart2.Rows[i][j] = markValues2[RegionsNamingHelper.FullName(dtChart2.Rows[i][0].ToString())];
                dtChart2.Rows[i + 1][j] = markValues2[RegionsNamingHelper.FullName(dtChart2.Rows[i + 1][0].ToString())];
                j++;
            }

            dtChart2.AcceptChanges();
            UltraChart3.DataSource = dtChart2;
        }

        protected void UltraChart4_DataBinding(object sender, EventArgs e)
        {
            DataTable dtChart2 = CreateDataTable();

            int j = 1;
            for (int i = 0; i < dtChart2.Rows.Count; i += 2)
            {
                dtChart2.Rows[i][j] = markValues3[RegionsNamingHelper.FullName(dtChart2.Rows[i][0].ToString())];
                dtChart2.Rows[i + 1][j] = markValues3[RegionsNamingHelper.FullName(dtChart2.Rows[i + 1][0].ToString())];
                j++;
            }

            dtChart2.AcceptChanges();
            UltraChart4.DataSource = dtChart2;
        }

        private static DataTable CreateDataTable()
        {
            DataTable dtChart2 = new DataTable();

            dtChart2.Columns.Add(new DataColumn("Субъект", typeof(string)));
            for (int i = 1; i <= 6; i++)
            {
                dtChart2.Columns.Add(new DataColumn(i.ToString(), typeof (double)));
            }

            DataRow row = dtChart2.NewRow();
            row[0] = RegionsNamingHelper.ShortName("Курганская область");
            dtChart2.Rows.Add(row);
            row = dtChart2.NewRow();
            row[0] = RegionsNamingHelper.ShortName("Курганская область");
            dtChart2.Rows.Add(row);

            row = dtChart2.NewRow();
            row[0] = RegionsNamingHelper.ShortName("Свердловская область");
            dtChart2.Rows.Add(row);
            row = dtChart2.NewRow();
            row[0] = RegionsNamingHelper.ShortName("Свердловская область");
            dtChart2.Rows.Add(row);
            
            row = dtChart2.NewRow();
            row[0] = RegionsNamingHelper.ShortName("Тюменская область");
            dtChart2.Rows.Add(row);
            row = dtChart2.NewRow();
            row[0] = RegionsNamingHelper.ShortName("Тюменская область");
            dtChart2.Rows.Add(row);

            row = dtChart2.NewRow();
            row[0] = RegionsNamingHelper.ShortName("Ханты-Мансийский автономный округ");
            dtChart2.Rows.Add(row);
            row = dtChart2.NewRow();
            row[0] = RegionsNamingHelper.ShortName("Ханты-Мансийский автономный округ");
            dtChart2.Rows.Add(row);

            row = dtChart2.NewRow();
            row[0] = RegionsNamingHelper.ShortName("Челябинская область");
            dtChart2.Rows.Add(row);
            row = dtChart2.NewRow();
            row[0] = RegionsNamingHelper.ShortName("Челябинская область");
            dtChart2.Rows.Add(row);

            row = dtChart2.NewRow();
            row[0] = RegionsNamingHelper.ShortName("Ямало-Ненецкий автономный округ");
            dtChart2.Rows.Add(row);
            row = dtChart2.NewRow();
            row[0] = RegionsNamingHelper.ShortName("Ямало-Ненецкий автономный округ");
            dtChart2.Rows.Add(row);
            return dtChart2;
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            DataTable dtChart3 = new DataTable();

            dtChart3.Columns.Add(new DataColumn("Субъект", typeof(string)));
            dtChart3.Columns.Add(new DataColumn("Комплексная (интегральная) оценка эффективности ОИВ", typeof(double)));
            dtChart3.Columns.Add(new DataColumn(" ", typeof(double)));

            DataRow row = dtChart3.NewRow();
            row[0] = "Курганская область";
            dtChart3.Rows.Add(row);

            row = dtChart3.NewRow();
            row[0] = "Свердловская область";
            dtChart3.Rows.Add(row);

            row = dtChart3.NewRow();
            row[0] = "Тюменская область";
            dtChart3.Rows.Add(row);

            row = dtChart3.NewRow();
            row[0] = "Ханты-Мансийский автономный округ";
            dtChart3.Rows.Add(row);

            row = dtChart3.NewRow();
            row[0] = "Челябинская область";
            dtChart3.Rows.Add(row);

            row = dtChart3.NewRow();
            row[0] = "Ямало-Ненецкий автономный округ";
            dtChart3.Rows.Add(row);

            for (int i = 0; i < dtChart3.Rows.Count; i++)
            {
                dtChart3.Rows[i][1] = complexValues[dtChart3.Rows[i][0].ToString()];
                dtChart3.Rows[i][2] = complexValues[dtChart3.Rows[i][0].ToString()];
            }

            dtChart3.AcceptChanges();
            UltraChart2.DataSource = dtChart3;
        }
        
        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            RedrawChart(e, complexValues);
        }

        protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            RedrawChart(e, markValues1);
        }

        protected void UltraChart3_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            RedrawChart(e, markValues2);
        }

        protected void UltraChart4_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            RedrawChart(e, markValues3);
        }

        private static void RedrawChart(FillSceneGraphEventArgs e, Dictionary<string, double> marks)
        {
            Dictionary<string, double> x = InitMarkDictionary();
            Dictionary<string, double> y = InitMarkDictionary();

            int l = 1;
            bool q = false;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Polygon)
                {
                    if (q)
                    {
                        primitive.PE.Fill = GetColor(l);
                        primitive.PE.FillStopColor = GetColor(l);
                        l++;
                    }
                    q = !q;
                } 
                else if (primitive is Text)
                {
                    Text text = (Text)primitive;
                    string key = RegionsNamingHelper.FullName(text.GetTextString());
                    if (x.ContainsKey(key))
                    {
                        if (x[key] == 0)
                        {
                            x[key] = text.bounds.X;
                            y[key] = text.bounds.Y;
                            text.Visible = false;
                        }
                        else
                        {
                            text.bounds.X = (int)Math.Abs((x[key] + text.bounds.X) / 2);
                            text.bounds.Y = (int)Math.Abs((y[key] + text.bounds.Y) / 2);
                            text.SetTextString(text.GetTextString() + "\n" + marks[key].ToString("N3"));
                            text.labelStyle.VerticalAlign = StringAlignment.Near;
                            text.labelStyle.HorizontalAlign = StringAlignment.Near;
                            text.bounds.Width = 60;
                            text.bounds.Height = 55;
                            text.labelStyle.FontSizeBestFit = false;
                            text.labelStyle.Font = new Font("Verdana", 10);
                            text.PE.Fill = Color.Black;
                        }
                    }
                    else
                    {
                        text.Visible = false;
                    }
                } 
                //else if (primitive is Polyline)
                //{
                //    // Do nothing
                //}
                //else
                //{
                //    primitive.Visible = false;
                //}
            }
        }

        protected void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive.Path != null &&
                    !primitive.Path.Contains("Legend"))
                {
                    primitive.Visible = false;
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int width = 100;
            e.CurrentWorksheet.Columns[0].Width = 50 * 37;
            e.CurrentWorksheet.Columns[1].Width = 250 * 37;
            e.CurrentWorksheet.Columns[2].Width = width * 37;
            e.CurrentWorksheet.Columns[3].Width = width * 37;
            e.CurrentWorksheet.Columns[4].Width = width * 37;
            e.CurrentWorksheet.Columns[5].Width = width * 37;
            e.CurrentWorksheet.Columns[6].Width = width * 37;
            e.CurrentWorksheet.Columns[7].Width = width * 37;
            e.CurrentWorksheet.Columns[8].Width = width * 37;
            e.CurrentWorksheet.Columns[9].Width = width * 37;
            e.CurrentWorksheet.Columns[10].Width = width * 37;
            e.CurrentWorksheet.Columns[11].Width = width * 37;
            e.CurrentWorksheet.Columns[12].Width = width * 37;
            e.CurrentWorksheet.Columns[13].Width = width * 37;
            e.CurrentWorksheet.Columns[14].Width = width * 37;

            e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "0";
            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "";
            //e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            //e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            //e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            //e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            //e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            //e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[10].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[11].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[12].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[13].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[14].CellFormat.FormatString = "#,##0.00";
            //e.CurrentWorksheet.Columns[15].CellFormat.FormatString = "#,##0.00";

            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = shownRows + 5;

            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Center;

                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Height = 20 * 37;
            }

            // расставляем стили у ячеек
            for (int i = 2; i < columnCount; i += 2)
            {
                for (int j = 4; j < rowsCount; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.FormatString = "#,##0.####;[Red]-#,##0.####";
                    double value;
                    if (e.CurrentWorksheet.Rows[j].Cells[i].Value != null && Double.TryParse(e.CurrentWorksheet.Rows[j].Cells[i].Value.ToString(), out value))
                    {
                        e.CurrentWorksheet.Rows[j].Cells[i].Value = value;
                    }

                    e.CurrentWorksheet.Rows[j].Cells[i + 1].CellFormat.Alignment = HorizontalCellAlignment.Right;
                    e.CurrentWorksheet.Rows[j].Cells[i + 1].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[j].Cells[i + 1].CellFormat.FormatString = "#,##0";

                    int value1;
                    if (e.CurrentWorksheet.Rows[j].Cells[i + 1].Value != null && Int32.TryParse(e.CurrentWorksheet.Rows[j].Cells[i + 1].Value.ToString(), out value1))
                    {
                        e.CurrentWorksheet.Rows[j].Cells[i + 1].Value = value1;
                    }

                    //e.CurrentWorksheet.Workbook.Styles.
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            foreach (UltraGridRow row in UltraWebGrid.Rows)
            {
                if (row.Cells[1] != null)
                {
                    row.Cells[1].Value = row.Cells[1].Value.ToString().Replace("<a href='../../reports/MFRF_0003_0003/Default.aspx'>>>></a>", string.Empty);
                }
            }

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            foreach (UltraGridRow row in UltraWebGrid.Rows)
            {
                if (row.Cells[1] != null)
                {
                    row.Cells[1].Value = row.Cells[1].Value.ToString().Replace("<a href='../../reports/MFRF_0003_0003/Default.aspx'>>>></a>", string.Empty);
                }
            }

            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(mapElementCaption.Text);

            Infragistics.Documents.Reports.Graphics.Image img1 = UltraGridExporter.GetImageFromMap(DundasMap1);
            e.Section.AddImage(img1);
        }

        #endregion

        private void FillComboIndicators()
        {
            DataTable dtIndicators = new DataTable();
            string query = DataProvider.GetQueryText("RG_0001_0002_Indicators");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dtIndicators);

            Dictionary<string, int> regions = new Dictionary<string, int>();


            regions.Add("Комплексная (интегральная) оценка эффективности ОИВ", 0);

            regions.Add("Индикаторы, характеризующие социальное направление", 0);

            DataRow[] rows = dtIndicators.Select("Направление = '1'");
            AddIndicators(regions, rows);

            rankValues2 = FillRanksDictionary(markValues2);

            regions.Add("Индикаторы, характеризующие экономическое направление", 0);

            rows = dtIndicators.Select("Направление = '2'");
            AddIndicators(regions, rows);

            rankValues3 = FillRanksDictionary(markValues3);

            regions.Add("Индикаторы, характеризующие финансовое направление", 0);

            rows = dtIndicators.Select("Направление = '3'");
            AddIndicators(regions, rows);

            ComboIndicator.FillDictionaryValues(regions);
        }

        private static void AddIndicators(Dictionary<string, int> regions, DataRow[] rows)
        {
            foreach (DataRow row in rows)
            {
                if (row[2] != DBNull.Value &&
                    row[2].ToString() != String.Empty)
                {
                    regions.Add(row[0].ToString(), 1);
                }
            }
        }
    }
}