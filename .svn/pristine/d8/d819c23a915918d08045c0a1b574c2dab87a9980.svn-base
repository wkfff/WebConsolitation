using System;
using System.Collections.Generic;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine.Oracle
{
    public class OracleScriptingEngineImpl : ScriptingEngineImpl
    {
        private static readonly Dictionary<Type, string> dataTypeMappings;

        /// <summary>
        /// Initializes static members of the OracleScriptingEngineImpl class.
        /// </summary>
        static OracleScriptingEngineImpl()
        {
            dataTypeMappings = new Dictionary<Type, string>();
            dataTypeMappings.Add(typeof(bool), "NUMBER");
            dataTypeMappings.Add(typeof(char), "CHAR");
            dataTypeMappings.Add(typeof(DateTime), "DATE");
            dataTypeMappings.Add(typeof(decimal), "NUMBER");
            dataTypeMappings.Add(typeof(int), "NUMBER");
            dataTypeMappings.Add(typeof(string), "VARCHAR2");
            dataTypeMappings.Add(typeof(byte[]), "BLOB");
        }

        public override string DataTypeMappings(Type dataAttributeType)
        {
            return dataTypeMappings[dataAttributeType];
        }

        public override string GetDataTypeScript(Type type, int? size, int? scale)
        {
            string sizeSlale = String.Empty;
            if (type == typeof(int))
            {
                sizeSlale = String.Format("({0})", size ?? 10);
            } 
            else if (type == typeof(bool))
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
