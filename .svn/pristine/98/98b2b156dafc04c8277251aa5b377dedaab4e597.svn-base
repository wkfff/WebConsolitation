using System;
using System.Collections.Generic;

using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.ScriptingEngine.Classes
{
    /// <summary>
    /// Общие методы для работы с атрибутами.
    /// </summary>
    internal class AttributeScriptingEngine : ScriptingEngineAbstraction
    {
        /// <summary>
        /// Инициализация экземпляра.
        /// </summary>
        /// <param name="impl">Реализация СУБД зависимого функционала.</param>
        internal AttributeScriptingEngine(ScriptingEngineImpl impl)
            : base(impl)
        {
        }

        /// <summary>
        /// Преобразует значение параметра в зависимости от его типа.
        /// </summary>
        /// <param name="obj">Значение параметра.</param>
        /// <returns>Преобразованное значение параметра.</returns>
        internal static string Value2QueryConstantParameter(object obj)
        {
            return obj is string ? String.Format("'{0}'", obj) : Convert.ToString(obj);
        }

        internal static string GetDefaultValue(DataAttributeTypes type, object defaultValue)
        {
            return ScriptingEngineImpl.GetDefaultValue(type, defaultValue);
        }

        internal static string GetNotNullScript(bool isNullable)
        {
            return isNullable ? " NULL" : " NOT NULL";
        }

        internal string GetDataTypeScript(EntityDataAttribute attribute)
        {
            if (attribute.Class == DataAttributeClassTypes.Reference && attribute.Size == 0)
                attribute.Size = 10;

            return _impl.GetDataTypeScript(attribute.Name, attribute.Class, attribute.Type, attribute.Size, attribute.Scale);
        }

        internal string ColumnNameCheckReserved(string name)
        {
            return _impl.CheckReservedColumnName(name);
        }

        internal string DataTypeMappings(DataAttributeTypes dataAttributeType)
        {
            return _impl.DataTypeMappings(dataAttributeType);
        }

        internal string ColumnScript(EntityDataAttribute attr, bool withNullClause)
        {
            if (withNullClause)
                return String.Format("{0} {1}{2}", 
                    ColumnNameCheckReserved(attr.Name),
                    GetDataTypeScript(attr) + ScriptingEngineImpl.GetDefaultClauseScript(attr.Type, attr.DefaultValue), 
                    GetNotNullScript(attr.IsNullable));
            else
                return String.Format("{0} {1}", ColumnNameCheckReserved(attr.Name),
                    GetDataTypeScript(attr) + ScriptingEngineImpl.GetDefaultClauseScript(attr.Type, attr.DefaultValue));
        }

        /// <summary>
        /// Устанавливает значение по умолчанию для атрибута.
        /// </summary>
		internal void UpdateTableSetDefaultValue(EntityDataAttribute attr, string tableName, string whereClause, List<string> script)
        {
			UpdateTableSetDefaultValue(
				attr, 
				tableName, 
				ScriptingEngineImpl.GetDefaultValue(attr.Type, attr.DefaultValue), 
				whereClause, 
				script);
        }

		/// <summary>
		/// Устанавливает значение по умолчанию для атрибута.
		/// </summary>
		internal void UpdateTableSetDefaultValue(EntityDataAttribute attr, string tableName, object value, string whereClause, List<string> script)
		{
			script.Add(_impl.UpdateColumnSetValueScript(
				tableName,
				attr.Name,
				ScriptingEngineImpl.GetDefaultValue(attr.Type, value),
				whereClause));
		}

		internal List<string> AddScript(EntityDataAttribute attr, Entity entity, bool withNullClause, bool generateDependendScripts)
        {
            List<string> script = new List<string>();

			script.Add(_impl.CreateColumnScript(entity.FullDBName, ColumnScript(attr, false)));

            // Если атрибут обязательный и у него есть значение по умолчанию
            if (withNullClause && !attr.IsNullable)
            {
                // устанавливаем значения по умолчанию
                if (attr.DefaultValue != null)
                    attr.UpdateTableSetDefaultValue(entity, script);

                // устанавливаем значения по умолчанию для системных записей
                attr.UpdateSystemRowsSetDefaultValue(entity, script);

                // устанавливаем модификатор обязательности
                script.AddRange(ModifyScript(attr, entity, true, withNullClause, false));
            }

            if (generateDependendScripts)
                script.AddRange(entity.GetDependentScripts());

            return script;
        }

        private string ModifyClause(EntityDataAttribute attr, bool withTypeModification, bool withNullClause)
        {
            bool withDefaultClause = 
				SchemeClass.Instance.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleClient || 
				SchemeClass.Instance.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleDataAccess ||
				SchemeClass.Instance.SchemeDWH.FactoryName == ProviderFactoryConstants.MSOracleDataAccess;

            if (SchemeClass.Instance.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleClient ||
				SchemeClass.Instance.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleDataAccess ||
				SchemeClass.Instance.SchemeDWH.FactoryName == ProviderFactoryConstants.MSOracleDataAccess)
            {
                if (withTypeModification && attr.Type == DataAttributeTypes.dtBLOB)
                {
                    withTypeModification = false;
                }
            }
            else if (SchemeClass.Instance.SchemeDWH.FactoryName == ProviderFactoryConstants.SqlClient)
            {
                withTypeModification = true;
            }
            else
                throw new Exception();

            string nullClause = withNullClause || SchemeClass.Instance.SchemeDWH.FactoryName == ProviderFactoryConstants.SqlClient ? GetNotNullScript(attr.IsNullable) : String.Empty;
            return String.Format("{0} {1}{2}{3}", 
                attr.Name,
                (withTypeModification ? GetDataTypeScript(attr) : String.Empty),
                (withDefaultClause ? ScriptingEngineImpl.GetDefaultClauseScript(attr.Type, attr.DefaultValue) : String.Empty), 
                nullClause);
        }

        internal List<string> ModifyScript(EntityDataAttribute attr, Entity entity, bool withTypeModification, bool withNullClause, bool generateDependendScripts)
        {
            List<string> script = new List<string>();

            script.Add(_impl.DropColumnDefaultValueScript(entity.FullDBName, attr.Name));
        	script.AddRange(_impl.ModifyColumnScript(entity.FullDBName, ModifyClause(attr, withTypeModification, withNullClause)));
            script.Add(_impl.CreateColumnDefaultValueScript(entity.FullDBName, attr.Name, attr.Type, attr.DefaultValue));

            if (generateDependendScripts)
                script.AddRange(entity.GetDependentScripts());

            return script;
        }

        internal List<string> DropScript(EntityDataAttribute attr, Entity entity)
        {
            List<string> script = new List<string>();
            script.AddRange(_impl.DropColumnScript(entity.FullDBName, attr.Name));
            script.AddRange(entity.GetDependentScripts(attr));
            return script;
        }

        #region Реализация абстрактных методов

        internal override List<string> CreateScript(ICommonDBObject obj)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override List<string> DropScript(ICommonDBObject obj)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        internal override List<string> CreateDependentScripts(IEntity entity, IDataAttribute withoutAttribute)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion Реализация абстрактных методов
    }
}
