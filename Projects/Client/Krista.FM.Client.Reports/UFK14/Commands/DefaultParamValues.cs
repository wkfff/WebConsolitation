using System;
using Krista.FM.Client.Reports.Database.ClsBridge;

namespace Krista.FM.Client.Reports.UFK14.Commands
{
    class DefaultParamValues
    {
        static readonly ReportMonthMethods ReportHelper = new ReportMonthMethods(ConvertorSchemeLink.scheme);
        public const string FullLevels = "0,1,2,3,4,5,6,7";
        public const string LimitSum = "1";
        public const string LimitSumMask = "{double:-9.3}";
        public const string HideEmptyStr = "false";
        public const string ShowOrg = "true";
        public const string ShowOrgTitle = "Показывать организации";
        public const string ConsBudget = "0";
        public const string SubjectBudget = "2";
        public static string Divider = SumDividerEnum.i2.ToString();
        public static string Precision = PrecisionNumberEnum.ctN0.ToString();
        public static string CurrentYearStart = ReportDataServer.GetYearStart(DateTime.Now.Year);
        public static string ProfitTax = ReportHelper.GetKDNestedID(String.Format("{0}, {1}", b_KD_Bridge.CodeNP, b_KD_Bridge.CodeNDFL)); // налог на прибыль + НДФЛ
        public static string CurrentYear { get { return ReportMonthMethods.Years(1); } }
    }
}
