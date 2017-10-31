using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGauge;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using FontUnit=System.Web.UI.WebControls.FontUnit;
using Symbol=Dundas.Maps.WebControl.Symbol;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class iPad_0001_0003_v : CustomReportPage
    {
        private DataTable dt;
        private DataTable dtChart;
        private DateTime debtsCurrDateTime;
        private DateTime debtsLastDateTime;
        private DataTable dtMap1;
        private DataTable dtMap2;
        // Дата
        private CustomParam periodDay;
        private CustomParam periodLastDay;
        
        private DataTable dtKoeff;

        private DateTime redundantLevelRFDateTime;

        private DateTime currDateTime;
        private DateTime lastDateTime;

        // Текущая дата
        private CustomParam periodCurrentDate;
        // Текущая дата для задолженности
        private CustomParam debtsPeriodCurrentDate;
        // На неделю назад для задолженности
        private CustomParam debtsPeriodLastWeekDate;
        // На неделю назад
        private CustomParam periodLastWeekDate;

        // Текущая дата для уровня безработицы по РФ
        private CustomParam redundantLevelRFDate;

        //private DateTime redundantLevelRFDateTime;

        public bool IsYearJoint()
        {
            return (currDateTime.Year != lastDateTime.Year);
        }

        private bool IsSmallResolution
        {
            get { return true; }
        }

        private int MinScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private int MinScreenHeight
        {
            get { return IsSmallResolution ? 600 : CustomReportConst.minScreenHeight; }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            UltraChart4.Width = 740;
            UltraChart4.Height = 240;

            DundasMap1.Width = 740;
            DundasMap1.Height = CRHelper.GetChartHeight(MinScreenHeight * 0.85);

            DundasMap2.Width = 740;
            DundasMap2.Height = CRHelper.GetChartHeight(MinScreenHeight * 0.85);

            DundasMap2.PostPaint += new MapPaintEvent(DundasMap2_PostPaint);

            #region Настройка карты 1

            DundasMap1.Meridians.Visible = false;
            DundasMap1.Parallels.Visible = false;
            DundasMap1.ZoomPanel.Visible = !IsSmallResolution;
            DundasMap1.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap1.NavigationPanel.Visible = !IsSmallResolution;
            DundasMap1.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap1.Viewport.EnablePanning = true;
            DundasMap1.Viewport.OptimizeForPanning = false;
            DundasMap1.Viewport.BackColor = Color.Black;
            
            // добавляем легенду
            DundasMap1.Legends.Clear();
            // добавляем легенду с символами
            Legend legend2 = new Legend("SymbolLegend");
            legend2.Visible = true;
            legend2.Dock = PanelDockStyle.Left;
            legend2.BackColor = Color.FromArgb(75, 255, 255, 255);
            legend2.BackSecondaryColor = Color.Black;
            legend2.BackGradientType = GradientType.TopBottom;
            legend2.BackHatchStyle = MapHatchStyle.None;
            legend2.BorderColor = Color.Black;
            legend2.BorderWidth = 1;
            legend2.BorderStyle = MapDashStyle.Solid;
            legend2.BackShadowOffset = 4;
            legend2.TextColor = Color.FromArgb(192, 192, 192);
            legend2.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend2.AutoFitText = true;
            legend2.Title = IsSmallResolution ? "Число безработных" : "Общая численность зарегистрированных безработных граждан";
            legend2.TitleColor = Color.White;
            legend2.AutoFitMinFontSize = 7;
            DundasMap1.Legends.Add(legend2);

            // добавляем легенду раскраски
            Legend legend1 = new Legend("CompleteLegend");
            legend1.Visible = true;
            legend1.Dock = PanelDockStyle.Left;
            legend1.BackColor = Color.FromArgb(75, 255, 255, 255);
            legend1.BackSecondaryColor = Color.Black;
            legend1.BackGradientType = GradientType.TopBottom;
            legend1.BackHatchStyle = MapHatchStyle.None;
            legend1.BorderColor = Color.Black;
            legend1.BorderWidth = 1;
            legend1.BorderStyle = MapDashStyle.Solid;
            legend1.BackShadowOffset = 4;
            legend1.TextColor = Color.FromArgb(192, 192, 192);
            legend1.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend1.AutoFitText = true;
            legend1.TitleColor = Color.White;
            legend1.Title = IsSmallResolution ? "Уровень безработицы" : "Уровень регистрируемой безработицы";
            legend1.AutoFitMinFontSize = 7;
            DundasMap1.Legends.Add(legend1);

            // добавляем поля для раскраски
            DundasMap1.ShapeFields.Clear();
            DundasMap1.ShapeFields.Add("Name");
            DundasMap1.ShapeFields["Name"].Type = typeof(string);
            DundasMap1.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap1.ShapeFields.Add("UnemploymentLevel");
            DundasMap1.ShapeFields["UnemploymentLevel"].Type = typeof(double);
            DundasMap1.ShapeFields["UnemploymentLevel"].UniqueIdentifier = false;


            // добавляем поля для символов
            DundasMap1.SymbolFields.Add("UnemploymentPopulation");
            DundasMap1.SymbolFields["UnemploymentPopulation"].Type = typeof(double);
            DundasMap1.SymbolFields["UnemploymentPopulation"].UniqueIdentifier = false;

            // добавляем правила раскраски
            DundasMap1.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "UnemploymentLevel";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Green;
            rule.MiddleColor = Color.Orange;
            rule.ToColor = Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText = "#FROMVALUE{P1} - #TOVALUE{P1}";

            DundasMap1.ShapeRules.Add(rule);

            // добавляем правила расстановки символов
            DundasMap1.SymbolRules.Clear();
            SymbolRule symbolRule = new SymbolRule();
            symbolRule.Name = "SymbolRule";
            symbolRule.Category = string.Empty;
            symbolRule.DataGrouping = DataGrouping.EqualInterval;
            symbolRule.SymbolField = "UnemploymentPopulation";
            symbolRule.ShowInLegend = "SymbolLegend";
            DundasMap1.SymbolRules.Add(symbolRule);

            // звезды для легенды
            for (int i = 1; i < 4; i++)
            {
                PredefinedSymbol predefined = new PredefinedSymbol();
                predefined.Name = "PredefinedSymbol" + i;
                predefined.MarkerStyle = MarkerStyle.Star;
                predefined.Width = 5 + (i * 5);
                predefined.Height = predefined.Width;
                predefined.Color = Color.DarkViolet;
                DundasMap1.SymbolRules["SymbolRule"].PredefinedSymbols.Add(predefined);
            }
            

            #endregion

            #region Настройка карты 2

            DundasMap2.Meridians.Visible = false;
            DundasMap2.Parallels.Visible = false;
            DundasMap2.ZoomPanel.Visible = !IsSmallResolution;
            DundasMap2.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap2.NavigationPanel.Visible = !IsSmallResolution;
            DundasMap2.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap2.Viewport.EnablePanning = true;
            DundasMap2.Viewport.OptimizeForPanning = false;
            DundasMap2.Viewport.BackColor =  Color.Black;

            // добавляем легенду
            DundasMap2.Legends.Clear();

            // добавляем легенду раскраски
            legend1 = new Legend("TensionLegend");
            legend1.Visible = true;
            legend1.Dock = PanelDockStyle.Left;
            legend1.BackColor = Color.FromArgb(75, 255, 255, 255);
            legend1.BackSecondaryColor = Color.Black;
            legend1.BackGradientType = GradientType.TopBottom;
            legend1.BackHatchStyle = MapHatchStyle.None;
            legend1.BorderColor = Color.Black;
            legend1.BorderWidth = 1;
            legend1.BorderStyle = MapDashStyle.Solid;
            legend1.BackShadowOffset = 4;
            legend1.TextColor = Color.FromArgb(192, 192, 192);
            legend1.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend1.AutoFitText = true;
            legend1.Title = IsSmallResolution ? "Число безработных на 1 вакансию" : "Число зарегистрированных безработных в расчёте на 1 вакансию";
            legend1.TitleColor = Color.White;
            legend1.AutoFitMinFontSize = 7;
            DundasMap2.Legends.Add(legend1);
            
            // добавляем легенду с символами
            legend2 = new Legend("VacancyLegend");
            legend2.Visible = true;
            legend2.Dock = PanelDockStyle.Left;
            legend2.BackColor = Color.FromArgb(75, 255, 255, 255);
            legend2.BackSecondaryColor = Color.Black;
            legend2.BackGradientType = GradientType.TopBottom;
            legend2.BackHatchStyle = MapHatchStyle.None;
            legend2.BorderColor = Color.Black;
            legend2.BorderWidth = 1;
            legend2.BorderStyle = MapDashStyle.Solid;
            legend2.BackShadowOffset = 4;
            legend2.TextColor = Color.FromArgb(192, 192, 192);
            legend2.Font = new Font("MS Sans Serif", 7, FontStyle.Regular);
            legend2.AutoFitText = true;
            legend2.TitleColor = Color.White;
            legend2.Title = IsSmallResolution ? "" : "Соотношение числа безработных и числа вакансий";
            legend2.AutoFitMinFontSize = 7;
            DundasMap2.Legends.Add(legend2);

            // добавляем поля для раскраски
            DundasMap2.ShapeFields.Clear();
            DundasMap2.ShapeFields.Add("Name");
            DundasMap2.ShapeFields["Name"].Type = typeof(string);
            DundasMap2.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap2.ShapeFields.Add("TensionKoeff");
            DundasMap2.ShapeFields["TensionKoeff"].Type = typeof(double);
            DundasMap2.ShapeFields["TensionKoeff"].UniqueIdentifier = false;

            // добавляем поля для символов
            DundasMap2.SymbolFields.Add("VacancyCount");
            DundasMap2.SymbolFields["VacancyCount"].Type = typeof(double);
            DundasMap2.SymbolFields["VacancyCount"].UniqueIdentifier = false;
            DundasMap2.SymbolFields.Add("RedundantCount");
            DundasMap2.SymbolFields["RedundantCount"].Type = typeof(double);
            DundasMap2.SymbolFields["RedundantCount"].UniqueIdentifier = false;

            LegendItem item = new LegendItem();
            item.Text = "Число безработных";
            item.Color = Color.DarkViolet;
            legend2.Items.Add(item);

            item = new LegendItem();
            item.Text = "Число вакансий";
            item.Color = Color.Blue;
            legend2.Items.Add(item);

            // добавляем правила раскраски
            DundasMap2.ShapeRules.Clear();
            rule = new ShapeRule();
            rule.Name = "TensionKoeffRule";
            rule.Category = String.Empty;
            rule.ShapeField = "TensionKoeff";
            rule.DataGrouping = DataGrouping.EqualInterval;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Green;
            rule.MiddleColor = Color.Orange;
            rule.ToColor = Color.Red;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "TensionLegend";
            rule.LegendText = "#FROMVALUE{N0} - #TOVALUE{N0}";
            DundasMap2.ShapeRules.Add(rule);

            #endregion

            #region Настройка диаграммы 4

            UltraChart4.ChartType = ChartType.AreaChart;
            UltraChart4.Border.Thickness = 0;

            UltraChart4.Tooltips.FormatString = "<ITEM_LABEL>";
            UltraChart4.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart4.Axis.X.Extent = 50;
            UltraChart4.Axis.X.Labels.Font = new Font("Verdana", 8);
            UltraChart4.Axis.X.Labels.Visible = true;
          //  UltraChart4.Axis.X.Labels.FontColor = Color.Black;
            UltraChart4.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            UltraChart4.Axis.Y.Labels.Font = new Font("Verdana", 8);
          //  UltraChart4.Axis.Y.Labels.FontColor = Color.Black;
            UltraChart4.Axis.Y.Extent = 40;

            UltraChart4.TitleLeft.Visible = true;
            UltraChart4.TitleLeft.Text = "млн.руб.";
            UltraChart4.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart4.TitleLeft.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart4.TitleLeft.Extent = 40;
            UltraChart4.TitleLeft.Margins.Top = 0;
            UltraChart4.TitleLeft.Font = new Font("Verdana", 10);
            UltraChart4.TitleLeft.FontColor = Color.White;

            UltraChart4.ColorModel.ModelStyle = ColorModels.CustomLinear;
            UltraChart4.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            UltraChart4.Data.EmptyStyle.Text = " ";
            UltraChart4.EmptyChartText = " ";

            UltraChart4.AreaChart.NullHandling = NullHandling.Zero;

            LineAppearance lineAppearance = new LineAppearance();
            lineAppearance.IconAppearance.Icon = SymbolIcon.Circle;
            lineAppearance.IconAppearance.IconSize = SymbolIconSize.Medium;
            lineAppearance.Thickness = 3;
            UltraChart4.AreaChart.LineAppearances.Add(lineAppearance);

            UltraChart4.Legend.Visible = true;
            UltraChart4.Legend.Location = LegendLocation.Top;
          //  UltraChart4.Legend.Margins.Right = IsSmallResolution ? 5 : Convert.ToInt32(UltraChart4.Width.Value) / 2;
            UltraChart4.Legend.SpanPercentage = 14;
            UltraChart4.Legend.Font = new Font("Verdana", 10);

            UltraChart4.InvalidDataReceived +=
                new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(
                    CRHelper.UltraChartInvalidDataReceived);
            UltraChart4.FillSceneGraph +=
                new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart4_FillSceneGraph);

            #endregion

            #region Инициализация параметров запроса

            if (periodCurrentDate == null)
            {
                periodCurrentDate = UserParams.CustomParam("period_current_date");
            }
            if (debtsPeriodCurrentDate == null)
            {
                debtsPeriodCurrentDate = UserParams.CustomParam("period_current_date_debts");
            }
            if (debtsPeriodLastWeekDate == null)
            {
                debtsPeriodLastWeekDate = UserParams.CustomParam("period_last_week_date_debts");
            }
            if (periodLastWeekDate == null)
            {
                periodLastWeekDate = UserParams.CustomParam("period_last_week_date");
            }
            if (redundantLevelRFDate == null)
            {
                redundantLevelRFDate = UserParams.CustomParam("redundant_level_RF_date");
            }

            periodDay = UserParams.CustomParam("period_day");
            periodLastDay = UserParams.CustomParam("period_last_day");

            #endregion

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                       
            DateTime date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][5].ToString(), 3);
            DateTime lastDate = date.AddDays(-7);

            TextBox1.Text = string.Format("На&nbsp;<span style='color: white'><b>{1:dd.MM.yyyy}</b></span>&nbsp;по данным органов службы занятости (динамика за неделю с&nbsp;{0:dd.MM}&nbsp;по&nbsp;{1:dd.MM})", lastDate, date);

            mapElementCaption.Text = string.Format("Уровень безработицы и численность безработных граждан на {0:dd.MM.yyyy}", date);
            map2ElementCaption.Text = string.Format("Число безработных в расчёте на 1 вакансию на {0:dd.MM.yyyy}", date);

            periodDay.Value = dtDate.Rows[0][5].ToString();
            periodLastDay.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", lastDate, 5);

            dtDate = new DataTable();
            query = DataProvider.GetQueryText("STAT_0001_0012_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            UserParams.PeriodYear.Value = dtDate.Rows[0][4].ToString();
            date = CRHelper.DateByPeriodMemberUName(dtDate.Rows[0][4].ToString(), 3);
            TextBox2.Text = string.Format("В&nbsp;<span style='color: white'><b>{0} {1} года</b></span>&nbsp;по данным органов статистики (динамика за {2} {1})", CRHelper.RusMonthPrepositional(date.Month), date.Year, CRHelper.RusMonth(date.Month));
            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid.DataBind();

            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid2.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid3.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            
            UltraWebGrid2.DataBind();
            UltraWebGrid3.DataBind();            

            query = DataProvider.GetQueryText("STAT_0001_0001_debts_date");
            DataTable dtDebtsDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsDate);

            debtsPeriodCurrentDate.Value = dtDebtsDate.Rows[1][1].ToString();
            debtsPeriodLastWeekDate.Value = dtDebtsDate.Rows[0][1].ToString();
            date = CRHelper.DateByPeriodMemberUName(dtDebtsDate.Rows[1][1].ToString(), 3);

            UltraWebGrid1.DataBind();

            CommentTextDataBind();
            UltraChart4.DataBind();

            // заполняем карту формами
            string regionStr = "УрФО";
            DundasMap1.Shapes.Clear();
            DundasMap1.LoadFromShapeFile(Server.MapPath(string.Format("../../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);
            // заполняем карту данными
            FillMapData1(DundasMap1);

            DundasMap2.Shapes.Clear();
            DundasMap2.LoadFromShapeFile(Server.MapPath(string.Format("../../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);
            // заполняем карту данными
            FillMapData2(DundasMap2);
        }

        #region Обработчики карты

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <returns>найденная форма</returns>
        public static Shape FindMapShape(MapControl map, string patternValue)
        {
            string subject = patternValue.Replace("область", "обл.");
            subject = subject.Replace("автономный округ", "АО");

            ArrayList shapeList = map.Shapes.Find(subject, true, false);
            if (shapeList.Count > 0)
            {
                return (Shape)shapeList[0];
            }
            return null;
        }

        public void FillMapData1(MapControl map)
        {
            if (map == null)
            {
                return;
            }

            string query = DataProvider.GetQueryText("STAT_0001_0002_map1");

            dtMap1 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtMap1);

            map.Symbols.Clear();

            foreach (DataRow row in dtMap1.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty)
                {
                    string regionName = row[0].ToString();

                    if (RegionsNamingHelper.IsSubject(regionName))
                    {
                        Shape shape = FindMapShape(map, regionName);
                        if (shape != null)
                        {
                            double unemploymentLevel = Convert.ToDouble(row[1]) / 100;
                            double unemploymentPopulation = Convert.ToDouble(row[2]);

                            shape["Name"] = regionName;
                            shape["UnemploymentLevel"] = unemploymentLevel;
                            shape.ToolTip = string.Format("#NAME \nчисленность безработных: {0:N0} чел.\nуровень безработицы: #UNEMPLOYMENTLEVEL{{P3}}",
                                    unemploymentPopulation);
                            shape.TextAlignment = ContentAlignment.MiddleCenter;
                            shape.Offset.X = -15;
                            if (!IsSmallResolution)
                            {
                                shape.Offset.Y = -30;
                            }

                            shape.Text = string.Format("{0}\n{2:N0} чел.\n{1:P3}", shape.Name, unemploymentLevel, unemploymentPopulation);

                            shape.BorderWidth = 2;
                            shape.TextColor = Color.White;
                            //shape.Font = new Font("Verdana", IsSmallResolution ? 7 : 8, FontStyle.Bold);
                            shape.TextVisibility = TextVisibility.Shown;

                            Symbol symbol = new Symbol();
                            symbol.Name = shape.Name + map.Symbols.Count;
                            symbol.ParentShape = shape.Name;
                            symbol["UnemploymentPopulation"] = unemploymentPopulation;
                            symbol.Offset.Y = -30;
                            symbol.Color = Color.DarkViolet;                            
                            symbol.MarkerStyle = MarkerStyle.Star;
                            map.Symbols.Add(symbol);

                            if (IsSmallResolution)
                            {
                                if (shape.Name.Contains("Курган"))
                                {
                                    shape.TextAlignment = ContentAlignment.MiddleCenter;
                                }
                                if (shape.Name.Contains("Челябинск"))
                                {
                                    shape.TextAlignment = ContentAlignment.TopCenter;
                                    shape.Offset.X = -10;
                                    symbol.Offset.Y = -10;
                                }
                                if (shape.Name.Contains("Тюмен"))
                                {
                                    //                                    shape.TextAlignment = ContentAlignment.TopCenter;
                                    //                                    symbol.Offset.Y = -10;
                                }
                                if (shape.Name.Contains("Свердловск"))
                                {
                                    shape.Offset.Y = -10;
                                    //symbol.Offset.Y = -10;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void FillMapData2(MapControl map)
        {
            if (map == null)
            {
                return;
            }

            string query = DataProvider.GetQueryText("STAT_0001_0002_map2");

            dtMap2 = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtMap2);

            foreach (DataRow row in dtMap2.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty &&
                    row[1] != DBNull.Value && row[1].ToString() != string.Empty &&
                    row[2] != DBNull.Value && row[2].ToString() != string.Empty)
                {
                    string regionName = row[0].ToString();

                    if (RegionsNamingHelper.IsSubject(regionName))
                    {
                        Shape shape = FindMapShape(map, regionName);
                        if (shape != null)
                        {
                            double tensionKoeff = Convert.ToDouble(row[1]);
                            double redundantCount = Convert.ToDouble(row[2]);
                            double vacancyCount = Convert.ToDouble(row[3]);
                            double totalCount = vacancyCount + redundantCount;

                            shape["Name"] = regionName;
                            shape["TensionKoeff"] = tensionKoeff;
                            shape.ToolTip = string.Format("#NAME \nчисло зарегистрированных безработных в расчёте на 1 вакансию: #TENSIONKOEFF{{N2}}\nчисло безработных: {0:N0} чел.\nчисло вакансий: {1:N0}",
                                    redundantCount, vacancyCount);
                            shape.TextAlignment = ContentAlignment.MiddleCenter;
                            shape.TextColor = Color.White;
                            shape.Offset.X = -15;
                            if (!IsSmallResolution)
                            {
                                shape.Offset.Y = -30;
                            }

                            shape.Text = string.Format("{0}\nвакансий: {2:N0}\n{1:N2}", shape.Name, tensionKoeff, vacancyCount);
                            shape.BorderWidth = 2;
                            //shape.Font = new Font("Verdana", IsSmallResolution ? 7 : 8, FontStyle.Bold);
                            shape.TextVisibility = TextVisibility.Shown;

                            Symbol symbol = new Symbol();
                            symbol.Name = shape.Name + map.Symbols.Count;
                            symbol.ParentShape = shape.Name;
                            symbol["vacancyCount"] = totalCount == 0 ? 0 : vacancyCount / totalCount * 100;
                            symbol["redundantCount"] = totalCount == 0 ? 0 : redundantCount / totalCount * 100;
                            symbol.Offset.Y = -40;
                            symbol.MarkerStyle = MarkerStyle.Circle;
                            map.Symbols.Add(symbol);

                            if (IsSmallResolution)
                            {
                                if (shape.Name.Contains("Курган"))
                                {
                                    shape.TextAlignment = ContentAlignment.MiddleCenter;
                                    shape.Offset.Y = -20;
                                }
                                if (shape.Name.Contains("Челябинск"))
                                {
                                    shape.TextAlignment = ContentAlignment.TopCenter;
                                    symbol.Offset.Y = -20;
                                }
                                if (shape.Name.Contains("Тюмен"))
                                {
                                    //                                    shape.TextAlignment = ContentAlignment.TopCenter;
                                    //                                    symbol.Offset.Y = -20;
                                }
                                if (shape.Name.Contains("Свердловск"))
                                {
                                    shape.Offset.Y = -10;
                                    //symbol.Offset.Y = -10;
                                }
                            }
                        }
                    }
                }
            }
        }

        void DundasMap2_PostPaint(object sender, MapPaintEventArgs e)
        {
            Symbol symbol = e.MapElement as Symbol;
            if (symbol != null && symbol.Visible)
            {
                // Размер диаграммы
                int width = 30;
                int height = 30;

                // Get the symbol location in pixels.
                MapGraphics mg = e.Graphics;
                PointF p = symbol.GetCenterPointInContentPixels(mg);
                int x = (int)p.X - width / 2;
                int y = (int)p.Y - height / 2;
                symbol.Width = width;
                symbol.Height = height;

                int startAngle, sweepAngle1, sweepAngle2;

                // Делим углы соотвественно долям
                startAngle = 0;
                sweepAngle1 = Convert.ToInt32(3.6 * Convert.ToInt32(symbol["redundantCount"]));
                sweepAngle2 = Convert.ToInt32(3.6 * Convert.ToInt32(symbol["vacancyCount"]));

                // Поверх символа рисуем круговую диаграмму
                Graphics g = mg.Graphics;
                g.FillPie(new SolidBrush(Color.DarkViolet), x, y, width, height, startAngle, sweepAngle1);
                startAngle += sweepAngle1;
                g.FillPie(new SolidBrush(Color.Blue), x, y, width, height, startAngle, sweepAngle2);

                g.DrawEllipse(new Pen(Color.Gray, 1), x, y, width, height);
            }
        }

        #endregion

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0001_h");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((dt.Rows[i][0].ToString().Contains("Уровень регистрируемой безработицы") ||
                             dt.Rows[i][0].ToString().Contains("Число зарегистрированных безработных в расчёте на 1 вакансию")) && dt.Rows[i][0].ToString().Contains("Прирост"))
                {
                    dt.Rows[i][0] = "ранг по УрФО";
                }
                else
                {
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Split(';')[0];
                }
            }
            ((UltraWebGrid)sender).DataSource = dt;
        }

        protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0001_h_redudants");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((dt.Rows[i][0].ToString().Contains("Уровень регистрируемой безработицы") ||
                             dt.Rows[i][0].ToString().Contains("Число зарегистрированных безработных в расчёте на 1 вакансию")) && dt.Rows[i][0].ToString().Contains("Прирост"))
                {
                    dt.Rows[i][0] = "ранг по УрФО";
                }
                else
                {
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Split(';')[0];
                }
            }
            ((UltraWebGrid)sender).DataSource = dt;
        }

        protected void UltraWebGrid3_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0001_mot_h");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((dt.Rows[i][0].ToString().Contains("Уровень общей безработицы по методологии МОТ")) && dt.Rows[i][0].ToString().Contains("Прирост"))
                {
                    dt.Rows[i][0] = "ранг по УрФО";
                }
                else
                {
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Split(';')[0];
                }
            }
            ((UltraWebGrid)sender).DataSource = dt;
        }

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            dt = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0001_debts_h");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((dt.Rows[i][0].ToString().Contains("Уровень общей безработицы по методологии МОТ")) && dt.Rows[i][0].ToString().Contains("Прирост"))
                {
                    dt.Rows[i][0] = "ранг по УрФО";
                }
                else
                {
                    dt.Rows[i][0] = dt.Rows[i][0].ToString().Split(';')[0];
                }
            }
            ((UltraWebGrid)sender).DataSource = dt;
        }


        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands.Count == 0)
                return;

            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.Bands[0].SelectTypeCell = SelectType.None;
            e.Layout.HeaderClickActionDefault = HeaderClickAction.Select;
            e.Layout.TableLayout = TableLayout.Fixed;
            e.Layout.RowStyleDefault.Wrap = true;
            
            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            e.Layout.Bands[0].Columns[0].CellStyle.VerticalAlign = VerticalAlign.Top;
           
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            e.Layout.Bands[0].Columns[0].Width = 188;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = 80;
                // CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                e.Layout.Bands[0].Columns[i].Header.Caption =
                        RegionsNamingHelper.ShortRegionsNames[e.Layout.Bands[0].Columns[i].Header.Caption.Trim(' ')];
                e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[i].CellStyle.VerticalAlign = VerticalAlign.Top;
            }
        }

        void UltraChart_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            string currentYear = string.Empty;

            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is PointSet)
                {
                    PointSet pointSet = (PointSet)primitive;
                    foreach (DataPoint point in pointSet.points)
                    {
                        point.hitTestRadius = 15;
                    }
                }
            }
        }

        private void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Index % 3 == 0 && e.Row.Cells[i].Value != null)
                {
                    if (e.Row.Cells[0].Value.ToString() == "Уровень регистрируемой безработицы")
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N3}%", e.Row.Cells[i].Value);
                        e.Row.Cells[i].Title = String.Format("{0}\n,{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                    }
                    else if (e.Row.Cells[0].Value.ToString() == "Число зарегистрированных безработных в расчёте на 1 вакансию")
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N2}", e.Row.Cells[i].Value);
                        e.Row.Cells[i].Title = String.Format("{0}\n,{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                    }
                    else if (e.Row.Cells[0].Value.ToString() == "Сумма задолженности по выплате заработной платы (млн.руб.)")
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N3}", e.Row.Cells[i].Value);
                        e.Row.Cells[i].Title = String.Format("{0}\n,{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                    } 
                    else if (e.Row.Cells[0].Value.ToString() == "Уровень общей безработицы по методологии МОТ ")
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N2}%", e.Row.Cells[i].Value);
                        e.Row.Cells[i].Title = String.Format("{0}\n,{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                    }
                    else if (e.Row.Cells[0].Value.ToString() == "Численность экономически активного населения ")
                    {
                        string value = String.Empty;
                        int k = 0;
                        for (int j = e.Row.Cells[i].Value.ToString().Length - 1; j >= 0; j-- )
                        {
                            value = e.Row.Cells[i].Value.ToString()[j] + value;
                            k++;
                            if (k == 3)
                            {
                                k = 0;
                                value = "<nobr><span style=\"color: black\">'</span></nobr>" + value;
                            }
                        }
                        e.Row.Cells[i].Value = value;
                    }
                    else
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N0}", e.Row.Cells[i].Value);
                        e.Row.Cells[i].Title = String.Format("{0}\n,{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                    }
                    e.Row.Cells[i].Style.Padding.Top = 5;
                    e.Row.Cells[i].Style.Padding.Bottom = 10;
                    if (e.Row.Cells[0].Value.ToString().Contains("аналогичный период прошлого года"))
                    {
                        e.Row.Cells[0].Style.Font.Italic = true;
                    }
                }
                if (e.Row.Index % 3 == 1 && e.Row.Cells[i].Value != null)
                {
                    //   e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "../../../images/CornerGreen.gif";
                    e.Row.Style.BorderDetails.WidthBottom = 0;
                    double value;
                    if (Double.TryParse(e.Row.Cells[i].Value.ToString(), out value))
                    {
                        e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.Padding.Top = 5;
                        e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.Padding.Bottom = 10;

                        if (value > 0)
                        {
                            e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "../../../images/CornerRed.gif";
                            e.Row.Cells[i].Value = String.Format("+{0:P2}", value);
                            e.Row.Cells[i].Title = String.Format("{0}\n,{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                        }
                        else
                        {
                            e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "../../../images/CornerGreen.gif";
                            e.Row.Cells[i].Value = String.Format("{0:P2}", value);
                            e.Row.Cells[i].Title = String.Format("{0}\n,{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                        }
                    }
                    e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top;";
                }

                if (e.Row.Index % 3 == 2 && e.Row.Cells[i].Value != null)
                {
                    if (e.Row.Cells[0].Value.ToString() == "Сумма задолженности по выплате заработной платы (млн.руб.)")
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N3}", e.Row.Cells[i].Value);
                        e.Row.Cells[i].Title = String.Format("{0}\n,{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                    }
                    
                    else
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N0}", e.Row.Cells[i].Value);
                        e.Row.Cells[i].Title = String.Format("{0}\n,{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                    }
                }

                e.Row.Cells[i].Style.Padding.Right = 2;

                if (e.Row.Cells[0].Value.ToString() == "ранг по УрФО" )
                {
                    if (i < e.Row.Cells.Count - 1)
                    {
                        e.Row.Band.Grid.Rows[e.Row.Index - 2].Cells[0].Style.BorderDetails.WidthBottom = 0;
                        e.Row.Style.BorderDetails.WidthTop = 0;
                        e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Right;

                        if (e.Row.Cells[i].ToString() == "6")
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "../../../images/StarYellow.png";
                            e.Row.Cells[i].Style.CustomRules =
                                "background-repeat: no-repeat; background-position: 20px top;";
                        }
                        else if (e.Row.Cells[i].ToString() == "1")
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "../../../images/StarGray.png";
                            e.Row.Cells[i].Style.CustomRules =
                                "background-repeat: no-repeat; background-position: 20px top;";
                        }
                    }
                    else
                    {
                        e.Row.Cells[i].Value = String.Empty;
                    }
                }
                else
                {
                    e.Row.Cells[0].Style.BorderDetails.WidthBottom = 1;
                }
            }

            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.Padding.Right = 5;
            }

            e.Row.Style.BorderDetails.WidthBottom = 0;
            e.Row.Style.BorderDetails.WidthTop = 0;

            if ((e.Row.Index + 1) % 3 == 0)
            {
                e.Row.Style.BorderDetails.WidthBottom = 1;
                e.Row.Style.BorderDetails.WidthTop = 0;
            }

           // e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().Replace("ь зарегистри", "ь зарегистри- ");
        }

        #region Комментарии к гриду

        private void CommentTextDataBind()
        {
            DataTable dtDateCur = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0001_date");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDateCur);

            currDateTime = CRHelper.DateByPeriodMemberUName(dtDateCur.Rows[0][5].ToString(), 3);
            lastDateTime = currDateTime.AddDays(-7);

            periodCurrentDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", currDateTime, 5);
            periodLastWeekDate.Value = CRHelper.PeriodMemberUName("[Период].[Период].[Данные всех периодов]", lastDateTime, 5);

            query = DataProvider.GetQueryText("STAT_0001_0002_debts_date");
            DataTable dtDebtsDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtDebtsDate);

            query = DataProvider.GetQueryText("STAT_0001_0002_redundantLevelRF_date");
            DataTable dtRedundantLevelRFDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Дата", dtRedundantLevelRFDate);
            redundantLevelRFDate.Value = dtRedundantLevelRFDate.Rows[0][1].ToString();
            redundantLevelRFDateTime = CRHelper.DateByPeriodMemberUName(dtRedundantLevelRFDate.Rows[0][1].ToString(), 3);

            if (dtDebtsDate.Rows.Count > 1)
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

            query = DataProvider.GetQueryText("STAT_0001_0002_commentText");
            DataTable dtCommentText = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtCommentText);

            if (dtCommentText.Rows.Count > 0)
            {
                
                string dateTimeDebtsStr = string.Format("{0:dd.MM.yyyy}", debtsCurrDateTime);
                string dateLastTimeDebtsStr = string.Format("{0:dd.MM.yyyy}", debtsLastDateTime);
                
                double factoryCount = GetDoubleDTValue(dtCommentText, "Число организаций");
                double incompleteEmployersCount = GetDoubleDTValue(dtCommentText, "Численность работников с неполной занятостью");
                double totalDebts = GetDoubleDTValue(dtCommentText, "Cумма задолженности");
                double totalLastWeekDebts = GetDoubleDTValue(dtCommentText, "Cумма задолженности прошлая неделя");
                double slavesCount = GetDoubleDTValue(dtCommentText, "Количество граждан, имеющих задолженность");
                double debtsPercent = GetDoubleDTValue(dtCommentText, "Прирост задолженности");
                string debtsPercentArrow = debtsPercent == 0
                                               ? "не изменилась"
                                               : debtsPercent > 0
                                               ? string.Format("увеличилась&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width=\"13px\" height=\"16px\">&nbsp;на&nbsp;<span style='color: white'><b>{0:N3}</b></span>&nbsp;млн.руб", Math.Abs(debtsPercent))
                                               : string.Format("уменьшилась&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width=\"13px\" height=\"16px\">&nbsp;на&nbsp;<span style='color: white'><b>{0:N3}</b></span>&nbsp;млн.руб", Math.Abs(debtsPercent));

                double almostRedundantsCount = GetDoubleDTValue(dtCommentText, "Численность работников, предполагаемых к увольнению");
                double almostRedundantsRate = GetDoubleDTValue(dtCommentText, "Прирост числа работников, предполагаемых к увольнению");
                string monitoringStartStr1 = currDateTime.Year == 2009
                                                ? "С начала проведения мониторинга (с октября 2008 года)"
                                                : "С начала года";
                string monitoringStartStr2 = currDateTime.Year == 2009
                                                ? "С начала проведения мониторинга"
                                                : "С начала года";
                double redundantsTotal = GetDoubleDTValue(dtCommentText, "Численность работников, уволенных с начала мониторинга");
                double redundantsTotalRate = GetDoubleDTValue(dtCommentText, "Прирост числа работников, уволенных за последнюю неделю");
               
                DateTime nextDateTime = currDateTime.AddMonths(1);
                int nextMonthNumber = nextDateTime.Month;
                int monthCount = currDateTime.Month != 12 ? (currDateTime.Year - 2007) * 12 + nextMonthNumber - 1 : (currDateTime.Year - 2007 + 1) * 12;

                if (dtKoeff == null)
                {
                    dtKoeff = GetSubjectKoeffTable();
                }


                double forecastValue = 0;
                for (int i = 1; i < dtKoeff.Rows.Count; i++)
                {
                    DataRow koeffRow = dtKoeff.Rows[i];

                    double b0 = Convert.ToDouble(koeffRow[1]);
                    double xi = Convert.ToDouble(koeffRow[2]) * (monthCount);
                    double koeff = (nextDateTime.Month == 12) ? 0 : Convert.ToDouble(koeffRow[nextMonthNumber + 2]);
                    double logForecast = b0 + xi + koeff;
                    forecastValue += Math.Pow(10, logForecast);
                }

                string str9 = string.Format(@"{1} общая заявленная численность 
работников, предполагаемых к увольнению (ликвидация  организаций, сокращение численности или штата), составляет&nbsp;<span style='color: white'><b>{0:N0}</b></span>&nbsp;чел.", almostRedundantsCount, monitoringStartStr1);

                string str10 = !IsYearJoint() ? (almostRedundantsRate > 0) ? string.Format("&nbsp;Количество граждан,<br/>предполагаемых к увольнению, возросло&nbsp;<img src=\"../../../images/arrowRedUpBB.png\" width='13px' height='16px'>&nbsp;на&nbsp;<span style='color: white'><b>{0:N0}</b></span>&nbsp;чел. в сравнении с прошлой неделей.<br/>", Math.Abs(almostRedundantsRate)) : string.Format("&nbsp;Количество граждан, предполагаемых к увольнению, сократилось&nbsp;<img src=\"../../../images/arrowGreenDownBB.png\" width='13px' height='16px'>&nbsp;на&nbsp;<span style='color: white'><b>{0:N0}</b></span>&nbsp;чел. в сравнении с прошлой неделей.<br/>", Math.Abs(almostRedundantsRate)) : string.Empty;

                string str11 = !IsYearJoint() ? string.Format(@"{2} численность уволенных работников по сокращению 
достигла&nbsp;<span style='color: white'><b>{0:N0}</b></span>&nbsp;чел., из них за последнюю неделю –&nbsp;<span style='color: white'><b>{1:N0}</b></span>&nbsp;чел.<br/>", redundantsTotal, redundantsTotalRate, monitoringStartStr2) :
                        string.Format(@"&nbsp;{1} численность уволенных работников по сокращению 
достигла&nbsp;<span style='color: white'><b>{0:N0}</b></span>&nbsp;чел.<br/>", redundantsTotal, monitoringStartStr2);

                string str12 = string.Format(@"<span style='color: white'><b>{0:N0}</b></span>&nbsp;организации заявили о переводе части работников на режим неполного рабочего времени, 
<br/>предоставлении вынужденных отпусков, а также простое. Суммарная численность работников, находившихся в простое по вине администрации, 
работавших неполное рабочее время, а также работников, которым были предоставлены отпуска по инициативе администрации, составила&nbsp;<span style='color: white'><b>{1:N0}</b></span>&nbsp;чел.<br/>",
                    factoryCount, incompleteEmployersCount);

                string str13;
                if (totalLastWeekDebts == 0 && totalDebts == 0)
                {
                    str13 = string.Format(@"По состоянию на&nbsp;<span style='color: white'><b>{0}</b></span>&nbsp;отсутствует задолженность по выплате 
заработной платы.<br/>", dateTimeDebtsStr);
                }
                else if (totalDebts == 0)
                {
                    str13 = string.Format(@"По состоянию на&nbsp;<span style='color: white'><b>{0}</b></span>&nbsp;отсутствует задолженность по выплате заработной платы.
Задолженность в сумме&nbsp;<span style='color: white'><b>{1:N3}</b></span>&nbsp;млн.руб. была погашена в период с&nbsp;<span style='color: white'><b>{2}</b></span>&nbsp;по&nbsp;<span style='color: white'><b>{0}</b></span>.<br/>",
    dateTimeDebtsStr, totalLastWeekDebts, dateLastTimeDebtsStr);
                }
                else
                {
                    str13 = string.Format(@"По состоянию на&nbsp;<span style='color: white'><b>{0}</b></span>&nbsp;задолженность по выплате заработной платы составляет 
<span style='color: white'><b>{1:N3}</b></span>&nbsp;млн.рублей (<span style='color: white'><b>{2:N0}</b></span>&nbsp;чел.). В период с&nbsp;<span style='color: white'><b>{4}</b></span>&nbsp;по&nbsp;<span style='color: white'><b>{0}</b></span>&nbsp;задолженность {3}.",
                        dateTimeDebtsStr, totalDebts, slavesCount, debtsPercentArrow, dateLastTimeDebtsStr);
                }

               //string str14 = string.Format("&nbsp;&nbsp;&nbsp;Прогнозируемое значение численности безработных в целом по УрФО на <b>{0}&nbsp;{1}</b> года <b>{2:N0}</b> чел.", CRHelper.RusMonth(nextDateTime.Month), nextDateTime.Year, forecastValue);

                CommentText1.Text = string.Format("{0}{1}{2}{3}", str9, str10, str11, str12);
                //CommentText2.Text = string.Format("{0}", str14);
                CommentText2.Text = str13;
            }
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

        #endregion

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
                            point.DataPoint.Label = string.Format("{2}\nна {3}\n {0:N2}{1}", ((NumericDataPoint)point.DataPoint).Value, unit, point.Series.Label, point.DataPoint.Label);

                        }
                    }
                }
            }
        }

    }
}
