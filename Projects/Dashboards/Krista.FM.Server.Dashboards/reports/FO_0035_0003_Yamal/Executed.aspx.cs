using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0035_0003_Yamal
{
    public partial class Executed : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtExecute = new DataTable();
        private DataTable dtExecuteOutcome = new DataTable();
        private int footerRowsCount = 0;
        private int headerRowsCount = 0;
        private double othersEthalon;
        private double salaryEthalon;
        private double commonEthalon;

        public int Devider
        {
            get
            {
                return 12;
            }
        }

        private DateTime currentDate;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            PopupInformer1.HelpPageUrl = "DailyYearExecutedHelp.html";

            grid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 230);
            grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            GridSearch1.LinkedGridId = grid.ClientID;
            grid.DisplayLayout.RowHeightDefault = 23;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.MultiHeader = true;

            Link3.Visible = true;
            Link3.Text = "Структура&nbsp;расходов&nbsp;&nbsp;бюджета&nbsp;авт.округа&nbsp;по&nbsp;ведомствам";
            Link3.NavigateUrl = "~/reports/FO_0035_0003_Yamal/Kinds.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            CustomCalendar1.WebPanel.Expanded = false;
            if (!Page.IsPostBack)
            {
 
                    // Получаем последнюю дату
                    string query = DataProvider.GetQueryText("date");
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(
                        query, dtDate);

                    DateTime date = new DateTime(
                        Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                        CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                        Convert.ToInt32(dtDate.Rows[0][4].ToString()));
                    
                    // Инициализируем календарь
                    CustomCalendar1.WebCalendar.SelectedDate = date;
            }


                // Инициализируем параметры из календаря
                UserParams.PeriodCurrentDate.Value =
                    CRHelper.PeriodMemberUName(
                        String.Empty, CustomCalendar1.WebCalendar.SelectedDate, 5);

                currentDate = CustomCalendar1.WebCalendar.SelectedDate;

                UserParams.CustomParam("period_quater").Value = CRHelper.PeriodMemberUName(String.Empty, CustomCalendar1.WebCalendar.SelectedDate, 3);

                lbTitle.Text = "Исполнение кассового плана";
                lbSubTitle.Text = "(на " + CustomCalendar1.WebCalendar.SelectedDate.ToString("dd.MM.yyyy") + "г.) по бюджету автономного округа";

            othersEthalon = OthersAssessionLimit();
            commonEthalon = CommonAssessionLimit();
            salaryEthalon = SalaryAssessionLimit();
            // биндим данные в гриды.
            grid.DataBind();

            AdditionalSetupGrid();
        }

        public string GetDateUniqName(string source, int level)
        {
            string[] sts = source.Split('|');
            if (sts.Length > 0)
            {
                switch (level)
                {
                    // нулевой уровень выбрать нельзя
                    case 1:
                        {
                            string month = sts[1].TrimEnd(' ');
                            return string.Format(".[{0}].[Полугодие {1}].[Квартал {2}].[{3}]",
                                                 sts[0], CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(month)),
                                                 CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month)), month);
                        }
                    case 2:
                        {
                            string month = sts[1].TrimEnd(' ');
                            string day = sts[2].TrimEnd(' ');
                            return string.Format(".[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]",
                                                 sts[0], CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(month)),
                                                 CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month)), month,
                                                 day);
                        }
                }
            }
            return string.Empty;
        }

        public string GetDateString(string source, int level)
        {
            string[] sts = source.Split('|');
            if (sts.Length > 0)
            {
                switch (level)
                {
                    // нулевой уровень выбрать нельзя
                    case 1:
                        {
                            string month = sts[1].TrimEnd(' ');
                            return string.Format("{1} {0} года", sts[0], month.ToLower());
                        }
                    case 2:
                        {
                            string month = sts[1].TrimEnd(' ');
                            string day = sts[2].TrimEnd(' ');
                            if (day == "Заключительные обороты")
                            {
                                return string.Format("{1} {0} года", sts[0], month.ToLower());
                            }
                            else
                            {
                                return string.Format("{2} {1} {0} года", sts[0], CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)), day);
                            }
                        }
                }
            }
            return string.Empty;
        }

        public DateTime GetCalendarDate(string source, int level)
        {
            string[] sts = source.Split('|');
            if (sts.Length > 0)
            {
                switch (level)
                {
                    // нулевой уровень выбрать нельзя
                    case 1:
                        {
                            string month = sts[1].TrimEnd(' ');
                            return new DateTime(Convert.ToInt32(sts[0]), CRHelper.MonthNum(month), CRHelper.MonthLastDay(CRHelper.MonthNum(month)));
                        }
                    case 2:
                        {
                            string month = sts[1].TrimEnd(' ');
                            string day = sts[2].TrimEnd(' ');
                            if (day == "Заключительные обороты")
                            {
                                return new DateTime(Convert.ToInt32(sts[0]), CRHelper.MonthNum(month), CRHelper.MonthLastDay(CRHelper.MonthNum(month)));
                            }
                            else
                            {
                                return new DateTime(Convert.ToInt32(sts[0]), CRHelper.MonthNum(month), Convert.ToInt32(day));
                            }
                        }
                }
            }
            return new DateTime();
        }

        private void AdditionalSetupGrid()
        {
            
            for (int i = 0; i < grid.Rows[0].Cells.Count; i++)
            {
                grid.Rows[0].Style.Font.Bold = true;
            }

            grid.Rows[headerRowsCount - 1].Style.BorderDetails.ColorTop = Color.FromArgb(192, 192, 192);
            grid.Rows[headerRowsCount - 1].Style.Font.Bold = true;
            grid.Rows[grid.Rows.Count - footerRowsCount - 1].Style.BorderDetails.ColorTop = Color.FromArgb(192, 192, 192);
            grid.Rows[grid.Rows.Count - 1].Style.BorderDetails.ColorTop = Color.FromArgb(192, 192, 192);

            for (int rowNum = headerRowsCount; rowNum < grid.Rows.Count - footerRowsCount; rowNum++)
            {
                if (rowNum > 0 && IsChildRow(rowNum))
                {
                    for (int i = 0; i < grid.Rows[rowNum].Cells.Count; i++)
                    {
                        grid.Rows[rowNum].Style.Font.Italic = true;
                    }
                }
                for (int colNum = 7; colNum < grid.Columns.Count; colNum += 3)
                {
                    UltraGridCell cell = grid.Rows[rowNum].Cells[colNum];
                    if (cell == null || cell.Value == null) continue;
                    try
                    {
                        double ethalon = 0;
                        switch (colNum)
                        {
                            case 7:
                                {
                                    ethalon = salaryEthalon;
                                    break;
                                }
                            case 10:
                            case 13:
                            case 16:
                                {
                                    ethalon = commonEthalon;
                                    break;
                                }
                            case 19:
                                {
                                    ethalon = othersEthalon;
                                    break;
                                }
                        }

                        double curVal;
                        curVal = double.Parse(cell.Value.ToString());
                        if (curVal >= ethalon)
                        {
                            cell.Style.BackgroundImage = "~/images/ballGreenBB.png";
                            cell.Title = String.Format("Соблюдается условие равномерности ({0:P2})", ethalon);
                        }
                        else
                        {

                            cell.Style.BackgroundImage = "~/images/ballRedBB.png";
                            cell.Title = String.Format("Не соблюдается условие равномерности ({0:P2})", ethalon);
                        }
                       cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                    catch
                    {
                        // не удалось покрасить, ну да пойдем дальше
                    }

                }

            }
        }

        private bool IsChildRow(int rowNum)
        {
            return grid.Rows[rowNum - 1].Cells[1].Value != null &&
                   grid.Rows[rowNum].Cells[1].Value != null &&
                   grid.Rows[rowNum - 1].Cells[1].Value.ToString() == grid.Rows[rowNum].Cells[1].Value.ToString();
        }

        protected void gridMain_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("Execute");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtExecute);
            query = DataProvider.GetQueryText("ExecuteOutcome");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "Наименование показателя", dtExecuteOutcome);

            DataTable dtSource = dtExecuteOutcome.Clone();
            // добавим колонку для кода.
            dtSource.Columns.Add(new DataColumn());

            // сначала набиваем первую часть первой таблицы
            int rowsCounter = 0;
            while (!dtExecute.Rows[rowsCounter][0].ToString().Equals("Расходы - всего"))
            {
                AddNewSourceRow(dtSource, rowsCounter);
                rowsCounter++;
            }

            AddNewSourceRow(dtSource, rowsCounter);
            rowsCounter++;

            // Запомним, сколько строк в первой части
            headerRowsCount = rowsCounter;
            // потом вторую таблицу
            for (int i = 0; i < dtExecuteOutcome.Rows.Count; i++)
            {
                DataRow row = dtSource.NewRow();
                row[0] = dtExecuteOutcome.Rows[i][0];
                row[1] = dtExecuteOutcome.Rows[i][1];
                for (int j = 2; j < dtExecuteOutcome.Columns.Count; j++)
                {
                    double value;
                    if (Double.TryParse(dtExecuteOutcome.Rows[i][j].ToString(), out value))
                    {

                        row[j] = (j - 1) % 3 != 0 ? value / 1000 : value;
                    }
                }
                dtSource.Rows.Add(row);
            }

            // и запомним, сколько в ней оставалось строк.
            footerRowsCount = dtExecute.Rows.Count - rowsCounter;

            AddNewSourceRow(dtSource, rowsCounter);
            rowsCounter++;
            DataRow Additionalrow = dtSource.NewRow();
            Additionalrow[0] = "из них:";
            dtSource.Rows.Add(Additionalrow);

            for (; rowsCounter < dtExecute.Rows.Count; rowsCounter++)
            {
                AddNewSourceRow(dtSource, rowsCounter);
            }
            grid.DataSource = dtSource;
        }

        private void AddNewSourceRow(DataTable dtSource, int rowsCounter)
        {
            if (rowsCounter > dtExecute.Rows.Count - 1)
            {
                return;
            }

            DataRow row = dtSource.NewRow();
            row[0] = dtExecute.Rows[rowsCounter][0];
            double value;
            if (Double.TryParse(dtExecute.Rows[rowsCounter][2].ToString(), out value))
            {
                row[dtSource.Columns.Count - 4] = Convert.ToDouble(value / 1000).ToString("N0");
            }
            if (Double.TryParse(dtExecute.Rows[rowsCounter][3].ToString(), out value))
            {
                row[dtSource.Columns.Count - 3] = Convert.ToDouble(value / 1000).ToString("N0");
            }
            if (Double.TryParse(dtExecute.Rows[rowsCounter][4].ToString(), out value))
            {
                if (dtSource.Columns[dtSource.Columns.Count - 2].DataType == typeof(decimal))
                {
                    row[dtSource.Columns.Count - 2] = value;
                }
                else if (dtSource.Columns[dtSource.Columns.Count - 2].DataType == typeof(string))
                {
                    row[dtSource.Columns.Count - 2] = Convert.ToDouble(value).ToString("P2");
                }
            }
            row[dtSource.Columns.Count - 1] = dtExecute.Rows[rowsCounter][1];
            dtSource.Rows.Add(row);
        }

        #region обработчики грида
        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (Page.IsPostBack) return;
            UltraGridColumn col = e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 4];
            e.Layout.Bands[0].Columns.RemoveAt(e.Layout.Bands[0].Columns.Count - 4);
            e.Layout.Bands[0].Columns.Insert(2, col);

            col = e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 3];
            e.Layout.Bands[0].Columns.RemoveAt(e.Layout.Bands[0].Columns.Count - 3);
            e.Layout.Bands[0].Columns.Insert(3, col);

            col = e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2];
            e.Layout.Bands[0].Columns.RemoveAt(e.Layout.Bands[0].Columns.Count - 2);
            e.Layout.Bands[0].Columns.Insert(4, col);

            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(270, 1900);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(45, 1900);

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(80, 1900);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
            }

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            if (Page.IsPostBack)
            {
                for (int i = 2; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 3)
                {

                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "N0");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "P2");
                }
                return;
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
            }

            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.SpanY = 2;
            e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.OriginY = 0;
            e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.SpanY = 2;
            e.Layout.Bands[0].Columns[1].Header.Caption = "КВСР";

            int multiHeaderPos = 2;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 3)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.BorderDetails.ColorLeft =
                        Color.FromArgb(192, 192, 192);

                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "План", "");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                e.Layout.Bands[0].Columns[i].Header.Title = "Годовые назначения";

                captions = e.Layout.Bands[0].Columns[i + 1].Header.Caption.Split(';');
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Факт", "");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "N0");
                e.Layout.Bands[0].Columns[i + 1].Header.Title = "Исполнено с начала года";

                captions = e.Layout.Bands[0].Columns[i + 2].Header.Caption.Split(';');
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, captions[1], captions[1]);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "P2");
                string assessionText = string.Empty;
                switch (i)
                {
                    // Итого
                    case 2:
                        {
                            e.Layout.Bands[0].Columns[i + 2].Header.Title = "Процент исполнения назначений";
                            break;
                        }
                    // Заработная плата
                    case 5:
                        {
                            assessionText = string.Format("выплаты до 6 и 21 числа каждого месяца не менее 1/{0} {1} плана", 
                                2 * Devider, "годового");
                            break;
                        }
                    // Публичные обязательства
                    // Коммунальные расходы
                    case 8:
                    case 11:
                    case 14:
                        {
                            assessionText = string.Format("на конец месяца должно быть исполнено 1/{0} {1} плана",
                                Devider, "годового");
                            break;
                        }
                    // Прочие
                    case 17:
                        {
                            assessionText = string.Format("равномерное расходование по числу дней {0}", "в году");
                            break;
                        }
                }
                if (i != 2)
                {
                    e.Layout.Bands[0].Columns[i + 2].Header.Title =
                        String.Format("Процент исполнения назначений и оценка равномерности расходования \r({0})",
                                      assessionText);
                }
                CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, captions[0] + ", тыс.руб.", multiHeaderPos, 0, 3, 1);

                multiHeaderPos += 3;
            }
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value != null)
            {
                e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().Replace("областного", "окружного");
            }


            // Убираем нули из КВСР
            double value = 0;
            if (e.Row.Cells[1].Value != null)
            {
                Double.TryParse(e.Row.Cells[1].Value.ToString(), out value);
            }
//            if (value == 0)
//            {
//                e.Row.Cells[1].Value = null;
//            }

            if (e.Row.Cells[grid.Columns.Count - 1].Value != null)
            {
                string cellValue = e.Row.Cells[grid.Columns.Count - 1].Value.ToString();
                if (cellValue[cellValue.Length - 1] == '0')
                {
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].Style.Font.Bold = true;
                    }
                }
            }
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    if (Double.TryParse(cell.Value.ToString(), out value))
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

        /// <summary>
        /// Пороговое значение оценки зарплаты
        /// </summary>
        /// <returns></returns>
        private double SalaryAssessionLimit()
        {
            // Берем номер месяца в квартале или году
            double monthNum = currentDate.Month;
            int day = currentDate.Day;
            if (day < 6)
            {
                // Выплат в этом месяце не было
                return (monthNum - 1) / Devider;
            }
            if (day < 21)
            {
                // Была одна выплата
                return (monthNum - 1) / Devider + 1.0 / (2 * Devider);
            }
            // Все выплаты
            return (monthNum) / Devider;
        }

        /// <summary>
        /// Пороговое значение оценки остальных
        /// </summary>
        /// <returns></returns>
        private double CommonAssessionLimit()
        {
            // Берем номер месяца в квартале или году
            double monthNum = currentDate.Month;
            // Если последний день месяца
            if (CRHelper.MonthLastDay(currentDate.Month) == currentDate.Day)
            {
                return (monthNum) / Devider;
            }
            else
            {
                return (monthNum - 1) / Devider;
            }
        }

        /// <summary>
        /// Пороговое значение других оценок
        /// </summary>
        /// <returns></returns>
        private double OthersAssessionLimit()
        {
            return Convert.ToDouble(currentDate.DayOfYear) / YearDaysCount(currentDate);
        }

        public bool IsLeapYear(DateTime date)
        {
            return date.Year % 4 == 0;
        }

        public double YearDaysCount(DateTime date)
        {
            return IsLeapYear(date) ? 366 : 365;
        }

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = lbTitle.Text + " " + lbSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.CurrentWorksheet.Columns[0].Width = 350 * 37;
            for (int i = 1; i < grid.Bands[0].Columns.Count; i++)
            {
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = i > 1 && (i - 1) % 3 == 0 ? "#,##0%" : "#,##0";
                e.CurrentWorksheet.Columns[i].Width = 80 * 37;
            }
        }

        private int offset = 0;

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = grid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + offset];
            e.HeaderText = col.Header.Key.Split(';')[0];
            if (col.Hidden)
            {
                offset++;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(grid);
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(grid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(lbTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(lbSubTitle.Text);
        }
    }
}
