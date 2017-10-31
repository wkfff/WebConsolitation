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

namespace Krista.FM.Server.Dashboards.reports.FO_0039_0004_Kostroma
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChart;
        private int firstYear = 2010;
        private int endYear = 2011;
        private int selectedQuarterIndex;

        #endregion

        private bool IsYearCompare
        {
            get { return selectedQuarterIndex == 4; }
        }

        #region Параметры запроса

        // выбранный начальный период
        private CustomParam selectedPeriod;
        // уровень районов
        private CustomParam regionsLevel;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.77);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #region Инициализация параметров запроса

            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selected_period");
            }
            regionsLevel = UserParams.CustomParam("regions_level");

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Data.ZeroAligned = true;
            UltraChart.Border.Thickness = 0;

            UltraChart.Axis.X.Extent = 210;
            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.Y.Extent = 20;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value) / 4;
            UltraChart.Legend.Font = new Font("Verdana", 8);
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 6;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value) / 4;
            UltraChart.TitleLeft.Text = "Число показателей";
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);

            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleBottom.Margins.Left = UltraChart.Axis.Y.Extent;
            UltraChart.TitleBottom.Text = "Место по рейтингу";
            UltraChart.TitleBottom.Font = new Font("Verdana", 8);

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N0>";
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart.ColumnChart.ChartText.Add(appearance);

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>";

            #endregion

            UltraGridExporter1.ExcelExportButton.Visible = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            
            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты оценки качества МР(ГО)";
            CrossLink1.NavigateUrl = "~/reports/FO_0039_0001_Kostroma/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Динамика&nbsp;результатов&nbsp;оценки";
            CrossLink2.NavigateUrl = "~/reports/FO_0039_0002_Kostroma/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Рейтинг&nbsp;МР(ГО)";
            CrossLink3.NavigateUrl = "~/reports/FO_0039_0003_Kostroma/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Картограмма";
            CrossLink4.NavigateUrl = "~/reports/FO_0039_0005_Kostroma/Default.aspx";

            CrossLink5.Visible = true;
            CrossLink5.Text = "Результаты&nbsp;оценки&nbsp;по&nbsp;отд.показателю";
            CrossLink5.NavigateUrl = "~/reports/FO_0039_0006_Kostroma/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0039_0004_Kostroma_date");
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
            }

            Page.Title = String.Format("Сравнительная характеристика максимальных и минимальных оценок показателей по МР(ГО) Костромской области");
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
            string query = DataProvider.GetQueryText("FO_0039_0004_Kostroma_chart");
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
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text axisText = (Text) primitive;
                    axisText.bounds.Width = 30;
                    axisText.labelStyle.VerticalAlign = StringAlignment.Near;
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
                        int columnIndex = box.Column == 0 ? 3 : 4;

                        string indicatorList = String.Empty;
                        if (dtChart != null && dtChart.Rows[rowIndex][columnIndex] != DBNull.Value &&
                            dtChart.Rows[rowIndex][columnIndex].ToString() != String.Empty)
                        {
                            indicatorList =
                                String.Format("({0})", dtChart.Rows[rowIndex][columnIndex].ToString().Replace("br", " "));
                        }

                        String indicatorStr;
                        String thatStr;
                        int boxValue = Convert.ToInt32(box.Value);

                        if (boxValue/10 != 1)
                        {
                            if (boxValue%10 == 1)
                            {
                                indicatorStr = "показатель";
                                thatStr = "которого";
                            }
                            else if (boxValue%10 < 5)
                            {
                                indicatorStr = "показателя";
                                thatStr = "которых";
                            }
                            else
                            {
                                indicatorStr = "показателей";
                                thatStr = "которых";
                            }
                        }
                        else
                        {
                            indicatorStr = "показателей";
                            thatStr = "которых";
                        }


                        box.DataPoint.Label = String.Format("{0}\n{1} {2}, оценка {3} равна {4}\n{5}",
                                                            box.Series.Label,
                                                            box.Value,
                                                            indicatorStr,
                                                            thatStr,
                                                            box.Column == 0 ? 1 : 0,
                                                            indicatorList);
                    }
                }
            }

            if (dtChart != null && dtChart.Rows.Count > 0)
            {
                IAdvanceAxis xAxis = (IAdvanceAxis) e.Grid["X"];
                IAdvanceAxis yAxis = (IAdvanceAxis) e.Grid["Y"];

                if (xAxis == null || yAxis == null)
                    return;

                double xMin = xAxis.MapMinimum;
                double xMax = xAxis.MapMaximum;
                double yMin = yAxis.MapMinimum;

                double axisStep = xAxis.Map(1) - xAxis.Map(0);
                double lineX = xMin + 2.5 * axisStep;
                double lineY = UltraChart.Height.Value - 30;
                double textX = xMin - 0.5 * axisStep;
                double textY = UltraChart.Height.Value - 50;

                Line line = new Line();
                line.p1 = new Point((int)xMin, (int)yMin);
                line.p2 = new Point((int)xMin, (int)lineY);
                line.PE.Fill = Color.Black;
                line.lineStyle.DrawStyle = LineDrawStyle.Solid;
                e.SceneGraph.Add(line);

                line = new Line();
                line.p1 = new Point((int)xMax, (int)yMin);
                line.p2 = new Point((int)xMax, (int)lineY);
                line.PE.Fill = Color.Black;
                line.lineStyle.DrawStyle = LineDrawStyle.Solid;
                e.SceneGraph.Add(line);

                for (int i = 0; i < dtChart.Rows.Count; i++)
                {
                    Text rankText = new Text();
                    rankText.bounds = new Rectangle((int)textX, (int)textY, 3 * (int)axisStep, 20);
                    rankText.SetTextString((i + 1).ToString());
                    LabelStyle labelStyle = new LabelStyle();
                    labelStyle.HorizontalAlign = StringAlignment.Center;
                    rankText.SetLabelStyle(labelStyle);
                    e.SceneGraph.Add(rankText);

                    if (i != dtChart.Rows.Count - 1)
                    {
                        line = new Line();
                        line.p1 = new Point((int) lineX, (int) yMin);
                        line.p2 = new Point((int) lineX, (int) lineY);
                        line.PE.Fill = Color.Black;
                        line.lineStyle.DrawStyle = LineDrawStyle.Solid;
                        e.SceneGraph.Add(line);
                    }

                    textX += 3 * axisStep;
                    lineX += 3 * axisStep;
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 5;

            UltraGridExporter.ChartExcelExport(e.CurrentWorksheet.Rows[2].Cells[0], UltraChart);
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

            UltraChart.Width = Unit.Pixel((int) (CustomReportConst.minScreenWidth*0.8));
            UltraChart.Legend.Margins.Right = 5;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        #endregion
    }
}
