using System;
using System.Collections.Generic;
using System.Diagnostics;
using Krista.FM.ServerLibrary;
using Krista.FM.Update.Framework;

namespace Krista.FM.Update.SchemeAdapter
{
    /// <summary>
    /// Класс-адаптер для работы с IScheme
    /// </summary>
    [Serializable]
    public class SchemeAdapter
    {
        public string GetOKTMO()
        {
            return ((IScheme)UpdateManager.Instance.Scheme).GlobalConstsManager.Consts["OKTMO"].Value.ToString();
        }

        public bool CheckPackageExistsByName(string packageName)
        {
            string sql = "select * from MetaPackages where name = ?";

            using (IDatabase db = ((IScheme)UpdateManager.Instance.Scheme).SchemeDWH.DB)
            {
                return (int)((IScheme)UpdateManager.Instance.Scheme).SchemeDWH.DB.ExecQuery(sql, QueryResultTypes.Scalar,
                                                        db.CreateParameter("PackageName", packageName)) == 1;
            }
        }

        public bool CheckEntityExistsByObjectKey(string entityObjectKey)
        {
            string sql = "select * from MetaObjects where objectKey = ?";

            using (IDatabase db = ((IScheme)UpdateManager.Instance.Scheme).SchemeDWH.DB)
            {
                return (int)((IScheme)UpdateManager.Instance.Scheme).SchemeDWH.DB.ExecQuery(sql, QueryResultTypes.Scalar,
                                                        db.CreateParameter("entityObjectKey", entityObjectKey)) == 1;
            }
        }

        public string GetServerModuleVersion(string moduleName)
        {
            Dictionary<string, string> dictionary =
                ((IScheme) UpdateManager.Instance.Scheme).UsersManager.GetServerAssemblyesInfo("Krista.*.dll");

            foreach (var keyValuePair in ((IScheme)UpdateManager.Instance.Scheme).UsersManager.GetServerAssemblyesInfo("Krista.FM.Common.dll"))
            {
                if (!dictionary.ContainsKey(keyValuePair.Key))
                {
                    dictionary.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            foreach (var keyValuePair in ((IScheme)UpdateManager.Instance.Scheme).UsersManager.GetServerAssemblyesInfo("Krista.FM.ServerLibrary.dll"))
            {
                if (!dictionary.ContainsKey(keyValuePair.Key))
                {
                    dictionary.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            if (dictionary.ContainsKey(moduleName))
            {
                return dictionary[moduleName];
            }

            return string.Empty;
        }
    }
}
