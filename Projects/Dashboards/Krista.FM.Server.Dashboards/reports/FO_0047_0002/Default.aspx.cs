using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0047_0002
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable gridDt = new DataTable();
        private int firstYear = 2010;
        private DateTime currentDate;
        private int currentQuarterIndex;
        private static MemberAttributesDigest debtKindDigest;
        private static MemberAttributesDigest kosguDigest;

        #endregion

        public bool IsGrbsSelected
        {
            get { return IndicatorButtonList.SelectedIndex == 0; }
        }

        public bool IsArrearsDebtSelected
        {
            get { return ArrearsCheckBox.Checked; }
        }

        #region ��������� �������

        // ��������� ����� �����
        private CustomParam gridRowSet;
        // ��� �������������
        private CustomParam debtKind;
        // ���������� ������������� (�����)
        private CustomParam debtIndicator;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ��������� �����

            GridBrick.Height = CustomReportConst.minScreenHeight - 235;
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);

            #endregion

            #region ������������� ���������� �������

            gridRowSet = UserParams.CustomParam("grid_row_set");
            debtKind = UserParams.CustomParam("debt_kind");
            debtIndicator = UserParams.CustomParam("debt_indicator");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink.Visible = false;
            CrossLink.Text = "���������&nbsp;��������&nbsp;����������� ��&nbsp;��������&nbsp;����������&nbsp;��������";
            CrossLink.NavigateUrl = "~/reports/FO_0002_0032/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DateTime lastDate = CubeInfoHelper.FoDebtKzDz.LastDate;
                string lastQuarter = String.Format("������� {0}", CRHelper.QuarterNumByMonthNum(lastDate.Month));
                
                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboYear.Set�heckedState(lastDate.Year.ToString(), true);

                ComboQuarter.Title = "�������";
                ComboQuarter.Width = 160;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillQuaters(false));
                ComboQuarter.RemoveTreeNodeByName("������� 1");
                ComboQuarter.Set�heckedState(lastQuarter, true);
                
                ComboDebt.Title = "�������������";
                ComboDebt.Width = 400;
                ComboDebt.MultiSelect = false;
                debtKindDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0047_0002_debtKind");
                ComboDebt.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(debtKindDigest.UniqueNames, debtKindDigest.MemberLevels));
                ComboDebt.Set�heckedState("����������� �������������", true);

                ComboKOSGU.Title = "�����";
                ComboKOSGU.Width = 300;
                ComboKOSGU.ParentSelect = true;
                ComboKOSGU.MultiSelect = false;
                kosguDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0047_0002_debtIndicator");
                ComboKOSGU.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(kosguDigest.UniqueNames, kosguDigest.MemberLevels));
                ComboKOSGU.Set�heckedState("������������� ����� ", true);
            }
            
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentQuarterIndex = ComboQuarter.SelectedIndex + 2;
            currentDate = new DateTime(yearNum, 3 * currentQuarterIndex, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));

            string debtIndicatorString = String.Format("{0} {1}",
                                           ComboKOSGU.SelectedValue.TrimEnd(' '),
                                           kosguDigest.GetMemberType(ComboKOSGU.SelectedValue).TrimEnd('0'));

            Page.Title = String.Format("��������� {0}{1} �� ���������� ������� � ������� ����������� �������������� ({2})",
                IsArrearsDebtSelected ? "������������ " : String.Empty,
                ComboDebt.SelectedValue.Contains("�����������")
                                  ? "����������� �������������"
                                  : "������������ �������������",
                debtIndicatorString.TrimEnd(' '));
            Label1.Text = Page.Title;
            Label2.Text = String.Format("���������� {0}, ���.���.", GetQuarterDateText(currentDate));

            gridRowSet.Value = IsGrbsSelected ? "�������������� � �����" : "���� � �����";
            debtKind.Value = String.Format("{0}{1}", IsArrearsDebtSelected ? "������������ " : String.Empty, ComboDebt.SelectedValue);

            debtIndicator.Value = kosguDigest.GetMemberUniqueName(ComboKOSGU.SelectedValue);

            GridDataBind();
        }

        private static string GetQuarterDateText(DateTime dateTime)
        {
            int quarterIndex = CRHelper.QuarterNumByMonthNum(dateTime.Month);

            switch (quarterIndex)
            {
                case 2:
                    {
                        return String.Format("�� ��������� �� 01.07.{0} ����", dateTime.Year);
                    }
                case 3:
                    {
                        return String.Format("�� ��������� �� 01.10.{0} ����", dateTime.Year);
                    }
                case 4:
                    {
                        return String.Format("�� ��������� �� 01.01.{0} ����", dateTime.Year + 1);
                    }
                default:
                    {
                        return String.Empty;
                    }
            }
        }

        #region ����������� �����
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0047_0002_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �����������", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                FontRowLevelRule levelRule = new FontRowLevelRule(gridDt.Columns.Count - 1);
                levelRule.AddFontLevel("0", GridBrick.BoldFont8pt);
                GridBrick.AddIndicatorRule(levelRule);

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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(40);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], IsGrbsSelected ? "000" : "00 00");
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 2; i < columnCount - 1; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("��������� ������");
            headerLayout.AddCell("���");

            AddBudgetDirectionGroup("��������� ��������");
            AddBudgetDirectionGroup("������������ ��������");
            AddBudgetDirectionGroup("����� �� ��������� � ������������ ���������");
 
            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }

        private void AddBudgetDirectionGroup(string groupName)
        {
            GridHeaderCell groupCell = GridBrick.GridHeaderLayout.AddCell(groupName);
            string currentDateStr = GetQuarterDateText(currentDate);
            groupCell.AddCell("������� ����������", String.Format("������������� �� �������� �� ������� ���������� {0}", currentDateStr));
            GridHeaderCell cell = groupCell.AddCell("���������������� ����������");
            cell.AddCell("��������� ����������", String.Format("������������� �� �������� �� ��������� ���������� {0}", currentDateStr));
            cell.AddCell("���������� ����������", String.Format("������������� �� �������� �� ���������� ���������� {0}", currentDateStr));
            cell.AddCell("�������� ����������", String.Format("������������� �� �������� �� �������� ���������� {0}", currentDateStr));
            cell.AddCell("����� �� ���������������� �����������", String.Format("������������� �� �������� �� ���������������� ���������� {0}", currentDateStr));
            groupCell.AddCell("������� ���������", String.Format("������������� �� �������� �� ������� ��������� {0}", currentDateStr));
            groupCell.AddCell("�����������", String.Format("������������� �� �������� �� ����������� {0}", currentDateStr));
            groupCell.AddCell("���.�������������", String.Format("������������� �� �������� �� ����������� ������������� {0}", currentDateStr));
            groupCell.AddCell("������ �������", String.Format("������������� �� �������� �� ������ �������� {0}", currentDateStr));
            groupCell.AddCell("�����", String.Format("������������� �� �������� �� �������� ����� {0}", currentDateStr));
        }

       #endregion
        
        #region ������� � Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("�������");
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);
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
        }

        #endregion
    }
}