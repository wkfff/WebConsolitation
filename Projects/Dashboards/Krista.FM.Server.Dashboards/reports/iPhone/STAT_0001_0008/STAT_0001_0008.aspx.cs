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
    public partial class STAT_0001_0008 : CustomReportPage
    {
        // ������� ����
        private DateTime currDateTime;
        private CustomParam periodCurrentDate;
        // ���������� ����
        private DateTime lastDateTime;
        private CustomParam periodLastDate;
        // �����
        private DateTime fdMonthDateTime;
        private CustomParam fdMonth;
        // �����
        private DateTime fdYearDateTime;
        private CustomParam fdYear;
        // �� ������ ����� ��� �������������
        private CustomParam debtsPeriodLastWeekDate;
        private DateTime debtsLastDateTime;
        // ������� ���� ��� �������������
        private CustomParam debtsPeriodCurrentDate;
        private DateTime debtsCurrDateTime;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            #region ������������� ���������� �������

            periodCurrentDate = UserParams.CustomParam("period_current_date");
            periodLastDate = UserParams.CustomParam("period_last_date");
            fdMonth = UserParams.CustomParam("fd_month");
            fdYear = UserParams.CustomParam("fd_year");
            debtsPeriodCurrentDate = UserParams.CustomParam("period_current_date_debts");
            debtsPeriodLastWeekDate = UserParams.CustomParam("period_last_week_date_debts");

            #endregion

            UltraChart3.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart3_FillSceneGraph);
            UltraChart3.DataBinding += new EventHandler(UltraChart3_DataBinding);
            UltraChart3.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart3_ChartDrawItem);

            #region ����

            DataTable dtDateCur = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0008_part1_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "dummy", dtDateCur);
            periodCurrentDate.Value = dtDateCur.Rows[1][1].ToString();
            currDateTime = CRHelper.DateByPeriodMemberUName(periodCurrentDate.Value, 3);
            periodLastDate.Value = dtDateCur.Rows[0][1].ToString();
            lastDateTime = CRHelper.DateByPeriodMemberUName(periodLastDate.Value, 3);
            fdMonthDateTime = currDateTime.AddMonths(-1);
            fdMonth.Value = CRHelper.PeriodMemberUName("[������__��� ������� �����].[������__��� ������� �����].[������ ���� ��������]", fdMonthDateTime, 4);

            #endregion
            
            CommentText1DataBind();
            LbDebtDescriptionDataBind();
            lbDescription.Text = GetDescritionText();

            UltraChart3.Width = 340;
            UltraChart3.Legend.SpanPercentage = 33;
            UltraChart3.Height = 225;
            SetUpPieChart(UltraChart3);
            AddColorModelChart2(UltraChart3);

            UltraChart3.DataBind();

            BindTagCloud();
        }

        private void LbDebtDescriptionDataBind()
        {
            string query = DataProvider.GetQueryText("STAT_0001_0008_debts_mo_date");
            DataTable dtDebtsDateNso = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtDebtsDateNso);

            if (dtDebtsDateNso.Rows.Count > 1)
            {
                if (dtDebtsDateNso.Rows[0][1] != DBNull.Value && dtDebtsDateNso.Rows[0][1].ToString() != string.Empty)
                {
                    debtsPeriodLastWeekDate.Value = dtDebtsDateNso.Rows[0][1].ToString();
                    debtsLastDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDateNso.Rows[0][1].ToString(), 3);
                }
                if (dtDebtsDateNso.Rows[1][1] != DBNull.Value && dtDebtsDateNso.Rows[1][1].ToString() != string.Empty)
                {
                    debtsPeriodCurrentDate.Value = dtDebtsDateNso.Rows[1][1].ToString();
                    debtsCurrDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDateNso.Rows[1][1].ToString(), 3);
                }
            }

            debtsPeriodCurrentDate.Value = dtDebtsDateNso.Rows[1][1].ToString();
            debtsPeriodLastWeekDate.Value = dtDebtsDateNso.Rows[0][1].ToString();

            query = DataProvider.GetQueryText("STAT_0001_0008_debts_Yamal");
            DataTable dtDebtsNso = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtDebtsNso);

            double totalDebts = GetDoubleDTValue(dtDebtsNso, "C���� �������������");
            double totalLastWeekDebts = GetDoubleDTValue(dtDebtsNso, "C���� ������������� ���������� �����");
            string dateTimeDebtsStr = string.Format("{0:dd.MM.yyyy}", debtsCurrDateTime);
            string dateLastTimeDebtsStr = string.Format("{0:dd.MM.yyyy}", debtsLastDateTime);
            double slavesCount = GetDoubleDTValue(dtDebtsNso, "���������� �������, ������� �������������");
            double debtsPercent = GetDoubleDTValue(dtDebtsNso, "������� �������������");
            string debtsPercentArrow = debtsPercent == 0
                                           ? "�� ����������"
                                           : debtsPercent > 0
                                           ? string.Format("�����������&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'>&nbsp;��&nbsp;<b>{0:N3}</b>&nbsp;���.���", Math.Abs(debtsPercent))
                                           : string.Format("�����������&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'>&nbsp;��&nbsp;<b>{0:N3}</b>&nbsp;���.���", Math.Abs(debtsPercent));

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
&nbsp;<span class='DigitsValue'><b>{1:N3}</b></span>&nbsp;���.���. (<span class='DigitsValue'><b>{2:N0}</b></span>&nbsp;���.).<br/>�� ������ �&nbsp;<span class='DigitsValue'><b>{5}</b></span>&nbsp;��&nbsp;<span class='DigitsValue'><b>{0}</b></span>&nbsp;������������� {3}",
dateTimeDebtsStr, totalDebts, slavesCount, debtsPercentArrow, "����-����", dateLastTimeDebtsStr);
            }
            string debtsMoList = String.Empty;

            if (dtDebtsDateNso.Rows.Count > 0)
            {

                query = DataProvider.GetQueryText("STAT_0001_0008_debts_mo");
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
        
        private void CommentText1DataBind()
        {
            DataTable dtTextYamal = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0008_Text_Yamal");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Dummy", dtTextYamal);
            DataTable dtFederalData = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0008_FD");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtFederalData);
            DataTable dtCitizen = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0008_FD_Citizen");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtCitizen);

            #region ������� �����������

            double yamalValue = Convert.ToDouble(dtTextYamal.Rows[0]["������� �����������"]) / 100;
            string str0 = String.Format("��&nbsp;<span class='DigitsValue'><b>{0:dd.MM.yyyy}</b></span>&nbsp;������� �������������� ����������� " +
                "� ����&nbsp;<span class='DigitsValueXLarge'><b>{1:P3}</b></span><br/>", currDateTime, yamalValue);
            string str1, str2 = str1 = String.Empty;
            double urfoValue = Convert.ToDouble(dtFederalData.Rows[0]["��������� ����������� �����"]);
            if (yamalValue < urfoValue)
                {
                    str1 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;����&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp; ��� � ����&nbsp;<span class='DigitsValue'><b>{0:P3}</b></span><br/>", urfoValue);
                }
            else if (yamalValue > urfoValue)
            {
                str1 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;����&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp; ��� � ����&nbsp;<span class='DigitsValue'><b>{0:P3}</b></span><br/>", urfoValue);
            }
            else
            {
                str1 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;�������������&nbsp;<img src='../../../images/ballYellowBB.png'>&nbsp; ������ ����&nbsp;<span class='DigitsValue'><b>{0:P3}</b></span><br/>", urfoValue);
            }
            double rfValue = Convert.ToDouble(dtFederalData.Rows[0]["����������  ���������"]);
            if (yamalValue < rfValue)
                {
                    str2 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;����&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp; ��� � ��&nbsp;<span class='DigitsValue'><b>{0:P3}</b></span>", rfValue);
                }
            else if (yamalValue > rfValue)
            {
                str2 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;����&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp; ��� � ��&nbsp;<span class='DigitsValue'><b>{0:P3}</b></span>", rfValue);
            }
            else
            {
                str2 = String.Format("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;�������������&nbsp;<img src='../../../images/ballYellowBB.png'>&nbsp; ������ ��&nbsp;<span class='DigitsValue'><b>{0:P3}</b></span>", rfValue);
            }

            str2 += "<div style='height: 7px; clear: both'></div>";

            #endregion

            #region ����������� �����������

            string str3 = String.Format("����������� ���������� ������������������ �����������&nbsp;<span class='DigitsValueXLarge'><b>{0:N0}</b></span>&nbsp;�������<br/>", dtTextYamal.Rows[0]["����������� ����������� "]);
            string str4 = String.Format(
                "�� ������ �&nbsp;<span class='DigitsValue'><b>{0:dd.MM}</b></span>&nbsp;��&nbsp;<span class='DigitsValue'><b>{1:dd.MM}</b></span>&nbsp;����������� ���������� ������������������ �����������<br/>",
                lastDateTime, currDateTime);
            if (MathHelper.AboveZero(dtTextYamal.Rows[1]["����������� ����������� "]))
            {
                str4 += String.Format("�����������&nbsp;<img src='../../../images/arrowRedUpBB.png' width='13px' height='16px'> ��&nbsp;<span class='DigitsValue'><b>{0:N0}</b></span>&nbsp;���. (���� �����&nbsp;<span class='DigitsValue'><b>{1:P2}</b></span>)<br/>",
                    dtTextYamal.Rows[1]["����������� ����������� "], dtTextYamal.Rows[2]["����������� ����������� "]);
            }
            else if (MathHelper.SubZero(dtTextYamal.Rows[1]["����������� ����������� "]))
            {
                str4 += String.Format("�����������&nbsp;<img src='../../../images/arrowGreenDownBB.png' width='13px' height='16px'> ��&nbsp;<span class='DigitsValue'><b>{0:N0}</b></span>&nbsp;���. (���� �����&nbsp;<span class='DigitsValue'><b>{1:P2}</b></span>)<br/>",
                    MathHelper.Abs(dtTextYamal.Rows[1]["����������� ����������� "]), dtTextYamal.Rows[2]["����������� ����������� "]);
            }
            else
            {
                str4 += "�� ����������<br/>";
            }
            
            #endregion

            #region ��������

            string str6 = String.Format("����������� � ����������, ���������� �������������� � ������ ������ ��������� ���������&nbsp;<span class='DigitsValueXLarge'><b>{0:N0}</b></span>&nbsp;��������<br/>",
                dtTextYamal.Rows[0]["����������� ��������"]);

            #endregion

            #region ����������� ������������� �� ����� �����

            string str7 = String.Format("������� ������������� �� ����� �����&nbsp;<span class='DigitsValueXLarge'><b>{0:N2}</b></span><br/><div style='height: 7px; clear: both'></div>",
                dtTextYamal.Rows[0]["����������� �������������"]);

            #endregion
            /*
            #region ����������� ����������� �� 1 ��������

            string str8 = String.Format("����������� ����������� �� 1 ��������&nbsp;<span class='DigitsValueXLarge'><b>{0:N2}</b></span><div style='height: 7px; clear: both'></div>",
                dtTextYamal.Rows[0]["����� ����������� �� 1 ��������"]);

            #endregion
            */
            #region ������������ �������� ���������

            string str9 = String.Format("������������ �������� ���������&nbsp;<span class='DigitsValueXLarge'><b>{0:N0}</b></span>&nbsp;�������<br/>",
                dtFederalData.Rows[1]["�����-�������� ���������� �����"]);

            #endregion

            #region ����������� ���������

            string str10 = String.Format("����������� ��������� ���� ��&nbsp;<span class='DigitsValue'><b>01.01.{0}</b></span>&nbsp;���������� &nbsp;<span class='DigitsValueXLarge'><b>{1:N0}</b></span>&nbsp; �������<div style='height: 7px; clear: both'></div>",
                dtCitizen.Rows[0]["���"], dtCitizen.Rows[0]["����"]);

            #endregion

            CommentText1.Text = String.Format("{0}{1}{2}{3}{4}", str0, str1, str2, str3, str4);
            CommentText11.Text = String.Format("{0}{1}{2}{3}", str6, str7, str9, str10);
        }

        private string GetDescritionText()
        {
            DataTable dtText = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0008_description_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "�����", dtText);
            fdMonth.Value = dtText.Rows[0]["������"].ToString();
            fdMonthDateTime = CRHelper.DateByPeriodMemberUName(fdMonth.Value, 3);
            dtText = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0008_description");
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
                   "�&nbsp;<span class='DigitsValue'><b>{0} {1} ����</b></span>&nbsp;������� ����� �����������<br/>�� ����������� ��� � ����&nbsp;<span class='DigitsValueXLarge'><b>{2:P2}</b></span><br/>{3}", CRHelper.RusMonthPrepositional(fdMonthDateTime.Month), fdMonthDateTime.Year, thisMot, description);
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
                description += String.Format("�� {1} {2} ���� ����� �����������<br/>{0}{5} ��&nbsp;<span class='DigitsValue'><b>{3:N0}</b></span>&nbsp;������� (���� �����&nbsp;<b><span class='DigitsValue'>{4:P2}</span></b>)<div style='height: 7px; clear: both'></div>", grown, CRHelper.RusMonth(fdMonthDateTime.Month), fdMonthDateTime.Year, Math.Abs(grownValue), grownTemp, GetImage(grown));
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
                case "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;����":
                    return "&nbsp;<img src='../../../images/ballRedBB.png'>&nbsp;";
                default:
                    return "&nbsp;<img src='../../../images/ballGreenBB.png'>&nbsp;";
            }
        }

        #region ��������� 3

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
            string query = DataProvider.GetQueryText(String.Format("STAT_0001_0008_chart3"));
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Rows.Count < 2)
            {
                UltraChart3.DataSource = null;
                return;
            }

            double total = Convert.ToDouble(dtChart.Rows[0][1]) +
                            Convert.ToDouble(dtChart.Rows[1][1]);

            dtChart.Rows[0][0] = String.Format("{0} {1:N0} ���. ({2:P3})", dtChart.Rows[0][0], dtChart.Rows[0][1], Convert.ToDouble(dtChart.Rows[0][1]) / total);
            dtChart.Rows[1][0] = String.Format("{0} {1:N0} ���.  ({2:P3})", dtChart.Rows[1][0], dtChart.Rows[1][1], Convert.ToDouble(dtChart.Rows[1][1]) / total);

            dtChart.Rows[1][1] = Convert.ToDouble(dtChart.Rows[1][1]) * 7;

            UltraChart3.DataSource = dtChart;
        }

        #endregion

        private void BindTagCloud()
        {
            // ������� �������� ���� � ����� �� ���� ���.
            string query = DataProvider.GetQueryText("STAT_0001_0008_part1_date");
            DataTable dtDateNso = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����������", dtDateNso);

            DateTime currDateTimeNso = CRHelper.DateByPeriodMemberUName(dtDateNso.Rows[1][1].ToString(), 3);
            DateTime lastDateTimeNso = CRHelper.DateByPeriodMemberUName(dtDateNso.Rows[0][1].ToString(), 3);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", currDateTimeNso, 5);
            periodLastDate.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", lastDateTimeNso, 5);

            DataTable dtTagCloud = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0008_tagCloud_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "������������ ����������", dtTagCloud);

            Dictionary<string, Tag> tags = new Dictionary<string, Tag>();
            Collection<Color> colors = new Collection<Color>();

            foreach (DataRow row in dtTagCloud.Rows)
            {
                if (row["������� �����������"] != DBNull.Value &&
                    row["������� �����������"].ToString() != String.Empty)
                {
                    Tag tag = new Tag();
                    tag.weight = Convert.ToInt32(row["������� �����������"]);
                    string rankImage = String.Empty;
                    if (row["���� "].ToString() == "1")
                    {
                        rankImage = "&nbsp;<img src='../../../images/StarGray.png'>";
                    }
                    else if (row["���� "].ToString() == row["������ ���� "].ToString())
                    {
                        rankImage = "&nbsp;<img src='../../../images/StarYellow.png'>";
                    }

                    tag.key = String.Format("{0}{2}&nbsp;({1:N3}%)", row["������� ������������ "], row["������� �����������"], rankImage);
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
            chart.DoughnutChart.Labels.FormatString = "<PERCENT_VALUE:N2>%";
            chart.DoughnutChart.OthersCategoryPercent = 0;
            
            chart.Legend.Visible = true;
            chart.Legend.Location = LegendLocation.Bottom;
            chart.Legend.Font = new Font("Verdana", 9);
            chart.DoughnutChart.StartAngle = 340;
            
            chart.Data.SwapRowsAndColumns = true;
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
