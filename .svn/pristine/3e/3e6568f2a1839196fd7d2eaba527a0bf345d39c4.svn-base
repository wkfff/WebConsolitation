using System;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.ScriptingEngine.Oralce
{
    internal class OracleScriptingEngineImpl : ScriptingEngineImpl
    {
        private static readonly Dictionary<DataAttributeTypes, string> dataTypeMappings;
        private bool bitmapIndexEnabled;

        /// <summary>
        /// Конструктор типа
        /// </summary>
        static OracleScriptingEngineImpl()
        {
            dataTypeMappings = new Dictionary<DataAttributeTypes, string>();
            dataTypeMappings.Add(DataAttributeTypes.dtBoolean, "NUMBER");
            dataTypeMappings.Add(DataAttributeTypes.dtChar, "CHAR");
            dataTypeMappings.Add(DataAttributeTypes.dtDate, "DATE");
            dataTypeMappings.Add(DataAttributeTypes.dtDateTime, "DATE");
            dataTypeMappings.Add(DataAttributeTypes.dtDouble, "NUMBER");
            dataTypeMappings.Add(DataAttributeTypes.dtInteger, "NUMBER");
            dataTypeMappings.Add(DataAttributeTypes.dtString, "VARCHAR2");
            dataTypeMappings.Add(DataAttributeTypes.dtBLOB, "BLOB");
        }

        public OracleScriptingEngineImpl(Database db)
            : base(db)
        {
            bitmapIndexEnabled = IsBitmapIndexEnabled(db);
        }

        internal override bool ExistsObject(string objectName, ObjectTypes objectType)
        {
            return 1 == Convert.ToInt32(_db.ExecQuery("select count(*) from DVDB_Objects where Name = ? and Type = ?",
                QueryResultTypes.Scalar,
                _db.CreateParameter("Name", objectName.ToUpper()),
                _db.CreateParameter("Type", ObjectTypes2String(objectType))));
        }

        /// <summary>
        /// Преобразует перечисление в строку
        /// </summary>
        /// <param name="objectType">Тып объекта в базе данны</param>
        /// <returns>Наименование объекта в базе данных</returns>
        private static string ObjectTypes2String(ObjectTypes objectType)
        {
            switch (objectType)
            {
                case ObjectTypes.Index: return "INDEX";
                case ObjectTypes.Procedure: return "PROCEDURE";
                case ObjectTypes.Sequence: return "SEQUENCE";
                case ObjectTypes.Table: return "TABLE";
                case ObjectTypes.Trigger: return "TRIGGER";
                case ObjectTypes.View: return "VIEW";
                case ObjectTypes.ForeignKeysConstraint: return "R";
                default: throw new Exception("Указан неизвестный тип объекта.");
            }
        }

        internal override List<string> CreateSequenceScript(string sequenceName, int seed, int increment)
        {
            List<string> script = new List<string>();
            script.Add(String.Format("CREATE SEQUENCE {0} START WITH {1} INCREMENT BY {2}", sequenceName, seed, increment));
            return script;
        }

        internal override string DropTableScript(string tableName)
        {
            return String.Format("DROP TABLE {0} CASCADE CONSTRAINTS", tableName);
        }

		internal override string CreateViewScript(string viewName, string viewBody)
		{
			return String.Format("CREATE VIEW {0} {1}", viewName, viewBody);
		}

		internal override string DropViewScript(string viewName)
		{
			return String.Format("DROP VIEW {0}", viewName);
		}

		internal override List<string> ModifyColumnScript(string tableName, string columnDefClause)
		{
            List<string> script = new List<string>();
		    script.Add(String.Format("ALTER TABLE {0} MODIFY ( {1} )", tableName, columnDefClause));
		    return script;
		}
		
		internal override string DropSequenceScript(string sequenceName)
        {
            return String.Format("DROP SEQUENCE {0}", sequenceName);
        }

        internal override List<string> ParentKeyConstraintScript(string tableName, string constraintName, string refFieldName)
        {
            List<string> script = new List<string>();
            
            string onDeleteClause = "ON DELETE SET NULL";

            script.Add(String.Format("ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ( {2} ) REFERENCES {0} ( ID ) {3}",
                tableName, constraintName, refFieldName, onDeleteClause));
            
            return script;
        }

        internal override int MaxIdentifierLength()
        {
            return 30;
        }

        public override string ConcatenateChar()
        {
            return "||";
        }

        public override string ParameterPrefixChar()
        {
            return ":";
        }

        private static string TriggerFireTypes2String(TriggerFireTypes triggerFireType)
        {
            switch (triggerFireType)
            {
                case TriggerFireTypes.Before: return "BEFORE";
                case TriggerFireTypes.After: return "AFTER";
                case TriggerFireTypes.InsteadOf: return "INSTEAD OF";
                default: throw new ScriptingEngineException(String.Format("Неизвестный модификатор {0}.", triggerFireType));
            }
        }

        private static string DMLEventTypes2String(DMLEventTypes dmlEventType)
        {
            List<string> result = new List<string>();
			if ((dmlEventType & DMLEventTypes.Insert) == DMLEventTypes.Insert)
                result.Add("INSERT");
            if ((dmlEventType & DMLEventTypes.Update) == DMLEventTypes.Update)
                result.Add("UPDATE");
            if ((dmlEventType & DMLEventTypes.Delete) == DMLEventTypes.Delete)
                result.Add("DELETE");
            return String.Join(" OR ", result.ToArray());
        }

        internal override string CreateTriggerScript(string triggerName, string tableName, TriggerFireTypes triggerFireType, DMLEventTypes dmlEventType, string sqlDeclareStatements, string sqlStatements)
        {
            return String.Format("CREATE OR REPLACE TRIGGER {0} {1} {2} ON {3} FOR EACH ROW\n{4}BEGIN\n{5}\nEND;", 
                triggerName, 
                TriggerFireTypes2String(triggerFireType), 
                DMLEventTypes2String(dmlEventType), 
                tableName,
                String.IsNullOrEmpty(sqlDeclareStatements) ? String.Empty : String.Format("DECLARE\n{0}\n", sqlDeclareStatements),
                sqlStatements);
        }

        internal override List<string> CreateCascadeDeleteTriggerScript(string tableName, Dictionary<string, string> referencesList, string triggerName, string defaultValue)
        {
            List<string> script = new List<string>();
            return script;
        }

        internal override List<string> DropCascadeDeleteTriggerScript(string tiggerName)
        {
            List<string> script = new List<string>();
            return script;
        }

        internal override List<string> CreateDeveloperLockTriggerScript(string tableName)
        {
            List<string> script = new List<string>();
            return script;
        }

        internal override List<string> DropDeveloperLockTriggerScript(string tableName)
        {
            List<string> script = new List<string>();

            script.Add(String.Format("DROP TRIGGER {0}", DeveloperLockTriggerName(tableName)));

            return script;
        }

        internal override string GetIDFromGeneratorScript(string generatorName)
        {
            return String.Format("if :new.ID is null then select {0}.NextVal into :new.ID from Dual; end if;\n",
                generatorName);
        }

        internal override string SqlBlockSetDefaultValue(string attributeName, string value)
        {
            return String.Format("if :new.{0} is null then :new.{0} := {1}; end if;",
                attributeName, value);
        }

        internal override string GeneratorName(string tableName)
        {
			return "g_" + tableName.Substring(0, tableName.Length > MaxIdentifierLength() - 2 ? MaxIdentifierLength() - 2 : tableName.Length);
        }

        internal override string AutoIncrementColumnScript(string columnName)
        {
            return String.Format("{0} NUMBER(10) not null", columnName);
        }

        internal override string DataTypeMappings(DataAttributeTypes dataAttributeType)
        {
            return dataTypeMappings[dataAttributeType];
        }

        internal override string GetDataTypeScript(string name, DataAttributeClassTypes classType, DataAttributeTypes type, int size, int scale)
        {
            string sizeSlale = String.Empty;
            switch (type)
            {
                case DataAttributeTypes.dtBoolean:
                case DataAttributeTypes.dtInteger:
                case DataAttributeTypes.dtString:
                    sizeSlale = String.Format("({0})", size);
                    break;
                case DataAttributeTypes.dtDouble:
                    sizeSlale = String.Format("({0}, {1})", size, scale);
                    break;
            }
            return DataTypeMappings(type) + sizeSlale;
        }

        internal override List<string> EnableAllTriggersScript(string objectName)
        {
            List<string> script = new List<string>();
            script.Add(String.Format("ALTER TABLE {0} ENABLE ALL TRIGGERS", objectName));
            return script;
        }

        internal override List<string> DisableAllTriggersScript(string objectName)
        {
            List<string> script = new List<string>();
            script.Add(String.Format("ALTER TABLE {0} DISABLE ALL TRIGGERS", objectName));
            return script;
        }

        internal override List<string> CreateAuditTriggerScript(string triggerName, string tableName, bool excludeDataPamp, string taskIdColumnName, string pumpIdColumnName, string logObjectName, int logObjectType)
        {
            List<string> attrNames = new List<string>();
            List<string> attrValues = new List<string>();
            List<string> attrValueN = new List<string>();

            if (taskIdColumnName != null)
            {
                attrNames.Add(", " + taskIdColumnName);
                attrValues.Add(", :old." + taskIdColumnName);
                attrValueN.Add(", :new." + taskIdColumnName);
            }

            if (pumpIdColumnName != null)
            {
                attrNames.Add(", " + pumpIdColumnName);
                attrValues.Add(", :old." + pumpIdColumnName);
                attrValueN.Add(", :new." + pumpIdColumnName);
            }

            string insertTemplate = "insert into DVAudit.DataOperations (KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID{0}) values ({2}, '{3}', {4}, UserName, SessionID, :{5}.ID{1});";
            
            //пох. на того, кто будет разбираться в этой строке, зато "сэкономили" память за счёт String.Format  >:o
            string triggerBody = String.Format(
                "  SELECT SYS_CONTEXT ('DVContext', 'USERNAME'), SYS_CONTEXT('DVContext', 'SESSIONID') into UserName, SessionID FROM dual;\n" +
                (excludeDataPamp ? "if (UserName not like 'Krista.FM.Server.DataPumps%') then\n" : String.Empty) +
                "  if inserting then {0}\n" +
                "  elsif updating then {1}\n" +
                "  elsif deleting then {2} end if;\n" +
                (excludeDataPamp ? "end if;\n" : String.Empty),
                String.Format(insertTemplate, String.Join("", attrNames.ToArray()), String.Join("", attrValueN.ToArray()), 0,
                              logObjectName, logObjectType, "new"),
                String.Format(insertTemplate, String.Join("", attrNames.ToArray()), String.Join("", attrValueN.ToArray()), 1,
                              logObjectName, logObjectType, "new"),
                String.Format(insertTemplate, String.Join("", attrNames.ToArray()), String.Join("", attrValues.ToArray()), 2,
                              logObjectName, logObjectType, "old"));

            
            List<string> script = new List<string>();

            script.Add(CreateTriggerScript(triggerName, tableName, TriggerFireTypes.After, DMLEventTypes.Insert | DMLEventTypes.Update | DMLEventTypes.Delete,
                                            "UserName varchar2(64); SessionID varchar2(24);", triggerBody));

            return script;
        }

        internal override string DropTriggerScript(string triggerName)
        {
            return String.Format("drop trigger {0}", triggerName);
        }

        internal override string DropProcedureScript(string procedureName)
        {
            return String.Format("drop procedure {0}", procedureName);
        }

        internal override string CreateVariantLockTriggerScript(string triggerName, string refVariantColumnName, string tableName, string shortTableName, string variantTableName)
        {
            string triggerDeclare = "declare variantState pls_integer;newVariantState pls_integer;\n";
            string triggerBody = String.Format(
                "newVariantState := 0;\n" +
                "if   inserting then select VariantCompleted into variantState from {0} where ID = :new.{1};\n" +
                "elsif updating then\n" +
                "\tselect VariantCompleted into variantState from {0} where ID = :old.{1};\n" +
                "\tselect VariantCompleted into newVariantState from {0} where ID = :new.{1};\n" +
                "elsif deleting then select VariantCompleted into variantState from {0} where ID = :old.{1}; end if;\n" +
                "if  variantState = 1 or newVariantState = 1 then raise_application_error(-20101, 'Вариант заблокирован.'); end if;\n",
                variantTableName, refVariantColumnName);

            return String.Format(
                "create or replace trigger {0} before delete or insert or update on {1} for each row\n{2}begin\n{3}end;",
                triggerName, tableName, triggerDeclare, triggerBody);
        }

        internal override string CreateDataSourceLockTriggerScript(string triggerName, string sourceIDColumnName, string tableName, string shortTableName, string dataSourceTableName)
        {
            string triggerDeclare = "declare sourceLocked pls_integer;newSourceLocked pls_integer;\n";
            string triggerBody = String.Format(
                "newSourceLocked := 0;\n" +
                "SourceLocked := 0;\n" +
                "if  inserting then " +
                "if :new.{1} > 0 then " +
                "select Locked into sourceLocked from {0} where ID = :new.{1};" +
                " end if;\n" +
                "elsif updating then\n" +
                "if :new.{1} > 0 and :old.{1} > 0 then" +
                "\tselect Locked into sourceLocked from {0} where ID = :old.{1};\n" +
                "\tselect Locked into newSourceLocked from {0} where ID = :new.{1};\n" +
                " end if;\n" +
                "elsif deleting then " +
                "if :old.{1} > 0 then " +
                "select Locked into sourceLocked from {0} where ID = :old.{1};" +
                " end if;\n" + 
                " end if;\n" + // Для основной секции.
                "if  sourceLocked = 1 or newSourceLocked = 1 then raise_application_error(-20102, 'Источник данных заблокирован от изменений.'); end if;\n",
                dataSourceTableName, sourceIDColumnName);

            return String.Format(
                "create or replace trigger {0} before delete or insert or update on {1} for each row\n{2}begin\n{3}end;",
                triggerName, tableName, triggerDeclare, triggerBody);
        }

        internal static void TestAndRestoreObjects(IDatabase db, string testSQL, string restoreSQL)
        {
            const int MaxBadPassCount = 3; // Сколько делать проходов, если нет улучшения.  
            int notCorrectCount = 0; // Количество инвалидных объектов
            int passCount = 1; // Первый проход.
            int badPassCount = 0; // Попыток неудачного исправления не было. 

            while (true)
            {
                // Ищем объекты в некорректном состоянии.
                DataTable t = (DataTable)db.ExecQuery(testSQL, QueryResultTypes.DataTable);

                if (t.Rows.Count == 0)
                    break; // нет некорректных объектов - выходим из цикла восстановления.

                // Если уже не первый проход
                if (passCount > 1)
                {
                    // Если количество некорректных объектов не уменьшается.
                    if (t.Rows.Count >= notCorrectCount)
                    {
                        // Попробуем ещё пару раз, вдруг получится.
                        badPassCount++; // Посчитаем, сколько безуспешных попыток совершено.
                        // Если давно уже ничего не улучшается - выходим.
                        if (badPassCount > MaxBadPassCount)
                            break;
                    }
                }

                // Переберём все некорректные объекты.
                foreach (DataRow row in t.Rows)
                {
                    string restoringObject = Convert.ToString(row[0]);
                    string typeOrTable = String.Empty;
                    if (t.Columns.Count > 1)
                        typeOrTable = Convert.ToString(row[1]);

                    // Собственно попытка восстановления объекта.
                    try
                    {
                        string restoreSQLText;
                        if (t.Columns.Count > 1)
                            restoreSQLText = String.Format(restoreSQL, typeOrTable, restoringObject);
                        else
                            restoreSQLText = String.Format(restoreSQL, restoringObject);

                        db.ExecQuery(restoreSQLText, QueryResultTypes.NonQuery);
                    }
                    catch (Exception e)
                    {
                        // просто глушим возможные ошибки 
                        Trace.TraceError("При попытке восстановить объект\n{0}\nпроизошла ошибка: {1}", restoringObject, e.Message);
                    }
                }
                notCorrectCount = t.Rows.Count;
                passCount++;
            }

            // Ищем объекты в некорректном состоянии.
            DataTable dt = (DataTable)db.ExecQuery(testSQL, QueryResultTypes.DataTable);

            foreach (DataRow row in dt.Rows)
            {
                string restoringObject = Convert.ToString(row[0]);
                string typeOrTable = String.Empty;
                if (dt.Columns.Count > 1)
                    typeOrTable = Convert.ToString(row[1]);
                Trace.TraceError("Не удалось восстановить объект {0} {1}", typeOrTable, restoringObject);
            }
        }

        internal static void TestDatabase(IDatabase db)
        {
            TestAndRestoreObjects(db,
                "select Object_Name from DBA_Objects where Owner = 'DV' and Object_Type = 'TYPE' and Status = 'INVALID'",
                "alter type {0} compile");

            TestAndRestoreObjects(db,
                "select Object_Name from DBA_Objects where Owner = 'DV' and Object_Type = 'VIEW' and Status = 'INVALID'",
                "alter view {0} compile");

            TestAndRestoreObjects(db,
                "select Object_Name, Object_Type from DBA_Objects where Owner = 'DV' and Object_Type in ('FUNCTION', 'PROCEDURE') and Status = 'INVALID'",
                "alter {0} {1} compile");

            TestAndRestoreObjects(db,
                "select Object_Name from DBA_Objects where Owner = 'DV' and Object_Type = 'TRIGGER' and Status = 'INVALID' and Object_Name not like 'BIN$%'",
                "alter trigger {0} compile");
        }

        internal override string CheckReservedColumnName(string name)
        {
            if (ReservedWordsClass.ReservedWords.IndexOf(name.ToUpper()) >= 0)
                return string.Format("\"{0}\"", name.ToUpper());
            else
                return name;
        }

        /// <summary>
        /// Возвращает скрипт для создания индекса на поле таблицы.
        /// </summary>
        internal override List<string> CreateIndexScript(string tableName, string columnName, string indexName)
        {
            return CreateIndexScript(tableName, columnName, indexName, IndexTypes.NormalIndex);
        }

        internal override List<string> CreateIndexScript(string tableName, string columnName, string indexName, IndexTypes indexType)
        {
            List<string> script = new List<string>();

            string placementOptions = String.Format(" TABLESPACE {0} COMPUTE STATISTICS", "DVINDX");

            script.Add(String.Format("CREATE {0} INDEX {1} ON {2} ( {3} ) {4}",
                ( (indexType == IndexTypes.BitmapIndex) && bitmapIndexEnabled) ? "BITMAP" : (indexType == IndexTypes.Unique) ? "UNIQUE" : String.Empty,
                indexName,
                tableName,
                columnName,
                placementOptions));

            return script;
        }

        /// <summary>
        /// Возвращает скрипт для удаления индекса на внешний ключ.
        /// </summary>
        internal override List<string> DropReferenceIndexScript(string tableName, string indexName)
        {
            List<string> script = new List<string>();
            script.Add(String.Format("DROP INDEX {0}", indexName));
            return script;
        }

        internal override List<string> DropColumnScript(string tableName, string columnName)
        {
            List<string> script = new List<string>();
            script.Add(String.Format("ALTER TABLE {0} DROP COLUMN {1}", tableName, columnName));
            return script;
        }

        /// <summary>
        /// Создает значение по умолчанию для атрибута .
        /// </summary>
        internal override string CreateColumnDefaultValueScript(string tableName, string columnName, DataAttributeTypes columnType, object defaultValue)
        {
            return String.Empty;
        }

        /// <summary>
        /// Удаляет значение по умолчанию для атрибута .
        /// </summary>
        internal override string DropColumnDefaultValueScript(string tableName, string columnName)
        {
            return String.Empty;
        }
        
        /// <summary>
        /// Определяет режим работы сервера с базой данных.
        /// Если с базой данных уже работает какой либо сервер приложений, то режим будет MultiServerMode.
        /// </summary>
        internal override bool DetectMultiServerMode()
        {
            string machine = Environment.MachineName;
            if (!String.IsNullOrEmpty(Environment.UserDomainName))
            {
                machine = String.Format("{0}\\{1}", Environment.UserDomainName, machine);
            }

            int count = Convert.ToInt32(_db.ExecQuery(
                "select count(distinct machine || '$' || substr(process, 1, instr(process, ':', 1, 1) - 1)) from v$session " +
                "where (program = 'Krista.FM.Server.FMService.exe' or program = 'Krista.FM.Server.AppServer.exe') " +
                "and status <> 'KILLED' and machine <> ? and substr(process, 1, instr(process, ':', 1, 1) - 1) <> ?",
                QueryResultTypes.Scalar,
                _db.CreateParameter("Machine", machine),
                _db.CreateParameter("Process", System.Diagnostics.Process.GetCurrentProcess().Id, DbType.AnsiString)));
            return count > 0;
        }

        internal override string GetShortNamesExpression(int count, string memberName, ref string shortNameHeader, bool needDivide, int size)
        {
            string shortNameSelectPart = String.Empty;
            for (int i = 0; i < count; i++)
            {
                string fieldName = i == 0 ? "short_name" : String.Format("short_name_part{0}", i + 1);
                shortNameHeader += String.Format(", {0}", fieldName);
                shortNameSelectPart += String.Format(", substr(T.{0}, {1}, {2}) as {3}", memberName, i * 255 + 1, (i == 0 && !needDivide) ? size : 255,
                                                     fieldName);
            }

            return shortNameSelectPart;
        }
        
        internal override List<string> CreateUniqueConstraintScript(string tableName, string uniqueKeyName, List<string> fields)
        {
            List<string> script = new List<string>();
            string fieldsByComma = String.Join(", ", fields.ToArray());
            script.Add(String.Format("ALTER TABLE {0} ADD CONSTRAINT {1} UNIQUE ( {2} ) USING INDEX TABLESPACE {3}", tableName, uniqueKeyName, fieldsByComma, "DVINDX"));
            return script;
        }

        internal override List<string> DropUniqueConstraintScript(string tableName, string uniqueKeyName)
        {
            List<string> script = new List<string>();
            script.Add(String.Format("ALTER TABLE {0} DROP CONSTRAINT {1}", tableName, uniqueKeyName));
            return script;
        }

        internal override List<string> CreateUniqueConstraintHashFieldScript(string tableName, string hashFieldName, DataAttributeTypes attributeType, int attributeSize)
        {
            List<string> script = new List<string>();
            script.Add(String.Format("ALTER TABLE {0} ADD ({1} {2}({3}) )",
                        tableName, hashFieldName, dataTypeMappings[attributeType],attributeSize));
            return script;
        }

        internal override string CreateUniqueConstraintHashTriggerScript(string triggerName, string tableName, string hashFieldName, List<string> fields)
        {
            string fieldsForTriggerRaise = String.Join(",", fields.ToArray());
            
            string sourceIdSelectStatement;
            
            //ищем sourceid среди списка полей - вместо него нужно брать разыменованное значение из другой таблицы
            string findedSourceIdField = fields.Find(delegate(string st) { return st.ToLower() == "sourceid"; });
            
            List<string> fieldsForUnion;
            if (findedSourceIdField == null)
            {
                sourceIdSelectStatement = null;
                fieldsForUnion = fields;
            }
            else
            {
                fieldsForUnion = new List<string>(fields);
                fieldsForUnion.Remove(findedSourceIdField);
                sourceIdSelectStatement = "begin\n  select ds.datasourcename into l_datasourcename " +
                                         "from Datasources DS where DS.id = :new.sourceid;\n " +
                                 "exception\n  when no_data_found then l_datasourcename:=null;\n end;";
            }

            //Строка, которая будет подвержена хешированию
            string fieldsWithPrefixForUnion = "l_datasourcename||:new." + String.Join("||:new.", fieldsForUnion.ToArray());


            string script = String.Format("CREATE OR REPLACE TRIGGER {0} BEFORE INSERT OR UPDATE OF {1},{2} ON {3} REFERENCING NEW AS NEW OLD AS OLD FOR EACH ROW\n"+
                                            "DECLARE\n l_datasourcename varchar2(4000);\n"+
                                            "BEGIN\n {5}\n :new.{1} := upper(rawtohex(dbms_obfuscation_toolkit.md5(input => utl_raw.cast_to_raw({4}))));\nEND;",
                                           triggerName, 
                                           hashFieldName, 
                                           fieldsForTriggerRaise,
                                           tableName,
                                           fieldsWithPrefixForUnion,
                                           sourceIdSelectStatement
                                          );
            return script;
        }


        /// <summary>
        /// Определяет возможность использования bitmap-индексов в БД Oracle (зависит от Enterprise или Standard Edition).
        /// </summary>
        /// <returns> boolean </returns>
        public bool IsBitmapIndexEnabled(IDatabase db)
        {
            //т.к. в 9i наблюдается чрезмерный рост bitmap-индексов, то их использование в купе с существующими закачками нежелательно
            if ( OracleVersion < 10)
            {
                return false;
            }

            //Проверяем наличие данной опцию по системной вьюхе v_$option (сейчас гранты на неё даны через роль DBA)
            String bitmapEnableParam = Convert.ToString(db.ExecQuery(
                "select t.value from sys.v_$option t where t.parameter = ?",
                QueryResultTypes.Scalar,
                db.CreateParameter("param_name", "Bit-mapped indexes")));

            switch (bitmapEnableParam)
            {
                case "TRUE": return true;
                case "FALSE": return false;
                default: return false;
            }
        }


        /// <summary>
        /// Определяет номер версии сервера Oracle (8,9,10,11)
        /// </summary>
        public int OracleVersion
        {
            get
            {
                int result = 0;

                string version = String.Empty;

                IDbDataParameter prm1 = _db.CreateParameter("version", null, DbType.String, ParameterDirection.Output);
                prm1.Size = 100;

                IDbDataParameter prm2 = _db.CreateParameter("compatibility", null, DbType.String, ParameterDirection.Output);
                prm2.Size = 100;

                try
                {
                    _db.ExecQuery("dbms_utility.db_version",
                                  QueryResultTypes.StoredProcedure,
                                  prm1,
                                  prm2
                                  );
                    version = (String)prm1.Value;
                    result = Convert.ToInt32(version.Substring(0, version.IndexOf('.')));
                }
                catch (Exception e)
                {
                    Trace.TraceError("Не удалось определить версию сервера Oracle: {0}", e.StackTrace);
                    result = 0;
                }

                return result;
            }
        }
      
    }
}
