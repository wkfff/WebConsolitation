using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Drawing;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class OIL_0004_0011 : CustomReportPage
    {
        private DateTime startDate;
        private DateTime lastDate;
        private DateTime currentDate;

        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("OIL_0004_0011_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtDate);

            
            lastDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[1][1].ToString(), 3);
            currentDate = CRHelper.DateByPeriodMemberUName(dtDate.Rows[2][1].ToString(), 3);
            startDate = new DateTime(currentDate.Year - 1, 12, 30);

            UserParams.PeriodCurrentDate.Value = dtDate.Rows[2][1].ToString();
            UserParams.PeriodLastDate.Value = dtDate.Rows[1][1].ToString();
            UserParams.PeriodFirstYear.Value = CRHelper.PeriodMemberUName("[Период__Период].[Период__Период].[Данные всех периодов]", startDate, 5);

            InitializeTable1();
        }


        #region Таблица1
        private DataTable dt;

        private void InitializeTable1()
        {

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);

            dt = new DataTable();
            string query = DataProvider.GetQueryText("OIL_0004_0011_Grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            DataTable dtSource = new DataTable();
            for (int i = 0; i < 8; i++)
            {
                dtSource.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dtSource.NewRow();

                row[0] = dt.Rows[i][0].ToString();
                row[1] = String.Format("{0:N2}", dt.Rows[i][1]);

                row[2] = dt.Rows[i][2];

                double value;
                if (Double.TryParse(dt.Rows[i][3].ToString(), out value))
                {
                    string absoluteGrown = value > 0
                                               ? String.Format("+{0:N2}", value)
                                               : String.Format("{0:N2}", value);

                    string img = String.Empty;
                    if (value != 0)
                    {
                        img = value > 0
                                  ? "<img src='../../../images/arrowRedUpBB.png'>"
                                  : "<img src='../../../images/arrowGreenDownBB.png'>";
                    }

                    row[3] = String.Format("<table><tr><td style='width: 30px; border: none'>{0}</td><td style='width: 70px; border: none'>{1}</td></tr></table>", img, absoluteGrown);
                }

                if (Double.TryParse(dt.Rows[i][4].ToString(), out value))
                {
                    string absoluteGrown = value > 0
                                               ? String.Format("+{0:N2}%", value)
                                               : String.Format("{0:N2}%", value);                   

                    row[4] = String.Format("{0}", absoluteGrown);
                }

                if (Double.TryParse(dt.Rows[i][5].ToString(), out value))
                {
                    string absoluteGrown = value > 0
                                               ? String.Format("+{0:N2}", value)
                                               : String.Format("{0:N2}", value);

                    string img = String.Empty;
                    if (value != 0)
                    {
                        img = value > 0
                                  ? "<img src='../../../images/arrowRedUpBB.png'>"
                                  : "<img src='../../../images/arrowGreenDownBB.png'>";
                    }

                    row[5] = String.Format("<table><tr><td style='width: 30px; border: none'>{0}</td><td style='width: 70px; border: none'>{1}</td></tr></table>", img, absoluteGrown);
                }

                if (Double.TryParse(dt.Rows[i][6].ToString(), out value))
                {
                    string absoluteGrown = value > 0
                                               ? String.Format("+{0:N2}%", value)
                                               : String.Format("{0:N2}%", value);

                    row[6] = String.Format("{0}", absoluteGrown);
                }
                row[7] = dt.Rows[i][7].ToString();
                dtSource.Rows.Add(row);
            }


            UltraWebGrid1.DataSource = dtSource;
            UltraWebGrid1.DataBind();
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            iPadBricks.iPadBricks.iPadBricksHelper.SetRankImage(e, 2, 7, false, "background-repeat: no-repeat; background-position: 10px center; padding-left: 2px; padding-right: 5px", 2, "~/images/min.png", "~/images/max.png");

            if (e.Row.Cells[0].Value.ToString().ToLower().Contains("федер"))
            {
                e.Row.Style.BorderDetails.WidthTop = borderWidth;
                if (e.Row.Cells[0].Value.ToString().ToLower().Contains("округ"))
                {
                    e.Row.Cells[0].Value =
                        String.Format(
                            "<a style='color: White' href='webcommand?showReport=OIL_0004_0002_FO={1}'>{0}</a>",
                            e.Row.Cells[0].Value, CustomParams.GetFOIdByName(e.Row.Cells[0].Value.ToString()));
                }
                e.Row.Cells[0].Style.ForeColor = Color.White;
            }
            else
            {
                e.Row.Cells[0].Value = String.Format("<a style='color: #d2d2d2' href='webcommand?showReport=OIL_0004_0003_{1}'>{0}</a>", e.Row.Cells[0].Value, CustomParams.GetSubjectIdByName(e.Row.Cells[0].Value.ToString()));
            }
        }

        private GridHeaderLayout headerLayout1;
        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.Bands[0].Grid.Width = Unit.Empty;
            e.Layout.Bands[0].Grid.Height = Unit.Empty;
            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);

            headerLayout1.AddCell(" ");
            headerLayout1.AddCell(String.Format("Средняя розничная цена на {0:dd.MM.yy}, руб.", currentDate));
            headerLayout1.AddCell("Ранг");

            GridHeaderCell headerCell = headerLayout1.AddCell(String.Format("Изменение к {0:dd.MM.yy}", lastDate));
            headerCell.AddCell("абс.");
            headerCell.AddCell("прирост цены");

            headerCell = headerLayout1.AddCell(String.Format("Изменение к {0:dd.MM.yy}", startDate));
            headerCell.AddCell("абс.");
            headerCell.AddCell("прирост цены");

            headerLayout1.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;            
            e.Layout.Bands[0].Columns[0].Width = 205;
            e.Layout.Bands[0].Columns[1].Width = 100;
            e.Layout.Bands[0].Columns[2].Width = 90;
            e.Layout.Bands[0].Columns[3].Width = 90;
            e.Layout.Bands[0].Columns[4].Width = 90;
            e.Layout.Bands[0].Columns[5].Width = 90;
            e.Layout.Bands[0].Columns[6].Width = 90;

            e.Layout.Bands[0].Columns[7].Hidden = true;

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[4].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[6].CellStyle.BorderDetails.WidthRight = borderWidth;

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[5].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[7].CellStyle.BorderDetails.WidthLeft = borderWidth;
        }

        private int borderWidth = 3;

        #endregion
    }
}
