namespace Krista.FM.RIA.Extensions.E86N.Services.StateTaskService
{
    /// <summary>
    /// Детализации документа
    /// </summary>
    public enum Details
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
        /// Значения предельных цен (тарифов)
        /// </summary>
        LimitValuesOfPrices,

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
    }
    
    /// <summary>
    /// Поля для "Показатели оказания услуги"
    /// </summary>
    public enum IndicatorsOfServiceFields
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
    }

    /// <summary>
    /// Поля для "Требование к отчетности"
    /// </summary>
    public enum ReportingRequirementsFields
    {
        /// <summary>
        /// ID
        /// </summary>
        ID,

        /// <summary>
        /// Сроки предоставления отчетов
        /// </summary>
        DeliveryTerm,

        /// <summary>
        /// Иное требование к отчетности
        /// </summary>
        OtherRequest,

        /// <summary>
        /// Иная информация, необходимая для исполнения
        /// </summary>
        OtherInfo,

        /// <summary>
        /// Форма отчета
        /// </summary>
        ReportForm
    }

    public static class StateTaskHelpers
    {
        public static string DatailsNameMapping(Details datail)
        {
            switch (datail)
            {
                case Details.StateTask:
                    {
                        return "Государственное (муниципальное) задание";
                    }

                case Details.ConsumerCategory:
                    {
                        return "Категории потребителей";
                    }

                case Details.IndicatorsOfService:
                    {
                        return "Показатели оказания услуги";
                    }

                case Details.RequisitesNpa:
                    {
                        return "Реквизиты НПА, устанавливающих цены";
                    }

                case Details.NpaRegulatesService:
                    {
                        return "НПА, регулирующий порядок оказания услуги";
                    }

                case Details.LimitValuesOfPrices:
                    {
                        return "Значения предельных цен (тарифов)";
                    }

                case Details.InformConsumers:
                    {
                        return "Порядок информирования потребителей";
                    }

                case Details.MonitoringExecution:
                    {
                        return "Порядок контроля за исполнением";
                    }

                case Details.RequirementForReportingOnPerformance:
                    {
                        return "Требование к отчетности об исполнении";
                    }

                case Details.ReportingRequirements:
                    {
                        return "Требование к отчетности";
                    }

                case Details.GroundsForTermination:
                    {
                        return "Основания для приостановления\\прекращения";
                    }
            }

            return string.Empty;
        }

        public static string IndicatorsOfServiceFieldsMapping(IndicatorsOfServiceFields field)
        {
            switch (field)
            {
                case IndicatorsOfServiceFields.ID:
                    {
                        return "ID";
                    }

                case IndicatorsOfServiceFields.ActualValue:
                    {
                        return "Фактическое значение";
                    }

                case IndicatorsOfServiceFields.ReportingYear:
                    {
                        return "Отчетный год<p/>";
                    }

                case IndicatorsOfServiceFields.ComingYear:
                    {
                        return "Очередной год<p/>";
                    }

                case IndicatorsOfServiceFields.FirstPlanYear:
                    {
                        return "Первый плановый год<p/>";
                    }

                case IndicatorsOfServiceFields.Protklp:
                    {
                        return "Причина отклонения";
                    }

                case IndicatorsOfServiceFields.CurrentYear:
                    {
                        return "Текущий год<p/>";
                    }

                case IndicatorsOfServiceFields.SecondPlanYear:
                    {
                        return "Второй плановый год<p/>";
                    }

                case IndicatorsOfServiceFields.PnrOkei:
                    {
                        return "Единица измерения";
                    }

                case IndicatorsOfServiceFields.PnrType:
                    {
                        return "Тип показателя";
                    }

                case IndicatorsOfServiceFields.Info:
                    {
                        return "Дополнительная информация";
                    }

                case IndicatorsOfServiceFields.Source:
                    {
                        return "Источник информации";
                    }

                case IndicatorsOfServiceFields.SourceInfFact:
                    {
                        return "Источник информации о фактическом значении показателя";
                    }

                case IndicatorsOfServiceFields.RefIndicatorsName:
                    {
                        return "Показатели объема и качества услуг/работ";
                    }
            }

            return string.Empty;
        }

        public static string ReportingRequirementsFieldsMapping(ReportingRequirementsFields field)
        {
            switch (field)
            {
                case ReportingRequirementsFields.ID:
                    {
                        return "ID";
                    }

                case ReportingRequirementsFields.DeliveryTerm:
                    {
                        return "Сроки предоставления отчетов";
                    }

                case ReportingRequirementsFields.OtherRequest:
                    {
                        return "Иное требование к отчетности";
                    }

                case ReportingRequirementsFields.OtherInfo:
                    {
                        return "Иная информация, необходимая для исполнения";
                    }

                case ReportingRequirementsFields.ReportForm:
                    {
                        return "Форма отчета";
                    }
            }

            return string.Empty;
        }
    }
}
