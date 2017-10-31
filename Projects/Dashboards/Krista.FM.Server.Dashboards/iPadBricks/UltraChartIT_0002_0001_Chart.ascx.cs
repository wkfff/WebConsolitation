using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Xml.Serialization;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class IT_0002_0001_Chart : UserControl
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

        protected void Page_Load(object sender, EventArgs e)
        {
            SetColumnChartAppearance();
            DataTable dtIncomes = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);
            UltraChart1.DataSource = dtIncomes;
            UltraChart1.DataBind();
        }

        private void SetColumnChartAppearance()
        {
            UltraChart1.Width = 780;
            UltraChart1.Height = 160;
            UltraChart1.ChartType = ChartType.ColumnChart;

            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL><br /><SERIES_LABEL><br /><b><DATA_VALUE:N0></b>&nbsp;тыс.руб.</span>";

            UltraChart1.Data.ZeroAligned = true;
            UltraChart1.Legend.SpanPercentage = 16;

            UltraChart1.Legend.Location = LegendLocation.Right;
            UltraChart1.Legend.Font = new Font("Verdana", 12);
            UltraChart1.Legend.FontColor = Color.White;

           // UltraChart1.Axis.X.Labels.SeriesLabels.FontSizeBestFit = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 12);
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 12);

            UltraChart1.Axis.X.MajorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart1.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            UltraChart1.Axis.X.MinorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MinorGridLines.Color = Color.Black;
            
            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Extent = 15;
            UltraChart1.Axis.Y.Extent = 90;
            UltraChart1.Style.Add("margin-left", "-5px");
            SetupTitleLeft();
            //SetupCustomSkin();
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomLinear;
        }

        private void SetupTitleLeft()
        {
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "тыс.руб.";
            UltraChart1.TitleLeft.Font = new Font("Verdana", 12);
            UltraChart1.TitleLeft.FontColor = Color.White;
            UltraChart1.TitleLeft.VerticalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = 50;
        }

        private void SetupCustomSkin()
        {
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
                        return Color.FromArgb(74, 3, 125);
                    }
                case 2:
                    {
                        return Color.FromArgb(145, 10, 149);
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