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
    public partial class IT_0003_0001_AssesmentGrid : UserControl
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
            bool direct = e.Row.Index < 3;
            if (e.Row.Index < 5)
            {
                SetConditionBall(e, 2, direct);
                SetRankImage(e, 3, 4, direct);
            }
            else
            {
                e.Row.Cells[3].Value = String.Empty;
            }
            e.Row.Cells[3].Style.CustomRules = "background-repeat: no-repeat; background-position: 40px center; padding-left: 2px; padding-right: 32px";
            string cellFormat = e.Row.Index > 3 ? "P2" : (e.Row.Index == 0 ||  e.Row.Index == 1 ? "N0" : "N2");
            for (int i = 1; i < 3; i++)
            {
                if (e.Row.Cells[i].Value != null)
                {
                    e.Row.Cells[i].Value = (Convert.ToDouble(e.Row.Cells[i].Value)).ToString(cellFormat);
                }
            }
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            e.Layout.Bands[0].Columns[0].Width = 320;
            e.Layout.Bands[0].Columns[1].Width = 164;
            e.Layout.Bands[0].Columns[2].Width = 162;
            e.Layout.Bands[0].Columns[3].Width = 113;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);
            
            e.Layout.Bands[0].Columns[4].Hidden = true;

            e.Layout.Bands[0].Columns[3].CellStyle.HorizontalAlign = HorizontalAlign.Right;
        }
        
        private static void SetConditionBall(RowEventArgs e, int index, bool directAssesment)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null &&
                e.Row.Cells[index - 1] != null &&
                e.Row.Cells[index - 1].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                double compareValue = Convert.ToDouble(e.Row.Cells[index - 1].Value.ToString());
                string positiveImg = directAssesment ? "~/images/ballGreenBB.png" : "~/images/ballRedBB.png";
                string negativeImg = directAssesment ? "~/images/ballRedBB.png" : "~/images/ballGreenBB.png";
                string img;
                //string title;
                if (value < compareValue)
                {
                    img = negativeImg;

                }
                else
                {
                    img = positiveImg;

                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules = "background-repeat: no-repeat; background-position: 20px center; padding-left: 2px";
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
                e.Row.Cells[rankCellIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: 40px center; padding-left: 2px; padding-right: 32px";
            }
        }
    }
}