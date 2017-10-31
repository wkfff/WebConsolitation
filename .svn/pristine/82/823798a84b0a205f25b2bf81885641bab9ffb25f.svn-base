using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls;

using Dundas.Maps.WebControl;

using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.Export;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using InitializeRowEventHandler = Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;
using SerializationFormat = Dundas.Maps.WebControl.SerializationFormat;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dt_incomes = new DataTable();
        private DataTable dt_outcomesTotal = new DataTable();
        private DataTable dt_fkrOutcomes = new DataTable();
        private DataTable dt_ekrOutcomes = new DataTable();
        private DataTable dt_debts = new DataTable();
        private DataTable dt_deficit = new DataTable();
        private DataTable dtDate = new DataTable();

        private GridHeaderLayout headerLayoutGridIncomes;
        private GridHeaderLayout headerLayoutGridOutcomes;
        private GridHeaderLayout headerLayoutGridDeficit;
        private GridHeaderLayout headerLayoutGridEkrOutcomes;
        private GridHeaderLayout headerLayoutGridFkrOutcomes;
        private GridHeaderLayout headerLayoutGridDebs;

        private int firstYear = 2005;
        private int endYear = 2011;
        private string month = "Январь";

        private double incomesLimit = 0;
        private double FKRLimit = 0;

        private string mapFolderName;
        private int mapWidth;
        private double mapSizeProportion;
        private bool waterLayerBottom = false;

        private bool regionSelected = false;

        // заголовки гридов
        private const string EKRGridCaption = "Расходы по КОСГУ";
        private const string FKRGridCaption = "Расходы по РзПр";
        private const string outcomesTotalGridCaption = "Расходы";
        private const string incomesGridCaption = "Доходы";
        private const string balanceGridCaption = "Дефицит/Профицит";
        private const string debtsGridCaption = "Задолженность";

        private bool UseConsolidateRegionBudget
        {
            get { return useConsolidateRegionBudget.Checked; }
        }

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        #endregion

        #region Параметры запроса

        // Доходы всего
        private CustomParam incomeAll;
        // Расходы всего
        private CustomParam outcomeAll;
        // Тип документа
        private CustomParam documentType;
        // Консолидированный уровень
        private CustomParam consolidateLevel;
        // выбранный множитель рублей
        private CustomParam rubMultiplier;
        // фильтр по КОСГУ
        private CustomParam kosguFilter;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (incomeAll == null)
            {
                incomeAll = UserParams.CustomParam("income_all");
            }
            if (outcomeAll == null)
            {
                outcomeAll = UserParams.CustomParam("outcome_all");
            }
            if (documentType == null)
            {
                documentType = UserParams.CustomParam("document_type");
            }
            if (consolidateLevel == null)
            {
                consolidateLevel = UserParams.CustomParam("consolidate_level");
            }
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            kosguFilter = UserParams.CustomParam("kosgu_filter");

            #endregion

            mapFolderName = RegionSettingsHelper.Instance.GetPropertyValue("MapFolderName");
            mapWidth = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("MapWidth"));
            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));
            waterLayerBottom = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("WaterLayerBottom"));

            string value = RegionSettingsHelper.Instance.GetPropertyValue("MapSizeProportion");
            value = value.Replace(",", NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator);
            value = value.Replace(".", NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator);
            mapSizeProportion = Convert.ToDouble(value);

            DundasMap.Width = CRHelper.GetChartWidth(mapWidth);
            DundasMap.Height = (int)(DundasMap.Width.Value * mapSizeProportion);

            documentType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            incomeAll.Value = RegionSettingsHelper.Instance.IncomeTotal;
            outcomeAll.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;

            #region Настройка гридов

            int dirtyWidth = (CustomReportConst.minScreenWidth - 30);
            grid_incomes.Width = CRHelper.GetGridWidth(dirtyWidth * 0.48);
            grid_deficit.Width = CRHelper.GetGridWidth(dirtyWidth * 0.48);
            grid_outcomesTotal.Width = CRHelper.GetGridWidth(dirtyWidth * 0.48);

            grid_ekrOutcomes.Width = CRHelper.GetGridWidth(dirtyWidth * 0.54);
            grid_fkrOutcomes.Width = CRHelper.GetGridWidth(dirtyWidth * 0.54);
            grid_debts.Width = CRHelper.GetGridWidth(dirtyWidth * 0.54);

            grid_ekrOutcomes.Caption = EKRGridCaption;
            grid_fkrOutcomes.Caption = FKRGridCaption;
            grid_outcomesTotal.Caption = outcomesTotalGridCaption;
            grid_incomes.Caption = incomesGridCaption;
            grid_deficit.Caption = balanceGridCaption;
            grid_debts.Caption = debtsGridCaption;

            grid_ekrOutcomes.InitializeRow += new InitializeRowEventHandler(grid_InitializeRow);
            grid_fkrOutcomes.InitializeRow += new InitializeRowEventHandler(grid_InitializeRow);
            grid_outcomesTotal.InitializeRow += new InitializeRowEventHandler(grid_InitializeRow);
            grid_incomes.InitializeRow += new InitializeRowEventHandler(grid_InitializeRow);
            grid_deficit.InitializeRow += new InitializeRowEventHandler(grid_InitializeRow);
            grid_debts.InitializeRow += new InitializeRowEventHandler(grid_InitializeRow);
            grid_outcomesTotal.Visible = true;

            #endregion

            #region Настройка карты

            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = false;
            DundasMap.NavigationPanel.Visible = false;
            DundasMap.SelectionBorderColor = System.Drawing.Color.Transparent;
            DundasMap.SelectionMarkerColor = System.Drawing.Color.Transparent;
            DundasMap.RenderType = RenderType.ImageTag;

            #endregion


            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
			}

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.FKRAllLevel.Value = RegionSettingsHelper.Instance.FKRAllLevel;
            UserParams.FKRSectionLevel.Value = RegionSettingsHelper.Instance.FKRSectionLevel;
            UserParams.FKRSubSectionLevel.Value = RegionSettingsHelper.Instance.FKRSubSectionLevel;
            UserParams.EKRDimension.Value = RegionSettingsHelper.Instance.EKRDimension;
            UserParams.IncomesKDGroupLevel.Value = RegionSettingsHelper.Instance.IncomesKDGroupLevel;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.KDInternalCircualtionExtruding.Value =
                RegionSettingsHelper.Instance.KDInternalCircualtionExtruding;
            UserParams.RzPrInternalCircualtionExtruding.Value =
                RegionSettingsHelper.Instance.RzPrInternalCircualtionExtruding;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string queryName =
                    string.Format("FO_0002_0001_date_{0}",
                                  RegionSettingsHelper.Instance.GetPropertyValue("LastDateQueryName"));
                string query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                if (dtDate.Rows.Count != 0)
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                    month = dtDate.Rows[0][3].ToString();
                }

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

                ComboBudget.Title = "Бюджет";
                ComboBudget.Width = 400;
                ComboBudget.MultiSelect = false;
                ComboBudget.FillDictionaryValues(
                    CustomMultiComboDataHelper.FillLocalBudgets(RegionsNamingHelper.LocalBudgetTypes));

                if (!String.IsNullOrEmpty(UserParams.Filter.Value))
                {
                    ComboBudget.SetСheckedState(UserParams.Filter.Value, true);
                }
                else
                {
                    ComboBudget.SetСheckedState("Консолидированный бюджет субъекта", true);
                }

                DundasMap.Shapes.Clear();
            }

            if (UserParams.SelectItem.Value != " ")
            {
                ComboBudget.SetСheckedState(UserParams.SelectedMap.Value, true);
            }
            UserParams.SelectItem.Value = " ";

            UserParams.PeriodMonth.Value = string.Format("[{0}].[Полугодие {1}].[Квартал {2}].[{3}]",
                                                         ComboYear.SelectedValue,
                                                         CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1),
                                                         CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1),
                                                         ComboMonth.SelectedValue);

            UserParams.SelectedMap.Value = string.Empty;
            UserParams.Region.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            consolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;

            kosguFilter.Value = RegionSettingsHelper.Instance.GetPropertyValue("KosguFilter");

            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";

            regionSelected = false;
            bool selectMap = false;
            switch (ComboBudget.SelectedValue)
            {
                case "Консолидированный бюджет субъекта":
                    {
                        UserParams.BudgetLevelEnum.Value = "[Уровни бюджета].[СКИФ].[Конс.бюджет субъекта]";
                        break;
                    }
                case "Местные бюджеты":
                    {
                        UserParams.BudgetLevelEnum.Value =
                            RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetLevel");
                        documentType.Value =
                            RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetDocumentSKIFType");
                        UserParams.Region.Value = RegionSettingsHelper.Instance.RegionsLocalBudgetLevel;
                        break;
                    }
                default:
                    {
                        if (ComboBudget.SelectedValue == RegionSettingsHelper.Instance.OwnSubjectBudgetName)
                        {
                            UserParams.BudgetLevelEnum.Value = "[Уровни бюджета].[СКИФ].[Бюджет субъекта]";
                        }
                        else
                        {
                            UserParams.Region.Value =
                                RegionsNamingHelper.LocalBudgetUniqueNames[ComboBudget.SelectedValue];
                            string selectedRegionType = RegionsNamingHelper.LocalBudgetTypes[ComboBudget.SelectedValue];
                            if (selectedRegionType.Contains("МР"))
                            {
                                regionSelected = true;
                                UserParams.BudgetLevelEnum.Value = UseConsolidateRegionBudget
                                                                       ? "[Уровни бюджета].[СКИФ].[Бюджет района и поселений]"
                                                                       : RegionSettingsHelper.Instance.GetPropertyValue(
                                                                           "RegionBudgetLevel");
                                documentType.Value =
                                    RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");
                            }
                            else if (selectedRegionType.Contains("ГО"))
                            {
                                UserParams.BudgetLevelEnum.Value =
                                    RegionSettingsHelper.Instance.GetPropertyValue("DistrictBudgetLevel");
                                documentType.Value =
                                    RegionSettingsHelper.Instance.GetPropertyValue("DistrictDocumentSKIFType");
                            }

                            // был выбран МР или ГО
                            UserParams.SelectedMap.Value = ComboBudget.SelectedValue;
                            selectMap = true;
                        }
                        break;
                    }
            }

            useConsolidateRegionBudget.Visible = regionSelected;

            Page.Title = "Исполнение бюджета";
            PageTitle.Text = Page.Title;

            RefreshMap();

            if (!DundasMap.IsCallback)
            {
                RefreshGrids();
            }

            if (selectMap)
            {
                CRHelper.SelectMapShape(DundasMap, UserParams.SelectedMap.Value, false, UserParams.SelectedMap.Value);
            }
        }

        #region Обработчики карты

        /// <summary>
        /// Обновление карты
        /// </summary>
        private void RefreshMap()
        {
            DundasMap.Shapes.Clear();
            DundasMap.Layers.Clear();
            try
            {
                if (Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("MapLoadFromBin")))
                {
                    DundasMap.Serializer.Format = SerializationFormat.Binary;
                    DundasMap.Serializer.Load(
                        Server.MapPath(string.Format("../../maps/Субъекты/{0}/{0}_СТВГ.bin", mapFolderName)));
                }
                else
                {
                    if (waterLayerBottom)
                    {
                        CRHelper.AddMapLayer(DundasMap,
                                             Server.MapPath(String.Format("../../maps/Субъекты/{0}/Водные.shp",
                                                                          mapFolderName)),
                                             false, CRHelper.MapShapeType.WaterObjects);
                    }

                    CRHelper.AddMapLayer(DundasMap,
                                         Server.MapPath(String.Format("../../maps/Субъекты/{0}/Соседние.shp",
                                                                      mapFolderName)),
                                         true, CRHelper.MapShapeType.SublingAreas);

                    CRHelper.AddMapLayer(DundasMap,
                                         Server.MapPath(String.Format("../../maps/Субъекты/{0}/Территор.shp",
                                                                      mapFolderName)),
                                         true, CRHelper.MapShapeType.Areas);

                    CRHelper.AddMapLayer(DundasMap,
                                         Server.MapPath(String.Format("../../maps/Субъекты/{0}/Города.shp",
                                                                      mapFolderName)),
                                         true, CRHelper.MapShapeType.Towns);

                    if (!waterLayerBottom)
                    {
                        CRHelper.AddMapLayer(DundasMap,
                                             Server.MapPath(String.Format("../../maps/Субъекты/{0}/Водные.shp",
                                                                          mapFolderName)),
                                             false, CRHelper.MapShapeType.WaterObjects);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Невозможно загрузить карту региона", ex);
            }

            // скрываем города, которых нет в словаре локальных бюджетов
            foreach (Shape shape in DundasMap.Shapes)
            {
                shape.ToolTip = shape.Name.Replace("\"", "'");
                if ((shape.Layer == "Города" || shape.Layer == "Территор") &&
                    !RegionsNamingHelper.LocalBudgetUniqueNames.ContainsKey(shape.Name))
                {
                    shape.Visible = false;
                }
            }
        }

        protected void map_Click(object sender, Dundas.Maps.WebControl.ClickEventArgs e)
        {
            HitTestResult result = e.MapControl.HitTest(e.X, e.Y);

            if (result == null || result.Object == null)
                return;

            if (result.Object is Shape)
            {
                Shape shape = (Shape)result.Object;

                // обрабатываем только города и районы, содержащиеся в словаре локальных бюджетов
                if (shape.Layer != "Территор" && shape.Layer != "Города" ||
                    !RegionsNamingHelper.LocalBudgetUniqueNames.ContainsKey(shape.Name))
                {
                    return;
                }

                UserParams.SelectItem.Value = shape.Name;

                // снимаем выделение со всех остальных слоев карты
                foreach (Shape sh in DundasMap.Shapes)
                {
                    if (sh.Selected)
                    {
                        CRHelper.SelectMapShape(sh, false);
                    }
                }

                // выделяем выбранный слой
                CRHelper.SelectMapShape(shape, true);

                shape.Text = shape.Name;
                UserParams.SelectedMap.Value = shape.Name;
                UserParams.Region.Value = RegionsNamingHelper.LocalBudgetUniqueNames[shape.Name];
            }
        }

        #endregion

        #region Обработчики грида

        /// <summary>
        /// Обновление гридов
        /// </summary>
        private void RefreshGrids()
        {
            string selectedBudget = UseConsolidateRegionBudget && regionSelected
                                        ? String.Format("{0} (Консолидированный бюджет МО)", ComboBudget.SelectedValue)
                                        : String.Format("{0}", ComboBudget.SelectedValue);

            int monthNum = ComboMonth.SelectedIndex + 1;
            PageSubTitle.Text = String.Format("{3} за {0} {1} {2} года", monthNum,
                                              CRHelper.RusManyMonthGenitive(monthNum), ComboYear.SelectedValue,
                                              selectedBudget);

            headerLayoutGridIncomes = new GridHeaderLayout(grid_incomes);
            headerLayoutGridOutcomes = new GridHeaderLayout(grid_outcomesTotal);
            headerLayoutGridDeficit = new GridHeaderLayout(grid_deficit);
            headerLayoutGridEkrOutcomes = new GridHeaderLayout(grid_ekrOutcomes);
            headerLayoutGridFkrOutcomes = new GridHeaderLayout(grid_fkrOutcomes);
            headerLayoutGridDebs = new GridHeaderLayout(grid_debts);
            // dataBind у grid2 нужно вызывать до grid1
            grid_fkrOutcomes.DataBind();
            grid_ekrOutcomes.DataBind();
            grid_incomes.DataBind();
            grid_deficit.DataBind();
            grid_debts.DataBind();
            grid_outcomesTotal.DataBind();

            grid_incomes.Height = Unit.Empty;
            grid_outcomesTotal.Height = Unit.Empty;
            grid_deficit.Height = Unit.Empty;
            grid_fkrOutcomes.Height = Unit.Empty;
            grid_ekrOutcomes.Height = Unit.Empty;
            grid_debts.Height = Unit.Empty;
        }

        protected void grid1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0001_ekrOutcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, EKRGridCaption, dt_ekrOutcomes);

            grid_ekrOutcomes.DataSource = dt_ekrOutcomes;
        }

        protected void grid2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0001_fkrOutcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, FKRGridCaption, dt_fkrOutcomes);


            if (dt_fkrOutcomes != null && dt_fkrOutcomes.Rows.Count != 0)
            {
                DataRow row = dt_fkrOutcomes.Rows[0];

                if (row[0] != DBNull.Value && row[0].ToString() == "Итого_")
                {
                    if (row[4] != DBNull.Value)
                    {
                        FKRLimit = Convert.ToDouble(row[4]);
                    }
                    dt_fkrOutcomes.Rows.Remove(row);
                }

                foreach (DataRow r in dt_fkrOutcomes.Rows)
                {
                    if (r[1] != DBNull.Value && r[1].ToString().Length > 4)
                    {
                        r[1] = r[1].ToString().Substring(0, 4);
                    }
                }
            }

            grid_fkrOutcomes.DataSource = dt_fkrOutcomes;
        }

        protected void grid3_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0001_Incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, incomesGridCaption, dt_incomes);

            if (dt_incomes != null && dt_incomes.Rows.Count != 0)
            {
                DataRow row = dt_incomes.Rows[0];
                if (row[0] != DBNull.Value && row[0].ToString() == "Итого_")
                {
                    if (row[3] != DBNull.Value)
                    {
                        incomesLimit = Convert.ToDouble(row[3]);
                    }
                    dt_incomes.Rows.Remove(row);
                }
            }

            grid_incomes.DataSource = dt_incomes;
        }

        protected void grid4_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0001_Deficit");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, balanceGridCaption, dt_deficit);

            grid_deficit.DataSource = dt_deficit;
        }

        protected void grid5_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0001_Debts");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, debtsGridCaption, dt_debts);

            grid_debts.DataSource = dt_debts;
        }

        protected void grid6_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0001_fkrOutcomes_total");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, outcomesTotalGridCaption,
                                                                             dt_outcomesTotal);

            grid_outcomesTotal.DataSource = dt_outcomesTotal;
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.FilterOptionsDefault.AllowRowFiltering = RowFiltering.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.Wrap = true;

            if (e.Layout.Grid.Columns.Count == 0)
            {
                return;
            }

            e.Layout.Grid.Columns[0].Width = CRHelper.GetColumnWidth(275);
            e.Layout.Grid.Columns[0].CellStyle.Wrap = true;

            string gridCaption = e.Layout.Grid.Caption;
            int i = 0;

            if (gridCaption == FKRGridCaption || gridCaption == EKRGridCaption)
            {
                int j = 1;

                e.Layout.Grid.Columns[j].Width = CRHelper.GetColumnWidth(40);
                e.Layout.Grid.Columns[j].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Grid.Columns[j].CellStyle.Padding.Right = 5;
                CRHelper.FormatNumberColumn(e.Layout.Grid.Columns[j],
                                            gridCaption == EKRGridCaption ? "000" : "00 00");

                i++;
            }

            e.Layout.Grid.Columns[i + 1].Width = CRHelper.GetColumnWidth(105);
            e.Layout.Grid.Columns[i + 2].Width = CRHelper.GetColumnWidth(98);
            e.Layout.Grid.Columns[i + 3].Width = CRHelper.GetColumnWidth(73);
            

            if (gridCaption == debtsGridCaption || gridCaption == balanceGridCaption)
            {
                e.Layout.Grid.Columns[i + 3].Hidden = true;
                if (gridCaption == debtsGridCaption)
                {
                    e.Layout.Grid.Columns[i + 2].Hidden = true;
                    e.Layout.Grid.Columns[i + 1].Width = CRHelper.GetColumnWidth(278);
                }
                else
                {
                    e.Layout.Grid.Columns[i + 1].Width = CRHelper.GetColumnWidth(105);
                    e.Layout.Grid.Columns[i + 2].Width = CRHelper.GetColumnWidth(172);
                }
            }          

            if (gridCaption != incomesGridCaption && gridCaption != balanceGridCaption &&
                gridCaption != outcomesTotalGridCaption)
            {
                if ((gridCaption == FKRGridCaption || gridCaption == EKRGridCaption))
                {
                    e.Layout.Grid.Columns[0].Width = CRHelper.GetColumnWidth(263);
                }
                else
                {
                    e.Layout.Grid.Columns[0].Width = CRHelper.GetColumnWidth(303);
                }
            }
            else
            {
                e.Layout.Grid.Columns[0].Width = CRHelper.GetColumnWidth(230);
            }

            CRHelper.FormatNumberColumn(e.Layout.Grid.Columns[i + 1], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Grid.Columns[i + 2], "N1");
            CRHelper.FormatNumberColumn(e.Layout.Grid.Columns[i + 3], "N0");
            #region settings headerlayout
            #region Incomes
            headerLayoutGridIncomes.AddCell("Доходы");
            headerLayoutGridIncomes.AddCell(String.Format("Годовые назначения, {0}",
                                                          RubMiltiplierButtonList.SelectedValue), "План на год");
            headerLayoutGridIncomes.AddCell(String.Format("Исполнено, {0}",
                                                          RubMiltiplierButtonList.SelectedValue),
                                            "Фактическое исполнение с начала года");
            headerLayoutGridIncomes.AddCell("Исполнено %");
            headerLayoutGridIncomes.ApplyHeaderInfo();
            #endregion

            #region OutcomesTotal
            headerLayoutGridOutcomes.AddCell("Расходы");
            headerLayoutGridOutcomes.AddCell(String.Format("Годовые назначения, {0}",
                                                          RubMiltiplierButtonList.SelectedValue), "План на год");
            headerLayoutGridOutcomes.AddCell(String.Format("Исполнено, {0}",
                                                          RubMiltiplierButtonList.SelectedValue),
                                            "Фактическое исполнение с начала года");
            headerLayoutGridOutcomes.AddCell("Исполнено %");
            headerLayoutGridOutcomes.ApplyHeaderInfo();
            #endregion

            #region deficit
            headerLayoutGridDeficit.AddCell("Дефицит/Профицит");
            headerLayoutGridDeficit.AddCell(String.Format("Годовые назначения, {0}",
                                                          RubMiltiplierButtonList.SelectedValue), "План на год");
            headerLayoutGridDeficit.AddCell(String.Format("Исполнено, {0}",
                                                          RubMiltiplierButtonList.SelectedValue),
                                            "Фактическое исполнение с начала года");
            headerLayoutGridDeficit.ApplyHeaderInfo();
            #endregion

            #region EkrOutcomes

            headerLayoutGridEkrOutcomes.AddCell("Расходы по КОСГУ");
            headerLayoutGridEkrOutcomes.AddCell("Код");
            headerLayoutGridEkrOutcomes.AddCell(String.Format("Годовые назначения, {0}",
                                              RubMiltiplierButtonList.SelectedValue), "План на год");
            headerLayoutGridEkrOutcomes.AddCell(String.Format("Исполнено, {0}",
                                                          RubMiltiplierButtonList.SelectedValue),
                                            "Фактическое исполнение с начала года");
            headerLayoutGridEkrOutcomes.AddCell("Исполнено %", "Процент исполнения годового плана. Индикация отклонения от среднего процента для выбранного бюджета");
            headerLayoutGridEkrOutcomes.ApplyHeaderInfo();
            #endregion

            #region FkrOutcomes
            headerLayoutGridFkrOutcomes.AddCell("Расходы на РзПр");
            headerLayoutGridFkrOutcomes.AddCell("Код");
            headerLayoutGridFkrOutcomes.AddCell(String.Format("Годовые назначения, {0}",
                                              RubMiltiplierButtonList.SelectedValue), "План на год");
            headerLayoutGridFkrOutcomes.AddCell(String.Format("Исполнено, {0}",
                                                          RubMiltiplierButtonList.SelectedValue),
                                            "Фактическое исполнение с начала года");
            headerLayoutGridFkrOutcomes.AddCell("Исполнено %", "Процент исполнения годового плана. Индикация отклонения от среднего процента для выбранного бюджета");
            headerLayoutGridFkrOutcomes.ApplyHeaderInfo();
            #endregion

            #region Debs

            headerLayoutGridDebs.AddCell("Задолженность");
            headerLayoutGridDebs.AddCell(String.Format("Исполнено, {0}",
                                                          RubMiltiplierButtonList.SelectedValue), "Фактическое исполнение с начала года");
            headerLayoutGridDebs.ApplyHeaderInfo();
            #endregion

            #endregion
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            string gridCaption = e.Row.Band.Grid.Caption;
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];

                cell.Style.Padding.Top = 5;
                cell.Style.Padding.Bottom = 5;

                if (gridCaption == incomesGridCaption || gridCaption == FKRGridCaption ||
                    gridCaption == EKRGridCaption || gridCaption == outcomesTotalGridCaption)
                {
                    int columnIndex = 3;
                    double percent = 0;

                    switch (gridCaption)
                    {
                        case incomesGridCaption:
                            {
                                columnIndex = 3;
                                percent = incomesLimit;
                                break;
                            }
                        case FKRGridCaption:
                            {
                                columnIndex = 4;
                                percent = FKRLimit;
                                break;
                            }
                        case EKRGridCaption:
                            {
                                columnIndex = 5;
                                percent = FKRLimit;
                                break;
                            }
                        case outcomesTotalGridCaption:
                            {
                                columnIndex = 3;
                                percent = FKRLimit;
                                break;
                            }
                    }

                    if (gridCaption == FKRGridCaption && i == 1 && cell.Value != null &&
                        cell.Value.ToString() != string.Empty)
                    {
                        cell.Value = Convert.ToDouble(cell.Value).ToString("00 00");
                    }

                    if (i == columnIndex && cell.Value != null && cell.Value.ToString() != string.Empty)
                    {
                        if (Convert.ToDouble(Convert.ToDouble(cell.Value)) < Convert.ToDouble(percent))
                        {
                            cell.Style.BackgroundImage = "~/images/ballRedBB.png";
                            cell.Title = string.Format("Ниже среднего ({0:N0}%)", percent);
                        }
                        else
                        {
                            cell.Style.BackgroundImage = "~/images/ballGreenBB.png";
                            if (Convert.ToDouble(Convert.ToDouble(cell.Value)) > Convert.ToDouble(percent))
                            {
                                cell.Title = string.Format("Выше среднего ({0:N0}%)", percent);
                            }
                            else
                            {
                                cell.Title = string.Format("Равно среднему ({0:N0}%)", percent);
                            }
                        }
                        cell.Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }

                    if (gridCaption == EKRGridCaption && e.Row.Cells[1] != null)
                    {
                        string levelName = e.Row.Cells[1].ToString();
                        // проверяем множащиеся значения
                        for (int j = 3; j <= 4; j++)
                        {
                            levelName = RegionsNamingHelper.CheckMultiplyValue(levelName, j);
                        }

                        int fontSize = 8;
                        bool bold = false;
                        bool italic = false;
                        switch (levelName)
                        {
                            case "Статья":
                                {
                                    fontSize = 8;
                                    bold = false;
                                    italic = false;
                                    break;
                                }
                            case "Подстатья":
                                {
                                    fontSize = 8;
                                    bold = false;
                                    italic = true;
                                    break;
                                }
                        }
                        e.Row.Cells[i].Style.Font.Size = fontSize;
                        e.Row.Cells[i].Style.Font.Bold = bold;
                        e.Row.Cells[i].Style.Font.Italic = italic;
                    }
                }

                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
            }
        }

        #endregion

        #region Экпорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Расходы по КОСГУ");
            Worksheet sheet2 = workbook.Worksheets.Add("Расходы по РзПр");
            Worksheet sheet3 = workbook.Worksheets.Add("Задолженность");
            Worksheet sheet4 = workbook.Worksheets.Add("Доходы");
            Worksheet sheet5 = workbook.Worksheets.Add("Расходы Итого");
            Worksheet sheet6 = workbook.Worksheets.Add("Дефицит Профицит");
            Worksheet sheet7 = workbook.Worksheets.Add("Карта");

            foreach (Worksheet sheet in workbook.Worksheets)
            {
                if (sheet.Name == "Расходы по КОСГУ" || sheet.Name == "Расходы по РзПр")
                {
                    sheet.Columns[0].Width = 340 * 37;
                    sheet.Columns[1].Width = 30 * 37;
                    sheet.Columns[2].Width = 120 * 37;
                    sheet.Columns[3].Width = 120 * 37;
                    sheet.Columns[4].Width = 100 * 37;

                    sheet.Columns[1].CellFormat.FormatString = "#,##0";
                    sheet.Columns[2].CellFormat.FormatString = "#,##0.0";
                    sheet.Columns[3].CellFormat.FormatString = "#,##0.0";
                    sheet.Columns[4].CellFormat.FormatString = "#,##0.0";
                }
                else
                {
                    sheet.Columns[0].Width = 340 * 37;
                    sheet.Columns[1].Width = 120 * 37;
                    sheet.Columns[2].Width = 120 * 37;
                    sheet.Columns[3].Width = 100 * 37;

                    sheet.Columns[1].CellFormat.FormatString = "#,##0.0";
                    sheet.Columns[2].CellFormat.FormatString = "#,##0.0";
                    sheet.Columns[3].CellFormat.FormatString = "#,##0.0";
                }
            }

            sheet1.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            sheet1.Rows[1].Cells[0].Value = "Расходы по КОСГУ";

            sheet2.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            sheet2.Rows[1].Cells[0].Value = "Расходы по ФКР";

            sheet3.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            sheet3.Rows[1].Cells[0].Value = "Задолженность";

            sheet4.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            sheet4.Rows[1].Cells[0].Value = "Доходы";

            sheet5.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            sheet5.Rows[1].Cells[0].Value = "Расходы Итого";

            sheet6.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            sheet6.Rows[1].Cells[0].Value = "Дефицит/Профицит";

            sheet7.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;


            ReportExcelExporter1.ExcelExporter.DownloadName = "report.xls";
            ReportExcelExporter1.Export(headerLayoutGridIncomes, sheet1, 3);
            ReportExcelExporter1.Export(headerLayoutGridOutcomes, sheet2, 3);
            ReportExcelExporter1.Export(headerLayoutGridDeficit, sheet3, 3);
            ReportExcelExporter1.Export(headerLayoutGridEkrOutcomes, sheet4, 3);
            ReportExcelExporter1.Export(headerLayoutGridFkrOutcomes, sheet5, 3);
            ReportExcelExporter1.Export(headerLayoutGridDebs, sheet6, 3);
            ReportExcelExporter1.Export(DundasMap, sheet7, 3);
        }

        #endregion


        private void PdfExportButton_Click(object sender, EventArgs e)
        {

            Report report = new Report();
            
            ISection section1 = report.AddSection();

            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayoutGridIncomes, PageSubTitle.Text, section1);
            ReportPDFExporter1.Export(headerLayoutGridOutcomes, section1);
            ReportPDFExporter1.Export(headerLayoutGridDeficit, section1);
            ISection section2 = report.AddSection();
            ReportPDFExporter1.Export(headerLayoutGridEkrOutcomes, PageSubTitle.Text, section2);
            ISection section3 = report.AddSection();
            ReportPDFExporter1.Export(headerLayoutGridFkrOutcomes,PageSubTitle.Text, section3);
            ISection section4 = report.AddSection();
            ReportPDFExporter1.Export(headerLayoutGridDebs,PageSubTitle.Text, section4);

            ISection section5 = report.AddSection();
            ReportPDFExporter1.Export(DundasMap, PageSubTitle.Text, section5);
        }
    }
}