using System;
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
    public partial class SE_0001_0007_Chart : UserControl
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


        public bool DescriptionVisible
        {
            get
            {
                return lbDescription.Visible;
            }
            set
            {
                lbDescription.Visible = value;
            }
        }

        private int chartHeight = 350;
        public int ChartHeight
        {
            get
            {
                return chartHeight;
            }
            set
            {
                chartHeight = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitializeDate();
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            SetStackChartAppearanceUnic();
            DataTable dtIncomes = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);

            //dtIncomes.Columns.RemoveAt(0);
            foreach (DataRow row in dtIncomes.Rows)
            {
                row[0] = RegionsNamingHelper.ShortName(row[0].ToString());
            }
            dtIncomes.AcceptChanges();
            UltraChart1.Series.Clear();
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(1, dtIncomes));
            UltraChart1.Data.SwapRowsAndColumns = false;
            // UltraChart1.DataSource = dtIncomes;
            UltraChart1.DataBind();

            string period = monthNum == 1 ? "январь" : String.Format("январь-{0}", CRHelper.RusMonth(monthNum));

            lbDescription.Text =
                String.Format(
                    "Объем отгруженных товаров собственного производства, выполненных работ и услуг собственными силами по субъектам&nbsp;<span class='DigitsValue'>{2}</span>&nbsp;за&nbsp;<span class='DigitsValue'>{0} {1}</span>&nbsp;года",
                    period, year,
                    RegionsNamingHelper.ShortName(CustomParam.CustomParamFactory("region").Value));
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
                    }
                }
                if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline)primitive;

                    foreach (DataPoint point in polyline.points)
                    {
                        point.hitTestRadius = 0;
                        point.DataPoint.Label = String.Format("<span style='font-family: Arial; font-size: 14pt'>{1}<br/><b>{0:N1}%</b></span>", point.Value, RegionsNamingHelper.FullName(point.DataPoint.Label));
                       // point.Series.Label = String.Format("<span style='font-family: Arial; font-size: 14pt'><b>{0:N1}%</b></span>", point.Value);
                    }
                }
            }
        }

        private int year = 2010;
        private int monthNum = 8;

        private void InitializeDate()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("SE_0001_0002_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0]);
        }
        
        private void SetStackChartAppearanceUnic()
        {
            UltraChart1.Width = 730;
            UltraChart1.Height = chartHeight;

            UltraChart1.ChartType = ChartType.ParetoChart;
            UltraChart1.ParetoChart.ColumnSpacing = 0;

            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart1.Axis.X.Labels.SeriesLabels.OrientationAngle = 90;
            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL><br /><b><DATA_VALUE:N0></b>&nbsp;млн.руб.</span>";

            UltraChart1.Legend.SpanPercentage = 18;

            //  UltraChart1.Axis.X.Labels.SeriesLabels.FontSizeBestFit = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 12);
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 12);
            UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 12);

            UltraChart1.Axis.X.MajorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart1.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;

            UltraChart1.Axis.X.MinorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MinorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y2.MinorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y2.MajorGridLines.Color = Color.Black;

            UltraChart1.Axis.Y2.Margin.Far.Value = 10;
            UltraChart1.Axis.Y2.Margin.Far.MarginType = LocationType.Pixels;

            UltraChart1.Axis.Y.Margin.Far.Value = 10;
            UltraChart1.Axis.Y.Margin.Far.MarginType = LocationType.Pixels;

            UltraChart1.Axis.Y2.Visible = true;
            UltraChart1.Axis.Y2.Extent = 40;
            UltraChart1.Axis.Y2.Labels.ItemFormatString = "<DATA_VALUE:N0>%";
            UltraChart1.Axis.Y2.Labels.Font = new Font("Verdana", 12);
            UltraChart1.Axis.Y2.Labels.FontColor = Color.White;

            //    UltraChart1.Axis.X.Labels.FontSizeBestFit = false;
            UltraChart1.Axis.X.Labels.Visible = true;

            UltraChart1.Axis.Y.Extent = 80;
            UltraChart1.Axis.X.Extent = 70;

            SetupTitleLeft();
            //SetupCustomSkin();
            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;

            PaintElement pe = new PaintElement();
            Color color = Color.FromArgb(78, 230, 228);
            Color stopColor = Color.FromArgb(168, 48, 137);
            pe.Fill = color;
            pe.FillStopColor = stopColor;
            pe.ElementType = PaintElementType.SolidFill;
            pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
            pe.FillOpacity = 150;
            pe.StrokeWidth = 4;

            UltraChart1.ParetoChart.LinePE = pe;
                        
            // UltraChart1.ParetoChart.

            // UltraChart1.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart1.Legend.Visible = false;

            //UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            //UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);
        }

        private void SetupTitleLeft()
        {
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "млн.руб.";
            UltraChart1.TitleLeft.Font = new Font("Verdana", 12);
            UltraChart1.TitleLeft.FontColor = Color.White;
            UltraChart1.TitleLeft.VerticalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = 150;
        }

        //private void SetupCustomSkin()
        //{
        //    UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
        //    UltraChart1.ColorModel.Skin.ApplyRowWise = false;
        //    UltraChart1.ColorModel.Skin.PEs.Clear();
        //    for (int i = 1; i <= 10; i++)
        //    {
        //        PaintElement pe = new PaintElement();
        //        Color color = GetColor(i);
        //        Color stopColor = GetStopColor(i);

        //        pe.Fill = color;
        //        pe.FillStopColor = stopColor;
        //        pe.ElementType = PaintElementType.Gradient;
        //        pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
        //        pe.FillOpacity = 150;
        //        UltraChart1.ColorModel.Skin.PEs.Add(pe);
        //    }
        //}

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 2:
                    {
                        return Color.FromArgb(74, 3, 125);
                    }
                case 1:
                    {
                        return Color.FromArgb(145, 10, 149);
                    }
                case 3:
                    {
                        return Color.FromArgb(141, 178, 105);
                    }
                case 4:
                    {
                        return Color.FromArgb(192, 178, 224);
                    }
                case 5:
                    {
                        return Color.FromArgb(245, 187, 102);
                    }
                case 6:
                    {
                        return Color.FromArgb(142, 164, 236);
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
                case 2:
                    {
                        return Color.FromArgb(74, 3, 125);
                    }
                case 1:
                case 3:
                    {
                        return Color.FromArgb(145, 10, 149);
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

        void UltraChart_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            Box box = e.Primitive as Box;
            if ((box != null) && !(string.IsNullOrEmpty(box.Path)) &&
                box.Path.EndsWith("Legend") && (box.rect.Width != box.rect.Height))
            {
                //  box.rect.Width = box.rect.Width + 4;
            }
        }
    }
}