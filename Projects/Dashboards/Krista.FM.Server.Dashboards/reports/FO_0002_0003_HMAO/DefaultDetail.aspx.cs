using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core.MemberDigests;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0003_HMAO
{
    public partial class DefaultDetail : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2009;
        private int endYear = 2011;
        private string month = "������";
        private string multiplierCaption;
        
        #region ��������� �������

        private CustomParam selectedRegion;
        // ������ �����
        private CustomParam incomesTotal;
        // ������� ������������������ �������
        private CustomParam regionsConsolidateLevel;
        // ��� ��������� ���� ��� ������������������ ������
        private CustomParam consolidateDocumentSKIFType;
        // ��� ��������� ���� ��� ������ �������
        private CustomParam regionDocumentSKIFType;
        // ������� ������� ���� ��� �������
        private CustomParam regionBudgetSKIFLevel;
        // ������� ��
        private CustomParam regionsLevel;
        // ��������� ��������� ������
        private CustomParam rubMultiplier;

        #endregion

        private MemberAttributesDigest budgetDigest;
        private GridHeaderLayout headerLayout;

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ������������� ���������� �������

            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }
            if (regionsConsolidateLevel == null)
            {
                regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            }
            if (consolidateDocumentSKIFType == null)
            {
                consolidateDocumentSKIFType = UserParams.CustomParam("consolidate_document_skif_type");
            }
            if (regionDocumentSKIFType == null)
            {
                regionDocumentSKIFType = UserParams.CustomParam("region_document_skif_type");
            }
            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }
            
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            regionsLevel = UserParams.CustomParam("regions_level");

            #endregion

            multiplierCaption = IsThsRubSelected ? "���.���." : "���.���.";
            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 220);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);
            UltraWebGrid.DisplayLayout.NoDataMessage = "��� ������";

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            CrossLink1.Text = "����������&nbsp;��&nbsp;����������&nbsp;�����������&nbsp;�������";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0011_01/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.IncomesKDSubSectionLevel.Value = RegionSettingsHelper.Instance.IncomesKDSubSectionLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.KDInternalCircualtionExtruding.Value = RegionSettingsHelper.Instance.KDInternalCircualtionExtruding;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            budgetDigest = MemberDigestHelper.Instance.LocalBudgetDigest;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0003_HMAO_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(UserParams.PeriodMonth.Value, true);
                
                ComboRegion.Title = "������";
                ComboRegion.Width = 400;
                ComboRegion.MultiSelect = false;
                ComboRegion.ParentSelect = true;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetDigest.UniqueNames, budgetDigest.MemberLevels));
                ComboRegion.Set�heckedState("����������������� ������ ��������", true);
            }
            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            Page.Title = string.Format("������ ({0})", ComboRegion.SelectedValue);
            Label1.Text = Page.Title;
            Label2.Text = string.Format("���������� �� {0} {1} {2} ����", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            switch (ComboRegion.SelectedValue)
            {
                case "����������������� ������ ��������":
                    {
                        selectedRegion.Value = "[������].[������������].[����������������� ������ �������� ]";
                        break;
                    }
                case "������������� ������":
                    {
                        selectedRegion.Value = "[������].[������������].[������������� ������ ]";
                        break;
                    }
                case "��������� ������":
                    {
                        selectedRegion.Value = "[������].[������������].[��������� ������ ]";
                        break;
                    }
                default:
                    {
                        selectedRegion.Value = budgetDigest.GetMemberUniqueName(ComboRegion.SelectedValue);
                        break;
                    }
            }

            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodEndYear.Value = (yearNum - 2).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;

            consolidateDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateDocumentSKIFType");
            regionDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0003_HMAO_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "�������", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        private static string GetColumnFormat(string columnName)
        {
            if (columnName.ToLower().Contains("���"))
            {
                return "0";
            }
            else if (columnName.ToLower().Contains("% ����������") || columnName.ToLower().Contains("���� �����"))
            {
                return "P2";
            } 

            return "N2";
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(340);
            e.Layout.Bands[0].Columns[7].Hidden = true;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], GetColumnFormat(columnName));
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(125);
            }

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            headerLayout.AddCell("��");
            headerLayout.AddCell("���");

            GridHeaderCell cell = headerLayout.AddCell(String.Format("�� {0} {1} {2} ����", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum));
            cell.AddCell(String.Format("���������� ������� ����������, {0}", multiplierCaption), "�������� ���������� �� ���");
            cell.AddCell(String.Format("���������, {0}", multiplierCaption), "����������� ���������� ����������� ������ � ������ ����");
            cell.AddCell("��������� %", "������� ���������� ����������/ ������ ������������� ���������� (1/12 �������� ����� � �����)");

            cell = headerLayout.AddCell("��������� � ������� �����");
            cell.AddCell(String.Format("��������� ������� ���, {0}", multiplierCaption), "��������� �� ����������� ������ �������� ����");
            cell.AddCell("���� ����� � �������� ����", "���� ����� ���������� � ������������ ������� �������� ����");

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                string columnName = e.Row.Band.Grid.Columns[i].Header.Caption;

                bool growRate = columnName.ToLower().Contains("���� �����");
                int levelColumnIndex = 7;

                if (growRate && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "���� � �������� ����";
                    }
                    else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "�������� � �������� ����";
                    }
                }

                if (e.Row.Cells[levelColumnIndex] != null && e.Row.Cells[levelColumnIndex].Value.ToString() != string.Empty && i != 1)
                {
                    string level = e.Row.Cells[levelColumnIndex].Value.ToString();
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;
                    switch (level)
                    {
                        case "������":
                            {
                                fontSize = 10;
                                bold = true;
                                italic = false;
                                break;
                            }
                        case "���������":
                            {
                                fontSize = 10;
                                bold = false;
                                italic = true;
                                break;
                            }
                        case "������":
                            {
                                fontSize = 8;
                                bold = false;
                                italic = false;
                                break;
                            }
                    }
                    e.Row.Cells[i].Style.Font.Size = fontSize;
                    e.Row.Cells[i].Style.Font.Bold = bold;
                    e.Row.Cells[i].Style.Font.Italic = italic;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
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

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;

            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            ReportExcelExporter1.Export(headerLayout, 3);
        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout);
        }

        #endregion
    }
}
