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

namespace Krista.FM.Server.Dashboards.reports.iPad
{
    public partial class STAT_0001_0007_v : CustomReportPage
    {
        private DataTable dt;
        private DataTable dtGrid1;
        private DataTable dtGrid2;
        private DataTable dtMap1;
        private DataTable dtMap2;
        // Дата
        private CustomParam periodDay;
        private CustomParam periodLastDay;

        DateTime lastDate;
        DateTime prevDate;
        DateTime oldLastDate;
        DateTime oldPrevDate;

        DateTime lastMonthDate;
        DateTime prevMonthDate;

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

        private CustomParam lastDateParam;
        private CustomParam prevDateParam;
        private CustomParam oldLastDateParam;
        private CustomParam oldPrevDateParam;
        private CustomParam lastMonthParam;
        private CustomParam prevMonthParam;

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UltraWebGrid1.Width = Unit.Empty;
            UltraWebGrid2.Width = Unit.Empty;

            DundasMap1.Width = 740;
            DundasMap1.Height = 740;

            DundasMap2.Width = 740;
            DundasMap2.Height = 740;

            DundasMap2.PostPaint += new MapPaintEvent(DundasMap2_PostPaint);

            #region Настройка карты 1

            DundasMap1.Meridians.Visible = false;
            DundasMap1.Parallels.Visible = false;
            DundasMap1.ZoomPanel.Visible = false;
            DundasMap1.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap1.NavigationPanel.Visible = false;
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
            legend2.Title = "Число безработных";
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
            legend1.Title = "Уровень безработицы";
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
            symbolRule.Category = String.Empty;
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
            DundasMap2.ZoomPanel.Visible = false;
            DundasMap2.ZoomPanel.Dock = PanelDockStyle.Right;
            DundasMap2.NavigationPanel.Visible = false;
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
            legend1.Title = "Уровень напряженности";
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
            legend2.Title = "Соотношение числа безработных\nи числа вакансий";
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
            rule.LegendText = "#FROMVALUE{N4} - #TOVALUE{N4}";
            DundasMap2.ShapeRules.Add(rule);

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

            lastDateParam = UserParams.CustomParam("last_date");
            prevDateParam = UserParams.CustomParam("prev_date");
            oldLastDateParam = UserParams.CustomParam("old_last_date");
            oldPrevDateParam = UserParams.CustomParam("old_prev_date");

            lastMonthParam = UserParams.CustomParam("last_month");
            prevMonthParam = UserParams.CustomParam("prev_month");

            #endregion

            string query = DataProvider.GetQueryText("STAT_0001_0007_v_grid1_date");
            DataTable dtDate = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "День", dtDate);
            lastDateParam.Value = dtDate.Rows[1]["Дата"].ToString();
            prevDateParam.Value = dtDate.Rows[0]["Дата"].ToString();
            oldLastDateParam.Value = dtDate.Rows[1]["Аналогичный период прошлого года"].ToString();
            oldPrevDateParam.Value = dtDate.Rows[0]["Аналогичный период прошлого года"].ToString();
            lastDate = CRHelper.DateByPeriodMemberUName(lastDateParam.Value, 3);
            prevDate = CRHelper.DateByPeriodMemberUName(prevDateParam.Value, 3);
            oldLastDate = CRHelper.DateByPeriodMemberUName(oldLastDateParam.Value, 3);
            oldPrevDate = CRHelper.DateByPeriodMemberUName(oldPrevDateParam.Value, 3);

            TextBox1.Text = String.Format(
                "На&nbsp;<span class='DigitsValue'>{1:dd.MM.yyyy}</span>&nbsp;по данным Минтруда НСО (динамика с {0:dd.MM} по {1:dd.MM})",
                prevDate, lastDate);

            query = DataProvider.GetQueryText("STAT_0001_0007_v_grid2_date");
            dtDate = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Месяц", dtDate);
            lastMonthParam.Value = dtDate.Rows[1]["Дата"].ToString();
            prevMonthParam.Value = dtDate.Rows[0]["Дата"].ToString();

            lastMonthDate = CRHelper.DateByPeriodMemberUName(lastMonthParam.Value, 3);
            prevMonthDate = CRHelper.DateByPeriodMemberUName(prevMonthParam.Value, 3);

            TextBox2.Text = String.Format(
                "В&nbsp;<span class='DigitsValue'>{0} {1} года</span>&nbsp;по данным органов статистики (динамика за {2} {1})",
                CRHelper.RusMonthPrepositional(lastMonthDate.Month), lastMonthDate.Year, CRHelper.RusMonth(lastMonthDate.Month));

            mapElementCaption.Text = String.Format("Уровень безработицы и численность безработных граждан на {0:dd.MM.yyyy}", lastDate);
            map2ElementCaption.Text = String.Format("Число безработных в расчёте на 1 вакансию на {0:dd.MM.yyyy}", lastDate);

            UltraWebGrid1.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid1.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid1.DataBinding += new EventHandler(UltraWebGrid1_DataBinding);
            
            UltraWebGrid2.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid2.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid2.DataBinding += new EventHandler(UltraWebGrid2_DataBinding);

            UltraWebGrid1.DataBind();
            UltraWebGrid2.DataBind();            

            // заполняем карту формами
            string regionStr = "СФО";
            DundasMap1.Shapes.Clear();
            DundasMap1.LoadFromShapeFile(Server.MapPath(String.Format("../../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);
            // заполняем карту данными
            FillMapData1(DundasMap1);

            DundasMap2.Shapes.Clear();
            DundasMap2.LoadFromShapeFile(Server.MapPath(String.Format("../../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);
            // заполняем карту данными
            FillMapData2(DundasMap2);
        }

        #region Обработчики карт

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <returns>найденная форма</returns>
        protected static Shape FindMapShape(MapControl map, string patternValue)
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

        protected void FillMapData1(MapControl map)
        {
            map.Symbols.Clear();

            for (int i = 1; i < dtGrid1.Columns.Count - 1; ++i)
            {
                // заполняем карту данными
                string regionName = dtGrid1.Columns[i].ColumnName.Replace("Республика ", "Р. ");

                if (RegionsNamingHelper.IsSubject(regionName))
                {
                    Shape shape = FindMapShape(map, regionName);
                    if (shape != null)
                    {
                        double unemploymentLevel = Convert.ToDouble(dtGrid1.Rows[6][i]) / 100;
                        double unemploymentPopulation = Convert.ToDouble(dtGrid1.Rows[0][i]);

                        shape["Name"] = regionName;
                        shape["UnemploymentLevel"] = unemploymentLevel;
                        shape.ToolTip = String.Format("#NAME\nчисленность безработных: {0:N0} чел.\nуровень безработицы: #UNEMPLOYMENTLEVEL{{P3}}",
                                unemploymentPopulation);
                        shape.TextAlignment = ContentAlignment.MiddleCenter;
                        shape.Offset.X = -15;

                        shape.Text = String.Format("{0}\n{2:N0} чел.\n{1:P3}", shape.Name, unemploymentLevel, unemploymentPopulation);

                        shape.BorderWidth = 2;
                        shape.TextColor = Color.White;
                        shape.TextVisibility = TextVisibility.Shown;

                        Symbol symbol = new Symbol();
                        symbol.Name = shape.Name + map.Symbols.Count;
                        symbol.ParentShape = shape.Name;
                        symbol["UnemploymentPopulation"] = unemploymentPopulation;
                        symbol.Offset.Y = -30;
                        symbol.Color = Color.DarkViolet;
                        symbol.MarkerStyle = MarkerStyle.Star;
                        map.Symbols.Add(symbol);

                    }
                }
            }
        }

        protected void FillMapData2(MapControl map)
        {
            string query = DataProvider.GetQueryText("STAT_0001_0007_v_map2");
            dtMap2 = new DataTable();
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Субъект", dtMap2);

            foreach (DataRow row in dtMap2.Rows)
            {
                string regionName = row["Субъект"].ToString().Replace("Республика ", "Р. ");

                if (RegionsNamingHelper.IsSubject(regionName))
                {
                    Shape shape = FindMapShape(map, regionName);
                    if (shape != null)
                    {
                        double tensionKoeff = Convert.ToDouble(row["Уровень напряженности на рынке труда"]);
                        double redundantCount = Convert.ToDouble(row["Численность незанятых граждан, состоящих на учете"]);
                        double vacancyCount = Convert.ToDouble(row["Число заявленных вакансий"]);
                        double totalCount = vacancyCount + redundantCount;

                        shape["Name"] = regionName;
                        shape["TensionKoeff"] = tensionKoeff;
                        shape.ToolTip = String.Format("#NAME\nУровень напряженности на рынке труда: #TENSIONKOEFF{{N4}}\nчисло безработных: {0:N0} чел.\nчисло вакансий: {1:N0}",
                                redundantCount, vacancyCount);
                        shape.TextAlignment = ContentAlignment.MiddleCenter;
                        shape.TextColor = Color.White;
                        shape.Offset.X = -20;

                        shape.Text = String.Format("{0}\nвакансий: {2:N0}\n{1:N4}", shape.Name, tensionKoeff, vacancyCount);
                        shape.BorderWidth = 2;
                        shape.TextVisibility = TextVisibility.Shown;

                        Symbol symbol = new Symbol();
                        symbol.Name = shape.Name + map.Symbols.Count;
                        symbol.ParentShape = shape.Name;
                        symbol["vacancyCount"] = totalCount == 0 ? 0 : vacancyCount / totalCount * 100;
                        symbol["redundantCount"] = totalCount == 0 ? 0 : redundantCount / totalCount * 100;
                        symbol.Offset.Y = -35;
                        symbol.MarkerStyle = MarkerStyle.Circle;
                        map.Symbols.Add(symbol);

                    }
                }
            }
        }

        protected void DundasMap2_PostPaint(object sender, MapPaintEventArgs e)
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

        #region Гриды (оба)

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            dtGrid1 = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0007_v_grid1");
            DataProvidersFactory.SpareMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid1);

            for (int i = 0; i < dtGrid1.Rows.Count; i++)
            {
                if (dtGrid1.Rows[i][0].ToString().Contains("Уровень") && dtGrid1.Rows[i][0].ToString().Contains("Прирост"))
                {
                    dtGrid1.Rows[i][0] = "ранг по СФО";
                }
                else
                {
                    if (dtGrid1.Rows[i][1] == DBNull.Value && i > 0 && dtGrid1.Rows[i - 1][1] != DBNull.Value)
                    {
                        dtGrid1.Rows[i][1] = dtGrid1.Rows[i - 1][1];
                    }
                    string paramName = dtGrid1.Rows[i][0].ToString().Split(';')[0];
                    string unit = CRHelper.ToLowerFirstSymbol(dtGrid1.Rows[i][1].ToString());
                    if (paramName.Contains("Уровень") && paramName.Contains("безработицы"))
                    {
                        dtGrid1.Rows[i][0] = String.Format("{0}", paramName);
                    }
                    else
                    {
                        dtGrid1.Rows[i][0] = String.Format("{0}, {1}", paramName, unit);
                    }
                }
            }
            dtGrid1.Columns.RemoveAt(1);
            UltraWebGrid1.DataSource = dtGrid1;
        }

        protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            dtGrid2 = new DataTable();
            string query = DataProvider.GetQueryText("STAT_0001_0007_v_grid2");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGrid2);

            for (int i = 0; i < dtGrid2.Rows.Count; i++)
            {
                if ((dtGrid2.Rows[i][0].ToString().Contains("Уровень общей безработицы по методологии МОТ")) && dtGrid2.Rows[i][0].ToString().Contains("Прирост"))
                {
                    dtGrid2.Rows[i][0] = "ранг по СФО";
                }
                else
                {
                    if (dtGrid2.Rows[i][1] == DBNull.Value && i > 0 && dtGrid2.Rows[i - 1][1] != DBNull.Value)
                    {
                        dtGrid2.Rows[i][1] = dtGrid2.Rows[i - 1][1];
                    }
                    string paramName = dtGrid2.Rows[i][0].ToString().Split(';')[0];
                    string unit = CRHelper.ToLowerFirstSymbol(dtGrid2.Rows[i][1].ToString());
                    if (paramName.Contains("Уровень") && paramName.Contains("безработицы"))
                    {
                        dtGrid2.Rows[i][0] = String.Format("{0}", paramName);
                    }
                    else
                    {
                        dtGrid2.Rows[i][0] = String.Format("{0}, {1}", paramName, unit);
                    }
                }
            }
            dtGrid2.Columns.RemoveAt(1);
            UltraWebGrid2.DataSource = dtGrid2;
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.AllowSortingDefault = AllowSorting.No;

            e.Layout.Bands[0].Columns[0].CellStyle.Font.Size = FontUnit.Parse("14px");
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.ForeColor = Color.FromArgb(209, 209, 209);

            e.Layout.Bands[0].Columns[0].CellStyle.VerticalAlign = VerticalAlign.Top;
           
            e.Layout.Bands[0].Columns[0].MergeCells = true;

            e.Layout.Bands[0].Columns[0].Width = 188;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].Width = 80;
                e.Layout.Bands[0].Columns[i].Header.Caption =
                        RegionsNamingHelper.ShortRegionsNames[e.Layout.Bands[0].Columns[i].Header.Caption.Trim(' ')];
                e.Layout.Bands[0].Columns[i].Header.Style.Font.Size = FontUnit.Parse("14px");
                e.Layout.Bands[0].Columns[i].CellStyle.VerticalAlign = VerticalAlign.Top;
            }
        }

        private void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Index % 3 == 0 && e.Row.Cells[i].Value != null)
                {
                    if (e.Row.Cells[0].Value.ToString().StartsWith("Уровень регистрируемой безработицы, % от численности экономически активного населения"))
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N3}%", e.Row.Cells[i].Value);
                        e.Row.Cells[i].Title = String.Format("{0},\n{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                    }
                    else if (e.Row.Cells[0].Value.ToString().StartsWith("Уровень напряженности на рынке труда"))
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N2}", e.Row.Cells[i].Value);
                        e.Row.Cells[i].Title = String.Format("{0},\n{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                    }
                    else if (e.Row.Cells[0].Value.ToString().StartsWith("Уровень общей безработицы по методологии МОТ"))
                    {
                        e.Row.Cells[i].Value = String.Format("{0:P2}", e.Row.Cells[i].Value);
                        e.Row.Cells[i].Title = String.Format("{0},\n{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                    }
                    else if (e.Row.Cells[0].Value.ToString().StartsWith("Численность экономически активного населения "))
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
                        e.Row.Cells[i].Title = String.Format("{0},\n{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
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
                            e.Row.Cells[i].Title = String.Format("{0},\n{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                        }
                        else
                        {
                            e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.BackgroundImage = "../../../images/CornerGreen.gif";
                            e.Row.Cells[i].Value = String.Format("{0:P2}", value);
                            e.Row.Cells[i].Title = String.Format("{0},\n{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                        }
                    }
                    e.Row.Band.Grid.Rows[e.Row.Index - 1].Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: right top;";
                    if (e.Row.Band.Grid == UltraWebGrid1)
                    {
                        if (e.Row.Cells[0].Value.ToString().Contains("аналогичный период прошлого года"))
                        {
                            e.Row.Cells[i].Title = String.Format("Темп прироста к {0:dd.MM.yyyy}", oldPrevDate);
                        }
                        else
                        {
                            e.Row.Cells[i].Title = String.Format("Темп прироста к {0:dd.MM.yyyy}", prevDate);
                        }
                    }
                    else
                    {
                        e.Row.Cells[i].Title = String.Format("Темп прироста к {0:dd.MM.yyyy}", prevMonthDate);
                    }
                }

                if (e.Row.Index % 3 == 2 && e.Row.Cells[i].Value != null)
                {
                    if (e.Row.Cells[0].Value.ToString() == "Сумма задолженности по выплате заработной платы (млн.руб.)")
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N3}", e.Row.Cells[i].Value);
                        e.Row.Cells[i].Title = String.Format("{0},\n{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                    }
                    
                    else
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N0}", e.Row.Cells[i].Value);
                        e.Row.Cells[i].Title = String.Format("{0},\n{1}", e.Row.Cells[i].Value, e.Row.Cells[0].Value);
                    }
                    if (e.Row.Cells[0].Value.ToString() != "ранг по СФО")
                    {
                        if (e.Row.Band.Grid == UltraWebGrid1)
                        {
                            if (e.Row.Cells[0].Value.ToString().Contains("аналогичный период прошлого года"))
                            {
                                e.Row.Cells[i].Title = String.Format("Прирост к {0:dd.MM.yyyy}", oldPrevDate);
                            }
                            else
                            {
                                e.Row.Cells[i].Title = String.Format("Прирост к {0:dd.MM.yyyy}", prevDate);
                            }
                        }
                        else
                        {
                            e.Row.Cells[i].Title = String.Format("Прирост к {0:dd.MM.yyyy}", prevMonthDate);
                        }
                    }
                }

                e.Row.Cells[i].Style.Padding.Right = 2;

                if (e.Row.Cells[0].Value.ToString() == "ранг по СФО")
                {
                    if (i < e.Row.Cells.Count - 1)
                    {
                        e.Row.Band.Grid.Rows[e.Row.Index - 2].Cells[0].Style.BorderDetails.WidthBottom = 0;
                        e.Row.Style.BorderDetails.WidthTop = 0;
                        e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Right;

                        if (e.Row.Cells[i].ToString() == e.Row.Cells[e.Row.Cells.Count - 1].ToString())
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "../../../images/StarGray.png";
                            e.Row.Cells[i].Style.CustomRules =
                                "background-repeat: no-repeat; background-position: 20px top;";
                        }
                        else if (e.Row.Cells[i].ToString() == "1")
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "../../../images/StarYellow.png";
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

        }

        #endregion

        private static Double GetDoubleDTValue(DataTable dt, string columnName)
        {
            return GetDoubleDTValue(dt, columnName, 0);
        }

        private static Double GetDoubleDTValue(DataTable dt, string columnName, double defaultValue)
        {
            if (dt.Rows[0][columnName] != DBNull.Value && dt.Rows[0][columnName].ToString() != String.Empty)
            {
                return Convert.ToDouble(dt.Rows[0][columnName].ToString());
            }
            return defaultValue;
        }
    }
}
