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

namespace Krista.FM.Server.Dashboards.reports.FO_0016_0003_HMAO
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtChart;

        private int endYear = 2011;

        private int selectedQuarterIndex;
        private string indicatorUnit;
        private string selectedIndicatorName;
        private string hintFormatString;
        private string fullNameIndicator;

        #endregion

        private bool IsYear_IsThirdQuarter
        {
            get { return ((selectedQuarterIndex == 4) || (selectedQuarterIndex == 3)); }
        }

        private bool IsFirstQuarter
        {
            get { return (selectedQuarterIndex == 1); }
        }

        #region Параметры запроса

        // выбранный индикатор
        private CustomParam selectedIndicator;
        // имя выбранного индикатора
        private CustomParam selectedIndicatorCaption;
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

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.9);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(UltraChart_InvalidDataReceived);

            #region Инициализация параметров запроса

            if (selectedIndicator == null)
            {
                selectedIndicator = UserParams.CustomParam("selected_indicator");
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

            UltraChart.Axis.X.Extent = 120;
            UltraChart.Axis.X.Labels.Visible = true;
            UltraChart.Axis.Y.Extent = 60;
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart.Axis.Y.Labels.ItemFormatString = "N1";

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
            lineAppearance.Thickness = 4;
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
            CrossLink1.Text = "Результаты&nbsp;мониторинга";
            CrossLink1.NavigateUrl = "~/reports/FO_0016_0001_HMAO/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Картограмма&nbsp;с&nbsp;результатами&nbsp;мониторинга";
            CrossLink2.NavigateUrl = "~/reports/FO_0016_0002_HMAO/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Расходы&nbsp;на&nbsp;содержание&nbsp;ОМСУ&nbsp;в&nbsp;разрезе&nbsp;поселений";
            CrossLink3.NavigateUrl = "~/reports/FO_0016_0004_HMAO/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0016_0003_HMAO_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                //selectedIndicatorCaption.Value = "БК 1 (периодичность расчета показателя - ежеквартально)";

                ComboPeriod.PanelHeaderTitle = "Выберите даты";
                ComboPeriod.Width = 150;
                ComboPeriod.ShowSelectedValue = false;
                ComboPeriod.MultiSelect = true;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0016_0003_HMAO_nonEmptyPeriodList");
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                Node lastNode = ComboPeriod.GetLastNode(1);
                Node lastNodeParent = ComboPeriod.GetLastNode(1).Parent;
                Node prevNode = ComboPeriod.GetPreviousSublingNode(lastNode);
                ComboPeriod.SetSelectedNode(prevNode, true);
                ComboPeriod.SetSelectedNode(prevNode.Parent, true);
                ComboPeriod.SetSelectedNode(lastNode, true);
                ComboPeriod.SetSelectedNode(lastNodeParent, true);

                ComboIndicator.Title = "Показатель";
                ComboIndicator.Width = 600;
                ComboIndicator.MultiSelect = false;
                indicatorDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0016_0003_HMAO_indicatorList");
                ComboIndicator.ClearNodes();
                ComboIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(indicatorDigest.UniqueNames, indicatorDigest.MemberLevels));
                ComboIndicator.SetСheckedState("БК 1 (периодичность расчета показателя - ежеквартально)", true);
            }

            Page.Title = String.Format("Динамика изменения значений показателей соответствия требованиям бюджетного законодательства в муниципальных районах и городских округах Ханты-Мансийского автономного округа - Югры");
            PageTitle.Text = Page.Title;

            PageSubTitle.Text = String.Empty;

            periodSet.Value = String.Empty;
      
            foreach (string value in ComboPeriod.SelectedValues)
            {
                periodSet.Value += periodDigest.GetMemberUniqueName(value) + ",";
            }
            periodSet.Value = periodSet.Value.TrimEnd(',');
            
            selectedIndicatorName = ComboIndicator.SelectedValue;
            selectedIndicator.Value = indicatorDigest.GetMemberUniqueName(selectedIndicatorName);
            selectedQuarterIndex = ComboIndicator.SelectedIndex + 1;
            fullNameIndicator = indicatorDigest.GetMemberType(selectedIndicatorName);
            CRHelper.SaveToErrorLog(fullNameIndicator);
            hintFormatString = "N2";

            chartElementCaption.Text = String.Format("Показатель «{0}»", fullNameIndicator);
            UltraChart.TitleLeft.Text = indicatorUnit;

            UltraChart.ColorModel.AlphaLevel = 250;

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

      
        #region Обработчики диаграммы

        private string GetShortRegionName(string fullRegionName)
        {
            //string shortRegionName = fullRegionName.Replace("муниципальное образование", "МО");
            //shortRegionName = shortRegionName.Replace("муниципальный район", "МР");
            //shortRegionName = shortRegionName.Replace("\"", "'");
            string shortRegionName = fullRegionName.Replace(" ДАННЫЕ", "");
            shortRegionName = shortRegionName.Replace("(", "");
            shortRegionName = shortRegionName.Replace(")", "");
            return shortRegionName;
        }

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0016_0003_HMAO_chart");
            
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);
            CRHelper.SaveToErrorLog(dtChart.Rows.Count.ToString());
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

            if (dtChart.Rows.Count > 0)
            {
                UltraChart.Data.SwapRowsAndColumns = true;
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart.Series.Add(series);
                }
            }
            else
            {
                UltraChart.DataSource = null;
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
                                if (point.Series != null)
                                {
                                    point.DataPoint.Label = String.Format("{0}\n{1}: {2}", point.DataPoint.Label,
                                                                          point.Series.Label, pointValue);
                                }
                            }
                        }
                    }
                }
            }
        }

        void UltraChart_InvalidDataReceived(object sender, ChartDataInvalidEventArgs e)
        {
            if (!IsYear_IsThirdQuarter && selectedIndicatorName == "БК 5 (периодичность расчета показателя - по итогам 9 месяцев и за год)")
            {
                e.Text = "Нет данных, т.к. показатель расчитывается только по итогам 9 месяцев и за год";
            } 
            else
            {
                if (!IsFirstQuarter && selectedIndicatorName == "БК 6 (периодичность расчета показателя - по итогам 1 квартала)")
                {
                    e.Text = "Нет данных, т.к. показатель расчитывается только по итогам 1го квартала";
                }
                else
                {
                    e.Text = "Нет данных";
                }
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
}