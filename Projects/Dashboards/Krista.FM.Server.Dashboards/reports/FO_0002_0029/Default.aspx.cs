using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Orientation = Infragistics.Documents.Excel.Orientation;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0029
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable monthReportDT;
        private DataTable yearReportDT;
        private DateTime currentDate;
        private int rubMultiplier;

        private GridHeaderLayout headerLayout;
        private static MemberAttributesDigest periodDigest;

        #endregion

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 240);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid.DisplayLayout.NoDataMessage = "��� ������";
            UltraWebGrid.EnableViewState = false;

            CrossLink.Text = "�������&nbsp;��������&nbsp;�� ";
            CrossLink.NavigateUrl = "~/reports/FO_0002_0028/Default.aspx";

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport +=new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.IncomesKD30000000000000000.Value = RegionSettingsHelper.Instance.IncomesKD30000000000000000; 

            if (!Page.IsPostBack)
            {
                ComboYear.Title = "������";
                ComboYear.Width = 150;
                ComboYear.ParentSelect = true;
                ComboYear.MultiSelect = false;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0029_date");
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                ComboYear.SelectLastNode();
            }

            string periodUniqueName = periodDigest.GetMemberUniqueName(ComboYear.SelectedValue);
            currentDate = CRHelper.PeriodDayFoDate(periodUniqueName);
            DateTime nextMonthDate = currentDate.AddMonths(1);

            Page.Title = "���������� ���������� ������������������ ������� �������� ��";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("�� {0} - {1} ����, �� ��������� �� {2:dd.MM.yyyy} ����, {3}", currentDate.Year - 3, currentDate.Year, nextMonthDate, RubMiltiplierButtonList.SelectedValue);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);

            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0029_grid_monthReport");
            monthReportDT = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �����������", monthReportDT);

            if (monthReportDT.Columns.Count > 1)
            {
                monthReportDT.PrimaryKey = new DataColumn[] { monthReportDT.Columns[0] };

                query = DataProvider.GetQueryText("FO_0002_0029_grid_yearReport");
                yearReportDT = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �����������", yearReportDT);

                if (yearReportDT.Columns.Count > 1)
                {

                    for (int i = 1; i < monthReportDT.Columns.Count; i++)
                    {
                        DataColumn newColumn = new DataColumn(monthReportDT.Columns[i].ColumnName, monthReportDT.Columns[i].DataType);
                        yearReportDT.Columns.Add(newColumn);
                    }
                    
                    foreach (DataRow yearRow in yearReportDT.Rows)
                    {
                        if (yearRow[0] != DBNull.Value)
                        {
                            string rowName = yearRow[0].ToString();
                            DataRow monthRow = monthReportDT.Rows.Find(rowName);
                            if (monthRow != null)
                            {
                                foreach (DataColumn column in monthReportDT.Columns)
                                {
                                    yearRow[column.ColumnName] = monthRow[column.ColumnName];
                                }
                            }
                        }
                    }
                }

                string monthName = CRHelper.RusMonth(currentDate.Month);
                string lastYearFactColumnName = String.Format("{0}; ��������� �� ���", currentDate.Year - 1);
                string currYearPlanColumnName = String.Format("{0}; ��������� �� ���", monthName);
                string currYearRateColumnName = String.Format("{0}; ���� �����", monthName);

                foreach (DataRow row in yearReportDT.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length - 1; i++)
                    {
                        if (row[i] != DBNull.Value && row[i].ToString() != String.Empty)
                        {
                            double value = Convert.ToDouble(row[i].ToString());
                            if (IsMonetaryColumn(yearReportDT.Columns[i].ColumnName))
                            {
                                row[i] = value / rubMultiplier;
                            }
                            else
                            {
                                row[i] = value * 100;
                            }

                        }

                        // � ����� ������ ������������� ���� ����� �������
                        if (row[lastYearFactColumnName] != DBNull.Value && row[lastYearFactColumnName].ToString() != String.Empty &&
                            row[currYearPlanColumnName] != DBNull.Value && row[currYearPlanColumnName].ToString() != String.Empty)
                        {
                            double lastYearValue = Convert.ToDouble(row[lastYearFactColumnName]);
                            double currYearValue = Convert.ToDouble(row[currYearPlanColumnName]);

                            if (row[currYearRateColumnName] != DBNull.Value && lastYearValue != 0)
                            {
                                row[currYearRateColumnName] = 100 * currYearValue / lastYearValue;
                            } 
                        }
                        else
                        {
                            row[currYearRateColumnName] = DBNull.Value;
                        }
                    }
                }

                UltraWebGrid.DataSource = yearReportDT;
            }
        }

        private static bool IsMonetaryColumn(string columnName)
        {
            return columnName.Contains("���������") || columnName.Contains("���������") || columnName.Contains("�������� ��� �������")
                || columnName.Contains("�������� ��� �� � ��") || columnName.Contains("�������� ��� ��");
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N1";
                int widthColumn = 90;
                
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(230);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;

            headerLayout.AddCell("������������ �����������");

            for (int year = currentDate.Year - 3; year < currentDate.Year; year++)
            {
                GridHeaderCell yearCell = headerLayout.AddCell(String.Format("{0} ���", year));
                yearCell.AddCell(String.Format("��������� �� ���, {0}", RubMiltiplierButtonList.SelectedValue));
                if (year != currentDate.Year - 3)
                {
                    yearCell.AddCell(String.Format("���� ����� � {0} ����, %", year - 1));
                }

                yearCell.AddCell("�������� ��� � ����� ������, %");
                GridHeaderCell includingCell = yearCell.AddCell("� ��� �����");
                includingCell.AddCell("�������");
                includingCell.AddCell("��, ���., ������. ���.");
                includingCell.AddCell("�. ����");
            }

            GridHeaderCell currYearCell = headerLayout.AddCell(String.Format("{0} ���", currentDate.Year));
            currYearCell.AddCell(String.Format("���� �� ���, {0}", RubMiltiplierButtonList.SelectedValue));
            currYearCell.AddCell(String.Format("���� ����� � {0} ����", currentDate.Year - 1));
            currYearCell.AddCell("�������� ��� � ����� ������, %");
            GridHeaderCell planIncludingCell = currYearCell.AddCell("� ��� �����");
            planIncludingCell.AddCell("�������");
            planIncludingCell.AddCell("��, ���., ������. ���.");
            planIncludingCell.AddCell("�. ����");

            currYearCell.AddCell(String.Format("���������, {0}", RubMiltiplierButtonList.SelectedValue));
            currYearCell.AddCell("% ����������");
            currYearCell.AddCell("�������� ��� � ����� ������, %");
            GridHeaderCell factIncludingCell = currYearCell.AddCell("� ��� �����");
            factIncludingCell.AddCell("�������");
            factIncludingCell.AddCell("��, ���., ������. ���.");
            factIncludingCell.AddCell("�. ����");

            headerLayout.ApplyHeaderInfo();
        }
        
        private static bool IsInvertIndication(string indicatorName)
        {
            switch (indicatorName)
            {
                case "����� ��������":
                case "������ �����":
                case "������������ ������":
                case "�������� ����� �� ����������� ����������":
                case "������, ������ �� ���������� ���������":
                case "������ ������ � ������":
                case "������� �� ������������ ���������������� �����":
                case "������������� ������������ ��������������� � ������������� ������������":
                case "������������� ������������ ������������, �� ����������� ��������������� � ������������� �����������":
                case "������������ ������ �������� ��������� ������� ���������� ���������":
                case "������ �������":
                case "������������������� �������":
                case "��������� �������������� �������� �������":
                case "��������� ���������� � �������� ��������� �����������":
                case "���������� �������":
                case "��������� �������, ���������� �� ������ �������� ��������� ������� ��":
                case "������ ��������� ����������� ��������������":
                case "��������� �������, ��������������� ������ ������":
                case "�������������� ��������� ��������":
                case "���������� ��������":
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string indicatorName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                indicatorName = e.Row.Cells[0].Value.ToString().Replace("_", String.Empty);
                e.Row.Cells[0].Value = indicatorName;
            }

            bool isInvertIndication = IsInvertIndication(indicatorName.TrimEnd(' '));

            string level = String.Empty;
            int levelColumnIndex = e.Row.Cells.Count - 1;
            if (e.Row.Cells[levelColumnIndex] != null)
            {
                level = e.Row.Cells[levelColumnIndex].ToString();
            }

            e.Row.Style.Padding.Right = 5;

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                string columnCaption = e.Row.Band.Columns[i].Header.Caption.ToLower();

                bool rate = columnCaption.Contains("���� �����");

                switch (level)
                {
                    case "0":
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                            e.Row.Cells[i].Style.Font.Size = 9;
                            break;
                        }
                    case "1":
                        {
                            e.Row.Cells[i].Style.Font.Bold = false;
                            e.Row.Cells[i].Style.Font.Italic = true;
                            e.Row.Cells[i].Style.Font.Size = 8;
                            break;
                        }
                    case "2":
                        {
                            e.Row.Cells[i].Style.Font.Bold = false;
                            e.Row.Cells[i].Style.Font.Italic = false;
                            e.Row.Cells[i].Style.Font.Size = 8;
                            break;
                        }
                }

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double currentValue = Convert.ToDouble(e.Row.Cells[i].Value);

                        if (currentValue > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = isInvertIndication ? "~/images/arrowRedUpBB.png" : "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "����������� ���� ���������� ������������ ����������� ����";
                        }
                        else if (currentValue < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = isInvertIndication ? "~/images/arrowGreenDownBB.png" : "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "����������� �������� ���������� ������������ ����������� ����";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                UltraGridCell cell = e.Row.Cells[i];
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

        #region ������� � Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 40;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            ReportExcelExporter1.HeaderCellHeight = 20;
            ReportExcelExporter1.Export(headerLayout, 3);
        }

        #endregion

        #region ������� � Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            ReportPDFExporter1.HeaderCellHeight = 30;
            ReportPDFExporter1.Export(headerLayout);
        }

        #endregion
    }
}