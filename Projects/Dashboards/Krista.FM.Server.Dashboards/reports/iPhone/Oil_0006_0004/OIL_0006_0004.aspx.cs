using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Drawing;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class OIL_0006_0004 : CustomReportPage
    {
        private DateTime currentDate;
        private DateTime lastDate;

        protected override void Page_Load(object sender, EventArgs e)
        {
            #region Настройка грида

            GridBrick.BrowserSizeAdapting = false;
            GridBrick.Height = Unit.Empty;
            GridBrick.Width = Unit.Empty;
            GridBrick.RedNegativeColoring = false;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            currentDate = CubeInfo.GetLastDate(DataProvidersFactory.SecondaryMASDataProvider, "Oil_0006_0004_lastDate");
            lastDate = new DateTime(currentDate.Year - 1, 12, 30);

            UserParams.PeriodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[День]", currentDate, 5);
            UserParams.PeriodLastDate.Value = CRHelper.PeriodMemberUName("[Период].[День]", lastDate, 5);

            IPadElementHeader1.Text = "Бензин АИ-92";
            InitializeTable1();
        }

        #region Настройка грида

        private DataTable dt;

        private void InitializeTable1()
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("OIL_0006_0004_Grid");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dt);

            if (dt.Columns.Count > 0)
            {
                dt.Columns.RemoveAt(0);
            }

            PaddingRule rule = new PaddingRule(0, "Уровень", 20);
            GridBrick.AddIndicatorRule(rule);

            GrowRateRule growRateRule = new GrowRateRule("Темп прироста, %");
            growRateRule.IncreaseImg = "~/images/ArrowRedUpIPad.png";
            growRateRule.DecreaseImg = "~/images/ArrowGreenDownIPad.png";
            growRateRule.Limit = 0;
            GridBrick.AddIndicatorRule(growRateRule);

            GridBrick.DataTable = dt;
        }

        private void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            double limit = GetRowValue(e.Row, 10);
            double value = GetRowValue(e.Row, 1);

            setStar(e.Row, 1, 6, 7);
            setStar(e.Row, 3, 9, 8);
            
            e.Row.Cells[1].Style.Font.Bold = true;
            
            if (e.Row.Cells[5].Value.ToString() == "0" && !e.Row.Cells[0].ToString().Contains("Среднее"))
            {
                e.Row.Cells[1].ColSpan = 4;
                e.Row.Style.BorderDetails.WidthTop = 3;
                
                e.Row.Cells[0].Style.ForeColor = Color.White;
                if (limit != Double.MinValue && limit > 0)
                {
                    e.Row.Cells[1].Value = String.Format("порог Минэнерго России 31.12.2011:&nbsp;<b><span style='color:white;'>{0:N2}</span></b>&nbsp;руб.", limit);
                    e.Row.Cells[1].Style.ForeColor = Color.Gray;
                    e.Row.Cells[1].Style.Font.Bold = false;
                }
                else
                {
                    e.Row.Cells[1].Value = String.Empty;
                }
            }
            else if (value != Double.MinValue && limit != Double.MinValue && value > limit && limit != 0)
            {
//                e.Row.Cells[1].Style.BackgroundImage = "~/images/ballRedBB.png";
//                e.Row.Cells[1].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                e.Row.Cells[1].Value = String.Format("<img src='../../../images/ballRedBB.png' style='margin-bottom:-4px'>&nbsp;{0:N2}", value);
            }
        }

        private static void setStar(UltraGridRow row, int valueIndex, int bestIndex, int worseIndex)
        {
            double value = GetRowValue(row, valueIndex);
            double bestValue = GetRowValue(row, bestIndex);
            double worseValue = GetRowValue(row, worseIndex);

            if (value != Double.MinValue && value == bestValue)
            {
                row.Cells[valueIndex].Style.BackgroundImage = "~/images/starGrayBB.png";
                row.Cells[valueIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: 10% center; padding-left: 2px";
            }

            if (value != Double.MinValue && value == worseValue)
            {
                row.Cells[valueIndex].Style.BackgroundImage = "~/images/starYellowBB.png";
                row.Cells[valueIndex].Style.CustomRules = "background-repeat: no-repeat; background-position: 10% center; padding-left: 2px";
            }
        }

        private static double GetRowValue(UltraGridRow e, int index)
        {
            if (e.Cells.Count > index)
            {
                if (e.Cells[index].Value != null && e.Cells[index].Value.ToString() != String.Empty)
                {
                    return Convert.ToDouble(e.Cells[index].Value);
                }
            }

            return Double.MinValue;
        }

        void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");

            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            e.Layout.Bands[0].Columns[5].Hidden = true;
            e.Layout.Bands[0].Columns[6].Hidden = true;
            e.Layout.Bands[0].Columns[7].Hidden = true;
            e.Layout.Bands[0].Columns[8].Hidden = true;
            e.Layout.Bands[0].Columns[9].Hidden = true;
            e.Layout.Bands[0].Columns[10].Hidden = true;

            GridHeaderLayout headerLayout1 = GridBrick.GridHeaderLayout;
            headerLayout1.AddCell("");
            headerLayout1.AddCell(string.Format("Розничная цена на {0:dd.MM.yyyy}", currentDate));
            headerLayout1.AddCell(string.Format("Розничная цена на {0:dd.MM.yyyy} ", lastDate));
            headerLayout1.AddCell("Абс.откл., руб.");
            headerLayout1.AddCell("Темп прироста, %");

            headerLayout1.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[0].Width = 285;
            e.Layout.Bands[0].Columns[1].Width = 120;
            e.Layout.Bands[0].Columns[2].Width = 120;
            e.Layout.Bands[0].Columns[3].Width = 120;
            e.Layout.Bands[0].Columns[4].Width = 112;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[1].CellStyle.Font.Size = 12;
            //e.Layout.Bands[0].Columns[1].CellStyle.Font.Bold = true;
            e.Layout.Bands[0].Columns[2].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[3].CellStyle.Font.Size = 12;
            e.Layout.Bands[0].Columns[4].CellStyle.Font.Size = 12;
        }

        #endregion
    }
}
