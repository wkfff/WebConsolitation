using FluentNHibernate.Mapping;

namespace Krista.FM.Domain.Reporitory.NHibernate.FluentMappings.System
{
    public class HashObjectsNamesMap : ClassMap<HashObjectsNames>
    {
        public HashObjectsNamesMap()
        {
            CompositeId()
                .KeyProperty(x => x.HashName)
                .KeyProperty(x => x.LongName);

            Map(x => x.ObjectType);
        }
    }
}