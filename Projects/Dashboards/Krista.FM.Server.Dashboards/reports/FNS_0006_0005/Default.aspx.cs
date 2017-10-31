using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.UltraChart.Core;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Krista.FM.Server.Dashboards.Core;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Infragistics.UltraChart.Core.Primitives;


namespace Krista.FM.Server.Dashboards.reports.FNS_0006_0005
{
    public partial class Default: CustomReportPage
    {
        #region Поля

        private DataTable dtGrid1;
        private DataTable dtGrid2;
        private DataTable dtChart;
       
        private int firstYear = 2008;
        private int endYear;
        private bool city = false;
        private string format;
        private DateTime currentDate;

        private CustomParam multiplier;
        private CustomParam areaType;
        private CustomParam incomType;
        private CustomParam territory;
        private CustomParam measures;
        private CustomParam listMeasures;
        private CustomParam finType;

        private GridHeaderLayout headerLayout1;
        private GridHeaderLayout headerLayout2;


        private static MemberAttributesDigest areaDigest;
        private static MemberAttributesDigest activeDigest;
        private static MemberAttributesDigest incomesDigest;
        private static MemberAttributesDigest incomesTypeDigest;

      #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2.0);
            UltraWebGrid1.DataBound +=new EventHandler(UltraWebGrid1_DataBound);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid2.Height = Unit.Empty;
            UltraWebGrid2.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 20);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight/1.6);

            #region Настройка диаграммы
            UltraChart1.ChartType = ChartType.ColumnChart;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Data.ZeroAligned = true;
            UltraChart1.Axis.X.Extent = 150;
            UltraChart1.Axis.Y.Extent = 40;
            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.TitleLeft.Visible = true;
            UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Top;
            UltraChart1.Legend.SpanPercentage = 10;
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
            UltraChart1.FillSceneGraph +=new FillSceneGraphEventHandler(UltraChart1_FillSceneGraph);

            UltraChart1.ColorModel.ModelStyle = ColorModels.LinearRange;
            UltraChart1.ColorModel.ColorBegin = Color.Green;
            UltraChart1.ColorModel.ColorEnd = Color.Red;
            #endregion


            #region инициализация параметров

            multiplier = UserParams.CustomParam("rub_multiplier");
            areaType = UserParams.CustomParam("area_type");
            incomType = UserParams.CustomParam("income_type");
            territory = UserParams.CustomParam("territory");
            measures = UserParams.CustomParam("measures");
            listMeasures = UserParams.CustomParam("list_measures");
            finType = UserParams.CustomParam("fin_type");
            #endregion


            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
       
       protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                DataTable dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FNS_0006_0005_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                string month = dtDate.Rows[0][3].ToString();

                ComboPeriod.MultiSelect = false;
                ComboPeriod.Title = "Год";
                ComboPeriod.Visible = true;
                ComboPeriod.Width = 100;
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboPeriod.SetСheckedState(endYear.ToString(), true);
                
                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 150;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.MultiSelect = false;
                ComboMonth.SetСheckedState(month, true);

                areaDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FNS_0006_0005_area");
                ComboArea.Title = "Территории";
                ComboArea.Width = 300;
                ComboArea.Visible = true;
                ComboArea.ParentSelect = false;
                ComboArea.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(areaDigest.UniqueNames, areaDigest.MemberLevels));
                ComboArea.SetСheckedState("", true);

                ComboFin.MultiSelect = true;
                incomesDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FNS_0006_0005_Incomes");
                ComboFin.Title = "Вид дохода";
                ComboFin.Visible = true;
                ComboFin.Width = 500;
                ComboFin.MultipleSelectionType = MultipleSelectionType.SimpleMultiple;
                ComboFin.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(incomesDigest.UniqueNames, incomesDigest.MemberLevels));
                ComboFin.SetСheckedState("Все виды доходов ", true);
              /*  ComboFin.SetСheckedState("НАЛОГИ НА ПРИБЫЛЬ, ДОХОДЫ", true);
                ComboFin.SetСheckedState("Налог на доходы физических лиц",true);
                ComboFin.SetСheckedState("НАЛОГИ НА СОВОКУПНЫЙ ДОХОД", true);
                ComboFin.SetСheckedState("Налог, взимаемый в связи с применением упрощенной системы налогообложения", true);
                ComboFin.SetСheckedState("Единый налог на вмененный доход для отдельных видов деятельности", true);
                ComboFin.SetСheckedState("Единый сельскохозяйственный налог", true);
                ComboFin.SetСheckedState("НАЛОГИ НА ИМУЩЕСТВО", true);
                ComboFin.SetСheckedState("Налог на имущество физических лиц", true);
                ComboFin.SetСheckedState("Земельный налог", true);
                ComboFin.SetСheckedState("НАЛОГИ, СБОРЫ И РЕГУЛЯРНЫЕ ПЛАТЕЖИ ЗА ПОЛЬЗОВАНИЕ ПРИРОДНЫМИ РЕСУРСАМИ", true);
                ComboFin.SetСheckedState("ГОСУДАРСТВЕННАЯ ПОШЛИНА", true);
                ComboFin.SetСheckedState("ЗАДОЛЖЕННОСТЬ И ПЕРЕРАСЧЕТЫ ПО ОТМЕНЕННЫМ НАЛОГАМ, СБОРАМ И ИНЫМ ОБЯЗАТЕЛЬНЫМ ПЛАТЕЖАМ", true);
                ComboFin.SetСheckedState("Неналоговые доходы", true);
                ComboFin.SetСheckedState("ДОХОДЫ ОТ ИСПОЛЬЗОВАНИЯ ИМУЩЕСТВА, НАХОДЯЩЕГОСЯ В ГОСУДАРСТВЕННОЙ И МУНИЦИПАЛЬНОЙ СОБСТВЕННОСТИ", true);
                ComboFin.SetСheckedState("ПЛАТЕЖИ ПРИ ПОЛЬЗОВАНИИ ПРИРОДНЫМИ РЕСУРСАМИ", true);
                ComboFin.SetСheckedState("ДОХОДЫ ОТ ОКАЗАНИЯ ПЛАТНЫХ УСЛУГ И КОМПЕНСАЦИИ ЗАТРАТ ГОСУДАРСТВА", true);
                ComboFin.SetСheckedState("ДОХОДЫ ОТ ПРОДАЖИ МАТЕРИАЛЬНЫХ И НЕМАТЕРИАЛЬНЫХ АКТИВОВ", true);
                ComboFin.SetСheckedState("ШТРАФЫ, САНКЦИИ, ВОЗМЕЩЕНИЕ УЩЕРБА", true);
                ComboFin.SetСheckedState("Итого собственных доходов", true);
               */

                activeDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FNS_0006_0005_Active");
                ComboActive.Title = "Показатели";
                ComboActive.Width = 350;
                ComboActive.Visible = true;
                ComboActive.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(activeDigest.UniqueNames, activeDigest.MemberLevels));
                ComboActive.SetСheckedState("% исполнения прогноза на год", true);

              //  incomesTypeDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FNS_0006_0005_IncomesType");
                ComboIncomesType.Title = "Доходы";
                ComboIncomesType.Width = 300;
                ComboIncomesType.Visible = true;
                ComboIncomesType.ParentSelect = true;
                ComboIncomesType.MultiSelect = false;
                ComboIncomesType.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(incomesDigest.UniqueNames, incomesDigest.MemberLevels));
                ComboIncomesType.SetСheckedState("Налоговые доходы", true);
                
            }
            currentDate = new DateTime(Convert.ToInt32(ComboPeriod.SelectedValues[ComboPeriod.SelectedValues.Count-1]), CRHelper.MonthNum(ComboMonth.SelectedValue), 1).AddMonths(1);
            Page.Title = string.Format("Анализ поступлений собственных доходов и недоимки в бюджет муниципального образования, {2}, по состоянию на {0: dd.MM.yyyy}, {1}", currentDate, rubMultiplier.SelectedIndex == 0 ? "тыс. руб." : "млн. руб.", ComboArea.SelectedValue);
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Empty;

            string year = ComboPeriod.SelectedValue;
            UserParams.PeriodLastYear.Value = (Convert.ToInt32(year)-1).ToString();
            UserParams.PeriodYear.Value = year;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(ComboMonth.SelectedValue)));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(ComboMonth.SelectedValue)));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            multiplier.Value = rubMultiplier.SelectedIndex == 0 ? "1000" : " 1000000 ";
            territory.Value = areaDigest.GetMemberUniqueName(ComboArea.SelectedValue);
            areaType.Value = areaDigest.GetMemberUniqueName(ComboArea.SelectedValue);
            measures.Value = activeDigest.GetMemberUniqueName(ComboActive.SelectedValue);
            finType.Value = incomesDigest.GetMemberUniqueName(ComboIncomesType.SelectedValue);

            string typeList = string.Empty;
            if (ComboFin.SelectedValues.Count> 0)
            {
              for (int i=0; i < ComboFin.SelectedValues.Count; i++)
               {
                 if (ComboFin.SelectedValues[i] == "Все виды доходов ")
                  {
                      typeList = string.Format("[Все доходы],");
                  }
                 else
                  {
                     typeList += string.Format("{0},", incomesDigest.GetMemberUniqueName(ComboFin.SelectedValues[i]));
                  }
               }
               incomType.Value = typeList.TrimEnd(',');
            }
           
            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);
            grid1CaptionElement.Text = "Исполнение консолидированного бюджета муниципального образования";
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();

            headerLayout2 = new GridHeaderLayout(UltraWebGrid2);
            grid2CaptionsElement.Text = "Недополученные доходы муниципального образования в сравнении с недоимкой по региону";
            UltraWebGrid2.Bands.Clear();
            UltraWebGrid2.DataBind();

            chart1ElementCaption.Text = "Сравнение показателей исполнения бюджета муниципальных образований с средними показателями по региону";
            UltraChart1.DataBind();
            
        }

       
   #region Обработчик грида
        
        
        protected void UltraWebGrid1_DataBinding(Object sender, EventArgs e)
        {
            listMeasures.Value = string.Format("{0} Measures.[% исполнения на дату], Measures.[темп роста1],Measures.[темп роста2],Measures.[Среднедушевые доходы]{1} * {0} {2} {1}", "{", "}", areaDigest.GetMemberUniqueName(ComboArea.SelectedValue));
            
            string query = DataProvider.GetQueryText("FNS_0006_0005_grid1");
            dtGrid1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", dtGrid1);
            
            areaType.Value = string.Format("{0}", areaDigest.GetMemberUniqueName(ComboArea.SelectedValue));
            
            if (dtGrid1.Rows.Count > 0)
            {
                if (dtGrid1.Rows[0][dtGrid1.Columns.Count - 2].ToString() == "1") // выбран городской округ
                {
                    city = true;
                    if (CheckBox1.Checked)
                    {
                        listMeasures.Value = string.Format("{0} Measures.[% исполнения на дату], Measures.[темп роста1] {1} * {0} {2}, [Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Алтайский край].[Городские округа Алтайского края] {1} + {0}  Measures.[темп роста2] {1} * {0} {2} {1} + {0} Measures.[Среднедушевые доходы]  {1} * {0}  {2}, [Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Алтайский край].[Городские округа Алтайского края] {1}", "{", "}", areaDigest.GetMemberUniqueName(ComboArea.SelectedValue));
                    }

                    if (CheckBox2.Checked)
                    {
                        listMeasures.Value = string.Format("{0} Measures.[% исполнения на дату], Measures.[% исполнения на дату_город+район],  Measures.[темп роста1], Measures.[темп роста1_город + район],  Measures.[темп роста2] , Measures.[Среднедушевые доходы] ,  Measures.[Среднедушевые доходы_город + район]  {1} * {0} {2} {1}", "{", "}", areaDigest.GetMemberUniqueName(ComboArea.SelectedValue));
                    }
                    if (CheckBox1.Checked && CheckBox2.Checked)
                    {
                        listMeasures.Value = string.Format("{0} Measures.[% исполнения на дату] {1} * {0} {2}, [Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Алтайский край].[Городские округа Алтайского края] {1} +  {0} Measures.[% исполнения на дату_город+район]{1}* {0} {2} {1} + {0} Measures.[темп роста1]{1}  * {0} {2}, [Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Алтайский край].[Городские округа Алтайского края] {1} +  {0} Measures.[темп роста1_город + район]{1} * {0} {2} {1} +  {0} Measures.[темп роста2]{1} * {0} {2} {1} + {0} Measures.[Среднедушевые доходы] {1} * {0} {2}, [Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Алтайский край].[Городские округа Алтайского края] {1}+  {0} Measures.[Среднедушевые доходы_город + район]  {1} * {0} {2} {1}", "{", "}", areaDigest.GetMemberUniqueName(ComboArea.SelectedValue));
                    }
               }

                if (dtGrid1.Rows[0][dtGrid1.Columns.Count - 2].ToString() == "2") // выбран район
                {
                    city = false;
                    if (CheckBox1.Checked)
                    {
                        listMeasures.Value = string.Format("{0} Measures.[% исполнения на дату], Measures.[темп роста1] {1} * {0} {2}, [Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Алтайский край].[Районы Алтайского края]{1} + {0}  Measures.[темп роста2] {1} * {0} {2} {1} + {0} Measures.[Среднедушевые доходы]  {1} * {0}  {2}, [Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Алтайский край].[Районы Алтайского края]{1}", "{", "}", areaDigest.GetMemberUniqueName(ComboArea.SelectedValue));
                    }

                    if (CheckBox2.Checked)
                    {
                        listMeasures.Value = string.Format("{0} Measures.[% исполнения на дату], Measures.[% исполнения на дату_город+район],  Measures.[темп роста1], Measures.[темп роста1_город + район],  Measures.[темп роста2] , Measures.[Среднедушевые доходы] ,  Measures.[Среднедушевые доходы_город + район]  {1} * {0} {2} {1}", "{", "}", areaDigest.GetMemberUniqueName(ComboArea.SelectedValue));
                    }
                    if (CheckBox1.Checked && CheckBox2.Checked)
                    {
                        listMeasures.Value = string.Format("{0} Measures.[% исполнения на дату] {1} * {0} {2}, [Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Алтайский край].[Районы Алтайского края]{1} +  {0} Measures.[% исполнения на дату_город+район]{1}* {0} {2} {1} + {0} Measures.[темп роста1]{1}  * {0} {2}, [Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Алтайский край].[Районы Алтайского края]{1} +  {0} Measures.[темп роста1_город + район]{1} * {0} {2} {1} +  {0} Measures.[темп роста2]{1} * {0} {2} {1} + {0} Measures.[Среднедушевые доходы] {1} * {0} {2}, [Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Алтайский край].[Районы Алтайского края]{1}+  {0} Measures.[Среднедушевые доходы_город + район]  {1} * {0} {2} {1}", "{", "}", areaDigest.GetMemberUniqueName(ComboArea.SelectedValue));
                    }
                }
            }

            query = DataProvider.GetQueryText("FNS_0006_0005_grid1");
            dtGrid1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", dtGrid1);
            
            if (dtGrid1.Rows.Count > 0)
            {
              if (Convert.ToInt32(dtGrid1.Rows[0][dtGrid1.Columns.Count-2]) == 1)
              {
                  CheckBox1.Text = "средние показатели исполнения бюджета по городам";
              }
              if (Convert.ToInt32(dtGrid1.Rows[0][dtGrid1.Columns.Count - 2]) == 2)
              {
                 CheckBox1.Text = "средние показатели исполнения бюджета по районам";
              }

              for (int rowNum=0; rowNum< dtGrid1.Rows.Count; rowNum++)
              {
                string[] captions = dtGrid1.Rows[rowNum][0].ToString().Split(';');
                dtGrid1.Rows[rowNum][0] = captions[0];
              }

              for (int rowNum = 0; rowNum < dtGrid1.Rows.Count; rowNum++ )
              {
                  if (dtGrid1.Rows[rowNum][dtGrid1.Columns.Count - 1] != DBNull.Value && dtGrid1.Rows[rowNum][dtGrid1.Columns.Count - 1].ToString() != string.Empty)
                  {
                      dtGrid1.Rows[rowNum][0] = dtGrid1.Rows[rowNum][dtGrid1.Columns.Count - 1];
                  }

              }

              dtGrid1.AcceptChanges();
              UltraWebGrid1.DataSource = dtGrid1;
            }
            
          
        }

        protected void UltraWebGrid1_DataBound(Object sender,EventArgs e)
        {
            if (UltraWebGrid1.Rows.Count <=10)
            {
                UltraWebGrid1.Height = Unit.Empty;
            }
        }

        protected void UltraWebGrid1_InitializeLayout(Object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].MergeCells = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[UltraWebGrid1.Columns.Count - 1].Hidden = true;
            e.Layout.Bands[0].Columns[UltraWebGrid1.Columns.Count - 2].Hidden = true;
            e.Layout.Bands[0].Columns[UltraWebGrid1.Columns.Count - 3].Hidden = true;

            headerLayout1.AddCell("Показатели");
            int month = CRHelper.MonthNum(ComboMonth.SelectedValue);
            DateTime currentDate1 = new DateTime(month == 12 ? Convert.ToInt32(ComboPeriod.SelectedValue) : Convert.ToInt32(ComboPeriod.SelectedValue)-1,month== 12? 1 : month+1, 1);
            headerLayout1.AddCell(string.Format("Исполнено на {0:dd.MM.yyyy}", currentDate1), "Сумма фактических поступлений доходов за аналогичный период прошлого года");
            headerLayout1.AddCell(string.Format("Факт за {0} год", Convert.ToDouble(ComboPeriod.SelectedValue)-1));
            GridHeaderCell cell1 = headerLayout1.AddCell("Прогноз");
            cell1.AddCell(string.Format("Крайфо на {0} год", ComboPeriod.SelectedValue), "Прогноз поступлений доходов в текущем году",2);
            DateTime currentDate2 = new DateTime(month == 12 ? Convert.ToInt32(ComboPeriod.SelectedValue) +1: Convert.ToInt32(ComboPeriod.SelectedValue), month==12 ? 1 : month + 1, 1);
            cell1.AddCell(string.Format("Мун.образования на {0:dd.MM.yyyy}", currentDate2), "Уточненные годовые назначения доходов на отчетную дату", 2);
            headerLayout1.AddCell(string.Format("Исполнено на {0:dd.MM.yyyy}", currentDate2), "Сумма фактических поступлений доходов с начала отчетного года на текущую дату");
            GridHeaderCell cell2 = headerLayout1.AddCell("% исп. прогноза");
            cell2.AddCell(string.Format("Крайфо на {0} год", ComboPeriod.SelectedValue), "Процент исполнения прогнозных назначений на начало года");
           
            GridHeaderCell cell2_1 = cell2.AddCell(string.Format("на {0:dd.MM.yyyy}", currentDate2), "Процент исполнения уточненных годовых назначений на текущий месяц");
            cell2_1.AddCell(string.Format("{0}", ComboArea.SelectedValue));
            if (CheckBox1.Checked)
            {
                cell2_1.AddCell(string.Format("По {0}", city ? "городам" : "районам"));
            }
            if (CheckBox2.Checked)
            {
                cell2_1.AddCell(string.Format("Средне кр. показ."));
            }
            DateTime currentDate3 = new DateTime(month == 12 ? Convert.ToInt32(ComboPeriod.SelectedValue) -1: Convert.ToInt32(ComboPeriod.SelectedValue)-2, month==12 ? 1 : month + 1, 1);
            GridHeaderCell cell3 = headerLayout1.AddCell("Темп роста");
            GridHeaderCell cell3_1 =  cell3.AddCell(string.Format("к {0:dd.MM.yyyy}", currentDate1), "Темп роста фактических поступлений доходов на текущую дату к сумме поступлений за аналогичный период прошлого года");
            cell3_1.AddCell(string.Format("{0}", ComboArea.SelectedValue));
            if (CheckBox1.Checked)
            {
                cell3_1.AddCell(string.Format("По {0}", city ? "городам" : "районам"));
            }
            if (CheckBox2.Checked)
            {
                cell3_1.AddCell(string.Format("Средне кр. показ."));
            }
            cell3.AddCell(string.Format("к {0:dd.MM.yyyy}", currentDate3), "Темп роста фактических поступлений доходов на текущую дату к сумме поступлений за аналогичный период позапрошлого года");
           
            GridHeaderCell cell4 = headerLayout1.AddCell("Среднедушевые доходы, руб", "Сумма фактических поступлений доходов с начала года на отчетную дату в расчете на одного жителя");
            cell4.AddCell(string.Format("{0}", ComboArea.SelectedValue),2);
            if (CheckBox1.Checked)
            {
                cell4.AddCell(string.Format("По {0}", city ? "городам" : "районам"),2);
            }
            if (CheckBox2.Checked)
            {
                cell4.AddCell(string.Format("Средне кр. показ."),2);
            }
            headerLayout1.ApplyHeaderInfo();


            string caption = string.Empty;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
               CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
               e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
               e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            for (int i = 6; i < e.Layout.Bands[0].Columns.Count; i++)
            {
              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N1");
            }

            int j = e.Layout.Bands[0].Columns.Count-4;

            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[j], "N0"); 

            if (CheckBox1.Checked)
            {
              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[j-1], "N0"); 
            }
            if (CheckBox2.Checked)
            {
              CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[j - 2], "N0");
            }
        }

        protected void UltraWebGrid1_InitializeRow(Object sender, RowEventArgs e )
        {
            double indication = CRHelper.MonthNum(ComboMonth.SelectedValue) / 12.0;
            
            int rowIndex = e.Row.Index;
           // int div = CheckBox1.Checked && CheckBox2.Checked ? 3 : CheckBox1.Checked || CheckBox2.Checked ? 2 : 1;

            if (e.Row.Cells[0] != null)
            {
                e.Row.Cells[0].Value = e.Row.Cells[0].Value.ToString().Replace("_", "");
            }

            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                if (i == 7 || i == 6)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {

                        if (Convert.ToDouble(e.Row.Cells[i].Value) >= indication)
                        {
                            CRHelper.SaveToErrorLog(Convert.ToDouble(e.Row.Cells[i].Value).ToString());
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:P2})",
                                                                 indication);
                        }
                        else if (Convert.ToDouble(e.Row.Cells[i].Value) < indication)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                            e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:P2})",
                                                                 indication);
                        }

                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: left center; margin: 2px";
                    }
                }

                if (!CheckBox1.Checked && !CheckBox2.Checked)
                {
                    if (i==8 || i==9)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (100*Convert.ToDouble(e.Row.Cells[i].Value) >= 100)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                                e.Row.Cells[i].Title = "Рост к прошлому отчетному периоду";
                            }
                            else if (100*Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                e.Row.Cells[i].Title = "Снижение к прошлому отчетному периоду";
                            }

                            e.Row.Cells[i].Style.CustomRules =
                                "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                    }
                }

                if ((CheckBox1.Checked && !CheckBox2.Checked) || (CheckBox2.Checked && !CheckBox1.Checked))
                {
                    if (i == 8)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (Convert.ToDouble(e.Row.Cells[i].Value) >= indication)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                                e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:P2})",
                                                                     indication);
                            }
                            else if (Convert.ToDouble(e.Row.Cells[i].Value) < indication)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                                e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:P2})",
                                                                     indication);
                            }

                            e.Row.Cells[i].Style.CustomRules =
                                "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                    }

                    if (i==9 || i==10 || i==11)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (100 * Convert.ToDouble(e.Row.Cells[i].Value) >= 100)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                                e.Row.Cells[i].Title = "Рост к прошлому отчетному периоду";
                            }
                            else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                e.Row.Cells[i].Title = "Снижение к прошлому отчетному периоду";
                            }

                            e.Row.Cells[i].Style.CustomRules =
                                "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                    }
                    if (i==13)
                    {
                        double value;
                        double predValue;
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (e.Row.Cells[i-1].Value != null && e.Row.Cells[i-1].Value.ToString() != string.Empty)
                            {
                                value = Convert.ToDouble(e.Row.Cells[i].Value) * 100;
                                predValue = Convert.ToDouble(e.Row.Cells[i-1].Value) * 100;

                                if (predValue >= value)
                                {
                                    e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerGreen.gif";
                                    e.Row.Cells[i].Title = string.Format("Показатель по выбранному {0} больше среднего показателя по {1}", city ? "городу" : "району", city ? "городам" : "районам");
                                }
                                else if ((predValue < value))
                                {
                                    e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerRed.gif";
                                    e.Row.Cells[i].Title =
                                        string.Format(
                                            "Показатель по выбранному {0} меньше среднего показателя по {1}", city ? "городу" : "району", city ? "городам" : "районам");
                                }
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: right top; margin: 2px";
                    }
                }

                if (CheckBox2.Checked && CheckBox1.Checked)
                {
                    if (i == 9 || i == 8)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (Convert.ToDouble(e.Row.Cells[i].Value) >= indication)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                                e.Row.Cells[i].Title = string.Format("Соблюдается условие равномерности ({0:P2})",
                                                                     indication);
                            }
                            else if (Convert.ToDouble(e.Row.Cells[i].Value) < indication)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                                e.Row.Cells[i].Title = string.Format("Не соблюдается условие равномерности ({0:P2})",
                                                                     indication);
                            }

                            e.Row.Cells[i].Style.CustomRules =
                                "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                    }

                    if (i == 12 || i == 10 || i == 11 || i==13)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (100 * Convert.ToDouble(e.Row.Cells[i].Value) >= 100)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                                e.Row.Cells[i].Title = "Рост к прошлому отчетному периоду";
                            }
                            else if (100 * Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                            {
                                e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                                e.Row.Cells[i].Title = "Снижение к прошлому отчетному периоду";
                            }

                            e.Row.Cells[i].Style.CustomRules =
                                "background-repeat: no-repeat; background-position: left center; margin: 2px";
                        }
                    }

                    if (i == 15 || i == 16)
                    {
                        double value;
                        double predValue;
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            if (e.Row.Cells[14].Value != null && e.Row.Cells[14].Value.ToString() != string.Empty)
                            {
                                value = Convert.ToDouble(e.Row.Cells[i].Value) * 100;
                                predValue = Convert.ToDouble(e.Row.Cells[14].Value) * 100;

                                if (predValue >= value)
                                {
                                    e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerGreen.gif";
                                    e.Row.Cells[i].Title = string.Format("Показатель по выбранному {0} больше среднего показателя по {1}", city ? "городу" : "району", city ? "городам" : "районам");
                                }
                                else if ((predValue < value))
                                {
                                    e.Row.Cells[i].Style.BackgroundImage = "~/images/CornerRed.gif";
                                    e.Row.Cells[i].Title =
                                        string.Format(
                                            "Показатель по выбранному {0} меньше среднего показателя по {1}", city ? "городу" : "району", city ? "городам" : "районам");
                                }
                            }
                        }
                        e.Row.Cells[i].Style.CustomRules =
                            "background-repeat: no-repeat; background-position: right top; margin: 2px";
                    }
                }

            }

            string level = e.Row.Cells[e.Row.Cells.Count-3].ToString();
            switch (level)
            {
                case "1":
                    {
                        e.Row.Cells[0].Style.Padding.Left = 15;
                        break;
                    }
                case "2":
                    {
                        e.Row.Cells[0].Style.Padding.Left = 25;
                        break;
                    }
                case "3":
                    {
                        e.Row.Cells[0].Style.Padding.Left = 35;
                        break;
                    }
            }
       }

        protected void UltraWebGrid2_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FNS_0006_0005_grid2");
            dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", dtGrid2);
            
            if  (dtGrid2.Rows.Count > 0 )
            {
               if (Convert.ToInt32(dtGrid2.Rows[0][dtGrid2.Columns.Count-1]) == 1)
               {
                   dtGrid2.Rows[5][0] = "Недоимка по городам";
               }
               else
               {
                   dtGrid2.Rows[5][0] = "Недоимка по районам";
               }
               dtGrid2.AcceptChanges();
               UltraWebGrid2.DataSource = dtGrid2;
            }
            
        }

        protected void UltraWebGrid2_InitializeLayout(Object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(180);
            e.Layout.Bands[0].Columns[UltraWebGrid2.Columns.Count - 1].Hidden = true;
            
            headerLayout2.AddCell("Показатели");
            headerLayout2.AddCell(string.Format("Недоимка на начало {0} года",ComboPeriod.SelectedValue));
            string month = ComboMonth.SelectedValue;
            currentDate = new DateTime(month == "Декабрь" ? Convert.ToInt32(ComboPeriod.SelectedValue)+1 : Convert.ToInt32(ComboPeriod.SelectedValue), month == "Декабрь" ? 1 :  CRHelper.MonthNum(ComboMonth.SelectedValue) + 1, 1);
            headerLayout2.AddCell(string.Format("Недоимка на {0:dd.MM.yyyy}", currentDate));
            GridHeaderCell cell2 = headerLayout2.AddCell(string.Format("Прирост недоимки с начала {0} года", ComboPeriod.SelectedValue));
            cell2.AddCell(string.Format("{0}", rubMultiplier.SelectedIndex == 0 ? "тыс. руб." : "млн. руб."));
            cell2.AddCell("%");
           
            headerLayout2.ApplyHeaderInfo();

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(100);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(100);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(100);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N0");
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(100);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N1");
           
        }

        protected void UltraWebGrid2_InitializeRow(Object sender, RowEventArgs e)
        {
           if (e.Row.Cells[3].Value != null && e.Row.Cells[3].ToString() != string.Empty)
           {
               if (Convert.ToDouble(e.Row.Cells[3].Value) > 0)
               {
                   e.Row.Cells[3].Style.ForeColor = Color.Red;
               }
           }

           if (e.Row.Cells[4].Value != null && e.Row.Cells[4].ToString() != string.Empty)
           {
             if (100 * Convert.ToDouble(e.Row.Cells[4].Value) > 0)
               {
                   
                   e.Row.Cells[4].Style.ForeColor = Color.Red;
               }
           }

           int i = 4;
           if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
             {
                if (100*Convert.ToDouble(e.Row.Cells[i].Value) > 0)
                  {
                      e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                      e.Row.Cells[i].Title = "Прирост недоимки";
                  }
                else if (100*Convert.ToDouble(e.Row.Cells[i].Value) < 0)
                  {
                     e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                     e.Row.Cells[i].Title = "Снижение недоимки";
                  }

              e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
             }
            
          if (e.Row.Cells[0].Value.ToString().Contains("налогам"))
          {
              e.Row.Cells[0].Style.Padding.Left = 15;
          }
        }
   #endregion 

   #region Обработчики диаграмм 
         
        protected void UltraChart1_DataBinding(Object sender, EventArgs e)
        {

            if (city)
            {
                areaType.Value = "[Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Алтайский край].[Городские округа Алтайского края]";
            }
            else
            {
                areaType.Value = "[Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Алтайский край].[Районы Алтайского края]";

            }
            string query = DataProvider.GetQueryText("FNS_0006_0005_chart");
            dtChart = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Диаграмма1", dtChart);

            if (dtChart.Rows.Count > 0)
            {
                for (int rowNum = 0; rowNum < dtChart.Rows.Count; rowNum++ )
                {
                    string[] captions = dtChart.Rows[rowNum][0].ToString().Split(';');
                    dtChart.Rows[rowNum][0] = captions[0].Replace("район", "р-н");
                }
                
                if (ComboActive.SelectedIndex != 4)
                {
                    format = "P1";
                    UltraChart1.Axis.Y.Labels.ItemFormatString = string.Format("<DATA_VALUE:{0}>", format);
                    UltraChart1.Tooltips.FormatString = string.Format("<ITEM_LABEL>");
                    UltraChart1.TitleLeft.Text = "%";
                }
                else
                {

                    format = "N0";
                    UltraChart1.Axis.Y.Labels.ItemFormatString = string.Format("<DATA_VALUE:{0}>",format);   
                    UltraChart1.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n <SERIES_LABEL> \n <DATA_VALUE:{0}> руб.", format);
                    UltraChart1.TitleLeft.Text = "руб.";
                    UltraChart1.Legend.Visible = false;
                }

                if (ComboActive.SelectedIndex == 0 || ComboActive.SelectedIndex == 1)
                {
                    dtChart.Columns[1].ColumnName = "соблюдение/нарушение условия равномерности";
                }
                if (ComboActive.SelectedIndex == 2 || ComboActive.SelectedIndex == 3)
                {
                    dtChart.Columns[1].ColumnName = "рост/снижение фактических поступлений доходов";
                }

                dtChart.AcceptChanges();
                UltraChart1.DataSource = dtChart;
            }
        }

        protected void UltraChart1_FillSceneGraph(object sender, FillSceneGraphEventArgs e)
        {
             double indication = CRHelper.MonthNum(ComboMonth.SelectedValue) / 12.0;
            for (int i = 0; i < e.SceneGraph.Count; i++)
            {
                Primitive primitive = e.SceneGraph[i];
                if (primitive is Box)
                {
                    Box box = (Box)primitive;

                    if (box.DataPoint != null)
                    {
                        if (box.DataPoint.Label != null)
                        {

                            if (ComboActive.SelectedIndex == 0)
                            {
                                box.DataPoint.Label = string.Format("{0} \n Исполнение прогноза \n {1:P1} ",box.Series.Label, box.Value);
                            }
                            if (ComboActive.SelectedIndex == 1)
                            {
                                box.DataPoint.Label = string.Format("{0} \n Исполнение уточненного прогноза \n {1:P1} ", box.Series.Label, box.Value);
                            }
                            if (ComboActive.SelectedIndex == 2)
                            {
                                box.DataPoint.Label = string.Format("{0} \n Темп роста к прошлому году \n {1:P1} ", box.Series.Label, box.Value);
                            }
                            if (ComboActive.SelectedIndex == 3)
                            {
                                box.DataPoint.Label = string.Format("{0} \n Темп роста к позапрошлому году \n {1:P1} ", box.Series.Label, box.Value);
                            }
                           
                        }
                    }
                    if (box.DataPoint != null && box.Value != null)
                    {
                       if (ComboActive.SelectedIndex == 0 || ComboActive.SelectedIndex == 1)
                       {
                           box.PE.ElementType = PaintElementType.Gradient;
                           box.PE.FillGradientStyle = GradientStyle.ForwardDiagonal;

                           if (Convert.ToDouble(box.Value) >= indication)
                           {
                              box.PE.Fill = Color.LimeGreen;
                              box.PE.FillStopColor = Color.ForestGreen;
                           }
                           else if (Convert.ToDouble(box.Value) < indication)
                           {
                               box.PE.Fill = Color.Maroon;
                               box.PE.FillStopColor = Color.Red;
                           }

                       }
                       else if (ComboActive.SelectedIndex == 2 || ComboActive.SelectedIndex == 3)
                       {
                           if (Convert.ToDouble(box.Value)*100 >= 100)
                           {
                               box.PE.Fill = Color.LimeGreen;
                               box.PE.FillStopColor = Color.ForestGreen;
                           }
                           else if (Convert.ToDouble(box.Value)*100 < 100)
                           {
                               box.PE.Fill = Color.Maroon;
                               box.PE.FillStopColor = Color.Red;
                           }
                       }
                       else
                       {
                           box.PE.Fill = Color.LightYellow;
                           box.PE.FillStopColor = Color.Yellow;
                       }

                    }
                    else if (box.Path != null && box.Path.ToLower().Contains("legend") && box.rect.Width < 20)
                    {
                        if (ComboActive.SelectedIndex != 4)
                        {
                            box.PE.ElementType = PaintElementType.CustomBrush;
                            LinearGradientBrush brush = new LinearGradientBrush(box.rect, Color.Green, Color.Red, 45,
                                                                                false);
                            box.PE.CustomBrush = brush;
                        }
                     
                    }

                }
            }
            if (CheckBox1.Checked || CheckBox2.Checked)
            {
                if (city)
                {
                    areaType.Value = "[Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Алтайский край].[Городские округа Алтайского края]";
                }
                else
                {
                    areaType.Value = "[Районы__Сопоставимый].[Районы__Сопоставимый].[Все районы].[Алтайский край].[Районы Алтайского края]";

                }
                string query = DataProvider.GetQueryText("FNS_0006_0005_line");
                DataTable dtChartLine = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Диаграмма1", dtChartLine);
                if (dtChartLine.Rows.Count > 0)
                {
                    if (CheckBox1.Checked) // средний показатель по городам(районам)
                    {
                        if (dtChartLine.Rows[0][1] != null && dtChartLine.Rows[0][1].ToString() != string.Empty)
                        {
                            double value = Convert.ToDouble(dtChartLine.Rows[0][1]);

                            int textWidht = 500;
                            int textHeight = 12;
                            IAdvanceAxis xAxis = (IAdvanceAxis) e.Grid["X"];
                            IAdvanceAxis yAxis = (IAdvanceAxis) e.Grid["Y"];

                            int lineStart = (int) xAxis.MapMinimum;
                            int lineLength = (int) xAxis.MapMaximum;

                            Line line = new Line();
                            line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                            line.PE.Stroke = Color.Blue;
                            line.PE.StrokeWidth = 1;
                            line.p1 = new Point(lineStart, (int) yAxis.Map(value));
                            line.p2 = new Point(lineStart + lineLength, (int) yAxis.Map(value));
                            e.SceneGraph.Add(line);

                            Text text = new Text();
                            text.PE.Fill = Color.Black;
                            text.bounds = new Rectangle(lineLength - textWidht, ((int) yAxis.Map(value)) - textHeight,
                                                        textWidht, textHeight);
                            if (ComboActive.SelectedIndex != 4)
                            {
                                text.SetTextString(string.Format("Средний показатель по городам (районам): {0:P1}",
                                                                 value));
                            }
                            else
                            {
                                text.SetTextString(string.Format("Средний показатель по городам (районам): {0:N0}",
                                                                 value));
                            }
                            e.SceneGraph.Add(text);
                        }
                    }

                    if (CheckBox2.Checked) // показатель по субъекту
                    {
                        double value = 0;
                        if (dtChartLine.Rows[0][2] != null && dtChartLine.Rows[0][2].ToString() != string.Empty)
                        {
                            value = Convert.ToDouble(dtChartLine.Rows[0][2]);
                        }
                        IAdvanceAxis xAxis = (IAdvanceAxis) e.Grid["X"];
                        IAdvanceAxis yAxis = (IAdvanceAxis) e.Grid["Y"];

                        int textWidht = 200;
                        int textHeight = 12;
                        int lineStart = (int) xAxis.MapMinimum;
                        int lineLength = (int) xAxis.MapMaximum;

                        Line line = new Line();
                        line.lineStyle.DrawStyle = LineDrawStyle.Dash;
                        line.PE.Stroke = Color.DarkGray;
                        line.PE.StrokeWidth = 1;
                        line.p1 = new Point(lineStart, (int) yAxis.Map(value));
                        line.p2 = new Point(lineStart + lineLength, (int) yAxis.Map(value));
                        e.SceneGraph.Add(line);

                        Text text = new Text();
                        text.PE.Fill = Color.Black;
                        text.bounds = new Rectangle(lineLength - textWidht, ((int) yAxis.Map(value)) - textHeight,
                                                    textWidht, textHeight);
                        if (ComboActive.SelectedIndex != 4)
                        {
                            text.SetTextString(string.Format("Показатель субъекта: {0:P1}", value));
                        }
                        else
                        {
                            text.SetTextString(string.Format("Показатель субъекта: {0:N0}", value));
                        }
                        e.SceneGraph.Add(text);
                    }
                }
            }

            
           
        }

        #endregion 

   #region экспорт

      #region экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");
            
            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 0.9;
            ReportExcelExporter1.Export(headerLayout1,grid1CaptionElement.Text, sheet1, 3);
            ReportExcelExporter1.Export(headerLayout2, grid2CaptionsElement.Text,sheet2, 3);
            ReportExcelExporter1.Export(UltraChart1, chart1ElementCaption.Text, sheet3, 3);
            
        }
      #endregion

      #region Экспорт в Pdf

        private void PdfExportButton_Click(Object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();
            
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
            
            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout1,grid1CaptionElement.Text, section1);
            ReportPDFExporter1.Export(headerLayout2,grid2CaptionsElement.Text,section2);
            ReportPDFExporter1.Export(UltraChart1, chart1ElementCaption.Text, section3);
            
        }
        
      #endregion

    #endregion


       }
}