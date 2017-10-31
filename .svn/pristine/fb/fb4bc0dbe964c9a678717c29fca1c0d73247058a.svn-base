using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = System.Drawing.Font;

namespace Krista.FM.Server.Dashboards.reports.STAT_0001_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private DateTime currDateTime;
        private DateTime lastDateTime;

        #endregion

        #region Параметры запроса

        // Текущая дата
        private CustomParam periodCurrentDate;
        // На неделю назад
        private CustomParam periodLastWeekDate;
        // Текущий год
        private CustomParam сurrentYear;
        // Прошлый год
        private CustomParam lastYear;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 120);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);

            #region Инициализация параметров запроса

            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (periodLastWeekDate == null)
            {
                periodLastWeekDate = UserParams.CustomParam("period_last_week_date");
            }
            if (сurrentYear == null)
            {
                сurrentYear = UserParams.CustomParam("current_year");
            }
            if (lastYear == null)
            {
                lastYear = UserParams.CustomParam("last_year");
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
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                ComboPeriod.Width = 300;
                ComboPeriod.MultiSelect = false;
                ComboPeriod.ShowSelectedValue = false;
                ComboPeriod.ParentSelect = false;
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillLabourMarketNonEmptyDays(DataDictionariesHelper.LabourMarketNonEmptyDays));
                ComboPeriod.SelectLastNode();
                ComboPeriod.PanelHeaderTitle = "Выберите дату";
            }

            currDateTime = GetDateString(ComboPeriod.GetSelectedNodePath(), ComboPeriod.SelectedNode.Level);
            //lastDateTime = currDateTime.AddDays(-7);

            Node selectedNode = ComboPeriod.SelectedNode;
            // если выбран месяц, то берем в нем последний день
            if (selectedNode.Nodes.Count != 0)
            {
                selectedNode = ComboPeriod.GetLastChild(selectedNode);
            }

            lastDateTime = GetDateString(ComboPeriod.GetPreviousSublingNodePath(selectedNode), selectedNode.Level);
            if (lastDateTime == DateTime.MinValue)
            {
                lastDateTime = currDateTime.AddDays(-7);
            }

            сurrentYear.Value = currDateTime.Year.ToString();
            lastYear.Value = (currDateTime.Year - 1).ToString();

            Page.Title = "Мониторинг ситуации на рынке труда";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format(@"Форма мониторинга ситуации на рынке труда в субъектах Российской Федерации, входящих в Уральский федеральный округ, по состоянию на {0:dd.MM.yyyy} (по данным органов государственной власти субъектов Российской Федерации)", currDateTime);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", currDateTime, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", lastDateTime, 5);

            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
        }

        public DateTime GetDateString(string source, int level)
        {
            string[] sts = source.Split('|');
            if (sts.Length > 1)
            {
                switch (level)
                {
                    // нулевой уровень выбрать нельзя
                    case 1:
                        {
                            return GetDateString(ComboPeriod.GetNodeLastChild(ComboPeriod.SelectedNode), level + 1);
                        }
                    case 2:
                        {
                            string month = sts[1].TrimEnd(' ');
                            string day = sts[2].TrimEnd(' ');
                            return new DateTime(Convert.ToInt32(sts[0]), CRHelper.MonthNum(month), Convert.ToInt32(day));
                        }
                }
            }
            return DateTime.MinValue;
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0001_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                DataColumn numberColumn = new DataColumn("№", typeof(string));
                dtGrid.Columns.Add(numberColumn);

                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString().Split(';').Length > 0)
                    {
                        row[0] = row[0].ToString().Split(';')[0];
                        row["№"] = DataDictionariesHelper.GetLabourMarketIndicatorNumber(row[0].ToString());

                        if (row[0].ToString().Contains("АППГ"))
                        {
                            row[0] = string.Format("Аналогичный период прошлого года ({0} г.)", currDateTime.Year - 1);
                        }
                    }
                }

                ((UltraWebGrid)sender).DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid1.Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.RowAlternateStyleDefault.BackColor = Color.White;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            // перемещаем колонку с номером в начало
            UltraGridColumn numberColumn = e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1];
            e.Layout.Bands[0].Columns.RemoveAt(e.Layout.Bands[0].Columns.Count - 1);
            e.Layout.Bands[0].Columns.Insert(0, numberColumn);

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                int widthColumn = i < e.Layout.Bands[0].Columns.Count - 2 ? 63 : 65;

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(20);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(210);
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].MergeCells = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
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

            int multiHeaderPos = 2;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, string.Format("{0:dd.MM}", lastDateTime), "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, string.Format("{0:dd.MM}", currDateTime), "");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 2;
                ch.RowLayoutColumnInfo.SpanX = 2;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellsCount = e.Row.Cells.Count;

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                int rowIndex = e.Row.Index;

                // номер показателя
                int k = (rowIndex % 3);

                // яркая колонка
                bool bright = (i % 2 != 0);

                if (i != 0 && bright)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 2;
                }
                else
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthLeft = 2;
                }

                if (i > 1)
                {
                    switch (k)
                    {
                        case 0:
                            {
                                e.Row.Cells[i].Style.BorderDetails.WidthBottom = 0;
                                break;
                            }
                        case 1:
                            {
                                e.Row.Cells[i].Style.BorderDetails.WidthTop = 0;
                                e.Row.Cells[i].Style.BorderDetails.WidthBottom = 0;
                                break;
                            }
                        case 2:
                            {
                                e.Row.Cells[i].Style.BorderDetails.WidthTop = 0;
                                break;
                            }
                    }
                }

                if (i != 0 && i != 1 && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    double value = Convert.ToDouble(e.Row.Cells[i].Value);

                    bool growRate = (k == 1);
                    bool rate = (k == 2);
                    bool mlnUnit = (e.Row.Cells[0].Value != null && e.Row.Cells[1].Value.ToString().Contains("млн"));

                    if (growRate)
                    {
                        e.Row.Cells[i].Title = "Темп прироста";
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = bright ? "~/images/arrowRedUpBB.png" : "~/images/arrowRedUpBBdim.png";
                            }
                            else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = bright ? "~/images/arrowGreenDownBB.png" : "~/images/arrowGreenDownBBdim.png";
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }

                    if (rate)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                            {
                                e.Row.Cells[i].Title = "Прирост к прошлой неделе";
                            }
                            else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                            {
                                e.Row.Cells[i].Title = "Падение относительно прошлой недели";
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }

                    switch (k)
                    {
                        case 0:
                            {
                                e.Row.Cells[i].Style.Font.Bold = true;
                                e.Row.Cells[i].Value = mlnUnit ? string.Format("{0:N3}", value) : string.Format("{0:N0}", value);
                                break;
                            }
                        case 1:
                            {
                                e.Row.Cells[i].Value = string.Format("{0:P2}", value);
                                break;
                            }
                        case 2:
                            {
                                e.Row.Cells[i].Value = mlnUnit ? string.Format("{0:N3}", value) : string.Format("{0:N0}", value);
                                break;
                            }
                    }
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    string cellValue = cell.Value.ToString();
                    if (cellValue.Contains("%"))
                    {
                        cellValue = cellValue.TrimEnd('%');
                    }

                    decimal value;
                    if (i != 0 && (k == 1 || k == 2) && decimal.TryParse(cellValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out value))
                    {
                        if (value > 0)
                        {
                            cell.Value = string.Format("+{0}", cell.Value);
                        }
                    }

                    e.Row.Cells[i].Style.Padding.Right = (i == 0) ? 1 : 5;

                    if (i >= cellsCount - 2)
                    {
                        cell.Style.Font.Bold = true;
                    }

                    if (i != 0 && !bright)
                    {
                         cell.Style.ForeColor = Color.DimGray;
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
            int columnCount = UltraWebGrid1.Columns.Count;
            int rowsCount = UltraWebGrid1.Rows.Count;

            e.CurrentWorksheet.Columns[0].Width = 20 * 37;
            e.CurrentWorksheet.Columns[1].Width = 200 * 37;

            for (int i = 2; i < columnCount; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 60 * 37;
            }
            
            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount + 4; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            }

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF

        private static void SetExportGridParams(UltraWebGrid grid)
        {
            bool unit4 = false;
            foreach (UltraGridRow row in grid.Rows)
            {
                // номер показателя
                int k = (row.Index % 3);
                UltraGridCell nameCell = row.Cells[1];
                UltraGridCell numberCell = row.Cells[0];
                switch(k)
                {
                    case 0:
                        {
                            if (!unit4 && numberCell.Value != null && numberCell.Value.ToString() == "4")
                            {
                                unit4 = true;
                            }
                            else
                            {
                                numberCell.Value = "";
                            }
                            numberCell.Style.BorderDetails.WidthBottom = 0;
                            nameCell.Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                    case 1:
                        {
                            if (numberCell.Value != null && numberCell.Value.ToString() == "4")
                            {
                                numberCell.Value = "";
                            }
                            nameCell.Value = "темп пророста";
                            nameCell.Style.HorizontalAlign = HorizontalAlign.Right;
                            numberCell.Style.BorderDetails.WidthBottom = 0;
                            numberCell.Style.BorderDetails.WidthTop = 0;
                            nameCell.Style.BorderDetails.WidthBottom = 0;
                            nameCell.Style.BorderDetails.WidthTop = 0;
                            break;
                        }
                    case 2:
                        {
                            if (numberCell.Value != null && numberCell.Value.ToString() == "4" &&
                                row.NextRow != null && row.NextRow.Cells[0].Value != null && 
                                row.NextRow.Cells[0].Value.ToString() == "4")
                            {
                                numberCell.Style.BorderDetails.WidthBottom = 0;
                            }
                            numberCell.Value = "";
                            nameCell.Value = "прирост";
                            nameCell.Style.HorizontalAlign = HorizontalAlign.Right;
                            numberCell.Style.BorderDetails.WidthTop = 0;
                            nameCell.Style.BorderDetails.WidthTop = 0;
                            break;
                        }
                }
            }
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            SetExportGridParams(UltraWebGrid1);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = (i == 1) ?  CRHelper.GetColumnWidth(300) : CRHelper.GetColumnWidth(60);
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

        #endregion
    }
}
