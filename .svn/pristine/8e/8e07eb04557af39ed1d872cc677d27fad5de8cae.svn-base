using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using System.Collections.ObjectModel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0003_0002
{
    public partial class DefaultDetail : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2003;
        private int endYear = 2011;

        #region Параметры запроса

        // множество выбранных лет
        private CustomParam yearSet;
        
        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (yearSet == null)
            {
                yearSet = UserParams.CustomParam("year_set");
            }

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.SplineChart;
            UltraChart.BorderWidth = 0;

            UltraChart.Axis.X.Extent = 50;
            UltraChart.Axis.Y.Extent = 60;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;

            CRHelper.FillCustomColorModel(UltraChart, 10, false);
            UltraChart.ColorModel.Skin.ApplyRowWise = true;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Right;
            UltraChart.Legend.SpanPercentage = 6;
            UltraChart.Legend.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value / 2);

            UltraChart.TitleLeft.Text = "Млн.руб.";
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Extent = 30;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Visible = true;

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <SERIES_LABEL>г.\n<DATA_VALUE:N3> млн.руб.";

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.3;
            UltraChart.SplineChart.LineAppearances.Add(lineAppearance);

            EmptyAppearance emptyAppearance = new EmptyAppearance();
            emptyAppearance.EnablePoint = true;
            emptyAppearance.EnablePE = true;
            emptyAppearance.EnableLineStyle = true;
            emptyAppearance.PointStyle.Icon = SymbolIcon.Circle;
            emptyAppearance.PointStyle.IconSize = SymbolIconSize.Large;
            emptyAppearance.LineStyle.MidPointAnchors = true;
            UltraChart.SplineChart.EmptyStyles.Add(emptyAppearance);

            UltraChart.Data.ZeroAligned = true;
            UltraChart.SplineChart.NullHandling = NullHandling.DontPlot;
            
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 120);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);

            int currentWidth = (int)Session["width_size"] - 20;
            UltraChart.Width = currentWidth - 20;

            int currentHeight = (int)Session["height_size"] - 150;
            UltraChart.Height = currentHeight / 2;
 
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.MultiHeader = true;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("MFRF_0003_0002_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Width = 150;
                ComboYear.Title = "Годы";
                ComboYear.MultiSelect = true;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
                ComboYear.SetСheckedState((endYear - 1).ToString(), true);
                ComboYear.SetСheckedState((endYear - 2).ToString(), true);

                ComboRegion.Width = 300;
                ComboRegion.Title = "Субъект РФ";
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));
                ComboRegion.ParentSelect = false;
                if (!string.IsNullOrEmpty(UserParams.StateArea.Value))
                {
                    ComboRegion.SetСheckedState(UserParams.StateArea.Value, true);
                }
                else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    ComboRegion.SetСheckedState(RegionSettings.Instance.Name, true);
                }
            }

            Page.Title = string.Format("Помесячная динамика государственного долга субъектов РФ");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Empty;

            UserParams.Region.Value = ComboRegion.SelectedNodeParent;
            UserParams.StateArea.Value = ComboRegion.SelectedValue;

            Collection<string> selectedValues = ComboYear.SelectedValues;
            if (selectedValues.Count > 0)
            {

                PageSubTitle.Text = string.Format("{0}, государственный долг субъектов РФ за {1} {2}, млн.руб.",
                    ComboRegion.SelectedValue, CRHelper.GetDigitIntervals(ComboYear.SelectedValuesString, ','),
                    ComboYear.SelectedValues.Count == 1 ? "год" : "годы");

                string yearSetStr = string.Empty;
                for (int i = 0; i < selectedValues.Count; i++)
                {
                    string year = selectedValues[i];
                    yearSetStr += string.Format("[Период].[Период].[Данные всех периодов].[{0}],", year);
                }

                yearSetStr = yearSetStr.TrimEnd(',');
                yearSet.Value = yearSetStr;
            }
            else
            {
                PageSubTitle.Text = string.Empty;
                yearSet.Value = " ";
            }
            
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            UltraChart.DataBind();
        }

        #region Обработчики грида

        private static bool NullValueDataTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 2; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("MFRF_0003_0002_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);

            if (dtGrid.Rows.Count > 0 && !NullValueDataTable(dtGrid))
            {
                DataTable newDtGrid = new DataTable();
                DataColumn column = new DataColumn("Год", typeof(string));
                newDtGrid.Columns.Add(column);

                for (int i = 1; i < 13; i++)
                {
                    DataColumn monthCompleteColumn = new DataColumn(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)) + "; Исполнено", typeof(double));
                    newDtGrid.Columns.Add(monthCompleteColumn);
                    DataColumn monthRateColumn = new DataColumn(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)) + "; Темп роста", typeof(double));
                    newDtGrid.Columns.Add(monthRateColumn);
                }

                int year = 0;
                DataRow currRow = null;
                foreach (DataRow row in dtGrid.Rows)
                {
                    int currYear = 0;
                    string period = string.Empty;
                    object completeValue = DBNull.Value;
                    object rateValue = DBNull.Value;
                    if (row[0] != DBNull.Value)
                    {
                        period = row[0].ToString();
                    }
                    if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                    {
                        currYear = Convert.ToInt32(row[1]);
                    }
                    if (row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                    {
                        completeValue = Convert.ToDouble(row[2]) / 1000000;
                    }

                    if (row[3] != DBNull.Value && row[3].ToString() != string.Empty)
                    {
                        rateValue = Convert.ToDouble(row[3]);
                    }

                    // добавляем новый год
                    if (year != currYear)
                    {
                        year = currYear;
                        DataRow newRow = newDtGrid.NewRow();
                        newRow[0] = year;
                        newDtGrid.Rows.Add(newRow);

                        currRow = newRow;
                    }

                    if (currRow != null && newDtGrid.Columns.Contains(period + "; Исполнено"))
                    {
                        currRow[period + "; Исполнено"] = completeValue;
                    }

                    if (currRow != null && newDtGrid.Columns.Contains(period + "; Темп роста"))
                    {
                        currRow[period + "; Темп роста"] = rateValue;
                    }
                }

                UltraWebGrid.DataSource = newDtGrid;
            }
        }

        private static string GetChartQuarterStr(string period)
        {
            string[] strs = period.Split(' ');
            if (strs.Length > 1)
            {
                double quarterNumber = Convert.ToInt32(strs[1]);
                return string.Format("{0} квартал", quarterNumber);
            }
            else
            {
                return period;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N1";
                double columnWidth = HttpContext.Current.Request.Browser.Browser == "IE" ? 45 : 47;

                if (i % 2 == 0)
                {
                    formatString = "P0";
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            int zeroColumnWidth = HttpContext.Current.Request.Browser.Browser == "IE" ? 26 : 28;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(zeroColumnWidth);

            int startOriginY = 0;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = startOriginY + 1;
                }
            }

            int multiHeaderPos = 1;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0].TrimEnd('_');

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Гос. долг", string.Format("Объем гос. долга на конец месяц, млн.руб."));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Темп роста", "Темп роста к аналогичному периоду предыдущего года");

                ch.RowLayoutColumnInfo.OriginY = startOriginY;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i % 2 == 0)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthLeft = 0;
                }
                else if (i % 2 != 0 && i != e.Row.Cells.Count - 1)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 0;
                }

                e.Row.Cells[i].Style.Padding.Left = 3;
                e.Row.Cells[i].Style.Padding.Right = 3;
                if ((i % 2 == 0 && i != 0) && e.Row.Cells[i].Value != null)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост к аналогичному периоду прошлого года";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Снижение к аналогичному периоду прошлого года";
                    }
                    if (e.Row.Cells[i].Column.Width.Value < 60)
                    {
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center;";
                    }
                    else
                    {
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; padding-left: 10px; background-position: 10px center;";
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
                }
            }
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.Width = Unit.Empty;
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("MFRF_0003_0002_chart");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                DataTable newDtChart = new DataTable();
                DataColumn column = new DataColumn("Год", typeof(string));
                newDtChart.Columns.Add(column);

                    for (int i = 1; i < 13; i++)
                    {
                        DataColumn monthColumn = new DataColumn(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(i)), typeof(double));
                        newDtChart.Columns.Add(monthColumn);
                    }

                int year = 0;
                DataRow currRow = null;
                foreach (DataRow row in dtChart.Rows)
                {
                    int currYear = 0;
                    string period = string.Empty;
                    double measureValue = double.MinValue;
                    if (row[0] != DBNull.Value)
                    {
                        period = row[0].ToString();
                        period = GetChartQuarterStr(period);
                    }
                    if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                    {
                        currYear = Convert.ToInt32(row[1]);
                    }
                    if (row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                    {
                        measureValue = Convert.ToDouble(row[2]) / 1000000;
                    }

                    // добавляем новый год
                    if (year != currYear)
                    {
                        year = currYear;
                        DataRow newRow = newDtChart.NewRow();
                        newRow[0] = year;
                        newDtChart.Rows.Add(newRow);

                        currRow = newRow;
                    }

                    if (currRow != null && newDtChart.Columns.Contains(period) && measureValue != double.MinValue)
                    {
                        currRow[period] = measureValue;
                    }
                }

                UltraChart.DataSource = newDtChart;
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
            for (int i = 1; i < UltraWebGrid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0.0";
                int widthColumn = 65;

                if (i % 2 == 0)
                {
                    formatString = UltraGridExporter.ExelPercentFormat;
                    widthColumn = 65;
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion

        #region Экспорт в Pdf

        private bool firstGridExported = false;

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(50);
            }

            if (!firstGridExported)
            {
                IText title = e.Section.AddText();
                Font font = new Font("Verdana", 16);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.Style.Font.Bold = true;
                title.AddContent(PageTitle.Text);

                title = e.Section.AddText();
                font = new Font("Verdana", 14);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent(PageSubTitle.Text.Replace("<br />", string.Empty));

                Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
                e.Section.AddImage(img);
            }
        }

        #endregion
    }
}

