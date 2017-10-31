using System;
using System.Collections.ObjectModel;
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
using Krista.FM.ServerLibrary;
using System.Web.SessionState;
using System.Web;
using Krista.FM.Common;
using System.Runtime.Remoting.Messaging;


namespace Krista.FM.Server.Dashboards.reports.FO_0002_0058
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2009;
        private int endYear;
        private string endMonth;
        private int badRank;
        private int rubMulti = 1000;
        private GridHeaderLayout headerLayout;
        private static MemberAttributesDigest levelDigest;
        private string multiplierCaption;
        private DateTime date;
        private bool sumRowFlag = false;

        private CustomParam periodYear;
        private CustomParam periodMonth;
        private CustomParam periodHalfYear;
        private CustomParam periodQuater;
        private CustomParam periodPrevYear;
        private CustomParam levelBudget;
        private CustomParam regionChart;
        //private CustomParam chartKateg;
        private CustomParam currentRegion;
        private CustomParam countItemLegend;
        
        // ��������� ��������� ������
        private CustomParam rubMultiplier;

        private bool IsNotEmptyYears()
        {
            return ComboYear.SelectedNodes.Count > 0;
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 330);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 45);
            UltraWebGrid.DisplayLayout.NoDataMessage = "��� ������";

            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            periodYear = UserParams.CustomParam("period_year");
            periodMonth = UserParams.CustomParam("period_month");
            periodHalfYear = UserParams.CustomParam("period_halfyear");
            periodQuater = UserParams.CustomParam("period_quater");
            periodPrevYear = UserParams.CustomParam("period_prev_year");
            levelBudget = UserParams.CustomParam("level_budget");
            rubMultiplier = UserParams.CustomParam("rubMultiplier");
            regionChart = UserParams.CustomParam("region_chart");
            //chartKateg = UserParams.CustomParam("chart_kateg");
            currentRegion = UserParams.CustomParam("current_Region");
            countItemLegend = UserParams.CustomParam("count_item");
        }


        private void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            int countBox = 0;
            int countText = 0;
            int offsetBox = 0;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
    
                if (primitive is Text)
                {
                    Text text = (Text)primitive;
                    string strText = text.GetTextString();
                    if (!strText.Contains("%"))
                    {
                        text.bounds.Height = 50;
                        text.labelStyle.WrapText = true;
                        text.bounds.Offset(0, countText * 20);
                        countText++;                    
                    }
                }

                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    countBox++;
                    if (countBox >= (Convert.ToInt32(countItemLegend.Value) + 2))
                    {
                        box.rect.Offset(0, (offsetBox * 20) + 3);
                        offsetBox++;
                    }
                }
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                //WebAsyncRefreshPanel1.AddRefreshTarget(UltraChart1);
                //WebAsyncRefreshPanel1.AddRefreshTarget(chart1Caption);
                //WebAsyncRefreshPanel1.AddLinkedRequestTrigger(UltraWebGrid);
                
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0058_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                endMonth = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "�������� ������";
                ComboYear.Width = 200;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.Set�heckedState(endYear.ToString(), true);
                
            }

            Page.Title = "���������� ����������� ������������� ������������ ������������ ������������� � �������-������������� ��������� ������������� ������� (�����������)";
            Label1.Text = Page.Title;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            date = new DateTime(yearNum, 01, 1).AddMonths(1);
            Label2.Text = String.Format("������ ��������������� ����������� �������� ����������� ������������� ������������ ������������ ������������� � �������-������������� ��������� ������������� ������� �� {0} ��� (����������� ������ � ������ ����)",
                ComboYear.SelectedValue);
            periodYear.Value = ComboYear.SelectedValue;
            periodPrevYear.Value = string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1);
                rubMulti = 1;
            rubMultiplier.Value = string.Format("{0}", rubMulti);
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

        }

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0058_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                dtGrid.Rows.RemoveAt(0);
                if (dtGrid.Rows.Count > 1)
                {
                    int numberStr;
                    dtGrid.Columns.RemoveAt(0);
                    dtGrid.AcceptChanges();
                }
                UltraWebGrid.DataSource = dtGrid;
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
            e.Layout.AllowSortingDefault = AllowSorting.Yes;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.VerticalAlign = VerticalAlign.Top;
 
            int columnCount = e.Layout.Bands[0].Columns.Count;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "0");
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(90);
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            headerLayout = new GridHeaderLayout(UltraWebGrid);
            headerLayout.AddCell("������������ �����������");
            headerLayout.AddCell("������� ���������");
            GridHeaderCell undercell = headerLayout;
            for (int i = 2; i < columnCount; i = i + 1)
            {
                switch (i % 2)
                {
                    
                    case 0:
                        {
                            switch (i)
                            {
                                case 2:
                                    {
                                        undercell = headerLayout.AddCell(String.Format("{0} ������� {1} ����", i / 2, ComboYear.SelectedValue));
                                        break;
                                    }
                                case 4:
                                    {
                                        undercell = headerLayout.AddCell(String.Format("1 ��������� {0} ����", ComboYear.SelectedValue));
                                        break;
                                    }
                                case 6:
                                    {
                                        undercell = headerLayout.AddCell(String.Format("9 ������� {0} ����", ComboYear.SelectedValue));
                                        break;
                                    }
                                case 8:
                                    {
                                        undercell = headerLayout.AddCell(String.Format("����� �� {0} ���", ComboYear.SelectedValue));
                                        break;
                                    }
                            }
                            undercell.AddCell("����");
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                            e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(90);
                            break;
                        }

                    case 1:
                        {
                            if (i == columnCount - 1)
                            {
                              undercell.AddCell("����-� ����� � ��, %");
                            }else
                            {
                                undercell.AddCell("����-� ����� � ����, %");
                            }
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P1");
                            e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(90);
                            break;
                        }
                }
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
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
            CRHelper.SaveToErrorLog(e.Row.Cells[1].ToString());
           if (e.Row.Cells[1].ToString() == "Cell")
            {
                e.Row.Cells[0].ColSpan = e.Row.Cells.Count; 
                e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Center;
                e.Row.Cells[0].Style.VerticalAlign = VerticalAlign.Middle;
                e.Row.Cells[0].Style.Font.Bold = true;
            }
           e.Row.Cells[1].Style.HorizontalAlign = HorizontalAlign.Center;
        }

        private void UltraWebGrid_DataBound(object sender, EventArgs e)
        {          
             /*   UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, currentRegion.Value, 0, 0);
                ActivateGridRow(row); */
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            ActivateGridRow(e.Row);
    
        }

        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
            {
                return;
            }
    
            string region = row.Cells[0].Text;
            string caption = string.Empty;

            if (region == "��������� ������")
            {
                caption = string.Format("��������� ���������������� ����� �� {0:dd.MM.yyyy} ����, ������������� �������", date);
                regionChart.Value = "[������������� ������� (������ ��������)]";
                //currentRegion.Value = region;
            }
            else
            {
                if (region == "������� �������")
                {
                    caption = string.Format("��������� �������������� ����� �� {0:dd.MM.yyyy} ����", date);
                    sumRowFlag = true;
                    //currentRegion.Value = region;
                }
                else
                {
                    if (region.Contains("�."))
                    {
                        caption = string.Format("��������� �������������� ����� �� {0:dd.MM.yyyy} ����, {1}", date, region);
                        regionChart.Value = string.Format("[��������� ������].[{0}]", region);
                        //currentRegion.Value = region;
                    }
                    else
                    {
                        caption = string.Format("��������� �������������� ����� �� {0:dd.MM.yyyy} ����, {1}", date, region);
                        regionChart.Value = string.Format("[������������� ������].[{0}]", region);
                        //currentRegion.Value = region;
                    }
                }
            }
            currentRegion.Value = region;
            UltraWebGrid.DisplayLayout.SelectedRows.Clear();
            row.Selected = true;
            UltraWebGrid.DisplayLayout.ActiveRow = row;
        }


        #endregion

        #region ������� � Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
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