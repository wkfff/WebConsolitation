using System.Runtime.InteropServices;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Common.Tasks
{
    [ComVisible(true)]
    public class TaskContext : DisposableObject, ITaskContext
    {
        private readonly ITask taskProxy;
        private TaskConstsCollectionStub taskConstsStub;
        private TaskParamsCollectionStub taskParamsStub;

        public TaskContext(ITask taskProxy)
        {
            this.taskProxy = taskProxy;
        }

        public ITask TaskProxy
        {
            get { return taskProxy; }
        }

        public ITaskParamsCollection GetTaskParams()
        {
            return taskParamsStub ?? (taskParamsStub = new TaskParamsCollectionStub(TaskProxy.GetTaskParams(), null));
        }

        public ITaskConstsCollection GetTaskConsts()
        {
            return taskConstsStub ?? (taskConstsStub = new TaskConstsCollectionStub(TaskProxy.GetTaskConsts(), null));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (taskConstsStub != null)
                {
                    taskConstsStub.Dispose();
                }
                if (taskParamsStub != null)
                {
                    taskParamsStub.Dispose();
                }
                if (taskProxy != null)
                {
                    taskProxy.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
