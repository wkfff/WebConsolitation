using System;
using System.Threading;

namespace Krista.FM.Common
{
    public abstract class BaseLock : IDisposable
    {
        protected ReaderWriterLockSlim _Locks;

        protected BaseLock(ReaderWriterLockSlim locks)
        {
            _Locks = locks;
        }

        #region IDisposable Members

        public abstract void Dispose();

        #endregion
    }
}
