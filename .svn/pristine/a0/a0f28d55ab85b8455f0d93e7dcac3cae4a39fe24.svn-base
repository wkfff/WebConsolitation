using System;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Common;
using System.Data;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebGrid;
using System.Web.UI.WebControls;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FST_0001_0011 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();

        private int currentYear = 2011;
        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {
            UltraWebGrid.Width = Unit.Empty;
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.DataBind();
        }


        #region Обработчики гридаaa

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FST_0001_0011_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            int notZeroCountRF = 0;
            double foAvgRF = 0;

            int notZeroGrownCountRF = 0;
            double foGrownAvgRF = 0;

            int notZeroDemosCountRF = 0;
            double foDemosAvgRF = 0;

            int notZeroGrownDemosCountRF = 0;
            double foGrownDemosAvgRF = 0;

            int foRowIndex = 0;

            int notZeroCount = 0;
            double foAvg = 0;

            int notZeroGrownCount = 0;
            double foGrownAvg = 0;

            int notZeroDemosCount = 0;
            double foDemosAvg = 0;

            int notZeroGrownDemosCount = 0;
            double foGrownDemosAvg = 0;

            for (int i = 1; i < dtGrid.Rows.Count; i++)
            {
                if (dtGrid.Rows[i]["Уровень"].ToString() == "Федеральный округ")
                {
                    if (foRowIndex == 0)
                    {
                        foRowIndex = i;
                    }
                    else
                    {
                        dtGrid.Rows[foRowIndex]["Тариф на электрическую энергию для населения "] = notZeroDemosCount == 0 ? 0 : foDemosAvg / notZeroDemosCount;
                        dtGrid.Rows[foRowIndex]["Рост тарифа на электрическую энергию для населения "] = notZeroGrownDemosCount == 0 ? 0 : foGrownDemosAvg / notZeroGrownDemosCount;
                        
                        foRowIndex = i;

                        notZeroCount = 0;
                        foAvg = 0;

                        notZeroGrownCount = 0;
                        foGrownAvg = 0;

                        notZeroDemosCount = 0;
                        foDemosAvg = 0;

                        notZeroGrownDemosCount = 0;
                        foGrownDemosAvg = 0;
                    }
                }
                else if (dtGrid.Rows[i]["Уровень"].ToString() == "Субъект РФ")
                {
                    double taxValue;
                    
                    double taxDemosValue;
                    if (dtGrid.Rows[i]["Тариф на электрическую энергию для населения "] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["Тариф на электрическую энергию для населения "].ToString(), out taxDemosValue) &&
                        taxDemosValue != 0)
                    {
                        notZeroDemosCount++;
                        foDemosAvg += taxDemosValue;
                        notZeroDemosCountRF++;
                        foDemosAvgRF += taxDemosValue;
                    }
                    double grownValue;
                   
                    double grownDemosValue;
                    if (dtGrid.Rows[i]["Рост тарифа на электрическую энергию для населения "] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["Рост тарифа на электрическую энергию для населения "].ToString(), out grownDemosValue) &&
                        grownDemosValue != -1)
                    {
                        notZeroGrownDemosCount++;
                        foGrownDemosAvg += grownDemosValue;
                        notZeroGrownDemosCountRF++;
                        foGrownDemosAvgRF += grownDemosValue;
                    }
                }
            }

           
            dtGrid.Rows[foRowIndex]["Тариф на электрическую энергию для населения "] = notZeroDemosCount == 0 ? 0 : foDemosAvg / notZeroDemosCount;
            dtGrid.Rows[foRowIndex]["Рост тарифа на электрическую энергию для населения "] = notZeroGrownDemosCount == 0 ? 0 : foGrownDemosAvg / notZeroGrownDemosCount;
                       
            
            dtGrid.Rows[0]["Тариф на электрическую энергию для населения "] = notZeroDemosCountRF == 0 ? 0 : foDemosAvgRF / notZeroDemosCountRF;

            dtGrid.Rows[0]["Рост тарифа на электрическую энергию для населения "] = notZeroGrownDemosCountRF == 0 ? 0 : foGrownDemosAvgRF / notZeroGrownDemosCountRF;

            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            e.Layout.Bands[0].Columns[0].Width = 395;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 165;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");

            e.Layout.Bands[0].Columns[2].Width = 195;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "P2");


            e.Layout.Bands[0].Columns[5].Hidden = true;
            e.Layout.Bands[0].Columns[6].Hidden = true;
            e.Layout.Bands[0].Columns[7].Hidden = true;
            e.Layout.Bands[0].Columns[8].Hidden = true;
            e.Layout.Bands[0].Columns[9].Hidden = true;
            e.Layout.Bands[0].Columns[10].Hidden = true;
            e.Layout.Bands[0].Columns[11].Hidden = true;
            e.Layout.Bands[0].Columns[3].Hidden = true;
            e.Layout.Bands[0].Columns[4].Hidden = true;


            headerLayout.AddCell(" ");

            GridHeaderCell cell = headerLayout.AddCell("Тариф для населения");

            cell.AddCell(String.Format("{0} год,<br/>руб./кВт.ч.", currentYear));
            cell.AddCell(String.Format("Прирост тарифа {0}/{1}", currentYear, currentYear - 1));

            headerLayout.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[4].CellStyle.BorderDetails.WidthRight = borderWidth;

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[3].CellStyle.BorderDetails.WidthLeft = borderWidth;
        }

        private int borderWidth = 3;

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[1].Value == null &&
                e.Row.Cells[3].Value == null)
            {
                e.Row.Hidden = true;
                return;
            }

            if (e.Row.Index != 0)
            {
                SetConditionCorner(e, 2, Convert.ToDouble(e.Row.Band.Grid.Rows[0].Cells[2].Value));
            }

            if (e.Row.Cells[11].Value.ToString() == "Субъект РФ")
            {
                if (e.Row.Cells[1].Value != null)
                {
                    e.Row.Cells[1].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 40px; border: 1px solid black'>{0}</td><td style='width: 185px; border: 1px solid black'>{1:N2}</td></tr></table>", GetRankImage(e, 3, 6), e.Row.Cells[1].Value);
                }
                if (e.Row.Cells[2].Value != null)
                {
                    e.Row.Cells[2].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 40px; border: 1px solid black'>{0}</td><td style='width: 185px; border: 1px solid black'>{1:P2}</td></tr></table>", GetRankImage(e, 5, 8), e.Row.Cells[2].Value);
                }
            }
            if (e.Row.Cells[11].Value != null &&
                (e.Row.Cells[11].Value.ToString() != "Субъект РФ"))
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
            else
            {
                e.Row.Cells[0].Value = String.Format("<a style='color: white' href='webcommand?showReport=fst_0001_0003_{1}'>{0}</a>", e.Row.Cells[0].Value, CustomParams.GetSubjectIdByName(e.Row.Cells[0].Value.ToString()));
            }
            if (e.Row.Cells[0].Value.ToString().ToLower().Contains("федер"))
            {
                e.Row.Style.BorderDetails.WidthTop = 3;
            }
            if (e.Row.Cells[11].Value != null &&
                (e.Row.Cells[11].Value.ToString() == "РФ"))
            {
                e.Row.Cells[0].Value = String.Format("<a style='color: white' href='webcommand?showReport=fst_0001_0002'>{0}</a>", e.Row.Cells[0].Value);
            }
        }
        #endregion

        public static void SetConditionCorner(RowEventArgs e, int index, double borderValue)
        {

            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value = Convert.ToDouble(e.Row.Cells[index].Value.ToString());
                string img;
                if (value > borderValue)
                {
                    img = "~/images/cornerRed.gif";

                }
                else if (value < borderValue)
                {
                    img = "~/images/cornerGreen.gif";

                }
                else
                {
                    img = "~/images/CornerGray.gif";

                }
                e.Row.Cells[index].Style.BackgroundImage = img;
                e.Row.Cells[index].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: right top; padding-left: 2px";
            }
        }

        public static string GetRankImage(RowEventArgs e, int rankCellIndex, int worseRankCelIndex)
        {
            string img = String.Empty;
            if (e.Row.Cells[rankCellIndex] != null &&
                e.Row.Cells[rankCellIndex].Value != null &&
                e.Row.Cells[worseRankCelIndex] != null &&
                e.Row.Cells[worseRankCelIndex].Value != null)
            {
                int value = Convert.ToInt32(e.Row.Cells[rankCellIndex].Value.ToString());
                int worseRankValue = Convert.ToInt32(e.Row.Cells[worseRankCelIndex].Value.ToString());

                if (value == 1)
                {
                    img = "<img src='../../../images/max.png'>";
                }
                else if (value == worseRankValue)
                {
                    img = "<img src='../../../images/min.png'>";
                }
            }
            return img;
        }
    }
}