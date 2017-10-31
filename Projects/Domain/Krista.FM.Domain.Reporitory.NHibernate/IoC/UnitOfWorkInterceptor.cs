using System;
using Krista.FM.Common;

namespace Krista.FM.Domain.Reporitory.NHibernate.IoC
{
    public class UnitOfWorkInterceptor : TransactionInterceptor
    {
        public UnitOfWorkInterceptor()
            : base(Resolver.Get<ITransactionManager>())
        {
        }

        protected override Type GetAttributeType()
        {
            return typeof(UnitOfWorkAttribute);
        }

        protected override object CloseUnitOfWork(
            TransactionAttributeSettings transactionAttributeSettings,
            object transactionState,
            Exception err)
        {
            transactionState = base.CloseUnitOfWork(transactionAttributeSettings, transactionState, err);
            if (transactionManager.TransactionDepth == 0)
            {
                var sessionStorage = (NHibernateSession.Storage as IUnitOfWorkSessionStorage);
                if (sessionStorage != null)
                {
                    sessionStorage.EndUnitOfWork(
                        ((UnitOfWorkAttributeSettings)transactionAttributeSettings).CloseSessions);
                }
            }

            return transactionState;
        }
    }
}
