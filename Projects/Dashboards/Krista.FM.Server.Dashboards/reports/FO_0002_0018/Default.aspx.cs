using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0018
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private int currentYear;
        private string month = "Январь";
        string regionType;

        #region Параметры запроса

        // Выбранный регион
        private CustomParam selectedRegion;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 260);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            #region Инициализация параметров запроса

            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }

            #endregion

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.PdfExportButton.Visible = false;
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

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0018_date");
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

                ComboRegion.Visible = true;
                ComboRegion.Title = "Территория";
                ComboRegion.Width = 280;
                ComboRegion.MultiSelect = false;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillTerritories(RegionsNamingHelper.LocalBudgetTypes, false));
                ComboRegion.SetСheckedState("Алексеевский", true);
            }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue);
            selectedRegion.Value = RegionsNamingHelper.LocalBudgetUniqueNames[ComboRegion.SelectedValue];
            regionType = RegionsNamingHelper.LocalBudgetTypes[ComboRegion.SelectedValue];

            Page.Title = String.Format("Паспорт доходов {0}", ComboRegion.SelectedValue);
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = String.Format("на {0} год", currentYear);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;

            CommentTextDataBind();

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0018_grid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                if (dtGrid.Columns.Count > 1)
                {
                    dtGrid.Columns.RemoveAt(0);
                }

                for (int k = 0; k < dtGrid.Rows.Count; k++)
                {
                    DataRow row = dtGrid.Rows[k];

                    for (int i = 1; i < row.ItemArray.Length - 1; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            if ((i == 9) || (i == 14)) row[i] = Convert.ToDouble(row[i]) * 100;
                        }
                    }
                }        

                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {

        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            string[] captionStr = e.Layout.Bands[0].Columns[0].Header.Caption.Split(';');
            if (captionStr.Length > 1)
            {
                e.Layout.Bands[0].Columns[0].Header.Caption = captionStr[1];
            }

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                captionStr = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                if (captionStr.Length > 1)
                {
                    e.Layout.Bands[0].Columns[i].Header.Caption = captionStr[1];
                }
                
                string formatString = "";
                int widthColumn = 120;

                if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains("Темп роста"))
                {
                    formatString = "N2";
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 15)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 3;
                }
                else if (i == 9 || i == 14)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 2;
                }

                switch(i)
                {
                    case 1:
                    case 5:
                    case 10:
                        {
                            CRHelper.SetHeaderCaption(UltraWebGrid, 0, i, "Абсолютное значение", "Абсолютное значение показателя");
                            break;
                        }
                    case 2:
                    case 6:
                    case 11:
                        {
                            CRHelper.SetHeaderCaption(UltraWebGrid, 0, i, "Относительное значение (место)", 
                                String.Format("Ранг по {0}", regionType));
                            break;
                        }
                    case 3:
                    case 7:
                    case 12:
                        {
                            CRHelper.SetHeaderCaption(UltraWebGrid, 0, i, "ОМСУ с максимальным значением (1 место), абсолютное значение", 
                                String.Format("Самый высокий показатель по {0}", regionType));
                            break;
                        }
                    case 4:
                    case 8:
                    case 13:
                        {
                            CRHelper.SetHeaderCaption(UltraWebGrid, 0, i, String.Format("Наименование {0} с максимальным значением", regionType), 
                                String.Format("Наименование {0} с самым высоким показателем", regionType));
                            break;
                        }
                    case 9:
                        {
                            CRHelper.SetHeaderCaption(UltraWebGrid, 0, i,
                                String.Format("Темп роста (+), снижения (-) факта {0} года к факту {1} года, %", currentYear - 1, currentYear - 2), "");
                            break;
                        }
                    case 14:
                        {
                            CRHelper.SetHeaderCaption(UltraWebGrid, 0, i,
                                String.Format("Темп роста (+), снижения (-) плана {0} года к факту {1} года, %", currentYear, currentYear - 1), "");
                            break;
                        }
                    case 15:
                        {
                            CRHelper.SetHeaderCaption(UltraWebGrid, 0, i,
                                String.Format("Динамика изменения рейтинга плана {0} года к факту {1} года", currentYear, currentYear - 1), "");
                            break;
                        }
                }
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(230);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            CRHelper.AddHierarchyHeader(UltraWebGrid, 0, String.Format("Фактические показатели {0} года", currentYear - 2), 1, 1, 4, 1);
            CRHelper.AddHierarchyHeader(UltraWebGrid, 0, String.Format("Фактические показатели {0} года", currentYear - 1), 5, 1, 4, 1);
            CRHelper.AddHierarchyHeader(UltraWebGrid, 0, String.Format("Плановые показатели {0} года", currentYear), 10, 1, 4, 1);

            CRHelper.AddHierarchyHeader(UltraWebGrid, 0, "Показатели бюджета муниципального образования", 1, 0, 14, 1);
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            string indicatorName = String.Empty;
            if (e.Row.Cells[0] != null)
            {
                indicatorName = e.Row.Cells[0].ToString();
            }

            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                bool decimalColumn = (i == 1 || i == 3 || i == 5 || i == 7 || i == 10 || i == 12);
                bool rate = (i == 9 || i == 14);

                if (decimalColumn && e.Row.Cells[i].Value != null)
                {
                    string formatString = "N2";
                    switch (indicatorName)
                    {
                        case "Кол-во н/плательщиков по налогу на имущество ФЛ: доля в численности населения, %":
                        case "Кол-во н/плательщиков по налогу на имущество ФЛ: доля льготников, %":
                        case "Кол-во н/плательщиков ФЛ по земельному налогу: доля в численности населения, %":
                            {
                                formatString = "P1";
                                break;
                            }
                        case "Доля налоговых и неналоговых доходов в бюджете муниципального образования, %":
                        case "Доля налоговых и неналоговых доходов по всем местным бюджетам, %":
                            {
                                formatString = "P2";
                                break;
                            }
                        case "Кол-во н/плательщиков по налогу на имущество ФЛ":
                        case "Кол-во н/плательщиков по налогу на имущество ФЛ: из них кол-во льготников, чел.":
                        case "Кол-во н/плательщиков ФЛ по земельному налогу, чел.":
                        case "Кол-во плательщиков ЕНВД":
                            {
                                formatString = "N0";
                                break;
                            }
                        case "Процент площади земельных участков общей долевой собственности земель с/х назначения, поставленных на ГЗКУ":
                        case "Процент нулевых деклараций по ЕНВД":
                        case "Доля неклассифицированных земельных участков, %":
                        case "Площадь земельных участков, находящихся в государственной и муниципальной собственности и представленных в аренду, га":
                        case "Отношение среднемесячной зарплаты муниципальных служащих к среднемесячной зарплате работников крупных и средних предприятий":
                            {
                                formatString = "N1";
                                break;
                            }
                        case "Единый сельскохозяйственный налог в расчете на 1 жителя, руб./чел":
                        case "Площадь жилых помещений, приходящаяся на 1жителя, кв.м./чел":
                            {
                                formatString = "N2";
                                break;
                            }
                    }

                    e.Row.Cells[i].Value = Convert.ToDouble(e.Row.Cells[i].Value).ToString(formatString);
                }

                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Прирост к прошлому году";
                        }
                        else if (Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Снижение к прошлому году";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
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

        #region Комментарии к гриду

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }

        private void CommentTextDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0018_population");
            DataTable dtCommentText = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtCommentText);

            CommentText.Text = String.Empty;
            if (dtCommentText.Rows.Count > 0)
            {
                double population = GetDoubleDTValue(dtCommentText, "Численность постоянного населения района");
                double populationPercent = GetDoubleDTValue(dtCommentText, "Доля численности постоянного населения в области");

                CommentText.Text = String.Format("Численность населения на 01.01.{0}г.: <b>{1:N0} чел.</b>&nbsp;&nbsp;&nbsp;Доля в численности населения Самарской области: <b>{2:P2}</b>", 
                    currentYear, population, populationPercent);
            }
        }
        
        #endregion

        #region Экспорт в Excel

        private int startRowIndex = 9;

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 40;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
            SetApprovedSection(e.CurrentWorksheet, 2, 0, 2, "«Утверждаю»", "Заместитель министра управления финансами Самарской области",
                String.Format("«__»___________{0} г. _________________", currentYear));

            string regionStr = "Муниципальное образование";
            if (RegionsNamingHelper.LocalBudgetTypes.ContainsKey(ComboRegion.SelectedValue))
            {
                regionStr = regionType == "МР" ? "Муниципальный район" : "Городской округ";
            }

            SetApprovedSection(e.CurrentWorksheet, 2, 5, 3, "Паспорт доходов", String.Format("{1} {0} Самарской области", ComboRegion.SelectedValue, regionStr),
                String.Format("(на 1 января {0} г.)", currentYear));

            SetApprovedSection(e.CurrentWorksheet, 2, 13, 3, "«Согласовано»", String.Format("Глава {1} {0}", ComboRegion.SelectedValue, regionStr),
                String.Format("«__»___________{0} г. _________________", currentYear));

            string commentText = CommentText.Text.Replace("&nbsp;", " ");
            commentText = commentText.Replace("<b>", String.Empty);
            commentText = commentText.Replace("</b>", String.Empty);
            e.CurrentWorksheet.Rows[startRowIndex - 2].Cells[0].Value = commentText;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            e.CurrentWorksheet.Columns[0].Width = 300 * 30;
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 100 * 38;
            }

            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "0.00";
            e.CurrentWorksheet.Columns[14].CellFormat.FormatString = "0.00";

            //Цвета у колонок
            e.CurrentWorksheet.ImageBackground = null;
            for (int i = 9; i < rowsCount + 9; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.FillPatternBackgroundColor = Color.LightBlue;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.FillPatternForegroundColor = Color.LightBlue;
                e.CurrentWorksheet.Rows[i].Cells[2].CellFormat.FillPatternBackgroundColor = Color.LightYellow;
                e.CurrentWorksheet.Rows[i].Cells[2].CellFormat.FillPatternForegroundColor = Color.LightYellow;
                e.CurrentWorksheet.Rows[i].Cells[3].CellFormat.FillPatternBackgroundColor = Color.LightGray;
                e.CurrentWorksheet.Rows[i].Cells[3].CellFormat.FillPatternForegroundColor = Color.LightGray;
                e.CurrentWorksheet.Rows[i].Cells[4].CellFormat.FillPatternBackgroundColor = Color.LightPink;
                e.CurrentWorksheet.Rows[i].Cells[4].CellFormat.FillPatternForegroundColor = Color.LightPink;

                e.CurrentWorksheet.Rows[i].Cells[5].CellFormat.FillPatternBackgroundColor = Color.LightBlue;
                e.CurrentWorksheet.Rows[i].Cells[5].CellFormat.FillPatternForegroundColor = Color.LightBlue;
                e.CurrentWorksheet.Rows[i].Cells[6].CellFormat.FillPatternBackgroundColor = Color.LightYellow;
                e.CurrentWorksheet.Rows[i].Cells[6].CellFormat.FillPatternForegroundColor = Color.LightYellow;
                e.CurrentWorksheet.Rows[i].Cells[7].CellFormat.FillPatternBackgroundColor = Color.LightGray;
                e.CurrentWorksheet.Rows[i].Cells[7].CellFormat.FillPatternForegroundColor = Color.LightGray;
                e.CurrentWorksheet.Rows[i].Cells[8].CellFormat.FillPatternBackgroundColor = Color.LightPink;
                e.CurrentWorksheet.Rows[i].Cells[8].CellFormat.FillPatternForegroundColor = Color.LightPink;

                e.CurrentWorksheet.Rows[i].Cells[10].CellFormat.FillPatternBackgroundColor = Color.LightBlue;
                e.CurrentWorksheet.Rows[i].Cells[10].CellFormat.FillPatternForegroundColor = Color.LightBlue;
                e.CurrentWorksheet.Rows[i].Cells[11].CellFormat.FillPatternBackgroundColor = Color.LightYellow;
                e.CurrentWorksheet.Rows[i].Cells[11].CellFormat.FillPatternForegroundColor = Color.LightYellow;
                e.CurrentWorksheet.Rows[i].Cells[12].CellFormat.FillPatternBackgroundColor = Color.LightGray;
                e.CurrentWorksheet.Rows[i].Cells[12].CellFormat.FillPatternForegroundColor = Color.LightGray;
                e.CurrentWorksheet.Rows[i].Cells[13].CellFormat.FillPatternBackgroundColor = Color.LightPink;
                e.CurrentWorksheet.Rows[i].Cells[13].CellFormat.FillPatternForegroundColor = Color.LightPink;
            }
            e.CurrentWorksheet.Rows[8].Cells[1].CellFormat.FillPatternBackgroundColor = Color.LightSkyBlue;
            e.CurrentWorksheet.Rows[8].Cells[1].CellFormat.FillPatternForegroundColor = Color.LightSkyBlue;
            e.CurrentWorksheet.Rows[8].Cells[5].CellFormat.FillPatternBackgroundColor = Color.LightSkyBlue;
            e.CurrentWorksheet.Rows[8].Cells[5].CellFormat.FillPatternForegroundColor = Color.LightSkyBlue;
            e.CurrentWorksheet.Rows[8].Cells[10].CellFormat.FillPatternBackgroundColor = Color.LightSkyBlue;
            e.CurrentWorksheet.Rows[8].Cells[10].CellFormat.FillPatternForegroundColor = Color.LightSkyBlue;

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[startRowIndex - 1].Height = 17 * 30;
                e.CurrentWorksheet.Rows[startRowIndex - 1].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[startRowIndex - 1].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[startRowIndex - 1].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[startRowIndex - 1].Cells[i].CellFormat.Font.Height = 145;

                e.CurrentWorksheet.Rows[startRowIndex].Height = 35 * 30;
                e.CurrentWorksheet.Rows[startRowIndex].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[startRowIndex].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[startRowIndex].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[startRowIndex].Cells[i].CellFormat.Font.Height = 145;
            }

            // расставляем стили у начальных колонок
            for (int i = startRowIndex + 1; i < rowsCount + startRowIndex + 1; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 20 * 35;//Ширина строк
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Font.Height = 145;
                for (int j = 1; j < columnCount; j++)
                {
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.Alignment = HorizontalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.Font.Height = 145;                     
                }
                e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
                e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
                e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
                e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
              
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = startRowIndex;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            switch (e.CurrentColumnIndex)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    {   
                        e.HeaderText = String.Format("Фактические показатели {0} года", currentYear - 2);
                        break;
                    }
                case 5:
                case 6:
                case 7:
                case 8:
                    {
                        e.HeaderText = String.Format("Фактические показатели {0} года", currentYear - 1);
                        break;
                    }
                case 10:
                case 11:
                case 12:
                case 13:
                    {
                        e.HeaderText = String.Format("Плановые показатели {0} года", currentYear);
                        break;
                    }
                default:
                    {
                        e.HeaderText = UltraWebGrid.Bands[0].Columns[e.CurrentColumnIndex].Header.Caption;
                        break;
                    }
            }
        }
 
        private static void SetApprovedSection(Worksheet sheet, int rowIndex, int cellIndex, int spanCells, string action, string person, string date)
        {
            sheet.Rows[rowIndex].Cells[cellIndex].Value = String.Format("{0}", action);
            sheet.Rows[rowIndex].Cells[cellIndex].CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            sheet.Rows[rowIndex].Cells[cellIndex].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            sheet.Rows[rowIndex].Cells[cellIndex].CellFormat.Alignment = HorizontalCellAlignment.Center;
            sheet.MergedCellsRegions.Add(rowIndex, cellIndex, rowIndex, cellIndex + spanCells - 1);

            sheet.Rows[rowIndex + 1].Cells[cellIndex].Value = String.Format("{0}", person);
            sheet.Rows[rowIndex + 1].Cells[cellIndex].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            sheet.Rows[rowIndex + 1].Cells[cellIndex].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
            sheet.Rows[rowIndex + 1].Cells[cellIndex].CellFormat.Alignment = HorizontalCellAlignment.Center;
            sheet.Rows[rowIndex + 1].Height = 27 * 30;
            sheet.MergedCellsRegions.Add(rowIndex + 1, cellIndex, rowIndex + 1, cellIndex + spanCells - 1);

            sheet.Rows[rowIndex + 2].Cells[cellIndex].Value = date;
            sheet.Rows[rowIndex + 2].Cells[cellIndex].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            sheet.Rows[rowIndex + 2].Cells[cellIndex].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            sheet.Rows[rowIndex + 2].Cells[cellIndex].CellFormat.Alignment = HorizontalCellAlignment.Center;
            sheet.MergedCellsRegions.Add(rowIndex + 2, cellIndex, rowIndex + 2, cellIndex + spanCells - 1);

        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
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
        
        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        #endregion
    }
}
