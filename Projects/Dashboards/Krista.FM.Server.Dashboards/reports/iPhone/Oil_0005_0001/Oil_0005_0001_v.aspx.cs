using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class Oil_0005_0001_v : CustomReportPage
    {

        private CustomParam lastDateParam;
        private CustomParam prevDateParam;
        private CustomParam dateParam;
        private CustomParam grown80Param;
        private CustomParam grown92Param;
        private CustomParam grown95Param;
        private CustomParam grownDTParam;

        private DateTime lastDate;
        private DateTime prevDate;

        private DataTable dtGrid;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_date");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "����", dtDate);

            if (dtDate.Rows.Count < 2)
            {
                LabelInfo.Text = String.Format("������ �� ��������� ����� �� ������������� �����������");
                return;
            }

            dateParam = UserParams.CustomParam("date");
            grown80Param = UserParams.CustomParam("grown_80");
            grown92Param = UserParams.CustomParam("grown_92");
            grown95Param = UserParams.CustomParam("grown_95");
            grownDTParam = UserParams.CustomParam("grown_DT");

            lastDateParam = UserParams.CustomParam("last_date");
            lastDateParam.Value = dtDate.Rows[0]["����"].ToString(); ;
            lastDate = CRHelper.DateByPeriodMemberUName(lastDateParam.Value, 3);
            LabelInfo.Text = String.Format("��������� ���� �� ������������� ��&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;", lastDate);

            prevDateParam = UserParams.CustomParam("prev_date");
            prevDateParam.Value = dtDate.Rows[1]["����"].ToString();
            prevDate = CRHelper.DateByPeriodMemberUName(prevDateParam.Value, 3);
            LabelInfo.Text += String.Format("(��������� �� ������ �&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>)", prevDate);

            FillInfoLabel();
            FillTextLabel();
            FillTable();

        }

        protected DataTable MakeHmaoTable(DataTable dtLastDate, DataTable dtPrevDate)
        {
            DataTable dtGrid = new DataTable();

            dtGrid.Columns.Add("��� ���", typeof(string));
            dtGrid.Columns.Add("������� �� �������� �� ��������� ����", typeof(double));
            dtGrid.Columns.Add("������� �� �������� �� ���������� ����", typeof(double));
            dtGrid.Columns.Add("�������", typeof(double));
            dtGrid.Columns.Add("���� ��������", typeof(double));

            string[] oil = { "������ ����� ��-80", "������ ����� ��-92", "������ ����� ��-95", "��������� �������" };

            foreach (string fuel in oil)
            {
                DataRow row = dtGrid.NewRow();

                row["��� ���"] = fuel;
                row["������� �� �������� �� ��������� ����"] = MathHelper.GeoMean(dtLastDate.Columns[fuel], DBNull.Value);
                row["������� �� �������� �� ���������� ����"] = MathHelper.GeoMean(dtPrevDate.Columns[fuel], DBNull.Value);
                row["�������"] = MathHelper.Minus(row["������� �� �������� �� ��������� ����"], row["������� �� �������� �� ���������� ����"]);
                row["���� ��������"] = MathHelper.Grown(row["������� �� �������� �� ��������� ����"], row["������� �� �������� �� ���������� ����"]);

                dtGrid.Rows.Add(row);
            }

            return dtGrid;
        }

        protected void FillInfoLabel()
        {
            CustomParam dateParam = UserParams.CustomParam("date");
            dateParam.Value = lastDateParam.Value;
            DataTable dtLastDate = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "�����������", dtLastDate);

            dateParam.Value = prevDateParam.Value;
            DataTable dtPrevDate = new DataTable();
            query = DataProvider.GetQueryText("Oil_0005_0001_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "�����������", dtPrevDate);

            dtGrid = MakeHmaoTable(dtLastDate, dtPrevDate);

            string fuelUp = String.Empty, fuelDown = String.Empty, fuelConst = String.Empty;
            foreach (DataColumn column in dtLastDate.Columns)
            {
                if (column.ColumnName == "�����������")
                    continue;
                string fuel = column.ColumnName;
                object price = MathHelper.GeoMean(column, DBNull.Value);
                object prevPrice = MathHelper.GeoMean(dtPrevDate.Columns[fuel], DBNull.Value);
                object value = MathHelper.Grown(price, prevPrice, 0, CalcMode.CalcIfTwo);
                switch (fuel)
                {
                    case "������ ����� ��-80":
                        {
                            grown80Param.Value = value.ToString();
                            break;
                        }
                    case "������ ����� ��-92":
                        {
                            grown92Param.Value = value.ToString();
                            break;
                        }
                    case "������ ����� ��-95":
                        {
                            grown95Param.Value = value.ToString();
                            break;
                        }
                    case "��������� �������":
                        {
                            grownDTParam.Value = value.ToString();
                            break;
                        }
                }
                if (MathHelper.AboveZero(value))
                {
                    fuelUp += String.Format("<br/>{0} ��&nbsp;<span class='DigitsValue'>{1:P2}</span><br/>(���� - {2:N2} ���.)", fuel, value, price);
                }
                else if (MathHelper.SubZero(value))
                {
                    fuelDown += String.Format("<br/>{0} ��&nbsp;<span class='DigitsValue'>{1:P2}</span><br/>(���� - {2:N2} ���.)", fuel, MathHelper.Abs(value), price);
                }
                else
                {
                    fuelConst += String.Format("<br/>{0}", fuel);
                }
            }
            
            if (fuelUp == fuelDown)
            {
                LabelInfo.Text += "<br/>���� � ���� �� ������ ���� ����� � ��������� ������� �������� ��� ���������.";
                return;
            }

            if (!String.IsNullOrEmpty(fuelUp))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>�������� ���������� ���{0}��:</span>{1}", "&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;", fuelUp);
            }

            if (!String.IsNullOrEmpty(fuelDown))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>�������� �������� ���{0}��:</span>{1}", "&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;", fuelDown);
            }

            if (!String.IsNullOrEmpty(fuelConst))
            {
                LabelInfo.Text += String.Format("<br/><span class='DigitsValue'>���� ��������� ��:</span>{0}", fuelConst);
            }
        }

        protected void FillTextLabel()
        {
            DataTable dtText = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_v_text");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "���;��", dtText);
            if (dtText.Rows.Count == 0)
                return;
            string[] names = new string[dtText.Rows.Count];
            string[] values = new string[dtText.Rows.Count];
            for (int i = 0; i < dtText.Rows.Count; ++i)
            {
                DataRow row = dtText.Rows[i];
                names[i] = row["���;��"].ToString().Split(';')[1];
                values[i] = row["���;��"].ToString().Split(';')[0] + "/" + row["������� �� �� �� ��������� ����"].ToString() + "/" + row["���� ����� �� ��"].ToString();
            }
            Array.Sort(names, values);
            LabelText.Text = "� �������� ������� � �� ����-���� ���� �����, ����������� �������������� ���������� ������� �:";
            string prevMO = String.Empty;
            for (int i = 0; i < names.Length; ++i)
            {
                string mo = names[i].Replace(" ������������� �����", " ��");
                string fuel = values[i].Split('/')[0].Replace("������ ����� ", String.Empty).Replace("��������� �������", "��");
                double cost = Convert.ToDouble(values[i].Split('/')[1]);
                double grown = Convert.ToDouble(values[i].Split('/')[2]);
                if (mo != prevMO)
                {
                    prevMO = mo;
                    LabelText.Text += String.Format("<br/><i>{0}:</i>", mo);
                }
                LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0} (���� - {1:N2} ���./�.) - &nbsp;<span class='DigitsValue'>{2:P2}</span>", fuel, cost, grown);
            }

        }

        protected void FillTable()
        {
            /*DataTable dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_v_grid_hmao");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "��� ���", dtGrid);*/

            lastDateParam.Value = lastDateParam.Value.Replace("[������__����]", "[������__������]");
            prevDateParam.Value = prevDateParam.Value.Replace("[������__����]", "[������__������]");

            DataTable dtGridRf = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_v_grid_rf");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "��� ���", dtGridRf);

            if (dtGrid.Rows.Count == 0)
                return;

            LabelGrid.Text = "<table Class=HtmlTable width=310px>";
            LabelGrid.Text += "<tr><th Class=HtmlTableHeader style=\"width: 110px; border-color: black;\">��� ���</th>";
            LabelGrid.Text += "<th Class=HtmlTableHeader style=\"width: 100px; border-color: black;\">����</th>";
            LabelGrid.Text += "<th Class=HtmlTableHeader style=\"width: 100px; border-color: black;\">��</th></tr>";

            for (int i = 0; i < dtGrid.Rows.Count && i < dtGridRf.Rows.Count; ++i)
            {
                DataRow rowHMAO = dtGrid.Rows[i];
                DataRow rowRF = dtGridRf.Rows[i];
                string fuel = rowHMAO["��� ���"].ToString();
                string hmao = String.Format("{0:N2}<br/>{1:N2}<br/>{2:P2}",
                    rowHMAO["������� �� �������� �� ��������� ����"], rowHMAO["�������"], rowHMAO["���� ��������"]);
                string rf = String.Format("{0:N2}<br/>{1:N2}<br/>{2:P2}",
                    rowRF["���� �� ��������� ����"], rowRF["�������"], rowRF["���� ��������"]);

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
                if (MathHelper.IsDouble(rowRF["�������"]))
                    if (MathHelper.AboveZero(rowRF["�������"]))
                    {
                        imageRF = " background-image: url(../../../images/arrowRedUpBB.png); background-repeat: no-repeat; background-position: 5% center;";
                    }
                    else if (MathHelper.SubZero(rowRF["�������"]))
                    {
                        imageRF = " background-image: url(../../../images/arrowGreenDownBB.png); background-repeat: no-repeat; background-position: 5% center;";
                    }

                LabelGrid.Text += "<tr>";

                LabelGrid.Text += String.Format("<td>{0}</td>", fuel);
                LabelGrid.Text += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", hmao, imageHMAO);
                LabelGrid.Text += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", rf, imageRF);

                LabelGrid.Text += "</tr>";
            }

            LabelGrid.Text += "</table>";
        }

    }
}
