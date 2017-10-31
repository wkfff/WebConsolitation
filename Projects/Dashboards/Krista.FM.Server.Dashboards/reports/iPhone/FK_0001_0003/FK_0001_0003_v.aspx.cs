using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FK_0001_0003_v : CustomReportPage
    {
        private DataTable dt;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0003_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
            UserParams.PeriodLastYear.Value = (Convert.ToInt32(dtDate.Rows[0][0]) - 1).ToString();
            UserParams.PeriodYear.Value = dtDate.Rows[0][0].ToString();

            int monthNum = CRHelper.MonthNum(dtDate.Rows[0][3].ToString());
            int yearNum = Convert.ToInt32(dtDate.Rows[0][0]);
            string monthStr;
            switch (monthNum)
            {
                case 1:
                    {
                        monthStr = "мес€ц";
                        break;
                    }
                case 2:
                case 3:
                case 4:
                    {
                        monthStr = "мес€ца";
                        break;
                    }
                default:
                    {
                        monthStr = "мес€цев";
                        break;
                    }
            }

            Label3.Text = string.Format("ѕроцент исполнени€ плана по расходам и бюджетные расходы на душу населени€ (тыс.руб./чел.) за {0}&nbsp;{1}&nbsp;{2}&nbsp;года по субъектам {3}. –анг субъекта в {3}.",
                                        monthNum,
                                        monthStr,
                                        yearNum,
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

            UltraWebGrid.DataBind();
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0003_v");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[2] != DBNull.Value)
                {
                    row[2] = Convert.ToDouble(row[2]) * 100;
                }
                if (row[5] != DBNull.Value)
                {
                    row[5] = Convert.ToDouble(row[5]) / 1000;
                }
            }

            UltraWebGrid.DataSource = dt;
        }

        private static void AddColumnHeader(UltraGridLayout layout, string caption, int originX, int originY, int spanX)
        {
            ColumnHeader ch = new ColumnHeader(true);
            ch.Caption = caption;
            ch.RowLayoutColumnInfo.OriginY = originX;
            ch.RowLayoutColumnInfo.OriginX = originY;
            ch.RowLayoutColumnInfo.SpanX = spanX;
            ch.Style.Padding.Top = 1;
            ch.Style.Padding.Bottom = 1;
            ch.Style.Height = 5;
            ch.Style.VerticalAlign = VerticalAlign.Middle;
            layout.Bands[0].HeaderLayout.Add(ch);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            //e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;
            e.Layout.RowStyleDefault.Wrap = false;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 7)
            {
                                foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
                {
                    c.Header.RowLayoutColumnInfo.OriginY = 1;
                }

                AddColumnHeader(e.Layout, "—убъекты", 0, 1, 1);
                AddColumnHeader(e.Layout, "»сполнено", 0, 2, 2);
                AddColumnHeader(e.Layout, "Ѕюдж.расх. на душу населени€", 0, 4, 2);

                e.Layout.Bands[0].Columns[0].Hidden = true;
                e.Layout.Bands[0].Columns[4].Hidden = true;
                e.Layout.Bands[0].Columns[7].Hidden = true;

                e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].HeaderStyle.Height = 10;

                e.Layout.Bands[0].Columns[1].Header.Caption = "";
                e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.OriginX = 1;
                e.Layout.Bands[0].Columns[2].Header.Caption = "%";
                e.Layout.Bands[0].Columns[2].Header.RowLayoutColumnInfo.OriginX = 2;
                e.Layout.Bands[0].Columns[3].Header.Caption = "ранг";
                e.Layout.Bands[0].Columns[3].Header.RowLayoutColumnInfo.OriginX = 3;
                e.Layout.Bands[0].Columns[5].Header.Caption = "тыс.руб.";
                e.Layout.Bands[0].Columns[5].Header.RowLayoutColumnInfo.OriginX = 5;
                e.Layout.Bands[0].Columns[6].Header.Caption = "ранг";
                e.Layout.Bands[0].Columns[6].Header.RowLayoutColumnInfo.OriginX = 6;

                e.Layout.Bands[0].Columns[3].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[6].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].HeaderStyle.Height = 5;
                for (int i = 2; i <= 7; i++)
                {
                    e.Layout.Bands[0].Columns[i].Header.Style.VerticalAlign = VerticalAlign.Top;
                    e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("14px");
                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Top = 1;
                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Bottom = 1;
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N0");

                e.Layout.Bands[0].Columns[1].Width = 91;
                e.Layout.Bands[0].Columns[2].Width = 53;
                e.Layout.Bands[0].Columns[3].Width = 48;
                e.Layout.Bands[0].Columns[5].Width = 66;
                e.Layout.Bands[0].Columns[6].Width = 51;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BorderColor = Color.FromArgb(50, 50, 50);

                if (i == 1)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthLeft = 3;
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 3;
                }
                else
                {
                    e.Row.Cells[i].Style.Padding.Right = 2;
                    e.Row.Cells[i].Style.Padding.Left = 1;
                }

                if (i == 3 || i == e.Row.Cells.Count - 2)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 3;
                }

                if (e.Row.Index == 2 || e.Row.Index == dt.Rows.Count - 1)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 3;
                }

                if (i == 3 || i == 6)
                {
                    string img = string.Empty;
                    if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                    {
                        img = "~/images/starYellow.png";
                    }
                    else if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value) &&
                             Convert.ToInt32(e.Row.Cells[i].Value) != 0)
                    {
                        img = "~/images/starGray.png";
                    }

                    e.Row.Cells[i].Style.VerticalAlign = VerticalAlign.Middle;
                    e.Row.Cells[i].Style.Padding.Top = 1;
                    e.Row.Cells[i].Style.Padding.Bottom = 1;
                    e.Row.Cells[i].Style.BackgroundImage = img;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; padding-top: 2px";
                }
            }
        }
    }
}
