using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections.Generic;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class Food_0005_0001_h : CustomReportPage
    {

        private CustomParam lastDateParam;
        private CustomParam prevDateParam;
        private CustomParam yearDateParam;
        private CustomParam rfYearDateParam;
        private CustomParam yearParam;

        private DateTime lastDate;
        private DateTime prevDate;
        private DateTime yearDate;
        private DateTime rfYearDate;
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            #region ����

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Food_0005_0001_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtDate);

            if (dtDate.Rows.Count == 0)
            {
                return;
            }

            lastDateParam = UserParams.CustomParam("last_date");
            prevDateParam = UserParams.CustomParam("prev_date");
            yearParam = UserParams.CustomParam("year");
            yearDateParam = UserParams.CustomParam("year_date");
            rfYearDateParam = UserParams.CustomParam("rf_year_date");
            
            lastDateParam.Value = dtDate.Rows[0]["����"].ToString(); ;
            lastDate = CRHelper.DateByPeriodMemberUName(lastDateParam.Value, 3);

            if (dtDate.Rows.Count == 1)
            {
                prevDate = lastDate.AddDays(-7);
                prevDateParam.Value = CRHelper.PeriodMemberUName("[������__����].[������__����].[������ ���� ��������]", prevDate, 5);
            }
            else
            {
                prevDateParam.Value = dtDate.Rows[1]["����"].ToString();
                prevDate = CRHelper.DateByPeriodMemberUName(prevDateParam.Value, 3);
            }

            yearParam.Value = CRHelper.PeriodMemberUName("[������__����].[������__����].[������ ���� ��������]", new DateTime(lastDate.Year, 1, 1), 1);
            dtDate = new DataTable();
            query = DataProvider.GetQueryText("Food_0005_0001_h_year");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtDate);
            if (dtDate.Rows.Count == 1)
            {
                yearDateParam.Value = dtDate.Rows[0]["����"].ToString();
                yearDate = CRHelper.DateByPeriodMemberUName(yearDateParam.Value, 3);
            }
            else
            {
                yearDate = new DateTime(lastDate.Year, 1, 1);
                yearDateParam.Value = CRHelper.PeriodMemberUName("[������__����].[������__����].[������ ���� ��������]", yearDate, 5);
            }

            yearParam.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", new DateTime(lastDate.Year, 1, 1), 1);
            dtDate = new DataTable();
            query = DataProvider.GetQueryText("Food_0005_0001_h_year_rf");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����", dtDate);
            if (dtDate.Rows.Count == 1)
            {
                rfYearDateParam.Value = dtDate.Rows[0]["����"].ToString();
                rfYearDate = CRHelper.DateByPeriodMemberUName(rfYearDateParam.Value, 3);
            }
            else
            {
                rfYearDate = new DateTime(lastDate.Year, 1, 1);
                rfYearDateParam.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", yearDate, 5);
            }

            #endregion

            Label.Text = "<table Class=HtmlTable width=580px>";

            //Label.Text += FillData(lastDate, prevDate, prevDate, "�� ������");
            //Label.Text += FillData(lastDate, yearDate, rfYearDate, "� ������ ����");

            Label.Text += FillData();

            Label.Text += "</table>";

        }

        protected string FillData()
        {
            string result = String.Format("<tr><td colspan=7 style=\"padding-left: 0px; border-color: black;\">��������� ���� �� �������� ������� ��&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span></td></tr>",
                lastDate);

            DataTable dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("Food_0005_0001_h_grid_hmao");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "������������", dtGrid);

            if (dtGrid.Rows.Count == 0)
                return result;

            lastDateParam.Value = lastDateParam.Value.Replace("[������__����]", "[������__������]");
            prevDateParam.Value = prevDateParam.Value.Replace("[������__����]", "[������__������]");
            yearDateParam.Value = yearDateParam.Value.Replace("[������__����]", "[������__������]");

            DataTable dtGridRf = new DataTable();
            query = DataProvider.GetQueryText("Food_0005_0001_h_grid_rf");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "������������", dtGridRf);

            Dictionary<string, string> rfFoods = new Dictionary<string, string>(dtGridRf.Rows.Count);
            foreach (DataRow rfGridRow in dtGridRf.Rows)
            {
                rfFoods.Add(rfGridRow["������������"].ToString(), String.Format("{0:N2};{1:N2};{2:P2};{3:N2};{4:P2}",
                    rfGridRow["���� �� ��������� ����"], rfGridRow["�������"], rfGridRow["���� ��������"], rfGridRow["������� � ������ ����"], rfGridRow["���� �������� � ������ ����"]));
            }

            result += "<tr><th Class=HtmlTableHeader rowspan=\"2\" style=\"width: 200px; border-color: black;\">�������</th>";
            result += String.Format("<th Class=HtmlTableHeader colspan=2 style=\"width: 120px; border-color: black;\">���� �� {0:dd.MM.yyyy}</th>", lastDate);
            result += String.Format("<th Class=HtmlTableHeader colspan=2 style=\"width: 130px; border-color: black;\">�������� �� ������</th>");
            result += String.Format("<th Class=HtmlTableHeader colspan=2 style=\"width: 130px; border-color: black;\">�������� � ������ ����</th></tr>");
            result += "<tr><th Class=HtmlTableHeader style=\"width: 60px; border-color: black;\">����</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 60px; border-color: black;\">��</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 65px; border-color: black;\">����</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 65px; border-color: black;\">��</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 65px; border-color: black;\">����</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 65px; border-color: black;\">��</th></tr>";

            string imgHmaoPrevDate = String.Empty, imgHmaoYearDate = String.Empty, imgRfPrevDate = String.Empty, imgRfYearDate = String.Empty;

            foreach (DataRow row in dtGrid.Rows)
            {
                string food = ConvertFoodName(row["������������"].ToString());
                string unit = row["������� ���������"].ToString().Replace("���������", "��.").Replace("����", "�.");

                string rfCost = "-";
                string rfPrevDate = "-<br/>-";
                string rfYearDate = "-<br/>-";
                string rfInfo = String.Empty;
                if (rfFoods.TryGetValue(food, out rfInfo))
                {
                    string[] rfInfoSplit = rfInfo.Split(';');
                    rfCost = rfInfoSplit[0];
                    if (MathHelper.IsDouble(rfInfoSplit[1]))
                    {
                        rfPrevDate = String.Format("{0}<br/>{1}", rfInfoSplit[1], rfInfoSplit[2]);
                        imgRfPrevDate = GetImage(rfInfoSplit[1]);
                    }
                    if (MathHelper.IsDouble(rfInfoSplit[3]))
                    {
                        rfYearDate = String.Format("{0}<br/>{1}", rfInfoSplit[3], rfInfoSplit[4]);
                        imgRfYearDate = GetImage(rfInfoSplit[3]);
                    }
                }

                imgHmaoPrevDate = GetImage(row["�������"].ToString());
                imgHmaoYearDate = GetImage(row["������� � ������ ����"].ToString());

                result += "<tr>";

                result += String.Format("<td>{0}, ���./{1}</td>", food, unit);
                result += String.Format("<td style=\"text-align: right;\">{0:N2}</td>", row["���� �� ��������� ����"]);
                result += String.Format("<td style=\"text-align: right;\">{0}</td>", rfCost);
                result += String.Format("<td style=\"text-align: right;{2}\">{0:N2}<br/>{1:P2}</td>", row["�������"], row["���� ��������"], imgHmaoPrevDate);
                result += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", rfPrevDate, imgRfPrevDate);
                result += String.Format("<td style=\"text-align: right;{2}\">{0:N2}<br/>{1:P2}</td>", row["������� � ������ ����"], row["���� �������� � ������ ����"], imgHmaoYearDate);
                result += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", rfYearDate, imgRfYearDate);

                result += "</tr>";
            }

            return result;
        }

        protected string GetImage(string value)
        {
            if (!MathHelper.IsDouble(value))
                return String.Empty;
            else if (MathHelper.AboveZero(value))
                return " background-image: url(../../../images/arrowRedUpBB.png); background-repeat: no-repeat; background-position: 5% center;";
            else if (MathHelper.SubZero(value))
                return " background-image: url(../../../images/arrowGreenDownBB.png); background-repeat: no-repeat; background-position: 5% center;";
            else
                return String.Empty;
        }

        /// <summary>
        /// ����������� �������������� � ������������
        /// </summary>
        /// <param name="food">������������ �������� � ��������������</param>
        /// <returns>������������ ���� �� �������� � ������������ ��������������</returns>
        private string ConvertFoodName(string food)
        {
            return food.
                Replace("���� ���������� �������������", "���� ������������ �������������").
                Replace("(2,5-3,2 % ����.)", "2,5-3,2% ��������").
                Replace("������� �������, 1 ����", "������� ������� 1 �����").
                Replace("����� ��������� - ������", "����� ���������-������").
                Replace("���� (����� �������� ���������)", "���� (����� ������� ���������)").
                Replace("���������� �������", "���������� ������� �� ��������� ���� ������� �����").
                Replace("���� ��������� (�/�, 1 ����)", "���� ���������").
                Replace("���� �������", "���� �������� ������� � ������").
                Replace("����� ��������� 40% ��.������  � ���� ������������� ��������", "����� ��������� 40% ��.������ � ���� ������������� ��������").
                Replace("���� � �������� ������� �� ���� ��������� 1 � 2 ������", "���� � �������� ������� �� ��������� ���� 1 � 2 ������");
        }
    }
}
