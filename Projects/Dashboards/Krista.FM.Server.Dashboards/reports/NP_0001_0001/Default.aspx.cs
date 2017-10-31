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

namespace Krista.FM.Server.Dashboards.reports.NP_0001_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();

        #endregion

        #region Параметры запроса

        // выбранная организация
        private CustomParam selectedProject;
        private CustomParam selectedYear;
        private CustomParam selectedDate;
        private CustomParam selectedLevel;
        private CustomParam additionalMeasures;

        private static string[] shortNames = { "ПНП «Здоровье»", "ПНП «Образование»", "ПНП «Доступное и комфортное жилье – гражданам России»", "ПНП «Развитие АПК»" };
        private static string[] cubeNames = { "Здоровье ПС", "Образование ПС", "Жилье ПС", "АПК ПС" };
        private static string[] level = { "6", "4", "4", "3" };

        #endregion

        private static Dictionary<string, string> dictDates;

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
            }
            else if (Resolution < 1200)
            {
                UltraWebGrid.Width = Unit.Parse("950px");
            }
            else
            {
                UltraWebGrid.Width = Unit.Parse("1200px");
            }

            //UltraWebGrid.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            UltraWebGrid.Height = Unit.Parse(String.Format("{0}px", Height - 300));
            UltraWebGrid.Grid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.Grid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            
            #endregion

            #region Инициализация параметров запроса

            selectedProject = UserParams.CustomParam("selected_project");
            selectedLevel = UserParams.CustomParam("selected_level");
            selectedYear = UserParams.CustomParam("selected_year");
            selectedDate = UserParams.CustomParam("selected_date");
            additionalMeasures = UserParams.CustomParam("additional_measures");

            #endregion
            
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            if (!Page.IsPostBack)
            {
                ComboDate.Title = "Выберите период";
                ComboDate.ParentSelect = true;
                ComboDate.MultiSelect = false;
                ComboDate.Width = 300;
                FillDateCombo(ComboDate);

                ComboProject.Title = "Национальный проект";
                ComboProject.ParentSelect = true;
                ComboProject.MultiSelect = false;
                ComboProject.Width = 360;
                FillProjectCombo(ComboProject);

            }

            Page.Title = String.Format("Мониторинг финансирования приоритетных национальных проектов в ХМАО-Югре (по состоянию на выбранную дату)");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("Данные ежегодного (ежеквартального, ежемесячного) мониторинга финансирования приоритетных национальных проектов в ХМАО-Югре (по состоянию на выбранную дату)");

            string s;
            dictDates.TryGetValue(ComboDate.SelectedValue, out s);
            selectedDate.Value = s;
            selectedYear.Value = s.Substring(58, 4);
            selectedProject.Value = cubeNames[Array.IndexOf(shortNames, ComboProject.SelectedValue)];
            selectedLevel.Value = level[Array.IndexOf(shortNames, ComboProject.SelectedValue)];
            if (selectedProject.Value == "Жилье ПС")
            {
                additionalMeasures.Value = ", [Measures].[Плановое финансирование Фонд ЖКХ ], [Measures].[Кассовое исполнение Фонд ЖКХ ]";
            }
            else
            {
                additionalMeasures.Value = String.Empty;
            }

            GridDataBind();
        }

        private void FillDateCombo(CustomMultiCombo ComboDate)
        {
            string[] uniqueNames = new string[0];
            string[] memberKeys = new string[0];
            foreach (string cube in cubeNames)
            {
                selectedProject.Value = cube;
                if (cube == "Жилье ПС")
                {
                    additionalMeasures.Value = ", [Measures].[Плановое финансирование Фонд ЖКХ], [Measures].[Кассовое исполнение Фонд ЖКХ]";
                }
                else
                {
                    additionalMeasures.Value = String.Empty;
                }
                string query = DataProvider.GetQueryText("NP_0001_0001_date");
                DataTable dtDates = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtDates);
                foreach (DataRow row in dtDates.Rows)
                {
                    string period = row["Период"].ToString();
                    if (Array.IndexOf(uniqueNames, period) != -1)
                        continue;
                    string code = GetConvertedCode(row["Ключ"].ToString(), row["Уровень"].ToString());
                    Array.Resize(ref uniqueNames, uniqueNames.Length + 1);
                    Array.Resize(ref memberKeys, memberKeys.Length + 1);
                    uniqueNames[uniqueNames.Length - 1] = period;
                    memberKeys[memberKeys.Length - 1] = code;
                }
            }
            Dictionary<string, int> dict = new Dictionary<string, int>();
            dictDates = new Dictionary<string, string>();
            Array.Sort(memberKeys, uniqueNames);
            for (int i = 0; i < memberKeys.Length; ++i)
            {
                string code = memberKeys[i];
                string uniqueName = uniqueNames[i];
                int level = GetLevel(code);
                string year, halfYear, quarter, month, day;
                ParseCode(code, out year, out halfYear, out quarter, out month, out day);
                object[] args = { year, halfYear, quarter, month, day };
                string key;
                if (level == 0)
                {
                    key = String.Format("{0} год", args);
                    uniqueName += ".[Данные года].[Данные года].[Данные года].[Данные года]";
                }
                else if (level == 1)
                {
                    key = String.Format("{1} полугодие {0} года", args);
                    uniqueName += ".[Данные года].[Данные года].[Данные полугодия]";
                }
                else if (level == 2)
                {
                    key = String.Format("{2} квартал {0} года", args);
                    uniqueName += ".[Данные года].[Данные квартала]";
                }
                else if (level == 3)
                {
                    args[3] = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(Convert.ToInt32(args[3])));
                    key = String.Format("{3} {0} года", args);
                    uniqueName += ".[Данные месяца]";
                }
                else
                {
                    args[3] = CRHelper.RusMonthGenitive(Convert.ToInt32(args[3])).ToLower();
                    key = String.Format("{4} {3} {0} года", args);
                }
                dictDates.Add(key, uniqueName);
                dict.Add(key, level);
            }
            ComboDate.FillDictionaryValues(dict);
            ComboDate.SelectLastNode();
        }

        private void ParseCode(string code, out string year, out string halfYear, out string quarter, out string month, out string day)
        {
            year = halfYear = quarter = month = day = String.Empty;
            if (code.Length < 10)
                return;
            year = code.Substring(0, 4);
            halfYear = code.Substring(4, 1);
            quarter = code.Substring(5, 1);
            month = code.Substring(6, 2);
            day = code.Substring(8, 2);
        }

        private int GetLevel(string code)
        {
            if (code.EndsWith("000000"))
                return 0;
            if (code.EndsWith("00000"))
                return 1;
            if (code.EndsWith("0000"))
                return 2;
            if (code.EndsWith("00"))
                return 3;
            else
                return 4;
        }

        private string GetConvertedCode(string code, string level)
        {
            if (String.IsNullOrEmpty(code) || String.IsNullOrEmpty(level))
                return String.Empty;

            string result = String.Empty;
            if (level == "Год")
            {
                string year = code.Substring(0, 4);
                result = String.Format("{0}000000", year);
            }
            else if (level == "Полугодие")
            {
                string year = code.Substring(0, 4);
                string halfYear = code.Substring(6, 1);
                result = String.Format("{0}{1}00000", year, halfYear);
            }
            else if (level == "Квартал")
            {
                string year = code.Substring(0, 4);
                string quarter = code.Substring(7, 1);
                string halfYear = CRHelper.HalfYearNumByQuarterNum(Convert.ToInt32(quarter)).ToString();
                result = String.Format("{0}{1}{2}0000", year, halfYear, quarter);
            }
            else if (level == "Месяц")
            {
                string year = code.Substring(0, 4);
                string month = code.Substring(4, 2);
                string quarter = CRHelper.QuarterNumByMonthNum(Convert.ToInt32(month)).ToString();
                string halfYear = CRHelper.HalfYearNumByQuarterNum(Convert.ToInt32(quarter)).ToString();
                object[] args = { year, halfYear, quarter, month};
                result = String.Format("{0}{1}{2}{3}00", args);
            }
            else if (level == "День")
            {
                string year = code.Substring(0, 4);
                string month = code.Substring(4, 2);
                string quarter = CRHelper.QuarterNumByMonthNum(Convert.ToInt32(month)).ToString();
                string halfYear = CRHelper.HalfYearNumByQuarterNum(Convert.ToInt32(quarter)).ToString();
                string day = code.Substring(6, 2);
                object[] args = { year, halfYear, quarter, month, day };
                result = String.Format("{0}{1}{2}{3}{4}", args);
            }

            return result;
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
            string query = DataProvider.GetQueryText("NP_0001_0001_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

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
            band.Columns[0].Width = Unit.Parse("300px");
            band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            band.Columns[1].CellStyle.Wrap = true;
            band.Columns[1].Width = Unit.Parse("100px");
            band.Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            for (int i = 3; i < band.Columns.Count; ++i)
            {
                band.Columns[i].Width = Unit.Parse("100px");
                CRHelper.FormatNumberColumn(band.Columns[i], "N3");
            }

            GridHeaderLayout headerLayout = UltraWebGrid.GridHeaderLayout;
            headerLayout.AddCell("Показатели");
            headerLayout.AddCell("Ед. изм.");
            headerLayout.AddCell("Уровень");
            headerLayout.AddCell("План на год");
            headerLayout.AddCell("Значение на отч. дату");
            GridHeaderCell headerCell = headerLayout.AddCell("Финансирование из ФБ, тыс. руб.");
            headerCell.AddCell("План");
            headerCell.AddCell("Факт");
            headerCell = headerLayout.AddCell("Финансирование из КБ субъекта, тыс. руб.");
            headerCell.AddCell("План");
            headerCell.AddCell("Факт");
            if (selectedProject.Value == "Жилье ПС")
            {
                headerCell = headerLayout.AddCell("Финансирование из средств Фонда ЖКХ, за счет средств материнского (семейного) капитала, тыс. руб");
                headerCell.AddCell("План");
                headerCell.AddCell("Кассовое исполнение");
            }

            headerLayout.ApplyHeaderInfo();
            
            band.Columns[2].Hidden = true;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int level;
            if (Int32.TryParse(e.Row.Cells[2].GetText(), out level))
            {
                e.Row.Cells[0].Style.Padding.Left = Unit.Parse(String.Format("{0}px", (level - 1) * 20));
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
            ReportExcelExporter1.SheetColumnCount = UltraWebGrid.Grid.Columns.Count - 1;
            ReportExcelExporter1.GridColumnWidthScale = 1;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            UltraWebGrid.GridHeaderLayout.childCells.Remove(UltraWebGrid.GridHeaderLayout.childCells[2]);
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
            UltraWebGrid.GridHeaderLayout.childCells.Remove(UltraWebGrid.GridHeaderLayout.childCells[2]);
            ReportPDFExporter1.Export(UltraWebGrid.GridHeaderLayout, String.Empty, section1);
        }

        #endregion
    }
}