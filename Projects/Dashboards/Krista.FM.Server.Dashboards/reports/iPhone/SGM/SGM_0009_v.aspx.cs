using System;
using System.Data;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.reports.SGM;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class SGM_0009_v : SGM_0009
    {
        protected override void FillComponentData()
        {            
            DataTable dtGrid = GetGridDataSet();

            dtGrid.Columns.RemoveAt(1);
            dtGrid.Columns.RemoveAt(3);
            dtGrid.Columns.RemoveAt(3);
            dtGrid.Columns.RemoveAt(3);

            grid.Height = 273;
            grid.DataSource = dtGrid;
            grid.DataBind();

            LabelPageTitle.Width = 310;
            LabelPageTitle.Text = string.Format("����� ������������ ��������������<br>�� {0} {1}",
                supportClass.GetMonthLabelFull(months, 0), year);


            Label2.Text = string.Format("������ �� {0} {1} ����", supportClass.GetMonthLabelFull(months, 0), year);
            Label1.Text = string.Format("��������� {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            LabelComments.Text = string.Format(@"<br>���. = ���������� ���������� ��������������.<br>
                �� 100 ���. = ������������� ���������� �������������� �� 100 ����� ���������.<br>
                ���� = ���� �� �������������� ���������� ��������������.");
        }

        protected void grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            grid.Width = 310;
            grid.Columns[0].Width = 170;
            grid.Columns[1].Width = 90;
            grid.Columns[2].Width = 45;

            CRHelper.FormatNumberColumn(grid.Columns[01], "N2");

            grid.Columns[0].Header.Caption = "�������";
            grid.Columns[1].Header.Caption = "�� 100 ���.";
            grid.Columns[2].Header.Caption = "����";
            grid.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
        }

        protected void grid_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            if (Convert.ToInt32(e.Row.Cells[2].Value) == 0)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }   
        }
    }
}
