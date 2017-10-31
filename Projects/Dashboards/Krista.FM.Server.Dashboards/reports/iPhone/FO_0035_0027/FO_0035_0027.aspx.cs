using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0027 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();

        // выбранный период
        private CustomParam selectedPeriod;

        private DateTime date;
        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {
            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0027_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);

            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            selectedPeriod.Value = dtDate.Rows[0][1].ToString();

            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "Нет данных";

            OutcomesGrid.DataBind();

            lbDescription.Text = String.Format("Зарегистрированные бюджетные обязательства и остатки по лимитам бюджетных обязательств областного бюджета Новосибирской области по состоянию на&nbsp;<b><span class='DigitsValue'>{0:dd.MM.yyyy}</span></b>, тыс.руб.", date);

            DataTable dtChart = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0027_chart");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            UltraChart1.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

            SetStackChartAppearanceUnic();
            UltraChart1.Width = 750;
            UltraChart1.Height = 1000;

            foreach (DataRow row in dtChart.Rows)
            {
                if (!grbsNames.ContainsKey(row[1].ToString()))
                {
                    grbsNames.Add(row[1].ToString(), row[0].ToString());
                }
            }

            dtChart.Columns.RemoveAt(0);

            UltraChart1.Series.Clear();
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(1, dtChart));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(2, dtChart));
            UltraChart1.Data.SwapRowsAndColumns = true;
            // UltraChart1.DataSource = dtChart;
            UltraChart1.DataBind();
            
            //lbGrbsDetalization.Text = String.Format("{0}&nbsp;<a href='webcommand?showReport=FO_0035_0028'><img src='../../../images/detail.png'></a>", lbGrbsDetalization.Text);
            //lbKosguDetalization.Text = String.Format("{0}&nbsp;<a href='webcommand?showPinchReport=FO_0035_0029'><img src='../../../images/detail.png'></a>", lbKosguDetalization.Text);
        }

        private Dictionary<String, String> grbsNames = new Dictionary<string, string>();

        void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.Series != null &&
                        grbsNames.ContainsKey(box.Series.Label))
                    {
                        box.Series.Label = GetWarpedHint(grbsNames[box.Series.Label]);
                    }
                }
            }
        }

        private string GetWarpedHint(string hint)
        {
            string name = hint.Replace("\"", "'");
            if (name.Length > 30)
            {
                int k = 11;

                for (int j = 0; j < name.Length; j++)
                {
                    k++;
                    if (k > 30 && name[j] == ' ')
                    {
                        name = name.Insert(j, "<br/>");
                        k = 0;
                    }
                }
            }
            return name;
        }

        private void SetStackChartAppearanceUnic()
        {
            UltraChart1.ChartType = ChartType.StackBarChart;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart1.Axis.X.Labels.SeriesLabels.OrientationAngle = 90;

            UltraChart1.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL> <br />  <SERIES_LABEL> <br /><b><DATA_VALUE:N0></b>&nbsp;тыс.руб.</span>";
            SetLabelsClipTextBehavior(UltraChart1.Axis.Y.Labels.SeriesLabels.Layout);

            UltraChart1.Axis.X.TickmarkStyle = AxisTickStyle.Percentage;
            UltraChart1.Axis.X.TickmarkInterval = 50;
            //UltraChart1.Axis.X.TickmarkIntervalType = AxisIntervalType.Months;

            UltraChart1.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart1.Axis.X.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart1.Axis.X.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            UltraChart1.Axis.X.MinorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MinorGridLines.Color = Color.Black;

            UltraChart1.Axis.X.Labels.Visible = true;

            UltraChart1.Axis.X.Labels.FontSizeBestFit = false;
            UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 10);      
            UltraChart1.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart1.Axis.Y.Labels.SeriesLabels.FontSizeBestFit = false;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.Y.Labels.Visible = false;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.Y.Labels.SeriesLabels.HorizontalAlign = StringAlignment.Far;

            UltraChart1.Axis.Y.Extent = 180;
            UltraChart1.Axis.X.Extent = 110;

            //UltraChart1.TitleLeft.Visible = true;
            //UltraChart1.TitleLeft.Text = "тыс.руб.";
            //UltraChart1.TitleLeft.Font = new Font("Verdana", 10);
            //UltraChart1.TitleLeft.FontColor = Color.White;
            //UltraChart1.TitleLeft.VerticalAlign = StringAlignment.Center;
            //UltraChart1.TitleLeft.Margins.Bottom = 350;
            UltraChart1.Legend.Font = new Font("Verdana", 10);
            UltraChart1.Legend.SpanPercentage = 8;

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetColor(i);

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }


            UltraChart1.Axis.X.NumericAxisType = NumericAxisType.Logarithmic;
            UltraChart1.Axis.X.LogBase = 3;
            UltraChart1.Axis.X.LogZero = 0.5;
            UltraChart1.Axis.Y.LineColor = Color.Transparent;

        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.Green;
                    }
                case 2:
                    {
                        return Color.Red;
                    }
                default:
                    {
                        return Color.White;
                    }
            }
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

        #region Обработчики грида

        protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0035_0027_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            DataTable dtSource = new DataTable();
            dtSource.Columns.Add(new DataColumn());
            dtSource.Columns.Add(new DataColumn());

            for (int i = 0; i < 6; i++)
            {
                dtSource.Rows.Add(dtSource.NewRow());
                dtSource.Rows[i][0] = dtGrid.Rows[i][0];
            }

            dtSource.Rows[0][1] = String.Format("{0:N2}", dtGrid.Rows[0][1]);
            dtSource.Rows[1][1] = String.Format("{0:N2}", dtGrid.Rows[1][1]);
            dtSource.Rows[2][1] = String.Format("{0:N2}", dtGrid.Rows[2][1]);
            dtSource.Rows[3][1] = String.Format("{0:N2}<br/>{1:P2}", dtGrid.Rows[3][1], dtGrid.Rows[4][1]);
            dtSource.Rows[5][1] = String.Format("{0:N2}", dtGrid.Rows[5][1]);

            dtSource.Rows.RemoveAt(4);

            OutcomesGrid.DataSource = dtSource;
        }

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            e.Layout.Bands[0].Columns[0].Width = 500;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 260;
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;

            headerLayout = new GridHeaderLayout(e.Layout.Grid);

            headerLayout.AddCell("Показатель");
            headerLayout.AddCell("Сумма");

            headerLayout.ApplyHeaderInfo();
        }

        protected void OutcomesGrid_InitializeRow(object sender, RowEventArgs e)
        {
            //e.Row.Style.Height = 60;

            //iPadBricks.iPadBricks.iPadBricksHelper.SetConditionCorner(e, 5, 0, true);

            //if (e.Row.Cells[1].Value.ToString() == "Всего")
            //{
            //    foreach (UltraGridCell cell in  e.Row.Cells)
            //    {
            //        cell.Style.Font.Bold = true;
            //    }
            //    e.Row.Cells[5].Style.Font.Size = 14;
            //}
            //e.Row.Cells[5].Style.Padding.Right = 6;
        }

        #endregion
    }
}
