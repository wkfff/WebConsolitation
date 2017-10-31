using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

using Krista.FM.Domain.MappingAttributes;

namespace Krista.FM.Domain.Reporitory.NHibernate.Automapping
{
    public class BlobColumnLengthConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Type == typeof(byte[]))
                .Expect(x => x.Length == 0);
        }

        public void Apply(IPropertyInstance instance)
        {
            int length = int.MaxValue;

            object[] customAttributes = instance.Property.MemberInfo.GetCustomAttributes(typeof(BlobLenghtAttribute), false);
            if (customAttributes.Length > 0)
            {
                length = ((BlobLenghtAttribute)(customAttributes[0])).Length;
            }

            instance.Length(length);
        }
    }
}
