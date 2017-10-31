using System;
using System.Collections.Generic;

using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Classes;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.ScriptingEngine
{
	/// <summary>
	/// Данная неализация используется в режиме без генерации скриптов базы данных.
	/// </summary>
	internal class NullScriptingEngineImpl : ScriptingEngineImpl
	{
		private static readonly Dictionary<DataAttributeTypes, string> dataTypeMappings;

        /// <summary>
        /// Конструктор типа
        /// </summary>
		static NullScriptingEngineImpl()
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

		public NullScriptingEngineImpl(Database db)
            : base(db)
        {
        }

		internal override bool ExistsObject(string objectName, ObjectTypes objectType)
		{
			return false;
		}

		internal override List<string> CreateSequenceScript(string sequenceName, int seed, int increment)
		{
			return new List<string>();
		}

		internal override string CreateTableScript(string tableName, string primaryKey, List<string> columnList)
		{
			return String.Empty;
		}

		internal override string DropTableScript(string tableName)
		{
			return String.Empty;
		}

		internal override string CreateViewScript(string viewName, string viewBody)
		{
			return String.Empty;
		}

		internal override string DropViewScript(string viewName)
		{
			return String.Empty;
		}

		internal override string CreateForeignKeyScript(string tableName, string constraintName, string foreignKeyName, string referenceTableName, string onDeleteActionClause)
		{
			return String.Empty;
		}

		internal override string DropConstraintScript(string tableName, string constraintName)
		{
			return String.Empty;
		}

		internal override string CreateColumnScript(string tableName, string columnDefClause)
		{
			return String.Empty;
		}

        internal override List<string> ModifyColumnScript(string tableName, string columnDefClause)
		{
			return new List<string>();
		}

		internal override List<string> DropColumnScript(string tableName, string columnName)
		{
			return new List<string>();
		}

		internal override string UpdateColumnSetValueScript(string tableName, string columnName, string defaultValue, string whereClause)
		{
			return String.Empty;
		}

		internal override string DropSequenceScript(string sequenceName)
		{
			return String.Empty;
		}

	    internal override List<string> CreateIndexScript(string tableName, string columnName, string indexName)
	    {
            return new List<string>();
	    }

	    internal override List<string> CreateIndexScript(string tableName, string columnName, string indexName, IndexTypes indexType)
	    {
            return new List<string>();
	    }

	    internal override List<string> DropReferenceIndexScript(string tableName, string indexName)
		{
			return new List<string>();
		}

		internal override List<string> ParentKeyConstraintScript(string tableName, string constraintName, string refFieldName)
		{
			return new List<string>();
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
			return ":";
		}

		internal override string CreateTriggerScript(string triggerName, string tableName, TriggerFireTypes triggerFireType, DMLEventTypes dmlEventType, string sqlDeclareStatements, string sqlStatements)
		{
			return String.Empty;
		}

		internal override List<string> CreateDeveloperLockTriggerScript(string tableName)
		{
			return new List<string>();
		}

		internal override List<string> DropDeveloperLockTriggerScript(string tableName)
		{
			return new List<string>();
		}

		internal override string CreateVariantLockTriggerScript(string triggerName, string refVariantColumnName, string tableName, string shortTableName, string variantTableName)
		{
			return String.Empty;
		}

		internal override string CreateDataSourceLockTriggerScript(string triggerName, string soureceIDColumnName, string tableName, string shortTableName, string dataSourceTableName)
		{
			return String.Empty;
		}

        internal override List<string> CreateAuditTriggerScript(string triggerName, string tableName, bool excludeDataPamp, string taskIdColumnName, string pumpIdColumnName, string logObjectName, int logObjectType)
	    {
            return new List<string>();
	    }

	    internal override string DropTriggerScript(string triggerName)
		{
			return String.Empty;
		}

		internal override string DropProcedureScript(string procedureName)
		{
			return String.Empty;
		}

		internal override List<string> CreateCascadeDeleteTriggerScript(string tableName, Dictionary<string, string> referencesList, string triggerName, string defaultValue)
		{
			return new List<string>();
		}

		internal override List<string> DropCascadeDeleteTriggerScript(string tiggerName)
		{
			return new List<string>();
		}

		internal override string GetIDFromGeneratorScript(string generatorName)
		{
			return String.Empty;
		}

		internal override string SqlBlockSetDefaultValue(string attributeName, string value)
		{
			return String.Empty;
		}

		internal override string GeneratorName(string tableName)
		{
			return "g_" + tableName.Substring(0, tableName.Length > MaxIdentifierLength() - 2 ? MaxIdentifierLength() - 2 : tableName.Length);
		}

		internal override string AutoIncrementColumnScript(string columnName)
		{
			return String.Empty;
		}

		internal override string DataTypeMappings(DataAttributeTypes dataAttributeType)
		{
			return dataTypeMappings[dataAttributeType];
		}

		internal override string GetDataTypeScript(string name, DataAttributeClassTypes classType, DataAttributeTypes type, int size, int scale)
		{
			return String.Empty;
		}

		internal override List<string> EnableAllTriggersScript(string objectName)
		{
			return new List<string>();
		}

		internal override List<string> DisableAllTriggersScript(string objectName)
		{
			return new List<string>();
		}

		internal override string CheckReservedColumnName(string name)
		{
			return name;
		}

		internal override string CreateColumnDefaultValueScript(string tableName, string columnName, DataAttributeTypes columnType, object defaultValue)
		{
			return String.Empty;
		}

		internal override string DropColumnDefaultValueScript(string tableName, string columnName)
		{
			return String.Empty;
		}

		internal override bool DetectMultiServerMode()
		{
			return true;
		}

        internal override string GetShortNamesExpression(int count, string memberName, ref string shortNameHeader, bool needDivide, int size)
	    {
	        return String.Empty;
	    }
	     internal override List<string> CreateUniqueConstraintScript(string tableName, string uniqueKeyName, List<string> fields)
	    {
            return new List<string>();
	    }

        internal override List<string> DropUniqueConstraintScript(string tableName, string uniqueKeyName)
        {
            return new List<string>();
        }

	    internal override List<string> CreateUniqueConstraintHashFieldScript(string tableName, string hashFieldName, DataAttributeTypes attributeType, int attributeSize)
	    {
            return new List<string>();
        }

	    internal override string CreateUniqueConstraintHashTriggerScript(string triggerName, string tableName, string hashFieldName, List<string> fields)
	    {
            return String.Empty;
	    }

	}
}
