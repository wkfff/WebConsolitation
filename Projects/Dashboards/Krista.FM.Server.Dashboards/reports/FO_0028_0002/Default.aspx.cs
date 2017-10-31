using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Infragistics.WebUI.UltraWebNavigator;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core.MemberDigests;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using Krista.FM.Server.Dashboards.Components;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Krista.FM.ServerLibrary;
using System.Web.SessionState;
using System.Web;
using Krista.FM.Common;
using System.Runtime.Remoting.Messaging;



namespace Krista.FM.Server.Dashboards.reports.FO_0028_0002
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2006;
        private int endYear = 2011;
        private GridHeaderLayout headerLayout;
        private int currentYear;
        private static MemberAttributesDigest orgDigest;
        private static MemberAttributesDigest dogDigest;

        private DateTime date;

        private CustomParam orgType;
        private CustomParam dogType;
        private CustomParam rekvGrid;
        private CustomParam orgGrid;
        private CustomParam yearGrid;
        private CustomParam gridContent;

        private bool IsNotEmptyYears()
        {
            return ComboYear.SelectedNodes.Count > 0;
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 235);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.DisplayLayout.NoDataMessage = "��� ������";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink1.Visible = true;
            CrossLink1.Text = "�������&nbsp;��������&nbsp;��";
            CrossLink1.NavigateUrl = "~/reports/FO_0028_0001/Default.aspx";


            yearGrid = UserParams.CustomParam("year_grid");
            orgType = UserParams.CustomParam("org_type");
            dogType = UserParams.CustomParam("dog_type");
            rekvGrid = UserParams.CustomParam("grid_rekv");
            orgGrid = UserParams.CustomParam("grid_org");
            gridContent = UserParams.CustomParam("grid_content");
           
            
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            IDatabase db = GetDataBase();

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                endYear = DateTime.Now.Year;

                ComboYear.Title = "���";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = true;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(endYear.ToString(), true);
                ComboYear.Set�heckedState((endYear - 1).ToString(), true);

                ComboOrg.Width = 500;
                ComboOrg.Title = "�����������";
                ComboOrg.MultiSelect = true;
                ComboOrg.ParentSelect = true;
                orgDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0028_0002_organiz");
                ComboOrg.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(orgDigest.UniqueNames, orgDigest.MemberLevels));
                ComboOrg.Set�heckedState("��� �����������", true);

                ComboDog.Width = 400;
                ComboDog.Title = "��������";
                ComboDog.MultiSelect = true;
                ComboDog.ParentSelect = true;
                dogDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0028_0002_dogovor");
                ComboDog.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(dogDigest.UniqueNames, dogDigest.MemberLevels));
                ComboDog.Set�heckedState("��� ��������", true);
            }

            string dateString = String.Empty;
            if (ComboYear.SelectedValues.Contains(DateTime.Now.Year.ToString()))
            {
                string queryDateText = "SELECT MAX([PumpDate]) FROM [novosibirsk].[DV].[PumpHistory] where ProgramIdentifier='FO28Pump'";
                try
                {
                    DataTable dtDate = (DataTable)db.ExecQuery(queryDateText, QueryResultTypes.DataTable);
                    date = (DateTime)(dtDate.Rows[0][0]);
                    dateString = String.Format(" �� ��������� �� {0:dd.MM.yyyy}", date);
                }
                finally
                {
                    db.Dispose();
                }
            }

            orgType.Value = orgDigest.GetMemberUniqueName(ComboOrg.SelectedValue);
            dogType.Value = dogDigest.GetMemberUniqueName(ComboDog.SelectedValue);

            Page.Title = "�������� ��������� ������������� �� ��������� ��������, �������� �� ���������� �������";
            Label1.Text = Page.Title;
            Label2.Text = String.Empty;

            Collection<string> selectedYears = ComboYear.SelectedValues;
            if (selectedYears.Count > 0)
            {

                Label2.Text = String.Format("������ �� {0} {1}{2}, ���.���.",
                                            CRHelper.GetDigitIntervals(ComboYear.SelectedValuesString, ','),
                                            ComboYear.SelectedValues.Count == 1 ? "���" : "����", dateString);
                string years = String.Empty;
                string gridCont = String.Empty;

                for (int i = selectedYears.Count - 1; i >= 0; i--)
                {
                    string currYear = selectedYears[i];
                    string year = string.Format(
                                "[������__������].[������__������].[������ ���� ��������].[{0}]",
                                selectedYears[i]);
                    years += string.Format(
                                "[������__������].[������__������].[������ ���� ��������].[{0}], ",
                                selectedYears[i]);
   
                    string prevYear = string.Format(
                                "[������__������].[������__������].[������ ���� ��������].[{0}]",
                                Convert.ToInt32(selectedYears[i]) - 1);
                    gridCont += "{" + "{" + year + "}" + "*" +
                        "{[Measures].[�������������_�� ����� ����_],[Measures].[�������������_�� ������_]}" + "}" +
                        " + " + "{" + "{" + prevYear + "}" + "*" + "{[Measures].[�������������_�� ����� ����_]}" + "}" + "+";
                }

                gridCont = gridCont.Remove(gridCont.Length - 1, 1);
                gridContent.Value = string.Format("{0}", gridCont);

                years = years.Remove(years.Length - 2, 1);
                yearGrid.Value = string.Format("{0}", years);
            }
            else
            {
                yearGrid.Value = "{}";
                gridContent.Value = "{}";

            }
            
            Collection<string> selectedValues = ComboDog.SelectedValues;
            if (selectedValues.Count > 0)
            {
                string rekv = String.Empty;
                if ((selectedValues[0] == "��� ��������") && (selectedValues.Count == 1))
                {
                    rekvGrid.Value = "[���������]";

                }
                else
                {
                    for (int i = 0; i < selectedValues.Count; i++)
                    {
                        string rekviz = selectedValues[i];
                        rekv +=
                            string.Format(
                                "[�������__������� ���������������].[�������__������� ���������������].[���].[{0}], ",
                                rekviz);

                    }
                    rekv = rekv.Remove(rekv.Length - 2, 1);
                    rekvGrid.Value = string.Format("{0}", rekv);
                }
            }
            else
            {
                rekvGrid.Value = "{}";
            }

            Collection<string> selectedOrg = ComboOrg.SelectedValues;
            if (ComboOrg.SelectedNodes.Count > 0)
            {
                bool delLastSymbol = false;
                string org = String.Empty;
                if (selectedOrg[0] == "��� �����������")
                {
                    orgGrid.Value = "[��� �����������]";

                }
                else
                {
                    for (int i = 0; i < ComboOrg.SelectedNodes.Count; i++)
                    {
                        Node node = new Node();
                        node = ComboOrg.SelectedNodes[i];
                        if (node.Text == "������������� �����������")
                        {
                            org += "[�����������__������������].[�����������__������������].[���].[������������� �����������].Children, ";
                        }
                        else
                        {
                            if (node.Text == "����������� ����")
                            {
                                org += "[�����������__������������].[�����������__������������].[���].[����������� ����].Children, ";
                            }
                            else
                            {
                                if (node.Text == "����������� ���� (����������� ��������)")
                                {
                                    org += "[�����������__������������].[�����������__������������].[���].[����������� ���� (����������� ��������)].Children, ";
                                }
                                else
                                {
                                    delLastSymbol = true;
                                    if (node.Parent.Text == "������������� �����������")
                                    {
                                        org +=
                                            string.Format(
                                                "[�����������__������������].[�����������__������������].[���].[������������� �����������].[{0}], ",
                                                node.Text);
                                    }
                                    else
                                    {
                                        if (node.Parent.Text == "����������� ����")
                                        {
                                            org +=
                                            string.Format(
                                                "[�����������__������������].[�����������__������������].[���].[����������� ����].[{0}], ",
                                                node.Text);
                                        }
                                        else
                                        {
                                            if (node.Parent.Text == "����������� ���� (����������� ��������)")
                                            {
                                                org +=
                                                string.Format(
                                                    "[�����������__������������].[�����������__������������].[���].[����������� ���� (����������� ��������)].[{0}], ",
                                                    node.Text);
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                    
                    org = org.Remove(org.Length - 2, 1);
                   
                    orgGrid.Value = string.Format("{0}", org);
                }
            }
            else
            {
                orgGrid.Value = "{}";
            }
            

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

            
        }

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            if ((ComboYear.SelectedNodes.Count > 0) && (ComboDog.SelectedNodes.Count > 0))
            {
                string query = DataProvider.GetQueryText("FO_0028_0002_grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "�����������", dtGrid);
                if (dtGrid.Rows.Count > 0)
                {
                    dtGrid.Columns.RemoveAt(0);
                    bool delete = true;
                    int i = 0;
                    while (i < dtGrid.Rows.Count)
                    {
                        for (int j = 2; j < dtGrid.Columns.Count; j++)
                        {
                            if (dtGrid.Rows[i][j].ToString() != string.Empty)
                            {
                                delete = false;
                            }     
                        }
                        if (delete)
                        {
                            dtGrid.Rows.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                            delete = true;
                        }
                    }
                    if (dtGrid.Rows.Count > 0)
                    {
                        UltraWebGrid.DataSource = GetNonEmptyDt(dtGrid);
                    }
                }
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
            
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

           

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.VerticalAlign = VerticalAlign.Top;
            e.Layout.Bands[0].Columns[0].MergeCells = true;
   

            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
 

            int columnCount = e.Layout.Bands[0].Columns.Count;
            for (int i = 2; i < columnCount; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            headerLayout.AddCell("�����������");
            headerLayout.AddCell("��������� ��������");  

            Collection<string> selectedYears = ComboYear.SelectedValues;
            if (selectedYears.Count > 0)
            {  
                    
                    int year = Convert.ToInt32(selectedYears[selectedYears.Count - 1]);
                    if (year == DateTime.Now.Year)
                    {
                        headerLayout.AddCell(String.Format("������������� �� {0:dd.MM.yyyy}", date),
                            String.Format("������������� �� {0:dd.MM.yyyy} �� ��������������� ��������", date));
                    }    
                    else
                    {
                        headerLayout.AddCell(String.Format("������������� �� 01.01.{0}", year + 1),
                        "������������� �� ������ ���� �� ��������������� ��������");
                    }    
                    int prevYear = year;

                    headerLayout.AddCell(String.Format("��������� ������������� �� {0} ���", year), "��������� ������������� �� ���");
                    headerLayout.AddCell(String.Format("������������� �� 01.01.{0}", year), "������������� �� ������ ���� �� ��������������� ��������");
                    if (selectedYears.Count > 1)
                    {
                        for (int i = selectedYears.Count - 2; i >= 0; i--)
                        {
                            year = Convert.ToInt32(selectedYears[i]);
                            if (year == (prevYear - 1))
                            {
                                headerLayout.AddCell(String.Format("��������� ������������� �� {0} ���", year), "��������� ������������� �� ���");
                                headerLayout.AddCell(String.Format("������������� �� 01.01.{0}", year), "������������� �� ������ ���� �� ��������������� ��������");
                            }
                            else
                            {
                                headerLayout.AddCell(String.Format("������������� �� 01.01.{0}", year + 1), "������������� �� ������ ���� �� ��������������� ��������");
                                headerLayout.AddCell(String.Format("��������� ������������� �� {0} ���", year), "��������� ������������� �� ���");
                                headerLayout.AddCell(String.Format("������������� �� 01.01.{0}", year), "������������� �� ������ ���� �� ��������������� ��������");
                            }
                            prevYear = year;
                         }
                    }
            }
         
            headerLayout.ApplyHeaderInfo();
        }

        private static DataTable GetNonEmptyDt(DataTable sourceDt)
        {
            DataTable dt = sourceDt.Clone();

            foreach (DataRow row in sourceDt.Rows)
            {
                if (!IsEmptyRow(row, 2))
                {
                    dt.ImportRow(row);
                }
            }
            dt.AcceptChanges();
            return dt;
        }

        private static bool IsEmptyRow(DataRow row, int startColumnIndex)
        {
            for (int i = startColumnIndex; i < row.Table.Columns.Count; i++)
            {
                if (row[i] != DBNull.Value)
                {
                    return false;
                }
            }
            return true;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string rowName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                rowName = e.Row.Cells[0].Value.ToString();
            }

            if (rowName.ToLower() == "�����")
            {
                e.Row.Style.Font.Bold = true;
            }
            
            for (int i = 0; i < e.Row.Cells.Count; i++)
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

        private static IDatabase GetDataBase()
        {
            try
            {
                HttpSessionState sessionState = HttpContext.Current.Session;
                LogicalCallContextData cnt =
                    sessionState[ConnectionHelper.LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] as LogicalCallContextData;
                if (cnt != null)
                    LogicalCallContextData.SetContext(cnt);
                IScheme scheme = (IScheme)sessionState[ConnectionHelper.SCHEME_KEY_NAME];
                return scheme.SchemeDWH.DB;
            }
            finally
            {
                CallContext.SetData("Authorization", null);
            }
        }

        #region ������� � Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("�������");
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
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
            ReportPDFExporter1.Export(headerLayout, section1);
        }

        #endregion
    }
}