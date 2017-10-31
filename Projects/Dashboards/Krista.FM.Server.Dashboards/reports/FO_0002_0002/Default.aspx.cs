using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color=System.Drawing.Color;
using Font=System.Drawing.Font;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0002
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart;
        private DataTable dtChartPareto;
        private int firstYear;
        private int endYear = 2011;
        private string month = "������";
        private const int cellCount = 36;
        private string multiplierCaption;

        #endregion

        #region ��������� �������

        // ������� �� � ��
        private CustomParam regionsLevel;
        // ������� ��� �������� ���������
        private CustomParam elementCount;
        // ������� �������
        private CustomParam budgetLevel;
        // ��� ���������
        private CustomParam documentType;
        // ��������� ����
        private CustomParam selectedMeasure;
        // ������ ����
        private CustomParam otherMeasure;
        // ��������� ��������� ������
        private CustomParam rubMultiplier;

        #endregion

        private bool IsFact
        {
            get { return MeasureFact.Checked; }
        }

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ������������� ���������� �������

            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (elementCount == null)
            {
                elementCount = UserParams.CustomParam("element_count");
            }
            if (budgetLevel == null)
            {
                budgetLevel = UserParams.CustomParam("budget_level");
            }
            if (documentType == null)
            {
                documentType = UserParams.CustomParam("document_type");
            }
            if (selectedMeasure == null)
            {
                selectedMeasure = UserParams.CustomParam("selected_measure");
            }
            if (otherMeasure == null)
            {
                otherMeasure = UserParams.CustomParam("other_measure");
            }
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            
            #endregion

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.63);

            multiplierCaption = IsThsRubSelected ? "���.���." : "���.���.";
            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";

            #region ��������� ���������

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.Border.Thickness = 0;
            UltraChart.Data.ZeroAligned = true;

            UltraChart.Axis.X.Extent = 160;
            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.StripLines.PE.Fill = Color.Gainsboro;
            UltraChart.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart.Axis.X.StripLines.PE.Stroke = Color.DarkGray;
            UltraChart.Axis.X.StripLines.Interval = 2;
            UltraChart.Axis.X.StripLines.Visible = true;
            UltraChart.Axis.Y.Extent = 65;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";

            UltraChart.Axis.X.Labels.WrapText = true;
            UltraChart.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;

            UltraChart.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart.Width.Value) / 4;
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 8;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value) / 4;
            UltraChart.TitleLeft.Text = CRHelper.ToUpperFirstSymbol(multiplierCaption);

            UltraChart.Tooltips.FormatString = "<SERIES_LABEL>\n<ITEM_LABEL>";
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart.FillSceneGraph +=new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            #region ��������� �����

            GridBrick.Height = CustomReportConst.minScreenWidth - 30;
            GridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.20 - 100);
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.Auto;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));

            if (!Page.IsPostBack)
            {
                MeasureFact.Attributes.Add("onclick", string.Format("uncheck('{0}')", MeasurePlan.ClientID));
                MeasurePlan.Attributes.Add("onclick", string.Format("uncheck('{0}')", MeasureFact.ClientID));

                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(MeasureFact.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(MeasurePlan.ClientID);

                dtDate = new DataTable();
                string queryName = string.Format("FO_0002_0002_date_{0}", RegionSettingsHelper.Instance.GetPropertyValue("LastDateQueryName"));
                string query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                if (dtDate.Rows.Count != 0)
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                    month = dtDate.Rows[0][3].ToString();
                }
                
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

            Page.Title = RegionSettingsHelper.Instance.GetPropertyValue("ReportTitle");
            PageTitle.Text = Page.Title;
            chart1Label.Text = "������������� �� ������ ���������(+)/��������(-) ������� ��������";

            string monthValue = ComboMonth.SelectedValue;
            string yearValue = ComboYear.SelectedValue;

            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                if (!Page.IsPostBack || !UserParams.PeriodYear.ValueIs(yearValue) ||
                    !UserParams.PeriodMonth.ValueIs(monthValue))
                {
                    int year = Convert.ToInt32(ComboYear.SelectedValue);

                    int monthNum = ComboMonth.SelectedIndex + 1;

                    UserParams.PeriodHalfYear.Value = string.Format("��������� {0}", CRHelper.HalfYearNumByMonthNum(monthNum));
                    UserParams.PeriodQuater.Value = string.Format("������� {0}", CRHelper.QuarterNumByMonthNum(monthNum));
                    UserParams.PeriodYear.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year,
                                                                UserParams.PeriodHalfYear.Value,
                                                                UserParams.PeriodQuater.Value,
                                                                CRHelper.RusMonth(monthNum));

                    regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
                    elementCount.Value = RegionSettingsHelper.Instance.GetPropertyValue("ElementCount");
                    budgetLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("BudgetLevel");
                    documentType.Value = RegionSettingsHelper.Instance.GetPropertyValue("DocumentSKIFType");

                    PageSubTitle.Text = string.Format("�� {0} {1} {2} ����", monthNum, CRHelper.RusManyMonthGenitive(monthNum), year);
                }
                GridDataBind();
            }
            selectedMeasure.Value = IsFact ? "����" : "������� ����������";
            otherMeasure.Value = !IsFact ? "����" : "������� ����������";
            UltraChart.DataBind();
        }

        #region ����������� �����

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0002_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������", dtGrid);

            DataTable dt = new DataTable();
            DataColumn column = new DataColumn("����������", typeof(string));
            dt.Columns.Add(column);
            column = new DataColumn("�����; ����", typeof(string));
            dt.Columns.Add(column);
            column = new DataColumn("�����; ����", typeof(string));
            dt.Columns.Add(column);
            column = new DataColumn("������������� ������; ����", typeof(string));
            dt.Columns.Add(column);
            column = new DataColumn("������������� ������; ����", typeof(string));
            dt.Columns.Add(column);
            column = new DataColumn("��������� ������; ����", typeof(string));
            dt.Columns.Add(column);
            column = new DataColumn("��������� ������; ����", typeof(string));
            dt.Columns.Add(column);

            int sixtetCounter = 0;

            for (int i = 0; i < cellCount; i += 6)
            {
                DataRow row = dt.NewRow();

                string markName = string.Empty;
                switch (sixtetCounter)
                {
                    case 0:
                        {
                            markName = "���������� ������� ��������, �����";
                            break;
                        }
                    case 1:
                        {
                            markName = "�����������";
                            break;
                        }
                    case 2:
                        {
                            markName = "����������";
                            break;
                        }
                    case 3:
                        {
                            markName = "����������������";
                            break;
                        }
                    case 4:
                        {
                            markName = String.Format("����� ���������, {0}", multiplierCaption);
                            break;
                        }
                    case 5:
                        {
                            markName = String.Format("����� ��������, {0}", multiplierCaption);
                            break;
                        }
                }

                row[0] = markName;

                for (int j = 0; j < 6; j++)
                {
                    if (dtGrid.Rows[0][j + i] != DBNull.Value &&
                        (markName.Contains("����� ���������") || markName.Contains("����� ��������")))
                    {
                        row[j + 1] = (Convert.ToDouble(dtGrid.Rows[0][j + i])).ToString("N2");
                    }
                    else if (dtGrid.Rows[0][j + i] != DBNull.Value)
                    {
                        row[j + 1] = Convert.ToInt32(dtGrid.Rows[0][j + i]);
                    }
                }

                dt.Rows.Add(row);
                sixtetCounter++;
            }

            GridBrick.DataTable = dt;
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(210);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            for (int i = 1; i < columnCount; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(150);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridBrick.GridHeaderLayout.AddCell("����������");
            AddGroupCell("�����");
            AddGroupCell("������������� ������");
            AddGroupCell("��������� ������");
            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }

        private void AddGroupCell(string groupName)
        {
            GridHeaderCell groupCell = GridBrick.GridHeaderLayout.AddCell(groupName);
            groupCell.AddCell("���������� ������� ����������");
            groupCell.AddCell("����");
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            switch (e.Row.Index)
            {
                case 1:
                case 4:
                    {
                        e.Row.Style.ForeColor = Color.Green;
                        break;
                    }
                case 2:
                case 5:
                    {
                        e.Row.Style.ForeColor = Color.Red;
                        break;
                    }
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Right = 3;
                if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (e.Row.Cells[0].Value != null &&
                        (e.Row.Cells[0].Value.ToString() == "�����������" ||
                         e.Row.Cells[0].Value.ToString() == "����������" ||
                         e.Row.Cells[0].Value.ToString() == "����������������"))
                    {
                        if (i == 0)
                        {
                            e.Row.Cells[i].Style.Padding.Left = 15;
                        }
                        else if (dtGrid != null)
                        {
                            int j = cellCount + (e.Row.Index - 1) * 6 + i - 1;

                            string value = dtGrid.Rows[0][j].ToString().Replace("br", "\r");
                            for (int k = 3; k <= 4; k++)
                            {
                                value = RegionsNamingHelper.CheckMultiplyValue(value.Replace("br", "\r"), k);
                            }
                            value = value.Replace("������������� �����", "��");
                            value = value.Replace("������������� �����", "��");
                            value = value.Replace("�����", "�-�");
                            e.Row.Cells[i].Title = value;
                        }
                    }
                }
            }
        }
        
//        void UltraWebGrid_DataBound(object sender, EventArgs e)
//        {
//            UltraWebGrid.Height = Unit.Empty;
//        }

        #endregion

        #region ����������� ���������

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0002_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            foreach (DataRow row in dtChart.Rows)
            {
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    if (i == 0 && row[i] != DBNull.Value)
                    {
                        row[i] = row[i].ToString().Replace("��������� �����", "��");
                        row[i] = row[i].ToString().Replace("��������� �����", "��");
                        row[i] = row[i].ToString().Replace("������������� �����������", "��");
                        row[i] = row[i].ToString().Replace("������������� �����", "��");
                        row[i] = row[i].ToString().Replace("������������� �����", "��");
                        row[i] = row[i].ToString().Replace("\"", "'");
                        row[i] = row[i].ToString().Replace(" �����", " �-�");
                    }
                }
            }

            if (dtChart.Columns.Count > 1)
            {
                dtChart.Columns[1].ColumnName = IsFact ? "����������� ��������/�������" : "�������� ��������/�������"; 
            }

            DataTable dtCopyChart = dtChart.Copy();
            if (dtCopyChart.Columns.Count > 2)
            {
                dtCopyChart.Columns.RemoveAt(2);
            }

            UltraChart.DataSource = dtCopyChart;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
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
                    text.labelStyle.Font = new Font("Verdana", 8);
                    text.labelStyle.WrapText = true;
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        string otherMeasureText = string.Empty;
                        double otherValue = 0;
                        if (dtChart != null && dtChart.Rows[box.Row][2] != DBNull.Value && 
                            dtChart.Rows[box.Row][2].ToString() != string.Empty)
                        {
                            otherValue = Convert.ToDouble(dtChart.Rows[box.Row][2]);
                            if (IsFact)
                            {
                                otherMeasureText = otherValue > 0 ? "�������� �������� (�������)" : "�������� ������� (�������)";
                            } 
                            else
                            {
                                otherMeasureText = otherValue > 0 ? "����������� ��������" : "����������� �������";
                            }
                        }

                        double value = Convert.ToDouble(box.Value);
                        if (value > 0)
                        {
                            box.DataPoint.Label = String.Format("{0} {1:N2} {4}\n ({2} {3:N2} {4})", 
                                IsFact ? "����������� ��������" : "�������� �������� (�������)",
                                value, otherMeasureText, otherValue, multiplierCaption);
                            box.PE.ElementType = PaintElementType.Gradient;
                            box.PE.FillGradientStyle = GradientStyle.Horizontal;
                            box.PE.Fill = Color.Green;
                            box.PE.FillStopColor = Color.ForestGreen;
                        }
                        else
                        {
                            box.DataPoint.Label = String.Format("{0} {1:N2} {4}\n ({2} {3:N2} {4})",
                                IsFact ? "����������� �������" : "�������� ������� (�������)",
                                value, otherMeasureText, otherValue, multiplierCaption);
                            box.PE.ElementType = PaintElementType.Gradient;
                            box.PE.FillGradientStyle = GradientStyle.Horizontal;
                            box.PE.Fill = Color.Red;
                            box.PE.FillStopColor = Color.Maroon;
                        }
                    }
                    else if (box.Path != null && box.Path.ToLower().Contains("legend") && box.rect.Width < 20)
                    {
                        box.PE.ElementType = PaintElementType.CustomBrush;
                        LinearGradientBrush brush = new LinearGradientBrush(box.rect, Color.Green, Color.Red, 45, false);
                        box.PE.CustomBrush = brush;
                    }
                }
            }
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
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("���������");
            ReportExcelExporter1.Export(UltraChart, chart1Label.Text, sheet2, 3);
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
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);

            ISection section2 = report.AddSection();
            UltraChart.Width = Convert.ToInt32(UltraChart.Width.Value * 0.8);
            UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value / 2);
            ReportPDFExporter1.Export(UltraChart, chart1Label.Text + " " + PageSubTitle.Text, section2);
        }

        #endregion
    }
}
