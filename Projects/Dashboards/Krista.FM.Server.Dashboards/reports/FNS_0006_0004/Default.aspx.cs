using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
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


namespace Krista.FM.Server.Dashboards.reports.FNS_0006_0004
{
    public partial class Default: CustomReportPage
    {
        #region Поля

        private DataTable dtDate;
        private DataTable dtGrid1;
        private DataTable dtGrid2;
        private DataTable dtChart;

        private string query;
        private int firstYear = 1995;
        private int endYear;
        private string month;
        private DateTime currentDate;

        private Collection<double> max;
        private Collection<double> min;
        private double Max = double.MinValue;
        private double Min = double.MaxValue;

        private CustomParam years;
        private CustomParam yearDescendants;
        private CustomParam monthsList;

        private GridHeaderLayout headerLayout1;
        private GridHeaderLayout headerLayout2;

      #endregion

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2.0);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth/2.0);
            UltraWebGrid2.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2.0);
            UltraWebGrid2.DisplayLayout.NoDataMessage = "Нет данных";

            UltraChart1.Width = CRHelper.GetChartWidth(CustomReportConst.minScreenWidth / 2.0);
            UltraChart1.Height = CRHelper.GetChartHeight(CustomReportConst.minScreenHeight/2.0);

            
            #region инициализация параметров

              years = UserParams.CustomParam("years");
              yearDescendants = UserParams.CustomParam("year_descendants");
              monthsList = UserParams.CustomParam("months");

            #endregion


            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }
       
       protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                query = DataProvider.GetQueryText("FNS_0006_0004_data");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                
                ComboPeriod.MultiSelect = true;
                ComboPeriod.Title = "Год";
                ComboPeriod.Visible = true;
                ComboPeriod.Width = 100;
                ComboPeriod.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboPeriod.SetСheckedState(endYear.ToString(), true);
                ComboPeriod.SetСheckedState((endYear-1).ToString(), true);
                ComboPeriod.SetСheckedState((endYear - 2).ToString(), true);
                ComboPeriod.SetСheckedState((endYear - 3).ToString(), true);
                ComboPeriod.SetСheckedState((endYear - 4).ToString(), true);
                ComboPeriod.SetСheckedState((endYear - 5).ToString(), true);
                ComboPeriod.SetСheckedState((endYear - 6).ToString(), true);
                ComboPeriod.SetСheckedState((endYear - 7).ToString(), true);
                ComboPeriod.SetСheckedState((endYear - 8).ToString(), true);
                ComboPeriod.SetСheckedState((endYear - 9).ToString(), true);
               
            }

            dtDate = new DataTable();
            query = DataProvider.GetQueryText("FNS_0006_0004_data");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            month = dtDate.Rows[0][3].ToString();

            currentDate = month == "Декабрь" ? new DateTime(Convert.ToInt32(ComboPeriod.SelectedValues[ComboPeriod.SelectedValues.Count - 1])+1, 1, 1) : new DateTime(Convert.ToInt32(ComboPeriod.SelectedValues[ComboPeriod.SelectedValues.Count - 1]), CRHelper.MonthNum(month) + 1, 1); 
            Page.Title = string.Format("Справка о поступлении и недоимке собственных доходов в консолидированный бюджет субъекта, по состоянию на {0: dd.MM.yyyy}, млн.руб.", currentDate);
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Empty;

            int lastYear = Convert.ToInt32(ComboPeriod.SelectedValues[ComboPeriod.SelectedValues.Count - 2]);
            UserParams.PeriodLastYear.Value = lastYear.ToString();
            UserParams.PeriodYear.Value = (lastYear + 1).ToString();

            string selectedValues=string.Empty;
            string selectedYears = string.Empty;
            if (ComboPeriod.SelectedValues.Count > 0)
            {
              for (int i=0; i < ComboPeriod.SelectedValues.Count-1 ;i++)
              {
                selectedValues += string.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}],",ComboPeriod.SelectedValues[i]);
              }
              years.Value = selectedValues.TrimEnd(',');

              for (int i = 0; i < ComboPeriod.SelectedValues.Count; i++)
              {
                  selectedYears += string.Format("Descendants ( [Период__Период].[Период__Период].[Данные всех периодов].[{0}], [Период__Период].[Период__Период].[Месяц],SELF_and_before ) +", ComboPeriod.SelectedValues[i]);
              }
                yearDescendants.Value = selectedYears.TrimEnd('+');
            }

           /* int numMonth = CRHelper.MonthNum(ComboMonth.SelectedValue);
            string str = string.Empty;
            for (int i = 1; i <numMonth + 1; i++ )
            {
                str += string.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{0}].[Полугодие {1}].[Квартал {2}].[{3}],", ComboPeriod.SelectedValues[ComboPeriod.SelectedValues.Count-1], CRHelper.HalfYearNumByMonthNum(i) ,CRHelper.QuarterNumByMonthNum(i), CRHelper.RusMonth(i));
            }
            monthsList.Value = str.TrimEnd(',');
           */
            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);
            grid1CaptionElement.Text = string.Format("Исполнение собственных доходов консолидированного бюджета субъекта в период с {0} по {1} год", ComboPeriod.SelectedValues[0], ComboPeriod.SelectedValues[ComboPeriod.SelectedValues.Count-1]);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();

            headerLayout2 = new GridHeaderLayout(UltraWebGrid2);
            UltraWebGrid2.Bands.Clear();
            UltraWebGrid2.DataBind();
           
            UltraChart1.DataBind();
            
        }

   #region Обработчик грида
        
        protected void UltraWebGrid1_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FNS_0006_0004_grid1");
            dtGrid1 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", dtGrid1);
            
            if (dtGrid1.Rows.Count > 0)
            {
               DataTable dtGrid = new DataTable();
               DataRow newRow;
               DataColumn column = new DataColumn("Период", typeof(string));
               dtGrid.Columns.Add(column);
               
                // добавляем колонки  - выбранные в параметре года
               for (int numCol = 0; numCol < ComboPeriod.SelectedValues.Count; numCol++)
               {
                   DataColumn newColumn = new DataColumn(string.Format("{0}", ComboPeriod.SelectedValues[numCol]), typeof(decimal));
                   dtGrid.Columns.Add(newColumn);
               }
               
                // добавляем строки и заполняем таблицу значениями
               int rowCount = 13; 
               for (int numNewRow = 1; numNewRow < rowCount; numNewRow ++ )
               {
                   newRow = dtGrid.NewRow();
                   string month = CRHelper.ToUpperFirstSymbol(CRHelper.RusMonth(numNewRow));
                   newRow[0] = month;

                   for (int rowNum = 0; rowNum < dtGrid1.Rows.Count; rowNum++ )
                   {
                       if (dtGrid1.Rows[rowNum][0].ToString().Contains(month))
                       {
                           string nYears = dtGrid1.Rows[rowNum][1].ToString();
                           if (dtGrid1.Rows[rowNum][2] != DBNull.Value && dtGrid1.Rows[rowNum][2].ToString() != string.Empty)
                           {
                               double value = Convert.ToDouble(dtGrid1.Rows[rowNum][2]);
                               newRow[nYears] = value;
                           }

                       }
                   }
                   
                   dtGrid.Rows.Add(newRow);
               }

               newRow = dtGrid.NewRow();
               newRow[0] ="Итого за год";
               for (int rowNum = 0; rowNum < dtGrid1.Rows.Count; rowNum++)
               {
                   if (dtGrid1.Rows[rowNum][0].ToString().Contains("20") || dtGrid1.Rows[rowNum][0].ToString().Contains("19"))
                   {
                       string nYears = dtGrid1.Rows[rowNum][0].ToString();
                       if (dtGrid1.Rows[rowNum][3] != DBNull.Value && dtGrid1.Rows[rowNum][3].ToString() != string.Empty)
                       {
                           double value = Convert.ToDouble(dtGrid1.Rows[rowNum][3]);
                           newRow[nYears] = value;
                       }

                   }
               }
               dtGrid.Rows.Add(newRow);

               
                   newRow = dtGrid.NewRow();
                   newRow[0] = "";
                   dtGrid.Rows.Add(newRow);

                   newRow = dtGrid.NewRow();
                   newRow[0] = "Квартал 1";
                   for (int rowNum = 0; rowNum < dtGrid1.Rows.Count; rowNum++)
                   {
                       if (dtGrid1.Rows[rowNum][0].ToString().Contains("Квартал 1"))
                       {
                           string nYears = dtGrid1.Rows[rowNum][1].ToString();

                           if (dtGrid1.Rows[rowNum][2] != DBNull.Value &&
                                  dtGrid1.Rows[rowNum][2].ToString() != string.Empty)
                           {
                               double value = Convert.ToDouble(dtGrid1.Rows[rowNum][2]);
                               newRow[nYears] = value;
                           }
                           if (nYears == endYear.ToString() && CRHelper.MonthNum(month) < 3)
                           {
                             newRow[nYears] = DBNull.Value;
                           }

                       }
                   }
                   dtGrid.Rows.Add(newRow);

                   newRow = dtGrid.NewRow();
                   newRow[0] = "Удельный вес в году, %";
                   dtGrid.Rows.Add(newRow);
               
                   newRow = dtGrid.NewRow();
                   newRow[0] = "";
                   dtGrid.Rows.Add(newRow);

                   newRow = dtGrid.NewRow();
                   newRow[0] = "Квартал 2";
                   for (int rowNum = 0; rowNum < dtGrid1.Rows.Count; rowNum++)
                   {
                       if (dtGrid1.Rows[rowNum][0].ToString().Contains("Квартал 2"))
                       {
                           string nYears = dtGrid1.Rows[rowNum][1].ToString();
                           if (dtGrid1.Rows[rowNum][2] != DBNull.Value &&
                                   dtGrid1.Rows[rowNum][2].ToString() != string.Empty)
                           {
                               double value = Convert.ToDouble(dtGrid1.Rows[rowNum][2]);
                               newRow[nYears] = value;
                           }

                           if (nYears == endYear.ToString() && CRHelper.MonthNum(month) < 6)
                           {
                              newRow[nYears] = DBNull.Value;
                           }

                       }
                   }
                   dtGrid.Rows.Add(newRow);
                   newRow = dtGrid.NewRow();
                   newRow[0] = "Удельный вес в году, %";
                   dtGrid.Rows.Add(newRow);
               
                   newRow = dtGrid.NewRow();
                   newRow[0] = "";
                   dtGrid.Rows.Add(newRow);

                   newRow = dtGrid.NewRow();
                   newRow[0] = "Квартал 3";
                   for (int rowNum = 0; rowNum < dtGrid1.Rows.Count; rowNum++)
                   {
                       if (dtGrid1.Rows[rowNum][0].ToString().Contains("Квартал 3"))
                       {
                           string nYears = dtGrid1.Rows[rowNum][1].ToString();
                           if (dtGrid1.Rows[rowNum][2] != DBNull.Value &&
                                   dtGrid1.Rows[rowNum][2].ToString() != string.Empty)
                           {
                               double value = Convert.ToDouble(dtGrid1.Rows[rowNum][2]);
                               newRow[nYears] = value;
                           }
                           if (nYears == endYear.ToString() && CRHelper.MonthNum(month) < 9)
                           {
                             newRow[nYears] = DBNull.Value;
                           }

                       }
                   }
                   dtGrid.Rows.Add(newRow);
                   newRow = dtGrid.NewRow();
                   newRow[0] = "Удельный вес в году, %";
                   dtGrid.Rows.Add(newRow);
               
               
                   newRow = dtGrid.NewRow();
                   newRow[0] = "";
                   dtGrid.Rows.Add(newRow);

                   newRow = dtGrid.NewRow();
                   newRow[0] = "Квартал 4";
                   for (int rowNum = 0; rowNum < dtGrid1.Rows.Count; rowNum++)
                   {
                       if (dtGrid1.Rows[rowNum][0].ToString().Contains("Квартал 4"))
                       {
                           string nYears = dtGrid1.Rows[rowNum][1].ToString();
                           if (dtGrid1.Rows[rowNum][2] != DBNull.Value &&
                                  dtGrid1.Rows[rowNum][2].ToString() != string.Empty)
                           {
                               double value = Convert.ToDouble(dtGrid1.Rows[rowNum][2]);
                               newRow[nYears] = value;
                           }
                           if (nYears == endYear.ToString() && CRHelper.MonthNum(month) < 12)
                           {
                              newRow[nYears] = DBNull.Value;
                           }

                       }
                   }
                   dtGrid.Rows.Add(newRow);
                   newRow = dtGrid.NewRow();
                   newRow[0] = "Удельный вес в году, %";
                   dtGrid.Rows.Add(newRow);
               
               dtGrid.AcceptChanges();

                int n = 12; // номер строки "итого за год"
               for (int rowNum = 0; rowNum < dtGrid.Rows.Count; rowNum++)
               {
                   if (dtGrid.Rows[rowNum][0].ToString().Contains("Удельный вес"))
                   {
                       for (int colNum = 1; colNum < dtGrid.Columns.Count; colNum++)
                       {
                           if (dtGrid.Rows[rowNum - 1][colNum] != DBNull.Value && dtGrid.Rows[rowNum - 1][colNum].ToString() != string.Empty && dtGrid.Rows[n][colNum] != DBNull.Value && dtGrid.Rows[n][colNum].ToString() != string.Empty )
                           {
                               if (Convert.ToDouble(dtGrid.Rows[n][colNum]) != 0)
                               {
                                   dtGrid.Rows[rowNum][colNum] = Convert.ToDouble(dtGrid.Rows[rowNum-1][colNum]) / Convert.ToDouble(dtGrid.Rows[n][colNum]) *100;
                               }
                               else
                               {
                                   dtGrid.Rows[rowNum][colNum] = DBNull.Value;
                               }
                           }
                           else
                           {
                               dtGrid.Rows[rowNum][colNum] = DBNull.Value;
                           }
                       }
                   }
               }
               dtGrid.AcceptChanges();

               max = new Collection<double>();
               min = new Collection<double>();
               for (int colNum = 1; colNum < dtGrid.Columns.Count; colNum++ )
               {
                  Max = double.MinValue;
                  Min = double.MaxValue;
                  for (int rowNum=0; rowNum < 12; rowNum++)
                  {
                      if (dtGrid.Rows[rowNum][colNum] != DBNull.Value && dtGrid.Rows[rowNum][colNum].ToString() != string.Empty)
                      {
                          double value = Convert.ToDouble(dtGrid.Rows[rowNum][colNum]);
                          if (value >= Max)
                          {
                              Max = value;
                          }
                          if (value <= Min)
                          {
                              Min = value;
                          }
                      }
                  }
                   max.Add(Max);
                   min.Add(Min);
               }

                UltraWebGrid1.DataSource = dtGrid;
            }
            
          
        }

        protected void UltraWebGrid1_InitializeLayout(Object sender, LayoutEventArgs e)
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

            headerLayout1.AddCell("Показатели");
            for (int i = 0; i < ComboPeriod.SelectedValues.Count; i++ )
            {
                headerLayout1.AddCell(string.Format("{0}", ComboPeriod.SelectedValues[i]));
            }
            headerLayout1.ApplyHeaderInfo();
         
            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
                {
                    e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                    e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(80);
                    CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N0");

                }
        }

        protected void UltraWebGrid1_InitializeRow(Object sender, RowEventArgs e )
        {
            
            if (e.Row.Cells[0].Value.ToString() == "Итого за год")
            {
                for (int i = 0; i < UltraWebGrid1.Columns.Count; i++)
                {
                    e.Row.Cells[i].Style.Font.Bold = true;
                }
            }
           
            for (int i=1; i<e.Row.Cells.Count; i++)
              {
                  if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                  {
                      CRHelper.SaveToErrorLog(string.Format("i = {2}, value = {0}, max ={1}", Convert.ToDouble(e.Row.Cells[i].Value), max[i-1], i-1));
                      if (Convert.ToDouble(e.Row.Cells[i].Value) == max[i-1])
                      {
                          e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                          e.Row.Cells[i].Title = "Месяц с наибольшим объемом поступлений доходов в году";
                      }
                      else if (Convert.ToDouble(e.Row.Cells[i].Value) == min[i-1])
                      {
                          e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                          e.Row.Cells[i].Title = "Месяц с наименьшим объемом поступлений доходов в году";
                      }

                      e.Row.Cells[i].Style.CustomRules =
                          "background-repeat: no-repeat; background-position: left center; margin: 2px";
                  }
              }


            if (e.Row.Cells[0].Value.ToString() == "Удельный вес в году, %")
            {
                for (int i = 1; i < UltraWebGrid1.Columns.Count; i++)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        double value = Convert.ToDouble(e.Row.Cells[i].Value);
                        e.Row.Cells[i].Value = string.Format("{0:N1}", value);
                    }
                }
            }
             
           
          }

        protected void UltraWebGrid2_DataBinding(Object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FNS_0006_0004_grid2");
            dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Показатели", dtGrid2);
            DateTime currentdate;
            if  (dtGrid2.Rows.Count > 0 )
            {
                for (int rowNum = 0; rowNum < dtGrid2.Rows.Count; rowNum++ )
                {
                    string nameRow = dtGrid2.Rows[rowNum][0].ToString();
                    if (nameRow.Contains("20") || nameRow.Contains("19"))
                    {
                        dtGrid2.Rows[rowNum][0] = string.Format("{0} г.", nameRow);
                    }
                    else
                    {
                        int year =  Convert.ToInt32(ComboPeriod.SelectedValues[ComboPeriod.SelectedValues.Count - 1]);
                        int month = CRHelper.MonthNum(nameRow);
                        currentdate = new DateTime(month == 12 ? year+1: year, month == 12 ? 1 : month +1, 1);
                        dtGrid2.Rows[rowNum][0] = string.Format("На {0:dd.MM.yyyy}", currentdate);
                    }
                }

                grid2CaptionsElement.Text = string.Format("Недоимка по платежам в бюджетную систему в период с {0} по {1} год", dtGrid2.Rows[0][0], ComboPeriod.SelectedValues[ComboPeriod.SelectedValues.Count - 1]);
                chart1ElementCaption.Text =
              string.Format(
                  "Доля недоимки в сумме фактических поступлений доходов за период с {0} по {1} год", dtGrid2.Rows[0][0], ComboPeriod.SelectedValues[ComboPeriod.SelectedValues.Count - 2]);
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
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(90);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.Bands[0].Columns[UltraWebGrid2.Columns.Count - 1].Hidden = true;

            headerLayout2.AddCell("Период");
            headerLayout2.AddCell("Всего, млн.руб.");
            GridHeaderCell cell1 = headerLayout2.AddCell("Прирост недоимки с начала года");
            cell1.AddCell("млн. руб.");
            cell1.AddCell("Удельный вес, %");
            headerLayout2.AddCell("Ранг по приросту недоимки, %");
            GridHeaderCell cell2 = headerLayout2.AddCell("в том числе:");
            cell2.AddCell("федеральные налоги, млн. руб.");
            cell2.AddCell("Удельный вес, %", "Удельный вес в общей сумме недоимки");
            cell2.AddCell("региональные налоги, млн. руб.");
            cell2.AddCell("Удельный вес, %", "Удельный вес в общей сумме недоимки");
            cell2.AddCell("местные налоги, млн. руб.");
            cell2.AddCell("Удельный вес, %", "Удельный вес в общей сумме недоимки");
            cell2.AddCell("Спец. налоговый режим, млн. руб.");
            cell2.AddCell("Удельный вес, %", "Удельный вес в общей сумме недоимки");
            headerLayout2.ApplyHeaderInfo();

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            }

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(55);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[1], "N0");
            e.Layout.Bands[0].Columns[2].Width = CRHelper.GetColumnWidth(40);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[2], "N0");
            e.Layout.Bands[0].Columns[3].Width = CRHelper.GetColumnWidth(70);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[3], "N1");
            e.Layout.Bands[0].Columns[4].Width = CRHelper.GetColumnWidth(70);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[4], "N0");
            e.Layout.Bands[0].Columns[5].Width = CRHelper.GetColumnWidth(90);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[5], "N0");
            e.Layout.Bands[0].Columns[6].Width = CRHelper.GetColumnWidth(70);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[6], "N1");
            e.Layout.Bands[0].Columns[7].Width = CRHelper.GetColumnWidth(90);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[7], "N0");
            e.Layout.Bands[0].Columns[8].Width = CRHelper.GetColumnWidth(70);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[8], "N1");
            e.Layout.Bands[0].Columns[9].Width = CRHelper.GetColumnWidth(60);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[9], "N0");
            e.Layout.Bands[0].Columns[10].Width = CRHelper.GetColumnWidth(70);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[10], "N1");
            e.Layout.Bands[0].Columns[11].Width = CRHelper.GetColumnWidth(70);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[11], "N0");
            e.Layout.Bands[0].Columns[12].Width = CRHelper.GetColumnWidth(70);
            CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[12], "N1");
           

        }

        protected void UltraWebGrid2_InitializeRow(Object sender, RowEventArgs e)
        {
           if (e.Row.Cells[2].Value != null && e.Row.Cells[2].ToString() != string.Empty)
           {
               if (Convert.ToDouble(e.Row.Cells[2].Value) > 0)
               {
                   e.Row.Cells[2].Style.ForeColor = Color.Red;
               }
           }

           if (e.Row.Cells[3].Value != null && e.Row.Cells[3].ToString() != string.Empty)
           {
             if (100 * Convert.ToDouble(e.Row.Cells[3].Value) > 0)
               {
                   
                   e.Row.Cells[3].Style.ForeColor = Color.Red;
               }
           }

           int i = 3;
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

           i = 4;
           if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
           {
               if (Convert.ToDouble(e.Row.Cells[i].Value) ==1)
               {
                   e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                   e.Row.Cells[i].Title = "Наименьший прирост недоимки";
               }
               else if ( Convert.ToDouble(e.Row.Cells[i].Value) == Convert.ToDouble(e.Row.Cells[e.Row.Cells.Count-1].Value))
               {
                   e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                   e.Row.Cells[i].Title = "Наибольший прирост недоимки";
               }

               e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
           }
            
        }
   #endregion 

   #region Обработчики диаграмм 
         
          protected  void UltraChart1_DataBinding(Object sender, EventArgs e)
          {
            if (DebtKindButtonList.SelectedIndex == 0) // абсолютные показатели
              {
                  UltraChart1.ChartType = ChartType.ColumnChart;
                  UltraChart1.Border.Thickness = 0;
                  UltraChart1.Axis.X.Extent = 20;
                  UltraChart1.Axis.Y.Extent = 40;
                  UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:N0>";
                  UltraChart1.Axis.X.Labels.Visible = false;
                  UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
                  UltraChart1.Data.SwapRowsAndColumns = false;
                  UltraChart1.TitleLeft.Visible = true;
                  UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
                  UltraChart1.TitleLeft.Text = "млн. руб";
                  UltraChart1.Legend.Visible = true;
                  UltraChart1.Legend.Location = LegendLocation.Top;
                  UltraChart1.Legend.SpanPercentage = 15;
                  UltraChart1.Tooltips.FormatString = string.Format("<ITEM_LABEL>\n <SERIES_LABEL> г.\n <DATA_VALUE:N0> млн. руб");
                //  UltraChart1.Legend.Margins.Bottom = Convert.ToInt32(UltraChart1.Height.Value / 1.5);
                  UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);


                  string query = DataProvider.GetQueryText("FNS_0006_0004_chart1");
                  dtChart = new DataTable();
                  DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Диаграмма1", dtChart);

                  if (dtChart.Rows.Count > 0)
                  {
                      
                      UltraChart1.DataSource = dtChart;
                  }
              }
            else  // относительные показатели
            {
                UltraChart1.ChartType = ChartType.StackColumnChart;
                UltraChart1.Border.Thickness = 0;
                UltraChart1.Axis.X.Extent = 20;
                UltraChart1.Axis.Y.Extent = 40;
                UltraChart1.Axis.Y.Labels.ItemFormatString = "<DATA_VALUE:P1>";
                UltraChart1.Axis.X.Labels.Visible = false;
                UltraChart1.Axis.X.Labels.SeriesLabels.Orientation = TextOrientation.Horizontal;
                UltraChart1.Data.SwapRowsAndColumns = false;
                UltraChart1.TitleLeft.Visible = true;
                UltraChart1.TitleLeft.HorizontalAlign = StringAlignment.Center;
                UltraChart1.TitleLeft.Text = "%";
                UltraChart1.Legend.Visible = true;
                UltraChart1.Legend.Location = LegendLocation.Top;
                UltraChart1.Legend.SpanPercentage = 25;
                //UltraChart1.Legend.Margins.Bottom = Convert.ToInt32(UltraChart1.Height.Value / 1.5);
                UltraChart1.InvalidDataReceived += new ChartDataInvalidEventHandler(CRHelper.UltraChartInvalidDataReceived);

                string query = DataProvider.GetQueryText("FNS_0006_0004_chart2");
                dtChart = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Диаграмма1", dtChart);

                if (dtChart.Rows.Count > 0)
                {
                    UltraChart1.Tooltips.FormatString = string.Format("<DATA_VALUE:P1>\n <ITEM_LABEL> в общей сумме поступлений \n в <SERIES_LABEL> году");
                    UltraChart1.DataSource = dtChart;
                }
            }


          }

       #endregion 

   #region экспорт

      #region экспорт в Excel

          private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
          {
              e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
          }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");
            
            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 0.7;
            ReportExcelExporter1.Export(headerLayout1,grid1CaptionElement.Text, sheet1, 3);
            ReportExcelExporter1.GridColumnWidthScale = 1.1;
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