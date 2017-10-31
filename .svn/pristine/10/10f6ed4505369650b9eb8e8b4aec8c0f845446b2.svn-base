using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Diagnostics;
using Krista.FM.Client.Workplace.Gui;
using Krista.FM.Common;
using Krista.FM.Client.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
    #region LookupDataObject
    /// <summary>
    /// Класс для получения информации по одному объекту-лукапу
    /// </summary>
    internal sealed class LookupDataObject : DisposableObject
    {
        #region Константы
        // название колонки, содержащей признак полной загруженности данных строки
        public static string FullLoadedColumnName = "FullLoaded";

        // **********
        // Идея с DISTINCT на таблице БД претерпела неудачу - на тяжелых таблицах фактов (типа 28Н) 
        // любые запросы идут порядка 5 секунд. Это неприемлемо. Будем делать DISTINCT руками на клиенте
        // по имеющейся выборке. Прирос производительности - на порядок
        // **********
        /*
         internal static string LOOKUP_IDS_QUERY_TEMPLATE = 
            "Select distinct({0}) from {1} where ({2})";
        */

        // щаблон для запроса обязательных значений лукапа
        internal static string LOOKUP_QUERY_TEMPLATE = 
            "Select ID, {0} from {1} where (ID in ({2}))";

        internal static string LOOKUP_QUERY_TEMPLATE2 =
            "Select ID from {0} where (ID in ({1}))";

        // шаблон для запроса пустой таблицы данных лукапа
        internal static string LOOKUP_TABLE_STRUCT_QUERY_TEMPLATE = 
            "Select ID, {0}, 0 " + FullLoadedColumnName + " from {1} where (ID is null)";
        
        // шаблон для запроса одной записи лукапа
        private static string LOOKUP_TABLE_ONE_RECORD_TEMPLATE =
            "Select ID, {0}, {1} from {2} where (ID = {3})";

        #endregion

        #region Поля
        internal DataTable _loadedData = null;
        /// <summary>
        /// DataTable c загруженными данными объекта-лукапа
        /// </summary>
        public DataTable LoadedData
        {
            get { return _loadedData; }
            set { _loadedData = value; }
        }

        // родительская коллекция
        internal LookupManager _parentManager;

        private Dictionary<string, string> _mainFieldsNames = new Dictionary<string, string>();
        /// <summary>
        /// Коллекция "Ключ - Заголовок" для обязательных полей лукапа
        /// </summary>
        public Dictionary<string, string> MainFieldsNames
        {
            get { return _mainFieldsNames; }
        }

        private Dictionary<string, string> _additionalFieldsNames = new Dictionary<string, string>();
        /// <summary>
        /// Коллекция "Ключ - Заголовок" для дополнительных полей лукапа
        /// </summary>
        public Dictionary<string, string> AdditionalFieldNames
        {
            get { return _additionalFieldsNames; }
        }

        private string _lookupObjectName;
        /// <summary>
        /// Полное имя объекта-лукапа в терминах схемы
        /// </summary>
        public string LookupObjectName
        {
            get { return _lookupObjectName; }
        }

        private string _lookupObjectDBName;
        /// <summary>
        /// Имя таблицы БД где располагаются данных лукапа
        /// </summary>
        public string LookupObjectDBName
        {
            get { return _lookupObjectDBName; }
        }
        #endregion

        #region Конструкторы, деструкторы, очистка ресурсов
        /// <summary>
        /// Создание нового объекта-лукапа
        /// </summary>
        /// <param name="parentManager">Родительская коллекция</param>
        /// <param name="lookupObjectName">Полное имя объекта-лукапа в терминах схемы</param>
        /// <param name="lookupObjectDBName">Имя таблицы БД где располагаются данные лукапа</param>
        /// <param name="mainFieldsNames">Коллекция "Ключ - Заголовок" для обязательных полей лукапа</param>
        /// <param name="additionalFieldsNames">Коллекция "Ключ - Заголовок" для дополнительных полей лукапа</param>
        public LookupDataObject(LookupManager parentManager, string lookupObjectName, string lookupObjectDBName,
            Dictionary<string, string> mainFieldsNames, Dictionary<string, string> additionalFieldsNames)
        {
            _parentManager = parentManager;
            _lookupObjectName = lookupObjectName;
            _lookupObjectDBName = lookupObjectDBName;
            _mainFieldsNames = mainFieldsNames;
            _additionalFieldsNames = additionalFieldsNames;
        }

        /// <summary>
        /// Принудительное освобождение ресурсов
        /// </summary>
        /// <param name="disposing">true - метод вызван пользователем, false - сборщиком мусора</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        public void Clear()
        {
            if (_loadedData != null)
                _loadedData.Clear();
            _mainFieldsNames.Clear();
            _additionalFieldsNames.Clear();
            _columnsMapping.Clear();
        }
        #endregion

        #region Методы

        /// <summary>
        /// Преобразовать коллекцию "Ключ - Заголовок" к строке, содержащей ключи перечисленные через запятую.
        /// Используется для построения SQL-запросов
        /// </summary>
        /// <param name="fields">коллекция "Ключ - Заголовок"</param>
        /// <param name="sb">Внешний StringBuilder</param>
        private static void AppendFieldsToStringBuilder(Dictionary<string, string> fields, StringBuilder sb)
        {
            foreach (string field in fields.Keys)
            {
                sb.Append(field);
                sb.Append(", ");
            }
        }
        
        /// <summary>
        /// Тип полей для построения SQL-ограничения по ключу
        /// </summary>
        public enum FieldsKind {Main, Additional, Both};

        /// <summary>
        /// Построение списка полей (колонок) лукапа для использования в SQL-запросе
        /// </summary>
        /// <param name="fieldsKind">Необходимое множенство полей</param>
        /// <returns>Строка в формате "поле1, поле2 ... полеN"</returns>
        public string GetFieldsList(FieldsKind fieldsKind)
        {
            StringBuilder sb = new StringBuilder(1024);
            if ((fieldsKind == FieldsKind.Main) || (fieldsKind == FieldsKind.Both))
                AppendFieldsToStringBuilder(_mainFieldsNames, sb);
            if ((fieldsKind == FieldsKind.Additional) || (fieldsKind == FieldsKind.Both))
                AppendFieldsToStringBuilder(_additionalFieldsNames, sb);
            if (sb.Length > 0)
                sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        /// <summary>
        /// Получение заголовка поля (для построения полной разыменовки строки лукапа).
        /// В текущей реализации используется в хинтах.
        /// </summary>
        /// <param name="fieldName">Название поля</param>
        /// <returns>Заголовок поля</returns>
        private string GetFieldCaption(string fieldName)
        {
            string res = fieldName;
            if (_mainFieldsNames.ContainsKey(fieldName))
                res = _mainFieldsNames[fieldName];
            else if (_additionalFieldsNames.ContainsKey(fieldName))
                res = _additionalFieldsNames[fieldName];
            return res;
        }

        // Коллекция "Имя столбца - столбец" для оптимизированного доступа к данным объекта- лукапа
        private Dictionary<string, DataColumn> _columnsMapping = new Dictionary<string, DataColumn>();

        /// <summary>
        /// Построение соответствия "Имя столбца - столбец" для оптимизации доступа к данным лукапа
        /// </summary>
        public void BuildFieldsMapping()
        {
            _columnsMapping.Clear();
            foreach (string field in _mainFieldsNames.Keys)
                _columnsMapping.Add(field, _loadedData.Columns[field]);
            foreach (string field in _additionalFieldsNames.Keys)
                _columnsMapping.Add(field, _loadedData.Columns[field]);
        }
        
        /// <summary>
        /// Добавление в разыменовку значения одного поля
        /// </summary>
        /// <param name="row">Строка данных</param>
        /// <param name="needFoolValue">Режим разыменовки (true - полный, false - частичный)</param>
        /// <param name="sb">Внешний StringBuilder</param>
        /// <param name="columnName">название поля (колонки)</param>
        /// <param name="clmn">Поле (колонка) - для оптимизированного доступа</param>
        private void AppendValueToStringBuilder(DataRow row, bool needFoolValue, 
            StringBuilder sb, string columnName, DataColumn clmn)
        {
            if (needFoolValue)
                sb.Append(GetFieldCaption(columnName) + ": ");
            sb.Append(Convert.ToString(row[clmn]));
            if (!needFoolValue)
                sb.Append("; ");
            else
                sb.Append(Environment.NewLine);
        }

        /// <summary>
        /// Разыменовка значения лукапа
        /// </summary>
        /// <param name="row">Строка данных</param>
        /// <param name="needFoolValue">Режим разыменовки (полный/частичный)</param>
        /// <returns>Строка с разыменовкой</returns>
        private string DataRowToString(DataRow row, bool needFoolValue)
        {
            StringBuilder sb = new StringBuilder(256);
            // ID - если полная разыменовка (для хинта)
            if (needFoolValue)
                AppendValueToStringBuilder(row, needFoolValue, sb, "ID", _loadedData.Columns[0]);
            // обязательные поля
            foreach (string fieldName in _mainFieldsNames.Keys)
                AppendValueToStringBuilder(row, needFoolValue, sb, fieldName, _columnsMapping[fieldName]);
            // необязательныe поля
            if (needFoolValue)
            {
                foreach (string fieldName in _additionalFieldsNames.Keys)
                    AppendValueToStringBuilder(row, needFoolValue, sb, fieldName, _columnsMapping[fieldName]);
            }
            // удаляем последнюю точку с запятой
            if (sb.Length >= 2)
                sb.Remove(sb.Length - 2, 2);
            // если разыменовка для грида - пишем ID в скобках
            if (!needFoolValue)
            {
                sb.AppendFormat(" ({0})", row[_loadedData.Columns[0]]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Загрузка данных по одной строке лукапа из БД
        /// </summary>
        /// <param name="ID">ID строки</param>
        /// <returns>DataTable с данными</returns>
        private DataTable LoadOneLookup(int ID)
        {
            // формируем запрос
            string query = String.Format(LOOKUP_TABLE_ONE_RECORD_TEMPLATE,
                GetFieldsList(FieldsKind.Both), String.Concat(" 1 ", FullLoadedColumnName, " "),
                _lookupObjectDBName, ID);
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
				return (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);
            }
        }

        /// <summary>
        /// Получение данных по одной строке лукапа из внутреннего кэша
        /// </summary>
        /// <param name="ID">ID строки</param>
        /// <returns>Строка с данными или NULL если не загружена</returns>
        private DataRow InternalGetLookupData(int ID)
        {
            return _loadedData.Rows.Find(ID); 
        }

        /// <summary>
        /// Определение наличия данных по строке лукапа
        /// </summary>
        /// <param name="ID">ID строки</param>
        /// <returns>true - строка была загружена, false - нет</returns>
        public bool LookupDataPresent(int ID)
        {
            return InternalGetLookupData(ID) != null;
        }

        /// <summary>
        /// Получить разыменовку для строки лукапа
        /// </summary>
        /// <param name="ID">ID строки</param>
        /// <param name="needFoolValue">Режим разыменовки (true - полный, false - частичный)</param>
        /// <returns>Строка с разыменовкой</returns>
        public string GetLookupData(int ID, bool needFoolValue)
        {
            // загружены ли данные по этому ID ?
            DataRow row = InternalGetLookupData(ID);
            // если нет - загружаем полностью (все поля)
            if (row == null)
            {
                DataTable dt = LoadOneLookup(ID);
                // если данных нет - это вообще то ошибка, но ругаться не будем, т.к. некритично
                if ((dt == null) || (dt.Rows.Count == 0))
                {
                    // если данных нет, то ругаться не будем, но написать все таки стоит, что данные на нашлись
                    return string.Format("Некорректное значение. ({0})", ID);
                    //return ID.ToString();
                }
                // помещаем вновь загруженные значения в основную таблицу
                _loadedData.Merge(dt, true, MissingSchemaAction.Ignore);
                _loadedData.AcceptChanges();
                // возвращаем внось загруженное значение
                return DataRowToString(InternalGetLookupData(ID), needFoolValue);
            }
            // если да и запрошено полное значение - проверяем полноту данных
            if ((needFoolValue) && (Convert.ToInt32(row[FullLoadedColumnName]) == 0))
            {
                DataTable dt = LoadOneLookup(ID);
                // в этом случае если нет данных то ругаться уже надо
                if ((dt == null) || (dt.Rows.Count == 0))
                    throw new Exception(String.Format(
                        "Внутренняя ошибка: невозможно загрузить полные данные для разыменовки. Объект '{0}' ID = {1}", 
                        LookupObjectName, ID));
                row.BeginEdit();
                foreach (string field in _additionalFieldsNames.Keys)
                {
                    row[field] = dt.Rows[0][field];
                }
                row[FullLoadedColumnName] = 1;
                row.EndEdit();
                _loadedData.AcceptChanges();
            }
            return DataRowToString(row, needFoolValue);
           
        }

        #endregion
    }
    #endregion

    #region LookupManager
    public sealed class LookupManager: DisposableObject, ILookupManager
    {
        #region Конструкторы, деcтрукторы, очистка ресурсов

		private static LookupManager instance;

		public static LookupManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new LookupManager();
				}
				return instance;
			}
		}

		private LookupManager()
		{
		}

        /// <summary>
        /// Детерминированное освобождение ресурсов объекта
        /// </summary>
        /// <param name="disposing">true - метод вызван пользователей, false - сборщиком мусора</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        public void Clear()
        {
            foreach (LookupDataObject lkp in loadedLookups.Values)
            {
                lkp.Dispose();
            }
            loadedLookups.Clear();
        }

        #endregion

        #region Поля

        // Коллекция "Имя объекта лукапа - Объект лукап" содержащая загруженные лукапы
        private Dictionary<string, LookupDataObject> loadedLookups = new Dictionary<string, LookupDataObject>();

        #endregion

        #region Методы

        /// <summary>
        /// Инициализирован ли (загружен ли) объект-лукап
        /// </summary>
        /// <param name="objName">Полное имя объекта-лукапа в формате схемы</param>
        /// <returns></returns>
        private LookupDataObject CheckLookupObj(string objName)
        {
            // есть ли аттрибут в кэше?
            if (!loadedLookups.ContainsKey(objName.ToUpper()))
                throw new Exception(String.Format("Внутренняя ошибка: не инициализирован объект-лукап '{0}'", objName));

            return loadedLookups[objName.ToUpper()];
        }

        /// <summary>
        /// Получить разыменовку по одной строке объекта-лукапа
        /// </summary>
        /// <param name="objName">Полное имя объекта-лукапа в формате схемы</param>
        /// <param name="needFoolValue">Режим разыменовки (true - полная, false - частичная)</param>
        /// <param name="ID">ID строки</param>
        /// <returns>Строка с разыменовкой</returns>
        public string GetLookupValue(string objName, bool needFoolValue, int ID)
        {
            // инициализирован ли лукап?
            LookupDataObject lkp = CheckLookupObj(objName);
            return lkp.GetLookupData(ID, needFoolValue);
        }

        /// <summary>
        /// Проверка правильности значения объекта-лукапа
        /// </summary>
        /// <param name="objName">Полное имя объекта-лукапа в формате схемы</param>
        /// <param name="ID">Значение (ID)</param>
        /// <returns>true - значение верно, false - значение не верно</returns>
        public bool CheckLookupValue(string objName, int ID)
        {
            // инициализирован ли лукап?
            LookupDataObject lkp = CheckLookupObj(objName);
            // есть ли данные по лукапу в нашем кэше?
            if (lkp.LookupDataPresent(ID))
                return true;
            // есть ли данные в базе?
            using (IDatabase db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string query = String.Format("Select count(*) from {0} where ID = {1}", lkp.LookupObjectDBName, ID);
                int recCnt = (int)db.ExecQuery(query, QueryResultTypes.Scalar);
                return recCnt != 0;
            }
        }

        /// <summary>
        /// Инициализация кэша лукапов по текущему объекту схемы. Предыдущие загруженные лукапы при этом не выгружаются.
        /// </summary>
        /// <param name="obj">Объект схемы</param>
        /// <param name="sourceData">Загруженные данные про объекту схемы (ID лукапов берутся отсюда)</param>
        public void InitLookupsCash(IEntity obj, DataSet sourceData)
        {
            Debug.WriteLine(String.Empty);
            Debug.WriteLine(String.Format("Старт инициализации кэша лукапов для объекта '{0}'", obj.ObjectKey));
            Stopwatch mainSW = new Stopwatch();
            Stopwatch additionalSW = new Stopwatch();
            mainSW.Start();
            //string parentTableName = ent.FullDBName;
            IDatabase db = null;
            try
            {
                // обрабатываем по очереди все аттрибуты объекта схемы
				foreach (IDataAttribute attr in obj.Attributes.Values)
                {
                    string lookupObjectName = String.Empty;
                    string lookupObjectDBName = String.Empty;
                    Dictionary<string, string> mainFieldsNames = null;
                    Dictionary<string, string> additionalFieldsNames = null;

                    string attrName = attr.Name;
                    additionalSW.Reset();
                    additionalSW.Start();
                    Debug.WriteLine(String.Format("Получение параметров аттрибута {0}", attrName));
                    // если аттрибут - лукап и получилось получить все нужны параметры - начинаем его инициализацию
					bool isLookup = BaseClsUI.GetLookupParams(obj, attr, ref lookupObjectName, ref lookupObjectDBName, 
                        ref mainFieldsNames, ref additionalFieldsNames);
                    additionalSW.Stop();
                    Debug.WriteLine(String.Format("Затрачено: {0} мс", additionalSW.ElapsedMilliseconds));

                    if (!isLookup)
                        continue;

                    #warning Проверка на существование лукапа

                    if (db == null)
                        db = WorkplaceSingleton.Workplace.ActiveScheme.SchemeDWH.DB;

                    LookupDataObject lkp;
                    bool isNewLookup = true;
                    // был ли такой лукап загружен?
                    if (loadedLookups.ContainsKey(lookupObjectName.ToUpper()))
                    {
                        lkp = loadedLookups[lookupObjectName.ToUpper()];
                        isNewLookup = false;
                        Debug.WriteLine("Используется существующий объект");
                    }
                    else
                    {
                        // пытаемся инициализировать поля лукапа по объекту схемы
                        lkp = new LookupDataObject(this, lookupObjectName, lookupObjectDBName,
                            mainFieldsNames, additionalFieldsNames);
                        Debug.WriteLine("Создан новый объект");
                    }

                    // значения обязательных полей лукапа
                    // .. ограничение на ID
                    Debug.WriteLine("Построение ограничения на используемые ID лукапа");
                    DataTable lkpMainColumnValues = null;
                    // получаем ID лукапа, содержщиеся в исходной выборке 
                    List<string> lkpIDs = DataTableHelper.GetDistinctFieldValuesConstraints(sourceData.Tables[0], attr.Name);
                    if (lkpIDs.Count > 0)
                    {
                        Debug.WriteLine(String.Format("Сформировано {0} ограничений", lkpIDs.Count));
                        foreach (string constr in lkpIDs)
                        {
                            additionalSW.Reset();
                            additionalSW.Start();
                            string fieldsList = lkp.GetFieldsList(LookupDataObject.FieldsKind.Main);
                            string lkpQuery = string.Empty;
                            // если список ID получен - формируем и исполняем запрос для получения обязательных полей лукапа
                            if (!string.IsNullOrEmpty(fieldsList))
                                lkpQuery = String.Format(LookupDataObject.LOOKUP_QUERY_TEMPLATE,
                                fieldsList, lkp.LookupObjectDBName, constr);
                            else
                                lkpQuery = String.Format(LookupDataObject.LOOKUP_QUERY_TEMPLATE2, 
                                    lkp.LookupObjectDBName, constr);
                                
                            DataTable curValues = (DataTable)db.ExecQuery(lkpQuery, QueryResultTypes.DataTable);
                            if (lkpMainColumnValues == null)
                                lkpMainColumnValues = curValues.Clone();
                            lkpMainColumnValues.Merge(curValues);
                            additionalSW.Stop();
                            Debug.WriteLine(lkpQuery);
                            Debug.WriteLine(String.Format("Выбрано {0} записей. Затрачено {1} мс", lkpMainColumnValues.Rows.Count, additionalSW.ElapsedMilliseconds));
                        }
                    }
                    else
                    {
                        additionalSW.Stop();
                        Debug.WriteLine("Текущая выборка не содержит данных для лукапа");
                    }

                   if (isNewLookup)
                   {
                       // получаем пустую таблицу с сформированной структурой полей лукапа
                       Debug.WriteLine("Построение таблицы данных лукапа");
                       additionalSW.Reset();
                       additionalSW.Start();
                       string templateQuery = String.Format(LookupDataObject.LOOKUP_TABLE_STRUCT_QUERY_TEMPLATE,
                        lkp.GetFieldsList(LookupDataObject.FieldsKind.Both), lkp.LookupObjectDBName);
                       lkp.LoadedData = (DataTable)db.ExecQuery(templateQuery, QueryResultTypes.DataTable);
                   }

                   // копируем значимые поля во временную таблицу
                   lkp.LoadedData.BeginLoadData();
                   if (isNewLookup)
                   {
                       lkp.LoadedData.PrimaryKey = new DataColumn[] { lkp.LoadedData.Columns[0] };
                       // делаем все поля nullable
                       foreach (string mainFieldName in lkp.MainFieldsNames.Keys)
                           lkp.LoadedData.Columns[mainFieldName].AllowDBNull = true;
                       foreach (string additionalFieldName in lkp.AdditionalFieldNames.Keys)
                           lkp.LoadedData.Columns[additionalFieldName].AllowDBNull = true;
                       //колонку хранящую признак загруженности делаем редактируемой
                       lkp.LoadedData.Columns[LookupDataObject.FullLoadedColumnName].ReadOnly = false;
                   }
                   // переносим данные по основным полям
                   if (lkpMainColumnValues != null)
                   {
                       foreach (DataRow row in lkpMainColumnValues.Rows)
                       {
                           int newID = Convert.ToInt32(row[0]);
                           // если данные по такому ID уже есть - ничего не делаем
                           if ((!isNewLookup) && (lkp.LookupDataPresent(newID)))
                               continue;
                           // иначе - создаем новую запись
                           DataRow newLkpRow = lkp.LoadedData.NewRow();
                           newLkpRow.BeginEdit();
                           // ID
                           newLkpRow[0] = row[0];
                           // копируем значения обязательных полей
                           foreach (string mainFieldName in lkp.MainFieldsNames.Keys)
                               newLkpRow[mainFieldName] = row[mainFieldName];
                           newLkpRow["FullLoaded"] = lkp.AdditionalFieldNames.Count == 0 ? 1 : 0;
                           newLkpRow.EndEdit();
                           lkp.LoadedData.Rows.Add(newLkpRow);
                       }
                   }
                   lkp.LoadedData.EndLoadData();
                   // строим структуру для оптимизированного доступа к данным по имени колонки
                   lkp.BuildFieldsMapping();
                   additionalSW.Stop();
                   Debug.WriteLine(String.Format("Затрачено {0} мс", additionalSW.ElapsedMilliseconds));
                   // если создан новый лукап - добавляем его в коллекцию загруженных 
                   if (isNewLookup)
                       loadedLookups.Add(lkp.LookupObjectName.ToUpper(), lkp);
                }
            }
            finally
            {
                if (db != null)
                    db.Dispose();
                mainSW.Stop();
                Debug.WriteLine(String.Format("Инициализация кэша лукапов завершена. Затрачено {0} мс.", mainSW.ElapsedMilliseconds));
            }
        }
        #endregion
    }
    #endregion

}