using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.Shared;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0010_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2009;
        private int endYear = 2011;
        private string endQuarter = "1 Квартал";
        #endregion

        #region Параметры запроса

        // выбранный код территории
        private CustomParam selectedRegionCode;
        // выбранный тип территории
        private CustomParam selectedRegionType;
        // выбранная территория
        private CustomParam selectedRegion;
        // выбранная множество территорий
        private CustomParam selectedRegionSet;
        // выбранный основной показатель
        private CustomParam selectedMainIndicator;
        // выбранный побочный показатель
        private CustomParam selectedDetailIndicator;
        // выбранный вариант
        private CustomParam selectedVariant;
        // мобилизация доходов
        private CustomParam incomesMobilization;
        // знак для множества показателей
        private CustomParam indicatorSign;
        // выбран элемент "Бюджеты поселений - Итого"
        private CustomParam settelmentTotalSelected;

        // уровень МР и ГО
        private CustomParam regionsLevel;
        // уровень поселений
        private CustomParam settlementLevel;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 250);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.EnableViewState = false;

            #region Инициализация параметров запроса

            if (selectedRegionCode == null)
            {
                selectedRegionCode = UserParams.CustomParam("selected_region_code");
            }
            if (selectedRegionType == null)
            {
                selectedRegionType = UserParams.CustomParam("selected_region_type");
            }
            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (selectedRegionSet == null)
            {
                selectedRegionSet = UserParams.CustomParam("selected_region_set");
            }
            if (selectedMainIndicator == null)
            {
                selectedMainIndicator = UserParams.CustomParam("selected_main_indicator");
            }
            if (selectedDetailIndicator == null)
            {
                selectedDetailIndicator = UserParams.CustomParam("selected_detail_indicator");
            }
            if (selectedVariant == null)
            {
                selectedVariant = UserParams.CustomParam("selected_variant");
            }
            if (incomesMobilization == null)
            {
                incomesMobilization = UserParams.CustomParam("incomes_mobilization");
            }
            if (indicatorSign == null)
            {
                indicatorSign = UserParams.CustomParam("indicator_sign");
            }
            if (settelmentTotalSelected == null)
            {
                settelmentTotalSelected = UserParams.CustomParam("settelment_total_selected");
            }

            regionsLevel = UserParams.CustomParam("region_level");
            settlementLevel = UserParams.CustomParam("settlement_level");

            #endregion

            UltraGridExporter1.PdfExportButton.Visible = false;
            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            settlementLevel.Value = RegionSettingsHelper.Instance.SettlementLevel;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0010_0001_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                endQuarter = GetParamQuarter(dtDate.Rows[0][1].ToString());

                ComboYear.Title = "Год";
                ComboYear.Width = 70;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboQuarter.Title = "Квартал";
                ComboQuarter.Width = 110;
                ComboQuarter.MultiSelect = false;
                ComboQuarter.FillDictionaryValues(CustomMultiComboDataHelper.FillMOFOQuaters());
                ComboQuarter.SetСheckedState(endQuarter, true);

                ComboRegionCode.Title = "Код";
                ComboRegionCode.Width = 80;
                ComboRegionCode.ParentSelect = true;
                ComboRegionCode.MultiSelect = false;
                ComboRegionCode.FillDictionaryValues(CustomMultiComboDataHelper.FillRegionCodeList());
                ComboRegionCode.SetСheckedState("Все ", true);

                ComboRegionType.Title = "Тип";
                ComboRegionType.Width = 80;
                ComboRegionType.ParentSelect = true;
                ComboRegionType.MultiSelect = false;
                ComboRegionType.FillDictionaryValues(CustomMultiComboDataHelper.FillRegionTypeList());
                ComboRegionType.SetСheckedState("Все ", true);

                ComboRegion.Visible = true;
                ComboRegion.Title = "Территории";
                ComboRegion.Width = 270;
                ComboRegion.ParentSelect = true;
                ComboRegion.MultiSelect = false;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillMOFORegionsList(DataDictionariesHelper.MOFORegionsTypes));
                ComboRegion.SetСheckedState("Бабаевский МР", true);

                ComboMainIndicator.Title = "Показатели";
                ComboMainIndicator.Width = 240;
                ComboMainIndicator.ParentSelect = true;
                ComboMainIndicator.MultiSelect = false;
                ComboMainIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillAdditionalActivityMainLevels(DataDictionariesHelper.AdditionalActivityUniqNames, 
                    DataDictionariesHelper.AdditionalActivityLevels));
                ComboMainIndicator.SetСheckedState("Все показатели", true);

                ComboDetailIndicator.Title = "Мероприятия";
                ComboDetailIndicator.Width = 240;
                ComboDetailIndicator.ParentSelect = true;
                ComboDetailIndicator.MultiSelect = false;
                ComboDetailIndicator.FillDictionaryValues(CustomMultiComboDataHelper.FillAdditionalActivityDetailLevels(DataDictionariesHelper.AdditionalActivityUniqNames,
                    DataDictionariesHelper.AdditionalActivityLevels));
                ComboDetailIndicator.SetСheckedState("Все мероприятия", true);
            }

            Page.Title = "Отчет о выполнении программы мероприятий по увеличению доходной базы бюджета";
            PageTitle.Text = Page.Title;

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            int quarterNum = ComboQuarter.SelectedIndex + 1;

            switch(quarterNum)
            {
                case 1:
                    {
                        PageSubTitle.Text = string.Format("за {1} квартал {0} года", yearNum, quarterNum);
                        break;
                    }
                case 2:
                    {
                        PageSubTitle.Text = string.Format("за 1 полугодие {0} года", yearNum);
                        break;
                    }

                case 3:
                    {
                        PageSubTitle.Text = string.Format("за 9 месяцев {0} года", yearNum);
                        break;
                    }
                case 4:
                    {
                        PageSubTitle.Text = string.Format("за {0} год", yearNum);
                        break;
                    }
                    
            }
            
            CommentTextLabel.Text = string.Format("{0}, {1}, {2}", ComboRegion.SelectedValue, ComboMainIndicator.SelectedValue, ComboDetailIndicator.SelectedValue);

            selectedRegionCode.Value = ComboRegionCode.SelectedValue.TrimEnd(' ');
            selectedRegionType.Value = ComboRegionType.SelectedValue.TrimEnd(' ');

            settelmentTotalSelected.Value = "false";
            if (DataDictionariesHelper.MOFORegionsUniqueNames.ContainsKey(ComboRegion.SelectedValue))
            {
                selectedRegionSet.Value = string.Format(
                        @"{{ {0} }} + {{{0}.DataMember}} + {{[Районы].[Сопоставимый].[Все районы].[Бюджеты поселений]}} +
                                                                  Descendants               
                                                                  (
                                                                     {0},
                                                                     {1},
                                                                     SELF               
                                                                  )",
                        DataDictionariesHelper.MOFORegionsUniqueNames[ComboRegion.SelectedValue], settlementLevel.Value);

                selectedRegion.Value = DataDictionariesHelper.MOFORegionsUniqueNames[ComboRegion.SelectedValue];
            }
            else
            {
                switch (ComboRegion.SelectedValue)
                {
                    case "Местные бюджеты – с разбивкой":
                        {
                            selectedRegionSet.Value = "[Все муниципальные образования]";
                            break;
                        }
                    case "Консолид.бюджеты районов":
                        {
                            selectedRegionSet.Value = "{[Районы].[Сопоставимый].[Консолид.бюджеты районов]} + [Муниципальные районы]";
                            break;
                        }
                    case "Городские округа ":
                        {
                            selectedRegionSet.Value = "{[Районы].[Сопоставимый].[Все районы].[Бюджеты ГО – Всего]} + [Городские округа]";
                            break;
                        }
                    case "Бюджеты поселений – Всего":
                        {
                            selectedRegionSet.Value = "{[Районы].[Сопоставимый].[Все районы].[Бюджеты поселений – Всего]} + [Муниципальные образования]";
                            settelmentTotalSelected.Value = "true";
                            break;
                        }
                    default:
                        {
                            selectedRegionSet.Value = string.Format("[Районы].[Сопоставимый].[{0}]", ComboRegion.SelectedValue);
                            break;
                        }
                }
                selectedRegion.Value = "[Районы].[Сопоставимый].[Все районы].[Вологодская область]";
            }

            if (ComboMainIndicator.SelectedValue == "Все показатели")
            {
                selectedMainIndicator.Value = @"Descendants
                                                (
                                                    [Показатели].[ДопМероприятия_Сопоставимый].[Все],
                                                    [Показатели].[ДопМероприятия_Сопоставимый].[Показатели уровень 2],
                                                    SELF
                                                )";
            }
            else
            {
                selectedMainIndicator.Value = DataDictionariesHelper.AdditionalActivityUniqNames[ComboMainIndicator.SelectedValue];
            }

            // для мобилизации вычитаем множество мероприятий
            indicatorSign.Value = (ComboMainIndicator.SelectedValue != "МОБИЛИЗАЦИЯ ДОХОДОВ") ? "+" : "-";

            if (ComboDetailIndicator.SelectedValue == "Все мероприятия")
            {
                selectedDetailIndicator.Value = "true ";
            }
            else
            {
                selectedDetailIndicator.Value = string.Format("[Показатели].[ДопМероприятия_Сопоставимый].CurrentMember.Name = \"{0}\"", ComboDetailIndicator.SelectedValue);
            }

            incomesMobilization.Value = ComboMainIndicator.SelectedValue != "Все показатели" || ComboDetailIndicator.SelectedValue != "Все мероприятия"
                                            ? " "
                                            : "{[Показатели].[ДопМероприятия_Сопоставимый].[Все].[МОБИЛИЗАЦИЯ ДОХОДОВ]} + ";
            selectedVariant.Value = GetClassQuarter(ComboQuarter.SelectedValue);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        private static string GetClassQuarter(string paramQuarter)
        {
            switch(paramQuarter)
            {
                case "Полугодие":
                    {
                        return "2 квартал";
                    }
                case "9 месяцев":
                    {
                        return "3 квартал";
                    }
                default:
                    {
                        return paramQuarter;
                    }
            }
        }

        private static string GetParamQuarter(string classQuarter)
        {
            switch (classQuarter)
            {
                case "2 квартал":
                    {
                        return "Полугодие";
                    }
                case "3 квартал":
                    {
                        return "9 месяцев";
                    }
                default:
                    {
                        return classQuarter;
                    }
            }
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0010_0001_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Полномочия", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                if (dtGrid.Columns.Count > 0)
                {
                    dtGrid.Columns.RemoveAt(0);
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
            e.Layout.RowAlternateStylingDefault = DefaultableBoolean.False;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "";
                int widthColumn = 100;
                bool wrapText = false;
                bool mergeCells = false;
                HorizontalAlign align = HorizontalAlign.Right;
                VerticalAlign vAlign = VerticalAlign.Middle;

                switch(i)
                {
                    case 0:
                    case 1:
                        {
                            widthColumn = 40;
                            break;
                        }
                    case 2:
                        {
                            wrapText = true;
                            mergeCells = true;
                            widthColumn = 130;
                            align = HorizontalAlign.Left;
                            vAlign = VerticalAlign.Top;
                            break;
                        }
                    case 3:
                        {
                            wrapText = true;
                            mergeCells = true;
                            widthColumn = 150;
                            align = HorizontalAlign.Left;
                            vAlign = VerticalAlign.Top;
                            break;
                        }
                    case 4:
                        {
                            wrapText = true;
                            mergeCells = false;
                            widthColumn = 230;
                            align = HorizontalAlign.Left;
                            vAlign = VerticalAlign.Top;
                            break;
                        }
                    case 5:
                        {
                            wrapText = false;
                            mergeCells = false;
                            widthColumn = 70;
                            formatString = "N2";
                            break;
                        }
                    case 6:
                    case 7:
                    case 8:
                        {
                            wrapText = false;
                            mergeCells = false;
                            widthColumn = 120;
                            formatString = "N2";
                            break;
                        }
                    case 9:
                    case 10:
                    case 11:
                        {
                            wrapText = false;
                            mergeCells = false;
                            widthColumn = 110;
                            formatString = "P2";
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = align;
                e.Layout.Bands[0].Columns[i].CellStyle.VerticalAlign = vAlign;
                e.Layout.Bands[0].Columns[i].CellStyle.Wrap = wrapText;
                e.Layout.Bands[0].Columns[i].MergeCells = mergeCells;
            }

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int levelColumnIndex = e.Row.Cells.Count - 1;
            int codeIndex = 0;
            int regionNameIndex = 2;

            string level = string.Empty;
            if (e.Row.Cells[levelColumnIndex].Value != null)
            {
                level = e.Row.Cells[levelColumnIndex].Value.ToString();
            }

            string code = string.Empty;
            if (e.Row.Cells[codeIndex].Value != null)
            {
                code = e.Row.Cells[codeIndex].Value.ToString();
            }

            string regionName = string.Empty;
            if (e.Row.Cells[regionNameIndex].Value != null)
            {
                regionName = e.Row.Cells[regionNameIndex].Value.ToString();
            }

            if (code.Contains("кон") || code.Contains("ГСП"))
            {
                e.Row.Cells[regionNameIndex].Value = regionName.Replace("муниципальный", string.Empty);
            }

            for (int i = 2; i < e.Row.Cells.Count; i++)
            {
                switch(level)
                {
                    case "Показатели уровень 1":
                    case "Показатели уровень 2":
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                            break;
                        }
                    case "Показатели уровень 3":
                        {
                            e.Row.Cells[i].Style.Font.Italic = true;
                            break;
                        }
                    case "Показатели уровень 05":
                        {
                            e.Row.Cells[i].Style.Padding.Left = 10;
                            break;
                        }
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
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            for (int i = 0; i < columnCount; i++)
            {
                switch(i)
                {
                    case 0:
                    case 1:
                        {
                            e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "";
                            e.CurrentWorksheet.Columns[i].Width = 50 * 37;
                            break;
                        }
                    case 2:
                    case 3:
                    case 4:
                        {
                            e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "";
                            e.CurrentWorksheet.Columns[i].Width = 200 * 37;
                            break;
                        }
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        {
                            e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,###0.00;[Red]-#,###0.00";
                            e.CurrentWorksheet.Columns[i].Width = 120 * 37;
                            break;
                        }
                    default:
                        {
                            e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "0.00%";
                            e.CurrentWorksheet.Columns[i].Width = 100 * 37;
                            break;
                        }
                }
            }

            // расставляем стили у начальных колонок
            for (int i = 5; i < rowsCount + 5; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 17* 37;
                e.CurrentWorksheet.Rows[i].Cells[2].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[2].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                e.CurrentWorksheet.Rows[i].Cells[2].CellFormat.Alignment = HorizontalCellAlignment.Left;

                e.CurrentWorksheet.Rows[i].Cells[3].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[3].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                e.CurrentWorksheet.Rows[i].Cells[3].CellFormat.Alignment = HorizontalCellAlignment.Left;

                e.CurrentWorksheet.Rows[i].Cells[4].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[4].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
                e.CurrentWorksheet.Rows[i].Cells[4].CellFormat.Alignment = HorizontalCellAlignment.Left;
            }

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 20 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[4].Height = 20 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
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
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Key;
        }

        #endregion
    }
}
