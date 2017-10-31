using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Band;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.List;
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

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0016_01
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtFactGrid = new DataTable();
        private DataTable dtPlanGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";
        private DateTime currDateTime;

        #region Параметры запроса

        // выбранный администратор МБТ
        private CustomParam selectedMBT;
        // Текущая дата
        private CustomParam periodCurrentDate;
        // Текущий год
        private CustomParam сurrentYear;
        // Уровень районов
        private CustomParam regionsLevel;
        // Уровень поселений
        private CustomParam settlementLevel;

        #endregion

        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenHeight < 900; }
        }

        private int MinScreenHeight
        {
            get { return IsSmallResolution ? 800 : CustomReportConst.minScreenHeight; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(MinScreenHeight - 280);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            if (selectedMBT == null)
            {
                selectedMBT = UserParams.CustomParam("selected_mbt");
            }
            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (сurrentYear == null)
            {
                сurrentYear = UserParams.CustomParam("current_year");
            }
            regionsLevel = UserParams.CustomParam("regions_level");
            settlementLevel = UserParams.CustomParam("settlement_level");

            #endregion

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            CrossLink1.Text = "Исполнение&nbsp;финансирования&nbsp;муниципальных&nbsp;образований";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0016_02/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.EKRDimension.Value = RegionSettingsHelper.Instance.EKRDimension;

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            settlementLevel.Value = RegionSettingsHelper.Instance.SettlementLevel;

            if (!Page.IsPostBack)
            {
                //                ComboPeriod.Width = 300;
                //                ComboPeriod.MultiSelect = false;
                //                ComboPeriod.ShowSelectedValue = false;
                //                ComboPeriod.ParentSelect = false;
                //                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMBTNonEmptyDays(DataDictionariesHelper.MBTNonEmptyDays));
                //                ComboPeriod.SelectLastNode();
                //                ComboPeriod.PanelHeaderTitle = "Выберите дату";

                string query = DataProvider.GetQueryText("FO_0002_0016_01_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

                DateTime date = new DateTime(
                    Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                    CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                    Convert.ToInt32(dtDate.Rows[0][4].ToString()));

                // Инициализируем календарь
                CustomCalendar1.WebCalendar.SelectedDate = date;

                ComboMBTAdmins.Title = "Ведомство";
                ComboMBTAdmins.Width = 400;
                ComboMBTAdmins.MultiSelect = false;
                ComboMBTAdmins.ParentSelect = false;
                ComboMBTAdmins.FillDictionaryValues(CustomMultiComboDataHelper.FillMBTAdministratorList(DataDictionariesHelper.MBTAdministratorsUniqNames, DataDictionariesHelper.MBTAdministratorsLevels));
                ComboMBTAdmins.SetСheckedState("Департамент финансов  Костромской области", true);
            }

            currDateTime = CustomCalendar1.WebCalendar.SelectedDate;
            //currDateTime = GetDateString(ComboPeriod.GetSelectedNodePath(), ComboPeriod.SelectedNode.Level);

            Page.Title = "Распределение межбюджетных трансфертов по муниципальным образованиям";
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Межбюджетные трансферты, передаваемые из бюджета субъекта в бюджеты муниципальных образований на {0:dd.MM.yyyy}", currDateTime);

            selectedMBT.Value = DataDictionariesHelper.MBTAdministratorsUniqNames[ComboMBTAdmins.SelectedValue];
            сurrentYear.Value = currDateTime.Year.ToString();
            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", currDateTime, 5);

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
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

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0016_01_grid_fact");
            dtFactGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Муниципальные образования", dtFactGrid);

            if (dtFactGrid.Rows.Count > 0)
            {
                query = DataProvider.GetQueryText("FO_0002_0016_01_grid_plan");
                dtPlanGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Муниципальное образование", dtPlanGrid);
                dtPlanGrid.PrimaryKey = new DataColumn[] { dtPlanGrid.Columns[0] };

                foreach (DataRow row in dtFactGrid.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                    {
                        string rowName = row[0].ToString();
                        DataRow planRow = dtPlanGrid.Rows.Find(rowName);
                        for (int i = 1; i < dtFactGrid.Columns.Count; i = i + 3)
                        {
                            string columnName = dtFactGrid.Columns[i].ColumnName;
                            if (dtPlanGrid.Rows.Count > 0 && planRow != null &&
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

                        if (row[0] != DBNull.Value)
                        {
                            row[0] = row[0].ToString().Replace("Муниципальный район", "МР");
                            row[0] = row[0].ToString().Replace("муниципальный район", "МР");
                        }
                    }
                }

                foreach (DataColumn column in dtFactGrid.Columns)
                {
                    column.ColumnName = column.ColumnName.Replace('"', '\'');
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

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
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

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
                {
                    if (e.Row.Cells[0].Value.ToString().Contains("Итого"))
                    {
                        e.Row.Style.Font.Bold = true;
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

                e.CurrentWorksheet.Rows[headerRowIndex + 1].Height = 30 * 37;
                e.CurrentWorksheet.Rows[headerRowIndex + 1].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[headerRowIndex + 1].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[headerRowIndex + 1].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            // расставляем стили у начальных колонок
            e.CurrentWorksheet.Columns[0].Width = 300 * 37;
            for (int i = 5; i < rowsCount + 5; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 10 * 37;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            }

            int width = 110;
            for (int i = 2; i < columnCount; i++)
            {
                string formatString = "#,##0;[Red]-#,##0";
                e.CurrentWorksheet.Columns[i].Width = width * 37;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
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
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        #endregion
    }
}

