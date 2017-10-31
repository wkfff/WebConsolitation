using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Collections.ObjectModel;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report.Table;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.WebUI.UltraWebChart;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.DocumentExport;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using EndExportEventArgs = Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs;
using Font = Infragistics.Documents.Reports.Graphics.Font;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.Documents.Excel;

namespace Krista.FM.Server.Dashboards.reports.FNS_0004_0002
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate;
        private DataTable dtData;
        private DataTable dtChart;
        private DataTable dtChart2;
        private int firstYear = 2000;
        private int endYear = 2011;

        private GridHeaderLayout headerLayout;

        //параметры
        private CustomParam selectedFO;
        
        public bool RFSelected
        {
            get { return ComboFO.SelectedIndex == 0; }
        }
        
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 15);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight * 0.45);

            UltraChartFF.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth);
            UltraChartFF.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight/2);

            UltraWebGrid.DataBound += new EventHandler(UltraWebGrid_DataBound);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            #region Инициализация параметров


            if (selectedFO == null)
            {
                selectedFO = UserParams.CustomParam("selected_fo");
            }
            
            #endregion

            #region Настройка диаграммы

            UltraChartFF.ChartType = ChartType.PieChart;
            UltraChartFF.Border.Thickness = 0;
           
            UltraChartFF.Legend.Visible = true;
            UltraChartFF.Legend.Location = LegendLocation.Left;
            UltraChartFF.Legend.SpanPercentage = 36;
            UltraChartFF.Legend.Margins.Bottom = Convert.ToInt32(UltraChartFF.Height.Value / 2);
            UltraChartFF.PieChart.OthersCategoryPercent = 0;
            System.Drawing.Font font = new System.Drawing.Font("Verdana", 9);
            UltraChartFF.Legend.Font = font;
            UltraChartFF.PieChart.Labels.Font = font;
           
            UltraChartFF.PieChart.StartAngle = 160;

            UltraChartFF.ColorModel.ModelStyle = ColorModels.PureRandom;
            UltraChartFF.ChartDrawItem += new ChartDrawItemEventHandler(UltraChartFF_ChartDrawItem);
            UltraChartFF.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);
          
           #endregion

            GridSearch1.LinkedGridId = UltraWebGrid.ClientID;
            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click); 
           }
        void UltraChartFF_ChartDrawItem(object sender, ChartDrawItemEventArgs e)
        {
            //устанавливаем ширину текста легенды 
            Text text = e.Primitive as Text;
            if ((text != null) && !(string.IsNullOrEmpty(text.Path)) && text.Path.EndsWith("Legend"))
            {
                int widthLegendLabel;

                if ((UltraChartFF.Legend.Location == LegendLocation.Top) || (UltraChartFF.Legend.Location == LegendLocation.Bottom))
                {
                    widthLegendLabel = (int)UltraChartFF.Width.Value - 20;
                }
                else
                {
                    widthLegendLabel = ((int)UltraChartFF.Legend.SpanPercentage * (int)UltraChartFF.Width.Value / 100) - 20;
                }

                widthLegendLabel -= UltraChartFF.Legend.Margins.Left + UltraChartFF.Legend.Margins.Right;
                if (text.labelStyle.Trimming != StringTrimming.None)
                {
                    text.bounds.Width = widthLegendLabel;
                }
            }
        }
        
        protected override void Page_Load(Object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (!Page.IsPostBack)   
            { //Инициализация элементов управления при первом обращении

                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FNS_0004_0002_Date");
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);
                
                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(dtDate.Rows[0][3].ToString(), true);

                UserParams.Filter.Value = "Все федеральные округа";
                ComboFO.Title = "ФО";
                ComboFO.Width = 254;
                ComboFO.MultiSelect =false;
                ComboFO.FillDictionaryValues(CustomMultiComboDataHelper.FillFONames(RegionsNamingHelper.FoNames));
                ComboFO.SetСheckedState(UserParams.Filter.Value,true);
              
               if (!string.IsNullOrEmpty(UserParams.Region.Value))
                {
                    ComboFO.SetСheckedState(UserParams.Region.Value, true);
                }
               else if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    ComboFO.SetСheckedState(RegionsNamingHelper.GetFoBySubject(RegionSettings.Instance.Name), true);
                }

               lbSubject.Text = string.Empty;
               
        }
            lbSubject.Text = string.Empty;
            Label1.Text = string.Empty;
            Label2.Text = string.Empty;

            Label1.Text = "Задолженность по налогам и сборам, пеням и налоговым санкциям в бюджетную систему РФ";
            if (CRHelper.MonthNum(ComboMonth.SelectedValue)+1 < 10)
            {
                Label2.Text = string.Format("В отчете приводятся данные по состоянию на 01.0{0}.{1}, в тыс.руб.",
                                            CRHelper.MonthNum(ComboMonth.SelectedValue)+1, ComboYear.SelectedValue);
            }
            else
            {
                if (CRHelper.MonthNum(ComboMonth.SelectedValue) + 1 == 13)
                {
                    Label2.Text = string.Format("В отчете приводятся данные по состоянию на 01.01.{0}, в тыс.руб.",
                                          Convert.ToInt32(ComboYear.SelectedValue) + 1);
                }
                else
                {


                    Label2.Text = string.Format("В отчете приводятся данные по состоянию на 01.{0}.{1}, в тыс.руб.",
                                                CRHelper.MonthNum(ComboMonth.SelectedValue) + 1, ComboYear.SelectedValue);
                }
            }

            selectedFO.Value = RFSelected ? " " : string.Format(".[{0}]", ComboFO.SelectedValue);
          
            int year = Convert.ToInt32(ComboYear.SelectedValue);

            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value =
                string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value =
                string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));

          
         
              headerLayout = new GridHeaderLayout(UltraWebGrid);
              UltraWebGrid.Bands.Clear();
              UltraWebGrid.DataBind();

              string patternValue = lbSubject.Text;
              int defaultRowIndex = 1;
              if (patternValue == string.Empty)
              {
                  patternValue = UserParams.StateArea.Value;
                  defaultRowIndex = 0;
              }

              UltraGridRow row = CRHelper.FindGridRow(UltraWebGrid, patternValue, 0, defaultRowIndex);
              ActivateGridRow(row);
              UltraChartFF.DataBind();
       
        }

      #region обработчик грида

        private void ActivateGridRow(UltraGridRow row)
        {
            if (row == null)
                return;

            string subject = row.Cells[0].Text;
            lbSubject.Text = subject;

            if (CRHelper.MonthNum(ComboMonth.SelectedValue)+1 < 10)
            {
                lbSubject.Text =
                    string.Format(
                        "{0}. Структура задолженности по основным видам налогов и сборов по состоянию на 01.0{1}.{2}",
                        subject, CRHelper.MonthNum(ComboMonth.SelectedValue)+1, ComboYear.SelectedValue);
            }
            else
            {
                if (CRHelper.MonthNum(ComboMonth.SelectedValue) + 1 == 13)
                {
                    lbSubject.Text =
                        string.Format(
                            "{0}. Структура задолженности по основным видам налогов и сборов по состоянию на 01.01.{1}",
                            subject, Convert.ToInt32(ComboYear.SelectedValue)+1);
                }
                else
                {


                    lbSubject.Text =
                        string.Format(
                            "{0}. Структура задолженности по основным видам налогов и сборов по состоянию на 01.{1}.{2}",
                            subject, CRHelper.MonthNum(ComboMonth.SelectedValue) + 1, ComboYear.SelectedValue);
                }
            }

            UltraChartFF.Tooltips.FormatString = string.Format("{0}\n <ITEM_LABEL> \n <DATA_VALUE:N2> тыс. руб.,\n Удельный вес от общей суммы задолженности \n <PERCENT_VALUE:#0.00>%  ", subject);
            

            if (RegionsNamingHelper.IsRF(subject))
            {
                UserParams.Subject.Value = "]";
                UserParams.SubjectFO.Value = "]";
            }
            else if (RegionsNamingHelper.IsFO(subject))
            {
                UserParams.Region.Value = subject;
                UserParams.Subject.Value = string.Format("].[{0}]", UserParams.Region.Value);
                UserParams.SubjectFO.Value = string.Format("].[{0}]", UserParams.Region.Value);
            }
            else
            {
                UserParams.Region.Value = RegionsNamingHelper.FullName(row.Cells[1].Text);
                UserParams.StateArea.Value = subject;
                UserParams.Subject.Value = string.Format("].[{0}].[{1}]", UserParams.Region.Value, UserParams.StateArea.Value);
                UserParams.SubjectFO.Value = string.Format("].[{0}]", UserParams.Region.Value);
            }

            UltraChartFF.DataBind();
          
          
        }
        
       
        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FNS_0004_0002_Grid");
            dtData = new DataTable();
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Субъект", dtData);
           
                for (int i = 0; i < dtData.Rows.Count; i++)
                {
                    for (int j = 2; j < dtData.Columns.Count; j++)
                    {
                        if (dtData.Rows[i][j] != DBNull.Value)
                        {
                            dtData.Rows[i][j] = Convert.ToDouble(dtData.Rows[i][j]) / 1000;

                        }
                    }
                }
            
         
            if (dtData.Columns.Count > 2)
             {
                dtData.Columns[1].ColumnName = "ФО";
                UltraWebGrid.DataSource = dtData;
             }
          
        }

        void UltraWebGrid_DataBound(object sender, EventArgs e)
        {
            if (ComboFO.SelectedIndex != 0)
            {
                UltraWebGrid.Height = Unit.Empty;
            }
        }
        
       protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
           

            for (int i=2;i<e.Layout.Bands[0].Columns.Count; i++)
              {
                  CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                  e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
                 
              }
            
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(250);
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(45);
          
           headerLayout.AddCell("Субъект", "Субъект РФ", 4);
           headerLayout.AddCell("ФО", "Федеральный округ", 4);

           for (int i=2; i<UltraWebGrid.Columns.Count; i+=5)
           {
               string[] caption = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');

               if (CRHelper.MonthNum(ComboMonth.SelectedValue) + 1 < 10)
               {
                   GridHeaderCell cell0 = headerLayout.AddCell(caption[0], " ");
                   GridHeaderCell cell1_1 = cell0.AddCell("Задолженность по налогам и сборам", "");
                   cell1_1.AddCell("Всего", string.Format("Задолженность по налогам и сборам по состоянию на 01.0{0}.{1}", CRHelper.MonthNum(ComboMonth.SelectedValue)+1, ComboYear.SelectedValue));
                   GridHeaderCell cell2_1 = cell1_1.AddCell("в том числе");
                   cell2_1.AddCell("Недоимка по налогам и сборам", string.Format("Недоимка по налогам и сборам по состоянию на 01.0{0}.{1}", CRHelper.MonthNum(ComboMonth.SelectedValue)+1, ComboYear.SelectedValue));
                   cell2_1.AddCell("Отсроченные (рассроченные) платежи по налогам и сборам", string.Format("Отсроченные и рассроченные платежи по состоянию на 01.0{0}.{1}", CRHelper.MonthNum(ComboMonth.SelectedValue)+1, ComboYear.SelectedValue));

                   GridHeaderCell cell1_2 = cell0.AddCell("Задолженность по пени и налоговым санкциям ", "");
                   cell1_2.AddCell("Всего", string.Format("Задолженность по пени и налоговым санкциям по состоянию на 01.0{0}.{1}", CRHelper.MonthNum(ComboMonth.SelectedValue)+1, ComboYear.SelectedValue));
                   cell1_2.AddCell("в том числе").AddCell("Отсроченные и рассроченные пени и налоговые санкции", string.Format("Отсроченные и рассроченные пени и налоговые санкции по состоянию на 01.0{0}.{1}", CRHelper.MonthNum(ComboMonth.SelectedValue)+1, ComboYear.SelectedValue));
               }
               else
               {
                   if (CRHelper.MonthNum(ComboMonth.SelectedValue) + 1 == 13)
                   {
                       GridHeaderCell cell0 = headerLayout.AddCell(caption[0], " ");
                       GridHeaderCell cell1_1 = cell0.AddCell("Задолженность по налогам и сборам", "");
                       cell1_1.AddCell("Всего", string.Format("Задолженность по налогам и сборам по состоянию на 01.01.{1}", CRHelper.MonthNum(ComboMonth.SelectedValue), Convert.ToInt32(ComboYear.SelectedValue)+1));
                       GridHeaderCell cell2_1 = cell1_1.AddCell("в том числе");
                       cell2_1.AddCell("Недоимка по налогам и сборам", string.Format("Недоимка по налогам и сборам по состоянию на 01.01.{1}", CRHelper.MonthNum(ComboMonth.SelectedValue), Convert.ToInt32(ComboYear.SelectedValue) + 1));
                       cell2_1.AddCell("Отсроченные (рассроченные) платежи по налогам и сборам", string.Format("Отсроченные и рассроченные платежи по состоянию на 01.01.{1}", CRHelper.MonthNum(ComboMonth.SelectedValue), Convert.ToInt32(ComboYear.SelectedValue) + 1));

                       GridHeaderCell cell1_2 = cell0.AddCell("Задолженность по пени и налоговым санкциям ", "");
                       cell1_2.AddCell("Всего", string.Format("Задолженность по пени и налоговым санкциям по состоянию на 01.01.{1}", CRHelper.MonthNum(ComboMonth.SelectedValue), Convert.ToInt32(ComboYear.SelectedValue) + 1));
                       cell1_2.AddCell("в том числе").AddCell("Отсроченные и рассроченные пени и налоговые санкции", string.Format("Отсроченные и рассроченные пени и налоговые санкции по состоянию на 01.01.{1}", CRHelper.MonthNum(ComboMonth.SelectedValue), Convert.ToInt32(ComboYear.SelectedValue) + 1));
                   }
                   else
                   {
                       GridHeaderCell cell0 = headerLayout.AddCell(caption[0], " ");
                       GridHeaderCell cell1_1 = cell0.AddCell("Задолженность по налогам и сборам", "");
                       cell1_1.AddCell("Всего", string.Format("Задолженность по налогам и сборам по состоянию на 01.{0}.{1}", CRHelper.MonthNum(ComboMonth.SelectedValue) + 1, ComboYear.SelectedValue));
                       GridHeaderCell cell2_1 = cell1_1.AddCell("в том числе");
                       cell2_1.AddCell("Недоимка по налогам и сборам", string.Format("Недоимка по налогам и сборам по состоянию на 01.{0}.{1}", CRHelper.MonthNum(ComboMonth.SelectedValue) + 1, ComboYear.SelectedValue));
                       cell2_1.AddCell("Отсроченные (рассроченные) платежи по налогам и сборам", string.Format("Отсроченные и рассроченные платежи по состоянию на 01.{0}.{1}", CRHelper.MonthNum(ComboMonth.SelectedValue) + 1, ComboYear.SelectedValue));

                       GridHeaderCell cell1_2 = cell0.AddCell("Задолженность по пени и налоговым санкциям ", "");
                       cell1_2.AddCell("Всего", string.Format("Задолженность по пени и налоговым санкциям по состоянию на 01.{0}.{1}", CRHelper.MonthNum(ComboMonth.SelectedValue) + 1, ComboYear.SelectedValue));
                       cell1_2.AddCell("в том числе").AddCell("Отсроченные и рассроченные пени и налоговые санкции", string.Format("Отсроченные и рассроченные пени и налоговые санкции по состоянию на 01.{0}.{1}", CRHelper.MonthNum(ComboMonth.SelectedValue) + 1, ComboYear.SelectedValue));

                   }
               }

            }

           headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_ActiveRowChange(object sender, RowEventArgs e)
        {
            ActivateGridRow(e.Row);
        }

       protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            foreach (UltraGridCell cell in e.Row.Cells)
            {
                if (e.Row.Cells[0].Value != null && e.Row.Cells[0].Value.ToString() != string.Empty)
                {
                    if (!RegionsNamingHelper.IsSubject(e.Row.Cells[0].Value.ToString()))
                    {
                        cell.Style.Font.Bold = true;
                    }
                }
             }
          
               foreach (UltraGridCell cell in e.Row.Cells)
               {
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

        #endregion

      #region обработчик диаграмм
        protected void UltraChartFF_DataBinding(object sender, EventArgs e)
        {
            dtChart = new DataTable();
            string query = DataProvider.GetQueryText("FNS_0004_0002_Chart");
            DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "seriesName", dtChart);

           UltraChartFF.DataSource = dtChart;
        }

       
        #endregion

        #region экспорт
          #region экспорт в PDF
          private void PdfExportButton_Click(Object sender, EventArgs e)
            {
              ReportPDFExporter1.PageTitle = Label1.Text;
                //ReportPDFExporter1.PageSubTitle = Label2.Text;

                Report report = new Report();
                ISection section1 = report.AddSection();
                ISection section2 = report.AddSection();

                UltraChartFF.Width = Unit.Pixel((int)(CustomReportConst.minScreenWidth * 0.8));

                ReportPDFExporter1.HeaderCellHeight = 50;
                ReportPDFExporter1.Export(headerLayout, Label2.Text, section1);
                ReportPDFExporter1.Export(UltraChartFF, lbSubject.Text, section2);


           }
     #endregion
          #region экспорт в Excel
     
          private void ExcelExportButton_Click(object sender, EventArgs e)
          {
              ReportExcelExporter1.WorksheetTitle = Label1.Text;
              ReportExcelExporter1.WorksheetSubTitle = Label2.Text;

              Workbook workbook = new Workbook();
              Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
              Worksheet sheet2 = workbook.Worksheets.Add("sheet2");

              ReportExcelExporter1.HeaderCellHeight = 33;
              ReportExcelExporter1.GridColumnWidthScale = 1.1;
              ReportExcelExporter1.Export(headerLayout, sheet1, 3);
              ReportExcelExporter1.Export(UltraChartFF, lbSubject.Text, sheet2, 3);
          }
    #endregion
         
#endregion

      }
}



