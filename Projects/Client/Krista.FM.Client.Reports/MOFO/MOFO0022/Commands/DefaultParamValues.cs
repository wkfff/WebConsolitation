namespace Krista.FM.Client.Reports.MOFO0022.Commands
{
    class DefaultParamValues
    {
        public static string Divider = SumDividerEnum.i2.ToString();
        public static string Precision = PrecisionNumberEnum.ctN2.ToString();
        public static string ContractType = MOFOContractTypeEnum.i3.ToString();
        public static string CurrentYear { get { return ReportMonthMethods.Years(1); } }
        public static string HideEmptyStr = System.Boolean.FalseString;
    }
}
