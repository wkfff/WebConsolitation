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
    public partial class IT_0002_0001_IncomesGrid : UserControl
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

        public string ImageUrl
        {
            get
            {
                return Image1.ImageUrl;
            }
            set
            {
                Image1.ImageUrl = value;
            }
        }

        public string DescriptionText
        {
            get
            {
                return Label1.Text;
            }
            set
            {
                Label1.Text = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dtIncomes = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "\"", dtIncomes);
            dtIncomes.Rows[3][3] = DBNull.Value;
            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.Width = Unit.Empty;
            Label1.Text = String.Format(Label1.Text, Convert.ToDouble(dtIncomes.Rows[2][2]) / 1000);
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid1.DataSource = dtIncomes;
            UltraWebGrid1.DataBind();
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            iPadBricksHelper.SetConditionArrow(e, 3);
            string cellFormat = e.Row.Index == 3 ? "P2" : "N0";
            double divisor = e.Row.Index == 3 ? 1 : 1000;
            for (int i = 1; i < 3; i++)
            {
                if (e.Row.Cells[i].Value != null)
                {
                    e.Row.Cells[i].Value = (Convert.ToDouble(e.Row.Cells[i].Value) / divisor).ToString(cellFormat);
                }
            }
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            e.Layout.Bands[0].Columns[0].Width = 109;
            e.Layout.Bands[0].Columns[1].Width = 83;
            e.Layout.Bands[0].Columns[2].Width = 84;
            e.Layout.Bands[0].Columns[3].Width = 100;
           
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.ForeColor = Color.FromArgb(50, 50, 50);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
           
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
        }

        
    }
}