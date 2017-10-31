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

namespace Krista.FM.Server.Dashboards.reports.FO_0040_0004
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
        private int selectedYear;
        private string indicatorName;
        private string indicatorFormat;
        private string selectedIndicatorName;
        
        #endregion

        private bool IsFirstQuarter
        {
            get { return selectedQuarterIndex == 1; }
        }

        private bool IsRating
        {
            get { return indicatorName == "Рейтинг"; }
        }

        private bool IsAVGEvaluation
        {
            get { return indicatorName == "Средняя оценка"; }
        }


        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
        // выбранный предыдущий период
        private CustomParam selectedPrevPeriod;
        // выбранный индикатор
        private CustomParam selectedIndicator;
        // множество периодов
        private CustomParam periodSet;

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
            if (selectedPrevPeriod == null)
            {
                selectedPrevPeriod = UserParams.CustomParam("selected_prev_period");
            }
            if (periodSet == null)
            {
                periodSet = UserParams.CustomParam("period_set");
            }
            
            #endregion

            UltraGridExporter1.ExcelExportButton.Visible = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            
            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;мониторинга&nbsp;качества ";
            CrossLink1.NavigateUrl = "~/reports/FO_0040_0001/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Динамика&nbsp;основных&nbsp;показателей";
            CrossLink2.NavigateUrl = "~/reports/FO_0040_0002/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Картограмма&nbsp;результатов&nbsp;мониторинга";
            CrossLink3.NavigateUrl = "~/reports/FO_0040_0003/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0040_0004_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillMonitoringQuarters());
                ComboQuarter.SetСheckedState(GetParamQuarter(quarter), true);

                ComboIndicator.Title = "Показатель";
                ComboIndicator.Width = 300;
                ComboIndicator.MultiSelect = false;
                ComboIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillQualityEvaluationIndicatorList(DataDictionariesHelper.QualityEvaluationIndicatorList));
                ComboIndicator.SetСheckedState("Рейтинг", true);
            }

            selectedYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedQuarterIndex = ComboQuarter.SelectedIndex + 1;

            Page.Title = String.Format("Динамика основных показателей мониторинга качества управления бюджетным процессом и соблюдения требований бюджетного законодательства РФ муниципальными образованиями Самарской области");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("по итогам {0} квартала {1} года", selectedQuarterIndex, selectedYear);
            
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByQuarterNum(selectedQuarterIndex));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", selectedQuarterIndex);

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);
            selectedPrevPeriod.Value = String.Format("[{0}].[Полугодие 2].[Квартал 4]", selectedYear - 1);
            periodSet.Value = (IsFirstQuarter) ? "Текущий и предыдущий кварталы" : "Кварталы";

            selectedIndicatorName = ComboIndicator.SelectedValue;
            selectedIndicator.Value = DataDictionariesHelper.QualityEvaluationIndicatorList[selectedIndicatorName];
            
            IndicatorDetailDataBind();
            chartElementCaption.Text = (indicatorName == selectedIndicatorName)
                ? String.Format("Показатель «{0}»", indicatorName)
                : String.Format("Показатель {1} «{0}»", indicatorName, selectedIndicatorName);

            ChartSetup();

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
                        return "мониторинг за 1 квартал";
                    }
                case "Квартал 2":
                    {
                        return "мониторинг за 2 квартал";
                    }
                case "Квартал 3":
                    {
                        return "мониторинг за 3 квартал";
                    }
                case "Квартал 4":
                    {
                        return "мониторинг за 4 квартал (по итогам года)";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        private string GetIndicatorFormatString(string indicatorName)
        {
            if (indicatorFormat == "ДА/НЕТ")
            {
                return "N0";
            }
            else
            {
                return "N2";
            }
            //switch (indicatorName)
            //{
            //    case "I (1)":
            //    case "I (2)":
            //    case "I (3)":
            //    case "I (5)":
            //    case "II (2)":
            //    case "II (3)":
            //    case "II (5)":
            //    case "II (6)":
            //    case "II (7)":
            //    case "III (5)":
            //    case "Рейтинг":
            //    case "Средняя оценка":
            //        {
            //            return "N2";
            //        }
            //    default:
            //        {
            //            return "N0";
            //        }
            //}
        }

        #region Обработчики диаграммы

        private void ChartSetup()
        {
            string formatString = GetIndicatorFormatString(selectedIndicatorName);

            if (IsRating)
            {
                UltraChart.Axis.Y.RangeType = AxisRangeType.Automatic;
            }
            else
            {
                UltraChart.Axis.Y.RangeType = AxisRangeType.Custom;
                UltraChart.Axis.Y.RangeMin = -3;
                UltraChart.Axis.Y.RangeMax = 3;
            }

            UltraChart.ChartType = ChartType.LineChart;
            UltraChart.LineChart.NullHandling = NullHandling.DontPlot;
            UltraChart.ColumnChart.ColumnSpacing = 0;
            UltraChart.ColumnChart.SeriesSpacing = 0;
            UltraChart.Border.Thickness = 0;

            UltraChart.Data.ZeroAligned = true;

            UltraChart.Axis.X.Extent = 140;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.Y.Extent = 35;
            UltraChart.Axis.Y.Labels.ItemFormatString = String.Format("<DATA_VALUE:{0}>", formatString);
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value) / 3;
            UltraChart.Legend.Font = new Font("Verdana", 8);
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 6;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Text = "Баллы";
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.Thickness = 2;
            lineAppearance.IconAppearance.Icon = SymbolIcon.Diamond;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            UltraChart.LineChart.LineAppearances.Add(lineAppearance);

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Row = -2;
            if (IsAVGEvaluation)
            {
                appearance.HorizontalAlign = StringAlignment.Near;
                appearance.Column = 1;
                appearance.ItemFormatString = String.Format("Средняя оценка: <DATA_VALUE:{0}>", formatString);
            }
            else
            {
                appearance.Column = -2;
                appearance.ItemFormatString = String.Format("<DATA_VALUE:{0}>", formatString);
            }
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart.LineChart.ChartText.Add(appearance);

            UltraChart.Tooltips.FormatString = String.Format("<ITEM_LABEL>\n<SERIES_LABEL>\n{0}: <DATA_VALUE:{1}>", "оценка показателя", formatString);
        }

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0040_0004_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value && row[1] != DBNull.Value)
                {
                    string quarterName = row[0].ToString();
                    string yearName = row[1].ToString();

                    string[] strs = quarterName.Split(' ');
                    if (strs.Length > 0)
                    {
                        quarterName = String.Format("{0} квартал", strs[1]);
                    }

                    row[0] = String.Format("{0} {1} года", quarterName, yearName); 
                }
            }

            dtChart.Columns.Remove("Год");

            UltraChart.DataSource = dtChart;
        }

        protected static void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
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

        protected static void UltraChart_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            e.Text = "Нет данных";
            e.LabelStyle.FontColor = Color.Black;
            e.LabelStyle.FontSizeBestFit = false;
            e.LabelStyle.HorizontalAlign = StringAlignment.Center;
            e.LabelStyle.VerticalAlign = StringAlignment.Center;
        }

        #endregion

        #region Параметры индикатора

        private void IndicatorDetailDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0040_0004_indicatorDescription");
            dtIndicatorDetail = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtIndicatorDetail);

            indicatorName = GetStringDTValue(dtIndicatorDetail, "Наименование");
            indicatorFormat = GetStringDTValue(dtIndicatorDetail, "Формат");
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