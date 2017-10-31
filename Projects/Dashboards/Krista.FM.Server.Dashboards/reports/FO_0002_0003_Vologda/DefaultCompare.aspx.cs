using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0003_Vologda
{
    public partial class DefaultCompare : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "������";

        private DateTime currentDate;
        private GridHeaderLayout headerLayout;
    
        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

       
       private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "���.���." : "���.���."; }
        }

       private string GridRegionsSet
       {
           get
           {
               if (RegionsCheckBox.Checked && DistrictCheckBox.Checked)
               {
                   return "������ � ������";
               }
               else if (RegionsCheckBox.Checked)
               {
                   return "������ � ��������� �����";
               }
               else
               {
                   return "������ � ��������� �����";
               }
           }
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

        // ��������� �� ��� �����
        private CustomParam gridRegionSet;

        private CustomParam kdGroupName; 
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
            gridRegionSet = UserParams.CustomParam("grid_region_set");

            if (kdGroupName == null)
            {
                kdGroupName = UserParams.CustomParam("kd_group_name");
            }

            #endregion

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight /2.0);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "��� ������";
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.EnableViewState = false;

            UltraChart1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2.0);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Text = "�����������&nbsp;��������&nbsp;�����������&nbsp;�������";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0003_Vologda/DefaultCompareChart.aspx";

            #region ��������� ��������
            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.Tooltips.FormatString = String.Format("<ITEM_LABEL> �. \n <DATA_VALUE:N2> {0}", RubMultiplierCaption.ToLower());
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Axis.X.Extent = 160;
            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.X.Labels.SeriesLabels.FormatString = "<SERIES_LABEL>";
            UltraChart1.Axis.Y.Extent = 40;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Legend.Margins.Right = Convert.ToInt32(UltraChart1.Width.Value) / 2;
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.SpanPercentage = 11;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart1.Height.Value) - UltraChart1.Axis.X.Extent;
            UltraChart1.TitleLeft.Text = RubMultiplierCaption;
             
            #endregion
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
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.IncomesKD30000000000000000.Value = RegionSettingsHelper.Instance.IncomesKD30000000000000000;

            if (!Page.IsPostBack)
            {
                RegionsCheckBox.Attributes.Add("onclick", string.Format("uncheck('{0}', true)", DistrictCheckBox.ClientID));
                DistrictCheckBox.Attributes.Add("onclick", string.Format("uncheck('{0}', true)", RegionsCheckBox.ClientID));

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0003_Vologda_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

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

                ComboIncomes.Width = 500;
                ComboIncomes.Title = "��� ������";
                ComboIncomes.MultiSelect = false;
                ComboIncomes.ParentSelect = true;
                ComboIncomes.FillDictionaryValues(KDList());
                ComboIncomes.Set�heckedState("���� ", true);
            }

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue)-1,ComboMonth.SelectedIndex+1,1);

            Page.Title = "���� ����� �������";
            Label1.Text = Page.Title;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            Label2.Text = String.Format("��������� ������ ����� ����������� ������� ������������������ ������� ��������, ������� �������� � ������� �������� �� {0} {1} {2} ����", 
                monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            chartHeaderLabel.Text = string.Format("{0}",ComboIncomes.SelectedValue);

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

            incomesTotalItem.Value = "������ ������� c ����������� ��������� ";
            gratuitousIncomesItem.Value = "������������� ����������� c ����������� ��������� ";

            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";
            gridRegionSet.Value = GridRegionsSet;

            if (ComboIncomes.SelectedValue == "������ ������� - ����� ")
            {
                kdGroupName.Value = "";
            }
            else
            {
                kdGroupName.Value = String.Format("{0}.[{1}]", UserParams.IncomesKDDimension.Value,
                                                  ComboIncomes.SelectedValue);
            }

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            kdGroupName.Value = String.Format("{0}.[{1}]", UserParams.IncomesKDDimension.Value,
                                                  ComboIncomes.SelectedValue);
            UltraChart1.DataBind();
        }

        Dictionary<string, int> KDList()
        {
            Dictionary<string, int> valuesDictionary = new Dictionary<string, int>();
            valuesDictionary.Add("��������� ������ ", 0);
            valuesDictionary.Add("����� �� ������� ", 1);
            valuesDictionary.Add("���� ", 1);
            valuesDictionary.Add("������ �� ��������� ", 1);
            valuesDictionary.Add("����� �� ��������� ���.��� ", 2);
            valuesDictionary.Add("����� �� ��������� ����������� ", 2);
            valuesDictionary.Add("������������ ����� ", 2);
            valuesDictionary.Add("������������ ����� � ����������� ", 3);
            valuesDictionary.Add("������������ ����� � ���. ��� ", 3);
            valuesDictionary.Add("��������� ����� ", 2);
            valuesDictionary.Add("������ ", 1);
            valuesDictionary.Add("���� ", 1);
            valuesDictionary.Add("������ �� ���������� ����� ", 1);
            valuesDictionary.Add("��� ", 2);
            valuesDictionary.Add("���� ", 2);
            valuesDictionary.Add("���� ", 2);
            valuesDictionary.Add("������ ��������� ������", 1);
            valuesDictionary.Add("����������� ������ ", 0);
            valuesDictionary.Add("������ �� ������������� ��������� ", 1);
            valuesDictionary.Add("������� ��� ����������� ���������� ��������� ", 1);
            valuesDictionary.Add("������ �� �������� ������� ����� ", 1);
            valuesDictionary.Add("������ �� ������� ������������ � �������������� ������� ", 1);
            valuesDictionary.Add("���������������� ������� � ����� ", 1);
            valuesDictionary.Add("������, �������, ���������� ������ ", 1);
            valuesDictionary.Add("������ ����������� ������ ", 1);
            valuesDictionary.Add("������ �������� �� �������� �������� ��� ������� ��� ", 1);
            valuesDictionary.Add("������� �������� ��� ������� ��� ", 1);
            valuesDictionary.Add("��������� � ����������� ������ ", 0);
            valuesDictionary.Add("������������� ����������� ", 0);
            valuesDictionary.Add("������ ������� - ����� ", 0);
            return valuesDictionary;
        }
        
        #region ����������� �����
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0003_Vologda_compare_grid");
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

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.FilterOptionsDefault.AllowRowFiltering = RowFiltering.No;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 60;

                int j = (i - 1) % 5;
                switch (j)
                {
                    case 0:
                    case 1:
                        {
                            formatString = "N1";
                            widthColumn = 100;
                            break;
                        }
                    case 2:
                        {
                            formatString = "N2";
                            widthColumn = 80;
                            break;
                        }
                    case 3:
                    case 4:
                        {
                            formatString = "N2";
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

          /*  for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
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

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 5)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0].TrimEnd('_');

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, "���������, ���.���.", "����������� ���������� ����������� ������ � ������ ����");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "��������� ������� ���, ���.���.", "��������� �� ����������� ������ �������� ����");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "���� ����� � �������� ����, %", "���� ����� ���������� � ������������ ������� �������� ����");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "����, %", "���� ������ � ����� ����� ������� ���������� �������");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, "���� � ������� ����, %", "���� ������ � ����� ����� ����������� ������� � ������� ����");

                if (i == 1)
                {
                    e.Layout.Bands[0].Columns[i + 3].Hidden = true;
                    e.Layout.Bands[0].Columns[i + 4].Hidden = true;
                }
                
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 5;
                ch.RowLayoutColumnInfo.SpanX = (i == 1) ? 3 : 5;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
           */
            headerLayout.AddCell("�������");
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i += 5)
            {
                string[] caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell cell0 = headerLayout.AddCell(caption[0]);
                if (caption[0] == "������ ������� - ����� ")
                {
                   cell0.AddCell("���������, ���.���.", "����������� ���������� ����������� ������ � ������ ����");
                   cell0.AddCell("��������� ������� ���, ���.���.", "��������� �� ����������� ������ �������� ����");
                   cell0.AddCell("���� ����� � �������� ����, %", "���� ����� ���������� � ������������ ������� �������� ����");
                }
                else
                {

                    cell0.AddCell("���������, ���.���.", "����������� ���������� ����������� ������ � ������ ����");
                    cell0.AddCell("��������� ������� ���, ���.���.", "��������� �� ����������� ������ �������� ����");
                    cell0.AddCell("���� ����� � �������� ����, %",
                                  "���� ����� ���������� � ������������ ������� �������� ����");
                    cell0.AddCell("����, %", "���� ������ � ����� ����� ������� ���������� �������");
                    cell0.AddCell("���� � ������� ����, %",
                                  "���� ������ � ����� ����� ����������� ������� � ������� ����");
                }
            }
            headerLayout.ApplyHeaderInfo();

        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 1; i < e.Row.Cells.Count; i = i + 1)
            {
                int groupCount = 5;
                int groupIndex = (i - 1) % groupCount;

                bool growRateColumn = (groupIndex == 2);
                bool percentColumn = groupIndex == 3;

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
            }

            if (e.Row.Cells[0].Value != null &&
                 (e.Row.Cells[0].Value.ToString().ToLower().Contains("������") || e.Row.Cells[0].Value.ToString().ToLower().Contains("�������") || e.Row.Cells[0].Value.ToString().Contains("�����")))
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


        #region ����������� ���������

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string queryName = "FO_0002_0003_Vologda_chart1";
            string query = DataProvider.GetQueryText(queryName);
            DataTable dtChart1 = new DataTable();

            
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
            foreach (DataColumn col in dtChart1.Columns)
            {
                col.ColumnName = col.ColumnName.Replace("������������� �����������", "��");
                col.ColumnName = col.ColumnName.Replace("������������� �����", "��");
                col.ColumnName = col.ColumnName.Replace("������������� �����", "��");
                col.ColumnName = col.ColumnName.Replace("�����", "�-�");
            }

            dtChart1.Rows[1][0] = string.Format("{0:dd.MM.yyyy}", currentDate.AddYears(1));
            dtChart1.Rows[0][0] = string.Format("{0:dd.MM.yyyy}", currentDate);

          UltraChart1.Data.SwapRowsAndColumns = true; 
          UltraChart1.DataSource = dtChart1;
        }

        #endregion

        #region ������� � Excel

       
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Page.Title;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");

            ReportExcelExporter1.HeaderCellHeight = 20;
            ReportExcelExporter1.GridColumnWidthScale = 1.5;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart1,sheet2,3);
        }

        #endregion

        #region ������� � Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Page.Title;
            ReportPDFExporter1.PageSubTitle = Label2.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart1, section2);
        }

        #endregion
/*
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

       private void ExcelExporter_EndExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[2].Height = 20 * 37;
        }

        #endregion
 */
    }
}
