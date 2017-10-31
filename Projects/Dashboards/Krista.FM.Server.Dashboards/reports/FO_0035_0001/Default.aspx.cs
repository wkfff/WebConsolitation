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

namespace Krista.FM.Server.Dashboards.reports.FO_0035_0001
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtExecute = new DataTable();
        private DataTable dtExecuteOutcome = new DataTable();
        private int footerRowsCount = 0;

        #region Параметры запроса

        // мера План
        private CustomParam measurePlan;
        // мера Факт
        private CustomParam measureFact;

        #endregion

        public bool IsKursk
        {
            get
            {
                return RegionSettings.Instance.Id.ToLower() == "kursk";
            }
        }

        public bool IsOmsk
        {
            get
            {
                return RegionSettings.Instance.Id.ToLower() == "omsk";
            }
        }

        public bool IsQuaterPlanType
        {
            get
            {
                return RegionSettingsHelper.Instance.CashPlanType.ToLower() == "quarter";
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            measurePlan = UserParams.CustomParam("measure_plan");
            measureFact = UserParams.CustomParam("measure_fact");

            #endregion

            PopupInformer1.HelpPageUrl = (IsKursk) ? "WeeklyHelp.html" : "DailyHelp.html";

            grid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 250);
            grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            grid.DisplayLayout.NoDataMessage = "Нет данных";

            GridSearch1.LinkedGridId = grid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.MultiHeader = true;

            Link1.Visible = IsQuaterPlanType;
            Link1.Text = "Исполнение&nbsp;кассового&nbsp;плана&nbsp;с&nbsp;динамикой";
            Link1.NavigateUrl = "~/reports/FO_0035_0002/Default.aspx";

            Link2.Visible = true;
            Link2.Text = "Структура&nbsp;расходов&nbsp;областного&nbsp;бюджета&nbsp;по&nbsp;ведомствам";
            Link2.NavigateUrl = "~/reports/FO_0035_0003/Kinds.aspx";

            Link3.Visible = true;
            Link3.Text = "Исполнение&nbsp;кассового&nbsp;плана&nbsp;(с&nbsp;процентом&nbsp;исполнения)";
            Link3.NavigateUrl = "~/reports/FO_0035_0003/Executed.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            CustomCalendar1.WebPanel.Expanded = false;
            if (!Page.IsPostBack)
            {
                if (IsKursk)
                {
                    // для Курска заменяем календарь на комбобокс
                    CustomCalendar1.Visible = false;
                    ComboCalendar.Visible = true;
                    ComboCalendar.Width = 300;
                    ComboCalendar.MultiSelect = false;
                    ComboCalendar.ShowSelectedValue = false;
                    ComboCalendar.ParentSelect = false;
                    ComboCalendar.FillDictionaryValues(CustomMultiComboDataHelper.FillCashPlanNonEmptyDays(DataDictionariesHelper.CashPlanNonEmptyDays));
                    ComboCalendar.SelectLastNode();
                    ComboCalendar.PanelHeaderTitle = "Выберите дату";
                }
                else
                {
                    ComboCalendar.Visible = false;
                    CustomCalendar1.Visible = true;
                   
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
            }

            if (IsKursk)
            {
                UserParams.PeriodCurrentDate.Value = GetDateUniqName(ComboCalendar.GetSelectedNodePath(), ComboCalendar.SelectedNode.Level);
            }
            else
            {
                UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName(String.Empty, CustomCalendar1.WebCalendar.SelectedDate, 5);
            }

            if (IsQuaterPlanType)
            {
                measurePlan.Value = "План";
                measureFact.Value = "Факт";

                lbTitle.Text = String.Format("Исполнение кассового плана за {0}", CRHelper.PeriodDescr(CustomCalendar1.WebCalendar.SelectedDate, 3));
                lbSubTitle.Text = "(на " + CustomCalendar1.WebCalendar.SelectedDate.ToString("dd.MM.yyyy") + "г.) по областному бюджету";
            }
            else
            {
                measurePlan.Value = "План_Нарастающий итог";
                measureFact.Value = "Факт_Нарастающий итог";

                lbTitle.Text = "Исполнение кассового плана";
                if (!IsKursk)
                {
                    lbSubTitle.Text = "(на " + CustomCalendar1.WebCalendar.SelectedDate.ToString("dd.MM.yyyy") + "г.) по областному бюджету";
                }
                else
                {
                    lbSubTitle.Text = "на " + GetDateString(ComboCalendar.GetSelectedNodePath(), ComboCalendar.SelectedNode.Level) + " по областному бюджету";
                }
            }
            
            // биндим данные в гриды.
            grid.Bands.Clear();
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

        private void AdditionalSetupGrid()
        {
            int headerRowsCount = dtExecute.Rows.Count - footerRowsCount - 1;

            if (footerRowsCount > 1)
            {
                grid.Rows[headerRowsCount].Style.BorderDetails.ColorTop = Color.FromArgb(192, 192, 192);
                grid.Rows[headerRowsCount].Style.Font.Bold = true;
                grid.Rows[grid.Rows.Count - footerRowsCount - 1].Style.BorderDetails.ColorTop = Color.FromArgb(192, 192,
                                                                                                               192);
                grid.Rows[grid.Rows.Count - 1].Style.BorderDetails.ColorTop = Color.FromArgb(192, 192, 192);

                grid.Rows[grid.Rows.Count - footerRowsCount - 1].Cells[0].ColSpan =
                    grid.Rows[grid.Rows.Count - footerRowsCount].Cells.Count - 1;

                if (IsOmsk)
                {
                    grid.Rows[grid.Rows.Count - footerRowsCount - 1].Cells[0].ColSpan =
                        grid.Rows[grid.Rows.Count - footerRowsCount].Cells.Count - 3;
                }
                else
                {
                    grid.Rows[grid.Rows.Count - footerRowsCount - 1].Cells[0].ColSpan =
                        grid.Rows[grid.Rows.Count - footerRowsCount].Cells.Count - 1;
                }

                for (int i = grid.Rows.Count - footerRowsCount; i < grid.Rows.Count; i++)
                {
                    grid.Rows[i].Cells[0].ColSpan = grid.Columns.Count - 3;
                }


                for (int rowNum = 0; rowNum < grid.Rows.Count - 1; rowNum++)
                {
                    if (rowNum > 0 && IsChildRow(rowNum))
                    {
                        for (int i = 0; i < grid.Rows[rowNum].Cells.Count; i++)
                        {
                            grid.Rows[rowNum].Style.Font.Italic = true;
                        }
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

            if (dtExecute.Rows.Count == 0 && dtExecuteOutcome.Rows.Count == 0)
            {
                grid.DataSource = null;
                return;
            }

            DataTable dtSource = dtExecuteOutcome.Clone();
            // добавим колонку для кода.
            dtSource.Columns.Add(new DataColumn());

            // сначала набиваем первую половину первой таблицы
            int rowsCounter = 0;
            while (rowsCounter >= 0 && rowsCounter < dtExecute.Rows.Count && dtExecute.Rows[rowsCounter][0] != DBNull.Value  && !(dtExecute.Rows[rowsCounter][0].ToString().Equals("Расходы - всего")))
            {
                AddNewSourceRow(dtSource, rowsCounter);
                rowsCounter++;
            }

            if (RegionSettings.Instance.Id == "Penza" || RegionSettings.Instance.Id == "Kursk")
            {
                AddNewSourceRow(dtSource, rowsCounter);
                rowsCounter++;
            }
            // потом вторую
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
                        row[j] = value/1000;
                    }
                }
                dtSource.Rows.Add(row);
            }
            // снова первую, пропустим строчку про расходы
            if (RegionSettings.Instance.Id != "Penza" && RegionSettings.Instance.Id != "Kursk")
            {
                rowsCounter++;
            }
            // и запомним, сколько в ней оставалось строк.
            footerRowsCount = dtExecute.Rows.Count - rowsCounter;

            AddNewSourceRow(dtSource, rowsCounter);
            rowsCounter++;
            DataRow Additionalrow = dtSource.NewRow();
            Additionalrow[0] = "из них:";
            dtSource.Rows.Add(Additionalrow);
            
            for (; rowsCounter < dtExecute.Rows.Count; rowsCounter++ )
            {
                AddNewSourceRow(dtSource, rowsCounter);
            }
            grid.DataSource = dtSource;
        }

        private void AddNewSourceRow(DataTable dtSource, int rowsCounter)
        {
            if (rowsCounter >= 0 && rowsCounter < dtExecute.Rows.Count)
            {
                DataRow row = dtSource.NewRow();
                row[0] = dtExecute.Rows[rowsCounter][0];
                double value;
                if (Double.TryParse(dtExecute.Rows[rowsCounter][2].ToString(), out value))
                {
                    row[dtSource.Columns.Count - 3] = Convert.ToDouble(value/1000).ToString("N0");
                }
                if (Double.TryParse(dtExecute.Rows[rowsCounter][3].ToString(), out value))
                {
                    row[dtSource.Columns.Count - 2] = Convert.ToDouble(value/1000).ToString("N0");
                }
                row[dtSource.Columns.Count - 1] = dtExecute.Rows[rowsCounter][1];
                dtSource.Rows.Add(row);
            }
        }

        #region обработчики грида
        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(245);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(45);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(69);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
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

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 2)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.BorderDetails.ColorLeft = 
                        Color.FromArgb(192, 192, 192);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "План", (IsQuaterPlanType) ? "Квартальные назначения" : "Годовые назначения");

                captions = e.Layout.Bands[0].Columns[i + 1].Header.Caption.Split(';');
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Факт", (IsQuaterPlanType) ? "Исполнено с начала квартала" : "Исполнено с начала года");

                CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, captions[0] + ", тыс.руб.", multiHeaderPos, 0, 2, 1);

                multiHeaderPos += 2;
            }
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            // для Омска переименовываем остатки
            if (IsOmsk && e.Row.Cells[0].Value != null)
            {
                string rowName = e.Row.Cells[0].Value.ToString();

                rowName = rowName.Replace("Остаток средств на начало квартала", "Остаток средств на начало года");
                rowName = rowName.Replace("Остаток средств на конец квартала", "Остаток средств на дату");

                rowName = rowName.Replace("безвозмездные поступления", "безвозмездные поступления целевого характера");
                rowName = rowName.Replace("собственные", "налоговые и неналоговые доходы и безвозмездные поступления нецелевого характера");

                e.Row.Cells[0].Value = rowName;
            }

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
            else
            {
                e.Row.Cells[1].Value = Convert.ToInt32(value).ToString("000");
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

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = lbTitle.Text + " " + lbSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.CurrentWorksheet.Columns[0].Width = 350 * 37;
            for (int i = 1; i < grid.Bands[0].Columns.Count; i++)
            {
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0";
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
