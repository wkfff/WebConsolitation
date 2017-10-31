using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class STAT_0001_0006_v : CustomReportPage
    {
        //private DataTable dt;
        private DataTable dtChart;

        private CustomParam periodLastDate;
        private CustomParam periodPrevDate;
        private CustomParam periodPrevPrevDate;

        // Текущая дата для задолженности
        private CustomParam debtsPeriodCurrentDate;
        // На неделю назад для задолженности
        private CustomParam debtsPeriodLastWeekDate;

        private string currentRegion;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.Mo.Value = !UserParams.Mo.Value.Contains("г.") ?
                        UserParams.Mo.Value.Replace(" муниципальный", String.Empty).Replace(" район", " муниципальный район") :
                        UserParams.Mo.Value.Replace("г. ", "Город ");

            currentRegion = UserParams.Mo.Value.Replace("Город ", "").Replace("город ", "").Replace("муниципальный район", "р-н").Replace(".[", "").Replace("]", "").Replace(".DataMember", "");

            periodLastDate = UserParams.CustomParam("period_last_date");
            periodPrevDate = UserParams.CustomParam("period_prev_date");
            periodPrevPrevDate = UserParams.CustomParam("period_prev_prev_date");

            UltraChart1.Width = 750;
            UltraChart1.Height = 240;

            UltraChart4.Width = 750;
            UltraChart4.Height = 240;

            UltraChart2.Width = 750;
            UltraChart2.Height = 240;

            #region Настройка диаграмм 1 и 2

            PaintElement pe = new PaintElement();
            pe.Fill = Color.FromArgb(0x660099);
            pe.FillStopColor = Color.FromArgb(0x990099);
            pe.ElementType = PaintElementType.Gradient;
            pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
            pe.FillOpacity = (byte)150;
            pe.FillStopOpacity = (byte)150;
            UltraChart2.ColorModel.Skin.PEs.Add(pe);
            SetupDynamicChart(UltraChart2, pe, false);

            SetUltraChart1Appearence(UltraChart1);

            UltraChart1.FillSceneGraph += UltraChart1_FillSceneGraph;
            UltraChart2.FillSceneGraph += UltraChart2_FillSceneGraph;

            #endregion

            #region Настройка диаграммы 4

            UltraChart4.ChartType = ChartType.AreaChart;
            UltraChart4.Border.Thickness = 0;

            UltraChart4.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart4.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart4.Axis.X.Extent = 50;
            UltraChart4.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart4.Axis.X.Labels.Visible = true;
            UltraChart4.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart4.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart4.Axis.Y.Extent = 40;

            UltraChart4.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart4.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            UltraChart4.Data.EmptyStyle.Text = " ";
            UltraChart4.EmptyChartText = " ";

            UltraChart4.AreaChart.NullHandling = NullHandling.Zero;

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance.Thickness = 3;
            UltraChart4.AreaChart.LineAppearances.Add(lineAppearance);

            UltraChart4.Legend.Visible = true;
            UltraChart4.Legend.Location = LegendLocation.Top;
            UltraChart4.Legend.SpanPercentage = 14;
            UltraChart4.Legend.Font = new Font("Verdana", 10);

            UltraChart4.InvalidDataReceived +=
                new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(
                    CRHelper.UltraChartInvalidDataReceived);
            UltraChart4.FillSceneGraph +=
                new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart4_FillSceneGraph);

            #endregion

            #region Подписи к осям диаграмм

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "процент";
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.TitleLeft.Extent = 40;
            UltraChart1.TitleLeft.Margins.Top = 0;
            UltraChart1.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart1.TitleLeft.FontColor = Color.White;

            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.Text = "единица";
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.TitleLeft.Extent = 40;
            UltraChart2.TitleLeft.Margins.Top = 0;
            UltraChart2.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart2.TitleLeft.FontColor = Color.White;

            UltraChart4.TitleLeft.Visible = true;
            UltraChart4.TitleLeft.Text = "тыс.руб.";
            UltraChart4.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart4.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart4.TitleLeft.Extent = 40;
            UltraChart4.TitleLeft.Margins.Top = 0;
            UltraChart4.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart4.TitleLeft.FontColor = Color.White;

            #endregion

            UltraChart1.DataBinding += UltraChart1_DataBinding;
            UltraChart1.DataBind();

            UltraChart2.DataBinding += UltraChart2_DataBinding;
            UltraChart2.DataBind();

            UltraChart4.DataBinding += UltraChart4_DataBinding;
            UltraChart4.DataBind();

            UltraWebGrid.DataBinding += UltraWebGrid_DataBinding;
            UltraWebGrid.InitializeLayout += UltraWebGrid_InitializeLayout;
            UltraWebGrid.DataBind();
        }

        #region Грид

        private GridHeaderLayout headerLayout;

        void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Width = Unit.Empty;
            e.Layout.Bands[0].Grid.Height = Unit.Empty;

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].Width = 320;
            e.Layout.Bands[0].Columns[1].Width = 110;
            e.Layout.Bands[0].Columns[2].Width = 110;
            e.Layout.Bands[0].Columns[3].Width = 110;
            e.Layout.Bands[0].Columns[4].Width = 110;

            e.Layout.Bands[0].Columns[2].CellStyle.ForeColor = Color.FromArgb(192, 192, 192);
            e.Layout.Bands[0].Columns[4].CellStyle.ForeColor = Color.FromArgb(192, 192, 192);

            headerLayout = new GridHeaderLayout(e.Layout.Bands[0].Grid);

            headerLayout.AddCell("");
            GridHeaderCell cell = headerLayout.AddCell(currentRegion);
            cell.AddCell(String.Format("{0:dd.MM}", lastDateTime));
            cell.AddCell(String.Format("{0:dd.MM}", prevDateTime));

            cell = headerLayout.AddCell("Новосибирская область");
            cell.AddCell(String.Format("{0:dd.MM}", lastDateTime));
            cell.AddCell(String.Format("{0:dd.MM}", prevDateTime));

            headerLayout.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = 2;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthLeft = 2;

            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = 2;
            e.Layout.Bands[0].Columns[3].CellStyle.BorderDetails.WidthLeft = 2;
        }

        private DateTime lastDateTime;
        private DateTime prevDateTime;

        void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0006_v_date");
            DataTable dtDate = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtDate);

            periodLastDate.Value = dtDate.Rows[0]["Последняя дата"].ToString();
            periodPrevDate.Value = dtDate.Rows[0]["Предшествующая дата"].ToString();
            lastDateTime = CRHelper.DateByPeriodMemberUName(periodLastDate.Value, 3);
            prevDateTime = CRHelper.DateByPeriodMemberUName(periodPrevDate.Value, 3);

            DataTable dtGrid = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0006_v_grid");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtGrid);

            DataTable dtSource = new DataTable();
            for (int i = 0; i < 5; i++)
            {
                dtSource.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
            }

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                DataRow row = dtSource.NewRow();

                row[0] = dtGrid.Rows[i][0];

                AddColumnGroup(dtGrid, i, row, 1, 1);
                AddColumnGroup(dtGrid, i, row, 4, 2);
                AddColumnGroup(dtGrid, i, row, 7, 3);
                AddColumnGroup(dtGrid, i, row, 10, 4);

                dtSource.Rows.Add(row);
            }

            UltraWebGrid.DataSource = dtSource;
        }

        private void AddColumnGroup(DataTable dtGrid, int i, DataRow row, int group, int column)
        {
            if (dtGrid.Rows[i][group] != DBNull.Value)
            {
                double value = Convert.ToDouble(dtGrid.Rows[i][group].ToString());
                double absoluteGrownValue = Convert.ToDouble(dtGrid.Rows[i][group + 1].ToString());
                string absoluteGrown;
                if (absoluteGrownValue > 0)
                {
                    switch (i)
                    {
                        case 0:
                            absoluteGrown = String.Format("+{0:N2}%", absoluteGrownValue);
                            break;
                        case 1:
                        case 2:
                        case 3:
                        case 6:
                            absoluteGrown = String.Format("+{0:N0}", absoluteGrownValue);
                            break;
                        default:
                            absoluteGrown = String.Format("+{0:N3}", absoluteGrownValue);
                            break;
                    }
                }
                else if (absoluteGrownValue < 0)
                {
                    switch (i)
                    {
                        case 0:
                            absoluteGrown = String.Format("-{0:N2}%", Math.Abs(absoluteGrownValue));
                            break;
                        case 1:
                        case 2:
                        case 3:
                        case 6:
                            absoluteGrown = String.Format("-{0:N0}", Math.Abs(absoluteGrownValue));
                            break;
                        default:
                            absoluteGrown = String.Format("-{0:N3}", Math.Abs(absoluteGrownValue));
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            absoluteGrown = String.Format("{0:N2}%", Math.Abs(absoluteGrownValue));
                            break;
                        case 1:
                        case 2:
                        case 3:
                        case 6:
                            absoluteGrown = String.Format("{0:N0}", Math.Abs(absoluteGrownValue));
                            break;
                        default:
                            absoluteGrown = String.Format("{0:N3}", Math.Abs(absoluteGrownValue));
                            break;
                    }
                }


                double grownValue = 0;
                if (dtGrid.Rows[i][group + 2] != DBNull.Value &&
                    dtGrid.Rows[i][group + 2].ToString() != String.Empty)
                {
                    grownValue = Convert.ToDouble(dtGrid.Rows[i][group + 2].ToString());
                }

                string grown;
                if (grownValue == 0)
                {
                    grown = String.Format("{0:P2}", grownValue);
                }
                else
                {
                    grown = grownValue > 0
                                   ? String.Format("+{0:P2}", grownValue)
                                   : String.Format("-{0:P2}", Math.Abs(grownValue));
                }

                string img = String.Empty;
                if (absoluteGrownValue != 0)
                {
                    if (i == 3)
                    {
                        img = absoluteGrownValue > 0
                                  ? "<img src='../../../images/arrowGreenUpBB.png'>"
                                  : "<img src='../../../images/arrowRedDownBB.png'>";
                    }
                    else
                    {
                        img = absoluteGrownValue > 0
                                  ? "<img src='../../../images/arrowRedUpBB.png'>"
                                  : "<img src='../../../images/arrowGreenDownBB.png'>";
                    }
                }

                switch (i)
                {
                    case 0:
                        row[column] = String.Format("{0:N2}%<br/>{1}&nbsp;{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 6:
                        row[column] = String.Format("{0:N0}<br/>{1}&nbsp;{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                    default:
                        row[column] = String.Format("{0:N3}<br/>{1}&nbsp;{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                }
            }
        }

        #endregion

        private void SetupDynamicChart(UltraChart chart, PaintElement pe, bool titleLeftVisible)
        {
            chart.ChartType = ChartType.AreaChart;
            chart.Border.Thickness = 0;

            chart.Tooltips.FormatString = "<ITEM_LABEL>";
            chart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            chart.Axis.X.Extent = 50;
            chart.Axis.X.Labels.Font = new Font("Verdana", 8);
            chart.Axis.X.Labels.Visible = true;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            chart.Axis.Y.Labels.Font = new Font("Verdana", 8);
            chart.Axis.Y.Extent = 40;

            chart.TitleLeft.Visible = titleLeftVisible;
            chart.TitleLeft.Text = "тыс.руб.";
            chart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            chart.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            chart.TitleLeft.Extent = 40;
            chart.TitleLeft.Margins.Top = 0;
            chart.TitleLeft.Font = new Font("Verdana", 10);
            chart.TitleLeft.FontColor = Color.White;

            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.FillSceneGraph += UltraChart_FillSceneGraph;

            chart.Data.EmptyStyle.Text = " ";
            chart.EmptyChartText = " ";

            chart.AreaChart.NullHandling = NullHandling.Zero;

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance.IconAppearance.PE = pe;
            lineAppearance.Thickness = 3;
            chart.AreaChart.LineAppearances.Add(lineAppearance);

            chart.Legend.Visible = true;
            chart.Legend.Location = LegendLocation.Top;
            chart.Legend.SpanPercentage = 14;
            chart.Legend.Font = new Font("Verdana", 10);
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0006_v_chart2");
            dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString());
                        row[0] = string.Format("{0:dd.MM.yy}", dateTime);
                    }
                }

                UltraChart2.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart2.Series.Add(series);
                }
            }
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0006_v_chart1");
            dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString());
                        row[0] = string.Format("{0:dd.MM.yy}", dateTime);
                    }
                }

                UltraChart1.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart1.Series.Add(series);
                }
            }
        }

        private void SetUltraChart1Appearence(UltraChart chart)
        {
            chart.FillSceneGraph += UltraChart_FillSceneGraph;

            chart.ChartType = ChartType.AreaChart;
            chart.Border.Thickness = 0;

            chart.Tooltips.FormatString = "<ITEM_LABEL>";
            chart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            chart.Axis.X.Extent = 50;
            chart.Axis.X.Labels.Font = new Font("Verdana", 8);
            chart.Axis.X.Labels.Visible = true;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P1>";
            chart.Axis.Y.Labels.Font = new Font("Verdana", 8);
            chart.Axis.Y.Extent = 40;
            
            chart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            chart.FillSceneGraph += UltraChart_FillSceneGraph;

            chart.Data.EmptyStyle.Text = " ";
            chart.EmptyChartText = " ";

            chart.AreaChart.NullHandling = NullHandling.Zero;

            chart.AreaChart.LineAppearances.Clear();

            chart.Legend.Visible = true;
            chart.Legend.Location = LegendLocation.Top;
            //  UltraChart4.Legend.Margins.Right = IsSmallResolution ? 5 : Convert.ToInt32(UltraChart4.Width.Value) / 2;
            chart.Legend.SpanPercentage = 14;
            chart.Legend.Font = new Font("Verdana", 10);

            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 3; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                PaintElementType peType = PaintElementType.Gradient;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Transparent;
                            stopColor = Color.Gray;
                            peType = PaintElementType.Hatch;
                            pe.Hatch = FillHatchStyle.ForwardDiagonal;
                            break;
                        }
                    case 2:
                        {
                            color = Color.MediumSeaGreen;
                            stopColor = Color.Green;
                            peType = PaintElementType.Gradient;
                            break;
                        }
                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = peType;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = (byte)100;
                pe.FillStopOpacity = (byte)100;
                chart.ColorModel.Skin.PEs.Add(pe);

                LineAppearance lineAppearance = new LineAppearance();
                lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
                lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
                lineAppearance.Thickness = 5;
                lineAppearance.IconAppearance.PE = pe;
                chart.AreaChart.LineAppearances.Add(lineAppearance);
            }
        }

        protected void UltraChart4_DataBinding(object sender, EventArgs e)
        {
            bool chartVisible = false;

            string query = DataProvider.GetQueryText("STAT_0001_0006_v_chart4");
            dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Дата", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                if (dtChart.Rows.Count > 1)
                {
                    DateTime startDate = CRHelper.PeriodDayFoDate(dtChart.Rows[0][0].ToString());
                    DateTime endDate = CRHelper.PeriodDayFoDate(dtChart.Rows[dtChart.Rows.Count - 1][0].ToString());
                    lbDebts.Text = String.Format("За весь период наблюдения с&nbsp;<span class='DigitsValue'><b>{0:dd.MM.yyyy}</b></span>&nbsp;по&nbsp;<span class='DigitsValue'><b>{1:dd.MM.yyyy}</b></span>&nbsp;отсутствует задолженность по выплате заработной платы", startDate, endDate);
                }
                else
                {
                    lbDebts.Text = "За весь период наблюдения отсутствует задолженность по выплате заработной платы";
                }

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString());
                        row[0] = string.Format("{0:dd.MM.yy}", dateTime);
                    }
                     if (row[1] != DBNull.Value)
                     {
                         double value = Convert.ToDouble(row[1].ToString());
                         if (value != 0)
                         {
                             chartVisible = true;
                         }
                     }
                }

                UltraChart4.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart4.Series.Add(series);
                }
            }
            UltraChart4.Visible = chartVisible;
            lbDebts.Visible = !chartVisible;
        }

        protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline)primitive;
                    foreach (DataPoint point in polyline.points)
                    {
                        if (point.Series != null)
                        {
                            point.DataPoint.Label = string.Format("<span style='font-family: Arial; font-size: 14pt'>{1}\nна&nbsp;<b>{2}</b>\n<b>{0:P2}</b><span>", ((NumericDataPoint)point.DataPoint).Value, point.Series.Label, point.DataPoint.Label);
                        }
                    }
                }
            }
        }

        protected void UltraChart4_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline)primitive;
                    foreach (DataPoint point in polyline.points)
                    {
                        if (point.Series != null)
                        {
                            point.DataPoint.Label = string.Format("<span style='font-family: Arial; font-size: 14pt'>{1}\nна&nbsp;<b>{2}</b>\n<b>{0:N2}</b>&nbsp;тыс.руб.<span>", ((NumericDataPoint)point.DataPoint).Value, point.Series.Label, point.DataPoint.Label);
                        }
                    }
                }
            }
        }

        protected void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline)primitive;
                    foreach (DataPoint point in polyline.points)
                    {
                        if (point.Series != null)
                        {
                            point.DataPoint.Label = string.Format("<span style='font-family: Arial; font-size: 14pt'>{1}\nна&nbsp;<b>{2}</b>\n<b>{0:N2}</b><span>", ((NumericDataPoint)point.DataPoint).Value, point.Series.Label, point.DataPoint.Label);
                        }
                    }
                }
            }
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is PointSet)
                {
                    PointSet pointSet = (PointSet)primitive;
                    foreach (DataPoint point in pointSet.points)
                    {
                        point.hitTestRadius = 15;
                    }
                }
            }
        }

    }
}

