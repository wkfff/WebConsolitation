using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Image=Infragistics.Documents.Reports.Graphics.Image;
using SerializationFormat=Dundas.Maps.WebControl.SerializationFormat;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0005_Omsk
{
    public partial class DefaultCompare : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2000;
        private int endYear = 2011;
        private int rubMultiplier;
        private string populationDate;

        // имя папки с картами региона
        private string mapFolderName;
        // пропорция карты
        private double mapSizeProportion;
        // масшбтаб карты
        private double mapZoomValue;

        #region Параметры запроса

        // доходы Итого
        private CustomParam incomesTotal;
        // группа КД
        private CustomParam kdGroupName;
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

        // множество районов
        private CustomParam regionSet;

        // вычислители рангов
        RankCalculator moRank = new RankCalculator(RankDirection.Asc);
        RankCalculator northZoneRank = new RankCalculator(RankDirection.Asc);
        RankCalculator northForestZoneRank = new RankCalculator(RankDirection.Asc);
        RankCalculator southForestZoneRank = new RankCalculator(RankDirection.Asc);
        RankCalculator southSteppeZoneRank = new RankCalculator(RankDirection.Asc);

        #endregion

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private bool UseClimaticZones
        {
            get { return useClimaticZones.Checked; }
        }

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
            
            #region Инициализация параметров запроса

            incomesTotal = UserParams.CustomParam("incomes_total");
            kdGroupName = UserParams.CustomParam("kd_group_name");
            regionsLevel = UserParams.CustomParam("regions_level");
            regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            populationMeasure = UserParams.CustomParam("population_measure");
            populationMeasureYear = UserParams.CustomParam("population_measure_year");
            consolidateDocumentSKIFType = UserParams.CustomParam("consolidate_document_skif_type");
            regionDocumentSKIFType = UserParams.CustomParam("region_document_skif_type");
            regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            regionSet = UserParams.CustomParam("region_set");

            #endregion

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * mapSizeProportion - 225);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            
            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight);

            CrossLink1.Text = "Диаграмма распределения МО по среднедушевым доходам";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0005_Omsk/DefaultAllocation.aspx";
            CrossLink2.Text = "Доходы&nbsp;субъекта&nbsp;РФ&nbsp;подробнее";
            CrossLink2.NavigateUrl = "~/reports/FO_0002_0003_Omsk/DefaultDetail.aspx";

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <EndExportEventArgs>(PdfExporter_EndExport);

            PopupInformer1.HelpPageUrl = "DefaultCompare_MRRanking.html";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;

            UserParams.IncomesKD30000000000000000.Value = RegionSettingsHelper.Instance.IncomesKD30000000000000000;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKD11406000000000430.Value = RegionSettingsHelper.Instance.IncomesKD11406000000000430;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11402000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11402000000000000;
            UserParams.IncomesKD11403000000000410.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000410;
            UserParams.IncomesKD11403000000000440.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000440 !=
                                                          "null"
                                                              ? string.Format(",{2}.[{1}].[ДОХОДЫ ОТ ПРОДАЖИ МАТЕРИАЛЬНЫХ И НЕМАТЕРИАЛЬНЫХ АКТИВОВ].[{0}],",
                                                                    RegionSettingsHelper.Instance.IncomesKD11403000000000440,
                                                                    RegionSettingsHelper.Instance.IncomesKDRootName,
                                                                    RegionSettingsHelper.Instance.IncomesKDAllLevel)
                                                              : ",";

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0005_Omsk_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = dtDate.Rows[0][3].ToString();
                UserParams.Filter.Value = "Все федеральные округа";
                UserParams.KDGroup.Value = "Доходы бюджета - Итого ";

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                ComboKD.Width = 300;
                ComboKD.Title = "Вид дохода";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(FillFullKDIncludingList());
                ComboKD.RenameTreeNodeByName("Доходы от предпринимательской деятельности ", String.Format("{0} ", UserParams.IncomesKD30000000000000000.Value));
                ComboKD.SetСheckedState(UserParams.KDGroup.Value, true);
            }

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            Page.Title = "Исполнение доходов";
            Label1.Text = Page.Title;
            Label2.Text = string.Format("({3}) за {0} {1} {2} года", monthNum,
                CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboKD.SelectedValue.TrimEnd(' '));

            int year = Convert.ToInt32(ComboYear.SelectedValue);

            populationDate = DataDictionariesHelper.GetOmskRegionPopulationDate(yearNum);

            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            kdGroupName.Value = ComboKD.SelectedValue;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;

            populationMeasure.Value = RegionSettingsHelper.Instance.PopulationMeasure;
            populationMeasureYear.Value = RegionSettingsHelper.Instance.PopulationMeasurePlanning
                                              ? (year + 1).ToString()
                                              : year.ToString();

            consolidateDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;
            regionSet.Value = UseClimaticZones ? "Климатические зоны" : "Список МО";

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
                DundasMap.ShapeFields["Name"].Type = typeof (string);
                DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
                DundasMap.ShapeFields.Add("CompletePercent");
                DundasMap.ShapeFields["CompletePercent"].Type = typeof (double);
                DundasMap.ShapeFields["CompletePercent"].UniqueIdentifier = false;

                SetMapSettings();
                AddMapLayer(DundasMap, mapFolderName, "Территор", CRHelper.MapShapeType.Areas);
                //AddMapLayer(DundasMap, mapFolderName, "Города", CRHelper.MapShapeType.Towns);
                //AddMapLayer(DundasMap, mapFolderName, "Выноски", CRHelper.MapShapeType.CalloutTowns);
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
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.Viewport.Zoom = (float)mapZoomValue;

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
            
            LegendCellColumn column = new LegendCellColumn("Код");
            column.HeaderFont = new Font("Verdana", 8, FontStyle.Bold);
            column.HeaderTextAlignment = StringAlignment.Far;
            legend.CellColumns.Add(column);

            column = new LegendCellColumn("Территория");
            column.HeaderFont = new Font("Verdana", 8, FontStyle.Bold);
            column.HeaderTextAlignment = StringAlignment.Center;
            legend.CellColumns.Add(column);

            column = new LegendCellColumn("Процент\nисполнения");
            column.HeaderFont = new Font("Verdana", 8, FontStyle.Bold);
            column.HeaderTextAlignment = StringAlignment.Far;
            legend.CellColumns.Add(column);

            DundasMap.Legends.Add(legend);

            Legend completelegend = new Legend("CompleteLegend");
            completelegend.Visible = true;
            completelegend.BackColor = Color.White;
            completelegend.BackSecondaryColor = Color.Gainsboro;
            completelegend.BackGradientType = GradientType.DiagonalLeft;
            completelegend.BackHatchStyle = MapHatchStyle.None;
            completelegend.BorderColor = Color.Gray;
            completelegend.BorderWidth = 1;
            completelegend.BorderStyle = MapDashStyle.Solid;
            completelegend.BackShadowOffset = 4;
            completelegend.TextColor = Color.Black;
            completelegend.Font = new System.Drawing.Font("MS Sans Serif", 7, FontStyle.Regular);
            completelegend.AutoFitText = true;
            completelegend.Title = "Процент исполнения";
            completelegend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(completelegend);

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "CompletePercent";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 7;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Green;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = "#FROMVALUE{N0}% - #TOVALUE{N0}%";
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

            map.LoadFromShapeFile(layerName, "CODE", true);
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

//            foreach (Shape shape in DundasMap.Shapes)
//            {
//                string shapeName = GetShapeName(shape);
//                if (!RegionsNamingHelper.LocalBudgetUniqueNames.ContainsKey(shapeName))
//                {
//                    shape.Visible = false;
//                }
//            }

            int regionCodeColumnIndex = dtGrid.Columns.Count - 1;
            


            foreach (DataRow row in dtGrid.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[regionCodeColumnIndex] != DBNull.Value && row[regionCodeColumnIndex].ToString() != string.Empty &&
                    row[3] != DBNull.Value && row[3].ToString() != string.Empty)
                {
                    string subject = row[regionCodeColumnIndex].ToString();
                    double value = Convert.ToDouble(row[3]);
                    double climaticZoneValue = 0;

                    string climaticZoneName = String.Empty;
                    string zoneRegionsList = String.Empty;

                    if (UseClimaticZones && 
                        row[14] != DBNull.Value && row[14].ToString() != string.Empty &&
                        row[15] != DBNull.Value && row[15].ToString() != string.Empty &&
                        row[16] != DBNull.Value)
                    {
                        climaticZoneValue = Convert.ToDouble(row[14]);
                        climaticZoneName = row[15].ToString();
                        zoneRegionsList = row[16].ToString().Replace("br", "," + Environment.NewLine);
                    }

                    LegendItem item = new LegendItem();
                    LegendCell cell = new LegendCell(subject);
                    cell.Alignment = ContentAlignment.MiddleRight;
                    item.Cells.Add(cell);
                    cell = new LegendCell(row[0].ToString());
                    cell.Alignment = ContentAlignment.MiddleLeft;
                    item.Cells.Add(cell);
                    DundasMap.Legends["SubjectLegend"].Items.Add(item);

                    ArrayList shapeList = FindMapShape(DundasMap, subject);
                    foreach (Shape shape in shapeList)
                    {
                        string shapeName = GetShapeName(shape);
                        shape["Name"] = subject;
                        shape["CompletePercent"] = UseClimaticZones ? climaticZoneValue : value;
                        shape.ToolTip = string.Format("{0}\n#COMPLETEPERCENT{{N2}}%", 
                            UseClimaticZones
                                ? String.Format("{0}{1}({2})", climaticZoneName, Environment.NewLine, zoneRegionsList) 
                                : subject);

                        cell = new LegendCell(String.Format("{0:N2}%", value));
                        cell.Alignment = ContentAlignment.MiddleRight;
                        item.Cells.Add(cell);

                        shape.Text = string.Format("{0}\n{1:N2}%", shapeName.Replace(" ", "\n"), value);
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
            return rowName != "Консолидированный бюджет субъекта" && rowName != "Местные бюджеты" && rowName != "г.Омск" &&
                rowName != "Городские округа" && rowName != "Муниципальные районы, всего" &&
                rowName != RegionSettingsHelper.Instance.OwnSubjectBudgetName;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0005_Omsk_compare_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Бюджеты", dtGrid);

            const string populationColumnName = "Численность постоянного населения";
            const string executeColumnName = "Факт";
            const string avgExecuteColumnName = "Среднедушевые доходы";
            string avgExecuteRankColumnName = "Ранг среднедуш МР";
            string avgExecuteWorseRankColumnName = "Худший ранг среднедуш МР";

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[1] != DBNull.Value)
                    {
                        row[1] = Convert.ToDouble(row[1]) / rubMultiplier;
                    }
                    if (row[2] != DBNull.Value)
                    {
                        row[2] = Convert.ToDouble(row[2]) / rubMultiplier;
                    }
                    if (row[3] != DBNull.Value)
                    {
                        row[3] = Convert.ToDouble(row[3]) * 100;
                    }
                }

                query = DataProvider.GetQueryText("FO_0002_0005_omsk_compare_population");
                DataTable populationDT = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Численность населения", populationDT);

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

                                    RankCalculator avgExecuteRank = GetRankCalculator(dtRow);

                                    if (population != 0 && dtRow[executeColumnName] != DBNull.Value && dtRow[executeColumnName].ToString() != String.Empty)
                                    {
                                        double execute = Convert.ToDouble(dtRow[executeColumnName]);
                                        double avgExecute = 1000 * execute / population;

                                        dtRow[avgExecuteColumnName] = avgExecute;

                                        if (IsRankingRow(rowName))
                                        {
                                            avgExecuteRank.AddItem(avgExecute);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    

                    // пробегаем еще раз, чтобы расставить ранги
                    foreach (DataRow row in dtGrid.Rows)
                    {
                        string rowName = row[0].ToString();
                        if (IsRankingRow(rowName) && row[avgExecuteColumnName] != DBNull.Value && row[avgExecuteColumnName].ToString() != String.Empty)
                        {
                            double value = Convert.ToDouble(row[avgExecuteColumnName]);

                            RankCalculator avgExecuteRank = GetRankCalculator(row);
                            double avgExecuteWorseRank = avgExecuteRank.GetWorseRank();
                            int rank = avgExecuteRank.GetRank(value);
                            if (rank != 0)
                            {
                                row[avgExecuteRankColumnName] = rank;
                                row[avgExecuteWorseRankColumnName] = avgExecuteWorseRank;
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
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        private RankCalculator GetRankCalculator(DataRow row)
        {
            if (!UseClimaticZones)
            {
                return moRank;
            }

            string regionType = String.Empty;
            if (row["ПКЗ"] != DBNull.Value)
            {
                regionType = row["ПКЗ"].ToString();
            }

            switch (regionType)
            {
                case "Северная зона":
                    {
                        return northZoneRank;
                    }
                case "Северная лесостепная зона":
                    {
                        return northForestZoneRank;
                    }
                case "Южная лесостепная зона":
                    {
                        return southForestZoneRank;
                    }
                case "Степная зона":
                    {
                        return southSteppeZoneRank;
                    }
                default:
                    {
                        return moRank;
                    }
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

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(240);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            SetColumnParams(e.Layout, 0, 1, "N1", 100, false);
            SetColumnParams(e.Layout, 0, 2, "N1", 100, false);
            SetColumnParams(e.Layout, 0, 3, "N2", 80, false);
            SetColumnParams(e.Layout, 0, 4, "N0", 62, false);
            SetColumnParams(e.Layout, 0, 5, "N0", 62, true);
            SetColumnParams(e.Layout, 0, 6, "N0", 62, false);
            SetColumnParams(e.Layout, 0, 7, "N0", 62, true);
            SetColumnParams(e.Layout, 0, 8, "N1", 90, false);
            SetColumnParams(e.Layout, 0, 9, "N1", 100, false);
            SetColumnParams(e.Layout, 0, 10, "N0", 90, false);
            SetColumnParams(e.Layout, 0, 11, "N0", 90, true);
            SetColumnParams(e.Layout, 0, 12, "N2", 90, false);
            SetColumnParams(e.Layout, 0, 13, "N2", 90, false);

            SetColumnParams(e.Layout, 0, 14, "", 90, true);
            SetColumnParams(e.Layout, 0, 15, "", 90, true);
            SetColumnParams(e.Layout, 0, 16, "", 90, true);
            SetColumnParams(e.Layout, 0, 17, "", 90, true);

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, 
                String.Format("Назначено, {0}", RubMiltiplierButtonList.SelectedValue), "План на год");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2,
                String.Format("Исполнено, {0}", RubMiltiplierButtonList.SelectedValue), "Фактическое исполнение нарастающим итогом с начала года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Исполнено %", "Процент выполнения назначений. Оценка равномерности исполнения (1/12 годового плана в месяц)");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4,
                "Ранг %",
                "Ранг (место) бюджета по проценту исполнения назначений среди муниципальных районов/климатических зон");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6,
                "Ранг по объему исп-ния",
                "Ранг (место) муниципального района/климатической зоны по объему фактического исполнения");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, String.Format("Численность постоянного населения ({0}), тыс.чел.", populationDate), "Численность постоянного населения");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9, "Среднедушевые доходы, руб./чел.", "Сумма доходов выбранного вида на душу населения");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 10, 
                "Ранг по среднедуш. доходу",
                "Ранг (место) бюджета по среднедушевым доходам среди муниципальных районов/климатических зон");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 12, "Доля МР, %", "Доля доходов бюджета в общей сумме доходов бюджетов МР, %");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 13, "Доля КБ, %", "Доля доходов бюджета в общей сумме доходов консолидированного бюджета");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Top = 5;
                e.Row.Cells[i].Style.Padding.Bottom = 5;

                bool rank = (i == 6 || i == 4 || i == 10);
                bool complete = (i == 3);
                bool isClimaticZone = e.Row.Cells[0].ToString().Contains(" зона");

                if (!isClimaticZone && rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        string obj = (i == 10) ? "доход на душу населения" : "процент исполнения";

                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = (i == 6)
                                ? "Район с самым высоким значением фактического исполнения"
                                : String.Format("Самый высокий {0} среди МР", obj);
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = (i == 6)
                                ? "Район с самым меньшим значением фактического исполнения"
                                : String.Format("Самый низкий {0} среди МР", obj);
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (complete)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double percent = (ComboMonth.SelectedIndex + 1) * 100.0 / 12;
                        
                        if (Convert.ToDouble(e.Row.Cells[i].Value) < percent)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                            e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:N2}%)", percent);
                        }
                        else
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:N2}%)", percent);
                        }
                        e.Row.Cells[i].Style.Padding.Right = 2;
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }

                if (e.Row.Cells[0].Value != null &&
                     (e.Row.Cells[0].Value.ToString().ToLower().Contains("бюджет") || e.Row.Cells[0].Value.ToString().ToLower().Contains("область") ||
                     e.Row.Cells[0].Value.ToString().ToLower().Contains(", всего") || e.Row.Cells[0].Value.ToString().ToLower().Contains(" зона")))
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

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);
        }

        private void PdfExporter_EndExport(object sender, EndExportEventArgs e)
        {
            Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);
        }
        
        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
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
            e.CurrentWorksheet.Columns[11].Width = 90 * 37;
            e.CurrentWorksheet.Columns[12].Width = 90 * 37;

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "##";
            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[10].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion

        /// <summary>
        /// Заполнить полный список доходами КД (с элементами "в том числе")
        /// </summary>
        public static Dictionary<string, int> FillFullKDIncludingList()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("Налоговые доходы ", 0);
            valuesDictionary.Add("Налог на прибыль ", 1);
            valuesDictionary.Add("НДФЛ ", 1);
            valuesDictionary.Add("Налоги на имущество ", 1);
            valuesDictionary.Add("Налог на имущество физ.лиц ", 2);
            valuesDictionary.Add("Налог на имущество организаций ", 2);
            valuesDictionary.Add("Транспортный налог ", 2);
            valuesDictionary.Add("Транспортный налог с организаций ", 3);
            valuesDictionary.Add("Транспортный налог с физ.лиц ", 3);
            valuesDictionary.Add("Земельный налог ", 2);
            valuesDictionary.Add("Акцизы ", 1);
            valuesDictionary.Add("Акцизы на нефтепродукты ", 2);
            valuesDictionary.Add("Акцизы на алкоголь ", 2);
            valuesDictionary.Add("НДПИ ", 1);
            valuesDictionary.Add("Налоги на совокупный доход ", 1);
            valuesDictionary.Add("УСН ", 2);
            valuesDictionary.Add("ЕНВД ", 2);
            valuesDictionary.Add("ЕСХН ", 2);
            valuesDictionary.Add("Гос.пошлина ", 1);
            valuesDictionary.Add("Задолженность по отмененным налогам ", 1);
            valuesDictionary.Add("Неналоговые доходы ", 0);
            valuesDictionary.Add("Доходы от использования имущества ", 1);
            valuesDictionary.Add("Дивиденды по акциям ", 2);
            valuesDictionary.Add("Арендная плата за земли ", 2);
            valuesDictionary.Add("Доходы от сдачи в аренду имущества ", 2);
            valuesDictionary.Add("Платежи от ГУПов и МУПов ", 2);
            valuesDictionary.Add("Платежи при пользовании природными ресурсами ", 1);
            valuesDictionary.Add("Плата за негативное воздействие на окруж.среду ", 2);
            valuesDictionary.Add("Платежи за пользование лесным фондом ", 2);
            valuesDictionary.Add("Платежи при пользовании недрами ", 2);
            valuesDictionary.Add("Доходы от оказания платных услуг ", 1);
            valuesDictionary.Add("Доходы от продажи активов ", 1);
            valuesDictionary.Add("Доходы от продажи активов (кроме зем.участков) ", 2);
            valuesDictionary.Add("Доходы от продажи зем. участков ", 2);
            valuesDictionary.Add("Административные платежи и сборы ", 1);
            valuesDictionary.Add("Штрафы ", 1);
            valuesDictionary.Add("Прочие ", 1);
            valuesDictionary.Add("Прочие неналоговые доходы ", 2);
            valuesDictionary.Add("Невыясненные поступления ", 2);
            valuesDictionary.Add("Доходы бюджетов от возврата остатков МБТ прошлых лет ", 1);
            valuesDictionary.Add("Возврат остатков МБТ прошлых лет ", 1);
            valuesDictionary.Add("Доходы от приносящей доход деятельности ", 0);
            valuesDictionary.Add("Налоговые и неналоговые доходы ", 0);
            valuesDictionary.Add("Безвозмездные поступления ", 0);
            valuesDictionary.Add("Доходы бюджета - Итого ", 0);
            return valuesDictionary;
        }
    }
}
