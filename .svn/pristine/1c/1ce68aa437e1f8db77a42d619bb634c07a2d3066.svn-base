using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Common
{
    /// <summary>
    /// Абстрактный предок для коллекций
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class DictionaryBase<TKey, TValue> : DisposableObject, /*IDictionaryBase<TKey, TValue>,*/ IDictionary<TKey, TValue> where TValue : ICloneable 
    {
        
        /// <summary>
        /// внутренний список
        /// </summary>
        protected Dictionary<TKey, TValue> list;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public DictionaryBase()
        {
            list = new Dictionary<TKey, TValue>();
        }

        #region IDictionaryBase Members

        /// <summary>
        /// New
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TValue New(TKey key)
        {
            throw new NotImplementedException(String.Format("Метод {0} у объекта {1} не реализован.", "New", this.GetType().ToString()));
        }

        /// <summary>
        /// Update
        /// </summary>
        public virtual void Update()
        {
            throw new NotImplementedException(String.Format("Метод {0} у объекта {1} не реализован.", "Update", this.GetType().ToString()));
        }

        #endregion

        #region IDictionary<TKey,TValue> Members

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void Add(TKey key, TValue value)
        {
            this.list.Add(key, value);
        }

        /// <summary>
        /// ContainsKey
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return this.list.ContainsKey(key);
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { return this.list.Keys; }
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool Remove(TKey key)
        {
            return this.list.Remove(key);
        }

        /// <summary>
        /// TryGetValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return list.TryGetValue(key, out value);
        }

        /// <summary>
        /// Values
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return this.list.Values; }
        }

        /// <summary>
        /// Индексатор
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TValue this[TKey key]
        {
            get
            {
                try
                {
                    return this.list[key];
                }
                catch (KeyNotFoundException)
                {
                    throw new KeyNotFoundException(String.Format("Ключ \"{0}\" не найден в коллекции \"{1}\"", key, this.ToString()));
                }
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(KeyValuePair<TKey, TValue> item)
        {
            list.Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Clear
        /// </summary>
        public virtual void Clear()
        {
            this.list.Clear();
        }
        
        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.list.ContainsKey(item.Key);
        }

        /// <summary>
        /// CopyTo
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Count
        /// </summary>
        public int Count
        {
            get { return this.list.Count; }
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion

        #region Object

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Count = {0} : {1}", Count, base.ToString());
        }

        #endregion Object
    }
}
