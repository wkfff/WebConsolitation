using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0003
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

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.ScatterChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.Y.Extent = 50;
            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Text = "Бюджетные расходы на душу населения, руб./чел.";
            UltraChart.TitleLeft.Font = new Font("Verdana", 12);
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;

            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.Text = "Численность населения, тыс.чел.";
            UltraChart.TitleBottom.Font = new Font("Verdana", 12);
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 20;
            UltraChart.ScatterChart.Icon = SymbolIcon.Square;
            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<DATA_VALUE_X:N3> тыс.чел.\n<DATA_VALUE_Y:N2> руб./чел.";
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            CrossLink1.Text = "Таблица&nbsp;исполнения&nbsp;расходов&nbsp;и&nbsp;бюджетных&nbsp;расходов&nbsp;на&nbsp;душу&nbsp;населения";
            CrossLink1.NavigateUrl = "~/reports/FK_0001_0003/DefaultCompare.aspx";

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            UltraGridExporter1.ExcelExportButton.Visible = false;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);

            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 200);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0003_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
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

                ComboFKR.Width = 320;
                ComboFKR.Title = "РзПр";
                ComboFKR.MultiSelect = false;
                ComboFKR.ParentSelect = true;
                ComboFKR.FillDictionaryValues(CustomMultiComboDataHelper.FillFKRNames(
                                                  DataDictionariesHelper.OutcomesFKRTypes,
                                                  DataDictionariesHelper.OutcomesFKRLevels));
                ComboFKR.SetСheckedState("Расходы бюджета - ИТОГО", true);

                regionsCombo.Width = 300;
                regionsCombo.Title = "Субъект РФ";
                regionsCombo.FillDictionaryValues(
                    CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames,
                                                           RegionsNamingHelper.RegionsFoDictionary));
                regionsCombo.ParentSelect = false;
               
                if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    regionsCombo.SetСheckedState(RegionSettings.Instance.Name, true);
                }

                selectedPointIndex = -1;
            }

            UserParams.Region.Value = regionsCombo.SelectedNodeParent;
            UserParams.StateArea.Value = regionsCombo.SelectedValue;

            UserParams.Filter.Value = (allRegionsCheckBox.Checked)
                                          ? string.Format(
                                                "and [Территории].[Сопоставимый].CurrentMember.Parent is [Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[{0}]",
                                                UserParams.Region.Value)
                                          : " ";

            UserParams.SelectItem.Value = (allRegionsCheckBox.Checked)
                                              ? string.Format(
                                                    "[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[{0}]",
                                                    UserParams.Region.Value)
                                              : "[Территории].[Сопоставимый].[Все территории].[Российская  Федерация]";

            Page.Title = string.Format("Распределение субъектов {0} по бюджетным расходам на душу населения",
                                       (!allRegionsCheckBox.Checked)
                                           ? "РФ"
                                           : RegionsNamingHelper.ShortName(UserParams.Region.Value));
            Label1.Text = Page.Title;
            Label2.Text = string.Empty;

            string monthValue = ComboMonth.SelectedValue;
            string yearValue = ComboYear.SelectedValue;
            string fkrValue = DataDictionariesHelper.OutcomesFKRTypes[ComboFKR.SelectedValue];

            int year = Convert.ToInt32(ComboYear.SelectedValue);

            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value =
                string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value =
                string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.FKRFilter.Value = DataDictionariesHelper.OutcomesFKRTypes[ComboFKR.SelectedValue];

            populationDate = DataDictionariesHelper.GetFederalPopulationDate();

            if (allRegionsCheckBox.Checked)
            {
                UltraChart.ScatterChart.UseGroupByColumn = true;
                UltraChart.ScatterChart.IconSize = SymbolIconSize.Medium;
                UltraChart.Legend.Location = LegendLocation.Right;
                UltraChart.Legend.SpanPercentage = 20;

                UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;
                int count = 20;

                for (int i = 1; i < count; i++)
                {
                    Color color = GetCustomColor(i);
                    UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color, 150));
                }
            }
            else
            {
                UltraChart.ScatterChart.UseGroupByColumn = false;
                UltraChart.Legend.SpanPercentage = 10;
                UltraChart.ColorModel.ModelStyle = ColorModels.CustomLinear;

            }

            UltraChart.DataBind();
        }

        private static Color GetCustomColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.Cyan;
                    }
                case 2:
                    {
                        return Color.LightSkyBlue;
                    }
                case 3:
                    {
                        return Color.Gold;
                    }
                case 4:
                    {
                        return Color.Peru;
                    }
                case 5:
                    {
                        return Color.Indigo;
                    }
                case 6:
                    {
                        return Color.PeachPuff;
                    }
                case 7:
                    {
                        return Color.MediumSlateBlue;
                    }
                case 8:
                    {
                        return Color.ForestGreen;
                    }
                case 9:
                    {
                        return Color.HotPink;
                    }
                case 10:
                    {
                        return Color.Gray;
                    }
                case 11:
                    {
                        return Color.Blue;
                    }
                case 12:
                    {
                        return Color.Magenta;
                    }
                case 13:
                    {
                        return Color.DarkBlue;
                    }
                case 14:
                    {
                        return Color.DarkRed;
                    }
                case 15:
                    {
                        return Color.DarkSalmon;
                    }
                case 16:
                    {
                        return Color.DarkOrange;
                    }
                case 18:
                    {
                        return Color.RosyBrown;
                    }
                case 19:
                    {
                        return Color.DarkGray;
                    }

            }
            return Color.White;
        }

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0003_allocation_chart");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtChart);

            query = DataProvider.GetQueryText("FK_0001_0003_allocation_avg");
            dtAVG = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Среднее", dtAVG);

            selectedPointIndex = -1;
            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                if (dtChart.Rows[i][0] != DBNull.Value && dtChart.Rows[i][0].ToString() == UserParams.StateArea.Value)
                {
                    selectedPointIndex = i;
                    break;
                }
            }

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            subjectLabel.Text = string.Format("{0}:", UserParams.StateArea.Value);
            populationLabel.Text = string.Empty;
            statisticLabel.Text = string.Empty;
            if (selectedPointIndex != -1 && dtChart.Rows[selectedPointIndex][1] != DBNull.Value &&
                dtChart.Rows[selectedPointIndex][2] != DBNull.Value)
            {
                populationLabel.Text = string.Format("Численность постоянного населения <b>{0}</b> тыс.чел. ({1})",
                        dtChart.Rows[selectedPointIndex][1], populationDate);

                double income = Convert.ToDouble(dtChart.Rows[selectedPointIndex][2]);
                statisticLabel.Text = string.Format("За {1} {2} {3} года бюджетные расходы на душу населения ({4}) составляют <b>{0}</b>&nbsp;руб./чел.",
                    income.ToString("N2"), monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum, ComboFKR.SelectedValue);
            }

            UltraChart.DataSource = dtChart;
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            if (!allRegionsCheckBox.Checked)
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
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Box)
                    {
                        Box box = (Box) primitive;

                        if (box.DataPoint != null)
                        {
                            if (box.DataPoint.Label != null)
                            {
                                CRHelper.SaveToErrorLog(box.DataPoint.Label);
                            }
                        }
                        else if (box.Path != null && box.Path.ToLower().Contains("legend"))
                        {
                            Point point = new Point(160, 14);
                            Box nBox = new Box(point, 16, 16);
                            nBox.PE.ElementType = PaintElementType.CustomBrush;
                            LinearGradientBrush brush = new LinearGradientBrush(nBox.rect, Color.Orange, Color.Orange,
                                                                                45, false);
                            nBox.PE.CustomBrush = brush;
                            e.SceneGraph.Add(nBox);

                            point = new Point(177, 22);
                            Text textRegion = new Text(point, string.Format("{0}", regionsCombo.SelectedValue));
                            textRegion.PE.Fill = Color.Black;
                            e.SceneGraph.Add(textRegion);
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
            int lineLength = (int)xAxis.MapMaximum-80;

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
            string regionStr = (!allRegionsCheckBox.Checked) ? "РФ" : RegionsNamingHelper.ShortName(UserParams.Region.Value);
            text.SetTextString(string.Format("Среднее по {1}: {0:N2} руб./чел.", avgValue, regionStr));
            e.SceneGraph.Add(text);
        }

        #endregion

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

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.86));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);
        }

        private string RemoveBoldTags(string source)
        {
            string result = source.Replace("<b>", string.Empty);
            result = result.Replace("</b>", string.Empty);
            return result;
        }
    }
}
