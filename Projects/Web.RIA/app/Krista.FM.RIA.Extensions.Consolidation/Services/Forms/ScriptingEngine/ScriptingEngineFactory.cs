using System;

using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine.Oracle;
using Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine.Sql;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine
{
    /// <summary>
    /// Отвечает за создание конкретной реализации ScriptingEngineImpl в зависимости от типа СУБД.
    /// </summary>
    public class ScriptingEngineFactory
    {
        private readonly IScheme scheme;

        public ScriptingEngineFactory(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public ScriptingEngineImpl Create()
        {
            var factoryName = scheme.SchemeDWH.FactoryName;

            if (factoryName == ProviderFactoryConstants.OracleClient || 
                factoryName == ProviderFactoryConstants.OracleDataAccess || 
                factoryName == ProviderFactoryConstants.MSOracleDataAccess)
            {
                return new OracleScriptingEngineImpl();
            }

            if (factoryName == ProviderFactoryConstants.SqlClient)
            {
                return new SqlScriptingEngineImpl();
            }

            throw new ApplicationException("Реализация ScriptingEngineImpl для {0} не найдена.".FormatWith(factoryName));
        }
    }
}
