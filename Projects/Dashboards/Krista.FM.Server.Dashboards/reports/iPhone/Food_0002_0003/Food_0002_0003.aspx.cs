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

    public partial class Food_0002_0003 : CustomReportPage
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
			UltraWebGrid1.Width = Unit.Empty;

            string labelText = "* - �� � ��������� ������� ��������� �������";
            labelText += "<br/>���� - ����, ���������� ��, �. ������, �. �����-��������, �. ������������� - ������ ���������������� ��������� ���� �� ����-����";
            labelText += "<br/>��������� �� - ������ ������������ ��������� ����-����";
            Label1.Text = labelText;

        }

		private DateTime currDate;
		private DateTime lastDate;
		private DateTime yearDate;

        private void InitializeDate()
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("Food_0002_0003_incomes_date");
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

            string moName = e.Row.Cells[0].Text.Replace("����� ", String.Empty).Replace("������������� �����", "��").Replace("�����-���������� ���������� �����", "���� - ����");
			string moNameForRef = e.Row.Cells[0].Text.Replace("����� ", "�.").Replace(" ������������� �����", String.Empty);

            if (moName == "���� - ����")
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
            }

            /*if (moName != "���� - ����")
                e.Row.Cells[0].Text = String.Format(
                    "<a href='webcommand?showPopoverReport=Food_0003_0003_Food={1};MO={2}&width=690&height=530&fitByHorizontal=true'>{0}{3}</a>",
                    moName, CustomParams.GetFoodIdByName(UserParams.Food.Value), CustomParams.GetMOIdByName(moNameForRef), GetStarOrNot(moName));
            else
                e.Row.Cells[0].Text = moName;*/

            e.Row.Cells[0].Text = String.Format("{0}{1}", moName, GetStarOrNot(moName));

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
            DataTable dt1 = new DataTable();
            string query = DataProvider.GetQueryText("Food_0002_0003_grid_stat");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "��", dt1);
			DataTable dt2 = new DataTable();
			query = DataProvider.GetQueryText("Food_0002_0003_grid_econ");
			DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "��", dt2);

            DataTable dtGrid = dt1.Clone();
            DataRow beloyarRow = null;
            foreach (DataRow row in dt1.Rows)
            {
                if (row["��"].ToString() == "�� ����������")
                {
                    beloyarRow = row;
                    continue;
                }
                else
                {
                    dtGrid.ImportRow(row);
                }
            }
            dtGrid.AcceptChanges();

            foreach (DataRow row in dt2.Rows)
			{
				string moName = row["��"].ToString();
                DataRow newRow = dtGrid.NewRow();
                newRow["��"] = moName;
                if ((moName == "���������� ������������� �����") && (beloyarRow != null))
                {
                    newRow[1] = MathHelper.GeoMean(row, 1, 3, DBNull.Value, beloyarRow[1]);
                    newRow[2] = MathHelper.GeoMean(row, 2, 3, DBNull.Value, beloyarRow[2]);
                    newRow[3] = MathHelper.GeoMean(row, 3, 3, DBNull.Value, beloyarRow[3]);
                }
                else
				{
                    newRow[1] = MathHelper.GeoMean(row, 1, 3, DBNull.Value);
                    newRow[2] = MathHelper.GeoMean(row, 2, 3, DBNull.Value);
                    newRow[3] = MathHelper.GeoMean(row, 3, 3, DBNull.Value);
				}
                dtGrid.Rows.Add(newRow);
			}
            dtGrid.AcceptChanges();

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

		private string GetStarOrNot(string moName)
		{
			if (moName.Contains("���������") || moName.Contains("������") || moName.Contains("�����������") || moName.Contains("�������") ||
				moName.Contains("��������") || moName.Contains("������") || (moName.Contains("�������������") && !moName.Contains("���������������")) ||
				moName.Contains("������") || moName.Contains("����-��") || moName.Contains("������") || moName.Contains("����"))
			{
				return String.Empty;
			}
			else
			{
				return "*";
			}
		}
		
		private bool IsStatMO(string moName)
		{
			if (moName == "����� ������" || moName == "����� �����-��������" || moName == "����� �������������" || moName == "�����-���������� ���������� �����")
				return true;
			else
				return false;
		}

    }
}
