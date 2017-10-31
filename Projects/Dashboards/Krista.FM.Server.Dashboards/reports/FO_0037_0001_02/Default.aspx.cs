using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0037_0001_02
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2000;
        private int endYear = 2011;

        #endregion

        #region Параметры запроса

        // Период
        private CustomParam periodDate;
        
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 120);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

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
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0037_0001_02_date");
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

            Page.Title = "Оперативные данные об исполнении бюджета субъекта по расходам";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = "на " + GetDateString(ComboPeriod.GetSelectedNodePath(), ComboPeriod.SelectedNode.Level);

            periodDate.Value = GetDateUniqName(ComboPeriod.GetSelectedNodePath(), ComboPeriod.SelectedNode.Level);

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
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
            string query = DataProvider.GetQueryText("FO_0037_0001_02_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = CRHelper.ToUpperFirstSymbol(row[0].ToString().ToLower());
                    }

                    for (int i = 1; i < row.ItemArray.Length; i++)
                    {
//                        if ((i == 1 || i == 2 || i == 3 || i == 4) && row[i] != DBNull.Value)
//                        {
//                            row[i] = Convert.ToDouble(row[i]) / 1000;
//                        }
                        if ((i == 5 || i == 6) && row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) / 100;
                        }
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Height = Unit.Empty;
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
                            formatString = "N2";
                            widthColumn = 130;
                            break;
                        }
                    case 5:
                    case 6:
                        {
                            formatString = "P2";
                            widthColumn = 120;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 400;
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
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Исполнено за аналогичный период", "Исполнено за аналогичный период предыдущего года");
                }
                else
                {
                    if (year != -1)
                    {
                        ch.Caption = string.Format("{0} год", year);
                    }

                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Годовые назначения", "План на год");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, string.Format("Исполнено на {0}", GetDateString(ComboPeriod.GetSelectedNodePath(), ComboPeriod.SelectedNode.Level)), "Исполнено с начала года");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, string.Format("Рост в % к исполнению {0} года", year - 1), "Темп прироста по отношению к аналогичному периоду предыдущего года");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "Удельный вес к годовым назначениям", "% исполнения годовых назначений");
                }

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += (i == 1) ? 2 : 4;
                ch.RowLayoutColumnInfo.SpanX = (i == 1) ? 2 : 4;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
 
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string monthStr = GetMonth(ComboPeriod.GetSelectedNodePath());
            int monthNum = CRHelper.MonthNum(monthStr);

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool complete = (i == 6);

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
                    }

                    if (e.Row.Cells[0].Value != null &&
                        (e.Row.Cells[0].Value.ToString().Contains("Налоговые и неналоговые доходы") ||
                         e.Row.Cells[0].Value.ToString().Contains("Расходы бюджета-итого")))
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;

            int width = 101;

            for (int i = 1; i < columnCount; i++)
            {
                string formatString = "P2";

                switch (i)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        {
                            formatString = UltraGridExporter.ExelNumericFormat;
                            break;
                        }
                    case 5:
                    case 6:
                        {
                            formatString = UltraGridExporter.ExelPercentFormat;
                            break;
                        }
                }
                e.CurrentWorksheet.Columns[i].Width = width * 37;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
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
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {

        }

        #endregion
    }
}
