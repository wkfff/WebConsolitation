using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0009
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";
        private double rubMultiplier;

        private bool PrevYearComparingDisplay
        {
            get { return Convert.ToBoolean(prevYearComparingDisplay.Value); }
        }

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "Тыс.руб." : "Млн.руб."; }
        }

        #region Параметры запроса

        // отображать сравнение с прошлым годом
        private CustomParam prevYearComparingDisplay;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 225);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            #region Инициализация параметров запроса

            if (prevYearComparingDisplay == null)
            {
                prevYearComparingDisplay = UserParams.CustomParam("prev_year_comparing_display");
            }

            #endregion

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);

            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));
            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.IncomesKDSubSectionLevel.Value = RegionSettingsHelper.Instance.IncomesKDSubSectionLevel;
            
            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0009_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                if (dtDate != null && dtDate.Rows.Count > 0)
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                    month = dtDate.Rows[0][3].ToString();
                }

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
            }
            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            Page.Title = String.Format("Финансовая помощь из федерального бюджета в {0}", RegionSettingsHelper.Instance.OwnSubjectBudgetName.ToLower());
            Label1.Text = Page.Title;
            Label2.Text = string.Format("Оценка исполнения за {0} {1} {2} года в сравнении с прошлым годом",
                monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodLastYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodFirstYear.Value = (yearNum - 2).ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;

            prevYearComparingDisplay.Value = RegionSettingsHelper.Instance.GetPropertyValue("PrevYearComparingDisplay");

            if (PrevYearComparingDisplay)
            {
                PopupInformer1.HelpPageUrl = "Default_PrevYearComparing.html";
            }

            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0009_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование вида помощи", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length; i++)
                    {
                        if ((i == 1 || i == 2 || i == 4 || i == 5 || (PrevYearComparingDisplay && i == 7))
                            && row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) / rubMultiplier;
                        }
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
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
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int columnWidth = 60;

                int groupNumber = (i - 1) / 3;
                bool prevYearComparingColumns = (groupNumber == 2 && PrevYearComparingDisplay);
                int j = (i - 1) % 3;
                switch (j)
                {
                    case 0:
                        {
                            formatString = "N2";
                            columnWidth = 110;
                            break;
                        }
                    case 1:
                        {
                            formatString = prevYearComparingColumns ? "P2" : "N2";
                            columnWidth = prevYearComparingColumns ? 80 : 110;
                            break;
                        }
                    case 2:
                        {
                            formatString = "P2";
                            columnWidth = 90;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = columnWidth;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(PrevYearComparingDisplay ? 260 : 500);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 1;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 2; i = i + 3)
            {
                int groupNumber = (i - 1) / 3;

                ColumnHeader ch = new ColumnHeader(true);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0];

                if (groupNumber == 2 && PrevYearComparingDisplay)
                {
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, String.Format("Отклонение{0}(+,-)", Environment.NewLine), "");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Темп роста, %", "");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "Рост (снижение)", "");
                }
                else
                {
                    int monthNum = ComboMonth.SelectedIndex + 1;
                    int year = i == 1
                               ? Convert.ToInt32(ComboYear.SelectedValue) - 1
                               : Convert.ToInt32(ComboYear.SelectedValue);
                    if (monthNum == 12)
                    {
                        monthNum = 1;
                        year++;
                    }
                    else
                    {
                        monthNum++;
                    }

                    string dateStr = string.Format("на 01.{0:00}.{1} г.", monthNum, year);

                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, String.Format("Уточенные годовые назначения, {0}", RubMultiplierCaption.ToLower()), "Годовые назначения доходов в прошлом году");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1,  String.Format("Факт, {0}", RubMultiplierCaption.ToLower()), string.Format("Исполнено {0}", dateStr));
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "% исполнения", "% исполнения годовых назначений расходов");
                }
                
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 3;
                ch.RowLayoutColumnInfo.SpanX = 3;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool execute = (i == 3 || i == 6);
                int levelIndex = PrevYearComparingDisplay ? 10 : 7;
                bool rate = (PrevYearComparingDisplay && i == 9);

                if ((execute) && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    int curYearIndex = i;
                    int lastYearIndex = i == 6 ? 3 : 8;

                    if (e.Row.Cells[curYearIndex].Value != null && e.Row.Cells[curYearIndex].Value.ToString() != string.Empty &&
                        e.Row.Cells[lastYearIndex].Value != null && e.Row.Cells[lastYearIndex].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToDouble(e.Row.Cells[curYearIndex].Value) > Convert.ToDouble(e.Row.Cells[lastYearIndex].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Процент исполнения больше, чем за аналогичный период прошлого года";
                        }
                        else if (Convert.ToDouble(e.Row.Cells[curYearIndex].Value) < Convert.ToDouble(e.Row.Cells[lastYearIndex].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Процент исполнения меньше, чем за аналогичный период прошлого года";
                        }
                    }
                }

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != String.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Прирост к прошлому году";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Снижение к прошлому году";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (e.Row.Cells[levelIndex] != null && e.Row.Cells[levelIndex].Value.ToString() != string.Empty)
                {
                    string level = e.Row.Cells[levelIndex].Value.ToString();
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;
                    switch (level)
                    {
                        case "Подгруппа":
                            {
                                fontSize = 10;
                                bold = true;
                                italic = false;
                                break;
                            }
                        case "Статья":
                            {
                                fontSize = 10;
                                bold = true;
                                italic = false;
                                break;
                            }
                        case "Подстатья":
                            {
                                fontSize = 9;
                                bold = false;
                                italic = false;
                                break;
                            }
                        case "Элемент подстатьи":
                            {
                                fontSize = 8;
                                bold = false;
                                italic = false;
                                break;
                            }
                    }
                    e.Row.Cells[i].Style.Font.Size = fontSize;
                    e.Row.Cells[i].Style.Font.Bold = bold;
                    e.Row.Cells[i].Style.Font.Italic = italic;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
                }
                
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

            for (int i = 1; i < columnCount; i = i + 1)
            {
                string formatString = "N0";
                int columnWidth = 60;

                int groupNumber = (i - 1) / 3;
                int j = (i - 1) % 3;
                switch (j)
                {
                    case 0:
                        {
                            formatString = "#,##0.00;[Red]-#,##0.00";
                            columnWidth = 120;
                            break;
                        }
                    case 1:
                        {
                            formatString = (groupNumber == 2 && PrevYearComparingDisplay) ? "#,##0.00%;[Red]-#,##0.00%" : "#,##0.00;[Red]-#,##0.00";
                            columnWidth = 120;
                            break;
                        }
                    case 2:
                        {
                            formatString = "#,##0.00%;[Red]-#,##0.00%";
                            columnWidth = 100;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = columnWidth * 37;
            }

            e.CurrentWorksheet.Columns[0].Width = 300 * 37;

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[4].Height = 17 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            // расставляем стили у начальных колонок
            for (int i = 5; i < rowsCount + 5; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 35 * 37;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
                for (int j = 1; j < columnCount; j++)
                {
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.Alignment = HorizontalCellAlignment.Center;
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
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
        }
        
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.GridElementCaption = Label2.Text;
            UltraGridExporter1.HeaderChildCellHeight = 60;
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        #endregion
    }
}
