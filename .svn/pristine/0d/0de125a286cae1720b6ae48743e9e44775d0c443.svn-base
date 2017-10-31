using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.SgmSupport;

namespace Krista.FM.Server.Dashboards.reports.SGM.SGM_0009
{
    public partial class sgm_0009 : CustomReportPage
    {
        private int year, prevYear;
        private string months;
        private string groupName;
        private readonly DataTable dtChart = new DataTable();
        private DataTable dtGrid = new DataTable();
        DataRow[] regionsSet;

        private readonly SGMDataRotator dataRotator = new SGMDataRotator();
        private readonly SGMDataObject dataObject = new SGMDataObject();
        private readonly SGMSupport supportClass = new SGMSupport();
        private readonly SGMPdfExportHelper exportClass = new SGMPdfExportHelper();

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            int dirtyHeigth = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 220);
            dirtyHeigth = Convert.ToInt32(dirtyHeigth * 0.30);
            chart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
            chart.Height = (dirtyHeigth * 2);
            grid.Width = chart.Width;
            grid.Height = Unit.Empty; //dirtyHeigth;
            LabelTitle.Width = (Unit)(chart.Width.Value - 100);
            LabelSubTitle.Width = LabelTitle.Width;

            if ((Session["PrintVersion"] != null && (bool)Session["PrintVersion"]) ||
                Session["ShowParams"] != null && !(bool)Session["ShowParams"])
            {
                LabelCriterium.Visible = false;
                RadioCriterium.Visible = false;
            }     
       
            SetExportHandlers();
        }

        protected virtual void AddRowToChart(DataRow r)
        {
            DataRow row = dtChart.Rows.Add();
            if (r != null)
            {
                string subjectName = Convert.ToString(r[0]);
                subjectName = subjectName.Replace("автономный округ", "АО");
                subjectName = subjectName.Replace("республика", "респ.");
                subjectName = subjectName.Replace("Республика", "Р.");

                row[0] = subjectName;
                row[1] = r[3];
                row[2] = r[7];
            }
        }

        protected virtual string GetDeseaseCodes()
        {
            return dataRotator.deseasesRelation[ComboDeseases.SelectedValue];
        }

        protected virtual void FillMedian(DataRow[] drsData, DataRow row, int colIndex)
        {
            if (drsData.Length > 1)
            {
                int medianIndex = drsData.Length / 2;
                bool oddCount = drsData.Length % 2 > 0;
                int columnIndex = 3 + (colIndex - 1) * 4;

                double value1 = Convert.ToDouble(drsData[medianIndex + 1][columnIndex]);

                if (oddCount)
                {
                    row[colIndex] = value1;
                }
                else
                {
                    double value2 = Convert.ToDouble(drsData[medianIndex][columnIndex]);
                    row[colIndex] = (value1 + value2) / 2;
                }
            }
        }

        protected virtual void CreateChartStruct()
        {
            dtChart.Columns.Add("ColumnCaption", typeof (string));
            dtChart.Columns.Add(Convert.ToString(year), typeof(double));
            dtChart.Columns.Add(Convert.ToString(prevYear), typeof(double));
        }

        protected virtual void FillReportParams()
        {
            year = Convert.ToInt32(ComboYear.SelectedValue);
            months = dataRotator.GetMonthParamString(ComboMonth, ComboYear.SelectedValue);
            prevYear = year - 1;
            var pgt = PeopleGroupType.pgtAll;
            int groupIndex = ComboGroup.SelectedIndex;
            if (groupIndex == 1) pgt = PeopleGroupType.pgtMature;
            if (groupIndex == 2) pgt = PeopleGroupType.pgtChild;
            if (groupIndex == 3) pgt = PeopleGroupType.pgtTeen;
            groupName = Convert.ToString(Convert.ToInt32(pgt));
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
                dataRotator.FillYearList(ComboYear);
                dataRotator.FillMonthListEx(ComboMonth, ComboYear.SelectedValue);
                dataRotator.FillDeseasesList(ComboDeseases, 0);
                supportClass.FillPeopleGroupList(ComboGroup);
            }
            else
            {
                dataRotator.FillDeseasesList(null, -1);
            }

            FillReportParams();
            dataRotator.CheckFormNumber(year, ref months);
            CreateChartStruct();
            FillGridDataSet();
            DataRow[] rowsYear1 = dtGrid.Select(String.Empty, dtGrid.Columns[3].ColumnName + " desc");
            DataRow[] rowsYear2 = dtGrid.Select(String.Empty, dtGrid.Columns[7].ColumnName + " desc");
            DataRow[] rowsPopulation = dtGrid.Select(String.Empty, dtGrid.Columns[4].ColumnName + " desc");
            regionsSet = dtGrid.Select(dtGrid.Columns[1].ColumnName + " ='' and " + 
                dtGrid.Columns[0].ColumnName + string.Format(" <> '{0}'", dataObject.GetRootMapName()));
            
            if (RadioCriterium.SelectedIndex == 0)
            {
                if (rowsYear1.Length > 4)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        AddRowToChart(rowsYear1[i]);
                    }
                    
                    var row = dtChart.Rows.Add();
                    row[0] = dataObject.GetRootMapName();
                    row[1] = rowsPopulation[0][3];
                    row[2] = rowsPopulation[0][7];

                    row = dtChart.Rows.Add();
                    row[0] = "Медиана";
                    FillMedian(rowsYear1, row, 1);
                    FillMedian(rowsYear2, row, 2);

                    for (int i = rowsYear1.Length - 5; i < rowsYear1.Length; i++)
                    {
                        AddRowToChart(rowsYear1[i]);
                    }
                }
                else
                {
                    dtChart.Rows.Add();
                }

                string territoryName = "Субъекты";
                if (dataRotator.isSubjectReport)
                {
                    territoryName = "Районы";
                }

                var annotation1 = new BoxAnnotation
                                      {
                                          Text =
                                              String.Format("{0} с самыми высокими показателями заболеваемости",
                                                            territoryName),
                                          Width = -1,
                                          Height = 17,
                                          Location =
                                              {
                                                  Type = LocationType.Percentage,
                                                  LocationX = 20,
                                                  LocationY = 3
                                              }
                                      };

                chart.Annotations.Add(annotation1);

                var annotation2 = new BoxAnnotation
                                      {
                                          Text =
                                              String.Format("{0} с самыми низкими показателями заболеваемости",
                                                            territoryName),
                                          Width = -1,
                                          Height = 17,
                                          Location =
                                              {
                                                  Type = LocationType.Percentage,
                                                  LocationX = 80,
                                                  LocationY = 3
                                              }
                                      };

                chart.Annotations.Add(annotation2);
            }
            else
            {
                int realCount = 0;
                for (int i = 0; i < regionsSet.Length; i++)
                {
                    DataRow[] drsSubjects = dtGrid.Select(
                        String.Format(dtGrid.Columns[1].ColumnName + " = '{0}'",
                        supportClass.GetFOShortName(regionsSet[i][0].ToString())),
                        String.Format("{0} desc", dtGrid.Columns[3].ColumnName));
                    if (drsSubjects.Length > 0) realCount++;
                }
                int counter = 0;
                for (int i = 0; i < regionsSet.Length; i++)
                {
                    DataRow[] drsSubjects = dtGrid.Select(
                        String.Format(dtGrid.Columns[1].ColumnName + " = '{0}'",
                        supportClass.GetFOShortName(regionsSet[i][0].ToString())), 
                        String.Format("{0} desc", dtGrid.Columns[3].ColumnName));


                    if (drsSubjects.Length > 0)
                    {
                        const double baseOff = 12;
                        double locX = baseOff + counter * Convert.ToInt32(100 / realCount);
                        if (i == 7) locX -= 2;

                        var anno = new BoxAnnotation
                                       {
                                           Text = string.Format(supportClass.GetFOShortName(regionsSet[i][0].ToString())),
                                           Width = -1,
                                           Height = -1,
                                           Location =
                                               {
                                                   Type = LocationType.Percentage,
                                                   LocationX = locX,
                                                   LocationY = 3
                                               }
                                       };

                        chart.Annotations.Add(anno);
                        counter++;
                        AddRowToChart(drsSubjects[0]);
                        AddRowToChart(drsSubjects[drsSubjects.Length - 1]);
                    }
                }
            }

            ConfigureChart();
            chart.DataSource = dtChart;
            chart.DataBind();

            grid.DataSource = dtGrid;
            grid.DataBind();

            LabelCriterium.Text = "Минимум и максимум по" ;
            Page.Title = String.Format("Территории {0} с самыми высокими/низкими показателями заболеваемости", 
                dataObject.GetRootMapName());
            LabelTitle.Text = Page.Title;
            string monthLabel = dataRotator.GetMonthParamLabel(ComboMonth, ComboYear.SelectedValue);
            LabelSubTitle.Text = String.Format("{0}, за {1} {2} год{4}, группа населения: {3}{5}",
                ComboDeseases.SelectedValue, monthLabel, ComboYear.SelectedValue,
                ComboGroup.SelectedValue, dataRotator.GetYearAppendix(), dataRotator.GetFormHeader());
        }

        private void ConfigureChart()
        {
            chart.TitleLeft.Visible = true;
            chart.TitleLeft.Text = chart.TitleLeft.Text = "На 100 тыс. населения";
            chart.TitleLeft.Extent = 30;
            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;

            chart.TitleTop.Visible = true;
            chart.TitleTop.Text = " ";
            chart.TitleTop.Extent = 20;

            string tooltipAppend = String.Empty;
            if (RadioCriterium.SelectedIndex == 0)
            {
                tooltipAppend = " год";
            }

            chart.Tooltips.FormatString = String.Format(
                "<SERIES_LABEL>\n <ITEM_LABEL>{0}\n <DATA_VALUE:N0> на 100 тыс.чел.", tooltipAppend);
            chart.Border.Thickness = 0;
            chart.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            chart.Data.ZeroAligned = true;

            int legendMargin = Convert.ToInt32(chart.Width.Value * 0.42);
            chart.Legend.SpanPercentage = Convert.ToInt32(100 * 37 / chart.Height.Value);
            chart.Legend.Margins.Right = legendMargin;
            chart.Legend.Margins.Left = legendMargin;
            
            // Оси
            chart.Axis.X.Extent = 170;
            chart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            chart.Axis.X.Labels.Visible = false;
            chart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 15, FontStyle.Regular);
        }

        protected virtual void FillGridDataSet()
        {
            dataObject.InitObject();
            dataObject.mainColumn = SGMDataObject.MainColumnType.mctMapName;
            dataObject.useLongNames = true;
            // ФО
            dataObject.AddColumn(SGMDataObject.DependentColumnType.dctFO, "0");
            for (int i = 0; i < 2; i++)
            {
                dataObject.AddColumn(
                    SGMDataObject.DependentColumnType.dctAbs,
                    Convert.ToString(year - i),
                    months,
                    String.Empty,
                    groupName,
                    GetDeseaseCodes());
                dataObject.AddColumn(
                    SGMDataObject.DependentColumnType.dctRelation,
                    Convert.ToString(i * 4 + 2));
                dataObject.AddColumn(
                    SGMDataObject.DependentColumnType.dctPopulation, Convert.ToString(2 + i * 4));
                dataObject.AddColumn(
                    SGMDataObject.DependentColumnType.dctRank,
                    Convert.ToString(i * 4 + 3));
            }
            dtGrid = dataObject.FillData(3);
            dataRotator.FillMaxAndMin(dtGrid, 2);
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            // Номер полосы для создания групповых заголовков в таблице
            const int BandIndex = 0;

            RowLayoutColumnInfo rlcInfo;
            e.Layout.GroupByBox.Hidden = true;

            for (int i = 0; i < e.Layout.Bands[BandIndex].Columns.Count; i++)
            {
                rlcInfo = e.Layout.Bands[BandIndex].Columns[i].Header.RowLayoutColumnInfo;
                if (i == 0 || i == 1)
                {
                    rlcInfo.OriginY = 0;
                    rlcInfo.SpanY = 2;
                }
                else
                {
                    rlcInfo.OriginY = 1;
                }
            }

            string foCaption = "ФО";
            string foHint = "Федеральный округ";
            if (dataRotator.isSubjectReport)
            {
                foCaption = "ТО";
                foHint = "Территориальное образование";
            }

            const string absTooltip = "Абсолютный показатель заболеваемости за {0} {1} года";
            const string relTooltip = "Относительный показатель заболеваемости на 100 тысяч населения за {0} {1} года";
            const string rngTooltip = "Ранг по относительному показателю заболеваемости за {0} {1} года";
            const string populationTooltip = "Численность населения";
            const string labelRegion = "Название территории";
            const string labelRng = "Ранг";
            const string labelAbs = "абс.";
            const string labelRel = "на 100 тыс.";
            const string labelPopulation = "Население";
            const HorizontalAlign dataAlign = HorizontalAlign.Right;

            string monthText = dataRotator.GetMonthParamLabel(ComboMonth, ComboYear.SelectedValue);
            const int colWidth = 80;
            supportClass.SetColumnWidthAndCaption(grid, 0, labelRegion, 200, HorizontalAlign.Left, labelRegion);
            supportClass.SetColumnWidthAndCaption(grid, 1, foCaption, 40, HorizontalAlign.Center, foHint);
            supportClass.SetColumnWidthAndCaption(grid, 2, labelAbs, colWidth, dataAlign, String.Format(absTooltip, monthText, year));
            supportClass.SetColumnWidthAndCaption(grid, 3, labelRel, colWidth, dataAlign, String.Format(relTooltip, monthText, year));
            supportClass.SetColumnWidthAndCaption(grid, 4, labelPopulation, colWidth, dataAlign, populationTooltip);
            supportClass.SetColumnWidthAndCaption(grid, 5, labelRng, colWidth, dataAlign, String.Format(rngTooltip, monthText, year));
            supportClass.SetColumnWidthAndCaption(grid, 6, labelAbs, colWidth, dataAlign, String.Format(absTooltip, monthText, prevYear));
            supportClass.SetColumnWidthAndCaption(grid, 7, labelRel, colWidth, dataAlign, String.Format(relTooltip, monthText, prevYear));
            supportClass.SetColumnWidthAndCaption(grid, 8, labelPopulation, colWidth, dataAlign, populationTooltip);
            supportClass.SetColumnWidthAndCaption(grid, 9, labelRng, colWidth, dataAlign, String.Format(rngTooltip, monthText, prevYear));

            for (int i = 2; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = colWidth;
                CRHelper.FormatNumberColumn(grid.Columns[i], "N0");
            }

            CRHelper.FormatNumberColumn(grid.Columns[03], "N2");
            CRHelper.FormatNumberColumn(grid.Columns[07], "N2");

            int multiHeaderPos = 2;
            for (int i = 2; i < e.Layout.Bands[BandIndex].Columns.Count; i += 4)
            {
                var ch = new ColumnHeader(true);
                if (i == 02) ch.Caption = String.Format("{0} {1} год", monthText, ComboYear.SelectedValue);
                if (i == 06) ch.Caption = String.Format("{0} {1} год", monthText, prevYear);
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 4;
                ch.RowLayoutColumnInfo.SpanX = 4;
                ch.Style.Wrap = true;
                ch.Style.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[BandIndex].HeaderLayout.Add(ch);

                if (e.Layout.Bands[BandIndex].HeaderLayout.Count > 10)
                {
                    if (i == 2) e.Layout.Bands[BandIndex].HeaderLayout[10].Caption = ch.Caption;
                    if (i == 6) e.Layout.Bands[BandIndex].HeaderLayout[11].Caption = ch.Caption;
                }
            }
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Style.Wrap = true;
            if (e.Row.Cells[0].Value.ToString() == "Медиана")
            {
                e.Row.Style.BackColor = Color.MediumAquamarine;
                e.Row.Cells[0].Style.Font.Bold = true;
                return;
            }
            if (e.Row.Cells[0].Value.ToString() == dataObject.GetRootMapName())
            {
                e.Row.Style.BackColor = Color.LightBlue;
                e.Row.Cells[0].Style.Font.Bold = true;
                return;
            }
            if (e.Row.Cells[1].Value.ToString() == string.Empty)
            {
                e.Row.Style.BackColor = Color.LightGray;
                e.Row.Cells[0].Style.Font.Bold = true;
                return;
            }

            supportClass.SetCellImageStar(e.Row, dataRotator.maxValues, dataRotator.minValues, 5, 5, 5, true);
            supportClass.SetCellImageStar(e.Row, dataRotator.maxValues, dataRotator.minValues, 9, 9, 9, true);
            supportClass.SetCellImageEx(e.Row, 7, 3, 3);
        }

        protected void chart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            var xAxis = (IAdvanceAxis)e.Grid["X"];
            var yAxis = (IAdvanceAxis)e.Grid["Y"];
            if (xAxis == null || yAxis == null) return;
            var xMin = (int)xAxis.MapMinimum;
            var yMin = (int)yAxis.MapMinimum;
            var yMax = (int)yAxis.MapMaximum;
            var xMax = (int)xAxis.MapMaximum;

            if (RadioCriterium.SelectedIndex == 0)
            {
                var line = new Line
                               {
                                   lineStyle = {DrawStyle = LineDrawStyle.Dash},
                                   PE =
                                       {
                                           Stroke = Color.DarkGray,
                                           StrokeWidth = 2
                                       },
                                   p1 = new Point(Convert.ToInt32(xMin + (xMax - xMin)*0.414), yMin),
                                   p2 = new Point(Convert.ToInt32(xMin + (xMax - xMin)*0.414), yMax)
                               };

                e.SceneGraph.Add(line);

                line = new Line
                           {
                               lineStyle = {DrawStyle = LineDrawStyle.Dash},
                               PE =
                                   {
                                       Stroke = Color.DarkGray,
                                       StrokeWidth = 2
                                   },
                               p1 = new Point(Convert.ToInt32(xMin + (xMax - xMin)*0.584), yMin),
                               p2 = new Point(Convert.ToInt32(xMin + (xMax - xMin)*0.584), yMax)
                           };

                e.SceneGraph.Add(line);
            }
            else
            {
                // 0.128
                double tickSize = 1.02 / Convert.ToDouble(dtChart.Rows.Count / 2);
                for (int i = 1; i < dtChart.Rows.Count / 2; i++)
                {
                    var line = new Line
                                   {
                                       lineStyle = {DrawStyle = LineDrawStyle.Dash},
                                       PE =
                                           {
                                               Stroke = Color.DarkGray,
                                               StrokeWidth = 2
                                           },
                                       p1 = new Point(Convert.ToInt32(xMin + (xMax - xMin) * tickSize * i) - 10, yMin),
                                       p2 = new Point(Convert.ToInt32(xMin + (xMax - xMin) * tickSize * i) - 10, yMax)
                                   };

                    e.SceneGraph.Add(line);
                }
            }
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    var box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.DataPoint.Label != prevYear.ToString())
                        {
                            if (RadioCriterium.SelectedIndex == 0)
                            {
                                if (box.Row > 6)
                                {
                                    box.PE.Fill = Color.Green;
                                    box.PE.FillStopColor = Color.ForestGreen;
                                }
                                if (box.Row < 5)
                                {
                                    box.PE.Fill = Color.Red;
                                    box.PE.FillStopColor = Color.Maroon;
                                }
                                if (box.Row == 5)
                                {
                                    box.PE.Fill = Color.LightSteelBlue;
                                    box.PE.FillStopColor = Color.DeepSkyBlue;
                                }
                                if (box.Row == 6)
                                {
                                    box.PE.Fill = Color.MediumAquamarine;
                                    box.PE.FillStopColor = Color.DarkCyan;
                                }
                            }
                            else
                            {
                                string foName = supportClass.GetFOShortName(regionsSet[Convert.ToInt32(box.Row / 2)][1].ToString());
                                string territoryName = "Субъект";
                                if (dataRotator.isSubjectReport) territoryName = "Район";

                                if (box.Row % 2 == 0)
                                {
                                    box.PE.Fill = Color.Red;
                                    box.PE.FillStopColor = Color.Maroon;
                                    box.DataPoint.Label = string.Format("{0} год<br>{1} {2}", 
                                        box.DataPoint.Label,
                                        string.Format("{0} с самым высоким уровнем заболеваемости в ", territoryName), 
                                        foName);
                                }
                                else
                                {
                                    box.DataPoint.Label = string.Format("{0} год<br>{1} {2}",
                                        box.DataPoint.Label, 
                                        string.Format("{0} с самым низким уровнем заболеваемости в ", territoryName),
                                        foName);
                                    box.PE.Fill = Color.Green;
                                    box.PE.FillStopColor = Color.ForestGreen;
                                }                              
                            }
                        }
                        else
                        {
                            box.PE.ElementType = PaintElementType.Hatch;
                            box.PE.Fill = Color.Blue;
                            box.PE.FillStopColor = Color.Transparent;
                            box.PE.Hatch = FillHatchStyle.Weave;
                            box.PE.FillOpacity = 100;
                            box.lineStyle.DrawStyle = LineDrawStyle.Dash;
                        }
                    }
                    else if (i != 0 && box.Path == "Border.Title.Legend")
                    {
                        Primitive primitive1 = e.SceneGraph[i - 0];
                        Primitive primitive2 = e.SceneGraph[i - 1];
                        if (primitive2 is Text && primitive1 is Box)
                        {
                            var text = (Text)primitive2;
                            var box2 = (Box)e.SceneGraph[i - 2];
                            var box1 = (Box)primitive1;
                            if (text.GetTextString() == ComboYear.SelectedValue)
                            {
                                box1.PE.ElementType = PaintElementType.Hatch;
                                box1.PE.Fill = Color.Blue;
                                box1.PE.FillStopColor = Color.Transparent;
                                box1.PE.Hatch = FillHatchStyle.Weave;
                                box1.PE.FillOpacity = 100;
                                box1.lineStyle.DrawStyle = LineDrawStyle.Dash;

                                box2.PE.ElementType = PaintElementType.Gradient;
                                box2.PE.Fill = Color.Green;
                                box2.PE.FillStopColor = Color.Red;
                            }
                        }
                    }
                }
            }
        }

        #region PDFExport

        protected virtual void SetExportHandlers()
        {
            UltraGridExporter1.PdfExportButton.Click += PdfExportButton_Click;
            UltraGridExporter1.PdfExporter.BeginExport += PdfExporter_BeginExport;
            UltraGridExporter1.ExcelExportButton.Visible = false;
            UltraGridExporter1.WordExportButton.Visible = true;
        }

        protected virtual void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            InitializeExportLayout(e);

            exportClass.ExportCaptionText(e, LabelTitle.Text);
            exportClass.ExportSubCaptionText(e, LabelSubTitle.Text);
            exportClass.ExportChart(e, chart);
        }

        protected virtual void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = String.Format("0009.pdf");
            UltraGridExporter1.PdfExporter.Export(grid);
        }

        protected virtual void InitializeExportLayout(Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            chart.Width = (Unit)(chart.Width.Value - 100);
        }

        #endregion
    }
    
}
