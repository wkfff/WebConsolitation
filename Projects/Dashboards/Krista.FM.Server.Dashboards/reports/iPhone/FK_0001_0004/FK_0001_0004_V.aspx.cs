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
    public partial class FK_0001_0004_V : CustomReportPage
    {
        private DataTable dt;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0004_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();

            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string monthStr;
            switch (monthNum)
            {
                case 1:
                    {
                        monthStr = "месяц";
                        break;
                    }
                case 2:
                case 3:
                case 4:
                    {
                        monthStr = "месяца";
                        break;
                    }
                default:
                    {
                        monthStr = "месяцев";
                        break;
                    }
            }

            Label3.Text = string.Format("Темп роста по налогу на прибыль и НДФЛ за {0}&nbsp;{3}&nbsp;{1}&nbsp;года к аналогичному периоду предыдущего года по {2} и субъектам {4}",
                            monthNum,
                            yearNum,
                            UserParams.ShortStateArea.Value,
                            monthStr,
                            UserParams.ShortRegion.Value);

            if (monthNum == 12)
            {
                monthNum = 1;
                yearNum++;
            }
            else
            {
                monthNum++;
            }

            Label1.Text = string.Format("данные на 1 {0} {1} года", CRHelper.RusMonthGenitive(monthNum), yearNum);
            Label2.Text = string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);
            Label4.Text = "доля % = доля налога в бюджете";

            UltraWebGrid.DataBind();
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0004_V");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            foreach (DataRow row in dt.Rows)
            {
                row[0] = row[0].ToString().TrimEnd('_');
                for (int i = 2; i < dt.Columns.Count; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i]) * 100;
                    }
                }
            }

            UltraWebGrid.DataSource = dt;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;
            e.Layout.RowStyleDefault.Wrap = false;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 5)
            {
                foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
                {
                    c.Header.RowLayoutColumnInfo.OriginY = 1;
                }

                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = "Субъекты";
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 1;
                ch.RowLayoutColumnInfo.SpanX = 1;

                ch.Style.Padding.Top = 1;
                ch.Style.Padding.Bottom = 1;
                ch.Style.Height = 5;

                e.Layout.Bands[0].HeaderLayout.Add(ch);

                ch = new ColumnHeader(true);
                ch.Caption = "Налог на прибыль";
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 2;
                ch.RowLayoutColumnInfo.SpanX = 2;

                ch.Style.Padding.Top = 1;
                ch.Style.Padding.Bottom = 1;
                ch.Style.Height = 5;

                e.Layout.Bands[0].HeaderLayout.Add(ch);

                ch = new ColumnHeader(true);
                ch.Caption = "НДФЛ";
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 4;
                ch.RowLayoutColumnInfo.SpanX = 2;

                ch.Style.Padding.Top = 1;
                ch.Style.Padding.Bottom = 1;
                ch.Style.Height = 5;

                e.Layout.Bands[0].HeaderLayout.Add(ch);

                e.Layout.Bands[0].Columns[0].Hidden = true;

                e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].HeaderStyle.Height = 10;
                
                e.Layout.Bands[0].Columns[1].Header.Caption = "";
                e.Layout.Bands[0].Columns[2].Header.Caption = "доля %";
                e.Layout.Bands[0].Columns[2].Header.RowLayoutColumnInfo.OriginX = 2;
                e.Layout.Bands[0].Columns[3].Header.Caption = "темп роста %";
                e.Layout.Bands[0].Columns[3].Header.RowLayoutColumnInfo.OriginX = 3;
                e.Layout.Bands[0].Columns[4].Header.Caption = "доля %";
                e.Layout.Bands[0].Columns[4].Header.RowLayoutColumnInfo.OriginX = 4;
                e.Layout.Bands[0].Columns[5].Header.Caption = "темп роста %";
                e.Layout.Bands[0].Columns[5].Header.RowLayoutColumnInfo.OriginX = 5;

                e.Layout.Bands[0].Columns[2].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[4].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].HeaderStyle.Height = 5;
                for (int i = 2; i <= 5; i++)
                {
                    e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("12px");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");

                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Top = 1;
                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Bottom = 1;
                    //e.Layout.Bands[0].Columns[i].Header.Style.VerticalAlign = VerticalAlign.Top;
                }

                e.Layout.Bands[0].Columns[1].Width = 93;
                e.Layout.Bands[0].Columns[2].Width = 52;
                e.Layout.Bands[0].Columns[3].Width = 56;
                e.Layout.Bands[0].Columns[4].Width = 52;
                e.Layout.Bands[0].Columns[5].Width = 56;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BorderColor = Color.FromArgb(50, 50, 50);

                if (i != 1)
                {
                    e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Right;
                    e.Row.Cells[i].Style.Padding.Right = 2;
                    e.Row.Cells[i].Style.Padding.Left = 1;
                }

                if (i % 2 == 0 || i == 1)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthLeft = 3;
                }

                if (i == e.Row.Cells.Count - 1)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 3;
                }

                if (e.Row.Index == 2)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 3;
                }
            }
        }
    }
}
