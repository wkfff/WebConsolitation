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

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0034_02
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dateDt = new DataTable();
        private DataTable factGridDt = new DataTable();
        private DataTable planGridDt = new DataTable();
        private DataTable chart1Dt = new DataTable();
        private DataTable chart2Dt = new DataTable();
        private int firstYear = 2011;
        private DateTime currentDate;

        private MemberAttributesDigest administratorDigest;

        #endregion

        #region Параметры запроса

        private CustomParam memberDeclaration;
        private CustomParam memberList;
        // выбранный показатель
        private CustomParam selectedIndicator;
        // множество показателей для диаграммы
        private CustomParam chartIndicatorSet;
        // выбранный администатор
        private CustomParam selectedAdmin;
        // имя выбранного администатора
        private CustomParam selectedAdminCaption;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            memberDeclaration = UserParams.CustomParam("member_declaration");
            memberList = UserParams.CustomParam("member_list");
            selectedIndicator = UserParams.CustomParam("selected_indicator");
            chartIndicatorSet = UserParams.CustomParam("chart_indicator_set");
            selectedAdmin = UserParams.CustomParam("selected_admin");
            selectedAdminCaption = UserParams.CustomParam("selected_admin_caption");

            #endregion

            #region Настройка грида

            GridBrick.Height = CustomReportConst.minScreenHeight - 235;
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.Auto;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.ActiveRowChange += new ActiveRowChangeEventHandler(grid_ActiveRowChange);
            GridBrick.Grid.DataBound += new EventHandler(Grid_DataBound);

            #endregion
            
            #region Настройка диаграммы 1

            PieChartBrick1.Width = Convert.ToInt32(CustomReportConst.minScreenWidth * 0.5 - 25);
            PieChartBrick1.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 - 100);

            PieChartBrick1.DataFormatString = "N2";
            PieChartBrick1.TooltipFormatString = "<SERIES_LABEL>\nплан <DATA_VALUE:N2> млн.руб.\nдоля <PERCENT_VALUE:N2>%";
            PieChartBrick1.Legend.Visible = false;
            PieChartBrick1.ColorModel = ChartColorModel.ExtendedFixedColors;
            PieChartBrick1.TitleTop = "План";

            #endregion

            #region Настройка диаграммы 2

            PieChartBrick2.Width = Convert.ToInt32(CustomReportConst.minScreenWidth * 0.5 - 25);
            PieChartBrick2.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.5 - 100);

            PieChartBrick2.DataFormatString = "N2";
            PieChartBrick2.TooltipFormatString = "<SERIES_LABEL>\nфакт <DATA_VALUE:N2> млн.руб.\nдоля <PERCENT_VALUE:N2>%";
            PieChartBrick2.Legend.Visible = false;
            PieChartBrick2.ColorModel = ChartColorModel.ExtendedFixedColors;
            PieChartBrick2.TitleTop = "Факт";

            #endregion

            #region Настройка диаграммы-легенды

            LegendChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            LegendChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.25 - 100);
            LegendChartBrick.SwapRowAndColumns = false;
            LegendChartBrick.ColorModel = ChartColorModel.ExtendedFixedColors;

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink.Visible = false;
            CrossLink.Text = "Сравнение&nbsp;плановых&nbsp;показателей по&nbsp;основным&nbsp;параметрам&nbsp;бюджетов";
            CrossLink.NavigateUrl = "~/reports/FO_0002_0032/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(chartCaption.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(PieChartBrick1.Chart);
                chartWebAsyncPanel.AddRefreshTarget(PieChartBrick2.Chart);
                chartWebAsyncPanel.AddLinkedRequestTrigger(GridBrick.Grid.ClientID);

                DateTime lastDate = CubeInfoHelper.BudgetOutocmesFactInfo.LastDate;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboYear.SetСheckedState(lastDate.Year.ToString(), true);
                ComboYear.AutoPostBack = true;

                hiddenIndicatorLabel.Text = "Расходы в отраслях социальной сферы и выплат гражданам ";
                selectedAdminCaption.Value = "Все ГРБС";
            }
            
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, 1, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            if (ComboAdmin.SelectedValue != String.Empty)
            {
                selectedAdminCaption.Value = ComboAdmin.SelectedValue;
            }

            ComboAdmin.Title = "ГРБС";
            ComboAdmin.Width = 510;
            ComboAdmin.ParentSelect = true;
            ComboAdmin.MultiSelect = false;
            administratorDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0034_02_administratorDigest");
            ComboAdmin.ClearNodes();
            ComboAdmin.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(administratorDigest.UniqueNames, administratorDigest.MemberLevels));
            ComboAdmin.SelectLastNode();
            ComboAdmin.SetСheckedState(selectedAdminCaption.Value, true);
            
            selectedAdmin.Value = administratorDigest.GetMemberUniqueName(ComboAdmin.SelectedValue);

            Page.Title = "Удельный вес расходов областного бюджета в отраслях социальной сферы и выплат гражданам";
            Label1.Text = Page.Title;
            Label2.Text = String.Format("{1} по состоянию на {0:dd.MM.yyyy}", GetLastYearDay(), ComboAdmin.SelectedValue);
            
            GenerateQuery(currentDate.Year);

            GridDataBind();
        }

        private DateTime GetLastYearDay()
        {
            dateDt = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0034_02_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dateDt);

            if (dateDt.Rows.Count > 0)
            {
                if (dateDt.Rows[0][1] != DBNull.Value && dateDt.Rows[0][1].ToString() != String.Empty)
                {
                    return CRHelper.PeriodDayFoDate(dateDt.Rows[0][1].ToString());
                }
            }

            return new DateTime(2020, 1, 1);
        }
        
        private void GenerateQuery(int yearNum)
        {
            DescendantsGenerator descendantsGenerator = new DescendantsGenerator("[Расходы__АС Бюджет].[Расходы__АС Бюджет]",
                String.Format("ФО\\0001 АС Бюджет - УФиНП {0}", yearNum), "Расходы уровень 8", "SELF");

            KbkMemberGenerator kbkGenerator = new KbkMemberGenerator(DataProvidersFactory.PrimaryMASDataProvider, RegionSettingsHelper.GetReportConfigFullName(),
                descendantsGenerator, "Sum");
            
            kbkGenerator.CodeComparingRule = "or ([Measures].[Код] = \"{0}000\")";
            
            kbkGenerator.GenerateQuery(yearNum);

            memberDeclaration.Value = kbkGenerator.MemberDeclarationListString;
            memberList.Value = kbkGenerator.MemberListString;
        }

        #region Обработчики грида
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0034_02_grid_plan");
            planGridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", planGridDt);

            if (planGridDt.Columns.Count > 1 && planGridDt.Rows.Count > 0)
            {
                query = DataProvider.GetQueryText("FO_0002_0034_02_grid_fact");
                factGridDt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", factGridDt);

                if (factGridDt.Columns.Count > 1 && factGridDt.Rows.Count > 0)
                {
                    factGridDt.PrimaryKey = new DataColumn[] { factGridDt.Columns[0] };

                    foreach (DataRow planRow in planGridDt.Rows)
                    {
                        string rowName = planRow[0].ToString();
                        DataRow factRow = factGridDt.Rows.Find(rowName);
                        if (factRow != null)
                        {
                            foreach (DataColumn column in factGridDt.Columns)
                            {
                                if (column.ColumnName != "Наименование показателей" && factGridDt.Columns.Contains(column.ColumnName))
                                {
                                    planRow[column.ColumnName] = factRow[column.ColumnName];
                                }
                            }
                        }
                    }
                }
                
                if (planGridDt.Columns.Count > 1)
                {
                    foreach (DataRow row in planGridDt.Rows)
                    {
                        if (row[0] != DBNull.Value && row[0].ToString() != String.Empty)
                        {
                            row[0] = row[0].ToString().TrimEnd('_');
                        }
                    }

                    FontRowLevelRule rule = new FontRowLevelRule(planGridDt.Columns.Count - 1);
                    rule.AddFontLevel("0", GridBrick.BoldFont8pt);
                    GridBrick.AddIndicatorRule(rule);
                }

                GridBrick.DataTable = planGridDt;
            }
        }

        private void Grid_DataBound(object sender, EventArgs e)
        {
            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                UltraGridRow row = CRHelper.FindGridRow(GridBrick.Grid, selectedIndicator.Value, 0, 0);
                ActivateGridRow(row);
            }
        }

        private void grid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActivateGridRow(e.Row);
        }

        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
            {
                return;
            }

            string indicatorName = String.Format("{0}_", row.Cells[0].Text);

            bool isTotalRow = indicatorName.ToLower().Contains("расходы в отраслях социальной сферы и выплат гражданам");
            chartIndicatorSet.Value = isTotalRow ? "[Для итоговой строки]" : "[Для отдельного показателя]";

            hiddenIndicatorLabel.Text = indicatorName;
            selectedIndicator.Value = hiddenIndicatorLabel.Text;

            chartCaption.Text = String.Format("{0}, удельный вес в общем объеме расходов областного бюджета",  
                isTotalRow ? "Отрасли социальной сферы и выплат гражданам" : indicatorName.TrimEnd('_').TrimEnd(' '));

            ChartsDataBind();
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(280);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            e.Layout.Bands[0].Columns[columnCount - 1].Hidden = true;

            for (int i = 1; i < columnCount - 1; i = i + 1)
            {
                string columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption;

                string formatString = GetColumnFormat(columnCaption);
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(200);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            headerLayout.AddCell("Наименование расходов");

            GridHeaderCell planGroupCell = headerLayout.AddCell("Уточненные бюджетные назначения");
            planGroupCell.AddCell("Сумма, млн.руб.", "Уточненные годовые назначения, млн.руб.");
            planGroupCell.AddCell("Удельный вес в общих расходах, %", "Удельный вес плановых назначений по каждому направлению в общем объеме расходов областного бюджета, %");

            GridHeaderCell factGroupCell = headerLayout.AddCell("Фактическое исполнение");
            factGroupCell.AddCell("Сумма, млн.руб.", "Кассовый расход нарастающим итогом с начала года, млн.руб.");
            factGroupCell.AddCell("Удельный вес в общих расходах, %", "Удельный вес кассового расхода по каждому направлению в общем объеме расходов областного бюджета, %");
            
            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }

        private static string GetColumnFormat(string columnName)
        {
            if (columnName.ToLower().Contains("удельный вес"))
            {
                return "P2";
            }
            return "N2";
        }

       #endregion
        
        #region Обработчики диаграммы

        private void ChartsDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0034_02_chart_plan");
            chart1Dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", chart1Dt);

            if (chart1Dt.Rows.Count > 0)
            {
                foreach (DataRow row in chart1Dt.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString() != String.Empty)
                    {
                        row[0] = row[0].ToString().Replace("Уточненные плановые назначения для отдельного показателя", "Остальные расходы областного бюджета");
                        row[0] = row[0].ToString().Replace("Уточненные плановые назначения для итого", "Остальные расходы областного бюджета");

                        row[0] = row[0].ToString().TrimEnd('_');
                    }
                }

                PieChartBrick1.DataTable = chart1Dt;
                PieChartBrick1.DataBind();

                LegendChartBrick.DataTable = chart1Dt;
                LegendChartBrick.DataBind();
            }

            query = DataProvider.GetQueryText("FO_0002_0034_02_chart_fact");
            chart2Dt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Период", chart2Dt);

            if (chart2Dt.Rows.Count > 0)
            {
                foreach (DataRow row in chart2Dt.Rows)
                {
                    if (row[0] != DBNull.Value && row[0].ToString() != String.Empty)
                    {
                        row[0] = row[0].ToString().Replace("Фактическое исполнение для отдельного показателя", "Остальные расходы областного бюджета");
                        row[0] = row[0].ToString().Replace("Фактическое исполнениея для итого", "Остальные расходы областного бюджета");

                        row[0] = row[0].ToString().TrimEnd('_');
                    }
                }

                PieChartBrick2.DataTable = chart2Dt;
                PieChartBrick2.DataBind();
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
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма План");
            PieChartBrick1.Legend.Visible = true;
            PieChartBrick1.Legend.Location = LegendLocation.Bottom;
            ReportExcelExporter1.Export(PieChartBrick1.Chart, chartCaption.Text, sheet2, 3);

            Worksheet sheet3 = workbook.Worksheets.Add("Диаграмма Факт");
            PieChartBrick2.Legend.Visible = true;
            PieChartBrick2.Legend.Location = LegendLocation.Bottom;
            ReportExcelExporter1.Export(PieChartBrick2.Chart, chartCaption.Text, sheet3, 3);
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
            ReportPDFExporter1.Export(PieChartBrick1.Chart, chartCaption.Text, section2);
            ReportPDFExporter1.Export(PieChartBrick2.Chart, section2);
            ReportPDFExporter1.Export(LegendChartBrick.Chart, section2);
        }

        #endregion
    }
}