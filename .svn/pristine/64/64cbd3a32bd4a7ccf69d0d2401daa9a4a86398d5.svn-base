using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Common
{
    public abstract class ListBase<TValue> : SMOSerializable, /*IListBase<TValue>,*/ IListBase<TValue> where TValue : IServerSideObject, ICloneable
    {
        /// <summary>
        /// внутренний список
        /// </summary>
        protected List<TValue> list;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public ListBase(ServerSideObject owner, ServerSideObjectStates state)
            : base(owner, state)
        {
            list = new List<TValue>();
        }

        #region ServerSideObject

        /// <summary>
        /// Состояние серверного объекта во времени его существования
        /// </summary>
        public override ServerSideObjectStates State
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return base.State; }
            set
            {
                if (base.State != value)
                {
                    base.State = value;
                    foreach (TValue item in Instance.list)
                        ((ServerSideObject)(IServerSideObject)item).State = value;
                }
            }
        }

        protected ListBase<TValue> Instance
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return (ListBase<TValue>)GetInstance(); }
        }

        protected List<TValue> Accessor
        {
            get
            {
                List<TValue> accessor = list;
                if (!Authentication.IsSystemRole() && !IsClone && State != ServerSideObjectStates.New)
                {
                    accessor = ((ListBase<TValue>)Lock()).list;
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
                foreach (TValue item in list)
                    item.Unlock();

                base.Unlock();
            }
        }

        protected virtual void CloneItems(ListBase<TValue> clon)
        {
            foreach (TValue item in this.list)
            {
                clon.list.Add((TValue)item.Clone());
            }
        }

        /// <summary>
        /// Создает новый объект, являющийся копией текущего экземпляра. 
        /// </summary>
        /// <returns>Новый объект, являющийся копией текущего экземпляра.</returns>
        public override object Clone()
        {
            ListBase<TValue> clon = base.Clone() as ListBase<TValue>;
            clon.list = new List<TValue>();
            CloneItems(clon);
            return clon;
        }

        #endregion ServerSideObject

        #region IList<TValue> Members

        public int IndexOf(TValue item)
        {
            return Instance.list.IndexOf(item);
        }

        public void Insert(int index, TValue item)
        {
            Accessor.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Accessor.RemoveAt(index);
        }

        public TValue this[int index]
        {
            get
            {
                return Instance.list[index];
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region ICollection<TValue> Members

        public void Add(TValue item)
        {
            Accessor.Add(item);
        }

        public void Clear()
        {
            Accessor.Clear();
        }

        public bool Contains(TValue item)
        {
            return Instance.list.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Count
        {
            get { return Instance.list.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(TValue item)
        {
            return Accessor.Remove(item);
        }

        #endregion

        #region IEnumerable<TValue> Members

        public IEnumerator<TValue> GetEnumerator()
        {
            return Instance.list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Instance.list.GetEnumerator();
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
