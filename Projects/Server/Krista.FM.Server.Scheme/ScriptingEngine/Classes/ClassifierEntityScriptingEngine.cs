using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.ScriptingEngine.Classes
{
    internal class ClassifierEntityScriptingEngine : DataSourceDividedClassScriptingEngine
    {
        /// <summary>
        /// Вазовое смещение герератора для данных раздаботчика
        /// </summary>
        private static readonly int developerGeneratorSeed = 1000000000;

        /// <summary>
        /// Инициализация экземпляра
        /// </summary>
        /// <param name="impl">Реализация СУБД зависимого функционала</param>
        internal ClassifierEntityScriptingEngine(ScriptingEngineImpl impl)
            : base(impl)
        {
        }

        /// <summary>
        /// Вазовое смещение герератора для данных раздаботчика
        /// </summary>
        public static int DeveloperGeneratorSeed
        {
            get { return developerGeneratorSeed; }
        }

        /// <summary>
        /// Возвращает наименование генератора (FullDBName обрезается до 28 символов)
        /// </summary>
        internal string DeveloperGeneratorName(string tableName)
        {
            return _impl.GeneratorName("_" + tableName);
        }

        /// <summary>
        /// Создание второго генератора для данных разработчика.
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <returns>Скрипт для создания генераторов</returns>
        internal virtual List<string> CreateDeveloperSequenceScript(Entity entity)
        {
            return _impl.CreateSequenceScript(DeveloperGeneratorName(entity.FullDBName), developerGeneratorSeed, 1);
        }

        /// <summary>
        /// Создание второго генератора для данных разработчика, если его нет.
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <returns>Скрипт для создания генераторов</returns>
        internal List<string> CreateIfNotExistsDeveloperSequenceScript(Entity entity)
        {
            if (!_impl.ExistsObject(DeveloperGeneratorName(entity.FullDBName), ObjectTypes.Sequence))
                return _impl.CreateSequenceScript(DeveloperGeneratorName(entity.FullDBName), developerGeneratorSeed, 1);
            else
                return new List<string>();
        }

        /// <summary>
        /// Создание генераторов для классификаторов.
        /// Для классификаторов создается дополнительный генератор для записей "Кристы".
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <returns>Скрипт для создания генераторов</returns>
        protected override List<string> CreateSequenceScript(Entity entity)
        {
            List<string> script = base.CreateSequenceScript(entity);
            script.AddRange(CreateDeveloperSequenceScript(entity));
            return script;
        }

        /// <summary>
        /// Возвращает наименование констрейнта на родительский ключ
        /// </summary>
        internal static string ParentIDForeignKeyConstraintName(string tableName)
        {
            string constraintName = tableName + DataAttribute.ParentIDColumnName;
            return "FK" + constraintName.Substring(0, constraintName.Length > 28 ? 28 : constraintName.Length);
        }

		/// <summary>
		/// Возвращает наименование индекса на родительский ключ ParentID.
		/// </summary>
		internal static string ParentIdIndexName(string tableName)
		{
			string constraintName = tableName + DataAttribute.ParentIDColumnName;
			return "I_" + constraintName.Substring(0, constraintName.Length > 28 ? 28 : constraintName.Length);
		}

		/// <summary>
		/// Создание индекса на поле ParentID, если его нет.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		internal List<string> CreateIfNotExistsParentIdIndexScript(Entity entity)
		{
			if (((IClassifier)entity).Levels.HierarchyType == HierarchyType.Regular)
				return new List<string>();
			
			if (!_impl.ExistsObject(ParentIdIndexName(entity.FullDBName), ObjectTypes.Index))
			{
                return _impl.CreateIndexScript(
                    entity.FullDBName,
                    DataAttribute.ParentIDColumnName,
                    ParentIdIndexName(entity.FullDBName),
                    IndexTypes.NormalIndex);
			}
			return new List<string>();
		}

        /// <summary>
        /// Возвращает опциональную часть скрипта для создания отношения
        /// </summary>
        /// <returns></returns>
        protected override List<string> CustomTableConstraintsScript(Entity entity)
        {
            List<string> script = new List<string>();

            if (((Classifier)entity).Levels.HierarchyType == HierarchyType.ParentChild)
            {
				// Создаем внешний ключ на поле ParentID
				script = _impl.ParentKeyConstraintScript(
					entity.FullDBName, 
					ParentIDForeignKeyConstraintName(entity.FullDBShortName), 
					DataAttribute.ParentIDColumnName);
				
				// Создаем индекс на поле ParentID
				script.AddRange(_impl.CreateIndexScript(
                    entity.FullDBName,
                    DataAttribute.ParentIDColumnName,
                    ParentIdIndexName(entity.FullDBName),
                    IndexTypes.NormalIndex));

			}

            return script;
        }

        /// <summary>
        /// Формирует определение выражений для короткого наименования
        /// </summary>
        /// <param name="classifier"></param>
        /// <param name="shortNameHeader">Часть определения атрибута</param>
        /// <param name="shortNameSelectPart">Часть выражения select</param>
        /// <returns></returns>
        internal bool GenerateShortNameParts(IClassifier classifier, out string shortNameHeader, out string shortNameSelectPart)
        {
            string memberName = String.Empty;
            foreach (IDimensionLevel level in classifier.Levels.Values)
            {
                if (level.LevelType == LevelTypes.All)
                    continue;
                memberName = level.MemberName.Name;
            }

            shortNameHeader = String.Empty;
            shortNameSelectPart = String.Empty;
            if (memberName != String.Empty)
            {
                int remainder;
                int quotient =
                    Math.DivRem(
                        DataAttributeCollection.GetAttributeByKeyName(classifier.Attributes, memberName, memberName).
                            Size, 255, out remainder);
                if (quotient == 0)
                {
                    return false;
                }

                if (remainder > 0)
                {
                    quotient += 1;
                }

                shortNameSelectPart = _impl.GetShortNamesExpression(quotient, memberName, ref shortNameHeader,
                                                                    !SchemeClass.Instance.SchemeMDStore.IsAS2005(),
                                                                    DataAttributeCollection.GetAttributeByKeyName(
                                                                        classifier.Attributes, memberName, memberName).
                                                                        Size);

                return true;
            }
            return false;
        }

        protected override List<string> CreateViewsScript(Entity entity, DataAttribute withoutAttribute)
        {
            List<string> script = new List<string>();
            string shortNameHeader;
            string shortNameSelectPart;

            bool generateShortName = GenerateShortNameParts((Classifier)entity, out shortNameHeader, out shortNameSelectPart);

            // Создаем представление только если
            // 1. Классификатор делится по источникам и имеет плоскую иерархию
            // 2. Необходима генерация укороченных частей наименований
            // 3. Для сопоставимых классификаторов, у которых есть ассоциация сопоставления версий

            if (!(
                (((Classifier)entity).Levels.HierarchyType == HierarchyType.Regular 
                    && ((Classifier)entity).IsDivided)
                || generateShortName
                || (entity.ClassType == ClassTypes.clsBridgeClassifier
                    && entity.Associations.Any(association => association.Value.AssociationClassType == AssociationClassTypes.BridgeBridge))))
            {
                // Представления могут быть у классификаторов данных, сопоставимых и фиксированных.
                if (entity.ClassType == ClassTypes.clsDataClassifier ||
                    entity.ClassType == ClassTypes.clsBridgeClassifier ||
                    entity.ClassType == ClassTypes.clsFixedClassifier)
                {
                    // Если представление не нужно, то оно удаляется
                    return DropIfExistsViewScript(entity);
                }
                else 
                    return script;
            }

            List<string> attributesNames = new List<string>();

            // Перебираем все атрибуты классификатора, кроме ссылочных
            foreach (DataAttribute item in entity.Attributes.Values)
            {
                if (item.Class == DataAttributeClassTypes.Reference)
                    continue;

                // Пропускаем атрибут, который не нужно учитывать
                if (item == withoutAttribute)
                    continue;

                attributesNames.Add(_impl.CheckReservedColumnName(item.Name));
            }

            // Перебираем все ассоциации
            foreach (EntityAssociation item in entity.Associations.Values)
            {
                // Пропускаем атрибут, который не нужно учитывать
                if (item.FullDBName == withoutAttribute.Name)
                    continue;

                if (item.DbObjectState == DBObjectStateTypes.InDatabase
                    || item.DbObjectState == DBObjectStateTypes.Changed
                    || item.InUpdating)
                {
                    attributesNames.Add(item.FullDBName);
                }
            }

            string dataSourceNameHeader = String.Empty;
            string dataSourceNameSelect = String.Empty;
            string dataSourceJoinClause = String.Empty;
            if (((Classifier)entity).Levels.HierarchyType == HierarchyType.Regular && ((Classifier)entity).IsDivided)
            {
                dataSourceNameHeader = ", DataSourceName";
                dataSourceNameSelect = String.Format(
                    ",\nDS.\"SUPPLIERCODE\" {0} ' ' {0} DS.\"DATANAME\" {0} " +
                    "CASE KindsOfParams WHEN 8 THEN '' " +
                    "ELSE ' - ' {0} " +
                    "cast((CASE DS.\"KINDSOFPARAMS\" " +
                    "WHEN 0 THEN DS.\"NAME\" {0} ' ' {0} cast(DS.\"YEAR\" as varchar(4)) " +
                    "WHEN 1 THEN cast(DS.\"YEAR\" as varchar(4)) " +
                    "WHEN 2 THEN cast(DS.\"YEAR\" as varchar(4)) {0} ' ' {0} cast(DS.\"MONTH\" as varchar(2)) " +
                    "WHEN 3 THEN cast(DS.\"YEAR\" as varchar(4)) {0} ' ' {0} cast(DS.\"MONTH\" as varchar(2)) {0} ' ' {0} DS.\"VARIANT\" " +
                    "WHEN 4 THEN cast(DS.\"YEAR\" as varchar(4)) {0} ' ' {0} DS.\"VARIANT\" " +
                    "WHEN 5 THEN cast(DS.\"YEAR\" as varchar(4)) {0} ' ' {0} cast(DS.\"QUARTER\" as varchar(1)) " +
                    "WHEN 6 THEN cast(DS.\"YEAR\" as varchar(4)) {0} ' ' {0} DS.\"TERRITORY\" " +
                    "WHEN 7 THEN cast(DS.\"YEAR\" as varchar(4)) {0} ' ' {0} cast(DS.\"QUARTER\" as varchar(1)) {0} ' ' {0} cast(DS.\"MONTH\" as varchar(2)) " +
                    "WHEN 9 THEN DS.\"VARIANT\" " +
                    "END ) as varchar(1000)) END",
                    _impl.ConcatenateChar());
                dataSourceJoinClause = " LEFT OUTER JOIN DataSources DS ON (T.SourceID = DS.ID)";
            }

            if (entity.ClassType == ClassTypes.clsBridgeClassifier
                    && entity.Associations.Any(association => association.Value.AssociationClassType == AssociationClassTypes.BridgeBridge))
            {
                dataSourceJoinClause = String.Format(
                    " WHERE sourceID IN (SELECT sourceID FROM ObjectVersions t2 WHERE t2.ObjectKey = '{0}' and t2.IsCurrent = 1) or ID = -1",
                    entity.ObjectKey);
            }

            string viewName = GetViewName(entity);
            if (String.IsNullOrEmpty(viewName))
            {
                throw new Exception("Дополнительные представления создаются только для классификаторов данных, сопоставимых и фиксированных");
            }

            viewName = viewName.Substring(0, viewName.Length > _impl.MaxIdentifierLength() ? _impl.MaxIdentifierLength() : viewName.Length);

            if (_impl.ExistsObject(viewName, ObjectTypes.View))
                script.Add(_impl.DropViewScript(viewName));

			script.Add(_impl.CreateViewScript(
				viewName,
				String.Format(
				"({0}{4}{5})\nAS " +
				"SELECT T.{1}{2}\n{6}\nFROM {3} T{7}",
				String.Join(", ", attributesNames.ToArray()),   // 0 - Атрибуты классификатора
				String.Join(", T.", attributesNames.ToArray()), // 1 - Атрибуты классификатора
				dataSourceNameSelect,                           // 2 - Вычисляемое поле DataSourceName
				entity.FullDBName,                              // 3 - Наименование таблицы классификатора
				dataSourceNameHeader,                           // 4 - 
				shortNameHeader,                                // 5 - 
				shortNameSelectPart,                            // 6 -
				dataSourceJoinClause)							// 7 -
				));
            
            return script;
        }

        internal static string GetViewName(IEntity entity)
        {
            string prefix;
            if (entity.ClassType == ClassTypes.clsDataClassifier)
                prefix = "DV";
            else if (entity.ClassType == ClassTypes.clsBridgeClassifier)
                prefix = "BV";
            else if (entity.ClassType == ClassTypes.clsFixedClassifier)
            {
                prefix = "FV";
                return prefix + entity.FullDBName.Substring(2);
            }
            else
                return String.Empty;

            return prefix + entity.FullDBName.Substring(1);
        }

        private List<string> DropIfExistsViewScript(IEntity entity)
        {
            List<string> script = new List<string>();
            if (_impl.ExistsObject(GetViewName(entity), ObjectTypes.View))
                script.Add(_impl.DropViewScript(GetViewName(entity)));
            return script;
        }

        internal override List<string> DropScript(ICommonDBObject obj)
        {
            List<string> script = new List<string>();

            Entity entity = (Entity)obj;

            if (_impl.ExistsObject(ScriptingEngineImpl.DeveloperLockTriggerName(entity.FullDBName), ObjectTypes.Trigger))
                script.AddRange(_impl.DropDeveloperLockTriggerScript(entity.FullDBName));

            if (_impl.ExistsObject(DeveloperGeneratorName(entity.FullDBName), ObjectTypes.Sequence))
                script.Add(_impl.DropSequenceScript(DeveloperGeneratorName(entity.FullDBName)));

            if (_impl.ExistsObject(GetViewName(entity), ObjectTypes.View))
                script.AddRange(DropIfExistsViewScript(entity));

            script.AddRange(base.DropScript(obj));

            return script;
        }

        protected override List<string> CreateTriggersScript(Entity entity, DataAttribute withoutAttribute)
        {
            List<string> script = base.CreateTriggersScript(entity, withoutAttribute);
            if (entity.ClassType == ClassTypes.clsDataClassifier || entity.ClassType == ClassTypes.clsBridgeClassifier)
            {
                if (ExistsObject(ScriptingEngineImpl.DeveloperLockTriggerName(entity.FullDBName), ObjectTypes.Trigger))
                    script.AddRange(_impl.DropDeveloperLockTriggerScript(entity.FullDBName));
                script.AddRange(_impl.CreateDeveloperLockTriggerScript(entity.FullDBName));
            }
            return script;
        }

        internal static string GetCascadeDeleteTriggerName(Entity entity)
        {
            return string.Format("t_{0}_d", entity.FullDBName);
        }

        /// <summary>
        /// Формирует скрипт для создания триггера каскадного удаления.
        /// </summary>
        /// <param name="entity">Классификатор, для которого создаем триггер.</param>
        /// <returns>Текст скрипта.</returns>
        internal List<string> GetCascadeDeleteTrigerScript(Entity entity)
        {
            List<string> script = new List<string>();
            string triggerName = GetCascadeDeleteTriggerName(entity);
            Dictionary<string, string> referencesList = new Dictionary<string, string>();

            // Если уже есть триггер каскадного удаления, убираем его.
            if (_impl.ExistsObject(triggerName, ObjectTypes.Trigger))
            {
                script.Add(_impl.DropTriggerScript(triggerName));
            }

            // Если иерахия ParentChild добавляем ссылку на родительскую запись.
            if (((Classifier)entity).Levels.HierarchyType == HierarchyType.ParentChild)
            {
                referencesList.Add(entity.FullDBName, DataAttribute.ParentIDColumnName);
            }
            // Если сопоставимый классификатор, то добавляем ссылки по ассоциациям.                                
            if (entity.ClassType == ClassTypes.clsBridgeClassifier)
            {
                foreach (IEntityAssociation item in entity.Associated.Values)
                {
                    if (item.AssociationClassType == AssociationClassTypes.Bridge && item.State == ServerSideObjectStates.Consistent)
                    {
                        if (entity.FullDBName != item.RoleData.FullDBName)
                        {
                            referencesList.Add(item.RoleData.FullDBName, item.RoleDataAttribute.Name);
                        }
                    }
                }
                if (referencesList.Count > 0)
                {
                    script.AddRange(_impl.CreateCascadeDeleteTriggerScript(entity.FullDBName, referencesList, triggerName, "-1"));
                }
            }
            // Если классификатор данных то оставляем только ParentChild.
            if (entity.ClassType == ClassTypes.clsDataClassifier && referencesList.Count > 0)
            {
                script.AddRange(_impl.CreateCascadeDeleteTriggerScript(entity.FullDBName, referencesList, triggerName, "null"));
            }
            
            return script;
        }
    }
}
