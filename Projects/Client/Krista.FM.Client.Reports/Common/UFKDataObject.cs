using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Reports.Database;

namespace Krista.FM.Client.Reports.UFK
{
    // Тип колонки
    public enum UFKFieldType
    {
        ftClsData = 0, // Поле заголовка
        ftData = 1, // Поле из БД
        ftRelation = 2, // Формула
        ftTemplate = 3, // Шаблонное поле с подстановками
        ftUniqueData = 4 // Поле из БД
    }

    // информация о колонке
    public class UFKColumnInfo
    {
        // тип поля
        public UFKFieldType columnType;
        // таблица к которой колонка принадлежит
        public AbstractTable columnTable;
        // имя колонки
        public string columnName;
        // по какой сущности фильтруем
        public Dictionary<AbstractTable, Collection<string>> columnFilters = new Dictionary<AbstractTable, Collection<string>>();
        // признак что темлейтовая
        public string templateStr;
        public bool onlyFilter;
    }

    // параметры ведущей колонки(район, месяц и прочее)
    public class UFKLeadRows
    {
        public string leadFieldName;
        // ведущая таблица классификатора
        public AbstractTable leadTable;
        // значения фильтра
        public Collection<string> leadFilterValues = new Collection<string>();
    }

    /// <summary>
    /// Объект выборок данных из таблиц фактов
    /// </summary>
    public class UFKDataObject
    {
        // схема
        private IScheme scheme;
        // Результирующий датасет
        private DataTable dtResult;
        // Параметры колонок(тип, подтип и параметры)
        public Dictionary<int, UFKColumnInfo> columnList = new Dictionary<int, UFKColumnInfo>();
        // Фильтры таблицы фактов
        public Dictionary<IEntity, Dictionary<string, string>> mainFilter = new Dictionary<IEntity, Dictionary<string, string>>();
        // параметры ведущей
        public UFKLeadRows leadingInfo = new UFKLeadRows();
        // список таблиц данных
        public List<IEntity> dbDataTables = new List<IEntity>();
        // Данные по объединенным фильтрам, из которых потом по клеткам таблицы растащим
        readonly Dictionary<IEntity, DataTable> dirtyData = new Dictionary<IEntity, DataTable>();
        // Связи с классификаторами
        readonly Dictionary<IEntity, Dictionary<IEntity, string>> relationPaths = new Dictionary<IEntity, Dictionary<IEntity, string>>();
        // связи с таблицами
        readonly Collection<IEntity> linkedTables = new Collection<IEntity>();
        // точность вычислений
        public PrecisionNumberEnum precision = PrecisionNumberEnum.ctN2;
        // делитель сумм
        public SumDividerEnum divider = SumDividerEnum.i1;
        // часть названия итогового подзаголовка по подчиненным записям
        public string summaryHierarchySuffix = String.Empty;
        // выводить ли итоговую строку
        public bool useSummaryRow = true;
        // выводить ли пустые строчки
        public bool useEmptyRows = true;
        //
        readonly Dictionary<string, DataRow[]> selectCache = new Dictionary<string, DataRow[]>();

        private readonly Dictionary<string, IEntity> cacheEntity = new Dictionary<string, IEntity>();

        // служебные колонки группировки
        readonly Collection<string> serviceColumns = new Collection<string>();

        // Реинкарнатор объекта
        public void InitObject(IScheme currentScheme)
        {
            scheme = currentScheme;
        }

        private IEntity GetEntity(string key)
        {
            if (cacheEntity.ContainsKey(key))
            {
                return cacheEntity[key];
            }
            
            var returnEntity = scheme.RootPackage.FindEntityByName(key);
            cacheEntity.Add(key, returnEntity);
            
            return returnEntity;
        }

        // ручное добавление путей - потому что автоматическое не всегда однозначно
        public void AddClsLink(AbstractTable clsTable, AbstractTable linkTable, string linkField)
        {
            var clsEntity = GetEntity(clsTable.InternalKey);

            if (relationPaths.ContainsKey(clsEntity))
            {
                var linkEntity = GetEntity(linkTable.InternalKey);
                
                if (!relationPaths[clsEntity].ContainsKey(linkEntity))
                {
                    relationPaths[clsEntity].Add(linkEntity, linkField);
                }
            }
            else
            {
                var entityPath = new Dictionary<IEntity, string> {{GetEntity(linkTable.InternalKey), linkField}};
                relationPaths.Add(clsEntity, entityPath);
            }
        }

        // добавление сущности в список таблиц фактов(надо отличать класификаторы от фактов)
        public void AddDataEntity(AbstractTable dataTable)
        {
            dbDataTables.Add(GetEntity(dataTable.InternalKey));
        }

        // фильтры накладываем по таблицам классификаторов\фактов
        public void AddFilter(AbstractTable baseTable, string columnFilter)
        {
            // если не было еще такого фильтра
            var filterEntity = GetEntity(baseTable.InternalKey);
            
            if (!mainFilter.ContainsKey(filterEntity))
            {
                mainFilter.Add(filterEntity, new Dictionary<string, string>());
            }

            var filterValues = mainFilter[filterEntity];
            // дубли не добавляем
            var filterParts = SplitFilterParts(columnFilter);

            if (filterParts.Length < 2) return;

            var addedFieldName = filterParts[0];
            var addedFieldValue = filterParts[1];

            if (columnFilter.Contains("<") || columnFilter.Contains(">"))
            {
                if (!filterValues.ContainsKey(columnFilter))
                {
                    filterValues.Add(columnFilter, string.Empty);
                }
            }
            else
            {
                if (!filterValues.ContainsKey(addedFieldName))
                {
                    filterValues.Add(addedFieldName, addedFieldValue);
                }
                else
                {
                    filterValues[addedFieldName] = string.Format("{0},{1}", filterValues[addedFieldName], addedFieldValue);
                }
            }
        }

        public void AddClsColumn(AbstractTable baseTable, string columnName)
        {
            AddDataColumn(baseTable, columnName, UFKFieldType.ftClsData);
        }

        public void AddDataColumn(AbstractTable baseTable, string columnName)
        {
            AddDataColumn(baseTable, columnName, UFKFieldType.ftData);
        }

        public void AddDataUniqueColumn(AbstractTable baseTable, string columnName)
        {
            AddDataColumn(baseTable, columnName, UFKFieldType.ftUniqueData);
        }

        public void AddTemplateColumn(AbstractTable baseTable, string templateStr)
        {
            AddDataColumn(baseTable, templateStr, UFKFieldType.ftTemplate);
        }

        public void AddRelationColumn(AbstractTable baseTable, string relationText)
        {
            AddDataColumn(baseTable, relationText, UFKFieldType.ftRelation);
        }

        // колонка из БД
        private void AddDataColumn(AbstractTable baseTable, string columnName, UFKFieldType columnType)
        {
            var columnInfo = new UFKColumnInfo
                                 {
                                     columnType = columnType,
                                     columnTable = baseTable,
                                     columnName = columnName,
                                     templateStr = columnName
                                 };

            columnList.Add(columnList.Count, columnInfo);
        }

        public void SetColumnUsing(bool onlyFilter)
        {
            columnList[columnList.Count - 1].onlyFilter = onlyFilter;            
        }

        public void AddColumnFilter(AbstractTable filterTable, string columnFilter)
        {
            var columnInfo = columnList[columnList.Count - 1];
            var filterCollection = new Collection<string>();
            
            if (columnInfo.columnFilters.ContainsKey(filterTable))
            {
                if (columnInfo.columnFilters[filterTable] == null)
                {
                    columnInfo.columnFilters[filterTable] = new Collection<string>();
                }
                
                filterCollection = columnInfo.columnFilters[filterTable];
            }
            else
            {
                columnInfo.columnFilters.Add(filterTable, filterCollection);
            }
            
            filterCollection.Add(columnFilter);
        }

        // структура выводимых столбцов
        private void CreateResultColumns()
        {
            dtResult = new DataTable();
            
            foreach (var dataColumn in columnList.Select(t => dtResult.Columns.Add()))
            {
                dataColumn.DataType = typeof(String);
            }
        }

        // расщепление на части общей записи фильтра имя=значение
        private static string[] SplitFilterParts(string filterValue)
        {
            var splitEqualsStr = new string[2];
            splitEqualsStr[0] = "<=";
            splitEqualsStr[1] = ">=";

            var splitNotEqualsStr = new string[2];
            splitNotEqualsStr[0] = "<";
            splitNotEqualsStr[1] = ">";

            var currentSplitTemplates = new[] {"="};

            if (filterValue.Contains(splitEqualsStr[0]) || filterValue.Contains(splitEqualsStr[1]))
            {
                currentSplitTemplates = splitEqualsStr;
            }
            else if (filterValue.Contains(splitNotEqualsStr[0]) || filterValue.Contains(splitNotEqualsStr[1]))
            {
                currentSplitTemplates = splitNotEqualsStr;
            }

            return filterValue.Split(currentSplitTemplates, StringSplitOptions.None);
            //return filterValue.Split('=');
        }

        // структура выводимых строк
        private void CreateResultRows()
        {
            for (var i = 0; i < dirtyData[dbDataTables[0]].Rows.Count; i++)
            {
                dtResult.Rows.Add();
            }
        }

        // общая запись фильтра имя=значение
        private static string FormFilterValue(string fieldName, string fieldValue)
        {
            return String.Format("{0}={1}", fieldName, fieldValue);
        }

        public static decimal GetNumber(object obj)
        {
            decimal result;

            if (!decimal.TryParse(obj.ToString(), out result))
            {
                result = 0;
            }
            
            return result;
        }

        public static decimal GetDecimal(object obj)
        {
            decimal result;

            if (!decimal.TryParse(obj.ToString(), out result))
            {
                result = 0;
            }

            return result;
        }

        // считарь сумм
        private decimal CalcSumValue(DataRow rowData, string fieldName)
        {
            // округлитель сумм
            var roundDigits = 2;
            
            if (precision == PrecisionNumberEnum.ctN1)
            {
                roundDigits = 1;
            }
            
            // делитель сумм
            double dividerValue = 1;
            if (divider == SumDividerEnum.i2)
            {
                dividerValue = Math.Pow(10, 3);
            }
            
            if (divider == SumDividerEnum.i3)
            {
                dividerValue = Math.Pow(10, 6);
            }
            
            if (divider == SumDividerEnum.i4)
            {
                dividerValue = Math.Pow(10, 9);
            }
            
            // вычислятор сумм
            var result = GetNumber(rowData[fieldName]);

            // возвращатель результата
            return Math.Round(result, roundDigits) / (decimal)dividerValue;
        }

        private static string CorrectFilterText(string filterValue)
        {
            var filterParts = SplitFilterParts(filterValue);

            if (filterParts.Length > 1)
            {
                var filterValues = filterParts[1];
                var filterName = filterParts[0];
                
                if (filterValues.Split(',').Length > 1)
                {
                    return string.Format("{0} in ({1})", filterName, filterValues);
                }
                
                if (filterValues.Contains(".."))
                {
                    var filterBounds = filterValues.Split('.');
                    return String.Format("{0} >= {1} and {0} <= {2}", filterName, filterBounds[0], filterBounds[2]);
                }
            }
            
            return filterValue;
        }

        private void EvaluateSingleRow(DataRow rowData, int rowIndex)
        {
            DataTable tblData;

            for (var j = 0; j < columnList.Count; j++)
            {
                var columnInfo = columnList[j];
                if (columnInfo.columnType != UFKFieldType.ftData 
                    && columnInfo.columnType != UFKFieldType.ftClsData 
                    && columnInfo.columnType != UFKFieldType.ftUniqueData) continue;

                var columnEntity = GetEntity(columnInfo.columnTable.InternalKey);
                var columnPrefix = GetTablePrefix(linkedTables, columnEntity);
                var columnName = GetPseudoName(columnPrefix, columnInfo.columnName);

                var dirtyColumnData = dirtyData[dbDataTables[0]];
                var vFullFilter = String.Empty;
                if (columnInfo.columnType == UFKFieldType.ftData)
                {
                    foreach (var pair in columnInfo.columnFilters)
                    {
                        var ttlCount = new Dictionary<string, int>();
                        var curCount = new Dictionary<string, int>();

                        foreach (var filterPart in pair.Value)
                        {
                            var parts = SplitFilterParts(filterPart);

                            if (!ttlCount.ContainsKey(parts[0]))
                            {
                                ttlCount.Add(parts[0], 1);
                                curCount.Add(parts[0], 0);
                            }
                            else
                            {
                                ttlCount[parts[0]] += 1;
                            }
                        }

                        foreach (var filterPart in pair.Value)
                        {
                            var vFilter = filterPart;
                            var filterEntity = GetEntity(pair.Key.InternalKey);
                            vFilter = GetPseudoName(GetTablePrefix(linkedTables, filterEntity), vFilter);
                            vFilter = CorrectFilterText(vFilter);
                            var parts = SplitFilterParts(filterPart);

                            if (ttlCount[parts[0]] == 1)
                            {
                                vFullFilter = String.Format("{0} and {1}", vFullFilter, vFilter);
                            }
                            else
                            {
                                vFullFilter = String.Format(curCount[parts[0]] == 0
                                                                ? "{0} and ({1}"
                                                                : "{0} or {1}", vFullFilter, vFilter);

                                curCount[parts[0]] += 1;

                                if (curCount[parts[0]] == ttlCount[parts[0]])
                                {
                                    vFullFilter = String.Format("{0})", vFullFilter);
                                }
                            }
                        }
                    }
                }

                if (vFullFilter.Length > 0)
                {
                    vFullFilter = vFullFilter.Remove(0, 4);
                }

                tblData = dirtyColumnData;
                if (dirtyColumnData.Rows.Count > 0 && !columnInfo.onlyFilter)
                {
                    if (columnInfo.columnType == UFKFieldType.ftData)
                    {
                        var dirtyRow = dirtyColumnData.Rows[rowIndex];

                        DataRow[] rowsSelect;
                        if (selectCache.ContainsKey(vFullFilter))
                        {
                            rowsSelect = selectCache[vFullFilter];
                        }
                        else
                        {
                            rowsSelect = dirtyColumnData.Select(vFullFilter);
                            selectCache.Add(vFullFilter, rowsSelect);
                        }

                        if (!rowsSelect.Contains(dirtyRow))
                        {
                            rowData[j] = 0;
                        }
                        else
                        {
                            rowData[j] = CalcSumValue(dirtyRow, columnName);
                        }

                    }
                    else
                    {
                        rowData[j] = tblData.Rows[rowIndex][columnName];
                    }
                }
            }

            for (var j = 0; j < columnList.Count; j++)
            {
                var columnInfo = columnList[j];
                
                if (columnInfo.columnType == UFKFieldType.ftTemplate)
                {
                    var resultStr = columnInfo.templateStr;
                    
                    for (var i = 0; i < columnList.Count; i++)
                    {
                        resultStr = resultStr.Replace(String.Format("<{0}>", i), Convert.ToString(rowData[i]));
                    }

                    rowData[j] = resultStr;
                }

                if (columnInfo.columnType != UFKFieldType.ftRelation) continue;

                var formulaParts = columnInfo.templateStr.Split(';');
                decimal sum = 0;
                    
                foreach (var formulaPart in formulaParts)
                {
                    if (formulaPart.Length > 1)
                    {
                        var sign = formulaPart[0];
                        var tempVal = GetNumber(rowData[Convert.ToInt32(formulaPart.Remove(0, 1))]);
                            
                        switch (sign)
                        {
                            case '%' :
                                sum /= tempVal;
                                break;
                            case '*':
                                sum *= tempVal;
                                break;
                            case '-':
                                sum -= tempVal;
                                break;
                            default:
                                sum += tempVal;
                                break;
                        }
                    }
                }

                rowData[j] = sum;
            }
        }

        // раскидываем данные из общей выборки по строкам и столбцам
        private void SpreadResultData()
        {
            selectCache.Clear();

            for (var i = 0; i < dtResult.Rows.Count; i++)
            {
                EvaluateSingleRow(dtResult.Rows[i], i);
            }

            selectCache.Clear();
        }

        // Номер сущности в общем списке, ибо индексатор коллекции не на всех сущностях работает... пичаль
        private static int GetEntityIndex(Collection<IEntity> linkedTables, IEntity currentTable)
        {
            if (linkedTables == null) throw new ArgumentNullException("LinkedTables");
            if (currentTable == null) throw new ArgumentNullException("CurrentTable");

            for (int i = 0; i < linkedTables.Count; i++)
            {
                if (linkedTables[i].FullDBName == currentTable.FullDBName)
                {
                    return i;
                }
            }

            return -1;
        }

        // префикс таблицы в зависимости от позиции в списке
        private static string GetTablePrefix(Collection<IEntity> tables, IEntity currentTable)
        {
            var tablePrefix = "t";
            var clsIndex = GetEntityIndex(tables, currentTable); 
            
            if (clsIndex > 0)
            {
                tablePrefix = GetPseudoName(tablePrefix, clsIndex);
            }
            
            return tablePrefix;
        }

        // имя поля с префиксом
        private static string GetFieldName(string prefix, string fieldName)
        {
            return String.Format("{0}.{1}", prefix, fieldName);
        }

        // имя поля с префиксом
        private static string GetPseudoName(string prefix, object fieldName)
        {
            return String.Format("{0}{1}", prefix, fieldName);
        }

        // Вытаскивает все нужные данные из БД одним запросом
        private void QueryDirtyData()
        {
            foreach (var dataTbl in dbDataTables)
            {
                var selectStr = new Collection<string>();
                var whereHardStr = new Collection<string>();
                var whereSoftStr = new Collection<string>();
                var dirtyHardWhereStr = new Dictionary<string, Collection<string>>();
                var dirtySoftWhereStr = new Dictionary<string, Collection<string>>();
                var fromStr = new Collection<string>();
                var groupList = new Collection<string>();

                // определяем используемые в выборке запросы
                linkedTables.Add(dataTbl);
                
                foreach (var linkedTable in relationPaths.Keys)
                {
                    if (relationPaths[linkedTable].ContainsKey(dataTbl))
                    {
                        linkedTables.Add(linkedTable);
                        
                        foreach (var pair in relationPaths[linkedTable])
                        {
                            if (!linkedTables.Contains(pair.Key))
                            {
                                linkedTables.Add(pair.Key);
                            }
                        }                        
                    }
                }
                
                // собираем каких таблиц выбирать
                foreach (var linkedTable in linkedTables)
                {
                    var tablePrefix = GetTablePrefix(linkedTables, linkedTable);
                    fromStr.Add(String.Format("{0} {1}", linkedTable.FullDBName, tablePrefix));
                }

                // фильтры на таблице фактов
                foreach (var pair in mainFilter)
                {
                    var fieldPrefix = GetTablePrefix(linkedTables, pair.Key);
                    
                    foreach (var filterPart in pair.Value)
                    {
                        var filterName = GetFieldName(fieldPrefix, filterPart.Key);
                        var filterValue = FormFilterValue(filterName, filterPart.Value);
                        
                        if (filterPart.Value == String.Empty)
                        {
                            filterValue = filterName;
                        }
                        
                        filterValue = CorrectFilterText(filterValue);

                        if (!dirtyHardWhereStr.ContainsKey(filterValue))
                        {
                            dirtyHardWhereStr.Add(filterValue, new Collection<string>());
                        }
                    }
                }

                // собираем строчку выбираемых полей и условий
                foreach (var columnInfo in columnList.Values)
                {
                    if (columnInfo.columnType != UFKFieldType.ftData 
                        && columnInfo.columnType != UFKFieldType.ftClsData
                        && columnInfo.columnType != UFKFieldType.ftUniqueData) continue;

                    var columnEntity = GetEntity(columnInfo.columnTable.InternalKey);
                    var fieldPrefix = GetTablePrefix(linkedTables, columnEntity);
                    var isFactTable = columnEntity == dataTbl;
                    var clsIndex = GetEntityIndex(linkedTables, columnEntity);

                    if (!isFactTable && clsIndex < 0) continue;

                    var originalField = GetFieldName(fieldPrefix, columnInfo.columnName);
                    var selectedField = originalField;
                    var pseudoName = GetPseudoName(fieldPrefix, columnInfo.columnName);

                    if (columnInfo.columnType == UFKFieldType.ftData)
                    {
                        selectedField = String.Format("Sum({0})", selectedField);
                    }

                    selectedField = String.Format("{0} as {1}", selectedField, pseudoName);

                    if (!selectStr.Contains(selectedField) && !columnInfo.onlyFilter)
                    {
                        selectStr.Add(selectedField);

                        if (columnInfo.columnType != UFKFieldType.ftData)
                        {
                            groupList.Add(originalField);
                        }
                    }

                    if (columnInfo.columnFilters.Count <= 0) continue;

                    foreach (var pair in columnInfo.columnFilters)
                    {
                        if (columnInfo.onlyFilter) continue;

                        columnEntity = GetEntity(pair.Key.InternalKey);
                        fieldPrefix = GetTablePrefix(linkedTables, columnEntity);
                        foreach (var filterPart in pair.Value)
                        {
                            var filterParts = SplitFilterParts(filterPart);
                            var filterName = filterParts[0];
                            var filterValue = filterParts[1];
                            var whereField = GetFieldName(fieldPrefix, filterName);

                            if (dirtySoftWhereStr.ContainsKey(whereField))
                            {
                                if (!dirtySoftWhereStr[whereField].Contains(filterValue))
                                {
                                    dirtySoftWhereStr[whereField].Add(filterValue);
                                }
                            }
                            else
                            {
                                var filterValues = new Collection<string>();
                                            
                                // диапазон
                                if (filterValue.Contains(".."))
                                {
                                    var filterBounds = filterValue.Split('.');
                                    whereField = String.Format("{0} BETWEEN {1} and {2}", 
                                                               whereField, 
                                                               filterBounds[0],
                                                               filterBounds[2]);
                                }
                                    // тупое равенство
                                else
                                {
                                    filterValues.Add(filterValue);
                                }

                                if (!dirtySoftWhereStr.ContainsKey(whereField))
                                {
                                    dirtySoftWhereStr.Add(whereField, filterValues);
                                }
                            }
                        }
                    }
                }

                // собираем строчку выбираемых полей из фильтров
                foreach (var columnInfo in columnList.Values)
                {
                    if (columnInfo.columnFilters.Count > 0)
                    {
                        foreach (var pair in columnInfo.columnFilters)
                        {
                            foreach (var filterPart in pair.Value)
                            {
                                var filterParts = SplitFilterParts(filterPart);
                                var fieldEntity = GetEntity(pair.Key.InternalKey);
                                var fieldPrefix = GetTablePrefix(linkedTables, fieldEntity);
                                var originalField = GetFieldName(fieldPrefix, filterParts[0]);
                                var selectedField = originalField;

                                var pseudoName = GetPseudoName(fieldPrefix, filterParts[0]);
                                selectedField = String.Format("{0} as {1}", selectedField, pseudoName);

                                if (!selectStr.Contains(selectedField) && !columnInfo.onlyFilter)
                                {
                                    selectStr.Add(selectedField);
                                    groupList.Add(originalField);
                                    serviceColumns.Add(pseudoName);
                                }
                            }
                        }
                    }
                }

                foreach (var pair in dirtyHardWhereStr)
                {
                    whereHardStr.Add(pair.Value.Count > 0
                                         ? String.Format("{0} in ({1})", pair.Key, GetCollectionText(pair.Value))
                                         : pair.Key);
                }

                foreach (var pair in dirtySoftWhereStr)
                {
                    whereSoftStr.Add(pair.Value.Count > 0
                                         ? String.Format("{0} in ({1})", pair.Key, GetCollectionText(pair.Value))
                                         : pair.Key);
                }

                var softFilter = String.Empty;
                if (whereSoftStr.Count > 0)
                {
                    softFilter = String.Empty;

                    if (whereHardStr.Count > 0)
                    {
                        softFilter = " and ";
                    }

                    softFilter = String.Format(" {0} ({1}) ", softFilter, GetCollectionText(whereSoftStr, " or "));
                }

                var linkedText = String.Empty;
                if (linkedTables.Count > 0)
                {
                    linkedText = String.Format(" and {0} ", GetSQLLinkText(linkedTables));
                }

                var dataQuery = String.Format("select {0} from {1} where {2} {3}{4} group by {5}",
                    GetCollectionText(selectStr),
                    GetCollectionText(fromStr),
                    GetCollectionText(whereHardStr, " and "),
                    softFilter,
                    linkedText,
                    GetCollectionText(groupList));

                var dbHelper = new ReportDBHelper(scheme);
                dirtyData[dataTbl] = dbHelper.GetTableData(dataQuery);
            }
        }

        // сцеплятель таблиц по полям ссылкам
        private string GetSQLLinkText(Collection<IEntity> tables)
        {
            var result = new Collection<string>();

            foreach (var linkedTable in tables)
            {
                if (!relationPaths.ContainsKey(linkedTable)) continue;

                var tablePrefix = GetTablePrefix(tables, linkedTable);
                var isFirst = true;
                var fullPath = relationPaths[linkedTable];

                foreach (var pair in fullPath)
                {
                    var currentPrefix = GetTablePrefix(tables, pair.Key);
                    var fieldName = GetFieldName(currentPrefix, pair.Value);
                        
                    if (isFirst)
                    {
                        if (result.Count > 0)
                        {
                            fieldName = String.Format(" and {0}", fieldName);
                        }
                        
                        result.Add(fieldName);
                        isFirst = false;
                    }
                    else
                    {
                        result.Add(String.Format("= {0}.id and {1} ", currentPrefix, fieldName));
                    }
                }

                result.Add(String.Format(" = {0}", GetFieldName(tablePrefix, "id")));
            }

            return GetCollectionText(result, String.Empty);
        }

        // получает текст из коллекции разделенный ,

        // получает текст из коллекции разделенный splitter
        private static string GetCollectionText(IEnumerable<string> listValues, string splitter = ",")
        {
            var result = listValues.Aggregate(String.Empty, (current, value) => 
                String.Format("{0}{1}{2}", current, splitter, value));
            
            if (result.StartsWith(splitter))
            {
                result = result.Remove(0, splitter.Length);
            }
            
            if (result.EndsWith(splitter))
            {
                result = result.Remove(result.Length - splitter.Length - 1, splitter.Length);
            }
            
            return result;
        }

        private void MergeDuplicates()
        {
            var selectFormat = String.Empty;
            var mergeableFields = new Collection<int>();
            var duplicateList = new Collection<DataRow>();

            for (var i = 0; i < dtResult.Columns.Count; i++)
            {
                var colType = columnList[i].columnType;

                if (columnList[i].onlyFilter) continue;

                if (colType == UFKFieldType.ftClsData || colType == UFKFieldType.ftUniqueData)
                {
                    selectFormat += String.Format(
                        " and {0} =", dtResult.Columns[i].ColumnName) + "'{" + Convert.ToString(i) + "}'";
                }

                if (colType == UFKFieldType.ftData)
                {
                    mergeableFields.Add(i);
                }                
            }

            if (selectFormat.Length > 0)
            {
                selectFormat = selectFormat.Remove(0, 5);
            }
            else
            {
                return;
            }

            foreach (DataRow rowData in dtResult.Rows)
            {
                for (var i = 0; i < dtResult.Columns.Count; i++)
                {
                    if (rowData.IsNull(i)) rowData[i] = String.Empty;
                }
            }

            foreach (DataRow rowData in dtResult.Rows)
            {
                if (duplicateList.Contains(rowData)) continue;

                var values = new object[rowData.ItemArray.Length];
                Array.Copy(rowData.ItemArray, values, rowData.ItemArray.Length);

                for (var i = 0; i < values.Length; i++)
                {
                    if (values[i] != DBNull.Value)
                    {
                        values[i] = Convert.ToString(values[i]).Replace("\"", " ");
                        values[i] = Convert.ToString(values[i]).Replace(@"'", @"''");
                    }
                }

                var duplicateRows = dtResult.Select(String.Format(selectFormat, values));

                if (duplicateRows.Length <= 1) continue;

                for (var i = 1; i < duplicateRows.Length; i++)
                {
                    duplicateList.Add(duplicateRows[i]);

                    foreach (var mergeableField in mergeableFields)
                    {
                        rowData[mergeableField] =
                            GetNumber(rowData[mergeableField]) +
                            GetNumber(duplicateRows[i][mergeableField]);
                    }
                }
            }

            foreach (var dataRow in duplicateList)
            {
                dtResult.Rows.Remove(dataRow);
            }
        }

        // Заполняем данные
        public DataTable FillData()
        {
            QueryDirtyData();
            CreateResultColumns();
            CreateResultRows();
            SpreadResultData();
            MergeDuplicates();
            return dtResult;
        }
    }
}
