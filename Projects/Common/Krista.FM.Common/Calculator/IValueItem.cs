namespace Krista.FM.Common.Calculator
{
    /// <summary>
    /// Обобщенное представление результата вычисления
    /// </summary>
    public interface IValueItem
    {
        string Name { get; }

        object Value { get; set; }
    }
}
