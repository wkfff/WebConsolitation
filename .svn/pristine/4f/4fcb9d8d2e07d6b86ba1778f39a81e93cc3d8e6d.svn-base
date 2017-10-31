using System;
using System.Collections;
using System.Collections.Generic;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoDictionary2<TIntf, TKey, TValue> : ServerManagedObject<TIntf>, IDictionary<TKey, TValue>
    {
        IDictionary<TKey, TValue> dict;

        public SmoDictionary2(TIntf serverObject)
            : base(serverObject)
        {
            dict = (IDictionary<TKey, TValue>)serverObject;
        }

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            dict.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return dict.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return dict.Keys; }
        }

        public bool Remove(TKey key)
        {
            return dict.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dict.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return dict.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return dict[key];
            }
            set
            {
                dict[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            dict.Add(item);
        }

        public void Clear()
        {
            dict.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dict.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            dict.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return dict.Count; }
        }

        public bool IsReadOnly
        {
            get { return dict.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return dict.Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        #endregion
    }
}
