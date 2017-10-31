using System;
using System.Text;

namespace Krista.FM.RIA.Extensions.Consolidation.Services.Forms.ScriptingEngine
{
    public abstract class ScriptingEngineImpl
    {
        public abstract string DataTypeMappings(Type dataAttributeType);

        public abstract string GetDataTypeScript(Type type, int? size, int? scale);

        public string GetColumnScript(string name, Type type, int? size, int? scale, bool nullable)
        {
            return new StringBuilder()
                .Append(name).Append(' ')
                .Append(GetDataTypeScript(type, size, scale)).Append(' ')
                .Append(GetNotNullScript(nullable))
                .ToString();
        }

        internal static string GetNotNullScript(bool isNullable)
        {
            return isNullable ? " NULL" : " NOT NULL";
        }
    }
}
