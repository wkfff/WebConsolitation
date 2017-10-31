using System.ComponentModel;

namespace Krista.FM.Client.Reports
{
    #region Списки для параметров

    public enum ReportPeriodEnum
    {
        [Description("Год")]
        i1,
        [Description("Месяц")]
        i2
    }

    public enum PrecisionNumberEnum
    {
        [Description("целое значение")]
        ctN0,
        [Description("с десятыми после запятой")]
        ctN1,
        [Description("с сотыми после запятой")]
        ctN2
    }

    public enum SumDividerEnum
    {
        [Description("руб.")]
        i1,
        [Description("тыс. руб.")]
        i2,
        [Description("млн. руб.")]
        i3,
        [Description("млрд. руб.")]
        i4
    }

    public enum ConstructorType
    {
        [Browsable(false)]
        ctNone,
        [Description("Кредиты организаций")]
        ctCreditOrg,
        [Description("Кредиты бюджетов")]
        ctCreditBud,
        [Browsable(false)]
        [Description("Гарантии")]
        ctGarant,
        [Browsable(false)]
        [Description("Кредиты предоставленные")]
        ctCreditIssued
    }

    public enum ContractTypeEnum
    {
        [Description("Действующие договора")]
        i1,
        [Description("Действующие и закрытые договора")]
        i2,
        [Description("Закрытые договора")]
        i3
    }

    public enum VariantTypeEnum
    {
        [Description("Действующие договора")]
        i1,
        [Description("Действующие и Архив")]
        i2
    }

    public enum DateParamEnum
    {
        [Description("Все договора")]
        i1,
        [Description("Начиная с даты")]
        i2,
        [Description("До даты")]
        i3,
        [Description("Начиная с даты до даты")]
        i4
    }

    enum FactDebtMainFormulaEnum
    {
        [Description("Не выводить")]
        i0,
        [Description("ОД факт привлечения – ОД факт погашения")]
        i1,
        [Description("ОД план привлечения – ОД факт погашения")]
        i2,
        [Description("ОД план погашения – ОД факт погашения")]
        i3
    }

    enum FactDebtServiceFormulaEnum
    {
        [Description("Не выводить")]
        i0,
        [Description("% план погашения – % факт погашения")]
        i1
    }

    enum FactDebtOverdatedFormulaEnum
    {
        [Description("Не выводить")]
        i0,
        [Description("ОД Факт привл. –ОД Факт пог. +% План пог. –% Факт пог.")]
        i1,
        [Description("ОД План привл. –ОД Факт пог. +% План пог. –% Факт пог.")]
        i2,
        [Description("ОД План пог. – ОД Факт пог. + % План пог. –% Факт пог.")]
        i3
    }

    enum DateParamTypeEnum
    {
        [Description("Выбранное значение")]
        i0,
        [Description("Текущая дата")]
        i1,
        [Description("Начало месяца")]
        i2,
        [Description("Конец месяца")]
        i3,
        [Description("Начало года")]
        i4,
        [Description("Конец года")]
        i5,
        [Description("Дата отчета 1")]
        i6,
        [Description("Даты отчета 2")]
        i7,
        [Description("Начало действия договора")]
        i8,
        [Description("Конец действия договора")]
        i9
    }

    enum DetailWriteTypeEnum
    {
        [Description("Не выводить")]
        i0,
        [Description("Все")]
        i1,
        [Description("Начиная с даты")]
        i2,
        [Description("До даты")]
        i3,
        [Description("Период")]
        i4,
        [Description("Детализация строк")]
        i5
    }

    enum MonthEnum
    {
        [Description("Январь")]
        i1,
        [Description("Февраль")]
        i2,
        [Description("Март")]
        i3,
        [Description("Апрель")]
        i4,
        [Description("Май")]
        i5,
        [Description("Июнь")]
        i6,
        [Description("Июль")]
        i7,
        [Description("Август")]
        i8,
        [Description("Сентябрь")]
        i9,
        [Description("Октябрь")]
        i10,
        [Description("Ноябрь")]
        i11,
        [Description("Декабрь")]
        i12
    }

    enum QuarterEnum
    {
        [Description("1 квартал")]
        i1,
        [Description("2 квартал")]
        i2,
        [Description("3 квартал")]
        i3,
        [Description("4 квартал")]
        i4,
    }

    enum HalfYearEnum
    {
        [Description("I полугодие")]
        i1,
        [Description("II полугодие")]
        i2
    }

    public enum RegionListTypeEnum
    {
        [Description("По ГО, МР и поселениям")]
        i1,
        [Description("По ГО и МР")]
        i2
    }

    public enum PlanSumFieldEnum
    {
        [Description("Оценка")]
        i1,
        [Description("Прогноз")]
        i2,
        [Description("Налоговый потенциал")]
        i3
    }

    public enum MOFOContractTypeEnum
    {
        [Description("По договорам, заключенным муниципальными образованиями")]
        i1,
        [Description("По договорам, заключенным минмособлимущество")]
        i2,
        [Description("По всем договорам")]
        i3
    }

    public enum VariantYearEnum
    {
        [Description("Очередной финансовый год")]
        i1,
        [Description("1 год планового периода")]
        i2,
        [Description("2 год планового периода")]
        i3
    }

    public enum DebtLoadingListTypeEnum
    {
        [Description("Расходы на погашение долга")]
        i1,
        [Description("Расходы на обслуживание")]
        i2,
        [Description("Расходы на погашение и обслуживание долга")]
        i3
    }

    #endregion
}
