using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Common;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.ServerLibrary;
using System.Web.SessionState;
using System.Web;
using System.Runtime.Remoting.Messaging;

namespace Krista.FM.Server.Dashboards.reports.NP_0001_0002
{
    public partial class Default : CustomReportPage
    {

        private IDatabase db;

        private static Dictionary<string, string> dictDates;

        #region Поля

        private DataTable gridDt = new DataTable();

        #endregion

        #region Параметры запроса

        // выбранная организация
        private CustomParam selectedProject;
        private CustomParam selectedDate;

        private static string[] shortNames = { "ПНП «Здоровье»", "ПНП «Образование»", "ПНП «Доступное и комфортное жилье – гражданам России»", "ПНП «Развитие АПК»" };
        private static string[] cubeNames = { "DV_NatProject_HlSG", "DV_NatProject_EdSG", "DV_NatProject_HSG", "DV_NatProject_AgSG" };

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

            db = GetDataBase();

            #region Настройка грида

            ComboProject.Width = 600;
            // Установка размеров
            if (Resolution < 900)
            {
                UltraWebGrid.Width = Unit.Parse("750px");
                ComboProject.Width = 450;
            }
            else if (Resolution < 1200)
            {
                UltraWebGrid.Width = Unit.Parse("975px");
            }
            else
            {
                UltraWebGrid.Width = Unit.Parse("1250px");
            }

            //UltraWebGrid.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            UltraWebGrid.Height = Unit.Parse(String.Format("{0}px", Height - 300));
            UltraWebGrid.Grid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.Grid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            
            #endregion

            #region Инициализация параметров запроса

            selectedProject = UserParams.CustomParam("selected_project");
            selectedDate = UserParams.CustomParam("selected_date");

            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            try
            {
                if (!Page.IsPostBack)
                {
                    ComboDate.Title = "Период";
                    ComboDate.ParentSelect = false;
                    ComboDate.MultiSelect = false;
                    ComboDate.Width = 200;
                    FillDateCombo(ComboDate);

                    ComboProject.Title = "Национальный проект";
                    ComboProject.ParentSelect = true;
                    ComboProject.MultiSelect = false;
                    FillProjectCombo(ComboProject);

                }

                Page.Title = String.Format("Мониторинг сведений о выполнении сетевых графиков приоритетных национальных проектов в ХМАО-Югре (по состоянию на {0})",
                    ComboDate.SelectedValue);
                PageTitle.Text = Page.Title;
                PageSubTitle.Text = String.Format("Данные ежемесячного мониторинга сведений о выполнении сетевых графиков приоритетных национальных проектов в ХМАО-Югре (по состоянию на <b>{0}</b>)",
                    ComboDate.SelectedValue);

                selectedDate.Value = getDBDate(ComboDate.SelectedValue);
                selectedProject.Value = cubeNames[Array.IndexOf(shortNames, ComboProject.SelectedValue)];

                //CRHelper.SaveToUserAgentLog(String.Format("Дата - {0}\nПроект - {1}", selectedDate.Value, selectedProject.Value));

                GridDataBind();
            }
            finally
            {
                db.Dispose();
            }
        }

        private string getDBDate(string dateString)
        {
            string dbDate = String.Empty;
            if (dictDates.TryGetValue(dateString, out dbDate))
                return dbDate;
            else
                return "0";
        }

        private void FillDateCombo(CustomMultiCombo ComboYear)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dictDates = new Dictionary<string, string>();
            string query = DataProvider.GetQueryText("NP_0001_0002_date");
            DataTable dtDate = db.ExecQuery(query, QueryResultTypes.DataTable) as DataTable;
            foreach (DataRow row in dtDate.Rows)
            {
                int day = Convert.ToInt32(row["Code"].ToString().PadLeft(8, '0').Substring(0, 2));
                int month = Convert.ToInt32(row["Code"].ToString().PadLeft(8, '0').Substring(2, 2));
                int year = Convert.ToInt32(row["Code"].ToString().PadLeft(8, '0').Substring(4, 4));
                DateTime date = new DateTime(year, month, day);
                if (!dict.ContainsKey(String.Format("{0} год", year)))
                {
                    dict.Add(String.Format("{0} год", year), 0);
                }
                if (!dict.ContainsKey(String.Format("{0:D}", date)))
                {
                    dict.Add(String.Format("{0:D}", date), 1);
                    dictDates.Add(String.Format("{0:D}", date), row["Code"].ToString());
                }
            }
            ComboYear.FillDictionaryValues(dict);
            ComboYear.SelectLastNode();
        }

        private void FillProjectCombo(CustomMultiCombo ComboProject)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            foreach (string key in shortNames)
                dict.Add(key, 0);
            ComboProject.FillDictionaryValues(dict);
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("NP_0001_0002_grid");
            gridDt = db.ExecQuery(query, QueryResultTypes.DataTable) as DataTable;

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
            band.Columns[0].CellStyle.Wrap = true;
            band.Columns[0].Width = Unit.Parse("100px");
            band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            band.Columns[1].CellStyle.Wrap = true;
            band.Columns[1].Width = Unit.Parse("270px");
            band.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            for (int i = 4; i < band.Columns.Count; ++i)
            {
                band.Columns[i].Width = Unit.Parse("200px");
                band.Columns[i].CellStyle.Wrap = true;
            }
            band.Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            band.Columns[2].Width = Unit.Parse("100px");
            band.Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            band.Columns[3].Width = Unit.Parse("100px");

            GridHeaderLayout headerLayout = UltraWebGrid.GridHeaderLayout;
            headerLayout.AddCell("Номер строки сетевого графика");
            headerLayout.AddCell("Наименование");
            headerLayout.AddCell("Плановый срок исполнения");
            headerLayout.AddCell("Фактический срок исполнения");
            headerLayout.AddCell("Сведения об исполнении");
            headerLayout.AddCell("Причины несоблюдения");
            headerLayout.AddCell("Примечания");

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Text = FormatString(e.Row.Cells[0].GetText(), 12);
            e.Row.Cells[2].Text = FormatDateString(e.Row.Cells[2].GetText());
            e.Row.Cells[3].Text = FormatDateString(e.Row.Cells[3].GetText());
        }

        private string FormatString(string stringToFormat, int lenght)
        {
            if (String.IsNullOrEmpty(stringToFormat))
                return stringToFormat;
            string fullLengthString = stringToFormat.PadLeft(lenght, '0');
            string result = String.Empty;
            for (int i = 0; i < fullLengthString.Length; i += 2)
            {
                if (String.IsNullOrEmpty(result))
                    result = fullLengthString.Substring(i, 2);
                else
                    result += "." + fullLengthString.Substring(i, 2);
            }
            return result;
        }

        private string FormatDateString(string stringToFormat)
        {
            if (String.IsNullOrEmpty(stringToFormat))
                return stringToFormat;
            if (stringToFormat.Length == 7)
                return String.Format("{0}.{1}.{2}", stringToFormat.Substring(0, 1), stringToFormat.Substring(1, 2), stringToFormat.Substring(3));
            else if (stringToFormat.Length == 8)
                return String.Format("{0}.{1}.{2}", stringToFormat.Substring(0, 2), stringToFormat.Substring(2, 2), stringToFormat.Substring(4));
            else
                return stringToFormat;
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
    }
}