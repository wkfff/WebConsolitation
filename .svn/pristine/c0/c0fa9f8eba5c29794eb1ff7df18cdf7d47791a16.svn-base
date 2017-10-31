using System;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Common;
using System.Data;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebGrid;
using System.Web.UI.WebControls;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FST_0001_0031_Horizontal : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();

        // выбранный период
        private CustomParam selectedPeriod;

        private int currentYear = 2011;
        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {
            UltraWebGrid.Width = Unit.Empty;
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FST_0001_0031_Horizontal_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            int notZeroCountRF = 0;
            double foAvgRF = 0;
            int notZeroGrownCountRF = 0;
            double foGrownAvgRF = 0;

            int notZeroDemosCountRF = 0;
            double foDemosAvgRF = 0;
            int notZeroGrownDemosCountRF = 0;
            double foGrownDemosAvgRF = 0;

            int notZeroBujetCountRF = 0;
            double foBujetAvgRF = 0;
            int notZeroGrownBujetCountRF = 0;
            double foGrownBujetAvgRF = 0;

            int notZeroOthersCountRF = 0;
            double foOthersAvgRF = 0;
            int notZeroGrownOthersCountRF = 0;
            double foGrownOthersAvgRF = 0;

            int foRowIndex = 0;

            int notZeroCount = 0;
            double foAvg = 0;
            int notZeroGrownCount = 0;
            double foGrownAvg = 0;

            int notZeroDemosCount = 0;
            double foDemosAvg = 0;
            int notZeroGrownDemosCount = 0;
            double foGrownDemosAvg = 0;

            int notZeroBujetCount = 0;
            double foBujetAvg = 0;
            int notZeroGrownBujetCount = 0;
            double foGrownBujetAvg = 0;

            int notZeroOthersCount = 0;
            double foOthersAvg = 0;
            int notZeroGrownOthersCount = 0;
            double foGrownOthersAvg = 0;

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
                        dtGrid.Rows[foRowIndex]["Среднеотпускной тариф на водоснабжение"] = notZeroCount == 0 ? 0 : foAvg / notZeroCount;
                        dtGrid.Rows[foRowIndex]["Рост тарифа на водоснабжение"] = notZeroGrownCount == 0 ? 0 : foGrownAvg / notZeroGrownCount;

                        dtGrid.Rows[foRowIndex]["Тариф на водоснабжение для населения"] = notZeroDemosCount == 0 ? 0 : foDemosAvg / notZeroDemosCount;
                        dtGrid.Rows[foRowIndex]["Рост тарифа на водоснабжение для населения"] = notZeroGrownDemosCount == 0 ? 0 : foGrownDemosAvg / notZeroGrownDemosCount;

                        dtGrid.Rows[foRowIndex]["Тариф на водоснабжение для бюджетных потребителей"] = notZeroBujetCount == 0 ? 0 : foBujetAvg / notZeroBujetCount;
                        dtGrid.Rows[foRowIndex]["Рост тарифа на водоснабжение для бюджетных потребителей"] = notZeroGrownBujetCount == 0 ? 0 : foGrownBujetAvg / notZeroGrownBujetCount;

                        dtGrid.Rows[foRowIndex]["Тариф на водоснабжение для прочих потребителей"] = notZeroOthersCount == 0 ? 0 : foOthersAvg / notZeroOthersCount;
                        dtGrid.Rows[foRowIndex]["Рост тарифа на водоснабжение для прочих потребителей"] = notZeroGrownOthersCount == 0 ? 0 : foGrownOthersAvg / notZeroGrownOthersCount;

                        notZeroCountRF = notZeroCount == 0 ? notZeroCountRF : notZeroCountRF + 1;
                        foAvgRF += notZeroCount == 0 ? 0 : foAvg / notZeroCount;

                        notZeroGrownCountRF = notZeroGrownCount == 0 ? notZeroGrownCountRF : notZeroGrownCountRF + 1;
                        foGrownAvgRF += notZeroGrownCount == 0 ? 0 : foGrownAvg / notZeroGrownCount;

                        notZeroDemosCountRF = notZeroDemosCount == 0 ? notZeroDemosCountRF : notZeroDemosCountRF + 1;
                        foDemosAvgRF += notZeroDemosCount == 0 ? 0 : foDemosAvg / notZeroDemosCount;

                        notZeroGrownDemosCountRF = notZeroGrownDemosCount == 0 ? notZeroGrownDemosCountRF : notZeroGrownDemosCountRF + 1;
                        foGrownDemosAvgRF += notZeroGrownDemosCount == 0 ? 0 : foGrownDemosAvg / notZeroGrownDemosCount;

                        notZeroBujetCountRF = notZeroBujetCount == 0 ? notZeroBujetCountRF : notZeroBujetCountRF + 1;
                        foBujetAvgRF += notZeroBujetCount == 0 ? 0 : foBujetAvg / notZeroBujetCount;

                        notZeroGrownBujetCountRF = notZeroGrownBujetCount == 0 ? notZeroGrownBujetCountRF : notZeroGrownBujetCountRF + 1;
                        foGrownBujetAvgRF += notZeroGrownBujetCount == 0 ? 0 : foGrownBujetAvg / notZeroGrownBujetCount;

                        notZeroOthersCountRF = notZeroOthersCount == 0 ? notZeroOthersCountRF : notZeroOthersCountRF + 1;
                        foOthersAvgRF += notZeroOthersCount == 0 ? 0 : foOthersAvg / notZeroOthersCount;

                        notZeroGrownOthersCountRF = notZeroGrownOthersCount == 0 ? notZeroGrownOthersCountRF : notZeroGrownOthersCountRF + 1;
                        foGrownOthersAvgRF += notZeroGrownOthersCount == 0 ? 0 : foGrownOthersAvg / notZeroGrownOthersCount;

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
                    if (dtGrid.Rows[i]["Среднеотпускной тариф на водоснабжение"] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["Среднеотпускной тариф на водоснабжение"].ToString(), out taxValue) &&
                        taxValue != 0)
                    {
                        notZeroCount++;
                        foAvg += taxValue;
                    }
                    double taxDemosValue;
                    if (dtGrid.Rows[i]["Тариф на водоснабжение для населения"] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["Тариф на водоснабжение для населения"].ToString(), out taxDemosValue) &&
                        taxDemosValue != 0)
                    {
                        notZeroDemosCount++;
                        foDemosAvg += taxDemosValue;
                    }
                    double taxBujetValue;
                    if (dtGrid.Rows[i]["Тариф на водоснабжение для бюджетных потребителей"] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["Тариф на водоснабжение для бюджетных потребителей"].ToString(), out taxBujetValue) &&
                        taxBujetValue != 0)
                    {
                        notZeroBujetCount++;
                        foBujetAvg += taxBujetValue;
                    }
                    double taxOthersValue;
                    if (dtGrid.Rows[i]["Тариф на водоснабжение для прочих потребителей"] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["Тариф на водоснабжение для прочих потребителей"].ToString(), out taxOthersValue) &&
                        taxOthersValue != 0)
                    {
                        notZeroOthersCount++;
                        foOthersAvg += taxOthersValue;
                    }

                    double grownValue;
                    if (dtGrid.Rows[i]["Рост тарифа на водоснабжение"] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["Рост тарифа на водоснабжение"].ToString(), out grownValue) &&
                        grownValue != -0)
                    {
                        notZeroGrownCount++;
                        foGrownAvg += grownValue;
                    }
                    double grownDemosValue;
                    if (dtGrid.Rows[i]["Рост тарифа на водоснабжение для населения"] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["Рост тарифа на водоснабжение для населения"].ToString(), out grownDemosValue) &&
                        grownDemosValue != -0)
                    {
                        notZeroGrownDemosCount++;
                        foGrownDemosAvg += grownDemosValue;
                    }
                    double grownBujetValue;
                    if (dtGrid.Rows[i]["Рост тарифа на водоснабжение для бюджетных потребителей"] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["Рост тарифа на водоснабжение для бюджетных потребителей"].ToString(), out grownBujetValue) &&
                        grownBujetValue != -0)
                    {
                        notZeroGrownBujetCount++;
                        foGrownBujetAvg += grownBujetValue;
                    }
                    double grownOthersValue;
                    if (dtGrid.Rows[i]["Рост тарифа на водоснабжение для прочих потребителей"] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["Рост тарифа на водоснабжение для прочих потребителей"].ToString(), out grownOthersValue) &&
                        grownOthersValue != -0)
                    {
                        notZeroGrownOthersCount++;
                        foGrownOthersAvg += grownOthersValue;
                    }
                }
            }

            dtGrid.Rows[0]["Среднеотпускной тариф на водоснабжение"] = notZeroCountRF == 0 ? 0 : foAvgRF / notZeroCountRF;
            dtGrid.Rows[0]["Рост тарифа на водоснабжение"] = notZeroGrownCountRF == 0 ? 0 : foGrownAvgRF / notZeroGrownCountRF;

            dtGrid.Rows[0]["Рост тарифа на водоснабжение для населения"] = notZeroDemosCountRF == 0 ? 0 : foDemosAvgRF / notZeroDemosCountRF;
            dtGrid.Rows[0]["Рост тарифа на водоснабжение для населения"] = notZeroGrownDemosCountRF == 0 ? 0 : foGrownDemosAvgRF / notZeroGrownDemosCountRF;

            dtGrid.Rows[0]["Рост тарифа на водоснабжение для бюджетных потребителей"] = notZeroBujetCountRF == 0 ? 0 : foBujetAvgRF / notZeroBujetCountRF;
            dtGrid.Rows[0]["Рост тарифа на водоснабжение для бюджетных потребителей"] = notZeroGrownBujetCountRF == 0 ? 0 : foGrownBujetAvgRF / notZeroGrownBujetCountRF;

            dtGrid.Rows[0]["Рост тарифа на водоснабжение для прочих потребителей"] = notZeroOthersCountRF == 0 ? 0 : foOthersAvgRF / notZeroOthersCountRF;
            dtGrid.Rows[0]["Рост тарифа на водоснабжение для прочих потребителей"] = notZeroGrownOthersCountRF == 0 ? 0 : foGrownOthersAvgRF / notZeroGrownOthersCountRF;

            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            e.Layout.Bands[0].Columns[0].Width = 205;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 110;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");

            e.Layout.Bands[0].Columns[2].Width = 91;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "P0");

            e.Layout.Bands[0].Columns[3].Width = 110;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");

            e.Layout.Bands[0].Columns[4].Width = 91;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P0");

            e.Layout.Bands[0].Columns[5].Width = 110;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");

            e.Layout.Bands[0].Columns[6].Width = 91;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P0");

            e.Layout.Bands[0].Columns[7].Width = 110;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N2");

            e.Layout.Bands[0].Columns[8].Width = 91;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "P0");

            for (int i = 9; i < 26; i++)
            {
                e.Layout.Bands[0].Columns[i].Hidden = true;
            }

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            headerLayout.AddCell(" ");

            GridHeaderCell cell = headerLayout.AddCell("Среднеотпускной тариф");

            cell.AddCell(String.Format("{0} год,<br/>руб./куб.м..", currentYear));
            cell.AddCell(String.Format("Темп роста к {0} году", currentYear - 1));

            cell = headerLayout.AddCell("Тариф для населения");

            cell.AddCell(String.Format("{0} год,<br/>руб./куб.м..", currentYear));
            cell.AddCell(String.Format("Темп роста к {0} году", currentYear - 1));

            cell = headerLayout.AddCell("Тариф для бюджетных потребителей");
            cell.AddCell(String.Format("{0} год,<br/>руб./куб.м..", currentYear));
            cell.AddCell(String.Format("Темп роста к {0} году", currentYear - 1));

            cell = headerLayout.AddCell("Тариф для прочих потребителей");

            cell.AddCell(String.Format("{0} год,<br/>руб./куб.м..", currentYear));
            cell.AddCell(String.Format("Темп роста к {0} году", currentYear - 1));

            headerLayout.ApplyHeaderInfo();
            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[2].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[4].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[6].CellStyle.BorderDetails.WidthRight = borderWidth;
            e.Layout.Bands[0].Columns[8].CellStyle.BorderDetails.WidthRight = borderWidth;

            e.Layout.Bands[0].Columns[0].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[1].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[3].CellStyle.BorderDetails.WidthLeft = borderWidth;
            e.Layout.Bands[0].Columns[7].CellStyle.BorderDetails.WidthLeft = borderWidth;

        }

        private int borderWidth = 3;

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {

            SetConditionCorner(e, 2, 1.065);
            SetConditionCorner(e, 4, 1.065);
            SetConditionCorner(e, 6, 1.065);
            SetConditionCorner(e, 8, 1.065);

            if (e.Row.Cells[25].Value.ToString() == "Субъект РФ")
            {
                if (e.Row.Cells[1].Value != null)
                {
                    e.Row.Cells[1].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 30px; border: 1px solid black'>{0}</td><td style='width: 65px; border: 1px solid black'>{1:N2}</td></tr></table>", GetRankImage(e, 9, 10), e.Row.Cells[1].Value);
                }
                if (e.Row.Cells[2].Value != null)
                {
                    e.Row.Cells[2].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 30px; border: 1px solid black'>{0}</td><td style='width: 40px; border: 1px solid black'>{1:P0}</td></tr></table>", GetRankImage(e, 11, 12), e.Row.Cells[2].Value);
                }
                if (e.Row.Cells[3].Value != null)
                {
                    e.Row.Cells[3].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 30px; border: 1px solid black'>{0}</td><td style='width: 65px; border: 1px solid black'>{1:N2}</td></tr></table>", GetRankImage(e, 13, 14), e.Row.Cells[3].Value);
                }
                if (e.Row.Cells[4].Value != null)
                {
                    e.Row.Cells[4].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 30px; border: 1px solid black'>{0}</td><td style='width: 40px; border: 1px solid black'>{1:P0}</td></tr></table>", GetRankImage(e, 15, 16), e.Row.Cells[4].Value);
                }
                if (e.Row.Cells[5].Value != null)
                {
                    e.Row.Cells[5].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 30px; border: 1px solid black'>{0}</td><td style='width: 65px; border: 1px solid black'>{1:N2}</td></tr></table>", GetRankImage(e, 17, 18), e.Row.Cells[5].Value);
                }
                if (e.Row.Cells[6].Value != null)
                {
                    e.Row.Cells[6].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 30px; border: 1px solid black'>{0}</td><td style='width: 40px; border: 1px solid black'>{1:P0}</td></tr></table>", GetRankImage(e, 19, 20), e.Row.Cells[6].Value);
                }
                if (e.Row.Cells[7].Value != null)
                {
                    e.Row.Cells[7].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 30px; border: 1px solid black'>{0}</td><td style='width: 65px; border: 1px solid black'>{1:N2}</td></tr></table>", GetRankImage(e, 21, 22), e.Row.Cells[7].Value);
                }
                if (e.Row.Cells[8].Value != null)
                {
                    e.Row.Cells[8].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 30px; border: 1px solid black'>{0}</td><td style='width: 40px; border: 1px solid black'>{1:P0}</td></tr></table>", GetRankImage(e, 23, 24), e.Row.Cells[8].Value);
                }
            }
            if (e.Row.Cells[25].Value != null &&
                (e.Row.Cells[25].Value.ToString() != "Субъект РФ"))
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
            if (e.Row.Cells[0].Value.ToString().ToLower().Contains("федер"))
            {
                e.Row.Style.BorderDetails.WidthTop = 3;
            }
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: right top; padding-left: 2px; padding-right: 8px; padding-top: 8px;";
            }
            if (e.Row.Cells[25].Value != null &&
                (e.Row.Cells[25].Value.ToString() == "Федеральный округ"))
            {
                e.Row.Cells[0].Value = String.Format("<a style='color: white' href='webcommand?showReport=fst_0001_0001_FO={1}'>{0}</a>", e.Row.Cells[0].Value, CustomParams.GetFoIdByName(e.Row.Cells[0].Value.ToString()));
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
                    img = "<img src='../../../images/StarGray.png'>";
                }
                else if (value == worseRankValue)
                {
                    img = "<img src='../../../images/StarYellow.png'>";
                }
            }
            return img;
        }
    }
}