using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Krista.FM.Client.Design.Editors
{
    public class BooleanTypeConvertor : BooleanConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            return ((bool)value) ? "Да" : "Нет";
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return (string)value == "Да";
        }
    }
}
