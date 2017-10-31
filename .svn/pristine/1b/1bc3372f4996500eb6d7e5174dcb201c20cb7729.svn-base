using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using ContentAlignment = System.Drawing.ContentAlignment;
using SerializationFormat=Dundas.Maps.WebControl.SerializationFormat;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0003_Saratov
{
    public partial class DefaultCompare : CustomReportPage
    {
        #region Поля

        private DataTable mapDt = new DataTable();
        private DataTable gridDt;
        private int firstYear = 2000;
        private string populationDate;

        private DateTime currentDate;

        #endregion

        #region Свойства

        /// <summary>
        /// Выбраны ли все федеральные округа
        /// </summary>
        public bool AllFO
        {
            get { return ComboFO.SelectedIndex == 0; }
        }

        /// <summary>
        /// Скрывать ранги доли для итогового элемента расходов
        /// </summary>
        public bool PercentRankHidden
        {
            get { return ComboFKR.SelectedValue == "Расходы бюджета - ИТОГО"; }
        }

        #endregion

        #region Параметры запроса

        private CustomParam percentRankHidden;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (percentRankHidden == null)
            {
                percentRankHidden = UserParams.CustomParam("percent_rank_hidden");
            }

            #endregion

            #region Настройка грида

            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.7 - 225);
            GridBrick.Width = CustomReportConst.minScreenWidth - 12;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.AutoHeightRowLimit = 15;

            #endregion

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 12);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.7);

            CrossLink1.Text = "Диаграмма&nbsp;распределения&nbsp;субъектов&nbsp;по&nbsp;бюджетным&nbsp;расходам&nbsp;на&nbsp;душу&nbsp;населения";
            CrossLink1.NavigateUrl = "~/reports/FK_0001_0003/DefaultAllocation.aspx";
            CrossLink2.Text = "Расходы&nbsp;субъекта&nbsp;РФ&nbsp;подробнее";
            CrossLink2.NavigateUrl = "~/reports/FK_0001_0001/DefaultDetail.aspx";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            GridSearch1.LinkedGridId = GridBrick.Grid.ClientID;
        }

        private void SetupMap()
        {
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;
            DundasMap.NavigationPanel.Visible = true;

            // добавляем легенду
            Legend legend = new Legend("CompleteLegend");
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
            legend.Title = "Бюджетные расходы\nна душу населения,\nруб./чел.";
            legend.TitleSeparator = LegendSeparatorType.GradientLine;
            legend.AutoFitMinFontSize = 7;
            legend.EquallySpacedItems = false;
            legend.InterlacedRows = false;
            legend.InterlacedRowsColor = Color.Empty;
            legend.TextWrapThreshold = 25;
            DundasMap.Legends.Clear();
            DundasMap.Legends.Add(legend);


            // добавляем поля
            DundasMap.ShapeFields.Clear();
            DundasMap.ShapeFields.Add("Name");
            DundasMap.ShapeFields["Name"].Type = typeof(string);
            DundasMap.ShapeFields["Name"].UniqueIdentifier = true;
            DundasMap.ShapeFields.Add("Complete");
            DundasMap.ShapeFields["Complete"].Type = typeof(double);
            DundasMap.ShapeFields["Complete"].UniqueIdentifier = false;

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "Complete";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 7;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Green;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            DundasMap.ShapeRules.Add(rule);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                currentDate = CubeInfoHelper.FkMonthReportOutcomesInfo.LastDate;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, currentDate.Year));
                ComboYear.SetСheckedState(currentDate.Year.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(CRHelper.RusMonth(currentDate.Month), true);

                ComboFO.Title = "ФО";
                ComboFO.Width = 300;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
                ComboFO.SetСheckedState("Все федеральные округа", true);

                ComboFKR.Width = 420;
                ComboFKR.Title = "РзПр";
                ComboFKR.MultiSelect = false;
                ComboFKR.ParentSelect = true;
                ComboFKR.FillDictionaryValues(CustomMultiComboDataHelper.FillFKRNames(
                                                  DataDictionariesHelper.OutcomesFKRTypes,
                                                  DataDictionariesHelper.OutcomesFKRLevels));
                ComboFKR.SetСheckedState("Расходы бюджета - ИТОГО", true);

                UserParams.Filter.Value = ComboFO.SelectedValue;
                UserParams.KDGroup.Value = DataDictionariesHelper.OutcomesFKRTypes[ComboFKR.SelectedValue];
                UserParams.Subject.Value = String.Empty;
            }

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex + 1, 1);

            Page.Title = String.Format("Исполнение расходов ({0})", ComboFO.SelectedIndex == 0
                                                                        ? "РФ"
                                                                        :
                                                                    RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
            Label1.Text = Page.Title;
            Label2.Text =
                String.Format(
                    "Исполнение консолидированных бюджетов субъектов РФ по расходам ({3}) за {0}&nbsp;{1}&nbsp;{2}&nbsp;года",
                    currentDate.Month, CRHelper.RusManyMonthGenitive(currentDate.Month), currentDate.Year, ComboFKR.SelectedValue);

            // заполняем карту формами
            string regionStr = (ComboFO.SelectedIndex == 0) ? "Российская Федерация" : ComboFO.SelectedValue;
            DundasMap.Shapes.Clear();

            //DundasMap.LoadFromShapeFile(Server.MapPath(String.Format("../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);

            DundasMap.Serializer.Format = SerializationFormat.Binary;
            DundasMap.Serializer.Load(
                (Server.MapPath(String.Format("../../maps/{0}/{0}.bin", RegionsNamingHelper.ShortName(regionStr)))));

            SetupMap();

            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.KDGroup.Value = DataDictionariesHelper.OutcomesFKRTypes[ComboFKR.SelectedValue];

            DundasMap.Legends[0].LegendStyle = LegendStyle.Table;
            DundasMap.Legends[0].Items.Add("LegendItem");
            DundasMap.Legends[0].Items["LegendItem"].Cells.Add(LegendCellType.Text, "Cell1", ContentAlignment.TopCenter);
            DundasMap.Legends[0].Items["LegendItem"].Cells.Add(LegendCellType.Text, "Cell2", ContentAlignment.TopCenter);
            DundasMap.Legends[0].Items["LegendItem"].Cells["Cell1"].CellType = LegendCellType.Text;
            DundasMap.Legends[0].Items["LegendItem"].Cells["Cell1"].CellSpan = 2;
            DundasMap.Legends[0].Items["LegendItem"].Cells["Cell1"].Text = ComboFKR.SelectedValue;
            DundasMap.Legends[0].Items["LegendItem"].Cells["Cell2"].CellType = LegendCellType.Text;

            populationDate = DataDictionariesHelper.GetFederalPopulationDate();

            percentRankHidden.Value = PercentRankHidden.ToString();

            GridDataBind();

            // заполняем карту данными
            FillMapData();
        }

        #region Обработчики карты

        /// <summary>
        /// Поиск формы карты
        /// </summary>
        /// <param name="map">карта</param>
        /// <param name="patternValue">искомое имя формы</param>
        /// <param name="searchFO">true, если ищем ФО</param>
        /// <returns>найденная форма</returns>
        public static Shape FindMapShape(MapControl map, string patternValue, bool searchFO)
        {
            string subject = String.Empty;
            bool isRepublic = patternValue.Contains("Республика");
            bool isTown = patternValue.Contains("г.");
            string[] subjects = patternValue.Split(' ');
            if (subjects.Length > 1)
            {
                // пока такой глупый способ сопоставления имен субъектов
                switch (subjects[0])
                {
                    case "Чеченская":
                        {
                            subject = "Чечня";
                            break;
                        }
                    case "Карачаево-Черкесская":
                        {
                            subject = "Карачаево-Черкессия";
                            break;
                        }
                    case "Кабардино-Балкарская":
                        {
                            subject = "Кабардино-Балкария";
                            break;
                        }
                    case "Удмуртская":
                        {
                            subject = "Удмуртия";
                            break;
                        }
                    case "Чувашская":
                        {
                            subject = "Чувашия";
                            break;
                        }
                    default:
                        {
                            subject = (isRepublic || isTown) ? subjects[1] : subjects[0];
                            break;
                        }
                }
            }

            ArrayList shapeList = map.Shapes.Find(subject, true, false);
            if (shapeList.Count > 0)
            {
                return (Shape)shapeList[0];
            }
            return null;
        }

        public void FillMapData()
        {
            string query = DataProvider.GetQueryText("FK_0001_0003_Saratov_compare_map");
            mapDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", mapDt);

            const string completePercentColumnName = "Коэффициент бюджетной обеспеченности населения";

            foreach (DataRow row in mapDt.Rows)
            {
                // заполняем карту данными
                if (row[0] != DBNull.Value && row[0].ToString() != String.Empty &&
                    row[completePercentColumnName] != DBNull.Value && row[completePercentColumnName].ToString() != String.Empty)
                {
                    string subject = row[0].ToString();
                    if (AllFO && RegionsNamingHelper.IsFO(subject) || !AllFO && RegionsNamingHelper.IsSubject(subject))
                    {
                        Shape shape = FindMapShape(DundasMap, subject, AllFO);
                        if (shape != null)
                        {
                            shape["Name"] = subject.Replace("федеральный округ", "ФО");
                            shape["Complete"] = Convert.ToDouble(row[completePercentColumnName]);
                            shape.ToolTip = "#NAME #COMPLETE{N3} руб./чел.";
                            shape.TextVisibility = TextVisibility.Shown;
                        }
                    }
                }
            }
        }

        #endregion

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FK_0001_0003_Saratov_compare_Grid");
            gridDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", gridDt);

            if (gridDt.Rows.Count > 0)
            {
                UserParams.Filter.Value = ComboFO.SelectedValue;
                if (ComboFO.SelectedIndex != 0)
                {
                    GridBrick.DataTable = CRHelper.SetDataTableFilter(gridDt, "ФО", RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
                }
                else
                {
                    GridBrick.DataTable = gridDt;
                }
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 2].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 3].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 4].Hidden = true;
            e.Layout.Bands[0].Columns[columnCount - 5].Hidden = true;

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("Субъект");
            headerLayout.AddCell("ФО", "Федеральный округ, которому принадлежит субъект");
            headerLayout.AddCell("Исполнено, млн.руб", "Фактическое исполнение нарастающим итогом с начала года");
            headerLayout.AddCell("Назначено, млн.руб", "План на год");
            headerLayout.AddCell("Исполнено %", "Процент выполнения назначений. Оценка равномерности исполнения (1/12 годового плана в месяц)");
            headerLayout.AddCell("Темп роста", "Темп роста к прошлому году");
            headerLayout.AddCell(String.Format("Численность постоянного населения {0}, тыс.чел.", populationDate), "Численность постоянного населения");
            headerLayout.AddCell("Бюджетные расходы на душу населения,\n руб./чел.", "Бюджетные расходы, рублей на душу населения");
            headerLayout.AddCell("Ранг бюдж. расходов на душу населения ФО", "Ранг (место) субъекта по бюджетным расходам на душу населения среди субъектов его федерального округа");
            headerLayout.AddCell("Ранг бюдж. расходов на душу населения РФ", "Ранг (место) по бюджетным расходам на душу населения среди всех субъектов");
            headerLayout.AddCell("Доля", "Доля расхода в общей сумме расходов субъекта");

            if (!PercentRankHidden)
            {
                headerLayout.AddCell("Ранг доля ФО", "Ранг (место) субъекта по доле расхода в общей сумме расходов субъекта среди субъектов его федерального округа");
                headerLayout.AddCell("Ранг доля РФ", "Ранг (место) по доле расхода в общей сумме расходов  субъекта среди всех субъектов");
            }

            GridBrick.GridHeaderLayout.ApplyHeaderInfo();

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(235);
            e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(43);

            for (int i = 2; i < columnCount - 5; i = i + 1)
            {
                string columnName = e.Layout.Bands[0].Columns[i].Header.Caption;

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], GetColumnFormat(columnName));
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(GetColumnWidth(columnName));
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }
            
            #region Настройка индикаторов

            FontRowLevelRule levelRule = new FontRowLevelRule(columnCount - 1);
            levelRule.AddFontLevel("0", GridBrick.BoldFont8pt);
            GridBrick.AddIndicatorRule(levelRule);

            GrowRateRule growRateRule = new GrowRateRule("Темп роста");
            GridBrick.AddIndicatorRule(growRateRule);

            PerformanceUniformityRule uniformityRule = new PerformanceUniformityRule("Исполнено %", currentDate.Month);
            GridBrick.AddIndicatorRule(uniformityRule);

            RankIndicatorRule percentFORankRule = new RankIndicatorRule("Ранг бюдж. расходов на душу населения ФО", "Худший ранг бюдж.обесп. ФО");
            percentFORankRule.BestRankHint = "Самые высокие бюджетные расходы на душу населения в федеральном округе";
            percentFORankRule.WorseRankHint = "Самые низкие бюджетные расходы на душу населения в федеральном округе";
            GridBrick.AddIndicatorRule(percentFORankRule);

            RankIndicatorRule percentRFRankRule = new RankIndicatorRule("Ранг бюдж. расходов на душу населения РФ", "Худший ранг бюдж.обесп. РФ");
            percentRFRankRule.BestRankHint = "Самые высокие бюджетные расходы на душу населения в РФ";
            percentRFRankRule.WorseRankHint = "Самые низкие бюджетные расходы на душу населения в РФ";
            GridBrick.AddIndicatorRule(percentRFRankRule);

            if (!PercentRankHidden)
            {
                RankIndicatorRule avgFORankRule = new RankIndicatorRule("Ранг доля ФО", "Худший ранг ФО");
                avgFORankRule.BestRankHint = "Самая высокая доля расхода в федеральном округе";
                avgFORankRule.WorseRankHint = "Самая низкая доля расхода в федеральном округе";
                GridBrick.AddIndicatorRule(avgFORankRule);

                RankIndicatorRule avgRFRankRule = new RankIndicatorRule("Ранг доля РФ", "Худший ранг РФ");
                avgRFRankRule.BestRankHint = "Самая высокая доля расхода в РФ";
                avgRFRankRule.WorseRankHint = "Самая низкая доля расхода в РФ";
                GridBrick.AddIndicatorRule(avgRFRankRule);
            }

            #endregion
        }

        private static string GetColumnFormat(string columnName)
        {
            if (columnName.ToLower().Contains("ранг"))
            {
                return "N0";
            }
            if (columnName.ToLower().Contains("доля") || columnName.Contains("%") || columnName.ToLower().Contains("темп роста"))
            {
                return "P2";
            }
            return "N3";
        }

        private int GetColumnWidth(string columnName)
        {
            double koeff = PercentRankHidden ? 1.23 : 1;
            if (columnName.ToLower().Contains("ранг"))
            {
                return Convert.ToInt32(67 * koeff);
            }
            if (columnName.ToLower().Contains("исполнено %"))
            {
                return Convert.ToInt32(80 * koeff);
            }
            if (columnName.ToLower().Contains("доля") || columnName.Contains("%"))
            {
                return Convert.ToInt32(60 * koeff);
            }
            if ( columnName.ToLower().Contains("темп роста"))
            {
                return Convert.ToInt32(80 * koeff);
            }
            return Convert.ToInt32(97 * koeff);
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
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("Карта");
            ReportExcelExporter1.Export(DundasMap, sheet2, 3);
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
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);

            ISection section2 = report.AddSection();
            DundasMap.Width = Unit.Pixel(Convert.ToInt32(DundasMap.Width.Value * 0.85));
            ReportPDFExporter1.Export(DundasMap, section2);
        }

        #endregion
    }
}