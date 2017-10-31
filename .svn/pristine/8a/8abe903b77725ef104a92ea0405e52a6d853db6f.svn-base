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
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Image=Infragistics.Documents.Reports.Graphics.Image;

namespace Krista.FM.Server.Dashboards.reports.STAT_0001_0002
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private DataTable dtDebtsGrid;
        private DataTable dtCommentText;
        private DataTable dtMap1;
        private DataTable dtMap2;
        private DataTable dtKoeff;
        private DateTime currDateTime;
        private DateTime lastDateTime;
        private DateTime debtsCurrDateTime;
        private DateTime debtsLastDateTime;
        private DateTime redundantLevelRFDateTime;

        private DateTime lastPrevDateTime;
        private DateTime debtLastPrevDateTime;

        private bool hasRfRedudantLevel = true;

        public bool IsYearJoint()
        {
            return (currDateTime.Year != lastDateTime.Year);
        }

        #endregion

        #region Параметры запроса

        // Текущая дата
        private CustomParam periodCurrentDate;
        // На неделю назад
        private CustomParam periodLastWeekDate;
        // На неделю назад
        private CustomParam periodPrevLastWeekDate;

        // Текущий год
        private CustomParam сurrentYear;
        // Прошлый год
        private CustomParam lastYear;

        // Текущая дата для задолженности
        private CustomParam debtsPeriodCurrentDate;
        // На неделю назад для задолженности
        private CustomParam debtsPeriodLastWeekDate;
        // На неделю назад для задолженности
        private CustomParam debtsPeriodPrevLastWeekDate;

        // Текущая дата для уровня безработицы по РФ
        private CustomParam redundantLevelRFDate;

        #endregion
        
        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }

        private int MinScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private int MinScreenHeight
        {
            get { return IsSmallResolution ? 600 : CustomReportConst.minScreenHeight; }
        }
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(MinScreenWidth - 12);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(MinScreenHeight - 120);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraWebGrid2.Width = CRHelper.GetGridWidth(MinScreenWidth - 12);
            UltraWebGrid2.Height = CRHelper.GetGridHeight(MinScreenHeight - 120);
            UltraWebGrid2.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid2.DataBound += new EventHandler(UltraWebGrid_DataBound);

            DundasMap1.Width = CRHelper.GetChartWidth(MinScreenWidth - 15);
            DundasMap1.Height = CRHelper.GetChartHeight(MinScreenHeight * 0.85);

            DundasMap2.Width = CRHelper.GetChartWidth(MinScreenWidth - 15);
            DundasMap2.Height = CRHelper.GetChartHeight(MinScreenHeight * 0.85);

            DundasMap2.PostPaint += new MapPaintEvent(DundasMap2_PostPaint);

            #region Инициализация параметров запроса

            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (periodLastWeekDate == null)
            {
                periodLastWeekDate = UserParams.CustomParam("period_last_week_date");
            }
            if (periodPrevLastWeekDate == null)
            {
                periodPrevLastWeekDate = UserParams.CustomParam("period_prev_last_week_date");
            }
            if (debtsPeriodCurrentDate == null)
            {
                debtsPeriodCurrentDate = UserParams.CustomParam("period_current_date_debts");
            }
            if (debtsPeriodLastWeekDate == null)
            {
                debtsPeriodLastWeekDate = UserParams.CustomParam("period_last_week_date_debts");
            }
            if (debtsPeriodPrevLastWeekDate == null)
            {
                debtsPeriodPrevLastWeekDate = UserParams.CustomParam("period_prev_last_week_date_debts");
            }
            if (сurrentYear == null)
            {
                сurrentYear = UserParams.CustomParam("current_year");
            }
            if (lastYear == null)
            {
                lastYear = UserParams.CustomParam("last_year");
            }
            if (redundantLevelRFDate == null)
            {
                redundantLevelRFDate = UserParams.CustomParam("redundant_level_RF_date");
            }

            #endregion

            #region Настройка карты 1

            DundasMap1.Meridians.Visible = false;
            DundasMap1.Parallels.Visible = false;
            DundasMap1.ZoomPanel.Visible = !IsSmallResolution;
            DundasMap1.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap1.NavigationPanel.Visible = !IsSmallResolution;
            DundasMap1.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap1.Viewport.EnablePanning = true;

            // добавляем легенду
            DundasMap1.Legends.Clear();
            // добавляем легенду с символами
            Legend legend2 = new Legend("SymbolLegend");
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
            legend2.Title = IsSmallResolution ? "Число безработных" : "Общая численность зарегистрированных безработных граждан";
            legend2.AutoFitMinFontSize = 7;
            DundasMap1.Legends.Add(legend2);

            // добавляем легенду раскраски
            Legend legend1 = new Legend("CompleteLegend");
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
            legend1.Title = IsSmallResolution ? "Уровень безработицы" : "Уровень регистрируемой безработицы";
            legend1.AutoFitMinFontSize = 7;
            DundasMap1.Legends.Add(legend1);

            // добавляем поля для раскраски
            DundasMap1.ShapeFields.Clear();
            DundasMap1.ShapeFields.Add("Name");
            DundasMap1.ShapeFields["Name"].Type = typeof(string);
            DundasMap1.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap1.ShapeFields.Add("UnemploymentLevel");
            DundasMap1.ShapeFields["UnemploymentLevel"].Type = typeof(double);
            DundasMap1.ShapeFields["UnemploymentLevel"].UniqueIdentifier = false;

            // добавляем поля для символов
            DundasMap1.SymbolFields.Add("UnemploymentPopulation");
            DundasMap1.SymbolFields["UnemploymentPopulation"].Type = typeof(double);
            DundasMap1.SymbolFields["UnemploymentPopulation"].UniqueIdentifier = false;

            // добавляем правила раскраски
            DundasMap1.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "UnemploymentLevel";
            rule.DataGrouping = DataGrouping.EqualInterval;
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
            rule.LegendText = "#FROMVALUE{P1} - #TOVALUE{P1}";

            DundasMap1.ShapeRules.Add(rule);

            // добавляем правила расстановки символов
            DundasMap1.SymbolRules.Clear();
            SymbolRule symbolRule = new SymbolRule();
            symbolRule.Name = "SymbolRule";
            symbolRule.Category = string.Empty;
            symbolRule.DataGrouping = DataGrouping.EqualInterval;
            symbolRule.SymbolField = "UnemploymentPopulation";
            symbolRule.ShowInLegend = "SymbolLegend";
            DundasMap1.SymbolRules.Add(symbolRule);

            // звезды для легенды
            for (int i = 1; i < 4; i++)
            {
                PredefinedSymbol predefined = new PredefinedSymbol();
                predefined.Name = "PredefinedSymbol" + i;
                predefined.MarkerStyle = MarkerStyle.Star;
                predefined.Width = 5 + (i * 5);
                predefined.Height = predefined.Width;
                predefined.Color = Color.DarkViolet;
                DundasMap1.SymbolRules["SymbolRule"].PredefinedSymbols.Add(predefined);
            }

            #endregion

            #region Настройка карты 2

            DundasMap2.Meridians.Visible = false;
            DundasMap2.Parallels.Visible = false;
            DundasMap2.ZoomPanel.Visible = !IsSmallResolution;
            DundasMap2.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap2.NavigationPanel.Visible = !IsSmallResolution;
            DundasMap2.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap2.Viewport.EnablePanning = true;

            // добавляем легенду
            DundasMap2.Legends.Clear();

            // добавляем легенду раскраски
            legend1 = new Legend("TensionLegend");
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
            legend1.Title = IsSmallResolution ? "Число безработных на 1 вакансию" : "Число зарегистрированных безработных в расчёте на 1 вакансию";
            legend1.AutoFitMinFontSize = 7;
            DundasMap2.Legends.Add(legend1);

            // добавляем легенду с символами
            legend2 = new Legend("VacancyLegend");
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
            legend2.Title = IsSmallResolution ? "" : "Соотношение числа безработных и числа вакансий";
            legend2.AutoFitMinFontSize = 7;
            DundasMap2.Legends.Add(legend2);

            // добавляем поля для раскраски
            DundasMap2.ShapeFields.Clear();
            DundasMap2.ShapeFields.Add("Name");
            DundasMap2.ShapeFields["Name"].Type = typeof(string);
            DundasMap2.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap2.ShapeFields.Add("TensionKoeff");
            DundasMap2.ShapeFields["TensionKoeff"].Type = typeof(double);
            DundasMap2.ShapeFields["TensionKoeff"].UniqueIdentifier = false;

            // добавляем поля для символов
            DundasMap2.SymbolFields.Add("VacancyCount");
            DundasMap2.SymbolFields["VacancyCount"].Type = typeof(double);
            DundasMap2.SymbolFields["VacancyCount"].UniqueIdentifier = false;
            DundasMap2.SymbolFields.Add("RedundantCount");
            DundasMap2.SymbolFields["RedundantCount"].Type = typeof(double);
            DundasMap2.SymbolFields["RedundantCount"].UniqueIdentifier = false;

            LegendItem item = new LegendItem();
            item.Text = "Число безработных";
            item.Color = Color.DarkViolet;
            legend2.Items.Add(item);

            item = new LegendItem();
            item.Text = "Число вакансий";
            item.Color = Color.Black;
            legend2.Items.Add(item);

            // добавляем правила раскраски
            DundasMap2.ShapeRules.Clear();
            rule = new ShapeRule();
            rule.Name = "TensionKoeffRule";
            rule.Category = String.Empty;
            rule.ShapeField = "TensionKoeff";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Green;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "TensionLegend";
            rule.LegendText = "#FROMVALUE{N0} - #TOVALUE{N0}";
            DundasMap2.ShapeRules.Add(rule);

            #endregion

            UltraGridExporter1.MultiHeader = !IsSmallResolution;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<EndExportEventArgs>(PdfExporter_EndExport);

            CrossLink1.Text = "Динамика&nbsp;показателей";
            CrossLink1.NavigateUrl = "~/reports/STAT_0001_0004/Default.aspx";
            CrossLink2.Text = "Анализ&nbsp;вклада&nbsp;субъектов&nbsp;в&nbsp;ситуацию&nbsp;в&nbsp;УрФО";
            CrossLink2.NavigateUrl = "~/reports/STAT_0001_0007/Default.aspx";
            CrossLink3.Text = "Анализ&nbsp;информации&nbsp;по&nbsp;выбранному&nbsp;субъекту&nbsp;РФ";

            CrossLink3.NavigateUrl = "~/reports/STAT_0001_0003/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                ComboPeriod.Width = 300;
                ComboPeriod.MultiSelect = false;
                ComboPeriod.ShowSelectedValue = false;
                ComboPeriod.ParentSelect = false;
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillLabourMarketNonEmptyDays(DataDictionariesHelper.LabourMarketNonEmptyDays));
                ComboPeriod.SelectLastNode();
                ComboPeriod.PanelHeaderTitle = "Выберите дату";

                ComboLastPeriod.Width = 300;
                ComboLastPeriod.MultiSelect = false;
                ComboLastPeriod.ShowSelectedValue = false;
                ComboLastPeriod.ParentSelect = false;
                ComboLastPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillLabourMarketNonEmptyDays(DataDictionariesHelper.LabourMarketNonEmptyDays));
                ComboLastPeriod.SelectLastNode();
                ComboLastPeriod.RemoveTreeNodeByName(ComboLastPeriod.SelectedValue);
                ComboLastPeriod.SelectLastNode();
                if (CRHelper.IsMonthCaption(ComboLastPeriod.SelectedValue.Trim(' ')))
                {
                    ComboLastPeriod.RemoveTreeNodeByName(ComboLastPeriod.SelectedValue);
                    ComboLastPeriod.SelectLastNode();
                }
                if (ComboLastPeriod.SelectedValue.Trim(' ').Length == 4)
                {
                    ComboLastPeriod.RemoveTreeNodeByName(ComboLastPeriod.SelectedValue);
                    ComboLastPeriod.SelectLastNode();
                }
                ComboLastPeriod.PanelHeaderTitle = "Выберите дату для сравнения";
            }

            currDateTime = GetDateString(ComboPeriod.GetSelectedNodePath(), ComboPeriod.SelectedNode.Level);
            //lastDateTime = currDateTime.AddDays(-7);

            Node selectedNode = ComboPeriod.SelectedNode;
            // если выбран месяц, то берем в нем последний день
            if (selectedNode.Nodes.Count != 0)
            {
                selectedNode = ComboPeriod.GetLastChild(selectedNode);
            }

            lastDateTime = GetDateString(ComboLastPeriod.GetSelectedNodePath(), ComboLastPeriod.SelectedNode.Level);

            if (lastDateTime > currDateTime)
            {
                DateTime temp = lastDateTime;
                lastDateTime = currDateTime;
                currDateTime = temp;
            }

            Page.Title = "Мониторинг ситуации на рынке труда";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format(@"Форма мониторинга ситуации на рынке труда в субъектах Российской Федерации, входящих в Уральский федеральный округ, по состоянию на {0:dd.MM.yyyy}, динамика к {1:dd.MM.yyyy} (по данным органов государственной власти субъектов Российской Федерации)", currDateTime, lastDateTime);

            CommentText1.Text = string.Empty;
            CommentText2.Text = string.Empty;

            mapElementCaption.Text = string.Format("Уровень зарегистрированной безработицы и общая численность зарегистрированных безработных граждан по субъектам УрФО на {0:dd.MM.yyyy}", currDateTime);
            map2ElementCaption.Text = string.Format("Число зарегистрированных безработных в расчёте на 1 вакансию по субъектам УрФО на {0:dd.MM.yyyy}", currDateTime);

            int days = currDateTime.DayOfYear - lastDateTime.DayOfYear;
            days += 365*(currDateTime.Year - lastDateTime.Year);

            lastPrevDateTime = new DateTime(lastDateTime.Year, lastDateTime.Month, lastDateTime.Day);
            lastPrevDateTime = lastPrevDateTime.AddDays(-days);
            periodPrevLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", lastPrevDateTime, 5);

            string queryDate = DataProvider.GetQueryText("STAT_0001_0002_date_prev_last");
            DataTable dtLastDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(queryDate, dtLastDate);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", currDateTime, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", lastDateTime, 5);
            periodPrevLastWeekDate.Value = dtLastDate.Rows[0][5].ToString();

            сurrentYear.Value = currDateTime.Year.ToString();
            lastYear.Value = (currDateTime.Year - 1).ToString();
            
            debtsPeriodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", lastDateTime, 5);

            string query = DataProvider.GetQueryText("STAT_0001_0002_debts_date");
            DataTable dtDebtsDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsDate);
                        
            if (dtDebtsDate.Rows.Count == 2)
            {
                if (dtDebtsDate.Rows[0][1] != DBNull.Value && dtDebtsDate.Rows[0][1].ToString() != string.Empty)
                {
                    debtsPeriodLastWeekDate.Value = dtDebtsDate.Rows[0][1].ToString();
                    debtsLastDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[0][1].ToString(), 3);
                }
                if (dtDebtsDate.Rows[1][1] != DBNull.Value && dtDebtsDate.Rows[1][1].ToString() != string.Empty)
                {
                    debtsPeriodCurrentDate.Value = dtDebtsDate.Rows[1][1].ToString();
                    debtsCurrDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[1][1].ToString(), 3);
                }
            }
            else if (dtDebtsDate.Rows.Count == 3)
            {
                if (dtDebtsDate.Rows[1][1] != DBNull.Value && dtDebtsDate.Rows[1][1].ToString() != string.Empty)
                {
                    debtsPeriodLastWeekDate.Value = dtDebtsDate.Rows[1][1].ToString();
                    debtsLastDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[1][1].ToString(), 3);
                }
                if (dtDebtsDate.Rows[2][1] != DBNull.Value && dtDebtsDate.Rows[2][1].ToString() != string.Empty)
                {
                    debtsPeriodCurrentDate.Value = dtDebtsDate.Rows[2][1].ToString();
                    debtsCurrDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[2][1].ToString(), 3);
                }
            }

            days = debtsCurrDateTime.DayOfYear - debtsLastDateTime.DayOfYear;
            days += 365 * (debtsCurrDateTime.Year - debtsLastDateTime.Year);

            debtLastPrevDateTime = new DateTime(debtsLastDateTime.Year, debtsLastDateTime.Month, debtsLastDateTime.Day);
            debtLastPrevDateTime = debtLastPrevDateTime.AddDays(-days);
            debtsPeriodPrevLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", debtLastPrevDateTime, 5);

            queryDate = DataProvider.GetQueryText("STAT_0001_0002_date_prev_last_debts");
            dtLastDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(queryDate, dtLastDate);

            debtsPeriodPrevLastWeekDate.Value = dtLastDate.Rows[0][5].ToString();

            query = DataProvider.GetQueryText("STAT_0001_0002_redundantLevelRF_date");
            DataTable dtRedundantLevelRFDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtRedundantLevelRFDate);

            if (dtRedundantLevelRFDate.Rows.Count > 0)
            {
                redundantLevelRFDate.Value = dtRedundantLevelRFDate.Rows[0][1].ToString();
                redundantLevelRFDateTime =
                    CRHelper.DateByPeriodMemberUName(dtRedundantLevelRFDate.Rows[0][1].ToString(), 3);
            }
            else
            {
                redundantLevelRFDate.Value = debtsPeriodPrevLastWeekDate.Value;
                redundantLevelRFDateTime = debtLastPrevDateTime;
                hasRfRedudantLevel = false;
            }

            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();

            UltraWebGrid2.Bands.Clear();
            UltraWebGrid2.DataBind();

            // заполняем карту формами
            string regionStr = "УрФО";
            DundasMap1.Shapes.Clear();
            DundasMap1.LoadFromShapeFile(Server.MapPath(string.Format("../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);
            // заполняем карту данными
            FillMapData1(DundasMap1);

            DundasMap2.Shapes.Clear();
            DundasMap2.LoadFromShapeFile(Server.MapPath(string.Format("../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);
            // заполняем карту данными
            FillMapData2(DundasMap2);
        }

        public DateTime GetDateString(string source, int level)
        {
            string[] sts = source.Split('|');
            if (sts.Length > 1)
            {
                switch (level)
                {
                    // нулевой уровень выбрать нельзя
                    case 1:
                        {
                            return GetDateString(ComboPeriod.GetNodeLastChild(ComboPeriod.SelectedNode), level + 1);
                        }
                    case 2:
                        {
                            string month = sts[1].TrimEnd(' ');
                            string day = sts[2].TrimEnd(' ');
                            return new DateTime(Convert.ToInt32(sts[0]), CRHelper.MonthNum(month), Convert.ToInt32(day));
                        }
                }
            }
            return DateTime.MinValue;
        }

        #region Обработчики карты

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <returns>найденная форма</returns>
        public static Shape FindMapShape(MapControl map, string patternValue)
        {
            string subject = patternValue.Replace("область", "обл.");
            subject = subject.Replace("автономный округ", "АО");

            ArrayList shapeList = map.Shapes.Find(subject, true, false);
            if (shapeList.Count > 0)
            {
                return (Shape)shapeList[0];
            }
            return null;
        }

        public void FillMapData1(MapControl map)
        {
            if (map == null)
            {
                return;
            }

            string query = DataProvider.GetQueryText("STAT_0001_0002_map1");

            dtMap1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtMap1);

            map.Symbols.Clear();

            foreach (DataRow row in dtMap1.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    string regionName = row[0].ToString();

                    if (RegionsNamingHelper.IsSubject(regionName))
                    {
                        Shape shape = FindMapShape(map, regionName);
                        if (shape != null)
                        {
                            double unemploymentLevel = Convert.ToDouble(row[1]) / 100;
                            double unemploymentPopulation = Convert.ToDouble(row[2]);

                            shape["Name"] = regionName;
                            shape["UnemploymentLevel"] = unemploymentLevel;
                            shape.ToolTip = string.Format("#NAME \nчисленность безработных: {0:N0} чел.\nуровень безработицы: #UNEMPLOYMENTLEVEL{{P3}}",
                                    unemploymentPopulation);
                            shape.TextAlignment = ContentAlignment.MiddleCenter;
                            shape.Offset.X = -15;
                            if (!IsSmallResolution)
                            {
                                shape.Offset.Y = -30;
                            }

                            shape.Text = string.Format("{0}\n{2:N0} чел.\n{1:P3}", shape.Name, unemploymentLevel, unemploymentPopulation);
                            
                            shape.BorderWidth = 2;
                            shape.TextColor = Color.Black;
                            //shape.Font = new Font("Verdana", IsSmallResolution ? 7 : 8, FontStyle.Bold);
                            shape.TextVisibility = TextVisibility.Shown;

                            Symbol symbol = new Symbol();
                            symbol.Name = shape.Name + map.Symbols.Count;
                            symbol.ParentShape = shape.Name;
                            symbol["UnemploymentPopulation"] = unemploymentPopulation;
                            symbol.Offset.Y = -30;
                            symbol.Color = Color.DarkViolet;
                            symbol.MarkerStyle = MarkerStyle.Star;
                            map.Symbols.Add(symbol);

                            if (IsSmallResolution)
                            {
                                if (shape.Name.Contains("Курган"))
                                {
                                    shape.TextAlignment = ContentAlignment.MiddleCenter;
                                }
                                if (shape.Name.Contains("Челябинск"))
                                {
                                    shape.TextAlignment = ContentAlignment.TopCenter;
                                    shape.Offset.X = -10;
                                    symbol.Offset.Y = -10;
                                }
                                if (shape.Name.Contains("Тюмен"))
                                {
//                                    shape.TextAlignment = ContentAlignment.TopCenter;
//                                    symbol.Offset.Y = -10;
                                }
                                if (shape.Name.Contains("Свердловск"))
                                {
                                    shape.Offset.Y = -10;
                                    //symbol.Offset.Y = -10;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void FillMapData2(MapControl map)
        {
            if (map == null)
            {
                return;
            }

            string query = DataProvider.GetQueryText("STAT_0001_0002_map2");

            dtMap2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtMap2);

            foreach (DataRow row in dtMap2.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty &&
                    row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                {
                    string regionName = row[0].ToString();

                    if (RegionsNamingHelper.IsSubject(regionName))
                    {
                        Shape shape = FindMapShape(map, regionName);
                        if (shape != null)
                        {
                            double tensionKoeff = Convert.ToDouble(row[1]);
                            double redundantCount = Convert.ToDouble(row[2]);
                            double vacancyCount = Convert.ToDouble(row[3]);
                            double totalCount = vacancyCount + redundantCount;

                            shape["Name"] = regionName;
                            shape["TensionKoeff"] = tensionKoeff;
                            shape.ToolTip = string.Format("#NAME \nчисло зарегистрированных безработных в расчёте на 1 вакансию: #TENSIONKOEFF{{N2}}\nчисло безработных: {0:N0} чел.\nчисло вакансий: {1:N0}",
                                    redundantCount, vacancyCount);
                            shape.TextAlignment = ContentAlignment.MiddleCenter;
                            shape.TextColor = Color.Black;
                            shape.Offset.X = -15;
                            if (!IsSmallResolution)
                            {
                                shape.Offset.Y = -30;
                            }

                            shape.Text = string.Format("{0}\nвакансий: {2:N0}\n{1:N2}", shape.Name, tensionKoeff, vacancyCount);
                            shape.BorderWidth = 2;
                            //shape.Font = new Font("Verdana", IsSmallResolution ? 7 : 8, FontStyle.Bold);
                            shape.TextVisibility = TextVisibility.Shown;

                            Symbol symbol = new Symbol();
                            symbol.Name = shape.Name + map.Symbols.Count;
                            symbol.ParentShape = shape.Name;
                            symbol["vacancyCount"] = totalCount == 0 ? 0 : vacancyCount / totalCount * 100;
                            symbol["redundantCount"] = totalCount == 0 ? 0 : redundantCount / totalCount * 100;
                            symbol.Offset.Y = -40;
                            symbol.MarkerStyle = MarkerStyle.Circle;
                            map.Symbols.Add(symbol);

                            if (IsSmallResolution)
                            {
                                if (shape.Name.Contains("Курган"))
                                {
                                    shape.TextAlignment = ContentAlignment.MiddleCenter;
                                    shape.Offset.Y = -20;
                                }
                                if (shape.Name.Contains("Челябинск"))
                                {
                                    shape.TextAlignment = ContentAlignment.TopCenter;
                                    symbol.Offset.Y = -20;
                                }
                                if (shape.Name.Contains("Тюмен"))
                                {
//                                    shape.TextAlignment = ContentAlignment.TopCenter;
//                                    symbol.Offset.Y = -20;
                                }
                                if (shape.Name.Contains("Свердловск"))
                                {
                                    shape.Offset.Y = -10;
                                    //symbol.Offset.Y = -10;
                                }
                            }
                        }
                    }
                }
            }
        }

        void DundasMap2_PostPaint(object sender, MapPaintEventArgs e)
        {
            Symbol symbol = e.MapElement as Symbol;
            if (symbol != null && symbol.Visible)
            {
                // Размер диаграммы
                int width = 30;
                int height = 30;

                // Get the symbol location in pixels.
                MapGraphics mg = e.Graphics;
                PointF p = symbol.GetCenterPointInContentPixels(mg);
                int x = (int)p.X - width / 2;
                int y = (int)p.Y - height / 2;
                symbol.Width = width;
                symbol.Height = height;

                int startAngle, sweepAngle1, sweepAngle2;

                // Делим углы соотвественно долям
                startAngle = 0;
                sweepAngle1 = Convert.ToInt32(3.6 * Convert.ToInt32(symbol["redundantCount"]));
                sweepAngle2 = Convert.ToInt32(3.6 * Convert.ToInt32(symbol["vacancyCount"]));

                // Поверх символа рисуем круговую диаграмму
                Graphics g = mg.Graphics;
                g.FillPie(new SolidBrush(Color.DarkViolet), x, y, width, height, startAngle, sweepAngle1);
                startAngle += sweepAngle1;
                g.FillPie(new SolidBrush(Color.Black), x, y, width, height, startAngle, sweepAngle2);

                g.DrawEllipse(new Pen(Color.Gray, 1), x, y, width, height);
            }
        }

        #endregion

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0002_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                DataColumn numberColumn = new DataColumn("№", typeof(string));
                dtGrid.Columns.Add(numberColumn);

                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString().Split(';').Length > 0)
                    {
                        bool isRank = false;
                        string rowString = string.Empty;
                        if ((row[0].ToString().Contains("Уровень регистрируемой безработицы") ||
                             row[0].ToString().Contains("Число зарегистрированных безработных в расчёте на 1 вакансию")) && row[0].ToString().Contains("Прирост"))
                        {
                            rowString = row[0].ToString().Split(';')[0];
                            row[0] = "ранг по УрФО";
                            isRank = true;
                        }

                        row[0] = row[0].ToString().Split(';')[0];

                        if (DataDictionariesHelper.LabourMarketIndicatorNumbers.ContainsKey(row[0].ToString()) ||
                            (isRank && DataDictionariesHelper.LabourMarketIndicatorNumbers.ContainsKey(rowString)) ||
                            row[0].ToString().Contains("АППГ"))
                        {
                            if (isRank)
                            {
                                row["№"] = DataDictionariesHelper.GetLabourMarketIndicatorNumber(rowString);
                            }
                            else 
                            {
                                row["№"] = DataDictionariesHelper.GetLabourMarketIndicatorNumber(row[0].ToString());
                            }

                            if (row[0].ToString().Contains("АППГ"))
                            {
                                row[0] = string.Format("Аналогичный период прошлого года ({0} г.)", currDateTime.Year - 1);
                            }
                        }
                        if (row[0].ToString() == "Целевое значение по уровню зарегистрированных безработных ")
                        {
                            row["№"] = 1;
                        } 
                    }
                }

                ((UltraWebGrid)sender).DataSource = dtGrid;

                CommentTextDataBind();
            }
        }

        protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0002_debts_grid");
            dtDebtsGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtDebtsGrid);
            if (dtDebtsGrid.Rows.Count > 0)
            {
                DataColumn numberColumn = new DataColumn("№", typeof (string));
                dtDebtsGrid.Columns.Add(numberColumn);

                foreach (DataRow row in dtDebtsGrid.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString().Split(';').Length > 0)
                    {
                        row[0] = row[0].ToString().Split(';')[0];
                        if (DataDictionariesHelper.LabourMarketIndicatorNumbers.ContainsKey(row[0].ToString()))
                        {
                            row["№"] = DataDictionariesHelper.GetLabourMarketIndicatorNumber(row[0].ToString());
                        }
                    }
                }

                ((UltraWebGrid) sender).DataSource = dtDebtsGrid;
            }
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        private static string GetStringDTValue(DataTable dt, string columnName)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return dt.Rows[0][columnName].ToString();
            }
            return string.Empty;
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            ((UltraWebGrid)sender).Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.RowAlternateStyleDefault.BackColor = Color.White;
            e.Layout.RowSelectorsDefault = IsSmallResolution ? RowSelectors.No : RowSelectors.Yes;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            DateTime lastDT = (sender == UltraWebGrid1) ? lastDateTime : debtsLastDateTime;
            DateTime currDT = (sender == UltraWebGrid1) ? currDateTime : debtsCurrDateTime;

            // перемещаем колонку с номером в начало
            UltraGridColumn numberColumn = e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1];
            e.Layout.Bands[0].Columns.RemoveAt(e.Layout.Bands[0].Columns.Count - 1);
            e.Layout.Bands[0].Columns.Insert(0, numberColumn);

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                int widthColumn = i < e.Layout.Bands[0].Columns.Count - 2 ? 63 : 65;

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn, 1280);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(20, 1280);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            int nameColumnWidth = IsSmallResolution ? 200 : 210;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(nameColumnWidth, 1280);
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].MergeCells = true;

            if (IsSmallResolution)
            {
                for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        e.Layout.Grid.Columns[i].Hidden = true;
                    }
                    else
                    {
                        
                        string caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];
                        caption = string.Format("{0}\n({1:dd.MM})", RegionsNamingHelper.ShortName(caption), currDT);
                        CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, caption, "");
                    }
                }
            }
            else
            {
                for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    if (i == 0 || i == 1)
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                    }
                    else
                    {
                        e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                    }
                }

                int multiHeaderPos = 2;
                
                for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
                {
                    ColumnHeader ch = new ColumnHeader(true);
                    ch.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];

                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, string.Format("{0:dd.MM}", lastDT), "");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, string.Format("{0:dd.MM}", currDT), "");

                    ch.RowLayoutColumnInfo.OriginY = 0;
                    ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                    ch.RowLayoutColumnInfo.SpanY = 1;
                    multiHeaderPos += 2;
                    ch.RowLayoutColumnInfo.SpanX = 2;
                    e.Layout.Bands[0].HeaderLayout.Add(ch);
                }
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellsCount = e.Row.Cells.Count;

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                int rowIndex = e.Row.Index;

                // номер показателя
                int k = (rowIndex % 3);
                // строка с уровнем регистрируемой безработицы
                bool isRedundantLevel = (e.Row.Cells[1].Value != null &&
                                         e.Row.Cells[1].Value.ToString().ToLower().Contains("уровень регистрируемой безработицы"));
                bool isRedundantLevelRank = (e.Row.PrevRow != null && e.Row.PrevRow.Cells[1].Value != null &&
                                             e.Row.PrevRow.Cells[1].Value.ToString().ToLower().Contains("уровень регистрируемой безработицы"));
                // строка с рангом уровня регистрируемой безработицы
                bool isRankRow = (e.Row.Cells[1].Value != null &&
                                  e.Row.Cells[1].Value.ToString().Contains("ранг по УрФО"));
                // яркая колонка
                bool bright = IsSmallResolution || (i % 2 != 0);

                if (i != 0 && bright)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 2;
                }
                else
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthLeft = 2;
                }

                if (i == 1 && isRankRow)
                {
                    e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Right;
                    if (e.Row.PrevRow != null && e.Row.PrevRow.PrevRow != null)
                    {
                        e.Row.PrevRow.PrevRow.Cells[i].Style.BorderDetails.WidthBottom = 0;
                    }
                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 0;
                }

                if (i > 1)
                {
                    switch (k)
                    {
                        case 0:
                            {
                                e.Row.Cells[i].Style.BorderDetails.WidthBottom = 0;
                                break;
                            }
                        case 1:
                            {
                                e.Row.Cells[i].Style.BorderDetails.WidthTop = 0;
                                e.Row.Cells[i].Style.BorderDetails.WidthBottom = 0;
                                break;
                            }
                        case 2:
                            {
                                e.Row.Cells[i].Style.BorderDetails.WidthTop = 0;
                                break;
                            }
                    }
                }

                if (i > 1 && e.Row.Cells[1].Value.ToString() == "Целевое значение по уровню зарегистрированных безработных " &&
                    e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[1].Value.ToString() == "Целевое значение по уровню зарегистрированных безработных ")
                {
                    e.Row.Cells[i].Value = null;
                }

                if (i != 0 && i != 1 && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    double value = Convert.ToDouble(e.Row.Cells[i].Value);

                    bool growRate = (k == 1);
                    bool rate = (!isRedundantLevel && k == 2);
                    bool mlnUnit = (e.Row.Cells[0].Value != null && e.Row.Cells[1].Value.ToString().Contains("млн.руб."));
                    bool thsUnit = (e.Row.Cells[0].Value != null && e.Row.Cells[1].Value.ToString().Contains("Число зарегистрированных безработных"));

                    if (growRate)
                    {
                        if (sender == UltraWebGrid1)
                        {
                            if (i % 2 ==1)
                            {
                                e.Row.Cells[i].Title = String.Format("Темп прироста к {0:dd.MM.yyyy}", lastDateTime);
                            }
                            else
                            {
                                e.Row.Cells[i].Title = String.Format("Темп прироста к {0:dd.MM.yyyy}", lastPrevDateTime);
                            }
                        }
                        else
                        {
                            if (i % 2 == 1)
                            {
                                e.Row.Cells[i].Title = String.Format("Темп прироста к {0:dd.MM.yyyy}", debtsLastDateTime);
                            }
                            else
                            {
                                e.Row.Cells[i].Title = String.Format("Темп прироста к {0:dd.MM.yyyy}", debtLastPrevDateTime);
                            }
                        }
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = bright ? "~/images/arrowRedUpBB.png" : "~/images/arrowRedUpBBdim.png";
                            }
                            else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = bright ? "~/images/arrowGreenDownBB.png" : "~/images/arrowGreenDownBBdim.png";
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                    if (!isRankRow && rate)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                            {
                                e.Row.Cells[i].Title = (sender == UltraWebGrid1) ? "Прирост к прошлой неделе" : "Прирост к прошлой дате";
                            }
                            else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                            {
                                e.Row.Cells[i].Title = (sender == UltraWebGrid1) ? "Падение относительно прошлой недели" : "Падение относительно прошлой даты";
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }

                    if (isRankRow)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = bright ? "~/images/starGrayBB.png" : "~/images/starGrayBBdim.png";
                                e.Row.Cells[i].Title = isRedundantLevelRank ? "Самый высокий уровень безработицы" : "Самое большое число безработных на 1 вакансию";
                            }
                            else if (Convert.ToInt32(e.Row.Cells[i].Value) == 6)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = bright ? "~/images/starYellowBB.png" : "~/images/starYellowBBdim.png";
                                e.Row.Cells[i].Title = isRedundantLevelRank ? "Самый низкий уровень безработицы" : "Самое маленькое число безработных на 1 вакансию";
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }

                    switch (k)
                    {
                        case 0:
                            {
                                e.Row.Cells[i].Style.Font.Bold = true;
                                if (mlnUnit)
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:N3}", value);
                                }
                                else if (thsUnit)
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:N2}", value);
                                }
                                else if (isRedundantLevel)
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:N3}%", value);
                                }
                                else if (e.Row.Cells[1].Value.ToString() == "Целевое значение по уровню зарегистрированных безработных ")
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:N2}%", e.Row.Cells[i].Value);
                                }
                                else
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:N0}", value);
                                }
                                break;
                            }
                        case 1:
                            {
                                e.Row.Cells[i].Value = string.Format("{0:P2}", value);
                                break;
                            }
                        case 2:
                            {
                                if (mlnUnit)
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:N3}", value);
                                }
                                else if (thsUnit)
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:N2}", value);
                                }
                                else
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:N0}", value);
                                }
                                break;
                            }
                    }
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    string cellValue = cell.Value.ToString();
                    if (cellValue.Contains("%"))
                    {
                        cellValue = cellValue.TrimEnd('%');
                    }

                    decimal value;
                    if (i != 0 && (k == 1 || k == 2) && decimal.TryParse(cellValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out value))
                    {
                        if (value > 0)
                        {
                            cell.Value = string.Format("+{0}", cell.Value);
                        }
                    }

                    e.Row.Cells[i].Style.Padding.Right = (i == 0) ? 1 : 5;

                    if (i >= cellsCount - 2)
                    {
                        cell.Style.Font.Bold = true;
                    }

                    if (i != 0 && !bright)
                    {
                        //cell.Style.Font.Italic = true;
                        //                        if (cell.Style.ForeColor == Color.Red)
                        //                        {
                        //                            cell.Style.ForeColor = Color.LightCoral;
                        //                        }
                        //                        else
                        //                        {
                        cell.Style.ForeColor = Color.DimGray;
                        //                        }
                    }
                }
            }
        }

        #endregion

        #region Комментарии к гриду

        private void CommentTextDataBind()
        {
            string query = DataProvider.GetQueryText("STAT_0001_0002_commentText");
            dtCommentText = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtCommentText);

            if (dtCommentText.Rows.Count > 0)
            {
                string dateTimeStr = string.Format("{0:dd.MM.yyyy}", currDateTime);
                string dateTimeDebtsStr = string.Format("{0:dd.MM.yyyy}", debtsCurrDateTime);
                string dateLastTimeDebtsStr = string.Format("{0:dd.MM.yyyy}", debtsLastDateTime);
                double totalCount = GetDoubleDTValue(dtCommentText, "Общая численность по УрФО");
                double totalRate = GetDoubleDTValue(dtCommentText, "Общий темп прироста по УрФО");
                double totalGrow = GetDoubleDTValue(dtCommentText, "Общий прирост по УрФО");
                string totalRateArrow = totalRate > 0
                                               ? "Прирост <img src=\"../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">"
                                               : "Снижение <img src=\"../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">";
                string totalRateStr = totalRate > 0 ? "составил" : "составило";
                string totalRatePlus = totalRate > 0 ? "+" : string.Empty;

                string growSubjectList = GetStringDTValue(dtCommentText, "Список субъектов с приростом");
                string fallSubjectList = GetStringDTValue(dtCommentText, "Список субъектов с падением");
                string normalSubjectList = GetStringDTValue(dtCommentText, "Список субъектов с неизменной численностью");
                double factoryCount = GetDoubleDTValue(dtCommentText, "Число организаций");
                double incompleteEmployersCount = GetDoubleDTValue(dtCommentText, "Численность работников с неполной занятостью");
                double totalDebts = GetDoubleDTValue(dtCommentText, "Cумма задолженности");
                double totalLastWeekDebts = GetDoubleDTValue(dtCommentText, "Cумма задолженности прошлая неделя");
                double slavesCount = GetDoubleDTValue(dtCommentText, "Количество граждан, имеющих задолженность");
                double debtsPercent = GetDoubleDTValue(dtCommentText, "Прирост задолженности");
                string debtsPercentArrow = debtsPercent == 0
                                               ? "не изменилась"
                                               : debtsPercent > 0
                                               ? string.Format("увеличилась <img src=\"../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\"> на <b>{0:N3}</b>&nbsp;млн.руб", Math.Abs(debtsPercent))
                                               : string.Format("уменьшилась <img src=\"../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\"> на <b>{0:N3}</b>&nbsp;млн.руб", Math.Abs(debtsPercent));

                double almostRedundantsCount = GetDoubleDTValue(dtCommentText, "Численность работников, предполагаемых к увольнению");
                double almostRedundantsRate = GetDoubleDTValue(dtCommentText, "Прирост числа работников, предполагаемых к увольнению");
                string monitoringStartStr1 = currDateTime.Year == 2009
                                                ? "С начала проведения мониторинга (с октября 2008 года)"
                                                : "С начала года";
                string monitoringStartStr2 = currDateTime.Year == 2009
                                                ? "С начала проведения мониторинга"
                                                : "С начала года";
                double redundantsTotal = GetDoubleDTValue(dtCommentText, "Численность работников, уволенных с начала мониторинга");
                double redundantsTotalRate = GetDoubleDTValue(dtCommentText, "Прирост числа работников, уволенных за отчетный период");

                double redundantlevelValue = GetDoubleDTValue(dtCommentText, "Уровень регистрируемой безработицы ");
                double redundantlevelGrow = GetDoubleDTValue(dtCommentText, "Прирост уровня регистрируемой безработицы");
                string redundantlevelArrow = redundantlevelGrow == 0
                                               ? "не изменился и составляет"
                                               : redundantlevelGrow > 0
                                                ? string.Format("увеличился <img src=\"../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\"> на <b>{0:N3}</b>&nbsp;процентных пункта и на <b>{1}</b> составил", Math.Abs(redundantlevelGrow), dateTimeStr)
                                                : string.Format("уменьшился <img src=\"../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\"> на <b>{0:N3}</b>&nbsp;процентных пункта и на <b>{1}</b> составил", Math.Abs(redundantlevelGrow), dateTimeStr);

                string redundantLevelRFGrow = String.Empty;
                if (hasRfRedudantLevel)
                {
                    double redundantLevelRFValue =
                        GetDoubleDTValue(dtCommentText, "Уровень регистрируемой безработицы РФ");
                    string redundantLevelRFArrow;
                    string redundantLevelRFDescription =
                        String.Format(", который на <b>{0:dd.MM.yyyy}</b> составляет <b>{1:N3}%</b>",
                                      redundantLevelRFDateTime, redundantLevelRFValue);
                    if (redundantlevelValue > redundantLevelRFValue)
                    {
                        redundantLevelRFArrow = "выше <img src=\"../../images/ballRedBB.png\"> уровня";
                    }
                    else if (redundantlevelValue < redundantLevelRFValue)
                    {
                        redundantLevelRFArrow = "ниже <img src=\"../../images/ballGreenBB.png\"> уровня";
                    }
                    else
                    {
                        redundantLevelRFArrow = "соответствует <img src=\"../../images/ballGreenBB.png\"> уровню";
                        redundantLevelRFDescription = String.Empty;
                    }
                    redundantLevelRFGrow =
                        String.Format(", что {0} безработицы в целом по РФ{1}", redundantLevelRFArrow,
                                      redundantLevelRFDescription);
                }

                double vacancyCount = GetDoubleDTValue(dtCommentText, "Потребность в работниках");
                double vacancyCountGrow = GetDoubleDTValue(dtCommentText, "Прирост потребности в работниках");
                string vacancyCountGrowArrow = vacancyCountGrow == 0
                               ? "не изменилась и составляет"
                               : vacancyCountGrow > 0
                               ? string.Format("увеличилась <img src=\"../../images/arrowGreenUpBB.png\" width=\"13px\" height=\"16px\"> на <b>{0:N0}</b>&nbsp;единиц и составила", Math.Abs(vacancyCountGrow))
                               : string.Format("уменьшилась <img src=\"../../images/arrowRedDownBB.png\" width=\"13px\" height=\"16px\"> на <b>{0:N0}</b>&nbsp;единиц и составила", Math.Abs(vacancyCountGrow));
                string freeVacancyCountStr = (totalCount - vacancyCount) > 0
                       ? string.Format("ниже количества безработных на <b>{0:N0}</b>&nbsp;единиц", Math.Abs(totalCount - vacancyCount))
                       : string.Format("выше количества безработных на <b>{0:N0}</b>&nbsp;единиц", Math.Abs(totalCount - vacancyCount));
                double tensionKoeff = GetDoubleDTValue(dtCommentText, "Число зарегистрированных безработных в расчёте на 1 вакансию", double.MinValue);
                double tensionKoeffGrow = GetDoubleDTValue(dtCommentText, "Прирост числа зарегистрированных безработных в расчёте на 1 вакансию", double.MinValue);
                string tensionKoeffGrowArrow = tensionKoeffGrow != double.MinValue ?
                                               tensionKoeffGrow > 0
                                                ? "выросло <img src=\"../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\"> до"
                                                : "снизилось <img src=\"../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\"> до"
                                                : "составило";

                DateTime nextDateTime = currDateTime.AddMonths(1);
                int nextMonthNumber = nextDateTime.Month;
                int monthCount = currDateTime.Month != 12 ? (currDateTime.Year - 2007) * 12 + nextMonthNumber - 1 : (currDateTime.Year - 2007 + 1) * 12;

                if (dtKoeff == null)
                {
                    dtKoeff = GetSubjectKoeffTable();
                }


                double forecastValue = 0;
                for (int i = 1; i < dtKoeff.Rows.Count; i++)
                {
                    DataRow koeffRow = dtKoeff.Rows[i];

                    double b0 = Convert.ToDouble(koeffRow[1]);
                    double xi = Convert.ToDouble(koeffRow[2]) * (monthCount);
                    double koeff = (nextDateTime.Month == 12) ? 0 : Convert.ToDouble(koeffRow[nextMonthNumber + 2]);
                    double logForecast = b0 + xi + koeff;
                    forecastValue += Math.Pow(10, logForecast);
                }

                string str1 = string.Format(@"&nbsp;&nbsp;&nbsp;Численность безработных граждан, зарегистрированных в органах службы занятости, 
по состоянию на <b>{0}</b> составила <b>{1:N3}</b>&nbsp;тыс.человек.<br/>",
                    dateTimeStr, totalCount / 1000);

                string str2 = string.Format(@"&nbsp;&nbsp;&nbsp;{0} числа безработных граждан за период с <b>{1:dd.MM}</b> по <b>{2:dd.MM}</b> в целом по <b>УрФО</b>
{5} <b>{3:N0}</b>&nbsp;чел. (темп прироста <b>{6}{4:P2}</b>).",
    totalRateArrow, lastDateTime, currDateTime, Math.Abs(totalGrow), totalRate, totalRateStr, totalRatePlus);

                string str3 = growSubjectList == string.Empty ? string.Empty :
                    string.Format("&nbsp;Рост <img src=\"../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\"> численности безработных граждан наблюдался в <b>{0}</b>.",
                    growSubjectList);

                string str4 = normalSubjectList == string.Empty ? string.Empty :
                    string.Format(@"&nbsp;<b>{0}</b> численность безработных граждан не изменилась по сравнению с <b>{1:dd.MM}</b>.",
                    normalSubjectList, lastDateTime);

                string str5 = fallSubjectList == string.Empty ? string.Empty :
                    string.Format("&nbsp;Снижение <img src=\"../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\"> численности безработных граждан наблюдалось в <b>{0}</b>.",
                    fallSubjectList);

                string str6 = string.Format(@"<br/>&nbsp;&nbsp;&nbsp;Уровень регистрируемой безработицы в течение отчетного периода {0} <b>{1:P3}</b>{2}.<br/>", redundantlevelArrow,
                    redundantlevelValue / 100, redundantLevelRFGrow);

                string str7 = string.Format(@"&nbsp;&nbsp;&nbsp;Потребность в работниках, заявленная работодателями в органы службы занятости населения, 
{0} <b>{1:N0}</b>&nbsp;вакансий, что {2}.<br/>",
   vacancyCountGrowArrow, vacancyCount, freeVacancyCountStr);

                string str8 = tensionKoeff != double.MinValue ? string.Format(@"&nbsp;&nbsp;&nbsp;Число зарегистрированных безработных в расчёте на 1 вакансию за период с <b>{0:dd.MM}</b> по <b>{1:dd.MM}</b> {2} <b>{3:N2}</b>
безработных граждан на одну вакансию.<br/>", lastDateTime, currDateTime, tensionKoeffGrowArrow, tensionKoeff) : string.Empty;

                string str9 = string.Format(@"&nbsp;&nbsp;&nbsp;{1} общая заявленная численность 
работников, предполагаемых к увольнению (ликвидация  организаций, сокращение численности или штата), составляет <b>{0:N0}</b>&nbsp;чел.", almostRedundantsCount, monitoringStartStr1);

                string str10 = !IsYearJoint() ? (almostRedundantsRate > 0) ? string.Format(@"&nbsp;Количество граждан, предполагаемых к увольнению, возросло <img src='../../images/arrowRedUpBB.png' width='13px' height='16px'> на <b>{0:N0}</b>&nbsp;чел. 
в сравнении с <b>{1:dd.MM}</b>.<br/>", Math.Abs(almostRedundantsRate), lastDateTime) : string.Format(@"&nbsp;Количество граждан, предполагаемых к увольнению, сократилось <img src='../../images/arrowGreenDownBB.png' width='13px' height='16px'> на <b>{0:N0}</b>&nbsp;чел. 
в сравнении с <b>{1:dd.MM}</b>.<br/>", Math.Abs(almostRedundantsRate), lastDateTime) : string.Empty;

                string str11 = !IsYearJoint() ? string.Format(@"&nbsp;&nbsp;&nbsp{2} численность уволенных работников по сокращению 
достигла <b>{0:N0}</b>&nbsp;чел., из них за отчетный период – <b>{1:N0}</b>&nbsp;чел.<br/>", redundantsTotal, redundantsTotalRate, monitoringStartStr2) :
                        string.Format(@"&nbsp;{1} численность уволенных работников по сокращению 
достигла <b>{0:N0}</b>&nbsp;чел.<br/>", redundantsTotal, monitoringStartStr2);

                string str12 = string.Format(@"&nbsp;&nbsp;&nbsp;<b>{0:N0}</b> организации заявили о переводе части работников на режим неполного рабочего времени, 
предоставлении вынужденных отпусков, а также простое. Суммарная численность работников, находившихся в простое по вине администрации, 
работавших неполное рабочее время, а также работников, которым были предоставлены отпуска по инициативе администрации, составила <b>{1:N0}</b>&nbsp;чел.<br/>",
                    factoryCount, incompleteEmployersCount);

                string str13;
                if (totalLastWeekDebts == 0 && totalDebts == 0)
                {
                    str13 = string.Format(@"&nbsp;&nbsp;&nbsp;По состоянию на <b>{0}</b> отсутствует задолженность по выплате 
заработной платы.<br/>", dateTimeDebtsStr);
                }
                else if (totalDebts == 0)
                {
                    str13 = string.Format(@"&nbsp;&nbsp;&nbsp;По состоянию на <b>{0}</b> отсутствует задолженность по выплате заработной платы.
Задолженность в сумме <b>{1:N3}</b>&nbsp;млн.руб. была погашена в период с <b>{2}</b> по <b>{0}</b>.<br/>",
    dateTimeDebtsStr, totalLastWeekDebts, dateLastTimeDebtsStr);
                }
                else
                {
                    str13 = string.Format(@"&nbsp;&nbsp;&nbsp;По состоянию на <b>{0}</b> задолженность по выплате заработной платы составляет 
<b>{1:N3}</b>&nbsp;млн.рублей (<b>{2:N0}</b>&nbsp;чел.). В период с <b>{4}</b> по <b>{0}</b> задолженность {3}.",
                        dateTimeDebtsStr, totalDebts, slavesCount, debtsPercentArrow, dateLastTimeDebtsStr);
                }

                string str14 = string.Format("&nbsp;&nbsp;&nbsp;Прогнозируемое значение численности безработных в целом по УрФО на <b>{0}&nbsp;{1}</b> года <b>{2:N0}</b> чел.", CRHelper.RusMonth(nextDateTime.Month), nextDateTime.Year, forecastValue);

                CommentText1.Text = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", str1, str2, str3, str4, str5, str6, str7, str8, str9, str10, str11, str12, str13);
                CommentText2.Text = string.Format("{0}", str14);
            }
        }

        private int GetKoeffNumber(string subjectName)
        {

            if (dtKoeff.Rows.Count != 0)
            {
                for (int i = 0; i < dtKoeff.Rows.Count; i++)
                {
                    DataRow row = dtKoeff.Rows[i];
                    if (row[0] != DBNull.Value && row[0].ToString() == subjectName)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private static DataTable GetSubjectKoeffTable()
        {
            DataTable dt = new DataTable();

            DataColumn column = new DataColumn("Субъект", typeof(string));
            dt.Columns.Add(column);

            for (int i = 0; i <= 13; i++)
            {
                string columnName = (i == 0) ? "Y-пересечение" : "Переменная X ";
                column = new DataColumn(columnName + i, typeof(string));
                dt.Columns.Add(column);
            }

            DataRow row = dt.NewRow();
            object[] array0 = { "Уральский федеральный округ", 4.834817, 0.011686, 0.056984, 0.087004, 0.093705, 0.084681, 0.049598, 0.013778, -0.00585,
                -0.02109, -0.04614, -0.05875, -0.04432};
            row.ItemArray = array0;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array1 = { "Челябинская область", 4.275802, 0.013926, 0.040292, 0.077282, 0.073687, 0.053920, 0.0079087, -0.010274,
                -0.025252, -0.045110, -0.077741, -0.097024, -0.067357};
            row.ItemArray = array1;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array2 =  { "Курганская область", 4.030716, 0.004315, 0.050866, 0.080746, 0.082402, 0.049926, 0.008242, -0.023601, -0.031912, -0.044864,
                -0.074787, -0.079989, -0.050917};
            row.ItemArray = array2;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array3 =  { "Свердловская область", 4.301695, 0.016367, 0.073546, 0.089745, 0.080245, 0.066612, 0.038165, 0.027910, 0.016237, 0.000114, -0.027503,
                -0.043127, -0.037974};
            row.ItemArray = array3;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array4 =  { "Тюменская область", 4.027350, -0.007477, 0.013083, 0.032511, 0.039706, 0.049971, 0.042602, 0.002576, -0.010114, -0.018256, -0.047603,
                -0.043513, -0.034087};
            row.ItemArray = array4;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array5 =  { "Ханты-Мансийский автономный округ", 4.021664, 0.004053, 0.055707, 0.080262, 0.077477, 0.069644, 0.033193, 0.003313, -0.024575,
                -0.040061, -0.047314, -0.049641, -0.038991};
            row.ItemArray = array5;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array6 =  { "Ямало-Ненецкий автономный округ", 3.676308, 0.001662, 0.045314, 0.075064, 0.069432, 0.048719, 0.009784,
                -0.069183, -0.130112, -0.124571, -0.106291, -0.079426, -0.051240};
            row.ItemArray = array6;
            dt.Rows.Add(row);

            return dt;
        }

        #endregion

        #region Экспорт в Excel

        private static string CommentTextExportsReplaces(string source)
        {
            string commentText = source;

            commentText = commentText.Replace("<\n>", "");
            commentText = commentText.Replace("<\r>", "");
            commentText = commentText.Replace(@"
", " ");
            commentText = commentText.Replace("&nbsp;", " ");
            commentText = commentText.Replace("<br/>", "\n");
            commentText = commentText.Replace("<b>", "");
            commentText = commentText.Replace("</b>", "");
            commentText = commentText.Replace("<img src=\"../../images/arrowRedDownBB.png\" width=\"13px\" height=\"16px\">", "");
            commentText = commentText.Replace("<img src=\"../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">", "");
            commentText = commentText.Replace("<img src=\"../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">", "");
            commentText = commentText.Replace("<img src=\"../../images/arrowGreenUpBB.png\" width=\"13px\" height=\"16px\">", "");
            commentText = commentText.Replace("<img src='../../images/arrowRedDownBB.png' width='13px' height='16px'>", "");
            commentText = commentText.Replace("<img src='../../images/arrowRedUpBB.png' width='13px' height='16px'>", "");
            commentText = commentText.Replace("<img src='../../images/arrowGreenDownBB.png' width='13px' height='16px'>", "");
            commentText = commentText.Replace("<img src='../../images/arrowGreenUpBB.png' width='13px' height='16px'>", "");

            return commentText;
        }

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = string.Format("по состоянию на {0:dd.MM.yyyy}",
                e.CurrentWorksheet == sheet1 ? currDateTime : debtsCurrDateTime);

            e.CurrentWorksheet.Rows[2].Cells[0].Value = CommentTextExportsReplaces(CommentText1.Text);
            e.CurrentWorksheet.Rows[3].Cells[0].Value = CommentTextExportsReplaces(CommentText2.Text);
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid1.Columns.Count;
            int rowsCount = UltraWebGrid1.Rows.Count;

            e.CurrentWorksheet.Columns[0].Width = 20*37;
            e.CurrentWorksheet.Columns[1].Width = 200*37;

            for (int i = 2; i < columnCount; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 60*37;
            }
            

            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount + 4; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            }

            if (!IsSmallResolution)
            {
                // расставляем стили у ячеек хидера
                for (int i = 1; i < columnCount; i++)
                {
                    e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                }
            }
        }

        private Workbook workbook;
        private Worksheet sheet1;
        private Worksheet sheet2;

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            workbook = new Workbook();
            sheet1 = workbook.Worksheets.Add("Sheet1");
            sheet2 = workbook.Worksheets.Add("Sheet2");

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;


            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1, sheet1);
            offset = 0;
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid2, sheet2);
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];

            while (col != null && col.Hidden)
            {
                offset++;
                col = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            }
            if (col != null)
            {
                e.HeaderText = col.Header.Key.Split(';')[0];
            }
        }

        #endregion

        #region Экспорт в PDF

        private static void SetExportGridParams(UltraWebGrid grid)
        {
            bool unit1 = false;
            bool unit4 = false;
            foreach (UltraGridRow row in grid.Rows)
            {
                // номер показателя
                int k = (row.Index % 3);
                UltraGridCell nameCell = row.Cells[1];
                UltraGridCell numberCell = row.Cells[0];
                switch (k)
                {
                    case 0:
                        {
                            if (!unit4 && numberCell.Value != null && numberCell.Value.ToString() == "4")
                            {
                                unit4 = true;
                            }
                            else if (!unit1 && numberCell.Value != null && numberCell.Value.ToString() == "1")
                            {
                                unit1 = true;
                            }
                            else
                            {
                                numberCell.Value = "";
                            }
                            numberCell.Style.BorderDetails.WidthBottom = 0;
                            nameCell.Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                    case 1:
                        {
                            if (numberCell.Value != null && numberCell.Value.ToString() == "1" ||
                                numberCell.Value != null && numberCell.Value.ToString() == "4")
                            {
                                numberCell.Value = "";
                            }
                            nameCell.Value = "темп прироста";
                            nameCell.Style.HorizontalAlign = HorizontalAlign.Right;
                            numberCell.Style.BorderDetails.WidthBottom = 0;
                            numberCell.Style.BorderDetails.WidthTop = 0;
                            nameCell.Style.BorderDetails.WidthBottom = 0;
                            nameCell.Style.BorderDetails.WidthTop = 0;
                            break;
                        }
                    case 2:
                        {
                            if (numberCell.Value != null && row.NextRow != null && row.NextRow.Cells[0].Value != null &&
                                ((numberCell.Value.ToString() == "4" && row.NextRow.Cells[0].Value.ToString() == "4") ||
                                 (numberCell.Value.ToString() == "1" && row.NextRow.Cells[0].Value.ToString() == "1")))
                            {
                                numberCell.Style.BorderDetails.WidthBottom = 0;
                            }

                            numberCell.Value = "";
                            if (nameCell.Value != null && nameCell.Value.ToString() != "ранг по УрФО")
                            {
                                nameCell.Value = "прирост";
                            }
                            nameCell.Style.HorizontalAlign = HorizontalAlign.Right;
                            numberCell.Style.BorderDetails.WidthTop = 0;
                            nameCell.Style.BorderDetails.WidthTop = 0;
                            break;
                        }
                }
            }
        }

        private static void SetExportMapParams(MapControl map)
        {
            map.ZoomPanel.Visible = false;
            map.NavigationPanel.Visible = false;
        }
        
        private bool titleAdded = false;
        private bool grid2Added = false;
        
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            SetExportGridParams(UltraWebGrid1);
            SetExportGridParams(UltraWebGrid2);
            SetExportMapParams(DundasMap1);
            SetExportMapParams(DundasMap2);
            
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = (i == 1)
                                                         ? CRHelper.GetColumnWidth(300)
                                                         : CRHelper.GetColumnWidth(60);
            }

            if (!titleAdded && !grid2Added)
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

                title = e.Section.AddText();
                font = new Font("Verdana", 12);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent("\n" + CommentTextExportsReplaces(CommentText1.Text) + "\n\n" +
                                 CommentTextExportsReplaces(CommentText2.Text) + "\n");
            }
            titleAdded = true;
        }

        private void PdfExporter_EndExport(object sender, EndExportEventArgs e)
        {
            if (titleAdded && !grid2Added)
            {
                grid2Added = true;
                UltraGridExporter1.PdfExporter.Export(UltraWebGrid2, e.Section);

                e.Section.AddPageBreak();
                IText title = e.Section.AddText();
                Font font = new Font("Verdana", 12);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent("\n" + mapElementCaption.Text);

                Image img = UltraGridExporter.GetImageFromMap(DundasMap1);
                e.Section.AddImage(img);

                e.Section.AddPageBreak();
                title = e.Section.AddText();
                font = new Font("Verdana", 12);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent("\n" + map2ElementCaption.Text);

                img = UltraGridExporter.GetImageFromMap(DundasMap2);
                e.Section.AddImage(img);
            }
        }

        #endregion
    }
}
