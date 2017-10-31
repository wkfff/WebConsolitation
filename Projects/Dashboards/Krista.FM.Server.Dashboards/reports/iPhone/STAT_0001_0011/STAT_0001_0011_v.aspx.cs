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
    public partial class STAT_0001_0011_v : CustomReportPage
    {

        private CustomParam periodCurrentDate;
        private CustomParam periodLastWeekDate;
        private CustomParam periodPrevLastWeekDate;

        private CustomParam debtsPeriodCurrentDate;
        private CustomParam debtsPeriodLastWeekDate;
        private CustomParam debtsPeriodPrevLastWeekDate;

        private CustomParam redundantLevelRFDate;

        private string currentRegion;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            //CustomParams.MakeMoParams("306", "id");

            UserParams.Mo.Value = !UserParams.Mo.Value.Contains("�.") ?
                        String.Format(".[{0}]", UserParams.Mo.Value) :
                        String.Format(".[{0}]", UserParams.Mo.Value.Replace("�.", "�����").Replace(" - ", "-"));

            currentRegion = UserParams.Mo.Value.Replace("����� ", "").Replace("����� ", String.Empty).Replace("������������� �����", "��").Replace(".[", String.Empty).
                Replace("[", String.Empty).Replace("]", String.Empty).Replace(".DataMember", String.Empty);

            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (debtsPeriodCurrentDate == null)
            {
                debtsPeriodCurrentDate = UserParams.CustomParam("period_current_date_debts");
            }
            if (debtsPeriodLastWeekDate == null)
            {
                debtsPeriodLastWeekDate = UserParams.CustomParam("period_last_week_date_debts");
            }
            if (debtsPeriodPrevLastWeekDate == null)
            {
                debtsPeriodPrevLastWeekDate = UserParams.CustomParam("period_prev_last_week_date_debts");
            }
            if (periodLastWeekDate == null)
            {
                periodLastWeekDate = UserParams.CustomParam("period_last_week_date");
            }
            if (periodPrevLastWeekDate == null)
            {
                periodPrevLastWeekDate = UserParams.CustomParam("period_prev_last_week_date");
            }
            if (redundantLevelRFDate == null)
            {
                redundantLevelRFDate = UserParams.CustomParam("redundant_level_RF_date");
            }

            UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);

            #region ��������� ��������

            UltraChart1.Width = 750;
            UltraChart1.Height = 240;

            UltraChart4.Width = 750;
            UltraChart4.Height = 240;

            UltraChart2.Width = 750;
            UltraChart2.Height = 240;

            #region ��������� ��������� 4

            UltraChart4.ChartType = ChartType.AreaChart;
            UltraChart4.Border.Thickness = 0;

            UltraChart4.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart4.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart4.Axis.X.Extent = 50;
            UltraChart4.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart4.Axis.X.Labels.Visible = true;
            //  UltraChart4.Axis.X.Labels.FontColor = Color.Black;
            UltraChart4.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart4.Axis.Y.Labels.Font = new Font("Verdana", 8);
            //  UltraChart4.Axis.Y.Labels.FontColor = Color.Black;
            UltraChart4.Axis.Y.Extent = 40;

            UltraChart4.TitleLeft.Visible = true;
            UltraChart4.TitleLeft.Text = "���.���.";
            UltraChart4.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart4.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart4.TitleLeft.Extent = 40;
            UltraChart4.TitleLeft.Margins.Top = 0;
            UltraChart4.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart4.TitleLeft.FontColor = Color.White;

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

            UltraChart4.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart4.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart4_FillSceneGraph);

            #endregion

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

            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            UltraChart2.DataBinding += new EventHandler(UltraChart2_DataBinding);
            UltraChart4.DataBinding += new EventHandler(UltraChart4_DataBinding);

            #endregion

            UltraWebGrid.DataBind();
            UltraWebGrid1.DataBind();
            UltraChart1.DataBind();
            UltraChart2.DataBind();
            UltraChart4.DataBind();
        }

        #region ����

        private GridHeaderLayout headerLayout;
        private GridHeaderLayout headerLayout1;

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
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

            headerLayout1 = new GridHeaderLayout(e.Layout.Bands[0].Grid);

            headerLayout1.AddCell("");
            headerLayout1.AddCell(String.Format("{0:dd.MM}", currDateTimeSakhalin));
            headerLayout1.AddCell(String.Format("{0:dd.MM}", lastDateTimeSakhalin));

            headerLayout1.AddCell(String.Format("{0:dd.MM}", currDateTimeSakhalin));
            headerLayout1.AddCell(String.Format("{0:dd.MM}", lastDateTimeSakhalin));

            headerLayout1.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = 2;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthLeft = 2;

            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = 2;
            e.Layout.Bands[0].Columns[3].CellStyle.BorderDetails.WidthLeft = 2;
        }

        void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0011_grid_date");
            DataTable dtDateSakhalin = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����������", dtDateSakhalin);

            currDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[2][1].ToString(), 3);
            lastDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[1][1].ToString(), 3);

            periodCurrentDate.Value = dtDateSakhalin.Rows[2][1].ToString();
            periodLastWeekDate.Value = dtDateSakhalin.Rows[1][1].ToString();
            periodPrevLastWeekDate.Value = dtDateSakhalin.Rows[0][1].ToString();

            DataTable dtGrid = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0011_grid_v");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtGrid);

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
            cell.AddCell(String.Format("{0:dd.MM}", currDateTimeSakhalin));
            cell.AddCell(String.Format("{0:dd.MM}", lastDateTimeSakhalin));

            cell = headerLayout.AddCell("����������� �������");
            cell.AddCell(String.Format("{0:dd.MM}", currDateTimeSakhalin));
            cell.AddCell(String.Format("{0:dd.MM}", lastDateTimeSakhalin));

            headerLayout.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = 2;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthLeft = 2;

            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = 2;
            e.Layout.Bands[0].Columns[3].CellStyle.BorderDetails.WidthLeft = 2;
        }

        private DateTime currDateTimeSakhalin;
        private DateTime lastDateTimeSakhalin;

        void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0011_chart4");
            DataTable dtDateSakhalin = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����������", dtDateSakhalin);

            currDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[dtDateSakhalin.Rows.Count - 1][1].ToString(), 3).AddMonths(1);
            lastDateTimeSakhalin = CRHelper.DateByPeriodMemberUName(dtDateSakhalin.Rows[dtDateSakhalin.Rows.Count - 2][1].ToString(), 3).AddMonths(1);

            debtsPeriodCurrentDate.Value = dtDateSakhalin.Rows[dtDateSakhalin.Rows.Count - 1][1].ToString();
            debtsPeriodLastWeekDate.Value = dtDateSakhalin.Rows[dtDateSakhalin.Rows.Count - 2][1].ToString();
            debtsPeriodPrevLastWeekDate.Value = dtDateSakhalin.Rows[dtDateSakhalin.Rows.Count - 3][1].ToString();

            DataTable dtGrid = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0011_grid1_v");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtGrid);

            DataTable dtSource = new DataTable();
            for (int i = 0; i < 5; i++)
            {
                dtSource.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
            }

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                DataRow row = dtSource.NewRow();

                row[0] = dtGrid.Rows[i][0];

                AddColumnGroup1(dtGrid, i, row, 1, 1);
                AddColumnGroup1(dtGrid, i, row, 4, 2);
                AddColumnGroup1(dtGrid, i, row, 7, 3);
                AddColumnGroup1(dtGrid, i, row, 10, 4);

                dtSource.Rows.Add(row);
            }

            UltraWebGrid1.DataSource = dtSource;
        }

        private void AddColumnGroup(DataTable dtGrid, int i, DataRow row, int group, int column)
        {
            if (dtGrid.Rows[i][group] != DBNull.Value)
            {
                double value = Convert.ToDouble(dtGrid.Rows[i][group].ToString());
                string absoluteGrown = "-";
                double absoluteGrownValue = 0;
                if (dtGrid.Rows[i][group + 1] != DBNull.Value)
                {
                    absoluteGrownValue = Convert.ToDouble(dtGrid.Rows[i][group + 1].ToString());
                    if (absoluteGrownValue > 0)
                    {
                        switch (i)
                        {
                            case 0:
                                absoluteGrown = String.Format("+{0:P2}", absoluteGrownValue);
                                break;
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                absoluteGrown = String.Format("+{0:N0}", absoluteGrownValue);
                                break;
                            case 5:
                                absoluteGrown = String.Format("+{0:N1}", absoluteGrownValue);
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
                                absoluteGrown = String.Format("-{0:P2}", Math.Abs(absoluteGrownValue));
                                break;
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                absoluteGrown = String.Format("-{0:N0}", Math.Abs(absoluteGrownValue));
                                break;
                            case 5:
                                absoluteGrown = String.Format("-{0:N1}", Math.Abs(absoluteGrownValue));
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
                                absoluteGrown = String.Format("{0:P2}", Math.Abs(absoluteGrownValue));
                                break;
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                absoluteGrown = String.Format("{0:N0}", Math.Abs(absoluteGrownValue));
                                break;
                            case 5:
                                absoluteGrown = String.Format("{0:N1}", Math.Abs(absoluteGrownValue));
                                break;
                            default:
                                absoluteGrown = String.Format("{0:N3}", Math.Abs(absoluteGrownValue));
                                break;
                        }
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
                    if (i == 3 || i == 4)
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
                        row[column] = String.Format("{0:P2}<br/>{1}&nbsp;{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        row[column] = String.Format("{0:N0}<br/>{1}&nbsp;{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                    case 5:
                        row[column] = String.Format("{0:N1}<br/>{1}&nbsp;{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                    default:
                        row[column] = String.Format("{0:N3}<br/>{1}&nbsp;{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                }
            }
        }

        private void AddColumnGroup1(DataTable dtGrid, int i, DataRow row, int group, int column)
        {
            if (dtGrid.Rows[i][group] != DBNull.Value)
            {
                double value = Convert.ToDouble(dtGrid.Rows[i][group].ToString());
                string absoluteGrown = "0";
                double absoluteGrownValue = 0;
                if (dtGrid.Rows[i][group + 1] != DBNull.Value)
                {
                    absoluteGrownValue = Convert.ToDouble(dtGrid.Rows[i][group + 1].ToString());
                    if (absoluteGrownValue > 0)
                    {
                        switch (i)
                        {
                            case 0:
                                absoluteGrown = String.Format("+{0:N3}", absoluteGrownValue);
                                break;
                            case 1:
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
                                absoluteGrown = String.Format("-{0:N3}", Math.Abs(absoluteGrownValue));
                                break;
                            case 1:
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
                                absoluteGrown = String.Format("{0:N3}", Math.Abs(absoluteGrownValue));
                                break;
                            case 1:
                                absoluteGrown = String.Format("{0:N0}", Math.Abs(absoluteGrownValue));
                                break;
                            default:
                                absoluteGrown = String.Format("{0:N3}", Math.Abs(absoluteGrownValue));
                                break;
                        }
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
                    if (i == 3 || i == 4)
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
                        row[column] = String.Format("{0:N3}<br/>{1}&nbsp;{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                    case 1:
                        row[column] = String.Format("{0:N0}<br/>{1}&nbsp;{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                    default:
                        row[column] = String.Format("{0:N3}<br/>{1}&nbsp;{2}<br/>{3}", value, img, grown, absoluteGrown);
                        break;
                }
            }
        }

        #endregion

        #region ���������

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
            chart.TitleLeft.Text = "���.���.";
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
            string query = DataProvider.GetQueryText("STAT_0001_0011_chart2");
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString());
                        row[0] = String.Format("{0:dd.MM.yy}", dateTime);
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
            string query = DataProvider.GetQueryText("STAT_0001_0011_v_chart1");
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString());
                        row[0] = String.Format("{0:dd.MM.yy}", dateTime);
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

            string query = DataProvider.GetQueryText("STAT_0001_0011_chart4");
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                if (dtChart.Rows.Count > 1)
                {
                    DateTime startDate = CRHelper.PeriodDayFoDate(dtChart.Rows[0][0].ToString());
                    DateTime endDate = CRHelper.PeriodDayFoDate(dtChart.Rows[dtChart.Rows.Count - 1][0].ToString());
                    lbDebts.Text = String.Format("�� ���� ������ ���������� �&nbsp;<span class='DigitsValue'><b>{0:dd.MM.yyyy}</b></span>&nbsp;��&nbsp;<span class='DigitsValue'><b>{1:dd.MM.yyyy}</b></span>&nbsp;����������� ������������� �� ������� ���������� �����", startDate, endDate);
                }
                else
                {
                    lbDebts.Text = "�� ���� ������ ���������� ����������� ������������� �� ������� ���������� �����";
                }

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString()).AddMonths(1);
                        row[0] = String.Format("{0:dd.MM.yy}", dateTime);
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

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
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
                            point.DataPoint.Label = String.Format("<span style='font-family: Arial; font-size: 14pt'>{1}\n��&nbsp;<b>{2}</b>\n<b>{0:P2}</b><span>", ((NumericDataPoint)point.DataPoint).Value, point.Series.Label, point.DataPoint.Label);
                        }
                    }
                }
            }
        }

        void UltraChart4_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
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
                            point.DataPoint.Label = String.Format("<span style='font-family: Arial; font-size: 14pt'>{1}\n��&nbsp;<b>{2}</b>\n<b>{0:N2}</b>&nbsp;���.���.<span>", ((NumericDataPoint)point.DataPoint).Value, point.Series.Label, point.DataPoint.Label);
                        }
                    }
                }
            }
        }

        void UltraChart2_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
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
                            point.DataPoint.Label = String.Format("<span style='font-family: Arial; font-size: 14pt'>{1}\n��&nbsp;<b>{2}</b>\n<b>{0:N2}</b><span>", ((NumericDataPoint)point.DataPoint).Value, point.Series.Label, point.DataPoint.Label);
                        }
                    }
                }
            }
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
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

        #endregion

    }
}
