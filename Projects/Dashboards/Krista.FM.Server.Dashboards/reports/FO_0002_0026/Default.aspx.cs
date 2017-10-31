using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0026
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2006;
        private int endYear = 2011;
        private int currentYear;
        private int currentMonth;
        private GridHeaderLayout headerLayout;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.3);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0026_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 170;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month, true);
            }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue);
            currentMonth = ComboMonth.SelectedIndex + 1;

            DateTime reportDate = new DateTime(currentYear, currentMonth, 1);

            UserParams.PeriodYear.Value = currentYear.ToString();
            UserParams.PeriodLastYear.Value = (currentYear - 1).ToString();
            UserParams.PeriodFirstYear.Value = (currentYear - 2).ToString();
            UserParams.PeriodEndYear.Value = (currentYear - 3).ToString();

            PageTitle.Text = "Отдельные показатели исполнения консолидированного бюджета";
            Page.Title = PageTitle.Text;
            PageSubTitle.Text = String.Format("за {0}-{1} годы (по состоянию на {2:dd.MM.yyyy} г.)", UserParams.PeriodEndYear.Value, currentYear, reportDate.AddMonths(1));

            UserParams.PeriodMonth.Value = CRHelper.PeriodMemberUName("", reportDate, 4);

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0026_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid);

            dtGrid.Columns.Add(new DataColumn("План", typeof (Double)));
            dtGrid.Columns.Add(new DataColumn("Факт", typeof(Double)));

            query = DataProvider.GetQueryText("FO_0002_0026_grid2");
            DataTable dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid2);
            dtGrid2.PrimaryKey = new DataColumn[] { dtGrid2.Columns[0] };

           // RecalculateShValues(dtGrid2);

            for (int i = 0; i < dtGrid.Rows.Count; i++ )
            {
                string[] rowName = new string[] { dtGrid.Rows[i][0].ToString() };
                DataRow row = dtGrid2.Rows.Find(rowName);
                if (row != null)
                {
                    dtGrid.Rows[i]["План"] = row[1];
                    dtGrid.Rows[i]["Факт"] = row[2];
                }
            }

            query = DataProvider.GetQueryText("FO_0002_0026_grid3");
            DataTable dtGrid3 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid3);
            dtGrid3.PrimaryKey = new DataColumn[] { dtGrid3.Columns[0] };

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                string[] rowName = new string[] { dtGrid.Rows[i][0].ToString() };
                DataRow row = dtGrid3.Rows.Find(rowName);
                if (row != null)
                {
                    dtGrid.Rows[i]["План"] = row[1];
                    dtGrid.Rows[i]["Факт"] = row[2];
                }
            }

            query = DataProvider.GetQueryText("FO_0002_0026_grid5");
            DataTable dtGrid5 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid5);
            dtGrid5.PrimaryKey = new DataColumn[] { dtGrid5.Columns[0] };

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                string[] rowName = new string[] { dtGrid.Rows[i][0].ToString() };
                DataRow row = dtGrid5.Rows.Find(rowName);
                
                if (row != null)
                {
                    CRHelper.SaveToErrorLog(rowName.ToString());
                    dtGrid.Rows[i]["План"] = row[1];
                    dtGrid.Rows[i]["Факт"] = row[2];
                }
            }

            query = DataProvider.GetQueryText("FO_0002_0026_grid4");
            DataTable dtGrid4 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid4);
            dtGrid4.PrimaryKey = new DataColumn[] { dtGrid4.Columns[0] };

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                string[] rowName = new string[] { dtGrid.Rows[i][0].ToString() };
                DataRow row = dtGrid4.Rows.Find(rowName);
                if (row != null)
                {
                    dtGrid.Rows[i]["План"] = row[1];
                    dtGrid.Rows[i]["Факт"] = row[2];
                }
            }

            query = DataProvider.GetQueryText("FO_0002_0026_grid6");
            DataTable dtGrid6 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid6);
            dtGrid6.PrimaryKey = new DataColumn[] { dtGrid6.Columns[0] };

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                string[] rowName = new string[] { dtGrid.Rows[i][0].ToString() };
                DataRow row = dtGrid6.Rows.Find(rowName);
                if (row != null)
                {
                    dtGrid.Rows[i]["План"] = row[1];
                    dtGrid.Rows[i]["Факт"] = row[2];
                }
            }

            query = DataProvider.GetQueryText("FO_0002_0026_grid7");
            DataTable dtGrid7 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid7);
            dtGrid7.PrimaryKey = new DataColumn[] { dtGrid7.Columns[0] };

            RecalculateHValues(dtGrid7);

            for (int i = 0; i < dtGrid.Rows.Count; i++)
            {
                string[] rowName = new string[] { dtGrid.Rows[i][0].ToString() };
                DataRow row = dtGrid7.Rows.Find(rowName);
                if (row != null)
                {
                    dtGrid.Rows[i]["План"] = row[1];
                    dtGrid.Rows[i]["Факт"] = row[2];
                }
            }
            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
        }

      /*  private static void RecalculateShValues(DataTable dtGrid2)
        {
            string query;
            query = DataProvider.GetQueryText("FO_0002_0026_sh");
            DataTable dtSh = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtSh);

            if (dtSh.Rows.Count > 0)
            {
                double shValue;
                
                if (double.TryParse(dtSh.Rows[0][1].ToString(), out shValue) && shValue != 0)
                {
                    string[] rowNameSh = new string[] { dtSh.Columns[1].ColumnName.ToString() };
                    DataRow rowSh = dtGrid2.Rows.Find(rowNameSh);
                    if (rowSh != null)
                    {
                        if (rowSh[1] != DBNull.Value)
                        {
                            double value;
                            if (double.TryParse(rowSh[1].ToString(), out value))
                            {
                                rowSh[1] = value / shValue;
                            }
                        }
                        if (rowSh[2] != DBNull.Value)
                        {
                            double value;
                            if (double.TryParse(rowSh[2].ToString(), out value))
                            {
                                rowSh[2] = value / shValue;
                            }
                        }
                    }
                }
            }
        }
        */
        private static void RecalculateHValues(DataTable dtGrid7)
        {
           
            string query = DataProvider.GetQueryText("FO_0002_0026_h");
            DataTable dth = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dth);

            if (dth.Rows.Count > 0)
            {
                double HValue;
              
                if (double.TryParse(dth.Rows[0][1].ToString(), out HValue) && HValue != 0)
                {
                  
                    string[] rowNameH = new string[] { dth.Columns[1].ColumnName.ToString() };
                   
                    DataRow rowSh = dtGrid7.Rows.Find(rowNameH);
                
                    if (rowSh != null)
                    {
                        if (rowSh[1] != DBNull.Value)
                        {
                            double value;
                            if (double.TryParse(rowSh[1].ToString(), out value))
                            {
                               
                                rowSh[1] = value / HValue;
                            }
                        }
                        if (rowSh[2] != DBNull.Value)
                        {
                            double value;
                            if (double.TryParse(rowSh[2].ToString(), out value))
                            {
                               
                                rowSh[2] = value / HValue;
                            }
                        }
                    }
                }
            }
        }
        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                int widthColumn = CRHelper.GetColumnWidth(110);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
            }

            UltraGridColumn col = e.Layout.Bands[0].Columns[1];
            e.Layout.Bands[0].Columns.RemoveAt(1);
            e.Layout.Bands[0].Columns.Insert(0, col);

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(40);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(330);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;

            headerLayout.AddCell("Пункт НПА");
            headerLayout.AddCell("Показатели");
            headerLayout.AddCell("Ед.изм");

            GridHeaderCell headerCell = headerLayout.AddCell("Исполнено за год");
            headerCell.AddCell((currentYear - 3) + " год");
            headerCell.AddCell((currentYear - 2) + " год");
            headerCell.AddCell((currentYear - 1) + " год");

            headerCell = headerLayout.AddCell("План на год");
            headerCell.AddCell(currentYear + " год");

            headerCell = headerLayout.AddCell("Исполнено за период");
            headerCell.AddCell(currentYear + " год");

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Index == 0 || e.Row.Index == 1)
            {
                e.Row.Hidden = true;
                return;
            }
            if (e.Row.Cells[2].Value != null && (e.Row.Cells[2].Value.ToString().ToLower() == "тыс руб" || e.Row.Cells[2].Value.ToString().ToLower() == "руб"))
            {
                for (int i = 3; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Value != null)
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N2}", e.Row.Cells[i].Value);
                    }
                }
                e.Row.Cells[2].Value = e.Row.Cells[2].Value.ToString().ToLower();
            }
            else if (e.Row.Cells[2].Value != null && e.Row.Cells[2].Value.ToString().ToLower() == "проц")
            {
                for (int i = 3; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Value != null)
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N2}", e.Row.Cells[i].Value);
                    }
                }
                e.Row.Cells[2].Value = e.Row.Cells[2].Value.ToString().ToLower();
            }

            for (int i = 3; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[i].Style.Padding.Right = 10;
            }

            e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Center;
            e.Row.Cells[2].Style.HorizontalAlign = HorizontalAlign.Center;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            ReportPDFExporter1.Export(headerLayout, section1);
        }

        #endregion
    }
}