using System;
using System.Collections.Generic;
using System.ComponentModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    /// <summary>
    /// Отображает список атрибутов родительского объекта
    /// </summary>
    public class AttributesListConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            return value;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            IClassifier classifier = (IClassifier)context.Instance.GetType().InvokeMember("Parent", System.Reflection.BindingFlags.GetProperty, null, context.Instance, null);
            IDictionary<string, IDataAttribute> propertyValue = classifier.Attributes;
            return new StandardValuesCollection(GetAttributesNames(propertyValue));
        }

        protected virtual string[] GetAttributesNames(IDictionary<string, IDataAttribute> propertyValue)
        {
            List<string> list = new List<string>();
            foreach (IDataAttribute item in propertyValue.Values)
            {
                list.Add(item.Caption + " (" + item.Name + ")");
            }
            return list.ToArray();
        }

        /// <summary>
        /// Извлекает Name из строки формата "Наименование (Name)"
        /// </summary>
        /// <param name="source">Исходная строка</param>
        /// <returns>Английское наименование</returns>
        public static string ExtractName(string source)
        {
            string[] parts = source.Split('(', ')');
            if (parts.GetLength(0) < 2)
            {
                throw new ExpectedIdentifierException();
            }
            return parts[parts.GetLength(0) - 2];
        }
    }

    public class ExpectedIdentifierException : ApplicationException
    {
    }

    public class EntityAttributesListConverter : AttributesListConverter
    {
        protected override string[] GetAttributesNames(IDictionary<string, IDataAttribute> propertyValue)
        {
            List<string> list = new List<string>();
            foreach (IDataAttribute item in propertyValue.Values)
            {
                if (item.Class != DataAttributeClassTypes.Reference &&
                    item.Name != "RowType")
                {
                    list.Add(item.Caption + " (" + item.Name + ")");
                }
            }
            return list.ToArray();
        }
    }
}
