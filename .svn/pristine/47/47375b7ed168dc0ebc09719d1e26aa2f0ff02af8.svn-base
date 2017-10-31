using System;
using System.Collections;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
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
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color=System.Drawing.Color;
using IList = Infragistics.Documents.Reports.Report.List.IList;

namespace Krista.FM.Server.Dashboards.reports.FO_0039_0008
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2010;
        private int endYear = 2011;
        private int selectedQuarterIndex;
        private int groupQuarterCount;

        #endregion

        private bool IsYearCompare
        {
            get { return selectedQuarterIndex == 4; }
        }

        private bool AreEmptyGrids
        {
            get { return !(increaseTD.Visible && decreaseTD.Visible && unchangedTD.Visible); }
        }

        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
        // выбранный предыдущий период
        private CustomParam selectedPrevPeriod;
        // множество районов
        private CustomParam regionSet;
        // множество периодов
        private CustomParam periodSet;
        // уровень районов
        private CustomParam regionsLevel;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }
            if (selectedPrevPeriod == null)
            {
                selectedPrevPeriod = UserParams.CustomParam("selected_prev_period");
            }
            if (regionSet == null)
            {
                regionSet = UserParams.CustomParam("region_set");
            }
            if (periodSet == null)
            {
                periodSet = UserParams.CustomParam("period_set");
            }
            regionsLevel = UserParams.CustomParam("regions_level");

            #endregion

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты оценки качества МР";
            CrossLink1.NavigateUrl = "~/reports/FO_0039_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Рейтинг&nbsp;МР";
            CrossLink2.NavigateUrl = "~/reports/FO_0039_0003/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Сравн.характеристика&nbsp;мин.&nbsp;и&nbsp;макс.&nbsp;оценок";
            CrossLink3.NavigateUrl = "~/reports/FO_0039_0004/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Картограмма";
            CrossLink4.NavigateUrl = "~/reports/FO_0039_0005/Default.aspx";

            CrossLink5.Visible = true;
            CrossLink5.Text = "Результаты&nbsp;оценки&nbsp;по&nbsp;отд.показателю";
            CrossLink5.NavigateUrl = "~/reports/FO_0039_0006/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0039_0008_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Оценка качества";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillDateQuarters());
                ComboQuarter.RemoveTreeNodeByName("по состоянию на 01.04");
                ComboQuarter.SetСheckedState(GetParamQuarter(quarter), true);
            }

            selectedQuarterIndex = ComboQuarter.SelectedIndex + 2;
            

            Page.Title = String.Format("Динамика рейтинга муниципальных районов Омской области по результатам оценки качества");
            PageTitle.Text = Page.Title;

            PageSubTitle.Text = (!IsYearCompare)
                ? String.Format("{0}.{1}", ComboQuarter.SelectedValue, ComboYear.SelectedValue)
                : String.Format("по итогам {0} года", ComboYear.SelectedValue);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", selectedQuarterIndex);

            int prevYear = Convert.ToInt32(UserParams.PeriodYear.Value.ToString()) - 1;
            string prevQuarter = String.Format("Квартал {0}", selectedQuarterIndex - 1);
            string prevHalfYear = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex - 1));

            if (IsYearCompare)
            {
                periodSet.Value = "Годы";
                selectedPeriod.Value = String.Format("[{0}]", UserParams.PeriodYear.Value);
                selectedPrevPeriod.Value = String.Format("[{0}]", prevYear);
                groupQuarterCount = 3;
            }
            else
            {
                periodSet.Value = "Кварталы";
                selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);
                selectedPrevPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, prevHalfYear, prevQuarter);
                groupQuarterCount = selectedQuarterIndex + 1;
            }

            // при выбранном 4м квартале и для первого года в списке параметра данных не выводим
            if (!(IsYearCompare && firstYear.ToString() == ComboYear.SelectedValue))
            {
                SetGridParams(IncreaseRatingGrid);
                SetGridParams(DecreaseRatingGrid);
                SetGridParams(UnChangedRatingGrid);

                increaseGridCaption.Text = "Перечень районов с улучшением рейтинга";
                decreaseGridCaption.Text = "Перечень районов со снижением рейтинга";
                unchangedGridCaption.Text = "Перечень районов с сохранением рейтинга";
                
                regionSet.Value = "Районы с увеличением рейтинга";
                IncreaseRatingGrid.DataBind();
                regionSet.Value = "Районы со снижением рейтинга";
                DecreaseRatingGrid.DataBind();
                regionSet.Value = "Районы со сохранением рейтинга";
                UnChangedRatingGrid.DataBind();
            }
            else
            {
                regionSet.Value = "Пустое множество районов";
                IncreaseRatingGrid.DataBind();
                DecreaseRatingGrid.DataBind();
                UnChangedRatingGrid.DataBind();
            }

            string emptyGridMessage = (IsYearCompare && firstYear.ToString() == ComboYear.SelectedValue)
                                          ? "Расчет динамики невозможен, т.к. нет данных по оценке качества за предыдущий финансовый год"
                                          : "Нет данных";
            EmptyGridText.Text = (AreEmptyGrids) ? emptyGridMessage : String.Empty;
            EmptyGridText.Font.Name = "Verdana";
            EmptyGridText.Font.Bold = false;
            EmptyGridText.Font.Size = 12;
        }

        private static void SetGridParams(UltraWebGrid grid)
        {
            Unit width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth / 2 - 20);
            grid.Width = width;
            grid.DisplayLayout.NoDataMessage = "Нет данных";
            grid.DisplayLayout.AllowRowNumberingDefault = RowNumbering.Continuous;
            grid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            grid.Bands.Clear();
        }

        /// <summary>
        /// Получить элемент параметра по значению классификатора
        /// </summary>
        /// <param name="classQuarter">элемент классификатора</param>
        /// <returns>значение параметра</returns>
        private static string GetParamQuarter(string classQuarter)
        {
            switch (classQuarter)
            {
                case "Квартал 1":
                    {
                        return "по состоянию на 01.04";
                    }
                case "Квартал 2":
                    {
                        return "по состоянию на 01.07";
                    }
                case "Квартал 3":
                    {
                        return "по состоянию на 01.10";
                    }
                case "Квартал 4":
                case "Данные года":
                    {
                        return "по итогам года";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        #region Обработчики грида

        private static void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            ((UltraWebGrid)sender).Height = Unit.Empty; 
            ((UltraWebGrid)sender).Width = Unit.Empty;
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            UltraWebGrid grid = ((UltraWebGrid) sender);

            string query = DataProvider.GetQueryText("FO_0039_0008_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                SetGridTDVisible(grid, true);

                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("муниципальный район", "МР");
                    }
                }

                grid.DataSource = dtGrid;
            }
            else
            {
                SetGridTDVisible(grid, false);

                grid.DataSource = null;
            }
        }

        private void SetGridTDVisible(UltraWebGrid grid, bool visible)
        {
            if (grid == IncreaseRatingGrid)
            {
                increaseTD.Visible = visible;
            }
            else if (grid == DecreaseRatingGrid)
            {
                decreaseTD.Visible = visible;
            }
            else
            {
                unchangedTD.Visible = visible;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.RowStyleDefault.BackColor = Color.LightSkyBlue;

            if (e.Layout.Bands[0].Columns.Count <= 0)
            {
                return;
            }

            if (e.Layout.Grid == IncreaseRatingGrid)
            {
                e.Layout.RowStyleDefault.BackColor = Color.LightGreen;
            }
            else if (e.Layout.Grid == DecreaseRatingGrid)
            {
                e.Layout.RowStyleDefault.BackColor = Color.LightCoral;
            }
            else
            {
                e.Layout.RowStyleDefault.BackColor = Color.Khaki;
            }
            
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, e.Layout.Bands[0].Columns[1].Header.Caption.Split(';')[0], "");

            int beginIndex = 1;

            for (int i = beginIndex; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 90;

                if (groupQuarterCount > 1 && i != e.Layout.Bands[0].Columns.Count - 1)
                {
                    widthColumn = 315 / (groupQuarterCount - 1);
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == e.Layout.Bands[0].Columns.Count - 1)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            for (int i = 1; i < groupQuarterCount; i++)
            {
                string columnHeader = e.Layout.Grid.Columns[i].Header.Caption;
                columnHeader = (IsYearCompare)
                    ? String.Format("по итогам {0} года", columnHeader)
                    : String.Format("{0}.{1}", GetParamQuarter(columnHeader), ComboYear.SelectedValue);

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, columnHeader, "");
            }

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, e.Layout.Bands[0].Columns.Count - 1,
                String.Format("Отклонение от прошлого периода", ComboYear.SelectedValue), "");

            CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "Рейтинговое место по итогам оценки качества", 1, 0, groupQuarterCount - 1, 1);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
//            for (int i = 0; i < e.Row.Cells.Count; i++)
//            {
//                UltraGridCell cell = e.Row.Cells[i];
//                if (i == e.Row.Cells.Count - 1 && cell.Value != null && cell.Value.ToString() != String.Empty)
//                {
//                    decimal value;
//                    if (decimal.TryParse(cell.Value.ToString(), out value))
//                    {
//                        if (value > 0)
//                        {
//                            cell.Value = String.Format("+ {0:N0}", cell.Value);
//                        }
//                    }
//                }
//            }
        }

        #endregion

        #region Экспорт в Excel

        private int beginExportRowIndex = 4;

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = groupQuarterCount + 1;

            e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "";
            e.CurrentWorksheet.Columns[0].Width = 150 * 37;
            
            // расставляем стили у ячеек хидера
            for (int i = 0; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[beginExportRowIndex - 1].Height = 18 * 37;
                e.CurrentWorksheet.Rows[beginExportRowIndex - 1].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[beginExportRowIndex - 1].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[beginExportRowIndex - 1].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[beginExportRowIndex].Height = 18 * 37;
                e.CurrentWorksheet.Rows[beginExportRowIndex].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[beginExportRowIndex].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[beginExportRowIndex].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            for (int i = 1; i < columnCount; i = i + 1)
            {
                string formatString = "#,##0";

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = 120*37;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            if (IncreaseRatingGrid.DataSource != null)
            {
                Worksheet sheet = workbook.Worksheets.Add("Районы с увеличением рейтинга");
                UltraGridExporter1.ExcelExporter.ExcelStartRow = beginExportRowIndex;
                UltraGridExporter1.ExcelExporter.Export(IncreaseRatingGrid, sheet);
            }
            if (DecreaseRatingGrid.DataSource != null)
            {
                Worksheet sheet = workbook.Worksheets.Add("Районы со снижением рейтинга");
                UltraGridExporter1.ExcelExporter.ExcelStartRow = beginExportRowIndex;
                UltraGridExporter1.ExcelExporter.Export(DecreaseRatingGrid, sheet);
            }
            if (UnChangedRatingGrid.DataSource != null)
            {
                Worksheet sheet = workbook.Worksheets.Add("Районы со сохранением рейтинга");
                UltraGridExporter1.ExcelExporter.ExcelStartRow = beginExportRowIndex;
                UltraGridExporter1.ExcelExporter.Export(UnChangedRatingGrid, sheet);
            }
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            int columnCount = groupQuarterCount + 1;
            if (e.CurrentColumnIndex != 0 && e.CurrentColumnIndex != columnCount - 1)
            {
                e.HeaderText = "Рейтинговое место по итогам оценки качества";
            }
        }

        #endregion

        #region Экспорт в PDF

        private bool titleAdded = false;
        private string exportGridCaption;
        private ReportSection exportSection;
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            Report report = new Report();
            exportSection = new ReportSection(report, true);
            exportSection.AddFlowColumns(500, 500);

            if (IncreaseRatingGrid.DataSource != null)
            {
                exportGridCaption = increaseGridCaption.Text;
                UltraGridExporter1.PdfExporter.Export(IncreaseRatingGrid, exportSection);
                exportSection.AddFlowColumnBreak();
            }
                        
            if (DecreaseRatingGrid.DataSource != null)
            {
                exportGridCaption = decreaseGridCaption.Text;
                UltraGridExporter1.PdfExporter.Export(DecreaseRatingGrid, exportSection);
            } 
            
            if (UnChangedRatingGrid.DataSource != null)
            {
                exportGridCaption = unchangedGridCaption.Text;
                UltraGridExporter1.PdfExporter.Export(UnChangedRatingGrid, exportSection);
            }
        }
        
        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            if (!titleAdded)
            {
                IText title = exportSection.AddTitleText();
                System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
                title.Style.Font = new Font(font);
                title.Style.Font.Bold = true;
                title.AddContent(PageTitle.Text);

                title = exportSection.AddTitleText();
                font = new System.Drawing.Font("Verdana", 14);
                title.Style.Font = new Font(font);
                title.AddContent(PageSubTitle.Text);

                titleAdded = true;
            }

            IText gridTitle = e.Section.AddText();
            System.Drawing.Font gridTitleFont = new System.Drawing.Font("Verdana", 14);
            gridTitle.Style.Font = new Font(gridTitleFont);
            gridTitle.Style.Font.Bold = true;
            gridTitle.AddContent(exportGridCaption);
        }

        #endregion
    }

    public class ReportSection : ISection
    {
        private readonly bool withFlowColumns;
        private readonly ISection section;
        private IFlow flow;
        private ITableCell titleCell;

        public ReportSection(Report report, bool withFlowColumns)
        {
            this.withFlowColumns = withFlowColumns;
            section = report.AddSection();
            ITable table = section.AddTable();
            ITableRow row = table.AddRow();
            titleCell = row.AddCell();
        }

        public void AddFlowColumns(float leftColumnWidth, float rightColumnWidth)
        {
            if (withFlowColumns)
            {
                flow = section.AddFlow();
                IFlowColumn col = flow.AddColumn();
                col.Width = new FixedWidth(leftColumnWidth);
                col = flow.AddColumn();
                col.Width = new FixedWidth(rightColumnWidth);
            }
        }

        public void AddFlowColumnBreak()
        {
            if (flow != null)
                flow.AddColumnBreak();
        }

        public IBand AddBand()
        {
            if (flow != null)
                return flow.AddBand();
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

        public IText AddTitleText()
        {
            return titleCell.AddText();
        }

        public IText AddText()
        {
            if (flow != null)
                return flow.AddText();
            return section.AddText();
        }

        public IImage AddImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            if (flow != null)
                return flow.AddImage(image);
            return section.AddImage(image);
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
            if (flow != null)
                return flow.AddTable();
            return section.AddTable();
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
            get { return section.PageSize; }
            set { section.PageSize = new PageSize(1200, 1600); }
        }

        public PageOrientation PageOrientation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ContentAlignment PageAlignment
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Borders PageBorders
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Margins PageMargins
        {
            get { return section.PageMargins; }
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
            get { return section.Parent; }
        }

        public IEnumerable Content
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
