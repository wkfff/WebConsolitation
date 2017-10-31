using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0016_0004
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtMap;
        private int firstYear = 2009;
        private int endYear = 2011;

        // имя папки с картами региона
        private string mapFolderName;
        // пропорция карты
        private double mapSizeProportion;
        // масшбтаб карты
        private double mapZoomValue;
        // сдвиг по вертикали текста городов-выносок карты
        private double mapCalloutOffsetY;
        // тип раскраски
        private bool twoColoring;

        #endregion

        #region Параметры запроса

        // выбранный индикатор
        private CustomParam selectedIndicator;
        // уровень районов
        private CustomParam regionLevel;
        // уровень поселений
        private CustomParam settlementLevel;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.6 - 220);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight);

            mapFolderName = RegionSettingsHelper.Instance.GetPropertyValue("MapFolderName");
            mapZoomValue = Convert.ToDouble(RegionSettingsHelper.Instance.GetPropertyValue("MapZoomValue"));

            double value = 1;
            mapSizeProportion = value;
            if (Double.TryParse(RegionSettingsHelper.Instance.GetPropertyValue("MapSizeProportion"), NumberStyles.Any, NumberFormatInfo.CurrentInfo, out value))
            {
                mapSizeProportion = value;
            }

            value = 1;
            mapCalloutOffsetY = value;
            if (Double.TryParse(RegionSettingsHelper.Instance.GetPropertyValue("MapCalloutOffsetY"), NumberStyles.Any, NumberFormatInfo.CurrentInfo, out value))
            {
                mapCalloutOffsetY = value;
            }

            #region Инициализация параметров запроса

            selectedIndicator = UserParams.CustomParam("selected_indicator");
            regionLevel = UserParams.CustomParam("region_level");
            settlementLevel = UserParams.CustomParam("settlement_level");

            #endregion

            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            regionLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            settlementLevel.Value = RegionSettingsHelper.Instance.SettlementLevel;

            if (!Page.IsPostBack)
            {
                mapWebAsyncPanel.AddRefreshTarget(mapCaption);
                mapWebAsyncPanel.AddRefreshTarget(DundasMap);
                //mapWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0016_0004_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string baseQuarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 170;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillQuaters(false));
                ComboQuarter.SetСheckedState(baseQuarter, true);

                hiddenndicatorLabel.Text = "[Показатели__БККУ_Сопоставимый].[Показатели__БККУ_Сопоставимый].[Данные всех источников].[БК 2]";
            }

            Page.Title = string.Format("Результаты мониторинга БК и КУ в разрезе показателей");
            PageTitle.Text = Page.Title;

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int quarterNum = ComboQuarter.SelectedIndex + 1;

            PageSubTitle.Text = string.Format("за {0} квартал {1} года", quarterNum, yearNum);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(quarterNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", quarterNum);

            if (!mapWebAsyncPanel.IsAsyncPostBack)
            {
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();

                string patternValue = selectedIndicator.Value;
                int defaultRowIndex = 0;
                if (patternValue == string.Empty)
                {
                    defaultRowIndex = 0;
                }

                if (UltraWebGrid.Columns.Count > 0 && UltraWebGrid.Rows.Count > 0)
                {
                    // ищем строку
                    UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, UltraWebGrid.Columns.Count - 1, defaultRowIndex);
                    // выделяем строку
                    ActiveGridRow(row);
                }
            }
            
            // заполняем карту данными
            //FillMapData();
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

            string indicatorName = row.Cells[0].Text;

            twoColoring = !indicatorName.Contains("Общее количество нарушений ");

            hiddenndicatorLabel.Text = row.Cells[row.Cells.Count - 1].Text;
            selectedIndicator.Value = hiddenndicatorLabel.Text;

            mapCaption.Text = string.Format("{0}", indicatorName);

            FillMapData();

            UltraWebGrid.DisplayLayout.SelectedRows.Clear();
            row.Selected = true;
            UltraWebGrid.DisplayLayout.ActiveRow = row;
        }

        void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0016_0004_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Обозначение", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            //UltraWebGrid.Height = Unit.Empty;
           // UltraWebGrid.Width = Unit.Empty;
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
                string formatString = "";
                
                int widthColumn = 150;
                HorizontalAlign horizontalAlign = HorizontalAlign.Right;
                switch(i)
                {
                    case 1:
                        {
                            widthColumn = 760;
                            e.Layout.Bands[0].Columns[i].CellStyle.Wrap = true;
                            horizontalAlign = HorizontalAlign.Left;
                            break;
                        }
                    case 2:
                        {
                            widthColumn = 70;
                            break;
                        }
                    case 3:
                        {
                            widthColumn = 80;
                            formatString = "N0";
                            break;
                        }
                    case 4:
                        {
                            widthColumn = 80;
                            formatString = "N0";
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = horizontalAlign;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(100);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count - 1; i++)
            {
                bool indicatorNameColumn = (i == 0);

                if (indicatorNameColumn && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    string indicatorName = e.Row.Cells[i].Value.ToString();
                    if (indicatorName.Contains("Общее количество нарушений"))
                    {
                        e.Row.Cells[i].Style.Font.Bold = true;
                        e.Row.Cells[i].ColSpan = e.Row.Cells.Count - 3;

//                        for (int j = 1; j < e.Row.Cells.Count - 1; j++)
//                        {
//                            e.Row.Cells[j].Value = string.Empty;
//                        }
                    }
                    else
                    {
                        e.Row.Cells[i].Style.Padding.Left = 15;
                    }
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

        #endregion

        #region Обработчики карты

        public void SetMapSettings()
        {
            DundasMap.RenderType = RenderType.InteractiveImage;
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = (float)mapZoomValue;

            // добавляем легенду
            Legend legend1 = new Legend("CrimeCountLegend");
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
            legend1.Title = "Количество нарушений\nМР и ГО";
            legend1.AutoFitMinFontSize = 7;
            if (twoColoring)
            {
                LegendItem item = new LegendItem();
                item.Text = "Нет нарушения";
                item.Color = Color.Green;
                legend1.Items.Add(item);

                item = new LegendItem();
                item.Text = "Есть нарушение";
                item.Color = Color.Red;
                legend1.Items.Add(item); 
            }

            Legend legend2 = new Legend("SymbolCrimeCountLegend");
            legend2.Visible = true;
            legend2.Dock = PanelDockStyle.Left;
            legend2.BackColor = Color.White;
            legend2.BackSecondaryColor = Color.Gainsboro;
            legend2.BackGradientType = GradientType.DiagonalLeft;
            legend2.BackHatchStyle = MapHatchStyle.None;
            legend2.BorderColor = Color.Gray;
            legend2.BorderWidth = 1;
            legend2.BorderStyle = MapDashStyle.Solid;
            legend2.BackShadowOffset = 4;
            legend2.TextColor = Color.Black;
            legend2.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend2.AutoFitText = true;
            legend2.Title = "Количество нарушений\nв поселениях";
            legend2.AutoFitMinFontSize = 7;

            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend1);
            DundasMap.Legends.Add(legend2);
            
            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CrimeCountRule";
            rule.Category = String.Empty;
            rule.ShapeField = "RegionCrimeCount";
            rule.DataGrouping = DataGrouping.Optimal;
            rule.ColorCount = twoColoring ? 2 : 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Green;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = !twoColoring ? "CrimeCountLegend" : string.Empty;
            rule.LegendText = "#FROMVALUE{N0} - #TOVALUE{N0}";
            DundasMap.ShapeRules.Add(rule);

            // добавляем правила расстановки символов
            DundasMap.SymbolRules.Clear();
            SymbolRule symbolRule = new SymbolRule();
            symbolRule.Name = "SymbolCrimeRule";
            symbolRule.Category = String.Empty;
            symbolRule.DataGrouping = DataGrouping.Optimal;
            symbolRule.SymbolField = "SettlementCrimeCount";
            symbolRule.ShowInLegend = "SymbolCrimeCountLegend";
            symbolRule.LegendText = "#FROMVALUE{N0} - #TOVALUE{N0}";
            DundasMap.SymbolRules.Add(symbolRule);

            // кружки для легенды
            for (int i = 1; i < 4; i++)
            {
                PredefinedSymbol predefined = new PredefinedSymbol();
                predefined.Name = "PredefinedSymbol" + i;
                predefined.MarkerStyle = MarkerStyle.Circle;
                predefined.Width = 5 + (i * 5);
                predefined.Height = predefined.Width;
                predefined.Color = Color.Gold;
                DundasMap.SymbolRules["SymbolCrimeRule"].PredefinedSymbols.Add(predefined);
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

            //shapeName = shapeName.Replace("муниципальный район", "МР");

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
        
//        private static bool IsNullValuesColumn(DataTable dt, int columnIndex)
//        {
//            foreach (DataRow row in dt.Rows)
//            {
//                if (row[columnIndex] == DBNull.Value)
//                {
//
//                }
//            }
//        }

        public void FillMapData()
        {
            //DundasMap.Shapes.Clear();
            if (Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("MapLoadFromBin")))
            {
                DundasMap.Serializer.Format = Dundas.Maps.WebControl.SerializationFormat.Binary;
                DundasMap.Serializer.Load(
                    (Server.MapPath(string.Format("../../maps/Субъекты/{0}/{0}_выноски.bin", mapFolderName))));
                SetMapSettings();
            }
            else
            {
                // добавляем поля для раскраски
                DundasMap.ShapeFields.Clear();
                DundasMap.ShapeFields.Add("Name");
                DundasMap.ShapeFields["Name"].Type = typeof(string);
                DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
                DundasMap.ShapeFields.Add("RegionCrimeCount");
                DundasMap.ShapeFields["RegionCrimeCount"].Type = typeof(double);
                DundasMap.ShapeFields["RegionCrimeCount"].UniqueIdentifier = false;

                // добавляем поля для символов
                DundasMap.SymbolFields.Clear();
                DundasMap.SymbolFields.Add("SettlementCrimeCount");
                DundasMap.SymbolFields["SettlementCrimeCount"].Type = typeof(double);
                DundasMap.SymbolFields["SettlementCrimeCount"].UniqueIdentifier = false;

                SetMapSettings();
                DundasMap.Shapes.Clear();
                DundasMap.Layers.Clear();
                AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);
                AddMapLayer(DundasMap, mapFolderName, "Города", CRHelper.MapShapeType.Towns);
                AddMapLayer(DundasMap, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);
            }

            dtMap = new DataTable();
            string query = DataProvider.GetQueryText("FO_0016_0004_map");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtMap);

            foreach (Shape shape in DundasMap.Shapes)
            {
                string shapeName = GetShapeName(shape);
                if (!RegionsNamingHelper.LocalBudgetUniqueNames.ContainsKey(shapeName) ||
                    (RegionsNamingHelper.LocalBudgetTypes[shapeName] == "ГО" && !IsCalloutTownShape(shape)))
                {
                    shape.Visible = false;
                }
            }

            DundasMap.Symbols.Clear();
            bool nullValueExists = false;

            foreach (DataRow row in dtMap.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                {
                    string subject = row[0].ToString();
                    ArrayList shapeList = FindMapShape(DundasMap, subject);
                    foreach (Shape shape in shapeList)
                    {
                        string shapeName = GetShapeName(shape);

                        if (IsCalloutTownShape(shape))
                        {
                            shape.Text = string.Format("{0}", shapeName);
                            shape.TextVisibility = TextVisibility.Shown;
                            shape.TextAlignment = ContentAlignment.BottomCenter;
                            shape.CentralPointOffset.Y = mapCalloutOffsetY;
                        }
                        else
                        {
                            shape.Text = string.Format("{0}", shapeName.Replace(" ", "\n"));
                        }

                        string settlementCrimeStr = string.Empty;
                        if (row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                        {
                            double settlementCrimeCount = Convert.ToDouble(row[2]);
                            settlementCrimeStr = string.Format("Число нарушений в поселениях: {0:N0}", settlementCrimeCount);

                            Symbol symbol = new Symbol();
                            symbol.Name = shape.Name + DundasMap.Symbols.Count;
                            symbol.ParentShape = shape.Name;
                            symbol["SettlementCrimeCount"] = settlementCrimeCount;
                            symbol.Offset.Y = -20;
                            symbol.Color = Color.Gold;
                            symbol.Layer = DundasMap.Layers[0].Name;
                            symbol.MarkerStyle = MarkerStyle.Circle;
                            DundasMap.Symbols.Add(symbol);
                        }

                        string regionCrimeStr = string.Empty;
                        if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                        {
                            double regionCrimeCount = Convert.ToDouble(row[1]);
                            shape["Name"] = subject;
                            shape["RegionCrimeCount"] = regionCrimeCount;
                            if (twoColoring)
                            {
                                regionCrimeStr = string.Format("{0}", regionCrimeCount == 1 ? "Есть нарушение" : "Нет нарушения");
                            }
                            else
                            {
                                regionCrimeStr = string.Format("Число нарушений: {0:N0}", regionCrimeCount);
                            }
                        }
                        else
                        {
                            shape.Color = Color.LightSkyBlue;
                            regionCrimeStr = string.Format("{0}", "Оценка отсутствует");
                            
                            if (!nullValueExists && DundasMap.Legends.Count > 0)
                            {
                                nullValueExists = true;

                                LegendItem item = new LegendItem();
                                item.Text = "Оценка отсутствует";
                                item.Color = Color.LightSkyBlue;
                                DundasMap.Legends["CrimeCountLegend"].Items.Add(item);
                            }
                        }

                        shape.ToolTip = string.Format("{0}\n{1}\n{2}", shapeName, regionCrimeStr, settlementCrimeStr);
                    }
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "";
            e.CurrentWorksheet.Columns[0].Width = 150 * 37;

            // расставляем стили у ячеек хидера
            for (int i = 0; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 18 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount + 4; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    e.CurrentWorksheet.Rows[i].Height = 22 * 37;
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.Alignment = j > columnCount - 2 ? HorizontalCellAlignment.Right : HorizontalCellAlignment.Left;
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                }
            }

            for (int i = 1; i < columnCount; i = i + 1)
            {
                string formatString = "";
                int width = 100;

                switch (i)
                {
                    case 1:
                        {
                            width = 400;
                            break;
                        }
                    case 2:
                        {
                            width = 70;
                            break;
                        }
                    case 3:
                    case 4:
                        {
                            width = 80;
                            formatString = "#0";
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = width * 37;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
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
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(mapCaption.Text);

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
