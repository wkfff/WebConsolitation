using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Orientation = Infragistics.Documents.Excel.Orientation;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0028
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable monthReportDT;
        private DataTable yearReportDT;
        private DataTable populationDT;
        private int firstYear = 2010;
        private int endYear = 2011;
        private int currentYear;
        private DateTime lastDate;

        private GridHeaderLayout headerLayout;

        #endregion

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        #region Параметры запроса

        // выбранный множитель рублей
        private CustomParam rubMultiplier;
        //Имя региона для выборки численности
        private CustomParam populationRegionName;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 240);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.EnableViewState = false;

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            #region Инициализация параметров запроса

            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            populationRegionName = UserParams.CustomParam("population_region_name");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport +=new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.IncomesKD30000000000000000.Value = RegionSettingsHelper.Instance.IncomesKD30000000000000000;

            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0028_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();
            lastDate = new DateTime(endYear, CRHelper.MonthNum(month), 1);

            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
            }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue.ToString());

            Page.Title = "Финансовый паспорт субъекта РФ";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("Исполнение консолидированного бюджета за {0} - {1} годы, по состоянию на {2:dd.MM.yyyy} года, {3}", 
                currentYear - 2, currentYear, lastDate, RubMiltiplierButtonList.SelectedValue);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";
            populationRegionName.Value = RegionSettingsHelper.Instance.GetPropertyValue("PopulationRegionName");

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        private static bool IsGroupRow(string rowName)
        {
            rowName = rowName.TrimEnd(' ');
            return (rowName == "I. Доходы" || rowName == "II. Расходы" ||
                    rowName == "III Источники финансирования дефицита бюджета" || rowName == "V.Справочно." ||
                    rowName.Contains("из них"));
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0028_grid_monthReport");
            monthReportDT = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", monthReportDT);

            if (monthReportDT.Columns.Count > 1 && monthReportDT.Rows.Count > 0)
            {
                query = DataProvider.GetQueryText("FO_0002_0028_grid_yearReport");
                yearReportDT = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", yearReportDT);

                if (yearReportDT.Columns.Count > 1 && yearReportDT.Rows.Count > 0)
                {
                    yearReportDT.PrimaryKey = new DataColumn[] { yearReportDT.Columns[0] };

                    foreach (DataRow monthRow in monthReportDT.Rows)
                    {
                        if (monthRow[0] != DBNull.Value)
                        {
                            string rowName = monthRow[0].ToString();
                            DataRow yearRow = yearReportDT.Rows.Find(rowName);
                            if (yearRow != null)
                            {
                                monthRow[1] = yearRow[1];
                            }
                        }
                    }
                }

                query = DataProvider.GetQueryText("FO_0002_0028_grid_population");
                populationDT = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", populationDT);

                monthReportDT = MergeDataTables(monthReportDT, populationDT);

                UltraWebGrid.DataSource = monthReportDT;
            }
        }

        private static DataTable MergeDataTables(DataTable dt1, DataTable dt2)
        {
            DataTable newDT = dt1.Copy();
            foreach (DataRow row in dt2.Rows)
            {
                DataRow newRow = newDT.NewRow();
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    newRow[i] = row[i];
                }
                newDT.Rows.Add(newRow);
            }
            newDT.AcceptChanges();

            return newDT;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            //e.Layout.UseFixedHeaders = true;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 1)
            {
                string columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption.ToLower();
                string formatString = columnCaption.Contains("темп роста") ? "N1" : "N1";
                int widthColumn = 120;
                
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            
            //e.Layout.Bands[0].Columns[0].Header.Fixed = true;
            //e.Layout.FixedColumnScrollType = FixedColumnScrollType.Delay;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(230);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            headerLayout.AddCell("Показатель");
            headerLayout.AddCell(String.Format("Исполнение {0} год (конс.бюджет)", currentYear - 2));

            GridHeaderCell lastYearCell = headerLayout.AddCell(String.Format("{0} год", currentYear - 1));
            GridHeaderCell cell = lastYearCell.AddCell("Исполнение");
            cell.AddCell("Конс.бюджет");
            cell.AddCell("в т.ч. областной");
            for (int month = 1; month < 12; month++)
            {
                lastYearCell.AddCell(String.Format("Исполнено с начала года на 1 {0}", CRHelper.RusMonthGenitive(month + 1).ToLower()));
                lastYearCell.AddCell(String.Format("Исполнение {0}", CRHelper.RusMonth(month + 1).ToLower()));
            }

            GridHeaderCell currentYearCell = headerLayout.AddCell(String.Format("{0} год", currentYear));
            currentYearCell.AddCell("План (первонача- льный)");
            cell = currentYearCell.AddCell("Уточненный план по данным субъекта РФ");
            cell.AddCell("Конс.бюджет");
            cell.AddCell("в т.ч. областной");
            currentYearCell.AddCell("Темп роста уточненного плана, %");
            for (int month = 1; month < 11; month++)
            {
                currentYearCell.AddCell(String.Format("Исполнено с начала года на 1 {0}", CRHelper.RusMonthGenitive(month + 1).ToLower()));
                currentYearCell.AddCell(String.Format("Темп роста исполнения с начала года на 1 {0}, %", CRHelper.RusMonthGenitive(month + 1).ToLower()));
                currentYearCell.AddCell(String.Format("Исполнено за {0}", CRHelper.RusMonth(month + 1).ToLower()));
                currentYearCell.AddCell(String.Format("Темп роста исполнения за {0}, %", CRHelper.RusMonth(month + 1).ToLower()));
            }

            currentYearCell.AddCell(String.Format("Исполнено с начала года на 1 {0}", CRHelper.RusMonthGenitive(12).ToLower()));
            currentYearCell.AddCell(String.Format("Темп роста исполнения с начала года на 1 {0}, %", CRHelper.RusMonthGenitive(12).ToLower()));

            cell = currentYearCell.AddCell("Исполнение");
            cell.AddCell("Конс.бюджет");
            cell.AddCell("в т.ч. областной");

            headerLayout.ApplyHeaderInfo();
        }

        private static bool IsInvertIndication(string indicatorName)
        {
            switch (indicatorName)
            {
                case "услуги связи (КОСГУ 221)":
                case "транспортные услуги (КОСГУ 222)":
                case "арендная плата за пользование имуществом (КОСГУ 224)":
                case "2.3 Расходы на прочие нужды":
                case "Работы, услуги по содержанию имущества (КОСГУ 225)":
                case "Прочие работы и услуги (КОСГУ 226)":
                case "Безвозмездные перечисления государственным и муниципальным предприятиям (КОСГУ 241)":
                case "Безвозмездные перечисления организациям, за исключением государственных и муниципальных предприятий (КОСГУ 242)":
                case "Прочие расходы (КОСГУ 290)":
                case "3 Расходы":
                case "3.2 Другие расходы (за искл. групп 1, 2 и 3.1)":
                case "ИТОГО РАСХОДОВ":
                case "Итого расходов без учета доходов за счет безвозмездных поступлений":
                case "Расходы за исключением группы 3":
                case "Превышение расходов (за исключением группы 3) над доходами (без учета безвозмездных перечислений на капитальные вложения)":
                case "3.2 Кредиты из федерального бюджета":
                case "3.3 Кредиты коммерческих банко":
                case "Привлечение кредитов":
                case "3.4 Иные источники":
                case "ИТОГО ИСТОЧНИКОВ ФИНАНСИРОВАНИЯ":
                case "Превышение расходов (за исключением группы 3) над доходами (без учета безвозмездных перечислений на капитальные вложения) с учетом источников финансирования дефицита бюджета":
                case "Превышение расходов над доходами с учетом источников финансирования дефицита бюджета":
                case "IV Просроченная кредиторская задолженность - всего":
                case "По заработной плате":
                case "По начислениям на оплату труда":
                case "По оплате коммунальных услуг":
                case "По обеспечению мер социальной поддержки отдельных категорий граждан":
                case "По расходам  на обязательное медицинское страхование неработающего населения":
                case "5.1 Объем государственного долга":
                case "5.2 Численность населения, тыс.чел.":
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string indicatorName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                indicatorName = e.Row.Cells[0].Value.ToString().Replace("_", String.Empty);
                e.Row.Cells[0].Value = indicatorName;
            }

            bool isInvertIndication = IsInvertIndication(indicatorName.TrimEnd(' '));

            string level = String.Empty;
            int levelColumnIndex = e.Row.Cells.Count - 1;
            if (e.Row.Cells[levelColumnIndex] != null)
            {
                level = e.Row.Cells[levelColumnIndex].ToString();
            }

            e.Row.Style.Padding.Right = 5;

            if (IsGroupRow(indicatorName))
            {
                e.Row.Cells[0].ColSpan = e.Row.Cells.Count - 1;
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                string columnCaption = e.Row.Band.Columns[i].Header.Caption.ToLower();

                bool rate = columnCaption.Contains("темп роста");

                switch (level)
                {
                    case "0":
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                            e.Row.Cells[i].Style.Font.Size = 9;
                            break;
                        }
                    case "1":
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                            e.Row.Cells[i].Style.Font.Italic = true;
                            e.Row.Cells[i].Style.Font.Size = 8;
                            break;
                        }
                    case "2":
                        {
                            e.Row.Cells[i].Style.Font.Bold = false;
                            e.Row.Cells[i].Style.Font.Italic = true;
                            e.Row.Cells[i].Style.Font.Size = 8;
                            break;
                        }
                }

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double currentValue = Convert.ToDouble(e.Row.Cells[i].Value);

                        if (currentValue > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = isInvertIndication ? "~/images/arrowRedUpBB.png" : "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Наблюдается рост исполнения относительно предыдущего года";
                        }
                        else if (currentValue < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = isInvertIndication ? "~/images/arrowGreenDownBB.png" : "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Наблюдается снижение исполнения относительно предыдущего года";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                UltraGridCell cell = e.Row.Cells[i];
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

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 40;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            ReportExcelExporter1.HeaderCellHeight = 20;
            ReportExcelExporter1.Export(headerLayout, 3);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            ReportPDFExporter1.HeaderCellHeight = 30;
            ReportPDFExporter1.Export(headerLayout);
        }

        #endregion
    }
}
