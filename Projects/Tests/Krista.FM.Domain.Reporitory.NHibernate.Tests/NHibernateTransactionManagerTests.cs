using NHibernate;
using NUnit.Framework;
using Rhino.Mocks;

namespace Krista.FM.Domain.Reporitory.NHibernate.Tests
{
    [TestFixture]
    public class NHibernateTransactionManagerTests
    {
        private MockRepository repository;
        private ISession session;
        private ITransaction transaction;
        private IUnitOfWorkSessionStorage sessionStorage;

        [SetUp]
        public void SetUp()
        {
            repository = new MockRepository();
            session = repository.DynamicMock<ISession>();
            transaction = repository.DynamicMock<ITransaction>();
            sessionStorage = repository.DynamicMock<IUnitOfWorkSessionStorage>();

            NHibernateSession.Storage = sessionStorage;
        }

        [TearDown]
        public void TearDown()
        {
            repository.VerifyAll();
        }

        [Test]
        public void SuccessStoryTest()
        {
            Expect.Call(sessionStorage.Session).Return(session);
            Expect.Call(session.Transaction).Return(transaction);
            Expect.Call(transaction.IsActive).Return(false).Repeat.Once();
            Expect.Call(transaction.Begin);
            Expect.Call(sessionStorage.Session).Return(session);
            Expect.Call(session.Transaction).Return(transaction);
            Expect.Call(transaction.IsActive).Return(true).Repeat.Once();
            Expect.Call(transaction.Commit);

            repository.ReplayAll();

            var tm = new NHibernateTransactionManager();

            tm.PushTransaction(null, null);
            Assert.AreEqual(1, tm.TransactionDepth);
            
            tm.CommitTransaction(null, null);
            tm.PopTransaction(null, null);

            Assert.AreEqual(0, tm.TransactionDepth);
        }

        [Test]
        public void SuccessNestedStoryTest()
        {
            Expect.Call(sessionStorage.Session).Return(session);
            Expect.Call(session.Transaction).Return(transaction);
            Expect.Call(transaction.IsActive).Return(false).Repeat.Once();
            Expect.Call(transaction.Begin);

            Expect.Call(sessionStorage.Session).Return(session);
            Expect.Call(session.Transaction).Return(transaction);
            Expect.Call(transaction.IsActive).Return(false).Repeat.Once();
            Expect.Call(transaction.Begin);

            Expect.Call(sessionStorage.Session).Return(session);
            Expect.Call(session.Transaction).Return(transaction);

            Expect.Call(sessionStorage.Session).Return(session);
            Expect.Call(session.Transaction).Return(transaction);
            Expect.Call(transaction.IsActive).Return(true).Repeat.Once();
            Expect.Call(transaction.Commit);

            repository.ReplayAll();

            var tm = new NHibernateTransactionManager();

            tm.PushTransaction(null, null);
            Assert.AreEqual(1, tm.TransactionDepth);

                tm.PushTransaction(null, null);
                Assert.AreEqual(2, tm.TransactionDepth);

                tm.CommitTransaction(null, null);
                tm.PopTransaction(null, null);

                Assert.AreEqual(1, tm.TransactionDepth);

            tm.CommitTransaction(null, null);
            tm.PopTransaction(null, null);

            Assert.AreEqual(0, tm.TransactionDepth);
        }

        [Test]
        public void FailureStoryTest()
        {
            Expect.Call(sessionStorage.Session).Return(session);
            Expect.Call(session.Transaction).Return(transaction);
            Expect.Call(transaction.IsActive).Return(false).Repeat.Once();
            Expect.Call(transaction.Begin);
            Expect.Call(sessionStorage.Session).Return(session);
            Expect.Call(session.Transaction).Return(transaction);
            Expect.Call(transaction.IsActive).Return(true).Repeat.Once();
            Expect.Call(transaction.Rollback);

            repository.ReplayAll();

            var tm = new NHibernateTransactionManager();

            tm.PushTransaction(null, null);
            Assert.AreEqual(1, tm.TransactionDepth);
            
            tm.RollbackTransaction(null, null);
            tm.PopTransaction(null, null);

            Assert.AreEqual(0, tm.TransactionDepth);
        }

        [Test]
        public void FailureNestedStoryTest()
        {
            Expect.Call(sessionStorage.Session).Return(session);
            Expect.Call(session.Transaction).Return(transaction);
            Expect.Call(transaction.IsActive).Return(false).Repeat.Once();
            Expect.Call(transaction.Begin);

            Expect.Call(sessionStorage.Session).Return(session);
            Expect.Call(session.Transaction).Return(transaction);
            Expect.Call(transaction.IsActive).Return(false).Repeat.Once();
            Expect.Call(transaction.Begin);

            Expect.Call(sessionStorage.Session).Return(session);
            Expect.Call(session.Transaction).Return(transaction);
            Expect.Call(transaction.IsActive).Return(true).Repeat.Once();
            Expect.Call(transaction.Rollback);

            Expect.Call(sessionStorage.Session).Return(session);
            Expect.Call(session.Transaction).Return(transaction);

            repository.ReplayAll();

            var tm = new NHibernateTransactionManager();

            tm.PushTransaction(null, null);
            Assert.AreEqual(1, tm.TransactionDepth);

            tm.PushTransaction(null, null);
            Assert.AreEqual(2, tm.TransactionDepth);

            tm.RollbackTransaction(null, null);
            tm.PopTransaction(null, null);

            Assert.AreEqual(1, tm.TransactionDepth);

            tm.RollbackTransaction(null, null);
            tm.PopTransaction(null, null);

            Assert.AreEqual(0, tm.TransactionDepth);
        }
    }
}
