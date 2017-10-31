using System;
using System.Collections.Generic;

using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.ScriptingEngine.Classes
{
    internal class EntityAssociationScriptingEngine : ScriptingEngineAbstraction
    {
        internal EntityAssociationScriptingEngine(ScriptingEngineImpl impl)
            : base(impl)
        {
        }

        /// <summary>
        /// Наименование внешнего ключа
        /// </summary>
        private static string ForeignKeyConstraintName(string constraintName)
        {
            return "FK" + constraintName.Substring(0, constraintName.Length > 28 ? 28 : constraintName.Length);
        }

        /// <summary>
        /// Возвращает скрипт для создания внешнего ключа
        /// </summary>
        internal List<string> CreateReferenceConstraintScript(EntityAssociation entityAssociation)
        {
            List<string> script = new List<string>();
            string onDeleteActionClause = String.Empty;

            switch (entityAssociation.onDeleteAction)
            {
                case OnDeleteAction.Cascade:
                    onDeleteActionClause = " ON DELETE CASCADE";
                    break;
                case OnDeleteAction.SetNull:
                    onDeleteActionClause = " ON DELETE SET NULL";
                    break;
            }

			script.Add(_impl.CreateForeignKeyScript(
				entityAssociation.RoleA.FullDBName,
				ForeignKeyConstraintName(entityAssociation.RoleA.FullDBShortName + entityAssociation.ShortName), 
                entityAssociation.RoleDataAttribute.Name, 
                entityAssociation.RoleB.FullDBName, 
                onDeleteActionClause));

            //для внешних ключей таблиц фактов по возможности создаем bitmap-индексы
            //кроме ссылок на фиксированные классификаторы
            bool asBitmap;
            asBitmap = ((entityAssociation.RoleA.ClassType == ClassTypes.clsFactData)
                        && (entityAssociation.RoleB.ClassType == ClassTypes.clsFixedClassifier));
         
            script.AddRange(_impl.CreateIndexScript(entityAssociation.RoleA.FullDBName,
                entityAssociation.RoleDataAttribute.Name,
                ForeignKeyIndexName(entityAssociation.RoleA.FullDBShortName + entityAssociation.ShortName),
                (asBitmap ? IndexTypes.BitmapIndex : IndexTypes.NormalIndex)));
            return script;
        }

        /// <summary>
        /// Возвращает скрипт для удаления внешнего ключа и индекса.
        /// </summary>
        internal List<string> DropReferenceConstraintScript(EntityAssociation entityAssociation)
        {
            List<string> script = new List<string>();

            script.AddRange(DropReferenceIndexScript(entityAssociation));

        	script.Add(_impl.DropConstraintScript(
				entityAssociation.RoleA.FullDBName,
				ForeignKeyConstraintName(entityAssociation.RoleA.FullDBShortName + entityAssociation.ShortName)
				));

            return script;
        }

        /// <summary>
        /// Наименование индекса на внешний ключ
        /// </summary>
        private static string ForeignKeyIndexName(string indexName)
        {
            return "I_" + indexName.Substring(0, indexName.Length > 28 ? 28 : indexName.Length);
        }

        /// <summary>
        /// Возвращает скрипт для удаления индекса на внешний ключ.
        /// </summary>
        private List<string> DropReferenceIndexScript(EntityAssociation entityAssociation)
        {
            List<string> script = new List<string>();

        	script.AddRange(
        		_impl.DropReferenceIndexScript(
					entityAssociation.RoleA.FullDBName,
        			ForeignKeyIndexName(entityAssociation.RoleA.FullDBShortName + entityAssociation.ShortName)));

            return script;
        }

        /// <summary>
        /// Возвращает опциональную часть скрипта для создания ассоциации
        /// </summary>
        protected virtual List<string> CustomScript(EntityAssociation entityAssociation)
        {
            return new List<string>();
        }

        internal override List<string> CreateScript(ICommonDBObject obj)
        {
            EntityAssociation entityAssociation = (EntityAssociation)obj;
            List<string> script = new List<string>();

            script.AddRange(_impl.DisableAllTriggersScript(entityAssociation.RoleA.FullDBName));

            // добавляем атрибут в отношение

            EntityDataAttribute attr = (EntityDataAttribute)DataAttributeCollection.GetAttributeByKeyName(entityAssociation.RoleA.Attributes, entityAssociation.ObjectKey, entityAssociation.FullDBName);
            script.AddRange(attr.AddScript(entityAssociation.RoleA, true, false));

            script.AddRange(_impl.EnableAllTriggersScript(entityAssociation.RoleA.FullDBName));
            
            // внешний ключ 
            script.AddRange(CreateReferenceConstraintScript(entityAssociation));
            
            // индекс на внешний ключ
            // перенесено в CreateReferenceConstraintScript: 
            // script.AddRange(FormReferenceIndex(entityAssociation));
            
            // пересоздание триггеров и пр. для отношения
            script.AddRange(entityAssociation.RoleA.GetDependentScripts());
            
            // добавляем дополнительный скрипт
            script.AddRange(CustomScript(entityAssociation));

            return script;
        }

        internal override List<string> DropScript(ICommonDBObject obj)
        {
            EntityAssociation entityAssociation = (EntityAssociation)obj;
            List<string> script = new List<string>();

            EntityDataAttribute attr = (EntityDataAttribute)DataAttributeCollection.GetAttributeByKeyName(entityAssociation.RoleA.Attributes, entityAssociation.ObjectKey, entityAssociation.FullDBName);
            
			// удаляем индекс
			if (_impl.ExistsObject(ForeignKeyIndexName(entityAssociation.RoleA.FullDBShortName + entityAssociation.ShortName), ObjectTypes.Index))
            {
                script.AddRange(_impl.DropReferenceIndexScript(
                    entityAssociation.RoleA.FullDBName, 
                    ForeignKeyIndexName(
                        entityAssociation.RoleA.FullDBShortName + 
                        entityAssociation.ShortName)));
            }

			// удаляем ограничение
			if (_impl.ExistsObject(ForeignKeyConstraintName(entityAssociation.RoleA.FullDBShortName + entityAssociation.ShortName), ObjectTypes.ForeignKeysConstraint))
			{
				script.Add(
					_impl.DropConstraintScript(entityAssociation.RoleA.FullDBName,
					                           ForeignKeyConstraintName(entityAssociation.RoleA.FullDBShortName +
					                                                    entityAssociation.ShortName)));
			}

			// удаляем поле
        	script.AddRange(_impl.DropColumnScript(entityAssociation.RoleA.FullDBName, attr.Name));
            
			// пересоздание триггеров и пр. для отношения
            script.AddRange(entityAssociation.RoleA.GetDependentScripts(attr));

            // Если роль B сопоставимый классификатор и есть триггер каскадного уделения, то убираем его.
            if (entityAssociation.RoleB.ClassType == ClassTypes.clsBridgeClassifier &&
                    _impl.ExistsObject(ClassifierEntityScriptingEngine.GetCascadeDeleteTriggerName(entityAssociation.RoleB), ObjectTypes.Trigger))
            {
                script.AddRange(_impl.DropCascadeDeleteTriggerScript(ClassifierEntityScriptingEngine.GetCascadeDeleteTriggerName(entityAssociation.RoleB)));
            }

            return script;
        }

        internal override List<string> CreateDependentScripts(IEntity entity, IDataAttribute withoutAttribute)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
