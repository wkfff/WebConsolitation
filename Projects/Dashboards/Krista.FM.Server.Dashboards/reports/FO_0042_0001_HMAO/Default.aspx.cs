using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0042_0001_HMAO
{
    public partial class Default : CustomReportPage
    {
        #region ����

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtIndicatorDescription;
        private int firstYear = 2009;
        private int endYear = 2012;
        private static Dictionary<string, string> indicatorUnitList;
        private GridHeaderLayout headerLayout;

        #endregion

        public bool ValueSelected
        {
            get { return MeasureButtonList.SelectedIndex == 1; }
        }

        #region ��������� �������

        // ��������� ����
        private CustomParam selectedMeasure;
        // ��������� ������
        private CustomParam selectedPeriod;
        // ��������� �����������
        private CustomParam indicatorSet;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double scale = 0.6;
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale);
            UltraWebGrid.DisplayLayout.NoDataMessage = "��� ������";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            #region ������������� ���������� �������

            selectedMeasure = UserParams.CustomParam("selected_maasure");
            selectedPeriod = UserParams.CustomParam("selected_period");
            indicatorSet = UserParams.CustomParam("indicator_set");

            #endregion

            CrossLink1.Visible = true;
            CrossLink1.Text = "�������&nbsp;����";
            CrossLink1.NavigateUrl = "~/reports/FO_0042_0002_HMAO/Default.aspx";

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
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0042_0001_HMAO_date");
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
            }

            int quarterNum = ComboQuarter.SelectedIndex + 1;

            Page.Title = String.Format("���������� ����������� �������� ����������� �����������, ��������������� �������� ��������������� ������� ������� ����������� ������, �������� ���������������� ������� ������� ����-����");
            PageTitle.Text = Page.Title;

            PageSubTitle.Text = (quarterNum != 4)
                ? String.Format("{0}.{1}", ComboQuarter.SelectedValue, ComboYear.SelectedValue)
                : String.Format("�� ������ {0} ����", ComboYear.SelectedValue);
            
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("��������� {0}", CRHelper.HalfYearNumByQuarterNum(quarterNum));
            UserParams.PeriodQuater.Value = String.Format("������� {0}", quarterNum);

            selectedPeriod.Value = String.Format("[{0}].[{1}].[{2}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value);
            selectedMeasure.Value = ValueSelected ? "��������" : "������ ���������� ";
            indicatorSet.Value = ValueSelected ? "���������� ��� ��������" : "���������� ��� ������";
            
            IndicatorDescriptionDataBind();

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();     
            UltraWebGrid.DataBind();
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

        #region ����������� �����

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0001_HMAO_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������ ��������������", dtGrid);
            if (dtGrid.Rows.Count > 0 && dtGrid.Columns.Count > 2)
            {
                dtGrid.Columns.RemoveAt(0);

                DataTable sortingDT = dtGrid.Clone();
                DataRow[] sortingRows = dtGrid.Select("", "��� ���� ASC");
                foreach (DataRow row in sortingRows)
                {
                    sortingDT.ImportRow(row);
                }
                sortingDT.Columns.Remove("��� ����");
                sortingDT.AcceptChanges();
                UltraWebGrid.DataSource = sortingDT;
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
            e.Layout.AllowRowNumberingDefault = RowNumbering.None;

            if (e.Layout.Bands[0].Columns.Count < 2)
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

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;

            headerLayout.AddCell("� �/�");
            headerLayout.AddCell("������������ ����");

            int beginIndex = 2;
            for (int i = beginIndex; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N2";
                int widthColumn = 250;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;

                GridHeaderCell cell = headerLayout.AddCell(e.Layout.Bands[0].Columns[i].Header.Caption);
                if (cell.Caption != "�������� ������")
                {
                    GridHeaderCell unitCell = cell.AddCell(ValueSelected ? "��������" : "������");
                    if (indicatorUnitList.ContainsKey(cell.Caption))
                    {
                        unitCell.Hint = ValueSelected ? indicatorUnitList[cell.Caption] : "�����";
                    }
                }
                else
                {
                    cell.Hint = "�����";
                }
            }

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
                        if (value == -100500)
                        {
                            cell.Value = "*";
                            cell.Style.ForeColor = Color.Black;
                            cell.Style.Font.Size = 12;
                        }
                    }
                }
            }
        }

        private void IndicatorDescriptionDataBind()
        {
            indicatorUnitList = new Dictionary<string, string>();

            string query = DataProvider.GetQueryText("FO_0042_0001_HMAO_indicatorDescription");
            dtIndicatorDescription = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "������������", dtIndicatorDescription);

            foreach (DataRow row in dtIndicatorDescription.Rows)
            {
                if (row[0] != DBNull.Value && row[1] != DBNull.Value)
                {
                    string name = row[0].ToString();
                    string unit = row[1].ToString();

                    indicatorUnitList.Add(name, unit);
                }
            }
        }

        #endregion

        #region ������� � PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout);
        }

        #endregion

        #region ������� � Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            ReportExcelExporter1.Export(headerLayout, 3);
        }

        #endregion
    }
}
