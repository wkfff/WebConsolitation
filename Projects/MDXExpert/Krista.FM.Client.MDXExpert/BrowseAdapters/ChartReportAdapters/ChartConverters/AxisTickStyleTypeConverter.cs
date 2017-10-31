using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert.BrowseAdapters.ChartReportAdapters.ChartConverters
{
    class AxisTickStyleTypeConverter : EnumConverter
    {
        const string tsDataInterval = "Интервальное";
        const string tsPercentage = "Процентное";
        const string tsSmart = "Интеллектуальное";

        public AxisTickStyleTypeConverter(Type type)
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
                case tsDataInterval: return AxisTickStyle.DataInterval;
                case tsPercentage: return AxisTickStyle.Percentage;
                case tsSmart: return AxisTickStyle.Smart;
            }
            return AxisTickStyle.Percentage;
        }

        public static string ToString(object value)
        {
            switch ((AxisTickStyle)value)
            {
                case AxisTickStyle.DataInterval: return tsDataInterval;
                case AxisTickStyle.Percentage: return tsPercentage;
                case AxisTickStyle.Smart: return tsSmart;
            }
            return string.Empty;
        }
    }
}
