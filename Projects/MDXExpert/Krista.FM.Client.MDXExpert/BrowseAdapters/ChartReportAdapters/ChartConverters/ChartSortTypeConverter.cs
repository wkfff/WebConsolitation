using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class ChartSortTypeConverter : EnumConverter
    {
        const string csNone = "Нет";
        const string csAscending = "По возрастанию";
        const string csDescending = "По убыванию";

        public ChartSortTypeConverter(Type type)
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
                case csNone: return ChartSortType.None;
                case csAscending: return ChartSortType.Ascending;
                case csDescending: return ChartSortType.Descending;
            }
            return ChartSortType.Ascending;
        }

        public static string ToString(object value)
        {
            switch ((ChartSortType)value)
            {
                case ChartSortType.None: return csNone;
                case ChartSortType.Ascending: return csAscending;
                case ChartSortType.Descending: return csDescending;
            }
            return string.Empty;
        }
    }
}