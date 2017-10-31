using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0015_01
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtPlanGrid = new DataTable();
        private DataTable dtFactGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";

        #region Параметры запроса

        // выбранный администратор МБТ
        private CustomParam selectedMBT;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 190);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 7);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            if (selectedMBT == null)
            {
                selectedMBT = UserParams.CustomParam("selected_mbt");
            }

            #endregion

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            CrossLink1.Text = "Распределение&nbsp;межбюджетных&nbsp;трансфертов&nbsp;по&nbsp;муниципальным&nbsp;образованиям";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0015_02/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.EKRDimension.Value = RegionSettingsHelper.Instance.EKRDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0015_01_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                ComboMBTAdmins.Title = "Вид межбюджетных трансфертов";
                ComboMBTAdmins.Width = 400;
                ComboMBTAdmins.MultiSelect = false;
                ComboMBTAdmins.ParentSelect = false;
                ComboMBTAdmins.FillDictionaryValues(CustomMultiComboDataHelper.FillMBTAdministratorList(DataDictionariesHelper.MBTAdministratorsDetailUniqNames, DataDictionariesHelper.MBTAdministratorsDetailLevels));
                ComboMBTAdmins.SetСheckedState("Аппарат Губернатора и Правительства Оренбургской области", true);
            }
            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            //string admin = ComboMBTAdmins.SelectedNodeParent;

            Page.Title = "Исполнение финансирования муниципальных образований";
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Межбюджетные трансферты, передаваемые из бюджета субъекта в бюджеты муниципальных образований за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            selectedMBT.Value = DataDictionariesHelper.MBTAdministratorsDetailUniqNames[ComboMBTAdmins.SelectedValue];

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0015_01_grid_fact");
            dtFactGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Муниципальное образование", dtFactGrid);

            if (dtFactGrid.Rows.Count > 0)
            {
                query = DataProvider.GetQueryText("FO_0002_0015_01_grid_plan");
                dtPlanGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Муниципальное образование", dtPlanGrid);
                dtPlanGrid.PrimaryKey = new DataColumn[]{dtPlanGrid.Columns[0]};

                foreach (DataRow row in dtFactGrid.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                    {
                        string rowName = row[0].ToString();

                        DataRow planRow = dtPlanGrid.Rows.Find(rowName);
                        if (dtPlanGrid.Rows.Count > 0 &&
                            planRow != null &&
                            planRow["Ассигнования, тыс.руб."] != DBNull.Value &&
                            planRow["Ассигнования, тыс.руб."].ToString() != string.Empty)
                        {
                            double planValue = Convert.ToDouble(planRow["Ассигнования, тыс.руб."]);
                            row["Ассигнования, тыс.руб."] = planValue;
                            if (row["Кассовый расход, тыс.руб."] != DBNull.Value &&
                                row["Кассовый расход, тыс.руб."].ToString() != string.Empty)
                            {
                                double factValue = Convert.ToDouble(row["Кассовый расход, тыс.руб."]);
                                row["Остаток ассигнований, тыс.руб."] = planValue - factValue;
                            }
                        }
                        else
                        {
                            row["Ассигнования, тыс.руб."] = DBNull.Value;
                            row["Остаток ассигнований, тыс.руб."] = DBNull.Value;
                        }

                        string[] strs = row[0].ToString().Split(';');
                        if (strs.Length > 1)
                        {
                            if (strs[1].Contains("Бюджеты муниципальных образований Оренбургской области"))
                            {
                                row[0] = strs[0] + "_";
                            }
                            else if (strs[1].Contains("Итого по администратору"))
                            {
                                row[0] = strs[0] + "%";
                            }
                            else
                            {
                                row[0] = strs[1];
                            }
                        }
                    }

                    if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                    {
                        row[1] = Convert.ToDouble(row[1].ToString().TrimEnd(';')).ToString("### #### ####### ###");
                    }
                }

                UltraWebGrid.DataSource = dtFactGrid;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            //            UltraWebGrid.Height = Unit.Empty;
            //            UltraWebGrid.Width = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "P2";
                int widthColumn = 100;

                switch (i)
                {
                    case 1:
                        {
                            //formatString = "### #### ####### # ###";
                            widthColumn = 150;
                            break;
                        }
                    case 2:
                    case 3:
                    case 4:
                        {
                            formatString = "N2";
                            widthColumn = 210;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
            {
                if (e.Row.Cells[0].Value.ToString().Contains("_"))
                {
                    e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().TrimEnd('_');
                    e.Row.Cells[0].ColSpan = 1;
                    e.Row.Style.Font.Bold = true;
                    e.Row.Style.Wrap = true;
                }
                else if (e.Row.Cells[0].Value.ToString().Contains("%"))
                {
                    e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().TrimEnd('%');
                    e.Row.Cells[0].ColSpan = 2;
                    e.Row.Style.Font.Size = 10;
                    e.Row.Style.Font.Bold = true;
                    e.Row.Style.Wrap = true;
                }
                else
                {
                    e.Row.Cells[0].ColSpan = 2;
                }
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
//                bool kd = (i == 1);
//
//                if (kd && e.Row.Cells[i].Value != null)
//                {
//                    e.Row.Cells[i].Value = e.Row.Cells[i].Value.ToString().Replace(';', ' ');
//                }

                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
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
        
        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = Label2.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;
            int headerRowIndex = 3;

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[headerRowIndex].Height = 10 * 37;
                e.CurrentWorksheet.Rows[headerRowIndex].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[headerRowIndex].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[headerRowIndex].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            // расставляем стили у начальных колонок
            e.CurrentWorksheet.Columns[0].Width = 300 * 37;
            e.CurrentWorksheet.Columns[1].Width = 200 * 37;
            for (int i = 5; i < rowsCount + 5; i++)
            {
                if (i < 6)
                {
                    e.CurrentWorksheet.Rows[i].Height = 20*37;
                    e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                    e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
                }
            }

            int width = 110;
            for (int i = 2; i < columnCount; i++)
            {
                string formatString = "#,##0;[Red]-#,##0";
                e.CurrentWorksheet.Columns[i].Width = width * 37;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
            }
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {

        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);
        }
        
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        #endregion

    }
}
