using System;
using System.Data;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.SessionState;
using Krista.FM.Common;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Components;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Drawing;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.Shared;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using System.Text.RegularExpressions;

namespace Krista.FM.Server.Dashboards.reports.EO_0010_0001
{
    public partial class Default : CustomReportPage
    {

        private IDatabase db;

        private CustomParam executor;
        private CustomParam dataSourceID;
        private CustomParam goalArrangID;

        private static Dictionary<string, string> dictDataSource;
        private static Dictionary<string, string> dictExecutor;

        private GridHeaderLayout headerLayout;

        private static int Resolution
        {
            get { return CRHelper.GetScreenWidth; }
        }

        private static int gridWidth;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            db = GetDataBase();

            Page.Title = "Послание президента";
            PageTitle.Text = "Отчет об исполнении мероприятий в рамках задач, сформированных в соответствии с ежегодным посланием Президента РФ Федеральному собранию РФ";


            // Установка размеров
            if (Resolution < 900)
            {
                Grid.Width = Unit.Parse("725px");
                ComboYears.Width = 150;
                ComboExecutor.Width = 500;
            }
            else if (Resolution < 1200)
            {
                Grid.Width = Unit.Parse("950px");
                ComboYears.Width = 150;
                ComboExecutor.Width = 700;
            }
            else
            {
                Grid.Width = Unit.Parse("1200px");
                ComboYears.Width = 150;
                ComboExecutor.Width = 850;
            }

            HeaderTable.Width = String.Format("{0}px", (int)Grid.Width.Value);

            #region Грид

            Grid.Height = Unit.Empty;

            Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            Grid.DataBinding += new EventHandler(Grid_DataBinding);
            Grid.DisplayLayout.NoDataMessage = "Нет данных";
            
            #endregion

            executor = UserParams.CustomParam("executor");
            dataSourceID = UserParams.CustomParam("data_source_id");
            goalArrangID = UserParams.CustomParam("goal_arrang_id");
                        
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            try
            {
                if (!Page.IsPostBack)
                {
                    ComboYears.Title = "Период";
                    ComboYears.ParentSelect = true;
                    FillYearsDictionary(ComboYears);
                    
                    ComboExecutor.Title = "Исполнитель";
                    ComboExecutor.ParentSelect = true;
                    FillExecutorsDictionary(ComboExecutor);
                }

                executor.Value = GetExecutor(ComboExecutor.SelectedValue);
                dataSourceID.Value = GetDataSourceID(ComboYears.SelectedValue);

                Grid.DataBind();
            }
            finally
            {
                db.Dispose();
            }
            
            #region Экспорт
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;
            
            #endregion

        }

        private void Grid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("EO_0010_0001_main");
            DataTable dtMain = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("Код", typeof(string));
            dtGrid.Columns.Add("Описание", typeof(string));
            string prevParentCode = String.Empty;
            DataRow newRow;
            foreach (DataRow row in dtMain.Rows)
            {
                string parentCode = ConvertCode(row["ParentCode"].ToString());
                if (parentCode != prevParentCode)
                {
                    prevParentCode = parentCode;
                    newRow = dtGrid.NewRow();
                    newRow["Код"] = String.Format("{0}", parentCode);
                    newRow["Описание"] = String.Format("<b>{0}</b>", row["Parent"]);
                    dtGrid.Rows.Add(newRow);
                }
                newRow = dtGrid.NewRow();
                newRow["Код"] = String.Format("{0}", ConvertCode(row["Code"].ToString()));
                newRow["Описание"] = String.Format("{0}<br/>(<i>Срок: {1}, исполнитель: {2}</i>)", row["Name"], row["Period"], row["ExecutorName"]);
                dtGrid.Rows.Add(newRow);

                goalArrangID.Value = row["id"].ToString();
                query = DataProvider.GetQueryText("EO_0010_0001_detail");
                DataTable dtDetail = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
                if (dtDetail.Rows.Count > 0)
                {
                    string desciption = String.Empty;
                    string separator = String.Empty;
                    foreach (DataRow detailRow in dtDetail.Rows)
                    {
                        desciption += String.Format("{0}<b>{1:dd.MM.yyyy}</b> - {2}", separator, ConvertDate(detailRow["Period"].ToString()), detailRow["report"]);
                        separator = "<br/>";
                    }
                    newRow = dtGrid.NewRow();
                    newRow["Описание"] = desciption;
                    dtGrid.Rows.Add(newRow);
                }

            }

            (sender as UltraWebGrid).DataSource = dtGrid;
        }

        private void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            UltraWebGrid grid = sender as UltraWebGrid;

            UltraGridBand band = grid.Bands[0];
            band.Columns[0].Width = Unit.Parse("50px");
            band.Columns[1].Width = Unit.Parse(String.Format("{0}px", (int)Grid.Width.Value - 90));
            band.Columns[1].CellStyle.Wrap = true;

            grid.DisplayLayout.RowSelectorStyleDefault.Width = Unit.Parse("20px");
            grid.DisplayLayout.AllowColSizingDefault = AllowSizing.Fixed;
            grid.DisplayLayout.AllowColumnMovingDefault = AllowColumnMoving.None;
            grid.DisplayLayout.AllowDeleteDefault = AllowDelete.No;
            grid.DisplayLayout.AllowSortingDefault = AllowSorting.No;
            grid.DisplayLayout.RowAlternateStylingDefault = DefaultableBoolean.False; 

            headerLayout = new GridHeaderLayout(grid);
            headerLayout.AddCell("№ п/п");
            headerLayout.AddCell("Задачи, мероприятия, отчет об исполнении");
            headerLayout.ApplyHeaderInfo();
        }

        private DateTime ConvertDate(string date)
        {
            int year = Convert.ToInt32(date.Substring(0, 4));
            int month = Convert.ToInt32(date.Substring(4, 2));
            int day = Convert.ToInt32(date.Substring(6));
            return new DateTime(year, month, day);
        }

        private string ConvertCode(string sourceCode)
        {
            return sourceCode.Substring(0, sourceCode.Length - 2) + "." + sourceCode.Substring(sourceCode.Length - 2);
        }

        private string GetExecutor(string key)
        {
            string value;
            if (dictExecutor.TryGetValue(key, out value))
            {
                return String.Format("and e.ID = {0}", value);
            }
            else
            {
                return String.Empty;
            }
        }

        private string GetDataSourceID(string key)
        {
            string value;
            dictDataSource.TryGetValue(key, out value);
            return value;
        }

        private void FillYearsDictionary(CustomMultiCombo ComboYears)
        {
            string query = DataProvider.GetQueryText("EO_0010_0001_date");
            DataTable dtYears = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            dictDataSource = new Dictionary<string, string>();
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (DataRow row in dtYears.Rows)
            {
                dict.Add(row["Year"].ToString(), 0);
                dictDataSource.Add(row["Year"].ToString(), row["ID"].ToString());
            }
            ComboYears.FillDictionaryValues(dict);
            ComboYears.SelectLastNode();
        }

        private void FillExecutorsDictionary(CustomMultiCombo ComboExecutor)
        {
            string query = DataProvider.GetQueryText("EO_0010_0001_executor");
            DataTable dtExec = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dict.Add("Все", 0);
            dictExecutor = new Dictionary<string, string>();
            foreach (DataRow row in dtExec.Rows)
            {
                dict.Add(row["Name"].ToString(), 0);
                dictExecutor.Add(row["Name"].ToString(), row["ID"].ToString());
            }
            ComboExecutor.FillDictionaryValues(dict);
        }

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

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            foreach (UltraGridRow row in Grid.Rows)
                row.Cells[1].Value = Regex.Replace(row.Cells[1].Text, "<[\\s\\S]*?>", String.Empty);

            ReportPDFExporter1.HeaderCellHeight = 20;
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");

            SetExportGridParams(headerLayout.Grid);

            ReportExcelExporter1.HeaderCellHeight = 25;
            ReportExcelExporter1.HeaderCellFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleFont = new Font("Verdana", 12, FontStyle.Bold);
            ReportExcelExporter1.SubTitleFont = new Font("Verdana", 11);
            ReportExcelExporter1.TitleAlignment = HorizontalCellAlignment.Left;
            ReportExcelExporter1.TitleStartRow = 0;

            foreach (UltraGridRow row in Grid.Rows)
                row.Cells[1].Value = Regex.Replace(row.Cells[1].Text.Replace("<br/>", "\n"), "<[\\s\\S]*?>", String.Empty);

            ReportExcelExporter1.Export(headerLayout, sheet1, 1);
        }

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0.25;
            e.CurrentWorksheet.PrintOptions.ScalingType = ScalingType.FitToPages;
        }

        private static void SetExportGridParams(UltraWebGrid grid)
        {
            string exportFontName = "Verdana";
            int fontSize = 10;
            double coeff = 1.1;
            foreach (UltraGridColumn column in grid.Columns)
            {
                column.Width = Convert.ToInt32(column.Width.Value * coeff);
                column.CellStyle.Font.Name = exportFontName;
                column.Header.Style.Font.Name = exportFontName;
                column.CellStyle.Font.Size = fontSize;
                column.Header.Style.Font.Size = fontSize;
            }
        }

        #endregion

    }
}
