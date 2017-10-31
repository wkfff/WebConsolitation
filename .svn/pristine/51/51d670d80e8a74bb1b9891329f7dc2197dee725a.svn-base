using System;
using System.ComponentModel;
using System.Globalization;
using Dundas.Maps.WinControl;
using System.Drawing;

namespace Krista.FM.Client.MDXExpert
{
    class DataGroupingConverter : EnumConverter
    {
        const string dgEqualDistribution = "Равномерное распределение";
        const string dgEqualInterval = "Равные интервалы";
        const string dgOptimal = "Оптимальное";

        public DataGroupingConverter(Type type)
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
                case dgEqualDistribution: return DataGrouping.EqualDistribution;
                case dgEqualInterval: return DataGrouping.EqualInterval;
                case dgOptimal: return DataGrouping.Optimal;
            }
            return DataGrouping.EqualInterval;
        }

        public static string ToString(object value)
        {
            switch ((DataGrouping)value)
            {
                case DataGrouping.EqualDistribution: return dgEqualDistribution;
                case DataGrouping.EqualInterval: return dgEqualInterval;
                case DataGrouping.Optimal: return dgOptimal;
            }
            return string.Empty;
        }
    }
}