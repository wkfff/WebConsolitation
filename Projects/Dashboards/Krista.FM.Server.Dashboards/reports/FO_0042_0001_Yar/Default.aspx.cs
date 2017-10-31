using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0042_0001_Yar
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtIndicatorDescription;
        private int firstYear = 2009;
        private int endYear = 2012;
        private static Dictionary<string, string> administratorCodeList;
        private static Dictionary<string, string> administratorShortNameList;
        private GridHeaderLayout headerLayout;

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedYear;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double scale = 0.75;
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); ;

            #region Инициализация параметров запроса

            selectedYear = UserParams.CustomParam("selected_year");

            #endregion

            CrossLink1.Visible = true;
            CrossLink1.Text = "Сводная&nbsp;оценка&nbsp;качества";
            CrossLink1.NavigateUrl = "~/reports/FO_0042_0002_Yar/Default.aspx";

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
                string query = DataProvider.GetQueryText("FO_0042_0001_Yar_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
            }

            Page.Title = String.Format("Итоги бальной оценки качества финансового менеджмента, осуществляемого главными распорядителями средств областного бюджета Ярославской области по каждому из показателей");
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("по итогам {0} года", ComboYear.SelectedValue);

            selectedYear.Value = ComboYear.SelectedValue;
            
            AdministratorDescriptionDataBind();

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();     
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0042_0001_Yar_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование администратора", dtGrid);
            if (dtGrid.Rows.Count > 0 && dtGrid.Columns.Count > 2)
            {
                dtGrid.Columns.RemoveAt(0);

                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("_", String.Empty);
                    }
                }

                foreach (DataColumn column in dtGrid.Columns)
                {
                    column.ColumnName = RemoveBadSymbols(column.ColumnName);
                }

                UltraWebGrid.DataSource = dtGrid;
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }
        }

        private static string RemoveBadSymbols(string str)
        {
            str = str.Replace("\"", "'");
            str = str.Replace(@"
", "'");
            return str;
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

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(40);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;

            headerLayout.AddCell("№ п/п");
            headerLayout.AddCell("Наименование направлений оценки, групп показателей, показателей");

            int beginIndex = 2;
            for (int i = beginIndex; i < e.Layout.Bands[0].Columns.Count - 2; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 60;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;

                string caption = RemoveBadSymbols(e.Layout.Bands[0].Columns[i].Header.Caption);
                string admCode = String.Empty;
                if (administratorCodeList.ContainsKey(caption))
                {
                    admCode = administratorCodeList[caption];
                }
                string shortName = String.Empty;
                if (administratorShortNameList.ContainsKey(caption))
                {
                    shortName = administratorShortNameList[caption];
                }

                GridHeaderCell topLevelCell = headerLayout.AddCell(admCode);
                GridHeaderCell secondLevelCell = topLevelCell.AddCell(shortName);
                secondLevelCell.Hint = caption;
            }
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = true;

            headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int levelColumn = e.Row.Cells.Count - 2;
            int emptyRowColumn = e.Row.Cells.Count - 1;
            string level = String.Empty;
            if (e.Row.Cells[levelColumn].Value != null)
            {
                level = e.Row.Cells[levelColumn].Value.ToString();
            }

            bool emptyRow = false;
            if (e.Row.Cells[emptyRowColumn].Value != null)
            {
                emptyRow = e.Row.Cells[emptyRowColumn].Value.ToString() == "true";
            }

            for (int i = 0; i < e.Row.Cells.Count - 2; i++)
            {
                int fontSize = 8;
                bool bold = false;
                bool italic = false;
                switch (level)
                {
                    case "Показатели 1":
                        {
                            fontSize = 8;
                            bold = true;
                            break;
                        }
                    case "Показатели 2":
                        {
                            fontSize = 8;
                            italic = true;
                            break;
                        }
                    case "Показатели 3":
                        {
                            fontSize = 8;
                            break;
                        }
                }
                e.Row.Cells[i].Style.Font.Size = fontSize;
                e.Row.Cells[i].Style.Font.Bold = bold;
                e.Row.Cells[i].Style.Font.Italic = italic;

                if (emptyRow)
                {
                    if (i == 1)
                    {
                        e.Row.Cells[i].ColSpan = e.Row.Cells.Count - 3;
                    }
                    else if (i > 1)
                    {
                        e.Row.Cells[i].Value = String.Empty;
                    }
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (i > 0 && cell.Value != null && cell.Value.ToString() != String.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        Color cellColor = GetValueColor(value);
                        if (cellColor == Color.Transparent)
                        {
                            cell.Value = "-";
                        }
                        else
                        {
                            cell.Style.BackColor = GetValueColor(value);
                        }
                    }
                }
            }
        }

        private static Color GetValueColor(decimal value)
        {
            if (value == 0)
            {
                return Color.FromArgb(200, 0, 0);
            }
            if (value > 0 && value <= 100)
            {
                return Color.FromArgb(255, 0, 0);
            }
            if (value > 100 && value <= 200)
            {
                return Color.FromArgb(250, 135, 72);
            }
            if (value > 200 && value <= 300)
            {
                return Color.FromArgb(246, 205, 54);
            } 
            if (value > 300 && value <= 400)
            {
                return Color.FromArgb(253, 225, 123);
            }
            if (value >= 500)
            {
                return Color.FromArgb(255, 255, 123);
            }
            return Color.Transparent;
        }

        private void AdministratorDescriptionDataBind()
        {
            administratorCodeList = new Dictionary<string, string>();
            administratorShortNameList = new Dictionary<string, string>();

            string query = DataProvider.GetQueryText("FO_0042_0001_Yar_adminstratorDescription");
            dtIndicatorDescription = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtIndicatorDescription);

            foreach (DataRow row in dtIndicatorDescription.Rows)
            {
                if (row[0] != DBNull.Value && row[1] != DBNull.Value)
                {
                    string name = RemoveBadSymbols(row[0].ToString());
                    string code = row[1].ToString();
                    string shortName = row[2].ToString();

                    administratorCodeList.Add(name, code);
                    administratorShortNameList.Add(name, shortName);
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

            ReportPDFExporter1.HeaderCellHeight = 40;
            ReportPDFExporter1.Export(headerLayout);
        }

        #endregion
    }
}
