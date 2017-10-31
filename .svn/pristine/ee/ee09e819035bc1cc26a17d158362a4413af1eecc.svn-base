using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Krista.FM.Server.Dashboards.Core;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Infragistics.UltraChart.Core.Primitives;


namespace Krista.FM.Server.Dashboards.reports.FO_0035_0011
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private int firstYear = 2009;
        private int endYear;
        private string month;
        private DateTime data;
        private GridHeaderLayout headerLayout;
        private CustomParam KBK;
        private CustomParam dim;
        private CustomParam correct;

        #endregion


        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 4);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";


            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            #region инициализация параметров
            if (KBK == null)
            {
                KBK = UserParams.CustomParam("KBK");
            }
            if (dim == null)
            {
                dim = UserParams.CustomParam("dim");
            }
            if (correct == null)
            {
                correct = UserParams.CustomParam("correct");
            }
            #endregion


        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {

                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0035_0011_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                DateTime date = new DateTime(
                       Convert.ToInt32(dtDate.Rows[0][0].ToString()),
                       CRHelper.MonthNum(dtDate.Rows[0][3].ToString()),
                       Convert.ToInt32(dtDate.Rows[0][4].ToString()));

                // Инициализируем календарь
                ComboYear.PanelHeaderTitle = "Выберите период";
                ComboYear.Title = "Выберите период";
                ComboYear.Width = 290;
                ComboYear.ParentSelect = false;
                ComboYear.ShowSelectedValue = true;
                ComboYear.MultiSelect = false;
                FillComboDate(ComboYear, "FO_0035_0011_list_of_dates", 0);
             
                
            }
            string periodUniqueName = string.Empty;
            switch (ComboYear.SelectedNode.Level)
            {
                case 0:
                    {
                        periodUniqueName = StringToMDXDate(ComboYear.GetLastChild(ComboYear.SelectedNode).FirstNode.Text);
                        break;
                    }
                case 1:
                    {
                        periodUniqueName = StringToMDXDate(ComboYear.SelectedNode.FirstNode.Text);
                        break;
                    }
                case 2:
                    {
                        periodUniqueName = StringToMDXDate(ComboYear.SelectedNode.Text);
                        break;
                    }
            }
            DateTime currentDate = CRHelper.PeriodDayFoDate(periodUniqueName);
            PageSubTitle.Text = string.Format("Данные на {0:dd.MM.yyyy} года, руб.", currentDate);
            Page.Title = "Остатки средств межбюджетных трансфертов";
            PageTitle.Text = Page.Title;
            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UserParams.PeriodCurrentDate.Value = periodUniqueName;
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
        }

        #region Обработчик грида
        int[] levels;
        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {

            string query = DataProvider.GetQueryText("FO_0035_0011_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query,
            "Муниципальные образования",
            dtGrid);
             dtGrid.AcceptChanges();
                UltraWebGrid1.DataSource = dtGrid;
           

        }


        protected void UltraWebGrid_InitializeLayout(Object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            headerLayout.AddCell("Муниципальные образования");
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            GridHeaderCell cell;
            GridHeaderCell lowcell = null;
            GridHeaderCell lowlowcell = null;
            cell = headerLayout;
            lowcell = cell.AddCell("Главные распорядители областного бюджета Новосибирской области");
            for (int colNum = 1; colNum < e.Layout.Bands[0].Columns.Count; colNum++)
            {
                string caption = e.Layout.Bands[0].Columns[colNum].Header.Caption;
                if (colNum < e.Layout.Bands[0].Columns.Count - 1)
                {
                    lowcell.AddCell(caption);
                }
                else
                {
                    cell.AddCell(caption);
                }
            }
            headerLayout.ApplyHeaderInfo();
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(160);
            }

        }

        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[0].Value != null &&
                (e.Row.Cells[0].Value.ToString().Contains("Итого")))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
                
            }
        }

        protected void FillComboDate(CustomMultiCombo combo, string queryName, int offset)
        {
            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText(queryName);
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            if (dtDate.Rows.Count == 0)
            {
                throw new Exception("Данные для построения отчета отсутствуют в кубе");
            }
            Dictionary<string, int> dictDate = new Dictionary<string, int>();
            for (int row = 0; row < dtDate.Rows.Count - offset; ++row)
            {
                string year = dtDate.Rows[row][0].ToString();
                string month = dtDate.Rows[row][3].ToString();
                string day = dtDate.Rows[row][4].ToString();
                AddPairToDictionary(dictDate, year + " год", 0);
                AddPairToDictionary(dictDate, month + " " + year + " года", 1);
                AddPairToDictionary(dictDate, day + " " + CRHelper.RusMonthGenitive(CRHelper.MonthNum(month)) + ' ' + year + " года", 2);
            }
            combo.FillDictionaryValues(dictDate);
            combo.SelectLastNode();
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
            string template = "[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}].[{4}]";
            string[] dateElements = str.Split(' ');
            int year = Convert.ToInt32(dateElements[2]);
            string month = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(CRHelper.MonthNum(dateElements[1])));
            int quarter = CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month));
            int halfYear = CRHelper.HalfYearNumByQuarterNum(quarter);
            int day = Convert.ToInt32(dateElements[0]);
            return String.Format(template, year, halfYear, quarter, month, day);
        }


        #endregion


        #region экспорт


        #region экспорт в Excel


        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");

            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в Pdf


        private void PdfExportButton_Click(Object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            ReportPDFExporter1.HeaderCellHeight = 105;
            ReportPDFExporter1.Export(headerLayout, section1);


        }

        #endregion

        #endregion


    }
}