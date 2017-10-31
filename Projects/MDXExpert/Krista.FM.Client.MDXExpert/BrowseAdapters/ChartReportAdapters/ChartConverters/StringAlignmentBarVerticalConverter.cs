using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace Krista.FM.Client.MDXExpert
{
    public class StringAlignmentBarVerticalConverter : EnumConverter
    {
        const string sCenter = "По центру";
        const string sFar = "По нижнему краю";
        const string sNear = "По верхнему краю";

        public StringAlignmentBarVerticalConverter(Type type)
            : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
            Type destType)
        {
            return ToString(value);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            StringAlignment result = StringAlignment.Far;
            string sValue = (string)value;
            if (sValue == sCenter)
                result = StringAlignment.Center;
            else
                if (sValue == sFar)
                    result = StringAlignment.Far;
                else
                    if (sValue == sNear)
                        result = StringAlignment.Near;
            return result;
        }

        public static string ToString(object value)
        {
            string result = string.Empty;
            switch ((StringAlignment)value)
            {
                case StringAlignment.Center: result = sCenter; break;
                case StringAlignment.Far: result = sFar; break;
                case StringAlignment.Near: result = sNear; break;
            }
            return result;
        }
    }
 
}
