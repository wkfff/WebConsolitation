using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Core;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.TOC;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Band;
using System.IO;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using InitializeRowEventHandler = Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;
using SerializationFormat = Dundas.Maps.WebControl.SerializationFormat;
using System.Globalization;
using Infragistics.Documents.Reports.Report.List;
using System.Drawing;
using System;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Krista.FM.Server.Dashboards.Components;
using System.Globalization;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Graphics;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Band;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.List;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.TOC;
using Infragistics.UltraChart.Shared.Events;
using System.Collections;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0003
{
    public partial class DefaultCompare : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear;
        private int endYear = 2011;
        private string month = "Январь";
        private bool internalCirculatoinExtrude = false;
        private string kdListKind;
        private double avgValue = 0;
        private bool GrowRateRanking
        {
            get { return Convert.ToBoolean(growRateRanking.Value); }
        }

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        #region Параметры запроса

        // доходы Итого
        private CustomParam incomesTotal;
        // уровень МР и ГО
        private CustomParam regionsLevel;

        // тип документа СКИФ для консолидированного бюджета субъекта
        private CustomParam consolidateBudgetDocumentSKIFType;
        // тип документа СКИФ для районов
        private CustomParam regionBudgetDocumentSKIFType;
        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        // тип документа СКИФ для местных бюджетов
        private CustomParam localBudgetDocumentSKIFType;
        // уровень бюджета СКИФ для местных бюджетов
        private CustomParam localBudgetSKIFLevel;

        // консолидированный бюджет субъекта
        private CustomParam regionsConsolidateBudget;

        // выводить ранги для темпа роста
        private CustomParam growRateRanking;

        // элемент доходы итого
        private CustomParam incomesTotalItem;
        // элемент безвозмездные поступления
        private CustomParam gratuitousIncomesItem;

        // выбранный множитель рублей
        private CustomParam rubMultiplier;

        // множество кодов доходов
        private CustomParam kd_set;


        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            incomesTotal = UserParams.CustomParam("incomes_total");
            regionsLevel = UserParams.CustomParam("regions_level");
            consolidateBudgetDocumentSKIFType = UserParams.CustomParam("consolidate_budget_document_skif_type");
            regionBudgetDocumentSKIFType = UserParams.CustomParam("region_budget_document_skif_type");
            regionBudgetSKIFLevel = UserParams.CustomParam("region_budget_skif_level");
            regionsConsolidateBudget = UserParams.CustomParam("regions_consolidate_budget");
            growRateRanking = UserParams.CustomParam("grow_rate_ranking");
            incomesTotalItem = UserParams.CustomParam("incomes_total_item");
            gratuitousIncomesItem = UserParams.CustomParam("gratuitous_incomes_item");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            localBudgetDocumentSKIFType = UserParams.CustomParam("local_budget_document_skif_type");
            localBudgetSKIFLevel = UserParams.CustomParam("local_budget_skif_level");
            kd_set = UserParams.CustomParam("kd_set");

            #endregion

            growRateRanking.Value = RegionSettingsHelper.Instance.GetPropertyValue("GrowRateRanking");

            if (GrowRateRanking)
            {
                PopupInformer1.HelpPageUrl = "DefaultCompare_GrowRateRanking.html";
            }

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 250);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.InitializeLayout += GrowRateRanking
            ? new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout_withRanking)
            : new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;

            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += GrowRateRanking
            ? new EndExportEventHandler(ExcelExporter_EndExport_withRanking)
            : new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.MultiHeader = true;

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.97));
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.8);
            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.ColumnChart.SeriesSpacing = 1;
            UltraChart1.ColumnChart.ColumnSpacing = 1;
            UltraChart1.Axis.X.Extent = 140;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.X.Labels.SeriesLabels.Visible = false;
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.X.StripLines.PE.Fill = System.Drawing.Color.Gainsboro;
            UltraChart1.Axis.X.StripLines.PE.FillOpacity = 150;
            UltraChart1.Axis.X.StripLines.PE.Stroke = System.Drawing.Color.DarkGray;
            UltraChart1.Axis.X.StripLines.Interval = 2;
            UltraChart1.Axis.X.StripLines.Visible = true;
            UltraChart1.Axis.Y.Extent = 20;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.WrapText = true;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.X.Labels.ItemFormatString = "<ITEM_LABEL>";
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart1.Height.Value) / 4;
            UltraChart1.TitleLeft.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.ColorModel.ModelStyle = ColorModels.LinearRange;
            UltraChart1.ColorModel.ColorBegin = System.Drawing.Color.Green;
            UltraChart1.ColorModel.ColorEnd = System.Drawing.Color.Red;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 25);
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 25);
            ChartTextAppearance appearance = new ChartTextAppearance();
            appearance.Column = -2;
            appearance.Row = -2;
            appearance.VerticalAlign = StringAlignment.Far;
            appearance.ItemFormatString = "<DATA_VALUE:N1>%";
            appearance.ChartTextFont = new System.Drawing.Font("Verdana", 6);
            appearance.Visible = true;
            UltraChart1.ColumnChart.ChartText.Add(appearance);
            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>\n<SERIES_LABEL>\n<DATA_VALUE:N2>%";
            UltraChart1.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            CrossLink1.Text = "Структурная&nbsp;динамика&nbsp;фактических&nbsp;доходов";
            CrossLink1.NavigateUrl = "~/reports/FO_0002_0003/DefaultCompareChart.aspx";
            CrossLink1.Visible = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("ChartReportLinkVisible"));
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.RegionDimension.Value = RegionSettingsHelper.Instance.RegionDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.IncomesKD10800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD10800000000000000;
            UserParams.IncomesKD11700000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11700000000000000;
            internalCirculatoinExtrude = Convert.ToBoolean(RegionSettingsHelper.Instance.GetPropertyValue("InternalCirculationExtrude"));
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.IncomesKD30000000000000000.Value = RegionSettingsHelper.Instance.IncomesKD30000000000000000;
            firstYear = Convert.ToInt32(RegionSettingsHelper.Instance.GetPropertyValue("BeginPeriodYear"));
            //UserParams.RegionsLocalBudgetLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionsLocalBudgetLevel");

            kdListKind = RegionSettingsHelper.Instance.GetPropertyValue("KdListKind");

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0003_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                UserParams.PeriodDayFK.Value = dtDate.Rows[0][4].ToString();
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month, true);
            }

            Page.Title = "Темп роста доходов";
            Label1.Text = Page.Title;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            Label2.Text = String.Format("Сравнение темпов роста фактических доходов консолидированного бюджета субъекта, бюджета субъекта и местных бюджетов за {0} {1} {2} года",
            monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UserParams.PeriodYear.Value = ComboYear.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;

            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;
            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;

            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");
            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");
            localBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetDocumentSKIFType");
            localBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("LocalBudgetSKIFLevel");

            regionsConsolidateBudget.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;

            kd_set.Value = kdListKind == "FullList" ? "[Полный список кодов доходов]" : "[Краткий список кодов доходов]";

            incomesTotalItem.Value = internalCirculatoinExtrude
            ? "Доходы бюджета без внутренних оборотов "
            : "Доходы бюджета c внутренними оборотами ";
            gratuitousIncomesItem.Value = internalCirculatoinExtrude
            ? "Безвозмездные поступления без внутренних оборотов "
            : "Безвозмездные поступления c внутренними оборотами ";

            rubMultiplier.Value = IsThsRubSelected ? "1000" : "1000000";

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
            UltraChart1.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0003_compare_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Бюджеты", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                if (!GrowRateRanking)
                {
                    dtGrid.Columns.RemoveAt(4);
                    dtGrid.Columns.RemoveAt(4);
                }
                dtGrid.AcceptChanges();
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (UltraWebGrid != null && UltraWebGrid.Rows.Count < 30)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 60;
                int j = 0;
                if (i > 5)
                {
                    j = (i + 1) % 7;
                    switch (j)
                    {
                        case 0:
                        case 1:
                        case 6:
                            {
                                formatString = "N1";
                                widthColumn = 105;
                                break;
                            }
                        case 2:
                        case 3:
                        case 4:
                            {
                                formatString = "P2";
                                widthColumn = 85;
                                break;
                            }
                        case 5:
                            {
                                formatString = "P1";
                                widthColumn = 80;
                                break;
                            }

                    }
                }
                else
                {
                    j = (i - 1) % 5;
                    switch (j)
                    {
                        case 0:
                        case 1:
                        case 4:
                            {
                                formatString = "N1";
                                widthColumn = 100;
                                break;
                            }
                        case 2:
                            {
                                formatString = "P2";
                                widthColumn = 80;
                                break;
                            }
                        case 3:
                            {
                                formatString = "P1";
                                widthColumn = 85;
                                break;
                            }

                    }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 250;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

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
            int columnCount = 5;
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + columnCount)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0].TrimEnd('_');

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i,
                String.Format("Исполнено, {0}", RubMiltiplierButtonList.SelectedValue), "Фактическое исполнение нарастающим итогом с начала года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1,
                String.Format("Исполнено прошлый год, {0}", RubMiltiplierButtonList.SelectedValue), "Исполнено за аналогичный период прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "Темп роста к прошлому году, %", "Темп роста исполнения к аналогичному периоду прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "Удельный вес в общей сумме доходов, %", "Удельный вес в общей сумме доходов выбранного бюджета");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, "Удельный вес в общей сумме доходов в прошлом году, %", "Удельный вес в общей сумме фактических доходов в прошлом году");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 5, "Выполнение годовых назначений", "Процент выполнения годовых назначений");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 6, "Исполнено на душу населения", "Фактическое исполнение доходов на душу населения");
                if (i == 1)
                {
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "Выполнение годовых назначений", "Процент выполнения годовых назначений");
                    CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, "Исполнено на душу населения", "Фактическое исполнение доходов на душу населения");
                }
                else
                    columnCount = 7;
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                if (i == 1)
                {
                    multiHeaderPos += 5;
                }
                else
                    multiHeaderPos += 7;
                ch.RowLayoutColumnInfo.SpanX = (i == 1) ? 5 : 7;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeLayout_withRanking(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                string formatString = "N0";
                int widthColumn = 60;

                int j = (i - 1) % 9;
                switch (j)
                {
                    case 0:
                    case 1:
                    case 7:
                        {
                            formatString = "N1";
                            widthColumn = 90;
                            break;
                        }
                    case 2:
                        {
                            formatString = "N2";
                            widthColumn = 80;
                            break;
                        }
                    case 4:
                    case 5:
                        {
                            formatString = "N2";
                            widthColumn = 75;
                            break;
                        }
                    case 3:
                        {
                            formatString = "N0";
                            widthColumn = 75;
                            break;
                        }
                    case 6:
                        {
                            formatString = "P1";
                            widthColumn = 75;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 250;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

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

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 9)
            {
                ColumnHeader ch = new ColumnHeader(true);
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0].TrimEnd('_');

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i,
                String.Format("Исполнено, {0}", RubMiltiplierButtonList.SelectedValue), "Фактическое исполнение нарастающим итогом с начала года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1,
                String.Format("Исполнено прошлый год, {0}", RubMiltiplierButtonList.SelectedValue), "Исполнено за аналогичный период прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "Темп роста к прошлому году, %", "Темп роста исполнения к аналогичному периоду прошлого года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "Ранг МР по темпу роста", "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, "Доля, %", "Доля дохода в общей сумме доходов выбранного бюджета");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 5, "Доля в прошлом году, %", "Доля дохода в общей сумме фактических доходов в прошлом году");

                if (i == 1)
                {
                    e.Layout.Bands[0].Columns[i + 4].Hidden = true;
                    e.Layout.Bands[0].Columns[i + 5].Hidden = true;
                }
                e.Layout.Bands[0].Columns[i + 6].Hidden = true;

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 9;
                ch.RowLayoutColumnInfo.SpanX = (i == 1) ? 6 : 8;
                ch.Style.Wrap = true;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 5; i < e.Row.Cells.Count; i = i + 1)
            {
                int groupCount = GrowRateRanking ? 9 : 7;
                int groupIndex = (i + 1) % groupCount;

                bool growRateColumn = (groupIndex == 2);
                bool percentColumn = (GrowRateRanking && groupIndex == 4) || (!GrowRateRanking && groupIndex == 3);
                bool rankColumn = (GrowRateRanking && groupIndex == 3);

                if (growRateColumn && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) < 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Снижение к прошлому году";
                    }
                    else if (Convert.ToDouble(e.Row.Cells[i].Value) > 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Рост к прошлому году";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (percentColumn &&
                e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty &&
                e.Row.Cells[i + 1].Value != null && e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                {
                    double currValue = Convert.ToDouble(e.Row.Cells[i].Value);
                    double prevValue = Convert.ToDouble(e.Row.Cells[i + 1].Value);

                    if (currValue < prevValue)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                        e.Row.Cells[i].Title = "Доля сократилась с прошлого года";
                    }
                    else if (currValue > prevValue)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                        e.Row.Cells[i].Title = "Доля выросла с прошлого года";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (rankColumn)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 3].Value != null &&
                    e.Row.Cells[i].Value.ToString() != string.Empty &&
                    e.Row.Cells[i + 3].Value.ToString() != string.Empty)
                    {
                        double rank = Convert.ToInt32(e.Row.Cells[i].Value);
                        double badRank = Convert.ToInt32(e.Row.Cells[i + 3].Value);

                        if (rank == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = "Максимальный темп роста";
                        }
                        else if (rank == badRank)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = "Минимальный темп роста";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }

            if (e.Row.Cells[0].Value != null &&
            (e.Row.Cells[0].Value.ToString().ToLower().Contains("бюджет") || e.Row.Cells[0].Value.ToString().ToLower().Contains("область")))
            {
                foreach (UltraGridCell cell in e.Row.Cells)
                {
                    cell.Style.Font.Bold = true;
                }
            }

            foreach (UltraGridCell cell in e.Row.Cells)
            {
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
            }
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text axisText = (Text)primitive;
                    axisText.bounds.Width = 30;
                    axisText.labelStyle.VerticalAlign = StringAlignment.Near;
                    axisText.labelStyle.FontSizeBestFit = false;
                    axisText.labelStyle.Font = new System.Drawing.Font("Verdana", 8);
                    axisText.labelStyle.WrapText = false;
                }
            }
            IAdvanceAxis xAxis = (IAdvanceAxis)e.Grid["X"];
            IAdvanceAxis yAxis = (IAdvanceAxis)e.Grid["Y"];

            if (xAxis == null || yAxis == null)
                return;

            int textWidht = 200;
            int textHeight = 12;
            double lineStart = xAxis.MapMinimum;
            double lineLength = xAxis.MapMaximum;

            Line line = new Line();
            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
            line.PE.Stroke = System.Drawing.Color.DarkGray;
            line.PE.StrokeWidth = 2;
            line.p1 = new System.Drawing.Point((int)lineStart, (int)yAxis.Map(avgValue));
            line.p2 = new System.Drawing.Point((int)lineStart + (int)lineLength, (int)yAxis.Map(avgValue));
            e.SceneGraph.Add(line);

            Text text = new Text();
            text.PE.Fill = System.Drawing.Color.Black;
            text.bounds = new System.Drawing.Rectangle((int)lineLength - textWidht, ((int)yAxis.Map(avgValue)) - textHeight, textWidht, textHeight);
            text.SetTextString(string.Format("Средний темп роста: {0:N2}", avgValue));
            e.SceneGraph.Add(text);


        }

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {

            string query = DataProvider.GetQueryText("FO_0002_0003_Chart");
            ChartCaption1.Text = "Темп роста доходов";
            DataTable dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
            for (int i = 0; i < dtChart1.Rows.Count; i++)
            {

                dtChart1.Rows[i][0] = dtChart1.Rows[i][0].ToString().Replace("Муниципальный район", "МР");
                dtChart1.Rows[i][0] = dtChart1.Rows[i][0].ToString().Replace("муниципальный район", "МР");

            }
            double Sum = 0;
            if (dtChart1.Columns.Count > 0)
            {
                for (int k = 1; k < dtChart1.Columns.Count; k++)
                {
                    if (dtChart1.Rows[0][k] != DBNull.Value)
                    {
                        Sum += Convert.ToDouble(dtChart1.Rows[0][k]);
                    }
                }
            }
            avgValue = Convert.ToDouble(Sum / (dtChart1.Columns.Count - 1));
            UltraChart1.DataSource = dtChart1;
        }
        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Label1.Text + " " + Label2.Text;
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

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[4].Height = 17 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }
        }

        private void ExcelExporter_EndExport_withRanking(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;

            e.CurrentWorksheet.Columns[0].Width = 300 * 37;
            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
            e.CurrentWorksheet.Columns[1].Width = 90 * 37;
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "#,##0.0;[Red]-#,##0.0";
            e.CurrentWorksheet.Columns[2].Width = 90 * 37;
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
            e.CurrentWorksheet.Columns[3].Width = 90 * 37;
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "#,##0;[Red]-#,##0";
            e.CurrentWorksheet.Columns[4].Width = 90 * 37;

            for (int i = 5; i < columnCount; i = i + 1)
            {
                string formatString = UltraGridExporter.ExelNumericFormat;
                int columnWidth = 70;

                int j = (i - 5) % 6;
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
                    case 4:
                    case 5:
                        {
                            formatString = "#,##0.00;[Red]-#,##0.00";
                            columnWidth = 85;
                            break;
                        }
                    case 3:
                        {
                            formatString = "#,##0;[Red]-#,##0";
                            columnWidth = 85;
                            break;
                        }
                }

                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
                e.CurrentWorksheet.Columns[i].Width = columnWidth * 37;
            }

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[4].Height = 17 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }
        }

        private int hiddenOffset;
        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            UltraGridColumn col = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + hiddenOffset];
            while (col.Hidden)
            {
                hiddenOffset++;
                col = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex + hiddenOffset];
            }
            string headerText = col.Header.Key.Split(';')[0];
            e.HeaderText = headerText;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            hiddenOffset = 0;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
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
            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label1.Text);

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(Label2.Text);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
        }

        #endregion
    }
}
