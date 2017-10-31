using System;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Common.GridIndicatorRules;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
namespace Krista.FM.Server.Dashboards.reports.FO_0008_0001
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtTable = new DataTable();
        private DataTable dtDate = new DataTable();
        private DataTable dtNewTable;
        private int endYear = 2012;
        private string month;
        private int day;
        private int year;
        private int nCol = 1;
        private int count = 1;
        private string query;

        private DateTime currentDate;
        private DateTime date;
        #endregion

        #region Параметры запроса

        private CustomParam param;
        // выбранный период
        private CustomParam selectedPeriod;
        // 
        private CustomParam dates;
        private CustomParam currentYear;
        private CustomParam rubMultiplier;
        private CustomParam csrParam;
        #endregion

        private static MemberAttributesDigest csrDigest;

        private bool IsThsRubSelected
        {
            get { return RubMiltiplierButtonList.SelectedIndex == 0; }
        }

        private string RubMultiplierCaption
        {
            get { return IsThsRubSelected ? "тыс.руб." : "млн.руб."; }
        }

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            #region Настройка грида

            GridBrick.Height = CustomReportConst.minScreenHeight - 255;
            GridBrick.Width = CustomReportConst.minScreenWidth - 15;
            GridBrick.AutoSizeStyle = GridAutoSizeStyle.AutoHeight;
            GridBrick.Grid.InitializeLayout += new InitializeLayoutEventHandler(Grid_InitializeLayout);
            GridBrick.Grid.InitializeRow +=new InitializeRowEventHandler(Grid_InitializeRow);

            #endregion

            #region Инициализация параметров

            param = UserParams.CustomParam("param");
            selectedPeriod = UserParams.CustomParam("selected_period");
            dates = UserParams.CustomParam("dates");
            currentYear = UserParams.CustomParam("cur_year");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            csrParam = UserParams.CustomParam("csr_param");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            
            if (!Page.IsPostBack)
            {
                CustomCalendar1.Visible = true;
                // Получаем последнюю дату
                query = DataProvider.GetQueryText("FO_0008_0001_endYear");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);

                query = DataProvider.GetQueryText("FO_0008_0001_endDate");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                month = dtDate.Rows[0][3].ToString();
                day = Convert.ToInt32(dtDate.Rows[0][4]);
                date = new DateTime(endYear, CRHelper.MonthNum(month), day);
                // Инициализируем календарь
                CustomCalendar1.WebCalendar.SelectedDate = date;

                csrDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "FO_0008_0001_CSRDigest");
                ComboCSR.Title = "Направление финансирования";
                ComboCSR.Width = 450;
                ComboCSR.MultiSelect = false;
                ComboCSR.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(csrDigest.UniqueNames, csrDigest.MemberLevels));
                ComboCSR.SetСheckedState("", true);
                ComboCSR.Visible = false;
            }
            
            year = CustomCalendar1.WebCalendar.SelectedDate.Year;
            month = CRHelper.RusMonth(CustomCalendar1.WebCalendar.SelectedDate.Month);
            day = CustomCalendar1.WebCalendar.SelectedDate.Day;
            currentYear.Value = year.ToString();
            date = new DateTime(year, CRHelper.MonthNum(month), day);

            string dateList = string.Empty;

            for (int i=1; i<=day; i++)
            {
                dateList += string.Format("[Период__Период].[Период__Период].[Данные всех периодов].[{4}].[Полугодие {3}].[Квартал {2}].[{1}].[{0}],", i, month, CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(month)), CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(month)), year);
            }
            dates.Value = dateList.TrimEnd(',');

            string par = string.Empty;
            if (CheckBox1.Checked)
            {
                par += "Measures.[СубКОСГУ],";
                nCol++;
                count++;
            }
            if (CheckBox2.Checked)
            {
                par += "Measures.[Целевая статья],";
                nCol++;
                count++;
            }
            if (CheckBox3.Checked)
            {
                par += "Measures.[Генподрядчик],";
                nCol++;
                count++;
            }
            if (CheckBox4.Checked)
            {
                par += "Measures.[Получатель бюджетных средств],";
                nCol++;
                count++;
            }
            param.Value = par;

            if (RadioButtonList1.SelectedIndex == 0)
            {
              ComboCSR.Visible = true;
              csrParam.Value = csrDigest.GetMemberUniqueName(ComboCSR.SelectedValue);
            }
            else
            {
              ComboCSR.Visible = false;
            }

            rubMultiplier.Value = IsThsRubSelected ? "1000": "1000000";
            Page.Title = string.Format("Информация по финансированию краевой адресной инвестиционной программы в {0} году, по состоянию на {1:dd.MM.yyyy}, {2}",year, CustomCalendar1.WebCalendar.SelectedDate.Date, RubMultiplierCaption);
            Label1.Text = Page.Title;

            selectedPeriod.Value = CRHelper.PeriodMemberUName("[Период].[Период]", date, 5);

            GridDataBind();
        }

        #region Обработчики грида
        
        private void GridDataBind()
        {
            if (RadioButtonList1.SelectedIndex == 0)
            {
                string query = DataProvider.GetQueryText("FO_0008_0001_grid_general_view");
                dtTable = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей",
                                                                                 dtTable);
            }
            else if (RadioButtonList1.SelectedIndex == 1)
            {
                string query = DataProvider.GetQueryText("FO_0008_0001_grid_decoding_view");
                dtTable = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей",
                                                                                 dtTable);

                if (dtTable.Rows.Count> 0)
                {
                    for (int i = 1; i < dtTable.Rows.Count; i++)
                    {
                        dtTable.Rows[i][1] = i;
                    }
                    dtTable.Columns.RemoveAt(0);
                   
                }
            }
            else if (RadioButtonList1.SelectedIndex == 2)
            {
                string query = DataProvider.GetQueryText("FO_0008_0001_grid_helpMO_view");
                dtTable = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей",
                                                                                 dtTable);
                if (dtTable.Rows.Count>0)
                {
                    dtTable.Columns.RemoveAt(0);

                    for (int numRow=0; numRow<dtTable.Rows.Count; numRow++)
                    {

                        if (dtTable.Rows[numRow][dtTable.Columns.Count-1].ToString() =="3")
                        {
                            dtTable.Rows[numRow][1] = dtTable.Rows[numRow][dtTable.Columns.Count - 2];
                        }
                    }
                    // фильтрация по КЦСР
                    query = DataProvider.GetQueryText("FO_0008_0001_grid_helpMO_detail");
                    dtNewTable = new DataTable();
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей",
                                                                                 dtNewTable);

                    if (dtNewTable.Rows.Count> 0)
                    {
                        string name = string.Empty;
                        double value = 0;

                        for (int newRow = 0; newRow < dtTable.Rows.Count; newRow++) // пробегаем по строкам таблицы dtNewTable
                        {
                            name = dtNewTable.Rows[newRow][1].ToString();
                            for (int numRow = 0; numRow < dtTable.Rows.Count; numRow++) //строки таблицы dtTable
                            {
                                if (dtTable.Rows[numRow][1].ToString() == name && dtNewTable.Rows[newRow][2] != DBNull.Value && dtNewTable.Rows[newRow][2].ToString() != string.Empty)
                               {
                                   if (dtTable.Rows[numRow][count + 1] != DBNull.Value && dtTable.Rows[numRow][count + 1].ToString() != string.Empty)
                                   {
                                       value = Convert.ToDouble(dtTable.Rows[numRow][count + 1]);
                                   }
                                       dtTable.Rows[numRow][count + 1] = dtNewTable.Rows[newRow][2];

                                       // остаток от плана
                                       if (dtTable.Rows[numRow][count + 1] != DBNull.Value && dtTable.Rows[numRow][count + 1].ToString() != string.Empty && dtTable.Rows[numRow][count + 2] != DBNull.Value && dtTable.Rows[numRow][count + 2].ToString() != string.Empty)
                                       {
                                           dtTable.Rows[numRow][count + 3] = Convert.ToDouble(dtTable.Rows[numRow][count + 1]) - Convert.ToDouble(dtTable.Rows[numRow][count + 2]);
                                       }
                                       else if (dtTable.Rows[numRow][count + 1] != DBNull.Value && dtTable.Rows[numRow][count + 1].ToString() != string.Empty)
                                       {
                                           dtTable.Rows[numRow][count + 3] = Convert.ToDouble(dtTable.Rows[numRow][count + 1]);
                                       }
                                       else if (dtTable.Rows[numRow][count + 2] != DBNull.Value && dtTable.Rows[numRow][count + 2].ToString() != string.Empty)
                                       {
                                           dtTable.Rows[numRow][count + 3] =  Convert.ToDouble(dtTable.Rows[numRow][count + 2]) * (-1);
                                       }
                                       // сумма по району
                                   if (dtTable.Rows[numRow-1][1].ToString().Contains("район"))
                                       {
                                           if (dtTable.Rows[numRow - 1][count + 1] != DBNull.Value && dtTable.Rows[numRow - 1][count + 1].ToString() != string.Empty && dtNewTable.Rows[newRow][2]!= DBNull.Value && dtNewTable.Rows[newRow][2].ToString() != string.Empty)
                                           {
                                             dtTable.Rows[numRow - 1][count + 1] = Convert.ToDouble(dtNewTable.Rows[newRow][2]) + Convert.ToDouble(dtTable.Rows[numRow - 1][count + 1]) - value;
                                           }
                                           else if (dtNewTable.Rows[newRow][2] != DBNull.Value && dtNewTable.Rows[newRow][2].ToString() != string.Empty)
                                           {
                                             dtTable.Rows[numRow - 1][count + 1] = Convert.ToDouble(dtNewTable.Rows[newRow][2]);
                                           }
                                           else if (dtTable.Rows[numRow - 1][count + 1] != DBNull.Value && dtTable.Rows[numRow - 1][count + 1].ToString() != string.Empty )
                                           {
                                             dtTable.Rows[numRow - 1][count + 1] = Convert.ToDouble(dtTable.Rows[numRow - 1][count + 1]);
                                           }

                                           if (dtTable.Rows[numRow - 1][count + 1] != DBNull.Value && dtTable.Rows[numRow - 1][count + 1].ToString() != string.Empty && dtTable.Rows[numRow - 1][count + 2] != DBNull.Value && dtTable.Rows[numRow - 1][count + 2].ToString() != string.Empty)
                                           {
                                               dtTable.Rows[numRow - 1][count + 3] =
                                                   Convert.ToDouble(dtTable.Rows[numRow - 1][count + 1]) -
                                                   Convert.ToDouble(dtTable.Rows[numRow - 1][count + 2]);
                                           }
                                           else if (dtTable.Rows[numRow - 1][count + 1] != DBNull.Value && dtTable.Rows[numRow - 1][count + 1].ToString() != string.Empty)
                                           {
                                               dtTable.Rows[numRow - 1][count + 3] =Convert.ToDouble(dtTable.Rows[numRow - 1][count + 1]);
                                           }
                                           else if ( dtTable.Rows[numRow - 1][count + 2] != DBNull.Value && dtTable.Rows[numRow - 1][count + 2].ToString() != string.Empty)
                                           {
                                               dtTable.Rows[numRow - 1][count + 3] = Convert.ToDouble(dtTable.Rows[numRow - 1][count + 2]) * (-1);
                                           }

                                       }
                                   
                               }
                            }
                        }
                    }
                    
                    //
                    DataRow row;
                    // строка ВСЕГО 
                    row = dtTable.NewRow();
                    row[0] = DBNull.Value;
                    row[1] = "Итого";

                    if (count > 1)
                    {
                        for (int i = 2; i <= count; i++)
                        {
                            row[i] = 1;
                        }
                    }

                    for (int numCol = count+1; numCol < dtTable.Columns.Count-2; numCol++)
                    {
                      double sum = 0;
                      for (int numRow = 0; numRow < dtTable.Rows.Count; numRow++)
                       {
                           if (dtTable.Rows[numRow][dtTable.Columns.Count - 1].ToString() == "1")
                           {
                              if (dtTable.Rows[numRow][numCol] != DBNull.Value && dtTable.Rows[numRow][numCol].ToString() != string.Empty)
                               {
                                   sum += Convert.ToDouble(dtTable.Rows[numRow][numCol]);
                               }
                           }
                       }
                     
             
                     if (sum == 0)
                      {
                          row[numCol] = DBNull.Value;
                      }
                     else
                      {
                          row[numCol] = sum;
                      }

                     if (row[dtTable.Columns.Count - 5] != DBNull.Value && row[dtTable.Columns.Count - 5].ToString() != string.Empty && row[dtTable.Columns.Count - 4] != DBNull.Value && row[dtTable.Columns.Count - 4].ToString() != string.Empty)
                     {
                       row[dtTable.Columns.Count - 3] = Convert.ToDouble(row[dtTable.Columns.Count - 5]) - Convert.ToDouble(row[dtTable.Columns.Count - 4]);
                     }

                      row[dtTable.Columns.Count - 1] = "1";
                    }
                    
                    dtTable.Rows.Add(row);
                }
            }
            else if (RadioButtonList1.SelectedIndex == 3)
            {
                string query = DataProvider.GetQueryText("FO_0008_0001_grid_gasification_view1");
                dtTable = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей",
                                                                                 dtTable);
                if (dtTable.Rows.Count> 0)
                {
                    DataRow row;
                   for (int numRow = 0; numRow < dtTable.Rows.Count; numRow++)
                    {

                        dtTable.Rows[numRow][0] = dtTable.Rows[numRow][dtTable.Columns.Count - 3];
                    }
                    
                   query = DataProvider.GetQueryText("FO_0008_0001_grid_gasification_view2");
                   dtNewTable = new DataTable();
                   DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей",
                                                                               dtNewTable);
                   
                    if (dtNewTable.Rows.Count > 0)
                    {
                        for (int numRow = 0; numRow < dtNewTable.Rows.Count; numRow++)
                        {
                            row = dtTable.NewRow();
 
                            for (int numCol = 0; numCol < dtTable.Columns.Count; numCol++)
                            {
                                row[numCol] = dtNewTable.Rows[numRow][numCol];
                            }
                            dtTable.Rows.Add(row);
                        }

                        dtTable.AcceptChanges();

                        for (int numRow = 0; numRow < dtTable.Rows.Count; numRow++)
                        {
                            if (dtTable.Rows[numRow][dtTable.Columns.Count-3].ToString() == "ст.220")
                            {
                              dtTable.Rows[numRow][0] = "ст.226";
                            }
                            else
                            {
                              dtTable.Rows[numRow][0] = dtTable.Rows[numRow][dtTable.Columns.Count - 3];
                            }
                        }
                        // строка ВСЕГО 
                     /*  row = dtTable.NewRow();
                       
                        row[0] = "Всего";
                        for (int numCol = nCol; numCol < dtTable.Columns.Count-4; numCol++)
                        {
                          double sum = 0;
                          for (int numRow = 0; numRow < dtTable.Rows.Count; numRow++)
                           {
                               if (dtTable.Rows[numRow][dtTable.Columns.Count - 4].ToString() == "3" && dtTable.Rows[numRow][dtTable.Columns.Count - 1].ToString() == "1")
                               {
                                  if (dtTable.Rows[numRow][numCol] != DBNull.Value && dtTable.Rows[numRow][numCol].ToString() != string.Empty)
                                   {
                                      sum += Convert.ToDouble(dtTable.Rows[numRow][numCol]);
                                   }
                               }
                           }
                           if (sum ==0)
                           {
                               row[numCol] = DBNull.Value;
                           }
                           else
                           {
                              row[numCol] = sum;
                           }
                        }
                        row[dtTable.Columns.Count - 1] = "1";

                        dtTable.Rows.Add(row);*/
                        dtTable.AcceptChanges();
                    }

                    
                    // строка ВСЕГО 
                    row = dtTable.NewRow();


                    row[0] = "Всего";
                    
                    for (int numCol = nCol; numCol < dtTable.Columns.Count - 4; numCol++)
                    {
                        double sum = 0;
                        for (int numRow = 0; numRow < dtTable.Rows.Count; numRow++)
                        {
                            if (dtTable.Rows[numRow][dtTable.Columns.Count - 4].ToString() == "3" && dtTable.Rows[numRow][dtTable.Columns.Count - 1].ToString() == "1")
                            {
                                if (dtTable.Rows[numRow][numCol] != DBNull.Value && dtTable.Rows[numRow][numCol].ToString() != string.Empty)
                                {
                                    sum += Convert.ToDouble(dtTable.Rows[numRow][numCol]);
                                }
                            }
                        }
                        if (sum == 0)
                        {
                            row[numCol] = DBNull.Value;
                        }
                        else
                        {
                            row[numCol] = sum;
                        }
                    }
                    row[dtTable.Columns.Count - 1] = "1";

                    if (row[count + 2] != DBNull.Value && row[count + 2].ToString() != string.Empty && row[count] != DBNull.Value && row[count].ToString() != string.Empty && Convert.ToDouble(row[count])!=0)
                    {
                        row[count + 4] = Convert.ToDouble(row[count + 2]) / Convert.ToDouble(row[count])*100;
                    }

                    dtTable.Rows.Add(row);
                    dtTable.AcceptChanges();
                }
            }

             FontRowLevelRule levelRule = new FontRowLevelRule(dtTable.Columns.Count - 1);
             levelRule.AddFontLevel("1", new Font(GridBrick.Grid.DisplayLayout.RowStyleDefault.Font.Name, 10, FontStyle.Bold));
             if (RadioButtonList1.SelectedIndex!=3)
             {
                 levelRule.AddFontLevel("2", new Font(GridBrick.Grid.DisplayLayout.RowStyleDefault.Font.Name, 10, FontStyle.Bold));
             }
             GridBrick.AddIndicatorRule(levelRule);

             
             GridBrick.DataTable = dtTable;
             
        }

        protected void Grid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;


            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count -1].Hidden = true;

            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;

            int columnCount = e.Layout.Bands[0].Columns.Count;

            string columnFormat;
            for (int i = 1; i < columnCount ; i++)
            {
                if (e.Layout.Bands[0].Columns[i].Header.Caption.ToLower().Contains("субкосгу") || e.Layout.Bands[0].Columns[i].Header.Caption.ToLower().Contains("целевая статья"))
                {
                    columnFormat = "000 00 00";
                }
               else
                {
                    columnFormat = "N2";
                }
                
               if (e.Layout.Bands[0].Columns[i].Header.Caption.ToLower().Contains("генподрядчик") || e.Layout.Bands[0].Columns[i].Header.Caption.ToLower().Contains("получатель бюджетных средств"))
                {
                   e.Layout.Bands[0].Columns[i].CellStyle.Wrap = true;
                   e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(140);
                }
                
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], columnFormat);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(120);
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
   
            }
            
            GridHeaderLayout headerLayout = GridBrick.GridHeaderLayout;

            if (RadioButtonList1.SelectedIndex == 1 || RadioButtonList1.SelectedIndex == 2)
             {
                headerLayout.AddCell("№");
             }
            headerLayout.AddCell("Наименование программ, мероприятий и объектов");
            if (CheckBox1.Checked)
            {
              headerLayout.AddCell("СубКОСГУ");
            }
            if (CheckBox2.Checked)
            {
                headerLayout.AddCell("Целевая статья");
            }
            if (CheckBox3.Checked)
            {
                headerLayout.AddCell("Генподрядчик");
            }
            if (CheckBox4.Checked)
            {
                headerLayout.AddCell("Получатель бюджетных средств");
            }
            if (RadioButtonList1.SelectedIndex == 0)
            {
               headerLayout.AddCell("План краевого бюджета");
               headerLayout.AddCell(string.Format("Финансирование за {0}", month));
               headerLayout.AddCell(string.Format("Финансирование на {0:dd.MM.yyyy}", date));
               headerLayout.AddCell("Остаток от плана");
               headerLayout.AddCell("% исполнения года");
               headerLayout.AddCell("Софинансирование план");
               headerLayout.AddCell("Софинансирование факт");
               headerLayout.AddCell("% исполнения софинансирования");
            }
            if (RadioButtonList1.SelectedIndex == 1)
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(30);
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(300);
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                headerLayout.AddCell(string.Format("Финансирование за {0}", month));
            }
            if (RadioButtonList1.SelectedIndex == 2)
            {
                e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(30);
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Center;
                e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(300);
                e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
                e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
                e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = true;
                headerLayout.AddCell("План краевого бюджета");
                headerLayout.AddCell(string.Format("Финансирование на {0:dd.MM.yyyy}", date));
                headerLayout.AddCell("Остаток от плана");
            }
            if (RadioButtonList1.SelectedIndex == 3)
            {
                e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = true;
                e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 3].Hidden = true;
                e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 4].Hidden = true;
                headerLayout.AddCell("План краевого бюджета");
                headerLayout.AddCell(string.Format("Финансирование за {0}", month));
                headerLayout.AddCell(string.Format("Финансирование на {0:dd.MM.yyyy}", date));
                headerLayout.AddCell("Остаток от плана");
                headerLayout.AddCell("% исполнения года");
                headerLayout.AddCell("Софинансирование план");
                headerLayout.AddCell("Софинансирование факт");
                headerLayout.AddCell("% исполнения софинансирования");
            }
            GridBrick.GridHeaderLayout.ApplyHeaderInfo();
        }
        
        protected  void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            string level = string.Empty;
            int numCol = (RadioButtonList1.SelectedIndex == 2) ? 1 : 0;
            if (RadioButtonList1.SelectedIndex != 1)
            {
                if (e.Row.Cells[e.Row.Cells.Count - 1].Value != null &&
                    e.Row.Cells[e.Row.Cells.Count - 1].ToString() != string.Empty)
                {
                    level = e.Row.Cells[e.Row.Cells.Count - 1].Value.ToString();

                    switch (level)
                    {

                        case "3":
                            {
                                e.Row.Cells[numCol].Style.Padding.Left = 10;
                                break;
                            }
                        case "4":
                            {
                                e.Row.Cells[numCol].Style.Padding.Left = 15;
                                break;
                            }
                        case "5":
                            {
                                e.Row.Cells[numCol].Style.Padding.Left = 20;
                                break;
                            }
                        case "6":
                            {
                                e.Row.Cells[numCol].Style.Padding.Left = 25;
                                break;
                            }
                    }
                }
            }

            if (RadioButtonList1.SelectedIndex == 2)
            {
              
              if (count > 1)
               {
                  for (int i = 2; i <= count  ; i++)
                  {
                   // CRHelper.SaveToErrorLog(e.Row.Cells[i].Value.ToString());
                    if (e.Row.Cells[i].Value.ToString() == "1")
                    {
                        e.Row.Cells[i].Value = "";
                    }
                  }
               }
            }

            if (RadioButtonList1.SelectedIndex == 3)
            {
                double value;
                if (e.Row.Cells[count+4]!= null && e.Row.Cells[count+4].ToString() != string.Empty)
                {
                    value = Convert.ToDouble(e.Row.Cells[count + 4].Value);
                    e.Row.Cells[count + 4].Value = string.Format("{0:N2}", value);
                } 
            }


        }

        #endregion
        
        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = Label1.Text;
            ReportExcelExporter1.SheetColumnCount = 15;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            Workbook workbook = new Workbook();

            Worksheet sheet1 = workbook.Worksheets.Add("Таблица");
            ReportExcelExporter1.Export(GridBrick.GridHeaderLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в PDF

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = Label1.Text;
            ReportPDFExporter1.HeaderCellHeight = 50;

            Report report = new Report();

            ISection section1 = report.AddSection();
            ReportPDFExporter1.Export(GridBrick.GridHeaderLayout, section1);
        }

        #endregion
    }
}