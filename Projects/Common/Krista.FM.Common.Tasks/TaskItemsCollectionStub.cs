using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Common.Tasks
{
    [ComVisible(true)]
    internal abstract class TaskItemsCollectionStub : DisposableObject, ITaskItemsCollection
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
}
