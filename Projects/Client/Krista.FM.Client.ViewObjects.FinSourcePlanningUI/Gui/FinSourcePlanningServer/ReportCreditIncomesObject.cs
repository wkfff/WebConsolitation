using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.ServerLibrary;
using System.Data;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using System.Collections.ObjectModel;
using System.IO;

// TODO List

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    /// <summary>
    /// Запросы в базу
    /// </summary>
    public class CreditSQLObject
    {
        /// <summary>
        /// Собиратель фильтров
        /// </summary>
        public static string CombineFilter(string filterValue, string paramName, string paramValue)
        {
            string paramString = string.Empty;
            if (paramValue != string.Empty)
            {
                if (filterValue != string.Empty)
                {
                    filterValue = string.Format("{0} and ", filterValue);
                }
                // Строковое
                if (paramValue[0].ToString() == "'")
                {
                    paramValue = paramValue.Remove(0, 1);
                    paramValue = paramValue.Remove(paramValue.Length - 1, 1);
                    paramString = string.Format("(c.{1} like '%{0}%')", paramValue, paramName);
                }
                // Дата
                else if (paramValue[0].ToString() == ">" 
                    || paramValue[0].ToString() == "<"
                    || paramValue[0].ToString() == "=")
                {
                    string[] paramListValues = paramValue.Split(';');
                    for (int i = 0; i < paramListValues.Length; i++)
                    {
                        paramString = string.Format("{2}(c.{1} {0}) and ", paramListValues[i], paramName, paramString);
                    }
                    paramString = paramString.Remove(paramString.Length - 4, 4);
                }
                else
                {
                    paramString = string.Format("(c.{1} in ({0}))", paramValue, paramName);
                }
            }
            return string.Format("{0}{1}", filterValue, paramString);
        }

        /// <summary>
        /// Основной фильтр таблицы фактов
        /// </summary>
        public static string GetMainSQLText(Dictionary<string, string> mainFilter)
        {
            string conditionValue = string.Empty;
            foreach (string key in mainFilter.Keys)
            {
                conditionValue = CombineFilter(conditionValue, key, mainFilter[key]);
            }
            return conditionValue.Replace("c.", string.Empty);
        }
    }

    // Тип колонки
    public enum FieldType : int
    {
        ftMain = 0, // Поле из таблицы фактов
        ftCalc = 1, // Обработанное поле таблицы фактов / прочая хрень        
    }

    // Тип вычисляемой колонки
    public enum CalcColumnType : int
    {
        cctContractNum = 0,
        cctOrganization = 1,
        cctDetail = 2,
        cctOKVValue = 3,
        cctOKVName = 4,
        cctCreditEndDate = 5,
        cctPercentText = 6,
        cctCurrentRest = 7,
        cctPosition = 8,
        cctContractDesc = 9,
        cctCollateralType = 10,
        cctOrganization3 = 11,
        cctRegress = 12,
        cctNumStartDate = 13,
        cctOrganizations = 14,
        cctCalcSum = 15,
        cctCapNum = 16,
        cctCapNameKind = 17,
        cctCapForm = 18,
        cctCapRegDateNum = 19,
        cctCapNPANames = 20,
        cctMinOperationDate = 21,
        cctAllOperationDates = 22,
        cctContractType = 23,
        cctNumContractDate = 24,
        cctContractNum2 = 25,
        cctContractNum3 = 26,
        cctOrganizationRegion = 27,
        cctSortStatus = 28,
        cctRegress2 = 29,
        cctPrincipalDoc = 30,
        cctPrincipalStartDate = 31,
        cctPrincipalEndDate = 32,
        cctRegion = 33,
        cctContractNum4 = 34,
        cctPercentTextMaxMin = 35,
        cctDetailText = 36,
        cctNumStartDate2 = 37,
        cctRelation = 38,
        cctNumContractDatePercent = 39,
        cctCapNumDateDiscount = 40,
        cctGrntNumRegPercent = 41,
        cctGarantEndDate = 42,
        cctReportPeriod = 43,
        cctPurposeActNumDate = 44,
        cctCapNumberRegDate = 45,
        cctCapPayPeriod = 48,
        cctCreditYear = 49,
        cctCapCoupon = 50,
        cctPercentValues = 53,
        cctGarantCalcSum = 54,
        cctCapOffNumNPANameDateNum = 55,
        cctCapLabelPurpose = 56,
        cctOrgPurpose = 57,
        cctSubCreditCaption = 58,
        cctPercentValues2 = 59,
        cctNumDateOKV = 60,
        cctCreditTypeNumDate = 61,
        cctGarantTypeNumDate = 62,
        cctCreditTypeNumStartDate = 63, 
        cctNativeSum = 64,
        cctGenOrg = 65, 
        cctCreditIssNumDocDate = 66,
        cctUndefined = -999,
    }

    // объект для предоставленных кредитов
    class CreditIssuedDataObject : CreditDataObject
    {
        protected override string[] GetDetailKeys()
        {
            return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}",
                CreditIssuedObjectsKeys.a_S_PlanAttractCO_RefCreditInc_Key,
                CreditIssuedObjectsKeys.a_S_PlanDebtCO_RefCreditInc_Key,
                CreditIssuedObjectsKeys.a_S_PlanServiceCO_RefCreditInc_Key,
                CreditIssuedObjectsKeys.a_S_FactAttractCO_RefCreditInc_Key,
                CreditIssuedObjectsKeys.a_S_FactDebtCO_RefCreditInc_Key,
                CreditIssuedObjectsKeys.a_S_ChargePenaltyDebtCO_RefCreditInc_Key,
                CreditIssuedObjectsKeys.a_S_ChargePenaltyPercentCO_RefCreditInc_Key,
                CreditIssuedObjectsKeys.a_S_FactPercentCO_RefCreditInc_Key,
                CreditIssuedObjectsKeys.a_S_FactPenaltyDebtCO_RefCreditInc_Key,
                CreditIssuedObjectsKeys.a_S_FactPenaltyPercentCO_RefCreditInc_Key).Split(';');
        }

        protected override string[] GetStartDates()
        {
            return "StartDate;StartDate;StartDate;;;StartDate;StartDate;;;".Split(';');
        }

        protected override string[] GetEndDates()
        {
            return "EndDate;EndDate;EndDate;FactDate;FactDate;;;FactDate;FactDate;FactDate".Split(';');
        }

        protected override string GetContractDesc()
        {
            return "Кредит предоставленный";
        }

        public override string GetParentRefName()
        {
            return "RefCreditInc";
        }

        protected override string GetCollateralKey()
        {
            return CreditIssuedObjectsKeys.t_S_CollateralCO;
        }

        protected override string GetMainTableKey()
        {
            return SchemeObjectsKeys.f_S_Creditissued_Key;
        }
        
        protected override string GetJournalKey()
        {
            return CreditIssuedObjectsKeys.t_S_JournalPercentCO;
        }
    }

    // объект для гарантий
    class GarantDataObject : CreditDataObject
    {
        protected override string[] GetDetailKeys()
        {
            return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12}",
                SchemeObjectsKeys.a_S_FactAttractPrGrnt_RefGrnt_Key,
                SchemeObjectsKeys.a_S_FactDebtPrGrnt_RefGrnt_Key,
                SchemeObjectsKeys.a_S_FactAttractGrnt_RefGrnt_Key,
                GuaranteeIssuedObjectKeys.a_S_PlanDebtPrGrnt_Key,
                GuaranteeIssuedObjectKeys.a_S_FactPercentPrGrnt_RefGrnt_Key,
                GuaranteeIssuedObjectKeys.a_S_PlanServicePrGrnt_Key,
                GuaranteeIssuedObjectKeys.a_S_PlanAttractGrnt_RefGuarantIsd_Key,
                GuaranteeIssuedObjectKeys.a_S_PlanAttractPrGrnt_RefGrnt_Key,
                GuaranteeIssuedObjectKeys.a_S_ChargePenaltyDebtPrGrnt_RefGrnt_Key,
                SchemeObjectsKeys.a_S_FactPenaltyPercentPrGrnt_RefGrnt_Key,
                GuaranteeIssuedObjectKeys.a_S_PrincipalContrGrnt_RefGrnt_Key,
                GuaranteeIssuedObjectKeys.a_S_PrGrntChargePenaltyPercent_RefGrnt_Key,
                SchemeObjectsKeys.a_S_FactPenaltyDebtPrGrnt_RefGrnt_Key
                ).Split(';');
        }

        protected override string[] GetStartDates()
        {
            return ";;;;;StartDate;StartDate;;;;;StartDate;".Split(';');
        }

        protected override string[] GetEndDates()
        {
            return "FactDate;FactDate;FactDate;EndDate;FactDate;EndDate;EndDate;StartDate;StartDate;FactDate;EndDate;StartDate;FactDate".Split(';');
        }

        protected override string GetContractDesc()
        {
            return "Гарантия";
        }

        public override string GetParentRefName()
        {
            return "RefGrnt";
        }

        protected override string GetCollateralKey()
        {
            return GuaranteeIssuedObjectKeys.t_S_CollateralGrnt_Key;
        }

        protected override string GetMainTableKey()
        {
            return SchemeObjectsKeys.f_S_Guarantissued_Key;
        }

        protected override string GetJournalKey()
        {
            return GuaranteeIssuedObjectKeys.t_S_JournalPercentGrnt_Key;
        }
    }

    // объект для ценных бумаг
    class CapitalDataObject : CreditDataObject
    {
        protected override string[] GetDetailKeys()
        {
            return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11}",
                CapitalObjectKeys.a_S_CPFactCapital_RefCap_Key,
                CapitalObjectKeys.a_S_CPFactDebt_RefCap_Key,
                CapitalObjectKeys.a_S_CPRateSwitch_RefCap_Key,
                CapitalObjectKeys.a_S_CPFactService_RefCap_Key,
                CapitalObjectKeys.a_S_CPFactCost_RefCap_Key,
                CapitalObjectKeys.a_S_CPPlanCapital_RefCap_Key,
                CapitalObjectKeys.a_S_CPPlanDebt_RefCap_Key,
                CapitalObjectKeys.a_S_CPPlanService_RefCap_Key,
                CapitalObjectKeys.a_S_CPChargePenaltyDeb_RefCap_Key,
                CapitalObjectKeys.a_S_CPFactPenaltyCap_RefCap_Key,
                CapitalObjectKeys.a_S_CPFactPenaltyPer_RefCap_Key,
                CapitalObjectKeys.a_S_CPChargePenaltyPer_RefCap_Key
                ).Split(';');
        }

        protected override string[] GetStartDates()
        {
            return ";;;PlanDate;;StartDate;;;;;;StartDate".Split(';');
        }

        protected override string[] GetEndDates()
        {
            return "DateDoc;DateDischarge;DateCharge;FactDate;CostDate;EndDate;EndDate;EndDate;StartDate;FactDate;FactDate;".Split(';');
        }

        protected override string GetContractDesc()
        {
            return "Ценная бумага";
        }

        public override string GetParentRefName()
        {
            return "RefCap";
        }

        protected override string GetCollateralKey()
        {
            return CapitalObjectKeys.t_S_CPCollateral;
        }

        protected override string GetMainTableKey()
        {
            return SchemeObjectsKeys.f_S_Capital_Key;
        }

        protected override string GetJournalKey()
        {
            return CapitalObjectKeys.t_S_CPJournalPercent;
        }
    }

    // объект для полученных кредитов
    class CreditDataObject
    {
        public IScheme scheme = WorkplaceSingleton.Workplace.ActiveScheme;        
        // Хреново глобальные переменные текущей записи, но тащить кучу параметров в парсер формул желания нету
        private string creditEndDate;
        private int currencyType = -1;
        // Результирующий датасет
        private DataTable dtResult;
        // Фильтры таблицы фактов
        public Dictionary<string, string> mainFilter = new Dictionary<string, string>();
        //Парметры отчеты
        public Dictionary<string, string> reportParams = new Dictionary<string, string>();
        // Список колонок, по которым считаем итоги
        public Collection<int> summaryColumnIndex = new Collection<int>();
        // Счетчик колонок
        private int columnCount;
        // Условия заполнения колонок
        public Dictionary<int, string> columnCondition = new Dictionary<int, string>();
        // Параметры колонок(тип, подтип и параметры)
        public Dictionary<int, FieldType> columnList = new Dictionary<int, FieldType>();
        private Dictionary<int, CalcColumnType> calcColumnTypes = new Dictionary<int, CalcColumnType>();
        public Dictionary<int, Dictionary<string, string>> columnParamList = new Dictionary<int, Dictionary<string, string>>();
        // Куча данных всех используемых деталей
        public DataTable[] dtDetail;
        // Данные журнала процентов по все договорам попадающим в отчет
        public DataTable dtJournalPercent = new DataTable();
        // Общий список курсов валют договора с которыми попадают в отчет
        public DataTable dtExchange = new DataTable();
        // Строка сортировки результата
        public string sortString = string.Empty;
        // Нужно ли делать двухуровневую сортировку результата
        public bool hierarchicalSort = false;
        // Кэш валют : храним нужные курсы валют(чтобы не лазать за ними по каждому договору в базу)
        private Dictionary<int, DataTable> exchangeRate = new Dictionary<int,DataTable>();
        // Кэш справочников
        private Dictionary<string, DataTable> bookCache = new Dictionary<string, DataTable>();
        // Кэш валют 2(чтобы не искать каждый раз в датасетах)
        Collection<int> okvCodes = new Collection<int>();
        public Dictionary<int, double> okvValues = new Dictionary<int, double>();
        // Коллекция используемых деталей(чтобы не выкачивать данные по всем)
        Collection<int> usedDetails = new Collection<int>();
        // Производить ли пересчет сумм валютных договоров
        public bool ignoreCurrencyCalc = false;

        private bool writeFullDetailText = false;
        private string fullDetailText = string.Empty;

        // Нужно ли добавлять строку с итогами
        public bool useSummaryRow = true;

        // Текущая рассчитываемая колонка
        private int currentColumnIndex = -1;
        // Список полей для суммирования в текущей колонке
        private string currentFieldList = string.Empty;
        // Список полей для суммирования в текущей колонке
        private bool currentIgnoreExchenge = false;
        // Брать курс валюты на предыдущий день и более ранние
        public bool exchangePrevDay = false;
        // Убрать в итоговом датасете служебные поля
        public bool removeServiceFields = false;

        public Collection<DataRow> sumIncludedRows = new Collection<DataRow>();

        protected virtual string[] GetDetailKeys()
        {
            return string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}",
                        SchemeObjectsKeys.a_S_FactAttractCI_RefCreditInc_Key,
                        SchemeObjectsKeys.a_S_FactDebtCI_RefCreditInc_Key,
                        SchemeObjectsKeys.a_S_PlanDebtCI_RefCreditInc_Key,
                        SchemeObjectsKeys.a_S_PlanAttractCI_RefCreditInc_Key,
                        SchemeObjectsKeys.a_S_FactPercentCI_RefCreditInc_Key,
                        SchemeObjectsKeys.a_S_PlanServiceCI_RefCreditInc_Key,
                        SchemeObjectsKeys.a_S_ChargePenaltyDebtCI_RefCreditInc_Key,
                        SchemeObjectsKeys.a_S_ChargePenaltyPercentCI_RefCreditInc_Key,
                        SchemeObjectsKeys.a_S_FactPenaltyDebtCI_RefCreditInc_Key,
                        SchemeObjectsKeys.a_S_FactPenaltyPercentCI_RefCreditInc_Key,
                        SchemeObjectsKeys.a_S_RateSwitchCI_RefCreditIncome_Key
                        ).Split(';');
        }

        protected virtual string[] GetStartDates()
        {
            return ";;StartDate;StartDate;;StartDate;;;;;EndDate".Split(';');
        }

        protected virtual string[] GetEndDates()
        {
            return "FactDate;FactDate;EndDate;StartDate;FactDate;EndDate;StartDate;StartDate;FactDate;FactDate;DateCharge".Split(';');
        }

        protected virtual string GetMainSQLQuery()
        {
            return CreditSQLObject.GetMainSQLText(mainFilter);
        }

        protected virtual string GetContractDesc()
        {
            if (mainFilter["RefSTypeCredit"] == "1")
            {
                return "Бюджетный кредит";
            }
            return "Кредит";
        }

        public virtual string GetParentRefName()
        {
            return "RefCreditInc";
        }

        protected virtual string GetCollateralKey()
        {
            return SchemeObjectsKeys.t_S_CollateralCI;
        }

        protected virtual string GetMainTableKey()
        {
            return SchemeObjectsKeys.f_S_Сreditincome_Key;
        }

        protected virtual string GetJournalKey()
        {
            return SchemeObjectsKeys.t_S_JournalPercentCI_Key;
        }

        // Создатель таблиц строковых полей
        private void CreateExchangeTable(int columnCount)
        {
            dtExchange = new DataTable();
            for (int i = 0; i < columnCount; i++)
            {
                dtExchange.Columns.Add(string.Format("{0}", i), typeof(String));
            }
        }

        // Подчищатор и реинкарнатор объекта
        public void InitObject()
        {
            okvCodes.Clear();
            okvValues.Clear();
            summaryColumnIndex.Clear();
            exchangeRate.Clear();
            mainFilter.Clear();
            columnList.Clear();
            calcColumnTypes.Clear();
            columnParamList.Clear();
            reportParams.Clear();
            columnCondition.Clear();
            dtJournalPercent.Clear();
            usedDetails.Clear();

            columnCount = 0;

            CreateExchangeTable(3);
            
            hierarchicalSort = false;
            sortString = string.Empty;
            ignoreCurrencyCalc = false;
            useSummaryRow = true;
            currentColumnIndex = -1;
            currentFieldList = string.Empty;
            currentIgnoreExchenge = false;
            exchangePrevDay = false;
            removeServiceFields = false;
        }

        // Добавитель колонок таблицы фактов
        public void AddDataColumn(string colName, string typeName)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("Name", colName);
            param.Add("DataType", typeName);
            AddColumn(FieldType.ftMain, CalcColumnType.cctUndefined, param);
        }

        // Добавитель колонок таблицы фактов
        public void AddDataColumn(string colName)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("Name", colName);
            AddColumn(FieldType.ftMain, CalcColumnType.cctUndefined, param);
        }

        // Добавитель вычисляемых колонок 
        public void AddCalcColumn(CalcColumnType calcType)
        {
            AddColumn(FieldType.ftCalc, calcType, new Dictionary<string, string>());
        }

        // Добавитель вычисляемых колонок(с параметрами) 
        public void AddCalcColumn(CalcColumnType calcType, Dictionary<string, string> paramsList)
        {
            AddColumn(FieldType.ftCalc, calcType, paramsList);
        }

        // Добавитель колонок в список(общий)
        private void AddColumn(FieldType colType, CalcColumnType calcType, Dictionary<string, string> paramsList)
        {
            columnList.Add(columnCount, colType);
            calcColumnTypes.Add(columnCount, calcType);
            columnParamList.Add(columnCount, paramsList);
            columnCount++;
        }

        // Создание структуры для вывода
        private void CreateFields()
        {
            dtResult = new DataTable();
            for (int i = 0; i < columnCount; i++)
            {
                DataColumn dataColumn = dtResult.Columns.Add();
                string dataType = "String";
                // Все поля кроме суммируемых деталей будем считать строковыми
                if (calcColumnTypes[i] == CalcColumnType.cctDetail
                    || calcColumnTypes[i] == CalcColumnType.cctCurrentRest
                    || calcColumnTypes[i] == CalcColumnType.cctRelation)
                {
                    dataType = "Double";
                    if (!summaryColumnIndex.Contains(i)) summaryColumnIndex.Add(i);
                }
                if (calcColumnTypes[i] == CalcColumnType.cctSortStatus) dataColumn.ColumnName = "SortStatus";

                dataColumn.DataType = Type.GetType(string.Format("System.{0}", dataType));
                if (columnParamList[i].ContainsKey("Name"))
                {
                    if (!dtResult.Columns.Contains(columnParamList[i]["Name"]))
                    {
                        dataColumn.ColumnName = columnParamList[i]["Name"];
                    }
                }
                if (columnParamList[i].ContainsKey("DataType"))
                {
                    dataColumn.DataType = Type.GetType(string.Format("System.{0}", columnParamList[i]["DataType"]));
                }
            }
            // служебные колонки для сортировки(надо бы после формирования потереть)
            if (!dtResult.Columns.Contains("SortCreditEndDate")) dtResult.Columns.Add("SortCreditEndDate", typeof(DateTime));
            if (!dtResult.Columns.Contains("RefOKV")) dtResult.Columns.Add("RefOKV", typeof(Int32));
            if (!dtResult.Columns.Contains("ParentID")) dtResult.Columns.Add("ParentID", typeof(Int32));
            if (!dtResult.Columns.Contains("ID")) dtResult.Columns.Add("ID", typeof(Int32));
        }

        // Заполнение полей из БД
        private void FillDataField(DataRow destRow, DataRow sourceRow, int colIndex)
        {
            object value = sourceRow[columnParamList[colIndex]["Name"]];
            if (value != DBNull.Value)
            {
                if (sourceRow.Table.Columns[columnParamList[colIndex]["Name"]].DataType == typeof(DateTime))
                {
                    // с датами надо похитрее - конвертим в краткое представление без времени
                    destRow[colIndex] = Convert.ToDateTime(value).ToShortDateString();
                }
                else
                {
                    destRow[colIndex] = value;
                }
            }
        }

        private string GetDateValue(object dateCell)
        {
            string strDate = string.Empty;
            if (dateCell != DBNull.Value) 
                strDate = Convert.ToDateTime(dateCell).ToShortDateString();
            return strDate;
        }

        // Заполнение вычисляемых полей
        private void FillCalcField(DataRow destRow, DataRow sourceRow, int colIndex)
        {
            IScheme scheme = WorkplaceSingleton.Workplace.ActiveScheme;

            // Наименование договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPrincipalDoc)
            {
                IEntity principalBook = scheme.RootPackage.FindEntityByName(GuaranteeIssuedObjectKeys.t_S_PrincipalContrGrnt_Key);
                destRow[colIndex] = GetBookValue(principalBook, Convert.ToInt32(sourceRow["id"]), "PrincipalDoc", true);
            }

            // Дата выдачи
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPrincipalStartDate)
            {
                IEntity principalBook = scheme.RootPackage.FindEntityByName(GuaranteeIssuedObjectKeys.t_S_PrincipalContrGrnt_Key);
                destRow[colIndex] = GetBookValue(principalBook, Convert.ToInt32(sourceRow["id"]), "StartDate", true);
            }

            // Дата выдачи
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPrincipalEndDate)
            {
                IEntity principalBook = scheme.RootPackage.FindEntityByName(GuaranteeIssuedObjectKeys.t_S_PrincipalContrGrnt_Key);
                destRow[colIndex] = GetBookValue(principalBook, Convert.ToInt32(sourceRow["id"]), "EndDate", true);
            }

            // Мин. дата операции по договору
            if (calcColumnTypes[colIndex] == CalcColumnType.cctMinOperationDate)
            {
                DataRow[] drAttract = dtDetail[Convert.ToInt32(columnParamList[colIndex]["ParamValue1"])].
                    Select(string.Format("{0} = {1}", GetParentRefName(), sourceRow["id"]), "FactDate asc");
                if (drAttract.Length > 0)
                {
                    destRow[colIndex] = Convert.ToDateTime(drAttract[0]["FactDate"]).ToShortDateString();
                }
            }

            // Все даты операций по детали
            if (calcColumnTypes[colIndex] == CalcColumnType.cctAllOperationDates)
            {
                DataRow[] drAttract = dtDetail[Convert.ToInt32(columnParamList[colIndex]["ParamValue1"])].
                    Select(string.Format("{0} = {1}", GetParentRefName(), sourceRow["id"]), "FactDate asc");

                string dateStr = string.Empty;
                foreach (DataRow row in drAttract)
                {
                    dateStr = string.Format("{0}, {1}", dateStr, Convert.ToDateTime(row["FactDate"]).ToShortDateString());
                }
                destRow[colIndex] = dateStr.TrimStart(',').TrimStart(' ');
            }

            // Тип договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctContractType)
            {
                IEntity creditType = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_TypeContract_Key);
                destRow[colIndex] = 
                    GetBookValue(creditType, Convert.ToInt32(sourceRow["RefTypeContract"]), "name");
            }

            // Дата муниципальной регистрации для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapNPANames)
            {
                destRow[colIndex] = string.Format("{0} № {1} от {2} {3}",
                    sourceRow["NameNPA"], sourceRow["NumberNPA"], 
                    GetDateValue(sourceRow["DateNPA"]), sourceRow["NameOrg"]);
            }

            // Дата муниципальной регистрации для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapRegDateNum)
            {
                destRow[colIndex] = string.Format("{0}, {1}",
                    GetDateValue(sourceRow["RegEmissionDate"]), sourceRow["RegNumber"]);
            }

            // Номер ценной бумаги + Дата муниципальной регистрации
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapNumberRegDate)
            {
                destRow[colIndex] = string.Format("{1} от {0}",
                    GetDateValue(sourceRow["RegEmissionDate"]), sourceRow["NumberCapital"]);
            }

            // Периодичность выплат
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapPayPeriod)
            {
                GetSumValue(dtDetail[7], Convert.ToInt32(sourceRow["id"]), "EndDate", "Sum",
                    DateTime.MinValue, DateTime.MaxValue, true, true);
                if (sumIncludedRows.Count > 0)
                {
                    destRow[colIndex] = string.Format("1 раз в {0} день", sumIncludedRows[0]["Period"]);
                }
            }

            // Форма существования для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapForm)
            {
                IEntity formBook = scheme.RootPackage.FindEntityByName(CapitalObjectKeys.fx_S_FormCapital);
                string formName = GetBookValue(formBook, Convert.ToInt32(sourceRow["RefSFormCap"]), "name");
                destRow[colIndex] = formName;
            }

            // Номер + вид для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapNameKind)
            {
                IEntity kindBook = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_Capital_Key);
                string kindName = GetBookValue(kindBook, Convert.ToInt32(sourceRow["RefSCap"]), "name");
                destRow[colIndex] = string.Format("{0} {1}",
                    kindName, string.Empty);
            }

            // Заголовок для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapNumDateDiscount)
            {
                destRow[colIndex] = string.Format("№ {0} {1}, {2:N2} %",
                    sourceRow["OfficialNumber"], GetDateValue(sourceRow["RegEmissionDate"]), sourceRow["Discount"]);
            }

            // Заголовок для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctGrntNumRegPercent)
            {
                destRow[colIndex] = string.Format("№ {0} {1}, {2:N2}",
                    sourceRow["Num"], GetDateValue(sourceRow["RegDate"]), 
                    GetPercentText(sourceRow, DateTime.MinValue, DateTime.MaxValue, "CreditPercent", true, true, true));
            }

            // Заголовок для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapNum)
            {
                destRow[colIndex] = string.Format("{0}, {1}, {2}",
                    sourceRow["CodeCapital"], sourceRow["SeriesCapital"], sourceRow["NumberCapital"]);
            }

            // Сумма вычисляемая для валютных
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCalcSum)
            {
                if (currencyType == -1)
                {
                    destRow[colIndex] = sourceRow["Sum"];
                }
                else
                {
                    if (sourceRow["CurrencySum"] != DBNull.Value) 
                        destRow[colIndex] = okvValues[currencyType] * Convert.ToDouble(sourceRow["CurrencySum"]);
                }
            }

            // Сумма вычисляемая для валютных
            if (calcColumnTypes[colIndex] == CalcColumnType.cctNativeSum)
            {
                if (currencyType == -1)
                {
                    destRow[colIndex] = sourceRow["Sum"];
                }
                else
                {
                    destRow[colIndex] = sourceRow["CurrencySum"];
                }
            }

            // Сумма вычисляемая для валютных гарантий
            if (calcColumnTypes[colIndex] == CalcColumnType.cctGarantCalcSum)
            {
                if (currencyType == -1)
                {
                    destRow[colIndex] = sourceRow["DebtSum"];
                }
                else
                {
                    if (sourceRow["CurrencySum"] != DBNull.Value)
                        destRow[colIndex] = okvValues[currencyType] * Convert.ToDouble(sourceRow["CurrencySum"]);
                }
            }

            // Сортировка
            if (calcColumnTypes[colIndex] == CalcColumnType.cctSortStatus)
            {
                destRow[colIndex] = "1";
                if (sourceRow["RefSStatusPlan"].ToString() == "4")
                {
                    destRow[colIndex] = "0";
                }
            }

            // Организации (для гарантий)
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOrganizationRegion)
            {
                IEntity orgBook = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Organizations_Plan_Key);
                if (sourceRow["RefOrganizations"].ToString() == "-1")
                {
                    IEntity orgRegion = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Regions_Plan);
                    destRow[colIndex] = GetBookValue(orgRegion, Convert.ToInt32(sourceRow["RefRegions"]), "name");
                }
                else
                {
                    destRow[colIndex] = GetBookValue(orgBook, Convert.ToInt32(sourceRow["RefOrganizations"]), "name");
                }
            }

            // Организации (для гарантий)
            if (calcColumnTypes[colIndex] == CalcColumnType.cctRegion)
            {
                IEntity orgRegion = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Regions_Plan);
                destRow[colIndex] = GetBookValue(orgRegion, Convert.ToInt32(sourceRow["RefRegions"]), "name");
            }

            // Организации (для гарантий)
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOrganizations)
            {
                IEntity orgBook = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Organizations_Plan_Key);
                string orgName1 = GetBookValue(orgBook, Convert.ToInt32(sourceRow["RefOrganizationsPlan2"]), "name");
                string orgName2 = GetBookValue(orgBook, Convert.ToInt32(sourceRow["RefOrganizations"]), "name");
                string orgName3 = GetBookValue(orgBook, Convert.ToInt32(sourceRow["RefOrganizationsPlan3"]), "name");
                destRow[colIndex] = string.Format("Гарант: {0}, принципал: {1}, бенефициар: {2}",
                    orgName1, orgName2, orgName3);
            }

            // Бенефициатор
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOrganization3)
            {
                IEntity orgBook = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Organizations_Plan_Key);
                destRow[colIndex] = GetBookValue(orgBook, Convert.ToInt32(sourceRow["RefOrganizationsPlan3"]), "name");
            }

            // Обеспечение
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCollateralType)
            {
                IEntity collateralBook = scheme.RootPackage.FindEntityByName(GetCollateralKey());
                destRow[colIndex] = GetBookValue(collateralBook, Convert.ToInt32(sourceRow["id"]), "name", true);
            }

            // Номер (для гарантий)
            if (calcColumnTypes[colIndex] == CalcColumnType.cctNumStartDate)
            {
                destRow[colIndex] = string.Format("{0}, {1}", 
                    GetDateValue(sourceRow["StartDate"]), sourceRow["Num"]);
            }

            // Период договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctReportPeriod)
            {
                string minDate = GetDateValue(sourceRow["StartDate"]);
                string maxDate = GetDateValue(sourceRow["EndDate"]);

                string paramMinDate = reportParams["StartDate"];
                string paramMaxDate = reportParams["EndDate"];

                if (minDate == string.Empty || Convert.ToDateTime(minDate) < Convert.ToDateTime(paramMinDate))
                {
                    minDate = paramMinDate;
                }
                if (maxDate == string.Empty || Convert.ToDateTime(maxDate) < Convert.ToDateTime(paramMaxDate))
                {
                    maxDate = paramMaxDate;
                }
                destRow[colIndex] = string.Format("{0}-{1}", GetDateValue(minDate), GetDateValue(maxDate));
            }

            // Номер (для гарантий)
            if (calcColumnTypes[colIndex] == CalcColumnType.cctNumStartDate2)
            {
                destRow[colIndex] = string.Format("{1} от {0}",
                    GetDateValue(sourceRow["StartDate"]), sourceRow["Num"]);
            }

            // Зависимая колонка
            if (calcColumnTypes[colIndex] == CalcColumnType.cctRelation)
            {
                string[] columnIndexList = columnParamList[colIndex]["ParamValue1"].Split(';');
                double sum = 0;
                for (int i = 0; i < columnIndexList.Length; i++)
                {
                    int tempIndex = Convert.ToInt32(columnIndexList[i].Remove(0, 1));
                    if (destRow[tempIndex] != DBNull.Value)
                    {
                        double tempVal = Convert.ToDouble(destRow[tempIndex]);
                        if ((columnIndexList[i][0] == '%'))
                        {
                            sum /= tempVal;
                        }
                        else if ((columnIndexList[i][0] == '*'))
                        {
                            sum *= tempVal;
                        }
                        else
                        {
                            int multiplier = 1;
                            if (columnIndexList[i][0] == '-') multiplier = -1;
                            sum += multiplier * tempVal;
                        }
                    }
                }
                destRow[colIndex] = sum;
            }

            // Регресс(для гарантий)
            if (calcColumnTypes[colIndex] == CalcColumnType.cctRegress)
            {
                int regress = Convert.ToInt32(sourceRow["Regress"]);
                destRow[colIndex] = "нет";
                if (regress == 1) destRow[colIndex] = "да";
            }

            // Регресс(для гарантий)
            if (calcColumnTypes[colIndex] == CalcColumnType.cctRegress2)
            {
                int regress = Convert.ToInt32(sourceRow["Regress"]);
                destRow[colIndex] = "Нет";
                if (regress == 1) destRow[colIndex] = "Есть";
            }

            // Номер + дата контракта
            if (calcColumnTypes[colIndex] == CalcColumnType.cctNumContractDatePercent)
            {
                destRow[colIndex] = string.Format("№ {1} {0}, {2:N2}%", 
                    GetDateValue(sourceRow["ContractDate"]), 
                    sourceRow["Num"], 
                    sourceRow["CreditPercent"]);
            }

            // Номер + дата контракта
            if (calcColumnTypes[colIndex] == CalcColumnType.cctNumContractDate)
            {
                destRow[colIndex] = string.Format("{0} № {1}", GetDateValue(sourceRow["ContractDate"])
                    , sourceRow["Num"]);
            }

            // Разыменовка типа договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctContractDesc)
            {
                destRow[colIndex] = GetContractDesc();
            }
            // Организация
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOrganization)
            {
                IEntity orgBook = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Organizations_Plan_Key);
                destRow[colIndex] = GetBookValue(orgBook, Convert.ToInt32(sourceRow["RefOrganizations"]), "name");
            }
            // Родительская организация
            if (calcColumnTypes[colIndex] == CalcColumnType.cctGenOrg)
            {
                IEntity orgBook = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Organizations_Plan_Key);
                destRow[colIndex] = GetBookValue(orgBook, Convert.ToInt32(sourceRow["RefOrganizations"]), "GenOrgName");
                //destRow[colIndex] = GetBookValue(orgBook, Convert.ToInt32(sourceRow["RefOrganizations"]), "name");
            }
            // Дата окончания договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCreditEndDate)
            {
                string endDateName = GetCreditEndFieldName(sourceRow);
                if (endDateName != string.Empty)
                {
                    destRow[colIndex] = Convert.ToDateTime(sourceRow[endDateName]).ToShortDateString();
                    creditEndDate = Convert.ToDateTime(destRow[colIndex]).ToShortDateString();
                }
                else
                {
                    creditEndDate = DateTime.MinValue.ToShortDateString();
                }
                destRow["SortCreditEndDate"] = creditEndDate;
            }
            // Название валюты
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOKVName)
            {
                IEntity okvBook = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_OKV_Currency_Key);
                destRow[colIndex] = GetBookValue(okvBook, Convert.ToInt32(sourceRow["RefOKV"]), "CodeLetter");
            }
            // Курс валюты
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOKVValue)
            {
                int okvType = Convert.ToInt32(sourceRow["RefOKV"]);
                if (okvType != -1)
                {
                    destRow[colIndex] = RefreshExchangeList(okvType);
                    if (columnParamList[colIndex].ContainsKey("ActualDate"))
                    {
                        destRow[colIndex] = GetExchangeValue(okvType, columnParamList[colIndex]["ActualDate"]);
                    }
                }
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctContractNum2)
            {
                IEntity orgBook = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Organizations_Plan_Key);
                // Составная информация о договоре(дата, тип)
                string contractDate = string.Empty;
                string orgName = string.Empty;
                if (sourceRow["ContractDate"] != DBNull.Value)
                    contractDate = Convert.ToDateTime(sourceRow["ContractDate"]).ToShortDateString();
                if (sourceRow["RefOrganizations"] != DBNull.Value)
                    orgName = string.Format("{0}", GetBookValue(orgBook, Convert.ToInt32(sourceRow["RefOrganizations"]), "name"));

                IEntity creditType = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_TypeContract_Key);
                string ctName = GetBookValue(creditType, Convert.ToInt32(sourceRow["RefTypeContract"]), "name");

                destRow[colIndex] = string.Format("{0} от {1} № {2} с {3}({4:N0} руб.)",
                    ctName, contractDate, sourceRow["Num"], orgName, sourceRow["Sum"]);
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCreditTypeNumDate)
            {
                IEntity creditType = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_TypeContract_Key);
                destRow[colIndex] = string.Format("{0} {1} от {2}",
                    GetBookValue(creditType, Convert.ToInt32(sourceRow["RefTypeContract"]), "name"), 
                    sourceRow["Num"], 
                    GetDateValue(sourceRow["ContractDate"]));
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCreditTypeNumStartDate)
            {
                IEntity creditType = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_TypeContract_Key);
                destRow[colIndex] = string.Format("{0}/ {1}/ {2}",
                    GetBookValue(creditType, Convert.ToInt32(sourceRow["RefTypeContract"]), "name"),
                    sourceRow["Num"],
                    GetDateValue(sourceRow["StartDate"]));
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctGarantTypeNumDate)
            {
                IEntity creditType = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_TypeContract_Key);
                destRow[colIndex] = string.Format("{0} {1} от {2}",
                    GetBookValue(creditType, Convert.ToInt32(sourceRow["RefTypeContract"]), "name"),
                    sourceRow["Num"],
                    GetDateValue(sourceRow["DateDoc"]));
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCreditIssNumDocDate)
            {
                destRow[colIndex] = string.Format("{0} от {1}",
                    sourceRow["Num"], GetDateValue(sourceRow["DocDate"]));
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctContractNum3)
            {
                IEntity creditType = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_TypeContract_Key);
                string ctName = GetBookValue(creditType, Convert.ToInt32(sourceRow["RefTypeContract"]), "name");
                IEntity orgBook = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Organizations_Plan_Key);
                // Составная информация о договоре(дата, тип)
                string contractDate = string.Empty;
                string orgName = string.Empty;
                if (sourceRow["RefOrganizations"] != DBNull.Value)
                    orgName = string.Format("{0}", GetBookValue(orgBook, Convert.ToInt32(sourceRow["RefOrganizations"]), "name"));

                destRow[colIndex] = string.Format("{0} от {1} с {2}({3:N0} руб.)",
                    ctName, GetDateValue(sourceRow["StartDate"]), orgName, Convert.ToDouble(sourceRow["Sum"]));
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctContractNum4)
            {
                destRow[colIndex] = string.Format("{0} от {1} на {2}",
                    sourceRow["ActNum"], GetDateValue(sourceRow["ActDate"]), sourceRow["Purpose"]);
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctNumDateOKV)
            {
                IEntity okvBook = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_OKV_Currency_Key);
                destRow[colIndex] = string.Format("{0}; {1}; {2}",
                    sourceRow["Num"], 
                    GetDateValue(sourceRow["ContractDate"]),
                    GetBookValue(okvBook, Convert.ToInt32(sourceRow["RefOKV"]), "CodeLetter"));
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapLabelPurpose)
            {
                destRow[colIndex] = string.Format(
                    "Министерство управления финансами Самарской области Цель займа - {0}",
                    sourceRow["Purpose"]);
            }
            
            // Цель + Организация
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOrgPurpose)
            {
                string orgName = string.Empty;
                IEntity orgBook = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Organizations_Plan_Key);
                if (sourceRow["RefOrganizations"] != DBNull.Value)
                    orgName = string.Format("{0}", GetBookValue(orgBook, Convert.ToInt32(sourceRow["RefOrganizations"]), "name"));
                destRow[colIndex] = string.Format("{0}. {1}",
                    orgName, sourceRow["Purpose"]);
            }

            // Субзаймы минфина Самара
            if (calcColumnTypes[colIndex] == CalcColumnType.cctSubCreditCaption)
            {
                IEntity creditType = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_TypeContract_Key);
                string contractTypeName = GetBookValue(creditType, Convert.ToInt32(sourceRow["RefTypeContract"]), "name");
                destRow[colIndex] = 
                    string.Format("{0} ({1} от {2}) между Минфином РФ, Администрацией Самарской области - {3}",
                    contractTypeName, sourceRow["Num"], 
                    GetDateValue(sourceRow["ContractDate"]), sourceRow["Purpose"]);
            }

            // Год старта договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCreditYear)
            {
                if (sourceRow["StartDate"] != DBNull.Value)
                {
                    destRow[colIndex] = Convert.ToDateTime(sourceRow["StartDate"]).Year;
                }
                else
                {
                    destRow[colIndex] = 0;
                }
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPurposeActNumDate)
            {
                destRow[colIndex] = string.Format("{0}, {1}, {2}",
                    sourceRow["Purpose"], sourceRow["ActNum"], GetDateValue(sourceRow["ActDate"]));
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctContractNum)
            {
                IEntity orgBook = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_Organizations_Plan_Key);
                // Составная информация о договоре(дата, тип)
                string part1 = string.Empty;
                string part2 = string.Empty;
                string part31 = string.Empty;
                string part32 = string.Empty;
                string part4 = string.Empty;
                if (sourceRow["RefOrganizations"] != DBNull.Value)
                    part1 = string.Format("{0}", GetBookValue(orgBook, Convert.ToInt32(sourceRow["RefOrganizations"]), "name"));
                if (sourceRow["Num"] != DBNull.Value)
                    part2 = string.Format("; {0}", sourceRow["Num"]);
                if (sourceRow["ContractDate"] != DBNull.Value)
                    part31 = string.Format("; {0}", 
                        Convert.ToDateTime(sourceRow["ContractDate"]).ToShortDateString());
                if (GetCreditEndFieldName(sourceRow) != string.Empty && sourceRow[GetCreditEndFieldName(sourceRow)] != DBNull.Value)
                    part32 = string.Format("-{0}",                         
                        Convert.ToDateTime(sourceRow[GetCreditEndFieldName(sourceRow)]).ToShortDateString());
                IEntity okvBook = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_OKV_Currency_Key);
                if (sourceRow["RefOKV"] != DBNull.Value && Convert.ToInt32(sourceRow["RefOKV"]) != -1) 
                    part4 = string.Format("; {0}", GetBookValue(okvBook, Convert.ToInt32(sourceRow["RefOKV"]), "CodeLetter"));

                destRow[colIndex] = string.Format("{0}{1}{2}{3}{4}", part1, part2, part31, part32, part4);
            }

            // Считарь значений по деталям
            if (calcColumnTypes[colIndex] == CalcColumnType.cctDetail)
            {
                string formula = columnParamList[colIndex]["Formula"];
                currentFieldList = string.Empty;
                currentIgnoreExchenge = false;
                if (columnParamList[colIndex].ContainsKey("FieldList"))
                {
                    currentFieldList = columnParamList[colIndex]["FieldList"];
                }
                if (columnParamList[colIndex].ContainsKey("IgnoreExchange"))
                {
                    currentIgnoreExchenge = Convert.ToBoolean(columnParamList[colIndex]["IgnoreExchange"]);
                }
                int id = Convert.ToInt32(sourceRow["id"]);
                destRow[colIndex] = ParseFormula(id, ref formula);
                currentFieldList = string.Empty;
                currentIgnoreExchenge = false;
            }

            // Считарь значений по деталям
            if (calcColumnTypes[colIndex] == CalcColumnType.cctDetailText)
            {
                writeFullDetailText = true;
                fullDetailText = string.Empty;
                string formula = columnParamList[colIndex]["Formula"];
                int id = Convert.ToInt32(sourceRow["id"]);
                ParseFormula(id, ref formula);
                destRow[colIndex] = fullDetailText;
                writeFullDetailText = false;
            }

            // Купонный доход
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapCoupon)
            {
                destRow[colIndex] = GetPercentText(sourceRow, DateTime.MinValue, DateTime.MaxValue,
                    "Coupon", false, true, false);
            }

            // Текстовка журнала процентов по договору
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPercentText)
            {
                destRow[colIndex] = GetPercentText(sourceRow, DateTime.MinValue, DateTime.MaxValue, 
                    "CreditPercent", false, false, true);
            }

            // Текстовка журнала процентов по договору
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPercentValues)
            {
                destRow[colIndex] = GetPercentText(sourceRow, DateTime.MinValue, DateTime.MaxValue,
                    "CreditPercent", false, true, true);
            }

            // Текстовка журнала процентов по договору
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPercentValues2)
            {
                destRow[colIndex] = GetPercentText(sourceRow, DateTime.MinValue, DateTime.MaxValue,
                    "CreditPercent", false, true, false);
            }

            // Текстовка журнала процентов по договору - Максимальное и минимальное
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPercentTextMaxMin)
            {
                destRow[colIndex] = GetPercentText(sourceRow, DateTime.MinValue, DateTime.MaxValue, 
                    "CreditPercent", true, false, true);
            }

            // Заголовок для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapOffNumNPANameDateNum)
            {
                destRow[colIndex] = string.Format("Облигационный займ Самарской области {0}, {1} от {2}, {3}",
                    sourceRow["OfficialNumber"], sourceRow["NameNPA"],
                    GetDateValue(sourceRow["DateNPA"]), sourceRow["NumberNPA"]);
            }

            // Остаток на дату
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCurrentRest)
            {
                string sumFieldName = "Sum";
                double currencyMult = 1;
                int masterKey = Convert.ToInt32(sourceRow["id"]);
                if (currencyType != -1 && !ignoreCurrencyCalc)
                {
                    sumFieldName = "CurrencySum";
                    currencyMult = okvValues[currencyType];
                }

                double contractSum = 0;
                if (sourceRow[sumFieldName] != DBNull.Value)
                    contractSum = currencyMult * Convert.ToDouble(sourceRow[sumFieldName]);

                int contractType = Convert.ToInt32(sourceRow["RefSExtension"]);
                if (contractType == 3 || contractType == 5 || contractType == 6)
                {
                    destRow[colIndex] = contractSum - currencyMult * GetSumValue(
                        dtDetail[0],
                        masterKey,
                        "FactDate",
                        sumFieldName,
                        DateTime.MinValue,
                        Convert.ToDateTime(reportParams["EndDate"]), 
                        true,  true);
                }
                else if (contractType == 4)
                {
                    destRow[colIndex] = contractSum -
                        currencyMult * GetSumValue(
                          dtDetail[0],
                          masterKey,
                          "FactDate",
                          sumFieldName,
                          DateTime.MinValue,
                          Convert.ToDateTime(reportParams["EndDate"]),
                          true, true)
                        +
                        currencyMult * GetSumValue(
                          dtDetail[1],
                          masterKey,
                          "FactDate",
                          sumFieldName,
                          DateTime.MinValue,
                          Convert.ToDateTime(reportParams["EndDate"]),
                          true, true);                        
                }
                else
                {
                    destRow[colIndex] = GetCurrentRest(masterKey, currencyType);
                }
            }
        }

        // Значение курса на дату
        public double GetExchangeValue(int okvType, string calcDate)
        {
            double result = 0;
            DataTable dt = exchangeRate[okvType];
            string addSign = string.Empty;
            if (!exchangePrevDay) addSign = "=";
            DataRow[] drExch = dt.Select(string.Format("DateFixing <{1}'{0}'", calcDate, addSign), "DateFixing desc");
            if (drExch.Length > 0)
                result = Convert.ToDouble(drExch[0]["ExchangeRate"]);
            return result;
        }

        // Заполняем кэши валют и таблицу используемых валют dtExchange для okvType
        private string RefreshExchangeList(int okvType)
        {
            string result = string.Empty;
            if (okvType != -1)
            {
                DataTable dt;
                if (!exchangeRate.ContainsKey(okvType))
                {
                    dt = new DataTable();
                    IEntity entity = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_S_ExchangeRate_Key);
                    using (IDataUpdater upd = entity.GetDataUpdater(string.Format("RefOKV = {0}", okvType), null, null))
                    {
                        dt = new DataTable();
                        upd.Fill(ref dt);
                    }
                    dt = SortDataSet(dt, "DateFixing desc", false);
                    exchangeRate.Add(okvType, dt);
                }
                else
                {
                    dt = exchangeRate[okvType];
                }

                string addSign = string.Empty;
                if (!exchangePrevDay) addSign = "=";

                DataRow[] drExch = dt.Select(
                    string.Format("DateFixing <{1}'{0}'", reportParams["EndDate"], addSign),
                    "DateFixing desc");

                if (drExch.Length > 0)
                {
                    result = string.Format("{0}({1})",
                        drExch[0]["ExchangeRate"],
                        Convert.ToDateTime(drExch[0]["DateFixing"]).ToShortDateString());

                    if (okvCodes.IndexOf(okvType) == -1)
                    {
                        DataRow drExchange = dtExchange.Rows.Add();
                        IEntity okvBook = scheme.RootPackage.FindEntityByName(SchemeObjectsKeys.d_OKV_Currency_Key);
                        drExchange[0] = string.Format("{0} {1} ({2})",
                            GetBookValue(okvBook, okvType, "CodeLetter"),
                            drExch[0]["ExchangeRate"],
                            Convert.ToDateTime(drExch[0]["DateFixing"]).ToShortDateString()
                            );
                        okvCodes.Add(okvType);
                        okvValues.Add(okvType, Convert.ToDouble(drExch[0]["ExchangeRate"]));
                    }
                }
                else
                {
                    DataRow fictiveRate = dt.NewRow();
                    fictiveRate["DateFixing"] = DateTime.MinValue.ToShortDateString();
                    fictiveRate["ExchangeRate"] = 0;
                    dt.Rows.Add(fictiveRate);
                    okvCodes.Add(okvType);
                    okvValues.Add(okvType, 0);
                }
            }
            return result;
        }

        // По заполненности полей определяет какое поле соответствует дате закрытия договора
        protected string GetCreditEndFieldName(DataRow sourceRow)
        {
            string endDateName = string.Empty;
            if (sourceRow["EndDate"] != DBNull.Value) endDateName = "EndDate";
            if (sourceRow["RenewalDate"] != DBNull.Value) endDateName = "RenewalDate";
            return endDateName;
        }

        // Заполняем данные
        public DataTable FillData()
        {
            // Даты расчета курса валюты
            if (!reportParams.ContainsKey("EndDate")) reportParams.Add("EndDate", DateTime.Now.ToShortDateString());
            // Определяем используемые детали
            FillUsedDetailList();
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                DataTable dtMaster;
                IEntity entity = scheme.RootPackage.FindEntityByName(GetMainTableKey());
                using (IDataUpdater upd = entity.GetDataUpdater(string.Empty, null, null))
                {
                    dtMaster = new DataTable();
                    upd.Fill(ref dtMaster);
                }
                DataTable dt = dtMaster.Clone(); ;
                DataRow[] drsSelect = dtMaster.Select(GetMainSQLQuery());
                for (int i = 0; i < drsSelect.Length; i++)
                {
                    dt.ImportRow(drsSelect[i]);
                }

                // Хитрованское извращение с запросом деталей(выбираем только т что нужны)
                dtDetail = new DataTable[GetDetailKeys().Length];
                for (int j = 0; j < GetDetailKeys().Length; j++)
                {
                    dtDetail[j] = null;
                    IEntityAssociation ass = scheme.RootPackage.FindAssociationByName(GetDetailKeys()[j]);
                    IEntity detailEntity = ass.RoleData;

                    if (entity != null)
                    {
                        using (IDataUpdater upd = detailEntity.GetDataUpdater(string.Empty, null, null))
                        {
                            dtDetail[j] = new DataTable();
                            upd.Fill(ref dtDetail[j]);
                        }
                    }
                }
                // Записи журнала вычитываем из БД, только если есть колонка с текстовкой процентов
                if (calcColumnTypes.ContainsValue(CalcColumnType.cctPercentText) ||
                    calcColumnTypes.ContainsValue(CalcColumnType.cctPercentTextMaxMin) ||
                    calcColumnTypes.ContainsValue(CalcColumnType.cctPercentValues) ||
                    calcColumnTypes.ContainsValue(CalcColumnType.cctPercentValues2))
                {
                    entity = scheme.RootPackage.FindEntityByName(GetJournalKey());
                    using (IDataUpdater upd = entity.GetDataUpdater(string.Empty, null, null))
                    {
                        dtJournalPercent = new DataTable();
                        upd.Fill(ref dtJournalPercent);
                    }
                }
                // Создаем поля в датасете результата
                CreateFields();
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    currencyType = Convert.ToInt32(dt.Rows[j]["RefOKV"]);
                    if (currencyType != -1) RefreshExchangeList(currencyType);

                    dtResult.Rows.Add();
                    for (int i = 0; i < columnCount; i++)
                    {
                        currentColumnIndex = i;
                        // заполняем значение поля
                        if (columnList[i] == FieldType.ftCalc)
                        {
                            FillCalcField(dtResult.Rows[j], dt.Rows[j], i);
                        }
                        else
                        {
                            FillDataField(dtResult.Rows[j], dt.Rows[j], i);
                        }
                        // Прооверим не нужно ли подчистить значение в зависимости от условия заполнения колонки
                        CheckColumnCondition(i, dt.Rows[j], dtResult.Rows[j]);
                    }
                    // служебные поля
                    dtResult.Rows[j]["RefOKV"] = dt.Rows[j]["RefOKV"];
                    if (dt.Columns.Contains("ParentID")) dtResult.Rows[j]["ParentID"] = dt.Rows[j]["ParentID"];
                    dtResult.Rows[j]["ID"] = dt.Rows[j]["ID"];
                }
                // Повторный проход по формульным полям, потому что составляющие могут идти после них
                for (int j = 0; j < dtResult.Rows.Count; j++)
                {
                    for (int i = 0; i < columnCount; i++)
                    {
                        currentColumnIndex = i;
                        if (columnList[i] == FieldType.ftCalc && calcColumnTypes[i] == CalcColumnType.cctRelation)
                        {
                            FillCalcField(dtResult.Rows[j], null, i);
                        }
                    }
                }

                // Сортируем
                if (sortString != string.Empty)
                    dtResult = SortDataSet(dtResult, sortString, hierarchicalSort);

                // в нулевой колонке иногда счетчик
                if (calcColumnTypes.Keys.Count > 0 && calcColumnTypes[0] == CalcColumnType.cctPosition)
                {
                    for (int j = 0; j < dtResult.Rows.Count; j++)
                    {
                        dtResult.Rows[j][0] = j + 1;
                    }
                }
                // Заполняем итоги
                if (useSummaryRow)
                {
                    DataRow drResult = dtResult.Rows.Add();
                    dtResult = RecalcSummary(dtResult);
                }
            }

            if (removeServiceFields)
            {
                for (int i = 0; i < 4; i++)
                {
                    dtResult.Columns.RemoveAt(dtResult.Columns.Count - 1);
                }
            }

            // Выходим, радуемся
            return dtResult;
        }

        /// <summary>
        /// Расчет итоговой строчки
        /// </summary>
        public DataTable RecalcSummary(DataTable dtResult)
        {
            DataRow drResult = dtResult.Rows[dtResult.Rows.Count - 1];
            for (int i = 0; i < summaryColumnIndex.Count; i++)
            {
                drResult[summaryColumnIndex[i]] = 0;
            }
            for (int j = 0; j < dtResult.Rows.Count - 1; j++)
            {
                for (int i = 0; i < summaryColumnIndex.Count; i++)
                {
                    if (dtResult.Rows[j][summaryColumnIndex[i]] != DBNull.Value)
                    {
                        drResult[summaryColumnIndex[i]] =
                            Convert.ToDouble(drResult[summaryColumnIndex[i]]) +
                            Convert.ToDouble(dtResult.Rows[j][summaryColumnIndex[i]]);
                    }
                }
            }
            return dtResult;
        }

        /// <summary>
        /// Строка с процентами
        /// </summary>
        private string GetPercentText(DataRow row, DateTime startDate, DateTime endDate, 
            string fieldName, bool isMaxMin, bool onlyValues, bool usePercentSym)
        {
            double minValue = 0; 
            double maxValue = 0;
            IScheme scheme = WorkplaceSingleton.Workplace.ActiveScheme;
            int masterKey = Convert.ToInt32(row["ID"]);
            // выбираем из кэша только записи текущего договора(сортированные по дате)
            DataRow[] drJournalPercent = dtJournalPercent.Select(string.Format("{1}={0}", masterKey, GetParentRefName()), "ChargeDate asc");
            string dateText = string.Empty;
            string percentSym = string.Empty;
            if (usePercentSym) percentSym = "%";
            if (drJournalPercent.Length > 0)
            {
                if (drJournalPercent.Length > 1)
                {
                    Collection<string> percentList = new Collection<string>();

                    string textPercent = string.Empty;
                    string recordText = string.Empty;
                    // Это извращение чтобы отсечь дубли(которых в "теории" нет)
                    foreach (DataRow rowPercent in drJournalPercent)
                    {
                        dateText = Convert.ToDateTime(rowPercent["ChargeDate"]).ToShortDateString();
                        recordText = string.Format("{0} {1}",
                            dateText,
                            rowPercent[fieldName].ToString());

                        if (!percentList.Contains(recordText))
                        {
                            percentList.Add(recordText);
                        }
                    }

                    string prevValue = string.Empty;
                    string prevKey = string.Empty;
                    // это извращение чтобы вывести несколько на одну дату недубли :)
                    foreach (string recordStr in percentList)
                    {
                        string keyName = recordStr.Split(' ')[0];
                        string keyValue = recordStr.Split(' ')[1];
                        if (Convert.ToDateTime(keyName) > startDate && Convert.ToDateTime(keyName) < endDate)
                        {
                            if (textPercent == string.Empty && prevValue != string.Empty)
                            {
                                dateText = prevKey;
                                if (onlyValues) dateText = string.Empty;
                                textPercent = string.Format("{0}, {1} {2:N2} {3}",
                                    textPercent, dateText, Convert.ToDouble(prevValue), percentSym);
                            }
                            if (prevValue != keyValue)
                            {
                                dateText = keyName;
                                if (onlyValues) dateText = string.Empty;
                                textPercent = string.Format("{0}, {1} {2:N2} {3}",
                                    textPercent, dateText, Convert.ToDouble(keyValue), percentSym);
                            }

                            maxValue = Math.Max(maxValue, Convert.ToDouble(keyValue));
                            minValue = Math.Min(minValue, Convert.ToDouble(keyValue));
                            if (minValue == 0) minValue = maxValue;
                        }
                        prevValue = keyValue;
                        prevKey = keyName;
                    }
                    if (textPercent == string.Empty)
                    {
                        dateText = prevKey;
                        if (onlyValues) dateText = string.Empty;
                        textPercent = string.Format("{0}, {1} {2:N2} {3}",
                            textPercent, dateText, Convert.ToDouble(prevValue), percentSym);
                    }

                    if (isMaxMin && maxValue != minValue)
                    {
                        return string.Format("{0:N2}{3}-{1:N2}{3}", minValue, maxValue, percentSym);
                    }
                    else
                    {
                        return textPercent.TrimStart(',').TrimStart(' ');
                    }
                }
                else
                {
                    // запись была одна...
                    dateText = Convert.ToDateTime(drJournalPercent[0]["ChargeDate"]).ToShortDateString();
                    if (onlyValues) dateText = string.Empty;
                    return string.Format("{1} {0:N2}{2}",
                            Convert.ToDouble(drJournalPercent[0][fieldName]),
                            dateText, percentSym).Trim();
                }
            }
            else
            {
                // в детали журнала процентов вообще пусто
                if (row.Table.Columns.Contains("CreditPercent") && row["CreditPercent"] != DBNull.Value 
                    && row["CreditPercent"].ToString() != string.Empty)
                {
                    return string.Format("{0:N2} {1}", Convert.ToDouble(row["CreditPercent"]), percentSym);
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// вытягивает значение из справочника по ссылке
        /// </summary>
        private string GetBookValue(IEntity book, int refValue, string resultField)
        {
            return GetBookValue(book, refValue, resultField, false);
        }

        /// <summary>
        /// вытягивает значение из справочника по ссылке
        /// </summary>
        private string GetBookValue(IEntity book, int refValue, string resultField, bool byRef)
        {
            string result = string.Empty;
            // Проверяем заполнен ли кэш данного справочника
            if (!bookCache.ContainsKey(book.FullDBName))
            {
                DataTable dtBook;
                using (IDataUpdater upd = book.GetDataUpdater(string.Empty, null, null))
                {
                    dtBook = new DataTable();
                    upd.Fill(ref dtBook);
                }
                bookCache.Add(book.FullDBName, dtBook);
            }
            // ищем в кэше
            string keyFieldName = "id";
            if (byRef) keyFieldName = GetParentRefName();
            DataRow[] drFind = bookCache[book.FullDBName].Select(string.Format("{0} = {1}", keyFieldName, refValue));
            if (drFind.Length > 0)
            {
                result = drFind[0][resultField].ToString();
            }
            return result;
        }

        private bool CheckColumnFilter(DataRow dr)
        {
            bool result = true;

            Collection<string> conditionFields = new Collection<string>();
            conditionFields.Add("RefTypSum");
            conditionFields.Add("Offset");

            foreach (string conditionField in conditionFields)
            {
                if (columnParamList[currentColumnIndex].ContainsKey(conditionField))
                {
                    result = false;
                    if (dr.Table.Columns.Contains(conditionField))
                    {
                        string[] values = columnParamList[currentColumnIndex][conditionField].Split(',');
                        for (int i = 0; i < values.Length; i++)
                        {
                            result = result || dr[conditionField].ToString() == values[i];
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// суммирование по детальной таблице
        /// </summary>
        public double GetSumValue(DataTable dtDetail, int masterID, string dateFieldName,
            string sumFieldName, DateTime startPeriodDate, DateTime endPeriodDate, bool includeDate1, bool includeDate2)
        {
            // Оставляем только детали по данному договору
            DataRow[] drsSelect = dtDetail.Select(string.Format("{1} = {0}", masterID, GetParentRefName()));
            double sum = 0;

            string[] fieldNames = sumFieldName.Split(',');
            sumIncludedRows.Clear();
            bool allValues = 
                startPeriodDate.ToShortDateString() == DateTime.MinValue.ToShortDateString()
                && endPeriodDate.ToShortDateString() == DateTime.MaxValue.ToShortDateString(); 
            foreach (DataRow row in drsSelect)
            {
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    if (row[fieldNames[i]] != DBNull.Value && CheckColumnFilter(row))
                    {
                        if (allValues)
                        {
                            sum += Convert.ToDouble(row[fieldNames[i]]);
                            sumIncludedRows.Add(row);
                        }
                        else
                        {
                            if (row[dateFieldName] != DBNull.Value)
                            {
                                DateTime dateValue = Convert.ToDateTime(row[dateFieldName]);
                                // Если дата попадает в диапазон, либо при установленном includeDate 
                                //точно соответствует одной из границ диапазона
                                if ((dateValue < endPeriodDate || (includeDate2 && dateValue == endPeriodDate))
                                    && (dateValue > startPeriodDate || (includeDate1 && dateValue == startPeriodDate)))
                                {
                                    if (writeFullDetailText)
                                    {
                                        if (!columnParamList[currentColumnIndex].ContainsKey("OnlyDates"))
                                        {
                                            fullDetailText = string.Format("{0},{1} {2:N2}",
                                                fullDetailText, dateValue.ToShortDateString(), Convert.ToDouble(row[fieldNames[i]]));
                                        }
                                        else
                                        {
                                            fullDetailText = string.Format("{0},{1}",
                                                fullDetailText, dateValue.ToShortDateString());
                                        }
                                    }
                                    sum += Convert.ToDouble(row[fieldNames[i]]);
                                    sumIncludedRows.Add(row);
                                }
                            }
                        }
                    }
                }
            }
            fullDetailText = fullDetailText.TrimStart(',');
            return sum;
        }

        // Разбирает значение вида [номер детали](
        //{0 нижняя граница диапазона, 1 - верхняя граница диапазона}<|>|<=|>='dd.mm.yy' 
        // повторить для настройик всех границ) БЕЗ ПРОБЕЛОВ!
        private double ParseValue(int id, ref string formula)
        {
            int balance = 1;
            int startPos = formula.IndexOf('(');
            int endPos = -1;
            int test;
            // частный случай значения поля таблицы фактов CreditEndDate
            if (formula[0].ToString() == "'")
            {
                endPos = formula.IndexOf("'", 1);
                string dateStr = formula.Substring(1, endPos - 1);
                DateTime dt;
                formula = formula.Remove(0, endPos + 1);
                if (DateTime.TryParse(dateStr, out dt))
                {
                    dt = Convert.ToDateTime(dateStr);
                }
                else
                {
                    dt = Convert.ToDateTime(creditEndDate);
                }
                string day = dt.Day.ToString();
                if (dt.Day < 10) day = "0" + day ;
                string month = dt.Month.ToString();
                if (dt.Month < 10) month = "0" + month;
                return Convert.ToDouble(string.Format("{0}{1}{2}", dt.Year, month, day));
            }
            // частный случай вычисления - число
            if (int.TryParse(formula[0].ToString(), out test))
            {
                endPos = formula.IndexOf(',');
                if (endPos == -1) 
                {
                    formula = string.Empty;
                    return Convert.ToDouble(formula);
                }
                else
                {
                    string numberStr = formula.Substring(0, endPos);
                    formula = formula.Remove(0, numberStr.Length);
                    return Convert.ToDouble(numberStr);
                }
            }
            // ищем закрывающую скобку выражения границ
            for (int i = startPos + 1; i < formula.Length; i++)
            {
                if (formula[i] == ')') balance--;
                if (formula[i] == '(') balance++;
                if (balance == 0)
                {
                    endPos = i;
                    break;
                }
            }
            // Она есть
            if (endPos > 0)
            {
                // Копируем выражение ограничений детали
                string value = formula.Substring(startPos + 1, endPos - startPos - 1);
                // Индекс детали по которой считаем
                string detailIndex = formula.Substring(1, startPos - 2);
                formula = formula.Remove(0, startPos);
                // dimKind1 = 0 - начальная граница, 1 - конечная
                string dimKind1 = formula.Substring(1, 1);
                formula = formula.Remove(0, 2);
                // Знак сравнения
                int sign1 = ParseSign(formula);
                formula = CutSign(formula, sign1);
                // Значение даты ограничения
                string date1 = formula.Substring(0, 10);
                
                // Повторить для второго ограничения(пока больше не было и надеюсь не будет)
                string dimKind2 = string.Empty;
                int sign2 = - 1;
                string date2 = string.Empty; ;
                formula = formula.Remove(0, 10);
                if (endPos > 20)
                {
                    dimKind2 = formula.Substring(0, 1);
                    formula = formula.Remove(0, 1);
                    sign2 = ParseSign(formula);
                    formula = CutSign(formula, sign2);
                    date2 = formula.Substring(0, 10);
                    formula = formula.Remove(0, 10);
                }
                formula = formula.Remove(0, 1);
                // Границы надо чем то заполнить даже если их не указали
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MaxValue;
                string dateFieldName = GetEndDates()[Convert.ToInt32(detailIndex)];

                bool include1 = false;
                bool include2 = false;
                if (dimKind1 == "0") dateFieldName = GetStartDates()[Convert.ToInt32(detailIndex)];
                if (sign1 == 3 || sign1 == 5)
                {
                    include1 = sign1 == 5 || sign1 == 6;
                    startDate = Convert.ToDateTime(date1);
                }
                else
                {
                    include2 = sign1 == 5 || sign1 == 6;
                    endDate = Convert.ToDateTime(date1);
                }
                if (sign2 != -1)
                {
                    if (sign2 == 3 || sign2 == 5)
                    {
                        include1 = sign2 == 5 || sign2 == 6;
                        startDate = Convert.ToDateTime(date2);
                    }
                    else
                    {
                        include2 = sign2 == 5 || sign2 == 6;
                        endDate = Convert.ToDateTime(date2);
                    }
                }

                string sumFieldName = "Sum";
                double currencyMult = 1;
                if (currencyType != -1 && !ignoreCurrencyCalc)
                {
                    sumFieldName = "CurrencySum";
                    currencyMult = okvValues[currencyType];
                }
                
                if (!dtDetail[Convert.ToInt32(detailIndex)].Columns.Contains(sumFieldName))
                {
                    sumFieldName = "Sum";
                    currencyMult = 1;
                }

                if (currentFieldList != string.Empty)
                {
                    int sumSeqIndex = 0;
                    if (currencyType != -1) sumSeqIndex = 1;
                    sumFieldName = currentFieldList.Split(';')[sumSeqIndex];
                }
                if (currentIgnoreExchenge)
                {
                    currencyMult = 1;
                }

                return currencyMult * GetSumValue(
                    dtDetail[Convert.ToInt32(detailIndex)],
                    id,
                    dateFieldName,
                    sumFieldName,
                    startDate,
                    endDate,
                    include1,
                    include2);
            }

            return 0;
        }
        // Знак действия с суммами\сравнения фильтров дат
        private int ParseSign(string formula)
        {
            if (formula[0] == '-') return 1;
            if (formula[0] == '+') return 2;
            if (formula.StartsWith(">=")) return 5;
            if (formula.StartsWith("<=")) return 6;
            if (formula[0] == '>') return 3;
            if (formula[0] == '<') return 4;
            return -1;
        }        
        // Удаляет разделитель из формулы
        private string ParseDelimiter(string formula)
        {
            return formula.Remove(0, 1);
        }        
        // Складывает суммы деталей
        private double ExecuteCommand1(double op1, double op2, int command)
        {
            if (command == 1) return op1 - op2;
            if (command == 2) return op1 + op2;
            return 0;
        }
        // Сравнивает суммы деталей
        private bool ExecuteCommand2(double op1, double op2, int command)
        {
            if (command == 3) return op1 > op2;
            if (command == 4) return op1 < op2;
            if (command == 5) return op1 >= op2;
            if (command == 6) return op1 <= op2;
            return false;
        }
        // Режет знак в зависимости от его длины
        private string CutSign(string formula, int sign)
        {
            formula = formula.Remove(0, 1);
            if (sign > 4) formula = formula.Remove(0, 1);
            return formula;
        }
        // Парсер формулы
        private double ParseFormula(int id, ref string formula)
        {
            double op1, op2, op3, op4;
            double result = 0;
            int sign;
            sign = ParseSign(formula);
            // Если первым делом был знак
            if (sign > 0)
            {
                formula = CutSign(formula, sign);
                // То сначала может идти число
                if (formula.StartsWith("["))
                {
                    op1 = ParseValue(id, ref formula);
                }
                else
                {
                    // ну или опять формула
                    op1 = ParseFormula(id, ref  formula);
                }
                // И второй операнд может быть
                if (formula.StartsWith("["))
                {
                    // Число
                    op2 = ParseValue(id, ref formula);
                }
                else
                {
                    // Формула
                    op2 = ParseFormula(id, ref formula);
                }
                // Операнды считаны - считаем
                return ExecuteCommand1(op1, op2, sign);
            }
            else
            {
                // Первым оператором было число
                if (formula.StartsWith("[")) result = ParseValue(id, ref formula);
                else
                {
                    // Ну или условный оператор...
                    if (formula.StartsWith("if", StringComparison.CurrentCultureIgnoreCase))
                    {
                        formula = formula.Remove(0, 2);
                        // Первый операнд условия
                        op1 = ParseValue(id, ref formula);
                        // Знак сравнения
                        sign = ParseSign(formula);
                        formula = CutSign(formula, sign);
                        // Второй опернд условия
                        op2 = ParseValue(id, ref formula);
                        formula = ParseDelimiter(formula);
                        // Операнд если выражение верно
                        if (formula.StartsWith("["))
                        {
                            op3 = ParseValue(id, ref formula);
                        }
                        else
                        {
                            op3 = ParseFormula(id, ref formula);
                        }
                        formula = ParseDelimiter(formula);
                        // Операнд если выражение неверно
                        if (formula.StartsWith("["))
                        {
                            op4 = ParseValue(id, ref formula);
                        }
                        else
                        {
                            op4 = ParseFormula(id, ref  formula);
                        }
                        // Считаем выражение
                        if (ExecuteCommand2(op1, op2, sign)) 
                        {
                            return op3;
                        }
                        else
                        {
                            return op4;
                        }
                    }
                    // 
                    if (formula.StartsWith("max", StringComparison.CurrentCultureIgnoreCase))
                    {
                        formula = formula.Remove(0, 4);
                        op1 = ParseValue(id, ref formula);
                        formula = ParseDelimiter(formula);
                        op2 = ParseValue(id, ref formula);
                        formula = formula.Remove(0, 1);
                        return Math.Max(op1, op2);
                    }
                }
            }
            return result;
        }

        // Сортировка датасета(возможна иерархическая)
        public DataTable SortDataSet(DataTable dt, string orderStr, bool hierarchicalSort)
        {
            DataTable dtTemp = dt.Clone();
            if (hierarchicalSort)
            {
                DataRow[] rowsMaster = dt.Select("ParentID is null", orderStr);
                for (int i = 0; i < rowsMaster.Length; i++)
                {
                    dtTemp.ImportRow(rowsMaster[i]);
                    DataRow[] rowsDetail = dt.Select(string.Format("ParentID={0}", rowsMaster[i]["id"]), orderStr);
                    for (int j = 0; j < rowsDetail.Length; j++)
                    {
                        dtTemp.ImportRow(rowsDetail[j]);
                    }
                }
                dtTemp.AcceptChanges();
            }
            else
            {
                DataRow[] rows = dt.Select(string.Empty, orderStr);
                for (int i = 0; i < rows.Length; i++)
                {
                    dtTemp.ImportRow(rows[i]);
                }
                dtTemp.AcceptChanges();
            }
            return dtTemp;
        }

        /// <summary>
        /// расчет текущего остатка долга
        /// </summary>
        public double GetCurrentRest(int masterKey, int refOKV)
        {
            DataRow[] drsFactAttract = dtDetail[0].Select(string.Format("{1} = {0}", masterKey, GetParentRefName()));
            DataRow[] drsFactDebt = dtDetail[1].Select(string.Format("{1} = {0}", masterKey, GetParentRefName()));

            double sum = 0;
            string sumFieldName = "Sum";
            double currencyMult = 1;
            if (refOKV != -1)
            {
                currencyMult = okvValues[refOKV];
                sumFieldName = "CurrencySum";
            }

            foreach (DataRow detailRow in drsFactAttract)
            {
                if (!detailRow.IsNull(sumFieldName))
                    sum += Convert.ToDouble(detailRow[sumFieldName]);
            }
            foreach (DataRow detailRow in drsFactDebt)
            {
                if (!detailRow.IsNull(sumFieldName))
                    sum -= Convert.ToDouble(detailRow[sumFieldName]);
            }
            return sum = sum * currencyMult;
        }

        // Некоторые колонки должны заполняться не всегда (например для определенного типа договора только)
        private void CheckColumnCondition(int cellIndex, DataRow sourceRow, DataRow destRow)
        {
            if (columnCondition.ContainsKey(cellIndex))
            {
                if (columnCondition[cellIndex] != string.Empty)
                {
                    string[] conditionList = columnCondition[cellIndex].Split(';');

                    for (int k = 0; k < conditionList.Length; k++)
                    {
                        string[] columnParam;
                        bool isEqual;
                        if (conditionList[k].IndexOf("=") > 0)
                        {
                            columnParam = conditionList[k].Split('=');
                            isEqual = true;
                        }
                        else
                        {
                            columnParam = conditionList[k].Split('!');
                            isEqual = false;
                        }
                        string[] values = columnParam[1].Split(',');
                        string sourceValue = string.Empty;
                        if (sourceRow.Table.Columns.Contains(columnParam[0]))
                        {
                            sourceValue = sourceRow[columnParam[0]].ToString();
                        }
                        else
                        {
                            sourceValue = destRow[columnParam[0]].ToString();
                        }
                        bool saveValue = false;
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (isEqual)
                            {
                                saveValue = saveValue || (values[i] == sourceValue);
                            }
                            else
                            {
                                saveValue = saveValue || (values[i] != sourceValue);
                            }
                        }
                        if (!saveValue)
                        {
                            if (calcColumnTypes[cellIndex] == CalcColumnType.cctDetail
                                || calcColumnTypes[cellIndex] == CalcColumnType.cctRelation)
                            {
                                destRow[cellIndex] = 0;
                            }
                            else
                            {
                                destRow[cellIndex] = DBNull.Value;
                            }
                        }
                    }
                }
            }
        }

        // Парсит формулы чтобы вычислить используемые детали(чтобы за ненужными в базу не лезть) 
        private void FillUsedDetailList()
        {
            if (calcColumnTypes.ContainsValue(CalcColumnType.cctCurrentRest))
            { 
                usedDetails.Add(0);
                usedDetails.Add(1);
            }
            for (int i = 0; i < columnCount; i++)
            {
                if (calcColumnTypes[i] == CalcColumnType.cctDetail ||
                    calcColumnTypes[i] == CalcColumnType.cctDetailText)
                {
                    string value = columnParamList[i]["Formula"];
                    int position1 = value.IndexOf("[");
                    int position2 = value.IndexOf("]");
                    int detailIndex;
                    while (position1 >= 0 && position2 > 0)
                    {
                        detailIndex = Convert.ToInt32(value.Substring(position1 + 1, position2 - position1 - 1));
                        if (!usedDetails.Contains(detailIndex)) usedDetails.Add(detailIndex);
                        value = value.Remove(0, position2 + 1);
                        position1 = value.IndexOf("[");
                        position2 = value.IndexOf("]");
                    }
                }
            }
        }
    }
}
