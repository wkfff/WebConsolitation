namespace Krista.FM.Common.Consolidation.Calculations.Expressions
{
    public enum BinaryExpressionTypes
    {
        /// <summary>
        /// Логическое И.
        /// </summary>
        And,

        /// <summary>
        /// Локическое ИЛИ.
        /// </summary>
        Or,

        /// <summary>
        /// Оператор сравнения Не равно.
        /// </summary>
        NotEqual,

        /// <summary>
        /// Оператор сравнения Меньше или равно.
        /// </summary>
        LesserOrEqual,

        /// <summary>
        /// Оператор сравнения Больше или равно.
        /// </summary>
        GreaterOrEqual,

        /// <summary>
        /// Оператор сравнения Меньше.
        /// </summary>
        Lesser,

        /// <summary>
        /// Оператор сравнения Больше.
        /// </summary>
        Greater,

        /// <summary>
        /// Оператор сравнения Равенство.
        /// </summary>
        Equal,

        /// <summary>
        /// Арифметическое вычитание.
        /// </summary>
        Minus,

        /// <summary>
        /// Арифметическое сложение.
        /// </summary>
        Plus,

        /// <summary>
        /// Деление по модулю.
        /// </summary>
        Modulo,

        /// <summary>
        /// Оператор деления.
        /// </summary>
        Div,

        /// <summary>
        /// Оператор умножения.
        /// </summary>
        Times,
        
        /// <summary>
        /// Неопределенный оператор.
        /// </summary>
        Unknown
    }
}
