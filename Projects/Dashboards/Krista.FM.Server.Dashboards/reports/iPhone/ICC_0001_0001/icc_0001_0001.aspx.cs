using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class ICC_0001_0001 : CustomReportPage
    {

        private CustomParam lastDateParam;
        private CustomParam prevDateParam;
        private CustomParam yearParam;

        private DateTime lastDate;
        private DateTime prevDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            lastDateParam = UserParams.CustomParam("last_date");
            prevDateParam = UserParams.CustomParam("prev_date");
            yearParam = UserParams.CustomParam("year");

            string query = DataProvider.GetQueryText("ICC_0001_0001_last_date");
            DataTable dtDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "�����", dtDate);

            if (dtDate.Rows.Count == 0)
                return;

            lastDateParam.Value = dtDate.Rows[0]["��������� ����� � �������"].ToString();
            lastDate = CRHelper.DateByPeriodMemberUName(lastDateParam.Value, 3);

            yearParam.Value = (lastDate.Year - 1).ToString();

            query = DataProvider.GetQueryText("ICC_0001_0001_prev_date");
            dtDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "�����", dtDate);

            prevDateParam.Value = dtDate.Rows[0]["��������� ����� ����������� ����"].ToString();
            prevDate = CRHelper.DateByPeriodMemberUName(prevDateParam.Value, 3);

            LabelTitle.Text += "������ ��������������� ���<br/>";
            LabelTitle.Text += String.Format("<span class='DigitsValue'>({0} {1:yyyy} �. � {3} {2:yyyy} �.)</span>", CRHelper.RusMonth(lastDate.Month), lastDate, prevDate, CRHelper.RusMonthDat(prevDate.Month));

            #region ������������������ �����

            /*
            query = DataProvider.GetQueryText("ICC_0001_0001_grid");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "������", dtGrid);
            
            if (dtGrid.Rows.Count == 0)
                return;

            DataRow row = dtGrid.Rows[0];

            object hmaoValue = row["�������� �� ��������� ����; �����-���������� ���������� �����"];
            object hmaoGrowth = row["�������; �����-���������� ���������� �����"];
            object urfoValue = row["�������� �� ��������� ����; ��������� ����������� �����"];
            object urfoGrowth = row["�������; ��������� ����������� �����"];
            object rfValue = row["�������� �� ��������� ����; ����������  ���������"];
            object rfGrowth = row["�������; ����������  ���������"];

            string img = String.Empty;

            if (MathHelper.IsDouble(hmaoValue))
            {
                if (!MathHelper.IsDouble(hmaoGrowth) || Convert.ToDouble(hmaoGrowth) == 0)
                {
                    LabelText.Text = String.Format("������ ��������������� ��� � ���� ����������&nbsp;<span class='DigitsValue'>{0:N2}%</span>", hmaoValue);
                }
                else if (Convert.ToDouble(hmaoGrowth) < 0)
                {
                    LabelText.Text = String.Format("������ ��������������� ��� � ���� ��������&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;��&nbsp;<span class='DigitsValue'>{0:N2}%</span>", hmaoValue);
                }
                else
                {
                    LabelText.Text = String.Format("������ ��������������� ��� � ���� ����������&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;��&nbsp;<span class='DigitsValue'>{0:N2}%</span>", hmaoValue);
                }

                if (MathHelper.IsDouble(urfoValue))
                {
                    if (Convert.ToDouble(hmaoValue) - Convert.ToDouble(urfoValue) < 0)
                    {
                        LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;����&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;��� � ����&nbsp;<span class='DigitsValue'>{0:N2}%</span>", urfoValue);
                    }
                    else if (Convert.ToDouble(hmaoValue) - Convert.ToDouble(urfoValue) > 0)
                    {
                        LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;����&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;��� � ����&nbsp;<span class='DigitsValue'>{0:N2}%</span>", urfoValue);
                    }
                    else
                    {
                        LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;������������� ����&nbsp;<span class='DigitsValue'>{0:N2}%</span>", urfoValue);
                    }
                }

                if (MathHelper.IsDouble(rfValue))
                {
                    if (Convert.ToDouble(hmaoValue) - Convert.ToDouble(rfValue) < 0)
                    {
                        LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;����&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;��� � ��&nbsp;<span class='DigitsValue'>{0:N2}%</span>", rfValue);
                    }
                    else if (Convert.ToDouble(hmaoValue) - Convert.ToDouble(rfValue) > 0)
                    {
                        LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;����&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;��� � ��&nbsp;<span class='DigitsValue'>{0:N2}%</span>", rfValue);
                    }
                    else
                    {
                        LabelText.Text += String.Format("<br/>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;������������� ��&nbsp;<span class='DigitsValue'>{0:N2}%</span>", rfValue);
                    }
                }
            }

            query = DataProvider.GetQueryText("ICC_0001_0001_text");
            DataTable dtText = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "�����", dtText);

            LabelText.Text = String.Format("��������� ������������ ������ ��������� ������� � ������� �� ����-���� � ����� {0} {1:yyyy} �. ���������" +
                "&nbsp;<span class='DigitsValue'>{2:N2} ���.</span>&nbsp;� ������� �� �����",
                CRHelper.RusMonthGenitive(lastDate.Month), lastDate, dtText.Rows[0]["�������� �� ��������� ����"]);

            if (MathHelper.IsDouble(dtText.Rows[0]["�������"]))
            {
                if (MathHelper.AboveZero(dtText.Rows[0]["�������"]))
                {
                    LabelText.Text += String.Format("<br/>�� ��������� � ��������� ������� ����������� ���� ��������� �����������" +
                        "&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;��&nbsp;<span class='DigitsValue'>{0:N2} ���.</span>&nbsp;",
                        Convert.ToDouble(dtText.Rows[0]["�������"]));
                }
                else if (MathHelper.SubZero(dtText.Rows[0]["�������"]))
                {
                    LabelText.Text += String.Format("<br/>�� ��������� � ��������� ������� ����������� ���� ��������� ���������" +
                        "&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;��&nbsp;<span class='DigitsValue'>{0:N2} ���.</span>&nbsp;",
                        -Convert.ToDouble(dtText.Rows[0]["�������"]));
                }
                else
                {
                    LabelText.Text += String.Format("<br/>�� ��������� � ��������� ������� ����������� ���� ��������� �� ����������");
                }
            }
            */

            #endregion

            query = DataProvider.GetQueryText("ICC_0001_0001_v_grid");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "������", dtGrid);

            //LabelGrid.Text = "������ ��������������� ���<br/>" + String.Format("({0:MMMM yyyy} �. � {2} {1:yyyy} �.)", lastDate, prevDate, CRHelper.RusMonthDat(prevDate.Month)).ToLower();

            LabelGrid.Text = "<table Class=HtmlTable width=310px>";
            LabelGrid.Text += "<tr>";
            foreach (DataColumn c in dtGrid.Columns)
            {
                string title = c.Caption;
                if (title.Contains("���������"))
                    title = "��";
                else if (title.Contains("���������"))
                    title = "����";
                else if (title.Contains("�����"))
                    title = "����";
                LabelGrid.Text += String.Format("<th Class=HtmlTableHeader style=\"width: {1}px; border-color: black;\">{0}</th>",
                    title, title == "������" ? 100 : 70);
            }
            LabelGrid.Text += "</tr>";

            string[] names = {"�������-�������� ���", "�� ������. ������", "�� ��������. ������", "������� ������ ������-���"};

            for (int rowIndex = 0; rowIndex < dtGrid.Rows.Count; ++rowIndex)
            {
                DataRow row = dtGrid.Rows[rowIndex];
                LabelGrid.Text += "<tr>";
                LabelGrid.Text += String.Format("<td style=\"text-align: left; padding-left: {1}px;\">{0}</td>", names[rowIndex], rowIndex == 0 ? 5 : 15);
                for (int i = 1; i < row.ItemArray.Length; ++i)
                {
                    object cell = row[i];
                    double value;
                    if (Double.TryParse(cell.ToString(), out value))
                    {
                        string img = String.Empty;
                        if (value > 100)
                            img = " background-image: url(../../../images/arrowRedUpBB.png); background-repeat: no-repeat; background-position: 5% center;";
                        else if (value < 100)
                            img = " background-image: url(../../../images/arrowGreenDownBB.png); background-repeat: no-repeat; background-position: 5% center;";
                        LabelGrid.Text += String.Format("<td style=\"text-align: right;{1}\">{0:N2}</td>", value, img);
                    }
                    else
                        LabelGrid.Text += String.Format("<td style=\"text-align: left;\">{0}</td>", cell);
                }
                LabelGrid.Text += "</tr>";
            }

            LabelGrid.Text += "</table>";
        }
    }
}
