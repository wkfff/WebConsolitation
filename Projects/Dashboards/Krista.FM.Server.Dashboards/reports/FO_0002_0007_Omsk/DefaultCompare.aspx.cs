using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using SerializationFormat=Dundas.Maps.WebControl.SerializationFormat;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0007_Omsk
{
    public partial class DefaultCompare : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2000;
        private int endYear = 2011;
        private string populationDate;

        // имя папки с картами региона
        private string mapFolderName;
        // пропорция карты
        private double mapSizeProportion;
        // масшбтаб карты
        private double mapZoomValue;
        // сдвиг по вертикали текста городов-выносок карты
        private double mapCalloutOffsetY;

        // тип расстановки рангов (MO/MR)
        private string rankingType;

        #region Параметры запроса

        // расходы Итого
        private CustomParam outcomesTotal;
        // группа ФКР
        private CustomParam fkrGroupName;
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

        // множество МО для ранжирования
        private CustomParam rankingSet;
        // условие для фильтрации рангов
        private CustomParam rankingCondition;

        #endregion

        public string RankingSetCaption
        {
            get { return rankingType == "MR" ? "МР" : "МО"; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            mapFolderName = RegionSettingsHelper.Instance.GetPropertyValue("MapFolderName");
            mapZoomValue = Convert.ToDouble(RegionSettingsHelper.Instance.GetPropertyValue("MapZoomValue"));

            double value = 1;
            mapSizeProportion = value;
            if (Double.TryParse(RegionSettingsHelper.Instance.GetPropertyValue("MapSizeProportion"), NumberStyles.Any,
                                NumberFormatInfo.CurrentInfo, out value))
            {
                mapSizeProportion = value;
            }

            value = 1;
            mapCalloutOffsetY = value;
            if (Double.TryParse(RegionSettingsHelper.Instance.GetPropertyValue("MapCalloutOffsetY"), NumberStyles.Any,
                                NumberFormatInfo.CurrentInfo, out value))
            {
                mapCalloutOffsetY = value;
            }

            rankingType = RegionSettingsHelper.Instance.GetPropertyValue("RankingType");

            #region Инициализация параметров запроса

            outcomesTotal = UserParams.CustomParam("outcomes_total");
            fkrGroupName = UserParams.CustomParam("fkr_group_name");
            regionsLevel = UserParams.CustomParam("regions_level");
            regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            populationMeasure = UserParams.CustomParam("population_measure");
            populationMeasureYear = UserParams.CustomParam("population_measure_year");
            consolidateDocumentSKIFType = UserParams.CustomParam("consolidate_document_skif_type");
            regionDocumentSKIFType = UserParams.CustomParam("region_document_skif_type");
            regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");

            rankingSet = UserParams.CustomParam("ranking_set");
            rankingCondition = UserParams.CustomParam("ranking_condition");

            #endregion

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight*mapSizeProportion - 225);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 12);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight);

            CrossLink1.Text = "Диаграмма&nbsp;распределения&nbsp;субъектов&nbsp;по&nbsp;коэффициенту&nbsp;бюджетной&nbsp;обеспеченности";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0007_Omsk/DefaultAllocation.aspx";
            CrossLink2.Text = "Расходы&nbsp;субъекта&nbsp;РФ&nbsp;подробнее";
            CrossLink2.NavigateUrl = "~/reports/FO_0002_0006/DefaultDetail.aspx";

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0007_Omsk_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);

                ComboFKR.Width = 420;
                ComboFKR.Title = "РзПр";
                ComboFKR.MultiSelect = false;
                ComboFKR.ParentSelect = true;
                ComboFKR.FillDictionaryValues(
                    CustomMultiComboDataHelper.FillFOFKRNames(DataDictionariesHelper.OutcomesFOFKRTypes,
                                                              DataDictionariesHelper.OutcomesFOFKRLevels, false));
                ComboFKR.SetСheckedState("Общегосударственные вопросы", true);
                ComboFKR.SetСheckedState("Расходы бюджета - ИТОГО", true);
                ComboFKR.SetСheckedState("Расходы бюджета Итого", true);
            }

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            Page.Title = string.Format("Исполнение расходов");
            Label1.Text = Page.Title;
            Label2.Text = string.Format("({3}) за {0}&nbsp;{1}&nbsp;{2}&nbsp;года", monthNum,
                                        CRHelper.RusManyMonthGenitive(monthNum), yearNum,
                                        ComboFKR.SelectedValue.TrimEnd(' '));

            int year = Convert.ToInt32(ComboYear.SelectedValue);

            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}",
                                                            CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}",
                                                          CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            outcomesTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;

            populationMeasure.Value = RegionSettingsHelper.Instance.PopulationMeasure;
            populationMeasureYear.Value = RegionSettingsHelper.Instance.PopulationMeasurePlanning
                                              ? (year + 1).ToString()
                                              : year.ToString();

            consolidateDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");

            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            if (ComboFKR.SelectedValue == "Расходы бюджета - ИТОГО")
            {
                fkrGroupName.Value = outcomesTotal.Value;
            }
            else
            {
                fkrGroupName.Value = DataDictionariesHelper.OutcomesFOFKRTypes[ComboFKR.SelectedValue];
            }

            switch (rankingType)
            {
                case "MO":
                    {
                        rankingSet.Value = "Города и районы";
                        rankingCondition.Value = "isNotRankingMO";
                        break;
                    }
                case "MR":
                    {
                        rankingSet.Value = "Районы";
                        rankingCondition.Value = "isNotRankingMR";
                        break;
                    }
            }
            
            populationDate = DataDictionariesHelper.GetOmskRegionPopulationDate(yearNum);

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
                DundasMap.Serializer.Load(
                    (Server.MapPath(string.Format("../../maps/Субъекты/{0}/{0}_выноски.bin", mapFolderName))));
                SetMapSettings();
            }
            else
            {
                //DundasMap.ShapeFields.Clear();
                DundasMap.ShapeFields.Add("Name");
                DundasMap.ShapeFields["Name"].Type = typeof (string);
                DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
                DundasMap.ShapeFields.Add("Complete");

                DundasMap.ShapeFields["Complete"].Type = typeof (double);
                DundasMap.ShapeFields["Complete"].UniqueIdentifier = false;
                DundasMap.ShapeFields.Add("CompletePercent");
                DundasMap.ShapeFields["CompletePercent"].Type = typeof (double);
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
            legend.Title = "Коэффициент бюджетной\nобеспеченности населения,\nруб./чел.";
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
            DundasMap.Legends[0].Items["LegendItem"].Cells["Cell1"].Text = ComboFKR.SelectedValue;
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

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0007_Omsk_compare_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Бюджет", dtGrid);

            const string populationColumnName = "Численность постоянного населения";
            const string executeColumnName = "Факт";
            const string avgExecuteColumnName = "Коэффициент бюджетной обеспеченности населения";
            const string avgExecuteRankColumnName = "Ранг бюдж.обесп. МО";
            const string avgExecuteWorseRankColumnName = "Худший ранг бюдж.обесп. МО";

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[1] != DBNull.Value)
                    {
                        row[1] = Convert.ToDouble(row[1]) / 1000;
                    }
                    if (row[2] != DBNull.Value)
                    {
                        row[2] = Convert.ToDouble(row[2]) / 1000;
                    }
                }

                query = DataProvider.GetQueryText("FO_0002_0007_Omsk_compare_population");
                DataTable populationDT = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Численность населения", populationDT);

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

        private bool IsRankingRow(string rowName)
        {
            return rowName != "Консолидированный бюджет субъекта" && rowName != "Местные бюджеты" && rowName != "г.Омск" &&
                rowName != "Городские округа" && rowName != "Муниципальные районы" &&
                rowName != RegionSettingsHelper.Instance.OwnSubjectBudgetName;
        }

        private void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
//            if (ComboFO.SelectedIndex != 0 || dtGrid.Rows.Count < 15)
//            {
//                UltraWebGrid.Height = Unit.Empty;
//            }
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

                bool rankHidden = ComboFKR.SelectedValue == "Расходы бюджета - ИТОГО";

                if (rankHidden)
                {
                    SetColumnParams(e.Layout, 0, 1, "N2", 130, false);
                    SetColumnParams(e.Layout, 0, 2, "N2", 130, false);
                    SetColumnParams(e.Layout, 0, 3, "P2", 130, false);
                    SetColumnParams(e.Layout, 0, 4, "N3", 130, false);
                    SetColumnParams(e.Layout, 0, 5, "N2", 130, false);
                    SetColumnParams(e.Layout, 0, 6, "N0", 100, false);
                    SetColumnParams(e.Layout, 0, 7, "N0", 100, true);
                    SetColumnParams(e.Layout, 0, 8, "P2", 100, false);
                    SetColumnParams(e.Layout, 0, 9, "N0", 100, true);
                    SetColumnParams(e.Layout, 0, 10, "N0", 100, true);
                }
                else
                {
                    SetColumnParams(e.Layout, 0, 1, "N2", 110, false);
                    SetColumnParams(e.Layout, 0, 2, "N2", 110, false);
                    SetColumnParams(e.Layout, 0, 3, "P2", 110, false);
                    SetColumnParams(e.Layout, 0, 4, "N3", 110, false);
                    SetColumnParams(e.Layout, 0, 5, "N2", 110, false);
                    SetColumnParams(e.Layout, 0, 6, "N0", 100, false);
                    SetColumnParams(e.Layout, 0, 7, "N0", 100, true);
                    SetColumnParams(e.Layout, 0, 8, "P2", 100, false);
                    SetColumnParams(e.Layout, 0, 9, "N0", 100, false);
                    SetColumnParams(e.Layout, 0, 10, "N0", 100, true);
                }

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Назначено, тыс.руб", "План на год");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Исполнено, тыс.руб", "Фактическое исполнение нарастающим итогом с начала года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Исполнено %", "Процент выполнения назначений. Оценка равномерности исполнения (1/12 годового плана в месяц)");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, String.Format("Численность постоянного населения ({0}), тыс.чел.", populationDate), "Численность постоянного населения");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Коэффициент бюджетной обеспеченности,\n руб./чел.", "Коэффициент бюджетной обеспеченности населения, рублей на душу населения");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6,
                    String.Format("Ранг бюдж. обеспеч. {0}", RankingSetCaption),
                    String.Format("Ранг (место) бюджета по коэффициенту бюджетной обеспеченности среди бюдежетов {0}", RankingSetCaption));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, "Доля", "Доля расхода в общей сумме расходов бюджета");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9,
                    String.Format("Ранг доля {0}", RankingSetCaption),
                    String.Format("Ранг (место) бюджета по доле расхода в общей сумме расходов среди бюджетов {0}", RankingSetCaption));
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Top = 5;
                e.Row.Cells[i].Style.Padding.Bottom = 5;

                bool rank = (i == 6 || i == 9);
                bool complete = (i == 3);

                if (rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        string obj = (i == 6) ? "коэффициент бюджетной обеспеченности" : "доля расхода";
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            string bestStr = (i == 6) ? "Самый высокий" : "Самая высокая";
                            e.Row.Cells[i].Title = string.Format("{2} {1} в {0}", RankingSetCaption, obj, bestStr);
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            string bestStr = (i == 6) ? "Самый низкий" : "Самая низкая";
                            e.Row.Cells[i].Title = string.Format("{2} {1} в {0}", RankingSetCaption, obj, bestStr);
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }

                if (complete)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double percent = (ComboMonth.SelectedIndex + 1) * 100.0 / 12;
                        
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < percent)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                            e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:N2}%)", percent);
                        }
                        else
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:N2}%)", percent);
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }

                if (e.Row.Cells[0].Value != null &&
                     (e.Row.Cells[0].Value.ToString().ToLower().Contains("бюджет") || e.Row.Cells[0].Value.ToString().ToLower().Contains("область") ||
                     (e.Row.Cells[0].Value.ToString().ToLower().Contains("городские округа") || e.Row.Cells[0].Value.ToString().ToLower().Contains("муниципальные районы"))))
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

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text.Replace("&nbsp;", " ") + " " + Label2.Text.Replace("&nbsp;", " ");
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.CurrentWorksheet.Columns[0].Width = 250 * 37;
            e.CurrentWorksheet.Columns[1].Width = 90 * 37;
            e.CurrentWorksheet.Columns[2].Width = 90 * 37;
            e.CurrentWorksheet.Columns[3].Width = 90 * 37;
            e.CurrentWorksheet.Columns[4].Width = 90 * 37;
            e.CurrentWorksheet.Columns[5].Width = 90 * 37;
            e.CurrentWorksheet.Columns[6].Width = 90 * 37;
            e.CurrentWorksheet.Columns[7].Width = 90 * 37;
            e.CurrentWorksheet.Columns[8].Width = 90 * 37;
            e.CurrentWorksheet.Columns[9].Width = 90 * 37;
            e.CurrentWorksheet.Columns[10].Width = 90 * 37;

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "#,##0.00";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "##";
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion

        #region Экспорт в Pdf

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
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text.Replace("&nbsp;", " "));
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
