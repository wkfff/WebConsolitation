using System.Runtime.InteropServices;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Common.Tasks
{
    [ComVisible(true)]
    internal class TaskConstsCollectionStub : TaskItemsCollectionStub, ITaskConstsCollection
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
}
