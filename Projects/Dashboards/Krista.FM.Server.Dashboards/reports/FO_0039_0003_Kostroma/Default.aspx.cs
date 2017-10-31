using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0039_0003_Kostroma
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChart;
        private DataTable dtChartAVG;
        private int firstYear = 2010;
        private int endYear = 2011;
        private int selectedQuarterIndex;
        private int selectedYear;

        private double beginQualityLimit;
        private double endQualityLimit;
        private double avgValue;
        private string avgCaption;
        private int avgCaptionWidth;

        private int beginQualityIndex;
        private int endQualityIndex;

        private double maxEvaluation;

        #endregion

        private bool UseQualityDegree
        {
            get { return WithQualityDegree.Checked && !IsGroupSelected; }
        }

        private bool IsGroupSelected
        {
            get { return ComboQualityEvaluationIndicator.SelectedValue.Contains("группа"); }
        }

        private bool IsYearCompare
        {
            get { return selectedQuarterIndex == 4; }
        }

        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
        // уровень районов
        private CustomParam regionsLevel;
        // выбранный индикатор
        private CustomParam selectedIndicator;

        #endregion

        private MemberAttributesDigest evaluationIndicatorDigest;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.7);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChart_InvalidDataReceived);

            #region Инициализация параметров запроса

            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }
            regionsLevel = UserParams.CustomParam("regions_level");
            selectedIndicator = UserParams.CustomParam("selected_indicator");

            #endregion

            #region Настройка диаграммы

            UltraChart.Data.ZeroAligned = true;

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;

            UltraChart.ColumnChart.SeriesSpacing = 1;
            UltraChart.ColumnChart.ColumnSpacing = 1;

            UltraChart.Axis.X.Extent = 175;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
            UltraChart.Axis.Y.Extent = 20;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);
            
            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value) / 4;
            UltraChart.TitleLeft.Text = "Баллы";
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);

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

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:N2>";

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты оценки качества МР(ГО)";
            CrossLink1.NavigateUrl = "~/reports/FO_0039_0001_Kostroma/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Динамика&nbsp;результатов&nbsp;оценки";
            CrossLink2.NavigateUrl = "~/reports/FO_0039_0002_Kostroma/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Картограмма";
            CrossLink3.NavigateUrl = "~/reports/FO_0039_0005_Kostroma/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Диаграмма&nbsp;динамики&nbsp;по&nbsp;отд.показателю";
            CrossLink4.NavigateUrl = "~/reports/FO_0039_0007_Kostroma/Default.aspx";

            CrossLink5.Visible = true;
            CrossLink5.Text = "Диаграмма&nbsp;динамики&nbsp;результатов&nbsp;оценки";
            CrossLink5.NavigateUrl = "~/reports/FO_0039_0006_Kostroma/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            evaluationIndicatorDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0039_0003_QualityEvaluationIndicatorList");

            if (!Page.IsPostBack)
            {
                WithoutQualityDegree.Attributes.Add("onclick", string.Format("uncheck('{0}')", WithQualityDegree.ClientID));
                WithQualityDegree.Attributes.Add("onclick", string.Format("uncheck('{0}')", WithoutQualityDegree.ClientID));

                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(WithoutQualityDegree.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(WithQualityDegree.ClientID);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0039_0003_Kostroma_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                string quarter = "Квартал 4";
                if (dtDate.Rows.Count > 0)
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                    quarter = dtDate.Rows[0][2].ToString();
                }

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Оценка качества";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillDateQuarters());
                ComboQuarter.SetСheckedState(GetParamQuarter(quarter), true);
                
                ComboQualityEvaluationIndicator.Title = "Показатель";
                ComboQualityEvaluationIndicator.Width = 500;
                ComboQualityEvaluationIndicator.MultiSelect = false;
                ComboQualityEvaluationIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(evaluationIndicatorDigest.UniqueNames, evaluationIndicatorDigest.MemberLevels));
                ComboQualityEvaluationIndicator.SetСheckedState("Итоговая оценка", true);
            }

            WithQualityDegree.Visible = !IsGroupSelected;
            WithoutQualityDegree.Visible = !IsGroupSelected;

            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedQuarterIndex = ComboQuarter.SelectedIndex + 1;
            
            string currentDate = (!IsYearCompare)
                ? String.Format("{0}.{1}", ComboQuarter.SelectedValue, ComboYear.SelectedValue)
                : String.Format("по итогам {0} года", ComboYear.SelectedValue);

            Page.Title = String.Format("Рейтинг муниципальных районов (городского округа) по результатам оценки качества");
            PageTitle.Text = Page.Title;
            chart1Label.Text =  String.Format("Результаты проведения оценки качества ОиОБП в МР(ГО) Костромской области {0}", currentDate);
            PageSubTitle.Text = String.Format("{0} {1}", ComboQualityEvaluationIndicator.SelectedValue, currentDate);
            
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", selectedQuarterIndex);

            if (IsYearCompare)
            {
                selectedPeriod.Value = String.Format("[{0}]", selectedYear);
            }
            else
            {
                selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", selectedYear, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);
            }

            selectedIndicator.Value = evaluationIndicatorDigest.GetMemberUniqueName(ComboQualityEvaluationIndicator.SelectedValue);

            AVGChartDataBind();
            
            UltraChart.DataBind();
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

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0039_0003_Kostroma_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            foreach (DataRow row in dtChart.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i == 0 && row[i] != DBNull.Value)
                    {
                        row[i] = row[i].ToString().Replace("муниципальный район", "МР");
                        row[i] = row[i].ToString().Replace("Муниципальный район", "МР");
                        row[i] = row[i].ToString().Replace("\"", "'");
                        row[i] = row[i].ToString().Replace(" район", " р-н");
                    }
                }
            }

            //UltraChart.DataSource = dtChart;

            UltraChart.Series.Clear();
            for (int i = 1; i < dtChart.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                series.Label = dtChart.Columns[i].ColumnName;
                UltraChart.Series.Add(series);
            }

            if (dtChart != null)
            {
                int currentDegree = 0;
                beginQualityIndex = -1;
                endQualityIndex = dtChart.Rows.Count - 1;
                for (int i = 0; i < dtChart.Rows.Count; i++)
                {
                    if (dtChart.Rows[i][1] != DBNull.Value && dtChart.Rows[i][1].ToString() != String.Empty)
                    {
                        double value = Convert.ToDouble(dtChart.Rows[i][1]);

                        if (i == 0 || currentDegree < GetQualityDegree(value))
                        {
                            currentDegree = GetQualityDegree(value);

                            if (i != 0)
                            {
                                if (currentDegree == 2)
                                {
                                    beginQualityIndex = i - 1;
                                }
                                else
                                {
                                    endQualityIndex = i - 1;
                                }
                            }
                        }
                    }
                }
            }
        }

        private int GetQualityDegree(double value)
        {
            if (value > beginQualityLimit)
            {
                return 1;
            }
            if (value <= endQualityLimit)
            {
                return 3;
            }
            return 2;
        }

        protected void UltraChart_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = (!IsYearCompare && IsGroupSelected)
                    ? "Нет данных, т.к. показатели расчитываются только по итогам года"
                    : "Нет данных";
            e.LabelStyle.FontColor = Color.Black;
            e.LabelStyle.FontSizeBestFit = false;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
        }

        protected void AVGChartDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0039_0003_Kostroma_avg_chart");
            dtChartAVG = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChartAVG);
            
            beginQualityLimit = IsYearCompare ? GetDoubleDTValue(dtChartAVG, "Начальная граница интервала для года") : GetDoubleDTValue(dtChartAVG, "Начальная граница интервала для квартала");
            endQualityLimit = IsYearCompare ? GetDoubleDTValue(dtChartAVG, "Конечная граница интервала для года") : GetDoubleDTValue(dtChartAVG, "Конечная граница интервала для квартала");
            maxEvaluation = IsYearCompare ? GetDoubleDTValue(dtChartAVG, "Максимальная оценка для года") : GetDoubleDTValue(dtChartAVG, "Максимальная оценка для квартала");

            if (IsGroupSelected)
            {
                avgValue = maxEvaluation;
                avgCaption = "Максимальная оценка по данному направлению";

                UltraChart.Axis.Y.RangeType = AxisRangeType.Custom;
                UltraChart.Axis.Y.RangeMax = maxEvaluation;
                UltraChart.TitleLeft.Text = "Оценка";

                avgCaptionWidth = 400;
            }
            else
            {
                avgValue = GetDoubleDTValue(dtChartAVG, "Средняя оценка уровня качества");
                avgCaption = "Средняя оценка";

                UltraChart.Axis.Y.RangeType = AxisRangeType.Automatic;
                UltraChart.TitleLeft.Text = "Баллы";

                avgCaptionWidth = 200;
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
                if (UseQualityDegree && primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        box.PE.ElementType = PaintElementType.Gradient;
                        box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;

                        double value = Convert.ToDouble(box.Value);
                        if (value > beginQualityLimit)
                        {
                            box.PE.Fill = Color.LimeGreen;
                            box.PE.FillStopColor = Color.ForestGreen;
                        }
                        else if (value <= endQualityLimit)
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
            
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int textHeight = 12;
            double lineStart = xAxis.MapMinimum;
            double lineLength = xAxis.MapMaximum;

            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point((int)lineStart, (int)yAxis.Map(avgValue));
            line.p2 = new Point((int)lineStart + (int)lineLength, (int)yAxis.Map(avgValue));
            e.SceneGraph.Add(line);

            Text text = new Text();
            text.PE.Fill = Color.Black;
            text.labelStyle.Font = new Font("Verdana", 8);
            text.bounds = new Rectangle((int)lineLength - avgCaptionWidth, ((int)yAxis.Map(avgValue)) - textHeight, avgCaptionWidth, textHeight);
            text.SetTextString(String.Format("{1}: {0:N2}", avgValue, avgCaption));
            e.SceneGraph.Add(text);

            if (UseQualityDegree)
            {
                double xMin = xAxis.MapMinimum;
                double xMax = xAxis.MapMaximum;
                double yMin = yAxis.MapMinimum;
                double yMax = yAxis.MapMaximum;

                double axisStep = (xAxis.Map(1) - xAxis.Map(0)) / 2;

                double beginLineAxisX = xAxis.Map(2 * beginQualityIndex + 1) + axisStep;
                line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                line.PE.Stroke = Color.DarkGray;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point((int)beginLineAxisX, (int)yMin);
                line.p2 = new Point((int)beginLineAxisX, (int)yMax - textHeight);
                if (beginLineAxisX >= xMin && beginLineAxisX <= xMax)
                {
                    e.SceneGraph.Add(line);
                }

                double endLineAxisX = xAxis.Map(2 * endQualityIndex + 1) + axisStep;
                line = new Line();
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                line.PE.Stroke = Color.DarkGray;
                line.PE.StrokeWidth = 2;
                line.p1 = new Point((int)endLineAxisX, (int)yMin);
                line.p2 = new Point((int)endLineAxisX, (int)yMax - textHeight);
                if (endLineAxisX >= xMin && endLineAxisX <= xMax)
                {
                    e.SceneGraph.Add(line);
                }

                LabelStyle labelStyle = new LabelStyle();
                labelStyle.HorizontalAlign = StringAlignment.Center;
                labelStyle.Font = new Font("Verdana", 8);
                labelStyle.FontColor = Color.Black;

                text = new Text();
                text.bounds = new Rectangle((int)xMin, (int)yMax - textHeight, (int)(beginLineAxisX - xMin), textHeight);
                text.SetTextString("I степень");
                text.SetLabelStyle(labelStyle);
                e.SceneGraph.Add(text);

                text = new Text();
                text.bounds = new Rectangle((int)beginLineAxisX, (int)yMax - textHeight, (int)(endLineAxisX - beginLineAxisX), textHeight);
                text.SetTextString("II степень");
                text.SetLabelStyle(labelStyle);
                e.SceneGraph.Add(text);

                text = new Text();
                text.bounds = new Rectangle((int)endLineAxisX, (int)yMax - textHeight, (int)(xMax - endLineAxisX), textHeight);
                text.SetTextString("III степень");
                text.SetLabelStyle(labelStyle);
                e.SceneGraph.Add(text);
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            
            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            ReportExcelExporter1.SheetColumnCount = 12;
            ReportExcelExporter1.Export(UltraChart, chart1Label.Text, 3);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            ReportPDFExporter1.Export(UltraChart);
        }

        #endregion
    }
}
