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
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Common;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class Oil_0006_0002_Text : UserControl
    {
        public int ChartWidth = 750;
        public int ChartHeight = 235;
        private string queryName = "Oil_0006_0002_chart";
        private string avgQueryName = "Oil_0006_0002_avg";
        private double avg;

        DataTable dtChart;
        private DataTable dtAvg;

        private DateTime reportDate;
        public DateTime ReportDate
        {
            get
            {
                return reportDate;
            }
            set
            {
                reportDate = value;
            }
        }

        private DateTime lastDate;
        public DateTime LastDate
        {
            get
            {
                return lastDate;
            }
            set
            {
                lastDate = value;
            }
        }

        private string oilName = "������ ����� ��-80";
        public string OilName
        {
            get
            {
                return oilName;
            }
            set
            {
                oilName = value;
            }
        }

        private string oilId = "1";

        public string OilId
        {
            get
            {
                return oilId;
            }
            set
            {
                oilId = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UltraChart.ChartType = ChartType.StackBarChart;

            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            
            UltraChart.Data.SwapRowsAndColumns = true;

            UltraChart.Axis.Y.Extent = 180;
            UltraChart.Axis.Y.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Far;

            UltraChart.Axis.X.RangeType = AxisRangeType.Custom;
            UltraChart.Axis.X.RangeMax = 45;
            UltraChart.Axis.X.RangeMin = 20;
            UltraChart.Axis.X.Extent = 20;
            
            UltraChart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.X.Labels.Visible = true;

            UltraChart.Data.ZeroAligned = false;
            UltraChart.Legend.Visible = false;

            UltraChart.Tooltips.FormatString = String.Format("<ITEM_LABEL>");
            UltraChart.Tooltips.Font.Name = "Verdana";

            UltraChart.Axis.X.MajorGridLines.Visible = false;
            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.Text = "���.";
            UltraChart.TitleBottom.Font = new Font("Verdana", 8);
            UltraChart.TitleBottom.FontColor = Color.FromArgb(209,209,209);
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Far;
            UltraChart.TitleBottom.Margins.Left = UltraChart.Axis.Y.Extent;
            
            CustomParams.MakeOilParams(oilId, "id");
            IncomesHeader.MultitouchReport = String.Format("Oil_0006_0004_OIL={0}", oilId);

            IncomesHeader.Text = oilName;

            SetColorModel();

            AvgDataBind();

            ChartDataBind();
        }

        private void AvgDataBind()
        {
            string query = DataProvider.GetQueryText(avgQueryName);
            dtAvg = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����������", dtAvg);

            if (dtAvg.Rows.Count > 0)
            {
                avg = GetRowValue(dtAvg.Rows[0], "�������");
            }
        }

        private void ChartDataBind()
        {
            string query = DataProvider.GetQueryText(queryName);
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����������", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                UltraChart.Width = ChartWidth;

                UltraChart.Height = dtChart.Rows.Count * 40 + 60;

                if (dtChart.Columns.Count > 0)
                {
                    dtChart.Columns.RemoveAt(0);
                }

                UltraChart.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart.Series.Add(series);
                }

                UltraChart.DataBind();

                UltraChart.Visible = true;
                Label1.Text = String.Format("�� <span class='DigitsValue'>{0:dd.MM.yyyy}</span> �. ���� ��� �� <span class='DigitsValue'>{2}</span> � {1:dd.MM.yyyy} �. ����������� � ��������� ��������� �� � ���������",
                    reportDate, lastDate, CRHelper.ToLowerFirstSymbol(oilName));
            }
            else
            {
                UltraChart.Visible = false;
                Label1.Text = String.Format("�� <span class='DigitsValue'>{0:dd.MM.yyyy}</span> �. � ��������� �� <span class='DigitsValue'>�� �����������</span> ����� ��� � {1:dd.MM.yyyy} �. �� <span class='DigitsValue'>{2}</span>",
                    reportDate, lastDate, CRHelper.ToLowerFirstSymbol(oilName));
            }
        }

        private void SetColorModel()
        {
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            Color color1 = Color.LightBlue;
            Color color2 = Color.Red;

            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            UltraChart.ColorModel.Skin.ApplyRowWise = false;

            UltraChart.Effects.Effects.Clear();
            GradientEffect effect = new GradientEffect();
            effect.Coloring = GradientColoringStyle.Darken;
            effect.Enabled = true;
            UltraChart.Effects.Enabled = true;
            UltraChart.Effects.Effects.Add(effect);
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            double xMax = xAxis.MapMaximum;
            double yMax = yAxis.MapMaximum;
            double yMin = yAxis.MapMinimum;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text axisText = (Text)primitive;
                    axisText.labelStyle.VerticalAlign = StringAlignment.Near;
                    axisText.labelStyle.FontSizeBestFit = false;
                    axisText.labelStyle.Font = new Font("Verdana", 8);
                    axisText.labelStyle.WrapText = false;
                }
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.Y"))
                {
                    Text text = (Text)primitive;
                    if (text.Row == -1)
                    {
                        text.bounds.Width = 170;

                        text.labelStyle.HorizontalAlign = StringAlignment.Far;

                        text.labelStyle.VerticalAlign = StringAlignment.Center;
                        text.labelStyle.FontSizeBestFit = false;
                        text.labelStyle.Font = new Font("Verdana", 8);
                        text.labelStyle.WrapText = true;
                    }
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        string serieName = GetRowStringValue(dtChart.Rows[box.Row], "������������").Replace("\"", "'");
                        double lastValue = GetRowValue(dtChart.Rows[box.Row], "�� ������ ����");
                        double increaseValue = GetRowValue(dtChart.Rows[box.Row], "�������");
                        
                        if (lastValue != Double.MinValue && increaseValue != Double.MinValue)
                        {
                            double currValue = lastValue + increaseValue;
                            double increasePercent = currValue == 0 ? 0 : increaseValue / currValue;

                            box.DataPoint.Label = String.Format("{0}\n{1}\n{2:dd.MM.yyyy}: <b>{3:N2}</b> ���.\n{4:dd.MM.yyyy}: <b>{5:N2}</b> ���.\n���� <b>{6:N2}</b> ���. (<b>+{7:P2}</b>)",
                                        serieName, oilName, lastDate, lastValue, reportDate, currValue, increaseValue, increasePercent);

                            if (box.Column == 0)
                            {
                                int textWidth = 140;

                                Text text = new Text();
                                text.bounds = new Rectangle(Convert.ToInt32(xMax - textWidth), box.rect.Top, textWidth, box.rect.Height);
                                text.labelStyle.Font = new Font("Verdana", 9);
                                text.labelStyle.FontColor = Color.White;
                                text.SetTextString(String.Format("+{0:N2} ���. ({1:P2})", increaseValue, increasePercent));
                                e.SceneGraph.Add(text);
                            }
                        }
                    }
                }
            }

            if (avg != Double.MinValue)
            {
                int textWidth = 130;
                int textHeight = 30;

                Line line = new Line();
                line.p1 = new Point((int)xAxis.Map(avg), (int)yMin + textHeight);
                line.p2 = new Point((int)xAxis.Map(avg), (int)yMax);
                line.PE.Stroke = Color.White;
                line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                e.SceneGraph.Add(line);

               Text text = new Text();
                text.bounds = new Rectangle(Convert.ToInt32(xAxis.Map(avg) - textWidth * 0.5), (int)yMin + textHeight - 5, textWidth, textHeight);
                text.labelStyle.Font = new Font("Verdana", 9);
                text.labelStyle.FontColor = Color.White;
                text.labelStyle.HorizontalAlign = StringAlignment.Center;
                text.SetTextString(String.Format("������� �� ��\n{0:N2} ���.", avg));
                e.SceneGraph.Add(text);
            }
        }

        private static double GetRowValue(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                if (row[columnName] != DBNull.Value && row[columnName].ToString() != String.Empty)
                {
                    return Convert.ToDouble(row[columnName]);
                }
            }

            return Double.MinValue;
        }

        private static string GetRowStringValue(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                if (row[columnName] != DBNull.Value && row[columnName].ToString() != String.Empty)
                {
                    return row[columnName].ToString();
                }
            }

            return String.Empty;
        }
    }
}