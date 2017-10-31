using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.WriteBack
{
    internal class RequestDataProcess
    {
        // Запрос
        private RequestData request;

        #region Зависимости

        private readonly ISchemeService schemeService;
        
        #endregion
        
        /// <summary>
        /// Список классификаторов данных связанных с таблицей фактов, [Алиас, IEntity]
        /// </summary>
        private Dictionary<string, IEntity> classifiersInRequest = new Dictionary<string, IEntity>();
        /// <summary>
        /// Список классификаторов данных отсутствующих в запросе, <FullName, IEntity> 
        /// </summary>
        //private Dictionary<string, IEntity> missingClassifiers = new Dictionary<string, IEntity>();
        /// <summary>
        /// Список ассоциаций отсутствующих в запросе, [FullName, IEntityAssociation] 
        /// </summary>
        private Dictionary<string, IEntityAssociation> missingAssociations = new Dictionary<string, IEntityAssociation>();
        /// <summary>
        /// Сописок соответствий сопоставимых с недостающими классификаторами, [FullName, IEntity]  
        /// </summary>
        private Dictionary<string, IEntity> bridge2MissingClassifier = new Dictionary<string, IEntity>();
        /// <summary>
        /// Список ассоциаций отсутствующих в запросе, у которых есть значение по умолчанию, [FullName, IEntityAssociation]
        /// </summary>
        private Dictionary<string, IEntityAssociation> missingDefaultAssociations = new Dictionary<string, IEntityAssociation>();
        /// <summary>
        /// Список соответствия недостающих классификаторов 
        /// и источников данных по которых необходимо в них записывать данные
        /// </summary>
        private Dictionary<string, int> missingClassifiers2SourceID = new Dictionary<string, int>();

        private List<string> factForProcess = new List<string>();

        private string dataSourceAlias = String.Empty;
        // Таблица фактов в которую производится запись данных
        private IFactTable fact;
        // Список мер в таблице фактов
        private Dictionary<string, IDataAttribute> measures = new Dictionary<string, IDataAttribute>();
        // Список ссылок в таблице фактов
        private Dictionary<int, string> references = new Dictionary<int, string>();
        // Данные для записи
        private DataTable dataTable;
        private string searchQueryWithoutTask = String.Empty;
        private string insertQuery = String.Empty;
        private string updateQuery = String.Empty;

        private int sourceID = -1;
        private int taskID = -1;

        /// <summary>
        /// Идентификатор текущего обрабатываемого запроса
        /// </summary>
        private int currentRequestID;

        /// <summary>
        /// Хитрая связь. Для классификатора отсутствующего в запросе [Key] находится связь [Value] 
        /// через другой классивикатор таблицы фактов, который имеет ссылку на отсутствующий классификатор.
        /// Value = первый элемент - ссылка на классификатор из таблицы фактов, 
        /// второй элемент - ссылка на отсутствующий классификатор из найденного классификатора.
        /// </summary>
        private Dictionary<IEntityAssociation, IEntityAssociation[]> calculatedBindedReferences = new Dictionary<IEntityAssociation, IEntityAssociation[]>();

        private const string IncorrectMeasureValueType =
            "Поддерживается запись только целочисленных, вещественных или логических показателей. " +
            "Тип показателя '{0}' не соответствует требуемому.";

        public RequestDataProcess(RequestData request, ISchemeService schemeService)
        {
            this.request = request;
            this.schemeService = schemeService;
        }

        /// <summary>
        /// Получает идентификатор задачи в контексте которой будет выполняться запрос.
        /// Дополнительно проверяет существование задачи
        /// </summary>
        private void GetTaskID(IDatabase db)
        {
            // Всегда должно присутствовать измерение источников
            XmlNode xmlTaskID = request.Data.SelectSingleNode(String.Format("/Requests/@taskID"));
            if (xmlTaskID == null)
                throw new Exception("Не указан идентификатор задачи, запись результатов расчётов вне контекста задачи невозможна. Необходимо прикрепить лист планирования к задаче.");

            try
            {
                taskID = Convert.ToInt32(xmlTaskID.Value);
            }
            catch
            {
                throw new InvalidCastException(String.Format(
                    "Неверный формат идентификатора задачи: taskID={0}. Обратитесь к разработчикам.", 
                    xmlTaskID.Value));
            }

            if (Authentication.UserID != null)
                schemeService.CheckTask(taskID);
            else
                CheckTaskOld(db);
        }

        /// <summary>
        /// Проверка существования задачи, в контексте которой производится запись.
        /// </summary>
        private void CheckTaskOld(IDatabase db)
        {
            if (0 == Convert.ToInt32(db.ExecQuery("select count(*) from Tasks where ID = " + taskID, QueryResultTypes.Scalar)))
            {
                throw new Exception(String.Format("Лист планирования прикреплён к несуществующей задаче (TaskID = {0}). Возможно задача находится в состоянии создания и еще не сохранена или указан неверный адрес веб-сервиса или имя схемы.", taskID));
            }
        }

        /// <summary>
        /// Разбор определения запроса.
        /// </summary>
        private void ParseDeclaration(int partID)
        {
            // Всегда должно присутствовать измерение источников
            XmlNode xmlDataSourceAlias = request.Data.SelectSingleNode(String.Format("/Requests/Request[@id = {0}]/Schema/AttributeType[@type = 'member' and @fullname = 'fx.System.DataSources']", partID));
            if (xmlDataSourceAlias == null)
                throw new Exception("Отсутствует определение измерения источников. Возможно не добавлен общий или частный фильтр по источникам для результата расчёта.");
            else
                dataSourceAlias = xmlDataSourceAlias.Attributes["name"].Value;

            // получаем показатели
            foreach (XmlNode xmlAxis in request.Data.SelectNodes(String.Format("/Requests/Request[@id = {0}]/Schema/AttributeType[@type = 'total']", partID)))
            {
                // Проверим есть ли такая таблица фактов в системе
                if (String.IsNullOrEmpty(xmlAxis.Attributes["fullname"].Value))
                    throw new Exception("У показателя не указано значение fullname.");

                string factTableFullName = xmlAxis.Attributes["fullname"].Value;
                fact = null;
                foreach (IFactTable factTableItem in schemeService.GetFactTables())
                {
                    if (factTableItem.FullName == factTableFullName)
                    {
                        fact = factTableItem;
                        break;
                    }
                }

                if (fact == null)
                    throw new Exception(String.Format("У показателя указано имя несуществующего объекта {0}.", xmlAxis.Attributes["fullname"].Value));

                if (!factForProcess.Contains(fact.FullName))
                {
                    factForProcess.Add(fact.FullName);
                }

                if (fact.SubClassType != SubClassTypes.Input && fact.SubClassType != SubClassTypes.PumpInput)
                    throw new Exception(String.Format("Запись данных в куб \"{0}\" невозможна, т.к. он не предназначен для ввода данных.", fact.OlapName));

                // Проверим наличие русского наименования
                if (xmlAxis.Attributes["totalName"] == null)
                    throw new Exception("У показателя не указано русское имя.");
                if (xmlAxis.Attributes["totalName"].Value == String.Empty)
                    throw new Exception("У показателя не указано имя.");

                // Находим атрибут по русскому наименованию
                foreach (IDataAttribute item in fact.Attributes.Values)
                {
                    if (item.Caption == xmlAxis.Attributes["totalName"].Value)
                    {
                        measures.Add(xmlAxis.Attributes["name"].Value, item);
                        break;
                    }
                }

                if (measures.Count == 0)
                    throw new Exception(String.Format("Показатель \"{0}\" не найден в таблице фактов \"{1}\"", xmlAxis.Attributes["totalName"].Value, fact.OlapName));
            }

            // получаем все измерения (ссылки)
            foreach (XmlNode xmlAxis in request.Data.SelectNodes(String.Format("/Requests/Request[@id = {0}]/Schema/AttributeType[@type = 'member' and @fullname != 'fx.System.DataSources']", partID)))
            {
                // Проверим есть ли такой классификатор в системе

                string axisFullName = xmlAxis.Attributes["fullname"].Value;
                string clsFullName = axisFullName;

                string axisAliasName = xmlAxis.Attributes["name"].Value;

                #region Заглушка
                string[] parts = axisFullName.Split(new char[] { ';' });
                if (parts.GetLength(0) > 1)
                {
                    clsFullName = parts[0];
                    for (int i = 1; i < parts.GetLength(0); i++)
                    {
						if (!schemeService.GetAllAssociations().ContainsKey(parts[i]))
							throw new Exception(String.Format("У оси указано имя несуществующего объекта \"{0}\"", parts[i]));

                        axisAliasName = String.Format("{0};{1}", axisAliasName, parts[i]);
                    }
                }
                #endregion Заглушка

                IClassifier classifier = schemeService.GetSchemeClassifierByFullName(clsFullName);
                if (classifier == null)
                    throw new Exception(String.Format("У оси указано имя несуществующего объекта \"{0}\"", axisFullName));

                // Ругаемся на наличие классификаторов, на которые нет ссылки из таблицы фактов
                if (!schemeService.ContainClassifierInFact(fact, classifier.ObjectKey))
                {
                    throw new Exception(String.Format(
                        "При наличии на листе измерения \"{0}\" обратная запись не возможна. Удалите измерение \"{0}\" с листа и повторите запись данных.",
                        schemeService.GetSchemeClassifierByFullName(clsFullName).OlapName));
                }

                classifiersInRequest.Add(axisAliasName, classifier);
            }
        }

        private static bool ContainObject(IDictionary<string, IEntity> classifiers, string fullName)
        {
            foreach (IClassifier item in classifiers.Values)
                if (item.FullName == fullName)
                    return true;
            return false;
        }

        private static IClassifier GetObject(IDictionary<string, IEntity> classifiers, string fullName)
        {
            foreach (IClassifier item in classifiers.Values)
                if (item.FullName == fullName)
                    return item;
            throw new Exception("Ключ не найден в коллекции.");
        }

        /// <summary>
        /// Находит необходимы для записи недостающие классификаторы данных, 
        /// которые не определены в запросе и помещает из в коллекцию missingAssociations.
        /// </summary>
        private void GetMissingClassifiers()
        {
            foreach (IEntityAssociation association in fact.Associations.Values)
            {
                if (!ContainObject(classifiersInRequest, association.RoleBridge.FullName))
                {
                    if (association.RoleBridge.ClassType == ClassTypes.clsDataClassifier ||
                        association.RoleBridge.ClassType == ClassTypes.clsFixedClassifier)
                    {
                        missingAssociations.Add(association.RoleBridge.FullName, association);
                    }
                    else
                        throw new Exception(String.Format("В таблице фактов определено неизвестное измерение '{0}' ({1})", association.RoleBridge.OlapName, association.RoleBridge.FullName));
                }
            }
        }
        
        /// <summary>
        /// Находим для пропущенных классификаторов соответствующие сопоставимые.
        /// </summary>
        private void FindAccordBridgeForMissingClassifiers()
        {
            // Находим для пропущенных классификаторов соответствующие сопоставимые.
            foreach (IEntityAssociation missingAssociation in missingAssociations.Values)
            {
                IEntity missingClassifier = missingAssociation.RoleBridge;
                foreach (IEntityAssociation association in missingClassifier.Associations.Values)
                {
                    // Если есть в запросе такой сопоставимый то используем его
                    if (ContainObject(classifiersInRequest, association.RoleBridge.FullName))
                        if (GetObject(classifiersInRequest, association.RoleBridge.FullName).ClassType == ClassTypes.clsBridgeClassifier)
                        {
                            #region Проверка на наличие двух сопоставимых для одного пропущенного классификатора
                            foreach (KeyValuePair<string, IEntity> item in bridge2MissingClassifier)
                            {
                                if (item.Value.ObjectKey == missingClassifier.ObjectKey)
                                {
                                    string concurentBridgeName = String.Empty;
                                    foreach (IAssociation itemAssociation in missingClassifier.Associations.Values)
                                    {
                                        if (itemAssociation.RoleBridge.FullName == item.Key)
                                        {
                                            concurentBridgeName = itemAssociation.RoleBridge.OlapName;
                                            break;
                                        }
                                    }
                                    throw new ServerException(String.Format(
                                        "Обратная запись не возможна, т.к. на листе размещено два сопоставимых классификатора \"{0}\" и \"{1}\" для неуказанного классификатора данных \"{2}\" ({3})",
                                        association.RoleBridge.OlapName, concurentBridgeName,
                                        missingClassifier.OlapName, missingClassifier.FullName));
                                }
                            }
                            #endregion Проверка на наличие двух сопоставимых для одного пропущенного классификатора

                            bridge2MissingClassifier.Add(association.RoleBridge.FullName, missingClassifier);
                        }
                }
            }
        }

        /// <summary>
        /// Находим для отсутствующих классификаторов ассоциации со значением по умолчанию.
        /// </summary>
        private void FindDefaultAssociationsForMissingClassifiers()
        {
            // Если у пропущенной ассоциации есть значение по умолчанию, то добавляем ее
            foreach (IEntityAssociation missingAssociation in missingAssociations.Values)
            {
                if (!bridge2MissingClassifier.ContainsValue(missingAssociation.RoleBridge))
                {
                    if (!String.IsNullOrEmpty(Convert.ToString(missingAssociation.RoleDataAttribute.DefaultValue)))
                    {
                        missingDefaultAssociations.Add(missingAssociation.RoleBridge.FullName, missingAssociation);
                    }
                }
            }
            // Удаляем пропущенные классификаторы, для которых есть ассоциации со значением по умолчанию
            foreach (KeyValuePair<string, IEntityAssociation> pair in missingDefaultAssociations)
            {
                missingAssociations.Remove(pair.Key);
            }
        }

        /// <summary>
        /// Для не указанных классификаторов таблицы фактов находит классификаторы, 
        /// через которые можно проставить ссылки на неуказанные.
        /// Ищем только в ассоциациях со значением по умолчанию.
        /// </summary>
        /// <remarks>
        /// Пример: 
        /// Есть таблица фактов (Ф) и два классификатора данных (А и Б). 
        /// Есть следующие ассоциации: Ф -> А, Ф -> Б, А -> Б.
        /// Классификатор (А) выведен на лист или для него есть выведенный сопоставимый, а (Б) не выведен.
        /// Необходимо при записе в (Ф) проставить ссылки на (Б) используя связку (А -> Б).
        /// </remarks>
        private void FindAccodsClsForMissingClassifiers()
        {
            // Ищем невыведенный классификатор (Б)
            foreach (IEntityAssociation association in missingDefaultAssociations.Values)
            {
                IEntity missingEntity = association.RoleBridge;
                // (Б) должен быть классификатором данных и для него не должно быть сопостовимого, выведенного на лист 
                if (missingEntity.ClassType == ClassTypes.clsDataClassifier && !bridge2MissingClassifier.ContainsValue(missingEntity))
                {
                    // Ищем в (Ф) классификатор (А) ссылающийся на (Б)
                    foreach (IEntityAssociation item in fact.Associations.Values)
                    {
                        IEntity entityA = item.RoleBridge;
                        if (entityA.ClassType == ClassTypes.clsDataClassifier)
                        {
                            // Классификатор (А) должен быть выведен на лист 
                            // либо для него должен быть сопостовимый, вынесенный на лист
                            if (ContainObject(classifiersInRequest, entityA.FullName) ||
                                bridge2MissingClassifier.ContainsValue(entityA))
                            {
                                IEntityAssociation bindingAssociation = null;
                                    // Ищем в классификаторе (А) ссылку на классификатор (Б)
                                    foreach (IEntityAssociation associationItem in entityA.Associations.Values)
                                {
                                    if (associationItem.RoleBridge.FullName == missingEntity.FullName)
                                        bindingAssociation = associationItem;
                                }
                                if (bindingAssociation != null)
                                {
                                    calculatedBindedReferences.Add(
                                        association,
                                        new IEntityAssociation[2] { item, bindingAssociation });
                                }
                            }
                        }
                    }
                }
            }
            foreach (IEntityAssociation calculatedReference in calculatedBindedReferences.Keys)
            {
                missingDefaultAssociations.Remove(calculatedReference.RoleBridge.FullName);
            }
        }

        /// <summary>
        /// Проверка на наличие всех необходимых классификаторов для обратной записи.
        /// Если чего-либо не будет хватать, то будет выдано исключение.
        /// </summary>
        private void CheckMissingClassifiers()
        {
            if (missingAssociations.Count <= bridge2MissingClassifier.Count)
                return;

            // Обрабатываем ненайденные соответствия и ругаемся
            List<string> errorMessages = new List<string>();
            foreach (IEntityAssociation missingAssociation in missingAssociations.Values)
            {
                IEntity missingClassifier = missingAssociation.RoleBridge;
                bool find = false;

                foreach (IClassifier dataCls in bridge2MissingClassifier.Values)
                {
                    if (missingClassifier == dataCls)
                    {
                        find = true;
                        break;
                    }
                }

                if (!find)
                    errorMessages.Add(String.Format("'{0}' ({1})", missingClassifier.OlapName, missingClassifier.FullName)); 
            }

            // TODO: если на листе разложено несколько сопоставимых, связанных с отсутствующим классификатором, то членораздельно ругаемся
            //if (errorMessages.Count == 0)
            // ...

            throw new Exception(String.Format("Для записи результатов необходимо поместить на лист следующие измерения или соответствующие им измерения сопоставимых классификаторов: {0}", 
                                              String.Join(", ", errorMessages.ToArray())));
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetDataSourcesForMissingDataCls(IDatabase db)
        {
            foreach (IEntityAssociation missingAssociation in missingAssociations.Values)
            {
                IClassifier cls = (IClassifier)missingAssociation.RoleBridge;
                if (cls.IsDivided)
                {
                    string query = String.Format(
                        "select distinct D.SourceID as SourceID, S.SupplierCode as SupplierCode, S.KindsOfParams as KindsOfParams, S.Year as Year, " +
                        "'N' {0} S.Name {0} " +
                        "'Y' {0} cast(S.Year as varchar(4)) {0} " +
                        "'M' {0} cast(S.Month as varchar(2)) {0} " +
                        "'V' {0} S.Variant {0} " +
                        "'Q' {0} cast(S.Quarter as varchar(1)) {0} " +
                        "'T' {0} S.Territory as Parameter " +
                        "from {1} D join DataSources S on (D.SourceID = S.ID) where D.RowType = 0",
                        WriteBackServerClass.ConcatenateChar, cls.FullDBName);

                    DataTable dt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable);

                    // a. Точно тот же источник, в котором пришли данные
                    DataRow[] rows = dt.Select(String.Format("SourceID = {0}", sourceID));
                    if (rows.GetLength(0) > 0)
                    {
                        missingClassifiers2SourceID.Add(cls.FullName, Convert.ToInt32(rows[0]["SourceID"]));
                        continue;
                    }

                    // b. Поставщик = "ФО" и те же самые параметры, что и у источника, в котором пришли данные
                    query = String.Format("select SupplierCode, KindsOfParams, Year, " +
                        "'N' {0} Name {0} " +
                        "'Y' {0} cast(Year as varchar(4)) {0} " +
                        "'M' {0} cast(Month as varchar(2)) {0} " +
                        "'V' {0} Variant {0} " +
                        "'Q' {0} cast(Quarter as varchar(1)) {0} " +
                        "'T' {0} Territory as Parameter " +
                        "from DataSources where ID = ?", WriteBackServerClass.ConcatenateChar);
                    DataTable sdt = (DataTable)db.ExecQuery(query, QueryResultTypes.DataTable,
                        db.CreateParameter("ID", sourceID));

                    if (sdt.Rows.Count == 0)
                        throw new Exception(String.Format("Источник данных ({0}) не найден.", sourceID));

                    rows = dt.Select(String.Format("SupplierCode = 'ФО' and KindsOfParams = {0} and Parameter = '{1}'", sdt.Rows[0]["KindsOfParams"], sdt.Rows[0]["Parameter"]));
                    if (rows.GetLength(0) > 0)
                    {
                        missingClassifiers2SourceID.Add(cls.FullName, Convert.ToInt32(rows[0]["SourceID"]));
                        continue;
                    }

                    // c. Те же самые параметры, что и у источника, в котором пришли данные
                    if (rows.GetLength(0) == 1)
                    {
                        missingClassifiers2SourceID.Add(cls.FullName, Convert.ToInt32(rows[0]["SourceID"]));
                        continue;
                    }
                    else if (rows.GetLength(0) > 1)
                        throw new Exception(String.Format("В классификаторе данных {0} найдено несколько источников с одинаковыми параметрами", cls.FullName));

                    // d. Тот же год параметра, что и у источника, в котором пришли данные
                    rows = dt.Select(String.Format("Year = {0}", sdt.Rows[0]["Year"]));
                    if (rows.GetLength(0) == 0)
                    {

                        throw new Exception(String.Format(
                            "В классификаторе данных {0}({1}) не найден подходящий источник данных.\n" +
                            "Доп.инф.: обратная запись производится по источнику (ID={2})",
                            cls.OlapName, cls.FullName, sourceID));
                    }
                    else if (rows.GetLength(0) == 1)
                    {
                        missingClassifiers2SourceID.Add(cls.FullName, Convert.ToInt32(rows[0]["SourceID"]));
                        continue;
                    }
                    else if (rows.GetLength(0) > 1)
                        throw new Exception(String.Format("В классификаторе данных {0} найдено несколько источников поставщика ФО с одинаковыми параметрами по {1} году.", cls.FullName, sdt.Rows[0]["Year"]));
                }
                else
                {
                    missingClassifiers2SourceID.Add(cls.FullName, -1);
                }
            }
        }

        /// <summary>
        /// Возвращает по ID записи сопоставимого ID записи в классификаторе данных.
        /// </summary>
        /// <param name="bridgeCls">Сопоставимый классификатор</param>
        /// <param name="dataCls">Классификатор данных</param>
        /// <param name="rowSourceID">ID источника в котором идет запись</param>
        /// <param name="bridgeRowID">ID запси в сопоставимом</param>
        /// <returns>ID запси в классификаторе данных</returns>
        private int FindRowIdInDataClsByBridgeCls(IDatabase db, IEntity bridgeCls, IEntity dataCls, int rowSourceID, int bridgeRowID)
        {
            string refName = String.Empty;
            // Находим в классификаторе данных наименование поля-ссылки на сопоставимый 
            foreach (IAssociation association in dataCls.Associations.Values)
            {
                if (association.RoleBridge.FullName == bridgeCls.FullName)
                {
                    refName = association.FullDBName;
                    break;
                }
            }
            if (refName == String.Empty)
                throw new Exception(String.Format("Не найдена ассоциация сопоставления классификатора данных '{0}' и сопоставимого '{1}'. Запись данных в разрезе сопоставимого классификатора '{1}' невозможна.", dataCls.OlapName, bridgeCls.OlapName));

            List<IDbDataParameter> parameters = new List<IDbDataParameter>();
            parameters.Add(db.CreateParameter("RefBridge", bridgeRowID));

            string searchQuery = String.Format("select ID from {0} where {1} = ? and RowType = 0",
                dataCls.FullDBName, refName);

            string msgBySourceID = String.Empty;

            // Если делимся по источникам, то добавляем фильтрацию по SourceID
            if (((IDataSourceDividedClass)dataCls).IsDivided)
            {
                parameters.Add(db.CreateParameter("SourceID", rowSourceID));
                searchQuery = String.Format("{0} and SourceID = ?", searchQuery);
                msgBySourceID = "по источнику ID={2}";
            }

            DataTable dt = (DataTable)db.ExecQuery(searchQuery, QueryResultTypes.DataTable, parameters.ToArray());

            if (dt.Rows.Count == 0)
            {
                throw new Exception(String.Format(
                    "Запись данных в разрезе сопоставимого классификатора '{3}'({4}) невозможна. " +
                    "Попробуйте сопоставить классификатор данных '{0}' с сопоставимым классификатором '{3}'. " +
                    "В классификаторе данных '{0}'({1}) " + msgBySourceID + " не найдено ни одной записи сопоставленных записи сопоставимого классификатора '{3}'({4}) (PKID={5}).",
                    dataCls.OlapName, dataCls.FullName, rowSourceID, bridgeCls.OlapName, bridgeCls.FullName, bridgeRowID));
            }
            else if (dt.Rows.Count == 1)
                return Convert.ToInt32(dt.Rows[0][0]);
            else
                throw new Exception(String.Format(
                    "Запись данных в разрезе сопоставимого классификатора '{4}'({5}) невозможна. " +
                    "В классификаторе данных '{0}'({1}) " + msgBySourceID + " найдено несколько записей ({3}) сопоставленных записи сопоставимого классификатора '{4}'({5}) (PKID={6}).",
                    dataCls.OlapName, dataCls.FullName, rowSourceID, dt.Rows.Count, bridgeCls.OlapName, bridgeCls.FullName, bridgeRowID));
        }

        /// <summary>
        /// Поиск ссылки на отсутствующий классификатор через промежуточный классификатор.
        /// </summary>
        private int FindRowIdInDataClsByBindedCls(IDatabase db, int bindedId, IEntity bindedCld, IEntityAssociation bindingAssociation, int defaultValue)
        {
            List<IDbDataParameter> parameters = new List<IDbDataParameter>();
            parameters.Add(db.CreateParameter("Id", bindedId));

            string searchQuery = String.Format("select {0} from {1} where Id = ? and RowType = 0",
                bindingAssociation.FullDBName, bindedCld.FullDBName);

            DataTable dt = (DataTable)db.ExecQuery(searchQuery, QueryResultTypes.DataTable, parameters.ToArray());

            if (dt.Rows.Count == 0)
            {
                return defaultValue;
            }
            return Convert.ToInt32(dt.Rows[0][0]);
        }

        /// <summary>
        /// Формирует структуру таблицы и запросы изменения данных.
        /// </summary>
        private DataTable CreateDataTableAndUpdateQueries()
        {
            DataTable dt = new DataTable(fact.FullDBName);
            List<string> searchConditions = new List<string>();
            List<string> attributeNames = new List<string>();
            List<string> attributeValues = new List<string>();
            List<string> attributeUpdates = new List<string>();

            // Добавляем ссылку на фиксированное измерение источников и поле для идентификатора задачи
            dt.Columns.Add("SourceID", Type.GetType("System.Int32"));
            dt.Columns.Add("TaskID", Type.GetType("System.Int32"));
            searchConditions.Add("SourceID = ?");
            searchConditions.Add("TaskID = ?");
            attributeNames.Add("SourceID");
            attributeValues.Add("?");
            attributeNames.Add("TaskID");
            attributeValues.Add("?");
            attributeUpdates.Add("SourceID = ?");
            attributeUpdates.Add("TaskID = ?");

            // Определим, есть ли в кубе изменения, построенные на одной таблице
            bool checkDoubleDimensions = false;
            foreach (IEntityAssociation outer in fact.Associations.Values)
            {
                foreach (IEntityAssociation inner in fact.Associations.Values)
                {
                    if (outer.ObjectKey != inner.ObjectKey)
                    {
                        checkDoubleDimensions = outer.RoleBridge.ObjectKey == inner.RoleBridge.ObjectKey;
						if (checkDoubleDimensions)
							break;
                    }
                }
				
				if (checkDoubleDimensions)
					break;
            }

            #region Добавляем ссылки на измерения

            foreach (KeyValuePair<string, IEntity> clsItem in classifiersInRequest)
            {
                IEntity cls = clsItem.Value;
                // Если это сопоставимый, то заменяем его на соответствующий классификатор данных
                if (cls.ClassType == ClassTypes.clsBridgeClassifier)
                {
                    if (!bridge2MissingClassifier.ContainsKey(cls.FullName))
                        throw new Exception(String.Format("Обратная запись в куб \"{0}\" в разрезе измерения \"{1}\" невозможна.", fact.OlapName, cls.OlapName));
                    cls = bridge2MissingClassifier[cls.FullName];
                }

                foreach (IEntityAssociation associationItem in fact.Associations.Values)
                {
                    #region Заплатка

                    string[] aliasParts = clsItem.Key.Split(new char[] { ';' });
                    string finedAliasName = aliasParts[0];
                    bool associationFound = true;
                    if (checkDoubleDimensions && aliasParts.GetLength(0) > 1)
                    {
                        associationFound = false;
                        for (int i = 1; i < aliasParts.GetLength(0); i++)
                        {
                            if (aliasParts[i] == associationItem.FullName ||
								aliasParts[i] == associationItem.ObjectKey)
                            {
                                finedAliasName = clsItem.Key;
                                associationFound = true;
                            }
                        }
                    }

                    #endregion Заплатка

                    if (associationItem.RoleBridge.FullName == cls.FullName && associationFound)
                    {
                        int alias = -1;
                        foreach (KeyValuePair<string, IEntity> fItem in classifiersInRequest)
                        {
                            if (fItem.Value == clsItem.Value && fItem.Key.Split(new char[] { ';' })[0] == finedAliasName.Split(new char[] { ';' })[0])
                            {
                                string aliasName = fItem.Key.Remove(0, 1);
                                string[] parts = aliasName.Split(new char[] { ';' });
                                alias = Convert.ToInt32(parts[0]);
                                break;
                            }
                        }
                        if (alias == -1)
                            throw new Exception("Inner BUG: Не найден алиаc.");

                        references.Add(alias, associationItem.FullDBName);
                        dt.Columns.Add(associationItem.FullDBName, Type.GetType("System.Int32"));
                        searchConditions.Add(associationItem.FullDBName + " = ?");
                        attributeNames.Add(associationItem.FullDBName);
                        attributeValues.Add("?");
                        attributeUpdates.Add(String.Format("{0} = ?", associationItem.FullDBName));
                        break;
                    }
                }
            }

            // добавляем ссылки на неуказанные в запросе классификаторы со значением по умолчанию.
            foreach (IEntityAssociation item in missingDefaultAssociations.Values)
            {
                if (item.RoleBridge.ClassType == ClassTypes.clsDataClassifier ||
                    item.RoleBridge.ClassType == ClassTypes.clsFixedClassifier)
                {
                    searchConditions.Add(item.FullDBName + " = ?");
                }
            }

            // добавляем ссылки на неуказанные в запросе классификаторы значение которых берется через другорой классификатор.
            foreach (KeyValuePair<IEntityAssociation, IEntityAssociation[]> pair in calculatedBindedReferences)
            {

                if (pair.Key.RoleBridge.ClassType == ClassTypes.clsDataClassifier ||
                    pair.Key.RoleBridge.ClassType == ClassTypes.clsFixedClassifier)
                {
                    IEntityAssociation associationItem = pair.Key;

                    //references.Add(alias, associationItem.FullDBName);
                    dt.Columns.Add(associationItem.FullDBName, Type.GetType("System.Int32"));
                    searchConditions.Add(associationItem.FullDBName + " = ?");
                    attributeNames.Add(associationItem.FullDBName);
                    attributeValues.Add("?");
                    attributeUpdates.Add(String.Format("{0} = ?", associationItem.FullDBName));
                }
            }

            #endregion Добавляем ссылки на измерения

            #region Добавляем меры

            List<string> measuresNames = new List<string>();
            foreach (IDataAttribute measure in fact.Attributes.Values)
            {
                if (measure.Class == DataAttributeClassTypes.Typed)
                    measuresNames.Add(measure.Name);
            }

            searchQueryWithoutTask = String.Format("select ID, {0} from {1} where {2}",
                                                   String.Join(", ", measuresNames.ToArray()),
                                                   fact.FullDBName,
                                                   String.Join(" and ", searchConditions.ToArray()).Replace("and TaskID = ?", ""));

            foreach (IDataAttribute item in measures.Values)
            {
                switch (item.Type)
                {
                    case DataAttributeTypes.dtDouble:
                        dt.Columns.Add(item.Name, Type.GetType("System.Double"));
                        break;
                    case DataAttributeTypes.dtInteger:
                    case DataAttributeTypes.dtBoolean:
                        dt.Columns.Add(item.Name, Type.GetType("System.Int32"));
                        break;
                    default:
                        throw new Exception(String.Format(IncorrectMeasureValueType, item.Caption));
                }
                attributeNames.Add(item.Name);
                attributeValues.Add("?");
                attributeUpdates.Add(String.Format("{0} = ?", item.Name));
            }
            #endregion Добавляем меры

            #region Формируем SQL-команды

            // TODO СУБД зависимый код
            if (schemeService.IsOracle())
            {
                insertQuery = String.Format("insert into {0} (ID, {1}) values (?, {2})",
                        fact.FullDBName,
                        String.Join(", ", attributeNames.ToArray()),
                        String.Join(", ", attributeValues.ToArray())
                    );

            }
            else if (schemeService.IsMsSql())
            {
                insertQuery = String.Format("insert into {0} ({1}) values ({2})",
                        fact.FullDBName,
                        String.Join(", ", attributeNames.ToArray()),
                        String.Join(", ", attributeValues.ToArray())
                    );

            }
            else
                throw new NotImplementedException("Для текущей СУБД операция обратной записи не поддерживается.");

            updateQuery = String.Format("update {0} set {1} where ID = ?",
                fact.FullDBName, String.Join(", ", attributeUpdates.ToArray()));
            
            #endregion Формируем SQL-команды

            return dt;
        }

        /// <summary>
        /// Разбор данных запроса
        /// </summary>
        private void ParseData(IDatabase db, int partID)
        {
            dataTable = CreateDataTableAndUpdateQueries();

            // разбираем строки данных
            foreach (XmlNode xmlRow in request.Data.SelectNodes(String.Format("/Requests/Request[@id = {0}]/Data/Row", partID)))
            {
                DataRow row = dataTable.NewRow();

                // ссылка задачу
                row["TaskID"] = taskID;

                // ссылка на классификатор источников
                if (xmlRow.Attributes[dataSourceAlias] == null)
                    throw new Exception(String.Format(
                        "Отсутствует определение измерения источников. " +
                        "Возможно не добавлен общий или частный фильтр по источникам для результата расчёта. " +
                        "Попробуйте добавить общий или частный фильтр по источникам, или обратитесь за помощью к разработчикам. " +
                        "В строке данных отсутствует алиас измерения источников {0}.", dataSourceAlias));
                if (sourceID == -1)
                {
                    sourceID = Convert.ToInt32(xmlRow.Attributes[dataSourceAlias].Value);
                    GetDataSourcesForMissingDataCls(db);
                }
                row["SourceID"] = sourceID;

                // ссылки на классификаторы
                foreach (KeyValuePair<string, IEntity> item in classifiersInRequest)
                {
                    string aliasName = item.Key.Split(new char[] {';'})[0];
                    if (xmlRow.Attributes[aliasName] == null)
                        throw new Exception(String.Format("В строке данных отсутствует ссылка на классификатор \"{0}\" (алиас {1}).", item.Value.OlapName, item.Key));

                    if (String.IsNullOrEmpty(xmlRow.Attributes[aliasName].Value))
                        throw new Exception(String.Format("В строке данных отсутствует значение ссылки на классификатор \"{0}\" (алиас {1}).", item.Value.OlapName, item.Key));

                    int alias = Convert.ToInt32(aliasName.Remove(0, 1));

                    int value;
                    try
                    {
                        value = Convert.ToInt32(xmlRow.Attributes[aliasName].Value);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(String.Format("В строке данных ссылка на классификатор \"{0}\" содержит неверное значение: {1}.", item.Value.Caption, e.Message));
                    }
                    
                    if (item.Value.ClassType == ClassTypes.clsBridgeClassifier)
                        value = FindRowIdInDataClsByBridgeCls(db,
                            item.Value, 
                            bridge2MissingClassifier[item.Value.FullName], 
                            missingClassifiers2SourceID[bridge2MissingClassifier[item.Value.FullName].FullName], 
                            value);

                    //CheckDataSource(item.Value, sourceID, value);
                    row[references[alias]] = value;
                }

                foreach (KeyValuePair<IEntityAssociation, IEntityAssociation[]> pair in calculatedBindedReferences)
                {
                    int bindedId = Convert.ToInt32(row[pair.Value[0].FullDBName]);
                    row[pair.Key.FullDBName] = FindRowIdInDataClsByBindedCls(db, bindedId, pair.Value[0].RoleBridge, pair.Value[1], Convert.ToInt32(pair.Key.RoleDataAttribute.DefaultValue));
                }

                bool deleteRow = false;

                // показатели
                foreach (KeyValuePair<string, IDataAttribute> item in measures)
                {
                    if (xmlRow.Attributes[item.Key] == null)
                        throw new Exception(String.Format("В строке данных отсутствует алиас показателя {0}.", item.Key));

                    if (xmlRow.Attributes[item.Key].Value == "" || xmlRow.Attributes[item.Key].Value == String.Empty)
                        throw new Exception(String.Format("В строке данных отсутствует значение алиаса показателя {0}.", item.Key));

                    if (xmlRow.Attributes[item.Key].Value.ToUpper() != "NULL")
                    {
                        deleteRow = false;
                        switch (item.Value.Type)
                        {
                            case DataAttributeTypes.dtDouble:
                                // Берем значение.
                                string attrValue = xmlRow.Attributes[item.Key].Value;
                                // Меняем запятую на текущий десятичный разделитель.
                                string decimalSeparator = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
                                attrValue = attrValue.Replace(",", decimalSeparator);
                                // Присваиваем.
                                row[item.Value.Name] = double.Parse(attrValue);
                                break;
                            case DataAttributeTypes.dtInteger:
                                row[item.Value.Name] = Convert.ToInt32(xmlRow.Attributes[item.Key].Value);
                                break;
                            case DataAttributeTypes.dtBoolean:
                                int value = Convert.ToInt32(xmlRow.Attributes[item.Key].Value);
                                row[item.Value.Name] = value != 0 ? 1 : 0;
                                break;
                            default:
                                throw new Exception(String.Format(IncorrectMeasureValueType, item.Value.Caption));
                        }
                    }
                    else
                    {
                        row[item.Value.Name] = DBNull.Value;
                        deleteRow = true;
                    }
                }

                dataTable.Rows.Add(row);

                row.AcceptChanges();
                if (deleteRow)
                    row.SetModified();
                else
                    row.SetAdded();
            }
        }

        /// <summary>
        /// Разбор и подготовка запроса к выполнению
        /// </summary>
        private void Prepare(IDatabase db, int partID)
        {
            ParseDeclaration(partID);

            GetMissingClassifiers();
            FindAccordBridgeForMissingClassifiers();
            FindDefaultAssociationsForMissingClassifiers();
            
            CheckMissingClassifiers();

            FindAccodsClsForMissingClassifiers();

            ParseData(db, partID);
        }

        private DataRow FindRow(IDatabase db, DataRow row)
        {
            IDbDataParameter[] parameters = new IDbDataParameter[this.classifiersInRequest.Count + missingDefaultAssociations.Count + calculatedBindedReferences.Count + 1];
            int i = 0;
            parameters[i] = new DbParameterDescriptor(dataTable.Columns[i].ColumnName, row["SourceID"], DbType.Int32);
            i++;
            /* При поиске не учитываем идентификатор задачи
             * parameters[i] = DB.CreateParameter(dataTable.Columns[i].ColumnName, this.taskID, DbType.Int32);
            i++;*/
            foreach (KeyValuePair<string, IEntity> item in this.classifiersInRequest)
            {
                parameters[i] = new DbParameterDescriptor(dataTable.Columns[i + 1].ColumnName, row[references[Convert.ToInt32(item.Key.Split(new char[] { ';' })[0].Remove(0, 1))]], DbType.Int32);
                i++;
            }
            
            // Добавляем в условия поиска неуказанные ссылки имеющие значение по умолчанию
            foreach (IEntityAssociation item in missingDefaultAssociations.Values)
            {
                parameters[i] = new DbParameterDescriptor(item.FullDBName, item.RoleDataAttribute.DefaultValue, DbType.Int32);
                i++;
            }

            foreach (KeyValuePair<IEntityAssociation, IEntityAssociation[]> pair in calculatedBindedReferences)
            {
                parameters[i] = new DbParameterDescriptor(pair.Key.FullDBName, row[pair.Key.FullDBName], DbType.Int32);
                i++;
            }

            DataTable dt = (DataTable)db.ExecQuery(searchQueryWithoutTask, QueryResultTypes.DataTable, parameters);
            if (dt.Rows.Count == 1)
                return dt.Rows[0];
            else if (dt.Rows.Count > 1)
                throw new Exception("Невозможно записать данные, так как в базе данных присутствуют дублирующие записи.");
            else
                return null;
        }

        /// <summary>
        /// Запись данных в базу данных
        /// </summary>
        private void WriteData(IDatabase db, int partID)
        {
            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    DataRow storedRow = FindRow(db, row);

                    // Если строка не найдена и не помечена на удаление
                    if (storedRow == null && row.RowState != DataRowState.Modified)
                    {
                        AddRow(db, row);
                    }
                    else
                    {
                        if (row.RowState == DataRowState.Modified && storedRow != null)
                        {
                            // удаляем показатель
                            DeleteMeasure(db, storedRow, row);
                        }
                        else
                        {
                            if (storedRow != null)
                                UpdateRow(db, storedRow, row);
                            else
                            {
                                Trace.TraceError("BUG в методе WriteData");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка при записи данных в хранилище (partID={0}): {1}", partID, e.Message);
                throw;
            }
        }

        private void AddRow(IDatabase db, DataRow row)
        {
            // TODO СУБД зависимый код
            int i = 0;
            int offset;
            IDbDataParameter[] parameters;
            if (schemeService.IsOracle())
            {
                parameters = new IDbDataParameter[3 + this.classifiersInRequest.Count + calculatedBindedReferences.Count + measures.Count];
                parameters[i++] = new DbParameterDescriptor("ID", fact.GetGeneratorNextValue, DbType.Int32);
                offset = 1;
            }
            else if (schemeService.IsMsSql())
            {
                parameters = new IDbDataParameter[2 + this.classifiersInRequest.Count + calculatedBindedReferences.Count + measures.Count];
                offset = 0;
            }
            else
                throw new NotImplementedException();

            parameters[i++] = new DbParameterDescriptor("SourceID", row["SourceID"], DbType.Int32);
            parameters[i++] = new DbParameterDescriptor("TaskID", taskID, DbType.Int32);
            foreach (KeyValuePair<string, IEntity> item in this.classifiersInRequest)
            {
                parameters[i] = new DbParameterDescriptor(dataTable.Columns[i - offset].ColumnName, row[references[Convert.ToInt32(item.Key.Split(new char[] { ';' })[0].Remove(0, 1))]], DbType.Int32);
                i++;
            }
            foreach (KeyValuePair<IEntityAssociation, IEntityAssociation[]> pair in calculatedBindedReferences)
            {
                parameters[i] = new DbParameterDescriptor(pair.Key.FullDBName, row[pair.Key.FullDBName], DbType.Int32);
                i++;
            }
            AddMeasuresParameters(row, parameters, ref i);
            db.ExecQuery(insertQuery, QueryResultTypes.NonQuery, parameters);
        }

        private void UpdateRow(IDatabase db, DataRow storedRow, DataRow row)
        {
            IDbDataParameter[] parameters = new IDbDataParameter[1 + this.classifiersInRequest.Count + calculatedBindedReferences.Count + measures.Count + 2];
            int i = 0;
            parameters[i++] = new DbParameterDescriptor("SourceID", row["SourceID"], DbType.Int32);
            parameters[i++] = new DbParameterDescriptor("TaskID", taskID, DbType.Int32);
            foreach (KeyValuePair<string, IEntity> item in this.classifiersInRequest)
            {
                parameters[i] = new DbParameterDescriptor(dataTable.Columns[i].ColumnName, row[references[Convert.ToInt32(item.Key.Split(new char[] { ';' })[0].Remove(0, 1))]], DbType.Int32);
                i++;
            }
            foreach (KeyValuePair<IEntityAssociation, IEntityAssociation[]> pair in calculatedBindedReferences)
            {
                parameters[i] = new DbParameterDescriptor(pair.Key.FullDBName, row[pair.Key.FullDBName], DbType.Int32);
                i++;
            }
            AddMeasuresParameters(row, parameters, ref i);
            parameters[i] = new DbParameterDescriptor("ID", storedRow["ID"], DbType.Int32);
            db.ExecQuery(updateQuery, QueryResultTypes.NonQuery, parameters);
        }

        private void DeleteMeasure(IDatabase db, DataRow storedRow, DataRow row)
        {
            if (measures.Count != 1)
                throw new Exception("measures.Count != 1");

            IDataAttribute measure = null;
            foreach (IDataAttribute item in measures.Values)
            {
                measure = item;
            }

            if (measure == null)
                throw new ServerException("В запросе не найден ни один показатель.");

            bool deleteRow = true;

            foreach (IDataAttribute item in this.fact.Attributes.Values)
            {
                if (item.Class == DataAttributeClassTypes.Typed && measure.Name != item.Name)
                {
                    if (!storedRow.IsNull(item.Name))
                    {
                        deleteRow = false;
                        break;
                    }
                }
            }

            if (deleteRow)
            {
                db.ExecQuery(String.Format("delete from {0} where ID = ?", fact.FullDBName), 
                    QueryResultTypes.NonQuery,
                    new DbParameterDescriptor("ID", storedRow["ID"]));
            }
            else
            {
                UpdateRow(db, storedRow, row);
            }
        }

        private void AddMeasuresParameters(DataRow row, IDbDataParameter[] parameters, ref int i)
        {
            foreach (IDataAttribute attr in this.measures.Values)
            {
                switch (attr.Type)
                {
                    case DataAttributeTypes.dtDouble:
                        parameters[i] = new DbParameterDescriptor(attr.Name, row[attr.Name], DbType.Double);
                        break;
                    case DataAttributeTypes.dtInteger:
                    case DataAttributeTypes.dtBoolean:
                        parameters[i] = new DbParameterDescriptor(attr.Name, row[attr.Name], DbType.Int32);
                        break;
                    default:
                        throw new Exception(String.Format(IncorrectMeasureValueType, attr.Name));
                }
                i++;
            }
        }

        private void InitContext()
        {
            ClearContext();
        }

        /// <summary>
        /// Очищает временные коллекции
        /// </summary>
        private void ClearContext()
        {
            fact = null;
            sourceID = -1;
            bridge2MissingClassifier.Clear();
            missingClassifiers2SourceID.Clear();
            classifiersInRequest.Clear();
            missingAssociations.Clear();
            missingDefaultAssociations.Clear();
            calculatedBindedReferences.Clear();
            dataSourceAlias = String.Empty;
            measures.Clear();
            references.Clear();
            if (dataTable != null)
                dataTable.Clear();
        }

        /// <summary>
        /// Выполнение этапов запроса
        /// </summary>
        private void Execute(IDatabase db, int partID)
        {
            WriteData(db, partID);
        }

        /// <summary>
        /// Расчет многомерной базы
        /// </summary>
        private void ProcessCubes()
        {
            foreach (string factName in factForProcess)
            {
                string errMessage;

                IFactTable factTable = null;
                foreach (IFactTable factTableItem in schemeService.GetFactTables())
                {
                    if (factTableItem.FullName == factName)
                    {
                        factTable = factTableItem;
                        break;
                    }
                }

                if (factTable == null)
                {
                    errMessage = String.Format("Таблица фактов {0} не найдена.", factName);
                    throw new ServerException(errMessage);
                }

                try
                {
                    request.BatchID = schemeService.GetOlapProcessor().CreateBatch();
                    schemeService.GetOlapProcessor().InvalidatePartition(
                        factTable,
                        "Krista.FM.Server.WriteBack",
                        ProcessorLibrary.InvalidateReason.WriteBack,
                        factTable.OlapName, request.BatchID);
                    schemeService.GetOlapProcessor().ProcessManager.StartBatch(request.BatchID);
                }
                catch (Exception e)
                {
                    errMessage = String.Format(
                        "Данные были успешно записаны в базу данных, но при расчете куба \"{0}\" возникла необработанная ошибка{1}. " + 
                        "Для того, чтобы записанные данные отобразились на листе, необходимо расчитать куб \"{0}\", а затем обновить данные на листе.",
                        factTable.OlapName, 
                        String.IsNullOrEmpty(e.Message) ? "" : ": " + e.Message);
                    throw new ServerException(errMessage, e.InnerException);
                }
            }
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        private void Finalizing()
        {
            // TODO: 
            fact = null;
        }

        /// <summary>
        /// Основная процедура выполнения запроса
        /// </summary>
        public void Process()
        {
            //Время исполнения запроса

            try
            {
                this.request.Data.Save(String.Format("{0}\\Debug\\{1}_{2}_{3}.xml", schemeService.GetRopositoryDirectory(), "User", this.request.ID, DateTime.Now.ToString().Replace(':', '-')));
            }
            catch { ;/* Просто глушим все глюки */}


            using(IDatabase db = schemeService.GetDb())
            {

                bool inTransaction = false;
                try
                {
                    // Получаем идентификатор задачи  
                    GetTaskID(db);

                    inTransaction = true;
                    db.BeginTransaction();

                    foreach (XmlAttribute partID in request.Data.SelectNodes("/Requests/Request/@id"))
                    {
                        try
                        {
                            currentRequestID = Convert.ToInt32(partID.Value);
                            InitContext();
                            Prepare(db, currentRequestID);
                            Execute(db, currentRequestID);
                        }
                        finally
                        {
                            ClearContext();
                        }
                    }

                    db.Commit();
                    inTransaction = false;
                }
                catch (Exception e)
                {
                    if (inTransaction)
                        db.Rollback();
                    throw new Exception(e.Message, e);
                }
            }

            // если надо - рассчитаем кубы
            bool needProcess = true;
            XmlNode xmlNeedProcess = request.Data.SelectSingleNode(String.Format("/Requests/@ProcessCube"));
            if (xmlNeedProcess != null)
                needProcess = Convert.ToBoolean(xmlNeedProcess.Value);
            if (needProcess)
                ProcessCubes();

            Finalizing();

            try
            {
                this.request.Data.Save(String.Format("{0}\\Debug\\OK\\{1}_{2}_{3}.xml", schemeService.GetRopositoryDirectory(), "User", this.request.ID, DateTime.Now.ToString().Replace(':', '-')));
            }
            catch { ;/* Просто глушим все глюки */}
        }

        /// <summary>
        /// Идентификатор текущего обрабатываемого запроса
        /// </summary>
        internal int CurrentRequestID
        {
            get { return currentRequestID; }
        }
    }
}
