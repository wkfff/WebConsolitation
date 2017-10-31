using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Krista.FM.Client.Design.Editors
{
    public class EnumTypeConvertor : EnumConverter
    {
        /// <summary>
        /// Использумый тип (строка)
        /// </summary>
        private Type _type;

        public EnumTypeConvertor(Type type)
            : base(type)
        {
            _type = type;
        }

        /// <summary>
        /// Возможность конвертирования
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return _type == typeof(string);
        }

        /// <summary>
        /// Преобразование к русскому описанию
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            FieldInfo fi = _type.GetField(Enum.GetName(_type, value));
            DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(
                fi, typeof(DescriptionAttribute));

            if (da != null)
                return da.Description;
            else
                return value.ToString();
        }

        /// <summary>
        /// Возможность восстановления
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <summary>
        /// Восстановление из русского описания
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            foreach (FieldInfo fi in _type.GetFields())
            {
                DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(
                    fi, typeof(DescriptionAttribute));

                if ((da != null) && (string)value == da.Description)
                    return Enum.Parse(_type, fi.Name);
            }

            return Enum.Parse(_type, value.ToString());
        }
    }
}
