using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.reports.FO_0006_0001
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear;
        private int endYear = 2011;
        private string month = "Январь";
        private string meas = string.Empty;
        private CustomParam mul;
        private bool internalCirculatoinExtrude = false;
        private static MemberAttributesDigest okvedDigest;
        // вычислители рангов
        RankCalculator mrRank = new RankCalculator(RankDirection.Desc);
        RankCalculator goRank = new RankCalculator(RankDirection.Desc);
        private bool GrowRateRanking
        {
            get { return Convert.ToBoolean(growRateRanking.Value); }
        }

        #region Параметры запроса

        // доходы Итого
        private CustomParam incomesTotal;
        // уровень МР и ГО
        private CustomParam regionsLevel;
        // выбранный регион
        private CustomParam selectedRegion;
        // предыдущий год
        private CustomParam predYear;
        // позапрошлый год
        private CustomParam predpredYear;
        // группа доходов
        private CustomParam fnsKDGroup;

        // тип документа СКИФ для консолидированного бюджета субъекта
        private CustomParam consolidateBudgetDocumentSKIFType;
        // тип документа СКИФ для районов
        private CustomParam regionBudgetDocumentSKIFType;
        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        // тип документа СКИФ для местных бюджетов
        private CustomParam localBudgetDocumentSKIFType;
        // уровень бюджета СКИФ для местных бюджетов
        private CustomParam localBudgetSKIFLevel;

        // консолидированный бюджет субъекта
        private CustomParam regionsConsolidateBudget;

        // выводить ранги для темпа роста
        private CustomParam growRateRanking;

        // элемент доходы итого
        private CustomParam incomesTotalItem;
        // элемент безвозмездные поступления
        private CustomParam gratuitousIncomesItem;

        // выбранный множитель рублей
        private CustomParam rubMultiplier;

        // уровень бюджета
        private CustomParam level;

        // уровень бюджета
        private CustomParam OKVDGroup;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (mul == null)
            {
                mul = UserParams.CustomParam("mul");
            }

            if (level == null)
            {
                level = UserParams.CustomParam("level");
            }
            if (fnsKDGroup == null)
            {
                fnsKDGroup = UserParams.CustomParam("fns_kd_group");
            }
            if (OKVDGroup == null)
            {
                OKVDGroup = UserParams.CustomParam("selected_OKVD");
            }
            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (predYear == null)
            {
                predYear = UserParams.CustomParam("predYear");
            }
            if (predpredYear == null)
            {
                predpredYear = UserParams.CustomParam("predpredYear");
            }
            incomesTotal = UserParams.CustomParam("incomes_total");
            regionsLevel = UserParams.CustomParam("regions_level");
            consolidateBudgetDocumentSKIFType = UserParams.CustomParam("consolidate_budget_document_skif_type");
            regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            regionsConsolidateBudget = UserParams.CustomParam("regions_consolidate_budget");
            growRateRanking = UserParams.CustomParam("grow_rate_ranking");
            incomesTotalItem = UserParams.CustomParam("incomes_total_item");
            gratuitousIncomesItem = UserParams.CustomParam("gratuitous_incomes_item");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            localBudgetDocumentSKIFType = UserParams.CustomParam("local_budget_document_skif_type");
            localBudgetSKIFLevel = UserParams.CustomParam("local_budget_skif_level");

            #endregion

            growRateRanking.Value = RegionSettingsHelper.Instance.GetPropertyValue("GrowRateRanking");

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth );
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.675);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.InitializeLayout +=
                new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport +=
                new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.MultiHeader = true;

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            CrossLink1.Text = "Темп роста доходов бюджетов муниципальных образований";
            CrossLink1.NavigateUrl = "";
            CrossLink1.Visible = false;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            firstYear = 2009;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0006_0001_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState((endYear).ToString(), true);

                ComboMonth.Title = "Месяц";

                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month, true);

            }
            
            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            DateTime currDateTime = new DateTime(Convert.ToInt32(ComboYear.SelectedValue.ToString()), CRHelper.MonthNum(ComboMonth.SelectedValue), 01);
            currDateTime = currDateTime.AddMonths(1);
            string incom = string.Empty;
            string date = String.Format("{0:dd.MM.yyyy}", currDateTime);
            if (RadioList.SelectedIndex == 0)
            {
                mul.Value = Convert.ToString(1000);
                meas = "тыс.руб";
            }
            else
            {
                mul.Value = Convert.ToString(1000000);
                meas = "млн.руб";
            }
            Page.Title = "Справка об исполнении бюджетов районов и городов края";
            Label1.Text = String.Format("Справка об исполнении бюджетов районов и городов края, по состоянию на 01.{0:MM.yyyy}г., {1}", currDateTime, meas);
            Label2.Text = string.Empty;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            predYear.Value = Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 1);
            predpredYear.Value = Convert.ToString(Convert.ToInt32(ComboYear.SelectedValue) - 2);
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");
            localBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetDocumentSKIFType");
            localBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetSKIFLevel");
            regionsConsolidateBudget.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesTotalItem.Value = internalCirculatoinExtrude
                  ? "Доходы бюджета без внутренних оборотов "
                  : "Доходы бюджета c внутренними оборотами ";
            gratuitousIncomesItem.Value = internalCirculatoinExtrude
                  ? "Безвозмездные поступления без внутренних оборотов "
                  : "Безвозмездные поступления c внутренними оборотами ";

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        private bool IsRankingRow(string rowName)
        {
            return rowName != "Консолидированный бюджет" && rowName != "Бюджет субъекта" &&
                rowName != "Городские округа" && rowName != "Муниципальные районы" &&
                rowName != RegionSettingsHelper.Instance.OwnSubjectBudgetName;
        }


        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
   
                string query = DataProvider.GetQueryText("FO_0006_0001_compare_grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);
                const string ExecuteColumnName = "Всего";
                const string ExecuteRankColumnName = "Ранг по абсолютной сумме недоимки";
                const string WorseRankColumnName = "Худший Ранг по абсолютной сумме недоимки";
                dtGrid.Columns.RemoveAt(0);
                dtGrid.AcceptChanges();
                if (dtGrid.Rows.Count > 0)
                {
                    if (dtGrid.Columns.Count > 1 && dtGrid.Rows.Count > 0)
                {
                    //dtGrid.PrimaryKey = new DataColumn[] { dtGrid.Columns[0] };

                    foreach (DataRow row in dtGrid.Rows)
                    {
                        if (row[0] != DBNull.Value)
                        {
                            string rowName = row[0].ToString();
                           
                                if (row != null)
                                {

                                    if (GetRankCalculator(row) != null)
                                    {
                                        RankCalculator ExecuteRank = GetRankCalculator(row);
                                        if (row[ExecuteColumnName] != DBNull.Value && row[ExecuteColumnName].ToString() != String.Empty)
                                        {
                                            double execute = Convert.ToDouble(row[ExecuteColumnName]);
                                            if (IsRankingRow(rowName))
                                            {
                                                ExecuteRank.AddItem(execute);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (DataRow row in dtGrid.Rows)
                    {
                        string rowName = row[0].ToString();
                        if (IsRankingRow(rowName) && row[ExecuteColumnName] != DBNull.Value && row[ExecuteColumnName].ToString() != String.Empty)
                        {
                            double value = Convert.ToDouble(row[ExecuteColumnName]);
                            if (GetRankCalculator(row) != null)
                            {
                                RankCalculator ExecuteRank = GetRankCalculator(row);
                                double ExecuteWorseRank = ExecuteRank.GetWorseRank();
                                int rank = ExecuteRank.GetRank(value);
                                if (rank != 0)
                                {
                                    row[ExecuteRankColumnName] = rank;
                                    row[WorseRankColumnName] = ExecuteWorseRank;
                                }
                            }
                        }
                        else
                        {
                            row[ExecuteRankColumnName] = DBNull.Value;
                        }
                    }
                }
            UltraWebGrid.DataSource = dtGrid;
        }


        private RankCalculator GetRankCalculator(DataRow row)
        {
            if (row[0].ToString().Contains("район") && !row[0].ToString().Contains("районы") && !row[0].ToString().Contains("районам"))
            {
                return mrRank;
            }else
                if (row[0].ToString().Contains("г.") || row[0].ToString().Contains("ЗАТО"))
                {
                    return goRank;
                }
            return null;
        }




        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            //if (UltraWebGrid != null && UltraWebGrid.Rows.Count < 30)без прокрутки
            //{
            //    UltraWebGrid.Height = Unit.Empty;
            //}
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(260);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 3;
            e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.SpanY = 3;
            for (int k = 1; k < e.Layout.Bands[0].Columns.Count; k++)
            {
                string formatString = "N0";
                if ((k == 4) || (k == 5) || (k == 6) || (k == 7) || (k == 17) || (k == 18))
                {
                    formatString = "N0";
                }
                if (k == 9)
                {
                    formatString = "N0";
                }
                e.Layout.Bands[0].Columns[k].Format = formatString;
                e.Layout.Bands[0].Columns[k].Width = 130;
                e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
                e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.OriginY = 1;
                e.Layout.Bands[0].Columns[k].Header.RowLayoutColumnInfo.SpanY = 2;
            }
                e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
                        DateTime currDateTime = new DateTime(Convert.ToInt32(ComboYear.SelectedValue.ToString()), CRHelper.MonthNum(ComboMonth.SelectedValue), 01);
                  string date = String.Format("{0:dd.MM.yyyy}", currDateTime);
                  currDateTime = currDateTime.AddMonths(1);
                  e.Layout.Bands[0].Columns[1].Header.Caption = string.Format("Прогноз на {0}г. крайфо", ComboYear.SelectedValue);
                  e.Layout.Bands[0].Columns[2].Header.Caption = string.Format("Прогноз на {0}г. обратный", ComboYear.SelectedValue);
                  e.Layout.Bands[0].Columns[3].Header.Caption = string.Format("Исполнено янв.-{0}.{1}г.", ComboMonth.SelectedValue.ToLower().Substring(0,3), ComboYear.SelectedValue);
                  e.Layout.Bands[0].Columns[14].Header.Caption = string.Format("Исполнено янв.-{0}.{1}г.", ComboMonth.SelectedValue.ToLower().Substring(0, 3), Convert.ToInt32(ComboYear.SelectedValue) - 2);
                  e.Layout.Bands[0].Columns[15].Header.Caption = string.Format("Исполнено янв.-{0}.{1}г.", ComboMonth.SelectedValue.ToLower().Substring(0, 3), Convert.ToInt32(ComboYear.SelectedValue) - 1);
                  e.Layout.Bands[0].Columns[4].Header.Caption = string.Format("Исполнение прогноза крайфо, %");
                  e.Layout.Bands[0].Columns[5].Header.Caption = string.Format("Исполнение обратного прогноза, %");
                  e.Layout.Bands[0].Columns[6].Header.Caption = string.Format("% роста к {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 1);
                  e.Layout.Bands[0].Columns[7].Header.Caption = string.Format("% роста к {0} году", Convert.ToInt32(ComboYear.SelectedValue) - 2);
                  e.Layout.Bands[0].Columns[16].Header.Caption = e.Layout.Bands[0].Columns[3].Header.Caption;
                  e.Layout.Bands[0].Columns[17].Header.Caption = e.Layout.Bands[0].Columns[6].Header.Caption;
                  e.Layout.Bands[0].Columns[18].Header.Caption = e.Layout.Bands[0].Columns[7].Header.Caption;
                  e.Layout.Bands[0].Columns[1].Header.Title = "Годовые прогнозные данные финансового органа субъекта";
                  e.Layout.Bands[0].Columns[2].Header.Title = "Ежемесячные годовые назначения финансовых органов муниципальных образований";
                  e.Layout.Bands[0].Columns[3].Header.Title = "Фактическое исполнение бюджета в текущем периоде";
                  e.Layout.Bands[0].Columns[4].Header.Title = "Процент исполнения прогнозных данных в сравнении с фактическим исполнением";
                  e.Layout.Bands[0].Columns[5].Header.Title = "Процент исполнения годовых назначений в сравнении с фактическим исполнением";
                  e.Layout.Bands[0].Columns[6].Header.Title = "Темп роста фактического исполнения в текущем периоде к аналогичному периоду прошлого года";
                  e.Layout.Bands[0].Columns[7].Header.Title = "Темп роста фактического исполнения в текущем периоде к аналогичному периоду позапрошлого года";
                  e.Layout.Bands[0].Columns[14].Header.Title = "Фактическое исполнение бюджета за аналогичный период позапрошлого года";
                  e.Layout.Bands[0].Columns[15].Header.Title = "Фактическое исполнение бюджета за аналогичный период прошлого года";
                  e.Layout.Bands[0].Columns[16].Header.Title = "Фактическое исполнение бюджета в текущем периоде с начала года";
                  e.Layout.Bands[0].Columns[17].Header.Title = "Темп роста фактического исполнения в текущем периоде к аналогичному периоду прошлого года";
                  e.Layout.Bands[0].Columns[18].Header.Title = "Темп роста фактического исполнения в текущем периоде к аналогичному периоду позапрошлого года";
                  int spanOX = 1;
                  int spanSX = 6;
                  ColumnHeader ch = new ColumnHeader(true);
                  ch.Caption = "Собственные доходы";
                  ch.Style.Font.Size = 11;
                  ch.Title = "";
                  ch.RowLayoutColumnInfo.OriginY = 0;
                  ch.RowLayoutColumnInfo.OriginX = spanOX;
                  ch.RowLayoutColumnInfo.SpanX = 7;
                  ch.Style.Wrap = true;
                  e.Layout.Bands[0].HeaderLayout.Add(ch);
                  spanOX = 7 + spanOX;
                  ch = new ColumnHeader(true);
                  ch.Caption = "Недоимка";
                  ch.Style.Font.Size = 11;
                  ch.Title = "";
                  ch.RowLayoutColumnInfo.OriginY = 0;
                  ch.RowLayoutColumnInfo.OriginX = spanOX;
                  ch.RowLayoutColumnInfo.SpanX = spanSX;
                  ch.Style.Wrap = true;
                  e.Layout.Bands[0].HeaderLayout.Add(ch);
                  spanOX = spanSX + spanOX;
                  ch = new ColumnHeader(true);
                  ch.Caption = "Собственные доходы без продажи имущества";
                  ch.Style.Font.Size = 11;
                  ch.Title = "";
                  ch.RowLayoutColumnInfo.OriginY = 0;
                  ch.RowLayoutColumnInfo.OriginX = spanOX;
                  ch.RowLayoutColumnInfo.SpanX = spanSX - 1;
                  ch.Style.Wrap = true;
                  e.Layout.Bands[0].HeaderLayout.Add(ch);                  

        }


        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 4; (i < UltraWebGrid.Columns.Count); i += 1)
            {
                if (((i > 5)&&(i < 8))||((i == UltraWebGrid.Columns.Count - 2)||(i == UltraWebGrid.Columns.Count - 3)))
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Снижение поступлений";
                        }
                        else if (Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Рост поступлений";
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }
                if (i == 9)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[UltraWebGrid.Columns.Count - 1].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[UltraWebGrid.Columns.Count - 1].Value.ToString() != string.Empty)
                    {

                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = "Наименьший абсолютный прирост недоимки";
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[UltraWebGrid.Columns.Count - 1].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = "Наибольший абсолютный прирост недоимки";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                if ((i == 4)||(i == 5))
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double percent = (ComboMonth.SelectedIndex + 1) * 100 / 12;

                        if (Convert.ToDouble(e.Row.Cells[i].Value) < percent)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerRed.gif";
                            e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:N2}%)", percent);
                        }
                        else
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerGreen.gif";
                            e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:N2}%)", percent);
                        }
                        e.Row.Cells[i].Style.Padding.Right = 2;
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; margin: 0px";
                    }
                }
            }

            
            int levelColumnIndex = e.Row.Cells.Count - 1;
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[0] != null && e.Row.Cells[0].Value.ToString() != string.Empty)
                {
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;
                    if ((e.Row.Cells[0].ToString().Contains("Бюджет")) || (e.Row.Cells[0].ToString().Contains("Всего")) || (e.Row.Cells[0].ToString().Contains("округа")))
                    {

                        bold = true;
                        italic = false;

                    }

                    e.Row.Cells[i].Style.Font.Size = fontSize;
                    e.Row.Cells[i].Style.Font.Bold = bold;
                    e.Row.Cells[i].Style.Font.Italic = italic;
                }
            }
        }

        private static string TrimName(string name)
        {
            while (Char.IsDigit(name[0]))
            {
                name = name.Remove(0, 1);
            }
            return name;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            string label = Label2.Text.Replace("<br/>", "");
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 65;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = label;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            
            int columnCount = UltraWebGrid.Columns.Count;
            int width = 150;
            e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.CurrentWorksheet.Columns[0].Width = width * 30 * 3;
            e.CurrentWorksheet.Columns[1].Width = width * 30;
            e.CurrentWorksheet.Columns[2].Width = width * 30;
            e.CurrentWorksheet.Columns[3].Width = width * 30;
            e.CurrentWorksheet.Columns[4].Width = width * 30;
            e.CurrentWorksheet.Columns[5].Width = width * 30;
            e.CurrentWorksheet.Columns[6].Width = width * 30;
            e.CurrentWorksheet.Columns[7].Width = width * 30;
            e.CurrentWorksheet.Columns[8].Width = width * 30;
            e.CurrentWorksheet.Columns[9].Width = width * 30;
            e.CurrentWorksheet.Columns[10].Width = width * 30;
            e.CurrentWorksheet.Columns[11].Width = width * 30;
            e.CurrentWorksheet.Columns[12].Width = width * 30;
            e.CurrentWorksheet.Columns[13].Width = width * 30;
            e.CurrentWorksheet.Columns[14].Width = width * 30;
            e.CurrentWorksheet.Columns[15].Width = width * 30;
            e.CurrentWorksheet.Columns[16].Width = width * 30;
            e.CurrentWorksheet.Columns[17].Width = width * 30;
            e.CurrentWorksheet.Columns[18].Width = width * 30;
            e.CurrentWorksheet.Columns[19].Width = width * 30;
            int columnCountt = UltraWebGrid.Columns.Count;

            e.CurrentWorksheet.Rows[6].Cells[0].CellFormat.FillPattern = FillPatternStyle.None;
            int columnCounttt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCounttt; i = i + 1)
            {
                if ((i == 4) || (i == 5) || (i == 6) || (i == 7) || (i == 17) || (i == 18))
                {
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#0;[Red]-#0";
                }
                else
                {
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#0;[Red]-#0";
                }
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
            }
            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "#0;[Red]-#0"; ;
            for (int k = 1; k < columnCounttt; k = k + 1)
            {
                e.CurrentWorksheet.Rows[3].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[4].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }
            e.CurrentWorksheet.Rows[4].Height = 600;

        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {

                if ((e.CurrentColumnIndex > 0) && (e.CurrentColumnIndex < 8))
                {
                    e.HeaderText = "Собственные доходы";
                }
                else
                    if ((e.CurrentColumnIndex > 7) && (e.CurrentColumnIndex < 14))
                    {
                        e.HeaderText = "Недоимка";
                    }
                    else
                        if ((e.CurrentColumnIndex > 13) && (e.CurrentColumnIndex < 19))
                        {
                            e.HeaderText = "Собственные доходы без продажи имущества";
                        }
                        
            
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Доходы");
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
        }

        #endregion

        #region Экспорт в PDF

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
            title.AddContent(Label2.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
        }

        #endregion
        
        public int sts { get; set; }
    }

         
}
