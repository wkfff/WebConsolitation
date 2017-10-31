using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FO_0035_0023 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();

        // ��������� ������
        private CustomParam selectedPeriod;

        private DateTime date;
        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {            
            #region ������������� ���������� �������

            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0035_0023_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtDate);

            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);

            selectedPeriod.Value = dtDate.Rows[0][1].ToString();
            UserParams.PeriodLastYear.Value = CRHelper.PeriodMemberUName("[������__������].[������__������].[������ ���� ��������]", date.AddYears(-1), 5);

            DataTable dtLastYearDate = new DataTable();
            query = DataProvider.GetQueryText("FO_0035_0023_date_appg");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtLastYearDate);

            if (dtLastYearDate.Rows.Count > 0)
            {
                UserParams.PeriodLastYear.Value = dtLastYearDate.Rows[0][1].ToString();
            }
            else
            {
                UserParams.PeriodLastYear.Value = "[������__������].[������__������].[������ ���� ��������].[1000].[��������� 1].[������� 1].[�������].[22] ";
            }

            OutcomesGrid.Width = Unit.Empty;
            OutcomesGrid.Height = Unit.Empty;
            OutcomesGrid.DisplayLayout.NoDataMessage = "��� ������";

            OutcomesGrid.DataBind();

            lbDescription.Text = String.Format("���������� ������ �������������� ���������� ������� ������������� �������<br/>��&nbsp;<b><span class='DigitsValue'>{0:dd.MM.yyyy}</span></b>, ���. ���.<br/><b><span class='ImportantText'>{1}</span></b>",
                date, CRHelper.ToUpperFirstSymbol(UserParams.Outcomes.Value.ToLower()).Replace("������ ���� ����������", "�����").Replace(" ��", " ��"));           
        }

        #region ����������� �����

        protected void OutcomesGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0035_0023_OutcomesGrid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ ����", dtGrid);

            OutcomesGrid.DataSource = dtGrid;
        }

        private int borderWidth = 3;

        protected void OutcomesGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            e.Layout.Bands[0].Columns[0].Width = 190;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 86;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");

            e.Layout.Bands[0].Columns[2].Width = 81;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");

            e.Layout.Bands[0].Columns[3].Width = 81;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");

            e.Layout.Bands[0].Columns[4].Width = 81;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");

            e.Layout.Bands[0].Columns[5].Width = 97;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");

            e.Layout.Bands[0].Columns[6].Width = 71;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");

            e.Layout.Bands[0].Columns[7].Width = 71;
            e.Layout.Bands[0].Columns[7].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "P2");

            e.Layout.Bands[0].Columns[8].Hidden = true;

            for (int i = 2; i < 8; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 3;
            }

            headerLayout = new GridHeaderLayout(e.Layout.Grid);
                       
            headerLayout.AddCell("�������");
            headerLayout.AddCell(String.Format("����"));
            GridHeaderCell cell = headerLayout.AddCell("���������� ����������������");
            cell.AddCell(String.Format("�� {0}", date.ToString("dd.MM")));
            cell.AddCell(String.Format("� ������ ������", CRHelper.RusMonthGenitive(date.Month)));
            cell.AddCell(String.Format("�� ���&nbsp;�� ������ ����.����", CRHelper.RusMonthGenitive(date.Month)));
            headerLayout.AddCell(String.Format("������� �� {0}", CRHelper.RusMonth(date.Month)));
            headerLayout.AddCell("% ���.");
            headerLayout.AddCell("���� �����");

            headerLayout.ApplyHeaderInfo();

            //e.Layout.Bands[0].Columns[2].HeaderStyle.BorderDetails.ColorRight = Color.FromArgb(85, 85, 85);
            //e.Layout.Bands[0].Columns[3].HeaderStyle.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);

            //e.Layout.Bands[0].Columns[5].HeaderStyle.BorderDetails.ColorRight = Color.FromArgb(85, 85, 85);
            //e.Layout.Bands[0].Columns[6].HeaderStyle.BorderDetails.ColorLeft = Color.FromArgb(85, 85, 85);
        }


        protected void OutcomesGrid_InitializeRow(object sender, RowEventArgs e)
        {
            iPadBricks.iPadBricks.iPadBricksHelper.SetConditionCorner(e, 5, 0, true);

            if (e.Row.Cells[0].Value.ToString() == "�����")
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
                e.Row.Cells[5].Style.Font.Size = 14;
            }
            else
            {
                if (e.Row.Cells[8].Value.ToString() == "������� 3")
                {
                    e.Row.Cells[0].Style.Padding.Left = 15;
                }
            }
        }
        #endregion
    }
}
