using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0012
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2000;
        private int endYear = 2012;

        #endregion

        #region Параметры запроса

        // группа КД
        private CustomParam kdGroupName;
        // выбранный регион
        private CustomParam selectedRegion;
        // Тип документа
        private CustomParam documentType;
        // Тип документа для районов
        private CustomParam regionDocumentType;
        // Консолидированный уровень
        private CustomParam consolidateLevel;
        // Уровень бюджета СКИФ
        private CustomParam budgetSKIFLevel;
        // фильтр по годам
        private CustomParam filterYear;
        // Последний год
        private CustomParam lastYear;
        // Последний год
        private CustomParam periodMonth;

        // Доходы-Всего
        private CustomParam incomesTotal;

        // уровень МР и ГО
        private CustomParam regionsLevel;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 150);

            #region Инициализация параметров запроса

            if (kdGroupName == null)
            {
                kdGroupName = UserParams.CustomParam("kd_group_name");
            }
            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (documentType == null)
            {
                documentType = UserParams.CustomParam("document_type");
            }
            if (regionDocumentType == null)
            {
                regionDocumentType = UserParams.CustomParam("region_document_type");
            }
            if (consolidateLevel == null)
            {
                consolidateLevel = UserParams.CustomParam("consolidate_level");
            }
            if (budgetSKIFLevel == null)
            {
                budgetSKIFLevel = UserParams.CustomParam("budget_SKIF_level");
            }
            if (filterYear == null)
            {
                filterYear = UserParams.CustomParam("filter_year");
            }
            if (lastYear == null)
            {
                lastYear = UserParams.CustomParam("last_year");
            }
            if (periodMonth == null)
            {
                periodMonth = UserParams.CustomParam("period_month");
            }

            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }

            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            #endregion

            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.HeaderCellExporting += new EventHandler<MarginCellExportingEventArgs>(PdfExporter_HeaderCellExporting);

            UltraWebGrid.EnableViewState = false;
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11402000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11402000000000000;
            UserParams.IncomesKD11403000000000410.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000410;
            UserParams.IncomesKD11403000000000440.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000440 != "null"
                    ? string.Format(",[КД].[Сопоставимый].[Все коды доходов].[{1}].[ДОХОДЫ ОТ ПРОДАЖИ МАТЕРИАЛЬНЫХ И НЕМАТЕРИАЛЬНЫХ АКТИВОВ].[{0}],",
                                    RegionSettingsHelper.Instance.IncomesKD11403000000000440,
                                    UserParams.IncomesKDRootName.Value)
                    : ",";
            UserParams.IncomesKD11406000000000430.Value = RegionSettingsHelper.Instance.IncomesKD11406000000000430;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKD30000000000000000.Value = RegionSettingsHelper.Instance.IncomesKD30000000000000000;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0012_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);

                ComboKD.Width = 420;
                ComboKD.Title = "Вид дохода";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillFullKDIncludingList());
                ComboKD.SetСheckedState("Доходы бюджета - Итого ", true);
            }
                        
            
            selectedRegion.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            documentType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            regionDocumentType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");
            kdGroupName.Value = ComboKD.SelectedValue;
            periodMonth.Value = MakePeriodMonth(ComboYear.SelectedValue, ComboMonth.SelectedValue);

            consolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            Page.Title = string.Format("Поступление доходов в бюджеты районов и поселений: {0}", ComboKD.SelectedValue);
            PageTitle.Text = Page.Title;

            PageSubTitle.Text = string.Format("Оценка исполнения бюджетов за {0} месяцев {1} года.",
                                                ComboMonth.SelectedIndex + 1, ComboYear.SelectedValue);
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0012_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length; i++)
                    {
                        if ((i == 1 || i == 2 || i == 4 || i == 5 || i == 7 || i == 8 || i == 13 || i == 14)
                            && row[i] != DBNull.Value && row[i].ToString() != String.Empty)
                        {
                            row[i] = Convert.ToDouble(row[i]) / 1000;
                        }
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
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
                string formatString;
                int widthColumn;
                switch(i)
                {
                    case 1:
                    case 2:
                    case 4:
                    case 5:
                    case 7:
                    case 8:
                    case 13:
                    case 14:
                        {
                            formatString = "N2";
                            widthColumn = 90;
                            break;
                        }
                    case 3:
                    case 6:
                    case 9:
                    case 10:
                    case 15:
                    case 16:
                        {
                            formatString = "P2";
                            widthColumn = 75;
                            break;
                        }
                    default:
                        {
                            formatString = "N0";
                            widthColumn = 75;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 200;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[12].Hidden = true;
            e.Layout.Bands[0].Columns[18].Hidden = true;
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 3;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 2;
                }
            }

            string year = ComboYear.SelectedValue;
            int monthCount = ComboMonth.SelectedIndex + 1;
            string months = CRHelper.RusManyMonthGenitive(monthCount);

            CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "Консолидированный бюджет МО", 1, 0, 3, 2);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, 
                    "Утверждено на год, тыс.руб.", String.Format("Утвержденные годовые назначения на {0} год", year));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "Исполнено, тыс.руб.", String.Format("Поступления за {0} {1} {2} года", monthCount, months, year));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "% исполнения", String.Format("«% выполнения годовых назначений за {0} {1} {2} года", monthCount, months, year));

            CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "В том числе:", 4, 0, 13, 1);

            CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "Бюджет городских округов", 4, 1, 3, 1);
            CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "Бюджет районов", 7, 1, 5, 1);
            CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "Бюджет поселений", 12, 1, 5, 1);

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "Утверждено на год, тыс.руб.", String.Format("Утвержденные годовые назначения на {0} год", year));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "Исполнено, тыс.руб.", String.Format("Поступления за {0} {1} {2} года", monthCount, months, year));
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "% исполнения", String.Format("«% выполнения годовых назначений за {0} {1} {2} года", monthCount, months, year));

            for (int i = 7; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 6)
            {
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "Утверждено на год, тыс.руб.", String.Format("Утвержденные годовые назначения на {0} год", year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Исполнено, тыс.руб.", String.Format("Поступления за {0} {1} {2} года", monthCount, months, year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "% исполнения", String.Format("«% выполнения годовых назначений за {0} {1} {2} года", monthCount, months, year));
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "Удельный вес", "Удельный вес поступлений в бюджет в консолидированном бюджете МО");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, "Ранг", "Ранг (место) МО по удельному весу поступлений в бюджет в консолидированном бюджете МО");
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            SetRankImg(e.Row, 11);
            SetRankImg(e.Row, 17);

            for (int i = 0; i < e.Row.Cells.Count - 1; i++)
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

                    if (e.Row.Cells[0].Value != null &&
                        (e.Row.Cells[0].Value.ToString().ToLower().Contains("итого")))
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }
        }

        private static void SetRankImg(UltraGridRow row, int i)
        {
            string css = string.Empty;
            if (Convert.ToInt32(row.Cells[i].Value) == 1)
            {
                css = "~/images/starYellowBB.png";
            }
            else if (UglyRank(row, i))
            {
                css =  "~/images/starGrayBB.png";
            }
            if (css != string.Empty)
            {
                row.Cells[i].Style.BackgroundImage = css;
                row.Cells[i].Title = (css == "~/images/starGrayBB.png")
                   ? "Самый маленький удельный вес"
                   : "Самый большой удельный вес";
            }
            row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
        }
        
        private static bool UglyRank(UltraGridRow row, int i)
        {
            return row.Cells[i] != null && row.Cells[i + 1] != null &&
                   Convert.ToInt32(row.Cells[i].Value) == Convert.ToInt32(row.Cells[i + 1].Value) &&
                   Convert.ToInt32(row.Cells[i].Value) != 0;
        }

        private string MakePeriodMonth(string year, string month)
        {
            int monthNum = CRHelper.MonthNum(month);
            int halfYear = CRHelper.HalfYearNumByMonthNum(monthNum);
            int quater = CRHelper.QuarterNumByMonthNum(monthNum);
            return String.Format("[{0}].[Полугодие {1}].[Квартал {2}].[{3}]", year, halfYear, quater, month);
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;

            UltraGridRow newRow = new UltraGridRow();

            foreach (UltraGridColumn col in e.Rows.Band.Columns)
            {
                AddCell(newRow, col.Header.Caption);
            }
            e.Rows.Insert(0, newRow, true);

            newRow = new UltraGridRow();

            AddCell(newRow, "Наименование МО");
            AddCell(newRow, "Консолидированный бюджет МО");
            AddCell(newRow, "Консолидированный бюджет МО");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    string caption = string.Empty;
                    switch (i)
                    {
                        case 0:
                            {
                                caption = "Бюджет городских округов";
                                j = j + 2;
                                break;
                            }
                        case 1:
                            {
                                caption = "Бюджет районов";
                                break;
                            }
                        case 2:
                            {
                                caption = "Бюджет поселений";
                                break;
                            }
                    }
                    AddCell(newRow, caption);
                }
            }
            e.Rows.Insert(0, newRow, true);
        }

        private static void AddCell(UltraGridRow newRow, string caption)
        {
            UltraGridCell cell = new UltraGridCell(caption);
            cell.Style.HorizontalAlign = HorizontalAlign.Left;
            newRow.Cells.Add(cell);
            cell.Style.BackColor = System.Drawing.Color.FromArgb(211, 211, 211);
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.CurrentWorksheet.MergedCellsRegions.Add(3, 0, 5, 0);
            e.CurrentWorksheet.MergedCellsRegions.Add(3, 1, 5, 3);
            e.CurrentWorksheet.MergedCellsRegions.Add(3, 4, 3, 16);

            e.CurrentWorksheet.MergedCellsRegions.Add(4, 4, 4, 6);
            e.CurrentWorksheet.MergedCellsRegions.Add(4, 7, 4, 11);
            e.CurrentWorksheet.MergedCellsRegions.Add(4, 12, 4, 16);

            for (int i = 1; i < 17; i = i + 1)
            {
                string formatString;
                int widthColumn;
                switch (i)
                {
                    case 1:
                    case 2:
                    case 4:
                    case 5:
                    case 7:
                    case 8:
                    case 12:
                    case 13:
                        {
                            formatString = "#,##0.00";
                            widthColumn = 90;
                            break;
                        }
                    case 3:
                    case 6:
                    case 9:
                    case 10:
                    case 14:
                    case 15:
                        {
                            formatString = UltraGridExporter.ExelPercentFormat; ;
                            widthColumn = 75;
                            break;
                        }
                    default:
                        {
                            formatString = "#,##0";
                            widthColumn = 75;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
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
            if (e.CurrentColumnIndex > 15)
                return;

            UltraGridColumn col = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex];

            if (e.CurrentColumnIndex == 0)
            {
                e.HeaderText = "Наименование МО";
            }
            else if (e.CurrentColumnIndex == 1 ||
                     e.CurrentColumnIndex == 2)
            {
                e.HeaderText = "Консолидированный бюджет МО";
            }
            else
            {
                e.HeaderText = "В том числе:";
            }
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

        private Collection<float> cellsWidth = new Collection<float>();
        private Collection<string> cellsCaption = new Collection<string>();
        // private Collection<string> cellsOriginsCaption = new Collection<string>();

        private void PdfExporter_HeaderCellExporting(object sender, MarginCellExportingEventArgs e)
        {
            // сохраняем информацию о родных ячейках
            cellsWidth.Add(((FixedWidth)e.ReportCell.Width).Value);
            cellsCaption.Add(e.ExportValue.ToString());

            // и скрываем их
            e.ReportCell.Height = new FixedHeight(0);

            if (e.Column.Index < 17)
                return;

            // Вся информация собрана, начинаем формировать заголовок.

            // Добавляем строку
            ITableRow r = e.ReportCell.Parent.Parent.AddRow();
            // Добавляем ячейку под селектор
            UltraGridExporter.AddSelectorCell(e, r);
            // Добавляем ячейку под хидеры
            ITableCell c = r.AddCell();
            // Добавляем таблицу
            ITable table = c.AddTable();
            // добавлем строку
            ITableRow mainHeaderRow = table.AddRow();

            // Поехали добавлять
            int columnIndex = 0;

            while (columnIndex < cellsWidth.Count - 1)
            {
                if (columnIndex < 1)
                {
                    // Лепим один уровень
                    ITableCell headerCell = mainHeaderRow.AddCell();
                    headerCell.Width = new FixedWidth(cellsWidth[columnIndex]);

                    table = headerCell.AddTable();
                    ITableRow headerRow = table.AddRow();
                    headerCell = headerRow.AddCell();
                    headerCell.Height = new FixedHeight(125);
                    UltraGridExporter.SetCellStyle(e, headerCell);
                    IText t = headerCell.AddText();
                    UltraGridExporter.SetFontStyle(t);
                    t.AddContent(cellsCaption[columnIndex]);
                    columnIndex++;
                }
                if (columnIndex < 4)
                {
                    // лепим 2 уровня
                    ITableCell headerCell = mainHeaderRow.AddCell();
                    float width = cellsWidth[columnIndex] + cellsWidth[columnIndex + 1] + cellsWidth[columnIndex + 2];
                    headerCell.Width = new FixedWidth(width);

                    table = headerCell.AddTable();
                    ITableRow headerRow = table.AddRow();
                    headerCell = headerRow.AddCell();
                    headerCell.Height = new FixedHeight(90);
                    UltraGridExporter.SetCellStyle(e, headerCell);
                    IText t = headerCell.AddText();
                    UltraGridExporter.SetFontStyle(t);
                    t.AddContent("Консолидированный бюджет МО");
                    headerRow = table.AddRow();
                    for (int i = 0; i < 3; i++)
                    {
                        headerCell = headerRow.AddCell();
                        headerCell.Height = new FixedHeight(35);
                        headerCell.Width = new FixedWidth(cellsWidth[columnIndex]);
                        UltraGridExporter.SetCellStyle(e, headerCell);
                        t = headerCell.AddText();
                        UltraGridExporter.SetFontStyle(t);
                        t.AddContent(cellsCaption[columnIndex]);
                        columnIndex++;
                    }
                }
                else
                {
                    // лепим три уровня
                    ITableCell headerContainerCell = mainHeaderRow.AddCell();
                    float cellWidth = 0;

                    table = headerContainerCell.AddTable();
                    ITableRow headerRow = table.AddRow();
                    ITableCell headerCell = headerRow.AddCell();
                    headerCell.Height = new FixedHeight(40);
                    UltraGridExporter.SetCellStyle(e, headerCell);
                    IText t = headerCell.AddText();
                    UltraGridExporter.SetFontStyle(t);
                    t.AddContent("В том числе:");

                    headerRow = table.AddRow();
                    ITableRow headerChildRow = table.AddRow();

                    headerCell = headerRow.AddCell();
                    headerCell.Height = new FixedHeight(50);
                    
                    float width = cellsWidth[columnIndex] +
                                  cellsWidth[columnIndex + 1] +
                                  cellsWidth[columnIndex + 2];

                    headerCell.Width = new FixedWidth(width);
                    
                    UltraGridExporter.SetCellStyle(e, headerCell);
                    t = headerCell.AddText();
                    UltraGridExporter.SetFontStyle(t);
                    t.AddContent("Бюджет городских округов");
                    AddHeaderCellKind("Бюджет городских округов", columnIndex, e, headerRow, 0);

                    for (int j = 0; j < 3; j++)
                    {
                        headerCell = headerChildRow.AddCell();
                        headerCell.Height = new FixedHeight(35);
                        headerCell.Width = new FixedWidth(cellsWidth[columnIndex]);
                        UltraGridExporter.SetCellStyle(e, headerCell);
                        t = headerCell.AddText();
                        UltraGridExporter.SetFontStyle(t);
                        t.AddContent(cellsCaption[columnIndex]);
                        cellWidth += cellsWidth[columnIndex];
                        columnIndex++;
                    }
                    headerContainerCell.Width = new FixedWidth(cellWidth);

                    for (int i = 0; i < 2; i++)
                    {
                        string caption = string.Empty;
                        switch (i)
                        {
                            case 0:
                                {
                                    caption = "Бюджет районов";
                                    break;
                                }
                            case 1:
                                {
                                    caption = "Бюджет поселений";
                                    break;
                                }
                        }
                        AddHeaderCellKind(caption, columnIndex, e, headerRow, i);

                        for (int j = 0; j < 5; j++)
                        {
                            headerCell = headerChildRow.AddCell();
                            headerCell.Height = new FixedHeight(35);
                            headerCell.Width = new FixedWidth(cellsWidth[columnIndex]);
                            UltraGridExporter.SetCellStyle(e, headerCell);
                            t = headerCell.AddText();
                            UltraGridExporter.SetFontStyle(t);
                            t.AddContent(cellsCaption[columnIndex]);
                            cellWidth += cellsWidth[columnIndex];
                            columnIndex++;
                        }
                        headerContainerCell.Width = new FixedWidth(cellWidth);
                    }
                }
            }
        }

        private void AddHeaderCellKind(string caption, int columnIndex, MarginCellExportingEventArgs e, ITableRow headerRow, int i)
        {
            ITableCell headerCell = headerRow.AddCell();
            headerCell.Height = new FixedHeight(50);
            if (cellsWidth.Count > columnIndex + i + 2)
            {
                float width = cellsWidth[columnIndex] +
                              cellsWidth[columnIndex + 1] +
                              cellsWidth[columnIndex + 2] +
                              cellsWidth[columnIndex + 3] +
                              cellsWidth[columnIndex + 4];

                headerCell.Width = new FixedWidth(width);
            }
            UltraGridExporter.SetCellStyle(e, headerCell);
            IText t = headerCell.AddText();
            UltraGridExporter.SetFontStyle(t);
            t.AddContent(caption);
        }

        #endregion
    }
}
