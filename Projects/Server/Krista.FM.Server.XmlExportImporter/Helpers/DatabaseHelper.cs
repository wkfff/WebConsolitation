using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.XmlExportImporter.Helpers
{
    internal class DatabaseHelper
    {
        /// <summary>
        /// В случае импорта с сохранением id, устанавливаем id в значение, следующее за последним импортируемым импортируемым
        /// </summary>
        /// <returns></returns>
        internal static int SetNextId(IScheme scheme, int lastId, IEntity schemeObject, IDatabase db)
        {
            var id = schemeObject.GetGeneratorNextValue;
            if (lastId > id)
            {
                // делаем поправку id на следующее значение за импортируемым
                var generatorName = GetGeneratorName(scheme, schemeObject);
                if (string.Compare(scheme.SchemeDWH.FactoryName, "SYSTEM.DATA.SQLCLIENT", true) == 0)
                {
                    db.ExecQuery(string.Format("set identity_insert g.{0} on", generatorName), QueryResultTypes.NonQuery);
                    db.ExecQuery(string.Format("insert into g.{0} (ID) Values (?)", generatorName), 
                        QueryResultTypes.NonQuery, new DbParameterDescriptor("p0", lastId + 1));
                    db.ExecQuery(string.Format("delete from g.{0}", generatorName), QueryResultTypes.NonQuery);
                    db.ExecQuery(string.Format("set identity_insert g.{0} off", generatorName), QueryResultTypes.NonQuery);
                }
                else
                {
                    db.ExecQuery(
                        string.Format("ALTER SEQUENCE {0} INCREMENT BY {1}", generatorName, lastId - id + 1), QueryResultTypes.NonQuery);
                }
            }

            return 0;
        }

        protected static string GetGeneratorName(IScheme scheme, IEntity schemeObject)
        {
            string generatorName = schemeObject.GeneratorName;
            if (string.Compare(scheme.SchemeDWH.FactoryName, "SYSTEM.DATA.SQLCLIENT", true) ==0)
            {
                if (generatorName.StartsWith("g_", StringComparison.OrdinalIgnoreCase))
                    generatorName = generatorName.Substring(2, generatorName.Length - 2);
            }
            return generatorName;
        }
    }
}
