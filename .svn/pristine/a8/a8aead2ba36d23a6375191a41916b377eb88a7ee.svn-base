using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.iPhone.IT_0002_0001.reports.iPhone.IT_0002_0001
{
    public partial class IT_0004_0001_AdminOutcomesGrid : UserControl
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

            

            UltraWebGrid1.Height = Unit.Empty;
            UltraWebGrid1.Width = Unit.Empty;

            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid1_InitializeRow);
            UltraWebGrid1.DataSource = dtIncomes;
            UltraWebGrid1.DataBind();
        }

       

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            SetConditionArrow(e, 3);
            if (e.Row.Index > 1)
            {
                
                SetRankImage(e, 6, 7, true);
            }
            else
            {
                e.Row.Cells[6].Value = String.Empty;
            }
            //e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: 40px center; padding-left: 2px; padding-right: 32px";
           // string cellFormat = e.Row.Index > 3 ? "P2" : (e.Row.Index == 0 ||  e.Row.Index == 1 ? "N0" : "N2");
            //for (int i = 1; i < 3; i++)
            //{
            //    if (e.Row.Cells[i].Value != null)
            //    {
            //        e.Row.Cells[i].Value = (Convert.ToDouble(e.Row.Cells[i].Value)).ToString(cellFormat);
            //    }
            //}
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            e.Layout.Bands[0].Columns[0].Width = 170;
            e.Layout.Bands[0].Columns[1].Width = 124;
            e.Layout.Bands[0].Columns[2].Width = 112;
            e.Layout.Bands[0].Columns[3].Width = 103;
            e.Layout.Bands[0].Columns[4].Width = 83;
            e.Layout.Bands[0].Columns[5].Width = 83;
            e.Layout.Bands[0].Columns[6].Width = 83;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[4].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            
            e.Layout.Bands[0].Columns[7].Hidden = true;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "P2");

           // e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
        }

        private static void SetConditionArrow(RowEventArgs e, int index)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img;
                //string title;
                if (value > 1)
                {
                    img = "~/images/arrowGreenUpBB.png";

                }
                else
                {
                    img = "~/images/arrowRedDownBB.png";

                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 10px center; padding-left: 2px";
                //   e.Row.Cells[3].Title = title;
            }
            if (e.Row.Index > 0)
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
        }

        private static void SetRankImage(RowEventArgs e, int rankCellIndex, int worseRankCelIndex, bool direct)
        {
            if (e.Row.Cells[rankCellIndex] != null &&
                e.Row.Cells[rankCellIndex].Value != null &&
                e.Row.Cells[worseRankCelIndex] != null &&
                e.Row.Cells[worseRankCelIndex].Value != null)
            {
                int value = Convert.ToInt32(e.Row.Cells[rankCellIndex].Value.ToString());
                int worseRankValue = Convert.ToInt32(e.Row.Cells[worseRankCelIndex].Value.ToString());
                string img = String.Empty;
                //string title;
                if (direct)
                {
                    if (value == 1)
                    {
                        img = "~/images/StarYellow.png";
                    }
                    else if (value == worseRankValue)
                    {
                        img = "~/images/StarGray.png";
                    }
                }
                else
                {
                    if (value == 1)
                    {
                        img = "~/images/StarGray.png";
                    }
                    else if (value == worseRankValue)
                    {
                        img = "~/images/StarYellow.png";
                    }
                    e.Row.Cells[rankCellIndex].Value = worseRankValue - value + 1;
                }
                e.Row.Cells[rankCellIndex].Style.BackgroundImage = img;
                e.Row.Cells[rankCellIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: 20px center; padding-left: 2px; padding-right: 15px";
            }
        }
    }
}