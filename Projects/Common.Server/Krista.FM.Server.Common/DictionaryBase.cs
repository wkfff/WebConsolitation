using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Common;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Common
{
    /// <summary>
    /// Абстрактный предок для коллекций
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class DictionaryBase<TKey, TValue> : SMOSerializable, IDictionaryBase<TKey, TValue>, IDictionary<TKey, TValue> where TValue : IServerSideObject, ICloneable
    {
        
        /// <summary>
        /// внутренний список
        /// </summary>
        protected Dictionary<TKey, TValue> list;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public DictionaryBase(ServerSideObject owner, ServerSideObjectStates state)
            : base(owner, state)
        {
            list = new Dictionary<TKey, TValue>();
        }

        protected DictionaryBase<TKey, TValue> Instance
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return (DictionaryBase<TKey, TValue>)GetInstance(); }
        }

        protected Dictionary<TKey, TValue> Accessor
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get
            {
                Dictionary<TKey, TValue> accessor = list;
                if (!Authentication.IsSystemRole() && !IsClone && State != ServerSideObjectStates.New)
                {
                    if (CheckEditMode())
                    {
                        accessor = ((DictionaryBase<TKey, TValue>)Lock()).list;
                    }
                }
                return accessor;
            }
        }

        /// <summary>
        /// Снимает блокировку с объекта
        /// </summary>
        public override void Unlock()
        {
            if (IsLocked)
            {
                foreach (TValue item in list.Values)
                    item.Unlock();

                base.Unlock();
            }
        }

        #region ISMOSerializable

        /// <summary>
        /// Переопределенный метод сериализации для коллекций
        /// </summary>
        /// <param name="levelSerialization">Глубина сериализации коллекции</param>
        /// <returns></returns>
        public override SMOSerializationInfo GetSMOObjectData(LevelSerialization levelSerialization)
        {
            SMOSerializationInfo info = base.GetSMOObjectData(levelSerialization);

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                if (levelSerialization == LevelSerialization.DeepSerialize)
                    info.Add(Convert.ToString(pair.Key), ((ISMOSerializable)pair.Value).GetSMOObjectData());
                else
                    info.Add(Convert.ToString(pair.Key), pair.Value);
            }

            return info;
        }

        #endregion ISMOSerializable

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
            try
            {
                Accessor.Add(key, value);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException(String.Format("Ключ \"{0}\" уже присутствует в коллекции \"{1}\"", key, this.ToString()));
            }
        }

        /// <summary>
        /// ContainsKey
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerStepThrough()]
        public virtual bool ContainsKey(TKey key)
        {
            return Instance.list.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return Instance.list.Keys; }
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool Remove(TKey key)
        {
            return Accessor.Remove(key);
        }

        /// <summary>
        /// TryGetValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return Instance.TryGetValue(key, out value);
        }

        /// <summary>
        /// Values
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return Instance.list.Values; }
        }

        /// <summary>
        /// Индексатор
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TValue this[TKey key]
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get
            {
                try
                {
                    return Instance.list[key];
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
            Accessor.Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return true; }
        }

        /// <summary>
        /// Clear
        /// </summary>
        public virtual void Clear()
        {
            Accessor.Clear();
        }
        
        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [System.Diagnostics.DebuggerStepThrough()]
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Instance.list.ContainsKey(item.Key);
        }

        /// <summary>
        /// Copies all the items in the collection into an array. Implemented by
        /// using the enumerator returned from GetEnumerator to get all the items
        /// and copy them to the provided array.
        /// </summary>
        /// <param name="array">Array to copy to.</param>
        /// <param name="arrayIndex">Starting index in <paramref name="array"/> to copy to.</param>
        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            int count = this.Count;

            if (count == 0)
                return;

            if (array == null)
                throw new ArgumentNullException("array");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", count, "The argument may not be less than zero.");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "The argument may not be less than zero.");
            if (arrayIndex >= array.Length || count > array.Length - arrayIndex)
                throw new ArgumentException("arrayIndex", "The array is too small to hold all of the items.");

            int index = arrayIndex, i = 0;
            foreach (KeyValuePair<TKey, TValue> item in this)
            {
                if (i >= count)
                    break;

                array[index] = item;
                ++index;
                ++i;
            }
        }

        /// <summary>
        /// Count
        /// </summary>
        public int Count
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return Instance.list.Count; }
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
            return (IEnumerator<KeyValuePair<TKey, TValue>>)Enumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Enumerator();
        }

        protected virtual IEnumerator Enumerator()
        {
            return Instance.list.GetEnumerator();
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clon"></param>
        /// <param name="cloneItems"></param>
        protected virtual void CloneItems(DictionaryBase<TKey, TValue> clon, bool cloneItems)
        {
            foreach (KeyValuePair<TKey, TValue> item in this.list)
            {
                clon.list.Add(item.Key, cloneItems ? (TValue)item.Value.Clone() : item.Value);
            }
        }

        /// <summary>
        /// Создает новый объект, являющийся копией текущего экземпляра. 
        /// </summary>
        /// <param name="cloneItems">True - значения TValue клонируются, false - клонируется только список, элементы не клонируются</param>
        /// <returns>Новый объект, являющийся копией текущего экземпляра.</returns>
        public virtual object Clone(bool cloneItems)
        {
            DictionaryBase<TKey, TValue> clon = base.Clone() as DictionaryBase<TKey, TValue>;
            clon.list = new Dictionary<TKey, TValue>();
            CloneItems(clon, cloneItems);
            return clon;
        }

        /// <summary>
        /// Создает новый объект, являющийся копией текущего экземпляра. 
        /// </summary>
        /// <returns>Новый объект, являющийся копией текущего экземпляра.</returns>
        public override object Clone()
        {
            return Clone(true);
        }

        #endregion
        
        #region Object

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Count = {0} : {1}", Instance.list.Count, base.ToString());
        }

        #endregion Object
    }
}
