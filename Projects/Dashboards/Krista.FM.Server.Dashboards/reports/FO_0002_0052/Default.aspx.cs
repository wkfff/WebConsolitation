using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Components.ChartBricks;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = System.Drawing.Image;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0052
{
    public enum DynamicChartType
    {
        ResidualDynamic,
        InventoriesDynamic,
        CreditsDynamic
    }

    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable gridDt = new DataTable();
        private DataTable residualChartDt = new DataTable();
        private DataTable inverntoriesChartDt = new DataTable();
        private DataTable creditsChartDt = new DataTable();
        private DataTable legendChartDt = new DataTable();
        private DataTable dynamicChartDt = new DataTable();
        private DateTime currentDate;
        private int firstYear = 2009;
        
        private static MemberAttributesDigest moDigest;
        private static MemberAttributesDigest adminDigest;
        private static MemberAttributesDigest facilityDigest;

        #endregion

        #region ��������� �������

        // ��������� ��������� ��������
        private CustomParam selectedBudgetSet;
        // ���������� ��� �������� ��������
        private CustomParam pieChartIndicator;
        // ���������� ��� ��������� ��������
        private CustomParam dynamicChartIndicator;
        // ��������� ��� �������
        private CustomParam selectedFacility;

        // ����������� ����
        private CustomParam largestExtrudeAdmin;

        // ����������� ��
        private CustomParam largestExtrudeMO;

        #endregion

        private bool AdminSliceSelected
        {
            get { return SliceTypeButtonList.SelectedIndex == 0; }
        }

        public DynamicChartType DynamicType
        {
            get
            {
                if (ResidualMeasure.Checked)
                {
                    return DynamicChartType.ResidualDynamic;
                }
                if (InventoriesMeasure.Checked)
                {
                    return DynamicChartType.InventoriesDynamic;
                }
                if (CreditsMeasure.Checked)
                {
                    return DynamicChartType.CreditsDynamic;
                }
                return DynamicChartType.ResidualDynamic;
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ��������� �����
            
            CommentGridBrick.AutoSizeStyle = GridAutoSizeStyle.Auto;
            CommentGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(GRBSGrid_InitializeLayout);

            #endregion

            #region ��������� ��������� ��������

            DynamicChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.8 - 100);

            DynamicChartBrick.YAxisLabelFormat = "N2";
            DynamicChartBrick.XAxisLabelVisible = false;
            DynamicChartBrick.DataFormatString = "N2";
            DynamicChartBrick.Legend.Visible = true;
            DynamicChartBrick.Legend.Location = LegendLocation.Top;
            DynamicChartBrick.Legend.SpanPercentage = 7;
            DynamicChartBrick.Legend.FormatString = "<ITEM_LABEL>";
            DynamicChartBrick.Legend.Margins.Right = 2 * Convert.ToInt32(DynamicChartBrick.Width.Value) / 3;
            DynamicChartBrick.ColorModel = ChartColorModel.DefaultFixedColors;
            DynamicChartBrick.XAxisExtent = 200;
            DynamicChartBrick.YAxisExtent = 90;
            DynamicChartBrick.ZeroAligned = true;
            DynamicChartBrick.SeriesLabelWrap = true;
            DynamicChartBrick.TooltipFormatString = "<SERIES_LABEL>\n<ITEM_LABEL> �.\n<b><DATA_VALUE:N2></b> ���.���.";
            DynamicChartBrick.Chart.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(Chart_FillSceneGraph);
            DynamicChartBrick.XAxisSeriesLabelWidth = 15;

            #endregion

            #region ��������� �������� ��������

            SetPieChartAppearance(ResidualChartBrick, "���������� ��������� �������� �������");
            SetPieChartAppearance(InventoriesChartBrick, "������������ ������");
            SetPieChartAppearance(CreditsChartBrick, "������������ �������������");

            #endregion

            #region ��������� ���������-�������

            LegendChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            LegendChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.25 - 100);
            LegendChartBrick.SwapRowAndColumns = false;
            LegendChartBrick.ColorModel = ChartColorModel.ExtendedFixedColors;

            #endregion

            #region ������������� ���������� �������

            selectedBudgetSet = UserParams.CustomParam("selected_budget_set");
            dynamicChartIndicator = UserParams.CustomParam("dynamic_chart_indicator");
            pieChartIndicator = UserParams.CustomParam("pie_chart_indicator");
            selectedFacility = UserParams.CustomParam("selected_facility");

            largestExtrudeAdmin = UserParams.CustomParam("largest_extrude_admin");
            largestExtrudeMO = UserParams.CustomParam("largest_extrude_mo");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                moDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0052_moDigest");
                adminDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0052_adminDigest");
                facilityDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0052_facilityDigest");

                ResidualMeasure.Attributes.Add("onclick", String.Format("uncheck('{0}', '{1}')", InventoriesMeasure.ClientID, CreditsMeasure.ClientID));
                InventoriesMeasure.Attributes.Add("onclick", String.Format("uncheck('{0}', '{1}')", ResidualMeasure.ClientID, CreditsMeasure.ClientID));
                CreditsMeasure.Attributes.Add("onclick", String.Format("uncheck('{0}', '{1}')", ResidualMeasure.ClientID, InventoriesMeasure.ClientID));

                chartWebAsyncPanel.AddRefreshTarget(DynamicChartBrick.Chart.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(DynamicChartCaption.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(ResidualMeasure.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(InventoriesMeasure.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(CreditsMeasure.ClientID);

                DateTime lastDate = CubeInfoHelper.FoYearReportBalansInfo.LastDate;
 
                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboYear.Set�heckedState(lastDate.Year.ToString(), true);

                ComboMO.Title = "������";
                ComboMO.Width = 400;
                ComboMO.MultiSelect = true;
                ComboMO.ParentSelect = true;
                ComboMO.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(moDigest.UniqueNames, moDigest.MemberLevels));
                ComboMO.Set�heckedState("��������� �����", true);
                //ComboMO.SetAll�heckedState(true, true);

                ComboAdmin.Title = "������";
                ComboAdmin.Width = 400;
                ComboAdmin.MultiSelect = true;
                ComboAdmin.ParentSelect = true;
                ComboAdmin.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(adminDigest.UniqueNames, adminDigest.MemberLevels));
                ComboAdmin.Set�heckedState("��������������� �������� ������������� �������", true);
                //ComboAdmin.SetAll�heckedState(true, true);

                ComboFacility.Title = "��� ������������";
                ComboFacility.Width = 300;
                ComboFacility.MultiSelect = false;
                ComboFacility.ParentSelect = true;
                ComboFacility.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(facilityDigest.UniqueNames, facilityDigest.MemberLevels));
                ComboFacility.Set�heckedState("��������� ������������", true);

                hiddenIndicatorLabel.Text = "���� ��������������� � ������������� ���������� � ������� ������";
            }
            
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, 1, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            if (AdminSliceSelected)
            {
                ComboAdmin.Visible = true;
                ComboMO.Visible = false;

                selectedBudgetSet.Value = GetMultiSelectedItems(adminDigest, ComboAdmin.SelectedValues);
            }
            else
            {
                ComboAdmin.Visible = false;
                ComboMO.Visible = true;

                selectedBudgetSet.Value = GetMultiSelectedItems(moDigest, ComboMO.SelectedValues);
            }

            Page.Title = String.Format("�������������� � ������������ ������ ������� �������� � �������� ������������� �����������");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("{1}, ������ �� {0} ���, ���.���.", currentDate.Year, ComboFacility.SelectedValue);

            ResidualChartCaption.Text = "���������� ��������� �������� �������";
            InventoriesChartCaption.Text = "������������ ������";
            CreditsChartCaption.Text = "������������ �������������";

            ResidualChartCaption.Visible = false;
            InventoriesChartCaption.Visible = false;
            CreditsChartCaption.Visible = false;
            
            selectedFacility.Value = facilityDigest.GetMemberUniqueName(ComboFacility.SelectedValue);

            string dynamicText = String.Empty;
            switch (DynamicType)
            {
                case DynamicChartType.ResidualDynamic:
                    {
                        dynamicChartIndicator.Value = "[����������__������������].[����������__������������].[���].[�������� �������� (���������� ���������, ���.010 - ���.020)]";
                        dynamicText = ResidualChartCaption.Text;
                        break;
                    }
                case DynamicChartType.InventoriesDynamic:
                    {
                        dynamicChartIndicator.Value = "[����������__������������].[����������__������������].[���].[������������ ������(010500000)]";
                        dynamicText = InventoriesChartCaption.Text;
                        break;
                    }
                case DynamicChartType.CreditsDynamic:
                    {
                        dynamicChartIndicator.Value = "[����������__������������].[����������__������������].[������� � ����������� �� �������� �������������� � �� �������� ��������������]";
                        dynamicText = CreditsChartCaption.Text;
                        break;
                    }
            }

            CommentGridCaption.Text = "���������";
            DynamicChartCaption.Text = String.Format("{0} � ������� {1}, �������� �� {2}-{3} ����, ���.���.", dynamicText, AdminSliceSelected ?  "��������" : "������������� �����������",
                currentDate.Year - 2, currentDate.Year);

            // ���� ������ ���� �������
            LargestAdminValueExtruding.Visible = false;
//            largestExtrudeAdmin.Value = LargestAdminValueExtruding.Checked
//                                            ? "- {[�������������__����������].[�������������__����������].[��� ��������������].[������������ �������� � ��������� �������� ������������� �������]}"
//                                            : "";

            LargestMOValueExtruding.Visible = !AdminSliceSelected;
            largestExtrudeMO.Value = LargestMOValueExtruding.Checked
                                            ? "- {[������__������������].[������__������������].[��� ������].[������������� ������� (�����������������)].[��������� ������].[�. �����������]}"
                                            : "";

            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                PieChartsDataBind();
            }

            DynamicChartDataBind();
            GridDataBind();
        }

        private static void SetPieChartAppearance(PieChartBrick pieBrick, string chartName)
        {
            pieBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth * 0.3 - 25);
            pieBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 - 100);

            pieBrick.StartAngle = 180;
            pieBrick.TooltipFormatString = String.Format("<ITEM_LABEL>\n{0}:\n<b><DATA_VALUE:N2></b> ���.���.\n����: <b><PERCENT_VALUE:N2>%</b>", chartName);
            pieBrick.DataFormatString = "N2";
            pieBrick.Legend.Visible = false;
            pieBrick.Legend.Location = LegendLocation.Bottom;
            pieBrick.Legend.SpanPercentage = 30;
            pieBrick.ColorModel = ChartColorModel.ExtendedFixedColors;
            pieBrick.TitleTop = chartName;
            pieBrick.OthersCategoryPercent = 0;
        }

        private static string GetMultiSelectedItems(MemberAttributesDigest digest, Collection<string> selectedItems)
        {
            string uniqueNames = String.Empty;

            foreach (string item in selectedItems)
            {
                uniqueNames += String.Format("{0},", digest.GetMemberUniqueName(item));
            }

            return uniqueNames.TrimEnd(',');
        }

        #region ����������� ���������

        private void PieChartsDataBind()
        {
            string queryName = AdminSliceSelected ? "FO_0002_0052_adminPieChart" : "FO_0002_0052_moPieChart";

            legendChartDt = null;

            pieChartIndicator.Value = "[����������__������������].[����������__������������].[���].[�������� �������� (���������� ���������, ���.010 - ���.020)]";
            string queryText = DataProvider.GetQueryText(queryName);
            residualChartDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryText, "����������", residualChartDt);
            if (residualChartDt.Rows.Count > 0)
            {
                ReplaceGRBSNames(residualChartDt);

                ResidualChartBrick.DataTable = residualChartDt;

                legendChartDt = residualChartDt;
            }

            pieChartIndicator.Value = "[����������__������������].[����������__������������].[���].[������������ ������(010500000)]";
            queryText = DataProvider.GetQueryText(queryName);
            inverntoriesChartDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryText, "����������", inverntoriesChartDt);
            if (inverntoriesChartDt.Rows.Count > 0)
            {
                ReplaceGRBSNames(inverntoriesChartDt);

                InventoriesChartBrick.DataTable = inverntoriesChartDt;

                legendChartDt = inverntoriesChartDt;
            }

            pieChartIndicator.Value = "[����������__������������].[����������__������������].[������� � ����������� �� �������� �������������� � �� �������� ��������������]";
            queryText = DataProvider.GetQueryText(queryName);
            creditsChartDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryText, "����������", creditsChartDt);
            if (creditsChartDt.Rows.Count > 0)
            {
                ReplaceGRBSNames(creditsChartDt);

                CreditsChartBrick.DataTable = creditsChartDt;

                legendChartDt = creditsChartDt;
            }

            LegendChartBrick.DataTable = legendChartDt;
        }

        private void DynamicChartDataBind()
        {
            string queryText = DataProvider.GetQueryText(AdminSliceSelected ? "FO_0002_0052_adminDynamicChart" : "FO_0002_0052_moDynamicChart");
            dynamicChartDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryText, "����������", dynamicChartDt);
            if (dynamicChartDt.Rows.Count > 0)
            {
                ReplaceGRBSNames(dynamicChartDt);
                
                DynamicChartBrick.DataTable = dynamicChartDt;
            }
        }

        private void ReplaceGRBSNames(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (AdminSliceSelected && row[0] != DBNull.Value)
                {
                    row[0] = adminDigest.GetShortName(row[0].ToString());
                }
            }
        }

        private void Chart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            if (AdminSliceSelected)
            {
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                    {
                        Text text = (Text) primitive;

                        string grbsName = text.GetTextString().Replace("'", "\"");
                        
                        text.SetTextString(adminDigest.GetShortName(grbsName));
                    }
                }
            }
        }

        #endregion

        #region ����������� �����

        private void GridDataBind()
        {
            string query = AdminSliceSelected ? DataProvider.GetQueryText("FO_0002_0052_admin_grid") : DataProvider.GetQueryText("FO_0002_0052_mo_grid");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ �����������", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                CommentGridBrick.DataTable = gridDt;
            }
        }

        protected void GRBSGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            int columnCount = e.Layout.Bands[0].Columns.Count;

            for (int i = 0; i < columnCount; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(370);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = CommentGridBrick.GridHeaderLayout;
            GridHeaderCell groupCell = headerLayout.AddCell(
                String.Format("����� ����� �� �������� ���� {0}, ���.���.", AdminSliceSelected ? "������� ��������������" : "������������ �����������"));
            groupCell.AddCell("���������� ��������� �������� �������");
            groupCell.AddCell("������������ ������");
            groupCell.AddCell("������������ �������������");
            headerLayout.ApplyHeaderInfo();
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
            
            Worksheet sheet2 = workbook.Worksheets.Add("���������");
            ReportExcelExporter1.Export(GetMergeChartsImage(), String.Empty, sheet2, 3);

            Worksheet sheet1 = workbook.Worksheets.Add("��������");
            ReportExcelExporter1.Export(DynamicChartBrick.Chart, DynamicChartCaption.Text, sheet1, 3);
        }

        private Graphics g;

        private Image GetMergeChartsImage()
        {
            LegendChartBrick.Width = Convert.ToInt32(LegendChartBrick.Width.Value * 0.8);

            Image chartImg1 = GetChartImage(ResidualChartBrick.Chart);
            Image chartImg2 = GetChartImage(InventoriesChartBrick.Chart);
            Image chartImg3 = GetChartImage(CreditsChartBrick.Chart);
            Image chartImg4 = GetChartImage(LegendChartBrick.Chart);

            Image img = new Bitmap(chartImg1.Width + chartImg2.Width + chartImg3.Width, chartImg1.Height + chartImg4.Height);
            g = Graphics.FromImage(img);

            g.DrawImage(chartImg1, 0, 0);
            g.DrawImage(chartImg2, chartImg1.Width, 0);
            g.DrawImage(chartImg3, chartImg1.Width + chartImg2.Width, 0);
            g.DrawImage(chartImg4, 0, chartImg1.Height);

            return img;
        }

        private static Image GetChartImage(UltraChart chart)
        {
            MemoryStream imageStream = new MemoryStream();
            chart.SaveTo(imageStream, ImageFormat.Png);
            return Image.FromStream(imageStream);
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
            ReportPDFExporter1.Export(GetMergeChartsImage(), String.Empty, section1);

            ISection section2 = report.AddSection();
            DynamicChartBrick.Chart.Width = Convert.ToInt32(DynamicChartBrick.Chart.Width.Value * 0.8);
            ReportPDFExporter1.Export(DynamicChartBrick.Chart, DynamicChartCaption.Text, section2);
        }

        #endregion
    }
}