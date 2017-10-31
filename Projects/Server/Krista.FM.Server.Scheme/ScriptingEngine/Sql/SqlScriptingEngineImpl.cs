using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.ScriptingEngine.Sql
{
    internal class SqlScriptingEngineImpl : ScriptingEngineImpl
    {
        private static readonly Dictionary<DataAttributeTypes, string> dataTypeMappings;

        /// <summary>
        /// Конструктор типа
        /// </summary>
        static SqlScriptingEngineImpl()
        {
            dataTypeMappings = new Dictionary<DataAttributeTypes, string>();
            dataTypeMappings.Add(DataAttributeTypes.dtBoolean, "BIT");
            dataTypeMappings.Add(DataAttributeTypes.dtChar, "CHAR");
            dataTypeMappings.Add(DataAttributeTypes.dtDate, "DATETIME");
            dataTypeMappings.Add(DataAttributeTypes.dtDateTime, "DATETIME");
            dataTypeMappings.Add(DataAttributeTypes.dtDouble, "NUMERIC");
            dataTypeMappings.Add(DataAttributeTypes.dtInteger, "NUMERIC");
            dataTypeMappings.Add(DataAttributeTypes.dtString, "VARCHAR");
            dataTypeMappings.Add(DataAttributeTypes.dtBLOB, "VARBINARY");
        }

        public SqlScriptingEngineImpl(Database db)
            : base(db)
        {
        }

        internal override bool ExistsObject(string objectName, ObjectTypes objectType)
        {
            string schemaName = objectType == ObjectTypes.Sequence ? "G" : "DV";

            int schemaID = Convert.ToInt32(_db.ExecQuery("select schema_id from sys.schemas where Upper(name) = ?",
                QueryResultTypes.Scalar, _db.CreateParameter("Name", schemaName)));

            return 1 == Convert.ToInt32(_db.ExecQuery("select count(*) from DVDB_Objects where Schema_id = ? and Upper(name) = ? and type = ?",
                QueryResultTypes.Scalar,
                _db.CreateParameter("Schema_id", schemaID),
                _db.CreateParameter("ObjectName", objectName.ToUpper()),
                _db.CreateParameter("ObjectType", ObjectTypes2String(objectType))));
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
                case ObjectTypes.Procedure: return "P";
                case ObjectTypes.Sequence: return "U";
                case ObjectTypes.Table: return "U";
                case ObjectTypes.Trigger: return "TR";
                case ObjectTypes.View: return "V";
                case ObjectTypes.ForeignKeysConstraint: return "F";
                default: throw new Exception("Указан неизвестный тип объекта.");
            }
        }

        internal override List<string> CreateSequenceScript(string sequenceName, int seed, int increment)
        {
            List<string> script = new List<string>();
            script.Add(String.Format("CREATE TABLE g.{0} ( ID int identity({1}, {2}) not null )", sequenceName, seed, increment));
            return script;
        }

        internal override string DropTableScript(string tableName)
        {
            return String.Format("exec utility$removeRelationships @parent_table_name = '{0}';\n" +
            "DROP TABLE {1}", tableName, tableName);
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

            // Удаление индексов
            DataTable dtIndex =
                (DataTable)
                _db.ExecQuery("select  i_obj.name " +
                              "from    sys.indexes              i_obj " +
                              "join    sys.index_columns		ic on i_obj.object_id  = ic.object_id and i_obj.index_id = ic.index_id " +
                              "join    sys.columns              col on i_obj.object_id = col.object_id " +
                              "and ic.column_id = col.column_id " +
                              "where  i_obj.object_id = OBJECT_ID(?) and col.name = ?",
                              QueryResultTypes.DataTable,
                              _db.CreateParameter("TableName", tableName),
                              _db.CreateParameter("ColumnName", columnDefClause.Split(' ')[0]));

            foreach (DataRow row in dtIndex.Rows)
            {
                script.AddRange(DropReferenceIndexScript(tableName, row[0].ToString()));
            }

			script.Add(String.Format("ALTER TABLE {0} ALTER COLUMN {1}", tableName, columnDefClause));

            foreach (DataRow row in dtIndex.Rows)
            {
                script.AddRange(CreateIndexScript(tableName, columnDefClause.Split(' ')[0], row[0].ToString()));
            }

		    return script;
		}

		internal override string DropSequenceScript(string sequenceName)
        {
            return DropTableScript("g." + sequenceName);
        }

        internal override List<string> ParentKeyConstraintScript(string tableName, string constraintName, string refFieldName)
        {
            List<string> script = new List<string>();

            string onDeleteClause = "ON DELETE NO ACTION";

            script.Add(String.Format("ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ( {2} ) REFERENCES {0} ( ID ) {3}",
                tableName, constraintName, refFieldName, onDeleteClause));

            // TODO SS Сделать каскадное удаление родительского ключа этой же таблицы

            return script;
        }

        internal override int MaxIdentifierLength()
        {
            return 30;
        }

        public override string ConcatenateChar()
        {
            return "+";
        }

        public override string ParameterPrefixChar()
        {
            return "@";
        }

        private static string TriggerFireTypes2String(TriggerFireTypes triggerFireType)
        {
            switch (triggerFireType)
            {
                case TriggerFireTypes.Before: return "FOR";
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
            return String.Join(", ", result.ToArray());
        }

        internal override string CreateTriggerScript(string triggerName, string tableName, TriggerFireTypes triggerFireType, DMLEventTypes dmlEventType, string sqlDeclareStatements, string sqlStatements)
        {
            return String.Format("CREATE TRIGGER {0} ON {3} {1} {2} AS\nBEGIN\n{4}\nEND;",
                triggerName,
                TriggerFireTypes2String(triggerFireType),
                DMLEventTypes2String(dmlEventType),
                tableName,
                sqlStatements);
        }
        
        internal override List<string> CreateCascadeDeleteTriggerScript(string tableName, Dictionary<string, string> referencesList, string triggerName, string defaultValue)
        {
            List<string> script = new List<string>();

            List<string> statementsList = new List<string>();
            foreach (KeyValuePair<string, string> item in referencesList)
            {
                statementsList.Add(String.Format(
                    "update {0} set {1} = {2} where {1} in (select ID from deleted)",
                    item.Key,
                    item.Value,
                    defaultValue));
            }

            string sqlStatements = String.Format(
                "SET NOCOUNT ON;\n" +
                "{0}\n" +
                "delete from {1} where ID in (select ID from deleted)",
                String.Join("\n", statementsList.ToArray()),
                tableName);

            script.Add(CreateTriggerScript(triggerName,
                tableName,
                TriggerFireTypes.InsteadOf,
                DMLEventTypes.Delete,
                "",
                sqlStatements));

            return script;
        }

        internal override List<string> DropCascadeDeleteTriggerScript(string triggerName)
        {
            List<string> script = new List<string>();
            if (ExistsObject(triggerName, ObjectTypes.Trigger))
            {
                script.Add(DropTriggerScript(triggerName));
            }
            return script;
        }

        internal override List<string> CreateDeveloperLockTriggerScript(string tableName)
        {
            List<string> script = new List<string>();

            string sqlStatements = String.Format(
                "SET NOCOUNT ON;\n" +
                "declare @IsDevelop int\n" +
                "select @IsDevelop=cast(left(cast(CONTEXT_INFO() as varbinary(1)), 1) as int)\n" +
                "if (@IsDevelop = 0)\n" +
                "begin\n" +
                "	declare	@vI int, @vU int, @vD int\n" +
                "	select @vI=count(I.ID) from deleted D right join inserted I on D.ID = I.ID where D.ID is null and I.ID >= 1000000000\n" +
                "	select @vU=count(I.ID) from deleted D inner join inserted I on I.ID = D.ID where I.ID >= 1000000000\n" +
                "	select @vD=count(D.ID) from deleted D left  join inserted I on D.ID = I.ID where I.ID is null and D.ID >= 1000000000\n" +
                "	if (@vI > 0 or @vU > 0 or @vD > 0)\n" +
                "	begin\n" +
                "		RAISERROR ('Данные защищены от изменения', 16, 1);\n" +
                "		rollback transaction\n" +
                "	end\n" +
                "end\n");

            script.Add(String.Format("CREATE TRIGGER {0} ON {3} {1} {2} AS\nBEGIN\n{4}\nEND;",
                "t_" + tableName + "_ac",
                "AFTER",
                "INSERT,DELETE,UPDATE",
                tableName,
                sqlStatements));

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
            /*string s = "declare @nullsCount int;" +
            "set nocount on;" +
            "select @nullsCount = count(*) from inserted where ID is null;" +
            "if @nullsCount = 0 insert into PumpRegistry (ID, SupplierCode, DataCode, ProgramIdentifier, Name, ProgramConfig, StagesParams, Schedule, Comments) select ID, SupplierCode, DataCode, ProgramIdentifier, Name, ProgramConfig, StagesParams, Schedule, Comments from inserted;" +
            "else if @nullsCount = 1" +
            "begin" +
            "	insert into g.PumpRegistry default values;" +
            "	delete from g.PumpRegistry where ID = @@IDENTITY;" +
            "	insert into PumpRegistry (ID, SupplierCode, DataCode, ProgramIdentifier, Name, ProgramConfig, StagesParams, Schedule, Comments) select @@IDENTITY, SupplierCode, DataCode, ProgramIdentifier, Name, ProgramConfig, StagesParams, Schedule, Comments from inserted;" +
            "end else" +
            "	RAISERROR (500001, 1, 1);";*/
            return String.Format("if :new.ID is null then select {0}.NextVal into :new.ID from Dual; end if;\n",
                generatorName);
        }

        internal override string SqlBlockSetDefaultValue(string attributeName, string value)
        {
            return String.Empty;
        }

        internal override string GeneratorName(string tableName)
        {
            return /*"g." + */tableName.Substring(0, tableName.Length > 28 ? 28 : tableName.Length);
        }

        internal override string AutoIncrementColumnScript(string columnName)
        {
            return String.Format("{0} INT IDENTITY (1, 1) NOT NULL", columnName);
        }

        internal override string DataTypeMappings(DataAttributeTypes dataAttributeType)
        {
            return dataTypeMappings[dataAttributeType];
        }

        internal override string GetDataTypeScript(string name, DataAttributeClassTypes classType, DataAttributeTypes type, int size, int scale)
        {
            if (name == "ID" ||
                name == "RowType" ||
                name == "PumpID" ||
                name == "SourceID" ||
                name == "TaskID" ||
                name == "ParentID" ||
                name == "CubeParentID" ||
                name == "SourceKey" ||
                classType == DataAttributeClassTypes.Reference)
                return "INT";

            string sizeSlale = String.Empty;
            switch (type)
            {
                //case DataAttributeTypes.dtBoolean: // Для типа BIT размер указывать не нужно
                case DataAttributeTypes.dtInteger:
                case DataAttributeTypes.dtString:
                    sizeSlale = String.Format("({0})", size);
                    break;
                case DataAttributeTypes.dtDouble:
                    sizeSlale = String.Format("({0}, {1})", size, scale);
                    break;
                case DataAttributeTypes.dtBLOB:
                    sizeSlale = "(max)";
                    break;
            }
            return DataTypeMappings(type) + sizeSlale;
        }

        internal override List<string> EnableAllTriggersScript(string objectName)
        {
            List<string> script = new List<string>();
            script.Add(String.Format("ALTER TABLE {0} ENABLE TRIGGER ALL", objectName));
            return script;
        }

        internal override List<string> DisableAllTriggersScript(string objectName)
        {
            List<string> script = new List<string>();
            script.Add(String.Format("ALTER TABLE {0} DISABLE TRIGGER ALL", objectName));
            return script;
        }

        internal override List<string> CreateAuditTriggerScript(string triggerName, 
                                                                string tableName, 
                                                                bool excludeDataPamp,
                                                                string taskIdColumnName, 
                                                                string pumpIdColumnName, 
                                                                string logObjectName, 
                                                                int logObjectType
                                                                )
        {
            List<string> script = new List<string>();

            StringBuilder sqlStatement = new StringBuilder(
@"  SET NOCOUNT ON;
  DECLARE
    @UserName varchar(64),
    @SessionID varchar(24),
    @SessionContext varbinary(128);

  select @SessionContext = CONTEXT_INFO();
  set @SessionID = RTRIM(CAST(SUBSTRING(@SessionContext,2,24) as VARCHAR(24)));
  set @UserName = RTRIM(CAST(SUBSTRING(@SessionContext,26,64) as VARCHAR(64)));", 1000);

            if (excludeDataPamp)
            {
                sqlStatement.AppendLine(@"

  if (@UserName like 'Krista.FM.Server.DataPumps%')
  begin
    return;
  end;");
            }

            sqlStatement.AppendLine(@"

  if ( exists ( select null from deleted D right join inserted I on D.ID = I.ID where D.ID is null) )
  begin -- Insert
    insert into DVAudit.dataoperations(KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID, taskid, pumpid)");
            sqlStatement.AppendFormat(
"      select 0, '{0}', {1}, @UserName, @SessionID, I.id, {2}, {3} from inserted I;",
                logObjectName, logObjectType,
                (taskIdColumnName != null ? "I." + taskIdColumnName : "null"),
                (pumpIdColumnName != null ? "I." + pumpIdColumnName : "null")
                );
            sqlStatement.AppendLine(@"
  end
  else if ( exists ( select null from deleted D inner join inserted I on I.ID = D.ID ) )
  begin -- Update
    insert into DVAudit.dataoperations(KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID, taskid, pumpid)");
            sqlStatement.AppendFormat(
"      select 1, '{0}', {1}, @UserName, @SessionID, I.id, {2}, {3} from inserted I;",
                logObjectName, logObjectType,
                (taskIdColumnName != null ? "I." + taskIdColumnName : "null"),
                (pumpIdColumnName != null ? "I." + pumpIdColumnName : "null")
                );
            sqlStatement.AppendLine(@"
  end
  else if ( exists ( select null from deleted D left join inserted I on D.ID = I.ID where I.ID is null) )
  begin -- Delete
    insert into DVAudit.dataoperations(KindOfOperation, ObjectName, ObjectType, UserName, SessionID, RecordID, taskid, pumpid)");
            sqlStatement.AppendFormat(
"      select 2, '{0}', {1}, @UserName, @SessionID, D.id, {2}, {3} from deleted D;",
                logObjectName, logObjectType,
                (taskIdColumnName != null ? "D." + taskIdColumnName : "null"),
                (pumpIdColumnName != null ? "D." + pumpIdColumnName : "null")
                );
            sqlStatement.AppendLine(@"
  end;");
                
            script.Add(CreateTriggerScript(triggerName, tableName, TriggerFireTypes.After, DMLEventTypes.Insert | DMLEventTypes.Update | DMLEventTypes.Delete, "", sqlStatement.ToString()));

            script.Add(string.Format("sp_settriggerorder @triggername= '{0}', @order='Last', @stmttype = 'INSERT';", triggerName));
            script.Add(string.Format("sp_settriggerorder @triggername= '{0}', @order='Last', @stmttype = 'UPDATE';", triggerName));
            script.Add(string.Format("sp_settriggerorder @triggername= '{0}', @order='Last', @stmttype = 'DELETE';", triggerName));
            
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
            string triggerBody = String.Format(
                "SET NOCOUNT ON;\n" +
                "if ( exists ( select (I.ID) from deleted D right join inserted I on D.ID = I.ID where D.ID is null and\n" +
                "	exists (select V.ID from {0} V where V.ID = I.{1} and V.VariantCompleted = 1)))\n" +
                "begin -- Insert\n" +
                "	RAISERROR ('Вариант заблокирован.', 16, 1);\n" +
                "	rollback transaction\n" +
                "end\n" +
                "if ( exists ( select (I.ID) from deleted D inner join inserted I on I.ID = D.ID where\n" +
                "	exists (select V.ID from {0} V where V.ID = I.{1} and V.VariantCompleted = 1) or\n" +
                "	exists (select V.ID from {0} V where V.ID = D.{1} and V.VariantCompleted = 1)))\n" +
                "begin -- Update\n" +
                "	RAISERROR ('Вариант заблокирован.', 16, 1);\n" +
                "	rollback transaction\n" +
                "end\n" +
                "if ( exists ( select (D.ID) from deleted D left  join inserted I on D.ID = I.ID where I.ID is null and\n" +
                "	exists (select V.ID from {0} V where V.ID = D.{1} and V.VariantCompleted = 1)))\n" +
                "begin -- Delete\n" +
                "	RAISERROR ('Вариант заблокирован.', 16, 1);\n" +
                "	rollback transaction\n" +
                "end\n",
                variantTableName, refVariantColumnName);

            return String.Format(
                "CREATE TRIGGER {0} ON {1} AFTER INSERT, DELETE, UPDATE AS\nBEGIN\n{2}END",
                triggerName, tableName, triggerBody);
        }

        internal override string CreateDataSourceLockTriggerScript(string triggerName, string sourceIDColumnName, string tableName, string shortTableName, string dataSourceTableName)
        {
            string triggerBody = String.Format(
                "SET NOCOUNT ON;\n" +
                "if ( exists ( select (I.ID) from deleted D right join inserted I on D.ID = I.ID where D.ID is null and\n" +
                "	exists (select S.ID from {0} S where S.ID = I.{1} and S.Locked = 1)))\n" +
                "begin -- Insert\n" +
                "	RAISERROR ('Источник заблокирован.', 16, 1);\n" +
                "	rollback transaction\n" +
                "end\n" +
                "if ( exists ( select (I.ID) from deleted D inner join inserted I on I.ID = D.ID where\n" +
                "	exists (select S.ID from {0} S where S.ID = I.{1} and S.Locked = 1)) or\n" +
                "   exists ( select (I.ID) from deleted D inner join inserted I on I.ID = D.ID where\n" +
                "	exists (select S.ID from {0} S where S.ID = D.{1} and S.Locked = 1)))\n" +
                "begin -- Update\n" +
                "	RAISERROR ('Источник заблокирован.', 16, 1);\n" +
                "	rollback transaction\n" +
                "end\n" +
                "if ( exists ( select (D.ID) from deleted D left  join inserted I on D.ID = I.ID where I.ID is null and\n" +
                "	exists (select S.ID from {0} S where S.ID = D.{1} and S.Locked = 1)))\n" +
                "begin -- Delete\n" +
                "	RAISERROR ('Источник данных заблокирован от изменений.', 16, 1);\n" +
                "	rollback transaction\n" +
                "end\n",
                dataSourceTableName, sourceIDColumnName);

            return String.Format(
                "CREATE TRIGGER {0} ON {1} AFTER INSERT, DELETE, UPDATE AS\nBEGIN\n{2}END",
                triggerName, tableName, triggerBody);
        }


        internal override string CheckReservedColumnName(string name)
        {
            if (ReservedWordsClass.ReservedWords.IndexOf(name.ToUpper()) >= 0)
                return string.Format("[{0}]", name);
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

            string placementOptions = String.Empty; // TODO SS Опция хранения индекса

            //В SQLServer отсутствуют bitmap-индексы, поэтому вместо bitmap создаем обычный
            script.Add(String.Format(
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'{0}') AND name = N'{1}') CREATE {5} INDEX {1} ON {2} ( {3} ) {4}",
                tableName,
                indexName,
                tableName,
                columnName,
                placementOptions,
                (indexType == IndexTypes.Unique) ? "UNIQUE" : String.Empty
                )
                );
            return script;
        }

        /// <summary>
        /// Возвращает скрипт для удаления индекса на внешний ключ.
        /// </summary>
        internal override List<string> DropReferenceIndexScript(string tableName, string indexName)
        {
            List<string> script = new List<string>();
            script.Add(
                String.Format(
                    "IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'{0}') AND name = N'{1}') DROP INDEX [{2}] ON {3};",
                    tableName, indexName, indexName, tableName));
            return script;
        }

        internal override List<string> DropColumnScript(string tableName, string columnName)
        {
            List<string> script = new List<string>();

			// Перед удалением колонки необходимо сначала удалить все зависимые ограничения
            DataTable dt = (DataTable)_db.ExecQuery(
                "select  c_obj.name " +
                "from    sysobjects      c_obj " +
                "join    sysobjects      t_obj on c_obj.parent_obj = t_obj.id  " +
                "join    sysconstraints con on c_obj.id  = con.constid " +
                "join    syscolumns      col on t_obj.id = col.id " +
                "and con.colid = col.colid " +
                "where t_obj.name = ?  and col.name  = ?",
                QueryResultTypes.DataTable,
                _db.CreateParameter("TableName", tableName),
                _db.CreateParameter("ColumnName", columnName));

            foreach (DataRow row in dt.Rows)
            {
                script.Add(
                    String.Format(
                        "IF EXISTS (SELECT * FROM sysobjects WHERE parent_obj = OBJECT_ID(N'{0}') and name = N'{1}') ALTER TABLE {2} DROP CONSTRAINT {3}",
                        tableName, row[0], tableName, row[0]));
            }

            // Удаление индексов
            DataTable dtIndex =
                (DataTable)
                _db.ExecQuery("select  i_obj.name " +
                              "from    sys.indexes              i_obj " +
                              "join    sys.index_columns		ic on i_obj.object_id  = ic.object_id and i_obj.index_id = ic.index_id " +
                              "join    sys.columns              col on i_obj.object_id = col.object_id " +
							  "and ic.column_id = col.column_id " +
                              "where  i_obj.object_id = OBJECT_ID(?) and col.name = ?",
                              QueryResultTypes.DataTable,
                              _db.CreateParameter("TableName", tableName),
                              _db.CreateParameter("ColumnName", columnName));

            foreach (DataRow row in dtIndex.Rows)
            {
                script.AddRange(DropReferenceIndexScript(tableName, row[0].ToString()));
            }

            script.Add(String.Format("ALTER TABLE {0} DROP COLUMN {1}", tableName, columnName));
            return script;
        }

        /// <summary>
        /// Создает значение по умолчанию для атрибута .
        /// </summary>
        internal override string CreateColumnDefaultValueScript(string tableName, string columnName, DataAttributeTypes columnType, object defaultValue)
        {
            if (!String.IsNullOrEmpty(Convert.ToString(defaultValue)))
            {
                return String.Format("{0}ALTER TABLE {1} ADD DEFAULT {2} FOR {3}",
                    Database.KeyWord_OnErrorContinue, tableName, defaultValue, columnName);
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Удаляет значение по умолчанию для атрибута .
        /// </summary>
        internal override string DropColumnDefaultValueScript(string tableName, string columnName)
        {
            string script = string.Empty;
            string constraintName;

            constraintName = Convert.ToString(_db.ExecQuery(
                "select DF.name " +
                "from sys.objects O join sys.columns C on (O.object_id = C.object_id)" +
                "join sys.default_constraints DF  on (O.object_id = DF.parent_object_id and C.column_id = DF.parent_column_id)" +
                "where O.name = ? and C.name = ? and DF.type = 'D'",
                QueryResultTypes.Scalar,
                _db.CreateParameter("TableName", tableName),
                _db.CreateParameter("ColumnName", columnName)));

            if (constraintName != String.Empty)
                return String.Format("ALTER TABLE {0} DROP CONSTRAINT {1}", tableName, constraintName);
            else
                return String.Empty;
        }

        /// <summary>
        /// Определяет режим работы сервера с базой данных.
        /// Если с базой данных уже работает какой либо сервер приложений, то режим будет MultiServerMode.
        /// </summary>
        internal override bool DetectMultiServerMode()
        {
            //int count = Convert.ToInt32(_db.ExecQuery(
            //    "select count(distinct terminal || '$' || substr(process, 1, instr(process, ':', 1, 1) - 1)) from v$session " +
            //    "where (program = 'Krista.FM.Server.FMService.exe' or program = 'Krista.FM.Server.AppServer.exe') " +
            //    "and terminal <> ? and substr(process, 1, instr(process, ':', 1, 1) - 1) <> ?",
            //    QueryResultTypes.Scalar,
            //    _db.CreateParameter("Terminal", Environment.MachineName),
            //    _db.CreateParameter("Process", Process.GetCurrentProcess().Id, DbType.AnsiString)));
            return false;// count > 0;
        }

        internal override string GetShortNamesExpression(int count, string memberName, ref string shortNameHeader, bool needDivide, int size)
        {
            string shortNameSelectPart = String.Empty;
            for (int i = 0; i < count; i++)
            {
                string fieldName = i == 0 ? "short_name" : String.Format("short_name_part{0}", i + 1);
                shortNameHeader += String.Format(", {0}", fieldName);
                shortNameSelectPart += String.Format(", substring(T.{0}, {1}, {2}) as {3}", memberName, i * 255 + 1, (i == 0 && !needDivide) ? size : 255,
                                                     fieldName);
            }

            return shortNameSelectPart;
        }
        
        internal override List<string> CreateUniqueConstraintScript(string tableName, string uniqueKeyName, List<string> fields)
        {
            List<string> script = new List<string>();
            string fieldsByComma = String.Join(", ", fields.ToArray());
            script.Add(String.Format("ALTER TABLE {0} ADD CONSTRAINT {1} UNIQUE ( {2} )", tableName, uniqueKeyName, fieldsByComma));
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
            script.Add(String.Format("ALTER TABLE {0} ADD {1} {2}({3})",
                        tableName, hashFieldName, dataTypeMappings[attributeType],
                        attributeSize));
            return script;
        }

        internal override string CreateUniqueConstraintHashTriggerScript(string triggerName, string tableName, string hashFieldName, List<string> fields)
        {
            //Строка, которая будет подвержена хешированию
            string unionFieldsWithPrefix;

            //ищем sourceid среди списка полей - вместо него нужно брать разыменованное значение из другой таблицы
            string findedSourceIdField = fields.Find(delegate(string st) { return st.ToLower() == "sourceid"; });

            List<string> fieldsForUnion;
            if (findedSourceIdField == null)
            {
                unionFieldsWithPrefix = null;
                fieldsForUnion = fields;
            }
            else
            {
                unionFieldsWithPrefix = "(select DS.datasourcename from Datasources DS where DS.id = I.sourceid)+";

                fieldsForUnion = new List<string>(fields);
                fieldsForUnion.Remove(findedSourceIdField);
            }

            for (int i = 0; i < fieldsForUnion.Count; i++)
            {
                fieldsForUnion[i] = string.Format("cast(I.{0} as varchar)", fieldsForUnion[i]);
            }
            
           
            unionFieldsWithPrefix += String.Join("+",fieldsForUnion.ToArray());

            string script = String.Format("CREATE TRIGGER {0} ON {1} AFTER INSERT, UPDATE  AS\n" +
                                            "BEGIN\n  SET NOCOUNT ON;\n  UPDATE T\n  SET T.{2} = UPPER(SUBSTRING(master.dbo.fn_varbintohexstr(HashBytes('MD5',{3})), 3, 32))\n  " +
                                            "FROM {1} T INNER JOIN inserted I on (T.id = I.id);\nEND;",
                                           triggerName,
                                           tableName,
                                           hashFieldName,
                                           unionFieldsWithPrefix
                                          );
            return script;
        }
    }
}
