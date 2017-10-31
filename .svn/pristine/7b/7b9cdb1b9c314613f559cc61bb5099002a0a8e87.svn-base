using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.SGM.SGM_0004
{
    public partial class sgm_0004 : CustomReportPage
    {
        // Коды заболеваний включаемые в данный отчет
        string[] deseasesCodes;
        string[] deseasesNames;

        int year;
        string months;
        string groupName;
        int lastYear;

        private DataTable dtMainFull;
        private DataTable dtMain;
        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMPdfExportHelper exportClass = new SGMPdfExportHelper();

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double dirtyWidth = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
            double dirtyHeight = CRHelper.GetChartHeight((CustomReportConst.minScreenHeight - 180) * 0.5);

            chart1.Height = (Unit)(dirtyHeight * 2);
            chart1.Width = (Unit)(dirtyWidth * 0.5);
            chart2.Width = (Unit)(dirtyWidth * 0.5);
            chart2.Height = chart1.Height;
            grid1.Width = (Unit)(dirtyWidth);
            grid1.Height = Unit.Empty;
            grid2.Width = grid1.Width;
            grid2.Height = Unit.Empty;
            grid3.Width = grid1.Width;
            grid3.Height = Unit.Empty;
            LabelTitle.Width = (Unit)(grid1.Width.Value - 100);
            LabelSubTitle.Width = LabelTitle.Width;
            
            SetExportHandlers();
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            dataRotator.CheckSubjectReport();
            base.Page_Load(sender, e);
            dataRotator.formNumber = 1;
            dataObject.InitObject();

            Array.Resize(ref deseasesCodes, 7);
            deseasesCodes[6] = "55,23,41,40,42,36,48";
            deseasesCodes[1] = "34,31,33,32,27,25,24";
            deseasesCodes[2] = "21,94";
            deseasesCodes[3] = "56,57,67,66,63";
            deseasesCodes[4] = "98,121,19,117,22,95,114,37,38,44,48,53,66,71,81,73";
            deseasesCodes[5] = "20,4,8,13,18,1";
            deseasesCodes[0] = "58,59";

            Array.Resize(ref deseasesNames, 7);
            deseasesNames[6] = "Природно-очаговые инфекции";
            deseasesNames[1] = "Воздушно-капельные инфекции";
            deseasesNames[2] = "Вирусные гепатиты парентеральные";
            deseasesNames[3] = "Социально-обусловленные инфекции";
            deseasesNames[4] = "Прочие инфекции";
            deseasesNames[5] = "Кишечные инфекции";
            deseasesNames[0] = "Грипп и ОРВИ";

            lastYear = dataRotator.GetLastYear();

            if (!Page.IsPostBack)
            {
                dataRotator.FillMonthListEx(ComboMonth, lastYear.ToString());
                dataRotator.FillYearList(ComboYear);
                supportClass.FillMeasure(ComboMeasure);
                dataRotator.FillSGMMapList(ComboMap, dataObject.dtAreaShort, false);
            }
            else
            {
                dataRotator.FillSGMMapList(null, dataObject.dtAreaShort, false);                
            }

            year = Convert.ToInt32(ComboYear.SelectedValue);
            months = dataRotator.GetMonthParamString(ComboMonth, ComboYear.SelectedValue);
            dataRotator.CheckFormNumber(year, ref months);
            groupName = Convert.ToString(Convert.ToInt32(PeopleGroupType.pgtAll));

            dataObject.InitObject();
            // Создание объекта
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
            dataObject.useLongNames = false;
            // ФО
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctFO, "0");
            for (int i = 0; i < deseasesNames.Length; i++)
            {
                dataObject.AddColumn(
                    SGMDataObject.DependentColumnType.dctAbs,
                    Convert.ToString(year),
                    months,
                    string.Empty,
                    groupName,
                    deseasesCodes[i]);

                dataObject.AddColumn(
                    SGMDataObject.DependentColumnType.dctRelation,
                    Convert.ToString(i * 3 + 2));

                dataObject.AddColumn(
                    SGMDataObject.DependentColumnType.dctPercentFromTotal,
                    "2, 5, 8, 11, 14, 17, 20", Convert.ToString(i * 3 + 2));
            }
            dtMainFull = dataObject.FillData();

            string filterName = String.Empty;
            string filterRfFo = String.Empty;
            string column1Name = dtMainFull.Columns[1].ColumnName;
            string column0Name = dtMainFull.Columns[0].ColumnName;
            if (ComboMap.SelectedIndex != 0)
            {
                filterName = supportClass.GetFOShortName(ComboMap.SelectedValue);
                if (ComboMeasure.SelectedIndex != 2)
                {
                    filterRfFo = String.Format(" or {0} = '{2}' or {0} = '{1}'", column0Name, filterName, dataObject.GetRootMapName());
                }
            }
            dtMain = dataObject.CloneDataTable(dtMainFull, 
                dtMainFull.Select(
                    string.Format("{0} = '{1}' {2}", column1Name, filterName, filterRfFo),
                    string.Format("{0} desc", dtMainFull.Columns[2].ColumnName)));
            dtMain.Columns.RemoveAt(1);

            ConfigureCharts();

            chart1.DataSource = GetDataChart1();
            chart1.DataBind();

            chart2.DataSource = GetDataChart2();
            chart2.DataBind();

            ConfigureGrid(grid1, 1, "N0", String.Empty);
            ConfigureGrid(grid2, 2, "N2", String.Empty);
            ConfigureGrid(grid3, 3, "N2", "Удельный вес в общем числе заболеваний");

            Page.Title = String.Format("Группы инфекционной заболеваемости по территориям ({0})",
                supportClass.GetFOShortName(ComboMap.SelectedValue));
            LabelTitle.Text = Page.Title;
            LabelSubTitle.Text = String.Format("{0} по группам за {1} {2} год{3}{4}", ComboMeasure.SelectedValue,
                dataRotator.GetMonthParamLabel(ComboMonth, ComboYear.SelectedValue), ComboYear.SelectedValue,
                dataRotator.GetYearAppendix(), dataRotator.GetFormHeader());
        }

        private void ConfigureGrid(UltraWebGrid grid, int columnIndex, string numberFormat, string tooltip)
        {
            var tblData = new DataTable();
            tblData.Columns.Add("ColumnName", typeof (string));
            for (int i = 0; i < deseasesNames.Length; i++)
            {
                tblData.Columns.Add(String.Format("ColumnData{0}", i), typeof(double));
            }

            foreach (DataRow rowFullData in dtMain.Rows)
            {
                DataRow rowData = tblData.Rows.Add();
                rowData[0] = rowFullData[0];

                for (int i = 0; i < deseasesNames.Length; i++)
                {
                    rowData[i + 1] = rowFullData[columnIndex + i * 3];
                }
            }

            dataRotator.FillMaxAndMin(tblData, 1);

            grid.DataSource = tblData;
            grid.DataBind();

            supportClass.SetColumnWidthAndCaption(grid, 0, "Территория", 170, HorizontalAlign.Left, String.Empty);
            for (int j = 0; j < deseasesCodes.Length; j++)
            {
                supportClass.SetColumnWidthAndCaption(grid, j + 1, deseasesNames[j], 100, HorizontalAlign.Right, tooltip);
                CRHelper.FormatNumberColumn(grid.Columns[j + 1], numberFormat);
            }            
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
        }

        protected virtual DataTable GetDataChart1()
        {
            var dtResult = new DataTable();

            DataColumn dataColumn = dtResult.Columns.Add();
            dataColumn.DataType = typeof(string);
            dataColumn = dtResult.Columns.Add();
            dataColumn.DataType = typeof(double);
            dataColumn.ColumnName = deseasesNames[0];

            for (int j = 0; j < dtMain.Rows.Count; j++)
            {
                DataRow dr = dtResult.Rows.Add();
                dr[0] = supportClass.GetFOShortName(dtMain.Rows[j][0].ToString());
                dr[1] = Convert.ToDouble(dtMain.Rows[j][3 - ComboMeasure.SelectedIndex].ToString());
            }

            return dtResult;
        }

        protected virtual DataTable GetDataChart2()
        {
            var dtResult = new DataTable();

            DataColumn dataColumn = dtResult.Columns.Add();
            dataColumn.DataType = typeof(string);
            for (int j = 1; j < deseasesCodes.Length; j++)
            {
                dataColumn = dtResult.Columns.Add();
                dataColumn.DataType = typeof(double);
                dataColumn.ColumnName = deseasesNames[j];
            }

            for (int i = 0; i < dtMain.Rows.Count; i++)
            {
                DataRow dr = dtResult.Rows.Add();
                dr[0] = supportClass.GetFOShortName(dtMain.Rows[i][0].ToString());
                for (int j = 1; j < deseasesCodes.Length; j++)
                {
                    dr[j] = Convert.ToDouble(dtMain.Rows[i][3 + j * 3 - ComboMeasure.SelectedIndex].ToString());
                }
            }
            return dtResult;
        }

        protected virtual void ConfigureCharts()
        {
            chart1.Border.Thickness = 0;
            chart2.Border.Thickness = 0;
            if (ComboMeasure.SelectedIndex != 0) chart1.Data.ZeroAligned = true;
            chart2.Data.ZeroAligned = true;

            string appendix = String.Empty;

            string axisMask = "N0";
            if (ComboMeasure.SelectedIndex == 0)
            {
                axisMask = "N2";
                appendix = "%";
            }
            if (ComboMeasure.SelectedIndex == 1) appendix = String.Empty;
            if (ComboMeasure.SelectedIndex == 2) appendix = String.Empty;

            chart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n <ITEM_LABEL>\n <DATA_VALUE:{1}>{0}", appendix, axisMask);
            chart2.Tooltips.FormatString = chart1.Tooltips.FormatString;


            // X

            chart1.Axis.X.Labels.ItemFormatString = String.Format("<DATA_VALUE:N0>{0}", appendix);
            chart2.Axis.X.Labels.ItemFormatString = chart1.Axis.X.Labels.ItemFormatString;

            chart1.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            chart2.Axis.X.Labels.Orientation = TextOrientation.Horizontal;

            chart1.Axis.X.Extent = 20;
            chart2.Axis.X.Extent = 20;

            chart1.Axis.X.Labels.SeriesLabels.Visible = false;
            chart2.Axis.X.Labels.SeriesLabels.Visible = false;

            chart1.Axis.X.Labels.Visible = true;
            chart2.Axis.X.Labels.Visible = true;

            chart1.Axis.X.Labels.WrapText = false;
            chart2.Axis.X.Labels.WrapText = false;

            CRHelper.FillCustomColorModel(chart2, 6, false);

            // Y

            chart1.Axis.Y.Extent = 150;
            chart2.Axis.Y.Extent = 150;
            
            if (ComboMap.SelectedIndex == 0 && !dataRotator.isSubjectReport)
            {
                chart1.Axis.Y.Extent = 50;
                chart2.Axis.Y.Extent = 50;
            }

            chart1.Axis.Y.Labels.Visible = false;
            chart2.Axis.Y.Labels.Visible = false;
            chart1.Axis.Y.Labels.SeriesLabels.Visible = true;
            chart2.Axis.Y.Labels.SeriesLabels.Visible = true;

            chart1.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            chart2.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;

            chart1.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 18);
            chart2.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 18);

            chart1.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Far;
            chart2.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Far;

            // Legend

            chart1.Legend.Visible = true;
            chart1.Legend.Location = LegendLocation.Bottom;
            chart2.Legend.Visible = true;
            chart2.Legend.Location = LegendLocation.Bottom;


            chart1.TitleTop.Visible = true;
            chart1.TitleTop.Extent = 30;
            chart1.TitleTop.Orientation = TextOrientation.Horizontal;
            chart1.TitleTop.HorizontalAlign = StringAlignment.Center;
            chart1.TitleTop.Text = String.Format("Грипп и ОРВИ ({0})", ComboMeasure.SelectedValue);
            chart1.TitleTop.Font = new Font("Verdana", 12);

            chart2.TitleTop.Visible = true;
            chart2.TitleTop.Extent = 30;
            chart2.TitleTop.Orientation = TextOrientation.Horizontal;
            chart2.TitleTop.HorizontalAlign = StringAlignment.Center;
            chart2.TitleTop.Text = String.Format("Другие группы ({0})", ComboMeasure.SelectedValue);
            chart2.TitleTop.Font = new Font("Verdana", 12);


            chart1.Height = dtMain.Rows.Count * 30 + 190;
            chart2.Height = chart1.Height;

            chart1.Legend.SpanPercentage = Convert.ToInt32(9500 / chart1.Height.Value);
            chart2.Legend.SpanPercentage = chart1.Legend.SpanPercentage;

            //chart2.Axis.X

            chart1.Axis.X.TickmarkStyle = AxisTickStyle.Percentage;
            chart1.Axis.X.TickmarkPercentage = 25;

            chart2.Axis.X.TickmarkStyle = AxisTickStyle.Percentage;
            chart2.Axis.X.TickmarkPercentage = 25;
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            if (ComboMap.SelectedIndex == 0 || (ComboMap.SelectedIndex != 0 && e.Row.Index < 2))
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }

            if (sender.Equals(grid1))
            {
                return;
            }

            for (int i = 0; i < 7; i++)
            {
                int cellIndex = i + 1;
                supportClass.SetCellImageStar(e.Row, dataRotator.maxValues, dataRotator.minValues, cellIndex, cellIndex, cellIndex);
            }
        }

        #region PDFExport

        private void SetExportHandlers()
        {
            UltraGridExporter1.PdfExportButton.Click += PdfExportButton_Click;
            UltraGridExporter1.PdfExporter.BeginExport += PdfExporter_BeginExport;
            UltraGridExporter1.ExcelExportButton.Visible = false;
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.WordExportButton.Visible = true;
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            InitializeExportLayout(e);
            exportClass.ExportCaptionText(e, LabelTitle.Text);
            exportClass.ExportMainText(e, LabelSubTitle.Text);
            exportClass.ExportChart(e, chart1);
            exportClass.ExportChart(e, chart2);

        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = string.Format("0004.pdf");
            UltraGridExporter1.PdfExporter.Export(grid1);
        }

        protected virtual void InitializeExportLayout(Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            chart1.Width = grid1.Width;
            chart2.Width = grid1.Width;
        }

        #endregion

        protected void chart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    var label = (Text)primitive;                    
                    if (label.GetTextString() != null)
                    {
                        label.bounds = new Rectangle(
                            label.bounds.X - 30,
                            label.bounds.Y, 
                            70,
                            label.bounds.Height);
                    }
                }
            }
        }
    }
}
