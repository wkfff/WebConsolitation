using System;
using System.Linq;
using System.Reflection;

namespace Krista.FM.Update.Framework.Conditions.SchemeConditions
{
    [Serializable]
    public class OKTMOCondition : SchemeDependedCondition
    {
        public OKTMOCondition(bool local) : base(local)
        {
        }

        public OKTMOCondition()
            : base()
        {
        }

        public override bool IsMet(IUpdateTask task)
        {
            if (instance == null)
                return false;
            
            if (Attributes.ContainsKey("OKTMO"))
            {
                // набор ОКТМО
                string[] values = Attributes["OKTMO"].Split(',');

                string oktmo = String.Empty;

                if (UpdateManager.Instance.IsServerMode)
                {
                    // Для службы автоматического обновления получаем ОКТМО из конфига
                    oktmo = UpdateManager.Instance.OKTMO;
                }
                else
                {
                    if (UpdateManager.Instance.Scheme == null)
                        return false;

                    MethodInfo mi = instance.GetType().GetMethod("GetOKTMO");
                    oktmo = (string) mi.Invoke(instance, null);
                }

                return values.Any(value => String.Equals(
                    value.Trim().Replace(" ", String.Empty),
                    oktmo.Trim().Replace(" ", String.Empty)));
            }

            return true;
        }
    }
}
