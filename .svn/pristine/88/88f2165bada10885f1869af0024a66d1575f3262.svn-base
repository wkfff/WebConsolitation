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
using System.IO;
using System.Drawing.Imaging;
using System.Web;
using System.Xml;

namespace Krista.FM.Server.Dashboards.reports.Dashboard
{
    public partial class FST_0001_0001_Chart : UserControl
    {
        public string fileName = String.Empty;
        public LegendLocation LegendLocation = LegendLocation.Bottom;
        public int ChartWidth = 750;
        public int ChartHeight = 335;

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

        private double rfMiddleLevel = 0;

        public double RfMiddleLevel
        {
            get
            {
                return rfMiddleLevel;
            }
            set
            {
                rfMiddleLevel = value;
            }
        }

        private string taxName = "среднеотпускной тариф";

        public string TaxName
        {
            get
            {
                return taxName;
            }
            set
            {
                taxName = value;
            }
        }

        DataTable dtChart;
        private int currentYear = 2011;
        private string currentFO = Core.CustomParam.CustomParamFactory("region").Value;

        protected void Page_Load(object sender, EventArgs e)
        {
            dtChart = new DataTable();
            String query = DataProvider.GetQueryText(QueryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtChart);

            foreach (DataRow row in dtChart.Rows)
            {
                row[0] = GetWarpedLabel(row[0].ToString());
            }

            SetColumnChartAppearance();

            UltraChart1.Series.Clear();
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(1, dtChart));
            UltraChart1.Series.Add(CRHelper.GetNumericSeries(2, dtChart));
            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.DataBind();

            int minColIndex = 0;
            int maxColIndex = 0;
            double minValue = Double.MaxValue;
            double maxValue = Double.MinValue;

            int notZeroCount = 0;
            double foAvg = 0;

            int notZeroGrownCount = 0;
            double foGrownAvg = 0;

            for (int i = 0; i < dtChart.Rows.Count; i++)
            {
                double taxValue;
                if (dtChart.Rows[i][10] != DBNull.Value &&
                    Double.TryParse(dtChart.Rows[i][10].ToString(), out taxValue) &&
                    taxValue != 0)
                {
                    notZeroCount++;
                    foAvg += taxValue;
                }
                double value;
                if (dtChart.Rows[i][3] != DBNull.Value &&
                    Double.TryParse(dtChart.Rows[i][3].ToString(), out value) && value != -1)
                {
                    if (value < minValue)
                    {
                        minValue = value;
                        minColIndex = i;
                    }
                    if (value > maxValue)
                    {
                        maxValue = value;
                        maxColIndex = i;
                    }
                }
                double grownValue;
                if (dtChart.Rows[i][4] != DBNull.Value &&
                    Double.TryParse(dtChart.Rows[i][4].ToString(), out grownValue) &&
                    grownValue != -1)
                {
                    notZeroGrownCount++;
                    foGrownAvg += grownValue;
                }
            }

            #region выноски
            //minAnnotation = new CalloutAnnotation();
            //minAnnotation.Text = "";
            //minAnnotation.PE.ElementType = PaintElementType.Image;

            //minAnnotation.PE.FillImage = GetImage("~/images/starYellowBB.png");

            //minAnnotation.Width = 40;
            //minAnnotation.Height = 40;
            //minAnnotation.TextStyle.Font = new Font("Verdana", 10);
            //minAnnotation.Location.Type = LocationType.RowColumn;

            //minAnnotation.Location.Row = minColIndex; // субъекты
            //minAnnotation.Location.Column = 1; // величиины

            //minAnnotation.TextStyle.HorizontalAlign = StringAlignment.Center;

            //UltraChart1.Annotations.Add(minAnnotation);

            //maxAnnotation = new CalloutAnnotation();
            //maxAnnotation.Text = "";
            //maxAnnotation.PE.ElementType = PaintElementType.Image;
            //maxAnnotation.PE.FillImage = GetImage("~/images/starGrayBB.png");          

            //maxAnnotation.Width = 40;
            //maxAnnotation.Height = 40;
            //maxAnnotation.TextStyle.Font = new Font("Verdana", 10);
            //maxAnnotation.Location.Type = LocationType.RowColumn;

            //maxAnnotation.Location.Row = maxColIndex; // субъекты
            //maxAnnotation.Location.Column = 1; // величиины

            //maxAnnotation.TextStyle.HorizontalAlign = StringAlignment.Center;

            //UltraChart1.Annotations.Add(maxAnnotation);
            #endregion

            foAvg = notZeroCount == 0 ? 0 : foAvg / notZeroCount;
            foGrownAvg = notZeroGrownCount == 0 ? 0 : foGrownAvg / notZeroGrownCount;

            string rfMiddleLevelDescription = foGrownAvg > rfMiddleLevel ? "&nbsp;выше" : "&nbsp;ниже";

            Label1.Text = String.Format("В&nbsp;<b><span class='DigitsValue'>{0}</span></b>&nbsp;году в&nbsp;<b><span class='DigitsValue'>{1}</span></b>&nbsp;{9} {12}&nbsp;<b><span class='DigitsValueXLarge'>{2:N2}</span></b>&nbsp;{8}. По сравнению с&nbsp;<b><span class='DigitsValue'>{3}</span></b>&nbsp;годом темп прироста составил&nbsp;<b><span class='DigitsValueXLarge'>{4:P2}</span></b>, что{10}, чем средний темп прироста по РФ (<b><span class='DigitsValue'>{11:P2}</span></b>)",
                currentYear, RegionsNamingHelper.ShortName(currentFO), foAvg, currentYear - 1, foGrownAvg, 0, 0, 0, dtChart.Rows[0][9], taxName, rfMiddleLevelDescription, rfMiddleLevel, dtChart.Rows[0][9].ToString().Contains("Гкал") ? string.Empty : "составил").Replace(".. ", ". ");
        }

        CalloutAnnotation minAnnotation;
        CalloutAnnotation maxAnnotation;

        public static System.Drawing.Image GetImage(string path)
        {
            string imagePath = HttpContext.Current.Server.MapPath(path);
            // Загружаем картинку
            System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath);
            // Рисуем картинку с рамкой
            Bitmap bitmap = GetBitmap(Color.White, image);
            GetGraphics(Color.White, bitmap, image);
            // Создаем из нее картинку для ячейки.
            return GetImg(bitmap);
        }

        private static System.Drawing.Image GetImg(Bitmap bitmap)
        {
            MemoryStream imageStream = new MemoryStream();
            bitmap.Save(imageStream, ImageFormat.Png);
            System.Drawing.Image image = System.Drawing.Image.FromStream(imageStream);

            return image;
        }

        private static Graphics GetGraphics(Color backColor, Bitmap bitmap, System.Drawing.Image image)
        {
            SolidBrush brush = new SolidBrush(backColor);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
            graphics.DrawImageUnscaled(image, 2, 2);
            return graphics;
        }

        private static Bitmap GetBitmap(Color backColor, System.Drawing.Image image)
        {
            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            bitmap.SetResolution(60, 50);

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    if (bitmap.GetPixel(x, y) == Color.FromArgb(0, 0, 0, 0))
                    {
                        bitmap.SetPixel(x, y, backColor);
                    }
                }
            }
            return bitmap;
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            bool firstSeriesTextHided = false;
            bool firstSeriesBoxHided = false;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive.ToString() == "Infragistics.UltraChart.Core.Primitives.Text"
                    && primitive.Path == "Border.Title.Left")
                {
                    Text titleLeft = (Text)primitive;
                    titleLeft.bounds.Width = 50;
                    titleLeft.bounds.Height = ChartHeight - (ChartHeight / 36);
                    titleLeft.bounds.X = 1;
                    titleLeft.bounds.Y = ChartHeight / 50;

                }
                else if ((primitive.ToString() == "Infragistics.UltraChart.Core.Primitives.Box"
                    && primitive.Path == "Legend") && !firstSeriesBoxHided)
                {
                    primitive.Visible = false;
                    firstSeriesBoxHided = true;
                }
                else if ((primitive.ToString() == "Infragistics.UltraChart.Core.Primitives.Text"
                && String.IsNullOrEmpty(primitive.Path)) && !firstSeriesTextHided)
                {
                    primitive.Visible = false;
                    firstSeriesTextHided = true;
                }
                else if ((primitive.ToString() == "Infragistics.UltraChart.Core.Primitives.Box"
                   && primitive.Path == "Legend") && firstSeriesBoxHided)
                {
                    Box box = (Box)primitive;
                    if (LegendLocation == LegendLocation.Bottom)
                    {                        
                        box.rect.X = 65;
                    }
                    else
                    {
                        box.rect.Y = 15;
                    }
                }
                else if ((primitive.ToString() == "Infragistics.UltraChart.Core.Primitives.Text"
                && String.IsNullOrEmpty(primitive.Path)) && firstSeriesTextHided)
                {
                    Text legeng = (Text)primitive;
                    if (LegendLocation == LegendLocation.Bottom)
                    {                        
                        legeng.SetTextString(String.Format("Рост тарифа к {0} году", currentYear - 1));
                        legeng.bounds.Width = 600;
                        legeng.bounds.X = 80;
                    }
                    else
                    {
                        legeng.SetTextString(String.Format("Рост тарифа\nк {0} году", currentYear - 1));
                        legeng.bounds.Width = 100;
                        legeng.bounds.Height = 100;
                        legeng.bounds.Y = 15;
                    }
                }
                if (primitive is Box)
                {

                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        DataRow row = dtChart.Rows[box.Row];

                        box.DataPoint.Label = String.Format("{0}<br/>{1}<br/>на&nbsp;<b>{10}</b>&nbsp;год&nbsp;<b>{9}</b>&nbsp;{2}<br/>Темп прироста тарифа&nbsp;<b>{6:P2}</b>",
                            RegionsNamingHelper.FullName(row[0].ToString().Replace("*", "")),
                            iPadBricks.iPadBricks.iPadBricksHelper.GetWarpedHint(dtChart.Columns[10].ColumnName),
                            row[9],
                            RegionsNamingHelper.ShortName(RegionsNamingHelper.GetFoBySubject(RegionsNamingHelper.FullName(row[0].ToString().Replace("*", "")))),
                            row["Ранг тариф ФО"],
                            row["Ранг тариф РФ"],
                            row[4],
                            row["Ранг прирост тарифа ФО"],
                            row["Ранг прирост тарифа РФ"],
                            row[10],
                            currentYear,
                            GetRankImg(row, "Ранг тариф ФО", "Худший тариф ФО"),
                            GetRankImg(row, "Ранг прирост тарифа ФО", "Худший прирост тарифа ФО"),
                            GetRankImg(row, "Ранг тариф РФ", "Худший тариф РФ"),
                            GetRankImg(row, "Ранг прирост тарифа РФ", "Худший прирост тарифа РФ"));
                    }
                }
            }
        }

        private static string GetRankImg(DataRow row, string columnName, string worseColumnName)
        {
            string foFirstRankImg = string.Empty;
            if (row[columnName] != DBNull.Value)
            {
                if (row[columnName].ToString() == "1")
                {
                    foFirstRankImg = "<img src='../../../images/starGray.png'>";
                }
                else if (row[columnName].ToString() == row[worseColumnName].ToString())
                {
                    foFirstRankImg = "<img src='../../../images/starYellow.png'>";
                }
            }
            return foFirstRankImg;
        }

        private void SetColumnChartAppearance()
        {
            UltraChart1.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

            UltraChart1.Width = ChartWidth;
            UltraChart1.Height = ChartHeight;
            UltraChart1.ChartType = ChartType.StackColumnChart;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Custom;
            UltraChart1.Axis.X.Labels.SeriesLabels.OrientationAngle = 90;

            UltraChart1.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL></span>";
            //SetLabelsClipTextBehavior(UltraChart1.Axis.X.Labels.SeriesLabels.Layout);

            UltraChart1.Axis.Y.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart1.Axis.X.MajorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart1.Axis.X.MajorGridLines.DrawStyle = LineDrawStyle.Dot;
            UltraChart1.Axis.X.MinorGridLines.Color = Color.FromArgb(150, 150, 150);
            UltraChart1.Axis.Y.MinorGridLines.Color = Color.FromArgb(150, 150, 150);

            UltraChart1.Axis.X.Labels.SeriesLabels.FontSizeBestFit = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.VerticalAlign = StringAlignment.Far;

            UltraChart1.Axis.Y.Labels.FontSizeBestFit = false;
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 10);
            UltraChart1.Axis.Y.Labels.Visible = true;

            UltraChart1.Axis.Y.Extent = 70;
            UltraChart1.Axis.X.Extent = 170;

            UltraChart1.Legend.Font = new Font("Verdana", 10);
            UltraChart1.Legend.SpanPercentage = 10;
            UltraChart1.Legend.Location = LegendLocation;
            if (LegendLocation == LegendLocation.Bottom)
            {
                UltraChart1.Legend.Margins.Left = 30;
            }

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
            UltraChart1.BackColor = Color.Transparent;
            UltraChart1.Border.Color = Color.Transparent;

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = String.Format("{2}\nна {1} год,{0}", dtChart.Rows[0][9], currentYear, taxName);
            //UltraChart1.TitleLeft.Margins.Bottom = 25;
            UltraChart1.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.VerticalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.FontColor = Color.White; //Color.FromArgb(209, 209, 209);
            UltraChart1.TitleLeft.Font = new Font("Verdana", 10);
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.Aqua;
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

        public static string GetWarpedLabel(string label)
        {
            string name = label.Replace("\"", "'");
            if (name.Length > 21)
            {
                int k = 10;

                for (int j = 0; j < name.Length; j++)
                {
                    k++;
                    if (k > 15 && name[j] == ' ')
                    {
                        name = name.Insert(j, "\n");
                        k = 0;
                    }
                }
            }
            return name;
        }

        private string GetName(string name)
        {
            // загружаем документ
            string xmlFile = HttpContext.Current.Server.MapPath(fileName);
            if (!File.Exists(xmlFile))
            {
                return name;
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFile);
                // Ищем узел регионов
                foreach (XmlNode rootNode in doc.ChildNodes)
                {
                    if (rootNode.Name == "RegionsList")
                    {
                        foreach (XmlNode regionNode in rootNode.ChildNodes)
                        {
                            if (RegionsNamingHelper.ShortName(regionNode.Attributes["name"].Value) == name)
                            {
                                return name;
                            }
                        }
                    }
                }
            }
            Label3.Visible = true;
            return name += '*';
        }
    }
}