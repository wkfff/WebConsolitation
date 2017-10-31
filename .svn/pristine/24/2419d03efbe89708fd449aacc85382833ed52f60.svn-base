using System;
using Krista.FM.Common;
using Krista.FM.Domain.Reporitory.NHibernate.IoC;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using NHibernate;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.Domain.Reporitory.NHibernate.Tests
{
    [TestFixture]
    public class UnitOfWorkInterceptorTests
    {
        private UnityContainer container;
        private MockRepository repository;
        private ISession session;
        private ITransactionManager transactionManager;
        private IUnitOfWorkSessionStorage sessionStorage;

        [SetUp]
        public void SetUp()
        {
            repository = new MockRepository();
            session = repository.DynamicMock<ISession>();
            transactionManager = repository.DynamicMock<ITransactionManager>();
            sessionStorage = repository.DynamicMock<IUnitOfWorkSessionStorage>();

            NHibernateSession.Storage = sessionStorage;

            UnitySetup();
        }

        [TearDown]
        public void TearDown()
        {
            repository.VerifyAll();
        }

        [Test]
        public void SuccessTest()
        {
            Expect.Call(sessionStorage.Session).Return(session).Repeat.Once();
            Expect.Call(transactionManager.PushTransaction(null, null)).Return(null).Repeat.Once().IgnoreArguments();
            Expect.Call(session.Flush).Repeat.Once();
            Expect.Call(transactionManager.CommitTransaction(null, null)).Return(null).Repeat.Once().IgnoreArguments();
            Expect.Call(transactionManager.PopTransaction(null, null)).Return(null).Repeat.Once().IgnoreArguments();
            Expect.Call(transactionManager.TransactionDepth).Return(0).Repeat.Once();
            Expect.Call(() => sessionStorage.EndUnitOfWork(true)).Repeat.Once();

            repository.ReplayAll();

            var obj = Resolver.Get<UnitOfWorkTestClass>();
            obj.Do();
        }

        [Test]
        public void WithoutCloseSessionsTest()
        {
            Expect.Call(sessionStorage.Session).Return(session).Repeat.Once();
            Expect.Call(transactionManager.PushTransaction(null, null)).Return(null).Repeat.Once().IgnoreArguments();
            Expect.Call(session.Flush).Repeat.Once();
            Expect.Call(transactionManager.CommitTransaction(null, null)).Return(null).Repeat.Once().IgnoreArguments();
            Expect.Call(transactionManager.PopTransaction(null, null)).Return(null).Repeat.Once().IgnoreArguments();
            Expect.Call(transactionManager.TransactionDepth).Return(0).Repeat.Once();
            Expect.Call(() => sessionStorage.EndUnitOfWork(false)).Repeat.Once();

            repository.ReplayAll();

            var obj = Resolver.Get<UnitOfWorkTestClass>();
            obj.DoWithoutCloseSessions();
        }

        [Test]
        public void ExceptionTest()
        {
            Expect.Call(transactionManager.PushTransaction(null, null)).Return(null).Repeat.Once().IgnoreArguments();
            Expect.Call(transactionManager.RollbackTransaction(null, null)).Return(null).Repeat.Once().IgnoreArguments();
            Expect.Call(transactionManager.PopTransaction(null, null)).Return(null).Repeat.Once().IgnoreArguments();
            Expect.Call(transactionManager.TransactionDepth).Return(0).Repeat.Once();
            Expect.Call(() => sessionStorage.EndUnitOfWork(true)).Repeat.Once();

            repository.ReplayAll();

            var obj = Resolver.Get<UnitOfWorkTestClass>();
            try
            {
                obj.ThrowException();
            }
            catch
            {
            }
        }

        [Test]
        public void ExceptionSilentTest()
        {
            Expect.Call(transactionManager.PushTransaction(null, null)).Return(null).Repeat.Once().IgnoreArguments();
            Expect.Call(transactionManager.RollbackTransaction(null, null)).Return(null).Repeat.Once().IgnoreArguments();
            Expect.Call(transactionManager.PopTransaction(null, null)).Return(null).Repeat.Once().IgnoreArguments();
            Expect.Call(transactionManager.TransactionDepth).Return(0).Repeat.Once();
            Expect.Call(() => sessionStorage.EndUnitOfWork(true)).Repeat.Once();

            repository.ReplayAll();

            var obj = Resolver.Get<UnitOfWorkTestClass>();
            obj.DoExceptionSilent();
        }

        [Test]
        public void AbortTransactionExceptionTest()
        {
            Expect.Call(transactionManager.PushTransaction(null, null)).Return(null).Repeat.Once().IgnoreArguments();
            Expect.Call(transactionManager.RollbackTransaction(null, null)).Return(null).Repeat.Once().IgnoreArguments();
            Expect.Call(transactionManager.PopTransaction(null, null)).Return(null).Repeat.Once().IgnoreArguments();
            Expect.Call(transactionManager.TransactionDepth).Return(0).Repeat.Once();
            Expect.Call(() => sessionStorage.EndUnitOfWork(true)).Repeat.Once();

            repository.ReplayAll();

            var obj = Resolver.Get<UnitOfWorkTestClass>();
            obj.DoAbortTransactionException();
        }

        private void UnitySetup()
        {
            container = new UnityContainer();
            container.AddNewExtension<Interception>();

            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));

            container.Configure<Interception>()
                .AddPolicy("UnitOfWork")
                .AddMatchingRule(new UnitOfWorkMatchingRule())
                .AddCallHandler<UnitOfWorkInterceptor>(
                    new ContainerControlledLifetimeManager(),
                    new InjectionConstructor());

            container.RegisterType<UnitOfWorkTestClass, UnitOfWorkTestClass>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<VirtualMethodInterceptor>());

            container.RegisterInstance(transactionManager);
        }

        public class UnitOfWorkTestClass
        {
            [UnitOfWork]
            public virtual void Do()
            {
            }

            [UnitOfWork(CloseSessions = false)]
            public virtual void DoWithoutCloseSessions()
            {
            }

            [UnitOfWork(IsExceptionSilent = true)]
            public virtual void DoExceptionSilent()
            {
                throw new Exception("Error");
            }

            [UnitOfWork]
            public virtual void DoAbortTransactionException()
            {
                throw new AbortTransactionException();
            }

            [UnitOfWork]
            public virtual void ThrowException()
            {
                throw new Exception("Error");
            }
        }
    }
}
