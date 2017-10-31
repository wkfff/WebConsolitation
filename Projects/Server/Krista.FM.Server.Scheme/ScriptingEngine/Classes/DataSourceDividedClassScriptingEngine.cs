using System;
using System.Collections.Generic;

using Krista.FM.Server.Scheme.Classes;

namespace Krista.FM.Server.Scheme.ScriptingEngine.Classes
{
    internal class DataSourceDividedClassScriptingEngine : EntityScriptingEngine 
    {
        internal DataSourceDividedClassScriptingEngine(ScriptingEngineImpl impl): base(impl)
        {
        }

        internal static string GetDataSourceLockTriggerName(string tableName, string shortTableName, string refColumnName)
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
            return String.Format("t_{0}_lc", name);
        }

        internal List<string> CreateDataSourceLockTriggersScript(Entity entity, DataAttribute withoutAttribute)
        {
            List<string> script = new List<string>();
           
            if ((entity is DataSourceDividedClass) && ((DataSourceDividedClass)entity).IsDivided)
            {
                string refSourceIDColumnName = "SourceID";
                string dataSourceTableName = "HUB_DataSources";
                string triggerName =
                    GetDataSourceLockTriggerName(entity.FullDBName, entity.FullDBShortName, refSourceIDColumnName);
                if (refSourceIDColumnName == withoutAttribute.Name)
                    script.Add(_impl.DropTriggerScript(triggerName));
                else
                {
                    if (_impl.ExistsObject(triggerName, ObjectTypes.Trigger))
                        script.Add(_impl.DropTriggerScript(triggerName));
                   script.Add(
                        _impl.CreateDataSourceLockTriggerScript(triggerName, refSourceIDColumnName, entity.FullDBName,
                                                                entity.FullDBShortName, dataSourceTableName));
                }
            } 
            return script;
        }

    }
}
