using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.iPadBricks.iPadBricks;

namespace Krista.FM.Server.Dashboards.iPhone.IT_0002_0003.reports.iPhone.IT_0002_0003
{
    public partial class IT_0002_0003_DetailGrid : UserControl
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
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtIncomes);
            dtIncomes.Rows[0][6] = DBNull.Value;

            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.Width = Unit.Parse(Width);

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid1.DataSource = dtIncomes;
            UltraWebGrid1.DataBind();
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            iPadBricksHelper.SetConditionArrow(e, 5);
            if (e.Row.Index > 0)
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
          //  HideRow(e);
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            e.Layout.Bands[0].Columns[0].Width = 183;
            e.Layout.Bands[0].Columns[1].Width = 99;
            e.Layout.Bands[0].Columns[2].Width = 99;
            e.Layout.Bands[0].Columns[3].Width = 99;
            e.Layout.Bands[0].Columns[4].Width = 99;
            e.Layout.Bands[0].Columns[5].Width = 98;
            e.Layout.Bands[0].Columns[6].Width = 80;
           
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");
        }
                
        private static void HideRow(RowEventArgs e)
        {
            e.Row.Hidden = true;

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i] != null && e.Row.Cells[i].Value != null &&
                    Convert.ToDouble(e.Row.Cells[i].Value.ToString()) != 0)
                {
                    e.Row.Hidden = false;
                }
            }
        }
    }
}