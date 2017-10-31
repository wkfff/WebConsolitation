using System;
using System.Reflection;

namespace Krista.FM.Update.Framework.Conditions.SchemeConditions
{
    /// <summary>
    /// Проверяет наличие пакета в схеме по имени пакета
    /// </summary>
    [Serializable]
    public class PackageExistsCondition : SchemeDependedCondition
    {
        public PackageExistsCondition(bool local) : base(local)
        {
        }

        public PackageExistsCondition()
            : base()
        {
        }

        public override bool IsMet(IUpdateTask task)
        {
            if (UpdateManager.Instance.IsServerMode)
                return true;

            if (instance == null)
                return false;

            if (UpdateManager.Instance.Scheme == null)
                return false;

            if (Attributes.ContainsKey("Name"))
            {
                // имя проверяемого пакета
                string packageName = Attributes["Name"];

                MethodInfo mi = instance.GetType().GetMethod("CheckPackageExistsByName");

                return (bool)mi.Invoke(instance, new [] { packageName });
            }

            return true;
        }
    }
}
