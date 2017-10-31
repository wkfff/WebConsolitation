using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

using Krista.FM.Extensions;

namespace Krista.FM.Domain.Reporitory.NHibernate.Automapping
{
    public class OracleIdConvention : IIdConvention
    {
        public void Apply(IIdentityInstance instance)
        {
            if (instance.EntityType.Name[0] == 'x')
            {
                // Для таблиц форм сбора, которые имеют префикс x, не используем никаких
                // генераторов. Значение суррогатного ключа должно быть установлено программой.
                instance.GeneratedBy.Assigned();
            }
            else
            {
                instance.GeneratedBy.Sequence("g_{0}".FormatWith(instance.EntityType.Name));
            }
        }
    }
}
