using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report;

//using Infragistics.WebUI.UltraWebGrid.DocumentExport;
//using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
//using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = System.Drawing.Font;
using FontStyle = System.Drawing.FontStyle;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0016_01
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private DataTable dtStackChart = new DataTable();
        private DataTable dtMap = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private DateTime currentDate;
        private string currentFoShortName;
        private GridHeaderLayout headerlayout;
        public bool PlanSelected
        {
            get { return MeasureButtonList.SelectedIndex == 0; }
        }

        public bool IsUrfo
        {
            get { return currentFoShortName == "УрФО"; }
        }

        #region Параметры запроса

        // Выбранная мера ФК
        private CustomParam selectedFKMeasure;
        // Выбранная мера ФО
        private CustomParam selectedFOMeasure;
        // Выбранный субъект
        private CustomParam selectedSubject;
        // Текущий ФО
        private CustomParam currentFO;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.3 - 225);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);

            UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            UltraChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
            UltraChart.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            StackChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            StackChart.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.4);
            StackChart.FillSceneGraph += new FillSceneGraphEventHandler(StackChart_FillSceneGraph);
            StackChart.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.85);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            #region Инициализация параметров запроса

            if (selectedFKMeasure == null)
            {
                selectedFKMeasure = UserParams.CustomParam("selected_fk_measure");
            }
            if (selectedFOMeasure == null)
            {
                selectedFOMeasure = UserParams.CustomParam("selected_fo_measure");
            }
            if (selectedSubject == null)
            {
                selectedSubject = UserParams.CustomParam("selected_subject");
            }
            currentFO = UserParams.CustomParam("current_fo");

            #endregion

            #region Настройка диаграммы

            UltraChart.ChartType = ChartType.SplineChart;
            UltraChart.BorderWidth = 0;

            UltraChart.Axis.X.Extent = 50;
            UltraChart.Axis.Y.Extent = 60;

            UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P2>";
            UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            UltraChart.Axis.X.Labels.Font = new Font("Verdana", 8);

            UltraChart.Legend.Visible = true;
            UltraChart.Legend.Location = LegendLocation.Top;
            UltraChart.Legend.SpanPercentage = 13;
            UltraChart.Legend.Margins.Right = Convert.ToInt32(UltraChart.Width.Value / 4);
            UltraChart.Legend.Font = new Font("Verdana", 8);

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance.Thickness = 3;
            lineAppearance.SplineTension = (float)0.3;
            UltraChart.SplineChart.LineAppearances.Add(lineAppearance);

            UltraChart.Data.ZeroAligned = true;
            UltraChart.Data.SwapRowsAndColumns = true;
            UltraChart.SplineChart.NullHandling = NullHandling.DontPlot;

            #endregion

            #region Настройка диаграммы с накоплением

            StackChart.ChartType = ChartType.StackColumnChart;
            StackChart.StackChart.StackStyle = StackStyle.Complete;
            StackChart.BorderWidth = 0;

            StackChart.Axis.X.Extent = 40;
            StackChart.Axis.Y.Extent = 60;

            StackChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>%";
            StackChart.Axis.Y.Labels.Font = new Font("Verdana", 8);
            StackChart.Axis.Y.TickmarkStyle = AxisTickStyle.Percentage;

            StackChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            StackChart.Axis.X.Labels.Font = new Font("Verdana", 8);

            StackChart.Legend.Visible = true;
            StackChart.Legend.Location = LegendLocation.Top;
            StackChart.Legend.SpanPercentage = 10;
            StackChart.Legend.Margins.Right = 3 * Convert.ToInt32(UltraChart.Width.Value / 5);
            StackChart.Legend.Font = new Font("Verdana", 8);

            StackChart.Tooltips.FormatString = "<ITEM_LABEL>\nДоля: <DATA_VALUE:N2>%";
            StackChart.Data.ZeroAligned = true;
            StackChart.Data.SwapRowsAndColumns = false;

            StackChart.ColorModel.ModelStyle = ColorModels.CustomSkin;

            Color color1 = Color.LimeGreen;
            Color color2 = Color.Firebrick;

            StackChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color1, 150));
            StackChart.ColorModel.Skin.PEs.Add(CRHelper.GetFillPaintElement(color2, 150));
            StackChart.ColorModel.Skin.ApplyRowWise = false;

            StackChart.Effects.Effects.Clear();
            GradientEffect effect = new GradientEffect();
            effect.Style = GradientStyle.ForwardDiagonal;
            effect.Coloring = GradientColoringStyle.Darken;
            effect.Enabled = true;
            StackChart.Effects.Enabled = true;
            StackChart.Effects.Effects.Add(effect);

            #endregion

            #region Настройка карты

            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Left;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Left;
            DundasMap.Viewport.EnablePanning = true;

            // добавляем легенду
            DundasMap.Legends.Clear();
            Legend legend = new Legend("EfficiencyLevelLegend");
            legend.Visible = true;
            legend.Dock = PanelDockStyle.Right;
            legend.BackColor = Color.White;
            legend.BackSecondaryColor = Color.Gainsboro;
            legend.BackGradientType = GradientType.DiagonalLeft;
            legend.BackHatchStyle = MapHatchStyle.None;
            legend.BorderColor = Color.Gray;
            legend.BorderWidth = 1;
            legend.BorderStyle = MapDashStyle.Solid;
            legend.BackShadowOffset = 4;
            legend.TextColor = Color.Black;
            legend.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend.Title = "Уровень эффективности\nбюджетных расходов\nв сфере ЖКХ";
            legend.AutoFitText = true;
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(legend);

            LegendItem item = new LegendItem();
            item.Text = "0% - минимальная эффективность";
            item.Color = Color.Red;
            DundasMap.Legends[0].Items.Add(item);

            item = new LegendItem();
            item.Text = "100% - максимальная эффективность";
            item.Color = Color.Green;
            DundasMap.Legends[0].Items.Add(item);

            // добавляем поля для раскраски
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("EfficiencyLevelValue");
            DundasMap.ShapeFields["EfficiencyLevelValue"].Type = typeof(double);
            DundasMap.ShapeFields["EfficiencyLevelValue"].UniqueIdentifier = false;

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "EfficiencyLevelRule";
            rule.Category = String.Empty;
            rule.ShapeField = "EfficiencyLevelValue";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 5;
            rule.FromValue = "0";
            rule.ToValue = "1";
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.OrangeRed;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.LimeGreen;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "EfficiencyLevelLegend";
            rule.LegendText = "#FROMVALUE{P2} - #TOVALUE{P2}";
            DundasMap.ShapeRules.Add(rule);

            #endregion

            CrossLink.Visible = true;
            CrossLink.Text = "Неэффективные&nbsp;расходы&nbsp;на&nbsp;гос.&nbsp;и&nbsp;муниц.е&nbsp;управление";
            CrossLink.NavigateUrl = "~/reports/FK_0001_0016_02/Default.aspx";
/*
             UltraGridExporter1.MultiHeader = true;
             UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
             UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
             UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
             UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

             UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
             UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PdfExporter_BeginExport);
             UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
             */

        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            currentFO.Value = RegionsNamingHelper.GetFoBySubject(RegionSettingsHelper.Instance.Name);
            currentFoShortName = RegionsNamingHelper.ShortName(currentFO.Value);

            PopupInformer.Visible = !IsUrfo;
            HelpSpan.Visible = IsUrfo;

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(chartElementCaption);
                chartWebAsyncPanel.AddRefreshTarget(UltraChart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(UltraWebGrid);

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FK_0001_0016_01_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);

                int yearNum = Convert.ToInt32(dtDate.Rows[0][0].ToString());
                int monthNum = Convert.ToInt32(CRHelper.MonthNum(dtDate.Rows[0][3].ToString()));

                currentDate = new DateTime(yearNum, monthNum, 1);
                endYear = currentDate.Year;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                String month = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(currentDate.Month));
                ComboMonth.SetСheckedState(month, true);

                hiddenLabel.Text = "[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[Уральский федеральный округ].[Курганская область]";
            }

            int year = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(year, ComboMonth.SelectedIndex + 1, 1);

            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                PageTitle.Text = "Оценка неэффективных расходов в сфере жилищно-коммунального хозяйства";
                Page.Title = PageTitle.Text;
                PageSubTitle.Text = string.Format("Оценка эффективности расходов консолидированных бюджетов субъектов {4} ({2} {0} {3} {1} года)",
                    currentDate.Month, currentDate.Year, PlanSelected ? "план на" : "факт за", CRHelper.RusManyMonthGenitive(currentDate.Month), currentFoShortName);

                mapElementCaption.Text = "Уровень эффективности расходования бюджетных средств в сфере ЖКХ";
                stackChartElementCaption.Text = String.Format("Структура расходов консолидированных бюджетов субъектов {0} в сфере ЖКХ", currentFoShortName);

                UserParams.PeriodYear.Value = currentDate.Year.ToString();
                UserParams.PeriodMonth.Value = CRHelper.RusMonth(currentDate.Month);
                UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
                UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));

                selectedFKMeasure.Value = PlanSelected ? "Назначено" : "Исполнено";
                selectedFOMeasure.Value = PlanSelected ? "Годовые назначения" : "Факт";

                headerlayout = new GridHeaderLayout(UltraWebGrid);
                UltraWebGrid.Bands.Clear();
                UltraWebGrid.DataBind();

                string patternValue = hiddenLabel.Text;
                int defaultRowIndex = 0;
                if (patternValue == string.Empty)
                {
                    defaultRowIndex = 0;
                }

                if (UltraWebGrid.Columns.Count > 0 && UltraWebGrid.Rows.Count > 0)
                {
                    // ищем строку
                    UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, UltraWebGrid.Columns.Count - 1, defaultRowIndex);
                    // выделяем строку
                    ActiveGridRow(row);
                }

                StackChart.DataBind();

                DundasMap.Shapes.Clear();
                DundasMap.LoadFromShapeFile(Server.MapPath(string.Format("../../maps/{0}/{0}.shp", currentFoShortName)), "NAME", true);
                // заполняем карту данными
                FillMapData(DundasMap);
            }
        }

        #region Обработчики грида

        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            string regionName = row.Cells[0].Text;

            if (!regionName.Contains("итоги"))
            {
                chartElementCaption.Text = string.Format("Помесячная динамика доли неэффективных расходов в сфере ЖКХ ({0})", regionName);
                hiddenLabel.Text = row.Cells[row.Cells.Count - 1].Text;
                selectedSubject.Value = hiddenLabel.Text;
            }

            UltraChart.DataBind();
        }

        void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0016_01_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект РФ", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                foreach (DataColumn column in dtGrid.Columns)
                {
                    switch (column.ColumnName)
                    {
                        case "Общий объем расходов консолидированного бюджета на ЖКХ":
                            {
                                column.ColumnName = "Общий объем расходов консолидированного бюджета субъекта РФ на ЖКХ, тыс.руб.";
                                break;
                            }
                        case "Общий объем расходов консолидированного бюджета":
                            {
                                column.ColumnName = "Общий объем расходов консолидированного бюджета субъекта РФ, тыс.руб.";
                                break;
                            }
                        case "Объем неэффективных расходов в сфере ЖКХ полож":
                            {
                                column.ColumnName = "Объем неэффективных расходов в сфере ЖКХ, тыс.руб.";
                                break;
                            }
                        case "Доля неэффективных расходов в общей сумме расходов на ЖКХ":
                            {
                                column.ColumnName = "Доля неэффективных расходов в общей сумме расходов на ЖКХ";
                                break;
                            }
                        case "Доля неэффективных расходов в общей сумме расходов субъекта":
                            {
                                column.ColumnName = "Доля неэффективных расходов в общей сумме расходов";
                                break;
                            }
                        case "Уровень эффективности бюджетных расходов":
                            {
                                column.ColumnName = "Уровень эффективности бюджетных расходов в сфере ЖКХ";
                                break;
                            }
                    }
                }

                UltraWebGrid.DataSource = dtGrid;
            }
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid.Height = Unit.Empty;
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
                string formatString = "N2";

                int widthColumn = 150;

                switch (i)
                {
                    case 1:
                    case 2:
                    case 3:
                        {
                            formatString = "N2";
                            break;
                        }
                    case 4:
                    case 5:
                    case 6:
                        {
                            formatString = "P2";
                            break;
                        }
                }


                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            cellsFormulasCaption = new Collection<string>();
            cellsFormulasCaption.Add("Субъект РФ");
            cellsFormulasCaption.Add("Ржкх общ");
            cellsFormulasCaption.Add("Робщ");
            cellsFormulasCaption.Add("Ржкх");
            cellsFormulasCaption.Add("Джкх = Ржкх / Ржкх общ * 100");
            cellsFormulasCaption.Add("Джкх общ = Ржкх / Робщ * 100");
            cellsFormulasCaption.Add("Ид жкх = (макс Джкх – Джкх) / (макс Джкх – мин Джкх)");
            /*
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

            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 1, cellsFormulasCaption[1], "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 2, cellsFormulasCaption[2], "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 3, cellsFormulasCaption[3], "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 4, cellsFormulasCaption[4], "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 6, cellsFormulasCaption[5], "");
            CRHelper.SetHeaderCaption(e.Layout.Grid, 0, 8, cellsFormulasCaption[6], "");

            ColumnHeader ch =  CRHelper.AddHierarchyHeader(e.Layout.Grid, 0,"Общий объем расходов консолидированного бюджета субъекта РФ на ЖКХ, тыс.руб.", 1, 0, 1, 1);
            ch.Title = PlanSelected
                           ? "Запланированный объем расходов на ЖКХ консолидированного бюджета субъекта РФ"
                           : "Фактический объем расходов на ЖКХ консолидированного бюджета субъекта РФ";

            ch = CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "Общий объем расходов консолидированного бюджета субъекта РФ, тыс.руб.", 2, 0, 1, 1);
            ch.Title = PlanSelected
                           ? "Запланированный объем расходов консолидированного бюджета субъекта РФ"
                           : "Фактический объем расходов консолидированного бюджета субъекта РФ";

            ch = CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "Объем неэффективных расходов в сфере ЖКХ, тыс.руб.", 3, 0, 1, 1);
            ch.Title = PlanSelected
                           ? "Планируемые расходы консолидированного бюджета субъекта РФ на компенсацию предприятиям ЖКХ разницы между экономически обоснованными тарифами и тарифами, установленными для населения, и на покрытие убытков предприятий ЖКХ, возникших в связи с применением регулируемых цен на ЖКУ"
                           : "Фактические расходы консолидированного бюджета субъекта РФ на компенсацию предприятиям ЖКХ разницы между экономически обоснованными тарифами и тарифами, установленными для населения, и на покрытие убытков предприятий ЖКХ, возникших в связи с применением регулируемых цен на ЖКУ";

            ch = CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "Доля неэффективных расходов в общей сумме расходов на ЖКХ", 4, 0, 1, 1);
            ch.Title = "Доля неэффективных расходов в сфере ЖКХ в общем объеме расходов консолидированного бюджета субъекта РФ на ЖКХ";

            ch = CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "Доля неэффективных расходов в общей сумме расходов", 5, 0, 1, 1);
            ch.Title = "Доля неэффектиных расходов в сфере ЖКХ в общем объеме расходов консолидированного бюджета субъекта РФ";

            ch = CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, "Уровень эффективности бюджетных расходов в сфере ЖКХ", 6, 0, 1, 1);
            ch.Title = "Уровень эффективности расходования бюджетных средств";
            */

            headerlayout.AddCell("Субъект РФ");
            GridHeaderCell cell = headerlayout.AddCell("Общий объем расходов консолидированного бюджета субъекта РФ на ЖКХ, тыс.руб.", PlanSelected
                           ? "Запланированный объем расходов на ЖКХ консолидированного бюджета субъекта РФ"
                           : "Фактический объем расходов на ЖКХ консолидированного бюджета субъекта РФ");
            cell.AddCell(cellsFormulasCaption[1]);
            cell = headerlayout.AddCell("Общий объем расходов консолидированного бюджета субъекта РФ, тыс.руб.", PlanSelected
                           ? "Запланированный объем расходов консолидированного бюджета субъекта РФ"
                           : "Фактический объем расходов консолидированного бюджета субъекта РФ");
            cell.AddCell(cellsFormulasCaption[2]);
            cell = headerlayout.AddCell("Объем неэффективных расходов в сфере ЖКХ, тыс.руб.", PlanSelected
                           ? "Планируемые расходы консолидированного бюджета субъекта РФ на компенсацию предприятиям ЖКХ разницы между экономически обоснованными тарифами и тарифами, установленными для населения, и на покрытие убытков предприятий ЖКХ, возникших в связи с применением регулируемых цен на ЖКУ"
                           : "Фактические расходы консолидированного бюджета субъекта РФ на компенсацию предприятиям ЖКХ разницы между экономически обоснованными тарифами и тарифами, установленными для населения, и на покрытие убытков предприятий ЖКХ, возникших в связи с применением регулируемых цен на ЖКУ");
            cell.AddCell(cellsFormulasCaption[3]);
            cell = headerlayout.AddCell("Доля неэффективных расходов в общей сумме расходов на ЖКХ", "Доля неэффективных расходов в сфере ЖКХ в общем объеме расходов консолидированного бюджета субъекта РФ на ЖКХ");
            cell.AddCell(cellsFormulasCaption[4]);
            cell = headerlayout.AddCell("Доля неэффективных расходов в общей сумме расходов", "Доля неэффектиных расходов в сфере ЖКХ в общем объеме расходов консолидированного бюджета субъекта РФ");
            cell.AddCell(cellsFormulasCaption[5]);
            cell = headerlayout.AddCell("Уровень эффективности бюджетных расходов в сфере ЖКХ", "Уровень эффективности расходования бюджетных средств");
            cell.AddCell(cellsFormulasCaption[6]);
            headerlayout.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[7].Hidden = true;
            e.Layout.Bands[0].Columns[8].Hidden = true;
            e.Layout.Bands[0].Columns[9].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
        }

        private Collection<string> cellsFormulasCaption;

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                bool rank = (i == 4 || i == 5 || i == 6);
                bool corner = (i == 3);

                if (rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 3].Value != null)
                    {
                        string hint = string.Empty;
                        switch (i)
                        {
                            case 4:
                                {
                                    hint = "доля неэффективных расходов на ЖКХ";
                                    break;
                                }
                            case 5:
                                {
                                    hint = "доля неэффективных расходов в общем сумме расходов";
                                    break;
                                }
                            case 6:
                                {
                                    hint = "эффективность расходов среди оцениваемых субъектов";
                                    break;
                                }
                        }
                        if (e.Row.Cells[i + 3].Value.ToString() == "max")
                        {
                            e.Row.Cells[i].Style.BackgroundImage = i == 6 ? "~/images/starYellowBB.png" : "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = string.Format("Максимальная {0}", hint);
                        }
                        else if (e.Row.Cells[i + 3].Value.ToString() == "min")
                        {
                            e.Row.Cells[i].Style.BackgroundImage = i == 6 ? "~/images/starGrayBB.png" : "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("Минимальная {0}", hint);
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (corner)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToDouble(e.Row.Cells[i].Value) == 0)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/cornerGreen.gif";
                            e.Row.Cells[i].Title = "Неэффективных расходов нет";
                        }
                        else
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/cornerRed.gif";
                            e.Row.Cells[i].Title = "Имеются неэффективные расходы";
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top; margin: 2px";
                }

                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString().Contains("итоги"))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
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
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0016_01_chart");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart);

            UltraChart.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n<SERIES_LABEL> {0} года\n Доля неэффективных расходов: <DATA_VALUE:P2>", currentDate.Year);

            UltraChart.TitleBottom.Visible = true;
            UltraChart.TitleBottom.Text = currentDate.Year.ToString();
            UltraChart.TitleBottom.HorizontalAlign = StringAlignment.Center;
            UltraChart.TitleBottom.Font = new Font("Verdana", 8);

            UltraChart.DataSource = dtChart;
        }

        protected void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {

            }
        }

        #endregion

        #region Обработчики диаграммы с накоплением

        protected void StackChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FK_0001_0016_01_stackChart");
            dtStackChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtStackChart);

            DataTable dtStackChartCopy = dtStackChart.Copy();
            CRHelper.NormalizeDataTable(dtStackChartCopy);

            StackChart.Series.Clear();
            for (int i = 1; i < dtStackChartCopy.Columns.Count; i++)
            {
                NumericSeries series = CRHelper.GetNumericSeries(i, dtStackChartCopy);
                string columnName = dtStackChartCopy.Columns[i].ColumnName;
                series.Label = columnName;
                StackChart.Series.Add(series);
            }
        }

        protected void StackChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    //                    Text text = (Text)primitive;
                    //                    text.bounds.Width = 150;
                    //                    text.labelStyle.VerticalAlign = StringAlignment.Near;
                    //                    text.labelStyle.FontSizeBestFit = false;
                    //                    text.labelStyle.Font = new Font("Verdana", 8);
                    //                    text.labelStyle.WrapText = true;
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        string outcomesVolumeStr = string.Empty;
                        if (dtStackChart.Rows[box.Column][box.Row + 1] != DBNull.Value &&
                            dtStackChart.Rows[box.Column][box.Row + 1].ToString() != string.Empty &&
                            box.Series != null)
                        {
                            double outcomesVolume = Convert.ToDouble(dtStackChart.Rows[box.Column].ItemArray[box.Row + 1]);
                            outcomesVolumeStr = string.Format("{0}: {1:N2} тыс.руб.", box.DataPoint.Label, outcomesVolume);
                        }

                        box.DataPoint.Label = string.Format("{0}\n{1}", box.Series.Label, outcomesVolumeStr);
                    }
                }
            }
        }

        #endregion

        #region Обработчики карты

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <returns>найденная форма</returns>
        public Shape FindMapShape(MapControl map, string patternValue)
        {
            string subject = patternValue;

            if (IsUrfo)
            {
                subject = subject.Replace("область", "обл.");
                subject = subject.Replace("автономный округ", "АО");
                subject = subject.Replace("федеральный округ", "ФО");
            }

            ArrayList shapeList = map.Shapes.Find(subject, true, false);
            if (shapeList.Count > 0)
            {
                return (Shape)shapeList[0];
            }
            return null;
        }

        public void FillMapData(MapControl map)
        {
            if (map == null)
            {
                return;
            }

            string query = DataProvider.GetQueryText("FK_0001_0016_01_map");

            dtMap = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtMap);

            foreach (DataRow row in dtMap.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                {
                    string regionName = row[0].ToString();

                    Shape shape = FindMapShape(map, regionName);
                    if (shape != null)
                    {
                        shape.TextVisibility = TextVisibility.Shown;

                        string outcomesVolumeStr = string.Empty;
                        if (row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                        {
                            double outcomesVolume = Convert.ToDouble(row[2]);
                            outcomesVolumeStr = string.Format("Неэффективные расходы: {0:N2} тыс.руб.", outcomesVolume);
                        }

                        string outcomesPercentageStr = string.Empty;
                        if (row[3] != DBNull.Value && row[3].ToString() != string.Empty)
                        {
                            double outcomesPercentage = Convert.ToDouble(row[3]);
                            outcomesPercentageStr = string.Format("Доля: {0:P2}", outcomesPercentage);
                        }

                        string levelValueStr = string.Empty;
                        string levelValueMinMaxStr = string.Empty;
                        if (row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                        {
                            double levelValue = Convert.ToDouble(row[1]);
                            levelValueStr = string.Format("Уровень эффективности : {0:P2}", levelValue);

                            if (levelValue == 0)
                            {
                                levelValueMinMaxStr = "- минимальная эффективность расходов";
                                shape.Color = Color.Red;
                            }
                            else if (levelValue == 1)
                            {
                                levelValueMinMaxStr = "- максимальная эффективность расходов";
                                shape.Color = Color.Green;
                            }
                            else
                            {
                                shape["EfficiencyLevelValue"] = levelValue;
                            }

                            shape["Name"] = regionName;
                            shape.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                            shape.Offset.X = -15;

                            shape.Text = string.Format("{0}\n{1:P2}", shape.Name, levelValue);

                            shape.BorderWidth = 2;
                            shape.TextColor = Color.Black;
                        }

                        shape.ToolTip = string.Format("#NAME\n{0}\n{1}\n{2}{3}", outcomesVolumeStr, outcomesPercentageStr, levelValueStr, levelValueMinMaxStr);
                    }
                }
            }
        }

        #endregion

        /*   #region Экспорт в Excel

        private static void SetExportGridParams(UltraWebGrid grid)
        {
            int columnCount = grid.Columns.Count;
            int columnIndex = 0;


            for (int i = 0; i < columnCount; i++)
            {
                if (grid.Columns[columnIndex].Hidden)
                {
                    grid.Columns.RemoveAt(columnIndex);
                }
                else
                {
                    columnIndex++;
                    grid.Columns[columnIndex].Width = CRHelper.GetColumnWidth(115);
                }
            }
        }

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            //e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            //e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid.Columns.Count;

            e.CurrentWorksheet.Columns[0].Width = 200 * 37;

            // расставляем стили у ячеек хидера
            for (int i = 0; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[3].Height = 28 * 37;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[3].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;

                e.CurrentWorksheet.Rows[4].Height = 20 * 37;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            int width = 160;

            for (int i = 1; i < columnCount; i++)
            {
                string formatString = "P2";

                switch (i)
                {
                    case 1:
                    case 2:
                    case 3:
                        {
                            formatString = "#,##0.00;[Red]-#,##0.00";
                            break;
                        }
                    case 4:
                    case 5:
                    case 6:
                        {
                            formatString = UltraGridExporter.ExelPercentFormat;
                            break;
                        }
                }
                e.CurrentWorksheet.Columns[i].Width = width * 37;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = formatString;
            }
            // e.Workbook.Worksheets[0].Rows[0].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Left;

        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма 1");
            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма 2");
            Worksheet sheet4 = workbook.Worksheets.Add("Карта");
            sheet1.Rows[1].Cells[0].Value = PageTitle.Text;
            sheet1.Rows[2].Cells[0].Value = PageSubTitle.Text;

            sheet2.Rows[0].Cells[0].Value = PageTitle.Text;
            sheet2.Rows[1].Cells[0].Value = PageSubTitle.Text;
            sheet2.Rows[2].Cells[0].Value = chartElementCaption.Text;
            UltraGridExporter.ChartExcelExport(sheet2.Rows[3].Cells[0], UltraChart);

            sheet3.Rows[0].Cells[0].Value = PageTitle.Text;
            sheet3.Rows[1].Cells[0].Value = PageSubTitle.Text;
            sheet3.Rows[2].Cells[0].Value = stackChartElementCaption.Text;
            UltraGridExporter.ChartExcelExport(sheet3.Rows[3].Cells[0], StackChart);

            sheet4.Rows[0].Cells[0].Value = PageTitle.Text;
            sheet4.Rows[1].Cells[0].Value = PageSubTitle.Text;
            sheet4.Rows[2].Cells[0].Value = mapElementCaption.Text;
            UltraGridExporter.MapExcelExport(sheet4.Rows[3].Cells[0], DundasMap);
            SetExportGridParams(UltraWebGrid);
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1, 3, 0);
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 3;
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

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private bool titleAdded = false;

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            if (!titleAdded)
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

            titleAdded = true;
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();



            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chartElementCaption.Text);

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.9));
            UltraChart.Legend.Margins.Right = 5;
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart);
            e.Section.AddImage(img);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(stackChartElementCaption.Text);

            StackChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.9));
            StackChart.Legend.Margins.Right = 5;
            img = UltraGridExporter.GetImageFromChart(StackChart);
            e.Section.AddImage(img);

            e.Section.AddPageBreak();

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            title = e.Section.AddText();
            font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(mapElementCaption.Text);

            img = UltraGridExporter.GetImageFromMap(DundasMap);
            e.Section.AddImage(img);
        }

        #endregion*/
        #region Export excel
        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма 1");
            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма 2");
            Worksheet sheet4 = workbook.Worksheets.Add("Карта");
            sheet1.Rows[1].Cells[0].Value = PageTitle.Text;
            sheet1.Rows[2].Cells[0].Value = PageSubTitle.Text;

            sheet2.Rows[0].Cells[0].Value = PageTitle.Text;
            sheet2.Rows[1].Cells[0].Value = PageSubTitle.Text;
            sheet2.Rows[2].Cells[0].Value = chartElementCaption.Text;

            sheet3.Rows[0].Cells[0].Value = PageTitle.Text;
            sheet3.Rows[1].Cells[0].Value = PageSubTitle.Text;
            sheet3.Rows[2].Cells[0].Value = stackChartElementCaption.Text;

            sheet4.Rows[0].Cells[0].Value = PageTitle.Text;
            sheet4.Rows[1].Cells[0].Value = PageSubTitle.Text;
            sheet4.Rows[2].Cells[0].Value = mapElementCaption.Text;

            ReportExcelExporter1.Export(headerlayout, sheet1, 4);
            ReportExcelExporter1.Export(UltraChart, sheet2, 4);
            ReportExcelExporter1.Export(StackChart, sheet3, 4);
            ReportExcelExporter1.Export(DundasMap, sheet4, 4);



        }
        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            Report report = new Report();
            ISection section1 = report.AddSection();
            ReportPDFExporter1.HeaderCellHeight = 70;
            IText text = section1.AddText();
            text.AddContent(" ");
            ReportPDFExporter1.Export(headerlayout, PageSubTitle.Text, section1);

            ISection section2 = report.AddSection();
            IText title = section2.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            title = section2.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(chartElementCaption.Text);

            UltraChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            UltraChart.Legend.Margins.Right = 5;
            ReportPDFExporter1.Export(UltraChart, section2);

            ISection section3 = report.AddSection();
            title = section3.AddText();
            font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            title = section3.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(stackChartElementCaption.Text);

            StackChart.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            StackChart.Legend.Margins.Right = 5;
            ReportPDFExporter1.Export(StackChart, section3);

            ISection section4 = report.AddSection();
            title = section4.AddText();
            font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text);

            title = section4.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(mapElementCaption.Text);
            DundasMap.Height = Unit.Pixel((int)(CustomReportConst.minScreenHeight * 0.8));
            DundasMap.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            //DundasMap.Legend.Margins.Right = 5;
            ReportPDFExporter1.Export(DundasMap, section4);
        }



        #endregion
    }
}
