using System.Threading;

namespace Krista.FM.Common
{
    public class ReadOnlyLock : BaseLock
    {
        public ReadOnlyLock(ReaderWriterLockSlim locks)
            : base(locks)
        {
            Locks.GetReadOnlyLock(_Locks);
        }

        public override void Dispose()
        {
            Locks.ReleaseReadOnlyLock(_Locks);
        }
    }
}