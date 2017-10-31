using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraGauge.Resources;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Text = Infragistics.UltraChart.Core.Primitives.Text;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_1035_0015 : CustomReportPage
    {
        private int worseRank = 19;

        private CustomParam settlementLevel;
        private CustomParam regionsConsolidateLevel;
        private CustomParam incomeTotal;
        private CustomParam outcomeFKRTotal;
        private CustomParam regionsLevel;
        private CustomParam populationDimension;
        private CustomParam populationValue;
        private CustomParam incomesKDClassifier;
        private DateTime date;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            InitRegionSettings();
            
            if (UserParams.Mo.Value == null ||
                UserParams.Mo.Value == String.Empty)
            {
                UserParams.Mo.Value = "����������� ��";
            }

            if (UserParams.Mo.Value == "������������� ��")
            {
                UserParams.Filter.Value = 
                        "[����������__������������].[����������__������������].[��� ����������].[����������  ���������].[������-�������� ����������� �����].[���������� �������].[�. ����������]";
            }
            else if (UserParams.Mo.Value == "�������������� ��")
            {
                UserParams.Filter.Value =
                        "[����������__������������].[����������__������������].[��� ����������].[����������  ���������].[������-�������� ����������� �����].[���������� �������].[�. ������������]";
            }
            else
            {
                UserParams.Filter.Value = String.Format("[����������__������������].[����������__������������].[��� ����������].[����������  ���������].[������-�������� ����������� �����].[���������� �������].[{0}]",
                        UserParams.Mo.Value.Replace("��", "������������� �����"));
            }
            CustomParam populationDate = UserParams.CustomParam("population_date");
            DataTable dtPopulationDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_1035_0015_population_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtPopulationDate);
            populationDate.Value = dtPopulationDate.Rows[0][1].ToString();
            DataTable dtPopulation = new DataTable();
            query = DataProvider.GetQueryText("FO_1035_0015_population");

            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "dummy", dtPopulation);
            double population = Convert.ToDouble(dtPopulation.Rows[0][0]);

            populationValue.Value = population.ToString().Replace(',', '.');

            lbPopulation.Text = String.Format("����������� ����������� ���������&nbsp;<span style=\"color: white\"><b>{0:N0}</b></span>&nbsp;���.", population);

            UserParams.Filter.Value = RegionsNamingHelper.LocalBudgetUniqueNames[UserParams.Mo.Value];

            if (UserParams.Mo.Value.Contains("��") || UserParams.Mo.Value.Contains("�.") || UserParams.Mo.Value.Contains("�.�.") || UserParams.Mo.Value.ToLower().Contains("����� ") || UserParams.Mo.Value.ToLower().Contains("��������"))
            {
                settlementHeader.Visible = false;
                UltraWebGridSettlements.Visible = false;
                settlementsDiv.Visible = false;
                lbIncomesRankFO.Text = "���� ��&nbsp;";
                lbOutcomesRankFO.Text = "���� ��&nbsp;";
                lbIncomesRankFOAverage.Text = "���� ��&nbsp;";
                lbOutcomesRankFOAverage.Text = "���� ��&nbsp;";
                worseRank = 18;
            }
            else
            {
                InitializeSettlement();
                settlementsDiv.Visible = false;
                lbIncomesRankFO.Text = "���� ��&nbsp;";
                lbOutcomesRankFO.Text = "���� M�&nbsp;";
                lbIncomesRankFOAverage.Text = "���� M�&nbsp;";
                lbOutcomesRankFOAverage.Text = "���� M�&nbsp;";
                worseRank = 18;
            }

            //HeraldImageContainer.InnerHtml = String.Format("<a href ='{1}' <img style='margin-left: -30px; margin-right: 20px; height: 65px' src=\"../../../images/HeraldsMo/{0}.png\"></a>", HttpContext.Current.Session["CurrentMoID"], HttpContext.Current.Session["CurrentMoSiteRef"]);
            //HeraldImageContainer.Visible = false;

            DataTable dtDate = new DataTable();
            query = DataProvider.GetQueryText("FO_1035_0015_incomes_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            month = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0].ToString());

            UserParams.PeriodLastYear.Value = string.Format("[{0}].[��������� 2].[������� 4].[�������]", year - 1);
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(month));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(month));
            UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, CRHelper.RusMonth(month));

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            InitializePopulation();

            InitializeIncomes();

            dtDate = new DataTable();
            query = DataProvider.GetQueryText("FO_1035_0015_outcomes_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            month = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0].ToString());

            UserParams.PeriodLastYear.Value = string.Format("[{0}].[��������� 2].[������� 4].[�������]", year - 1);
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(month));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(month));
            UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, CRHelper.RusMonth(month));

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            InitializeOutcomes();

            dtDate = new DataTable();
            query = DataProvider.GetQueryText("FO_1035_0015_incomes_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            month = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0].ToString());

            UserParams.PeriodLastYear.Value = string.Format("[{0}].[��������� 2].[������� 4].[�������]", year - 1);
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(month));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(month));
            UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, CRHelper.RusMonth(month));

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            date = new DateTime(year, month, 1).AddMonths(1);

            InitializeBkku();
            InitializeBudget();

            DataTable dt�reditDebts = new DataTable();
            query = DataProvider.GetQueryText("FO_1035_0015_credit_debts");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dt�reditDebts);

            creditDebts.Text = String.Format("����� ������������ �������������<br/><span style=\"color: white\"><b>{0:N0}</b></span>&nbsp;���.���., � ��� ����� ��:<br/>&nbsp;&nbsp;&nbsp;&nbsp;������ �����<br/>&nbsp;&nbsp;&nbsp;&nbsp;� ������������:&nbsp;<span style=\"color: white\"><b>{1:N0}</b></span>&nbsp;���.���.<br/>&nbsp;&nbsp;&nbsp;&nbsp;������������ {4}�������:&nbsp;<span style=\"color: white\"><b>{2:N0}</b></span>&nbsp;���.���.<br/>&nbsp;&nbsp;&nbsp;&nbsp;������ ��������:&nbsp;<span style=\"color: white\"><b>{3:N0}</b></span>&nbsp;���.���.", dt�reditDebts.Rows[0][2], dt�reditDebts.Rows[1][2], dt�reditDebts.Rows[2][2], dt�reditDebts.Rows[3][2], Convert.ToDouble(dt�reditDebts.Rows[2][2].ToString()) > 100000 ? string.Empty : string.Empty);

            fns28nSplitting = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("FNS28nSplitting"));
            cubeName.Value = fns28nSplitting ? "���_28�_� ������������" : "���_28�_��� �����������";

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            topRegionsCount.Value = "5";

            UltraWebGrid.DataBind();
            UltraWebGrid.Height = Unit.Empty;
        }

        private void InitRegionSettings()
        {
            settlementLevel = UserParams.CustomParam("settlement_level");
            settlementLevel.Value = RegionSettingsHelper.Instance.SettlementLevel;

            regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;

            incomeTotal = UserParams.CustomParam("income_total");
            incomeTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;

            outcomeFKRTotal = UserParams.CustomParam("outcome_FKR_total");
            outcomeFKRTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;

            regionsLevel = UserParams.CustomParam("regions_level");
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;

            populationDimension = UserParams.CustomParam("population_measure");
            populationDimension.Value = RegionSettingsHelper.Instance.PopulationMeasure;

            populationValue = UserParams.CustomParam("population_value");

            incomesKDClassifier = UserParams.CustomParam("incomes_KD_classifier");
            incomesKDClassifier.Value = RegionSettingsHelper.Instance.GetPropertyValue("IncomesKDClassifier");
        }

        private DataTable dtPopulation;

        private void InitializePopulation()
        {
            dtPopulation = new DataTable();
            string query = DataProvider.GetQueryText("FO_1035_0015_population_allMO");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtPopulation);
        }

        #region ������
        private DataTable dt;
        private DataTable newDt;

        private void InitializeIncomes()
        {
            chart3ElementCaption.Text = String.Format("������ �� {0} {1} {2}�.", month, CRHelper.RusManyMonthGenitive(month), year);

            dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_1035_0015_Incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            lbIncomesPlanValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["������� ����������"]) / 1000);
            lbIncomesFactValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["����"]) / 1000);
            lbIncomesExecutedValue.Text = String.Format("{0:N2}", Convert.ToDouble(dt.Rows[0]["% ���������� ������� ���������� "]) * 100);

            string img1 = String.Empty;
            if (dt.Rows[0]["���� �������"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["���� �������"]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["���� �������"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["���� �������"]) == worseRank)
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbIncomesRankFOValue.Text = String.Format("{0:N0}{1}&nbsp;", dt.Rows[0]["���� �������"], img1);

            lbIncomesRankFOValue.Style.Add("line-height", "40px");


            DataTable dtIncomesAllMo = new DataTable();
            query = DataProvider.GetQueryText("FO_1035_0015_Incomes_allMO");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomesAllMo);

            RankCalculator calc = new RankCalculator(RankDirection.Asc);
            for (int i = 0; i < dtPopulation.Rows.Count; i++)
            {
                if (dtPopulation.Rows[i][1] != DBNull.Value &&
                    dtIncomesAllMo.Rows[i][1] != DBNull.Value)
                {
                    calc.AddItem(Convert.ToDouble(dtIncomesAllMo.Rows[i][1]) / Convert.ToDouble(dtPopulation.Rows[i][1]));
                }
            }
            calc.ItemsToLog();
            img1 = String.Empty;
            if (calc.GetRank(Convert.ToDouble(dt.Rows[0]["����"]) / Convert.ToDouble(dt.Rows[0]["����������� ����������� ���������"].ToString())) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (calc.GetRank(Convert.ToDouble(dt.Rows[0]["����"]) / Convert.ToDouble(dt.Rows[0]["����������� ����������� ���������"].ToString())) == worseRank)
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbIncomesRankFOAverageValue.Text = String.Format("{0:N0}{1}&nbsp;", calc.GetRank(Convert.ToDouble(dt.Rows[0]["����"]) / Convert.ToDouble(dt.Rows[0]["����������� ����������� ���������"].ToString())), img1);

            double value;
            if (Double.TryParse(dt.Rows[0]["������������� ������"].ToString(), out value))
            {
                lbIncomesAverageValue.Text = String.Format("{0:N0}", value);
            }
            //if (Double.TryParse(dt.Rows[0]["����������� ����������� ���������"].ToString(), out value))
            //{
            //    lbPopulationValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["����������� ����������� ���������"]));
            //}
            lbIncomesRankFOAverageValue.Style.Add("line-height", "40px");

            UltraGauge gauge = GetGauge(Convert.ToDouble(dt.Rows[0]["% ���������� ������� ���������� "]) * 100);
            //  gauge.ToolTip = String.Format("������� ���������� ����������: {0:P2}", dt.Rows[0][2]);
            gauge.Style.Add("padding-left", "45px");
            gauge.Style.Add("margin-top", "-5px");
            GaugeIncomesPlaceHolder.Controls.Add(gauge);
        }

        #endregion

        #region �������

        private void InitializeOutcomes()
        {
            Label1.Text = String.Format("������� �� {0} {1} {2}�.", month, CRHelper.RusManyMonthGenitive(month), year);

            dt = new DataTable();
            string query = DataProvider.GetQueryText("FO_1035_0015_Outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            lbOutcomesPlanValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["������� ����������"]) / 1000);
            lbOutcomesFactValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["����"]) / 1000);
            lbOutcomesExecutedValue.Text = String.Format("{0:N2}", Convert.ToDouble(dt.Rows[0]["% ���������� ������� ���������� "]) * 100);

            string img1 = String.Empty;
            if (dt.Rows[0]["���� �������"] != DBNull.Value && Convert.ToInt32(dt.Rows[0]["���� �������"]) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (dt.Rows[0]["���� �������"] != DBNull.Value && dt.Rows[0]["������ ���� �������"] != DBNull.Value &&
                             Convert.ToInt32(dt.Rows[0]["���� �������"]) == worseRank)
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbOutcomesRankFOValue.Style.Add("line-height", "40px");

            lbOutcomesRankFOValue.Text = String.Format("{0:N0}{1}&nbsp;", dt.Rows[0]["���� �������"], img1);

            DataTable dtOutcomesAllMo = new DataTable();
            query = DataProvider.GetQueryText("FO_1035_0015_Outcomes_allMO");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtOutcomesAllMo);

            RankCalculator calc = new RankCalculator(RankDirection.Asc);
            for (int i = 0; i < dtPopulation.Rows.Count; i++)
            {
                if (dtPopulation.Rows[i][1] != DBNull.Value &&
                    dtOutcomesAllMo.Rows[i][1] != DBNull.Value)
                {
                    calc.AddItem(Convert.ToDouble(dtOutcomesAllMo.Rows[i][1]) / Convert.ToDouble(dtPopulation.Rows[i][1]));
                }
            }

            img1 = String.Empty;
            if (calc.GetRank(Convert.ToDouble(dt.Rows[0]["����"]) / Convert.ToDouble(dt.Rows[0]["����������� ����������� ���������"].ToString())) == 1)
            {
                img1 = string.Format("<img src=\"../../../images/starYellow.png\">");
            }
            else if (calc.GetRank(Convert.ToDouble(dt.Rows[0]["����"]) / Convert.ToDouble(dt.Rows[0]["����������� ����������� ���������"].ToString())) == worseRank)
            {
                img1 = string.Format("<img src=\"../../../images/starGray.png\">");
            }

            lbOutcomesRankFOAverageValue.Text = String.Format("{0:N0}{1}&nbsp;", calc.GetRank(Convert.ToDouble(dt.Rows[0]["����"]) / Convert.ToDouble(dt.Rows[0]["����������� ����������� ���������"].ToString())), img1);

            double value;
            if (double.TryParse(dt.Rows[0]["����������� ��������� �������������� ���������"].ToString(), out value))
            {
                lbOutcomesAverageValue.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0]["����������� ��������� �������������� ���������"]));
            }
            lbOutcomesRankFOAverageValue.Style.Add("line-height", "40px");

            UltraGauge gauge = GetGauge(Convert.ToDouble(dt.Rows[0]["% ���������� ������� ���������� "]) * 100);
            // gauge.ToolTip = String.Format("������� ���������� ����������: {0:P2}", dt.Rows[0][2]);
            gauge.Style.Add("padding-left", "15px");
            GaugeOutcomesPlaceHolder.Controls.Add(gauge);
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

        String text1 = string.Empty;
        String text2 = string.Empty;
        String text3 = string.Empty;

        private static void AddText(FillSceneGraphEventArgs e, string textValue, int offsetX)
        {
            Label lb = new Label();
            Text text = new Text(new Point(GetOffsetX(textValue), 37), textValue);
            LabelStyle style = new LabelStyle();

            style.Font = new Font("Arial", 12, FontStyle.Bold);
            //style.Font.Bold = true;
            style.FontColor = Color.White;
            text.SetLabelStyle(style);
            e.SceneGraph.Add(text);
        }

        private static int GetOffsetX(string text)
        {
            switch (text.Length)
            {
                case 3:
                    return 22;
                case 5:
                    return 17;
                case 6:
                    return 10;
                case 7:
                    return 4;
                default:
                    return 2;
            }
        }

        void UltraChart11_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, text1, 10);
        }

        void UltraChart12_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, text2, 2);
        }

        void UltraChart13_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            AddText(e, text3, 2);
        }

        private void ConfigureActionChart(UltraChart chart, bool crime, string ToolTip)
        {
            chart.Width = 75;
            chart.Height = 75;
            chart.ChartType = ChartType.PieChart;
            chart.Border.Thickness = 0;
            chart.Axis.Y.Extent = 10;
            chart.Axis.X.Extent = 10;
            chart.Axis.Y2.Extent = 10;
            chart.Axis.X2.Extent = 10;
            chart.Tooltips.FormatString = "<SERIES_LABEL>";
            chart.Legend.Visible = false;
            chart.Axis.X.Labels.SeriesLabels.Visible = false;
            chart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            chart.PieChart.Labels.Visible = false;
            chart.PieChart.Labels.LeaderLineThickness = 0;

            // chart.PieChart.RadiusFactor = 70;

            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            chart.ColorModel.Skin.PEs.Clear();

            Color color = Color.Gray;
            Color colorEnd = Color.Gray;
            PaintElement pe = new PaintElement();

            if (crime)
            {
                color = Color.Red;
                colorEnd = Color.Red;
            }
            else
            {
                color = Color.FromArgb(70, 118, 5);
                colorEnd = Color.FromArgb(70, 118, 5);
            }

            pe.Fill = color;
            pe.FillStopColor = colorEnd;
            pe.ElementType = PaintElementType.Gradient;
            pe.FillGradientStyle = GradientStyle.Horizontal;
            pe.FillOpacity = 150;
            chart.ColorModel.Skin.PEs.Add(pe);

            //chart.Style.Add("margin-top", " -10px");

            DataTable actionDataTable = new DataTable();
            actionDataTable.Columns.Add("name", typeof(string));
            actionDataTable.Columns.Add("value", typeof(double));
            object[] fictiveValue = { String.Format("<span style=' font-family: Verdana; font-size: 14px'>{0}</span>", ToolTip), 100 };
            actionDataTable.Rows.Add(fictiveValue);
            chart.DataSource = actionDataTable;
            chart.DataBind();
        }

        private void InitializeBkku()
        {
            UltraChart11.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart11_FillSceneGraph);
            UltraChart12.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart12_FillSceneGraph);
            UltraChart13.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart13_FillSceneGraph);

            DataTable dtInnerDebt = new DataTable();
            string query = DataProvider.GetQueryText("FO_1035_0015_Inner_Debt");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtInnerDebt);

            DataTable incomesWithoutBp = new DataTable();
            query = DataProvider.GetQueryText("FO_1035_0015_Incomes_without_BP");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", incomesWithoutBp);

            DataTable innerFinSource = new DataTable();
            query = DataProvider.GetQueryText("FO_1035_0015_inner_fin_source");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", innerFinSource);

            DataTable deficite = new DataTable();
            query = DataProvider.GetQueryText("FO_1035_0015_Deficite");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", deficite);

            DataTable outcomesDebt = new DataTable();
            query = DataProvider.GetQueryText("FO_1035_0015_Outcomes_debt");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", outcomesDebt);

            DataTable moGroup = new DataTable();
            query = DataProvider.GetQueryText("FO_1035_0015_MoGroup");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", moGroup);

            double innerDebtValue;
            Double.TryParse(dtInnerDebt.Rows[0][1].ToString(), out innerDebtValue);

            double incomesWithoutBpValue;
            Double.TryParse(incomesWithoutBp.Rows[0][1].ToString(), out incomesWithoutBpValue);

            double budjetCreditValue;
            Double.TryParse(innerFinSource.Rows[0][1].ToString(), out budjetCreditValue);

            double budjetCreditPlanValue;
            Double.TryParse(innerFinSource.Rows[0][4].ToString(), out budjetCreditPlanValue);

            double restsChangeValue;
            Double.TryParse(innerFinSource.Rows[0][2].ToString(), out restsChangeValue);

            double sellIncomesValue;
            Double.TryParse(innerFinSource.Rows[0][3].ToString(), out sellIncomesValue);

            double deficiteProficiteValue;
            Double.TryParse(deficite.Rows[0][1].ToString(), out deficiteProficiteValue);

            double bpValue;
            Double.TryParse(incomesWithoutBp.Rows[0][2].ToString(), out bpValue);

            double bpMoValue;
            Double.TryParse(incomesWithoutBp.Rows[0][3].ToString(), out bpMoValue);

            double outcomesServeValue;
            Double.TryParse(outcomesDebt.Rows[0][1].ToString(), out outcomesServeValue);

            double outcomesAllValue;
            Double.TryParse(outcomesDebt.Rows[0][2].ToString(), out outcomesAllValue);

            double moGroupValue;
            Double.TryParse(moGroup.Rows[0][0].ToString(), out moGroupValue);

            BindDebtIndicator(innerDebtValue, incomesWithoutBpValue, budjetCreditValue, moGroupValue);
            BindDeficiteIndicator(incomesWithoutBpValue, restsChangeValue, sellIncomesValue, deficiteProficiteValue, budjetCreditPlanValue, moGroupValue);
            BindServeIndicator(bpMoValue, outcomesServeValue, outcomesAllValue);
        }

        private void BindServeIndicator(double bpValue, double outcomesServeValue, double outcomesAllValue)
        {
            double outcomesWithoutBp = outcomesAllValue - bpValue;
            double limitServe = outcomesWithoutBp * 0.15;
            double indicator = outcomesServeValue > 0 ? outcomesServeValue / outcomesWithoutBp : 0;
            bool crime = limitServe - outcomesServeValue < 0;

            string serveDescription = outcomesServeValue == 0 ? String.Format("������� ������� �� �� ������������ �����<br/>�� ����� �� {0} ��� �� ��������� �� {1:dd.MM.yyyy} �. �����������", year, date) : String.Format("������� ������� �� �� ������������ �����<br/>�� ����� �� {0} ��� �� ��������� �� {1:dd.MM.yyyy} �.&nbsp;<b>{2:N2}</b>&nbsp;���.���.", year, date, outcomesServeValue / 1000);

            string debtOutcomesTooltip = String.Format("<b>���������� ����������� �������� �� ������������ �����</b><br/>��������� ������ �� ������������ �������,<br/>��� ����� ���������.<br/>������� ������� �� �� ������������<br/>����� �� ������ ��������� 15% �� ����� �����<br/>�������� (��� ����� �������� �� ����<br/>���������� ������ �� ������ ��������)<br/><br/>{0}<b>{1}</b><br/>{2}<br/>���������� ������ �������� �� ������������ �����:&nbsp;<b>{3:N2}</b> ���.���.",
                                                indicator > 0 ? String.Format("�������� ����������:&nbsp;<b>{0:P2}</b><br/>", indicator) : String.Empty, GetCrime(crime), serveDescription, limitServe / 1000);

            if (outcomesServeValue == 0)
            {
                lbDebtServe.Text = "&nbsp;";
                Label29.Text = "&nbsp;";
                text3 = "���";
            }
            else
            {
                lbDebtServe.Text = String.Format("{0:N0}", outcomesServeValue / 1000);
                text3 = String.Format("{0:P2}", indicator);
            }

            ConfigureActionChart(UltraChart13, crime, debtOutcomesTooltip);
        }

        private void BindDeficiteIndicator(double incomesWithoutBpValue, double restsChangeValue, double sellIncomesValue, double deficiteProficiteValue, double budjetCreditPlanValue, double moGroupValue)
        {
            double deficiteValue = deficiteProficiteValue > 0 ? 0 : deficiteProficiteValue * -1;
            double indicator = deficiteValue > 0 && incomesWithoutBpValue  != 0 ? (deficiteValue - restsChangeValue - sellIncomesValue - budjetCreditPlanValue) / incomesWithoutBpValue : 0;

            double deficiteCoeff = moGroupValue < 3 ? 0.1 : 0.05;

            // indicator = (deficiteValue - restsChangeValue - sellIncomesValue - budjetCreditPlanValue) < 0 ? 0 : indicator;
            double deficiteLimit = deficiteValue < 0 ? 0 : (incomesWithoutBpValue + restsChangeValue + sellIncomesValue + budjetCreditPlanValue) * deficiteCoeff;
            deficiteLimit = deficiteLimit > 0 ? deficiteLimit : 0;
            bool crime = !((moGroupValue < 3 && indicator < 0.1) ||
                         (moGroupValue == 3 && indicator < 0.05) ||
                         deficiteValue == 0);

            string deficiteDescription = String.Empty;

            if (deficiteProficiteValue == 0)
            {
                deficiteDescription = String.Format("������ �� {0} ��� ����������������<br/>���������� ������� (� ������ ��������� ��������<br/>� ����������� �� ������� �����):&nbsp;<b>{0:N2}</b>&nbsp;���.���.", deficiteLimit / 1000, year);
            }
            else if (deficiteProficiteValue > 0)
            {
                deficiteDescription = String.Format("�������� ������� ��<br/>�� ����� �� {0} ���:&nbsp;<b>{1:N2}</b>&nbsp;���.���.<br/>���������� ������� (� ������ ��������� ��������<br/>� ����������� �� ������� �����):&nbsp;<b>{2:N2}</b>&nbsp;���.���.", year, deficiteProficiteValue, deficiteLimit / 1000);
            }
            else
            {
                deficiteDescription =
                    String.Format(
                        "������� ������� ��<br/>�� ����� �� {0} ���:&nbsp;<b>{1:N2}</b>&nbsp;���.���.<br/>���������� �������:&nbsp;<b>{2:N2}</b>&nbsp;���.���.",
                        year, deficiteValue / 1000, deficiteLimit / 1000);
            }

            if (deficiteValue > 0)
            {
                lbDeficite.Text = String.Format("{0:N0}", deficiteValue / 1000);                
            }
            else
            {
                lbDeficite.Text = "&nbsp;";
                Label30.Text = "&nbsp;";
            }
            if (indicator <= 0)
            {                
                text2 = "���";
            }
            else
            {                
                text2 = String.Format("{0:P2}", indicator);
            }
            string deficiteTooltip = String.Format("<b>���������� ����������� �������� �������</b><br/>��������� ������ �� ������������ �������,<br/>��� ����� ���������.<br/>������ �� �� ���� ���&nbsp;<b>{3:N0}</b><br/>������� ������� �� (� ������ ��������� ��������<br/>� ����������� �� ������� �����)<br/>�� ����� ��������� 10% (��� �� ������ 3 �� ���� ��� 5%)<br/>������������� �������� ������ ������� ��� �����<br/>������������� �����������<br/><br/>{0}<b>{1}</b><br/>{2}",
                indicator > 0 ? String.Format("�������� ����������:&nbsp;<b>{0:P2}</b><br/>", indicator) : String.Empty, GetCrime(crime), deficiteDescription, moGroupValue);

            ConfigureActionChart(UltraChart12, crime, deficiteTooltip);
        }

        private static string GetCrime(bool crime)
        {
            return crime ? "��������� �� ��" : "��������� ���";
        }

        private void BindDebtIndicator(double innerDebtValue, double incomesWithoutBpValue, double budjetCreditValue, double moGroupValue)
        {
            double debtLimit = incomesWithoutBpValue > 0 ? incomesWithoutBpValue : 0;
            debtLimit = moGroupValue == 3 ? debtLimit * 0.5 : debtLimit;
            double debtIndicator = incomesWithoutBpValue > 0 ? (innerDebtValue) / incomesWithoutBpValue : 0;

            bool crime = !((moGroupValue < 3 && debtIndicator < 1) ||
                         (moGroupValue == 3 && debtIndicator < 0.5) ||
                         innerDebtValue == 0);

            string debtDescription = innerDebtValue == 0 ? "������������� ����&nbsp;<b>�����������</b>" : String.Format("����� ����� �� {0}:&nbsp;<b>{1:N2}</b>&nbsp;���.���.", String.Format("�� {0} ������� {1} ����", month, year), innerDebtValue / 1000);

            string debtTooltip = String.Format("<b>���������� ����������� �� ����� �����,<br/>�������������� �� ��</b><br/>��������� ������ �� ������������ �������,<br/>��� ����� ���������.<br/>������ �� �� ���� ���&nbsp;<b>{4:N0}</b><br/>����� ����� �� �� ����� ��������� ������������<br/>������� ����� ������� ��� ����� �������������<br/>�����������<br/>(��� �� 3 ������ �� ���� ��� �� ����� 50% �� �������)<br/><br/>�������� ����������:&nbsp;<b>{0:P2}</b><br/><b>{1}</b><br/>{2}<br/>���������� ��������<br/>������ ����� ��:&nbsp;<b>{3:N2}</b>&nbsp;���.���.",
                                                debtIndicator, GetCrime(crime), debtDescription, debtLimit / 1000, moGroupValue);

            if (innerDebtValue == 0)
            {
                lbDebts.Text = "&nbsp;";
                Label31.Text = "&nbsp;";
                text1 = "���";
            }
            else
            {
                lbDebts.Text = String.Format("{0:N0}", innerDebtValue / 1000);
                text1 = debtIndicator == 0 ? "���" : String.Format("{0:P2}", debtIndicator);
            }
            ConfigureActionChart(UltraChart11, crime, debtTooltip);
        }

        #endregion

        #region ������

        private int monthNum = 1;

        private void InitializeBudget()
        {
            UltraWebGridBudget.Width = Unit.Empty;
            UltraWebGridBudget.Height = Unit.Empty;

            UltraWebGridBudget.DataBind();
            UltraWebGridBudget.Style.Add("margin-top", "-4px");
        }

        protected void UltraWebGridBudget_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();

            string query = DataProvider.GetQueryText("FO_1035_0015_budget_incomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "����������", dt);

            DataTable source = new DataTable();

            query = DataProvider.GetQueryText("FO_1035_0015_budget_outcomes_all");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "����������", source);

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
            query = DataProvider.GetQueryText("FO_1035_0015_budget_outcomes");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "����������", source);

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
            query = DataProvider.GetQueryText("FO_1035_0015_budget_deficite");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "����������", source);

            foreach (DataRow row in source.Rows)
            {
                DataRow newRow = dt.NewRow();
                for (int i = 0; i < source.Columns.Count; i++)
                {
                    newRow[i] = row[i];
                }
                dt.Rows.Add(newRow);
            }

            dt.Columns.RemoveAt(5);
            dt.Columns.RemoveAt(3);
            UltraWebGridBudget.Style.Add("margin-right", "-10px");
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

            e.Layout.Bands[0].Columns[0].Width = 171;
            e.Layout.Bands[0].Columns[1].Width = 81;
            e.Layout.Bands[0].Columns[2].Width = 81;
            e.Layout.Bands[0].Columns[3].Width = 70;

            for (int i = 1; i <= e.Layout.Bands[0].Columns.Count - 1; i++)
            {
                e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("16px");
                //e.Layout.Bands[0].Columns[i].CellStyle.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = false;

            }
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].HeaderStyle.Height = 10;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("�� {0} ���. {1}�. ���.���.", month, year - 1);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("�� {0} ���. {1}�. ���.���.", month, year);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");
            e.Layout.Bands[0].Columns[3].Header.Caption = "���� ����� %";

            e.Layout.RowStyleDefault.Padding.Top = 1;
            e.Layout.RowStyleDefault.Padding.Bottom = 1;
            e.Layout.RowStyleDefault.Padding.Left = 1;
            e.Layout.RowStyleDefault.Padding.Right = 1;
        }

        protected void UltraWebGridBudget_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString().ToLower() != "����� ������� " &&
                e.Row.Cells[0].Value.ToString().ToLower() != "����� �������� " &&
               !e.Row.Cells[0].Value.ToString().ToLower().Contains("��������(+)/"))
            {
                e.Row.Cells[0].Style.Padding.Left = 10;
            }
            if (e.Row.Cells[3] != null &&
                e.Row.Cells[3].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[3].Value.ToString());
                string img;
                string title;
                if (value > 100)
                {
                    img = "~/images/arrowGreenUpBB.png";
                    title = "���� � �������� ����";
                }
                else
                {
                    img = "~/images/arrowRedDownBB.png";
                    title = "������� � �������� ����";
                }
                e.Row.Cells[3].Style.BackgroundImage = img;
                e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: 6px center; padding-top: 2px";
                //   e.Row.Cells[3].Title = title;
            }
            e.Row.Cells[1].Style.Padding.Right = 3;
            e.Row.Cells[2].Style.Padding.Right = 3;
            e.Row.Cells[3].Style.Padding.Right = 1;

            if (e.Row.Cells[0].Value.ToString().ToLower().Contains("��������(+)/") && e.Row.Cells[0].Value.ToString().ToLower().Contains("�������(-)"))
            {
                if (e.Row.Cells[1] != null &&
                e.Row.Cells[1].Value != null)
                {
                    double value = Convert.ToDouble(e.Row.Cells[1].Value.ToString());
                    string img;
                    string title;
                    if (value > 0)
                    {
                        img = "~/images/CornerGreen.gif";
                        title = "��������";
                    }
                    else
                    {
                        img = "~/images/CornerRed.gif";
                        title = "�������";
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
                    string title;
                    if (value > 0)
                    {
                        img = "~/images/CornerGreen.gif";
                        title = "��������";
                    }
                    else
                    {
                        img = "~/images/CornerRed.gif";
                        title = "�������";
                    }
                    e.Row.Cells[2].Style.BackgroundImage = img;
                    e.Row.Cells[2].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; padding-top: 3px; padding-right: 4px";
                    //  e.Row.Cells[2].Title = title;
                }
            }
        }

        #endregion

        #region ��������
        private DataTable dtGrid;

        private bool fns28nSplitting;
        private int month;
        private int year;

        #region ��������� �������

        // ������� �� � ��

        // ���
        private CustomParam cubeName;
        // ����� ��������� ��������
        private CustomParam topRegionsCount;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ������������� ���������� �������

            if (cubeName == null)
            {
                cubeName = UserParams.CustomParam("cube_name");
            }
            if (topRegionsCount == null)
            {
                topRegionsCount = UserParams.CustomParam("top_regions_count");
            }

            #endregion
        }


        #region ������ ��������� �������� DataTable

        private static string ParseDTValue(DataRow row, int columnIndex)
        {
            if (row == null || row[columnIndex] == DBNull.Value)
            {
                return string.Empty;
            }

            return row[columnIndex].ToString();
        }

        private static string ParseDoubleDTValue(DataTable dt, int rowIndex, int columnIndex, string format)
        {
            if (dt == null || dt.Rows[rowIndex][columnIndex] == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToDouble(dt.Rows[rowIndex][columnIndex]).ToString(format);
        }

        private static string ParseDoubleDTValue(DataRow row, int columnIndex, string format)
        {
            if (row == null || row[columnIndex] == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToDouble(row[columnIndex]).ToString(format);
        }

        #endregion

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0001_0001_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            month = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0].ToString());

            UserParams.PeriodLastYear.Value = string.Format("[{0}].[��������� 2].[������� 4].[�������]", year - 1);
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(month));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(month));
            UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, CRHelper.RusMonth(month));

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            dtGrid = new DataTable();
            string queryName = "FNS_0001_0001";
            query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtGrid);
            UltraWebGrid.Width = Unit.Empty;
            UltraWebGrid.DataSource = dtGrid;
            //UltraWebGrid.Style.Add("margin-left", "-6px");
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;

            e.Layout.Bands[0].Columns[0].Width = 114;
            e.Layout.Bands[0].Columns[1].Width = 120;
            e.Layout.Bands[0].Columns[3].Width = 115;

            e.Layout.Bands[0].Columns[2].Hidden = true;
            e.Layout.Bands[0].Columns[4].Hidden = true;
            e.Layout.Bands[0].Columns[5].Hidden = true;
            e.Layout.Bands[0].Columns[6].Hidden = true;

            int reportYear = year;
            int reportMonth = month + 1;

            if (reportMonth == 13)
            {
                reportMonth = 1;
                reportYear += 1;
            }

            string dateMonth = string.Format("�������� �� 01.{0:00}.{1} ���.���.", reportMonth, reportYear);

            e.Layout.Bands[0].Columns[0].Header.Caption = dateMonth;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[0], "N0");
            e.Layout.Bands[0].Columns[1].Header.Caption = e.Layout.Bands[0].Columns[1].Header.Caption.Split(';')[0];
            e.Layout.Bands[0].Columns[3].Header.Caption = e.Layout.Bands[0].Columns[3].Header.Caption.Split(';')[0];

            UltraWebGrid.Style.Add("margin-left", "-3px");
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            int rowIndex = 0;
            int percentColumnIndex = 2;
            int indincateColumnIndex = 1;
            int rankColumnIndex = 4;

            if (dtGrid.Rows[rowIndex][percentColumnIndex] != DBNull.Value &&
                 dtGrid.Rows[rowIndex][percentColumnIndex].ToString() != string.Empty)
            {
                double value = Convert.ToDouble(dtGrid.Rows[rowIndex][percentColumnIndex]);
                e.Row.Cells[indincateColumnIndex].Style.BackgroundImage = (value > 0)
                                                           ? "../../../images/arrowRedUpBB.png"
                                                           : "../../../images/arrowGreenDownBB.png";
                e.Row.Cells[indincateColumnIndex].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: 20px center; padding-top: 2px";
                e.Row.Cells[indincateColumnIndex].Value =
                    String.Format("{0:N0}<br/>{1:P2}", e.Row.Cells[indincateColumnIndex].Value,
                                  e.Row.Cells[percentColumnIndex].Value);
            }

            if (dtGrid.Rows[rowIndex][rankColumnIndex] != DBNull.Value &&
                 dtGrid.Rows[rowIndex][rankColumnIndex].ToString() != string.Empty)
            {
                double worseRank = Convert.ToDouble(dtGrid.Rows[rowIndex][6]);
                double value = worseRank - Convert.ToDouble(dtGrid.Rows[rowIndex][rankColumnIndex]) + 1;
                if (value == 1)
                {
                    e.Row.Cells[3].Style.BackgroundImage = "../../../images/starYellow.png";
                }
                else
                {
                    if (value == worseRank)
                    {
                        e.Row.Cells[3].Style.BackgroundImage = "../../../images/starGray.png";
                    }
                }
                e.Row.Cells[3].Value = String.Format("{0:P2}<br>���� {1}", e.Row.Cells[3].Value, value);

                e.Row.Cells[indincateColumnIndex].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: 20px center; padding-top: 2px";
                e.Row.Cells[3].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: 20px bottom; padding-top: 2px";
            }
        }
        #endregion

        #endregion

        #region ������ ���������


        void UltraWebGridSettlements_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            if (e.Layout.Bands.Count == 0)
                return;

            e.Layout.Bands[0].Columns[0].Width = 80;
            e.Layout.Bands[0].Columns[1].Width = 91;
            e.Layout.Bands[0].Columns[2].Width = 90;
            e.Layout.Bands[0].Columns[3].Width = 88;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            e.Layout.Bands[0].Columns[1].Header.Caption = String.Format("�� {0} ���. {1}�. ���.���.", month, year - 1);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            e.Layout.Bands[0].Columns[2].Header.Caption = String.Format("�� {0} ���. {1}�. ���.���.", month, year);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");
            e.Layout.Bands[0].Columns[3].Header.Caption = "���� ����� %";

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].HeaderStyle.Height = 10;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");

        }

        void UltraWebGridSettlements_DataBinding(object sender, EventArgs e)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_1035_0015_incomes_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            month = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0].ToString());

            UserParams.PeriodLastYear.Value = string.Format("[{0}].[��������� 2].[������� 4].[�������]", year - 1);
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(month));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(month));
            UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, CRHelper.RusMonth(month));

            monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);

            DataTable dtIncomes = new DataTable();
            query = DataProvider.GetQueryText("FO_1035_0015_Incomes_settlement");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);

            dtDate = new DataTable();
            query = DataProvider.GetQueryText("FO_1035_0015_outcomes_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            month = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            year = Convert.ToInt32(dtDate.Rows[0][0].ToString());

            UserParams.PeriodLastYear.Value = string.Format("[{0}].[��������� 2].[������� 4].[�������]", year - 1);
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(month));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(month));
            UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, CRHelper.RusMonth(month));


            DataTable dtOutcomes = new DataTable();
            query = DataProvider.GetQueryText("FO_1035_0015_Outcomes_settlement");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtOutcomes);

            dt = new DataTable();

            dt.Columns.Add(" ", typeof(string));
            dt.Columns.Add("���� ���.���.", typeof(double));
            dt.Columns.Add("���� ���.���.", typeof(double));
            dt.Columns.Add("% ���", typeof(double));

            DataRow row = dt.NewRow();
            row[0] = "������";
            if (dtIncomes.Rows[0][0] != DBNull.Value)
            {
                row[1] = Convert.ToDouble(dtIncomes.Rows[0][0]);
            }
            if (dtIncomes.Rows[0][1] != DBNull.Value)
            {
                row[2] = Convert.ToDouble(dtIncomes.Rows[0][1]);
            }
            if (dtIncomes.Rows[0][2] != DBNull.Value)
            {
                row[3] = Convert.ToDouble(dtIncomes.Rows[0][2]);
            }
            dt.Rows.Add(row);

            row = dt.NewRow();
            row[0] = "�������";
            if (dtOutcomes.Rows[0][0] != DBNull.Value)
            {
                row[1] = Convert.ToDouble(dtOutcomes.Rows[0][0]);
            }
            if (dtOutcomes.Rows[0][1] != DBNull.Value)
            {
                row[2] = Convert.ToDouble(dtOutcomes.Rows[0][1]);
            }
            if (dtOutcomes.Rows[0][2] != DBNull.Value)
            {
                row[3] = Convert.ToDouble(dtOutcomes.Rows[0][2]);
            }
            dt.Rows.Add(row);

            UltraWebGridSettlements.DataSource = dt;
        }

        #endregion

        private void InitializeSettlement()
        {
            DataTable dtSettlementsCount = new DataTable();
            string query = DataProvider.GetQueryText("FO_1035_0015_settlements_count");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dtSettlementsCount);

            Label6.Text = String.Format("������� ��������� ({0})", dtSettlementsCount.Rows[0][0]);

            UltraWebGridSettlements.Width = Unit.Empty;
            UltraWebGridSettlements.Height = Unit.Empty;

            UltraWebGridSettlements.DataBinding += new EventHandler(UltraWebGridSettlements_DataBinding);
            UltraWebGridSettlements.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGridSettlements_InitializeLayout);
            UltraWebGridSettlements.InitializeRow += new InitializeRowEventHandler(UltraWebGridSettlements_InitializeRow);
            UltraWebGridSettlements.DataBind();

            InitializeDebtSettlement();
            InitializeFnsSettlement();
        }

        void UltraWebGridSettlements_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[1].Style.CustomRules = "padding-right: 4px";
            e.Row.Cells[2].Style.CustomRules = "padding-right: 4px";
            e.Row.Cells[3].Style.CustomRules = "padding-right: 4px";

            if (e.Row.Cells[3] != null &&
                e.Row.Cells[3].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[3].Value.ToString());
                string img;
                string title;
                if (value > 100)
                {
                    img = "~/images/arrowGreenUpBB.png";
                    title = "���� � �������� ����";
                }
                else
                {
                    img = "~/images/arrowRedDownBB.png";
                    title = "������� � �������� ����";
                }
                e.Row.Cells[3].Style.BackgroundImage = img;
                e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: 2px center; padding-top: 2px; padding-right: 2px";
                //   e.Row.Cells[3].Title = title;
            }
        }

        private void InitializeDebtSettlement()
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0001_0001_debt_settlement");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            settlementDebts.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0][0]));
        }

        private void InitializeFnsSettlement()
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0001_0001_settlement");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            //settlementFns.Text = String.Format("{0:N0}", Convert.ToDouble(dt.Rows[0][0]));
        }
    }

    public class RankCalculator
    {
        private Collection<double> itemCollection;
        private RankDirection rankDireciton;

        public RankCalculator(RankDirection direciton)
        {
            rankDireciton = direciton;
            itemCollection = new Collection<double>();
        }

        public void AddItem(double item)
        {
            // ������������� ���������
            if (itemCollection.Contains(item))
            {
                return;
            }

            // ��������� � ��������������� ��������� ����� � ������ �����
            int i = 0;
            while (i < itemCollection.Count &&
                (rankDireciton == RankDirection.Asc && itemCollection[i] > item ||
                 rankDireciton == RankDirection.Desc && itemCollection[i] < item))
            {
                i++;
            }

            itemCollection.Insert(i, item);
        }

        public int GetRank(double value)
        {
            for (int i = 0; i < itemCollection.Count; i++)
            {
                if (IsDoubleEquals(itemCollection[i], value))
                {
                    return i + 1;
                }
            }

            return 0;
        }

        public int GetWorseRank()
        {
            return itemCollection.Count;
        }

        public void ItemsToLog()
        {
            for (int i = 0; i < itemCollection.Count; i++)
            {
                CRHelper.SaveToErrorLog(String.Format("{0} - {1}", i, itemCollection[i]));
            }
        }

        private bool IsDoubleEquals(double value1, double value2)
        {
            return Math.Round(value1 * 10000) == Math.Round(value2 * 10000);
        }
    }

    public enum RankDirection
    {
        Asc,
        Desc
    }
}