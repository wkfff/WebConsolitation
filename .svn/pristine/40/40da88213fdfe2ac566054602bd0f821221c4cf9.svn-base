namespace Krista.FM.RIA.Extensions.E86N.Services.AnnualBalance
{
    /// <summary>
    /// Поля для формы с общими атрибутами
    /// </summary>
    public enum HeadAttributeFields
    {
        /// <summary>
        /// ID поле
        /// </summary>
        HeadAttributeID,

        /// <summary>
        /// Дата предоставления данных
        /// </summary>
        Datedata,

        /// <summary>
        /// Периодичность поле
        /// </summary>
        RefPeriodic,

        /// <summary>
        /// Периодичность разыменовка
        /// </summary>
        RefPeriodicName,

        /// <summary>
        /// Параметры документа
        /// </summary>
        RefParametr,

        /// <summary>
        /// ОКПО учредителя (только для ф. 0503737)
        /// </summary>
        FounderAuthorityOkpo
    }
    
    /// <summary>
    /// Детализации документов "Баланс государственного (муниципального) учреждения (ф. 0503730) (бюджетные учреждения)" и "Баланс (ф. 0503130)"
    /// </summary>
    public enum F0503130F0503730Details
    {
        /// <summary>
        /// Нефинансовые активы
        /// </summary>
        NonfinancialAssets,

        /// <summary>
        /// Финансовые активы
        /// </summary>
        FinancialAssets,

        /// <summary>
        /// Обязательства детализация
        /// </summary>
        Liabilities,

        /// <summary>
        /// Финансовый результат
        /// </summary>
        FinancialResult,

        /// <summary>
        /// Справка детализация
        /// </summary>
        Information
    }

    /// <summary>
    /// Поля для документа "Баланс государственного (муниципального) учреждения (ф. 0503730) (бюджетные учреждения)"
    /// </summary>
    public enum F0503730Fields
    {
        /// <summary>
        /// ID поле
        /// </summary>
        ID,

        /// <summary>
        /// Параметры документа
        /// </summary>
        RefParametr,

        /// <summary>
        /// Название актива или пассива
        /// </summary>
        Name,

        /// <summary>
        /// Код строки
        /// </summary>
        LineCode,

        /// <summary>
        /// Деятельность с целевыми средствами на начало года
        /// </summary>
        TargetFundsBegin,

        /// <summary>
        /// Деятельность с целевыми средствами на конец года
        /// </summary>
        TargetFundsEnd,

        /// <summary>
        /// Деятельность по оказанию услуг_работ на начало года
        /// </summary>
        ServicesBegin,

        /// <summary>
        /// Деятельность по оказанию услуг_работ на конец года
        /// </summary>
        ServicesEnd,

        /// <summary>
        /// Средства во временном распоряжении на начало года
        /// </summary>
        TemporaryFundsBegin,

        /// <summary>
        /// Средства во временном распоряжении на конец года
        /// </summary>
        TemporaryFundsEnd,
        
        /// <summary>
        /// Деятельность по государственному заданию на начало года
        /// </summary>
        StateTaskFundStartYear,

        /// <summary>
        /// Деятельность по государственному заданию на конец года
        /// </summary>
        StateTaskFundEndYear,

        /// <summary>
        /// Приносящая доход деятельность на начало года
        /// </summary>
        RevenueFundsStartYear,

        /// <summary>
        /// Приносящая доход деятельность на конец года
        /// </summary>
        RevenueFundsEndYear,

        /// <summary>
        /// Итого на начало года
        /// </summary>
        TotalStartYear,

        /// <summary>
        /// Итого на конец года
        /// </summary>
        TotalEndYear,

        /// <summary>
        /// Номер забалансового счета
        /// </summary>
        NumberOffBalance,
    }

    /// <summary>
    /// Детализации документа "Отчет о финансовых результатах деятельности (ф. 0503121)"
    /// </summary>
    public enum F0503121Details
    {
        /// <summary>
        /// Доходы детализация
        /// </summary>
        Incomes,

        /// <summary>
        /// Расходы детализация
        /// </summary>
        Expenses,

        /// <summary>
        /// Чистый операционный результат
        /// </summary>
        OperatingResult,

        /// <summary>
        /// Операции с нефинансовыми активами
        /// </summary>
        OperationNonfinancialAssets,

        /// <summary>
        /// Операции с финансовыми активами и обязательствами
        /// </summary>
        OperationFinancialAssets
    }

    /// <summary>
    /// Поля для документа "Отчет о финансовых результатах деятельности (ф. 0503121)"
    /// </summary>
    public enum F0503121Fields
    {
        /// <summary>
        /// ID поле
        /// </summary>
        ID,

        /// <summary>
        /// Параметры документа
        /// </summary>
        RefParametr,

        /// <summary>
        /// Название актива или пассива
        /// </summary>
        Name,

        /// <summary>
        /// Код строки
        /// </summary>
        LineCode,

        /// <summary>
        /// Бюджетная деятельность
        /// </summary>
        BudgetActivity,

        /// <summary>
        /// Приносящая доход деятельность
        /// </summary>
        IncomeActivity,

        /// <summary>
        /// Средства во временном распоряжении
        /// </summary>
        AvailableMeans,

        /// <summary>
        /// Итого поле
        /// </summary>
        Total,

        /// <summary>
        /// КОСГУ Разименовка
        /// </summary>
        RefKosgyName,

        /// <summary>
        /// Поля КОСГУ для кадой из детализаций.
        /// Из за того что едитор для поля таблицы явлется компонентом и ID для каждого едитора детализации должен быть уникальным. Изврат! Может как то обрабоку ".SetHBLookup(" переделать!?
        /// </summary>
        IncomesRefKosgy,

        /// <summary>
        /// The incomes ref kosgy name.
        /// </summary>
        IncomesRefKosgyName,

        /// <summary>
        /// The expenses ref kosgy.
        /// </summary>
        ExpensesRefKosgy,

        /// <summary>
        /// The expenses ref kosgy name.
        /// </summary>
        ExpensesRefKosgyName,

        /// <summary>
        /// The operating result ref kosgy.
        /// </summary>
        OperatingResultRefKosgy,

        /// <summary>
        /// The operating result ref kosgy name.
        /// </summary>
        OperatingResultRefKosgyName,

        /// <summary>
        /// The operation nonfinancial assets ref kosgy.
        /// </summary>
        OperationNonfinancialAssetsRefKosgy,

        /// <summary>
        /// The operation nonfinancial assets ref kosgy name.
        /// </summary>
        OperationNonfinancialAssetsRefKosgyName,

        /// <summary>
        /// The operation financial assets ref kosgy.
        /// </summary>
        OperationFinancialAssetsRefKosgy,

        /// <summary>
        /// The operation financial assets ref kosgy name.
        /// </summary>
        OperationFinancialAssetsRefKosgyName
    }

    /// <summary>
    /// Детализации документа "Отчет об исполнении бюджета (ф. 0503127)"
    /// </summary>
    public enum F0503127Details
    {
        /// <summary>
        /// Доходы бюджета
        /// </summary>
        BudgetIncomes,

        /// <summary>
        /// Расходы бюджета
        /// </summary>
        BudgetExpenses,

        /// <summary>
        /// Источники финансирования дефицита бюджета
        /// </summary>
        SourcesOfFinancing,
    }

    /// <summary>
    /// Поля для документа "Отчет об исполнении бюджета (ф. 0503127)"
    /// </summary>
    public enum F0503127Fields
    {
        /// <summary>
        /// ID поле
        /// </summary>
        ID,

        /// <summary>
        /// Параметры документа
        /// </summary>
        RefParametr,

        /// <summary>
        /// Название актива или пассива
        /// </summary>
        Name,

        /// <summary>
        /// Код строки
        /// </summary>
        LineCode,

        /// <summary>
        /// Утвержденные бюджетные назначения
        /// </summary>
        ApproveBudgAssign,

        /// <summary>
        /// Исполнено через финансовые органы
        /// </summary>
        ExecFinAuthorities,

        /// <summary>
        /// Исполнено через банковские счета
        /// </summary>
        ExecBankAccounts,

        /// <summary>
        /// Исполнено через некассовые операции
        /// </summary>
        ExecNonCashOperation,

        /// <summary>
        /// Исполнено итого
        /// </summary>
        ExecTotal,

        /// <summary>
        /// Неисполненные назначения по БА
        /// </summary>
        UnexecAssignments,

        /// <summary>
        /// Неисполненные назначения по ЛБО
        /// </summary>
        UnexecBudgObligatLimit,

        /// <summary>
        /// Лимиты бюджетных обязательств
        /// </summary>
        BudgObligatLimits,
        
        /// <summary>
        /// Код дохода по бюджетной классификации
        /// </summary>
        BudgClassifCode
    }

    /// <summary>
    /// Поля для документа "Баланс (ф. 0503130)"
    /// </summary>
    public enum F0503130Fields
    {
        /// <summary>
        /// ID поле
        /// </summary>
        ID,

        /// <summary>
        /// Параметры документа
        /// </summary>
        RefParametr,

        /// <summary>
        /// Название актива или пассива
        /// </summary>
        Name,

        /// <summary>
        /// Код строки
        /// </summary>
        LineCode,

        /// <summary>
        /// Бюджетная деятельность на начало года
        /// </summary>
        BudgetActivityBegin,

        /// <summary>
        /// Бюджетная деятельность на конец года
        /// </summary>
        BudgetActivityEnd,

        /// <summary>
        /// Приносящая доход деятельность на начало года
        /// </summary>
        IncomeActivityBegin,

        /// <summary>
        /// Приносящая доход деятельность на конец года
        /// </summary>
        IncomeActivityEnd,

        /// <summary>
        /// Средства во временном распоряжении на начало года
        /// </summary>
        AvailableMeansBegin,

        /// <summary>
        /// Средства во временном распоряжении на конец года
        /// </summary>
        AvailableMeansEnd,

        /// <summary>
        /// Итого на начало года
        /// </summary>
        TotalBegin,

        /// <summary>
        /// Итого на конец года
        /// </summary>
        TotalEnd,

        /// <summary>
        /// Номер забалансового счета
        /// </summary>
        NumberOffBalance,
    }

    /// <summary>
    /// Детализации документа "Отчет об исполнении смет доходов и расходов по приносящей доход деятельности(ф. 0503137)"
    /// </summary>
    public enum F0503137Details
    {
        /// <summary>
        /// Доходы детализация
        /// </summary>
        Incomes,

        /// <summary>
        /// Расходы детализация
        /// </summary>
        Expenses,

        /// <summary>
        /// Источники финансирования дефицита средств учреждения 
        /// </summary>
        SourcesOfFinancing
    }

    /// <summary>
    /// Детализации документа "Отчет об исполнении учреждением плана ФХД (ф. 0503737)"
    /// </summary>
    public enum F0503737Details
    {
        /// <summary>
        /// Доходы детализация
        /// </summary>
        Incomes,

        /// <summary>
        /// Расходы детализация
        /// </summary>
        Expenses,

        /// <summary>
        /// Источники финансирования дефицита средств учреждения 
        /// </summary>
        SourcesOfFinancing,

        /// <summary>
        /// Сведения о возвратах расходов и выплат обеспечений прошлых лет
        /// </summary>
        ReturnExpense
    }

    /// <summary>
    /// Поля для документа "Отчет об исполнении смет доходов и расходов по приносящей доход деятельности(ф. 0503137)"
    /// </summary>
    public enum F0503137Fields
    {
        /// <summary>
        /// ID поле
        /// </summary>
        ID,

        /// <summary>
        /// Параметры документа
        /// </summary>
        RefParametr,

        /// <summary>
        /// Название актива или пассива
        /// </summary>
        Name,

        /// <summary>
        /// Код строки
        /// </summary>
        LineCode,

        /// <summary>
        /// Код дохода по бюджетной классификации
        /// </summary>
        BudgClassifCode,
        
        /// <summary>
        /// Утвержденные сметные назначения
        /// </summary>
        ApproveEstimateAssign,

        /// <summary>
        /// Исполнено через финансовые органы
        /// </summary>
        ExecFinancAuthorities,

        /// <summary>
        /// Исполнено через банковские счета
        /// </summary>
        ExecBankAccounts,

        /// <summary>
        /// Исполнено через некассовые операции
        /// </summary>
        ExecNonCashOperation,

        /// <summary>
        /// Исполнено итого
        /// </summary>
        ExecTotal,

        /// <summary>
        /// Неисполненные назначения
        /// </summary>
        UnexecAssignments
    }

    /// <summary>
    /// Детализации документа "Отчет о финансовых результатах деятельности (ф. 0503721)"
    /// </summary>
    public enum F0503721Details
    {
        /// <summary>
        /// Доходы детализация
        /// </summary>
        Incomes,

        /// <summary>
        /// Расходы детализация
        /// </summary>
        Expenses,

        /// <summary>
        /// Нефинансовые активы 
        /// </summary>
        NonFinancialAssets,

        /// <summary>
        /// Финансовые активы и обязательства
        /// </summary>
        FinancialAssetsLiabilities
    }
    
    /// <summary>
    /// Поля для документа "Отчет об исполнении учреждением плана ФХД (ф. 0503737)"
    /// </summary>
    public enum F0503737Fields
    {
        /// <summary>
        /// ID поле
        /// </summary>
        ID,

        /// <summary>
        /// Параметры документа
        /// </summary>
        RefParametr,

        /// <summary>
        /// Название актива или пассива
        /// </summary>
        Name,

        /// <summary>
        /// Код строки
        /// </summary>
        LineCode,

        /// <summary>
        /// Код аналитики
        /// </summary>
        AnalyticCode,

        /// <summary>
        /// Утверждено плановых назначений
        /// </summary>
        ApprovePlanAssign,

        /// <summary>
        /// Исполнено через лицевые счета
        /// </summary>
        ExecPersonAuthorities,

        /// <summary>
        /// Исполнено через банковские счета
        /// </summary>
        ExecBankAccounts,

        /// <summary>
        /// Исполнено через некассовые операции
        /// </summary>
        ExecNonCashOperation,

        /// <summary>
        /// Исполнено через кассу учреждения
        /// </summary>
        ExecCashAgency,

        /// <summary>
        /// Исполнено итого
        /// </summary>
        ExecTotal,

        /// <summary>
        /// Не исполнено плановых назначений
        /// </summary>
        UnexecPlanAssign,

        /// <summary>
        /// Вид финансового обеспечения
        /// </summary>
        RefTypeFinSupport,

        /// <summary>
        /// Вид финансового обеспечения разыменовка
        /// </summary>
        RefTypeFinSupportName
    }

    /// <summary>
    /// Поля для показателей
    /// </summary>
    public enum IndicatorsFields
    {
        /// <summary>
        /// ID поле
        /// </summary>
        ID,

        /// <summary>
        /// Код поле
        /// </summary>
        Code,

        /// <summary>
        /// Наименование показателя
        /// </summary>
        Name,

        /// <summary>
        /// Код строки
        /// </summary>
        LineCode
    }

    /// <summary>
    /// Поля для настроек интерфейсов
    /// </summary>
    public enum SettingsFields
    {
        /// <summary>
        /// ID поле
        /// </summary>
        ID,

        /// <summary>
        /// Детализации код
        /// </summary>
        Section,

        /// <summary>
        /// Детализации разыменовка
        /// </summary>
        SectionName,

        /// <summary>
        /// Код строки
        /// </summary>
        LineCode,

        /// <summary>
        /// Индикатор идентификатор
        /// </summary>
        RefIndicators,

        /// <summary>
        /// Индикатор разыменовка
        /// </summary>
        RefIndicatorsName,

        /// <summary>
        /// Документ идентификатор
        /// </summary>
        RefPartDoc,

        /// <summary>
        /// Документ разыменовка
        /// </summary>
        RefPartDocName,

        /// <summary>
        /// Дополнительное поле
        /// </summary>
        Additional,

        /// <summary>
        /// Год начала
        /// </summary>
        StartYear,

        /// <summary>
        /// Год окончания
        /// </summary>
        EndYear,
    }

    internal static class AnnualBalanceHelpers
    {
        public static string HeadAttributeFieldsNameMapping(HeadAttributeFields field)
        {
            switch (field)
            {
                case HeadAttributeFields.HeadAttributeID:
                    {
                        return "ID";
                    }

                case HeadAttributeFields.Datedata:
                    {
                        return "Дата предоставления данных";
                    }

                case HeadAttributeFields.RefPeriodicName:
                    {
                        return "Периодичность";
                    }

                case HeadAttributeFields.FounderAuthorityOkpo:
                    {
                        return "ОКПО учредителя";
                    }
            }

            return string.Empty;
        }

        public static string F0503130F0503730DetailsNameMapping(F0503130F0503730Details detail)
        {
            switch (detail)
            {
                case F0503130F0503730Details.NonfinancialAssets:
                    {
                        return "Нефинансовые активы";
                    }

                case F0503130F0503730Details.FinancialAssets:
                    {
                        return "Финансовые активы";
                    }

                case F0503130F0503730Details.Liabilities:
                    {
                        return "Обязательства";
                    }

                case F0503130F0503730Details.FinancialResult:
                    {
                        return "Финансовый результат";
                    }

                case F0503130F0503730Details.Information:
                    {
                        return "Справка";
                    }
            }

            return string.Empty;
        }

        public static string F0503730NameMapping(F0503730Fields field, F0503130F0503730Details detail)
        {
            switch (field)
            {
                case F0503730Fields.ID:
                    {
                        return "ID";
                    }

                case F0503730Fields.Name:
                    {
                        return detail == F0503130F0503730Details.Information ? "Наименование забалансового счета, показателя" : "Название актива или пассива";
                    }

                case F0503730Fields.LineCode:
                    {
                        return "Код строки";
                    }

                case F0503730Fields.TargetFundsBegin:
                    {
                        return "Деятельность с целевыми средствами на начало года";
                    }

                case F0503730Fields.TargetFundsEnd:
                    {
                        return "Деятельность с целевыми средствами на конец года";
                    }

                case F0503730Fields.ServicesBegin:
                    {
                        return "Деятельность по оказанию услуг (работ) на начало года";
                    }

                case F0503730Fields.ServicesEnd:
                    {
                        return "Деятельность по оказанию услуг (работ) на конец года";
                    }

                case F0503730Fields.TemporaryFundsBegin:
                    {
                        return "Средства во временном распоряжении на начало года";
                    }

                case F0503730Fields.TemporaryFundsEnd:
                    {
                        return "Средства во временном распоряжении на конец года";
                    }

                case F0503730Fields.StateTaskFundStartYear:
                    {
                        return "Деятельность по государственному заданию на начало года";
                    }

                case F0503730Fields.StateTaskFundEndYear:
                    {
                        return "Деятельность по государственному заданию на конец года";
                    }

                case F0503730Fields.RevenueFundsStartYear:
                    {
                        return "Приносящая доход деятельность на начало года";
                    }

                case F0503730Fields.RevenueFundsEndYear:
                    {
                        return "Приносящая доход деятельность на конец года";
                    }

                case F0503730Fields.TotalStartYear:
                    {
                        return "Итого на начало года";
                    }

                case F0503730Fields.TotalEndYear:
                    {
                        return "Итого на конец года";
                    }

                case F0503730Fields.NumberOffBalance:
                    {
                        return "Номер забалансового счета";
                    }
            }

            return string.Empty;
        }

        public static string F0503121DetailsNameMapping(F0503121Details detail)
        {
            switch (detail)
            {
                case F0503121Details.Incomes:
                    {
                        return "Доходы";
                    }

                case F0503121Details.Expenses:
                    {
                        return "Расходы";
                    }

                case F0503121Details.OperatingResult:
                    {
                        return "Чистый операционный результат";
                    }

                case F0503121Details.OperationNonfinancialAssets:
                    {
                        return "Операции с нефинансовыми активами";
                    }

                case F0503121Details.OperationFinancialAssets:
                    {
                        return "Операции с финансовыми активами и обязательствами";
                    }
            }

            return string.Empty;
        }

        public static string F0503121NameMapping(F0503121Fields field, F0503121Details detail)
        {
            switch (field)
            {
                case F0503121Fields.ID:
                    {
                        return "ID";
                    }

                case F0503121Fields.Name:
                    {
                        return "Наименование показателя";
                    }

                case F0503121Fields.LineCode:
                    {
                        return "Код строки";
                    }

                case F0503121Fields.RefKosgyName:
                    {
                        return "Код по КОСГУ";
                    }

                case F0503121Fields.BudgetActivity:
                    {
                        return "Бюджетная деятельность";
                    }

                case F0503121Fields.IncomeActivity:
                    {
                        return "Приносящая доход деятельность";
                    }

                case F0503121Fields.AvailableMeans:
                    {
                        return "Средства во временном распоряжении";
                    }

                case F0503121Fields.Total:
                    {
                        return "Итого";
                    }
            }

            return string.Empty;
        }

        public static string F0503127DetailsNameMapping(F0503127Details detail)
        {
            switch (detail)
            {
                case F0503127Details.BudgetIncomes:
                    {
                        return "Доходы бюджета";
                    }

                case F0503127Details.BudgetExpenses:
                    {
                        return "Расходы бюджета";
                    }

                case F0503127Details.SourcesOfFinancing:
                    {
                        return "Источники финансирования дефицита бюджета";
                    }
            }

            return string.Empty;
        }

        public static string F0503127NameMapping(F0503127Fields field, F0503127Details detail)
        {
            switch (field)
            {
                case F0503127Fields.ID:
                    {
                        return "ID";
                    }

                case F0503127Fields.Name:
                    {
                        return "Наименование показателя";
                    }

                case F0503127Fields.LineCode:
                    {
                        return "Код строки";
                    }

                case F0503127Fields.BudgClassifCode:
                    {
                        switch (detail)
                        {
                            case F0503127Details.BudgetIncomes:
                                {
                                    return "Код дохода по бюджетной классификации";
                                }

                            case F0503127Details.BudgetExpenses:
                                {
                                    return "Код расхода по бюджетной классификации";
                                }
                        }

                        return "Код источника финансирования по бюджетной классификации";
                    }

                case F0503127Fields.ApproveBudgAssign:
                    {
                        return "Утвержденные бюджетные назначения";
                    }

                case F0503127Fields.BudgObligatLimits:
                    {
                        return "Лимиты бюджетных обязательств";
                    }

                case F0503127Fields.ExecFinAuthorities:
                    {
                        return "Исполнено через финансовые органы";
                    }

                case F0503127Fields.ExecBankAccounts:
                    {
                        return "Исполнено через банковские счета";
                    }

                case F0503127Fields.ExecNonCashOperation:
                    {
                        return "Исполнено через некассовые операции";
                    }

                case F0503127Fields.ExecTotal:
                    {
                        return "Исполнено итого";
                    }

                case F0503127Fields.UnexecAssignments:
                    {
                        return "Неисполненные назначения (по ассигнованиям)";
                    }

                case F0503127Fields.UnexecBudgObligatLimit:
                    {
                        return "Неисполненные назначения по лимитам бюджетных обязательств";
                    }
            }

            return string.Empty;
        }

        public static string F0503130NameMapping(F0503130Fields field, F0503130F0503730Details detail)
        {
            switch (field)
            {
                case F0503130Fields.ID:
                    {
                        return "ID";
                    }

                case F0503130Fields.Name:
                    {
                        return detail == F0503130F0503730Details.Information ? "Наименование забалансового счета, показателя" : "Название актива или пассива";
                    }

                case F0503130Fields.LineCode:
                    {
                        return "Код строки";
                    }

                case F0503130Fields.BudgetActivityBegin:
                    {
                        return "Бюджетная деятельность на начало года";
                    }

                case F0503130Fields.BudgetActivityEnd:
                    {
                        return "Бюджетная деятельность на конец года";
                    }

                case F0503130Fields.IncomeActivityBegin:
                    {
                        return "Приносящая доход деятельность на начало года";
                    }

                case F0503130Fields.IncomeActivityEnd:
                    {
                        return "Приносящая доход деятельность на конец года";
                    }

                case F0503130Fields.AvailableMeansBegin:
                    {
                        return "Средства во временном распоряжении на начало года";
                    }

                case F0503130Fields.AvailableMeansEnd:
                    {
                        return "Средства во временном распоряжении на конец года";
                    }

                case F0503130Fields.TotalBegin:
                    {
                        return "Итого на начало года";
                    }

                case F0503130Fields.TotalEnd:
                    {
                        return "Итого на конец года";
                    }

                case F0503130Fields.NumberOffBalance:
                    {
                        return "Номер забалансового счета";
                    }
            }

            return string.Empty;
        }

        public static string F0503137DetailsNameMapping(F0503137Details detail)
        {
            switch (detail)
            {
                case F0503137Details.Incomes:
                    {
                        return "Доходы";
                    }

                case F0503137Details.Expenses:
                    {
                        return "Расходы";
                    }

                case F0503137Details.SourcesOfFinancing:
                    {
                        return "Источники финансирования дефицита бюджета";
                    }
            }

            return string.Empty;
        }

        public static string F0503737DetailsNameMapping(F0503737Details detail)
        {
            switch (detail)
            {
                case F0503737Details.Incomes:
                    {
                        return "Доходы";
                    }

                case F0503737Details.Expenses:
                    {
                        return "Расходы";
                    }

                case F0503737Details.SourcesOfFinancing:
                    {
                        return "Источники финансирования дефицита бюджета";
                    }

                case F0503737Details.ReturnExpense:
                    {
                        return "Сведения о возвратах расходов и выплат обеспечений прошлых лет";
                    }
            }

            return string.Empty;
        }

        public static string F0503137NameMapping(F0503137Fields field, F0503137Details detail)
        {
            switch (field)
            {
                case F0503137Fields.ID:
                    {
                        return "ID";
                    }

                case F0503137Fields.Name:
                    {
                        return "Наименование показателя";
                    }

                case F0503137Fields.LineCode:
                    {
                        return "Код строки";
                    }

                case F0503137Fields.BudgClassifCode:
                    {
                        switch (detail)
                        {
                            case F0503137Details.Incomes:
                                {
                                    return "Код дохода по бюджетной классификации";
                                }

                            case F0503137Details.Expenses:
                                {
                                    return "Код расхода по бюджетной классификации";
                                }
                        }

                        return "Код источника финансирования по бюджетной классификации";
                    }

                case F0503137Fields.ApproveEstimateAssign:
                    {
                        return "Утвержденные сметные назначения";
                    }

                case F0503137Fields.ExecFinancAuthorities:
                    {
                        return "Исполнено через финансовые органы";
                    }

                case F0503137Fields.ExecBankAccounts:
                    {
                        return "Исполнено через банковские счета";
                    }

                case F0503137Fields.ExecNonCashOperation:
                    {
                        return "Исполнено через некассовые операции";
                    }

                case F0503137Fields.ExecTotal:
                    {
                        return "Исполнено итого";
                    }

                case F0503137Fields.UnexecAssignments:
                    {
                        return "Неисполненные назначения";
                    }
            }

            return string.Empty;
        }

        public static string F0503721DetailsNameMapping(F0503721Details detail)
        {
            switch (detail)
            {
                case F0503721Details.Incomes:
                    {
                        return "Доходы";
                    }

                case F0503721Details.Expenses:
                    {
                        return "Расходы";
                    }

                case F0503721Details.NonFinancialAssets:
                    {
                        return "Нефинансовые активы";
                    }

                case F0503721Details.FinancialAssetsLiabilities:
                    {
                        return "Финансовые активы и обязательства";
                    }
            }

            return string.Empty;
        }
        
        public static string F0503737NameMapping(F0503737Fields field, F0503737Details detail)
        {
            switch (field)
            {
                case F0503737Fields.ID:
                    {
                        return "ID";
                    }

                case F0503737Fields.Name:
                    {
                        return "Наименование показателя";
                    }

                case F0503737Fields.LineCode:
                    {
                        return "Код строки";
                    }

                case F0503737Fields.AnalyticCode:
                    {
                        return "Код аналитики";
                    }

                case F0503737Fields.ApprovePlanAssign:
                    {
                        return "Утверждено плановых назначений";
                    }

                case F0503737Fields.ExecPersonAuthorities:
                    {
                        return "Исполнено через лицевые счета";
                    }

                case F0503737Fields.ExecBankAccounts:
                    {
                        return "Исполнено через банковские счета";
                    }

                case F0503737Fields.ExecNonCashOperation:
                    {
                        return "Исполнено через некассовые операции";
                    }

                case F0503737Fields.ExecCashAgency:
                    {
                        return "Исполнено через кассу учреждения";
                    }

                case F0503737Fields.ExecTotal:
                    {
                        return "Исполнено итого";
                    }

                case F0503737Fields.UnexecPlanAssign:
                    {
                        return "Не исполнено плановых назначений";
                    }

                case F0503737Fields.RefTypeFinSupportName:
                    {
                        return "Вид финансового обеспечения";
                    }
            }

            return string.Empty;
        }

        public static string IndicatorsFieldsNameMapping(IndicatorsFields field)
        {
            switch (field)
            {
                case IndicatorsFields.ID:
                    {
                        return "ID";
                    }

                case IndicatorsFields.Name:
                    {
                        return "Наименование показателя";
                    }

                case IndicatorsFields.LineCode:
                    {
                        return "Код строки";
                    }

                case IndicatorsFields.Code:
                    {
                        return "Код";
                    }
            }

            return string.Empty;
        }

        public static string SettingsFieldsNameMapping(SettingsFields field)
        {
            switch (field)
            {
                case SettingsFields.ID:
                    {
                        return "ID";
                    }

                case SettingsFields.SectionName:
                    {
                        return "Детализация";
                    }

                case SettingsFields.RefIndicatorsName:
                    {
                        return "Наименование показателя";
                    }

                case SettingsFields.RefPartDocName:
                    {
                        return "Документ";
                    }

                case SettingsFields.Additional:
                    {
                        return "Дополнительное";
                    }

                case SettingsFields.StartYear:
                    {
                        return "Год начала";
                    }

                case SettingsFields.EndYear:
                    {
                        return "Год окончания";
                    }
            }

            return string.Empty;
        }
    }
}
