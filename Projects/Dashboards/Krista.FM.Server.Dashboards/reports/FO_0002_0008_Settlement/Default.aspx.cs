using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core.MemberDigests;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0008_Settlement
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart;
        private int firstYear = 2000;
        private int endYear = 2011;

        #endregion

        #region ��������� �������

        // ��������� ������
        private CustomParam selectedRegion;
        // ��� ���������
        private CustomParam documentType;
        // ����������������� �������
        private CustomParam consolidateLevel;
        // ������� ������� ����
        private CustomParam budgetSKIFLevel;
        // ������� ��� �����
        private CustomParam outcomesFKRTotal;
        // ��� ����� ���� �� �������� ������� ���������
        private CustomParam isNonCompareYear;
        // ������ ��� ����������� ��������� ����
        private CustomParam rzprExtrudeFilter;

        private CustomParam rubMultiplier;

        #endregion

        private GridHeaderLayout headerLayout;
        private DateTime currentDate;

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "���.���." : "���.���."; }
        }

        private bool isFKR
        {
            get { return OutcomesButtonList.SelectedIndex == 0; }
        }

        private bool IsNonCompareYear
        {
            get { return currentDate.Year == 2011 && isFKR; }
        }

        private MemberAttributesDigest budgetDigest;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.37 - 140);

            UltraChart.Width = CRHelper.GetChartWidth(2 * CustomReportConst.minScreenWidth / 3 - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.59 - 120);

            UltraChart2.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 3 - 25);
            UltraChart2.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.59 - 120);

            #region ������������� ���������� �������

            selectedRegion = UserParams.CustomParam("selected_region");
            documentType = UserParams.CustomParam("document_type");
            consolidateLevel = UserParams.CustomParam("consolidate_level");
            budgetSKIFLevel = UserParams.CustomParam("budget_SKIF_level");
            outcomesFKRTotal = UserParams.CustomParam("outcomes_fkr_total");
            isNonCompareYear = UserParams.CustomParam("is_non_compare_year");
            rzprExtrudeFilter = UserParams.CustomParam("rzpr_extrude_filter");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");

            #endregion

            #region ��������� ���������

            UltraChart.ChartType = ChartType.PieChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.PieChart.ColumnIndex = 1;
            UltraChart.PieChart.OthersCategoryPercent = 0;
            UltraChart.Tooltips.FormatString = String.Format("<ITEM_LABEL>\n���� <DATA_VALUE:N2> {0}\n���� <PERCENT_VALUE:N2>%", RubMultiplierCaption.ToLower());
            UltraChart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Left;
            UltraChart.Legend.SpanPercentage = 32;
            UltraChart.Legend.Margins.Top = 0;

            CRHelper.FillCustomColorModel(UltraChart, 17, false);
            UltraChart.ColorModel.Skin.ApplyRowWise = true;

            UltraChart.TitleTop.Visible = true;
            UltraChart.TitleTop.Text = "����";
            UltraChart.TitleTop.Margins.Top = 2;
            UltraChart.TitleTop.Margins.Bottom = 5;
            UltraChart.TitleTop.Margins.Left = Convert.ToInt32(UltraChart.Legend.SpanPercentage * UltraChart.Width.Value / 100) + 5;
            UltraChart.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleTop.Font = new Font("Verdana", 10, FontStyle.Bold);
            UltraChart.TitleTop.Extent = 20;

            UltraChart2.TitleTop.Visible = true;
            UltraChart2.TitleTop.Text = "����";
            UltraChart2.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleTop.Font = new Font("Verdana", 10, FontStyle.Bold);
            UltraChart2.TitleTop.Extent = 20;

            UltraChart2.ChartType = ChartType.PieChart;
            UltraChart2.Border.Thickness = 0;
            UltraChart2.PieChart.ColumnIndex = 2;
            UltraChart2.PieChart.OthersCategoryPercent = 0;
            UltraChart2.Tooltips.FormatString = String.Format("<ITEM_LABEL>\n���� <DATA_VALUE:N2> {0}\n���� <PERCENT_VALUE:N2>%", RubMultiplierCaption.ToLower());
            CRHelper.CopyCustomColorModel(UltraChart, UltraChart2);
            UltraChart2.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            //firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));
            firstYear = 2008;
            rzprExtrudeFilter.Value = RegionSettingsHelper.Instance.GetPropertyValue("RzPrExtrudeFilter");

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.FKRAllLevel.Value = RegionSettingsHelper.Instance.FKRAllLevel;
            UserParams.FKRSectionLevel.Value = RegionSettingsHelper.Instance.FKRSectionLevel;
            UserParams.FKRSubSectionLevel.Value = RegionSettingsHelper.Instance.FKRSubSectionLevel;
            UserParams.EKRDimension.Value = RegionSettingsHelper.Instance.EKRDimension;
            UserParams.FOFKRCulture.Value = RegionSettingsHelper.Instance.FOFKRCulture;
            UserParams.FOFKRHelthCare.Value = RegionSettingsHelper.Instance.FOFKRHelthCare;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.RzPrInternalCircualtionExtruding.Value = RegionSettingsHelper.Instance.RzPrInternalCircualtionExtruding;

            budgetDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0008_Settlement_Digest");

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0008_Settlement_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(endYear.ToString(), true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(dtDate.Rows[0][3].ToString(), true);

                ComboBudgetLevel.Title = "������ ���������";
                ComboBudgetLevel.Width = 400;
                ComboBudgetLevel.MultiSelect = false;
                ComboBudgetLevel.ParentSelect = true;
                ComboBudgetLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetDigest.UniqueNames, budgetDigest.MemberLevels));
                ComboBudgetLevel.Set�heckedState("����������������� ������ ��������", true);
            }

            selectedRegion.Value = budgetDigest.GetMemberUniqueName(ComboBudgetLevel.SelectedValue);
            outcomesFKRTotal.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;
            budgetSKIFLevel.Value = "[������ �������__����].[������ �������__����].[���].[������ ���������]";
            documentType.Value = "[������������__����].[������������__����].[���].[����������� ����� ���������]";
            CRHelper.SaveToErrorLog(budgetDigest.GetMemberLevel(ComboBudgetLevel.SelectedValue));
            string settlement = string.Empty;
            if (ComboBudgetLevel.SelectedNode.Level == 0)
            {
                settlement = string.Format("{0}, ��� ���������", ComboBudgetLevel.SelectedValue);
            }
            else
            {
                settlement = string.Format("{0}, {1}", ComboBudgetLevel.SelectedNodeParent, ComboBudgetLevel.SelectedValue);
            }
            Page.Title = String.Format("��������� �������� {1}: {0}", settlement, isFKR ? "�� �������� ��������� �������������" : "� ������� �����");
            PageTitle.Text = Page.Title;
            chartHeaderLabel.Text = "��������� �������� � ����������� ��������� ��������";

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, monthNum, 1);
            isNonCompareYear.Value = IsNonCompareYear.ToString();

            PageSubTitle.Text = String.Format("������ �������� � ����������� ��������� �������� �� {0} {1} {2} ����",
                currentDate.Month, CRHelper.RusManyMonthGenitive(currentDate.Month), currentDate.Year);
            CommentTextLabel.Text = IsNonCompareYear
                                        ? "� ����� � ���������� � 01.01.2011 ���� ��������� ������������� � ������������ � �������� ������� �� �190� �� 28.12.2010 ���� ��������� ��������� �������� � ������� ���� �� 2011 ��� � ����������� ������ �����������."
                                        : String.Empty;

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = String.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("������� {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);

            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            UltraChart.DataBind();
            UltraChart2.DataBind();
        }

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = isFKR ? DataProvider.GetQueryText("FO_0002_0008_Settlement_grid_FKR") : DataProvider.GetQueryText("FO_0002_0008_Settlement_grid_KOSGU");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if ((i == 1) && row[i] != DBNull.Value && row[i].ToString().Length > 4)
                    {
                        row[i] = row[i].ToString().Substring(0, 4);
                    }
                }
            }
            decimal Sum = 0, Sum1 = 0, Sum3 = 0;
            for (int i = 0; i <= dtGrid.Rows.Count - 2; i++)
            {
                object val1 = dtGrid.Rows[i][2];
                if (val1 != DBNull.Value)
                {
                    Sum += (decimal)val1;
                }
                val1 = dtGrid.Rows[i][3];
                if (val1 != DBNull.Value)
                {
                    Sum1 += (decimal)val1;
                }
                val1 = dtGrid.Rows[i][6];
                if (val1 != DBNull.Value)
                {
                    Sum3 += (decimal)val1;
                }
            }
            dtGrid.Rows[dtGrid.Rows.Count - 1][2] = Sum;
            dtGrid.Rows[dtGrid.Rows.Count - 1][3] = Sum1;
            dtGrid.Rows[dtGrid.Rows.Count - 1][6] = Sum3;
            if (Sum != 0)
            {
                dtGrid.Rows[dtGrid.Rows.Count - 1][4] = Sum1 / Sum;
            }
            UltraWebGrid.DataSource = dtGrid;
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Hidden = hidden;
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            //            if (!Page.IsPostBack)
            //            {
            UltraGridColumn column = e.Layout.Bands[0].Columns[0];
            e.Layout.Bands[0].Columns.RemoveAt(0);
            e.Layout.Bands[0].Columns.Insert(1, column);
            //            }

            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(330);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            SetColumnParams(e.Layout, 0, 0, isFKR ? "00 00" : "N0", 50, false);

            int columnCount = e.Layout.Bands[0].Columns.Count;
            for (int i = 2; i < columnCount - 2; i = i + 1)
            {
                string formatString = GetColumnFormat(e.Layout.Bands[0].Columns[i].Header.Caption);
                int widthColumn = GetColumnWidth(e.Layout.Bands[0].Columns[i].Header.Caption);

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;

            headerLayout.AddCell("���", isFKR ? "��� ����" : "��� �� �����");
            headerLayout.AddCell("������������", isFKR ? "�������� �������� ������������� ��������" : "�������� ������ �������� �� �����");

            GridHeaderCell groupCell = headerLayout.AddCell(String.Format("�� {0:dd.MM.yyyy} �.", currentDate.AddMonths(1)));

            groupCell.AddCell(String.Format("���������� ������� ����������, {0}", RubMultiplierCaption.ToLower()), String.Format("������� ���������� �������� �� {0} ���", currentDate.Year));
            groupCell.AddCell(String.Format("����, {0}", RubMultiplierCaption.ToLower()), String.Format("��������� {0}", groupCell.Caption));
            groupCell.AddCell("% ����������", "% ���������� ������� ���������� ��������");
            groupCell.AddCell("���� % ���.", "���� (�����) ������ �� % ���������� ������� ���������� ��������");
            groupCell.AddCell("����", "�������� ��� ������ ����������� �������� � ����� ����� ��������");
            groupCell.AddCell("���� ����", "���� (�����) ������ �� ��������� ���� ����������� �������� � ����� ����� ��������");

            if (!IsNonCompareYear)
            {
                GridHeaderCell compareGroupCell = headerLayout.AddCell(String.Format("{0} ��� � {1} ����", currentDate.Year, currentDate.Year - 1));
                compareGroupCell.AddCell(String.Format("����������(+,-), {0}", RubMultiplierCaption.ToLower()), "");
                compareGroupCell.AddCell("���� �����, %", "");
                compareGroupCell.AddCell("���� (��������)", "");
            }

            headerLayout.ApplyHeaderInfo();
        }

        private static string GetColumnFormat(string columnName)
        {
            if (columnName.ToLower().Contains("����") || columnName.ToLower().Contains("����") || columnName.ToLower().Contains("����������"))
            {
                return "N2";
            }
            if (columnName.ToLower().Contains("����"))
            {
                return "N0";
            }
            return "P2";
        }

        private int GetColumnWidth(string columnName)
        {
            double widthMultiplier = IsNonCompareYear ? 1.55 : 1;
            if (columnName.ToLower().Contains("����") || columnName.ToLower().Contains("����") || columnName.ToLower().Contains("����������"))
            {
                return Convert.ToInt32(widthMultiplier * 100);
            }
            if (columnName.ToLower().Contains("����"))
            {
                return Convert.ToInt32(widthMultiplier * 50);
            }
            return Convert.ToInt32(widthMultiplier * 80);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int columnCount = e.Row.Cells.Count;
            for (int i = 0; i < columnCount - 2; i++)
            {
                string columnName = e.Row.Band.Columns[i].Header.Caption.ToLower();

                bool rank = columnName.Contains("����");
                bool rate = columnName.Contains("���� �����");

                if (rank)
                {
                    bool isShareRank = columnName.Contains("����");
                    int worseRankColumnIndex = isShareRank ? columnCount - 1 : columnCount - 2;
                    string obj = isShareRank ? "�������� ���" : "������� ����������";
                    string best = isShareRank ? "������������" : "����� �������";
                    string pour = isShareRank ? "�����������" : "����� ���������";

                    if (e.Row.Cells[i].Value != null && e.Row.Cells[worseRankColumnIndex].Value != null &&
                        e.Row.Cells[i].Value.ToString() != String.Empty &&
                        e.Row.Cells[worseRankColumnIndex].Value.ToString() != String.Empty)
                    {
                        int currentRank = Convert.ToInt32(e.Row.Cells[i].Value);
                        int worseRank = Convert.ToInt32(e.Row.Cells[worseRankColumnIndex].Value);

                        if (currentRank == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = String.Format("{0} {1}", best, obj);
                        }
                        else if (currentRank == worseRank)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = String.Format("{0} {1}", pour, obj);
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != String.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "������� � �������� ����";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "�������� � �������� ����";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (e.Row.Cells[1].Value != null &&
                    (e.Row.Cells[1].Value.ToString() == "������� ������� - ����� " ||
                     e.Row.Cells[1].Value.ToString() == "������� ������� - �����" ||
                     e.Row.Cells[1].Value.ToString().Contains("������� ������� � �����") ||
                     e.Row.Cells[1].Value.ToString() == "������� ����� "))
                {
                    cell.Style.Font.Bold = true;
                }

                if (cell.Value != null && cell.Value.ToString() != String.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        #endregion

        #region ����������� ���������

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = isFKR ? DataProvider.GetQueryText("FO_0002_0008_Settlement_chart_FKR") : DataProvider.GetQueryText("FO_0002_0008_Settlement_chart_KOSGU");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            foreach (DataRow row in dtChart.Rows)
            {
                if (row[0] != DBNull.Value)
                {
                    if (isFKR)
                    {
                        row[0] = GetShortRzPrName(row[0].ToString().ToUpper());
                    }
                    else
                    {
                        row[0] = DataDictionariesHelper.GetShortKOSGUName(row[0].ToString());
                    }
                }
            }

            ((UltraChart)sender).DataSource = dtChart;
        }

        private static string GetShortRzPrName(string fullName)
        {
            string shortName = fullName;

            switch (fullName)
            {
                case "������������������� �������":
                    {
                        return "���������.�������";
                    }
                case "������������ �������":
                    {
                        return "������������ �������";
                    }
                case "������������ ������������ � ������������������ ������������":
                    {
                        return "���.������������ � ������������.����.";
                    }
                case "������������ ���������":
                    {
                        return "������������ ���������";
                    }
                case "�������-������������ ���������":
                    {
                        return "���";
                    }
                case "������ ���������� �����":
                    {
                        return "������ �����.�����";
                    }
                case "�����������":
                    {
                        return "�����������";
                    }
                case "��������, ��������������":
                    {
                        return "�������� � ��������������";
                    }
                case "��������, ��������������, �������� �������� ����������":
                    {
                        return "��������,  ��������������, ���";
                    }
                case "�������� �������� ����������":
                    {
                        return "���";
                    }
                case "���������������":
                    {
                        return "���������������";
                    }
                case "���������������, ���������� �������� � �����":
                    {
                        return "�����., ���.�������� � �����";
                    }
                case "���������� �������� � �����":
                    {
                        return "���������� �������� � �����";
                    }
                case "���������� ��������":
                    {
                        return "���������� ��������";
                    }
                case "������������ ����������":
                    {
                        return "������������ ����������";
                    }
                case "������������ ���������� ������ ��������� �������� ��������� ���������� ��������� � ������������� �����������":
                    {
                        return "��� �������� ���.�� � ��";
                    }
                case "������������ ���������������� � �������������� �����":
                    {
                        return "������.���.� ���.�����";
                    }
            }
            return shortName;
        }

        #endregion

        #region ������� � Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("�������");
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("��������� ����");
            ReportExcelExporter1.Export(UltraChart, chartHeaderLabel.Text, sheet2, 3);

            Worksheet sheet3 = workbook.Worksheets.Add("��������� ����");
            UltraChart2.Width = UltraChart.Width;
            UltraChart2.Legend = UltraChart.Legend;
            ReportExcelExporter1.Export(UltraChart2, chartHeaderLabel.Text, sheet3, 3);
        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(headerLayout, section1);

            ISection section2 = report.AddSection();
            ReportPDFExporter1.Export(UltraChart, chartHeaderLabel.Text, section2);
            ReportPDFExporter1.Export(UltraChart2, section2);
        }

        #endregion
    }
}
