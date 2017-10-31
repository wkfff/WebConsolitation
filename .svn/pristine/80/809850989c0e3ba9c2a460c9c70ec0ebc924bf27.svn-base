using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0042_0002
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChartAVG;
        private DataTable dtChart;
        private int firstYear = 2008;
        private int endYear = 2011;

        private int currentYear;
        private double avgValue;

        private int firstGroupCount;
        private int secondGroupCount;
        private int thirdGroupCount;

        private double firstGroupAvg;
        private double secondGroupAvg;
        private double thirdGroupAvg;

        private int agvTextWidth = 150;
        private int avgTextHeight = 12;

        private static MemberAttributesDigest grbsDigest;
        private static MemberAttributesDigest indicatorDigest;

        #endregion

        private bool UseGRBSGroups
        {
            get { return WithoutGRBSGroups.Checked; }
        }

        public bool UseComparabledDimension
        {
            get { return currentYear > 2009; }
        }

        public bool IndicatorPercentFormat 
        {
            get { return ComboIndicator.SelectedValue == "Оценка качества финансового менеджмента"; }
        }

        public string IndicatorFormat
        {
            get { return IndicatorPercentFormat ? "<DATA_VALUE:N2>%" : "<DATA_VALUE:N2>"; }
        }

        #region Параметры запроса

        // множество администраторов ГРБС
        private CustomParam grbsSet;

        // измерение [Показатели].[Оценка качества ФМ]
        private CustomParam indicatorDimension;
        // корневой элемент [Показатели].[Оценка качества ФМ]
        private CustomParam indicatorAllLevel;

        // измерение [Администратор].[Анализ]
        private CustomParam administratorDimension;
        // корневой элемент [Администратор].[Анализ]
        private CustomParam administratorAllLevel;

        // выбранный показатель
        private CustomParam selectedIndicator;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            
            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.7);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChart_InvalidDataReceived);

            #region Инициализация параметров запроса

            grbsSet = UserParams.CustomParam("grbs_set");

            indicatorDimension = UserParams.CustomParam("indicator_dimension");
            indicatorAllLevel = UserParams.CustomParam("indicator_all_level");
            administratorDimension = UserParams.CustomParam("administrator_dimension");
            administratorAllLevel = UserParams.CustomParam("administrator_all_level");
            selectedIndicator = UserParams.CustomParam("selected_indicator");

            #endregion

            UltraGridExporter1.MultiHeader = false;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;оценки&nbsp;качества";
            CrossLink1.NavigateUrl = "~/reports/FO_0042_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Анализ&nbsp;оценки&nbsp;качества";
            CrossLink2.NavigateUrl = "~/reports/FO_0042_0003/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            grbsDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0042_0002_grbsDigest");
            indicatorDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0042_0002_indicatorDigest");

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddRefreshTarget(LegendChartBrick.Chart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(WithoutGRBSGroups.ClientID);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0042_0002_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.AutoPostBack = true;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboIndicator.Title = "Показатель";
                ComboIndicator.Width = 500;
                ComboIndicator.MultiSelect = false;
                ComboIndicator.ParentSelect = false;
                ComboIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(indicatorDigest.UniqueNames, indicatorDigest.MemberLevels));
                ComboIndicator.SetСheckedState("Оценка качества финансового менеджмента", true);
            }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue);

            ComboIndicator.Visible = UseComparabledDimension;

            Page.Title = String.Format("Рейтинг главных распорядителей средств областного бюджета {0}",
                UseComparabledDimension ? "(" + ComboIndicator.SelectedValue + ")" : ", сформированный по результатам оценки качества финансового менеджмента");
            PageTitle.Text = Page.Title; 
            PageSubTitle.Text = String.Format("по итогам {0} года", currentYear);
            
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;

            grbsSet.Value = UseGRBSGroups ? "Cписок администраторов c группами" : "Cписок администраторов";

            indicatorDimension.Value = UseComparabledDimension
                               ? "[Показатели].[Оценка качества ФМ_Сопоставимый]"
                               : "[Показатели].[Оценка качества ФМ]";
            indicatorAllLevel.Value = UseComparabledDimension
                                           ? "[Показатели].[Оценка качества ФМ_Сопоставимый].[Все показатели]"
                                           : String.Format("[Показатели].[Оценка качества ФМ].[Данные всех источников].[ФО\\0042 Оценка качества ФМ - {0} квартал 0]", currentYear);
            administratorDimension.Value = UseComparabledDimension
                                           ? "[Администратор].[Сопоставим]"
                                           : "[Администратор].[Анализ]";
            administratorAllLevel.Value = UseComparabledDimension
                                           ? "[Администратор].[Сопоставим].[Все администраторы]"
                                           : String.Format("[Администратор].[Анализ].[Данные всех источников].[ФО Анализ данных - {0}]", currentYear);

            selectedIndicator.Value = UseComparabledDimension ? indicatorDigest.GetMemberUniqueName(ComboIndicator.SelectedValue) : "[Показатели].[Оценка качества ФМ_Сопоставимый].[Все показатели]";

            SetChartAppearance();
            AVGChartDataBind();
            UltraChart.DataBind();
        }

        #region Обработчики диаграммы

        private void SetChartAppearance()
        {
            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;

            UltraChart.ColumnChart.SeriesSpacing = 1;
            UltraChart.ColumnChart.ColumnSpacing = 1;

            UltraChart.Axis.X.Extent = 130;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
            UltraChart.Axis.Y.Extent = 50;
            UltraChart.Axis.Y.Labels.ItemFormatString = IndicatorFormat;
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Axis.X2.Visible = true;
            UltraChart.Axis.X2.Labels.Visible = false;
            UltraChart.Axis.X2.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X2.LineColor = Color.Transparent;
            UltraChart.Axis.X2.Extent = UseGRBSGroups ? 30 : 10;

            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";

            UltraChart.ColorModel.ModelStyle = ColorModels.LinearRange;
            UltraChart.ColorModel.ColorBegin = Color.Green;
            UltraChart.ColorModel.ColorEnd = Color.Red;

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N2>";
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart.ColumnChart.ChartText.Add(appearance);

            UltraChart.Tooltips.FormatString = String.Format("<ITEM_LABEL>\n<SERIES_LABEL>\n{0}", IndicatorFormat);

            #endregion

            #region Настройка диаграммы-легенды

            LegendChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            LegendChartBrick.Height = 60;
            LegendChartBrick.SwapRowAndColumns = true;
            LegendChartBrick.ColorModel = ChartColorModel.ExtendedFixedColors;
            LegendChartBrick.Legend.Margins.Right = Convert.ToInt32(LegendChartBrick.Width.Value / 4);

            LegendChartBrick.Chart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            LegendChartBrick.Chart.Effects.Effects.Clear();
            GradientEffect effect = new GradientEffect();
            effect.Coloring = GradientColoringStyle.Darken;
            effect.Enabled = true;
            LegendChartBrick.Chart.Effects.Enabled = true;
            LegendChartBrick.Chart.Effects.Effects.Add(effect);

            #endregion
        }

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            if (currentYear == 2008 && UseGRBSGroups)
            {
                // для 2008 года нет деления на группы
                return;
            }

            string query = DataProvider.GetQueryText("FO_0042_0002_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            string compareValue = String.Empty;
            bool allEqual = true;

            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    row[0] = UseComparabledDimension ? grbsDigest.GetShortName(row[0].ToString()) : DataDictionariesHelper.GetShortFMGRBSNames(row[0].ToString());
                }

                if (row[1] != DBNull.Value)
                {
                    if (compareValue == String.Empty)
                    {
                        compareValue = row[1].ToString();
                    }

                    allEqual = allEqual && (row[1].ToString() == compareValue);
                }
            }

            UltraChart.Data.ZeroAligned = allEqual;

            UltraChart.Series.Clear();
            for (int i = 1; i < dtChart.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                series.Label = dtChart.Columns[i].ColumnName;
                UltraChart.Series.Add(series);
            }
        }

        protected void UltraChart_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = (currentYear == 2008 && UseGRBSGroups)
                ? "Нет данных о подведомственной сети ГРБС"
                : "Нет данных";
            e.LabelStyle.FontColor = Color.Black;
            e.LabelStyle.FontSizeBestFit = false;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
        }

        protected void AVGChartDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0042_0002_avg_chart");
            dtChartAVG = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChartAVG);

            avgValue = GetDoubleDTValue(dtChartAVG, "Средняя оценка");
            firstGroupCount = Convert.ToInt32(GetDoubleDTValue(dtChartAVG, "Число ГРБС первой группы"));
            secondGroupCount = Convert.ToInt32(GetDoubleDTValue(dtChartAVG, "Число ГРБС второй группы"));
            thirdGroupCount = Convert.ToInt32(GetDoubleDTValue(dtChartAVG, "Число ГРБС третьей группы"));

            firstGroupAvg = GetDoubleDTValue(dtChartAVG, "Средняя оценка по первой группе");
            secondGroupAvg = GetDoubleDTValue(dtChartAVG, "Средняя оценка по второй группе");
            thirdGroupAvg = GetDoubleDTValue(dtChartAVG, "Средняя оценка по третьей группе");

            if (UseGRBSGroups && currentYear != 2008)
            {
                LegendChartBrick.Visible = true;

                string percent = IndicatorPercentFormat ? "%" : String.Empty;
                DataTable legendDt = new DataTable();
                
                Color color1 = Color.ForestGreen;
                Color color2 = Color.Red;
                Color color3 = Color.DeepSkyBlue;

                if (firstGroupCount != 0)
                {
                    legendDt.Columns.Add(new DataColumn(String.Format("Средняя оценка I группы: {0:N2}{1}", firstGroupAvg, percent),
                                                   typeof(Double)));
                    LegendChartBrick.Chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
                }


                if (secondGroupCount != 0)
                {
                    legendDt.Columns.Add(new DataColumn(String.Format("Средняя оценка II группы: {0:N2}{1}", secondGroupAvg, percent),
                                                   typeof(Double)));
                    LegendChartBrick.Chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
                }

                if (thirdGroupCount != 0)
                {
                    legendDt.Columns.Add(new DataColumn(String.Format("Средняя оценка III группы: {0:N2}{1}", thirdGroupAvg, percent),
                                                   typeof(Double)));
                    LegendChartBrick.Chart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color3, 150));
                }

                DataRow row = legendDt.NewRow();
                foreach (DataColumn column in legendDt.Columns)
                {
                    row[column.ColumnName] = 0;
                }
                legendDt.Rows.Add(row);

                LegendChartBrick.DataTable = legendDt;
                LegendChartBrick.DataBind();
            }
            else
            {
                LegendChartBrick.Visible = false;
            }
        }


        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }
        
        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text axisText = (Text)primitive;
                    axisText.bounds.Width = 30;
                    axisText.labelStyle.VerticalAlign = StringAlignment.Near;
                    axisText.labelStyle.FontSizeBestFit = false;
                    axisText.labelStyle.Font = new Font("Verdana", 8);
                    axisText.labelStyle.WrapText = false;
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        string grbsName = box.DataPoint.Label;
                        box.DataPoint.Label = UseComparabledDimension ? grbsDigest.GetFullName(grbsName) : DataDictionariesHelper.GetFullFMGRBSNames(grbsName);

                        if (UseGRBSGroups)
                        {
                            box.PE.ElementType = PaintElementType.Gradient;
                            box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;

                            int boxRow = box.Column;

                            if (boxRow < firstGroupCount)
                            {
                                box.PE.Fill = Color.LimeGreen;
                                box.PE.FillStopColor = Color.ForestGreen;
                            }
                            else if (boxRow < firstGroupCount + secondGroupCount)
                            {
                                box.PE.Fill = Color.OrangeRed;
                                box.PE.FillStopColor = Color.Red;
                            }
                            else
                            {
                                box.PE.Fill = Color.SkyBlue;
                                box.PE.FillStopColor = Color.DeepSkyBlue;
                            }
                        }
                    }
                }
            }
            
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            if (UseGRBSGroups)
            {
                double xMin = xAxis.MapMinimum;
                double xMax = xAxis.MapMaximum;
                double yMin = yAxis.MapMinimum;
                double yMax = yAxis.MapMaximum;

                double axisStep = (xAxis.Map(1) - xAxis.Map(0)) / 2;

                double beginLineAxisX = xAxis.Map(2 * firstGroupCount - 1) + axisStep;

                Line line = new Line();
                if (beginLineAxisX > xMin)
                {
                    line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                    line.PE.Stroke = Color.DarkGray;
                    line.PE.StrokeWidth = 2;
                    line.p1 = new Point((int) beginLineAxisX, (int) yMin);
                    line.p2 = new Point((int) beginLineAxisX, (int) yMax - avgTextHeight);
                    e.SceneGraph.Add(line);
                }
                else
                {
                    beginLineAxisX = xMin;
                }

                double endLineAxisX = xAxis.Map(2 * (firstGroupCount + secondGroupCount) - 1) + axisStep;
                line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                line.PE.Stroke = Color.DarkGray;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point((int)endLineAxisX, (int)yMin);
                line.p2 = new Point((int)endLineAxisX, (int)yMax - avgTextHeight);
                e.SceneGraph.Add(line);

                AddDashLine(e, (int)xAxis.MapMinimum, (int)beginLineAxisX, firstGroupAvg, false);
                AddDashLine(e, (int)beginLineAxisX, (int)endLineAxisX, secondGroupAvg, false);
                AddDashLine(e, (int)endLineAxisX, (int)xAxis.MapMaximum, thirdGroupAvg, false);

                LabelStyle labelStyle = new LabelStyle();
                labelStyle.HorizontalAlign = StringAlignment.Center;
                labelStyle.Font = new Font("Verdana", 8);
                labelStyle.FontColor = Color.Black;

                Text text = new Text();
                text.bounds = new Rectangle((int)xMin, (int)yMax - avgTextHeight - 20, (int)(beginLineAxisX - xMin), avgTextHeight);
                text.SetTextString("I группа (0 ПБС)");
                text.SetLabelStyle(labelStyle);
                e.SceneGraph.Add(text);

                text = new Text();
                text.bounds = new Rectangle((int)beginLineAxisX, (int)yMax - avgTextHeight - 20, (int)(endLineAxisX - beginLineAxisX), avgTextHeight);
                text.SetTextString("II группа (от 1 до 10 ПБС)");
                text.SetLabelStyle(labelStyle);
                e.SceneGraph.Add(text);

                text = new Text();
                text.bounds = new Rectangle((int)endLineAxisX, (int)yMax - avgTextHeight - 20, (int)(xMax - endLineAxisX), avgTextHeight);
                text.SetTextString("III группа (свыше 10 ПБС)");
                text.SetLabelStyle(labelStyle);
                e.SceneGraph.Add(text);
            }
            else
            {
                AddDashLine(e, (int)xAxis.MapMinimum, (int)xAxis.MapMaximum, avgValue, true);
            }
        }

        private void AddDashLine(FillSceneGraphEventArgs sceneGraphArgs, int lineStart, int lineEnd, double avg, bool textVisible)
        {
            IAdvanceAxis yAxis = (IAdvanceAxis)sceneGraphArgs.Grid["Y"];

            if (yAxis == null)
                return;

            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point(lineStart, (int)yAxis.Map(avg));
            line.p2 = new Point(lineEnd, (int)yAxis.Map(avg));
            sceneGraphArgs.SceneGraph.Add(line);

            if (textVisible)
            {
                Text text = new Text();
                text.PE.Fill = Color.Black;
                text.bounds = new Rectangle(lineEnd - agvTextWidth, ((int) yAxis.Map(avg)) - avgTextHeight, agvTextWidth,
                                            avgTextHeight);
                text.SetTextString(String.Format("Средняя оценка: {0:N2}{1}", avg, IndicatorPercentFormat ? "%" : String.Empty));
                sceneGraphArgs.SceneGraph.Add(text);
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 5;

            System.Drawing.Image chartImage = GetMergeChartsImage();
            UltraGridExporter.ImageExcelExport(e.CurrentWorksheet.Rows[2].Cells[0], chartImage, chartImage.Width, chartImage.Height);
        }
        
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(emptyExportGrid);
        }

        private Graphics g;

        private System.Drawing.Image GetMergeChartsImage()
        {
            System.Drawing.Image chartImg1 = GetChartImage(UltraChart);
            System.Drawing.Image chartImg2 = GetChartImage(LegendChartBrick.Chart);

            System.Drawing.Image img = new Bitmap(chartImg1.Width, chartImg1.Height + chartImg2.Height);
            g = Graphics.FromImage(img);

            g.DrawImage(chartImg1, 0, 0);
            g.DrawImage(chartImg2, 0, chartImg1.Height);

            return img;
        }

        private static System.Drawing.Image GetChartImage(UltraChart chart)
        {
            MemoryStream imageStream = new MemoryStream();
            chart.SaveTo(imageStream, ImageFormat.Png);
            return System.Drawing.Image.FromStream(imageStream);
        }
        
        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            UltraGridExporter1.PdfExporter.Export(new UltraWebGrid());
        }


        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            UltraChart.Width = Unit.Pixel((int) (CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 5;
            e.Section.AddImage(new Infragistics.Documents.Reports.Graphics.Image(GetMergeChartsImage()));
        }

        #endregion
    }
}
