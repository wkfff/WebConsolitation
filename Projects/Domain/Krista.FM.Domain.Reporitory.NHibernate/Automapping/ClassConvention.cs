using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Krista.FM.Domain.Reporitory.NHibernate.Automapping
{
    public class ClassConvention : IClassConvention, IClassConventionAcceptance
    {
        public void Apply(IClassInstance instance)
        {
            var name = instance.EntityType.Name;
            instance.Table(name.ToUpper());

            // Фиктированные классификаторы делаем только для чтения и кешируем
            if (name.StartsWith("FX_"))
            {
                instance.ReadOnly();

                instance.Cache.ReadOnly();
                instance.Cache.Region("FixedCls");
            }
        }

        public void Accept(IAcceptanceCriteria<IClassInspector> criteria)
        {
            criteria.Expect(x => x.EntityType.IsClass && !x.EntityType.IsAbstract);
        }
    }
}
