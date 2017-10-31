using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

using Krista.FM.Domain.Reporitory.NHibernate.MsSql;

namespace Krista.FM.Domain.Reporitory.NHibernate.Automapping
{
    public class MsSqlIdConvention : IIdConvention
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
                if (instance.EntityType.BaseType.Name == "FactTable" ||
                    instance.EntityType.BaseType.Name == "DebtorBookFactBase" ||
                    instance.EntityType.BaseType.Name == "DebtorBookStructureServiceBase")
                {
                    instance.GeneratedBy.Identity();
                }
                else
                {
                    instance.GeneratedBy.Custom<MsSqlIdGenerator>();
                }
            }
        }
    }
}
