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

namespace Krista.FM.Server.Dashboards.reports.PM_0001_0004
{
    public partial class Default : CustomReportPage
    {

        private IDatabase db;

        private static Dictionary<string, string> dictDates;
        private static Dictionary<string, string> dictPurch;

        #region Поля

        private DataTable gridDt = new DataTable();

        #endregion

        #region Параметры запроса

        // выбранная организация
        private CustomParam refPurchType;
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

            ComboPurchType.Width = 650;
            // Установка размеров
            if (Resolution < 900)
            {
                UltraWebGrid.Width = Unit.Parse("750px");
                ComboPurchType.Width = 325;
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
            UltraWebGrid.Height = Unit.Parse(String.Format("{0}px", Height - 325));
            UltraWebGrid.Grid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.Grid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            
            #endregion

            #region Инициализация параметров запроса

            refPurchType = UserParams.CustomParam("ref_purch_type");
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
                    ComboDate.Title = "Дата публикации заказа";
                    ComboDate.ParentSelect = false;
                    ComboDate.MultiSelect = false;
                    ComboDate.Width = 325;
                    FillDateCombo(ComboDate);

                    ComboPurchType.Title = "Способ закупки";
                    ComboPurchType.ParentSelect = true;
                    ComboPurchType.MultiSelect = false;
                    FillPurchTypeCombo(ComboPurchType);

                }

                if (ComboDate.SelectedNode.Level == 0)
                {
                    ComboDate.SetSelectedNode(ComboDate.SelectedNode.Nodes[0].Nodes[0], true);
                }
                else if (ComboDate.SelectedNode.Level == 1)
                {
                    ComboDate.SetSelectedNode(ComboDate.SelectedNode.Nodes[0], true);
                }

                Page.Title = String.Format("Статистика размещения заказов");
                PageTitle.Text = Page.Title;
                PageSubTitle.Text = String.Format("Данные ежедневного мониторинга размещения заказов на Официальном сайте Российской Федерации для размещения информации о размещении заказов органами власти Ямало-Ненецкого автономного округа за {0}",
                    ComboDate.SelectedValue);

                selectedDate.Value = getDBDate(ComboDate.SelectedValue);
                refPurchType.Value = getPurchType(ComboPurchType.SelectedValue);

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

        private string getPurchType(string selectedValue)
        {
            string dbValue = String.Empty;
            if (dictPurch.TryGetValue(selectedValue, out dbValue))
                return dbValue;
            else
                return "0";
        }

        private void FillDateCombo(CustomMultiCombo ComboYear)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dictDates = new Dictionary<string, string>();
            string query = DataProvider.GetQueryText("PM_0001_0004_date");
            DataTable dtDate = db.ExecQuery(query, QueryResultTypes.DataTable) as DataTable;
            foreach (DataRow row in dtDate.Rows)
            {
                int day   = Convert.ToInt32(row["Date"].ToString().Substring(6, 2));
                int month = Convert.ToInt32(row["Date"].ToString().Substring(4, 2));
                int year  = Convert.ToInt32(row["Date"].ToString().Substring(0, 4));
                DateTime date = new DateTime(year, month, day);
                if (!dict.ContainsKey(String.Format("{0} год", year)))
                {
                    dict.Add(String.Format("{0} год", year), 0);
                }
                if (!dict.ContainsKey(String.Format("{0:MMMM} {1} года", date, year)))
                {
                    dict.Add(String.Format("{0:MMMM} {1} года", date, year), 1);
                }
                if (!dict.ContainsKey(String.Format("{0:D}", date)))
                {
                    dict.Add(String.Format("{0:D}", date), 2);
                    dictDates.Add(String.Format("{0:D}", date), row["Date"].ToString());
                }
            }
            ComboYear.FillDictionaryValues(dict);
            ComboYear.SelectLastNode();
        }

        private void FillPurchTypeCombo(CustomMultiCombo ComboPurchType)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dictPurch = new Dictionary<string, string>();
            string query = DataProvider.GetQueryText("PM_0001_0004_purch_type");
            DataTable dtPurch = db.ExecQuery(query, QueryResultTypes.DataTable) as DataTable;
            foreach (DataRow row in dtPurch.Rows)
            {
                int id = Convert.ToInt32(row["id"]);
                string name = row["Name"].ToString();
                if (!dict.ContainsKey(name))
                {
                    dict.Add(name, 0);
                    dictPurch.Add(name, id.ToString());
                }
            }
            ComboPurchType.FillDictionaryValues(dict);
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("PM_0001_0004_grid");
            gridDt = db.ExecQuery(query, QueryResultTypes.DataTable) as DataTable;

            if (gridDt.Rows.Count > 0)
            {
                if (refPurchType.Value == "1")
                {
                    gridDt.Columns.Remove("MatchDate");
                }
                else if (refPurchType.Value == "3")
                {
                    gridDt.Columns.Remove("ResultDate");
                }
                else if (refPurchType.Value == "4")
                {
                    gridDt.Columns.Remove("MatchDate");
                    gridDt.Columns.Remove("ResultDate");
                    gridDt.Columns.Remove("ConsiderDate");
                }
                else if (refPurchType.Value == "6")
                {
                    gridDt.Columns.Remove("MatchDate");
                    gridDt.Columns.Remove("ResultDate");
                    gridDt.Columns.Remove("ConsiderDate");
                }
                else if (refPurchType.Value == "8")
                {
                    gridDt.Columns.Remove("GiveDate");
                    gridDt.Columns.Remove("MatchDate");
                    gridDt.Columns.Remove("ResultDate");
                    gridDt.Columns.Remove("ConsiderDate");
                }
                else
                {
                    UltraWebGrid.DataTable = null;
                    return;
                }
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

            //UltraWebGrid.AllowColumnSorting = true;

            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;

            e.Layout.HeaderStyleDefault.Height = Unit.Parse("80px");
            band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            band.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            band.Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            band.Columns[0].CellStyle.VerticalAlign = VerticalAlign.Top;
            band.Columns[2].CellStyle.VerticalAlign = VerticalAlign.Top;
            band.Columns[0].CellStyle.Wrap = true;
            band.Columns[2].CellStyle.Wrap = true;
            band.Columns[0].Width = Unit.Parse("250px");
            band.Columns[1].Width = Unit.Parse("250px");
            band.Columns[2].Width = Unit.Parse("250px");
            for (int i = 3; i < band.Columns.Count; ++i)
            {
                band.Columns[i].Width = Unit.Parse("100px");
                band.Columns[i].CellStyle.Wrap = true;
                band.Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            }
            GridHeaderLayout headerLayout = UltraWebGrid.GridHeaderLayout;
            headerLayout.AddCell("Наименование закупки");
            headerLayout.AddCell("Номер извещения");
            headerLayout.AddCell("Заказчик");
            
            if (refPurchType.Value == "1")
            {
                headerLayout.AddCell("Дата вскрытия конвертов с заявками");
                headerLayout.AddCell("Дата рассмотрения заявок");
                headerLayout.AddCell("Дата подведения итогов");
            }
            else if (refPurchType.Value == "3")
            {
                headerLayout.AddCell("Дата окончания сроков подачи заявок");
                headerLayout.AddCell("Дата окончания срока рассмотрения первых частей заявок");
                headerLayout.AddCell("Дата проведения открытого аукциона в электронной форме");
            }
            else if (refPurchType.Value == "4")
            {
                headerLayout.AddCell("Дата окончания сроков подачи заявок");
            }
            else if (refPurchType.Value == "6")
            {
                headerLayout.AddCell("Дата окончания сроков подачи заявок");
            }
            
            headerLayout.AddCell("Ссылка на сведения о закупке");
            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            UltraGridCell linkCell = e.Row.Cells[e.Row.Cells.Count - 1];
            linkCell.Text = String.Format("<a href='{0}'>Подробнее...</a>", linkCell.Value);
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