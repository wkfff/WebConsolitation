using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Web;

namespace Krista.FM.Server.Dashboards.reports.FO_0016_0002_HMAO
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtMap;
        private int firstYear = 2011;
        private int endYear = 2011;
        private bool isSumIndicator = false;

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

        private GridHeaderLayout headerLayout;

 

        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }
      

         #endregion


        #region Параметры запроса

        // выбранный индикатор
        private CustomParam selectedIndicator;
        // уровень районов
        private CustomParam regionLevel;
        // выбранный период
        private CustomParam selectedPeriod;
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 25);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 220);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.93);

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
            selectedPeriod = UserParams.CustomParam("selected_period");
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;мониторинга";
            CrossLink1.NavigateUrl = "~/reports/FO_0016_0001_HMAO/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Динамика&nbsp;изменения&nbsp;значений&nbsp;показателей&nbsp;мониторинга";
            CrossLink2.NavigateUrl = "~/reports/FO_0016_0003_HMAO/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Расходы&nbsp;на&nbsp;содержание&nbsp;ОМСУ&nbsp;в&nbsp;разрезе&nbsp;поселений";
            CrossLink3.NavigateUrl = "~/reports/FO_0016_0004_HMAO/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            regionLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            

            if (!Page.IsPostBack)
            {
                mapWebAsyncPanel.AddRefreshTarget(mapCaption);
                mapWebAsyncPanel.AddRefreshTarget(DundasMap);
                mapWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0016_0002_HMAO_date");
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

                hiddenndicatorLabel.Text = "[Показатели__БККУ_Сопоставимый].[Показатели__БККУ_Сопоставимый].[Данные всех источников].[Общее количество нарушений]";
            }

            Page.Title = string.Format("Результаты мониторинга БК и КУ в разрезе показателей");
            PageTitle.Text = Page.Title;

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int quarterNum = ComboQuarter.SelectedIndex + 1;

            PageSubTitle.Text = string.Format("за {0} квартал {1} года", quarterNum, yearNum);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(quarterNum));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", quarterNum);
            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);
            
            if (!mapWebAsyncPanel.IsAsyncPostBack)
            {
                headerLayout = new GridHeaderLayout(UltraWebGrid);
                
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

            if (selectedIndicator.Value != String.Empty)
            {
                // заполняем карту данными
                FillMapData();
            }
            
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
            if (indicatorName == "Общее количество нарушений")
            {
                isSumIndicator = true;
            }
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
            string query = DataProvider.GetQueryText("FO_0016_0002_HMAO_grid");
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
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.Width = Unit.Empty;
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(80);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Header.Caption = "Обозначение";
            e.Layout.Bands[0].Columns[0].MergeCells = false;
            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "";
                e.Layout.Bands[0].Columns[i].MergeCells = false;
                int widthColumn = 150;
                HorizontalAlign horizontalAlign = HorizontalAlign.Right;
                switch(i)
                {
                    case 1:
                        {
                            widthColumn = 600;
                            e.Layout.Bands[0].Columns[i].CellStyle.Wrap = true;
                            horizontalAlign = HorizontalAlign.Left;
                            break;
                        }
                    case 2:
                        {
                            widthColumn = 380;
                            e.Layout.Bands[0].Columns[i].CellStyle.Wrap = true;
                            horizontalAlign = HorizontalAlign.Left;
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
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

           
            headerLayout.AddCell("Наименование");
            headerLayout.AddCell("Содержание");
            headerLayout.AddCell("Число нарушений в МР и ГО");
            headerLayout.ApplyHeaderInfo();
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
                        e.Row.Cells[i].ColSpan = 3;
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
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = (float)mapZoomValue;
            //DundasMap.Viewport.ViewCenter.X -= 10;

            DundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Right;

            // добавляем легенду
            Legend legend1 = new Legend("SubjectLegend");
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
          
                LegendItem item = new LegendItem();
                item.Text = "Нет нарушений";
                item.Color = Color.Green;
                legend1.Items.Add(item);

                item = new LegendItem();
                item.Text = "Есть нарушения";
                item.Color = Color.Red;
                
                legend1.Items.Add(item);
            


            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend1);

            
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

        public void FillMapData()
        {
            //DundasMap.Shapes.Clear();

            dtMap = new DataTable();
            string query;
            DundasMap.Visible = true;
            if (isSumIndicator)
            {
                query = DataProvider.GetQueryText("FO_0016_0002_HMAO_map_sum");
            }
            else
            {
                query = DataProvider.GetQueryText("FO_0016_0002_HMAO_map");
            }
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtMap);
            dtMap.Columns.RemoveAt(0);
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

                ArrayList shapeList = FindMapShape(DundasMap, subjectName);
                foreach (Shape shape in shapeList)
                {
                    string shapeName = GetShapeName(shape);
                    shape.Visible = true;

                    //subjectName = GetShortRegionName(subjectName);

                    // заполняем карту данными
                    if (row[1] != DBNull.Value && row[1].ToString() != String.Empty)
                    {
                        double value = Convert.ToDouble(row[1]);

                        if (!IsCalloutTownShape(shape))
                        {
                            cell = new LegendCell(value.ToString("N0"));
                            cell.Margins.Right = 0;
                            cell.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
                            item.Cells.Add(cell);
                        }

                        shape["IndicatorName"] = subjectName;
                        shape["IndicatorValue"] = value;
                        shape.ToolTip = string.Format("{0}{1}Количество нарушений: {2}", subjectName.Replace("\"", "&quot"), valueSeparator, value);
                        if (value == 0)
                        {
                            shape.Color = Color.Green;
                        }
                        else
                        {
                            shape.Color = Color.Red;
                        }

                        //shape.TextVisibility = IsTownShape(shape) ? TextVisibility.Auto : TextVisibility.Shown;
                        shape.TextVisibility = TextVisibility.Shown;
                        if (IsTownShape(shape))
                        {
                            shape.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            if (shape["IndicatorName"].ToString().Contains("Нижневартовск") || shape["IndicatorName"].ToString().Contains("Ханты-Мансийск"))
                            {
                                shape.CentralPointOffset.Y = -0.4;
                            }
                            else
                            {
                                shape.CentralPointOffset.Y = -0.3;
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
                                shape.CentralPointOffset.X = -0.3;
                            }
                        }

                        if (IsCalloutTownShape(shape))
                        {
                            shape.Text = string.Format("{0}", subjectName);
                            shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;

                            shape.CentralPointOffset.Y = mapCalloutOffsetY;
                        }
                        else
                        {
                            shape.Text = string.Format("{0}", subjectName);
                        }
                    }
                    else
                    {
                        if (IsCalloutTownShape(shape))
                        {
                            shape.Color = Color.LightSkyBlue;
                            shape.ToolTip = subjectName.Replace("\"", "&quot");
                            shape.Text = shapeName;
                            shape.TextVisibility = TextVisibility.Shown;
                            shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;

                            shape.CentralPointOffset.Y = mapCalloutOffsetY;
                        }
                        else
                        {
                            cell = new LegendCell("-");
                            cell.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
                            item.Cells.Add(cell);

                            shape.Color = Color.LightSkyBlue;
                            shape.ToolTip = string.Format("{0}", subjectName);
                            shape.Text = shapeName;
                        }
                    }
                }
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

            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.9));
   
            ReportPDFExporter1.HeaderCellHeight = 70;

            UltraWebGrid.Rows[0].Cells[0].Style.BorderDetails.WidthRight = 0;
            UltraWebGrid.Rows[0].Cells[1].Style.BorderDetails.WidthLeft = 0;
            UltraWebGrid.Rows[0].Cells[1].Style.BorderDetails.WidthRight = 0;
            UltraWebGrid.Rows[0].Cells[2].Style.BorderDetails.WidthLeft = 0;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(DundasMap, mapCaption.Text, section2);

        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");

            ReportExcelExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.SheetColumnCount = 20;

            ReportExcelExporter1.GridColumnWidthScale = 1;
            WorksheetMergedCellsRegion mergedRegion = sheet1.MergedCellsRegions.Add(4,0,4,2);
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);

            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.7));

            ReportExcelExporter1.Export(DundasMap, mapCaption.Text, sheet2, 3);
           
        }

        #endregion
    }
}
