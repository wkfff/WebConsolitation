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
using Infragistics.Documents.Reports.Report.Text;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace Krista.FM.Server.Dashboards.reports.FK_0001_0002
{
    public partial class DefaultCompare : CustomReportPage
    {
        #region Поля

        private DataTable mapDt = new DataTable();
        private DataTable gridDt;
        private int firstYear = 2000;
        private string populationDate;

        private DateTime currentDate;

        private const string cookieStateAreaParamName = "StateArea";

        #endregion

        #region Свойства

        /// <summary>
        /// Выбраны ли все федеральные округа
        /// </summary>
        public bool AllFO
        {
            get { return ComboFO.SelectedIndex == 0; }
        }

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.7 - 225);
            GridBrick.Width = CustomReportConst.minScreenWidth - 12;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.AutoHeightRowLimit = 15;

            #endregion

            DundasMap.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 15);
            DundasMap.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.7);

            CrossLink1.Text = "Диаграмма распределения субъектов по среднедушевым доходам";
            CrossLink1.NavigateUrl = "~/reports/FK_0001_0002/DefaultAllocation.aspx";
            CrossLink2.Text = "Доходы&nbsp;субъекта&nbsp;РФ&nbsp;подробнее";
            CrossLink2.NavigateUrl = "~/reports/FK_0001_0004/DefaultDetail.aspx";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            GridSearch1.LinkedGridId = GridBrick.Grid.ClientID;
        }

        private void SetupMap()
        {
            DundasMap.Meridians.Visible = false;
            DundasMap.Parallels.Visible = false;
            DundasMap.ZoomPanel.Visible = true;
            DundasMap.NavigationPanel.Visible = true;
            DundasMap.Viewport.EnablePanning = true;

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
            legend.Title = "Процент исполнения";
            legend.AutoFitMinFontSize = 7;
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
            DundasMap.ShapeFields.Add("CompletePercent");
            DundasMap.ShapeFields["CompletePercent"].Type = typeof(double);
            DundasMap.ShapeFields["CompletePercent"].UniqueIdentifier = false;

            // добавляем правила раскраски
            DundasMap.ShapeRules.Clear();
            ShapeRule rule = new ShapeRule();
            rule.Name = "CompleteRule";
            rule.Category = String.Empty;
            rule.ShapeField = "CompletePercent";
            rule.DataGrouping = DataGrouping.EqualDistribution;
            rule.ColorCount = 5;
            rule.ColoringMode = ColoringMode.ColorRange;
            rule.FromColor = Color.Red;
            rule.MiddleColor = Color.Yellow;
            rule.ToColor = Color.Green;
            rule.BorderColor = Color.FromArgb(50, Color.Black);
            rule.GradientType = GradientType.None;
            rule.HatchStyle = MapHatchStyle.None;
            rule.ShowInColorSwatch = false;
            rule.ShowInLegend = "CompleteLegend";
            rule.LegendText =  "#FROMVALUE{N2} - #TOVALUE{N2}";
            DundasMap.ShapeRules.Add(rule);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                currentDate = CubeInfoHelper.FkMonthReportIncomesInfo.LastDate;

                UserParams.PeriodYear.Value = currentDate.Year.ToString();
                UserParams.PeriodMonth.Value = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(currentDate.Month));
              //  UserParams.Filter.Value = "Все федеральные округа";
                UserParams.KDGroup.Value = "Доходы ВСЕГО ";

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, currentDate.Year));
                ComboYear.SetСheckedState(currentDate.Year.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                ComboFO.Title = "ФО";
                ComboFO.Width = 300;
                ComboFO.MultiSelect = false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
             //   ComboFO.SetСheckedState(UserParams.Filter.Value, true);

                ComboKD.Width = 420;
                ComboKD.Title = "Вид дохода";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillFullKDIncludingList());
                ComboKD.RenameTreeNodeByName("Доходы бюджета - Итого ", "Доходы ВСЕГО ");
                ComboKD.SetСheckedState(UserParams.KDGroup.Value, true);

                if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    ComboFO.SetСheckedState(RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name), true);
                }
            }

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), ComboMonth.SelectedIndex + 1, 1);

            Page.Title = String.Format("Исполнение доходов ({0})", ComboFO.SelectedIndex == 0
                                                                       ? "РФ"
                                                                       : RegionsNamingHelper.ShortName(ComboFO.SelectedValue));
            Label1.Text = Page.Title;
            Label2.Text = String.Format(
                    "Исполнение консолидированных бюджетов субъектов РФ по доходам ({3}) за {0} {1} {2} года", currentDate.Month,
                    CRHelper.RusManyMonthGenitive(currentDate.Month), currentDate.Year, ComboKD.SelectedValue);

            UserParams.PeriodLastYear.Value = (currentDate.Year - 1).ToString();
            UserParams.PeriodYear.Value = currentDate.Year.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = String.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(currentDate.Month));
            UserParams.PeriodQuater.Value = String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(currentDate.Month));
            UserParams.KDGroup.Value = ComboKD.SelectedValue;

            populationDate = DataDictionariesHelper.GetFederalPopulationDate();

            GridDataBind();

            // заполняем карту формами
            string regionStr = (ComboFO.SelectedIndex == 0) ? "Российская Федерация" : ComboFO.SelectedValue;
            DundasMap.Shapes.Clear();

            DundasMap.LoadFromShapeFile(Server.MapPath(String.Format("../../maps/{0}/{0}.shp", RegionsNamingHelper.ShortName(regionStr))), "NAME", true);

//            DundasMap.Serializer.Format = SerializationFormat.Binary;
//            DundasMap.Serializer.Load((Server.MapPath(String.Format("../../maps/{0}/{0}.bin", RegionsNamingHelper.ShortName(regionStr)))));

            SetupMap();
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
                            subject = (isRepublic || isTown ) ? subjects[1] : subjects[0];
                            break;
                        }
                }
            }
           /* foreach (Shape name in map.Shapes)
            {
                CRHelper.SaveToErrorLog(name.Name);
            }*/
            
            ArrayList shapeList = map.Shapes.Find(subject, true, false);
            if (shapeList.Count > 0)
            {
                return (Shape)shapeList[0];
            }
            return null;
        }

        public void FillMapData()
        {
            string query = DataProvider.GetQueryText("FK_0001_0002_compare_map");
            mapDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", mapDt);

            const string completePercentColumnName = "Процент выполнения назначений";

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
                            double value = 100*Convert.ToDouble(row[completePercentColumnName]);

                            if (subject.Contains("Чувашия"))
                            {
                                subject = "Чувашская республика";
                            }

                            shape["Name"] = subject;
                            shape["CompletePercent"] = value;
                            shape.ToolTip = "#NAME\n#COMPLETEPERCENT{N2}%";
                            shape.TextVisibility = TextVisibility.Shown;
                            shape.Text = String.Format("{0}\n{1:N2}%", subject.Replace(" ", "\n"), value);

                            if (subject.Contains("Кемеров"))
                            {
                                shape.CentralPointOffset.Y = 90000;
                            }

                            if (subject.Contains("Приморский"))
                            {
                                shape.CentralPointOffset.Y = -90000;
                            }

                            if (subject.Contains("Калинингр"))
                            {
                                shape.TextAlignment = ContentAlignment.MiddleRight;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Обработчики грида

        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FK_0001_0002_compare_Grid");
            gridDt = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", gridDt);
            
            if (gridDt.Rows.Count > 0)
            {
                foreach (DataRow row in gridDt.Rows)
                {
                    if (row[2] != DBNull.Value)
                    {
                        row[2] = Convert.ToDouble(row[2]) / 1000000;
                    }
                    if (row[3] != DBNull.Value)
                    {
                        row[3] = Convert.ToDouble(row[3]) / 1000000;
                    }
                }

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
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

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
            headerLayout.AddCell("Ранг % ФО", "Ранг (место) субъекта по проценту исполнения назначений среди субъектов его федерального округа");
            headerLayout.AddCell("Ранг % РФ", "Ранг (место) по проценту исполнения назначений среди всех субъектов");
            headerLayout.AddCell(String.Format("Численность постоянного населения {0}, тыс.чел.", populationDate), "Численность постоянного населения");
            headerLayout.AddCell("Бюджетные доходы на душу населения, руб./чел.", "Сумма доходов выбранного вида на душу населения");
            headerLayout.AddCell("Ранг среднедуш. ФО", "Ранг (место) субъекта по среднедушевым доходам среди субъектов его федерального округа");
            headerLayout.AddCell("Ранг среднедуш. РФ", "Ранг (место) по среднедушевым доходам среди всех субъектов");
            headerLayout.AddCell("Доля ФО", "Доля доходов субъекта в общей сумме доходов федерального округа");
            headerLayout.AddCell("Доля РФ", "Доля доходов субъекта (округа) в общей сумме доходов всех субъектов РФ");
            
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

            PerformanceUniformityRule uniformityRule = new PerformanceUniformityRule("Исполнено %", currentDate.Month);
            GridBrick.AddIndicatorRule(uniformityRule);

            RankIndicatorRule percentFORankRule = new RankIndicatorRule("Ранг % ФО", "Худший ранг процент ФО");
            percentFORankRule.BestRankHint = "Самый высокий процент исполнения в федеральном округе";
            percentFORankRule.WorseRankHint = "Самый низкий процент исполнения в федеральном округе";
            GridBrick.AddIndicatorRule(percentFORankRule);

            RankIndicatorRule percentRFRankRule = new RankIndicatorRule("Ранг % РФ", "Худший ранг процент РФ");
            percentRFRankRule.BestRankHint = "Самый высокий процент исполнения в РФ";
            percentRFRankRule.WorseRankHint = "Самый низкий процент исполнения в РФ";
            GridBrick.AddIndicatorRule(percentRFRankRule);

            RankIndicatorRule avgFORankRule = new RankIndicatorRule("Ранг среднедуш. ФО", "Худший ранг среднедуш ФО");
            avgFORankRule.BestRankHint = "Самый высокий доход на душу населения в федеральном округе";
            avgFORankRule.WorseRankHint = "Самый низкий доход на душу населения в федеральном округе";
            GridBrick.AddIndicatorRule(avgFORankRule);

            RankIndicatorRule avgRFRankRule = new RankIndicatorRule("Ранг среднедуш. РФ", "Худший ранг среднедуш РФ");
            avgRFRankRule.BestRankHint = "Самый высокий доход на душу населения в РФ";
            avgRFRankRule.WorseRankHint = "Самый низкий доход на душу населения в РФ";
            GridBrick.AddIndicatorRule(avgRFRankRule);

            #endregion
        }

        private static string GetColumnFormat(string columnName)
        {
            if (columnName.ToLower().Contains("ранг"))
            {
                return "N0";
            }
            if (columnName.ToLower().Contains("доля") || columnName.Contains("%"))
            {
                return "P2";
            }
            return "N3";
        }

        private static int GetColumnWidth(string columnName)
        {
            if (columnName.ToLower().Contains("ранг"))
            {
                return 66;
            }
            if (columnName.ToLower().Contains("исполнено %"))
            {
                return 80;
            }
            if (columnName.ToLower().Contains("доля") || columnName.Contains("%"))
            {
                return 60;
            }
            return 97;
        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width)
        {
            CRHelper.FormatNumberColumn(layout.Bands[bandIndex].Columns[columnIndex], format);
            layout.Bands[bandIndex].Columns[columnIndex].Width = CRHelper.GetColumnWidth(width);
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
            ReportExcelExporter1.Export(DundasMap, String.Format("Процент исполнения плана по доходному источнику ({0})", ComboKD.SelectedValue.TrimEnd(' ')), sheet2, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;
            Report report = new Report();

            ISection section1 = report.AddSection();



            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, Label2.Text, section1);

            ISection section2 = report.AddSection();
            DundasMap.Width = Unit.Pixel(Convert.ToInt32(DundasMap.Width.Value * 0.85));
            IText title = section2.AddText();
            Font font = new Font("Verdana", 12);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(Label2.Text);
            ReportPDFExporter1.Export(DundasMap,  section2);
        }

        #endregion
    }
}
