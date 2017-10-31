using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Web;

namespace Krista.FM.Server.Dashboards.reports.STAT_0023_0004
{
    public enum SliceType
    {
        OKVED,
        OKOPF,
        OKFS
    }

    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable chartDt = new DataTable();
        private DataTable gridDt = new DataTable();
        private string gridIndicatorCaption = "Показатели";
        private int columnWidth = 100;

        private static MemberAttributesDigest periodDigest;
        private DateTime lastDate;

        #endregion

        private int GetScreenWidth
        {
            get
            {
                if (Request.Cookies != null)
                {
                    if (Request.Cookies[CustomReportConst.ScreenWidthKeyName] != null)
                    {
                        HttpCookie cookie = Request.Cookies[CustomReportConst.ScreenWidthKeyName];
                        int value = Int32.Parse(cookie.Value);
                        return value;
                    }
                }
                return (int)Session["width_size"];
            }
        }

        private bool IsSmallResolution
        {
            get { return GetScreenWidth < 1000; }
        }

        private int MinScreenWidth
        {
            get { return IsSmallResolution ? 800 : CustomReportConst.minScreenWidth; }
        }

        private int MinScreenHeight
        {
            get { return IsSmallResolution ? 600 : CustomReportConst.minScreenHeight; }
        }

        #region Параметры запроса

        // множество для среза данных
        private CustomParam sliceSet;
        // элемент для среза данных
        private CustomParam sliceMember;
        // множество лет
        private CustomParam periodSet;

        #endregion

        public SliceType SliceType
        {
            get
            {
                switch (SliceTypeButtonList.SelectedIndex)
                {
                    case 0:
                        {
                            return SliceType.OKFS;
                        }
                    case 1:
                        {
                            return SliceType.OKOPF;
                        }
                    case 2:
                        {
                            return SliceType.OKVED;
                        }
                }
                return SliceType.OKVED;
            }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.AutoHeightRowLimit = 15;
            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.8 - 235);
            //GridBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth * 0.8 - 15);
            //GridBrick.Width = IsSmallResolution ? 720 : CRHelper.GetGridWidth(MinScreenWidth - 12);
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);
            //SliceTypeButtonList.RepeatDirection = IsSmallResolution ? RepeatDirection.Vertical : RepeatDirection.Horizontal;
            SliceTypeButtonList.Width = IsSmallResolution ? 500 : 900;
            #endregion

            #region Настройка диаграммы динамики

            ChartBrick.Width = IsSmallResolution ? 720 : Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            ChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.8 - 100);

            ChartBrick.YAxisLabelFormat = "N0";
            ChartBrick.DataFormatString = "N0";
            ChartBrick.DataItemCaption = "Единицы";
            ChartBrick.Legend.Visible = true;
            ChartBrick.Legend.Location = LegendLocation.Bottom;
            ChartBrick.Legend.SpanPercentage = 40;
            ChartBrick.ColorModel = ChartColorModel.ExtendedFixedColors;
            ChartBrick.ColorSkinRowWise = false;
            ChartBrick.XAxisExtent = 40;
            ChartBrick.YAxisExtent = 90;
            ChartBrick.ZeroAligned = true;
            ChartBrick.SeriesLabelWrap = true;
            ChartBrick.TooltipFormatString = "<b><ITEM_LABEL></b> год\n<SERIES_LABEL>\n<b><DATA_VALUE:N0></b> единиц";
            ChartBrick.SwapRowAndColumns = true;

            #endregion

            #region Инициализация параметров запроса

            sliceSet = UserParams.CustomParam("slice_set");
            periodSet = UserParams.CustomParam("period_set");
            sliceMember = UserParams.CustomParam("slice_member");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = true;
                periodDigest = new MemberAttributesDigest(DataProvidersFactory.SpareMASDataProvider, "STAT_0023_0004_periodDigest");
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(periodDigest.UniqueNames, periodDigest.MemberLevels));
               
                lastDate = CubeInfo.GetLastDate(DataProvidersFactory.SpareMASDataProvider, "STAT_0023_0004_lastDate");
                ComboYear.SetСheckedState(lastDate.Year.ToString(), true);
                ComboYear.SetСheckedState((lastDate.Year - 1).ToString(), true);
                ComboYear.SetСheckedState((lastDate.Year - 2).ToString(), true);
                ComboYear.SetСheckedState((lastDate.Year - 3).ToString(), true);
                ComboYear.SetСheckedState((lastDate.Year - 4).ToString(), true);
            }
            
            if (ComboYear.SelectedValues.Count == 0)
            {
                ComboYear.SetСheckedState(lastDate.Year.ToString(), true);
            }

            string periods = String.Empty;
            foreach (string year in ComboYear.SelectedValues)
            {
                periods += String.Format("[Период__Год Квартал Месяц].[Период__Год Квартал Месяц].[{0}],", year);
            }
            periodSet.Value = periods.TrimEnd(',');

            string sliceText = String.Empty;
            switch (SliceType)
            {
                case SliceType.OKFS:
                    {
                        sliceSet.Value = "[ОКФС]";
                        sliceMember.Value = "[ОК__ОКФС].[ОК__ОКФС]";
                        sliceText = "в разрезе форм собственности";
                        gridIndicatorCaption = "Формы собственности";
                        break;
                    }
                case SliceType.OKOPF:
                    {
                        sliceSet.Value = "[ОКОПФ]";
                        sliceMember.Value = "[ОК__ОКОПФ].[ОК__ОКОПФ]";
                        sliceText = "в разрезе организационно-правовых форм";
                        gridIndicatorCaption = "Организационно-правовые формы";
                        break;
                    }
                case SliceType.OKVED:
                    {
                        sliceSet.Value = "[ОКВЭД]";
                        sliceMember.Value = "[ОК__ОКВЭД].[ОК__ОКВЭД]";
                        sliceText = "в разрезе видов экономической деятельности";
                        gridIndicatorCaption = "Виды экономической деятельности";
                        break;
                    }
            }

            Page.Title = String.Format("Демография организаций");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Данные ежегодного мониторинга данных по числу учтенных организаций и предприятий в Статистическом регистре хозяйствующих субъектов {0}, ХМАО-Югра, за {1} {2}", sliceText,
                    CRHelper.GetDigitIntervals(ComboYear.SelectedValuesString, ','),
                    ComboYear.SelectedValues.Count == 1 ? "год" : "годы");

            ChartCaption.Text = String.Format("Структурная динамика числа учтенных организаций и предприятий на начало года {0}, единиц", sliceText);

            GridDataBind();
            ChartDataBind();
        }

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("STAT_0023_0004_grid");
            gridDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Наименование показателей", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                if (gridDt.Columns.Count > 1)
                {
                    gridDt.Columns.RemoveAt(0);
                }

                GridBrick.Width = IsSmallResolution ? 720 : CRHelper.GetGridWidth(340 + (gridDt.Columns.Count - 2) * columnWidth + 20);

                GridBrick.DataTable = gridDt;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.Bands[0].HeaderStyle.Wrap = true;
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(340);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell(gridIndicatorCaption);

            for (int i = 1; i < columnCount - 2; i = i + 1)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;

                string year = e.Layout.Bands[0].Columns[i].Header.Caption;
                headerLayout.AddCell(String.Format("{0} год", year));
            }

            headerLayout.ApplyHeaderInfo();
        }

        protected void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellCount = e.Row.Cells.Count;

            int type = 0;
            if (e.Row.Cells[cellCount - 2].Value != null)
            {
                type = Convert.ToInt32(e.Row.Cells[cellCount - 2].Value.ToString());
            }

            if (e.Row.Cells[0].Text == "Всего")
            {
                e.Row.Cells[0].Style.Font.Bold = true;
            }

            if (e.Row.Cells[cellCount - 1].Value != DBNull.Value)
            {
                if (Convert.ToInt32(e.Row.Cells[cellCount - 1].Text) == 1)
                {
                    e.Row.Cells[0].Style.Padding.Left = 20;
                }
                if (Convert.ToInt32(e.Row.Cells[cellCount - 1].Text) == 2)
                {
                    e.Row.Cells[0].Style.Padding.Left = 40;
                }
            }

            for (int i = 1; i < cellCount - 2; i++)
            {
                
                UltraGridCell cell = e.Row.Cells[i];
                cell.Style.Padding.Right = 3;
                if (e.Row.Cells[0].Text == "Всего")
                {
                    cell.Style.Font.Bold = true;
                }

                
                string[] nameParts = e.Row.Band.Columns[i].Header.Caption.Split(' ');
                int prevYear = lastDate.Year;
                if (nameParts.Length > 0)
                {
                    prevYear = Convert.ToInt32(nameParts[0]) - 1;
                }

                switch (type)
                {
                    case 0:
                        {
                            if (cell.Value != null)
                            {
                                cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N0");
                            }
                            cell.Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                    case 1:
                        {
                            if (cell.Value != null)
                            {
                                cell.Value = Convert.ToDouble(cell.Value.ToString()).ToString("N0");
                                cell.Title = String.Format("Прирост к {0} году", prevYear);
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
                                cell.Value = growRate.ToString("P1");

                                if (growRate > 0)
                                {
                                    cell.Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                                }
                                else if (growRate < 0)
                                {
                                    cell.Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                }
                                cell.Title = String.Format("Темп прироста к {0} году", prevYear);
                                cell.Style.CustomRules = "background-repeat: no-repeat; background-position: center center; margin: 2px";
                            }
                            cell.Style.BorderDetails.WidthTop = 0;
                            break;
                        }
                }
            }
        }

        #endregion

        #region Обработчики диаграммы

        private void ChartDataBind()
        {
            string queryText = DataProvider.GetQueryText("STAT_0023_0004_chart");
            chartDt = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(queryText, "Показатель", chartDt);
            if (chartDt.Rows.Count > 0)
            {
                if (chartDt.Columns.Count > 1)
                {
                    chartDt.Columns.RemoveAt(0);
                }

                foreach (DataRow row in chartDt.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = GetCropString(row[0].ToString(), 80);
                    }
                }

                ChartBrick.DataTable = chartDt;
            }
        }

        private static string GetCropString(string str, int lengthLimit)
        {
            if (str.Length > lengthLimit)
            {
                return str.Substring(0, lengthLimit) + "...";
            }
            return str;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            GridBrick.Grid.DisplayLayout.SelectedRows.Clear(); 
            
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            ChartBrick.Chart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.6));
            ReportExcelExporter1.Export(ChartBrick.Chart, ChartCaption.Text, sheet2, 3);
        }

        #endregion

        #region Экспорт в PDF

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

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ExportGridSetup();

            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);

            ISection section2 = report.AddSection();
            ChartBrick.Chart.Width = Convert.ToInt32(ChartBrick.Chart.Width.Value * 0.8);
            ReportPDFExporter1.Export(ChartBrick.Chart, ChartCaption.Text, section2);
        }

        #endregion
    }
}