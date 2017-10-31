using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core.MemberDigests;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0008_Settlement
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart;
        private int firstYear = 2000;
        private int endYear = 2011;

        #endregion

        #region Параметры запроса

        // выбранный регион
        private CustomParam selectedRegion;
        // Тип документа
        private CustomParam documentType;
        // Консолидированный уровень
        private CustomParam consolidateLevel;
        // Уровень бюджета СКИФ
        private CustomParam budgetSKIFLevel;
        // Расходы ФКР Всего
        private CustomParam outcomesFKRTotal;
        // для этого года не выводить колонки сравнения
        private CustomParam isNonCompareYear;
        // фильтр для исключаемых элементов РзПр
        private CustomParam rzprExtrudeFilter;

        private CustomParam rubMultiplier;

        #endregion

        private GridHeaderLayout headerLayout;
        private DateTime currentDate;

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "Тыс.руб." : "Млн.руб."; }
        }

        private bool isFKR
        {
            get { return OutcomesButtonList.SelectedIndex == 0; }
        }

        private bool IsNonCompareYear
        {
            get { return currentDate.Year == 2011 && isFKR; }
        }

        private MemberAttributesDigest budgetDigest;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.37 - 140);

            UltraChart.Width = CRHelper.GetChartWidth(2 * CustomReportConst.minScreenWidth / 3 - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.59 - 120);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 3 - 25);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.59 - 120);

            #region Инициализация параметров запроса

            selectedRegion = UserParams.CustomParam("selected_region");
            documentType = UserParams.CustomParam("document_type");
            consolidateLevel = UserParams.CustomParam("consolidate_level");
            budgetSKIFLevel = UserParams.CustomParam("budget_SKIF_level");
            outcomesFKRTotal = UserParams.CustomParam("outcomes_fkr_total");
            isNonCompareYear = UserParams.CustomParam("is_non_compare_year");
            rzprExtrudeFilter = UserParams.CustomParam("rzpr_extrude_filter");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.PieChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.PieChart.ColumnIndex = 1;
            UltraChart.PieChart.OthersCategoryPercent = 0;
            UltraChart.Tooltips.FormatString = String.Format("<ITEM_LABEL>\nплан <DATA_VALUE:N2> {0}\nдоля <PERCENT_VALUE:N2>%", RubMultiplierCaption.ToLower());
            UltraChart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Left;
            UltraChart.Legend.SpanPercentage = 32;
            UltraChart.Legend.Margins.Top = 0;

            CRHelper.FillCustomColorModel(UltraChart, 17, false);
            UltraChart.ColorModel.Skin.ApplyRowWise = true;

            UltraChart.TitleTop.Visible = true;
            UltraChart.TitleTop.Text = "План";
            UltraChart.TitleTop.Margins.Top = 2;
            UltraChart.TitleTop.Margins.Bottom = 5;
            UltraChart.TitleTop.Margins.Left = Convert.ToInt32(UltraChart.Legend.SpanPercentage * UltraChart.Width.Value / 100) + 5;
            UltraChart.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleTop.Font = new Font("Verdana", 10, FontStyle.Bold);
            UltraChart.TitleTop.Extent = 20;

            UltraChart2.TitleTop.Visible = true;
            UltraChart2.TitleTop.Text = "Факт";
            UltraChart2.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleTop.Font = new Font("Verdana", 10, FontStyle.Bold);
            UltraChart2.TitleTop.Extent = 20;

            UltraChart2.ChartType = ChartType.PieChart;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.PieChart.ColumnIndex = 2;
            UltraChart2.PieChart.OthersCategoryPercent = 0;
            UltraChart2.Tooltips.FormatString = String.Format("<ITEM_LABEL>\nфакт <DATA_VALUE:N2> {0}\nдоля <PERCENT_VALUE:N2>%", RubMultiplierCaption.ToLower());
            CRHelper.CopyCustomColorModel(UltraChart, UltraChart2);
            UltraChart2.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            //firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));
            firstYear = 2008;
            rzprExtrudeFilter.Value = RegionSettingsHelper.Instance.GetPropertyValue("RzPrExtrudeFilter");

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.FKRAllLevel.Value = RegionSettingsHelper.Instance.FKRAllLevel;
            UserParams.FKRSectionLevel.Value = RegionSettingsHelper.Instance.FKRSectionLevel;
            UserParams.FKRSubSectionLevel.Value = RegionSettingsHelper.Instance.FKRSubSectionLevel;
            UserParams.EKRDimension.Value = RegionSettingsHelper.Instance.EKRDimension;
            UserParams.FOFKRCulture.Value = RegionSettingsHelper.Instance.FOFKRCulture;
            UserParams.FOFKRHelthCare.Value = RegionSettingsHelper.Instance.FOFKRHelthCare;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.RzPrInternalCircualtionExtruding.Value = RegionSettingsHelper.Instance.RzPrInternalCircualtionExtruding;

            budgetDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0008_Settlement_Digest");

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0008_Settlement_date");
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

                ComboBudgetLevel.Title = "Бюджет поселения";
                ComboBudgetLevel.Width = 400;
                ComboBudgetLevel.MultiSelect = false;
                ComboBudgetLevel.ParentSelect = true;
                ComboBudgetLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetDigest.UniqueNames, budgetDigest.MemberLevels));
                ComboBudgetLevel.SetСheckedState("Консолидированный бюджет субъекта", true);
            }

            selectedRegion.Value = budgetDigest.GetMemberUniqueName(ComboBudgetLevel.SelectedValue);
            outcomesFKRTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;
            budgetSKIFLevel.Value = "[Уровни бюджета__СКИФ].[Уровни бюджета__СКИФ].[Все].[Бюджет поселения]";
            documentType.Value = "[ТипДокумента__СКИФ].[ТипДокумента__СКИФ].[Все].[Собственный отчет поселения]";
            CRHelper.SaveToErrorLog(budgetDigest.GetMemberLevel(ComboBudgetLevel.SelectedValue));
            string settlement = string.Empty;
            if (ComboBudgetLevel.SelectedNode.Level == 0)
            {
                settlement = string.Format("{0}, все поселения", ComboBudgetLevel.SelectedValue);
            }
            else
            {
                settlement = string.Format("{0}, {1}", ComboBudgetLevel.SelectedNodeParent, ComboBudgetLevel.SelectedValue);
            }
            Page.Title = String.Format("Структура расходов {1}: {0}", settlement, isFKR ? "по разделам бюджетной классификации" : "в разрезе КОСГУ");
            PageTitle.Text = Page.Title;
            chartHeaderLabel.Text = "Сравнение плановой и фактической структуры расходов";

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, monthNum, 1);
            isNonCompareYear.Value = IsNonCompareYear.ToString();

            PageSubTitle.Text = String.Format("Оценка плановой и фактической структуры расходов за {0} {1} {2} года",
                currentDate.Month, CRHelper.RusManyMonthGenitive(currentDate.Month), currentDate.Year);
            CommentTextLabel.Text = IsNonCompareYear
                                        ? "В связи с изменением с 01.01.2011 года бюджетной классификации в соответствии с Приказом Минфина РФ №190н от 28.12.2010 года приводить сравнение расходов в разрезе РзПр за 2011 год с предыдущими годами некорректно."
                                        : String.Empty;

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);

            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            UltraChart.DataBind();
            UltraChart2.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = isFKR ? DataProvider.GetQueryText("FO_0002_0008_Settlement_grid_FKR") : DataProvider.GetQueryText("FO_0002_0008_Settlement_grid_KOSGU");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if ((i == 1) && row[i] != DBNull.Value && row[i].ToString().Length > 4)
                    {
                        row[i] = row[i].ToString().Substring(0, 4);
                    }
                }
            }
            decimal Sum = 0, Sum1 = 0, Sum3 = 0;
            for (int i = 0; i <= dtGrid.Rows.Count - 2; i++)
            {
                object val1 = dtGrid.Rows[i][2];
                if (val1 != DBNull.Value)
                {
                    Sum += (decimal)val1;
                }
                val1 = dtGrid.Rows[i][3];
                if (val1 != DBNull.Value)
                {
                    Sum1 += (decimal)val1;
                }
                val1 = dtGrid.Rows[i][6];
                if (val1 != DBNull.Value)
                {
                    Sum3 += (decimal)val1;
                }
            }
            dtGrid.Rows[dtGrid.Rows.Count - 1][2] = Sum;
            dtGrid.Rows[dtGrid.Rows.Count - 1][3] = Sum1;
            dtGrid.Rows[dtGrid.Rows.Count - 1][6] = Sum3;
            if (Sum != 0)
            {
                dtGrid.Rows[dtGrid.Rows.Count - 1][4] = Sum1 / Sum;
            }
            UltraWebGrid.DataSource = dtGrid;
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

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            //            if (!Page.IsPostBack)
            //            {
            UltraGridColumn column = e.Layout.Bands[0].Columns[0];
            e.Layout.Bands[0].Columns.RemoveAt(0);
            e.Layout.Bands[0].Columns.Insert(1, column);
            //            }

            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(330);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            SetColumnParams(e.Layout, 0, 0, isFKR ? "00 00" : "N0", 50, false);

            int columnCount = e.Layout.Bands[0].Columns.Count;
            for (int i = 2; i < columnCount - 2; i = i + 1)
            {
                string formatString = GetColumnFormat(e.Layout.Bands[0].Columns[i].Header.Caption);
                int widthColumn = GetColumnWidth(e.Layout.Bands[0].Columns[i].Header.Caption);

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;

            headerLayout.AddCell("Код", isFKR ? "Код РзПз" : "Код по КОСГУ");
            headerLayout.AddCell("Наименование", isFKR ? "Перечень разделов классификации расходов" : "Перечень статей расходов по КОСГУ");

            GridHeaderCell groupCell = headerLayout.AddCell(String.Format("на {0:dd.MM.yyyy} г.", currentDate.AddMonths(1)));

            groupCell.AddCell(String.Format("Уточненные годовые назначения, {0}", RubMultiplierCaption.ToLower()), String.Format("Годовые назначения расходов на {0} год", currentDate.Year));
            groupCell.AddCell(String.Format("Факт, {0}", RubMultiplierCaption.ToLower()), String.Format("Исполнено {0}", groupCell.Caption));
            groupCell.AddCell("% исполнения", "% исполнения годовых назначений расходов");
            groupCell.AddCell("Ранг % исп.", "Ранг (место) статьи по % исполнения годовых назначений расходов");
            groupCell.AddCell("Доля", "Удельный вес статьи фактических расходов в общей сумме расходов");
            groupCell.AddCell("Ранг доля", "Ранг (место) статьи по удельному весу фактических расходов в общей сумме расходов");

            if (!IsNonCompareYear)
            {
                GridHeaderCell compareGroupCell = headerLayout.AddCell(String.Format("{0} год к {1} году", currentDate.Year, currentDate.Year - 1));
                compareGroupCell.AddCell(String.Format("Отклонение(+,-), {0}", RubMultiplierCaption.ToLower()), "");
                compareGroupCell.AddCell("Темп роста, %", "");
                compareGroupCell.AddCell("Рост (снижение)", "");
            }

            headerLayout.ApplyHeaderInfo();
        }

        private static string GetColumnFormat(string columnName)
        {
            if (columnName.ToLower().Contains("план") || columnName.ToLower().Contains("факт") || columnName.ToLower().Contains("отклонение"))
            {
                return "N2";
            }
            if (columnName.ToLower().Contains("ранг"))
            {
                return "N0";
            }
            return "P2";
        }

        private int GetColumnWidth(string columnName)
        {
            double widthMultiplier = IsNonCompareYear ? 1.55 : 1;
            if (columnName.ToLower().Contains("план") || columnName.ToLower().Contains("факт") || columnName.ToLower().Contains("отклонение"))
            {
                return Convert.ToInt32(widthMultiplier * 100);
            }
            if (columnName.ToLower().Contains("ранг"))
            {
                return Convert.ToInt32(widthMultiplier * 50);
            }
            return Convert.ToInt32(widthMultiplier * 80);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int columnCount = e.Row.Cells.Count;
            for (int i = 0; i < columnCount - 2; i++)
            {
                string columnName = e.Row.Band.Columns[i].Header.Caption.ToLower();

                bool rank = columnName.Contains("ранг");
                bool rate = columnName.Contains("темп роста");

                if (rank)
                {
                    bool isShareRank = columnName.Contains("доля");
                    int worseRankColumnIndex = isShareRank ? columnCount - 1 : columnCount - 2;
                    string obj = isShareRank ? "удельный вес" : "процент исполнения";
                    string best = isShareRank ? "Максимальный" : "Самый большой";
                    string pour = isShareRank ? "Минимальный" : "Самый маленький";

                    if (e.Row.Cells[i].Value != null && e.Row.Cells[worseRankColumnIndex].Value != null &&
                        e.Row.Cells[i].Value.ToString() != String.Empty &&
                        e.Row.Cells[worseRankColumnIndex].Value.ToString() != String.Empty)
                    {
                        int currentRank = Convert.ToInt32(e.Row.Cells[i].Value);
                        int worseRank = Convert.ToInt32(e.Row.Cells[worseRankColumnIndex].Value);

                        if (currentRank == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = String.Format("{0} {1}", best, obj);
                        }
                        else if (currentRank == worseRank)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = String.Format("{0} {1}", pour, obj);
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != String.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Прирост к прошлому году";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Снижение к прошлому году";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (e.Row.Cells[1].Value != null &&
                    (e.Row.Cells[1].Value.ToString() == "Расходы бюджета - ИТОГО " ||
                     e.Row.Cells[1].Value.ToString() == "Расходы бюджета - ИТОГО" ||
                     e.Row.Cells[1].Value.ToString().Contains("Расходы бюджета – Всего") ||
                     e.Row.Cells[1].Value.ToString() == "Расходы итого "))
                {
                    cell.Style.Font.Bold = true;
                }

                if (cell.Value != null && cell.Value.ToString() != String.Empty)
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

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = isFKR ? DataProvider.GetQueryText("FO_0002_0008_Settlement_chart_FKR") : DataProvider.GetQueryText("FO_0002_0008_Settlement_chart_KOSGU");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    if (isFKR)
                    {
                        row[0] = GetShortRzPrName(row[0].ToString().ToUpper());
                    }
                    else
                    {
                        row[0] = DataDictionariesHelper.GetShortKOSGUName(row[0].ToString());
                    }
                }
            }

            ((UltraChart)sender).DataSource = dtChart;
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

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма план");
            ReportExcelExporter1.Export(UltraChart, chartHeaderLabel.Text, sheet2, 3);

            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма факт");
            UltraChart2.Width = UltraChart.Width;
            UltraChart2.Legend = UltraChart.Legend;
            ReportExcelExporter1.Export(UltraChart2, chartHeaderLabel.Text, sheet3, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(headerLayout, section1);

            ISection section2 = report.AddSection();
            ReportPDFExporter1.Export(UltraChart, chartHeaderLabel.Text, section2);
            ReportPDFExporter1.Export(UltraChart2, section2);
        }

        #endregion
    }
}
