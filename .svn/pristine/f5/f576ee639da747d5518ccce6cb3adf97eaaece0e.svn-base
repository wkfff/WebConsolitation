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

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0021
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private DateTime currentDate;
        private DateTime nextMonthDate;
        private string month = "Январь";
        string regionType;

        private string exportFontName = "Times New Roman";
        private GridHeaderLayout headerLayout;

        private bool UseConsolidateLevel
        {
            get
            {
                return UseConsodiataLevelCheckBox.Checked;
            }
        }

        #region Параметры запроса

        // Выбранный регион
        private CustomParam selectedRegion;
        // Выбранный уровень бюджета
        private CustomParam selectedSKIFLevel;

        // уровень бюджета СКИФ для муниципальных районов
        private CustomParam regionBudgetLevel;
        // уровень бюджета СКИФ для городских округов
        private CustomParam districtBudgetLevel;

        // тип документа СКИФ для муниципальных районов
        private CustomParam regionDocumentSKIFType;
        // тип документа СКИФ для городских округов
        private CustomParam districtDocumentSKIFType;

        // доходы всего
        private CustomParam incomesAll;
        // расходы всего
        private CustomParam outcomesAll;
        // использовать для предыдущих лет данные из годовой отчетности
        private CustomParam useYearReporting;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 220);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            #region Инициализация параметров запроса

            selectedRegion = UserParams.CustomParam("selected_region");
            selectedSKIFLevel = UserParams.CustomParam("selected_skif_level");
            
            regionBudgetLevel = UserParams.CustomParam("region_budget_level");
            districtBudgetLevel = UserParams.CustomParam("district_budget_level");
            regionDocumentSKIFType = UserParams.CustomParam("region_document_skif_type");
            districtDocumentSKIFType = UserParams.CustomParam("district_document_skif_type");

            useYearReporting = UserParams.CustomParam("use_year_reporting");

            incomesAll = UserParams.CustomParam("incomes_all");
            outcomesAll = UserParams.CustomParam("outcomes_all");
            
            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport +=new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0021_date");
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
                ComboMonth.SetСheckedState(month, true);

                ComboRegion.Visible = true;
                ComboRegion.Title = "Территория";
                ComboRegion.Width = 280;
                ComboRegion.MultiSelect = false;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillTerritories(RegionsNamingHelper.LocalBudgetTypes, false));
                ComboRegion.SetСheckedState("Самара", true);
            }

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex + 1, 1);
            nextMonthDate = currentDate.AddMonths(1);

            selectedRegion.Value = RegionsNamingHelper.LocalBudgetUniqueNames[ComboRegion.SelectedValue];
            selectedSKIFLevel.Value = UseConsolidateLevel ? "Конс.бюджет МО" : "Уровень бюджета района / городского округа";
            regionType = RegionsNamingHelper.LocalBudgetTypes[ComboRegion.SelectedValue];

            string levelStr =  UseConsolidateLevel ? "(Конс.бюджет МО)" : String.Empty;

            Page.Title = String.Format("Финансовый паспорт МО {0} {1}", ComboRegion.SelectedValue, levelStr);
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("по состоянию на 1 {0} {1} года", CRHelper.RusMonthGenitive(nextMonthDate.Month), nextMonthDate.Year);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));

            regionBudgetLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetLevel");
            districtBudgetLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("DistrictBudgetLevel");
            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");
            districtDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("DistrictDocumentSKIFType");

            useYearReporting.Value = RegionSettingsHelper.Instance.GetPropertyValue("UseYearReporting");

            incomesAll.Value = RegionSettingsHelper.Instance.IncomeTotal;
            outcomesAll.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0021_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = GetNonEmptyRowsDT(dtGrid);
            }
        }

        private static bool IsEmptyRow(DataRow row)
        {
            for (int i = 1; i < row.ItemArray.Length - 1; i++)
            {
                if (row[i] != DBNull.Value)
                {
                    return false;
                }
            }

            return true;
        }

        private static DataTable GetNonEmptyRowsDT(DataTable dt)
        {
            DataTable dtCopy = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                if (!IsEmptyRow(row))
                {
                    dtCopy.ImportRow(row);
                }
            }
            dtCopy.AcceptChanges();

            return dtCopy;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 1)
            {
                string formatString;
                int widthColumn;
                
                if (i == 3 || i == 6 || i == 7 || i == 10 || i == 11)
                {
                    formatString = "N0";
                    widthColumn = 80;
                }
                else
                {
                    formatString = "N0";
                    widthColumn = 80;
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(230);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[12].Hidden = true;

            headerLayout.AddCell(e.Layout.Bands[0].Columns[0].Header.Caption);
            headerLayout.AddCell(String.Format("План на {0}&nbsp;год, тыс.руб.", currentDate.Year - 2), 
                String.Format("Запланировано на {0} год", currentDate.Year - 2));
            headerLayout.AddCell(String.Format("Фактически исполнено за {0} год, тыс.руб.", currentDate.Year - 2), 
                String.Format("Фактически исполнено за {0} год", currentDate.Year - 2));
            headerLayout.AddCell(String.Format("% исполнения"), String.Format("Процент выполнения назначений"));
            headerLayout.AddCell(String.Format("План на {0}&nbsp;год, тыс.руб.", currentDate.Year - 1), 
                String.Format("Запланировано на {0} год", currentDate.Year - 1));
            headerLayout.AddCell(String.Format("Фактически исполнено за {0} год, тыс.руб.", currentDate.Year - 1), 
                String.Format("Фактически исполнено за {0} год", currentDate.Year - 1));
            headerLayout.AddCell(String.Format("% исполнения"), String.Format("Процент выполнения назначений"));
            headerLayout.AddCell(String.Format("Прирост {0}/{1} в % (факт)", currentDate.Year - 1, currentDate.Year - 2), 
                String.Format("Темп прироста фактического исполнения {0} года к {1} году", currentDate.Year - 1, currentDate.Year - 2));
            headerLayout.AddCell(String.Format("План на {0}&nbsp;год, тыс.руб.", currentDate.Year), 
                String.Format("Запланировано на {0} год", currentDate.Year));
            headerLayout.AddCell(String.Format("Фактически исполнено за {0} год, тыс.руб.", currentDate.Year), 
                String.Format("Фактически исполнено за {0} год", currentDate.Year));
            headerLayout.AddCell(String.Format("% исполнения"), String.Format("Процент выполнения назначений"));
            headerLayout.AddCell(String.Format("Прирост {0}/{1} в % (план к факту)", currentDate.Year, currentDate.Year - 1), 
                String.Format("Темп прироста плана {0} года к факту {1} года", currentDate.Year - 1, currentDate.Year - 2));
            
            headerLayout.ApplyHeaderInfo();
        }

        private static bool IsInvertIndication(string indicatorName)
        {
            switch(indicatorName)
            {
                case "Оплата работ, услуг(220)":
                case "Услуги связи(221)":
                case "Транспортные услуги(222)":
                case "Коммунальные услуги(223)":
                case "Арендная плата за пользование имуществом (224)":
                case "Работы, услуги по содержанию имущества (225)":
                case "Прочие работы, услуги (226)":
                case "Обслуживание муниципального долга (230)":
                case "Безвозмездные перечисления государственным и муниципальным организациям (241)":
                case "Безвозмездные перечисления организациям, за исключением государственных и муниципальных организаций (242)":
                case "Прочие расходы (290)":
                case "Размер дефицита в % от суммы налоговых и неналоговых доходов":
                case "ИСТОЧНИКИ ФИНАНСИРОВАНИЯ ДЕФИЦИТА":
                case "Кредиты кредитных организаций":
                case "Получение кредитов":
                case "Бюджетные кредиты от других бюджетов бюджетной системы РФ":
                case "Получение кредитов ":
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
            if (e.Row.Cells[0] != null)
            {
                indicatorName = e.Row.Cells[0].ToString();
            }
            bool isInvertIndication = IsInvertIndication(indicatorName.TrimEnd(' '));
            bool noRateIndication = indicatorName == "РАСХОДЫ " || indicatorName == "Профицит (+)/дефицит (-) ";

            string level = String.Empty;
            int levelColumnIndex = 12;
            if (e.Row.Cells[levelColumnIndex] != null)
            {
                level = e.Row.Cells[levelColumnIndex].ToString();
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rate = ((i == 7 || i == 11) && !noRateIndication);
                bool complete = (i == 10);

                switch (level)
                {
                    case "0":
                    case "1":
                    case "2":
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                            break;
                        }
                    case "4":
                        {
                            e.Row.Cells[i].Style.Font.Italic = true;
                            break;
                        }
                }

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 1].Value != null && e.Row.Cells[i + 1].Value.ToString() != string.Empty)
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

                if (complete)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double percent = currentDate.Month * 100.0 / 12;

                        if (Convert.ToDouble(e.Row.Cells[i].Value) < percent)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                            e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:N0}%)", percent);
                        }
                        else
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:N0}%)", percent);
                        }
                        e.Row.Cells[i].Style.Padding.Right = 2;
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
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

        private void SetExportGridParams(UltraWebGrid grid)
        {
            int fontSize = 10;
            foreach (UltraGridColumn column in grid.Columns)
            {
                column.Width = column.Index == 0 ? 380 : 190;
                column.CellStyle.Font.Name = exportFontName;
                column.Header.Style.Font.Name = exportFontName;
                column.CellStyle.Font.Size = fontSize;
                column.Header.Style.Font.Size = fontSize;
            }

            int levelColumnIndex = 12;
            foreach (UltraGridRow row in grid.Rows)
            {
                if (row.Cells[levelColumnIndex].Value != null)
                {
                    string level = row.Cells[levelColumnIndex].Value.ToString();

                    Color rowColor = Color.White;
                    switch (level)
                    {
                        case "0":
                            {
                                rowColor = Color.MediumPurple;
                                break;
                            }
                        case "1":
                            {
                                rowColor = Color.LightGreen;
                                break;
                            }
                        case "2":
                            {
                                rowColor = Color.Khaki;
                                break;
                            }
                    }

                    int opacity = 10;
                    row.Style.BackColor = Color.FromArgb(opacity, rowColor.R, rowColor.G, rowColor.B);
                }
            }
        }
        
        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 40;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;

            string regionStr = "Муниципальное образование";
            if (RegionsNamingHelper.LocalBudgetTypes.ContainsKey(ComboRegion.SelectedValue))
            {
                regionStr = regionType == "МР" ? "Муниципальный район" : "Городской округ";
            }

            ReportExcelExporter1.SetApprovedSection(e.CurrentWorksheet, 1, 4, 3, "Основные показатели бюджета",
                String.Format("по состоянию на 1 {0} {1} года", CRHelper.RusMonthGenitive(nextMonthDate.Month), nextMonthDate.Year),
                String.Format("{1} {0}", ComboRegion.SelectedValue, regionStr));
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            SetExportGridParams(headerLayout.Grid);

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.HeaderCellFont = new Font(exportFontName, 12, FontStyle.Bold);
            ReportExcelExporter1.TitleFont = new Font(exportFontName, 10, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new Font(exportFontName, 10);

            ReportExcelExporter1.Export(headerLayout, 5);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout);
        }

        #endregion
    }
}
