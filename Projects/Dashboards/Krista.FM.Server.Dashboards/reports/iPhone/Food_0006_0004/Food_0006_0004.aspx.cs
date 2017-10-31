using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.iPadBricks.iPadBricks;

namespace Krista.FM.Server.Dashboards.reports.iPad
{

    public partial class Food_0006_0004 : CustomReportPage
    {

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            InitializeDate();
			
            UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid1.DataBind();
			iPadBricksHelper.SetMinMaxImage(UltraWebGrid1, 1);
            UltraWebGrid1.Height = Unit.Empty;
			UltraWebGrid1.Width = Unit.Empty;

        }

		private DateTime currDate;
		private DateTime lastDate;
		private DateTime yearDate;

        private void InitializeDate()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Food_0006_0004_incomes_date");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", dtDate);

			UserParams.PeriodFirstYear.Value = dtDate.Rows[0]["��������"].ToString();
			UserParams.PeriodLastDate.Value = dtDate.Rows[1]["��������"].ToString();
			UserParams.PeriodCurrentDate.Value = dtDate.Rows[2]["��������"].ToString();

			currDate = CRHelper.DateByPeriodMemberUName(UserParams.PeriodCurrentDate.Value, 3);
			lastDate = CRHelper.DateByPeriodMemberUName(UserParams.PeriodLastDate.Value, 3);
			yearDate = CRHelper.DateByPeriodMemberUName(UserParams.PeriodFirstYear.Value, 3);
        }

		#region ����

		private void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
			iPadBricksHelper.SetConditionArrow(e, 3, 0, false);
			iPadBricksHelper.SetConditionArrow(e, 5, 0, false);

            string moName = e.Row.Cells[0].Text.Replace(" �����", " �-�").Replace("�����-�������� ���������� �����", "����");

            if (!moName.StartsWith("&nbsp;&nbsp;&nbsp;"))
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
            }

            e.Row.Cells[0].Text = String.Format("{0}", moName);

        }

		private void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

			UltraGridBand band = e.Layout.Bands[0];

            band.Columns[0].Width = 230;
            band.Columns[1].Width = 130;
            band.Columns[2].Width = 100;
            band.Columns[3].Width = 100;
            band.Columns[4].Width = 100;
            band.Columns[5].Width = 100;

			GridHeaderLayout headerLayout = new GridHeaderLayout(e.Layout.Grid);
			headerLayout.AddCell("������������� �����������");
			headerLayout.AddCell(String.Format("��. ���� �� {0:dd.MM.yyyy}�.", currDate));
			GridHeaderCell headerCell = headerLayout.AddCell(String.Format("��������� �� ��������� � {0:dd.MM.yyyy}�.", lastDate));
			headerCell.AddCell("���. ������.");
			headerCell.AddCell("���� ��������");
			headerCell = headerLayout.AddCell(String.Format("��������� �� ��������� � {0:dd.MM.yyyy}�.", yearDate));
			headerCell.AddCell("���. ������.");
			headerCell.AddCell("���� ��������");

			headerLayout.ApplyHeaderInfo();

            band.Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            band.Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            band.Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            band.Columns[0].CellStyle.BorderDetails.WidthRight = borderWidth;
            band.Columns[1].CellStyle.BorderDetails.WidthRight = borderWidth;
            band.Columns[3].CellStyle.BorderDetails.WidthRight = borderWidth;
            band.Columns[5].CellStyle.BorderDetails.WidthRight = borderWidth;

            band.Columns[0].CellStyle.BorderDetails.WidthLeft = borderWidth;
            band.Columns[1].CellStyle.BorderDetails.WidthLeft = borderWidth;
            band.Columns[2].CellStyle.BorderDetails.WidthLeft = borderWidth;
            band.Columns[4].CellStyle.BorderDetails.WidthLeft = borderWidth;

			band.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            CRHelper.FormatNumberColumn(band.Columns[1], "N2");
            CRHelper.FormatNumberColumn(band.Columns[2], "N2");
            CRHelper.FormatNumberColumn(band.Columns[3], "P2");
            CRHelper.FormatNumberColumn(band.Columns[4], "N2");
            CRHelper.FormatNumberColumn(band.Columns[5], "P2");

        }

        private int borderWidth = 3;

		private void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            DataTable dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("Food_0006_0004_grid_stat");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "��", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                if (row["��"].ToString().Contains("��� �����������") || row["��"].ToString().Contains("������� �� ��"))
                {
                    row["��"] = row["��"].ToString().Split(';')[0];
                }
                else
                {
                    row["��"] = "&nbsp;&nbsp;&nbsp;" + row["��"].ToString().Split(';')[1];
                }
            }

			// ���������� ��������
			dtGrid.Columns.Add("���������� ������������ ���������� ����", typeof(double));
			MathHelper.RowMinus(dtGrid, 3, 2, 4);
			dtGrid.Columns.Add("������� ������������ ���������� ����", typeof(double));
			MathHelper.RowGrown(dtGrid, 3, 2, 5);
			dtGrid.Columns.Add("���������� ������������ ������ ����", typeof(double));
			MathHelper.RowMinus(dtGrid, 3, 1, 6);
			dtGrid.Columns.Add("������� ������������ ������ ����", typeof(double));
			MathHelper.RowGrown(dtGrid, 3, 1, 7);

            dtGrid.Columns.RemoveAt(2);
            dtGrid.Columns.RemoveAt(1);

            UltraWebGrid1.DataSource = dtGrid;
        }

		#endregion

    }
}
