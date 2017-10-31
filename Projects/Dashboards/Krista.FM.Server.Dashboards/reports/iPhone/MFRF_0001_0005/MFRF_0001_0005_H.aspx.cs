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
    public partial class MFRF_0001_0005_H : CustomReportPage
    {
        private DataTable dt;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("MFRF_0001_0005_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

            UserParams.PeriodLastYear.Value = "2010";
            UserParams.PeriodYear.Value = "2011";

            //Label4.Text = string.Format("данные до {0} года", dtDate.Rows[0][0]);
            Label4.Text = "данные на 2011 год";
            Label5.Text =
                string.Format("обновлено {0:dd.MM.yyyy HH:mm:ss}", DateTime.Now);

            UltraWebGrid.DataBind();
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("MFRF_0001_0005_h");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dt);

            string lastYear = (DateTime.Now.Year - 1).ToString();
            TextBox1.Text = string.Format("Межбюджетные трансферты {0} из Федеральных фондов", dt.Rows[0][2]);
            TextBox2.Text = string.Format("на {0} год, данные в млн.руб., темп роста к {1} году", UserParams.PeriodYear.Value, UserParams.PeriodLastYear.Value/*lastYear*/);
            
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 3; i <= 9; i++)
                {
                    if (row[i] != DBNull.Value)
                    {
                        if (i == 4 || i == 6 || i == 8)
                        {
                            row[i] = Convert.ToDouble(row[i]) * 100;
                        }
                        else
                        {
                            row[i] = Convert.ToDouble(row[i]) / 1000;
                        }
                    }
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

            if (e.Layout.Bands.Count == 0)
                return;

            if (e.Layout.Bands[0].Columns.Count > 8)
            {
                foreach (UltraGridColumn c in e.Layout.Bands[0].Columns)
                {
                    c.Header.RowLayoutColumnInfo.OriginY = 1;
                }

                AddColumnHeader(e.Layout, "Субьекты", 0, 1, 1);
                AddColumnHeader(e.Layout, "ФФФПР", 0, 2, 2);
                AddColumnHeader(e.Layout, "ФФК", 0, 4, 2);
                AddColumnHeader(e.Layout, "ФФСР", 0, 6, 2);
                AddColumnHeader(e.Layout, "Иные", 0, 8, 1);

                e.Layout.Bands[0].Columns[0].Hidden = true;
                e.Layout.Bands[0].Columns[2].Hidden = true;

                e.Layout.Bands[0].Columns[1].Header.Caption = "";
                e.Layout.Bands[0].Columns[1].Header.RowLayoutColumnInfo.OriginX = 1;
                e.Layout.Bands[0].Columns[3].Header.Caption = "сумма";
                e.Layout.Bands[0].Columns[3].Header.RowLayoutColumnInfo.OriginX = 2;
                e.Layout.Bands[0].Columns[4].Header.Caption = "темп роста %";
                e.Layout.Bands[0].Columns[4].Header.RowLayoutColumnInfo.OriginX = 3;
                e.Layout.Bands[0].Columns[5].Header.Caption = "сумма";
                e.Layout.Bands[0].Columns[5].Header.RowLayoutColumnInfo.OriginX = 4;
                e.Layout.Bands[0].Columns[6].Header.Caption = "темп роста %";
                e.Layout.Bands[0].Columns[6].Header.RowLayoutColumnInfo.OriginX = 5;
                e.Layout.Bands[0].Columns[7].Header.Caption = "сумма";
                e.Layout.Bands[0].Columns[7].Header.RowLayoutColumnInfo.OriginX = 6;
                e.Layout.Bands[0].Columns[8].Header.Caption = "темп роста %";
                e.Layout.Bands[0].Columns[8].Header.RowLayoutColumnInfo.OriginX = 7;
                e.Layout.Bands[0].Columns[9].Header.Caption = "сумма";
                e.Layout.Bands[0].Columns[9].Header.RowLayoutColumnInfo.OriginX = 8;

                e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = FontUnit.Parse("16px");
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].Columns[4].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[6].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
                e.Layout.Bands[0].Columns[8].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

                e.Layout.Bands[0].Columns[1].Width = 93;
                e.Layout.Bands[0].Columns[3].Width = 64;
                e.Layout.Bands[0].Columns[4].Width = 45;
                e.Layout.Bands[0].Columns[5].Width = 64;
                e.Layout.Bands[0].Columns[6].Width = 45;
                e.Layout.Bands[0].Columns[7].Width = 64;
                e.Layout.Bands[0].Columns[8].Width = 45;
                e.Layout.Bands[0].Columns[9].Width = 54;

                for (int i = 3; i <= 9; i++)
                {
                    if (i == 4 || i == 6 || i == 8)
                    {
                        CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N1");
                    }
                    else
                    {
                        CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                    }

                    e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("12px"); 
                    e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 2;
                    e.Layout.Bands[0].Columns[i].CellStyle.Padding.Left = 1;
                }
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BorderColor = Color.FromArgb(50, 50, 50);

                if (i % 2 != 0)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthLeft = 3;
                }
                else
                {
                    if (i == e.Row.Cells.Count - 1)
                    {
                        e.Row.Cells[i].Style.BorderDetails.WidthRight = 3;
                    }
                }

                if (e.Row.Index == 2)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthBottom = 3;
                }
            }
        }
    }
}
