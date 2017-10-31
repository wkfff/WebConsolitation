using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.SGM.SGM_0003
{
    public partial class sgm_0003 : CustomReportPage
    {
        // Коды заболеваний включаемые в данный отчет
        const string deseasesCodes = "1,4,8,13,18,20;1;4;8;13;18;20";
        string selectedName,selectedFO, months;
        string year;
        readonly CalloutAnnotation Annotation1 = new CalloutAnnotation();
        readonly CalloutAnnotation Annotation2 = new CalloutAnnotation();
        readonly CalloutAnnotation Annotation3 = new CalloutAnnotation();
        readonly Collection<string> groupNames = new Collection<string>();
        readonly Collection<string> groupTooltips = new Collection<string>();
        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMPdfExportHelper exportClass = new SGMPdfExportHelper();
        DataTable dtFullData;
        int lastYear;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            grid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 45);
            grid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 650);

            if (grid.Width != Unit.Empty)
            {
                LabelTitle.Width = (Unit)(grid.Width.Value - 150);
                LabelSubTitle.Width = LabelTitle.Width;
            }
            chartGroup.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth) / 2 - 30;
            chartGroup.Height = 300;
            chartSubject.Width = chartGroup.Width;
            chartSubject.Height = chartGroup.Height;

            #region Настройка диаграмм

            chartSubject.ChartType = ChartType.StackColumnChart;
            chartSubject.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            chartSubject.Axis.Y.Labels.Visible = true;
            chartSubject.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>%";
            chartSubject.Axis.X.Extent = 100;
            chartSubject.Axis.Y.Extent = 25;
            chartSubject.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N2>%";
            CRHelper.FillCustomColorModel(chartSubject, 6, false);
            chartSubject.InvalidDataReceived += CRHelper.UltraChartInvalidDataReceived;

            chartGroup.ChartType = ChartType.DoughnutChart;
            chartGroup.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N2>%";
            chartGroup.DoughnutChart.Concentric = true;
            chartGroup.DoughnutChart.OthersCategoryPercent = 0;
            chartGroup.DoughnutChart.ShowConcentricLegend = false;
            chartGroup.Data.SwapRowsAndColumns = true;
            chartGroup.Legend.Visible = true;
            chartGroup.Legend.Location = LegendLocation.Left;
            chartGroup.Legend.SpanPercentage = 33;
            chartGroup.Legend.Margins.Bottom = 0;
            chartGroup.Legend.Margins.Top = 0;
            chartGroup.Legend.Margins.Left = 0;
            chartGroup.Legend.Margins.Right = 0;
            CRHelper.CopyCustomColorModel(chartSubject, chartGroup);
            chartGroup.InvalidDataReceived += CRHelper.UltraChartInvalidDataReceived;
            if (chartGroup.Height.Value != 0)
            {
                chartGroup.Legend.Margins.Bottom = Convert.ToInt32(chartGroup.Height.Value * (1 - 150 / chartGroup.Height.Value));
            }

            Annotation1.Text = "Всего";
            Annotation1.Width = 85;
            Annotation1.Height = 15;
            Annotation1.Location.Type = LocationType.Percentage;
            Annotation1.Location.LocationX = 64;
            Annotation1.Location.LocationY = 10;

            Annotation2.Text = "Дети";
            Annotation2.Width = 85;
            Annotation2.Height = 15;
            Annotation2.Location.Type = LocationType.Percentage;
            Annotation2.Location.LocationX = 64;
            Annotation2.Location.LocationY = 20;

            Annotation3.Text = "Взрослые";
            Annotation3.Width = 85;
            Annotation3.Height = 15;
            Annotation3.Location.Type = LocationType.Percentage;
            Annotation3.Location.LocationX = 64;
            Annotation3.Location.LocationY = 30;

            chartGroup.Annotations.Add(Annotation1);
            chartGroup.Annotations.Add(Annotation2);
            chartGroup.Annotations.Add(Annotation3);

            #endregion

            SetExportHandlers();
        }

        private void FillGroupLists()
        {
            groupNames.Clear();
            groupNames.Add("Брюшной тиф");
            groupNames.Add("Другие сальмонеллезные инфекции");
            groupNames.Add("Бактериальная дизентерия (шигеллез)");
            groupNames.Add("ОКИ установленные");
            groupNames.Add("ОКИ неустановленные");
            groupNames.Add("Острый гепатит А");

            groupTooltips.Clear();
            groupTooltips.Add("Брюшной тиф");
            groupTooltips.Add("Другие сальмонеллезные инфекции");
            groupTooltips.Add("Бактериальная дизентерия (шигеллез)");
            groupTooltips.Add("Другие острые кишечные инфекции, вызванные установленными бактериальными, вирусными возбудителями, а также пищевые токсикоинфекции установленной этиологии");
            groupTooltips.Add("Острые кишечные инфекции, вызванные неустановленными инфекционными возбудителями, пищевые токсикоинфекции неустановленной этиологии");
            groupTooltips.Add("Острый гепатит А");
        }

        private void FillReportParams()
        {
            year = ComboYear.SelectedValuesString;
            var yearList = year.Split(',');
            lastYear = Convert.ToInt32(yearList[yearList.Length - 1]);
            months = dataRotator.GetMonthParamString(ComboMonth, lastYear.ToString());
            dataRotator.CheckFormNumber(lastYear, ref months);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            dataObject.reportFormRotator = dataRotator;
            dataRotator.CheckSubjectReport();
            base.Page_Load(sender, e);
            dataRotator.formNumber = 1;
            dataObject.InitObject();

            dataRotator.FillRegionsDictionary(dataObject.dtAreaShort);
            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(lbSubject);
                chartWebAsyncPanel.AddRefreshTarget(chartGroup);
                chartWebAsyncPanel.AddRefreshTarget(chartSubject);
                chartWebAsyncPanel.AddLinkedRequestTrigger(grid);
                gridWebAsyncPanel.AddRefreshTarget(grid);

                dataRotator.FillYearList(ComboYear);
                dataRotator.FillMonthListEx(ComboMonth, ComboYear.SelectedValue);
                supportClass.FillPeopleGroupList(ComboPeopleGroup);
                dataRotator.FillSGMMapList(ComboMap, dataObject.dtAreaShort, false);
            }
            else
            {
                dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, false);                
            }

            FillReportParams();
            FillGroupLists();
            if (ViewState["SGM_0003_FULLDATA"] != null && chartWebAsyncPanel.IsAsyncPostBack)
            {
                dtFullData = (DataTable)ViewState["SGM_0003_FULLDATA"];
            }
            else
            {
                FillCommonData();
                ViewState["SGM_0003_FULLDATA"] = dtFullData;
            }

            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                selectedName = supportClass.GetFOShortName(dataRotator.mapList[0]);
                selectedFO = string.Empty;
                FillSubjectGrid();
                FillSubjectChart();
                FillGroupChart();

                Page.Title = string.Format("Структура заболеваемости кишечными инфекциями");
                LabelTitle.Text = Page.Title;
                
                LabelSubTitle.Text = string.Format("По данным за {0} {1} год{3} по группе населения {2}{4}",
                    dataRotator.GetMonthParamLabel(ComboMonth, year),
                    ComboYear.SelectedValuesString, ComboPeopleGroup.SelectedValue, dataRotator.GetYearAppendix(),
                    dataRotator.GetFormHeader());
            }
        }

        #region Обработчики диаграмм

        private string GetAreaFilter()
        {
            string filterStr = string.Empty;
            if (selectedName != dataObject.GetRootMapName())
            {
                if (selectedFO == "Cell")
                {
                    filterStr = string.Format("{0} = '{1}' or {2} = ''",
                        dtFullData.Columns[0].ColumnName, 
                        dataObject.GetRootMapName(), 
                        dtFullData.Columns[1].ColumnName);
                }
                else
                {
                    filterStr = string.Format("{0} = '{1}' or {0} = '{3}' or {2} = '{3}'",
                        dtFullData.Columns[0].ColumnName, 
                        dataObject.GetRootMapName(), 
                        dtFullData.Columns[1].ColumnName, 
                        selectedFO);
                }
            }
            return filterStr;
        }

        private int GetColumnIndex()
        {
            int columnIndex = 0;
            if (ComboPeopleGroup.SelectedIndex == 1) columnIndex = 3;
            if (ComboPeopleGroup.SelectedIndex == 2) columnIndex = 1;
            if (ComboPeopleGroup.SelectedIndex == 3) columnIndex = 2;
            return columnIndex;
        }

        private void FillSubjectGrid()
        {
            var dtGrid = new DataTable();
            DataColumn dataColumn = dtGrid.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            dataColumn = dtGrid.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            for (int i = 0; i < 18; i++)
            {
                dtGrid.Columns.Add(string.Format("DataColumn{0}", i), typeof(double));
            }

            string filterStr = GetAreaFilter();
            int columnIndex = GetColumnIndex();

            DataRow[] drsSelect = dtFullData.Select(filterStr);
            for (int i = 0; i < drsSelect.Length; i++)
            {
                DataRow drGrid = dtGrid.Rows.Add();
                drGrid[0] = drsSelect[i][0];
                drGrid[1] = drsSelect[i][1];
                for (int j = 0; j < 18; j++)
                {
                    drGrid[j + 2] = drsSelect[i][columnIndex * 18 + j + 2];
                }
            }

            dataRotator.FillMaxAndMin(dtGrid, 3, true);
            grid.DataSource = dtGrid;
            grid.DataBind();
        }

        private void FillCommonData()
        {
            dataObject.ClearParams();
            dataObject.InitObject();
            dataObject.useLongNames = true;
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctFO, "0");
            string[] diesCodes = deseasesCodes.Split(';');

            for (int k = 0; k < 4; k++)
            {
                string columnList = string.Empty;
                for (int j = 0; j < 6; j++)
                {
                    columnList = string.Format("{0},{1}", columnList, k * 18 + j * 3 + 3);
                }
                columnList = columnList.TrimStart(',');

                for (int i = 1; i < diesCodes.Length; i++)
                {
                    dataObject.AddColumn(
                        SGMDataObject.DependentColumnType.dctAbs,
                        year,
                        months,
                        string.Empty,
                        k.ToString(),
                        diesCodes[i]);

                    dataObject.AddColumn(
                        SGMDataObject.DependentColumnType.dctRelation,
                        Convert.ToString(k * 18 + i * 3 - 1));

                    dataObject.AddColumn(
                        SGMDataObject.DependentColumnType.dctPercentFromTotal,
                        columnList, Convert.ToString(k * 18 + i * 3));
                }
            }
            dtFullData = dataObject.FillData();
        }

        private void FillSubjectChart()
        {
            var dtResult = new DataTable();

            DataColumn dataColumn = dtResult.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            for (int i = 0; i < 6; i++)
            {
                dtResult.Columns.Add(groupNames[i], typeof(double));
            }

            int columnIndex = GetColumnIndex();
            string filterStr = GetAreaFilter();
            string orderStr = string.Empty;
            if (selectedName == dataObject.GetRootMapName())
            {
                filterStr = string.Format("{0} = '' and {1} <> '{2}'",
                dtFullData.Columns[1].ColumnName,
                dtFullData.Columns[0].ColumnName,
                dataObject.GetRootMapName());
            }
            else
            {
                filterStr = string.Format("({0}) and {1} <> '{2}'", 
                    filterStr, 
                    dtFullData.Columns[0].ColumnName,
                    dataObject.GetRootMapName());
                orderStr = string.Format("{0} asc, {1} asc",
                    dtFullData.Columns[1].ColumnName, dtFullData.Columns[0].ColumnName);
            }
            DataRow[] drsSelectRF = dtFullData.Select(string.Format("{0} = '{1}'", 
                dtFullData.Columns[0].ColumnName,
                dataObject.GetRootMapName()));
            DataRow[] drsSelect = dtFullData.Select(filterStr, orderStr);

            DataRow drChart = dtResult.Rows.Add();
            drChart[0] = drsSelectRF[0][0];
            for (int j = 0; j < 6; j++)
            {
                drChart[j + 1] = drsSelectRF[0][columnIndex * 18 + j * 3 + 4];
            }
            for (int i = 0; i < drsSelect.Length; i++)
            {
                drChart = dtResult.Rows.Add();
                drChart[0] = drsSelect[i][0];
                for (int j = 0; j < 6; j++)
                {
                    drChart[j + 1] = drsSelect[i][columnIndex * 18 + j * 3 + 4];
                }
            }
            chartSubject.DataSource = dtResult;
            chartSubject.DataBind();
        }

        private void FillGroupChart()
        {
            var dtResult = new DataTable();

            var groups = new Dictionary<int, string> {{0, "Всего"}, {1, "Дети"}, {3, "Взрослые"}, {2, "Подростки"}};

            DataColumn dataColumn = dtResult.Columns.Add();
            dataColumn.DataType = Type.GetType("System.String");
            for (int i = 0; i < 6; i++)
            {
                dtResult.Columns.Add(groupNames[i], typeof(double));
            }
            string filterStr = string.Format("{0} = '{1}'", dtFullData.Columns[0].ColumnName, selectedName);
            DataRow[] drsSelect = dtFullData.Select(filterStr);
            foreach (int index in groups.Keys)
            {
                if (index != 2)
                {
                    DataRow drChart = dtResult.Rows.Add();
                    drChart[0] = groups[index];
                    for (int j = 0; j < 6; j++)
                    {
                        drChart[j + 1] = drsSelect[0][index * 18 + j * 3 + 4];
                    }
                }
            }

            chartGroup.DataSource = dtResult;
            chartGroup.DataBind();
        }

        #endregion

        #region Обработчики грида

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[1].ToString() == "Cell")
            {
                e.Row.Style.Font.Bold = true;
                return;
            }
            for (int i = 0; i < 6; i++)
            {
                int imageCellIndex = 3 + i * 3;
                supportClass.SetCellImageStar(e.Row, dataRotator.maxValues, dataRotator.minValues, imageCellIndex, imageCellIndex, imageCellIndex);
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            string foCaption = "ФО";
            if (dataRotator.isSubjectReport) foCaption = "ТО";

            e.Layout.Bands[0].Columns[0].Header.Caption = "Территория";
            e.Layout.Bands[0].Columns[0].Width = 160;
            e.Layout.Bands[0].Columns[1].Header.Caption = foCaption;
            e.Layout.Bands[0].Columns[1].Width = 50;

            ColumnsCollection colCollection = e.Layout.Bands[0].Columns;
            for (int i = 2; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = 76;
                CRHelper.FormatNumberColumn(grid.Columns[i], "N0");
            }
            CRHelper.FormatNumberColumn(grid.Columns[04], "N2");
            for (int i = 0; i < 6; i++)
            {
                CRHelper.FormatNumberColumn(grid.Columns[3 + i * 3], "N2");
                CRHelper.FormatNumberColumn(grid.Columns[4 + i * 3], "N2");
            }
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1)
                {
                    colCollection[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    colCollection[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    colCollection[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }
            for (int i = 0; i < 6; i++)
            {
                CRHelper.SetHeaderCaption(grid, 0, 2 + i * 3, "Абс.", "Абсолютное количество заболевших");
                CRHelper.SetHeaderCaption(grid, 0, 3 + i * 3, "На 100 тыс.", "Относительное количество заболевших на 100 тысяч населения территории");
                CRHelper.SetHeaderCaption(grid, 0, 4 + i * 3, "% от суммы всех кишечных инфекций", "Доля заболевания от общего числа всех кишечных инфекций");
            }

            for (int i = 0; i < 6; i++)
            {
                var ch = new ColumnHeader(true)
                             {
                                 Caption = groupNames[i],
                                 RowLayoutColumnInfo =
                                     {
                                         OriginY = 0,
                                         OriginX = 2 + i*3,
                                         SpanX = 3
                                     },
                                 Title = groupTooltips[i]
                             };

                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            selectedName = e.Row.Cells[0].ToString();
            selectedFO = e.Row.Cells[1].ToString();
            FillSubjectChart();
            FillGroupChart();

            lbSubject.Text = string.Format("<b>{0}</b> Сравнительный анализ структуры кишечной заболеваемости", selectedName);
        }

        #endregion

        #region PDFExport

        protected virtual void SetExportHandlers()
        {
            UltraGridExporter1.PdfExportButton.Click += PdfExportButton_Click;
            UltraGridExporter1.PdfExporter.BeginExport += PdfExporter_BeginExport;
            UltraGridExporter1.PdfExporter.EndExport += PdfExporter_EndExport;
            UltraGridExporter1.ExcelExportButton.Visible = false;
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.WordExportButton.Visible = true;
        }

        protected virtual void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            exportClass.ExportCaptionText(e, LabelTitle.Text);
            exportClass.ExportSubCaptionText(e, LabelSubTitle.Text);
        }

        protected virtual void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = string.Format("0003.pdf");
            UltraGridExporter1.HeaderCellHeight = 40;
            UltraGridExporter1.HeaderChildCellHeight = 40;
            UltraGridExporter1.PdfExporter.Export(grid);
        }

        protected virtual void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            exportClass.ExportSubCaptionText(e, lbSubject.Text);

            ITable table = e.Section.AddTable();
            ITableRow row = table.AddRow();
            ITableCell cell = row.AddCell();
            IText title = cell.AddText();
            title.AddContent(exportClass.GetImageFromChart(chartGroup));
            cell = row.AddCell();
            title = cell.AddText();
            title.AddContent(exportClass.GetImageFromChart(chartSubject));
        }

        #endregion
    }
}
