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

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class STAT_0001_0004_Chart : UserControl
    {
        private DataRow source;

        public DataRow Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UltraChart1.FillSceneGraph += UltraChart1_FillSceneGraph;
            DataTable dtChart = new DataTable();
            dtChart.Columns.Add(new DataColumn("1", typeof(string)));
            dtChart.Columns.Add(new DataColumn("2", typeof(double)));

            for (int i = 1; i < 5; i++)
            {
                DataRow row = dtChart.NewRow();
                row[0] = source.Table.Columns[i].ColumnName.Split(';')[1];
                row[1] = source[i];
                dtChart.Rows.Add(row);
            }

            SetColumnChartAppearance();

            UltraChart1.DataSource = dtChart;
            UltraChart1.DataBind();

            string name = source.Table.Columns[1].ColumnName.Split(';')[1].Replace("Город ", "г.").Replace("муниципальный район", "р-н");
            Label1.Text = GetDescription(5, 9, name);
            name = source.Table.Columns[2].ColumnName.Split(';')[1].Replace("Город ", "г.").Replace("муниципальный район", "р-н");
            Label2.Text = GetDescription(6, 10, name);
            name = source[13].ToString().Replace("Город ", "г.").Replace("муниципальный район", "р-н");
            Label3.Text = GetDescription(7, 11, name);
            name = source[14].ToString().Replace("Город ", "г.").Replace("муниципальный район", "р-н");
            Label4.Text = GetDescription(8, 11, name);
        }

        private string GetDescription(int grownIndex, int absoluteGrownIndex, string name)
        {
            string img = string.Empty;
            double value;
            if (double.TryParse(source[grownIndex].ToString(), out value))
            {
                if (value != 0)
                {
                    img = source[grownIndex].ToString().Contains("-") ?
                        "<img src='../../../images/arrowGreenDownBB.png'>" :
                        "<img src='../../../images/arrowRedUpBB.png'>";
                }
            }

            string absoluteGrown = (source[absoluteGrownIndex] != DBNull.Value && !source[absoluteGrownIndex].ToString().Contains("-")) ?
                                   String.Format("+{0:N2}", source[absoluteGrownIndex]) :
                                   String.Format("{0:N2}", source[absoluteGrownIndex]);

            return String.Format("<span style='Color: White'>{0}</span><br/>{1}&nbsp;{2:N2}%", name, img, source[grownIndex]);
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            Collection<int> columnsX = new Collection<int>();
            Collection<int> columnsHeights = new Collection<int>();
            Collection<string> columnsValues = new Collection<string>();
            int columnWidth = 0;
            int axisZero = 0;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (axisZero == 0)
                        {
                            axisZero = box.rect.Y + box.rect.Height;
                        }
                        columnsX.Add(box.rect.X);
                        columnsHeights.Add(box.rect.Height);
                        columnWidth = box.rect.Width;
                        if (box.Value != null)
                        {
                            columnsValues.Add(box.Value.ToString());
                        }
                    }
                }
            }

            for (int i = 0; i < columnsValues.Count; i++)
            {
                double value;
                if (double.TryParse(columnsValues[i].ToString(), out value))
                {
                    Text text = new Text();
                    text.labelStyle.Font = new Font("Arial", 12, FontStyle.Bold);
                    text.PE.Fill = Color.White;

                    int yPos = value > 0 ? axisZero - columnsHeights[i] - 20 : axisZero + columnsHeights[i] + 20;

                    text.bounds = new Rectangle(columnsX[i], yPos, columnWidth, 20);
                    text.SetTextString(value.ToString("P2"));
                    text.labelStyle.HorizontalAlign = StringAlignment.Center;
                    e.SceneGraph.Add(text);
                }
            }
        }

        private void SetColumnChartAppearance()
        {
            UltraChart1.Width = 750;
            UltraChart1.Height = 170;
            UltraChart1.ChartType = ChartType.ColumnChart;

            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P1>";
            UltraChart1.Tooltips.FormatString = "";

            UltraChart1.Data.ZeroAligned = false;
            UltraChart1.Legend.Visible = false;

            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 12);
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 12);
            UltraChart1.Legend.Margins.Bottom = 70;
            UltraChart1.Legend.SpanPercentage = 20;

            UltraChart1.Axis.X.MajorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart1.Axis.Y.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            UltraChart1.Axis.X.MinorGridLines.Color = Color.Black;
            UltraChart1.Axis.Y.MinorGridLines.Color = Color.Black;

            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.LineColor = Color.Transparent;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart1.Axis.X.Extent = 0;
            UltraChart1.Axis.X2.Extent = 0;
            UltraChart1.Axis.Y.Extent = 40;
            UltraChart1.Style.Add("margin-left", "-5px");
            UltraChart1.TitleLeft.Visible = false;

            UltraChart1.BackColor = Color.Transparent;
            UltraChart1.BorderColor = Color.Transparent;
            SetupCustomSkin();
        }

        private void SetupCustomSkin()
        {
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 7; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetColor(i);
                                
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = i == 1 ? PaintElementType.Gradient : PaintElementType.Hatch;
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
                        return Color.FromArgb(145, 10, 149);
                    }
                default:
                    {
                        return Color.DarkGray;
                    }
            }
        }
    }
}