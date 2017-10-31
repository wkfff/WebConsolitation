using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs=Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using InitializeRowEventHandler=Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0017_03
{
    public partial class Default : CustomReportPage
    {
        private static bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 900; }
        }

        private static int MinScreenWidth
        {
            get { return IsSmallResolution ? 750 : CustomReportConst.minScreenWidth; }
        }

        private static int MinScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }

        #region Поля

        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2006;
        private int endYear = 2010;

        #endregion

        #region Параметры запроса

        // множество выбранных лет
        private CustomParam yearSet;
        // выбранный период
        private CustomParam lastYear;

        // выбранный период квартал
        private CustomParam lastYearQuarter;

        // выбранный администратор
        private CustomParam selectedAdministrator;
        // выбранный администратор
        private CustomParam selectedTool;
        
        private CustomParam plan;
        private CustomParam planLastYear;
        private CustomParam planAdmin;
        private CustomParam planLastYearAdmin;

        #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Инициализация параметров запроса

            if (yearSet == null)
            {
                yearSet = UserParams.CustomParam("year_set");
            }
            if (lastYear == null)
            {
                lastYear = UserParams.CustomParam("last_year");
            }
            if (selectedAdministrator == null)
            {
                selectedAdministrator = UserParams.CustomParam("administrator_type");
            }
            if (selectedTool == null)
            {
                selectedTool = UserParams.CustomParam("tool_type");
            }
            if (planAdmin == null)
            {
                planAdmin = UserParams.CustomParam("plan_admin");
            }
            if (planLastYearAdmin == null)
            {
                planLastYearAdmin = UserParams.CustomParam("plan_last_year_admin");
            }

            if (plan == null)
            {
                plan = UserParams.CustomParam("plan");
            }
            if (planLastYear == null)
            {
                planLastYear = UserParams.CustomParam("plan_last_year");
            }

            if (lastYearQuarter == null)
            {
                lastYearQuarter = UserParams.CustomParam("last_year_quarter");
            }

            #endregion

            #region Настройка диаграммы

            //UltraChart.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 50);
            //UltraChart.Height = CRHelper.GetChartWidth(CustomReportConst.minScreenHeight / 2);

            //UltraChart.ChartType = ChartType.StackBarChart;
            //UltraChart.Border.Thickness = 0;
            //UltraChart.ColumnChart.SeriesSpacing = 2;

            //UltraChart.Tooltips.FormatString = "<ITEM_LABEL> <SERIES_LABEL> \n<DATA_VALUE:N3> млн.руб.";

            //UltraChart.Axis.X.Extent = 50;
            //UltraChart.Axis.Y.Extent = 60;

            //UltraChart.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N2>";
            //UltraChart.Axis.Y.Labels.Font = new Font("Verdana", 8);

            //UltraChart.Axis.X.Labels.Font = new Font("Verdana", 8);
            //UltraChart.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.Auto;

            //UltraChart.Legend.Visible = true;
            //UltraChart.Legend.Location = LegendLocation.Bottom;
            //UltraChart.Legend.SpanPercentage = 18;
            //UltraChart.Legend.Margins.Bottom = Convert.ToInt32(UltraChart.Height.Value / 2);

            //UltraChart.TitleLeft.Text = "руб.";
            //UltraChart.TitleLeft.HorizontalAlign = StringAlignment.Center;
            //UltraChart.TitleLeft.Extent = 30;
            //UltraChart.TitleLeft.Margins.Bottom = UltraChart.Axis.X.Extent;
            //UltraChart.TitleLeft.Visible = true;

            ////UltraChart.Data.SwapRowsAndColumns = true;

            //UltraChart.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(UltraChart_FillSceneGraph);

            #endregion

            UltraWebGrid.Width = IsSmallResolution ? CRHelper.GetGridWidth(MinScreenWidth + 20) : CRHelper.GetGridWidth(MinScreenWidth - 30);
            UltraWebGrid.Height = IsSmallResolution ? CRHelper.GetGridHeight(MinScreenHeight - 300) : CRHelper.GetGridHeight(MinScreenHeight - 300);

            UltraGridExporter1.MultiHeader = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);

            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler<DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler<EndExportEventArgs>(PdfExporter_EndExport);

            UltraWebGrid.InitializeRow += new InitializeRowEventHandler(UltraWebGrid_InitializeRow);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

        }
                
        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            DataTable dtDate = new DataTable();
            string query = DataProvider.GetQueryText(String.Format("FO_0002_0017_03_date_nonEmptyCrossJoin"));
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            
            //DateTime date = CRHelper.PeriodDayFoDate(dtDate.Rows[0][1].ToString());
             DateTime date = new DateTime(Convert.ToInt32(dtDate.Rows[0][0]), CRHelper.MonthNum(dtDate.Rows[0][3].ToString()), 1);
             endYear = date.Year;
                        
            FillAdmin();
            FillTools();
            if (!Page.IsPostBack)
            {
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Квартал";
                ComboMonth.Width = 150;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillQuaters());
                ComboMonth.SetСheckedState(String.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(date.Month)), true);
                
                ComboBill.Width = 430;
                ComboBill.Title = "ГРБС";
                ComboBill.FillValues(admin);
                ComboBill.MultiSelect = false;
                ComboBill.SetСheckedState("Департамент агропромышленного комплекса Новосибирской области", true);
                
                ComboToolType.Width = 420;
                ComboToolType.Title = "Тип средств";
                ComboToolType.MultiSelect = true;
                ComboToolType.ParentSelect = true;
                ComboToolType.ShowSelectedValue = true;
                ComboToolType.FillDictionaryValues(tools);
                ComboToolType.SetСheckedState("Бюджетные средства", true);
            }

            string toolType = String.Empty;
            string toolsList = String.Empty;
            if (ComboToolType.SelectedValues.Count == 0)
            {
                selectedTool.Value = "[Тип средств__Сопоставимый].[Тип средств__Сопоставимый].[Все типы средств].[Бюджетные средства]";
            }
            else
            {
                foreach (string group in ComboToolType.SelectedValues)
                {
                    toolType = String.Format("{0},{1}", toolType, toolsDict[group]);
                    toolsList = String.Format("{0}, {1}", toolsList, group);
                }
                toolType = toolType.TrimStart(',');
                toolsList = toolsList.TrimStart(',');

                selectedTool.Value = toolType;
            }

            selectedAdministrator.Value = adminDict[ComboBill.SelectedValue];

            UserParams.PeriodYear.Value = String.Format("[{0}]", ComboYear.SelectedValue);
            UserParams.PeriodQuater.Value = String.Format("[{0}].[Полугодие {1}].[{2}]", ComboYear.SelectedValue, CRHelper.HalfYearNumByQuarterNum(ComboMonth.SelectedIndex + 1), ComboMonth.SelectedValue);
            //UserParams.PeriodMonth.Value = String.Format("[{0}].[Полугодие {1}].[Квартал {2}].[{3}]", ComboYear.SelectedValue, CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1), CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1), ComboMonth.SelectedValue);
            lastYearQuarter.Value = String.Format("[{0}].[Полугодие {1}].[{2}]", Convert.ToInt32(ComboYear.SelectedValue) - 1, CRHelper.HalfYearNumByQuarterNum(ComboMonth.SelectedIndex + 1), ComboMonth.SelectedValue);
            lastYear.Value = String.Format("[{0}]", Convert.ToInt32(ComboYear.SelectedValue) - 1);
            
            string yearInterval;
            if ((ComboMonth.SelectedIndex + 1) != 4)
            {
                date = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.QuarterLastMonth(ComboMonth.SelectedIndex + 1), 1);
                date = date.AddMonths(1);
                yearInterval = String.Format("на {0:dd.MM.yyyy}", date);
            }
            else
            {
                yearInterval = String.Format("за {0} год", ComboYear.SelectedValue);
            }
            if (toolsList == String.Empty)
            {
                toolsList = "Бюджетные средства";
            }

            int quarter = ComboMonth.SelectedIndex + 1;

            int year = Convert.ToInt32(ComboYear.SelectedValue);
            string hancockAdmin = year < 2008 ? "[Роспись]" : "[Кассовый план]";

            string precisionPlanAdmin = string.Empty;
            int i = 1;
            for (; i <= quarter; i++)
            {
                precisionPlanAdmin += String.Format("member measures.[Уточненный план на дату {0} ] as ' Sum ( {{{1}}}, LookupCube ( \"[ФО_АС Бюджет_План расходов]\", \" ( [Measures].[Ассигнования], [Роспись Кассовый план].[Роспись Кассовый план].[Все].{4}, [Роспись Уведомления].[Уточненный план].[Уведомления], [Период__Дата принятия].[Период__Дата принятия].[Данные всех периодов].[{2}], [Период__Период].[Период__Период].[Данные всех периодов].[{2}].[Полугодие {3}].[Квартал {0}], \" + MemberToStr([Расходы__Базовый].CurrentMember ) + \",\" + MemberToStr ([Администратор__Сопоставим].[Администратор__Сопоставим].CurrentMember) + \",\" + MemberToStr      ([Тип средств__Сопоставимый].[Тип средств__Сопоставимый].CurrentMember      ) + \" ,\" + MemberToStr      ([КОСГУ__Сопоставимый].[КОСГУ__Сопоставимый].CurrentMember      ) + \"      )      \"  )   ) ' ", i, selectedTool.Value, ComboYear.SelectedValue, CRHelper.HalfYearNumByQuarterNum(i), hancockAdmin);
            }

            for (; i <= 4; i++)
            {
                precisionPlanAdmin += String.Format("member measures.[Уточненный план на дату {0} ] as 'null' ", i);
            }
            planAdmin.Value = precisionPlanAdmin;

            string precisionPlanLastYearAdmin = string.Empty;

            i = 1;
            for (; i <= quarter; i++)
            {
                precisionPlanLastYearAdmin += String.Format("member measures.[Уточненный план на дату прошлый год {0} ] as ' Sum ( {{{1}}}, LookupCube ( \"[ФО_АС Бюджет_План расходов]\", \" ( [Measures].[Ассигнования], [Роспись Кассовый план].[Роспись Кассовый план].[Все].[Роспись], [Роспись Уведомления].[Уточненный план].[Уведомления], [Период__Дата принятия].[Период__Дата принятия].[Данные всех периодов].[{2}], [Период__Период].[Период__Период].[Данные всех периодов].[{2}].[Полугодие {3}].[Квартал {0}], \" + MemberToStr([Расходы__Базовый].CurrentMember ) + \",\" + MemberToStr ([Администратор__Сопоставим].[Администратор__Сопоставим].CurrentMember) + \",\" + MemberToStr      ([Тип средств__Сопоставимый].[Тип средств__Сопоставимый].CurrentMember      ) + \" ,\" + MemberToStr      ([КОСГУ__Сопоставимый].[КОСГУ__Сопоставимый].CurrentMember      ) + \"      )      \"  )   ) ' ", i, selectedTool.Value, Convert.ToInt32(ComboYear.SelectedValue) - 1, CRHelper.HalfYearNumByQuarterNum(i));
            }

            for (; i <= 4; i++)
            {
                precisionPlanLastYearAdmin += String.Format("member measures.[Уточненный план на дату прошлый год {0} ] as 'null' ", i);
            }
            planLastYearAdmin.Value = precisionPlanLastYearAdmin;

            plan.Value = year < 2008 ? "[Роспись]" : "[Кассовый план]";
            planLastYear.Value = (year - 1) < 2008 ? "[Роспись]" : "[Кассовый план]";

            Page.Title = "Динамика исполнения областного бюджета";
            PageTitle.Text = "Динамика исполнения областного бюджета в соответствии с ведомственной структурой расходов";
            PageSubTitle.Text = String.Format("Динамика исполнения областного бюджета {0}, руб.<br/>ГРБС: {1}.<br/>Тип средств: {2}.", yearInterval.ToLower(), ComboBill.SelectedValue.ToLower(), toolsList.ToLower());

            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        private Collection<string> importedRows = new Collection<string>();
        private Collection<string> importedUncollationRows = new Collection<string>();

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
          
            string query = DataProvider.GetQueryText(String.Format("FO_0002_0017_03_Grid1"));
            DataTable dtGridFact = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGridFact);
            dtGridFact.PrimaryKey = new DataColumn[] { dtGridFact.Columns[0], dtGridFact.Columns[2], dtGridFact.Columns[3], dtGridFact.Columns[4], dtGridFact.Columns[5], dtGridFact.Columns[6] };
           
            query = DataProvider.GetQueryText(String.Format("FO_0002_0017_03_Grid2"));
            DataTable dtGridPlan = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGridPlan);
            dtGridPlan.PrimaryKey = new DataColumn[] { dtGridPlan.Columns[0], dtGridPlan.Columns[2], dtGridPlan.Columns[3], dtGridPlan.Columns[4], dtGridPlan.Columns[5], dtGridPlan.Columns[6] };

            query = DataProvider.GetQueryText(String.Format("FO_0002_0017_03_GridTotal"));
            DataTable dtGridTotal = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGridTotal);
            dtGridTotal.Rows.RemoveAt(0);

            query = DataProvider.GetQueryText(String.Format("FO_0002_0017_03_GridAdmin"));
            DataTable dtGridAdmin = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGridAdmin);
            dtGridTotal.ImportRow(dtGridAdmin.Rows[1]);

            query = DataProvider.GetQueryText(String.Format("FO_0002_0017_03_Grid1_6"));
            DataTable dtGridFact6 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGridFact6);
            dtGridFact6.PrimaryKey = new DataColumn[] { dtGridFact6.Columns[0], dtGridFact6.Columns[1], dtGridFact6.Columns[2], dtGridFact6.Columns[3], dtGridFact6.Columns[4], dtGridFact6.Columns[5], dtGridFact6.Columns[6], dtGridFact6.Columns[7] };
            
            query = DataProvider.GetQueryText(String.Format("FO_0002_0017_03_Grid2_6"));
            DataTable dtGridPlan6 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель", dtGridPlan6);
            dtGridPlan6.PrimaryKey = new DataColumn[] { dtGridPlan6.Columns[0], dtGridPlan6.Columns[1], dtGridPlan6.Columns[2], dtGridPlan6.Columns[3], dtGridPlan6.Columns[4], dtGridPlan6.Columns[5], dtGridPlan6.Columns[6], dtGridPlan6.Columns[7] };

            foreach (DataRow row in dtGridPlan.Rows)
            {
                if (row[0] != DBNull.Value && row[0].ToString() != string.Empty)
                {
                    // во втором гриде выборка будет меньше, добавляем по нему
                    DataRow importingRow = dtGridTotal.NewRow();

                    foreach (DataColumn col in dtGridPlan.Columns)
                    {
                        importingRow[col.ColumnName] = row[col.ColumnName];
                    }

                    string rowId =
                        String.Format("{0}{1}{2}{3}{4}{5}", TrimName(row[0].ToString()), row[2], row[3], row[4], row[5],
                                      row[6]);

                    if (!importedRows.Contains(rowId))
                    {
                        importedRows.Add(rowId);
                        string[] rowName =
                            new string[]
                                {
                                    row[0].ToString(), row[2].ToString(), row[3].ToString(), row[4].ToString(),
                                    row[5].ToString(), row[6].ToString()
                                };
                        DataRow factRow = dtGridFact.Rows.Find(rowName);

                        if (factRow != null)
                        {
                            foreach (DataColumn col in dtGridFact.Columns)
                            {
                                importingRow[col.ColumnName] = factRow[col.ColumnName];
                            }

                            if (row["Уточненный план "] != DBNull.Value &&
                                row["Уточненный план "].ToString() != string.Empty &&
                                factRow["Факт расходов "] != DBNull.Value &&
                                factRow["Факт расходов "].ToString() != string.Empty &&
                                Convert.ToDouble(row["Уточненный план "].ToString()) != 0)
                            {
                                importingRow["% исполнения "] = Convert.ToDouble(factRow["Факт расходов "].ToString())/
                                                                Convert.ToDouble(row["Уточненный план "].ToString());
                            }

                            if (factRow["Факт расходов "] != DBNull.Value &&
                                factRow["Факт расходов "].ToString() != string.Empty)
                            {
                                importingRow["Остаток ассигнований "] =
                                    Convert.ToDouble(row["Уточненный план "].ToString()) -
                                    Convert.ToDouble(factRow["Факт расходов "].ToString());
                            }
                            else
                            {
                                importingRow["Остаток ассигнований "] =
                                    Convert.ToDouble(row["Уточненный план "].ToString());
                            }
                        }
                        else
                        {
                            if (row["Уточненный план "] != DBNull.Value &&
                                row["Уточненный план "].ToString() != string.Empty)
                            {
                                importingRow["Остаток ассигнований "] =
                                    Convert.ToDouble(row["Уточненный план "].ToString());
                            }
                        }

                        dtGridTotal.Rows.Add(importingRow);

                        if (row[1].ToString() == "Несопоставленные данные")
                        {
                            query = DataProvider.GetQueryText(String.Format("FO_0002_0017_03_1_GridUncollation"));
                            DataTable dtGridUncollationFact = new DataTable();
                            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель",
                                                                                             dtGridUncollationFact);
                            dtGridUncollationFact.PrimaryKey =
                                new DataColumn[]
                                    {
                                        dtGridUncollationFact.Columns[0], dtGridUncollationFact.Columns[1],
                                        dtGridUncollationFact.Columns[2], dtGridUncollationFact.Columns[3],
                                        dtGridUncollationFact.Columns[4], dtGridUncollationFact.Columns[5],
                                        dtGridUncollationFact.Columns[6], dtGridUncollationFact.Columns[11]
                                    };

                            query = DataProvider.GetQueryText(String.Format("FO_0002_0017_03_2_GridUncollation"));
                            DataTable dtGridUncollationPlan = new DataTable();
                            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель",
                                                                                             dtGridUncollationPlan);
                            dtGridUncollationPlan.PrimaryKey =
                                new DataColumn[]
                                    {
                                        dtGridUncollationPlan.Columns[0], dtGridUncollationPlan.Columns[1],
                                        dtGridUncollationPlan.Columns[2], dtGridUncollationPlan.Columns[3],
                                        dtGridUncollationPlan.Columns[4], dtGridUncollationPlan.Columns[5],
                                        dtGridUncollationPlan.Columns[6], dtGridUncollationPlan.Columns[12]
                                    };

                            query = DataProvider.GetQueryText(String.Format("FO_0002_0017_03_1_6_GridUncollation"));
                            DataTable dtGridUncollationFact6 = new DataTable();
                            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель",
                                                                                             dtGridUncollationFact6);
                            dtGridUncollationFact6.PrimaryKey =
                                new DataColumn[]
                                    {
                                        dtGridUncollationFact6.Columns[0], dtGridUncollationFact6.Columns[1],
                                        dtGridUncollationFact6.Columns[2], dtGridUncollationFact6.Columns[3],
                                        dtGridUncollationFact6.Columns[4], dtGridUncollationFact6.Columns[5],
                                        dtGridUncollationFact6.Columns[6], dtGridUncollationFact6.Columns[7]
                                    };

                            query = DataProvider.GetQueryText(String.Format("FO_0002_0017_03_2_6_GridUncollation"));
                            DataTable dtGridUncollationPlan6 = new DataTable();
                            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатель",
                                                                                             dtGridUncollationPlan6);
                            dtGridUncollationPlan6.PrimaryKey =
                                new DataColumn[]
                                    {
                                        dtGridUncollationPlan6.Columns[0], dtGridUncollationPlan6.Columns[1],
                                        dtGridUncollationPlan6.Columns[2], dtGridUncollationPlan6.Columns[3],
                                        dtGridUncollationPlan6.Columns[4], dtGridUncollationPlan6.Columns[5],
                                        dtGridUncollationPlan6.Columns[6],  dtGridUncollationPlan6.Columns[7]
                                    };

                            foreach (DataRow uncollationRow in dtGridUncollationPlan.Rows)
                            {
                                string uncollationRowId =
                                    String.Format("{0}{1}{2}{3}{4}{5}", TrimName(uncollationRow[0].ToString()),
                                                  uncollationRow[2], uncollationRow[3], uncollationRow[4],
                                                  uncollationRow[5],
                                                  uncollationRow[6]);

                                if (!importedUncollationRows.Contains(uncollationRowId))
                                {
                                    importedUncollationRows.Add(uncollationRowId);

                                    if (uncollationRow[0] != DBNull.Value &&
                                        uncollationRow[0].ToString() != string.Empty)
                                    {
                                        // во втором гриде выборка будет меньше, добавляем по нему

                                        DataRow importingChildRow = dtGridTotal.NewRow();

                                        foreach (DataColumn col in dtGridUncollationPlan.Columns)
                                        {
                                            importingChildRow[col.ColumnName] = uncollationRow[col.ColumnName];
                                        }

                                        string[] uncollationRowName =
                                            new string[]
                                                {
                                                    uncollationRow[0].ToString(), uncollationRow[1].ToString(),
                                                    uncollationRow[2].ToString(),
                                                    uncollationRow[3].ToString(), uncollationRow[4].ToString(),
                                                    uncollationRow[5].ToString(), uncollationRow[6].ToString(),
                                                    uncollationRow[12].ToString()
                                                };

                                        DataRow fact6Row = dtGridUncollationFact.Rows.Find(uncollationRowName);


                                        if (fact6Row != null)
                                        {
                                            foreach (DataColumn col in dtGridFact6.Columns)
                                            {
                                                importingChildRow[col.ColumnName] = fact6Row[col.ColumnName];
                                            }

                                            if (uncollationRow["Уточненный план "] != DBNull.Value &&
                                                uncollationRow["Уточненный план "].ToString() != string.Empty &&
                                                fact6Row["Факт расходов "] != DBNull.Value &&
                                                fact6Row["Факт расходов "].ToString() != string.Empty &&
                                                Convert.ToDouble(uncollationRow["Уточненный план "].ToString()) != 0)
                                            {
                                                importingChildRow["% исполнения "] =
                                                    Convert.ToDouble(fact6Row["Факт расходов "].ToString())/
                                                    Convert.ToDouble(uncollationRow["Уточненный план "].ToString());
                                            }

                                            if (fact6Row["Факт расходов "] != DBNull.Value &&
                                                fact6Row["Факт расходов "].ToString() != string.Empty)
                                            {
                                                importingChildRow["Остаток ассигнований "] =
                                                    Convert.ToDouble(uncollationRow["Уточненный план "].ToString()) -
                                                    Convert.ToDouble(fact6Row["Факт расходов "].ToString());
                                            }
                                            else
                                            {
                                                importingChildRow["Остаток ассигнований "] =
                                                    Convert.ToDouble(uncollationRow["Уточненный план "].ToString());
                                            }
                                        }
                                        else if (uncollationRow["Уточненный план "] != DBNull.Value &&
                                                 uncollationRow["Уточненный план "].ToString() != string.Empty)
                                        {
                                            importingChildRow["Остаток ассигнований "] =
                                                Convert.ToDouble(uncollationRow["Уточненный план "].ToString());
                                        }

                                        dtGridTotal.Rows.Add(importingChildRow);

                                        if (importingChildRow["ВР"] != DBNull.Value &&
                                            importingChildRow["ВР"].ToString() != String.Empty &&
                                            importingChildRow["ВР"].ToString() != "0")
                                        {
                                            ImportChildRecords(dtGridTotal, dtGridUncollationFact6, dtGridUncollationPlan6, importingChildRow);

                                            dtGridUncollationPlan6RowsCount = dtGridUncollationPlan6.Rows.Count;
                                        }
                                    }
                                }
                            }
                        }
                        // Если появляется квр, пытаемся импортировать еще из других таблиц
                        if (importingRow["ВР"] != DBNull.Value &&
                            importingRow["ВР"].ToString() != String.Empty &&
                            importingRow["ВР"].ToString() != "0")
                        {
                            ImportChildRecords(dtGridTotal, dtGridFact6, dtGridPlan6, importingRow);
                            
                        }
                    }
                }
            }

            dtGridTotal.Columns.RemoveAt(0);
            
            dtGrid.AcceptChanges();
            dtGridTotal.AcceptChanges();
            ((UltraWebGrid)sender).DataSource = dtGridTotal;
        }

        private void ImportChildRecords(DataTable dtGridAdmin, DataTable dtGridFact6, DataTable dtGridPlan6, DataRow importingRow)
        {
            DataRow[] plan6rows =
                dtGridPlan6.Select(
                    String.Format("ВР='{0}' and ЦСР='{1}' and РЗ='{2}' and ПР='{3}'", importingRow["ВР"],
                                  importingRow["ЦСР"], importingRow["РЗ"], importingRow["ПР"]));

            foreach (DataRow row6 in plan6rows)
            {
                if (row6[0] != DBNull.Value && row6[0].ToString() != string.Empty)
                {
                    // во втором гриде выборка будет меньше, добавляем по нему

                    DataRow importingChildRow = dtGridAdmin.NewRow();

                    foreach (DataColumn col in dtGridPlan6.Columns)
                    {
                        importingChildRow[col.ColumnName] = row6[col.ColumnName];
                    }

                    string[] row6Name =
                        new string[]
                            {
                                row6[0].ToString(), row6[1].ToString(), row6[2].ToString(),
                                row6[3].ToString(), row6[4].ToString(),
                                row6[5].ToString(), row6[6].ToString(), row6[7].ToString()
                            };

                    DataRow fact6Row = dtGridFact6.Rows.Find(row6Name);


                    if (fact6Row != null)
                    {
                        foreach (DataColumn col in dtGridFact6.Columns)
                        {
                            importingChildRow[col.ColumnName] = fact6Row[col.ColumnName];
                        }

                        if (row6["Уточненный план "] != DBNull.Value &&
                            row6["Уточненный план "].ToString() != string.Empty &&
                            fact6Row["Факт расходов "] != DBNull.Value &&
                            fact6Row["Факт расходов "].ToString() != string.Empty &&
                            Convert.ToDouble(row6["Уточненный план "].ToString()) != 0)
                        {
                            importingChildRow["% исполнения "] =
                                Convert.ToDouble(fact6Row["Факт расходов "].ToString())/
                                Convert.ToDouble(row6["Уточненный план "].ToString());
                        }

                        if (fact6Row["Факт расходов "] != DBNull.Value &&
                            fact6Row["Факт расходов "].ToString() != string.Empty)
                        {
                            importingChildRow["Остаток ассигнований "] =
                                Convert.ToDouble(row6["Уточненный план "].ToString()) -
                                Convert.ToDouble(fact6Row["Факт расходов "].ToString());
                        }
                        else
                        {
                            importingChildRow["Остаток ассигнований "] =
                                Convert.ToDouble(row6["Уточненный план "].ToString());
                        }
                    }
                    else if (row6["Уточненный план "] != DBNull.Value &&
                             row6["Уточненный план "].ToString() != string.Empty)
                    {
                        importingChildRow["Остаток ассигнований "] =
                            Convert.ToDouble(row6["Уточненный план "].ToString());
                    }

                    dtGridAdmin.Rows.Add(importingChildRow);
                }
            }
        }

        private int dtGridUncollationPlan6RowsCount = 0;

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            int start = Environment.TickCount;
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.No;
            e.Layout.FilterOptionsDefault.AllowRowFiltering = RowFiltering.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
           // e.Layout.Bands[0].Columns[0].MergeCells = true;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            //e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;

            //e.Layout.Bands[0].Columns[0].Header.Caption = "Расходы";
            //e.Layout.Bands[0].Columns[1].Header.Caption = "КОСГУ";

            CRHelper.SetHeaderCaption(UltraWebGrid,0,1,"ГРБС","Код главного распорядителя бюджетных средств");
            CRHelper.SetHeaderCaption(UltraWebGrid, 0, 2, "Рз", "Код раздела расходов"  );
            CRHelper.SetHeaderCaption(UltraWebGrid, 0, 3, "Пр", "Код подраздела расходов" );
            CRHelper.SetHeaderCaption(UltraWebGrid, 0, 4, "ЦСР", "Код целевой статьи расходов" );
            CRHelper.SetHeaderCaption(UltraWebGrid, 0, 5, "ВР", "Код вида расходов" );
            CRHelper.SetHeaderCaption(UltraWebGrid, 0, 6, "КОСГУ", "Код классификации операций сектора государственного управления");
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(165, 1280);
        //    e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(165, 1280);
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(50, 1280);
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(30, 1280);
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(30, 1280);
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(60, 1280);
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(40, 1280);
            e.Layout.Bands[0].Columns[6].Width = CRHelper.GetColumnWidth(60, 1280);

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "000");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "00");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "00");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "0000000");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "000");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "000");

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[11], "P2");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[12], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[13], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[14], "N0");
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[15], "P2");

            e.Layout.Bands[0].Columns[7].Width = CRHelper.GetColumnWidth(130, 1280);
            e.Layout.Bands[0].Columns[8].Width = CRHelper.GetColumnWidth(130, 1280);
            e.Layout.Bands[0].Columns[9].Width = CRHelper.GetColumnWidth(130, 1280);
            e.Layout.Bands[0].Columns[10].Width = CRHelper.GetColumnWidth(130, 1280);
            e.Layout.Bands[0].Columns[11].Width = CRHelper.GetColumnWidth(100, 1280);
            e.Layout.Bands[0].Columns[12].Width = CRHelper.GetColumnWidth(130, 1280);
            e.Layout.Bands[0].Columns[13].Width = CRHelper.GetColumnWidth(130, 1280);
            e.Layout.Bands[0].Columns[14].Width = CRHelper.GetColumnWidth(130, 1280);
            e.Layout.Bands[0].Columns[15].Width = CRHelper.GetColumnWidth(100, 1280);

            e.Layout.Bands[0].Columns[16].Hidden = true;
        }

        private int uncollationRows = 0;
        private bool uncollationStarted = false;

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (uncollationStarted)
            {

                uncollationRows++;
                SetUncolationRowBackColor(e);
                if (uncollationRows == importedUncollationRows.Count + dtGridUncollationPlan6RowsCount)
                {
                    for (int i = 0; i < e.Row.Cells.Count; i++)
                    {
                        e.Row.Cells[i].Style.BorderDetails.WidthBottom = 2;
                        e.Row.Cells[i].Style.BorderDetails.ColorBottom = Color.Black;
                      //  e.Row.Cells[i].Style.BackColor = Color.FromArgb(253, 252, 165);
                    }
                    uncollationStarted = false;
                    
                }
            }

            if (e.Row.Index == 0 ||
                e.Row.Index == 1)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[2].Value = String.Empty;
                    e.Row.Cells[3].Value = String.Empty;
                    e.Row.Cells[4].Value = String.Empty;
                    e.Row.Cells[5].Value = String.Empty;
                    e.Row.Cells[6].Value = String.Empty;

                    if (i == 1 || i == 2 || i == 3 || i == 4 || i == 5 || i == 6)
                    {
                        
                    }
                    else
                    {
                        e.Row.Cells[i].Style.Font.Size = 10;
                        e.Row.Cells[i].Style.Font.Bold = true;
                    }
                }
            }

            if (e.Row.Cells[0].Value.ToString() == "Несопоставленные данные")
            {
                SetUncolationRowBackColor(e);
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].Style.BorderDetails.WidthTop = 2;
                    e.Row.Cells[i].Style.BorderDetails.ColorTop = Color.Black;
                    uncollationStarted = true;
                }
            }


            if (e.Row.Cells[6].Value != null && e.Row.Cells[6].Value.ToString() != string.Empty && e.Row.Cells[6].Value.ToString() != "0")
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (i == 1 || i == 2 || i == 3 || i == 4 || i == 5 || i == 6)
                    {
                        // DoNothing();
                    }
                    else
                    {
                        e.Row.Cells[i].Style.Font.Size = 8;
                        e.Row.Cells[i].Style.Font.Bold = false;
                    }
                }
                return;
            }
            if (e.Row.Cells[e.Row.Cells.Count - 1].Value != null && e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString() != string.Empty)
            {
                string level = e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString();
                int fontSize = 10;
                bool bold = true;
                bool italic = false;
                if (uncollationStarted)
                {
                    switch (level)
                    {
                        case "1":
                        case "3":
                            {
                                e.Row.Cells[3].Value = String.Empty;
                                e.Row.Cells[4].Value = String.Empty;
                                e.Row.Cells[5].Value = String.Empty;
                                fontSize = 9;
                                bold = true;
                                italic = false;

                                break;
                            }
                        case "4":
                            {
                                e.Row.Cells[4].Value = String.Empty;
                                e.Row.Cells[5].Value = String.Empty;
                                fontSize = 9;
                                bold = false;
                                italic = false;
                                break;
                            }
                        case "5":
                            {
                                e.Row.Cells[5].Value = String.Empty;
                                fontSize = 9;
                                bold = false;
                                italic = false;
                                break;
                            }
                        case "6":
                            {
                                e.Row.Cells[5].Value = String.Empty;
                                fontSize = 9;
                                bold = false;
                                italic = false;
                                break;
                            }
                        case "7":
                            {
                                e.Row.Cells[5].Value = String.Empty;
                                fontSize = 9;
                                bold = false;
                                italic = false;
                                break;
                                ;
                            }
                        case "8":
                            {
                                fontSize = 9;
                                bold = false;
                                italic = false;
                                break;
                            }
                    }
                }
                else
                {
                    switch (level)
                    {
                        case "1":
                            {
                                e.Row.Cells[3].Value = String.Empty;
                                e.Row.Cells[4].Value = String.Empty;
                                e.Row.Cells[5].Value = String.Empty;
                                fontSize = 9;
                                bold = true;
                                italic = false;

                                break;
                            }
                        case "2":
                            {
                                e.Row.Cells[4].Value = String.Empty;
                                e.Row.Cells[5].Value = String.Empty;
                                fontSize = 9;
                                bold = false;
                                italic = false;
                                break;
                            }
                        case "3":
                            {
                                e.Row.Cells[5].Value = String.Empty;
                                fontSize = 9;
                                bold = false;
                                italic = false;
                                break;
                            }
                        case "4":
                            {
                                e.Row.Cells[5].Value = String.Empty;
                                fontSize = 9;
                                bold = false;
                                italic = false;
                                break;
                            }
                        case "5":
                            {
                                e.Row.Cells[5].Value = String.Empty;
                                fontSize = 9;
                                bold = false;
                                italic = false;
                                break;
                                ;
                            }
                        case "6":
                            {
                                fontSize = 9;
                                bold = false;
                                italic = false;
                                break;
                            }
                    }
                }

                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (i == 1 || i == 2 || i == 3 || i == 4 || i == 5 || i == 6)
                    {
                        // DoNothing();
                    }
                    else
                    {
                        e.Row.Cells[i].Style.Font.Size = fontSize;
                        e.Row.Cells[i].Style.Font.Bold = bold;
                    }
                }
            }
            
            e.Row.Cells[0].Value = TrimName(e.Row.Cells[0].Value.ToString());
        }

        private static void SetUncolationRowBackColor(RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Style.BackColor = Color.FromArgb(255, 255, 220);
            }
        }

        private static string TrimName(string name)
        {
            while (Char.IsDigit(name[0]))
            {
                name = name.Remove(0, 1);
            }
            return name;
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {

        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.Rows[0].Cells[0].Value = PageTitle.Text.Replace("<br/>", " ") + " " + PageSubTitle.Text.Replace("<br/>", " ");
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {
            int width = 200;
            e.CurrentWorksheet.Columns[0].Width = 250 * 37;
            e.CurrentWorksheet.Columns[1].Width = 50 * 37;
            e.CurrentWorksheet.Columns[2].Width = 50 * 37;
            e.CurrentWorksheet.Columns[3].Width = 50 * 37;
            e.CurrentWorksheet.Columns[4].Width = 50 * 37;
            e.CurrentWorksheet.Columns[5].Width = 50 * 37;
            e.CurrentWorksheet.Columns[6].Width = 50 * 37;
            e.CurrentWorksheet.Columns[7].Width = width * 37;
            e.CurrentWorksheet.Columns[8].Width = width * 37;
            e.CurrentWorksheet.Columns[9].Width = width * 37;
            e.CurrentWorksheet.Columns[10].Width = width * 37;
            e.CurrentWorksheet.Columns[11].Width = width * 37;
            e.CurrentWorksheet.Columns[12].Width = width * 37;
            e.CurrentWorksheet.Columns[13].Width = width * 37;
            e.CurrentWorksheet.Columns[14].Width = width * 37;

            e.CurrentWorksheet.Columns[1].CellFormat.FormatString = "000";
            e.CurrentWorksheet.Columns[2].CellFormat.FormatString = "00";
            e.CurrentWorksheet.Columns[3].CellFormat.FormatString = "00";
            e.CurrentWorksheet.Columns[4].CellFormat.FormatString = "0000000";
            e.CurrentWorksheet.Columns[5].CellFormat.FormatString = "000";
            e.CurrentWorksheet.Columns[6].CellFormat.FormatString = "000";

            e.CurrentWorksheet.Columns[7].CellFormat.FormatString = "#,##0";
            e.CurrentWorksheet.Columns[8].CellFormat.FormatString = "#,##0";
            e.CurrentWorksheet.Columns[9].CellFormat.FormatString = "#,##0";
            e.CurrentWorksheet.Columns[10].CellFormat.FormatString = "#,##0";
            e.CurrentWorksheet.Columns[11].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;
            e.CurrentWorksheet.Columns[12].CellFormat.FormatString = "#,##0";
            e.CurrentWorksheet.Columns[13].CellFormat.FormatString = "#,##0";
            e.CurrentWorksheet.Columns[14].CellFormat.FormatString = "#,##0";
            e.CurrentWorksheet.Columns[15].CellFormat.FormatString = UltraGridExporter.ExelPercentFormat;

            // расставляем стили у ячеек
            for (int i = 4; i < UltraWebGrid.Rows.Count + 4; i ++)
            {
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[i].Height = 20 * 37;
            }
            int columnCount = UltraWebGrid.Columns.Count;
            int rowsCount = UltraWebGrid.Rows.Count;

            // расставляем стили у ячеек
            for (int i = 1; i < 5; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[i].Cells[j].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                }
            }

            //// расставляем стили у начальных колонок
            //for (int i = 4; i < rowsCount; i++)
            //{
            //    e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
            //    e.CurrentWorksheet.Rows[i].Cells[0].CellFormat.Alignment = HorizontalCellAlignment.Center;

            //    e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.VerticalAlignment = VerticalCellAlignment.Top;
            //    e.CurrentWorksheet.Rows[i].Cells[1].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            //    e.CurrentWorksheet.Rows[i].Height = 20 * 37;
            //}

           
            //// расставляем стили у ячеек
            for (int i = 1; i < 7; i += 1)
            {
                for (int j = 4; j < rowsCount + 4; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Center;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                }
            }

            //// расставляем стили у ячеек
            for (int i = 7; i < columnCount; i += 1)
            {
                for (int j = 4; j < rowsCount + 4; j++)
                {
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
                    e.CurrentWorksheet.Rows[j].Cells[i].CellFormat.VerticalAlignment = VerticalCellAlignment.Center;
                }
            }
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;

            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid);
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
            e.HeaderText = UltraWebGrid.DisplayLayout.Bands[0].Columns[e.CurrentColumnIndex].Header.Key.Split(';')[0];
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, DocumentExportEventArgs e)
        {
            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 16);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
            title.AddContent(PageTitle.Text.Replace("<br/>", " "));

            title = e.Section.AddText();
            font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.AddContent(PageSubTitle.Text.Replace("<br/>", " "));
        }

        private void PdfExporter_EndExport(object sender, EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);

        }

        #endregion

        Dictionary<string, int> tools = new Dictionary<string, int>();
        private Dictionary<string, string> toolsDict = new Dictionary<string, string>();

        private void FillTools()
        {
            DataTable dtTools = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0017_03_toolType");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtTools);

            foreach (DataRow row in dtTools.Rows)
            {
                string key = GetDictionaryUniqueKey(tools, row[0].ToString());
                tools.Add(key, Convert.ToInt32(row[1]) - 1);
                toolsDict.Add(key, row[2].ToString());
            }
        }

        private string GetDictionaryUniqueKey(Dictionary<string, int> dictionary, string key)
        {
            string newKey = key;
            while (dictionary.ContainsKey(newKey))
            {
                newKey += " ";
            }
            return newKey;
        }

        private Collection<string> admin = new Collection<string>();
        private Dictionary<string, string> adminDict = new Dictionary<string, string>();

        private void FillAdmin()
        {
            DataTable dtAdmin = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0017_03_adminType");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtAdmin);

            foreach (DataRow row in dtAdmin.Rows)
            {
                admin.Add(String.Format("({0}) {1}", row[1], row[0]));
                adminDict.Add(String.Format("({0}) {1}", row[1], row[0]), row[0].ToString());
            }
        }
    }
}