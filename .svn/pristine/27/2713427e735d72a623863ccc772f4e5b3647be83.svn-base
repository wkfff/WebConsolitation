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
    public partial class FK_0001_0002_h : CustomReportPage
    {
        private DataTable dt;
        private string populationDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FK_0001_0002_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
            UserParams.PeriodLastYear.Value = (Convert.ToInt32(dtDate.Rows[0][0]) - 1).ToString();
            UserParams.PeriodYear.Value = dtDate.Rows[0][0].ToString();

            populationDate = DataDictionariesHelper.GetFederalPopulationDate();

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
            TextBox1.Text = string.Format("Доходы бюджетов, процент исполнения плана по доходам");
            TextBox5.Text = string.Format("и среднедушевые доходы (тыс.руб./чел.)");

            TextBox6.Text = string.Format("за {0} {1} {2} года по субъектам {3}.",
                monthNum,
                monthStr,
                yearNum,
                UserParams.ShortRegion.Value);
            TextBox7.Text = string.Format("Ранг субъекта в {0}.", UserParams.ShortRegion.Value);


            TextBox2.Text = string.Format("Доходы млдр.руб. = доходы консолидированных бюджетов");

            TextBox4.Text = string.Format("субъектов РФ за {0} {1} {2} года",
                monthNum,
                monthStr,
                yearNum);


            TextBox3.Text = string.Format("Население тыс.чел. = численность пост.населения {0}", populationDate);

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
            string query = DataProvider.GetQueryText("FK_0001_0002_h");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            foreach (DataRow row in dt.Rows)
            {
                if (row[2] != DBNull.Value)
                {
                    row[2] = Convert.ToDouble(row[2]) / 1000000000;
                }
                if (row[3] != DBNull.Value)
                {
                    row[3] = Convert.ToDouble(row[3]) * 100;
                }
                if (row[6] != DBNull.Value)
                {
                    row[6] = Convert.ToDouble(row[6]);
                }
                if (row[7] != DBNull.Value)
                {
                    row[7] = Convert.ToDouble(row[7]) / 1000;
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

            if (e.Layout.Bands[0].Columns.Count > 8)
            {
                foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
                {
                    c.Header.RowLayoutColumnInfo.OriginY = 1;
                }

                AddColumnHeader(e.Layout, "Субъекты", 0, 1, 1);
                AddColumnHeader(e.Layout, "Доходы", 0, 2, 1);
                AddColumnHeader(e.Layout, "Исполнено", 0, 3, 2);
                AddColumnHeader(e.Layout, "Население", 0, 5, 1);
                AddColumnHeader(e.Layout, "Среднедуш. доходы", 0, 6, 2);

                e.Layout.Bands[0].Columns[0].Hidden = true;
                e.Layout.Bands[0].Columns[5].Hidden = true;
                e.Layout.Bands[0].Columns[9].Hidden = true;

                e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].HeaderStyle.Height = 5;

                e.Layout.Bands[0].Columns[4].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[8].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].Columns[1].Header.Caption = "";
                e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.OriginX = 1;
                e.Layout.Bands[0].Columns[2].Header.Caption = "млдр.руб.";
                e.Layout.Bands[0].Columns[2].Header.RowLayoutColumnInfo.OriginX = 2;
                e.Layout.Bands[0].Columns[3].Header.Caption = "%";
                e.Layout.Bands[0].Columns[3].Header.RowLayoutColumnInfo.OriginX = 3;
                e.Layout.Bands[0].Columns[4].Header.Caption = "ранг";
                e.Layout.Bands[0].Columns[4].Header.RowLayoutColumnInfo.OriginX = 4;
                e.Layout.Bands[0].Columns[6].Header.Caption = "тыс.чел.";
                e.Layout.Bands[0].Columns[6].Header.RowLayoutColumnInfo.OriginX = 6;
                e.Layout.Bands[0].Columns[7].Header.Caption = "тыс.руб.";
                e.Layout.Bands[0].Columns[7].Header.RowLayoutColumnInfo.OriginX = 7;
                e.Layout.Bands[0].Columns[8].Header.Caption = "ранг";
                e.Layout.Bands[0].Columns[8].Header.RowLayoutColumnInfo.OriginX = 8;

                for (int i = 2; i <= 8; i++)
                {
                    e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("14px");

                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Top = 1;
                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Bottom = 1;
                    e.Layout.Bands[0].Columns[i].Header.Style.Height = 5;
                    e.Layout.Bands[0].Columns[i].Header.Style.VerticalAlign = VerticalAlign.Top;
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N2");
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "N0");

                e.Layout.Bands[0].Columns[1].Width = 93;
                e.Layout.Bands[0].Columns[2].Width = 71;
                e.Layout.Bands[0].Columns[3].Width = 53;
                e.Layout.Bands[0].Columns[4].Width = 52;
                e.Layout.Bands[0].Columns[6].Width = 89;
                e.Layout.Bands[0].Columns[7].Width = 62;
                e.Layout.Bands[0].Columns[8].Width = 52;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BorderColor = Color.FromArgb(50, 50, 50);
                e.Row.Cells[i].Style.Padding.Top = 7;

                if (i == 1)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthLeft = 3;
                }
                else
                {
                    e.Row.Cells[i].Style.Padding.Right = 2;
                    e.Row.Cells[i].Style.Padding.Left = 1;
                }

                if (i == 1 || i == 2 || i == 4 || i == 6)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 3;
                }

                if (i == e.Row.Cells.Count - 2)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 3;
                }

                if (e.Row.Index == 2 || e.Row.Index == dt.Rows.Count - 1)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 3;
                }

                if (i == 4 || i == 8)
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
