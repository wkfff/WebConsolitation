using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Krista.FM.Common
{
    public abstract class ServerManagedObjectAbstract : IDisposable
    {
        public event EventHandler OnChange;


        protected void CallOnChange(object sender, EventArgs e)
        {
            if (OnChange != null)
                OnChange(this, null);
        }

        protected void CallOnChange()
        {
            CallOnChange(this, null);
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion

        #region Identifier

        /// <summary>
        /// Счетчик для генерации идентификаторов
        /// </summary>
        private static object g_identifier = 0;

        /// <summary>
        /// Идентификатор текущего объекта
        /// </summary>
        protected int identifier = -1;


        /// <summary>
        /// Возвращает следующий идентификатор из счетчика
        /// </summary>
        /// <returns>Новый идентификатор</returns>
        [DebuggerStepThrough()]
        protected static int GetNewIdentifier()
        {
            int localIdentifier;
            lock (g_identifier)
            {
                g_identifier = localIdentifier = Convert.ToInt32(g_identifier) + 1;
            }
            return localIdentifier;
        }

        #endregion
    }

    public abstract class ServerManagedObject<T> : ServerManagedObjectAbstract
    {
        protected T serverControl;

        public ServerManagedObject(T serverControl)
        {
            this.serverControl = serverControl;
        }

        [Browsable(false)]
        public virtual T ServerControl
        {
            get { return serverControl; }
            set { serverControl = value; }
        }
    }
}
