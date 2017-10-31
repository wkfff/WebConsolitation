using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
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
using ContentAlignment=System.Drawing.ContentAlignment;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.reports.FO_0040_0003
{
    public enum ColoringRule
    {
        // сложная раскраска диапазоном
        SimpleRangeColoring,
        // сложная раскраска диапазоном
        ComplexRangeColoring,
        // положительные/отрицательные
        PositiveNegativeColoring
    }

    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtMap;
        private DataTable dtIndicatorDetail;
        private int firstYear = 2010;
        private int endYear = 2011;

        private string selectedIndicatorName;
        private int selectedQuarterIndex;
        private int selectedYear;
        private string indicatorName;
        private string indicatorFormat;
        private int beginQualityLimit;
        private int endQualityLimit;

        private ColoringRule coloringRule;
        private Color beginMapColor;
        private Color middleMapColor;
        private Color endMapColor;
        private Color nullEmptyColor;
        private string nullEmptyText;
        private string mapFormatString;
        private string mapLegendCaption;
        
        #endregion

        // имя папки с картами региона
        private string mapFolderName;
        // масшбтаб карты
        private double mapZoomValue;
        // сдвиг по вертикали текста городов-выносок карты
        private double mapCalloutOffsetY;

        private bool IsRating
        {
            get { return indicatorName == "Рейтинг"; }
        }

        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;
        // выбранный индикатор
        private CustomParam selectedIndicator;

        #endregion
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            mapFolderName = RegionSettingsHelper.Instance.GetPropertyValue("MapFolderName");
            mapZoomValue = Convert.ToDouble(RegionSettingsHelper.Instance.GetPropertyValue("MapZoomValue"));

            double value = 1;
            mapCalloutOffsetY = value;
            if (Double.TryParse(RegionSettingsHelper.Instance.GetPropertyValue("MapCalloutOffsetY"), NumberStyles.Any, NumberFormatInfo.CurrentInfo, out value))
            {
                mapCalloutOffsetY = value;
            }

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 200);

            #region Инициализация параметров запроса

            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }
            if (selectedIndicator == null)
            {
                selectedIndicator = UserParams.CustomParam("selected_indicator");
            }
 
            #endregion

            UltraGridExporter1.ExcelExportButton.Visible = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;мониторинга&nbsp;качества ";
            CrossLink1.NavigateUrl = "~/reports/FO_0040_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Динамика&nbsp;основных&nbsp;показателей";
            CrossLink2.NavigateUrl = "~/reports/FO_0040_0002/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Диаграмма&nbsp;динамики&nbsp;основных&nbsp;показателей";
            CrossLink3.NavigateUrl = "~/reports/FO_0040_0004/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0040_0003_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillMonitoringQuarters());
                ComboQuarter.SetСheckedState(GetParamQuarter(quarter), true);

                ComboIndicator.Title = "Показатель";
                ComboIndicator.Width = 300;
                ComboIndicator.MultiSelect = false;
                ComboIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillQualityEvaluationIndicatorList(DataDictionariesHelper.QualityEvaluationIndicatorList));
                ComboIndicator.SetСheckedState("Рейтинг", true);
            }

            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedQuarterIndex = ComboQuarter.SelectedIndex + 1;

            Page.Title = String.Format("Картограмма результатов мониторинга по отдельному показателю");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = (selectedQuarterIndex == 4)
                ? String.Format("по итогам {0} года", selectedYear)
                : String.Format("по итогам {0} квартала {1} года", selectedQuarterIndex, selectedYear);
            
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", selectedQuarterIndex);

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);

            selectedIndicatorName = ComboIndicator.SelectedValue;
            selectedIndicator.Value = DataDictionariesHelper.QualityEvaluationIndicatorList[selectedIndicatorName];
                        
            IndicatorDetailDataBind();
            mapElementCaption.Text = indicatorName == selectedIndicatorName
                ? String.Format("Показатель «{0}»", indicatorName)
                : String.Format("Показатель {1} «{0}»", indicatorName, selectedIndicatorName);

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
                        return "мониторинг за 1 квартал";
                    }
                case "Квартал 2":
                    {
                        return "мониторинг за 2 квартал";
                    }
                case "Квартал 3":
                    {
                        return "мониторинг за 3 квартал";
                    }
                case "Квартал 4":
                    {
                        return "мониторинг за 4 квартал (по итогам года)";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        #region Обработчики карты

        private string GetIndicatorFormatString(string indicatorName)
        {

            if (indicatorFormat == "ДА/НЕТ")
            {
                return "N0";
            }
            else
            {
                return "N2";
            }
            //switch (indicatorName)
            //{
            //    case "I (1)":
            //    case "I (2)":
            //    case "I (3)":
            //    case "I (5)":
            //    case "II (2)":
            //    case "II (3)":
            //    case "II (5)":
            //    case "II (6)":
            //    case "II (7)":
            //    case "III (5)":
            //    case "Рейтинг":
            //    case "Средняя оценка":
            //        {
            //            return "N2";
            //        }
            //    default:
            //        {
            //            return "N0";
            //        }
            //}
        }
        
        public void SetMapSettings()
        {
            nullEmptyColor = Color.LightSkyBlue;
            nullEmptyText = "Нет данных";
            
            switch (selectedIndicatorName)
            {
                case "Рейтинг":
                    {
                        beginMapColor = Color.Red;
                        middleMapColor = Color.Yellow;
                        endMapColor = Color.Green;
                        mapFormatString = "N2";
                        coloringRule = ColoringRule.ComplexRangeColoring;
                        mapLegendCaption = "Рейтинг";
                        break;
                    }
                case "Средняя оценка":
                    {
                        beginMapColor = Color.LightSkyBlue;
                        middleMapColor = Color.LightSkyBlue;
                        endMapColor = Color.LightSkyBlue;
                        mapFormatString = "N2";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        mapLegendCaption = "Средняя оценка";
                        break;
                    }
                default:
                    {
                        beginMapColor = Color.Green;
                        middleMapColor =  Color.Yellow;
                        endMapColor = Color.Red;
                        mapFormatString = GetIndicatorFormatString(selectedIndicatorName);
                        coloringRule = ColoringRule.PositiveNegativeColoring;
                        mapLegendCaption = "Оценка";
                        break;
                    }
            }

            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = (float)mapZoomValue;
            DundasMap.Viewport.ViewCenter.X -= 10;

            DundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Right;

            // добавляем легенду
            DundasMap.Legends.Clear();

            Legend legend = new Legend("SubjectLegend");
            legend.Dock = PanelDockStyle.Left;
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
            legend.Title = String.Empty;//"Коды территорий";
            legend.AutoFitMinFontSize = 7;
            legend.ItemColumnSpacing = 100;

            LegendCellColumn column = new LegendCellColumn("Территория");
            column.HeaderFont = new Font("Verdana", 8, FontStyle.Bold);
            column.HeaderTextAlignment = StringAlignment.Center;
            legend.CellColumns.Add(column);

            column = new LegendCellColumn("Оценка");
            column.HeaderFont = new Font("Verdana", 8, FontStyle.Bold);
            column.HeaderTextAlignment = StringAlignment.Far;
            legend.CellColumns.Add(column);

            DundasMap.Legends.Add(legend);

            legend = new Legend("MapLegend");
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
                rule.ColorCount = 1;
                rule.ColoringMode = ColoringMode.ColorRange;
                rule.FromColor = beginMapColor;
                rule.MiddleColor = middleMapColor;
                rule.ToColor = endMapColor;
                rule.BorderColor = Color.FromArgb(50, Color.Black);
                rule.GradientType = GradientType.None;
                rule.HatchStyle = MapHatchStyle.None;
                rule.ShowInColorSwatch = false;
                rule.ShowInLegend = "MapLegend";
                rule.LegendText = string.Format("#FROMVALUE{{{0}}}", mapFormatString);
                DundasMap.ShapeRules.Add(rule);
            }
            else if (coloringRule == ColoringRule.ComplexRangeColoring)
            {
                if (beginQualityLimit != endQualityLimit)
                {
                    LegendItem item = new LegendItem();
                    item.Text = String.Format("Менее {0} {1}", endQualityLimit.ToString(mapFormatString), IsRating ? "(III группа)" : String.Empty);
                    item.Color = beginMapColor;
                    DundasMap.Legends["MapLegend"].Items.Add(item);

                    item = new LegendItem();
                    item.Text = String.Format("{0} - {1} {2}", endQualityLimit.ToString(mapFormatString), beginQualityLimit.ToString(mapFormatString), 
                        IsRating ? "(II группа)" : String.Empty);
                    item.Color = Color.Yellow;
                    DundasMap.Legends["MapLegend"].Items.Add(item);

                    item = new LegendItem();
                    item.Text = String.Format("Более {0} {1}", beginQualityLimit.ToString(mapFormatString), IsRating ? "(I группа)" : String.Empty);
                    item.Color = endMapColor;
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
            else if (coloringRule == ColoringRule.PositiveNegativeColoring)
            {
                LegendItem item = new LegendItem();
                item.Text = "больше либо равно 0";
                item.Color = beginMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = "меньше 0";
                item.Color = endMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);
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

            //map.LoadFromShapeFile(layerName, "CODE", true);
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
                        if (row[columnIndex] != DBNull.Value && Convert.ToDouble(row[columnIndex]) != 0)
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
            string query = DataProvider.GetQueryText("FO_0040_0003_map");
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
                switch (colorCount)
                {
                    case -1:
                        {
                            // совсем убираем раскраску из легенды
                            DundasMap.ShapeRules["IndicatorRule"].ShowInLegend = string.Empty;
                            break;
                        }
                    case 0:
                        {
                            // значение только одно
                            DundasMap.ShapeRules["IndicatorRule"].ColorCount = 1;
                            DundasMap.ShapeRules["IndicatorRule"].LegendText = string.Format("#FROMVALUE{{{0}}}", mapFormatString);
                            break;
                        }
                    default:
                        {
                            // значений несколько
                            DundasMap.ShapeRules["IndicatorRule"].ColorCount = colorCount;
                            break;
                        }
                }
            }

            bool nullValueExists = false;

            foreach (DataRow row in dtMap.Rows)
            {
                string subjectName = row[0].ToString();

                LegendItem item = new LegendItem();
                LegendCell cell = new LegendCell(subjectName);
                cell.Alignment = ContentAlignment.MiddleLeft;
                item.Cells.Add(cell);
                DundasMap.Legends["SubjectLegend"].Items.Add(item);

                ArrayList shapeList = FindMapShape(DundasMap, subjectName);
                foreach (Shape shape in shapeList)
                {
                    string shapeName = GetShapeName(shape);
                    shape.Visible = true;

                    // заполняем карту данными
                    if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                    {
                        double value = Convert.ToDouble(row[1]);

                        if (!IsCalloutTownShape(shape))
                        {
                            cell = new LegendCell(value.ToString(mapFormatString));
                            cell.Alignment = ContentAlignment.MiddleRight;
                            item.Cells.Add(cell);
                        }

                        shape["IndicatorName"] = subjectName;
                        shape["IndicatorValue"] = value;
                        shape.ToolTip = string.Format("{0}{1}{2}: #INDICATORVALUE{{{3}}}", subjectName, valueSeparator, selectedIndicatorName, mapFormatString);

                        if (coloringRule == ColoringRule.ComplexRangeColoring)
                        {
                            shape.Color = GetQualityColor(value);
                        }
                        else if (coloringRule == ColoringRule.PositiveNegativeColoring)
                        {
                            if (value >= 0)
                            {
                                shape.Color = beginMapColor;
                            }
                            else if (value < 0)
                            {
                                shape.Color = endMapColor;
                            }
                        }

                        if (IsCalloutTownShape(shape))
                        {
                            shape.Text = string.Format("{0}\n{1}", shapeName, value.ToString(mapFormatString));
                            shape.TextVisibility = TextVisibility.Shown;
                            shape.TextAlignment = ContentAlignment.BottomCenter;

                            shape.CentralPointOffset.Y = mapCalloutOffsetY;
                        }
                        else
                        {
                            shape.Text = string.Format("{0}\n{1}", shapeName.Replace(" ", "\n"), value.ToString(mapFormatString));
                        }
                    }
                    else
                    {
                        cell = new LegendCell("-");
                        cell.Alignment = ContentAlignment.MiddleCenter;
                        item.Cells.Add(cell);

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
                }
            }
        }

        #endregion

        #region Параметры индикатора

        private void IndicatorDetailDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0040_0003_indicatorDescription");
            dtIndicatorDetail = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtIndicatorDetail);

            indicatorName = GetStringDTValue(dtIndicatorDetail, "Наименование");
            indicatorFormat = GetStringDTValue(dtIndicatorDetail, "Формат");
            beginQualityLimit = 1;
            endQualityLimit = 0;
        }

        private Color GetQualityColor(double value)
        {
            if (value > beginQualityLimit)
            {
                return endMapColor;
            }
            if (value <= endQualityLimit)
            {
                return beginMapColor;
            }
            return Color.Yellow;
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