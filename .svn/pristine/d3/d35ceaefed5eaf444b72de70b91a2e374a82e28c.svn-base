using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core;
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

namespace Krista.FM.Server.Dashboards.reports.FO_0039_0006
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChart;
        private DataTable dtIndicatorDetail;
        private int firstYear = 2010;
        private int endYear = 2011;

        private int selectedQuarterIndex;
        private string indicatorName;
        private double avgEvaluation;
        private string indicatorPeriod;

        #endregion

        private bool IsYearCompare
        {
            get { return selectedQuarterIndex == 4; }
        }

        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
        // выбранный индикатор
        private CustomParam selectedIndicator;
        // уровень районов
        private CustomParam regionsLevel;

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
            regionsLevel = UserParams.CustomParam("regions_level");

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
            CrossLink1.Text = "Результаты оценки качества МР";
            CrossLink1.NavigateUrl = "~/reports/FO_0039_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Динамика&nbsp;результатов&nbsp;оценки";
            CrossLink2.NavigateUrl = "~/reports/FO_0039_0002/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Рейтинг&nbsp;МР";
            CrossLink3.NavigateUrl = "~/reports/FO_0039_0003/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Сравн.характеристика&nbsp;мин.&nbsp;и&nbsp;макс.&nbsp;оценок";
            CrossLink4.NavigateUrl = "~/reports/FO_0039_0004/Default.aspx";

            CrossLink5.Visible = true;
            CrossLink5.Text = "Картограмма";
            CrossLink5.NavigateUrl = "~/reports/FO_0039_0005/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0039_0006_date");
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
                ComboQuarter.SetСheckedState(GetParamQuarter(quarter), true);

                ComboIndicator.Title = "Показатель";
                ComboIndicator.Width = 300;
                ComboIndicator.MultiSelect = false;
                ComboIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillQualityEvaluationIndicatorList(DataDictionariesHelper.QualityEvaluationIndicatorList));
                ComboIndicator.RemoveTreeNodeByName("Итоговая оценка уровня качества ОиОБП");
                ComboIndicator.RemoveTreeNodeByName("Рейтинг МО");
                ComboIndicator.RemoveTreeNodeByName("Средняя оценка уровня качества ОиОБП");
                ComboIndicator.RemoveTreeNodeByName("Степень качества ОиОБП");
                ComboIndicator.SetСheckedState("P1", true);
            }

            Page.Title = String.Format("Результаты оценки по отдельному показателю");
            PageTitle.Text = Page.Title;

            selectedQuarterIndex = ComboQuarter.SelectedIndex + 1;

            PageSubTitle.Text = (!IsYearCompare)
                ? String.Format("{0}.{1}", ComboQuarter.SelectedValue, ComboYear.SelectedValue)
                : String.Format("по итогам {0} года", ComboYear.SelectedValue);
            
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", selectedQuarterIndex);

            if (IsYearCompare)
            {
                selectedPeriod.Value = String.Format("[{0}]", UserParams.PeriodYear.Value);
            }
            else
            {
                selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);
            }

            selectedIndicator.Value = DataDictionariesHelper.QualityEvaluationIndicatorList[ComboIndicator.SelectedValue];
                        
            IndicatorDetailDataBind();
            chartElementCaption.Text = indicatorName == ComboIndicator.SelectedValue
                ? String.Format("Показатель «{0}»", indicatorName)
                : String.Format("Показатель «{0}» ({1})", indicatorName, ComboIndicator.SelectedValue);

            UltraChart.Legend.FormatString = String.Format("Среднее значение оценки: {0:N2}", avgEvaluation);

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
            string query = DataProvider.GetQueryText("FO_0039_0006_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    row[0] = row[0].ToString().Replace("_", "]");
                }
            }

            DataTable dtChartCopy = dtChart.Copy();
            if (dtChartCopy.Columns.Count > 2)
            {
                dtChartCopy.Columns.RemoveAt(2);
            }

            UltraChart.DataSource = dtChartCopy;
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
                        axisText.bounds.Width = 50;
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

                double axisStep = (xAxis.Map(1) -  xAxis.Map(0));
                double colIndex = avgEvaluation * 10;
                double lineX = xAxis.Map(colIndex) + avgEvaluation/axisStep;

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
            e.Text = (!IsYearCompare && indicatorPeriod.ToLower() == "год")
                    ? "Нет данных, т.к. показатель расчитывается только по итогам года"
                    : "Нет данных";
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
            string query = DataProvider.GetQueryText("FO_0039_0006_indicator_detail");
            dtIndicatorDetail = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtIndicatorDetail);

            indicatorName = GetStringDTValue(dtIndicatorDetail, "Наименование");
            avgEvaluation = GetDoubleDTValue(dtIndicatorDetail, "Средняя оценка уровня качества");
            indicatorPeriod = GetStringDTValue(dtIndicatorDetail, "Периодичность расчета показателя");
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
