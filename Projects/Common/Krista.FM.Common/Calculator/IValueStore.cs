using System.Collections.Generic;

namespace Krista.FM.Common.Calculator
{
    public interface IValueStore
    {
        ICollection<string> Index();

        bool IsExists(string valueName);

        IValueItem Get(string valueName);
    }
}
