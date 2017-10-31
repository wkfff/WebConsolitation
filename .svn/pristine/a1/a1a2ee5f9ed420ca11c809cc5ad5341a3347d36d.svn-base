using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.iPhone.IT_0002_0005.reports.iPhone.IT_0002_0005
{
    public partial class IT_0002_0005_DetailGrid : UserControl
    {
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
            dtIncomes.Rows[0][3] = DBNull.Value;
            UltraWebGrid1.DataSource = dtIncomes;
            UltraWebGrid1.DataBind();
        }

        void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
            bool direct = CustomParam.CustomParamFactory("direct_assess").Value == "BDESC" ? true : false;
            SetConditionBall(e, 2, direct);
            SetRankImage(e, 3, 4);
            if (e.Row.Index > 0)
            {
                e.Row.Cells[0].Style.Padding.Left = 20;
            }
        }

        void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.TableLayout = TableLayout.Fixed;

            e.Layout.Bands[0].Columns[0].Width = 200;
            e.Layout.Bands[0].Columns[1].Width = 150;
            e.Layout.Bands[0].Columns[2].Width = 150; 
            e.Layout.Bands[0].Columns[3].Width = 140;
            e.Layout.Bands[0].Columns[4].Hidden = true;
           
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].Header.Style.Font.Size = FontUnit.Parse("16px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.Black;
            e.Layout.Bands[0].Columns[1].CellStyle.ForeColor = Color.Black;
            e.Layout.Bands[0].Columns[2].CellStyle.ForeColor = Color.Black;
            e.Layout.Bands[0].Columns[3].CellStyle.ForeColor = Color.Black;

            e.Layout.Bands[0].Columns[0].CellStyle.BackColor = Color.FromArgb(168, 168, 168);
            e.Layout.Bands[0].Columns[1].CellStyle.BackColor = Color.FromArgb(168, 168, 168);
            e.Layout.Bands[0].Columns[2].CellStyle.BackColor = Color.FromArgb(168, 168, 168);
            e.Layout.Bands[0].Columns[3].CellStyle.BackColor = Color.FromArgb(168, 168, 168);

            e.Layout.Bands[0].Columns[0].CellStyle.BorderColor = Color.FromArgb(0x6b6b6b);
            e.Layout.Bands[0].Columns[1].CellStyle.BorderColor = Color.FromArgb(0x6b6b6b);
            e.Layout.Bands[0].Columns[2].CellStyle.BorderColor = Color.FromArgb(0x6b6b6b);
            e.Layout.Bands[0].Columns[3].CellStyle.BorderColor = Color.FromArgb(0x6b6b6b);

            e.Layout.Bands[0].HeaderStyle.BackColor = Color.FromArgb(0x6b6b6b);
            e.Layout.Bands[0].HeaderStyle.BorderColor = Color.FromArgb(0x6b6b6b);
            e.Layout.Bands[0].HeaderStyle.ForeColor = Color.Black;
            
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
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

        private static void SetRankImage(RowEventArgs e, int rankCellIndex, int worseRankCelIndex)
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
                if (value == 1)
                {
                    img = "~/images/StarYellow.png";

                }
                else if (value == worseRankValue)
                {
                    img = "~/images/StarGray.png";

                }
                e.Row.Cells[rankCellIndex].Style.BackgroundImage = img;
                e.Row.Cells[rankCellIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: 20px center; padding-left: 2px";
            }
        }
    }
}