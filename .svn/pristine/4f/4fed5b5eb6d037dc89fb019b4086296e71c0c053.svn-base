using System;
using System.Collections.Generic;
using System.Reflection;

namespace Krista.FM.Update.Framework.Conditions.SchemeConditions
{
    /// <summary>
    /// Условие проверки версии модуля сервера
    /// </summary>
    [Serializable]
    public class ServerModuleVersionCondition : SchemeDependedCondition
    {
        public ServerModuleVersionCondition(bool local) : base(local)
        {
        }

        public ServerModuleVersionCondition() : base()
        {
        }

        public override bool IsMet(IUpdateTask task)
        {
            if (!Attributes.ContainsKey("version"))
                return false;
            if (!Attributes.ContainsKey("moduleName"))
                return false;

            if (UpdateManager.Instance.IsServerMode)
                return true;

            if (instance == null)
                if (String.IsNullOrEmpty(UpdateManager.Instance.ServerModulesString))
                    return false;

            if (UpdateManager.Instance.Scheme == null)
                if (String.IsNullOrEmpty(UpdateManager.Instance.ServerModulesString))
                    return false;

            Version updateVersion = new Version(Attributes["version"]);

            string localVersionStr = string.Empty;

            if (Attributes.ContainsKey("moduleName"))
            {
                string moduleName = Attributes["moduleName"];

                localVersionStr = UpdateManager.Instance.Scheme != null 
                    ? GetLocalVersionStrFromScheme(moduleName)
                    : GetLocalVersionStrFromString(moduleName);
            }

            if (string.IsNullOrEmpty(localVersionStr))
                return false;

            Version localVersion = new Version(localVersionStr);

            /*Trace.TraceVerbose(string.Format(
                "moduleName = {0}; Версия локального файла {1};Версия файла в патче {2}", Attributes["moduleName"], localVersion,
                updateVersion));*/

            if (Attributes.ContainsKey("what"))
            {
                switch (Attributes["what"])
                {
                    case "above":
                        return updateVersion < localVersion;
                    case "is":
                        return updateVersion == localVersion;
                    case "is-above":
                        return updateVersion <= localVersion;
                    case "is-below":
                        return updateVersion >= localVersion;
                }
            }
            return updateVersion > localVersion;
        }

        private static string GetLocalVersionStrFromString(string moduleName)
        {
            string[] modules = UpdateManager.Instance.ServerModulesString.Split(';');

            foreach (var module in modules)
            {
                if (module.Split('_').Length > 0 && module.Split('_')[0] == moduleName)
                    if (module.Split('_').Length > 1) 
                        return module.Split('_')[1];
            }

            return String.Empty;
        }

        private string GetLocalVersionStrFromScheme(string moduleName)
        {
            string localVersionStr;
            MethodInfo mi = instance.GetType().GetMethod("GetServerModuleVersion");

            localVersionStr = (string)mi.Invoke(instance, new[] { moduleName });
            return localVersionStr;
        }
    }
}
