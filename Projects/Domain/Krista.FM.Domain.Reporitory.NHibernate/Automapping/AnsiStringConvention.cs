using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

using Krista.FM.Domain.MappingAttributes;

namespace Krista.FM.Domain.Reporitory.NHibernate.Automapping
{
    public class AnsiStringConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Type == typeof(string));
        }

        public void Apply(IPropertyInstance instance)
        {
            if (0 < instance.Property.MemberInfo.GetCustomAttributes(typeof(AnsiStringAttribute), false).Length)
            {
                instance.CustomType("AnsiString");
            }
        }
    }
}
