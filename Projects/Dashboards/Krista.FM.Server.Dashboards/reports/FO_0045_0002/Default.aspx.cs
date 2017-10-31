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
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0045_0002
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable populationDt;
        private DataTable dtGrid;
        private DataTable dtChart;
        private int firstYear = 2009;
        private int endYear = 2012;
        private int selectedMonthIndex;
        private int selectedYear;
        private DateTime currentDateTime;

        private GridHeaderLayout headerLayout;

        // имя папки с картами региона
        private string mapFolderName;
        // пропорция карты
        private double mapSizeProportion;
        // масшбтаб карты
        private double mapZoomValue;
        // сдвиг по вертикали текста городов-выносок карты
        private double mapCalloutOffsetY;

        #endregion

        #region Параметры запроса

        // численность текущего года
        private CustomParam currentYearPopulation;
        // численность прошлого года
        private CustomParam lastYearPopulation;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

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

            double scale = 0.9;

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale - 220);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.58);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph +=new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight);


            #region Инициализация параметров запроса

            currentYearPopulation = UserParams.CustomParam("current_year_population");
            lastYearPopulation = UserParams.CustomParam("last_year_population");

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.StackColumnChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Data.SwapRowsAndColumns = true;

            UltraChart.Axis.X.Extent = 120;
            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);

            UltraChart.Axis.Y.Extent = 70;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value) / 5;
            UltraChart.Legend.Font = new Font("Verdana", 8);
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 8;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Text = "Тыс.руб.";
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);

            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N1> тыс.руб.";

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            CrossLink1.Visible = true;
            CrossLink1.Text = "Государственный&nbsp;долг&nbsp;субъекта&nbsp;РФ";
            CrossLink1.NavigateUrl = "~/reports/FO_0045_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Долговая&nbsp;нагрузка&nbsp;";
            CrossLink2.NavigateUrl = "~/reports/FO_0045_0003/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0045_0002_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month, true);
            }
            
            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedMonthIndex = ComboMonth.SelectedIndex + 1;
            currentDateTime = new DateTime(selectedYear, selectedMonthIndex, 1);
            DateTime nextMonthDateTime = currentDateTime.AddMonths(1);

            Page.Title = String.Format("Объем муниципального долга на душу населения");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("по состоянию на 1 {0} {1} года", CRHelper.RusMonthGenitive(nextMonthDateTime.Month), nextMonthDateTime.Year);
            chartCaption.Text = "Структура муниципального долга субъекта в разрезе МО";
            mapCaption.Text = "Муниципальный долг субъекта";

            UserParams.PeriodYear.Value = currentDateTime.Year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDateTime.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDateTime.Month));
            UserParams.PeriodMonth.Value = String.Format(CRHelper.RusMonth(currentDateTime.Month));

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            UltraChart.DataBind();

            DundasMap.Shapes.Clear();
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Complete");
            DundasMap.ShapeFields["Complete"].Type = typeof(double);
            DundasMap.ShapeFields["Complete"].UniqueIdentifier = false;

            SetMapSettings();
            AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);
            AddMapLayer(DundasMap, mapFolderName, "Города", CRHelper.MapShapeType.Towns);
            AddMapLayer(DundasMap, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);

            // заполняем карту данными
            FillMapData();
        }

        #region Обработчики грида
        
        private bool IsEmptyDataTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0045_0002_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid);
            if (dtGrid.Rows.Count > 0 && !IsEmptyDataTable(dtGrid))
            {
                query = DataProvider.GetQueryText("FO_0045_0002_population");
                populationDt = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Численность населения", populationDt);

                const string populationColumnName = "Численность постоянного населения";

                if (dtGrid.Columns.Count > 1 && dtGrid.Rows.Count > 0)
                {
                    dtGrid.PrimaryKey = new DataColumn[] {dtGrid.Columns[0]};

                    foreach (DataRow populationRow in populationDt.Rows)
                    {
                        if (populationRow[0] != DBNull.Value)
                        {
                            string rowName = populationRow[0].ToString();

                            DataRow dtRow = dtGrid.Rows.Find(rowName);
                            if (dtRow != null)
                            {
                                double population = 0;
                                if (populationRow[populationColumnName] != DBNull.Value && populationRow[populationColumnName].ToString() != String.Empty)
                                {
                                     population = Convert.ToDouble(populationRow[populationColumnName]);
                                }

                                foreach (DataColumn column in dtGrid.Columns)
                                {
                                    if (column.ColumnName.Contains("Сумма государственного долга на душу населения"))
                                    {
                                        if (population != 0 && dtRow[column.ColumnName] != DBNull.Value && dtRow[column.ColumnName].ToString() != String.Empty)
                                        {
                                            double debt = Convert.ToDouble(dtRow[column.ColumnName]);
                                            double debtPercent = debt / population;

                                            dtRow[column.ColumnName] = debtPercent;
                                        }
                                        else
                                        {
                                            dtRow[column.ColumnName] = DBNull.Value;
                                        }
                                    }
                                }
                            }

                            
                        }
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count == 0 )
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(110);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;
                string formatString = columnName.ToLower().Contains("темп роста") ? "P1" : "N1";
                int columnWidth = columnName.ToLower().Contains("темп роста") ? 75 : 95;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = columnWidth;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            string currYearCaption = (String.Format("На {0:dd.MM.yyyy}", currentDateTime.AddMonths(1)));

            headerLayout.AddCell("Наименование МО");

            GridHeaderCell cell = headerLayout.AddCell("Итого муниципальный долг");
            cell.AddCell(String.Format("{0}, тыс.руб.", currYearCaption), String.Format("Сумма муниципального долга {0}", currYearCaption.ToLower()), 2);
            cell.AddCell("Темп роста, %", "Темп роста/снижения муниципального долга относительно прошлого отчётного периода", 2);
            cell.AddCell("Долг на душу населения, тыс.руб./чел.", String.Format("Сумма муниципального долга на душу населения {0}", currYearCaption.ToLower()), 2);

            GridHeaderCell groupCell = headerLayout.AddCell("В том числе по видам долговых обязательств:");
            cell = groupCell.AddCell("Бюджетные кредиты из бюджета");
            cell.AddCell(String.Format("{0}, тыс.руб.", currYearCaption), String.Format("Сумма бюджетных кредитов {0}", currYearCaption.ToLower()));
            cell.AddCell("Темп роста, %", "Темп роста/снижения объема бюджетных кредитов относительно прошлого отчётного периода");
            cell.AddCell("Долг на душу населения, тыс.руб./чел.", String.Format("Сумма бюджетных кредитов на душу населения {0}", currYearCaption.ToLower()));

            cell = groupCell.AddCell("Кредиты кредитных организаций");
            cell.AddCell(String.Format("{0}, тыс.руб.", currYearCaption), String.Format("Сумма кредитов кредитных организаций {0}", currYearCaption.ToLower()));
            cell.AddCell("Темп роста, %", "Темп роста/снижения объема кредитов кредитных организаций относительно прошлого отчётного периода");
            cell.AddCell("Долг на душу населения, тыс.руб./чел.", String.Format("Сумма кредитов кредитных организаций на душу населения {0}", currYearCaption.ToLower()));

            cell = groupCell.AddCell("Муниципальные гарантии");
            cell.AddCell(String.Format("{0}, тыс.руб.", currYearCaption), String.Format("Сумма муниципальных гарантий {0}", currYearCaption.ToLower()));
            cell.AddCell("Темп роста, %", "Темп роста/снижения объема муниципальных гарантий относительно прошлого отчётного периода");
            cell.AddCell("Долг на душу населения, тыс.руб./чел.", String.Format("Сумма муниципальных гарантий на душу населения {0}", currYearCaption.ToLower()));

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string rowName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                rowName = e.Row.Cells[0].Value.ToString();
            }

            bool totalRow = rowName.ToLower().Contains("итого");

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                string columnName = e.Row.Band.Grid.Columns[i].Header.Caption.ToLower();

                bool rateColumn = columnName.Contains("темп роста");

                if (totalRow)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }

                if (rateColumn)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (100*Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            e.Row.Cells[i].Title = "Прирост к прошлому году";
                        }
                        else if (100*Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            e.Row.Cells[i].Title = "Снижение к прошлому году";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }


                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != String.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (rateColumn && value > 1)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0045_0002_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Columns.Count > 0 && !IsEmptyDataTable(dtChart))
            {
                foreach (DataColumn column in dtChart.Columns)
                {
                    //column.ColumnName = column.ColumnName.Replace("автономного округа", String.Empty);
                    column.ColumnName = CRHelper.ToUpperFirstSymbol(column.ColumnName);
                }

                UltraChart.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart.Series.Add(series);
                }
            }
            else
            {
                UltraChart.DataSource = null;
            }
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            int seriesNum = 0;
            if (dtChart != null && dtChart.Rows.Count > 0)
            {
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                    {
                        Text axisText = (Text)primitive;
                        axisText.bounds.Width = 30;
                        axisText.labelStyle.HorizontalAlign = StringAlignment.Far;
                        axisText.labelStyle.VerticalAlign = StringAlignment.Center;
                        axisText.labelStyle.FontSizeBestFit = false;
                        axisText.labelStyle.Font = new Font("Verdana", 8);
                        axisText.labelStyle.WrapText = false;
                    }
                    if (primitive is Box && primitive.Path == "Legend")
                    {
                        Box box = (Box) primitive;
                        box.rect.X += 20 * seriesNum;
                        seriesNum++;
                    }
                    else if (primitive is Text && String.IsNullOrEmpty(primitive.Path))
                    {
                        Text legeng = (Text) primitive;
                        legeng.bounds.X += 20 * seriesNum;
                        
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

            // добавляем легенду
            Legend legend = new Legend("CompleteLegend");
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
            legend.Title = "Муниципальный долг\nна душу населения, тыс.руб./чел.";
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Complete";
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
            rule.ShowInLegend = "CompleteLegend";
            DundasMap.ShapeRules.Add(rule);
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


        public void FillMapData()
        {
            if (dtGrid == null || DundasMap == null)
            {
                return;
            }

            foreach (Shape shape in DundasMap.Shapes)
            {
                string shapeName = GetShapeName(shape);
                if (!RegionsNamingHelper.LocalBudgetUniqueNames.ContainsKey(shapeName))
                {
                    shape.Visible = false;
                }
            }

            bool nullShapesExists = false;
            foreach (DataRow row in dtGrid.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != String.Empty)
                {
                    string subject = row[0].ToString();
                    
                    ArrayList shapeList = FindMapShape(DundasMap, subject);
                    foreach (Shape shape in shapeList)
                    {
                        string shapeName = GetShapeName(shape);

                        if (row[3] != DBNull.Value && row[3].ToString() != String.Empty)
                        {
                            double value = Convert.ToDouble(row[3]);

                            shape["Name"] = subject;
                            shape["Complete"] = value;
                            shape.ToolTip = String.Format("{0}\nМуниципальный долг на душу населения\n#COMPLETE{{N1}} тыс.руб./чел.", shapeName);

                            if (IsCalloutTownShape(shape))
                            {
                                shape.Text = String.Format("{0}\n{1:N1} тыс.руб./чел.", shapeName, value);
                                shape.TextVisibility = TextVisibility.Shown;
                                shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;

                                shape.CentralPointOffset.Y = mapCalloutOffsetY;
                            }
                            else
                            {
                                shape.Text = String.Format("{0}\n{1:N1} тыс.руб./чел.", shapeName.Replace(" ", "\n"), value);
                            }
                        }
                        else
                        {
                            if (!nullShapesExists && DundasMap.Legends.Count > 0)
                            {
                                LegendItem item = new LegendItem();
                                item.Text = "Нет долга";
                                item.Color = Color.SkyBlue;
                                DundasMap.Legends[0].Items.Add(item);
                            }
                            nullShapesExists = true;
                            
                            shape.Color = Color.SkyBlue;
                            shape.ToolTip = String.Format("{0}\nнет долга", shapeName.Replace(" ", "\n"));
                            shape.Text = shapeName.Replace(" ", "\n");

                            if (IsCalloutTownShape(shape))
                            {
                                shape.TextVisibility = TextVisibility.Shown;
                                shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;

                                shape.CentralPointOffset.Y = mapCalloutOffsetY;
                            }
                        }
                    }
                }
            }
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
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");

            ReportExcelExporter1.SheetColumnCount = 20;
            
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.9));
            UltraChart.Legend.Margins.Right = 0;
            ReportExcelExporter1.Export(UltraChart, chartCaption.Text, sheet2, 3);

            DundasMap.NavigationPanel.Visible = false;
            DundasMap.ZoomPanel.Visible = false;
            ReportExcelExporter1.Export(DundasMap, mapCaption.Text, sheet3, 3);
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
            ISection section3 = report.AddSection();

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 0;

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart, chartCaption.Text, section2);

            DundasMap.NavigationPanel.Visible = false;
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            ReportPDFExporter1.Export(DundasMap, section3);
        }

        #endregion
    }
}