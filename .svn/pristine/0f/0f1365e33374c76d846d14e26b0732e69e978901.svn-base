using System.Runtime.InteropServices;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Common.Tasks
{
    [ComVisible(true)]
    internal class TaskParamsCollectionStub : TaskItemsCollectionStub, ITaskParamsCollection
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
