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
using System.Web;

namespace Krista.FM.Server.Dashboards.reports.STAT_0023_0003_02
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();

        #endregion

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

        private bool IsSmallResolution
        {
            get { return GetScreenWidth < 1000; }
        }

        private int MinScreenWidth
        {
            get { return IsSmallResolution ? 800 : CustomReportConst.minScreenWidth; }
        }

        private int MinScreenHeight
        {
            get { return IsSmallResolution ? 600 : CustomReportConst.minScreenHeight; }
        }

        #region Параметры запроса

        // выбранная организация
        private CustomParam selectedOrganization;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            //GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.Width = IsSmallResolution ? 720 : CRHelper.GetGridWidth(MinScreenWidth - 15);
            //GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.8 - 100);
            GridBrick.Height = IsSmallResolution ? 520 : Convert.ToInt32(MinScreenHeight * 0.8 - 100);
            //GridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 15);
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);
            
            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink.Visible = true;
            CrossLink.Text = "Мониторинг&nbsp;фин.-хоз.деятельности&nbsp;гос.предприятий";
            CrossLink.NavigateUrl = "~/reports/STAT_0023_0003_01/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            Page.Title = String.Format("Реестр государственных предприятий ХМАО-Югры");
            Label1.Text = Page.Title;


            #region Инициализация параметров запроса

            selectedOrganization = UserParams.CustomParam("selected_organization", true);

            #endregion

            GridDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("STAT_0023_0003_02_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                if (gridDt.Columns.Count > 1)
                {
                    gridDt.Columns.RemoveAt(0);
                }

                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.Yes;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
   
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(280);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            headerLayout.AddCell("Наименование");

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(GetColumnWidth(columnName));
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = true;

                headerLayout.AddCell(columnName);
            }

            headerLayout.ApplyHeaderInfo();
        }

        private static int GetColumnWidth(string columnName)
        {
            if (columnName.Contains("Руководители"))
            {
                return 450;
            }
            if ((columnName.Contains("Mail")) || (columnName.Contains("ОКОПФ")))
            {
                return 140;
            }
            if (columnName.Contains("ОКВЭД"))
            {
                return 300;
            }
            if (columnName.ToLower().Contains("адрес"))
            {
                return 200;
            }
            if (columnName.Contains("Место регистрации")) 
            {
                return 200;
            }
            if (columnName.Contains("ОКФС"))
            {
                return 160;
            }

            return 100;
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellCount = e.Row.Cells.Count;

            if (e.Row.Cells[0].Value != null && e.Row.Cells[cellCount - 1].Value != null)
            {
                selectedOrganization.Value = e.Row.Cells[cellCount - 1].Value.ToString();
                e.Row.Cells[0].Value = String.Format("{0} <a href ='../STAT_0023_0003_01/Default.aspx?paramlist=selected_organization={1}'>&gt;&gt;&gt;</a>", e.Row.Cells[0].Value, selectedOrganization.Value);
            }

            if (e.Row.Cells[5].Value != null)
            {
                e.Row.Cells[5].Value = String.Format("<a href ='mailto:{0}'>{0}</a>", e.Row.Cells[5].Value);
            }

            if (e.Row.Cells[cellCount - 2].Value != null)
            {
                e.Row.Cells[cellCount - 2].Value = GetOrgInfo(e.Row.Cells[cellCount - 2].Value.ToString());
            }
        }

        private static string GetOrgInfo(string info)
        {
            info = info.Replace("Руководитель", "<b>Руководитель</b>");
            info = info.Replace("Дата договора", "<br/>Дата договора");
            info = info.Replace("Дата окончания договора", "<br/>Дата окончания договора");
            info = info.Replace("Номер договора", "<br/>Номер договора");
            info = info.Replace("Главный бухгалтер", "<br/><b>Главный бухгалтер</b>");
            info = info.Replace("Главного бухгалтера", "<br/><b>Главный бухгалтер</b>");
            info = info.Replace("Телефон", "<br/>Телефон");
            return info;
        }

        #endregion
        
        #region Экспорт в Excel

        private void RemoveTags()
        {
            for (int i = 0; i < GridBrick.Grid.Columns.Count; i++)
            {
                foreach (UltraGridRow row in GridBrick.Grid.Rows)
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

            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, String.Empty, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            RemoveTags();

            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, String.Empty, section1);
        }

        #endregion
    }
}