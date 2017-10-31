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

namespace Krista.FM.Server.Dashboards.reports.SEP_0005_0001
{
    public partial class Default : CustomReportPage
    {

        #region Поля

        private DataTable gridDt = new DataTable();

        #endregion

        #region Параметры запроса

        private CustomParam selectedYear;

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
                UltraWebGrid.Width = Unit.Parse("725px");
                LabelInfo.Width = Unit.Parse("725px");
            }
            else if (Resolution < 1200)
            {
                UltraWebGrid.Width = Unit.Parse("950px");
                LabelInfo.Width = Unit.Parse("950px");
            }
            else
            {
                UltraWebGrid.Width = Unit.Parse("1250px");
                LabelInfo.Width = Unit.Parse("1250px");
            }

            //UltraWebGrid.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            UltraWebGrid.Height = Unit.Parse(String.Format("{0}px", Height - 350));
            UltraWebGrid.Grid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.Grid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            
            #endregion

            #region Инициализация параметров запроса

            selectedYear = UserParams.CustomParam("selected_year");

            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            if (!Page.IsPostBack)
            {
                ComboDate.Title = "Год";
                ComboDate.ParentSelect = true;
                ComboDate.MultiSelect = false;
                ComboDate.Width = 100;
                FillDateCombo(ComboDate);

            }
            
            selectedYear.Value = ComboDate.SelectedValue;

            Page.Title = String.Format("Мониторинг социально-экономических показателей");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("Ежемесячный мониторинг социально-экономических показателей по утвержденной Минэкономразвития РФ форме, Ханты-Мансийский автономный округ – Югра, за {0} год", selectedYear.Value);

            GridDataBind();
            //UltraWebGrid_MergeCells(UltraWebGrid, 0);
        }

        private void FillDateCombo(CustomMultiCombo ComboDate)
        {
            string query = DataProvider.GetQueryText("SEP_0005_0001_date");
            DataTable dtDate = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Год", dtDate);
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (DataRow row in dtDate.Rows)
                dict.Add(row["Год"].ToString(), 0);
            ComboDate.FillDictionaryValues(dict);
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("SEP_0005_0001_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                gridDt.Columns.RemoveAt(0);
                UltraWebGrid.DataTable = gridDt;
                LabelInfo.Text = "* - Индекс промышленного производства - агрегированный индекс производства по видам деятельности «добыча полезных ископаемых», «обрабатывающие производства»,  «производство и распределение электроэнергии, газа и воды»<br/>";
                LabelInfo.Text += "** - Данные предоставляются ежеквартально<br/>";
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
            //band.Columns[0].MergeCells = true;
            band.Columns[0].Width = Unit.Parse("200px");
            band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            band.Columns[1].CellStyle.Wrap = true;
            band.Columns[1].Width = Unit.Parse("160px");
            band.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            for (int i = 2; i < band.Columns.Count; ++i)
            {
                band.Columns[i].Width = Unit.Parse("80px");
                CRHelper.FormatNumberColumn(band.Columns[i], "N2");
            }
        }

        private static UltraGridCell startCell = null;
        private static int rowSpan = 1;

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[1].GetText() == "Уровень 1")
            {
                e.Row.Cells[0].ColSpan = e.Row.Cells.Count;
                e.Row.Cells[0].Style.Font.Bold = true;
            }
            if (e.Row.Cells[0].GetText() == "Индекс промышленного производства *")
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }
            if (e.Row.Cells[0].GetText().Trim().EndsWith("**"))
            {
                for (int i = 2; i < e.Row.Cells.Count; ++i)
                    if (i % 3 != 1 && e.Row.Cells[i].Value == null)
                    {
                        e.Row.Cells[i].Value = "x";
                        e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Center;
                    }
            }
            UltraGridCell cell = e.Row.Cells[0];
            if (startCell == null || cell.GetText() != startCell.GetText())
            {
                if (startCell != null)
                {
                    startCell.RowSpan = rowSpan;
                }
                startCell = cell;
                rowSpan = 1;
            }
            else
            {
                ++rowSpan;
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

            for (int i = 0; i < UltraWebGrid.Grid.Rows.Count; i++)
            {
                if (UltraWebGrid.Grid.Rows[i].Cells[1].GetText() == "Уровень 1")
                {
                    for (int j = 1; j < UltraWebGrid.Grid.Columns.Count; ++j)
                        UltraWebGrid.Grid.Rows[i].Cells[j].Value = null;
                }
            }

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

            for (int i = 0; i < UltraWebGrid.Grid.Rows.Count; i++)
            {
                if (UltraWebGrid.Grid.Rows[i].Cells[1].GetText() == "Уровень 1")
                {
                    for (int j = 1; j < UltraWebGrid.Grid.Columns.Count; ++j)
                        UltraWebGrid.Grid.Rows[i].Cells[j].Value = null;
                }
            }

            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(UltraWebGrid.GridHeaderLayout, String.Empty, section1);
        }

        #endregion
    }
}