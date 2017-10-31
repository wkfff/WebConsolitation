namespace Krista.FM.RIA.Core
{
    /// <summary>
    /// Определяет обязательность атрибута.
    /// </summary>
    public enum Mandatory
    {
        /// <summary>
        /// Не может принимать пустые значения.
        /// </summary>
        NotNull,

        /// <summary>
        /// Может принимать пустые значения.
        /// </summary>
        Nullable
    }
}
