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

namespace Krista.FM.Server.Dashboards.reports.FO_0039_0006_Kostroma
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChart;
        private int firstYear = 2009;
        private int endYear = 2011;
        private int selectedQuarterIndex;
        private int selectedYear;

        #endregion


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

            UltraChart.Axis.X.Extent = 180;
            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 1;
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

            UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value) / 2;
            UltraChart.Legend.Font = new Font("Verdana", 8);
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 6;

            

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
            CrossLink5.Text = "Рейтинг&nbsp;МР(ГО)";
            CrossLink5.NavigateUrl = "~/reports/FO_0039_0003_Kostroma/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            evaluationIndicatorDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0039_0003_QualityEvaluationIndicatorList");

            if (!Page.IsPostBack)
            {
               
                //chartWebAsyncPanel.AddRefreshTarget(UltraChart);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0039_0006_Kostroma_date");
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

            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedQuarterIndex = ComboQuarter.SelectedIndex + 1;
            
            string currentDate = (!IsYearCompare)
                ? String.Format("{0}.{1}", ComboQuarter.SelectedValue, ComboYear.SelectedValue)
                : String.Format("по итогам {0} года", ComboYear.SelectedValue);

            Page.Title = String.Format("Диаграмма динамики результатов оценки качества организации и осуществления бюджетного процесса в муниципальных образованиях Костромской области");
            PageTitle.Text = Page.Title;
            chart1Label.Text = String.Format("Диаграмма динамики результатов оценки {0}", currentDate);
            PageSubTitle.Text = String.Format("{0} {1}", ComboQualityEvaluationIndicator.SelectedValue, currentDate);
             
            string period = String.Empty;
   
            if (IsYearCompare)
            {
                for (int i = 2; i >= 0; i--)
                {
                    period += String.Format("[Период].[Период].[Данные всех периодов].[{0}], ", selectedYear - i);
                }
            }
            else
            {
                for (int i = selectedQuarterIndex-1; i >= 0; i--)
                {
                    
                    
                    period += String.Format("[Период].[Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}], ", selectedYear, CRHelper.HalfYearNumByQuarterNum(
                                                                        selectedQuarterIndex - i),
                                            selectedQuarterIndex - i);
                }
            }
            period = period.Remove(period.Length - 2, 1);
            selectedPeriod.Value = string.Format("{0}", period);
            


                
            
            selectedIndicator.Value = evaluationIndicatorDigest.GetMemberUniqueName(ComboQualityEvaluationIndicator.SelectedValue);

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
            string query = DataProvider.GetQueryText("FO_0039_0006_Kostroma_chart");
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

            UltraChart.DataSource = dtChart;

            //UltraChart.Series.Clear();
           // for (int i = 1; i < dtChart.Columns.Count; i++)
           // {
           //     NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
          //      series.Label = dtChart.Columns[i].ColumnName;
          //      UltraChart.Series.Add(series);
           // }
            
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
