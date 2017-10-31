using System;
using System.Collections.Generic;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine.Sql
{
    public class SqlScriptingEngineImpl : ScriptingEngineImpl
    {
        private static readonly Dictionary<Type, string> dataTypeMappings;

        /// <summary>
        /// Initializes static members of the SqlScriptingEngineImpl class.
        /// </summary>
        static SqlScriptingEngineImpl()
        {
            dataTypeMappings = new Dictionary<Type, string>();
            dataTypeMappings.Add(typeof(bool), "NUMERIC");
            dataTypeMappings.Add(typeof(char), "CHAR");
            dataTypeMappings.Add(typeof(DateTime), "DATETIME");
            dataTypeMappings.Add(typeof(decimal), "NUMERIC");
            dataTypeMappings.Add(typeof(int), "INT");
            dataTypeMappings.Add(typeof(string), "VARCHAR");
            dataTypeMappings.Add(typeof(byte[]), "VARBINARY(MAX)");
        }

        public override string DataTypeMappings(Type dataAttributeType)
        {
            return dataTypeMappings[dataAttributeType];
        }

        public override string GetDataTypeScript(Type type, int? size, int? scale)
        {
            string sizeSlale = String.Empty;
            if (type == typeof(bool))
            {
                sizeSlale = String.Format("({0})", 1);
            }
            else if (type == typeof(string))
            {
                sizeSlale = String.Format("({0})", size ?? 4000);
            }
            else if (type == typeof(decimal))
            {
                sizeSlale = String.Format("({0}, {1})", size ?? 17, scale ?? 4);
            }

            return DataTypeMappings(type) + sizeSlale;
        }
    }
}
