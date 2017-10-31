using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core.QueryGenerators;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0035_07
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable gridDt = new DataTable();
        private DataTable factGridDt = new DataTable();
        private DataTable planGridDt = new DataTable();
        private DataTable dynamicChartDt = new DataTable();
        private DataTable structChartDt = new DataTable();
        private int firstYear = 2010;

        private DateTime currentDate;
        private DateTime lastCubeDate;

        private KbkMemberGenerator kbkGenerator;

        const string indicatorColumnName = "Наименование показателей";
        const string planColumnName = "Уточненная сводная бюджетная роспись на год";
        const string factColumnName = "Кассовое исполнение за год";
        const string completePercentColumnName = "% исполнения";
        const string growRateColumnName = "Темп роста";

        #endregion

        #region Параметры запроса

        private CustomParam memberDeclaration;
        private CustomParam memberList;
        private CustomParam memberDetailList;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.Height = CustomReportConst.minScreenHeight - 235;
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);

            #endregion

            #region Настройка диаграммы динамики

            DynamicChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.7 - 100);

            DynamicChartBrick.YAxisLabelFormat = "N2";
            DynamicChartBrick.XAxisLabelVisible = false;
            DynamicChartBrick.DataFormatString = "N2";
            DynamicChartBrick.Legend.Visible = true;
            DynamicChartBrick.Legend.Location = LegendLocation.Top;
            DynamicChartBrick.Legend.SpanPercentage = 7;
            DynamicChartBrick.Legend.FormatString = "<ITEM_LABEL>";
            DynamicChartBrick.Legend.Margins.Right = 2 * Convert.ToInt32(DynamicChartBrick.Width.Value) / 3;
            DynamicChartBrick.ColorModel = ChartColorModel.DefaultFixedColors;
            DynamicChartBrick.XAxisExtent = 180;
            DynamicChartBrick.YAxisExtent = 90;
            DynamicChartBrick.ZeroAligned = true;
            DynamicChartBrick.SeriesLabelWrap = true;
            DynamicChartBrick.TooltipFormatString = "<SERIES_LABEL>\n<b><ITEM_LABEL></b> г.\n<b><DATA_VALUE:N2></b> тыс.руб.";
            DynamicChartBrick.XAxisSeriesLabelWidth = 30;
            DynamicChartBrick.DataItemCaption = "Тыс.руб.";

            #endregion

            #region Настройка структурной диаграммы

            StructChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth- 25);
            StructChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 - 100);

            StructChartBrick.StartAngle = 180;
            StructChartBrick.TooltipFormatString = "<ITEM_LABEL>\n<b><DATA_VALUE:N2></b> тыс.руб.\nдоля: <b><PERCENT_VALUE:N2>%</b>";
            StructChartBrick.DataFormatString = "N2";
            StructChartBrick.Legend.Visible = true;
            StructChartBrick.Legend.Location = LegendLocation.Left;
            StructChartBrick.Legend.SpanPercentage = 30;
            StructChartBrick.ColorModel = ChartColorModel.ExtendedFixedColors;
            StructChartBrick.OthersCategoryPercent = 0;

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            #region Инициализация параметров запроса

            memberDeclaration = UserParams.CustomParam("member_declaration");
            memberList = UserParams.CustomParam("member_list");
            memberDetailList = UserParams.CustomParam("member_detail_list");

            #endregion

            lastCubeDate = CubeInfoHelper.MonthReportIncomesInfo.LastDate;

            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastCubeDate.Year));
                ComboYear.SetСheckedState(lastCubeDate.Year.ToString(), true);
            }
            
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, 1, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            Page.Title = "Динамика и структура объема ассигнований областного бюджета на капитальные вложения в разрезе направлений";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("Данные за {0}-{1} годы, тыс.руб.", currentDate.Year - 2, currentDate.Year);
            DynamicChartCaption.Text = String.Format("Динамика объема бюджетных ассигнований на капитальные вложения по направлениям за {0}-{1} годы, тыс. руб.", currentDate.Year - 2, currentDate.Year);
            StructChartCaption.Text = String.Format("Структура объема бюджетных ассигнований на капитальные вложения по направлениям на {0} год", currentDate.Year);

            GenerateQuery(currentDate.Year);
            GridDataBind();
            DynamicChartDataBind();
            StructChartDataBind();
        }

        private void GenerateQuery(int yearNum)
        {
            DescendantsGenerator descendantsGenerator = new DescendantsGenerator("[Расходы__АС Бюджет].[Расходы__АС Бюджет]",
                String.Format("ФО\\0001 АС Бюджет - УФиНП {0}", yearNum), "Расходы уровень 8", "SELF");

            kbkGenerator = new KbkMemberGenerator(DataProvidersFactory.PrimaryMASDataProvider, RegionSettingsHelper.GetReportConfigFullName(),
                descendantsGenerator, "Sum");
            kbkGenerator.CodeProperty = "[Расходы__АС Бюджет].[Расходы__АС Бюджет].CurrentMember.Properties(\"Код\")";
            kbkGenerator.CodeComparingRule = "or ([Measures].[Код] = \"{0}000\")";

            kbkGenerator.GenerateQuery(yearNum);

            memberDeclaration.Value = kbkGenerator.MemberDeclarationListString;
            memberList.Value = kbkGenerator.MemberListString;
            memberDetailList.Value = kbkGenerator.MemberListString;
        }

        #region Обработчики грида
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0035_07_objectFinanseCube");
            gridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, indicatorColumnName, gridDt);

            if (currentDate.Year == 2011)
            {
                query = DataProvider.GetQueryText("FO_0002_0035_07_grid_fact");
                factGridDt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, indicatorColumnName, factGridDt);

                if (factGridDt.Columns.Count > 1 && factGridDt.Rows.Count > 0)
                {
                    factGridDt.PrimaryKey = new DataColumn[] {factGridDt.Columns[0]};

                    foreach (DataRow gridRow in gridDt.Rows)
                    {
                        string rowName = gridRow[0].ToString();
                        DataRow factRow = factGridDt.Rows.Find(rowName + "_");

                        if (factRow != null)
                        {
                            gridRow[currentDate.Year + "; " + factColumnName] = factRow[currentDate.Year + "; " + factColumnName];
                        }
                        else
                        {
                            SetNullRowValue(gridRow, currentDate.Year + "; " + factColumnName);
                        }
                    }
                }
                
                query = DataProvider.GetQueryText("FO_0002_0035_07_grid_plan");
                planGridDt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, indicatorColumnName, planGridDt);

                if (planGridDt.Columns.Count > 1 && planGridDt.Rows.Count > 0)
                {
                    planGridDt.PrimaryKey = new DataColumn[] { planGridDt.Columns[0] };

                    foreach (DataRow gridRow in gridDt.Rows)
                    {
                        string rowName = gridRow[0].ToString();
                        DataRow planRow = planGridDt.Rows.Find(rowName + "_");
                        
                        if (planRow != null)
                        {
                            gridRow[currentDate.Year + "; " + planColumnName] = planRow[currentDate.Year + "; " + planColumnName];

                            double plan = GetRowValue(gridRow, currentDate.Year + "; " + planColumnName);
                            double fact = GetRowValue(gridRow, currentDate.Year + "; " + factColumnName);
                            double lastPlan = GetRowValue(gridRow, currentDate.Year - 1 + "; " + planColumnName);
                            double lastFact = GetRowValue(gridRow, currentDate.Year - 1 + "; " + factColumnName);

                            if (plan != 0 && plan != Double.MinValue && fact != Double.MinValue)
                            {
                                SetRowValue(gridRow, currentDate.Year + "; " + completePercentColumnName, fact/plan);
                            }
                            else
                            {
                                SetNullRowValue(gridRow, currentDate.Year + "; " + completePercentColumnName);
                            }

                         /* if (lastPlan != 0 && lastPlan != Double.MinValue && plan != Double.MinValue) // темп роста 
                            {
                                
                                SetRowValue(gridRow, currentDate.Year + "; " + growRateColumnName, plan / lastPlan);
                            }
                            else
                            {
                                SetNullRowValue(gridRow, currentDate.Year + "; " + growRateColumnName);
                            }*/

                            if (lastFact != 0 && lastFact != Double.MinValue && fact != Double.MinValue)
                            {

                                SetRowValue(gridRow, currentDate.Year + "; " + growRateColumnName, fact / lastFact);
                            }
                            else
                            {
                                SetNullRowValue(gridRow, currentDate.Year + "; " + growRateColumnName);
                            }

                        }
                        else
                        {
                            SetNullRowValue(gridRow, currentDate.Year + "; " + planColumnName);
                            SetNullRowValue(gridRow, currentDate.Year + "; " + factColumnName);
                            SetNullRowValue(gridRow, currentDate.Year + "; " + completePercentColumnName);
                            SetNullRowValue(gridRow, currentDate.Year + "; " + growRateColumnName);
                        }
                    }

                }
            }

            GrowRateRule gropRateRule = new GrowRateRule("Темп роста кассового исполнения");
            gropRateRule.IncreaseImg = "~/images/arrowGreenUpBB.png"; 
            gropRateRule.IncreaseText = "Рост по отношению к предыдущему году";
            gropRateRule.DecreaseImg = "~/images/arrowRedDownBB.png";
            gropRateRule.DecreaseText = "Сокращение по отношению к предыдущему году";
            GridBrick.AddIndicatorRule(gropRateRule);

            FontRowLevelRule levelRule = new FontRowLevelRule(gridDt.Columns.Count - 1);
            levelRule.AddFontLevel("0", GridBrick.BoldFont8pt);
            GridBrick.AddIndicatorRule(levelRule);

            GridBrick.DataTable = gridDt;
        }

        private static double GetRowValue(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                if (row[columnName] != DBNull.Value && row[columnName].ToString() != String.Empty)
                {
                    return Convert.ToDouble(row[columnName]);
                }
            }

            return Double.MinValue;
        }

        private static void SetRowValue(DataRow row, string columnName, double value)
        {
            if (row.Table.Columns.Contains(columnName) && value != Double.MinValue)
            {
                row[columnName] = value;
            }
        }

        private static void SetNullRowValue(DataRow row, string columnName)
        {
            if (row.Table.Columns.Contains(columnName))
            {
                row[columnName] = DBNull.Value;
            }
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(210);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;
            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                string columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption;

                string formatString = GetColumnFormat(columnCaption);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("Направление капитальных вложений ");

            AddColumnGroup(headerLayout, String.Format("{0} год", currentDate.Year - 2));
            AddColumnGroup(headerLayout, String.Format("{0} год", currentDate.Year - 1));
            AddColumnGroup(headerLayout, String.Format("{0} год", currentDate.Year));

            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }

        private static void AddColumnGroup(GridHeaderLayout headerLayout, string columnName)
        {
            GridHeaderCell groupCell = headerLayout.AddCell(columnName);
            groupCell.AddCell("Уточненная сводная бюджетная роспись на год", "Уточненный план на год");
            groupCell.AddCell("Кассовое исполнение за год", "Кассовое исполнение за год");
            groupCell.AddCell("% исполнения к уточненной сводной бюджетной росписи", "Процент исполнения к уточненной сводной бюджетной росписи");
            groupCell.AddCell("Темп роста кассового исполнения", "Темп роста кассового исполнения к прошлому году");
        }
        
        private static string GetColumnFormat(string columnName)
        {
            if (columnName.ToLower().Contains("%") || columnName.ToLower().Contains("темп роста"))
            {
                return "P2";
            }
            return "N2";
        }

        #endregion

        #region Обработчики диаграмм

        private void StructChartDataBind()
        {
            if (gridDt == null)
            {
                return;
            }

            structChartDt = new DataTable();
            structChartDt.Columns.Add(indicatorColumnName, typeof(string));
            structChartDt.Columns.Add(String.Format("{0}; {1}", currentDate.Year, planColumnName), typeof(double));

            FillDtValues(structChartDt, gridDt, true);

            structChartDt.Columns[1].ColumnName = (currentDate.Year).ToString();

            StructChartBrick.DataTable = structChartDt;
        }

        private void DynamicChartDataBind()
        {
            if (gridDt == null)
            {
                return;
            }

            dynamicChartDt = new DataTable();
            dynamicChartDt.Columns.Add(indicatorColumnName, typeof(string));
            dynamicChartDt.Columns.Add(String.Format("{0}; {1}", currentDate.Year - 2, planColumnName), typeof(double));
            dynamicChartDt.Columns.Add(String.Format("{0}; {1}", currentDate.Year - 1, planColumnName), typeof(double));
            dynamicChartDt.Columns.Add(String.Format("{0}; {1}", currentDate.Year, planColumnName), typeof(double));

            FillDtValues(dynamicChartDt, gridDt, false);

            dynamicChartDt.Columns[1].ColumnName = (currentDate.Year - 2).ToString();
            dynamicChartDt.Columns[2].ColumnName = (currentDate.Year - 1).ToString();
            dynamicChartDt.Columns[3].ColumnName = (currentDate.Year).ToString();

            DynamicChartBrick.DataTable = dynamicChartDt;
        }

        private static void FillDtValues(DataTable destDt, DataTable sourceDt, bool buidlingSkip)
        {
            foreach (DataRow sourceRow in sourceDt.Rows)
            {
                string rowName = String.Empty;
                if (sourceRow[0] != DBNull.Value)
                {
                    rowName = sourceRow[0].ToString();
                }

                bool skipRow = rowName == "Итого" || rowName == "Итого за счет средств областного бюджета" ||
                              (buidlingSkip && rowName == "Строительство за счет средств федерального бюджета");

                if (!skipRow)
                {
                    DataRow destRow = destDt.NewRow();

                    bool isEmptyRow = true;
                    foreach (DataColumn sourceColumn in sourceDt.Columns)
                    {
                        if (destDt.Columns.Contains(sourceColumn.ColumnName))
                        {
                            destRow[sourceColumn.ColumnName] = sourceRow[sourceColumn.ColumnName];

                            if (sourceColumn.ColumnName != indicatorColumnName)
                            {
                                isEmptyRow = isEmptyRow && destRow[sourceColumn.ColumnName] == DBNull.Value;
                            }
                        }
                    }

                    if (!isEmptyRow)
                    {
                        destDt.Rows.Add(destRow);
                    }
                }
            }
            destDt.AcceptChanges();
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

            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма динамики");
            ReportExcelExporter1.Export(DynamicChartBrick.Chart, DynamicChartCaption.Text, sheet2, 3);

            Worksheet sheet3 = workbook.Worksheets.Add("Структурная диаграмма");
            ReportExcelExporter1.Export(StructChartBrick.Chart, StructChartCaption.Text, sheet3, 3);
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
            DynamicChartBrick.Chart.Width = Convert.ToInt32(DynamicChartBrick.Chart.Width.Value * 0.85);
            DynamicChartBrick.Legend.Margins.Right = Convert.ToInt32(DynamicChartBrick.Chart.Width.Value) / 2;
            ReportPDFExporter1.Export(DynamicChartBrick.Chart, DynamicChartCaption.Text, section2);

            ISection section3 = report.AddSection();
            ReportPDFExporter1.Export(StructChartBrick.Chart, StructChartCaption.Text, section3);
        }

        #endregion
    }
}