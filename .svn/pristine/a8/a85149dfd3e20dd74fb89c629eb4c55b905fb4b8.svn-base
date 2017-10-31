using System;
using System.Reflection;
using Krista.FM.Common;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Krista.FM.Domain.Reporitory.NHibernate.IoC
{
    public class TransactionInterceptor : ICallHandler
    {
        public int Order { get; set; }

        protected readonly ITransactionManager transactionManager;

        public TransactionInterceptor(ITransactionManager transactionManager)
        {
            Check.Require(transactionManager != null, "transactionManager");

            this.transactionManager = transactionManager;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            MethodBase methodInfo = input.MethodBase;

            Type attributeType = GetAttributeType();

            var classAttributes =
                (ITransactionAttributeSettings[])
                methodInfo.ReflectedType.GetCustomAttributes(attributeType, false);

            var methodAttributes =
                (ITransactionAttributeSettings[])
                methodInfo.GetCustomAttributes(attributeType, false);

            if (classAttributes.Length == 0 && methodAttributes.Length == 0)
            {
                return getNext()(input, getNext);
            }
            
            TransactionAttributeSettings transactionAttributeSettings =
                GetTransactionAttributeSettings(methodAttributes, classAttributes);

            object transactionState = OnEntry(transactionAttributeSettings, null);

            var methodReturn = getNext()(input, getNext);

            if (methodReturn.Exception != null)
            {
                CloseUnitOfWork(transactionAttributeSettings, transactionState, methodReturn.Exception);
                
                if (transactionManager.TransactionDepth == 0 &&
                    (transactionAttributeSettings.IsExceptionSilent || methodReturn.Exception is AbortTransactionException))
                {
                    methodReturn.Exception = null;
                    methodReturn.ReturnValue = transactionAttributeSettings.ReturnValue;
                    return methodReturn;
                }

                return methodReturn;
            }
            
            OnSuccess(transactionAttributeSettings, transactionState);

            return methodReturn;
        }

        protected virtual Type GetAttributeType()
        {
            return typeof(TransactionAttribute);
        }

        private TransactionAttributeSettings GetTransactionAttributeSettings(
            ITransactionAttributeSettings[] methodAttributes,
            ITransactionAttributeSettings[] classAttributes)
        {
            var transactionAttributeSettings = new TransactionAttributeSettings();

            if (methodAttributes.Length > 0)
            {
                transactionAttributeSettings = methodAttributes[methodAttributes.Length - 1].Settings;
            }
            else if (classAttributes.Length > 0)
            {
                transactionAttributeSettings = classAttributes[classAttributes.Length - 1].Settings;
            }

            return transactionAttributeSettings;
        }

        private object OnEntry(TransactionAttributeSettings transactionAttributeSettings, object transactionState)
        {
            return transactionManager.PushTransaction(String.Empty, transactionState);
        }

        private object OnSuccess(TransactionAttributeSettings transactionAttributeSettings, object transactionState)
        {
            return CloseUnitOfWork(transactionAttributeSettings, transactionState, null);
        }

        protected virtual object CloseUnitOfWork(
            TransactionAttributeSettings transactionAttributeSettings,
            object transactionState,
            Exception err)
        {
            string factoryKey = String.Empty;
            if (err == null)
            {
                try
                {
                    NHibernateSession.Current.Flush();
                    transactionState = transactionManager.CommitTransaction(factoryKey, transactionState);
                }
                catch (Exception)
                {
                    transactionState = transactionManager.RollbackTransaction(factoryKey, transactionState);
                    transactionState = transactionManager.PopTransaction(factoryKey, transactionState);
                    throw;
                }
            }
            else
            {
                transactionState = transactionManager.RollbackTransaction(factoryKey, transactionState);
            }

            transactionState = transactionManager.PopTransaction(factoryKey, transactionState);

            return transactionState;
        }
    }
}
