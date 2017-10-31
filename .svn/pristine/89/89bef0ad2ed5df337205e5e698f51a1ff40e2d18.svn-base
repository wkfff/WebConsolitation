using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.iPadBricks.iPadBricks;

namespace Krista.FM.Server.Dashboards.iPhone.IT_0002_0001.reports.iPhone.IT_0002_0001
{
    public partial class IT_0002_0001_AssesmentGrid : UserControl
    {
        public string Text
        {
            get { return IPadElementHeader1.Text; }
            set { IPadElementHeader1.Text = value; }
        }

        public string Width
        {
            get
            {
                return IPadElementHeader1.Width;
            }
            set
            {
                IPadElementHeader1.Width = value;
            }
        }

        private string queryName = String.Empty;

        public string QueryName
        {
            get
            {
                return queryName;
            }
            set
            {
                queryName = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dtIncomes = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "\"", dtIncomes);

            for (int i = 0; i < dtIncomes.Rows.Count; i++ )
            {
                //dtIncomes.Rows[i][0] = String.Format("<a href='http://avolkov/test/reports/iphone/default.aspx?reportid=IT_0002_0005_IT={1}&width=690&height=530&fitByHorizontal=true' style='color: #c0c0c0'>{0}</a>",  dtIncomes.Rows[i][0], i + 4);
            }

            if (dtIncomes.Rows[0][3] != DBNull.Value)
            {
                dtIncomes.Rows[0][3] =
                    String.Format(
                        "<table style='border: 0px none black'><tr><td style='border: 0px none black'><img src='../../../images/StarYellow.png'><td style='border: 0px none black; width: 45px; text-align:left' align='left'>{0}</td><td style='border: 0px none black'>{1:N0}</td></tr></table>",
                        dtIncomes.Rows[0][3], dtIncomes.Rows[0][4]);
            }

            if (dtIncomes.Rows[0][5] != DBNull.Value)
            {
                dtIncomes.Rows[0][5] =
                    String.Format(
                        "<table style='border: 0px none black'><tr><td style='border: 0px none black'><img src='../../../images/StarGray.png'></td><td style='border: 0px none black; width: 45px; text-align:left' align='left'>{0}</td><td style='border: 0px none black'>{1:N0}</td></tr></table>",
                        dtIncomes.Rows[0][5], dtIncomes.Rows[0][6]);
            }



            if (dtIncomes.Rows[1][3] != DBNull.Value)
            {
                dtIncomes.Rows[1][3] =
                    String.Format(
                        "<table style='border: 0px none black'><tr><td style='border: 0px none black'><img src='../../../images/StarYellow.png'></td><td style='border: 0px none black; width: 45px; text-align:left; text-align:left' align='left'>{0}</td><td style='border: 0px none black'>{1:N0}</td></tr></table>",
                        dtIncomes.Rows[1][3], dtIncomes.Rows[1][4]);
            }
            if (dtIncomes.Rows[1][5] != DBNull.Value)
            {
                dtIncomes.Rows[1][5] =
                    String.Format(
                        "<table style='border: 0px none black'><tr><td style='border: 0px none black'><img src='../../../images/StarGray.png'></td><td style='border: 0px none black; width: 45px; text-align:left' align='left'>{0}</td><td style='border: 0px none black'>{1:N0}</td></tr></table>",
                        dtIncomes.Rows[1][5], dtIncomes.Rows[1][6]);
            }
            if (dtIncomes.Rows[2][3] != DBNull.Value)
            {
                dtIncomes.Rows[2][3] =
                    String.Format(
                        "<table style='border: 0px none black'><tr><td style='border: 0px none black'><img src='../../../images/StarYellow.png'></td><td style='border: 0px none black; width: 45px; text-align:left' align='left'>{0}</td><td style='border: 0px none black'>{1:N2}</td></tr></table>",
                        dtIncomes.Rows[2][3], dtIncomes.Rows[2][4]);
            }
            if (dtIncomes.Rows[2][5] != DBNull.Value)
            {
                dtIncomes.Rows[2][5] =
                    String.Format(
                        "<table style='border: 0px none black'><tr><td style='border: 0px none black'><img src='../../../images/StarGray.png'></td><td style='border: 0px none black; width: 45px; text-align:left' align='left'>{0}</td><td style='border: 0px none black'>{1:N2}</td></tr></table>",
                         dtIncomes.Rows[2][5], dtIncomes.Rows[2][6]);
            }

            string worse = String.Format(
                        "<table style='border: 0px none black'><tr><td style='border: 0px none black'><img src='../../../images/StarGray.png'></td><td style='border: 0px none black; width: 45px; text-align:left' align='left'>{0}</td><td style='border: 0px none black'>{1:N2}</td></tr></table>",
                        dtIncomes.Rows[3][3], dtIncomes.Rows[3][4]);

            if (dtIncomes.Rows[3][5] != DBNull.Value)
            {
                dtIncomes.Rows[3][3] =
                    String.Format(
                        "<table style='border: 0px none black'><tr><td style='border: 0px none black'><img src='../../../images/StarYellow.png'></td><td style='border: 0px none black; width: 45px; text-align:left' align='left'>{0}</td><td style='border: 0px none black'>{1:N2}</td></tr></table>",
                        dtIncomes.Rows[3][5], dtIncomes.Rows[3][6]);
            }
            if (dtIncomes.Rows[3][3] != DBNull.Value)
            {
                dtIncomes.Rows[3][5] = worse;
            }

            worse =
                    String.Format(
                        "<table style='border: 0px none black'><tr><td style='border: 0px none black'><img src='../../../images/StarGray.png'></td><td style='border: 0px none black; width: 45px; text-align:left' align='left'>{0}</td><td style='border: 0px none black'>{1:P2}</td></tr></table>",
                        dtIncomes.Rows[4][3], dtIncomes.Rows[4][4]);

            if (dtIncomes.Rows[4][5] != DBNull.Value)
            {
                dtIncomes.Rows[4][3] =
                    String.Format(
                        "<table style='border: 0px none black'><tr><td style='border: 0px none black'><img src='../../../images/StarYellow.png'></td><td style='border: 0px none black; width: 45px; text-align:left' align='left'>{0}</td><td style='border: 0px none black'>{1:P2}</td></tr></table>",
                        dtIncomes.Rows[4][5], dtIncomes.Rows[4][6]);
            }
            
            if (dtIncomes.Rows[4][3] != DBNull.Value)
            {
                dtIncomes.Rows[4][5] = worse;
            }

            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.Width = Unit.Empty;

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid1.DataSource = dtIncomes;
            UltraWebGrid1.DataBind();
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            bool direct = e.Row.Index < 3;
            iPadBricksHelper.SetConditionBall(e, 2, direct);
            string cellFormat = e.Row.Index == 4 ? "P2" : (e.Row.Index == 0 ||  e.Row.Index == 1 ? "N0" : "N2");
            for (int i = 1; i < 3; i++)
            {
                if (e.Row.Cells[i].Value != null)
                {
                    e.Row.Cells[i].Value = (Convert.ToDouble(e.Row.Cells[i].Value)).ToString(cellFormat);
                }
            }
            e.Row.Cells[0].Value = String.Format("<a style='color: white' href='webcommand?showPopoverReport=it_0002_0005_IT={1}&width=680&height=730&fitByHorizontal=true'>{0}</a>", e.Row.Cells[0].Value, e.Row.Index + 4);
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            e.Layout.Bands[0].Columns[0].Width = 230;
            e.Layout.Bands[0].Columns[1].Width = 92;
            e.Layout.Bands[0].Columns[2].Width = 118;
            e.Layout.Bands[0].Columns[3].Width = 159;
            e.Layout.Bands[0].Columns[5].Width = 159;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].Header.Style.ForeColor = Color.FromArgb(50, 50, 50);
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            
            e.Layout.Bands[0].Columns[4].Hidden = true;
            e.Layout.Bands[0].Columns[6].Hidden = true;

            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[5].CellStyle.HorizontalAlign = HorizontalAlign.Left;
        }
    }
}