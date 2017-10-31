using System;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color=System.Drawing.Color;
using Font=System.Drawing.Font;

namespace Krista.FM.Server.Dashboards.reports.MFRF_0004_0002
{
    public partial class DefaultEvenness : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2008;
        private int endYear = 2009;
        private bool yearValuation = true;

        private const double yearLimit1_123n = 20;
        private const double yearLimit1_34n = 50;
        private const double yearLimit2 = 100;
        private const double quarterLimit1 = 88.9;
        private const double quarterLimit2 = 111.1;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 190);
            UltraWebGrid.EnableViewState = false;

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 - 22);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 160);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.5 - 22);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.5 - 160);

            #region ��������� ��������

            UltraChart1.ChartType = ChartType.PieChart;
            UltraChart1.PieChart.Labels.FormatString = "<ITEM_LABEL>; <DATA_VALUE:N0>";
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart1.PieChart.OthersCategoryPercent = 0;

            UltraChart2.ChartType = ChartType.PieChart;
            UltraChart2.PieChart.Labels.FormatString = "<ITEM_LABEL>; <DATA_VALUE:N0>";
            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart2.PieChart.OthersCategoryPercent = 0;

            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart2.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart2.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            
            #endregion

            GridSearch1.LinkedGridId = this.UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.RowExporting += new RowExportingEventHandler(ExcelExporter_RowExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            UltraGridExporter1.MultiHeader = true;

            CrossLink1.Text = "��������&nbsp;����������&nbsp;��������&nbsp;����";
            CrossLink1.NavigateUrl = "~/reports/MFRF_0004_0004/DefaultDynamic.aspx";
            CrossLink2.Text = "���������&nbsp;�������������&nbsp;��������&nbsp;����";
            CrossLink2.NavigateUrl = "~/reports/MFRF_0004_0002/DefaultEvennessChart.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("MFRF_0004_0002_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();
                string quarter;
                int monthNum = CRHelper.MonthNum(month);
                int quarterNum = CRHelper.QuarterNumByMonthNum(monthNum);

                if (monthNum % 3 == 0)
                {
                    quarter = string.Format("������� {0}", quarterNum);
                }
                else
                {
                    if (quarterNum == 1)
                    {
                        endYear = endYear - 1;
                        quarter = "������� 4";
                    }
                    else
                    {
                        quarter = string.Format("������� {0}", quarterNum - 1);
                    }
                }

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;

                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));

                ComboYear.Set�heckedState(endYear.ToString(), true);

                ComboQuarter.Title = "�������";
                ComboQuarter.Width = 150;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillQuaters());
                ComboQuarter.Set�heckedState(quarter, true);
            }

            Label1.Text = "������ ������������� �������� ����"; ;
            Page.Title = Label1.Text;
            
            yearValuation = (MeasureButtonList.SelectedIndex == 0);
            
            if (yearValuation)
            {
                ComboYear.ClearNodes();
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(2008, 2008));
                ComboYear.Set�heckedState("2008", true);

                UltraWebGrid.DataBinding +=new EventHandler(UltraWebGrid_DataBinding_yearValuation);
                UltraWebGrid.InitializeLayout +=new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout_yearValuation);
                UltraWebGrid.InitializeRow +=new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow_yearValuation);
                ComboQuarter.Visible = false;
                Label2.Text = string.Format("������ ���������� ������� ���� � ����� �������� �������� ��������� �� ����������� ���������� ����������� �������� ����������� �����������, ��������������� ���� �� {0} ���", ComboYear.SelectedValue);
            }
            else
            {
                string year = ComboYear.SelectedValue;
                ComboYear.ClearNodes();
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(year, true);

                UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding_quarterValuation);
                UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout_quarterValuation);
                UltraWebGrid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(UltraWebGrid_InitializeRow_quarterValuation);
                ComboQuarter.Visible = true;
                Label2.Text = string.Format("������ ���������� ������� ���� � ����� �������� �������� ��������� �� ����������� ���������� ����������� �������� ����������� �����������, ��������������� ���� �� {0} ������� {1} ����", 
                    ComboQuarter.SelectedIndex + 1, ComboYear.SelectedValue);

                UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByQuarterNum(ComboQuarter.SelectedIndex + 1));
                UserParams.PeriodQuater.Value = string.Format("������� {0}", ComboQuarter.SelectedIndex + 1);
            }

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;

            if (UserParams.PeriodYear.Value == "2009" && UserParams.PeriodQuater.Value == "������� 2")
            {
                ValuationCount123nLabel.Text = string.Empty;
                ValuationCount34nLabel.Text = string.Empty;
                AVGEvennessLabel.Text = string.Empty;
                MinEvennessLabel.Text = string.Empty;
                MaxEvennessLabel.Text = string.Empty;
                return;
            }

            UltraWebGrid.DataBind();
            if (yearValuation)
            {
                UltraChart2.DataBind();
                UltraChart2.Visible = true;
                chartTable.Visible = true;
                lbSubject2.Visible = true;
                lbSubject1.Text = "������ ���� �� ������� �123�";
                lbSubject2.Text = "������ ���� �� ������� �34�";

                tdChart1.ColSpan = 1;
                tdChart1.Width = "50%";
            }
            else
            {
                UltraChart2.Visible = false;
                chartTable.Visible = false;
                lbSubject2.Visible = false;
                lbSubject1.Text = "������ ���� �� ������� �34�";

                tdChart1.ColSpan = 2;
                tdChart1.Width = "100%";
            }

            UltraChart1.DataBind();
            
            if (dtGrid != null && dtGrid.Rows.Count != 0)
            {
                DetailsDataBind();
            }
        }

        private void DetailsDataBind()
        {
            string query = (yearValuation) ? DataProvider.GetQueryText("MFRF_0004_0002_details_yearValuation") : DataProvider.GetQueryText("MFRF_0004_0002_details_quarterValuation");
            DataTable dtDetails = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, " ", dtDetails);

            int countGRBS = dtDetails.Rows[0][6] != DBNull.Value ? Convert.ToInt32(dtDetails.Rows[0][6]) : 0;

            if (yearValuation)
            {
                ValuationCount123nLabel.Visible = true;
                int valuation123nCount = dtDetails.Rows[0][7] != DBNull.Value ? Convert.ToInt32(dtDetails.Rows[0][7]) : 0;
                ValuationCount123nLabel.Text = string.Format("���������� ����, �� ��������������� ��������, �������������� �������� 123�: <b>{0}</b> �� <b>{1}</b> ���������<br />", 
                    valuation123nCount, countGRBS);
            }
            else
            {
                ValuationCount123nLabel.Text = string.Empty;
                ValuationCount123nLabel.Visible = false;
            }

            string kindCrime = (yearValuation) ? "�� ���������������" : "���������������";
            int valuation34nCount = dtDetails.Rows[0][0] != DBNull.Value ? Convert.ToInt32(dtDetails.Rows[0][0]) : 0;
            ValuationCount34nLabel.Text = string.Format("���������� ����, {2} ��������, �������������� �������� 34�: <b>{0}</b> �� <b>{1}</b> ���������",
                valuation34nCount, countGRBS, kindCrime);

            double avgEvenness = dtDetails.Rows[0][1] != DBNull.Value ? Convert.ToDouble(dtDetails.Rows[0][1]) : 0;
            AVGEvennessLabel.Text = string.Format("������� ������������� ��������: <b>{0:P2}</b>", avgEvenness);

            double minEvenness = dtDetails.Rows[0][2] != DBNull.Value ? Convert.ToDouble(dtDetails.Rows[0][2]) : 0;
            string minEvennessGRBS = dtDetails.Rows[0][3] != DBNull.Value ? dtDetails.Rows[0][3].ToString() : string.Empty;
            MinEvennessLabel.Text = string.Format("����������� ������������� �������� : <b>{0:P2} ({1})</b>", minEvenness, DataDictionariesHelper.GetShortGRBSName(minEvennessGRBS));


            double maxEvenness = dtDetails.Rows[0][4] != DBNull.Value ? Convert.ToDouble(dtDetails.Rows[0][4]) : 0;
            string maxEvennessGRBS = dtDetails.Rows[0][5] != DBNull.Value ? dtDetails.Rows[0][5].ToString() : string.Empty;
            MaxEvennessLabel.Text = string.Format("������������ ������������� �������� : <b>{0:P2} ({1})</b>", maxEvenness, DataDictionariesHelper.GetShortGRBSName(maxEvennessGRBS));
        }


        #region ����������� �����

        protected void UltraWebGrid_DataBinding_yearValuation(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("MFRF_0004_0002_grid_yearValuation");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "������������ ���� ��", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value && (i == 2 || i == 3 || i == 4 || i == 5 || i == 6))
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }
            }

            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout_yearValuation(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(380);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 100;

                switch (i)
                {
                    case 1:
                        {
                            formatString = "N0";
                            widthColumn = 42;
                            break;
                        }
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        {
                            formatString = "N3";
                            widthColumn = 80;
                            break;
                        }
                    case 7:
                        {
                            formatString = "P2";
                            widthColumn = 100;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i != 2 && i != 3 && i != 4 && i != 5)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "�������� ����������, ���.���.", 2, 0, 4, 1);

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "������������ ���� ��", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "���", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "������� 1", "�������� ������� �� 1 ������� ��������� �������");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "������� 2", "�������� ������� �� 2 ������� ��������� �������");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, "������� 3", "�������� ������� �� 3 ������� ��������� �������");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 5, "������� 4", "�������� ������� �� 4 ������� ��������� �������");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, "������� �������� ���������� 1-3 �������, ���.���.", "������� ����� �������� �������� �� 1-3 ������� ��������� �������");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, "������������� ��������", "������������� �������� ���� � �������� ���������� ����");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, "������ ������������� �������� (������ 123�)", "������ �������� ������� ������� �� �� 10 ������� 2007 ���� �123�");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 9, "������ ������������� �������� (������ 34�)", "������ �������� ������� ������� �� �� 13 ������ 2009 ���� �34�");
        }

        protected void UltraWebGrid_InitializeRow_yearValuation(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[i].Value != null && (i == 8 || i == 9))
                {
                    if (e.Row.Cells[i].Value.ToString() != "�� �������")
                    {
                        string hint = string.Empty;
                        e.Row.Cells[i].Style.BackgroundImage =
                            GetIndicatorStyle(e.Row.Cells[i].Value.ToString(), ref hint, i == 8);
                        e.Row.Cells[i].Title = hint;
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: center center; margin: 2px";

                        // ��� ���������� �� ����� �����������
                        if (e.Row.Cells[i].Style.BackgroundImage.Contains("Red"))
                        {
                            e.Row.Cells[i].Value = "   ";
                        }
                        else if (e.Row.Cells[i].Style.BackgroundImage.Contains("Yellow"))
                        {
                            e.Row.Cells[i].Value = "  ";
                        }
                        else if (e.Row.Cells[i].Style.BackgroundImage.Contains("Green"))
                        {
                            e.Row.Cells[i].Value = " ";
                        }
                        else
                        {
                            e.Row.Cells[i].Value = string.Empty;
                        }
                    }
                    else
                    {
                        e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Center;
                    }
                }

                UltraGridCell c = e.Row.Cells[i];
                if (c.Value != null && c.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(c.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            c.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        protected void UltraWebGrid_DataBinding_quarterValuation(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("MFRF_0004_0002_grid_quarterValuation");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "������������ ���� ��", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value && (i == 2 || i == 3))
                    {
                        row[i] = Convert.ToDouble(row[i]) / 1000000;
                    }
                }
            }

            UltraWebGrid.DataSource = dtGrid;
        }

        protected void UltraWebGrid_InitializeLayout_quarterValuation(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(525);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 100;

                switch (i)
                {
                    case 1:
                        {
                            formatString = "N0";
                            widthColumn = 50;
                            break;
                        }
                    case 2:
                    case 3:
                        {
                            formatString = "N3";
                            widthColumn = 150;
                            break;
                        }
                    case 4:
                        {
                            formatString = "P2";
                            widthColumn = 150;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
            }

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "������������ ���� ��", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, "���", "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, "��������� ������������, ���.���.", "����� ��������� ������������ �������� ��������� �������");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, "�������� ����������, ���.���.", "�������� ������� � �������� �������");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 7, "������������� ��������", "������������� �������� ���� � �������� ���������� ����");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, "������ ������������� �������� (������ 34�)", "������ �������� ������� ������� �� �� 13 ������ 2009 ���� �34�");
        }

        protected void UltraWebGrid_InitializeRow_quarterValuation(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {

                
                if (e.Row.Cells[i].Value != null && (i == 5))
                {
                    string hint = string.Empty;
                    e.Row.Cells[i].Style.BackgroundImage = GetIndicatorStyle(e.Row.Cells[i].Value.ToString(), ref hint, true);
                    e.Row.Cells[i].Title = hint;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: center center; margin: 2px";

                    if (e.Row.Cells[i].Style.BackgroundImage.Contains("Red"))
                    {
                        e.Row.Cells[i].Value = "   ";
                    }
                    else if (e.Row.Cells[i].Style.BackgroundImage.Contains("Yellow"))
                    {
                        e.Row.Cells[i].Value = "  ";
                    }
                    else if (e.Row.Cells[i].Style.BackgroundImage.Contains("Green"))
                    {
                        e.Row.Cells[i].Value = " ";
                    }
                    else
                    {
                        e.Row.Cells[i].Value = string.Empty;
                    }
                }

                UltraGridCell c = e.Row.Cells[i];
                if (c.Value != null && c.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(c.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            c.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ��������� ������� �������� ��� ������ �� ��������
        /// </summary>
        /// <param name="cellValue">�������� ������</param>
        /// <param name="hintText">���� � ������</param>
        /// <param name="brightColor">������������ �� ����� �����</param>
        /// <returns>������ �� ������� �������� ������</returns>
        private string GetIndicatorStyle(string cellValue, ref string hintText, bool brightColor)
        {
            string dim = (!brightColor) ? "dim" : string.Empty;
            string colorStyle = string.Empty;

            switch (cellValue)
            {
                case "�������������":
                    {
                        colorStyle = "ballGreenBB";
                        if (yearValuation)
                        {
                            hintText = string.Format("������������� �������� ������ (����� {0}%)", brightColor ? yearLimit1_123n : yearLimit1_34n);
                        }
                        else
                        {
                            hintText = "������������� �������� ������ (�� 88,9% �� 111,1%)";
                        }
                        break;
                    }
                case "�� �������������":
                    {
                        colorStyle = "ballRedBB";
                        hintText = string.Format("�� ������������� �������� ������ (����� {0}%)", yearLimit2);
                        break;
                    }
                case "� ����������� ���������":
                    {
                        colorStyle = "ballYellowBB";
                        if (yearValuation)
                        {
                            hintText = string.Format("������������� �������� � ����������� ��������� (�� {0}% �� 100%)", (brightColor) ? 20 : 50);
                        }
                        else
                        {
                            hintText = "������������� �������� � ����������� ��������� (�� 88,9% � ����� 111,1%)";
                        }
                        break;
                    }
            }

            return string.Format("~/images/{0}{1}.png", colorStyle, dim);
        }

        /// <summary>
        /// ��������� ������ ���������� �� ������� ��������
        /// </summary>
        /// <param name="backgroundImage">������� ��������</param>
        /// <returns>����� ����������</returns>
        private static string GetIndicatorText(string backgroundImage)
        {
            if (backgroundImage.Contains("ballGreenBB"))
            {
                return "�������������";
            }

            if (backgroundImage.Contains("ballRedBB"))
            {
                return "�� �������������";
            }

            if (backgroundImage.Contains("ballYellowBB"))
            {
                return "� ����������� ���������";
            }

            return string.Empty;
        }

        #endregion

        #region ����������� ��������

        private string[] subjectList = new string[3];

        /// <summary>
        /// ���������� � ������ ��������� (��� �����)
        /// </summary>
        /// <param name="array">������</param>
        /// <param name="index">������ ������</param>
        /// <param name="fullName">������ ��� ��������</param>
        private static void AddSubject(string[] array, int index, string fullName)
        {
            if (array == null || index >= array.Length)
            {
                return;
            }

            string shortName = DataDictionariesHelper.GetShortGRBSName(fullName);

            if (array[index] == null)
            {
                // ������ �������
                array[index] = shortName;
            }
            else
            {
                string[] strs = array[index].Split(',');
                // ����� ������ 5 ������ ������� ������
                if (strs.Length % 5 == 0)
                {
                    array[index] = string.Format("{0},\n{1}", array[index], shortName);
                }
                else
                {
                    array[index] = string.Format("{0}, {1}", array[index], shortName);
                }
            }
        }

        /// <summary>
        /// ��������� ������ ������
        /// </summary>
        /// <param name="value">��������</param>
        /// <returns>����� ������</returns>
        private static int GetGroupNumber(double value, double limit1, double limit2)
        {
            value = 100 * value;

            if (value < limit1)
            {
                return 0;
            }

            if (value >= limit1 && value < limit2)
            {
                return 1;
            }

            if (value >= limit2)
            {
                return 2;
            }

            return 0;
        }

        /// <summary>
        /// ��������� ����� ���������
        /// </summary>
        /// <param name="groupRowIndex">����� ������ � �������</param>
        /// <returns></returns>
        private static string GetIntervalName(int groupRowIndex, double limit1, double limit2)
        {
            switch (groupRowIndex)
            {
                case 0:
                {
                    return string.Format("����� {0}%", limit1);
                }
                case 1:
                {
                    return string.Format("�� {0}% �� {1}%", limit1, limit2);
                }
                case 2:
                {
                    return string.Format("����� {0}%", limit2);
                }
                default:
                {
                    return string.Empty;
                }
            }
        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            if (dtGrid == null || dtGrid.Rows.Count == 0)
            {
                UltraWebGrid.Height = Unit.Empty;

                UltraChart1.DataSource = null;
                UltraChart2.DataSource = null;

                ValuationCount34nLabel.Text = string.Empty;
                ValuationCount123nLabel.Text = string.Empty;
                AVGEvennessLabel.Text = string.Empty;
                MinEvennessLabel.Text = string.Empty;
                MaxEvennessLabel.Text = string.Empty;

                return;
            }

            int columnIndex;
            double limit1;
            double limit2;

            if (yearValuation)
            {
                columnIndex = 7;
                limit1 = ((UltraChart)sender == UltraChart1) ? yearLimit1_123n : yearLimit1_34n;
                limit2 = yearLimit2;
            }
            else
            {
                columnIndex = 4;
                limit1 = quarterLimit1;
                limit2 = quarterLimit2;
            }

            int[] groupArray = new int[3];
            subjectList = new string[3];

            foreach (DataRow row in dtGrid.Rows)
            {
                if (row[columnIndex] != DBNull.Value && row[columnIndex].ToString() != string.Empty)
                {
                    double value = Convert.ToDouble(row[columnIndex]);
                    int groupNum = GetGroupNumber(value, limit1, limit2);
                    groupArray.SetValue(Convert.ToInt32(groupArray.GetValue(groupNum)) + 1, groupNum);

                    AddSubject(subjectList, groupNum, row[0].ToString());
                }
            }

            DataTable dt = new DataTable();
            DataColumn column1 = new DataColumn("������", typeof(string));
            dt.Columns.Add(column1);
            DataColumn column2 = new DataColumn("����� ����", typeof(double));
            dt.Columns.Add(column2);

            for (int i = 0; i < 3; i++)
            {
                string rowName;

                if (yearValuation)
                {
                    rowName = GetIntervalName(i, limit1, limit2);
                }
                else
                {
                    if (i == 0)
                    {
                        rowName = "�� 88,9% � ����� 111,1%";
                    }
                    else if (i == 1)
                    {
                        rowName = "�� 88,9% �� 111,1%";
                    }
                    else
                    {
                        rowName = "����� 111,1%";
                    }
                }

                DataRow row = dt.NewRow();
                row[0] = rowName.ToString();
                row[1] = groupArray[i];
                dt.Rows.Add(row);
            }

            ((UltraChart)sender).Series.Clear();

            NumericSeries series1 = CRHelper.GetNumericSeries(1, dt);
            ((UltraChart)sender).Series.Add(series1);
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            double limit1;
            double limit2;
            byte chartOpacity;

            if (yearValuation)
            {
                chartOpacity = ((UltraChart)sender == UltraChart1) ? (byte)255 : (byte)50;
                limit1 = ((UltraChart)sender == UltraChart1) ? yearLimit1_123n : yearLimit1_34n;
                limit2 = yearLimit2;
            }
            else
            {
                chartOpacity = (byte)255;
                limit1 = quarterLimit1;
                limit2 = quarterLimit2;
            }

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Wedge)
                {
                    Wedge wedge = (Wedge)primitive;
                    if (wedge.DataPoint != null && wedge.Value != null)
                    {
                        string valuation = string.Empty;

                        switch (wedge.Row)
                        {
                            case 0:
                                {
                                    wedge.PE.Fill = (yearValuation) ? Color.Green : Color.Yellow;
                                    wedge.PE.FillStopColor = (yearValuation) ? Color.DarkGreen : Color.Gold;
                                    valuation = "� ����������� ���������";
                                    break;
                                }
                            case 1:
                                {
                                    wedge.PE.Fill = (yearValuation) ? Color.Yellow : Color.Green;
                                    wedge.PE.FillStopColor = (yearValuation) ? Color.Gold : Color.DarkGreen;
                                    valuation = "������������ �������� ������";
                                    break;
                                }
                            case 2:
                                {
                                    wedge.PE.Fill = Color.Red;
                                    wedge.PE.FillStopColor = Color.DarkRed;
                                    valuation = "�� ������������ �������� ������";
                                    break;
                                }
                        }

                        string intervalName;
                        if (yearValuation)
                        {
                            intervalName = GetIntervalName(wedge.Row, limit1, limit2);
                        }
                        else
                        {
                            intervalName = (wedge.Row == 0) ? "�� 88,9% � ����� 111,1%" : "�� 88,9% �� 111,1%";
                        }

                        wedge.DataPoint.Label = string.Format("������������� �������� {0} ({2})\n({1})",
                                intervalName,
                                subjectList[wedge.Row],
                                valuation);

                        wedge.PE.FillOpacity = chartOpacity;
                    }
                }
            }
        }

        #endregion

        #region ������� � Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            for (int i = 0; i < UltraWebGrid.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "#,##0.00;[Red]-#,##0.00";
                int widthColumn = 500;

                if (yearValuation)
                {
                    switch (i)
                    {
                        case 1:
                            {
                                formatString = "#,##0";
                                widthColumn = 60;
                                break;
                            }
                        case 7:
                            {
                                formatString = UltraGridExporter.ExelPercentFormat;
                                widthColumn = 60;
                                break;
                            }
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            {
                                formatString = "#,##0.000;[Red]-#,##0.000";
                                widthColumn = 95;
                                break;
                            }
                        case 8:
                        case 9:
                            {
                                widthColumn = 150;
                                break;
                            }
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 1:
                            {
                                formatString = "#,##0";
                                widthColumn = 60;
                                break;
                            }
                        case 5:
                        case 7:
                            {
                                formatString = "#,##0";
                                widthColumn = 150;
                                break;
                            }
                        case 2:
                        case 3:
                            {
                                formatString = "#,##0.000;[Red]-#,##0.000";
                                widthColumn = 95;
                                break;
                            }
                        case 4:
                        case 6:
                            {
                                formatString = UltraGridExporter.ExelPercentFormat;
                                widthColumn = 85;
                                break;
                            }
                        case 8:
                        case 9:
                            {
                                widthColumn = 150;
                                break;
                            }
                    }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = widthColumn * 37;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_RowExporting(object sender, RowExportingEventArgs e)
        {
            if (yearValuation)
            {
                e.CurrentWorksheet.Rows[e.GridRow.Index + 4].Cells[8].Value = GetIndicatorText(e.GridRow.Cells[8].Style.BackgroundImage);
                e.CurrentWorksheet.Rows[e.GridRow.Index + 4].Cells[9].Value = GetIndicatorText(e.GridRow.Cells[9].Style.BackgroundImage);
                if (e.GridRow.Cells[8].Value != null && e.GridRow.Cells[8].Value.ToString() != "�� �������")
                {
                    e.GridRow.Cells[8].Value = e.CurrentWorksheet.Rows[e.GridRow.Index + 4].Cells[8].Value;
                }
                if (e.GridRow.Cells[9].Value != null && e.GridRow.Cells[9].Value.ToString() != "�� �������")
                {
                    e.GridRow.Cells[9].Value = e.CurrentWorksheet.Rows[e.GridRow.Index + 4].Cells[9].Value;
                }
            }
            else
            {
                e.CurrentWorksheet.Rows[e.GridRow.Index + 4].Cells[5].Value = GetIndicatorText(e.GridRow.Cells[5].Style.BackgroundImage);
                if (e.GridRow.Cells[5].Value != null && e.GridRow.Cells[5].Value.ToString() != "�� �������")
                {
                    e.GridRow.Cells[5].Value = e.CurrentWorksheet.Rows[e.GridRow.Index + 4].Cells[5].Value;
                }
            }
        }

        #endregion

        #region ������� � Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            UltraWebGrid.Bands[0].Columns[2].Header.Caption = UltraWebGrid.Bands[0].Columns[2].Header.Caption.Replace("&nbsp;", " ");
            UltraWebGrid.Bands[0].Columns[3].Header.Caption = UltraWebGrid.Bands[0].Columns[3].Header.Caption.Replace("&nbsp;", " ");

            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            ITable table = e.Section.AddTable();
            ITableRow row = table.AddRow();
            ITableCell cell = row.AddCell();
            IText title = cell.AddText();
            title.Alignment = TextAlignment.Center;
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(lbSubject1.Text);

            cell.AddImage(GetImageFromChart(UltraChart1));

            cell = row.AddCell();
            title = cell.AddText();
            title.Alignment = TextAlignment.Center;
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(lbSubject2.Text);

            cell.AddImage(GetImageFromChart(UltraChart2));
        }

        private static Infragistics.Documents.Reports.Graphics.Image GetImageFromChart(UltraChart chart)
        {
            MemoryStream imageStream = new MemoryStream();
            chart.SaveTo(imageStream, ImageFormat.Png);
            Infragistics.Documents.Reports.Graphics.Image img = new Infragistics.Documents.Reports.Graphics.Image(imageStream);
            img.Preferences.Compressor = ImageCompressors.Flate;
            return img;
        }

        #endregion
    }
}