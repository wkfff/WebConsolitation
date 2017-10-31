using System;
using System.Collections.Generic;
using System.ComponentModel;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.ScriptingEngine
{
    internal abstract class ScriptingEngineImpl
    {
        protected Database _db;

        public ScriptingEngineImpl(Database db)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            _db = db;
        }

        internal static string DeveloperLockTriggerName(string tableName)
        {
            return "t_" + tableName + "_ac";
        }

        internal abstract bool ExistsObject(string objectName, ObjectTypes objectType);

        internal abstract List<string> CreateSequenceScript(string sequenceName, int seed, int increment);

		/// <summary>
		/// Возвращает скрипт для создания таблицы с указанными параметрами.
		/// </summary>
		internal virtual string CreateTableScript(string tableName, string primaryKey, List<string> columnList)
		{
			return String.Format("CREATE TABLE {0}\n(\n\t{1},\n\tCONSTRAINT {2} PRIMARY KEY ( ID )\n)",
				tableName, String.Join(",\n\t", columnList.ToArray()), primaryKey);
		}
		
		/// <summary>
		/// Возвращает скрипт для удаления указанной таблицы.
		/// </summary>
		internal abstract string DropTableScript(string tableName);

    	/// <summary>
    	/// Возвращает скрипт для создания представления с указанными параметрами.
    	/// </summary>
    	internal abstract string CreateViewScript(string viewName, string viewBody);

		/// <summary>
		/// Возвращает скрипт для удаления указанного представления.
		/// </summary>
		internal abstract string DropViewScript(string viewName);

		/// <summary>
		/// Возвращает скрипт для создания внешнего ключа с указанными параметрами.
		/// </summary>
		internal virtual string CreateForeignKeyScript(string tableName, string constraintName, string foreignKeyName, string referenceTableName, string onDeleteActionClause)
		{
			return String.Format(
				"ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY ( {2} ) REFERENCES {3} ( ID ) {4}",
				tableName, constraintName, foreignKeyName, referenceTableName, onDeleteActionClause);
		}

		/// <summary>
		/// Возвращает скрипт для удаления ограничения из указанной таблицы.
		/// </summary>
		internal virtual string DropConstraintScript(string tableName, string constraintName)
		{
			return String.Format("ALTER TABLE {0} DROP CONSTRAINT {1}", tableName, constraintName);
		}

		/// <summary>
		/// Возвращает скрипт для создания столбца в указанной таблице.
		/// </summary>
		internal virtual string CreateColumnScript(string tableName, string columnDefClause)
		{
			return String.Format("ALTER TABLE {0} ADD {1}", tableName, columnDefClause);
		}

    	/// <summary>
    	/// Возвращает скрипт для изменения столбца в указанной таблице.
    	/// </summary>
    	internal abstract List<string> ModifyColumnScript(string tableName, string columnDefClause);

		/// <summary>
		/// Возвращает скрипт для удаления поля таблицы.
		/// </summary>
		internal abstract List<string> DropColumnScript(string tableName, string columnName);

		/// <summary>
		/// Устанавливает значение столбца.
		/// </summary>
		internal virtual string UpdateColumnSetValueScript(string tableName, string columnName, string defaultValue, string whereClause)
		{
			return String.Format("UPDATE {0} set {1} = {2}{3}{4}", 
				tableName, 
				columnName, 
				defaultValue,
				String.IsNullOrEmpty(whereClause) ? String.Empty : " where ", 
				whereClause);
		}

		/// <summary>
		/// Возвращает скрипт для удаления указанной последовательности.
		/// </summary>
		internal abstract string DropSequenceScript(string sequenceName);

        /// <summary>
        /// Возвращает скрипт для создания индекса по указанному столбцу указанной таблицы.
        /// </summary>
        internal abstract List<string> CreateIndexScript(string tableName, string columnName, string indexName);
        internal abstract List<string> CreateIndexScript(string tableName, string columnName, string indexName, IndexTypes indexType);
        
		/// <summary>
        /// Возвращает скрипт для удаления индекса на внешний ключ.
        /// </summary>
        internal abstract List<string> DropReferenceIndexScript(string tableName, string indexName);

        internal abstract List<string> ParentKeyConstraintScript(string tableName, string constraintName, string refFieldName);

        internal abstract int MaxIdentifierLength();

        public abstract string ConcatenateChar();

        public abstract string ParameterPrefixChar();

        internal abstract string CreateTriggerScript(string triggerName, string tableName, TriggerFireTypes triggerFireType, DMLEventTypes dmlEventType, string sqlDeclareStatements, string sqlStatements);

        internal abstract List<string> CreateDeveloperLockTriggerScript(string tableName);

        internal abstract List<string> DropDeveloperLockTriggerScript(string tableName);

        internal abstract string CreateVariantLockTriggerScript(string triggerName, string refVariantColumnName, string tableName, string shortTableName, string variantTableName);

        internal abstract string CreateDataSourceLockTriggerScript(string triggerName, string soureceIDColumnName, string tableName, string shortTableName, string dataSourceTableName);

        internal abstract List<string> CreateAuditTriggerScript(string triggerName, string tableName, bool excludeDataPamp, string taskIdColumnName, string pumpIdColumnName, string logObjectName, int logObjectType);

        internal abstract string DropTriggerScript(string triggerName);

        internal abstract string DropProcedureScript(string procedureName);

        internal abstract List<string> CreateCascadeDeleteTriggerScript(string tableName, Dictionary<string, string> referencesList, string triggerName, string defaultValue);

        internal abstract List<string> DropCascadeDeleteTriggerScript(string tiggerName);

        internal abstract string GetIDFromGeneratorScript(string generatorName);

        internal abstract string SqlBlockSetDefaultValue(string attributeName, string value);

        internal abstract string GeneratorName(string tableName);

        /// <summary>
        /// Возвращает определения автоинкрементного поля
        /// </summary>
        /// <param name="columnName">Имя поля</param>
        internal abstract string AutoIncrementColumnScript(string columnName);

        internal abstract string DataTypeMappings(DataAttributeTypes dataAttributeType);

        internal abstract string GetDataTypeScript(string name, DataAttributeClassTypes classType, DataAttributeTypes type, int size, int scale);

        /// <summary>
        /// Активизация триггеров объекта базы данных.
        /// </summary>
        /// <param name="objectName">Имя объекта базы данных.</param>
        /// <returns>Скрипт активизирующий триггера.</returns>
        internal abstract List<string> EnableAllTriggersScript(string objectName);

        /// <summary>
        /// Деактивизация триггеров объекта базы данных.
        /// </summary>
        /// <param name="objectName">Имя объекта базы данных.</param>
        /// <returns>Скрипт деактивизирующий триггера.</returns>
        internal abstract List<string> DisableAllTriggersScript(string objectName);

        /// <summary>
        /// Возвращает безопасное имя столбца.
        /// </summary>
        /// <param name="name">Имя столбца.</param>
        /// <returns>Безопасное имя столбца.</returns>
        internal abstract string CheckReservedColumnName(string name);

        internal static string GetDefaultValue(DataAttributeTypes type, object defaultValue)
        {
            if (!String.IsNullOrEmpty(Convert.ToString(defaultValue)))
            {
                string value;
                switch (type)
                {
                    case DataAttributeTypes.dtBoolean:
                    case DataAttributeTypes.dtInteger:
                        value = Convert.ToString(Convert.ToInt32(defaultValue));
                        break;
					case DataAttributeTypes.dtString:
                		{
							if (Convert.ToString(defaultValue).StartsWith("'") && Convert.ToString(defaultValue).EndsWith("'"))
							{
								value = Convert.ToString(defaultValue);
							}
							else
							{
								value = "'" + Convert.ToString(defaultValue) + "'";
							}
                			break;
                		}
                	case DataAttributeTypes.dtChar:
                        if (Convert.ToString(defaultValue).StartsWith("'") && Convert.ToString(defaultValue).EndsWith("'"))
                        {
                            value = Convert.ToString(defaultValue);
                        }
                        else
                        {
                            value = "'" + Convert.ToString(Convert.ToDouble(defaultValue)) + "'";
                        }
                        break;
                    case DataAttributeTypes.dtDouble:
                        value = Convert.ToString(Convert.ToDouble(defaultValue)).Replace(',', '.');
                        break;
                    case DataAttributeTypes.dtDate:
                    case DataAttributeTypes.dtDateTime:
                        value = "'" + Convert.ToString(Convert.ToDateTime(defaultValue)) + "'";
                        break;
                    default:
                        throw new Exception("Значение по умолчания для типа ??? не поддерживается.");
                }
                return value;
            }
            else
                return String.Empty;
        }

        internal static string GetDefaultClauseScript(DataAttributeTypes type, object defaultValue)
        {
            if (String.IsNullOrEmpty(Convert.ToString(defaultValue)))
                return " DEFAULT NULL";
            else
                return " DEFAULT " + GetDefaultValue(type, defaultValue);
        }

        /// <summary>
        /// Создает значение по умолчанию для атрибута .
        /// </summary>
        internal abstract string CreateColumnDefaultValueScript(string tableName, string columnName, DataAttributeTypes columnType, object defaultValue);

        /// <summary>
        /// Удаляет значение по умолчанию для атрибута .
        /// </summary>
        internal abstract string DropColumnDefaultValueScript(string tableName, string columnName);

        /// <summary>
        /// Определяет режим работы сервера с базой данных.
        /// Если с базой данных уже работает какой либо сервер приложений, то режим будет MultiServerMode.
        /// </summary>
        internal abstract bool DetectMultiServerMode();

        /// <summary>
        /// Возвращает выражение со списком коротких имен для создания представления 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        internal abstract string GetShortNamesExpression(int count, string memberName, ref string shortNameHeader, bool needDivide, int size);
        
        internal abstract List<string> CreateUniqueConstraintScript(string tableName, string uniqueKeyName, List<string> fields);
        internal abstract List<string> DropUniqueConstraintScript(string tableName, string uniqueKeyName);
        
        /// <summary>
        ////Возвращает скрипт для добавления к таблице системного поля с хэшем и уникального индекса по нему
        /// </summary>
        internal abstract List<string> CreateUniqueConstraintHashFieldScript(string tableName, string hashFieldName, DataAttributeTypes attributeType, int attributeSize);

        /// <summary>
        ////Возвращает скрипт для добавления к таблице триггера, который будет вычислять хэш
        /// </summary>
        internal abstract string CreateUniqueConstraintHashTriggerScript(string triggerName, string tableName, string hashFieldName, List<string> fields);

    }

    /// <summary>
    /// Типы индексов.
    /// </summary>
    public enum IndexTypes
    {
        /// <summary>
        /// Обычный B*Tree индекс
        /// </summary>
        [Description("Обычный B*Tree-индекс")]
        NormalIndex = 0,

        /// <summary>
        /// Bitmap-индекс (Oracle)
        /// </summary>
        [Description("Bitmap-индекс (Oracle)")]
        BitmapIndex = 1,

        /// <summary>
        ////Уникальный индекс
        /// </summary>
        Unique = 2
        
    }
}
