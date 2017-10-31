using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;

namespace Krista.FM.Server.Dashboards.reports.FK_0004_0005
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable grbsGridDt = new DataTable();
        private DataTable restChartDt = new DataTable();
        private DataTable restChartDtNormalized = new DataTable();
        private DataTable dynamicChartDt = new DataTable();
        private DataTable dynamicChartDtNormalized = new DataTable();
        private DataTable dtChartLimit = new DataTable();

        private DateTime currentDate;

        private double chartLimit;

        #endregion

        #region Параметры запроса

        // выбранный период
        private CustomParam selectedPeriod;
        // выбранный показатель таблицы
        private CustomParam selectedGridIndicator;

        #endregion

        private bool IsMlnRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private bool daily
        {
            get { return DailyMonthlyButtonList.SelectedIndex == 0; }
        }

        private string multiplierCaption;

        // выбранный множитель рублей
        private CustomParam rubMultiplier;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            multiplierCaption = IsMlnRubSelected ? "млн.руб." : "млрд.руб.";

            #region Инициализация параметров запроса

            selectedPeriod = UserParams.CustomParam("selected_period");
            selectedGridIndicator = UserParams.CustomParam("selected_grid_indicator");

            #endregion
            rubMultiplier = UserParams.CustomParam("rub_multiplier");

            rubMultiplier.Value = IsMlnRubSelected ? "1000000" : "1000000000";

            #region Настройка диаграммы динамики

            DynamicChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 45);
            DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5);
            

            //  DynamicChartBrick.YAxisLabelFormat = "N2";
            DynamicChartBrick.DataFormatString = "N0";
            DynamicChartBrick.DataItemCaption = "млрд.руб.";
            DynamicChartBrick.Legend.Visible = true;
            DynamicChartBrick.Legend.Location = LegendLocation.Bottom;
            DynamicChartBrick.Legend.SpanPercentage = 10;
            DynamicChartBrick.Legend.Font = new Font("Verdana", 8);
            DynamicChartBrick.PeriodMonthSpan = 1;
            DynamicChartBrick.ColorModel = ChartColorModel.PureRandom;
            DynamicChartBrick.XAxisExtent = 80;
            DynamicChartBrick.YAxisExtent = 60;
            DynamicChartBrick.ZeroAligned = true;
            DynamicChartBrick.SwapRowAndColumns = true;
            DynamicChartBrick.TooltipFormatString = String.Format("<ITEM_LABEL>\nфакт на <b><SERIES_LABEL></b> г.\n<b><DATA_VALUE:N2></b> {0}", multiplierCaption);
            DynamicChartBrick.Daily = daily;

            if (CheckBox1.Checked)
            {
                ChartTextAppearance appearance = new ChartTextAppearance();
                appearance.Column = -2;
                appearance.Row = -2;
                appearance.VerticalAlign = StringAlignment.Far;
                appearance.ItemFormatString = "<DATA_VALUE:N2>";
                appearance.ChartTextFont = new Font("Verdana", 8);
                appearance.Visible = true;
                DynamicChartBrick.Chart.SplineChart.ChartText.Add(appearance);
                DynamicChartBrick.Chart.Axis.X.Margin.Near.MarginType = LocationType.Pixels;
                DynamicChartBrick.Chart.Axis.X.Margin.Near.Value = 30;
            }

            //   DynamicChartBrick.IconSize = SymbolIconSize.Medium;

            #endregion

            #region Настройка диаграммы остатков

            ChartControl.Width = CRHelper.GetChartWidth(Convert.ToInt32(CustomReportConst.minScreenWidth - 45));
            ChartControl.Height = CRHelper.GetChartWidth(Convert.ToInt32(CustomReportConst.minScreenHeight * 0.57));
            ChartControl.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(ChartControl_FillSceneGraph);
            ChartControl.Border.Color = Color.White;

            ChartControl.ChartType = ChartType.StackColumnChart;

            ChartControl.Legend.Visible = true;
            ChartControl.Legend.Location = LegendLocation.Bottom;
            ChartControl.Legend.SpanPercentage = 17;
            ChartControl.Legend.Font = new Font("Verdana", 8);

            ChartControl.Axis.Y.Extent = 60;
            ChartControl.Axis.X.Extent = 100;
            ChartControl.Data.SwapRowsAndColumns = true;
            ChartControl.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

            ChartControl.Axis.Y.Labels.Visible = true;
            ChartControl.Axis.Y.Labels.Font = new Font("Verdana", 8);
            ChartControl.Axis.Y.Labels.Orientation = TextOrientation.Horizontal;
            ChartControl.Axis.Y.Labels.ItemFormatString = normalize.Checked ? "<DATA_VALUE:N0>%" : "<DATA_VALUE:N0>";
            if (normalize.Checked)
            {
                ChartControl.Axis.Y.TickmarkStyle = AxisTickStyle.Percentage;
                ChartControl.Axis.Y.TickmarkPercentage = 10;
            }

            ChartControl.Axis.X.Labels.Orientation = TextOrientation.Horizontal;

            ChartControl.Axis.X.Labels.SeriesLabels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;
            ChartControl.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;

            ChartControl.TitleLeft.Visible = !normalize.Checked;
            ChartControl.TitleLeft.HorizontalAlign = StringAlignment.Center;
            ChartControl.TitleLeft.VerticalAlign = StringAlignment.Center;
            ChartControl.TitleLeft.Text = multiplierCaption;
            ChartControl.TitleLeft.Extent = 20;
            ChartControl.TitleLeft.Font = new Font("Verdana", 10);
            ChartControl.Tooltips.FormatString = String.Format("<ITEM_LABEL>\nфакт на <b><SERIES_LABEL></b> г.\n<b><DATA_VALUE:N2></b> {0}", multiplierCaption);

            ChartControl.ColorModel.ModelStyle = ColorModels.CustomLinear;
            ChartControl.ColorModel.Skin.ApplyRowWise = false;
            ChartControl.ColorModel.Skin.PEs.Clear();
            for (int i = 17; i <= 24; i++)
            {
                PaintElement pe = new PaintElement();
                Color color = GetColor(i);
                Color stopColor = GetColor(i);

                pe.Fill = color;
                pe.FillStopColor = stopColor;
                pe.ElementType = PaintElementType.Gradient;
                pe.FillGradientStyle = GradientStyle.ForwardDiagonal;
                pe.FillOpacity = 150;
                ChartControl.ColorModel.Skin.PEs.Add(pe);
            }

            #endregion

            //   ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            //   ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        private static Color GetColor(int i)
        {
            switch (i)
            {
                case 1:
                    {
                        return Color.LimeGreen;
                    }
                case 2:
                    {
                        return Color.LightSkyBlue;
                    }
                case 3:
                    {
                        return Color.Gold;
                    }
                case 4:
                    {
                        return Color.Peru;
                    }
                case 5:
                    {
                        return Color.DarkOrange;
                    }
                case 6:
                    {
                        return Color.PeachPuff;
                    }
                case 7:
                    {
                        return Color.MediumSlateBlue;
                    }
                case 8:
                    {
                        return Color.ForestGreen;
                    }
                case 9:
                    {
                        return Color.HotPink;
                    }
                case 10:
                    {
                        return Color.SandyBrown;
                    }
                case 11:
                    {
                        return Color.Olive;
                    }
                case 12:
                    {
                        return Color.SeaGreen;
                    }
                case 13:
                    {
                        return Color.PaleTurquoise;
                    }
                case 14:
                    {
                        return Color.DarkTurquoise;
                    }
                case 15:
                    {
                        return Color.LawnGreen;
                    }
                case 16:
                    {
                        return Color.Moccasin;
                    }
                case 17:
                    {
                        return Color.RoyalBlue;
                    }
                case 18:
                    {
                        return Color.LightSalmon;
                    }
                case 19:
                    {
                        return Color.Aquamarine;
                    }
                case 21:
                    {
                        return Color.Crimson;
                    }
                case 22:
                    {
                        return Color.Thistle;
                    }
                case 23:
                    {
                        return Color.RoyalBlue;
                    }
                default:
                    {
                        return Color.PowderBlue;
                    }
            }
        }

        private Dictionary<string, string> shortToFullNames = new Dictionary<string, string>();
        private Dictionary<string, string> fullToShortNames = new Dictionary<string, string>();

        private string GetUniqueKey(Dictionary<string, string> dictionary, string key)
        {
            while (dictionary.ContainsKey(key))
            {
                key += " ";
            }
            return key;
        }

        private string GetUniqueValue(Dictionary<string, string> dictionary, string key)
        {
            while (dictionary.ContainsValue(key))
            {
                key += " ";
            }
            return key;
        }

        private void InitFilterDictionary()
        {
            string query = DataProvider.GetQueryText("FK_0004_0005_names");
            DataTable dtNames = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtNames);

            foreach (DataRow row in dtNames.Rows)
            {
                shortToFullNames.Add(GetUniqueKey(shortToFullNames, row[1].ToString()), row[0].ToString());
                fullToShortNames.Add(row[0].ToString(), GetUniqueValue(fullToShortNames, row[1].ToString()));
            }
        }

        Dictionary<string, int> filter = new Dictionary<string, int>();

        private Dictionary<string, int> GetFilterDictionary()
        {
            Dictionary<string, int> filter = new Dictionary<string, int>();
            filter.Add(fullToShortNames["1000_Остатки средств федерального бюджета на ЕКС"], 0);
            filter.Add(fullToShortNames["2000_Средства ФБ, размещенные на депозиты"], 0);
            filter.Add(fullToShortNames["3000_Резервный Фонд (совокупный объем), в рублях"], 0);
            filter.Add(fullToShortNames["3001_Остаток на счете в рублях в Банке России"], 1);
            filter.Add(fullToShortNames["3002_Остаток на счетах в иностранных валютах в Банке России"], 1);
            filter.Add(fullToShortNames["3003_Иные финансовые активы (долговые обязательства МВФ)"], 1);
            filter.Add(fullToShortNames["3004_направлено на финансирование дефицита федерального бюджета"], 1);
            filter.Add(fullToShortNames["3005_курсовая разница"], 1);
            filter.Add(fullToShortNames["3006_расчетная сумма процентного дохода"], 1);
            filter.Add(fullToShortNames["3007_уплаченная сумма процентного дохода"], 1);
            filter.Add(fullToShortNames["4000_в долларовом эквиваленте"], 0);
            filter.Add(fullToShortNames["5000_ФНБ (совокупный объем), в рублях"], 0);
            filter.Add(fullToShortNames["5001_Остаток на счете в рублях в Банке России"], 1);
            filter.Add(fullToShortNames["5002_Остаток на счетах в иностранных валютах в Банке России"], 1);
            filter.Add(fullToShortNames["5003_Иные финансовые активы (долговые обязательства ВЭБа)"], 1);
            filter.Add(fullToShortNames["5004_направлено на софинансирование добровольных пенсионных накоплений граждан"], 1);
            filter.Add(fullToShortNames["5005_курсовая разница"], 1);
            filter.Add(fullToShortNames["5006_расчетная сумма процентного дохода"], 1);
            filter.Add(fullToShortNames["5007_уплаченная сумма процентного дохода"], 1);
            filter.Add(fullToShortNames["6000_в долларовом эквиваленте"], 0);
            filter.Add(fullToShortNames["7000_Остатки на других счетах Казначейства России"], 0);
            filter.Add(fullToShortNames["7001_Средства для выплаты наличных денег ПБС (40116)"], 1);
            filter.Add(fullToShortNames["7002_Средства бюджетов субъектов Российской Федерации (40201)"], 1);
            filter.Add(fullToShortNames["7003_Средства местных бюджетов  (40204)"], 1);
            filter.Add(fullToShortNames["7004_Средства, поступающие во времен. распоряжение бюдж. учреждений (40302)"], 1);
            filter.Add(fullToShortNames["7005_Счета организаций, находящихся в фед. собственности. финансовые орг-ции (40501)"], 1);
            filter.Add(fullToShortNames["7006_Счета организаций, находящихся в фед. собственности. некоммер. орг-ции (40503)"], 1);
            filter.Add(fullToShortNames["7007_Счета организаций, находящихся в госуд. (кроме фед.) собственности. некоммер. орг-ции (40603)"], 1);
            filter.Add(fullToShortNames["7008_Счета негосуд. организаций. некоммер. организации (40703)"], 1);

            return filter;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            InitFilterDictionary();

            if (daily)
            {
                UserParams.Filter.Value = "День";
            }
            else
            {
                UserParams.Filter.Value = "Месяц";
            }

            if (!Page.IsPostBack)
            {
                //chartWebAsyncPanel.AddRefreshTarget(ChartControl);
                //chartWebAsyncPanel.AddLinkedRequestTrigger(daily.ClientID);
                CustomCalendar1.WebCalendar.SelectedDate = CubeInfo.GetLastDate(DataProvidersFactory.SecondaryMASDataProvider, "FK_0004_0005_lastDate");

                WebAsyncRefreshPanel1.AddRefreshTarget(ChartControl);
                WebAsyncRefreshPanel1.AddLinkedRequestTrigger(normalize.ClientID);

                ComboIncomes.Width = 420;
                ComboIncomes.Title = "Показатель";
                ComboIncomes.ParentSelect = false;
                ComboIncomes.MultiSelect = true;
                ComboIncomes.MultipleSelectionType = MultipleSelectionType.SimpleMultiple;
                ComboIncomes.FillDictionaryValues(GetFilterDictionary());
                ComboIncomes.SetСheckedState(fullToShortNames["7002_Средства бюджетов субъектов Российской Федерации (40201)"], true);
                ComboIncomes.SetСheckedState(fullToShortNames["7003_Средства местных бюджетов  (40204)"], true);
            }

            currentDate = CustomCalendar1.WebCalendar.SelectedDate;

            Page.Title = String.Format("Динамика остатков средств бюджета на счетах Казначейства России по выбранным показателям ");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("по состоянию на <b>{0:dd.MM.yyyy} г., {1}</b>", currentDate.AddDays(1), multiplierCaption);

            DynamicChartCaption.Text = String.Format("Структура остатков средств на счетах Резервного фонда и Фонда национального благосостояния Российской Федерации, {1}", currentDate.AddDays(1), multiplierCaption);

            selectedPeriod.Value = CRHelper.PeriodMemberUName("[Период].[Период]", currentDate, 5);
            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            switch (ComboIncomes.SelectedValues.Count)
            {
                case 1:
                case 2:
                    {
                        DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5);
                        DynamicChartBrick.Legend.SpanPercentage = 10;
                        break;
                    }
                case 3:
                case 4:
                    {
                        DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 + 25);
                        DynamicChartBrick.Legend.SpanPercentage = 15;
                        break;
                    }
                case 5:
                case 6:
                    {
                        DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 + 50);
                        DynamicChartBrick.Legend.SpanPercentage = 19;
                        break;
                    }
                case 7:
                case 8:
                    {
                        DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 + 75);
                        DynamicChartBrick.Legend.SpanPercentage = 24;
                        break;
                    }
                case 9:
                case 10:
                    {
                        DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 + 100);
                        DynamicChartBrick.Legend.SpanPercentage = 27;
                        break;
                    }
                case 11:
                case 12:
                    {
                        DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 + 125);
                        DynamicChartBrick.Legend.SpanPercentage = 31;
                        break;
                    }
                case 13:
                case 14:
                    {
                        DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 + 145);
                        DynamicChartBrick.Legend.SpanPercentage = 33;
                        break;
                    }
                case 15:
                case 16:
                    {
                        DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 + 160);
                        DynamicChartBrick.Legend.SpanPercentage = 35;
                        break;
                    }
                case 17:
                case 18:
                    {
                        DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 + 180);
                        DynamicChartBrick.Legend.SpanPercentage = 37;
                        break;
                    }
                case 19:
                case 20:
                    {
                        DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 + 200);
                        DynamicChartBrick.Legend.SpanPercentage = 39;
                        break;
                    }
                case 21:
                case 22:
                    {
                        DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 + 220);
                        DynamicChartBrick.Legend.SpanPercentage = 41;
                        break;
                    }
                case 23:
                case 24:
                    {
                        DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 + 240);
                        DynamicChartBrick.Legend.SpanPercentage = 43;
                        break;
                    }
                case 25:
                case 26:
                    {
                        DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 + 260);
                        DynamicChartBrick.Legend.SpanPercentage = 45;
                        break;
                    }
                case 27:
                case 28:
                    {
                        DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 + 280);
                        DynamicChartBrick.Legend.SpanPercentage = 47;
                        break;
                    }
                case 29:
                case 30:
                    {
                        DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 + 300);
                        DynamicChartBrick.Legend.SpanPercentage = 49;
                        break;
                    }
            }
            selectedGridIndicator.Value = String.Empty;
            foreach (string kd in ComboIncomes.SelectedValues)
            {
                selectedGridIndicator.Value += String.Format("[Показатели].[Остатки средств].[Все показатели].[{0}],", shortToFullNames[kd]);
            }
            selectedGridIndicator.Value = selectedGridIndicator.Value.Trim(',');

            DynamicChartDataBind();
            RestsChartDataBind();
        }

        #region Обработчики диаграммы остатков

        private void RestsChartDataBind()
        {
            string query = DataProvider.GetQueryText("FK_0004_0001_restsChart");
            restChartDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", restChartDt);
            restChartDt.Columns.RemoveAt(0);
            ReplaceDateFormat(restChartDt, 0);
            ChartControl.Series.Clear();

            restChartDtNormalized = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(DataProvider.GetQueryText("FK_0004_0001_restsChart_normalized"), "Наименование показателей", restChartDtNormalized);
            restChartDtNormalized.Columns.RemoveAt(0);
            ReplaceDateFormat(restChartDtNormalized, 0);
            ChartControl.Series.Clear();

            if (normalize.Checked)
            {
                ChartControl.StackChart.StackStyle = StackStyle.Complete;
                for (int i = 2; i < restChartDt.Columns.Count; i++)
                {
                    ChartControl.Series.Add(CRHelper.GetNumericSeries(i, restChartDtNormalized));
                }
            }
            else
            {
                ChartControl.StackChart.StackStyle = StackStyle.Normal;
                for (int i = 2; i < restChartDt.Columns.Count; i++)
                {
                    ChartControl.Series.Add(CRHelper.GetNumericSeries(i, restChartDt));
                }
            }

            ChartControl.DataBind();
        }

        private void ReplaceDateFormat(DataTable dt, int dateColumnIndex)
        {
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = dt.Rows[i];
                if (row[dateColumnIndex] != DBNull.Value)
                {
                    DateTime dataTime = CRHelper.PeriodDayFoDate(row[dateColumnIndex].ToString());
                    if (!daily &&
                        dataTime.Day != 1)
                    {
                        dt.Rows.RemoveAt(i);
                    }
                    else
                    {
                        dataTime = dataTime.Add(new TimeSpan(0));
                        dataTime = daily ? dataTime : dataTime.AddMonths(1);
                        row[dateColumnIndex] = daily ? dataTime.ToString("dd.MM.yyyy") : "на " + dataTime.ToString("dd.MM.yyyy");
                    }                    
                }
            }
        }

        protected void ChartControl_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Grid.X"))
                {
                    Text text = (Text)primitive;
                    if (text.Row == -1)
                    {
                        //text.bounds.Height = 40;
                        //text.bounds = new Rectangle(text.bounds.Left, text.bounds.Y, text.bounds.Width, text.bounds.Height);

                        //text.labelStyle.HorizontalAlign = StringAlignment.Center;

                        //text.labelStyle.VerticalAlign = StringAlignment.Far;
                        //text.labelStyle.FontSizeBestFit = false;
                        text.labelStyle.Font = new Font("Verdana", 8);
                    }
                }
                if (primitive is Text && primitive.Path != null && primitive.Path.Contains("Border.Title.Left"))
                {
                    Text text = (Text)primitive;
                    text.bounds.Width = 20;
                }
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null && box.Value != null)
                    {
                        box.DataPoint.Label = normalize.Checked ?
                             String.Format("{0}<br/><b>{1:P2}</b>", box.DataPoint.Label, restChartDtNormalized.Rows[box.Row][box.Column + 2]) :
                             String.Format("{0}", box.DataPoint.Label);
                    }
                }
            }
        }



        #endregion

        #region Обработчики диаграммы динамики

        private void DynamicChartDataBind()
        {
            string query = DataProvider.GetQueryText("FK_0004_0001_dynamicChart");
            dynamicChartDt = new DataTable();

            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dynamicChartDt);

            foreach (DataColumn col in dynamicChartDt.Columns)
            {
                if (fullToShortNames.ContainsKey(col.ColumnName))
                {
                    col.ColumnName = fullToShortNames[col.ColumnName];
                }
            }

            dynamicChartDt.AcceptChanges();

            if (dynamicChartDt.Rows.Count > 0)
            {
                DynamicChartBrick.DataTable = dynamicChartDt;
                DynamicChartBrick.DataBind();
            }
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            //  ReportExcelExporter1.Export(GRBSGridBrick.GridHeaderLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");
            ChartControl.Width = Convert.ToInt32(ChartControl.Width.Value * 0.8);
            ReportExcelExporter1.Export(ChartControl, DynamicChartCaption.Text, sheet2, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.PageSubTitle = Label2.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            //   ReportPDFExporter1.Export(GRBSGridBrick.GridHeaderLayout, section1);

            ISection section2 = report.AddSection();
            ChartControl.Width = Convert.ToInt32(ChartControl.Width.Value * 0.8);
            ReportPDFExporter1.Export(ChartControl, DynamicChartCaption.Text, section2);
        }

        #endregion
    }
}