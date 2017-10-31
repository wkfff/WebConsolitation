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
    public partial class Food_0001_0001_Chart : UserControl
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

		private string city = "Вологда";

		public string City
		{
			get
			{
				return city;
			}
			set
			{
				city = value;
			}
		}

		public int LabelAreaHeight
		{
			set
			{
				LabelArea.Height = String.Format("{0}px", value);
			}
		}

        protected void Page_Load(object sender, EventArgs e)
        {
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            DataTable dtChart = new DataTable();
            dtChart.Columns.Add(new DataColumn("1", typeof(string)));
            dtChart.Columns.Add(new DataColumn("2", typeof(double)));

            DataRow row = dtChart.NewRow();
            row[0] = city;
            row[1] = source["Текущая цена"];
            dtChart.Rows.Add(row);

            row = dtChart.NewRow();
            row[0] = source["Минимум город"];
            row[1] = source["Минимум "];
            dtChart.Rows.Add(row);

            row = dtChart.NewRow();
            row[0] = source["Максимум город"];
            row[1] = source["Максимум "];
            dtChart.Rows.Add(row);

            SetColumnChartAppearance(0);

            UltraChart1.DataSource = dtChart;
            UltraChart1.DataBind();

            string img;
            if (source["Темп прироста (динамика к предыдущему периоду) "].ToString().Contains("-"))
            {
                img = "<img src='../../../images/arrowGreenDownBB.png'>";
            }
            else
            {
                img = "<img src='../../../images/arrowRedUpBB.png'>";
            }

            string absoluteGrown = (source["Абсолютное отклонение к прошлой дате"] != DBNull.Value &&
                !source["Абсолютное отклонение к прошлой дате"].ToString().Contains("-"))
                                                  ? String.Format("+{0:N2}", source["Абсолютное отклонение к прошлой дате"])
                                                  : String.Format("{0:N2}", source["Абсолютное отклонение к прошлой дате"]);

            Label1.Text = String.Format("<span style='Color: White'>{0}</span><br/>{1} руб.<br/>{2}&nbsp;{3:N2}%", city.Replace("Город ", "").Replace("город ", "").Replace("Великий", "В."), absoluteGrown, img, source["Темп прироста (динамика к предыдущему периоду) "]);
                        
            if (source["Темп прироста (динамика к предыдущему периоду) минимум"].ToString().Contains("-"))
            {
                img = "<img src='../../../images/arrowGreenDownBB.png'>";
            }
            else
            {
                img = "<img src='../../../images/arrowRedUpBB.png'>";
            }

            absoluteGrown = (source["Абсолютное отклонение к прошлой дате минимум"] != DBNull.Value &&
                !source["Абсолютное отклонение к прошлой дате минимум"].ToString().Contains("-"))
                                                  ? String.Format("+{0:N2}", source["Абсолютное отклонение к прошлой дате минимум"])
                                                  : String.Format("{0:N2}", source["Абсолютное отклонение к прошлой дате минимум"]);

            Label2.Text = String.Format("<span style='Color: White'>{0}</span><br/>{1} руб.<br/>{2}&nbsp;{3:N2}%", source["Минимум город"].ToString().Replace("Город ", "").Replace("город ", "").Replace("Великий", "В."), absoluteGrown, img, source["Темп прироста (динамика к предыдущему периоду) минимум"]);
                        
            if (source["Темп прироста (динамика к предыдущему периоду) максимум"].ToString().Contains("-"))
            {
                img = "<img src='../../../images/arrowGreenDownBB.png'>";
            }
            else
            {
                img = "<img src='../../../images/arrowRedUpBB.png'>";
            }

            absoluteGrown = (source["Абсолютное отклонение к прошлой дате максимум"] != DBNull.Value &&
                !source["Абсолютное отклонение к прошлой дате максимум"].ToString().Contains("-"))
                                                  ? String.Format("+{0:N2}", source["Абсолютное отклонение к прошлой дате максимум"])
                                                  : String.Format("{0:N2}", source["Абсолютное отклонение к прошлой дате максимум"]);

            Label3.Text = String.Format("<span style='Color: White'>{0}</span><br/>{1} руб.<br/>{2}&nbsp;{3:N2}%", source["Максимум город"].ToString().Replace("Город ", "").Replace("город ", "").Replace("Великий", "В."), absoluteGrown, img, source["Темп прироста (динамика к предыдущему периоду) максимум"]);
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            Collection<int> columnsX = new Collection<int>();
            Collection<int> columnsHeights = new Collection<int>();
            Collection<string> columnsValues = new Collection<string>();
            Collection<string> regions = new Collection<string>();
            int columnWidth = 0;
            int axisZero = 0;

            regions.Add("РФ");
            regions.Add(RegionsNamingHelper.ShortName(CustomParam.CustomParamFactory("region").Value));
            regions.Add(RegionsNamingHelper.ShortName(CustomParam.CustomParamFactory("state_area").Value));

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
                    text.SetTextString(value.ToString("N2") + "руб.");
                    text.labelStyle.HorizontalAlign = StringAlignment.Center;
                    e.SceneGraph.Add(text);

                    //text = new Text();
                    //text.labelStyle.Font = new Font("Arial", 12, FontStyle.Bold);

                    //text.PE.Fill = Color.White;
                    //yPos = value > 0 ? axisZero - columnsHeights[i] - 40 : axisZero + columnsHeights[i];
                    //text.bounds = new Rectangle(columnsX[i], yPos, columnWidth, 20);
                    //text.SetTextString(regions[i]);
                    //text.labelStyle.HorizontalAlign = StringAlignment.Center;
                    //e.SceneGraph.Add(text);
                }
            }
        }

        private void SetColumnChartAppearance(double value)
        {
            UltraChart1.Width = 370;
            UltraChart1.Height = 170;
            UltraChart1.ChartType = ChartType.ColumnChart;

            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
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
            SetupCustomSkin(value);
        }

        private void SetupCustomSkin(double value)
        {
            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.ApplyRowWise = false;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 4; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetColor(i);

                if (i == 3)
                {
                    if (value < 0)
                    {
                        color = Color.Orange;
                        stopColor = Color.Orange;
                    }
                }
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
                case 2:
                case 3:
                    {
                        return Color.DarkGray;
                    }
            }
            return Color.White;
        }
    }
}