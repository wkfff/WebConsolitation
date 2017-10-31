using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class SE_0001_0009_Chart : UserControl
    {
        private DateTime reportDate;

        private string queryName = String.Empty;

        public string QueryName
        {
            get
            {
                return queryName;
            }
            set
            {
                queryName = value;
            }
        }

        private int year = 2000;
        private int monthNum = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            InitializeDate();

            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            DataTable dtIncomes = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);

            SetColumnChartAppearance(Convert.ToDouble(dtIncomes.Rows[0][1]));

            UltraChart1.DataSource = dtIncomes;
            UltraChart1.DataBind();
        }

        private void InitializeDate()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("SE_0001_0002_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0]);
            reportDate = new DateTime(year, monthNum, 1);

            CustomParams UserParams = new CustomParams();
            UserParams.PeriodMonth.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate, 4);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate.AddYears(-1), 4);
            UserParams.PeriodLastYear.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate.AddYears(-1), 1);
            UserParams.PeriodEndYear.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate.AddYears(-2), 1);
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            Collection<int> columnsX = new Collection<int>();
            Collection<int> columnsHeights = new Collection<int>();
            Collection<string> columnsValues = new Collection<string>();
            Collection<string> regions = new Collection<string>();
            int columnWidth = 0;
            int minHeight = 0;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        columnsX.Add(box.rect.X);
                        columnsHeights.Add(box.rect.Height);
                        columnWidth = box.rect.Width;
                        if (box.Value != null)
                        {
                            columnsValues.Add(box.Value.ToString());
                            if (box.Value.ToString().Contains("-") && box.rect.Height > minHeight)
                            {
                                minHeight = box.rect.Height;
                            }
                        }

                        regions.Add(RegionsNamingHelper.ShortName(box.DataPoint.Label));
                    }
                }
            }

            for (int i = 0; i < columnsValues.Count; i++)
            {
                double value;
                if (double.TryParse(columnsValues[i].ToString(), out value))
                {
                    Text text = new Text();
                    text.labelStyle.Font = new Font("Arial", 12);
                    text.PE.Fill = Color.White;
                    int yPos = value > 0 ? 115 - columnsHeights[i] : (int)(UltraChart1.Height.Value) - 160 + columnsHeights[i];

                    text.bounds = new Rectangle(columnsX[i], yPos, columnWidth, 20);
                    text.SetTextString(value.ToString("N1"));
                    text.labelStyle.HorizontalAlign = StringAlignment.Center;
                    e.SceneGraph.Add(text);

                    text = new Text();
                    text.labelStyle.Font = new Font("Arial", 12);
                    text.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;

                    text.PE.Fill = Color.White;
                    yPos = (int)(UltraChart1.Height.Value) - 140 + minHeight;
                    text.bounds = new Rectangle(columnsX[i], yPos, columnWidth, 100);
                    text.SetTextString(regions[i]);
                    text.labelStyle.HorizontalAlign = StringAlignment.Far;
                    text.labelStyle.VerticalAlign = StringAlignment.Center;
                    e.SceneGraph.Add(text);
                }
            }
        }

        private void SetColumnChartAppearance(double value)
        {
            UltraChart1.Width = 750;
            UltraChart1.Height = 300;
            UltraChart1.ChartType = ChartType.ColumnChart;

            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>%";
            UltraChart1.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL><br /><b><DATA_VALUE:N1></b>%.</span>";

            UltraChart1.Data.ZeroAligned = true;
            UltraChart1.Legend.Visible = false;

            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 12);
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 12);
            UltraChart1.Legend.Margins.Bottom = 70;
            UltraChart1.Legend.SpanPercentage = 20;

            UltraChart1.Axis.X.MajorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart1.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            UltraChart1.Axis.X.MinorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MinorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
            UltraChart1.Axis.Y.RangeMin = -50;
            UltraChart1.Axis.Y.RangeMax = 40;

            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart1.Axis.X.Extent = 0;
            UltraChart1.Axis.X2.Extent = 0;
            UltraChart1.Axis.Y.Extent = 40;
            UltraChart1.Style.Add("margin-left", "-5px");
            UltraChart1.TitleLeft.Visible = false;

            UltraChart1.BackColor = Color.Transparent;
            UltraChart1.BorderColor = Color.Transparent;
            SetupCustomSkin(value);
        }

        private void SetupCustomSkin(double value)
        {
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 20; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetColor(i);

                if (i == 2)
                {
                    if (value < 0)
                    {
                        color = Color.Orange;
                        stopColor = Color.Orange;
                    }
                    //else
                    //{
                    //    color = Color.LimeGreen;
                    //    stopColor = Color.LimeGreen;
                    //}
                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = i == 2 ? PaintElementType.Gradient : PaintElementType.Hatch;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.Hatch = FillHatchStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 2:
                    {
                        return Color.FromArgb(145, 10, 149);
                    }
                default:
                    {
                        return Color.DarkGray;
                    }
            }
        }
    }
}