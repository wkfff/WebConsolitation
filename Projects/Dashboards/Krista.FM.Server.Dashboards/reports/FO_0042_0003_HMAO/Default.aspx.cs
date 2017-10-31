using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
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

namespace Krista.FM.Server.Dashboards.reports.FO_0042_0003_HMAO
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtChart;
        private int firstYear = 2009;
        private int endYear = 2011;

        private double avgEvaluation;
        private static MemberAttributesDigest grbsDigest;
        private GridHeaderLayout headerLayout;

        #endregion

        #region ��������� �������

        // ��������� ���������
        private CustomParam selectedIndicator;
        // ��������� ������
        private CustomParam selectedPeriod;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.45);
            UltraWebGrid.DisplayLayout.NoDataMessage = "��� ������";
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);
            

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.50);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #region ��������� ���������

            UltraChart.ChartType = ChartType.ColumnChart;
            UltraChart.ColumnChart.ColumnSpacing = 0;
            UltraChart.ColumnChart.SeriesSpacing = 0;
            UltraChart.Data.ZeroAligned = true;
            UltraChart.Border.Thickness = 0;

            UltraChart.Axis.X.Extent = 30;
            UltraChart.Axis.X.Labels.Visible = false;
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
            UltraChart.Axis.Y.Extent = 20;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Axis.X2.Visible = true;
            UltraChart.Axis.X2.Labels.Visible = false;
            UltraChart.Axis.X2.Labels.SeriesLabels.Visible = false;
            UltraChart.Axis.X2.LineColor = Color.Transparent;
            UltraChart.Axis.X2.Extent = 10;

            UltraChart.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart.Width.Value) / 4;
            UltraChart.Legend.Font = new Font("Verdana", 8);
            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 9;
            UltraChart.Legend.FormatString = "�������";

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            UltraChart.TitleLeft.Text = "���������� ����";
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);

            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleBottom.Margins.Left = UltraChart.Axis.Y.Extent;
            UltraChart.TitleBottom.Text = "������";
            UltraChart.TitleBottom.Font = new Font("Verdana", 8);

            //CRHelper.FillCustomColorModel(UltraChart, 10, true);

            UltraChart.Legend.MoreIndicatorText = " ";
            UltraChart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            Color color1 = Color.LimeGreen;
            Color color2 = Color.DodgerBlue;
            Color color3 = Color.Gold;
            Color color4 = Color.Red;
            Color color5 = Color.Orange;

            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color3, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color4, 150));
            UltraChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color5, 150));
            UltraChart.ColorModel.Skin.ApplyRowWise = true;

            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N0>";
            appearance.ChartTextFont = new Font("Verdana", 9, FontStyle.Bold);
            appearance.Visible = true;
            UltraChart.ColumnChart.ChartText.Add(appearance);

            UltraChart.Tooltips.FormatString = "������ � ��������� <SERIES_LABEL> ����������� � <DATA_VALUE:N0> ����\n<ITEM_LABEL>";

            #endregion

            #region ������������� ���������� �������
            
            selectedIndicator = UserParams.CustomParam("selected_indicator");
            selectedPeriod = UserParams.CustomParam("selected_period");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            CrossLink1.Visible = true;
            CrossLink1.Text = "����������&nbsp;������&nbsp;��������";
            CrossLink1.NavigateUrl = "~/reports/FO_0042_0001_HMAO/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "�������&nbsp;����";
            CrossLink2.NavigateUrl = "~/reports/FO_0042_0002_HMAO/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "��������&nbsp;�����������&nbsp;������&nbsp;��������";
            CrossLink3.NavigateUrl = "~/reports/FO_0042_0004_HMAO/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(chartCaption);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0042_0003_HMAO_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string quarter = dtDate.Rows[0][2].ToString();

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(endYear.ToString(), true);

                ComboQuarter.Title = "������ ��������";
                ComboQuarter.Width = 300;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillDateQuarters());
                ComboQuarter.Set�heckedState(GetParamQuarter(quarter), true);

                grbsDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0042_0003_HMAO_grbsList");

                hiddenIndicatorLabel.Text = "[����������__������ �������� ��_������������].[����������__������ �������� ��_������������].[��� ����������].[1. ������������� ���������� ������������]";
            }

            int quarterNum = ComboQuarter.SelectedIndex + 1;

            Page.Title = String.Format("������ ������ �������� ����������� �����������, ��������������� �������� ��������������� ������� ������� ���� - ���� � ������� �����������");
            PageTitle.Text = Page.Title;

            PageSubTitle.Text = (quarterNum != 4)
               ? String.Format("{0}.{1}", ComboQuarter.SelectedValue, ComboYear.SelectedValue)
               : String.Format("�� ������ {0} ����", ComboYear.SelectedValue);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("��������� {0}", CRHelper.HalfYearNumByQuarterNum(quarterNum));
            UserParams.PeriodQuater.Value = String.Format("������� {0}", quarterNum);

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);

            FillGRBSDictionary();

            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                headerLayout = new GridHeaderLayout(UltraWebGrid);
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();

                string patternValue = selectedIndicator.Value;
                int defaultRowIndex = 0;
                if (patternValue == string.Empty)
                {
                    defaultRowIndex = 0;
                }

                if (UltraWebGrid.Columns.Count > 0 && UltraWebGrid.Rows.Count > 0)
                {
                    // ���� ������
                    UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, UltraWebGrid.Columns.Count - 1, defaultRowIndex);
                    // �������� ������
                    ActiveGridRow(row);
                }
            }

           //UltraChart.DataBind();
        }

        /// <summary>
        /// �������� ������� ��������� �� �������� ��������������
        /// </summary>
        /// <param name="classQuarter">������� ��������������</param>
        /// <returns>�������� ���������</returns>
        private static string GetParamQuarter(string classQuarter)
        {
            switch (classQuarter)
            {
                case "������� 1":
                    {
                        return "�� ��������� �� 01.04";
                    }
                case "������� 2":
                    {
                        return "�� ��������� �� 01.07";
                    }
                case "������� 3":
                    {
                        return "�� ��������� �� 01.10";
                    }
                case "������ ����":
                    {
                        return "�� ������ ����";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        private Dictionary<string, string> grbsDictionary;

        private void FillGRBSDictionary()
        {
            grbsDictionary = new Dictionary<string, string>();

            grbsDictionary.Add("10", "���� ����");
            grbsDictionary.Add("20", "������������� ����");
            grbsDictionary.Add("30", "����������������� ����");
            grbsDictionary.Add("40", "����������������� ���� � �. ������");
            grbsDictionary.Add("120", "��� ����");
            grbsDictionary.Add("130", "����������� ����");
            grbsDictionary.Add("160", "������������� ����");
            grbsDictionary.Add("170", "������������ ����");
            grbsDictionary.Add("177", "�� �� ����� �� � �� ����");
            grbsDictionary.Add("180", "��������� � ���������� ����");
            grbsDictionary.Add("188", "��� ����");
            grbsDictionary.Add("210", "��������� � ���������� ����");
            grbsDictionary.Add("230", "�������������� � �������� ����");
            grbsDictionary.Add("240", "����������� ����");
            grbsDictionary.Add("250", "����������� ������������ ������ ����");
            grbsDictionary.Add("260", "�������� ����");
            grbsDictionary.Add("270", "�������� ����");
            grbsDictionary.Add("280", "������� �� ���������� �������� ����");
            grbsDictionary.Add("290", "�������������� ����");
            grbsDictionary.Add("340", "����������� ����");
            grbsDictionary.Add("350", "�������� � ��������� ����");
            grbsDictionary.Add("360", "�������� ����������� ����");
            grbsDictionary.Add("370", "����������� ����������� ������ ��������� ����");
            grbsDictionary.Add("380", "���������� ����");
            grbsDictionary.Add("390", "����������������� ���� � �. �����-����������");
            grbsDictionary.Add("400", "��������������� � ����������� ������� ��������� ����");
            grbsDictionary.Add("410", "��������� ����");
            grbsDictionary.Add("420", "�������������� ����");
            grbsDictionary.Add("430", "������������ ����");
            grbsDictionary.Add("440", "������������� �������� ����");
            grbsDictionary.Add("450", "������� �������������� ����");
            grbsDictionary.Add("460", "�������� ����");
            grbsDictionary.Add("470", "���������� ��� ����");
            grbsDictionary.Add("480", "������������������ � ��� ����");
            grbsDictionary.Add("490", "������������������ ������ ����");
            grbsDictionary.Add("500", "������ ����");
            grbsDictionary.Add("510", "�������� ����");
            grbsDictionary.Add("520", "�������������� ����");
            grbsDictionary.Add("530", "������������ ����");
            grbsDictionary.Add("540", "����������������� ���� � �. �������������");
            grbsDictionary.Add("550", "�������� ������ ����");
            grbsDictionary.Add("560", "������������ ����");
            grbsDictionary.Add("570", "������������������� ����");
            grbsDictionary.Add("580", "����������� ����");
            grbsDictionary.Add("590", "����������� ����������� ����");
            grbsDictionary.Add("600", "������������ ����");
            grbsDictionary.Add("610", "�������������� ����");
            grbsDictionary.Add("620", "�������� ����");
            grbsDictionary.Add("630", "��������� ����");
            grbsDictionary.Add("640", "������������� ����");
        }

        private string GetShortGRBSName(string grbsName)
        {
            string grbsCode = grbsDigest.GetMemberType(grbsName);
            if (grbsDictionary.ContainsKey(grbsCode))
            {
                return grbsDictionary[grbsCode];
            }
            return grbsName;
        }

        #region ����������� �����
        
        /// <summary>
        /// ��������� ������ �����
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            string indicatorName = row.Cells[1].Text;

            avgEvaluation = Double.MinValue;
            UltraChart.Legend.Visible = false;
            if (row.Cells[2].Value != null && row.Cells[2].Value.ToString() != String.Empty)
            {
                avgEvaluation = Convert.ToDouble(row.Cells[2].Value);
                UltraChart.Legend.Visible = true;
                UltraChart.Legend.FormatString = String.Format("������� �������� ������: {0:N2}", avgEvaluation);
            }

            hiddenIndicatorLabel.Text = row.Cells[row.Cells.Count - 1].Text;
            selectedIndicator.Value = hiddenIndicatorLabel.Text;

            chartCaption.Text = String.Format("���������� �{0}�", indicatorName);

            UltraChart.DataBind();
        }

        void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0003_HMAO_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ ����������", dtGrid);
            if (dtGrid.Rows.Count > 0 && dtGrid.Columns.Count > 2)
            {
                dtGrid.Columns.RemoveAt(0);

                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 3; i <= 5; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            row[i] = GetShortGRBSNames(row[i].ToString());
                        }
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        private string GetShortGRBSNames(string fullNames)
        {
            string shortNames = String.Empty;

            string[] names = fullNames.Split(';');
            foreach (string s in names)
            {
                if (s != String.Empty)
                {
                    shortNames += String.Format("{0}, ", GetShortGRBSName(s));
                }
            }

            return shortNames.TrimEnd(' ').TrimEnd(',');
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            UltraGridColumn numberColumn = e.Layout.Bands[0].Columns[0];
            numberColumn.Header.Caption = "� �/�";
            numberColumn.Width = CRHelper.GetColumnWidth(30);
            numberColumn.CellStyle.Padding.Right = 5;
            numberColumn.CellStyle.BackColor = numberColumn.Header.Style.BackColor;
            numberColumn.CellStyle.Font.Bold = true;
            numberColumn.SortingAlgorithm = SortingAlgorithm.NotSet;
            numberColumn.CellStyle.HorizontalAlign = HorizontalAlign.Right;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "";
                int widthColumn = 250;
                bool textWrap = false;
                HorizontalAlign textAlignment = HorizontalAlign.Left;

                switch(i)
                {
                    case 1:
                        {
                            widthColumn = 280;
                            textWrap = true;
                            break;
                        }
                    case 2:
                        {
                            widthColumn = 100;
                            formatString = "N2";
                            textAlignment = HorizontalAlign.Right;
                            break;
                        }
                    case 3:
                    case 4:
                    case 5:
                        {
                            widthColumn = 233;
                            textWrap = true;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = textAlignment;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = textWrap;
            }

            e.Layout.Bands[0].Columns[6].Hidden = true;

            headerLayout.AddCell("� �/�");
            headerLayout.AddCell("������������ ����������");
            headerLayout.AddCell("������� ������ ���������� ������ �� ��� ������������ � ����");
            headerLayout.AddCell("����, ���������� ����������� ������");
            headerLayout.AddCell("����, ���������� ������������ ������");
            headerLayout.AddCell("����, �� �������� ���������� �� ��������");

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            e.Row.Cells[0].Value = Convert.ToInt32(e.Row.Index + 1).ToString("N0");

            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
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

        #region ����������� ���������

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0003_HMAO_chart_tenth");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            if (dtChart.Columns.Count > 0 && !IsZeroOrNullColumn(dtChart, 1))
            {
                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("_", "]");
                    }

                    if (row[2] != DBNull.Value)
                    {
                        row[2] = GetShortGRBSNames(row[2].ToString());
                    }
                }

                DataTable dtChartCopy = dtChart.Copy();
                if (dtChartCopy.Columns.Count > 2)
                {
                    dtChartCopy.Columns.RemoveAt(2);
                }

                UltraChart.DataSource = dtChartCopy;
            }
        }

        private static bool IsZeroOrNullColumn(DataTable dt, int columnIndex)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row[columnIndex] != DBNull.Value && row[columnIndex].ToString() != String.Empty)
                {
                    double value = Convert.ToDouble(row[columnIndex]);
                    if (value != 0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            if (dtChart != null && dtChart.Rows.Count > 0)
            {
                for (int i = 0; i < e.SceneGraph.Count; i++)
                {
                    Primitive primitive = e.SceneGraph[i];
                    if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                    {
                        Text axisText = (Text)primitive;
                        axisText.bounds.Width = 50;
                        axisText.labelStyle.HorizontalAlign = StringAlignment.Center;
                        axisText.labelStyle.FontSizeBestFit = false;
                        axisText.labelStyle.Font = new Font("Verdana", 8);
                        axisText.labelStyle.WrapText = false;
                    }
                    if (primitive is Box)
                    {
                        Box box = (Box)primitive;
                        if (box.DataPoint != null && box.Value != null)
                        {
                            int rowIndex = box.Row;
                            int columnIndex = box.Column + 2;

                            string indicatorList = String.Empty;
                            if (dtChart != null && dtChart.Rows[rowIndex][columnIndex] != DBNull.Value &&
                                dtChart.Rows[rowIndex][columnIndex].ToString() != String.Empty)
                            {
                                string list = String.Format("({0})", dtChart.Rows[rowIndex][columnIndex].ToString().TrimEnd(','));
                                //list = BreakCollocator(list, ',', 5);
                                indicatorList = GetWrapText(list.Replace(",", ", "), 60);
                            }

                            box.DataPoint.Label = indicatorList;
                        }
                        else if (box.Path != null && box.Path.ToLower().Contains("legend") && box.rect.Width < 20)
                        {
                            box.PE.Fill = Color.MediumPurple;
                            box.PE.FillStopColor = Color.MediumOrchid;
                            box.rect = new Rectangle(box.rect.X, box.rect.Y + box.rect.Height / 3, box.rect.Width, box.rect.Height / 3);
                        }
                    }
                }

                if (avgEvaluation != Double.MinValue)
                {
                    IAdvanceAxis xAxis = (IAdvanceAxis) e.Grid["X"];
                    IAdvanceAxis yAxis = (IAdvanceAxis) e.Grid["Y"];

                    if (xAxis == null || yAxis == null)
                        return;

                    double yMin = yAxis.MapMinimum;
                    double yMax = yAxis.MapMaximum;

                    double axisStep = (xAxis.Map(1) - xAxis.Map(0));
                    int scale = 10;
                    double colIndex = avgEvaluation * scale;
                    double lineX = xAxis.Map(colIndex) + avgEvaluation/axisStep;

                    Line line = new Line();
                    line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                    line.p1 = new Point((int)lineX, (int)yMax);
                    line.PE.Stroke = Color.MediumOrchid;
                    line.PE.FillOpacity = 50;
                    line.PE.StrokeWidth = 5;
                    line.p2 = new Point((int)lineX, (int)yMin);
                    e.SceneGraph.Add(line);
                    //e.SceneGraph.Insert(10, line);
                }
            }
        }

        private static string GetWrapText(string text, int charCount)
        {
            bool wrap = false;
            string wrapText = String.Empty;
            int rowLenght = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (!wrap)
                {
                    wrap = (rowLenght == charCount);
                }

                if (wrap && text[i] == ' ')
                {
                    wrapText += '\n';
                    wrap = false;
                    rowLenght = 0;
                }
                else
                {
                    rowLenght++;
                    wrapText += text[i];
                }
            }

            return wrapText;
        }

        private static string BreakCollocator(string source, char breakChar, int charIndex)
        {
            string breakedStr = String.Empty;

            int charCount = 0;
            foreach (char ch in source)
            {
                breakedStr += ch;
                if (ch == breakChar)
                {
                    charCount++;
                    if (charCount == charIndex)
                    {
                        breakedStr += "\n";
                        charCount = 0;
                    }
                }
            }

            return breakedStr;
        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 2 * Convert.ToInt32(UltraChart.Width.Value / 3);

            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart, chartCaption.Text, section2);
        }

        #endregion

        #region ������� � Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");

            ReportExcelExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.GridColumnWidthScale = 1.5;
            ReportExcelExporter1.SheetColumnCount = 20;

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.75));
            UltraChart.Legend.Margins.Right = 2 * Convert.ToInt32(UltraChart.Width.Value / 3);
            ReportExcelExporter1.Export(UltraChart, sheet2, 3);
        }

        #endregion
    }
}
