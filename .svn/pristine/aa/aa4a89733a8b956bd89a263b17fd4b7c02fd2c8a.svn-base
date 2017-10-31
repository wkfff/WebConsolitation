using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Common;

namespace Krista.FM.Client.SMO
{
    public class SmoSimpleDictionary<Tkey, TValue> : ServerManagedObjectAbstract, IDictionary<Tkey, TValue> where TValue : ICloneable
    {
        Dictionary<Tkey, TValue> innerDictionary = new Dictionary<Tkey, TValue>();

        #region IDictionary<Tkey,TValue> Members

        public bool ContainsKey(Tkey key)
        {
            return innerDictionary.ContainsKey(key);
        }

        public void Add(Tkey key, TValue value)
        {
            innerDictionary.Add(key, value);
        }

        public bool Remove(Tkey key)
        {
            return innerDictionary.Remove(key);
        }

        public bool TryGetValue(Tkey key, out TValue value)
        {
            return innerDictionary.TryGetValue(key, out value);
        }

        public TValue this[Tkey key]
        {
            get { return innerDictionary[key]; }
            set { innerDictionary[key] = value; }
        }

        public ICollection<Tkey> Keys
        {
            get { return innerDictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return innerDictionary.Values; }
        }

        #endregion

        #region ICollection<KeyValuePair<Tkey,TValue>> Members

        public void Add(KeyValuePair<Tkey, TValue> item)
        {
            innerDictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            innerDictionary.Clear();
        }

        public bool Contains(KeyValuePair<Tkey, TValue> item)
        {
            return innerDictionary.ContainsKey(item.Key) && innerDictionary.ContainsValue(item.Value);
        }

        public void CopyTo(KeyValuePair<Tkey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<Tkey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return innerDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IEnumerable<KeyValuePair<Tkey,TValue>> Members

        IEnumerator<KeyValuePair<Tkey, TValue>> IEnumerable<KeyValuePair<Tkey, TValue>>.GetEnumerator()
        {
            return innerDictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<Tkey, TValue>>) this).GetEnumerator();
        }

        #endregion
    }
}
