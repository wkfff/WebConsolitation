using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using Krista.FM.ServerLibrary;

namespace Krista.FM.ServerLibrary.FinSourcePlanning
{
    /// <summary>
    /// типы кредитов. значение совпадает со значением справочника типов кредитов
    /// </summary>
    public enum CreditsTypes
    {
        [Description("Кредиты полученные от других бюджетов")]
        BudgetIncoming = 1,
        [Description("Кредиты предоставленные другим бюджетам")]
        BudgetOutcoming = 3,
        [Description("Кредиты полученные от организаций")]
        OrganizationIncoming = 0,
        [Description("Кредиты предоствленные огранизациям")]
        OrganizationOutcoming = 4,
        [Description("Неизвестный тип")]
        Unknown = -1
    }

    /// <summary>
    /// Типы подсистем планирования источников финансирования.
    /// </summary>
    public enum FinSourcePlanningServiceTypes
    {
        [Description("Гарантии полученные")]
        GuarantIncome,
        
        [Description("Гарантии предоставленные")]
        GuarantIssued,

        [Description("Кредиты полученные")]
        CreditIncome,

        [Description("Кредиты предоставленные")]
        CreditIssued,

        [Description("Ценные бумаги")]
        Capital,

		[Description("Оценка проекта бюджета")]
        BKKUIndicators,

        [Description("Доступная долговая емкость")]
        DDEIndicators
    }

    /// <summary>
    /// 
    /// </summary>
    public enum BorrowingVolumeBudgetType
    {
        // кассовая роспись
        BudgetList = 0,
        // Поквартальный кассовый план
        QuarterCashPlan = 1,
        // Помесячный кассовый план
        MonthCashPlan = 2,
    }

    /// <summary>
    /// опции переноса данных в проект бюджета
    /// </summary>
    [Flags]
    public enum BudgetTransfertOption
    {
        /// <summary>
        /// перенос в доходную часть
        /// </summary>
        IncomesPart = 1,
        /// <summary>
        /// перенос в расходную часть
        /// </summary>
        ChargesPart = 2,
        /// <summary>
        /// перенос в часть ИФ
        /// </summary>
        IfPart = 4,
        /// <summary>
        /// перенос всех частей
        /// </summary>
        All = IncomesPart | ChargesPart | IfPart
    }

    /// <summary>
    /// Фасад системы планирования источников финансирования.
    /// </summary>
    public interface IFinSourcePlanningFace : IServerSideObject
    {
        /// <summary>
        /// Сервис для рассчета значений индикаторов БК и КУ.
        /// </summary>
        IIndicatorsService BKKUIndicators { get; }

        /// <summary>
        /// Сервис для рассчета значений ДДЕ.
        /// </summary>
        IIndicatorsService DDEIndicators { get; }

        /// <summary>
        /// ИФ.Гарантии полученные.
        /// </summary>
        IGuarantIncomeService GuarantIncomeService { get; }
        
        /// <summary>
        /// ИФ.Гарантии предоставленные.
        /// </summary>
        IGuarantIssuedService GuarantIssuedService { get; }
        
        /// <summary>
        /// ИФ.Кредиты полученные.
        /// </summary>
        IСreditIncomeService СreditIncomeService { get; }

        /// <summary>
        /// ИФ.Кредиты предоставленные.
        /// </summary>
        IСreditIssuedService СreditIssuedService { get; }

        /// <summary>
        /// ИФ.Ценные бумаги.
        /// </summary>
        ICapitalService CapitalService { get; }

        /// <summary>
        /// Получение ссылки на классификатор в кредитах (по константам)
        /// </summary>
        /// <param name="detailObjectKey">уникальный ключ детали</param>
        /// <param name="classifierKey">уникальный ключ мастер-таблицы</param>
        /// <param name="sourceID">источник, по которому будет произведен поиск записи в классификаторе</param>
        /// <param name="refOKV">Валюта</param>
        /// <param name="creditType">Тип кредита</param>
        /// <param name="refTerrType">Тип территории</param>
        /// <returns></returns>
        int GetCreditClassifierRef(string detailObjectKey,
            string classifierKey, int sourceID, int refOKV, CreditsTypes creditType, int refTerrType, string programCode);

        /// <summary>
        /// получение ссылки на классификатор в гарантиях (по константам)
        /// </summary>
        /// <param name="detailObjectKey">уникальный ключ детали</param>
        /// <param name="classifierKey">уникальный ключ мастер-таблицы</param>
        /// <param name="isRegress">Регресс в гарантии</param>
        /// <param name="sourceID">источник, по которому будет произведен поиск записи в классификаторе</param>
        /// <returns></returns>
        int GetGuaranteeClassifierRef(string detailObjectKey, string classifierKey, bool isRegress, int sourceID);

        /// <summary>
        /// получение ссылки на классификаторы в ценных бумагах (по константам)
        /// </summary>
        /// <param name="detailObjectKey">уникальный ключ детали</param>
        /// <param name="classifierKey">уникальный ключ мастер-таблицы</param>
        /// <param name="sourceID">источник, по которому будет произведен поиск записи в классификаторе</param>
        /// <param name="refOKV"></param>
        /// <returns></returns>
        int GetCapitalClassifierRef(string detailObjectKey, string classifierKey, int sourceID, int refOKV);

        /// <summary>
        /// Базовый год для расчета.
        /// </summary>
        int BaseYear { get; }

        /// <summary>
        /// Получение видимости модуля в источниках финансирования 
        /// </summary>
        /// <param name="moduleKey"></param>
        /// <returns></returns>
        Boolean CheckUIModuleVisible(string moduleKey);

        /// <summary>
        /// Установка ссылок на классификаторы в деталях договоров
        /// </summary>
        void SetAllReferences(int sourceId, IDatabase db);

        /// <summary>
        /// Установка ссылок на классификаторы в деталях договоров
        /// </summary>
        /// <param name="sourceId"></param>
        void SetAllReferences(int sourceId);

        /// <summary>
        /// обновляем данные по константам
        /// </summary>
        void ResreshConstsData();

        /// <summary>
        /// установка ссылок на справочник валют в курсах валют
        /// </summary>
        void SetCurrencyRatesReferences();

        /// <summary>
        /// Перенос данных в долговую книгу
        /// </summary>
        /// <param name="errors"></param>
        void TransfertDebtBookData(object pumpId, ref string errors);
    }

    /// <summary>
    /// Базовый интерфейс для блоков планирования источников финансирования.
    /// </summary>
	public interface IFinSourceBaseService : IServerSideObject
    {
        /// <summary>
        /// Таблица с данными.
        /// </summary>
        IFactTable Data { get; }

        /// <summary>
        /// Детальные данные блока.
        /// </summary>
		// TODO: Возвращаемую коллекцию обернуть в объект унаследованный от ISMOSerializable
        Dictionary<string, IEntityAssociation> Details { get; }
    }

    /// <summary>
    /// ИФ.Кредиты полученные.
    /// </summary>
    public interface IGuarantIncomeService : IFinSourceBaseService
    {
    }

    /// <summary>
    /// ИФ.Кредиты предоставленные.
    /// </summary>
    public interface IGuarantIssuedService : IFinSourceBaseService
    {
        void FillDebtRemainder(int refVariant);
    }

    /// <summary>
    /// ИФ.Кредиты полученные.
    /// </summary>
    public interface IСreditIncomeService : IFinSourceBaseService
    {
        void FillDebtRemainder(int refVariant);
    }

    /// <summary>
    /// ИФ.Кредиты предоставленные.
    /// </summary>
    public interface IСreditIssuedService : IFinSourceBaseService
    {
    }

    /// <summary>
    /// ИФ.Кредиты предоставленные.
    /// </summary>
    public interface ICapitalService : IFinSourceBaseService
    {
        void FillDebtRemainder(int refVariant);
    }

    /// <summary>
    /// Сервис для рассчета значений индикаторов БК и КУ.
    /// </summary>
    public interface IIndicatorsService
    {
        /// <summary>
        /// Производит вычисление индикаторов.
        /// </summary>
        /// <param name="variantIF">Вариант ИФ.</param>
        /// <param name="variantIncome">Вариант доходов.</param>
        /// /// <param name="variantIncomeYear">Год варианта доходов.</param>
        /// <param name="variantOutcome">Вариант расходов.</param>
        /// <param name="variantBorrowID">ID варианта заимствований.</param>
        /// <returns>Таблица с результатами.</returns>
        DataTable CalculateAndAssessIndicators(string variantIF, string variantIncome, string variantIncomeYear, string variantOutcome, int variantBorrowID);

        /// <summary>
        /// Количество лет для расчета.
        /// </summary>
        int YearsCount
        { get; }
    }

    /// <summary>
    /// Типы оценки значения индикатора.
    /// </summary>
    public enum AssessionType
    {
        Logical,
        Interval,
        NonAssessed
    }
}
