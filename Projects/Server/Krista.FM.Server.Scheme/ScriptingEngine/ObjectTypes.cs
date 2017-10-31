namespace Krista.FM.Server.Scheme.ScriptingEngine
{
    /// <summary>
    /// Тыпы объектов в базе данны.
    /// </summary>
    internal enum ObjectTypes
    {
        /// <summary>
        /// Индекс.
        /// </summary>
        Index,
        /// <summary>
        /// Хранимая процедура.
        /// </summary>
        Procedure,
        /// <summary>
        /// Последовательность (генератор).
        /// </summary>
        Sequence,
        /// <summary>
        /// Отношение.
        /// </summary>
        Table,
        /// <summary>
        /// Триггер.
        /// </summary>
        Trigger,
        /// <summary>
        /// Представление.
        /// </summary>
        View,
        /// <summary>
        /// Ограничение.
        /// </summary>
        ForeignKeysConstraint 
    }
}
