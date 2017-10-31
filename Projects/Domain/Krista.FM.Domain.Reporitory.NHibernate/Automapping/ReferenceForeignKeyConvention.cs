using System;

using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

using Krista.FM.Domain.MappingAttributes;

namespace Krista.FM.Domain.Reporitory.NHibernate.Automapping
{
    public class ReferenceForeignKeyConvention : ForeignKeyConvention, ICollectionConvention, IJoinedSubclassConvention
    {
        public new void Apply(ICollectionInstance instance)
        {
            object[] attributes = instance.Member.GetCustomAttributes(typeof(ReferenceFieldAttribute), true);
            if (attributes.Length > 0)
            {
                instance.Key.Column(((ReferenceFieldAttribute)attributes[0]).Name);
            }
            else
            {
                throw new Exception(String.Format("Для свойства {0}.{1} нужно указать ссылочное поле с помощью атрибута [ReferenceField].", instance.EntityType.FullName, instance.Member.Name));
            }
        }

        public new void Apply(IJoinedSubclassInstance instance)
        {
            instance.Key.Column("ID");
        }

        protected override string GetKeyName(FluentNHibernate.Member property, Type type)
        {
            return property.Name;
        }
    }
}