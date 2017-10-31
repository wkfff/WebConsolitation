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
    public partial class FK_0001_0004 : CustomReportPage
    {
        DataTable dt = new DataTable();

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


            Label1.Text = string.Format("Темп роста фактических доходов за {0}&nbsp;{3}&nbsp;{1}&nbsp;года к аналогичному периоду предыдущего года по {2}",
                                        monthNum,
                                        yearNum,
                                        UserParams.ShortStateArea.Value,
                                        monthStr);
            UltraWebGrid.DataBind();
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0004");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            foreach (DataRow row in dt.Rows)
            {
                row[0] = row[0].ToString().TrimEnd('_');
                for (int i = 1; i < dt.Columns.Count; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        row[i] = Convert.ToDouble(row[i])*100;
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
            //e.Layout.RowStyleDefault.Wrap = false;

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 4)
            {
                foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
                {
                    c.Header.RowLayoutColumnInfo.OriginY = 1;
                }

                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = "Доля в суб";
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 1;
                ch.RowLayoutColumnInfo.SpanX = 1;
                ch.Style.Font.Size = FontUnit.Parse("14px");

                ch.Style.Padding.Top = 1;
                ch.Style.Padding.Bottom = 1;
                ch.Style.Height = 5;

                e.Layout.Bands[0].HeaderLayout.Add(ch);
                
                ch = new ColumnHeader(true);
                ch.Caption = "Темп роста к прошлому году %";
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = 2;
                ch.RowLayoutColumnInfo.SpanX = 3;
                ch.Style.Font.Size = FontUnit.Parse("14px");

                ch.Style.Padding.Top = 1;
                ch.Style.Padding.Bottom = 1;
                ch.Style.Height = 5;

                e.Layout.Bands[0].HeaderLayout.Add(ch);
                
                e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[0].Header.Style.Padding.Top = 1;

                e.Layout.Bands[0].Columns[1].Header.Caption = "%";
                e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.OriginX = 1;
                e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[2].Header.Caption = "суб";
                e.Layout.Bands[0].Columns[2].Header.RowLayoutColumnInfo.OriginX = 2;
                e.Layout.Bands[0].Columns[3].Header.Caption = UserParams.ShortRegion.Value;
                e.Layout.Bands[0].Columns[3].Header.RowLayoutColumnInfo.OriginX = 3;
                e.Layout.Bands[0].Columns[4].Header.Caption = "РФ";
                e.Layout.Bands[0].Columns[4].Header.RowLayoutColumnInfo.OriginX = 4;

                e.Layout.Bands[0].HeaderStyle.Height = 5;
                for (int i = 1; i <= 4; i++)
                {
                    e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("14px");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N1");

                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Top = 1;
                    e.Layout.Bands[0].Columns[i].Header.Style.Padding.Bottom = 1;
                    e.Layout.Bands[0].Columns[i].Header.Style.VerticalAlign = VerticalAlign.Top;
                    e.Layout.Bands[0].Columns[0].CellStyle.Wrap = false;
                }

                e.Layout.Bands[0].Columns[0].Width = 105;
                e.Layout.Bands[0].Columns[1].Width = 55;
                e.Layout.Bands[0].Columns[2].Width = 51;
                e.Layout.Bands[0].Columns[3].Width = 51;
                e.Layout.Bands[0].Columns[4].Width = 51;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells.Count > 4 && e.Row.Index < dt.Rows.Count - 1)
            {
                if (Convert.ToDouble(e.Row.Cells[2].Value) < 100 ||
                    Convert.ToDouble(e.Row.Cells[2].Value) < Convert.ToDouble(e.Row.Cells[3].Value) ||
                    Convert.ToDouble(e.Row.Cells[2].Value) < Convert.ToDouble(e.Row.Cells[4].Value))
                {
                    e.Row.Cells[2].Style.BackgroundImage = "~/images/CornerRed.gif";
                    e.Row.Cells[2].Style.CustomRules = "background-repeat: no-repeat; background-position: right top";
                }
                else
                {
                    e.Row.Cells[2].Style.BackgroundImage = "~/images/CornerGreen.gif";
                    e.Row.Cells[2].Style.CustomRules = "background-repeat: no-repeat; background-position: right top";
                }
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                int index = e.Row.Index;
                if (i == 0 && (index == 1 || index == 2 || index == 3 || index == 4))
                {
                    if (index != 2)
                    {
                        e.Row.Cells[i].Value = e.Row.Cells[i].Value.ToString().ToLower();
                    }
                    e.Row.Cells[i].Style.Padding.Left = 20;
                }
                else
                {
                    switch (i)
                    {
                        case 2 :
                            {
                                e.Row.Cells[i].Style.Padding.Right = 8;
                                break;
                            }
                        case 3:
                            {
                                e.Row.Cells[i].Style.Padding.Right = 2;
                                break;
                            }
                        default :
                            {
                                e.Row.Cells[i].Style.Padding.Right = 4;
                                break;
                            }
                    }
                    e.Row.Cells[i].Style.Padding.Left = 1;
                }
                e.Row.Cells[i].Style.Padding.Top = 1;
                e.Row.Cells[i].Style.Padding.Bottom = 1;
            }
        }
    }
}
