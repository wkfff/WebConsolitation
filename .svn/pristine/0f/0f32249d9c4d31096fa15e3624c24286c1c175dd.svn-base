using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0035_0003_2_HMAO
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtExecute = new DataTable();
        private DataTable dtExecuteOutcome = new DataTable();
        private int footerRowsCount = 0;
        private int headerRowsCount = 0;
        private double othersEthalon;
        private double salaryEthalon;
        private double commonEthalon;

        private DateTime currentDate;

        private GridHeaderLayout headerLayout;

        public int Devider
        {
            get
            {
                return 12;
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            PopupInformer1.HelpPageUrl = "Default.html";
            
            grid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 240);
            grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            GridSearch1.LinkedGridId = grid.ClientID;
            grid.DisplayLayout.RowHeightDefault = 23;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            Link1.Visible = true;
            Link1.Text = "Исполнение&nbsp;кассового&nbsp;плана";
            Link1.NavigateUrl = "~/reports/FO_0035_0001_HMAO/Default.aspx";

            Link2.Visible = true;
            Link2.Text = "Структура&nbsp;расходов&nbsp;бюджета&nbsp;автономного&nbsp;округа&nbsp;по&nbsp;ведомствам";
            Link2.NavigateUrl = "~/reports/FO_0035_0003_HMAO/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                ComboCalendar.Visible = true;
                ComboCalendar.Width = 300;
                ComboCalendar.MultiSelect = false;
                ComboCalendar.ShowSelectedValue = false;
                ComboCalendar.ParentSelect = false;
                ComboCalendar.FillDictionaryValues(
                    CustomMultiComboDataHelper.FillCashPlanNonEmptyDays(DataDictionariesHelper.CashPlanNonEmptyDays));
                ComboCalendar.SelectLastNode();
                ComboCalendar.PanelHeaderTitle = "Выберите дату";

            }

            UserParams.PeriodCurrentDate.Value = GetDateUniqName(ComboCalendar.GetSelectedNodePath(), ComboCalendar.SelectedNode.Level);
            currentDate = GetCalendarDate(ComboCalendar.GetSelectedNodePath(), ComboCalendar.SelectedNode.Level);

            lbTitle.Text = "Исполнение кассового плана";
            lbSubTitle.Text = "на " + GetDateString(ComboCalendar.GetSelectedNodePath(), ComboCalendar.SelectedNode.Level) + " по бюджету автономного округа";
            IndicationCommentLabel.Text =
                String.Format("<img style=\"vertical-align:middle\" src=\"../../images/ballRedBB.png\">/<img style=\"vertical-align:middle\" src=\"../../images/ballGreenBB.png\"> – сравнение процента исполнения годового кассового плана по выбранному ГРБС со средним процентом исполнения по всем ГРБС");

            // биндим данные в гриды
            headerLayout = new GridHeaderLayout(grid);
            grid.Bands.Clear();
            grid.DataBind();
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
            return String.Empty;
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

            // сначала набиваем первую половину первой таблицы
            for (int i = 0; i < dtExecute.Rows.Count; i++)
            {
                AddNewSourceRow(dtSource, i);
            }
            
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
                        row[j] = value;
                    }
                }
                dtSource.Rows.Add(row);
            }
           
            grid.DataSource = dtSource;
        }

        private void AddNewSourceRow(DataTable dtSource, int rowsCounter)
        {
            DataRow row = dtSource.NewRow();
            row[0] = dtExecute.Rows[rowsCounter][0];
            double value;
            if (Double.TryParse(dtExecute.Rows[rowsCounter][2].ToString(), out value))
            {
                row[dtSource.Columns.Count - 5] = Convert.ToDouble(value / 1000).ToString("N0");
            }
            if (Double.TryParse(dtExecute.Rows[rowsCounter][3].ToString(), out value))
            {
                row[dtSource.Columns.Count - 4] = Convert.ToDouble(value / 1000).ToString("N0"); ;
            }
            if (Double.TryParse(dtExecute.Rows[rowsCounter][4].ToString(), out value))
            {
                if (dtSource.Columns[dtSource.Columns.Count - 2].DataType == typeof(decimal))
                {
                    row[dtSource.Columns.Count - 3] = value;
                }
                else if (dtSource.Columns[dtSource.Columns.Count - 2].DataType == typeof(string))
                {
                    row[dtSource.Columns.Count - 3] = Convert.ToDouble(value).ToString("P2");
                }
            }

            row[dtSource.Columns.Count - 1] = dtExecute.Rows[rowsCounter][1];
            dtSource.Rows.Add(row);
        }

        #region Обработчики грида

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(270);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(45);
            e.Layout.Bands[0].Columns[1].CellStyle.Padding.Right = 10;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "000");
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;

            headerLayout.AddCell("Наименование показателя");
            headerLayout.AddCell("КВСР");

            int columnWidth = 80;
            int columnCount = e.Layout.Bands[0].Columns.Count;

            for (int i = 2; i < columnCount - 1; i = i + 4)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "N0");
                e.Layout.Bands[0].Columns[i + 1].Width = CRHelper.GetColumnWidth(columnWidth);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "P2");
                e.Layout.Bands[0].Columns[i + 2].Width = CRHelper.GetColumnWidth(columnWidth);
                e.Layout.Bands[0].Columns[i + 3].Hidden = true;

                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell groupCell = headerLayout.AddCell(String.Format("{0}, тыс.руб.", captions[0].TrimEnd(' ')));
                groupCell.AddCell("План", "Годовые назначения");
                groupCell.AddCell("Факт", "Исполнено с начала года");
                groupCell.AddCell("Процент исполнения", "Процент исполнения годовых назначений");
            }

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            headerLayout.ApplyHeaderInfo();
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            string rowName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                rowName = e.Row.Cells[0].Value.ToString();
                e.Row.Cells[0].Value = CRHelper.ToUpperFirstSymbol(rowName);
            }

            bool boldRow = rowName.ToLower().Contains("расходы- всего");

            // Убираем нули из КВСР
            double value = 0;
            if (e.Row.Cells[1].Value != null)
            {
                Double.TryParse(e.Row.Cells[1].Value.ToString(), out value);
            }
            if (value == 0)
            {
                e.Row.Cells[1].Value = null;
            }

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

            for (int i = 0; i < e.Row.Cells.Count - 1; i++)
            {
                string columnName = e.Row.Band.Grid.Columns[i].Header.Caption;
                bool percentColumn = columnName.ToLower().Contains("процент исполнения");

                UltraGridCell cell = e.Row.Cells[i];

                double avgPercent = Double.MinValue;
                if (percentColumn && e.Row.Cells[i + 1].Value != null && e.Row.Cells[i + 1].Value.ToString() != String.Empty)
                {
                    avgPercent = 100 * Convert.ToDouble(e.Row.Cells[i + 1].Value);
                }

                if (percentColumn && avgPercent != Double.MinValue && cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    double currValue = 100 * Convert.ToDouble(cell.Value);

                    if (currValue < avgPercent)
                    {
                        cell.Style.BackgroundImage = "~/images/ballRedBB.png";
                        cell.Title = string.Format("Ниже среднего ({0:N2}%)", avgPercent);
                    }
                    else
                    {
                        cell.Style.BackgroundImage = "~/images/ballGreenBB.png";
                        if (currValue > avgPercent)
                        {
                            cell.Title = string.Format("Выше среднего ({0:N2}%)", avgPercent);
                        }
                        else
                        {
                            cell.Title = string.Format("Равно среднему ({0:N2}%)", avgPercent);
                        }
                    }
                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }


                if (boldRow)
                {
                    e.Row.Cells[i].Style.BorderDetails.ColorTop = Color.FromArgb(192, 192, 192);
                    e.Row.Cells[i].Style.Font.Bold = true;
                }

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

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = lbTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = lbSubTitle.Text;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = lbTitle.Text;
            ReportPDFExporter1.PageSubTitle = lbSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            ReportPDFExporter1.Export(headerLayout);
        }

        #endregion
    }
}
