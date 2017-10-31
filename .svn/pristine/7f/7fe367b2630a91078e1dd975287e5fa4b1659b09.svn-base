using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0009_0002
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2009;
        private int endYear = 2011;
        private string nonEmptySource;

        #endregion

        #region Параметры запроса

        // выбранный вариант
        private CustomParam selectedVariant;
        // источник данных
        private CustomParam dataSource;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 170);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            selectedVariant = UserParams.CustomParam("selected_variant");
            dataSource = UserParams.CustomParam("data_source");

            #endregion

            CrossLink.Visible = true;
            CrossLink.Text = "Распределение&nbsp;ФФПМР(ГО)";
            CrossLink.NavigateUrl = "~/reports/FO_0009_0001/Default.aspx";

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Visible = false;
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

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0009_0002_nonEmptySource");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);
            nonEmptySource = dtDate.Rows[0][1].ToString();
            firstYear = GetYearFromSourceName(nonEmptySource);
            endYear = firstYear + 2;

            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(firstYear.ToString(), true);

                ComboVariant.Title = "Вариант";
                ComboVariant.Width = 380;
                ComboVariant.MultiSelect = false;
                ComboVariant.FillDictionaryValues(CustomMultiComboDataHelper.FillFOFondVariantList(DataDictionariesHelper.FOFondVariantList));
                ComboVariant.SetСheckedState(String.Format("Основной по поселениям {0}", firstYear - 1), true);
            }

            selectedVariant.Value = DataDictionariesHelper.FOFondVariantList[ComboVariant.SelectedValue];
            dataSource.Value = String.Format("ФО Проект бюджета - {0}", firstYear);

            Page.Title = "Распределение фонда финансовой поддержки поселений";
            PageTitle.Text = Page.Title;

            PageSubTitle.Text = string.Format("{0} на {1} год", ComboVariant.SelectedValue, ComboYear.SelectedValue);

            UserParams.PeriodYear.Value =  ComboYear.SelectedValue;
            UserParams.PeriodLastYear.Value = ComboYear.SelectedValue;

            UltraWebGrid.Bands.Clear();     
            UltraWebGrid.DataBind();
        }

        private int GetYearFromSourceName(string sourceName)
        {
            string[] nameParts = sourceName.Split(' ');
            int yearNum = 2010;
            if (nameParts.Length > 0)
            {
                string year = nameParts[nameParts.Length - 1];
                int value;
                if (Int32.TryParse(year, out value))
                {
                    yearNum = value;
                }
            }
            return yearNum;
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0009_0002_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Полномочия", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                if (dtGrid.Columns.Count > 0)
                {
                    dtGrid.Columns.RemoveAt(0);
                }

                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("_", String.Empty);
                    }
                }

                string regionName = string.Empty;

                for(int i = 2; i < dtGrid.Columns.Count - 1; i++)
                {
                    DataColumn column = dtGrid.Columns[i];
                    if (RegionsNamingHelper.LocalBudgetTypes.ContainsKey(column.ColumnName))
                    {
                        regionName = column.ColumnName;
                        column.Caption = string.Format("{0};Всего", regionName);
                    }
                    else
                    {
                        if (column.ColumnName == "Всего")
                        {
                            regionName = "Всего";
                        }

                        column.Caption = string.Format("{1};{0}", column.ColumnName, regionName);
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "";
                int widthColumn = 100;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                if (i == 0 || i == 1)
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
            int spanX = 1;
            string currentCaption = string.Empty;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 1)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                if (captions.Length > 1)
                {
                    string headerCaption = captions[0];
                    string columnCaption = captions[1];

                    e.Layout.Bands[0].Columns[i].Key = headerCaption;

                    if (i == 1)
                    {
                        currentCaption = headerCaption;
                    }
                    else
                    {
                        if (currentCaption != headerCaption || i == e.Layout.Bands[0].Columns.Count - 2)
                        {
                            ColumnHeader ch = new ColumnHeader(true);
                            ch.Caption = currentCaption;
                            ch.RowLayoutColumnInfo.OriginY = 0;
                            ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                            multiHeaderPos += spanX;
                            ch.RowLayoutColumnInfo.SpanX = spanX;
                            e.Layout.Bands[0].HeaderLayout.Add(ch);

                            if (headerCaption.ToLower().Contains("всего") || 
                                headerCaption.ToLower().Contains("нормативы"))
                            {
                                e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                                e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                            }

                            currentCaption = headerCaption;
                            spanX = 1;
                        }
                        else
                        {
                            spanX++;
                        }
                    }

                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, columnCaption, "");
                }
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                int levelColumnIndex = e.Row.Cells.Count - 1;

                string name = string.Empty;
                if (e.Row.Cells[0].Value != null)
                {
                    name = e.Row.Cells[0].Value.ToString();
                }

                string level = string.Empty;
                if (e.Row.Cells[levelColumnIndex].Value != null)
                {
                    level = e.Row.Cells[levelColumnIndex].Value.ToString();
                }

                string indicatorName = String.Empty;
                if (e.Row.Cells[1].Value != null)
                {
                    indicatorName = e.Row.Cells[1].Value.ToString();
                }

                bool boldRow = name == "РАСХОДЫ" || name == "ВСЕГО РАСХОДЫ" || name == "ДОТАЦИИ ВСЕГО" ||
                               name == "Дотация на выравнивание бюджетной обеспеченности" || name == "Дотация на обеспечение сбалансированности бюджетов";

                bool nonZeroSignRow = name == "Индекс налогового потенциала" || name == "Индекс бюджетных расходов" ||
                                      name == "Бюджетная обеспеченность до выравнивания" || name == "Бюджетная обеспеченность после выравнивания";

                bool totalOutcomesRow = name.ToLower() == "расходы";

                bool twoSignRow = !indicatorName.ToLower().Contains("расходы муниципального образования") && !indicatorName.ToLower().Contains("численность") &&
                    level == "Вид расхода 2";

                bool koeffRow = (indicatorName.ToLower().Contains("коэффициент") || indicatorName.ToLower().Contains("разработанный норматив")) && level == "Вид расхода 2";

                bool totalRegionColumn = e.Row.Band.Columns[i].Header.Caption.ToLower().Contains("итого") || e.Row.Band.Columns[i].Header.Caption.ToLower().Contains("всего");

                if (i == 0 && level != "Вид расхода 1" && level != "Вид расхода 2" || level == "Вид расхода 1 без потомков")
                {
                    //e.Row.Cells[i].ColSpan = 2;
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 0;
                    e.Row.Cells[i + 1].Style.BorderDetails.WidthLeft = 0;
                    e.Row.Cells[i + 1].Value = "";
                }

                if (i == 0 && totalOutcomesRow)
                {
                    for (int j = 0; j < e.Row.Cells.Count - 2; j++)
                    {
                        e.Row.Cells[j].Style.BorderDetails.WidthRight = 0;
                        e.Row.Cells[j + 1].Style.BorderDetails.WidthLeft = 0;
                    }
                }

                if (boldRow || level == "Вид расхода")
                {
                    e.Row.Cells[i].Style.Font.Bold = true;   
                }

                if (koeffRow && totalRegionColumn)
                {
                    e.Row.Cells[i].Value = null;
                }

                if (i != 0 && i != 1 && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(e.Row.Cells[i].Value.ToString(), out value))
                    {
                        e.Row.Cells[i].Value = nonZeroSignRow || twoSignRow ? Convert.ToDouble(e.Row.Cells[i].Value).ToString("N2") : Convert.ToDouble(e.Row.Cells[i].Value).ToString("N0");
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

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "";
            e.CurrentWorksheet.Columns[0].Width = 200 * 37;
            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "";
            e.CurrentWorksheet.Columns[1].Width = 200 * 37;

            // расставляем стили у начальных колонок
            for (int i = 5; i < rowsCount + 5; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 17 * 37;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;

                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.Alignment = HorizontalCellAlignment.Left;

                string name = string.Empty;
                if (e.CurrentWorksheet.Rows[i].Cells[0].Value != null)
                {
                    name = e.CurrentWorksheet.Rows[i].Cells[0].Value.ToString();
                }

                int gridRowIndex = i - 4;
                string level = string.Empty;
                if (gridRowIndex < rowsCount && UltraWebGrid.Rows[gridRowIndex].Cells[columnCount - 1].Value != null)
                {
                    level = UltraWebGrid.Rows[gridRowIndex].Cells[columnCount - 1].Value.ToString();
                }

                //CRHelper.SaveToErrorLog(string.Format("row #{1}: {2} {0}", level, gridRowIndex, name));
                if (level != "Вид расхода 1" && level != "Вид расхода 2")
                {
                    if (name.ToLower() == "расходы")
                    {
                        e.CurrentWorksheet.MergedCellsRegions.Add(i, 0, i, columnCount - 2);
                    }
                    else
                    {
                        e.CurrentWorksheet.MergedCellsRegions.Add(i, 0, i, 1);
                    }
                }

                for (int j = 2; j < columnCount; j++)
                {
                    bool nonZeroSignRow = name == "Индекс налогового потенциала" || name == "Индекс бюджетных расходов" ||
                       name == "Бюджетная обеспеченность до выравнивания" || name == "Бюджетная обеспеченность после выравнивания";

                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.FormatString = nonZeroSignRow ? "#,##0.00;[Red]-#,##0.00" : "#0;[Red]-#0";

                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.Alignment = HorizontalCellAlignment.Right;
                }
            }

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 15 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[4].Height = 20 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            for (int i = 2; i < columnCount; i = i + 1)
            {
                //string formatString = "0";
                //e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = 100 * 37;
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
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Key.TrimEnd(' ');
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
