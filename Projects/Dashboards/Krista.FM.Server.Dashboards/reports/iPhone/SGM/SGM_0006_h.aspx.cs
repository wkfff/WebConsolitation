using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.reports.SGM;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class SGM_0006_h : SGM_0006
    {

        protected override void FillChartData()
        {
        }

        protected override void FillComponentData()
        {
            DataTable dtGrid = dtFullData;

            dtGrid.Columns.RemoveAt(1);
            dtGrid.Columns.RemoveAt(2);
            dtGrid.Columns.RemoveAt(2);
            dtGrid.Columns.RemoveAt(2);
            dtGrid.Columns.RemoveAt(3);
            dtGrid.Columns.RemoveAt(4);
            dtGrid.Columns.RemoveAt(4);

            grid.Width = 475;
            grid.Height = 250;

            grid.DataSource = dtGrid;
            grid.DataBind();

            LabelText1.Width = 470;
            LabelText1.Text = string.Format("Сравнение заболеваемости в {0} за {1} {2} года",
                RegionsNamingHelper.ShortName(UserParams.ShortStateArea.Value), supportClass.GetMonthLabelShort(months), curYear);

            LabelText2.Width = LabelText1.Width;
            LabelText2.Text = "с показателями по стране и федеральному округу";

            LabelGenerated2.Text = String.Format("данные за {0} {1} года", supportClass.GetMonthLabelFull(months, 0), curYear);
            LabelGenerated1.Text = String.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            TextBoxComments1.Height = 14;
            TextBoxComments2.Height = TextBoxComments1.Height;

            TextBoxComments1.Text = String.Format("На 100 тыс. = относительный показатель заболеваемости на 100");
            TextBoxComments2.Text = String.Format("тысяч населения.");            
        }

        protected void grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            grid.Columns[0].Width = 170;
            grid.Columns[1].Width = 90;
            grid.Columns[2].Width = 105;
            grid.Columns[3].Width = 105;

            CRHelper.FormatNumberColumn(grid.Columns[01], "N2");
            CRHelper.FormatNumberColumn(grid.Columns[02], "N2");
            CRHelper.FormatNumberColumn(grid.Columns[03], "N2");

            grid.Columns[0].Header.Caption = "Заболевание";
            grid.Columns[1].Header.Caption = String.Format("{0}, на 100 тыс.", RegionsNamingHelper.ShortName(UserParams.ShortStateArea.Value));
            grid.Columns[2].Header.Caption = String.Format("{0}, на 100 тыс.", supportClass.GetFOShortName(foName));
            grid.Columns[3].Header.Caption = String.Format("{0}, на 100 тыс.", supportClass.GetFOShortName(rfName));
            grid.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
        }

        protected void SetCellImage(UltraGridRow row, int cellIndex1, int cellIndex2)
        {
            double value1 = Convert.ToDouble(row.Cells[cellIndex1].Value);
            double value2 = Convert.ToDouble(row.Cells[cellIndex2].Value);
            UltraGridCell cell = row.Cells[cellIndex2];
            cell.Style.BackgroundImage = value2 < value1 ? "~/images/red.png" : "~/images/green.png";
            cell.Style.CustomRules = "background-repeat: no-repeat; background-position: left; margin-left: 3px";
        }

        protected void grid_InitializeRow(object sender, RowEventArgs e)
        {
            SetCellImage(e.Row, 1, 2);
            SetCellImage(e.Row, 1, 3);

            e.Row.Cells[2].Style.ForeColor = Color.Gray;
            e.Row.Cells[3].Style.ForeColor = Color.Gray;
        }
    }
}
