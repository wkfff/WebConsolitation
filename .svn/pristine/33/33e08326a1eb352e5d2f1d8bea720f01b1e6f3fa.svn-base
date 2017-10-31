using System;
using System.Collections.Generic;

using Krista.FM.Server.Scheme.Classes;
using Krista.FM.Server.Scheme.ScriptingEngine.Sql;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.ScriptingEngine.Classes
{
    internal class EntityScriptingEngine : ScriptingEngineAbstraction
    {
        internal EntityScriptingEngine(ScriptingEngineImpl impl)
            : base(impl)
        {
        }

        /// <summary>
        /// Возвращает наименование генератора (FullDBName обрезается до 28 символов)
        /// </summary>
        internal string GeneratorName(string tableName)
        {
            return _impl.GeneratorName(tableName);
        }

        private static string PrimaryKeyConstraintName(string tableName)
        {
            return "PK" + tableName.Substring(0, tableName.Length > 28 ? 28 : tableName.Length);
        }

		private static string TriggerNamePart(Entity entity)
		{
			if (entity.FullDBName.Length > 25)
				return entity.FullDBShortName.Substring(0, entity.FullDBShortName.Length > 25 ? 25 : entity.FullDBShortName.Length);
			else
				return entity.FullDBName.Substring(0, entity.FullDBName.Length > 25 ? 25 : entity.FullDBName.Length);
		}

		internal static string TriggerNamePart(string tableName, string tableShortName)
        {
            if (tableName.Length > 25)
                return tableShortName.Substring(0, tableShortName.Length > 25 ? 25 : tableShortName.Length);
            else
                return tableName.Substring(0, tableName.Length > 25 ? 25 : tableName.Length);
        }

        private static string IndexName(string tableName, string columnName)
        {
            String indexName = "I_" + tableName + columnName;
            return indexName.Substring(0, indexName.Length > 30 ? 30 : indexName.Length);
        }

        /// <summary>
        /// Возвращает опциональную часть скрипта для создания отношения
        /// </summary>
        protected virtual List<string> CustomTableConstraintsScript(Entity entity)
        {
            return new List<string>();
        }

        /// <summary>
        /// Часть скрипта по созданию колонки таблицы
        /// </summary>
        /// <param name="attr">Колонка</param>
        /// <returns>Скрипт</returns>
        protected virtual string CreateTableColumnScript(EntityDataAttribute attr)
        {
            return EntityDataAttribute.ScriptingEngine.ColumnScript(attr, true);
        }

        private string CreateTableScript(IEntity entity)
        {
            // Определения атрибутов
            List<string> columnList = new List<string>();
            foreach (EntityDataAttribute attr in entity.Attributes.Values)
            {
                // атрибуты-ссылки не учитываем
                if (attr.Class == DataAttributeClassTypes.Reference)
                    continue;

                // Хэш-поле уникального ключа не учитываем
                if ( (attr.Class == DataAttributeClassTypes.System) && (attr.Name == DataAttribute.SystemHashUK.Name) )
                    continue;

                columnList.Add(CreateTableColumnScript(attr));
            }

            // Определение уникальных атрибутов
            // ...

			return _impl.CreateTableScript(
				entity.FullDBName, 
				PrimaryKeyConstraintName(entity.FullDBName), 
				columnList);
        }

        protected virtual List<string> CreateSequenceScript(Entity entity)
        {
            return _impl.CreateSequenceScript(GeneratorName(entity.FullDBName), 1, 1);
        }

        private static List<string> CreateAuditTriggersScript(Entity entity, DataAttribute withoutAttribute)
        {
            List<string> script = new List<string>();

            // TODO Триггера для аудита

            return script;
        }

        protected virtual List<string> CreateTriggersScript(Entity entity, DataAttribute withoutAttribute)
        {
            List<string> script = new List<string>();

            // TODO Триггера для SqlServer
            if (_impl is SqlScriptingEngineImpl)
            {
                if (this is ClassifierEntityScriptingEngine)
                {
                    script.AddRange(((ClassifierEntityScriptingEngine)this).GetCascadeDeleteTrigerScript(entity));
                }
                return script;
            }

            List<string> insertSqlStatements = new List<string>();
            List<string> updateSqlStatements = new List<string>();
            
            List<string> setDefaultRefValues = new List<string>();
            // Утановка значений по-умолчанию для пользовательских атрибутов
            foreach (EntityDataAttribute attribute in entity.Attributes.Values)
            {
                // Пропускаем атрибут, который не нужно учитывать
                if (attribute == withoutAttribute)
                    continue;

                // Обрабатываем только обязательные атрибуты, у которых есть значение по умолчанию
                if (attribute.IsNullable || attribute.DefaultValue == null || attribute.Class == DataAttributeClassTypes.Reference)
                    continue;

                setDefaultRefValues.Add(_impl.SqlBlockSetDefaultValue(attribute.Name, attribute.GetDefaultValue));
            }

            // Утановка значений по-умолчанию для атрибутов ссылок
            foreach (EntityAssociation association in entity.Associations.Values)
            {
                // Пропускаем атрибут, который не нужно учитывать
                if (association.FullDBName == withoutAttribute.Name)
                    continue;

                if ((association.AssociationClassType == AssociationClassTypes.Link ||
                    association.AssociationClassType == AssociationClassTypes.Bridge ||
                    association.AssociationClassType == AssociationClassTypes.BridgeBridge)
                    && (association.DbObjectState == DBObjectStateTypes.InDatabase ||
                        (association.DbObjectState == DBObjectStateTypes.New && association.InUpdating)))
                {
                    if (association.MandatoryRoleData && association.DefaultRoleDataID != null)
                    {
                        setDefaultRefValues.Add(_impl.SqlBlockSetDefaultValue(association.FullDBName, Convert.ToString(association.DefaultRoleDataID)));
                    }
                }
            }

            string defaultIDFromGenerator = _impl.GetIDFromGeneratorScript(GeneratorName(entity.FullDBName));
            insertSqlStatements.Add(defaultIDFromGenerator);

            insertSqlStatements.AddRange(setDefaultRefValues);
            updateSqlStatements.AddRange(setDefaultRefValues);

            if (insertSqlStatements.Count > 0)
                script.Add(_impl.CreateTriggerScript(
                    String.Format("T_{0}_BI", TriggerNamePart(entity.FullDBName, entity.FullDBShortName)),
                    entity.FullDBName, TriggerFireTypes.Before, DMLEventTypes.Insert, 
                    "",
                    String.Join("\n", insertSqlStatements.ToArray())));

            if (updateSqlStatements.Count > 0)
                script.Add(_impl.CreateTriggerScript(
                    String.Format("T_{0}_BU", TriggerNamePart(entity.FullDBName, entity.FullDBShortName)),
                    entity.FullDBName, TriggerFireTypes.Before, DMLEventTypes.Update,
                    "",
                    String.Join("\n", updateSqlStatements.ToArray())));

            script.AddRange(CreateAuditTriggersScript(entity, withoutAttribute));
            
            return script;
        }

        protected virtual List<string> CreateViewsScript(Entity entity, DataAttribute withoutAttribute)
        {
            return new List<string>();
        }

        internal override List<string> CreateDependentScripts(IEntity entity, IDataAttribute withoutAttribute)
        {
            List<string> script = new List<string>();

            script.AddRange(CreateTriggersScript((Entity)entity, (DataAttribute)withoutAttribute));
            script.AddRange(CreateViewsScript((Entity)entity, (DataAttribute)withoutAttribute));
            
            return script;
        }

        internal override List<string> CreateScript(ICommonDBObject obj)
        {
            Entity entity = (Entity)obj;
            List<string> script = new List<string>();

            script.Add(CreateTableScript(entity));
            script.AddRange(CreateIndexesScript(entity));  //Создание индексов по системным полям SourceID,TaskID,PumpID
            script.AddRange(CreateUniqueKeyListScript(entity)); //Создание уникальных ключей таблицы
            script.AddRange(CustomTableConstraintsScript(entity));
            script.AddRange(CreateSequenceScript(entity));
            script.AddRange(CreateDependentScripts(entity, DataAttribute.SystemDummy));

            return script;
        }

        /// <summary>
        /// Cкрипт по формированию индексов по системным полям таблицы (SourceID,PumpID,TaskID)
        /// </summary>
        /// <returns>Скрипт</returns>
        private List<string> CreateIndexesScript(IEntity entity)
        {
            List<string> script = new List<string>();

            //Создаем индексы для таблиц фактов и классификаторов данных(которые делятся по источникам) по полям SourceID,PumpID,TaskID
            if (entity.ClassType == ClassTypes.clsFactData )
            {
                script.AddRange(_impl.CreateIndexScript(
                    entity.FullDBName, 
                    DataAttribute.SourceIDColumnName,
                    IndexName(((Entity)entity).FullDBShortName, DataAttribute.SourceIDColumnName), 
                    IndexTypes.BitmapIndex));
                
                if (entity.SubClassType == SubClassTypes.Pump
                    || entity.SubClassType == SubClassTypes.PumpInput)
                {
                    script.AddRange(_impl.CreateIndexScript(
                        entity.FullDBName, 
                        DataAttribute.PumpIDColumnName,
                        IndexName(((Entity)entity).FullDBShortName, DataAttribute.PumpIDColumnName), 
                        IndexTypes.BitmapIndex));
                }

                if (entity.SubClassType == SubClassTypes.Input
                    || entity.SubClassType == SubClassTypes.PumpInput)
                {
                    script.AddRange(_impl.CreateIndexScript(
                        entity.FullDBName, 
                        DataAttribute.TaskIDColumnName,
                        IndexName(((Entity)entity).FullDBShortName, DataAttribute.TaskIDColumnName), 
                        IndexTypes.BitmapIndex));
                }
            }

            else if ( entity.ClassType == ClassTypes.clsDataClassifier)
            {
                if ( (entity is IDataSourceDividedClass)
                     && ((DataSourceDividedClass)entity).IsDivided 
                    )
                {
                    script.AddRange(_impl.CreateIndexScript(
                        entity.FullDBName, 
                        DataAttribute.SourceIDColumnName,
                        IndexName(((Entity)entity).FullDBShortName, DataAttribute.SourceIDColumnName), 
                        IndexTypes.BitmapIndex));
                }

                if (entity.SubClassType == SubClassTypes.Pump
                    || entity.SubClassType == SubClassTypes.PumpInput)
                {
                    script.AddRange(_impl.CreateIndexScript(
                        entity.FullDBName, 
                        DataAttribute.PumpIDColumnName,
                        IndexName(((Entity)entity).FullDBShortName, DataAttribute.PumpIDColumnName), 
                        IndexTypes.BitmapIndex));
                } 

            }

            return script;
        }


        private List<string> CreateUniqueKeyListScript(IEntity entity)
        {
            List<string> script = new List<string>();
            foreach (UniqueKey uniqueKey in entity.UniqueKeys.Values)
            {
                script.AddRange(CreateUniqueKeyScript(uniqueKey));
                script.AddRange(CreateUniqueKeyHashScript(uniqueKey));
            }
            return script;
        }

        internal List<string> CreateUniqueKeyScript(IUniqueKey uniqueKey)
        {
            List<string> script = new List<string>();
            script.AddRange(_impl.CreateUniqueConstraintScript(uniqueKey.Parent.FullDBName, uniqueKey.FullDBName, uniqueKey.Fields));
            return script;
        }

        internal List<string> DropUniqueKeyScript(IUniqueKey uniqueKey)
        {
            List<string> script = new List<string>();
            script.AddRange(_impl.DropUniqueConstraintScript(uniqueKey.Parent.FullDBName, uniqueKey.FullDBName));
            return script;
        }

        internal List<string> CreateUniqueKeyHashScript(IUniqueKey uniqueKey)
        {
            if (!uniqueKey.Hashable) return new List<string>();

            List<string> script = new List<string>();
            //Добавляем поле, которое будет содержать хэш
            script.AddRange(_impl.CreateUniqueConstraintHashFieldScript(uniqueKey.Parent.FullDBName, DataAttribute.SystemHashUK.Name, DataAttribute.SystemHashUK.Type, DataAttribute.SystemHashUK.Size));
            
            //Добавляем триггер, который будет вычислять хэш
            script.Add(_impl.CreateUniqueConstraintHashTriggerScript(((UniqueKey)uniqueKey).HashFieldTriggerDBName, uniqueKey.Parent.FullDBName, DataAttribute.SystemHashUK.Name, uniqueKey.Fields));
            
            //этим скриптом вызовем фиктивный расчет хэша существующих данных созданным триггером
            script.Add(String.Format("update {0} set {1}={1}", uniqueKey.Parent.FullDBName, DataAttribute.SystemHashUK.Name));

            //Добавляем уникальный индекс на это поле
            script.AddRange(_impl.CreateIndexScript(uniqueKey.Parent.FullDBName,
                                                    DataAttribute.SystemHashUK.Name,
                                                    IndexName(uniqueKey.Parent.FullDBName, DataAttribute.SystemHashUK.Name),
                                                    IndexTypes.Unique
                                                    )
                           );

            return script;
        }

        internal List<string> DropUniqueKeyHashScript(IUniqueKey uniqueKey)
        {
            List<string> script = new List<string>();
            script.AddRange(_impl.DropColumnScript(uniqueKey.Parent.FullDBName, DataAttribute.SystemHashUK.Name)); //сначала пытаемся грохнуть столбец в таблице
            script.Add(_impl.DropTriggerScript(((UniqueKey)uniqueKey).HashFieldTriggerDBName)); //потом удаляем триггер, формирующий этот столбец
            return script;
        }


        /// <summary>
        /// Удаление внешних ключей ссылающихся на таблицу
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <returns>Скрипт</returns>
        private static List<string> DropReferencedFKScript(IEntity entity)
        {
            List<string> script = new List<string>();

			foreach (EntityAssociation association in entity.Associated.Values)
			{
				script.AddRange(((EntityAssociationScriptingEngine) association.ScriptingEngine)
				                	.DropReferenceConstraintScript(association));
			}

        	return script;
        }

        internal override List<string> DropScript(ICommonDBObject obj)
        {
            Entity entity = (Entity)obj;
            List<string> script = new List<string>();

            if (_impl.ExistsObject(String.Format("t_{0}_bi", TriggerNamePart(entity)), ObjectTypes.Trigger))
                script.Add(_impl.DropTriggerScript(String.Format("t_{0}_bi", TriggerNamePart(entity))));

            if (_impl.ExistsObject(String.Format("t_{0}_bu", TriggerNamePart(entity)), ObjectTypes.Trigger))
                script.Add(_impl.DropTriggerScript(String.Format("t_{0}_bu", TriggerNamePart(entity))));

            if (_impl.ExistsObject(String.Format("t_{0}_bd", TriggerNamePart(entity)), ObjectTypes.Trigger))
                script.Add(_impl.DropTriggerScript(String.Format("t_{0}_bd", TriggerNamePart(entity))));

            if (_impl.ExistsObject(String.Format("t_{0}_d", entity.FullDBName), ObjectTypes.Trigger))
                script.Add(_impl.DropTriggerScript(String.Format("t_{0}_d", entity.FullDBName)));

            if (_impl.ExistsObject(GeneratorName(entity.FullDBName), ObjectTypes.Sequence))
                script.Add(_impl.DropSequenceScript(GeneratorName(entity.FullDBName)));

            script.AddRange(DropReferencedFKScript(entity));

            if (_impl.ExistsObject(entity.FullDBName, ObjectTypes.Table))
                script.Add(_impl.DropTableScript(entity.FullDBName));
            
            return script;
        }

        internal List<string> GetAuditTriggersScript(ICommonDBObject obj)
        { 
            Entity entity = (Entity)obj;

            if (entity.SubClassType == SubClassTypes.Pump)
            {
                return new List<string>();
            }

            bool excludeDataPamp = (entity.SubClassType == SubClassTypes.PumpInput);

            return _impl.CreateAuditTriggerScript(entity.AuditTriggerName, entity.FullDBName, excludeDataPamp, 
                                                  (entity.Attributes.ContainsKey(DataAttribute.TaskIDColumnName) ? DataAttribute.TaskIDColumnName : null),
                                                  (entity.Attributes.ContainsKey(DataAttribute.PumpIDColumnName) ? DataAttribute.PumpIDColumnName : null),
                                                  entity.FullName, (int)entity.ClassType);
        }

        internal string DropAuditTriggersScript(ICommonDBObject obj)
        {
            return _impl.DropTriggerScript(((Entity)obj).AuditTriggerName);
        }

        internal virtual List<string> ReCreateSequences(Entity entity, int seed)
        {
            List<string> scripts = new List<string>();
            scripts.Add(_impl.DropSequenceScript(GeneratorName(entity.FullDBName)));
            scripts.AddRange(_impl.CreateSequenceScript(GeneratorName(entity.FullDBName), seed, 1));
            return scripts;
        }
    }
}
