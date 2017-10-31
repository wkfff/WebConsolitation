

using System;

namespace Krista.FM.Client.Reports.UFNS.Commands
{
    class DefaultParamValues
    {
        static readonly ReportMonthMethods ReportHelper = new ReportMonthMethods(ConvertorSchemeLink.scheme);
        public static string HideEmptyStr = Boolean.FalseString;
        public static string Divider = SumDividerEnum.i2.ToString();
        public static string Precision = PrecisionNumberEnum.ctN0.ToString();
        public static string OrgProfitTax = ReportHelper.GetKDNestedID("00010101000000000110"); // налог на прибыль организаций
        public static string OrgAssetTax = ReportHelper.GetKDNestedID("00010602000020000110");  // налог на имущество организаций
        public static string AssetTaxArrears = ReportHelper.GetArrearsFNSNestedID("2610");      // задолженность  по налогу на имущество
        public static string Arrears = ReportHelper.GetArrearsFNSNestedID("1010, 1250, 1260, 2010, 2020"); // раздел I и раздел II
        public static string CurrentYear { get { return ReportMonthMethods.Years(1); } }
        public static string TwoLastYears { get { return ReportMonthMethods.Years(2); } }
        public static string ThreeLastYears { get { return ReportMonthMethods.Years(3); } }
        public static string FourLastYears { get { return ReportMonthMethods.Years(4); } }
    }
}
