using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0042_0002_Yar
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2009;
        private int endYear = 2012;
        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedYear;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double scale = 0.7;
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            #region Инициализация параметров запроса

            selectedYear = UserParams.CustomParam("selected_year");

            #endregion

            CrossLink1.Visible = true;
            CrossLink1.Text = "Результаты&nbsp;бальной&nbsp;оценки";
            CrossLink1.NavigateUrl = "~/reports/FO_0042_0001_Yar/Default.aspx";

            CrossLink2.Visible = true;
            CrossLink2.Text = "Рейтинг&nbsp;ГРБС";
            CrossLink2.NavigateUrl = "~/reports/FO_0042_0003_Yar/Default.aspx";

            CrossLink3.Visible = true;
            CrossLink3.Text = "Анализ&nbsp;оценок&nbsp;ГРБС";
            CrossLink3.NavigateUrl = "~/reports/FO_0042_0004_Yar/Default.aspx";
        }
        
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0042_0002_Yar_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
            }

            Page.Title = String.Format("Сводные итоги оценки качества финансового менеджмента, осуществляемого главными распорядителями средств областного бюджета Ярославской области");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("по итогам {0} года", ComboYear.SelectedValue);

            selectedYear.Value = ComboYear.SelectedValue;

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();     
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0002_Yar_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование администратора", dtGrid);
            if (dtGrid.Rows.Count > 0 && dtGrid.Columns.Count > 2)
            {
                dtGrid.Columns.RemoveAt(0);

                DataTable sortingDT = dtGrid.Clone();
                DataRow[] sortingRows = dtGrid.Select("", "Код ASC");
                foreach (DataRow row in sortingRows)
                {
                    sortingDT.ImportRow(row);
                }
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
            numberColumn.Header.Caption = "№ п/п";
            numberColumn.Width = CRHelper.GetColumnWidth(30);
            numberColumn.CellStyle.Padding.Right = 5;
            numberColumn.CellStyle.BackColor = numberColumn.Header.Style.BackColor;
            numberColumn.CellStyle.Font.Bold = true;
            numberColumn.SortingAlgorithm = SortingAlgorithm.NotSet;
            numberColumn.CellStyle.HorizontalAlign = HorizontalAlign.Right;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(50);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "000");

            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].Columns[2].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[2].CellStyle.Wrap = true;

            int beginIndex = 3;
            for (int i = beginIndex; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = GetColumnFormat(e.Layout.Bands[0].Columns[i].Header.Caption);
                int widthColumn = 140;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            
            headerLayout.AddCell("№ п/п");
            headerLayout.AddCell("Код ГРБС");
            headerLayout.AddCell("Наименование ГРБС");

            headerLayout.AddCell("Рейтинговая оценка (R)", "Баллы");
            headerLayout.AddCell("Интегральная оценка качества финансового менеджмента (КФМ)", "Баллы");
            headerLayout.AddCell("Максимальная оценка за качество финансового менеджмента (MAX)", "Баллы");
            headerLayout.AddCell("Уровень качества финансового менеджмента (Q)", "Проценты");
            headerLayout.AddCell("Коэффициент сложности управления финансами (k)", "Баллы");

            headerLayout.ApplyHeaderInfo();
        }

        private static string GetColumnFormat(string columnName)
        {
            return columnName.ToLower().Contains("уровень качества финансового менеджмента") ? "P0" : "N1";
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

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            ReportExcelExporter1.Export(headerLayout, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            ReportPDFExporter1.HeaderCellHeight = 70;
            ReportPDFExporter1.Export(headerLayout);
        }

        #endregion
    }
}
