using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class iPad_0001_0001 : CustomReportPage
    {
        private int monthNum;
        private int yearNum;

        private int monthNumDebt = 9;
        private int yearNumDebt = 2010;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (String.IsNullOrEmpty(UserParams.Region.Value) ||
                String.IsNullOrEmpty(UserParams.StateArea.Value))
            {
                UserParams.Region.Value = "��������������� ����������� �����";
                UserParams.StateArea.Value = "���������� ����";
            }
            HeraldImageContainer.InnerHtml = String.Format("<a href ='{1}' <img style='margin-left: -30px; margin-right: 20px; height: 65px' src=\"../../../images/Heralds/{0}.png\"></a>", HttpContext.Current.Session["CurrentSubjectID"], HttpContext.Current.Session["CurrentSiteRef"]);
            InitializeIncomes();
            InitializeOutcomes();
            InitializeFonds();
            InitializeDebts();
            InitializeBudget();
        }

        #region ������
        private DataTable dt;

        private void InitializeIncomes()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("iPad_0001_0001_incomes_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
            UserParams.PeriodLastYear.Value = (Convert.ToInt32(dtDate.Rows[0][0]) - 1).ToString();
            UserParams.PeriodYear.Value = dtDate.Rows[0][0].ToString();

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            string monthStr;
            switch (monthNum)
            {
                case 1:
                    {
                        monthStr = "�����";
                        break;
                    }
                case 2:
                case 3:
                case 4:
                    {
                        monthStr = "������";
                        break;
                    }
                default:
                    {
                        monthStr = "�������";
                        break;
                    }
            }

            IncomesHeader.Text = string.Format("������ �� {0} {1} {2}�.", monthNum, monthStr, yearNum);

            dt = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_Incomes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            lbIncomesPlanValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["���������"]) / 1000000);
            lbIncomesFactValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["���������"]) / 1000000);
            lbIncomesExecutedValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["������� ���������� ����������"]) * 100);

            string img1 = String.Empty;
            if (dt.Rows[0]["���� �������"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["���� �������"]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["���� �������"] != DBNull.Value && dt.Rows[0]["������ ���� �������"] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[0]["���� �������"]) == Convert.ToInt32(dt.Rows[0]["������ ���� �������"]))
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            string img2 = string.Empty;
            if (dt.Rows[0]["���� ������� ��"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["���� ������� ��"]) == 1)
            {
                img2 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["���� ������� ��"] != DBNull.Value && dt.Rows[0]["������ ���� ������� ��"] != DBNull.Value &&
                     Convert.ToInt32(dt.Rows[0]["���� ������� ��"]) == Convert.ToInt32(dt.Rows[0]["������ ���� ������� ��"]))
            {
                img2 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbIncomesRankFO.Text = String.Format("���� {0}&nbsp;", RegionsNamingHelper.ShortName(UserParams.Region.Value));
            lbIncomesRankFOValue.Text = String.Format("{0:N0}{1}&nbsp;", dt.Rows[0]["���� �������"], img1);
            lbIncomesRankRFValue.Text = String.Format("{0:N0}{1}", dt.Rows[0]["���� ������� ��"], img2);

            lbIncomesRankFOValue.Style.Add("line-height", "40px");


            img1 = String.Empty;
            if (dt.Rows[0]["���� ���������"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["���� ���������"]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["���� ���������"] != DBNull.Value && dt.Rows[0]["������ ���� ���������"] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[0]["���� ���������"]) == Convert.ToInt32(dt.Rows[0]["������ ���� ���������"]))
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            img2 = string.Empty;
            if (dt.Rows[0]["���� ��������� ��"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["���� ��������� ��"]) == 1)
            {
                img2 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["���� ��������� ��"] != DBNull.Value && dt.Rows[0]["������ ���� ��������� ��"] != DBNull.Value &&
                     Convert.ToInt32(dt.Rows[0]["���� ��������� ��"]) == Convert.ToInt32(dt.Rows[0]["������ ���� ��������� ��"]))
            {
                img2 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbIncomesRankFOAverage.Text = String.Format("���� {0}&nbsp;", RegionsNamingHelper.ShortName(UserParams.Region.Value));
            lbIncomesRankFOAverageValue.Text = String.Format("{0:N0}{1}&nbsp;", dt.Rows[0]["���� ���������"], img1);
            lbIncomesRankRFAverageValue.Text = String.Format("{0:N0}{1}", dt.Rows[0]["���� ��������� ��"], img2);

            double value;
            if (Double.TryParse(dt.Rows[0]["������������� ������"].ToString(), out value))
            {
                lbIncomesAverageValue.Text = String.Format("{0:N1}", value);
            }
            if (Double.TryParse(dt.Rows[0]["����������� ����������� ���������"].ToString(), out value))
            {
                lbPopulationValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["����������� ����������� ���������"]));
            }
            lbIncomesRankFOAverageValue.Style.Add("line-height", "40px");

            UltraGauge gauge = GetGauge(Convert.ToDouble(dt.Rows[0][2]) * 100);
            //  gauge.ToolTip = String.Format("������� ���������� ����������: {0:P2}", dt.Rows[0][2]);
            gauge.Style.Add("padding-left", "45px");
            GaugeIncomesPlaceHolder.Controls.Add(gauge);
        }

        #endregion

        #region �������

        private void InitializeOutcomes()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("iPad_0001_0001_outcomes_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
            UserParams.PeriodLastYear.Value = dtDate.Rows[0][0].ToString();
            UserParams.PeriodYear.Value = (Convert.ToInt32(dtDate.Rows[0][0]) - 1).ToString();
            string lastlastYear = (Convert.ToInt32(dtDate.Rows[0][0]) - 1).ToString();
            UserParams.PeriodLastDate.Value = UserParams.PeriodDayFK.Value.Replace(UserParams.PeriodLastYear.Value, lastlastYear);

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            string monthStr;
            switch (monthNum)
            {
                case 1:
                    {
                        monthStr = "�����";
                        break;
                    }
                case 2:
                case 3:
                case 4:
                    {
                        monthStr = "������";
                        break;
                    }
                default:
                    {
                        monthStr = "�������";
                        break;
                    }
            }

            OutcomesHeader.Text = string.Format("������� �� {0} {1} {2}�.", monthNum, monthStr, yearNum);

            dt = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_Outcomes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            lbOutcomesPlanValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["���������"]) / 1000000);
            lbOutcomesFactValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["���������"]) / 1000000);
            lbOutcomesExecutedValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["������� ���������� ����������"]) * 100);

            string img1 = String.Empty;
            if (dt.Rows[0]["���� �������"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["���� �������"]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["���� �������"] != DBNull.Value && dt.Rows[0]["������ ���� �������"] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[0]["���� �������"]) == Convert.ToInt32(dt.Rows[0]["������ ���� �������"]))
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            string img2 = string.Empty;
            if (dt.Rows[0]["���� ������� ��"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["���� ������� ��"]) == 1)
            {
                img2 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["���� ������� ��"] != DBNull.Value && dt.Rows[0]["������ ���� ������� ��"] != DBNull.Value &&
                     Convert.ToInt32(dt.Rows[0]["���� ������� ��"]) == Convert.ToInt32(dt.Rows[0]["������ ���� ������� ��"]))
            {
                img2 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbOutcomesRankFOValue.Style.Add("line-height", "40px");

            lbOutcomesRankFO.Text = String.Format("���� {0}&nbsp;", RegionsNamingHelper.ShortName(UserParams.Region.Value));
            lbOutcomesRankFOValue.Text = String.Format("{0:N0}{1}&nbsp;", dt.Rows[0]["���� �������"], img1);
            lbOutcomesRankRFValue.Text = String.Format("{0:N0}{1}", dt.Rows[0]["���� ������� ��"], img2);

            img1 = String.Empty;
            if (dt.Rows[0]["���� ��������� ��������������"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["���� ��������� ��������������"]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["���� ��������� ��������������"] != DBNull.Value && dt.Rows[0]["������ ���� ��������� ��������������"] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[0]["���� ��������� ��������������"]) == Convert.ToInt32(dt.Rows[0]["������ ���� ��������� ��������������"]))
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            img2 = string.Empty;
            if (dt.Rows[0]["���� ��������� �������������� ��"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["���� ��������� �������������� ��"]) == 1)
            {
                img2 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["���� ��������� �������������� ��"] != DBNull.Value && dt.Rows[0]["������ ���� ��������� �������������� ��"] != DBNull.Value &&
                     Convert.ToInt32(dt.Rows[0]["���� ��������� �������������� ��"]) == Convert.ToInt32(dt.Rows[0]["������ ���� ��������� �������������� ��"]))
            {
                img2 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbOutcomesRankFOAverage.Text = String.Format("���� {0}&nbsp;", RegionsNamingHelper.ShortName(UserParams.Region.Value));
            lbOutcomesRankFOAverageValue.Text = String.Format("{0:N0}{1}&nbsp;", dt.Rows[0]["���� ��������� ��������������"], img1);
            lbOutcomesRankRFAverageValue.Text = String.Format("{0:N0}{1}", dt.Rows[0]["���� ��������� �������������� ��"], img2);

            double value;
            if (double.TryParse(dt.Rows[0]["����������� ��������� �������������� ���������"].ToString(), out value))
            {
                lbOutcomesAverageValue.Text = String.Format("{0:N1}", Convert.ToDouble(dt.Rows[0]["����������� ��������� �������������� ���������"]));
            }
            lbOutcomesRankFOAverageValue.Style.Add("line-height", "40px");

            UltraGauge gauge = GetGauge(Convert.ToDouble(dt.Rows[0][2]) * 100);
            // gauge.ToolTip = String.Format("������� ���������� ����������: {0:P2}", dt.Rows[0][2]);
            gauge.Style.Add("margin-left", "-40px");
            GaugeOutcomesPlaceHolder.Controls.Add(gauge);
        }

        #endregion

        #region �����

        private void InitializeFonds()
        {
            DataTable dtDate = new DataTable();
            dt = new DataTable();

            string query = DataProvider.GetQueryText("iPad_0001_0001_fonds_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodLastYear.Value = "2010";
            UserParams.PeriodYear.Value = "2011";

            query = DataProvider.GetQueryText("iPad_0001_0001_Fonds");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    dt.Columns[j].ColumnName = dt.Columns[j].ColumnName.TrimEnd('_');
                    dt.Columns[j].ColumnName = dt.Columns[j].ColumnName.Replace("br", "\n");
                    if (dt.Rows[i][j] != DBNull.Value)
                    {
                        dt.Rows[i][j] = Convert.ToDouble(dt.Rows[i][j]) / 1000;
                    }
                }
            }

            UltraChartFonds.DataSource = dt;
            UltraChartFonds.DataBind();

            UltraChartFonds.Width = 310;
            UltraChartFonds.Height = 345;
            ((GradientEffect)UltraChartFonds.Effects.Effects[0]).Coloring = GradientColoringStyle.Lighten;

            UltraChartFonds.ChartType = ChartType.StackColumnChart;
            UltraChartFonds.TitleLeft.Visible = true;
            UltraChartFonds.TitleLeft.Text = "                      ���.���.";
            UltraChartFonds.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChartFonds.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChartFonds.TitleLeft.FontColor = Color.FromArgb(209, 209, 209);
            UltraChartFonds.TitleLeft.Font = new Font("Verdana", 10);
            UltraChartFonds.Axis.X.Extent = 50;
            UltraChartFonds.Axis.Y.Extent = 40;

            UltraChartFonds.Tooltips.FormatString = "<span style='font-family: Arial; font-size: 14pt'><SERIES_LABEL> <ITEM_LABEL>\n<b><DATA_VALUE:N3></b>&nbsp;���.���.</span>";
            UltraChartFonds.Data.SwapRowsAndColumns = true;

            UltraChartFonds.Legend.Visible = true;
            UltraChartFonds.Legend.Location = LegendLocation.Bottom;
            UltraChartFonds.Legend.SpanPercentage = 20;
            //UltraChartFonds.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChartFonds_FillSceneGraph);
        }

        #endregion

        #region �����

        private static UltraGauge GetGauge(double markerValue)
        {
            UltraGauge gauge = new UltraGauge();
            gauge.Width = 300;
            gauge.Height = 61;
            gauge.DeploymentScenario.FilePath = "../../../TemporaryImages";
            gauge.DeploymentScenario.ImageURL = "../../../TemporaryImages/gauge_imfrf02_01_#SEQNUM(100).png";

            // ����������� �����
            LinearGauge linearGauge = new LinearGauge();
            linearGauge.CornerExtent = 10;
            linearGauge.MarginString = "2, 10, 2, 10, Pixels";

            // �������� �������� ����� 
            double endValue = (Math.Max(100, markerValue));

            LinearGaugeScale scale = new LinearGaugeScale();
            scale.EndExtent = 98;
            scale.StartExtent = 2;
            scale.OuterExtent = 93;
            scale.InnerExtent = 52;

            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(0, 255, 255, 255);
            gradientBrush.StartColor = Color.FromArgb(120, 255, 255, 255);
            scale.BrushElements.Add(gradientBrush);
            linearGauge.Scales.Add(scale);
            AddMainScale(endValue, linearGauge, markerValue);
            AddMajorTickmarkScale(endValue, linearGauge);
            AddGradient(linearGauge);

            linearGauge.Margin.Top = 1;
            linearGauge.Margin.Bottom = 1;

            gauge.Gauges.Add(linearGauge);
            return gauge;
        }

        private const int ScaleStartExtent = 5;
        private const int ScaleEndExtent = 97;

        private static void AddMajorTickmarkScale(double endValue, LinearGauge linearGauge)
        {
            LinearGaugeScale scale = new LinearGaugeScale();
            scale.EndExtent = ScaleEndExtent;
            scale.StartExtent = ScaleStartExtent;
            scale.MajorTickmarks.EndWidth = 2;
            scale.MajorTickmarks.StartWidth = 2;
            scale.MajorTickmarks.EndExtent = 40;
            scale.MajorTickmarks.StartExtent = 25;
            scale.MajorTickmarks.BrushElements.Add(new SolidFillBrushElement(Color.White));
            scale.Axes.Add(new NumericAxis(0, endValue + endValue / 30, endValue / 10));
            linearGauge.Scales.Add(scale);
        }

        private static void AddGradient(LinearGauge linearGauge)
        {
            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(150, 150, 150);
            gradientBrush.StartColor = Color.FromArgb(10, 255, 255, 255);
            gradientBrush.GradientStyle = Gradient.BackwardDiagonal;
            linearGauge.BrushElements.Add(gradientBrush);
        }

        private static void AddMainScale(double endValue, LinearGauge linearGauge, double markerValue)
        {
            LinearGaugeScale scale = new LinearGaugeScale();
            scale.EndExtent = ScaleEndExtent;
            scale.StartExtent = ScaleStartExtent;
            scale.Axes.Add(new NumericAxis(0, endValue + endValue / 30, endValue / 5));

            AddMainScaleRange(scale, markerValue);
            SetMainScaleLabels(scale);
            linearGauge.Scales.Add(scale);
        }

        private static void SetMainScaleLabels(LinearGaugeScale scale)
        {

            scale.Labels.ZPosition = LinearTickmarkZPosition.AboveMarkers;
            scale.Labels.Extent = 9;
            Font font = new Font("Arial", 9);
            scale.Labels.Font = font;
            scale.Labels.EqualFontScaling = false;
            SolidFillBrushElement solidFillBrushElement = new SolidFillBrushElement(Color.White);
            solidFillBrushElement.RelativeBoundsMeasure = Measure.Percent;
            Rectangle rect = new Rectangle(0, 0, 80, 0);
            solidFillBrushElement.RelativeBounds = rect;
            scale.Labels.BrushElements.Add(solidFillBrushElement);
            scale.Labels.Shadow.Depth = 2;
            scale.Labels.Shadow.BrushElements.Add(new SolidFillBrushElement());
        }

        private static void AddMainScaleRange(LinearGaugeScale scale, double markerValue)
        {
            LinearGaugeRange range = new LinearGaugeRange();
            range.EndValue = markerValue;
            range.StartValue = 0;
            range.OuterExtent = 80;
            range.InnerExtent = 20;
            SimpleGradientBrushElement gradientBrush = new SimpleGradientBrushElement();
            gradientBrush.EndColor = Color.FromArgb(1, 51, 75);
            gradientBrush.StartColor = Color.FromArgb(8, 218, 164);
            gradientBrush.GradientStyle = Gradient.Vertical;
            range.BrushElements.Add(gradientBrush);
            scale.Ranges.Add(range);
        }

        #endregion

        #region ����

        private void InitializeDebts()
        {
            SetIndicatorsData();
        }

        private DataTable dtDateYear = new DataTable();
        private DataTable dtDateMonths = new DataTable();

        private string GetMbtDescription(string group)
        {
            switch (group)
            {
                case "1":
                    {
                        return "���� ��� � �����������<br/>������� ����� 60%";
                    }
                case "2":
                    {
                        return "���� ��� � �����������<br/>������� �� 20% �� 60%";
                    }
                case "3":
                    {
                        return "���� ��� � �����������<br/>������� �� 5% �� 20%";
                    }
                case "4":
                    {
                        return "���� ��� � �����������<br/>������� ����� 5%";
                    }
                default:
                    {
                        return "���� ��� � �����������<br/>������� ����� 60%";
                    }
            }
        }

        private void SetIndicatorsData()
        {
            #region ����������
            dt = new DataTable();

            string query = DataProvider.GetQueryText("iPad_0001_0001_date_year_bk");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateYear);

            query = DataProvider.GetQueryText("iPad_0001_0001_date_months_bk");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateMonths);

            if (Convert.ToInt32(dtDateMonths.Rows[0][0].ToString()) > Convert.ToInt32(dtDateYear.Rows[0][0].ToString()))
            {
                UserParams.PeriodYearQuater.Value = string.Format("{0}", dtDateMonths.Rows[0][4]);
                UserParams.Filter.Value = "�� ������ �������� ����������";
            }
            else
            {
                UserParams.PeriodYearQuater.Value = string.Format("{0}", dtDateYear.Rows[0][1]);
                UserParams.Filter.Value = "�� ������ ������� ����������";
            }

            DataTable mbtGroup = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_mbtGroup");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, mbtGroup);

            lbRankCaption.Text = String.Format("������ �� ���� ��� �� {0} ���: ", yearNum);
            lbRankCaption.CssClass = "InformationText";

            if (mbtGroup.Rows[0][1] != DBNull.Value)
            {
                Rank.Text = string.Format("&nbsp;{0:N0}", mbtGroup.Rows[0][1]);
                Rank.CssClass = "DigitsValue";

                string imageUrl = String.Empty;
                switch (mbtGroup.Rows[0][1].ToString())
                {
                    case "1":
                        {
                            imageUrl = "~/images/ballRedBB.png";
                            break;
                        }
                    case "2":
                        {
                            imageUrl = "~/images/ballOrangeBB.png";
                            break;
                        }
                    case "3":
                        {
                            imageUrl = "~/images/ballYellowBB.png";
                            break;
                        }
                    case "4":
                        {
                            imageUrl = "~/images/ballGreenBB.png";
                            break;
                        }
                }

                imgMbt.ImageUrl = imageUrl;
                TooltipHelper.AddToolTip(mbtRankIng, GetMbtDescription(mbtGroup.Rows[0][1].ToString()), Page);
            }
            else
            {
                Rank.Text = String.Empty;
                lbRankCaption.Text = String.Empty;
                lbRankDescription.Text = String.Empty;
            }
            #endregion

            DataTable debtsDate = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_debts_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, debtsDate);

            monthNumDebt = CRHelper.MonthNum(debtsDate.Rows[0][3].ToString());
            yearNumDebt = Convert.ToInt32(debtsDate.Rows[0][0].ToString());

            UserParams.PeriodYear.Value =
                    CRHelper.PeriodMemberUName("",
                                           new DateTime(yearNumDebt, monthNumDebt, 1), 4);
            UserParams.PeriodLastDate.Value =
                    CRHelper.PeriodMemberUName("",
                               new DateTime(yearNumDebt - 1, monthNumDebt, 1), 4);
            if (monthNumDebt != 1)
            {
                UserParams.PeriodFirstYear.Value = CRHelper.PeriodMemberUName("", new DateTime(yearNumDebt, monthNumDebt-1, 1), 4);
                UserParams.PeriodLastYear.Value = CRHelper.PeriodMemberUName("", new DateTime(yearNumDebt - 1, monthNumDebt-1, 1), 4);
            }
            else
            {
                UserParams.PeriodFirstYear.Value = CRHelper.PeriodMemberUName("", new DateTime(yearNumDebt, 12, 1), 4);
                UserParams.PeriodLastYear.Value = CRHelper.PeriodMemberUName("", new DateTime(yearNumDebt - 1, 12, 1), 4);
            }
            
            DataTable debts = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_GosDebts_v");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", debts);

            if (debts.Rows[0]["��������"] == DBNull.Value ||
                Convert.ToDouble(debts.Rows[0]["��������"]) == 0)
            {
                debtsDiv.Visible = false;
                crimeText.Visible = true;
            }
            else
            {
                gosDebt.Text = String.Format("{0:N1}", debts.Rows[0]["��������"]);
                gosDebtAvg.Text = String.Format("{0:N1}", debts.Rows[3]["��������"]);

                string img1 = GetRankImgage(debts, 0, "���� ��������������� ����", "������ ���� ��������������� ����");
                string img2 = GetRankImgage(debts, 0, "���� ��������������� ���� ��", "������ ���� ��������������� ���� ��");

                lbGosDebtRankFO.Text =
                    String.Format("���� {0}&nbsp;", RegionsNamingHelper.ShortName(UserParams.Region.Value));
                lbGosDebtRankFOValue.Text =
                    String.Format("{0:N0}&nbsp;{1}", debts.Rows[0]["���� ��������������� ����"], img1);
                lbGosDebtRankRFValue.Text =
                    String.Format("{0:N0}&nbsp;{1}", debts.Rows[0]["���� ��������������� ���� ��"], img2);
                lbGosDebtRankFOValue.Style.Add("line-height", "30px");
                lbGosDebtRankRFValue.Style.Add("line-height", "30px");

                img1 = GetRankImgage(debts, 3, "���� ��������������� ����", "������ ���� ��������������� ����");
                img2 = GetRankImgage(debts, 3, "���� ��������������� ���� ��", "������ ���� ��������������� ���� ��");

                lbGosDebtAvgRankFO.Text =
                    String.Format("���� {0}&nbsp;", RegionsNamingHelper.ShortName(UserParams.Region.Value));
                lbGosDebtAvgRankFOValue.Text =
                    String.Format("{0:N0}&nbsp;{1}", debts.Rows[3]["���� ��������������� ����"], img1);
                lbGosDebtAvgRankRFValue.Text =
                    String.Format("{0:N0}&nbsp;{1}", debts.Rows[3]["���� ��������������� ���� ��"], img2);
                lbGosDebtAvgRankFOValue.Style.Add("line-height", "40px");
                lbGosDebtAvgRankRFValue.Style.Add("line-height", "40px");

                DataTable dtCrime = new DataTable();
                query = DataProvider.GetQueryText("iPad_0001_0001_crime");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtCrime);

                double crimeValue = 0;
                if (dtCrime.Rows.Count > 0 &&
                   dtCrime.Rows[0][0] != DBNull.Value)
                {
                    crimeValue = Convert.ToDouble(dtCrime.Rows[0][0]);
                }

                bool isCrime = false;
                string mbtPart = String.Empty;
                string borderValue = String.Empty;
                switch (mbtGroup.Rows[0][1].ToString())
                {
                    case "1":
                        {
                            isCrime = crimeValue >= 0.5;
                            mbtPart = "�����";
                            borderValue = "0,5000";
                            break;
                        }
                    case "2":
                    case "3":
                    case "4":
                        {
                            isCrime = crimeValue >= 1;
                            mbtPart = "�����";
                            borderValue = "1,0000";
                            break;
                        }
                }
                string crimeTooltip = isCrime ? String.Format("��������� ������ ���.�����<br/>(� ������ ������������ ��������� ��������)<br/>� ������ �������� ������ ������� �������<br/>�������� �� ��� ����� ������<br/>������������� �����������&nbsp;<b>{0:N4}</b><br/>���� ���&nbsp;<b>{1} 60%</b><br/>���������� ��������&nbsp;<b>{2}</b><br/>��������� ��.107 �� ��&nbsp;<img src=\"../../../images/ballRedBB.png\">", crimeValue, mbtPart, borderValue) :
                        String.Format("��������� ������ ���.�����<br/>(� ������ ������������ ��������� ��������)<br/>� ������ �������� ������ ������� �������<br/>�������� �� ��� ����� ������<br/>������������� �����������&nbsp;<b>{0:N4}</b><br/>���� ���&nbsp;<b>{1} 60%</b><br/>���������� ��������&nbsp;<b>{2}</b><br/>����������� ����������� ��.107 �� ��&nbsp;<img src=\"../../../images/ballGreenBB.png\">", crimeValue, mbtPart, borderValue);
                TooltipHelper.AddToolTip(UltraChartcrimeDiv, crimeTooltip, Page);
                if (crimeValue > 0)
                {
                    if (monthNum != 12)
                    {
                        crime.Text = isCrime
                                         ? String.Format(
                                               "��������� ����� � �������<br/>(��� �������.�����������)<br/>�� {1:dd.MM.yyyy}&nbsp;<span class=\"DigitsValue\">{0:N4}</span>&nbsp;<img src=\"../../../images/ballRedBB.png\">",
                                               crimeValue, new DateTime(yearNum, monthNum+1, 1))
                                         : String.Format(
                                               "��������� ����� � �������<br/>(��� �������.�����������)<br/>�� {1:dd.MM.yyyy}&nbsp;<span class=\"DigitsValue\">{0:N4}</span>&nbsp;<img src=\"../../../images/ballGreenBB.png\">",
                                               crimeValue, new DateTime(yearNum, monthNum+1, 1));
                    }
                    else
                    {
                        crime.Text = isCrime
                                         ? String.Format(
                                               "��������� ����� � �������<br/>(��� �������.�����������)<br/>�� {1:dd.MM.yyyy}&nbsp;<span class=\"DigitsValue\">{0:N4}</span>&nbsp;<img src=\"../../../images/ballRedBB.png\">",
                                               crimeValue, new DateTime(yearNum+1, 1, 1))
                                         : String.Format(
                                               "��������� ����� � �������<br/>(��� �������.�����������)<br/>�� {1:dd.MM.yyyy}&nbsp;<span class=\"DigitsValue\">{0:N4}</span>&nbsp;<img src=\"../../../images/ballGreenBB.png\">",
                                               crimeValue, new DateTime(yearNum+1, 1, 1));
                    }
                }
                else
                {
                    crime.Visible = false;
                }
                
                Label1.Text = String.Format("����� ����� �� {0:dd.MM.yyyy}", new DateTime(yearNumDebt, monthNumDebt + 1, 1));
                Label2.Text = String.Format("�� ���� ��������� �� {0:dd.MM.yyyy}", new DateTime(yearNumDebt, monthNumDebt + 1, 1));
            }
        }

        private static string GetRankImgage(DataTable debts, int row, string column, string worseColumn)
        {
            string img1 = String.Empty;
            if (debts.Rows[row][column] != DBNull.Value &&
                Convert.ToInt32(debts.Rows[row][column]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (debts.Rows[row][column] != DBNull.Value &&
                     debts.Rows[row][worseColumn] != DBNull.Value &&
                     Convert.ToInt32(debts.Rows[row][column]) ==
                     Convert.ToInt32(debts.Rows[row][worseColumn]))
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }
            return img1;
        }

        #endregion

        #region ������

        private void InitializeBudget()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("iPad_0001_0001_incomes_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
            UserParams.PeriodLastYear.Value = dtDate.Rows[0][0].ToString();
            UserParams.PeriodYear.Value = (Convert.ToInt32(dtDate.Rows[0][0]) + 1).ToString();

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());

            UltraWebGridBudget.Width = Unit.Empty;
            UltraWebGridBudget.Height = Unit.Empty;

            UltraWebGridBudget.DataBind();
        }

        protected void UltraWebGridBudget_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();

            string query = DataProvider.GetQueryText("iPad_0001_0001_budget_incomes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����������", dt);

            DataTable source = new DataTable();

            query = DataProvider.GetQueryText("iPad_0001_0001_budget_outcomes_all");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����������", source);

            foreach (DataRow row in source.Rows)
            {
                DataRow newRow = dt.NewRow();
                for (int i = 0; i < source.Columns.Count; i++)
                {
                    newRow[i] = row[i];
                }
                dt.Rows.Add(newRow);
            }

            source = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_budget_outcomes");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����������", source);

            foreach (DataRow row in source.Rows)
            {
                DataRow newRow = dt.NewRow();
                for (int i = 0; i < source.Columns.Count; i++)
                {
                    newRow[i] = row[i];
                }
                dt.Rows.Add(newRow);
            }

            source = new DataTable();
            query = DataProvider.GetQueryText("iPad_0001_0001_budget_deficite");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����������", source);

            foreach (DataRow row in source.Rows)
            {
                DataRow newRow = dt.NewRow();
                for (int i = 0; i < source.Columns.Count; i++)
                {
                    newRow[i] = row[i];
                }
                dt.Rows.Add(newRow);
            }

            // UltraWebGridBudget.Style.Add("margin-right", "-10px");
            UltraWebGridBudget.DataSource = dt;
        }

        protected void UltraWebGridBudget_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            if (e.Layout.Bands.Count == 0)
                return;

            e.Layout.Bands[0].Columns[0].Width = 181;
            e.Layout.Bands[0].Columns[1].Width = 85;
            e.Layout.Bands[0].Columns[2].Width = 85;
            e.Layout.Bands[0].Columns[3].Width = 64;
            e.Layout.Bands[0].Columns[4].Hidden = true;
            e.Layout.Bands[0].Columns[5].Hidden = true;

            //e.Layout.Bands[0].Columns[1].Header.Title =
            //    "����������� ���������� \n�� ����������� ������ \n�������� ����";
            //e.Layout.Bands[0].Columns[2].Header.Title =
            //                "���������";
            //e.Layout.Bands[0].Columns[3].Header.Title =
            //    "���� ����� \n� ������������ ������� \n�������� ����";

            for (int i = 1; i <= e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = false;

            }

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].HeaderStyle.Height = 10;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N1");
            e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("�� {0} ���. 2010�. ���.���.", monthNum);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N1");
            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("�� {0} ���. 2011�. ���.���.", monthNum);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");
            e.Layout.Bands[0].Columns[3].Header.Caption = "���� ����� %";

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N1");
            e.Layout.Bands[0].Columns[4].Header.Caption = String.Format("���� ����� {0}, %", RegionsNamingHelper.ShortName(UserParams.Region.Value));

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N1");
            e.Layout.Bands[0].Columns[5].Header.Caption = "���� ����� ��, %";

            e.Layout.RowStyleDefault.Padding.Top = 1;
            e.Layout.RowStyleDefault.Padding.Bottom = 1;
            e.Layout.RowStyleDefault.Padding.Left = 1;
            e.Layout.RowStyleDefault.Padding.Right = 1;
        }

        protected void UltraWebGridBudget_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString().ToLower() != "����� ������� " &&
                e.Row.Cells[0].Value.ToString().ToLower() != "����� �������� " &&
                e.Row.Cells[0].Value.ToString().ToLower() != "��������(+)/�������(-) ")
            {
                e.Row.Cells[0].Style.Padding.Left = 8;
            }
            if (e.Row.Cells[3] != null &&
                e.Row.Cells[3].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[3].Value.ToString());
                string img;
                if (value > 100)
                {
                    img = "~/images/arrowGreenUpBB.png";
                }
                else
                {
                    img = "~/images/arrowRedDownBB.png";
                }
                e.Row.Cells[3].Style.BackgroundImage = img;
                e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: 6px center; padding-top: 2px";
                //   e.Row.Cells[3].Title = title;
            }
            e.Row.Cells[1].Style.Padding.Right = 3;
            e.Row.Cells[2].Style.Padding.Right = 3;
            e.Row.Cells[3].Style.Padding.Right = 1;

            if (e.Row.Cells[0].Value.ToString().ToLower() == "��������(+)/ �������(-) ")
            {
                if (e.Row.Cells[1] != null &&
                e.Row.Cells[1].Value != null)
                {
                    double value = Convert.ToDouble(e.Row.Cells[1].Value.ToString());
                    string img;
                    if (value > 0)
                    {
                        img = "~/images/CornerGreen.gif";
                    }
                    else
                    {
                        img = "~/images/CornerRed.gif";
                    }
                    e.Row.Cells[1].Style.BackgroundImage = img;
                    e.Row.Cells[1].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-top: 3px; padding-right: 4px";
                    //   e.Row.Cells[1].Title = title;
                }

                if (e.Row.Cells[2] != null &&
                e.Row.Cells[2].Value != null)
                {
                    double value = Convert.ToDouble(e.Row.Cells[2].Value.ToString());
                    string img;
                    // string title;
                    if (value > 0)
                    {
                        img = "~/images/CornerGreen.gif";
                        //     title = "��������";
                    }
                    else
                    {
                        img = "~/images/CornerRed.gif";
                        //     title = "�������";
                    }
                    e.Row.Cells[2].Style.BackgroundImage = img;
                    e.Row.Cells[2].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-top: 3px; padding-right: 4px";
                    //  e.Row.Cells[2].Title = title;
                }
            }
        }

        #endregion
    }
}
