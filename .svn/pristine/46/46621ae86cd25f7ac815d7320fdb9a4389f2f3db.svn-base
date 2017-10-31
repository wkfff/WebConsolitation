using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.reports.SGM;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class SGM_0006_v : SGM_0006
    {
        protected override void FillChartData()
        {
        }

        protected override void FillComponentData()
        {
            DataTable dtGrid = dataObject.CloneDataTable(dtFullData);

            dtGrid.Columns.RemoveAt(1);
            dtGrid.Columns.RemoveAt(2);
            dtGrid.Columns.RemoveAt(2);
            dtGrid.Columns.RemoveAt(2);
            dtGrid.Columns.RemoveAt(2);
            dtGrid.Columns.RemoveAt(2);
            dtGrid.Columns.RemoveAt(2);
            dtGrid.Columns.RemoveAt(2);

            grid.Height = Unit.Empty;

            grid.DataSource = dtGrid;
            grid.DataBind();

            LabelPageTitle.Width = 310;
            LabelPageTitle.Text = String.Format("–ост\\снижение заболеваемости в {0}<br>за {1} {2} года к аналогичному<br>периоду прошлого года",
                RegionsNamingHelper.ShortName(UserParams.ShortStateArea.Value), supportClass.GetMonthLabelShort(months), curYear);

            LabelGeneratedV2.Text = String.Format("данные за {0} {1} года", supportClass.GetMonthLabelFull(months, 0), curYear);
            LabelGeneratedV1.Text = String.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            LabelComments.Text = String.Format(@"Ќа 100 тыс. = относительный показатель заболеваемости на 100 тыс€ч населени€.<br>–ост/снижение = динамика заболеваемости по сравнению с аналогичным периодом предыдущего года.");
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            grid.Width = 310;
            grid.Columns[0].Width = 140;
            grid.Columns[1].Width = 75;
            grid.Columns[2].Width = 94;

            CRHelper.FormatNumberColumn(grid.Columns[01], "N1");
            CRHelper.FormatNumberColumn(grid.Columns[02], "N1");

            grid.Columns[0].Header.Caption = "«аболевание";
            grid.Columns[1].Header.Caption = "Ќа 100 тыс.";
            grid.Columns[2].Header.Caption = "–ост \\ снижение";
            grid.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            double value = Convert.ToDouble(dtFullData.Rows[e.Row.Index][9]);
            UltraGridCell cell = e.Row.Cells[2];
            cell.Style.BackgroundImage = value > 1 ? "~/images/cornerRed.gif" : "~/images/cornerGreen.gif";
            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: right top";
        }
    }
}
