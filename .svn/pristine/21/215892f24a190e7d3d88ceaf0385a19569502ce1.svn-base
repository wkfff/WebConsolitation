using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using InitializeRowEventHandler = Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;

namespace Krista.FM.Server.Dashboards.reports.STAT_0001_0004
{
    public partial class Default : CustomReportPage
    {
        private Dictionary<DateTime, string> candleLabelsDictionary;
        private DateTime currDateTime;
        private DateTime lastDateTime;

        private DataTable candleDT;

        // Показатель
        private CustomParam labourIndicatorName;
        // Территория
        private CustomParam regionName;
        // УФО
        private CustomParam ufo;
        // Текущая дата
        private CustomParam periodCurrentDate;
        // На неделю назад
        private CustomParam periodLastWeekDate;

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
            get { return GetScreenWidth < 1200; }
        }

        private int MinScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private int MinScreenHeight
        {
            get { return IsSmallResolution ? 600 : CustomReportConst.minScreenHeight; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (periodLastWeekDate == null)
            {
                periodLastWeekDate = UserParams.CustomParam("period_last_week_date");
            }

            #endregion
            if (!IsPostBack)
            {
                ComboKind.FillDictionaryValues(
                    CustomMultiComboDataHelper.FillLabourMarketIndicators(
                        DataDictionariesHelper.LabourMarketIndicatorsTypes));
                ComboKind.Title = "Показатель";
                ComboKind.Width = 500;
                ComboKind.SetСheckedState("Общая численность зарегистрированных безработных граждан", true);
                ComboKind.ParentSelect = true;
            }
            System.Drawing.Font font = new System.Drawing.Font("verdana", 10f);

            UltraChart1.Axis.X.Labels.LabelStyle.Font = font;
            UltraChart1.Axis.Y.Labels.LabelStyle.Font = font;

            #region настройка диаграммы
            UltraChart1.ChartType = ChartType.LineChart;
            UltraChart1.BorderWidth = 0;

//            UltraChart1.LineChart.LineAppearances.Clear();
//
//            LineAppearance lineAppearance1 = new LineAppearance();
//            lineAppearance1.IconAppearance.Icon = SymbolIcon.Circle;
//            lineAppearance1.IconAppearance.IconSize = SymbolIconSize.Small;
//            lineAppearance1.Thickness = 3;
//            UltraChart1.LineChart.LineAppearances.Add(lineAppearance1);
//
//            LineAppearance lineAppearance2 = new LineAppearance();
//            lineAppearance2.IconAppearance.Icon = SymbolIcon.Circle;
//            lineAppearance2.IconAppearance.IconSize = SymbolIconSize.Small;
//            lineAppearance2.Thickness = 3;
//            UltraChart1.LineChart.LineAppearances.Add(lineAppearance2);
//
//            LineAppearance lineAppearance3 = new LineAppearance();
//            lineAppearance3.IconAppearance.Icon = SymbolIcon.Circle;
//            lineAppearance3.IconAppearance.IconSize = SymbolIconSize.Small;
//            lineAppearance3.Thickness = 3;
//            UltraChart1.LineChart.LineAppearances.Add(lineAppearance3);
//
//            LineAppearance lineAppearance4 = new LineAppearance();
//            lineAppearance4.IconAppearance.Icon = SymbolIcon.Circle;
//            lineAppearance4.IconAppearance.IconSize = SymbolIconSize.Small;
//            lineAppearance4.Thickness = 3;
//            UltraChart1.LineChart.LineAppearances.Add(lineAppearance4);
//
//            LineAppearance lineAppearance5 = new LineAppearance();
//            lineAppearance5.IconAppearance.Icon = SymbolIcon.Circle;
//            lineAppearance5.IconAppearance.IconSize = SymbolIconSize.Small;
//            lineAppearance5.Thickness = 3;
//            UltraChart1.LineChart.LineAppearances.Add(lineAppearance5);
//
//            LineAppearance lineAppearance6 = new LineAppearance();
//            lineAppearance6.IconAppearance.Icon = SymbolIcon.Circle;
//            lineAppearance6.IconAppearance.IconSize = SymbolIconSize.Small;
//            lineAppearance6.Thickness = 3;
//            UltraChart1.LineChart.LineAppearances.Add(lineAppearance6);

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;

            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 7; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                switch(i)
                {
                    case 1:
                        {
                            color = Color.Green;
                            break;
                        }
                    case 2:
                        {
                            color = Color.Gold;
                            break;
                        }
                    case 3:
                        {
                            color = Color.Black;
                            break;
                        }
                    case 4:
                        {
                            color = Color.LightSlateGray;
                            break;
                        }
                    case 5:
                        {
                            color = Color.Red;
                            break;
                        }
                    case 6:
                        {
                            color = Color.Blue;
                            break;
                        }
                    case 7:
                        {
                            color = Color.DarkViolet;
                            break;
                        }
                }
                pe.Fill = color;
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }

            UltraChart1.Axis.X.Extent = 80;
            UltraChart1.Axis.Y.Extent = 60;

            UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;

            UltraChart2.Axis.X.LineColor = Color.Black;
            UltraChart2.Axis.Y.LineColor = Color.Black;

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 15;


            UltraChart1.TitleLeft.Visible = true;

            UltraChart1.TitleLeft.Font = font;
            UltraChart2.TitleLeft.Font = font;
            UltraChart2.TitleLeft.FontColor = Color.Black;

            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            //  UltraChart1.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart1.Height.Value) - UltraChart1.Axis.X.Extent;

            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
                 //UltraChart2.TitleLeft.Margins.Bottom = Convert.ToInt32(UltraChart2.Height.Value) - UltraChart2.Axis.X.Extent;

            if (ComboKind.SelectedValue == "Уровень регистрируемой безработицы")
            {
                UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> \n<DATA_VALUE:P3>";
                UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P2>";
                UltraChart1.TitleLeft.Text = "Процент";

                UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
                UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P2>";
                UltraChart2.TitleLeft.Text = "Процент";

                lbMeasures.Text = "Процент";
            }
            else if (ComboKind.SelectedValue == "Число зарегистрированных безработных в расчёте на 1 вакансию")
            {
                UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> \n<DATA_VALUE:N2>";
                UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
                UltraChart1.TitleLeft.Text = "чел/на 1 вакансию";

                UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
                UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
                UltraChart2.TitleLeft.Text = "чел/на 1 вакансию";

                lbMeasures.Text = "чел/на 1 вакансию";
            }
            else if (ComboKind.SelectedValue == "Количество предприятий, имеющих задолженность по выплате заработной платы")
            {
                UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> \n<DATA_VALUE:N0> шт.";
                UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                UltraChart1.TitleLeft.Text = "Штук";

                UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
                UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0> шт.";
                UltraChart2.TitleLeft.Text = "Штук";

                lbMeasures.Text = "Штук";
            }
            else if (ComboKind.SelectedValue == "Сумма задолженности по выплате заработной платы (млн.руб.)")
            {
                UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> \n<DATA_VALUE:N3>";
                UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N3>";
                UltraChart1.TitleLeft.Text = "млн. руб.";

                UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
                UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N3>";
                UltraChart2.TitleLeft.Text = "млн. руб.";

                lbMeasures.Text = "млн. руб.";
            }
            else
            {
                UltraChart1.Tooltips.FormatString = "<ITEM_LABEL> \n<DATA_VALUE:N0> чел.";
                UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                UltraChart1.TitleLeft.Text = "Человек";

                UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
                UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                UltraChart2.TitleLeft.Text = "Человек";

                lbMeasures.Text = "Человек";
            }
            UltraChart2.Axis.Y.Labels.SeriesLabels.Visible = true;
            UltraChart1.Width = CRHelper.GetChartWidth(MinScreenWidth - 70);
            UltraChart1.Height = CRHelper.GetChartHeight(MinScreenHeight * 0.65 - 100);

            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.DataBinding += new EventHandler(UltraChart1_DataBinding);
            #endregion

            UltraWebGrid.Width = CRHelper.GetGridWidth(MinScreenWidth - 35);
            UltraWebGrid.Height = CRHelper.GetGridHeight(MinScreenHeight * 0.55);

            #region Настройка диаграммы

            UltraChart2.ChartType = ChartType.CandleChart;
            UltraChart2.Border.Thickness = 0;

            UltraChart2.CandleChart.SkipN = 0;

            if (ComboKind.SelectedValue == "Потребность предприятий в работниках, заявленная в службы занятости")
            {
                UltraChart2.CandleChart.NegativeRangeColor = Color.Red;
                UltraChart2.CandleChart.PositiveRangeColor = Color.Green;
            }
            else
            {
                UltraChart2.CandleChart.NegativeRangeColor = Color.Green;
                UltraChart2.CandleChart.PositiveRangeColor = Color.Red;
            }
            UltraChart2.Axis.X.Labels.SeriesLabels.Visible = true;
            UltraChart2.Axis.X.Labels.SeriesLabels.FormatString = "<SERIES_LABEL>";

            UltraChart2.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart2.Axis.X.Labels.Orientation = TextOrientation.Horizontal;

            UltraChart2.Axis.X.Labels.LabelStyle.Font = font;
            UltraChart2.Axis.X.Labels.FontColor = Color.Black;
            UltraChart2.Axis.Y.Labels.LabelStyle.Font = font;
            UltraChart2.Axis.Y.Labels.FontColor = Color.Black;

            UltraChart2.Axis.X.Extent = 80;
            UltraChart2.Axis.Y.Extent = 60;

            UltraChart2.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);
            UltraChart2.DataBinding += new EventHandler(UltraChart_DataBinding);

            UltraChart2.Width = CRHelper.GetChartWidth(MinScreenWidth - 70);
            UltraChart2.Height = CRHelper.GetChartHeight(MinScreenHeight * 0.75);



            #endregion

            UltraWebGrid.DataBinding += new EventHandler(UltraWebGrid_DataBinding);
            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);

            UltraWebGrid.EnableViewState = false;

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            WebAsyncPanel.AddRefreshTarget(UltraChart1);
            WebAsyncPanel.AddRefreshTarget(UltraChart2);
            WebAsyncPanel.AddRefreshTarget(UltraWebGrid);
            WebAsyncPanel.AddLinkedRequestTrigger(showUfo.ClientID);

            labourIndicatorName = UserParams.CustomParam("labour_indicator_name");
            regionName = UserParams.CustomParam("region_name");
            ufo = UserParams.CustomParam("ufo");

            labourIndicatorName.Value = DataDictionariesHelper.LabourMarketIndicatorsTypes[ComboKind.SelectedValue];

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0004_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtDate);

            if (dtDate.Rows.Count > 0)
            {
                currDateTime = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
                if (currDateTime != DateTime.MinValue)
                {
                    lastDateTime = currDateTime.AddDays(-7);
                }
            }
            else
            {
                string labourIndicatorNameValue = labourIndicatorName.Value;
                labourIndicatorName.Value = DataDictionariesHelper.LabourMarketIndicatorsTypes["Общая численность зарегистрированных безработных граждан"];

                dtDate = new DataTable();
                query = DataProvider.GetQueryText("STAT_0001_0004_date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtDate);

                currDateTime = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][1].ToString(), 3);
                if (currDateTime != DateTime.MinValue)
                {
                    lastDateTime = currDateTime.AddDays(-7);
                }
                labourIndicatorName.Value = labourIndicatorNameValue;
            }

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", currDateTime, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", lastDateTime, 5);

            ufo.Value = showUfo.Checked ? "+ {[Территории].[Сопоставимый].[Итого по УФО]}" : String.Empty;
            gridElementCaption.Text = String.Format("Динамика показателя «{0}» ({1})", ComboKind.SelectedValue, lbMeasures.Text.ToLower());
            lbMeasures.Visible = false;
            UltraChart1.DataBind();
            UltraWebGrid.DataBind();
            UltraChart2.DataBind();

            lbChartTitle.Text = "Анализ динамики показателей ситуации на рынке труда";
            lbTitle.Text =
                    String.Format("Анализ динамики показателя «{0}» мониторинга ситуации на рынке труда в субъектах Российской Федерации, входящих в Уральский федеральный округ", ComboKind.SelectedValue);
            chartElementCaption.Text = String.Format("Колебания показателя «{0}»", ComboKind.SelectedValue);
        }

        void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.HeaderStyleDefault.Wrap = true;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            UltraGridBand band = e.Layout.Bands[0];

            band.Columns[0].Header.Caption = "День";

            UltraGridColumn col = new UltraGridColumn();
            col = col.CopyFrom(band.Columns[0]);
            band.Columns.Insert(band.Columns.Count, col);
            band.Columns[band.Columns.Count - 1].Width = CRHelper.GetColumnWidth(60);

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == e.Layout.Bands[0].Columns.Count - 1)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(40);

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count - 2; i = i + 3)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(62);
                e.Layout.Bands[0].Columns[i + 1].Width = CRHelper.GetColumnWidth(60);
                e.Layout.Bands[0].Columns[i + 2].Width = CRHelper.GetColumnWidth(61);

                CRHelper.AddHierarchyHeader(e.Layout.Grid, 0, e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0],
                                            i, 0, 3, 1);

                if (e.Layout.Bands[0].Columns[i].Header.Caption.Contains(";"))
                    e.Layout.Bands[0].Columns[i].Header.Caption =
                        e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[1];

                if (e.Layout.Bands[0].Columns[i + 1].Header.Caption.Contains(";"))
                    e.Layout.Bands[0].Columns[i + 1].Header.Caption =
                        e.Layout.Bands[0].Columns[i + 1].Header.Caption.Split(';')[1];

                if (e.Layout.Bands[0].Columns[i + 2].Header.Caption.Contains(";"))
                    e.Layout.Bands[0].Columns[i + 2].Header.Caption =
                        e.Layout.Bands[0].Columns[i + 2].Header.Caption.Split(';')[1];

                e.Layout.Bands[0].Columns[i].Header.Style.HorizontalAlign = HorizontalAlign.Center;

                e.Layout.Bands[0].Columns[i].Header.Title = "Значение показателя";
                e.Layout.Bands[0].Columns[i + 1].Header.Title = "Темп прироста значения показателя к предыдущей неделе";


                if (ComboKind.SelectedValue == "Уровень регистрируемой безработицы")
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N3");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "P2");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "N0");
                    e.Layout.Bands[0].Columns[i + 2].Header.Title = "ранг в УрФО";
                    e.Layout.Bands[0].Columns[i + 2].Header.Caption = "Ранг";
                }
                else if (ComboKind.SelectedValue == "Число зарегистрированных безработных в расчёте на 1 вакансию")
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "P2");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "N0");
                    e.Layout.Bands[0].Columns[i + 2].Header.Title = "ранг в УрФО";
                    e.Layout.Bands[0].Columns[i + 2].Header.Caption = "Ранг";
                }
                else if (ComboKind.SelectedValue == "Сумма задолженности по выплате заработной платы (млн.руб.)")
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N3");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "P2");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "N3");
                    e.Layout.Bands[0].Columns[i + 2].Header.Title = "Прирост значения показателя к предыдущей неделе";
                    e.Layout.Bands[0].Columns[i + 2].Header.Caption = "Прирост";
                }
                else
                {
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 1], "P2");
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i + 2], "N0");
                    e.Layout.Bands[0].Columns[i + 2].Header.Title = "Прирост значения показателя к предыдущей неделе";
                    e.Layout.Bands[0].Columns[i + 2].Header.Caption = "Прирост";
                }
            }
        }

        void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (CRHelper.IsMonthCaption(e.Row.Cells[0].Value.ToString()))
            {
                e.Row.Cells[0].ColSpan = e.Row.Band.Columns.Count - 1;
                e.Row.Cells[0].Style.Font.Bold = true;
                e.Row.Cells[e.Row.Band.Columns.Count - 1].Style.Font.Bold = true;
            }
            if (ComboKind.SelectedValue == "Уровень регистрируемой безработицы")
            {
                for (int i = 1; i < e.Row.Cells.Count - 2; i = i + 3)
                {
                    double value;
                    if (e.Row.Cells[i].Value != null && Double.TryParse(e.Row.Cells[i].Value.ToString(), out value))
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N3}%", e.Row.Cells[i].Value);
                    }
                }
            }
            for (int i = 2; i < e.Row.Cells.Count - 2; i = i + 3)
            {
                double value;
                if (e.Row.Cells[i].Value != null && Double.TryParse(e.Row.Cells[i].Value.ToString(), out value))
                {
                    if (value > 0)
                    {
                        e.Row.Cells[i].Value = String.Format("+{0:P2}", value);
                    }
                    else
                    {
                        e.Row.Cells[i].Value = String.Format("{0:P2}", value);
                    }
                }
            }
        }

        /*    void UltraChart2_DataBinding(object sender, EventArgs e)
            {
                string query = DataProvider.GetQueryText("STAT_0001_0004_chart2");
                DataTable dtChart2 = new DataTable();
                try
                {
                    DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtChart2);
                }
                catch (OleDbException ex)
                { }

                DataTable dtSource = new DataTable();
                DataColumn col = new DataColumn("Округ", typeof (string));
                dtSource.Columns.Add(col);

                for (int i = 1; i <= 52; i++ )
                {
                    col = new DataColumn(i.ToString(), typeof(double));
                    dtSource.Columns.Add(col);
                }

                DataRow row = dtSource.NewRow();

                for (int i = 1; i <= 52; i++)
                {
                    row[i] = 0;
                }

              //  int start = ComboRegiones.SelectedIndex == 0 ? 1 : 2;

             //   row[0] = dtChart2.Rows[0][start];

              //  for (int i = start + 1; i < dtChart2.Columns.Count; i++)
                {
               //     DateTime date = CRHelper.DateByPeriodMemberUName(dtChart2.Columns[i].Caption, 3);
               //     if (date != DateTime.MinValue)
                    {
                //        int index = date.DayOfYear / 7;
                 //       row[index] = dtChart2.Rows[0][i];
                    }
                }

                dtSource.Rows.Add(row);

                UltraChart2.DataSource = dtSource;
            } */

        void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0004_grid");
            DataTable dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "День", dtGrid);

            foreach (DataRow row in dtGrid.Rows)
            {
                if (CRHelper.IsMonthCaption(row[0].ToString()))
                {
                    for (int i = 1; i < dtGrid.Columns.Count; i++)
                    {
                        row[i] = DBNull.Value;
                    }
                }
            }
            
            UltraWebGrid.DataSource = dtGrid;
        }

        void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0004_chart1");
            DataTable dtChart1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dtChart1);

            DataTable dtSource = dtChart1.Clone();
            string currentMonth = string.Empty;
            foreach (DataRow row in dtChart1.Rows)
            {
                if (!row[0].ToString().Contains("Данные"))
                {
                    if (CRHelper.IsMonthCaption(row[0].ToString()))
                    {
                        currentMonth = row[0].ToString();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(currentMonth))
                        {
                            row[0] = currentMonth + " - " + row[0];
                            currentMonth = string.Empty;
                        }
                        dtSource.ImportRow(row);
                    }
                }
            }

            UltraChart1.DataSource = dtSource;
            UltraChart1.Data.SwapRowsAndColumns = true;
        }

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = lbChartTitle.Text + " " + lbTitle.Text;
        }



        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            for (int i = 1; i < 19; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 90 * 37;
                e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0";
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

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(115);
            }

            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 16);
            title.Style.Font = new Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(lbChartTitle.Text);

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Font(font);
            title.AddContent(lbTitle.Text);

            UltraChart1.Width = 1000;
            UltraChart2.Width = 1000;

            e.Section.AddImage(UltraGridExporter.GetImageFromChart(UltraChart1));

            title = e.Section.AddText();
            font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Font(font);
            title.AddContent(gridElementCaption.Text);
        }

        private void PdfExporter_EndExport(object sender, EndExportEventArgs e)
        {
            //    IText title = e.Section.AddText();
            //    Font font = new Font("Verdana", 14);
            //    title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            //    title.AddContent(chartHeaderLabel.Text);
            IText title = e.Section.AddText();
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 14);
            title.Style.Font = new Font(font);
            title.AddContent(chartElementCaption.Text);
            e.Section.AddImage(UltraGridExporter.GetImageFromChart(UltraChart2));
        }



        #endregion

        /*   private void FillComboRegions()
        {
            DataTable dtRegions = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0004_regions");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dtRegions);

            Dictionary<string, int> regions = new Dictionary<string, int>();
          //  regions.Add(dtRegions.Rows[0][1].ToString(), 0);
            foreach(DataRow row in dtRegions.Rows)
            {
                regions.Add(row[0].ToString(), 0);
            }
            //ComboRegiones.FillDictionaryValues(regions);
        } */

        #region Обработчики диаграммы

        protected void UltraChart_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0004_chart2");
            DataTable dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Series Name", dtChart);
            if (dtChart.Rows.Count > 0)
            {
                // dtChart.Columns[1].ColumnName = string.Format("{1} ({0:dd.MM})", lastDateTime, dtChart.Columns[1].ColumnName);
                // dtChart.Columns[2].ColumnName = string.Format("{1} ({0:dd.MM})", currDateTime, dtChart.Columns[4].ColumnName);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = RegionsNamingHelper.ShortName(row[0].ToString());
                    }
                }

                candleLabelsDictionary = new Dictionary<DateTime, string>();

                UltraChart2.Series.Clear();
                UltraChart2.Series.Add(GetCandleSeries("Name", GetCandleChartDT(dtChart)));

            }
        }

        private DataTable GetCandleChartDT(DataTable chartDT)
        {
            candleDT = new DataTable();

            DataColumn nameColumn = new DataColumn("Name", typeof(string));
            candleDT.Columns.Add(nameColumn);

            DataColumn dateColumn = new DataColumn("DateTime", typeof(DateTime));
            candleDT.Columns.Add(dateColumn);

            for (int i = 1; i < chartDT.Columns.Count; i++)
            {
                DataColumn candleColumn = new DataColumn(chartDT.Columns[i].ColumnName, chartDT.Columns[i].DataType);
                candleDT.Columns.Add(candleColumn);
            }

            DateTime time = Convert.ToDateTime("01-01-90");

            for (int i = 0; i < chartDT.Rows.Count; i++)
            {
                DataRow row = chartDT.Rows[i];
                DataRow candleRow = candleDT.NewRow();
                candleRow[1] = time;
                candleRow[0] = row[0];
                candleLabelsDictionary.Add(time, candleRow[0].ToString());
                time = time.AddYears(1);

                for (int j = 1; j < row.ItemArray.Length; j++)
                {
                    candleRow[j + 1] = row[j];
                }

                candleDT.Rows.Add(candleRow);
            }

            return candleDT;
        }

        public static CandleSeries GetCandleSeries(string name, object dataSource)
        {
            DataTable dataTable = (DataTable)dataSource;

            CandleSeries candleSeries = new CandleSeries();
            candleSeries.Label = name;
            candleSeries.Data.LabelColumn = dataTable.Columns[0].ColumnName;
            candleSeries.Data.DateColumn = dataTable.Columns[1].ColumnName;
            candleSeries.Data.OpenColumn = dataTable.Columns[2].ColumnName;
            candleSeries.Data.CloseColumn = dataTable.Columns[3].ColumnName;
            candleSeries.Data.LowColumn = dataTable.Columns[4].ColumnName;
            candleSeries.Data.HighColumn = dataTable.Columns[5].ColumnName;
            candleSeries.Data.VolumeColumn = dataTable.Columns[6].ColumnName;

            candleSeries.Data.DataSource = dataSource;
            candleSeries.DataBind();

            return candleSeries;
        }

        void UltraChart_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

//                if (primitive != null && primitive is Polyline)
//                {
//                    Polyline polyline = (Polyline) primitive;
//                    polyline.PE.ElementType = PaintElementType.SolidFill;
//                    polyline.PE.FillOpacity = 25;
//                }

                if (primitive != null && primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    DateTime dateTimeLabel = Convert.ToDateTime(text.GetTextString());
                    if (candleLabelsDictionary != null && candleLabelsDictionary.ContainsKey(dateTimeLabel))
                    {
                        string shortRegionName = candleLabelsDictionary[dateTimeLabel];
                        text.SetTextString(shortRegionName);
                    }
                }

                if (primitive != null && primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.Y"))
                {
                    Text text = (Text)primitive;
                    text.labelStyle.FontColor = Color.Black;
                    if (ComboKind.SelectedValue == "Уровень регистрируемой безработицы")
                    {
                        double value;
                        if (Double.TryParse(text.GetTextString(), out value))
                        {
                            text.SetTextString(String.Format("{0:N2}%", value));
                        }
                    }
                    else if (ComboKind.SelectedValue == "Число зарегистрированных безработных в расчёте на 1 вакансию")
                    {
                        double value;
                        if (Double.TryParse(text.GetTextString(), out value))
                        {
                            text.SetTextString(String.Format("{0:N2}", value));
                        }
                    }
                    else if (ComboKind.SelectedValue == "Сумма задолженности по выплате заработной платы (млн.руб.)")
                    {
                        double value;
                        if (Double.TryParse(text.GetTextString(), out value))
                        {
                            text.SetTextString(String.Format("{0:N3}", value));
                        }
                    }
                    else
                    {
                        double value;
                        if (Double.TryParse(text.GetTextString(), out value))
                        {
                            text.SetTextString(String.Format("{0:N0}", value));
                        }
                    }
                }

                if (primitive.DataPoint != null)
                {
                    CandleDataPoint dataPoint = (CandleDataPoint)primitive.DataPoint;

                    if (ComboKind.SelectedValue == "Уровень регистрируемой безработицы")
                    {
                        primitive.DataPoint.Label =
                            String.Format(
                                "{6} \nОткрытие ({4:dd.MM.yyyy}) {0:N3}\nЗакрытие ({5:dd.MM.yyyy}) {1:N3}\nМаксимум ({7:dd.MM.yyyy}) {2:N3}\nМинимум ({8:dd.MM.yyyy}) {3:N3} ",
                                dataPoint.Open, dataPoint.Close, dataPoint.High, dataPoint.Low, lastDateTime,
                                currDateTime, candleDT.Rows[primitive.Row][0],
                                CRHelper.DateByPeriodMemberUName(candleDT.Rows[primitive.Row][8].ToString(), 3),
                                CRHelper.DateByPeriodMemberUName(candleDT.Rows[primitive.Row][7].ToString(), 3));
                    }
                    else if (ComboKind.SelectedValue == "Число зарегистрированных безработных в расчёте на 1 вакансию")
                    {
                        primitive.DataPoint.Label =
                            String.Format(
                                "{6} \nОткрытие ({4:dd.MM.yyyy}) {0:N2}\nЗакрытие ({5:dd.MM.yyyy}) {1:N2}\nМаксимум ({7:dd.MM.yyyy}) {2:N2}\nМинимум ({8:dd.MM.yyyy}) {3:N2} ",
                                dataPoint.Open, dataPoint.Close, dataPoint.High, dataPoint.Low, lastDateTime,
                                currDateTime, candleDT.Rows[primitive.Row][0],
                                CRHelper.DateByPeriodMemberUName(candleDT.Rows[primitive.Row][8].ToString(), 3),
                                CRHelper.DateByPeriodMemberUName(candleDT.Rows[primitive.Row][7].ToString(), 3));
                    }
                    else if (ComboKind.SelectedValue == "Количество предприятий, имеющих задолженность по выплате заработной платы")
                    {
                        primitive.DataPoint.Label =
                            String.Format(
                                "{6} \nОткрытие ({4:dd.MM.yyyy}) {0:N0}\nЗакрытие ({5:dd.MM.yyyy}) {1:N0}\nМаксимум ({7:dd.MM.yyyy}) {2:N0}\nМинимум ({8:dd.MM.yyyy}) {3:N0} ",
                                dataPoint.Open, dataPoint.Close, dataPoint.High, dataPoint.Low, lastDateTime,
                                currDateTime, candleDT.Rows[primitive.Row][0],
                                CRHelper.DateByPeriodMemberUName(candleDT.Rows[primitive.Row][8].ToString(), 3),
                                CRHelper.DateByPeriodMemberUName(candleDT.Rows[primitive.Row][7].ToString(), 3));
                    }
                    else if (ComboKind.SelectedValue == "Сумма задолженности по выплате заработной платы (млн.руб.)")
                    {
                        primitive.DataPoint.Label =
                            String.Format(
                                "{6} \nОткрытие ({4:dd.MM.yyyy}) {0:N3}\nЗакрытие ({5:dd.MM.yyyy}) {1:N3}\nМаксимум ({7:dd.MM.yyyy}) {2:N3}\nМинимум ({8:dd.MM.yyyy}) {3:N3} ",
                                dataPoint.Open, dataPoint.Close, dataPoint.High, dataPoint.Low, lastDateTime,
                                currDateTime, candleDT.Rows[primitive.Row][0],
                                CRHelper.DateByPeriodMemberUName(candleDT.Rows[primitive.Row][8].ToString(), 3),
                                CRHelper.DateByPeriodMemberUName(candleDT.Rows[primitive.Row][7].ToString(), 3));
                    }
                    else
                    {
                        primitive.DataPoint.Label =
                            String.Format(
                                "{6} \nОткрытие ({4:dd.MM.yyyy}) {0:N0}\nЗакрытие ({5:dd.MM.yyyy}) {1:N0}\nМаксимум ({7:dd.MM.yyyy}) {2:N0}\nМинимум ({8:dd.MM.yyyy}) {3:N0} ",
                                dataPoint.Open, dataPoint.Close, dataPoint.High, dataPoint.Low, lastDateTime,
                                currDateTime, candleDT.Rows[primitive.Row][0],
                                CRHelper.DateByPeriodMemberUName(candleDT.Rows[primitive.Row][8].ToString(), 3),
                                CRHelper.DateByPeriodMemberUName(candleDT.Rows[primitive.Row][7].ToString(), 3));
                    }
                }

            }
        }

        #endregion
    }
}