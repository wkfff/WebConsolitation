namespace Krista.FM.Common.Calculator
{
    public interface IValueResolverEnvironment
    {
        object GetParameter(string name);

        void SetParameter(string name, object newValue);
    }
}
