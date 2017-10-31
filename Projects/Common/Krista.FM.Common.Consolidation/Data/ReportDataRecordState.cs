namespace Krista.FM.Common.Consolidation.Data
{
    /// <summary>
    /// Состояние объекта ReportDataRecord.
    /// </summary>
    public enum ReportDataRecordState
    {
        /// <summary>
        /// Строка была добавлена в коллекцию.
        /// </summary>
        Added,

        /// <summary>
        /// Строка была изменена.
        /// </summary>
        Modified,

        /// <summary>
        /// Строка была удалена.
        /// </summary>
        Deleted,

        /// <summary>
        /// Строка не изменялась.
        /// </summary>
        Unchanged
    }
}
