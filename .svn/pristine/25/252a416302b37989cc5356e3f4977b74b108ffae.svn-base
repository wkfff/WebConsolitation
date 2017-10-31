using System.Runtime.InteropServices;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Common.Tasks
{
    [ComVisible(true)]
    internal class TaskParamStub : TaskConstStub, ITaskParam
    {
        internal TaskParamStub(TaskItemsCollectionStub parentCollection, ITaskParamBase paramStub)
            : base(parentCollection, paramStub)
        {
        }

        public string Dimension
        {
            get { return ((ITaskParam)ParamProxy).Dimension; }
            set { ((ITaskParam)ParamProxy).Dimension = value; }
        }

        public bool AllowMultiSelect
        {
            get { return ((ITaskParam)ParamProxy).AllowMultiSelect; }
            set { ((ITaskParam)ParamProxy).AllowMultiSelect = value; }
        }
    }
}
