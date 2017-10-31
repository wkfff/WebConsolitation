namespace Krista.FM.Client.Reports.MOFO0024.Commands
{
    class DefaultParamValues
    {
        static readonly ReportMonthMethods ReportHelper = new ReportMonthMethods(ConvertorSchemeLink.scheme);
        public const string LimitSum = "10";
        public const string LimitSumMask = "{double:-9.0}";
        public const string ShowOrg = "true";
        public const string ShowOrgTitle = "Показывать организации";
        public static string Divider = SumDividerEnum.i2.ToString();
        public static string Precision = PrecisionNumberEnum.ctN0.ToString();
        public static string CurrentYear { get { return ReportMonthMethods.Years(1); } }
        public static string ProfitTax = ReportHelper.GetKDNestedID("00010101000000000110"); // налог на прибыль
    }
}
