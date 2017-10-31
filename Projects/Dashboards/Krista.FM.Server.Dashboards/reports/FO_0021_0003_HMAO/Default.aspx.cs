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
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.Misc;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using ContentAlignment=System.Drawing.ContentAlignment;

namespace Krista.FM.Server.Dashboards.reports.FO_0021_0003_HMAO
{
    public enum ColoringRule
    {
        // раскраска относительно среднего
        TwoColoringAVG,
        // раскраска относительно нуля
        TwoColoringPositive,
        // раскраска тремя цветами
        TreeColoring,
        // раскраска двумя цветами нарушения
        TwoColoringOneNul,
        //раскраска одним цветом
        AllGreen,
        //раскраска двумя цветами ВысДотМО
        TwoColoringOneNul2    
    }

    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtMap;
        private DataTable dtMapLimit;
        private int firstYear = 2010;
        private int endYear = 2012;

        private string selectedIndicatorName;
        private string selectedIndicatorType;
        private string selectedIndicatorCode;

        private double beginColorLimit;
        private double endColorLimit;
        private double avgColorLimitTowns;
        private double avgColorLimitRegions;
        private int selectedYear;

        private ColoringRule coloringRule;
        private Color beginMapColor;
        private Color middleMapColor;
        private Color endMapColor;
        private Color beginMapColorGO;
        private Color endMapColorGO;
        private Color nullEmptyColor;
        private string nullEmptyText;
        private string mapFormatString;
        private string mapLegendCaption;
        
        #endregion

        // имя папки с картами региона
        private string mapFolderName;
        // масштаб карты
        private double mapZoomValue;
        // сдвиг по вертикали текста городов-выносок карты
        private double mapCalloutOffsetY;

        private static MemberAttributesDigest indicatorDigest;
        
        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;
        // выбранный индикатор
        private CustomParam selectedIndicator;
        // имя выбранного индикатора
        private CustomParam selectedIndicatorCaption;
        // фильтр по направлению
        private CustomParam directionFilter;
        // тип сортировки таблицы для карты
        private CustomParam sortingType;

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
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 180);

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
            selectedIndicator = UserParams.CustomParam("selected_indicator");
            selectedIndicatorCaption = UserParams.CustomParam("selected_indicator_caption");
            directionFilter = UserParams.CustomParam("direction_filter");
            sortingType = UserParams.CustomParam("sorting_type");

            #endregion

            UltraGridExporter1.ExcelExportButton.Visible = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;оценки";
            CrossLink1.NavigateUrl = "~/reports/FO_0021_0001_HMAO/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Рейтинг&nbsp;МО";
            CrossLink2.NavigateUrl = "~/reports/FO_0021_0002_HMAO/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Результаты&nbsp;оценки&nbsp;по&nbsp;отдельному&nbsp;показателю";
            CrossLink3.NavigateUrl = "~/reports/FO_0021_0004_HMAO/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Мониторинг&nbsp;соблюдения&nbsp;бюджетного&nbsp;законодательства";
            CrossLink4.NavigateUrl = "~/reports/FO_0021_0005_HMAO/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0021_0003_HMAO_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                selectedIndicatorCaption.Value = "Итоговая сводная оценка";

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
                ComboYear.AutoPostBack = true;

             
            }

            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);

            Page.Title = String.Format("Картограмма результатов сводной оценки качества организации и осуществления бюджетного процесса в муниципальных образованиях автономного округа");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("по итогам {0} года", selectedYear);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
 
            selectedPeriod.Value = String.Format("{0}",selectedYear);

            directionFilter.Value = "true";

            if (ComboIndicator.SelectedValue != String.Empty)
            {
                selectedIndicatorCaption.Value = ComboIndicator.SelectedValue;
            }

            ComboIndicator.Title = "Показатель";
            ComboIndicator.Width = 600;
            ComboIndicator.MultiSelect = false;
            indicatorDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0021_0003_HMAO_indicatorList");
            ComboIndicator.ClearNodes();
            ComboIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(indicatorDigest.UniqueNames, indicatorDigest.MemberLevels));
            ComboIndicator.SelectLastNode();
            ComboIndicator.SetСheckedState(selectedIndicatorCaption.Value, true);
            
            selectedIndicatorName = ComboIndicator.SelectedValue;
            selectedIndicator.Value = indicatorDigest.GetMemberUniqueName(selectedIndicatorName);
            selectedIndicatorType = indicatorDigest.GetMemberType(selectedIndicatorName);
            selectedIndicatorCode = indicatorDigest.GetShortName(selectedIndicatorName);

            sortingType.Value = selectedIndicatorCode == "Рейтинг" ? "BASC" : "BDESC";

            mapElementCaption.Text = selectedIndicatorName == String.Empty ? String.Empty : String.Format("Показатель «{0}»", selectedIndicatorName);

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
                //AddMapLayer(DundasMap, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);
            }

            if (selectedIndicator.Value == String.Empty)
            {
                ShowNoDataMessage();
            }
            else
            {
                // заполняем карту данными
                FillMapData();
            }
        }

        #region Обработчики карты

        private string GetIndicatorFormatString(string indicatorCode)
        {
            switch (indicatorCode)
            {
                case "ИСО":
                case "СОК":
                    {
                        return "N1";
                    }
                case "Рейтинг":
                case "Н-вРасхОМСУ":
                case "ОбъемМЗ":
                case "ОбслужМД":
                case "ДФ":
                case "МД":
                case "ВысДотМО":
                    {
                        return "N0";
                    }
                default:
                    {
                        return "N1";
                    }
            }
        }
        
        public void SetMapSettings()
        {
            nullEmptyColor = Color.LightSkyBlue;
            nullEmptyText = "Нет данных";
            mapFormatString = GetIndicatorFormatString(selectedIndicatorCode);

            switch (selectedIndicatorCode)
            {
                case "СОК":
                case "ИСО":
                    {
                        coloringRule = ColoringRule.TwoColoringAVG;
                        beginMapColorGO = Color.DarkGreen;
                        endMapColorGO = Color.Red;
                        beginMapColor = Color.Yellow;
                        endMapColor = Color.SkyBlue;
                        mapLegendCaption = "Оценка";
                        break;
                    }
                case "Рейтинг":
                        {
                            coloringRule = ColoringRule.AllGreen;
                            beginMapColor = Color.DarkGreen;
                            endMapColor = Color.Red;
                          
                            mapLegendCaption = "Рейтинг";
                            break;
                        }
                case "Грант":
                        {
                            coloringRule = ColoringRule.TwoColoringPositive;
                            beginMapColor = Color.DarkGreen;
                            endMapColor = Color.Red;
                            mapLegendCaption = string.Format("{0},тыс.руб.",selectedIndicatorName);
                            break;
                        }
                case "Н-вРасхОМСУ":
                case "ОбъемМЗ":
                case "ОбслужМД":
                case "ДФ":
                case "МД":
                        {
                            coloringRule = ColoringRule.TwoColoringOneNul;
                            beginMapColor = Color.Red;
                            endMapColor = Color.DarkGreen;
                            mapLegendCaption = "Наличие нарушений";
                            break;
                        }
                case "ВысДотМО":
                        {
                            coloringRule = ColoringRule.TwoColoringOneNul2;
                            beginMapColor = Color.DarkGreen;
                            endMapColor = Color.Red;
                            mapLegendCaption = "Оценка";
                            break;
                        }
                default:
                        {
                            coloringRule = ColoringRule.TreeColoring;
                            beginMapColor = Color.Red;
                            middleMapColor = Color.Yellow;
                            endMapColor = Color.Green;
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
            legend.Title = String.Empty;
            legend.AutoFitMinFontSize = 7;
            legend.ItemColumnSpacing = 100;
            legend.MaxAutoSize = 100;
            legend.AutoSize = true;

            LegendCellColumn column = new LegendCellColumn("Территория");
            column.HeaderFont = new Font("Verdana", 7.5f, FontStyle.Bold);
            column.HeaderTextAlignment = StringAlignment.Center;
            //column.MaximumWidth = 1500;
            legend.CellColumns.Add(column);
            column = new LegendCellColumn(mapLegendCaption.Replace(' ', '\n'));
            column.HeaderFont = new Font("Verdana", 7.5f, FontStyle.Bold);
            column.HeaderTextAlignment = StringAlignment.Center;
            legend.CellColumns.Add(column);
            DundasMap.Legends.Add(legend);

            legend = new Legend("MapLegend");
            legend.Dock = PanelDockStyle.Right;
            legend.AutoSize = true;    
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
          
            if (coloringRule == ColoringRule.TwoColoringAVG)
            {
                LegendItem item = new LegendItem();
                item.Text = String.Format("МР с оценкой\nвыше средней");
                item.Color = beginMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("МР с оценкой\nниже средней");
                item.Color = endMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("ГО с оценкой\nвыше средней");
                item.Color = beginMapColorGO;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("ГО с оценкой\nниже средней");
                item.Color = endMapColorGO;
                DundasMap.Legends["MapLegend"].Items.Add(item);
            }
            else if (coloringRule == ColoringRule.TreeColoring)
            {
                LegendItem item = new LegendItem();
                item.Text = "Оценка равна 0";
                item.Color = beginMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = "Оценка от 0 до 1";
                item.Color = middleMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = "Оценка больше 1";
                item.Color = endMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);
            }
            else if (coloringRule == ColoringRule.TwoColoringPositive)
            {
                LegendItem item = new LegendItem();
                item.Text = String.Format("МО, получившие\n грант");
                item.Color = beginMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("МО, не получившие\n грант");
                item.Color = endMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);
            }
            else if (coloringRule == ColoringRule.TwoColoringOneNul)
            {
                LegendItem item = new LegendItem();
                item.Text = String.Format("МО с нарушениями");
                item.Color = beginMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("МО без нарушений");
                item.Color = endMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);
            }
            else if (coloringRule == ColoringRule.TwoColoringOneNul2)
            {
                LegendItem item = new LegendItem();
                item.Text = String.Format("МО не являтся\nвысокодотационным");
                item.Color = beginMapColor;
                DundasMap.Legends["MapLegend"].Items.Add(item);

                item = new LegendItem();
                item.Text = String.Format("МО являтся\nвысокодотационным");
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
        /// Является ли форма городом
        /// </summary>
        /// <param name="shape">форма</param>
        /// <returns>true, если является</returns>
        public static bool IsTownShape(Shape shape)
        {
            string shapeName = shape.Name;
            if (RegionsNamingHelper.LocalBudgetTypes.ContainsKey(shapeName))
            {
                return RegionsNamingHelper.LocalBudgetTypes[shapeName].Contains("ГО");
            }

            return false;
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

        private string GetShortRegionName(string fullRegionName)
        {
            string shortRegionName = fullRegionName.Replace("муниципальное образование", "МО");
            shortRegionName = shortRegionName.Replace("муниципальный район", "МР");
            return shortRegionName;
        }

        private void ShowNoDataMessage()
        {
            DundasMap.Visible = false;
            EmptyMapCaption.Visible = true;
            EmptyMapCaption.Text = "<br/>Нет данных";
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Columns.Contains(columnName) && dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        public void FillMapData()
        {
            string queryQuality = DataProvider.GetQueryText("FO_0021_0003_HMAO_quality_limit");

            dtMapLimit = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryQuality, "Dummy", dtMapLimit);
            if (dtMapLimit.Rows.Count > 0)
            {
                beginColorLimit = GetDoubleDTValue(dtMapLimit, "Первая граница");
                endColorLimit = GetDoubleDTValue(dtMapLimit, "Вторая граница");
            }

            //string queryAvg = DataProvider.GetQueryText("FO_0021_0003_HMAO_avg_indicator");

            //dtMapLimit = new DataTable();
            //DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryAvg, "Dummy", dtMapLimit);
            //if (dtMapLimit.Rows.Count > 0)
            //{
            //    avgColorLimit = GetDoubleDTValue(dtMapLimit, "Среднее значение");
            //}
            
            DundasMap.Visible = true;
            EmptyMapCaption.Visible = false;
            string query = string.Empty;
            switch (selectedIndicatorCode)
            {
                case "ИСО":
                case "СОК":
                    {
                        query = DataProvider.GetQueryText("FO_0021_0003_HMAO_map_avg");
                        break;
                    }
                case "Рейтинг":
                    {
                        query = DataProvider.GetQueryText("FO_0021_0003_HMAO_map_rating");
                        break;
                    }
                default:
                    {
                        query = DataProvider.GetQueryText("FO_0021_0003_HMAO_map");
                        break;
                    }
            }
            
            dtMap = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,
                                                                             "Наименование муниципального образования",
                                                                             dtMap);

            if (!NonNullValueGrid(dtMap, 1))
            {
                ShowNoDataMessage();
                return;
            }

            foreach (Shape shape in DundasMap.Shapes)
            {
                shape.Visible = false;
            }

            string valueSeparator = IsMozilla ? ";" : "\n";
            
            for (int i = 0; i < dtMap.Rows.Count; i++)
            {
                DataRow row = dtMap.Rows[i];             
                string subjectName = row[0].ToString();
                LegendItem item = new LegendItem();
                LegendCell cell = new LegendCell(subjectName);
                if (subjectName == "Средняя оценка по ГО")
                {
                    cell.Text = "Средняя оценка по\nгородским округам";
                }
                else
                {
                    if (subjectName == "Средняя оценка по МР")
                    {
                        cell.Text = "Средняя оценка по\nмуниципальным районам";
                    }
                }
                cell.Alignment = ContentAlignment.MiddleLeft;
                item.Cells.Add(cell);
                DundasMap.Legends["SubjectLegend"].Items.Add(item);

                if ((subjectName == "Средняя оценка по ГО") || (subjectName == "Средняя оценка по МР"))
                {
                    if (row[1] != DBNull.Value && row[1].ToString() != String.Empty)
                    {
                        double value = Convert.ToDouble(row[1]);
                        cell = new LegendCell(value.ToString(mapFormatString));
                        cell.Margins.Right = 0;
                        cell.Alignment = ContentAlignment.MiddleCenter;
                        cell.Font = new Font("Verdana", 8, FontStyle.Bold);
                        item.Cells.Add(cell);
                        if (subjectName == "Средняя оценка по ГО")
                        {
                            avgColorLimitTowns = value;
                        }
                        else
                        {
                            avgColorLimitRegions = value;
                        }
                    }
                }
                else
                {
                ArrayList shapeList = FindMapShape(DundasMap, subjectName);
                foreach (Shape shape in shapeList)
                {
                    string shapeName = GetShapeName(shape);
                    shape.Visible = true;

                    subjectName = GetShortRegionName(subjectName);

                    // заполняем карту данными
                    if (row[1] != DBNull.Value && row[1].ToString() != String.Empty)
                    {
                        double value = Convert.ToDouble(row[1]);

                        if (!IsCalloutTownShape(shape))
                        {
                            cell = new LegendCell(value.ToString(mapFormatString));
                            cell.Margins.Right = 0;
                            cell.Alignment = ContentAlignment.MiddleCenter;
                            item.Cells.Add(cell);
                        }

                        shape["IndicatorName"] = subjectName;
                        shape["IndicatorValue"] = value;
                        shape.ToolTip = string.Format("{0}{1}{2}: #INDICATORVALUE{{{3}}}", subjectName.Replace("\"", "&quot"), valueSeparator, selectedIndicatorName, mapFormatString);

                        switch (coloringRule)
                        {
                            case ColoringRule.TwoColoringAVG:
                                {
                                    if ((value >= avgColorLimitTowns) && (IsTownShape(shape)))
                                    {
                                        shape.Color = beginMapColorGO;
                                    }
                                    else
                                    {
                                        if ((value >= avgColorLimitRegions) && (!IsTownShape(shape)))
                                        {
                                            shape.Color = beginMapColor;
                                        }
                                        else
                                        {
                                            if ((value < avgColorLimitTowns) && (IsTownShape(shape)))
                                            {
                                                shape.Color = endMapColorGO;
                                            }
                                            else
                                            {
                                                shape.Color = endMapColor;
                                            }
                                        }
                                    }
                                    break;
                                }
                            case ColoringRule.TreeColoring:
                                {
                                    if (value == 0)
                                    {
                                        shape.Color = beginMapColor;
                                    }
                                    else if ((value > 0) && (value < 1))
                                    {
                                        shape.Color = middleMapColor;
                                    }
                                    else if (value >= 1)
                                    {
                                        shape.Color = endMapColor;
                                    }
                                    break;
                                }
                            case ColoringRule.TwoColoringPositive:
                                {
                                    if (value > 0)
                                    {
                                        shape.Color = beginMapColor;
                                    }
                                    else
                                    {
                                        shape.Color = endMapColor;
                                    }
                                    break;
                                }
                            case ColoringRule.TwoColoringOneNul:
                                {
                                    if (value == 1)
                                    {
                                        shape.Color = beginMapColor;
                                    }
                                    else
                                    {
                                        shape.Color = endMapColor;
                                    }
                                    break;
                                }
                            case ColoringRule.AllGreen:
                                {
                                    shape.Color = beginMapColor;
                                    break;
                                }
                            case ColoringRule.TwoColoringOneNul2:
                                {
                                    if (value == 0)
                                    {
                                        shape.Color = beginMapColor;
                                    }
                                    else
                                    {
                                        shape.Color = endMapColor;
                                    }
                                    break;
                                }
                        }


                        //shape.TextVisibility = IsTownShape(shape) ? TextVisibility.Auto : TextVisibility.Shown;
                        shape.TextVisibility = TextVisibility.Shown;
                        if (IsTownShape(shape))
                        {
                            shape.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            if (shape["IndicatorName"].ToString().Contains("Ханты-Мансийск"))
                            {
                                shape.CentralPointOffset.Y = -0.4;
                            }
                            else
                            {
                                if (shape["IndicatorName"].ToString().Contains("Нижневартовск"))
                                {
                                    shape.CentralPointOffset.Y = -0.6;
                                }
                                else
                                {
                                    shape.CentralPointOffset.Y = -0.4;
                                }
                            }
                        }
                        else
                        {
                            if (shape["IndicatorName"].ToString().Contains("Сургутский"))
                            {
                                shape.CentralPointOffset.Y = 0.4;
                            }
                            else
                            {
                                shape.CentralPointOffset.Y = 0.2;
                            }
                            if (shape["IndicatorName"].ToString().Contains("Нефтеюганский"))
                            {
                                shape.CentralPointOffset.X = -0.4;
                            }
                        }
                        
                        if (IsCalloutTownShape(shape))
                        {
                            shape.Text = string.Format("{0}\n{1}", subjectName.Replace("ЗАТО ", String.Empty), value.ToString(mapFormatString));
                            shape.TextAlignment = ContentAlignment.BottomCenter;

                            shape.CentralPointOffset.Y = mapCalloutOffsetY;
                        }
                        else
                        {
                            shape.Text = string.Format("{0}\n{1}", subjectName.Replace(" ", "\n"), value.ToString(mapFormatString));
                        }
                    }
                    else
                    {
                        if (IsCalloutTownShape(shape))
                        {
                            shape.Color = nullEmptyColor;
                            shape.ToolTip = subjectName.Replace("\"", "&quot");
                            shape.Text = shapeName.Replace("ЗАТО ", String.Empty);
                            shape.TextVisibility = TextVisibility.Shown;
                            shape.TextAlignment = ContentAlignment.BottomCenter;

                            shape.CentralPointOffset.Y = mapCalloutOffsetY;
                        }
                        else
                        {
                            cell = new LegendCell("-");
                            cell.Alignment = ContentAlignment.MiddleCenter;
                            item.Cells.Add(cell);

                            shape.Color = nullEmptyColor;
                            shape.ToolTip = string.Format("{0}\n{1}", subjectName.Replace("\"", "&quot"), nullEmptyText);
                            shape.Text = shapeName;
                        }
                    }
                }
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = mapElementCaption.Text;

            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth));
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

            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.9));
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);
        }

        #endregion
    }
}