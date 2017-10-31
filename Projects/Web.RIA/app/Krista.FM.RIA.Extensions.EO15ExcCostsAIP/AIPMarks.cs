namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP
{
    internal static class AIPMarks
    {
        /// <summary>
        /// Показатель. Направление строительства
        /// </summary>
        public static readonly int MarkBuildDirection = 100010000;

        /// <summary>
        /// Показатель. Тип объетка
        /// </summary>
        public static readonly int MarkObjectType = 100020000;

        /// <summary>
        /// Показатель. Вид объекта
        /// </summary>
        public static readonly int MarkObjectKind = 100030000;

        /// <summary>
        /// Показатель. Основания для финансирования непрограммного объекта
        /// </summary>
        public static readonly int MarkReasonsNonProgram = 100040000;
        
        /// <summary>
        /// Показатель. План
        /// </summary>
        public static readonly int MarkPlan = 500010000;

        /// <summary>
        /// Показатель. Лимит
        /// </summary>
        public static readonly int MarkLimit = 400010000;

        /// <summary>
        /// Показатель. Смета в ценах 2001 года
        /// </summary>
        public static readonly int MarkSmeta2001 = 200010000;

        /// <summary>
        /// Показатель. Смета в текущих ценах
        /// </summary>
        public static readonly int MarkSmeta = 200020000;

        /// <summary>
        /// Показатель. Начальная цена контракта
        /// </summary>
        public static readonly int MarkStartPrice = 300010000;

        /// <summary>
        /// Показатель. Стоимость в текущих ценах по контракту
        /// </summary>
        public static readonly int MarkCurPrice = 300020000;
        
        /// <summary>
        /// Показатель. Ожидаемая стоимость основных фондов
        /// </summary>
        public static readonly int MarkExpectPrice = 300030000;

        /// <summary>
        /// Показатель. Финансирование
        /// </summary>
        public static readonly int MarkFinance = 700010000;

        /// <summary>
        /// Показатель. Профинансировано МО за счет субсидий бюджета АО
        /// </summary>
        public static readonly int MarkFinanceSubBudgetAO = 700020000;

        /// <summary>
        /// Показатель. Освоено в текущих ценах
        /// </summary>
        public static readonly int MarkUtilizedInCurPrice = 700030000;

        /// <summary>
        /// Показатель. Освоено в базовых ценах
        /// </summary>
        public static readonly int MarkUtilizedInBasePrice = 700040000;

        /// <summary>
        /// Показатель. Остаток средств в базовых ценах
        /// </summary>
        public static readonly int MarkBalanceBasePrice = 700050000;

        /// <summary>
        /// Показатель. Введено мощностей
        /// </summary>
        public static readonly int MarkPermissionCapacity = 800010000;

        /// <summary>
        /// Показатель. Введено основных фондов
        /// </summary>
        public static readonly int MarkPermissionGeneralFund = 800020000;

        /// <summary>
        /// Показатель. Введено основных фондов, в т.ч. за счет средств бюджета АО
        /// </summary>
        public static readonly int MarkPermissionGeneralFundAO = 800030000;

        /// <summary>
        /// Показатель. Разрешение на ввод в эксплуатацию
        /// </summary>
        public static readonly int MarkPermissionExploitation = 800040000;

        /// <summary>
        /// Показатель. Выбыло мощностей
        /// </summary>
        public static readonly int MarkRetireCapacity = 800050000;

        /// <summary>
        /// Показатель. Перепрофилировано мощностей
        /// </summary>
        public static readonly int MarkRedesignCapacity = 800060000;

        /// <summary>
        /// Показатель. Плановая численность персонала введенного объекта
        /// </summary>
        public static readonly int MarkPlanStaff = 800070000;

        /// <summary>
        /// Показатель. Плановая численность персонала введенного объекта, в т.ч. вновь созданные рабочие места
        /// </summary>
        public static readonly int MarkPlanStaffNew = 800080000;

        /// <summary>
        /// Показатель. Причины не ввода объекта в эксплуатацию
        /// </summary>
        public static readonly int MarkReasonsNotCommissioning = 800090000;
    }
}