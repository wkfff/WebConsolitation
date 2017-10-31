using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0013_01
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2000;
        private int endYear = 2011;
		private GridHeaderLayout headerLayout;
        #endregion

        #region Параметры запроса

        // Уровень районов
        private CustomParam regionsLevel;
        // консолидированный бюджет субъекта
        private CustomParam regionsConsolidateBudget;
        // доходы всего
        private CustomParam incomesAll;
        // расходы всего
        private CustomParam outcomesAll;

        // тип документа СКИФ для консолидированного бюджета субъекта
        private CustomParam consolidateBudgetDocumentSKIFType;
        // тип документа СКИФ для районов
        private CustomParam regionBudgetDocumentSKIFType;

        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        #endregion

        private double rubMultiplier;

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "Тыс.руб." : "Млн.руб."; }
        }
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 120);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (regionsConsolidateBudget == null)
            {
                regionsConsolidateBudget = UserParams.CustomParam("regions_consolidate_budget");
            }
            if (incomesAll == null)
            {
                incomesAll = UserParams.CustomParam("incomes_all");
            }
            if (outcomesAll == null)
            {
                outcomesAll = UserParams.CustomParam("outcomes_all");
            }

            if (consolidateBudgetDocumentSKIFType == null)
            {
                consolidateBudgetDocumentSKIFType = UserParams.CustomParam("consolidate_budget_document_skif_type");
            }
            if (regionBudgetDocumentSKIFType == null)
            {
                regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            }

            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }

            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.FKRAllLevel.Value = RegionSettingsHelper.Instance.FKRAllLevel;
            UserParams.FKRSectionLevel.Value = RegionSettingsHelper.Instance.FKRSectionLevel;
            UserParams.FKRSubSectionLevel.Value = RegionSettingsHelper.Instance.FKRSubSectionLevel;
            UserParams.EKRDimension.Value = RegionSettingsHelper.Instance.EKRDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;

            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0013_01_date");
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
            }

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateBudget.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesAll.Value = RegionSettingsHelper.Instance.IncomeTotal;
            outcomesAll.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;

            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");

            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            Page.Title = "Исполнение бюджета в разрезе отдельных показателей";
            PageTitle.Text = Page.Title;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            PageSubTitle.Text = string.Format("Информация за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString(); 
            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;

            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;
            UltraWebGrid.Bands.Clear();
			headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0013_01_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length - 1; i++)
                    {
                        // номер в тройке
                        int j = (i - 1) % 3;
                        if ((j == 0 || j == 1) && row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) / rubMultiplier;
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

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 1)
            {
                string formatString = "P2";
                int widthColumn = 80;

               // номер в тройке
                int j = (i - 1) % 3;
                switch (j)
                {
                    case 0:
                    case 1:
                        {
                            formatString = "N2";
                            widthColumn = 140;
                            break;
                        }
                    case 2:
                        {
                            formatString = "P2";
                            widthColumn = 85;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(220);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
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
			headerLayout.AddCell("Наименование");
			for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 3)
			{
			bool isLastColumn =  i == (e.Layout.Bands[0].Columns.Count - 3);
			string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
			GridHeaderCell cell = headerLayout.AddCell(captions[0]);
			cell.AddCell(String.Format("Назначено, {0}", RubMultiplierCaption.ToLower()), "Плановые назначения на год");
			cell.AddCell(String.Format("Исполнено, {0}", RubMultiplierCaption.ToLower()), "Фактическое исполнение нарастающим итогом с начала года");			
			if (!isLastColumn)
                {
                    cell.AddCell("Исполнено %", "Процент выполнения назначений/ Оценка равномерности исполнения (1/12 годового плана в месяц)");
                }
			}
			headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count - 1; i++)
            {
                int j = (i - 1) % 3;
                bool complete = (j == 2);

                e.Row.Cells[i].Style.Padding.Right = 2;

                if (complete)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
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
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
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

                    if (e.Row.Cells[0].Value != null && 
                        (e.Row.Cells[0].Value.ToString().ToLower().Contains("бюджет") || 
                         e.Row.Cells[0].Value.ToString().ToLower().Contains("городские округа") ||
                         e.Row.Cells[0].Value.ToString().ToLower().Contains("муниципальные районы")))
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (UltraWebGrid.Rows.Count < 30)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        #endregion

        #region Экспорт в Excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            sheet1.Rows[0].Cells[0].Value = PageTitle.Text;
			sheet1.Rows[1].Cells[0].Value = PageSubTitle.Text;
            ReportExcelExporter1.HeaderCellHeight = 70;
            ReportExcelExporter1.GridColumnWidthScale = 1.3;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }
       
        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 40;
            ReportPDFExporter1.Export(headerLayout, PageSubTitle.Text, section1);
        }

        #endregion
    }
}
