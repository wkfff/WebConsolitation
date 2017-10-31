using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using Infragistics.Documents.Reports.Report;
using Infragistics.Documents.Reports.Report.Section;
using Infragistics.Documents.Reports.Report.Text;
using Infragistics.Documents.Excel;
using Infragistics.WebUI.UltraWebGrid;
using Infragistics.WebUI.UltraWebGrid.ExcelExport;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.FNS_0001_0005
{
    public partial class Default : CustomReportPage
    {
        private DataTable dtDate = new DataTable();
        private DataTable dtGrid = new DataTable();
        private int firstYear = 2000;
        private int endYear = 2011;
        private string month = "Январь";
        private DateTime currentDate;
        private int columnCount;
        private int columnNum;
        private Collection<string> selectedValues;
        private Collection<int> Max = new Collection<int>();
        private Collection<int> Min = new Collection<int>();
        private int max;
        private int min;

        private CustomParam LevelBudget;
        private CustomParam selectedDebit;

        private GridHeaderLayout headerLayout;

        protected override void Page_PreLoad(object sender, EventArgs e)
        {
            base.Page_PreLoad(sender, e);

            UltraWebGrid.Height = CRHelper.GetGridHeight(CustomReportConst.minScreenHeight / 1.4);
            UltraWebGrid.Width = CRHelper.GetGridWidth(CustomReportConst.minScreenWidth - 5);
            UltraWebGrid.DisplayLayout.NoDataMessage = "Нет данных";


            #region Инициализация параметров
            if (LevelBudget == null)
            {
                LevelBudget = UserParams.CustomParam("level_budget");
            }
            if (selectedDebit == null)
            {
                selectedDebit = UserParams.CustomParam("selected_Debit");
            }

            #endregion


            ReportExcelExporter1.ExcelExportButton.Click += new EventHandler(ExcelExportButton_Click);
            ReportPDFExporter1.PdfExportButton.Click += new EventHandler(PdfExportButton_Click);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            UserParams.PeriodDimension.Value = RegionSettingsHelper.Instance.PeriodDimension;

            if (!Page.IsPostBack)
            {
                dtDate = new DataTable();
                string query = DataProvider.GetQueryText("FNS_0001_0005_date");
                DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForPivotTable(query, dtDate);
                if (dtDate != null && dtDate.Rows.Count > 0)
                {
                    endYear = Convert.ToInt32(dtDate.Rows[0][0]);
                    month = dtDate.Rows[0][3].ToString();
                }

                UserParams.PeriodYear.Value = endYear.ToString();
                UserParams.PeriodMonth.Value = month;

                ComboYear.Title = "Год";
                ComboYear.Width = 100;
                ComboYear.MultiSelect = false;
                ComboYear.FillDictionaryValues(CustomMultiComboDataHelper.FillYearValues(firstYear, endYear));
                ComboYear.SetСheckedState(UserParams.PeriodYear.Value, true);

                ComboMonth.Title = "Месяц";
                ComboMonth.Width = 130;
                ComboMonth.MultiSelect = false;
                ComboMonth.FillDictionaryValues(CustomMultiComboDataHelper.FillMonthValues());
                ComboMonth.SetСheckedState(UserParams.PeriodMonth.Value, true);

                FillComboBudget();
                ComboBudget.Title = "Уровень бюджета";
                ComboBudget.Width = 400;
                ComboBudget.ParentSelect = true;
                ComboBudget.SetСheckedState("Консолидированный бюджет субъекта", true);

                ComboDebit.Title = "Вид дохода";
                ComboDebit.MultiSelect = true;
                ComboDebit.MultipleSelectionType = MultipleSelectionType.SimpleMultiple;
                ComboDebit.Width = 400;
                
                FillComboDebit();

                ComboDebit.SetСheckedState("Налоговые доходы",true);
                ComboDebit.SetСheckedState("Неналоговые доходы", true);
             

            }

            int monthNum = ComboMonth.SelectedIndex + 1;
            int yearNum = Convert.ToInt32(ComboYear.SelectedValue);

            currentDate = new DateTime(Convert.ToInt32(ComboYear.SelectedValue), CRHelper.MonthNum(ComboMonth.SelectedValue), 1);
            currentDate = currentDate.AddMonths(1);

            Page.Title = string.Format("Информация о поступлении налоговых и неналоговых доходов в {0} по территории МР (по 65н)", ComboBudget.SelectedValue.ToLower());
            PageTitle.Text = Page.Title;
            PageSubTitle.Text = string.Format("за {0} {1} {2} года", monthNum, CRHelper.RusManyMonthGenitive(monthNum), yearNum);

            UserParams.PeriodYear.Value = yearNum.ToString();
            UserParams.PeriodMonth.Value = ComboMonth.SelectedValue;
            UserParams.PeriodHalfYear.Value = string.Format("Полугодие {0}", CRHelper.HalfYearNumByMonthNum(ComboMonth.SelectedIndex + 1));
            UserParams.PeriodQuater.Value = string.Format("Квартал {0}", CRHelper.QuarterNumByMonthNum(ComboMonth.SelectedIndex + 1));
            
            LevelBudget.Value = "[Уровни бюджетов].[Все].[Все уровни].[Конс.бюджет субъекта]";

            switch (ComboBudget.SelectedValue)
            {
              case "Консолидированный бюджет субъекта":
                    {
                       LevelBudget.Value = "[Уровни бюджетов].[Все].[Все уровни].[Конс.бюджет субъекта]";
                        break;
                    }
                case "Бюджет субъекта":
                    {
                        LevelBudget.Value = "[Уровни бюджетов].[Все].[Все уровни].[Конс.бюджет субъекта].[Бюджет субъекта]";
                        break;
                    } 
                case "Консолидированный бюджет МО":
                    {
                        LevelBudget.Value = "[Уровни бюджетов].[Все].[Все уровни].[Конс.бюджет субъекта].[Конс.бюджет МО]";
                        break;
                    }
                case "Бюджет ГО":
                    {
                        LevelBudget.Value = "[Уровни бюджетов].[Все].[Все уровни].[Конс.бюджет субъекта].[Конс.бюджет МО].[Бюджет ГО]";
                        break;
                    }
                case "Консолидированный бюджет МР":
                    {
                        LevelBudget.Value = "[Уровни бюджетов].[Все].[Все уровни].[Конс.бюджет субъекта].[Конс.бюджет МО].[Конс.бюджет МР]";
                        break;
                    }
                case "Бюджет района":
                    {
                        LevelBudget.Value = "[Уровни бюджетов].[Все].[Все уровни].[Конс.бюджет субъекта].[Конс.бюджет МО].[Конс.бюджет МР].[Бюджет района]";
                        break;
                    }
                case "Бюджет поселения":
                    {
                        LevelBudget.Value = "[Уровни бюджетов].[Все].[Все уровни].[Конс.бюджет субъекта].[Конс.бюджет МО].[Конс.бюджет МР].[Бюджет поселения]";
                        break;
                    }

            }

            selectedDebit.Value ="{[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА ПРИБЫЛЬ, ДОХОДЫ] }*{Measures.[Неналоговые доходы_С начала года],Measures.[Неналоговые доходы_Темп роста],Measures.[Неналоговые доходы_Ранг поселения]}  + {[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА ПРИБЫЛЬ, ДОХОДЫ]}*{Measures.[Налоговые доходы_С начала года],Measures.[Налоговые доходы_Темп роста],Measures.[Налоговые доходы_Ранг поселения]}  ";
            
            string debits = string.Empty;
            selectedValues = ComboDebit.SelectedValues;
            if (selectedValues.Count > 0)
            {
              for (int i = 0; i < selectedValues.Count; i++)
                {
                    string debit = selectedValues[i];
                    switch (debit)
                    {
                        case "Налоговые доходы":
                            {
                                debits += "[КД].[Сопоставимый].[Налоговые доходы],";  
                                break;
                            }

                        case "Налоги на прибыль, доходы":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА ПРИБЫЛЬ, ДОХОДЫ],";
                                break;
                            }

                        case "Налог на прибыль":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА ПРИБЫЛЬ, ДОХОДЫ].[Налог на прибыль организаций],";
                                break;
                            }

                        case "НДФЛ":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА ПРИБЫЛЬ, ДОХОДЫ].[Налог на доходы физических лиц],";
                                break;
                            }

                        case "Налоги на совокупный доход":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА СОВОКУПНЫЙ ДОХОД],";
                                break;
                            }

                        case "УСН":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА СОВОКУПНЫЙ ДОХОД].[Налог, взимаемый в связи с применением упрощенной системы налогообложения],";
                                break;
                            }

                        case "ЕНВД":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА СОВОКУПНЫЙ ДОХОД].[Единый налог на вмененный доход для отдельных видов деятельности],";
                                break;
                            }

                        case "ЕСХН":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА СОВОКУПНЫЙ ДОХОД].[Единый сельскохозяйственный налог],";
                                break;
                            }
                        case "Акцизы":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Акцизы],";
                                break;
                            }
                        case "Налоги на имущество":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА ИМУЩЕСТВО],";
                                break;
                            }
                        case "Налог на имущество физ. лиц":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА ИМУЩЕСТВО].[Налог на имущество физических лиц],";
                                break;
                            }
                        case "Налог на имущество организаций":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА ИМУЩЕСТВО].[Налог на имущество организаций],";
                                break;
                            }
                        case "Транспортный налог с организации":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА ИМУЩЕСТВО].[Транспортный налог].[Транспортный налог с организаций],";
                                break;
                            }
                        case "Транспортный налог с физ. лиц":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА ИМУЩЕСТВО].[Транспортный налог].[Транспортный налог с физических лиц],";
                                break;
                            }
                        case "Налог на игорный бизнес":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА ИМУЩЕСТВО].[Налог на игорный бизнес],";
                                break;
                            }
                        case "Земельный налог":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ НА ИМУЩЕСТВО].[Земельный налог],";
                                break;
                            }
                        case "Налоги, сборы за пользование природными ресурсами":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ, СБОРЫ И РЕГУЛЯРНЫЕ ПЛАТЕЖИ ЗА ПОЛЬЗОВАНИЕ ПРИРОДНЫМИ РЕСУРСАМИ],";
                                break;
                            }

                        case "НДПИ":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ, СБОРЫ И РЕГУЛЯРНЫЕ ПЛАТЕЖИ ЗА ПОЛЬЗОВАНИЕ ПРИРОДНЫМИ РЕСУРСАМИ].[Налог на добычу полезных ископаемых],";
                                break;
                            }

                        case "Сборы за пользование объектами животного мира":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[НАЛОГИ, СБОРЫ И РЕГУЛЯРНЫЕ ПЛАТЕЖИ ЗА ПОЛЬЗОВАНИЕ ПРИРОДНЫМИ РЕСУРСАМИ].[Сборы за пользование объектами животного мира и за пользование объектами водных биологических ресурсов],";
                                break;
                            }

                        case "Гос. пошлина":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[ГОСУДАРСТВЕННАЯ ПОШЛИНА],";
                                break;
                            }

                        case "Задолженность по отмененным налогам":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[ЗАДОЛЖЕННОСТЬ И ПЕРЕРАСЧЕТЫ ПО ОТМЕНЕННЫМ НАЛОГАМ, СБОРАМ И ИНЫМ ОБЯЗАТЕЛЬНЫМ ПЛАТЕЖАМ],";
                                break;
                            }

                        case "Неналоговые доходы":
                            {
                                debits += "[КД].[Сопоставимый].[Неналоговые доходы],";
                                break;
                            }

                        case "Штрафы":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[ШТРАФЫ, САНКЦИИ, ВОЗМЕЩЕНИЕ УЩЕРБА],";
                                break;
                            }

                        case "Платежи при пользовании недрами":
                            {
                                debits += "[КД].[Сопоставимый].[Все коды доходов].[Всего доходов].[НАЛОГОВЫЕ И НЕНАЛОГОВЫЕ ДОХОДЫ].[ПЛАТЕЖИ ПРИ ПОЛЬЗОВАНИИ ПРИРОДНЫМИ РЕСУРСАМИ].[Платежи при пользовании недрами],";
                                break;
                            }
                    }
                }
            }
            selectedDebit.Value = debits.TrimEnd(',');

            headerLayout = new GridHeaderLayout(UltraWebGrid);
            UltraWebGrid.Bands.Clear();
            UltraWebGrid.DataBind();
        }

        private void FillComboBudget()
        {
            Dictionary<string, int> levels = new Dictionary<string, int>();
            levels.Add("Консолидированный бюджет субъекта", 0);
            levels.Add("Бюджет субъекта", 1);
            levels.Add("Консолидированный бюджет МО",1);
            levels.Add("Бюджет ГО", 2);
            levels.Add("Консолидированный бюджет МР", 2);
            levels.Add("Бюджет района", 3);
            levels.Add("Бюджет поселения", 3);
           
            ComboBudget.FillDictionaryValues(levels);
        }

       private void FillComboDebit()
        {
            Dictionary<string,int> debit = new Dictionary<string, int>();
            debit.Add("Налоговые доходы",0);
            debit.Add("Налоги на прибыль, доходы", 1);
            debit.Add("Налог на прибыль", 2);
            debit.Add("НДФЛ", 2);
            debit.Add("Налоги на совокупный доход", 1);
            debit.Add("УСН", 2);
            debit.Add("ЕНВД", 2);
            debit.Add("ЕСХН", 2);
            debit.Add("Акцизы", 1);
            debit.Add("Налоги на имущество", 1);
            debit.Add("Налог на имущество физ. лиц", 2);
            debit.Add("Налог на имущество организаций", 2);
            debit.Add("Транспортный налог с организации", 2);
            debit.Add("Транспортный налог с физ. лиц", 2);
            debit.Add("Налог на игорный бизнес", 2);
            debit.Add("Земельный налог", 2);
            debit.Add("Налоги, сборы за пользование природными ресурсами", 1);
            debit.Add("НДПИ", 2);
            debit.Add("Сборы за пользование объектами животного мира", 2);
            debit.Add("Гос. пошлина", 1);
            debit.Add("Задолженность по отмененным налогам", 1);
            debit.Add("Неналоговые доходы",0);
            debit.Add("Штрафы", 1);
            debit.Add("Платежи при пользовании недрами", 1);

            ComboDebit.FillDictionaryValues(debit); 
        }
        
        #region Обработчики грида

        protected void UltraWebGrid_DataBinding(object sender, EventArgs e)
        {
            string query = DataProvider.GetQueryText("FNS_0001_0005_grid1");
            DataProvidersFactory.PrimaryMASDataProvider.GetDataTableForChart(query, "Код", dtGrid);

            if (dtGrid.Rows.Count > 0)
            {
                if (dtGrid.Columns.Count > 1)
                {
                    dtGrid.Columns.RemoveAt(0);
                }

                foreach (DataRow row in dtGrid.Rows)
                {
                    if (row[1] != DBNull.Value)
                    {
                        row[1] = row[1].ToString().Replace("сельское поселение", "СП");
                        row[1] = row[1].ToString().Replace("городское поселение", "ГП");
                        row[1] = row[1].ToString().Replace("муниципальное образование", "МО");
                        row[1] = row[1].ToString().Replace("муниципальный район", "МР");
                        row[1] = row[1].ToString().Replace("муниципального района", "МР");
                    }
                }

                UltraWebGrid.DataSource = dtGrid;

                columnCount = selectedValues.Count * 3;
               
                    /*for (int numCol=4; numCol<columnCount; numCol+=3)
                    {
                        max = Int32.MinValue;
                        min = Int32.MaxValue;

                        for (int numRow = 0; numRow < dtGrid.Rows.Count; numRow++)
                        {
                            if (dtGrid.Rows[numRow][numCol] != DBNull.Value && dtGrid.Rows[numRow][numCol].ToString() != string.Empty)
                            {
                                int value = Convert.ToInt32(dtGrid.Rows[numRow][numCol]);
                                if (value > max)
                                {

                                    max = value;
                                }
                                if (value < min)
                                {
                                    min = value;
                                }
                            }
                        }
                        Max.Add(max);
                        Min.Add(min);

                    }*/
                
            }
        }

        protected void UltraWebGrid_InitializeLayout(object sender, LayoutEventArgs e)
        {
            e.Layout.GroupByBox.Hidden = true;
            e.Layout.HeaderStyleDefault.Wrap = true;
            e.Layout.HeaderStyleDefault.HorizontalAlign = HorizontalAlign.Center;

            if (e.Layout.Bands[0].Columns.Count == 0)
            {
                return;
            }
            int i;
          
            e.Layout.Bands[0].Columns[1].CellStyle.Wrap = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 2].Hidden = true;
            e.Layout.Bands[0].Columns[e.Layout.Bands[0].Columns.Count - 1].Hidden = true;
            e.Layout.Bands[0].Columns[0].Width = 60;
            e.Layout.Bands[0].Columns[1].Width = 195;
           
            columnCount = selectedValues.Count * 5; // количество видов дохода * н

            headerLayout.AddCell("Код");
            headerLayout.AddCell("Наименование");
            
            for (i = 2; i < columnCount; i+=5 ) // вкдючая всего налоговых и неналоговых доходов
            {
                string[] captions = e.Layout.Bands[0].Columns[i].Header.Caption.Split(';');
                GridHeaderCell cell0 = headerLayout.AddCell(captions[0]);
                cell0.AddCell(string.Format("На <br/> {0:dd.MM.yyyy} г. ", currentDate), string.Format("Данные за {0} месяцев {1} года, тыс. руб.", CRHelper.MonthNum(ComboMonth.SelectedValue), ComboYear.SelectedValue));
                cell0.AddCell("Темп роста", "Темп роста к аналогичному периоду предыдущего года, %");
                cell0.AddCell("Ранг поселения", "Ранг (место) поселений по темпу роста доходов в целом по области с начала года");
                cell0.AddCell("Удельный вес доходов в целом по области", "Удельный вес отдельного доходного источника в целом по области, %");
                cell0.AddCell("Удельный вес доходов в отдельном МР",
                              "Удельный вес отдельного доходного источника в итоге по поселениям и бюджету МР в отдельном МР, %");

                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i], "N2");
                e.Layout.Bands[0].Columns[i].Width = 110;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i+1], "P2");
                e.Layout.Bands[0].Columns[i+1].Width = 80;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i+2], "N0");
                e.Layout.Bands[0].Columns[i + 2].Width = 70;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i+3], "P4");
                e.Layout.Bands[0].Columns[i+3].Width = 90;
                CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[i+4], "P2");
                e.Layout.Bands[0].Columns[i+4].Width = 90;
            }
            
             columnNum = i;
            
           
             CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[columnNum],"N2");
             e.Layout.Bands[0].Columns[columnNum].Width = 90;
             CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[columnNum + 1],"P2");
             e.Layout.Bands[0].Columns[columnNum+1].Width = 100;
             CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[columnNum + 2],"N2");
             e.Layout.Bands[0].Columns[columnNum+2].Width = 100;
             CRHelper.FormatNumberColumn(e.Layout.Bands[0].Columns[columnNum + 3],"N0");
             e.Layout.Bands[0].Columns[columnNum+3].Width = 70;

             
             for (int j=2; j<e.Layout.Grid.Columns.Count-1 ; j++)
             {
               e.Layout.Bands[0].Columns[j].CellStyle.HorizontalAlign = HorizontalAlign.Right;
             }

             headerLayout.AddCell("Численность постоянного населения", "Численность постоянного населения городских и сельских поселений, чел.");
             headerLayout.AddCell("Удельный вес населения в отдельном МР", "Удельный вес населения в итоге по поселениям в отдельном муниципальном районе, %");
             headerLayout.AddCell("Среднедушевые доходы, руб./чел.", "Сумма налоговых доходов бюджетов поселений на душу населения руб./ чел.");
             headerLayout.AddCell("Ранг поселения", "Ранг (место) поселений по среднедушевым доходам в целом по области");
             headerLayout.ApplyHeaderInfo();
        }

        protected void UltraWebGrid_InitializeRow(object sender, RowEventArgs e)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                int levelIndex = dtGrid.Columns.Count-1;
                bool rank = (i == columnNum + 3);

                if (e.Row.Cells[levelIndex] != null && e.Row.Cells[levelIndex].Value.ToString() != string.Empty)
                {
                    string level = e.Row.Cells[levelIndex].Value.ToString();
                    int fontSize = 8;
                    bool bold = false;
                    bool italic = false;
                    switch (level)
                    {
                        case "(All)":
                            {
                                fontSize = 10;
                                bold = true;
                                italic = false;
                                break;
                            }
                        case "Уровень 04":
                            {
                                fontSize = 9;
                                bold = true;
                                italic = false;
                                break;
                            }
                        case "Уровень 05":
                            {
                                fontSize = 8;
                                bold = false;
                                italic = false;
                                break;
                            }
                    }
                    e.Row.Cells[i].Style.Font.Size = fontSize;
                    e.Row.Cells[i].Style.Font.Bold = bold;
                    e.Row.Cells[i].Style.Font.Italic = italic;
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center";
                }

                
               if (rank)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null &&
                        e.Row.Cells[i].Value.ToString() != string.Empty &&
                        e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToInt32(e.Row.Cells[i].Value) == 1)
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("Самый высокий доход я в поселениях");
                        }
                         if (Convert.ToInt32(e.Row.Cells[i].Value) == Convert.ToInt32(e.Row.Cells[i + 1].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = string.Format("Самый низкий доход на душу населения в поселениях");
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
              /*  if (i== columnNum ) 
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null &&
                       e.Row.Cells[i].Value.ToString() != string.Empty &&
                       e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToDouble(e.Row.Cells[i].Value) == Convert.ToDouble(e.Row.Cells[i + 9].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("Наибольший удельный вес отдельного доходного источника");
                        }
                        else if (Convert.ToDouble(e.Row.Cells[i].Value) == Convert.ToDouble(e.Row.Cells[i + 8].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = string.Format("Наименьший удельный вес отдельного доходного источника");
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }

                if (i == columnNum + 1)
                {
                    if (e.Row.Cells[i].Value != null && e.Row.Cells[i + 1].Value != null &&
                       e.Row.Cells[i].Value.ToString() != string.Empty &&
                       e.Row.Cells[i + 1].Value.ToString() != string.Empty)
                    {
                        if (Convert.ToDouble(e.Row.Cells[i].Value)  == Convert.ToDouble(e.Row.Cells[i + 6].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                            e.Row.Cells[i].Title = string.Format("Наибольший удельный вес отдельного доходного источника");
                        }
                        else if (Convert.ToDouble(e.Row.Cells[i].Value) == Convert.ToDouble(e.Row.Cells[i + 5].Value))
                        {
                            e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                            e.Row.Cells[i].Title = string.Format("Наименьший удельный вес отдельного доходного источника");
                        }
                    }
                    e.Row.Cells[i].Style.CustomRules = "background-repeat: no-repeat; background-position: left center; margin: 2px";
                }
                */
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

           for (int i=3; i<columnNum; i+=5)
           {
               if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
               {
                   if (Convert.ToDouble(e.Row.Cells[i].Value) < 1)
                   {
                       e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowRedDownBB.png";
                       e.Row.Cells[i].Title = "Снижение к прошлому отчетному году";
                   }
                   else if (Convert.ToDouble(e.Row.Cells[i].Value) > 1)
                   {
                       e.Row.Cells[i].Style.BackgroundImage = "~/images/arrowGreenUpBB.png";
                       e.Row.Cells[i].Title = "Рост к прошлому отчетному году";
                   }
                   e.Row.Cells[i].Style.CustomRules =
                       "background-repeat: no-repeat; background-position: left center; margin: 2px";
               }
           }

        /*   int k = 0; 
           for (int i = 4; i < columnNum; i+=3) //Ранг поселения (по темпу роста доходов)
           {
                 if (e.Row.Cells[i].Value != null && e.Row.Cells[i].Value.ToString() != string.Empty)
                   {
                       if (Convert.ToInt32(e.Row.Cells[i].Value) == Min[k])
                       {
                           e.Row.Cells[i].Style.BackgroundImage = "~/images/starYellowBB.png";
                           e.Row.Cells[i].Title = "Самый высокий темп роста доходов среди поселений";
                       }
                       else if (Convert.ToInt32(e.Row.Cells[i].Value) == Max[k])
                       {
                           e.Row.Cells[i].Style.BackgroundImage = "~/images/starGrayBB.png";
                           e.Row.Cells[i].Title = "Самый низкий темп роста доходов среди поселений";
                       }
                       e.Row.Cells[i].Style.CustomRules =
                           "background-repeat: no-repeat; background-position: left center; margin: 2px";
                   }

                   k++;
               }
            */
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
