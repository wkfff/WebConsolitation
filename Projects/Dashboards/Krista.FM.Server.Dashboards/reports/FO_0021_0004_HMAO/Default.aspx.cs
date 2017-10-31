using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;

using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;

using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Core;

namespace Krista.FM.Server.Dashboards.reports.FO_0021_0004_HMAO
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChart;
        private DataTable dtMax;
        private DataTable dtGrid;
        private DataTable dtIndicatorDetail;
        private int firstYear = 2010;
        private int endYear = 2011;
        private int maxValue = 1;

        private string indicatorName;
        private double avgEvaluation;
        private string selectedIndicatorCode;

        private static MemberAttributesDigest indicatorDigest;

        #endregion

        private bool IsRomanNumeral()
        {
            return ((selectedIndicatorCode == "Бплан") || (selectedIndicatorCode == "Бисп") || (selectedIndicatorCode == "ДП") || (selectedIndicatorCode == "Оказ-еМУ") || (selectedIndicatorCode == "ОткБП"));
        }

        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
        // выбранный индикатор
        private CustomParam selectedIndicator;
        // имя выбранного индикатора
        private CustomParam selectedIndicatorCaption;
        // максимальное значение индикатора
        private CustomParam indicatorMaxValue;

        #endregion
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.75);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChart_InvalidDataReceived);

            #region Инициализация параметров запроса

            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }
            if (selectedIndicator == null)
            {
                selectedIndicator = UserParams.CustomParam("selected_indicator");
            }
            selectedIndicatorCaption = UserParams.CustomParam("selected_indicator_caption");
            indicatorMaxValue = UserParams.CustomParam("max_value");
        
            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.ColumnChart.ColumnSpacing = 0;
            UltraChart.ColumnChart.SeriesSpacing = 0;
            UltraChart.Data.ZeroAligned = true;
            UltraChart.Border.Thickness = 0;

            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.Y.Extent = 20;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart.Width.Value) / 4;
            UltraChart.Legend.Font = new Font("Verdana", 8);
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 6;
            UltraChart.Legend.FormatString = "Среднее";

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Text = "Число МР";
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);

            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleBottom.Margins.Left = UltraChart.Axis.Y.Extent;
            UltraChart.TitleBottom.Text = "Оценки";
            UltraChart.TitleBottom.Font = new Font("Verdana", 8);

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N0>";
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart.ColumnChart.ChartText.Add(appearance);

            UltraChart.Tooltips.FormatString = "Оценка качества в интервале <SERIES_LABEL> наблюдается у <DATA_VALUE:N0> МО\n<ITEM_LABEL>";

            #endregion

            UltraGridExporter1.ExcelExportButton.Visible = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;оценки";
            CrossLink1.NavigateUrl = "~/reports/FO_0021_0001_HMAO/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Рейтинг&nbsp;МО";
            CrossLink2.NavigateUrl = "~/reports/FO_0021_0002_HMAO/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Картограмма&nbsp;с&nbsp;результатами&nbsp;оценки";
            CrossLink3.NavigateUrl = "~/reports/FO_0021_0003_HMAO/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Мониторинг&nbsp;соблюдения&nbsp;бюджетного&nbsp;законодательства";
            CrossLink4.NavigateUrl = "~/reports/FO_0021_0005_HMAO/Default.aspx";

           
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0021_0004_HMAO_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                selectedIndicatorCaption.Value = "1.3 Исполнение бюджета муниципального образования Ханты-Мансийского автономного округа – Югры (далее – муниципальное образование) по доходам без учета безвозмездных поступлений";

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                
            }

            Page.Title = String.Format("Анализ показателей оценки качества организации и осуществления бюджетного процесса в муниципальных образованиях Ханты-Мансийского автономного округа - Югры");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("По итогом {0} года", ComboYear.SelectedValue);

            selectedPeriod.Value = String.Format("{0}", ComboYear.SelectedValue);

            if (ComboIndicator.SelectedValue != String.Empty)
            {
                selectedIndicatorCaption.Value = ComboIndicator.SelectedValue;
            }

            ComboIndicator.Title = "Показатель";
            ComboIndicator.Width = 600;
            ComboIndicator.MultiSelect = false;
            indicatorDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0021_0004_HMAO_indicatorList");
            ComboIndicator.ClearNodes();
            ComboIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(indicatorDigest.UniqueNames, indicatorDigest.MemberLevels));
            ComboIndicator.SelectLastNode();
            ComboIndicator.SetСheckedState(selectedIndicatorCaption.Value, true);
           
            selectedIndicator.Value = indicatorDigest.GetMemberUniqueName(ComboIndicator.SelectedValue);

            selectedIndicatorCode = indicatorDigest.GetShortName(ComboIndicator.SelectedValue);
            IndicatorDetailDataBind();          
            chartElementCaption.Text = String.Format("Показатель «{0}»", indicatorName);

            
            UltraChart.Legend.FormatString = String.Format("Среднее значение оценки: {0:N2}", avgEvaluation);
            UltraChart.DataBind();
        }

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string queryChart = string.Empty;
            if (IsRomanNumeral())
            {
                string query = DataProvider.GetQueryText("FO_0021_0004_HMAO_grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtGrid);
                
                string queryMax = DataProvider.GetQueryText("FO_0021_0004_HMAO_indicator_max_value");
                dtMax = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryMax, "Dummy", dtMax);
                maxValue = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dtMax.Rows[0][0])));

                //dtChart = new DataTable();
                dtChart = new DataTable();
                dtChart.Columns.Add(new DataColumn("диапазон", typeof(string)));
                dtChart.Columns.Add(new DataColumn("количество", typeof(int)));
                dtChart.Columns.Add(new DataColumn("хинт", typeof(string)));
                int i;
                for (i = 0; i < maxValue; i++)
                {
                    int count = 0;
                    string hint = string.Empty;
                    for (int j = 0; j < dtGrid.Rows.Count; j++)
                    {
                        if ((Convert.ToDouble(dtGrid.Rows[j][1]) >= Convert.ToDouble(i)) && (Convert.ToDouble(dtGrid.Rows[j][1]) < Convert.ToDouble(i+1)))
                        {
                            count++;
                            hint += string.Format("{0}, ", dtGrid.Rows[j][0].ToString());
                        }
                    }
                    if (hint.Length > 0)
                    {
                        hint = hint.Remove(hint.Length - 2, 2);
                    }
                    
                    DataRow row;
                    row = dtChart.NewRow();
                    row[0] = string.Format("[{0},0:{1},0)", i, i + 1); 
                    row[1] = count;
                    row[2] = hint;
                    dtChart.Rows.Add(row);
                }
                dtChart.Rows[maxValue-1][0] = string.Format("[{0},0:{1},0]", maxValue - 1, maxValue); 
            }
            else
            {
                queryChart = DataProvider.GetQueryText("FO_0021_0004_HMAO_chart");
                dtChart = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryChart, "Dummy", dtChart);
                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("_", "]");
                    }
                }
            }

            UltraChart.DataSource = dtChart;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            if (dtChart.Rows.Count > 0)
            {
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                    {
                        Text axisText = (Text) primitive;
                        axisText.bounds.Width = 80;
                        axisText.labelStyle.HorizontalAlign = StringAlignment.Center;
                        axisText.labelStyle.FontSizeBestFit = false;
                        axisText.labelStyle.Font = new Font("Verdana", 8);
                        axisText.labelStyle.WrapText = false;
                    }
                    if (primitive is Box)
                    {
                        Box box = (Box) primitive;
                        if (box.DataPoint != null && box.Value != null)
                        {
                            int rowIndex = box.Row;
                            int columnIndex = box.Column + 2;

                            string indicatorList = String.Empty;
                            if (dtChart != null && dtChart.Rows[rowIndex][columnIndex] != DBNull.Value &&
                                dtChart.Rows[rowIndex][columnIndex].ToString() != String.Empty)
                            {
                                string list = String.Format("({0})", dtChart.Rows[rowIndex][columnIndex].ToString().TrimEnd(','));
                                list = BreakCollocator(list, ',', 5);
                                indicatorList = list.Replace(",", ", ");
                            }

                            box.DataPoint.Label = indicatorList;
                        }
                        else if (box.Path != null && box.Path.ToLower().Contains("legend") && box.rect.Width < 20)
                        {
                            box.PE.Fill = Color.Red;
                            box.PE.FillStopColor = Color.Firebrick;
                            box.rect = new Rectangle(box.rect.X, box.rect.Y + box.rect.Height/3, box.rect.Width, box.rect.Height / 3);
                        }
                    }
                }


                IAdvanceAxis xAxis = (IAdvanceAxis) e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis) e.Grid["Y"];

                if (xAxis == null || yAxis == null)
                    return;

                double yMin = yAxis.MapMinimum;
                double yMax = yAxis.MapMaximum;

                double axisStep;
                double colIndex;
                double lineX;
                if (IsRomanNumeral())
                {
                    axisStep = (xAxis.Map(maxValue) - xAxis.Map(0));
                    colIndex = avgEvaluation;
                    lineX = xAxis.Map(colIndex) + avgEvaluation / axisStep;
                }
                else
                {
                    axisStep = (xAxis.Map(1) - xAxis.Map(0));
                    colIndex = avgEvaluation * 10;
                    lineX = xAxis.Map(colIndex) + avgEvaluation/axisStep;
                }

//                Box avgBox = new Box(new Rectangle(lineX, (int)yAxis.MapMaximum, 5, 20));
//                avgBox.PE.ElementType = PaintElementType.SolidFill;
//                
////                avgBox.Layer = e.ChartCore.GetChartLayer();
////                avgBox.Value = avgBox.Row = avgBox.Column = -1;
////                avgBox.Caps = PCaps.HitTest | PCaps.Tooltip;
//                
//                avgBox.PE.Fill = Color.Red;
//                e.SceneGraph.Add(avgBox);

                Line line = new Line();
                line.p1 = new Point((int)lineX, (int)yMax);
                line.PE.Stroke = Color.Firebrick;
                line.PE.StrokeWidth = 5;
                line.p2 = new Point((int)lineX, (int)yMin);
                e.SceneGraph.Add(line);
            }
        }

        void UltraChart_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "Нет данных";
            e.LabelStyle.FontColor = Color.Black;
            e.LabelStyle.FontSizeBestFit = false;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
        }

        private static string BreakCollocator(string source, char breakChar, int charIndex)
        {
            string breakedStr = String.Empty;

            int charCount = 0;
            foreach (char ch in source)
            {
                breakedStr += ch;
                if (ch == breakChar)
                {
                    charCount++;
                    if (charCount == charIndex)
                    {
                        breakedStr += "\n";
                        charCount = 0;
                    }
                }
            }
            
            return breakedStr;
        }

        #endregion

        #region Параметры индикатора

        protected void IndicatorDetailDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0021_0004_HMAO_indicator_detail");
            dtIndicatorDetail = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtIndicatorDetail);

            indicatorName = GetStringDTValue(dtIndicatorDetail, "Наименование");
            avgEvaluation = GetDoubleDTValue(dtIndicatorDetail, "Средняя оценка уровня качества");
           
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != String.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        private static string GetStringDTValue(DataTable dt, string columnName)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != String.Empty)
            {
                return dt.Rows[0][columnName].ToString();
            }
            return String.Empty;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = chartElementCaption.Text;

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 5;

            UltraGridExporter.ChartExcelExport(e.CurrentWorksheet.Rows[3].Cells[0], UltraChart);
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(emptyExportGrid);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            
            Report report = new Report();
            ISection section = report.AddSection();

            UltraGridExporter1.PdfExporter.Export(new UltraWebGrid(), section);
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

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chartElementCaption.Text);

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 5;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
