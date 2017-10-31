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

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0013_01_Saratov
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private int firstYear = 2000;
        private int endYear = 2011;
        private double avgValue = 0;
        #endregion

        #region Параметры запроса

        // Уровень районов
        private CustomParam regionsLevel;
        // консолидированный бюджет субъекта
        private CustomParam regionsConsolidateBudget;
        // доходы всего
        private CustomParam incomesAll;
        // расходы всего
        private CustomParam outcomesAll;

        // тип документа СКИФ для консолидированного бюджета субъекта
        private CustomParam consolidateBudgetDocumentSKIFType;
        // тип документа СКИФ для районов
        private CustomParam regionBudgetDocumentSKIFType;

        // уровень бюджета СКИФ для районов
        private CustomParam regionBudgetSKIFLevel;

        #endregion

        private double rubMultiplier;

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "Тыс.руб." : "Млн.руб."; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 12);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.9 - 120);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров запроса

            if (regionsLevel == null)
            {
                regionsLevel = UserParams.CustomParam("regions_level");
            }
            if (regionsConsolidateBudget == null)
            {
                regionsConsolidateBudget = UserParams.CustomParam("regions_consolidate_budget");
            }
            if (incomesAll == null)
            {
                incomesAll = UserParams.CustomParam("incomes_all");
            }
            if (outcomesAll == null)
            {
                outcomesAll = UserParams.CustomParam("outcomes_all");
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

            #endregion

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
            <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
            <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
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
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.FKRDimension.Value = RegionSettingsHelper.Instance.FKRDimension;
            UserParams.FKRAllLevel.Value = RegionSettingsHelper.Instance.FKRAllLevel;
            UserParams.FKRSectionLevel.Value = RegionSettingsHelper.Instance.FKRSectionLevel;
            UserParams.FKRSubSectionLevel.Value = RegionSettingsHelper.Instance.FKRSubSectionLevel;
            UserParams.EKRDimension.Value = RegionSettingsHelper.Instance.EKRDimension;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.OwnSubjectBudgetName.Value = RegionSettingsHelper.Instance.OwnSubjectBudgetName;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0013_01_Saratov_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);
            }

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            regionsConsolidateBudget.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
            incomesAll.Value = RegionSettingsHelper.Instance.IncomeTotal;
            outcomesAll.Value = RegionSettingsHelper.Instance.OutcomeFKRTotal;

            consolidateBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("ConsolidateBudgetDocumentSKIFType");
            regionBudgetDocumentSKIFType.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetDocumentSKIFType");

            regionBudgetSKIFLevel.Value = RegionSettingsHelper.Instance.GetPropertyValue("RegionBudgetSKIFLevel");

            Page.Title = "Исполнение бюджета в разрезе отдельных показателей";
            PageTitle.Text = Page.Title;

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            PageSubTitle.Text = string.Format("Информация за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UserParams.PeriodLastYear.Value = (yearNum - 1).ToString();
            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            rubMultiplier = IsThsRubSelected ? 1000 : 1000000;

            UltraWebGrid.DataBind();
            UltraChart1.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0013_01_Saratov_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataRow row in dtGrid.Rows)
                {
                    for (int i = 1; i < row.ItemArray.Length - 1; i++)
                    {
                        // номер в группе
                        int j = (i - 1) % 5;
                        if ((j == 0 || j == 1) && row[i] != DBNull.Value)
                        {
                            row[i] = Convert.ToDouble(row[i]) / rubMultiplier;
                        }
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
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

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 1; i = i + 1)
            {
                string formatString = "P2";
                int widthColumn = 80;

                // номер в группе
                int j = (i - 1) % 5;
                switch (j)
                {
                    case 0:
                    case 1:
                        {
                            formatString = "N2";
                            widthColumn = 110;
                            break;
                        }
                    case 2:
                    case 3:
                        {
                            formatString = "P2";
                            widthColumn = 85;
                            break;
                        }
                    case 4:
                        {
                            formatString = "N1";
                            widthColumn = 100;
                            break;
                        }
                }

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = widthColumn;
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = 220;
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

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 2; i = i + 5)
            {
                ColumnHeader ch = new ColumnHeader(true);

                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                ch.Caption = captions[0];

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, String.Format("Назначено, {0}", RubMultiplierCaption.ToLower()), "Плановые назначения на год");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, String.Format("Исполнено, {0}", RubMultiplierCaption.ToLower()), "Фактическое исполнение нарастающим итогом с начала года");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "Исполнено %", "Процент выполнения назначений/ Оценка равномерности исполнения (1/12 годового плана в месяц)");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, "Темп роста", "Темп роста к прошлому году");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 4, "Исполнено на душу населения", "Фактическое исполнение на душу населения");
                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 5;
                ch.RowLayoutColumnInfo.SpanX = 5;
                e.Layout.Bands[0].HeaderLayout.Add(ch);

            }
        }

        private bool IsInverseIndicator(string indicatorName)
        {
            // изменения по индикации откатили обратно
            return false;// (indicatorName == "РАСХОДЫ ВСЕГО ") || (indicatorName == "Оплата труда ") || (indicatorName == "Капитальные вложения ") || (indicatorName == "Материальные затраты ");
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
            text.SetTextString(string.Format("Средний процент исполнения: {0:N2}", avgValue));
            e.SceneGraph.Add(text);


        }


        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count - 2; i++)
            {
                int j = (i - 1) % 5;
                bool complete = (j == 2);
                bool growRateColumn = (j == 3);

                string indicatorName = String.Empty;
                string[] indicatorNameParts = dtGrid.Columns[i].ColumnName.Split(';');
                if (indicatorNameParts.Length > 0)
                {
                    indicatorName = indicatorNameParts[0];
                }

                if (complete)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double percent = (ComboMonth.SelectedIndex + 1) * 100.0 / 12;

                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < percent)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                            e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:N2}%)", percent);
                        }
                        else
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:N2}%)", percent);
                        }
                        e.Row.Cells[i].Style.Padding.Right = 2;
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }

                if (growRateColumn)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != String.Empty)
                    {
                        if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = IsInverseIndicator(indicatorName) ? "~/images/arrowRedUpBB.png" : "~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Прирост к прошлому году";
                        }
                        else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = IsInverseIndicator(indicatorName) ? "~/images/arrowGreenDownBB.png" : "~/images/arrowRedDownBB.png";
                            e.Row.Cells[i].Title = "Снижение к прошлому году";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                UltraGridCell cell = e.Row.Cells[i];
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

                    if (e.Row.Cells[0].Value != null &&
                    (e.Row.Cells[0].Value.ToString().ToLower().Contains("бюджет") ||
                    e.Row.Cells[0].Value.ToString().ToLower().Contains("городские округа") ||
                    e.Row.Cells[0].Value.ToString().ToLower().Contains("муниципальные районы")))
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
            }
        }



        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = string.Empty;
            if (kindList.SelectedValue == "Исполнение доходов")
            {
                query = DataProvider.GetQueryText("FO_0002_0013_01_Saratov_incomes");
                ChartCaption1.Text = "Исполнение доходов";
            }
            else
            {
                query = DataProvider.GetQueryText("FO_0002_0013_01_Saratov_chart_outcomes");
                ChartCaption1.Text = "Исполнение расходов";
            }
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
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text + " " + PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count - 1;

            int width = 101;

            for (int i = 1; i < columnCount; i++)
            {
                string formatString = "P2";

                // номер в группе
                int j = (i - 1) % 4;
                switch (j)
                {
                    case 0:
                    case 1:
                        {
                            formatString = UltraGridExporter.ExelNumericFormat;
                            break;
                        }
                    case 2:
                    case 3:
                        {
                            formatString = UltraGridExporter.ExelPercentFormat;
                            break;
                        }
                }
                e.CurrentWorksheet.Columns[i].Width = width * 37;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
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
            Report report = new Report();
            ReportSection section1 = new ReportSection(report, false);
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid, section1);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            //string label = Label2.Text.Replace("<br/>", "");
            IText title = e.Section.AddText();
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 16);
            title.Style.Font.Bold = true;
            title.AddContent(Page.Title);

            title = e.Section.AddText();
            font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            //title.AddContent(label);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption1.Text);
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);
        }

        #endregion
    }

    public class ReportSection : ISection
    {
        private readonly bool withFlowColumns;
        private readonly ISection section;
        private IFlow flow;
        private ITableCell titleCell;

        public ReportSection(Report report, bool withFlowColumns)
        {
            section = report.AddSection();
            ITable table = section.AddTable();
            ITableRow row = table.AddRow();
            titleCell = row.AddCell();

        }

        public void AddFlowColumnBreak()
        {
            if (flow != null)
                flow.AddColumnBreak();
        }

        public Infragistics.Documents.Reports.Report.ContentAlignment PageAlignment
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IBand AddBand()
        {
            return section.AddBand();
        }

        #region ISection members
        public ISectionHeader AddHeader()
        {
            throw new NotImplementedException();
        }

        public ISectionFooter AddFooter()
        {
            throw new NotImplementedException();
        }

        public IStationery AddStationery()
        {
            throw new NotImplementedException();
        }

        public IDecoration AddDecoration()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(PageSize size)
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(float width, float height)
        {
            throw new NotImplementedException();
        }

        public ISegment AddSegment()
        {
            throw new NotImplementedException();
        }

        public IQuickText AddQuickText(string text)
        {
            throw new NotImplementedException();
        }

        public IQuickImage AddQuickImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            throw new NotImplementedException();
        }

        public IQuickList AddQuickList()
        {
            throw new NotImplementedException();
        }

        public IQuickTable AddQuickTable()
        {
            throw new NotImplementedException();
        }

        public IText AddText()
        {
            return this.titleCell.AddText();
        }

        public IImage AddImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            if (flow != null)
                return flow.AddImage(image);
            return this.section.AddImage(image);
        }

        public IMetafile AddMetafile(Metafile metafile)
        {
            throw new NotImplementedException();
        }

        public IRule AddRule()
        {
            throw new NotImplementedException();
        }

        public IGap AddGap()
        {
            throw new NotImplementedException();
        }

        public IGroup AddGroup()
        {
            throw new NotImplementedException();
        }

        public IChain AddChain()
        {
            throw new NotImplementedException();
        }

        public ITable AddTable()
        {
            return this.section.AddTable();
        }

        public IGrid AddGrid()
        {
            throw new NotImplementedException();
        }

        public IFlow AddFlow()
        {
            throw new NotImplementedException();
        }

        public Infragistics.Documents.Reports.Report.List.IList AddList()
        {
            throw new NotImplementedException();
        }

        public ITree AddTree()
        {
            throw new NotImplementedException();
        }

        public ISite AddSite()
        {
            throw new NotImplementedException();
        }

        public ICanvas AddCanvas()
        {
            throw new NotImplementedException();
        }

        public IRotator AddRotator()
        {
            throw new NotImplementedException();
        }

        public IContainer AddContainer(string name)
        {
            throw new NotImplementedException();
        }

        public ICondition AddCondition(IContainer container, bool fit)
        {
            throw new NotImplementedException();
        }

        public IStretcher AddStretcher()
        {
            throw new NotImplementedException();
        }

        public void AddPageBreak()
        {
            throw new NotImplementedException();
        }

        public ITOC AddTOC()
        {
            throw new NotImplementedException();
        }

        public IIndex AddIndex()
        {
            throw new NotImplementedException();
        }

        public bool Flip
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public PageSize PageSize
        {
            get { throw new NotImplementedException(); }
            set { this.section.PageSize = new PageSize(2560, 1350); }
        }

        public PageOrientation PageOrientation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }



        public Borders PageBorders
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Margins PageMargins
        {
            get { return this.section.PageMargins; }
            set { throw new NotImplementedException(); }
        }

        public Paddings PagePaddings
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Background PageBackground
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Section.PageNumbering PageNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public SectionLineNumbering LineNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Report Parent
        {
            get { return this.section.Parent; }
        }

        public IEnumerable Content
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

}