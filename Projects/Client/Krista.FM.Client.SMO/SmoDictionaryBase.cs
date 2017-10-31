using System;
using System.Collections.Generic;
using System.ComponentModel;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO
{
    public /*abstract*/ class SmoDictionaryBase<TKey, TValue> : SMOSerializableObject<IDictionaryBase<TKey, TValue>>, IDictionaryBase<TKey, TValue>
    {
        private Dictionary<TKey, TValue> innerDictionary;


        public SmoDictionaryBase(IDictionaryBase<TKey, TValue> serverObject)
            : base(serverObject)
        {
        }

        public SmoDictionaryBase(SMOSerializationInfo cache)
            : base(cache)
        {
            innerDictionary = new Dictionary<TKey, TValue>();

            FillCache();
        }

        protected virtual Type GetItemValueSmoObjectType(object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Заполнение коллекции из кэша
        /// </summary>
        private void FillCache()
        {
            foreach (KeyValuePair<string, object> pair in CachedValues)
            {
                if (pair.Key != "this" && pair.Key != "Identifier")
                {
                    if (pair.Value is SMOSerializationInfo)
                    {
                        SMOSerializationInfo info = (SMOSerializationInfo)pair.Value;

                        innerDictionary.Add((TKey)(object)pair.Key, (TValue)(object)SmoObjectsCache.GetSmoObject(GetItemValueSmoObjectType(info["this"]), info));
                    }
                    else
                    {
                        // в этом случае объект коллекции - proxy
                        if (pair.Value is TValue)
                            innerDictionary.Add((TKey)(object)pair.Key, (TValue)pair.Value);
                    }
                }
            }

            ClearCache();
        }

        /// <summary>
        /// Очистка кэша после заполнения коллекции
        /// </summary>
        private void ClearCache()
        {
            foreach (KeyValuePair<TKey, TValue> pair in innerDictionary)
            {
                if (CachedValues.ContainsKey(Convert.ToString(pair.Key)))
                    CachedValues.Remove(Convert.ToString(pair.Key));
            }
        }

        protected Dictionary<TKey, TValue> Dictionary
        {
            get { return innerDictionary; }
            set { innerDictionary = value; }
        }


        #region IDictionaryBase<TKey,TValue> Members

        public TValue New(TKey key)
        {
            return ServerControl.New(key);
        }

        public void Update()
        {
            ServerControl.Update();
        }

        #endregion

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            if (cached)
            {
                innerDictionary.Add(key, value);
            }
            else
                throw new Exception("Только для сериализованной коллекции можно создавать объекты");
        }

        public bool ContainsKey(TKey key)
        {
            return cached ? innerDictionary.ContainsKey(key) : serverControl.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return cached ? innerDictionary.Keys : serverControl.Keys; }
        }

        public bool Remove(TKey key)
        {
            if (cached) 
                return innerDictionary.Remove(key);
            else 
                throw new Exception("Только для сериализованной коллекции можно удалять объекты");
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return cached ? innerDictionary.TryGetValue(key, out value) : serverControl.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return cached ? innerDictionary.Values : serverControl.Values; }
        }

        /// <summary>
        /// Если объект закэширован (cached = true), то всегда возвращаем сериализованный объект,
        /// даже если для коллекции реализована неглубокая сериализация
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                return cached ? (TValue)(object)SmoObjectsCache.GetSmoObject(GetItemValueSmoObjectType(serverControl[key]),
                    (ISMOSerializable)serverControl[key]) : serverControl[key];
            }
            set
            {
                if (cached)
                    innerDictionary[key] = value;
                else
                    throw new Exception("Только для сериализованной коллекции можно изменять объекты");
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (cached)
                innerDictionary.Add(item.Key, item.Value);
            else
                throw new Exception("Только для сериализованной коллекции можно создавать объекты");
        }

        public void Clear()
        {
            if (cached) 
                innerDictionary.Clear();
            else
                throw new Exception("Только для сериализованной коллекции можно очищать объекты");

        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return
                cached
                    ? (innerDictionary.ContainsKey(item.Key) && innerDictionary.ContainsValue(item.Value))
                    : (serverControl.ContainsKey(item.Key));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Count
        {
            get { return cached ? innerDictionary.Count : serverControl.Count; }
        }

        public bool IsReadOnly
        {
            get { return serverControl.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (cached) 
                return innerDictionary.Remove(item.Key);
            else 
                throw new Exception("Только для сериализованной коллекции можно удалять объекты");
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return (IEnumerator<KeyValuePair<TKey, TValue>>) Enumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Enumerator();
        }

        /// <summary>
        /// Возвращает IEnumerator. Переопределен для коллекции атрибутов (по аналогии с сервером)
        /// </summary>
        /// <returns></returns>
        protected virtual System.Collections.IEnumerator Enumerator()
        {
            return cached ? innerDictionary.GetEnumerator() : serverControl.GetEnumerator();   
        }

        #endregion
    }
}
