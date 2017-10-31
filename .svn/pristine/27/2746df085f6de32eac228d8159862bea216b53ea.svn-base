using System;
using System.Diagnostics;
using System.Threading;

namespace Krista.FM.Domain.Reporitory.NHibernate
{
    [Serializable]
    public abstract class TransactionManagerBase : ITransactionManager
    {
        private static readonly TraceSource logger = new TraceSource(typeof(TransactionManagerBase).ToString());

        [ThreadStatic]
        private static int transactionDepth;

        #region ITransactionManager Members

        public int TransactionDepth
        {
            get { return transactionDepth; }
        }

        public virtual object PushTransaction(string factoryKey, object transactionState)
        {
            Interlocked.Increment(ref transactionDepth);
            Log(String.Format("Push Transaction to Depth {0}", transactionDepth));
            return transactionState;
        }

        public abstract bool TransactionIsActive(string factoryKey);

        public virtual object PopTransaction(string factoryKey, object transactionState)
        {
            Interlocked.Decrement(ref transactionDepth);
            Log(String.Format("Pop Transaction to Depth {0}", transactionDepth));
            return transactionState;
        }

        public abstract object RollbackTransaction(string factoryKey, object transactionState);
        
        public abstract object CommitTransaction(string factoryKey, object transactionState);
        
        public abstract string Name { get; }

        #endregion

        protected void Log(string message)
        {
            logger.TraceEvent(TraceEventType.Verbose, 0, "{0}: {1}", Name, message);
        }
    }
}
