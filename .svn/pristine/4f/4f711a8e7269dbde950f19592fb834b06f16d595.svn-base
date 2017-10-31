using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps.BudgetDataPump
{
	/// <summary>
	/// Главный класс закачки данных бюджета. Управляет закачками из отдельных блоков.
	/// </summary>
    public partial class BudgetDataPumpModule : BudgetPumpModuleBase
    {
        #region Делегаты

        public delegate void ProcessBudgetDataDelegate(ref int addedRowsCount, ref int updatedRowsCount,
            ref int processedRecCount, ref string skippedRecsMessage);

        public delegate void ProcessClsRowDelegate(DataRow budgetRow, DataRow clsRow, string[] fieldsMapping);

        #endregion Делегаты

        #region Поля

        #region классификаторы

        // КД.АС Бюджет (d_KD_ASBudget)
        private IDbDataAdapter daKD;
        private DataSet dsKD;
        private IClassifier clsKD;
        private Dictionary<int, DataRow> kdCache = null;
        private int nullKD;
        // Виды доходов (d.IncomesKinds.Budget)
        private IDbDataAdapter daIncomesKinds;
        private DataSet dsIncomesKinds;
        private IClassifier clsIncomesKinds;
        private Dictionary<string, DataRow> incomesKindsCache = null;
        private int nullIncomesKinds;
        // КВСР (d.KVSR.Budget)
        private IDbDataAdapter daKVSR;
        private DataSet dsKVSR;
        private IClassifier clsKVSR;
        private Dictionary<int, DataRow> kvsrCache = null;
        private int nullKVSR;
        // ЭКД (d.EKD.Budget)
        private IDbDataAdapter daEKD;
        private DataSet dsEKD;
        private IClassifier clsEKD;
        private int nullEKD;
        // Программы (d.Programs.Budget)
        private IDbDataAdapter daPrograms;
        private DataSet dsPrograms;
        private IClassifier clsPrograms;
        private int nullPrograms;
        // Районы (d.Regions.Budget)
        private IDbDataAdapter daRegions;
        private DataSet dsRegions;
        private IClassifier clsRegions;
        private int nullRegions;
        private Dictionary<int, DataRow> regionsCache = null;
        // Слои доходов (d.BudgetAccounts.Budget)
        private IDbDataAdapter daBudgetAccounts;
        private DataSet dsBudgetAccounts;
        private IClassifier clsBudgetAccounts;
        private int nullBudgetAccounts;
        // Тип средств (d.MeansType.Budget)
        private IDbDataAdapter daMeansType;
        private DataSet dsMeansType;
        private IClassifier clsMeansType;
        private Dictionary<int, DataRow> meansTypeCache = null;
        private int nullMeansType;
        // ЭКР.АС Бюджет (d_EKR_ASBudget)
        private IDbDataAdapter daEKR;
        private DataSet dsEKR;
        private IClassifier clsEKR;
        private Dictionary<int, DataRow> ekrCache = null;
        private int nullEKR;
        // ФКР.АС Бюджет (d_FKR_ASBudget)
        private IDbDataAdapter daFKR;
        private DataSet dsFKR;
        private IClassifier clsFKR;
        private Dictionary<int, DataRow> fkrCache = null;
        private int nullFKR;
        // КЦСР.АС Бюджет (d_KCSR_ASBudget)
        private IDbDataAdapter daKCSR;
        private DataSet dsKCSR;
        private IClassifier clsKCSR;
        private Dictionary<int, DataRow> kcsrCache = null;
        private int nullKCSR;
        // виды расходов (d.KVR.Budget)
        private IDbDataAdapter daKVR;
        private DataSet dsKVR;
        private IClassifier clsKVR;
        private Dictionary<int, DataRow> kvrCache = null;
        private int nullKVR;
        // ТипУвед (d.NotifyTypes.Budget)
        private IDbDataAdapter daNotifyTypes;
        private DataSet dsNotifyTypes;
        private IClassifier clsNotifyTypes;
        private int nullNotifyTypes;
        // СубЭКР.АС Бюджет (d_SubKESR_Budget)
        private IDbDataAdapter daSubKESR;
        private DataSet dsSubKESR;
        private IClassifier clsSubKESR;
        private Dictionary<int, DataRow> subEkrCache = null;
        private int nullSubKESR;
        // Мероприятия (d.Fact.Budget)
        private IDbDataAdapter daFact;
        private DataSet dsFact;
        private IClassifier clsFact;
        private Dictionary<int, DataRow> factClsCache = null;
        private int nullFact;
        // ЛицСчета (d.FacialAcc.Budget)
        private IDbDataAdapter daFacialAcc;
        private DataSet dsFacialAcc;
        private IClassifier clsFacialAcc;
        private Dictionary<int, DataRow> facialAccCache = null;
        private int nullFacialAcc;
        // Направление (d.Direction.Budget)
        private IDbDataAdapter daDirection;
        private DataSet dsDirection;
        private IClassifier clsDirection;
        private Dictionary<int, DataRow> directionCache = null;
        private int nullDirection;
        // Источники внутреннего финансирования 2004 (d.KIVF.Budget2004)
        private IDbDataAdapter daKIF2004;
        private DataSet dsKIF2004;
        private IClassifier clsKIF2004;
        private Dictionary<int, DataRow> ifsCache = null;
        private int nullKIF2004;
        // Источники внутреннего финансирования 2005 (d.KIVF.Budget2005)
        private IDbDataAdapter daKIF2005;
        private DataSet dsKIF2005;
        private IClassifier clsKIF2005;
        private int nullKIF2005;
        // Бюджет.АС Бюджет (d.Budget.Budget)
        private IDbDataAdapter daBudgetBudget;
        private DataSet dsBudgetBudget;
        private IClassifier clsBudgetBudget;
        private Dictionary<int, DataRow> budgetBudgetCache = null;
        private int nullBudgetBudget;
        // FundsSource.АС Бюджет (d.FundsSource.Budget)
        private IDbDataAdapter daFundsSource;
        private DataSet dsFundsSource;
        private IClassifier clsFundsSource;
        private Dictionary<int, DataRow> fundsSourceCache = null;
        private int nullFundsSource;
        // AsgmtKind.АС Бюджет (d.AsgmtKind.Budget)
        private IDbDataAdapter daAsgmtKind;
        private DataSet dsAsgmtKind;
        private IClassifier clsAsgmtKind;
        private Dictionary<int, DataRow> asgmtKindCache = null;
        private int nullAsgmtKind;
        // AsgmtSrce.АС Бюджет (d.AsgmtSrce.Budget)
        private IDbDataAdapter daAsgmtSrce;
        private DataSet dsAsgmtSrce;
        private IClassifier clsAsgmtSrce;
        private Dictionary<int, DataRow> asgmtSrceCache = null;
        private int nullAsgmtSrce;
        // БухОперации.АС Бюджет (d.BuhOperations.Budget)
        private IDbDataAdapter daBuhOperations;
        private DataSet dsBuhOperations;
        private IClassifier clsBuhOperations;
        private Dictionary<int, DataRow> buhOperationsCache = null;
        private int nullBuhOperations;
        // Расходы.АС Бюджет (d.R.Budget)
        private IDbDataAdapter daBudgetOutcomesCls;
        private DataSet dsBudgetOutcomesCls;
        private IClassifier clsBudgetOutcomesCls;
        private Dictionary<string, DataRow> budgetOutcomesClsCache = null;
        private int nullBudgetOutcomesCls;
        private int errorBudgetOutcomesCls;
        // PlanKind.АС Бюджет (d.PlanKind.Budget)
        private IDbDataAdapter daPlanKind;
        private DataSet dsPlanKind;
        private IClassifier clsPlanKind;
        private Dictionary<int, DataRow> planKindCache = null;
        private int nullPlanKind;
        // ТипФинансир (d.FinType.Budget)
        private IDbDataAdapter daFinType;
        private DataSet dsFinType;
        private IClassifier clsFinType;
        private Dictionary<int, DataRow> finTypeCache = null;
        private int nullFinType;
        // Вид плана.АС Бюджет (d_PlanDocType_Budget)
        private IDbDataAdapter daPlanDocType;
        private DataSet dsPlanDocType;
        private IClassifier clsPlanDocType;
        private Dictionary<int, DataRow> planDocTypeCache = null;
        private int nullPlanDocType;
        // Код целевых средств.АС Бюджет (d_Transfert_Budget)
        private IDbDataAdapter daTransfert;
        private DataSet dsTransfert;
        private IClassifier clsTransfert;
        private Dictionary<int, DataRow> transfertCache = null;
        private int nullTransfert;
        // фиксированные 
        private IClassifier fxcProgIndexIncomes;
        private IClassifier fxcFederalFunds;
        private IClassifier fxcNotifType;
        private IClassifier fxcProgIndexFacial;
        private IClassifier fxcProgIndexFinDoc;
        private IClassifier fxcProgIndexBudget;

        #endregion классификаторы

        #region факты

        // ФО_АС Бюджет_Факт доходов (f_D_Incomes32)
        private IFactTable fctIncomes32;
        // ФО_АС Бюджет_План доходов (f_D_IncomesData)
        private IFactTable fctIncomesPlan;
        // ФО_АС Бюджет_План расходов (f_R_BudgetData)
        private IFactTable fctBudgetData;
        // ФО_АС Бюджет_КазнИсп_Расходы казначейство (f_R_FacialFinDetail)
        private IFactTable fctFacialFinDetail;
        // ФО_АС Бюджет_Операции со счетами (f_Buh_DFAccountOperation)
        private IFactTable fctAccountOperations;
        // ФО_АС Бюджет_Финансирование (f_R_FinDocDetail)
        private IFactTable fctFinDocDetail;
        // ИФ.ФО_АС Бюджет_План ИФ (f_S_BudDataSource)
        private IFactTable fctIFPlan;
        // ИФ.ФО_АС Бюджет_Факт ИФ (f_S_FDDSource)
        private IFactTable fctIFFact;

        #endregion факты

        private IDbDataAdapter daBudgetFacts;
        private DataSet dsBudgetFacts;
        private IDbDataAdapter daBudgetCls;
        // Кэш закачанных фактов
        private Dictionary<string, DataRow> factCache = null; 
        // Кэш закачанных классификаторов
        private Dictionary<string, DataRow> clsCache = null; 

        private int totalFiles = 0;
        private int filesCount = 0;
        private SortedList<int, int> badDateList = new SortedList<int, int>();
        private string currentBlockName;

        private bool toPumpIncomes = false;
        private bool toPumpIncomesPlan = false;
        private bool toPumpOutcomesPlan = false;
        private bool toPumpTreasury = false;
        private bool toPumpFinancing = false;
        private bool toPumpAccountOperations = false;
        private bool toPumpIFPlan = false;
        private bool toPumpIFFact = false;

        // Тип средств, находящихся во временном распоряжении - закачивать их не нужно (казначейство)
        private string tempMeansType = string.Empty;

        // хранит закачанные данные казначейства (база нашей системы)
        Dictionary<int, string> fmTreasuryCache = null;

        // тру - данный этап - проверка
        bool isCheckStage = false;

        // параметр обработки - формировать Расходы.АС Бюджет с КОСГУ или нет
        private bool withKosgu = false;

        #endregion Поля

        #region Структуры, перечисления

        private enum BudgetBlock
        {
            /// Доходы
            Incomes,
            /// План доходов
            IncomesPlan,
            /// План расходов
            OutcomesPlan,
            /// Казначейство
            Treasury,
            /// Финансирование
            Financing,
            /// Операции со счетами
            AccountOperations,
            // План ИФ
            IFPlan,
            // Факт ИФ
            IFFact
        }

        #endregion Структуры, перечисления

        #region Общие функции

        private int FindBudgetBudgetRef(string budgetName, string budgetYear)
        {
            object year = budgetYear;
            if (budgetYear == string.Empty)
                year = DBNull.Value;
            foreach (DataRow row in dsBudgetBudget.Tables[0].Rows)
                if (year == DBNull.Value)
                {
                    if ((row["Name"].ToString() == budgetName) && (row["AYear"] == year))
                        return Convert.ToInt32(row["Id"]);
                }
                else
                {
                    if ((row["Name"].ToString() == budgetName) && (Convert.ToInt32(row["AYear"]) == Convert.ToInt32(year)))
                        return Convert.ToInt32(row["Id"]);
                }
            return nullBudgetBudget;
        }

        /// <summary>
        /// Добавляет запись в список неверных дат
        /// </summary>
        /// <param name="key">Дата</param>
        private void AddToBadDateList(ref SortedList<int, int> badDateList, int key)
        {
            if (badDateList.ContainsKey(key))
            {
                badDateList[key]++;
            }
            else
            {
                badDateList.Add(key, 1);
            }
        }

        /// <summary>
        /// Записывает в протокол данные о неправильных датах
        /// </summary>
        /// <param name="badDateList">Список дат</param>
        private void RecordBadDateList()
        {
            if (badDateList.Count > 0)
            {
                string msg = string.Empty;

                foreach (int i in badDateList.Keys)
                {
                    msg = string.Format("{0}Дата: {1}, записей: {2};\n", msg, i, badDateList[i]);
                }

                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeWarning, string.Format(
                        "Пропущено записей по причине некорректных дат: \n{0}", msg.TrimEnd(';')));
            }
        }

        /// <summary>
        /// Очищает датасет с данными бюджета
        /// </summary>
        private void ClearBudgetFactDataSet()
        {
            if (dsBudgetFacts.Tables.Contains("Table"))
            {
                dsBudgetFacts.Tables["Table"].Clear();
            }
            while (dsBudgetFacts.Relations.Count > 0)
            {
                dsBudgetFacts.Relations.RemoveAt(0);
            }
        }

        /// <summary>
        /// Проверяет дату
        /// </summary>
        /// <param name="date">Дата</param>
        /// <returns>Дата корректна или нет</returns>
        private bool CheckDate(int date)
        {
            try
            {
                if (date % 100 == 0)
                {
                    return true;
                }
                int day = date % 100;
                if (day == 32)
                    return true;
                if (!CommonRoutines.CheckDaysInMonth(date))
                {
                    AddToBadDateList(ref badDateList, date);
                    return false;
                }
            }
            catch
            {
                AddToBadDateList(ref badDateList, date);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Проверяет поле даты в таблице бюджета на правильность
        /// </summary>
        /// <param name="constr">Ограничение</param>
        private void CheckBudgetTableDate(string tableName, string tablePrefix, string dateField, string constr)
        {
            string whereStr = string.Empty;
            if (constr == string.Empty)
            {
                whereStr = "where";
            }
            else
            {
                whereStr = "and";
            }

            // Узнаем количество записей с неправильной датой
            DataTable dt = (DataTable)this.BudgetDB.ExecQuery(string.Format(
                "select distinct cast({4}{0} as varchar(10)) {0} from {1} {5} {2} {3} not ({4}{0} between 20000101 and 20201217)",
                dateField, tableName, constr, whereStr, tablePrefix, tablePrefix.Trim('.')), 
                QueryResultTypes.DataTable);

            // Если таковые имеются, пишем в лог
            if (dt.Rows.Count > 0)
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeWarning, string.Format(
                        "В таблице данных АС Бюджет найдено {0} записей, содержащих некорректные даты: {1}. " +
                        "Данные записи будут исключены из выборки.",
                        dt.Rows.Count, CommonRoutines.DataTableColumnToString(dt, dateField)));
            }
        }

        /// <summary>
        /// Устанавливает значения полей лицевого счета и организаций
        /// </summary>
        /// <param name="budgetRow">Строка бюджета</param>
        /// <param name="factRow">Строка нашей таблицы фактов</param>
        private void SetOrgFields(DataRow budgetRow, DataRow factRow, string budgetFAccField)
        {
            DataRow facialAcc = FindCachedRow(facialAccCache,
                Convert.ToInt32(budgetRow[budgetFAccField]));

            if (facialAcc != null)
            {
                // Лицевые счета
                factRow["REFFACIALACC"] = facialAcc["ID"];
            }
            else
            {
                // Лицевые счета
                factRow["REFFACIALACC"] = nullFacialAcc;
            }
        }

        /// <summary>
        /// Возвращает количество записей с некорректными датами
        /// </summary>
        private int GetBadDatesRecCount(SortedList<int, int> badDateList)
        {
            int result = 0;

            foreach (int i in badDateList.Keys)
            {
                result += badDateList[i];
            }

            return result;
        }

        #endregion Общие функции

        #region Закачка данных

        #region работа с базой и кэшами

        private void InitUpdateFixedRows()
        {
            nullKD = clsKD.UpdateFixedRows(this.DB, this.SourceID);
            nullKVSR = clsKVSR.UpdateFixedRows(this.DB, this.SourceID);
            nullEKD = clsEKD.UpdateFixedRows(this.DB, this.SourceID);
            nullPrograms = clsPrograms.UpdateFixedRows(this.DB, this.SourceID);
            nullRegions = clsRegions.UpdateFixedRows(this.DB, this.SourceID);
            nullBudgetAccounts = clsBudgetAccounts.UpdateFixedRows(this.DB, this.SourceID);
            nullMeansType = clsMeansType.UpdateFixedRows(this.DB, this.SourceID);
            nullEKR = clsEKR.UpdateFixedRows(this.DB, this.SourceID);
            nullFKR = clsFKR.UpdateFixedRows(this.DB, this.SourceID);
            nullKCSR = clsKCSR.UpdateFixedRows(this.DB, this.SourceID);
            nullKVR = clsKVR.UpdateFixedRows(this.DB, this.SourceID);
            nullIncomesKinds = clsIncomesKinds.UpdateFixedRows(this.DB, this.SourceID);
            nullNotifyTypes = clsNotifyTypes.UpdateFixedRows(this.DB, this.SourceID);
            nullSubKESR = clsSubKESR.UpdateFixedRows(this.DB, this.SourceID);
            nullFact = clsFact.UpdateFixedRows(this.DB, this.SourceID);
            nullFacialAcc = clsFacialAcc.UpdateFixedRows(this.DB, this.SourceID);
            nullDirection = clsDirection.UpdateFixedRows(this.DB, this.SourceID);
            nullKIF2004 = clsKIF2004.UpdateFixedRows(this.DB, this.SourceID);
            nullKIF2005 = clsKIF2005.UpdateFixedRows(this.DB, this.SourceID);
            nullBudgetOutcomesCls = clsBudgetOutcomesCls.UpdateFixedRows(this.DB, this.SourceID);
            nullBudgetBudget = -1;
            nullFundsSource = -1;
            nullAsgmtKind = -1;
            nullAsgmtSrce = -1;
            nullBuhOperations = -1;
            nullPlanKind = -1;
            nullFinType = clsFinType.UpdateFixedRows(this.DB, this.SourceID);
            nullPlanDocType = clsPlanDocType.UpdateFixedRows(this.DB, this.SourceID);
            nullTransfert = clsTransfert.UpdateFixedRows(this.DB, this.SourceID);
        }

        protected override void QueryData()
        {
            InitClsDataSet(ref daKD, ref dsKD, clsKD, false, string.Empty);
            InitClsDataSet(ref daIncomesKinds, ref dsIncomesKinds, clsIncomesKinds, false, string.Empty);
            InitClsDataSet(ref daKVSR, ref dsKVSR, clsKVSR, false, string.Empty);
            InitClsDataSet(ref daEKD, ref dsEKD, clsEKD, false, string.Empty);
            InitClsDataSet(ref daPrograms, ref dsPrograms, clsPrograms, false, string.Empty);
            InitClsDataSet(ref daRegions, ref dsRegions, clsRegions, false, string.Empty);
            InitClsDataSet(ref daBudgetAccounts, ref dsBudgetAccounts, clsBudgetAccounts, false, string.Empty);
            InitClsDataSet(ref daMeansType, ref dsMeansType, clsMeansType, false, string.Empty);
            InitClsDataSet(ref daEKR, ref dsEKR, clsEKR, false, string.Empty);
            InitClsDataSet(ref daFKR, ref dsFKR, clsFKR, false, string.Empty);
            InitClsDataSet(ref daKCSR, ref dsKCSR, clsKCSR, false, string.Empty);
            InitClsDataSet(ref daKVR, ref dsKVR, clsKVR, false, string.Empty);
            InitClsDataSet(ref daNotifyTypes, ref dsNotifyTypes, clsNotifyTypes, false, string.Empty);
            InitClsDataSet(ref daSubKESR, ref dsSubKESR, clsSubKESR, false, string.Empty);
            InitClsDataSet(ref daFact, ref dsFact, clsFact, false, string.Empty);
            InitClsDataSet(ref daFacialAcc, ref dsFacialAcc, clsFacialAcc, false, string.Empty);
            InitClsDataSet(ref daDirection, ref dsDirection, clsDirection, false, string.Empty);
            InitClsDataSet(ref daKIF2004, ref dsKIF2004, clsKIF2004, false, string.Empty);
            InitClsDataSet(ref daKIF2005, ref dsKIF2005, clsKIF2005, false, string.Empty);
            InitClsDataSet(ref daFinType, ref dsFinType, clsFinType, false, string.Empty);
            InitClsDataSet(ref daPlanDocType, ref dsPlanDocType, clsPlanDocType, false, string.Empty);
            InitClsDataSet(ref daTransfert, ref dsTransfert, clsTransfert, false, string.Empty);

            InitDataSet(ref daBudgetOutcomesCls, ref dsBudgetOutcomesCls, clsBudgetOutcomesCls, false,
                string.Format("SOURCEID = {0} and rowType = 0", this.SourceID), string.Empty);

            InitDataSet(ref daBudgetBudget, ref dsBudgetBudget, clsBudgetBudget, false, string.Empty, string.Empty);
            InitDataSet(ref daFundsSource, ref dsFundsSource, clsFundsSource, false, string.Empty, string.Empty);
            InitDataSet(ref daAsgmtKind, ref dsAsgmtKind, clsAsgmtKind, false, string.Empty, string.Empty);
            InitDataSet(ref daAsgmtSrce, ref dsAsgmtSrce, clsAsgmtSrce, false, string.Empty, string.Empty);
            InitDataSet(ref daBuhOperations, ref dsBuhOperations, clsBuhOperations, false, string.Empty, string.Empty);
            InitDataSet(ref daPlanKind, ref dsPlanKind, clsPlanKind, false, string.Empty, string.Empty);

            InitUpdateFixedRows();
        }

        protected override void UpdateData()
        {
            UpdateDataSet(daKD, dsKD, clsKD);
            UpdateDataSet(daIncomesKinds, dsIncomesKinds, clsIncomesKinds);
            UpdateDataSet(daKVSR, dsKVSR, clsKVSR);
            UpdateDataSet(daEKD, dsEKD, clsEKD);
            UpdateDataSet(daPrograms, dsPrograms, clsPrograms);
            UpdateDataSet(daRegions, dsRegions, clsRegions);
            UpdateDataSet(daBudgetBudget, dsBudgetBudget, clsBudgetBudget);
            UpdateDataSet(daFundsSource, dsFundsSource, clsFundsSource);
            UpdateDataSet(daAsgmtKind, dsAsgmtKind, clsAsgmtKind);
            UpdateDataSet(daAsgmtSrce, dsAsgmtSrce, clsAsgmtSrce);
            UpdateDataSet(daBudgetAccounts, dsBudgetAccounts, clsBudgetAccounts);
            UpdateDataSet(daMeansType, dsMeansType, clsMeansType);
            UpdateDataSet(daEKR, dsEKR, clsEKR);
            UpdateDataSet(daFKR, dsFKR, clsFKR);
            UpdateDataSet(daKCSR, dsKCSR, clsKCSR);
            UpdateDataSet(daKVR, dsKVR, clsKVR);
            UpdateDataSet(daNotifyTypes, dsNotifyTypes, clsNotifyTypes);
            UpdateDataSet(daSubKESR, dsSubKESR, clsSubKESR);
            UpdateDataSet(daFact, dsFact, clsFact);
            UpdateDataSet(daFacialAcc, dsFacialAcc, clsFacialAcc);
            UpdateDataSet(daDirection, dsDirection, clsDirection);
            UpdateDataSet(daKIF2004, dsKIF2004, clsKIF2004);
            UpdateDataSet(daKIF2005, dsKIF2005, clsKIF2005);
            UpdateDataSet(daBuhOperations, dsBuhOperations, clsBuhOperations);
            UpdateDataSet(daBudgetOutcomesCls, dsBudgetOutcomesCls, clsBudgetOutcomesCls);
            UpdateDataSet(daPlanKind, dsPlanKind, clsPlanKind);
            UpdateDataSet(daFinType, dsFinType, clsFinType);
            UpdateDataSet(daPlanDocType, dsPlanDocType, clsPlanDocType);
            UpdateDataSet(daTransfert, dsTransfert, clsTransfert);
        }

        #region GUID

        private const string FX_FX_PROGINDEX_INCOMES_GUID = "0e63ea74-85f9-40ee-af0c-94f2e68a99d4";
        private const string FX_FX_FEDERAL_FUNDS_GUID = "ef479135-37ce-4db7-aee1-beca846c190c";
        private const string FX_FX_NOTIF_TYPE_GUID = "18cf6157-4a48-40e5-b755-944b9cceaa06";
        private const string FX_FX_PROGINDEX_FACIAL_GUID = "68f80a07-4107-4e76-8db6-83c047333f03";
        private const string FX_FX_PROGINDEX_FIN_DOC_GUID = "4a244b1d-67a8-4711-bdf6-a7ce02d950f4";
        private const string FX_FX_PROGINDEX_BUDGET_GUID = "f9aba104-1905-4fc8-8a3a-40316d31d878";

        private const string D_BUDGET_BUDGET_GUID = "6377a0c8-d75e-4ef9-a50f-b3344fde7c0e";
        private const string D_FUNDS_SOURCE_BUDGET_GUID = "d1d65dfc-193f-4c8d-8017-a1a4d34c1c35";
        private const string D_ASGMT_KIND_BUDGET_GUID = "3ca300e5-c766-4182-a26c-64fdc0f6bf66";
        private const string D_ASGMT_SRCE_BUDGET_GUID = "cdff0223-289c-4f5d-8659-c3c4489fae79";
        private const string D_PLAN_KIND_BUDGET_GUID = "a51b447f-a94c-4458-a3a4-dea75a0509d8";

        private const string D_KD_BUDGET_GUID = "f9747df9-e29b-4293-83c8-801b37461230";
        private const string D_KVSR_BUDGET_GUID = "c5f2917f-109e-4c3a-bc0a-9736e28f532e";
        private const string D_EKD_BUDGET_GUID = "dff6565f-54df-4c92-999c-e21c35778c39";
        private const string D_PROGRAMS_BUDGET_GUID = "73fad982-0f3b-423f-b59d-085a48ea3649";
        private const string D_REGIONS_BUDGET_GUID = "6a869647-4795-4984-9186-d17b19bb6226";
        private const string D_BUDGET_ACCOUNTS_BUDGET_GUID = "3df30a25-2ebc-4c81-8a15-624ca65460dc";
        private const string D_MEANS_TYPE_BUDGET_GUID = "87d67237-e7f9-4543-a96b-eabb0f68a519";
        private const string D_EKR_BUDGET_GUID = "4c4a5a53-7862-41f3-9f3b-acaa0b71d513";
        private const string D_FKR_BUDGET_GUID = "19999614-e1df-4205-a22d-7b098600a13c";
        private const string D_KCSR_BUDGET_GUID = "884a99a5-0623-4744-bd85-fe0d0fa7ff35";
        private const string D_KVR_BUDGET_GUID = "9371d823-773b-4afd-a278-24db14f9bf04";
        private const string D_INCOMES_KINDS_BUDGET_GUID = "db51b810-6b7f-43bd-91f6-a94e92dd7e48";
        private const string D_NOTIFY_TYPES_BUDGET_GUID = "e408783f-2c5a-4600-9d97-c23923322200";
        private const string D_SUB_KESR_BUDGET_GUID = "55e02537-25ad-4c65-9701-c48912691cd4";
        private const string D_FACT_BUDGET_GUID = "75e964bc-b533-41df-b72a-a595f6ae74b4";
        private const string D_FACIAL_ACC_BUDGET_GUID = "fe16c2f1-d3d1-4ef1-acc7-b45d1c9bc87b";
        private const string D_DIRECTION_BUDGET_GUID = "3d28dd6b-b022-4b7a-a521-bfc40d005a1a";
        private const string D_KIVF_BUDGET_2004_GUID = "8a68084c-b0af-4350-9dbb-feb6fde878d9";
        private const string D_KIF_BUDGET_2005_GUID = "e37360ca-9838-4c8a-bab1-a62633b88f13";
        private const string D_BUH_OPERATIONS_BUDGET_GUID = "16c40a26-14bd-4e78-981f-eb6b8486e97c";
        private const string D_BUDGET_OUTCOMES_CLS_GUID = "369ba18a-8f2a-499d-a8d3-12df53229d0a";
        private const string D_FIN_TYPE_BUDGET_GUID = "e40beb58-75ae-41df-acfb-962fa40a2b75";
        private const string D_PLAN_DOC_TYPE_BUDGET_GUID = "13bdb350-376c-4935-8e75-4508b9d0064c";
        private const string D_TRANSFERT_BUDGET_GUID = "a606fd04-52e0-4a66-8248-e6e64de4623e";

        private const string F_D_INCOMES32_GUID = "978a74be-9957-4cc5-90c7-76bb9dbb913b";
        private const string F_D_INCOMES_DATA_GUID = "4cfdd7af-3c90-4f27-8330-57e4e671ea26";
        private const string F_R_BUDGET_DATA_GUID = "9ab30b73-a50a-434c-b002-06918a82376b";
        private const string F_R_FACIAL_FIN_DETAIL_GUID = "1ed1e2cb-814e-48cf-9c89-72dcbff09f04";
        private const string F_R_FIN_DOC_DETAIL_GUID = "c3f8caad-79c1-4d3d-bb29-f47bd380cbb3";
        private const string F_BUH_DF_ACCOUNT_OPERATION_GUID = "667aa9ed-34f2-4725-bb24-0f064f6d1747";
        private const string F_IF_PLAN_GUID = "7f64de2e-1ddc-42e5-a498-73c005d6eef3";
        private const string F_IF_FACT_GUID = "3dd83ece-7e56-4fd1-9d76-aabd0293367d";

        private const string DIRECTION_DIM_GUID = "667aa9ed-34f2-4725-bb24-0f064f6d1747";
        private const string DIRECTION_DIM_NAME = "НаправлИспользСредств";

        #endregion GUID

        protected override void InitDBObjects()
        {
            fxcProgIndexIncomes = this.Scheme.Classifiers[FX_FX_PROGINDEX_INCOMES_GUID];
            fxcFederalFunds = this.Scheme.Classifiers[FX_FX_FEDERAL_FUNDS_GUID];
            fxcNotifType = this.Scheme.Classifiers[FX_FX_NOTIF_TYPE_GUID];
            fxcProgIndexFacial = this.Scheme.Classifiers[FX_FX_PROGINDEX_FACIAL_GUID];
            fxcProgIndexFinDoc = this.Scheme.Classifiers[FX_FX_PROGINDEX_FIN_DOC_GUID];
            fxcProgIndexBudget = this.Scheme.Classifiers[FX_FX_PROGINDEX_BUDGET_GUID];

            clsBudgetBudget = this.Scheme.Classifiers[D_BUDGET_BUDGET_GUID];
            clsFundsSource = this.Scheme.Classifiers[D_FUNDS_SOURCE_BUDGET_GUID];
            clsAsgmtKind = this.Scheme.Classifiers[D_ASGMT_KIND_BUDGET_GUID];
            clsAsgmtSrce = this.Scheme.Classifiers[D_ASGMT_SRCE_BUDGET_GUID];
            clsPlanKind = this.Scheme.Classifiers[D_PLAN_KIND_BUDGET_GUID];

            clsBuhOperations = this.Scheme.Classifiers[D_BUH_OPERATIONS_BUDGET_GUID];

            this.UsedClassifiers = new IClassifier[] {
                clsKD = this.Scheme.Classifiers[D_KD_BUDGET_GUID],
                clsKVSR = this.Scheme.Classifiers[D_KVSR_BUDGET_GUID],
                clsEKD = this.Scheme.Classifiers[D_EKD_BUDGET_GUID],
                clsPrograms = this.Scheme.Classifiers[D_PROGRAMS_BUDGET_GUID],
                clsRegions = this.Scheme.Classifiers[D_REGIONS_BUDGET_GUID],
                clsBudgetAccounts = this.Scheme.Classifiers[D_BUDGET_ACCOUNTS_BUDGET_GUID],
                clsMeansType = this.Scheme.Classifiers[D_MEANS_TYPE_BUDGET_GUID],
                clsEKR = this.Scheme.Classifiers[D_EKR_BUDGET_GUID],
                clsFKR = this.Scheme.Classifiers[D_FKR_BUDGET_GUID],
                clsKCSR = this.Scheme.Classifiers[D_KCSR_BUDGET_GUID],
                clsKVR = this.Scheme.Classifiers[D_KVR_BUDGET_GUID],
                clsIncomesKinds = this.Scheme.Classifiers[D_INCOMES_KINDS_BUDGET_GUID],
                clsNotifyTypes = this.Scheme.Classifiers[D_NOTIFY_TYPES_BUDGET_GUID],
                clsSubKESR = this.Scheme.Classifiers[D_SUB_KESR_BUDGET_GUID],
                clsFact = this.Scheme.Classifiers[D_FACT_BUDGET_GUID],
                clsFacialAcc = this.Scheme.Classifiers[D_FACIAL_ACC_BUDGET_GUID],
                clsDirection = this.Scheme.Classifiers[D_DIRECTION_BUDGET_GUID],
                clsFinType = this.Scheme.Classifiers[D_FIN_TYPE_BUDGET_GUID],
                clsKIF2004 = this.Scheme.Classifiers[D_KIVF_BUDGET_2004_GUID],
                clsKIF2005 = this.Scheme.Classifiers[D_KIF_BUDGET_2005_GUID],
                clsPlanDocType = this.Scheme.Classifiers[D_PLAN_DOC_TYPE_BUDGET_GUID],
                clsTransfert = this.Scheme.Classifiers[D_TRANSFERT_BUDGET_GUID],
                clsBudgetOutcomesCls = this.Scheme.Classifiers[D_BUDGET_OUTCOMES_CLS_GUID] };

            this.VersionClassifiers = new IClassifier[] { clsKD, clsFKR, clsEKR, clsKCSR, clsSubKESR, clsBudgetOutcomesCls };

            this.CubeClassifiers = (IClassifier[])CommonRoutines.ConcatArrays(this.UsedClassifiers,
                new IClassifier[] { clsBudgetBudget, clsFundsSource, clsAsgmtKind, clsAsgmtSrce, clsPlanKind, clsBuhOperations, 
                                    fxcProgIndexIncomes, fxcFederalFunds, fxcNotifType, fxcProgIndexFacial, fxcProgIndexFinDoc, fxcProgIndexBudget });

            this.UsedFacts = new IFactTable[] {
                fctIncomes32 = this.Scheme.FactTables[F_D_INCOMES32_GUID],
                fctIncomesPlan = this.Scheme.FactTables[F_D_INCOMES_DATA_GUID],
                fctBudgetData = this.Scheme.FactTables[F_R_BUDGET_DATA_GUID],
                fctFacialFinDetail = this.Scheme.FactTables[F_R_FACIAL_FIN_DETAIL_GUID],
                fctFinDocDetail = this.Scheme.FactTables[F_R_FIN_DOC_DETAIL_GUID],
                fctAccountOperations = this.Scheme.FactTables[F_BUH_DF_ACCOUNT_OPERATION_GUID], 
                fctIFPlan = this.Scheme.FactTables[F_IF_PLAN_GUID], 
                fctIFFact = this.Scheme.FactTables[F_IF_FACT_GUID] };

            this.CubeFacts = new IFactTable[] { };
            if (CheckBudgetBlockPump(BudgetBlock.Incomes))
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctIncomes32 });
            if (CheckBudgetBlockPump(BudgetBlock.IncomesPlan))
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctIncomesPlan });
            if (CheckBudgetBlockPump(BudgetBlock.OutcomesPlan))
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctBudgetData });
            if (CheckBudgetBlockPump(BudgetBlock.Treasury))
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctFacialFinDetail });
            if (CheckBudgetBlockPump(BudgetBlock.Financing))
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctFinDocDetail });
            if (CheckBudgetBlockPump(BudgetBlock.AccountOperations))
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctAccountOperations });
            if (CheckBudgetBlockPump(BudgetBlock.IFPlan))
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctIFPlan });
            if (CheckBudgetBlockPump(BudgetBlock.IFFact))
                CubeFacts = (IFactTable[])CommonRoutines.ConcatArrays(CubeFacts, new IFactTable[] { fctIFFact });

            dimensionsForProcess = new string[] { DIRECTION_DIM_GUID, DIRECTION_DIM_NAME };

        }

        protected override void PumpFinalizing()
        {
            ClearDataSet(ref dsBudgetAccounts);
            ClearDataSet(ref dsEKD);
            ClearDataSet(ref dsEKR);
            ClearDataSet(ref dsBudgetFacts);
            ClearDataSet(ref dsFKR);
            ClearDataSet(ref dsIncomes32);
            ClearDataSet(ref dsIncomesKinds);
            ClearDataSet(ref dsIncomesPlan);
            ClearDataSet(ref dsKCSR);
            ClearDataSet(ref dsKD);
            ClearDataSet(ref dsKVR);
            ClearDataSet(ref dsKVSR);
            ClearDataSet(ref dsMeansType);
            ClearDataSet(ref dsProgIndexIncomes);
            ClearDataSet(ref dsPrograms);
            ClearDataSet(ref dsRegions);
            ClearDataSet(ref dsFacialFinDetail);
            ClearDataSet(ref dsFinancing);
            ClearDataSet(ref dsOutcomesPlan);
            ClearDataSet(ref dsAccountOperations);
            ClearDataSet(ref dsBudgetBudget);
            ClearDataSet(ref dsFundsSource);
            ClearDataSet(ref dsAsgmtKind);
            ClearDataSet(ref dsAsgmtSrce);
            ClearDataSet(ref dsBuhOperations);
            ClearDataSet(ref dsBudgetOutcomesCls);
            ClearDataSet(ref dsPlanKind);
            ClearDataSet(ref dsFinType);
            ClearDataSet(ref dsPlanDocType);
            ClearDataSet(ref dsTransfert);

            if (clsCache != null)
                clsCache.Clear();
            if (factCache != null)
                factCache.Clear();
        }

        #endregion работа с базой и кэшами

        /// <summary>
        /// Проверяет, нужно ли закачивает указанный блок бюджета
        /// </summary>
        /// <param name="budgetBlock">Блок бюджета</param>
        /// <returns>Нужно закачивать или нет</returns>
        private bool CheckBudgetBlockPump(BudgetBlock budgetBlock)
        {
            string paramName = string.Empty;
            switch (budgetBlock)
            {
                case BudgetBlock.Incomes: paramName = "ucbIncomes";
                    break;
                case BudgetBlock.IncomesPlan: paramName = "ucbIncomesPlan";
                    break;
                case BudgetBlock.OutcomesPlan: paramName = "ucbOutcomesPlan";
                    break;
                case BudgetBlock.Treasury: paramName = "ucbTreasury";
                    break;
                case BudgetBlock.Financing: paramName = "ucbFinancing";
                    break;
                case BudgetBlock.AccountOperations: paramName = "ucbAccountOperations";
                    break;
                case BudgetBlock.IFPlan: paramName = "ucbIFPlan";
                    break;
                case BudgetBlock.IFFact: paramName = "ucbIFFact";
                    break;
            }
            if (paramName == string.Empty)
                return false;
            return Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, paramName, "False"));
        }

        /// <summary>
        /// Закачивает данные одного блока из бюджетной базы в нашу
        /// </summary>
        /// <param name="blockName">Наименование блока</param>
        /// <param name="tableName">Наименование таблицы в базе бюджета</param>
        /// <param name="queryFMData">Делегат функции запроса закачанных данных</param>
        /// <param name="queryBudgetData">Делегат функции запроса данных бюджета</param>
        /// <param name="processData">Делегат функции обработки данных</param>
        /// <param name="preWritingProtocolActions">Делегат функции, выполняющей какие-либо действия до записи в лог</param>
        private int PumpBudgetBlock(string blockName, string tableName, GetVoidDelegate queryFMData,
            GetIntDelegate queryBudgetData, ProcessBudgetDataDelegate processData,
            GetVoidDelegate preWritingProtocolActions)
        {
            // Счетчик обработанных записей
            int processedRecCount = 0;
            // Счетчик добавленных записей
            int addedRowsCount = 0;
            // Счетчик измененных записей
            int updatedRowsCount = 0;
            // Дополнительное сообщение о пропущенных записях
            string skippedRecsMessage = string.Empty;


            try
            {
                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeInformation, string.Format("Обработка данных {0}.", blockName));

                ClearDataSet(ref dsBudgetFacts);

                // Запрос данных ФМ
                SetProgress(0, 0, string.Format("{0}. Запрос закачанных данных...", blockName), string.Empty);
                WriteToTrace(string.Format("{0}. Запрос закачанных данных...", blockName), TraceMessageKind.Information);

                queryFMData.Invoke();
                if (this.PumpMode == BudgetDataPumpMode.Update)
                {
                    CollectGarbage();
                }

                WriteToTrace(string.Format("{0}. Закачанные данные получены.", blockName), TraceMessageKind.Information);

                // Запрос данных бюджета
                SetProgress(-1, -1, string.Format("{0}. Запрос данных АС Бюджет ...", blockName), string.Empty);
                WriteToTrace(string.Format("{0}. Запрос данных АС Бюджет ...", blockName), TraceMessageKind.Information);
                
                int totalRecs = queryBudgetData.Invoke();
                CollectGarbage();

                WriteToTrace(string.Format("{0}. Данные AC Бюджет получены.", blockName), TraceMessageKind.Information);

                WriteToTrace(string.Format("Записей {0} ({1}): {2}.", blockName, tableName, totalRecs), TraceMessageKind.Information);
                if (totalRecs == 0)
                {
                    if (this.PumpMode == BudgetDataPumpMode.Update)
                        WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, 
                            "Нет данных для закачки. Возможно со времени последней закачки изменений в базу АС Бюджет внесено не было.");
                    return 0;
                }

                badDateList.Clear();
                currentBlockName = blockName;

                // Обработка данных
                processData.Invoke(ref addedRowsCount, ref updatedRowsCount, ref processedRecCount,
                    ref skippedRecsMessage);

                RecordBadDateList();
                if (preWritingProtocolActions != null)
                {
                    preWritingProtocolActions.Invoke();
                }

                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeInformation, 
                    string.Format("Обработка данных {0} успешно закончена. \n" +
                        "Обработано записей {1}: {2}, добавлено записей: {3}, обновлено: {4}. " +
                        "Пропущено записей с некорректной датой: {5}{6}.",
                        blockName, tableName, processedRecCount, addedRowsCount, updatedRowsCount,
                        GetBadDatesRecCount(badDateList), skippedRecsMessage));

                return totalRecs;
            }
            catch (ThreadAbortException)
            {
                RecordBadDateList();
                if (preWritingProtocolActions != null)
                {
                    preWritingProtocolActions.Invoke();
                }

                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeCriticalError, string.Format(
                        "Обработка данных {0} прервана пользователем. \n" +
                        "На момент прерывания достигнуты следующие результаты. " +
                        "Обработано записей {1}: {2}, добавлено записей: {3}, обновлено: {4}. " +
                        "Пропущено записей с неверной датой: {5}{6}. Данные не сохранены.",
                        blockName, tableName, processedRecCount, addedRowsCount, updatedRowsCount,
                        GetBadDatesRecCount(badDateList), skippedRecsMessage));
                throw;
            }
            catch (Exception ex)
            {
                RecordBadDateList();
                if (preWritingProtocolActions != null)
                {
                    preWritingProtocolActions.Invoke();
                }

                WriteEventIntoDataPumpProtocol(
                    DataPumpEventKind.dpeCriticalError, string.Format(
                        "Обработка данных {0} завершена с ошибками. \n" +
                        "На момент возникновения ошибки достигнуты следующие результаты. " +
                        "Обработано записей {1}: {2}, добавлено записей: {3}, обновлено: {4}. " +
                        "Пропущено записей с неверной датой: {5}{6}. Данные не сохранены.",
                        blockName, tableName, processedRecCount, addedRowsCount, updatedRowsCount,
                        GetBadDatesRecCount(badDateList), skippedRecsMessage), ex);
                throw;
            }
            finally
            {
                ClearBudgetFactDataSet();

                ClearDataSet(ref dsBudgetFacts);

                CollectGarbage();
            }
        }

        private void PumpTreasuryData()
        {
        	isTreasury = true;
            isCheckStage = (this.StagesQueue[PumpProcessStates.CheckData].StageCurrentState == StageState.InProgress);
            toShowIncorrectDate = true;

            sumForCheckDebit = 0;
            sumForCheckReturnDebit = 0;

            tCaptionTableName = "FacialFinCaption";
            tDetailTableName = "FacialFinDetail";
            PumpBudgetBlock("Блок \"Казначейство\"", "FacialFinCaption",
                new GetVoidDelegate(QueryFMTreasuryData),
                new GetIntDelegate(QueryFacialFinCaptionCount),
                new ProcessBudgetDataDelegate(PumpTreasury),
                null);
                
            if (isCheckStage)
            {
                string message = string.Format("ИТОГО НЕ ЗАКАЧАНО ДАННЫХ ПО ТАБЛИЦЕ КАЗНАЧЕЙСТВА: расход '{0}', возврат расхода '{1}'.",
                    sumForCheckDebit, sumForCheckReturnDebit);
                CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeWarning, message, this.PumpID, this.SourceID);
            }
                
            // если на уточнения не заводятся детальные данные - качаем из уточнений
            if (MajorDBVersion >= 35)
                {
	                sumForCheckDebit = 0;
    	            sumForCheckReturnDebit = 0;

	                tCaptionTableName = "CorrectionCaption";
    	            tDetailTableName = "CorrectionDetail";


        	        PumpBudgetBlock("Блок \"Казначейство\" (уточнения)", "CorrectionCaption",
            	        new GetVoidDelegate(QueryFMTreasuryData),
                	    new GetIntDelegate(QueryFacialFinCaptionCount),
                    	new ProcessBudgetDataDelegate(PumpTreasury),
	                    null);
	                    
	                if (isCheckStage)
    	            {
        	            string message = string.Format("ИТОГО НЕ ЗАКАЧАНО ДАННЫХ ПО ТАБЛИЦЕ УТОЧНЕНИЙ: расход '{0}', возврат расхода '{1}'.", 
            	            sumForCheckDebit, sumForCheckReturnDebit);
	                    CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeWarning, message, this.PumpID, this.SourceID);
    	            }
	                    
                }
        }

        private void PumpIncomes()
        {
            toShowIncorrectDate = true;
            isTreasury = false;
            // закачиваем из incomes32
            tDetailTableName = "Incomes32";
            PumpBudgetBlock("Блок \"Доходы\"", "Incomes32",
                new GetVoidDelegate(QueryFMIncomesData),
                new GetIntDelegate(QueryBudgetIncomesData),
                new ProcessBudgetDataDelegate(PumpIncomes),
                null);
            // закачиваем из facialFinDetail
            tCaptionTableName = "FacialFinCaption";
            tDetailTableName = "FacialFinDetail";
            PumpBudgetBlock("Блок \"Доходы\" Казначейство", "FacialFinDetail",
                new GetVoidDelegate(QueryFMIncomesData),
                new GetIntDelegate(QueryBudgetIncomesFacialFinDetailData),
                new ProcessBudgetDataDelegate(PumpIncomes),
                null);
            // если на уточнения не заводятся детальные данные - качаем из уточнений
            if (MajorDBVersion >= 35)
            {
                tCaptionTableName = "CorrectionCaption";
                tDetailTableName = "CorrectionDetail";
                PumpBudgetBlock("Блок \"Доходы\" (уточнения)", "CorrectionCaption",
                    new GetVoidDelegate(QueryFMIncomesData),
                    new GetIntDelegate(QueryBudgetIncomesFacialFinDetailData),
                    new ProcessBudgetDataDelegate(PumpIncomes),
                    null);
            }
        }

        private void PumpOutcomesPlan()
        {
            PumpBudgetBlock("Блок \"План расходов\"", "BudgetData",
                new GetVoidDelegate(QueryFMOutcomesPlanData),
                new GetIntDelegate(QueryBudgetOutcomesPlanData),
                new ProcessBudgetDataDelegate(PumpOutcomesPlan),
                null);
            // для новосибирска и саратова качаем лимиты
            if ((this.Region == RegionName.Novosibirsk) || (this.Region == RegionName.Saratov))
                PumpBudgetBlock("Блок \"План расходов (лимиты)\"", "BudgetData",
                    new GetVoidDelegate(QueryFMOutcomesPlanData),
                    new GetIntDelegate(QueryBudgetOutcomesPlanDataLimit),
                    new ProcessBudgetDataDelegate(PumpOutcomesPlanLimit),
                    null);
        }

        private void PumpIFPlan()
        {
            PumpBudgetBlock("Блок \"План источников финансирования\"", "IFPlan",
                new GetVoidDelegate(QueryFMIFPlanData),
                new GetIntDelegate(QueryBudgetIFPlanData),
                new ProcessBudgetDataDelegate(PumpIFPlan),
                null);
        }

        private void PumpIFFact()
        {
            isTreasury = true;
            tCaptionTableName = "FacialFinCaption";
            tDetailTableName = "FacialFinDetail";
            PumpBudgetBlock("Блок \"Факт источников финансирования\"", "FacialFinCaption",
                new GetVoidDelegate(QueryFMIFFact),
                new GetIntDelegate(QueryFacialFinCaptionIFFactCount),
                new ProcessBudgetDataDelegate(PumpIFFact),
                null);
            // если на уточнения не заводятся детальные данные - качаем из уточнений
            if (MajorDBVersion >= 35)
            {
                tCaptionTableName = "CorrectionCaption";
                tDetailTableName = "CorrectionDetail";
                PumpBudgetBlock("Блок \"Факт источников финансирования\" (уточнения)", "CorrectionCaption",
                    new GetVoidDelegate(QueryFMIFFact),
                    new GetIntDelegate(QueryFacialFinCaptionIFFactCount),
                    new ProcessBudgetDataDelegate(PumpIFFact),
                    null);
            }
        }

        /// <summary>
        /// Закачивает блоки бюджета
        /// </summary>
        private void PumpBudgetBlocks()
        {
            tempMeansType = Convert.ToString(GetParamValueByName(
                            this.PumpRegistryElement.ProgramConfig, "edMeansType", string.Empty)).Trim();
            // Доходы
            if (toPumpIncomes)
                PumpIncomes();
            // План доходов
            if (toPumpIncomesPlan)
            {
                PumpBudgetBlock("Блок \"План доходов\"", "IncomesData",
                    new GetVoidDelegate(QueryFMIncomesPlanData),
                    new GetIntDelegate(QueryBudgetIncomesPlanData),
                    new ProcessBudgetDataDelegate(PumpIncomesPlan),
                    null);
            }
            // План расходов
            if (toPumpOutcomesPlan)
                PumpOutcomesPlan();
            // Казначейство
            if (toPumpTreasury)
                PumpTreasuryData();
            // Финансирование
            if (toPumpFinancing)
            {
                PumpBudgetBlock("Блок \"Финансирование\"", "FinDocDetail",
                    new GetVoidDelegate(QueryFMFinancingData),
                    new GetIntDelegate(QueryBudgetFinancingData),
                    new ProcessBudgetDataDelegate(PumpFinancing),
                    null);
            }
            // Операции со счетами
            if (toPumpAccountOperations)                
            {
                PumpBudgetBlock("Блок \"Операции со счетами\"", "DFAccountOperation",
                   new GetVoidDelegate(QueryFMAccountOperationsData),
                   new GetIntDelegate(QueryBudgetAccountOperationsData),
                   new ProcessBudgetDataDelegate(PumpAccountOperations),
                   null);
            }
            if (toPumpIFPlan)
                PumpIFPlan();
            if (toPumpIFFact)
                PumpIFFact();
        }

        /// <summary>
        /// Возвращает строку с описанием режима закачки
        /// </summary>
        /// <param name="pumpMode">Режим закачки</param>
        /// <returns>Строка с описанием</returns>
        private string PumpModeToString(BudgetDataPumpMode pumpMode)
        {
            switch (this.PumpMode)
            {
                // Полная закачка
                case BudgetDataPumpMode.Full:
                    return "Полная закачка.";

                // Обновление
                case BudgetDataPumpMode.Update:
                    return "Обновление.";

                // Полная закачка фактов, обновление классификаторов
                case BudgetDataPumpMode.FullFact:
                    return "Полная закачка фактов, обновление классификаторов.";
            }

            return string.Empty;
        }

        private void SetClsHierarchy()
        {
        }

        /// <summary>
        /// Функция обработки файлов источника данных
        /// </summary>
        /// <param name="dir">Каталог источника</param>
        protected override void ProcessFiles(DirectoryInfo dir)
        {
            FileInfo[] files = dir.GetFiles("*.udl", SearchOption.AllDirectories);

            if (files.GetLength(0) == 0)
                throw new Exception("Файлы подключения к базе данных в источнике не обнаружены.");

            for (int i = 0; i < files.GetLength(0); i++)
            {
                // Подключаемся
                if (!InitBudgetDB(files[i]))
                {
                    throw new DataSourceIsCorruptException("Ошибка при подключении к источнику.");
                }

                // Проверка версии базы
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    string.Format("Версия бюджета: {0}; Версия базы данных {1}.{2}", 
                    this.CurrentDBVersion, this.MajorDBVersion, this.MinorDBVersion));
                if (this.MajorDBVersion < 36)
                    if (!this.SupportedVersions.Contains(this.CurrentDBVersion))
                    {
                        {

                            // По версиям старше 32 (6.08) выдавать не ошибку, а предупреждение, что версия 
                            // официально не поддерживется
                            if (this.MajorDBVersion < 32)
                            {
                                throw new DataSourceIsCorruptException(string.Format(
                                    "Версия базы {0} не поддерживается.", this.CurrentDBVersion));
                            }
                            else
                            {
                                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                                    "Версия {0} официально не поддерживается. Для ее поддержки обратитесь к разработчику.",
                                    this.CurrentDBVersion));
                            }
                        }


                        // По версиям старше 32 (6.08) выдавать не ошибку, а предупреждение, что версия 
                        // официально не поддерживется
                        if (this.MajorDBVersion < 32)
                        {
                            throw new DataSourceIsCorruptException(string.Format(
                                "Версия базы {0} не поддерживается.", this.CurrentDBVersion));
                        }
                        else
                        {
                            WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeWarning, string.Format(
                                "Версия {0} официально не поддерживается. Для ее поддержки обратитесь к разработчику.",
                                this.CurrentDBVersion));
                        }
                    }

                // Проверка соответствия года базы и параметра источника
                if (this.BudgetYear != this.DataSource.Year)
                {
                    throw new DataSourceIsCorruptException(string.Format(
                        "Дата текущего бюджета {0} не соответствует параметрам источника.", this.BudgetYear));
                }

                try
                {
                    filesCount++;

                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeStartFilePumping, string.Format(
                            "Обработка данных базы АС Бюджет {0}, версия {1}.{2}. Текущие бюджеты: {3}; год: {4}. {5}",
                            this.DatabasePath, this.MajorDBVersion, this.MinorDBVersion, 
                            string.Join(", ", this.BudgetRefs.ToArray()), this.BudgetYear, PumpModeToString(this.PumpMode)));

                    // Закачка классификаторов
                    PumpBudgetClsData();

                    // Закачка блоков АС Бюджет
                    for (int m = 0; m < this.BudgetRefs.Count; m++)
                    {
                        this.BudgetRef = Convert.ToInt32(this.BudgetRefs[m]);
                        PumpBudgetBlocks();
                        UpdateData();
                    }

                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeSuccessfullFinishFilePump, string.Format(
                            "Обработка данных базы АС Бюджет {0}, версия {1} успешно завершена. " +
                            "Текущие бюджеты: {2}; год: {3}.",
                            this.DatabasePath, this.CurrentDBVersion, string.Join(", ", this.BudgetRefs.ToArray()), this.BudgetYear));
                }
                catch (ThreadAbortException)
                {
                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                            "Обработка данных базы АС Бюджет {0}, версия {1} прервана пользователем. " +
                            "Текущие бюджеты: {2}; год: {3}.",
                            this.DatabasePath, this.CurrentDBVersion, string.Join(", ", this.BudgetRefs.ToArray()), this.BudgetYear));
                    throw;
                }
                catch (Exception ex)
                {
                    WriteEventIntoDataPumpProtocol(
                        DataPumpEventKind.dpeFinishFilePumpWithError, string.Format(
                            "Обработка данных базы АС Бюджет {0}, версия {1} закончена с ошибками: {2}. " +
                            "Текущие бюджеты: {3}; год: {4}.",
                            this.DatabasePath, this.CurrentDBVersion, ex.Message, string.Join(", ", this.BudgetRefs.ToArray()), this.BudgetYear));
                    throw;
                }

                break;
            }
            UpdateData();
            SetClsHierarchy();
            UpdateData();
        }

		protected override void DirectPumpData()
		{
            totalFiles = this.RootDir.GetFiles("*.udl", SearchOption.AllDirectories).GetLength(0);
            filesCount = 0;

            if (!GetPumpParams())
            {
                throw new Exception("Ошибка при получении параметров закачки.");
            }

            PumpDataFYTemplate();
		}

        private string GetDateConstrByPumpParams(string dateFieldName, bool withDay)
        {
            if ((!this.useDateConstraint) || (this.PumpMode != BudgetDataPumpMode.FullFact))
                return string.Empty;
            if ((this.dateMin == string.Empty) || (this.dateMax == string.Empty))
                return string.Empty;
            string construct = string.Empty;
            if (withDay)
                construct += "(" + dateFieldName + ">= " + this.dateMin.Substring(4) + this.dateMin.Substring(2, 2).PadLeft(2, '0') +
                                this.dateMin.Substring(0, 2).PadLeft(2, '0') + ") and " +
                             "(" + dateFieldName + "<= " + this.dateMax.Substring(4) + this.dateMax.Substring(2, 2).PadLeft(2, '0') +
                                this.dateMax.Substring(0, 2).PadLeft(2, '0') + ")";
            else
                construct += "(" + dateFieldName + ">= " + this.dateMin.Substring(4) + this.dateMin.Substring(2, 2).PadLeft(2, '0') + "00) and " +
                             "(" + dateFieldName + "<= " + this.dateMax.Substring(4) + this.dateMax.Substring(2, 2).PadLeft(2, '0') + "00)";
            return construct;
        }

        private void DeleteFactDataWithConstrByDate(IFactTable[] usedFacts, string dateFieldName, bool withDay)
        {
            string construct = GetDateConstrByPumpParams(dateFieldName, withDay);
            DirectDeleteFactData(usedFacts, -1, this.SourceID, construct);
        }

        private void DeleteData()
        {
            // удаляем факты

            // Доходы
            if (toPumpIncomes)
                DeleteFactDataWithConstrByDate(new IFactTable[] { fctIncomes32 }, "RefYearDayUNV", true);
            // План доходов
            if (toPumpIncomesPlan)
                DeleteFactDataWithConstrByDate(new IFactTable[] { fctIncomesPlan }, "RefDateUNV", true);
            // План расходов
            if (toPumpOutcomesPlan)
                DeleteFactDataWithConstrByDate(new IFactTable[] { fctBudgetData }, "RefDateUNV", true);
            // Казначейство
            if (toPumpTreasury)
                DeleteFactDataWithConstrByDate(new IFactTable[] { fctFacialFinDetail }, "RefYearDayUNV", true);
            // Финансирование
            if (toPumpFinancing)
                DeleteFactDataWithConstrByDate(new IFactTable[] { fctFinDocDetail }, "RefYearDayUNV", true);
            // Операции со счетами
            if (toPumpAccountOperations)
                DeleteFactDataWithConstrByDate(new IFactTable[] { fctAccountOperations }, "RefYearDayUNV", true);
            // План ИФ
            if (toPumpIFPlan)
                DeleteFactDataWithConstrByDate(new IFactTable[] { fctIFPlan }, "RefYearDayUNV", true);
            // Факт ИФ
            if (toPumpIFPlan)
                DeleteFactDataWithConstrByDate(new IFactTable[] { fctIFFact }, "RefYearDayUNV", true);

            // удаляем классификаторы
            if (this.PumpMode != BudgetDataPumpMode.Full)
                return;

            if (toPumpIncomes && toPumpIncomesPlan && toPumpOutcomesPlan && toPumpFinancing && toPumpTreasury)
                DirectDeleteClsData(new IClassifier[] { clsKVSR }, -1, this.SourceID, string.Empty);

            if (toPumpIncomes && toPumpIncomesPlan)
                DirectDeleteClsData(new IClassifier[] { clsIncomesKinds, clsPrograms, clsEKD }, -1, this.SourceID, string.Empty);

            if (toPumpIncomes && toPumpIncomesPlan)
                DirectDeleteClsData(new IClassifier[] { clsKD }, -1, this.SourceID, string.Empty);

            if (toPumpIncomes && toPumpIncomesPlan && toPumpOutcomesPlan && toPumpTreasury &&
                toPumpFinancing && toPumpAccountOperations)
                DirectDeleteClsData(new IClassifier[] { clsRegions }, -1, this.SourceID, string.Empty);

            if (toPumpIncomes)
                DirectDeleteClsData(new IClassifier[] { clsBudgetAccounts }, -1, this.SourceID, string.Empty);

            if (toPumpIncomes && toPumpIncomesPlan && toPumpOutcomesPlan && toPumpTreasury &&
                toPumpFinancing && toPumpAccountOperations)
                DirectDeleteClsData(new IClassifier[] { clsMeansType }, -1, this.SourceID, string.Empty);

            if (toPumpIncomes && toPumpOutcomesPlan && toPumpTreasury && toPumpFinancing)
                DirectDeleteClsData(new IClassifier[] { clsEKR, clsFKR, clsKCSR, clsKVR },
                    -1, this.SourceID, string.Empty);

            if (toPumpIncomes && toPumpTreasury && toPumpFinancing && toPumpOutcomesPlan)
                DirectDeleteClsData(new IClassifier[] { clsDirection }, -1, this.SourceID, string.Empty);

            if (toPumpOutcomesPlan && toPumpTreasury && toPumpFinancing)
                DirectDeleteClsData(new IClassifier[] { clsSubKESR, clsFact }, -1, this.SourceID, string.Empty);

            if (toPumpOutcomesPlan && toPumpTreasury && toPumpFinancing)
                DirectDeleteClsData(new IClassifier[] { clsNotifyTypes }, -1, this.SourceID, string.Empty);

            if (toPumpOutcomesPlan && toPumpTreasury && toPumpFinancing && toPumpIncomes && toPumpIncomesPlan)
                DirectDeleteClsData(new IClassifier[] { clsFacialAcc }, -1, this.SourceID, string.Empty);

            if (toPumpOutcomesPlan && toPumpTreasury)
                DirectDeleteClsData(new IClassifier[] { clsBudgetOutcomesCls }, -1, this.SourceID, string.Empty);

            if (toPumpFinancing)
                DirectDeleteClsData(new IClassifier[] { clsKIF2004, clsKIF2005 }, -1, this.SourceID, string.Empty);

            if (toPumpOutcomesPlan && toPumpTreasury && toPumpFinancing)
                DirectDeleteClsData(new IClassifier[] { clsFinType }, -1, this.SourceID, string.Empty);

            if (toPumpOutcomesPlan && toPumpIncomesPlan)
                DirectDeleteClsData(new IClassifier[] { clsPlanDocType }, -1, this.SourceID, string.Empty);

            if (toPumpOutcomesPlan && toPumpIncomesPlan && toPumpTreasury && toPumpIncomes)
                DirectDeleteClsData(new IClassifier[] { clsTransfert }, -1, this.SourceID, string.Empty);
        }

        protected override void DeleteEarlierPumpedData()
        {
            string blockInfo = string.Empty;
            toPumpIncomes = CheckBudgetBlockPump(BudgetBlock.Incomes);
            if (toPumpIncomes)
                blockInfo += "Доходы; ";
            toPumpIncomesPlan = CheckBudgetBlockPump(BudgetBlock.IncomesPlan);
            if (toPumpIncomesPlan)
                blockInfo += "План доходов; ";
            toPumpOutcomesPlan = CheckBudgetBlockPump(BudgetBlock.OutcomesPlan);
            if (toPumpOutcomesPlan)
                blockInfo += "План расходов; ";
            toPumpTreasury = CheckBudgetBlockPump(BudgetBlock.Treasury);
            if (toPumpTreasury)
                blockInfo += "Казначейство; ";
            toPumpFinancing = CheckBudgetBlockPump(BudgetBlock.Financing);
            if (toPumpFinancing)
                blockInfo += "Финансирование; ";
            toPumpAccountOperations = CheckBudgetBlockPump(BudgetBlock.AccountOperations);
            if (toPumpAccountOperations)
                blockInfo += "Операции со счетами; ";
            toPumpIFPlan = CheckBudgetBlockPump(BudgetBlock.IFPlan);
            if (toPumpIFPlan)
                blockInfo += "План ИФ; ";
            toPumpIFFact = CheckBudgetBlockPump(BudgetBlock.IFFact);
            if (toPumpIFFact)
                blockInfo += "Факт ИФ; ";

            if (blockInfo != string.Empty)
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation, 
                    string.Format("Закачиваемые блоки: {0}", blockInfo));

            if (this.PumpMode == BudgetDataPumpMode.Update)
                return;
            DeleteData();
        }

		#endregion Закачка данных

        #region Обработка данных

        /// <summary>
        /// Заполняет расходный классификатор
        /// </summary>
        /// <param name="fct">Таблица фактов</param>
        /// <param name="fctIndex"> 1 - fctBudgetData, 2 - fctFacialFinDetail</param>
        private void FillFactOutcomesCls(IFactTable fct, int fctIndex)
        {
            string semantic = fct.FullCaption;
            WriteEventIntoProcessDataProtocol(
                ProcessDataEventKind.pdeStart, string.Format("Старт обработки данных {0}.", semantic));
            int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select count(id) from {0} where SOURCEID = {1}", fct.FullDBName, this.SourceID), QueryResultTypes.Scalar));
            if (totalRecs == 0)
            {
                WriteEventIntoProcessDataProtocol(
                    ProcessDataEventKind.pdeWarning, string.Format(
                        "Нет данных {0} для обработки по текущему источнику.", semantic));
                return;
            }
            int firstID = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select min(id) from {0} where SOURCEID = {1}", fct.FullDBName, this.SourceID), QueryResultTypes.Scalar));
            int lastID = firstID + MAX_DS_RECORDS_AMOUNT * 2 - 1;
            int processedRecCount = 0;
            try
            {
                IDbDataAdapter da = null;
                DataSet ds = null;
                do
                {
                    // Ограничение запроса для выборки порции данных
                    string idConstr = string.Format("ID >= {0} and ID <= {1} and SOURCEID = {2}", firstID, lastID, this.SourceID);
                    firstID = lastID + 1;
                    lastID += MAX_DS_RECORDS_AMOUNT * 2;
                    InitDataSet(ref da, ref ds, fct, idConstr);
                    int selectedRecCount = ds.Tables[0].Rows.Count;
                    if (selectedRecCount == 0)
                        continue;
                    // Обработка полученных данных
                    for (int i = 0; i < selectedRecCount; i++)
                    {
                        processedRecCount++;
                        SetProgress(totalRecs, processedRecCount,
                            string.Format("Обработка данных {0}...", semantic),
                            string.Format("Запись {0} из {1}", processedRecCount, totalRecs));
                        DataRow row = ds.Tables[0].Rows[i];
                        // работа с единым расходным классификатором
                        switch (fctIndex)
                        {
                            case 1:
                                // по плану расходов формируем классификатор и проставляем ссылку
                                row["RefRBudget"] = PumpBudgetOutcomesClsRows(row, false);
                                break;
                            case 2:
                                // по факту расходов (казначейство) - только проставляем ссылку
                                row["RefRBudget"] = PumpBudgetOutcomesClsRows(row, true);
                                break;
                        }
                    }
                    UpdateProcessedData();
                    UpdateDataSet(da, ds, fct);
                }
                while (processedRecCount < totalRecs);
                UpdateProcessedData();
                UpdateDataSet(da, ds, fct);
                WriteEventIntoProcessDataProtocol(
                    ProcessDataEventKind.pdeSuccefullFinished, string.Format(
                        "Обработка данных {0} успешно завершена. Обработано {1} записей.", semantic, processedRecCount));
            }
            catch (ThreadAbortException)
            {
                WriteEventIntoProcessDataProtocol(
                    ProcessDataEventKind.pdeFinishedWithErrors, string.Format(
                        "Обработка данных {0} прервана пользователем. Обработано {1} записей. Данные не сохранены.", semantic, processedRecCount));
                throw;
            }
            catch (Exception ex)
            {
                WriteEventIntoProcessDataProtocol(
                    ProcessDataEventKind.pdeFinishedWithErrors, string.Format(
                        "Обработка данных {0} завершена с ошибками: {1}. Обработано {2} записей. Данные не сохранены.", semantic, ex.Message, processedRecCount));
                throw;
            }
        }

        #region заполнение классификатора Расходы

        private string GetBudgetOutcomesClsCode(string kvsrCode, string fkrCode, string kcsrCode, string kvrCode, string dirCode, string kosguCode)
        {
            if (this.DataSource.Year <= 2006)
                return string.Format("{0}{1}{2}{3}{4}", kvsrCode, fkrCode, kcsrCode, kvrCode, dirCode).TrimStart('0').PadLeft(1, '0');
            else
                return string.Format("{0}{1}{2}{3}{4}", kvsrCode, fkrCode, kcsrCode, kvrCode, kosguCode).TrimStart('0').PadLeft(1, '0');
        }

        private const string NPA = "АС Бюджет - {0}";
        private int PumpBudgetOutcomesClsRow(string kvsrCode, string fkrCode, string kcsrCode, 
            string kvrCode, string name, int parentId, string dirCode, string kosguCode, int hierLevel)
        {
            string budgetOutcomesClsCode = GetBudgetOutcomesClsCode(kvsrCode, fkrCode, kcsrCode, kvrCode, dirCode, kosguCode);
            string section = fkrCode.Substring(0, 2);
            string subSection = fkrCode.Substring(2, 2);
            string npa = string.Format(NPA, this.DataSource.Year);

            if (dirCode == string.Empty)
                dirCode = "0";
            object[] mapping = new object[] { "NormativeAct", npa, "CodeStr", budgetOutcomesClsCode, "KVSR", kvsrCode, 
                "FKR", fkrCode, "KCSR", kcsrCode, "KVR", kvrCode, "Name", name, "Section", section, 
                "Subsection", subSection, "Direction", dirCode, "HierarchyLevel", hierLevel };
            if ((this.DataSource.Year > 2006) && (kosguCode != string.Empty))
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "KOSGU", kosguCode });


            string cacheKey = string.Format("{0}|", budgetOutcomesClsCode);
            if (parentId != -1)
            {
                mapping = (object[])CommonRoutines.ConcatArrays(mapping, new object[] { "ParentId", parentId });
                cacheKey += parentId.ToString();
            }
            return PumpCachedRow(budgetOutcomesClsCache, clsBudgetOutcomesCls, dsBudgetOutcomesCls.Tables[0],
                cacheKey, mapping, true);
        }

        private string GetClsNameByCode(int code, DataTable dt)
        {
            DataRow[] rows = dt.Select(string.Format("Code={0}", code));
            if (rows.GetLength(0) == 0)
                return constDefaultClsName;
            else
                return rows[0]["Name"].ToString();
        }

        private int FindOutcomesClsIdByCode(string code)
        {
            foreach (KeyValuePair<string, DataRow> item in budgetOutcomesClsCache)
                if (item.Key.Split('|')[0] == code)
                    return Convert.ToInt32(item.Value["Id"]);
            return errorBudgetOutcomesCls;
        }

        private string GetOutcomesClsName(string[] codes, string[] names)
        {
            string name = constDefaultClsName;
            for (int i = 0; i < codes.GetLength(0); i++)
            {
                string code = codes[i];
                if (code.TrimStart('0') != string.Empty)
                {
                    name = names[i];
                    break;
                }
            }
            return name;
        }

        private const string NULL_FKR_CODE = "0000";
        private const string NULL_KCSR_CODE = "0000000";
        private const string NULL_KVR_CODE = "000";
        private const string NULL_DIR_CODE = "000";
        private const string NULL_KOSGU_CODE = "000";
        private int PumpBudgetOutcomesClsRows(DataRow factRow, bool isTreasury)
        {
            DataRow clsRow = FindCachedRow(kvsrCache, Convert.ToInt32(factRow["RefKVSR"]));
            if (clsRow == null)
                return nullBudgetOutcomesCls;
            string kvsrCode = clsRow["Code"].ToString().PadLeft(3, '0');
            string kvsrName = clsRow["Name"].ToString();

            clsRow = FindCachedRow(fkrCache, Convert.ToInt32(factRow["RefFKR"]));
            if (clsRow == null)
                return nullBudgetOutcomesCls;
            string fkrCode = clsRow["Code"].ToString().PadLeft(4, '0');
            string fkrName = clsRow["Name"].ToString();

            clsRow = FindCachedRow(kcsrCache, Convert.ToInt32(factRow["RefKCSR"]));
            if (clsRow == null)
                return nullBudgetOutcomesCls;
            string kcsrCode = clsRow["Code"].ToString().PadLeft(7, '0');
            string kcsrName = clsRow["Name"].ToString();

            clsRow = FindCachedRow(kvrCache, Convert.ToInt32(factRow["RefKVR"]));
            if (clsRow == null)
                return nullBudgetOutcomesCls;
            string kvrCode = clsRow["Code"].ToString().PadLeft(3, '0');
            string kvrName = clsRow["Name"].ToString();

            string dirCode = string.Empty;
            string dirName = string.Empty;
            string kosguCode = string.Empty;
            string kosguName = string.Empty;
            if (this.DataSource.Year <= 2006)
            {
                clsRow = FindCachedRow(directionCache, Convert.ToInt32(factRow["RefDirection"]));
                if (clsRow == null)
                    return nullBudgetOutcomesCls;
                dirCode = clsRow["Code"].ToString().PadLeft(3, '0');
                dirName = clsRow["Name"].ToString();
            }
            else
            {
                clsRow = FindCachedRow(ekrCache, Convert.ToInt32(factRow["RefEKR"]));
                if (clsRow == null)
                    return nullBudgetOutcomesCls;
                kosguCode = clsRow["Code"].ToString().PadLeft(3, '0');
                kosguName = clsRow["Name"].ToString();
            }

            string nullDirCode = string.Empty;
            string nullKosguCode = string.Empty;
            if (this.DataSource.Year <= 2006)
                nullDirCode = NULL_DIR_CODE;
            else
                nullKosguCode = NULL_KOSGU_CODE;

            if (!isTreasury)
            {
                // 1 - создаем запись с нулевыми кодами кроме квср
                int parentId = PumpBudgetOutcomesClsRow(kvsrCode, NULL_FKR_CODE, NULL_KCSR_CODE, NULL_KVR_CODE, kvsrName, -1, nullDirCode, nullKosguCode, 1);
                // 2 - последние два кода фкр заменяем на нули, добавляем запись
                string parentFkrCode = string.Format("{0}{1}", fkrCode.Substring(0, 2), "00");
                string parentFkrName = GetClsNameByCode(Convert.ToInt32(parentFkrCode.TrimStart('0').PadLeft(1, '0')), dsFKR.Tables[0]);
                parentId = PumpBudgetOutcomesClsRow(kvsrCode, parentFkrCode, NULL_KCSR_CODE, NULL_KVR_CODE, parentFkrName, parentId, nullDirCode, nullKosguCode, 2);
                // 3 - добавляем запись с заполненным квср и фкр
                parentId = PumpBudgetOutcomesClsRow(kvsrCode, fkrCode, NULL_KCSR_CODE, NULL_KVR_CODE, fkrName, parentId, nullDirCode, nullKosguCode, 3);
                // 4 - последние 4 кода кцср заменяем на нули, добавляем запись
                string parentKcsrCode = string.Format("{0}{1}", kcsrCode.Substring(0, 3), "0000");
                string parentKcsrName = GetClsNameByCode(Convert.ToInt32(parentKcsrCode.TrimStart('0').PadLeft(1, '0')), dsKCSR.Tables[0]);
                parentId = PumpBudgetOutcomesClsRow(kvsrCode, fkrCode, parentKcsrCode, NULL_KVR_CODE, parentKcsrName, parentId, nullDirCode, nullKosguCode, 4);
                // 5 - последние 2 кода кцср заменяем на нули, добавляем запись
                parentKcsrCode = string.Format("{0}{1}", kcsrCode.Substring(0, 5), "00");
                parentKcsrName = GetClsNameByCode(Convert.ToInt32(parentKcsrCode.TrimStart('0').PadLeft(1, '0')), dsKCSR.Tables[0]);
                parentId = PumpBudgetOutcomesClsRow(kvsrCode, fkrCode, parentKcsrCode, NULL_KVR_CODE, parentKcsrName, parentId, nullDirCode, nullKosguCode, 5);
                // 6 - добавляем запись с заполненным квср фкр и кцср
                parentId = PumpBudgetOutcomesClsRow(kvsrCode, fkrCode, kcsrCode, NULL_KVR_CODE, kcsrName, parentId, nullDirCode, nullKosguCode, 6);
                // 7 - добавляем запись с заполненным квср фкр кцср и квр
                parentId = PumpBudgetOutcomesClsRow(kvsrCode, fkrCode, kcsrCode, kvrCode, kvrName, parentId, nullDirCode, nullKosguCode, 7);
                if (this.DataSource.Year <= 2006)
                    // 8 - добавляем запись с всеми заполненными кодами + ссылку на направления
                    return PumpBudgetOutcomesClsRow(kvsrCode, fkrCode, kcsrCode, kvrCode, dirName, parentId, dirCode, nullKosguCode, 8);
                else if (withKosgu)
                    // 8 - добавляем запись с всеми заполненными кодами + ссылку на косгу
                    return PumpBudgetOutcomesClsRow(kvsrCode, fkrCode, kcsrCode, kvrCode, kosguName, parentId, nullDirCode, kosguCode, 8);
                else
                    return parentId;
            }
            else
            {
                string budgetOutcomesClsCode = string.Empty;
                if (this.DataSource.Year <= 2006)
                    budgetOutcomesClsCode = GetBudgetOutcomesClsCode(kvsrCode, fkrCode, kcsrCode, kvrCode, dirCode, nullKosguCode);
                else if (withKosgu)
                    budgetOutcomesClsCode = GetBudgetOutcomesClsCode(kvsrCode, fkrCode, kcsrCode, kvrCode, nullDirCode, kosguCode);
                else
                    budgetOutcomesClsCode = GetBudgetOutcomesClsCode(kvsrCode, fkrCode, kcsrCode, kvrCode, nullDirCode, nullKosguCode);
                int refOutcomes = FindOutcomesClsIdByCode(budgetOutcomesClsCode);
                if (refOutcomes == errorBudgetOutcomesCls)
                {
                    // добавляем эту запись в группу ошибочных
                    if (this.DataSource.Year <= 2006)
                    {
                        string outcomesClsName = GetOutcomesClsName(new string[] { dirCode, kvrCode, kcsrCode, fkrCode, kvsrCode },
                                                                    new string[] { dirName, kvrName, kcsrName, fkrName, kvsrName });
                        refOutcomes = PumpBudgetOutcomesClsRow(kvsrCode, fkrCode, kcsrCode, kvrCode, outcomesClsName, errorBudgetOutcomesCls, dirCode, nullKosguCode, 2);
                    }
                    else if (withKosgu)
                    {
                        string outcomesClsName = GetOutcomesClsName(new string[] { kosguCode, kvrCode, kcsrCode, fkrCode, kvsrCode },
                                                                    new string[] { kosguName, kvrName, kcsrName, fkrName, kvsrName });
                        refOutcomes = PumpBudgetOutcomesClsRow(kvsrCode, fkrCode, kcsrCode, kvrCode, outcomesClsName, errorBudgetOutcomesCls, nullDirCode, kosguCode, 2);
                    }
                    else
                    {
                        string outcomesClsName = GetOutcomesClsName(new string[] { kvrCode, kcsrCode, fkrCode, kvsrCode },
                                                                    new string[] { kvrName, kcsrName, fkrName, kvsrName });
                        refOutcomes = PumpBudgetOutcomesClsRow(kvsrCode, fkrCode, kcsrCode, kvrCode, outcomesClsName, errorBudgetOutcomesCls, nullDirCode, nullKosguCode, 2);
                    }
                }
                return refOutcomes;
            }
        }

        #endregion заполнение классификатора Расходы

        private void FillOutcomesRepCode()
        {
            foreach (DataRow row in dsBudgetOutcomesCls.Tables[0].Rows)
            {
                if (Convert.ToInt32(row["Fkr"]) == -1)
                    continue;
                row["RepCode"] = string.Format("{0}{1}{2}", row["FKR"].ToString().PadLeft(4, '0'),
                    row["KCSR"].ToString().PadLeft(7, '0'), row["KVR"].ToString().PadLeft(3, '0'));
            }
        }

        private void FillCache()
        {
            FillRowsCache(ref budgetOutcomesClsCache, dsBudgetOutcomesCls.Tables[0], new string[] { "CodeStr", "ParentId" }, "|" );

            FillRowsCache(ref kvrCache, dsKVR.Tables[0], "ID");
            FillRowsCache(ref kvsrCache, dsKVSR.Tables[0], "ID");
            FillRowsCache(ref meansTypeCache, dsMeansType.Tables[0], "ID");
            FillRowsCache(ref factClsCache, dsFact.Tables[0], "ID");
            FillRowsCache(ref directionCache, dsDirection.Tables[0], "ID");

            FillRowsCache(ref fkrCache, dsFKR.Tables[0], "ID");
            FillRowsCache(ref ekrCache, dsEKR.Tables[0], "ID");
            FillRowsCache(ref kcsrCache, dsKCSR.Tables[0], "ID");
            FillRowsCache(ref subEkrCache, dsSubKESR.Tables[0], "ID");
        }

        protected override void QueryDataForProcess()
        {
            if ((this.StagesQueue[PumpProcessStates.PumpData].IsExecuted && (this.PumpMode == BudgetDataPumpMode.Full)) ||
                !this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
            {
                DeleteTableData(clsBudgetOutcomesCls, string.Empty);
            }

            InitClsDataSet(ref daFKR, ref dsFKR, clsFKR, false, string.Empty);
            InitClsDataSet(ref daKCSR, ref dsKCSR, clsKCSR, false, string.Empty);
            InitClsDataSet(ref daKVR, ref dsKVR, clsKVR, false, string.Empty);
            InitClsDataSet(ref daMeansType, ref dsMeansType, clsMeansType, false, string.Empty);
            InitClsDataSet(ref daEKR, ref dsEKR, clsEKR, false, string.Empty);
            InitClsDataSet(ref daDirection, ref dsDirection, clsDirection, false, string.Empty);
            InitClsDataSet(ref daSubKESR, ref dsSubKESR, clsSubKESR, false, string.Empty);
            InitClsDataSet(ref daKVSR, ref dsKVSR, clsKVSR, false, string.Empty);
            InitClsDataSet(ref daDirection, ref dsDirection, clsDirection, false, string.Empty);
            InitClsDataSet(ref daFact, ref dsFact, clsFact, false, string.Empty);
            InitDataSet(ref daBudgetOutcomesCls, ref dsBudgetOutcomesCls, clsBudgetOutcomesCls, false,
                string.Format("SOURCEID = {0} and rowType = 0", this.SourceID), string.Empty);

            FillCache();
        }

        private void DoUniqueNamesOutcomesCls()
        {
            string query = string.Empty;
            if (this.ServerDBMSName == DBMSName.Oracle)
            {
                query = string.Format("update d_R_Budget t1 set t1.name = concat(t1.id, t1.name) where t1.sourceid = {0} and " +
                    "exists (select * from d_R_Budget t2 where t1.name = t2.name and t1.id <> t2.id and t2.sourceid = {0} and t1.KVSR = t2.KVSR and t1.FKR = t2.FKR)", this.SourceID);
            }
            else
            {
                query = string.Format("update d_R_Budget set d_R_Budget.name = cast(d_R_Budget.id as varchar) + d_R_Budget.name " +
                    "where d_R_Budget.sourceid = {0} and exists (select * from d_R_Budget t where d_R_Budget.name = t.name and d_R_Budget.id <> t.id " + 
                    "and t.sourceid = {0} and d_R_Budget.KVSR=t.KVSR and d_R_Budget.FKR=t.FKR)", this.SourceID);
            }
            this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
        }

        private bool HasHierarchy(IEntity cls)
        {
            foreach (IDataAttribute attr in cls.Attributes.Values)
            {
                if (attr.Name.ToUpper() == "PARENTID")
                    return true;
            }
            return false;
        }

        private void DeleteUnusedCls(IEntity cls, string[] refClsParams)
        {
            string query = string.Empty;
            if (HasHierarchy(cls))
            {
                query = string.Format("update {0} set parentId = null where sourceid = {1}", cls.FullDBName, this.SourceID);
                this.DB.ExecQuery(query, QueryResultTypes.NonQuery, new IDbDataParameter[] { });
            }
            query = string.Format("delete from {0} where sourceid = {1} and ", cls.FullDBName, this.SourceID);
            for (int i = 0; i < refClsParams.GetLength(0); i += 2)
            {
                query += string.Format("id not in (select distinct {0} from {1} where sourceid = {2}) ", 
                    refClsParams[i + 1], refClsParams[i], this.SourceID);
                if (i != refClsParams.GetLength(0) - 2)
                    query += " and ";
            }
            this.DB.ExecQuery(query, QueryResultTypes.Scalar, new IDbDataParameter[] { });
        }

        protected override void ProcessDataSource()
        {
            SetPresentationContext(clsBudgetOutcomesCls);
            nullBudgetOutcomesCls = clsBudgetOutcomesCls.UpdateFixedRows(this.DB, this.SourceID);

            string npa = string.Format(NPA, this.DataSource.Year);
            object[] mapping = new object[] { "NormativeAct", npa, "CodeStr", 0, "KVSR", 0, 
                "FKR", 0, "KCSR", 0, "KVR", 0, "Name", "Ошибочные данные", "Section", 0, "Subsection", 0, "HierarchyLevel", 1 };
            string cacheKey = "0|";
            errorBudgetOutcomesCls = PumpCachedRow(budgetOutcomesClsCache, clsBudgetOutcomesCls,
                dsBudgetOutcomesCls.Tables[0], cacheKey, mapping, true);

            FillFactOutcomesCls(fctBudgetData, 1);
            FillFactOutcomesCls(fctFacialFinDetail, 2);
            UpdateData();

            // проставляем поле "код_отчет" расходного классификатора
            FillOutcomesRepCode();
            UpdateData();

            // делаем уникальные наименования в расходном клс
            DoUniqueNamesOutcomesCls();

            // удаляем из клс записи, которые не используются в фактах
            DeleteUnusedCls(clsKVSR, new string[] { fctIncomes32.FullDBName, "REFKVSRINCOME", fctIncomesPlan.FullDBName, "RefKVSR", 
                                                               fctBudgetData.FullDBName, "RefKVSR", fctFacialFinDetail.FullDBName, "RefKVSR",
                                                               fctFinDocDetail.FullDBName, "RefKVSR" });
            DeleteUnusedCls(clsFact, new string[] { fctBudgetData.FullDBName, "RefFact", fctFacialFinDetail.FullDBName, "RefFact", 
                                                               fctFinDocDetail.FullDBName, "RefFact" });
        }

        protected override void UpdateProcessedData()
        {
            UpdateData();
        }

        protected override void ProcessFinalizing()
        {
            PumpFinalizing();
        }

        private int GetYearProcessParam()
        {
            int year = -1;
            if (this.StagesQueue[PumpProcessStates.PumpData].IsExecuted)
                return -1;
            string str = GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "umeYears", string.Empty);
            if (str != string.Empty)
                year = Convert.ToInt32(str);
            return year;
        }

        protected override void DirectProcessData()
        {
            withKosgu = Convert.ToBoolean(GetParamValueByName(this.PumpRegistryElement.ProgramConfig, "cbWithKosgu", "False"));
            int year = GetYearProcessParam();
            ProcessDataSourcesTemplate(year, 0, "Заполнение классификатора показателей АС Бюджет");
        }

        #endregion Обработка данных

        #region проверка закачанных сумм

        protected override void QueryDataForCheck()
        {
        }

        private string GetFactRowValues(DataRow row, string[] fieldNames)
        {
            string rowValues = string.Empty;
            foreach (string fieldName in fieldNames)
                rowValues += string.Format("{0}|", row[fieldName]);
            return rowValues.Remove(rowValues.Length - 1);
        }

        // получить кэш - данные факта, задаваемые массивом полей
        private Dictionary<int, string> GetFactCache(IEntity fct, string constraint, string keyFieldName, string[] fieldNames)
        {
            Dictionary<int, string> cache = new Dictionary<int, string>();
            int totalRecs = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select count(id) from {0} where {1}", fct.FullDBName, constraint), QueryResultTypes.Scalar));
            if (totalRecs == 0)
                return cache;
            int firstID = Convert.ToInt32(this.DB.ExecQuery(string.Format(
                "select min(id) from {0} where {1}", fct.FullDBName, constraint), QueryResultTypes.Scalar));
            int lastID = firstID + MAX_DS_RECORDS_AMOUNT * 2 - 1;
            int processedRecCount = 0;
            IDbDataAdapter da = null;
            DataSet ds = null;
            do
            {
                string idConstr = string.Format("ID >= {0} and ID <= {1} and {2}", firstID, lastID, constraint);
                firstID = lastID + 1;
                lastID += MAX_DS_RECORDS_AMOUNT * 2;
                InitDataSet(ref da, ref ds, fct, idConstr);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    processedRecCount++;
                    int key = Convert.ToInt32(row[keyFieldName]);
                    // в конце приписываем ноль - понадобится дальше при проверке наших данных с бюджетовскими
                    string value = string.Format("{0}|0", GetFactRowValues(row, fieldNames));
                    if (!cache.ContainsKey(key))
                        cache.Add(key, value);
                }
                ClearDataSet(ref ds);
            }
            while (processedRecCount < totalRecs);
            return cache;
        }

        private void CheckTreasury()
        {
            fmTreasuryCache = GetFactCache(fctFacialFinDetail,
                string.Format(" SourceId = {0} ", this.SourceID), "SourceKey", new string[] { "Id" });
            try
            {
                // ОЛОЛО БЛЯ!!!! АДЫН АДЫН!!!11  
                // ололо бля, то же самое что и закачка, только вместо собственно закачки строк - проверяем их, пездец.
                // проверка базы бюджета с нашей базой идет в процедуре PumpTreasuryRow
                // то есть в ней выводятся в лог записи, которые были добавлены в базу бюджета после последней закачки,
                // это те, записи, которых у нас нет.
                // также в этой процедуре в кэше fmTreasuryCache помечаются записи, которые присутствуют в нашей базе
                // это нужно для обратной сверки - от нашей базы к бюджетовской. 
                // те записи, которые не пометились - в базе бюджета отсутствуют, их удалили
                PumpTreasuryData();
                // теперь идем по нашим записям - смотрим те, которых нет в базе бюджета
                foreach (KeyValuePair<int, string> item in fmTreasuryCache)
                {
                    string value = item.Value;
                    if (value[value.Length - 1] == '1')
                        continue;
                    string rowId = item.Value.Split('|')[0];
                    string message = string.Empty;
                    if ((value[value.Length - 1] == '0'))
                    {
                        message = string.Format("Запись с ID '{0}', SourceKey '{1}' отсутствует в выборке базы данных бюджета или не удовлетворяет условиям закачки.", rowId, item.Key);
                        CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeWarning, message, this.PumpID, this.SourceID);
                    }
//                    else if ((value[value.Length - 1] == '2'))
 //                   {
                        // 2
   //                     message = string.Format("Запись с ID '{0}', SourceKey '{1}' не удовлетворила условиям закачки.", rowId, item.Key);
    //                    CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeWarning, message, this.PumpID, this.SourceID);
     //               }
                }
            }
            finally
            {
                fmTreasuryCache.Clear();
            }
        }

        private FileInfo GetUdlFle()
        {
            FileInfo[] udls = this.RootDir.GetFiles("*.udl", SearchOption.AllDirectories);
            foreach (FileInfo udl in udls)
            {
                string dirYear = udl.Directory.Name;
                if (dirYear == this.DataSource.Year.ToString())
                    return udl;
            }
            return null;
        }

        protected override void CheckDataSource()
        {
            FileInfo udl = GetUdlFle();
            if (udl == null)
                throw new Exception("В папках источников не найден подходящий udl файл.");
            if (!InitBudgetDB(udl))
                throw new DataSourceIsCorruptException("Ошибка при подключении к базе бюджета.");
            this.BudgetRef = Convert.ToInt32(this.BudgetRefs[0]);
            CheckDataProtocol.WriteEventIntoReviseDataProtocol(ReviseDataEventKind.pdeInformation,
                string.Format("Проверка данных базы АС Бюджет {0} ({1}).", this.DatabasePath, udl.Name), this.PumpID, this.SourceID);
            CheckTreasury();
        }

        protected override void DirectCheckData()
        {
            int year = GetYearProcessParam();
            CheckDataSourcesTemplate(year, -1, "Проверка закачанных сумм.");
        }

        #endregion проверка закачанных сумм

    }
}