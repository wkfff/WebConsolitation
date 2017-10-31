using System;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Common;
using System.Data;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.WebUI.UltraWebGrid;
using System.Web.UI.WebControls;
using System.Web;
using System.IO;
using System.Xml;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class FST_0001_0022 : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();

        // ��������� ������
        private CustomParam selectedPeriod;

        private int currentYear = 2011;
        private GridHeaderLayout headerLayout;

        protected override void Page_Load(object sender, EventArgs e)
        {
            UserParams.Region.Value = RegionsNamingHelper.FullName(UserParams.Region.Value.Replace("���", "����"));
            UltraChartFST_0001_0001_Chart1.QueryName = "FST_0001_0001_ChartHeat";

            UltraChartFST_0001_0001_Chart2.QueryName = "FST_0001_0002_ChartHeat";
            UltraChartFST_0001_0001_Chart2.TaxName = "����� ��� ���������";

            UltraChartFST_0001_0001_Chart3.QueryName = "FST_0001_0003_ChartHeat";
            UltraChartFST_0001_0001_Chart3.TaxName = "����� ���\n��������� ������������";

            UltraChartFST_0001_0001_Chart4.QueryName = "FST_0001_0004_ChartHeat";
            UltraChartFST_0001_0001_Chart4.TaxName = "����� ���\n������ ������������";

            UltraChartFST_0001_0001_Chart1.fileName = "~/FstRegionsHeat.xml";
            UltraChartFST_0001_0001_Chart2.fileName = "~/FstRegionsHeat.xml";
            UltraChartFST_0001_0001_Chart3.fileName = "~/FstRegionsHeat.xml";
            UltraChartFST_0001_0001_Chart4.fileName = "~/FstRegionsHeat.xml";

            UltraWebGrid.Width = Unit.Empty;
            UltraWebGrid.Height = Unit.Empty;
            UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid.DataBind();
        }

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FST_0001_0022_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����������", dtGrid);

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
                if (dtGrid.Rows[i]["�������"].ToString() == "����������� �����")
                {
                    if (foRowIndex == 0)
                    {
                        foRowIndex = i;
                    }
                    else
                    {
                        dtGrid.Rows[foRowIndex]["��������������� ����� �� �������� �������"] = notZeroCount == 0 ? 0 : foAvg / notZeroCount;
                        dtGrid.Rows[foRowIndex]["���� ������ �� �������� ������� "] = notZeroGrownCount == 0 ? 0 : foGrownAvg / notZeroGrownCount;

                        dtGrid.Rows[foRowIndex]["����� �� �������� ������� ��� ���������"] = notZeroDemosCount == 0 ? 0 : foDemosAvg / notZeroDemosCount;
                        dtGrid.Rows[foRowIndex]["���� ������ �� �������� ������� ��� ��������� "] = notZeroGrownDemosCount == 0 ? 0 : foGrownDemosAvg / notZeroGrownDemosCount;

                        dtGrid.Rows[foRowIndex]["����� �� �������� ������� ��� ��������� ������������"] = notZeroBujetCount == 0 ? 0 : foBujetAvg / notZeroBujetCount;
                        dtGrid.Rows[foRowIndex]["���� ������ �� �������� ������� ��� ��������� ������������ "] = notZeroGrownBujetCount == 0 ? 0 : foGrownBujetAvg / notZeroGrownBujetCount;

                        dtGrid.Rows[foRowIndex]["����� �� �������� ������� ��� ������ ������������"] = notZeroOthersCount == 0 ? 0 : foOthersAvg / notZeroOthersCount;
                        dtGrid.Rows[foRowIndex]["���� ������ �� �������� ������� ��� ������ ������������ "] = notZeroGrownOthersCount == 0 ? 0 : foGrownOthersAvg / notZeroGrownOthersCount;

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
                else if (dtGrid.Rows[i]["�������"].ToString() == "������� ��")
                {
                    double taxValue;
                    if (dtGrid.Rows[i]["��������������� ����� �� �������� �������"] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["��������������� ����� �� �������� �������"].ToString(), out taxValue) &&
                        taxValue != 0)
                    {
                        notZeroCount++;
                        foAvg += taxValue;
                    }
                    double taxDemosValue;
                    if (dtGrid.Rows[i]["����� �� �������� ������� ��� ���������"] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["����� �� �������� ������� ��� ���������"].ToString(), out taxDemosValue) &&
                        taxDemosValue != 0)
                    {
                        notZeroDemosCount++;
                        foDemosAvg += taxDemosValue;
                    }
                    double taxBujetValue;
                    if (dtGrid.Rows[i]["����� �� �������� ������� ��� ��������� ������������"] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["����� �� �������� ������� ��� ��������� ������������"].ToString(), out taxBujetValue) &&
                        taxBujetValue != 0)
                    {
                        notZeroBujetCount++;
                        foBujetAvg += taxBujetValue;
                    }
                    double taxOthersValue;
                    if (dtGrid.Rows[i]["����� �� �������� ������� ��� ������ ������������"] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["����� �� �������� ������� ��� ������ ������������"].ToString(), out taxOthersValue) &&
                        taxOthersValue != 0)
                    {
                        notZeroOthersCount++;
                        foOthersAvg += taxOthersValue;
                    }

                    double grownValue;
                    if (dtGrid.Rows[i]["���� ������ �� �������� ������� "] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["���� ������ �� �������� ������� "].ToString(), out grownValue) &&
                        grownValue != -1)
                    {
                        notZeroGrownCount++;
                        foGrownAvg += grownValue;
                    }
                    double grownDemosValue;
                    if (dtGrid.Rows[i]["���� ������ �� �������� ������� ��� ��������� "] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["���� ������ �� �������� ������� ��� ��������� "].ToString(), out grownDemosValue) &&
                        grownDemosValue != -1)
                    {
                        notZeroGrownDemosCount++;
                        foGrownDemosAvg += grownDemosValue;
                    }
                    double grownBujetValue;
                    if (dtGrid.Rows[i]["���� ������ �� �������� ������� ��� ��������� ������������ "] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["���� ������ �� �������� ������� ��� ��������� ������������ "].ToString(), out grownBujetValue) &&
                        grownBujetValue != -1)
                    {
                        notZeroGrownBujetCount++;
                        foGrownBujetAvg += grownBujetValue;
                    }
                    double grownOthersValue;
                    if (dtGrid.Rows[i]["���� ������ �� �������� ������� ��� ������ ������������ "] != DBNull.Value &&
                        Double.TryParse(dtGrid.Rows[i]["���� ������ �� �������� ������� ��� ������ ������������ "].ToString(), out grownOthersValue) &&
                        grownOthersValue != -1)
                    {
                        notZeroGrownOthersCount++;
                        foGrownOthersAvg += grownOthersValue;
                    }
                }
            }

            dtGrid.Rows[foRowIndex]["��������������� ����� �� �������� �������"] = notZeroCount == 0 ? 0 : foAvg / notZeroCount;
            dtGrid.Rows[foRowIndex]["���� ������ �� �������� ������� "] = notZeroGrownCount == 0 ? 0 : foGrownAvg / notZeroGrownCount;

            dtGrid.Rows[foRowIndex]["����� �� �������� ������� ��� ���������"] = notZeroDemosCount == 0 ? 0 : foDemosAvg / notZeroDemosCount;
            dtGrid.Rows[foRowIndex]["���� ������ �� �������� ������� ��� ��������� "] = notZeroGrownDemosCount == 0 ? 0 : foGrownDemosAvg / notZeroGrownDemosCount;

            dtGrid.Rows[foRowIndex]["����� �� �������� ������� ��� ��������� ������������"] = notZeroBujetCount == 0 ? 0 : foBujetAvg / notZeroBujetCount;
            dtGrid.Rows[foRowIndex]["���� ������ �� �������� ������� ��� ��������� ������������ "] = notZeroGrownBujetCount == 0 ? 0 : foGrownBujetAvg / notZeroGrownBujetCount;

            dtGrid.Rows[foRowIndex]["����� �� �������� ������� ��� ������ ������������"] = notZeroOthersCount == 0 ? 0 : foOthersAvg / notZeroOthersCount;
            dtGrid.Rows[foRowIndex]["���� ������ �� �������� ������� ��� ������ ������������ "] = notZeroGrownOthersCount == 0 ? 0 : foGrownOthersAvg / notZeroGrownOthersCount;



            query = DataProvider.GetQueryText("FST_0001_0021_Grid");
            DataTable dtGrid1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "����������", dtGrid1);

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
            
            for (int i = 1; i < dtGrid1.Rows.Count; i++)
            {
                if (dtGrid1.Rows[i]["�������"].ToString() == "������� ��")
                {
                    double taxValue;
                    if (dtGrid1.Rows[i]["��������������� ����� �� �������� �������"] != DBNull.Value &&
                        Double.TryParse(dtGrid1.Rows[i]["��������������� ����� �� �������� �������"].ToString(), out taxValue) &&
                        taxValue != 0)
                    {
                        notZeroCountRF++;
                        foAvgRF += taxValue;
                    }
                    double taxDemosValue;
                    if (dtGrid1.Rows[i]["����� �� �������� ������� ��� ���������"] != DBNull.Value &&
                        Double.TryParse(dtGrid1.Rows[i]["����� �� �������� ������� ��� ���������"].ToString(), out taxDemosValue) &&
                        taxDemosValue != 0)
                    {
                        notZeroDemosCountRF++;
                        foDemosAvgRF += taxDemosValue;
                    }
                    double taxBujetValue;
                    if (dtGrid1.Rows[i]["����� �� �������� ������� ��� ��������� ������������"] != DBNull.Value &&
                        Double.TryParse(dtGrid1.Rows[i]["����� �� �������� ������� ��� ��������� ������������"].ToString(), out taxBujetValue) &&
                        taxBujetValue != 0)
                    {
                        notZeroBujetCountRF++;
                        foBujetAvgRF += taxBujetValue;
                    }
                    double taxOthersValue;
                    if (dtGrid1.Rows[i]["����� �� �������� ������� ��� ������ ������������"] != DBNull.Value &&
                        Double.TryParse(dtGrid1.Rows[i]["����� �� �������� ������� ��� ������ ������������"].ToString(), out taxOthersValue) &&
                        taxOthersValue != 0)
                    {
                        notZeroOthersCountRF++;
                        foOthersAvgRF += taxOthersValue;
                    }

                    double grownValue;
                    if (dtGrid1.Rows[i]["���� ������ �� �������� ������� "] != DBNull.Value &&
                        Double.TryParse(dtGrid1.Rows[i]["���� ������ �� �������� ������� "].ToString(), out grownValue) &&
                        grownValue != -1)
                    {
                        notZeroGrownCountRF++;
                        foGrownAvgRF += grownValue;
                    }
                    double grownDemosValue;
                    if (dtGrid1.Rows[i]["���� ������ �� �������� ������� ��� ��������� "] != DBNull.Value &&
                        Double.TryParse(dtGrid1.Rows[i]["���� ������ �� �������� ������� ��� ��������� "].ToString(), out grownDemosValue) &&
                        grownDemosValue != -1)
                    {
                        notZeroGrownDemosCountRF++;
                        foGrownDemosAvgRF += grownDemosValue;
                    }
                    double grownBujetValue;
                    if (dtGrid1.Rows[i]["���� ������ �� �������� ������� ��� ��������� ������������ "] != DBNull.Value &&
                        Double.TryParse(dtGrid1.Rows[i]["���� ������ �� �������� ������� ��� ��������� ������������ "].ToString(), out grownBujetValue) &&
                        grownBujetValue != -1)
                    {
                        notZeroGrownBujetCountRF++;
                        foGrownBujetAvgRF += grownBujetValue;
                    }
                    double grownOthersValue;
                    if (dtGrid1.Rows[i]["���� ������ �� �������� ������� ��� ������ ������������ "] != DBNull.Value &&
                        Double.TryParse(dtGrid1.Rows[i]["���� ������ �� �������� ������� ��� ������ ������������ "].ToString(), out grownOthersValue) &&
                        grownOthersValue != -1)
                    {
                        notZeroGrownOthersCountRF++;
                        foGrownOthersAvgRF += grownOthersValue;
                    }
                }
            }

            dtGrid.Rows[0]["��������������� ����� �� �������� �������"] = notZeroCountRF == 0 ? 0 : foAvgRF / notZeroCountRF;
            dtGrid.Rows[0]["���� ������ �� �������� ������� "] = notZeroGrownCountRF == 0 ? 0 : foGrownAvgRF / notZeroGrownCountRF;

            UltraChartFST_0001_0001_Chart1.RfMiddleLevel = notZeroGrownCountRF == 0 ? 0 : (foGrownAvgRF / notZeroGrownCountRF);

            dtGrid.Rows[0]["����� �� �������� ������� ��� ���������"] = notZeroDemosCountRF == 0 ? 0 : foDemosAvgRF / notZeroDemosCountRF;
            dtGrid.Rows[0]["���� ������ �� �������� ������� ��� ��������� "] = notZeroGrownDemosCountRF == 0 ? 0 : foGrownDemosAvgRF / notZeroGrownDemosCountRF;

            UltraChartFST_0001_0001_Chart2.RfMiddleLevel = notZeroGrownDemosCountRF == 0 ? 0 : (foGrownDemosAvgRF / notZeroGrownDemosCountRF);

            dtGrid.Rows[0]["����� �� �������� ������� ��� ��������� ������������"] = notZeroBujetCountRF == 0 ? 0 : foBujetAvgRF / notZeroBujetCountRF;
            dtGrid.Rows[0]["���� ������ �� �������� ������� ��� ��������� ������������ "] = notZeroGrownBujetCountRF == 0 ? 0 : foGrownBujetAvgRF / notZeroGrownBujetCountRF;

            UltraChartFST_0001_0001_Chart3.RfMiddleLevel = notZeroGrownBujetCountRF == 0 ? 0 : (foGrownBujetAvgRF / notZeroGrownBujetCountRF);

            dtGrid.Rows[0]["����� �� �������� ������� ��� ������ ������������"] = notZeroOthersCountRF == 0 ? 0 : foOthersAvgRF / notZeroOthersCountRF;
            dtGrid.Rows[0]["���� ������ �� �������� ������� ��� ������ ������������ "] = notZeroGrownOthersCountRF == 0 ? 0 : foGrownOthersAvgRF / notZeroGrownOthersCountRF;

            UltraChartFST_0001_0001_Chart4.RfMiddleLevel =  notZeroGrownOthersCountRF == 0 ? 0 : (foGrownOthersAvgRF / notZeroGrownOthersCountRF);

            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            e.Layout.Bands[0].Columns[0].Width = 190;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[1].Width = 132;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N2");

            e.Layout.Bands[0].Columns[2].Width = 126;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "P2");

            e.Layout.Bands[0].Columns[3].Width = 132;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N2");

            e.Layout.Bands[0].Columns[4].Width = 126;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "P2");

            e.Layout.Bands[0].Columns[5].Width = 132;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N2");

            e.Layout.Bands[0].Columns[6].Width = 126;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "P2");

            e.Layout.Bands[0].Columns[7].Width = 132;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N2");

            e.Layout.Bands[0].Columns[8].Width = 126;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "P2");

            for (int i = 9; i < 26; i++)
            {
                e.Layout.Bands[0].Columns[i].Hidden = true;
            }

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            headerLayout.AddCell(" ");

            GridHeaderCell cell = headerLayout.AddCell("��������������� �����");

            cell.AddCell(String.Format("{0} ���,<br/>���./����.", currentYear));
            cell.AddCell(String.Format("������� ������ {0}/{1}", currentYear, currentYear - 1));

            cell = headerLayout.AddCell("����� ��� ���������");

            cell.AddCell(String.Format("{0} ���,<br/>���./����.", currentYear));
            cell.AddCell(String.Format("������� ������ {0}/{1}", currentYear, currentYear - 1));

            cell = headerLayout.AddCell("����� ��� ��������� ������������");
            cell.AddCell(String.Format("{0} ���,<br/>���./����.", currentYear));
            cell.AddCell(String.Format("������� ������ {0}/{1}", currentYear, currentYear - 1));

            cell = headerLayout.AddCell("����� ��� ������ ������������");

            cell.AddCell(String.Format("{0} ���,<br/>���./����.", currentYear));
            cell.AddCell(String.Format("������� ������ {0}/{1}", currentYear, currentYear - 1));

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
            if (e.Row.Index != 0)
            {
                SetConditionCorner(e, 2, Convert.ToDouble(e.Row.Band.Grid.Rows[0].Cells[2].Value));
                SetConditionCorner(e, 4, Convert.ToDouble(e.Row.Band.Grid.Rows[0].Cells[4].Value));
                SetConditionCorner(e, 6, Convert.ToDouble(e.Row.Band.Grid.Rows[0].Cells[6].Value));
                SetConditionCorner(e, 8, Convert.ToDouble(e.Row.Band.Grid.Rows[0].Cells[8].Value));
            }

            if (e.Row.Cells[25].Value.ToString() == "������� ��")
            {
                if (e.Row.Cells[1].Value != null)
                {
                    e.Row.Cells[1].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 40px; border: 1px solid black'>{0}</td><td style='width: 70px; border: 1px solid black'>{1:N2}</td></tr></table>", GetRankImage(e, 9, 10), e.Row.Cells[1].Value);
                }
                if (e.Row.Cells[2].Value != null)
                {
                    e.Row.Cells[2].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 40px; border: 1px solid black'>{0}</td><td style='width: 65px; border: 1px solid black'>{1:P2}</td></tr></table>", GetRankImage(e, 11, 12), e.Row.Cells[2].Value);
                }
                if (e.Row.Cells[3].Value != null)
                {
                    e.Row.Cells[3].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 40px; border: 1px solid black'>{0}</td><td style='width: 70px; border: 1px solid black'>{1:N2}</td></tr></table>", GetRankImage(e, 13, 14), e.Row.Cells[3].Value);
                }
                if (e.Row.Cells[4].Value != null)
                {
                    e.Row.Cells[4].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 40px; border: 1px solid black'>{0}</td><td style='width: 65px; border: 1px solid black'>{1:P2}</td></tr></table>", GetRankImage(e, 15, 16), e.Row.Cells[4].Value);
                }
                if (e.Row.Cells[5].Value != null)
                {
                    e.Row.Cells[5].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 40px; border: 1px solid black'>{0}</td><td style='width: 70px; border: 1px solid black'>{1:N2}</td></tr></table>", GetRankImage(e, 17, 18), e.Row.Cells[5].Value);
                }
                if (e.Row.Cells[6].Value != null)
                {
                    e.Row.Cells[6].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 40px; border: 1px solid black'>{0}</td><td style='width: 65px; border: 1px solid black'>{1:P2}</td></tr></table>", GetRankImage(e, 19, 20), e.Row.Cells[6].Value);
                }
                if (e.Row.Cells[7].Value != null)
                {
                    e.Row.Cells[7].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 40px; border: 1px solid black'>{0}</td><td style='width: 70px; border: 1px solid black'>{1:N2}</td></tr></table>", GetRankImage(e, 21, 22), e.Row.Cells[7].Value);
                }
                if (e.Row.Cells[8].Value != null)
                {
                    e.Row.Cells[8].Value = String.Format("<table style='border: 1px solid black; border-collapse: collapse; margin-right: -4px'><tr><td style='width: 40px; border: 1px solid black'>{0}</td><td style='width: 65px; border: 1px solid black'>{1:P2}</td></tr></table>", GetRankImage(e, 23, 24), e.Row.Cells[8].Value);
                }
            }
            if (e.Row.Cells[0].Value.ToString().ToLower().Contains("�����"))
            {
                e.Row.Style.BorderDetails.WidthTop = 3;
            }
            else
            {
                e.Row.Cells[0].Value = GetName(e.Row.Cells[0].Value.ToString());
            }
            if (e.Row.Cells[25].Value != null &&
                (e.Row.Cells[25].Value.ToString() != "������� ��"))
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
            
            if (e.Row.Cells[25].Value != null &&
               (e.Row.Cells[25].Value.ToString() == "��"))
            {
                e.Row.Cells[0].Value = String.Format("<a style='color: white' href='webcommand?showReport=fst_0001_0002'>{0}</a>", e.Row.Cells[0].Value);
            }
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: right top; padding-left: 2px; padding-right: 8px; padding-top: 8px;";
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
                    img = "<img src='../../../images/max.png'>";
                }
                else if (value == worseRankValue)
                {
                    img = "<img src='../../../images/min.png'>";
                }
            }
            return img;
        }

        private string GetName(string name)
        {
            // ��������� ��������
            string xmlFile = HttpContext.Current.Server.MapPath("~/FstRegionsHeat.xml");
            if (!File.Exists(xmlFile))
            {
                return name;
            }
            else
            {                  
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFile);
                // ���� ���� ��������
                foreach (XmlNode rootNode in doc.ChildNodes)
                {
                    if (rootNode.Name == "RegionsList")
                    {
                        foreach (XmlNode regionNode in rootNode.ChildNodes)
                        {
                            if (regionNode.Attributes["name"].Value == name)
                            {
                                return name;
                            }
                        }
                    }
                }
            }
            Label3.Visible = true;
            return name += '*';
        }
    }
}