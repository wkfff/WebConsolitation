using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections.Generic;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

namespace Krista.FM.Server.Dashboards.reports.PM_0001_0003
{
    public partial class Default : CustomReportPage
    {

        #region Поля

        private DataTable gridDt;

        private static MemberAttributesDigest periodDigest;

        private static int columnWidth = 75;

        #endregion

        #region Параметры запроса

        private CustomParam selectedPeriod;

        #endregion

        private static int Resolution
        {
            get { return CRHelper.GetScreenWidth; }
        }

        private static int Height
        {
            get { return CRHelper.GetScreenHeight; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            // Установка размеров
            if (Resolution < 900)
            {
                UltraWebGrid.Width = Unit.Parse("780px");
            }
            else if (Resolution < 1200)
            {
                UltraWebGrid.Width = Unit.Parse("950px");
            }
            else
            {
                UltraWebGrid.Width = Unit.Parse("1260px");
            }

            //UltraWebGrid.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            UltraWebGrid.Height = Unit.Parse(String.Format("300px"));
            UltraWebGrid.Grid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.Grid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            
            #endregion

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            if (!Page.IsPostBack)
            {
                ComboPeriod.Title = "Выберите год";
                ComboPeriod.ParentSelect = false;
                ComboPeriod.MultiSelect = false;
                ComboPeriod.Width = 250;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "PM_0001_0003_date");
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                ComboPeriod.SelectLastNode();
            }

            selectedPeriod.Value = periodDigest.GetMemberUniqueName(ComboPeriod.SelectedValue);

            Page.Title = String.Format("Отчет о показателях работ органа, уполномоченного на осуществление контроля законодательства РФ о размещении заказов для государственных и муниципальных нужд");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("Анализ  показателей работ, уполномоченного на осуществление контроля законодательства РФ о размещении заказов для государственных и муниципальных нужд");

            GridDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("PM_0001_0003_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                UltraWebGrid.DataTable = gridDt;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            if (band.Columns.Count == 0)
            {
                return;
            }

            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowDeleteDefault = AllowDelete.No;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            band.Columns[0].CellStyle.Wrap = true;
            band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            band.Columns[0].Width = Unit.Parse("350px");
            for (int i = 1; i < band.Columns.Count; ++i)
            {
                band.Columns[i].Width = Unit.Parse(String.Format("{0}px", columnWidth));
                CRHelper.FormatNumberColumn(band.Columns[i], "N0");
            }

        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].GetText() == "Согласование возможности заключения государственного (муниципального) контракта  с единственным поставщиком (исполнителем, подрядчиком)" ||
                e.Row.Cells[0].GetText() == "Рассмотрение жалоб")
            {
                e.Row.Style.Font.Bold = true;
                e.Row.Cells[0].ColSpan = e.Row.Cells.Count;
            }
        }

        #endregion
        
        #region Экспорт в Excel

        private void RemoveTags()
        {
            for (int i = 0; i < UltraWebGrid.Grid.Columns.Count; i++)
            {
                foreach (UltraGridRow row in UltraWebGrid.Grid.Rows)
                {
                    UltraGridCell cell = row.Cells[i];
                    if (cell.Value != null)
                    {
                        cell.Value = cell.Value.ToString().Replace("&gt;", String.Empty);
                        cell.Value = Regex.Replace(cell.Value.ToString(), "<[^>]*?>", String.Empty);
                    }
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            RemoveTags();

            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            ReportExcelExporter1.SheetColumnCount = UltraWebGrid.Grid.Columns.Count;
            ReportExcelExporter1.GridColumnWidthScale = 1;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(UltraWebGrid.GridHeaderLayout, String.Empty, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            RemoveTags();

            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(UltraWebGrid.GridHeaderLayout, String.Empty, section1);
        }

        #endregion
    }
}