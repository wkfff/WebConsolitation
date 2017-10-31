namespace Krista.FM.RIA.Extensions.E86N.Services.StateTaskService
{
    /// <summary>
    /// Детализации документа
    /// </summary>
    public enum Details2016
    {
        /// <summary>
        /// Государственное (муниципальное) задание
        /// </summary>
        StateTask,

        /// <summary>
        /// Порядок контроля за исполнением
        /// </summary>
        MonitoringExecution,

        /// <summary>
        /// Требование к отчетности об исполнении
        /// </summary>
        RequirementForReportingOnPerformance,

        /// <summary>
        /// Категории потребителей
        /// </summary>
        ConsumerCategory,

        /// <summary>
        /// Показатели оказания услуги
        /// </summary>
        IndicatorsOfService,

        /// <summary>
        /// Реквизиты НПА, устанавливающих цены
        /// </summary>
        RequisitesNpa,

        /// <summary>
        /// НПА, регулирующий порядок оказания услуги
        /// </summary>
        NpaRegulatesService,

        /// <summary>
        /// Порядок информирования потребителей
        /// </summary>
        InformConsumers,

        /// <summary>
        /// Требование к отчетности
        /// </summary>
        ReportingRequirements,

        /// <summary>
        /// Основания для приостановления\прекращения
        /// </summary>
        GroundsForTermination,

        /// <summary>
        /// Иная информация, необходимая для исполнения
        /// </summary>
        GroundsForOtherInfo,

        /// <summary>
        /// Платность
        /// </summary>
        PayService,

        /// <summary>
        /// Среднегодовой размер платы (цена, тариф)
        /// </summary>
        AveragePrice,

        /// <summary>
        /// Отчет(ы) о выполнении государственного (муниципального) задания
        /// </summary>
        Reports,
    }

    /// <summary>
    /// Поля для "Показатели оказания услуги"
    /// </summary>
    public enum IndicatorsOfServiceFields2016
    {
        /// <summary>
        /// ID
        /// </summary>
        ID,

        /// <summary>
        /// Фактическое значение
        /// </summary>
        ActualValue,

        /// <summary>
        /// Отчетный год
        /// </summary>
        ReportingYear,

        /// <summary>
        /// Очередной год
        /// </summary>
        ComingYear,

        /// <summary>
        /// Первый плановый год
        /// </summary>
        FirstPlanYear,

        /// <summary>
        /// Отклонение, превышающее допустимое (возможное) значение
        /// </summary>
        Reject,

        /// <summary>
        /// Причина отклонения
        /// </summary>
        Protklp,

        /// <summary>
        /// Текущий год
        /// </summary>
        CurrentYear,

        /// <summary>
        /// Второй плановый год
        /// </summary>
        SecondPlanYear,

        /// <summary>
        /// Единица измерения
        /// </summary>
        PnrOkei,

        /// <summary>
        /// Тип показателя
        /// </summary>
        PnrType,

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        Info,

        /// <summary>
        /// Источник информации
        /// </summary>
        Source,

        /// <summary>
        /// Источник информации о фактическом значении показателя
        /// </summary>
        SourceInfFact,

        /// <summary>
        /// Показатели объема и качества услуг/работ код из справочника
        /// </summary>
        RefIndicators,

        /// <summary>
        /// Показатели объема и качества услуг/работ разыменовка из справочника
        /// </summary>
        RefIndicatorsName,

        /// <summary>
        /// Допустимое (возможное) отклонение (%)
        /// </summary>
        Deviation,
    }

    /// <summary>
    /// Поля для "Требование к отчетности"
    /// </summary>
    public enum ReportingRequirementsFields2016
    {
        /// <summary>
        /// ID
        /// </summary>
        ID,

        /// <summary>
        /// Периодичность предоставления отчетов о выполнении государственного (муниципального) задания
        /// </summary>
        PeriodicityTerm,

        /// <summary>
        /// Сроки предоставления отчетов
        /// </summary>
        DeliveryTerm,

        /// <summary>
        /// Иное требование к отчетности
        /// </summary>
        OtherRequest,

        /// <summary>
        /// Иные показатели, связанные с выполнением государственного (муниципального) задания
        /// </summary>
        OtherIndicators,

        /// <summary>
        /// Форма отчета
        /// </summary>
        ReportForm
    }

    public enum ReportsFields2016
    {
        /// <summary>
        /// Уникальный идентификатор отчета
        /// </summary>
        ReportGuid,

        /// <summary>
        /// Наименование  отчета
        /// </summary>
        NameReport,

        /// <summary>
        /// ФИО руководителя
        /// </summary>
        HeadName,

        /// <summary>
        /// Должность руководителя
        /// </summary>
        HeadPosition,

        /// <summary>
        /// Дата отчета 
        /// </summary>
        DateReport,
    }

    public static class StateTask2016Helpers
    {
        public static string DatailsNameMapping(Details2016 datail)
        {
            switch (datail)
            {
                case Details2016.StateTask:
                    {
                        return "Государственное (муниципальное) задание";
                    }

                case Details2016.ConsumerCategory:
                    {
                        return "Категории потребителей";
                    }

                case Details2016.IndicatorsOfService:
                    {
                        return "Показатели оказания услуги";
                    }

                case Details2016.RequisitesNpa:
                    {
                        return "Реквизиты НПА, устанавливающих цены";
                    }

                case Details2016.NpaRegulatesService:
                    {
                        return "НПА, регулирующий порядок оказания услуги";
                    }

                case Details2016.InformConsumers:
                    {
                        return "Порядок информирования потребителей";
                    }

                case Details2016.MonitoringExecution:
                    {
                        return "Порядок контроля за исполнением";
                    }

                case Details2016.RequirementForReportingOnPerformance:
                    {
                        return "Требование к отчетности об исполнении";
                    }

                case Details2016.ReportingRequirements:
                    {
                        return "Требование к отчетности";
                    }

                case Details2016.GroundsForTermination:
                    {
                        return "Основания для приостановления\\прекращения";
                    }

                case Details2016.GroundsForOtherInfo:
                    {
                        return "Иная информация, необходимая для исполнения";
                    }

                case Details2016.PayService:
                    {
                        return "Платность";
                    }

                case Details2016.Reports:
                    {
                        return "Отчеты о выполнении";
                    }

                case Details2016.AveragePrice:
                    {
                        return "Среднегодовой размер платы";
                    }
            }

            return string.Empty;
        }

        public static string IndicatorsOfServiceFields2016Mapping(IndicatorsOfServiceFields2016 field)
        {
            switch (field)
            {
                case IndicatorsOfServiceFields2016.ID:
                    {
                        return "ID";
                    }

                case IndicatorsOfServiceFields2016.ActualValue:
                    {
                        return "Исполнено на текущую дату";
                    }

                case IndicatorsOfServiceFields2016.ReportingYear:
                    {
                        return "Отчетный год<p/>";
                    }

                case IndicatorsOfServiceFields2016.ComingYear:
                    {
                        return "Очередной год<p/>";
                    }

                case IndicatorsOfServiceFields2016.FirstPlanYear:
                    {
                        return "Первый плановый год<p/>";
                    }

                case IndicatorsOfServiceFields2016.Reject:
                    {
                        return "Отклонение, превышающее допустимое значение";
                    }

                case IndicatorsOfServiceFields2016.Protklp:
                    {
                        return "Причина отклонения";
                    }

                case IndicatorsOfServiceFields2016.CurrentYear:
                    {
                        return "Текущий год<p/>";
                    }

                case IndicatorsOfServiceFields2016.SecondPlanYear:
                    {
                        return "Второй плановый год<p/>";
                    }

                case IndicatorsOfServiceFields2016.PnrOkei:
                    {
                        return "Единица измерения";
                    }

                case IndicatorsOfServiceFields2016.PnrType:
                    {
                        return "Тип показателя";
                    }

                case IndicatorsOfServiceFields2016.Info:
                    {
                        return "Дополнительная информация";
                    }

                case IndicatorsOfServiceFields2016.Source:
                    {
                        return "Источник информации";
                    }

                case IndicatorsOfServiceFields2016.SourceInfFact:
                    {
                        return "Источник информации о фактическом значении показателя";
                    }

                case IndicatorsOfServiceFields2016.RefIndicatorsName:
                    {
                        return "Показатели объема и качества услуг/работ";
                    }

                case IndicatorsOfServiceFields2016.Deviation:
                    {
                        return "Допустимое отклонение (%)";
                    }
            }

            return string.Empty;
        }

        public static string ReportingRequirementsFields2016Mapping(ReportingRequirementsFields2016 field)
        {
            switch (field)
            {
                case ReportingRequirementsFields2016.ID:
                    {
                        return "ID";
                    }

                case ReportingRequirementsFields2016.PeriodicityTerm:
                    {
                        return "Периодичность предоставления отчетов о выполнении";
                    }

                case ReportingRequirementsFields2016.DeliveryTerm:
                    {
                        return "Сроки предоставления отчетов";
                    }

                case ReportingRequirementsFields2016.OtherRequest:
                    {
                        return "Иное требование к отчетности";
                    }

                case ReportingRequirementsFields2016.OtherIndicators:
                    {
                        return "Иные показатели";
                    }

                case ReportingRequirementsFields2016.ReportForm:
                    {
                        return "Форма отчета";
                    }
            }

            return string.Empty;
        }

        public static string ReportsFields2016Mapping(ReportsFields2016 field)
        {
            switch (field)
            {
                case ReportsFields2016.ReportGuid:
                    return "Уникальный идентификатор отчета";
                case ReportsFields2016.NameReport:
                    return "Наименование  отчета";
                case ReportsFields2016.HeadName:
                    return "ФИО руководителя";
                case ReportsFields2016.HeadPosition:
                    return "Должность руководителя";
                case ReportsFields2016.DateReport:
                    return "Дата отчета";
                default:
                    return string.Empty;
            }
        }
    }
}
