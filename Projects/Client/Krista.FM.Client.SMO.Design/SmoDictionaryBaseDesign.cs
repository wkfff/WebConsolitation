using System;
using System.Collections.Generic;
using System.ComponentModel;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public /*abstract*/ class SmoDictionaryBaseDesign<TKey, TValue> : SmoServerSideObjectDesign<IDictionaryBase<TKey, TValue>>, IDictionaryBase<TKey, TValue>
    {
        public SmoDictionaryBaseDesign(IDictionaryBase<TKey, TValue> serverControl)
            : base(serverControl)
        {
        }

        protected virtual Type GetItemValueSmoObjectType(object obj)
        {
            throw new NotImplementedException();
        }
        
        #region IDictionaryBase<TKey,TValue> Members

        public TValue New(TKey key)
        {
            return serverControl.New(key);
        }

        public void Update()
        {
            serverControl.Update();
        }

        #endregion

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            serverControl.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return serverControl.ContainsKey(key);
        }

        [Browsable(false)]
        public ICollection<TKey> Keys
        {
            get { return serverControl.Keys; }
        }

        public bool Remove(TKey key)
        {
            return serverControl.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return serverControl.TryGetValue(key, out value);
        }

        [Browsable(false)]
        public ICollection<TValue> Values
        {
            get { return serverControl.Values; }
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
                return serverControl[key];
            }
            set
            {
                serverControl[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            serverControl.Add(item);            
        }

        public void Clear()
        {
            serverControl.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return
                serverControl.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Count
        {
            get { return serverControl.Count; }
        }

        [Browsable(false)]
        public bool IsReadOnly
        {
            get { return serverControl.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return serverControl.Remove(item);            
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return (IEnumerator<KeyValuePair<TKey, TValue>>)Enumerator();
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
            return serverControl.GetEnumerator();
        }

        #endregion

        public SMOSerializationInfo GetSMOObjectData()
        {
            throw new NotImplementedException();
        }

        public SMOSerializationInfo GetSMOObjectData(LevelSerialization level)
        {
            throw new NotImplementedException();
        }
    }
}
