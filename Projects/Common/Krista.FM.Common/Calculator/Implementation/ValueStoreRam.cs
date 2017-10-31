using System.Collections.Generic;
using System.Linq;
using Krista.FM.Common.Calculator.Exceptions;

namespace Krista.FM.Common.Calculator.Implementation
{
    public class ValueStoreRam : IValueItemStore, IValueResolver
    {
        private readonly IList<IValueItem> values;

        public ValueStoreRam()
        {
            values = new List<IValueItem>();
        }

        #region IValueResolver

        public object GetValue(string valueName)
        {
            return Get(valueName).Value;
        }

        public void SetValue(string valueName, object newValue)
        {
            Get(valueName).Value = newValue;
        }

        #endregion

        #region IValueItemStore

        public ICollection<string> Index()
        {
            return (from cursor in values select cursor.Name).ToList();
        }

        public IValueItem Get(string valueName)
        {
            if (values.All(v => v.Name != valueName))
            {
                throw new UndefinedIdentifierException(valueName);
            }

            return values.SingleOrDefault(v => v.Name == valueName);
        }

        #endregion

        public void Add(IValueItem valueItem)
        {
            if (values.Any(v => v.Name == valueItem.Name))
            {
                throw new DuplicateIdentifierException(valueItem.Name);
            }

            values.Add(valueItem);
        }

        public void Drop(string valueName)
        {
            if (values.All(v => v.Name != valueName))
            {
                throw new UndefinedIdentifierException(valueName);
            }

            values.Remove(Get(valueName));
        }

        public void Clear()
        {
            values.Clear();
        }
    }
}
