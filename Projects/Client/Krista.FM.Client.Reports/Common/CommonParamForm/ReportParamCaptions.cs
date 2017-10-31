using System;

namespace Krista.FM.Client.Reports.Common.CommonParamForm
{
    class ReportParamCaptions
    {
        public static string GetParamDefaultCaption(string paramName, Type enumObj)
        {
            switch (paramName)
            {
                case ReportConsts.ParamExchangeRate:
                    return "Прогнозный курс";
                case ReportConsts.ParamStartDate:
                    return "Начальная дата";
                case ReportConsts.ParamEndDate:
                    return "Конечная дата";
                case ReportConsts.ParamExchangeDate:
                    return "Дата курса";
                case ReportConsts.ParamYear:
                    return "Год";
                case ReportConsts.ParamMonth:
                    return "Месяц";
                case ReportConsts.ParamDigitNumber:
                    return "Точность вывода";
                case ReportConsts.ParamHideEmptyStr:
                    return "Скрывать пустые строки";
                case ReportConsts.ParamPhone:
                    return "Телефон";
                case ReportConsts.ParamExecutor:
                case ReportConsts.ParamExecutor1:
                case ReportConsts.ParamExecutor2:
                case ReportConsts.ParamExecutor3:
                    return "Исполнитель";
                case ReportConsts.ParamOrgID:
                    return "ИНН";
                case ReportConsts.ParamOrgName:
                    return "Название организации";
            }

            if (enumObj == typeof (VariantTypeEnum))
                return "Вариант.ИФ";
            if (enumObj == typeof (ContractTypeEnum))
                return "Статус договора";
            if (enumObj == typeof (ReportTestType))
                return "Способ тестирования";
            if (enumObj == typeof (PrecisionNumberEnum))
                return "Точность вывода значений";
            if (enumObj == typeof (SumDividerEnum))
                return "Выбор единиц измерения";
            if (enumObj == typeof (ReportPeriodEnum))
                return "Период";
            if (enumObj == typeof (RegionListTypeEnum))
                return "Типы территорий";
            if (enumObj == typeof (MonthEnum))
                return "Месяц";
            if (enumObj == typeof (QuarterEnum))
                return "Квартал";
            if (enumObj == typeof(HalfYearEnum))
                return "Полугодие";
            if (enumObj == typeof(PlanSumFieldEnum))
                return "Показатели";
            if (enumObj == typeof (MOFOContractTypeEnum))
                return "Вид договора";
            if (enumObj == typeof (VariantYearEnum))
                return "Год";

            return "Параметр";
        }
    }
}
