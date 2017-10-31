using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Band;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Reports.Report.TOC;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color=System.Drawing.Color;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font=System.Drawing.Font;
using IList = Infragistics.Documents.Reports.Report.List.IList;

namespace Krista.FM.Server.Dashboards.reports.FO_0037_0001_03
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2000;
        private int endYear = 2011;
        private DateTime currentDateTime;

        #endregion

        #region Параметры запроса

        // Период
        private CustomParam periodDate;
        
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 120);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);

            #region Инициализация параметров запроса

            if (periodDate == null)
            {
                periodDate = UserParams.CustomParam("period_date");
            }

            #endregion

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Text = "Ежемесячные&nbsp;данные&nbsp;по&nbsp;исполнению&nbsp;областного&nbsp;бюджета";
            CrossLink1.NavigateUrl = "~/reports/FO_0037_0001_01/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0037_0001_03_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboPeriod.Width = 300;
                ComboPeriod.MultiSelect = false;
                ComboPeriod.ShowSelectedValue = false;
                ComboPeriod.ParentSelect = false;
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillHotInfoNonEmptyDays(DataDictionariesHelper.HotInfoNonEmptyDays));
                ComboPeriod.SelectLastNode();
                ComboPeriod.PanelHeaderTitle = "Выберите дату";
            }

            currentDateTime = GetDateTime(ComboPeriod.GetSelectedNodePath(), ComboPeriod.SelectedNode.Level);
            string dateTimeStr = String.Format("на {0:dd.MM.yyyy}", currentDateTime);

            Page.Title = "Справочная информация об исполнении областного бюджета по налоговым и неналоговым доходам";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = dateTimeStr + " в динамике к предыдущему году";
            CommentText.Text = "Еженедельная информация об исполнении собственного бюджета субъекта по доходам и расходам.";
            CommentText.Font.Italic = true;
            
            dataItemCaption.Text = "(тыс.руб.)";

            incomesGridLabel.Text = string.Empty;

            periodDate.Value = GetDateUniqName(ComboPeriod.GetSelectedNodePath(), ComboPeriod.SelectedNode.Level);

            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
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
                            return GetDateUniqName(ComboPeriod.GetNodeLastChild(ComboPeriod.SelectedNode), level + 1);
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

        public DateTime GetDateTime(string source, int level)
        {
            string[] sts = source.Split('|');
            if (sts.Length > 0)
            {
                switch (level)
                {
                    // нулевой уровень выбрать нельзя
                    case 1:
                        {
                            return GetDateTime(ComboPeriod.GetNodeLastChild(ComboPeriod.SelectedNode), level + 1);
                        }
                    case 2:
                        {
                            string month = sts[1].TrimEnd(' ');
                            string day = sts[2].TrimEnd(' ');
         
                            DateTime dateTime = new DateTime(Convert.ToInt32(sts[0]), CRHelper.MonthNum(month), Convert.ToInt32(day));

                            if (CRHelper.MonthLastDay(CRHelper.MonthNum(month)) == Convert.ToInt32(day))
                            {
                                dateTime = dateTime.AddDays(1);
                            }
                                
                            return dateTime;
                            
                        }
                }
            }
            return new DateTime(2020, 1, 1);
        }

        public int GetYear(string source)
        {
            string[] sts = source.Split('|');
            if (sts.Length > 0)
            {
                return Convert.ToInt32(sts[0]);
            }
            return -1;
        }

        public string GetMonth(string source)
        {
            string[] sts = source.Split('|');
            if (sts.Length > 1)
            {
                return sts[1];
            }
            return string.Empty;
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = (sender == UltraWebGrid1) ? 
                DataProvider.GetQueryText("FO_0037_0001_03_grid_incomes") : 
                DataProvider.GetQueryText("FO_0037_0001_03_grid_outcomes");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length; i++)
                    {
                        if ((i == 1 || i == 2 || i == 3 || i == 4) && row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) / 1000;
                        }
                    }
                }

                ((UltraWebGrid)sender).DataSource = dtGrid;

                string dateTimeStr = String.Format("на {0:dd.MM.yyyy}", currentDateTime);
                double currYearFact = 0;
                double prevYearFact = 0;
                double currYearPlan = 0;
                double executePercent = 0;
                double rateValue = 0;
                string rateArrow = string.Empty;
                int year = GetYear(ComboPeriod.GetSelectedNodePath());

                for (int i = 1; i < dtGrid.Columns.Count; i++)
                {
                    double value = 0;
                    if (dtGrid.Rows[0][i] != DBNull.Value && dtGrid.Rows[0][i].ToString() != string.Empty)
                    {
                        value = Convert.ToDouble(dtGrid.Rows[0][i].ToString());
                    }

                    switch (i)
                    {
                        case 2:
                            {
                                prevYearFact = value;
                                break;
                            }
                        case 4:
                            {
                                currYearFact = value;
                                break;
                            }
                        case 3:
                            {
                                currYearPlan = value;
                                break;
                            }
                        case 5:
                            {
                                rateValue = value / 100;
                                rateArrow = rateValue > 0 ? "<img src=\"../../images/arrowGreenUpBB.png\" width=\"13px\" height=\"16px\"> больше"
                                    : "<img src=\"../../images/arrowRedDownBB.png\" width=\"13px\" height=\"16px\"> меньше";
                                break;
                            }
                        case 6:
                            {
                                executePercent = value / 100;
                                break;
                            }
                    }
                }

                incomesGridLabel.Text = string.Format(
                            @"Налоговые и неналоговые доходы в областной бюджет <b>{0}</b> поступили в объеме <b>{1:N0}</b> тыс.руб. 
Уточненные годовые назначения по налоговым и неналоговым доходам составляют <b>{2:N0}</b> тыс.руб., доходная часть бюджета исполнена на <b>{3:P2}</b>. 
Доходов поступило на <b>{4:P2}</b> {6}, чем за аналогичный период предыдущего года (в {7} году поступления составили <b>{5:N0}</b> тыс.руб.).",
                            dateTimeStr, currYearFact, currYearPlan, executePercent, Math.Abs(rateValue), prevYearFact, rateArrow, year - 1);
            }
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            ((UltraWebGrid)sender).Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "P2";
                int widthColumn = 80;

                switch (i)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        {
                            formatString = "N0";
                            widthColumn = 120;
                            break;
                        }
                    case 5:
                    case 6:
                    case 7:
                        {
                            formatString = "N1";
                            widthColumn = 110;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;
            int year = GetYear(ComboPeriod.GetSelectedNodePath());

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 3; i = i + 2)
            {
                ColumnHeader ch = new ColumnHeader(true);

                if (i == 1)
                {
                    if (year != -1)
                    {
                        ch.Caption = string.Format("{0} год", year - 1);
                    }

                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Исполнено за год", "Исполнено за предыдущий год");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1,
                        String.Format("Исполнено на {0:dd.MM.yyyy}", currentDateTime.AddYears(-1)), "Исполнено за аналогичный период предыдущего года");
                }
                else
                {
                    if (year != -1)
                    {
                        ch.Caption = string.Format("{0} год", year);
                    }

                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Годовые назначения", "План на год");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, string.Format("Исполнено на {0:dd.MM.yyyy}", currentDateTime), "Исполнено с начала года");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, string.Format("Рост в % к исполнению {0} года", year - 1), "Темп прироста по отношению к аналогичному периоду предыдущего года");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "Удельный вес к годовым назначениям, %", "% исполнения годовых назначений");
                    string weightStr = (sender == UltraWebGrid1) ? "Удельный вес в общем объеме доходов, %" : "Удельный вес в общем объеме расходов";
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, weightStr, weightStr);
                }

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += (i == 1) ? 2 : 5;
                ch.RowLayoutColumnInfo.SpanX = (i == 1) ? 2 : 5;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
 
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string monthStr = GetMonth(ComboPeriod.GetSelectedNodePath());
            int monthNum = CRHelper.MonthNum(monthStr.TrimEnd(' '));

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool complete = (i == 6);
                bool rate = (i == 5);

                if (complete)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double percent = monthNum * 100.0 / 12;

                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < percent)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                            e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:N2}%)", percent);
                        }
                        else
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:N2}%)", percent);
                        }
                        e.Row.Cells[i].Style.Padding.Right = 2;
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Прирост к прошлому году";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Снижение относительно прошлого года";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                        else if (value == 0)
                        {
                            e.Row.Cells[i].Value = null;
                        }
                    }

                    if (e.Row.Cells[0].Value != null &&
                        (e.Row.Cells[0].Value.ToString().ToLower().Contains("налоговые и неналоговые доходы") ||
                         e.Row.Cells[0].Value.ToString().ToLower().Contains("расходы бюджета-итого")))
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private static string CommentTextExportsReplaces(string source)
        {
            string commentText = source;

            commentText = commentText.Replace("<\n>", "");
            commentText = commentText.Replace("<\r>", "");
            commentText = commentText.Replace(@"
", " ");
            commentText = commentText.Replace("&nbsp;", " ");
            commentText = commentText.Replace("<br/>", "\n");
            commentText = commentText.Replace("<b>", "");
            commentText = commentText.Replace("</b>", "");
            commentText = commentText.Replace("<img src=\"../../images/arrowGreenUpBB.png\" width=\"13px\" height=\"16px\">", "");
            commentText = commentText.Replace("<img src=\"../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">", "");
            commentText = commentText.Replace("<img src=\"../../images/arrowRedDownBB.png\" width=\"13px\" height=\"16px\">", "");
            commentText = commentText.Replace("<img src=\"../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">", "");
            return commentText;
        }

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.Workbook.Worksheets["Налоговые и неналоговые доходы"].Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            e.Workbook.Worksheets["Налоговые и неналоговые доходы"].Rows[1].Cells[0].Value = CommentText.Text;
            e.Workbook.Worksheets["Налоговые и неналоговые доходы"].Rows[2].Cells[0].Value = CommentTextExportsReplaces(incomesGridLabel.Text);
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = exportGrid.Columns.Count;
            int rowsCount = exportGrid.Rows.Count;

            e.CurrentWorksheet.MergedCellsRegions.Add(2, 0, 2, 10);
            e.CurrentWorksheet.Rows[2].Height = 20 * 37;
            e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            e.CurrentWorksheet.Rows[2].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;

            e.CurrentWorksheet.Columns[0].Width = 200 * 37;

            // расставляем стили у начальных колонок
            for (int i = 6; i < rowsCount + 6; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 17 * 37;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
                for (int j = 1; j < columnCount; j++)
                {
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.Alignment = HorizontalCellAlignment.Center;
                }
            }

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[5].Height = 17 * 37;
                e.CurrentWorksheet.Rows[5].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[5].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[5].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            int width = 110;

            for (int i = 1; i < columnCount; i++)
            {
                string formatString = "#,##0;[Red]-#,##0";

                switch (i)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        {
                            formatString = "#,##0;[Red]-#,##0";
                            break;
                        }
                    case 5:
                    case 6:
                    case 7:
                        {
                            formatString = "#,##0;[Red]-#,##0";
                            break;
                        }
                }
                e.CurrentWorksheet.Columns[i].Width = width * 37;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
            }
        }

        private UltraWebGrid exportGrid = new UltraWebGrid();

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Налоговые и неналоговые доходы");

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 5;
            exportGrid = UltraWebGrid1;
            UltraGridExporter1.ExcelExporter.Export(exportGrid, sheet1);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = exportGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];

            int year = GetYear(ComboPeriod.GetSelectedNodePath());
            switch (e.CurrentColumnIndex)
            {
                case 0:
                    {
                        e.HeaderText = "Наименование";
                        break;
                    }
                case 1:
                case 2:
                    {
                        e.HeaderText = string.Format("{0} год", year - 1);
                        break;
                    }
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    {
                        e.HeaderText = string.Format("{0} год", year);
                        break;
                    }
            }
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            Report report = new Report();
            ReportSection section1 = new ReportSection(report, true);

            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1, section1);
        }

        private bool titleAdded = false;

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            if (!titleAdded)
            {
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(115);
                }

                IText title = e.Section.AddText();
                Font font = new Font("Verdana", 16);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.Style.Font.Bold = true;
                title.AddContent(PageTitle.Text);

                title = e.Section.AddText();
                font = new Font("Verdana", 14);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent(PageSubTitle.Text);

                title = e.Section.AddText();
                font = new Font("Verdana", 12);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent(CommentText.Text);
            }

            if (!titleAdded)
            {
                IText title = e.Section.AddText();
                Font font = new Font("Verdana", 14);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent("\n" + CommentTextExportsReplaces(incomesGridLabel.Text));
            }

            titleAdded = true;
        }

        #endregion
    }

    public class ReportSection : ISection
    {
        private readonly bool withFlowColumns;
        private readonly ISection section;
        private IFlow flow;
        private ITableCell titleCell;

        public ReportSection(Report report, bool withFlowColumns)
        {
            this.withFlowColumns = withFlowColumns;
            section = report.AddSection();
            ITable table = section.AddTable();
            ITableRow row = table.AddRow();
            titleCell = row.AddCell();
        }

        public void AddFlowColumnBreak()
        {
            if (flow != null)
                flow.AddColumnBreak();
        }

        public IBand AddBand()
        {
            return section.AddBand();
        }

        #region ISection members
        public ISectionHeader AddHeader()
        {
            throw new NotImplementedException();
        }

        public ISectionFooter AddFooter()
        {
            throw new NotImplementedException();
        }

        public IStationery AddStationery()
        {
            throw new NotImplementedException();
        }

        public IDecoration AddDecoration()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(PageSize size)
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(float width, float height)
        {
            throw new NotImplementedException();
        }

        public ISegment AddSegment()
        {
            throw new NotImplementedException();
        }

        public IQuickText AddQuickText(string text)
        {
            throw new NotImplementedException();
        }

        public IQuickImage AddQuickImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            throw new NotImplementedException();
        }

        public IQuickList AddQuickList()
        {
            throw new NotImplementedException();
        }

        public IQuickTable AddQuickTable()
        {
            throw new NotImplementedException();
        }

        public IText AddText()
        {
            return this.section.AddText();
        }

        public IImage AddImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            return this.section.AddImage(image);
        }

        public IMetafile AddMetafile(Metafile metafile)
        {
            throw new NotImplementedException();
        }

        public IRule AddRule()
        {
            throw new NotImplementedException();
        }

        public IGap AddGap()
        {
            throw new NotImplementedException();
        }

        public IGroup AddGroup()
        {
            throw new NotImplementedException();
        }

        public IChain AddChain()
        {
            throw new NotImplementedException();
        }

        public ITable AddTable()
        {
            if (flow != null)
                return flow.AddTable();
            return this.section.AddTable();
        }

        public IGrid AddGrid()
        {
            throw new NotImplementedException();
        }

        public IFlow AddFlow()
        {
            throw new NotImplementedException();
        }

        public IList AddList()
        {
            throw new NotImplementedException();
        }

        public ITree AddTree()
        {
            throw new NotImplementedException();
        }

        public ISite AddSite()
        {
            throw new NotImplementedException();
        }

        public ICanvas AddCanvas()
        {
            throw new NotImplementedException();
        }

        public IRotator AddRotator()
        {
            throw new NotImplementedException();
        }

        public IContainer AddContainer(string name)
        {
            throw new NotImplementedException();
        }

        public ICondition AddCondition(IContainer container, bool fit)
        {
            throw new NotImplementedException();
        }

        public IStretcher AddStretcher()
        {
            throw new NotImplementedException();
        }

        public void AddPageBreak()
        {
            throw new NotImplementedException();
        }

        public ITOC AddTOC()
        {
            throw new NotImplementedException();
        }

        public IIndex AddIndex()
        {
            throw new NotImplementedException();
        }

        public bool Flip
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public PageSize PageSize
        {
            get { throw new NotImplementedException(); }
            set { this.section.PageSize = new PageSize(960, 1350); }
        }

        public PageOrientation PageOrientation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.ContentAlignment PageAlignment
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Borders PageBorders
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Margins PageMargins
        {
            get { return this.section.PageMargins; }
            set { throw new NotImplementedException(); }
        }

        public Paddings PagePaddings
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Background PageBackground
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Section.PageNumbering PageNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public SectionLineNumbering LineNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Report Parent
        {
            get { return this.section.Parent; }
        }

        public IEnumerable Content
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
