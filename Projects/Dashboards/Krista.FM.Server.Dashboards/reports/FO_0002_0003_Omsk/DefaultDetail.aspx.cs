using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core.MemberDigests;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0003_Omsk
{
    public partial class DefaultDetail : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";
        private string multiplierCaption;

        private GridHeaderLayout headerLayout;

        #region Параметры запроса

        private CustomParam selectedRegion;
        // доходы Итого
        private CustomParam incomesTotal;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
        // тип документа СКИФ для консолидированного уровня
        private CustomParam consolidateDocumentSKIFType;
        // тип документа СКИФ для уровня районов
        private CustomParam regionDocumentSKIFType;
        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;
        // уровень МО
        private CustomParam regionsLevel;
        // выбранный множитель рублей
        private CustomParam rubMultiplier;

        #endregion

        private MemberAttributesDigest budgetDigest;

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }
            if (regionsConsolidateLevel == null)
            {
                regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
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
            
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            regionsLevel = UserParams.CustomParam("regions_level");

            #endregion

            multiplierCaption = IsThsRubSelected ? "тыс.руб." : "млн.руб.";
            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 220);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Text = "Информация&nbsp;об&nbsp;исполнении&nbsp;собственных&nbsp;доходов";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0011_01/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.IncomesKDSubSectionLevel.Value = RegionSettingsHelper.Instance.IncomesKDSubSectionLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.KDInternalCircualtionExtruding.Value = RegionSettingsHelper.Instance.KDInternalCircualtionExtruding;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            UserParams.RegionsLocalBudgetLevel.Value = RegionSettingsHelper.Instance.RegionsLocalBudgetLevel;

            budgetDigest = MemberDigestHelper.Instance.LocalBudgetDigest;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0003_Omsk_date");
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
                
                ComboRegion.Title = "Бюджет";
                ComboRegion.Width = 400;
                ComboRegion.MultiSelect = false;
                ComboRegion.ParentSelect = true;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetDigest.UniqueNames, budgetDigest.MemberLevels));
                ComboRegion.SetСheckedState("Консолидированный бюджет субъекта", true);
            }
            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            Page.Title = string.Format("Доходы ({0})", ComboRegion.SelectedValue);
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Информация за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            switch (ComboRegion.SelectedValue)
            {
                case "Консолидированный бюджет субъекта":
                    {
                        selectedRegion.Value = "[Районы].[Сопоставимый].[Консолидированный бюджет субъекта ]";
                        break;
                    }
                case "Муниципальные районы":
                    {
                        selectedRegion.Value = "[Районы].[Сопоставимый].[Муниципальные районы ]";
                        break;
                    }
                case "Городские округа":
                    {
                        selectedRegion.Value = "[Районы].[Сопоставимый].[Городские округа ]";
                        break;
                    }
                default:
                    {
                        selectedRegion.Value = budgetDigest.GetMemberUniqueName(ComboRegion.SelectedValue);
                        break;
                    }
            }

            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodEndYear.Value = (yearNum - 2).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;

            consolidateDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0003_Omsk_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

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
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(345);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                string formatString = GetColumnFormat(e.Layout.Bands[0].Columns[i].Header.Caption);
                int widthColumn = GetColumnWidth(e.Layout.Bands[0].Columns[i].Header.Caption);

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int lastYearNum = Convert.ToInt32(ComboYear.SelectedValue) - 1;
            int lastLastYearNum = Convert.ToInt32(ComboYear.SelectedValue) - 2;

            headerLayout.AddCell("КД");
            headerLayout.AddCell("Код");

            headerLayout.AddCell("Назначено (первоначально за январь), тыс.руб.");
            GridHeaderCell currentYearCell = headerLayout.AddCell(String.Format("За {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum));
            currentYearCell.AddCell(String.Format("Назначено, {0}", multiplierCaption), "Плановые назначения на год");
            currentYearCell.AddCell("Изменение плана, %", "Изменение плана, %");
            currentYearCell.AddCell("Изменение плана, тыс.руб.", "Изменение плана, тыс.руб.");
            currentYearCell.AddCell(String.Format("Исполнено, {0}", multiplierCaption), "Фактическое исполнение нарастающим итогом с начала года");
            currentYearCell.AddCell("Исполнено %", "Процент выполнения назначений/ Оценка равномерности исполнения (1/12 годового плана в месяц)");
            currentYearCell.AddCell("Исполнено к плану, тыс.руб.", "Исполнено к плану, тыс.руб.");
            currentYearCell.AddCell("Средний % исполнения", 
                String.Format("Средний % исполнения за {0} {1} {2} и {3} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), lastLastYearNum, lastYearNum));
            currentYearCell.AddCell("% исп. – отклонение от среднего",
                String.Format("Отклонение от среднего % исполнения за {0} {1} {2} и {3} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), lastLastYearNum, lastYearNum));
            
            GridHeaderCell lastYearCell = headerLayout.AddCell("Сравнение с прошлым годом");
            lastYearCell.AddCell(String.Format("Исполнено прошлый год, {0}", multiplierCaption), "Исполнено за аналогичный период прошлого года");
            lastYearCell.AddCell("Темп роста к прошлому году", "Темп роста исполнения к аналогичному периоду прошлого года");
            lastYearCell.AddCell("Рост (снижение) поступлений, тыс.руб.", "Рост (снижение) поступлений, тыс.руб.");

            headerLayout.ApplyHeaderInfo();
        }

        private static string GetColumnFormat(string columnName)
        {
            if ((columnName.ToLower().Contains("%") || columnName.ToLower().Contains("отклонение") || columnName.ToLower().Contains("темп")))
            {
                return "P2";
            }
            if (columnName.ToLower().Contains("код"))
            {
                return "0";
            }
            return "N2";
        }

        private static int GetColumnWidth(string columnName)
        {
            if ((columnName.ToLower().Contains("%") || columnName.ToLower().Contains("отклонение") || columnName.ToLower().Contains("темп")))
            {
                return 81;
            }
            if (columnName.ToLower().Contains("код"))
            {
                return 120;
            }
            return 105;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                string columnName = e.Row.Band.Columns[i].Header.Caption.ToLower();
                int levelColumnIndex = e.Row.Cells.Count - 1;
                bool executePercent = columnName.Contains("исполнено %");
                bool growRate = columnName.Contains("темп роста");
                bool deviation = columnName.Contains("отклонение");

                if (executePercent && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
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
                    e.Row.Cells[i].Style.Padding.Right = 2;
                }

                if (growRate && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост к прошлому году";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Снижение к прошлому году";
                    }
                }

                if (deviation && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "% исполнения выше среднего";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "% исполнения ниже среднего";
                    }
                }

                if (e.Row.Cells[levelColumnIndex] != null && e.Row.Cells[levelColumnIndex].Value.ToString() != string.Empty && i != 1)
                {
                    string level = e.Row.Cells[levelColumnIndex].Value.ToString();
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;
                    switch (level)
                    {
                        case "Группа":
                            {
                                fontSize = 10;
                                bold = true;
                                break;
                            }
                        case "Подгруппа":
                            {
                                fontSize = 10;
                                italic = true;
                                break;
                            }
                        case "Статья":
                            {
                                fontSize = 8;
                                break;
                            }
                    }
                    e.Row.Cells[i].Style.Font.Size = fontSize;
                    e.Row.Cells[i].Style.Font.Bold = bold;
                    e.Row.Cells[i].Style.Font.Italic = italic;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
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
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            ReportPDFExporter1.Export(headerLayout);
        }

        #endregion
    }
}
