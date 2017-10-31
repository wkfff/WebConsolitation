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
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color=System.Drawing.Color;
using Font=System.Drawing.Font;
using IList = Infragistics.Documents.Reports.Report.List.IList;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0016_02
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtFactGrid1 = new DataTable();
        private DataTable dtPlanGrid1 = new DataTable();
        private DataTable dtFactGrid2 = new DataTable();
        private DataTable dtPlanGrid2 = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";
        private DateTime currDateTime;

        #region Параметры запроса

        // выбранный район
        private CustomParam selectedRegion;
        // Текущая дата
        private CustomParam periodCurrentDate;
        // Текущий год
        private CustomParam сurrentYear;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.6 - 190);
            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);
            UltraWebGrid1.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid1_ActiveRowChange);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid2.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.6 - 190);
            UltraWebGrid2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);
            UltraWebGrid2.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (сurrentYear == null)
            {
                сurrentYear = UserParams.CustomParam("current_year");
            }

            #endregion

            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid1_DataBound);
            GridSearch1.LinkedGridId = UltraWebGrid1.ClientID;

            UltraWebGrid2.DataBound += new EventHandler(UltraWebGrid2_DataBound);
            
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            CrossLink1.Text = "Исполнение&nbsp;финансирования&nbsp;муниципальных&nbsp;образований";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0016_01/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.EKRDimension.Value = RegionSettingsHelper.Instance.EKRDimension;

            if (!Page.IsPostBack)
            {
                webAsyncPanel.AddRefreshTarget(UltraWebGrid2);
                webAsyncPanel.AddRefreshTarget(mbtCaption);
                webAsyncPanel.AddRefreshTarget(regionCaption);
                webAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid1);

//                ComboPeriod.Width = 300;
//                ComboPeriod.MultiSelect = false;
//                ComboPeriod.ShowSelectedValue = false;
//                ComboPeriod.ParentSelect = false;
//                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMBTNonEmptyDays(DataDictionariesHelper.MBTNonEmptyDays));
//                ComboPeriod.SelectLastNode();
//                ComboPeriod.PanelHeaderTitle = "Выберите дату";

                string query = DataProvider.GetQueryText("FO_0002_0016_02_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

                DateTime date = new DateTime(
                    Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                    CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                    Convert.ToInt32(dtDate.Rows[0][4].ToString()));

                // Инициализируем календарь
                CustomCalendar1.WebCalendar.SelectedDate = date;

                regionCaption.Text = "Костромской области";
            }

            currDateTime = CustomCalendar1.WebCalendar.SelectedDate;
            //currDateTime = GetDateString(ComboPeriod.GetSelectedNodePath(), ComboPeriod.SelectedNode.Level);
            
            Page.Title = "Распределение межбюджетных трансфертов по муниципальным образованиям";
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Межбюджетные трансферты, передаваемые из бюджета субъекта в бюджеты муниципальных образований на {0:dd.MM.yyyy}", currDateTime);

            сurrentYear.Value = currDateTime.Year.ToString();
            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", currDateTime, 5);

            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();

            string patternValue = regionCaption.Text;
            int defaultRowIndex = 1;
            if (patternValue == string.Empty)
            {
                patternValue = selectedRegion.Value;
                defaultRowIndex = 0;
            }
            else
            {
                if (patternValue == "Костромской области")
                {
                    patternValue = "Итого";
                }
            }

            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid1, patternValue, 0, defaultRowIndex);
            ActiveGridRow(row);

            UltraWebGrid2.Bands.Clear();
            UltraWebGrid2.DataBind();
        }

//        public DateTime GetDateString(string source, int level)
//        {
//            string[] sts = source.Split('|');
//            if (sts.Length > 1)
//            {
//                switch (level)
//                {
//                    // нулевой уровень выбрать нельзя
//                    case 1:
//                        {
//                            return GetDateString(ComboPeriod.GetNodeLastChild(ComboPeriod.SelectedNode), level + 1);
//                        }
//                    case 2:
//                        {
//                            string month = sts[1].TrimEnd(' ');
//                            string day = sts[2].TrimEnd(' ');
//                            return new DateTime(Convert.ToInt32(sts[0]), CRHelper.MonthNum(month), Convert.ToInt32(day));
//                        }
//                }
//            }
//            return DateTime.MinValue;
//        }

        #region Обработчики грида 1

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0016_02_grid1_fact");
            dtFactGrid1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Муниципальные образования", dtFactGrid1);

            if (dtFactGrid1.Rows.Count > 0)
            {
                query = DataProvider.GetQueryText("FO_0002_0016_02_grid1_plan");
                dtPlanGrid1 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Муниципальное образование", dtPlanGrid1);
                dtPlanGrid1.PrimaryKey = new DataColumn[] { dtPlanGrid1.Columns[0] };

                foreach (DataRow row in dtFactGrid1.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                    {
                        string rowName = row[0].ToString();
                        DataRow planRow = dtPlanGrid1.Rows.Find(rowName);
                        for (int i = 1; i < dtFactGrid1.Columns.Count; i = i + 3)
                        {
                            string columnName = dtFactGrid1.Columns[i].ColumnName;
                            if (dtPlanGrid1.Rows.Count > 0 && planRow != null &&
                                planRow[columnName] != DBNull.Value && planRow[columnName].ToString() != string.Empty)
                            {
                                double planValue = Convert.ToDouble(planRow[columnName]);
                                row[columnName] = planValue;

                                if (row[i + 1] != DBNull.Value &&
                                    row[i + 1].ToString() != string.Empty)
                                {
                                    double factValue = Convert.ToDouble(row[i + 1]);
                                    row[i + 2] = planValue - factValue;
                                }
                                else
                                {
                                    row[i + 2] = DBNull.Value;
                                }
                            }
                            else
                            {
                                row[columnName] = DBNull.Value;
                                row[i + 2] = DBNull.Value;
                            }
                        }
                    }
                }

                UltraWebGrid1.DataSource = dtFactGrid1;
            }
        }

        protected void UltraWebGrid1_DataBound(object sender, EventArgs e)
        {
            //            UltraWebGrid.Height = Unit.Empty;
            //            UltraWebGrid.Width = Unit.Empty;
        }

        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.Wrap = true;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(115);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
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

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 3)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0];

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Утверждено на год", "План на год");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Профинансировано", "Профинансировано нарастающим итогом с начала года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "Остаток финансирования от плана на год", "");

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 3;
                ch.RowLayoutColumnInfo.SpanX = 3;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
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

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            string region = row.Cells[0].Text;
            
            if (region == "Итого")
            {
                regionCaption.Text = "Костромской области";
                mbtCaption.Text = string.Format("Межбюджетные трансферты, передаваемые муниципальным образованиям ");
                selectedRegion.Value = "Итого ";
            }
            else
            {
                regionCaption.Text = region;
                mbtCaption.Text = string.Format("Межбюджетные трансферты, передаваемые муниципальному образованию ");
                selectedRegion.Value = region;
            }
            
            UltraWebGrid2.Bands.Clear();
            UltraWebGrid2.DataBind();
        }

        protected void UltraWebGrid1_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        #endregion

        #region Обработчики грида 2

        private static DataTable DistinctDTRowValue(DataTable dt, int columnIndex)
        {
            DataTable resDT = dt.Clone();
            resDT.PrimaryKey = new DataColumn[] { resDT.Columns[columnIndex] };

            foreach (DataRow row in dt.Rows)
            {
                if (row[columnIndex] != DBNull.Value && row[columnIndex].ToString() != string.Empty)
                {
                    if (resDT.Rows.Find(row[columnIndex].ToString()) == null)
                    {
                        resDT.ImportRow(row);
                    }
                }
            }

            return resDT;
        }

        protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0016_02_grid2_fact");
            dtFactGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Межбюджетные трансферты", dtFactGrid2);

            if (dtFactGrid2.Rows.Count > 0)
            {
                query = DataProvider.GetQueryText("FO_0002_0016_02_grid2_plan");
                dtPlanGrid2 = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Муниципальное образование", dtPlanGrid2);

                dtPlanGrid2 = DistinctDTRowValue(dtPlanGrid2, 0);

                dtPlanGrid2.PrimaryKey = new DataColumn[] { dtPlanGrid2.Columns[0] };

                foreach (DataRow row in dtFactGrid2.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                    {
                        string rowName = row[0].ToString();
                        DataRow planRow = dtPlanGrid2.Rows.Find(rowName);
                        for (int i = 1; i < dtFactGrid2.Columns.Count - 1; i = i + 3)
                        {
                            string columnName = dtFactGrid2.Columns[i].ColumnName;
                            if (dtPlanGrid2.Rows.Count > 0 && planRow != null &&
                                planRow[columnName] != DBNull.Value && planRow[columnName].ToString() != string.Empty)
                            {
                                double planValue = Convert.ToDouble(planRow[columnName]);
                                row[columnName] = planValue;

                                if (row[i + 1] != DBNull.Value &&
                                    row[i + 1].ToString() != string.Empty)
                                {
                                    double factValue = Convert.ToDouble(row[i + 1]);
                                    row[i + 2] = planValue - factValue;
                                }
                                else
                                {
                                    row[i + 2] = DBNull.Value;
                                }
                            }
                            else
                            {
                                row[columnName] = DBNull.Value;
                                row[i + 2] = DBNull.Value;
                            }
                        }
                    }
                }

                UltraWebGrid2.DataSource = dtFactGrid2;
            }
        }

        protected void UltraWebGrid2_DataBound(object sender, EventArgs e)
        {
            //UltraWebGrid2.Height = Unit.Empty;
//            UltraWebGrid2.Width = Unit.Empty;
        }

        protected void UltraWebGrid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.HeaderStyleDefault.Wrap = true;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(200);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(500);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "Утверждено на год", "План на год");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Профинансировано", "Профинансировано нарастающим итогом с начала года");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "Остаток финансирования от плана на год", "");

            e.Layout.Bands[0].Columns[4].Hidden = true;
        }

        protected void UltraWebGrid2_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool kd = (i == 1);
                int levelColumnIndex = 4;

                if (e.Row.Cells[levelColumnIndex] != null && e.Row.Cells[levelColumnIndex].Value.ToString() != string.Empty)
                {
                    string level = e.Row.Cells[levelColumnIndex].Value.ToString();
                   
                    int fontSize = 8;
                    bool bold = false;
                    switch (level)
                    {
                        case "(All)":
                            {
                                fontSize = 10;
                                bold = true;
                                break;
                            }
                        case "Расходы уровень 2":
                            {
                                bold = true;
                                break;
                            }
                        case "Расходы уровень 4":
                            {
                                bold = false;
                                break;
                            }
                        case "Расходы уровень 5":
                            {
                                bold = false;
                                 if (e.Row.Cells[0].Value != null)
                                 {
                                     e.Row.Cells[0].Value = "  " + e.Row.Cells[0].Value;
                                 }
                                break;
                            }
                    }
                    e.Row.Cells[i].Style.Font.Size = fontSize;
                    e.Row.Cells[i].Style.Font.Bold = bold;
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

        #region Экспорт в Pdf

        private bool titleAdded = false;

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            if (!titleAdded)
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
            else
            {
                IText title = e.Section.AddText();
                Font font = new Font("Verdana", 14);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.Style.Font.Bold = true;
                title.AddContent(mbtCaption.Text + " " + regionCaption.Text);
            }

            titleAdded = true;
        }
        
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            Report report = new Report();
            ReportSection section1 = new ReportSection(report);
            ReportSection section2 = new ReportSection(report);

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid1, section1);
            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid2, section2);
        }

        #endregion

        #region Экспорт в Excel

        private UltraWebGrid exportGrid;

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;

            e.Workbook.Worksheets["Распределение МБТ"].Rows[0].Cells[0].Value = Label1.Text;
            e.Workbook.Worksheets["Распределение МБТ"].Rows[1].Cells[0].Value = Label2.Text;
            e.Workbook.Worksheets["МБТ, передаваемые МО"].Rows[0].Cells[0].Value = Label1.Text;
            e.Workbook.Worksheets["МБТ, передаваемые МО"].Rows[1].Cells[0].Value = Label2.Text;
            e.Workbook.Worksheets["МБТ, передаваемые МО"].Rows[2].Cells[0].Value = mbtCaption.Text + regionCaption.Text;
        }
                
        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = exportGrid.Columns.Count;
            int rowsCount = exportGrid.Rows.Count;
            int headerRowIndex = 3;
            int startNumericColumnIndex = exportGrid == UltraWebGrid1 ? 1 : 2;

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[headerRowIndex].Height = 30 * 37;
                e.CurrentWorksheet.Rows[headerRowIndex].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[headerRowIndex].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[headerRowIndex].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[headerRowIndex + 1].Height = 30 * 37;
                e.CurrentWorksheet.Rows[headerRowIndex + 1].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[headerRowIndex + 1].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[headerRowIndex + 1].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            // расставляем стили у начальных колонок
            e.CurrentWorksheet.Columns[0].Width = exportGrid == UltraWebGrid1 ? 300 * 37 : 500 * 37;
            for (int i = 6; i < rowsCount + 6; i++)
            {
                if (i < 7 && exportGrid == UltraWebGrid2)
                {
                    e.CurrentWorksheet.Rows[i].Height = 20 * 37;
                    e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                    e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
                }
            }

            int width = 110;
            for (int i = startNumericColumnIndex; i < columnCount; i++)
            {
                string formatString = "#,##0;[Red]-#,##0";
                e.CurrentWorksheet.Columns[i].Width = width * 37;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
            }
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            if (exportGrid == UltraWebGrid1)
            {
                e.HeaderText = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Распределение МБТ");
            Worksheet sheet2 = workbook.Worksheets.Add("МБТ, передаваемые МО");

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            exportGrid = UltraWebGrid1;
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(exportGrid, sheet1);

            exportGrid = UltraWebGrid2;
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(exportGrid, sheet2);
        }

        #endregion
    }

    public class ReportSection : ISection
    {
        private readonly ISection section;

        public ReportSection(Report report)
        {
            section = report.AddSection();
            ITable table = section.AddTable();
            ITableRow row = table.AddRow();
            ITableCell titleCell = row.AddCell();
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
            set { this.section.PageSize = new PageSize(1800, 1350); }
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
