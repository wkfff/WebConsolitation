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
            LabelPageTitle.Text = string.Format("Общая инфекционная заболеваемость<br>за {0} {1}",
                supportClass.GetMonthLabelFull(months, 0), year);


            Label2.Text = string.Format("данные за {0} {1} года", supportClass.GetMonthLabelFull(months, 0), year);
            Label1.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            LabelComments.Text = string.Format(@"<br>Абс. = абсолютный показатель заболеваемости.<br>
                На 100 тыс. = относительный показатель заболеваемости на 100 тысяч населения.<br>
                Ранг = ранг по относительному показателю заболеваемости.");
        }

        protected void grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            grid.Width = 310;
            grid.Columns[0].Width = 170;
            grid.Columns[1].Width = 90;
            grid.Columns[2].Width = 45;

            CRHelper.FormatNumberColumn(grid.Columns[01], "N2");

            grid.Columns[0].Header.Caption = "Субъект";
            grid.Columns[1].Header.Caption = "На 100 тыс.";
            grid.Columns[2].Header.Caption = "Ранг";
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
