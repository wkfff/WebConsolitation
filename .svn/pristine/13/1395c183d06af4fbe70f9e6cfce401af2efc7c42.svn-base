using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = System.Web.UI.WebControls.Image;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.UltraChart.Core.Primitives;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0003_0008_Horizontal : CustomReportPage
    {
        DataTable dt1 = new DataTable();
        DataTable dt2 = new DataTable();

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            CustomParams.MakeRegionParams("65", "id");

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0003_0008_Horizontal_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "дата", dtDate);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[0][1].ToString();
            DateTime date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            SetChartAppearance(UltraChart1, 360);
            UltraChart1.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            SetupCustomSkin1();
            DataTable dtIncomes = new DataTable();
            query = DataProvider.GetQueryText("FO_0003_0008_Horizontal_chart");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);

            dtIncomes.Columns[1].ColumnName = String.Format("План {0}", date.Year);
            dtIncomes.Columns[2].ColumnName = String.Format("Уточненный план {0}\nна {1:dd.MM.yyyy}", date.Year, date.AddMonths(1));
            dtIncomes.Columns[3].ColumnName = String.Format("на {0:dd.MM.yyyy}", date.AddMonths(1));
            dtIncomes.Columns[4].ColumnName = String.Format("{0} год", date.AddYears(-1).Year);
            dtIncomes.Columns[5].ColumnName = String.Format("{0} год", date.AddYears(-2).Year);

            dt1 = new DataTable();
            query = DataProvider.GetQueryText("FO_0003_0008_Horizontal_chart_summ");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt1);

            UltraChart1.Series.Clear();

            UltraChart1.Series.Add(CRHelper.GetNumericSeries(1, dtIncomes));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(2, dtIncomes));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(3, dtIncomes));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(4, dtIncomes));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(5, dtIncomes));
            UltraChart1.DataBind();

            SetChartAppearance(UltraChart2, 220);
            SetupCustomSkin2();
            UltraChart2.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);
            dtIncomes = new DataTable();
            query = DataProvider.GetQueryText("FO_0003_0008_Horizontal_chart2");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);

            dtIncomes.Columns[1].ColumnName = String.Format("на {0:dd.MM.yyyy}", date.AddMonths(1));
            dtIncomes.Columns[2].ColumnName = String.Format("{0} год", dtIncomes.Columns[2].ColumnName);
            dtIncomes.Columns[3].ColumnName = String.Format("{0} год", dtIncomes.Columns[3].ColumnName);

            dt2 = new DataTable();
            query = DataProvider.GetQueryText("FO_0003_0008_Horizontal_chart2_summ");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt2);

            UltraChart2.Series.Clear();

            UltraChart2.Series.Add(CRHelper.GetNumericSeries(1, dtIncomes));
            UltraChart2.Series.Add(CRHelper.GetNumericSeries(2, dtIncomes));
            UltraChart2.Series.Add(CRHelper.GetNumericSeries(3, dtIncomes));
            UltraChart2.DataBind();
            lbDescription.Text = "Соотношение собственных доходов и межбюджетных трансфертов из федерального бюджета (за исключением субвенций)";
            Label1.Text = "Поступление налогов, сборов, иных обязательных платежей в доходы федерального бюджета и консолидированного бюджета субъекта РФ";
        }

        void UltraChart2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        box.DataPoint.Label = String.Format("{0} {2}<br/>Сумма&nbsp;<b>{1:N0}</b>&nbsp;тыс.руб.", box.DataPoint.Label, dt2.Rows[box.Column][box.Row + 1], box.Series.Label);
                    }
                }
            }
        }

        void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        box.DataPoint.Label = String.Format("{0} {2}<br/>Сумма&nbsp;<b>{1:N0}</b>&nbsp;тыс.руб.", box.DataPoint.Label, dt1.Rows[box.Column][box.Row + 1], box.Series.Label);
                    }
                }
            }
        }

        private void SetChartAppearance(UltraChart chart, int height)
        {
            chart.Width = 760;
            chart.Height = height;
            chart.ChartType = ChartType.StackBarChart;
            chart.StackChart.StackStyle = StackStyle.Complete;

            //chart.Data.SwapRowsAndColumns = true;
            chart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";            
            chart.Axis.X.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            chart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            chart.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL><br />Доля&nbsp;<b><DATA_VALUE_ITEM:N0>%</b></span>";

            chart.Data.ZeroAligned = true;
            chart.Legend.SpanPercentage = 15;
            //chart.Legend.Margins.Bottom = 80;

            chart.Legend.Location = LegendLocation.Bottom;
            chart.Legend.Font = new Font("Verdana", 10);
            chart.Legend.FontColor = Color.Black;
            chart.BackColor = Color.FromArgb(0xa8a8a8);

            // chart.Axis.X.Labels.SeriesLabels.FontSizeBestFit = false;
            chart.Axis.Y.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            chart.Axis.Y.Labels.HorizontalAlign = StringAlignment.Far;
            chart.Axis.Y.Labels.VerticalAlign = StringAlignment.Far;

            chart.Axis.X.MajorGridLines.Color = Color.Black;
            chart.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            chart.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            chart.Axis.X.MinorGridLines.Color = Color.Black;
            chart.Axis.Y.MinorGridLines.Color = Color.Black;

            chart.Axis.Y.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            chart.Axis.X.Labels.Orientation = TextOrientation.Horizontal;
            chart.Axis.Y.Labels.SeriesLabels.Visible = true;
            chart.Axis.Y.Labels.SeriesLabels.FontColor = Color.Black;
            chart.Axis.X.Labels.FontColor = Color.Black;
            chart.Axis.Y.Extent = 150;

            
            chart.Axis.X.Labels.Visible = true;
            chart.Border.Color = Color.FromArgb(0xa8a8a8);

            chart.Style.Add("margin-left", "-5px");
        }

        private void SetupCustomSkin1()
        {
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
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
        }

        private void SetupCustomSkin2()
        {
            UltraChart2.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart2.ColorModel.Skin.ApplyRowWise = false;
            UltraChart2.ColorModel.Skin.PEs.Clear();
            for (int i = 3; i <= 4; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetStopColor(i);

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart2.ColorModel.Skin.PEs.Add(pe);
            }
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.FromArgb(74, 3, 125);
                    }
                case 2:
                    {
                        return Color.FromArgb(145, 10, 149);
                    }
                case 3:
                    {
                        return Color.FromArgb(143, 199, 219);
                    }
                case 4:
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
                        return Color.FromArgb(74, 3, 125);
                    }
                case 2:
                    {
                        return Color.FromArgb(145, 10, 149);
                    }
                case 3:
                    {
                        return Color.FromArgb(11, 100, 131);
                    }
                case 4:
                    {
                        return Color.FromArgb(102, 168, 9);
                    }
            }
            return Color.White;
        }
    }

}
