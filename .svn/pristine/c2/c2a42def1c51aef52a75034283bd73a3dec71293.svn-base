using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using SerializationFormat = Dundas.Maps.WebControl.SerializationFormat;
using Infragistics.Documents.Reports.Report.Text;
using System.Drawing.Imaging;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0014
{
    public enum ColoringRule
    {
        // простая раскраска диапазоном
        SimpleRangeColoring,
        // более сложная раскраска диапазоном
        ComplexRangeColoring,
        // положительные/отрицательные
        PositiveNegativeColoring
    }

    /// <summary>
    /// Положение легенды
    /// </summary>
    public enum LegendPosition
    {
        LeftTop,
        LeftBottom,
        RightTop,
        RightBottom
    }

    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2007;
        private int endYear = 2011;
        private string gridQueryName;
        private int mapColumnIndex1;
        private int mapColumnIndex2;
        private Color beginMapColor;
        private Color endMapColor;
        private Color nullEmptyColor;
        private string nullEmptyText;
        private string mapFormatString;
        private string mapIndicatorName1;
        private string mapIndicatorName2;
        private bool twoMapVisible;
        private ColoringRule coloringRule;
        private string positiveValueText;
        private string negativeValueText;
        private string mapSimpleColoringUnits;
        private double mapZoomValue;
        private int measuresCount;
        private LegendPosition mapLegendPositon;

        private bool loadFromBin = false;
        private double calloutOffsetY = 0;

        private bool nullShapeHidden = true;
        private bool useRegionCodes = true;

        private GridHeaderLayout gridHeaderLayout;
        private DateTime currentDate;

        private bool isDeficitSelected = false;
        private bool isBudgetarySurplusesSelected = false;
        private bool isCreditDebtsSelected = false;
        private bool isInnerDebtsMOSelected = false;

        private string currDateStr;
        private string lastDateStr;

        #endregion

        #region Параметры запроса

        // Уровень районов
        private CustomParam regionsLevel;
        // Выбранный показатель
        private CustomParam selectedIndicator;

        // тип документа СКИФ для консолидированного бюджета субъекта
        private CustomParam consolidateBudgetDocumentSKIFType;
        // тип документа СКИФ для собственного бюджета субъекта
        private CustomParam ownBudgetDocumentSKIFType;
        // тип документа СКИФ для районов
        private CustomParam regionBudgetDocumentSKIFType;

        // уровень бюджета для графы "Бюджет гор.округа, мун.района и поселения"
        private CustomParam consolidateRegionBudgetLevel;

        // консолидированный бюджет субъекта
        private CustomParam regionsConsolidateBudget;
        // собственный бюджет субъекта
        private CustomParam regionsOwnBudget;

        // удельный вес
        private CustomParam shareMeasure;
        // фильтр показателя
        private CustomParam filterIndicator;
        // доходы ВСЕГО
        private CustomParam incomesAll;
        // расходы ФКР ВСЕГО
        private CustomParam outcomesFKRAll;

        //         // Элемент КИФ "Получение кредитов ..."
        //        private CustomParam creditReceipt;
        //        // Элемент КИФ "Погашение кредитов ..."
        //        private CustomParam creditRepayment;


        // фильтр по КОСГУ
        private CustomParam kosguFilter;

        #endregion

        private bool shapeTextShown = true;
        private string mapFolder;

        private double rubMultiplier;

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "Тыс.руб." : "Млн.руб."; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 120);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            #region Инициализация параметров запроса

            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (selectedIndicator == null)
            {
                selectedIndicator = UserParams.CustomParam("selected_indicator");
            }
            if (consolidateBudgetDocumentSKIFType == null)
            {
                consolidateBudgetDocumentSKIFType = UserParams.CustomParam("consolidate_budget_document_skif_type");
            }
            if (ownBudgetDocumentSKIFType == null)
            {
                ownBudgetDocumentSKIFType = UserParams.CustomParam("own_budget_document_skif_type");
            }
            if (regionBudgetDocumentSKIFType == null)
            {
                regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            }
            if (regionsConsolidateBudget == null)
            {
                regionsConsolidateBudget = UserParams.CustomParam("regions_consolidate_budget");
            }
            if (regionsOwnBudget == null)
            {
                regionsOwnBudget = UserParams.CustomParam("regions_own_budget");
            }

            if (shareMeasure == null)
            {
                shareMeasure = UserParams.CustomParam("share_measure");
            }
            if (filterIndicator == null)
            {
                filterIndicator = UserParams.CustomParam("filter_indicator");
            }
            if (incomesAll == null)
            {
                incomesAll = UserParams.CustomParam("incomes_all");
            }
            if (outcomesFKRAll == null)
            {
                outcomesFKRAll = UserParams.CustomParam("outcomes_fkr_all");
            }

            if (consolidateRegionBudgetLevel == null)
            {
                consolidateRegionBudgetLevel = UserParams.CustomParam("consolidate_region_budget_level");
            }

            //            if (creditReceipt == null)
            //            {
            //                creditReceipt = UserParams.CustomParam("credit_receipt");
            //            }
            //            if (creditRepayment == null)
            //            {
            //                creditRepayment = UserParams.CustomParam("credit_repayment");
            //            }

            kosguFilter = UserParams.CustomParam("kosgu_filter");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            mapZoomValue = Convert.ToDouble(RegionSettingsHelper.Instance.GetPropertyValue("MapZoomValue"));
            loadFromBin = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("MapLoadFromBin"));
            calloutOffsetY = Convert.ToDouble(RegionSettingsHelper.Instance.GetPropertyValue("CalloutOffsetY"));
            nullShapeHidden = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("NullShapeHidden"));
            useRegionCodes = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("UseRegionCodes"));
            mapLegendPositon = GetLegendPositon(RegionSettingsHelper.Instance.GetPropertyValue("MapLegendPosition"));

            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));
            shapeTextShown = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("ShapeTextShown"));

            kosguFilter.Value = RegionSettingsHelper.Instance.GetPropertyValue("KosguFilter");

            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.FKRAllLevel.Value = RegionSettingsHelper.Instance.FKRAllLevel;
            UserParams.FKRSectionLevel.Value = RegionSettingsHelper.Instance.FKRSectionLevel;
            UserParams.FKRSubSectionLevel.Value = RegionSettingsHelper.Instance.FKRSubSectionLevel;
            UserParams.EKRDimension.Value = RegionSettingsHelper.Instance.EKRDimension;

            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;

            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;

            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0014_date");
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

                ComboIndicator.Width = 420;
                ComboIndicator.Title = "Анализируемый показатель";
                ComboIndicator.MultiSelect = false;
                ComboIndicator.ParentSelect = true;
                ComboIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillMOIndicatorList());
                ComboIndicator.SetСheckedState("Доходы бюджета ", true);
            }

            int year = Convert.ToInt32(ComboYear.SelectedValue);
            int month = ComboMonth.SelectedIndex + 1;

            currentDate = new DateTime(year, month, 1);

            Page.Title = string.Format("Анализ исполнения бюджетов муниципальных образований");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format(
                    "Картограмма основных показателей исполнения бюджета муниципальных образований за {0} {2} {1} года",
                    currentDate.Month, currentDate.Year,
                    CRHelper.RusManyMonthGenitive(currentDate.Month));

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            selectedIndicator.Value = ComboIndicator.SelectedValue;
            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            ownBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("OwnBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            regionsConsolidateBudget.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            regionsOwnBudget.Value = RegionSettingsHelper.Instance.RegionsOwnBudgetLevel;
            incomesAll.Value = RegionSettingsHelper.Instance.IncomeTotal;
            outcomesFKRAll.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;
            consolidateRegionBudgetLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateRegionBudgetLevel");

            SetQueryParams(ComboIndicator.SelectedValue);

            currDateStr = String.Format("за {0} {2} {1} года, {3}", currentDate.Month,
                                    currentDate.Year,
                                    CRHelper.RusManyMonthGenitive(currentDate.Month),
                                    RubMultiplierCaption.ToLower());
            lastDateStr = String.Format("за {0} {2} {1} года, {3}", currentDate.Month,
                                        currentDate.Year - 1,
                                        CRHelper.RusManyMonthGenitive(currentDate.Month),
                                        RubMultiplierCaption.ToLower());

            gridHeaderLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            mapFolder = RegionSettingsHelper.Instance.GetPropertyValue("MapFolderName");
            DundasMap1.Shapes.Clear();

            if (loadFromBin)
            {
                DundasMap1.Serializer.Format = SerializationFormat.Binary;
                DundasMap1.Serializer.Load(
                    Server.MapPath(string.Format("../../maps/Субъекты/{0}/{0}_выноски.bin", mapFolder)));
            }
            else
            {
                AddMapLayer(DundasMap1, mapFolder, "Территор", CRHelper.MapShapeType.Areas);
                AddMapLayer(DundasMap1, mapFolder, "Города", CRHelper.MapShapeType.Towns);
                AddMapLayer(DundasMap1, mapFolder, "Выноски", CRHelper.MapShapeType.CalloutTowns);
            }

            SetMapSettings(DundasMap1, 1);
            FillMapData(DundasMap1, 1);

            if (twoMapVisible)
            {
                DundasMap2.Shapes.Clear();
                if (loadFromBin)
                {
                    DundasMap2.Serializer.Format = SerializationFormat.Binary;
                    DundasMap2.Serializer.Load(
                        (Server.MapPath(string.Format("../../maps/Субъекты/{0}/{0}_выноски.bin", mapFolder))));
                }
                else
                {
                    AddMapLayer(DundasMap2, mapFolder, "Территор", CRHelper.MapShapeType.Areas);
                    AddMapLayer(DundasMap2, mapFolder, "Города", CRHelper.MapShapeType.Towns);
                    AddMapLayer(DundasMap2, mapFolder, "Выноски", CRHelper.MapShapeType.CalloutTowns);
                }
                SetMapSettings(DundasMap2, 2);
                FillMapData(DundasMap2, 2);
            }
        }

        private void AddMapLayer(MapControl map, string mapFolder, string layerFileName, CRHelper.MapShapeType shapeType)
        {
            string layerName = Server.MapPath(string.Format("../../maps/Субъекты/{0}/{1}.shp", mapFolder, layerFileName));

            if (!File.Exists(layerName))
            {
                return;
            }

            int oldShapesCount = map.Shapes.Count;
            string mapField = useRegionCodes ? "CODE" : "NAME";

            map.LoadFromShapeFile(layerName, "NAME", true);
            map.Layers.Add(shapeType.ToString());

            for (int i = oldShapesCount; i < map.Shapes.Count; i++)
            {
                Shape shape = map.Shapes[i];
                shape.Layer = shapeType.ToString();
            }
        }

        private void SetQueryParams(string indicatorName)
        {
            twoMapVisible = false;
            mapElementCaption2.Text = String.Empty;
            mapIndicatorName2 = String.Empty;
            mapColumnIndex2 = 5;
            nullEmptyText = String.Empty;
            positiveValueText = String.Empty;
            negativeValueText = String.Empty;
            mapSimpleColoringUnits = String.Empty;
            switch (indicatorName)
            {
                case "Доходы бюджета ":
                    {
                        gridQueryName = "FO_0002_0014_grid_incomes";
                        shareMeasure.Value = "";
                        filterIndicator.Value = RegionSettingsHelper.Instance.IncomeTotal;
                        mapElementCaption1.Text = "Темп роста доходов бюджетов муниципальных образований";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Red;
                        endMapColor = Color.Green;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста к прошлому году";
                        nullEmptyText = "Темп роста не рассчитывается";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Налоговые и неналоговые доходы ":
                    {
                        gridQueryName = "FO_0002_0014_grid_incomes";
                        shareMeasure.Value = "";
                        filterIndicator.Value = String.Format("{1}.[{0}]", RegionSettingsHelper.Instance.IncomesKDRootName,
                            RegionSettingsHelper.Instance.IncomesKDAllLevel);
                        mapElementCaption1.Text = "Темп роста налоговых и неналоговых доходов бюджетов муниципальных образований";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Red;
                        endMapColor = Color.Green;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста к прошлому году";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Темп роста не рассчитывается";
                        break;
                    }
                case "Налог на доходы физических лиц ":
                    {
                        gridQueryName = "FO_0002_0014_grid_incomes";
                        shareMeasure.Value = "";
                        filterIndicator.Value = String.Format("{1}.[{0}].[НАЛОГИ НА ПРИБЫЛЬ, ДОХОДЫ].[Налог на доходы физических лиц]",
                            RegionSettingsHelper.Instance.IncomesKDRootName, RegionSettingsHelper.Instance.IncomesKDAllLevel);
                        mapElementCaption1.Text = "Темп роста налога на доходы физических лиц";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Red;
                        endMapColor = Color.Green;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста к прошлому году";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Налоги на совокупный доход ":
                    {
                        gridQueryName = "FO_0002_0014_grid_incomes";
                        shareMeasure.Value = "";
                        filterIndicator.Value = String.Format("{1}.[{0}].[НАЛОГИ НА СОВОКУПНЫЙ ДОХОД]",
                            RegionSettingsHelper.Instance.IncomesKDRootName, RegionSettingsHelper.Instance.IncomesKDAllLevel);
                        mapElementCaption1.Text = "Темп роста налога на совокупный доход";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Red;
                        endMapColor = Color.Green;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста к прошлому году";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Земельный налог ":
                    {
                        gridQueryName = "FO_0002_0014_grid_incomes";
                        shareMeasure.Value = "";
                        filterIndicator.Value = String.Format("{1}.[{0}].[НАЛОГИ НА ИМУЩЕСТВО].[Земельный налог]",
                            RegionSettingsHelper.Instance.IncomesKDRootName, RegionSettingsHelper.Instance.IncomesKDAllLevel);
                        mapElementCaption1.Text = "Темп роста земельного налога";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Red;
                        endMapColor = Color.Green;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста к прошлому году";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Транспортный налог ":
                    {
                        gridQueryName = "FO_0002_0014_grid_incomes";
                        shareMeasure.Value = "";
                        filterIndicator.Value = String.Format("{1}.[{0}].[НАЛОГИ НА ИМУЩЕСТВО].[Транспортный налог]",
                            RegionSettingsHelper.Instance.IncomesKDRootName, RegionSettingsHelper.Instance.IncomesKDAllLevel);
                        mapElementCaption1.Text = "Темп роста транспортного налога";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Red;
                        endMapColor = Color.Green;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста к прошлому году";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Налог на имущество физических лиц ":
                    {
                        gridQueryName = "FO_0002_0014_grid_incomes";
                        shareMeasure.Value = "";
                        filterIndicator.Value = String.Format("{1}.[{0}].[НАЛОГИ НА ИМУЩЕСТВО].[Налог на имущество физических лиц]",
                            RegionSettingsHelper.Instance.IncomesKDRootName, RegionSettingsHelper.Instance.IncomesKDAllLevel);
                        mapElementCaption1.Text = "Темп роста налога на имущество физических лиц";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Red;
                        endMapColor = Color.Green;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста к прошлому году";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Прочие налоговые доходы ":
                    {
                        gridQueryName = "FO_0002_0014_grid_incomes";
                        shareMeasure.Value = "";
                        filterIndicator.Value = String.Format("{0}.[Прочие налоговые доходы ]", RegionSettingsHelper.Instance.IncomesKDDimension);
                        mapElementCaption1.Text = "Темп роста прочих налоговых доходов";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Red;
                        endMapColor = Color.Green;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста к прошлому году";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Неналоговые доходы ":
                    {
                        gridQueryName = "FO_0002_0014_grid_incomes";
                        shareMeasure.Value = "";
                        filterIndicator.Value = String.Format("{0}.[Неналоговые доходы ]", RegionSettingsHelper.Instance.IncomesKDDimension);
                        mapElementCaption1.Text = "Темп роста прочих неналоговых доходов";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Red;
                        endMapColor = Color.Green;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста к прошлому году";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Безвозмездные поступления ":
                    {
                        gridQueryName = "FO_0002_0014_grid_incomes";
                        shareMeasure.Value = ",[Measures].[Удельный вес]";
                        filterIndicator.Value = String.Format("{0}.[БЕЗВОЗМЕЗДНЫЕ ПОСТУПЛЕНИЯ]", RegionSettingsHelper.Instance.IncomesKDAllLevel);
                        mapElementCaption1.Text = "Доля безвозмездных поступлений в доходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес безвозм.доходов";
                        coloringRule = ColoringRule.ComplexRangeColoring;
                        nullEmptyText = "Безвозмездные поступления отсутствуют";
                        break;
                    }
                case "Дотации ":
                    {
                        gridQueryName = "FO_0002_0014_grid_incomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = String.Format("{0}.[БЕЗВОЗМЕЗДНЫЕ ПОСТУПЛЕНИЯ].[БЕЗВОЗМЕЗДНЫЕ ПОСТУПЛЕНИЯ ОТ ДРУГИХ БЮДЖЕТОВ БЮДЖЕТНОЙ СИСТЕМЫ РОССИЙСКОЙ ФЕДЕРАЦИИ].[Дотации бюджетам субъектов Российской Федерации и муниципальных образований]",
                            RegionSettingsHelper.Instance.IncomesKDAllLevel);
                        mapElementCaption1.Text = "Доля дотаций в доходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Доля дотаций в доходах";
                        coloringRule = ColoringRule.ComplexRangeColoring;
                        nullEmptyText = "Дотации отсутствуют";
                        break;
                    }
                case "на выравнивание бюдж. обеспеченности ":
                    {
                        gridQueryName = "FO_0002_0014_grid_incomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = String.Format("{0}.[БЕЗВОЗМЕЗДНЫЕ ПОСТУПЛЕНИЯ].[БЕЗВОЗМЕЗДНЫЕ ПОСТУПЛЕНИЯ ОТ ДРУГИХ БЮДЖЕТОВ БЮДЖЕТНОЙ СИСТЕМЫ РОССИЙСКОЙ ФЕДЕРАЦИИ].[Дотации бюджетам субъектов Российской Федерации и муниципальных образований].[Дотации на выравнивание бюджетной обеспеченности]",
                            RegionSettingsHelper.Instance.IncomesKDAllLevel);
                        mapElementCaption1.Text = "Доля дотаций на выравнивание бюджетной обеспеченности в доходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Доля дотаций в доходах";
                        coloringRule = ColoringRule.ComplexRangeColoring;
                        nullEmptyText = "Дотации отсутствуют";
                        break;
                    }
                case "Субвенции ":
                    {
                        gridQueryName = "FO_0002_0014_grid_incomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = String.Format("{0}.[БЕЗВОЗМЕЗДНЫЕ ПОСТУПЛЕНИЯ].[БЕЗВОЗМЕЗДНЫЕ ПОСТУПЛЕНИЯ ОТ ДРУГИХ БЮДЖЕТОВ БЮДЖЕТНОЙ СИСТЕМЫ РОССИЙСКОЙ ФЕДЕРАЦИИ].[Субвенции бюджетам субъектов Российской Федерации и муниципальных образований]",
                            RegionSettingsHelper.Instance.IncomesKDAllLevel);
                        mapElementCaption1.Text = "Доля субвенций в доходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Доля субвенций в доходах";
                        coloringRule = ColoringRule.ComplexRangeColoring;
                        nullEmptyText = "Субвенции отсутствуют";
                        break;
                    }
                case "Субсидии ":
                    {
                        gridQueryName = "FO_0002_0014_grid_incomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = String.Format("{0}.[БЕЗВОЗМЕЗДНЫЕ ПОСТУПЛЕНИЯ].[БЕЗВОЗМЕЗДНЫЕ ПОСТУПЛЕНИЯ ОТ ДРУГИХ БЮДЖЕТОВ БЮДЖЕТНОЙ СИСТЕМЫ РОССИЙСКОЙ ФЕДЕРАЦИИ].[Субсидии бюджетам субъектов Российской Федерации и муниципальных образований (межбюджетные субсидии)]",
                            RegionSettingsHelper.Instance.IncomesKDAllLevel);
                        mapElementCaption1.Text = "Доля субсидий в доходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Доля субсидий в доходах";
                        coloringRule = ColoringRule.ComplexRangeColoring;
                        nullEmptyText = "Субсидии отсутствуют";
                        break;
                    }
                case "Иные межбюджетные трансферты ":
                    {
                        gridQueryName = "FO_0002_0014_grid_incomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = String.Format("{0}.[БЕЗВОЗМЕЗДНЫЕ ПОСТУПЛЕНИЯ].[БЕЗВОЗМЕЗДНЫЕ ПОСТУПЛЕНИЯ ОТ ДРУГИХ БЮДЖЕТОВ БЮДЖЕТНОЙ СИСТЕМЫ РОССИЙСКОЙ ФЕДЕРАЦИИ].[Иные межбюджетные трансферты]",
                            RegionSettingsHelper.Instance.IncomesKDAllLevel);
                        mapElementCaption1.Text = "Доля иных межбюджетных трансфертов в доходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Доля иных межбюджетных трансфертов в доходах";
                        coloringRule = ColoringRule.ComplexRangeColoring;
                        nullEmptyText = "Иные межбюджетные трансферты отсутствуют";
                        break;
                    }
                case "Расходы бюджета ":
                case "В том числе по разделам ":
                case "В том числе по КОСГУ ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = "";
                        filterIndicator.Value = String.Format("{0}, {1}", RegionSettingsHelper.Instance.OutcomeFKRTotal, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов муниципальных образований";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Общегосударственные вопросы ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ",[Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{0}.[Все коды ФКР].[ОБЩЕГОСУДАРСТВЕННЫЕ ВОПРОСЫ], {1}", UserParams.FKRDimension.Value, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов на общегосударственные вопросы";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов\nна общегосударственные вопросы";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Нац.оборона ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{0}.[Все коды ФКР].[НАЦИОНАЛЬНАЯ ОБОРОНА], {1}", UserParams.FKRDimension.Value, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов на национальную оборону";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов\nна национальную оборону";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Нац.безопасность и правоохр. деятельность ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{0}.[Все коды ФКР].[Национальная безопасность и правоохранительная деятельность], {1}", UserParams.FKRDimension.Value, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов на национальную безопасность и правоохранительную деятельность";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов\nна национальную безопасность и правоохранительную\nдеятельность";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Национальная экономика ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{0}.[Все коды ФКР].[Национальная экономика], {1}", UserParams.FKRDimension.Value, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов на национальную экономику";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов\nна национальную экономику";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "ЖКХ ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{0}.[Все коды ФКР].[Жилищно-коммунальное хозяйство], {1}", UserParams.FKRDimension.Value, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов на ЖКХ";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов на ЖКХ";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Охрана окр.среды ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{0}.[Все коды ФКР].[Охрана окружающей среды], {1}", UserParams.FKRDimension.Value, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов на охрану окружающей среды";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов\nна охрану окружающей среды";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Образование ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{0}.[Все коды ФКР].[Образование], {1}", UserParams.FKRDimension.Value, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов на образование";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов\n на образование";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Культура, кинематография ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{0}.[Все коды ФКР].[Культура, кинематография], {1}", UserParams.FKRDimension.Value, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов на культуру, кинематографию";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов\nна культуру, кинематографию";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Здравоохранение ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{0}.[Все коды ФКР].[Здравоохранение], {1}", UserParams.FKRDimension.Value, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов на здравоохранение";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов\nна здравоохранение";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Социальная политика ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{0}.[Все коды ФКР].[Социальная политика], {1}", UserParams.FKRDimension.Value, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов на социальную политику";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов\nна социальную политику";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Физическая культура и спорт ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{0}.[Все коды ФКР].[Физическая культура и спорт], {1}", UserParams.FKRDimension.Value, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов на физическую культуру и спорт";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов\nна физическую культуру и спорт";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "СМИ ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{0}.[Все коды ФКР].[Средства массовой информации], {1}", UserParams.FKRDimension.Value, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов на СМИ";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов на СМИ";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Обслуживание гос. и мун. долга ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{0}.[Все коды ФКР].[Обслуживание государственного и муниципального долга], {1}", UserParams.FKRDimension.Value, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов на обслуживание государственного и муниципального долга";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов\nна обслуживание государственного и\nмуниципального долга";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Межбюджетн.трансферты общего характера бюджетам субъектов РФ и МО ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{0}.[Все коды ФКР].[МЕЖБЮДЖЕТНЫЕ ТРАНСФЕРТЫ ОБЩЕГО ХАРАКТЕРА БЮДЖЕТАМ СУБЪЕКТОВ РОССИЙСКОЙ ФЕДЕРАЦИИ И МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ], {1}", UserParams.FKRDimension.Value, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов на межбюджетн.трансферты общего характера бюджетам субъектов РФ и МО";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов\nна межбюджетн.трансферты общего характера бюджетам субъектов РФ и МО";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Межбюджетные трансферты ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{0}.[Все коды ФКР].[МЕЖБЮДЖЕТНЫЕ ТРАНСФЕРТЫ], {1}", UserParams.FKRDimension.Value, kosguFilter.Value);
                        mapElementCaption1.Text = "Темп роста расходов на межбюджетные трансферты";
                        mapColumnIndex1 = 5;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        nullEmptyText = "Темп роста не рассчитывается";
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста расходов\nна межбюджетные трансферты";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Заработная плата и начисления на выплаты по оплате труда ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Оплата труда и начисления на выплаты по оплате труда], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля расходов на оплату труда и начислений на выплаты по оплате труда в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес расходов\nна оплату труда";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Заработная плата ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Оплата труда и начисления на выплаты по оплате труда].[Заработная плата], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля расходов на заработную плату в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес расходов на заработную плату";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Прочие выплаты ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Оплата труда и начисления на выплаты по оплате труда].[Прочие выплаты], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля прочих выплат по оплате труда в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес прочих выплат по оплате труда";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Начисления на выплаты по оплате труда ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Оплата труда и начисления на выплаты по оплате труда].[Начисления на выплаты по оплате труда], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля начислений на выплаты по оплате труда в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес начислений\nна выплаты по оплате труда";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Оплата работ, услуг ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Оплата работ, услуг], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля расходов на оплату работ, услуг в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес расходов на оплату работ, услуг";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Услуги связи ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Оплата работ, услуг].[Услуги связи], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля расходов на услуги связи в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес расходов на услуги связи";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Транспортные услуги ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Оплата работ, услуг].[Транспортные услуги], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля расходов на транспортные услуги в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес расходов на транспортные услуги";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Коммунальные услуги ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Оплата работ, услуг].[Коммунальные услуги], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля расходов на коммунальные услуги в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес расходов на коммунальные услуги";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Арендная плата за пользование имуществом ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Оплата работ, услуг].[Арендная плата за пользование имуществом], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля расходов на арендную плату за пользование имуществом в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес расходов\nна арендную плату за пользование имуществом";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Работы, услуги по содержанию имущества ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Оплата работ, услуг].[Работы, услуги по содержанию имущества], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля расходов на оплату работ, услуг по содержанию имущества в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес расходов на оплату работ,\nуслуг по содержанию имущества";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Прочие работы, услуги ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Оплата работ, услуг].[Прочие работы, услуги], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля расходов на оплату прочих услуг, работ в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес расходов\nна оплату прочих услуг, работ";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Обслуживание государственного (муниципального) долга ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Обслуживание государственного (муниципального) долга], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля расходов на обслуживание государственного (муниципального) долга в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес расходов на обслуживание\nгосударственного (муниципального) долга";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Обслуживание внутреннего долга ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Обслуживание государственного (муниципального) долга], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля расходов на обслуживание внутреннего долга в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес расходов а обслуживание внутреннего долга";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Безвозмездные перечисления организациям ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Безвозмездные перечисления организациям], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля безвозмездных перечислений организациям в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес безвозмездных перечислений организациям";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Безвозм. перечисления гос. и мун. организациям ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Безвозмездные перечисления организациям].[Безвозмездные перечисления государственным и муниципальным организациям], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля безвозмездных перечислений государственным и муниципальным организациям в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес безвозмездных перечислений\nгосударственным и муниципальным организациям";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Безвозм. перечисления организациям, за исключением гос. и мун. организаций ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Безвозмездные перечисления организациям].[Безвозмездные перечисления организациям, за исключением государственных и муниципальных организаций], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля безвозмездных перечислений организациям, за исключением государственных и муниципальных организаций, в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес безвозмездных перечислений организациям,\nза исключением государственных и муниципальных организаций";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Безвозмездные перечисления бюджетам ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Безвозмездные перечисления бюджетам], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля безвозмездных перечислений бюджетам в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес безвозмездных перечислений бюджетам";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Социальное обеспечение ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Социальное обеспечение], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля расходов на социальное обеспечение в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес расходов на социальное обеспечение";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Пособия по соц. помощи населению ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Социальное обеспечение].[Пособия по социальной помощи населению], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля расходов на пособия по социальной помощи населению в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес расходов на пособия\nпо социальной помощи населению";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Пенсии, пособия, выплачиваемые организациями сектора гос. управления ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Социальное обеспечение].[Пенсии, пособия, выплачиваемые организациями сектора государственного управления], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля расходов на пенсии, пособия, выплачиваемые организациями сектора государственного управления, в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес расходов на пенсии, пособия,\nвыплачиваемые организациями сектора государственного\nуправления";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Прочие расходы ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Расходы].[Прочие расходы], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля прочих расходов в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес прочих расходов";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Увеличение стоимости основных средств ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Поступление нефинансовых активов].[Увеличение стоимости основных средств], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля увеличения стоимости основных средств в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес увеличения стоимости основных средств";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Увеличение стоимости нематериальных активов ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Поступление нефинансовых активов].[Увеличение стоимости нематериальных активов], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля увеличения стоимости нематериальных активов в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес увеличения стоимости нематериальных активов";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }
                case "Увеличение стоимости материальных запасов ":
                    {
                        gridQueryName = "FO_0002_0014_grid_outcomes";
                        shareMeasure.Value = ", [Measures].[Удельный вес]";
                        filterIndicator.Value = string.Format("{1}.[Все коды ЭКР].[Поступление нефинансовых активов].[Увеличение стоимости материальных запасов], {0}.[FKRSlicer]",
                            UserParams.FKRDimension.Value, UserParams.EKRDimension.Value);
                        mapElementCaption1.Text = "Доля увеличения стоимости материальных запасов в расходах муниципальных образований";
                        mapColumnIndex1 = 8;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.White;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Удельный вес увеличения стоимости материальных запасов";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        break;
                    }

                case "Результат исполнения бюджета (дефицит/профицит) ":
                    {
                        gridQueryName = "FO_0002_0014_grid_deficit";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Вариант].[МесОтч].[Все варианты]";
                        mapElementCaption1.Text = "Результат исполнения бюджетов муниципальных образований";
                        mapColumnIndex1 = 4;
                        beginMapColor = Color.SandyBrown;
                        endMapColor = Color.GreenYellow;
                        nullEmptyColor = Color.White;
                        mapFormatString = "N2";
                        mapIndicatorName1 = String.Format("Результат исполнения бюджетов, {0}", RubMultiplierCaption.ToLower());
                        coloringRule = ColoringRule.PositiveNegativeColoring;
                        positiveValueText = "Профицит";
                        negativeValueText = "Дефицит";
                        isDeficitSelected = true;
                        break;
                    }
                case "Кредиты кредитных организаций ":
                    {
                        twoMapVisible = true;

                        gridQueryName = "FO_0002_0014_grid_credit";
                        shareMeasure.Value = "";
                        filterIndicator.Value = @"[КИФ].[Сопоставимый].[Все источники финансирования].[ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ].[Кредиты кредитных организаций в валюте Российской Федерации].[Получение кредитов от кредитных организаций в валюте Российской Федерации],
[КИФ].[Сопоставимый].[Все источники финансирования].[ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ].[Кредиты кредитных организаций в валюте Российской Федерации].[Погашение кредитов, предоставленных кредитными организациями в валюте Российской Федерации]";
                        mapElementCaption1.Text = "Получение кредитов от кредитных организаций в валюте РФ";
                        mapElementCaption2.Text = "Погашение кредитов от кредитных организаций в валюте РФ";
                        mapColumnIndex1 = 4;
                        mapColumnIndex2 = 11;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "N2";
                        mapIndicatorName1 = String.Format("Получение кредитов \nот кредитных организаций,\n{0}", RubMultiplierCaption.ToLower());
                        mapIndicatorName2 = String.Format("Погашение кредитов \nот кредитных организаций,\n{0}", RubMultiplierCaption.ToLower());
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Кредиты отсутствуют";
                        break;
                    }
                case "Бюджетные кредиты от других бюджетов бюджетной системы ":
                    {
                        twoMapVisible = true;

                        gridQueryName = "FO_0002_0014_grid_credit";
                        shareMeasure.Value = "";
                        filterIndicator.Value = @"[КИФ].[Сопоставимый].[Все источники финансирования].[ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ].[Бюджетные кредиты от других бюджетов бюджетной системы Российской Федерации].[Получение бюджетных кредитов от других бюджетов бюджетной системы Российской Федерации в валюте Российской Федерации],
[КИФ].[Сопоставимый].[Все источники финансирования].[ИСТОЧНИКИ ВНУТРЕННЕГО ФИНАНСИРОВАНИЯ ДЕФИЦИТОВ БЮДЖЕТОВ].[Бюджетные кредиты от других бюджетов бюджетной системы Российской Федерации].[Погашение бюджетных кредитов, полученных от других бюджетов бюджетной системы Российской Федерации в валюте Российской Федерации]";
                        mapElementCaption1.Text = "Получение бюджетных кредитов от других бюджетов";
                        mapElementCaption2.Text = "Погашение бюджетных кредитов, полученных от других бюджетов";
                        mapColumnIndex1 = 4;
                        mapColumnIndex2 = 11;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "N2";
                        mapIndicatorName1 = String.Format("Получение кредитов \nот других бюджетов, {0}", RubMultiplierCaption.ToLower());
                        mapIndicatorName2 = String.Format("Погашение кредитов \nот других бюджетов, {0}", RubMultiplierCaption.ToLower());
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Кредиты отсутствуют";
                        break;
                    }
                case "Муниципальный долг ":
                    {
                        gridQueryName = "FO_0002_0014_grid_innerDebtsMO";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОСпВнтД].[Все показатели].[МУНИЦИПАЛЬНЫЙ ДОЛГ, всего]";
                        mapElementCaption1.Text = "Долговые обязательства бюджетов муниципальных образований";
                        mapColumnIndex1 = 4;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "N2";
                        mapIndicatorName1 = String.Format("Долговые обязательства, {0}", RubMultiplierCaption.ToLower());
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Муниципальный долг отсутствует";
                        mapSimpleColoringUnits = "руб.";
                        isInnerDebtsMOSelected = true;
                        break;
                    }
                case "Остатки бюджетных средств ":
                    {
                        gridQueryName = "FO_0002_0014_grid_budgetarySurpluses";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОтСпОст].[Все показатели].[ОСТАТКИ СРЕДСТВ БЮДЖЕТОВ НА ОТЧЕТНУЮ ДАТУ:]";
                        mapElementCaption1.Text = "Темп роста остатков бюджетных\nсредств муниципальных образований";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста остатков";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Остатки отсутствуют";
                        isBudgetarySurplusesSelected = true;
                        break;
                    }
                case "имеющих целевое назначение ":
                    {
                        gridQueryName = "FO_0002_0014_grid_budgetarySurpluses";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОтСпОст].[Все показатели].[ОСТАТКИ СРЕДСТВ БЮДЖЕТОВ НА ОТЧЕТНУЮ ДАТУ:].[остатки целевых средств бюджетов]";
                        mapElementCaption1.Text = "Темп роста остатков целевых средств\nв бюджетах муниципальных образований";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста остатков целевых средств";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Остатки целевых средств отсутствуют";
                        isBudgetarySurplusesSelected = true;
                        break;
                    }
                case "имеющие нецелевое назначение ":
                    {
                        gridQueryName = "FO_0002_0014_grid_budgetarySurpluses";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОтСпОст].[остатки нецелевых средств бюджетов]";
                        mapElementCaption1.Text = "Темп роста остатков нецелевых средств в бюджетах муниципальных образований";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста остатков нецелевых средств";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Остатки нецелевых средств отсутствуют";
                        isBudgetarySurplusesSelected = true;
                        break;
                    }
                case "Просроченная кредиторская задолженность ":
                    {
                        gridQueryName = "FO_0002_0014_grid_creditDebts";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОСпЗадо].[Все показатели].[ПРОСРОЧЕННАЯ КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, всего]";
                        mapElementCaption1.Text = "Темп роста просроченной кредиторской задолженности бюджетов муниципальных образований";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста просроченной кредиторской задолженности";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Просроченная кредиторская задолженность отсутствует";
                        isCreditDebtsSelected = true;
                        break;
                    }
                case "по заработной плате ":
                    {
                        gridQueryName = "FO_0002_0014_grid_creditDebts";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОСпЗадо].[Все показатели].[ПРОСРОЧЕННАЯ КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, всего].[по заработной плате]";
                        mapElementCaption1.Text = "Темп роста просроченной кредиторской задолженности по заработной плате";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста просроченной кредиторской задолженности \nпо заработной плате";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Просроченная кредиторская задолженность отсутствует";
                        isCreditDebtsSelected = true;
                        break;
                    }
                case "по начислениям на выплаты по оплате труда ":
                    {
                        gridQueryName = "FO_0002_0014_grid_creditDebts";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОСпЗадо].[Все показатели].[ПРОСРОЧЕННАЯ КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, всего].[по начислениям на выплаты по  оплате труда]";
                        mapElementCaption1.Text = "Темп роста просроченной кредиторской задолженности по начислениям на выплаты по оплате труда";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста просроченной кредиторской задолженности \nпо начислениям на выплаты по оплате труда";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Просроченная кредиторская задолженность отсутствует";
                        isCreditDebtsSelected = true;
                        break;
                    }
                case "по оплате коммунальных услуг ":
                    {
                        gridQueryName = "FO_0002_0014_grid_creditDebts";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОСпЗадо].[Все показатели].[ПРОСРОЧЕННАЯ КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, всего].[по коммунальным услугам]";
                        mapElementCaption1.Text = "Темп роста просроченной кредиторской задолженности по оплате коммунальных услуг";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста просроченной кредиторской задолженности \nпо оплате коммунальных услуг";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Просроченная кредиторская задолженность отсутствует";
                        isCreditDebtsSelected = true;
                        break;
                    }
                case "по услугам связи ":
                    {
                        gridQueryName = "FO_0002_0014_grid_creditDebts";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОСпЗадо].[Все показатели].[ПРОСРОЧЕННАЯ КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, всего].[по услугам связи]";
                        mapElementCaption1.Text = "Темп роста просроченной кредиторской задолженности по услугам связи";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста просроченной кредиторской задолженности \nпо услугам связи";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Просроченная кредиторская задолженность отсутствует";
                        isCreditDebtsSelected = true;
                        break;
                    }
                case "по транспортным услугам ":
                    {
                        gridQueryName = "FO_0002_0014_grid_creditDebts";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОСпЗадо].[Все показатели].[ПРОСРОЧЕННАЯ КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, всего].[по транспортным услугам]";
                        mapElementCaption1.Text = "Темп роста просроченной кредиторской задолженности по транспортным услугам";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста просроченной кредиторской задолженности \nпо транспортным услугами";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Просроченная кредиторская задолженность отсутствует";
                        isCreditDebtsSelected = true;
                        break;
                    }
                case "по работам, услугам по содержанию имущества ":
                    {
                        gridQueryName = "FO_0002_0014_grid_creditDebts";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОСпЗадо].[Все показатели].[ПРОСРОЧЕННАЯ КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, всего].[по работам, услугам по содержанию имущества]";
                        mapElementCaption1.Text = "Темп роста просроченной кредиторской задолженности по работам, услугам по содержанию имущества";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста просроченной кредиторской задолженности \nпо работам, услугам по содержанию имущества";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Просроченная кредиторская задолженность отсутствует";
                        isCreditDebtsSelected = true;
                        break;
                    }
                case "по прочим работам, услугам ":
                    {
                        gridQueryName = "FO_0002_0014_grid_creditDebts";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОСпЗадо].[Все показатели].[ПРОСРОЧЕННАЯ КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, всего].[по прочим работам, услугам]";
                        mapElementCaption1.Text = "Темп роста просроченной кредиторской задолженности по прочим работам, услугама";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста просроченной кредиторской задолженности \nпо прочим работам, услугам";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Просроченная кредиторская задолженность отсутствует";
                        isCreditDebtsSelected = true;
                        break;
                    }
                case "по безвозм. перечислениям гос. и мун-м организациям ":
                    {
                        gridQueryName = "FO_0002_0014_grid_creditDebts";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОСпЗадо].[Все показатели].[ПРОСРОЧЕННАЯ КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, всего].[по безвозмездным перечислениям государственным и муниципальным организациям]";
                        mapElementCaption1.Text = "Темп роста просроченной кредиторской задолженности по безвозмездным перечислениям гос. и мун-м организациям";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста просроченной кредиторской задолженности \nпо безвозмездным перечислениям гос. и мун-м организациям";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Просроченная кредиторская задолженность отсутствует";
                        isCreditDebtsSelected = true;
                        break;
                    }
                case "по безвозм. перечислениям организациям, за исключением гос. и мун. организаций ":
                    {
                        gridQueryName = "FO_0002_0014_grid_creditDebts";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОСпЗадо].[Все показатели].[ПРОСРОЧЕННАЯ КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, всего].[по безвозмездным перечислениям  организациям, за исключением государственных и муниципальных организаций]";
                        mapElementCaption1.Text = "Темп роста просроченной кредиторской задолженности по безвозмездным перечислениям организациям, за исключением гос. и мун. организаций";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста просроченной кредиторской задолженности\nпо безвозм.перечислениям организациям, за исключением\nгос. и мун. организаций";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Просроченная кредиторская задолженность отсутствует";
                        isCreditDebtsSelected = true;
                        break;
                    }
                case "по пособиям по соц. помощи населению ":
                    {
                        gridQueryName = "FO_0002_0014_grid_creditDebts";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОСпЗадо].[Все показатели].[ПРОСРОЧЕННАЯ КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, всего].[по пособиям по социальной помощи населению]";
                        mapElementCaption1.Text = "Темп роста просроченной кредиторской задолженности по пособиям по соц. помощи населению";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста просроченной кредиторской задолженности \nпо пособиям по соц. помощи населению";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Просроченная кредиторская задолженность отсутствует";
                        isCreditDebtsSelected = true;
                        break;
                    }
                case "по прочим расходам ":
                    {
                        gridQueryName = "FO_0002_0014_grid_creditDebts";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОСпЗадо].[Все показатели].[ПРОСРОЧЕННАЯ КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, всего].[по прочим расходам]";
                        mapElementCaption1.Text = "Темп роста просроченной кредиторской задолженности по прочим расходам";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста просроченной кредиторской задолженности \nпо прочим расходам";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Просроченная кредиторская задолженность отсутствует";
                        isCreditDebtsSelected = true;
                        break;
                    }
                case "по оплате договоров на приобретение объектов, относящихся к основным средствам ":
                    {
                        gridQueryName = "FO_0002_0014_grid_creditDebts";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОСпЗадо].[Все показатели].[ПРОСРОЧЕННАЯ КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, всего].[по оплате договоров на приобретение, строительство, реконструкцию, техническое перевооружение, расширение и модернизацию объектов, относящихся к основным средствам]";
                        mapElementCaption1.Text = "Темп роста просроченной кредиторской задолженности по оплате договоров на приобретение объектов, относящихся к основным средствам";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста просроченной кредиторской задолженности \nпо оплате договоров на приобретение объектов,\nотносящихся к основным средствам";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Просроченная кредиторская задолженность отсутствует";
                        isCreditDebtsSelected = true;
                        break;
                    }
                case "по оплате договоров на приобретение сырья и материалов на оказание гос. (мун-х) услуг ":
                    {
                        gridQueryName = "FO_0002_0014_grid_creditDebts";
                        shareMeasure.Value = "";
                        filterIndicator.Value = "[Показатели].[СопМесОСпЗадо].[Все показатели].[ПРОСРОЧЕННАЯ КРЕДИТОРСКАЯ  ЗАДОЛЖЕННОСТЬ, всего].[по оплате договоров на приобретение сырья и материалов в целях оказания государственных (муниципальных) услуг]";
                        mapElementCaption1.Text = "Темп роста просроченной кредиторской задолженности по оплате договоров на приобретение сырья и материалов на оказание гос. (мун-х) услуг";
                        mapColumnIndex1 = 3;
                        beginMapColor = Color.Green;
                        endMapColor = Color.Red;
                        nullEmptyColor = Color.LightSkyBlue;
                        mapFormatString = "P2";
                        mapIndicatorName1 = "Темп роста просроченной кредиторской задолженности \nпо оплате договоров на приобретение\nсырья и материалов на оказание гос. (мун-х) услуг";
                        coloringRule = ColoringRule.SimpleRangeColoring;
                        nullEmptyText = "Просроченная кредиторская задолженность отсутствует";
                        isCreditDebtsSelected = true;
                        break;
                    }
            }
        }

        #region Обработчики карты

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

        public void SetMapSettings(MapControl map, int mapIndex)
        {
            map.Meridians.Visible = false;
            map.Parallels.Visible = false;
            map.ZoomPanel.Visible = true;
            map.ZoomPanel.Dock = PanelDockStyle.Left;
            map.NavigationPanel.Visible = true;
            map.NavigationPanel.Dock = PanelDockStyle.Left;
            map.Viewport.EnablePanning = true;
            map.Viewport.Zoom = (float)mapZoomValue;

            map.Viewport.LocationUnit = CoordinateUnit.Pixel;
            map.Viewport.Location.Y = -100;

            map2Table.Visible = twoMapVisible;
            map.Width = !twoMapVisible ? CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15) : CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20) / 2;
            map.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.8);


            // добавляем легенду раскраски
            map.Legends.Clear();
            Legend legend = new Legend("IndicatorLegend");
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
            legend.Title = (mapIndex == 1) ? mapIndicatorName1 : mapIndicatorName2;
            legend.AutoFitMinFontSize = 7;

            SetLegendPosition(legend, mapLegendPositon);

            map.Legends.Add(legend);

            if (useRegionCodes)
            {
                Legend regionsLegend = new Legend("RegionCodeLegend");
                regionsLegend.Dock = PanelDockStyle.Right;
                regionsLegend.Visible = true;
                regionsLegend.BackColor = Color.White;
                regionsLegend.BackSecondaryColor = Color.Gainsboro;
                regionsLegend.BackGradientType = GradientType.DiagonalLeft;
                regionsLegend.BackHatchStyle = MapHatchStyle.None;
                regionsLegend.BorderColor = Color.Gray;
                regionsLegend.BorderWidth = 1;
                regionsLegend.BorderStyle = MapDashStyle.Solid;
                regionsLegend.BackShadowOffset = 4;
                regionsLegend.TextColor = Color.Black;
                regionsLegend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
                regionsLegend.AutoFitText = true;
                regionsLegend.Title = "Коды территорий";
                regionsLegend.AutoFitMinFontSize = 7;
                regionsLegend.ItemColumnSpacing = 100;
                map.Legends.Add(regionsLegend);
            }

            map.ShapeRules.Clear();
            map.ShapeFields.Clear();
            map.ShapeFields.Add("Name");
            map.ShapeFields["Name"].Type = typeof(string);
            map.ShapeFields["Name"].UniqueIdentifier = true;
            map.ShapeFields.Add("Indicator");
            map.ShapeFields["Indicator"].Type = typeof(double);
            map.ShapeFields["Indicator"].UniqueIdentifier = false;

            if (coloringRule == ColoringRule.SimpleRangeColoring)
            {
                // добавляем правила раскраски
                ShapeRule rule = new ShapeRule();
                rule.Name = "IndicatorRule";
                rule.Category = String.Empty;
                rule.ShapeField = "Indicator";
                rule.DataGrouping = DataGrouping.Optimal;
                rule.ColorCount = 5;
                rule.ColoringMode = ColoringMode.ColorRange;
                rule.FromColor = beginMapColor;
                rule.MiddleColor = Color.Yellow;
                rule.ToColor = endMapColor;
                rule.BorderColor = Color.FromArgb(50, Color.Black);
                rule.GradientType = GradientType.None;
                rule.HatchStyle = MapHatchStyle.None;
                rule.ShowInColorSwatch = false;
                rule.ShowInLegend = "IndicatorLegend";
                rule.LegendText = string.Format("#FROMVALUE{{{0}}} - #TOVALUE{{{0}}}", mapFormatString);
                map.ShapeRules.Add(rule);
            }
            else if (coloringRule == ColoringRule.ComplexRangeColoring)
            {
                LegendItem item = new LegendItem();
                item.Text = "Менее 5%";
                item.Color = Color.Green;
                legend.Items.Add(item);

                item = new LegendItem();
                item.Text = "5% - 15%";
                item.Color = Color.YellowGreen;
                legend.Items.Add(item);

                item = new LegendItem();
                item.Text = "15% - 30%";
                item.Color = Color.Yellow;
                legend.Items.Add(item);

                item = new LegendItem();
                item.Text = "30% - 50%";
                item.Color = Color.Orange;
                legend.Items.Add(item);

                item = new LegendItem();
                item.Text = "Более 50%";
                item.Color = Color.Red;
                legend.Items.Add(item);
            }
            else if (coloringRule == ColoringRule.PositiveNegativeColoring)
            {
                LegendItem item = new LegendItem();
                item.Text = positiveValueText;
                item.Color = endMapColor;
                legend.Items.Add(item);

                item = new LegendItem();
                item.Text = negativeValueText;
                item.Color = beginMapColor;
                legend.Items.Add(item);
            }

            if (nullEmptyColor != Color.White)
            {
                LegendItem item = new LegendItem();
                item.Text = nullEmptyText;
                item.Color = nullEmptyColor;
                legend.Items.Add(item);
            }
        }

        public void FillMapData(MapControl map, int mapIndex)
        {
            if (dtGrid == null || dtGrid.Rows.Count > 0 && map == null)
            {
                return;
            }

            foreach (Shape shape in map.Shapes)
            {
                string shapeName = GetShapeName(shape);
                if (!RegionsNamingHelper.LocalBudgetUniqueNames.ContainsKey(shapeName))
                {
                    if (nullShapeHidden)
                    {
                        shape.Visible = false;
                    }
                    else
                    {
                        shape.TextVisibility = TextVisibility.Hidden;
                    }
                }
            }

            int mapColumnIndex = mapIndex == 1 ? mapColumnIndex1 : mapColumnIndex2;
            string indicatorName = mapIndex == 1 ? mapIndicatorName1 : mapIndicatorName2;

            if (!NonNullValueGrid(dtGrid, mapColumnIndex) && map.ShapeRules.Count != 0)
            {
                map.ShapeRules[0].ShowInLegend = "";
            }

            if (coloringRule == ColoringRule.SimpleRangeColoring)
            {
                int colorCount = CRHelper.GetMapIntervalCount(dtGrid, mapColumnIndex, 5, false);
                switch (colorCount)
                {
                    case -1:
                        {
                            // совсем убираем раскраску из легенды
                            map.ShapeRules["IndicatorRule"].ShowInLegend = string.Empty;
                            break;
                        }
                    case 0:
                        {
                            // значение только одно
                            map.ShapeRules["IndicatorRule"].ColorCount = 1;
                            map.ShapeRules["IndicatorRule"].LegendText = string.Format("#FROMVALUE{{{0}}}", mapFormatString);
                            break;
                        }
                    default:
                        {
                            // значений несколько
                            map.ShapeRules["IndicatorRule"].ColorCount = colorCount;
                            break;
                        }
                }
            }

            foreach (DataRow row in dtGrid.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                {
                    string subject = row[0].ToString();

                    ArrayList shapeList = FindMapShape(map, subject);

                    foreach (Shape shape in shapeList)
                    {
                        string shapeName = GetShapeName(shape);
                        string shapeCode = string.Empty;

                        if (useRegionCodes && !IsCalloutTownShape(shape) &&
                            RegionsNamingHelper.LocalBudgetCodes.ContainsKey(shapeName))
                        {
                            shapeCode = RegionsNamingHelper.LocalBudgetCodes[shapeName];

                            LegendItem item = new LegendItem();
                            LegendCell cell = new LegendCell(shapeCode);
                            cell.Alignment = System.Drawing.ContentAlignment.MiddleRight;
                            item.Cells.Add(cell);
                            cell = new LegendCell(shapeName);
                            cell.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
                            item.Cells.Add(cell);

                            map.Legends["RegionCodeLegend"].Items.Add(item);
                        }

                        shape["Name"] = subject;
                        shape.BorderColor = Color.Gray;
                        shape.BorderWidth = 1;
                        string valueStr = string.Empty;
                        if (row.ItemArray.Length > mapColumnIndex && row[mapColumnIndex] != DBNull.Value && row[mapColumnIndex].ToString() != string.Empty)
                        {
                            double value = Convert.ToDouble(row[mapColumnIndex]);
                            if ((ComboIndicator.SelectedValue == "Кредиты кредитных организаций " ||
                                ComboIndicator.SelectedValue == "Бюджетные кредиты от других бюджетов бюджетной системы " ||
                                ComboIndicator.SelectedValue == "Муниципальный долг ") &&
                                value == 0)
                            {
                                shape.Color = nullEmptyColor;
                                shape.ToolTip = string.Format("{0}\n{1}", shapeName.Replace("\"", "&quot"), nullEmptyText);
                            }
                            else
                            {
                                valueStr = value.ToString(mapFormatString);
                                shape["Indicator"] = value;

                                double percentValue = value * 100;
                                if (coloringRule == ColoringRule.ComplexRangeColoring)
                                {
                                    if (percentValue < 5)
                                    {
                                        shape.Color = beginMapColor;
                                    }
                                    else if (percentValue > 50)
                                    {
                                        shape.Color = endMapColor;
                                    }
                                    else if (percentValue >= 5 && percentValue <= 15)
                                    {
                                        shape.Color = Color.YellowGreen;
                                    }
                                    else if (percentValue > 15 && percentValue <= 30)
                                    {
                                        shape.Color = Color.Yellow;
                                    }
                                    else if (percentValue > 30 && percentValue <= 50)
                                    {
                                        shape.Color = Color.Orange;
                                    }
                                }
                                else if (coloringRule == ColoringRule.PositiveNegativeColoring)
                                {
                                    if (percentValue < 0)
                                    {
                                        shape.Color = beginMapColor;
                                    }
                                    else if (percentValue > 0)
                                    {
                                        shape.Color = endMapColor;
                                    }
                                }
                                shape.ToolTip = string.Format("{0}\n{1}: #INDICATOR{{{2}}}", shapeName.Replace("\"", "&quot"), indicatorName,
                                                  mapFormatString);
                            }
                        }
                        else
                        {
                            shape.Color = nullEmptyColor;
                            shape.ToolTip = string.Format("{0}\n{1}", shapeName, nullEmptyText);
                        }

                        SetShapeMargin(shape);

                        if (IsCalloutTownShape(shape))
                        {
                            shape.Text = string.Format("{0}\n{1}", shapeName, valueStr);
                            shape.TextVisibility = TextVisibility.Shown;
                            shape.TextAlignment = System.Drawing.ContentAlignment.BottomCenter;
                            shape.CentralPointOffset.Y = calloutOffsetY;
                        }
                        else
                        {
                            string shapeText = useRegionCodes ? shapeCode : shapeName.Replace(" ", "\n");
                            shape.Text = string.Format("{0}\n{1}", shapeText, valueStr);
                            shape.TextVisibility = shapeTextShown ? TextVisibility.Shown : TextVisibility.Auto;
                        }
                    }
                }
            }
        }

        private static bool NonNullValueGrid(DataTable dt, int columnIndex)
        {
            if (dt != null && dt.Columns.Count > columnIndex)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row[0] != DBNull.Value && RegionsNamingHelper.LocalBudgetUniqueNames.ContainsKey(row[0].ToString()))
                    {
                        if (row[columnIndex] != DBNull.Value && Convert.ToDouble(row[columnIndex]) != 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }

        private LegendPosition GetLegendPositon(string positionName)
        {
            switch (positionName)
            {
                case "LeftTop":
                    {
                        return LegendPosition.LeftTop;
                    }
                case "LeftBottom":
                    {
                        return LegendPosition.LeftBottom;
                    }
                case "RightTop":
                    {
                        return LegendPosition.RightTop;
                    }
                case "RightBottom":
                    {
                        return LegendPosition.RightBottom;
                    }
            }
            return LegendPosition.LeftTop;
        }

        private void SetLegendPosition(Legend legend, LegendPosition position)
        {
            switch (position)
            {
                case LegendPosition.LeftTop:
                    {
                        legend.DockAlignment = DockAlignment.Near;
                        legend.Dock = PanelDockStyle.Left;
                        break;
                    }
                case LegendPosition.LeftBottom:
                    {
                        legend.DockAlignment = DockAlignment.Far;
                        legend.Dock = PanelDockStyle.Left;
                        break;
                    }
                case LegendPosition.RightTop:
                    {
                        legend.DockAlignment = DockAlignment.Near;
                        legend.Dock = PanelDockStyle.Right;
                        break;
                    }
                case LegendPosition.RightBottom:
                    {
                        legend.DockAlignment = DockAlignment.Far;
                        legend.Dock = PanelDockStyle.Right;
                        break;
                    }
            }
        }

        private void SetShapeMargin(Shape shape)
        {
            if (mapFolder == "Сахалин")
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

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(gridQueryName);
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Бюджет", dtGrid);
            if (dtGrid.Columns.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length; i = i + 1)
                    {
                        bool decimalColumn = GetColumnFormat(dtGrid.Columns[i].ColumnName.ToLower()) == "N2";
                        if (row[i] != null && row[i].ToString() != String.Empty)
                        {
                            if (decimalColumn)
                            {
                                row[i] = Convert.ToDouble(row[i]) / rubMultiplier;
                            }
                        }
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
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
                string formatString = GetColumnFormat(e.Layout.Bands[0].Columns[i].Header.Caption.ToLower());
                int widthColumn = 100;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            gridHeaderLayout.AddCell("Бюджет");

            GridHeaderCell consolidateCell = gridHeaderLayout.AddCell("Консолидированный бюджет муниципального района (городского округа)");
            GridHeaderCell regionCell = gridHeaderLayout.AddCell("Бюджет мун.района");
            GridHeaderCell settlemntCell = gridHeaderLayout.AddCell("Бюджеты поселений");

            GridHeaderCell groupCell = new GridHeaderCell();

            measuresCount = twoMapVisible ? dtGrid.Columns.Count / 6 : dtGrid.Columns.Count / 3;
            int groupCount = twoMapVisible ? 2 * measuresCount : measuresCount;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + groupCount)
            {
                int groupIndex = (i - 1) / groupCount;
                switch (groupIndex)
                {
                    case 0:
                        {
                            groupCell = consolidateCell;
                            break;
                        }
                    case 1:
                        {
                            groupCell = regionCell;
                            break;
                        }
                    case 2:
                        {
                            groupCell = settlemntCell;
                            break;
                        }
                }

                if (!twoMapVisible)
                {
                    AddColumnGroup(groupCell);
                }
                else
                {
                    GridHeaderCell cell1 = groupCell.AddCell(mapIndicatorName1);
                    GridHeaderCell cell2 = groupCell.AddCell(mapIndicatorName2);

                    AddColumnGroup(cell1);
                    AddColumnGroup(cell2);
                }
            }

            gridHeaderLayout.ApplyHeaderInfo();
        }

        private void AddColumnGroup(GridHeaderCell parentCell)
        {
            if (!isBudgetarySurplusesSelected && !isCreditDebtsSelected)
            {
                GridHeaderCell planCell = parentCell.AddCell("Назначено");
                planCell.AddCell(lastDateStr);
                planCell.AddCell(currDateStr);
            }

            GridHeaderCell factCell = parentCell.AddCell("Исполнено");
            factCell.AddCell(lastDateStr);
            factCell.AddCell(currDateStr);

            parentCell.AddCell("Темп роста к прошлому году");

            if (!isDeficitSelected && !isBudgetarySurplusesSelected && !isCreditDebtsSelected)
            {
                GridHeaderCell executePercentCell = parentCell.AddCell("Процент исполнения");
                executePercentCell.AddCell(lastDateStr);
                executePercentCell.AddCell(currDateStr);
            }

            if (measuresCount > 7)
            {
                parentCell.AddCell("Удельный вес");
            }
        }

        private static string GetColumnFormat(string columnName)
        {
            return columnName.Contains("темп роста") || columnName.Contains("удельный вес") || columnName.Contains("% выполнения") ? "P2" : "N2";
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rateGrow = e.Row.Band.Grid.Columns[i].Header.Caption.ToLower().Contains("темп роста");
                bool invertGrowIndicator = isInnerDebtsMOSelected || isCreditDebtsSelected;

                if (i == 0 && e.Row.Cells[i].Value != null)
                {
                    e.Row.Cells[i].Value = e.Row.Cells[i].Value.ToString().Replace("муниципальный район", "МР");
                }

                if (rateGrow)
                {
                    double lastValue = double.MinValue;
                    if (e.Row.Cells[i - 2].Value != null && e.Row.Cells[i - 2].Value.ToString() != String.Empty)
                    {
                        lastValue = Convert.ToDouble(e.Row.Cells[i - 2].Value);
                    }

                    double currValue = double.MinValue;
                    if (e.Row.Cells[i - 1].Value != null && e.Row.Cells[i - 1].Value.ToString() != String.Empty)
                    {
                        currValue = Convert.ToDouble(e.Row.Cells[i - 1].Value);
                    }

                    if (currValue > lastValue)
                    {
                        e.Row.Cells[i - 1].Style.BackgroundImage = !invertGrowIndicator ? "~/images/arrowGreenUpBB.png" : "~/images/arrowRedUpBB.png";
                        e.Row.Cells[i - 1].Title = "Рост к прошлому году";
                    }
                    else if (currValue < lastValue)
                    {
                        e.Row.Cells[i - 1].Style.BackgroundImage = !invertGrowIndicator ? "~/images/arrowRedDownBB.png" : "~/images/arrowGreenDownBB.png";
                        e.Row.Cells[i - 1].Title = "Снижение к прошлому году";
                    }

                    e.Row.Cells[i - 1].Style.Padding.Right = 2;
                    e.Row.Cells[i - 1].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
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

                if (e.Row.Cells[0].Value != null &&
                    (
                       e.Row.Cells[0].Value.ToString().ToLower().Contains("бюджет") ||
                       e.Row.Cells[0].Value.ToString().ToLower().Contains("городские округа") ||
                       e.Row.Cells[0].Value.ToString().ToLower().Contains("муниципальные районы")
                    ))
                {
                    foreach (UltraGridCell c in e.Row.Cells)
                    {
                        c.Style.Font.Bold = true;
                    }
                }
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (UltraWebGrid.Rows.Count < 30)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = mapElementCaption1.Text + string.Format(" за {0} {2} {1} года",
                        currentDate.Month, currentDate.Year,
                        CRHelper.RusManyMonthGenitive(currentDate.Month)); 
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            if (twoMapVisible)
            {
                Worksheet sheet1 = workbook.Worksheets.Add("Карта1");
                DundasMap1.Width = Unit.Pixel((int)DundasMap1.Width.Value * 2);
                ReportExcelExporter1.Export(DundasMap1, mapElementCaption1.Text, sheet1, 3);

                Worksheet sheet3 = workbook.Worksheets.Add("Карта2");
                DundasMap2.Width = Unit.Pixel((int)DundasMap2.Width.Value * 2);
                ReportExcelExporter1.Export(DundasMap2, mapElementCaption2.Text, sheet3, 3);
            }
            else
            {
                Worksheet sheet1 = workbook.Worksheets.Add("Карта");
                ReportExcelExporter1.Export(DundasMap1, PageSubTitle.Text, sheet1, 3);
            }

            Worksheet sheet2 = workbook.Worksheets.Add("Таблица");
            sheet2.Rows[1].Cells[0].Value = mapElementCaption1.Text + string.Format(" за {0} {2} {1} года",
                    currentDate.Month, currentDate.Year,
                    CRHelper.RusManyMonthGenitive(currentDate.Month));
            ReportExcelExporter1.Export(gridHeaderLayout, sheet2, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = mapElementCaption1.Text + string.Format(" за {0} {2} {1} года",
                        currentDate.Month, currentDate.Year,
                        CRHelper.RusManyMonthGenitive(currentDate.Month));
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
           
            section1.PageMargins.Top = 35;
            section1.PageMargins.Left = 30;
            IText title = section1.AddText();
            Font font = new Font("Verdana", 16);

            if (twoMapVisible)
            {
                DundasMap1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
                //DundasMap1.Width = Unit.Pixel((int)DundasMap1.Width.Value * 2);
                ReportPDFExporter1.Export(DundasMap1, mapElementCaption1.Text, section1);

                ISection section3 = report.AddSection();
                DundasMap2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
                //DundasMap2.Width = Unit.Pixel((int)DundasMap2.Width.Value * 2);
                ReportPDFExporter1.Export(DundasMap2, mapElementCaption2.Text, section3);
            }
            else
            {
                //DundasMap1.Legends["IndicatorLegend"].Dock = PanelDockStyle.Bottom;
                //DundasMap1.Legends["IndicatorLegend"].DockAlignment = DockAlignment.Far;
                // DundasMap1.Legends["RegionCodeLegend"].Dock = PanelDockStyle.Bottom;              
                font = new Font("Verdana", 16);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.Style.Font.Bold = true;
                title.AddContent(PageTitle.Text);


                title = section1.AddText();
                font = new Font("Verdana", 14);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent(PageSubTitle.Text);

                section1.PageOrientation = PageOrientation.Landscape;
                DundasMap1.NavigationPanel.Visible = false;
                DundasMap1.ZoomPanel.Visible = false;
                MemoryStream imageStream = new MemoryStream();
                DundasMap1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 1));
                DundasMap1.SaveAsImage(imageStream, MapImageFormat.Png);
                Infragistics.Documents.Reports.Graphics.Image image = (new Bitmap(imageStream)).ScaleImageIg(1);


                section1.AddImage(image);
                //DundasMap1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 1));
                //ReportPDFExporter1.Export(image, mapElementCaption1.Text, section1);
            }

            ISection section2 = report.AddSection();
            //title = section2.AddText();
            //font = new Font("Verdana", 16);
            //title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            //title.Style.Font.Bold = true;
            //title.AddContent(PageTitle.Text);

            //title = section2.AddText();
            //font = new Font("Verdana", 14);
            //title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            //title.AddContent(mapElementCaption1.Text + string.Format(" за {0} {2} {1} года",
            //        currentDate.Month, currentDate.Year,
            //        CRHelper.RusManyMonthGenitive(currentDate.Month)));
            ReportPDFExporter1.Export(gridHeaderLayout, section2);
        }

        #endregion
    }
}
