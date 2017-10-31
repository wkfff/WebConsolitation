using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Common.Calculator.Exceptions;

namespace Krista.FM.Common.Calculator.Implementation
{
    public class ValueStoreGroup : IValueStoreGroup
    {
        private readonly IDictionary<string, IValueStore> sources;

        private string currentSource;

        public ValueStoreGroup()
        {
            sources = new Dictionary<string, IValueStore>();
            currentSource = string.Empty;
        }

        #region IValueStore

        public ICollection<string> Index()
        {
            return GetStore(currentSource).Index();
        }

        public bool IsExists(string valueName)
        {
            return GetStore(currentSource).IsExists(valueName);
        }

        public IValueItem Get(string valueName)
        {
            return Get(valueName, CurrentStore);
        }

        #endregion

        #region IValueStoreGroup

        public string CurrentStore
        {
            get
            {
                if (String.IsNullOrEmpty(currentSource))
                {
                    throw new InvalidConfigurationException("Не указан текущий источник данных");
                }

                if (!GetStoresIndex().Contains(currentSource))
                {
                    throw new UndefinedIdentifierException(currentSource);
                }

                return currentSource;
            }

            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new InvalidConfigurationException("Не указан текущий источник данных");
                }

                if (!GetStoresIndex().Contains(value))
                {
                    throw new UndefinedIdentifierException(value);
                }

                currentSource = value;
            }
        }

        public IValueItem Get(string valueName, string valueStoreId)
        {
            return GetStore(valueStoreId).Get(valueName);
        }

        public void RegisterStore(string sourceId, IValueStore store)
        {
            if (store == null)
            {
                throw new InvalidConfigurationException("Должно быть указано хранилище данных");
            }

            if (GetStoresIndex().Contains(sourceId))
            {
                throw new DuplicateIdentifierException(sourceId);
            }

            sources[sourceId] = store;
        }

        public void UnregisterStore(string sourceId)
        {
            if (!GetStoresIndex().Contains(sourceId))
            {
                throw new UndefinedIdentifierException(sourceId);
            }

            sources.Remove(sourceId);
        }

        public IValueStore GetStore(string sourceId)
        {
            if (!sources.ContainsKey(sourceId))
            {
                throw new UndefinedIdentifierException(sourceId);
            }

            return sources[sourceId];
        }

        public ICollection<string> GetStoresIndex()
        {
            return (from keyValuePair in sources select keyValuePair.Key).ToList();
        }

        #endregion
    }
}
