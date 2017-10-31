using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebChart;
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


namespace Krista.FM.Server.Dashboards.reports.FO_0005_0001
{
    public partial class Default: CustomReportPage
    {
      #region Поля

        private DataTable dtGrid;
        private DataTable dtChart;

        private int firstYear = 2006;
        private int endYear;

        private GridHeaderLayout headerLayout;

        public CustomParam LastHalf;
        public CustomParam LastQuater;
        public CustomParam LastMonth;
        public CustomParam Measures;
        
        public string curDate;
        public string lastDate;
        public DateTime currentDate;

      #endregion

        
      protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width =  CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 30);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2.5);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth - 30);
            UltraChart1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight/1.48);
            
            #region инициализация параметров
             if (LastHalf ==null)
             {
                 LastHalf = UserParams.CustomParam("lastHalf");
             }

             if (LastQuater == null)
             {
                 LastQuater = UserParams.CustomParam("lastQuater");
             }

             if (LastMonth == null)
             {
                 LastMonth = UserParams.CustomParam("lastMonth");
             }
          
            #endregion

            #region  Настройка диаграмм
            
            //System.Drawing.Font font = new System.Drawing.Font("Verdana", 8);
            UltraChart1.ChartType = ChartType.StackColumnChart;
            UltraChart1.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChart1.Border.Thickness = 0;
            UltraChart1.Axis.X.Labels.Visible = false;
            UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.VerticalLeftFacing;
            UltraChart1.Axis.X.Extent = 100;
            UltraChart1.Axis.Y.Labels.Visible = true;
            UltraChart1.Axis.Y.Labels.SeriesLabels.Visible = true;
            UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
            UltraChart1.Axis.Y.Extent = 100;
            UltraChart1.Data.SwapRowsAndColumns = false;
            UltraChart1.TitleTop.Visible = true;
            UltraChart1.TitleTop.HorizontalAlign = StringAlignment.Center;
            UltraChart1.TitleTop.Text = "тыс.руб.";
            UltraChart1.Legend.Visible = true;
            UltraChart1.Legend.Location = LegendLocation.Left;
            UltraChart1.Legend.SpanPercentage = 40;
            UltraChart1.Tooltips.FormatString = "<SERIES_LABEL> \n <ITEM_LABEL> \n <DATA_VALUE:N2> тыс.руб.";
          
            UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
         
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
                string query = DataProvider.GetQueryText("FO_0005_0001_Data");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                string month = dtDate.Rows[0][3].ToString();
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                
                ComboYear.Title = "Год";
                ComboYear.Visible = true;
                ComboYear.Width = 100;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.MultiSelect = false;
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Visible = true;
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month,true);

                FillComboRegions();
                ComboFO.Title = "Террирория";
                ComboFO.Visible = true;
                ComboFO.Width = 350;
                ComboFO.MultiSelect = false;
                ComboFO.ParentSelect = true;
                ComboFO.SetСheckedState("Уральский федеральный округ", true);

                FillComboBudget();
                ComboBudget.Title = "Уровень бюджета";
                ComboBudget.Visible = true;
                ComboBudget.Width = 350;
                ComboBudget.MultiSelect = false;
                ComboBudget.SetСheckedState("Консолидированный бюджет субъекта", true);

            }

            PageSubTitle.Text = string.Empty;
            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue), 1);
            
            Page.Title = "Динамика просроченной кредиторской задолженности";
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("Динамика просроченной кредиторской задолженности за {2} {3} года, {0}, {1}", ComboBudget.SelectedValue.ToLower(), ComboFO.SelectedValue, ComboMonth.SelectedValue.ToLower(), ComboYear.SelectedValue);
            UserParams.SubjectFO.Value = ComboFO.SelectedValue == "Уральский федеральный округ"
                                             ? String.Empty
                                             : String.Format(".[{0}]", ComboFO.SelectedValue);
            int year = Convert.ToInt32(ComboYear.SelectedValue);
            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}",CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}",CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            LastHalf.Value = string.Format("Полугодие {0}", ComboMonth.SelectedIndex == 0 ? CRHelper.HalfYearNumByMonthNum(12) : CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex)); 
            LastQuater.Value = string.Format("Квартал {0}", ComboMonth.SelectedIndex == 0 ? CRHelper.QuarterNumByMonthNum(12) : CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex));
            LastMonth.Value = ComboMonth.SelectedIndex == 0 ? CRHelper.RusMonth(12) : CRHelper.RusMonth(ComboMonth.SelectedIndex);
            UserParams.SKIFLevel.Value = string.Format("[Уровни бюджета].[СКИФ].[Все].[{0}]", ComboBudget.SelectedValue); 
            
           chart1ElementCaption.Text =
               string.Format(
                   "{2}. Структурная динамика просроченной кредиторской задолженности за {0} - {1} годы",year-1,year,ComboFO.SelectedValue);

            headerLayout = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();
            UltraChart1.DataBind();
       }

       private void FillComboRegions()
        {
            DataTable dtRegions = new DataTable();
            string query = DataProvider.GetQueryText("FO_0005_0001_Regions");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Район", dtRegions);

            Dictionary<string, int> regions = new Dictionary<string, int>();
            foreach (DataRow row in dtRegions.Rows)
            {
                regions.Add(row[0].ToString(), 0);
            }
            regions.Add("Уральский федеральный округ", 0);
            ComboFO.FillDictionaryValues(regions);
        }

        private void FillComboBudget()
        {
            DataTable dtBudget = new DataTable();
            string query = DataProvider.GetQueryText("FO_0005_0001_Budget");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query,"Уровень бюджета", dtBudget);

            Dictionary <string,int> budget = new Dictionary<string, int>();
            foreach (DataRow row in dtBudget.Rows)
            {
                budget.Add(row[0].ToString(),0);
            }
            ComboBudget.FillDictionaryValues(budget);

        }

        #region Обработчик грида

       

        protected void UltraWebGrid_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0005_0001_Grid");
            dtGrid = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query,"Показатели",dtGrid);
            
            if (dtGrid.Rows.Count > 1)
            {
                UltraWebGrid1.DataSource = dtGrid;
            }
        }

      

        protected void UltraWebGrid_InitializeLayout(Object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(210);


            headerLayout.AddCell("Вид задолженности");
            headerLayout.AddCell(string.Format("Задолженность на  {0:dd.MM.yyyy}, тыс.руб.",currentDate.AddYears(-1)), "Сумма задолженности за аналогичный период предыдущего года");
            headerLayout.AddCell(string.Format("Задолженность на  01.01.{0}, тыс.руб.", ComboYear.SelectedValue), "Сумма задолженности на начало года");
            headerLayout.AddCell(string.Format("Задолженность на  {0:dd.MM.yyyy}, тыс.руб.", currentDate.AddMonths(-1)), "Сумма задолженности за предыдущий месяц");
            headerLayout.AddCell(string.Format("Задолженность на  {0:dd.MM.yyyy}, тыс.руб.", currentDate), string.Format("Сумма задолженности за {0}",ComboMonth.SelectedValue));

            GridHeaderCell cell0 = headerLayout.AddCell("Отклонение от аналогичного периода прошлого года");
            cell0.AddCell("Отклонение от аналогичного периода предыдущего года, тыс. руб.", string.Format(""));
            cell0.AddCell("Темп роста к аналогичному периоду, %", string.Format("Темп роста к аналогичному периоду предыдущего года"));

            GridHeaderCell cell1 = headerLayout.AddCell("Изменение с начала года");
            cell1.AddCell("Изменение с начала года, тыс. руб.", string.Format(""));
            cell1.AddCell("Темп роста к началу года, %", string.Format("Темп роста к началу года"));

            GridHeaderCell cell2 = headerLayout.AddCell("Изменение относительно предыдущего месяца");
            cell2.AddCell("Изменение относительно предыдущего месяца, тыс. руб. ", string.Format(""));
            cell2.AddCell("Темп роста к предыдущему месяцу, %", string.Format(""));

            headerLayout.AddCell("Удельный вес в общей величине задолженности, %", string.Format("Удельный вес в общей величине задолженности по состоянию на {0:dd:MM:yyy}",currentDate));

            headerLayout.ApplyHeaderInfo();
            
        
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                   e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                   CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");
                   e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(95);
                }
       
           for (int i=6; i<e.Layout.Bands[0].Columns.Count; i+=2)
           {
               CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P2");
           }

           CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[11], "P2");
        }

        protected void UltraWebGrid_InitializeRow(Object sender, RowEventArgs e )
        {
            e.Row.Cells[0].Style.HorizontalAlign = HorizontalAlign.Left;

            string image = string.Empty;
            string title = string.Empty;

            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                if (i == 6 || i == 8 || i == 10)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        if (100*Convert.ToDouble(e.Row.Cells[i].Value) > 100)
                        {
                            switch (i)
                            {
                                case 6:
                                    {
                                        image = "~/images/arrowRedUpBB.png";
                                        title =
                                            "Задолженность увеличилась по сравнению с аналогичным периодом прошлого года";
                                        break;
                                    }
                                case 8:
                                    {
                                        image = "~/images/arrowRedUpBB.png";
                                        title = "Задолженность увеличилась по сравнению с началом года";
                                        break;
                                    }
                                case 10:
                                    {
                                        image = "~/images/arrowRedUpBB.png";
                                        title = "Задолженность увеличилась по сравнению с предыдущим месяцем";
                                        break;
                                    }

                            }


                        }
                        else if (100*Convert.ToDouble(e.Row.Cells[i].Value) < 100)
                        {
                            switch (i)
                            {
                                case 6:
                                    {
                                        image = "~/images/arrowGreenDownBB.png";
                                        title =
                                            "Задолженность сократилась по сравнению с аналогичным периодом прошлого года";
                                        break;
                                    }
                                case 8:
                                    {
                                        image = "~/images/arrowGreenDownBB.png";
                                        title = "Задолженность сократилась по сравнению с началом года";
                                        break;
                                    }
                                case 10:
                                    {
                                        image = "~/images/arrowGreenDownBB.png";
                                        title = "Задолженность сократилась по сравнению с предыдущим месяцем";
                                        break;
                                    }

                            }
                        }

                    }
                    e.Row.Cells[i].Style.BackgroundImage = image;
                    e.Row.Cells[i].Title = title;
                }
                e.Row.Cells[i].Style.CustomRules =
                    "background-repeat: no-repeat; background-position: left center; margin: 2px";



                UltraGridCell cell = e.Row.Cells[i];
                if (cell.Value != null && cell.Value.ToString() != String.Empty)
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

        #endregion 

        #region Обработчики диаграмм 
        
          protected  void UltraChart1_DataBinding(Object sender, EventArgs e)
          {
              string query = DataProvider.GetQueryText("FO_0005_0001_Chart");
              dtChart = new DataTable();
              DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Диаграмма1" , dtChart);
              if (dtChart.Columns.Count >=1)
              {
                  UltraChart1.DataSource = dtChart;
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
           
            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
            ReportExcelExporter1.Export(UltraChart1, chart1ElementCaption.Text, sheet2, 3);
            
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
            
            UltraChart1.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));
           
            ReportPDFExporter1.HeaderCellHeight = 50;
            ReportPDFExporter1.Export(headerLayout, section1);
            ReportPDFExporter1.Export(UltraChart1, chart1ElementCaption.Text, section2);
           

        }


        #endregion

      #endregion


       }
}