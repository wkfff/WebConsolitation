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

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0063
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2000;
        private int endYear = 2011;
        private string multiplierCaption;
        private int rubMulti = 1000;
        private DateTime date;

        private CustomParam periodYear;
        private CustomParam periodMonth;
        private CustomParam periodHalfYear;
        private CustomParam periodQuater;

        private CustomParam periodPrevYear;
        #endregion

        #region Параметры запроса

        // выбранный множитель рублей
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

        private MemberAttributesDigest budgetDigest; private CustomParam levelBudget;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 180);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";


            #region Инициализация параметров запроса

            periodYear = UserParams.CustomParam("period_year");
            periodMonth = UserParams.CustomParam("period_month");
            periodHalfYear = UserParams.CustomParam("period_halfyear");
            periodQuater = UserParams.CustomParam("period_quater");
            periodPrevYear = UserParams.CustomParam("period_prev_year");
            levelBudget = UserParams.CustomParam("level_budget");
            rubMultiplier = UserParams.CustomParam("rubMultiplier");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            //firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));
            firstYear = 2008;


            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;


            budgetDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0063_level_budget");

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0063_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 150;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);

                ComboBudgetLevel.Title = "Уровень бюджета ";
                ComboBudgetLevel.Width = 400;
                ComboBudgetLevel.MultiSelect = false;
                ComboBudgetLevel.ParentSelect = true;
                ComboBudgetLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetDigest.UniqueNames, budgetDigest.MemberLevels));
                ComboBudgetLevel.SetСheckedState("Консолидированный бюджет субъекта", true);
            }

            levelBudget.Value = budgetDigest.GetMemberUniqueName(ComboBudgetLevel.SelectedValue);

            int selIndex = RubMiltiplierButtonList.SelectedIndex;
            if (selIndex == 0)
            {
                multiplierCaption = "руб.";
                rubMulti = 1;
            }
            else
            {
                if (selIndex == 1)
                {
                    multiplierCaption = "тыс.руб.";
                    rubMulti = 1000;
                }
                else
                {
                    multiplierCaption = "млн.руб.";
                    rubMulti = 1000000;
                }
            }
            rubMultiplier.Value = string.Format("{0}", rubMulti);
            string msgSubTitle = "";
            PageTitle.Text = "Информация о результатах исполнения бюджетов";

            if (ComboMonth.SelectedIndex + 2 == 13)
            { msgSubTitle = string.Format("01.{0:00}.{1} года", 1, (Convert.ToInt16(ComboYear.SelectedValue) + 1).ToString()); }
            else
            { msgSubTitle = string.Format("01.{0:00}.{1} года", ComboMonth.SelectedIndex + 2, ComboYear.SelectedValue); }

            PageSubTitle.Text = string.Format("{0}, данные на {1}", ComboBudgetLevel.SelectedValue, msgSubTitle); ;
            Page.Title = PageTitle.Text;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            date = new DateTime(yearNum, monthNum, 1).AddMonths(1);

            periodYear.Value = ComboYear.SelectedValue;
            periodMonth.Value = ComboMonth.SelectedValue;
            periodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            periodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            periodPrevYear.Value = string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1);

            levelBudget.Value = ComboBudgetLevel.SelectedValue;

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();


        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {

            string query = DataProvider.GetQueryText("FO_0002_0063_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                decimal sum = 0, sum2 = 0;
                for (int i = 0; i < dtGrid.Rows.Count - 2; i++)
                {
                    if (dtGrid.Rows[i][3] != DBNull.Value)
                    {
                        sum += Convert.ToDecimal(dtGrid.Rows[i][3]);
                    }
                    if (dtGrid.Rows[i][4] != DBNull.Value)
                    {
                        sum2 += Convert.ToDecimal(dtGrid.Rows[i][4]);
                    }
                }
                dtGrid.Rows[dtGrid.Rows.Count - 2][3] = sum;
                dtGrid.Rows[dtGrid.Rows.Count - 2][4] = sum2;
                if ((dtGrid.Rows[dtGrid.Rows.Count - 2][1] != DBNull.Value) && (dtGrid.Rows[dtGrid.Rows.Count - 2][3] != DBNull.Value))
                {
                    if (Convert.ToDecimal(dtGrid.Rows[dtGrid.Rows.Count - 2][3]) != 0)
                    {
                        dtGrid.Rows[dtGrid.Rows.Count - 2][5] = Convert.ToDecimal(dtGrid.Rows[dtGrid.Rows.Count - 2][1]) / Convert.ToDecimal(dtGrid.Rows[dtGrid.Rows.Count - 2][3]);
                    }
                }

                if ((dtGrid.Rows[dtGrid.Rows.Count - 2][2] != DBNull.Value) && (dtGrid.Rows[dtGrid.Rows.Count - 2][4] != DBNull.Value))
                {
                    if (Convert.ToDecimal(dtGrid.Rows[dtGrid.Rows.Count - 2][4]) != 0)
                    {
                        dtGrid.Rows[dtGrid.Rows.Count - 2][6] = Convert.ToDecimal(dtGrid.Rows[dtGrid.Rows.Count - 2][2]) / Convert.ToDecimal(dtGrid.Rows[dtGrid.Rows.Count - 2][4]);
                    }
                }

                dtGrid.Rows[dtGrid.Rows.Count - 1][0] = "Областной бюджет";
                if (ComboBudgetLevel.SelectedValue == "Бюджет субъекта")
                { dtGrid.Rows.RemoveAt(dtGrid.Rows.Count - 2); }
                if (dtGrid.Rows[dtGrid.Rows.Count - 1][1] == DBNull.Value)
                {
                    dtGrid.Rows.RemoveAt(dtGrid.Rows.Count - 1);
                }

                if (dtGrid.Rows[0][2] != DBNull.Value)
                { UltraWebGrid.DataSource = dtGrid; }
                else
                    if (ComboBudgetLevel.SelectedValue == "Бюджет субъекта")
                    { UltraWebGrid.DataSource = dtGrid; }
            }

        }


        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(160);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(158);
            }

            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;

            string date = "";
            headerLayout.AddCell("Бюджет");
            headerLayout.AddCell(String.Format("Исполнение бюджета на 01.01.{0} года", (ComboYear.SelectedValue)),
                String.Format("Результат исполнения бюджета на 01.01.{0} года (- дефицит, + профицит)",
                (ComboYear.SelectedValue)));
            if (ComboMonth.SelectedIndex + 2 == 13)
            { date = string.Format("01.{0:00}.{1} года", 1, (Convert.ToInt16(ComboYear.SelectedValue) + 1).ToString()); }
            else
            { date = string.Format("01.{0:00}.{1} года", ComboMonth.SelectedIndex + 2, ComboYear.SelectedValue); }
            headerLayout.AddCell(String.Format("Исполнение бюджета на {0}", date),
                String.Format("Результат исполнения бюджета на {0} (- дефицит, + профицит)", date));
            headerLayout.AddCell(String.Format("Доходы без учета безвозмездных поступлений на 01.01.{0} года", (ComboYear.SelectedValue)),
                String.Format("Доходы бюджета без учета безвозмездных поступлений на 01.01.{0} года", ComboYear.SelectedValue));
            headerLayout.AddCell(String.Format("Доходы без учета безвозмездных поступлений на {0}", date),
                String.Format("Доходы бюджета без учета безвозмездных поступлений на {0}", date));
            headerLayout.AddCell(String.Format("Отношение дефицита/профицита к доходам бюджета на 01.01.{0} года", ComboYear.SelectedValue),
                String.Format("Отношение дефицита/профицита к доходам бюджета без учета безвозмездных поступлений на 01.01.{0} года", ComboYear.SelectedValue));
            headerLayout.AddCell(String.Format("Отношение дефицита/профицита к доходам бюджета на {0}", date),
                String.Format("Отношение дефицита/профицита к доходам бюджета без учета безвозмездных поступлений на {0}", date));
            headerLayout.ApplyHeaderInfo();
        }




        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            SetRankImg(e.Row, 1);
            SetRankImg(e.Row, 2);
            if (e.Row.Cells[0].ToString() == "Итого")
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
            if (e.Row.Cells[0].ToString() == "Областной бюджет")
            {
                e.Row.Cells[0].Style.Font.Bold = true;
                e.Row.Cells[0].Style.Height = 30;
            }
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
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

        private static void SetRankImg(UltraGridRow row, int i)
        {
            string css = string.Empty;
            if (row.Cells[i].Value != null)
            {

                if ((Convert.ToDouble(row.Cells[i].Value)) >= 1)
                {
                    css = "~/images/ballGreenBB.png";
                }
                else
                {
                    css = "~/images/ballRedBB.png";
                }
            }
            if (css != string.Empty)
            {
                row.Cells[i].Style.BackgroundImage = css;
                row.Cells[i].Title = (css == "~/images/ballRedBB.png")
                   ? "Дефицит"
                   : "Профицит";
            }
            row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: 30% center; margin: 2px";
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

        #endregion
    }
}
