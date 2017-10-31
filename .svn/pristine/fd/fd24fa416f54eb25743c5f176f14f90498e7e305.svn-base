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
    public partial class FO_0007_0002 : CustomReportPage
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

            UltraChart1.Width = 750;
            UltraChart1.Height = 850;

            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            int legendBoxCount = 0;
            int legendTextCount = 0;

            Collection<int> columnsX = new Collection<int>();
            Dictionary<int, int> columnsHeights = new Dictionary<int, int>();
            Dictionary<int, double> columnsValues = new Dictionary<int, double>();
            int columnWidth = 0;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box &&
                        primitive.Path.Contains("Legend"))
                {
                    Box box = (Box)primitive;
                    if (box.rect.Width == box.rect.Height)
                    {
                        legendBoxCount++;
                        if (legendBoxCount == 1)
                        {
                            box.PE.Fill = Color.ForestGreen;
                            box.PE.FillStopColor = Color.ForestGreen;
                        }
                        if (legendBoxCount == 2)
                        {
                            box.PE.Fill = Color.Red;
                            box.PE.FillStopColor = Color.Red;
                        }                            
                        if (legendBoxCount > 2)
                        {
                            box.Visible = false;
                        }
                    }
                }

                if (primitive is Text &&
                        primitive.Path == null)
                {
                    Text text = (Text)primitive;
                    legendTextCount++;
                    if (legendTextCount == 1)
                    {
                        text.SetTextString("Профицит");
                    }
                    if (legendTextCount == 2)
                    {
                        text.SetTextString("Дефицит");
                    }
                    if (legendTextCount > 2)
                    {
                        text.Visible = false;
                    }
                }

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

                        if (Convert.ToDouble(box.Value) < 0)
                        {
                            box.DataPoint.Label = "Дефицит";
                            box.Series.Label = String.Format("{0}<br/><b>{1:N1}</b>", box.Series.Label, Math.Abs(Convert.ToDouble(box.Value)));
                        }
                        else
                        {
                            box.DataPoint.Label = "Профицит";
                            box.Series.Label = String.Format("{0}<br/><b>{1:N1}</b>", box.Series.Label, box.Value);
                        }
                    }
                }
            }

            for (int i = 0; i < columnsX.Count; i++)
            {

                Text text = new Text();
                text.labelStyle.Font = new Font("Arial", 12);
                text.PE.Fill = Color.White;

                int yPos = columnsValues[columnsX[i]] > 0 ? 100 - columnsHeights[columnsX[i]] : 200 + columnsHeights[columnsX[i]];
                text.labelStyle.HorizontalAlign = columnsValues[columnsX[i]] > 0 ? StringAlignment.Near : StringAlignment.Far;

                text.bounds = new Rectangle(columnsX[i], yPos, columnWidth, 120);
                text.labelStyle.Orientation = TextOrientation.VerticalLeftFacing;
                text.SetTextString(columnsValues[columnsX[i]].ToString("N1"));
                
                text.labelStyle.VerticalAlign = StringAlignment.Center;
                e.SceneGraph.Add(text);
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0007_0002_rests_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();
            date = CRHelper.PeriodDayFoDate(UserParams.PeriodCurrentDate.Value);
            
            dtChart = new DataTable();
            query = DataProvider.GetQueryText("ChartOutcomesAll");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(
                query, "name", dtChart);

            RemoveRedudantRows(dtChart);
            SetStackChartAppearanceUnic();

            UltraChart1.Series.Clear();

            for (int i = 1; i < dtChart.Columns.Count; i++)
            {
                UltraChart1.Series.Add(CRHelper.GetNumericSeries(i, dtChart));
            }

             //   UltraChart1.DataSource = dtChart;
            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.DataBind();

            Label16.Text = String.Format("Результат исполнения областного бюджета Омской области<br/>в динамике на&nbsp;{0:dd.MM.yyyy}, тыс.руб.",
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
            UltraChart1.ChartType = ChartType.StackColumnChart;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart1.Axis.X.Labels.SeriesLabels.OrientationAngle = 90;
            
            UltraChart1.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL> <SERIES_LABEL>&nbsp;тыс.руб.</span>";
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

            UltraChart1.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;
            UltraChart1.Axis.Y.Margin.Far.Value = 60;

            UltraChart1.Axis.Y.Margin.Near.MarginType = LocationType.Pixels;
            UltraChart1.Axis.Y.Margin.Near.Value = 70;

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "тыс.руб.";
            UltraChart1.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart1.TitleLeft.FontColor = Color.White;
            UltraChart1.TitleLeft.VerticalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = 350;
            UltraChart1.Legend.Font = new Font("Verdana", 10);
            UltraChart1.Legend.SpanPercentage = 5;

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = true;
            UltraChart1.ColorModel.Skin.PEs.Clear();

            for (int i = 1; i <= dtChart.Rows.Count; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(dtChart.Rows[i - 1][1].ToString());
                Color stopColor = GetColor(dtChart.Rows[i - 1][1].ToString());

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }

           // UltraChart1.Legend.Visible = false;
        }

        private string axisMonth = String.Empty;

        private void RemoveRedudantRows(DataTable dtSource)
        {
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                int year;

                if (!Int32.TryParse(dtSource.Rows[i][0].ToString(), out year))
                {
                    dtSource.Rows[i][0] = String.Format("на {0:dd.MM.yyyy}", new DateTime(Convert.ToInt32(dtSource.Rows[i][2].ToString()), CRHelper.MonthNum(dtSource.Rows[i][0].ToString()), 1).AddMonths(1));
                }
                else
                {
                    dtSource.Rows[i][0] = String.Format("на 01.01.{0}", year + 1);
                }
            }

            dtSource.Columns.RemoveAt(2);           
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

        private static Color GetColor(string value)
        {
            return value.Contains("-") ? Color.Red : Color.ForestGreen;                    
        }

        private static Color GetStopColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.FromArgb(9, 135, 214);
                    }
                case 2:
                    {
                        return Color.FromArgb(138, 71, 10);
                    }
                case 3:
                    {
                        return Color.FromArgb(65, 124, 9);
                    }
                case 4:
                    {
                        return Color.FromArgb(44, 20, 91);
                    }
                case 5:
                    {
                        return Color.FromArgb(229, 140, 13);
                    }
                case 6:
                    {
                        return Color.FromArgb(11, 45, 160);
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
