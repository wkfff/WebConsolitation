using System;
using System.Reflection;

namespace Krista.FM.Update.Framework.Conditions.SchemeConditions
{
    [Serializable]
    public class EntityExistsCondition : SchemeDependedCondition
    {
        public EntityExistsCondition(bool local) : base(local)
        {
        }

        public EntityExistsCondition()
            : base()
        {
        }

        public override bool IsMet(IUpdateTask task)
        {
            if (UpdateManager.Instance.IsServerMode)
            {
                return true;
            }

            if (instance == null)
            {
                return false;
            }

            if (UpdateManager.Instance.Scheme == null)
                return false;

            if (Attributes.ContainsKey("ObjectKey"))
            {
                // GUID 
                string entityObjectKey = Attributes["ObjectKey"];

                MethodInfo mi = instance.GetType().GetMethod("CheckEntityExistsByObjectKey");

                return (bool)mi.Invoke(instance, new[] { entityObjectKey });
            }

            return true;
        }
    }
}
