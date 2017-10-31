using System;

namespace Krista.FM.Domain.MappingAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnorePropertyAttribute : Attribute
    {
    }
}
