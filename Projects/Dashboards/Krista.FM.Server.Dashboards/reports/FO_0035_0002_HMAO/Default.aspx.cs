using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using InitializeRowEventHandler=Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;

namespace Krista.FM.Server.Dashboards.reports.FO_0035_0002_HMAO
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtExecute = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtGrid = new DataTable();
        private UltraWebGrid grid = new UltraWebGrid();
        private DateTime lastDataDate;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            PopupInformer1.HelpPageUrl = "Default.html";

            gridPlaceHolder.Controls.Clear();

            grid = new UltraWebGrid();
            grid.EnableViewState = false;
            grid.DisplayLayout.Reset();
            grid.Bands.Clear();
            grid.SkinID = "UltraWebGrid";
            gridPlaceHolder.Controls.Add(grid);

            grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 50);
            ultraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
            ultraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenWidth * 0.6 - 130);
            grid.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenWidth * 0.6 - 130);
            grid.DisplayLayout.RowHeightDefault = 23;

            ComboYear.Width = 100;
            ComboYear.Title = "Год";
            ComboQuater.Title = "Квартал";

            ComboQuater.Width = 150;
            Parameter.Width = 500;

            Link1.Visible = true;
            Link1.Text = "Исполнение&nbsp;кассового&nbsp;плана";
            Link1.NavigateUrl = "~/reports/FO_0035_0001_HMAO/Default.aspx";

            Link2.Visible = true;
            Link2.Text = "Структура&nbsp;расходов&nbsp;бюджета&nbsp;автономного&nbsp;округа&nbsp;по&nbsp;ведомствам";
            Link2.NavigateUrl = "~/reports/FO_0035_0003_HMAO/Default.aspx";

            Link3.Visible = true;
            Link3.Text = "Исполнение&nbsp;кассового&nbsp;плана&nbsp;(с&nbsp;процентом&nbsp;исполнения)";
            Link3.NavigateUrl = "~/reports/FO_0035_0003_2_HMAO/Default.aspx";

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.MultiHeader = true;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            // Получаем последнюю дату
            lastDataDate = GetLastDataDate();

            if (!Page.IsPostBack)
            {
                InitializeComboDate(lastDataDate);
                UserParams.CubeName.Value = "[ФО_Исполнение кассового плана]";
                lbChartTitle.Text = "Остаток средств бюджета";
            }
            // колотим дату из календаря
            DateTime date = GetDateFromParametrs();

            bool fullQuater = date < lastDataDate;
            // Выбираем последнюю дату с данными.
            lastDataDate = fullQuater ? date : lastDataDate;

            // Инициализируем параметры даты из последней даты
            UserParams.PeriodQuater.Value = CRHelper.PeriodMemberUName(String.Empty, lastDataDate, 3);

            // Заполним дерево
            if (!IsPostBack)
            {
                Dictionary<string, int> parametrs = GetParametrsCollection();
                Parameter.FillDictionaryValues(parametrs);
                Parameter.SetСheckedState("Доходы бюджета", true);
            }
            Parameter.ParentSelect = true;
            // Выставляем параметры.
            SetParamsByGroup();
            // биндим данные.
            ultraChart.DataBind();
            grid.DataBind();

            // настраиваем лейблы
            SetupTitle(fullQuater);
        }

        private void SetupTitle(bool fullQuater)
        {
            // Данные или на последний день квартала или на последний день данных
            lbTitle.Text =
                CRHelper.ToUpperFirstSymbol(String.Format("Динамика исполнения кассового плана за {0}",
                                                          CRHelper.PeriodDescr(lastDataDate, 3)));
            string subTitle = string.Empty;
            if (!fullQuater)
            {
                subTitle = "(на " + lastDataDate.ToString("dd.MM.yyyy") + "г.)";
            }
            lbSubTitle.Text = subTitle + " по бюджету автономного округа";
            Parameter.Title = "Показатель";
        }

        #region Работа с параметрами

        private DateTime GetDateFromParametrs()
        {
            int selectedQuaterNum = ComboQuater.SelectedIndex + 1;
            int monthNum = CRHelper.QuarterLastMonth(selectedQuaterNum);
            return new DateTime(
                Int32.Parse(ComboYear.SelectedValue),
                monthNum, CRHelper.MonthLastDay(monthNum));
        }

        private void InitializeComboDate(DateTime date)
        {
            ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(2010, 2011));
            ComboQuater.FillDictionaryValues(CustomMultiComboDataHelper.FillQuaters());
            // В первый раз инициализируем календарь из даты.
            ComboYear.SetСheckedState(date.Year.ToString(), true);
            int quaterNum = CRHelper.QuarterNumByMonthNum(date.Month);
            ComboQuater.SetСheckedState(String.Format("Квартал {0}", quaterNum), true);
        }

        private DateTime GetLastDataDate()
        {
            string query = DataProvider.GetQueryText("date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(
                query, dtDate);
            return new DateTime(
                Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                Convert.ToInt32(dtDate.Rows[0][4].ToString()));
        }

        private Dictionary<string, int> GetParametrsCollection()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            string query = DataProvider.GetQueryText("Execute");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "name", dtExecute);

            for (int i = 0; i < dtExecute.Rows.Count; i++)
            {
                result.Add(dtExecute.Rows[i][0].ToString(), 0);
            }
            return result;
        }

        private static string GetMonth(string month)
        {
            month = month.Trim('(');
            month = month.Replace(" ДАННЫЕ)", String.Empty);
            return month;
        }

        private void SetParamsByGroup()
        {
            string value = Parameter.SelectedValue;
            if (value == "Доходы бюджета")
            {
                value = "Доходы областного бюджета";
            }

            ultraChart.DataBinding += new EventHandler(ultraChart_DataBindingIncomes);
            SetGridHandlers();

            SetUpChartLabel(value);
            UserParams.SelectItem.Value = value;

            UserParams.Organization.Value = String.Format("[Показатели].[ИспКасПлан].[Все].[{0}]", value);
            ultraChart.InvalidDataReceived += CRHelper.UltraChartInvalidDataReceived;
        }

        #endregion

        #region Обработчики грида

        private void SetGridHandlers()
        {
            grid.DataBinding += new EventHandler(grid_DataBindingIncomes);
            grid.InitializeLayout += new InitializeLayoutEventHandler(grid_InitializeLayoutIncomes);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExportIncomes);
            grid.InitializeRow += new InitializeRowEventHandler(grid_InitializeRowIncomes);
            grid.DataBound += new EventHandler(grid_DataBound);
        }

        private void grid_DataBindingIncomes(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("GridIncomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "День", dtGrid);
            grid.DataSource = dtGrid;
        }

        private void grid_DataBound(object sender, EventArgs e)
        {
            grid.Height = Unit.Empty;
            grid.Width = Unit.Empty;
        }

        private void grid_InitializeLayoutIncomes(object sender, LayoutEventArgs e)
        {
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.ColorLeft =
                Color.FromArgb(192, 192, 192);

            e.Layout.Bands[0].Columns[0].Width = 50;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 5;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
            }

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
        }

        private void grid_InitializeRowIncomes(object sender, RowEventArgs e)
        {
            TrimMonthCell(e.Row.Cells[0]);

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    double cellValue;
                    if (Double.TryParse(cell.Value.ToString(), out cellValue))
                    {
                        if (cellValue < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        private static void TrimMonthCell(UltraGridCell cell, int index = 0)
        {
            int value;
            if (!Int32.TryParse(cell.Value.ToString(), out value))
            {
                cell.Value = GetMonth(cell.Value.ToString());
                cell.ColSpan = cell.Row.Cells.Count - 1 - index > 0 ? cell.Row.Cells.Count - 1 - index : 1;
                cell.Style.Font.Bold = true;
                cell.Row.Style.BorderDetails.ColorTop = Color.FromArgb(192, 192, 192);
            }
        }


        #endregion

        #region Обработчики диаграммы

        private void ultraChart_DataBindingIncomes(object sender, EventArgs e)
        {
            SetColumnChartAppearance();

            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("ChartIncomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "name", dtChart);

            if (dtChart.Columns.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);
            }

            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                DateTime dateTime = CRHelper.PeriodDayFoDate(dtChart.Rows[i][0].ToString());
                dtChart.Rows[i][0] = string.Format("{0:dd.MM.yy}", dateTime);
            }

            ultraChart.DataSource = dtChart;
            ultraChart.Data.SwapRowsAndColumns = false;
        }

        private void SetChartAppearance()
        {
            ultraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            ultraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            ultraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            ultraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            ultraChart.Axis.X.Labels.Font = new Font("Verdana", 8);
            ultraChart.Axis.X.Labels.Visible = false;
            ultraChart.Tooltips.FormatString = "<ITEM_LABEL> <br />  <SERIES_LABEL> <br /> <DATA_VALUE:N0> тыс.руб.";
            SetLabelsClipTextBehavior(ultraChart.Axis.X.Labels.SeriesLabels.Layout);
            ultraChart.Axis.Y.Extent = 60;

            ultraChart.TitleLeft.Visible = true;
            ultraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            ultraChart.TitleLeft.Margins.Bottom = Convert.ToInt32(ultraChart.Height.Value)*ultraChart.Legend.SpanPercentage / 100;
            ultraChart.TitleLeft.Text = "Тыс.руб.";
            ultraChart.TitleLeft.Font = new Font("Verdana", 8);
        }

        private void SetColumnChartAppearance()
        {
            ultraChart.ChartType = ChartType.StackColumnChart;
            SetChartAppearance();
            ultraChart.Legend.Margins.Right = (int)ultraChart.Width.Value / 4;
            ultraChart.Legend.SpanPercentage = 7;
        }

        private void SetLabelsClipTextBehavior(AxisLabelLayoutAppearance layout)
        {
            layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;
            layout.BehaviorCollection.Clear();
            ClipTextAxisLabelLayoutBehavior behavior = new ClipTextAxisLabelLayoutBehavior();
            behavior.ClipText = false;
            behavior.Enabled = true;
            behavior.Trimming = StringTrimming.None;
            behavior.UseOnlyToPreventCollisions = false;
            layout.BehaviorCollection.Add(behavior);
        }

        private void SetUpChartLabel(string value)
        {
            if (Parameter.SelectedNode.Parent != null)
            {
                string parentText = Parameter.SelectedNodeParent.ToLower();
                parentText = CRHelper.ToUpperFirstSymbol(parentText);
                parentText = parentText.Replace("- всего", String.Empty);
                lbChartTitle.Text = parentText + " &#8212 " + value;
            }
            else
            {
                lbChartTitle.Text = value;
            }
        }

        #endregion

        #region Экспорт

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = lbTitle.Text + " " + lbSubTitle.Text;
        }

        private void ExcelExporter_EndExportIncomes(object sender, EndExportEventArgs e)
        {
            for (int i = 1; i < grid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0";
                int widthColumn = 80;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = grid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
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

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(lbChartTitle.Text.Replace("&#8212", "-"));

            ultraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.82);
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(ultraChart);
            e.Section.AddImage(img);
        }

        #endregion

    }
}