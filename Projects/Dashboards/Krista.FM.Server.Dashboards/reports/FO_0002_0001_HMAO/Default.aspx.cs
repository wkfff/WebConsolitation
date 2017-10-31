using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls;

using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Band;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.TOC;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using IList = Infragistics.Documents.Reports.Report.List.IList;
using InitializeRowEventHandler = Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;
using SerializationFormat=Dundas.Maps.WebControl.SerializationFormat;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0001_HMAO
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

        private int firstYear = 2008;
        private int endYear = 2011;
        private string month = "Январь";

        private double incomesLimit = 0;
        private double FKRLimit = 0;

        private string mapFolderName;
        private int mapWidth;
        private double mapSizeProportion;

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
        // исключать внутренние обороты
        private CustomParam internalCircualtionExtruding;
        // исключать дубликат итогового элемента
        private CustomParam totalDublicateExtruding;

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
            internalCircualtionExtruding = UserParams.CustomParam("internal_circulation_extruding");
            totalDublicateExtruding = UserParams.CustomParam("total_dublicate_extruding");

            #endregion

            mapFolderName = RegionSettingsHelper.Instance.GetPropertyValue("MapFolderName");
            mapWidth = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("MapWidth"));
            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));

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

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PdfExporter_BeginExport);
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
            UserParams.KDInternalCircualtionExtruding.Value = RegionSettingsHelper.Instance.KDInternalCircualtionExtruding;
            UserParams.RzPrInternalCircualtionExtruding.Value = RegionSettingsHelper.Instance.RzPrInternalCircualtionExtruding;

            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string queryName =
                    string.Format("FO_0002_0001_HMAO_date_{0}",
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

            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";
            internalCircualtionExtruding.Value = "False";

            regionSelected = false;
            bool selectMap = false;
            totalDublicateExtruding.Value = "false";
            switch (ComboBudget.SelectedValue)
            {
                case "Консолидированный бюджет субъекта":
                    {
                        UserParams.BudgetLevelEnum.Value = "[Уровни бюджета].[СКИФ].[Конс.бюджет субъекта]";
                        totalDublicateExtruding.Value = "true";
                        break;
                    }
                case "Местные бюджеты":
                    {
                        UserParams.BudgetLevelEnum.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetLevel");
                        internalCircualtionExtruding.Value = "True";
                        break;
                    }
                default:
                    {
                        if (ComboBudget.SelectedValue == RegionSettingsHelper.Instance.OwnSubjectBudgetName)
                        {
                            UserParams.BudgetLevelEnum.Value = "[Уровни бюджета].[СКИФ].[Бюджет субъекта]";
                            totalDublicateExtruding.Value = "true";
                        }
                        else
                        {
                            UserParams.Region.Value = RegionsNamingHelper.LocalBudgetUniqueNames[ComboBudget.SelectedValue];
                            string selectedRegionType = RegionsNamingHelper.LocalBudgetTypes[ComboBudget.SelectedValue];
                            if (selectedRegionType.Contains("МР"))
                            {
                                regionSelected = true;
                                UserParams.BudgetLevelEnum.Value = UseConsolidateRegionBudget
                                                                       ? "[Уровни бюджета].[СКИФ].[Бюджет района и поселений]"
                                                                       : RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetLevel");
                                internalCircualtionExtruding.Value = UseConsolidateRegionBudget ? "True" : "False";
                                documentType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");
                            }
                            else if (selectedRegionType.Contains("ГО"))
                            {
                                UserParams.BudgetLevelEnum.Value = RegionSettingsHelper.Instance.GetPropertyValue("DistrictBudgetLevel");
                                documentType.Value = RegionSettingsHelper.Instance.GetPropertyValue("DistrictDocumentSKIFType");
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
            IndicationCommentLabel.Text =
                String.Format("<img style=\"vertical-align:middle\" src=\"../../images/ballRedBB.png\">/<img style=\"vertical-align:middle\" src=\"../../images/ballGreenBB.png\"> – сравнение процента исполнения уточненных годовых назначений по показателю доходов (расходов) со средним процентом исполнения в целом по доходам (расходам) анализируемого бюджета");

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
                    DundasMap.Serializer.Load(Server.MapPath(string.Format("../../maps/Субъекты/{0}/{0}_СТВГ.bin", mapFolderName)));
                }
                else
                {
                    CRHelper.AddMapLayer(DundasMap,
                                     Server.MapPath(String.Format("../../maps/Субъекты/{0}/Соседние.shp", mapFolderName)),
                                     true, CRHelper.MapShapeType.SublingAreas);
                    CRHelper.AddMapLayer(DundasMap,
                                         Server.MapPath(String.Format("../../maps/Субъекты/{0}/Территор.shp", mapFolderName)),
                                         true, CRHelper.MapShapeType.Areas);
                    CRHelper.AddMapLayer(DundasMap, 
                                         Server.MapPath(String.Format("../../maps/Субъекты/{0}/Водные.shp", mapFolderName)), 
                                         false, CRHelper.MapShapeType.WaterObjects);
                    CRHelper.AddMapLayer(DundasMap,
                                         Server.MapPath(String.Format("../../maps/Субъекты/{0}/Города.shp", mapFolderName)),
                                         true, CRHelper.MapShapeType.Towns);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Невозможно загрузить карту региона", ex);
            }

            // скрываем города, которых нет в словаре локальных бюджетов
            foreach (Shape shape in DundasMap.Shapes)
            {
                shape.ToolTip = shape.ToolTip.Replace("\"", "'");
                if ((shape.Layer == "Города" || shape.Layer == "Территор") && !RegionsNamingHelper.LocalBudgetUniqueNames.ContainsKey(shape.Name))
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
                if (shape.Layer != "Территор" && shape.Layer != "Города" || !RegionsNamingHelper.LocalBudgetUniqueNames.ContainsKey(shape.Name))
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
            PageSubTitle.Text = String.Format("{3} за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), ComboYear.SelectedValue, selectedBudget);

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
            string query = DataProvider.GetQueryText("FO_0002_0001_HMAO_ekrOutcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, EKRGridCaption, dt_ekrOutcomes);

            grid_ekrOutcomes.DataSource = dt_ekrOutcomes;
        }

        protected void grid2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0001_HMAO_fkrOutcomes");
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
            string query = DataProvider.GetQueryText("FO_0002_0001_HMAO_Incomes");
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
            string query = DataProvider.GetQueryText("FO_0002_0001_HMAO_Deficit");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, balanceGridCaption, dt_deficit);

            grid_deficit.DataSource = dt_deficit;
        }

        protected void grid5_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0001_HMAO_Debts");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, debtsGridCaption, dt_debts);

            grid_debts.DataSource = dt_debts;
        }

        protected void grid6_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0001_HMAO_fkrOutcomes_total");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, outcomesTotalGridCaption, dt_outcomesTotal);

            grid_outcomesTotal.DataSource = dt_outcomesTotal;
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.FilterOptionsDefault.AllowRowFiltering = RowFiltering.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.HeaderStyleDefault.Wrap = true;

            if (e.Layout.Grid.Columns.Count == 0)
            {
                return;
            }

            e.Layout.Grid.Columns[0].Width = CRHelper.GetColumnWidth(275);
            e.Layout.Grid.Columns[0].CellStyle.Wrap = true;

            string gridCaption = e.Layout.Grid.Caption;
            int i = 0;
            
            switch (gridCaption)
            {
                case EKRGridCaption:
                    {
                        i = 1;
                        e.Layout.Grid.Columns[1].Hidden = true;

                        break;
                    }
            }
            
            if (gridCaption == FKRGridCaption || gridCaption == EKRGridCaption)
            {
                int j = gridCaption == FKRGridCaption ? 1 : 2;
                
                e.Layout.Grid.Columns[j].Width = CRHelper.GetColumnWidth(40);
                e.Layout.Grid.Columns[j].Header.Caption = "Код";
                e.Layout.Grid.Columns[j].Header.Title = "Код";
                e.Layout.Grid.Columns[j].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Grid.Columns[j].CellStyle.Padding.Right = 5;
                CRHelper.FormatNumberColumn(e.Layout.Grid.Columns[j], "00 00");

                i++;
            }

            e.Layout.Grid.Columns[i + 1].Width = CRHelper.GetColumnWidth(105);
            e.Layout.Grid.Columns[i + 1].Header.Caption = String.Format("Уточненные годовые назначения, {0}", RubMiltiplierButtonList.SelectedValue);
            e.Layout.Grid.Columns[i + 1].Header.Title = "Уточненный план на год";

            e.Layout.Grid.Columns[i + 2].Width = CRHelper.GetColumnWidth(98);
            e.Layout.Grid.Columns[i + 2].Header.Caption = String.Format("Исполнено, {0}", RubMiltiplierButtonList.SelectedValue);
            e.Layout.Grid.Columns[i + 2].Header.Title = "Фактическое исполнение с начала года";

            e.Layout.Grid.Columns[i + 3].Width = CRHelper.GetColumnWidth(73);
            e.Layout.Grid.Columns[i + 3].Header.Caption = "Исполнено %";

            if (gridCaption == debtsGridCaption || gridCaption == balanceGridCaption)
            {
                e.Layout.Grid.Columns[i + 3].Hidden = true;
                if (gridCaption == debtsGridCaption)
                {
                    e.Layout.Grid.Columns[i + 1].Hidden = true;
                    e.Layout.Grid.Columns[i + 2].Width = CRHelper.GetColumnWidth(278);
                }
                else
                {
                    e.Layout.Grid.Columns[i + 1].Width = CRHelper.GetColumnWidth(105);
                    e.Layout.Grid.Columns[i + 2].Width = CRHelper.GetColumnWidth(172);
                }

                e.Layout.Grid.Columns[i + 3].Header.Title = "Процент исполнения годового плана";
            }
            else
            {
                e.Layout.Grid.Columns[i + 3].Header.Title = "Процент исполнения годового плана. Индикация отклонения от среднего процента для выбранного бюджета";
            }

            if (gridCaption != incomesGridCaption && gridCaption != balanceGridCaption && gridCaption != outcomesTotalGridCaption)
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
                        cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
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

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.Workbook.Worksheets["Расходы по КОСГУ"].Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            e.Workbook.Worksheets["Расходы по КОСГУ"].Rows[1].Cells[0].Value = "Расходы по КОСГУ";

            e.Workbook.Worksheets["Расходы по РзПр"].Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            e.Workbook.Worksheets["Расходы по РзПр"].Rows[1].Cells[0].Value = "Расходы по ФКР";

            e.Workbook.Worksheets["Задолженность"].Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            e.Workbook.Worksheets["Задолженность"].Rows[1].Cells[0].Value = "Задолженность";

            e.Workbook.Worksheets["Доходы"].Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            e.Workbook.Worksheets["Доходы"].Rows[1].Cells[0].Value = "Доходы";

            e.Workbook.Worksheets["Расходы Итого"].Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            e.Workbook.Worksheets["Расходы Итого"].Rows[1].Cells[0].Value = "Расходы Итого";
            
            e.Workbook.Worksheets["Дефицит Профицит"].Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            e.Workbook.Worksheets["Дефицит Профицит"].Rows[1].Cells[0].Value = "Дефицит/Профицит";
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            foreach (Worksheet sheet in e.Workbook.Worksheets)
            {
                if (sheet.Name == "Расходы по КОСГУ" || sheet.Name == "Расходы по РзПр")
                {
                    sheet.Columns[0].Width = 340*37;
                    sheet.Columns[1].Width = 30*37;
                    sheet.Columns[2].Width = 120*37;
                    sheet.Columns[3].Width = 120*37;
                    sheet.Columns[4].Width = 100*37;

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
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Расходы по КОСГУ");
            Worksheet sheet2 = workbook.Worksheets.Add("Расходы по РзПр");
            Worksheet sheet3 = workbook.Worksheets.Add("Задолженность");
            Worksheet sheet4 = workbook.Worksheets.Add("Доходы");
            Worksheet sheet5 = workbook.Worksheets.Add("Расходы Итого");
            Worksheet sheet6 = workbook.Worksheets.Add("Дефицит Профицит");

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(grid_ekrOutcomes, sheet1);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(grid_fkrOutcomes, sheet2);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(grid_debts, sheet3);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(grid_incomes, sheet4);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(grid_outcomesTotal, sheet5);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(grid_deficit, sheet6);
         }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            Report report = new Report();
            ReportSection section1 = new ReportSection(report, true);

            UltraGridExporter1.PdfExporter.Export(grid_incomes, section1);
            UltraGridExporter1.PdfExporter.Export(grid_outcomesTotal, section1);
            UltraGridExporter1.PdfExporter.Export(grid_deficit, section1);

            section1.AddFlowColumnBreak();
            UltraGridExporter1.PdfExporter.Export(grid_ekrOutcomes, section1);
            UltraGridExporter1.PdfExporter.Export(grid_fkrOutcomes, section1);
            UltraGridExporter1.PdfExporter.Export(grid_debts, section1);
        }

        private bool titleAndMapAdded = false;

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            InitializeExportLayout(e);

            if (titleAndMapAdded)
                return;

            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Font(font);
            title.AddContent(PageSubTitle.Text);

            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);

            titleAndMapAdded = true;
        }

        private void InitializeExportLayout(DocumentExportEventArgs e)
        {
            string gridCaption = e.Layout.Bands[0].Grid.Caption;
            int i = 0;
            if (gridCaption == EKRGridCaption)
            {
                i = 1;
                e.Layout.Grid.Columns[1].Hidden = true;
            }

            if (gridCaption == FKRGridCaption || gridCaption == EKRGridCaption)
            {
                e.Layout.Grid.Columns[i + 1].Width = CRHelper.GetColumnWidth(50);
                i++;
            }

            if (gridCaption == balanceGridCaption)
            {
                e.Layout.Grid.Columns[i + 1].Width = 108;
                e.Layout.Grid.Columns[i + 2].Width = 180;
            }
            else if (gridCaption == debtsGridCaption)
            {
                e.Layout.Grid.Columns[i + 2].Width = 340;
            }
            
            if (gridCaption != incomesGridCaption && gridCaption != balanceGridCaption &&
                gridCaption != outcomesTotalGridCaption)
            {
                e.Layout.Grid.Columns[0].Width = 400;
            }
            else if (gridCaption == FKRGridCaption || gridCaption == EKRGridCaption)
            {
                e.Layout.Grid.Columns[0].Width = 205;
            }
            else
            {
                e.Layout.Grid.Columns[0].Width = 260;
            }

            //DundasMap.Width = (int)(CustomReportConst.minScreenWidth * mapSizeProportion);
            DundasMap.Height = (int) (DundasMap.Width.Value*mapSizeProportion);
        }

        #endregion
    }

    public class ReportSection : ISection
    {
        private readonly bool withFlowColumns;
        private readonly ISection section;
        private IFlow flow;
        private ITableCell titleCell;

        public ReportSection(Report report, bool withFlowColumns)
        {
            this.withFlowColumns = withFlowColumns;
            section = report.AddSection();
            ITable table = section.AddTable();
            ITableRow row = table.AddRow();
            titleCell = row.AddCell();
            if (this.withFlowColumns)
            {
                flow = section.AddFlow();
                IFlowColumn col = flow.AddColumn();
                col.Width = new FixedWidth(450);
                col = flow.AddColumn();
                col.Width = new FixedWidth(600);
            }
        }

        public void AddFlowColumnBreak()
        {
            if (flow != null)
                flow.AddColumnBreak();
        }

        public IBand AddBand()
        {
            if (flow != null)
                return flow.AddBand();
            return section.AddBand();
        }

        #region ISection members
        public ISectionHeader AddHeader()
        {
            throw new NotImplementedException();
        }

        public ISectionFooter AddFooter()
        {
            throw new NotImplementedException();
        }

        public IStationery AddStationery()
        {
            throw new NotImplementedException();
        }

        public IDecoration AddDecoration()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(PageSize size)
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(float width, float height)
        {
            throw new NotImplementedException();
        }

        public ISegment AddSegment()
        {
            throw new NotImplementedException();
        }

        public IQuickText AddQuickText(string text)
        {
            throw new NotImplementedException();
        }

        public IQuickImage AddQuickImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            throw new NotImplementedException();
        }

        public IQuickList AddQuickList()
        {
            throw new NotImplementedException();
        }

        public IQuickTable AddQuickTable()
        {
            throw new NotImplementedException();
        }

        public IText AddText()
        {
            return this.titleCell.AddText();
        }

        public IImage AddImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            if (flow != null)
                return flow.AddImage(image);
            return this.section.AddImage(image);
        }

        public IMetafile AddMetafile(Metafile metafile)
        {
            throw new NotImplementedException();
        }

        public IRule AddRule()
        {
            throw new NotImplementedException();
        }

        public IGap AddGap()
        {
            throw new NotImplementedException();
        }

        public IGroup AddGroup()
        {
            throw new NotImplementedException();
        }

        public IChain AddChain()
        {
            throw new NotImplementedException();
        }

        public ITable AddTable()
        {
            if (flow != null)
                return flow.AddTable();
            return this.section.AddTable();
        }

        public IGrid AddGrid()
        {
            throw new NotImplementedException();
        }

        public IFlow AddFlow()
        {
            throw new NotImplementedException();
        }

        public IList AddList()
        {
            throw new NotImplementedException();
        }

        public ITree AddTree()
        {
            throw new NotImplementedException();
        }

        public ISite AddSite()
        {
            throw new NotImplementedException();
        }

        public ICanvas AddCanvas()
        {
            throw new NotImplementedException();
        }

        public IRotator AddRotator()
        {
            throw new NotImplementedException();
        }

        public IContainer AddContainer(string name)
        {
            throw new NotImplementedException();
        }

        public ICondition AddCondition(IContainer container, bool fit)
        {
            throw new NotImplementedException();
        }

        public IStretcher AddStretcher()
        {
            throw new NotImplementedException();
        }

        public void AddPageBreak()
        {
            throw new NotImplementedException();
        }

        public ITOC AddTOC()
        {
            throw new NotImplementedException();
        }

        public IIndex AddIndex()
        {
            throw new NotImplementedException();
        }

        public bool Flip
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public PageSize PageSize
        {
            get { throw new NotImplementedException(); }
            set { this.section.PageSize = new PageSize(1200, 1600); }
        }

        public PageOrientation PageOrientation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ContentAlignment PageAlignment
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Borders PageBorders
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Margins PageMargins
        {
            get { return this.section.PageMargins; }
            set { throw new NotImplementedException(); }
        }

        public Paddings PagePaddings
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Background PageBackground
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Section.PageNumbering PageNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public SectionLineNumbering LineNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Report Parent
        {
            get { return this.section.Parent; }
        }

        public IEnumerable Content
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}