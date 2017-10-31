using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class SortStyleTypeConverter : EnumConverter
    {
        const string ssNone = "Нет";
        const string ssAscending = "По возрастанию";
        const string ssDescending = "По убыванию";

        public SortStyleTypeConverter(Type type)
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
            switch ((string)value)
            {
                case ssNone: return SortStyle.None;
                case ssAscending: return SortStyle.Ascending;
                case ssDescending: return SortStyle.Descending;
            }
            return SortStyle.Ascending;
        }

        public static string ToString(object value)
        {
            switch ((SortStyle)value)
            {
                case SortStyle.None: return ssNone;
                case SortStyle.Ascending: return ssAscending;
                case SortStyle.Descending: return ssDescending;
            }
            return string.Empty;
        }
    }
}