using System.Reflection;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Krista.FM.Domain.Reporitory.NHibernate.IoC
{
    public class UnitOfWorkMatchingRule : IMatchingRule
    {
        public bool Matches(MethodBase member)
        {
            if (member.ReflectedType.GetCustomAttributes(typeof(UnitOfWorkAttribute), false).Length > 0)
            {
                return true;
            }

            if (member.GetCustomAttributes(typeof(UnitOfWorkAttribute), false).Length > 0)
            {
                return true;
            }

            return false;
        }
    }
}
