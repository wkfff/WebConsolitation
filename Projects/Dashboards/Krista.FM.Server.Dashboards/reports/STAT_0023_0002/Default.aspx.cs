using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.STAT_0023_0002
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();

        private DateTime currDate;
        private DateTime prevDate;

        private static MemberAttributesDigest periodDigest;
        private static MemberAttributesDigest periodCompareDigest;
        private static MemberAttributesDigest moDigest;
        private static MemberAttributesDigest orgDigest;

        #endregion

        #region Параметры запроса

        // текущий период
        private CustomParam currPeriod;
        // предыдущий период
        private CustomParam prevPeriod;
        // выбранное МО
        private CustomParam selectedMO;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.AutoSizeStyle = GridAutoSizeStyle.Auto;
            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.8 - 235);
            GridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 15);
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);
            GridBrick.Grid.DataBound += new EventHandler(Grid_DataBound);
            
            #endregion
            
            #region Инициализация параметров запроса

            currPeriod = UserParams.CustomParam("curr_period");
            prevPeriod = UserParams.CustomParam("prev_period");
            selectedMO = UserParams.CustomParam("selected_mo");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.CellExporting += new Infragistics.WebUI.UltraWebGrid.ExcelExport.CellExportingEventHandler(ExcelExporter_CellExporting);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }


        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            orgDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0023_0002_orgDigest");

            if (!Page.IsPostBack)
            {
                ComboPeriod.Title = "Период";
                ComboPeriod.Width = 250;
                ComboPeriod.MultiSelect = false;
                ComboPeriod.ParentSelect = false;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0023_0002_periodDigest");
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
                ComboPeriod.SelectLastNode();

                ComboCompareType.Title = "Период для сравнения";
                ComboCompareType.Width = 350;
                periodCompareDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0023_0002_periodCompareDigest");
                ComboCompareType.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodCompareDigest.UniqueNames, periodCompareDigest.MemberLevels));

                ComboMO.Title = "МО";
                ComboMO.Width = 350;
                ComboMO.MultiSelect = false;
                ComboMO.ParentSelect = false;
                moDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0023_0002_moDigest");
                ComboMO.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(moDigest.UniqueNames, moDigest.MemberLevels));
                ComboMO.SetСheckedState("Город Ханты-Мансийск", true);
            }

            currPeriod.Value = periodDigest.GetMemberUniqueName(ComboPeriod.SelectedValue);

            currDate = CRHelper.PeriodDayFoDate(currPeriod.Value);

            prevDate = GetPreviousDate(currDate);

            prevPeriod.Value = CRHelper.PeriodMemberUName("[Период].[Период]", prevDate, 4);

            Page.Title = String.Format("Мониторинг финансово-экономического состояния предприятий, включенных  в перечень предприятий регионального значения ");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Ежемесячный анализ основных показателей, характеризующих работу предприятий регионального значения, {2}, за {0} {1} года",
                CRHelper.RusMonth(currDate.Month), currDate.Year, ComboMO.SelectedValue);

            selectedMO.Value = moDigest.GetMemberUniqueName(ComboMO.SelectedValue);

            GridDataBind();
        }

        private DateTime GetPreviousDate(DateTime currentDate)
        {
            switch (ComboCompareType.SelectedIndex)
            {
                case 0:
                    {
                        return currentDate.AddMonths(-1);
                    }
                case 1:
                    {
                        return new DateTime(currentDate.Year, 1, 1);
                    }
                case 2:
                    {
                        return currentDate.AddYears(-1);
                    }
            }
            return currentDate.AddMonths(-1);
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("STAT_0023_0002_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                if (gridDt.Columns.Count > 1)
                {
                    gridDt.Columns.RemoveAt(0);
                }

                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(340);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(113);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            
            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            headerLayout.AddCell("Предприятие");

            headerLayout.AddCell("Среднемесячная численность работников, чел.");
            headerLayout.AddCell("Численность работников, находящихся в простое, длительных отпусках без сохранения, содержания, занятых неполный рабочий день, чел.");
            headerLayout.AddCell("Численность работников, планируемых к высвобождению, чел.");
            headerLayout.AddCell("Месячный объем фонда оплаты труда , тыс.руб.");
            headerLayout.AddCell(String.Format("Объем задолженности по выплате заработной платы по состоянию на {0:dd.MM.yyyy} г., тыс.руб.", currDate.AddMonths(1)));
            headerLayout.AddCell("Объем среднемесячных оборотов предприятия, млн.руб.");
            headerLayout.AddCell("Меры (объем) государственной поддержки, реализуемые на региональном уровне в отношении предприятия, тыс.руб.");

            headerLayout.ApplyHeaderInfo();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellCount = e.Row.Cells.Count;

            int type = 0;
            if (e.Row.Cells[cellCount - 1].Value != null)
            {
                type = Convert.ToInt32(e.Row.Cells[cellCount - 1].Value.ToString());
            }

            if (e.Row.Cells[0].Value != null)
            {
                string name = e.Row.Cells[0].Value.ToString();
                string info = orgDigest.GetMemberType(name).Replace(";", ";<br/>");
                e.Row.Cells[0].Value = name + AddExpandSection(info);
            }

            for (int i = 1; i < cellCount - 1; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                cell.Style.Padding.Right = 3;

                string columnCaption = e.Row.Band.Grid.Columns[i].Header.Caption;
                bool inverseRate = columnCaption.Contains("задолженности") || columnCaption.Contains("планируемых к высвобождению");

                switch (type)
                {
                    case 0:
                        {
                            if (cell.Value != null)
                            {
                                cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N2");
                            }
                            cell.Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                    case 1:
                        {
                            if (cell.Value != null)
                            {
                                cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N2");
                                cell.Title = String.Format("Прирост к {0} {1} года", CRHelper.RusMonthDat(prevDate.Month), prevDate.Year);
                            }
                            cell.Style.BorderDetails.WidthTop = 0;
                            cell.Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                    case 2:
                        {
                            if (cell.Value != null)
                            {
                                double growRate = Convert.ToDouble(cell.Value.ToString());
                                cell.Value = growRate.ToString("P2");

                                if (growRate > 0)
                                {
                                    cell.Style.BackgroundImage = inverseRate ? "~/images/arrowRedUpBB.png" : "~/images/arrowGreenUpBB.png";
                                }
                                else if (growRate < 0)
                                {
                                    cell.Style.BackgroundImage = inverseRate ? "~/images/arrowGreenDownBB.png" : "~/images/arrowRedDownBB.png";
                                   
                                }
                                cell.Title = String.Format("Темп прироста к {0} {1} года", CRHelper.RusMonthDat(prevDate.Month), prevDate.Year);
                                cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center center; margin: 2px";
                            }
                            cell.Style.BorderDetails.WidthTop = 0;
                            break;
                        }
                }
            }
        }

        private string AddExpandSection(string text)
        {
            return String.Format(@"
 <table style='border-collapse: collapse;border-style: none' width='100%'> 
   <tr>
     <td class='ExpandBlockSecondState' onclick='resize(this)' style='padding-left:20px;background-position:5px Center'>Подробнее</td>
   </tr>
   <tr class='ExpandBlockRowSecondState'>
     <td>{0}</td>
   </tr>
 </table>", text);
        }

        private void Grid_DataBound(object sender, EventArgs e)
        {
//            GridBrick.
        }

        #endregion
        
        #region Экспорт в Excel

        private void RemoveTags()
        {
            foreach (UltraGridRow row in GridBrick.Grid.Rows)
            {
                UltraGridCell cell = row.Cells[0];
                if (cell.Value != null)
                {
                    cell.Value = cell.Value.ToString().Replace("<br/>", ((Char)10).ToString());
                    cell.Value = cell.Value.ToString().Replace("Подробнее", ((Char)10).ToString());
                    cell.Value = Regex.Replace(cell.Value.ToString(), "<[^>]*?>", String.Empty);
                }
            }
        }

        private void ExcelExporter_CellExporting(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.CellExportingEventArgs e)
        {
//            if (e.Value != null)
//            {
//                e.Value = e.Value.ToString().Replace("<br/>", Environment.NewLine);
//                e.Value = e.Value.ToString().Replace("Подробнее", Environment.NewLine);
//                e.Value = Regex.Replace(e.Value.ToString(), "<[^>]*?>", String.Empty);
//            }
        }

        private void ExportGridSetup()
        {
            for (int i = 0; i < GridBrick.Grid.Rows.Count; i++)
            {
                UltraGridCell cell = GridBrick.Grid.Rows[i].Cells[0];

                int groupIndex = i % 3;

                switch (groupIndex)
                {
                    case 0:
                        {
                            cell.Value = String.Empty;
                            cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
                            break;
                        }
                    case 1:
                        {
                            cell.Style.BorderDetails.StyleTop = BorderStyle.None;
                            cell.Style.BorderDetails.StyleBottom = BorderStyle.None;
                            break;
                        }
                    case 2:
                        {
                            cell.Value = String.Empty;
                            cell.Style.BorderDetails.StyleTop = BorderStyle.None;
                            break;
                        }
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            RemoveTags();

            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, String.Empty, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ExportGridSetup(); 
            RemoveTags();

            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 120;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, String.Empty, section1);
        }

        #endregion
    }
}