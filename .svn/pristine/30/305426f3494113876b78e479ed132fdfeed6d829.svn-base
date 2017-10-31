using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraGauge.Resources;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0064
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart;

        private int firstYear = 2009;
        private int endYear = 2010;
        private string periodCurrentYearStr;
        private string periodLastYearStr;
        private DateTime currentDate;
        private DateTime lastdate;

        #region Параметры запроса

        // Выбранный открывающий период
        private CustomParam openPeriodYear;
        // Выбранный год закрывающего периода
        private CustomParam periodYear;
        // Выбранный месяц открывающего периода
        private CustomParam openPeriodMonth;
        // Выбранный месяц закрывающего периода
        private CustomParam periodMonth;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.6 - 280);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 2);
            gaugeTable1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2).ToString();
            gaugeTable2.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2).ToString();
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraGauge1.Height = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 4.5);
            UltraGauge2.Height = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 4.5);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            #region Инициализация параметров запроса

            if (openPeriodYear == null)
            {
                openPeriodYear = UserParams.CustomParam("open_period_year");
            }
            if (periodYear == null)
            {
                periodYear = UserParams.CustomParam("close_period_year");
            }
            if (openPeriodMonth == null)
            {
                openPeriodMonth = UserParams.CustomParam("open_period_month");
            }
            if (periodMonth == null)
            {
                periodMonth = UserParams.CustomParam("close_period_month");
            }

            #endregion

            #region Настройка диаграммы

            #endregion

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0064_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            string month = dtDate.Rows[0][3].ToString();

            DateTime date = new DateTime(
            Convert.ToInt32(dtDate.Rows[0][0].ToString()),
            CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
            Convert.ToInt32(dtDate.Rows[0][4].ToString()));
            if (!Page.IsPostBack)
            {
                CustomCalendar1.WebCalendar.SelectedDate = date;
            }
            currentDate = CustomCalendar1.WebCalendar.SelectedDate;
            if (!Page.IsPostBack)
            {
                Label1_1.Text = String.Format("Исполнено за текущий период: ");
                Label2_1.Text = String.Format("Исполнено за аналогичный период предыдущего года: ");
                Label3_1.Text = String.Format("Исполнено за текущий период: ");
                Label4_1.Text = String.Format("Исполнено за аналогичный период предыдущего года: ");

                Label1.Text = string.Empty;
                Label2.Text = string.Empty;
                Label3.Text = string.Empty;
                Label4.Text = string.Empty;

            }

            UserParams.PeriodYear.Value = CRHelper.PeriodMemberUName(string.Empty, CustomCalendar1.WebCalendar.SelectedDate, 5);
            UserParams.PeriodLastYear.Value = UserParams.PeriodYear.Value.Replace(CustomCalendar1.WebCalendar.SelectedDate.Year.ToString(), Convert.ToString(CustomCalendar1.WebCalendar.SelectedDate.Year - 1));
            UserParams.PeriodFirstYear.Value = CustomCalendar1.WebCalendar.SelectedDate.Year.ToString();

            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            Page.Title = String.Format("Анализ капитальных вложений");
            PageTitle.Text = String.Format("Анализ капитальных вложений за период с 01.01.{0} по {1:dd.MM.yyyy}", currentDate.Year, currentDate);
            PageSubTitle.Text = String.Format("по состоянию на {0:dd.MM.yyyy} года", date);
            chartCaptionLabel1.Text = "Кассовое исполнение за счет средств областного бюджета, %";
            chartCaptionLabel2.Text = "Кассовое исполнение за счет средств федерального бюджета, %"; //и иных безвозмездных поступлений (без учета средств, выделенных из федерального бюджета)";
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

        }

        private static string GetSpace(int length)
        {
            const string spaceString = " ";
            return spaceString.Substring(0, length);
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0064_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }

            if ((dtGrid.Rows[4][2] != DBNull.Value) && (dtGrid.Rows[1][2] != DBNull.Value) && (dtGrid.Rows[4][2].ToString() != string.Empty) && (dtGrid.Rows[1][2].ToString() != string.Empty))
            {
                Label1.Text = (Convert.ToDouble(dtGrid.Rows[4][2]) / Convert.ToDouble(dtGrid.Rows[1][2])).ToString("P1");
            }
            if ((dtGrid.Rows[4][1] != DBNull.Value) && (dtGrid.Rows[1][1] != DBNull.Value))
            {
                Label2.Text = (Convert.ToDouble(dtGrid.Rows[4][1]) / Convert.ToDouble(dtGrid.Rows[1][1])).ToString("P1");
            }
            if ((dtGrid.Rows[5][2] != DBNull.Value) && (dtGrid.Rows[2][2] != DBNull.Value))
            {
                Label3.Text = (Convert.ToDouble(dtGrid.Rows[5][2]) / Convert.ToDouble(dtGrid.Rows[2][2])).ToString("P1");
            }
            if ((dtGrid.Rows[5][1] != DBNull.Value) && (dtGrid.Rows[2][1] != DBNull.Value))
            {
                Label4.Text = (Convert.ToDouble(dtGrid.Rows[5][1]) / Convert.ToDouble(dtGrid.Rows[2][1])).ToString("P1");
            }

            ((RadialGauge)UltraGauge1.Gauges[0]).Scales[0].Axis.SetStartValue(0);
            ((RadialGauge)UltraGauge1.Gauges[0]).Scales[0].Axis.SetEndValue(100);
            if ((dtGrid.Rows[4][2] != DBNull.Value) && (dtGrid.Rows[1][2] != DBNull.Value) && (dtGrid.Rows[4][2].ToString() != string.Empty) && (dtGrid.Rows[1][2].ToString() != string.Empty))
            {
                ((RadialGauge)UltraGauge1.Gauges[0]).Scales[0].Markers[0].Value = (Convert.ToDouble(dtGrid.Rows[4][2]) / Convert.ToDouble(dtGrid.Rows[1][2]) * 100);//кр. стрелка
            }
            if ((dtGrid.Rows[4][1] != DBNull.Value) && (dtGrid.Rows[1][1] != DBNull.Value))
            {
                ((RadialGauge)UltraGauge1.Gauges[0]).Scales[0].Markers[1].Value = (Convert.ToDouble(dtGrid.Rows[4][1]) / Convert.ToDouble(dtGrid.Rows[1][1]) * 100);//чёрн. стрелка
            }
            ((RadialGauge)UltraGauge1.Gauges[0]).Scales[0].Ranges[0].StartValue = 0;
            ((RadialGauge)UltraGauge1.Gauges[0]).Scales[0].Ranges[0].EndValue = 100;
            ((RadialGauge)UltraGauge1.Gauges[0]).Scales[0].Ranges[1].StartValue = 0;
            ((RadialGauge)UltraGauge1.Gauges[0]).Scales[0].Ranges[1].EndValue = 90;
            ((RadialGauge)UltraGauge1.Gauges[0]).Scales[0].Ranges[2].StartValue = 90;
            ((RadialGauge)UltraGauge1.Gauges[0]).Scales[0].Ranges[2].EndValue = 100;

            ((RadialGauge)UltraGauge2.Gauges[0]).Scales[0].Axis.SetStartValue(0);
            ((RadialGauge)UltraGauge2.Gauges[0]).Scales[0].Axis.SetEndValue(100);
            if ((dtGrid.Rows[5][2] != DBNull.Value) && (dtGrid.Rows[2][2] != DBNull.Value))
            {
                ((RadialGauge)UltraGauge2.Gauges[0]).Scales[0].Markers[0].Value = (Convert.ToDouble(dtGrid.Rows[5][2]) / Convert.ToDouble(dtGrid.Rows[2][2]) * 100 );//кр. стрелка
            }
            if ((dtGrid.Rows[5][1] != DBNull.Value) && (dtGrid.Rows[2][1] != DBNull.Value))
            {
                ((RadialGauge)UltraGauge2.Gauges[0]).Scales[0].Markers[1].Value = (Convert.ToDouble(dtGrid.Rows[5][1]) / Convert.ToDouble(dtGrid.Rows[2][1]) * 100 );//чёрн. стрелка
            }
            ((RadialGauge)UltraGauge2.Gauges[0]).Scales[0].Ranges[0].StartValue = 0;
            ((RadialGauge)UltraGauge2.Gauges[0]).Scales[0].Ranges[0].EndValue = 100;
            ((RadialGauge)UltraGauge2.Gauges[0]).Scales[0].Ranges[1].StartValue = 0;
            ((RadialGauge)UltraGauge2.Gauges[0]).Scales[0].Ranges[1].EndValue = 90;
            ((RadialGauge)UltraGauge2.Gauges[0]).Scales[0].Ranges[2].StartValue = 90;
            ((RadialGauge)UltraGauge2.Gauges[0]).Scales[0].Ranges[2].EndValue = 100;
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {

        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            string[] captionStr = e.Layout.Bands[0].Columns[0].Header.Caption.Split(';');
            if (captionStr.Length > 1)
            {
                e.Layout.Bands[0].Columns[0].Header.Caption = captionStr[1];
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                captionStr = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                if (captionStr.Length > 1)
                {
                    e.Layout.Bands[0].Columns[i].Header.Caption = captionStr[1];
                }

                string formatString = "N1";
                int widthColumn = 150;


                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(230);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            int multiHeaderPos = 1;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                int columnGroupNum = (i - 1) / 2;

                ColumnHeader ch = new ColumnHeader(true);

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, string.Format("с 01.01.{0} по {1:dd.MM.yyyy}, руб.", currentDate.Year - 1, currentDate.AddYears(-1)), "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, string.Format("с 01.01.{0} по {1:dd.MM.yyyy}, руб.", currentDate.Year, currentDate), "");
                ch.Caption = String.Format("Период");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
  
                bool rate = (i == 6);

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            e.Row.Cells[i].Title = "Прирост к прошлому году";
                        }
                        else if (Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            e.Row.Cells[i].Title = "Снижение к прошлому году";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString().Contains("Итого"))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != String.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
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

        #endregion

        #region Обработчики диаграммы



        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {

        }

        #endregion

        #region Экспорт в Excel

        private int startRowIndex = 4;

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            e.CurrentWorksheet.Columns[0].Width = 250 * 37;
            for (int i = 1; i < columnCount; i++)
            {
                string formatString = "#,#0.0;[Red]-#,#0.0";

                if (UltraWebGrid.Bands[0].Columns[i].Header.Caption.Contains("Темп роста"))
                {
                    formatString = "#,#0.0%;[Red]-#,#0.0%";
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = 100 * 37;
            }

            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[startRowIndex - 1].Height = 17 * 37;
                e.CurrentWorksheet.Rows[startRowIndex - 1].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[startRowIndex - 1].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[startRowIndex - 1].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[startRowIndex].Height = 17 * 37;
                e.CurrentWorksheet.Rows[startRowIndex].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[startRowIndex].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[startRowIndex].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = startRowIndex;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            switch (e.CurrentColumnIndex)
            {
                case 1:
                case 2:
                    {
                        e.HeaderText = String.Format("Период: {0}", periodLastYearStr);
                        break;
                    }
                case 3:
                case 4:
                    {
                        e.HeaderText = String.Format("Период: {0}", periodCurrentYearStr);
                        break;
                    }
                case 5:
                case 6:
                    {
                        e.HeaderText = "Сравнение с аналогичным периодом прошлого года в среднем за месяц";
                        break;
                    }
                default:
                    {
                        e.HeaderText = UltraWebGrid.Bands[0].Columns[e.CurrentColumnIndex].Header.Caption;
                        break;
                    }
            }
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
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

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            //title.AddContent(chartCaptionLabel1.Text);

        }

        #endregion
    }
}

