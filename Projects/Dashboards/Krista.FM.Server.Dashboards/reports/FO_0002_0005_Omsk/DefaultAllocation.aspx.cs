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

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0005_Omsk
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

        // доходы Итого
        private CustomParam incomesTotal;
        // выбранный район
        private CustomParam selectedRegion;
        // уровень МР и ГО
        private CustomParam regionsLevel;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        // численность населения
        private CustomParam populationMeasure;
        // год для численности населения
        private CustomParam populationMeasureYear;

        // тип документа СКИФ для консолидированного уровня
        private CustomParam consolidateDocumentSKIFType;
        // тип документа СКИФ для уровня районов
        private CustomParam regionDocumentSKIFType;
        
        // численность населения по всем МО
        private CustomParam populationTotal;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }
            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (regionsConsolidateLevel == null)
            {
                regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            }
            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
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
            
            populationTotal = UserParams.CustomParam("population_total");

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ScatterChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.Y.Extent = 50;
            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Text = "Среднедушевые доходы, руб./чел.";
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

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 40);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 220);

            CrossLink1.Text = "Таблица&nbsp;исполнения&nbsp;доходов&nbsp;и&nbsp;среднедушевых&nbsp;доходов";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0005_Omsk/DefaultCompare.aspx";
            CrossLink2.Text = "Доходы&nbsp;субъекта&nbsp;РФ&nbsp;подробнее";
            CrossLink2.NavigateUrl = "~/reports/FO_0002_0003/DefaultDetail.aspx";

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            UltraGridExporter1.ExcelExportButton.Visible = false;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0005_Omsk_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                ComboRegion.Visible = true;
                ComboRegion.Title = "Территория";
                ComboRegion.Width = 280;
                ComboRegion.MultiSelect = false;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillTerritories(RegionsNamingHelper.LocalBudgetTypes, false));

                selectedPointIndex = -1;
            }

            UltraChart.ScatterChart.ColumnY = (!useDotationCheckBox.Checked) ? 2 : 3;

            Page.Title = "Распределение муниципальных образований по среднедушевым доходам";
            Label1.Text = Page.Title;
            Label2.Text = string.Empty;

            int year = Convert.ToInt32(ComboYear.SelectedValue);

            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            
            populationMeasure.Value = RegionSettingsHelper.Instance.PopulationMeasure;
            populationMeasureYear.Value = RegionSettingsHelper.Instance.PopulationMeasurePlanning ? (year + 1).ToString() : year.ToString();

            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");
            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");

            populationDate = DataDictionariesHelper.GetOmskRegionPopulationDate(year);

            UltraChart.DataBind();
        }

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0005_Omsk_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtChart);

            query = DataProvider.GetQueryText("FO_0002_0005_Omsk_allcation_population");
            DataTable populationDT = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Численность", populationDT);

            // в первой строке хранится общая численность для вычисления среднего
            if (populationDT.Columns.Count > 1 && populationDT.Rows.Count > 0)
            {
                if (populationDT.Rows[0][1] != DBNull.Value && populationDT.Rows[0][1].ToString() != String.Empty)
                {
                    populationTotal.Value = Convert.ToDouble(populationDT.Rows[0][1]).ToString().Replace(",", ".");
                    populationDT.Rows.RemoveAt(0);
                }
            }

            const string populationColumnName = "Численность постоянного населения";
            const string avgExecuteColumnName = "Среднедушевые доходы";
            const string avgExecuteWithoutDotationColumnName = "Среднедушевые доходы без дотаций";

            if (dtChart.Columns.Count > 1 && dtChart.Rows.Count > 0)
            {
                dtChart.PrimaryKey = new DataColumn[] { dtChart.Columns[0] };

                foreach (DataRow populationRow in populationDT.Rows)
                {
                    if (populationRow[0] != DBNull.Value)
                    {
                        string rowName = populationRow[0].ToString();
                        if (populationRow[populationColumnName] != DBNull.Value &&
                            populationRow[populationColumnName].ToString() != String.Empty)
                        {
                            double population = Convert.ToDouble(populationRow[populationColumnName]);

                            DataRow dtRow = dtChart.Rows.Find(rowName);
                            if (dtRow != null)
                            {
                                dtRow[populationColumnName] = population;

                                if (population != 0 && dtRow[avgExecuteColumnName] != DBNull.Value &&
                                    dtRow[avgExecuteColumnName].ToString() != String.Empty)
                                {
                                    double execute = Convert.ToDouble(dtRow[avgExecuteColumnName]);
                                    double avgExecute = execute / population;

                                    dtRow[avgExecuteColumnName] = avgExecute;
                                }

                                if (population != 0 && dtRow[avgExecuteWithoutDotationColumnName] != DBNull.Value &&
                                    dtRow[avgExecuteWithoutDotationColumnName].ToString() != String.Empty)
                                {
                                    double execute = Convert.ToDouble(dtRow[avgExecuteWithoutDotationColumnName]);
                                    double avgExecute = execute / population;

                                    dtRow[avgExecuteWithoutDotationColumnName] = avgExecute;
                                }
                            }
                        }
                    }
                }
            }

            query = DataProvider.GetQueryText("FO_0002_0005_Omsk_avg");
            dtAVG = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Среднее", dtAVG);

            selectedPointIndex = -1;
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                if (dtChart.Rows[i][0] != DBNull.Value && dtChart.Rows[i][0].ToString() == ComboRegion.SelectedValue)
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
            if (dtChart.Rows.Count > 0 && selectedPointIndex != -1 && dtChart.Rows[selectedPointIndex][1] != DBNull.Value &&
                dtChart.Rows[selectedPointIndex][2] != DBNull.Value && dtChart.Rows[selectedPointIndex][3] != DBNull.Value)
            {
                populationLabel.Text = string.Format("Численность постоянного населения  ({1}): <b>{0:N3}</b> тыс.чел.",
                        dtChart.Rows[selectedPointIndex][1], populationDate);

                double income = Convert.ToDouble(dtChart.Rows[selectedPointIndex][2]);
                double dotation = Convert.ToDouble(dtChart.Rows[selectedPointIndex][3]);
                statisticLabel.Text = string.Format("За {2} {3} {4} года доходы на душу населения составляют <b>{0}</b> руб./чел. (без учета дотаций на выравнивание бюджетной обеспеченности <b>{1}</b> руб./чел.).",
                    income.ToString("N2"), dotation.ToString("N2"), monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);
            }

            UltraChart.DataSource = dtChart;
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            foreach (Primitive primitive in e.SceneGraph)
            {
                if (selectedPointIndex == -1)
                {
                    break;
                }

                PointSet pointSet = primitive as PointSet;

                if (pointSet == null)
                {
                    continue;
                }

                foreach (DataPoint point in pointSet.points)
                {
                    if (point.Row == selectedPointIndex)
                    {
                        Symbol symbol = new Symbol(point.point, pointSet.icon, pointSet.iconSize);
                        symbol.PE.Fill = Color.DarkOrange;
                        e.SceneGraph.Add(symbol);
                    }
                }

                break;
            }

            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int textWidht = 200;
            int textHeight = 12;
            int lineStart = (int)xAxis.MapMinimum;
            int lineLength = (int)xAxis.MapMaximum;

            int annotationColumnIndex = (!useDotationCheckBox.Checked) ? 0 : 1;
            double avgValue = 0;
            if (dtAVG.Rows.Count > 0 &&
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
            title.AddContent(RemoveBoldTags(statisticLabel.Text));

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.86));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        private static string RemoveBoldTags(string source)
        {
            string result = source.Replace("<b>", string.Empty);
            result = result.Replace("</b>", string.Empty);
            return result;
        }

        #endregion
    }
}
