using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Section;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0012_Sakhalin
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2000;
        private int endYear = 2012;
        private int badRank = 0;

        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // группа КД
        private CustomParam kdGroupName;
        // выбранный регион
        private CustomParam selectedRegion;
        // Тип документа
        private CustomParam documentType;
        // Тип документа для районов
        private CustomParam regionDocumentType;
        // Консолидированный уровень
        private CustomParam consolidateLevel;
        // Уровень бюджета СКИФ
        private CustomParam budgetSKIFLevel;
        // фильтр по годам
        private CustomParam filterYear;
        // Последний год
        private CustomParam lastYear;
        // Последний год
        private CustomParam periodMonth;

        // Доходы-Всего
        private CustomParam incomesTotal;

        // уровень МР и ГО
        private CustomParam regionsLevel;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            //UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 250);
            UltraWebGrid.Height = Unit.Empty;

            #region Инициализация параметров запроса

            if (kdGroupName == null)
            {
                kdGroupName = UserParams.CustomParam("kd_group_name");
            }
            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (documentType == null)
            {
                documentType = UserParams.CustomParam("document_type");
            }
            if (regionDocumentType == null)
            {
                regionDocumentType = UserParams.CustomParam("region_document_type");
            }
            if (consolidateLevel == null)
            {
                consolidateLevel = UserParams.CustomParam("consolidate_level");
            }
            if (budgetSKIFLevel == null)
            {
                budgetSKIFLevel = UserParams.CustomParam("budget_SKIF_level");
            }
            if (filterYear == null)
            {
                filterYear = UserParams.CustomParam("filter_year");
            }
            if (lastYear == null)
            {
                lastYear = UserParams.CustomParam("last_year");
            }
            if (periodMonth == null)
            {
                periodMonth = UserParams.CustomParam("period_month");
            }

            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }

            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraWebGrid.EnableViewState = false;
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11402000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11402000000000000;
            UserParams.IncomesKD11403000000000410.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000410;
            UserParams.IncomesKD11403000000000440.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000440 != "null"
                    ? string.Format(",[КД].[Сопоставимый].[Все коды доходов].[{1}].[ДОХОДЫ ОТ ПРОДАЖИ МАТЕРИАЛЬНЫХ И НЕМАТЕРИАЛЬНЫХ АКТИВОВ].[{0}],",
                                    RegionSettingsHelper.Instance.IncomesKD11403000000000440,
                                    UserParams.IncomesKDRootName.Value)
                    : ",";
            UserParams.IncomesKD11406000000000430.Value = RegionSettingsHelper.Instance.IncomesKD11406000000000430;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKD30000000000000000.Value = RegionSettingsHelper.Instance.IncomesKD30000000000000000;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0012_Sakhalin_date");
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

                ComboKD.Width = 420;
                ComboKD.Title = "Вид дохода";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillFullKDIncludingList());
                ComboKD.SetСheckedState("Доходы бюджета - Итого ", true);
            }


            selectedRegion.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            documentType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            regionDocumentType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");
            kdGroupName.Value = ComboKD.SelectedValue;
            periodMonth.Value = MakePeriodMonth(ComboYear.SelectedValue, ComboMonth.SelectedValue);

            consolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            Page.Title = string.Format("Поступления доходов в бюджеты городских округов, районов и поселений: {0}", ComboKD.SelectedValue);
            PageTitle.Text = Page.Title;
            string monthString = string.Empty;
            switch (ComboMonth.SelectedIndex + 1)
            {
                case 1:
                    monthString = "месяц";
                    break;
                case 2:
                case 3:
                case 4: monthString = "месяца";
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12: monthString = "месяцев";
                    break;
            }
            PageSubTitle.Text = string.Format("Оценка исполнения бюджетов за {0} {2} {1} года.",
                                                ComboMonth.SelectedIndex + 1, ComboYear.SelectedValue, monthString);
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0012_Sakhalin_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length; i++)
                    {
                        if ((i == 1 || i == 2 || i == 6 || i == 7 || i == 9 || i == 10 || i == 13 || i == 14)
                            && row[i] != DBNull.Value && row[i].ToString() != String.Empty)
                        {
                            row[i] = Convert.ToDouble(row[i]) / 1000;
                        }
                    }
                }
                badRank = 0;
                if (dtGrid.Rows[1][5] != DBNull.Value && dtGrid.Rows[1][5].ToString() != String.Empty)
                {
                    badRank = Convert.ToInt32(dtGrid.Rows[1][5]);
                }
                dtGrid.Columns.RemoveAt(5);

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
                string formatString;
                int widthColumn;
                switch (i)
                {
                    case 1:
                    case 2:
                    case 5:
                    case 6:
                        {
                            formatString = "N2";
                            widthColumn = 100;
                            break;
                        }
                    case 8:
                    case 9:
                    case 12:
                    case 13:
                        {
                            formatString = "N2";
                            widthColumn = 95;
                            break;
                        }
                    case 3:
                    case 7:
                    case 10:
                    case 11:
                    case 14:
                    case 15:
                        {
                            formatString = "P2";
                            widthColumn = 75;
                            break;
                        }
                    default:
                        {
                            formatString = "N0";
                            widthColumn = 75;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 200;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            //e.Layout.Bands[0].Columns[5].Hidden = true;
            ////e.Layout.Bands[0].Columns[18].Hidden = true;
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 3;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 2;
                }
            }

            string year = ComboYear.SelectedValue;
            int monthCount = ComboMonth.SelectedIndex + 1;
            string months = CRHelper.RusManyMonthGenitive(monthCount);

            headerLayout.AddCell("Наименование МО");
            GridHeaderCell groupCell = headerLayout.AddCell("Консолидированный бюджет МО");
            GridHeaderCell groupCell2 = headerLayout.AddCell("В том числе:");

            groupCell.AddCell("Утверждено на год, тыс.руб.", String.Format("Утвержденные годовые назначения на {0} год", year), 2);
            groupCell.AddCell("Исполнено, тыс.руб.", String.Format("Поступления за {0} {1} {2} года", monthCount, months, year), 2);
            groupCell.AddCell("% исполнения", String.Format("«% выполнения годовых назначений за {0} {1} {2} года", monthCount, months, year), 2);
            groupCell.AddCell("Ранг", String.Format("Ранг (место) МО по % выполнения годовых назначений за {0} {1} {2} года", monthCount, months, year), 2);


            GridHeaderCell groupCell2_1 = groupCell2.AddCell("Бюджет городских округов");
            GridHeaderCell groupCell2_2 = groupCell2.AddCell("Бюджет районов");
            GridHeaderCell groupCell2_3 = groupCell2.AddCell("Бюджет поселений");

            groupCell2_1.AddCell("Утверждено на год, тыс.руб.", String.Format("Утвержденные годовые назначения на {0} год", year));
            groupCell2_1.AddCell("Исполнено, тыс.руб.", String.Format("Поступления за {0} {1} {2} года", monthCount, months, year));
            groupCell2_1.AddCell("% исполнения", String.Format("«% выполнения годовых назначений за {0} {1} {2} года", monthCount, months, year));

            groupCell2_2.AddCell("Утверждено на год, тыс.руб.", String.Format("Утвержденные годовые назначения на {0} год", year));
            groupCell2_2.AddCell("Исполнено, тыс.руб.", String.Format("Поступления за {0} {1} {2} года", monthCount, months, year));
            groupCell2_2.AddCell("% исполнения", String.Format("«% выполнения годовых назначений за {0} {1} {2} года", monthCount, months, year));
            groupCell2_2.AddCell("Удельный вес", "Удельный вес поступлений в бюджет в консолидированном бюджете МО");

            groupCell2_3.AddCell("Утверждено на год, тыс.руб.", String.Format("Утвержденные годовые назначения на {0} год", year));
            groupCell2_3.AddCell("Исполнено, тыс.руб.", String.Format("Поступления за {0} {1} {2} года", monthCount, months, year));
            groupCell2_3.AddCell("% исполнения", String.Format("«% выполнения годовых назначений за {0} {1} {2} года", monthCount, months, year));
            groupCell2_3.AddCell("Удельный вес", "Удельный вес поступлений в бюджет в консолидированном бюджете МО");

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            SetRankImg(e.Row, 4);

            for (int i = 0; i < e.Row.Cells.Count - 1; i++)
            {
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

                    if (e.Row.Cells[0].Value != null &&
                        (e.Row.Cells[0].Value.ToString().ToLower().Contains("итого")))
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }
        }

        private void SetRankImg(UltraGridRow row, int i)
        {
            string css = string.Empty;
            if (Convert.ToInt32(row.Cells[i].Value) == 1)
            {
                css = "~/images/starYellowBB.png";
            }
            else if (UglyRank(row, i))
            {
                css = "~/images/starGrayBB.png";
            }
            if (css != string.Empty)
            {
                row.Cells[i].Style.BackgroundImage = css;
                row.Cells[i].Title = (css == "~/images/starGrayBB.png")
                   ? "Наименьший % исполнения"
                   : "Наибольший % исполнения";
            }
            row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
        }

        private bool UglyRank(UltraGridRow row, int i)
        {
            return (row.Cells[i] != null) && (badRank != 0) &&
                   (Convert.ToInt32(row.Cells[i].Value) == badRank) &&
                   (Convert.ToInt32(row.Cells[i].Value) != 0);
        }

        private string MakePeriodMonth(string year, string month)
        {
            int monthNum = CRHelper.MonthNum(month);
            int halfYear = CRHelper.HalfYearNumByMonthNum(monthNum);
            int quater = CRHelper.QuarterNumByMonthNum(monthNum);
            return String.Format("[{0}].[Полугодие {1}].[Квартал {2}].[{3}]", year, halfYear, quater, month);
        }

        #endregion

        #region Экспорт в Excel



        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");


            ReportExcelExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.SheetColumnCount = 20;

            ReportExcelExporter1.GridColumnWidthScale = 0.9;

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            //ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        #endregion
    }
}
