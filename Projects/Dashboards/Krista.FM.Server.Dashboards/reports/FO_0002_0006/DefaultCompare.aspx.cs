using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.IO;
using System.Drawing.Imaging;
using Infragistics.Documents.Excel;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0006
{
    public partial class DefaultCompare : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart1 = new DataTable();
        private DataTable dtChart2 = new DataTable();
        private DataTable dtRankRF = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";
        private bool internalCirculatoinExtrude = false;

        // тип расстановки рангов (MO/MR)
        private string rankingType;

        #region Параметры запроса

        // расходы Итого
        private CustomParam outcomesTotal;
        // уровень МР и ГО
        private CustomParam regionsLevel;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
        // выбранный район
        private CustomParam selectedRegion;
        // тип документа СКИФ для консолидированного уровня
        private CustomParam consolidateDocumentSKIFType;
        // тип документа СКИФ для уровня районов
        private CustomParam regionDocumentSKIFType;

        // элемент расходы всего
        private CustomParam outcomesTotalItem;

        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        // выбранный множитель рублей
        private CustomParam rubMultiplier;

        // множество МО для ранжирования
        private CustomParam rankingSet;
        // условие для фильтрации рангов
        private CustomParam rankingCondition;

        // тип документа СКИФ для местных бюджетов
        private CustomParam localBudgetDocumentSKIFType;
        // уровень бюджета СКИФ для местных бюджетов
        private CustomParam localBudgetSKIFLevel;

        // фильтр по КОСГУ
        private CustomParam kosguFilter;

        #endregion

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "Тыс.руб." : "Млн.руб."; }
        }

        private static bool IsMozilla
        {
            get { return HttpContext.Current.Request.Browser.Browser == "Firefox"; }
        }

        public string RankingSetCaption
        {
            get { return rankingType == "MR" ? "МР" : "МО"; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            outcomesTotal = UserParams.CustomParam("outcomes_total");
            regionsLevel = UserParams.CustomParam("regions_level");
            regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            selectedRegion = UserParams.CustomParam("selected_region");
            consolidateDocumentSKIFType = UserParams.CustomParam("consolidate_document_skif_type");
            regionDocumentSKIFType = UserParams.CustomParam("region_document_skif_type");
            regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            outcomesTotalItem = UserParams.CustomParam("outcomes_total_item");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");

            rankingSet = UserParams.CustomParam("ranking_set");
            rankingCondition = UserParams.CustomParam("ranking_condition");

            localBudgetDocumentSKIFType = UserParams.CustomParam("local_budget_document_skif_type");
            localBudgetSKIFLevel = UserParams.CustomParam("local_budget_skif_level");

            kosguFilter = UserParams.CustomParam("kosgu_filter");

            #endregion

            double scale = IsMozilla ? 0.5 : 0.4;

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale - 150);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);
            UltraWebGrid.DisplayLayout.ViewType = ViewType.Flat;

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.47 - 15);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 100);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.53 - 15);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 100);

            #region Настройка диаграмм

            UltraChart1.ChartType = ChartType.StackColumnChart;

            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;

            UltraChart1.StackChart.StackStyle = StackStyle.Complete;
            UltraChart1.Axis.Y.Labels.Visible = false;
            UltraChart1.Axis.X.Extent = 40;
            UltraChart1.Axis.X.Labels.SeriesLabels.WrapText = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8.5f);
            UltraChart1.Axis.Y.Extent = 10;
            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL> <DATA_VALUE:P2>";
            CRHelper.FillCustomColorModel(UltraChart1, 11, false);
            UltraChart1.InvalidDataReceived +=
                new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(
                    CRHelper.UltraChartInvalidDataReceived);

            UltraChart2.ChartType = ChartType.DoughnutChart;
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL> <DATA_VALUE:P2>";
            UltraChart2.DoughnutChart.Concentric = true;
            UltraChart2.DoughnutChart.OthersCategoryPercent = 0;
            UltraChart2.DoughnutChart.ShowConcentricLegend = false;
            UltraChart2.Data.SwapRowsAndColumns = true;
            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Left;
            UltraChart2.Legend.SpanPercentage = 38;
            UltraChart2.Legend.Margins.Bottom = 0;
            UltraChart2.Legend.Margins.Top = 0;
            UltraChart2.Legend.Margins.Left = 0;
            UltraChart2.Legend.Margins.Right = 0;
            CRHelper.CopyCustomColorModel(UltraChart1, UltraChart2);
            UltraChart2.InvalidDataReceived +=
                new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(
                    CRHelper.UltraChartInvalidDataReceived);

            UltraChart2.ColorModel.Skin.ApplyRowWise = true;

            CalloutAnnotation planAnnotation = new CalloutAnnotation();
            planAnnotation.Text = "Исполнено";
            planAnnotation.Width = 80;
            planAnnotation.Height = 20;
            planAnnotation.Location.Type = LocationType.Percentage;
            planAnnotation.Location.LocationX = 69;
            planAnnotation.Location.LocationY = 15;

            CalloutAnnotation factAnnotation = new CalloutAnnotation();
            factAnnotation.Text = "Назначено";
            factAnnotation.Width = 80;
            factAnnotation.Height = 20;
            factAnnotation.Location.Type = LocationType.Percentage;
            factAnnotation.Location.LocationX = 69;
            factAnnotation.Location.LocationY = 70;

            UltraChart2.Annotations.Add(planAnnotation);
            UltraChart2.Annotations.Add(factAnnotation);

            #endregion

            GridSearch1.LinkedGridId = this.UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting +=
                new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            UltraGridExporter1.MultiHeader = true;

            CrossLink1.Text = "Структура&nbsp;расходов&nbsp;по&nbsp;РЗПР&nbsp;и&nbsp;КОСГУ";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0008/Default.aspx";
            CrossLink2.Text = "Расходы&nbsp;бюджета";
            CrossLink2.NavigateUrl = "~/reports/FO_0002_0006/DefaultDetail.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));
            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.FKRAllLevel.Value = RegionSettingsHelper.Instance.FKRAllLevel;
            UserParams.FKRSectionLevel.Value = RegionSettingsHelper.Instance.FKRSectionLevel;
            UserParams.FKRSubSectionLevel.Value = RegionSettingsHelper.Instance.FKRSubSectionLevel;
            UserParams.FOFKRCulture.Value = RegionSettingsHelper.Instance.FOFKRCulture;
            UserParams.FOFKRHelthCare.Value = RegionSettingsHelper.Instance.FOFKRHelthCare;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            internalCirculatoinExtrude = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("InternalCirculationExtrude"));
            rankingType = RegionSettingsHelper.Instance.GetPropertyValue("RankingType");
            UserParams.RegionsLocalBudgetLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionsLocalBudgetLevel");

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(lbFO);
                chartWebAsyncPanel.AddRefreshTarget(lbSubject);
                chartWebAsyncPanel.AddRefreshTarget(lbFOSub);
                chartWebAsyncPanel.AddRefreshTarget(lbSubjectSub);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart2);
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0006_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                hiddenIndicatorLabel.Text = String.Format("{0}.[Консолидированный бюджет субъекта]", RegionSettingsHelper.Instance.RegionDimension);
                selectedRegion.Value = hiddenIndicatorLabel.Text;

                lbSubject.Text = String.Empty;
                lbFO.Text = String.Empty;
                lbSubjectSub.Text = String.Empty;
                lbFOSub.Text = String.Empty;
            }

            Page.Title = string.Format("Структура расходов");
            Label1.Text = Page.Title;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            Label2.Text = string.Format("Сравнение структуры расходов консолидированного бюджета субъекта РФ за {0} {1} {2} года по разделам классификации расходов", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            outcomesTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;
            outcomesTotalItem.Value = internalCirculatoinExtrude
                                          ? "Всего расходов без внутренних оборотов "
                                          : "Всего расходов c внутренними оборотами ";

            consolidateDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            localBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetDocumentSKIFType");
            localBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetSKIFLevel");

            kosguFilter.Value = RegionSettingsHelper.Instance.GetPropertyValue("KosguFilter");

            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";

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

            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();

                string patternValue = selectedRegion.Value;
                int defaultRowIndex = 0;
                if (patternValue == string.Empty)
                {
                    defaultRowIndex = 0;
                }

                if (UltraWebGrid.Columns.Count > 0 && UltraWebGrid.Rows.Count > 0)
                {
                    // ищем строку
                    UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, UltraWebGrid.Columns.Count - 1, defaultRowIndex);
                    // выделяем строку
                    ActivateGridRow(row);
                }
            }
        }

        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row">строка</param>
        private void ActivateGridRow(UltraGridRow row)
        {
            hiddenIndicatorLabel.Text = row.Cells[row.Cells.Count - 1].Text;
            selectedRegion.Value = hiddenIndicatorLabel.Text;

            lbSubject.Text = row.Cells[0].Text;
            lbSubjectSub.Text = "Сравнение плановой и фактической структуры";
            lbFOSub.Text = string.Empty;

            UltraChart1.DataBind();
            UltraChart2.DataBind();
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActivateGridRow(e.Row);
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0006_compare_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Бюджет", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 60;

                int j = (i - 1) % 4;
                switch (j)
                {
                    case 0:
                        {
                            formatString = "N2";
                            widthColumn = 120;
                            break;
                        }
                    case 1:
                        {
                            formatString = "P2";
                            widthColumn = 70;
                            break;
                        }
                    case 2:
                        {
                            formatString = "N0";
                            widthColumn = 65;
                            break;
                        }
                }

                e.Layout.Bands[0].Columns[i].Hidden = (j == 3);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 250;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;
            int lastColumnIndex = e.Layout.Bands[0].Columns.Count - 2;

            for (int i = 1; i < lastColumnIndex; i = i + 4)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0];

                string rankFOCaption;
                string rankFOHint;
                rankFOCaption = String.Format("Ранг {0}", RankingSetCaption);
                rankFOHint = String.Format("Место по доле расходов среди {0}", RankingSetCaption);

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, String.Format("Исполнено, {0}", RubMultiplierCaption.ToLower()), "Фактическое исполнение по разделу расходов");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Доля", "Доля раздела расхода в общей сумме расходов");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, rankFOCaption, rankFOHint);

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 3;
                ch.RowLayoutColumnInfo.SpanX = 3;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }

            ColumnHeader lastCh = new ColumnHeader(true);
            lastCh.Caption = e.Layout.Bands[0].Columns[lastColumnIndex].Header.Caption.Split(';')[0];

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, lastColumnIndex, String.Format("Исполнено, {0}", RubMultiplierCaption.ToLower()), "Фактическое исполнение по разделу расходов");

            lastCh.RowLayoutColumnInfo.OriginY = 0;
            lastCh.RowLayoutColumnInfo.OriginX = multiHeaderPos;
            lastCh.RowLayoutColumnInfo.SpanX = 1;
            e.Layout.Bands[0].HeaderLayout.Add(lastCh);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count - 1; i++)
            {
                int k = (i - 1) % 4;

                bool rankMO = (k == 2);

                if (e.Row.Cells[0].Value != null && (e.Row.Cells[0].Value.ToString().ToLower().Contains("бюджет") ||
                    e.Row.Cells[0].Value.ToString().ToLower().Contains("область") ||
                     (e.Row.Cells[0].Value.ToString().ToLower().Contains("городские округа") || e.Row.Cells[0].Value.ToString().ToLower().Contains("муниципальные районы"))))
                {
                    foreach (UltraGridCell cell in e.Row.Cells)
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
                else if (rankMO)
                {
                    bool param = Check1(e.Row, i);
                    string css = GetImg(e.Row, i, param);
                    e.Row.Cells[i].Style.BackgroundImage = css;
                    if (css != string.Empty)
                    {
                        e.Row.Cells[i].Title = (css == "~/images/starGrayBB.png")
                           ? String.Format("Самая низкая доля расходов среди {0}", RankingSetCaption)
                           : String.Format("Самая высокая доля расходов среди {0}", RankingSetCaption);
                    }
                }

                e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                UltraGridCell c = e.Row.Cells[i];

                if (c.Value != null && c.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(c.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            c.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        private static string GetImg(UltraGridRow row, int i, bool param)
        {
            if (param)
            {
                return "~/images/starGrayBB.png";
            }
            else if (Convert.ToInt32(row.Cells[i].Value) == 1)
            {
                return "~/images/starYellowBB.png";
            }
            return string.Empty;
        }

        private static bool Check1(UltraGridRow row, int i)
        {
            return row.Cells[i] != null && row.Cells[i + 1] != null &&
                   Convert.ToInt32(row.Cells[i].Value) == Convert.ToInt32(row.Cells[i + 1].Value) &&
                   Convert.ToInt32(row.Cells[i].Value) != 0;
        }

        #endregion

        #region Обработчики диаграмм

        private static bool NonZeroValueDataTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value && row[i].ToString() != string.Empty && Convert.ToDouble(row[i]) != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static string GetShortRzPrName(string fullName)
        {
            string shortName = fullName;

            switch (fullName)
            {
                case "ОБЩЕГОСУДАРСТВЕННЫЕ ВОПРОСЫ":
                    {
                        return "Общегосуд.вопросы";
                    }
                case "НАЦИОНАЛЬНАЯ ОБОРОНА":
                    {
                        return "Национальная оборона";
                    }
                case "НАЦИОНАЛЬНАЯ БЕЗОПАСНОСТЬ И ПРАВООХРАНИТЕЛЬНАЯ ДЕЯТЕЛЬНОСТЬ":
                    {
                        return "Нац.безопасность и правоохранит.деят.";
                    }
                case "НАЦИОНАЛЬНАЯ ЭКОНОМИКА":
                    {
                        return "Национальная экономика";
                    }
                case "ЖИЛИЩНО-КОММУНАЛЬНОЕ ХОЗЯЙСТВО":
                    {
                        return "ЖКХ";
                    }
                case "ОХРАНА ОКРУЖАЮЩЕЙ СРЕДЫ":
                    {
                        return "Охрана окруж.среды";
                    }
                case "ОБРАЗОВАНИЕ":
                    {
                        return "Образование";
                    }
                case "КУЛЬТУРА, КИНЕМАТОГРАФИЯ":
                    {
                        return "Культура и кинематография";
                    }
                case "КУЛЬТУРА, КИНЕМАТОГРАФИЯ, СРЕДСТВА МАССОВОЙ ИНФОРМАЦИИ":
                    {
                        return "Культура,  кинематография, СМИ";
                    }
                case "СРЕДСТВА МАССОВОЙ ИНФОРМАЦИИ":
                    {
                        return "СМИ";
                    }
                case "ЗДРАВООХРАНЕНИЕ":
                    {
                        return "Здравоохранение";
                    }
                case "ЗДРАВООХРАНЕНИЕ, ФИЗИЧЕСКАЯ КУЛЬТУРА И СПОРТ":
                    {
                        return "Здрав., физ.культура и спорт";
                    }
                case "ФИЗИЧЕСКАЯ КУЛЬТУРА И СПОРТ":
                    {
                        return "Физическая культура и спорт";
                    }
                case "СОЦИАЛЬНАЯ ПОЛИТИКА":
                    {
                        return "Социальная политика";
                    }
                case "МЕЖБЮДЖЕТНЫЕ ТРАНСФЕРТЫ":
                    {
                        return "Межбюджетные трансферты";
                    }
                case "МЕЖБЮДЖЕТНЫЕ ТРАНСФЕРТЫ ОБЩЕГО ХАРАКТЕРА БЮДЖЕТАМ СУБЪЕКТОВ РОССИЙСКОЙ ФЕДЕРАЦИИ И МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ":
                    {
                        return "МБТ бюджетам суб.РФ и МО";
                    }
                case "ОБСЛУЖИВАНИЕ ГОСУДАРСТВЕННОГО И МУНИЦИПАЛЬНОГО ДОЛГА":
                    {
                        return "Обслуж.гос.и мун.долга";
                    }
            }
            return shortName;
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0006_compare_chart1");
            dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Район", dtChart1);

            if (dtChart1.Rows.Count > 0 && NonZeroValueDataTable(dtChart1))
            {
                foreach (DataColumn column in dtChart1.Columns)
                {
                    column.ColumnName = GetShortRzPrName(column.ColumnName.ToUpper());
                }

                UltraChart1.DataSource = dtChart1;
            }
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0006_compare_chart2");
            dtChart2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Район", dtChart2);

            if (dtChart2.Rows.Count > 0 && NonZeroValueDataTable(dtChart2))
            {
                foreach (DataColumn column in dtChart2.Columns)
                {
                    column.ColumnName = GetShortRzPrName(column.ColumnName.ToUpper());
                }

                UltraChart2.DataSource = dtChart2;
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            // e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 1; i < UltraWebGrid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0.00;[Red]-#,##0.00";
                int widthColumn = 70;

                int j = (i - 1) % 3;
                switch (j)
                {
                    case 0:
                        {
                            formatString = "#,##0.00;[Red]-#,##0.00";
                            widthColumn = 95;
                            break;
                        }
                    case 1:
                        {
                            formatString = "0.00%";
                            widthColumn = 90;
                            break;
                        }
                    case 2:
                        {
                            formatString = "##";
                            widthColumn = 85;
                            break;
                        }
                }
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            e.HeaderText = col.Header.Key.Split(';')[0];
            if (col.Hidden || col.Index == 0)
            {
                offset++;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
           
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            sheet1.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
            sheet2.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
            sheet2.Rows[1].Cells[0].Value = lbSubject.Text;
            sheet2.Rows[2].Cells[0].Value = lbSubjectSub.Text;
            UltraGridExporter.ChartExcelExport(sheet2.Rows[4].Cells[0], UltraChart2);
            UltraGridExporter.ChartExcelExport(sheet2.Rows[4].Cells[13], UltraChart1);
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.HeaderCellHeight = 50;
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
            e.Section.AddPageBreak();
            ITable table = e.Section.AddTable();
            ITableRow row = table.AddRow();
            ITableCell cell = row.AddCell();

            IText title = cell.AddText();
            Font font = new Font("Verdana", 46);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = cell.AddText();
            font = new Font("Verdana", 44);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);

            title = cell.AddText();
            font = new Font("Verdana", 44);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(lbSubject.Text);

            title = cell.AddText();
            font = new Font("Verdana", 42);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(lbSubjectSub.Text);


            MemoryStream imageStream = new MemoryStream();
            UltraChart2.SaveTo(imageStream, ImageFormat.Png);
            Infragistics.Documents.Reports.Graphics.Image image = (new Bitmap(imageStream)).ScaleImageIg(5);

            cell.AddImage(image);


            //Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart2);
            //cell.Width = new FixedWidth((float)UltraChart2.Width.Value);
            //cell.AddImage(img);

            title = cell.AddText();
            font = new Font("Verdana", 42);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(lbFO.Text + " " + lbFOSub.Text);

            imageStream = new MemoryStream();
            UltraChart1.SaveTo(imageStream, ImageFormat.Png);
            image = (new Bitmap(imageStream)).ScaleImageIg(5);

            cell.AddImage(image);
            //img = UltraGridExporter.GetImageFromChart(UltraChart1);
            //cell.Width = new FixedWidth((float)UltraChart1.Width.Value);
            //cell.AddImage(img);

        }

        #endregion
    }
}