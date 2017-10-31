using System.Runtime.InteropServices;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Common.Tasks
{
    [ComVisible(true)]
    internal class TaskConstStub : TaskParamBaseStub, ITaskConst
    {
        public TaskConstStub(TaskItemsCollectionStub parentCollection, ITaskParamBase paramStub)
            : base(parentCollection, paramStub)
        {
        }

        public string Name
        {
            get { return ((ITaskConst)ParamProxy).Name; }
            set { ((ITaskConst)ParamProxy).Name = value; }
        }

        public string Comment
        {
            get { return ((ITaskConst)ParamProxy).Comment; }
            set { ((ITaskConst)ParamProxy).Comment = value; }
        }

        public object Values
        {
            get { return ((ITaskConst)ParamProxy).Values; }
            set { ((ITaskConst)ParamProxy).Values = value; }
        }
    }
}
