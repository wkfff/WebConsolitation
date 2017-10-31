using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0007_0003 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private DateTime lastDataDate;
        private DataTable dtData = new DataTable();

        private DateTime date;

        #region Параметры запроса

        // мера План
        private CustomParam measurePlan;
        // мера Факт
        private CustomParam measureFact;
        // мера Остаток на начало
        private CustomParam measureStartBalance;
        // мера Остаток на конец
        private CustomParam measureEndBalance;

        #endregion

        public bool IsQuaterPlanType
        {
            get
            {
                return RegionSettingsHelper.Instance.CashPlanType.ToLower() == "quarter";
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraChart1.Width = 765;
            UltraChart1.Height = 780;

            UltraChart1.Style.Add("margin-left", "-10px");
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0007_0003_rests_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();
            date = CRHelper.PeriodDayFoDate(UserParams.PeriodCurrentDate.Value);

            SetStackChartAppearanceUnic();
            BindChartData("ChartOutcomesAll");

             Label16.Text = String.Format("Динамика структуры государственного внутреннего долга<br/>Омской области на&nbsp;{0:dd.MM.yyyy}, тыс.руб.",
                    date.AddMonths(1));
        }

        private void SetLabelText(Label label, object cell)
        {
            if (cell.ToString() != "0")
            {
                label.Text = String.Format(label.Text, String.Format("&nbsp;<span style=\"color: White\"><b>{0:N1}</b></span>&nbsp;", cell)) + "<br/>";
            }
            else
            {
                label.Visible = false;
            }
        }

        private void SetStackChartAppearanceUnic()
        {
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.ChartType = ChartType.StackColumnChart;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart1.Axis.X.Labels.SeriesLabels.OrientationAngle = 90;

            UltraChart1.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            UltraChart1.Axis.Y.Margin.Far.Value = 80;

            UltraChart1.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL> <br />  <SERIES_LABEL> <br /><b><DATA_VALUE:N1></b>&nbsp;тыс.руб.</span>";
            SetLabelsClipTextBehavior(UltraChart1.Axis.X.Labels.SeriesLabels.Layout);

            UltraChart1.Axis.X.TickmarkStyle = AxisTickStyle.Percentage;
            UltraChart1.Axis.X.TickmarkInterval = 50;
            //UltraChart1.Axis.X.TickmarkIntervalType = AxisIntervalType.Months;

            UltraChart1.Axis.X.MajorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart1.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            UltraChart1.Axis.X.MinorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MinorGridLines.Color = Color.Black;

            UltraChart1.Axis.X.Labels.FontSizeBestFit = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.FontSizeBestFit = false;
            UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;

            UltraChart1.Axis.Y.Labels.FontSizeBestFit = false;
            UltraChart1.Axis.Y.Labels.SeriesLabels.FontSizeBestFit = false;
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.Y.Labels.Visible = true;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Visible = true;

            UltraChart1.Axis.Y.Extent = 80;
            UltraChart1.Axis.X.Extent = 110;

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "тыс.руб.";
            UltraChart1.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart1.TitleLeft.FontColor = Color.White;
            UltraChart1.TitleLeft.VerticalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = 350;
            UltraChart1.Legend.Font = new Font("Verdana", 10);
            UltraChart1.Legend.SpanPercentage = 20;

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 10; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetStopColor(i);

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }
            //UltraChart1.Legend.Visible = false;
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            Collection<int> columnsX = new Collection<int>();
            Dictionary<int, int> columnsHeights = new Dictionary<int, int>();
            Dictionary<int, double> columnsValues = new Dictionary<int, double>();
            int columnWidth = 0;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (!columnsX.Contains(box.rect.X))
                        {
                            columnsX.Add(box.rect.X);
                            columnsHeights.Add(box.rect.X, box.rect.Height);
                            columnsValues.Add(box.rect.X, Convert.ToDouble(box.Value));
                            columnWidth = box.rect.Width;
                        }
                        else
                        {
                            columnsHeights[box.rect.X] += box.rect.Height;
                            columnsValues[box.rect.X] = Convert.ToDouble(box.Value) == 0 ? columnsValues[box.rect.X] : Convert.ToDouble(box.Value);
                        }
                    }
                }

                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Legend"))
                {
                    Text text = (Text)primitive;
                    text.SetTextString(text.GetTextString().Replace("<br/>", String.Empty));
                }
            }

            for (int i = 0; i < columnsX.Count; i++)
            {

                Text text = new Text();
                text.labelStyle.Font = new Font("Arial", 12);
                text.PE.Fill = Color.White;

                //int yPos = value > 0 ? axisZero - columnsHeights[i] - 20 : axisZero + columnsHeights[i] + 20;

                text.bounds = new Rectangle(columnsX[i], 535 - columnsHeights[columnsX[i]], columnWidth, 120);
                text.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
                text.SetTextString(columnsValues[columnsX[i]].ToString("N1"));
                text.labelStyle.HorizontalAlign = StringAlignment.Near;
                text.labelStyle.VerticalAlign = StringAlignment.Center;
                e.SceneGraph.Add(text);
            }
        }

        private void BindChartData(string queryId)
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText(queryId);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtChart);

            RemoveRedudantRows(dtChart);

            foreach (DataColumn col in dtChart.Columns)
            {
                col.ColumnName = iPadBricks.iPadBricks.iPadBricksHelper.GetWarpedHint(col.ColumnName);
            }

            UltraChart1.DataSource = dtChart;

            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.DataBind();
        }

        private string axisMonth = String.Empty;

        private void RemoveRedudantRows(DataTable dtSource)
        {
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                int year;

                if (!Int32.TryParse(dtSource.Rows[i][0].ToString(), out year))
                {
                    dtSource.Rows[i][0] = String.Format("на {0:dd.MM.yyyy}", new DateTime(Convert.ToInt32(dtSource.Rows[i][6].ToString()), CRHelper.MonthNum(dtSource.Rows[i][0].ToString()), 1).AddMonths(1));
                }
                else
                {
                    dtSource.Rows[i][0] = String.Format("на 01.01.{0}", year + 1);
                }
            }

            dtSource.Columns.RemoveAt(6);
            dtSource.AcceptChanges();
        }

        private void SetLabelsClipTextBehavior(AxisLabelLayoutAppearance layout)
        {
            layout.Behavior = AxisLabelLayoutBehaviors.UseCollection;
            layout.BehaviorCollection.Clear();
            ClipTextAxisLabelLayoutBehavior behavior = new ClipTextAxisLabelLayoutBehavior();

            behavior.ClipText = false;
            behavior.Enabled = true;
            behavior.Trimming = StringTrimming.None;
            behavior.UseOnlyToPreventCollisions = false;
            layout.BehaviorCollection.Add(behavior);
        }

        private void SplitColumnNames()
        {
            foreach (DataColumn col in dtChart.Columns)
            {
                string[] names = col.ColumnName.Split(';');
                if (names.Length == 2)
                {
                    col.ColumnName = names[0];
                }
            }
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.FromArgb(52, 104, 166);
                    }
                case 2:
                    {
                        return Color.FromArgb(168, 56, 53);
                    }
                case 3:
                    {
                        return Color.FromArgb(249, 249, 4);
                    }
                case 4:
                    {
                        return Color.FromArgb(102, 78, 130);
                    }
                case 5:
                    {
                        return Color.FromArgb(188, 104, 56);
                    }
                case 6:
                    {
                        return Color.FromArgb(240, 240, 240);
                    }
                case 7:
                    {
                        return Color.FromArgb(217, 230, 117);
                    }
                case 8:
                    {
                        return Color.FromArgb(162, 154, 98);
                    }
                case 9:
                    {
                        return Color.FromArgb(143, 199, 219);
                    }
                case 10:
                    {
                        return Color.FromArgb(176, 217, 117);
                    }
            }
            return Color.White;
        }

        private static Color GetStopColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.FromArgb(52, 104, 166);
                    }
                case 2:
                    {
                        return Color.FromArgb(168, 56, 53);
                    }
                case 3:
                    {
                        return Color.FromArgb(249, 249, 4);
                    }
                case 4:
                    {
                        return Color.FromArgb(102, 78, 130);
                    }
                case 5:
                    {
                        return Color.FromArgb(188, 104, 56);
                    }
                case 6:
                    {
                        return Color.FromArgb(240, 240, 240);
                    }
                case 7:
                    {
                        return Color.FromArgb(164, 184, 10);
                    }
                case 8:
                    {
                        return Color.FromArgb(110, 98, 8);
                    }
                case 9:
                    {
                        return Color.FromArgb(11, 100, 131);
                    }
                case 10:
                    {
                        return Color.FromArgb(102, 168, 9);
                    }
            }
            return Color.White;
        }

    }
}
