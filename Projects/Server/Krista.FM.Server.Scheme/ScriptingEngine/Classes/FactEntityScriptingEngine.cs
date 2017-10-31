using System;
using System.Collections.Generic;

using Krista.FM.Server.Scheme.Classes;
using Krista.FM.Server.Scheme.ScriptingEngine.Oralce;
using Krista.FM.Server.Scheme.ScriptingEngine.Sql;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.ScriptingEngine.Classes
{
    internal class FactEntityScriptingEngine : DataSourceDividedClassScriptingEngine
    {
        public FactEntityScriptingEngine(ScriptingEngineImpl impl)
            : base(impl)
        {
        }

        protected override List<string> CreateSequenceScript(Entity entity)
        {
            if (_impl is OracleScriptingEngineImpl)
                return _impl.CreateSequenceScript(GeneratorName(entity.FullDBName), 1, 1);
			else if (_impl is SqlScriptingEngineImpl || _impl is NullScriptingEngineImpl)
                return new List<string>();
            else
                throw new NotImplementedException(String.Format("{0}.CreateSequenceScript для FactEntityScriptingEngine", _impl));
        }

        protected override string CreateTableColumnScript(EntityDataAttribute attr)
        {
            if (attr.Name == DataAttribute.IDColumnName)
                return _impl.AutoIncrementColumnScript(DataAttribute.IDColumnName);
            else
                return EntityDataAttribute.ScriptingEngine.ColumnScript(attr, true);
        }

        protected override List<string> CreateTriggersScript(Entity entity, DataAttribute withoutAttribute)
        {
            List<string> script = base.CreateTriggersScript(entity, withoutAttribute);

            // Триггера на манипуляции с данными привязанных к классификаторам
            foreach (EntityAssociation association in entity.Associations.Values)
            {
                //if (association.RoleDataAttribute.Name == withoutAttribute.Name)
                //    continue;
                if (association.State == ServerSideObjectStates.Consistent || association.InUpdating)
                {
                    script.AddRange(association.RoleB.GetCustomTriggerScriptForChildEntity(entity, withoutAttribute));
                }
            }

            return script;
        }
    }
}
