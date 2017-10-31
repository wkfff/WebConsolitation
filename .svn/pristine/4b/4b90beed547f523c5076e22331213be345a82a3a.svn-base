using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Krista.FM.Common
{
    public class LogMatchingRule : IMatchingRule
    {
        public bool Matches(MethodBase member)
        {
            if (member.ReflectedType.Assembly.GetCustomAttributes(typeof(LogAttribute), false).Length > 0)
            {
                return true;
            }

            if (member.ReflectedType.GetCustomAttributes(typeof(LogAttribute), false).Length > 0)
            {
                return true;
            }

            if (member.GetCustomAttributes(typeof(LogAttribute), false).Length > 0)
            {
                return true;
            }

            return false;
        }
    }
}
