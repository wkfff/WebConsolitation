namespace Krista.FM.Common.Calculator
{
    public interface IValueResolver
    {
        object GetValue(string valueName);

        void SetValue(string valueName, object newValue);
    }
}
