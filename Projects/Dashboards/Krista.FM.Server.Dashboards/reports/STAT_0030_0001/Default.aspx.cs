using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.STAT_0030_0001
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable gridDt = new DataTable();
        private DateTime currentDate;
        private DateTime prevDate;
        private static MemberAttributesDigest periodDigest;

        // ��� ����� � ������� �������
        private const string mapFolderName = "����";

        #endregion

        private int GetScreenWidth
        {
            get
            {
                if (Request.Cookies != null)
                {
                    if (Request.Cookies[CustomReportConst.ScreenWidthKeyName] != null)
                    {
                        HttpCookie cookie = Request.Cookies[CustomReportConst.ScreenWidthKeyName];
                        int value = Int32.Parse(cookie.Value);
                        return value;
                    }
                }
                return (int)Session["width_size"];
            }
        }

        private bool IsSmallResolution1200
        {
            get { return GetScreenWidth < 1200; }
        }

        private bool IsSmallResolution900
        {
            get { return GetScreenWidth < 900; }
        }

        #region ��������� �������

        // ��������� ����� �����
        private CustomParam gridRowSet;
        // ������� ������
        private CustomParam currentPeriod;
        // ������� ������
        private CustomParam previousPeriod;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region ��������� �����

            GridBrick.Height = Unit.Empty;
            GridBrick.Width = IsSmallResolution900 ? 750 : IsSmallResolution1200 ? 950 : 1050;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow +=new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            #region ������������� ���������� �������

            gridRowSet = UserParams.CustomParam("grid_row_set");
            currentPeriod = UserParams.CustomParam("current_period");
            previousPeriod = UserParams.CustomParam("previous_period");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            periodDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0030_0001_periodDigest");

            if (!Page.IsPostBack)
            {
                ComboYear.Title = "������";
                ComboYear.Width = 200;
                ComboYear.MultiSelect = false;
                ComboYear.ParentSelect = true;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                ComboYear.SelectLastNode();
            }
            
            if (ComboYear.SelectedValue.Contains("2")) // ������ ���
            {
                 if (ComboYear.SelectedValue.Contains("2011"))
                 {
                     string curdate = periodDigest.GetMemberUniqueName(ComboYear.SelectedValue);
                     currentPeriod.Value = string.Format("{0}.DATAMEMBER", curdate);
                     currentDate = CRHelper.DateByPeriodMemberUName(curdate, 3);
                 }
                 else
                 {
                     currentPeriod.Value = periodDigest.GetMemberUniqueName(ComboYear.SelectedValue);
                     currentDate = CRHelper.DateByPeriodMemberUName(currentPeriod.Value, 3);
                 }
                 
                 prevDate = currentDate.AddYears(-1);
                 previousPeriod.Value = CRHelper.PeriodMemberUName("[������__��� ������� �����].[������__��� ������� �����]", prevDate, 1);
               
                gridRowSet.Value = "[������ ������� ���]";
            }
            else // ������ �����
            {
                 currentPeriod.Value = periodDigest.GetMemberUniqueName(ComboYear.SelectedValue);
                 currentDate = CRHelper.DateByPeriodMemberUName(currentPeriod.Value, 3);
                 prevDate = currentDate.AddMonths(-1);
                 previousPeriod.Value = CRHelper.PeriodMemberUName("[������__��� ������� �����].[������__��� ������� �����]", prevDate, 4);
                 gridRowSet.Value =  "[������ ������� �����] ";
            }
            
            Page.Title = "���������� �������� ������������� ��� � ����-����";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("������ ���������� � ������������ ����������� �������� ������������� ��� � ����-���� �� ��������� �� {0}", ComboYear.SelectedValue.ToLowerFirstSymbol());
            
            GridDataBind();

        }

        #region ����������� �����
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("STAT_0030_0001_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "������������ �����������", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                if (gridDt.Columns.Count > 0)
                {
                    gridDt.Columns.RemoveAt(0);
                }

                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
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

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            
            int columnCount = e.Layout.Bands[0].Columns.Count;

            for (int i = 2; i < columnCount; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = true;
            }

            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(100);
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(100);

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("�����");
            headerLayout.AddCell("������ ������, �����, �����");
            AddPeriodGroup("�����");
            AddPeriodGroup("�������� ����");
            headerLayout.AddCell("��������, �������� �������� ����� � �����������");
 
            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }

        private void AddPeriodGroup(string groupName)
        {
            string hint = groupName.Contains("�����") ? "�������� ������������� ������ �������������� ����" : "";
            GridHeaderCell groupCell = GridBrick.GridHeaderLayout.AddCell(groupName, hint);
            groupCell.AddCell("�������� ������");
            groupCell.AddCell("���������� ������");
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool currentUkuzvColumn = i == 2;

                if (currentUkuzvColumn && e.Row.Cells[i].Value != null)
                {
                    string[] valueParts = e.Row.Cells[i].Value.ToString().Split(';');
                    if (valueParts.Length > 2)
                    {
                        string value = GetDoubleValue(valueParts[0], "N2");
                        string increase = GetDoubleValue(valueParts[1], "N2");
                        string rateGrow = GetDoubleValue(valueParts[2], "P2");
                        CRHelper.SaveToErrorLog(increase);
                        
                        if (increase != String.Empty)
                        {
                           if (Convert.ToDouble(increase) > 0)
                          {
                            e.Row.Cells[i].Style.BackgroundImage = string.Format("~/images/arrowRedUpBB.png");
                          }
                          else if (Convert.ToDouble(increase) < 0)
                          {
                            e.Row.Cells[i].Style.BackgroundImage = string.Format("~/images/arrowGreenDownBB.png");
                          }
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        e.Row.Cells[i].Value = String.Format("{0}<br/>{1}<br/>{2}", value, increase, rateGrow);
                        if (value!= string.Empty && increase != string.Empty && rateGrow!= string.Empty)
                        {
                            //e.Row.Cells[i].Title = string.Format("�����                                                                                                                                             ���������� �� ����������� �������                                                                               ���� ����� � ����������� �������");
                            e.Row.Cells[i].Title = string.Format("�����, ���������� �� ����������� �������, ���� ����� � ����������� �������");
                        }
                        else if (increase == string.Empty && rateGrow == string.Empty)
                        {
                            e.Row.Cells[i].Title = string.Format("����� (�������� ������������� ������ �������������� ����)");
                        }
                    }
                }

                if (i == 4 || i == 5)
                {
                    string strQuality = string.Empty;
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        switch (e.Row.Cells[i].Value.ToString())
                        {
                            case "������� ������":
                                {
                                    strQuality = "ballGreenBB.png";
                                    break;
                                }
                            case "����� ������������":
                                {
                                    strQuality = "ballGreenBB.png";
                                    break;
                                }

                            case "������������":
                                {
                                    strQuality = "ballYellowBB.png";
                                    break;
                                }
                            case "������ ������������":
                                {
                                    strQuality = "ballYellowBB.png";
                                    break;
                                }
                            case "����� ������������":
                                {
                                    strQuality = "ballOrangeBB.png";
                                    break;
                                }
                            case "�������":
                                {
                                    strQuality = "ballOrangeBB.png";
                                    break;
                                }
                            case "����� �������":
                                {
                                    strQuality = "ballRedBB.png";
                                    break;
                                }
                            case "������������ �������":
                                {
                                    strQuality = "ballRedBB.png";
                                    break;
                                }
                        }


                        e.Row.Cells[i].Style.BackgroundImage = string.Format("~/images/{0}", strQuality);
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        
                    }
                }
            }
        }

        private string GetDoubleValue(string strValue, string format)
        {
            decimal value;
            if (Decimal.TryParse(strValue, out value))
            {
                return Convert.ToDouble(value).ToString(format);
            }
            return String.Empty;
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

            for (int i = 0; i < GridBrick.Grid.Rows.Count; i++)
            {
                if (GridBrick.Grid.Rows[i].Cells[2].Value.ToString() != string.Empty && GridBrick.Grid.Rows[i].Cells[2].Value != null)
                {
                    GridBrick.Grid.Rows[i].Cells[2].Value = GridBrick.Grid.Rows[i].Cells[2].Value.ToString().Replace("<br/>", "                                          ");
                }
            }

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

           for (int i = 0; i < GridBrick.Grid.Rows.Count; i++ )
            {
                if (GridBrick.Grid.Rows[i].Cells[2].Value.ToString() !=string.Empty && GridBrick.Grid.Rows[i].Cells[2].Value != null)
                {
                    GridBrick.Grid.Rows[i].Cells[2].Value = GridBrick.Grid.Rows[i].Cells[2].Value.ToString().Replace("<br/>", "                             ");
                }
            }
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);
        }

        #endregion
    }
}