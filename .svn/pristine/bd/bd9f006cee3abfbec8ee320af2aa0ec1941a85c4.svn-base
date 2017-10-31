using System.Runtime.InteropServices;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Common.Tasks
{
    /// <summary>
    /// Предоставляет информацию о возможности редактирования объекта.
    /// </summary>
    [ComVisible(true)]
    internal abstract class TaskParamBaseStub : DisposableObject, ITaskParamBase
    {
        private readonly ITaskParamBase paramProxy;
        private readonly TaskItemsCollectionStub _parentCollection;

        protected TaskParamBaseStub(TaskItemsCollectionStub parentCollection, ITaskParamBase paramProxy)
        {
            this.paramProxy = paramProxy;
            _parentCollection = parentCollection;
            _parentCollection.AddToReturnedItems(this);
        }

        public int ID
        {
            get { return ParamProxy.ID; }
        }

        public bool Inherited
        {
            get { return ParamProxy.Inherited; }
        }

        public ITaskParamBase ParamProxy
        {
            get { return paramProxy; }
        }

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        /// <param name="disposing">вызван пользователем или сборщиком мусора</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ParamProxy.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
