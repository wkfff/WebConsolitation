using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using ContentAlignment = System.Drawing.ContentAlignment;
using SerializationFormat=Dundas.Maps.WebControl.SerializationFormat;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0007_Yaroslavl
{
    public partial class DefaultCompare : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtAVG;
        private int firstYear = 2008;
        private int endYear = 2011;
        private int avgOutcomesValue = Int32.MinValue;
        private MemberAttributesDigest rzprDigest;

        private GridHeaderLayout headerLayout;
        #region Параметры запроса

        // расходы Итого
        private CustomParam outcomesTotal;
        // группа ФКР
        private CustomParam fkrGroupName;
        // выбранное значение группы ФКР
        private CustomParam fkrGroupComboValue;
        // уровень МР и ГО
        private CustomParam regionsLevel;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;

        // численность населения
        private CustomParam populationMeasure;
        // год для численности населения
        private CustomParam populationMeasureYear;

        // тип документа СКИФ для консолидированного уровня
        private CustomParam consolidateDocumentSKIFType;
        // тип документа СКИФ для уровня районов
        private CustomParam regionDocumentSKIFType;

        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        private CustomParam useConsolidateMO;
        // имя папки с картами региона
        private string mapFolderName;
        // пропорция карты
        private double mapSizeProportion;
        // масшбтаб карты
        private double mapZoomValue;
        // сдвиг по вертикали текста городов-выносок карты
        private double mapCalloutOffsetY;


        #endregion

        private bool UseConsolidateRegionBudget
        {
            get { return useConsolidateRegionBudget.Checked; }
        }
       

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
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

            if (outcomesTotal == null)
            {
                outcomesTotal = UserParams.CustomParam("outcomes_total");
            }
            if (fkrGroupName == null)
            {
                fkrGroupName = UserParams.CustomParam("fkr_group_name");
            }
            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (regionsConsolidateLevel == null)
            {
                regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            }

            if (populationMeasure == null)
            {
                populationMeasure = UserParams.CustomParam("population_measure");
            }
            if (populationMeasureYear == null)
            {
                populationMeasureYear = UserParams.CustomParam("population_measure_year");
            }

            if (consolidateDocumentSKIFType == null)
            {
                consolidateDocumentSKIFType = UserParams.CustomParam("consolidate_document_skif_type");
            }
            if (regionDocumentSKIFType == null)
            {
                regionDocumentSKIFType = UserParams.CustomParam("region_document_skif_type");
            }

            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }
            fkrGroupComboValue = UserParams.CustomParam("fkr_group_combo_value");
            useConsolidateMO = UserParams.CustomParam("use_mo");
            #endregion

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * mapSizeProportion - 225);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 12);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight);

            CrossLink1.Text = "Расходы&nbsp;субъекта&nbsp;РФ&nbsp;подробнее";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0006/DefaultDetail.aspx";
 
            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            outcomesTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0007_HMAO_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.AutoPostBack = true;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
                
                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);

                fkrGroupComboValue.Value = "Расходы бюджета - ИТОГО";
            }

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

            Page.Title = string.Format("Исполнение собственных расходных полномочий");
            Label1.Text = Page.Title;
            Label2.Text = string.Format(" за {0}&nbsp;{1}&nbsp;{2}&nbsp;года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);
            MoPercentLabel.Text = "* - выводится доля расходов бюджета в консолидированном бюджете субъекта РФ";
            
            populationMeasure.Value = RegionSettingsHelper.Instance.PopulationMeasure;
            populationMeasureYear.Value = RegionSettingsHelper.Instance.PopulationMeasurePlanning ? (yearNum + 1).ToString() : yearNum.ToString();

            consolidateDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");

            regionBudgetSKIFLevel.Value = UseConsolidateRegionBudget
                                              ? "[Уровни бюджета].[СКИФ].[Бюджет района и поселений]"
                                              : "[Уровни бюджета].[СКИФ].[Бюджет района и города]";
            useConsolidateMO.Value = UseConsolidateRegionBudget
                                              ? "   [Уровни бюджета].[СКИФ].[Все].[Бюджет района],[Уровни бюджета].[СКИФ].[Все].[Бюджет поселения]"
                                              : "  [Уровни бюджета].[СКИФ].[Все].[Бюджет района], [Уровни бюджета].[СКИФ].[Бюджет гор.округа]";
            AVGDataBind();

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            string patternValue = UserParams.Subject.Value;
            int defaultRowIndex = 1;
            if (patternValue == string.Empty)
            {
                patternValue = UserParams.StateArea.Value;
                defaultRowIndex = 0;
            }

            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
            ActiveGridRow(row);

            DundasMap.Shapes.Clear();
            if (Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("MapLoadFromBin")))
            {
                DundasMap.Serializer.Format = SerializationFormat.Binary;
                DundasMap.Serializer.Load((Server.MapPath(string.Format("../../maps/Субъекты/{0}/{0}_выноски.bin", mapFolderName))));
                SetMapSettings();
            }
            else
            {
                //DundasMap.ShapeFields.Clear();
                DundasMap.ShapeFields.Add("Name");
                DundasMap.ShapeFields["Name"].Type = typeof(string);
                DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
                DundasMap.ShapeFields.Add("Complete");

                DundasMap.ShapeFields["Complete"].Type = typeof(double);
                DundasMap.ShapeFields["Complete"].UniqueIdentifier = false;
                DundasMap.ShapeFields.Add("CompletePercent");
                DundasMap.ShapeFields["CompletePercent"].Type = typeof(double);
                DundasMap.ShapeFields["CompletePercent"].UniqueIdentifier = false;

                SetMapSettings();
                AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);
                AddMapLayer(DundasMap, mapFolderName, "Города", CRHelper.MapShapeType.Towns);
                AddMapLayer(DundasMap, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);
            }
                        
            // заполняем карту данными
            FillMapData();
        }

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
            legend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.AutoFitText = true;
            legend.Title = "Бюдежетные расходы\nна душу населения,\nруб./чел.";
            legend.TitleSeparator = LegendSeparatorType.GradientLine;
            legend.AutoFitMinFontSize = 7;
            legend.EquallySpacedItems = false;
            legend.InterlacedRows = false;
            legend.InterlacedRowsColor = Color.Empty;
            legend.TextWrapThreshold = 25;
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);

            DundasMap.Legends[0].LegendStyle = LegendStyle.Table;
            DundasMap.Legends[0].Items.Add("LegendItem");
            DundasMap.Legends[0].Items["LegendItem"].Cells.Add(LegendCellType.Text, "Cell1", ContentAlignment.TopCenter);
            DundasMap.Legends[0].Items["LegendItem"].Cells.Add(LegendCellType.Text, "Cell2", ContentAlignment.TopCenter);
            DundasMap.Legends[0].Items["LegendItem"].Cells["Cell1"].CellType = LegendCellType.Text;
            DundasMap.Legends[0].Items["LegendItem"].Cells["Cell1"].CellSpan = 2;
            DundasMap.Legends[0].Items["LegendItem"].Cells["Cell1"].Text = string.Empty;
            DundasMap.Legends[0].Items["LegendItem"].Cells["Cell2"].CellType = LegendCellType.Text;
            
            // добавляем поля
//            DundasMap.ShapeFields.Clear();
//            DundasMap.ShapeFields.Add("Name");
//            DundasMap.ShapeFields["Name"].Type = typeof(string);
//            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
//            DundasMap.ShapeFields.Add("Complete");
//       
//            DundasMap.ShapeFields["Complete"].Type = typeof(double);
//            DundasMap.ShapeFields["Complete"].UniqueIdentifier = false;
//            DundasMap.ShapeFields.Add("CompletePercent");
//            DundasMap.ShapeFields["CompletePercent"].Type = typeof(double);
//            DundasMap.ShapeFields["CompletePercent"].UniqueIdentifier = false;

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "CompletePercent";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Green;
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

            foreach (DataRow row in dtGrid.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty &&
                    row[5] != DBNull.Value && row[5].ToString() != string.Empty)
                {
                    string subject = row[0].ToString();
                    ArrayList shapeList = FindMapShape(DundasMap, subject);
                    foreach (Shape shape in shapeList)
                    {
                        string shapeName = GetShapeName(shape);
                        double value = Convert.ToDouble(row[5]);
                        shape["Name"] = subject;
                        shape["Complete"] = Convert.ToDouble(row[1]);
                        shape["CompletePercent"] = Convert.ToDouble(row[5]);
                        shape.ToolTip = string.Format("{0}\n#COMPLETEPERCENT{{N2}} руб./чел.", shapeName);

                        if (IsCalloutTownShape(shape))
                        {
                            shape.Text = string.Format("{0}\n{1:N2}\nруб./чел.", shapeName, value);
                            shape.TextVisibility = TextVisibility.Shown;
                            shape.TextAlignment = ContentAlignment.BottomCenter;
                            shape.CentralPointOffset.Y = mapCalloutOffsetY;
                        }
                        else
                        {
                            shape.Text = string.Format("{0}\n{1:N2} руб./чел.", shapeName.Replace(" ", "\n"), value);
                        }
                    }
                }
            }
        }

        #endregion

        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            UserParams.Subject.Value = row.Cells[0].Text;
        }

        private bool IsRankingRow(string rowName)
        {
            return rowName != "Консолидированный бюджет субъекта" && rowName != "Местные бюджеты" &&
                rowName != RegionSettingsHelper.Instance.OwnSubjectBudgetName;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0007_HMAO_compare_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Бюджет", dtGrid);

            const string populationColumnName = "Численность постоянного населения";
            const string executeColumnName = "Факт ";
            const string avgExecuteColumnName = "Коэффициент бюджетной обеспеченности населения";
            const string avgExecuteRankColumnName = "Ранг бюдж.обесп. МО";
            const string avgExecuteWorseRankColumnName = "Худший ранг бюдж.обесп. МО";

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[1] != DBNull.Value)
                    {
                        row[1] = Convert.ToDouble(row[1])/1000;
                    }
                    if (row[2] != DBNull.Value)
                    {
                        row[2] = Convert.ToDouble(row[2])/1000;
                    }
                }

                query = DataProvider.GetQueryText("FO_0002_0007_HMAO_compare_population");
                DataTable populationDT = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Численность населения", populationDT);

                RankCalculator avgExecuteRank = new RankCalculator(RankDirection.Asc);

                if (dtGrid.Columns.Count > 1 && dtGrid.Rows.Count > 0)
                {
                    dtGrid.PrimaryKey = new DataColumn[] { dtGrid.Columns[0] };

                    foreach (DataRow populationRow in populationDT.Rows)
                    {
                        if (populationRow[0] != DBNull.Value)
                        {
                            string rowName = populationRow[0].ToString();
                            if (populationRow[populationColumnName] != DBNull.Value && populationRow[populationColumnName].ToString() != String.Empty)
                            {
                                double population = Convert.ToDouble(populationRow[populationColumnName]);

                                DataRow dtRow = dtGrid.Rows.Find(rowName);
                                if (dtRow != null)
                                {
                                    dtRow[populationColumnName] = population;

                                    if (population != 0 && dtRow[executeColumnName] != DBNull.Value && dtRow[executeColumnName].ToString() != String.Empty)
                                    {
                                        double execute = Convert.ToDouble(dtRow[executeColumnName]);
                                        double avgExecute = execute / population;

                                        dtRow[avgExecuteColumnName] = avgExecute;

                                        if (avgExecute != 0 && IsRankingRow(rowName))
                                        {
                                            avgExecuteRank.AddItem(avgExecute);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    double avgExecuteWorseRank = avgExecuteRank.GetWorseRank();

                    // пробегаем еще раз, чтобы расставить ранги
                    foreach (DataRow row in dtGrid.Rows)
                    {
                        string rowName = row[0].ToString();
                        if (IsRankingRow(rowName) && row[avgExecuteColumnName] != DBNull.Value && row[avgExecuteColumnName].ToString() != String.Empty)
                        {
                            double value = Convert.ToDouble(row[avgExecuteColumnName]);
                            int rank = avgExecuteRank.GetRank(value);
                            if (value != 0 && rank != 0)
                            {
                                row[avgExecuteRankColumnName] = rank;
                                row[avgExecuteWorseRankColumnName] = avgExecuteWorseRank;
                            }
                            else
                            {
                                row[avgExecuteRankColumnName] = DBNull.Value;
                            }
                        }
                        else
                        {
                            row[avgExecuteRankColumnName] = DBNull.Value;
                        }
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count != 0)
            {
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(245);
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

              
                    SetColumnParams(e.Layout, 0, 1, "N2", 110, false);
                    SetColumnParams(e.Layout, 0, 2, "N2", 110, false);
                    SetColumnParams(e.Layout, 0, 3, "P2", 110, false);
                    SetColumnParams(e.Layout, 0, 4, "N0", 110, false);
                    SetColumnParams(e.Layout, 0, 5, "N2", 110, false);
                    SetColumnParams(e.Layout, 0, 6, "N0", 100, false);
                    SetColumnParams(e.Layout, 0, 7, "P2", 100, false);
                    SetColumnParams(e.Layout, 0, 8, "N0", 100, false);
                    SetColumnParams(e.Layout, 0, 9, "N0", 100, true);
                    SetColumnParams(e.Layout, 0, 10, "N0", 100, true);
                    
              

                headerLayout.AddCell("Бюджет");
                headerLayout.AddCell("Уточненные годовые назначения, тыс.руб", "План на год");
                headerLayout.AddCell("Исполнено, тыс.руб", "Фактическое исполнение нарастающим итогом с начала года");
                headerLayout.AddCell("Исполнено %", "Процент выполнения назначений. Оценка равномерности исполнения (1/12 годового плана в месяц)");
                headerLayout.AddCell("Численность постоянного населения, чел.", "Численность постоянного населения");
                headerLayout.AddCell("Бюджетные расходы на душу населения,\nтыс. руб./чел.", "Бюджетные расходы на душу населения");
                headerLayout.AddCell("Ранг бюдж. расх. МО", "Ранг (место) бюджета по бюджетным расходам на душу населения среди бюдежетов МО");
                headerLayout.AddCell("Доля *", "Доля расхода в общей сумме расходов бюджета");
                headerLayout.AddCell("Ранг доля МО", "Ранг (место) бюджета по доле расхода в общей сумме расходов среди бюджетов МО");
                
                headerLayout.ApplyHeaderInfo();
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Top = 5;
                e.Row.Cells[i].Style.Padding.Bottom = 5;

                bool rank = (i == 6 );
                bool complete = (i == 3);

                if (rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 3].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 3].Value.ToString() != string.Empty)
                    {
                        string region = "МО";
                        string obj = (i == 6) ? "бюджетные расходы на душу населения" : "доля расхода";
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            string bestStr = (i == 6) ? "Самые высокие" : "Самая высокая";
                            e.Row.Cells[i].Title = string.Format("{2} {1} в {0}", region, obj, bestStr);
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 3].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            string bestStr = (i == 6) ? "Самые низкие" : "Самая низкая";
                            e.Row.Cells[i].Title = string.Format("{2} {1} в {0}", region, obj, bestStr);
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }

                if (i==8)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        string region = "МО";
                        string obj = (i == 6) ? "бюджетные расходы на душу населения" : "доля расхода";
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            string bestStr = (i == 6) ? "Самые высокие" : "Самая высокая";
                            e.Row.Cells[i].Title = string.Format("{2} {1} в {0}", region, obj, bestStr);
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            string bestStr = (i == 6) ? "Самые низкие" : "Самая низкая";
                            e.Row.Cells[i].Title = string.Format("{2} {1} в {0}", region, obj, bestStr);
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }

                if (complete)
                {
                    if (avgOutcomesValue != Int32.MinValue && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        int value = Convert.ToInt32(100 * Convert.ToDouble(e.Row.Cells[i].Value));
                        if (value < avgOutcomesValue)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerRed.gif";
                            e.Row.Cells[i].Title = string.Format("Ниже среднего исполнения по субъекту РФ ({0:N0}%)", avgOutcomesValue);
                        }
                        else if (value > avgOutcomesValue)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerGreen.gif";
                            e.Row.Cells[i].Title = string.Format("Выше среднего исполнения по субъекту РФ ({0:N0}%)", avgOutcomesValue);
                        }
                        e.Row.Cells[i].Style.Padding.Right = 10;
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; margin: 2px";
                    }
                }

                if (e.Row.Cells[0].Value != null &&
                     (e.Row.Cells[0].Value.ToString().ToLower().Contains("бюджет") || e.Row.Cells[0].Value.ToString().ToLower().Contains("область")))
                {
                    foreach (UltraGridCell cell in e.Row.Cells)
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }

            foreach (UltraGridCell cell in e.Row.Cells)
            {
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

        #region Среднее значение процента исполнения

        protected void AVGDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0007_HMAO_compare_avg");
            dtAVG = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Среднее", dtAVG);

            avgOutcomesValue = GetIntDTValue(dtAVG, "Среднее");
        }

        private static int GetIntDTValue(DataTable dt, string columnName)
        {
            if (dt != null && dt.Rows.Count > 0 &&
                dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != String.Empty)
            {
                return Convert.ToInt32(dt.Rows[0][columnName]);
            }
            return Int32.MinValue;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            
            ReportExcelExporter1.SheetColumnCount = 20;

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);

            sheet1.Rows[3 + UltraWebGrid.Rows.Count + 1].Cells[0].Value = MoPercentLabel.Text;
            
            sheet2.Rows[0].Cells[0].Value = Label1.Text;
            sheet2.Rows[1].Cells[0].Value = Label2.Text;
            DundasMap.NavigationPanel.Visible = false;
            DundasMap.ZoomPanel.Visible = false;
            ReportExcelExporter1.Export(DundasMap, sheet2, 3);
        }
        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            
            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);

            IText title = section1.AddText();
            Font font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(MoPercentLabel.Text);

            DundasMap.NavigationPanel.Visible = false;
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.7));
            DundasMap.Height = Unit.Pixel((int)(CustomReportConst.minScreenHeight * 0.7));
            ReportPDFExporter1.Export(DundasMap, Label2.Text, section2);
        }
        #endregion
    }
}
