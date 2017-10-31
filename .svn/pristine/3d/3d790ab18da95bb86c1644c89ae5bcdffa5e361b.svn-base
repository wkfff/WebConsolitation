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
using Infragistics.WebUI.UltraWebNavigator;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0021_0005_Saratov
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChart;
        private int firstYear = 2010;
        private int endYear = 2011;

        private int selectedQuarterIndex;
        private int selectedYear;
        private string selectedMeasureCaption;

        private string selectedIndicatorName;
        private string selectedIndicatorType;
        private string indicatorUnit;
        private string hintFormatString;

        #endregion

        private bool IsYearCompare
        {
            get { return selectedQuarterIndex == 4; }
        }

        private MeasureType MeasureType
        {
            get
            {
                switch (MeasureButtonList.SelectedIndex)
                {
                    case 0:
                        {
                            return MeasureType.Density;
                        }
                    case 1:
                        {
                            return MeasureType.Evaluation;
                        }
                    default:
                        {
                            return MeasureType.Value;
                        }
                }
            }
        }

        #region Параметры запроса

        // выбранный индикатор
        private CustomParam selectedIndicator;
        // имя выбранного индикатора
        private CustomParam selectedIndicatorCaption;
        // выбраная мера
        private CustomParam selectedMeasure;
        // множество периодов
        private CustomParam periodSet;
        // уровень районов
        private CustomParam regionsLevel;

        #endregion

        private static MemberAttributesDigest indicatorDigest;

        private static MemberAttributesDigest periodDigest;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.73);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChart_InvalidDataReceived);

            #region Инициализация параметров запроса

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
            selectedIndicatorCaption = UserParams.CustomParam("selected_indicator_caption");

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.LineChart;
            UltraChart.LineChart.NullHandling = NullHandling.DontPlot;
            UltraChart.ColumnChart.ColumnSpacing = 0;
            UltraChart.ColumnChart.SeriesSpacing = 0;
            UltraChart.Border.Thickness = 0;

            UltraChart.Axis.X.Extent = 160;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.Y.Extent = 60;
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

            #endregion

            UltraGridExporter1.ExcelExportButton.Visible = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            CrossLink1.Visible = true;
            CrossLink1.Text = "Рейтинг&nbsp;МО";
            CrossLink1.NavigateUrl = "~/reports/FO_0021_0002_Saratov/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Результаты&nbsp;оценки&nbsp;качества";
            CrossLink2.NavigateUrl = "~/reports/FO_0021_0001_Saratov/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Динамика&nbsp;результатов&nbsp;оценки&nbsp;качества";
            CrossLink3.NavigateUrl = "~/reports/FO_0021_0003_Saratov/Default.aspx";

            CrossLink4.Visible = true;
            CrossLink4.Text = "Картограмма&nbsp;результатов&nbsp;оценки&nbsp;качества";
            CrossLink4.NavigateUrl = "~/reports/FO_0021_0004_Saratov/Default.aspx";


            MeasureButtonList.Items[0].Text = "<span title='Оценка индикатора умноженная на его вес относительной значимости (в методике это (MixWi)). Различия в величинах весов обусловлены разной степенью влияния отражаемых индикаторами факторов на общий уровень финансового положения и качество управления финансами'>Удельный вес</span>";
            MeasureButtonList.Items[1].Text = "<span title='Оценка полученного значения (в методике это Mi - оценка по индикатору, рассчитанное на основе Vi и критических значений индикатора)'>Оценка</span>";
            MeasureButtonList.Items[2].Text = "<span title='Непосредственно считается значение по формуле (в методике это расчетное значения каждого индикатора - Vi)'>Значение</span>";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0021_0005_Saratov_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                selectedIndicatorCaption.Value = "Предельный объем муниципального долга";

                ComboPeriod.PanelHeaderTitle = "Выберите даты";
                ComboPeriod.Width = 150;
                ComboPeriod.ShowSelectedValue = false;
                ComboPeriod.MultiSelect = true;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0021_0005_Saratov_nonEmptyPeriodList");
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                Node lastNode = ComboPeriod.GetLastNode(1);
                Node lastNodeParent = ComboPeriod.GetLastNode(1).Parent;
                ComboPeriod.SetSelectedNode(ComboPeriod.GetPreviousSublingNode(lastNode),true);
                ComboPeriod.SetSelectedNode(ComboPeriod.GetPreviousSublingNode(lastNodeParent), true);
                ComboPeriod.SetSelectedNode(lastNode, true);
                ComboPeriod.SetSelectedNode(lastNodeParent, true);
            }

            Page.Title = String.Format("Диаграмма динамики результатов оценки качества управления финансами и платежеспособности муниципальных образований Саратовской области по отдельному показателю");
            PageTitle.Text = Page.Title;

            PageSubTitle.Text = String.Empty;
            
            switch (MeasureType)
            {
                case MeasureType.Density:
                    {
                        selectedMeasure.Value = "Взвешенное значение";
                        selectedMeasureCaption = "удельный вес показателя";
                        break;
                    }
                case MeasureType.Evaluation:
                    {
                        selectedMeasure.Value = "Оценка индикатора";
                        selectedMeasureCaption = "оценка показателя";
                        break;
                    }
                case MeasureType.Value:
                    {
                        selectedMeasure.Value = "Значение";
                        selectedMeasureCaption = "значение показателя";
                        break;
                    }
            }

            periodSet.Value = String.Empty;
            CRHelper.SaveToErrorLog(ComboPeriod.SelectedValues.Count.ToString());
            foreach (string value in ComboPeriod.SelectedValues)
            {
                periodSet.Value += periodDigest.GetMemberUniqueName(value) + ",";
            }
            periodSet.Value = periodSet.Value.TrimEnd(',');

            if (ComboIndicator.SelectedValue != String.Empty)
            {
                selectedIndicatorCaption.Value = ComboIndicator.SelectedValue;
            }

            ComboIndicator.Title = "Показатель";
            ComboIndicator.Width = 600;
            ComboIndicator.MultiSelect = false;
            indicatorDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0021_0005_Saratov_indicatorList");
            ComboIndicator.ClearNodes();
            ComboIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(indicatorDigest.UniqueNames, indicatorDigest.MemberLevels));
            ComboIndicator.SelectLastNode();
            ComboIndicator.SetСheckedState(selectedIndicatorCaption.Value, true);
            
            selectedIndicatorName = ComboIndicator.SelectedValue;
            selectedIndicator.Value = indicatorDigest.GetMemberUniqueName(selectedIndicatorName);

            hintFormatString = GetIndicatorFormatString(selectedIndicatorName);

            chartElementCaption.Text = String.Format("Показатель «{0}»", selectedIndicatorName);
            UltraChart.TitleLeft.Text = indicatorUnit;

            UltraChart.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart.Axis.Y.Labels.ItemFormatString = String.Format("<DATA_VALUE:N3>", hintFormatString);

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = String.Format("<DATA_VALUE:{0}>", hintFormatString);
            appearance.ChartTextFont = new Font("Verdana", 8);
            appearance.Visible = true;
            UltraChart.LineChart.ChartText.Add(appearance);

            UltraChart.DataBind();
        }

        private string GetIndicatorFormatString(string indicatorName)
        {
            string shortName = indicatorDigest.GetMemberType(indicatorName);
            switch (shortName)
            {
                case "P1":
                case "P2":
                case "P3":
                case "P4":
                case "P5":
                case "P6":
                case "P7":
                case "P8":
                case "P9":
                case "P10":
                case "P11":
                case "P12":
                case "P13":
                case "P14":
                case "P15":
                case "P16":
                case "P17":
                case "P18":
                case "P19":
                case "P20":
                case "P21":
                    {
                        return (MeasureType == MeasureType.Density) ? "N2" : "N0";
                    }
                case "Итоговая оценка":
                case "Средняя оценка":
                    {
                        return "N1";
                    }
                case "Ранг":
                    {
                        return "N0";
                    }
                default:
                    {
                        return "N2";
                    }
            }
        }

        private bool IsBooleanIndicator(string indicatorName)
        {
            string format =  GetIndicatorFormatString(indicatorName);
            return (indicatorName != "Ранг") && (format == "N0");
        }

        #region Обработчики диаграммы

        private string GetShortRegionName(string fullRegionName)
        {
            string shortRegionName = fullRegionName.Replace("муниципальное образование", "МО");
            shortRegionName = shortRegionName.Replace("муниципальный район", "МР");
            shortRegionName = shortRegionName.Replace("\"", "'");
            return shortRegionName;
        }

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0021_0005_Saratov_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Columns.Count > 1)
            {
                dtChart.Columns.RemoveAt(0);
            }

            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    string periodUniqueName = row[0].ToString();
                    DateTime periodDateTime = CRHelper.PeriodDayFoDate(periodUniqueName);
                    row[0] = String.Format("{0} квартал {1} года", CRHelper.QuarterNumByMonthNum(periodDateTime.Month), periodDateTime.Year);
                }
            }

            foreach (DataColumn column in dtChart.Columns)
            {
                column.ColumnName = GetShortRegionName(column.ColumnName);
            }

            UltraChart.Data.SwapRowsAndColumns = true;
            for (int i = 1; i < dtChart.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                series.Label = dtChart.Columns[i].ColumnName;
                UltraChart.Series.Add(series);
            }
           // UltraChart.DataSource = dtChart;
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
                else if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline) primitive;

                    if (polyline.Path != null && polyline.Path.ToLower().Contains("legend"))
                    {
                        polyline.Visible = false;

                        Symbol symbol = new Symbol();
                        symbol.PE.Fill = polyline.PE.Fill;
                        symbol.icon = SymbolIcon.Square;
                        symbol.iconSize = SymbolIconSize.Medium;
                        symbol.PE.ElementType = PaintElementType.SolidFill;
                        int x1 = polyline.points[0].point.X;
                        int x2 = polyline.points[1].point.X;
                        int y = polyline.points[0].point.Y;
                        symbol.point = new Point(x1 + (x2 - x1) / 2 + 5, y);
                        e.SceneGraph.Add(symbol);
                    }
                    else
                    {
                        foreach (DataPoint point in polyline.points)
                        {
                            if (point.DataPoint != null)
                            {
                                double pointValue = Convert.ToDouble(point.Value);

                                point.DataPoint.Label = String.Format("{0}\n{1}\n{2}: {3}", point.DataPoint.Label,
                                                                      point.Series.Label, selectedMeasureCaption,
                                                                      pointValue);

                                if (IsBooleanIndicator(selectedIndicatorName))
                                {
                                    string boolHint = String.Empty;
                                    if (pointValue == 0)
                                    {
                                        boolHint = " (Нет)";
                                    }
                                    else if (pointValue == 1)
                                    {
                                        boolHint = " (Да)";
                                    }
                                    point.DataPoint.Label += boolHint;
                                }
                            }
                        }
                    }
                }
            }
        }

        void UltraChart_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            if (!IsYearCompare && selectedIndicatorType == "Ежегодно")
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

    public enum MeasureType
    {
        Density,
        Value,
        Evaluation
    }
}