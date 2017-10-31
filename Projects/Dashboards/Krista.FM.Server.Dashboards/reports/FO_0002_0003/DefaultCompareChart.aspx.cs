using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using System.Collections.ObjectModel;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0003
{
    public partial class DefaultCompareChart : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2006;
        private int endYear = 2011;
        private int rubMultiplier;
        private string FilterParams;

        private bool UseConsolidateRegionBudget
        {
            get { return useConsolidateRegionBudget.Checked; }
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

        private CustomParam selectedRegion;
        // доходы Итого
        private CustomParam incomesTotal;
        // уровень консолидированного бюджета
        private CustomParam regionsConsolidateLevel;
        // уровень МО
        private CustomParam regionsLevel;

        // тип документа СКИФ для консолидированного бюджета субъекта
        private CustomParam consolidateBudgetDocumentSKIFType;
        // тип документа СКИФ для районов
        private CustomParam regionBudgetDocumentSKIFType;
        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;
        // Вид дохода
        private CustomParam kindOfIncomes;

        // тип документа СКИФ для местных бюджетов
        private CustomParam localBudgetDocumentSKIFType;
        // уровень бюджета СКИФ для местных бюджетов
        private CustomParam localBudgetSKIFLevel;
        
        #endregion
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (selectedRegion == null)
            {
                selectedRegion = UserParams.CustomParam("selected_region");
            }
            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }
            if (regionsConsolidateLevel == null)
            {
                regionsConsolidateLevel = UserParams.CustomParam("regions_consolidate_level");
            }

            if (consolidateBudgetDocumentSKIFType == null)
            {
                consolidateBudgetDocumentSKIFType = UserParams.CustomParam("consolidate_budget_document_skif_type");
            }
            if (regionBudgetDocumentSKIFType == null)
            {
                regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            }
            if (regionBudgetSKIFLevel == null)
            {
                regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            }
            if (kindOfIncomes == null)
            {
                kindOfIncomes = UserParams.CustomParam("kind_Of_Incomes");
            }
            regionsLevel = UserParams.CustomParam("regions_level");

            localBudgetDocumentSKIFType = UserParams.CustomParam("local_budget_document_skif_type");
            localBudgetSKIFLevel = UserParams.CustomParam("local_budget_skif_level");

            #endregion

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 25);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 160);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.5 - 160);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.StackAreaChart;
            UltraChart.Axis.X.Extent = 100;
            UltraChart.Axis.X.Labels.SeriesLabels.Font = new Font("Verdana", 8);
            UltraChart.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart.Axis.Y.Extent = IsThsRubSelected ? 70 : 50;
            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Font = new Font("Verdana", 8);
            UltraChart.Legend.Location = LegendLocation.Bottom;
            UltraChart.Legend.SpanPercentage = 25;
            //UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value / 3);

            UltraChart.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart.Border.Thickness = 0;
            UltraChart.Data.SwapRowsAndColumns = true;

            UltraChart.TitleLeft.Visible = true;
            UltraChart.TitleLeft.Font = new Font("Verdana", 8);
            UltraChart.TitleLeft.Text = RubMultiplierCaption;
            UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent + (UltraChart.Legend.SpanPercentage * Convert.ToInt32(UltraChart.Height.Value)) / 100;

            UltraChart.Tooltips.FormatString = String.Format("<ITEM_LABEL>\n<DATA_VALUE_ITEM:N1> {0}", RubMiltiplierButtonList.SelectedValue);

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Diamond;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            UltraChart.AreaChart.LineAppearances.Add(lineAppearance);

            UltraChart.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            #endregion


            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            UltraGridExporter1.MultiHeader = true;

            CrossLink1.Text = "Сравнение&nbsp;темпа&nbsp;роста&nbsp;фактических&nbsp;доходов";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0003/DefaultCompare.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;

            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKD30000000000000000.Value = RegionSettingsHelper.Instance.IncomesKD30000000000000000;

            UserParams.IncomesKD11402000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11402000000000000;
            UserParams.IncomesKD11403000000000410.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000410;
            UserParams.IncomesKD11403000000000440.Value = RegionSettingsHelper.Instance.IncomesKD11403000000000440 !=
                                                          "null"
                                                              ? string.Format(",{2}.[{1}].[ДОХОДЫ ОТ ПРОДАЖИ МАТЕРИАЛЬНЫХ И НЕМАТЕРИАЛЬНЫХ АКТИВОВ].[{0}],",
                                                                    RegionSettingsHelper.Instance.IncomesKD11403000000000440,
                                                                    RegionSettingsHelper.Instance.IncomesKDRootName,
                                                                    RegionSettingsHelper.Instance.IncomesKDAllLevel)
                                                              : ",";
            UserParams.IncomesKD11406000000000430.Value = RegionSettingsHelper.Instance.IncomesKD11406000000000430;
            UserParams.RegionsLocalBudgetLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionsLocalBudgetLevel");

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0003_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboRegion.Title = "Бюджет";
                ComboRegion.Width = 400;
                ComboRegion.ParentSelect = true;
                ComboRegion.MultiSelect = false;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillLocalBudgets(RegionsNamingHelper.LocalBudgetTypes));
                ComboRegion.SetСheckedState("Консолидированный бюджет субъекта", true);

                ComboIncomes.Width = 420;
                ComboIncomes.Title = "Вид дохода";
                ComboIncomes.MultiSelect = true;
                ComboIncomes.MultipleSelectionType = MultipleSelectionType.SimpleMultiple;
                ComboIncomes.ParentSelect = true;
                ComboIncomes.FillDictionaryValues(CustomMultiComboDataHelper.FillFullKDIncludingMultipleList());
                ComboIncomes.SetСheckedState("Налоговые доходы ", true);
                ComboIncomes.SetСheckedState("Неналоговые доходы ", true);
                ComboIncomes.SetСheckedState("Безвозмездные поступления ", true);
            }

            bool regionSelected = false;
            if (RegionsNamingHelper.LocalBudgetTypes.ContainsKey(ComboRegion.SelectedValue))
            {
                regionSelected = RegionsNamingHelper.LocalBudgetTypes[ComboRegion.SelectedValue] == "МР";
            }
            useConsolidateRegionBudget.Visible = regionSelected;

            string selectedBudget = UseConsolidateRegionBudget && regionSelected
                            ? String.Format("{0} (Консолидированный бюджет МО)", ComboRegion.SelectedValue)
                            : String.Format("{0}", ComboRegion.SelectedValue);

            Page.Title = String.Format("Структурная динамика фактических доходов ({0})", selectedBudget);
            Label1.Text = Page.Title;
            
            switch (ComboRegion.SelectedValue)
            {
                case "Консолидированный бюджет субъекта":
                    {
                        selectedRegion.Value = String.Format("{0}.[Консолидированный бюджет субъекта ]", RegionSettingsHelper.Instance.RegionDimension);
                        break;
                    }
                case "Муниципальные районы":
                    {
                        selectedRegion.Value =  String.Format("{0}.[Муниципальные районы ]", RegionSettingsHelper.Instance.RegionDimension);
                        break;
                    }
                case "Городские округа":
                    {
                        selectedRegion.Value = String.Format("{0}.[Городские округа ]", RegionSettingsHelper.Instance.RegionDimension);
                        break;
                    }
                default:
                    {
                        selectedRegion.Value = RegionsNamingHelper.LocalBudgetUniqueNames[ComboRegion.SelectedValue];
                        break;
                    }
            }

            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodEndYear.Value = yearNum.ToString();
            UserParams.PeriodYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodFirstYear.Value = (yearNum - 2).ToString();
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            regionsConsolidateLevel.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            
            string kdList = String.Empty;
            foreach (string kd in ComboIncomes.SelectedValues)
            {
                kdList += String.Format("{0}.[{1}],", UserParams.IncomesKDDimension.Value, kd);
            }
            kindOfIncomes.Value = kdList.TrimEnd(',');
            
            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");

            localBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetDocumentSKIFType");
            localBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetSKIFLevel");

            regionBudgetSKIFLevel.Value = UseConsolidateRegionBudget && regionSelected
                ? RegionSettingsHelper.Instance.GetPropertyValue("ConsMOBudgetSKIFLevel")
                : RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;
            
            UltraWebGrid.Bands.Clear();
            if  (kindOfIncomes.Value != String.Empty)
            {
                UltraChart.DataBind();
                UltraWebGrid.DataBind();
            }
            FilterParams = string.Format("за {0} год, бюджет: {1}, {2}", ComboYear.SelectedValue, ComboRegion.SelectedValue, RubMiltiplierButtonList.SelectedValue);
        }

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0003_compare_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dtChart);

            foreach (DataColumn column in dtChart.Columns)
            {
                column.ColumnName = column.ColumnName.TrimEnd('_');
            }

            DataTable dt = new DataTable();
            query = DataProvider.GetQueryText("FO_0002_0003_compare_chart");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dt);

            List<string> yearList = new List<string>();
            if (dtChart.Rows.Count > 0)
            {
                for (int j = 0; j < dtChart.Rows.Count; j++)
                {
                    DataRow row = dtChart.Rows[j];
                    // если строка первая, либо в первой ячейке Январь, либо предыдущий Декабрь
                    if (row[0] != DBNull.Value && dt.Rows[j][0] != DBNull.Value &&
                            (j == 0 ||
                             row[0].ToString() == "Январь" ||
                             (j != 0 && dtChart.Rows[j - 1][0].ToString() == "Декабрь")))
                    {
                        row[0] = string.Format("{0} - {1}", dt.Rows[j][0], row[0]);
                        yearList.Add(dt.Rows[j][0].ToString());
                    }
                    for (int i = 1; i < row.ItemArray.Length; i++)
                    {
                        if (row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) / rubMultiplier;
                        }
                    }
                }

                if (yearList.Count == 1)
                {
                    Label2.Text = string.Format("Сравнение структуры помесячного поступления доходов за {0} год",
                        yearList[0]);
                }
                else if (yearList.Count > 1)
                {
                    Label2.Text = string.Format("Сравнение структуры помесячного поступления доходов за {0} - {1} годы",
                        yearList[0], yearList[yearList.Count - 1]);
                }

                UltraChart.DataSource = dtChart;
            }
            else
            {
                UltraChart.DataSource = null;
            }
        }

        #endregion

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dtGrid = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0003_compare_addGrid");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                DataTable newDt = dtGrid.Clone();

                string curYear = string.Empty;
                foreach (DataRow row in dtGrid.Rows)
                {
                    string year = (row[1] != DBNull.Value) ? row[1].ToString() : string.Empty;
                    if (curYear != year)
                    {
                        DataRow newRow = newDt.NewRow();
                        newRow[0] = RegionsNamingHelper.CheckMultiplyValue(year, 3);
                        newDt.Rows.Add(newRow);
                        curYear = year;
                    }
                    for (int i = 2; i < row.ItemArray.Length; i++)
                    {
                        if (i%2 == 0 && row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i])/rubMultiplier;
                        }
                        else if (row[i] != DBNull.Value && row[i].ToString() != string.Empty &&
                                 dtGrid.Columns[i].DataType == typeof (string))
                        {
                            row[i] = (Convert.ToDouble(row[i].ToString())).ToString("N4");
                        }
                    }
                    newDt.ImportRow(row);
                }
                newDt.Columns.RemoveAt(1);
                newDt.AcceptChanges();

                UltraWebGrid.DataSource = newDt;
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
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(90);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 25;

                int j = i % 2;
                switch (j)
                {
                    case 0:
                        {
                            formatString = "P1";
                            widthColumn = 90;
                            break;
                        }
                    case 1:
                        {
                            formatString = "N1";
                            widthColumn = 90;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].Header.Style.Wrap = true;
            }

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

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 2)
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                string headerCaption = (captions.Length > 0) ? captions[0] : string.Empty;

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, String.Format("Исполнено, {0}", RubMiltiplierButtonList.SelectedValue), "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, "Доля", "Удельный вес в общей сумме доходов");

                CRHelper.AddHierarchyHeader(e.Layout.Grid,
                                                      0,
                                                      headerCaption,
                                                      multiHeaderPos,
                                                      0,
                                                      2,
                                                      1);
                multiHeaderPos += 2;
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            bool isYear = false;
            if (e.Row.Cells[0] != null && e.Row.Cells[0].Value.ToString() != string.Empty)
            {
                Decimal value;
                isYear = Decimal.TryParse(e.Row.Cells[0].Value.ToString(), out value);
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                UltraGridCell c = e.Row.Cells[i];
                if (c.Value != null && c.Value.ToString() != string.Empty)
                {
                    if (isYear)
                    {
                        c.Style.Font.Bold = true;
                        c.ColSpan = e.Row.Band.Columns.Count;
                    }

                    decimal value;
                    if (decimal.TryParse(c.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            c.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
          
           
        }
        private int hiddenOffset;
        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
//            UltraGridColumn col = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + hiddenOffset];
//            while (col.Hidden)
//            {
//                hiddenOffset++;
//                col = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + hiddenOffset];
//            }
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {

            int columnCount = UltraWebGrid.Columns.Count;

            e.CurrentWorksheet.Columns[0].Width = 300 * 37;
            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
            e.CurrentWorksheet.Columns[1].Width = 90 * 37;
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.0;[Red]-#,##000";
            e.CurrentWorksheet.Columns[2].Width = 90 * 37;
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[3].Width = 90 * 37;
            
            for (int i = 4; i < columnCount; i = i + 1)
            {
                string formatString = UltraGridExporter.ExelNumericFormat;
                int columnWidth = 70;

                int j = (i - 4) % 5;
                switch (j)
                {
                    case 0:
                    case 1:
                        {
                            formatString = "#,##0.0;[Red]-#,##0.0";
                            columnWidth = 110;
                            break;
                        }
                    case 2:
                        {
                            formatString = "#,##0.00;[Red]-#,##0.00";
                            columnWidth = 100;
                            break;
                        }
                    case 3:
                    case 4:
                        {
                            formatString = "#,##0.00;[Red]-#,##0.00";
                            columnWidth = 85;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = columnWidth * 37;
            }
            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = UltraGridExporter.ExelNumericFormat;
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = UltraGridExporter.ExelNumericFormat;
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[4].Height = 17 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            hiddenOffset = 0;
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Структура");
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Worksheet sheet2 = workbook.Worksheets.Add("Сравнение структуры");
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            sheet2.Rows[0].Cells[0].Value = Label1.Text;
            sheet2.Rows[1].Cells[0].Value = Label2.Text;
            UltraGridExporter.ChartExcelExport(sheet2.Rows[3].Cells[0], UltraChart);
            sheet1.Rows[0].Cells[0].Value = Label1.Text;
            sheet1.Rows[1].Cells[0].Value = sheet2.Rows[1].Cells[0].Value;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.GridElementCaption = Label2.Text;
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
            UltraGridExporter1.HeaderChildCellHeight = 60;
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);
           
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {

            
            e.Section.AddPageBreak();
            IText title = e.Section.AddText();
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.Style.Font.Bold = true;
            title.AddContent(Label2.Text);

            ITable table = e.Section.AddTable();
            ITableRow row = table.AddRow();
            ITableCell cell = row.AddCell();
            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            cell.Width = new FixedWidth((float)UltraChart.Width.Value + 5);
            cell.AddImage(UltraGridExporter.GetImageFromChart(UltraChart));

        }

        #endregion
    }
}
