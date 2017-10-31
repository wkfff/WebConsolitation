using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Lvls;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.MOFO;
using Krista.FM.Client.Reports.Month.Fillers;
using NPOI.HSSF.UserModel;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Variants;

namespace Krista.FM.Client.Reports
{
    class DefaultParamValues
    {
        public const string FullLevels = "0,1,2,3,4,5,6,7";
        public static string Divider = SumDividerEnum.i2.ToString();
        public static string Precision = PrecisionNumberEnum.ctN0.ToString();
        public const string LimitSum = "1";
        public const string HideEmptyStr = "false";
        public static string CurrentYearStart = ReportDataServer.GetYearStart(DateTime.Now.Year);
        public const string YearPrev3 = "3";
        public const string YearPrev2 = "4";
        public const string YearPrev1 = "5";
        public const string YearCurrent = "6";
        public const string Last3Years = "3,4,5";
        public const string ConsBudget = "0";
        public const string AllLevels = "0,1,2";
        public const string AllMarks = "1,2,3,4,5";
        public const string MarkIncome = "3";
        public static string CurrentYear { get { return ReportMonthMethods.Years(1); } }
    }

    class DebugValues
    {
       public const string KD = "350004,350008,350013,350009,350010,350011,350006,350012,350014,350007,350005"; 
    }

    [Description("ReportMonth001")]
    public class ReportMonth001Command : CommonReportsCommand
    {
        public ReportMonth001Command()
        {
            key = "ReportMonth001";
            caption = "ДИНАМИКА ЕЖЕМЕСЯЧНЫХ ПОСТУПЛЕНИЙ В БЮДЖЕТЫ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.Last3Years);
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamMultiBdgtLvls())
                .SetValue(DefaultParamValues.ConsBudget);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport001DynamicIncomeData(reportParams);
        }
    }

    [Description("ReportMonth002")]
    public class ReportMonth002Command : CommonReportsCommand
    {
        public ReportMonth002Command()
        {
            key = "ReportMonth002";
            caption = "УДЕЛЬНЫЙ ВЕС ЕЖЕМЕСЯЧНЫХ И КВАРТАЛЬНЫХ ПОСТУПЛЕНИЙ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.YearPrev2);
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamSingleBdgtLvls())
                .SetValue(DefaultParamValues.ConsBudget);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof (SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof (PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport002SpecificGravityData(reportParams);
        }
    }

    [Description("ReportMonth003")]
    public class ReportMonth003Command : CommonReportsCommand
    {
        public ReportMonth003Command()
        {
            key = "ReportMonth003";
            caption = "ИСПОЛНЕНИЕ ПЛАНА КОНСОЛИДИРОВАННОГО БЮДЖЕТА ОБЛАСТИ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.YearPrev2);
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamSingleBdgtLvls())
                .SetValue(DefaultParamValues.ConsBudget);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport003ExecutingBudgetPlanData(reportParams);
        }
    }

    [Description("ReportMonth004")]
    public class ReportMonth004Command : CommonReportsCommand
    {
        public ReportMonth004Command()
        {
            key = "ReportMonth004";
            caption = "ИСПОЛНЕНИЕ МЕСТНОГО БЮДЖЕТА В РАЗРЕЗЕ МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.YearCurrent);
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport004ExecutingMunicipalBudgetData(reportParams);
        }
    }

    [Description("ReportMonth005")]
    public class ReportMonth005Command : CommonReportsCommand
    {
        public ReportMonth005Command()
        {
            key = "ReportMonth005";
            caption = "ДИНАМИКА ЕЖЕМЕСЯЧНЫХ  ПОСТУПЛЕНИЙ В КБС ПО НЕСКОЛЬКИМ КБК";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.Last3Years);
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamSingleBdgtLvls())
                .SetValue(DefaultParamValues.ConsBudget);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport005DynamicIncomeKDData(reportParams);
        }
    }

    [Description("ReportMonth006")]
    public class ReportMonth006Command : ExcelDirectCommand
    {
        public ReportMonth006Command()
        {
            key = "ReportMonth006";
            caption = "ДАННЫЕ ПО ФАКТИЧЕСКОМУ ИСПОЛНЕНИЮ И ПЛАНОВЫМ НАЗНАЧЕНИЯМ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddListParam(ReportConsts.ParamOutputMode)
                .SetValue("0,1")
                .SetCaption("Показатели")
                .SetMultiSelect(true)
                .Values = new List<object> { "Исполнено", "Назначено" };
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.YearCurrent);
            paramBuilder.AddEnumParam(ReportConsts.ParamMonth, typeof(MonthEnum));
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport006BudgetAppointmentData(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new Report006Filler();
            reportFiller.FillMonthReport(wb, tableList);
        }
    }

    [Description("ReportMonth007")]
    public class ReportMonth007Command : CommonReportsCommand
    {
        public ReportMonth007Command()
        {
            key = "ReportMonth007";
            caption = "ИСПОЛНЕНИЕ ОБЛАСТНОГО БЮДЖЕТА В РАЗРЕЗЕ МУНИЦИПАЛЬНЫХ ОБРАЗОВАНИЙ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamKVSRComparable, new ParamKVSRBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.YearPrev1);
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonth007ExecutingSubjectBudgetData(reportParams);
        }
    }

    [Description("ReportMonth008")]
    public class ReportMonth008Command : ReportMonth004Command
    {
        public ReportMonth008Command()
        {
            key = "ReportMonth008";
            caption = "Исполнение консолидированного бюджета в разрезе муниципальных образований";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.YearCurrent);
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport008ExecutingMunicipalBudgetData(reportParams);
        }
    }

    [Description("ReportMonth009")]
    public class ReportMonth009Command : ExcelDirectCommand
    {
        public ReportMonth009Command()
        {
            key = "ReportMonth009";
            caption = "Данные об исполнении консолидированного бюджета по доходному источнику";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamMark, new ParamMarksInpayments())
                .SetValue(DefaultParamValues.MarkIncome);
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.YearPrev1);
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport009ExecutionYearConsBudgetData(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new Report009Filler();
            reportFiller.FillMonthReport(wb, tableList);
        }
    }

    [Description("ReportMonth010")]
    public class ReportMonth010Command : CommonReportsCommand
    {
        public ReportMonth010Command()
        {
            key = "ReportMonth010";
            caption = "ПОСТУПЛЕНИЯ В ОБЛАСТНОЙ БЮДЖЕТ ПО КБК И АДМИНИСТРАТОРАМ В РАЗРЕЗЕ ОМСУ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate)
                .SetValue(DefaultParamValues.CurrentYearStart);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamKVSRComparable, new ParamKVSRBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamMark, new ParamMarksInpayments())
                .SetValue(DefaultParamValues.AllMarks);
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport010IncomeSubjectBudgetData(reportParams);
        }
    }

    [Description("ReportMonth011")]
    public class ReportMonth011Command : CommonReportsCommand
    {
        public ReportMonth011Command()
        {
            key = "ReportMonth011";
            caption = "ПОСТУПЛЕНИЯ В ОБЛАСТНОЙ БЮДЖЕТ ПО МУНИЦИПАЛЬНЫМ ОБРАЗОВАНИЯМ И АДМИНИСТРАТОРАМ В РАЗРЕЗЕ КБК";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate)
                .SetValue(DefaultParamValues.CurrentYearStart);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamRegionComparable, new ParamRegionBridgeFull());
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamKVSRComparable, new ParamKVSRBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamMark, new ParamMarksInpayments())
                .SetValue(DefaultParamValues.AllMarks);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport011IncomeSubjectBudgetRegionData(reportParams);
        }
    }

    [Description("ReportMonth012")]
    public class ReportMonth012Command : ReportMonth004Command
    {
        public ReportMonth012Command()
        {
            key = "ReportMonth012";
            caption = "АНАЛИЗ ПОСТУПЛЕНИЙ ПО НДФЛ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.YearCurrent);
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport012AnalisysIncomeNDFLData(reportParams);
        }
    }

    [Description("ReportMonth013")]
    public class ReportMonth013Command : CommonReportsCommand
    {
        public ReportMonth013Command()
        {
            key = "ReportMonth013";
            caption = "АНАЛИЗ КОНТИНГЕНТА ПО НДФЛ В ЦЕЛОМ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.Last3Years);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport013DynamicNDFLData(reportParams);
        }
    }

    [Description("ReportMonth014")]
    public class ReportMonth014Command : CommonReportsCommand
    {
        public ReportMonth014Command()
        {
            key = "ReportMonth014";
            caption = "ИСПОЛНЕНИЕ ПЛАНА КОНСОЛИДИРОВАННОГО БЮДЖЕТА ОБЛАСТИ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.YearPrev1);
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamSingleBdgtLvls())
                .SetValue(DefaultParamValues.ConsBudget);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport014PlanExecutingKBKData(reportParams);
        }
    }

    [Description("ReportMonth014_01")]
    public class ReportMonth014_01Command : CommonReportsCommand
    {
        public ReportMonth014_01Command()
        {
            key = "ReportMonth014_01";
            caption = "ИСПОЛНЕНИЕ ПЛАНА КОНСОЛИДИРОВАННОГО БЮДЖЕТА ОБЛАСТИ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.YearPrev1);
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamSingleBdgtLvls())
                .SetValue(DefaultParamValues.ConsBudget);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport014_01_PlanExecutingKBKData(reportParams);
        }
    }

    [Description("ReportMonth015")]
    public class ReportMonth015Command : CommonReportsCommand
    {
        public ReportMonth015Command()
        {
            key = "ReportMonth015";
            caption = "АНАЛИЗ ЕЖЕМЕСЯЧНЫХ ПОСТУПЛЕНИЙ ПО АКЦИЗАМ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.Last3Years);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport015DynamicExciseData(reportParams);
        }
    }

    [Description("ReportMonth016")]
    public class ReportMonth016Command : CommonReportsCommand
    {
        public ReportMonth016Command()
        {
            key = "ReportMonth016";
            caption = "ДИНАМИКА ЕЖЕМЕСЯЧНЫХ ПОСТУПЛЕНИЙ В БЮДЖЕТЫ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.Last3Years);
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamMultiBdgtLvls())
                .SetValue(DefaultParamValues.ConsBudget);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport016AnalisysDynamicIncomeData(reportParams);
        }
    }

    [Description("ReportMonth017")]
    public class ReportMonth017Command : CommonReportsCommand
    {
        public ReportMonth017Command()
        {
            key = "ReportMonth017";
            caption = "ВЫВОД СУММ ПЛАНА НА ТЕКУЩИЙ ГОД ПО ДОХОДНЫМ ИСТОЧНИКАМ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.YearPrev1);
            paramBuilder.AddEnumParam(ReportConsts.ParamMonth, typeof(MonthEnum));
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridgeLine());
            paramBuilder.AddBookParam(ReportConsts.ParamKVSRComparable, new ParamKVSRBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamMultiBdgtLvls())
                .SetValue(DefaultParamValues.AllLevels);
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport017PlanKBKData(reportParams);
        }
    }

    [Description("ReportMonth018")]
    public class ReportMonth018Command : CommonReportsCommand
    {
        public ReportMonth018Command()
        {
            key = "ReportMonth018";
            caption = "ВЫВОД СПРОГНОЗИРОВАННЫХ СУММ НА ТЕКУЩИЙ ГОД, ОЧЕРЕДНОЙ ФИНАНСОВЫЙ ГОД И ПЛАНОВЫЙ ПЕРИОД";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantDID, new ParamVariantIncomePlan());
            paramBuilder.AddEnumParam(ReportConsts.ParamSum, typeof(PlanSumFieldEnum));
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamYearList())
                .SetValue(DefaultParamValues.YearPrev1);
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridgeLine());
            paramBuilder.AddBookParam(ReportConsts.ParamKVSRComparable, new ParamKVSRBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamMultiBdgtLvls())
                .SetValue(DefaultParamValues.AllLevels);
            paramBuilder.AddBookParam(ReportConsts.ParamRegionLevels, new ParamMultiRegionLvls())
                .SetValue("0,1");
            paramBuilder.AddEnumParam(ReportConsts.ParamRegionListType, typeof(RegionListTypeEnum));
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);

            paramBuilder.paramFilters[ReportConsts.ParamBdgtLevels] =
                String.Format("{0}={1};{2}", ReportConsts.ParamSum, PlanSumFieldEnum.i1, PlanSumFieldEnum.i2);
            paramBuilder.paramFilters[ReportConsts.ParamRegionLevels] =
                String.Format("{0}={1}", ReportConsts.ParamSum, PlanSumFieldEnum.i3);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport018ForecastKDIncomeData(reportParams);
        }
    }

    [Description("ReportMonth019")]
    public class ReportMonth019Command : CommonReportsCommand
    {
        public ReportMonth019Command()
        {
            key = "ReportMonth019";
            caption = "СВОДНАЯ ФОРМА ВЫВОДА СПРОГНОЗИРОВАННЫХ СУММ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantDID, new ParamVariantIncomePlan());
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamKVSRComparable, new ParamKVSRBridge());
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport019VaultFormForecastData(reportParams);
        }
    }

    [Description("ReportMonth021")]
    public class ReportMonth021Command : CommonReportsCommand
    {
        public ReportMonth021Command()
        {
            key = "ReportMonth021";
            caption = "АНАЛИЗ ПОСТУПЛЕНИЙ В БЮДЖЕТ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantDID, new ParamVariantIncomePlan());
            paramBuilder.AddBookParam(ReportConsts.ParamGroupKD, new ParamGroupKD());
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamBdgtLevels, new ParamSingleBdgtLvls())
                .SetValue(DefaultParamValues.ConsBudget);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport021AnalysIncomeData(reportParams);
        }
    }

    [Description("ReportMonth022")]
    public class ReportMonth022Command : CommonReportsCommand
    {
        public ReportMonth022Command()
        {
            key = "ReportMonth022";
            caption = "ИСПОЛНЕНИЕ БЮДЖЕТА ЗА МЕСЯЦ В РАЗРЕЗЕ ДОХОДНЫХ ИСТОЧНИКОВ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamYear, new ParamSingleYear())
                .SetValue(DefaultParamValues.YearPrev2);
            paramBuilder.AddEnumParam(ReportConsts.ParamMonth, typeof(MonthEnum));
            paramBuilder.AddBookParam(ReportConsts.ParamGroupKD, new ParamGroupKD());
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport022ExecuteMonthIncomeData(reportParams);
        }
    }

    [Description("ReportMonth025")]
    public class ReportMonth025Command : CommonReportsCommand
    {
        public ReportMonth025Command()
        {
            key = "ReportMonth025";
            caption = "Расчетные показатели";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantDID, new ParamVariantIncomePlan());
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport025CalcMeasuresData(reportParams);
        }
    }

    [Description("ReportMonth026")]
    public class ReportMonth026Command : ExcelDirectCommand
    {
        public ReportMonth026Command()
        {
            key = "ReportMonth026";
            caption = "ВЫВОД СУММ ПО РАСЧЕТНОМУ НАЛОГОВОМУ ПОТЕНЦИАЛУ";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantDID, new ParamVariantIncomePlan());
            paramBuilder.AddEnumParam(ReportConsts.ParamYear, typeof(VariantYearEnum));
            paramBuilder.AddBookParam(ReportConsts.ParamKDComparable, new ParamKDBridge());
            paramBuilder.AddBookParam(ReportConsts.ParamOutputMode, new ParamMultiRegionLvls())
                .SetValue("0,1");
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum))
                .SetValue(DefaultParamValues.Divider);
            paramBuilder.AddEnumParam(ReportConsts.ParamPrecision, typeof(PrecisionNumberEnum))
                .SetValue(DefaultParamValues.Precision);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMonthReport026TaxPotentionData(reportParams);
        }

        protected override void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
            var reportFiller = new Report026Filler();
            reportFiller.FillMonthReport(wb, tableList);
        }
    }
}
