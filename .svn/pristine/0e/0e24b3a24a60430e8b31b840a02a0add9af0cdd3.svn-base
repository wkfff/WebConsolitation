using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Krista.FM.ServerLibrary
{
    /// <summary>
    /// Класс для передачи на клиент сериализованных свойств объекта
    /// </summary>
    [Serializable]
    public class SMOSerializationInfo : IDictionary<string, object>, ISerializable
    {
        /// <summary>
        /// Типизированная коллекция
        /// </summary>
        private Dictionary<string, object> dictionary = new Dictionary<string, object>();

        #region Конструктор

        /// <summary>
        /// Конструктор для десериализации объекта
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public SMOSerializationInfo(SerializationInfo info, StreamingContext context)
        {
            foreach (SerializationEntry entry in info)
            {
                Dictionary.Add(entry.Name, entry.Value);
            }
        }

        public SMOSerializationInfo(Dictionary<string, object> dictionary)
        {
            this.dictionary = dictionary;
        }

        #endregion

        public Dictionary<string, object> Dictionary
        {
            get { return dictionary; }
            set { dictionary = value; }
        }

        #region ISerializable Members

        /// <summary>
        /// Переопределенная сериализация объекта
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (KeyValuePair<string, object> pair in Dictionary)
            {
                info.AddValue(pair.Key, pair.Value);
            }
        }

        #endregion

        #region IDictionary<string,object> Members

        public bool ContainsKey(string key)
        {
            return dictionary.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            dictionary.Add(key, value);
        }

        public bool Remove(string key)
        {
            return dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public object this[string key]
        {
            get { return dictionary[key]; }
            set { dictionary[key] = value; }
        }

        public ICollection<string> Keys
        {
            get { return dictionary.Keys; }
        }

        public ICollection<object> Values
        {
            get { return dictionary.Values; }
        }

        #endregion

        #region ICollection<KeyValuePair<string,object>> Members

        public void Add(KeyValuePair<string, object> item)
        {
            dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return (dictionary.ContainsKey(item.Key) && dictionary.ContainsValue(item.Value));
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return dictionary.Remove(item.Key);
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,object>> Members

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, object>>)this).GetEnumerator();
        }

        #endregion
    }
}
