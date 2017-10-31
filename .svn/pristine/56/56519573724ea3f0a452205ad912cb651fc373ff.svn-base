using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Krista.FM.Domain.Reporitory.NHibernate.Automapping
{
    public class BlobColumnLazyLoadConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Type == typeof(byte[]));
        }

        public void Apply(IPropertyInstance instance)
        {
            instance.LazyLoad();
        }
    }
}