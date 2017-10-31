using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.WebUI.UltraWebNavigator;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Color = System.Drawing.Color;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using Font = System.Drawing.Font;

namespace Krista.FM.Server.Dashboards.reports.STAT_0001_0003
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtGrid;
        private DataTable dtDebtsGrid;
        private DataTable dtChart;
        private DataTable dtKoeff;
        private DataTable dtCommentText;
        private DateTime currDateTime;
        private DateTime lastDateTime;
        private DateTime debtsCurrDateTime;
        private DateTime debtsLastDateTime;

        private DateTime lastPrevDateTime;
        private DateTime debtLastPrevDateTime;

        private DateTime redundantLevelRFDateTime;

        public bool IsYearJoint()
        {
            return (currDateTime.Year != lastDateTime.Year);
        }

        #endregion

        #region Параметры запроса

        // Текущая дата
        private CustomParam periodCurrentDate;
        // На неделю назад
        private CustomParam periodLastWeekDate;
        // Выбранная территория
        private CustomParam selectedSubject;
        // Текущий год
        private CustomParam сurrentYear;
        // Прошлый год
        private CustomParam lastYear;

        // Текущая дата для задолженности
        private CustomParam debtsPeriodCurrentDate;
        // На неделю назад для задолженности
        private CustomParam debtsPeriodLastWeekDate;

        // Текущая дата для уровня безработицы по РФ
        private CustomParam redundantLevelRFDate;

        // Мера для УрФО в целом
        private CustomParam urfoMeasure;

        // На неделю назад
        private CustomParam periodPrevLastWeekDate;
        // На неделю назад для задолженности
        private CustomParam debtsPeriodPrevLastWeekDate;

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
            
            UltraWebGrid1.Width = CRHelper.GetGridWidth(MinScreenWidth - 12);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(MinScreenHeight - 120);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid1.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraWebGrid2.Width = CRHelper.GetGridWidth(MinScreenWidth - 12);
            UltraWebGrid2.Height = CRHelper.GetGridHeight(MinScreenHeight - 120);
            UltraWebGrid2.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid2.DataBound += new EventHandler(UltraWebGrid_DataBound);

            double chartHeight = IsSmallResolution ? MinScreenHeight * 0.8 : MinScreenHeight * 0.6;

            UltraChart1.Width = CRHelper.GetChartWidth(MinScreenWidth - 15);
            UltraChart1.Height = CRHelper.GetChartHeight(chartHeight - 110);

            UltraChart2.Width = CRHelper.GetChartWidth(MinScreenWidth - 15);
            UltraChart2.Height = CRHelper.GetChartHeight(chartHeight - 110);

            UltraChart3.Width = CRHelper.GetChartWidth(MinScreenWidth - 15);
            UltraChart3.Height = CRHelper.GetChartHeight(chartHeight - 110);

            UltraChart4.Width = CRHelper.GetChartWidth(MinScreenWidth - 15);
            UltraChart4.Height = CRHelper.GetChartHeight(chartHeight - 110);

            #region Инициализация параметров запроса

            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (periodLastWeekDate == null)
            {
                periodLastWeekDate = UserParams.CustomParam("period_last_week_date");
            }
            if (debtsPeriodCurrentDate == null)
            {
                debtsPeriodCurrentDate = UserParams.CustomParam("period_current_date_debts");
            }
            if (debtsPeriodLastWeekDate == null)
            {
                debtsPeriodLastWeekDate = UserParams.CustomParam("period_last_week_date_debts");
            }
            if (selectedSubject == null)
            {
                selectedSubject = UserParams.CustomParam("selected_subject");
            }
            if (сurrentYear == null)
            {
                сurrentYear = UserParams.CustomParam("current_year");
            }
            if (lastYear == null)
            {
                lastYear = UserParams.CustomParam("last_year");
            }
            if (urfoMeasure == null)
            {
                urfoMeasure = UserParams.CustomParam("urfo_measure");
            }
            if (redundantLevelRFDate == null)
            {
                redundantLevelRFDate = UserParams.CustomParam("redundant_level_RF_date");
            }
            if (periodPrevLastWeekDate == null)
            {
                periodPrevLastWeekDate = UserParams.CustomParam("period_prev_last_week_date");
            }
            if (debtsPeriodPrevLastWeekDate == null)
            {
                debtsPeriodPrevLastWeekDate = UserParams.CustomParam("period_prev_last_week_date_debts");
            }

            #endregion

            #region Настройка диаграммы 1

            UltraChart1.ChartType = ChartType.AreaChart;
            UltraChart1.Border.Thickness = 0;

            UltraChart1.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart1.Axis.X.Extent = 50;
            UltraChart1.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.FontColor = Color.Black;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>%";
            UltraChart1.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart1.Axis.Y.Labels.FontColor = Color.Black;
            UltraChart1.Axis.Y.Extent = 40;

            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.Text = "Уровень безработицы";
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.TitleLeft.Margins.Bottom = UltraChart1.Axis.X.Extent + 5;
            UltraChart1.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart1.TitleLeft.FontColor = Color.Black;

            UltraChart1.Data.EmptyStyle.Text = " ";
            UltraChart1.EmptyChartText = " ";

            UltraChart1.AreaChart.NullHandling = NullHandling.DontPlot;

            UltraChart1.AreaChart.LineAppearances.Clear();

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 5;
            UltraChart1.AreaChart.LineAppearances.Add(lineAppearance);

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            UltraChart1.AreaChart.LineAppearances.Add(lineAppearance);

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.None;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 5;
            lineAppearance.LineStyle.MidPointAnchors = false;
            UltraChart1.AreaChart.LineAppearances.Add(lineAppearance);

            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.Margins.Right = IsSmallResolution ? 0 : Convert.ToInt32(UltraChart1.Width.Value) / 3;
            UltraChart1.Legend.SpanPercentage = 14;
            UltraChart1.Legend.Font = new Font("Verdana", 10);

            UltraChart1.InvalidDataReceived +=
                new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(
                    CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.FillSceneGraph +=
                new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

            UltraChart1.ColorModel.ModelStyle = ColorModels.CustomSkin;
            UltraChart1.ColorModel.Skin.PEs.Clear();
            for (int i = 1; i <= 3; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = Color.White;
                Color stopColor = Color.White;
                PaintElementType peType = PaintElementType.Gradient;
                switch (i)
                {
                    case 1:
                        {
                            color = Color.Transparent;
                            stopColor = Color.Gray;
                            peType = PaintElementType.Hatch;
                            pe.Hatch = FillHatchStyle.ForwardDiagonal;
                            break;
                        }
                    case 2:
                        {
                            color = Color.MediumSeaGreen;
                            stopColor = Color.Green;
                            peType = PaintElementType.Gradient;
                            break;
                        }
                    case 3:
                        {
                            color = Color.Red;
                            stopColor = Color.Red;
                            peType = PaintElementType.None;
                            break;
                        }
                }
                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = peType;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = (byte)150;
                pe.FillStopOpacity = (byte)150;
                if (i == 3)
                {
                    pe.Stroke = Color.Red;
                    pe.StrokeWidth = 5;
                }
                UltraChart1.ColorModel.Skin.PEs.Add(pe);
            }

            #endregion

            #region Настройка диаграммы 2

            UltraChart2.ChartType = ChartType.AreaChart;
            UltraChart2.Border.Thickness = 0;

            UltraChart2.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart2.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart2.Axis.X.Extent = 50;
            UltraChart2.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart2.Axis.X.Labels.FontColor = Color.Black;
            UltraChart2.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N1>";
            UltraChart2.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart2.Axis.Y.Extent = 30;
            UltraChart2.Axis.Y.Labels.FontColor = Color.Black;

            UltraChart2.TitleLeft.Visible = true;
            UltraChart2.TitleLeft.Text = "Число безработных,\n чел/на 1 вакансию";
            UltraChart2.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart2.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart2.TitleLeft.Extent = 40;
            UltraChart2.TitleLeft.Margins.Bottom = UltraChart2.Axis.X.Extent + 5;
            UltraChart2.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart2.TitleLeft.FontColor = Color.Black;

            UltraChart2.ColorModel.ModelStyle = ColorModels.PureRandom;

            UltraChart2.Data.EmptyStyle.Text = " ";
            UltraChart2.EmptyChartText = " ";

            UltraChart2.AreaChart.NullHandling = NullHandling.Zero;

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            UltraChart2.AreaChart.LineAppearances.Add(lineAppearance);

            UltraChart2.Legend.Visible = true;
            UltraChart2.Legend.Location = LegendLocation.Top;
            UltraChart2.Legend.Margins.Right = IsSmallResolution ? 5 : Convert.ToInt32(UltraChart2.Width.Value) / 2;
            UltraChart2.Legend.SpanPercentage = 14;
            UltraChart2.Legend.Font = new Font("Verdana", 10);

            UltraChart2.InvalidDataReceived +=
                new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(
                    CRHelper.UltraChartInvalidDataReceived);
            UltraChart2.FillSceneGraph +=
                new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart2_FillSceneGraph);

            #endregion

            #region Настройка диаграммы 3

            UltraChart3.ChartType = ChartType.AreaChart;
            UltraChart3.Border.Thickness = 0;

            UltraChart3.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart3.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart3.Axis.X.Extent = 50;
            UltraChart3.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart3.Axis.X.Labels.FontColor = Color.Black;
            UltraChart3.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart3.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart3.Axis.Y.Labels.FontColor = Color.Black;
            UltraChart3.Axis.Y.Extent = 30;

            UltraChart3.TitleLeft.Visible = true;
            UltraChart3.TitleLeft.Text = "Потребность предприятий\nв работниках, чел.";
            UltraChart3.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart3.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart3.TitleLeft.Extent = 40;
            UltraChart3.TitleLeft.Margins.Bottom = UltraChart3.Axis.X.Extent + 5;
            UltraChart3.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart3.TitleLeft.FontColor = Color.Black;

            UltraChart3.ColorModel.ModelStyle = ColorModels.PureRandom;

            UltraChart3.Data.EmptyStyle.Text = " ";
            UltraChart3.EmptyChartText = " ";

            UltraChart3.AreaChart.NullHandling = NullHandling.Zero;

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            UltraChart3.AreaChart.LineAppearances.Add(lineAppearance);

            UltraChart3.Legend.Visible = true;
            UltraChart3.Legend.Location = LegendLocation.Top;
            UltraChart3.Legend.Margins.Right = IsSmallResolution ? 5 : Convert.ToInt32(UltraChart3.Width.Value) / 2;
            UltraChart3.Legend.SpanPercentage = 14;
            UltraChart3.Legend.Font = new Font("Verdana", 10);

            UltraChart3.InvalidDataReceived +=
                new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(
                    CRHelper.UltraChartInvalidDataReceived);
            UltraChart3.FillSceneGraph +=
                new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart3_FillSceneGraph);

            #endregion

            #region Настройка диаграммы 4

            UltraChart4.ChartType = ChartType.AreaChart;
            UltraChart4.Border.Thickness = 0;

            UltraChart4.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart4.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart4.Axis.X.Extent = 50;
            UltraChart4.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart4.Axis.X.Labels.FontColor = Color.Black;
            UltraChart4.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart4.Axis.Y.Labels.Font = new Font("Verdana", 8);
            UltraChart4.Axis.Y.Labels.FontColor = Color.Black;
            UltraChart4.Axis.Y.Extent = 30;

            UltraChart4.TitleLeft.Visible = true;
            UltraChart4.TitleLeft.Text = "Сумма задолженности по выплате\nзаработной платы, млн.руб.";
            UltraChart4.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart4.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart4.TitleLeft.Extent = 40;
            UltraChart4.TitleLeft.Margins.Bottom = UltraChart4.Axis.X.Extent + 5;
            UltraChart4.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart4.TitleLeft.FontColor = Color.Black;

            UltraChart4.ColorModel.ModelStyle = ColorModels.PureRandom;

            UltraChart4.Data.EmptyStyle.Text = " ";
            UltraChart4.EmptyChartText = " ";

            UltraChart4.AreaChart.NullHandling = NullHandling.Zero;

            lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Small;
            lineAppearance.Thickness = 3;
            UltraChart4.AreaChart.LineAppearances.Add(lineAppearance);

            UltraChart4.Legend.Visible = true;
            UltraChart4.Legend.Location = LegendLocation.Top;
            UltraChart4.Legend.Margins.Right = IsSmallResolution ? 5 : Convert.ToInt32(UltraChart4.Width.Value) / 2;
            UltraChart4.Legend.SpanPercentage = 14;
            UltraChart4.Legend.Font = new Font("Verdana", 10);

            UltraChart4.InvalidDataReceived +=
                new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(
                    CRHelper.UltraChartInvalidDataReceived);
            UltraChart4.FillSceneGraph +=
                new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart4_FillSceneGraph);

            #endregion

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting +=
                new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                ComboPeriod.Width = 150;
                ComboPeriod.MultiSelect = false;
                ComboPeriod.ShowSelectedValue = false;
                ComboPeriod.ParentSelect = false;
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillLabourMarketNonEmptyDays(DataDictionariesHelper.LabourMarketNonEmptyDays));
                ComboPeriod.SelectLastNode();
                ComboPeriod.PanelHeaderTitle = "Выберите дату";

                ComboLastPeriod.Width = 300;
                ComboLastPeriod.MultiSelect = false;
                ComboLastPeriod.ShowSelectedValue = false;
                ComboLastPeriod.ParentSelect = false;
                ComboLastPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillLabourMarketNonEmptyDays(DataDictionariesHelper.LabourMarketNonEmptyDays));
                ComboLastPeriod.SelectLastNode();
                ComboLastPeriod.RemoveTreeNodeByName(ComboLastPeriod.SelectedValue);
                ComboLastPeriod.SelectLastNode();
                if (CRHelper.IsMonthCaption(ComboLastPeriod.SelectedValue.Trim(' ')))
                {
                    ComboLastPeriod.RemoveTreeNodeByName(ComboLastPeriod.SelectedValue);
                    ComboLastPeriod.SelectLastNode();
                }
                if (ComboLastPeriod.SelectedValue.Trim(' ').Length == 4)
                {
                    ComboLastPeriod.RemoveTreeNodeByName(ComboLastPeriod.SelectedValue);
                    ComboLastPeriod.SelectLastNode();
                }
                ComboLastPeriod.PanelHeaderTitle = "Выберите дату для сравнения";

                ComboRegion.Width = 300;
                ComboRegion.MultiSelect = false;
                ComboRegion.ParentSelect = true;
                ComboRegion.FillDictionaryValues((CustomMultiComboDataHelper.FillFOSubjectList(DataDictionariesHelper.FOSubjectList, true)));
                ComboRegion.Title = "Территория";
                ComboRegion.SetСheckedState("Курганская область", true);
            }

            
            currDateTime = GetDateString(ComboPeriod.GetSelectedNodePath(), ComboPeriod.SelectedNode.Level);
            //lastDateTime = currDateTime.AddDays(-7);

            Node selectedNode = ComboPeriod.SelectedNode;
            // если выбран месяц, то берем в нем последний день
            if (selectedNode.Nodes.Count != 0)
            {
                selectedNode = ComboPeriod.GetLastChild(selectedNode);
            }

            lastDateTime = GetDateString(ComboLastPeriod.GetSelectedNodePath(), ComboLastPeriod.SelectedNode.Level);

            if (lastDateTime > currDateTime)
            {
                DateTime temp = lastDateTime;
                lastDateTime = currDateTime;
                currDateTime = temp;
            }

            Page.Title = string.Format("Мониторинг ситуации на рынке труда ({0})", ComboRegion.SelectedValue);
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("Форма мониторинга ситуации на рынке труда по состоянию на {0:dd.MM.yyyy}, динамика к {1:dd.MM.yyyy} (по данным органов государственной власти субъектов Российской Федерации)", currDateTime, lastDateTime);
            chart1ElementCaption.Text = string.Format("Динамика уровня регистрируемой безработицы на рынке труда ({0})",
                    RegionsNamingHelper.ShortName(ComboRegion.SelectedValue));
            chart2ElementCaption.Text = string.Format("Динамика числа безработных на 1 вакансию ({0})",
        RegionsNamingHelper.ShortName(ComboRegion.SelectedValue));

            chart3ElementCaption.Text = string.Format("Динамика потребности предприятий в работниках ({0})",
RegionsNamingHelper.ShortName(ComboRegion.SelectedValue));

            chart4ElementCaption.Text = string.Format("Динамика суммы задолженности по выплате заработной платы ({0})",
RegionsNamingHelper.ShortName(ComboRegion.SelectedValue));

            CommentText1.Text = string.Empty;
            CommentText2.Text = string.Empty;

            int days = currDateTime.DayOfYear - lastDateTime.DayOfYear;
            days += 365 * (currDateTime.Year - lastDateTime.Year);

            lastPrevDateTime = new DateTime(lastDateTime.Year, lastDateTime.Month, lastDateTime.Day);
            lastPrevDateTime = lastPrevDateTime.AddDays(-days);
            periodPrevLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", lastPrevDateTime, 5);

            string queryDate = DataProvider.GetQueryText("STAT_0001_0002_date_prev_last");
            DataTable dtLastDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(queryDate, dtLastDate);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", currDateTime, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", lastDateTime, 5);
            periodPrevLastWeekDate.Value = dtLastDate.Rows[0][5].ToString();

            сurrentYear.Value = currDateTime.Year.ToString();
            lastYear.Value = (currDateTime.Year - 1).ToString();

            debtsPeriodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", lastDateTime, 5);

            string query = DataProvider.GetQueryText("STAT_0001_0003_debts_date");
            DataTable dtDebtsDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsDate);

            if (dtDebtsDate.Rows.Count == 2)
            {
                if (dtDebtsDate.Rows[0][1] != DBNull.Value && dtDebtsDate.Rows[0][1].ToString() != string.Empty)
                {
                    debtsPeriodLastWeekDate.Value = dtDebtsDate.Rows[0][1].ToString();
                    debtsLastDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[0][1].ToString(), 3);
                }
                if (dtDebtsDate.Rows[1][1] != DBNull.Value && dtDebtsDate.Rows[1][1].ToString() != string.Empty)
                {
                    debtsPeriodCurrentDate.Value = dtDebtsDate.Rows[1][1].ToString();
                    debtsCurrDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[1][1].ToString(), 3);
                }
            }
            else if (dtDebtsDate.Rows.Count == 3)
            {
                if (dtDebtsDate.Rows[1][1] != DBNull.Value && dtDebtsDate.Rows[1][1].ToString() != string.Empty)
                {
                    debtsPeriodLastWeekDate.Value = dtDebtsDate.Rows[1][1].ToString();
                    debtsLastDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[1][1].ToString(), 3);
                }
                if (dtDebtsDate.Rows[2][1] != DBNull.Value && dtDebtsDate.Rows[2][1].ToString() != string.Empty)
                {
                    debtsPeriodCurrentDate.Value = dtDebtsDate.Rows[2][1].ToString();
                    debtsCurrDateTime = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[2][1].ToString(), 3);
                }
            }

            days = debtsCurrDateTime.DayOfYear - debtsLastDateTime.DayOfYear;
            days += 365 * (debtsCurrDateTime.Year - debtsLastDateTime.Year);

            debtLastPrevDateTime = new DateTime(debtsLastDateTime.Year, debtsLastDateTime.Month, debtsLastDateTime.Day);
            debtLastPrevDateTime = debtLastPrevDateTime.AddDays(-days);
            debtsPeriodPrevLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", debtLastPrevDateTime, 5);

            queryDate = DataProvider.GetQueryText("STAT_0001_0002_date_prev_last_debts");
            dtLastDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(queryDate, dtLastDate);

            debtsPeriodPrevLastWeekDate.Value = dtLastDate.Rows[0][5].ToString();

            query = DataProvider.GetQueryText("STAT_0001_0002_redundantLevelRF_date");
            DataTable dtRedundantLevelRFDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtRedundantLevelRFDate);
            redundantLevelRFDate.Value = dtRedundantLevelRFDate.Rows[0][1].ToString();
            redundantLevelRFDateTime = CRHelper.DateByPeriodMemberUName(dtRedundantLevelRFDate.Rows[0][1].ToString(), 3);

            if (ComboRegion.SelectedIndex != 0)
            {
                selectedSubject.Value = string.Format("[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[Уральский федеральный округ].[{0}]", ComboRegion.SelectedValue);
                urfoMeasure.Value = "[Measures].[В целом по УрФО],";

                UltraWebGrid1.Bands.Clear();
                UltraWebGrid1.DataBind();

                UltraWebGrid2.Bands.Clear();
                UltraWebGrid2.DataBind();

                commentTextTable.Visible = true;
                ultraGridTable.Visible = true;
                UltraGridExporter1.ExcelExportButton.Visible = true;
            }
            else
            {
                UltraChart1.ColorModel.ModelStyle = ColorModels.CustomLinear;

                selectedSubject.Value = "[Территории].[Сопоставимый].[Все территории].[Российская  Федерация].[Уральский федеральный округ]";
                urfoMeasure.Value = " ";

                commentTextTable.Visible = false;
                ultraGridTable.Visible = false;
                UltraGridExporter1.ExcelExportButton.Visible = false;
            }

            UltraChart1.DataBind();
            UltraChart2.DataBind();
            UltraChart3.DataBind();
            UltraChart4.DataBind();
        }

        public DateTime GetDateString(string source, int level)
        {
            string[] sts = source.Split('|');
            if (sts.Length > 1)
            {
                switch (level)
                {
                    // нулевой уровень выбрать нельзя
                    case 1:
                        {
                            return GetDateString(ComboPeriod.GetNodeLastChild(ComboPeriod.SelectedNode), level + 1);
                        }
                    case 2:
                        {
                            string month = sts[1].TrimEnd(' ');
                            string day = sts[2].TrimEnd(' ');
                            return new DateTime(Convert.ToInt32(sts[0]), CRHelper.MonthNum(month), Convert.ToInt32(day));
                        }
                }
            }
            return DateTime.MinValue;
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0003_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                DataColumn numberColumn = new DataColumn("№", typeof(string));
                dtGrid.Columns.Add(numberColumn);

                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString().Split(';').Length > 0)
                    {
                        bool isRank = false;
                        string rowString = string.Empty;
                        if ((row[0].ToString().Contains("Уровень регистрируемой безработицы") ||
                             row[0].ToString().Contains("Число зарегистрированных безработных в расчёте на 1 вакансию")) && row[0].ToString().Contains("Прирост"))
                        {
                            rowString = row[0].ToString().Split(';')[0];
                            row[0] = "ранг по УрФО";
                            isRank = true;
                        }

                        row[0] = row[0].ToString().Split(';')[0];

                        if (DataDictionariesHelper.LabourMarketIndicatorNumbers.ContainsKey(row[0].ToString()) ||
                            (isRank && DataDictionariesHelper.LabourMarketIndicatorNumbers.ContainsKey(rowString)))
                        {
                            if (isRank)
                            {
                                row["№"] = DataDictionariesHelper.GetLabourMarketIndicatorNumber(rowString);
                            }
                            else
                            {
                                row["№"] = DataDictionariesHelper.GetLabourMarketIndicatorNumber(row[0].ToString());
                            }
                        }
                        if (row[0].ToString() == "Целевое значение по уровню зарегистрированных безработных ")
                        {
                            row["№"] = 1;
                        } 
                    }
                }

                ((UltraWebGrid)sender).DataSource = dtGrid;

                CommentTextDataBind();
            }
        }

        protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0003_debts_grid");
            dtDebtsGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtDebtsGrid);
            if (dtDebtsGrid.Rows.Count > 0)
            {
                DataColumn numberColumn = new DataColumn("№", typeof(string));
                dtDebtsGrid.Columns.Add(numberColumn);

                foreach (DataRow row in dtDebtsGrid.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString().Split(';').Length > 0)
                    {
                        row[0] = row[0].ToString().Split(';')[0];
                        if (DataDictionariesHelper.LabourMarketIndicatorNumbers.ContainsKey(row[0].ToString()))
                        {
                            row["№"] = DataDictionariesHelper.GetLabourMarketIndicatorNumber(row[0].ToString());
                        }
                    }
                }

                ((UltraWebGrid)sender).DataSource = dtDebtsGrid;
            }
        }

        protected void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid1.Height = Unit.Empty;

            UltraWebGrid2.Width = Unit.Empty;
            UltraWebGrid2.Height = Unit.Empty;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.RowAlternateStyleDefault.BackColor = Color.White;
            e.Layout.RowSelectorsDefault = IsSmallResolution ? RowSelectors.No : RowSelectors.Yes;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            DateTime lastDT = (sender == UltraWebGrid1) ? lastDateTime : debtsLastDateTime;
            DateTime currDT = (sender == UltraWebGrid1) ? currDateTime : debtsCurrDateTime;

            // перемещаем колонку с номером в начало
            UltraGridColumn numberColumn = e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1];
            e.Layout.Bands[0].Columns.RemoveAt(e.Layout.Bands[0].Columns.Count - 1);
            e.Layout.Bands[0].Columns.Insert(0, numberColumn);

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                int widthColumn = i < e.Layout.Bands[0].Columns.Count - 2 ? 63 : 70;

                if (IsSmallResolution && i % 4 == 0)
                {
                    widthColumn = 55;
                }

                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(20);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(IsSmallResolution ? 185 : 240);
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].MergeCells = true;

            for (int i = 0; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                if (i == 0 || i == 1)
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 0;
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.SpanY = 2;
                }
                else
                {
                    e.Layout.Bands[0].Columns[i].Header.RowLayoutColumnInfo.OriginY = 1;
                }
            }

            int multiHeaderPos = 2;

            for (int i = 2; i < e.Layout.Bands[0].Columns.Count; i = i + 4)
            {
                ColumnHeader ch = new ColumnHeader(true);
                ch.Caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';')[0];

                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i, string.Format("{0:dd.MM}", lastDT), "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 1, string.Format("{0:dd.MM}", currDT), "");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 2, "Доля в УрФО", "Доля значения субъекта в сумме по УрФО");
                CRHelper.SetHeaderCaption(e.Layout.Grid, 0, i + 3, string.Format("АППГ ({0}г.)", currDateTime.Year - 1), "Значение показателя за аналогичный период прошлого года");

                //                if (IsSmallResolution)
                //                {
                //                    e.Layout.Bands[0].Columns[i].Hidden = true;
                //                }

                ch.RowLayoutColumnInfo.OriginY = 0;
                ch.RowLayoutColumnInfo.OriginX = multiHeaderPos;
                multiHeaderPos += 4;
                ch.RowLayoutColumnInfo.SpanX = 4;
                e.Layout.Bands[0].HeaderLayout.Add(ch);
            }
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int cellsCount = e.Row.Cells.Count;

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                int rowIndex = e.Row.Index;

                // номер показателя
                int k = (rowIndex % 3);
                // строка с уровнем регистрируемой безработицы
                bool isRedundantLevel = (e.Row.Cells[1].Value != null &&
                                         e.Row.Cells[1].Value.ToString().ToLower().Contains("уровень регистрируемой безработицы"));
                bool isRedundantLevelRank = (e.Row.PrevRow != null && e.Row.PrevRow.Cells[1].Value != null &&
                                             e.Row.PrevRow.Cells[1].Value.ToString().ToLower().Contains("уровень регистрируемой безработицы"));
                // строка с рангом уровня регистрируемой безработицы
                bool isRankRow = (e.Row.Cells[1].Value != null &&
                                  e.Row.Cells[1].Value.ToString().Contains("ранг по УрФО"));
                // яркая колонка
                bool bright = (i % 2 != 0);

                // доля
                bool share = (i % 4 == 0);

                // АППГ
                bool appg = ((i - 1) % 4 == 0);

                if (i != 0 && appg)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthRight = 2;
                }
                else
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthLeft = 2;
                }

                if (i == 1 && isRankRow)
                {
                    e.Row.Cells[i].Style.HorizontalAlign = HorizontalAlign.Right;
                    if (e.Row.PrevRow != null && e.Row.PrevRow.PrevRow != null)
                    {
                        e.Row.PrevRow.PrevRow.Cells[i].Style.BorderDetails.WidthBottom = 0;
                    }
                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 0;
                }

                if (i > 1)
                {
                    switch (k)
                    {
                        case 0:
                            {
                                e.Row.Cells[i].Style.BorderDetails.WidthBottom = 0;
                                break;
                            }
                        case 1:
                            {
                                e.Row.Cells[i].Style.BorderDetails.WidthTop = 0;
                                e.Row.Cells[i].Style.BorderDetails.WidthBottom = 0;
                                break;
                            }
                        case 2:
                            {
                                e.Row.Cells[i].Style.BorderDetails.WidthTop = 0;
                                break;
                            }
                    }
                }

                if (i > 1 && e.Row.Cells[1].Value.ToString() == "Целевое значение по уровню зарегистрированных безработных " &&
                    e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[1].Value.ToString() == "Целевое значение по уровню зарегистрированных безработных ")
                {
                    e.Row.Cells[i].Value = null;
                }

                if (i != 0 && i != 1 && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    double value = Convert.ToDouble(e.Row.Cells[i].Value);

                    bool growRate = (k == 1);
                    bool rate = (!isRedundantLevel && k == 2);
                    bool mlnUnit = (e.Row.Cells[0].Value != null && e.Row.Cells[1].Value.ToString().Contains("млн.руб."));
                    bool thsUnit = (e.Row.Cells[0].Value != null && e.Row.Cells[1].Value.ToString().Contains("Число зарегистрированных безработных в расчёте на 1 вакансию"));

                    if (growRate)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (sender == UltraWebGrid1)
                            {
                                if (i == 3 || i == 7)
                                {
                                    e.Row.Cells[i].Title = appg
                                                               ? "Темп прироста к аналогичному периоду прошлого года"
                                                               : String.Format("Темп прироста к {0:dd.MM.yyyy}", lastDateTime);
                                }
                                else
                                {
                                    e.Row.Cells[i].Title = appg
                                                               ? "Темп прироста к аналогичному периоду прошлого года"
                                                               : String.Format("Темп прироста к {0:dd.MM.yyyy}", lastPrevDateTime);
                                }
                            }
                            else
                            {
                                if (i == 2 || i == 6)
                                {
                                    e.Row.Cells[i].Title = String.Format("Темп прироста к {0:dd.MM.yyyy}", debtLastPrevDateTime);
                                }
                                else
                                {
                                    e.Row.Cells[i].Title = String.Format("Темп прироста к {0:dd.MM.yyyy}", debtsLastDateTime);
                                }
                            }
                            if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = bright ? "~/images/arrowRedUpBB.png" : "~/images/arrowRedUpBBdim.png";
                            }
                            else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = bright ? "~/images/arrowGreenDownBB.png" : "~/images/arrowGreenDownBBdim.png";
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }

                    if (!isRankRow && rate)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (100 * Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                            {
                                e.Row.Cells[i].Title = (sender == UltraWebGrid1) ? "Прирост к прошлой неделе" : "Прирост к прошлой дате";
                            }
                            else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                            {
                                e.Row.Cells[i].Title = (sender == UltraWebGrid1) ? "Падение относительно прошлой недели" : "Падение относительно прошлой даты";
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }

                    if (isRankRow)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = bright ? "~/images/starGrayBB.png" : "~/images/starGrayBBdim.png";
                                e.Row.Cells[i].Title = isRedundantLevelRank ? "Самый высокий уровень безработицы" : "Самое большое число безработных на 1 вакансию";
                            }
                            else if (Convert.ToInt32(e.Row.Cells[i].Value) == 6)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = bright ? "~/images/starYellowBB.png" : "~/images/starYellowBBdim.png";
                                e.Row.Cells[i].Title = isRedundantLevelRank ? "Самый низкий уровень безработицы" : "Самое маленькое число безработных на 1 вакансию";
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }

                    switch (k)
                    {
                        case 0:
                            {
                                e.Row.Cells[i].Style.Font.Bold = true;
                                if (share)
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:P2}", value);
                                }
                                else
                                    if (mlnUnit)
                                    {
                                        e.Row.Cells[i].Value = string.Format("{0:N3}", value);
                                    }
                                    else if (thsUnit)
                                    {
                                        e.Row.Cells[i].Value = string.Format("{0:N2}", value);
                                    }
                                    else if (isRedundantLevel)
                                    {
                                        e.Row.Cells[i].Value = string.Format("{0:N3}%", value);
                                    }
                                    else if (e.Row.Cells[1].Value.ToString() == "Целевое значение по уровню зарегистрированных безработных ")
                                    {
                                        e.Row.Cells[i].Value = string.Format("{0:N2}%", e.Row.Cells[i].Value);
                                    }
                                    else
                                    {
                                        e.Row.Cells[i].Value = string.Format("{0:N0}", value);
                                    }
                                break;
                            }
                        case 1:
                            {
                                e.Row.Cells[i].Value = string.Format("{0:P2}", value);
                                break;
                            }
                        case 2:
                            {
                                if (mlnUnit)
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:N3}", value);
                                }
                                else if (thsUnit)
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:N2}", value);
                                }
                                else
                                {
                                    e.Row.Cells[i].Value = string.Format("{0:N0}", value);
                                }
                                break;
                            }
                    }
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    string cellValue = cell.Value.ToString();
                    if (cellValue.Contains("%"))
                    {
                        cellValue = cellValue.TrimEnd('%');
                    }

                    decimal value;
                    if (i != 0 && (k == 1 || k == 2) && decimal.TryParse(cellValue, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out value))
                    {
                        if (value > 0)
                        {
                            cell.Value = string.Format("+{0}", cell.Value);
                        }
                    }

                    e.Row.Cells[i].Style.Padding.Right = (i == 0) ? 1 : 5;

                    if (k != 1 && i >= cellsCount - 2)
                    {
                        cell.Style.Font.Bold = true;
                    }

                    if (i != 0 && !share && !bright)
                    {
                        cell.Style.ForeColor = Color.DimGray;
                    }
                }
            }
        }

        #endregion

        #region Комментарий к таблице

        private void CommentTextDataBind()
        {
            string query = DataProvider.GetQueryText("STAT_0001_0003_commentText");
            dtCommentText = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtCommentText);

            if (dtCommentText.Rows.Count > 0)
            {
                string dateTimeStr = string.Format("{0:dd.MM.yyyy}", currDateTime);
                string dateTimeDebtsStr = string.Format("{0:dd.MM.yyyy}", debtsCurrDateTime);
                string dateLastTimeDebtsStr = string.Format("{0:dd.MM.yyyy}", debtsLastDateTime);
                double totalCount = GetDoubleDTValue(dtCommentText, "Общая численность");
                double totalRate = GetDoubleDTValue(dtCommentText, "Общий темп прироста");
                double totalGrow = GetDoubleDTValue(dtCommentText, "Общий прирост");
                string totalRateArrow = totalRate > 0
                                               ? "Прирост <img src=\"../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">"
                                               : "Снижение <img src=\"../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">";
                string totalRateStr = totalRate > 0 ? "составил" : "составило";
                string totalRatePlus = totalRate > 0 ? "+" : string.Empty;

                double factoryCount = GetDoubleDTValue(dtCommentText, "Число организаций");
                double incompleteEmployersCount = GetDoubleDTValue(dtCommentText, "Численность работников с неполной занятостью");
                double totalDebts = GetDoubleDTValue(dtCommentText, "Cумма задолженности");
                double totalLastWeekDebts = GetDoubleDTValue(dtCommentText, "Cумма задолженности прошлая неделя");
                double slavesCount = GetDoubleDTValue(dtCommentText, "Количество граждан, имеющих задолженность");
                double debtsPercent = GetDoubleDTValue(dtCommentText, "Прирост задолженности");
                string debtsPercentArrow = debtsPercent == 0
                                               ? "не изменилась"
                                               : debtsPercent > 0
                                               ? string.Format("увеличилась <img src=\"../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\"> на <b>{0:N3}</b>&nbsp;млн.руб", Math.Abs(debtsPercent))
                                               : string.Format("уменьшилась <img src=\"../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\"> на <b>{0:N3}</b>&nbsp;млн.руб", Math.Abs(debtsPercent));

                double almostRedundantsCount = GetDoubleDTValue(dtCommentText, "Численность работников, предполагаемых к увольнению");
                double almostRedundantsRate = GetDoubleDTValue(dtCommentText, "Прирост числа работников, предполагаемых к увольнению");
                string monitoringStartStr1 = currDateTime.Year == 2009
                                ? "С начала проведения мониторинга (с октября 2008 года)"
                                : "С начала года";
                string monitoringStartStr2 = currDateTime.Year == 2009
                                ? "С начала проведения мониторинга"
                                : "С начала года";
                double redundantsTotal = GetDoubleDTValue(dtCommentText, "Численность работников, уволенных с начала мониторинга");
                double redundantsTotalRate = GetDoubleDTValue(dtCommentText, "Прирост числа работников, уволенных за отчетный период");

                double redundantlevelValue = GetDoubleDTValue(dtCommentText, "Уровень регистрируемой безработицы ");
                double redundantlevelGrow = GetDoubleDTValue(dtCommentText, "Прирост уровня регистрируемой безработицы");
                string redundantlevelArrow = redundantlevelGrow == 0
                                               ? "не изменился и составляет"
                                               : redundantlevelGrow > 0
                                                ? string.Format("увеличился <img src=\"../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\"> на <b>{0:N3}</b>&nbsp;процентных пункта и на <b>{1}</b> составил", Math.Abs(redundantlevelGrow), dateTimeStr)
                                                : string.Format("уменьшился <img src=\"../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\"> на <b>{0:N3}</b>&nbsp;процентных пункта и на <b>{1}</b> составил", Math.Abs(redundantlevelGrow), dateTimeStr);



                double redundantLevelURFOValue =
                    GetDoubleDTValue(dtCommentText, "Уровень регистрируемой безработицы УрФО");
                string redundantLevelURFOArrow;
                string redundantLevelURFODescription = String.Format("(<b>{0:N3}%</b>)", redundantLevelURFOValue);
                if (redundantlevelValue > redundantLevelURFOValue)
                {
                    redundantLevelURFOArrow = "<nobr>выше <img src=\"../../images/ballRedBB.png\"> уровня</nobr>";
                }
                else if (redundantlevelValue < redundantLevelURFOValue)
                {
                    redundantLevelURFOArrow = "<nobr>ниже <img src=\"../../images/ballGreenBB.png\"> уровня</nobr>";
                }
                else
                {
                    redundantLevelURFOArrow = "<nobr>соответствует <img src=\"../../images/ballGreenBB.png\"> уровню</nobr>";
                    redundantLevelURFODescription = String.Empty;
                }
                string redundantLevelURFOGrow =
                    String.Format(" {0} безработицы в целом по УрФО {1}", redundantLevelURFOArrow,
                                  redundantLevelURFODescription);

                double redundantLevelRFValue = GetDoubleDTValue(dtCommentText, "Уровень регистрируемой безработицы РФ");
                string redundantLevelRFArrow;
                string redundantLevelRFDescription = String.Format("(<b>{0:N3}%</b>)", redundantLevelRFValue);
                if (redundantlevelValue > redundantLevelRFValue)
                {
                    redundantLevelRFArrow = "<nobr>выше <img src=\"../../images/ballRedBB.png\"> уровня</nobr>";
                }
                else if (redundantlevelValue < redundantLevelRFValue)
                {
                    redundantLevelRFArrow = "<nobr>ниже <img src=\"../../images/ballGreenBB.png\"> уровня</nobr>";
                }
                else
                {
                    redundantLevelRFArrow = "<nobr>соответствует <img src=\"../../images/ballGreenBB.png\"> уровню</nobr>";
                    redundantLevelRFDescription = String.Empty;
                }

                double targetValue = GetDoubleDTValue(dtCommentText, "Целевое значение по уровню зарегистрированных безработных ");
                string targerRFArrow;
                string targetDescription = String.Format("(<b>{0:N3}%</b>)", targetValue);
                if (redundantlevelValue > targetValue)
                {
                    targerRFArrow = "<nobr>выше <img src=\"../../images/ballRedBB.png\"> целевого значения</nobr>";
                }
                else if (redundantlevelValue < targetValue)
                {
                    targerRFArrow = "<nobr>ниже <img src=\"../../images/ballGreenBB.png\"> целевого значения</nobr>";
                }
                else
                {
                    targerRFArrow = "<nobr>соответствует <img src=\"../../images/ballGreenBB.png\"> целевому значению</nobr>";
                    targetDescription = String.Empty;
                }

                string redundantLevelRFGrow = String.Format("{0} безработицы в целом по РФ {1} и {2} по уровню зарегистрированных безработных {3}", redundantLevelRFArrow, redundantLevelRFDescription, targerRFArrow, targetDescription);
                string redundantLevelRfAndUrfoGrow = String.Format(", что {0} и {1}", redundantLevelURFOGrow, redundantLevelRFGrow);

                double vacancyCount = GetDoubleDTValue(dtCommentText, "Потребность в работниках");
                double vacancyCountGrow = GetDoubleDTValue(dtCommentText, "Прирост потребности в работниках");
                string vacancyCountGrowArrow = vacancyCountGrow == 0
                               ? "не изменилась и составляет"
                               : vacancyCountGrow > 0
                               ? string.Format("увеличилась <img src=\"../../images/arrowGreenUpBB.png\" width=\"13px\" height=\"16px\"> на <b>{0:N0}</b>&nbsp;единиц и составила", Math.Abs(vacancyCountGrow))
                               : string.Format("уменьшилась <img src=\"../../images/arrowRedDownBB.png\" width=\"13px\" height=\"16px\"> на <b>{0:N0}</b>&nbsp;единиц и составила", Math.Abs(vacancyCountGrow));
                string freeVacancyCountStr = (totalCount - vacancyCount) > 0
                       ? string.Format("ниже количества безработных на <b>{0:N0}</b>&nbsp;единиц", Math.Abs(totalCount - vacancyCount))
                       : string.Format("выше количества безработных на <b>{0:N0}</b>&nbsp;единиц", Math.Abs(totalCount - vacancyCount));
                double tensionKoeff = GetDoubleDTValue(dtCommentText, "Число зарегистрированных безработных в расчёте на 1 вакансию", double.MinValue);
                double tensionKoeffGrow = GetDoubleDTValue(dtCommentText, "Прирост числа зарегистрированных безработных в расчёте на 1 вакансию", double.MinValue);
                string tensionKoeffGrowArrow = tensionKoeffGrow != double.MinValue ?
                                               tensionKoeffGrow > 0
                                                ? "выросло <img src=\"../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\"> до"
                                                : "снизилось <img src=\"../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\"> до"
                                                : "составило";

                int koeffNumber = GetKoeffNumber(ComboRegion.SelectedValue);
                DataRow koeffRow = dtKoeff.Rows[koeffNumber];

                double bi = 0;
                if (koeffRow[2] != DBNull.Value && koeffRow[2].ToString() != string.Empty)
                {
                    bi = Convert.ToDouble(koeffRow[2]);
                }
                double forecastRateGrow = Math.Pow(10, bi) - 1;
                string forecastRageGrowArrow = forecastRateGrow > 0
                       ? "прироста <img src=\"../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">"
                       : "сокращения <img src=\"../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">";

                DateTime nextDateTime = currDateTime.AddMonths(1);
                int nextMonthNumber = nextDateTime.Month;

                double b0 = Convert.ToDouble(koeffRow[1]);
                int monthCount = currDateTime.Month != 12 ? (currDateTime.Year - 2007) * 12 + nextMonthNumber - 1 : (currDateTime.Year - 2007 + 1) * 12;
                double xi = Convert.ToDouble(koeffRow[2]) * (monthCount);
                double koeff = (nextDateTime.Month == 12) ? 0 : Convert.ToDouble(koeffRow[nextMonthNumber + 2]);
                double logForecast = b0 + xi + koeff;
                double forecastValue = Math.Pow(10, logForecast);

                string str1 = string.Format(@"&nbsp;&nbsp;&nbsp;Численность безработных граждан, зарегистрированных в органах службы занятости, 
по состоянию на <b>{0}</b> составила <b>{1:N3}</b> тыс.человек.<br/>",
                    dateTimeStr, totalCount / 1000);

                string str2 = string.Format(@"&nbsp;&nbsp;&nbsp;{0} числа безработных граждан за период с <b>{1:dd.MM}</b> по <b>{2:dd.MM}</b> в <b>{6}</b>
{5} <b>{3:N0}</b>&nbsp;чел. (темп прироста <b>{7}{4:P2}</b>).<br/>",
    totalRateArrow, lastDateTime, currDateTime, Math.Abs(totalGrow), totalRate, totalRateStr, RegionsNamingHelper.ShortName(ComboRegion.SelectedValue), totalRatePlus);

                string str3 = string.Format(@"&nbsp;&nbsp;&nbsp;Уровень регистрируемой безработицы в течение отчетного периода {0} <b>{1:P3}</b>{2}.<br/>",
                    redundantlevelArrow, redundantlevelValue / 100, redundantLevelRfAndUrfoGrow);

                string str4 = string.Format(@"&nbsp;&nbsp;&nbsp;Потребность в работниках, заявленная работодателями в органы службы занятости населения, 
{0} <b>{1:N0}</b>&nbsp;вакансий, что {2}.<br/>",
   vacancyCountGrowArrow, vacancyCount, freeVacancyCountStr);

                string str5 = tensionKoeff != double.MinValue ? string.Format(@"&nbsp;&nbsp;&nbsp;Число зарегистрированных безработных в расчёте на 1 вакансию за период с <b>{0:dd.MM}</b> по <b>{1:dd.MM}</b> {2} <b>{3:N2}</b>
безработных граждан на одну вакансию.<br/>", lastDateTime, currDateTime, tensionKoeffGrowArrow, tensionKoeff) : string.Empty;

                string str6 = string.Format(@"&nbsp;&nbsp;&nbsp;{1} заявленная численность 
работников, предполагаемых к увольнению (ликвидация  организаций, сокращение численности или штата), составляет <b>{0:N0}</b>&nbsp;чел.", almostRedundantsCount, monitoringStartStr1);

                string str7 = !IsYearJoint() ? (almostRedundantsRate > 0) ? string.Format(@"&nbsp;Количество граждан, предполагаемых к увольнению, возросло <img src='../../images/arrowRedUpBB.png' width='13px' height='16px'> на <b>{0:N0}</b>&nbsp;чел. 
в сравнении с <b>{1:dd.MM}</b>.<br/>", Math.Abs(almostRedundantsRate), lastDateTime) : string.Format(@"&nbsp;Количество граждан, предполагаемых к увольнению, сократилось <img src='../../images/arrowGreenDownBB.png' width='13px' height='16px'> на <b>{0:N0}</b>&nbsp;чел. 
в сравнении с <b>{1:dd.MM}</b>.<br/>", Math.Abs(almostRedundantsRate), lastDateTime) : string.Empty;

                string str8 = !IsYearJoint() ? string.Format(@"&nbsp;&nbsp;&nbsp;{2} численность уволенных работников по сокращению 
достигла <b>{0:N0}</b>&nbsp;чел., из них за последнюю неделю – <b>{1:N0}</b>&nbsp;чел.<br/>", redundantsTotal, redundantsTotalRate, monitoringStartStr2) :
                string.Format(@"&nbsp;{1} численность уволенных работников по сокращению 
достигла <b>{0:N0}</b>&nbsp;чел.<br/>", redundantsTotal, monitoringStartStr2);

                string str9 = string.Format(@"&nbsp;&nbsp;&nbsp;<b>{0:N0}</b> организации заявили о переводе части работников на режим неполного рабочего времени, 
предоставлении вынужденных отпусков, а также простое. Суммарная численность работников, находившихся в простое по вине администрации, 
работавших неполное рабочее время, а также работников, которым были предоставлены отпуска по инициативе администрации, составила <b>{1:N0}</b>&nbsp;чел.<br/>",
                    factoryCount, incompleteEmployersCount);

                string str10;
                if (totalLastWeekDebts == 0 && totalDebts == 0)
                {
                    str10 = string.Format(@"&nbsp;&nbsp;&nbsp;В <b>{1}</b> на <b>{0}</b> отсутствует задолженность по выплате 
заработной платы.<br/>", dateTimeDebtsStr, RegionsNamingHelper.ShortName(ComboRegion.SelectedValue));
                }
                else if (totalDebts == 0)
                {
                    str10 = string.Format(@"&nbsp;&nbsp;&nbsp;В <b>{1}</b> на <b>{0}</b> отсутствует задолженность по выплате заработной платы. 
Задолженность в сумме <b>{2:N3}</b>&nbsp;млн.руб. была погашена за период с <b>{3}</b> по <b>{0}</b>.<br/>",
dateTimeDebtsStr, RegionsNamingHelper.ShortName(ComboRegion.SelectedValue), totalLastWeekDebts, dateLastTimeDebtsStr);
                }
                else
                {
                    str10 = string.Format(@"&nbsp;&nbsp;&nbsp;В <b>{4}</b> на <b>{0}</b> задолженность по выплате заработной платы составляет 
<b>{1:N3}</b>&nbsp;млн.рублей (<b>{2:N0}</b>&nbsp;чел.). За период с <b>{5}</b> по <b>{0}</b> задолженность {3}.<br/>",
dateTimeDebtsStr, totalDebts, slavesCount, debtsPercentArrow, RegionsNamingHelper.ShortName(ComboRegion.SelectedValue), dateLastTimeDebtsStr);
                }

                string str11 = string.Format("&nbsp;&nbsp;&nbsp;Ежемесячный прогнозируемый темп {1} численности безработных в <b>{2}</b>&nbsp;<b>{0:P2}</b>.<br/>", Math.Abs(forecastRateGrow), forecastRageGrowArrow, RegionsNamingHelper.ShortName(ComboRegion.SelectedValue));

                string str12 = string.Format("&nbsp;&nbsp;&nbsp;Прогнозируемое значение численности безработных на <b>{0}&nbsp;{1}</b> года <b>{2:N0}</b> чел.", CRHelper.RusMonth(nextDateTime.Month), nextDateTime.Year, forecastValue);

                CommentText1.Text = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}", str1, str2, str3, str4, str5, str6, str7, str8, str9, str10);
                CommentText2.Text = string.Format("{0}{1}", str11, str12);
            }
        }

        private int GetKoeffNumber(string subjectName)
        {
            if (dtKoeff == null)
            {
                dtKoeff = GetSubjectKoeffTable();
            }

            if (dtKoeff.Rows.Count != 0)
            {
                for (int i = 0; i < dtKoeff.Rows.Count; i++)
                {
                    DataRow row = dtKoeff.Rows[i];
                    if (row[0] != DBNull.Value && row[0].ToString() == subjectName)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private static DataTable GetSubjectKoeffTable()
        {
            DataTable dt = new DataTable();

            DataColumn column = new DataColumn("Субъект", typeof(string));
            dt.Columns.Add(column);

            for (int i = 0; i <= 13; i++)
            {
                string columnName = (i == 0) ? "Y-пересечение" : "Переменная X ";
                column = new DataColumn(columnName + i, typeof(string));
                dt.Columns.Add(column);
            }

            DataRow row = dt.NewRow();
            object[] array0 = { "Уральский федеральный округ", 4.834817, 0.011686, 0.056984, 0.087004, 0.093705, 0.084681, 0.049598, 0.013778, -0.00585,
                -0.02109, -0.04614, -0.05875, -0.04432};
            row.ItemArray = array0;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array1 = { "Челябинская область", 4.275802, 0.013926, 0.040292, 0.077282, 0.073687, 0.053920, 0.0079087, -0.010274,
                -0.025252, -0.045110, -0.077741, -0.097024, -0.067357};
            row.ItemArray = array1;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array2 =  { "Курганская область", 4.030716, 0.004315, 0.050866, 0.080746, 0.082402, 0.049926, 0.008242, -0.023601, -0.031912, -0.044864,
                -0.074787, -0.079989, -0.050917};
            row.ItemArray = array2;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array3 =  { "Свердловская область", 4.301695, 0.016367, 0.073546, 0.089745, 0.080245, 0.066612, 0.038165, 0.027910, 0.016237, 0.000114, -0.027503,
                -0.043127, -0.037974};
            row.ItemArray = array3;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array4 =  { "Тюменская область", 4.027350, -0.007477, 0.013083, 0.032511, 0.039706, 0.049971, 0.042602, 0.002576, -0.010114, -0.018256, -0.047603,
                -0.043513, -0.034087};
            row.ItemArray = array4;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array5 =  { "Ханты-Мансийский автономный округ", 4.021664, 0.004053, 0.055707, 0.080262, 0.077477, 0.069644, 0.033193, 0.003313, -0.024575,
                -0.040061, -0.047314, -0.049641, -0.038991};
            row.ItemArray = array5;
            dt.Rows.Add(row);

            row = dt.NewRow();
            object[] array6 =  { "Ямало-Ненецкий автономный округ", 3.676308, 0.001662, 0.045314, 0.075064, 0.069432, 0.048719, 0.009784,
                -0.069183, -0.130112, -0.124571, -0.106291, -0.079426, -0.051240};
            row.ItemArray = array6;
            dt.Rows.Add(row);

            return dt;
        }


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

        private static string GetStringDTValue(DataTable dt, string columnName)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != string.Empty)
            {
                return dt.Rows[0][columnName].ToString();
            }
            return string.Empty;
        }

        #endregion

        #region Обработчики диаграммы

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0003_chart1");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString());
                        row[0] = string.Format("{0:dd.MM.yy}", dateTime);
                    }
                }

                if (ComboRegion.SelectedIndex != 0)
                {
                    foreach (DataColumn column in dtChart.Columns)
                    {
                        column.ColumnName = column.ColumnName.Replace("Уровень безработицы", string.Format("Уровень безработицы в {0}",
                                                                                           RegionsNamingHelper.ShortName(ComboRegion.SelectedValue)));
                    }
                }
                else
                {
                    dtChart.Columns.RemoveAt(2);
                }
                dtChart.AcceptChanges();

                UltraChart1.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart1.Series.Add(series);
                }

                //UltraChart.DataSource = dtChart;
            }
        }

        protected void UltraChart2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0003_chart2");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString());
                        row[0] = string.Format("{0:dd.MM.yy}", dateTime);
                    }
                }

                UltraChart2.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart2.Series.Add(series);
                }

                //UltraChart.DataSource = dtChart;
            }
        }

        protected void UltraChart3_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0003_chart3");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString());
                        row[0] = string.Format("{0:dd.MM.yy}", dateTime);
                    }
                }

                UltraChart3.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart3.Series.Add(series);
                }

                //UltraChart.DataSource = dtChart;
            }
        }

        protected void UltraChart4_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0003_chart4");
            dtChart = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                dtChart.Columns.RemoveAt(0);

                foreach (DataRow row in dtChart.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        DateTime dateTime = CRHelper.PeriodDayFoDate(row[0].ToString());
                        row[0] = string.Format("{0:dd.MM.yy}", dateTime);
                    }
                }

                UltraChart4.Series.Clear();
                for (int i = 1; i < dtChart.Columns.Count; i++)
                {
                    NumericSeries series = CRHelper.GetNumericSeries(i, dtChart);
                    series.Label = dtChart.Columns[i].ColumnName;
                    UltraChart4.Series.Add(series);
                }

                //UltraChart.DataSource = dtChart;
            }
        }

        void UltraChart1_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                
                if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline)primitive;
                    foreach (DataPoint point in polyline.points)
                    {
                        if (point.Series != null)
                        {
                            string unit = "%";
                            point.DataPoint.Label = string.Format("{2} на {3}\n {0:N2}{1}", ((NumericDataPoint)point.DataPoint).Value, unit, point.Series.Label, point.DataPoint.Label);

                        }
                    }
                    if (polyline.Series.Label == "Целевое значение по уровню зарегистрированных безработных ")
                    {
                        e.SceneGraph[i + 1].Visible = false;
                    }
                }
            }
        }

        void UltraChart2_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline)primitive;
                    foreach (DataPoint point in polyline.points)
                    {
                        if (point.Series != null)
                        {
                            string unit = " чел/на 1 вакансию";
                            point.DataPoint.Label = string.Format("{2} на {3}\n {0:N2}{1}", ((NumericDataPoint)point.DataPoint).Value, unit, point.Series.Label, point.DataPoint.Label);

                        }
                    }
                }
            }
        }

        void UltraChart3_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline)primitive;
                    foreach (DataPoint point in polyline.points)
                    {
                        if (point.Series != null)
                        {
                            string unit = " чел.";
                            point.DataPoint.Label = string.Format("{2} на {3}\n {0:N2}{1}", ((NumericDataPoint)point.DataPoint).Value, unit, point.Series.Label, point.DataPoint.Label);

                        }
                    }
                }
            }
        }

        void UltraChart4_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];

                if (primitive is Polyline)
                {
                    Polyline polyline = (Polyline)primitive;
                    foreach (DataPoint point in polyline.points)
                    {
                        if (point.Series != null)
                        {
                            string unit = " млн.руб.";
                            point.DataPoint.Label = string.Format("{2} на {3}\n {0:N2}{1}", ((NumericDataPoint)point.DataPoint).Value, unit, point.Series.Label, point.DataPoint.Label);

                        }
                    }
                }
            }
        }

        #endregion

        #region Экспорт в Excel

        private static string CommentTextExportsReplaces(string source)
        {
            string commentText = source;

            commentText = commentText.Replace("<\n>", "");
            commentText = commentText.Replace("<\r>", "");
            commentText = commentText.Replace(@"
", " ");
            commentText = commentText.Replace("&nbsp;", " ");
            commentText = commentText.Replace("<br/>", "\n");
            commentText = commentText.Replace("<b>", "");
            commentText = commentText.Replace("</b>", "");
            commentText = commentText.Replace("<img src=\"../../images/arrowRedDownBB.png\" width=\"13px\" height=\"16px\">", "");
            commentText = commentText.Replace("<img src=\"../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">", "");
            commentText = commentText.Replace("<img src=\"../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">", "");
            commentText = commentText.Replace("<img src=\"../../images/arrowGreenUpBB.png\" width=\"13px\" height=\"16px\">", "");
            commentText = commentText.Replace("<img src='../../images/arrowRedDownBB.png' width='13px' height='16px'>", "");
            commentText = commentText.Replace("<img src='../../images/arrowRedUpBB.png' width='13px' height='16px'>", "");
            commentText = commentText.Replace("<img src='../../images/arrowGreenDownBB.png' width='13px' height='16px'>", "");
            commentText = commentText.Replace("<img src='../../images/arrowGreenUpBB.png' width='13px' height='16px'>", "");

            return commentText;
        }

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = PageSubTitle.Text;

            e.CurrentWorksheet.Rows[2].Cells[0].Value = CommentTextExportsReplaces(CommentText1.Text);
            e.CurrentWorksheet.Rows[3].Cells[0].Value = CommentTextExportsReplaces(CommentText2.Text);
        }

        private void ExcelExporter_EndExport(object sender, EndExportEventArgs e)
        {
            int columnCount = UltraWebGrid1.Columns.Count;
            int rowsCount = UltraWebGrid1.Rows.Count;

            e.CurrentWorksheet.Columns[0].Width = 20 * 37;
            e.CurrentWorksheet.Columns[1].Width = 200 * 37;

            for (int i = 2; i < columnCount; i++)
            {
                e.CurrentWorksheet.Columns[i].Width = 60 * 37;
            }

            // расставляем стили у начальных колонок
            for (int i = 4; i < rowsCount + 4; i++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            }

            // расставляем стили у ячеек хидера
            for (int i = 1; i < columnCount; i++)
            {
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                e.CurrentWorksheet.Rows[4].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("Sheet2");

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid1, sheet1);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid2, sheet2);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid1.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF

        private static void SetExportGridParams(UltraWebGrid grid)
        {
            grid.DisplayLayout.RowSelectorsDefault = RowSelectors.Yes;

            bool unit1 = false;
            bool unit4 = false;
            foreach (UltraGridRow row in grid.Rows)
            {
                // номер показателя
                int k = (row.Index % 3);
                UltraGridCell nameCell = row.Cells[1];
                UltraGridCell numberCell = row.Cells[0];
                switch (k)
                {
                    case 0:
                        {
                            if (!unit4 && numberCell.Value != null && numberCell.Value.ToString() == "4")
                            {
                                unit4 = true;
                            }
                            else if (!unit1 && numberCell.Value != null && numberCell.Value.ToString() == "1")
                            {
                                unit1 = true;
                            }
                            else
                            {
                                numberCell.Value = "";
                            }
                            numberCell.Style.BorderDetails.WidthBottom = 0;
                            nameCell.Style.BorderDetails.WidthBottom = 0;
                            break;
                        }
                    case 1:
                        {
                            if (numberCell.Value != null && numberCell.Value.ToString() == "1" ||
                                numberCell.Value != null && numberCell.Value.ToString() == "4")
                            {
                                numberCell.Value = "";
                            }
                            nameCell.Value = "темп прироста";
                            nameCell.Style.HorizontalAlign = HorizontalAlign.Right;
                            numberCell.Style.BorderDetails.WidthBottom = 0;
                            numberCell.Style.BorderDetails.WidthTop = 0;
                            nameCell.Style.BorderDetails.WidthBottom = 0;
                            nameCell.Style.BorderDetails.WidthTop = 0;
                            break;
                        }
                    case 2:
                        {
                            if (numberCell.Value != null && row.NextRow != null && row.NextRow.Cells[0].Value != null &&
                                ((numberCell.Value.ToString() == "4" && row.NextRow.Cells[0].Value.ToString() == "4") ||
                                 (numberCell.Value.ToString() == "1" && row.NextRow.Cells[0].Value.ToString() == "1")))
                            {
                                numberCell.Style.BorderDetails.WidthBottom = 0;
                            }
                            numberCell.Value = "";
                            if (nameCell.Value != null && nameCell.Value.ToString() != "ранг по УрФО")
                            {
                                nameCell.Value = "прирост";
                            }
                            nameCell.Style.HorizontalAlign = HorizontalAlign.Right;
                            numberCell.Style.BorderDetails.WidthTop = 0;
                            nameCell.Style.BorderDetails.WidthTop = 0;
                            break;
                        }
                }
            }
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";

            if (ComboRegion.SelectedIndex != 0)
            {
                SetExportGridParams(UltraWebGrid1);
                SetExportGridParams(UltraWebGrid2);
                UltraGridExporter1.PdfExporter.Export(UltraWebGrid1);
            }
            else
            {
                UltraGridExporter1.PdfExporter.Export(new UltraWebGrid());
            }
        }

        private bool titleAdded = false;
        private bool grid2Added = false;

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            if (e.Layout.Bands.Count != 0)
            {
                for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].Width = (i == 1)
                                                             ? CRHelper.GetColumnWidth(300)
                                                             : CRHelper.GetColumnWidth(60);
                }
            }

            if (!titleAdded && !grid2Added)
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

                title = e.Section.AddText();
                font = new Font("Verdana", 12);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent("\n" + CommentTextExportsReplaces(CommentText1.Text) + "\n" + CommentTextExportsReplaces(CommentText2.Text));
            }
            titleAdded = true;
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            if (titleAdded && !grid2Added)
            {
                grid2Added = true;
                UltraGridExporter1.PdfExporter.Export(UltraWebGrid2, e.Section);

                e.Section.AddPageBreak();
                IText title = e.Section.AddText();
                Font font = new Font("Verdana", 12);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent("\n" + chart1ElementCaption.Text);

                UltraChart1.Legend.Margins.Right = 0;
                UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.82));
                MemoryStream imageStream = new MemoryStream();
                UltraChart1.SaveTo(imageStream, ImageFormat.Png);
                Infragistics.Documents.Reports.Graphics.Image img1 = new Infragistics.Documents.Reports.Graphics.Image(imageStream);
                e.Section.AddImage(img1);

                title = e.Section.AddText();
                font = new Font("Verdana", 12);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent("\n" + chart2ElementCaption.Text);

                UltraChart2.Legend.Margins.Right = 0;
                UltraChart2.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.82));
                Infragistics.Documents.Reports.Graphics.Image img2 = UltraGridExporter.GetImageFromChart(UltraChart2);
                e.Section.AddImage(img2);

                title = e.Section.AddText();
                font = new Font("Verdana", 12);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent("\n" + chart3ElementCaption.Text);

                UltraChart3.Legend.Margins.Right = 0;
                UltraChart3.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.82));
                Infragistics.Documents.Reports.Graphics.Image img3 = UltraGridExporter.GetImageFromChart(UltraChart3);
                e.Section.AddImage(img3);

                title = e.Section.AddText();
                font = new Font("Verdana", 12);
                title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
                title.AddContent("\n" + chart4ElementCaption.Text);

                UltraChart4.Legend.Margins.Right = 0;
                UltraChart4.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.82));
                Infragistics.Documents.Reports.Graphics.Image img4 = UltraGridExporter.GetImageFromChart(UltraChart4);
                e.Section.AddImage(img4);
            }
        }

        #endregion
    }
}
