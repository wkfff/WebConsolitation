using System;
using System.Collections.Generic;
using System.Data;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme 
{
    /// <summary>
    /// Таблица перекодировок.
    /// </summary>
    internal class ConversionTable : DisposableObject, IConversionTable
    {
        // Идентификатор (первичный ключ) таблицы перекодировки
        private int _ID = -1;
        /// <summary>
        /// Aссоциация к которой привязана таблица перекодировок
        /// </summary>
        private BridgeAssociation association;
        /// <summary>
        /// Наименование правила сопоставления в котором используется таблицаперекодировок
        /// </summary>
        private string ruleName;
        /// <summary>
        /// Полное наименовение ( = Name + "." + RuleName)
        /// </summary>
        private string fullName = String.Empty;
        // Семантическое наименование
        private string semanticName = "NotSpecified";

        /// <summary>
        /// Запрос для выборки данных из таблицы перекодировок
        /// </summary>
        private string selectQuery = String.Empty;

        /// <summary>
        /// Ключ - Английское наименование атрибута из классификатора данных;
        /// Значение - Выражение запроса из секции select для атрибута с псевдонимом
        /// </summary>
        private Dictionary<string, string> inputAttributesNames = new Dictionary<string, string>();

        /// <summary>
        /// Ключ - Английское наименование атрибута из сопоставимого;
        /// Значение - Выражение запроса из секции select для атрибута с псевдонимом
        /// </summary>
        private Dictionary<string, string> outputAttributesNames = new Dictionary<string, string>();

        /// <summary>
        /// Ключ - Английское наименование атрибута из классификатора данных;
        /// Значение - Выражение запроса из секции select для атрибута без псевдонима
        /// </summary>
        private Dictionary<string, string> inputAttributesAliasNames = new Dictionary<string, string>();

        /// <summary>
        /// Ключ - Английское наименование атрибута из сопоставимого;
        /// Значение - Выражение запроса из секции select для атрибута без псевдонима
        /// </summary>
        private Dictionary<string, string> outputAttributesAliasNames = new Dictionary<string, string>();

        /// <summary>
        /// Ключ - Английское наименование;
        /// Значение - Русское наименование атрибута
        /// </summary>
        private Dictionary<string, string> inputAttributesCaptions = new Dictionary<string, string>();

        /// <summary>
        /// Ключ - Английское наименование;
        /// Значение - Русское наименование атрибута
        /// </summary>
        private Dictionary<string, string> outputAttributesCaptions = new Dictionary<string, string>();


        #region Инициализация

        /// <summary>
        /// Инициализация экземпляра объекта.
        /// </summary>
        /// <param name="association"></param>
        /// <param name="ruleName"></param>
        public ConversionTable(BridgeAssociation association, string ruleName)
        {
            if (association == null)
                throw new ArgumentNullException("association");
            this.association = association;

            if (ruleName == null)
                throw new ArgumentNullException("ruleName");
            this.ruleName = ruleName;

            GetAttributes();
        }

        /// <summary>
        /// Возвращает наименование поля-значения из сателита в зависимости от типа данных
        /// </summary>
        /// <param name="dataType">Тип данных атрибута</param>
        /// <returns>Инаименование поля-значения</returns>
        private static string GetAttributeValueName(DataAttributeTypes dataType)
        {
            if (dataType == DataAttributeTypes.dtString)
            {
                return "ValueStr";
            }
            else if (dataType == DataAttributeTypes.dtInteger || dataType == DataAttributeTypes.dtDouble)
            {
                return "ValueNum";
            }
            else
                throw new Exception("В таблице перекодитовок указан атрибут с недопустимым типом.");
        }

        private void InitializeDataCollections2(Association association, string attrName)
        {
            string inputCaption = ">" + attrName;
            inputCaption = inputCaption.Substring(0, inputCaption.Length > 30 ? 30 : inputCaption.Length);
            inputAttributesCaptions.Add(attrName, inputCaption);

            DataAttribute da =  DataAttributeCollection.GetAttributeByKeyName(association.RoleData.Attributes, attrName, attrName);
            inputAttributesNames.Add(attrName,
                String.Format("I_{0}.{1} as \"{2}\"",
                attrName, GetAttributeValueName(da.Type), inputCaption));

            inputAttributesAliasNames.Add(attrName,
                String.Format("I_{0}.{1}", attrName, GetAttributeValueName(da.Type)));
        }

        private void InitializeDataCollections(Association association, MappingValue attr)
        {
            if (attr.IsSample)
            {
                InitializeDataCollections2(association, attr.Name);
            }
            else
            {
                foreach (string attrName in attr.SourceAttributes)
                {
                    InitializeDataCollections2(association, attrName);
                }
            }
        }

        private void InitializeBridgeCollections2(Association association, string attrName)
        {
            string outputCaption = "<" + attrName;
            outputCaption = outputCaption.Substring(0, outputCaption.Length > 30 ? 30 : outputCaption.Length);
            outputAttributesCaptions.Add(attrName, outputCaption);

            DataAttribute da = DataAttributeCollection.GetAttributeByKeyName(association.RoleBridge.Attributes, attrName, attrName);
            outputAttributesNames.Add(attrName, 
                String.Format("O_{0}.{1} as \"{2}\"",
                attrName, GetAttributeValueName(da.Type), outputCaption));
            
            outputAttributesAliasNames.Add(attrName, 
                String.Format("O_{0}.{1}", attrName, GetAttributeValueName(da.Type)));
        }

        private void InitializeBridgeCollections(Association association, MappingValue attr)
        {
            if (attr.IsSample)
            {
                InitializeBridgeCollections2(association, attr.Name);
            }
            else
            {
                foreach (string attrName in attr.SourceAttributes)
                {
                    InitializeBridgeCollections2(association, attrName);
                }
            }
        }

        private void GetAttributes()
        {
            inputAttributesNames.Clear();
            outputAttributesNames.Clear();
            inputAttributesAliasNames.Clear();
            outputAttributesAliasNames.Clear();
            inputAttributesCaptions.Clear();
            outputAttributesCaptions.Clear();

            if (!SchemeClass.Instance.Associations.ContainsKey(Name))
                throw new Exception(String.Format("Для таблицы перекодировок указана неверная ассоциация с именем {0}", Name));

            Association association = (Association)SchemeClass.Instance.Associations[Name];

            foreach (AssociateMapping mapping in GetAssociateMappings())
            {
                InitializeDataCollections(association, mapping.DataAttribute);
                InitializeBridgeCollections(association, mapping.BridgeAttribute);
            }
        }

        /// <summary>
        /// Формирует запрос для выборки данных из таблицы перекодировок
        /// </summary>
        /// <returns>Текст SQL-запроса</returns>
        private string FormQuery()
        {
            if (!String.IsNullOrEmpty(selectQuery))
                return selectQuery; // Берем уже сформированный запрос
            else
            {
                if (ID == -1)
                    return String.Empty;

                List<string> inputJoines = new List<string>();
                List<string> outputJoines = new List<string>();

                foreach (AssociateMapping mapping in GetAssociateMappings())
                {
                    inputJoines.Add(String.Format("join ConversionInputAttributes I_{0} on (I_{0}.TypeID = {1} and T.ID = I_{0}.ID and I_{0}.Name = '{0}')", mapping.DataAttribute.Name, ID));
                    if (mapping.BridgeAttribute.IsSample)
                    {
                        outputJoines.Add(String.Format("join ConversionOutAttributes O_{0} on (O_{0}.TypeID = {1} and T.ID = O_{0}.ID and O_{0}.Name = '{0}')", mapping.BridgeAttribute.Name, ID));
                    }
                    else
                    {
                        foreach (string attrName in mapping.BridgeAttribute.SourceAttributes)
                        {
                            outputJoines.Add(String.Format("join ConversionOutAttributes O_{0} on (O_{0}.TypeID = {1} and T.ID = O_{0}.ID and O_{0}.Name = '{0}')", attrName, ID));
                        }
                    }
                }

                string[] inputAttributes = new string[inputAttributesNames.Count];
                string[] outputAttributes = new string[outputAttributesNames.Count];
                inputAttributesNames.Values.CopyTo(inputAttributes, 0);
                outputAttributesNames.Values.CopyTo(outputAttributes, 0);

                // Запрос возврашающий собственно саму таблицу перекодировок
                string queryConversionTable = String.Format("select T.ID,\n\t{0},\n\t{1}\nfrom ConversionTable T\n\t{2}\n\t{3}\nwhere T.TypeID = {4}",
                    String.Join(", ", inputAttributes),
                    String.Join(", ", outputAttributes),
                    String.Join("\n\t", inputJoines.ToArray()),
                    String.Join("\n\t", outputJoines.ToArray()),
                    ID);
                
/*                string queryConversionTableUsedRows = String.Format(
                    "select distinct CT.ID, {0}, {1}\nfrom\n {3} CT\n join B on (T.* = B.*)\n join D on (T.* = D.*)",
                    String.Join(", ", inputAttributes),
                    String.Join(", ", outputAttributes),
                    queryConversionTable);            */
                return queryConversionTable;
            }
        }

        #endregion Инициализация

        /// <summary>
        /// Создает таблицу перекодировок в базе данных.
        /// </summary>
        internal void AttachToDB()
        {
            IDatabase db = SchemeDWH.Instance.DB;
            try
            {
                AttachToDB(db);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Создает таблицу перекодировок в базе данных.
        /// </summary>
        /// <param name="db"></param>
        internal void AttachToDB(IDatabase db)
        {
            // Добавляем в базу
            this.ID = db.GetGenerator("g_MetaConversionTable");
            db.ExecQuery(
                "insert into MetaConversionTable (ID, RefAssociation, AssociateRule) values (?, ?, ?)",
                QueryResultTypes.NonQuery,
                db.CreateParameter("ID", this.ID),
                db.CreateParameter("RefAssociation", this.association.ID),
                db.CreateParameter("AssociateRule", this.RuleName)
            );
        }

        /// <summary>
        /// Добавляет новую запись в таблицу перекодировок
        /// </summary>
        /// <param name="fromRow"></param>
        /// <param name="toRow"></param>
        public void AddConversion(object[] fromRow, object[] toRow)
        {
            if (IsDetached)
                AttachToDB();
            // проверим права на добавление записи в таблицу перекодировок
            IUsersManager userManager = SchemeClass.Instance.UsersManager;
            bool canAddToConversion = userManager.CheckPermissionForSystemObject(this.association.ObjectKey,
                                                       (int) Krista.FM.ServerLibrary.AssociateOperations.AddRecordIntoBridgeTable, false);
            if (!canAddToConversion)
                return;

            Association association = (Association)SchemeClass.Instance.Associations[Name];

            DataUpdater du = (DataUpdater)GetDataUpdater();
            try
            {
                DataTable dt = new DataTable();

                du.Fill(ref dt);

                DataRow row = dt.NewRow();

                foreach (KeyValuePair<string, string> item in inputAttributesCaptions)
                {
                    int ColumnID = 0;
                    foreach (IDataAttribute attr in association.RoleA.Attributes.Values)
                    {
                        if (attr.Name == item.Key)
                            break;
                        ColumnID++;
                    }
                    if (ColumnID > association.RoleA.Attributes.Count)
                        throw new Exception(String.Format("Не найден атрибут {0} в таблице перекодировок {1} со стороны классификатора данных.", item.Key, Name));

                    row[item.Value] = fromRow[ColumnID];
                }

                foreach (KeyValuePair<string, string> item in outputAttributesCaptions)
                {
                    int ColumnID = 0;
                    foreach (IDataAttribute attr in association.RoleB.Attributes.Values)
                    {
                        if (attr.Name == item.Key)
                            break;
                        ColumnID++;
                    }
                    if (ColumnID > association.RoleB.Attributes.Count)
                        throw new Exception(String.Format("Не найден атрибут {0} в таблице перекодировок {1} со стороны сопоставимого классификатора.", item.Key, Name));

                    row[item.Value] = toRow[ColumnID];
                }

                dt.Rows.Add(row);
                
                du.Update(ref dt);
            }
            finally
            {
                du.Dispose();
            }

        }

        /// <summary>
        /// Возвращает коллекцию правил соответствия для правила сопоставления.
        /// </summary>
        /// <returns>Коллекция правил соответствия.</returns>
        private IAssociateMappingCollection GetAssociateMappings()
        {
            BridgeAssociation association = (BridgeAssociation)SchemeClass.Instance.Associations[Name];

            if (!association.AssociateRules.ContainsKey(ruleName))
                throw new Exception("У данного объекта не указано ни одного правила сопоставления.");

            return ((AssociateRule)association.AssociateRules[ruleName]).Mappings;
        }

        /// <summary>
        /// Находит ID записи таблицы перекодировки соответствующей записи dataRow
        /// </summary>
        /// <param name="db">Объект доступа к БД в контексте которого будут производится все операции с данными</param>
        /// <param name="dataRow">Строка для которой необходимо найти ID записи таблицы перекодировок</param>
        /// <returns>ID записи таблицы перекодировки</returns>
        private int FindRowID(IDatabase db, DataRow dataRow)
        {
            List<IDbDataParameter> parametersList = new List<IDbDataParameter>();
            
            // фильтровые условия 
            List<string> conditions = new List<string>();

            // входные параметры
            foreach (KeyValuePair<string, string> item in inputAttributesAliasNames)
            {
                if (!dataRow.Table.Columns.Contains(inputAttributesCaptions[item.Key]))
                    throw new Exception(String.Format("Искомая строка не содержит исходный атрибут \"{0}\"", inputAttributesCaptions[item.Key]));
                
                conditions.Add(String.Format("{0} = ?", item.Value));

                parametersList.Add(db.CreateParameter("I_" + item.Key, dataRow[inputAttributesCaptions[item.Key]]));
//                parameters[paramNo] = db.CreateParameter("I_" + item.Key, dataRow[inputAttributesCaptions[item.Key]]);
//                paramNo++;
            }

            // выходные параметры
            foreach (KeyValuePair<string, string> item in outputAttributesAliasNames)
            {
                if (!dataRow.Table.Columns.Contains(outputAttributesCaptions[item.Key]))
                    throw new Exception(String.Format("Искомая строка не содержит результирующий атрибут \"{0}\"", outputAttributesCaptions[item.Key]));

                object value = dataRow[outputAttributesCaptions[item.Key]];
                if (value is DBNull)
                {
                    conditions.Add(String.Format("{0} is null", item.Value));
                }
                else
                {
                    conditions.Add(String.Format("{0} = ?", item.Value));
                    parametersList.Add(db.CreateParameter("O_" + item.Key, value));
                    //parameters[paramNo] = db.CreateParameter("O_" + item.Key, value);
                    //paramNo++;
                }
            }

            string queryText = String.Format("{0} and ({1})", FormQuery(), String.Join(" and ", conditions.ToArray()));

            DataTable dt = (DataTable)db.ExecQuery(queryText, QueryResultTypes.DataTable, parametersList.ToArray());
            if (dt.Rows.Count > 1)
                throw new Exception("Нарушено ограничение уникальности: в таблице перекодировок присутствуют дублирующиеся записи.");
            else if (dt.Rows.Count == 0)
                // запись не найдена
                return -1;
            else
                return Convert.ToInt32(dt.Rows[0][0]);
        }

        private bool InsteadInsert(IDatabase db, DataRow dataRow)
        {
            if (FindRowID(db, dataRow) != -1)
                throw new Exception("Вставляемая запись в таблице перекодировок уже присутствует.");
            else
            {
                // Вставляем запись в главную таблицу
                int rowID = db.GetGenerator("g_ConversionTable");
                db.ExecQuery("insert into ConversionTable (ID, TypeID) values (?, ?)", QueryResultTypes.NonQuery, 
                    db.CreateParameter("ID", rowID), db.CreateParameter("TypeID", ID));

                // входные параметры
                foreach (KeyValuePair<string, string> item in inputAttributesAliasNames)
                {
                    string[] aliasName = item.Value.Split('.');
                    db.ExecQuery(String.Format(
                        "insert into ConversionInputAttributes (TypeID, ID, Name, {0}) values (?, ?, ?, ?)", aliasName[1]), 
                        QueryResultTypes.NonQuery,
                        db.CreateParameter("TypeID", ID),
                        db.CreateParameter("ID", rowID),
                        db.CreateParameter("Name", item.Key),
                        db.CreateParameter("Value", dataRow[inputAttributesCaptions[item.Key]]));
                }

                // выходные параметры
                foreach (KeyValuePair<string, string> item in outputAttributesAliasNames)
                {
                    string[] aliasName = item.Value.Split('.');
                    object value = dataRow[outputAttributesCaptions[item.Key]];
                    DbType valueType = aliasName[1] == "ValueStr" ? DbType.String : DbType.Decimal;
                    db.ExecQuery(String.Format(
                        "insert into ConversionOutAttributes (TypeID, ID, Name, {0}) values (?, ?, ?, ?)", aliasName[1]),
                        QueryResultTypes.NonQuery,
                        db.CreateParameter("TypeID", ID),
                        db.CreateParameter("ID", rowID),
                        db.CreateParameter("Name", item.Key),
                        db.CreateParameter("Value", value, valueType));
                }

                return true;
            }
        }

        private bool InsteadUpdate(IDatabase db, DataRow dataRow)
        {
            int rowID = Convert.ToInt32(dataRow["ID"]);

            // обновляем входные параметры
            foreach (KeyValuePair<string, string> item in inputAttributesAliasNames)
            {
                string[] aliasName = item.Value.Split('.');
                db.ExecQuery(String.Format(
                    "update ConversionInputAttributes set {0} = ? where TypeID = ? and ID = ? and Name = ?", aliasName[1]),
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("Value", dataRow[inputAttributesCaptions[item.Key]]),
                    db.CreateParameter("TypeID", ID),
                    db.CreateParameter("ID", rowID),
                    db.CreateParameter("Name", item.Key));
            }

            // обновляем выходные параметры
            foreach (KeyValuePair<string, string> item in outputAttributesAliasNames)
            {
                string[] aliasName = item.Value.Split('.');
                db.ExecQuery(String.Format(
                    "update ConversionOutAttributes set {0} = ? where TypeID = ? and ID = ? and Name = ?", aliasName[1]),
                    QueryResultTypes.NonQuery,
                    db.CreateParameter("Value", dataRow[outputAttributesCaptions[item.Key]]),
                    db.CreateParameter("TypeID", ID),
                    db.CreateParameter("ID", rowID),
                    db.CreateParameter("Name", item.Key));
            }

            return true;
        }

        private bool InsteadDelete(IDatabase db, DataRow dataRow)
        {
            return false;
        }

        private bool OnInsteadUpdate(IDatabase db, DataRow dataRow)
        {
            switch (dataRow.RowState)
            {
                case DataRowState.Added:
                    return InsteadInsert(db, dataRow);
                case DataRowState.Modified:
                    return InsteadUpdate(db, dataRow);
                case DataRowState.Deleted:
                    return InsteadDelete(db, dataRow);
                default:
                    Trace.TraceError("Запись не имеет изменений или имеет состояние Detached. ConversionTable.OnInsteadUpdate()");
                    return false;
            }
        }

        public IDataUpdater GetDataUpdater()
        {
            IDatabase db = SchemeDWH.Instance.DB;
            DataUpdater du;
            try
            {
                if (IsDetached)
                    AttachToDB(db);

                string queryText = FormQuery();
                du = (DataUpdater)db.GetDataUpdater(queryText);
                du.OnInsteadUpdate += new InsteadUpdateEventDelegate(OnInsteadUpdate);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                throw new Exception(e.Message, e);
            }
            finally
            {
                db.Dispose();
            }
            return du;
        }

        /// <summary>
        /// Удаляет все данные из таблицы перекодировки
        /// </summary>
        public void Clear()
        {
            IDatabase db = SchemeDWH.Instance.DB;
            try
            {
                db.ExecQuery("delete from ConversionTable where TypeID = ?", QueryResultTypes.NonQuery, 
                    db.CreateParameter("TypeID", this.ID));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                throw new Exception(e.Message, e);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Удаляет строку из таблицы перекодировок
        /// </summary>
        /// <param name="rowID">ID записи которую необходимо удалить</param>
        public void DeleteRow(int rowID)
        {
            IDatabase db = SchemeDWH.Instance.DB;
            try
            {
                db.ExecQuery("delete from ConversionTable where TypeID = ? and ID = ?", QueryResultTypes.NonQuery,
                    db.CreateParameter("TypeID", this.ID),
                    db.CreateParameter("ID", rowID));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                throw new Exception(e.Message, e);
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// ID таблицы перекодировок.
        /// </summary>
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public bool IsDetached
        {
            get { return ID == -1; }
        }

        /// <summary>
        /// Наименование, сейчас соответствует имени ассоциации.
        /// </summary>
        public string Name
        {
            get { return KeyIdentifiedObject.GetKey(association.ObjectKey, association.FullName); }
        }

        /// <summary>
        /// Наименование правила сопоставления в котором используется таблицаперекодировок.
        /// </summary>
        public string RuleName
        {
            get { return ruleName; }
        }

        /// <summary>
        /// Полное наименовение ( = Name + "." + RuleName).
        /// </summary>
        public string FullName
        {
			get { return String.Format("{0}.{1}", association.FullCaption, ((IBridgeAssociation)association).AssociateRules[ruleName].Name); }
        }
        
        /// <summary>
        /// Семантическое наименование.
        /// </summary>
        public string SemanticName
        {
            get { return semanticName; }
            set { semanticName = value; }
        }
    }
}
