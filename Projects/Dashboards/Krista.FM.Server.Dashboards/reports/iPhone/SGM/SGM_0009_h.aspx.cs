using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.reports.SGM;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class SGM_0009_h : SGM_0009
    {
        protected override void FillComponentData()
        {
            grid.Width = 470;
            grid.Height = 270;

            DataTable dtGrid = GetGridDataSet();

            dtGrid.Columns.RemoveAt(4);
            dtGrid.Columns.RemoveAt(4);

            grid.DataSource = dtGrid;
            grid.DataBind();

            Label2.Text = string.Format("данные за {0} {1} года", supportClass.GetMonthLabelFull(months, 0), year);
            Label1.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            LabelText1.Width = 460;
            LabelText2.Width = LabelText1.Width;
            LabelText1.Text = string.Format("Общая инфекционная заболеваемость");
            LabelText2.Text = string.Format("за {0} {1}",
                supportClass.GetMonthLabelFull(months, 0), year);

            TextBoxComments1.Height = 12;
            TextBoxComments2.Height = TextBoxComments1.Height;
            TextBoxComments3.Height = TextBoxComments1.Height;
            TextBoxComments4.Height = 16;

            TextBoxComments1.Text = string.Format("Абс. = абсолютный показатель заболеваемости.");
            TextBoxComments2.Text = string.Format("На 100 тыс. = относительный показатель заболеваемости на 100");
            TextBoxComments3.Text = string.Format("тысяч населения.");
            TextBoxComments4.Text = string.Format("Ранг = ранг по относительному показателю заболеваемости.");
        }

        protected void grid_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            grid.Columns[0].Width = 183;

            for (int i = 1; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Width = 90;
                CRHelper.FormatNumberColumn(grid.Columns[i], "N0");
            }
            grid.Columns[1].Width = 85;
            grid.Columns[3].Width = 55;
            grid.Columns[4].Width = 50;

            CRHelper.FormatNumberColumn(grid.Columns[02], "N2");
            CRHelper.FormatNumberColumn(grid.Columns[06], "N2");

            grid.Columns[0].Header.Caption = "Субъект";
            grid.Columns[1].Header.Caption = "Абс.";
            grid.Columns[2].Header.Caption = "На 100 тыс.";
            grid.Columns[3].Header.Caption = string.Format("Ранг {0}", year);
            grid.Columns[4].Header.Caption = string.Format("Ранг {0}", year - 1);
            grid.Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
        }

        protected void grid_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            e.Row.Cells[1].Style.ForeColor = Color.Gray;
            e.Row.Cells[4].Style.ForeColor = Color.Gray;

            int value1 = Convert.ToInt32(e.Row.Cells[3].Value);
            int value2 = Convert.ToInt32(e.Row.Cells[4].Value);

            if (value1 == 0)
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }

            if (value1 != 0 && value2 != 0)
            {

                if (value1 < value2)
                {
                    e.Row.Cells[3].Style.BackgroundImage = "~/images/ArrowRedUpBB.png";
                }
                if (value1 > value2)
                {
                    e.Row.Cells[3].Style.BackgroundImage = "~/images/ArrowGreenDownBB.png";
                }
                e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: left; margin-left: 3px";
            }
        }
    }
}
