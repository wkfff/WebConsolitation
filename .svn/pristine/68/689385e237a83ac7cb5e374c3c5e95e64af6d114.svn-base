using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Krista.FM.Server.Dashboards.Core.MemberDigests;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0027
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2010;
        private int endYear = 2011;
        private int currentYear;
        private string month;

        private CustomParam LastLastYear;
        private CustomParam Quarts;
        private CustomParam lastQuarts;
        private CustomParam subject;

        private GridHeaderLayout headerLayout;

        private MemberAttributesDigest budgetDigest;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 35);
            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.3);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);


            if (LastLastYear == null)
            {
                LastLastYear = UserParams.CustomParam("last_year");
            }

            if (Quarts == null)
            {
                Quarts = UserParams.CustomParam("quarts");
            }

            if (lastQuarts == null)
            {
                lastQuarts = UserParams.CustomParam("last_quarts");
            }
           
            subject = UserParams.CustomParam("subject_area");
            
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            WebAsyncPanel.AddRefreshTarget(UltraWebGrid);
            WebAsyncPanel.AddLinkedRequestTrigger(thousands.ClientID);
            WebAsyncPanel.AddLinkedRequestTrigger(millions.ClientID);

            month = String.Empty;
            base.Page_PreLoad(sender, e);

            dtDate = new DataTable();
            string query = DataProvider.GetQueryText("FO_0002_0027_date");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
            endYear = Convert.ToInt32(dtDate.Rows[0][0]);
            month = dtDate.Rows[0][3].ToString();

            budgetDigest = MemberDigestHelper.Instance.LocalBudgetDigest;

            if (!Page.IsPostBack)
            {
                thousands.Attributes.Add("onclick", string.Format("uncheck('{0}')", millions.ClientID));
                millions.Attributes.Add("onclick", string.Format("uncheck('{0}')", thousands.ClientID));

                UserParams.Filter.Value = "000";

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 150;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month,true);

                DataTable dtTerritory = new DataTable();
                query = DataProvider.GetQueryText("FO_0002_0027_territory");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "dummy", dtTerritory);
                
                Dictionary<string, int> territory = new Dictionary<string, int>();
                foreach (DataRow row in dtTerritory.Rows)
                {
                    territory.Add(row[0].ToString(), 0);
                }

                ComboTerritory.Title = "Территории";
                ComboTerritory.Width = 300;
                ComboTerritory.MultiSelect = false;
                ComboTerritory.ParentSelect = false;
                ComboTerritory.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(budgetDigest.UniqueNames, budgetDigest.MemberLevels));
                ComboTerritory.RemoveTreeNodeByName("Консолидированный бюджет субъекта");
                ComboTerritory.RemoveTreeNodeByName("Бюджет субъекта");
                ComboTerritory.RemoveTreeNodeByName("Местные бюджеты");
                ComboTerritory.SetСheckedState("Городские округа ", true);
                ComboTerritory.SetСheckedState("Муниципальные районы ", true);
            }

            currentYear = Convert.ToInt32(ComboYear.SelectedValue);

            DateTime reportDate = new DateTime(currentYear, 1, 1);

            UserParams.PeriodYear.Value = currentYear.ToString();
            UserParams.PeriodLastYear.Value = (currentYear - 1).ToString();
            LastLastYear.Value = (currentYear - 2).ToString();  // позапрошлый год

            subject.Value = budgetDigest.GetMemberUniqueName(ComboTerritory.SelectedValue);
            
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}",CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(ComboMonth.SelectedValue))) ;
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}",CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(ComboMonth.SelectedValue)));
            Quarts.Value = string.Empty;
            if ((ComboMonth.SelectedIndex + 1) < 4)
            {
                Quarts.Value = string.Format("[Период].[Период].[Данные всех периодов].[{0}].[{1}].[{2}].[{3}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, UserParams.PeriodMonth.Value);
                lastQuarts.Value = string.Format("[Период].[Период].[Данные всех периодов].[{0}].[{1}].[{2}].[{3}]", UserParams.PeriodLastYear.Value , UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, UserParams.PeriodMonth.Value);
            }

            if ((ComboMonth.SelectedIndex + 1) > 3 && (ComboMonth.SelectedIndex + 1) < 7)
            {
                Quarts.Value = string.Format("[Период].[Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 1].[Март], [Период].[Период].[Данные всех периодов].[{0}].[{1}].[{2}].[{3}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, UserParams.PeriodMonth.Value);
                lastQuarts.Value = string.Format("[Период].[Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 1].[Март], [Период].[Период].[Данные всех периодов].[{0}].[{1}].[{2}].[{3}]", UserParams.PeriodLastYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, UserParams.PeriodMonth.Value); 
            }

            if ((ComboMonth.SelectedIndex + 1) >6 && (ComboMonth.SelectedIndex + 1) < 10)
            {
                Quarts.Value = string.Format("[Период].[Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 1].[Март], [Период].[Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 2].[Июнь],  [Период].[Период].[Данные всех периодов].[{0}].[{1}].[{2}].[{3}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, UserParams.PeriodMonth.Value);
                lastQuarts.Value = string.Format("[Период].[Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 1].[Март], [Период].[Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 2].[Июнь],  [Период].[Период].[Данные всех периодов].[{0}].[{1}].[{2}].[{3}]", UserParams.PeriodLastYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, UserParams.PeriodMonth.Value);
            }

            if ((ComboMonth.SelectedIndex + 1) >9 && (ComboMonth.SelectedIndex + 1) < 13)
            {
                Quarts.Value = string.Format("[Период].[Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 1].[Март], [Период].[Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 2].[Июнь], [Период].[Период].[Данные всех периодов].[{0}].[Полугодие 2].[Квартал 3].[Сентябрь],  [Период].[Период].[Данные всех периодов].[{0}].[{1}].[{2}].[{3}]", UserParams.PeriodYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, UserParams.PeriodMonth.Value);
                lastQuarts.Value = string.Format("[Период].[Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 1].[Март], [Период].[Период].[Данные всех периодов].[{0}].[Полугодие 1].[Квартал 2].[Июнь], [Период].[Период].[Данные всех периодов].[{0}].[Полугодие 2].[Квартал 3].[Сентябрь],  [Период].[Период].[Данные всех периодов].[{0}].[{1}].[{2}].[{3}]", UserParams.PeriodLastYear.Value, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, UserParams.PeriodMonth.Value);
            }

            if (WebAsyncPanel.IsAsyncPostBack)
            {
                bool thousandsWasChecked = UserParams.Filter.Value == "000";

                if (thousandsWasChecked)
                {
                    thousands.Checked = false;
                    millions.Checked = true;
                    UserParams.Filter.Value = "000000";
                }
                else
                {
                    thousands.Checked = true;
                    millions.Checked = false;
                    UserParams.Filter.Value = "000";
                }
            }
            PageTitle.Text = "Финансовый паспорт";
            Page.Title = PageTitle.Text;
            PageSubTitle.Text = String.Format(
                   "{0} данные консолидированного бюджета за {1} год и {2} {3} {4} года", ComboTerritory.SelectedValue,
                   UserParams.PeriodLastYear.Value, ComboMonth.SelectedIndex +1,
                   CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1), UserParams.PeriodYear.Value);

          /*  if (ComboYear.SelectedValue == endYear.ToString())
            {
                PageSubTitle.Text = String.Format(
                    "{0} данные консолидированного бюджета за {1} год и {2} {3} {4} года", UserParams.Subject.Value,
                    UserParams.PeriodLastYear.Value, CRHelper.MonthNum(month),
                    CRHelper.RusManyMonthGenitive(CRHelper.MonthNum(month)), UserParams.PeriodYear.Value);
            }
            else
            {
                PageSubTitle.Text = String.Format(
                   "{0} данные консолидированного бюджета за {1} год и 12 месяцев {4} года", UserParams.Subject.Value,
                   UserParams.PeriodLastYear.Value, CRHelper.MonthNum(month),
                   CRHelper.RusManyMonthGenitive(CRHelper.MonthNum(month)), UserParams.PeriodYear.Value);
            }
            */
            UserParams.PeriodMonth.Value = CRHelper.PeriodMemberUName("", reportDate, 4);

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0027_grid_Rests");
            dtGrid = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGrid);


            /*for (int i = 1; i < dtGrid.Columns.Count; i++ )
            {
                DataColumn col = dtGrid.Columns[i];
                string SeriesFieldName = col.ColumnName.Replace("_", "");
                while (dtGrid.Columns.Contains(SeriesFieldName))
                {
                    SeriesFieldName += ' ';
                }
                col.ColumnName = SeriesFieldName;
            }

           dtGrid.AcceptChanges();*/

            query = DataProvider.GetQueryText("FO_0002_0027_grid_incomes");
            DataTable dtGridIncomes = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGridIncomes);

            query = DataProvider.GetQueryText("FO_0002_0027_grid_%rosta_CurYear_incomes_godOtch");
            DataTable dtGridRostCur = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGridRostCur);
            dtGridRostCur.PrimaryKey = new DataColumn[] { dtGridRostCur.Columns[0] };

            query = DataProvider.GetQueryText("FO_0002_0027_grid_%rosta_LastYear_incomes_godOtch");
            DataTable dtGridRostLast = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGridRostLast);
            dtGridRostLast.PrimaryKey = new DataColumn[] { dtGridRostLast.Columns[0] };

            query = DataProvider.GetQueryText("FO_0002_0027_grid_incomes_godOtch_Last_%Rosta");
            DataTable dtGridRostLastgodOtch = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGridRostLastgodOtch);
            dtGridRostLastgodOtch.PrimaryKey = new DataColumn[] { dtGridRostLastgodOtch.Columns[0] };

            query = DataProvider.GetQueryText("FO_0002_0027_grid_incomes_godOtch_Cur_%Rosta");
            DataTable dtGridRostCurgodOtch = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGridRostCurgodOtch);
            dtGridRostCurgodOtch.PrimaryKey = new DataColumn[] { dtGridRostCurgodOtch.Columns[0] };

            for (int i = 0; i < dtGridIncomes.Rows.Count; i++)
            {
                string[] rowName = new string[] { dtGridIncomes.Rows[i][0].ToString() };
                DataRow row1 = dtGridRostCur.Rows.Find(rowName);
                DataRow row2 = dtGridRostLast.Rows.Find(rowName);
                DataRow row3 = dtGridRostLastgodOtch.Rows.Find(rowName);
                DataRow row4 = dtGridRostCurgodOtch.Rows.Find(rowName);

                if (row1 != null && row2 != null)
                { 
                    for (int indCol=1; indCol<dtGridRostCur.Columns.Count; indCol+=3)
                    {
                        if (indCol != 10)
                        {
                            if (row1[indCol] != DBNull.Value && row1[indCol].ToString() != string.Empty && row2[indCol] != DBNull.Value && row2[indCol].ToString() != string.Empty)
                            {
                                dtGridIncomes.Rows[i + 1][indCol] = Convert.ToDouble(row1[indCol]) / Convert.ToDouble(row2[indCol])*100;

                            }
                            if (row1[indCol + 1] != DBNull.Value && row1[indCol + 1].ToString() != string.Empty && row2[indCol + 1] != DBNull.Value && row2[indCol + 1].ToString() != string.Empty)
                            {
                                dtGridIncomes.Rows[i + 1][indCol + 1] = Convert.ToDouble(row1[indCol + 1])/ Convert.ToDouble(row2[indCol + 1])*100;
                            }
                       }
                    }
                }
                if (row3 !=null && row4 != null)
                {

                    if (row3[1] != DBNull.Value || row3[2] != DBNull.Value || row4[1] != DBNull.Value || row4[2] != DBNull.Value)
                            {
                                dtGridIncomes.Rows[i + 1][10] = Convert.ToDouble(row3[1])/
                                                                    Convert.ToDouble(row4[1])*100;
                                dtGridIncomes.Rows[i + 1][11] = Convert.ToDouble(row3[2])/
                                                                        Convert.ToDouble(row4[2])*100;
                            }
                   
                }


            }
            query = DataProvider.GetQueryText("FO_0002_0027_grid_incomes_godOtch");
            DataTable dtGridIncomesgodOtch = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGridIncomesgodOtch);
            dtGridIncomesgodOtch.PrimaryKey = new DataColumn[] { dtGridIncomesgodOtch.Columns[0] };

            for (int i = 0; i < dtGridIncomes.Rows.Count; i++)
            {
                string[] rowName = new string[] { dtGridIncomes.Rows[i][0].ToString() };
                DataRow row = dtGridIncomesgodOtch.Rows.Find(rowName);
                if (row != null)
                {
                    dtGridIncomes.Rows[i][10] = row[1];
                    dtGridIncomes.Rows[i][11] = row[2];
                    dtGridIncomes.Rows[i][12] = row[3];
                }
            }

            foreach (DataRow row in dtGridIncomes.Rows)
            {
                row[0] = row[0].ToString().Replace('_', ' ');
                dtGrid.ImportRow(row);
            }

            query = DataProvider.GetQueryText("FO_0002_0027_grid_outcomes");
            DataTable dtGridOutcomes = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGridOutcomes);
         /*   for (int i = 1; i < dtGridOutcomes.Columns.Count; i++)
            {
                DataColumn col = dtGridOutcomes.Columns[i];
                string SeriesFieldName = col.ColumnName.Replace("_", "");
                while (dtGridOutcomes.Columns.Contains(SeriesFieldName))
                {
                    SeriesFieldName += ' ';
                }
                col.ColumnName = SeriesFieldName;
            }

            dtGridOutcomes.AcceptChanges();
            */
            query = DataProvider.GetQueryText("FO_0002_0027_grid_outcomes_godOtch");
            DataTable dtGridOutcomesgodOtch = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGridOutcomesgodOtch);
            dtGridOutcomesgodOtch.PrimaryKey = new DataColumn[] { dtGridOutcomesgodOtch.Columns[0] };

            for (int i = 0; i < dtGridOutcomes.Rows.Count; i++)
            {
                string[] rowName = new string[] { dtGridOutcomes.Rows[i][0].ToString() };
                DataRow row = dtGridOutcomesgodOtch.Rows.Find(rowName);
                if (row != null)
                {
                    dtGridOutcomes.Rows[i][10] = row[1];
                    dtGridOutcomes.Rows[i][11] = row[2];
                    dtGridOutcomes.Rows[i][12] = row[3];
                }
            }

            foreach (DataRow row in dtGridOutcomes.Rows)
            {
                dtGrid.ImportRow(row);
            }

            query = DataProvider.GetQueryText("FO_0002_0027_grid_finsources");
            DataTable dtGridfinSources = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGridfinSources);

            query = DataProvider.GetQueryText("FO_0002_0027_grid_finsources_godOtch");
            DataTable dtGridfinSourcesgodOtch = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGridfinSourcesgodOtch);
            dtGridfinSourcesgodOtch.PrimaryKey = new DataColumn[] { dtGridfinSourcesgodOtch.Columns[0] };

            for (int i = 0; i < dtGridfinSources.Rows.Count; i++)
            {
                string[] rowName = new string[] { dtGridfinSources.Rows[i][0].ToString() };
                DataRow row = dtGridfinSourcesgodOtch.Rows.Find(rowName);
                if (row != null)
                {
                    dtGridfinSources.Rows[i][10] = row[1];
                    dtGridfinSources.Rows[i][11] = row[2];
                    dtGridfinSources.Rows[i][12] = row[3];
                }
            }

            foreach (DataRow row in dtGridfinSources.Rows)
            {
                dtGrid.ImportRow(row);
            }

            query = DataProvider.GetQueryText("FO_0002_0027_creditDebts");
            DataTable dtGridcreditDebts = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGridcreditDebts);

            foreach (DataRow row in dtGridcreditDebts.Rows)
            {
                dtGrid.ImportRow(row);
            }

            query = DataProvider.GetQueryText("FO_0002_0027_grid_munDebts");
            DataTable dtGridmunDebts = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование МО", dtGridmunDebts);

            foreach (DataRow row in dtGridmunDebts.Rows) 
            {
                dtGrid.ImportRow(row);
            }


            // считаем % исполнения начиная п.2 по п.4 
            for (int indRow = 3; indRow < 42; indRow++)
            {
                for (int indCol = 3; indCol < dtGrid.Columns.Count; indCol += 3)
                {
                    if (dtGrid.Rows[indRow][indCol - 1].ToString() != string.Empty || dtGrid.Rows[indRow][indCol - 1] != DBNull.Value || dtGrid.Rows[indRow][indCol - 2].ToString() != string.Empty || dtGrid.Rows[indRow][indCol - 2] != DBNull.Value)
                    {
                        if (Convert.ToDouble(dtGrid.Rows[indRow][indCol - 2]) != 0.0)
                        {
                            dtGrid.Rows[indRow][indCol] = Convert.ToDouble(dtGrid.Rows[indRow][indCol - 1])/
                                                          Convert.ToDouble(dtGrid.Rows[indRow][indCol - 2]);
                        }
                    }


                }
            }

            dtGrid.Columns.Add(new DataColumn("Код", typeof(string)));

            foreach (DataRow row in dtGrid.Rows)
            {
                string[] rowName = row[0].ToString().Split(';');
                if (rowName.Length == 2)
                {
                    row[0] = rowName[1];
                    row["Код"] = rowName[0];
                }
            }

            

            if (dtGrid.Rows.Count > 0)
            {
                UltraWebGrid.DataSource = dtGrid;
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;

            for (int i = 1; i < e.Layout.Bands[0].Columns.Count; i = i + 1)
            {
                int widthColumn = 110;
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(widthColumn);
            }

            UltraGridColumn col = e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1];
            e.Layout.Bands[0].Columns.RemoveAt(e.Layout.Bands[0].Columns.Count - 1);
            e.Layout.Bands[0].Columns.Insert(0, col);

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(40);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Right;
            e.Layout.Bands[0].Columns[0].CellStyle.Padding.Right = 10;

            e.Layout.Bands[0].Columns[1].Width = CRHelper.GetColumnWidth(330);
            e.Layout.Bands[0].Columns[1].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;


            headerLayout.AddCell("Код");
            headerLayout.AddCell("Показатель");

            GridHeaderCell headerCell = headerLayout.AddCell((currentYear - 1) + " год");
            GridHeaderCell headerChildCell = headerCell.AddCell("Квартал 1");
            AddChildLevelCells(headerChildCell);
            headerChildCell = headerCell.AddCell("Квартал 2");
            AddChildLevelCells(headerChildCell);
            headerChildCell = headerCell.AddCell("Квартал 3");
            AddChildLevelCells(headerChildCell);
            headerChildCell = headerCell.AddCell("Исполнено за год");
            AddChildLevelCells(headerChildCell);

            if ((ComboMonth.SelectedIndex + 1) < 4)
            {
                headerCell = headerLayout.AddCell((currentYear) + " год");
                headerChildCell = headerCell.AddCell(string.Format("Квартал 1 (за {0} {1})",CRHelper.MonthNum(ComboMonth.SelectedValue),CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1)));
                AddChildLevelCells(headerChildCell);
            }
            if ((ComboMonth.SelectedIndex + 1) >3 && (ComboMonth.SelectedIndex + 1) < 7)
            {
                headerCell = headerLayout.AddCell((currentYear) + " год");
                headerChildCell = headerCell.AddCell("Квартал 1");
                AddChildLevelCells(headerChildCell);
                headerChildCell = headerCell.AddCell(string.Format("Квартал 2 (за {0} {1})", CRHelper.MonthNum(ComboMonth.SelectedValue), CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1)));
                AddChildLevelCells(headerChildCell);
            }
            if ((ComboMonth.SelectedIndex + 1) > 6 && (ComboMonth.SelectedIndex + 1) < 10)
            {
                headerCell = headerLayout.AddCell((currentYear) + " год");
                headerChildCell = headerCell.AddCell("Квартал 1");
                AddChildLevelCells(headerChildCell);
                headerChildCell = headerCell.AddCell("Квартал 2");
                AddChildLevelCells(headerChildCell);
                headerChildCell = headerCell.AddCell(string.Format("Квартал 3 (за {0} {1})", CRHelper.MonthNum(ComboMonth.SelectedValue), CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1)));
                AddChildLevelCells(headerChildCell);
            }

            if ((ComboMonth.SelectedIndex + 1) > 9 && (ComboMonth.SelectedIndex + 1) < 13)
            {
                headerCell = headerLayout.AddCell((currentYear) + " год");
                headerChildCell = headerCell.AddCell("Квартал 1");
                AddChildLevelCells(headerChildCell);
                headerChildCell = headerCell.AddCell("Квартал 2");
                AddChildLevelCells(headerChildCell);
                headerChildCell = headerCell.AddCell("Квартал 3"); 
                AddChildLevelCells(headerChildCell);
                
                if (ComboYear.SelectedValue == endYear.ToString())
                {
                    headerChildCell =
                        headerCell.AddCell(string.Format("Квартал 4 (за {0} {1})",
                                                         CRHelper.MonthNum(ComboMonth.SelectedValue),
                                                         CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1)));
                    AddChildLevelCells(headerChildCell);
                }
                else
                {
                    headerChildCell =
                        headerCell.AddCell(string.Format("Квартал 4 (за {0} {1})",
                                                         CRHelper.MonthNum(ComboMonth.SelectedValue),
                                                         CRHelper.RusManyMonthGenitive(ComboMonth.SelectedIndex + 1)));
                    AddChildLevelCells(headerChildCell);
                }
            }
         
            headerLayout.ApplyHeaderInfo();

            for (int i = 2; i < e.Layout.Grid.Columns.Count; i++)
            {
                e.Layout.Bands[0].Columns[i].CellStyle.HorizontalAlign = HorizontalAlign.Right;
                e.Layout.Bands[0].Columns[i].CellStyle.Padding.Right = 10;
            }
        }

        private static void AddChildLevelCells(GridHeaderCell headerChildCell)
        {
            headerChildCell.AddCell("План");
            headerChildCell.AddCell("Факт");
            headerChildCell.AddCell("% исполнения");
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            bool bold = (e.Row.Cells[0].Value != null && !e.Row.Cells[0].Value.ToString().Contains("."));
            e.Row.Cells[0].Style.Font.Bold = bold;
            e.Row.Cells[1].Style.Font.Bold = bold;

            bool arrow = e.Row.Cells[1].Value != null &&
                         e.Row.Cells[1].Value.ToString().Contains("% роста");

            bool corner = e.Row.Cells[0].Value != null &&  e.Row.Cells[0].Value.ToString() == ("8.2");
            

            for (int i = 2; i < e.Row.Cells.Count - 1; i += 3)
            {
                if (e.Row.Cells[i].Value != null)
                {
                    if (e.Row.Cells[0] != null && e.Row.Cells[0].Value != null && 
                            (e.Row.Cells[0].Value.ToString().Contains("2.1.1.2") || 
                            e.Row.Cells[0].Value.ToString() == ("8") || 
                            e.Row.Cells[0].Value.ToString() == ("9")))
                    {
                        e.Row.Cells[i].Value = String.Empty;
                    }
                    double value;
                    if (corner)
                    {
                        SetConditionCorner(e, i);
                    }
                    if (arrow)
                    {
                        SetConditionArrow(e, i);
                        
                        if (Double.TryParse(e.Row.Cells[i].Value.ToString(), out value))
                        {
                            e.Row.Cells[i].Value = String.Format("{0:N2}", value);
                            e.Row.Cells[i].Style.Font.Bold = bold;
                        }
                    }
                    else if (Double.TryParse(e.Row.Cells[i].Value.ToString(), out value))
                    {
                        e.Row.Cells[i].Value = String.Format("{0:N1}", value);
                        e.Row.Cells[i].Style.Font.Bold = bold;
                    }
                }
                if (e.Row.Cells[i + 1].Value != null)
                {
                    double value;
                    if (corner)
                    {
                        SetConditionCorner(e, i + 1);
                    }
                    if (arrow || e.Row.Cells[0].Value.ToString().Contains("2.1.1.2"))
                    {
                        SetConditionArrow(e, i + 1);
                        if (Double.TryParse(e.Row.Cells[i + 1].Value.ToString(), out value))
                        {
                            e.Row.Cells[i + 1].Value = String.Format("{0:N2}", value);
                            e.Row.Cells[i + 1].Style.Font.Bold = bold;
                        }
                    }
                    else if (Double.TryParse(e.Row.Cells[i + 1].Value.ToString(), out value))
                    {

                        e.Row.Cells[i + 1].Value = String.Format("{0:N1}", value);
                        e.Row.Cells[i + 1].Style.Font.Bold = bold;
                    }
                }
                if (e.Row.Cells[i + 2].Value != null)
                {
                    if (arrow)
                    {
                        e.Row.Cells[i + 2].Value = String.Empty;
                    }
                    else if (e.Row.Cells[0].Value != null && 
                            (e.Row.Cells[0].Value.ToString().Contains("2.1.1.2") ||
                            e.Row.Cells[0].Value.ToString() == ("4") ||
                            e.Row.Cells[0].Value.ToString() == ("8") ||
                            e.Row.Cells[0].Value.ToString() == ("8.2") ||
                            e.Row.Cells[0].Value.ToString() == ("9")))
                    {
                        e.Row.Cells[i + 2].Value = String.Empty;
                    }
                    else
                    {
                        SetConditionBall(e, i + 2);
                        // Форматируем процент исполнения
                        double value;
                        if (Double.TryParse(e.Row.Cells[i + 2].Value.ToString(), out value))
                        {
                            e.Row.Cells[i + 2].Value = String.Format("{0:N2}", value * 100);
                            e.Row.Cells[i + 2].Style.Font.Bold = bold;
                        }
                    }
                }
            }
        }

        public static void SetConditionArrow(RowEventArgs e, int index)
        {
            if (index < 14 || e.Row.Cells[0].ToString().Split('.')[0] == "4" || e.Row.Cells[0].ToString().Split('.')[0] == "5" || e.Row.Cells[0].ToString() == "2.1.1.2")
            {
                return;
            }
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value;
                if (Double.TryParse(e.Row.Cells[index].Value.ToString(), out value))
                {
                    string title = String.Empty;
                    string img;
                    if (value > 100)
                    {
                        img = "~/images/arrowGreenUpBB.png";
                        title = "Рост к прошлому году";
                    }
                    else
                    {
                        img = "~/images/arrowRedDownBB.png";
                        title = "Снижение к прошлому году";
                    }
                    e.Row.Cells[index].Style.BackgroundImage = img;
                    e.Row.Cells[index].Title = title;
                    e.Row.Cells[index].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: 10px center; padding-left: 0px";
                }
            }
        }

        public static void SetConditionCorner(RowEventArgs e, int index)
        {
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null)
            {
                double value;
                if (Double.TryParse(e.Row.Cells[index].Value.ToString(), out value))
                {
                    string img;
                    string title;
                    if (value > 0)
                    {
                        img = "~/images/cornerRed.gif";
                        title = "Превышение установленного норматива";
                    }
                    else
                    {
                        img = "~/images/cornerGreen.gif";
                        title = "Соблюдение установленного норматива";
                    }
                    e.Row.Cells[index].Style.BackgroundImage = img;
                    e.Row.Cells[index].Title = title;
                    e.Row.Cells[index].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: right top; padding-left: 2px";
                }
            }
        }

        public static void SetConditionBall(RowEventArgs e, int index)
        {
            if (index < 14 || e.Row.Cells[0].ToString().Split('.')[0] == "4" || e.Row.Cells[0].ToString().Split('.')[0] == "5" || e.Row.Cells[1].ToString().Contains("% роста") || e.Row.Cells[0].ToString() == "2.1.1.2")
            {
                return;
            }
            if (e.Row.Cells[index] != null &&
                e.Row.Cells[index].Value != null &&
                e.Row.Cells[index].Value.ToString().Length > 0)
            {
                double value;
                if (Double.TryParse(e.Row.Cells[index].Value.ToString(), out value))
                {
                    string title = String.Empty;
                    string img;
                    if (value > GetAssession(index))
                    {
                        img = "~/images/ballGreenBB.png";
                        title = "Соблюдается условие равномерности";
                    }
                    else
                    {
                        img = "~/images/ballRedBB.png";
                        title = "Не соблюдается условие равномерности";
                    }
                    e.Row.Cells[index].Style.BackgroundImage = img;
                    e.Row.Cells[index].Title = title;
                    e.Row.Cells[index].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: 10px center; padding-left: 0px";
                }
            }
        }

        private static double GetAssession(int index)
        {
            switch(index)
            {
                case 4:
                case 16:
                    {
                        return 0.25;
                    }
                case 7:
                case 19:
                    {
                        return 0.50;
                    }
                case 10:
                case 22:
                    {
                        return 0.75;
                    }
                case 13:
                case 25:
                    {
                        return 1;
                    }
            }
            return 0;
        }

        #endregion

        #region Экспорт в Excel

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");

            ReportExcelExporter1.Export(headerLayout, sheet1, 3);
        }

        #endregion

        #region Экспорт в Pdf

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();

            ReportPDFExporter1.Export(headerLayout, section1);
        }

        #endregion
    }
}