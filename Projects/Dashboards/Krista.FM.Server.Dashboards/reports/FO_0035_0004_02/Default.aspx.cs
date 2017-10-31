using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0035_0004_02
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid1 = new DataTable();
        private DataTable dtGrid2 = new DataTable();
        private DataTable dtCommentText = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";

        #region Параметры запроса

        // выбранный бюджет
        private CustomParam selectedBudget;

        #endregion

        // пока для Костромы
        public bool IsKostroma
        {
            get
            {
                return ComboBudget.SelectedValue == "г.Кострома ";
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (selectedBudget == null)
            {
                selectedBudget = UserParams.CustomParam("selected_budget");
            }

            #endregion

            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 190);
            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid2.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 190);
            UltraWebGrid2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid2.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid2.DataBound += new EventHandler(UltraWebGrid_DataBound);
            GridSearch1.LinkedGridId = UltraWebGrid1.ClientID;

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.PdfExportButton.Visible = false;
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
                        
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0035_0004_02_date");
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

                ComboBudget.Title = "Бюджет";
                ComboBudget.Visible = false;
                ComboBudget.Width = 230;
                ComboBudget.MultiSelect = false;
                ComboBudget.FillDictionaryValues(CustomMultiComboDataHelper.FillCashPlanBudgetList());
                ComboBudget.SetСheckedState("г.Кострома ", true);
            }
            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            Page.Title = "Прогноз расходования средств бюджета г. Костромы";
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Ежедневная информация за {0} {1} года ", 
                CRHelper.RusMonth(monthNum).ToLower(), yearNum, ComboBudget.SelectedValue.TrimEnd(' '));

            grid1Label.Text = "Прогноз";
            grid2Label.Text = "Оперативная информация по средствам бюджета";
            MeansDeficit.Text = string.Empty;

            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

            if (ComboBudget.SelectedValue == "Областной бюджет ")
            {
                selectedBudget.Value = "[Районы].[Сопоставимый].[Все районы].[Областной бюджет]";
            }
            else if (ComboBudget.SelectedValue == "г.Кострома ")
            {
                selectedBudget.Value = "[Районы].[Сопоставимый].[Все районы].[ГОРОДСКИЕ ОКРУГА].[г.Кострома]";
            }

            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();

            UltraWebGrid2.Bands.Clear();
            UltraWebGrid2.DataBind();

            CommentTextDataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0035_0004_02_grid1");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Дата", dtGrid1);

            if (dtGrid1.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid1.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length; i++)
                    {
                        if ((i > 7  && i < 21) && row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) / 1000;
                        }
                    }
                }

                UltraWebGrid1.DataSource = dtGrid1;
            }
        }

        protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0035_0004_02_grid2");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Дата", dtGrid2);

            if (dtGrid2.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid2.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length; i++)
                    {
                        if ((i > 7 && i < 21) && row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) / 1000;
                        }
                    }
                }

                UltraWebGrid2.DataSource = dtGrid2;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            ((UltraWebGrid)sender).Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            int percentColumnIndex = 5;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString =  i == percentColumnIndex ? "P2" : "N1";
                int widthColumn = 110;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 60;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            //e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].MergeCells = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1 || i == e.Layout.Bands[0].Columns.Count - 1 || i == e.Layout.Bands[0].Columns.Count - 2)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }

                if (IsKostroma && (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("Федеральные средства на счетах в ДФ")))
                {
                    e.Layout.Bands[0].Columns[i].Header.Caption = "Средства других бюджетов на счетах ФКУ";
                }
            }

            if (IsKostroma)
            {

            }

            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = "Доходы месяца";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 2;
            ch.RowLayoutColumnInfo.SpanX = 6;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);

            ch = new ColumnHeader(true);
            ch.Caption = "Фактически произведенные расходы";
            ch.RowLayoutColumnInfo.OriginY = 0;
            ch.RowLayoutColumnInfo.OriginX = 8;
            ch.RowLayoutColumnInfo.SpanX = e.Layout.Bands[0].Columns.Count - 8;
            ch.Style.Wrap = true;
            e.Layout.Bands[0].HeaderLayout.Add(ch);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell nameCell = e.Row.Cells[0];
                UltraGridCell cell = e.Row.Cells[i];

                if (nameCell.Value != null && nameCell.Value.ToString() != string.Empty)
                {
                    bool percentRow = nameCell.Value.ToString().Contains("%");
                    bool remainsRow = nameCell.Value.ToString().ToLower().Contains("остаток");
                    bool totalRow = nameCell.Value.ToString().ToLower().Contains("итого");
                    bool monthEndRemainsRow = nameCell.Value.ToString().ToLower().Contains("на конец месяца");
                    bool totalColumn = UltraWebGrid1.Bands[0].Columns[i].Header.Caption.ToString().ToLower().Contains("итого");
                    bool lastColumn = i == e.Row.Cells.Count - 1;

                    if (i == 0 && (percentRow || remainsRow))
                    {
                        cell.ColSpan = 8;
                    }

                    if (i != 0 && percentRow && cell.Value != null && cell.Value.ToString() != string.Empty)
                    {
                        cell.Value = (Convert.ToDouble(cell.Value) * 1000).ToString("P2");
                    }

                    if (totalRow || totalColumn)
                    {
                        cell.Style.Font.Bold = true;
                    }

                    if (monthEndRemainsRow && lastColumn)
                    {
//                        cell.Value = "Дефицит средств для проведения расходов по доведенным лимитам";
//                        cell.Style.Wrap = true;
//                        cell.Style.Font.Bold = false;
//                        cell.Style.HorizontalAlign = HorizontalAlign.Center;
//                        cell.Style.VerticalAlign = VerticalAlign.Middle;
                        //cell.RowSpan = 5;

                        if (cell.Value != null && cell.Value.ToString() != string.Empty)
                        {
                            double deficitValue = Convert.ToDouble(cell.Value.ToString());
                            MeansDeficit.Text = string.Format("Дефицит средств для проведения расходов по доведенным лимитам: <b>{0:N1}</b> тыс.руб.", deficitValue);
                            cell.Value = string.Empty;
                        }
                    }
                }
                
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

        #region Комментарий

        private void CommentTextDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0035_0004_02_commentText");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Дата", dtCommentText);

            if (dtCommentText.Rows.Count > 0)
            {
                double totalRemains = GetDoubleDTValue(dtCommentText, "Общий остаток средств на счете ");
                double distributionRemains = GetDoubleDTValue(dtCommentText, "Остаток к распределению ");
                double stateRemains = GetDoubleDTValue(dtCommentText, "ФС (заявлено) ");

                string totalRemainsStr = totalRemains == double.MinValue
                                             ? string.Empty
                                             : string.Format("Общий остаток средств на счете: <b>{0:N0}</b> тыс.руб.", totalRemains);

                string distributionRemainsStr = totalRemains == double.MinValue
                             ? string.Empty
                             : string.Format("Остаток к распределению: <b>{0:N0}</b> тыс.руб.", distributionRemains);

                string stateRemainsStr = totalRemains == double.MinValue
                             ? string.Empty
                             : string.Format("ФС (заявлено): <b>{0:N0}</b> тыс.руб.", stateRemains);

                CommentText.Text = String.Format("{0}&nbsp;&nbsp;{1}&nbsp;&nbsp;{2}", totalRemainsStr, distributionRemainsStr, stateRemainsStr);
            }
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, double.MinValue);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
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
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1);

            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid2);
        }

        #endregion

        #region Экспорт в Excel

        private static string CommentTextExportsReplaces(string source)
        {
            string commentText = source;

            commentText = commentText.Replace("&nbsp;", " ");
            commentText = commentText.Replace("<br/>", "\n");
            commentText = commentText.Replace("<b>", "");
            commentText = commentText.Replace("</b>", "");
            return commentText;
        }
        
        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.Workbook.Worksheets["Прогноз"].Rows[0].Cells[0].Value = Label1.Text;
            e.Workbook.Worksheets["Прогноз"].Rows[1].Cells[0].Value = Label2.Text;

            e.Workbook.Worksheets["Оперативные данные"].Rows[0].Cells[0].Value = Label1.Text;
            e.Workbook.Worksheets["Оперативные данные"].Rows[1].Cells[0].Value = Label2.Text;
            e.Workbook.Worksheets["Оперативные данные"].Rows[2].Cells[0].Value = CommentTextExportsReplaces(CommentText.Text);
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = exportGrid.Columns.Count;
            int rowsCount = exportGrid.Rows.Count;

            e.CurrentWorksheet.Columns[0].Width = 60 * 37;

            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount + 4; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 10 * 37;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;

                if (e.CurrentWorksheet.Name == "Прогноз" &&
                    (i >= 4 + (rowsCount - 5)))
                {
                    e.CurrentWorksheet.MergedCellsRegions.Add(i, 0, i, 7);
                    e.CurrentWorksheet.Rows[i].Height = 10 * 37;
                    e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                }
            }

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 12 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[4].Height = 17 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            int width = 110;

            for (int i = 1; i < columnCount; i++)
            {
                string format = "#,#0.0;[Red]-#,#0.0";
                if (i == 5)
                {
                    format = UltraGridExporter.ExelPercentFormat;
                }
                e.CurrentWorksheet.Columns[i].Width = width * 37;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = format;
            }

            if (e.CurrentWorksheet.Name == "Прогноз")
            {
                e.CurrentWorksheet.Rows[rowsCount + 5].Cells[0].Value = CommentTextExportsReplaces(MeansDeficit.Text);
            }
//            else
//            {
//                e.CurrentWorksheet.Rows[rowsCount + 5].Cells[0].Value = CommentTextExportsReplaces(CommentText.Text);
//            }
        }

        private UltraWebGrid exportGrid = new UltraWebGrid();

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Прогноз");
            Worksheet sheet2 = workbook.Worksheets.Add("Оперативные данные");

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            exportGrid = UltraWebGrid1;
            UltraGridExporter1.ExcelExporter.Export(exportGrid, sheet1);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            exportGrid = UltraWebGrid2;
            UltraGridExporter1.ExcelExporter.Export(exportGrid, sheet2);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            if (e.CurrentColumnIndex > 1 && e.CurrentColumnIndex < 8)
            {
                e.HeaderText = "Доходы месяца";
            }
            else if (e.CurrentColumnIndex > 7 && e.CurrentColumnIndex < 20)
            {
                e.HeaderText = "Фактически произведенные расходы";
            }
            else
            {
                e.HeaderText = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
            }
        }

        #endregion
    }
}
