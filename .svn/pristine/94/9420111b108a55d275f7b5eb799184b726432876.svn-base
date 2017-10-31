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
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace Krista.FM.Server.Dashboards.reports.FO_0045_0003_Sakhalin
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable populationDt;
        private DataTable dtGrid;
        private int firstYear = 2009;
        private int endYear = 2012;
        private int selectedMonthIndex;
        private int selectedYear;
        private DateTime currentDateTime;
        private string selectedMeasureCaption;
        private bool useRegionCodes = true;

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

        private bool FactSelected
        {
            get { return MeasureButtonList.SelectedIndex == 1; }
        }

        private bool DecemberSelected
        {
            get { return ComboMonth.SelectedValue.ToLower() == "декабрь"; }
        }

        #region Параметры запроса

        // выбранная мера
        private CustomParam selectedMeasure;

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

            #region Инициализация параметров запроса

            selectedMeasure = UserParams.CustomParam("selected_measure");

            #endregion

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale - 220);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            CrossLink2.Visible = true;
            CrossLink2.Text = "Муниципальный&nbsp;долг";
            CrossLink2.NavigateUrl = "~/reports/FO_0045_0002_Sakhalin/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                DateTime lastDate = CubeInfo.GetLastDate(DataProvidersFactory.PrimaryMASDataProvider, "FO_0045_0003_Sakhalin_date");

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboYear.SetСheckedState(lastDate.Year.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(lastDate.Month)), true);
            }
            
            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedMonthIndex = ComboMonth.SelectedIndex + 1;
            currentDateTime = new DateTime(selectedYear, selectedMonthIndex, 1);
            DateTime nextMonthDateTime = currentDateTime.AddMonths(1);

            Page.Title = String.Format("Долговая нагрузка на бюджеты");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("по состоянию на 1 {0} {1} года", CRHelper.RusMonthGenitive(nextMonthDateTime.Month), nextMonthDateTime.Year);
            mapCaption.Text = "Уровень долговой нагрузки";

            UserParams.PeriodYear.Value = currentDateTime.Year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDateTime.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDateTime.Month));
            UserParams.PeriodMonth.Value = String.Format(CRHelper.RusMonth(currentDateTime.Month));

            MeasureButtonList.Visible = DecemberSelected;
            selectedMeasure.Value = DecemberSelected && FactSelected ? "Факт" : "Годовые назначения";
            selectedMeasureCaption = DecemberSelected && FactSelected ? "факт" : "план";

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

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
            //AddMapLayer(DundasMap, mapFolderName, "Города", CRHelper.MapShapeType.Towns);
            //AddMapLayer(DundasMap, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);

            // заполняем карту данными
            FillMapData();
        }

        #region Обработчики грида
        
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0045_0003_Sakhalin_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
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

        private string GetColumnFormat(string columnName)
        {
            if (columnName.ToLower().Contains("темп роста") || columnName.ToLower().Contains("удельный вес"))
            {
                return "P1";
            }
            if (columnName.ToLower().Contains("ранг"))
            {
                return "N0";
            } if (columnName.ToLower().Contains("уровень"))
            {
                return "P2";
            }
            return "N1";
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

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(240);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 1)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;
                string formatString = GetColumnFormat(columnName);
                int columnWidth = 125;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = true;

            string currYearCaption = (String.Format("На {0:dd.MM.yyyy}", currentDateTime.AddMonths(1)));

            headerLayout.AddCell("Бюджет");
            headerLayout.AddCell("Объем долга, тыс.руб.", String.Format("Сумма долга {0}", currYearCaption.ToLower()));
            headerLayout.AddCell(
                   String.Format("Доходы бюджета за исключением субвенций ({0}), тыс.руб.", selectedMeasureCaption),
                   String.Format("Доходы бюджета за исключением субвенций  ({1}) {0}", currYearCaption.ToLower(), selectedMeasureCaption));
            headerLayout.AddCell("Уровень долговой нагрузки, %", String.Format("Индикатор долговой нагрузки {0}", currYearCaption.ToLower()));
            headerLayout.AddCell("Ранг по долговой нагрузке", "Ранг (место) МО по долговой нагрузке");
            headerLayout.AddCell("Темп роста долговой нагрузки, %", "Темп роста/ снижения долговой нагрузки по сравнению с аналогичным периодом прошлого года");
            headerLayout.AddCell("Просроченная задолженность по долговым обязательствам, тыс.руб", "Объем просроченной задолженности по государственным и муниципальным долговым обязательствам");
            headerLayout.AddCell("Удельный вес просроченной задолженности по долговым обязательствам, %", "Удельный вес просроченной задолженности по долговым обязательствам в общем объеме долговых обязательств");

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string rowName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                rowName = e.Row.Cells[0].Value.ToString();
            }

            int worseRankColumnIndex = e.Row.Cells.Count - 2;
            bool totalRow = rowName.ToLower().Contains("юджет субъекта");

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                string columnName = e.Row.Band.Grid.Columns[i].Header.Caption.ToLower();

                bool rateColumn = columnName.Contains("темп роста");
                bool rankColumn = columnName.Contains("ранг");

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

                if (rankColumn)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[worseRankColumnIndex].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[worseRankColumnIndex].Value.ToString() != string.Empty)
                    {
                        int rank = Convert.ToInt32(e.Row.Cells[i].Value);
                        int worseRank = Convert.ToInt32(e.Row.Cells[worseRankColumnIndex].Value);
                        if (rank == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("Самый низкий уровень долговой нагрузки");
                        }
                        else if (rank == worseRank)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = string.Format("Самый высокий уровень долговой нагрузки");
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
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
            legend.Title = "Уровень долговой нагрузки";
            legend.AutoFitMinFontSize = 7;                       

            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);
            if (useRegionCodes)
            {
                // добавляем легенду для подписей
                legend = new Legend("SubjectLegend");
                legend.Margins.Top = 200;
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
                legend.Title = String.Empty;
                legend.AutoFitMinFontSize = 7;
                legend.ItemColumnSpacing = 100;

                LegendCellColumn column = new LegendCellColumn("Код");
                column.HeaderFont = new Font("Verdana", 8, FontStyle.Bold);
                column.HeaderTextAlignment = StringAlignment.Far;
                legend.CellColumns.Add(column);

                column = new LegendCellColumn("Территория");
                column.HeaderFont = new Font("Verdana", 8, FontStyle.Bold);
                column.HeaderTextAlignment = StringAlignment.Center;
                legend.CellColumns.Add(column);

                DundasMap.Legends.Add(legend);
            }
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
            rule.LegendText = "#FROMVALUE{P2} - #TOVALUE{P2}";
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
                    string code = row[dtGrid.Columns.Count-1].ToString();
                    ArrayList shapeList = FindMapShape(DundasMap, subject);
                    foreach (Shape shape in shapeList)
                    {
                        //string shapeName = GetShapeName(shape);
                        subject = subject.Replace("Муниципальный район", "МР");
                        subject = subject.Replace("муниципальный раойн", "МР");
                        subject = subject.Replace("Городской округ", "ГО");
                        subject = subject.Replace("городской округ", "ГО");
                        subject = subject.Replace("\"", "'");
                        string shapeTitle = subject;
                        if (useRegionCodes)
                        {
                            shapeTitle = code;

                            LegendItem item = new LegendItem();

                            LegendCell cell = new LegendCell(code);
                            cell.Alignment = ContentAlignment.MiddleRight;
                            item.Cells.Add(cell);

                            cell = new LegendCell(subject);
                            cell.Alignment = ContentAlignment.MiddleLeft;
                            item.Cells.Add(cell);

                            DundasMap.Legends["SubjectLegend"].Items.Add(item);

                            shape.ToolTip = String.Format("{1} ({0})\n#COMPLETEPERCENT{{N1}} руб./чел.", shapeTitle, subject);
                        }
                        if (row[3] != DBNull.Value && row[3].ToString() != String.Empty)
                        {
                            double value = Convert.ToDouble(row[3]);

                            shape["Name"] = subject;
                            shape["Complete"] = value;
                            shape.ToolTip = String.Format("{0} ({1})\nУровень долговой нагрузки\n#COMPLETE{{P2}}", subject, shapeTitle);
                            shape.TextVisibility = TextVisibility.Shown;
                            SetShapeMargins(shape);
                            if (IsCalloutTownShape(shape))
                            {
                                shape.Text = String.Format("{0}\n{1:P2}", shapeTitle.Replace(" ", "\n"), value);
                                shape.TextAlignment = ContentAlignment.BottomCenter;
                            }
                            else
                            {
                                shape.Text = String.Format("{0}\n{1:P2}", shapeTitle.Replace(" ", "\n"), value);
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
                            shape.ToolTip = String.Format("{0} ({1})\nнет долга", subject, shapeTitle);
                            shape.Text = shapeTitle.Replace(" ", "\n");

                            if (IsCalloutTownShape(shape))
                            {
                                shape.TextAlignment = ContentAlignment.BottomCenter;
                            }
                        }
                    }
                }
            }
        }
        private void SetShapeMargins(Shape shape)
        {
            if (mapFolderName == "Сахалин")
            {
                if (shape.Name.Contains("Корсаковский"))
                {
                    shape.CentralPointOffset.X = 0.7;
                }
                if (shape.Name.Contains("Холмский"))
                {
                    shape.CentralPointOffset.X = -0.5;
                }
                if (shape.Name.Contains("Углегорский"))
                {
                    shape.CentralPointOffset.X = -0.5;
                }
                if (shape.Name.Contains("Невельский"))
                {
                    shape.CentralPointOffset.Y = -0.5;
                }
                if (shape.Name.Contains("Анивский"))
                {
                    shape.CentralPointOffset.Y = -0.3;
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

            ReportExcelExporter1.SheetColumnCount = 20;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);

            DundasMap.NavigationPanel.Visible = false;
            DundasMap.ZoomPanel.Visible = false;
            ReportExcelExporter1.Export(DundasMap, mapCaption.Text, sheet2, 3);
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

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);



            IText title = section2.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = section2.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);
            DundasMap.NavigationPanel.Visible = false;
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.9));
            DundasMap.Height = Unit.Pixel((int)(CustomReportConst.minScreenHeight * 0.8));
            ReportPDFExporter1.Export(DundasMap, mapCaption.Text, section2);
        }

        #endregion
    }
}