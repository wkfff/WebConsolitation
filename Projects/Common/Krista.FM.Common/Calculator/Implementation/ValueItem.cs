namespace Krista.FM.Common.Calculator.Implementation
{
    public class ValueItem : IValueItem
    {
        public ValueItem(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public object Value { get; set; }
    }
}
