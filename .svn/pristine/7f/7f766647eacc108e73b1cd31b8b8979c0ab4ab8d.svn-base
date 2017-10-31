using System;

namespace Krista.FM.Domain.MappingAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReferenceFieldAttribute : Attribute
    {
        public ReferenceFieldAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
