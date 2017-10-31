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
    public partial class SE_0001_0002_Chart : UserControl
    {
        private DateTime reportDate;

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

        private string dateQueryName = String.Empty;

        public string DateQueryName
        {
            get
            {
                return dateQueryName;
            }
            set
            {
                dateQueryName = value;
            }
        }
                
        public string DetalizationReportId
        {
            get { return IPadElementHeader1.DetalizationReportId; }
            set { IPadElementHeader1.DetalizationReportId = value; }
        }

        public string Text
        {
            get { return IPadElementHeader1.Text; }
            set { IPadElementHeader1.Text = value; }
        }

        public bool Multitouch
        {
            get { return IPadElementHeader1.Multitouch; }
            set { IPadElementHeader1.Multitouch = value; }
        }

        public string DescriptionText
        {
            get { return lbDescription.Text; }
            set { lbDescription.Text = value; }
        }

        public string Width
        {
            get
            {
                return IPadElementHeader1.Width;
            }
            set
            {
                IPadElementHeader1.Width = value;
            }
        }

        private int year = 2000;
        private int monthNum = 0;

        private int rangeMax = 60;
        private int rangeMin = -20;

        protected void Page_Load(object sender, EventArgs e)
        {
            InitializeDate();

            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            DataTable dtIncomes = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);

            string sphere = String.Empty;
            if (dtIncomes.Columns[1].ColumnName != "Индекс промышленного производства ")
            {
                sphere = String.Format("в сфере «{0}»", dtIncomes.Columns[1].ColumnName);
            }

            if (!(dtIncomes.Rows[2][1] == DBNull.Value) && dtIncomes.Rows[2][1].ToString() != String.Empty)
            {
                string grown = "прирост";
                string constitute = "составил";
                string img = "<img src='../../../images/arrowGreenUpIPad.png'>";
                string br = dtIncomes.Columns[1].ColumnName.Contains("электроэнергии") ? "&nbsp;" : "<br/>";
                if (dtIncomes.Rows[2][1].ToString().Contains("-"))
                {
                    grown = "снижение";
                    constitute = "составило";
                    img = "<img src='../../../images/arrowRedDownIPad.png'>";

                    double indexValue = Convert.ToDouble(dtIncomes.Rows[2][1].ToString());
                    while (indexValue - rangeMin < 10)
                    {
                        rangeMin -= 10;
                    }
                }
                else
                {
                    double indexValue = Convert.ToDouble(dtIncomes.Rows[2][1].ToString());
                    while (rangeMax - indexValue < 10)
                    {
                        rangeMax += 10;
                    }
                    if (rangeMax > 200)
                    {
                        rangeMax += 20;
                    }
                }

                string rankDescription =
                    String.Format(
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Ранг {0}&nbsp;<span class='DigitsValue'>{2:N0}</span>&nbsp;{1}&nbsp;РФ&nbsp;<span class='DigitsValue'>{4:N0}</span>&nbsp;{3}",
                        RegionsNamingHelper.ShortName(CustomParam.CustomParamFactory("region").Value),
                        GetRankImgage(dtIncomes, 2, "Ранг по ФО", "Худший ранг ФО"), dtIncomes.Rows[2][4],
                        GetRankImgage(dtIncomes, 2, "Ранг по РФ", "Худший ранг РФ"), dtIncomes.Rows[2][2]);

                string period = monthNum == 1 ? "январь" : String.Format("январь-{0}", CRHelper.RusMonth(monthNum));

                lbDescription.Text =
                    String.Format(
                        "За&nbsp;<span class='DigitsValue'>{0} {1}</span>&nbsp;года&nbsp;<span class='DigitsValue'>{3}</span><br/>индекса промышленного производства {7}{8}{4}&nbsp;{5}<span class='DigitsValue'>&nbsp;{2:N1}%</span> &nbsp; по сравнению с аналогичным периодом прошлого года.<br/>{6}",
                        period, year, dtIncomes.Rows[2][1], grown, constitute, img, rankDescription,
                        sphere, br);
                
            }
            else
            {
                lbDescription.Text =
                    String.Format(
                        "За&nbsp;<span class='DigitsValue'>январь-{0} {1}</span>&nbsp;года индекс промышленного производства {2} отсутствует.",
                        CRHelper.RusMonth(monthNum), year, sphere);
            }

            for (int i = 5; i >= 2; i--)
            {
                dtIncomes.Columns.RemoveAt(i);
            }

            

            double value = 0;
            if (dtIncomes.Rows[2][1] != DBNull.Value)
            {
                value = Convert.ToDouble(dtIncomes.Rows[2][1]);
            }
            SetColumnChartAppearance(value);

            UltraChart1.DataSource = dtIncomes;
            UltraChart1.DataBind();
        }

        private void InitializeDate()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText(dateQueryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0]);
            reportDate = new DateTime(year, monthNum, 1);

            CustomParams UserParams = new CustomParams();
            UserParams.PeriodMonth.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate, 4);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate.AddYears(-1), 4);
            UserParams.PeriodLastYear.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate.AddYears(-1), 1);
            UserParams.PeriodEndYear.Value = CRHelper.PeriodMemberUName(String.Empty, reportDate.AddYears(-2), 1);
        }

        private static string GetRankImgage(DataTable dt, int row, string column, string worseColumn)
        {
            string img1 = String.Empty;
            if (dt.Rows[row][column] != DBNull.Value &&
                Convert.ToInt32(dt.Rows[row][column]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[row][column] != DBNull.Value &&
                     dt.Rows[row][worseColumn] != DBNull.Value &&
                     Convert.ToInt32(dt.Rows[row][column]) ==
                     Convert.ToInt32(dt.Rows[row][worseColumn]))
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }
            return img1;
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

                    int yPos = value > 0 ? axisZero - columnsHeights[i] - 20: axisZero + columnsHeights[i] + 20;

                    text.bounds = new Rectangle(columnsX[i], yPos, columnWidth, 20);
                    text.SetTextString(value.ToString("N1") + "%");
                    text.labelStyle.HorizontalAlign = StringAlignment.Center;
                    e.SceneGraph.Add(text);

                    text = new Text();
                    text.labelStyle.Font = new Font("Arial", 12, FontStyle.Bold);

                    text.PE.Fill = Color.White;
                    yPos = value > 0 ? axisZero - columnsHeights[i] - 40: axisZero + columnsHeights[i];
                    text.bounds = new Rectangle(columnsX[i], yPos, columnWidth, 20);
                    text.SetTextString(regions[i]);
                    text.labelStyle.HorizontalAlign = StringAlignment.Center;
                    e.SceneGraph.Add(text);
                }
            }
        }

        private void SetColumnChartAppearance(double value)
        {
            UltraChart1.Width = 370;
            UltraChart1.Height = 230;
            UltraChart1.ChartType = ChartType.ColumnChart;

            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><ITEM_LABEL><br /><b><DATA_VALUE:N1></b>%.</span>";

            UltraChart1.Data.ZeroAligned = true;
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
            UltraChart1.Axis.Y.RangeType = AxisRangeType.Custom;
            UltraChart1.Axis.Y.RangeMin = rangeMin;
            UltraChart1.Axis.Y.RangeMax = rangeMax;

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
                    //else
                    //{
                    //    color = Color.LimeGreen;
                    //    stopColor = Color.LimeGreen;
                    //}
                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = i == 3 ? PaintElementType.Gradient : PaintElementType.Hatch;
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
                case 2:
                    {
                        return Color.DarkGray;
                    }
                case 3:
                    {
                        return Color.FromArgb(145, 10, 149);
                    }
            }
            return Color.White;
        }
    }
}