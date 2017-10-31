using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Excel;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0007
{
    public partial class DefaultAllocation : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtAVG = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private int selectedPointIndex;
        private string populationDate;

        #region Параметры запроса

        // расходы Итого
        private CustomParam outcomesTotal;
        // группа ФКР
        private CustomParam fkrGroupName;
        // уровень МР и ГО
        private CustomParam regionsLevel;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;

        // численность населения
        private CustomParam populationMeasure;
        // год для численности населения
        private CustomParam populationMeasureYear;

        // тип документа СКИФ для уровня районов
        private CustomParam regionDocumentSKIFType;

        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        // фильтр по КОСГУ
        private CustomParam kosguFilter;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            UltraWebGrid.Visible = false;
            #region Инициализация параметров запроса

            if (outcomesTotal == null)
            {
                outcomesTotal = UserParams.CustomParam("outcomes_total");
            }
            if (fkrGroupName == null)
            {
                fkrGroupName = UserParams.CustomParam("fkr_group_name");
            }
            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (regionsConsolidateLevel == null)
            {
                regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            }

            if (populationMeasure == null)
            {
                populationMeasure = UserParams.CustomParam("population_measure");
            }
            if (populationMeasureYear == null)
            {
                populationMeasureYear = UserParams.CustomParam("population_measure_year");
            }

            if (regionDocumentSKIFType == null)
            {
                regionDocumentSKIFType = UserParams.CustomParam("region_document_skif_type");
            }

            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }
            kosguFilter = UserParams.CustomParam("kosgu_filter");

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ScatterChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.Y.Extent = 50;
            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Text = "Коэффициент бюджетной обеспеченности, руб./чел.";
            UltraChart.TitleLeft.Font = new Font("Verdana", 12);
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;

            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.Text = "Численность населения, тыс.чел.";
            UltraChart.TitleBottom.Font = new Font("Verdana", 12);
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;

            UltraChart.ScatterChart.Icon = SymbolIcon.Square;
            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<DATA_VALUE_X:N3> тыс.чел.\n<DATA_VALUE_Y:N2> руб./чел.";
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 200);

            CrossLink1.Text = "Таблица&nbsp;исполнения&nbsp;расходов&nbsp;и&nbsp;бюджетной&nbsp;обеспеченности";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0007/DefaultCompare.aspx";

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            //UltraGridExporter1.ExcelExportButton.Visible = false;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            UserParams.PopulationCube.Value = RegionSettingsHelper.Instance.PopulationCube;
            UserParams.PopulationFilter.Value = RegionSettingsHelper.Instance.PopulationFilter;
            UserParams.PopulationPeriodDimension.Value = RegionSettingsHelper.Instance.PopulationPeriodDimension;
            UserParams.PopulationValueDivider.Value = RegionSettingsHelper.Instance.PopulationValueDivider;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0007_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);

                ComboFKR.Width = 420;
                ComboFKR.Title = "РзПр";
                ComboFKR.MultiSelect = false;
                ComboFKR.ParentSelect = true;
                ComboFKR.FillDictionaryValues(CustomMultiComboDataHelper.FillFOFKRNames(DataDictionariesHelper.OutcomesFOFKRTypes, DataDictionariesHelper.OutcomesFOFKRLevels, false));
                ComboFKR.SetСheckedState("Общегосударственные вопросы", true);
                ComboFKR.SetСheckedState("Расходы бюджета - ИТОГО", true);
                ComboFKR.SetСheckedState("Расходы бюджета Итого", true);

                ComboRegion.Visible = true;
                ComboRegion.Title = "Территории";
                ComboRegion.Width = 280;
                ComboRegion.MultiSelect = false;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillTerritories(RegionsNamingHelper.LocalBudgetTypes, false));

                selectedPointIndex = -1;
            }


            Page.Title = string.Format("Распределение муниципальных образований по коэффициенту бюджетной обеспеченности");
            Label1.Text = Page.Title;
            Label2.Text = string.Empty;

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            outcomesTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            populationMeasure.Value = RegionSettingsHelper.Instance.PopulationMeasure;
            populationMeasureYear.Value = RegionSettingsHelper.Instance.PopulationMeasurePlanning ? (yearNum + 1).ToString() : yearNum.ToString();

            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");
            kosguFilter.Value = RegionSettingsHelper.Instance.GetPropertyValue("KosguFilter");

            if (ComboFKR.SelectedValue == "Расходы бюджета - ИТОГО")
            {
                fkrGroupName.Value = outcomesTotal.Value;
            }
            else
            {
                fkrGroupName.Value = DataDictionariesHelper.OutcomesFOFKRTypes[ComboFKR.SelectedValue];
            }

            populationDate = DataDictionariesHelper.GetRegionPopulationDate(yearNum);

            UltraChart.DataBind();
        }

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0007_allocation_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtChart);

            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    row[0] = row[0].ToString().Replace("\"", "'");
                }
            }

            query = DataProvider.GetQueryText("FO_0002_0007_allocation_avg");
            dtAVG = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Среднее", dtAVG);

            selectedPointIndex = -1;
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                if (dtChart.Rows[i][0] != DBNull.Value && dtChart.Rows[i][0].ToString() == ComboRegion.SelectedValue.Replace("\"", "'"))
                {
                    selectedPointIndex = i;
                    break;
                }
            }

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            subjectLabel.Text = string.Format("{0}:", ComboRegion.SelectedValue);
            populationLabel.Text = string.Empty;
            statisticLabel.Text = string.Empty;
            if (selectedPointIndex != -1 && dtChart.Rows[selectedPointIndex][1] != DBNull.Value &&
                dtChart.Rows[selectedPointIndex][2] != DBNull.Value)
            {
                populationLabel.Text = string.Format("Численность постоянного населения ({1}): <b>{0}</b> тыс.чел.",
                        dtChart.Rows[selectedPointIndex][1], populationDate);

                double income = Convert.ToDouble(dtChart.Rows[selectedPointIndex][2]);
                statisticLabel.Text = string.Format("За {1} {2} {3} года коэффициент бюджетной обеспеченности населения ({4}) составляет <b>{0}</b>&nbsp;руб./чел.",
                    income.ToString("N2"), monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboFKR.SelectedValue);
            }

            UltraChart.DataSource = dtChart;
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for(int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is PointSet)
                {
                    PointSet pointSet = primitive as PointSet;

                    foreach (DataPoint point in pointSet.points)
                    {
                        if (point.Row == selectedPointIndex)
                        {
                            Symbol symbol = new Symbol(point.point, pointSet.icon, pointSet.iconSize);
                            symbol.PE.Fill = Color.DarkOrange;
                            e.SceneGraph.Add(symbol);
                        }
                    }
                }
            }

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int textWidht = 200;
            int textHeight = 12;
            int lineStart = (int)xAxis.MapMinimum;
            int lineLength = (int)xAxis.MapMaximum;

            int annotationColumnIndex = 0;
            double avgValue = 0;
            if (dtAVG.Rows[0].ItemArray.Length > annotationColumnIndex &&
                dtAVG.Rows[0][annotationColumnIndex] != DBNull.Value &&
                dtAVG.Rows[0][annotationColumnIndex].ToString() != string.Empty)
            {
                avgValue = Convert.ToDouble(dtAVG.Rows[0][annotationColumnIndex]);
            }

            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new Point(lineStart, (int)yAxis.Map(avgValue));
            line.p2 = new Point(lineStart + lineLength, (int)yAxis.Map(avgValue));
            e.SceneGraph.Add(line);

            Text text = new Text();
            text.PE.Fill = Color.Black;
            text.bounds = new Rectangle(lineLength - textWidht, ((int)yAxis.Map(avgValue)) - textHeight, textWidht, textHeight);
            text.SetTextString(string.Format("Среднее по МО: {0:N2} руб./чел.", avgValue));
            e.SceneGraph.Add(text);
        }

        #endregion

        #region Экспорт в Excel
        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.Workbook.Worksheets["Диаграмма"].Rows[0].Cells[0].Value = Label1.Text;
            e.Workbook.Worksheets["Диаграмма"].Rows[1].Cells[0].Value = subjectLabel.Text;
            e.Workbook.Worksheets["Диаграмма"].Rows[2].Cells[0].Value = RemoveBoldTags(populationLabel.Text);
            string statisticsText = RemoveBoldTags(statisticLabel.Text);
            statisticsText = statisticsText.Replace("&nbsp;", " ");
            e.Workbook.Worksheets["Диаграмма"].Rows[3].Cells[0].Value = RemoveBoldTags(statisticsText);
        }
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Диаграмма");
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            UltraGridExporter.ChartExcelExport(sheet1.Rows[5].Cells[0], UltraChart);

            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1, 100, 0);
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
        }

        #endregion

        #region Экспорт в Pdf

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
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(subjectLabel.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(RemoveBoldTags(populationLabel.Text));

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            string statisticsText = RemoveBoldTags(statisticLabel.Text);
            statisticsText = statisticsText.Replace("&nbsp;", " ");
            title.AddContent(RemoveBoldTags(statisticsText));

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        private string RemoveBoldTags(string source)
        {
            string result = source.Replace("<b>", string.Empty);
            result = result.Replace("</b>", string.Empty);
            return result;
        }

        #endregion
    }
}
