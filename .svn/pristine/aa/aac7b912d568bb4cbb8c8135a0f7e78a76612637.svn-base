using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0015_03
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtFactGrid = new DataTable();
        private DataTable dtPlanGrid = new DataTable();
        private DataTable dtKBK = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 320);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            CrossLink1.Text = "Распределение&nbsp;межбюджетных&nbsp;трансфертов&nbsp;по&nbsp;муниципальным&nbsp;образованиям";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0015_02/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.EKRDimension.Value = RegionSettingsHelper.Instance.EKRDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0015_03_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);
            }
            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            Page.Title = "Исполнение финансирования муниципальных образований";
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Межбюджетные трансферты, передаваемые из бюджета субъекта в бюджеты муниципальных образований за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0015_03_grid_fact");
            dtFactGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtFactGrid);

            dtKBK = new DataTable();
            dtKBK = dtFactGrid.Clone();
            dtKBK.ImportRow(dtFactGrid.Rows[0]);
            dtKBK.AcceptChanges();

            dtFactGrid.Rows.RemoveAt(0);

            if (dtFactGrid.Rows.Count > 0)
            {
                query = DataProvider.GetQueryText("FO_0002_0015_03_grid_plan");
                dtPlanGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Муниципальное образование", dtPlanGrid);
                dtPlanGrid.PrimaryKey = new DataColumn[] { dtPlanGrid.Columns[0] };

                foreach (DataRow row in dtFactGrid.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                    {
                        string rowName = row[0].ToString();
                        DataRow planRow = dtPlanGrid.Rows.Find(rowName);
                        for (int i = 1; i < dtFactGrid.Columns.Count; i = i + 2)
                        {
                            string columnName = dtFactGrid.Columns[i].ColumnName;
                            if (dtPlanGrid.Rows.Count > 0 && dtPlanGrid.Columns.Contains(columnName) && 
                                planRow != null && planRow[columnName] != DBNull.Value && planRow[columnName].ToString() != string.Empty)
                            {
                                double planValue = Convert.ToDouble(planRow[columnName]);
                                row[columnName] = planValue;
                            }
                            else
                            {
                                row[columnName] = DBNull.Value;
                            }
                        }

                        if (row[0].ToString().Contains("Бюджеты муниципальных образований Оренбургской области"))
                        {
                            row[0] = "Итого";
                        }
                    }

                    for (int i = 1; i < row.ItemArray.Length; i++)
                    {
                        if (row[i] != DBNull.Value && row[i].ToString() != string.Empty)
                        {
                            row[i] = Convert.ToDouble(row[i]) / 1000;
                        }
                    }
                }

                foreach (DataColumn column in dtFactGrid.Columns)
                {
                    column.ColumnName = column.ColumnName.Replace("\"", "'");
                }

                UltraWebGrid.DataSource = dtFactGrid;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            //            UltraWebGrid.Height = Unit.Empty;
            //            UltraWebGrid.Width = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N2";
                int widthColumn = 90;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 )
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

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];

                ch.Caption = ch.Caption.Replace(
                    "Итого",
                    "ИТОГО");
                ch.Caption = ch.Caption.Replace(
                    "Дотации бюджетам субъектов Российской Федерации и муниципальных образований",
                    "ДОТАЦИИ БЮДЖЕТАМ СУБЪЕКТОВ РОССИЙСКОЙ ФЕДЕРАЦИИ И МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ");
                ch.Caption = ch.Caption.Replace(
                    "Субсидии бюджетам субъектов Российской Федерации и муниципальных образований (межбюджетные субсидии)",
                    "СУБСИДИИ БЮДЖЕТАМ СУБЪЕКТОВ РОССИЙСКОЙ ФЕДЕРАЦИИ И МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ (МЕЖБЮДЖЕТНЫЕ СУБСИДИИ)");
                ch.Caption = ch.Caption.Replace(
                    "Субвенции бюджетам субъектов Российской Федерации и муниципальных образований",
                    "СУБВЕНЦИИ БЮДЖЕТАМ СУБЪЕКТОВ РОССИЙСКОЙ ФЕДЕРАЦИИ И МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ");
                ch.Caption = ch.Caption.Replace(
                    "Иные межбюджетные трансферты",
                    "ИНЫЕ МЕЖБЮДЖЕТНЫЕ ТРАНСФЕРТЫ");

                if (i != 1 && dtKBK != null && dtKBK.Rows[0][i] != DBNull.Value && dtKBK.Rows[0][i].ToString() != string.Empty)
                {
                    ch.Caption = string.Format("{0}<br>({1})", ch.Caption, Convert.ToDouble(dtKBK.Rows[0][i]).ToString("#### ####### ###"));
                    ch.Key = string.Format("{0}<br>({1})", ch.Key, Convert.ToDouble(dtKBK.Rows[0][i]).ToString("#### ####### ###"));
                }

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "План", "План на год");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Факт", "Факт нарастающим итогом с начала года");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
            //Response.Write(UltraWebGrid.Columns.Count);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells.Count > 0 && e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
            {
                if (e.Row.Cells[0].Value.ToString().Contains("Итого"))
                {
                    e.Row.Style.Font.Bold = true;
                }
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
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

        private static void GridExportsReplaces(UltraWebGrid grid)
        {
            foreach (UltraGridColumn column in grid.Columns)
            {
                column.Header.Caption = column.Header.Caption.Replace("<br/>", "\n");
            }
        }

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;
            int headerRowIndex = 3;

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[headerRowIndex].Height = 30 * 37;
                e.CurrentWorksheet.Rows[headerRowIndex].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[headerRowIndex].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[headerRowIndex].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            // расставляем стили у начальных колонок
            e.CurrentWorksheet.Columns[0].Width = 300 * 37;

            int width = 110;
            for (int i = 1; i < columnCount; i++)
            {
                string formatString = "#,##0;[Red]-#,##0";
                e.CurrentWorksheet.Columns[i].Width = width * 37;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
            }
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            string header = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
            if (dtKBK != null && dtKBK.Rows.Count > 0 &&
                dtKBK.Rows[0][e.CurrentColumnIndex] != DBNull.Value &&
                dtKBK.Rows[0][e.CurrentColumnIndex].ToString() != string.Empty &&
                dtKBK.Rows[0][e.CurrentColumnIndex].ToString() != "0")
            {
                try
                {
                    e.HeaderText = string.Format("{0}<br>({1})", header, Convert.ToDouble(dtKBK.Rows[0][e.CurrentColumnIndex]).ToString("#### ####### ###"));
                }
                catch
                {
                }
            }
            else
            {
                e.HeaderText = header;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            GridExportsReplaces(UltraWebGrid);
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);
        }
        
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            GridExportsReplaces(UltraWebGrid);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        #endregion

    }
}
