using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Dundas.Maps.WebControl;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Image = Infragistics.Documents.Reports.Graphics.Image;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.UltraChart.Core;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Index;
using Infragistics.Documents.Reports.Report.TOC;
using Infragistics.Documents.Reports.Report.Tree;
using Infragistics.Documents.Reports.Report.Flow;
using Infragistics.Documents.Reports.Report.Grid;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.QuickTable;
using Infragistics.Documents.Reports.Report.QuickList;
using Infragistics.Documents.Reports.Report.QuickText;
using Infragistics.Documents.Reports.Report.Segment;
using Infragistics.Documents.Reports.Report.Band;
using System.IO;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs;
using InitializeRowEventHandler = Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler;
using SerializationFormat = Dundas.Maps.WebControl.SerializationFormat;
using System.Globalization;
using Infragistics.Documents.Reports.Report.List;
using System.Drawing;
using System.Collections;

namespace Krista.FM.Server.Dashboards.reports.FNS_0001_0007
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid;
        private DataTable dtComments;
        private CustomParam Year;
        private CustomParam regionChart;
        private CustomParam mul;
        private CustomParam dec;
        private CustomParam nMonth;
        private CustomParam incomesTotal;
        private CustomParam selectedPeriod;
        private CustomParam regType;
        private CustomParam measStr;
        #region Параметры запроса

        // уровень бюджета
        private CustomParam budgetLevel;
        // группа доходов
        private CustomParam fnsKDGroup;
        private CustomParam selectedRegion;
        Boolean hideRang = false;

        // уровень МР и ГО
        private CustomParam regionsLevel;
        // уровень поселений
        private CustomParam settlementLevel;

        #endregion

        private bool IsSmallResolution
        {
            get { return CRHelper.GetScreenWidth < 1200; }
        }

        private int minScreenWidth
        {
            get { return IsSmallResolution ? 850 : CustomReportConst.minScreenWidth; }
        }

        private int minScreenHeight
        {
            get { return IsSmallResolution ? 700 : CustomReportConst.minScreenHeight; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            int start = Environment.TickCount;
            base.Page_PreLoad(sender, e);
           
            if (Year == null)
            {
                Year = UserParams.CustomParam("Year");
            }

            if (regionChart == null)
            {
                regionChart = UserParams.CustomParam("regionChart");
            }
            if (selectedRegion == null)
            {
                selectedRegion =  UserParams.CustomParam("selectedRegion");
            }
            if (mul == null)
            {
                mul = UserParams.CustomParam("mul");
            }
            if (regType == null)
            {
                regType = UserParams.CustomParam("regType");
            }
            if (dec == null)
            {
                dec = UserParams.CustomParam("dec");
            }
            if (budgetLevel == null)
            {
                budgetLevel = UserParams.CustomParam("budget_level");
            }
            if (fnsKDGroup == null)
            {
                fnsKDGroup = UserParams.CustomParam("fns_kd_group");
            }
            if (nMonth == null)
            {
                nMonth = UserParams.CustomParam("nMonth");
            }
            if (measStr == null)
            {
                measStr = UserParams.CustomParam("measStr");
            }
            if (incomesTotal == null)
            {
                incomesTotal = UserParams.CustomParam("incomes_total");
            }
            if (selectedPeriod == null)
            {
                selectedPeriod = UserParams.CustomParam("selectedPeriod");
            }

            regionsLevel = UserParams.CustomParam("regions_level");
            settlementLevel = UserParams.CustomParam("settlement_level");

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth -20);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.40);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraGridExporter1.Visible = true;
            UltraGridExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            UltraGridExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            UltraGridExporter1.ExcelExporter.EndExport += new EndExportEventHandler(ExcelExporter_EndExport);
            UltraGridExporter1.ExcelExporter.HeaderCellExporting += new HeaderCellExportingEventHandler(ExcelExporter_HeaderCellExporting);
            UltraGridExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            UltraGridExporter1.PdfExporter.BeginExport += new EventHandler
                    <Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs>(PdfExporter_BeginExport);
            UltraGridExporter1.PdfExporter.EndExport += new EventHandler
                <Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
            CRHelper.SaveToUserAgentLog(String.Format("Остальной прелоад {0}", Environment.TickCount - start));
            UltraGridExporter1.HeaderChildCellHeight = 100;
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.95));
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight * 0.70);
            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL> за {0} год\n<DATA_VALUE:N1> тыс.руб.", ComboYear.SelectedValue);
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Axis.X.Extent = 210;
            UltraChart1.TitleLeft.HorizontalAlign = System.Drawing.StringAlignment.Center;
            UltraChart1.TitleLeft.Margins.Bottom = UltraChart1.Axis.X.Extent;
            UltraChart1.Axis.X.Labels.Visible = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.X.Labels.ItemFormatString = " ";
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Bottom;
            UltraChart1.Legend.SpanPercentage = 12;
            UltraChart1.Legend.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 15);
            UltraChart1.Axis.Y.Labels.SeriesLabels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Font = new System.Drawing.Font("Verdana", 15);
            UltraChart1.Axis.Y.Labels.Font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.Axis.X.Labels.Layout.Behavior = AxisLabelLayoutBehaviors.None;
            UltraChart1.Axis.X.Labels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.LineChart.NullHandling = NullHandling.DontPlot;
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";
        }

        void UltraChart_InterpolateValues(object sender, InterpolateValuesEventArgs e)
        {
        
        }
        
        string units = string.Empty;
        protected override void Page_Load(object sender, EventArgs e)
        {

            int start = Environment.TickCount;
            base.Page_Load(sender, e);
            CRHelper.SaveToUserAgentLog(String.Format("Базовый лоад {0}", Environment.TickCount - start));

            start = Environment.TickCount;
            UserParams.IncomesKDSocialNeedsTax.Value = RegionSettingsHelper.Instance.IncomesKDSocialNeedsTax;
            UserParams.IncomesKDReturnOfRemains.Value = RegionSettingsHelper.Instance.IncomesKDReturnOfRemains;
            UserParams.IncomesKD11800000000000000.Value = RegionSettingsHelper.Instance.IncomesKD11800000000000000;
            UserParams.IncomesKDRootName.Value = RegionSettingsHelper.Instance.IncomesKDRootName;
            UserParams.IncomesKDDimension.Value = RegionSettingsHelper.Instance.IncomesKDDimension;
            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;
            UserParams.IncomesKDAllLevel.Value = RegionSettingsHelper.Instance.IncomesKDAllLevel;
            UserParams.FNSOKVEDGovernment.Value = RegionSettingsHelper.Instance.FNSOKVEDGovernment;
            UserParams.FNSOKVEDHousehold.Value = RegionSettingsHelper.Instance.FNSOKVEDHousehold;
            incomesTotal.Value = RegionSettingsHelper.Instance.IncomeTotal;

            regionsLevel.Value = RegionSettingsHelper.Instance.RegionsLevel;
            settlementLevel.Value = RegionSettingsHelper.Instance.SettlementLevel;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string queryName = "FNS_0001_0007_date";
                string query = DataProvider.GetQueryText(queryName);
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                int endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string endmonth = dtDate.Rows[0][3].ToString();

                int firstYear = (RegionSettingsHelper.Instance.Name == "Ярославская область") ? 2008: 2006;
                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(endmonth, true);
                
                ComboBudgetLevel.Title = "Бюджет";
                ComboBudgetLevel.Width = 220;
                ComboBudgetLevel.MultiSelect = false;
                ComboBudgetLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillBudgetLevels(
                        DataDictionariesHelper.BudgetLevelTypes)
                       );
                ComboBudgetLevel.SetСheckedState("Консолидированный бюджет субъекта", true);

                ComboRegion.Title = "Территория";
                ComboRegion.Width = 280;
                ComboRegion.MultiSelect = false;
                ComboRegion.FillDictionaryValues(CustomMultiComboDataHelper.FillLocalBudgets(RegionsNamingHelper.LocalBudgetTypes));
                ComboRegion.RemoveTreeNodeByName("Собственный бюджет субъекта");
                ComboRegion.RemoveTreeNodeByName("Местные бюджеты");
                ComboRegion.RenameTreeNodeByName("Консолидированный бюджет субъекта","Все территории");
                ComboRegion.SetСheckedState("Все территории",true);
           

                ComboKD.Width = 350;
                ComboKD.Title = "Вид налога";
                ComboKD.MultiSelect = false;
                ComboKD.ParentSelect = true;
                ComboKD.FillDictionaryValues(CustomMultiComboDataHelper.FillFullFNSKDIncludingList());
                ComboKD.SetСheckedState("Налоговые доходы ", true);
                
            }


           
            budgetLevel.Value = ComboBudgetLevel.SelectedValue;
           
            Year.Value = ComboYear.SelectedValue;
           
           
           Page.Title = "Начисленные и поступившие платежи";
           Label1.Text = "Анализ сумм начисленных и поступивших платежей по отдельным видам налоговых доходов"; 
           UltraChart1.Tooltips.FormatString = String.Format("<SERIES_LABEL>\n<ITEM_LABEL>\n<DATA_VALUE:N1> {1} тыс.руб.", ComboYear.SelectedValue, units);
           int month = ComboMonth.SelectedIndex + 1;
           nMonth.Value = Convert.ToString(month);
           int year = Convert.ToInt32(ComboYear.SelectedValue);
           UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(month));
           UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(month));
           UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;           
           selectedPeriod.Value = String.Empty;
 
           if (ComboRegion.SelectedValue != "Все территории")
           {
               switch (ComboRegion.SelectedValue)
               {
                   case "Консолидированный бюджет субъекта":
                       {
                           selectedRegion.Value = "[Районы].[Сопоставимый].[Консолидированный бюджет субъекта ]";
                           break;
                       }
                   default:
                       {
                           selectedRegion.Value = RegionsNamingHelper.LocalBudgetUniqueNames[ComboRegion.SelectedValue];
                           break;
                       }
               }
               regType.Value = "[Район с поселениями]";
               measStr.Value = string.Empty;
           }
           else
           {
               selectedRegion.Value = RegionSettingsHelper.Instance.RegionsConsolidateLevel;
               regType.Value = "[Районы]";
               measStr.Value = "[Районы].[Сопоставимый].[Все территории],";
           }

           UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
           fnsKDGroup.Value = ComboKD.SelectedValue;
           UserParams.PeriodYear.Value = year.ToString();
           string titlemonth = string.Empty;
           if (ComboMonth.SelectedValue != "Январь")
           {
               titlemonth = String.Format("за январь - {0}", ComboMonth.SelectedValue.ToLower());
           }
           else
           {
               titlemonth = "за январь";
           }
           Label2.Text = String.Format("<br/>{0} {1} года, {2}, {3}, {4}, тыс.руб.", titlemonth, ComboYear.SelectedValue, ComboBudgetLevel.SelectedValue.ToLower(), ComboRegion.SelectedValue, ComboKD.SelectedValue.TrimEnd(' '));
           LabelDesc.Text = "* - Некорректно расчитывать уровень собираемости для краткосрочных периодов, поскольку суммы, исчисленные к уплате в текущем периоде, будут получены в следующем периоде, в соответствии с указанными в Налоговом кодексе или утвержденными  региональными и местными законами сроками уплаты налогов и авансовых платежей по ним.";
           string patternValue = UserParams.StateArea.Value;
           int defaultRowIndex = 1;
           UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
           UltraWebGrid.Bands.Clear();
           UltraWebGrid.DataBind();
           UltraChart1.DataBind();
           CRHelper.SaveToUserAgentLog(String.Format("Остльной лоад {0}", Environment.TickCount - start));            
        }

        #region Обработчики грида
        /// <summary>
        /// Активация строки грида
        /// </summary>
        /// <param name="row"></param>
        private void ActiveGridRow(UltraGridRow row)
        {
        }
        int minf = 0;
        int minu = 0;
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {

            string query = DataProvider.GetQueryText("FNS_0001_0007_compare_Grid");
                dtGrid = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование муниципального образования", dtGrid);
                if (dtGrid.Rows.Count > 0 && dtGrid.Columns.Count > 3)
                {
                    dtGrid.Columns.RemoveAt(0);
                    dtGrid.AcceptChanges();
                    UltraWebGrid.DataSource = dtGrid;
                }
                else
                    UltraWebGrid.DataSource = null;
                
        }

        private static string GetChartQuarterStr(string period)
        {
            string[] strs = period.Split(' ');
            if (strs.Length > 1)
            {
                double quarterNumber = Convert.ToInt32(strs[1]);
                return string.Format("{0} квартал", quarterNumber);
            }
            else
            {
                return period;
            }
        }

        void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    if (box.DataPoint != null)
                    {
                        double percent = 0;
                        string columnname = string.Empty;
                        if (box.DataPoint.Label == "Начисления по юридическим лицам")
                        {
                            columnname = "Доля начислений по юридическим лицам в общей сумме начислений, % ";
                        }
                        if (box.DataPoint.Label == "Начисления по физическим лицам")
                        {
                            columnname = "Доля начислений по физическим лицам в общей сумме начислений, % ";
                        }

                            for (int k = 0; k < Chart.Columns.Count; k++)
                            {
         
                                if (Chart.Columns[k].Caption == columnname)
                                {
                                    percent = Convert.ToDouble(Chart.Rows[box.Row][k]);
                                }
                            }
                        if (percent > 0)
                        {
                            box.DataPoint.Label =
                                String.Format("Доля в общей сумме начислений {0:N2}%<br />{1} за {2} год", percent, box.DataPoint.Label, ComboYear.SelectedValue);
                        }
                    }
                }
            }

        }


        DataTable Chart = new DataTable();
        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FNS_0001_0007_compare_Chart1_1");
            DataTable dtChart1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Dummy", dtChart1);
            for (int i = 0; i < dtChart1.Rows.Count; i++)
            {

                dtChart1.Rows[i][0] = dtChart1.Rows[i][0].ToString().Replace("Муниципальный район", "МР");
                dtChart1.Rows[i][0] = dtChart1.Rows[i][0].ToString().Replace("муниципальный район", "МР");
                
            } 


                    UltraChart1.DataSource = dtChart1;

                
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {

        }

        void UltraWebGrid1_DataBound(object sender, EventArgs e)
        {

        }

        private static void SetColumnParams(UltraGridLayout layout, int bandIndex, int columnIndex, string format, int width, bool hidden)
        {
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            if (e.Layout.Bands[0].Columns.Count > 0)
            {
                e.Layout.GroupByBox.Hidden = true;
                e.Layout.HeaderStyleDefault.Wrap = true;
                e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
                e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(265);
                e.Layout.Bands[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[0].Header.RowLayoutColumnInfo.OriginY = 0;
                for (int k = 2; k < e.Layout.Bands[0].Columns.Count; k++)
                {
                    e.Layout.Bands[0].Columns[k].Width = 176;
                    string formatString = "N2";
                    e.Layout.Bands[0].Columns[k].Format = formatString;
                    e.Layout.Bands[0].Columns[k].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[k].CellStyle.Padding.Right = 5;
                    switch (k)
                    {
                        case 5:
                            {
                                e.Layout.Bands[0].Columns[k].Format = "N0";
                                break;
                            }
                            case 6:
                            {
                                e.Layout.Bands[0].Columns[k].Format = "N0";
                                e.Layout.Bands[0].Columns[k].Hidden = true;
                              break;
                            }
                    }
                }
                e.Layout.Bands[0].Columns[0].Width = 126;
             
            }
        }
        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            int min = 0;
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {

                if ((i == 5)||(i == 10))
                {
                    if (i == 10)
                    {
                        min = Convert.ToInt32(e.Row.Cells[i + 1].Value);
                    }
                    if (e.Row.Cells[i].Value != null && min != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        min.ToString() != string.Empty)
                    {
                        
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("Самый высокий уровень собираемости");
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == (Convert.ToInt32(e.Row.Cells[i + 1].Value)))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = string.Format("Самый низкий уровень собираемости");
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                if ((i == 9))
                {
                    if (i == 4)
                    {
                      min = minf;
                    }
                    if (i == 8)
                    {
                     min = minu;
                    }
                    if (e.Row.Cells[i].Value != null && 
                        e.Row.Cells[i].Value.ToString() != string.Empty )
                    {

                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("Самый высокий уровень собираемости");
                        }
                        else if (Convert.ToInt32(e.Row.Cells[i].Value) == min)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = string.Format("Самый низкий уровень собираемости");
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

               
            }

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Cells[1].Value != null && (e.Row.Index == 0) &&
                       ((e.Row.Cells[1].Value.ToString().Contains("Все территории")) || (e.Row.Cells[1].Value.ToString().Contains("район"))))
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
        }
        
        #endregion

        #region Обработчики диаграмы
        
        void UltraChart_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {

        }
        #endregion
        
        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }

        protected void UltraWebGrid1_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActiveGridRow(e.Row);
        }
        #region Экспорт в Excel

        private void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            string label = Label2.Text.Replace("<br/>", "");
            ActiveGridRow(UltraWebGrid.Rows[0]);
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 65;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            ActiveGridRow(UltraWebGrid.Rows[3]);
            e.CurrentWorksheet.Rows[0].Cells[0].Value = Page.Title;
            e.CurrentWorksheet.Rows[1].Cells[0].Value = label;
        }

        private void ExcelExporter_HeaderCellExporting(object sender, HeaderCellExportingEventArgs e)
        {
          
        }

        private void ExcelExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.ExcelExport.EndExportEventArgs e)
        {

            int columnCount = UltraWebGrid.Columns.Count;
            int width = 150;
            e.CurrentWorksheet.Rows[7].CellFormat.Alignment = HorizontalCellAlignment.Left;
            e.CurrentWorksheet.Columns[0].Width = width * 30;
            e.CurrentWorksheet.Columns[1].Width = width * 75;
            e.CurrentWorksheet.Columns[2].Width = width * 30;
            e.CurrentWorksheet.Columns[3].Width = width * 30;
            e.CurrentWorksheet.Columns[4].Width = width * 30;
            e.CurrentWorksheet.Columns[5].Width = width * 30;
            e.CurrentWorksheet.Columns[6].Width = width * 30;
            e.CurrentWorksheet.Columns[7].Width = width * 30;
            e.CurrentWorksheet.Columns[8].Width = width * 30;
            e.CurrentWorksheet.Columns[9].Width = width * 30;
            e.CurrentWorksheet.Columns[10].Width = width * 30;
            e.CurrentWorksheet.Columns[11].Width = width * 30;
            e.CurrentWorksheet.Columns[12].Width = width * 30;
            e.CurrentWorksheet.Columns[13].Width = width * 30;
            e.CurrentWorksheet.Columns[14].Width = width * 30;

            int columnCountt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCountt; i = i + 1)
            {
                e.CurrentWorksheet.Rows[6].Cells[i].CellFormat.FillPattern = FillPatternStyle.None;                               
            }
            e.CurrentWorksheet.Rows[6].Cells[0].CellFormat.FillPattern = FillPatternStyle.None;
            int columnCounttt = UltraWebGrid.Columns.Count;
            for (int i = 1; i < columnCounttt; i = i + 1)
            {
                if ((i == 3) || (i == 7))
                {
                    e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
                }
                else
                    if ((i == 5) || (i == 8) || (i == 9))
                    {
                        e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0;[Red]-#,##0";
                    }
                    else
                    {
                        e.CurrentWorksheet.Columns[i].CellFormat.FormatString = "#,##0.00;[Red]-#,##0.00";
                    }
                e.CurrentWorksheet.Columns[i].CellFormat.Alignment = HorizontalCellAlignment.Right;
            }
            for (int i = 3; i <  4; i++)
            {
                e.CurrentWorksheet.Rows[i].Height = 20 * 35;
            }
            e.CurrentWorksheet.Rows[5].Height = 20 * 15;
            for (int k = 1; k < columnCounttt; k = k + 1)
            {
                e.CurrentWorksheet.Rows[3].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
                e.CurrentWorksheet.Rows[4].Cells[k].CellFormat.WrapText = ExcelDefaultableBoolean.True;
            }

            e.CurrentWorksheet.Rows[3 + UltraWebGrid.Rows.Count + 1].Cells[0].Value = LabelDesc.Text;
            
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            string label = Label2.Text.Replace("<br/>", "");
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("Анализ сумм платежей ");
            Worksheet sheet2 = workbook.Worksheets.Add("Сравнение сумм платежей ");
            UltraGridExporter1.ExcelExporter.DownloadName = "report.xls";
            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;
            sheet2.Rows[0].Cells[0].Value = ChartCaption1.Text;
            sheet2.Rows[1].Cells[0].Value = label;
            UltraGridExporter.ChartExcelExport(sheet2.Rows[2].Cells[0], UltraChart1);
            UltraGridExporter1.ExcelExporter.Export(UltraWebGrid, sheet1);

            UltraGridExporter1.ExcelExporter.ExcelStartRow = 4;


            
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            UltraGridExporter1.PdfExporter.DownloadName = "report.pdf";
            UltraGridExporter1.PdfExporter.Export(UltraWebGrid);
        }

        private void PdfExporter_BeginExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.DocumentExportEventArgs e)
        {
            string label = Label2.Text.Replace("<br/>", "");
            IText title = e.Section.AddText();
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 16);
            title.Style.Font.Bold = true;
            title.AddContent(Page.Title);

            title = e.Section.AddText();
            font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.AddContent(label);
        }

        private void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {

            IText title = e.Section.AddText();
            title.AddContent(LabelDesc.Text);
            
            e.Section.AddPageBreak();
            string label = Label2.Text.Replace("<br/>", "");

            title = e.Section.AddText();
            Infragistics.Documents.Reports.Graphics.Font font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.Style.Font.Bold = true;
            title.AddContent(ChartCaption1.Text);

            title = e.Section.AddText();
            font = new Infragistics.Documents.Reports.Graphics.Font("Verdana", 14);
            title.AddContent(label);

            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            Infragistics.Documents.Reports.Graphics.Image img = UltraGridExporter.GetImageFromChart(UltraChart1);
            e.Section.AddImage(img);
        }

        #endregion
   }

    public class ReportSection : ISection
    {
        private readonly bool withFlowColumns;
        private readonly ISection section;
        private IFlow flow;
        private ITableCell titleCell;

        public ReportSection(Report report, bool withFlowColumns)
        {
            section = report.AddSection();
            ITable table = section.AddTable();
            ITableRow row = table.AddRow();
            titleCell = row.AddCell();
        }

        public void AddFlowColumnBreak()
        {
            if (flow != null)
                flow.AddColumnBreak();
        }

        public Infragistics.Documents.Reports.Report.ContentAlignment PageAlignment
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IBand AddBand()
        {
            return section.AddBand();
        }

        #region ISection members
        public ISectionHeader AddHeader()
        {
            throw new NotImplementedException();
        }

        public ISectionFooter AddFooter()
        {
            throw new NotImplementedException();
        }

        public IStationery AddStationery()
        {
            throw new NotImplementedException();
        }

        public IDecoration AddDecoration()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage()
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(PageSize size)
        {
            throw new NotImplementedException();
        }

        public ISectionPage AddPage(float width, float height)
        {
            throw new NotImplementedException();
        }

        public ISegment AddSegment()
        {
            throw new NotImplementedException();
        }

        public IQuickText AddQuickText(string text)
        {
            throw new NotImplementedException();
        }

        public IQuickImage AddQuickImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            throw new NotImplementedException();
        }

        public IQuickList AddQuickList()
        {
            throw new NotImplementedException();
        }

        public IQuickTable AddQuickTable()
        {
            throw new NotImplementedException();
        }

        public IText AddText()
        {
            return this.titleCell.AddText();
        }

        public IImage AddImage(Infragistics.Documents.Reports.Graphics.Image image)
        {
            if (flow != null)
                return flow.AddImage(image);
            return this.section.AddImage(image);
        }

        public IMetafile AddMetafile(Infragistics.Documents.Reports.Graphics.Metafile metafile)
        {
            throw new NotImplementedException();
        }

        public IRule AddRule()
        {
            throw new NotImplementedException();
        }

        public IGap AddGap()
        {
            throw new NotImplementedException();
        }

        public IGroup AddGroup()
        {
            throw new NotImplementedException();
        }

        public IChain AddChain()
        {
            throw new NotImplementedException();
        }

        public ITable AddTable()
        {
            return this.section.AddTable();
        }

        public IGrid AddGrid()
        {
            throw new NotImplementedException();
        }

        public IFlow AddFlow()
        {
            throw new NotImplementedException();
        }

        public Infragistics.Documents.Reports.Report.List.IList AddList()
        {
            throw new NotImplementedException();
        }

        public ITree AddTree()
        {
            throw new NotImplementedException();
        }

        public ISite AddSite()
        {
            throw new NotImplementedException();
        }

        public ICanvas AddCanvas()
        {
            throw new NotImplementedException();
        }

        public IRotator AddRotator()
        {
            throw new NotImplementedException();
        }

        public IContainer AddContainer(string name)
        {
            throw new NotImplementedException();
        }

        public ICondition AddCondition(IContainer container, bool fit)
        {
            throw new NotImplementedException();
        }

        public IStretcher AddStretcher()
        {
            throw new NotImplementedException();
        }

        public void AddPageBreak()
        {
            throw new NotImplementedException();
        }

        public ITOC AddTOC()
        {
            throw new NotImplementedException();
        }

        public IIndex AddIndex()
        {
            throw new NotImplementedException();
        }

        public bool Flip
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public PageSize PageSize
        {
            get { throw new NotImplementedException(); }
            set { this.section.PageSize = new PageSize(2560, 1350); }
        }

        public PageOrientation PageOrientation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }



        public Borders PageBorders
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Margins PageMargins
        {
            get { return this.section.PageMargins; }
            set { throw new NotImplementedException(); }
        }

        public Paddings PagePaddings
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Background PageBackground
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Infragistics.Documents.Reports.Report.Section.PageNumbering PageNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public SectionLineNumbering LineNumbering
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Report Parent
        {
            get { return this.section.Parent; }
        }
        
        public IEnumerable Content
        {
            get { throw new NotImplementedException(); }
        }
		        
        #endregion
    }
   
}
