using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core.MemberDigests;

namespace Krista.FM.Server.Dashboards.reports.FO_0003_0006
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";
        private double rubMultiplier;
        private GridHeaderLayout headerLayout;
        private DateTime currentDate;

        #region Параметры запроса

        private CustomParam selectedRegion;
        // расходы Итого
        private CustomParam outcomesTotal;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
        // уровень МР и ГО
        private CustomParam regionsLevel;
        // тип документа СКИФ для консолидированного уровня
        private CustomParam consolidateDocumentSKIFType;
        // тип документа СКИФ для уровня районов
        private CustomParam regionDocumentSKIFType;
        
        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;
        // для этого года не выводить колонки сравнения
        private CustomParam isNonCompareYear;

        #endregion

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "Тыс.руб." : "Млн.руб."; }
        }

        private bool IsNonCompareYear
        {
            get { return currentDate.Year == 2011; }
        }

        private MemberAttributesDigest budgetDigest;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса
           
            selectedRegion = UserParams.CustomParam("selected_region");
            outcomesTotal = UserParams.CustomParam("outcomes_total");
            regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            regionsLevel = UserParams.CustomParam("regions_level");
            consolidateDocumentSKIFType = UserParams.CustomParam("consolidate_document_skif_type");
            regionDocumentSKIFType = UserParams.CustomParam("region_document_skif_type");
            regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            isNonCompareYear = UserParams.CustomParam("is_non_compare_year");

            #endregion

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 250);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.FKRAllLevel.Value = RegionSettingsHelper.Instance.FKRAllLevel;
            UserParams.FKRSectionLevel.Value = RegionSettingsHelper.Instance.FKRSectionLevel;
            UserParams.FKRSubSectionLevel.Value = RegionSettingsHelper.Instance.FKRSubSectionLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.RzPrInternalCircualtionExtruding.Value = RegionSettingsHelper.Instance.RzPrInternalCircualtionExtruding;

            firstYear = 2000;

            budgetDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0003_0006_Digest");

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0003_0006_date");
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
                ComboMonth.Width = 135;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                ComboRegion.Title = "Бюджет поселения";
                ComboRegion.Width = 400;
                ComboRegion.MultiSelect = false;
                ComboRegion.ParentSelect = true;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetDigest.UniqueNames, budgetDigest.MemberLevels));
            }

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, monthNum, 1);
            isNonCompareYear.Value = IsNonCompareYear.ToString();
            string settlement = string.Empty;
            if (ComboRegion.SelectedNode.Level == 0)
            { 
                settlement = string.Format("{0}, все поселения", ComboRegion.SelectedValue);
            }else
            {
                settlement = string.Format("{0}, {1}", ComboRegion.SelectedNodeParent, ComboRegion.SelectedValue);
            }
            Page.Title = string.Format("Расходы бюджета поселения({0})", settlement);
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Информация об исполнении за {0} {1} {2} года в разрезе разделов, подразделов классификации расходов, {3}",
                currentDate.Month, CRHelper.RusManyMonthGenitive(currentDate.Month), currentDate.Year, RubMiltiplierButtonList.SelectedValue);
            CommentTextLabel.Text = IsNonCompareYear
                            ? "В связи с изменением с 01.01.2011 года бюджетной классификации в соответствии с Приказом Минфина РФ №190н от 28.12.2010 года приводить сравнение расходов в разрезе РзПр за 2011 год с предыдущими годами некорректно."
                            : String.Empty;

            selectedRegion.Value = budgetDigest.GetMemberUniqueName(ComboRegion.SelectedValue);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            outcomesTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            consolidateDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");

            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;
            
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FО_0002_0006_detail_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length; i++)
                    {
                        if ((i == 1) && row[i] != DBNull.Value && row[i].ToString().Length > 4)
                        {
                            row[i] = row[i].ToString().Substring(0, 4);
                        }
                        if (GetColumnFormat(dtGrid.Columns[i].ColumnName) == "N2" && row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(String.Format("{0:N2}", Convert.ToDouble(row[i]) / rubMultiplier));
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
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            for (int i = 1; i < columnCount - 2; i = i + 1)
            {
                string formatString = GetColumnFormat(e.Layout.Bands[0].Columns[i].Header.Caption);
                int widthColumn = GetColumnWidth(e.Layout.Bands[0].Columns[i].Header.Caption);

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;

            headerLayout.AddCell("РзПр");
            headerLayout.AddCell("Код");

            GridHeaderCell groupCell = headerLayout.AddCell(String.Format("за {0} {1} {2} года",
                currentDate.Month, CRHelper.RusManyMonthGenitive(currentDate.Month), currentDate.Year));

            groupCell.AddCell(String.Format("Уточненные годовые назначения, {0}", RubMultiplierCaption.ToLower()), "Плановые назначения на год");
            groupCell.AddCell("Доля по уточненным годовым назначениям", "Доля расхода в общей сумме расходов плановая");
            groupCell.AddCell(String.Format("Исполнено, {0}", RubMultiplierCaption.ToLower()), "Фактическое исполнение нарастающим итогом с начала года");
            groupCell.AddCell("Доля", "Доля расхода в общей сумме расходов бюджета");
            //groupCell.AddCell("Доля КБ", "Доля данного вида расходов в консолидированном бюджете субъекта РФ");
            groupCell.AddCell("Исполнено %", "Процент выполнения назначений. Индикация сравнения со средним исполнением по району");
            
            if (!IsNonCompareYear)
            {
                GridHeaderCell compareGroupCell = headerLayout.AddCell("Сравнение с прошлым годом");
                compareGroupCell.AddCell(String.Format("Назначено прошлый год, {0}", RubMultiplierCaption.ToLower()), "Назначения в аналогичном периоде прошлого года");
                compareGroupCell.AddCell(String.Format("Исполнено прошлый год, {0}", RubMultiplierCaption.ToLower()), "Исполнено за аналогичный период прошлого года");
                compareGroupCell.AddCell("Доля в прошлом году", "Доля расхода в общей сумме фактических расходов в прошлом году");
                compareGroupCell.AddCell("Исполнено % прошлый год", "Процент выполнения назначений за аналогичный период прошлого года");
                compareGroupCell.AddCell("Темп роста к прошлому году", "Темп роста исполнения к аналогичному периоду прошлого года");
            }

            headerLayout.ApplyHeaderInfo();
        }

        private static string GetColumnFormat(string columnName)
        {
            if ((columnName.ToLower().Contains("годовые назначения") || columnName.ToLower().Contains("назначено") || columnName.ToLower().Contains("факт")) &&
                !columnName.ToLower().Contains("доля"))
            {
                return "N2";
            }
            if (columnName.ToLower().Contains("код"))
            {
                return "00 00";
            }
            return "P2";
        }

        private int GetColumnWidth(string columnName)
        {
            double widthMultiplier = IsNonCompareYear ? 1.7 : 1;
            if (columnName.ToLower().Contains("годовые назначения") || columnName.ToLower().Contains("назначено") || columnName.ToLower().Contains("факт"))
            {
                return Convert.ToInt32(widthMultiplier * 90);
            }
            if (columnName.ToLower().Contains("код"))
            {
                return Convert.ToInt32(widthMultiplier * 45);
            }
            return Convert.ToInt32(widthMultiplier * 70);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                string columnName = e.Row.Band.Columns[i].Header.Caption.ToLower();
                int levelColumnIndex = e.Row.Cells.Count - 1;
                int avgColumnIndex = e.Row.Cells.Count - 2;
                int lastYearShartColumnIndex = IsNonCompareYear ? - 1 : e.Row.Cells.Count - 5;

                bool executePercent = columnName.Contains("исполнено %");
                bool weight = columnName == "доля";
                bool rate = columnName.Contains("темп роста");
                
                if (executePercent)
                {
                    int avgValue = Int32.MinValue;
                    if (e.Row.Cells[avgColumnIndex].Value != null && e.Row.Cells[avgColumnIndex].Value.ToString() != string.Empty)
                    {
                        avgValue = Convert.ToInt32(100 * Convert.ToDouble(e.Row.Cells[avgColumnIndex].Value));
                    }

                    if (avgValue != Int32.MinValue && e.Row.Cells[i].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        int value = Convert.ToInt32(100*Convert.ToDouble(e.Row.Cells[i].Value));

                        if (value < avgValue)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerRed.gif";
                            e.Row.Cells[i].Title = string.Format("Ниже среднего исполнения по муниципальному району ({0:N0}%)",
                                                                 avgValue);
                        }
                        else if (value > avgValue)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerGreen.gif";
                            e.Row.Cells[i].Title = string.Format("Выше среднего исполнения по муниципальному району ({0:N0}%)",
                                                                 avgValue);
                        }
                        e.Row.Cells[i].Style.Padding.Right = 10;
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: right top; margin: 2px";
                    }
                }

                if (weight && lastYearShartColumnIndex != - 1 && e.Row.Cells[i].Value != null)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) > Convert.ToDouble(e.Row.Cells[lastYearShartColumnIndex].Value))
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Доля выросла с прошлого года";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[i].Value) < Convert.ToDouble(e.Row.Cells[lastYearShartColumnIndex].Value))
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Доля снизилась с прошлого года";
                    }
                    e.Row.Cells[i].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (rate && e.Row.Cells[i].Value != null)
                {
                    if (100*Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост к прошлому году";

                    }
                    else if (100*Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Снижение к прошлому году";
                    }
                    e.Row.Cells[i].Style.Padding.Right = 2;
                    e.Row.Cells[i].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (e.Row.Cells[levelColumnIndex] != null && e.Row.Cells[levelColumnIndex].Value != null &&
                    e.Row.Cells[levelColumnIndex].Value.ToString() == UserParams.FKRSectionLevel.Value ||
                    e.Row.Cells[0] != null && e.Row.Cells[0].Value != null &&
                    (e.Row.Cells[0].Value.ToString() == "Расходы Итого" ||
                     e.Row.Cells[0].Value.ToString() == "Расходы Итого "))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
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