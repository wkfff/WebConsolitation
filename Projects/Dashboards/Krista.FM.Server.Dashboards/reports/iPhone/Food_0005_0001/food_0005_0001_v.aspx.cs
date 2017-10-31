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
    public partial class Food_0005_0001_v : CustomReportPage
    {

        private CustomParam lastDateParam;
        private CustomParam prevDateParam;
        private CustomParam food;

        private DateTime lastDate;
        private DateTime prevDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Food_0005_0001_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtDate);

            if (dtDate.Rows.Count < 2)
            {
                LabelInfo.Text = String.Format("������ �� ��������� ����� �� �������� ������� �����������");
                return;
            }

            food = UserParams.CustomParam("food");

            lastDateParam = UserParams.CustomParam("last_date");
            lastDateParam.Value = dtDate.Rows[0]["����"].ToString(); ;
            lastDate = CRHelper.DateByPeriodMemberUName(lastDateParam.Value, 3);
            LabelInfo.Text = String.Format("��������� ���� �� �������� ������� ��&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;", lastDate);

            prevDateParam = UserParams.CustomParam("prev_date");
            prevDateParam.Value = dtDate.Rows[1]["����"].ToString();
            prevDate = CRHelper.DateByPeriodMemberUName(prevDateParam.Value, 3);
            LabelInfo.Text += String.Format("(��������� �� ������ �&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>)", prevDate);

            FillInfoLabel();
            FillTextLabel();
            FillTable();

        }

        protected void FillInfoLabel()
        {
            DataTable dtData = new DataTable();
            string query = DataProvider.GetQueryText("Food_0005_0001_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "������������", dtData);

            string foodUp = String.Empty, foodDown = String.Empty, foodConst = String.Empty;
            foreach (DataRow row in dtData.Rows)
            {
                if (!MathHelper.IsDouble(row["���� ��������"]))
                    continue;
                string food = row["������������"].ToString();
                double value = Convert.ToDouble(row["���� ��������"]);
                if (value > 0)
                {
                    foodUp += String.Format("<br/>&nbsp;&nbsp;&nbsp;{0} (��&nbsp;<span class='DigitsValue'>{1:P2}</span>)", food, value);
                }
                else if (value < 0)
                {
                    foodDown += String.Format("<br/>&nbsp;&nbsp;&nbsp;{0} (��&nbsp;<span class='DigitsValue'>{1:P2}</span>)", food, -value);
                }
                else
                {
                    foodConst += String.Format("<br/>&nbsp;&nbsp;&nbsp;{0}", food);
                }
            }
            if (foodUp == foodDown)
            {
                LabelInfo.Text += "<br/>���� � ���� �� �������� ������� �������� ��� ���������.";
                return;
            }

            if (!String.IsNullOrEmpty(foodUp))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>�������� ���������� ���{0}��:</span>{1}", "&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;", foodUp);
            }

            if (!String.IsNullOrEmpty(foodDown))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>�������� �������� ���{0}��:</span>{1}", "&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;", foodDown);
            }

            if (!String.IsNullOrEmpty(foodConst))
            {
                LabelInfo.Text += String.Format("<br/>�� ��������� �������� ������� ���� ��������� ��� ���������� �������������.");
            }

        }

        protected void FillTextLabel()
        {
            double[] values = new double[0];
            string[] names = new string[0];
            DataTable dtHMAO = new DataTable();
            string query = DataProvider.GetQueryText("Food_0005_0001_v_text_hmao");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "�������", dtHMAO);
            foreach (DataRow row in dtHMAO.Rows)
            {
                double growthHMAO = Convert.ToDouble(row["���� �����"]);
                string shortFood = ConvertFoodName(row["�������"].ToString());
                string unit = row["������� ���������"].ToString().Replace("���������", "��.").Replace("����", "�.");
                food.Value = row["���������� ���"].ToString();
                query = DataProvider.GetQueryText("Food_0005_0001_v_one_food_stat_data");
                DataTable dtFood = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "��", dtFood);
                DataRow beloyarRow = null;
                foreach (DataRow moRow in dtFood.Rows)
                {
                    string mo = moRow["��"].ToString();
                    if (mo == "�� ����������")
                    {
                        beloyarRow = moRow;
                        continue;
                    }
                    object growthMO = moRow["���� �����"];
                    object lastCost = moRow["���� �� ��������� ����"];
                    if (MathHelper.IsDouble(growthMO) && Convert.ToDouble(growthMO) > growthHMAO)
                    {
                        Array.Resize(ref values, values.Length + 1);
                        Array.Resize(ref names, names.Length + 1);
                        values[values.Length - 1] = Convert.ToDouble(growthMO);
                        names[names.Length - 1] = String.Format("{0}-/-{1}-/-{2}-/-{3}", mo, shortFood, unit, lastCost);
                    }
                }

                query = DataProvider.GetQueryText("Food_0005_0001_v_one_food_data");
                dtFood = new DataTable();
                DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "��", dtFood);
                foreach (DataRow moRow in dtFood.Rows)
                {
                    string mo = moRow["��"].ToString();
                    object lastCostMO, prevCostMO;
                    if ((mo == "���������� ������������� �����") && (beloyarRow != null))
                    {
                        lastCostMO = MathHelper.GeoMean(moRow, 1, 2, DBNull.Value, beloyarRow["���� �� ��������� ����"]);
                        prevCostMO = MathHelper.GeoMean(moRow, 2, 2, DBNull.Value, beloyarRow["���� �� ���������� ����"]);
                    }
                    else
                    {
                        lastCostMO = MathHelper.GeoMean(moRow, 1, 2, DBNull.Value);
                        prevCostMO = MathHelper.GeoMean(moRow, 2, 2, DBNull.Value);
                    }
                    object growthMO = MathHelper.Div(lastCostMO, prevCostMO);
                    if (MathHelper.IsDouble(growthMO) && Convert.ToDouble(growthMO) > growthHMAO)
                    {
                        Array.Resize(ref values, values.Length + 1);
                        Array.Resize(ref names, names.Length + 1);
                        values[values.Length - 1] = Convert.ToDouble(growthMO);
                        names[names.Length - 1] = String.Format("{0}-/-{1}-/-{2}-/-{3}", mo, shortFood, unit, lastCostMO);
                    }
                }
            }

            Array.Sort(values, names, new ReverseComparer());

            double[] shortValues;
            string[] shortNames;

            int index = values.Length >= 5 ? 5 : values.Length;

            shortValues = new double[index];
            shortNames = new string[index];
            Array.Copy(names, shortNames, index);
            Array.Copy(values, shortValues, index);

            Array.Sort(names, values);

            LabelText.Text = "� �������� ������� ���� �����, ����������� �������������� ���������� ������� � �� ����-����:";
            string prevMO = String.Empty;
            for (int i = 0; i < shortNames.Length; ++i)
            {
                string[] separator = { "-/-" };
                string mo = shortNames[i].Split(separator, StringSplitOptions.None)[0].Replace(" ������������� �����", " ��");
                string food = shortNames[i].Split(separator, StringSplitOptions.None)[1];
                string unit = shortNames[i].Split(separator, StringSplitOptions.None)[2];
                double cost = Convert.ToDouble(shortNames[i].Split(separator, StringSplitOptions.None)[3]);
                double grown = Convert.ToDouble(shortValues[i]);
                if (mo != prevMO)
                {
                    prevMO = mo;
                    LabelText.Text += String.Format("<br/><i>{0}:</i>", mo);
                }
                LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0} (���� - {1:N2} ���./{3}) - &nbsp;<span class='DigitsValue'>{2:P2}</span>", food, cost, grown, unit);
            }

        }

        protected void FillTable()
        {
            DataTable dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("Food_0005_0001_v_grid_hmao");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "������������", dtGrid);

            if (dtGrid.Rows.Count == 0)
                return;

            lastDateParam.Value = lastDateParam.Value.Replace("[������__����]", "[������__������]");
            prevDateParam.Value = prevDateParam.Value.Replace("[������__����]", "[������__������]");

            DataTable dtGridRf = new DataTable();
            query = DataProvider.GetQueryText("Food_0005_0001_v_grid_rf");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "������������", dtGridRf);

            Dictionary<string, string> rfFoods = new Dictionary<string, string>(dtGridRf.Rows.Count);
            foreach (DataRow rfGridRow in dtGridRf.Rows)
            {
                rfFoods.Add(rfGridRow["������������"].ToString(), String.Format("{0:N2};{1:N2};{2:P2}", rfGridRow["���� �� ��������� ����"], rfGridRow["�������"], rfGridRow["���� ��������"]));
            }

            LabelGrid.Text = "<table Class=HtmlTable width=310px>";
            LabelGrid.Text += "<tr><th Class=HtmlTableHeader style=\"width: 110px; border-color: black;\">�������</th>";
            LabelGrid.Text += "<th Class=HtmlTableHeader style=\"width: 100px; border-color: black;\">����</th>";
            LabelGrid.Text += "<th Class=HtmlTableHeader style=\"width: 100px; border-color: black;\">��</th></tr>";

            for (int i = 0; i < dtGrid.Rows.Count; ++i)
            {
                DataRow rowHMAO = dtGrid.Rows[i];
                string food = ConvertFoodName(rowHMAO["������������"].ToString());
                string unit = rowHMAO["������� ���������"].ToString().Replace("���������", "��.").Replace("����", "�.");
                string hmao = String.Format("{0:N2}<br/>{1:N2}<br/>{2:P2}",
                    rowHMAO["���� �� ��������� ����"], rowHMAO["�������"], rowHMAO["���� ��������"]);
                string rfInfo = String.Empty;
                string rf = "-<br/>-<br/>-";
                if (rfFoods.TryGetValue(food, out rfInfo))
                {
                    rf = String.Format("{0}<br/>{1}<br/>{2}", rfInfo.Split(';')[0], rfInfo.Split(';')[1], rfInfo.Split(';')[2]);
                }

                string imageHMAO = String.Empty, imageRF = String.Empty;

                if (MathHelper.IsDouble(rowHMAO["�������"]))
                    if (MathHelper.AboveZero(rowHMAO["�������"]))
                    {
                        imageHMAO = " background-image: url(../../../images/arrowRedUpBB.png); background-repeat: no-repeat; background-position: 5% center;";
                    }
                    else if (MathHelper.SubZero(rowHMAO["�������"]))
                    {
                        imageHMAO = " background-image: url(../../../images/arrowGreenDownBB.png); background-repeat: no-repeat; background-position: 5% center;";
                    }
                if (!String.IsNullOrEmpty(rfInfo) && MathHelper.IsDouble(rfInfo.Split(';')[1]))
                    if (MathHelper.AboveZero(rfInfo.Split(';')[1]))
                    {
                        imageRF = " background-image: url(../../../images/arrowRedUpBB.png); background-repeat: no-repeat; background-position: 5% center;";
                    }
                    else if (MathHelper.SubZero(rfInfo.Split(';')[1]))
                    {
                        imageRF = " background-image: url(../../../images/arrowGreenDownBB.png); background-repeat: no-repeat; background-position: 5% center;";
                    }

                LabelGrid.Text += "<tr>";

                LabelGrid.Text += String.Format("<td>{0}, ���./{1}</td>", food, unit);
                LabelGrid.Text += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", hmao, imageHMAO);
                LabelGrid.Text += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", rf, imageRF);

                LabelGrid.Text += "</tr>";
            }

            LabelGrid.Text += "</table>";
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
