using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class STAT_0001_0003 : CustomReportPage
    {
        private DateTime currDateTime;
        private DateTime lastDateTime;
        // ������� ����
        private CustomParam periodCurrentDate;
        // �� ������ �����
        private CustomParam periodLastWeekDate;
        // ������� ���� ��� ������ ����������� �� ��
        private CustomParam redundantLevelRFDate;
        private DateTime redundantLevelRFDateTime;
        // �� ������ ����� ��� �������������
        private CustomParam debtsPeriodLastWeekDate;
        private DateTime debtsLastDateTime;
        // ������� ���� ��� �������������
        private CustomParam debtsPeriodCurrentDate;
        private DateTime debtsCurrDateTime;



        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            CustomParams.MakeMoParams(UserParams.Mo.Value, "id");

            #region ������������� ���������� �������

            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (debtsPeriodCurrentDate == null)
            {
                debtsPeriodCurrentDate = UserParams.CustomParam("period_current_date_debts");
            }
            if (debtsPeriodLastWeekDate == null)
            {
                debtsPeriodLastWeekDate = UserParams.CustomParam("period_last_week_date_debts");
            }
            if (periodLastWeekDate == null)
            {
                periodLastWeekDate = UserParams.CustomParam("period_last_week_date");
            }
            if (redundantLevelRFDate == null)
            {
                redundantLevelRFDate = UserParams.CustomParam("redundant_level_RF_date");
            }

            #endregion

            UltraChart3.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart3_FillSceneGraph);
            UltraChart3.DataBinding += new EventHandler(UltraChart3_DataBinding);
            UltraChart3.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart3_ChartDrawItem);

            DataTable dtDateCur = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0001_chart1_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtDateCur);
            periodCurrentDate.Value = dtDateCur.Rows[1][1].ToString();

            UltraChart1.Width = 340;
            UltraChart1.Height = 310;
            UltraChart1.DataBinding += UltraChart1_DataBinding;
            SetUpPieChart(UltraChart1);
            UltraChart1.Legend.SpanPercentage = 52;
            AddColorModelChart1(UltraChart1);
            UltraChart1.DataBind();
            
            CommentTextDataBind();
            BindTagCloud();

            DataTable dtDate = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0012_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            UserParams.PeriodYear.Value = dtDate.Rows[0][4].ToString();

            UltraChart3.Width = 340;
            UltraChart3.Legend.SpanPercentage = 33;
            UltraChart3.Height = 225;
            SetUpPieChart(UltraChart3);
            AddColorModelChart2(UltraChart3);

            UltraChart3.DataBind();

            lbDescription.Text = GetDescritionText(CRHelper.PeriodDayFoDate(UserParams.PeriodYear.Value));
        }

        private string GetDescritionText(DateTime date)
        {
            DataTable dtText = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0012_text");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtText);

            double thisMot; //2
            double ufoMot;  //3
            double rfMot;   //4

            string description = string.Empty;

            if (Double.TryParse(dtText.Rows[2][1].ToString(), out thisMot) &&
               Double.TryParse(dtText.Rows[3][1].ToString(), out ufoMot))
            {
                double grownValue = ufoMot - thisMot;
                string grown;
                string middleLevel = String.Format(" ��� � ����&nbsp;<b><span class='DigitsValue'>{0:P2}</span></b>", ufoMot); ;
                if (grownValue < 0)
                {
                    grown = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;����";
                }
                else if (grownValue > 0)
                {
                    grown = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;����";
                }
                else
                {
                    grown = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;�������������";
                    middleLevel = "�������� ������ ����� ����������� �� ����";
                }

                description =
                    String.Format(
                        "{0}{1} {2}<br/> ", grown, GetImage(grown), middleLevel);
                description = String.Format(
                   "�&nbsp;<span class='DigitsValue'><b>{0} {1} ����</b></span>&nbsp;������� ����� �����������<br/>�� ����������� ��� � ����-����&nbsp;<span class='DigitsValueXLarge'><b>{2:P2}</b></span><br/>{3}", CRHelper.RusMonthPrepositional(date.Month), date.Year, thisMot, description);
            }

            string rfDescription;
            if (Double.TryParse(dtText.Rows[2][1].ToString(), out thisMot) &&
               Double.TryParse(dtText.Rows[4][1].ToString(), out rfMot))
            {
                double grownValue = rfMot - thisMot;
                string grown;
                string middleLevel = String.Format(" ��� � ��&nbsp;<b><span class='DigitsValue'>{0:P2}</span></b><br/>", rfMot); ;
                if (grownValue < 0)
                {
                    grown = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;����";
                }
                else if (grownValue > 0)
                {
                    grown = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;����";
                }
                else
                {
                    grown = "�������������";
                    middleLevel = "�������� ������ ����� ����������� �� ��";
                }

                rfDescription =
                    String.Format(
                        "{0}{1} {2}", grown, GetImage(grown), middleLevel);
                
            }
            else
            {
                rfDescription = "";
            }

            description = String.Format("{0}{1}<div style='height: 7px; clear: both'></div>", description, rfDescription);

            double thisMonth;
            double prevMonth;
            if (Double.TryParse(dtText.Rows[1][1].ToString(), out thisMonth) &&
                Double.TryParse(dtText.Rows[1][2].ToString(), out prevMonth))
            {
                double grownValue = prevMonth - thisMonth;
                string grown = grownValue < 0 ? "�����������" : "���������";
                // string compile = grownValue < 0 ? "��������" : "���������";
                double grownTemp = thisMonth / prevMonth;
                description += String.Format("�� {1} {2} ���� ����� �����������<br/>{0}{5} ��&nbsp;<span class='DigitsValue'><b>{3:N0}</b></span>&nbsp;������� (���� �����&nbsp;<b><span class='DigitsValue'>{4:P2}</span></b>)<div style='height: 7px; clear: both'></div>", grown, CRHelper.RusMonth(date.Month), date.Year, Math.Abs(grownValue), grownTemp, GetImage(grown));
            }
            description += String.Format("����������� �����������&nbsp;<span class='DigitsValue'><b>{0:N0}</b></span>&nbsp;�������", dtText.Rows[1][1]);

            return description;
        }

        private static string GetImage(string direction)
        {
            switch (direction.ToLower())
            {
                case "�����������":
                    return "&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'/>&nbsp;";
                case "���������":
                    return "&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'/>&nbsp;";
                case "����":
                    return "&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp;";
                default:
                    return "&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;";
            }
        }

        void UltraChart3_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //������������� ������ ������ ������� 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((UltraChart3.Legend.Location == LegendLocation.Top) || (UltraChart3.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChart3.Width.Value - 20;
                }
                else
                {
                    widthLegendLabel = (UltraChart3.Legend.SpanPercentage * (int)UltraChart3.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChart3.Legend.Margins.Left + UltraChart3.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }
        }

        void UltraChart3_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if ((primitive is Box))
                {
                    Box box = (Box) primitive;
                    if (box.DataPoint != null)
                    {
                        box.DataPoint.Label = String.Format("{0}\n{1}", "��������� ����������� �����",
                                                            box.DataPoint.Label);
                    }
                }
            }
        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0012_chart3"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            double total = Convert.ToDouble(dtChart.Rows[0][1]) +
                            Convert.ToDouble(dtChart.Rows[1][1]);

            dtChart.Rows[0][0] = String.Format("{0} {1:N0} ���. ({2:P2})", dtChart.Rows[0][0], dtChart.Rows[0][1], Convert.ToDouble(dtChart.Rows[0][1]) / total);
            dtChart.Rows[1][0] = String.Format("{0} {1:N0} ���.  ({2:P2})", dtChart.Rows[1][0], dtChart.Rows[1][1], Convert.ToDouble(dtChart.Rows[1][1]) / total);

            dtChart.Rows[1][1] = Convert.ToDouble(dtChart.Rows[1][1]) * 7;

            UltraChart3.DataSource = dtChart;
        }


        void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            DataTable dtChart = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0001_chart1");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(
                query, "������������ ����������", dtChart);

            double total = Convert.ToDouble(dtChart.Rows[0][1]) + 
                            Convert.ToDouble(dtChart.Rows[1][1]) +
                            Convert.ToDouble(dtChart.Rows[2][1]);

            dtChart.Rows[0][0] = String.Format("������� ��������� {0:N0} ���. ({1:P2})", dtChart.Rows[0][1], Convert.ToDouble(dtChart.Rows[0][1])/total);
            dtChart.Rows[1][0] = String.Format("��������� ��� �������\n����������� {0:N0} ��� ({1:P2})", dtChart.Rows[1][1], Convert.ToDouble(dtChart.Rows[1][1]) / total);
            dtChart.Rows[2][0] = String.Format("����������� {0:N0} ��� ({1:P2})", dtChart.Rows[2][1], Convert.ToDouble(dtChart.Rows[2][1]) / total);

            dtChart.Rows[1][1] = Convert.ToDouble(dtChart.Rows[1][1]) * 10;
            dtChart.Rows[2][1] = Convert.ToDouble(dtChart.Rows[2][1]) * 10;

            UltraChart1.DataSource = dtChart;
        }

        private void BindTagCloud()
        {
            // ������� �������� ���� � ����� �� ���� ����.
            string query = DataProvider.GetQueryText("STAT_0001_0001_chart1_date");
            DataTable dtDateHmao = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����������", dtDateHmao);

            DateTime currDateTimeHmao = CRHelper.DateByPeriodMemberUName(dtDateHmao.Rows[1][1].ToString(), 3);
            DateTime lastDateTimeHmao = CRHelper.DateByPeriodMemberUName(dtDateHmao.Rows[0][1].ToString(), 3);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", currDateTimeHmao, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", lastDateTimeHmao, 5);

            DataTable dtTagCloud = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0003_tagCloud_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(
                query, "������������ ����������", dtTagCloud);

            Dictionary<string, Tag> tags = new Dictionary<string, Tag>();
            Collection<Color> colors = new Collection<Color>();

            foreach (DataRow row in dtTagCloud.Rows)
            {
                if (row["������� �����������"] != DBNull.Value &&
                    row["������� �����������"].ToString() != String.Empty)
                {
                    Tag tag = new Tag();
                    tag.weight = Convert.ToInt32(row["����������� ��������� "]);
                    string rankImage = String.Empty;
                    if (row["���� "].ToString() == "1")
                    {
                        rankImage = "&nbsp;<img src='../../../images/StarGray.png'>";
                    }
                    else if (row["���� "].ToString() == row["������ ���� "].ToString())
                    {
                        rankImage = "&nbsp;<img src='../../../images/StarYellow.png'>";
                    }

                    tag.key = String.Format("{0}{2}&nbsp;({1:P2})", row["������� ������������ "], row["������� �����������"], rankImage);
                    tag.toolTip = String.Empty;
                    tags.Add(tag.key, tag);
                }

                colors.Add(GetTagColor(row["�������"].ToString()));
            }
            CloudTag1.ForeColors = colors;
            CloudTag1.Render(tags);
        }

        private Color GetTagColor (string grown)
        {
            double value = Convert.ToDouble(grown);
            if (value <= 0)
            {
                return Color.Green;
            }
            return Color.Red;
        }

        private void SetUpPieChart(UltraChart chart)
        {
            chart.ChartType = ChartType.StackBarChart;

            chart.StackChart.StackStyle = StackStyle.Complete;

            chart.Border.Thickness = 0;
            chart.Axis.Y.Extent = 5;
            chart.Axis.X.Extent = 5;

            chart.Axis.X.Labels.SeriesLabels.Visible = false;
            chart.Axis.X.Labels.Visible = false;
            chart.Axis.Y.Labels.Visible = false;
            chart.Axis.Y.LineColor = Color.Black;
            chart.Axis.X.LineColor = Color.Black;
           
            chart.InvalidDataReceived += CRHelper.UltraChartInvalidDataReceived;
            
            chart.BackColor = Color.Transparent;
            
            chart.DoughnutChart.RadiusFactor = 70;
            chart.DoughnutChart.InnerRadius = 20;
            chart.Tooltips.FormatString = "";
            chart.DoughnutChart.Labels.FormatString = "<PERCENT_VALUE:N2>%";
            chart.DoughnutChart.OthersCategoryPercent = 0;
            chart.Tooltips.FormatString = "<ITEM_LABEL>\n<DATA_VALUE:N0> ������� (<PERCENT_VALUE:N2>%)";
            
            chart.Legend.Visible = true;
            chart.Legend.Location = LegendLocation.Bottom;
            chart.Legend.Font = new Font("Verdana", 9);
            chart.DoughnutChart.StartAngle = 340;
            
            chart.Data.SwapRowsAndColumns = true;
        }

        private void AddColorModelChart1(UltraChart chart)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 3; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetStopColor(i);

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                pe.Stroke = Color.FromArgb(40, 40, 40);
                chart.ColorModel.Skin.PEs.Add(pe);
            }
            chart.ColorModel.Skin.ApplyRowWise = false;
        }

        private void AddColorModelChart2(UltraChart chart)
        {
            chart.ColorModel.ModelStyle = ColorModels.CustomSkin;
            chart.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 2; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Green;
                            stopColor = Color.ForestGreen;
                            break;
                        }
                    case 2:
                        {
                            color = Color.Red;
                            stopColor = Color.Red;
                            break;
                        }
                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                pe.Stroke = Color.FromArgb(40, 40, 40);
                chart.ColorModel.Skin.PEs.Add(pe);
            }
            chart.ColorModel.Skin.ApplyRowWise = false;
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.Green;
                    }
                case 2:
                    {
                        return Color.Gold;
                    }
                case 3:
                    {
                        return Color.Red;
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
                        return Color.ForestGreen;
                    }
                case 2:
                    {
                        return Color.Yellow;
                    }
                case 3:
                    {
                        return Color.Red;
                    }
            }
            return Color.White;
        }


        private void CommentTextDataBind()
        {
            // ������� �������� ���� � ����� �� ���� ����.
            string query = DataProvider.GetQueryText("STAT_0001_0001_chart1_date");
            DataTable dtDateHmao = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����������", dtDateHmao);

            DateTime currDateTimeHmao = CRHelper.DateByPeriodMemberUName(dtDateHmao.Rows[1][1].ToString(), 3);
            DateTime lastDateTimeHmao = CRHelper.DateByPeriodMemberUName(dtDateHmao.Rows[0][1].ToString(), 3);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", currDateTimeHmao, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", lastDateTimeHmao, 5);

            query = DataProvider.GetQueryText("STAT_0001_0001_Text_Hmao");
            DataTable dtCommentTextHmao = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����������", dtCommentTextHmao);

            // ����������� �������� ���� � 2000 ���
            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[������].[������].[������ ���� ��������]", currDateTimeHmao, 5);

            // ������� ������ �� ����������� ����
            DataTable dtDateCur = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateCur);

            currDateTime = CRHelper.DateByPeriodMemberUName(dtDateCur.Rows[0][5].ToString(), 3);
            lastDateTime = currDateTime.AddDays(-7);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[������].[������].[������ ���� ��������]", currDateTime, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[������].[������].[������ ���� ��������]", lastDateTime, 5);

            query = DataProvider.GetQueryText("STAT_0001_0002_debts_date");
            DataTable dtDebtsDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����", dtDebtsDate);

            query = DataProvider.GetQueryText("STAT_0001_0002_redundantLevelRF_date");
            DataTable dtRedundantLevelRFDate = new DataTable();

            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����", dtRedundantLevelRFDate);
            redundantLevelRFDate.Value = dtRedundantLevelRFDate.Rows[0][1].ToString();
            redundantLevelRFDateTime = CRHelper.DateByPeriodMemberUName(dtRedundantLevelRFDate.Rows[0][1].ToString(), 3);

            if (dtDebtsDate.Rows.Count > 1)
            {
                if (dtDebtsDate.Rows[0][1] != DBNull.Value && dtDebtsDate.Rows[0][1].ToString() != string.Empty)
                {
                    debtsPeriodLastWeekDate.Value = dtDebtsDate.Rows[0][1].ToString();
                    debtsLastDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[0][1].ToString(), 3);
                }
                if (dtDebtsDate.Rows[1][1] != DBNull.Value && dtDebtsDate.Rows[1][1].ToString() != string.Empty)
                {
                    debtsPeriodCurrentDate.Value = dtDebtsDate.Rows[1][1].ToString();
                    debtsCurrDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[1][1].ToString(), 3);
                }
            }

            query = DataProvider.GetQueryText("STAT_0001_0002_commentText");
            DataTable dtCommentText = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����������", dtCommentText);

            if (dtCommentText.Rows.Count > 0)
            {
                string dateTimeStr = string.Format("&nbsp;{0:dd.MM.yyyy}&nbsp;", currDateTimeHmao);
                double totalCount = Convert.ToDouble(dtCommentTextHmao.Rows[1][1]);
                double totalRate = 1 + Convert.ToDouble(dtCommentTextHmao.Rows[1][3]);
                double totalGrow = Convert.ToDouble(dtCommentTextHmao.Rows[1][2]);
                string totalRateArrow = totalRate > 1
                                               ? "�����������&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'>&nbsp;"
                                               : "���������&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'>&nbsp;";
                //string totalRateStr = totalRate > 0 ? "��������" : "���������";
                double unemploued = Convert.ToDouble(dtCommentTextHmao.Rows[2][1]);

                double redundantlevelValue = Convert.ToDouble(dtCommentTextHmao.Rows[3][1]);

                double redundantLevelRFValue = GetDoubleDTValue(dtCommentText, "������� �������������� ����������� ��");

                string redundantLevelRFArrow;
                string redundantLevelRFDescription = String.Format("&nbsp;<span class='DigitsValue'><b>{0:N2}%</b></span>", redundantLevelRFValue);
                if (redundantlevelValue > redundantLevelRFValue)
                {
                    redundantLevelRFArrow = "����&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp;";
                }
                else if (redundantlevelValue < redundantLevelRFValue)
                {
                    redundantLevelRFArrow = "����&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;";
                }
                else
                {
                    redundantLevelRFArrow = "�������������&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;������";
                    redundantLevelRFDescription = String.Empty;
                }
                string redundantLevelRFGrow = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0}��� � ��{1}<br/>", redundantLevelRFArrow, redundantLevelRFDescription);

                double redundantLevelUrfoValue = GetDoubleDTValue(dtCommentText, "������� �������������� ����������� ����");
                string redundantLevelUrfoArrow;
                string redundantLevelUrfoDescription = String.Format("&nbsp;<span class='DigitsValue'><b>{0:N2}%</b></span>", redundantLevelUrfoValue);
                if (redundantlevelValue > redundantLevelUrfoValue)
                {
                    redundantLevelUrfoArrow = "����&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp;";
                }
                else if (redundantlevelValue < redundantLevelUrfoValue)
                {
                    redundantLevelUrfoArrow = "����&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;";
                }
                else
                {
                    redundantLevelUrfoArrow = "�������������&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;������";
                    redundantLevelUrfoDescription = String.Empty;
                }
                string redundantLevelUrfoGrow = String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0}��� � ����{1}<br/>", redundantLevelUrfoArrow, redundantLevelUrfoDescription);

                redundantLevelRFGrow = String.Format("{0}{1}", redundantLevelUrfoGrow, redundantLevelRFGrow);

                double vacancyCount = GetDoubleDTValue(dtCommentText, "����������� � ����������");

                double tensionKoeff = GetDoubleDTValue(dtCommentText, "����� ������������������ ����������� � ������� �� 1 ��������", double.MinValue);

                string str1 =
                    string.Format(
                        @"��&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;������� �������������� � ������� ������ ��������� ����������� � ����-����&nbsp;<span class='DigitsValueXLarge'><b>{1:P2}</b></span> {2} <div style='height: 7px; clear: both'></div>",
                        dateTimeStr, redundantlevelValue, redundantLevelRFGrow);

                string str2 = string.Format(@"����������� �����������&nbsp;<span class='DigitsValueXLarge'><b>{0:N0}</b></span>&nbsp;�������<br>", totalCount);

                string str3 = string.Format(@"�� ������ �&nbsp;<span class='DigitsValue'><b>{1:dd.MM}</b></span>&nbsp;��&nbsp;<span class='DigitsValue'><b>{2:dd.MM}</b></span>&nbsp;����� ����������� {0} ��&nbsp;<span class='DigitsValue'><b>{3:N0}</b></span>&nbsp;���. (���� �����&nbsp;<span class='DigitsValue'><b>{4:P2}</b></span>)<br/>",
                        totalRateArrow, lastDateTimeHmao, currDateTimeHmao, Math.Abs(totalGrow), totalRate);

                string str5 = string.Format(@"��������� ��� ������� ����������� ��&nbsp;<span class='DigitsValue'>{1:01.MM.yyyy}</span>&nbsp;����������&nbsp;<span class='DigitsValueXLarge'><b>{0:N0}</b></span>&nbsp;�������<div style='height: 7px; clear: both'></div>", unemploued, currDateTimeHmao);

                string str6 = string.Format(@"����������� � ����������, ���������� �������������� � ������ ������ ��������� ���������&nbsp;<span class='DigitsValueXLarge'><b>{0:N0}</b></span>&nbsp;��������<br/>", dtCommentTextHmao.Rows[5][1]);

                string str7 = string.Format(@"����������� ������������� �� ����� �����&nbsp;<span class='DigitsValueXLarge'><b>{0:N2}</b></span><br/>", dtCommentTextHmao.Rows[6][1]);

                string str8 = string.Format(@"����������� ����������� �� 1 ��������&nbsp;<span class='DigitsValueXLarge'><b>{0:N1}</b></span>", dtCommentTextHmao.Rows[7][1]);

                DataTable dtPopulation = new DataTable();
                query = DataProvider.GetQueryText("STAT_0001_0012_population");
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtPopulation);

                string str9 = String.Format("<div style='height: 7px; clear: both'></div>������������ �������� ���������&nbsp;<span class='DigitsValue'><b>{0:N0}</b></span>&nbsp;�������<br/>����������� ��������� ����-����&nbsp;<span class='DigitsValue'><b>{1:N0}</b></span>&nbsp;�������",
                                               dtCommentTextHmao.Rows[4][1], dtPopulation.Rows[0][1]);

                CommentText1.Text = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", str1, str2, str3, str5, str6, str7, str8, str9);

                query = DataProvider.GetQueryText("STAT_0001_0012_debts_mo_date");
                DataTable dtDebtsDateHmao = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtDebtsDateHmao);

                if (dtDebtsDateHmao.Rows.Count > 1)
                {
                    if (dtDebtsDateHmao.Rows[0][1] != DBNull.Value && dtDebtsDateHmao.Rows[0][1].ToString() != string.Empty)
                    {
                        debtsPeriodLastWeekDate.Value = dtDebtsDateHmao.Rows[0][1].ToString();
                        debtsLastDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDateHmao.Rows[0][1].ToString(), 3);
                    }
                    if (dtDebtsDateHmao.Rows[1][1] != DBNull.Value && dtDebtsDateHmao.Rows[1][1].ToString() != string.Empty)
                    {
                        debtsPeriodCurrentDate.Value = dtDebtsDateHmao.Rows[1][1].ToString();
                        debtsCurrDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDateHmao.Rows[1][1].ToString(), 3);
                    }
                }

                debtsPeriodCurrentDate.Value = dtDebtsDateHmao.Rows[1][1].ToString();
                debtsPeriodLastWeekDate.Value = dtDebtsDateHmao.Rows[0][1].ToString();

                query = DataProvider.GetQueryText("STAT_0001_0012_debts_Hmao");
                DataTable dtDebtsHmao = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtDebtsHmao);

                double totalDebts = GetDoubleDTValue(dtDebtsHmao, "C���� �������������");
                double totalLastWeekDebts = GetDoubleDTValue(dtDebtsHmao, "C���� ������������� ������� ������");
                string dateTimeDebtsStr = string.Format("{0:dd.MM.yyyy}", debtsCurrDateTime);
                string dateLastTimeDebtsStr = string.Format("{0:dd.MM.yyyy}", debtsLastDateTime);
                double slavesCount = GetDoubleDTValue(dtDebtsHmao, "���������� �������, ������� �������������");
                double debtsPercent = GetDoubleDTValue(dtDebtsHmao, "������� �������������");
                string debtsPercentArrow = debtsPercent == 0
                                               ? "�� ����������"
                                               : debtsPercent > 0
                                               ? string.Format("����������� <img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'> �� <b>{0:N3}</b>&nbsp;���.���", Math.Abs(debtsPercent))
                                               : string.Format("����������� <img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'> �� <b>{0:N3}</b>&nbsp;���.���", Math.Abs(debtsPercent));

                string str10;
                if (totalLastWeekDebts == 0 && totalDebts == 0)
                {
                    str10 = string.Format(@"��&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;����������� ������������� �� ������� 
���������� �����<br/>", dateTimeDebtsStr, "����-����");
                }
                else if (totalDebts == 0)
                {
                    str10 = string.Format(@"��&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;����������� ������������� �� ������� ���������� �����. 
������������� � �����&nbsp;<span class='DigitsValue'><b>{2:N3}</b></span>&nbsp;���.���. ���� �������� �� ������ �&nbsp;<span class='DigitsValue'><b>{3}</b></span>&nbsp;��&nbsp;<span class='DigitsValue'><b>{0}</b></span>.<br/>",
dateTimeDebtsStr, "����-����", totalLastWeekDebts, dateLastTimeDebtsStr);
                }
                else
                {
                    str10 = string.Format(@"��&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;������������� �� ������� ���������� ����� ���������� 
&nbsp;<span class='DigitsValue'><b>{1:N3}</b></span>&nbsp;���.������ (<span class='DigitsValue'><b>{2:N0}</b></span>&nbsp;���.).<br/>�� ������ �&nbsp;<span class='DigitsValue'><b>{5}</b></span>&nbsp;��&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;������������� {3}",
dateTimeDebtsStr, totalDebts, slavesCount, debtsPercentArrow, "����-����", dateLastTimeDebtsStr);
                }
                string debtsMoList = String.Empty;

                if (dtDebtsDateHmao.Rows.Count > 0)
                {
                    
                    query = DataProvider.GetQueryText("STAT_0001_0012_debts_mo");
                    DataTable dtDebtsMo = new DataTable();
                    DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����������", dtDebtsMo);
                    if (dtDebtsMo.Rows.Count > 0)
                    {
                        debtsMoList =
                            String.Format("������������� �� ������� ���������� ����� ������������ �&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;������������� ������������", dtDebtsMo.Rows.Count);
                    }
                }
                
                lbDebtDescription.Text = String.Format("{0} {1}", str10, debtsMoList);
            }
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

       

    }
}
