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
    public partial class FO_0002_0061_1 : CustomReportPage
    {
        private DataTable dtTable;
        private GridHeaderLayout headerLayout;
        private int endYear;

        private decimal max1 = 0;
        private decimal max2 = 0;
        private decimal max3 = 0;
        private decimal min1 = 0;
        private decimal min2 = 0;
        private decimal min3 = 0;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0061_rests_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            string dimension = "";

            endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            DateTime newDate = new DateTime(endYear, 1, 1);
            dimension = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период]", newDate, 1);
            UserParams.PeriodDimension.Value = dimension;

            dtTable = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0061_table");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Муниципальное образование", dtTable);

            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);

            headerLayout = new GridHeaderLayout(UltraWebGrid);

            UltraWebGrid.DataSource = dtTable;
            //Занимаемся поиском минимального/максимального значения

            if (dtTable.Rows[0]["Расчетная мера_1"] != DBNull.Value) { min1 = (decimal)dtTable.Rows[0]["Расчетная мера_1"]; }
            if (dtTable.Rows[0]["Расчетная мера_2"] != DBNull.Value) { min2 = (decimal)dtTable.Rows[0]["Расчетная мера_2"]; }
            if (dtTable.Rows[0]["Расчетная мера_3"] != DBNull.Value) { min3 = (decimal)dtTable.Rows[0]["Расчетная мера_3"]; }
            for (int i = 0; i < dtTable.Rows.Count - 1; i++)
            {
                object val1 = dtTable.Rows[i]["Расчетная мера_1"];
                object val2 = dtTable.Rows[i]["Расчетная мера_2"];
                object val3 = dtTable.Rows[i]["Расчетная мера_3"];
                if (val1 != DBNull.Value)
                {
                    if ((decimal)val1 > max1)
                    { max1 = (decimal)val1; }

                    if ((decimal)val1 < min1)
                    { min1 = (decimal)val1; }
                }
                if (val2 != DBNull.Value)
                {
                    if ((decimal)val2 > max2)
                    { max2 = (decimal)val2; }
                    else
                        if ((decimal)val2 < min2)
                        { min2 = (decimal)val2; }
                }
                if (val3 != DBNull.Value)
                {
                    if ((decimal)val3 > max3)
                    { max3 = (decimal)val3; }
                    else
                        if ((decimal)val3 < min3)
                        { min3 = (decimal)val3; }
                }
            }

            UltraWebGrid.DataBind();

            UltraWebGrid.Width = Unit.Empty;

            lbDescription.Text =
                String.Format(
                    "Удельный вес расходов на содержание органов местного самоуправления в общем объеме расходов местного бюджета за&nbsp;<b><span class='DigitsValue'>{0}</span></b>&nbsp;год, тыс.руб.",
                    endYear);
        }

        void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            setMinMax(e, 2, min1, max1);
            setMinMax(e, 3, min2, max2);
            setMinMax(e, 4, min3, max3);

            if (e.Row.Cells[5].Value != null)
            {
                e.Row.Cells[2].Value = String.Format("{0:P2}<br/><span class='ServeText' style='font-size: 12pt'>{1:N2}</span>", e.Row.Cells[5].Value, e.Row.Cells[2].Value);
            }
            if (e.Row.Cells[6].Value != null)
            {
                e.Row.Cells[3].Value = String.Format("{0:P2}<br/><span class='ServeText' style='font-size: 12pt'>{1:N2}</span>", e.Row.Cells[6].Value, e.Row.Cells[3].Value);
            }
            if (e.Row.Cells[7].Value != null)
            {
                e.Row.Cells[4].Value = String.Format("{0:P2}<br/><span class='ServeText' style='font-size: 12pt'>{1:N2}</span>", e.Row.Cells[7].Value, e.Row.Cells[4].Value);
            }
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {

                e.Row.Cells[i].Style.BorderColor = Color.FromArgb(50, 50, 50);
                e.Row.Cells[i].Style.Padding.Bottom = 2;
                e.Row.Cells[i].Style.Padding.Top = 2;
                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;
                e.Row.Cells[0].Style.Padding.Right = 2;
            }
            if (e.Row.Cells[0].Value.ToString().ToLower() == "итого")
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
            }
        }

        private int borderWidth = 3;

        void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            UltraWebGrid.Columns[0].Width = 180;
            UltraWebGrid.Columns[1].Width = 143;
            UltraWebGrid.Columns[2].Width = 143;
            UltraWebGrid.Columns[3].Width = 143;
            UltraWebGrid.Columns[4].Width = 143;
            //скроем ненужных нам 3 поля
            UltraWebGrid.Columns[5].Hidden = true;
            UltraWebGrid.Columns[6].Hidden = true;
            UltraWebGrid.Columns[7].Hidden = true;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N2");

            e.Layout.Bands[0].HeaderStyle.BorderColor = Color.FromArgb(60, 60, 60);

            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            headerLayout.AddCell("Муниципальное образование");
            headerLayout.AddCell("Итого расходов");
            GridHeaderCell Cell = headerLayout.AddCell("Расходы на содержание органов местного самоуправления");
            Cell.AddCell("Всего");
            GridHeaderCell Cell2 = Cell.AddCell("В том числе");
            Cell2.AddCell("Оплата труда");
            Cell2.AddCell("Материальные затраты");
            headerLayout.ApplyHeaderInfo();

        }

        void setMinMax(RowEventArgs e, int i, decimal min, decimal max)
        {

            string img = "";
            decimal value = (decimal)e.Row.Cells[i + 3].Value;
            //string title;
            if (min == value)
            {
                img = "~/images/min.png";
            }
            if (max == value)
            { img = "~/images/max.png"; }
            e.Row.Cells[i].Style.BackgroundImage = img;
            if (img == "~/images/max.png")
            { e.Row.Cells[i].Title = "Максимальный удельный вес расходов на содержание ОМС в общем объеме расходов местного бюджета"; }
            else
                if (img == "~/images/min.png")
                {
                    e.Row.Cells[i].Title = "Минимальный удельный вес расходов на содержание ОМС в общем объеме расходов местного бюджета";
                }
            e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: 5px top; padding-left: 2px";
        }
    }
}