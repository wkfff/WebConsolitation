using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Xml.Serialization;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class IT_0002_0004_Chart : UserControl
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
            SetStackChartAppearanceUnic();
            DataTable dtIncomes = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            dtIncomes.Columns.RemoveAt(0);
            foreach (DataColumn col in dtIncomes.Columns)
            {
                col.ColumnName = col.ColumnName.Split(';')[0].Replace('"', '\'');
            }
            
            UltraChart1.DataSource = dtIncomes;
            UltraChart1.DataBind();
        }

        private string axisMonth = String.Empty;

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            SceneGraph grahp = e.SceneGraph;

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
                        int year;
                        if (Int32.TryParse(textArray[1], out year) && CRHelper.IsMonthCaption(textArray[0]))
                        {
                            if (axisMonth == textArray[1])
                            {
                                ((Text)primitive).SetTextString(textArray[0]);
                            }
                            else
                            {
                                ((Text)primitive).SetTextString(String.Format("{0}-{1}",
                                              year ,CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(CRHelper.MonthNum(textArray[1])))));
                                axisMonth = textArray[1];
                            }
                        }
                    }
                }
            }
        }

        private void SetStackChartAppearanceUnic()
        {
            UltraChart1.Width = 750;
            UltraChart1.Height = 350;

            UltraChart1.ChartType = ChartType.StackColumnChart;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart1.Axis.X.Labels.SeriesLabels.OrientationAngle = 90;
            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL><br /><SERIES_LABEL><br /><b><DATA_VALUE:N0></b>&nbsp;тыс.руб.</span>";
           
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

        //    UltraChart1.Axis.X.Labels.FontSizeBestFit = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;

            UltraChart1.Axis.Y.Extent = 90;
            UltraChart1.Axis.X.Extent = 140;
           
            SetupTitleLeft();
            SetupCustomSkin();
           // UltraChart1.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart1.Legend.Font = new Font("Verdana", 12);
            UltraChart1.Legend.FontColor = Color.White;

            //UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            //UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);
        }

        private void SetupTitleLeft()
        {
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "тыс.руб.";
            UltraChart1.TitleLeft.Font = new Font("Verdana", 12);
            UltraChart1.TitleLeft.FontColor = Color.White;
            UltraChart1.TitleLeft.VerticalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = 150;
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
                case 2:
                    {
                        return  Color.FromArgb(74, 3, 125);
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

        private static string GetColorString(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return "#6ebdf1";
                    }
                case 2:
                    {
                        return "#d6ab85";
                    }
                case 3:
                    {
                        return "#8db269";
                    }
                case 4:
                    {
                        return "#c0b2e0";
                    }
                case 5:
                    {
                        return "#f5bb66";
                    }
                case 6:
                    {
                        return "#8ea4ec";
                    }
                case 7:
                    {
                        return "#d9e675";
                    }
                case 8:
                    {
                        return "#a29a62";
                    }
                case 9:
                    {
                        return "#8ec7db";
                    }
                case 10:
                    {
                        return "#b0d975";
                    }
            }
            return "#FFFFFF";
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