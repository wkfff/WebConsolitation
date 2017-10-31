using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Tasks
{
    public class TaskManager : DisposableObject, ITaskManager
    {
        private IScheme scheme;
        private TaskCollection tasks;

        public TaskManager(IScheme scheme)
        {
            this.scheme = scheme;
            tasks = new TaskCollection(this.scheme);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // освобождаем управляемые ресурсы
                tasks.Dispose();
            }
            // освобождаем неуправляемые ресурсы
            // ...
        }

        public ITaskCollection Tasks
        {
            get { return tasks; }
        }

    }
}
