using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.UltraChart.Core;
using Infragistics.UltraChart.Core.Primitives;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.UltraChart.Shared.Events;
using Infragistics.UltraChart.Shared.Styles;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;
using Font=System.Drawing.Font;

namespace Krista.FM.Server.Dashboards.reports.FO_0002_0030
{
    public partial class Default : CustomReportPage
    {
        #region Поля

        private DataTable dtDate = new DataTable();
        private int firstYear = 2010;
        private int endYear;
        private int year;
        private DateTime currentDate;
        private string month;
        private bool flag = false;

        private GridHeaderLayout headerLayout1;
        private GridHeaderLayout headerLayout2;
        private GridHeaderLayout headerLayout3;
        private GridHeaderLayout headerLayout4;

        private static MemberAttributesDigest grbsDigest;

        #endregion

        #region Параметры запроса

      
        // выбранный множитель рублей
        private CustomParam rubMultiplier;
        // уровень районов
        private CustomParam regionsLevel;

        // предыдущий месяц 
        private CustomParam prevMonth;
        // следующий месяц после выбранного
        private CustomParam nextMonth;
        // следующий через один месяц после выбранного
        private CustomParam nextNextMonth;

        // месяцы выбранного года
        private CustomParam prCurMonth;

        // куб
        private CustomParam cube;
        // typeDolg
        private CustomParam dolg;
        private CustomParam indication;
        // должности
        private CustomParam dolgnosti;
        private CustomParam selectedBudget;
        private CustomParam nullMeasures;
        private CustomParam firstNullMeasures;
        private CustomParam tempMeasures;
        private CustomParam curTempMeasures;
        private CustomParam curYearMeasures;
        private CustomParam lastYearMeasures;
        private CustomParam planMeasures;
        private CustomParam factMeasures;
        private CustomParam levelBudget;
        private CustomParam typeDocument;
        // уровень бюджета (субъекта или поселения) 
        private CustomParam levelSubjectBudget;
        private CustomParam budget;
        private CustomParam orgGosVlast;
        //
        private CustomParam currentMonth;
        #endregion
 
        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid1.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid1.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2.0);
            UltraWebGrid1.DisplayLayout.NoDataMessage = "Нет данных";

            UltraWebGrid2.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid2.Height =  CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2.5);
            UltraWebGrid2.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid2.DataBound += new EventHandler(UltraWebGrid1_DataBound);

            UltraWebGrid3.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid3.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2.0);
            UltraWebGrid3.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid3.DataBound += new EventHandler(UltraWebGrid1_DataBound);

            UltraWebGrid4.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 20);
            UltraWebGrid4.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 2.0);
            UltraWebGrid4.DisplayLayout.NoDataMessage = "Нет данных";
            UltraWebGrid4.DataBound += new EventHandler(UltraWebGrid1_DataBound);
           
            #region Инициализация параметров запроса

            prevMonth = UserParams.CustomParam("prev_month");
            nextMonth = UserParams.CustomParam("next_month");
            nextNextMonth = UserParams.CustomParam("next_next_month");
            prCurMonth = UserParams.CustomParam("pr_cur_month");
            nullMeasures = UserParams.CustomParam("null_measures");
            firstNullMeasures = UserParams.CustomParam("first_null_measures");

            curYearMeasures = UserParams.CustomParam("cur_Measures");
            lastYearMeasures = UserParams.CustomParam("last_Measures");
            tempMeasures = UserParams.CustomParam("temp_measures");
            curTempMeasures = UserParams.CustomParam("cur_temp_measures");
            planMeasures = UserParams.CustomParam("plan_measures");
            factMeasures = UserParams.CustomParam("fact_measures");

            selectedBudget = UserParams.CustomParam("selected_budget");
            levelBudget = UserParams.CustomParam("level_budget");
            typeDocument = UserParams.CustomParam("type_document");
            rubMultiplier = UserParams.CustomParam("rub_multiplier");
            levelSubjectBudget = UserParams.CustomParam("subject_budget");
            budget = UserParams.CustomParam("budget");
            orgGosVlast = UserParams.CustomParam("org_vlast");

            cube = UserParams.CustomParam("cube");
            indication = UserParams.CustomParam("indication");
            dolg = UserParams.CustomParam("dolg");
            dolgnosti = UserParams.CustomParam("dolgnosti");

            currentMonth = UserParams.CustomParam("current_month");

            #endregion

            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportExcelExporter1.ExcelExporter.BeginExport += new BeginExportEventHandler(ExcelExporter_BeginExport);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
            ReportPDFExporter1.PdfExporter.EndExport += new EventHandler<Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs>(PdfExporter_EndExport);
        }

        void UltraWebGrid1_DataBound(object sender, EventArgs e)
        {
            ((UltraWebGrid) sender).Height = Unit.Empty;
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FO_0002_0030_Date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                month = dtDate.Rows[0][3].ToString();

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(endYear.ToString(), true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(month, true);

                ComboBudget.Title = "Бюджет";
                ComboBudget.Width = 500;
                ComboBudget.MultiSelect = false;
                ComboBudget.ParentSelect = false;
                grbsDigest = new MemberAttributesDigest(DataProvidersFactory.PrimaryMASDataProvider, "OrenburgGRBSList", CRHelper.BasePath);
                ComboBudget.FillDictionaryValues(CustomMultiComboDataHelper.FillMemberUniqueNameList(grbsDigest.UniqueNames, grbsDigest.MemberLevels));
                ComboBudget.SetСheckedState("Консолидированный бюджет субъекта", true);
            }

            Page.Title = "Финансовый паспорт субъекта РФ";
            PageTitle.Text = "Финансовый паспорт субъекта РФ";
            year = Convert.ToInt32(ComboYear.SelectedValue);
            month = ComboMonth.SelectedValue;
            
            currentDate = new DateTime(year, CRHelper.MonthNum(month), 1);
            currentDate = currentDate.AddMonths(1);
            PageSubTitle.Text = string.Format("Исполнение {0} за {1}-{2} годы, по состоянию на {3:dd.MM.yyyy} года", ComboBudget.SelectedValue, Convert.ToInt32(ComboYear.SelectedValue)-1, ComboYear.SelectedValue, currentDate);

            UserParams.PeriodYear.Value = year.ToString();
            UserParams.PeriodLastYear.Value = (year - 1).ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}",CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(ComboMonth.SelectedValue)));
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}",CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(ComboMonth.SelectedValue)));
           
            if (RubMiltiplierButtonList.SelectedIndex == 0)
            {
                rubMultiplier.Value = "1000";
            }
            else
            {
                rubMultiplier.Value = "1000000";
            }
           
            if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта")
            {
                tempMeasures.Value = "[Measures].[Темп к прошлому году_По субъекту], [Measures].[Темп к прошлому году_По ФО], [Measures].[Темп к прошлому году_По РФ]";
                curTempMeasures.Value = "[Measures].[темп роста к прошлому году / По субъекту], [Measures].[темп роста к прошлому году / По ФО], [Measures].[темп роста к прошлому году / По РФ] ";

                typeDocument.Value = "[ТипДокумента__СКИФ].[ТипДокумента__СКИФ].[Все].[Консолидированная отчетность и отчетность внебюджетных территориальных фондов]";
                levelBudget.Value = "[Уровни бюджета__СКИФ].[Уровни бюджета__СКИФ].[Все].[Конс.бюджет субъекта]";
                selectedBudget.Value = string.Empty;
                levelSubjectBudget.Value = "[Уровни бюджета__СКИФ].[Уровни бюджета__СКИФ].[Все].[Бюджет субъекта]";
            }
            else
            {
                tempMeasures.Value = "[Measures].[Темп к прошлому году_По субъекту]";
                curTempMeasures.Value = "[Measures].[темп роста к прошлому году / По субъекту]";

                typeDocument.Value = "[ТипДокумента__СКИФ].[ТипДокумента__СКИФ].[Тип документа ]";
                levelBudget.Value = "[Уровни бюджета__СКИФ].[Уровни бюджета__СКИФ].[Уровень бюджета ]";
                selectedBudget.Value = string.Format(",{0}", grbsDigest.GetMemberUniqueName(ComboBudget.SelectedValue));
                levelSubjectBudget.Value = "[Уровни бюджета__СКИФ].[Уровни бюджета__СКИФ].[Все].[Бюджет поселения]";
            }
            
            // прошлый год относительно выбранного в параметре, предыдущий месяц
            string nMonth = string.Format("{0}",(ComboMonth.SelectedValue == "Январь") ? ComboMonth.SelectedValue : CRHelper.RusMonth(CRHelper.MonthNum(ComboMonth.SelectedValue) - 1));
            prevMonth.Value = string.Format("[{0}].[Полугодие {1}].[Квартал {2}].[{3}]", year-1 ,CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(nMonth)) ,CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(nMonth)),nMonth);
            factMeasures.Value = "[Measures].[Исполнено за N+1]";

            if (ComboMonth.SelectedValue == "Декабрь")
            {
                nextMonth.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year - 1, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value,ComboMonth.SelectedValue);
                nextNextMonth.Value = string.Format("[{0}].[{1}].[{2}].[{3}]", year - 1, UserParams.PeriodHalfYear.Value, UserParams.PeriodQuater.Value, ComboMonth.SelectedValue);
                factMeasures.Value = " [Measures].[Исполнено с начала года]  ";
            }
            else if (ComboMonth.SelectedValue == "Ноябрь")
            {
                string NMonth = string.Format("{0}",CRHelper.RusMonth(CRHelper.MonthNum(ComboMonth.SelectedValue) + 1));
                nextMonth.Value = string.Format("[{0}].[Полугодие {1}].[Квартал {2}].[{3}]", year - 1, CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(NMonth)),CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(NMonth)), NMonth);
                nextNextMonth.Value = nextMonth.Value; 
            }
           else
                {
                    string NMonth = string.Format("{0}",
                                                  CRHelper.RusMonth(CRHelper.MonthNum(ComboMonth.SelectedValue) + 1));
                    nextMonth.Value = string.Format("[{0}].[Полугодие {1}].[Квартал {2}].[{3}]", year - 1,
                                                    CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(NMonth)),
                                                    CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(NMonth)), NMonth);

                    string NNMonth = string.Format("{0}",
                                                   CRHelper.RusMonth(CRHelper.MonthNum(ComboMonth.SelectedValue) + 2));
                    nextNextMonth.Value = string.Format("[{0}].[Полугодие {1}].[Квартал {2}].[{3}]", year - 1, CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(NNMonth)),
                                                        CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(NNMonth)), NNMonth);
                }
            
            
            // выбранный год
            string prevCurMonth = string.Format("{0}", (ComboMonth.SelectedValue == "Январь") ? ComboMonth.SelectedValue : CRHelper.RusMonth(CRHelper.MonthNum(ComboMonth.SelectedValue) - 1));
            prCurMonth.Value = string.Format("[{0}].[Полугодие {1}].[Квартал {2}].[{3}]",ComboYear.SelectedValue,CRHelper.HalfYearNumByMonthNum(CRHelper.MonthNum(prevCurMonth)),CRHelper.QuarterNumByMonthNum(CRHelper.MonthNum(prevCurMonth)),prevCurMonth);
           
            if (CRHelper.MonthNum(ComboMonth.SelectedValue)<10)
            {
                nullMeasures.Value = "[Measures].[Ожидаемое исполнение за N+1],[Measures].[темп роста N+1], [Measures].[Ожидаемое исполнение c начала года на 1 N+2], [Measures].[темп роста с начала года на 1 N+2],[Measures].[Ожидаемое исполнение за N+2], [Measures].[темп роста N+2], [Measures].[Ожидаемое исполнение с начала года на 1 N+3], [Measures].[темп роста с начала года на 1 N+3] ";
                firstNullMeasures.Value = " [Measures].[Ожидаемое исполнение за N+1], [Measures].[Темп роста_], [Measures].[Ожидаемое исполнение за N+2], [Measures].[Темп роста__], [Measures].[Ожидаемое исполнение c нач года на 1 N+2], [Measures].[Темп роста___], [Measures].[Ожидаемое исполнение c нач года на 1 N+3], [Measures].[Темп роста____] "; 
            }
            else
            {
                if (CRHelper.MonthNum(ComboMonth.SelectedValue) == 10)
                {
                    nullMeasures.Value = "[Measures].[Ожидаемое исполнение за N+1],[Measures].[темп роста N+1],  [Measures].[Ожидаемое исполнение c начала года на 1 N+2], [Measures].[темп роста с начала года на 1 N+2], [Measures].[Ожидаемое исполнение за N+2], [Measures].[темп роста N+2],  [Measures].[Ожидаемое исполнение с начала года на 1 N+3], [Measures].[темп роста с начала года на 1 N+3]";
                    firstNullMeasures.Value = " [Measures].[Ожидаемое исполнение за N+1], [Measures].[Темп роста_], [Measures].[Ожидаемое исполнение c нач года на 1 N+2], [Measures].[Темп роста___], [Measures].[Ожидаемое исполнение за N+2], [Measures].[Темп роста__],[Measures].[Ожидаемое исполнение c нач года на 1 N+3], [Measures].[Темп роста____] "; 
                }
                if (CRHelper.MonthNum(ComboMonth.SelectedValue) == 11)
                {
                    nullMeasures.Value = "[Measures].[Ожидаемое исполнение за N+1],[Measures].[темп роста N+1], [Measures].[Ожидаемое исполнение c начала года на 1 N+2], [Measures].[темп роста с начала года на 1 N+2]";
                    firstNullMeasures.Value = " [Measures].[Ожидаемое исполнение за N+1], [Measures].[Темп роста_] , [Measures].[Ожидаемое исполнение c нач года на 1 N+2], [Measures].[Темп роста___]"; 
                }

                if (CRHelper.MonthNum(ComboMonth.SelectedValue) == 12)
                {
                    firstNullMeasures.Value = "[Measures].[Исполнено за N+1], [Measures].[Исполнено с начала года], [Measures].[Темп к прошлому году_По субъекту] ";
                    nullMeasures.Value = "  [Measures].[темп роста к прошлому году / По субъекту]";
                }

            }

            budget.Value = string.Format("{0}", grbsDigest.GetMemberUniqueName(ComboBudget.SelectedValue));
            table1Captions.Font.Bold = true;
            table1Captions.Text = "Исполнение бюджета по доходам, расходам, и источникам финансирования дефицита бюджета";
            headerLayout1 = new GridHeaderLayout(UltraWebGrid1);
            UltraWebGrid1.Bands.Clear();
            UltraWebGrid1.DataBind();

            selectedBudget.Value = string.Format(",{0}", grbsDigest.GetMemberUniqueName(ComboBudget.SelectedValue));
            table2Captions.Font.Bold = true;
            table2Captions.Text = "Просроченная кредиторская задолженность";
            headerLayout2 = new GridHeaderLayout(UltraWebGrid2);
            UltraWebGrid2.Bands.Clear();
            UltraWebGrid2.DataBind();

            headerLayout3 = new GridHeaderLayout(UltraWebGrid3);
            table3Captions.Font.Bold = true;
            table3Captions.Text = "Справочная информация";
            UltraWebGrid3.Bands.Clear();
            UltraWebGrid3.DataBind();

            UltraWebGrid4.Visible = false;

           table4Captions.Text = string.Empty;
           if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта")
            {

                table4Captions.Font.Bold = true;
                table4Captions.Text = "Соблюдение требований Бюджетного кодекса РФ (используется информация об исполнении бюджета субъекта)";
                UltraWebGrid4.Visible = true; 
                headerLayout4 = new GridHeaderLayout(UltraWebGrid4);
                UltraWebGrid4.Bands.Clear();
                UltraWebGrid4.DataBind();
            }
          
        }

        #region Обработчики грида

        protected void UltraWebGrid1_DataBinding(object sender, EventArgs e)
        {
            if (ComboBudget.SelectedValue != "Консолидированный бюджет субъекта")
           {
                string queryType = DataProvider.GetQueryText("Typeterritory");
                DataTable dtType = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryType, "Наименование показателей", dtType);
                if (dtType.Rows.Count > 0)
                {
                  if (dtType.Rows[0][1].ToString() == "1") // муниципальные районы
                    {
                        flag = true;
                        orgGosVlast.Value ="[Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов местного самоуправления, всего].[на заработную плату] , [Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов местного самоуправления, всего].[на начисления на выплаты по оплате труда], [Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов местного самоуправления, всего].[расходы по содержанию органов местного самоуправления, направленные на выполнение полномочий Российской Федерации].[на заработную плату], [Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов местного самоуправления, всего].[расходы по содержанию органов местного самоуправления, направленные на выполнение полномочий Российской Федерации].[на начисления на выплаты по оплате труда] ";
                        typeDocument.Value = "[ТипДокумента__СКИФ].[ТипДокумента__СКИФ].[Все].[Консолидированный отчет муниципального района]";
                        levelBudget.Value = "[Уровни бюджета__СКИФ].[Уровни бюджета__СКИФ].[Все].[Конс.бюджет субъекта]";
                    }
                  else
                  {
                      flag = false;
                      if (dtType.Rows[0][1].ToString() == "2") // городские округа
                      {
                          orgGosVlast.Value = "[Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов местного самоуправления, всего].[на заработную плату] ,[Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов местного самоуправления, всего].[на начисления на выплаты по оплате труда], [Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов местного самоуправления, всего].[расходы по содержанию органов местного самоуправления, направленные на выполнение полномочий Российской Федерации].[на заработную плату], [Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов местного самоуправления, всего].[расходы по содержанию органов местного самоуправления, направленные на выполнение полномочий Российской Федерации].[на начисления на выплаты по оплате труда]";
                          typeDocument.Value = "[ТипДокумента__СКИФ].[ТипДокумента__СКИФ].[Все].[Собственный отчет городского округа]";
                          levelBudget.Value = "[Уровни бюджета__СКИФ].[Уровни бюджета__СКИФ].[Все].[Конс.бюджет субъекта]";
                      }
                      else  // бюджет субъекта
                      {
                          orgGosVlast.Value =  "[Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов государственной власти субъекта Российской Федерации, всего].[на заработную плату] , [Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов государственной власти субъекта Российской Федерации, всего].[на начисления на выплаты по оплате труда], [Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов государственной власти субъекта Российской Федерации, всего].[расходы по содержанию органов государственной власти субъекта Российской Федерации, направленные на выполнение полномочий Российской Федерации].[на начисления на выплаты по оплате труда], [Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов местного самоуправления, всего].[на заработную плату] , [Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов местного самоуправления, всего].[на начисления на выплаты по оплате труда] ";
                          typeDocument.Value = "[ТипДокумента__СКИФ].[ТипДокумента__СКИФ].[Все].[Отчетность главных распорядителей средств бюджета]";
                          levelBudget.Value =  "[Уровни бюджета__СКИФ].[Уровни бюджета__СКИФ].[Все].[Бюджет субъекта]";
                      }
                  }
                }
           }

           if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта")
           {
               orgGosVlast.Value = " [Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов государственной власти субъекта Российской Федерации, всего].[на заработную плату], [Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов государственной власти субъекта Российской Федерации, всего].[на начисления на выплаты по оплате труда], [Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов государственной власти субъекта Российской Федерации, всего].[расходы по содержанию органов государственной власти субъекта Российской Федерации, направленные на выполнение полномочий Российской Федерации].[на заработную плату], [Показатели__СопМесОСпРасх].[Показатели__СопМесОСпРасх].[Все показатели].[Расходы по содержанию органов государственной власти субъекта Российской Федерации, всего].[расходы по содержанию органов государственной власти субъекта Российской Федерации, направленные на выполнение полномочий Российской Федерации].[на начисления на выплаты по оплате труда]";}
            
            if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта" || flag)
            {
                curYearMeasures.Value = ",[Measures].[Исполнено год_в т.ч. бюджет субъекта]";
                lastYearMeasures.Value = "[Measures].[Исполнено на 1_в т.ч. бюджет субъекта]";
                planMeasures.Value = "[Measures].[Уточненный план_бюджет субъекта]";
                currentMonth.Value = string.Format("+ {2} [Период__Период].[Период__Период].[Данные всех периодов].{0}{3} * {2}[Measures].[Исполнено на 1_консолидированный бюджет],{1}{3}", prevMonth.Value, lastYearMeasures.Value,"{","}");
            }
            else
            {
                curYearMeasures.Value = string.Empty; //curYearMeasures.Value = "[Measures].[Исполнено год_консолидированный бюджет]";
                lastYearMeasures.Value = "[Measures].[Исполнено на 1_консолидированный бюджет]";
                planMeasures.Value = "[Measures].[Уточненный план_конс бюджет]";
                currentMonth.Value = string.Empty;
            }

           string query = DataProvider.GetQueryText("FO_0002_0030_MesOtchGrid");
           DataTable dtGridMesOtch = new DataTable();
           DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGridMesOtch);
           dtGridMesOtch.PrimaryKey = new DataColumn[] { dtGridMesOtch.Columns[0] };

            if (dtGridMesOtch.Rows.Count > 0)
            {
                 query = DataProvider.GetQueryText("FO_0002_0030_GodOtchGrid");
                 DataTable dtGridGodOtch = new DataTable();
                 DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGridGodOtch);

                 if (dtGridGodOtch.Rows.Count > 0)
                 {
                     for (int i = 0; i < dtGridGodOtch.Rows.Count; i++)
                     {
                         string[] rowName = new string[] {dtGridGodOtch.Rows[i][0].ToString()};
                         DataRow row = dtGridMesOtch.Rows.Find(rowName);
                         if (row != null)
                         {
                             for (int indCol = 1; indCol < dtGridGodOtch.Columns.Count; indCol++)
                             {
                                 row[indCol] = dtGridGodOtch.Rows[i][indCol];
                             }
                         }
                     }
                 }

                 if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта" || flag)
                 {
                     query = DataProvider.GetQueryText("FO_0002_0030_GodOthSources");
                     DataTable dtGridGodOtchSources = new DataTable();
                     DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей",
                                                                                      dtGridGodOtchSources);
                     if (dtGridGodOtchSources.Rows.Count > 0)
                     {
                         for (int i = 0; i < dtGridGodOtchSources.Rows.Count; i++)
                         {
                             string[] rowName = new string[] { dtGridGodOtchSources.Rows[i][0].ToString() };
                             DataRow row = dtGridMesOtch.Rows.Find(rowName);
                             if (row != null)
                             {
                               row[3] = dtGridGodOtchSources.Rows[i][1];
                             }
                         }
                     }
                    
                     query = DataProvider.GetQueryText("FO_0002_0030_MesOtchGridTempRosta");
                     DataTable dtGridTempRosta = new DataTable();
                     DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей",
                                                                                      dtGridTempRosta);
                     if (dtGridTempRosta.Rows.Count > 0)
                     {
                         for (int i = 0; i < dtGridTempRosta.Rows.Count; i++)
                         {
                             string[] rowName = new string[] {dtGridTempRosta.Rows[i][0].ToString()};
                             DataRow row = dtGridMesOtch.Rows.Find(rowName);
                             if (row != null)
                             {
                              
                                 for (int j = 1; j < dtGridMesOtch.Columns.Count; j++ )
                                 {
                                     string captions = dtGridMesOtch.Columns[j].Caption;
                                     if (captions.Contains("Темп к прошлому году_По ФО"))
                                     {
                                         row[j] = dtGridTempRosta.Rows[i][1];
                                     }
                                     if (captions.Contains("Темп к прошлому году_По РФ"))
                                     {
                                         row[j] = dtGridTempRosta.Rows[i][2];
                                     }
                                 }
                            }
                         }
                     }

                 }
             
                 // расчет темпа уточненного плана к прошлому году 
                 double multiplier = Convert.ToInt32(rubMultiplier.Value);
                 for (int rowNum = 0; rowNum < dtGridMesOtch.Rows.Count; rowNum++ )
                 {
                     double value = 1;
                     for (int j = 1; j < dtGridMesOtch.Columns.Count; j++)
                     {
                         string captions = dtGridMesOtch.Columns[j].Caption;

                         if (captions.Contains("Исполнено год_консолидированный бюджет"))
                         {
                             if (dtGridMesOtch.Rows[rowNum][j] != DBNull.Value && dtGridMesOtch.Rows[rowNum][j].ToString() != string.Empty )
                             {
                               value = Convert.ToDouble(dtGridMesOtch.Rows[rowNum][j]);
                             }
                         }
                         
                         if (captions.Contains("Темп уточ. плана к пред году"))
                         {
                           if (dtGridMesOtch.Rows[rowNum][j] != DBNull.Value && dtGridMesOtch.Rows[rowNum][j].ToString() != string.Empty && value !=0)
                           {

                               dtGridMesOtch.Rows[rowNum][j] = Convert.ToDouble(dtGridMesOtch.Rows[rowNum][j]) / value / multiplier;
                           }
                         }
                      
                     }
                     dtGridMesOtch.AcceptChanges();
                 }

                UltraWebGrid1.DataSource = dtGridMesOtch;
                 
            }

        }

        protected void UltraWebGrid2_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0030_Grid2");
            DataTable dtGrid2 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGrid2);

            if (dtGrid2.Rows.Count > 0)
            {
                UltraWebGrid2.DataSource = dtGrid2;
            }

        }

        protected void UltraWebGrid3_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0030_Grid3");
            DataTable dtGrid3 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGrid3);

            if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта")
            {
                query = DataProvider.GetQueryText("FO_0002_0030_CountKonst");
                DataTable dtCount = new DataTable();
                DataProvidersFactory.SecondaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtCount);

               if (dtCount.Rows.Count > 0)
                {
                    if (dtGrid3.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtGrid3.Rows)
                        {
                            dtCount.ImportRow(row);
                        }
                        dtCount.AcceptChanges();
                    }
                query = DataProvider.GetQueryText("FO_0002_0030_Nedoimka");
                DataTable dtNedoimka = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtNedoimka);
                if (dtNedoimka.Rows.Count > 0)
                  {
                    foreach (DataRow row in dtNedoimka.Rows)
                     {
                        dtCount.ImportRow(row);
                     }
                    dtCount.AcceptChanges();
                  }
                // тип долга
                dolg.Value = "[Показатели__СопМесОСпВнтД].[Показатели__СопМесОСпВнтД].[Государственный долг субъекта], [Показатели__СопМесОСпВнтД].[Показатели__СопМесОСпВнтД].[Муниципальный долг]"; 
                query = DataProvider.GetQueryText("FO_0002_0030_GosDolg");
                DataTable dtGosDolg = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGosDolg);
                if (dtGosDolg.Rows.Count > 0)
                {
                    foreach (DataRow row in dtGosDolg.Rows)
                    {
                        dtCount.ImportRow(row);
                    }
                    dtCount.AcceptChanges();
                }

                query = DataProvider.GetQueryText("FO_0002_0030_Build");
                DataTable dtBuild = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtBuild);
                if (dtBuild.Rows.Count > 0)
                {
                    foreach (DataRow row in dtBuild.Rows)
                    {
                        dtCount.ImportRow(row);
                    }
                    dtCount.AcceptChanges();
                }
                

                query = DataProvider.GetQueryText("FO_0002_0030_AllCountKonst");
                DataTable dtAllCount = new DataTable();
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtAllCount);
                if (dtAllCount.Rows.Count > 0)
                {
                    foreach (DataRow row in dtAllCount.Rows)
                    {
                        dtCount.ImportRow(row);
                    }
                    dtCount.AcceptChanges();
                }


             
            } 

                DataRow rowAdd = dtCount.NewRow();
                rowAdd[0] = "Установленный уровень возмещения затрат за предоставленные жилищно-коммунальные услуги в %";
                rowAdd[1] = DBNull.Value;
                rowAdd[2] = DBNull.Value;
                rowAdd[3] = DBNull.Value;
                dtCount.Rows.Add(rowAdd);

                rowAdd = dtCount.NewRow();
                rowAdd[0] = "Фактический уровень возмещения затрат за предоставленные жилищно-коммунальные услуги в %";
                rowAdd[1] = DBNull.Value;
                rowAdd[2] = DBNull.Value;
                rowAdd[3] = DBNull.Value;
                dtCount.Rows.Add(rowAdd);

                rowAdd = dtCount.NewRow();
                rowAdd[0] = "Достижение наилучших значений показателей деятельности органов исполнительной власти субъектов Российской Федерации";
                rowAdd[1] = DBNull.Value;
                rowAdd[2] = DBNull.Value;
                rowAdd[3] = DBNull.Value;
                dtCount.Rows.Add(rowAdd);

                rowAdd = dtCount.NewRow();
                rowAdd[0] = "Степень качества организации бюджетного процесса в соответствии с приказом Минфина № 552 от 3 декабря 2010 г.";
                rowAdd[1] = DBNull.Value;
                rowAdd[2] = DBNull.Value;
                rowAdd[3] = DBNull.Value;
                dtCount.Rows.Add(rowAdd);

                UltraWebGrid3.DataSource = dtCount;
            }
            else
            {
                
                    string queryType = DataProvider.GetQueryText("Typeterritory");
                    DataTable dtType = new DataTable();
                    DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(queryType, "Наименование показателей", dtType);

                    if (dtType.Rows.Count > 0)
                    {
                        if (dtType.Rows[0][1].ToString() == "1" || dtType.Rows[0][1].ToString() == "2") // Муниципальные районы или городские округа
                        {

                            cube.Value = string.Format("{0}", grbsDigest.GetMemberUniqueName(ComboBudget.SelectedValue));
                            indication.Value = "[Показатели__Показатели для планирования Сопост].[Показатели__Показатели для планирования Сопост].[Все показатели].[Данные по численности населения и показателям].[Численность населения]";
                           
                            query = DataProvider.GetQueryText("FO_0002_0030_Count");
                            DataTable dtCount = new DataTable();
                            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtCount);

                            //тип долга
                            dolg.Value = "[Показатели__СопМесОСпВнтД].[Показатели__СопМесОСпВнтД].[Муниципальный долг]"; 
                            query = DataProvider.GetQueryText("FO_0002_0030_GosDolg");
                            DataTable dtGosDolg = new DataTable();
                            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGosDolg);
                            if (dtGosDolg.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtGosDolg.Rows)
                                {
                                    dtCount.ImportRow(row);
                                }
                                dtCount.AcceptChanges();
                            }

                            query = DataProvider.GetQueryText("FO_0002_0030_Build");
                            DataTable dtBuild = new DataTable();
                            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtBuild);
                            if (dtBuild.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtBuild.Rows)
                                {
                                    dtCount.ImportRow(row);
                                }
                                dtCount.AcceptChanges();
                            }

                            dolgnosti.Value = " [Должности__Сопоставимый].[Должности__Сопоставимый].[Все].[Общая численность работников  органа местного самоуправления, избирательной комиссии муниципального образования  (сумма строк 200+210+270+280)]";
                            query = DataProvider.GetQueryText("FO_0002_0030_AllCount");
                            DataTable dtAllCount = new DataTable();
                            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtAllCount);
                            if (dtAllCount.Rows.Count > 0)
                            {
                                foreach (DataRow row in dtAllCount.Rows)
                                {
                                    dtCount.ImportRow(row);
                                }
                                dtCount.AcceptChanges();
                            } 
                       
                            UltraWebGrid3.DataSource = dtCount;
                        }
                        else // бюджет субъекта
                        {

                            //тип долга
                            dolg.Value = "[Показатели__СопМесОСпВнтД].[Показатели__СопМесОСпВнтД].[Государственный долг субъекта]";
                            query = DataProvider.GetQueryText("FO_0002_0030_GosDolg");
                            DataTable dtGosDolg = new DataTable();
                            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGosDolg);
                            if (dtGosDolg.Rows.Count > 0)
                            {
                                query = DataProvider.GetQueryText("FO_0002_0030_Build");
                                DataTable dtBuild = new DataTable();
                                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtBuild);
                                if (dtBuild.Rows.Count > 0)
                                {
                                    foreach (DataRow row in dtBuild.Rows)
                                    {
                                        dtGosDolg.ImportRow(row);
                                    }
                                    dtGosDolg.AcceptChanges();
                                }

                                dolgnosti.Value = "[Должности__Сопоставимый].[Должности__Сопоставимый].[Все].[Общая численность работников  государственного органа ( органа местного самоуправления)    (сумма строк 200+230+280+290+300)]";
                                query = DataProvider.GetQueryText("FO_0002_0030_AllCount");
                                DataTable dtAllCount = new DataTable();
                                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtAllCount);
                                if (dtAllCount.Rows.Count > 0)
                                {
                                    foreach (DataRow row in dtAllCount.Rows)
                                    {
                                        dtGosDolg.ImportRow(row);
                                    }
                                    dtGosDolg.AcceptChanges();
                                } 
                       
                               UltraWebGrid3.DataSource = dtGosDolg;
                            }
                        }
                     
                    }
                   
          }

        }
        
        protected void UltraWebGrid4_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FO_0002_0030_Grid4");
            DataTable dtGrid4 = new DataTable();
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGrid4);
            dtGrid4.PrimaryKey = new DataColumn[] { dtGrid4.Columns[0] };
            if (dtGrid4.Rows.Count > 0)
            {
                 query = DataProvider.GetQueryText("FO_0002_0030_Grid4MesOtch");
                 DataTable dtGrid4MesOtch = new DataTable();
                 DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Наименование показателей", dtGrid4MesOtch);

                 if (dtGrid4MesOtch.Rows.Count > 0)
                 {
                     for (int i = 0; i < dtGrid4MesOtch.Rows.Count; i++)
                     {
                         string[] rowName = new string[] { dtGrid4MesOtch.Rows[i][0].ToString() };
                         DataRow row = dtGrid4.Rows.Find(rowName);
                         if (row != null)
                         {
                             row[2] = dtGrid4MesOtch.Rows[i][1];
                             row[3] = dtGrid4MesOtch.Rows[i][2];
                         }
                     }
                 }

                dtGrid4.AcceptChanges();
                UltraWebGrid4.DataSource = dtGrid4;
            }

        }
        
        protected void UltraWebGrid1_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowSortingDefault = AllowSorting.No;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[UltraWebGrid1.Columns.Count - 1].Hidden = true;
            e.Layout.Bands[0].Columns[UltraWebGrid1.Columns.Count - 2].Hidden = true;
            month = ComboMonth.SelectedValue;
            headerLayout1.AddCell("Наименование показателей");

            GridHeaderCell cell = headerLayout1.AddCell(string.Format("{0}",year-1));
            cell.AddCell("План");
            GridHeaderCell cell1 = cell.AddCell("Исполнено год");
           
            if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта")
            {
                cell1.AddCell("Конс. бюджет");
                cell1.AddCell("в т.ч. бюджет субъекта");
            }
            if (flag)
            {
                cell1.AddCell("Конс. бюджет");
                cell1.AddCell("в т.ч. бюджет поселений");
            }

            
            if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта")
            {
                GridHeaderCell cell2 = cell.AddCell(string.Format("Исполнено с начала года на 1 {0}", (ComboMonth.SelectedValue == "Январь") ? "февраля" : CRHelper.RusMonthGenitive(CRHelper.MonthNum(ComboMonth.SelectedValue))));
                cell2.AddCell("Конс. бюджет");
                cell2.AddCell("в т.ч. бюджет субъекта");
            }
            if (flag)
            {
                GridHeaderCell cell2 = cell.AddCell(string.Format("Исполнено с начала года на 1 {0}", (ComboMonth.SelectedValue == "Январь") ? "февраля" : CRHelper.RusMonthGenitive(CRHelper.MonthNum(ComboMonth.SelectedValue) )));
                cell2.AddCell("Конс. бюджет");
                cell2.AddCell("в т.ч. бюджет поселений");
            }
            
            int numMonth = CRHelper.MonthNum(month);
            for (int i = 2; i < numMonth +2; i++)
            {
                cell.AddCell(string.Format("Исполнено с начала года на 1 {0}", CRHelper.RusMonthGenitive(i)));
            }

            if (CRHelper.MonthNum(ComboMonth.SelectedValue)<12) // ноябрь
            {
                cell.AddCell(string.Format("Исполнено за {0}", CRHelper.RusMonth(numMonth +1)));
                cell.AddCell(string.Format("Исполнено с начала года на 1 {0}", CRHelper.RusMonthGenitive(numMonth + 2)));
            }
            if (CRHelper.MonthNum(ComboMonth.SelectedValue) < 11) // октябрь
            {
                cell.AddCell(string.Format("Исполнено за {0}", CRHelper.RusMonth(numMonth + 2)));
                cell.AddCell(string.Format("Исполнено с начала года на 1 {0}", CRHelper.RusMonthGenitive(numMonth + 3)));
            }
          

            // выбранный год
            GridHeaderCell cell3 = headerLayout1.AddCell(string.Format("{0}", year));
            cell3.AddCell("План");
            GridHeaderCell cell4 = cell3.AddCell(string.Format("Уточненный план с начала года на 1 {0}", CRHelper.RusMonthGenitive(CRHelper.MonthNum(ComboMonth.SelectedValue)+1)));

            if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта")
            {
                cell4.AddCell("Конс. бюджет");
                cell4.AddCell("в т.ч. бюджет субъекта");
            }
            if (flag)
            {
                cell4.AddCell("Конс. бюджет");
                cell4.AddCell("в т.ч. бюджет поселений");
            }
            cell3.AddCell("Темп уточненного плана к пред. году, %");

            GridHeaderCell cell6 = cell3.AddCell(string.Format("Исполнение с начала года на 1 {0}", (ComboMonth.SelectedValue == "Январь") ? "февраля" : CRHelper.RusMonthGenitive(CRHelper.MonthNum(ComboMonth.SelectedValue) )));
            if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта")
            {
                cell6.AddCell("Конс. бюджет");
                cell6.AddCell("в т.ч. бюджет субъекта");
            }
            if (flag)
            {
                cell6.AddCell("Конс. бюджет");
                cell6.AddCell("в т.ч. бюджет поселений");
            }
            GridHeaderCell cell5 = cell3.AddCell(string.Format("Темп роста к прошлому году"));
            if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта")
            {
                cell5.AddCell("По субъекту");
                cell5.AddCell("По ФО");
                cell5.AddCell("По РФ");
            }

            numMonth = CRHelper.MonthNum(month);
            for (int i = 2; i < numMonth + 1; i++)
            {
                cell3.AddCell(string.Format("Исполнено с начала года на 1 {0}", CRHelper.RusMonthGenitive(i)));
            }

            cell3.AddCell(string.Format("Исполнено за {0}", CRHelper.RusMonth(CRHelper.MonthNum(ComboMonth.SelectedValue))));
            cell3.AddCell(string.Format("Исполнено с начала года на 1 {0}", CRHelper.RusMonthGenitive(numMonth +1)));
            cell3.AddCell("Темп роста, %");
            if (CRHelper.MonthNum(ComboMonth.SelectedValue) < 12)
            {
                cell3.AddCell(string.Format("Ожидаемое исполнение за {0}", CRHelper.RusMonth(numMonth + 1)));
                cell3.AddCell("Темп роста, %");
                cell3.AddCell(string.Format("Ожидаемое исполнение с начала года 1 {0}", CRHelper.RusMonthGenitive(numMonth + 2)));
                cell3.AddCell("Темп роста, %");
            }
            if (CRHelper.MonthNum(ComboMonth.SelectedValue) < 11)
            {
                cell3.AddCell(string.Format("Ожидаемое исполнение за {0}", CRHelper.RusMonth(numMonth + 2)));
                cell3.AddCell("Темп роста, %");
               
            }

            if (CRHelper.MonthNum(ComboMonth.SelectedValue) <= 10)
            {
                cell3.AddCell(string.Format("Ожидаемое исполнение с начала года 1 {0}", CRHelper.RusMonthGenitive(numMonth + 3)));
                cell3.AddCell("Темп роста, %");

            }
            cell3.AddCell("Оценка субъекта");
            cell3.AddCell("Темп к прошлому году, %");
            cell3.AddCell("Оценка субъекта, с учетом прогноза");
            cell3.AddCell("Темп к прошлому году, %");
            cell3.AddCell("Оценка МинФина РФ");
            cell3.AddCell("Темп к прошлому году, %");

            headerLayout1.ApplyHeaderInfo();

           for (int i = 1; i < UltraWebGrid1.Columns.Count; i++)
            {
                string columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption.ToLower();
                string formatString = columnCaption.Contains("темп") || columnCaption.Contains("по субъекту") ? "P0" : "N0";
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
            }
        }

        private static bool IsInvertIndication(string indicatorName) // красная стрелка вниз, зеленая вверх
        {
            switch (indicatorName)
            {

                case "Безвозмездные перечисления государственным и муниципальным предприятиям, за исключением государственных и муниципальных организаций":
                case "Безвозмездные перечисления государственным и муниципальным предприятиям":
                case "Прочие работы и услуги":
                case "Расходы на прочие нужды":
                case "увеличение стоимости мат.запасов":
                case "арендная плата за пользование имуществом":
                case "транспортные услуги":
                case "услуги связи":
                case "Расходы на первоочередные нужды, из них":
                case "Раздел II. Первоочередные расходы":
                case "ИТОГО РАСХОДОВ":
                case "Итого расходов без учёта безвозмездных поступлений":
                case "Общегосударственные вопросы":
                case "Национальная оборона":
                case "Нац. безопасность и правоохр. деят-ть":
                case "Национальная экономика":
                case "Жилищно-коммунальное хозяйство":
                case "Межбюджетные трансферты":
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }



        protected void UltraWebGrid1_InitializeRow(object sender, RowEventArgs e)
        {
           
            string indicatorName = String.Empty;
            if (e.Row.Cells[0].Value != null)
            {
                indicatorName = e.Row.Cells[0].Value.ToString().Replace("_", String.Empty);
                e.Row.Cells[0].Value = indicatorName;
            }
            bool isInvertIndication = IsInvertIndication(indicatorName.TrimEnd(' '));

            for (int i=1; i<e.Row.Cells.Count ; i++)
            {
                string columnCaption = e.Row.Band.Columns[i].Header.Caption.ToLower();
                bool rate = columnCaption.Contains("темп роста") || columnCaption.Contains("по субъекту") || columnCaption.Contains("по фо") || columnCaption.Contains("по рф");
               
                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        
                        double currentValue = Convert.ToDouble(e.Row.Cells[i].Value);

                        if (currentValue*100 < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = isInvertIndication
                                                                       ? "~/images/arrowGreenDownBB.png"
                                                                       : "~/images/arrowRedDownBB.png" ;
                            e.Row.Cells[i].Title = "Наблюдается снижение исполнения относительно предыдущего года";
                        }
                        else if (currentValue*100 > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = isInvertIndication
                                                                       ? "~/images/arrowRedUpBB.png"
                                                                       :"~/images/arrowGreenUpBB.png";
                            e.Row.Cells[i].Title = "Наблюдается рост исполнения относительно предыдущего года";
                        }

                    }
                    e.Row.Cells[i].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                string captions = e.Row.Cells[i].Column.Header.Caption;
                if (captions.Contains("По РФ") || captions.Contains("По ФО"))
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {
                        e.Row.Cells[i].Value = Convert.ToDouble(e.Row.Cells[i].Value).ToString("P0");
                    }
                }
            }
            string level = e.Row.Cells[e.Row.Cells.Count - 2].Value.ToString();

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                switch (level)
                {
                    case "1":
                        {
                            e.Row.Cells[i].Style.Font.Bold = true;
                            e.Row.Cells[i].Style.Font.Size = 9;
                            e.Row.Cells[0].ColSpan = UltraWebGrid1.Columns.Count - 2;
                            break;
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
            if (e.Row.Cells[0].Value != null)
            {
                string caption = e.Row.Cells[0].Value.ToString();
                if (caption == "доля межбюджетных трансфертов из федерального бюджета (за исключением субвенций) в доходах")
                {
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            e.Row.Cells[i].Value = Convert.ToDouble(e.Row.Cells[i].Value).ToString("P0");
                        }
                    }

                }
            }
            
        }
       
        protected void UltraWebGrid2_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            month = ComboMonth.SelectedValue;
            headerLayout2.AddCell("Наименование показателей");
            // предыдущий относительно выбранного год
            GridHeaderCell cell = headerLayout2.AddCell(string.Format("{0}", year-1));
            GridHeaderCell cell0 = cell.AddCell("Исполнено год");
           
            if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта")
            {
                cell0.AddCell("Конс. бюджет");
                cell0.AddCell("в т.ч. бюджет субъекта");    
            }
            if (flag)
            {
                cell0.AddCell("Конс. бюджет");
                cell0.AddCell("в т.ч. бюджет поселений");   
            }
            currentDate = new DateTime(year,CRHelper.MonthNum(month.ToLower()), 1);
            int numMonth = CRHelper.MonthNum(month);
           
            
            if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта")
            {
                GridHeaderCell cell1 = cell.AddCell(string.Format("Исполнено с начала года на 1 {0}", (ComboMonth.SelectedValue == "Январь") ? "февраля" : CRHelper.RusMonthGenitive(numMonth)));
                cell1.AddCell("Конс. бюджет");
                cell1.AddCell("в т.ч. бюджет субъекта");
            }
            if (flag)
            {
                GridHeaderCell cell1 = cell.AddCell(string.Format("Исполнено с начала года на 1 {0}", (ComboMonth.SelectedValue == "Январь") ? "февраля" : CRHelper.RusMonthGenitive(numMonth)));
                cell1.AddCell("Конс. бюджет");
                cell1.AddCell("в т.ч. бюджет поселений");
            }

            numMonth = CRHelper.MonthNum(month);
            for (int i = 2; i < numMonth + 2; i++ )
            {
                cell.AddCell(string.Format("Исполнено с начала года на 1 {0}", CRHelper.RusMonthGenitive(i)));
            }

            if (numMonth <=10)
             {
                cell.AddCell(string.Format("Исполнено с начала года на 1 {0}", CRHelper.RusMonthGenitive(numMonth + 2)));
             }
            if (ComboMonth.SelectedValue != "Декабрь")
            {
                cell.AddCell(string.Format("Исполнено с начала года на 1 {0}", CRHelper.RusMonthGenitive(numMonth +3)));
            }

            //выбранный год
            GridHeaderCell cell2 = headerLayout2.AddCell(string.Format("{0}", year));
            GridHeaderCell cell3 = cell2.AddCell(string.Format("Исполнено с начала года на 1 {0}",CRHelper.RusMonthGenitive(CRHelper.MonthNum(month.ToLower())+1)));
            
            if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта")
            {
                cell3.AddCell("Конс. бюджет");
                cell3.AddCell("в т.ч. бюджет субъекта");
            }
            if (flag)
            {
                cell3.AddCell("Конс. бюджет");
                cell3.AddCell("в т.ч. бюджет поселений");
            }
            GridHeaderCell cell4 = cell2.AddCell("Темп роста к прошлому году, %");

            if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта")
            {
              cell4.AddCell("По субъекту");
              cell4.AddCell("По ФО");
              cell4.AddCell("По РФ");
            }
            numMonth = CRHelper.MonthNum(month);
            for (int i = 2; i < numMonth + 2; i++)
            {
                cell2.AddCell(string.Format("Исполнено с начала года на 1 {0}", CRHelper.RusMonthGenitive(i)));
            }
            cell2.AddCell("Темп роста, %");

            if (CRHelper.MonthNum(ComboYear.SelectedValue) < 12)
            {
                cell2.AddCell(string.Format("Ожидаемое исполнение за {0}", CRHelper.RusMonth(numMonth + 1)));
                cell2.AddCell("Темп роста, %");

                cell2.AddCell(string.Format("Ожидаемое исполнение с начала года на 1 {0}", CRHelper.RusMonthGenitive(numMonth + 2)));
                cell2.AddCell("Темп роста, %");
                if  (CRHelper.MonthNum(ComboYear.SelectedValue) < 11)
                {
                    cell2.AddCell(string.Format("Ожидаемое исполнение за {0}", CRHelper.RusMonth(numMonth + 2)));
                    cell2.AddCell("Темп роста, %");
               
                }
                if (CRHelper.MonthNum(ComboYear.SelectedValue) <= 10)
                {
                    cell2.AddCell(string.Format("Ожидаемое исполнение с начала года на 1 {0}", CRHelper.RusMonthGenitive(numMonth + 3)));
                    cell2.AddCell("Темп роста, %");
                }
             
            }
         
            headerLayout2.ApplyHeaderInfo();

            for (int i = 1; i < UltraWebGrid2.Columns.Count; i++)
            {
                string columnCaption = e.Layout.Bands[0].Columns[i].Header.Caption.ToLower();
                string formatString = columnCaption.Contains("темп роста") || columnCaption.Contains("по субъекту") ? "P0" : "N0";
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
            }
        }

        protected void UltraWebGrid2_InitializeRow(object sender, RowEventArgs e)
        {

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
            for (int i=1; i < e.Row.Cells.Count; i++)
            {
                string columnCaption = e.Row.Band.Columns[i].Header.Caption.ToLower();
                bool rate = columnCaption.Contains("темп роста") || columnCaption.Contains("по субъекту");
                if (rate)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {

                        double currentValue = Convert.ToDouble(e.Row.Cells[i].Value);

                        if (currentValue*100 > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedUpBB.png";
                            e.Row.Cells[i].Title = "Наблюдается рост исполнения относительно предыдущего года";
                        }
                        else if (currentValue*100 < 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenDownBB.png";
                            e.Row.Cells[i].Title = "Наблюдается снижение исполнения относительно предыдущего года";
                        }

                    }
                    e.Row.Cells[i].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }
        }

        protected void UltraWebGrid3_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(200);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;
            int numMonth = CRHelper.MonthNum(ComboMonth.SelectedValue);

            headerLayout3.AddCell("Наименование показателей");
            GridHeaderCell cell = headerLayout3.AddCell(string.Format("{0}", year - 1));
            cell.AddCell("Исполнение год");
            cell.AddCell(string.Format("Исполнено с начала года на 1 {0}", CRHelper.RusMonthGenitive(numMonth + 1)));
            GridHeaderCell cell1 = headerLayout3.AddCell(string.Format("{0}", year));
            cell1.AddCell(string.Format("Исполнение с начала года на 1 {0}",CRHelper.RusMonthGenitive(numMonth+1)));

            headerLayout3.ApplyHeaderInfo();

            for (int i = 1; i < UltraWebGrid3.Columns.Count; i++)
            {
                string formatString = "N0";
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], formatString);
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(150);
            }
        }

        protected void UltraWebGrid3_InitializeRow(object sender, RowEventArgs e)
        {
            
            if (e.Row.Cells[0].Value != DBNull.Value)
            {
                if ( e.Row.Cells[0].Value.ToString() == "Численность работающих в органах государственной власти, чел. " || e.Row.Cells[0].Value.ToString() == "Численность работающих в бюджетной сфере, чел., в т.ч." || e.Row.Cells[0].Value.ToString() == "в здравоохранении" || e.Row.Cells[0].Value.ToString() == "в образовании")
                {
                    e.Row.Cells[1].ColSpan = 2;

                    for (int i = 1; i < e.Row.Cells.Count-1; i++)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            e.Row.Cells[i].Value = Convert.ToDouble(e.Row.Cells[i].Value).ToString("N0");
                        }
                    }
                    

                }

                else if (e.Row.Cells[0].Value.ToString() == "Доля налогов, поступающих в федеральный бюджет, %" || e.Row.Cells[0].Value.ToString() == "Доля налогов, поступающих в конс. бюджет региона, %")
                {
                    for (int i = 1; i < e.Row.Cells.Count; i++)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            e.Row.Cells[i].Value = Convert.ToDouble(e.Row.Cells[i].Value).ToString("P0");
                        }
                    }
                }
               else
                {
                    for (int i = 1; i < e.Row.Cells.Count-1; i++)
                    {
                        if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                        {
                            e.Row.Cells[i].Value = Convert.ToDouble(e.Row.Cells[i].Value).ToString("N0");
                        }
                    }
                }
                
                if (e.Row.Cells[0].Value.ToString() == "Численность населения (чел.) ")
                  {
                      e.Row.Cells[1].ColSpan = 2;
                      for (int i = 1; i < e.Row.Cells.Count; i++)
                      {
                          if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                          {
                              e.Row.Cells[i].Value = Convert.ToDouble(e.Row.Cells[i].Value).ToString("N0");
                          }
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

        protected void UltraWebGrid4_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;
            e.Layout.AllowColumnMovingDefault = AllowColumnMoving.None;
            e.Layout.AllowColSizingDefault = AllowSizing.Fixed;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            e.Layout.Bands[0].Columns[0].Width = CRHelper.GetColumnWidth(300);
            e.Layout.Bands[0].Columns[0].CellStyle.HorizontalAlign = HorizontalAlign.Left;
            e.Layout.Bands[0].Columns[0].CellStyle.Wrap = true;

            headerLayout4.AddCell("Наименование показателей");

            int numMonth = CRHelper.MonthNum(ComboMonth.SelectedValue) + 1;

            GridHeaderCell cell0 = headerLayout4.AddCell(string.Format("{0}", year - 1));
            cell0.AddCell("Исполнение год");
            cell0.AddCell(string.Format("Исполнено с начала года на 1 {0}", CRHelper.RusMonthGenitive(numMonth)));
            GridHeaderCell cell1 = headerLayout4.AddCell(string.Format("{0}", year));
            cell1.AddCell(string.Format("Исполнено с начала года на 1 {0}", CRHelper.RusMonthGenitive(numMonth)));
            
            headerLayout4.ApplyHeaderInfo();

            for (int i = 1; i < UltraWebGrid4.Columns.Count; i++)
            {
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "P0");
                e.Layout.Bands[0].Columns[i].Width = CRHelper.GetColumnWidth(100);
            }
        }

        protected void UltraWebGrid4_InitializeRow(object sender, RowEventArgs e)
        {

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

            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                if (e.Row.Index == 0)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {

                        double currentValue = Convert.ToDouble(e.Row.Cells[i].Value);

                        if (currentValue * 100 > 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                            e.Row.Cells[i].Title = "Превышение установленного норматива";
                        }
                        else if (currentValue * 100 <= 100)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            e.Row.Cells[i].Title = "Соблюдение установленного норматива";
                        }

                    }
                    e.Row.Cells[i].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                else
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                    {

                        double currentValue = Convert.ToDouble(e.Row.Cells[i].Value);

                        if (currentValue * 100 > 15)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballRedBB.png";
                            e.Row.Cells[i].Title = "Превышение установленного норматива";
                        }
                        else if (currentValue * 100 <= 15)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/ballGreenBB.png";
                            e.Row.Cells[i].Title = "Соблюдение установленного норматива";
                        }

                    }
                    e.Row.Cells[i].Style.CustomRules =
                        "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
            }
        }
        
        #endregion

        #region Экспорт

        #region Экспорт в Excel

        private static void ExcelExporter_BeginExport(object sender, BeginExportEventArgs e)
        {
            e.CurrentWorksheet.PrintOptions.Orientation = Infragistics.Documents.Excel.Orientation.Landscape;
            e.CurrentWorksheet.PrintOptions.PaperSize = PaperSize.A4;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.MaxPagesHorizontally = 1;
            e.CurrentWorksheet.PrintOptions.ScalingFactor = 40;
            e.CurrentWorksheet.PrintOptions.BottomMargin = 0;
            e.CurrentWorksheet.PrintOptions.TopMargin = 0;
            e.CurrentWorksheet.PrintOptions.LeftMargin = 0;
            e.CurrentWorksheet.PrintOptions.RightMargin = 0;
        }

        private void ExcelExportButton_Click(object sender, EventArgs e)
        {
            Workbook workbook = new Workbook();
            Worksheet sheet1 = workbook.Worksheets.Add("sheet1");
            Worksheet sheet2 = workbook.Worksheets.Add("sheet2");
            Worksheet sheet3 = workbook.Worksheets.Add("sheet3");
            Worksheet sheet4 = workbook.Worksheets.Add("sheet4");

            ReportExcelExporter1.WorksheetTitle = PageTitle.Text;
            ReportExcelExporter1.WorksheetSubTitle = PageSubTitle.Text;

            ReportExcelExporter1.RowsAutoFitEnable = true;

            ReportExcelExporter1.HeaderCellHeight = 30;
            ReportExcelExporter1.GridColumnWidthScale = 1.2;

            ReportExcelExporter1.Export(headerLayout1,table1Captions.Text, sheet1, 3);
            ReportExcelExporter1.Export(headerLayout2,table2Captions.Text, sheet2, 3);
            ReportExcelExporter1.Export(headerLayout3,table3Captions.Text, sheet3, 3);
            if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта")
            {
                ReportExcelExporter1.Export(headerLayout4,table4Captions.Text, sheet4, 3);
            }
            
        }

        #endregion

        #region Экспорт в Pdf

        void PdfExporter_EndExport(object sender, Infragistics.WebUI.UltraWebGrid.DocumentExport.EndExportEventArgs e)
        {
            e.Section.AddPageBreak();

            IText title = e.Section.AddText();
            Font font = new Font("Verdana", 14);
            title.Style.Font = new Infragistics.Documents.Reports.Graphics.Font(font);
            title.Style.Font.Bold = true;
        }

        private void PdfExportButton_Click(object sender, EventArgs e)
        {
            ReportPDFExporter1.PageTitle = PageTitle.Text;
            ReportPDFExporter1.PageSubTitle = PageSubTitle.Text;

            Report report = new Report();
            ISection section1 = report.AddSection();
            ISection section2 = report.AddSection();
            ISection section3 = report.AddSection();
           
            ReportPDFExporter1.HeaderCellHeight = 60;
            ReportPDFExporter1.Export(headerLayout1, table1Captions.Text, section1);
            ReportPDFExporter1.Export(headerLayout2, table2Captions.Text, section2);
            ReportPDFExporter1.Export(headerLayout3, table3Captions.Text, section3);

            if (ComboBudget.SelectedValue == "Консолидированный бюджет субъекта")
            {
              ISection section4 = report.AddSection();
              ReportPDFExporter1.Export(headerLayout4, table4Captions.Text, section4);
            }

        }

        #endregion

        #endregion
    }

}