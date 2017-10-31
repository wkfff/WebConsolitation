using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;

namespace Krista.FM.Server.Dashboards.reports.SB_002
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt;
        private DataTable gridHintDt;
        private DataTable chartDt;
        private DataTable mapDt;

        private DataTable bankTerritory;
        private DataTable investmentProjectGoal;
        private DataTable sberbankFacility;
        private DataTable activityEvaluation;
        private DataTable priority;
        private DataTable facilityEvaluation;

        private List<string> bankList;

        private Dictionary<string, int> bankDicitionary;
        private Dictionary<string, int> goalDicitionary;

        private Dictionary<string, string> goalDescriptionDictionary;
        private Dictionary<string, string> goalPriorityDictionary;
        private Dictionary<string, string> regionBankDicitionary;

        
        private string federalSubjectColumn = "federal-subject";

        private string communicativeEvaluationColumn = "communicative-evaluation";
        private string agreementColumnColumn = "agreement";
        private string progressStrategyColumn = "progress-strategy";
        private string dealStrategyColumn = "deal-strategy";
        private string limitPolicyColumn = "limit-policy";
        private string budgetaryPolicyColumn = "budgetary-policy";

        private string commuticativeCharacreristicColumn = "commuticative-characteristic";
        private string agreementCharacteristicColumn = "agreement-characteristic";
        private string progressStrategyCharacteristicColumn = "progress-strategy-characteristic";
        private string dealStrategyCharacteristicColumn = "deal-strategy-characteristic";
        private string limitPolicyCharacteristicColumn = "limit-policy-characteristic";
        private string budgetaryPolicyCharacteristicColumn = "budgetary-policy-characteristic";

        private string goadClumn = "goal";
        private string descriptionColumn = "description";

        string bankColumn = "bank";
        string levelColumn = "level";

        #endregion

        public bool IsAllBankSelected
        {
            get { return ComboBank.SelectedNode.Level == 0; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 190);

            #region Настройка грида

            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.7 - 235);
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.AutoHeightRowLimit = 15;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow += new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            #region Настройка диаграммы

            PieChartBrick.Width = 300;
            PieChartBrick.Height = 300;

            PieChartBrick.DataFormatString = "P2";
            PieChartBrick.TooltipFormatString = "<ITEM_LABEL> для <DATA_VALUE:N0> субъектов (<PERCENT_VALUE:N2>%)";
            PieChartBrick.Legend.Visible = false;
            PieChartBrick.ColorModel = ChartColorModel.ExtendedFixedColors;
            PieChartBrick.SwapRowAndColumns = true;
            PieChartBrick.Chart.PieChart.Labels.Font = new Font("Verdana", 8);

            PieChartBrick.Legend.Visible = true;
            PieChartBrick.Legend.SpanPercentage = 23;
            PieChartBrick.Legend.Location = LegendLocation.Bottom;

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            LoadXmlData();

            if (!Page.IsPostBack)
            {
                FillComboBank();
                ComboBank.Title = "Тербанки";
                ComboBank.Width = 260;
                ComboBank.MultiSelect = false;
                ComboBank.ParentSelect = true;
                ComboBank.FillDictionaryValues(bankDicitionary);
                ComboBank.SetСheckedState("Все тербанки", true);

                FillComboGoal();
                ComboGoal.Title = "Направление";
                ComboGoal.Width = 260;
                ComboGoal.MultiSelect = false;
                ComboGoal.ParentSelect = true;
                ComboGoal.AutoPostBack = true;
                ComboGoal.FillDictionaryValues(goalDicitionary);
                ComboGoal.SetСheckedState("Малый бизнес", true);
            }

            PageTitle.Text = "Анализ текущей деятельности";
            PageSubTitle.Text = "Анализ коммуникативной активности, заключенных соглашений, существующей стратегии и приоритетных направлений 5+ ";
            Page.Title = PageTitle.Text;

            BroadCast.Text = String.Format("Главная » Территориальный банк » <a href='../../reports/sb_004/Default.aspx'>Планирование и текущая деятельность</a> » Анализ текущей деятельности");
            BroadCast.Font.Italic = true;

            mapElementCaption.Text = String.Format("{0} ({1})", ComboGoal.SelectedValue, ComboBank.SelectedValue);
            gridElementCaption.Text = "Анализ текущей деятельности";

            GoalImage.ImageUrl = String.Format("../../images/Sberbank/{0}.png", ComboGoal.SelectedValue);

            GridDataBind();

            string mapPath = Server.MapPath(string.Format("../../maps/Сбербанк/{0}/{0}.shp", IsAllBankSelected ? "РФ по тербанкам" : ComboBank.SelectedValue));
            
            DundasMap.Shapes.Clear();
            DundasMap.LoadFromShapeFile(mapPath, "NAME", true);
            
            if (IsAllBankSelected)
            {
                HeraldTr.Visible = false;
                ChartTd.Visible = false;
                PriorityTotalLabel.Visible = false;
                //ChartDataBind();

                SetRFMapSettings();
                FillRFMapData();
            }
            else
            {
                SetMapSettings();
                FillMapData();

                PriorityTotalLabel.Visible = true;
                HeraldTr.Visible = true;
                ChartTd.Visible = false;
                GenerateHeraldIcons();
            }
        }

        #region Обработчики грида
        
        private void GridDataBind()
        {
            // создаем грид для значений
            gridDt = new DataTable();
            DataColumn column = new DataColumn(federalSubjectColumn, typeof(String));
            gridDt.Columns.Add(column);
            column = new DataColumn(communicativeEvaluationColumn, typeof(String));
            gridDt.Columns.Add(column);
            column = new DataColumn(agreementColumnColumn, typeof(String));
            gridDt.Columns.Add(column);
            column = new DataColumn(progressStrategyColumn, typeof(String));
            gridDt.Columns.Add(column);
            column = new DataColumn(dealStrategyColumn, typeof(String));
            gridDt.Columns.Add(column);
            column = new DataColumn(limitPolicyColumn, typeof(String));
            gridDt.Columns.Add(column);
            column = new DataColumn(budgetaryPolicyColumn, typeof(String));
            gridDt.Columns.Add(column);

            foreach (string goal in goalDescriptionDictionary.Keys)
            {
                column = new DataColumn(goal, typeof(String));
                gridDt.Columns.Add(column);
            }

            column = new DataColumn(levelColumn, typeof(String));
            gridDt.Columns.Add(column);

            // создаем грид для хинтов
            gridHintDt = new DataTable();
            column = new DataColumn(federalSubjectColumn, typeof(String));
            gridHintDt.Columns.Add(column);
            column = new DataColumn(commuticativeCharacreristicColumn, typeof(String));
            gridHintDt.Columns.Add(column);
            column = new DataColumn(agreementCharacteristicColumn, typeof(String));
            gridHintDt.Columns.Add(column);
            column = new DataColumn(progressStrategyCharacteristicColumn, typeof(String));
            gridHintDt.Columns.Add(column);
            column = new DataColumn(dealStrategyCharacteristicColumn, typeof(String));
            gridHintDt.Columns.Add(column);
            column = new DataColumn(limitPolicyCharacteristicColumn, typeof(String));
            gridHintDt.Columns.Add(column);
            column = new DataColumn(budgetaryPolicyCharacteristicColumn, typeof(String));
            gridHintDt.Columns.Add(column);

            foreach (string goal in goalDescriptionDictionary.Keys)
            {
                column = new DataColumn(goal, typeof(String));
                gridHintDt.Columns.Add(column);
            }

            // забиваем гриды строками
            if (IsAllBankSelected)
            {
                foreach (string bank in bankList)
                {
                    string sqlFilter = String.Format("[{0}] = '{1}'", bankColumn, bank);
                    DataRow[] terrRows = activityEvaluation.Select(sqlFilter);

                    // считаем среднее по регионам банка
                    AvgCalculator avgCalculator = new AvgCalculator();
                    foreach (DataRow terrRow in terrRows)
                    {
                        if (terrRow[communicativeEvaluationColumn] != DBNull.Value && terrRow[communicativeEvaluationColumn].ToString() != String.Empty)
                        {
                            int value;
                            if (Int32.TryParse(terrRow[communicativeEvaluationColumn].ToString(), out value))
                            {
                                avgCalculator.AddValue(value);
                            }
                        }
                    }

                    // добавляем строку банка в грид
                    DataRow bankRow = gridDt.NewRow();
                    bankRow[federalSubjectColumn] = bank;
                    bankRow[communicativeEvaluationColumn] = Math.Round(avgCalculator.GetAverage());
                    bankRow[levelColumn] = "1";

                    // для банка направления забиваем нулями
                    foreach (string goal in goalDescriptionDictionary.Keys)
                    {
                        bankRow[goal] = "0";
                    }

                    gridDt.Rows.Add(bankRow);
                    gridDt.AcceptChanges();

                    FillGridRows(ref gridDt, terrRows);

                    // добавляем строку банка в грид хинтов
                    bankRow = gridHintDt.NewRow();
                    bankRow[federalSubjectColumn] = bank;
                    gridHintDt.Rows.Add(bankRow);
                    gridHintDt.AcceptChanges();

                    FillGridRows(ref gridHintDt, terrRows);
                }
            }
            else
            {
                string sqlFilter = String.Format("[{0}] = '{1}'", bankColumn, ComboBank.SelectedValue);
                DataRow[] activityRows = activityEvaluation.Select(sqlFilter);

                FillGridRows(ref gridDt, activityRows);
                FillGridRows(ref gridHintDt, activityRows);
            }

            FontRowLevelRule levelRule = new FontRowLevelRule(gridDt.Columns.Count - 1);
            levelRule.AddFontLevel("1", GridBrick.BoldFont8pt);
            GridBrick.AddIndicatorRule(levelRule);

            GridBrick.AddIndicatorRule(new ImageRowValueRule("Оценка коммуникативной активности ТБ", "Отсутствует возможность", "ballRedBB.png"));
            GridBrick.AddIndicatorRule(new ImageRowValueRule("Заключение соглашения ТБ с субъектом РФ", "Не заключено", "ballRedBB.png"));
            GridBrick.AddIndicatorRule(new ImageRowValueRule("Стратегия развития субъекта РФ", "В разработке, не утверждена", "ballRedBB.png"));
            GridBrick.AddIndicatorRule(new ImageRowValueRule("Стратегия развития субъекта РФ", "Не существует", "ballRedBB.png"));
            GridBrick.AddIndicatorRule(new ImageRowValueRule("Стратегия работы ТБ с субъектом РФ", "Не существует", "ballRedBB.png"));
            GridBrick.AddIndicatorRule(new ImageRowValueRule("Стратегия работы ТБ с субъектом РФ", "Не утверждена", "ballRedBB.png"));
            GridBrick.AddIndicatorRule(new ImageRowValueRule("Лимитная политика ТБ", "Требует увеличения", "ballRedBB.png"));

            GridBrick.Grid.DataSource = gridDt;
        }

        /// <summary>
        /// Перекачка строк в грид
        /// </summary>
        /// <param name="dt">грид</param>
        /// <param name="rowCollection">набор строк</param>
        private static void FillGridRows(ref DataTable dt, DataRow[] rowCollection)
        {
            foreach (DataRow row in rowCollection)
            {
                DataRow newRow = dt.NewRow();
                for (int i = 0; i < row.ItemArray.Length; i++)
                {
                    string columnName = row.Table.Columns[i].ColumnName;

                    if (dt.Columns.Contains(columnName))
                    {
                        newRow[columnName] = row[columnName];
                    }
                }
                dt.Rows.Add(newRow);
            }
            dt.AcceptChanges();
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.RowStyleDefault.Wrap = true;
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;
                
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(GetColumnWidth(columnName));
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], GetColumnFormat(columnName));
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = GetColumnAlignment(columnName);
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;
            headerLayout.AddCell("Субъект РФ / Тербанк");
            headerLayout.AddCell("Оценка коммуникативной активности ТБ");
            headerLayout.AddCell("Заключение соглашения ТБ с субъектом РФ");

            GridHeaderCell groupCell = headerLayout.AddCell("Существующие стратегии");
            groupCell.AddCell("Стратегия развития субъекта РФ");
            groupCell.AddCell("Стратегия работы ТБ с субъектом РФ");

            groupCell = headerLayout.AddCell("Политика");
            groupCell.AddCell("Лимитная политика ТБ");
            groupCell.AddCell("Бюджетная политика субъекта РФ");

            groupCell = headerLayout.AddCell("Приоритетные программы (5+) для субъекта РФ");
            foreach (string goal in goalDescriptionDictionary.Keys)
            {
                groupCell.AddCell(goal);
            }

            headerLayout.ApplyHeaderInfo();
        }

        private void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 1; i < e.Row.Cells.Count - 1; i++)
            {
                UltraGridCell cell = e.Row.Cells[i];
                string columnName = e.Row.Band.Columns[i].Header.Caption;

                // для приоритетов значение пихаем в хинт
                if (goalDescriptionDictionary.ContainsKey(columnName))
                {
                    if (cell.Value != null)
                    {
                        if (cell.Value.ToString() == "0")
                        {
                            cell.Value = String.Empty;
                        }
                        else
                        {
                            cell.Title = cell.Value.ToString();
                            cell.Value = "Да";                            
                        }
                    }
                    else
                    {
                        cell.Value = "Нет";
                    }
                }
                else if (gridHintDt != null)
                {
                    if (e.Row.Index < gridHintDt.Rows.Count && i < gridHintDt.Columns.Count &&
                        gridHintDt.Rows[e.Row.Index][i] != DBNull.Value)
                    {
                        cell.Title = gridHintDt.Rows[e.Row.Index][i].ToString();
                    }
                }
            }
        }

        private int GetColumnWidth(string columnName)
        {
            if (columnName == communicativeEvaluationColumn)
            {
                return 170;
            }
            return 140;
        }

        private string GetColumnFormat(string columnName)
        {
            if (columnName == communicativeEvaluationColumn)
            {
                return "N0";
            }
            return String.Empty;
        }

        private HorizontalAlign GetColumnAlignment(string columnName)
        {
            if (columnName == communicativeEvaluationColumn)
            {
                return HorizontalAlign.Right;
            }
            return HorizontalAlign.Center;
        }

        #endregion

        #region Обработчики карты

        private void SetMapSettings()
        {
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("subjectName");
            DundasMap.ShapeFields["subjectName"].Type = typeof(string);
            DundasMap.ShapeFields["subjectName"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("goal");
            DundasMap.ShapeFields["goal"].Type = typeof(int);
            DundasMap.ShapeFields["goal"].UniqueIdentifier = false;
            DundasMap.ShapeFields.Add("description");
            DundasMap.ShapeFields["description"].Type = typeof(string);
            DundasMap.ShapeFields["description"].UniqueIdentifier = true;

            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Right;
            
            // добавляем легенду
            DundasMap.Legends.Clear();

            Legend legend = new Legend("MapLegend");
            legend.Dock = PanelDockStyle.Left;
            legend.DockAlignment = DockAlignment.Far;
            legend.Visible = true;
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
            legend.AutoFitText = true;
            legend.Title = "Приоритеты";
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(legend);

            LegendItem item = new LegendItem();
            item.Text = "Является приоритным";
            item.Color = Color.Green;
            DundasMap.Legends["MapLegend"].Items.Add(item);

            item = new LegendItem();
            item.Text = "Не является приоритетным";
            item.Color = Color.Red;
            DundasMap.Legends["MapLegend"].Items.Add(item);
        }

        private void FillMapData()
        {
            foreach (DataRow row in gridDt.Rows)
            {
                string subjectName = row[federalSubjectColumn].ToString();
                string description = row[ComboGoal.SelectedValue].ToString();

                string key = String.Format("{0};{1}", ComboGoal.SelectedValue, subjectName);

                ArrayList shapeList = FindMapShape(DundasMap, subjectName);
                foreach (Shape shape in shapeList)
                {
                    shape.Visible = true;

                    shape.BorderWidth = 1;
                    shape.BorderStyle = MapDashStyle.Solid;
                    shape.BorderColor = Color.Black;

                    shape["subjectName"] = subjectName;
                    shape["description"] = description;
                    shape.TextVisibility = TextVisibility.Shown;
                    

                    if (goalPriorityDictionary.ContainsKey(key))
                    {
                        shape.Color = Color.Green;
                        shape.ToolTip = String.Format("{0}\n{1}\n{2}", subjectName, ComboGoal.SelectedValue, description);
                    }
                    else
                    {
                        shape.Color = Color.Red;
                        shape.ToolTip = String.Format("{0}\n{1}\nНе приоритетно", subjectName, ComboGoal.SelectedValue);
                    }

                    shape.Text = subjectName.Replace(" ", "\n");
                }
            }
        }

        private void SetRFMapSettings()
        {
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("priorityPercent");
            DundasMap.ShapeFields["priorityPercent"].Type = typeof(double);
            DundasMap.ShapeFields["priorityPercent"].UniqueIdentifier = false;

            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.NavigationPanel.Dock = PanelDockStyle.Right;
            DundasMap.ZoomPanel.Dock = PanelDockStyle.Right;

            // добавляем легенду
            DundasMap.Legends.Clear();

            Legend legend = new Legend("MapLegend");
            legend.Dock = PanelDockStyle.Left;
            legend.DockAlignment = DockAlignment.Far;
            legend.Visible = true;
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
            legend.AutoFitText = true;
            legend.Title = "Доля приоритетных субъектов";
            legend.AutoFitMinFontSize = 7;
            DundasMap.Legends.Add(legend);

            ShapeRule rule = new ShapeRule();
            rule.Name = "IndicatorRule";
            rule.Category = String.Empty;
            rule.ShapeField = "priorityPercent";
            rule.DataGrouping = DataGrouping.Optimal;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Green;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "MapLegend";
            rule.LegendText = "#FROMVALUE{N2}% - #TOVALUE{N2}%";
            DundasMap.ShapeRules.Add(rule);
        }

        private void FillRFMapData()
        {
            foreach (string bank in bankList)
            {
                string sqlFilter = String.Format("[{0}] = '{1}'", bankColumn, bank);
                DataRow[] terrRows = activityEvaluation.Select(sqlFilter);

                int priorityRegionCount = 0;
                foreach (DataRow terrRow in terrRows)
                {
                    string subject = terrRow[federalSubjectColumn].ToString().TrimEnd(' ');
                    string key = String.Format("{0};{1}", ComboGoal.SelectedValue, subject);

                    if (goalPriorityDictionary.ContainsKey(key))
                    {
                        priorityRegionCount++;
                    }
                }
                
                double priorityPercent = 100 * priorityRegionCount / terrRows.Length;

                ArrayList shapeList = FindMapShape(DundasMap, bank);
                foreach (Shape shape in shapeList)
                {
                    shape.Visible = true;

                    shape.BorderWidth = 1;
                    shape.BorderStyle = MapDashStyle.Solid;
                    shape.BorderColor = Color.Black;
                    
                    shape["priorityPercent"] = priorityPercent;
                    shape.TextVisibility = TextVisibility.Shown;

                    shape.ToolTip = String.Format("{0}\n{1:N2}% ({2}/{3})", bank, priorityPercent, priorityRegionCount, terrRows.Length);
                    shape.Text = String.Format("{0}\n({1}/{2})", bank, priorityRegionCount, terrRows.Length);
                }
            }
        }

        public static ArrayList FindMapShape(MapControl map, string patternValue)
        {
            patternValue = patternValue.TrimEnd(' ');
            patternValue = patternValue.Replace("Чувашская Республика", "Республика Чувашия");
            patternValue = patternValue.Replace("Еврейская автономная область", "Еврейская АО");
            patternValue = patternValue.Replace("Ханты-Мансийский автономный округ - Югра", "Ханты-Мансийский АО");
            patternValue = patternValue.Replace("Ямало-Ненецкий автономный округ", "Ямало-Ненецкий АО");
            patternValue = patternValue.Replace("Удмуртская Республика", "Республика Удмуртия");
            patternValue = patternValue.Replace("Ненецкий автономный округ", "Ненецкий АО");
            patternValue = patternValue.Replace("Чукотский автономный округ", "Чукотский АО");
            patternValue = patternValue.Replace("Чеченская Республика", "Республика Чечня");
            patternValue = patternValue.Replace("Карачаево-Черкесская Республика", "Республика Карачаево-Черкессия");
            patternValue = patternValue.Replace("Кабардино-Балкарская Республика", "Республика Кабардино-Балкария");
            patternValue = patternValue.Replace("Республика Северная Осетия -Алания", "Республика Северная Осетия");

            ArrayList shapeList = new ArrayList();
            foreach (Shape shape in map.Shapes)
            {
                if (shape.Name == patternValue)
                {
                    shapeList.Add(shape);
                }
            }

            return shapeList;
        }

        #endregion

        #region Обработчики диаграммы

        private void ChartDataBind()
        {
            DataRow[] terrRows = bankTerritory.Select();

            int priorityRegionCount = 0;
            string priorityRegionList = String.Empty;
            foreach (DataRow row in terrRows)
            {
                string subject = row[federalSubjectColumn].ToString().TrimEnd(' ');
                string key = String.Format("{0};{1}", ComboGoal.SelectedValue, subject);

                if (goalPriorityDictionary.ContainsKey(key))
                {
                    string description = goalPriorityDictionary[key];
                    priorityRegionList += String.Format("&nbsp;<img src='{1}' height='20px' title='{2}'/>&nbsp;{0},", subject, GetRegionImg(subject, "../.."), description);
                    priorityRegionCount++;
                }
            }

            chartDt = new DataTable();
            DataColumn column = new DataColumn("Является приоритетным", typeof(double));
            chartDt.Columns.Add(column);
            column = new DataColumn("Не является приоритетным", typeof(double));
            chartDt.Columns.Add(column);

            int regionCount = terrRows.Length;

            DataRow newRow = chartDt.NewRow();
            if (regionCount != 0)
            {
                newRow[0] = priorityRegionCount;
                newRow[1] = regionCount - priorityRegionCount;
            }
            chartDt.Rows.Add(newRow);
            chartDt.AcceptChanges();

            PieChartBrick.DataTable = chartDt;
            
            PriorityTotalLabel.Text = String.Format("<b>Приоритетный для {0} {1} из {2}</b><br/>{3}", priorityRegionCount, 
                priorityRegionCount % 10 == 1 ? "субъекта" : "субъектов", regionCount, priorityRegionList.TrimEnd(','));
        }

        #endregion

        #region Лента гербов

        private void GenerateHeraldIcons()
        {
            string sqlFilter = String.Format("[{0}] = '{1}'", bankColumn, ComboBank.SelectedValue);
            DataRow[] terrRows = bankTerritory.Select(sqlFilter);

            int priorityRegionCount = 0;
            foreach (DataRow row in terrRows)
            {
                string subject = row[federalSubjectColumn].ToString().TrimEnd(' ');
                string key = String.Format("{0};{1}", ComboGoal.SelectedValue, subject);

                string description = String.Empty;
                double heraldOpacity = 0.4;
                if (goalPriorityDictionary.ContainsKey(key))
                {
                    description = goalPriorityDictionary[key];
                    heraldOpacity = 1;
                    priorityRegionCount++;
                }
                                    
                Control heraldIcon = Page.LoadControl("../../Components/HeraldIcon.ascx");
                
                ((HeraldIcon)heraldIcon).Image = GetRegionImg(subject, "~");
                ((HeraldIcon)heraldIcon).Title = subject.Replace(" ", "<br/>");
                ((HeraldIcon)heraldIcon).Description = description;
                ((HeraldIcon)heraldIcon).SetOpacity(heraldOpacity);
                ((HeraldIcon)heraldIcon).Width = "120px";

                HtmlTableCell cell = new HtmlTableCell();
                cell.Controls.Add(heraldIcon);
                
                HeraldTr.Controls.Add(cell);
            }

            PriorityTotalLabel.Text = String.Format("<b>Приоритетный для {0} {1} из {2}</b>", priorityRegionCount, priorityRegionCount % 10 == 1 ? "субъекта" : "субъектов",
                terrRows.Length);
        }

        private static string GetRegionImg(string subjectName, string prefixPath)
        {

            string subjectId = CustomParams.GetSubjectIdByName(GetRegionName(subjectName));
            return String.Format("{1}/images/Heralds/{0}.png", subjectId, prefixPath);
        }

        private static string GetRegionName(string subjectName)
        {
            subjectName = subjectName.Replace("Республика Татарстан", "Республика Татарстан (Татарстан)");
            subjectName = subjectName.Replace("Ханты-Мансийский автономный округ - Югра", "Ханты-Мансийский автономный округ");
            subjectName = subjectName.Replace("Удмуртская Республика", "Удмуртская республика");
            subjectName = subjectName.Replace("Астраханская область", "Астраханская область");
            subjectName = subjectName.Replace("Республика Северная Осетия -Алания", "Республика Северная Осетия-Алания");
            subjectName = subjectName.Replace("Чеченская Республика", "Чеченская республика");
            subjectName = subjectName.Replace("Республика Адыгея", "Республика Адыгея (Адыгея)");
            subjectName = subjectName.Replace("Чувашская Республика", "Чувашская Республика-Чувашия");

            return subjectName;
        }

        #endregion

        #region Эагрузка из XML

        public void LoadXmlData()
        {
            string dataPath = Server.MapPath("../../data/Sberbank/");
            string xmlFileName = String.Format("{0}/dataset.xml", dataPath);
            string xsdFileName = String.Format("{0}/dataset.xsd", dataPath);

            DataSet ds = new DataSet();
            ds.ReadXmlSchema(xsdFileName);

            XmlDataDocument xmlDataDocument = new XmlDataDocument(ds);
            xmlDataDocument.Load(xmlFileName);

            bankTerritory = xmlDataDocument.DataSet.Tables["bank-territory"];
            investmentProjectGoal = xmlDataDocument.DataSet.Tables["investment-project-goal"];
            sberbankFacility = xmlDataDocument.DataSet.Tables["sberbank-facility"];

            priority = GetPriority(xmlDataDocument.DataSet.Tables["priority"]);
            facilityEvaluation = xmlDataDocument.DataSet.Tables["facility-evaluation"];

            // заполняем список банков
            bankList = new List<string>();
            foreach (DataRow row in bankTerritory.Rows)
            {
                string bank = row[bankColumn].ToString().TrimEnd(' ');

                if (!bankList.Contains(bank))
                {
                    bankList.Add(bank);
                }
            }

            // заполняем словарь направлений
            goalDescriptionDictionary = new Dictionary<string, string>();
            foreach (DataRow row in priority.Rows)
            {
                string goal = row[goadClumn].ToString();
                string description = row[descriptionColumn].ToString();

                if (!goalDescriptionDictionary.ContainsKey(goal))
                {
                    goalDescriptionDictionary.Add(goal, description);
                }
            }

            // заполняем словарь приоритетов направлений
            goalPriorityDictionary = new Dictionary<string, string>();
            foreach (DataRow row in priority.Rows)
            {
                string goal = row[goadClumn].ToString();
                string subject = row[federalSubjectColumn].ToString().TrimEnd(' ');
                string description = row[descriptionColumn].ToString();
                string key = String.Format("{0};{1}", goal, subject);

                if (!goalPriorityDictionary.ContainsKey(key))
                {
                    goalPriorityDictionary.Add(key, description);
                }
            }


            activityEvaluation = GetActivityEvaluation(xmlDataDocument.DataSet.Tables["activity-evaluation"]);
        }

        private DataTable GetActivityEvaluation(DataTable xmlDt)
        {
            DataTable dt = xmlDt.Copy();

            // дописываем колонку с банками
            DataColumn newBankColumn = new DataColumn(bankColumn, typeof(String));
            dt.Columns.Add(newBankColumn);

            regionBankDicitionary = new Dictionary<string, string>();
            foreach (DataRow row in bankTerritory.Rows)
            {
                string subject = row[federalSubjectColumn].ToString();
                string bank = row[bankColumn].ToString();
                if (!regionBankDicitionary.ContainsKey(subject))
                {
                    regionBankDicitionary.Add(subject, bank);
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                string subject = row[federalSubjectColumn].ToString();
                if (regionBankDicitionary.ContainsKey(subject))
                {
                    row[bankColumn] = regionBankDicitionary[subject];
                }
            }

            // дописываем колонки с проритетами
            foreach (string goal in goalDescriptionDictionary.Keys)
            {
                DataColumn column = new DataColumn(goal, typeof(String));
                dt.Columns.Add(column);
            }

            foreach (DataRow row in dt.Rows)
            {
                string subject = row[federalSubjectColumn].ToString();
                foreach (string goal in goalDescriptionDictionary.Keys)
                {
                    if (dt.Columns.Contains(goal))
                    {
                        string key = String.Format("{0};{1}", goal, subject);
                        if (goalPriorityDictionary.ContainsKey(key))
                        {
                            row[goal] = goalPriorityDictionary[key];
                        }
                    }
                }
            }

            return dt;
        }

        private DataTable GetPriority(DataTable xmlDt)
        {
            DataTable dt = xmlDt.Copy();

            // дописываем колонку с банками
            DataColumn newBankColumn = new DataColumn(bankColumn, typeof(String));
            dt.Columns.Add(newBankColumn);

            regionBankDicitionary = new Dictionary<string, string>();
            foreach (DataRow row in bankTerritory.Rows)
            {
                string subject = row[federalSubjectColumn].ToString();
                string bank = row[bankColumn].ToString();
                if (!regionBankDicitionary.ContainsKey(subject))
                {
                    regionBankDicitionary.Add(subject, bank);
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                string subject = row[federalSubjectColumn].ToString();
                if (regionBankDicitionary.ContainsKey(subject))
                {
                    row[bankColumn] = regionBankDicitionary[subject];
                }
            }
            
            return dt;
        }

        private void FillComboBank()
        {
            bankDicitionary = new Dictionary<string, int>();
            bankDicitionary.Add("Все тербанки", 0);
            foreach (string bank in bankList)
            {
                if (!bankDicitionary.ContainsKey(bank))
                {
                    bankDicitionary.Add(bank, 1);
                }
            }
        }

        private void FillComboGoal()
        {
            goalDicitionary = new Dictionary<string, int>();
            foreach (string goal in goalDescriptionDictionary.Keys)
            {
                if (!goalDicitionary.ContainsKey(goal))
                {
                    goalDicitionary.Add(goal, 0);
                }
            }
        }


        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("Карта");
            ReportExcelExporter1.Export(DundasMap, mapElementCaption.Text, sheet2, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);

            ISection section2 = report.AddSection();
            DundasMap.Width = Convert.ToInt32(DundasMap.Width.Value * 0.8);
            ReportPDFExporter1.Export(DundasMap, mapElementCaption.Text, section2);
        }

        #endregion
    }

    public class AvgCalculator
    {
        private double summary = 0;
        private int count = 0;

        public void AddValue(double value)
        {
            summary += value;
            if (value != 0)
            {
                count++;
            }
        }

        public double GetAverage()
        {
            if (count != 0)
                return summary / count;
            return 0;
        }

        public void Reset()
        {
            summary = 0;
            count = 0;
        }
    }
}
