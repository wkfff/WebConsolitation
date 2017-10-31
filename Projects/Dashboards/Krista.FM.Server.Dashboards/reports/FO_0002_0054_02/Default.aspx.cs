using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0054_02
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable chartDt = new DataTable();
        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private int firstYear = 2009;
        private string rzPrDefaultValue = "������� ������� � ����� (��� ���������� ��������";

        private MemberAttributesDigest rzprDigest;
        
        #endregion

        #region ��������� �������

        // ��������� ����
        private CustomParam selectedMeasure;
        // ��������� �������� ����
        private CustomParam rzPrComboValue;
        // ��������� ����
        private CustomParam selectedRzPr;

        #endregion

        public bool FactSelected
        {
            get { return MeasureButtonList.SelectedIndex == 0; }
        }

        public bool RfMethodSelected
        {
            get { return ComboMethod.SelectedIndex == 0; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ��������� �����

            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.8 - 235);
            GridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 20);
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.AllowColumnSorting = true;
            
            #endregion

            #region ��������� ��������� ��������

            ChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            ChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.8 - 100);
            ChartBrick.Chart.FillSceneGraph +=new FillSceneGraphEventHandler(Chart_FillSceneGraph);
            ChartBrick.InvalidDataMessage = "������ ��� ������������ ������ �� ������������� ����������� �� ��������� �� ��������� ������";

            ChartBrick.YAxisLabelFormat = "P0";
            ChartBrick.DataFormatString = "N2";
            ChartBrick.Legend.Visible = false;
            ChartBrick.ColorSkinRowWise = false;
            ChartBrick.XAxisExtent = 140;
            ChartBrick.YAxisExtent = 60;
            ChartBrick.ZeroAligned = true;
            ChartBrick.SeriesLabelWrap = true;
            ChartBrick.TooltipFormatString = "<b><SERIES_LABEL></b>\n<ITEM_LABEL>: <b><DATA_VALUE:P2></b>";
            ChartBrick.SwapRowAndColumns = true;

            ChartBrick.Legend.Visible = true;
            ChartBrick.Legend.Location = LegendLocation.Top;
            ChartBrick.Legend.SpanPercentage = 9;
            ChartBrick.Legend.Font = new Font("Verdana", 8);
            ChartBrick.Legend.Margins.Right = Convert.ToInt32(ChartBrick.Width.Value / 2);

            ChartBrick.ColorModel = ChartColorModel.GreenRedColors;

            #endregion

            #region ������������� ���������� �������

            selectedMeasure = UserParams.CustomParam("selected_measure");
            rzPrComboValue = UserParams.CustomParam("rzpr_combo_value");
            selectedRzPr = UserParams.CustomParam("selected_rzpr");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink.Visible = true;
            CrossLink.Text = "�������&nbsp;��&nbsp;����������&nbsp;��"; 
            CrossLink.NavigateUrl = "~/reports/FO_0002_0054_01/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DateTime lastDate = CubeInfoHelper.MonthReportOutcomesInfo.LastDate;

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboYear.Set�heckedState(lastDate.Year.ToString(), true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(lastDate.Month)), true);

                ComboMethod.Title = "��������";
                ComboMethod.Width = 210;
                ComboMethod.MultiSelect = false;
                ComboMethod.AutoPostBack = true;
                Dictionary<string, int> dict = new Dictionary<string, int>();
                dict.Add("������ ��", 0);
                dict.Add("������ ���� � ����", 0);
                ComboMethod.FillDictionaryValues(dict);

                rzPrComboValue.Value = rzPrDefaultValue;
            }

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex + 1, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));

            rzprDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0054_02_rzprDigest");

            if (ComboRzPr.SelectedValue != String.Empty)
            {
                rzPrComboValue.Value = ComboRzPr.SelectedValue;
            }

            ComboRzPr.ClearNodes();
            ComboRzPr.Width = 380;
            ComboRzPr.Title = "����";
            ComboRzPr.MultiSelect = false;
            ComboRzPr.ParentSelect = true;
            ComboRzPr.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(rzprDigest.UniqueNames, rzprDigest.MemberLevels));
            ComboRzPr.SelectLastNode();
            ComboRzPr.Set�heckedState(rzPrComboValue.Value, true);

            Page.Title = String.Format("������������� ������������� ����������� ���� �� ���� ���������-�������� � �������������� �������� � �������� �������");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("{0}, {1}, {4} �� {2} {3} ����", MeasureButtonList.SelectedValue, ComboRzPr.SelectedValue, CRHelper.RusMonth(currentDate.Month), currentDate.Year,
                RfMethodSelected ? "�������� ������� ��" : "�������� ������� ���� � ����");

            ChartCaption.Text = "��������� ������������� �� �� ���� ��������";
            
            selectedRzPr.Value = rzPrComboValue.Value == rzPrDefaultValue ? "[����__������������].[����__������������].DefaultMember" : rzprDigest.GetMemberUniqueName(rzPrComboValue.Value);
            
            if (RfMethodSelected)
            {
                ComboRzPr.Visible = true;
                selectedMeasure.Value = FactSelected ? "����" : "������� ����������";    
            }
            else
            {
                ComboRzPr.Visible = false;
                selectedMeasure.Value = FactSelected ? "��������� �� ������" : "���������� ������� ����������";    
            }            

            GridDataBind();
            ChartDataBind();
        }

        #region ����������� �����

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText(RfMethodSelected ? "FO_0002_0054_02_grid_RF" : "FO_0002_0054_02_grid_HMAO");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �����������", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                GrowRateRule growRateRule = new GrowRateRule("���� �����");
                GridBrick.AddIndicatorRule(growRateRule);

                RankIndicatorRule percentFORankRule = new RankIndicatorRule("���� �� �� ���� ��������",
                    "������ ���� �� �� ���� ��������");
                percentFORankRule.BestRankHint = "����� ������� ���� ���������-�������� � �������������� �������� � �������� �������";
                percentFORankRule.WorseRankHint = "����� ������ ���� ���������-�������� � �������������� �������� � �������� �������";
                GridBrick.AddIndicatorRule(percentFORankRule);

                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(120);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            
            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(GetColumnWidth(columnName.ToLower()));
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], GetColumnFormat(columnName.ToLower()));
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            headerLayout.AddCell("������������� �����������");

            if (RfMethodSelected)
            {
                GridHeaderCell groupCell = headerLayout.AddCell("���������-�������� �������");
                groupCell.AddCell("������ ����� � ������������ �� ������� �� ������ ����� (����� 211, 213)");
                groupCell.AddCell("������ ������������ ����� (����� 223)");
                groupCell.AddCell("������� �� ���������� ����������� (����� 260)");
                groupCell.AddCell("����� ���������-�������� �������");

                groupCell = headerLayout.AddCell("�������������� �������");
                groupCell.AddCell("������� �� ������������ ���������������� (��������������) ����� (����� 230)");
                groupCell.AddCell("������� �� �������������� ����� (����� 212, ����� 221, ����� 222, ����� 224, ����� 340)");
                groupCell.AddCell("������� �� ������ ����� (����� 225, ����� 226, ����� 241, ����� 290)");
                groupCell.AddCell("����� �������������� ��������");
            }
            else
            {
                GridHeaderCell groupCell = headerLayout.AddCell("���������-�������� �������");
                groupCell.AddCell("������ ����� � ���������� �� ������� �� ������ ����� �� ����������� �������� (����� 211, 213)");
                groupCell.AddCell("���������� ������� �������� �������������� (������� �� ������ ����� � ���������� �� ������� �� ������ ����� (����� 211, 213))");
                groupCell.AddCell("������ ������������ ����� (����� 223)");
                groupCell.AddCell("���������� ����������� (����� 260)");
                groupCell.AddCell("��������� (����� 290)");
                groupCell.AddCell("�������� �������, �����������");
                groupCell.AddCell("����� ���������-�������� �������");

                groupCell = headerLayout.AddCell("�������������� �������");
                groupCell.AddCell("������ ������� (����� 212)");
                groupCell.AddCell("������� �� ������������ ����� (����� 230)");
                groupCell.AddCell("������������ ������� (����� 221, ����� 222, ����� 224)");
                groupCell.AddCell("����� �������������� ��������");
            }

            headerLayout.AddCell("������� �����, ���. ���.");
            headerLayout.AddCell("���� ���������-�������� � �������������� �������� � �������� �������, %");
            headerLayout.AddCell("���� �� �� ���� ��������");
            headerLayout.AddCell("������ ���� ���������-�������� � �������������� �������� � �������� �������, %");
            headerLayout.AddCell("���� ����� ���������-�������� � �������������� �������� � �������� ������� � ������������ ������� �������� ����, %");
            headerLayout.ApplyHeaderInfo();
        }

        private static string GetColumnFormat(string columnName)
        {
            if (columnName.Contains("����"))
            {
                return "N0";
            }
            if (columnName.Contains("����") || columnName.Contains("���� �����") || columnName.Contains("������"))
            {
                return "P2";
            }
            return "N2";
        }

        private static int GetColumnWidth(string columnName)
        {
            if (columnName.Contains("���� �����"))
            {
                return 190;
            } 
            if (columnName.Contains("����"))
            {
                return 80;
            }
            if (columnName.Contains("����"))
            {
                return 130;
            }
            if (columnName.Contains("������"))
            {
                return 120;
            }
            return 130;
        }

        #endregion

        #region ����������� ���������

        protected void ChartDataBind()
        {
            string queryText = DataProvider.GetQueryText(RfMethodSelected ? "FO_0002_0054_02_chart_RF" : "FO_0002_0054_02_chart_HMAO");
            chartDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryText, "����������", chartDt);

            if (chartDt.Rows.Count > 1)
            {
                // ������������� ������� �����, ��-�� ������ ����� ��������� �����������
                double sum = 0;
                foreach (DataRow row in chartDt.Rows)
                {
                    if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                    {
                        sum += Convert.ToDouble(row[1]);
                    }  
                }
                double avgValue = sum / chartDt.Rows.Count;
                
                // ��������� �������
                DataTable avgDT = chartDt.Clone();
                for (int i = 0; i < chartDt.Rows.Count; i++)
                {
                    avgDT.ImportRow(chartDt.Rows[i]);

                    double value;
                    Double.TryParse(chartDt.Rows[i][1].ToString(), out value);
                    if (value >= avgValue && i != chartDt.Rows.Count - 1)
                    {
                        Double.TryParse(chartDt.Rows[i + 1][1].ToString(), out value);
                        if (value < avgValue)
                        {
                            DataRow row = avgDT.NewRow();
                            row[0] = "�������";
                            row[2] = avgValue;
                            avgDT.Rows.Add(row);
                        }
                    }
                }

                // ������������ �������
                int medianIndex = MedianIndex(avgDT.Rows.Count);
                DataTable medianDT = chartDt.Clone();
                for (int i = 0; i < chartDt.Rows.Count; i++)
                {
                    medianDT.ImportRow(chartDt.Rows[i]);
                    if (i == medianIndex)
                    {
                        DataRow row = medianDT.NewRow();
                        row[0] = "�������";
                        row[2] = MedianValue(chartDt, "���� ��������");
                        medianDT.Rows.Add(row);
                    }

                    double value;
                    Double.TryParse(chartDt.Rows[i][1].ToString(), out value);
                    if (value >= avgValue && i != chartDt.Rows.Count - 1)
                    {
                        Double.TryParse(chartDt.Rows[i + 1][1].ToString(), out value);
                        if (value < avgValue)
                        {
                            DataRow row = medianDT.NewRow();
                            row[0] = "�������";
                            row[2] = avgValue;
                            medianDT.Rows.Add(row);
                        }
                    }
                }

                ChartBrick.Chart.Series.Clear();

                NumericSeries series1 = CRHelper.GetNumericSeries(2, medianDT);
                series1.Label = "���� ���������-�������� ��������\n � �������� �������";
                ChartBrick.Chart.Series.Add(series1);

                NumericSeries series2 = CRHelper.GetNumericSeries(3, medianDT);
                series2.Label = "���� �������������� ��������\n� �������� �������";
                ChartBrick.Chart.Series.Add(series2);
            }
        }

        protected void Chart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    text.bounds.Width = 30;
                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                    text.labelStyle.FontSizeBestFit = false;
                    text.labelStyle.Font = new Font("Verdana", 7);
                    text.labelStyle.WrapText = true;

                    if (text.GetTextString() == "�������" || text.GetTextString() == "�������")
                    {
                        LabelStyle boldStyle = text.GetLabelStyle();
                        boldStyle.Font = new Font("Verdana", 8, FontStyle.Bold);
                        boldStyle.FontColor = Color.Black;
                        text.SetLabelStyle(boldStyle);
                    }
                }

                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        if (box.Series != null && (box.Series.Label == "�������" || box.Series.Label == "�������"))
                        {
                            box.PE.Fill = Color.Yellow;
                            box.PE.FillStopColor = Color.Orange;
                            box.DataPoint.Label = "���� ���������-�������� � ��������������\n�������� � �������� �������";
                        }
                    }
                }
            }
        }

        #endregion

        #region ������ �������

        private static bool Even(int input)
        {
            if (input % 2 == 0)
            {
                return true;
            }
            return false;
        }

        private static int MedianIndex(int length)
        {
            if (length == 0)
            {
                return 0;
            }

            if (Even(length))
            {
                return length / 2 - 1;
            }
            else
            {
                return (length + 1) / 2 - 1;
            }
        }

        private static double MedianValue(DataTable dt, string medianValueColumn)
        {
            if (dt.Rows.Count == 0)
            {
                return 0;
            }

            if (Even(dt.Rows.Count))
            {
                double value1;
                double value2;
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count)][medianValueColumn].ToString(),
                        out value1);
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count) + 1][medianValueColumn].ToString(),
                        out value2);
                return (value1 + value2) / 2;
            }
            else
            {
                double value;
                Double.TryParse(
                        dt.Rows[MedianIndex(dt.Rows.Count)][medianValueColumn].ToString(),
                        out value);
                return value;
            }
        }

        #endregion

        #region ������� � Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            GridBrick.Grid.DisplayLayout.SelectedRows.Clear(); 
            
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("�������");
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("���������");
            ReportExcelExporter1.Export(ChartBrick.Chart, ChartCaption.Text, sheet2, 3);
        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);

            ISection section2 = report.AddSection();
            ChartBrick.Chart.Legend.Margins.Right = 0;
            ChartBrick.Chart.Width = Convert.ToInt32(ChartBrick.Chart.Width.Value * 0.8);
            ReportPDFExporter1.Export(ChartBrick.Chart, ChartCaption.Text, section2);
        }

        #endregion
    }
}