using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    [ComVisibleAttribute(true)]
    public class TaskParamBaseStub : DisposableObject, ITaskParamBase
    {
        protected ITaskParamBase _paramProxy = null;
        public ITaskParamBase ParamProxy
        {
            get { return _paramProxy; }
        }

        public int ID
        {
            get { return _paramProxy.ID; }
        }


        private TaskItemsCollectionStub _parentCollection = null;

        internal TaskParamBaseStub(TaskItemsCollectionStub parentCollection, ITaskParamBase paramProxy)
        {
            _paramProxy = paramProxy;
            _parentCollection = parentCollection;
            _parentCollection.AddToReturnedItems(this);

        }

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        /// <param name="disposing">вызван пользователем или сборщиком мусора</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _paramProxy.Dispose();
                _paramProxy = null;
            }
            base.Dispose(disposing);
        }

        public bool Inherited
        {
            get { return _paramProxy.Inherited; }
        }

    }

    [ComVisibleAttribute(true)]
    public class TaskConstStub : TaskParamBaseStub, ITaskConst
    {

        internal TaskConstStub(TaskItemsCollectionStub parentCollection, ITaskParamBase paramStub)
            : base(parentCollection, paramStub)
        {
        }

        public string Name
        {
            get
            {
                return ((ITaskConst)_paramProxy).Name;
            }
            set
            {
                ((ITaskConst)_paramProxy).Name = value;
            }
        }

        public string Comment
        {
            get
            {
                return ((ITaskConst)_paramProxy).Comment;
            }
            set
            {
                ((ITaskConst)_paramProxy).Comment = value;
            }
        }

        public object Values
        {
            get
            {
                return ((ITaskConst)_paramProxy).Values;
            }
            set
            {
                ((ITaskConst)_paramProxy).Values = value;
            }
        }

    }

    [ComVisibleAttribute(true)]
    public class TaskParamStub : TaskConstStub, ITaskParam
    {
        internal TaskParamStub(TaskItemsCollectionStub parentCollection, ITaskParamBase paramStub)
            : base(parentCollection, paramStub)
        {
        }

        public string Dimension
        {
            get
            {
                return ((ITaskParam)_paramProxy).Dimension;
            }
            set
            {
                ((ITaskParam)_paramProxy).Dimension = value;
            }
        }

        public bool AllowMultiSelect
        {
            get
            {
                return ((ITaskParam)_paramProxy).AllowMultiSelect;
            }
            set
            {
                ((ITaskParam)_paramProxy).AllowMultiSelect = value;
            }
        }
    }

    [ComVisibleAttribute(true)]
    public abstract class TaskItemsCollectionStub : DisposableObject, ITaskItemsCollection 
    {
        protected ITaskItemsCollection _collectionProxy = null;

        public bool IsReadOnly
        {
            get { return _collectionProxy.IsReadOnly; }
        }

        /// <summary>
        /// Данные коллекции ( а нужен ли здесь вообще этот метод???)
        /// </summary>
        public DataTable ItemsTable
        {
            get 
            {
                DataTable dt = _collectionProxy.ItemsTable;
                // !!!
                if (this is TaskParamsCollectionStub)
                    dt.Columns.Add("ParsedParamValues", typeof(string));
                return dt; 
            }
        }

        private ReloadDelegate _onReload = null;

        public TaskItemsCollectionStub(ITaskItemsCollection collectionProxy, ReloadDelegate onReload)
        {
            _collectionProxy = collectionProxy;
            _onReload = onReload;
        }

        protected void ReloadClientCollection()
        {
            if (_onReload != null)
                _onReload();
        }

        private List<TaskParamBaseStub> returnedItems = new List<TaskParamBaseStub>();

        private void ClearReturnedItems()
        {
            foreach (TaskParamBaseStub stub in returnedItems)
                stub.Dispose();
            returnedItems.Clear();
        }

        internal void AddToReturnedItems(TaskParamBaseStub item)
        {
            returnedItems.Add(item);
        }

        internal void RemoveFromReturnedItems(TaskParamBaseStub item)
        {
            int index = returnedItems.IndexOf(item);
            if (index != -1)
            {
                returnedItems[index].Dispose();
                returnedItems.RemoveAt(index);
            }
        }

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        /// <param name="disposing">вызван пользователем или сборщиком мусора</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearReturnedItems();
                _collectionProxy = null;
            }
            base.Dispose(disposing);
        }

        public int Count
        {
            get { return _collectionProxy.Count; }
        }

        public void ReloadItemsTable()
        {
            _collectionProxy.ReloadItemsTable();
            ReloadClientCollection();
        }

        // синхронизируем себя с другой коллекцией
        public bool SyncWith(ITaskItemsCollection destination, out string errorStr)
        {
            return _collectionProxy.SyncWith(destination, out errorStr);
        }

        public void SaveChanges()
        {
            _collectionProxy.SaveChanges();
            ReloadClientCollection();
        }

        public void CancelChanges()
        {
            _collectionProxy.CancelChanges();
            ReloadClientCollection();
        }

        public bool HasChanges()
        {
            return _collectionProxy.HasChanges();
        }
    }

    [ComVisibleAttribute(true)]
    public class TaskConstsCollectionStub : TaskItemsCollectionStub, ITaskConstsCollection 
    {

        public TaskConstsCollectionStub(ITaskItemsCollection collectionProxy, ReloadDelegate onReload)
            : base(collectionProxy, onReload)
        {
        }

        public ITaskConst ConstByIndex(int index)
        {
            ITaskConst cnst = ((ITaskConstsCollection)_collectionProxy).ConstByIndex(index);
            if (cnst == null)
                return null;
            TaskParamBaseStub stub = new TaskConstStub(this, cnst);
            return stub as ITaskConst;
        }

        public ITaskConst ConstByID(int id)
        {
            ITaskConst cnst = ((ITaskConstsCollection)_collectionProxy).ConstByID(id);
            if (cnst == null)
                return null;
            TaskParamBaseStub stub = new TaskConstStub(this, cnst);
            return stub as ITaskConst;
        }

        public ITaskConst ConstByName(string name)
        {
            ITaskConst cnst = ((ITaskConstsCollection)_collectionProxy).ConstByName(name);
            if (cnst == null)
                return null;
            TaskParamBaseStub stub = new TaskConstStub(this, cnst);
            return stub as ITaskConst;
        }

        public ITaskConst AddNew()
        {
            TaskParamBaseStub stub = new TaskConstStub(this, ((ITaskConstsCollection)_collectionProxy).AddNew());
            ReloadClientCollection();
            return stub as ITaskConst; 
        }

        public void Remove(ITaskConst item)
        {
            ((ITaskConstsCollection)_collectionProxy).Remove(((TaskParamBaseStub)item).ParamProxy as ITaskConst);
            RemoveFromReturnedItems((TaskParamBaseStub)item);
            ReloadClientCollection();
        }
    }

    [ComVisibleAttribute(true)]
    public class TaskParamsCollectionStub : TaskItemsCollectionStub, ITaskParamsCollection 
    {
        public TaskParamsCollectionStub(ITaskItemsCollection collectionProxy, ReloadDelegate onReload)
            : base(collectionProxy, onReload)
        {
        }

        public ITaskParam ParamByIndex(int index)
        {
            ITaskParam prm = ((ITaskParamsCollection)_collectionProxy).ParamByIndex(index);
            if (prm == null)
                return null;
            TaskParamBaseStub stub = new TaskParamStub(this, prm);
            return stub as ITaskParam;
        }

        public ITaskParam ParamByID(int id)
        {
            ITaskParam prm = ((ITaskParamsCollection)_collectionProxy).ParamByID(id);
            if (prm == null)
                return null;
            TaskParamBaseStub stub = new TaskParamStub(this, prm);
            return stub as ITaskParam;
        }

        public ITaskParam ParamByName(string name)
        {
            ITaskParam prm = ((ITaskParamsCollection)_collectionProxy).ParamByName(name);
            if (prm == null)
                return null;
            TaskParamBaseStub stub = new TaskParamStub(this, prm);
            return stub as ITaskParam;
        }

        public ITaskParam AddNew()
        {
            TaskParamBaseStub stub = new TaskParamStub(this, ((ITaskParamsCollection)_collectionProxy).AddNew());
            ReloadClientCollection();
            return stub as ITaskParam;
        }

        public void Remove(ITaskParam item)
        {
            ((ITaskParamsCollection)_collectionProxy).Remove(((TaskParamBaseStub)item).ParamProxy as ITaskParam);
            RemoveFromReturnedItems((TaskParamBaseStub)item);
            ReloadClientCollection();
        }

    }

}
#region Эксперименты
/* 
        #region IBindingList
        // Events
        private ListChangedEventHandler _onListChanged = null;
        public event ListChangedEventHandler ListChanged
        {
            add { _onListChanged += value; }
            remove { _onListChanged -= value; }
        }

        // Methods
        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public object AddNew()
        {
            //throw new NotImplementedException();
            return (object)InternalAddNew();
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotImplementedException();
        }

        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotImplementedException();
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public void RemoveSort()
        {
            throw new NotImplementedException();
        }


        // Properties
        public bool AllowEdit
        {
            get { return true; }
        }

        public bool AllowNew
        {
            get { return true; }
        }

        public bool AllowRemove
        {
            get { return true; }
        }

        public bool IsSorted
        {
            get { return true; }
        }

        public ListSortDirection SortDirection
        {
            get { return ListSortDirection.Ascending; }
        }

        public PropertyDescriptor SortProperty
        {
            get { return null; }
        }

        public bool SupportsChangeNotification
        {
            get { return true; }
        }

        public bool SupportsSearching
        {
            get { return true; }
        }

        public bool SupportsSorting
        {
            get { return true; }
        }
        #endregion

        #region  IList
        // Methods
        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        // Properties
        public bool IsFixedSize
        {
            get { return false; }
        }


        public object this[int index]
        {
            get
            {
                return InternalItemByIndex(index);
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region ICollection
        // Methods
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        // Properties

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return false; }
        }
        #endregion

        #region IEnumerable
        public IEnumerator GetEnumerator()
        {
            return this as IEnumerator;
        }
        #endregion

        #region IEnumerator

        private int _curIndex = 0;
        public bool MoveNext()
        {
            _curIndex++;
            return (_curIndex >= Count - 1);
        }

        public object Current
        {
            get { return InternalItemByIndex(_curIndex); }
        }

        public void Reset()
        {
            _curIndex = 0;
        }
        #endregion
  */
#endregion
