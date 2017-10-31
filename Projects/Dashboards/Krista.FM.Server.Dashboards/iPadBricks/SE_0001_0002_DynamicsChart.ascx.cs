using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class SE_0001_0002_DynamicsChart : UserControl
    {
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

        public string Text
        {
            get { return IPadElementHeader1.Text; }
            set { IPadElementHeader1.Text = value; }
        }

        public string Width
        {
            get
            {
                return IPadElementHeader1.Width;
            }
            set
            {
                IPadElementHeader1.Width = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            SetupChart(UltraChart1, 33);
            AddLineAppearencesUltraChart(UltraChart1, Color.FromArgb(78, 230, 228), Color.FromArgb(168, 48, 137), Color.FromArgb(170, 70, 53), Color.FromArgb(3, 12, 200));
           
            UltraChart1.DataSource = GetDataSource(queryName);
            UltraChart1.DataBind();
        }

        private void SetupChart(UltraChart chart, int legendSpanPercentage)
        {
            chart.Width = 740;
            chart.Height = 250;
            chart.ChartType = ChartType.SplineChart;
            chart.SplineChart.NullHandling = NullHandling.DontPlot;
            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            chart.TitleLeft.FontColor = Color.FromArgb(209, 209, 209);
            chart.TitleLeft.Font = new Font("Verdana", 10);
            chart.Axis.X.Extent = 110;
            chart.Axis.Y.Extent = 40;
            chart.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            chart.Axis.Y.Margin.Far.Value = 10;

            chart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL>\n<b><DATA_VALUE:N1></b></span>";

            chart.Legend.Visible = false;
            chart.Legend.Location = LegendLocation.Right;
            chart.Legend.SpanPercentage = legendSpanPercentage;

            chart.Axis.Y.Labels.Visible = true;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.Axis.Y.Labels.Font = new Font("Verdana", 10);

            chart.Axis.X.Labels.Visible = true;
            chart.Axis.X.Labels.Font = new Font("Verdana", 10);
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is PointSet)
                {
                    PointSet pointSet = (PointSet)primitive;
                    foreach (DataPoint point in pointSet.points)
                    {
                        point.hitTestRadius = 20;
                        if (point.DataPoint != null)
                        {
                            point.DataPoint.Label = "1";
                        }
                    }
                }
            }
        }

        private string axisMonth = String.Empty;

        private void ReplaceAxisLabels(SceneGraph grahp)
        {
            for (int i = 0; i < grahp.Count; i++)
            {
                Primitive primitive = grahp[i];
                if (primitive is Text)
                {
                    string text = ((Text)primitive).GetTextString();
                    text = text.Trim();
                    // Проверяем формат
                    string[] textArray = text.Split();
                    if (textArray.Length == 2)
                    {
                        int day;
                        if (Int32.TryParse(textArray[0], out day) && CRHelper.IsMonthCaption(textArray[1]))
                        {
                            if (axisMonth == textArray[1])
                            {
                                ((Text)primitive).SetTextString(day.ToString());
                            }
                            else
                            {
                                ((Text)primitive).SetTextString(String.Format("{0}-{1}",
                                              CRHelper.ToUpperFirstSymbol(
                                                  CRHelper.RusMonth(CRHelper.MonthNum(textArray[1]))), day));
                                axisMonth = textArray[1];
                            }
                        }
                    }
                }
            }
        }

        private void AddLineAppearencesUltraChart(UltraChart chart, Color SeriesColor1, Color SeriesColor2, Color SeriesColor3, Color SeriesColor4)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.ApplyRowWise = true;
            chart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 4; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = SeriesColor1;
                            stopColor = SeriesColor1;
                            break;
                        }
                    case 2:
                        {
                            color = SeriesColor2;
                            stopColor = SeriesColor2;
                            break;
                        }

                    case 3:
                        {
                            color = SeriesColor3;
                            stopColor = SeriesColor3;
                            break;
                        }
                    case 4:
                        {
                            color = SeriesColor4;
                            stopColor = SeriesColor4;
                            break;
                        }
                }

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.SolidFill;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                pe.StrokeWidth = 4;
                chart.ColorModel.Skin.PEs.Add(pe);

                LineAppearance lineAppearance2 = new LineAppearance();
                lineAppearance2.IconAppearance.Icon = SymbolIcon.Diamond;
                lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Medium;
                lineAppearance2.IconAppearance.PE = pe;
                lineAppearance2.Thickness = 10;

                chart.SplineChart.LineAppearances.Add(lineAppearance2);
            }
        }
        
        private static DataTable GetDataSource(string queryName)
        {
            DataTable dt = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);
            string year = String.Empty;

            for (int i = 1; i < dt.Columns.Count; i++)
            {
                if (dt.Rows[0][i].ToString() == year)
                {
                    dt.Columns[i].ColumnName = String.Format("{0}", dt.Columns[i].ColumnName);
                }
                else
                {
                    dt.Columns[i].ColumnName = String.Format("{0} - {1}", dt.Rows[0][i], dt.Columns[i].ColumnName);
                    year = dt.Rows[0][i].ToString();
                }

            }
            dt.Rows.RemoveAt(0);
            dt.AcceptChanges();
            return dt;
        }
    }
}