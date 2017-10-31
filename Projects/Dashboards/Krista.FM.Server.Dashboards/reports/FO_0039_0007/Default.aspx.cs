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

namespace Krista.FM.Server.Dashboards.reports.FO_0039_0007
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
        private string selectedIndicatorName;
        private string indicatorPeriod;
        private string indicatorUnit;
        
        #endregion

        private bool IsYearCompare
        {
            get { return selectedQuarterIndex == 4; }
        }

        public bool ValueSelected
        {
            get { return MeasureButtonList.SelectedIndex == 1; }
        }

        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
        // выбранный индикатор
        private CustomParam selectedIndicator;
        // выбраная мера
        private CustomParam selectedMeasure;
        // множество периодов
        private CustomParam periodSet;
        // уровень районов
        private CustomParam regionsLevel;

        #endregion
        
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
            if (selectedIndicator == null)
            {
                selectedIndicator = UserParams.CustomParam("selected_indicator");
            }
            if (selectedMeasure == null)
            {
                selectedMeasure = UserParams.CustomParam("selected_measure");
            }
            if (periodSet == null)
            {
                periodSet = UserParams.CustomParam("period_set");
            }
            regionsLevel = UserParams.CustomParam("regions_level");

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.LineChart;
            UltraChart.LineChart.NullHandling = NullHandling.DontPlot;
            UltraChart.ColumnChart.ColumnSpacing = 0;
            UltraChart.ColumnChart.SeriesSpacing = 0;
            UltraChart.Border.Thickness = 0;

            UltraChart.Axis.X.Extent = 140;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.Y.Extent = 35;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value) / 4;
            UltraChart.Legend.Font = new Font("Verdana", 8);
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 6;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Text = "";
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.Thickness = 2;
            lineAppearance.IconAppearance.Icon = SymbolIcon.Diamond;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            UltraChart.LineChart.LineAppearances.Add(lineAppearance);

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N1>";
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart.LineChart.ChartText.Add(appearance);

            UltraChart.Tooltips.FormatString = String.Format("<ITEM_LABEL>\n<SERIES_LABEL>\n{0}: <DATA_VALUE:N1>", 
                ValueSelected ? "значение показателя" : "оценка показателя");

            #endregion

            UltraGridExporter1.ExcelExportButton.Visible = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;оценки&nbsp;качества&nbsp;МР";
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
            CrossLink5.Text = "Результаты&nbsp;оценки&nbsp;по&nbsp;отд.показателю";
            CrossLink5.NavigateUrl = "~/reports/FO_0039_0006/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0039_0007_date");
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

                ComboQualityEvaluationIndicator.Title = "Показатель";
                ComboQualityEvaluationIndicator.Width = 300;
                ComboQualityEvaluationIndicator.MultiSelect = false;
                ComboQualityEvaluationIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillQualityEvaluationIndicatorList(DataDictionariesHelper.QualityEvaluationIndicatorList));
                ComboQualityEvaluationIndicator.SetСheckedState("Итоговая оценка уровня качества ОиОБП", true);

                ComboQualityValueIndicator.Title = "Показатель";
                ComboQualityValueIndicator.Width = 300;
                ComboQualityValueIndicator.MultiSelect = false;
                ComboQualityValueIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillQualityValueIndicatorList(DataDictionariesHelper.QualityValueIndicatorList));
                ComboQualityValueIndicator.SetСheckedState("P1", true);
            }

            Page.Title = String.Format("Динамика результатов оценки качества организации бюджетного процесса в МР Омской области по отдельному показателю");
            PageTitle.Text = Page.Title;

            selectedQuarterIndex = ComboQuarter.SelectedIndex + 2;

            PageSubTitle.Text = (!IsYearCompare)
                ? String.Format("{0}.{1}", ComboQuarter.SelectedValue, ComboYear.SelectedValue)
                : String.Format("по итогам {0} года", ComboYear.SelectedValue);
            
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", selectedQuarterIndex);

            selectedMeasure.Value = ValueSelected ? "Значение" : "Оценка показателя";

            if (IsYearCompare)
            {
                periodSet.Value = "Годы";
                selectedPeriod.Value = String.Format("[{0}]", UserParams.PeriodYear.Value);
            }
            else
            {
                periodSet.Value = "Кварталы";
                selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);
            } 
            
            if (ValueSelected)
            {
                valueComboTD.Visible = true;
                evaluationComboTD.Visible = false;

                selectedIndicatorName = ComboQualityValueIndicator.SelectedValue;
                selectedIndicator.Value = DataDictionariesHelper.QualityValueIndicatorList[ComboQualityValueIndicator.SelectedValue];
                ComboQualityEvaluationIndicator.SetСheckedState(selectedIndicatorName, true);
            }
            else
            {
                valueComboTD.Visible = false;
                evaluationComboTD.Visible = true;

                selectedIndicatorName = ComboQualityEvaluationIndicator.SelectedValue;
                selectedIndicator.Value = DataDictionariesHelper.QualityEvaluationIndicatorList[ComboQualityEvaluationIndicator.SelectedValue];
                ComboQualityValueIndicator.SetСheckedState(selectedIndicatorName, true);
            }
            
            IndicatorDetailDataBind();
            chartElementCaption.Text = indicatorName == selectedIndicatorName
                ? String.Format("Показатель «{0}»", indicatorName)
                : String.Format("Показатель «{0}» ({1})", indicatorName, selectedIndicatorName);
            UltraChart.TitleLeft.Text = indicatorUnit;

            // при выбранном 4м квартале и для первого года в списке параметра данных не выводим
            if (!(IsYearCompare && firstYear.ToString() == ComboYear.SelectedValue))
            {
                UltraChart.DataBind();
            }
            else
            {
                UltraChart.DataSource = null;
            }
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
            string query = DataProvider.GetQueryText("FO_0039_0007_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    if (IsYearCompare)
                    {
                        row[0] = String.Format("по итогам {0} года", row[0]); 
                    }
                    else
                    {
                        row[0] = String.Format("{0}.{1}", GetParamQuarter(row[0].ToString()), ComboYear.SelectedValue);
                    }
                }
            }

            UltraChart.DataSource = dtChart;
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

        void UltraChart_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            if (IsYearCompare && firstYear.ToString() == ComboYear.SelectedValue)
            {
                e.Text = "Расчет динамики невозможен, т.к. нет данных по оценке качества за предыдущий финансовый год";
            }
            else if (!IsYearCompare && indicatorPeriod.ToLower() == "год")
            {
                e.Text = "Нет данных, т.к. показатель расчитывается только по итогам года";
            } 
            else
            {
                e.Text = "Нет данных";
            }

            e.LabelStyle.FontColor = Color.Black;
            e.LabelStyle.FontSizeBestFit = false;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
        }

        #endregion

        #region Параметры индикатора

        protected void IndicatorDetailDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0039_0007_indicator_detail");
            dtIndicatorDetail = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtIndicatorDetail);

            indicatorName = GetStringDTValue(dtIndicatorDetail, "Наименование");
            indicatorPeriod = GetStringDTValue(dtIndicatorDetail, "Периодичность расчета показателя");
            indicatorUnit = ValueSelected ? GetStringDTValue(dtIndicatorDetail, "Единицы измерения") : "Баллы";
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
