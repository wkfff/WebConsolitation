using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Krista.FM.Client.Reports.Database.ClsData;
using Krista.FM.Client.Reports.Database.ClsFx;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Capital;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIncome;
using Krista.FM.Client.Reports.Database.FactTables.Planning.CreditIssued;
using Krista.FM.Client.Reports.Database.FactTables.Planning.Garant;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Reports.Common
{
    /// <summary>
    /// Запросы в базу
    /// </summary>
    public class CreditSQLObject
    {
        public const string MTPrefix = "c";
        
        /// <summary>
        /// Собиратель фильтров
        /// </summary>
        public static string CombineFilter(string filterValue, string paramName, string paramValue, string separator = "and")
        {
            var paramString = String.Empty;

            if (paramValue.Length > 0)
            {
                if (filterValue.Length > 0)
                {
                    filterValue = String.Format("{0} {1} ", filterValue, separator);
                }

                var fieldName = paramName;

                if (!paramName.Contains('.'))
                {
                    fieldName = String.Format("{0}.{1}", MTPrefix, fieldName);                    
                }

                // Строковое)
                switch (Convert.ToString(paramValue[0]))
                {
                    case "'":
                        paramValue = paramValue.Remove(0, 1);
                        paramValue = paramValue.Remove(paramValue.Length - 1, 1);
                        paramString = String.Format("({1} like '%{0}%')", paramValue, fieldName);
                        break;
                    case "=":
                    case "<":
                    case ">":
                        {
                            var paramListValues = paramValue.Split(';');
                            paramString = paramListValues.Aggregate(paramString, (current, t) =>
                                String.Format("{2}({1} {0}) {3} ", t, fieldName, current, separator));
                            paramString = paramString.Remove(paramString.Length - 4, 4);
                        }
                        break;
                    default:
                        paramString = String.Format("({1} in ({0}))", paramValue, fieldName);
                        break;
                }
            }

            return String.Format("{0}{1}", filterValue, paramString);
        }

        /// <summary>
        /// Основной фильтр таблицы фактов
        /// </summary>
        public static string GetMainSQLText(
            Dictionary<string, string> mainFilter, 
            Collection<string> optionalList,
            bool clearPrefix)
        {
            var qAndFilters =
                from filter in mainFilter where !optionalList.Contains(filter.Key) select filter;
            var qOrFilters =
                from filter in mainFilter where optionalList.Contains(filter.Key) select filter;

            var listAndFilters = qAndFilters.ToList();
            var listOrFilters = qOrFilters.ToList();

            var conditionValue = String.Empty;

            foreach (var andFilter in listAndFilters)
            {
                if (andFilter.Key.Length > 0 && andFilter.Value.Length > 0)
                {
                    conditionValue = CombineFilter(conditionValue, andFilter.Key, andFilter.Value);
                }
            }

            if (listOrFilters.Count() > 0)
            {
                if (conditionValue.Length > 0)
                {
                    conditionValue += " and ";
                }

                conditionValue += "(";

                var optionalFilterStr = String.Empty;

                foreach (var orFilter in listOrFilters)
                {
                    if (orFilter.Key.Length > 0 && orFilter.Value.Length > 0)
                    {
                        optionalFilterStr = CombineFilter(optionalFilterStr, orFilter.Key, orFilter.Value, " or ");
                    }
                }

                conditionValue += optionalFilterStr + ")";
            }

            if (clearPrefix)
            {
                conditionValue = conditionValue.Replace(String.Format("{0}.", MTPrefix), String.Empty);
            }

            return conditionValue;
        }
    }

    // Тип колонки
    public enum FieldType
    {
        ftMain = 0, // Поле из таблицы фактов
        ftCalc = 1, // Обработанное поле таблицы фактов / прочая хрень        
    }

    // Тип вычисляемой колонки
    public enum CalcColumnType
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
        cctCapNumDateDiscount = 40,
        cctGrntNumRegPercent = 41,
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
        cctGarantTypeNumStartDate = 67,
        cctPrincipalSum = 68,
        cctOperationDates = 71,
        cctNearestDown = 72,
        cctListDocs = 73,
        cctOrganization2 = 74,
        cctBorrowKind = 76,
        cctOrganization3INNKPP = 77,
        cctOrganizationINNKPP = 78,
        cctPrincipalCurrencySum = 79,
        cctCapMOFoundation = 81,
        cctPrincipalOKV = 82,
        cctNearestPercent = 83,
        cctRecordCount = 84,
        cctPrincipalPercent = 85,
        cctContractDateNum = 86,
        cctPrincipalNum = 87,
        cctPrincipalPeriod = 88,
        cctCapKind = 90,
        cctCapCouponPeriod = 91,
        cctCreditPeriodRate = 92,
        cctOKVFullName = 94,
        cctPercentTextSplitter = 95,
        cctPrincipalOKVName = 96,
        cctCommonBookValue = 97,
        cctPrincipalDocExist = 98,
        cctCreditNumPercent = 99,
        cctCreditNumContractDateOrg = 100,
        cctGarantOrgTypeNumDate = 101,
        cctCreditNumDatePercent = 102,
        cctAlterationNumDocDate = 103,
        cctPercentGroupText = 104,
        cctFlagColumn = 105,
        cctCommonTemplate = 106,
        cctCalcBookValue = 107,
        cctUndefined = -999,
    }

    class JournalTextParams
    {
        public DataRow row;
        public DateTime startDate = DateTime.MinValue;
        public DateTime endDate = DateTime.MaxValue;
        public string fieldName;
        public bool isMaxMin;
        public bool onlyValues;
        public bool usePercentSym = true;
        public bool useJoining = false;
        public string splitter = " ";

        public JournalTextParams(DataRow rowMaster, string field)
        {
            row = rowMaster;
            fieldName = field;
        }
    }

    // объект для полученных кредитов
    class CommonDataObject
    {
        public IScheme scheme;        
        // Хреново глобальные переменные текущей записи, но тащить кучу параметров в парсер формул желания нету
        private string creditEndDate;
        private int currencyType = -1;
        // Результирующий датасет
        private DataTable dtResult;
        // Фильтры таблицы фактов
        public Dictionary<string, string> mainFilter = new Dictionary<string, string>();
        // Фильтры используемые как "или"
        public Collection<string> optionalFilters = new Collection<string>();
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
        private readonly Dictionary<int, CalcColumnType> calcColumnTypes = new Dictionary<int, CalcColumnType>();
        public Dictionary<int, Dictionary<string, string>> columnParamList = new Dictionary<int, Dictionary<string, string>>();
        // Куча данных всех используемых деталей
        public DataTable[] dtDetail;
        // Данные журнала процентов по все договорам попадающим в отчет
        public DataTable dtJournalPercent = new DataTable();
        // Общий список курсов валют договора с которыми попадают в отчет
        public DataTable dtExchange = new DataTable();
        // Строка сортировки результата
        public string sortString = String.Empty;
        // Нужно ли делать двухуровневую сортировку результата
        public bool hierarchicalSort;
        // Кэш валют : храним нужные курсы валют(чтобы не лазать за ними по каждому договору в базу)
        private readonly Dictionary<int, DataTable> exchangeRate = new Dictionary<int,DataTable>();
        // Кэш справочников
        private readonly Dictionary<string, DataTable> bookCache = new Dictionary<string, DataTable>();
        // Кэш валют 2(чтобы не искать каждый раз в датасетах)
        readonly Collection<int> okvCodes = new Collection<int>();
        public Dictionary<int, decimal> okvValues = new Dictionary<int, decimal>();
        // Коллекция используемых деталей(чтобы не выкачивать данные по всем)
        readonly Collection<int> usedDetails = new Collection<int>();
        // Производить ли пересчет сумм валютных договоров
        public bool ignoreCurrencyCalc;
        // Кэш справочников
        public Dictionary<int, int> realSummaryIndex = new Dictionary<int, int>();

        private string fullDetailText = String.Empty;

        public Dictionary<int, List<string>> columnTemplateFields = new Dictionary<int, List<string>>();

        // Нужно ли добавлять строку с итогами
        public bool useSummaryRow = true;

        // Текущая рассчитываемая колонка
        private int currentColumnIndex = -1;
        // Список полей для суммирования в текущей колонке
        private string currentFieldList = String.Empty;
        // Список полей для суммирования в текущей колонке
        private bool currentIgnoreExchenge;
        // Брать курс валюты на предыдущий день и более ранние
        public bool exchangePrevDay;
        // Убрать в итоговом датасете служебные поля
        public bool removeServiceFields;

        // разделитель списков дат\значений
        public string valuesSeparator = String.Empty;
        // дописка каждому элменту в текстовке колонки
        public string textAppendix = String.Empty;
        // фиксированный фильтр
        public string fixedFilter = String.Empty;

        public Collection<DataRow> sumIncludedRows = new Collection<DataRow>();

        // кэш сущностей
        private readonly Dictionary<string, IEntity> cacheEntity = new Dictionary<string, IEntity>();

        // чтобы нечитать курсы из бд, если пользователь вводит все руками в параметрах
        public Dictionary<int, decimal> fixedExchangeRate = new Dictionary<int, decimal>();

        public bool ignoreZeroCurrencyRows;
        public bool ignoreNegativeSum;
        public bool clearQueryFields;

        public string externalFilter = String.Empty;
        private bool writeFullDetailText;

        // полный нефильтрованные наборы
        public Dictionary<int, DataTable> safespotDetail = new Dictionary<int, DataTable>();

        protected string[] dtlKeysList;
        protected string[] loDatesList;
        protected string[] hiDatesList;
        protected string[] mdDatesList;

        public string ParamActualDate = "ActualDate";
        public string ParamValue1 = "ParamValue1";
        public string ParamValue2 = "ParamValue2";
        public string ParamValue3 = "ParamValue3";
        public string ParamValue4 = "ParamValue4";
        public string ParamDataType = "DataType";
        public string ParamName = "Name";
        public string ParamFormula = "Formula";
        public string ParamFieldList = "FieldList";
        public string ParamIgnoreExchange = "IgnoreExchange";
        public string ParamOnlyDates = "OnlyDates";
        public string ParamOnlyValues = "OnlyValues";
        public string ParamSumValueType = "SumValueType";
        public string ParamPlanDate = "ParamPlanDate";

        private string objectKey = String.Empty;
        public string ObjectKey
        {
            get { return objectKey; }
            set { objectKey = value; }
        }

        protected virtual void InitDetailDateLists()
        {
            const int detailCount = 0;
            dtlKeysList = new string[detailCount];
            loDatesList = new string[detailCount];
            mdDatesList = new string[detailCount];
            hiDatesList = new string[detailCount];
        }

        protected virtual string[] GetDetailKeys()
        {
            return dtlKeysList;
        }

        public virtual string[] GetStartDates()
        {
            return loDatesList;
        }

        public virtual string[] GetMiddleDates()
        {
            return mdDatesList;
        }

        public virtual string[] GetEndDates()
        {
            return hiDatesList;
        }

        protected virtual string GetMainSQLQuery()
        {
            var clearPrefix = InnerJoins().Count == 0;
            var result = CreditSQLObject.GetMainSQLText(mainFilter, optionalFilters, clearPrefix);

            // если был внешний фильтр со страницы
            if (externalFilter.Length > 0)
            {
                var splitter = " and ";
                
                if (result.Length == 0)
                {
                    splitter = String.Empty;
                }
                
                result = String.Format("{0}{1}{2}", result, splitter, externalFilter);
            }

            return result;
        }

        protected virtual string GetContractDesc()
        {
            return String.Empty;
        }

        public virtual string GetParentRefName()
        {
            return String.Empty;
        }

        protected virtual string GetCollateralKey()
        {
            return String.Empty;
        }

        protected virtual string GetMainTableKey()
        {
            return objectKey;
        }

        protected virtual string GetJournalKey()
        {
            return t_S_JournalPercentCI.internalKey;
        }

        protected virtual string GetDocListKey()
        {
            return String.Empty;
        }

        // фильтрация деталей под конкретную запись
        public virtual void FilterDetailTables(object masterKey)
        {
        }

        public virtual void FilterDetailTables(object masterKey, string dateStr)
        {
        }

        // бекап фильтруемых деталей
        protected virtual void MakeSafeSpot()
        {
        }

        protected virtual string GetAlterationKey()
        {
            return String.Empty;
        }

        // Создатель таблиц строковых полей
        private void CreateExchangeTable(int totalColumnCount)
        {
            dtExchange = new DataTable();

            for (var i = 0; i < totalColumnCount; i++)
            {
                dtExchange.Columns.Add(Convert.ToString(i), typeof(String));
            }
        }

        // Подчищатор и реинкарнатор объекта
        public void InitObject(IScheme currentScheme)
        {
            okvCodes.Clear();
            okvValues.Clear();
            summaryColumnIndex.Clear();
            exchangeRate.Clear();
            mainFilter.Clear();
            optionalFilters.Clear();
            columnList.Clear();
            calcColumnTypes.Clear();
            columnParamList.Clear();
            reportParams.Clear();
            columnCondition.Clear();
            dtJournalPercent.Clear();
            usedDetails.Clear();
            fixedExchangeRate.Clear();
            columnTemplateFields.Clear();

            columnCount = 0;

            CreateExchangeTable(3);
            
            hierarchicalSort = false;
            sortString = String.Empty;
            ignoreCurrencyCalc = false;
            useSummaryRow = true;
            currentColumnIndex = -1;
            currentFieldList = String.Empty;
            currentIgnoreExchenge = false;
            exchangePrevDay = false;
            removeServiceFields = false;

            scheme = currentScheme;

            valuesSeparator = ",";
            textAppendix = String.Empty;

            clearQueryFields = false;

            InitDetailDateLists();
        }

        // Добавитель колонок таблицы фактов
        public void AddDataColumn(string colName, string typeName)
        {
            var param = new Dictionary<string, string> {{ParamName, colName}, {ParamDataType, typeName}};
            AddColumn(FieldType.ftMain, CalcColumnType.cctUndefined, param);
        }

        // Добавитель колонок таблицы фактов
        public void AddDataColumn(string colName)
        {
            var param = new Dictionary<string, string> {{ParamName, colName}};
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

        public void AddExchangeColumn(string actualDate)
        {
            var paramColumn = new Dictionary<string, string> {{ParamActualDate, actualDate}};
            AddCalcColumn(CalcColumnType.cctOKVValue, paramColumn);
        }

        public void AddParamColumn(string param1, string param2)
        {
            AddParamColumn(CalcColumnType.cctUndefined, param1, param2);
        }

        public void AddParamColumn(CalcColumnType ct, string param1, string param2)
        {
            var paramColumn = new Dictionary<string, string> {{ParamValue1, param1}, {ParamValue2, param2}};
            AddCalcColumn(ct, paramColumn);
        }

        public void AddParamColumn(string param1, string param2, string param3, string param4)
        {
            AddParamColumn(CalcColumnType.cctUndefined, param1, param2, param3, param4);
        }

        public void AddParamColumn(CalcColumnType ct, string param1, string param2, string param3, string param4)
        {
            var paramColumn = new Dictionary<string, string>
                                  {
                                      {ParamValue1, param1},
                                      {ParamValue2, param2},
                                      {ParamValue3, param3},
                                      {ParamValue4, param4}
                                  };

            AddCalcColumn(ct, paramColumn);
        }

        public void AddParamColumn(CalcColumnType ct, string param1, List<string> fieldList)
        {
            columnTemplateFields.Add(columnList.Count, fieldList);
            AddCalcColumn(ct, new Dictionary<string, string> { {ParamValue1, param1 } });
        }

        public void AddDetailTextColumn(string formula, string paramName, string paramValue)
        {
            var paramColumn = new Dictionary<string, string> {{ParamFormula, formula}, {paramName, paramValue}};
            AddCalcColumn(CalcColumnType.cctDetailText, paramColumn);
        }

        public void AddParamColumn(string param)
        {
            AddParamColumn(CalcColumnType.cctUndefined, param);
        }

        public void AddParamColumn(CalcColumnType ct, string param)
        {
            var paramColumn = new Dictionary<string, string> {{ParamValue1, param}};
            AddCalcColumn(ct, paramColumn);
        }

        public void AddCalcNamedColumn(CalcColumnType colType, string columnName)
        {
            var paramColumn = new Dictionary<string, string> {{ParamName, columnName}};
            AddCalcColumn(colType, paramColumn);
        }

        public void AddDetailColumn(string formula, string paramName, string paramValue)
        {
            var paramColumn = new Dictionary<string, string> {{ParamFormula, formula}, {paramName, paramValue}};
            AddCalcColumn(CalcColumnType.cctDetail, paramColumn);
        }

        public void AddDetailColumn(string formula, string fieldNames, bool ignoreExchange)
        {
            var paramColumn = new Dictionary<string, string>
                                  {
                                      {ParamFormula, formula},
                                      {ParamFieldList, fieldNames},
                                      {ParamIgnoreExchange, Convert.ToString(ignoreExchange)}
                                  };

            AddCalcColumn(CalcColumnType.cctDetail, paramColumn);
        }

        public void AddDetailColumn(string formula, string fieldNames)
        {
            var paramColumn = new Dictionary<string, string>
                                  {
                                      {ParamFormula, formula}, {ParamFieldList, fieldNames}
                                  };

            AddCalcColumn(CalcColumnType.cctDetail, paramColumn);
        }

        public void AddDetailColumn(string formula)
        {
            var paramColumn = new Dictionary<string, string> {{ParamFormula, formula}};
            AddCalcColumn(CalcColumnType.cctDetail, paramColumn);
        }

        public void SetColumnNameParam(string columnName)
        {
            columnParamList[columnList.Count - 1][ParamName] = columnName;
        }

        public void SetColumnParam(string paramName, string paramValue)
        {
            columnParamList[columnList.Count - 1][paramName] = paramValue;
        }

        public void SetColumnCondition(string conditionField, string conditionValue)
        {
            columnParamList[columnList.Count - 1][conditionField] = conditionValue;
        }

        // Создание структуры для вывода
        private void CreateFields()
        {
            dtResult = new DataTable();

            for (var i = 0; i < columnCount; i++)
            {
                var dataColumn = dtResult.Columns.Add();
                var dataType = ReportConsts.ftString;
                
                // Все поля кроме суммируемых деталей будем считать строковыми
                if (calcColumnTypes[i] == CalcColumnType.cctDetail
                    || calcColumnTypes[i] == CalcColumnType.cctCurrentRest
                    || calcColumnTypes[i] == CalcColumnType.cctRelation)
                {
                    dataType = ReportConsts.ftDecimal;
                    if (!summaryColumnIndex.Contains(i)) summaryColumnIndex.Add(i);
                }
                
                dataColumn.DataType = Type.GetType(String.Format("System.{0}", dataType));
                
                if (columnParamList[i].ContainsKey(ParamName))
                {
                    if (!dtResult.Columns.Contains(columnParamList[i][ParamName]))
                    {
                        dataColumn.ColumnName = columnParamList[i][ParamName];
                    }
                }
                
                if (columnParamList[i].ContainsKey(ParamDataType))
                {
                    dataColumn.DataType = Type.GetType(String.Format("System.{0}", columnParamList[i][ParamDataType]));
                }
            }
            // служебные колонки для сортировки(надо бы после формирования потереть)
            if (!dtResult.Columns.Contains(TempFieldNames.SortCreditEndDate)) 
                dtResult.Columns.Add(TempFieldNames.SortCreditEndDate, typeof(DateTime));
            if (!dtResult.Columns.Contains(f_S_Creditincome.RefOKV))
                dtResult.Columns.Add(f_S_Creditincome.RefOKV, typeof(Int32));
            if (!dtResult.Columns.Contains(f_S_Creditincome.ParentID))
                dtResult.Columns.Add(f_S_Creditincome.ParentID, typeof(Int32));
            if (!dtResult.Columns.Contains(f_S_Creditincome.id))
                dtResult.Columns.Add(f_S_Creditincome.id, typeof(Int32));
        }

        // Заполнение полей из БД
        private void FillDataField(DataRow destRow, DataRow sourceRow, int colIndex)
        {
            var value = sourceRow[columnParamList[colIndex][ParamName]];
            if (value == DBNull.Value) return;
            
            if (sourceRow.Table.Columns[columnParamList[colIndex][ParamName]].DataType == typeof(DateTime))
            {
                // с датами надо похитрее - конвертим в краткое представление без времени
                destRow[colIndex] = GetDateValue(value);
            }
            else
            {
                destRow[colIndex] = value;
            }
        }

        protected IEntity GetEntity(string key)
        {
            // если много записей с кучей справочников, то получение сущности по ключу = тормоз
            if (!cacheEntity.ContainsKey(key))
            {
                cacheEntity.Add(key, scheme.RootPackage.FindEntityByName(key));
            }

            return cacheEntity[key];
        }

        private string GetCommonDetailField(string detailKey, object id, string fieldName)
        {
            var book = GetEntity(detailKey);
            return GetBookValue(book, id, fieldName, true, true);
        }

        private string GetAlterationField(object id, string fieldName)
        {
            return GetCommonDetailField(GetAlterationKey(), id, fieldName);
        }

        private string GetGarantPrincipalField(object id, string fieldName)
        {
            return GetCommonDetailField(t_S_PrincipalContrGrnt.internalKey, id, fieldName);
        }

        // Заполнение вычисляемых полей
        private void FillCalcField(DataRow destRow, DataRow sourceRow, int colIndex)
        {
            if (sourceRow == null)
            {
                return;
            }

            int id = Convert.ToInt32(sourceRow[f_S_Creditincome.id]);

            if (calcColumnTypes[colIndex] == CalcColumnType.cctCalcBookValue
                || calcColumnTypes[colIndex] == CalcColumnType.cctCommonBookValue)
            {
                var colParams = columnParamList[colIndex];
                var refField = colParams[ParamValue1];
                var tableKey = colParams[ParamValue2];
                var fieldName = colParams[ParamValue3];
                var entity = GetEntity(tableKey);
                var refValue = calcColumnTypes[colIndex] == CalcColumnType.cctCommonBookValue ? 
                    sourceRow[refField] :
                    destRow[refField];
                destRow[colIndex] = GetBookValue(entity, refValue, fieldName);
            }

            if (calcColumnTypes[colIndex] == CalcColumnType.cctCommonTemplate)
            {
                var colParams = columnParamList[colIndex];
                var templateStr = colParams[ParamValue1];
                var lstFields = columnTemplateFields[colIndex];
                var lstValues = new object[lstFields.Count];
                var counter = 0;

                foreach (var fieldName in lstFields)
                {
                    var lstValue = sourceRow[fieldName];

                    if (sourceRow.Table.Columns[fieldName].DataType == typeof(DateTime))
                    {
                        lstValue = Convert.ToDateTime(lstValue).ToShortDateString();
                    }

                    lstValues[counter++] = lstValue;
                }


                destRow[colIndex] = String.Format(templateStr, lstValues);
            }

            // Периодичность выплаты
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCreditPeriodRate)
            {
                var periodDetail = GetEntity(fx_S_Periodicity.internalKey);
                destRow[colIndex] =
                    GetBookValue(periodDetail, sourceRow[f_S_Creditincome.RefPeriodRate], fx_S_Periodicity.Name);
            }

            // Число записей в детали
            if (calcColumnTypes[colIndex] == CalcColumnType.cctRecordCount)
            {
                var detailIndex = Convert.ToInt32(columnParamList[colIndex][ParamValue1]);
                var drsSelect = dtDetail[detailIndex].Select(String.Format("{0} = {1}", GetParentRefName(), id));
                destRow[colIndex] = drsSelect.Length;
            }

            // ЦБ МО Основание
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapMOFoundation)
            {
                destRow[colIndex] = String.Format("Московский областной внутренний облигационный займ {0} года",
                    Convert.ToDateTime(sourceRow[f_S_Capital.StartDate]).Year);
            }

            // Наименование договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctAlterationNumDocDate)
            {
                var strNumb = GetAlterationField(id, t_S_AlterationGrnt.Num);
                var strDate = GetAlterationField(id, t_S_AlterationGrnt.DocDate);

                if (strNumb.Length > 0 && strDate.Length > 0)
                {
                    destRow[colIndex] = String.Format("{0} от {1}", 
                        strNumb, 
                        Convert.ToDateTime(strDate).ToShortDateString());
                }
            }

            // Наименование договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPrincipalDoc)
            {
                destRow[colIndex] = GetGarantPrincipalField(id, t_S_PrincipalContrGrnt.PrincipalDoc);
            }

            // Наименование договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPrincipalDocExist)
            {
                var docValue = GetGarantPrincipalField(id, t_S_PrincipalContrGrnt.PrincipalDoc);

                if (docValue.Length > 0)
                {
                    destRow[colIndex] = 1;
                }
                else
                {
                    destRow[colIndex] = 0;
                }
            }

            // Дата выдачи
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPrincipalStartDate)
            {
                var dateStr = GetGarantPrincipalField(id, t_S_PrincipalContrGrnt.StartDate);

                if (dateStr.Length > 0)
                {
                    destRow[colIndex] = GetDateValue(dateStr);
                }
            }

            // Объем долга
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPrincipalSum)
            {
                destRow[colIndex] = GetGarantPrincipalField(id, t_S_PrincipalContrGrnt.Sum);
            }

            // Дата выдачи
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPrincipalEndDate)
            {
                var dateStr = GetGarantPrincipalField(id, t_S_PrincipalContrGrnt.EndDate);

                if (dateStr.Length > 0)
                {
                    destRow[colIndex] = GetDateValue(dateStr);
                }
            }

            // Дата выдачи
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPrincipalNum)
            {
                destRow[colIndex] = GetGarantPrincipalField(id, t_S_PrincipalContrGrnt.Num);
            }

            // Периодичность выдачи
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPrincipalPeriod)
            {
                var refPeriod = GetGarantPrincipalField(id, t_S_PrincipalContrGrnt.RefPeriodDebt);
                var notePeriod = GetGarantPrincipalField(id, t_S_PrincipalContrGrnt.Note);

                if (refPeriod.Length > 0)
                {
                    destRow[colIndex] = String.Format("{0} {1}", 
                        GetBookValue(GetEntity(fx_S_Periodicity.internalKey), refPeriod, fx_S_Periodicity.Name),
                        notePeriod).Trim();
                }
            }

            // Валюта гарантируемого договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPrincipalOKV)
            {
                var okvRef = GetGarantPrincipalField(id, t_S_PrincipalContrGrnt.RefOKV);

                if (okvRef.Length == 0)
                {
                    okvRef = ReportConsts.codeRUBStr;
                }

                destRow[colIndex] = okvRef;
            }

            // Валюта гарантируемого договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPrincipalOKVName)
            {
                var okvRef = GetGarantPrincipalField(id, t_S_PrincipalContrGrnt.RefOKV);
                
                if (okvRef.Length == 0)
                {
                    okvRef = ReportConsts.codeRUBStr;
                }

                var okvBook = GetEntity(d_OKV_Currency.internalKey);
                destRow[colIndex] = GetBookValue(okvBook, okvRef, d_OKV_Currency.Name);
            }

            // Процент гарантируемого договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPrincipalPercent)
            {
                destRow[colIndex] = GetGarantPrincipalField(id, t_S_PrincipalContrGrnt.CreditPercent);
            }

            // Объем долга в валюте
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPrincipalCurrencySum)
            {
                destRow[colIndex] = GetGarantPrincipalField(id, t_S_PrincipalContrGrnt.CurrencySum);
            }

            // Мин. дата операции по договору
            if (calcColumnTypes[colIndex] == CalcColumnType.cctMinOperationDate)
            {
                var drAttract = dtDetail[Convert.ToInt32(columnParamList[colIndex][ParamValue1])].
                    Select(String.Format("{0} = {1}", GetParentRefName(), id),
                    String.Format("{0} asc", t_S_FactAttractCI.FactDate));
                
                if (drAttract.Length > 0)
                {
                    destRow[colIndex] = GetDateValue(drAttract[0][t_S_FactAttractCI.FactDate]);
                }
            }

            // Все даты операций по детали
            if (calcColumnTypes[colIndex] == CalcColumnType.cctAllOperationDates)
            {
                var drAttract = dtDetail[Convert.ToInt32(columnParamList[colIndex][ParamValue1])].
                    Select(String.Format("{0} = {1}", GetParentRefName(), id),
                    String.Format("{0} asc", t_S_FactDebtCI.FactDate));

                var dateStr = drAttract.Aggregate(
                    String.Empty, (current, row) =>
                        String.Format("{0}, {1}", current, GetDateValue(row[t_S_FactDebtCI.FactDate])));

                destRow[colIndex] = dateStr.TrimStart(',').TrimStart(' ');
            }

            // Все даты операций по детали до даты отчета
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOperationDates)
            {
                var colParams = columnParamList[colIndex];
                var detailIndex = Convert.ToInt32(colParams[ParamValue1]);
                var endDateField = GetEndDates()[detailIndex];
                var resultField = endDateField;
                if (colParams.ContainsKey(ParamValue2)) resultField = colParams[ParamValue2];
                var endDateValue = GetDateValue(reportParams[ReportConsts.ParamHiDate]);

                if (colParams.ContainsKey(ParamValue3))
                {
                    var ignorePeriod = Convert.ToBoolean(colParams[ParamValue3]);

                    if (ignorePeriod)
                    {
                        endDateValue = DateTime.MaxValue.ToShortDateString();
                    }
                }

                var drAttract = dtDetail[detailIndex].Select(
                        String.Format("{0} = {1} and {2} <= '{3}'", GetParentRefName(), id, endDateField, endDateValue),
                        String.Format("{0} asc", endDateField));
                var resStr = String.Empty;

                foreach (var row in drAttract)
                {
                    var appendValue = Convert.ToString(row[resultField]);
                    if (resultField.Contains("Date")) appendValue = GetDateValue(row[resultField]);
                    resStr = String.Format("{0}; {1}", resStr, appendValue);
                }
                
                destRow[colIndex] = resStr.TrimStart(';').TrimStart();
            }

            // Все даты операций по детали до даты отчета
            if (calcColumnTypes[colIndex] == CalcColumnType.cctNearestDown)
            {
                var detailIndex = Convert.ToInt32(columnParamList[colIndex][ParamValue1]);
                var drNearest = GetNearestRow(detailIndex,
                    GetDateValue(reportParams[ReportConsts.ParamHiDate]), id, false);
                
                if (drNearest != null)
                {
                    destRow[colIndex] = drNearest[columnParamList[colIndex][ParamValue2]];
                }
            }

            // Все даты операций по детали до даты отчета
            if (calcColumnTypes[colIndex] == CalcColumnType.cctListDocs)
            {
                var filterList = columnParamList[colIndex][ParamValue1].Split(';');
                var filterRows = String.Empty;
                var kindCodes = new Collection<string>();
                
                for (var i = 0; i < filterList.Length; i++)
                {
                    if (filterList[i].Length == 0) continue;

                    var filterParts = filterList[i].Split('=');

                    if (filterParts[0] != t_S_ListContractCl.RefViewContract)
                    {
                        if (filterParts[1].Contains(","))
                            filterList[i] = String.Format("{0} in ({1})", filterParts[0], filterParts[1]);
                        filterRows = String.Format("{0} and {1}", filterRows, filterList[i]);
                    }
                    else
                    {
                        kindCodes = new Collection<string>(filterParts[1].Split(','));
                    }
                }

                var listDocsEntity = GetEntity(GetDocListKey());
                var viewKindEntity = GetEntity(d_S_ViewContract.internalKey);
                var drsDocs = GetTableRows(listDocsEntity, id, true, filterRows);
                var resStr = String.Empty;

                foreach (var t in drsDocs)
                {
                    var insertRecord = true;

                    if (kindCodes.Count > 0)
                    {
                        var kindCode = GetBookValue(viewKindEntity,
                                                    t[t_S_ListContractCl.RefViewContract], d_S_ViewContract.Code);
                        insertRecord = kindCodes.Contains(kindCode);
                    }

                    if (!insertRecord) continue;
                    var appendValue = String.Format("{0} от {1}",
                                                    t[t_S_ListContractCl.NumberContract],
                                                    GetDateValue(t[t_S_ListContractCl.DataContract]));
                    resStr = String.Format("{0}; {1}", resStr, appendValue);
                }
                
                destRow[colIndex] = resStr.Trim().Trim(';').Trim();
            }

            // Тип договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctContractType)
            {
                var creditType = GetEntity(d_S_TypeContract.internalKey);
                destRow[colIndex] =
                    GetBookValue(creditType, sourceRow[f_S_Creditincome.RefTypeContract], d_S_TypeContract.Name);
            }

            // Периодичность выплаты
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapCouponPeriod)
            {
                var periodDetail = GetEntity(fx_S_CPPeriodicity.internalKey);
                destRow[colIndex] =
                    GetBookValue(periodDetail, sourceRow[f_S_Capital.RefPeriodicity], fx_S_CPPeriodicity.Name);
            }

            // Вид заимствования
            if (calcColumnTypes[colIndex] == CalcColumnType.cctBorrowKind)
            {
                var creditType = GetEntity(d_S_KindBorrow.internalKey);
                destRow[colIndex] =
                    GetBookValue(creditType, sourceRow[f_S_Creditincome.RefKindBorrow], d_S_KindBorrow.Name);
            }

            // Дата муниципальной регистрации для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapNPANames)
            {
                destRow[colIndex] = String.Format("{0} № {1} от {2} {3}",
                    sourceRow[f_S_Capital.NameNPA], sourceRow[f_S_Capital.NumberNPA],
                    GetDateValue(sourceRow[f_S_Capital.DateNPA]), sourceRow[f_S_Capital.NameOrg]);
            }

            // Дата муниципальной регистрации для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapRegDateNum)
            {
                destRow[colIndex] = String.Format("{0}, {1}",
                    GetDateValue(sourceRow[f_S_Capital.RegEmissionDate]), sourceRow[f_S_Capital.RegNumber]);
            }

            // Номер ценной бумаги + Дата муниципальной регистрации
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapNumberRegDate)
            {
                destRow[colIndex] = String.Format("{1} от {0}",
                    GetDateValue(sourceRow[f_S_Capital.RegEmissionDate]), sourceRow[f_S_Capital.NumberCapital]);
            }

            // Периодичность выплат
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapPayPeriod)
            {
                GetSumValue(dtDetail[7], id, t_S_CPPlanService.EndDate, t_S_CPPlanService.Sum,
                    DateTime.MinValue, DateTime.MaxValue, true, true);
                if (sumIncludedRows.Count > 0)
                {
                    destRow[colIndex] = String.Format("1 раз в {0} день", sumIncludedRows[0][t_S_CPPlanService.Period]);
                }
            }

            // Форма существования для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapKind)
            {
                var kindBook = GetEntity(fx_S_KindCapital.internalKey);
                destRow[colIndex] = GetBookValue(kindBook, sourceRow[f_S_Capital.RefKindCap], fx_S_KindCapital.Name);
            }

            // Форма существования для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapForm)
            {
                var formBook = GetEntity(fx_S_FormCapital.internalKey);
                destRow[colIndex] = GetBookValue(formBook, sourceRow[f_S_Capital.RefSFormCap], fx_S_FormCapital.Name);
            }

            // Номер + вид для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapNameKind)
            {
                var kindBook = GetEntity(d_S_Capital.internalKey);
                var kindName = GetBookValue(kindBook, sourceRow[f_S_Capital.RefSCap], d_S_Capital.Name);
                destRow[colIndex] = String.Format("{0} {1}", kindName, String.Empty);
            }

            // Заголовок для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapNumDateDiscount)
            {
                var minPercent = GetNearestPercent(sourceRow, reportParams[ReportConsts.ParamHiDate]);
                var maxPercent = GetNearestPercent(sourceRow, DateTime.MaxValue.ToShortDateString());
                var percentFormat = "{0}";

                if (minPercent != maxPercent && (minPercent.Length > 0 || maxPercent.Length > 0))
                {
                    percentFormat = "{0}-{1}";
                }

                var percentText = String.Format(percentFormat, minPercent, maxPercent);
                destRow[colIndex] = String.Format("{0} ({1}% год.)", sourceRow[f_S_Capital.OfficialNumber], percentText);
            }

            // Заголовок для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctGrntNumRegPercent)
            {
                var textParams = new JournalTextParams(sourceRow, t_S_JournalPercentCI.CreditPercent)
                                     {
                                         isMaxMin = true,
                                         onlyValues = true
                                     };

                destRow[colIndex] = String.Format("№ {0} {1}, {2}",
                    sourceRow[f_S_Guarantissued.Num], GetDateValue(sourceRow[f_S_Guarantissued.RegDate]),
                    GetPercentText(textParams));
            }

            // Заголовок для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapNum)
            {
                destRow[colIndex] = String.Format("{0}, {1}, {2}",
                    sourceRow[f_S_Capital.CodeCapital],
                    sourceRow[f_S_Capital.SeriesCapital],
                    sourceRow[f_S_Capital.NumberCapital]);
            }

            // Сумма вычисляемая для валютных
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCalcSum)
            {
                if (currencyType == -1)
                {
                    destRow[colIndex] = sourceRow[f_S_Creditincome.Sum];
                }
                else
                {
                    if (sourceRow[f_S_Creditincome.CurrencySum] != DBNull.Value)
                        destRow[colIndex] = okvValues[currencyType] * Convert.ToDecimal(sourceRow[f_S_Creditincome.CurrencySum]);
                }
            }

            // Сумма вычисляемая для валютных
            if (calcColumnTypes[colIndex] == CalcColumnType.cctNativeSum)
            {
                if (currencyType == -1)
                {
                    destRow[colIndex] = sourceRow[f_S_Creditincome.Sum];
                }
                else
                {
                    destRow[colIndex] = sourceRow[f_S_Creditincome.CurrencySum];
                }
            }

            // Сумма вычисляемая для валютных гарантий
            if (calcColumnTypes[colIndex] == CalcColumnType.cctGarantCalcSum)
            {
                if (currencyType == -1)
                {
                    destRow[colIndex] = sourceRow[f_S_Guarantissued.DebtSum];
                }
                else
                {
                    if (sourceRow[f_S_Guarantissued.CurrencySum] != DBNull.Value)
                        destRow[colIndex] = okvValues[currencyType] * Convert.ToDecimal(sourceRow[f_S_Guarantissued.CurrencySum]);
                }
            }

            // Сортировка
            if (calcColumnTypes[colIndex] == CalcColumnType.cctSortStatus)
            {
                destRow[colIndex] = "1";

                if (Convert.ToString(sourceRow[f_S_Creditincome.RefSStatusPlan]) == "4")
                {
                    destRow[colIndex] = "0";
                }
            }

            // Организации
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOrganizationRegion)
            {
                var orgBook = GetEntity(d_Organizations_Plan.internalKey);

                if (Convert.ToString(sourceRow[f_S_Creditincome.RefOrganizations]) == "-1")
                {
                    var orgRegion = GetEntity(d_Regions_Plan.internalKey);
                    destRow[colIndex] = GetBookValue(orgRegion, sourceRow[f_S_Creditincome.RefRegions], d_Regions_Plan.Name);
                }
                else
                {
                    destRow[colIndex] = GetBookValue(orgBook, sourceRow[f_S_Creditincome.RefOrganizations], d_Organizations_Plan.Name);
                }
            }

            // Организации (для гарантий)
            if (calcColumnTypes[colIndex] == CalcColumnType.cctRegion)
            {
                var orgRegion = GetEntity(d_Regions_Plan.internalKey);
                destRow[colIndex] = GetBookValue(orgRegion, sourceRow[f_S_Creditincome.RefRegions], d_Regions_Plan.Name);
            }

            // Организации (для гарантий)
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOrganizations)
            {
                var orgBook = GetEntity(d_Organizations_Plan.internalKey);
                var orgName1 = GetBookValue(orgBook, sourceRow[f_S_Guarantissued.RefOrganizationsPlan2], d_Organizations_Plan.Name);
                var orgName2 = GetBookValue(orgBook, sourceRow[f_S_Guarantissued.RefOrganizations], d_Organizations_Plan.Name);
                var orgName3 = GetBookValue(orgBook, sourceRow[f_S_Guarantissued.RefOrganizationsPlan3], d_Organizations_Plan.Name);
                destRow[colIndex] = String.Format("Гарант: {0}, принципал: {1}, бенефициар: {2}",
                    orgName1, orgName2, orgName3);
            }

            // Бенефициатор
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOrganization3)
            {
                var orgBook = GetEntity(d_Organizations_Plan.internalKey);
                destRow[colIndex] = GetBookValue(orgBook, sourceRow[f_S_Guarantissued.RefOrganizationsPlan3], d_Organizations_Plan.Name);
            }

            // Бенефициатор
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOrganization3INNKPP)
            {
                var orgBook = GetEntity(d_Organizations_Plan.internalKey);
                var regOrg = sourceRow[f_S_Guarantissued.RefOrganizationsPlan3];
                destRow[colIndex] = String.Format("{0} ИНН {1} КПП {2}",
                    GetBookValue(orgBook, regOrg, d_Organizations_Plan.Name),
                    GetBookValue(orgBook, regOrg, d_Organizations_Plan.Code),
                    GetBookValue(orgBook, regOrg, d_Organizations_Plan.INN20));
            }

            // Бенефициатор
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOrganizationINNKPP)
            {
                var orgBook = GetEntity(d_Organizations_Plan.internalKey);
                var regOrg = sourceRow[f_S_Guarantissued.RefOrganizations];
                destRow[colIndex] = String.Format("{0} ИНН {1} КПП {2}",
                    GetBookValue(orgBook, regOrg, d_Organizations_Plan.Name),
                    GetBookValue(orgBook, regOrg, d_Organizations_Plan.Code),
                    GetBookValue(orgBook, regOrg, d_Organizations_Plan.INN20));
            }

            // Гарант
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOrganization2)
            {
                var orgBook = GetEntity(d_Organizations_Plan.internalKey);
                destRow[colIndex] = GetBookValue(orgBook, sourceRow[f_S_Guarantissued.RefOrganizationsPlan2], d_Organizations_Plan.Name);
            }

            // Обеспечение
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCollateralType)
            {
                var collateralBook = GetEntity(GetCollateralKey());
                destRow[colIndex] = GetBookValue(collateralBook, id, t_S_CollateralCI.Name, true, true);
            }

            // Номер (для гарантий)
            if (calcColumnTypes[colIndex] == CalcColumnType.cctNumStartDate)
            {
                destRow[colIndex] = String.Format("{0}, {1}",
                    GetDateValue(sourceRow[f_S_Creditincome.StartDate]), sourceRow[f_S_Creditincome.Num]);
            }

            // Период договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctReportPeriod)
            {
                var minDate = GetDateValue(sourceRow[f_S_Creditincome.StartDate]);
                var maxDate = GetDateValue(sourceRow[f_S_Creditincome.EndDate]);

                var paramMinDate = reportParams[ReportConsts.ParamLoDate];
                var paramMaxDate = reportParams[ReportConsts.ParamHiDate];

                if (minDate.Length == 0 || Convert.ToDateTime(minDate) < Convert.ToDateTime(paramMinDate))
                {
                    minDate = paramMinDate;
                }

                if (maxDate.Length == 0 || Convert.ToDateTime(maxDate) < Convert.ToDateTime(paramMaxDate))
                {
                    maxDate = paramMaxDate;
                }

                destRow[colIndex] = String.Format("{0}-{1}", GetDateValue(minDate), GetDateValue(maxDate));
            }

            // Номер (для гарантий)
            if (calcColumnTypes[colIndex] == CalcColumnType.cctNumStartDate2)
            {
                destRow[colIndex] = String.Format("№ {1} от {0}",
                    GetDateValue(sourceRow[f_S_Creditincome.StartDate]), sourceRow[f_S_Creditincome.Num]);
            }

            // Зависимая колонка
            if (calcColumnTypes[colIndex] == CalcColumnType.cctRelation)
            {
                var columnIndexList = columnParamList[colIndex][ParamValue1].Split(';');
                decimal sum = 0;

                foreach (var columnIndex in columnIndexList)
                {
                    var tempIndex = Convert.ToInt32(columnIndex.Remove(0, 1));
                    if (destRow[tempIndex] == DBNull.Value) continue;
                    var tempVal = GetNumber(destRow[tempIndex]);
                    
                    switch (columnIndex[0])
                    {
                        case '%':
                            sum /= tempVal;
                            break;
                        case '*':
                            sum *= tempVal;
                            break;
                        default:
                            {
                                int multiplier = 1;
                                if (columnIndex[0] == '-') multiplier = -1;
                                sum += multiplier * tempVal;
                            }
                            break;
                    }
                }

                destRow[colIndex] = sum;
            }

            // Регресс(для гарантий)
            if (calcColumnTypes[colIndex] == CalcColumnType.cctRegress)
            {
                var regress = Convert.ToInt32(sourceRow[f_S_Guarantissued.Regress]);
                destRow[colIndex] = "нет";
                if (regress == 1) destRow[colIndex] = "да";
            }

            // Регресс(для гарантий)
            if (calcColumnTypes[colIndex] == CalcColumnType.cctRegress2)
            {
                var regress = Convert.ToInt32(sourceRow[f_S_Guarantissued.Regress]);
                destRow[colIndex] = "Нет";
                if (regress == 1) destRow[colIndex] = "Есть";
            }

            // Номер + дата заключения + организация
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCreditNumContractDateOrg)
            {
                var orgBook = GetEntity(d_Organizations_Plan.internalKey);
                destRow[colIndex] = String.Format("№ {0} от {1} {2}",
                    sourceRow[f_S_Creditincome.Num],
                    GetDateValue(sourceRow[f_S_Creditincome.ContractDate]),
                    GetBookValue(orgBook, sourceRow[f_S_Creditincome.RefOrganizations], d_Organizations_Plan.Name));
            }

            // Номер + дата заключения + организация
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCreditNumDatePercent)
            {
                var percentValue = GetNumber(GetNearestPercent(sourceRow, reportParams[ReportConsts.ParamHiDate]));
                destRow[colIndex] = String.Format("№ {0} от {1} ({2}%)",
                    sourceRow[f_S_Creditincome.Num],
                    GetDateValue(sourceRow[f_S_Creditincome.ContractDate]),
                    percentValue);
            }

            // Номер + дата заключения + организация
            if (calcColumnTypes[colIndex] == CalcColumnType.cctGarantOrgTypeNumDate)
            {
                var orgBook = GetEntity(d_Organizations_Plan.internalKey);
                var typBook = GetEntity(d_S_TypeContract.internalKey);

                destRow[colIndex] = String.Format("({0}) {1} №{2} от {3}",
                    GetBookValue(orgBook, sourceRow[f_S_Guarantissued.RefOrganizations], d_Organizations_Plan.Name),
                    GetBookValue(typBook, sourceRow[f_S_Guarantissued.RefTypeContract], d_S_TypeContract.Name),
                    sourceRow[f_S_Guarantissued.Num],
                    GetDateValue(sourceRow[f_S_Guarantissued.StartDate]));
            }

            // Номер + процент
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCreditNumPercent)
            {
                var percentText = GetNumber(GetNearestPercent(sourceRow, reportParams[ReportConsts.ParamHiDate]));
                destRow[colIndex] = String.Format("№{0} ({1}% год.)",
                    sourceRow[f_S_Creditincome.Num], percentText);
            }

            // Номер + дата контракта
            if (calcColumnTypes[colIndex] == CalcColumnType.cctNumContractDate)
            {
                destRow[colIndex] = String.Format("{0} № {1}",
                    GetDateValue(sourceRow[f_S_Creditincome.ContractDate]), sourceRow[f_S_Creditincome.Num]);
            }

            // Номер + дата контракта
            if (calcColumnTypes[colIndex] == CalcColumnType.cctContractDateNum)
            {
                destRow[colIndex] = String.Format("№ {0} от {1}",
                    sourceRow[f_S_Creditincome.Num], GetDateValue(sourceRow[f_S_Creditincome.ContractDate]));
            }

            // Разыменовка типа договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctContractDesc)
            {
                destRow[colIndex] = GetContractDesc();
            }
            // Организация
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOrganization)
            {
                var orgBook = GetEntity(d_Organizations_Plan.internalKey);
                destRow[colIndex] = GetBookValue(orgBook, sourceRow[f_S_Creditincome.RefOrganizations], d_Organizations_Plan.Name);
            }
            // Родительская организация
            if (calcColumnTypes[colIndex] == CalcColumnType.cctGenOrg)
            {
                var orgBook = GetEntity(d_Organizations_Plan.internalKey);
                destRow[colIndex] = GetBookValue(orgBook, sourceRow[f_S_Creditincome.RefOrganizations], d_Organizations_Plan.GenOrgName);
            }
            // Дата окончания договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCreditEndDate)
            {
                var endDateName = GetCreditEndFieldName(sourceRow);

                if (endDateName.Length > 0)
                {
                    destRow[colIndex] = GetDateValue(sourceRow[endDateName]);
                    creditEndDate = GetDateValue(destRow[colIndex]);
                }
                else
                {
                    creditEndDate = DateTime.MinValue.ToShortDateString();
                }

                destRow[TempFieldNames.SortCreditEndDate] = creditEndDate;
            }
            // Название валюты
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOKVName)
            {
                var okvBook = GetEntity(d_OKV_Currency.internalKey);
                destRow[colIndex] = GetBookValue(okvBook, sourceRow[f_S_Creditincome.RefOKV], d_OKV_Currency.CodeLetter);
            }

            // Полное название валюты
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOKVFullName)
            {
                var okvBook = GetEntity(d_OKV_Currency.internalKey);
                destRow[colIndex] = GetBookValue(okvBook, sourceRow[f_S_Creditincome.RefOKV], d_OKV_Currency.Name);
            }

            // Курс валюты
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOKVValue)
            {
                var okvType = Convert.ToInt32(sourceRow[f_S_Creditincome.RefOKV]);
                if (okvType != -1)
                {
                    destRow[colIndex] = RefreshExchangeList(okvType);
                    if (columnParamList[colIndex].ContainsKey(ParamActualDate))
                    {
                        destRow[colIndex] = GetExchangeValue(okvType, columnParamList[colIndex][ParamActualDate]);
                    }
                }
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctContractNum2)
            {
                var orgBook = GetEntity(d_Organizations_Plan.internalKey);
                // Составная информация о договоре(дата, тип)
                var contractDate = String.Empty;
                var orgName = String.Empty;
                if (sourceRow[f_S_Creditincome.ContractDate] != DBNull.Value)
                    contractDate = GetDateValue(sourceRow[f_S_Creditincome.ContractDate]);
                if (sourceRow[f_S_Creditincome.RefOrganizations] != DBNull.Value)
                    orgName = GetBookValue(orgBook, sourceRow[f_S_Creditincome.RefOrganizations], d_Organizations_Plan.Name);

                var creditType = GetEntity(d_S_TypeContract.internalKey);
                var ctName = GetBookValue(creditType, sourceRow[f_S_Creditincome.RefTypeContract], d_S_TypeContract.Name);

                destRow[colIndex] = String.Format("{0} от {1} № {2} с {3}({4:N0} руб.)",
                    ctName, contractDate, sourceRow[f_S_Creditincome.Num], orgName, sourceRow[f_S_Creditincome.Sum]);
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCreditTypeNumDate)
            {
                var creditType = GetEntity(d_S_TypeContract.internalKey);
                destRow[colIndex] = String.Format("{0} № {1} от {2}",
                    GetBookValue(creditType, sourceRow[f_S_Creditincome.RefTypeContract], d_S_TypeContract.Name),
                    sourceRow[f_S_Creditincome.Num],
                    GetDateValue(sourceRow[f_S_Creditincome.ContractDate]));
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCreditTypeNumStartDate)
            {
                var creditType = GetEntity(d_S_TypeContract.internalKey);
                destRow[colIndex] = String.Format("{0}/ {1}/ {2}",
                    GetBookValue(creditType, sourceRow[f_S_Creditincome.RefTypeContract], d_S_TypeContract.Name),
                    sourceRow[f_S_Creditincome.Num],
                    GetDateValue(sourceRow[f_S_Creditincome.StartDate]));
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctGarantTypeNumDate)
            {
                var creditType = GetEntity(d_S_TypeContract.internalKey);
                destRow[colIndex] = String.Format("{0} № {1} от {2}",
                    GetBookValue(creditType, sourceRow[f_S_Guarantissued.RefTypeContract], d_S_TypeContract.Name),
                    sourceRow[f_S_Guarantissued.Num],
                    GetDateValue(sourceRow[f_S_Guarantissued.DateDoc]));
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctGarantTypeNumStartDate)
            {
                var creditType = GetEntity(d_S_TypeContract.internalKey);
                destRow[colIndex] = String.Format("{0} № {1} от {2}",
                    GetBookValue(creditType, sourceRow[f_S_Guarantissued.RefTypeContract], d_S_TypeContract.Name),
                    sourceRow[f_S_Guarantissued.Num],
                    GetDateValue(sourceRow[f_S_Guarantissued.StartDate]));
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCreditIssNumDocDate)
            {
                destRow[colIndex] = String.Format("{0} от {1}",
                    sourceRow[f_S_Creditissued.Num], GetDateValue(sourceRow[f_S_Creditissued.DocDate]));
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctContractNum3)
            {
                var creditType = GetEntity(d_S_TypeContract.internalKey);
                var ctName = GetBookValue(creditType, sourceRow[f_S_Creditincome.RefTypeContract], d_S_TypeContract.Name);
                var orgBook = GetEntity(d_Organizations_Plan.internalKey);
                // Составная информация о договоре(дата, тип)
                var orgName = String.Empty;
                if (sourceRow[f_S_Creditincome.RefOrganizations] != DBNull.Value)
                    orgName = GetBookValue(orgBook, sourceRow[f_S_Creditincome.RefOrganizations], d_Organizations_Plan.Name);

                destRow[colIndex] = String.Format("{0} от {1} с {2}({3:N0} руб.)",
                    ctName, GetDateValue(sourceRow[f_S_Creditincome.StartDate]), orgName,
                    Convert.ToDecimal(sourceRow[f_S_Creditincome.Sum]));
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctContractNum4)
            {
                destRow[colIndex] = String.Format("{0} от {1} на {2}",
                    sourceRow[f_S_Creditissued.ActNum],
                    GetDateValue(sourceRow[f_S_Creditissued.ActDate]),
                    sourceRow[f_S_Creditissued.Purpose]);
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctNumDateOKV)
            {
                var okvBook = GetEntity(d_OKV_Currency.internalKey);
                destRow[colIndex] = String.Format("{0}; {1}; {2}",
                    sourceRow[f_S_Creditincome.Num],
                    GetDateValue(sourceRow[f_S_Creditincome.ContractDate]),
                    GetBookValue(okvBook, sourceRow[f_S_Creditincome.RefOKV], d_OKV_Currency.CodeLetter));
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapLabelPurpose)
            {
                destRow[colIndex] = String.Format(
                    "Министерство управления финансами Самарской области Цель займа - {0}",
                    sourceRow[f_S_Capital.Purpose]);
            }

            if (calcColumnTypes[colIndex] == CalcColumnType.cctFlagColumn)
            {
                var columnName = columnParamList[colIndex][ParamValue1];
                destRow[colIndex] = GetBoolValue(sourceRow[columnName]) ? "Да" : "Нет";
            }

            // Цель + Организация
            if (calcColumnTypes[colIndex] == CalcColumnType.cctOrgPurpose)
            {
                var orgName = String.Empty;
                var orgBook = GetEntity(d_Organizations_Plan.internalKey);

                if (sourceRow[f_S_Creditincome.RefOrganizations] != DBNull.Value)
                    orgName = GetBookValue(orgBook, sourceRow[f_S_Creditincome.RefOrganizations], d_Organizations_Plan.Name);
                destRow[colIndex] = String.Format("{0}. {1}", orgName, sourceRow[f_S_Creditincome.Purpose]);
            }

            // Субзаймы минфина Самара
            if (calcColumnTypes[colIndex] == CalcColumnType.cctSubCreditCaption)
            {
                var creditType = GetEntity(d_S_TypeContract.internalKey);
                destRow[colIndex] = 
                    String.Format("{0} ({1} от {2}) между Минфином РФ, Администрацией Самарской области - {3}",
                    GetBookValue(creditType, sourceRow[f_S_Creditincome.RefTypeContract], d_S_TypeContract.Name),
                    sourceRow[f_S_Creditincome.Num],
                    GetDateValue(sourceRow[f_S_Creditincome.ContractDate]),
                    sourceRow[f_S_Creditincome.Purpose]);
            }

            // Год старта договора
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCreditYear)
            {
                if (sourceRow[f_S_Creditincome.StartDate] != DBNull.Value)
                {
                    destRow[colIndex] = Convert.ToDateTime(sourceRow[f_S_Creditincome.StartDate]).Year;
                }
                else
                {
                    destRow[colIndex] = 0;
                }
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPurposeActNumDate)
            {
                destRow[colIndex] = String.Format("{0}, {1}, {2}",
                    sourceRow[f_S_Creditissued.Purpose],
                    sourceRow[f_S_Creditissued.ActNum],
                    GetDateValue(sourceRow[f_S_Creditissued.ActDate]));
            }

            // Составные данные о договоре
            if (calcColumnTypes[colIndex] == CalcColumnType.cctContractNum)
            {
                var orgBook = GetEntity(d_Organizations_Plan.internalKey);
                // Составная информация о договоре(дата, тип)
                var part1 = String.Empty;
                var part2 = String.Empty;
                var part31 = String.Empty;
                var part32 = String.Empty;
                var part4 = String.Empty;
                if (sourceRow[f_S_Creditincome.RefOrganizations] != DBNull.Value)
                    part1 = GetBookValue(orgBook, sourceRow[f_S_Creditincome.RefOrganizations], d_Organizations_Plan.Name);
                if (sourceRow[f_S_Creditincome.Num] != DBNull.Value)
                    part2 = String.Format("; {0}", sourceRow[f_S_Creditincome.Num]);
                if (sourceRow[f_S_Creditincome.ContractDate] != DBNull.Value)
                    part31 = String.Format("; {0}", GetDateValue(sourceRow[f_S_Creditincome.ContractDate]));
                var endFieldName = GetCreditEndFieldName(sourceRow);
                if (endFieldName.Length > 0 && sourceRow[endFieldName] != DBNull.Value)
                    part32 = String.Format("-{0}", GetDateValue(sourceRow[endFieldName]));
                var okvBook = GetEntity(d_OKV_Currency.internalKey);
                if (sourceRow[f_S_Creditincome.RefOKV] != DBNull.Value && Convert.ToInt32(sourceRow[f_S_Creditincome.RefOKV]) != -1)
                    part4 = String.Format("; {0}", GetBookValue(okvBook, sourceRow[f_S_Creditincome.RefOKV], d_OKV_Currency.CodeLetter));

                destRow[colIndex] = String.Format("{0}{1}{2}{3}{4}", part1, part2, part31, part32, part4);
            }

            // Считарь значений по деталям
            if (calcColumnTypes[colIndex] == CalcColumnType.cctDetail)
            {
                var formula = columnParamList[colIndex][ParamFormula];
                currentFieldList = String.Empty;
                currentIgnoreExchenge = false;
                
                if (columnParamList[colIndex].ContainsKey(ParamFieldList))
                {
                    currentFieldList = columnParamList[colIndex][ParamFieldList];
                }
                
                if (columnParamList[colIndex].ContainsKey(ParamIgnoreExchange))
                {
                    currentIgnoreExchenge = Convert.ToBoolean(columnParamList[colIndex][ParamIgnoreExchange]);
                }

                destRow[colIndex] = ParseFormula(id, ref formula);
                currentFieldList = String.Empty;
                currentIgnoreExchenge = false;
            }

            // Считарь значений по деталям
            if (calcColumnTypes[colIndex] == CalcColumnType.cctDetailText)
            {
                writeFullDetailText = true;
                fullDetailText = String.Empty;
                
                if (columnParamList[colIndex].ContainsKey(ParamFieldList))
                {
                    currentFieldList = columnParamList[colIndex][ParamFieldList];
                }

                var formula = columnParamList[colIndex][ParamFormula];
                ParseFormula(id, ref formula);
                destRow[colIndex] = fullDetailText;
                writeFullDetailText = false;
                currentFieldList = String.Empty;
            }

            // Купонный доход
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapCoupon)
            {
                var textParams = new JournalTextParams(sourceRow, t_S_CPJournalPercent.Coupon)
                                     {
                                         onlyValues = true,
                                         usePercentSym = false
                                     };

                destRow[colIndex] = GetPercentText(textParams);
            }

            // Ближайший процент к дате отчета
            if (calcColumnTypes[colIndex] == CalcColumnType.cctNearestPercent)
            {
                destRow[colIndex] = GetNearestPercent(sourceRow, reportParams[ReportConsts.ParamHiDate]);
            }

            // Текстовка журнала процентов по договору
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPercentTextSplitter)
            {
                var textParams = new JournalTextParams(sourceRow, t_S_JournalPercentCI.CreditPercent)
                                     {
                                         splitter = " - "
                                     };

                destRow[colIndex] = GetPercentText(textParams);
            }

            // Текстовка журнала процентов по договору
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPercentGroupText)
            {
                var contractStartDate = DateTime.MinValue;

                if (sourceRow.Table.Columns.Contains(f_S_Creditincome.StartDate) &&
                    sourceRow[f_S_Creditincome.StartDate] != DBNull.Value)
                {
                    contractStartDate = Convert.ToDateTime(sourceRow[f_S_Creditincome.StartDate]);
                }

                var textParams = new JournalTextParams(sourceRow, t_S_JournalPercentCI.CreditPercent)
                                     {
                                         useJoining = true,
                                         startDate = contractStartDate
                                     };

                destRow[colIndex] = GetPercentText(textParams);
            }

            // Текстовка журнала процентов по договору
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPercentText)
            {
                var textParams = new JournalTextParams(sourceRow, t_S_JournalPercentCI.CreditPercent);
                destRow[colIndex] = GetPercentText(textParams);
            }

            // Текстовка журнала процентов по договору
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPercentValues)
            {
                var textParams = new JournalTextParams(sourceRow, t_S_JournalPercentCI.CreditPercent) 
                {
                    onlyValues = true
                };

                destRow[colIndex] = GetPercentText(textParams);
            }

            // Текстовка журнала процентов по договору
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPercentValues2)
            {
                var textParams = new JournalTextParams(sourceRow, t_S_JournalPercentCI.CreditPercent)
                                     {
                                         onlyValues = true,
                                         usePercentSym = false
                                     };

                destRow[colIndex] = GetPercentText(textParams);
            }

            // Текстовка журнала процентов по договору - Максимальное и минимальное
            if (calcColumnTypes[colIndex] == CalcColumnType.cctPercentTextMaxMin)
            {
                var textParams = new JournalTextParams(sourceRow, t_S_JournalPercentCI.CreditPercent)
                                     {
                                         isMaxMin = true
                                     };

                destRow[colIndex] = GetPercentText(textParams);
            }

            // Заголовок для ценных бумаг
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCapOffNumNPANameDateNum)
            {
                destRow[colIndex] = String.Format("Облигационный займ Самарской области {0}, {1} от {2}, {3}",
                    sourceRow[f_S_Capital.OfficialNumber],
                    sourceRow[f_S_Capital.NameNPA],
                    GetDateValue(sourceRow[f_S_Capital.DateNPA]),
                    sourceRow[f_S_Capital.NumberNPA]);
            }

            // Остаток на дату
            if (calcColumnTypes[colIndex] == CalcColumnType.cctCurrentRest)
            {
                var sumFieldName = ReportConsts.SumField;
                decimal currencyMult = 1;
                var masterKey = id;

                if (currencyType != -1 && !ignoreCurrencyCalc)
                {
                    sumFieldName = ReportConsts.CurrencySumField;
                    currencyMult = okvValues[currencyType];
                }

                decimal contractSum = 0;
                
                if (sourceRow[sumFieldName] != DBNull.Value)
                {
                    contractSum = currencyMult * Convert.ToDecimal(sourceRow[sumFieldName]);
                }

                var contractType = Convert.ToInt32(sourceRow[f_S_Creditincome.RefSExtension]);

                switch (contractType)
                {
                    case 6:
                    case 5:
                    case 3:
                        destRow[colIndex] = contractSum - currencyMult * GetSumValue(
                            dtDetail[0],
                            masterKey,
                            t_S_FactAttractCI.FactDate,
                            sumFieldName,
                            DateTime.MinValue,
                            Convert.ToDateTime(reportParams[ReportConsts.ParamHiDate]), 
                            true,  true);
                        break;
                    case 4:
                        destRow[colIndex] = contractSum -
                                            currencyMult * GetSumValue(
                                                dtDetail[0],
                                                masterKey,
                                                t_S_FactAttractCI.FactDate,
                                                sumFieldName,
                                                DateTime.MinValue,
                                                Convert.ToDateTime(reportParams[ReportConsts.ParamHiDate]),
                                                true, true)
                                            +
                                            currencyMult * GetSumValue(
                                                dtDetail[1],
                                                masterKey,
                                                t_S_FactDebtCI.FactDate,
                                                sumFieldName,
                                                DateTime.MinValue,
                                                Convert.ToDateTime(reportParams[ReportConsts.ParamHiDate]),
                                                true, true);
                        break;
                    default:
                        destRow[colIndex] = GetCurrentRest(masterKey, currencyType);
                        break;
                }
            }

            SetCalcFieldValues(destRow, sourceRow, colIndex);
        }

        protected virtual void SetCalcFieldValues(DataRow destRow, DataRow sourceRow, int colIndex)
        {
        }

        private static decimal GetNumber(object obj)
        {
            decimal result;

            if (!decimal.TryParse(Convert.ToString(obj), out result))
            {
                result = 0;
            }

            return result;
        }

        // Значение курса на дату
        public decimal GetExchangeValue(int okvType, string calcDate)
        {
            decimal result = 0;
            var dt = exchangeRate[okvType];
            var addSign = String.Empty;
            
            if (!exchangePrevDay)
            {
                addSign = "=";
            }

            var drExch = dt.Select(
                String.Format("{2} <{1}'{0}'", calcDate, addSign, d_S_ExchangeRate.DateFixing),
                String.Format("{0} desc", d_S_ExchangeRate.DateFixing));

            if (drExch.Length > 0)
            {
                result = Convert.ToDecimal(drExch[0][d_S_ExchangeRate.ExchangeRate]);
            }

            return result;
        }

        // Заполняем кэши валют и таблицу используемых валют dtExchange для okvType
        private string RefreshExchangeList(int okvType)
        {
            var result = String.Empty;

            if (okvType != -1)
            {
                if (fixedExchangeRate.ContainsKey(okvType))
                {
                    if (!okvCodes.Contains(okvType))
                    {
                        okvCodes.Add(okvType);
                        okvValues.Add(okvType, fixedExchangeRate[okvType]);
                    }

                    return Convert.ToString(okvValues[okvType]);
                }

                DataTable dt;
                if (!exchangeRate.ContainsKey(okvType))
                {
                    var filter = String.Format("{1} = {0}", okvType, f_S_Creditincome.RefOKV);
                    var dbHelper = new ReportDBHelper(scheme);
                    dt = dbHelper.GetEntityData(d_S_ExchangeRate.internalKey, filter);
                    dt = SortDataSet(dt, String.Format("{0} desc", d_S_ExchangeRate.DateFixing), false);
                    exchangeRate.Add(okvType, dt);
                }
                else
                {
                    dt = exchangeRate[okvType];
                }

                var addSign = String.Empty;
                if (!exchangePrevDay) addSign = "=";

                var drExch = dt.Select(
                    String.Format("{2} <{1}'{0}'",
                        reportParams[ReportConsts.ParamHiDate], addSign, d_S_ExchangeRate.DateFixing),
                    String.Format("{0} desc", d_S_ExchangeRate.DateFixing));

                if (drExch.Length > 0)
                {
                    result = String.Format("{0}({1})",
                        drExch[0][d_S_ExchangeRate.ExchangeRate],
                        GetDateValue(drExch[0][d_S_ExchangeRate.DateFixing]));

                    if (okvCodes.IndexOf(okvType) == -1)
                    {
                        var drExchange = dtExchange.Rows.Add();
                        var okvBook = GetEntity(d_OKV_Currency.internalKey);
                        drExchange[0] = String.Format("{0} {1} ({2})",
                            GetBookValue(okvBook, okvType, "CodeLetter"),
                            drExch[0][d_S_ExchangeRate.ExchangeRate],
                            GetDateValue(drExch[0][d_S_ExchangeRate.DateFixing])
                            );

                        okvCodes.Add(okvType);
                        okvValues.Add(okvType, Convert.ToDecimal(drExch[0][d_S_ExchangeRate.ExchangeRate]));
                    }
                }
                else
                {
                    var fictiveRate = dt.NewRow();
                    fictiveRate[d_S_ExchangeRate.DateFixing] = DateTime.MinValue.ToShortDateString();
                    fictiveRate[d_S_ExchangeRate.ExchangeRate] = 0;
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
            var endDateName = String.Empty;
            if (sourceRow[f_S_Creditincome.EndDate] != DBNull.Value) endDateName = f_S_Creditincome.EndDate;
            if (sourceRow[f_S_Creditincome.RenewalDate] != DBNull.Value) endDateName = f_S_Creditincome.RenewalDate;
            return endDateName;
        }

        protected virtual bool UseStrongFilters()
        {
            return false;
        }

        protected virtual Dictionary<string, string> InnerJoins()
        {
            return new Dictionary<string, string>();
        }

        protected virtual Dictionary<string, string> JoinRelations()
        {
            return new Dictionary<string, string>();
        }

        protected virtual Dictionary<string, string> OuterJoins()
        {
            return new Dictionary<string, string>();
        }

        private static string GetFieldNames(IEntity entity, string prefix, string pseudoPrefix)
        {
            var fieldStr = entity.Attributes.Values.Aggregate(String.Empty, (current, attr) => 
                String.Format("{0},{1}.{2} as {3}{2}", current, prefix, attr.Name, pseudoPrefix));

            return fieldStr.TrimStart(',');
        }

        private string GetTablesList(string mainTableKey, Dictionary<string, string> innerTables)
        {
            var result = String.Format("{0} {1}", GetEntity(mainTableKey).FullDBName, CreditSQLObject.MTPrefix);
            var joinCounter = 1;

            foreach (var innerTableKey in innerTables.Keys)
            {
                var tablePrefix = GetTablePrefix(joinCounter++);
                result = String.Format("{0},{1} {2}", result, GetEntity(innerTableKey).FullDBName, tablePrefix);
            }

            return result;
        }

        private static string GetLinksList(
            Dictionary<string, string> innerTables, 
            Dictionary<string, string> relations)
        {
            var result = String.Empty;
            var joinCounter = 1;
            var arrTables = innerTables.ToArray();

            foreach (var pair in innerTables)
            {
                var fieldPrefix = GetTablePrefix(joinCounter++);
                var refPrefix = CreditSQLObject.MTPrefix;

                if (relations.ContainsKey(pair.Key))
                {
                    var index = 0;
                    var position = -1;

                    foreach (var item in arrTables)
                    {
                        if (item.Key == pair.Key)
                        {
                            position = index;
                        }
                        
                        index++;
                    }

                    refPrefix = GetTablePrefix(position);
                }

                result = String.Format(" {0} and {1}.{2} = {3}.id", result, refPrefix, pair.Value, fieldPrefix);
            }

            return result;
        }

        private string GetFullFieldList(string mainTableKey, Dictionary<string, string> innerTables)
        {
            var result = GetFieldNames(GetEntity(mainTableKey), CreditSQLObject.MTPrefix, String.Empty);
            var joinCounter = 1;

            foreach (var innerTableKey in innerTables.Keys)
            {
                string fieldPrefix = GetTablePrefix(joinCounter++);
                result = String.Format("{0},{1}", result, GetFieldNames(GetEntity(innerTableKey), fieldPrefix, fieldPrefix));
            }

            return result;
        }

        private static string GetTablePrefix(int tableIndex)
        {
            return String.Format("ij{0}", tableIndex);
        }

        public static string GetJoinFieldName(int tableIndex, string fieldName)
        {
            return String.Format("{0}{1}", GetTablePrefix(tableIndex), fieldName);            
        }

        public static string GetJoinFilterName(int tableIndex, string fieldName)
        {
            return String.Format("{0}.{1}", GetTablePrefix(tableIndex), fieldName);
        }

        public DataTable FillMainTableData()
        {
            var dbHelper = new ReportDBHelper(scheme);
            DataTable dtMaster;
            var tableKey = GetMainTableKey();
            var tableFilter = GetMainSQLQuery();
            
            if (UseStrongFilters())
            {
                var innerTables = InnerJoins();

                if (innerTables.Count == 0)
                {
                    dtMaster = dbHelper.GetEntityData(tableKey, tableFilter);
                }
                else
                {
                    var fieldList = GetFullFieldList(tableKey, innerTables);

                    if (clearQueryFields)
                    {
                        fieldList = String.Empty;

                        for (var i = 0; i < columnCount; i++)
                        {
                            if (columnList[i] == FieldType.ftMain)
                            {
                                fieldList = String.Join(",", new[] { fieldList, 
                                    String.Format("{0}.{1}", CreditSQLObject.MTPrefix, columnParamList[i][ParamName]) });
                            }
                        }

                        fieldList = fieldList.Trim(',');
                    }

                    if (fixedFilter.Length > 0)
                    {
                        tableFilter = String.Format("{0} and {1} ", tableFilter, fixedFilter);
                    }

                    var tableList = GetTablesList(tableKey, innerTables);
                    var linksList = GetLinksList(innerTables, JoinRelations());
                    var selectStr = String.Format("select {0} from {1} where {2} {3}",
                                                  fieldList, tableList, tableFilter, linksList);
                    dtMaster = dbHelper.GetTableData(selectStr);
                }
            }
            else
            {
                dtMaster = dbHelper.GetEntityData(tableKey);
                dtMaster = DataTableUtils.FilterDataSet(dtMaster, tableFilter);
            }
            
            return dtMaster;
        }

        // Заполняем данные
        public DataTable FillData()
        {
            // Даты расчета курса валюты
            if (!reportParams.ContainsKey(ReportConsts.ParamHiDate))
                reportParams.Add(ReportConsts.ParamHiDate, DateTime.Now.ToShortDateString());
            // Определяем используемые детали
            FillUsedDetailList();
            var dt = FillMainTableData();
            // Хитрованское извращение с запросом деталей(выбираем только т что нужны)
            dtDetail = new DataTable[GetDetailKeys().Length];
            safespotDetail.Clear();
            var dbHelper = new ReportDBHelper(scheme);

            for (var j = 0; j < GetDetailKeys().Length; j++)
            {
                if (usedDetails.Contains(j))
                {
                    dtDetail[j] = null;
                    var ass = scheme.RootPackage.FindAssociationByName(GetDetailKeys()[j]);
                    var detailEntity = ass.RoleData;
                    dtDetail[j] = dbHelper.GetEntityData(detailEntity, String.Empty);
                }
            }

            // Записи журнала вычитываем из БД, только если есть колонка с текстовкой процентов
            if (calcColumnTypes.ContainsValue(CalcColumnType.cctPercentText) ||
                calcColumnTypes.ContainsValue(CalcColumnType.cctPercentTextMaxMin) ||
                calcColumnTypes.ContainsValue(CalcColumnType.cctPercentValues) ||
                calcColumnTypes.ContainsValue(CalcColumnType.cctPercentValues2) ||
                calcColumnTypes.ContainsValue(CalcColumnType.cctNearestPercent) ||
                calcColumnTypes.ContainsValue(CalcColumnType.cctPercentTextSplitter) ||
                calcColumnTypes.ContainsValue(CalcColumnType.cctCreditNumPercent) ||
                calcColumnTypes.ContainsValue(CalcColumnType.cctCreditNumDatePercent) ||
                calcColumnTypes.ContainsValue(CalcColumnType.cctPercentGroupText))
            {
                dtJournalPercent = dbHelper.GetEntityData(GetJournalKey());
            }

            // сохраняем детальки до всяческих фильтраций над ними
            MakeSafeSpot();
            // Создаем поля в датасете результата
            CreateFields();
            for (var j = 0; j < dt.Rows.Count; j++)
            {
                var sourceRow = dt.Rows[j];
                FilterDetailTables(sourceRow[f_S_Creditincome.id]);
                currencyType = -1;

                if (dt.Columns.Contains(f_S_Creditincome.RefOKV))
                {
                    currencyType = Convert.ToInt32(sourceRow[f_S_Creditincome.RefOKV]);
                }
                
                if (currencyType != -1)
                {
                    RefreshExchangeList(currencyType);
                }

                var resultRow = dtResult.Rows.Add();
                for (var i = 0; i < columnCount; i++)
                {
                    currentColumnIndex = i;
                    // заполняем значение поля
                    if (columnList[i] == FieldType.ftCalc)
                    {
                        if (columnParamList[i].ContainsKey(ParamPlanDate))
                        {
                            FilterDetailTables(sourceRow[f_S_Creditincome.id], columnParamList[i][ParamPlanDate]);
                        }

                        FillCalcField(resultRow, sourceRow, i);
                    }
                    else
                    {
                        FillDataField(resultRow, sourceRow, i);
                    }
                    // Прооверим не нужно ли подчистить значение в зависимости от условия заполнения колонки
                    CheckColumnCondition(i, sourceRow, resultRow);
                }
                // служебные поля

                if (dt.Columns.Contains(f_S_Creditincome.RefOKV))
                {
                    resultRow[f_S_Creditincome.RefOKV] = sourceRow[f_S_Creditincome.RefOKV];
                }

                if (dt.Columns.Contains(f_S_Creditincome.ParentID))
                {
                    resultRow[f_S_Creditincome.ParentID] = sourceRow[f_S_Creditincome.ParentID];
                }

                if (dt.Columns.Contains(f_S_Creditincome.id))
                {
                    resultRow[f_S_Creditincome.id] = sourceRow[f_S_Creditincome.id];
                }
            }
            
            // Повторный проход по формульным полям, потому что составляющие могут идти после них
            for (var j = 0; j < dtResult.Rows.Count; j++)
            {
                for (var i = 0; i < columnCount; i++)
                {
                    currentColumnIndex = i;
                    
                    if (columnList[i] == FieldType.ftCalc && calcColumnTypes[i] == CalcColumnType.cctRelation)
                    {
                        FillCalcField(dtResult.Rows[j], dtResult.Rows[j], i);
                    }
                }
            }

            // Сортируем
            if (sortString.Length > 0)
            {
                dtResult = SortDataSet(dtResult, sortString, hierarchicalSort);
            }

            // в нулевой колонке иногда счетчик
            if (calcColumnTypes.Keys.Count > 0 && calcColumnTypes[0] == CalcColumnType.cctPosition)
            {
                for (var j = 0; j < dtResult.Rows.Count; j++)
                {
                    dtResult.Rows[j][0] = j + 1;
                }
            }

            // Заполняем итоги
            if (useSummaryRow)
            {
                dtResult.Rows.Add();
                dtResult = RecalcSummary(dtResult);
            }

            if (removeServiceFields)
            {
                for (var i = 0; i < 4; i++)
                {
                    dtResult.Columns.RemoveAt(dtResult.Columns.Count - 1);
                }
            }

            currentColumnIndex = 0;

            // Выходим, радуемся
            return dtResult;
        }

        /// <summary>
        /// Расчет итоговой строчки
        /// </summary>
        public DataTable RecalcSummary(DataTable tblResult)
        {
            var drResult = tblResult.Rows[tblResult.Rows.Count - 1];
            
            foreach (var t in summaryColumnIndex)
            {
                drResult[t] = 0;
            }

            for (var j = 0; j < tblResult.Rows.Count - 1; j++)
            {
                foreach (var summaryIndex in summaryColumnIndex)
                {
                    var realIndex = summaryIndex;
                    
                    if (realSummaryIndex.ContainsKey(summaryIndex))
                    {
                        realIndex = realSummaryIndex[summaryIndex];
                    }

                    if (tblResult.Rows[j][realIndex] != DBNull.Value)
                    {                        
                        drResult[summaryIndex] =
                            GetNumber(drResult[summaryIndex]) + GetNumber(tblResult.Rows[j][realIndex]);
                    }
                }
            }

            return tblResult;
        }

        /// <summary>
        /// Ближайший процент
        /// </summary>
        private string GetNearestPercent(DataRow row, string endDate)            
        {
            var masterKey = Convert.ToInt32(row[f_S_Creditincome.id]);
            var drJournalPercent = dtJournalPercent.Select(
                String.Format("{1}={0} and {2} <= '{3}'", masterKey, GetParentRefName(), t_S_JournalPercentCI.ChargeDate, endDate),
                String.Format("{0} desc", t_S_JournalPercentCI.ChargeDate));
            
            // журнал непуст
            if (drJournalPercent.Length > 0)
            {
                return Convert.ToString(drJournalPercent[0][t_S_JournalPercentCI.CreditPercent]);
            }
            
            // если журнал пуст, то берем ставку из договора
            return row.Table.Columns.Contains(f_S_Creditincome.CreditPercent) ?
                Convert.ToString(row[f_S_Creditincome.CreditPercent]) : String.Empty;
        }

        private string JoiPercentPeriodText(string oldText, string startDate, string endDate, decimal pctValue)
        {
            var templateStr = endDate.Length == 0 ? "c {0} - {2:N2}%" : "c {0} по {1} - {2:N2}%";
            var pctPeriod = String.Format(templateStr, startDate, endDate, GetNumber(pctValue));
            return String.Join(", ", new[] { oldText, pctPeriod });            
        }

        /// <summary>
        /// Строка с процентами
        /// </summary>
        private string GetPercentText(JournalTextParams paramList)
        {
            decimal minValue = 0; 
            decimal maxValue = 0;
            var masterKey = Convert.ToInt32(paramList.row[f_S_Creditincome.id]);
            // выбираем из кэша только записи текущего договора(сортированные по дате)
            var drJournalPercent = dtJournalPercent.Select(
                String.Format("{1}={0}", masterKey, GetParentRefName()),
                String.Format("{0} asc", t_S_JournalPercentCI.ChargeDate));
            var percentSym = String.Empty;
            
            if (paramList.usePercentSym) percentSym = "%";
            
            if (drJournalPercent.Length > 0)
            {
                if (drJournalPercent.Length > 1)
                {
                    var percentList = new Collection<string>();

                    var textPercent = String.Empty;
                    // Это извращение чтобы отсечь дубли(которых в "теории" нет)
                    string dateText;
                    foreach (var rowPercent in drJournalPercent)
                    {
                        dateText = GetDateValue(rowPercent[t_S_JournalPercentCI.ChargeDate]);
                        var recordText = String.Format("{0} {1}",
                                                          dateText,
                                                          Convert.ToString(rowPercent[paramList.fieldName]));

                        if (!percentList.Contains(recordText))
                        {
                            percentList.Add(recordText);
                        }
                    }

                    var prevValue = String.Empty;
                    var prevKey = String.Empty;

                    var fstGroupVal = String.Empty;
                    var fstGroupKey = String.Empty;
                    var lstGroupKey = String.Empty;

                    if (paramList.startDate != DateTime.MinValue)
                    {
                        fstGroupKey = paramList.startDate.ToShortDateString();
                    }

                    var recordCounter = 1;
                    var uniqueCount = 0;

                    // это извращение чтобы вывести несколько на одну дату недубли :)
                    foreach (var recordStr in percentList)
                    {
                        var recordParts = recordStr.Split(' ');
                        var keyName = recordParts[0];
                        var keyValue = recordParts[1];

                        if (paramList.useJoining)
                        {
                            if (recordCounter == 1)
                            {
                                fstGroupVal = keyValue;
                            }

                            var isLastRecord = recordCounter == percentList.Count;
                            var isEqualValue = String.Compare(fstGroupVal, keyValue, true) == 0;

                            if (!isEqualValue || isLastRecord)
                            {
                                var pctFstValue = GetNumber(fstGroupVal);
                                var pctLstValue = GetNumber(keyValue);

                                textPercent = JoiPercentPeriodText(textPercent, fstGroupKey, lstGroupKey, pctFstValue);

                                if (isLastRecord && String.Compare(fstGroupVal, keyValue, true) != 0)
                                {
                                    textPercent = JoiPercentPeriodText(textPercent, lstGroupKey, keyName, pctLstValue);                                    
                                }

                                fstGroupKey = lstGroupKey;
                                fstGroupVal = keyValue;
                                uniqueCount++;
                            }

                            lstGroupKey = keyName;
                            recordCounter++;
                            continue;
                        }

                        if (Convert.ToDateTime(keyName) > paramList.startDate && Convert.ToDateTime(keyName) < paramList.endDate)
                        {
                            if (textPercent.Length == 0 && prevValue.Length > 0)
                            {
                                dateText = prevKey;
                                if (paramList.onlyValues) dateText = String.Empty;
                                textPercent = String.Format("{0}, {1}{4}{2:N2}{3}",
                                    textPercent, dateText, Convert.ToDecimal(prevValue), percentSym, paramList.splitter);
                            }
                            if (prevValue != keyValue)
                            {
                                dateText = keyName;
                                if (paramList.onlyValues) dateText = String.Empty;
                                textPercent = String.Format("{0}, {1}{4}{2:N2}{3}",
                                    textPercent, dateText, Convert.ToDecimal(keyValue), percentSym, paramList.splitter);
                            }

                            maxValue = Math.Max(maxValue, Convert.ToDecimal(keyValue));
                            minValue = Math.Min(minValue, Convert.ToDecimal(keyValue));
                            if (minValue == 0) minValue = maxValue;
                        }
                        
                        prevValue = keyValue;
                        prevKey = keyName;
                    }

                    if (uniqueCount == 1)
                    {
                        textPercent = String.Format("{0}{1}", fstGroupVal, percentSym);
                    }
                    
                    if (textPercent.Length == 0)
                    {
                        dateText = prevKey;
                        if (paramList.onlyValues) dateText = String.Empty;
                        textPercent = String.Format("{0}, {1}{4}{2:N2}{3}",
                            textPercent, dateText, Convert.ToDecimal(prevValue), percentSym, paramList.splitter);
                    }

                    if (paramList.isMaxMin && maxValue != minValue)
                    {
                        return String.Format("{0:N2}{2}-{1:N2}{2}", minValue, maxValue, percentSym);
                    }

                    return textPercent.TrimStart(',').TrimStart(' ');
                }
                // запись была одна...
                return String.Format("{0:N2}{1}", 
                    Convert.ToDecimal(drJournalPercent[0][paramList.fieldName]), percentSym);
            }
            
            // в детали журнала процентов вообще пусто
            if (paramList.row.Table.Columns.Contains(f_S_Creditincome.CreditPercent)
                && paramList.row[f_S_Creditincome.CreditPercent] != DBNull.Value
                && Convert.ToString(paramList.row[f_S_Creditincome.CreditPercent]).Length > 0)
            {
                return String.Format("{0:N2}{1}",
                    Convert.ToDecimal(paramList.row[f_S_Creditincome.CreditPercent]), percentSym);
            }
            
            return String.Empty;
        }

        /// <summary>
        /// вытягивает значение из справочника по ссылке
        /// </summary>
        protected string GetBookValue(IEntity book, object refValue, string resultField)
        {
            return GetBookValue(book, refValue, resultField, false, true);
        }

        /// <summary>
        /// вытягивает значение из справочника по ссылке
        /// </summary>
        protected string GetDirectBookValue(IEntity book, object refValue, string resultField)
        {
            return GetBookValue(book, refValue, resultField, false, false);
        }

        /// <summary>
        /// ключевое поле выборки из справочника
        /// </summary>
        private string GetBookKeyField(bool byRef)
        {
            var keyFieldName = "id";

            if (byRef)
            {

                keyFieldName = GetParentRefName();
            }

            return keyFieldName;
        }

        /// <summary>
        /// шаблон фильтра
        /// </summary>
        private static string GetBookFilter(string filter)
        {
            var selectFilter = "{0} = {1} {2}";

            if (filter == "ALL")
            {
                selectFilter = String.Empty;
            }

            return selectFilter;
        }

        /// <summary>
        /// вытягивает строки из справочника по ссылке
        /// </summary>
        protected DataRow[] GetTableRows(IEntity book, object refValue, bool byRef, string filter)
        {
            // Проверяем заполнен ли кэш данного справочника
            if (!bookCache.ContainsKey(book.FullDBName))
            {
                var dbHelper = new ReportDBHelper(scheme);
                var dtBook = dbHelper.GetEntityData(book, String.Empty);
                bookCache.Add(book.FullDBName, dtBook);
            }

            var selectStr = String.Format(GetBookFilter(filter), GetBookKeyField(byRef), refValue, filter);
            return bookCache[book.FullDBName].Select(selectStr);
        }

        /// <summary>
        /// вытягивает строки из справочника по ссылке без кэша
        /// </summary>
        protected DataRow[] GetDirectTableRows(IEntity book, object refValue, bool byRef, string filter)
        {
            var selectStr = String.Format(GetBookFilter(filter), GetBookKeyField(byRef), refValue, filter);
            var dbHelper = new ReportDBHelper(scheme);
            var dtBook = dbHelper.GetEntityData(book, selectStr);
            return dtBook.Select();
        }

        /// <summary>
        /// вытягивает значение из справочника по ссылке
        /// </summary>
        private string GetBookValue(IEntity book, object refValue, string resultField, bool byRef, bool useCache)
        {
            var result = String.Empty;

            if (refValue != DBNull.Value)
            {
                var drFind = useCache ? 
                    GetTableRows(book, refValue, byRef, String.Empty) : 
                    GetDirectTableRows(book, refValue, byRef, String.Empty);

                if (drFind.Length > 0)
                {
                    result = Convert.ToString(drFind[0][resultField]);
                }
            }

            return result;
        }

        private static bool GetBoolValue(object boolValue)
        {
            if (boolValue != DBNull.Value)
            {
                var valueStr = Convert.ToString(boolValue);
                if (valueStr == "1") valueStr = "True";
                if (valueStr == "0") valueStr = "False";
                return Convert.ToBoolean(valueStr);
            }

            return false;
        }

        private bool CheckColumnFilter(DataRow dr)
        {
            var result = true;

            var conditionFields = new Collection<string>
                                      {
                                          "RefTypSum", 
                                          "RefTypeSum", 
                                          "Offset", 
                                          "IsForgiven", 
                                          t_S_CPFactCapital.Secondary
                                      };

            foreach (var conditionField in conditionFields)
            {
                if (!columnParamList[currentColumnIndex].ContainsKey(conditionField)) continue;
                if (!dr.Table.Columns.Contains(conditionField)) continue;

                result = false;
                var values = columnParamList[currentColumnIndex][conditionField].Split(',');

                foreach (var valueStr in values)
                {
                    var conditionValue = Convert.ToString(dr[conditionField]);
                    result = result || conditionValue == valueStr;

                    if (conditionField == "IsForgiven" || conditionField == t_S_CPFactCapital.Secondary)
                    {
                        result = result || conditionValue == Convert.ToString(GetBoolValue(valueStr));
                    }
                }
            }

            return result;
        }

        private void ConcateDetailText(object dateValue, object cellValue)
        {
            if (dateValue == DBNull.Value) return;
            var currentDate = Convert.ToDateTime(dateValue);

            if (!columnParamList[currentColumnIndex].ContainsKey(ParamOnlyDates))
            {
                if (!columnParamList[currentColumnIndex].ContainsKey(ParamOnlyValues))
                {
                    fullDetailText = String.Format(
                        "{0}{1} {2}г. - {3:N2}{4}",
                        fullDetailText,
                        valuesSeparator,
                        currentDate.ToShortDateString(),
                        Convert.ToDecimal(cellValue),
                        textAppendix);
                }
                else
                {
                    fullDetailText = String.Format(
                        "{0}{1} {2:N2}{3}",
                        fullDetailText,
                        valuesSeparator,
                        Convert.ToDecimal(cellValue),
                        textAppendix);
                }
            }
            else
            {
                fullDetailText = String.Format(
                    "{0}{1} {2}{3}",
                    fullDetailText,
                    valuesSeparator,
                    currentDate.ToShortDateString(),
                    textAppendix);
            }
        }

        /// <summary>
        /// суммирование по детальной таблице
        /// </summary>
        public decimal GetSumValue(DataTable tblDetail, int masterID, string dateFieldName,
            string sumFieldName, DateTime startPeriodDate, DateTime endPeriodDate, bool includeDate1, bool includeDate2)
        {
            // Оставляем только детали по данному договору
            var drsSelect = tblDetail.Select(
                String.Format("{1} = {0}", masterID, GetParentRefName()),
                String.Format("{0} asc", dateFieldName));
            
            decimal sum = 0;
            var fieldNames = sumFieldName.Split(',');
            sumIncludedRows.Clear();
            var allValues = 
                startPeriodDate.ToShortDateString() == DateTime.MinValue.ToShortDateString()
                && endPeriodDate.ToShortDateString() == DateTime.MaxValue.ToShortDateString();

            foreach (var row in drsSelect)
            {
                var writeText = false;
                decimal sumRow = 0;

                foreach (var fieldName in fieldNames)
                {
                    if (row[fieldName] == DBNull.Value || !CheckColumnFilter(row))
                    {
                        continue;
                    }

                    var sumValue = Convert.ToDecimal(row[fieldName]);

                    // оставлять в списке только положительные или отрицательные
                    if (columnParamList[currentColumnIndex].ContainsKey(ParamSumValueType))
                    {
                        var sumType = columnParamList[currentColumnIndex][ParamSumValueType];

                        if (String.Compare(sumType, "-", true) == 0 && sumValue >= 0 ||
                            String.Compare(sumType, "+", true) == 0 && sumValue <= 0)
                        {
                            continue;
                        }
                    }

                    if (allValues)
                    {
                        sum += sumValue;
                        sumRow += sumValue;
                        sumIncludedRows.Add(row);
                        writeText = true;
                    }
                    else
                    {
                        if (row[dateFieldName] != DBNull.Value)
                        {
                            var dateValue = Convert.ToDateTime(row[dateFieldName]);
                            // Если дата попадает в диапазон, либо при установленном includeDate 
                            //точно соответствует одной из границ диапазона
                            if ((dateValue < endPeriodDate || includeDate2 && DateEqual(dateValue, endPeriodDate))
                                && (dateValue > startPeriodDate || includeDate1 && DateEqual(dateValue, startPeriodDate)))
                            {
                                writeText = true;
                                sum += sumValue;
                                sumRow += sumValue;
                                sumIncludedRows.Add(row);
                            }
                        }
                    }
                }

                if (writeText)
                {
                    ConcateDetailText(row[dateFieldName], sumRow);
                }
            }

            if (valuesSeparator.Length > 0)
            {
                fullDetailText = fullDetailText.TrimStart(valuesSeparator[0]).Trim();
            }
            
            return Convert.ToDecimal(sum);
        }

        private bool DateEqual(DateTime date1, DateTime date2)
        {
            return date1.Date == date2.Date;
        }

        // Разбирает значение вида [номер детали](
        //{0 нижняя граница диапазона, 1 - верхняя граница диапазона}<|>|<=|>='dd.mm.yy' 
        // повторить для настройик всех границ) БЕЗ ПРОБЕЛОВ!
        private decimal ParseValue(int id, ref string formula)
        {
            var balance = 1;
            var startPos = formula.IndexOf('(');
            var endPos = -1;
            int test;
            // частный случай значения поля таблицы фактов CreditEndDate
            if (Convert.ToString(formula[0]) == "'")
            {
                endPos = formula.IndexOf("'", 1);
                var dateStr = formula.Substring(1, endPos - 1);
                DateTime dt;
                formula = formula.Remove(0, endPos + 1);
                dt = Convert.ToDateTime(DateTime.TryParse(dateStr, out dt) ? dateStr : creditEndDate);
                var day = Convert.ToString(dt.Day);
                if (dt.Day < 10) day = "0" + day ;
                var month = Convert.ToString(dt.Month);
                if (dt.Month < 10) month = "0" + month;
                return Convert.ToDecimal(String.Format("{0}{1}{2}", dt.Year, month, day));
            }
            // частный случай вычисления - число
            if (Int32.TryParse(Convert.ToString(formula[0]), out test))
            {
                endPos = formula.IndexOf(',');
                
                if (endPos == -1) 
                {
                    formula = String.Empty;
                    return Convert.ToDecimal(formula);
                }

                var numberStr = formula.Substring(0, endPos);
                formula = formula.Remove(0, numberStr.Length);
                return Convert.ToDecimal(numberStr);
            }
            // ищем закрывающую скобку выражения границ
            for (var i = startPos + 1; i < formula.Length; i++)
            {
                if (formula[i] == ')') balance--;
                if (formula[i] == '(') balance++;
                if (balance != 0) continue;
                endPos = i;
                break;
            }
            // Она есть
            if (endPos > 0)
            {
                // Индекс детали по которой считаем
                var detailIndex = Convert.ToInt32(formula.Substring(1, startPos - 2));
                formula = formula.Remove(0, startPos);
                // dimKind1 = 0 - начальная граница, 1 - конечная
                var dimKind1 = formula.Substring(1, 1);
                formula = formula.Remove(0, 2);
                // Знак сравнения
                var sign1 = ParseSign(formula);
                formula = CutSign(formula, sign1);
                // Значение даты ограничения
                var date1 = formula.Substring(0, 10);
                // Повторить для второго ограничения
                var sign2 = -1;
                var date2 = String.Empty;
                formula = formula.Remove(0, 10);
                
                if (endPos > 20)
                {
                    formula = formula.Remove(0, 1);
                    sign2 = ParseSign(formula);
                    formula = CutSign(formula, sign2);
                    date2 = formula.Substring(0, 10);
                    formula = formula.Remove(0, 10);
                }

                formula = formula.Remove(0, 1);
                // Границы надо чем то заполнить даже если их не указали
                var startDate = DateTime.MinValue;
                var endDate = DateTime.MaxValue;
                var dateFieldName = GetEndDates()[detailIndex];
                var include1 = false;
                var include2 = false;
                if (dimKind1 == "0") dateFieldName = GetStartDates()[detailIndex];
                if (dimKind1 == "2") dateFieldName = GetMiddleDates()[detailIndex];
                
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

                var sumFieldName = ReportConsts.SumField;
                decimal currencyMult = 1;
                
                if (currencyType != -1 && !ignoreCurrencyCalc)
                {
                    sumFieldName = ReportConsts.CurrencySumField;
                    currencyMult = okvValues[currencyType];
                }
                
                if (currentFieldList.Length > 0)
                {
                    var sumSeqIndex = 0;
                    if (currencyType != -1) sumSeqIndex = 1;
                    sumFieldName = currentFieldList.Split(';')[sumSeqIndex];
                }

                if (!dtDetail[detailIndex].Columns.Contains(sumFieldName))
                {
                    var fieldList = sumFieldName.Split(',');
                    var hasFields = true;

                    foreach (var fieldName in fieldList)
                    {
                        hasFields = hasFields && dtDetail[detailIndex].Columns.Contains(fieldName);
                    }

                    if (!hasFields)
                    {
                        sumFieldName = ReportConsts.SumField;
                        currencyMult = 1;
                    }
                }

                if (currentIgnoreExchenge)
                {
                    currencyMult = 1;
                }

                var resultSum = currencyMult * GetSumValue(
                    dtDetail[detailIndex],
                    id,
                    dateFieldName,
                    sumFieldName,
                    startDate,
                    endDate,
                    include1,
                    include2);

                var hasCurrencyField = dtDetail[detailIndex].Columns.Contains(ReportConsts.CurrencySumField);

                if (currencyType != -1 && ignoreZeroCurrencyRows && sumFieldName == ReportConsts.SumField && hasCurrencyField)
                {
                    resultSum = RecalcCurrencySum();
                }

                if (ignoreNegativeSum && resultSum < 0)
                {
                    resultSum = 0;
                }

                return resultSum;
            }

            return 0;
        }

        private decimal RecalcCurrencySum()
        {
            return (from rowDetail in sumIncludedRows
                    let currencySum = GetNumber(rowDetail[ReportConsts.CurrencySumField])
                    where Math.Abs(currencySum) > (decimal)0.001
                    select GetNumber(rowDetail[ReportConsts.SumField])).Sum();
        }

        // Знак действия с суммами\сравнения фильтров дат
        private static int ParseSign(string formula)
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
        private static string ParseDelimiter(string formula)
        {
            return formula.Remove(0, 1);
        }        
        // Складывает суммы деталей
        private static decimal ExecuteCommand1(decimal op1, decimal op2, int command)
        {
            if (command == 1) return op1 - op2;
            if (command == 2) return op1 + op2;
            return 0;
        }
        // Сравнивает суммы деталей
        private static bool ExecuteCommand2(decimal op1, decimal op2, int command)
        {
            if (command == 3) return op1 > op2;
            if (command == 4) return op1 < op2;
            if (command == 5) return op1 >= op2;
            if (command == 6) return op1 <= op2;
            return false;
        }
        // Режет знак в зависимости от его длины
        private static string CutSign(string formula, int sign)
        {
            formula = formula.Remove(0, 1);
            if (sign > 4) formula = formula.Remove(0, 1);
            return formula;
        }
        // Парсер формулы
        public decimal ParseFormula(int id, ref string formula)
        {
            decimal op1, op2;
            decimal result = 0;
            var sign = ParseSign(formula);
            
            // Если первым делом был знак
            if (sign > 0)
            {
                formula = CutSign(formula, sign);
                // То сначала может идти число
                op1 = formula.StartsWith("[") ? ParseValue(id, ref formula) : ParseFormula(id, ref  formula);
                // И второй операнд может быть
                op2 = formula.StartsWith("[") ? ParseValue(id, ref formula) : ParseFormula(id, ref formula);
                // Операнды считаны - считаем
                return ExecuteCommand1(op1, op2, sign);
            }
            
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
                    var op3 = formula.StartsWith("[") ? 
                        ParseValue(id, ref formula) : 
                        ParseFormula(id, ref formula);

                    formula = ParseDelimiter(formula);
                        
                    // Операнд если выражение неверно
                    var op4 = formula.StartsWith("[") ? 
                        ParseValue(id, ref formula) : 
                        ParseFormula(id, ref  formula);
                        
                    // Считаем выражение
                    return ExecuteCommand2(op1, op2, sign) ? op3 : op4;
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
            return result;
        }

        // Сортировка датасета(возможна иерархическая)
        public DataTable SortDataSet(DataTable dt, string orderStr)
        {
            return SortDataSet(dt, orderStr, false);
        }

        // Сортировка датасета(возможна иерархическая)
        public DataTable SortDataSet(DataTable dt, string orderStr, bool hierarchySort)
        {
            var dtTemp = dt.Clone();
            if (hierarchySort)
            {
                var rowsMaster = dt.Select("ParentID is null", orderStr);
                
                foreach (var t in rowsMaster)
                {
                    dtTemp.ImportRow(t);
                    var rowsDetail = dt.Select(String.Format("ParentID={0}", t["id"]), orderStr);

                    foreach (var row in rowsDetail)
                    {
                        dtTemp.ImportRow(row);
                    }
                }

                dtTemp.AcceptChanges();
            }
            else
            {
                var rows = dt.Select(String.Empty, orderStr);

                foreach (var t in rows)
                {
                    dtTemp.ImportRow(t);
                }

                dtTemp.AcceptChanges();
            }
            return dtTemp;
        }

        /// <summary>
        /// расчет текущего остатка долга
        /// </summary>
        public decimal GetCurrentRest(int masterKey, int refOKV)
        {
            var drsFactAttract = dtDetail[0].Select(String.Format("{1} = {0}", masterKey, GetParentRefName()));
            var drsFactDebt = dtDetail[1].Select(String.Format("{1} = {0}", masterKey, GetParentRefName()));

            var sumFieldName = ReportConsts.SumField;
            decimal currencyMult = 1;
            
            if (refOKV != -1)
            {
                currencyMult = okvValues[refOKV];
                sumFieldName = ReportConsts.CurrencySumField;
            }

            var sum = drsFactAttract.Where(
                detailRow => !detailRow.IsNull(sumFieldName)).Sum(
                    detailRow => Convert.ToDecimal(detailRow[sumFieldName]));

            sum = drsFactDebt.Where(
                detailRow => !detailRow.IsNull(sumFieldName)).Aggregate(
                    sum, (current, detailRow) => current - Convert.ToDecimal(detailRow[sumFieldName]));

            return sum * currencyMult;
        }

        // Некоторые колонки должны заполняться не всегда (например для определенного типа договора только)
        private void CheckColumnCondition(int cellIndex, DataRow sourceRow, DataRow destRow)
        {
            if (columnCondition.ContainsKey(cellIndex))
            {
                if (columnCondition[cellIndex].Length > 0)
                {
                    var conditionList = columnCondition[cellIndex].Split(';');

                    foreach (var conditionStr in conditionList)
                    {
                        string[] columnParam;
                        bool isEqual;
                        if (conditionStr.IndexOf("=") > 0)
                        {
                            columnParam = conditionStr.Split('=');
                            isEqual = true;
                        }
                        else
                        {
                            columnParam = conditionStr.Split('!');
                            isEqual = false;
                        }
                        
                        var values = columnParam[1].Split(',');
                        var sourceValue = sourceRow.Table.Columns.Contains(columnParam[0]) ?
                            Convert.ToString(sourceRow[columnParam[0]]) :
                            Convert.ToString(destRow[columnParam[0]]);
                        
                        var saveValue = false;

                        foreach (var t in values)
                        {
                            if (isEqual)
                            {
                                saveValue = saveValue || (t == sourceValue);
                            }
                            else
                            {
                                saveValue = saveValue || (t != sourceValue);
                            }
                        }

                        if (saveValue) continue;

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

        // Парсит формулы чтобы вычислить используемые детали(чтобы за ненужными в базу не лезть) 
        private void FillUsedDetailList()
        {
            if (calcColumnTypes.ContainsValue(CalcColumnType.cctCurrentRest))
            { 
                usedDetails.Add(0);
                usedDetails.Add(1);
            }

            for (var i = 0; i < columnCount; i++)
            {
                if (calcColumnTypes[i] != CalcColumnType.cctDetail && 
                    calcColumnTypes[i] != CalcColumnType.cctDetailText)
                        continue;

                var value = columnParamList[i][ParamFormula];
                var position1 = value.IndexOf("[");
                var position2 = value.IndexOf("]");
                
                while (position1 >= 0 && position2 > 0)
                {
                    var detailIndex = Convert.ToInt32(value.Substring(position1 + 1, position2 - position1 - 1));
                    if (!usedDetails.Contains(detailIndex)) usedDetails.Add(detailIndex);
                    value = value.Remove(0, position2 + 1);
                    position1 = value.IndexOf("[");
                    position2 = value.IndexOf("]");
                }
            }
        }

        // выбиратор строки из детали(detailIndex) нужного договора(parentId), которая наиболее близка к дате(calcDate) 
        // сверху(nearestUp = true) или снизу(nearestUp = false)
        public DataRow GetNearestRow(int detailIndex, string calcDate, object parentId, bool nearestUp)
        {
            DataTable dtPlanDetail;
            string dateFieldName;
            
            // если деталь с суммами
            if (detailIndex >= 0)
            {
                dtPlanDetail = dtDetail[detailIndex];
                dateFieldName = GetEndDates()[detailIndex];
            }
            else
            // если журнал процентов
            {
                dtPlanDetail = dtJournalPercent;
                dateFieldName = t_S_JournalPercentCI.ChargeDate;
            }
            
            var nearestSign = ">=";
            var nearestSort = "asc";
            
            if (!nearestUp)
            {
                nearestSign = "<";
                nearestSort = "desc";
            }
            
            if (dtPlanDetail != null)
            {
                var drsSelect = dtPlanDetail.Select(
                    String.Format("{0}{1}'{2}' and {3}={4}", dateFieldName, nearestSign, calcDate, GetParentRefName(), parentId),
                    String.Format("{0} {1}", dateFieldName, nearestSort));
                
                if (drsSelect.Length > 0)
                {
                    return drsSelect[0];
                }
            }

            return null;
        }

        public string GetDateValue(object dateCell)
        {
            var strDate = String.Empty;
            
            if (dateCell != DBNull.Value)
                strDate = Convert.ToDateTime(dateCell).ToShortDateString();

            return strDate;
        }

        protected class ParamsDetailFilter
        {
            public object masterKey { get; set; }
            public bool onlyLastPlan { get; set; }
            public bool isPartial { get; set; }
            public string planMaxDate { get; set; }
            public string dateField { get; set; }
            public string detailIndex { get; set; }
            public List<string> Items { get; set; }
        }

        private string GetEQStrFilter(string field, object value)
        {
            return String.Format("{0} = '{1}'", field, value);
        }

        protected void InternalDetailFilter(ParamsDetailFilter paramsFilter)
        {
            var planIndex = Convert.ToInt32(paramsFilter.detailIndex);

            if (!paramsFilter.onlyLastPlan || 
                safespotDetail[planIndex] == null || 
                safespotDetail[planIndex].Rows.Count == 0)
            {
                return;
            }

            dtDetail[planIndex] = safespotDetail[planIndex];
            var fltCredit = String.Format("{0} = {1}", GetParentRefName(), paramsFilter.masterKey);
            var tblCreditPlan = DataTableUtils.FilterDataSet(safespotDetail[planIndex], fltCredit);

            if (tblCreditPlan.Rows.Count == 0)
            {
                return;
            }

            var fltDate = String.Format("{0} <= '{1}'", paramsFilter.dateField, paramsFilter.planMaxDate);
            var fltNull = String.Format("{0} is null", paramsFilter.dateField);
            var tblDebtPlan = DataTableUtils.FilterDataSet(tblCreditPlan, fltDate);

            if (tblDebtPlan.Rows.Count == 0)
            {
                tblDebtPlan = DataTableUtils.FilterDataSet(tblCreditPlan, fltNull);

                if (tblDebtPlan.Rows.Count > 0)
                {
                    dtDetail[planIndex] = tblDebtPlan;
                    return;
                }
            }

            tblDebtPlan = DataTableUtils.SortDataSet(tblDebtPlan, paramsFilter.dateField);

            if (tblDebtPlan.Rows.Count > 0)
            {
                var activePlanDebtRow = tblDebtPlan.Rows[tblDebtPlan.Rows.Count - 1];
                var dateValue = activePlanDebtRow[paramsFilter.dateField];
                dtDetail[planIndex] = DataTableUtils.FilterDataSet(
                    tblDebtPlan,
                    GetEQStrFilter(paramsFilter.dateField, dateValue));
            }
            else
            {
                if (paramsFilter.isPartial)
                {
                    if (paramsFilter.Items.Count > 0)
                    {
                        dtDetail[planIndex] = DataTableUtils.FilterDataSet(
                            tblDebtPlan,
                            GetEQStrFilter(paramsFilter.dateField, paramsFilter.Items[0]));
                    }
                }
            }
        }
    }
}
