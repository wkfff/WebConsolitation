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
    public partial class IT_0002_0001_RestsGrid : UserControl
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
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.Width = Unit.Parse(Width);

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid1.DataSource = dtIncomes;
            UltraWebGrid1.DataBind();
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            iPadBricksHelper.SetConditionCorner(e, 1);
            iPadBricksHelper.SetConditionCorner(e, 2);
            string cellFormat = "N2";
            for (int i = 1; i < 2; i++)
            {
                if (e.Row.Cells[i].Value != null)
                {
                    e.Row.Cells[i].Value = (Convert.ToDouble(e.Row.Cells[i].Value) / 1000).ToString(cellFormat);
                }
            }
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            e.Layout.Bands[0].Columns[0].Width = 171;
            e.Layout.Bands[0].Columns[1].Width = 181;
            e.Layout.Bands[0].Columns[2].Width = 181;
           
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].Header.Style.ForeColor = Color.FromArgb(50, 50, 50);
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
        }
    }
}