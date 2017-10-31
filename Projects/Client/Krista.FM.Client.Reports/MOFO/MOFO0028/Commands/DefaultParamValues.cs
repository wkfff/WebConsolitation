namespace Krista.FM.Client.Reports.MOFO0028.Commands
{
    class DefaultParamValues
    {
        public static string Divider = SumDividerEnum.i2.ToString();
        public static string Precision = PrecisionNumberEnum.ctN0.ToString();
        public static string CurrentYear { get { return ReportMonthMethods.Years(1); } }
        public static string HideEmptyStr = System.Boolean.FalseString;
    }
}
