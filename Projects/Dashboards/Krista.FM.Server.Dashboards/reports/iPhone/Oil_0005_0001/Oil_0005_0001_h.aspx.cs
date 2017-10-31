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
    public partial class Oil_0005_0001_h : CustomReportPage
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
            string query = DataProvider.GetQueryText("Oil_0005_0001_date");
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
            query = DataProvider.GetQueryText("Oil_0005_0001_h_year");
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
            query = DataProvider.GetQueryText("Oil_0005_0001_h_year_rf");
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

            Label.Text = String.Format("��������� ���� �� ������������� ��&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span><br/>", lastDate);

            Label.Text += "<table Class=HtmlTable width=470px>";

            Label.Text += FillData(lastDate, prevDate, prevDate, "�� ������");
            Label.Text += FillData(lastDate, yearDate, rfYearDate, "� ������ ����");

            Label.Text += "</table>";

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

        protected string FillData(DateTime currentDate, DateTime compareDate, DateTime rfCompareDate, string period)
        {
            lastDateParam.Value = CRHelper.PeriodMemberUName("[������__����].[������__����].[������ ���� ��������]", currentDate, 5);
            prevDateParam.Value = CRHelper.PeriodMemberUName("[������__����].[������__����].[������ ���� ��������]", compareDate, 5);

            string result = String.Format("<tr><td colspan=7 style=\"padding-left: 0px; border-color: black;\">��������� �� ������ �&nbsp;<span class='DigitsValue'>{0:dd.MM.yyyy}</span>&nbsp;��&nbsp;<span class='DigitsValue'>{1:dd.MM.yyyy}</span></td></tr>",
                compareDate, currentDate);

            /*DataTable dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_v_grid_hmao");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "��� ���", dtGrid);*/

            #region ���� �� ����

            CustomParam dateParam = UserParams.CustomParam("date");
            dateParam.Value = lastDateParam.Value;
            DataTable dtLastDate = new DataTable();
            string query = DataProvider.GetQueryText("Oil_0005_0001_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "�����������", dtLastDate);

            dateParam.Value = prevDateParam.Value;
            DataTable dtPrevDate = new DataTable();
            query = DataProvider.GetQueryText("Oil_0005_0001_data");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "�����������", dtPrevDate);

            DataTable dtGrid = MakeHmaoTable(dtLastDate, dtPrevDate);

            #endregion

            lastDateParam.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", currentDate, 5);
            prevDateParam.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", rfCompareDate, 5);

            DataTable dtGridRf = new DataTable();
            query = DataProvider.GetQueryText("Oil_0005_0001_v_grid_rf");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "��� ���", dtGridRf);

            if (dtGrid.Rows.Count == 0)
                return result;

            //result += "<table Class=HtmlTable width=480px>";

            result += "<tr><th Class=HtmlTableHeader rowspan=\"2\" style=\"width: 140px; border-color: black;\">��� ���</th>";
            result += String.Format("<th Class=HtmlTableHeader colspan=2 style=\"width: 110px; border-color: black;\">���� �� {0:dd.MM.yyyy}</th>", currentDate);
            result += String.Format("<th Class=HtmlTableHeader colspan=2 style=\"width: 110px; border-color: black;\">���� �� {0:dd.MM.yyyy}</th>", compareDate);
            result += String.Format("<th Class=HtmlTableHeader colspan=2 style=\"width: 110px; border-color: black;\">�������� {0}</th></tr>", period);
            result += "<tr><th Class=HtmlTableHeader style=\"width: 55px; border-color: black;\">����</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 55px; border-color: black;\">��</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 55px; border-color: black;\">����</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 55px; border-color: black;\">��</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 55px; border-color: black;\">����</th>";
            result += "<th Class=HtmlTableHeader style=\"width: 55px; border-color: black;\">��</th></tr>";

            for (int i = 0; i < dtGrid.Rows.Count && i < dtGridRf.Rows.Count; ++i)
            {
                DataRow rowHMAO = dtGrid.Rows[i];
                DataRow rowRF = dtGridRf.Rows[i];
                string hmao = String.Format("{0:N2}<br/>{1:P2}", rowHMAO["�������"], rowHMAO["���� ��������"]);
                string rf = String.Format("{0:N2}<br/>{1:P2}", rowRF["�������"], rowRF["���� ��������"]);

                string imageHMAO = String.Empty, imageRF = String.Empty;

                if (MathHelper.IsDouble(rowHMAO["�������"]))
                    if (MathHelper.AboveZero(rowHMAO["�������"]))
                    {
                        imageHMAO = " background-image: url(../../../images/arrowRedUpBB.png); background-repeat: no-repeat; background-position: 5% 5%;";
                    }
                    else if (MathHelper.SubZero(rowHMAO["�������"]))
                    {
                        imageHMAO = " background-image: url(../../../images/arrowGreenDownBB.png); background-repeat: no-repeat; background-position: 5% 5%;";
                    }
                if (MathHelper.IsDouble(rowRF["�������"]))
                    if (MathHelper.AboveZero(rowRF["�������"]))
                    {
                        imageRF = " background-image: url(../../../images/arrowRedUpBB.png); background-repeat: no-repeat; background-position: 5% 5%;";
                    }
                    else if (MathHelper.SubZero(rowRF["�������"]))
                    {
                        imageRF = " background-image: url(../../../images/arrowGreenDownBB.png); background-repeat: no-repeat; background-position: 5% 5%;";
                    }

                result += "<tr>";

                result += String.Format("<td>{0}</td>", rowHMAO["��� ���"].ToString().Replace(" ��-", "<br/>��-"));
                result += String.Format("<td style=\"text-align: right;\">{0:N2}</td>", rowHMAO["������� �� �������� �� ��������� ����"]);
                result += String.Format("<td style=\"text-align: right;\">{0:N2}</td>", rowRF["���� �� ��������� ����"]);
                result += String.Format("<td style=\"text-align: right;\">{0:N2}</td>", rowHMAO["������� �� �������� �� ���������� ����"]);
                result += String.Format("<td style=\"text-align: right;\">{0:N2}</td>", rowRF["���� �� ���������� ����"]);
                result += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", hmao, imageHMAO);
                result += String.Format("<td style=\"text-align: right;{1}\">{0}</td>", rf, imageRF);

                result += "</tr>";
            }

            //result += "</table>";

            return result;

        }

    }
}
