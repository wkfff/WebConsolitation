using System;
using System.ComponentModel;
using System.Globalization;
using Infragistics.UltraChart.Shared.Styles;

namespace Krista.FM.Client.MDXExpert
{
    class AxisIntervalTypeConverter : EnumConverter
    {
        const string itNotSet = "���";
        const string itTicks = "����";
        const string itMilliseconds = "������������";
        const string itSeconds = "�������";
        const string itMinutes = "������";
        const string itHours = "����";
        const string itDays = "���";
        const string itWeeks = "������";
        const string itMonths = "������";
        const string itYears = "����";

        public AxisIntervalTypeConverter(Type type)
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
                case itNotSet: return AxisIntervalType.NotSet;
                case itTicks: return AxisIntervalType.Ticks;
                case itMilliseconds: return AxisIntervalType.Milliseconds;
                case itSeconds: return AxisIntervalType.Seconds;
                case itMinutes: return AxisIntervalType.Minutes;
                case itHours: return AxisIntervalType.Hours;
                case itDays: return AxisIntervalType.Days;
                case itWeeks: return AxisIntervalType.Weeks;
                case itMonths: return AxisIntervalType.Months;
                case itYears: return AxisIntervalType.Years;
            }
            return AxisIntervalType.NotSet;
        }

        public static string ToString(object value)
        {
            switch ((AxisIntervalType)value)
            {
                case AxisIntervalType.NotSet: return itNotSet;
                case AxisIntervalType.Ticks: return itTicks;
                case AxisIntervalType.Milliseconds: return itMilliseconds;
                case AxisIntervalType.Seconds: return itSeconds;
                case AxisIntervalType.Minutes: return itMinutes;
                case AxisIntervalType.Hours: return itHours;
                case AxisIntervalType.Days: return itDays;
                case AxisIntervalType.Weeks: return itWeeks;
                case AxisIntervalType.Months: return itMonths;
                case AxisIntervalType.Years: return itYears;
            }
            return string.Empty;
        }
    }
}