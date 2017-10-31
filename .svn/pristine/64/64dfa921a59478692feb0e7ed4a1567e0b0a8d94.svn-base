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
    public partial class IT_0002_0001_BanksChart : UserControl
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
            dtIncomes.Rows[0][0] = dtIncomes.Rows[0][0].ToString().Replace("br", "\n");
            dtIncomes.Rows[1][0] = dtIncomes.Rows[1][0].ToString().Replace("br", "\n");
            UltraChart3.DataSource = dtIncomes;
            UltraChart3.DataBind();
        }

        private void SetStackChartAppearanceUnic()
        {
            UltraChart3.Width = 380;
            UltraChart3.Height = 115;
            UltraChart3.ChartType = ChartType.PieChart;
            UltraChart3.Border.Thickness = 0;
            UltraChart3.Axis.Y.Extent = 5;
            UltraChart3.Axis.X.Extent = 5;
            UltraChart3.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N0> (<PERCENT_VALUE:N2>%)";
            UltraChart3.Legend.Visible = false;
            UltraChart3.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart3.PieChart.Labels.FormatString = "<ITEM_LABEL>\n<PERCENT_VALUE:N2>%";
            UltraChart3.PieChart.Labels.Font = new Font("Verdana", 12);
            UltraChart3.PieChart.Labels.FontColor = Color.White;
            UltraChart3.PieChart.Labels.LeaderLineColor = Color.White;
            UltraChart3.PieChart.Labels.LeaderDrawStyle = LineDrawStyle.Solid;
            UltraChart3.PieChart.Labels.LeaderEndStyle = LineCapStyle.DiamondAnchor;
            UltraChart3.PieChart.RadiusFactor = 110;
            UltraChart3.PieChart.StartAngle = 300;

            UltraChart3.ColorModel.ModelStyle = ColorModels.CustomSkin;
            //UltraChart3.ColorModel.Skin.ApplyRowWise = false;
            UltraChart3.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                switch (i)
                {
                    case 2:
                        {
                            color = Color.FromArgb(74, 3, 125);
                            stopColor = Color.FromArgb(74, 3, 125);
                            break;
                        }
                    case 1:
                        {
                            color = Color.FromArgb(145, 10, 149);
                            stopColor = Color.FromArgb(145, 10, 149);
                            break;
                        }
                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                UltraChart3.ColorModel.Skin.PEs.Add(pe);
            }
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.FromArgb(110, 189, 241);
                    }
                case 2:
                    {
                        return Color.FromArgb(214, 171, 133);
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