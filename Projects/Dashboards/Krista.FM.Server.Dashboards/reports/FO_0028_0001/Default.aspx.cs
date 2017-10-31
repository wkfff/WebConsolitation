using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core.MemberDigests;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using Krista.FM.Server.Dashboards.Components;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Krista.FM.ServerLibrary;
using System.Web.SessionState;
using System.Web;
using Krista.FM.Common;
using System.Runtime.Remoting.Messaging;

namespace Krista.FM.Server.Dashboards.reports.FO_0028_0001
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2007;
        private int endYear = 2011;
        private GridHeaderLayout headerLayout;
        private int currentYear;
        private DateTime date;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 235);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Динамика&nbsp;погашения&nbsp;задолженности&nbsp;по&nbsp;бюджетным&nbsp;кредитам";
            CrossLink1.NavigateUrl = "~/reports/FO_0028_0002/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            IDatabase db = GetDataBase();

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                endYear = DateTime.Now.Year;
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
            }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue);

            string dateString = String.Empty;
            if (ComboYear.SelectedValues.Contains(DateTime.Now.Year.ToString()))
            {
                string queryDateText = "SELECT MAX([PumpDate]) FROM [novosibirsk].[DV].[PumpHistory] where ProgramIdentifier='FO28Pump'";
                try
                {
                    DataTable dtDate = (DataTable)db.ExecQuery(queryDateText, QueryResultTypes.DataTable);
                    date = (DateTime)(dtDate.Rows[0][0]);
                    dateString = String.Format(" по состоянию на {0:dd.MM.yyyy}", date);
                }
                finally
                {
                    db.Dispose();
                }
            }

            Page.Title = "Бюджетные кредиты из областного бюджета бюджетам муниципальных образований";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Данные за {0}-{1} годы{2}, тыс.руб.", currentYear - 1, currentYear, dateString);

            UserParams.PeriodYear.Value = currentYear.ToString();

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0028_0001_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Муниципальные образования", dtGrid);

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
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            for (int i = 1; i < columnCount; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            
            headerLayout.AddCell("Муниципальные образования");

            GridHeaderCell lastYearCell = headerLayout.AddCell(String.Format("{0} год", (currentYear - 1).ToString()));
            lastYearCell.AddCell("Остаток на начало года", "Остаток основного долга на начало года");
            lastYearCell.AddCell("Выдано", "Сумма предоставленного бюджетного кредита");
            lastYearCell.AddCell("Погашено", "Погашенная сумма основного долга");
            lastYearCell.AddCell("Остаток на конец года", "Остаток основного долга на конец года");

            GridHeaderCell currentYearCell = headerLayout.AddCell(String.Format("{0} год", currentYear.ToString()));
            currentYearCell.AddCell("Остаток на начало года", "Остаток основного долга на начало года");
            currentYearCell.AddCell("Выдано", "Сумма предоставленного бюджетного кредита");
            currentYearCell.AddCell("Погашено", "Погашенная сумма основного долга");
            currentYearCell.AddCell("Остаток на конец года", "Остаток основного долга на конец года");

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string rowName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                rowName = e.Row.Cells[0].Value.ToString();
            }

            if (rowName.ToLower() == "итого")
            {
                e.Row.Style.Font.Bold = true;
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
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

        private static IDatabase GetDataBase()
        {
            try
            {
                HttpSessionState sessionState = HttpContext.Current.Session;
                LogicalCallContextData cnt =
                    sessionState[ConnectionHelper.LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] as LogicalCallContextData;
                if (cnt != null)
                    LogicalCallContextData.SetContext(cnt);
                IScheme scheme = (IScheme)sessionState[ConnectionHelper.SCHEME_KEY_NAME];
                return scheme.SchemeDWH.DB;
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
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
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        #endregion
    }
}