using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
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

namespace Krista.FM.Server.Dashboards.reports.FO_0003_0007
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart;
        private int firstYear = 2008;
        private int endYear = 2011;
        private string FilterParams;
        
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

        // фильтр по КОСГУ
        private CustomParam kosguFilter;

        #endregion

       private GridHeaderLayout headerLayout;
       private DateTime currentDate;
        
       private MemberAttributesDigest budgetDigest;

       private int GetScreenWidth
       {
           get
           {
               if (Request.Cookies != null)
               {
                   if (Request.Cookies[CustomReportConst.ScreenWidthKeyName] != null)
                   {
                       HttpCookie cookie = Request.Cookies[CustomReportConst.ScreenWidthKeyName];
                       int value = Int32.Parse(cookie.Value);
                       return value;
                   }
               }
               return (int)Session["width_size"];
           }
       }

       private bool IsSmallResolution1200
       {
           get { return GetScreenWidth < 1200; }
       }

       private bool IsSmallResolution900
       {
           get { return GetScreenWidth < 900; }
       }


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = IsSmallResolution900 ? 750 : IsSmallResolution1200 ? 950: CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = Unit.Empty;

            #region Инициализация параметров запроса

            selectedRegion = UserParams.CustomParam("selected_region");
            documentType = UserParams.CustomParam("document_type");
            consolidateLevel = UserParams.CustomParam("consolidate_level");
            budgetSKIFLevel = UserParams.CustomParam("budget_SKIF_level");
            outcomesFKRTotal = UserParams.CustomParam("outcomes_fkr_total");
            isNonCompareYear = UserParams.CustomParam("is_non_compare_year");
            rzprExtrudeFilter = UserParams.CustomParam("rzpr_extrude_filter");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");

            kosguFilter = UserParams.CustomParam("kosgu_filter");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            firstYear = 2009;
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
            UserParams.RegionsLocalBudgetLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionsLocalBudgetLevel");

            budgetDigest = MemberDigestHelper.Instance.LocalBudgetDigest;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0003_0007_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                CRHelper.SaveToErrorLog(firstYear.ToString());
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);
                
                ComboBudgetLevel.Title = "Уровень бюджета";
                ComboBudgetLevel.Width = 400;
                ComboBudgetLevel.MultiSelect = false;
                ComboBudgetLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetDigest.UniqueNames, budgetDigest.MemberLevels));
                ComboBudgetLevel.SetСheckedState("Консолидированный бюджет субъекта", true);
            }

            selectedRegion.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            documentType.Value = "[ТипДокумента].[СКИФ].[Консолидированная отчетность и отчетность внебюджетных территориальных фондов]";
            outcomesFKRTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;

            kosguFilter.Value = RegionSettingsHelper.Instance.GetPropertyValue("KosguFilter");

            switch (ComboBudgetLevel.SelectedValue)
            {
                case "Консолидированный бюджет субъекта":
                    {
                        budgetSKIFLevel.Value = "[Уровни бюджета].[СКИФ].[Конс.бюджет субъекта]";
                        break;
                    }
                case "Местные бюджеты":
                    {
                        documentType.Value = "[ТипДокумента].[СКИФ].[Консолидированная отчетность и отчетность внебюджетных территориальных фондов]";
                        budgetSKIFLevel.Value = "[Уровни бюджета].[СКИФ].[Все].[Конс.бюджет МО]";
                        selectedRegion.Value = RegionSettingsHelper.Instance.RegionsLocalBudgetLevel;
                        break;
                    }
                default:
                    {
                        if (ComboBudgetLevel.SelectedValue == RegionSettingsHelper.Instance.OwnSubjectBudgetName)
                        {
                            budgetSKIFLevel.Value = "[Уровни бюджета].[СКИФ].[Бюджет субъекта]";
                        }
                        else
                        {
                            selectedRegion.Value = budgetDigest.GetMemberUniqueName(ComboBudgetLevel.SelectedValue);
                            string regionType = budgetDigest.GetMemberType(ComboBudgetLevel.SelectedValue);
                            if (regionType.Contains("МР"))
                            {
                                budgetSKIFLevel.Value = "[Уровни бюджета].[СКИФ].[Бюджет гор.округа, мун.района и поселения]";
                                documentType.Value = "[ТипДокумента].[СКИФ].[Все].[Данные конс. отчета субъекта в разрезе городов и районов (5+7+10)]";
                            }
                            else if (regionType.Contains("ГО"))
                            {
                                budgetSKIFLevel.Value = "[Уровни бюджета].[СКИФ].[Бюджет гор.округа, мун.района и поселения]";
                                documentType.Value = "[ТипДокумента].[СКИФ].[Все].[Данные конс. отчета субъекта в разрезе городов и районов (5+7+10)]";
                            }
                        }
                        break;
                    }
            }

            Page.Title = "Мониторинг расходов бюджета на охрану окружающей среды";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = "Ежемесячные данные по мониторингу расходов бюджета на охрану окружающей среды";
            

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, monthNum, 1);
            
            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);
            
            string titlemonth = string.Empty;
            if (ComboMonth.SelectedValue != "Январь")
            {
                titlemonth = String.Format("за январь - {0}", ComboMonth.SelectedValue.ToLower());
            }
            else
            {
                titlemonth = "за январь";
            }

            FilterParams = String.Format("<br/>{0} {1} года, {2}", titlemonth, ComboYear.SelectedValue, ComboBudgetLevel.SelectedValue.ToLower());
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();     
            UltraWebGrid.DataBind();
            
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0003_0007_grid");
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
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

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
            e.Layout.Bands[0].Columns[1].Width = IsSmallResolution1200 || IsSmallResolution900 ? 200 : CRHelper.GetColumnWidth(330);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            SetColumnParams(e.Layout, 0, 0, "00 00" , 50, false);

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

            headerLayout.AddCell("Код");
            headerLayout.AddCell("Наименование", "Наименование");

            GridHeaderCell groupCell = headerLayout.AddCell(String.Format("на {0:dd.MM.yyyy} г.", currentDate.AddMonths(1)));

            groupCell.AddCell(String.Format("Уточненные годовые назначения, руб."), String.Format("Годовые назначения расходов на {0} год", currentDate.Year));
            groupCell.AddCell(String.Format("Факт, руб."), String.Format("Исполнено {0}", groupCell.Caption));
            groupCell.AddCell("% исполнения", "% исполнения годовых назначений расходов");
            groupCell.AddCell("Ранг % исп.", "Ранг (место) статьи по % исполнения годовых назначений расходов");
            groupCell.AddCell("Доля", "Удельный вес статьи фактических расходов в общей сумме расходов");
            groupCell.AddCell("Ранг доля", "Ранг (место) статьи по удельному весу фактических расходов в общей сумме расходов");

           GridHeaderCell compareGroupCell = headerLayout.AddCell(String.Format("{0} год к {1} году", currentDate.Year, currentDate.Year - 1));
           compareGroupCell.AddCell(String.Format("Отклонение(+,-), руб."), "");
           compareGroupCell.AddCell("Темп роста, %", "");
           compareGroupCell.AddCell("Рост (снижение)", "");
            

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
            double widthMultiplier = 1;
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
        }

        private void AddTitleText(ISection section)
        {
            string label = FilterParams.Replace("<br/>", "");
            IText title = section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);


        }

        #endregion

     
    }
}
