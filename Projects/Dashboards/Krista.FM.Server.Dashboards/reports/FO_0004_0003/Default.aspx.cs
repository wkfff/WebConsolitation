using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0004_0003
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2010;
        private int endYear = 2012;
        private int currentYear;

        #endregion

        #region Параметры запроса

        // выбранный год
        private CustomParam selectedYear;
        // множество районов/поселений
        private CustomParam regionSet;
        // фильтр по группе МО
        private CustomParam moGroupFilter;
        // уровень районов
        private CustomParam regionsLevel;
        // уровень поселений
        private CustomParam settlementsLevel;

        #endregion

        private bool WithSettlements
        {
            get { return withSettlementsCheckBox.Checked; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            double scale = 0.9;

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * scale - 160);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            if (selectedYear == null)
            {
                selectedYear = UserParams.CustomParam("selected_year");
            }
            if (regionSet == null)
            {
                regionSet = UserParams.CustomParam("region_set");
            }
            moGroupFilter = UserParams.CustomParam("mo_group_filter");
            regionsLevel = UserParams.CustomParam("regions_level");
            settlementsLevel = UserParams.CustomParam("settlements_level");

            #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.HeaderCellHeight = 100;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            settlementsLevel.Value = RegionSettingsHelper.Instance.SettlementLevel;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0039_0009_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMOGroup.Title = "Группа МО";
                ComboMOGroup.Width = 220;
                ComboMOGroup.MultiSelect = false;
                ComboMOGroup.ParentSelect = true;
                ComboMOGroup.FillDictionaryValues(CustomMultiComboDataHelper.FillBKKUGroupMO());
                ComboMOGroup.SetСheckedState("Все группы ", true);
            }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue) - 2;

            Group1Label.Text = "Группа 3 <img src=\"../../images/ballRedBB.png\"> доля более 70%";
            Group2Label.Text = "Группа 2 <img src=\"../../images/ballOrangeBB.png\"> доля от 30% до 70%";
            Group3Label.Text = "Группа 1 <img src=\"../../images/ballYellowBB.png\"> доля от 10% до 30%";
            Group4Label.Text = "Группа 0 <img src=\"../../images/ballGreenBB.png\"> доля менее 10%";

            Page.Title = String.Format("Расчет группы, присвоенной муниципальным образованиям Саратовской области по ст.136 БКРФ");
            PageTitle.Text = Page.Title;
            
            PageSubTitle.Text = String.Format("на {0} год", ComboYear.SelectedValue);

            if (ComboMOGroup.SelectedValue == "Все группы ")
            {
                moGroupFilter.Value = "(true)";
            }
            else
            {
                moGroupFilter.Value = string.Format("not IsEmpty( [Показатели__ФО_ФПКУ_Исходные данные].[Показатели__ФО_ФПКУ_Исходные данные].[Группа МР(ГО)])and ( [Показатели__ФО_ФПКУ_Исходные данные].[Показатели__ФО_ФПКУ_Исходные данные].[Группа МР(ГО)]) = {0}", ComboMOGroup.SelectedIndex );
            }

            selectedYear.Value = currentYear.ToString();
            regionSet.Value = WithSettlements ? "Список районов и поселений" : "Список МО";

            UltraWebGrid.Bands.Clear();     
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0039_0009_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                if (dtGrid.Columns.Count > 1)
                {
                    dtGrid.Columns.RemoveAt(0);
                }

                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = row[0].ToString().Replace("район", "р-н");
                        row[0] = row[0].ToString().Replace("сельское поселение", "СП");
                        row[0] = row[0].ToString().Replace("городское поселение", "ГП");
                    }
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

            if (e.Layout.Bands[0].Columns.Count < 2)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(150);
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 0, "Наименование муниципального образования", "");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            
            int beginIndex = 1;

            for (int i = beginIndex; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString;
                int columnWidth = (i == beginIndex) ? 74 : 75;

                int groupIndex = (i - beginIndex - 1) / 3;
                switch(groupIndex)
                {
                    case 0:
                        {
                            formatString = (i == beginIndex) ? "N0" : "P2";
                            break;
                        }
                    default:
                        {
                            formatString = (i == beginIndex) ? "N0" : "N2";
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(columnWidth);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            
            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i <= beginIndex)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, beginIndex, WithSettlements ? "Группа МО" : "Группа МР(ГО)", "");

            int multiHeaderPos = beginIndex + 1;
            int spanX = 3;

            for (int i = beginIndex + 1; i < e.Layout.Bands[0].Columns.Count; i = i + spanX)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, currentYear.ToString(), " ");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, (currentYear - 1).ToString(), " ");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, (currentYear - 2).ToString(), " ");

                CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, captions[0], multiHeaderPos, 0, spanX, 1);
                
                multiHeaderPos += spanX;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                int nameColumnIndex = 0;
                bool groupColumn = i == 1;

                string name = String.Empty;
                if (e.Row.Cells[nameColumnIndex].Value != null)
                {
                    name = e.Row.Cells[nameColumnIndex].Value.ToString();
                }

                bool groupRow = name.ToLower().Contains("итого по");

                bool groupRow1 = name.Contains("по собственному");

                if (groupColumn && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != String.Empty)
                {
                    string ballImage = String.Empty;
                    int hintPercent = 0;
                    string groupNumber = e.Row.Cells[i].Value.ToString();

                    if (groupNumber == "4")
                    {
                        if (i > 0 && i < 5)
                        {
                            e.Row.Cells[i].Value = "Нет данных";
                        }
                    }
                    else
                    {
                        switch (groupNumber)
                        {
                            case "3":
                                {
                                    e.Row.Cells[i].Title ="Доля межбюджетных трансфертов из других бюджетов бюджетной системы РФ в течение двух из трех последних отчетных финансовых лет более 70 процентов объема собственных доходов местных бюджетов";
                                    ballImage = "~/images/ballRedBB.png";
                                    break;
                                }
                            case "2":
                                {
                                     e.Row.Cells[i].Title ="Доля межбюджетных трансфертов из других бюджетов бюджетной системы РФ в течение двух из трех последних отчетных финансовых лет от 30 до 70 процентов объема собственных доходов местных бюджетов";
                                    ballImage = "~/images/ballOrangeBB.png";
                                    break;
                                }
                            case "1":
                                {
                                    e.Row.Cells[i].Title ="Доля межбюджетных трансфертов из других бюджетов бюджетной системы РФ в течение двух из трех последних отчетных финансовых лет от 10 до 30 процентов объема собственных доходов местных бюджетов";
                                    ballImage = "~/images/ballYellowBB.png";
                                    break;
                                }
                            case "0":
                                {
                                    e.Row.Cells[i].Title = "Доля межбюджетных трансфертов из других бюджетов бюджетной системы РФ в течение двух из трех последних отчетных финансовых лет менее 10 процентов объема собственных доходов местных бюджетов";
                                    ballImage = "~/images/ballGreenBB.png";
                                    break;
                                }
                        }
                        e.Row.Cells[i].Style.BackgroundImage = ballImage;
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        
                    }
                }

                if (groupRow)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                    if (i > 0 && i < 5)
                    {
                        e.Row.Cells[i].Value = "X";
                    }
                }
                if (groupRow1)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
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

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            e.CurrentWorksheet.Columns[0].CellFormat.FormatString = "";
            e.CurrentWorksheet.Columns[0].Width = 150 * 37;

            for (int i = 1; i < columnCount; i = i + 1)
            {
                string formatString;
                int columnWidth = (i == 1) ? 80 : 100;

                int groupIndex = (i - 2) / 3;
                switch (groupIndex)
                {
                    case 0:
                        {
                            formatString = (i == 1) ? "#,##0;[Red]-#,##0" : UltraGridExporter.ExelPercentFormat;
                            break;
                        }
                    default:
                        {
                            formatString = (i == 1) ? "#,##0;[Red]-#,##0" : "#,##0.00;[Red]-#,##0.00";
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = columnWidth * 37;
            }

            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount + 4; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 17 * 37;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
            }

            // расставляем стили у ячеек хидера
            for (int i = 0; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 20 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Left;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
               
                e.CurrentWorksheet.Rows[4].Height = 20 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.FormatString = "0";
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

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(115);
            }

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);
        }

        #endregion
    }
}
