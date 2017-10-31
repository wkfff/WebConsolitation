using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebChart;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class FO_0004_0002_Chart : UserControl
    {
        private string queryName;

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

        private DateTime date;

        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
            }
        }

        private int colorModelStartIndex = 1;

        public int ColorModelStartIndex
        {
            get
            {
                return colorModelStartIndex;
            }
            set
            {
                colorModelStartIndex = value;
            }
        }

        public string Caption
        {
            get
            {
                return IPadElementHeader1.Text;
            }
            set
            {
                IPadElementHeader1.Text = value;
            }
        }

        private string percentFormat = "N2";
        public string PercentFormat
        {
            get
            {
                return percentFormat;
            }
            set
            {
                percentFormat = value;
            }
        }

        private string currencyFormat = "N2";
        public string CurrencyFormat
        {
            get
            {
                return currencyFormat;
            }
            set
            {
                currencyFormat = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetupChart(UltraChart1);
            UltraChart1.DataBind();
        }

        private void SetupChart(UltraChart chart)
        {
            chart.Width = 360;
            chart.Height = 300;

            chart.ChartType = ChartType.DoughnutChart;
            chart.Border.Thickness = 0;
            chart.DoughnutChart.OthersCategoryPercent = 0;
            chart.DoughnutChart.ShowConcentricLegend = false;
            chart.DoughnutChart.Concentric = true;
            chart.DoughnutChart.Labels.Font = new Font("Verdana", 8);
            chart.DoughnutChart.Labels.FontColor = Color.White;
            chart.DoughnutChart.Labels.LeaderLineColor = Color.White;
            chart.DoughnutChart.Labels.FormatString = String.Format("<PERCENT_VALUE:{0}>%", percentFormat);
            //chart.DoughnutChart.Labels.
            chart.Tooltips.FormatString = String.Format("<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL><br/><b><DATA_VALUE:{0}></b>&nbsp;млн.руб.\nдоля&nbsp;<b><PERCENT_VALUE:{1}>%</b></span>", currencyFormat, percentFormat);
            //chart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            // chart.FillSceneGraph += new FillSceneGraphEventHandler(chart_FillSceneGraph);
            chart.DataBinding += new EventHandler(chart_DataBinding);

            chart.Legend.Visible = false;
            chart.Legend.Location = LegendLocation.Left;
            chart.Legend.SpanPercentage = 40;
            chart.Legend.Margins.Top = 0;
            chart.Legend.Font = new Font("Verdana", 8);

            SetupCustomSkin();
            chart.ColorModel.Skin.ApplyRowWise = true;
            chart.BackColor = Color.Transparent;

            CalloutAnnotation planAnnotation = new CalloutAnnotation();
            planAnnotation.Text = date.Month == 1 ?
                String.Format("январь\n{1:yyyy} года", CRHelper.RusMonth(date.Month), date.AddYears(-1)) :
                String.Format("январь-{0}\n{1:yyyy} года", CRHelper.RusMonth(date.Month), date.AddYears(-1));
            planAnnotation.Width = 117;
            planAnnotation.Height = 32;
            planAnnotation.TextStyle.Font = new Font("Verdana", 10);
            planAnnotation.Location.Type = LocationType.Percentage;
            planAnnotation.TextStyle.HorizontalAlign = StringAlignment.Center;
            planAnnotation.Location.LocationX = 50;
            planAnnotation.Location.LocationY = 71;

            CalloutAnnotation factAnnotation = new CalloutAnnotation();
            factAnnotation.Text = date.Month == 1 ?
                String.Format("январь\n{1:yyyy} года", CRHelper.RusMonth(date.Month), date) :
                String.Format("январь-{0}\n{1:yyyy} года", CRHelper.RusMonth(date.Month), date);
            factAnnotation.Width = 117;
            factAnnotation.Height = 32;
            factAnnotation.TextStyle.Font = new Font("Verdana", 10);
            factAnnotation.Location.Type = LocationType.Percentage;
            factAnnotation.TextStyle.HorizontalAlign = StringAlignment.Center;
            factAnnotation.Location.LocationX = 50;
            factAnnotation.Location.LocationY = 14;

            chart.Annotations.Add(planAnnotation);
            chart.Annotations.Add(factAnnotation);
            chart.Annotations.Visible = true;
        }

        protected void chart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText(queryName);
            DataTable dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);

            UltraChart1.DataSource = dtChart1;
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Infragistics.UltraChart.Core.Primitives.Primitive primitive = e.SceneGraph[i];
                if (primitive is Infragistics.UltraChart.Core.Primitives.Wedge)
                {
                    Infragistics.UltraChart.Core.Primitives.Wedge wedge = (Infragistics.UltraChart.Core.Primitives.Wedge)primitive;
                    if (wedge.DataPoint != null)
                    {
                        string shortName = wedge.DataPoint.Label;
                        string fullName = DataDictionariesHelper.GetFullKDName(shortName);
                        string name = shortName == fullName ? fullName : string.Format("{0} ({1}) ", fullName, shortName);
                        wedge.DataPoint.Label = string.Format("{0}\n{1}", name, wedge.Column == 1 ? "план" : "факт");
                    }
                }
            }
        }

        private void SetupCustomSkin()
        {
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = colorModelStartIndex; i <= 12; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetStopColor(i);

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.Hatch = FillHatchStyle.ForwardDiagonal;
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
                case 11:
                    {
                        return Color.Cyan;
                    }
                case 12:
                    {
                        return Color.Gold;
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
                case 11:
                    {
                        return Color.Cyan;
                    }
                case 12:
                    {
                        return Color.Gold;
                    }
            }
            return Color.White;
        }
    }
}