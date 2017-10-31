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
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Infragistics.UltraChart.Core.Primitives;


namespace Krista.FM.Server.Dashboards.reports.UFK_0018_0001
{
    public partial class Default : Krista.FM.Server.Dashboards.Core.CustomReportPage
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
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.5);

            tableContainer.Style.Add("width", CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 10).ToString());
            tableContainer.Style.Add("height", CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.5).ToString());

            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid1.EnableViewState = false;

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
                string query = DataProvider.GetQueryText("UFK_0018_0001_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

                if (dtDate.Rows.Count > 0)
                {
                    month = dtDate.Rows[0][3].ToString();
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                }
                ComboYear.Title = "Год";
                ComboYear.Visible = true;
                ComboYear.Width = 100;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.MultiSelect = false;
                ComboYear.SetСheckedState(endYear.ToString(), true);
                data = new DateTime(endYear, CRHelper.MonthNum(month), 1);
                data = data.AddMonths(1);
                PageSubTitle.Text = string.Format("Приводятся данные по состоянию на {0:dd.MM.yyyy} года, тыс.руб.", data);
            }
            int year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodLastYear.Value = (year - 1).ToString();

            Page.Title = "Расходы консолидированного бюджета";
            PageTitle.Text = Page.Title;
            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            KBK.Value = KBKList.SelectedValue;
            switch (KBK.Value)
            {
                case "Раздел/подраздел":
                    {
                        correct.Value = string.Empty;
                        dim.Value = "[РзПр__Сопоставимый].[РзПр__Сопоставимый]";
                        break;
                    }
                case "Вид расходов":
                    {
                        correct.Value = "--";
                        dim.Value = "[КВР__Сопоставим].[КВР__Сопоставим]";
                        break;
                    }
                case "КОСГУ":
                    {
                        correct.Value = string.Empty;
                        dim.Value = "[КОСГУ__Сопоставимый].[КОСГУ__Сопоставимый]";
                        break;
                    }
            }
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
            UltraWebGrid1.Visible = false;
        }

        #region Обработчик грида
        int[] levels;
        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("UFK_0018_0001_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Муниципальные образования",  dtGrid);
            for (int k = 0; k < dtGrid.Rows.Count - 1; k++)
            {
                DataRow row = dtGrid.Rows[k];

                for (int i = 1; i < row.ItemArray.Length; i++)
                {
                    if (row[i] != DBNull.Value)
                    {

                        row[i] = Convert.ToDouble(row[i]) / 1000;

                    }
                }
            }
            if (KBK.Value == "Вид расходов")
            {
                dtGrid.AcceptChanges();
                UltraWebGrid1.DataSource = dtGrid;
            }
            else
            {
                levels = new int[dtGrid.Columns.Count + 1];
                DataRow levelrow = dtGrid.Rows[dtGrid.Rows.Count - 1];
                for (int i = 1; i < dtGrid.Columns.Count; i++)
                {
                    if (levelrow[i] != DBNull.Value)
                    {
                        levels[i] = Convert.ToInt32(levelrow[i]);
                    }
                }
                levels[dtGrid.Columns.Count] = 0;
                dtGrid.Rows.RemoveAt(dtGrid.Rows.Count - 1);
                dtGrid.AcceptChanges();
                UltraWebGrid1.DataSource = dtGrid;
            }

            SetHeaderInfo();

            header = new TableRow[headerLayout.HeaderHeight];
            for (int i = 0; i < headerLayout.HeaderHeight; i++)
            {
                header[i] = new TableRow();
                header[i].CssClass = "DasHeaderRow";
            }

            TableCell headerCell = new TableCell();
            headerCell.RowSpan = headerLayout.HeaderHeight;
            headerCell.CssClass = "DasSelectorRow";
            header[0].Cells.Add(headerCell);

            int cellIndex = 0;
            foreach (GridHeaderCell cell in headerLayout.childCells)
            {
                if (cell.ChildCount == 0)
                {
                    header[0].Cells.Add(GetGridHeaderCell(cell.Caption, 1, headerLayout.HeaderHeight));
                    cellIndex++;
                }
                else
                {
                    cellIndex += ApplyCellsInfo(cell, cellIndex, 0, header);
                }
            }

            Table1.Rows.AddRange(header);            

            BindDasGrid();
        }

        TableRow[] header;

        private int ApplyCellsInfo(GridHeaderCell headerCell, int cellIndex, int level, TableRow[] headerRow)
        {
            int childIndex = cellIndex;
            int childCount = 0;

            for (int i = 0; i < headerCell.ChildCount; i++)
            {
                GridHeaderCell cell = headerCell.childCells[i];
                if (cell.ChildCount == 0)
                {
                    headerRow[level + headerCell.SpanY].Cells.Add(GetGridHeaderCell(cell.Caption, 1, headerLayout.HeaderHeight - level - headerCell.SpanY));
                    childCount++;
                    childIndex++;
                }
                else
                {
                    int count = ApplyCellsInfo(cell, childIndex, level + headerCell.SpanY, headerRow);
                    childIndex += count;
                    childCount += count;
                }
            }

            headerRow[level].Cells.Add(GetGridHeaderCell(headerCell.Caption, childCount, headerCell.SpanY));

            return childCount;
        }

        private void BindDasGrid()
        {
            for (int rowCount = 0; rowCount < dtGrid.Rows.Count; rowCount++)
            {
                DataRow row = dtGrid.Rows[rowCount];

                TableRow tableRow = new TableRow();

                if (rowCount % 2 != 0)
                {
                    tableRow.CssClass = "DasRowAlternate";
                }

                TableCell firstCell = new TableCell();
                firstCell.CssClass = "DasSelectorRow";
                tableRow.Cells.Add(firstCell);

                if (row[0] != DBNull.Value &&
                ((row[0].ToString().Contains("район")) ||
                (row[0].ToString().Contains("округа")) ||
                (row[0].ToString().Contains("Итого") ||
                (row[0].ToString().Contains("Новосибирская Область (Бюджет Субъекта)")))) &&
                (row[0].ToString().Contains("Собственный бюджет муниципального района") == false))
                {
                    tableRow.Font.Bold = true;
                }

                if ((row[0] != DBNull.Value) &&
                (row[0].ToString().Contains("ДАННЫЕ")))
                {
                    row[0] = "Cобственный бюджет района";
                }

                for (int cellCount = 0; cellCount < dtGrid.Columns.Count; cellCount++)
                {
                    TableCell tableCell = new TableCell();
                    tableCell.Text = row[cellCount] == DBNull.Value ? "&nbsp;" : String.Format("{0:N2}", row[cellCount]);
                    tableRow.Cells.Add(tableCell);
                }
                tableRow.Cells[1].CssClass = "NameCell";
                Table1.Rows.Add(tableRow);
            }
            Table1.CssClass = "DasGrid";
            Table1.Attributes.Add("cellspacing", "0px");
        }

        private void SetHeaderInfo()
        {
            headerLayout.AddCell("Муниципальные образования");
            GridHeaderCell cell;
            GridHeaderCell lowcell = null;
            GridHeaderCell lowlowcell = null;
            cell = headerLayout;
            for (int colNum = 1; colNum < dtGrid.Columns.Count; colNum++)
            {
                string caption = dtGrid.Columns[colNum].ColumnName;
                if ((caption == "Все коды ФКР") || (caption == "Все виды расходов") || (caption == "Все коды ЭКР"))
                {
                    caption = "Итого расходов";
                }
                if (KBK.Value == "Вид расходов")
                {
                    cell.AddCell(caption);
                }
                else
                {
                    switch (levels[colNum])
                    {
                        case 0:
                            {
                                cell.AddCell(caption, levels[colNum]);
                                break;
                            }
                        case 1:
                            {
                                lowcell = cell.AddCell(caption);
                                if (levels[colNum + 1] == 2)
                                {
                                    if ((colNum > 24) && (KBK.Value == "КОСГУ"))
                                    {
                                        lowcell.AddCell("Всего", 2);
                                    }
                                    else
                                    {
                                        lowcell.AddCell("Всего");
                                    }
                                }
                                break;
                            }
                        case 2:
                            {
                                if ((colNum > 24) && (KBK.Value == "КОСГУ"))
                                {
                                    lowlowcell = lowcell.AddCell(caption, 2);
                                }
                                else
                                {
                                    lowlowcell = lowcell.AddCell(caption);
                                }
                                if (levels[colNum + 1] == 3)
                                {
                                    lowlowcell.AddCell("Всего");
                                }
                                break;
                            }
                        case 3:
                            {
                                lowlowcell.AddCell(caption);
                                break;
                            }
                    }
                }

            }
        }

        private TableCell GetGridHeaderCell(string caption, int spanX, int spanY)
        {
            TableCell cell = new TableCell();
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.Text = caption;
            cell.ColumnSpan = spanX;
            cell.RowSpan = spanY;
            return cell; 
        }

        protected void UltraWebGrid_InitializeLayout(Object sender, LayoutEventArgs e)
        {
            int summWidth = 0;
            e.Layout.StationaryMargins = StationaryMargins.No;

            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
           
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);

            summWidth += 20;
            summWidth += CRHelper.GetColumnWidth(200);
            Table1.Rows[0].Cells[0].Style.Add("Width", "20px");
            Table1.Rows[0].Cells[1].Style.Add("Width", CRHelper.GetColumnWidth(200).ToString() + "px");

           
            headerLayout.ApplyHeaderInfo();
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(160);
                Table1.Rows[headerLayout.HeaderHeight].Cells[i + 1].Width = CRHelper.GetColumnWidth(160);
                summWidth += CRHelper.GetColumnWidth(160);
            }
            Table1.Width = (int)(summWidth);
        }


        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[0].Value != null &&
                ((e.Row.Cells[0].Value.ToString().Contains("район")) || (e.Row.Cells[0].Value.ToString().Contains("округа")) || (e.Row.Cells[0].Value.ToString().Contains("Итого")|| (e.Row.Cells[0].Value.ToString().Contains("Новосибирская Область (Бюджет Субъекта)")))) &&
                (e.Row.Cells[0].Value.ToString().Contains("Собственный бюджет муниципального района") == false))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
                if ((e.Row.Cells[0].Value != null) &&
                (e.Row.Cells[0].Value.ToString().Contains("ДАННЫЕ")))
                {
                    e.Row.Cells[0].Value =  "Cобственный бюджет района";
                }
            }
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