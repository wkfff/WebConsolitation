using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Krista.FM.Domain.Reporitory.NHibernate.Automapping
{
    /// <summary>
    /// This is a convention that will be applied to all entities in your application. What this particular
    /// convention does is to specify that many-to-one, one-to-many, and many-to-many relationships will all
    /// have their Cascade option set to All.
    /// </summary>
    public class CascadeConvention : IReferenceConvention, IHasManyConvention, IHasManyToManyConvention, ICollectionConvention
    {
        public void Apply(IManyToOneInstance instance)
        {
            instance.Cascade.SaveUpdate();
        }

        public void Apply(IOneToManyCollectionInstance instance)
        {
            instance.Cascade.AllDeleteOrphan();
        }

        public void Apply(IManyToManyCollectionInstance instance)
        {
            instance.Cascade.All();
        }

        public void Apply(ICollectionInstance instance)
        {
            instance.Inverse();
            instance.Cascade.AllDeleteOrphan();
        }
    }
}
