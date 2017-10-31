using System.Threading;

namespace Krista.FM.Common
{
    public class ReadLock : BaseLock
    {
        public ReadLock(ReaderWriterLockSlim locks)
            : base(locks)
        {
            Locks.GetReadLock(_Locks);
        }

        public override void Dispose()
        {
            Locks.ReleaseReadLock(_Locks);
        }
    }
}