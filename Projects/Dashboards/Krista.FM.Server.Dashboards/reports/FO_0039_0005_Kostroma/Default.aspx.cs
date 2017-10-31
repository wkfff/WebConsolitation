using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core.MemberDigests;
using ContentAlignment=System.Drawing.ContentAlignment;

namespace Krista.FM.Server.Dashboards.reports.FO_0039_0005_Kostroma
{
    public enum ColoringRule
    {
        // сложная раскраска диапазоном
        SimpleRangeColoring,
        // сложная раскраска диапазоном
        ComplexRangeColoring,
        // раскраска диапазонов от 0 до 1
        DistributionColoring,
        // дискретная раскраска для 0 и 1
        DiscreteColoring,
        // раскраска только желтым
        YellowSolidColoring
    }

    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtMap;
        private DataTable dtIndicatorDetail;
        private int firstYear = 2009;
        private int endYear = 2011;

        private string selectedIndicatorName;
        private int selectedQuarterIndex;
        private string indicatorName;
        private double beginQualityLimit;
        private double endQualityLimit;
        private string indicatorPeriod;

        private ColoringRule coloringRule;
        private Color beginMapColor;
        private Color middleMapColor;
        private Color endMapColor;
        private Color nullEmptyColor;
        private string nullEmptyText;
        private string mapFormatString;
        private string mapLegendCaption;

        private double calloutOffsetY = 0;

        #endregion

        private static MemberAttributesDigest valueIndicatorDigest;
        private static MemberAttributesDigest evaluationIndicatorDigest;
        
        // имя папки с картами региона
        private string mapFolderName;
        // масшбтаб карты
        private double mapZoomValue;

        private bool IsYearCompare
        {
            get { return selectedQuarterIndex == 4; }
        }

        private bool IsQualityDegree
        {
            get { return indicatorName == "Итоговая оценка"; }
        }

        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }

        public bool ValueSelected
        {
            get { return MeasureButtonList.SelectedIndex == 1; }
        }

        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
        // выбранный индикатор
        private CustomParam selectedIndicator;
        // выбраная мера
        private CustomParam selectedMeasure;
        // уровень районов
        private CustomParam regionsLevel;

        #endregion
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            mapFolderName = RegionSettingsHelper.Instance.GetPropertyValue("MapFolderName");
            mapZoomValue = Convert.ToDouble(RegionSettingsHelper.Instance.GetPropertyValue("MapZoomValue"));
            calloutOffsetY = Convert.ToDouble(RegionSettingsHelper.Instance.GetPropertyValue("MapCalloutOffsetY"));

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 190);

            #region Инициализация параметров запроса

            regionsLevel = UserParams.CustomParam("regions_level");

            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }
            if (selectedIndicator == null)
            {
                selectedIndicator = UserParams.CustomParam("selected_indicator");
            }
            if (selectedMeasure == null)
            {
                selectedMeasure = UserParams.CustomParam("selected_measure");
            }

            #endregion

            UltraGridExporter1.ExcelExportButton.Visible = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;оценки&nbsp;качества&nbsp;МР(ГО)";
            CrossLink1.NavigateUrl = "~/reports/FO_0039_0001_Kostroma/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Динамика&nbsp;результатов&nbsp;оценки";
            CrossLink2.NavigateUrl = "~/reports/FO_0039_0002_Kostroma/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Рейтинг&nbsp;МР(ГО)";
            CrossLink3.NavigateUrl = "~/reports/FO_0039_0003_Kostroma/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Диаграмма&nbsp;динамики&nbsp;по&nbsp;отд.показателю";
            CrossLink4.NavigateUrl = "~/reports/FO_0039_0007_Kostroma/Default.aspx";

            CrossLink5.Visible = true;
            CrossLink5.Text = "Диаграмма&nbsp;динамики&nbsp;результатов&nbsp;оценки";
            CrossLink5.NavigateUrl = "~/reports/FO_0039_0006_Kostroma/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0039_0005_Kostroma_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                string quarter = "Квартал 4";
                if (dtDate.Rows.Count > 0)
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                    quarter = dtDate.Rows[0][2].ToString();
                }

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Оценка качества";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillDateQuarters());
                ComboQuarter.SetСheckedState(GetParamQuarter(quarter), true);

                valueIndicatorDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0039_0005_QualityValueIndicatorList");
                evaluationIndicatorDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0039_0005_QualityEvaluationIndicatorList");

                ComboQualityEvaluationIndicator.Title = "Показатель";
                ComboQualityEvaluationIndicator.Width = 300;
                ComboQualityEvaluationIndicator.MultiSelect = false;
                ComboQualityEvaluationIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(evaluationIndicatorDigest.UniqueNames, evaluationIndicatorDigest.MemberLevels));
                ComboQualityEvaluationIndicator.SetСheckedState("Итоговая оценка", true);

                ComboQualityValueIndicator.Title = "Показатель";
                ComboQualityValueIndicator.Width = 300;
                ComboQualityValueIndicator.MultiSelect = false;
                ComboQualityValueIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(valueIndicatorDigest.UniqueNames, valueIndicatorDigest.MemberLevels));
                ComboQualityValueIndicator.SetСheckedState("P1", true);
            }

            Page.Title = String.Format("Картограмма результатов оценки по отдельному показателю");
            PageTitle.Text = Page.Title;

            selectedQuarterIndex = ComboQuarter.SelectedIndex + 1;

            PageSubTitle.Text = (!IsYearCompare)
                ? String.Format("{0}.{1}", ComboQuarter.SelectedValue, ComboYear.SelectedValue)
                : String.Format("по итогам {0} года", ComboYear.SelectedValue);
            
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", selectedQuarterIndex);

            selectedMeasure.Value = ValueSelected ? "Значение" : "Оценка показателя";

            if (IsYearCompare)
            {
                selectedPeriod.Value = String.Format("[{0}]", UserParams.PeriodYear.Value);
            }
            else
            {
                selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);
            }

            if (ValueSelected)
            {
                valueComboTD.Visible = true;
                evaluationComboTD.Visible = false;

                selectedIndicatorName = ComboQualityValueIndicator.SelectedValue;
                selectedIndicator.Value = valueIndicatorDigest.GetMemberUniqueName(ComboQualityValueIndicator.SelectedValue);
                ComboQualityEvaluationIndicator.SetСheckedState(selectedIndicatorName, true);
            }
            else
            {
                valueComboTD.Visible = false;
                evaluationComboTD.Visible = true;

                selectedIndicatorName = ComboQualityEvaluationIndicator.SelectedValue;
                selectedIndicator.Value = evaluationIndicatorDigest.GetMemberUniqueName(ComboQualityEvaluationIndicator.SelectedValue);
                ComboQualityValueIndicator.SetСheckedState(selectedIndicatorName, true);
            }
                        
            IndicatorDetailDataBind();
            mapElementCaption.Text = indicatorName == selectedIndicatorName
                        ? String.Format("Показатель «{0}»", indicatorName)
                        : String.Format("Показатель «{0}» ({1})", indicatorName, selectedIndicatorName);

            DundasMap.Shapes.Clear();
            if (Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("MapLoadFromBin")))
            {
                DundasMap.Serializer.Format = Dundas.Maps.WebControl.SerializationFormat.Binary;
                DundasMap.Serializer.Load((Server.MapPath(string.Format("../../maps/Субъекты/{0}/{0}_выноски.bin", mapFolderName))));
                SetMapSettings();
            }
            else
            {
                //DundasMap.ShapeFields.Clear();
                DundasMap.ShapeFields.Add("IndicatorName");
                DundasMap.ShapeFields["IndicatorName"].Type = typeof(string);
                DundasMap.ShapeFields["IndicatorName"].UniqueIdentifier = true;
                DundasMap.ShapeFields.Add("IndicatorValue");
                DundasMap.ShapeFields["IndicatorValue"].Type = typeof(double);
                DundasMap.ShapeFields["IndicatorValue"].UniqueIdentifier = false;

                SetMapSettings();
                AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);
                AddMapLayer(DundasMap, mapFolderName, "Города", CRHelper.MapShapeType.Towns);
                AddMapLayer(DundasMap, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);
            }

            // заполняем карту данными
            FillMapData();

        }

        /// <summary>
        /// Получить элемент параметра по значению классификатора
        /// </summary>
        /// <param name="classQuarter">элемент классификатора</param>
        /// <returns>значение параметра</returns>
        private static string GetParamQuarter(string classQuarter)
        {
            switch (classQuarter)
            {
                case "Квартал 1":
                    {
                        return "по состоянию на 01.04";
                    }
                case "Квартал 2":
                    {
                        return "по состоянию на 01.07";
                    }
                case "Квартал 3":
                    {
                        return "по состоянию на 01.10";
                    }
                case "Квартал 4":
                case "Данные года":
                    {
                        return "по итогам года";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        #region Обработчики карты

        public void SetMapSettings()
        {
            nullEmptyColor = Color.LightSkyBlue;
            nullEmptyText = (!IsYearCompare && indicatorPeriod.ToLower() == "ежегодно")
                ? "Нет данных, т.к. показатель расчитывается только по итогам года"
                : "Нет данных";
            
            switch (selectedIndicatorName)
            {
                case "Ранг":
                    {
                        beginMapColor = Color.Green;
                        middleMapColor = Color.Yellow;
                        endMapColor = Color.Red;
                        mapFormatString = "N0";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        mapLegendCaption = "Рейтинг МО";
                        nullEmptyText = "Не участвует в рейтинге";
                        break;
                    }
                case "Средняя оценка по МО":
                    {
                        beginMapColor = Color.LightSkyBlue;
                        middleMapColor = Color.LightSkyBlue;
                        endMapColor = Color.LightSkyBlue;
                        mapFormatString = "N2";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        mapLegendCaption = "Средняя оценка качества";
                        break;
                    }
                case "Итоговая оценка":
                    {
                        beginMapColor = Color.Red;
                        middleMapColor = Color.Yellow;
                        endMapColor = Color.Green;
                        mapFormatString = "N2";
                        coloringRule = ColoringRule.ComplexRangeColoring;
                        mapLegendCaption = "Степень качества";
                        break;
                    }
                case "Р6":
                case "Р7":
                case "Р10":
                case "Р11":
                case "Р13":
                case "Р14":
                    {
                        beginMapColor = Color.Green;
                        middleMapColor = Color.Yellow;
                        endMapColor = Color.Red;
                        mapFormatString = "N2";
                        coloringRule = ValueSelected ? ColoringRule.YellowSolidColoring : ColoringRule.DistributionColoring;
                        mapLegendCaption = ValueSelected ? "Значение" : "Оценка";
                        break;
                    }
                default:
                    {
                        beginMapColor = Color.Green;
                        middleMapColor = Color.Yellow;
                        endMapColor = Color.Red;
                        mapFormatString = "N2";
                        coloringRule = ValueSelected ? ColoringRule.YellowSolidColoring : ColoringRule.DiscreteColoring;
                        mapLegendCaption = ValueSelected ? "Значение" : "Оценка";
                        break;
                    }
            }

            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = (float)mapZoomValue;

            DundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Right;

            // добавляем легенду
            DundasMap.Legends.Clear();

            Legend legend = new Legend("MapLegend");
            legend.Dock = PanelDockStyle.Right;
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
            legend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = mapLegendCaption;
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(legend);

            if (coloringRule == ColoringRule.SimpleRangeColoring)
            {
                // добавляем правила раскраски
                ShapeRule rule = new ShapeRule();
                rule.Name = "IndicatorRule";
                rule.Category = String.Empty;
                rule.ShapeField = "IndicatorValue";
                rule.DataGrouping = DataGrouping.Optimal;
                rule.ColorCount = 3;
                rule.ColoringMode = ColoringMode.ColorRange;
                rule.FromColor = beginMapColor;
                rule.MiddleColor = middleMapColor;
                rule.ToColor = endMapColor;
                rule.BorderColor = Color.FromArgb(50, Color.Black);
                rule.GradientType = GradientType.None;
                rule.HatchStyle = MapHatchStyle.None;
                rule.ShowInColorSwatch = false;
                rule.ShowInLegend = "MapLegend";
                rule.LegendText = (selectedIndicatorName == "Рейтинг МО")
                    ? String.Format("#FROMVALUE{{{0}}}.00 - #TOVALUE{{{0}}}.00", "N0")
                    : String.Format("#FROMVALUE{{{0}}} - #TOVALUE{{{0}}}", mapFormatString);
                DundasMap.ShapeRules.Add(rule);
            }
            else if (coloringRule == ColoringRule.ComplexRangeColoring)
            {
                if (beginQualityLimit != endQualityLimit)
                {
                    LegendItem item = new LegendItem();
                    item.Text = String.Format("Более {0} {1}", beginQualityLimit.ToString(mapFormatString), IsQualityDegree ? "(I степень)" : String.Empty);
                    item.Color = endMapColor;
                    DundasMap.Legends["MapLegend"].Items.Add(item); 

                    item = new LegendItem();
                    item.Text = String.Format("{0} - {1} {2}", endQualityLimit.ToString(mapFormatString), beginQualityLimit.ToString(mapFormatString), 
                        IsQualityDegree ? "(II степень)" : String.Empty);
                    item.Color = Color.Yellow;
                    DundasMap.Legends["MapLegend"].Items.Add(item);

                     item = new LegendItem();
                    item.Text = String.Format("Менее {0} {1}", endQualityLimit.ToString(mapFormatString), IsQualityDegree ? "(III степень)" : String.Empty);
                    item.Color = beginMapColor;
                    DundasMap.Legends["MapLegend"].Items.Add(item);
                }
                else
                {
                    LegendItem item = new LegendItem();
                    item.Text = String.Format("{0}", beginQualityLimit.ToString(mapFormatString));
                    item.Color = endMapColor;
                    DundasMap.Legends["MapLegend"].Items.Add(item);
                }
           }
            else if (coloringRule == ColoringRule.DistributionColoring)
            {
                LegendItem item = new LegendItem();
                item.Text = "Равна 1";
                item.Color = beginMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = "От 0 до 1";
                item.Color = Color.Yellow;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = "Равна 0";
                item.Color = endMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);
            }
            else if (coloringRule == ColoringRule.DiscreteColoring)
            {
                LegendItem item = new LegendItem();
                item.Text = "Равна 1";
                item.Color = beginMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = "Равна 0";
                item.Color = endMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);
            }
            else if (coloringRule == ColoringRule.YellowSolidColoring)
            {
                DundasMap.Legends["MapLegend"].Visible = false;
            }
        }

        /// <summary>
        /// Является ли форма городом-выноской
        /// </summary>
        /// <param name="shape">форма</param>
        /// <returns>true, если является</returns>
        public static bool IsCalloutTownShape(Shape shape)
        {
            return shape.Layer == CRHelper.MapShapeType.CalloutTowns.ToString();
        }

        /// <summary>
        /// Получение имени формы (с выделением имени из города-выноски)
        /// </summary>
        /// <param name="shape">форма</param>
        /// <returns>имя формы</returns>
        public static string GetShapeName(Shape shape)
        {
            string shapeName = shape.Name;
            if (IsCalloutTownShape(shape) && shape.Name.Split('_').Length > 1)
            {
                shapeName = shape.Name.Split('_')[0];
            }

            return shapeName;
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/Субъекты/{0}/{1}.shp", mapFolder, layerFileName));
            int oldShapesCount = map.Shapes.Count;

            map.LoadFromShapeFile(layerName, "NAME", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <returns>найденные формы</returns>
        public static ArrayList FindMapShape(MapControl map, string patternValue)
        {
            ArrayList shapeList = new ArrayList();
            foreach (Shape shape in map.Shapes)
            {
                if (GetShapeName(shape) == patternValue)
                {
                    shapeList.Add(shape);
                }
            }

            return shapeList;
        }

        private static bool NonNullValueGrid(DataTable dt, int columnIndex)
        {
            if (dt != null && dt.Columns.Count > columnIndex)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        if (row[columnIndex] != DBNull.Value)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }

        public void FillMapData()
        {
            string query = DataProvider.GetQueryText("FO_0039_0005_Kostroma_map");
            dtMap = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,
                                                                             "Наименование муниципального образования",
                                                                             dtMap);
            
            foreach (Shape shape in DundasMap.Shapes)
            {
                shape.Visible = false;
            }

            string valueSeparator = IsMozilla ? ";" : "\n";

            if (!NonNullValueGrid(dtMap, 1))
            {
                if (coloringRule == ColoringRule.SimpleRangeColoring)
                {
                    DundasMap.ShapeRules["IndicatorRule"].ShowInLegend = "";
                }
                else
                {
                    DundasMap.Legends["MapLegend"].Items.Clear();
                }
            }
                       
            if (coloringRule == ColoringRule.SimpleRangeColoring)
            {
                int colorCount = CRHelper.GetMapIntervalCount(dtMap, 1, 3, true);
                if (colorCount == 0)
                {
                    DundasMap.ShapeRules["IndicatorRule"].ColorCount = 1;
                    DundasMap.ShapeRules["IndicatorRule"].LegendText = string.Format("#FROMVALUE{{{0}}}", mapFormatString);
                }
            }

            bool nullValueExists = false;

            foreach (DataRow row in dtMap.Rows)
            {
                string subjectName = row[0].ToString();

                ArrayList shapeList = FindMapShape(DundasMap, subjectName);
                foreach (Shape shape in shapeList)
                {
                    string shapeName = GetShortRegionName(GetShapeName(shape));
                    shape.Visible = true;

                    string valueStr = String.Empty;
                    // заполняем карту данными
                    if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                    {
                        double value = Convert.ToDouble(row[1]);
                        valueStr = value.ToString(mapFormatString);

                        shape["IndicatorName"] = subjectName;
                        shape["IndicatorValue"] = value;
                        shape.ToolTip = string.Format("{0}{1}{2}: #INDICATORVALUE{{{3}}}", subjectName, valueSeparator,
                                                      selectedIndicatorName, mapFormatString);

                        if (coloringRule == ColoringRule.ComplexRangeColoring)
                        {
                            shape.Color = GetQualityColor(value);
                        }
                        else if (coloringRule == ColoringRule.DistributionColoring)
                        {
                            if (value == 1)
                            {
                                shape.Color = beginMapColor;
                            }
                            else if (value == 0)
                            {
                                shape.Color = endMapColor;
                            }
                            else
                            {
                                shape.Color = middleMapColor;
                            }
                        }
                        else if (coloringRule == ColoringRule.DiscreteColoring)
                        {
                            if (value == 1)
                            {
                                shape.Color = beginMapColor;
                            }
                            else if (value == 0)
                            {
                                shape.Color = endMapColor;
                            }
                        }
                        else if (coloringRule == ColoringRule.YellowSolidColoring)
                        {
                            shape.Color = middleMapColor;
                        }
                    }
                    else
                    {
                        shape.Color = nullEmptyColor;
                        shape.ToolTip = string.Format("{0}\n{1}", subjectName, nullEmptyText);
                        shape.Text = shapeName;

                        if (!nullValueExists)
                        {
                            nullValueExists = true;

                            LegendItem nullTextItem = new LegendItem();
                            nullTextItem.Text = nullEmptyText;
                            nullTextItem.Color = nullEmptyColor;
                            DundasMap.Legends["MapLegend"].Items.Add(nullTextItem);
                        }
                    }

                    if (IsCalloutTownShape(shape))
                    {
                        shape.Text = string.Format("{0}\n{1}", shapeName, valueStr);
                        shape.TextVisibility = TextVisibility.Shown;
                        shape.TextAlignment = ContentAlignment.BottomCenter;
                        shape.CentralPointOffset.Y = calloutOffsetY;
                    }
                    else
                    {
                        shape.Text = string.Format("{0}\n{1}", shapeName.Replace(" ", "\n"), valueStr);
                    }
                }
            }
        }

        private static string GetShortRegionName(string fullName)
        {
            string shortName = fullName.Replace("муниципальный район", "МР");
            shortName = shortName.Replace("Муниципальный район", "МР");
            shortName = shortName.Replace("\"", "'");
            shortName = shortName.Replace(" район", " р-н");
            return shortName;
        }

        #endregion

        #region Параметры индикатора

        protected void IndicatorDetailDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0039_0005_indicator_detail");
            dtIndicatorDetail = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtIndicatorDetail);

            indicatorName = GetStringDTValue(dtIndicatorDetail, "Наименование");
            beginQualityLimit = IsYearCompare ? GetDoubleDTValue(dtIndicatorDetail, "Начальная граница интервала для года") : GetDoubleDTValue(dtIndicatorDetail, "Начальная граница интервала для квартала");
            endQualityLimit = IsYearCompare ? GetDoubleDTValue(dtIndicatorDetail, "Конечная граница интервала для года") : GetDoubleDTValue(dtIndicatorDetail, "Конечная граница интервала для квартала");
            indicatorPeriod = GetStringDTValue(dtIndicatorDetail, "Периодичность расчета показателя");
        }

        private Color GetQualityColor(double value)
        {
            if (value > beginQualityLimit)
            {
                return Color.Green;
            }
            if (value <= endQualityLimit)
            {
                return Color.Red;
            }
            return Color.Yellow;
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != String.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        private static string GetStringDTValue(DataTable dt, string columnName)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != String.Empty)
            {
                return dt.Rows[0][columnName].ToString();
            }
            return String.Empty;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = mapElementCaption.Text;

            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraGridExporter.MapExcelExport(e.CurrentWorksheet.Rows[3].Cells[0], DundasMap);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(emptyExportGrid);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            
            Report report = new Report();
            ISection section = report.AddSection();

            UltraGridExporter1.PdfExporter.Export(new UltraWebGrid(), section);
        }
        
        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
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
            
            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(mapElementCaption.Text);

            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
