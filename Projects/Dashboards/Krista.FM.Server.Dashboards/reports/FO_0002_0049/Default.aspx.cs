using System;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0049
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable moGridDt = new DataTable();
        private DataTable percentChartDt = new DataTable();
        private DataTable structChartDt = new DataTable();
        private DataTable dynamicChartDt = new DataTable();
        private DateTime currentDate;
        private int firstYear = 2009;
        private static MemberAttributesDigest budgetDigest;

        #endregion

        #region Параметры запроса

        // выбранный бюджет
        private CustomParam selectedBudget;
        // выбранный показатель
        private CustomParam selectedIndicator;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            MOGridBrick.AutoSizeStyle = GridAutoSizeStyle.Auto;
            MOGridBrick.Height = CustomReportConst.minScreenHeight - 235;
            MOGridBrick.Width = CustomReportConst.minScreenWidth - 15;
            MOGridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(MOGrid_InitializeLayout);
            MOGridBrick.Grid.ActiveRowChange += new ActiveRowChangeEventHandler(MOGrid_ActiveRowChange);
            MOGridBrick.Grid.DataBound += new EventHandler(MOGrid_DataBound);

            #endregion

            #region Настройка диаграммы динамики

            DynamicChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth - 25);
            DynamicChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.65 - 100);

            DynamicChartBrick.YAxisLabelFormat = "N0";
            DynamicChartBrick.DataFormatString = "N0";
            DynamicChartBrick.Legend.Visible = false;
            DynamicChartBrick.ColorModel = ChartColorModel.DefaultFixedColors;
            DynamicChartBrick.TitleTop = "";
            DynamicChartBrick.XAxisExtent = 150;
            DynamicChartBrick.ZeroAligned = true;
            DynamicChartBrick.SeriesLabelWrap = true;

            #endregion

            #region Настройка диаграммы удельного веса

            PercentChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth * 0.5 - 25);
            PercentChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.7 - 100);

            PercentChartBrick.DataFormatString = "N0";
            PercentChartBrick.Legend.Visible = true;
            PercentChartBrick.Legend.Location = LegendLocation.Bottom;
            PercentChartBrick.Legend.SpanPercentage = 30;
            PercentChartBrick.ColorModel = ChartColorModel.PureRandom;
            PercentChartBrick.TitleTop = "";
            PercentChartBrick.OthersCategoryPercent = 3;

            #endregion

            #region Настройка диаграммы структуры

            StructChartBrick.Width = Convert.ToInt32(CustomReportConst.minScreenWidth * 0.5 - 25);
            StructChartBrick.Height = Convert.ToInt32(CustomReportConst.minScreenHeight * 0.7 - 100);

            StructChartBrick.DataFormatString = "N0";
            StructChartBrick.Legend.Visible = true;
            StructChartBrick.Legend.Location = LegendLocation.Bottom;
            StructChartBrick.Legend.SpanPercentage = 30;
            StructChartBrick.ColorModel = ChartColorModel.PureRandom;
            StructChartBrick.TitleTop = "";
            StructChartBrick.OthersCategoryPercent = 3;

            #endregion

            #region Инициализация параметров запроса

            selectedBudget = UserParams.CustomParam("selected_budget");
            selectedIndicator = UserParams.CustomParam("selected_indicator");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            CrossLink.Visible = true;
            CrossLink.Text = "Анализ&nbsp;данных&nbsp;по&nbsp;контингентам";
            CrossLink.NavigateUrl = "~/reports/FO_0002_0050/Default.aspx";
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            budgetDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0049_budgetDigest");

            if (!Page.IsPostBack)
            {
                chartWebAsyncPanel.AddRefreshTarget(DynamicChartBrick.Chart.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(PercentChartBrick.Chart.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(StructChartBrick.Chart.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(DynamicChartCaption.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(PercentChartCaption.ClientID);
                chartWebAsyncPanel.AddRefreshTarget(StructChartCaption.ClientID);
                chartWebAsyncPanel.AddLinkedRequestTrigger(MOGridBrick.Grid.ClientID);

                DateTime lastDate = CubeInfoHelper.FoYearReportStatesInfo.LastDate;
 
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, lastDate.Year));
                ComboYear.SetСheckedState(lastDate.Year.ToString(), true);

                ComboBudget.Title = "Бюджет";
                ComboBudget.Width = 600;
                ComboBudget.MultiSelect = false;
                ComboBudget.ParentSelect = true;
                ComboBudget.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetDigest.UniqueNames, budgetDigest.MemberLevels));
                ComboBudget.SetСheckedState("Консолидированный бюджет субъекта", true);

                hiddenIndicatorLabel.Text = "Сеть государственных и муниципальных учреждений и органов власти";
            }
            
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            currentDate = new DateTime(yearNum, 1, 1);

            UserParams.PeriodYear.Value = currentDate.Year.ToString();

            Page.Title = String.Format("Анализ данных по сетям и штатам");
            Label1.Text = Page.Title;
            Label2.Text = String.Format("{1}, данные за {0} год", currentDate.Year, ComboBudget.SelectedValue);

            selectedBudget.Value = budgetDigest.GetMemberUniqueName(ComboBudget.SelectedValue);

            GridDataBind();
        }

        #region Обработчики грида
        
        private void GridDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0049_moGrid");
            moGridDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", moGridDt);

            if (moGridDt.Rows.Count > 0)
            {
                MOGridBrick.DataTable = moGridDt;

                MOGridBrick.Grid.Columns.RemoveAt(0);
            }
        }

        protected void MOGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(400);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            for (int i = 1; i < columnCount; i = i + 1)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            GridHeaderLayout headerLayout = MOGridBrick.GridHeaderLayout;

            headerLayout.AddCell("Показатель");
            
            headerLayout.AddCell((currentDate.Year - 2).ToString());
            headerLayout.AddCell((currentDate.Year - 1).ToString());
            headerLayout.AddCell((currentDate.Year).ToString());

            headerLayout.ApplyHeaderInfo();
        }

        private void MOGrid_DataBound(object sender, EventArgs e)
        {
            if (!chartWebAsyncPanel.IsAsyncPostBack)
            {
                UltraGridRow row = CRHelper.FindGridRow(MOGridBrick.Grid, selectedIndicator.Value, 0, 0);
                ActivateGridRow(row);
            }
        }

        private void MOGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActivateGridRow(e.Row);
        }

        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
            {
                return;
            }

            string indicatorName = row.Cells[0].Text;

            hiddenIndicatorLabel.Text = indicatorName;
            selectedIndicator.Value = hiddenIndicatorLabel.Text;

            if (selectedIndicator.Value.ToLower().Contains("сеть"))
            {
                DynamicChartCaption.Text = "Динамика сети государственных и муниципальных учреждений и органов власти";
                PercentChartCaption.Text = String.Format("Отраслевая структура сети государственных и муниципальных учреждений и органов власти за {0} год", currentDate.Year);
                StructChartCaption.Text = String.Format("Структура государственных и муниципальных учреждений и органов власти за {0} год", currentDate.Year);

                SetChartTooltips(String.Empty);
            }
            else if (selectedIndicator.Value.ToLower().Contains("штаты"))
            {
                DynamicChartCaption.Text = "Динамика штатов государственных и муниципальных учреждений и органов власти";
                PercentChartCaption.Text = String.Format("Отраслевая структура штатов государственных и муниципальных учреждений и органов власти за {0} год", currentDate.Year);
                StructChartCaption.Text = String.Format("Структура штатной численности по видам государственных и муниципальных учреждений и органов власти за {0} год", currentDate.Year);

                SetChartTooltips(String.Empty);
            }
            else if (selectedIndicator.Value.ToLower().Contains("расходы на оплату труда"))
            {
                DynamicChartCaption.Text = "Динамика расходов на оплату труда и начисления, тыс.руб.";
                PercentChartCaption.Text = String.Format("Отраслевая структура расходов на оплату труда и начисления за {0} год, тыс.руб.", currentDate.Year);
                StructChartCaption.Text = String.Format("Структура расходов на оплату труда и начисления в разрезе видов учреждений за {0} год, тыс.руб.", currentDate.Year);
                
                DynamicChartBrick.DataItemCaption = "тыс.руб.";
                SetChartTooltips("тыс.руб.");
            }
            else if (selectedIndicator.Value.ToLower().Contains("текущие расходы"))
            {
                DynamicChartCaption.Text = "Динамика текущих расходов государственных и муниципальных учреждений и органов власти, производимых за счет бюджета, тыс.руб.";

                DynamicChartBrick.DataItemCaption = "тыс.руб.";
                SetChartTooltips("тыс.руб.");
            }
            else
            {
                DynamicChartCaption.Text = "Динамика средней заработной платы государственных и муниципальных учреждений и органов власти, тыс.руб.";
                PercentChartCaption.Text = String.Format("Отраслевая структура средней заработной платы за {0} год,<br/>тыс.руб.", currentDate.Year);
                StructChartCaption.Text = String.Format("Структура средней заработной платы в разрезе видов учреждений за {0} год, тыс.руб.", currentDate.Year);

                SetChartTooltips("тыс.руб.");
            }

            ChartsDataBind();
        }

        private void SetChartTooltips(string dataItem)
        {
            DynamicChartBrick.TooltipFormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL> г.\n<DATA_VALUE:N0> {0}", dataItem);
            PercentChartBrick.TooltipFormatString = String.Format("<SERIES_LABEL>\n<DATA_VALUE:N0> {0}\nдоля <PERCENT_VALUE:N2>%", dataItem);
            StructChartBrick.TooltipFormatString = String.Format("<SERIES_LABEL>\n<DATA_VALUE:N0> {0}\nдоля <PERCENT_VALUE:N2>%", dataItem);
        }

        #endregion

        #region Обработчики диаграммы

        private void ChartsDataBind()
        {
            string query = DataProvider.GetQueryText("FO_0002_0049_dynamicChart");
            dynamicChartDt = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dynamicChartDt);

            if (dynamicChartDt.Rows.Count > 0)
            {
                foreach (DataRow row in dynamicChartDt.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        row[0] = DataDictionariesHelper.GetShortRzPrName(row[0].ToString());
                    }
                }

                DynamicChartBrick.DataTable = dynamicChartDt;
                DynamicChartBrick.DataBind();
            }

            if (selectedIndicator.Value.ToLower().Contains("текущие расходы"))
            {
                StructChartTable.Visible = false;
                PercentChartTable.Visible = false;
            }
            else
            {
                StructChartTable.Visible = true;
                query = DataProvider.GetQueryText("FO_0002_0049_structChart");
                structChartDt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", structChartDt);

                if (structChartDt.Rows.Count > 0)
                {
                    StructChartBrick.DataTable = structChartDt;
                    StructChartBrick.DataBind();
                }

                PercentChartTable.Visible = true;
                query = DataProvider.GetQueryText("FO_0002_0049_percentChart");
                percentChartDt = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", percentChartDt);

                if (percentChartDt.Rows.Count > 0)
                {
                    foreach (DataRow row in percentChartDt.Rows)
                    {
                        if (row[0] != DBNull.Value)
                        {
                            row[0] = DataDictionariesHelper.GetShortRzPrName(row[0].ToString());
                        }
                    }

                    PercentChartBrick.DataTable = percentChartDt;
                    PercentChartBrick.DataBind();
                }
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
            ReportExcelExporter1.Export(MOGridBrick.GridHeaderLayout, sheet1, 3);

            Worksheet sheet2 = workbook.Worksheets.Add("Динамика");
            ReportExcelExporter1.Export(DynamicChartBrick.Chart, DynamicChartCaption.Text, sheet2, 3);



            Worksheet sheet3 = workbook.Worksheets.Add("Доля отраслей");
            ReportExcelExporter1.Export(PercentChartBrick.Chart, PercentChartCaption.Text, sheet3, 3);

            Worksheet sheet4 = workbook.Worksheets.Add("Структура");
            ReportExcelExporter1.Export(StructChartBrick.Chart, StructChartCaption.Text, sheet4, 3);
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
            ReportPDFExporter1.Export(MOGridBrick.GridHeaderLayout, section1);

            ISection section2 = report.AddSection();
            DynamicChartBrick.Chart.Width = Convert.ToInt32(DynamicChartBrick.Chart.Width.Value * 0.8);
            ReportPDFExporter1.Export(DynamicChartBrick.Chart, DynamicChartCaption.Text, section2);

            ISection section3 = report.AddSection();
            ReportPDFExporter1.Export(PercentChartBrick.Chart, PercentChartCaption.Text, section3);

            ISection section4 = report.AddSection();
            ReportPDFExporter1.Export(StructChartBrick.Chart, StructChartCaption.Text, section4);
        }

        #endregion
    }
}