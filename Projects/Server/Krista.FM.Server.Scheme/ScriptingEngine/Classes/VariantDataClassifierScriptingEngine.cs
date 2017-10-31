using System;
using System.Collections.Generic;

using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.ScriptingEngine.Classes
{
    internal class VariantDataClassifierScriptingEngine : ClassifierEntityScriptingEngine
    {
        internal VariantDataClassifierScriptingEngine(ScriptingEngineImpl impl)
            : base(impl)
        {
        }

        internal static string GetVariantLockTriggerName(string tableName, string shortTableName, string refColumnName)
        {
            string name = String.Format("{0}_{1}", tableName, refColumnName);
            if (name.Length > 25)
            {
                name = String.Format("{0}_{1}", shortTableName, refColumnName);
                name = name.Substring(0, name.Length > 25 ? 25 : name.Length);
            }
            else
            {
                name = name.Substring(0, name.Length > 25 ? 25 : name.Length);
            }
            return String.Format("t_{0}_bc", name);
        }

        /// <summary>
        /// Возвращает скрипт триггера для дочернего отношения 
        /// запрещающий редактирование заблокированных вариантов.
        /// </summary>
        /// <param name="entity">Классификатор вариантов.</param>
        /// <param name="childEntity">Дочернее отношение.</param>
        /// <param name="withoutAttribute"></param>
        /// <returns>Скрипт.</returns>
        internal List<string> CreateVariantLockTriggerScript(VariantDataClassifier entity, Entity childEntity, DataAttribute withoutAttribute)
        {
            List<string> script = new List<string>();

            if (childEntity.State == ServerSideObjectStates.Consistent)
            {
                string refVariantColumnName = String.Empty;
                foreach (EntityAssociation item in entity.Associated.Values)
                {
                    if (item.RoleA.FullName == childEntity.FullName)
                    {
                        refVariantColumnName = item.FullDBName;
                        break;
                    }
                }

                if (String.IsNullOrEmpty(refVariantColumnName))
                    throw new Exception(String.Format("Дочерняя сущность {0} не найдена.", childEntity.FullName));

                string triggerName =
                    GetVariantLockTriggerName(childEntity.FullDBName, childEntity.FullDBShortName, refVariantColumnName);
                if (refVariantColumnName == withoutAttribute.Name)
                    script.Add(_impl.DropTriggerScript(triggerName));
                else
                {
                    if (_impl.ExistsObject(triggerName, ObjectTypes.Trigger))
                        script.Add(_impl.DropTriggerScript(triggerName));
                    script.Add(
                        _impl.CreateVariantLockTriggerScript(triggerName, refVariantColumnName, childEntity.FullDBName,
                                                             childEntity.FullDBShortName, entity.FullDBName));
                }
            }
            return script;
        }
    }
}
