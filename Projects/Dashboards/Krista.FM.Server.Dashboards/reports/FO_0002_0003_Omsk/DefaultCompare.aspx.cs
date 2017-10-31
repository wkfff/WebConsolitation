using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0003_Omsk
{
    public partial class DefaultCompare : CustomReportPage
    {
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "������";
        private bool internalCirculatoinExtrude = false;
    
        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private bool UseClimaticZones
        {
            get { return useClimaticZones.Checked; }
        }

        private bool UseComparableStandard
        {
            get { return comparableStandandCheckBox.Checked; }
        }

        #region ��������� �������

        // ������ �����
        private CustomParam incomesTotal;
        // ������� �� � ��
        private CustomParam regionsLevel;

        // ��� ��������� ���� ��� ������������������ ������� ��������
        private CustomParam consolidateBudgetDocumentSKIFType;
        // ��� ��������� ���� ��� �������
        private CustomParam regionBudgetDocumentSKIFType;
        // ������� ������� ���� ��� �������
        private CustomParam regionBudgetSKIFLevel;

        // ����������������� ������ ��������
        private CustomParam regionsConsolidateBudget;

        // ������� ������ �����
        private CustomParam incomesTotalItem;
        // ������� ������������� �����������
        private CustomParam gratuitousIncomesItem;
        
        // ��������� ��������� ������
        private CustomParam rubMultiplier;

        // ��������� �������
        private CustomParam regionSet;

        // ������������ ���������� � ������������ ����������
        private CustomParam useComparableStandard;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ������������� ���������� �������

            incomesTotal = UserParams.CustomParam("incomes_total");
            regionsLevel = UserParams.CustomParam("regions_level");
            consolidateBudgetDocumentSKIFType = UserParams.CustomParam("consolidate_budget_document_skif_type");
            regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            regionsConsolidateBudget = UserParams.CustomParam("regions_consolidate_budget");
            incomesTotalItem = UserParams.CustomParam("incomes_total_item");
            gratuitousIncomesItem = UserParams.CustomParam("gratuitous_incomes_item");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            regionSet = UserParams.CustomParam("region_set");
            useComparableStandard = UserParams.CustomParam("use_comparable_standard");

            #endregion

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 240);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "��� ������";
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout_withRanking);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport_withRanking);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.MultiHeader = true;

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);

            CrossLink1.Text = "�����������&nbsp;��������&nbsp;�����������&nbsp;�������";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0003_Omsk/DefaultCompareChart.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.RegionsLocalBudgetLevel.Value = RegionSettingsHelper.Instance.RegionsLocalBudgetLevel;
           
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            internalCirculatoinExtrude = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("InternalCirculationExtrude"));
           
            UserParams.IncomesKD30000000000000000.Value = CubeInfoHelper.MonthReportIncomesInfo.GetDimensionElement("��.������������", "30000000000000000");
            UserParams.IncomesKD11800000000000000.Value = CubeInfoHelper.MonthReportIncomesInfo.GetDimensionElement("��.������������", "11800000000000000");

            if (!Page.IsPostBack)
            {
                DateTime lastDate = CubeInfoHelper.MonthReportIncomesInfo.LastDate;
                endYear = lastDate.Year;
                month = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(lastDate.Month));

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(endYear.ToString(), true);

                ComboMonth.Title = "�����";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.Set�heckedState(month, true);
            }

            Page.Title = "���� ����� �������";
            Label1.Text = Page.Title;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            Label2.Text = String.Format("��������� ������ ����� ����������� ������� ������������������ ������� ��������, ������� �������� � ������� �������� �� {0} {1} {2} ����", 
                monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;

            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            
            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");
                       
            regionsConsolidateBudget.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;

            incomesTotalItem.Value = internalCirculatoinExtrude
                  ? "������ ������� ��� ���������� �������� "
                  : "������ ������� c ����������� ��������� ";
            gratuitousIncomesItem.Value = internalCirculatoinExtrude
                  ? "������������� ����������� ��� ���������� �������� "
                  : "������������� ����������� c ����������� ��������� ";

            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";

            regionSet.Value = UseClimaticZones ? "������������� ����" : "������ ��";
            useComparableStandard.Value = UseComparableStandard ? "true" : "false";

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0003_Omsk_compare_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "�������", dtGrid);
            
            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (UltraWebGrid != null && UltraWebGrid.Rows.Count < 30)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        protected void UltraWebGrid_InitializeLayout_withRanking(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 60;

                int j = (i - 1) % 7;
                switch (j)
                {
                    case 0:
                    case 1:
                        {
                            formatString = "N1";
                            widthColumn = 90;
                            break;
                        }
                    case 2:
                        {
                            formatString = "N2";
                            widthColumn = 90;
                            break;
                        }
                    case 4:
                    case 5:
                        {
                            formatString = "N2";
                            widthColumn = 75;
                            break;
                        }
                    case 3:
                        {
                            formatString = "N0";
                            widthColumn = 75;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 250;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;
            string rubMultiplierName = RubMiltiplierButtonList.SelectedValue;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 7)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0].TrimEnd('_');

                bool isComparableStandardColumn = UseComparableStandard &&
                                (ch.Caption.Contains("����") || ch.Caption.Contains("��������� � ����������� ������"));

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, String.Format("���������, {0}", rubMultiplierName), "����������� ���������� ����������� ������ � ������ ����");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1,
                    !isComparableStandardColumn ? String.Format("��������� ������� ���, {0}", rubMultiplierName) : String.Format("��������� ������� ��� � ������. ���������, {0}", rubMultiplierName),
                    !isComparableStandardColumn ? "��������� �� ����������� ������ �������� ����" : "����������� ���������� � ������� ���� � ������ ��������� ��������������� ��������� �� ���� �� ������� ��� ��� ������������� �������");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2,
                    !isComparableStandardColumn ? "���� ����� � �������� ����, %" : "���� ����� � �������� ���� � ������. ���������,%",
                    !isComparableStandardColumn ? "���� ����� ���������� � ������������ ������� �������� ����" : "���� ����� � �������� ���� � ������ ��������� ��������������� ��������� �� ���� �� ������� ��� ��� ������������� �������");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3,
                    !isComparableStandardColumn ? "���� �� ����� �����" : "����� ��/������������� ���� �� ����� ����� � ������. ���������", "����� ��/������������� ���� �� ����� �����");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, "����, %", "���� ������ � ����� ����� ������� ���������� �������");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 5, "���� � ������� ����, %", "���� ������ � ����� ����� ����������� ������� � ������� ����");

                if (i == 1)
                {
                    e.Layout.Bands[0].Columns[i + 4].Hidden = true;
                    e.Layout.Bands[0].Columns[i + 5].Hidden = true;
                }
                e.Layout.Bands[0].Columns[i + 6].Hidden = true;

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 7;
                ch.RowLayoutColumnInfo.SpanX = (i == 1) ? 4 : 6;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 1; i < e.Row.Cells.Count; i = i + 1)
            {
                int groupCount = 7;
                int groupIndex = (i - 1) % groupCount;

                bool growRateColumn = (groupIndex == 2);
                bool percentColumn = groupIndex == 4;
                bool rankColumn = groupIndex == 3;

                if (growRateColumn && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "�������� � �������� ����";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "���� � �������� ����";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px"; 
                }

                if (percentColumn && 
                    e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty &&
                    e.Row.Cells[i + 1].Value != null && e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                {
                    double currValue = Convert.ToDouble(e.Row.Cells[i].Value);
                    double prevValue = Convert.ToDouble(e.Row.Cells[i + 1].Value);

                    if (currValue < prevValue)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "���� ����� � �������� ����";
                    }
                    else if (currValue > prevValue)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "���� ������� � �������� ����";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (rankColumn)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 3].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 3].Value.ToString() != string.Empty)
                    {
                        double rank = Convert.ToInt32(e.Row.Cells[i].Value);
                        double badRank = Convert.ToInt32(e.Row.Cells[i + 3].Value);

                        if (rank == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = "������������ ���� �����";
                        }
                        else if (rank == badRank)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = "����������� ���� �����";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }

            if (e.Row.Cells[0].Value != null &&
                 (e.Row.Cells[0].Value.ToString().ToLower().Contains("������") || e.Row.Cells[0].Value.ToString().ToLower().Contains("�������") ||
                 e.Row.Cells[0].Value.ToString().ToLower().Contains(", �����") || e.Row.Cells[0].Value.ToString().ToLower().Contains(" ����")))
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
            }

            foreach (UltraGridCell cell in e.Row.Cells)
            {
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

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;

            e.CurrentWorksheet.Columns[0].Width = 300 * 37;
            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
            e.CurrentWorksheet.Columns[1].Width = 90 * 37;
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.0;[Red]-#,##000";
            e.CurrentWorksheet.Columns[2].Width = 90 * 37;
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[3].Width = 90 * 37;

            for (int i = 4; i < columnCount; i = i + 1)
            {
                string formatString = UltraGridExporter.ExelNumericFormat;
                int columnWidth = 70;

                int j = (i - 4) % 5;
                switch (j)
                {
                    case 0:
                    case 1:
                        {
                            formatString = "#,##0.0;[Red]-#,##0.0";
                            columnWidth = 110;
                            break;
                        }
                    case 2:
                        {
                            formatString = "#,##0.00;[Red]-#,##0.00";
                            columnWidth = 100;
                            break;
                        }
                    case 3:
                    case 4:
                        {
                            formatString = "#,##0.00;[Red]-#,##0.00";
                            columnWidth = 85;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = columnWidth * 37;
            }

            // ����������� ����� � ����� ������
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[4].Height = 17 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }
        }

        private void ExcelExporter_EndExport_withRanking(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;

            e.CurrentWorksheet.Columns[0].Width = 300 * 37;
            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
            e.CurrentWorksheet.Columns[1].Width = 90 * 37;
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
            e.CurrentWorksheet.Columns[2].Width = 90 * 37;
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[3].Width = 90 * 37;
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0;[Red]-#,##0";
            e.CurrentWorksheet.Columns[4].Width = 90 * 37;

            for (int i = 5; i < columnCount; i = i + 1)
            {
                string formatString = UltraGridExporter.ExelNumericFormat;
                int columnWidth = 70;

                int j = (i - 5) % 6;
                switch (j)
                {
                    case 0:
                    case 1:
                        {
                            formatString = "#,##0.0;[Red]-#,##0.0";
                            columnWidth = 110;
                            break;
                        }
                    case 2:
                        {
                            formatString = "#,##0.00;[Red]-#,##0.00";
                            columnWidth = 100;
                            break;
                        }
                    case 4:
                    case 5:
                        {
                            formatString = "#,##0.00;[Red]-#,##0.00";
                            columnWidth = 85;
                            break;
                        }
                    case 3:
                        {
                            formatString = "#,##0;[Red]-#,##0";
                            columnWidth = 85;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = columnWidth * 37;
            }

            // ����������� ����� � ����� ������
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[4].Height = 17 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }
        }

        private int hiddenOffset;
        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + hiddenOffset];
            while (col.Hidden)
            {
                hiddenOffset++;
                col = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + hiddenOffset];
            }
            string headerText = col.Header.Key.Split(';')[0];
            e.HeaderText = headerText;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            hiddenOffset = 0;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
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
        }

        #endregion
    }
}
