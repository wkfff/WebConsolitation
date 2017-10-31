using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Layers;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core.MemberDigests;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using Krista.FM.Server.Dashboards.Components;
//using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Krista.FM.ServerLibrary;
using System.Web.SessionState;
using System.Web;
using Krista.FM.Common;
using System.Runtime.Remoting.Messaging;


namespace Krista.FM.Server.Dashboards.reports.FO_0002_0036
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private DataTable dtChart = new DataTable();
        private int firstYear = 2007;
        private int endYear;
        private string endMonth;
        private int badRank;
        private int rubMulti = 1000;
        private GridHeaderLayout headerLayout;
        private static MemberAttributesDigest levelDigest;
        private string multiplierCaption;
        private DateTime date;
        private bool sumRowFlag = false;

        private CustomParam periodYear;
        private CustomParam periodMonth;
        private CustomParam periodHalfYear;
        private CustomParam periodQuater;
        private CustomParam periodPrevYear;
        private CustomParam levelBudget;
        private CustomParam regionChart;
        //private CustomParam chartKateg;
        private CustomParam currentRegion;
        private CustomParam countItemLegend;
        
        // выбранный множитель рублей
        private CustomParam rubMultiplier;

        private bool IsNotEmptyYears()
        {
            return ComboYear.SelectedNodes.Count > 0;
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка диаграммы

            UltraChart1.ChartType = ChartType.PieChart; // круговая диаграмма
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 9);
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Extent = 130;
            UltraChart1.Axis.Y.Extent = 130; 
            UltraChart1.PieChart.Labels.Font = font;
            UltraChart1.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n<DATA_VALUE:N2> {0}, удельный вес <PERCENT_VALUE:N2>%",multiplierCaption);
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Right;
            UltraChart1.Legend.SpanPercentage = 50;
            UltraChart1.Legend.Margins.Right = 10;
            UltraChart1.Legend.Margins.Left = 40;
            UltraChart1.Legend.Margins.Top = 10;
            UltraChart1.Legend.Font = font;
            UltraChart1.PieChart.RadiusFactor = 80;
            UltraChart1.PieChart.StartAngle = 260;
            UltraChart1.Data.SwapRowsAndColumns = true;
            UltraChart1.PieChart.OthersCategoryPercent = 0;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.ChartDrawItem += new ChartDrawItemEventHandler(UltraChart1_ChartDrawItem);
            UltraChart1.FillSceneGraph += new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

            #endregion
            
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight - 500);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid.InitializeLayout += new InitializeLayoutEventHandler(UltraWebGrid_InitializeLayout);
            UltraWebGrid.ActiveRowChange += new ActiveRowChangeEventHandler(UltraWebGrid_ActiveRowChange);
            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth * 0.65);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight - 400);

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);

            periodYear = UserParams.CustomParam("period_year");
            periodMonth = UserParams.CustomParam("period_month");
            periodHalfYear = UserParams.CustomParam("period_halfyear");
            periodQuater = UserParams.CustomParam("period_quater");
            periodPrevYear = UserParams.CustomParam("period_prev_year");
            levelBudget = UserParams.CustomParam("level_budget");
            rubMultiplier = UserParams.CustomParam("rubMultiplier");
            regionChart = UserParams.CustomParam("region_chart");
            //chartKateg = UserParams.CustomParam("chart_kateg");
            currentRegion = UserParams.CustomParam("current_Region");
            countItemLegend = UserParams.CustomParam("count_item");
        }

        void UltraChart1_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;
                widthLegendLabel = ((int)UltraChart1.Legend.SpanPercentage * (int)UltraChart1.Width.Value / 100) - 20;
            
                widthLegendLabel -= UltraChart1.Legend.Margins.Left + UltraChart1.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }
        }

        private void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
            int countBox = 0;
            int countText = 0;
            int offsetBox = 0;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
    
                if (primitive is Text)
                {
                    Text text = (Text)primitive;
                    string strText = text.GetTextString();
                    if (!strText.Contains("%"))
                    {
                        text.bounds.Height = 50;
                        text.labelStyle.WrapText = true;
                        text.bounds.Offset(0, countText * 20);
                        countText++;                    
                    }
                }

                if (primitive is Box)
                {
                    Box box = (Box)primitive;
                    countBox++;
                    if (countBox >= (Convert.ToInt32(countItemLegend.Value) + 2))
                    {
                        box.rect.Offset(0, (offsetBox * 20) + 3);
                        offsetBox++;
                    }
                }
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                //WebAsyncRefreshPanel1.AddRefreshTarget(UltraChart1);
                //WebAsyncRefreshPanel1.AddRefreshTarget(chart1Caption);
                //WebAsyncRefreshPanel1.AddLinkedRequestTrigger(UltraWebGrid);
                
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0036_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                endMonth = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
                

                ComboMonth.Width = 140;
                ComboMonth.Title = "Месяц";
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(endMonth, true);
              

                ComboLevel.Width = 350;
                ComboLevel.Title = "Уровень бюджета";
                ComboLevel.MultiSelect = false;
                levelDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0002_0036_level_budget");
                ComboLevel.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(levelDigest.UniqueNames, levelDigest.MemberLevels));
                ComboLevel.SetСheckedState("Конс.бюджет субъекта", true);
            }

            Page.Title = "Анализ общей долговой нагрузки на бюджеты";
            Label1.Text = Page.Title;
         
            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);
            date = new DateTime(yearNum, monthNum, 1).AddMonths(1);

            Label2.Text = String.Format("Данные на {0:dd.MM.yyyy} года",
                date);
            periodYear.Value = ComboYear.SelectedValue;
            periodMonth.Value = ComboMonth.SelectedValue;
            periodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            periodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            periodPrevYear.Value = string.Format("{0}", Convert.ToInt32(ComboYear.SelectedValue) - 1);
            levelBudget.Value = ComboLevel.SelectedValue;
         
            int selIndex = RubMiltiplierButtonList.SelectedIndex;
            if (selIndex == 0)
            {
                multiplierCaption = "руб.";
                rubMulti = 1;
            }
            else
            {
                if (selIndex == 1)
                {
                    multiplierCaption = "тыс.руб.";
                    rubMulti = 1000;
                }
                else
                {
                    multiplierCaption = "млн.руб.";
                    rubMulti = 1000000;
                }
            }
            rubMultiplier.Value = string.Format("{0}", rubMulti);
            UltraChart1.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n<DATA_VALUE:N2> {0}, удельный вес <PERCENT_VALUE:N2>%", multiplierCaption);


            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();

        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0036_grid");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Бюджет", dtGrid);
            if (dtGrid.Rows.Count > 0)
            {
                badRank = Convert.ToInt32(dtGrid.Rows[0][10]);
                dtGrid.Rows[0][7] = DBNull.Value;
                dtGrid.Columns.RemoveAt(10);
                int numberStr;
                
                if (dtGrid.Rows.Count > 1)
                {
                    if (dtGrid.Rows[0][0].ToString() == "Итого")
                    {
                        numberStr = 0;
                    }
                    else
                    {
                        numberStr = 1;
                    }
                    double sumCol3 = 0;
                    double sumCol4 = 0;
                    for (int i = numberStr+1; i < dtGrid.Rows.Count; i++)
                    {
                        if (dtGrid.Rows[i][3] != DBNull.Value)
                        {
                            sumCol3 += Convert.ToDouble(dtGrid.Rows[i][3]);
                        }
                        if (dtGrid.Rows[i][4] != DBNull.Value)
                        {
                            sumCol4 += Convert.ToDouble(dtGrid.Rows[i][4]);
                        }
                    }
                    if (sumCol3 != 0)
                    {
                        dtGrid.Rows[numberStr][3] = sumCol3;
                    }
                    if (sumCol4 != 0)
                    {
                        dtGrid.Rows[numberStr][4] = sumCol4;
                    }
                    if ((sumCol3 - sumCol4) != 0)
                    {
                        dtGrid.Rows[numberStr][5] = sumCol3 - sumCol4;
                        dtGrid.Rows[numberStr][6] = Convert.ToDouble(dtGrid.Rows[numberStr][1]) / Convert.ToDouble(dtGrid.Rows[numberStr][5]);
                    }
                    dtGrid.Rows[numberStr][7] = DBNull.Value;
                }
                UltraWebGrid.DataSource = dtGrid;                
            }
            else
            {
                UltraWebGrid.DataSource = null;
            }    
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowSortingDefault = AllowSorting.Yes;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(180);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.VerticalAlign = VerticalAlign.Top;
 
            int columnCount = e.Layout.Bands[0].Columns.Count;
            for (int i = 1; i < columnCount; i = i + 1)
            {
                switch (i)
                {
                    case 1:
                    case 8:
                    case 9:
                        { 
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                            e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(110);
                            break;
                        }
                    case 4:
                    case 5:
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                            e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(115);
                            break;
                        }
                    case 3:
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                            e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(113);
                            break;
                        }
                    case 2:
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                            e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(107);
                            break;
                        }
                    case 6:
                        { 
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
                            e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(110);
                            break;
                        }
                    case 7:
                        {
                            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                            e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(50);
                            break;
                        }
                }
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            headerLayout.AddCell("Бюджет");
            headerLayout.AddCell(string.Format("Государственный (муниципальный) долг на {0:dd.MM.yyyy} года, ",
                date) + multiplierCaption, 
                string.Format("Сумма государственного (муниципального) долга на {0:dd.MM.yyyy} года", date));
            headerLayout.AddCell("Доля в общем объеме муниципального долга, %", "Доля долговых обязательств местного бюджета в общем объеме муниципального долга");
            headerLayout.AddCell(string.Format("Доходы на {0:dd.MM.yyyy} года, ", date) + multiplierCaption,
                string.Format("Доходы бюджета на {0:dd.MM.yyyy} года", date));
            headerLayout.AddCell("в том числе безвозмездные поступления, " + multiplierCaption,
                "Безвозмездные поступления");
            headerLayout.AddCell("Доходы без учета безвозмездных поступлений, " + multiplierCaption,
                "Доходы без учета безвозмездных поступлений");
            headerLayout.AddCell("Отношение долга к доходам без учета безвозмездных поступлений, %",
                "Отношение долга к доходам без учета безвозмездных поступлений");
            headerLayout.AddCell("Ранг", "Ранг по отношению долга к доходам без учета безвозмездных поступлений");
            headerLayout.AddCell("Изменение долга к аналогичному периоду прошлого года, " + multiplierCaption,
                "Абсолютное изменение долга к аналогичному периоду прошлого года");
            headerLayout.AddCell("Изменение долга к началу года, " + multiplierCaption,
                "Абсолютное изменение долга к началу года");
         
            headerLayout.ApplyHeaderInfo();
        }

        private static DataTable GetNonEmptyDt(DataTable sourceDt)
        {
            DataTable dt = sourceDt.Clone();

            foreach (DataRow row in sourceDt.Rows)
            {
                if (!IsEmptyRow(row, 2))
                {
                    dt.ImportRow(row);
                }
            }
            dt.AcceptChanges();
            return dt;
        }

        private static bool IsEmptyRow(DataRow row, int startColumnIndex)
        {
            for (int i = startColumnIndex; i < row.Table.Columns.Count; i++)
            {
                if (row[i] != DBNull.Value)
                {
                    return false;
                }
            }
            return true;
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            if (e.Row.Cells[0].ToString() == "Новосибирская Область (Бюджет Субъекта)")
            {
                e.Row.Cells[0].Value = "Областной бюджет";
                for (int j = 0; j < e.Row.Cells.Count; j = j + 1)
                {
                    e.Row.Cells[j].Style.Font.Bold = true;
                }
            }
            else
            {
                if (e.Row.Cells[0].ToString() == "Итого")
                {
                    e.Row.Cells[0].Value = "Местные бюджеты";
                    for (int j = 0; j < e.Row.Cells.Count; j = j + 1)
                    {
                        e.Row.Cells[j].Style.Font.Bold = true;
                    }
                }
            }
           
            for (int i = 1; i < e.Row.Cells.Count; i = i + 1)
            {
                if (i == 6 && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    if (Convert.ToDouble(e.Row.Cells[i].Value) > 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                        e.Row.Cells[i].Title = "Норматив нарушен (>100%)";
                    }
                    else 
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                        e.Row.Cells[i].Title = "Норматив соблюдается (<=100%)";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (i == 7 && e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                {
                    int rank = Convert.ToInt32(e.Row.Cells[i].Value);
                    if (rank == 1)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                        e.Row.Cells[i].Title = "Минимальный уровень долговой нагрузки";
                    }
                    else if (rank == badRank)
                    {
                        e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                        e.Row.Cells[i].Title = "Максимальный уровень долговой нагрузки";
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != string.Empty)
                {
                    decimal value;
                    if (decimal.TryParse(cell.Value.ToString(), out value))
                    {
                        if (value < 0)
                        {
                            cell.Style.ForeColor = Color.Red;
                        }
                    }
                }

                
            }
        }

        private void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (!WebAsyncRefreshPanel1.IsAsyncPostBack)
            {               
                UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, currentRegion.Value, 0, 0);
                ActivateGridRow(row);
            }
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            ActivateGridRow(e.Row);
    
        }

        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
            {
                return;
            }
            
            string region = row.Cells[0].Text;
            string caption = string.Empty;

            if (region == "Областной бюджет")
            {
                caption = string.Format("Структура государственного долга на {0:dd.MM.yyyy} года, Новосибирская область", date);
                regionChart.Value = "[Новосибирская Область (Бюджет Субъекта)]";
                //currentRegion.Value = region;
            }
            else
            {
                if (region == "Местные бюджеты")
                {
                    caption = string.Format("Структура муниципального долга на {0:dd.MM.yyyy} года", date);
                    sumRowFlag = true;
                    //currentRegion.Value = region;
                }
                else
                {
                    if (region.Contains("г."))
                    {
                        caption = string.Format("Структура муниципального долга на {0:dd.MM.yyyy} года, {1}", date, region);
                        regionChart.Value = string.Format("[Городские округа].[{0}]", region);
                        //currentRegion.Value = region;
                    }
                    else
                    {
                        caption = string.Format("Структура муниципального долга на {0:dd.MM.yyyy} года, {1}", date, region);
                        regionChart.Value = string.Format("[Муниципальные районы].[{0}]", region);
                        //currentRegion.Value = region;
                    }
                }
            }
            currentRegion.Value = region;
            UltraWebGrid.DisplayLayout.SelectedRows.Clear();
            row.Selected = true;
            UltraWebGrid.DisplayLayout.ActiveRow = row;
            chart1Caption.Text = caption;
            UltraChart1.DataBind();
        }

        #region Обработчики диаграммы

        protected void UltraChart1_DataBinding(object sender, EventArgs e)
        {
            dtChart = new DataTable();
            string query;
            if (!sumRowFlag)
            {
                query = DataProvider.GetQueryText("FO_0002_0036_chart");
            }
            else
            {
                query = DataProvider.GetQueryText("FO_0002_0036_chartSum");
                sumRowFlag = false;
            }
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Диаграмма", dtChart);
            countItemLegend.Value = dtChart.Columns.Count.ToString();
            UltraChart1.DataSource = dtChart;
        }

        #endregion

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.WorksheetSubTitle = Label2.Text;
            ReportExcelExporter1.GridColumnWidthScale = 0.9;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            Worksheet sheet2 = workbook.Worksheets.Add("Диаграмма");

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart1, chart1Caption.Text, sheet2, 3);
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
            ISection section2 = report.AddSection();
            ReportPDFExporter1.Export(headerLayout, section1);
            UltraChart1.Width = Convert.ToInt32(UltraChart1.Width.Value * 0.9);
            ReportPDFExporter1.Export(UltraChart1, chart1Caption.Text, section2);
        }

        #endregion
    }
}