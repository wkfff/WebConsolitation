using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using System.Drawing;
using Infragistics.UltraChart.Shared.Styles;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;

namespace Krista.FM.Server.Dashboards.reports.STAT_0002_0010
{
    public enum SliceType
    {
        OKVED,
        OKOPF,
        OKFS
    }

    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable chartDt = new DataTable();
        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private static MemberAttributesDigest periodDigest;
        private static MemberAttributesDigest plantDigest;
        private int firstYear = 2000;
        private CustomParam currentPeriod;
        private CustomParam lastPeriod;
        private CustomParam Finance;
        private CustomParam kind;
        private GridHeaderLayout headerLayout;
        private UltraGridRow gridrow;
        #endregion

        #region ��������� �������

        // ��������� ��� ����� ������
        private CustomParam sliceSet;
        private CustomParam period;
        private CustomParam lastquart;
        private CustomParam datasources;
        private CustomParam rows;
        private CustomParam prds;
        private CustomParam plant;
        private string queryName = string.Empty;

        #endregion


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ��������� �����
            UltraWebGrid1.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.2 * 0.6);
            UltraWebGrid1.Width = Convert.ToInt32(CustomReportConst.minScreenWidth * 0.66);
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(Grid1_InitializeLayout);
            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(Grid1_InitializeRow);
            UltraWebGrid1.DataBinding += new System.EventHandler(UltraWebGrid1_DataBinding);
            UltraWebGrid2.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.8 - 235);
            UltraWebGrid2.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid2.InitializeLayout += new InitializeLayoutEventHandler(Grid2_InitializeLayout);
            UltraWebGrid2.InitializeRow += new InitializeRowEventHandler(Grid2_InitializeRow);
            UltraWebGrid2.DataBinding += new System.EventHandler(UltraWebGrid2_DataBinding);

            #endregion

            #region ��������� ��������� ��������


            #endregion

            #region ������������� ���������� �������

            currentPeriod = UserParams.CustomParam("current_period");
            lastPeriod = UserParams.CustomParam("last_period");
            Finance = UserParams.CustomParam("finance");
            kind = UserParams.CustomParam("kind");
            period = UserParams.CustomParam("period");
            lastquart = UserParams.CustomParam("lastquart");
            datasources = UserParams.CustomParam("data_sources");
            rows = UserParams.CustomParam("rows");
            prds = UserParams.CustomParam("periods");
            plant = UserParams.CustomParam("plant");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DateTime lastDate = CubeInfo.GetLastDate(DataProvidersFactory.SpareMASDataProvider, "STAT_0002_0010_lastDate");
                ComboYear.PanelHeaderTitle = "�������� ������";
                ComboYear.Title = "�������� ������";
                ComboYear.Width = 290;
                ComboYear.ParentSelect = true;
                ComboYear.ShowSelectedValue = true;
                ComboYear.MultipleSelectionType = MultipleSelectionType.SimpleMultiple;
                ComboYear.MultiSelect = true;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0002_0010_Date");
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                ComboYear.SetAll�heckedState(true, false);
                Collection<string> conditions = ComboYear.SelectedValues;
                for (int i = 0; i <= conditions.Count - 1; i++)
                {
                    if (i > conditions.Count - 6)
                    {
                        ComboYear.Set�heckedState(conditions[i], true);
                    }
                    else
                        ComboYear.Set�heckedState(conditions[i], false);
                }

                ComboPlant.PanelHeaderTitle = "�����������";
                ComboPlant.Title = "�����������";
                ComboPlant.Width = 390;
                ComboPlant.ParentSelect = false;
                ComboPlant.ShowSelectedValue = true;
                ComboPlant.MultiSelect = false;
                plantDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0002_0010_Plants");
                ComboPlant.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(plantDigest.UniqueNames, plantDigest.MemberLevels));
                
                ComboPlant.Set�heckedState("����� � ���", true);
                if (ComboPlant.SelectedNode.Nodes.Count == 0)
                {
                    ComboPlant.RemoveTreeNodeByName("����� � ���"); 
                }
                ComboPlant.Set�heckedState("��������", true);
                if (ComboPlant.SelectedNode.Nodes.Count == 0)
                {
                    ComboPlant.RemoveTreeNodeByName("��������"); 
                }
                ComboPlant.Set�heckedState("������������ ��������������", true);
                if (ComboPlant.SelectedNode.Nodes.Count == 0)
                {
                    ComboPlant.RemoveTreeNodeByName("������������ ��������������"); 
                }
                ComboPlant.Set�heckedState("����������", true);
                if (ComboPlant.SelectedNode.Nodes.Count == 0)
                {
                    ComboPlant.RemoveTreeNodeByName("����������"); 
                }
                ComboPlant.Set�heckedState("���������������� ��������", true);
                if (ComboPlant.SelectedNode.Nodes.Count == 0)
                {
                    ComboPlant.RemoveTreeNodeByName("���������������� ��������"); 
                }
                ComboPlant.Set�heckedState("�����", true);
                if (ComboPlant.SelectedNode.Nodes.Count == 0)
                {
                    ComboPlant.RemoveTreeNodeByName("�����"); 
                }
                ComboPlant.Set�heckedState("���������", true);
                if (ComboPlant.SelectedNode.Nodes.Count == 0)
                {
                    ComboPlant.RemoveTreeNodeByName("���������"); 
                }
                ComboPlant.Set�heckedState("���������������� ��������", true);
                if (ComboPlant.SelectedNode.Nodes.Count == 0)
                {
                    ComboPlant.RemoveTreeNodeByName("���������������� ��������"); 
                }
                ComboPlant.Set�heckedState("�������������", true);
                if (ComboPlant.SelectedNode.Nodes.Count == 0)
                {
                    ComboPlant.RemoveTreeNodeByName("�������������");
                }
                ComboPlant.SelectLastNode();
            }
            string periodUniqueName = string.Empty;
            int yearNum = firstYear;
            GridCaption.Text = String.Format("�������� ����������, ��������������� ������������ \"{0}\"", ComboPlant.SelectedValue);
            Label2.Text = String.Format("���������� �����������, ��������������� ������ �������� �����������, �����-���������� ���������� ����� � ����");
            Collection<string> selectedYears = ComboYear.SelectedValues;
            if (selectedYears.Count > 0)
            {

                string years = String.Empty;
                string gridCont = String.Empty;
                prds.Value = string.Empty;
                for (int i = 0; i <= selectedYears.Count - 1; i++)
                {
                    string ps = string.Empty;

                    if (selectedYears[i].Contains("�������"))
                    {
                        ps = ".[������ ���������].[������ ��������]";
                    }
                    else
                    {
                        ps = ".[������ ����]";
                    }
                    if (i == selectedYears.Count - 1)
                    {
                        prds.Value += string.Format("{0}{1}", StringToMDXDate(selectedYears[i]), ps);
                    }
                    else
                    {
                        prds.Value += string.Format("{0}{1},", StringToMDXDate(selectedYears[i]), ps);
                    }
                }

            }
            lastquart.Value = periodUniqueName;
            UserParams.PeriodYear.Value = yearNum.ToString();
            Page.Title = String.Format("���������� �������� ����������� ����-����");
            Label1.Text = Page.Title;
            DocLink.Text = "������������� ����������� �� ��������� ����� ������������";
            DocLink.NavigateUrl = "../STAT_0002_0011/Default.aspx";
            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            plant.Value = plantDigest.UniqueNames[ComboPlant.SelectedValue];
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid2.Bands.Clear();
            UltraWebGrid1.DataBind();
            UltraWebGrid2.DataBind();
            string patternValue = string.Empty;
            int defaultRowIndex = 0;
            patternValue = Finance.Value;
            UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid1, patternValue, UltraWebGrid1.Columns.Count - 1, defaultRowIndex);
            ActiveGridRow(row);

        }

        #region ����������� �����

        private void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0002_0010_grid1");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, " ", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                gridDt.Rows.RemoveAt(0);
                UltraWebGrid1.DataSource = gridDt;
            }
        }

        private void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0002_0010_grid2");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "������������ �����������", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                if (gridDt.Columns.Count > 1)
                {
                    gridDt.Columns.RemoveAt(0);
                }
                gridDt.Rows.RemoveAt(0);
                gridDt.Rows.RemoveAt(0);
                gridDt.Rows.RemoveAt(0);
                UltraWebGrid2.DataSource = gridDt;
            }
        }
        private void ActiveGridRow(UltraGridRow row)
        {

        }

        protected void Grid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(240);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(490);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Caption = string.Empty;
            }
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
        }

        protected void Grid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 1)
            {
                return;
            }
            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(340);
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(60);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.Bands[0].Columns[1].MergeCells = true;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 3].Hidden = true;
            headerLayout = new GridHeaderLayout(UltraWebGrid2);
            string queryText = DataProvider.GetQueryText("STAT_0002_0010_header");
            DataTable headerDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(queryText, "��.�.", headerDt);
            headerLayout.AddCell("� �.�.");
            headerLayout.AddCell("����������");
            for (int i = 2; i < columnCount - 2; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(130);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                if (e.Layout.Bands[0].Columns[i].Header.Caption.ToString().Contains("��������"))
                {
                    string year = headerDt.Rows[i - 2][2].ToString().Split('[')[4].Split(']')[0].ToString();
                    string quarter = headerDt.Rows[i - 2][2].ToString().Split('[')[6].Split(']')[0].ToString().Split(' ')[1].ToString();
                    headerLayout.AddCell(String.Format("{0} ������� {1} ����", quarter, year));
                }
                else if (e.Layout.Bands[0].Columns[i].Header.Caption.ToString().Contains("���"))
                { headerLayout.AddCell(String.Format("{0}", headerDt.Rows[i - 2][1].ToString()));
             } 
            }
            headerLayout.ApplyHeaderInfo();
            e.Layout.Bands[0].Columns[0].MergeCells = true;
            e.Layout.Bands[0].Columns[1].MergeCells = true;
        }

        protected void Grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }
        protected void Grid1_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value.ToString().Contains("������������ �����������"))
            {
                e.Row.Style.Font.Bold = true;
            }
        }

        protected void Grid2_InitializeRow(object sender, RowEventArgs e)
        {
            if (UltraWebGrid2.Rows.Count > 0)
            {
                int cellCount = e.Row.Cells.Count;
                int type = 0;
                if (e.Row.Cells[cellCount - 3].Value != null)
                {
                    type = Convert.ToInt32(e.Row.Cells[cellCount - 3].Value.ToString());
                }
                if (e.Row.Cells[1].Value != null)
                {
                    e.Row.Cells[1].Value = e.Row.Cells[1].Value.ToString().Replace(e.Row.Cells[1].Value.ToString().Split(',')[e.Row.Cells[1].Value.ToString().Split(',').Length - 1], e.Row.Cells[1].Value.ToString().Split(',')[e.Row.Cells[1].Value.ToString().Split(',').Length - 1].ToLower()); //������� �� �������� �����
                }

                for (int i = 2; i < cellCount - 2; i++)
                {
                    UltraGridCell cell = e.Row.Cells[i];
                    cell.Style.Padding.Right = 3;
                    string prev = UltraWebGrid2.Columns[i - 1].Header.Caption.ToString();
                    if (i == 2)
                    {
                        prev = "����������� �������";
                    }

                    if (prev.Contains("�������"))
                    {
                        prev = prev.Replace("�������", "��������");

                    }
                    else
                        prev = prev.Replace("���", "����");
                    switch (type)
                    {
                        case 0:
                            {
                                if ((cell.Value != null) && (i != 0) && (!e.Row.Cells[1].ToString().Contains("������ �������� (�___��___)")))
                                {
                                    if (e.Row.Cells[1].ToString().Contains("�������"))
                                    {
                                        cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N0");
                                    }
                                    else
                                    {
                                        cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N1");
                                    }
                                }

                                cell.Style.BorderDetails.WidthBottom = 0;
                                break;
                            }
                        case 1:
                            {
                                if ((cell.Value != null) && (i != 0) && (!e.Row.Cells[1].ToString().Contains("������ �������� (�___��___)")))
                                {
                                    if (i == 1)
                                    {
                                        cell.Value = string.Empty;
                                        cell.Style.BorderDetails.WidthTop = 0;
                                        cell.Style.BorderDetails.WidthBottom = 0;
                                        break;
                                    }
                                    if (e.Row.Cells[1].ToString().Contains("�������"))
                                    {
                                        cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N0");
                                    }
                                    else
                                    {
                                        cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N1");
                                    }
                                }
                                cell.Style.BorderDetails.WidthTop = 0;
                                cell.Style.BorderDetails.WidthBottom = 0;
                                cell.Title = string.Format("���������� ���������� � {0}", prev);
                                break;
                            }
                        case 2:
                            {
                                if ((cell.Value != null) && (i != 0) && (!e.Row.Cells[1].ToString().Contains("������ �������� (�___��___)")))
                                {
                                    if (i == 1)
                                    {
                                        cell.Value = string.Empty;
                                        cell.Style.BorderDetails.WidthTop = 0;
                                        break;
                                    }
                                    if (e.Row.Cells[1].ToString().Contains("����������� ���������� ���������� ������") ||
                                    e.Row.Cells[1].ToString().Contains("���� ����������� ���������� ����� ���������� �� �������� ������") ||
                                    e.Row.Cells[1].ToString().Contains("������� ���������� �����") ||
                                    e.Row.Cells[1].ToString().Contains("����� ����������� ������� ") ||
                                    e.Row.Cells[1].ToString().Contains("����� ��������� �������") ||
                                    e.Row.Cells[1].ToString().Contains("���������� � �������� �������") ||
                                    e.Row.Cells[1].ToString().Contains("� �. �. � ��������������� �������") ||
                                    e.Row.Cells[1].ToString().Contains("����� ��������������") ||
                                    e.Row.Cells[1].ToString().Contains("���������� ��������� � ������ ���������� ��������� ������� ����") ||
                                    e.Row.Cells[1].ToString().Contains("��������� �������� ������ �� ������ ������� ��������� �� ����� ��������� �������") ||
                                    e.Row.Cells[1].ToString().Contains("������� (������) �� ��������������� �� �������� ������"))
                                    {

                                        double growRate = Convert.ToDouble(cell.Value.ToString());
                                        cell.Value = growRate.ToString("P2");

                                        if (growRate > 0)
                                        {
                                            cell.Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                                            cell.Title = string.Format("���� �������� � {0}", prev);
                                        }
                                        else if (growRate < 0)
                                        {
                                            cell.Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                            cell.Title = string.Format("���� �������� � {0}", prev);
                                        }
                                    }
                                    else
                                    {
                                        double growRate = Convert.ToDouble(cell.Value.ToString());
                                        cell.Value = growRate.ToString("P2");
                                        if (growRate > 0)
                                        {
                                            cell.Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                                            cell.Title = string.Format("���� �������� � {0}", prev);
                                        }
                                        else if (growRate < 0)
                                        {
                                            cell.Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                                            cell.Title = string.Format("���� �������� � {0}", prev);
                                        }
                                    }
                                    cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center; margin: 2px";

                                }
                                cell.Style.BorderDetails.WidthTop = 0;
                                break;
                            }
                    }
                    if ((cell.Value != null) && (i != 0) && (e.Row.Cells[1].ToString().Contains("������� �������������� ���������")))
                    {
                        e.Row.Cells[0].Value = "12";
                        e.Row.Cells[1].Value = "������� �������������� ���������";
                    }
                    if ((cell.Value != null) && (i != 0) && (e.Row.Cells[1].ToString().Contains("������ �������� (�___��___)")))
                    {
                        e.Row.Cells[0].Value = "12.1";
                    }
                    if ((cell.Value != null) && (i != 0) && (e.Row.Cells[1].ToString().Contains("���������� �� ����� ��������� �������")))
                    {
                        e.Row.Cells[0].Value = "12.3";
                    }
                    if (e.Row.Cells[cellCount - 1].Value != null)
                    {
                        if ((e.Row.Cells[cellCount - 1].Value.ToString().Contains(",") || (e.Row.Cells[0].Value.ToString().Contains("12.1"))))
                        {
                            e.Row.Cells[1].Style.Padding.Left = 50;
                        }
                    }
                    if (cell.Value != null)  
                    {
                        if (e.Row.Cells[0].Value.ToString().Contains("12.1"))
                        {
                            e.Row.Cells[1].Style.Padding.Left = 50;
                            //if (!e.Row.Cells[1].Value.ToString().Contains("���"))
                            //{
                            e.Row.Cells[1].Value = "������ �������� (�___��___), ���";
                            //}
                        }
                    }
                }
            }
        }


        #endregion
        protected void FillComboDate(CustomMultiCombo combo, string queryName, int offset)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            if (dtDate.Rows.Count == 0)
            {
                throw new Exception("������ ��� ���������� ������ ����������� � ����");
            }
            Dictionary<string, int> dictDate = new Dictionary<string, int>();
            for (int row = 0; row < dtDate.Rows.Count - offset; ++row)
            {
                string year = dtDate.Rows[row][0].ToString();
                string month = dtDate.Rows[row][3].ToString();
                AddPairToDictionary(dictDate, year + " ���", 0);
                AddPairToDictionary(dictDate, month + " " + year + " ����", 1);
            }
            combo.FillDictionaryValues(dictDate);
        }

        protected void AddPairToDictionary(Dictionary<string, int> dict, string key, int value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, value);
            }
        }

        public string StringToMDXDate(string str)
        {
            if (str.Contains("�������"))
            {
                string template = "[������__������].[������__������].[������ ���� ��������].[{0}].[��������� {1}].[������� {2}]";
                string[] dateElements = str.Split(' ');
                int year = Convert.ToInt32(dateElements[2]);
                int quarter = Convert.ToInt32(dateElements[0]);
                int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
                return String.Format(template, year, halfYear, quarter);
            }
            else
            {
                return String.Format("[������__������].[������__������].[������ ���� ��������].[{0}]", str.Split(' ')[0].ToString());
            }
        }

        #region ������� � Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;
            SetExportGridParams(headerLayout.Grid);
            Workbook workbook = new Workbook();
            GridHeaderLayout headerinfo = new GridHeaderLayout(UltraWebGrid1);
            Worksheet sheet1 = workbook.Worksheets.Add("�������");
            Worksheet sheet2 = workbook.Worksheets.Add("����������");
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(headerinfo, sheet2, 3);
            for (int i = 0; i < UltraWebGrid2.Columns.Count; i++)
            {
                if (i > 1)
                {
                    sheet1.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                }
                sheet1.Rows[4].Cells[i].CellFormat.Font.Name = "Verdana";
                sheet1.Rows[4].Cells[i].CellFormat.Font.Height = 200;
            }
            for (int i = 0; i < UltraWebGrid1.Columns.Count; i++)
            {
                if (i > 1)
                {
                    sheet1.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                }
                sheet1.Rows[4].Cells[i].CellFormat.Font.Name = "Verdana";
                sheet1.Rows[4].Cells[i].CellFormat.Font.Height = 200;
            }
        }

        #endregion

        #region ������� � PDF

        private void ExportGridSetup()
        {
            for (int i = 0; i < UltraWebGrid1.Rows.Count; i++)
            {
                UltraGridCell cell = UltraWebGrid2.Rows[i].Cells[0];

                int groupIndex = i % 3;

                switch (groupIndex)
                {
                    case 0:
                        {
                            cell.Value = String.Empty;
                            cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
                            break;
                        }
                    case 1:
                        {
                            cell.Style.BorderDetails.StyleTop = BorderStyle.None;
                            cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
                            break;
                        }
                    case 2:
                        {
                            cell.Value = String.Empty;
                            cell.Style.BorderDetails.StyleTop = BorderStyle.None;
                            break;
                        }
                }
            }
        }
        private void SetExportGridParams(UltraWebGrid grid)
        {
            string exportFontName = "Verdana";
            int fontSize = 10;
            double coeff = 1.0;

            grid.Columns.Add("��������� �������");
            foreach (UltraGridRow Row in grid.Rows)
            {
                if (Row.Index % 3 == 0)
                {
                    Row.Cells.FromKey("��������� �������").Value = "��������";
                    Row.NextRow.Cells.FromKey("��������� �������").Value = "���������� ����������";
                    Row.NextRow.NextRow.Cells.FromKey("��������� �������").Value = "���� ��������";
                }
            }

            headerLayout = new GridHeaderLayout(UltraWebGrid2);
            headerLayout.AddCell("��.�.");
            headerLayout.AddCell("����������");
            headerLayout.AddCell(" ");
            // ���������
            GridHeaderCell lowcell = headerLayout;
            for (int i = 3; i < UltraWebGrid2.Columns.Count - 3; i = i + 1)
            {
                headerLayout.AddCell(headerLayout.Grid.Columns[i - 1].Header.Caption);
            }

            headerLayout.ApplyHeaderInfo();

            grid.Columns.FromKey("��������� �������").Move(2);
            grid.Columns.FromKey("��������� �������").Width = 180;

            foreach (UltraGridColumn column in grid.Columns)
            {
                column.Width = Convert.ToInt32(column.Width.Value * coeff);
                column.CellStyle.Font.Name = exportFontName;
                column.Header.Style.Font.Name = exportFontName;
                column.CellStyle.Font.Size = fontSize;
                column.Header.Style.Font.Size = fontSize;
            }
        }
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ExportGridSetup();
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            SetExportGridParams(headerLayout.Grid);

            ReportPDFExporter1.HeaderCellHeight = 60;
            Infragistics.Documents.Reports.Report.Text.IText header1 = section1.AddText();
            header1.Style.Font.Name = "Verdana";
            header1.Style.Font.Size = 15;
            header1.Style.Font.Bold = true;
            header1.AddContent(Label1.Text);

            Infragistics.Documents.Reports.Report.Text.IText header2 = section1.AddText();
            header2.Style.Font.Name = "Verdana";
            header2.Style.Font.Size = 13;
            header2.AddContent(Label2.Text);

            ReportPDFExporter1.Export(headerLayout, "", section1);
        }

        #endregion
    }
}